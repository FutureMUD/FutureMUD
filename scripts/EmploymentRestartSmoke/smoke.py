"""Owned MySQL migration/snapshot and employment process/engine restart acceptance.

Run only through AuthoredCelestialSmokeWorld/native.py, which owns the server.
F03_SNAPSHOT_ONLY=1 performs the schema/snapshot phase before the final frozen build.
"""
from __future__ import annotations
import argparse
import hashlib
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

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--run-dir', required=True, type=pathlib.Path)
args = parser.parse_args()
repo = pathlib.Path(__file__).resolve().parents[2]
root = args.run_dir.resolve()
assert root.is_relative_to(repo / '.artifacts/authored-celestials')
instance = json.loads((root / 'mysql-instance.json').read_text(encoding='utf-8'))
instance['client'].append('--raw')
assert pathlib.Path(instance['data']).resolve() == root / 'mysql/data'
assert pathlib.Path(instance['client'][0]).name.lower() == 'mysql.exe'
assert instance['database'] == 'authored_celestial_smoke'
connection = f"server=127.0.0.1;port={int(instance['port'])};database=authored_celestial_smoke;uid=root;SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"',
                    (repo / 'SeederFoundations/DebugFoundationReplayProfiles.cs').read_text(encoding='utf-8-sig')).group(1)
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
tag = uuid.uuid4().hex[:8]
runtime = root / ('employment-' + tag)
runtime.mkdir()
report = {'status': 'RUNNING', 'scenario': 'F03 employment durability', 'assertions': [], 'run': tag}
process = session = None
transcripts = []
deadline = time.monotonic() + 1800
flags = subprocess.CREATE_NO_WINDOW

def check(label, condition, detail=''):
    report['assertions'].append({'name': label, 'passed': bool(condition), 'detail': detail})
    if not condition:
        raise AssertionError(label + ': ' + detail)

def sql(query, database=None):
    answer = subprocess.run(instance['client'] + ([database] if database else []) + ['-e', query],
                            capture_output=True, text=True, creationflags=flags, timeout=120)
    if answer.returncode:
        raise RuntimeError('Owned SQL command failed: ' + answer.stderr[:1000])
    return answer.stdout.strip()

def identity():
    fields = sql('SELECT @@server_uuid, @@datadir, @@port').split('\t')
    check('owned UUID/datadir/port', fields[0] == instance['server_uuid'] and
          pathlib.Path(fields[1]).resolve() == pathlib.Path(instance['data']).resolve() and int(fields[2]) == instance['port'])

def invoke(command, label, env=None, timeout=600):
    assert time.monotonic() < deadline
    with (runtime / (label + '.log')).open('w', encoding='utf-8') as output:
        result = subprocess.run(command, cwd=repo, env=env, stdout=output, stderr=subprocess.STDOUT,
                                creationflags=flags, timeout=timeout)
    check(label, result.returncode == 0, f'exit {result.returncode}; see {label}.log')

def native_phase(phase):
    identity()
    env = os.environ.copy()
    env.update(FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION=connection.replace('authored_celestial_smoke', 'employment_upgrade') if phase == 'upgrade' else connection,
               FUTUREMUD_EMPLOYMENT_NATIVE_DATADIR=instance['data'], FUTUREMUD_EMPLOYMENT_NATIVE_PHASE=phase,
               DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_CLI_USE_MSBUILD_SERVER='0', MSBUILDUSESERVER='0', MSBUILDDISABLENODEREUSE='1')
    invoke(['dotnet', 'test', 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj', '-c', 'Debug',
            '--no-build', '--no-restore', '-m:1', '-p:IncludeExtendedTests=true', '--settings', 'test-extended.runsettings',
            '--filter', 'FullyQualifiedName~EmploymentNative_ProcessRestart_PreservesApprovalAndRefusal',
            '--logger', f'trx;LogFileName={phase}.trx', '--results-directory', str(runtime / 'native-results')], 'native-' + phase, env)

def import_snapshot(path, database):
    identity()
    assert database in ('employment_upgrade', 'authored_celestial_snapshot_import')
    check('snapshot schema absent: ' + database,
          not sql("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" + database + "'"))
    sql('CREATE DATABASE `' + database + '`')
    payload = path.read_text(encoding='utf-8-sig').replace('__FUTUREMUD_DATABASE__', database)
    result = subprocess.run(instance['client'], input=payload, capture_output=True, text=True, encoding='utf-8',
                            creationflags=flags, timeout=300)
    check('snapshot import: ' + database, result.returncode == 0, result.stderr[:1000])

def command(text, expected=None, seconds=.4):
    assert time.monotonic() < deadline
    answer = session.send(text, read_seconds=seconds)
    until = time.monotonic() + 8
    while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < until:
        answer += session.read_for(.3)
    if expected:
        check(text, re.search(expected, answer, re.I | re.S) is not None, expected)
    return answer

def start():
    global process, session
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    (runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    process = helper.launch_mud(repo, runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), connection, [connection, password])
    session = helper.MudSocket('127.0.0.1', port, [connection, password])
    session.read_for(.5)
    command('l')
    command('Admin')
    session.send(password, secret=True, read_seconds=.4)
    command('c')
    command('1', 'Guest Lounge', seconds=2)
    command('impdebug freezetime', 'freeze all')

def stop():
    global process, session
    if session:
        transcripts.append(session.full_transcript())
        session.close()
        session = None
    if process:
        helper.stop_process_tree(process)
        process = None

def details(host, name):
    text = command(f'employment clan {host} tasks show {name}', 'Approval:')
    keys = ['Status', 'Assigned Employee', 'Correlation', 'Recorded Provenance', 'Source', 'Creator',
            'Priority', 'Approval', 'Authority', 'Approval limit / remaining', 'Sources', 'Revoked by']
    result = {}
    # The helper strips ANSI/MXP; allow terminal line wrapping in long scope/identity rows.
    for key in keys:
        match = re.search(r'^' + re.escape(key) + r':\s*(.*?)(?=\n(?:' + '|'.join(re.escape(k) for k in keys) + r'|Required Authority|Required AI Capabilities|Steps):|\Z)', text, re.M | re.S)
        if match:
            result[key] = ' '.join(match[1].split())
    return result

try:
    identity()
    if os.environ.get('F03_SNAPSHOT_ONLY') == '1':
        baseline = repo / '.artifacts/f03-snapshot-baseline.sql'
        import_snapshot(baseline, 'employment_upgrade')
        sql("INSERT INTO boards(Id,Name,ShowOnLogin) VALUES(991071,'F03 legacy',b'0');"
            "INSERT INTO employmenthoststates(Id,HostType,HostId,BoardId,CreatedAt,LastUpdatedAt) VALUES(991071,'Shop',991071,991071,'2026-11-16','2026-11-16');"
            "INSERT INTO employmentactionplans(Id,EmploymentHostStateId,Name) VALUES(991071,991071,'F03 legacy');"
            "INSERT INTO employmentactivetasks(Id,PublicId,EmploymentHostStateId,Name,EmploymentActionPlanId,Status,CorrelationId,IdempotencyKey) "
            "VALUES(991071,'7a0789e2-e0de-4c1e-b839-995f8c194f9e',991071,'F03 upgrade legacy',991071,0,'72eadf20-ab6d-4a55-a764-05e98de7ada6','f03-legacy-proof');", 'employment_upgrade')
        native_phase('upgrade')
        env = os.environ.copy()
        env['FUTUREMUD_SNAPSHOT_CONNECTION_STRING'] = connection.replace('authored_celestial_smoke', 'futuremud_f03_snapshot')
        # CREATE DATABASE inherits this private server's default. Preserve the maintained
        # snapshot's default rather than silently changing unrelated fresh-install tables.
        baseline_collation = re.search(r'CREATE DATABASE[^\n]*\bCOLLATE\s+([a-zA-Z0-9_]+)', baseline.read_text()).group(1)
        previous_collation = sql('SELECT @@GLOBAL.collation_server')
        assert re.fullmatch(r'[a-zA-Z0-9_]+', previous_collation)
        try:
            sql("SET GLOBAL collation_server='" + baseline_collation + "'")
            invoke(['dotnet', 'DatabaseSeeder/bin/Debug/net11.0/DatabaseSeeder.dll', '--refresh-blank-snapshot'], 'snapshot-refresh', env)
        finally:
            sql("SET GLOBAL collation_server='" + previous_collation + "'")
        manifest = json.loads((repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.manifest.json').read_text())
        check('snapshot migration', manifest['LatestMigrationId'].endswith('_PersistEmploymentTaskAuthorisation'))
        import_snapshot(repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql', 'authored_celestial_snapshot_import')
        count = sql("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='authored_celestial_snapshot_import' AND LOWER(TABLE_NAME) IN ('employmenttaskgrants','employmenttaskgrantlimits','employmenttaskgrantscopes')")
        check('three grant tables imported', count == '3', count)
        check('snapshot migration history', manifest['LatestMigrationId'] in sql('SELECT MigrationId FROM __EFMigrationsHistory', 'authored_celestial_snapshot_import'))
        report['scope'] = 'schema upgrade/down/up, model parity and refreshed snapshot import; gameplay NOT_RUN in this phase'
        report['status'] = 'PASS'
        (root / 'employment-schema.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    else:
        check('schema phase previously passed', json.loads((root / 'employment-schema.json').read_text())['status'] == 'PASS')
        for phase in ('write', 'read', 'revoked'):
            native_phase(phase)
        start()
        host, name = 'qaf03' + tag, 'F03Grant' + tag
        command(f'clan create new {host} {host}', 'You create the clan')
        command(f'employment clan {host} tasks create {name} authorise F03RestartApproval then paytax all', 'with 2 steps')
        original = details(host, name)
        check('recorded provenance', original.get('Recorded Provenance', '').lower() == 'true', str(original))
        check('approval UUID', re.search(r'[0-9a-f-]{36}', original.get('Approval', ''), re.I) is not None)
        command('impdebug flush', 'All queued saves have been flushed')
        stop()
        start()
        reloaded = details(host, name)
        check('normal loader preserves pending approval snapshot', original == reloaded, str(reloaded))
        command(f'employment clan {host} tasks revoke {name} F03RestartRevocation', 'Task approval revoked')
        revoked = details(host, name)
        check('revocation reason visible', 'F03RestartRevocation' in revoked.get('Revoked by', ''))
        command('impdebug flush', 'All queued saves have been flushed')
        stop()
        start()
        check('revocation survives normal loader cold restart', details(host, name) == revoked)
        stop()
        report.update(status='PASS', scope='three separate native store test-host processes and three normal engine boots; pending real clan approval/revocation preserved; no live payment was executed')
        report['snapshots'] = {'created': original, 'reloaded': reloaded, 'revoked': revoked}
except BaseException as error:
    report.update(status='FAIL', error=helper.redact(str(error), [connection, password]))
    raise
finally:
    stop()
    report['runtimeAssemblySha256'] = hashlib.sha256((repo / 'MudSharpCore/bin/Debug/net11.0/MudSharp.dll').read_bytes()).hexdigest()
    (runtime / 'transcript.txt').write_text('\n\n=== NEXT PROCESS ===\n\n'.join(transcripts), encoding='utf-8')
    report['transcript'] = str((runtime / 'transcript.txt').relative_to(root))
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
