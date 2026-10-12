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
SECRETS = [CONNECTION, PASSWORD]
BOOT_SCOPE = os.environ.get('FUTUREMUD_CATALOGUE_BOOT_SCOPE', 'catalogue')
assert BOOT_SCOPE in ('catalogue', 'attunement-builder'), 'Unknown catalogue boot scope.'
assert BOOT_SCOPE != 'attunement-builder' or os.environ.get('FUTUREMUD_S4_ATTUNEMENT_BUILDER_SMOKE') == '1', \
    'The Attunement-only lane requires its explicit builder assertions.'

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

def sql(statement, hex_columns=()):
    global marker
    env = os.environ.copy()
    env['MYSQL_PWD'] = sql_password
    command = [str(mysql), '--no-defaults', '--protocol=tcp', '--host=127.0.0.1', f'--port={port}',
               f'--user={user}', f'--database={database}', '--batch', '--skip-column-names',
               '--execute=SELECT @@server_uuid, @@port, @@datadir; SELECT RunToken FROM __gathering_harness_ownership; ' + statement]
    result = subprocess.run(command, env=env, stdout=subprocess.PIPE, stderr=subprocess.PIPE,
                            text=True, encoding='utf-8', errors='replace', timeout=40,
                            creationflags=subprocess.CREATE_NO_WINDOW)
    require(result.returncode == 0, 'Owned SQL failed: ' + redact(result.stderr))
    lines = result.stdout.rstrip('\r\n').splitlines()
    require(len(lines) >= 2, 'Missing owned SQL descriptor/marker.')
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
    require(time.monotonic()-started < 570, 'Global boot smoke budget exhausted.')
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
            require('Guest Lounge' in looked, 'Native login/LOOK did not reach seeded Guest Lounge.')
            assertions.append(phase+': native Admin avatar login and LOOK')
            if phase == 'first' and BOOT_SCOPE == 'catalogue':
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
    receipt['helper_source'] = {'path':str(spec.origin),'sha256':sha(spec.origin)}
    receipt['assemblies'] = [{'path':str(REPO/'MudSharpCore/bin/Debug/net10.0'/name),'sha256':sha(REPO/'MudSharpCore/bin/Debug/net10.0'/name)}
                            for name in ('MudSharp.dll','FutureMUDLibrary.dll','MudsharpDatabaseLibrary.dll','ExpressionEngine.dll')]
    boot('first')
    boot('cold')
    require(receipt['first_after_shutdown'][:4] == receipt['cold_after_shutdown'][:4], 'Cold restart did not retain seeded avatar identity/cell.')
    require(receipt['cold_after_flush'][4] > receipt['first_after_flush'][4], 'Cold login did not advance persisted login time.')
    require(all(sha(row['path']) == row['sha256'] for row in receipt['assemblies']), 'Boot assembly source changed during qualification.')
    require(sha(__file__) == receipt['script_sha256'] and sha(spec.origin) == receipt['helper_source']['sha256'],
            'Boot script/helper source changed during qualification.')
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
