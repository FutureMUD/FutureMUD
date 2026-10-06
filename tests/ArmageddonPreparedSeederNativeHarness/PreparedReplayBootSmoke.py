"""Bounded lane-local boot qualification while the replay harness owns its database."""
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
assert OUTPUT.parent == ROOT and OUTPUT.name.startswith('prepared-replay-boot-native')
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

def sql(statement):
    global marker
    env = os.environ.copy()
    env['MYSQL_PWD'] = sql_password
    command = [str(mysql), '--no-defaults', '--protocol=tcp', '--host=127.0.0.1', f'--port={port}',
               f'--user={user}', f'--database={database}', '--batch', '--raw', '--skip-column-names',
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
    return [line.split('\t') for line in lines[2:]]

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
           'limitations':['Boot/login/LOOK/flush/shutdown/cold login only; no Armageddon whole-repertoire certificate.']}
discord_socket = socket.socket()
started = time.monotonic()

def character_state():
    rows = sql("SELECT a.Id,c.Id,c.Name,c.Location,IFNULL(DATE_FORMAT(c.LastLoginTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL'),IFNULL(DATE_FORMAT(c.LastLogoutTime,'%Y-%m-%dT%H:%i:%s.%f'),'NULL') FROM Accounts a JOIN Characters c ON c.AccountId=a.Id WHERE a.Name='Admin' AND c.IsAdminAvatar=1 ORDER BY c.Id")
    require(len(rows) == 1 and len(rows[0]) == 6, 'Expected one legitimately seeded Admin avatar.')
    return rows[0]

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
    require(sha(__file__) == receipt['script_sha256'] and sha(REPO/'tests/ArmageddonPreparedSeederNativeHarness/PreparedReplayBootSmoke.py') == receipt['script_sha256'] and
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
sys.exit(0 if receipt['status']=='PASS' and receipt['owned_mud_processes_stopped'] else 1)
