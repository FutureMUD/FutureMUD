"""Owned MySQL upgrade/snapshot and Release account-security acceptance.

Run through AuthoredCelestialSmokeWorld/native.py. F05_SCHEMA_ONLY=1 precedes
the final embedded-snapshot rebuild/fast gate; normal mode resumes that world.
"""
from __future__ import annotations
import argparse
import contextlib
import hashlib
import importlib.util
import io
import json
import os
import pathlib
import re
import socket
import subprocess
import time
import uuid

p = argparse.ArgumentParser(description=__doc__)
p.add_argument('--run-dir', type=pathlib.Path, required=True)
args = p.parse_args()
repo = pathlib.Path(__file__).resolve().parents[2]
root = args.run_dir.resolve()
assert root.is_relative_to(repo / '.artifacts/authored-celestials')
instance = json.loads((root / 'mysql-instance.json').read_text())
assert pathlib.Path(instance['data']).resolve() == root / 'mysql/data'
assert instance['database'] == 'authored_celestial_smoke'
client = instance['client'] + ['--raw']
connection = f"server=127.0.0.1;port={int(instance['port'])};database=authored_celestial_smoke;uid=root;SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
bootstrap = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'SeederFoundations/DebugFoundationReplayProfiles.cs').read_text(encoding='utf-8-sig')).group(1)
chosen = 'Synthetic F05 chosen password!'
recovered = 'Synthetic F05 recovered password!'
self_changed = 'Synthetic F05 self changed password!'
secrets = [connection, bootstrap, chosen, recovered, self_changed]
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
runtime = root / ('accounts-' + uuid.uuid4().hex[:8])
runtime.mkdir()
report = {'status': 'RUNNING', 'scenario': 'F05 account security', 'assertions': []}
process = session = None
sessions = []
transcripts = []
flags = subprocess.CREATE_NO_WINDOW
deadline = time.monotonic() + 1800

def check(name, condition, detail=''):
    report['assertions'].append({'name': name, 'passed': bool(condition), 'detail': helper.redact(str(detail), secrets)})
    if not condition:
        raise AssertionError(name + ': ' + helper.redact(str(detail), secrets))

def sql(query, database=None):
    result = subprocess.run(client + ([database] if database else []) + ['-e', query], capture_output=True, text=True, creationflags=flags, timeout=60)
    if result.returncode:
        raise RuntimeError('Owned SQL operation failed: ' + helper.redact(result.stderr[:1000], secrets))
    return result.stdout.strip()

def identity():
    uid, data, port = sql('SELECT @@server_uuid, @@datadir, @@port').split('\t')
    check('owned UUID/datadir/port', uid == instance['server_uuid'] and pathlib.Path(data).resolve() == root / 'mysql/data' and int(port) == instance['port'])

def invoke(command, label, env=None):
    assert time.monotonic() < deadline
    with (runtime / (label + '.log')).open('w', encoding='utf-8') as output:
        done = subprocess.run(command, cwd=repo, env=env, stdout=output, stderr=subprocess.STDOUT, creationflags=flags, timeout=600)
    check(label, done.returncode == 0, 'exit ' + str(done.returncode))

def native_phase(upgrade):
    identity()
    env = os.environ.copy()
    env.update(FUTUREMUD_ACCOUNT_NATIVE_CONNECTION=connection.replace('authored_celestial_smoke', 'account_upgrade') if upgrade else connection,
               FUTUREMUD_ACCOUNT_NATIVE_DATADIR=instance['data'])
    invoke(['dotnet', 'test', 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj', '-c', 'Debug', '--no-build', '--no-restore', '-m:1',
            '-p:IncludeExtendedTests=true', '--settings', 'test-extended.runsettings', '--filter', 'FullyQualifiedName~AccountSecurityNative_',
            '--logger', 'trx;LogFileName=' + ('upgrade' if upgrade else 'prepare') + '.trx', '--results-directory', str(runtime / 'native-results')],
           'native-' + ('upgrade' if upgrade else 'prepare'), env)

def import_snapshot(path, database):
    identity()
    assert database in ('account_upgrade', 'account_snapshot_import')
    check('unused disposable schema ' + database, not sql("SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" + database + "'"))
    sql('CREATE DATABASE `' + database + '`')
    payload = path.read_text(encoding='utf-8-sig').replace('__FUTUREMUD_DATABASE__', database)
    result = subprocess.run(client, input=payload, capture_output=True, text=True, encoding='utf-8', creationflags=flags, timeout=300)
    check('snapshot import ' + database, result.returncode == 0, result.stderr[:1000])

def cmd(text, expected=None, secret=False, seconds=.4, target=None):
    assert time.monotonic() < deadline
    current = target or session
    answer = current.send(text, secret=secret, read_seconds=seconds)
    until = time.monotonic() + 10
    while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < until:
        answer += current.read_for(.3)
    if expected:
        check('secret input' if secret else text, re.search(expected, answer, re.I | re.S) is not None, expected)
    return answer

def new_session():
    answer = helper.MudSocket('127.0.0.1', listen_port, secrets)
    answer.read_for(.5)
    sessions.append(answer)
    return answer

def login(name, password, character=False, target=None):
    current = target or session
    cmd('l', target=current)
    cmd(name, target=current)
    result = cmd(password, r'Welcome,\s*' + re.escape(name), secret=True, seconds=1.5, target=current)
    check('password accepted for ' + name, 'password was incorrect' not in result.lower())
    if character:
        cmd('c', target=current)
        cmd('1', 'Guest Lounge', seconds=2, target=current)
    return result

def stop():
    global process, session
    for item in sessions:
        transcripts.append(item.full_transcript())
        item.close()
    sessions.clear()
    session = None
    if process:
        helper.stop_process_tree(process)
        process = None

def start(configuration='Release'):
    global process, session, listen_port
    identity()
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        listen_port = listener.getsockname()[1]
    (runtime / 'Connection.config').write_text(f'127.0.0.1\n{listen_port}\n\n', encoding='utf-8')
    # The general skill launcher deliberately targets Debug. This scenario must
    # exercise Release verification, so select the built configuration explicitly.
    previous_mode = helper.windows_error_mode()
    try:
        process = subprocess.Popen(['dotnet', 'run', '--project', str(repo / 'MudSharpCore/MudSharpCore.csproj'), '-c', configuration,
                                    '--no-build', '--no-restore', '--', 'MySql.Data.MySqlClient', connection], cwd=runtime,
                                   stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True, encoding='utf-8', errors='replace', creationflags=flags)
    finally:
        helper.restore_windows_error_mode(previous_mode)
    collector = helper.OutputCollector(process, secrets)
    if not collector.wait_for(helper.READY_TEXT, 180):
        raise RuntimeError('Owned MUD startup failed: ' + collector.tail())
    session = new_session()

try:
    identity()
    if os.environ.get('F05_SCHEMA_ONLY') == '1':
        baseline = repo / '.artifacts/f05-snapshot-baseline.sql'
        import_snapshot(baseline, 'account_upgrade')
        sql('INSERT IGNORE INTO authoritygroups SELECT * FROM authored_celestial_smoke.authoritygroups', 'account_upgrade')
        columns = sql("SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='account_upgrade' AND TABLE_NAME='accounts' ORDER BY ORDINAL_POSITION").splitlines()
        assert all(re.fullmatch(r'\w+', x) for x in columns)
        names = ','.join('`' + x + '`' for x in columns)
        sql(f"INSERT INTO accounts ({names}) SELECT {names} FROM authored_celestial_smoke.accounts WHERE Name='admin'", 'account_upgrade')
        native_phase(True)
        original = sql('SELECT @@GLOBAL.collation_server')
        collation = re.search(r'CREATE DATABASE[^\n]*\bCOLLATE\s+([a-zA-Z0-9_]+)', baseline.read_text()).group(1)
        assert re.fullmatch(r'[a-zA-Z0-9_]+', original)
        env = os.environ.copy()
        env['FUTUREMUD_SNAPSHOT_CONNECTION_STRING'] = connection.replace('authored_celestial_smoke', 'futuremud_f05_snapshot')
        try:
            sql("SET GLOBAL collation_server='" + collation + "'")
            invoke(['dotnet', 'DatabaseSeeder/bin/Debug/net11.0/DatabaseSeeder.dll', '--refresh-blank-snapshot'], 'snapshot-refresh', env)
        finally:
            sql("SET GLOBAL collation_server='" + original + "'")
        manifest = json.loads((repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.manifest.json').read_text())
        check('snapshot records F05 migration', manifest['LatestMigrationId'].endswith('_HardenAccountCredentialRecovery'))
        import_snapshot(repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql', 'account_snapshot_import')
        columns = sql("SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='account_snapshot_import' AND TABLE_NAME='accounts' AND COLUMN_NAME IN ('CredentialVersion','LoginAttemptWindowUtc','LoginAttempts','LoginBlockedUntilUtc','RecoveryAttempts','RecoveryCredentialVersion','RecoveryExpiresUtc','RecoveryIssuedUtc')")
        check('all eight security columns imported', columns == '8', columns)
        check('snapshot history', manifest['LatestMigrationId'] in sql('SELECT MigrationId FROM __EFMigrationsHistory', 'account_snapshot_import'))
        report.update(status='PASS', scope='schema upgrade/down/up and refreshed blank snapshot import; gameplay NOT_RUN')
        (root / 'account-schema.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    else:
        check('schema phase passed', json.loads((root / 'account-schema.json').read_text())['status'] == 'PASS')
        native_phase(False)
        sql("UPDATE staticconfigurations SET Definition='false' WHERE SettingName='DisableAccountRegistrationByEmail'", instance['database'])
        start()
        cmd('l\nAdmin\n' + bootstrap + '\nc\n1', 'Guest Lounge', secret=True, seconds=2)
        check('queued client login commands survive asynchronous authentication', True)
        cmd('registeraccount f05owner', 'register the account')
        check('manual registration persisted without email', sql("SELECT IsRegistered+0 FROM accounts WHERE Name='f05owner'", instance['database']) == '1')
        owner = new_session()
        login('f05owner', 'old', target=owner)
        check('legacy short password upgraded after Release verification', sql("SELECT LEFT(Password,10) FROM accounts WHERE Name='f05owner'", instance['database']) == '$argon2id$')
        cmd('p', 'previous password', target=owner)
        cmd('old', 'Enter a new password', secret=True, target=owner)
        cmd(self_changed, 'enter the new password again', secret=True, target=owner)
        cmd(self_changed, 'Password updated', secret=True, seconds=1, target=owner)
        cmd('', r'Welcome,\s*f05owner', target=owner)
        check('authenticated self change preserves its session', sql("SELECT CredentialVersion FROM accounts WHERE Name='f05owner'", instance['database']) == '1')
        cmd('setpassword f05owner', 'Enter the new account password')
        cmd(chosen, 'Enter the new password again', secret=True)
        cmd(chosen, 'Password updated', secret=True, seconds=1)
        cmd('look', 'Guest Lounge')
        check('other target session revoked', 'credentials were changed' in owner.read_for(2).lower())
        check('password replacement incremented epoch', sql("SELECT CredentialVersion FROM accounts WHERE Name='f05owner'", instance['database']) == '2')
        # A second session cannot replace the successful password with the old one.
        rejected = new_session()
        cmd('l', target=rejected); cmd('f05owner', target=rejected)
        cmd(self_changed, 'password was incorrect', secret=True, seconds=1, target=rejected)
        stop()
        start()
        login('Admin', bootstrap, True)
        owner = new_session()
        login('f05owner', chosen, target=owner)
        with contextlib.redirect_stdout(io.StringIO()):
            response = cmd('resetpassword f05owner code', 'Recovery code for f05owner:')
        match = re.search(r'Recovery code for f05owner: (\S+)\. Give it', response, re.I)
        check('implementor receives manual code without email', match is not None)
        code = match.group(1)
        secrets.append(code)
        recovery = new_session()
        cmd('r', target=recovery); cmd('2', target=recovery); cmd('f05owner', target=recovery)
        cmd(code, 'Enter the new account password', secret=True, target=recovery)
        cmd(recovered, 'Enter the new password again', secret=True, target=recovery)
        cmd(recovered, 'Password updated', secret=True, seconds=1, target=recovery)
        check('recovery closed an existing owner session', 'credentials were changed' in owner.read_for(2).lower())
        check('recovery consumed and epoch advanced', sql("SELECT CredentialVersion, RecoveryCode IS NULL FROM accounts WHERE Name='f05owner'", instance['database']) == '3\t1')
        replay = new_session()
        cmd('r', target=replay); cmd('2', target=replay); cmd('f05owner', target=replay)
        cmd(code, 'invalid, expired', secret=True, target=replay)
        cmd('impdebug flush', 'All queued saves have been flushed')
        stop()
        sql("UPDATE staticconfigurations SET Definition='true' WHERE SettingName='DisableAccountRegistrationByEmail'", instance['database'])
        start()
        login('Admin', bootstrap, True)
        owner = new_session()
        login('f05owner', recovered, target=owner)
        development = new_session()
        menu = login('f05development', 'old', target=development)
        check('disabled registration requirement admits email-free unregistered account', 'account is unregistered' not in menu.lower() and 'create a new character' in menu.lower())
        cmd('registeraccount f05development', 'register the account')
        check('explicit manual registration persists even when global requirement is disabled', sql("SELECT IsRegistered+0 FROM accounts WHERE Name='f05development'", instance['database']) == '1')
        before = sql("SELECT SHA2(Password,256),CredentialVersion FROM accounts WHERE Name='f05owner'", instance['database'])
        stop()
        start('Debug')
        support = 'Synthetic support impersonation input'
        secrets.append(support)
        login('f05owner', support)
        check('intentional Debug support login never overwrites credentials', before == sql("SELECT SHA2(Password,256),CredentialVersion FROM accounts WHERE Name='f05owner'", instance['database']))
        stop()
        report.update(status='PASS', scope='native upgrade/concurrency/KDF/worker checks, three Release boots plus one Debug boot; manual registration, legacy rehash, direct setter, session revocation, recovery/replay and email-free mode')
except BaseException as error:
    report.update(status='FAIL', error=helper.redact(str(error), secrets))
    raise
finally:
    stop()
    report['runtimeHashes'] = {configuration: hashlib.sha256((repo / f'MudSharpCore/bin/{configuration}/net11.0/MudSharp.dll').read_bytes()).hexdigest() for configuration in ('Debug','Release')}
    (runtime / 'transcript.txt').write_text(helper.redact('\n\n=== NEXT SESSION ===\n\n'.join(transcripts), secrets), encoding='utf-8')
    report['transcript'] = str((runtime / 'transcript.txt').relative_to(root))
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
