"""Bounded lane-local boot qualification while the replay harness owns its database."""
from __future__ import annotations
import ctypes
from ctypes import wintypes
import hashlib
import importlib.util
import json
import math
import os
from pathlib import Path
import re
import socket
import subprocess
import sys
import time
import traceback
import uuid
import xml.etree.ElementTree as ET

REPO = Path(os.environ['FUTUREMUD_PREPARED_REPLAY_REPOSITORY']).resolve()
ROOT = REPO.parent
OUTPUT = Path(os.environ['FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT']).resolve()
CONNECTION = os.environ['FUTUREMUD_PREPARED_REPLAY_BOOT_CONNECTION']
PASSWORD = os.environ['FUTUREMUD_PREPARED_REPLAY_LOGIN_PASSWORD']
assert REPO.parent == ROOT and REPO.name == 'FutureMUD'
CHECKED_IN_SCRIPT = REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedCatalogueBootSmoke.py'
SCRIPT_PATH = Path(__file__).resolve()
require_script_path = SCRIPT_PATH == CHECKED_IN_SCRIPT or SCRIPT_PATH == ROOT / 'PreparedCatalogueBootSmoke.py'
assert require_script_path, 'Boot script must run from its checked-in path or the exact parent handoff path.'
assert OUTPUT.parent == ROOT and OUTPUT.name.startswith('prepared-catalogue-boot-native')
assert not OUTPUT.exists(), 'Refusing to reuse a boot evidence directory.'
OUTPUT.mkdir()
spec = importlib.util.spec_from_file_location('lane_mud_session', REPO / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
assert spec.loader
spec.loader.exec_module(helper)
campaign_path = REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedStockCampaign.py'
campaign_spec = importlib.util.spec_from_file_location('prepared_stock_campaign', campaign_path)
campaign = importlib.util.module_from_spec(campaign_spec)
assert campaign_spec.loader
campaign_spec.loader.exec_module(campaign)
SECRETS = [CONNECTION, PASSWORD]
BOOT_SCOPE = os.environ.get('FUTUREMUD_CATALOGUE_BOOT_SCOPE', 'catalogue')
assert BOOT_SCOPE in ('catalogue', 'attunement-builder', 'attunement-stock'), 'Unknown catalogue boot scope.'
assert BOOT_SCOPE != 'attunement-builder' or os.environ.get('FUTUREMUD_S4_ATTUNEMENT_BUILDER_SMOKE') == '1', \
    'The Attunement-only lane requires its explicit builder assertions.'
assert BOOT_SCOPE != 'attunement-stock' or os.environ.get('FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE') == '1', \
    'The stock Attunement lane requires explicit paid stock assertions.'

def redact(text): return helper.redact(str(text), SECRETS)
def sha(path): return hashlib.sha256(Path(path).read_bytes()).hexdigest().upper()
def require(value, message):
    if not value: raise RuntimeError(message)

# Connection is supplied privately by the live TestDatabase owner. Never use tester defaults.
fields = {}
for field in CONNECTION.split(';'):
    if '=' in field:
        key, value = field.split('=', 1)
        fields[key.strip().lower()] = value.strip().strip('"\'')
host = fields.get('server', fields.get('host'))
port = int(fields['port'])
database = fields['database']
user = fields.get('user id', fields.get('uid', fields.get('user')))
sql_password = fields.get('password', fields.get('pwd', ''))
SECRETS.append(sql_password)
require(host == '127.0.0.1' and re.fullmatch(r'futuremud_land_\d{14}_[a-f0-9]+', database), 'Boot refuses a non-owned target.')
require(port == int(os.environ['FUTUREMUD_OWNED_MYSQL_PORT']), 'Owned port mismatch.')
expected_uuid = os.environ['FUTUREMUD_OWNED_MYSQL_UUID']
expected_datadir = os.path.normcase(os.path.normpath(os.environ['FUTUREMUD_OWNED_MYSQL_DATADIR']))
mysql = Path('C:/Program Files/MySQL/MySQL Server 8.0/bin/mysql.exe')
require(mysql.is_file() and user, 'Owned SQL executable/user unavailable.')
marker = None
assertions = []

def batch_cell(value):
    # mysql --batch escapes separators and backslashes; decode each cell only after row splitting.
    escapes = {'n':'\n', 'r':'\r', 't':'\t', '0':'\0', '\\':'\\'}
    return re.sub(r'\\([nrt0\\])', lambda match: escapes[match.group(1)], value)

def sql(statement, hex_columns=(), context=''):
    global marker
    env = os.environ.copy()
    env['MYSQL_PWD'] = sql_password
    command = [str(mysql), '--no-defaults', '--protocol=tcp', '--host=127.0.0.1', f'--port={port}',
               f'--user={user}', f'--database={database}', '--batch', '--skip-column-names',
               '--execute=SELECT @@server_uuid, @@port, @@datadir; SELECT RunToken FROM __gathering_harness_ownership; ' + statement]
    query_hash = hashlib.sha256(statement.encode('utf-8')).hexdigest().upper()
    def execute():
        result = subprocess.run(command, env=env, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                                text=True, encoding='utf-8', errors='replace', timeout=40,
                                creationflags=subprocess.CREATE_NO_WINDOW)
        require(result.returncode == 0, 'Owned SQL failed: ' + redact(result.stderr))
        return result, result.stdout.rstrip('\r\n').splitlines()
    result, lines = execute()
    if len(lines) < 2:
        anomaly = {'context': context, 'query_sha256': query_hash,
                   'first_rows': len(lines), 'first_stdout_chars': len(result.stdout),
                   'first_stdout_sha256': hashlib.sha256(result.stdout.encode('utf-8')).hexdigest().upper(),
                   'first_stderr': redact(result.stderr)[:240]}
        receipt.setdefault('sql_output_anomalies', []).append(anomaly)
        # Retry only a single read-only SELECT. Mutations always fail closed on a missing marker.
        if statement.lstrip().upper().startswith('SELECT ') and ';' not in statement:
            result, lines = execute()
            anomaly.update(retry_rows=len(lines), retry_stdout_chars=len(result.stdout),
                           retry_stdout_sha256=hashlib.sha256(result.stdout.encode('utf-8')).hexdigest().upper(),
                           retry_stderr=redact(result.stderr)[:240])
    require(len(lines) >= 2,
            f'Missing owned SQL descriptor/marker (context={context}, query_sha256={query_hash}, rows={len(lines)}, stdout_chars={len(result.stdout)}, stderr={redact(result.stderr)[:240]}).')
    descriptor = lines[0].split('\t')
    require(len(descriptor) == 3 and descriptor[0] == expected_uuid and int(descriptor[1]) == port and
            os.path.normcase(os.path.normpath(descriptor[2].replace('/', '\\'))) == expected_datadir,
            'Exact owned MySQL descriptor mismatch.')
    require(re.fullmatch(r'[a-f0-9]{40}', lines[1]), 'Invalid owned database marker.')
    if marker is None: marker = lines[1]
    require(lines[1] == marker, 'Owned database marker changed.')
    rows = [[batch_cell(cell) for cell in line.split('\t')] for line in lines[2:]]
    for row in rows:
        for column in hex_columns:
            row[column] = bytes.fromhex(row[column]).decode('utf-8')
    return rows

class IoCounters(ctypes.Structure):
    _fields_ = [(name, ctypes.c_ulonglong) for name in ('ReadOperationCount','WriteOperationCount','OtherOperationCount','ReadTransferCount','WriteTransferCount','OtherTransferCount')]
class BasicLimits(ctypes.Structure):
    _fields_ = [('PerProcessUserTimeLimit',ctypes.c_longlong),('PerJobUserTimeLimit',ctypes.c_longlong),('LimitFlags',wintypes.DWORD),
                ('MinimumWorkingSetSize',ctypes.c_size_t),('MaximumWorkingSetSize',ctypes.c_size_t),('ActiveProcessLimit',wintypes.DWORD),
                ('Affinity',ctypes.c_size_t),('PriorityClass',wintypes.DWORD),('SchedulingClass',wintypes.DWORD)]
class ExtendedLimits(ctypes.Structure):
    _fields_ = [('BasicLimitInformation',BasicLimits),('IoInfo',IoCounters),('ProcessMemoryLimit',ctypes.c_size_t),('JobMemoryLimit',ctypes.c_size_t),
                ('PeakProcessMemoryUsed',ctypes.c_size_t),('PeakJobMemoryUsed',ctypes.c_size_t)]
class Accounting(ctypes.Structure):
    _fields_ = [('TotalUserTime',ctypes.c_longlong),('TotalKernelTime',ctypes.c_longlong),('ThisPeriodTotalUserTime',ctypes.c_longlong),
                ('ThisPeriodTotalKernelTime',ctypes.c_longlong),('TotalPageFaultCount',wintypes.DWORD),('TotalProcesses',wintypes.DWORD),
                ('ActiveProcesses',wintypes.DWORD),('TotalTerminatedProcesses',wintypes.DWORD)]
kernel = ctypes.WinDLL('kernel32', use_last_error=True)
kernel.CreateJobObjectW.argtypes = [ctypes.c_void_p,wintypes.LPCWSTR]
kernel.CreateJobObjectW.restype = wintypes.HANDLE
kernel.SetInformationJobObject.argtypes = [wintypes.HANDLE,ctypes.c_int,ctypes.c_void_p,wintypes.DWORD]
kernel.AssignProcessToJobObject.argtypes = [wintypes.HANDLE,wintypes.HANDLE]
kernel.QueryInformationJobObject.argtypes = [wintypes.HANDLE,ctypes.c_int,ctypes.c_void_p,wintypes.DWORD,ctypes.c_void_p]
kernel.TerminateJobObject.argtypes = [wintypes.HANDLE,wintypes.UINT]
kernel.CloseHandle.argtypes = [wintypes.HANDLE]

class OwnedJob:
    def __init__(self):
        self.handle = kernel.CreateJobObjectW(None, None)
        require(self.handle, 'Cannot create owned MUD process job.')
        limit = ExtendedLimits()
        limit.BasicLimitInformation.LimitFlags = 0x2000 # KILL_ON_JOB_CLOSE
        if not kernel.SetInformationJobObject(self.handle,9,ctypes.byref(limit),ctypes.sizeof(limit)):
            kernel.CloseHandle(self.handle)
            raise ctypes.WinError(ctypes.get_last_error())
    def assign(self, process):
        require(kernel.AssignProcessToJobObject(self.handle,wintypes.HANDLE(int(process._handle))), 'Cannot assign owned MUD process job.')
    def active(self):
        data = Accounting()
        require(kernel.QueryInformationJobObject(self.handle,1,ctypes.byref(data),ctypes.sizeof(data),None), 'Cannot inspect owned MUD job.')
        return data.ActiveProcesses
    def stop(self):
        if self.active(): require(kernel.TerminateJobObject(self.handle,1), 'Cannot terminate owned MUD job.')
        deadline = time.monotonic() + 20
        while self.active() and time.monotonic() < deadline: time.sleep(0.1)
        require(self.active() == 0, 'Owned MUD job still has active processes.')
    def close(self):
        self.stop()
        require(kernel.CloseHandle(self.handle), 'Cannot close owned MUD job.')

def free_port():
    with socket.socket() as listener:
        listener.bind(('127.0.0.1',0))
        return listener.getsockname()[1]

processes = []
jobs = []
collectors = []
receipt = {'status':'FAIL','database':database,'mysql_endpoint':f'127.0.0.1:{port}',
           'repository':str(REPO),'runtime_paths':[],'processes':processes,'assertions':assertions,
           'owned_mud_processes_stopped':False,'external_email_disabled':False,'discord_loopback_only':False,
           'limitations':['S3 catalogue first boot and cold restart smoke only; controller authority has its separate native fixture, and S4 hydration and S5 production calibration remain separate gates.']}
discord_socket = socket.socket()
started = time.monotonic()

def character_state():
    rows = sql("SELECT a.Id,c.Id,c.Name,c.Location,IFNULL(DATE_FORMAT(c.LastLoginTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL'),IFNULL(DATE_FORMAT(c.LastLogoutTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') FROM Accounts a JOIN Characters c ON c.AccountId=a.Id WHERE a.Name='Admin' AND c.IsAdminAvatar=1 ORDER BY c.Id")
    require(len(rows) == 1 and len(rows[0]) == 6, 'Expected one legitimately seeded Admin avatar.')
    return rows[0]

def catalogue_record(key):
    rows = sql("SELECT StableKey,EntityType,LogicalId FROM SeederManagedRecords WHERE StableKey='" + key.replace("'", "''") + "' AND Retired=0")
    require(len(rows) == 1 and len(rows[0]) == 3 and rows[0][2].isdigit(), 'Missing or ambiguous stable catalogue record: ' + key)
    return rows[0]

def assert_catalogue_inventory():
    keys = ['arm.gather.component_blank', 'arm.item.component_blank', 'arm.material.abjuration.rank0.recipe',
            'arm.material.abjuration.rank0', 'arm.item.food.bread', 'arm.item.drink_container',
            'arm.liquid.wine', 'arm.support.component_crafting.skill', 'arm.npc.earth_mount.g1',
            'arm.npc.air_guardian.g7', 'arm.item.flame_knife.g1', 'arm.item.flame_knife.g1.component',
            'arm.item.flame_knife.g1.weapon']
    records = {key: catalogue_record(key) for key in keys}
    require(records['arm.gather.component_blank'][1] == 'Foragable', 'Component gathering stable key is not a native Foragable.')
    require(records['arm.item.component_blank'][1] == 'GameItemProto' and
            records['arm.material.abjuration.rank0.recipe'][1] == 'Craft' and
            records['arm.material.abjuration.rank0'][1] == 'GameItemProto', 'Ranked recipe/input/output have unexpected native types.')
    require(records['arm.item.food.bread'][1] == 'GameItemProto' and records['arm.item.drink_container'][1] == 'GameItemProto',
            'Food or bottle stable key is not a native item prototype.')
    require(records['arm.npc.earth_mount.g1'][1] == 'NpcTemplate' and records['arm.npc.air_guardian.g7'][1] == 'NpcTemplate',
            'Grade-specific creature keys are not native NPC templates.')
    for key in ('arm.item.component_blank', 'arm.material.abjuration.rank0', 'arm.item.food.bread', 'arm.item.drink_container', 'arm.item.flame_knife.g1'):
        proto = records[key][2]
        require(len(sql(f'SELECT Id FROM GameItemProtos WHERE Id={proto}')) == 1, 'Stable item row does not resolve to one prototype: ' + key)
    forage = sql(f"SELECT ItemProtoId FROM Foragables WHERE Id={records['arm.gather.component_blank'][2]}")
    require(len(forage) == 1 and forage[0][0] == records['arm.item.component_blank'][2], 'Native component-blank forage does not yield the exact managed GameItemProto.')
    recipe_id = records['arm.material.abjuration.rank0.recipe'][2]
    recipe_inputs = sql(f'SELECT HEX(Definition) FROM CraftInputs WHERE CraftId={recipe_id}', (0,))
    recipe_products = sql(f'SELECT HEX(Definition) FROM CraftProducts WHERE CraftId={recipe_id}', (0,))
    require(any(int(ET.fromstring(row[0]).findtext('TargetItemId', '0')) == int(records['arm.item.component_blank'][2]) for row in recipe_inputs),
            'Exact native rank-0 recipe does not consume the managed component blank.')
    require(any(int(ET.fromstring(row[0]).findtext('ProductProducedId', '0')) == int(records['arm.material.abjuration.rank0'][2]) for row in recipe_products),
            'Exact native rank-0 recipe does not produce its managed ranked material prototype.')
    assertions.append('Exact stable records resolved for gathering yield, recipe/product, provision, vessel, grade 1/7 creatures and melee prototype')
    return records

def item_command_reference(proto_id):
    # Item_Load explicitly accepts the prototype ID. Use the proven managed link,
    # avoiding nullable UniqueName and non-unique display-name lookup entirely.
    rows = sql(f'SELECT Id FROM GameItemProtos WHERE Id={proto_id}')
    require(len(rows) == 1 and rows[0][0] == str(proto_id), 'Exact native item prototype is missing.')
    return str(proto_id)

def attunement_builder_smoke(session, phase):
    # This authors only a disposable builder fixture. It is never a qualified stock admission.
    if phase == 'first':
        school = catalogue_record('arm.school.water')
        require(school[1] == 'MagicSchool', 'Attunement fixture requires the exact declared native school binding.')
        created = session.send(f'magic spell new "S4 Attunement Adapter Fixture" {school[2]}', read_seconds=1)
        require('You create a new magic spell' in created, 'Native Attunement builder fixture creation failed.')
        configured = session.send('magic spell set effect add readattunement', read_seconds=1)
        require('readattunement' in configured.lower() or 'Read Attunement' in configured,
                'Native builder did not accept the Read Attunement effect.')
        session.send('impdebug flush', read_seconds=2)
        rows = sql("SELECT Id,HEX(Definition) FROM MagicSpells WHERE Name='S4 Attunement Adapter Fixture'", (1,))
        require(len(rows) == 1, 'Expected exactly one disposable Attunement builder fixture.')
        root = ET.fromstring(rows[0][1])
        require(len(root.findall("Effects/Effect[@type='readattunement']")) == 1,
                'Native builder save did not persist the exact effect discriminator.')
        receipt['attunement_builder_fixture'] = {'spell':int(rows[0][0]), 'definition':rows[0][1]}
        assertions.append('S4 adapter: native builder accepted and persisted one readattunement effect')
    else:
        fixture = receipt['attunement_builder_fixture']
        shown = session.send(f"magic spell show {fixture['spell']}", read_seconds=1)
        require('Read Attunement' in shown, 'Cold native spell loader did not restore Attunement template.')
        rows = sql(f"SELECT HEX(Definition) FROM MagicSpells WHERE Id={fixture['spell']}", (0,))
        require(len(rows) == 1 and rows[0][0] == fixture['definition'], 'Cold builder fixture definition changed.')
        assertions.append('S4 adapter: cold native loader showed the saved Read Attunement template unchanged')

def stock_definition_loading(session, phase, data, mapped):
    reference_schema = campaign.reference_schema(data['typed_reference_schema'])
    s = int(data['spell'])
    stock_spells = data.get('stock_spells', [])
    require(stock_spells and len({entry['key'] for entry in stock_spells}) == len(stock_spells), 'Missing or duplicate canonical definition inputs.')
    claimed = sql("SELECT StableKey,LogicalId FROM SeederManagedRecords WHERE Seeder='ArmageddonMagicSeeder' AND Module='stock-spells' AND EntityType='MagicSpell' AND Retired=0 ORDER BY StableKey")
    require([(row[0], int(row[1])) for row in claimed] == sorted((entry['key'], int(entry['id'])) for entry in stock_spells),
            'Prepared stock list does not cover every installed managed definition exactly once.')
    require(any(entry['key'] == 'arm.spell.read_attunement' and int(entry['id']) == s for entry in stock_spells), 'Original Read identity is missing.')
    resolved_refs = set()
    for entry in stock_spells:
        _, refs = campaign.definition_contract(entry)
        actual = sql(f"SELECT HEX(Definition) FROM MagicSpells WHERE Id={int(entry['id'])}", (0,))
        require(len(actual) == 1 and actual[0][0] == entry['definition'],
                'Canonical definition changed during native loader validation: ' + entry['key'])
        native = sql(mapped(f"SELECT MagicSchoolId,CastingTraitDefinitionId,EffectDurationExpressionId,SpellKnownProgId,IFNULL(ResistingTraitDefinitionId,0) FROM MagicSpells WHERE Id={int(entry['id'])}"))
        require(native == [[str(entry[field]) for field in ('school', 'trait', 'duration_expression', 'known_prog', 'resisting_trait')]], 'Native spell metadata drift: ' + entry['key'])
        for expression, formula in ((entry['duration_expression'], entry['duration_formula']), (entry['cost_expression'], entry['cost_formula'])):
            require(sql(mapped(f'SELECT HEX(Expression) FROM TraitExpressions WHERE Id={int(expression)}'), (0,)) == [[formula]], 'Native duration/cost formula drift: ' + entry['key'])
        for kind, identity in refs:
            if (kind, identity) in resolved_refs: continue
            require(kind in reference_schema, 'Missing typed EF reference mapping: ' + kind)
            binding = reference_schema[kind]; table = binding['table']; column = binding['id_column']
            require(sql(f'SELECT DISTINCT `{column}` FROM `{table}` WHERE `{column}`={identity}',
                        context=entry['key'] + ':' + kind + ':' + str(identity)) == [[str(identity)]],
                    'Unresolved exact typed native reference: ' + kind + ':' + str(identity))
            resolved_refs.add((kind, identity))
        shown = session.send(f"magic spell show {int(entry['id'])}", read_seconds=.2)
        def ready(text):
            return (entry['name'] in text and 'Building Error' not in text and 'Controlled Power Profile' in text and
                    all(f'Grade {grade}:' in text for grade in range(1, 8)))
        if not ready(shown):
            # A busy server may answer after the short fast-path read. Continue
            # reading this command's response without issuing the command twice.
            shown += session.read_for(5.0)
        require(ready(shown),
                'Native canonical spell loader/readiness failed: ' + entry['key'] + ': ' + redact(shown))
        receipt.setdefault('stock_definition_loader', {}).setdefault(phase, {})[entry['key']] = {
            'id': entry['id'], 'definition_sha256': hashlib.sha256(entry['definition'].encode('utf-8')).hexdigest(),
            'typed_references': refs, 'shown': redact(shown), 'coverage': 'native-definition-readiness'}
    assertions.append(f'{phase}: all {len(stock_spells)} installed canonical definitions retained exact identity, cost/duration formulas, grades 1..7 and typed references and loaded ready')


def attunement_stock_smoke(session, phase):
    prepared = Path(os.environ['FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT']).resolve()
    require(prepared.parent == ROOT and prepared.name.startswith('prepared-stock-attunement-input_') and
            prepared.suffix == '.json', 'Stock Attunement requires its explicit local fixture input.')
    data = json.loads(prepared.read_text(encoding='utf-8'))
    pin = {'path': str(prepared), 'sha256': sha(prepared)}
    require(receipt.setdefault('stock_input_pin', pin) == pin, 'Prepared stock input changed across cold restart.')
    preparation = data['prepared_source']
    expected_source = REPO / 'tests/ArmageddonPreparedSeederNativeHarness/Program.StockAttunement.cs'
    require(Path(preparation['path']).resolve() == expected_source and sha(expected_source) == preparation['sha256'], 'Stock C# preparation source differs from its pin.')
    require(receipt.setdefault('stock_preparation_source', preparation) == preparation, 'Stock preparation source pin drift.')
    reference_schema = campaign.reference_schema(data['typed_reference_schema'])
    schema = data['sql_schema']
    require(isinstance(schema, list) and len(schema) >= 16, 'Missing bounded EF SQL schema contract.')
    identifiers = {}
    for table in schema:
        for alias, name in [(table['alias'], table['table'])] + [(x['alias'], x['column']) for x in table['columns']]:
            require(isinstance(name, str) and re.fullmatch(r'[A-Za-z_][A-Za-z_0-9]*', name), 'Unsafe EF SQL identifier.')
            quoted = '`' + name + '`'
            require(alias not in identifiers or identifiers[alias] == quoted, 'Ambiguous EF SQL identifier: ' + alias)
            identifiers[alias] = quoted

    def mapped(statement):
        # All names are declared DbSet/property aliases from the existing typed EF mapping helper.
        return re.sub(r"'(?:''|[^'])*'|[A-Za-z_][A-Za-z_0-9]*",
                      lambda match: match[0] if match[0].startswith("'") else identifiers.get(match[0], match[0]), statement)
    c, s, r, cap, trait = (int(data[k]) for k in ('character', 'spell', 'resource', 'capability', 'trait'))
    require(c == int(character_state()[1]), 'Stock qualification actor differs from the exact seeded Admin avatar.')
    row = catalogue_record('arm.spell.read_attunement')
    require(row[1] == 'MagicSpell' and int(row[2]) == s, 'Paid test does not use the installed canonical spell ID.')
    unchanged = sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s}', (0,))
    require(len(unchanged) == 1 and unchanged[0][0] == data['definition'], 'Qualification changed the installed stock spell definition.')
    for package_cap in data['package_capabilities']:
        definition = sql(f'SELECT HEX(Definition) FROM MagicCapabilities WHERE Id={int(package_cap)}', (0,))
        casting = ET.fromstring(definition[0][0]).find('Casting')
        require(casting is not None and casting.get('enabled') == 'false',
                'A partial package roster was enabled for qualification.')

    if os.environ.get('FUTUREMUD_S6_INTEGRATION_SMOKE_ONLY') == '1':
        stock_definition_loading(session, phase, data, mapped)
        module_spec = importlib.util.spec_from_file_location('s6_integration_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS6IntegrationSmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    if os.environ.get('FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY') == '1':
        module_spec = importlib.util.spec_from_file_location('s5_progression_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS5ProgressionSmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    if os.environ.get('FUTUREMUD_S5_DOSE_SMOKE_ONLY') == '1':
        module_spec = importlib.util.spec_from_file_location('s5_dose_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS5DoseSmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    if os.environ.get('FUTUREMUD_S4_FAMILY_SMOKE_ONLY') == '1':
        module_spec = importlib.util.spec_from_file_location('family_acceptance_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS4FamilySmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    if os.environ.get('FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY') == '1':
        module_spec = importlib.util.spec_from_file_location('focus_device_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedFocusDeviceSmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    if os.environ.get('FUTUREMUD_S4_REMAINING_SMOKE_ONLY') == '1':
        module_spec = importlib.util.spec_from_file_location('remaining_stock_smoke', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedRemainingStockSmoke.py')
        module = importlib.util.module_from_spec(module_spec)
        module_spec.loader.exec_module(module)
        module.run(globals(), session, phase, data, mapped)
        return

    def flush():
        require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.3), 'S4 save flush did not confirm.')

    def state():
        rows = sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={c} AND MagicResourceId={r}),0), '
                   f'IFNULL((SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={c} AND MagicSpellId={s}),0), '
                   f'IFNULL((SELECT Value FROM CharacterTraits WHERE CharacterId={c} AND TraitDefinitionId={trait}),0), '
                   f'(SELECT COUNT(*) FROM CharacterCastingEnrolments WHERE CharacterId={c} AND MagicCapabilityId={cap}), HEX(EffectData) FROM Characters WHERE Id={c}'), (4,))
        require(len(rows) == 1 and len(rows[0]) == 5, 'S4 actor state is missing.')
        balance, grade, raw, enrol, saved = rows[0]
        parents = []
        for wrapper in ET.fromstring(saved).iter('Effect'):
            if wrapper.findtext('Type') != 'MagicSpellParent': continue
            inner = wrapper.find('Effect')
            if inner is None or inner.findtext('Spell') != str(s): continue
            children = inner.find('Children')
            parents.append({'identity':inner.findtext('Identity'), 'remaining_ms':int(wrapper.findtext('Remaining', '0')),
                            'children': [child.findtext('Type') for child in children]})
        return {'balance':float(balance), 'grade':int(grade), 'raw':float(raw), 'enrolments':int(enrol), 'parents':parents}

    def operations():
        return sql(mapped(f'SELECT HEX(Id),HEX(Stage),HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={c} AND MagicSpellId={s} ORDER BY CreatedUtc,Id'), (0,1,2))

    def verify_overlay(output):
        expected = {cap: data['capability_name']}
        candidates = {row['key']: row for row in data.get('representative_campaign', {}).get('candidates', [])}
        for selected in receipt.get('representative_campaign', {}).get('selected', []):
            if selected['coverage'] != 'direct-paid-route': continue
            candidate = candidates[selected['key']]
            require(selected.get('saved_state', {}).get('enrolments') == 1, 'Read roster sample lacks completed native enrolment evidence.')
            expected[int(candidate['capability'])] = candidate['capability_name']
        rows = sql(mapped(f'SELECT e.MagicCapabilityId,m.Name,e.CompletedStartingGrantVersion FROM CharacterCastingEnrolments e '
                          f'JOIN MagicCapabilities m ON m.Id=e.MagicCapabilityId WHERE e.CharacterId={c} AND e.CompletedStartingGrantVersion>0'))
        require(len(rows) == len(expected) and {int(row[0]): row[1] for row in rows} == expected and all(int(row[2]) > 0 for row in rows),
                'Read learned roster differs from the exact Read and actually enrolled campaign capabilities.')
        campaign.verify_read_overlay(output, list(expected.values()))
        receipt.setdefault('stock_attunement_learned_roster', {})[phase] = [{'capability': identity, 'name': name} for identity, name in sorted(expected.items())]

    flush()
    initial = state()
    if phase == 'first':
        require(initial == {'balance':0.0, 'grade':0, 'raw':0.0, 'enrolments':0, 'parents':[]}, 'S4 preparation pre-granted or refilled a player.')
        session.send(f"givemerit me {data['merit']}", read_seconds=.3)
        enrolled = session.send(f'magic casting enrol {c} {cap} explicit disposable S4 stock qualification', read_seconds=.5)
        flush()
        current = state()
        require(current['balance'] == 0 and current['grade'] == 1 and current['raw'] == 100 and current['enrolments'] == 1,
                'Native S4 enrolment did not grant grade one with the fixture opening and zero reserve: ' + redact(enrolled))
        assertions.append('S4 stock: native merit attachment and enrolment acquired the real canonical Read Attunement at grade one without reserve refill')
    else:
        saved = receipt['stock_attunement_first']
        require(all(initial[k] == saved[k] for k in ('balance', 'grade', 'raw', 'enrolments')), 'Cold S4 state was refilled or reacquired.')
        require(len(initial['parents']) == 1 and initial['parents'][0]['identity'] == saved['parents'][0]['identity'] and
                initial['parents'][0]['children'] == ['SpellReadAttunement'] and
                0 < initial['parents'][0]['remaining_ms'] <= saved['parents'][0]['remaining_ms'], 'Cold S4 parent/child identity or remaining duration was not retained.')
        assertions.append('S4 stock: cold restart retained paid reserve, grade, skill, completed enrolment and exact parent/child identity with decreasing lifetime')

    def verify_refusal():
        current = state()
        require(current['balance'] == 0, 'S4 depletion control must use a saved zero reserve.')
        old = operations()
        refused = session.send(f'armwater cast "Read Attunement" grade 1 on self via {cap}', read_seconds=.3)
        flush()
        after_refusal = state()
        require('Insufficient' in refused and operations() == old and
                all(after_refusal[k] == current[k] for k in ('balance', 'grade', 'raw', 'enrolments')) and
                [(x['identity'], x['children']) for x in after_refusal['parents']] == [(x['identity'], x['children']) for x in current['parents']],
                'Unfunded S4 cast changed state or produced an operation: ' + redact(refused))
        assertions.append(phase + ': S4 stock unfunded cast refused with no operation, debit, learning or overlay side effect')

    if phase == 'cold':
        # The grade-one overlay lives for 60 seconds. Inspect it before the
        # 100-definition loader sweep, which can outlast that live effect.
        verify_refusal()
        looked = session.send('look me', read_seconds=.5)
        if 'Assigned elemental affinities:' not in looked or 'Learned magical affiliations:' not in looked:
            looked += session.read_for(2.0)
        verify_overlay(looked)
        assertions.append('S4 stock: cold visible self overlay separated unassigned affinity from completed learned affiliation')

    stock_definition_loading(session, phase, data, mapped)
    stock_campaign_smoke(session, phase, data, mapped)

    if phase == 'cold':
        mark_inherited_read('cold')
        return

    # A depleted reserve refuses before a roll/payment operation or overlay replacement.
    verify_refusal()

    session.send(f'armwater gather {cap} draw 100', read_seconds=.2)
    deadline = time.monotonic() + 15
    while time.monotonic() < deadline:
        session.read_for(.5); flush()
        if state()['balance'] == 100: break
    require(state()['balance'] == 100, 'S4 finite paid Self gathering did not fund exactly100 units.')
    assertions.append('S4 stock: timed native Self gathering funded the real route reserve without SQL credit')
    # At controlled grade one the approved source curve costs50. Two funded attempts is a finite RNG bound.
    success = False
    for attempt in range(2):
        prior = state(); old = operations()
        output = session.send(f'armwater cast "Read Attunement" grade 1 on self via {cap}', read_seconds=.5)
        flush()
        added = [row for row in operations() if row[0] not in {x[0] for x in old}]
        require(len(added) == 1 and added[0][1] == 'Completed', 'S4 cast did not produce one completed paid operation: ' + redact(output))
        payload = ET.fromstring(added[0][2]); costs = payload.findall('Cost')
        require(payload.get('grade') == '1' and payload.get('controlledGrade') == '1' and
                len(costs) == 1 and costs[0].get('resource') == str(r) and float(costs[0].get('amount')) == 50,
                'S4 grade/source/reserve cost mapping differs from approved efficiency.')
        after = state()
        require(prior['balance'] - after['balance'] == 50 and after['grade'] == 1 and after['enrolments'] == 1, 'S4 invocation did not persist exactly one debit.')
        receipt.setdefault('stock_attunement_paid_attempts', []).append({'attempt':attempt + 1, 'payload':added[0][2], 'before':prior, 'after':after})
        if payload.get('applied') == 'true':
            require(len(after['parents']) == 1 and after['parents'][0]['children'] == ['SpellReadAttunement'] and
                    0 < after['parents'][0]['remaining_ms'] <= 60000, 'S4 paid cast did not install its native overlay with the grade-one lifetime.')
            success = True
            break
        require(payload.get('outcome') in ('MinorFail', 'Fail', 'MajorFail'), 'Unexpected S4 defining-result failure.')
    require(success, 'S4 finite native RNG budget exhausted without defining result; no gameplay certificate.')
    assertions.append('S4 stock: real grade-one invocation retained one50-unit payment and native target-scoped overlay with60-second lifetime')
    looked = session.send('look me', read_seconds=.5)
    verify_overlay(looked)
    # Spend the remaining finite budget through another ordinary cast, retaining its paid failure if RNG fails.
    if state()['balance'] == 50:
        prior = state(); old = operations()
        session.send(f'armwater cast "Read Attunement" grade 1 on self via {cap}', read_seconds=.5); flush()
        added = [row for row in operations() if row[0] not in {x[0] for x in old}]
        require(len(added) == 1 and added[0][1] == 'Completed' and state()['balance'] == 0, 'S4 remaining casting budget was not consumed once.')
        receipt['stock_attunement_budget_exhaustion'] = {'payload':added[0][2], 'before':prior, 'after':state()}
    receipt['stock_attunement_first'] = state()
    receipt['stock_attunement_limits'] = data['limits'] + ' All seven grade costs/durations have deterministic pipeline tests; this Telnet lane casts grade one.'
    assertions.append('S4 stock: native visible overlay separated assigned affinity and learned affiliation; finite paid budget depleted before cold restart')
    mark_inherited_read('first')

def mark_inherited_read(phase):
    report = receipt.get('representative_campaign', {})
    for selected in report.get('selected', []):
        if selected['coverage'] == 'inherited-original-read':
            selected.setdefault('phases', {})[phase] = 'PASS-preserved-original-paid-read-assertions'
            selected['outcome'] = 'PASS' if phase == 'cold' else 'FIRST-PASS-COLD-PENDING'
    if phase == 'cold' and report.get('selected') and all(row['outcome'] == 'PASS' for row in report['selected']): report['status'] = 'PASS'

def exchange_places_smoke(session, phase):
    data = json.loads(Path(os.environ['FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT']).read_text(encoding='utf-8'))
    route = data['exchange_places']; actor = int(data['character']); body = int(data['body'])
    cap = int(route['capability']); resource = int(route['resource']); spell = int(route['spell'])
    require(cap not in [int(x) for x in data['package_capabilities']], 'Exchange must use its separate disposable admission.')
    def flush():
        require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.4), 'Exchange flush failed.')
    def balance():
        return float(sql(f'SELECT IFNULL((SELECT Amount FROM Characters_MagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)')[0][0])
    def operations():
        return sql(f'SELECT HEX(Id),HEX(Stage),HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} AND MagicSpellId={spell} ORDER BY CreatedUtc,Id', (0,1,2))
    def materials():
        return sql(f'SELECT gi.Id,gi.GameItemProtoId,HEX(gc.Definition) FROM GameItems gi '
                   f'JOIN GameItemComponents gc ON gc.GameItemId=gi.Id JOIN GameItemComponentProtos cp ON cp.Id=gc.GameItemComponentProtoId '
                   f'AND cp.RevisionNumber=gc.GameItemComponentProtoRevision WHERE cp.Type="Stackable" '
                   f'AND gi.GameItemProtoId IN ({route["teleportation"]},{route["alteration"]}) ORDER BY gi.Id', (2,))
    command = f'{route["invoked"]} cast "Exchange Places" grade 1 on mount via {cap}'
    if phase == 'cold':
        report = receipt['exchange_places']
        target = sql(f'SELECT Location,BodyId FROM Characters WHERE Id={report["target"]}')
        require(int(character_state()[3]) == report['destination'] and target == [[str(report['home']),str(report['target_body'])]],
                'Cold exchange did not retain the exact swapped character/body locations.')
        before = operations(); funds = balance(); items = materials()
        session.send(command, read_seconds=1); flush()
        require(before == operations() and funds == balance() and items == materials(), 'Cold cast without a live contact paid or moved again.')
        report['cold_status'] = 'PASS'
        assertions.append('S4 Exchange: cold restart retained both exact swapped bodies and refused a new cast without live contact without payment')
        return
    report = receipt['exchange_places'] = {'status':'FAIL', 'attempts':[], 'home':int(receipt['initial_avatar'][3]),
                                          'destination':int(receipt['campaign_exit_destination'])}
    session.send(f'givemerit me {route["merit"]}', read_seconds=.5)
    session.send(f'magic casting enrol {actor} {cap} disposable Exchange acceptance', read_seconds=.5); flush()
    require(sql(f'SELECT CompletedStartingGrantVersion FROM CharacterCastingEnrolments WHERE CharacterId={actor} AND MagicCapabilityId={cap}')[0][0] == '1', 'Exchange enrolment failed.')
    session.send('east', read_seconds=1)
    template = sql(f'SELECT UniqueName FROM NPCTemplates WHERE Id={route["target_template"]}')[0][0]
    session.send('npc load ' + template, read_seconds=1); flush()
    targets = sql(f'SELECT c.Id,c.BodyId,c.Location FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE n.TemplateId={route["target_template"]} ORDER BY c.Id DESC LIMIT 1')
    require(len(targets) == 1 and int(targets[0][2]) == report['destination'], 'Exchange target did not load outdoors.')
    report.update(target=int(targets[0][0]), target_body=int(targets[0][1]))
    for attempt in range(4):
        output = session.send(f'{route["invoked"]} contact mount', read_seconds=1)
        if 'connect your mind' in output.lower() or 'establish' in output.lower(): break
    # The cast's exact native contact selector is the authority; the transcript is supplementary.
    session.send('west', read_seconds=1)
    session.send('drop all', read_seconds=.5)
    session.send(f'item load {item_command_reference(int(route["pouch"]))}', read_seconds=.5)
    session.send('wear pouch', read_seconds=.5); session.send('open pouch', read_seconds=.5)
    session.send(f'item load {item_command_reference(int(route["teleportation"]))}', read_seconds=.5)
    session.send('put teleportation pouch', read_seconds=.5)
    session.send(f'item load 4 {item_command_reference(int(route["alteration"]))}', read_seconds=.5); flush()
    initial = materials()
    require(len(initial) == 2, 'Exchange lacks the exact retained and consumed carried stock components.')
    custody = sql(f'SELECT retained.Id,pouch.Id,bgi.WearProfile FROM GameItems retained '
                  f'JOIN GameItems pouch ON pouch.Id=retained.ContainerId '
                  f'JOIN Bodies_GameItems bgi ON bgi.GameItemId=pouch.Id AND bgi.BodyId={body} '
                  f'WHERE retained.GameItemProtoId={route["teleportation"]} AND pouch.GameItemProtoId={route["pouch"]} '
                  f'AND bgi.WearProfile IS NOT NULL')
    require(len(custody) == 1 and len(sql(f'SELECT gi.Id FROM GameItems gi '
            f'JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id AND bgi.BodyId={body} '
            f'WHERE gi.GameItemProtoId={route["alteration"]} AND gi.ContainerId IS NULL')) == 1,
            'Exchange requires retained Teleportation in the worn pouch and directly carried Alteration.')
    report['initial_custody'] = custody
    for attempt in range(4):
        if balance() < float(route['cost']):
            session.send(f'{route["invoked"]} gather {cap} draw {int(100-balance())}', read_seconds=.5)
            deadline = time.monotonic()+18
            while time.monotonic() < deadline and balance() < float(route['cost']): session.read_for(.5); flush()
        before = operations(); funds = balance(); items = materials()
        output = session.send(command, read_seconds=1); flush()
        added = [row for row in operations() if row[0] not in {old[0] for old in before}]
        require(len(added) == 1 and added[0][1] == 'Completed', 'Exchange did not complete one paid native operation: '+redact(output))
        payload = ET.fromstring(added[0][2]); costs = payload.findall('Cost')
        require(payload.get('grade') == '1' and payload.get('controlledGrade') == '1' and len(costs) == 1 and
                costs[0].get('resource') == str(resource) and float(costs[0].get('amount')) == float(route['cost']) and
                abs(funds-balance()-float(route['cost'])) < .001, 'Exchange payment differs from the exact grade-one quote.')
        after = materials()
        retained = [row for row in items if row[1] == str(route['teleportation'])]
        require(retained == [row for row in after if row[1] == str(route['teleportation'])], 'Exchange consumed or replaced retained Teleportation.')
        prior = [row for row in items if row[1] == str(route['alteration'])]
        remaining = [row for row in after if row[1] == str(route['alteration'])]
        require(len(prior) == 1 and (int(ET.fromstring(prior[0][2]).get('Quantity'))-1 ==
                (int(ET.fromstring(remaining[0][2]).get('Quantity')) if remaining else 0)), 'Exchange did not consume exactly one Alteration unit.')
        report['attempts'].append({'operation':added[0][0], 'payload':added[0][2], 'before':funds, 'after':balance()})
        if payload.get('applied') == 'true': break
        require(payload.get('outcome') in ('MinorFail','Fail','MajorFail'), 'Exchange returned an unexpected non-applied operation.')
    require(payload.get('applied') == 'true', 'Four bounded ordinary Exchange attempts did not succeed.')
    target = sql(f'SELECT Location,BodyId FROM Characters WHERE Id={report["target"]}')
    require(int(character_state()[3]) == report['destination'] and target == [[str(report['home']),str(report['target_body'])]], 'Exchange did not swap the two exact saved bodies.')
    report['status'] = 'PASS'
    assertions.append('S4 Exchange: exact native live-contact paid grade-one swap retained Teleportation, consumed one Alteration per attempt and saved both locations')


def nilaz_gate_smoke(session, data):
    config = data['nilaz_gate']
    routes = {route['key']: route for route in config['routes']}
    require(set(routes) == {'arm.spell.arcane_mark', 'arm.spell.linked_threshold'} and
            'campaign_exit_destination' in receipt, 'Nilaz scenario needs the exact two paid spells and approved real east room.')
    actor = int(data['character']); body = int(data['body'])
    home = int(receipt['initial_avatar'][3]); destination = int(receipt['campaign_exit_destination'])
    mark = routes['arm.spell.arcane_mark']; gate = routes['arm.spell.linked_threshold']
    require(home != destination and all(int(route['resource']) == int(mark['resource']) and
            int(route['capability']) not in [int(x) for x in data['package_capabilities']] for route in routes.values()),
            'Nilaz route is not a separate Void builder admission.')
    report = receipt['nilaz_gate'] = {'status': 'FAIL', 'routes': [], 'home': home, 'destination': destination, 'attempts': []}
    def flush():
        require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.4), 'Nilaz save flush failed.')
    def balance():
        rows = sql(f'SELECT IFNULL((SELECT Amount FROM Characters_MagicResources WHERE CharacterId={actor} '
                   f'AND MagicResourceId={int(mark["resource"])}),0)')
        return float(rows[0][0])
    def operations(route):
        return sql(f'SELECT HEX(Id),HEX(Stage),HEX(Definition) FROM MagicCastingOperations WHERE '
                   f'CharacterId={actor} AND MagicSpellId={int(route["spell"])} ORDER BY CreatedUtc,Id', (0,1,2))
    def funded_cast(route, target):
        cap = int(route['capability']); cost = float(route['cost'])
        for attempt in range(4):
            if balance() + 0.001 < cost:
                top_up = int(100 - balance())
                session.send(f'{route["invoked"]} gather {cap} draw {top_up}', read_seconds=.5)
                deadline = time.monotonic() + 18
                while time.monotonic() < deadline and balance() + 0.001 < cost:
                    session.read_for(.5); flush()
                require(balance() + 0.001 >= cost, 'Native Void gathering did not fund the next paid cast.')
            before = balance(); old = operations(route)
            output = session.send(f'{route["invoked"]} cast "{ "Arcane Mark" if route is mark else "Linked Threshold" }" '
                                  f'grade 1 on {target} via {cap}', read_seconds=1)
            flush()
            added = [row for row in operations(route) if row[0] not in {entry[0] for entry in old}]
            require(len(added) == 1 and added[0][1] == 'Completed', 'Nilaz paid operation missing: ' + redact(output))
            payload = ET.fromstring(added[0][2]); costs = payload.findall('Cost')
            require(payload.get('grade') == '1' and payload.get('controlledGrade') == '1' and
                    len(costs) == 1 and costs[0].get('resource') == str(route['resource']) and
                    float(costs[0].get('amount')) == cost and abs(before - balance() - cost) < .001,
                    'Nilaz normal cast did not persist one exact grade-one resource payment.')
            report['attempts'].append({'key':route['key'], 'payload':added[0][2], 'before':before, 'after':balance()})
            if payload.get('applied') == 'true': return
            require(payload.get('outcome') in ('MinorFail','Fail','MajorFail'), 'Unexpected Nilaz cast result.')
        require(False, 'Four ordinary paid Nilaz attempts did not achieve a defining result.')
    for route in (mark, gate):
        cap = int(route['capability'])
        session.send(f'givemerit me {int(route["merit"])}', read_seconds=.4)
        session.send(f'magic casting enrol {actor} {cap} explicit disposable Nilaz acceptance', read_seconds=.5)
        flush()
        rows = sql(f'SELECT CompletedStartingGrantVersion FROM CharacterCastingEnrolments WHERE CharacterId={actor} '
                   f'AND MagicCapabilityId={cap}')
        require(len(rows) == 1 and int(rows[0][0]) > 0, 'Nilaz route was not natively enrolled.')
        name = sql(f'SELECT Name FROM MagicCapabilities WHERE Id={cap}')[0][0]
        report['routes'].append({'key':route['key'], 'capability':cap, 'capability_name':name})
    # The earlier provision fixture leaves its bottle in one hand. Clear held
    # fixtures before loading the mark leaf so the ordinary cast has a free hand.
    session.send('drop all', read_seconds=1)
    flush()
    session.send(f'item load {item_command_reference(int(config["item_prototype"]))}', read_seconds=1)
    flush()
    held = sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id WHERE '
               f'gi.GameItemProtoId={int(config["item_prototype"])} AND bgi.BodyId={body} ORDER BY gi.Id DESC LIMIT 1')
    require(len(held) == 1, 'Nilaz mark lacks an exact native carried leaf.')
    report['marked_item'] = int(held[0][0])
    claimed = session.send('ownership claim leaf', read_seconds=1)
    flush()
    ownership = sql(f'SELECT OwnerType,OwnerId FROM GameItems WHERE Id={report["marked_item"]}')
    require('you claim ownership' in claimed.lower() and len(ownership) == 1 and
            ownership[0][0] == 'Character' and int(ownership[0][1]) > 0,
            'The exact carried leaf did not acquire ordinary character ownership before marking.')
    report['marked_item_ownership'] = ownership[0]
    session.send('east', read_seconds=1); flush()
    require(int(character_state()[3]) == destination, 'Admin did not traverse the approved ordinary east exit to mark destination.')
    funded_cast(mark, 'leaf')
    session.send('west', read_seconds=1); flush()
    require(int(character_state()[3]) == home, 'Admin did not return to the distinct Nilaz source room.')
    funded_cast(gate, 'here')
    lives = sql(f"SELECT HEX(Id),State,IFNULL(DATE_FORMAT(GateLinksClosedUtc,'%Y-%m-%dT%H:%i:%s.%f'),'NULL'),"
                f"IFNULL(DATE_FORMAT(GateLastPulseUtc,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') FROM MagicSpellLifecycles "
                f"WHERE Family='native-nilaz-gate-v1' AND SpellId={int(gate['spell'])}")
    require(len(lives) == 1 and lives[0][1] == '0', 'Paid Linked Threshold did not create one active durable Nilaz lifecycle.')
    life = lives[0][0]; report['lifecycle'] = life
    claims = sql(f'SELECT Kind,EntityId,Role FROM MagicSpellOwnedEntities WHERE HEX(LifecycleId)="{life}" ORDER BY Kind,Role')
    require(len(claims) == 7 and sorted(int(x[0]) for x in claims) == [4,4,5,5,5,6,6],
            'Nilaz did not claim exactly two rooms, two overlays and three exits.')
    source_portal = next(int(x[1]) for x in claims if int(x[0]) == 5 and int(x[2]) == 2)
    corridor = next(int(x[1]) for x in claims if int(x[0]) == 5 and int(x[2]) == 3)
    destination_portal = next(int(x[1]) for x in claims if int(x[0]) == 5 and int(x[2]) == 4)
    rooms = [int(x[1]) for x in claims if int(x[0]) == 4]
    report['claims'] = claims
    entered = session.send('enter gate', read_seconds=1); flush()
    require(int(character_state()[3]) in rooms, 'Normal gate entry did not move Admin into a claimed temporary room: ' + redact(entered))
    south = int(character_state()[3]); report['south'] = south
    session.send('north', read_seconds=1); flush()
    north = int(character_state()[3]); report['north'] = north
    require(north in rooms and north != south, 'Admin did not cross the claimed internal north corridor.')
    require(sql(f'SELECT RoomId1,RoomId2 FROM Exits WHERE Id={corridor}') == [[str(south),str(north)]],
            'Native north traversal did not follow exact owned corridor endpoints.')
    # Retirement is driven by the fuzzy-minute heartbeat after the 120-second
    # grade-one deadline, so allow one full heartbeat plus scheduling slack.
    deadline = time.monotonic() + 190
    while time.monotonic() < deadline:
        flush()
        lives = sql(f"SELECT State,IFNULL(DATE_FORMAT(GateLinksClosedUtc,'%Y-%m-%dT%H:%i:%s.%f'),'NULL'),"
                    f"IFNULL(DATE_FORMAT(GateLastPulseUtc,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') FROM MagicSpellLifecycles WHERE HEX(Id)='{life}'")
        if lives and lives[0][0] == '1' and lives[0][1] != 'NULL': break
        time.sleep(2)
    require(lives and lives[0][0] == '1' and lives[0][1] != 'NULL', 'Occupied Nilaz gate did not close by its native deadline.')
    closed = lives[0][1]; report['closed_at'] = closed
    require(int(character_state()[3]) == north and
            sql(f'SELECT COUNT(*) FROM Exits WHERE Id IN ({source_portal},{destination_portal})')[0][0] == '0' and
            sql(f'SELECT COUNT(*) FROM Exits WHERE Id={corridor}')[0][0] == '1' and
            sql(f'SELECT COUNT(*) FROM Rooms WHERE Id IN ({south},{north})')[0][0] == '2',
            'Occupied closure did not retain both rooms, internal corridor and Admin while deleting external links.')
    assertions.append('S4 Nilaz: two normal paid casts created the exact seven owned claims and native traversal; occupied expiry closed external links without evacuation')
    wound_before = int(sql(f'SELECT COUNT(*) FROM Wounds WHERE BodyId={body} AND DamageType=12')[0][0])
    pulse_deadline = time.monotonic() + 145
    while time.monotonic() < pulse_deadline:
        flush()
        pulse = sql(f"SELECT IFNULL(DATE_FORMAT(GateLastPulseUtc,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') "
                    f"FROM MagicSpellLifecycles WHERE HEX(Id)='{life}'")[0][0]
        if pulse != closed: break
        time.sleep(2)
    hypoxia = sql(f'SELECT OriginalDamage,CurrentStun FROM Wounds WHERE BodyId={body} AND DamageType=12 ORDER BY Id DESC')
    require(pulse != closed and int(character_state()[3]) == north and len(hypoxia) > wound_before and
            any(float(row[0]) > 0 and float(row[1]) > 0 for row in hypoxia[:len(hypoxia)-wound_before]),
            'Closed occupied Nilaz room did not persist a native hypoxia wound with stun after the durable pulse.')
    report['pulse_at'] = pulse
    report['wounds_after'] = sql(f'SELECT DamageType,OriginalDamage,CurrentDamage,CurrentStun FROM Wounds WHERE BodyId={body} ORDER BY Id DESC LIMIT 10')
    assertions.append('S4 Nilaz: occupied closed-room suffocation advanced the durable native pulse cursor while retaining custody')
    session.send(f'goto {home}', read_seconds=1); flush()
    require(int(character_state()[3]) == home, 'Admin rescue did not leave the closed gate through its authorized staff command.')
    cleanup_deadline = time.monotonic() + 145
    while time.monotonic() < cleanup_deadline:
        flush()
        state = sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')[0][0]
        if state == '3': break
        time.sleep(2)
    require(state == '3' and sql(f'SELECT COUNT(*) FROM Rooms WHERE Id IN ({south},{north})')[0][0] == '0' and
            sql(f'SELECT COUNT(*) FROM Exits WHERE Id={corridor}')[0][0] == '0',
            'Empty Nilaz gate was not safely retired after occupant left.')
    report['status'] = 'PASS'
    assertions.append('S4 Nilaz: empty closed topology reconciled to completed lifecycle with both rooms and corridor removed')

def nilaz_gate_destination(session, label='Nilaz'):
    # Build only the ordinary east/west exit needed by this focused gate lane.
    package_name = 'S4 ' + label + ' ' + uuid.uuid4().hex
    session.send('room package new ' + package_name, read_seconds=1)
    require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.5),
            'Nilaz room package save failed.')
    rows = sql("SELECT Id FROM RoomOverlayPackages WHERE Name='" + package_name + "'")
    require(len(rows) == 1, 'Nilaz ordinary destination package was not persisted.')
    output = session.send('room dig east', read_seconds=2)
    require(any(word in output.lower() for word in ('created', 'exit', 'room')),
            'Nilaz ordinary east exit build failed: ' + redact(output))
    require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.5),
            'Nilaz destination room save failed.')
    destination = int(character_state()[3])
    require(destination != int(receipt['initial_avatar'][3]), 'Nilaz destination is not distinct.')
    session.send('room set type outdoors', read_seconds=1)
    session.send('room package submit S4 Nilaz disposable destination', read_seconds=1)
    session.send('room package review ' + rows[0][0], read_seconds=1)
    session.send('accept edit S4 Nilaz disposable destination', read_seconds=1)
    session.send('room package swap ' + rows[0][0], read_seconds=1)
    session.send('west', read_seconds=1)
    require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.5) and
            int(character_state()[3]) == int(receipt['initial_avatar'][3]),
            'Nilaz normal west exit did not return to Guest Lounge.')
    receipt['campaign_exit_destination'] = destination
    assertions.append(f'S4 {label}: live builder created and traversed distinct ordinary east/west destination')

def nilaz_gate_cold_smoke(session):
    report = receipt['nilaz_gate']
    require(report['status'] == 'PASS' and int(character_state()[3]) == report['home'],
            'Cold Nilaz check lost the completed first-boot gate or rescued avatar location.')
    life = report['lifecycle']; south = report['south']; north = report['north']
    rows = sql(f'SELECT State,GateLinksClosedUtc,GateLastPulseUtc FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')
    require(len(rows) == 1 and rows[0][0] == '3' and rows[0][1] != 'NULL' and rows[0][2] != 'NULL' and
            sql(f'SELECT COUNT(*) FROM Rooms WHERE Id IN ({south},{north})')[0][0] == '0' and
            sql(f'SELECT COUNT(*) FROM Exits WHERE Id IN ({ ",".join(str(row[1]) for row in report["claims"] if int(row[0]) == 5) })')[0][0] == '0',
            'Cold restart resurrected a completed Nilaz gate, external link, room or corridor.')
    report['cold_status'] = 'PASS'
    assertions.append('S4 Nilaz: cold restart retained completed lifecycle, rescued avatar and no owned rooms or exits')

def stock_campaign_smoke(session, phase, data, mapped):
    config = data.get('representative_campaign')
    if config is None:
        receipt['representative_campaign'] = {'seed': campaign.SEED, 'status': 'PENDING', 'reason': 'No explicit disposable builder admission input; no campaign was executed.'}
        receipt['earth_mount_sample'] = {'status': 'PENDING', 'reason': 'No paid Earth Mount route/exit observation fixture; native controller following is not certified.'}
        return
    installed = {entry['key']: entry for entry in data['stock_spells']}
    selected = campaign.select_campaign(config, installed)
    report = receipt.setdefault('representative_campaign', campaign.selection_receipt(selected))
    config_hash = hashlib.sha256(json.dumps(config, sort_keys=True, separators=(',', ':')).encode()).hexdigest()
    require(report.setdefault('configuration_sha256', config_hash) == config_hash, 'Campaign configuration changed across cold restart.')
    report['configuration_source'] = receipt['stock_input_pin']
    require([row['key'] for row in report['selected']] == [row['key'] for row in selected], 'Campaign selection drift across cold restart.')
    report['sample_limit'] = config['sample_limit']
    report['limits'] = 'Disposable builder admissions; grade one only; no package/Class/producer gates or natural progression claim. At most four separately funded paid RNG attempts per direct sample.'
    receipt['earth_mount_sample'] = {'status': 'PENDING', 'reason': 'Earth Mount spawn/controller movement requires an explicit paid route plus disposable real-exit observation; this bounded representative lane does not fake AI proof.'}
    actor = int(data['character'])
    def flush():
        require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.3), 'Campaign native save flush failed.')
    def state(row):
        resource, spell, cap, trait = (int(row[field]) for field in ('resource', 'spell', 'capability', 'trait'))
        values = sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0), '
                     f'IFNULL((SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} AND MagicSpellId={spell}),0), '
                     f'IFNULL((SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={trait}),0), '
                     f'(SELECT COUNT(*) FROM CharacterCastingEnrolments WHERE CharacterId={actor} AND MagicCapabilityId={cap})'))
        require(len(values) == 1 and len(values[0]) == 4, 'Campaign actor state is missing.')
        return dict(zip(('balance', 'grade', 'raw', 'enrolments'), map(float, values[0])))
    def operations(row):
        return sql(mapped(f'SELECT HEX(Id),HEX(Stage),HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} AND MagicSpellId={int(row["spell"])} ORDER BY CreatedUtc,Id'), (0,1,2))
    def native_stamina():
        values = sql(mapped(f'SELECT CurrentStamina FROM Bodies WHERE Id={int(data["body"])}'))
        require(len(values) == 1 and math.isfinite(float(values[0][0])) and float(values[0][0]) >= 0, 'Campaign native saved stamina is invalid.')
        return float(values[0][0])
    def native_needs():
        values = sql(mapped(f'SELECT NeedsModel,DrinkSatiatedHours,AlcoholLitres FROM Characters WHERE Id={actor}'))
        require(len(values) == 1 and len(values[0]) == 3 and values[0][0] == data['fixture_needs_model'] == 'Active' and
                all(math.isfinite(float(value)) for value in values[0][1:]), 'Campaign saved native needs are invalid.')
        return {'model': values[0][0], 'drink_satiated_hours': float(values[0][1]), 'alcohol_litres': float(values[0][2])}
    def delayed_alcohol():
        rows = sql(mapped(f'SELECT HEX(EffectData) FROM Bodies WHERE Id={int(data["body"])}'), (0,))
        require(len(rows) == 1, 'Campaign needs body is missing.')
        values = []
        for wrapper in ET.fromstring(rows[0][0]).iter('Effect'):
            if wrapper.findtext('Type') != 'DelayedNeedsFulfillment': continue
            payload = wrapper.find('Effect')
            require(payload is not None and math.isfinite(float(payload.get('AlcoholLitres', 'nan'))),
                    'Campaign delayed native alcohol payload is invalid.')
            values.append(float(payload.get('AlcoholLitres')))
        return values
    def parents(row):
        table, identity = ('Characters', actor) if row['target'] == 'self' else ('Rooms', int(character_state()[3]))
        rows = sql(mapped(f'SELECT HEX(EffectData) FROM {table} WHERE Id={identity}'), (0,))
        require(len(rows) == 1, 'Campaign effect owner is missing.')
        saved = []
        for wrapper in ET.fromstring(rows[0][0]).iter('Effect'):
            inner = wrapper.find('Effect')
            if wrapper.findtext('Type') != 'MagicSpellParent' or inner is None or inner.findtext('Spell') != str(row['spell']): continue
            children = list(inner.find('Children') or [])
            saved.append({'identity': inner.findtext('Identity'), 'remaining_ms': int(wrapper.findtext('Remaining', '0')),
                          'children': [child.findtext('Type') for child in children],
                          'child_definitions': [ET.tostring(child.find('Effect'), encoding='unicode') for child in children]})
        return saved
    def venom_doses(row):
        definition = ET.fromstring(installed[row['key']]['definition'])
        effect = definition.find('Effects/Effect')
        require(effect is not None and effect.get('type') == 'poison', 'Paid Venom Touch lost its stock effect.')
        drug = int(effect.findtext('Drug', '0'))
        require(drug > 0, 'Paid Venom Touch has no bound native drug.')
        rows = sql(mapped(f"SELECT SpellOriginId,SpellOriginFamily,SpellOriginFormula,OriginalVector,DrugId,Grams "
                          f"FROM Bodies_DrugDoses WHERE BodyId={int(data['body'])} AND DrugId={drug} "
                          "AND SpellOriginFamily='arm.effect.poison'"))
        result = [{'origin': cells[0], 'family': cells[1], 'formula': cells[2],
                   'vector': int(cells[3]), 'drug': int(cells[4]), 'grams': float(cells[5])} for cells in rows]
        require(all(re.fullmatch(r'[0-9a-fA-F-]{36}', x['origin']) and x['family'] == 'arm.effect.poison' and
                    x['formula'] == '0.001*grade' and x['vector'] == 8 and x['drug'] == drug and
                    math.isfinite(x['grams']) and x['grams'] > 0 for x in result),
                'Paid Venom Touch saved a dose with wrong origin, family, formula, vector or drug.')
        return result
    for row, result in zip(selected, report['selected']):
        if result['coverage'] == 'inherited-original-read': continue
        key = row['key']; cap = int(row['capability']); cost = float(row['grade_one_cost'])
        definition = sql(f'SELECT HEX(Definition) FROM MagicCapabilities WHERE Id={cap}', (0,))
        require(len(definition) == 1, 'Campaign capability is missing.')
        capability_definition = ET.fromstring(definition[0][0])
        casting = capability_definition.find('Casting')
        gathering = capability_definition.findall('Gathering/Method')
        require(cap not in [int(x) for x in data['package_capabilities']] and casting is not None and casting.get('enabled') == 'true' and
                casting.get('passive') == 'false' and row['source'] == installed[key]['cost_resource'] and
                all(casting.get(field) == str(row[expected]) for field, expected in (('trait','trait'), ('source','source'), ('reserve','resource'))) and
                any(admission.get('spell') == str(row['spell']) and admission.get('starting') == 'true' and admission.get('min') == '1' for admission in casting.findall('Admission')),
                'Campaign route is not its explicit separate grade-one builder admission: ' + key)
        require(len(gathering) == 1 and all(gathering[0].get(field) == value for field, value in
                (('alias','draw'), ('kind','Self'), ('destination',str(row['resource'])), ('ratio','1'), ('min','1'), ('max','100'))) and
                float(gathering[0].get('stamina', 'nan')) == float(row.get('gather_stamina_cost', 1)),
                'Campaign funding is not its bounded native Self method: ' + key)
        result['route'] = {field: row[field] for field in ('capability','merit','trait','school','resource','source','invoked','grade_one_cost')}
        capability_hash = hashlib.sha256(definition[0][0].encode()).hexdigest()
        require(result.setdefault('capability_definition_sha256', capability_hash) == capability_hash, 'Campaign native capability changed across cold reload.')
        flush(); initial = state(row)
        if phase == 'cold':
            require(initial == result['saved_state'] and operations(row) == result['saved_operations'], 'Cold campaign refilled/reacquired/replayed: ' + key)
            result['phases']['cold'] = {'state': initial, 'outcome': 'PASS-no-refill-no-replay'}
            if key == 'arm.spell.erode_object':
                item = result['physical_item']
                current = sql(f'SELECT GameItemProtoId FROM GameItems WHERE Id={item["id"]}')
                wounds = float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE GameItemId={item["id"]}')[0][0])
                require(len(current) == 1 and current[0][0] == str(item['prototype']) and
                        math.isfinite(wounds) and wounds == result['saved_item_damage'],
                        'Cold Erode Object changed the exact native item or replayed its wound.')
                result['phases']['cold']['native_item_damage'] = wounds
            if key in campaign.EXPECTED_CHILDREN:
                current_parents = parents(row); saved = result['saved_parents']
                elapsed_ms = (time.monotonic() - result['saved_at_monotonic']) * 1000
                if not current_parents and elapsed_ms >= saved[0]['remaining_ms']:
                    result['phases']['cold']['effect_outcome'] = 'EXPIRED-before-observation-no-cold-effect-certificate'
                else:
                    require(len(current_parents) == 1 and current_parents[0]['identity'] == saved[0]['identity'] and
                            current_parents[0]['children'] == saved[0]['children'] and current_parents[0]['child_definitions'] == saved[0]['child_definitions'] and
                            0 < current_parents[0]['remaining_ms'] <= saved[0]['remaining_ms'],
                            'Cold campaign effect identity/children/lifetime changed: ' + key)
                    result['phases']['cold']['effect_outcome'] = 'PASS-exact-native-parent-child-reload'
                result['phases']['cold']['parents'] = current_parents
                if key == 'arm.spell.venom_touch':
                    doses = venom_doses(row)
                    require(all(x['origin'].casefold() == result['dose_origin'].casefold() for x in doses),
                            'Cold Venom Touch dose changed its persisted spell origin.')
                    result['phases']['cold']['doses'] = doses
                    result['phases']['cold']['dose_outcome'] = 'PASS-exact-native-dose-reload' if doses else 'CLEARED-physiologically-before-cold-observation'
            result['outcome'] = 'PASS'
            continue
        require(initial['balance'] == 0 and initial['grade'] == 0 and initial['enrolments'] == 0, 'Campaign preparation pregranted/refilled actor: ' + key)
        physical_target = None
        cast_target = row['target']
        if key in ('arm.spell.gust_hands', 'arm.spell.cinder_rain'):
            template_id = int(catalogue_record('arm.npc.earth_mount.g1')[2])
            unique = sql(f'SELECT UniqueName FROM NPCTemplates WHERE Id={template_id}')
            require(len(unique) == 1 and unique[0][0] == 'arm.npc.earth_mount.g1', 'Campaign lost exact native mount template.')
            previous = int(sql(f'SELECT COUNT(*) FROM Npcs WHERE TemplateId={template_id}')[0][0])
            spawned = session.send('npc load ' + unique[0][0], read_seconds=2)
            flush()
            result['physical_spawn_output'] = redact(spawned)
            require(int(sql(f'SELECT COUNT(*) FROM Npcs WHERE TemplateId={template_id}')[0][0]) == previous + 1,
                    'Campaign did not persist exactly one disposable native physical target: ' + redact(spawned))
            rows = sql(f'SELECT c.Id,c.BodyId,c.Location,c.Name FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id '
                       f'WHERE n.TemplateId={template_id} ORDER BY c.Id DESC LIMIT 1')
            require(len(rows) == 1 and len(rows[0]) == 4 and all(rows[0][index].isdigit() for index in (0,1,2)) and
                    rows[0][2] == character_state()[3] and rows[0][3], 'Campaign target lacks exact local Character/Body custody.')
            physical_target = {'id': int(rows[0][0]), 'body': int(rows[0][1]), 'home': int(rows[0][2]), 'name': rows[0][3]}
            result['physical_target'] = physical_target
            if key == 'arm.spell.gust_hands':
                cast_target = '.'.join(rows[0][3].lower().split()) + ' east'
                require(row['target'] == 'mount east' and 'campaign_exit_destination' in receipt,
                        'Wind sample lacks its approved explicit native east exit.')
            elif key == 'arm.spell.cinder_rain':
                cast_target = '.'.join(rows[0][3].lower().split())
                require(row['target'] == 'mount', 'Area sample lacks its approved local character target.')
        if key == 'arm.spell.erode_object':
            item_prototype = int(config['item_prototype'])
            require(row['target'] == 'leaf' and item_prototype > 0, 'Item sample lacks its explicit native prototype.')
            before_items = int(sql(f'SELECT COUNT(*) FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id '
                                   f'WHERE gi.GameItemProtoId={item_prototype} AND bgi.BodyId={int(data["body"])}')[0][0])
            item_output = session.send('item load ' + item_command_reference(item_prototype), read_seconds=2)
            flush()
            item_rows = sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id '
                            f'WHERE gi.GameItemProtoId={item_prototype} AND bgi.BodyId={int(data["body"])} ORDER BY gi.Id DESC LIMIT 1')
            require('you load ' in item_output.lower() and len(item_rows) == 1 and item_rows[0][0].isdigit() and
                    int(sql(f'SELECT COUNT(*) FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id '
                            f'WHERE gi.GameItemProtoId={item_prototype} AND bgi.BodyId={int(data["body"])}')[0][0]) == before_items + 1,
                    'Native Erode Object sample did not load one exact disposable organic item.')
            result['physical_item'] = {'id': int(item_rows[0][0]), 'prototype': item_prototype}
            result['physical_item_load_output'] = redact(item_output)
            dropped = session.send('drop leaf', read_seconds=2)
            flush()
            room_item = sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={result["physical_item"]["id"]}')
            require('you drop ' in dropped.lower() and len(room_item) == 1 and
                    room_item[0][0] == character_state()[3] and
                    int(sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={result["physical_item"]["id"]}')[0][0]) == 0,
                    'Native Erode Object item did not leave the caster with free hands in the exact local room.')
            result['physical_item_drop_output'] = redact(dropped)
        session.send(f'givemerit me {int(row["merit"])}', read_seconds=.3)
        enrol = session.send(f'magic casting enrol {actor} {cap} explicit disposable seeded representative campaign', read_seconds=.4)
        flush(); current = state(row)
        require(current['balance'] == 0 and current['grade'] == 1 and current['raw'] == 100 and current['enrolments'] == 1, 'Campaign native enrolment did not acquire grade one: ' + key + ': ' + redact(enrol))
        result['phases'] = {'first': {'enrolled': current}}
        result['attempts'] = []
        for attempt in range(1, 5):
            require(state(row)['balance'] == 0, 'Campaign must fund only a depleted reserve.')
            session.send(f'{row["invoked"]} gather {cap} draw {cost:g}', read_seconds=.2)
            deadline = time.monotonic() + 15
            while time.monotonic() < deadline:
                session.read_for(.4); flush()
                if state(row)['balance'] == cost: break
            before = state(row)
            require(before['balance'] == cost, 'Campaign Self gathering did not fund exactly its finite cost: ' + key)
            before_stamina = native_stamina() if key == 'arm.spell.second_breath' else None
            before_needs = native_needs() if key in ('arm.spell.desiccate', 'arm.spell.headlong_revel') else None
            before_delayed = delayed_alcohol() if key == 'arm.spell.headlong_revel' else None
            before_wounds = None
            if key == 'arm.spell.cinder_rain':
                before_wounds = float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={physical_target["body"]}')[0][0])
            if key == 'arm.spell.erode_object':
                before_wounds = float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE GameItemId={result["physical_item"]["id"]}')[0][0])
            old = operations(row)
            output = session.send(f'{row["invoked"]} cast "{installed[key]["name"]}" grade 1 on {cast_target} via {cap}', read_seconds=.5)
            flush(); after = state(row); new = operations(row)
            added = [operation for operation in new if operation[0] not in {previous[0] for previous in old}]
            attempt_receipt = {'attempt': attempt, 'before': before, 'after': after, 'output': redact(output), 'operations': added}
            if before_stamina is not None:
                attempt_receipt['native_stamina_observed'] = {'before': before_stamina, 'after': native_stamina(),
                    'limits': 'Saved native observations include heartbeat regeneration; exact five-unit bounds are covered by deterministic native-effect tests.'}
            if before_needs is not None:
                attempt_receipt['native_needs_observed'] = {'before': before_needs, 'after': native_needs(),
                    'limits': 'Saved native observations include heartbeat changes; exact selected-grade caps are covered by deterministic native-effect tests.'}
            if before_delayed is not None:
                attempt_receipt['native_delayed_alcohol_observed'] = {'before': before_delayed, 'after': delayed_alcohol()}
            result['attempts'].append(attempt_receipt) # Preserve the failed boundary before any assertion.
            require(len(added) == 1 and added[0][1] == 'Completed', 'Campaign did not persist one completed paid operation: ' + key)
            payload = ET.fromstring(added[0][2]); costs = payload.findall('Cost')
            require(payload.get('grade') == '1' and payload.get('controlledGrade') == '1' and len(costs) == 1 and
                    costs[0].get('resource') == str(row['resource']) and float(costs[0].get('amount')) == cost and
                    before['balance'] - after['balance'] == cost and after['balance'] == 0 and after['grade'] == 1 and after['enrolments'] == 1,
                    'Campaign payment/grade/reserve conservation differs: ' + key)
            if payload.get('applied') == 'true':
                if key == 'arm.spell.gust_hands':
                    destination = int(receipt['campaign_exit_destination'])
                    observed = sql(f'SELECT Location FROM Characters WHERE Id={physical_target["id"]}')
                    deadline = time.monotonic() + 20
                    while time.monotonic() < deadline and (len(observed) != 1 or observed[0][0] != str(destination)):
                        session.read_for(.4); flush()
                        observed = sql(f'SELECT Location FROM Characters WHERE Id={physical_target["id"]}')
                    require(len(observed) == 1 and observed[0][0] == str(destination) and destination != physical_target['home'],
                            'Paid Gust Hands did not move the exact physical NPC across the selected east exit.')
                    attempt_receipt['native_relocation'] = {'before': physical_target['home'], 'after': destination}
                if key == 'arm.spell.cinder_rain':
                    after_wounds = float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={physical_target["body"]}')[0][0])
                    attempt_receipt['native_area_damage'] = {'before': before_wounds, 'after': after_wounds}
                    require(math.isfinite(after_wounds), 'Paid Cinder Rain produced non-finite native injury.')
                    if after_wounds <= before_wounds:
                        attempt_receipt['outcome'] = 'APPLIED-NO-PERSISTED-WOUND-RETRY'
                        continue
                if key == 'arm.spell.erode_object':
                    item_id = result['physical_item']['id']
                    after_wounds = float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE GameItemId={item_id}')[0][0])
                    require(math.isfinite(after_wounds) and before_wounds < after_wounds <= before_wounds + 5,
                            'Paid Erode Object did not persist damage inside its grade-one native item budget.')
                    attempt_receipt['native_item_damage'] = {'before': before_wounds, 'after': after_wounds}
                    result['saved_item_damage'] = after_wounds
                if before_stamina is not None: require(attempt_receipt['native_stamina_observed']['after'] > before_stamina, 'Applied Second Breath did not persist a positive native stamina change.')
                if key == 'arm.spell.desiccate':
                    observed = attempt_receipt['native_needs_observed']
                    require(observed['after']['drink_satiated_hours'] < observed['before']['drink_satiated_hours'],
                            'Applied Desiccate did not persist a native thirst increase.')
                if key == 'arm.spell.headlong_revel':
                    observed = attempt_receipt['native_delayed_alcohol_observed']
                    added_alcohol = observed['after'][len(observed['before']):]
                    require(len(added_alcohol) == 3 and all(value > 0 for value in added_alcohol) and
                            sum(added_alcohol) <= .007500001,
                            'Applied Headlong Revel did not persist its three bounded native delayed alcohol children.')
                if key in campaign.EXPECTED_CHILDREN:
                    active = parents(row)
                    expected_children = campaign.EXPECTED_CHILDREN[key]
                    if isinstance(expected_children, str): expected_children = [expected_children]
                    require(len(active) == 1 and active[0]['children'] == expected_children and
                            0 < active[0]['remaining_ms'] <= float(row['duration_seconds']) * 1000,
                            'Paid campaign did not persist its exact native child/lifetime: ' + key)
                    child = ET.fromstring(active[0]['child_definitions'][0])
                    if key == 'arm.spell.daylight': require(float(child.findtext('GlowLux', 'nan')) == 20, 'Grade-one Daylight did not persist twenty native lux.')
                    if key == 'arm.spell.quickened_healing': require(float(child.findtext('Multiplier', 'nan')) == 1.1 and child.findtext('Stages') == '0', 'Grade-one healing multiplier/stages differ.')
                    if key == 'arm.spell.earthen_strength':
                        require(float(child.findtext('Bonus', 'nan')) == 1 and child.findtext('Trait') == ET.fromstring(installed[key]['definition']).find('Effects/Effect').get('trait'),
                                'Grade-one strength did not boost the exact configured native attribute by one.')
                    if key == 'arm.spell.fleet_step':
                        require(math.isclose(float(child.findtext('Multiplier', 'nan')), 1/1.05, rel_tol=1e-12),
                                'Grade-one Fleet Step did not save the selected-grade native movement delay multiplier.')
                        effort = ET.fromstring(active[0]['child_definitions'][1])
                        require(math.isclose(float(effort.findtext('Multiplier', 'nan')), .95, rel_tol=1e-12),
                                'Grade-one Fleet Step did not save its reduced native stamina expenditure.')
                    if key == 'arm.spell.venom_touch':
                        child_origin = child.findtext('DoseOriginId')
                        doses = venom_doses(row)
                        require(child_origin and 1 <= len(doses) <= 2 and
                                all(x['origin'].casefold() == child_origin.casefold() for x in doses) and
                                0 < sum(x['grams'] for x in doses) <= .001000001,
                                'Grade-one paid Venom Touch did not save its exact native owned dose.')
                        result['dose_origin'] = child_origin
                        result['phases']['first']['doses'] = doses
                    result['saved_parents'] = active; result['saved_at_monotonic'] = time.monotonic()
                result['outcome'] = 'FIRST-PASS-COLD-PENDING'
                break
            require(payload.get('outcome') in ('MinorFail', 'Fail', 'MajorFail'), 'Campaign failed outside its defining RNG result: ' + key)
        require(result['outcome'] == 'FIRST-PASS-COLD-PENDING', 'Campaign finite RNG budget exhausted: ' + key)
        result['saved_state'] = state(row); result['saved_operations'] = operations(row)
        assertions.append('Seeded representative paid grade-one ' + key + ': actual enrolment, native Self funding, one debit per attempt and defining result')
    report['status'] = 'PASS' if phase == 'cold' and all(row['outcome'] == 'PASS' for row in report['selected']) else 'FIRST-PASS-COLD-PENDING'

def food_bites(definition):
    # These catalogue provisions use the native Food component's Bites element.
    return float(ET.fromstring(definition).findtext('Bites', 'nan'))

def liquid_volume(definition):
    return sum(float(row.attrib['amount']) for row in ET.fromstring(definition).findall('./Mix/Liquid'))

def trait_values(character_id, body_id, trait_id):
    scope = sql(f'SELECT Type,OwnerScope FROM TraitDefinitions WHERE Id={trait_id}')
    require(len(scope) == 1 and scope[0][0] in ('0', '1', '2', '3', '4') and scope[0][1] in ('0', '1'),
            'Trait has no supported native type/owner scope: ' + repr(scope))
    # TraitDefinition(model) explicitly keeps attributes on bodies and normalises
    # legacy Body(0) skill scopes to Character(1). Follow that loader contract.
    trait_type, stored_scope = map(int, scope[0])
    effective_scope = 0 if trait_type in (1, 3) else 1
    receipt.setdefault('trait_owner_scopes', {})[str(trait_id)] = {
        'type':trait_type, 'stored':stored_scope, 'effective':effective_scope}
    if effective_scope == 1:
        return sql(f'SELECT Value FROM CharacterTraits WHERE CharacterId={character_id} AND TraitDefinitionId={trait_id}')
    return sql(f'SELECT Value FROM Traits WHERE BodyId={body_id} AND TraitDefinitionId={trait_id}')

def native_first_boot_scenario(session):
    records = assert_catalogue_inventory()
    # Builder syntax uses a quoted multi-word skill name and the native skill-level command.
    craft_trait = records['arm.support.component_crafting.skill'][2]
    output = session.send(f'skill level me {craft_trait} 100', read_seconds=2)
    require('100' in output or 'set the value' in output.lower() or 'add the' in output.lower(), 'Could not set numeric Component Crafting on the Admin avatar.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Crafting trait save flush did not confirm.')
    admin = character_state()
    admin_body = sql(f'SELECT BodyId FROM Characters WHERE Id={admin[1]}')
    require(len(admin_body) == 1 and admin_body[0][0].isdigit(), 'Admin avatar has no persisted body for native skill verification.')
    admin_skill = trait_values(admin[1], admin_body[0][0], craft_trait)
    require(len(admin_skill) == 1 and float(admin_skill[0][0]) == 100.0, 'Native numeric Component Crafting value 100 was not persisted at its declared owner scope.')
    assertions.append('Native owned Component Crafting trait set to exact numeric level 100')
    blank_id = records['arm.item.component_blank'][2]
    product_id = records['arm.material.abjuration.rank0'][2]
    input_before = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={blank_id}')[0][0])
    product_before = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={product_id}')[0][0])
    recipe_name = sql(f"SELECT Name FROM Crafts WHERE Id={records['arm.material.abjuration.rank0.recipe'][2]}")
    require(len(recipe_name) == 1 and recipe_name[0][0], 'Native recipe row has no exact command name.')
    phase_rows = sql(f"SELECT PhaseLengthInSeconds FROM CraftPhases WHERE CraftPhaseId={records['arm.material.abjuration.rank0.recipe'][2]} ORDER BY PhaseNumber")
    require(len(phase_rows) == 1 and math.isfinite(float(phase_rows[0][0])) and 0 < float(phase_rows[0][0]) <= 60, 'Craft phase is missing or exceeds bounded wait.')
    receipt['craft_attempts'] = []
    # Native checks can legitimately spoil a blank even at skill 100. Retry only
    # that exact outcome, with a fresh loaded input and independent conservation
    # checks on every attempt. Success remains required within three attempts.
    for attempt in range(1, 4):
        output = session.send('item load ' + item_command_reference(blank_id), read_seconds=2)
        require('you load ' in output.lower(), 'Native item load did not acknowledge the recipe input.')
        output = session.send('impdebug flush', read_seconds=3)
        require('All queued saves have been flushed' in output, 'Loaded craft input flush did not confirm.')
        input_loaded = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={blank_id}')[0][0])
        require(input_loaded == input_before + 1, 'Native item load did not persist exactly one recipe input.')
        held_input = sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id WHERE gi.GameItemProtoId={blank_id} AND bgi.BodyId={admin_body[0][0]}')
        require(len(held_input) == 1 and held_input[0][0].isdigit(), 'Loaded recipe input is not in the Admin avatar inventory.')
        output = session.send('craft begin "' + recipe_name[0][0] + '"', read_seconds=2)
        require('prepar' in output.lower() or 'craft' in output.lower(), 'Native craft begin did not accept the exact ranked recipe name.')
        output += session.read_for(math.ceil(float(phase_rows[0][0])) + 3)
        flushed = session.send('impdebug flush', read_seconds=3)
        require('All queued saves have been flushed' in flushed, 'Craft save flush did not confirm.')
        input_after = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={blank_id}')[0][0])
        product_after = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE GameItemProtoId={product_id}')[0][0])
        consumed = int(sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={held_input[0][0]}')[0][0]) == 0
        require(input_after == input_before and consumed, 'Craft did not consume exactly its loaded blank input.')
        receipt['craft_attempts'].append({'attempt': attempt, 'input': int(held_input[0][0]),
                                         'consumed': consumed, 'product_count': product_after})
        if product_after == product_before + 1:
            receipt['craft_attempts'][-1]['outcome'] = 'success'
            break
        require(product_after == product_before and 'spoil the prepared materials' in output.lower(),
                'Craft failed without the exact native spoil outcome and unchanged product count.')
        receipt['craft_attempts'][-1]['outcome'] = 'native-spoil'
    require(product_after == product_before + 1, 'Craft did not persist exactly one native product within three attempts.')
    assertions.append('Native exact ranked craft persisted product and consumed its loaded input after bounded phase and flush')
    session.send('drop all', read_seconds=2)

    # Item and character instances are loaded through proven prototype references, never inserted by SQL.
    knife_id = records['arm.item.flame_knife.g1'][2]
    knife_reference = item_command_reference(knife_id)
    session.send('item load ' + knife_reference, read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Loaded melee weapon flush did not confirm.')
    attack_trait = sql(f"SELECT AttackTraitId FROM WeaponTypes WHERE Id={records['arm.item.flame_knife.g1.weapon'][2]}")
    require(len(attack_trait) == 1 and attack_trait[0][0].isdigit(), 'Native WeaponType has no exact attack trait reference.')
    attack_trait_id = int(attack_trait[0][0])
    output = session.send(f'skill level me {attack_trait_id} 100', read_seconds=2)
    require('100' in output or 'set the value' in output.lower() or 'add the' in output.lower(), 'Could not grant the exact native WeaponType AttackTrait.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Attack trait save flush did not confirm.')
    attack_value = trait_values(admin[1], admin_body[0][0], attack_trait_id)
    # Native Trait.Save stores Skill.Value, which is min(MaxValue, RawValue).
    # The installed mundane skill cap can be below the requested setup value.
    require(len(attack_value) == 1 and math.isfinite(float(attack_value[0][0])) and 0 < float(attack_value[0][0]) <= 100.0,
            'Exact native AttackTrait has no positive persisted capped value: ' + repr(attack_value))
    receipt['weapon_attack_trait'] = {'id':attack_trait_id, 'requested':100.0, 'persisted':float(attack_value[0][0])}
    knife_instance = sql(f"SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id WHERE gi.GameItemProtoId={knife_id} AND bgi.BodyId={admin_body[0][0]} ORDER BY gi.Id DESC LIMIT 1")
    require(len(knife_instance) == 1 and knife_instance[0][0].isdigit(), 'Loaded grade 1 knife is not persisted in the Admin Body inventory.')
    knife_item_id = int(knife_instance[0][0])
    wield = session.send('wield knife', read_seconds=2)
    require('not holding' not in wield.lower() and 'cannot wield' not in wield.lower() and 'why cannot' not in wield.lower(), 'Native wield command refused the grade 1 melee item.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Wielded melee instance flush did not confirm.')
    weapon_component_id = records['arm.item.flame_knife.g1.component'][2]
    native_weapon = sql(f"SELECT HEX(gc.Definition) FROM GameItemComponents gc WHERE gc.GameItemId={knife_item_id} AND gc.GameItemComponentProtoId={weapon_component_id}", (0,))
    require(len(native_weapon) == 1 and int(ET.fromstring(native_weapon[0][0]).findtext('Wielded', '0')) > 0, 'Exact loaded native melee component has no persisted Wielded bodypart location: ' + repr(native_weapon))
    wield_row = sql(f'SELECT Wielded FROM Bodies_GameItems WHERE BodyId={admin_body[0][0]} AND GameItemId={knife_item_id}')
    require(len(wield_row) == 1 and wield_row[0][0] not in ('', 'NULL', '0'), 'Native BodiesGameItems row has no saved wield location.')
    assertions.append('Loaded grade 1 melee instance has exact native component Definition and BodiesGameItems Wielded location')

    creature_rows = {}
    creature_names = {}
    for key in ('arm.npc.earth_mount.g1', 'arm.npc.air_guardian.g7'):
        template = sql(f"SELECT UniqueName FROM NPCTemplates WHERE Id={records[key][2]}")
        require(len(template) == 1 and template[0][0], 'Creature template lacks exact native UniqueName: ' + key)
        creature_rows[key] = template[0][0]
        before_spawn = sql(f'SELECT COUNT(*) FROM Npcs WHERE TemplateId={records[key][2]}')
        session.send('npc load ' + template[0][0], read_seconds=3)
        output = session.send('impdebug flush', read_seconds=3)
        require('All queued saves have been flushed' in output, 'Spawned NPC flush did not confirm for ' + key)
        after_spawn = sql(f'SELECT COUNT(*) FROM Npcs WHERE TemplateId={records[key][2]}')
        require(before_spawn[0][0].isdigit() and after_spawn[0][0].isdigit() and int(after_spawn[0][0]) == int(before_spawn[0][0]) + 1,
                'Exact native NPC load command did not create one instance for ' + key)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Creature instance flush did not confirm.')
    for key, unique_name in creature_rows.items():
        spawned = sql(f"SELECT c.Id,c.Location,c.BodyId,b.RaceId FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id JOIN Bodies b ON b.Id=c.BodyId WHERE n.TemplateId={records[key][2]} ORDER BY c.Id DESC LIMIT 1")
        require(len(spawned) == 1 and len(spawned[0]) == 4, 'Spawned native character/body/race row missing for ' + key)
        receipt.setdefault('spawned_creatures', {})[key] = spawned[0]
        named = sql(f"SELECT c.Name FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE n.TemplateId={records[key][2]} ORDER BY c.Id DESC LIMIT 1")
        require(len(named) == 1 and named[0][0], 'Spawned NPC has no native character name for ' + key)
        # Native GetFromItemListByKeyword separates compound keywords with dots.
        # The fixed family name also appears in the native short description.
        creature_names[key] = '.'.join(named[0][0].lower().split())
        grade = 1 if key.endswith('.g1') else 7
        definition = sql(f"SELECT HEX(Definition) FROM NPCTemplates WHERE Id={records[key][2]}", (0,))
        template_xml = ET.fromstring(definition[0][0])
        selected_attributes = template_xml.findall('./SelectedAttributes/Attribute')
        require(selected_attributes and all(float(row.attrib['Value']) == 10 + 2 * grade for row in selected_attributes), 'NPC template native attribute values do not match approved grade scaling for ' + key)
        for attribute in selected_attributes:
            persisted = trait_values(spawned[0][0], spawned[0][2], int(attribute.attrib['Id']))
            require(len(persisted) == 1 and float(persisted[0][0]) == 10 + 2 * grade, 'Spawned Body did not retain its exact grade-scaled native attribute for ' + key + ': ' + repr(persisted))
        selected_skills = template_xml.findall('./SelectedSkills/Skill')
        expected_combat = 20 + 5 * grade
        combat_rows = [row for row in selected_skills if float(row.attrib['Value']) == expected_combat]
        require(combat_rows, 'NPC template has no expected grade-scaled native combat skill for ' + key)
        for skill in combat_rows:
            persisted = trait_values(spawned[0][0], spawned[0][2], int(skill.text))
            require(len(persisted) == 1 and float(persisted[0][0]) == expected_combat, 'Spawned Body did not retain the exact grade-scaled combat skill for ' + key + ': ' + repr(persisted))
        receipt.setdefault('spawned_creature_traits', {})[key] = {'attributes':len(selected_attributes),'combat_skill_value':expected_combat}
    assertions.append('Grade 1 mount and grade 7 winged template loads persisted native Character and Body identities')

    # Partial food and liquid exercise native inventory consumption; all outcomes are independently read after flush.
    food_reference = item_command_reference(records['arm.item.food.bread'][2])
    bottle_reference = item_command_reference(records['arm.item.drink_container'][2])
    session.send('item load ' + food_reference, read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Loaded meal flush did not confirm.')
    food_component_id = catalogue_record('arm.item.food.bread.component')[2]
    food_instance = sql(f"SELECT i.Id,HEX(gc.Definition) FROM GameItems i JOIN GameItemComponents gc ON gc.GameItemId=i.Id WHERE i.GameItemProtoId={records['arm.item.food.bread'][2]} AND gc.GameItemComponentProtoId={food_component_id} ORDER BY i.Id DESC LIMIT 1", (1,))
    require(len(food_instance) == 1 and len(food_instance[0]) == 2, 'Loaded meal has no native Food component instance.')
    food_instance_id, before_food_definition = food_instance[0]
    before_bites = food_bites(before_food_definition)
    eating = session.send('eat 1 flatbread', read_seconds=2)
    require('eat' in eating.lower() or 'bite' in eating.lower(), 'Native partial meal command produced no consumption evidence.')
    session.send('drop flatbread', read_seconds=2)
    session.send('item load ' + bottle_reference, read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Loaded bottle flush did not confirm.')
    bottle_component_id = catalogue_record('arm.item.drink_container.component')[2]
    bottle_instance = sql(f"SELECT i.Id,HEX(gc.Definition) FROM GameItems i JOIN GameItemComponents gc ON gc.GameItemId=i.Id WHERE i.GameItemProtoId={records['arm.item.drink_container'][2]} AND gc.GameItemComponentProtoId={bottle_component_id} ORDER BY i.Id DESC LIMIT 1", (1,))
    require(len(bottle_instance) == 1 and len(bottle_instance[0]) == 2, 'Loaded bottle has no native liquid-container component instance.')
    bottle_id, bottle_empty = bottle_instance[0]
    liquid_id = catalogue_record('arm.liquid.wine')[2]
    liquid_row = sql(f'SELECT Id FROM Liquids WHERE Id={liquid_id}')
    require(len(liquid_row) == 1, 'Native wine stable key does not resolve to one Liquid.')
    bottle_use = session.send(f'loadliquid {liquid_id} me bottle', read_seconds=2)
    require('load' in bottle_use.lower() or 'wine' in bottle_use.lower(), 'Native loadliquid command did not fill the supply bottle.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Filled bottle flush did not confirm.')
    bottle_filled = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={bottle_id} AND GameItemComponentProtoId={bottle_component_id} ORDER BY Id LIMIT 1', (0,))
    empty_volume = liquid_volume(bottle_empty)
    filled_volume = liquid_volume(bottle_filled[0][0]) if bottle_filled else math.nan
    require(len(bottle_filled) == 1 and math.isfinite(filled_volume) and filled_volume > empty_volume, 'Native liquid amount did not increase after bottle fill.')
    session.send('open bottle', read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Opened bottle flush did not confirm.')
    bottle_opened = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={bottle_id} AND GameItemComponentProtoId={bottle_component_id} ORDER BY Id LIMIT 1', (0,))
    require(len(bottle_opened) == 1 and ET.fromstring(bottle_opened[0][0]).attrib.get('Open') == 'true' and
            liquid_volume(bottle_opened[0][0]) == filled_volume, 'Native bottle opening did not persist its open state with unchanged contents.')
    drinking = session.send('drink bottle', read_seconds=2)
    require('closed containers' not in drinking.lower(), 'Native drink command refused the opened supply bottle.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Provision consumption flush did not confirm.')
    after_food = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={food_instance_id} AND GameItemComponentProtoId={food_component_id} ORDER BY Id LIMIT 1', (0,))
    after_bites = food_bites(after_food[0][0]) if after_food else math.nan
    require(len(after_food) == 1 and math.isfinite(before_bites) and math.isfinite(after_bites) and after_bites < before_bites,
            'Partial native meal did not reduce exact persisted BitesRemaining.')
    receipt['food_component_instance'] = [food_instance_id, before_bites, after_bites]
    bottle_after_drink = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={bottle_id} AND GameItemComponentProtoId={bottle_component_id} ORDER BY Id LIMIT 1', (0,))
    consumed_volume = liquid_volume(bottle_after_drink[0][0]) if bottle_after_drink else math.nan
    require(len(bottle_after_drink) == 1 and math.isfinite(consumed_volume) and consumed_volume < filled_volume, 'Drink did not reduce exact persisted liquid volume.')
    receipt['bottle_component_instance'] = [bottle_id, empty_volume, filled_volume, consumed_volume]
    assertions.append('Native food consumption flushed and independently checked after save')

    # A native attack is accepted only with persisted wound evidence; no transcript-only inference.
    mount = creature_names['arm.npc.earth_mount.g1']
    mount_template_id = records['arm.npc.earth_mount.g1'][2]
    hit = session.send('hit ' + mount, read_seconds=3)
    require('attack' in hit.lower() or 'hit' in hit.lower() or 'combat' in hit.lower(), 'Native combat attack command did not begin against the grade 1 mount.')
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Native attack flush did not confirm.')
    target_body = int(receipt['spawned_creatures']['arm.npc.earth_mount.g1'][2])
    receipt['combat_item_id'] = knife_item_id
    receipt['combat_target_body'] = target_body
    receipt['combat_target_native_refs'] = sql(f'SELECT b.Id,b.BodyPrototypeId,b.RaceId,r.BaseBodyId,r.DefaultHealthStrategyId,IFNULL(r.NaturalArmourTypeId,0),IFNULL(r.NaturalArmourMaterialId,0),r.BodypartHealthMultiplier FROM Bodies b JOIN Races r ON r.Id=b.RaceId WHERE b.Id={target_body}')
    receipt['combat_wound_observations'] = []
    wound_deadline = time.monotonic() + 30
    wound_rows = []
    while time.monotonic() < wound_deadline:
        session.send('impdebug flush', read_seconds=1)
        wound_rows = sql(f"SELECT Id FROM Wounds WHERE BodyId={target_body} AND ToolOriginId={knife_item_id}")
        observed = sql(f'SELECT Id,IFNULL(BodyId,0),IFNULL(ActorOriginId,0),IFNULL(ToolOriginId,0),DamageType,OriginalDamage,CurrentDamage FROM Wounds WHERE BodyId={target_body} OR ToolOriginId={knife_item_id} ORDER BY Id')
        if observed and observed not in receipt['combat_wound_observations']:
            receipt['combat_wound_observations'].append(observed)
        if wound_rows:
            break
        time.sleep(1)
    if not wound_rows:
        receipt['combat_all_wounds_at_failure'] = sql('SELECT Id,IFNULL(BodyId,0),IFNULL(ActorOriginId,0),IFNULL(ToolOriginId,0),DamageType,OriginalDamage,CurrentDamage FROM Wounds ORDER BY Id LIMIT 100')
    require(wound_rows, 'Native melee attack produced no persisted wound with this exact weapon as ToolOriginId within 30 seconds; observed=' + repr(receipt['combat_wound_observations']))
    assertions.append('Native attack produced a persisted target Wounds row with the exact loaded melee GameItem as ToolOriginId')
    peace = session.send('peace', read_seconds=2)
    require('peace' in peace.lower() or 'combat' in peace.lower(), 'Native peace command did not report combat cleanup.')
    # The melee fixture must release its hand before the later ordinary paid casts.
    session.send('unwield knife', read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Melee unwield save flush did not confirm.')
    unwielded = sql(f'SELECT IFNULL(Wielded,0) FROM Bodies_GameItems WHERE BodyId={admin_body[0][0]} AND GameItemId={knife_item_id}')
    require(len(unwielded) == 1 and unwielded[0][0] == '0', 'The exact melee fixture item remained wielded before casting.')
    session.send('drop knife', read_seconds=2)
    output = session.send('impdebug flush', read_seconds=3)
    require('All queued saves have been flushed' in output, 'Melee drop save flush did not confirm.')
    require(sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE BodyId={admin_body[0][0]} AND GameItemId={knife_item_id}')[0][0] == '0',
            'The exact melee fixture item still occupies the avatar inventory before casting.')

    # Create disposable rooms through the live builder and confirm an NPC moves via a real exit.
    package_name = 'S3 Movement ' + uuid.uuid4().hex
    session.send('room package new ' + package_name, read_seconds=2)
    session.send('impdebug flush', read_seconds=3)
    package = sql("SELECT Id FROM RoomOverlayPackages WHERE Name='" + package_name + "'")
    require(len(package) == 1 and package[0][0].isdigit(), 'Native builder did not persist one disposable overlay package.')
    dig = session.send('room dig east', read_seconds=3)
    if 'created' not in dig.lower() and 'exit' not in dig.lower() and 'room' not in dig.lower():
        require(False, 'Native room dig east did not create a disposable connected room; no NPC movement evidence is claimed.')
    else:
        output = session.send('impdebug flush', read_seconds=3)
        require('All queued saves have been flushed' in output, 'Disposable room/exit flush did not confirm.')
        home_location = receipt['initial_avatar'][3]
        moved_to = character_state()[3]
        require(moved_to != home_location, 'Native room dig did not move the avatar to a distinct room.')
        receipt['campaign_exit_destination'] = moved_to
        original_mount_location = receipt['spawned_creatures']['arm.npc.earth_mount.g1'][1]
        session.send('room set type outdoors', read_seconds=2)
        session.send('room package submit S3 disposable movement acceptance', read_seconds=2)
        session.send('room package review ' + package[0][0], read_seconds=2)
        session.send('accept edit S3 disposable movement acceptance', read_seconds=2)
        session.send('room package swap ' + package[0][0], read_seconds=2)
        session.send('west', read_seconds=2)
        output = session.send('impdebug flush', read_seconds=3)
        require('All queued saves have been flushed' in output and character_state()[3] == home_location,
                'Admin avatar did not return to its original Guest Lounge location before cold restart.')
        move = session.send('force ' + mount + ' east', read_seconds=3)
        require('force' in move.lower() or 'east' in move.lower() or 'move' in move.lower(), 'Could not command the spawned mount through the real east exit.')
        def await_native_location(character_id, expected, evidence_key):
            observations = []
            receipt[evidence_key] = observations
            deadline = time.monotonic() + 45
            rows = []
            while time.monotonic() < deadline:
                flush = session.send('impdebug flush', read_seconds=1)
                require('All queued saves have been flushed' in flush, 'Movement save flush did not confirm.')
                rows = sql(f'SELECT Location FROM Characters WHERE Id={character_id}')
                observations.append(rows)
                if len(rows) == 1 and rows[0][0] == str(expected):
                    return rows
                time.sleep(1)
            return rows

        moved = await_native_location(receipt['spawned_creatures']['arm.npc.earth_mount.g1'][0], moved_to, 'mount_movement_locations')
        require(len(moved) == 1 and moved[0][0] == str(moved_to) and moved[0][0] != str(original_mount_location), 'Forced NPC did not persist the expected changed Location after crossing the real exit.')
        assertions.append('Native force moved spawned NPC across builder-created room exit; persisted Location changed after flush')
        guardian = creature_names['arm.npc.air_guardian.g7']
        flight = session.send('force ' + guardian + ' fly', read_seconds=1)
        require('begins to fly' in flight.lower(), 'Native winged takeoff did not confirm.')
        # Fly() adds the real five-second BlockLayerChange movement blocker.
        # A forced command discards the NPC's private refusal, so capture it
        # through the staff AS output proxy while the blocker is still active.
        delay_description = 'prevented from entering the commands ascend'
        delay_before = session.send('effect list ' + guardian, read_seconds=0.5)
        require('Effects for' in delay_before and delay_description in delay_before.lower(),
                'Native takeoff did not install its declared movement cooldown.')
        refused = session.send('as ' + guardian + ' east', read_seconds=0.5)
        require('cannot move until you stop recovering from your recent swim, fly or climb' in refused.lower(),
                'Native takeoff cooldown did not refuse early east movement.')
        session.send('impdebug flush', read_seconds=3)
        guardian_id = receipt['spawned_creatures']['arm.npc.air_guardian.g7'][0]
        guardian_position = sql(f'SELECT PositionId FROM Characters WHERE Id={guardian_id}')
        require(len(guardian_position) == 1 and guardian_position[0][0] == '18', 'Winged creature did not enter native PositionFlying (18) using its working wings.')
        refused_location = sql(f'SELECT Location FROM Characters WHERE Id={guardian_id}')
        require(len(refused_location) == 1 and refused_location[0][0] == str(home_location),
                'A cooldown-refused movement changed the native guardian Location.')
        receipt['guardian_takeoff_cooldown'] = {'before': delay_before, 'refusal': refused,
                                               'refused_location': refused_location, 'observations': []}
        assertions.append('Native winged takeoff cooldown refused early movement and preserved its Location')
        delay_deadline = time.monotonic() + 10
        while time.monotonic() < delay_deadline:
            delay_after = session.send('effect list ' + guardian, read_seconds=0.5)
            require('Effects for' in delay_after, 'Native guardian effect inspection failed.')
            receipt['guardian_takeoff_cooldown']['observations'].append(delay_after)
            if delay_description not in delay_after.lower():
                break
        require(delay_description not in delay_after.lower(), 'Native takeoff cooldown did not expire within ten seconds.')
        session.send('force ' + guardian + ' east', read_seconds=3)
        guardian_location = await_native_location(guardian_id, moved_to, 'guardian_movement_locations')
        require(len(guardian_location) == 1 and guardian_location[0][0] == str(moved_to), 'Winged creature did not fly across the live room exit.')
        assertions.append('Native winged creature entered Flying posture and crossed the same approved room exit')

def boot(phase):
    # Leave one minute for owned teardown before the readiness parent's 45-minute limit.
    budget = 2640 if os.environ.get('FUTUREMUD_S6_READINESS_SMOKE_ONLY') == '1' else (
        1140 if os.environ.get('FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY') == '1' else 780)
    require(time.monotonic()-started < budget, 'Global boot smoke budget exhausted.')
    sql('SELECT 1')
    runtime = OUTPUT / ('runtime-' + phase + '-' + uuid.uuid4().hex)
    runtime.mkdir()
    mud_port = free_port()
    (runtime/'Connection.config').write_text(f'127.0.0.1\n{mud_port}\n127.0.0.1,::1\n',encoding='ascii')
    receipt['runtime_paths'].append(str(runtime))
    job = OwnedJob()
    jobs.append(job)
    env = os.environ.copy()
    for key in ('FUTUREMUD_PREPARED_REPLAY_BOOT_CONNECTION','FUTUREMUD_PREPARED_REPLAY_LOGIN_PASSWORD','MYSQL_PWD',
                'FUTUREMUD_PREPARED_REPLAY_PARENT_READY_PATH','FUTUREMUD_PREPARED_REPLAY_PARENT_READY_TOKEN'):
        env.pop(key,None)
    previous_mode = helper.windows_error_mode()
    try:
        process = subprocess.Popen(['dotnet',str(REPO/'MudSharpCore/bin/Debug/net10.0/MudSharp.dll'),'MySql.Data.MySqlClient',CONNECTION],
                                   cwd=runtime,env=env,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True,encoding='utf-8',errors='replace',
                                   creationflags=subprocess.CREATE_NO_WINDOW)
    finally: helper.restore_windows_error_mode(previous_mode)
    record = {'phase':phase,'pid':process.pid,'endpoint':f'127.0.0.1:{mud_port}','runtime':str(runtime),'stopped':False,'graceful':False}
    processes.append(record)
    collector = helper.OutputCollector(process, SECRETS)
    collectors.append((collector,runtime))
    try:
        try: job.assign(process)
        except BaseException:
            process.kill(); process.wait(timeout=20)
            raise
        require(collector.wait_for(helper.READY_TEXT,180), 'MUD did not reach ready state: '+collector.tail())
        assertions.append(phase+': native MUD ready')
        if phase == 'cold' and os.environ.get('FUTUREMUD_S5_DOSE_SMOKE_ONLY') == '1':
            dose_report = receipt['focus_devices']
            saved_wound = sql(f'SELECT CurrentDamage FROM Wounds WHERE Id={int(dose_report["wound"])}')
            dose_report['cold_wound_before_login'] = saved_wound
            require(len(saved_wound) == 1 and abs(float(saved_wound[0][0]) - dose_report['remaining_damage']) < 1e-7,
                    'Cold pre-login wound differs from exact saved topical remainder: ' + str(saved_wound))
            assertions.append('S5 topical: cold pre-login SQL retains exact post-dose wound remainder before native login healing')
        helper.wait_for_port('127.0.0.1',mud_port,10)
        session = helper.MudSocket('127.0.0.1',mud_port,SECRETS)
        try:
            session.read_for(1)
            session.send('l',read_seconds=1)
            session.send('Admin',read_seconds=1)
            session.send(PASSWORD,secret=True,read_seconds=1)
            session.send('c',read_seconds=1)
            session.send('1',read_seconds=3)
            looked = session.send('look',read_seconds=3)
            require('Guest Lounge' in looked or phase == 'cold' and (os.environ.get('FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY') == '1' or os.environ.get('FUTUREMUD_S4_REMAINING_SMOKE_ONLY') == '1' or os.environ.get('FUTUREMUD_S4_FAMILY_SMOKE_ONLY') == '1'), 'Native login/LOOK did not reach seeded Guest Lounge.')
            assertions.append(phase+': native Admin avatar login and LOOK')
            if phase == 'first' and BOOT_SCOPE == 'catalogue':
                if any(os.environ.get(key) == '1' for key in ('FUTUREMUD_S4_REMAINING_SMOKE_ONLY', 'FUTUREMUD_S4_FOCUS_DEVICE_SMOKE_ONLY', 'FUTUREMUD_S4_FAMILY_SMOKE_ONLY', 'FUTUREMUD_S5_PROGRESSION_SMOKE_ONLY', 'FUTUREMUD_S6_INTEGRATION_SMOKE_ONLY')):
                    pass  # Its mapped permanent destination was prepared before boot.
                elif os.environ.get('FUTUREMUD_S4_GATE_SMOKE_ONLY') == '1':
                    nilaz_gate_destination(session)
                elif os.environ.get('FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY') == '1':
                    nilaz_gate_destination(session, label='Exchange')
                else:
                    native_first_boot_scenario(session)
            elif phase == 'cold' and 'food_component_instance' in receipt:
                food_component_id = catalogue_record('arm.item.food.bread.component')[2]
                persisted_food = sql(f"SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={receipt['food_component_instance'][0]} AND GameItemComponentProtoId={food_component_id} ORDER BY Id LIMIT 1", (0,))
                require(len(persisted_food) == 1 and food_bites(persisted_food[0][0]) == receipt['food_component_instance'][2], 'Cold restart did not retain the exact partial meal BitesRemaining value.')
                assertions.append('Cold restart retained exact partially-consumed native Food component state')
                bottle_component_id = catalogue_record('arm.item.drink_container.component')[2]
                persisted_bottle = sql(f"SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={receipt['bottle_component_instance'][0]} AND GameItemComponentProtoId={bottle_component_id} ORDER BY Id LIMIT 1", (0,))
                require(len(persisted_bottle) == 1 and liquid_volume(persisted_bottle[0][0]) == receipt['bottle_component_instance'][3], 'Cold restart did not retain reduced numeric liquid volume.')
                assertions.append('Cold restart retained reduced native bottle liquid contents')
            if os.environ.get('FUTUREMUD_S4_ATTUNEMENT_BUILDER_SMOKE') == '1':
                attunement_builder_smoke(session, phase)
            if os.environ.get('FUTUREMUD_S4_ATTUNEMENT_STOCK_SMOKE') == '1':
                if os.environ.get('FUTUREMUD_S4_EXCHANGE_SMOKE_ONLY') == '1':
                    exchange_places_smoke(session, phase)
                elif os.environ.get('FUTUREMUD_S4_GATE_SMOKE_ONLY') == '1':
                    if phase == 'first':
                        nilaz_gate_smoke(session, json.loads(Path(os.environ['FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT']).read_text(encoding='utf-8')))
                    else: nilaz_gate_cold_smoke(session)
                else:
                    attunement_stock_smoke(session, phase)
            flush = session.send('impdebug flush',read_seconds=3)
            require('All queued saves have been flushed' in flush, 'Native save queue flush did not confirm.')
            state = character_state()
            require(state[4] != 'NULL', 'Native login time was not persisted after flush.')
            receipt[phase+'_after_flush'] = state
            assertions.append(phase+': native flush and independent persisted LastLoginTime')
            shutdown = session.send('shutdown stop',read_seconds=2)
            require('shutdown' in shutdown.lower(), 'Native shutdown command did not confirm.')
        finally:
            (runtime/'telnet-transcript.txt').write_text(session.full_transcript(),encoding='utf-8')
            session.close()
        try: process.wait(timeout=40)
        except subprocess.TimeoutExpired: raise RuntimeError('Native graceful shutdown did not stop within40 seconds.')
        require(process.returncode == 0 and job.active() == 0, 'Native shutdown did not cleanly stop its owned process job.')
        record['graceful'] = True
        state_after = character_state()
        require(state_after[:4] == state[:4] and state_after[5] != 'NULL', 'Shutdown persistence lost avatar/cell or LastLogoutTime.')
        receipt[phase+'_after_shutdown'] = state_after
        assertions.append(phase+': graceful shutdown, persisted avatar/cell and logout')
    finally:
        job.stop()
        process.wait(timeout=20)
        record['exit'] = process.returncode
        record['stopped'] = process.poll() is not None and job.active() == 0
        collector.thread.join(timeout=5)
        (runtime/'process-output.txt').write_text('\n'.join(collector.lines)+'\n',encoding='utf-8')

try:
    receipt['scope'] = BOOT_SCOPE
    if BOOT_SCOPE == 'attunement-builder':
        receipt['limitations'] = ['S4 builder registration/save and unchanged cold template loading only; '
                                  'no S3 crafting/combat/movement qualification and no paid installed-stock Attunement gameplay.']
    elif BOOT_SCOPE == 'attunement-stock':
        receipt['limitations'] = ['S4 canonical grade-one paid self cast and cold overlay reload through a separate disposable builder capability. '
                                  'No partial package roster activation, S3 gameplay matrix, native all-grade/ward/resistance or S5 learning/affordability certificate.']
    ready = Path(os.environ['FUTUREMUD_PREPARED_REPLAY_PARENT_READY_PATH']).resolve()
    ready_token = os.environ['FUTUREMUD_PREPARED_REPLAY_PARENT_READY_TOKEN']
    require(ready.parent == ROOT and ready.name.startswith('prepared-replay-boot-parent-ready_') and ready.suffix == '.txt' and
            re.fullmatch(r'[a-f0-9]{32}',ready_token), 'Invalid bounded parent startup receipt.')
    deadline = time.monotonic()+30
    confirmed = False
    while time.monotonic() < deadline:
        try: confirmed = ready.read_text(encoding='utf-8') == ready_token+':'+str(os.getpid())
        except OSError: pass
        if confirmed: break
        time.sleep(0.05)
    require(confirmed, 'Parent job attachment was not confirmed; refusing SQL/MUD startup.')
    receipt['parent_job_attachment_confirmed'] = True
    assertions.append('Matching parent-ready receipt before owned SQL or MUD startup')
    sql('SELECT 1')
    receipt['ownership_marker_sha256'] = hashlib.sha256(marker.encode()).hexdigest().upper()
    receipt['initial_avatar'] = character_state()
    discord_socket.bind(('127.0.0.1',0)) # Reserved but not listening: bridge fails locally, sends nowhere.
    discord_port = discord_socket.getsockname()[1]
    configuration = sql("UPDATE StaticConfigurations SET Definition='<EmailServer><Version>2</Version><Enabled>false</Enabled></EmailServer>' WHERE SettingName='EmailServer'; SELECT ROW_COUNT(); "
        "UPDATE StaticConfigurations SET Definition='false' WHERE SettingName='UseDiscordBot'; SELECT ROW_COUNT(); "
        "UPDATE StaticConfigurations SET Definition='127.0.0.1' WHERE SettingName='DiscordBotIpAddress'; SELECT ROW_COUNT(); "
        f"UPDATE StaticConfigurations SET Definition='{discord_port}' WHERE SettingName='DiscordBotPort'; SELECT ROW_COUNT(); "
        "SELECT SettingName,Definition FROM StaticConfigurations WHERE SettingName IN ('EmailServer','UseDiscordBot','DiscordBotIpAddress','DiscordBotPort') ORDER BY SettingName")
    settings = {row[0]:row[1] for row in configuration if len(row)==2}
    require(settings.get('EmailServer') == '<EmailServer><Version>2</Version><Enabled>false</Enabled></EmailServer>' and
            settings.get('UseDiscordBot') == 'false' and settings.get('DiscordBotIpAddress') == '127.0.0.1' and
            settings.get('DiscordBotPort') == str(discord_port), 'Owned-only external service isolation configuration failed.')
    receipt.update(external_email_disabled=True,discord_loopback_only=True,discord_reserved_endpoint=f'127.0.0.1:{discord_port}')
    assertions.append('Owned database only: disabled email and reserved non-listening loopback Discord bridge')
    receipt['script_sha256'] = sha(__file__)
    receipt['campaign_helper'] = {'path': str(campaign_path), 'sha256': sha(campaign_path)}
    receipt['helper_source'] = {'path':str(spec.origin),'sha256':sha(spec.origin)}
    receipt['assemblies'] = [{'path':str(REPO/'MudSharpCore/bin/Debug/net10.0'/name),'sha256':sha(REPO/'MudSharpCore/bin/Debug/net10.0'/name)}
                            for name in ('MudSharp.dll','FutureMUDLibrary.dll','MudsharpDatabaseLibrary.dll','ExpressionEngine.dll')]
    boot('first')
    if os.environ.get('FUTUREMUD_S4_FAMILY_CASES') == 'pocket':
        residual_spec = importlib.util.spec_from_file_location('s4_residual_cold', REPO / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS4ResidualSmoke.py')
        residual = importlib.util.module_from_spec(residual_spec)
        residual_spec.loader.exec_module(residual)
        residual.prepare_cold(globals(), json.loads(Path(os.environ['FUTUREMUD_S4_ATTUNEMENT_STOCK_INPUT']).read_text(encoding='utf-8')))
    boot('cold')
    require(receipt['first_after_shutdown'][:4] == receipt['cold_after_shutdown'][:4], 'Cold restart did not retain seeded avatar identity/cell.')
    require(receipt['cold_after_flush'][4] > receipt['first_after_flush'][4], 'Cold login did not advance persisted login time.')
    require(all(sha(row['path']) == row['sha256'] for row in receipt['assemblies']), 'Boot assembly source changed during qualification.')
    require(sha(__file__) == receipt['script_sha256'] and sha(spec.origin) == receipt['helper_source']['sha256'],
            'Boot script/helper source changed during qualification.')
    require(sha(campaign_path) == receipt['campaign_helper']['sha256'], 'Campaign helper changed during qualification.')
    receipt['status'] = 'PASS'
except BaseException as error:
    receipt['first_failure'] = redact(error)
    receipt['failure_traceback'] = redact(traceback.format_exc())
finally:
    for job in jobs:
        try: job.close()
        except BaseException as cleanup:
            receipt['status'] = 'FAIL'
            receipt.setdefault('cleanup_failures',[]).append(redact(cleanup))
    discord_socket.close()
    receipt['owned_mud_processes_stopped'] = bool(processes) and all(row['stopped'] for row in processes) and not receipt.get('cleanup_failures')
    receipt['elapsed_seconds'] = round(time.monotonic()-started,3)
    (OUTPUT/'boot-latest.json').write_text(json.dumps(receipt,indent=2)+'\n',encoding='utf-8')
    print('ARMPREP-catalogue-boot-smoke='+json.dumps({'status':receipt['status'],'owned_mud_processes_stopped':receipt['owned_mud_processes_stopped'],
                                         'receipt':str(OUTPUT/'boot-latest.json'),'first_failure':receipt.get('first_failure')}))
sys.exit(0 if receipt['status']=='PASS' and receipt['owned_mud_processes_stopped'] else 1)
