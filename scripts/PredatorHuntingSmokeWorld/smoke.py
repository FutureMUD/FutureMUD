"""Bounded predator checks in the disposable world provisioned by AuthoredCelestialSmokeWorld/native.py."""
from __future__ import annotations
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
import xml.etree.ElementTree as ET

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

def sql(query):
    common = dict(capture_output=True, text=True, creationflags=subprocess.CREATE_NO_WINDOW, timeout=30)
    identity = subprocess.run(instance['client'] + ['-e', 'SELECT @@datadir'], **common)
    assert identity.returncode == 0 and pathlib.Path(identity.stdout.strip()).resolve() == pathlib.Path(instance['data']).resolve()
    result = subprocess.run(instance['client'] + [instance['database'], '--raw', '-e', query], **common)
    assert result.returncode == 0, result.stderr
    return result.stdout.strip()

connection = f"server=127.0.0.1;port={instance['port']};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')).group(1)
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
tag = uuid.uuid4().hex[:8]
runtime = root / ('predator-' + tag)
runtime.mkdir()
report = {'status': 'RUNNING', 'run': tag, 'assertions': [], 'objects': {}, 'replay': 'native-replay.json'}
process = None
session = None
transcripts = []
deadline = time.monotonic() + 850
os.environ['FUTUREMUD_ENVIRONMENT'] = 'test'

def check(label, condition, detail=''):
    report['assertions'].append(dict(name=label, passed=bool(condition), detail=detail))
    assert condition, label + ': ' + detail

def command(text, expected=None, seconds=.25):
    assert time.monotonic() < deadline, 'Scenario deadline exceeded'
    answer = session.send(text, read_seconds=seconds)
    limit = time.monotonic() + 6
    while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < limit:
        answer += session.read_for(.25)
    if expected:
        check(text, re.search(expected, answer, re.I | re.S) is not None, answer[-1600:])
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
    session.send(password, secret=True, read_seconds=.5)
    command('c')
    command('1', 'Guest Lounge', seconds=2)
    command('impdebug freezetime', 'freeze all')

def stop():
    global session, process
    if session:
        command('impdebug flush', seconds=1)
        command('shutdown stop', seconds=2)
        process.wait(timeout=30)
        check('graceful server shutdown', process.returncode == 0, str(process.returncode))
        transcripts.append(session.full_transcript())
        session.close()
        session = None
    if process:
        helper.stop_process_tree(process)
        process = None

try:
    names = ['Leopard', 'Panther', 'Jaguar', 'Tiger', 'Crocodile', 'Alligator', 'Caiman', 'Bunyip', 'Yacumama']
    for name in names:
        count = int(sql("SELECT COUNT(*) FROM Races_WeaponAttacks rw JOIN Races r ON r.Id=rw.RaceId JOIN WeaponAttacks w ON w.Id=rw.WeaponAttackId WHERE r.Name='" + name + "' AND w.Name LIKE 'Wildlife - Ambush %'"))
        check(name + ' has linked ambush anatomy', count > 0)
    for name in ['Giant Spider', 'Giant Scorpion', 'Giant Centipede']:
        attacks = sql("SELECT DISTINCT w.Name FROM Races_WeaponAttacks rw JOIN Races r ON r.Id=rw.RaceId JOIN WeaponAttacks w ON w.Id=rw.WeaponAttackId JOIN BodypartProto b ON b.Id=rw.BodypartId WHERE r.Name='" + name + "' AND w.Name LIKE 'Wildlife - %Venom' AND b.BodypartShapeId=w.BodypartShapeId")
        check(name + ' has matching melee and clinch venom anatomy', len(attacks.splitlines()) == 2, attacks)
    start()
    source = int(sql("SELECT Id FROM ArtificialIntelligences WHERE Name='Wildlife - Tree Ambush Hunter'"))
    clone_name = 'QAHunt' + tag
    command(f'ai clone {source} {clone_name}', clone_name)
    clone = int(sql("SELECT Id FROM ArtificialIntelligences WHERE Name='" + clone_name + "'"))
    command(f'ai edit {clone}', clone_name)
    for setting in ['hunting on', 'hunting opening Ambush', 'hunting followup Extract', 'hunting layer InTrees',
                    'prey people Never', 'prey selection Safest', 'assessment balanced', 'assessment weight weapons -17',
                    'hunting timeout 240', 'hunting lost 45']:
        command('ai set ' + setting, 'Hunting:')
    show = command('ai show', 'Ready:.*True')
    check('builder reports independent hunting settings', all(x in show for x in ['Ambush', 'Extract', 'Never', '-17']))
    attack = int(sql("SELECT Id FROM WeaponAttacks WHERE Name LIKE 'Wildlife - Ambush %' ORDER BY Id LIMIT 1"))
    command(f'weaponattack show {attack}', 'Ambush.*seize')
    report['objects']['ai'] = clone
    command('impdebug flush', seconds=1)
    saved = ET.fromstring(sql(f'SELECT Definition FROM ArtificialIntelligences WHERE Id={clone}')).find('Hunting')
    check('independent MySQL sees configured hunting XML', saved is not None and saved.findtext('People') == 'Never' and saved.findtext('TimeoutSeconds') == '240')
    stop()
    start()
    command(f'ai show {clone}', 'Never.*Ambush|Ambush.*Never')
    command(f'weaponattack show {attack}', 'Ambush.*seize')
    stop()
    report['status'] = 'PASS'
except BaseException as error:
    report['status'] = 'FAIL'
    report['error'] = helper.redact(str(error), [connection, password])
finally:
    if session:
        transcripts.append(session.full_transcript())
        session.close()
    if process:
        helper.stop_process_tree(process)
    (runtime / 'transcript.txt').write_text(helper.redact('\n'.join(transcripts), [connection, password]), encoding='utf-8')
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(report['status'] + ': ' + str(runtime / 'receipt.json'))
sys.exit(0 if report['status'] == 'PASS' else 1)
