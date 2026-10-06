"""Bounded installed Sense Enchantment qualification while the replay harness owns its database."""
from __future__ import annotations
import ctypes
from ctypes import wintypes
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import re
import socket
import subprocess
import sys
import time
import traceback
import uuid

ROOT = Path(__file__).resolve().parent
REPO = Path(os.environ['FUTUREMUD_PREPARED_REPLAY_REPOSITORY']).resolve()
OUTPUT = Path(os.environ['FUTUREMUD_PREPARED_REPLAY_BOOT_OUTPUT']).resolve()
CONNECTION = os.environ['FUTUREMUD_PREPARED_REPLAY_BOOT_CONNECTION']
PASSWORD = os.environ['FUTUREMUD_PREPARED_REPLAY_LOGIN_PASSWORD']
assert REPO == ROOT / 'FutureMUD-charged'
assert OUTPUT.parent == ROOT and OUTPUT.name.startswith('prepared-installed-sense-native')
assert not OUTPUT.exists(), 'Refusing to reuse a boot evidence directory.'
OUTPUT.mkdir()
spec = importlib.util.spec_from_file_location('lane_mud_session', REPO / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
assert spec.loader
spec.loader.exec_module(helper)
SECRETS = [CONNECTION, PASSWORD]

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
sql_identifiers = {}
sql_preflight_queries = None


def mapped_sql(statement):
    # Replace only SQL identifier tokens, never quoted values (including XML and setting names).
    return re.sub(r"'(?:''|[^'])*'|[A-Za-z_][A-Za-z_0-9]*",
                  lambda match: match[0] if match[0].startswith("'") else sql_identifiers.get(match[0], match[0]), statement)


def install_sql_mapping(schema):
    require(isinstance(schema, list) and len(schema) == 16, 'Missing bounded EF SQL schema contract.')
    for table in schema:
        for alias, name in [(table['alias'], table['table'])] + [(x['alias'], x['column']) for x in table['columns']]:
            require(isinstance(name, str) and re.fullmatch(r'[A-Za-z_][A-Za-z_0-9]*', name), 'Unsafe EF SQL identifier.')
            quoted = '`' + name + '`'
            require(alias not in sql_identifiers or sql_identifiers[alias] == quoted, 'Ambiguous unqualified EF SQL column: ' + alias)
            sql_identifiers[alias] = quoted


def sql(statement):
    global marker
    env = os.environ.copy()
    env['MYSQL_PWD'] = sql_password
    command = [str(mysql), '--no-defaults', '--protocol=tcp', '--host=127.0.0.1', f'--port={port}',
               f'--user={user}', f'--database={database}', '--default-character-set=utf8mb4', '--batch', '--raw', '--skip-column-names',
               '--execute=SELECT @@server_uuid, @@port, @@datadir; SELECT RunToken FROM __gathering_harness_ownership; ' + mapped_sql(statement)]
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
    rows = [line.split('\t') for line in lines[2:]]
    if sql_preflight_queries is not None:
        sql_preflight_queries.append({'sql': mapped_sql(statement), 'rows': len(rows), 'field_counts': sorted({len(x) for x in rows})})
    return rows

def decode_sql_text(value):
    # mysql's NULL sentinel is distinct from HEX('NULL') and HEX('') in this protocol.
    require(isinstance(value, str), 'SQL HEX field is not text.')
    if value == 'NULL': return None
    require(re.fullmatch(r'(?:[a-fA-F0-9]{2})*', value) is not None, 'Invalid SQL HEX text framing.')
    return bytes.fromhex(value).decode('utf-8', errors='strict')


def decoded_text_rows(statement, columns):
    rows = sql(statement)
    for row in rows:
        require(len(row) > max(columns), 'SQL text row has missing fields.')
        for column in columns: row[column] = decode_sql_text(row[column])
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
           'limitations':['Builder-prepared four-admission partial package; no whole repertoire, Water or device prerequisite closure.']}
discord_socket = socket.socket()
started = time.monotonic()

def avatar_query():
    return "SELECT a.Id,c.Id,HEX(c.Name),c.Location,IFNULL(DATE_FORMAT(c.LastLoginTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL'),IFNULL(DATE_FORMAT(c.LastLogoutTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') FROM Accounts a JOIN Characters c ON c.AccountId=a.Id WHERE a.Name='Admin' AND c.IsAdminAvatar=1 ORDER BY c.Id"

def character_state():
    rows = decoded_text_rows(avatar_query(), [2])
    require(len(rows) == 1 and len(rows[0]) == 6, 'Expected one legitimately seeded Admin avatar.')
    return rows[0]


import xml.etree.ElementTree as ET
installed = None

def xmlhex(value):
    return ET.fromstring(bytes.fromhex(value).decode('utf-8'))

def flush(session):
    require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=1), 'Native flush did not confirm.')

def state_query():
    c, s, r, cap, trait = (int(installed[k]) for k in ('character','spell','resource','capability','source_skill'))
    return (f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={c} AND MagicResourceId={r}),0), '
               f'IFNULL((SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={c} AND MagicSpellId={s}),0), '
               f'IFNULL((SELECT Value FROM CharacterTraits WHERE CharacterId={c} AND TraitDefinitionId={trait}),0), '
               f'(SELECT COUNT(*) FROM CharacterCastingEnrolments WHERE CharacterId={c} AND MagicCapabilityId={cap}), '
               f'HEX(EffectData) FROM Characters WHERE Id={c}')

def installed_state():
    s = installed['spell']
    rows = sql(state_query())
    require(len(rows)==1 and len(rows[0])==5, 'Installed avatar state missing.')
    balance, grade, raw, enrol, effects = rows[0]
    parents = []
    root = xmlhex(effects)
    for wrapper in root.iter('Effect'):
        if wrapper.findtext('Type') != 'MagicSpellParent': continue
        inner = wrapper.find('Effect')
        if inner is None or inner.findtext('Spell') != str(s): continue
        children = inner.find('Children')
        require(children is not None and len(children)>0, 'Sense parent has no saved native children.')
        parents.append({'identity':inner.findtext('Identity'), 'remaining_ms':int(wrapper.findtext('Remaining','0')),
                        'child_types':[x.findtext('Type') for x in children]})
    return {'balance':float(balance), 'grade':int(grade), 'raw_skill':float(raw), 'enrolments':int(enrol), 'parents':parents}

def operations_query():
    c, s = int(installed['character']), int(installed['spell'])
    return f'SELECT HEX(Id),HEX(Stage),HEX(Definition),HEX(Diagnostic) FROM MagicCastingOperations WHERE CharacterId={c} AND MagicSpellId={s} ORDER BY CreatedUtc,Id'

def operations():
    return decoded_text_rows(operations_query(), [0,1])

def gathering_ids_query():
    return f"SELECT HEX(Id) FROM MagicGatheringOperations WHERE OwnerId={installed['character']}"

def gathering_query():
    return f"SELECT HEX(Id),HEX(Status),HEX(Kind),RequestedAmount,StaminaCost,BodilyCostApplied,DestinationCredited,AccountingPersisted FROM MagicGatheringOperations WHERE OwnerId={installed['character']} AND MagicCapabilityId={installed['capability']} AND DestinationResourceId={installed['resource']}"

def spell_query():
    return f"SELECT HEX(Definition) FROM MagicSpells WHERE Id={installed['spell']}"

def configuration_query():
    return "SELECT HEX(SettingName),HEX(Definition) FROM StaticConfigurations WHERE SettingName IN ('EmailServer','UseDiscordBot','DiscordBotIpAddress','DiscordBotPort') ORDER BY SettingName"

def configuration_updates(discord_port):
    require(type(discord_port) is int and 0 < discord_port < 65536, 'Invalid loopback bridge port.')
    return ["UPDATE StaticConfigurations SET Definition='<EmailServer><Version>2</Version><Enabled>false</Enabled></EmailServer>' WHERE SettingName='EmailServer'",
            "UPDATE StaticConfigurations SET Definition='false' WHERE SettingName='UseDiscordBot'",
            "UPDATE StaticConfigurations SET Definition='127.0.0.1' WHERE SettingName='DiscordBotIpAddress'",
            f"UPDATE StaticConfigurations SET Definition='{discord_port}' WHERE SettingName='DiscordBotPort'"]

def gather(session, phase, amount=None, reason='phase casting budget'):
    maximum = int(installed['gather_amount'])
    amount = maximum if amount is None else amount
    require(type(amount) is int and 0 < amount <= maximum, 'Gathering amount exceeds the finite authored acceptance budget.')
    c, cap, resource = (int(installed[k]) for k in ('character','capability','resource'))
    before = installed_state()['balance']
    require(before+amount <= float(installed['native_capacity']), 'Gathering would exceed actual seeded attribute capacity.')
    old = {row[0] for row in decoded_text_rows(gathering_ids_query(), [0])}
    session.send(f'armsense gather {cap} methods', read_seconds=.3)
    session.send(f'armsense gather {cap} preview draw {amount}', read_seconds=.3)
    action_start = time.monotonic()
    session.send(f'armsense gather {cap} draw {amount}', read_seconds=.2)
    require(time.monotonic()-action_start < 2 and installed_state()['balance']==before, 'Timed gathering credited before its authored duration.')
    deadline = time.monotonic()+20
    rows = []
    while time.monotonic()<deadline:
        session.read_for(1)
        flush(session)
        rows = [x for x in decoded_text_rows(gathering_query(), [0,1,2]) if x[0] not in old]
        if rows: break
    record = {'phase':phase,'before':before,'requested_amount':amount,'reason':reason,'rows':rows,'elapsed_seconds':time.monotonic()-action_start}
    receipt.setdefault('gathering',[]).append(record)
    require(len(rows)==1 and rows[0][1:3]==['Completed','Self'], 'Timed paid Self gathering did not complete exactly once.')
    require(float(rows[0][3])==amount and float(rows[0][4])==1 and rows[0][5:]==['1','1','1'], 'Paid gathering accounting/cost receipt mismatch.')
    record['after'] = installed_state()['balance']
    require(record['after']==before+amount, 'Paid gathering did not persist exactly its requested credit.')
    assertions.append(phase+': zero-injection timed paid Self gathering; fixed one-stamina action price')

def cast(session, phase, route):
    cap, resource = int(installed['capability']), int(installed['resource'])
    command = f'armsense cast "Sense Enchantment" grade 1 on self via {cap}' if route=='command' else f'say wek fm-self fm-magic fm-detect fm-open on self via {cap}'
    for attempt in range(1,4):
        before = installed_state()
        if before['balance']==0:
            # The installed grade-one Say route costs 50, independently checked below.
            # Fund one permitted paid attempt through the same native two-second Self action.
            gather(session,phase,50,'depleted bounded casting attempt')
            before = installed_state()
        require(before['balance']>=50, 'Installed grade-one casting budget is insufficient before its bounded paid attempt.')
        old = {x[0] for x in operations()}
        output = session.send(command, read_seconds=1)
        flush(session)
        rows = [x for x in operations() if x[0] not in old]
        record = {'phase':phase,'route':route,'attempt':attempt,'command':command,'output':redact(output),'operations':rows,'before':before}
        receipt.setdefault('casting_attempts',[]).append(record)
        require(len(rows)==1, 'Native invocation did not produce exactly one new paid operation: '+redact(output))
        require(rows[0][1]=='Completed', 'Native paid operation unresolved: '+str(rows))
        payload = xmlhex(rows[0][2])
        speech = payload.find('Speech')
        require(payload.get('grade')=='1' and payload.get('controlledGrade')=='1' and payload.get('masteryEligible')=='false', 'Unexpected grade/overreach/mastery receipt.')
        require(speech is not None and speech.get('kind')==('GeneratedCasting' if route=='command' else 'PlayerInput') and speech.get('method')=='Say' and speech.get('language')==str(installed['language']), 'Native speech route/language receipt mismatch.')
        costs = [x for x in payload.findall('Cost') if x.get('resource')==str(resource)]
        require(len(costs)==1 and float(costs[0].get('amount'))==50, 'Installed grade-one Say route must retain exactly one source-bound 50-unit reserve cost.')
        after = installed_state()
        record['after'] = after
        record['reserve_debit'] = float(costs[0].get('amount'))
        record['payload'] = dict(payload.attrib)
        require(abs(before['balance']-after['balance']-float(costs[0].get('amount')))<1e-8, 'Native casting did not persist exactly one debit.')
        require(after['grade']==1 and after['enrolments']==1, 'Casting changed controlled grade/enrolment unexpectedly.')
        if payload.get('applied')=='true':
            require(len(after['parents'])==1 and after['parents'][0]['identity'], 'Successful Sense casting did not retain one native parent and children.')
            assertions.append(phase+': '+route+' real grade-one casting, one paid operation/debit, native Sense effect')
            return
        require(payload.get('outcome') in ('MinorFail','Fail','MajorFail'), 'Non-random native effect failure requires investigation.')
        require([(x['identity'],x['child_types']) for x in after['parents']]==[(x['identity'],x['child_types']) for x in before['parents']],
                'Ordinary paid random failure changed native Sense effect identity/children.')
        record['ordinary_paid_failure'] = True
        assertions.append(phase+': '+route+' ordinary paid random failure retained its debit and existing Sense identity/children')
    raise RuntimeError('Three ordinary paid random failures exhausted the finite casting bound; no success certificate.')

def exercise_installed(session, phase):
    flush(session)
    before = installed_state()
    receipt[phase+'_installed_before'] = before
    if phase=='first':
        require(before['balance']==0 and before['grade']==0 and before['raw_skill']==0 and before['enrolments']==0, 'Admin was mutated/granted/refilled before explicit native commands.')
        session.send(f"givemerit me {installed['merit']}", read_seconds=1)
        session.send(f"magic casting enrol {installed['character']} {installed['capability']} owned disposable installed acceptance", read_seconds=1)
        flush(session)
        enrolled = installed_state()
        receipt['native_enrolment'] = enrolled
        require(enrolled['balance']==0 and enrolled['grade']==1 and enrolled['raw_skill']==60 and enrolled['enrolments']==1, 'Native enrolment did not grant legitimate Sense opening 60/grade1 with zero reserve.')
        session.send(f"magic spell edit {installed['spell']}", read_seconds=.5)
        response = session.send(f"magic spell set grades incantation fixture {installed['language']} fm-self fm-magic fm-detect fm-open fm-sense", read_seconds=.5)
        require('Incantation policy updated' in response, 'Native builder formula authoring failed: '+redact(response))
        session.send('magic spell set grades incantation provenance Native acceptance aliases; historical POWER word wek', read_seconds=.5)
        session.send('magic spell set grades show', read_seconds=.5)
        flush(session)
        definition = xmlhex(sql(spell_query())[0][0])
        incantation = definition.find('.//Incantation')
        require(incantation is not None and incantation.get('language')==str(installed['language']) and incantation.get('reach')=='fm-self', 'Native formula/language edits not persisted.')
        receipt['authored_incantation'] = ET.tostring(incantation,encoding='unicode')
        gather(session,phase)
    else:
        saved = receipt['first_installed_after']
        require(all(before[k]==saved[k] for k in ('balance','grade','raw_skill','enrolments')), 'Cold restart changed saved reserve/acquisition/skill/enrolment.')
        require(len(before['parents'])==1 and before['parents'][0]['identity']==saved['parents'][0]['identity'], 'Cold restart lost/recreated the saved Sense parent.')
        require(before['parents'][0]['child_types']==saved['parents'][0]['child_types'] and before['parents'][0]['remaining_ms']<=saved['parents'][0]['remaining_ms'], 'Cold restart reset Sense duration/children.')
        assertions.append('cold: saved reserve, permanent acquisition, raw skill, enrolment and Sense identity/children retained without refill')
        require(before['balance']==0, 'Cold affordability control requires the saved depleted reserve.')
        old_operations = operations()
        refusal = session.send(f'armsense cast "Sense Enchantment" grade 1 on self via {installed["capability"]}', read_seconds=1)
        flush(session)
        refused_state = installed_state()
        receipt['cold_depletion_refusal'] = {'output':redact(refusal),'before':before,'after':refused_state,
                                              'operations_before':old_operations,'operations_after':operations()}
        require('Insufficient Installed Sense Reserve' in refusal and operations()==old_operations,
                'Cold depleted invocation did not refuse without a paid operation.')
        require(all(refused_state[k]==before[k] for k in ('balance','grade','raw_skill','enrolments')) and
                [x['identity'] for x in refused_state['parents']]==[x['identity'] for x in before['parents']],
                'Depleted refusal changed saved reserve/acquisition/skill/enrolment or Sense identity.')
        assertions.append('cold: depleted reserve refused without debit, paid operation or acquisition/effect identity change')
        gather(session,phase)
    session.send(f"speak \"{installed['language_name']}\"", read_seconds=.5)
    cast(session,phase,'command')
    cast(session,phase,'speech')
    flush(session)
    receipt[phase+'_installed_after'] = installed_state()

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
            exercise_installed(session,phase)
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


def preflight_schema_sql():
    # These are exact physical EF names and store types, checked against this fresh owned server.
    def normal_type(value):
        return re.sub(r'\b(tinyint|smallint|mediumint|int|bigint)\(\d+\)', r'\1', value.lower())
    for table in installed['sql_schema']:
        rows = sql("SELECT COLUMN_NAME,COLUMN_TYPE,IS_NULLABLE FROM information_schema.COLUMNS "
                   f"WHERE TABLE_SCHEMA='{database}' AND TABLE_NAME='{table['table']}'")
        actual = {row[0]: row[1:] for row in rows}
        for column in table['columns']:
            require(column['column'] in actual, 'Missing mapped SQL column: ' + table['table'] + '.' + column['column'])
            value = actual[column['column']]
            require(normal_type(value[0]) == normal_type(column['store_type']) and (value[1] == 'YES') == column['nullable'],
                    'EF/fresh schema store type or nullability mismatch: ' + table['table'] + '.' + column['column'])


def preflight_text_transport():
    # Read-only derived rows prove the actual CLI protocol before any complete replay/MUD.
    # No configuration or player values are inserted or changed for this fixture.
    cases = [('01-multiline\tÉcho\n', '<Definition>\n\tÉλ漢😀\nbackslash: \\ and literal \\n\n</Definition>'),
             ('02-empty', ''), ('03-null', None), ('04-literal-null', 'NULL')]
    def literal(value):
        return 'NULL' if value is None else "CONVERT(UNHEX('" + value.encode('utf-8').hex() + "') USING utf8mb4)"
    derived = ' UNION ALL '.join('SELECT ' + literal(name) + ' AS SettingName,' + literal(value) + ' AS Definition' for name,value in cases)
    statement = 'SELECT HEX(SettingName),HEX(Definition) FROM (' + derived + ') AS TransportFixture ORDER BY SettingName'
    raw = sql(statement)
    require(len(raw)==len(cases) and all(len(row)==2 for row in raw), 'Multiline SQL text lost physical row framing.')
    decoded = [(decode_sql_text(row[0]),decode_sql_text(row[1])) for row in raw]
    require(decoded==cases, 'Multiline/tab/non-ASCII/empty/NULL SQL text roundtrip failed.')
    receipt['sql_transport_preflight'] = {'status':'PASS','physical_rows':len(raw),'fields_per_row':2,
        'null_distinct_from_empty_and_literal_null':True,'multiline_tab_unicode_roundtrip':True,
        'player_or_configuration_writes':False,'expected':cases,'decoded':decoded,'raw_hex_rows':raw}
    assertions.append('Actual owned SQL CLI HEX/UTF-8 protocol preserved multiline, tabs, Unicode, empty, NULL and literal NULL with exact row cardinality')


def preflight_installed_sql():
    global sql_preflight_queries
    sql_preflight_queries = []
    receipt['sql_preflight'] = {'status': 'FAIL', 'player_writes': False, 'mud_started': False,
                                'queries': sql_preflight_queries, 'ef_schema': installed['sql_schema']}
    preflight_schema_sql()
    preflight_text_transport()
    for key, table in [('character','Characters'), ('body','Bodies'), ('language','Languages'),
                       ('resource','MagicResources'), ('capability','MagicCapabilities'), ('merit','Merits'),
                       ('spell','MagicSpells'), ('source_skill','TraitDefinitions')]:
        rows = sql(f"SELECT Id FROM {table} WHERE Id={installed[key]}")
        require(rows == [[str(installed[key])]], 'Selected SQL identity cardinality mismatch: ' + key)
        id_column = next(x for x in installed['sql_schema'] if x['alias'] == table)['columns']
        require(next(x for x in id_column if x['alias'] == 'Id')['clr_type'] == 'System.Int64', 'Selected ID is not an EF Int64: ' + key)
    relations = sql(f"SELECT BodyId,NativeLanguageId FROM Characters WHERE Id={installed['character']}")
    require(relations == [[str(installed['body']),str(installed['language'])]], 'Selected avatar body/language mismatch.')
    skill = sql(f"SELECT Type,OwnerScope FROM TraitDefinitions WHERE Id={installed['source_skill']}")
    require(skill == [['0','1']], 'Selected source trait is not a character skill.')
    require(int(character_state()[1]) == installed['character'], 'Selected Admin avatar mismatch.')
    before = installed_state()
    require(before == {'balance':0.0,'grade':0,'raw_skill':0.0,'enrolments':0,'parents':[]}, 'Fresh installed player state already mutated.')
    require(not operations() and not decoded_text_rows(gathering_ids_query(), [0]) and not decoded_text_rows(gathering_query(), [0,1,2]), 'Fresh native operation receipts already exist.')
    spell = sql(spell_query())
    require(len(spell) == 1 and len(spell[0]) == 1, 'Selected spell definition cardinality mismatch.')
    xmlhex(spell[0][0])
    configurations = decoded_text_rows(configuration_query(), [0,1])
    require(len(configurations) == 4 and len({x[0] for x in configurations}) == 4 and all(len(x)==2 for x in configurations),
            'Expected exactly four external service configuration rows.')
    # EXPLAIN validates the exact UPDATE plans without executing writes. ROW_COUNT and SELECT 1 have no table dependencies.
    for update in configuration_updates(1): sql('EXPLAIN ' + update)
    require(sql('SELECT ROW_COUNT()') == [['-1']] and sql('SELECT 1') == [['1']], 'SQL scalar preflight failed.')
    receipt['sql_preflight'].update(status='PASS', initial_state=before, selected_avatar=character_state(),
                                    query_count=len(sql_preflight_queries))
    sql_preflight_queries = None
    assertions.append('Read-only EF schema, every smoke query, typed selected identities and fresh cardinalities passed before MUD startup')

def preflight_blank_sql():
    global sql_preflight_queries
    sql_preflight_queries = []
    receipt['sql_preflight'] = {'status':'FAIL','scope':'fresh migrated schema only; no selected world identities',
                                'player_writes':False,'mud_started':False,'queries':sql_preflight_queries,
                                'ef_schema':installed['sql_schema']}
    preflight_schema_sql()
    preflight_text_transport()
    for query in [avatar_query(),state_query(),operations_query(),gathering_ids_query(),gathering_query(),spell_query(),configuration_query()]:
        require(sql(query) == [], 'Blank-schema SQL query unexpectedly found a player/configuration/operation.')
    for update in configuration_updates(1): sql('EXPLAIN ' + update)
    require(sql('SELECT ROW_COUNT()') == [['-1']] and sql('SELECT 1') == [['1']], 'Blank SQL scalar preflight failed.')
    receipt['sql_preflight'].update(status='PASS',query_count=len(sql_preflight_queries))
    sql_preflight_queries = None
    receipt['status'] = 'PASS'
    receipt['schema_only'] = True
    assertions.append('Every gameplay SQL query and isolation UPDATE plan passed on a blank owned migrated schema; no MUD started')


try:
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
    input_path = Path(os.environ['FUTUREMUD_PREPARED_INSTALLED_INPUT']).resolve()
    require(input_path.parent==ROOT and input_path.name.startswith('prepared-installed-sense-input_') and input_path.stat().st_size<2000000, 'Invalid bounded installed input.')
    installed = json.loads(input_path.read_text(encoding='utf-8-sig'))
    require(all(type(installed[k]) is int and 0<installed[k]<=9223372036854775807 for k in ('character','body','language','resource','capability','merit','spell','source_skill')), 'Installed IDs invalid.')
    install_sql_mapping(installed['sql_schema'])
    if installed.get('sql_schema_only') is True:
        preflight_blank_sql()
    else:
        receipt['initial_avatar'] = character_state()
        preflight_installed_sql()
        require(int(receipt['initial_avatar'][1])==installed['character'] and installed['initial_players_unchanged'], 'Installed avatar/preparation mismatch.')
        receipt['installed_input'] = {'path':str(input_path),'sha256':sha(input_path),'content':installed}
        discord_socket.bind(('127.0.0.1',0)) # Reserved but not listening: bridge fails locally, sends nowhere.
        discord_port = discord_socket.getsockname()[1]
        configuration = sql('; SELECT ROW_COUNT(); '.join(configuration_updates(discord_port)) + '; SELECT ROW_COUNT(); ' + configuration_query())
        settings = {decode_sql_text(row[0]):decode_sql_text(row[1]) for row in configuration if len(row)==2}
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
        require(sha(__file__) == receipt['script_sha256'] and sha(REPO/'tests/ArmageddonPreparedSeederNativeHarness/PreparedInstalledMagicSmoke.py') == receipt['script_sha256'] and
                sha(spec.origin) == receipt['helper_source']['sha256'], 'Boot script/helper source changed during qualification.')
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
    print('ARMPREP-boot-smoke='+json.dumps({'status':receipt['status'],'owned_mud_processes_stopped':receipt['owned_mud_processes_stopped'],
                                         'receipt':str(OUTPUT/'boot-latest.json'),'first_failure':receipt.get('first_failure')}))
sys.exit(0 if receipt['status']=='PASS' and (receipt['owned_mud_processes_stopped'] or (receipt.get('schema_only') and not processes)) else 1)
