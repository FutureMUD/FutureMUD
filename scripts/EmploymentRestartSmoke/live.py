"""Adaptive Telnet acceptance on an owned disposable MySQL world.

Run through AuthoredCelestialSmokeWorld/native.py. Requests and responses stay
in the run's employment-live-* directory; no default game database is used.
"""
from __future__ import annotations

import argparse
import contextlib
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

sys.dont_write_bytecode = True
repo = pathlib.Path(__file__).resolve().parents[2]
sys.path.insert(0, str(pathlib.Path(__file__).resolve().parent))
from lifecycle_fixture import seed_lodged_remains_fixture

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('--run-dir', required=True, type=pathlib.Path)
parser.add_argument('--plan', type=pathlib.Path,
                    help='Preload bounded read-only/command requests from an acceptance JSONL file.')
args = parser.parse_args()
root = args.run_dir.resolve()
if not root.is_relative_to((repo / '.artifacts/authored-celestials').resolve()):
    raise RuntimeError('Expected an owned disposable run directory.')
instance = json.loads((root / 'mysql-instance.json').read_text())
data = pathlib.Path(instance['data']).resolve()
if data != root / 'mysql/data' or instance['database'] != 'authored_celestial_smoke':
    raise RuntimeError('Unexpected owned database identity.')
client = instance['client'] + ['--raw']
connection = ('server=127.0.0.1;port={};database=authored_celestial_smoke;uid=root;'
              'SslMode=Disabled;AllowPublicKeyRetrieval=True;Default Command Timeout=600;').format(instance['port'])
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"',
    (repo / 'SeederFoundations/DebugFoundationReplayProfiles.cs').read_text(encoding='utf-8-sig')).group(1)
secrets = [connection, password]
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
runtime = root / ('employment-live-' + uuid.uuid4().hex[:12])
runtime.mkdir()
(root / 'employment-live-runtime.txt').write_text(str(runtime), encoding='utf-8')
requests = runtime / 'requests.jsonl'
plan_arg = args.plan or (pathlib.Path(os.environ['FUTUREMUD_LIVE_PLAN'])
                         if os.environ.get('FUTUREMUD_LIVE_PLAN') else None)
if plan_arg:
    plan_path = plan_arg.resolve()
    if not plan_path.is_relative_to(root):
        raise RuntimeError('Acceptance plan must be stored in the exact owned disposable run directory.')
    requests.write_text(plan_path.read_text(encoding='utf-8'), encoding='utf-8')
else:
    requests.touch()
responses = runtime / 'responses.jsonl'
report = {'status': 'RUNNING', 'scenario': 'employment live Telnet restart', 'runtime': str(runtime),
          'assertions': [], 'completedScenarios': [], 'boots': [], 'shutdowns': []}
process = session = None
current_port = None
transcripts = []
deadline = time.monotonic() + 3300
os.environ['FUTUREMUD_ENVIRONMENT'] = 'test'


def emit(value):
    with responses.open('a', encoding='utf-8') as output:
        output.write(helper.redact(json.dumps(value), secrets) + '\n')


def check(label, condition, detail=''):
    report['assertions'].append({'name': label, 'passed': bool(condition),
        'detail': helper.redact(str(detail), secrets)})
    if not condition:
        raise AssertionError(label + ': ' + str(detail))


def identity():
    result = subprocess.run(client + ['-e', 'SELECT @@server_uuid, @@datadir, @@port'],
        capture_output=True, text=True, creationflags=subprocess.CREATE_NO_WINDOW, timeout=30)
    fields = result.stdout.strip().split('\t')
    if (result.returncode or len(fields) != 3 or fields[0] != instance['server_uuid'] or
            pathlib.Path(fields[1]).resolve() != data or int(fields[2]) != instance['port']):
        raise RuntimeError('Owned MySQL identity changed; refusing database operations.')


def sql(query, fixture=False):
    identity()
    client_options = []
    if fixture:
        if process is not None:
            raise RuntimeError('Fixture mutations require the owned MUD to be stopped.')
        normalized_query = re.sub(r'\s+', ' ', query.strip()).upper()
        permitted_fixture = normalized_query.startswith(('CREATE TRIGGER QA_EMPLOYMENT_',
                'DROP TRIGGER QA_EMPLOYMENT_', 'UPDATE EMPLOYMENTACTIVETASKS ',
                'UPDATE EMPLOYMENTACTIVETASKSTEPSTATES '))
        item_delete_trigger = re.fullmatch(
            r"CREATE TRIGGER QA_ITEM_DELETE_([1-9][0-9]*) BEFORE DELETE ON GAMEITEMS FOR EACH ROW "
            r"BEGIN IF OLD\.ID = \1 THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'QA_ITEM_DELETE_FAILURE'; END IF; END",
            normalized_query) or re.fullmatch(r'DROP TRIGGER QA_ITEM_DELETE_[1-9][0-9]*', normalized_query)
        if not permitted_fixture and item_delete_trigger is None:
            raise RuntimeError('Fixture operation is outside the employment crash/legacy scenario.')
        if item_delete_trigger and normalized_query.startswith('CREATE'):
            client_options.append('--delimiter=//')
    elif not query.lstrip().upper().startswith(('SELECT ', 'SHOW ')):
        raise RuntimeError('Only read queries are accepted while the MUD is live.')
    answer = subprocess.run(client + client_options + [instance['database'], '-e', query], capture_output=True,
        text=True, encoding='utf-8', errors='replace', creationflags=subprocess.CREATE_NO_WINDOW, timeout=60)
    if answer.returncode:
        raise RuntimeError(helper.redact(answer.stderr[:2000], secrets))
    if fixture:
        report.setdefault('fixtureOperations', []).append(query)
    return answer.stdout.strip()


class CapturingCollector(helper.OutputCollector):
    def _read(self):
        if self.proc.stdout is None:
            return
        with (runtime / ('server-output-' + str(len(report['boots'])) + '.log')).open('a', encoding='utf-8') as output:
            for line in self.proc.stdout:
                clean = helper.redact(line.rstrip('\r\n'), self.secrets)
                self.lines.append(clean)
                self.queue.put(clean)
                output.write(clean + '\n')
                output.flush()


helper.OutputCollector = CapturingCollector


def command(text, seconds=.4):
    if session is None:
        raise RuntimeError('No live Telnet session.')
    with (runtime / 'operator.log').open('a', encoding='utf-8') as output, contextlib.redirect_stdout(output):
        return session.send(text, read_seconds=min(10, seconds))


def start():
    global process, session, current_port
    identity()
    if process is not None:
        raise RuntimeError('The owned MUD is already running.')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    current_port = port
    (runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    with (runtime / 'operator.log').open('a', encoding='utf-8') as output, contextlib.redirect_stdout(output):
        process = helper.launch_mud(repo, runtime,
            argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), connection, secrets)
        report['boots'].append({'pid': process.pid, 'port': port,
            'binarySha256': hashlib.sha256((repo / 'MudSharpCore/bin/Debug/net11.0/MudSharp.dll').read_bytes()).hexdigest()})
        session = helper.MudSocket('127.0.0.1', port, secrets)
        session.read_for(.5)
        session.send('l', read_seconds=.3)
        session.send('Admin', read_seconds=.3)
        session.send(password, secret=True, read_seconds=.5)
        session.send('c', read_seconds=.3)
        answer = session.send('1', read_seconds=8)
    check('administrator entered world, boot ' + str(len(report['boots'])),
        'Guest Lounge' in answer or 'Exits' in answer, answer[-1500:])
    command('impdebug freezetime')
    emit({'started': str(runtime), 'boot': len(report['boots']), 'port': port})


def reconnect_character():
    global session
    if session is None or process is None or process.poll() is not None or current_port is None:
        raise RuntimeError('Character reconnect requires the same running MUD process.')
    original_pid = process.pid
    try:
        command('quit', 1)
    except (OSError, ConnectionError):
        # The server closes this socket as part of the normal quit flow.
        pass
    transcripts.append(session.full_transcript())
    session.close()
    session = helper.MudSocket('127.0.0.1', current_port, secrets)
    session.read_for(.5)
    session.send('l', read_seconds=.3)
    session.send('Admin', read_seconds=.3)
    session.send(password, secret=True, read_seconds=.5)
    session.send('c', read_seconds=.3)
    answer = session.send('1', read_seconds=8)
    check('same-process character logout/login', process.pid == original_pid and
          ('Guest Lounge' in answer or 'Exits' in answer), answer[-1500:])


def stop(crash=False):
    global process, session
    if process is None:
        return
    pid = process.pid
    if not crash:
        command('impdebug flush', 1)
        command('shutdown stop', 2)
        process.wait(timeout=30)
        check('graceful server shutdown', process.returncode == 0, str(process.returncode))
    if session:
        transcripts.append(session.full_transcript())
        session.close()
        session = None
    helper.stop_process_tree(process)
    report['shutdowns'].append({'pid': pid, 'intentionalCrash': crash, 'exitCode': process.returncode})
    process = None


try:
    start()
    cursor = 0
    finished = False
    while time.monotonic() < deadline and not finished:
        lines = requests.read_text(encoding='utf-8-sig').splitlines()
        for line in lines[cursor:]:
            request = json.loads(line)
            cursor += 1
            if request.get('crash'):
                stop(crash=True)
            if request.get('stop') or request.get('restart'):
                stop()
            if 'fixtureSql' in request:
                emit({'request': cursor, 'fixture': sql(request['fixtureSql'], fixture=True)})
            if 'seedLodgedRemainsFixture' in request:
                if process is not None:
                    raise RuntimeError('Lodged-remains fixtures require the owned MUD to be stopped.')
                identity()
                fixture = seed_lodged_remains_fixture(
                    client, instance['database'], process,
                    request['seedLodgedRemainsFixture']['parentItemId'],
                    request['seedLodgedRemainsFixture']['lodgedItemId'],
                    subprocess.CREATE_NO_WINDOW)
                report.setdefault('lifecycleFixtures', []).append(fixture)
                emit({'request': cursor, 'lifecycleFixture': fixture})
            if 'seedItemLodgingFixture' in request:
                if process is not None:
                    raise RuntimeError('Item-lodging fixtures require the owned MUD to be stopped.')
                identity()
                from lifecycle_fixture import seed_item_lodging_fixture
                fixture = seed_item_lodging_fixture(
                    client, instance['database'], process,
                    request['seedItemLodgingFixture']['parentItemId'],
                    request['seedItemLodgingFixture']['lodgedItemId'],
                    subprocess.CREATE_NO_WINDOW)
                report.setdefault('lifecycleFixtures', []).append(fixture)
                emit({'request': cursor, 'itemLodgingFixture': fixture})
            if request.get('start') or request.get('restart'):
                start()
            if 'sql' in request:
                answer = sql(request['sql'])
                emit({'request': cursor, 'sql': answer})
                if 'expect' in request:
                    check(request.get('label', 'database assertion'), re.search(request['expect'], answer, re.I | re.S) is not None, answer[-4000:])
            for step in request.get('commands', []):
                if isinstance(step, str):
                    step = {'text': step}
                answer = command(step['text'], step.get('seconds', .4))
                if 'expect' in step:
                    until = min(deadline, time.monotonic() + 5)
                    while not re.search(step['expect'], answer, re.I | re.S) and time.monotonic() < until:
                        answer += session.read_for(.2)
                    check(step.get('label', step['text']), re.search(step['expect'], answer, re.I | re.S) is not None, answer[-4000:])
                if 'forbidden' in step:
                    check(step.get('label', step['text']) + ' suppressed', re.search(step['forbidden'], answer, re.I | re.S) is None, answer[-4000:])
                emit({'request': cursor, 'command': step['text'], 'output': answer})
            if request.get('reconnectCharacter'):
                reconnect_character()
                emit({'request': cursor, 'reconnectedCharacter': True, 'pid': process.pid})
            if request.get('expectItemDeleteFailure') or request.get('expectCheckpointedItemDeleteFailure'):
                if process is None:
                    raise RuntimeError('Expected a running server for the provider refusal.')
                checkpointed_delete = request.get('expectCheckpointedItemDeleteFailure', False)
                delete_stack_pattern = (r'(?:CommitCheckpointedNativeDeletion|DeleteReadyNativeItemRow)'
                                        if checkpointed_delete else r'CommitOrdinaryContainerDeletion')
                until = time.monotonic() + 15
                observed = False
                while time.monotonic() < until:
                    reports = [path.read_text(encoding='utf-8', errors='replace')
                               for path in runtime.glob('Fatal Exception*.txt')]
                    observed = any('QA_ITEM_DELETE_FAILURE' in log and
                                   re.search(delete_stack_pattern, log, re.I) is not None
                                   for log in reports)
                    if observed:
                        break
                    time.sleep(.2)
                path_name = 'checkpointed native deletion' if checkpointed_delete else 'ordinary native deletion'
                check('MySQL refused the ' + path_name, observed)
                stop(crash=True)
            if 'read' in request:
                answer = session.read_for(min(10, request['read']))
                emit({'request': cursor, 'output': answer})
            if 'completedScenario' in request:
                report['completedScenarios'].append(request['completedScenario'])
            finished = request.get('finish', False)
            if session:
                (runtime / 'transcript-current.txt').write_text(session.full_transcript(), encoding='utf-8')
            (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
            emit({'request': cursor, 'done': True})
        if not finished:
            time.sleep(.2)
    check('operator finished within deadline', finished)
    check('named acceptance scenario completed', bool(report['completedScenarios']))
    check('live acceptance assertions were supplied', len(report['assertions']) > len(report['boots']))
    check('all live assertions passed', all(x['passed'] for x in report['assertions']))
    stop()
    report['status'] = 'PASS'
except BaseException as error:
    report['status'] = 'FAIL'
    report['error'] = helper.redact(str(error), secrets)
    emit({'error': report['error']})
finally:
    if session and process and process.poll() is None:
        try:
            stop()
        except Exception as cleanup_error:
            report['gracefulCleanupError'] = helper.redact(str(cleanup_error), secrets)
    if session:
        transcripts.append(session.full_transcript())
        session.close()
    if process:
        helper.stop_process_tree(process)
    report['mudStopped'] = process is None or process.poll() is not None
    (runtime / 'transcript.txt').write_text(helper.redact('\n\n=== NEXT SESSION ===\n\n'.join(transcripts), secrets), encoding='utf-8')
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(report['status'] + ': ' + str(runtime / 'receipt.json'))
sys.exit(0 if report['status'] == 'PASS' else 1)
