"""Provision an owned MySQL instance, refresh/import the blank snapshot, seed and run weather acceptance."""
import argparse
import importlib.util
import json
import os
import pathlib
import re
import socket
import subprocess
import sys
import time
import uuid

sys.dont_write_bytecode = True
repo = pathlib.Path(__file__).resolve().parents[2]
parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--mysql-bin', type=pathlib.Path, default=pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'))
parser.add_argument('--no-build', action='store_true')
parser.add_argument('--use-existing-snapshot', action='store_true', help='Import the already refreshed repository snapshot without regenerating it.')
parser.add_argument('--resume-smoke', type=pathlib.Path, help='Restart an owned, stopped instance after a passing replay, then rerun smoke only.')
args = parser.parse_args()
base = (repo / '.artifacts/weather').resolve()
root = args.resume_smoke.resolve() if args.resume_smoke else base / ('native-' + uuid.uuid4().hex[:10])
assert root.is_relative_to(base), 'Only owned weather runs can be resumed.'
root.mkdir(parents=True, exist_ok=bool(args.resume_smoke))
temp = root / 'temp'; temp.mkdir(exist_ok=True)
os.environ['TEMP'] = str(temp); os.environ['TMP'] = str(temp)
flags = subprocess.CREATE_NO_WINDOW
process = None
instance = None
report = {'status': 'RUNNING', 'snapshot': 'NOT_RUN', 'replay': 'NOT_RUN', 'smoke': 'NOT_RUN'}

spec = importlib.util.spec_from_file_location('owned_mysql', repo / 'scripts/AuthoredCelestialSmokeWorld/native.py')
owned = importlib.util.module_from_spec(spec); spec.loader.exec_module(owned)

def run(command, **kwargs):
    return subprocess.run(command, cwd=repo, creationflags=flags, **kwargs)

def query(statement):
    result = run(instance['client'] + ['-e', statement], capture_output=True, text=True, timeout=30)
    if result.returncode: raise RuntimeError('Owned MySQL query failed: ' + result.stderr)
    return result.stdout.strip()

def identity():
    assert instance['database'] == 'authored_celestial_smoke'
    data = pathlib.Path(instance['data']).resolve()
    assert data.is_relative_to(root.resolve()) and pathlib.Path(query('SELECT @@datadir')).resolve() == data

def logged(command, filename, timeout=600, env=None):
    with (root / filename).open('w', encoding='utf-8') as output:
        result = run(command, stdout=output, stderr=subprocess.STDOUT, timeout=timeout, env=env)
    if result.returncode: raise RuntimeError(f'{filename}: process exit {result.returncode}')

try:
    if not args.no_build:
        logged(['dotnet', 'build', 'scripts/AuthoredCelestialSmokeWorld/AuthoredCelestialSmokeWorld.csproj',
                '-c', 'Debug', '-m:1', '-p:RestoreBuildInParallel=false', '-p:NuGetAudit=false'], 'build.log')
    mysql = root / 'mysql'; data = mysql / 'data'
    binary = args.mysql_bin.resolve()
    if args.resume_smoke:
        previous = json.loads((root / 'mysql-instance.json').read_text())
        assert pathlib.Path(previous['data']).resolve() == data.resolve() and previous['database'] == 'authored_celestial_smoke'
        replay = json.loads((root / 'native-replay.json').read_text())
        assert replay['firstRun'] == 'PASS' and replay['secondRun'] == 'REFUSED_WITHOUT_MUTATION'
    else:
        data.mkdir(parents=True)
        logged([str(binary / 'mysqld.exe'), '--no-defaults', '--initialize-insecure',
                '--basedir=' + str(binary.parent), '--datadir=' + str(data), '--log-error=' + str(mysql / 'mysql.err')], 'initialize.log', 180)
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0)); port = listener.getsockname()[1]
    with (mysql / 'process.log').open('w', encoding='utf-8') as output:
        process = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent),
            '--datadir=' + str(data), '--port=' + str(port), '--bind-address=127.0.0.1', '--mysqlx=0',
            '--character-set-server=utf8mb4', '--collation-server=utf8mb4_0900_ai_ci', '--max-allowed-packet=64M',
            '--log-error=' + str(mysql / 'mysql.err')], stdout=output, stderr=subprocess.STDOUT, creationflags=flags)
    client = [str(binary / 'mysql.exe'), '--no-defaults', '--protocol=TCP', '--host=127.0.0.1',
              '--port=' + str(port), '--user=root', '--batch', '--skip-column-names']
    instance = dict(port=port, pid=process.pid, data=str(data), database='authored_celestial_smoke', client=client)
    (root / 'mysql-instance.json').write_text(json.dumps(instance, indent=2), encoding='utf-8')
    for _ in range(120):
        if process.poll() is not None: raise RuntimeError('Owned MySQL exited.')
        if run(client + ['-e', 'SELECT 1'], capture_output=True, timeout=5).returncode == 0: break
        time.sleep(.5)
    else: raise RuntimeError('Owned MySQL startup exceeded 60 seconds.')
    identity()
    connection = f'server=127.0.0.1;port={port};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;'
    env = os.environ.copy()
    password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')).group(1)
    env['AUTHORED_SMOKE_CONNECTION'] = connection; env['AUTHORED_SMOKE_PASSWORD'] = password
    if not args.resume_smoke:
        env['FUTUREMUD_SNAPSHOT_CONNECTION_STRING'] = connection.replace('database=authored_celestial_smoke', 'database=weather_snapshot')
        if not args.use_existing_snapshot:
            logged(['dotnet', str(repo / 'DatabaseSeeder/bin/Debug/net10.0/DatabaseSeeder.dll'), '--refresh-blank-snapshot'], 'snapshot-refresh.log', 900, env)
            report['snapshot'] = 'REFRESHED'
        identity()
        query('CREATE DATABASE authored_celestial_smoke')
        snapshot = (repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql').read_text(encoding='utf-8-sig')
        imported = run(client, input=snapshot.replace('__FUTUREMUD_DATABASE__', instance['database']), capture_output=True, text=True, encoding='utf-8', timeout=180)
        (root / 'snapshot-import.log').write_text(imported.stdout + imported.stderr, encoding='utf-8')
        if imported.returncode: raise RuntimeError('Blank snapshot import failed.')
        columns = query("SELECT COLUMN_NAME,DATA_TYPE,COLUMN_DEFAULT FROM information_schema.COLUMNS WHERE TABLE_SCHEMA='authored_celestial_smoke' AND TABLE_NAME='weathercontrollers' AND COLUMN_NAME IN ('ForecastState','ForecastHorizonDays')")
        assert 'longtext' in columns and 'ForecastHorizonDays' in columns
        report['snapshot'] = 'EXISTING_SNAPSHOT_IMPORTED' if args.use_existing_snapshot else 'REFRESHED_AND_IMPORTED'
        report['forecastColumns'] = columns
        try:
            logged(['dotnet', str(repo / 'scripts/AuthoredCelestialSmokeWorld/bin/Debug/net10.0/AuthoredCelestialSmokeWorld.dll'),
                    str(data), str(root / 'native-replay.json')], 'native-replay.log', 1800, env)
        finally:
            log = root / 'native-replay.log'
            if log.exists(): log.write_text(log.read_text(encoding='utf-8').replace(password, '[REDACTED]').replace(connection, '[REDACTED]'), encoding='utf-8')
    else:
        report['snapshot'] = 'PREVIOUS_OWNED_IMPORT'
    report['replay'] = 'PASS'
    report['smoke'] = 'RUNNING'
    logged([sys.executable, '-B', '-u', str(pathlib.Path(__file__).with_name('smoke.py')), '--run-dir', str(root)], 'smoke.log', 960, env)
    report['smoke'] = 'PASS'; report['status'] = 'PASS'
except BaseException as error:
    if report['smoke'] == 'RUNNING': report['smoke'] = 'FAIL'
    report['status'] = 'FAIL'; report['error'] = str(error)
    raise
finally:
    if instance or process: owned.stop_owned_mysql(instance, process, identity, run, root)
    (root / 'verification.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(report['status'] + ': ' + str(root / 'verification.json'))
