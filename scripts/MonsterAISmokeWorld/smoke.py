"""Bounded native Monster acceptance operator for an isolated, freshly seeded world.

Run through AuthoredCelestialSmokeWorld/native.py. Requests and responses are JSONL
files under the owned run directory; every assertion and command is retained.
No existing database or service is used. See README.md for the scenario contract.
"""
from __future__ import annotations
import argparse
import contextlib
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
parser.add_argument('--run-dir', type=pathlib.Path, required=True)
args = parser.parse_args()
root = args.run_dir.resolve()
assert root.is_relative_to((repo / '.artifacts/authored-celestials').resolve())
instance = json.loads((root / 'mysql-instance.json').read_text())
assert instance['database'] == 'authored_celestial_smoke'
assert pathlib.Path(instance['data']).resolve().is_relative_to(root)
replay = json.loads((root / 'native-replay.json').read_text())
assert replay['firstRun'] == 'PASS' and replay['secondRun'] == 'REFUSED_WITHOUT_MUTATION'
connection = f"server=127.0.0.1;port={instance['port']};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')).group(1)
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
runtime = root / ('monster-' + uuid.uuid4().hex[:8])
runtime.mkdir()


class CapturingCollector(helper.OutputCollector):
    def _read(self):
        if self.proc.stdout is None: return
        with (runtime / 'server-output.txt').open('a', encoding='utf-8') as output:
            for line in self.proc.stdout:
                clean = helper.redact(line.rstrip('\r\n'), self.secrets)
                self.lines.append(clean); self.queue.put(clean)
                output.write(clean + '\n'); output.flush()


helper.OutputCollector = CapturingCollector
(root / 'monster-runtime.txt').write_text(str(runtime), encoding='utf-8')
requests = runtime / 'requests.jsonl'
requests.touch()
responses = runtime / 'responses.jsonl'
report = {'status': 'RUNNING', 'assertions': [], 'completedScenarios': [], 'runtime': str(runtime)}
process = session = None
transcripts = []
deadline = time.monotonic() + 880
os.environ['FUTUREMUD_ENVIRONMENT'] = 'test'


def emit(value):
    with responses.open('a', encoding='utf-8') as output:
        output.write(helper.redact(json.dumps(value), [connection, password]) + '\n')


def check(label, condition, detail=''):
    report['assertions'].append(dict(name=label, passed=bool(condition), detail=detail))
    assert condition, label + ': ' + detail


def sql(query):
    assert query.lstrip().upper().startswith(('SELECT ', 'SHOW ')), 'Only independent read queries are accepted'
    common = dict(capture_output=True, text=True, creationflags=subprocess.CREATE_NO_WINDOW, timeout=30)
    identity = subprocess.run(instance['client'] + ['-e', 'SELECT @@datadir'], **common)
    assert identity.returncode == 0 and pathlib.Path(identity.stdout.strip()).resolve() == pathlib.Path(instance['data']).resolve()
    result = subprocess.run(instance['client'] + [instance['database'], '--raw', '-e', query], **common)
    assert result.returncode == 0, result.stderr
    return result.stdout.strip()


def command(text, seconds=.25):
    assert time.monotonic() < deadline, 'Native acceptance deadline exceeded'
    with (runtime / 'operator.log').open('a', encoding='utf-8') as output, contextlib.redirect_stdout(output):
        return session.send(text, read_seconds=min(30, seconds))


def start():
    global process, session
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    (runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    process = helper.launch_mud(repo, runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), connection, [connection, password])
    session = helper.MudSocket('127.0.0.1', port, [connection, password])
    session.read_for(.5)
    command('l'); command('Admin'); session.send(password, secret=True, read_seconds=.5); command('c')
    answer = command('1', 2)
    check('administrator entered disposable world', 'Guest Lounge' in answer or 'Exits' in answer, answer[-1200:])
    command('impdebug freezetime')
    invisible = command('invis')
    if 'no longer' in invisible.lower(): command('invis')
    emit({'started': str(runtime)})


def stop():
    global process, session
    if session:
        command('impdebug flush', 1); command('shutdown stop', 2)
        process.wait(timeout=30)
        check('graceful server shutdown', process.returncode == 0, str(process.returncode))
        transcripts.append(session.full_transcript()); session.close(); session = None
    if process:
        helper.stop_process_tree(process); process = None


try:
    check('all 12 stock Monster profiles seeded', sql("SELECT COUNT(*) FROM ArtificialIntelligences WHERE Type='Monster' AND Name LIKE 'Monster - %'") == '12')
    start()
    cursor = 0
    finished = False
    while time.monotonic() < deadline and not finished:
        lines = requests.read_text(encoding='utf-8-sig').splitlines()
        for line in lines[cursor:]:
            request = json.loads(line)
            cursor += 1
            try:
                if request.get('restart'):
                    stop(); start()
                if 'sql' in request:
                    answer = sql(request['sql'])
                    emit({'request': cursor, 'sql': answer})
                    if 'expect' in request:
                        check(request.get('label', request['sql']), re.search(request['expect'], answer, re.I | re.S) is not None, answer[-2000:])
                for step in request.get('commands', []):
                    if isinstance(step, str): step = {'text': step}
                    answer = command(step['text'], step.get('seconds', .25))
                    emit({'request': cursor, 'command': step['text'], 'output': answer})
                    if 'expect' in step:
                        check(step.get('label', step['text']), re.search(step['expect'], answer, re.I | re.S) is not None, answer[-2000:])
                if 'read' in request:
                    answer = session.read_for(min(30, request['read']))
                    emit({'request': cursor, 'output': answer})
                    if 'expect' in request:
                        check(request.get('label', 'native output'), re.search(request['expect'], answer, re.I | re.S) is not None, answer[-2000:])
                if 'observe' in request:
                    observation = request['observe']
                    until = min(deadline, time.monotonic() + min(60, observation.get('seconds', 45)))
                    answer = ''
                    while time.monotonic() < until:
                        answer += session.read_for(min(1, until - time.monotonic()))
                        if re.search(observation['expect'], answer, re.I | re.S): break
                    emit({'request': cursor, 'output': answer})
                    check(observation.get('label', 'bounded native observation'),
                        re.search(observation['expect'], answer, re.I | re.S) is not None, answer[-4000:])
                if 'completedScenario' in request:
                    report['completedScenarios'].append(request['completedScenario'])
                finished = request.get('finish', False)
            except Exception as error:
                emit({'request': cursor, 'error': str(error)})
            if session:
                (runtime / 'transcript-current.txt').write_text(session.full_transcript(), encoding='utf-8')
            (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
        if not finished: time.sleep(.2)
    check('operator finished within deadline', finished)
    check('six scenarios have explicit acceptance receipts', len(set(report['completedScenarios'])) == 6)
    check('all recorded assertions passed', all(x['passed'] for x in report['assertions']))
    stop()
    report['status'] = 'PASS'
except BaseException as error:
    report['status'] = 'FAIL'
    report['error'] = helper.redact(str(error), [connection, password])
finally:
    if session:
        transcripts.append(session.full_transcript()); session.close()
    if process: helper.stop_process_tree(process)
    (runtime / 'transcript.txt').write_text(helper.redact('\n'.join(transcripts), [connection, password]), encoding='utf-8')
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(report['status'] + ': ' + str(runtime / 'receipt.json'))
sys.exit(0 if report['status'] == 'PASS' else 1)
