"""Fury/Calm qualification in AuthoredCelestialSmokeWorld/native.py's owned world."""
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
connection = f"server=127.0.0.1;port={instance['port']};database=authored_celestial_smoke;uid=root;SslMode=None;AllowPublicKeyRetrieval=True;Default Command Timeout=600;"
password = re.search(r'const string DebugPassword\s*=\s*"([^"]+)"', (repo / 'DatabaseSeeder/DebugSeederReplay.cs').read_text(encoding='utf-8-sig')).group(1)
spec = importlib.util.spec_from_file_location('mud_session', repo / '.agents/skills/futuremud-mud-tester/scripts/mud_session.py')
helper = importlib.util.module_from_spec(spec)
spec.loader.exec_module(helper)
collectors = []
original_collector = helper.OutputCollector


class RetainedOutputCollector(original_collector):
    """Retain the skill helper's existing single stdout reader for the receipt."""
    def __init__(self, proc, secrets):
        super().__init__(proc, secrets)
        collectors.append(self)


helper.OutputCollector = RetainedOutputCollector
runtime = root / ('fury-calm-' + uuid.uuid4().hex[:8])
runtime.mkdir()
report = dict(status='RUNNING', scope='installer and first paid persistence slice', milestoneQualified=False,
              assertions=[], objects={}, replay='native-replay.json')
report['revision'] = subprocess.run(['git', 'rev-parse', 'HEAD'], cwd=repo, capture_output=True, text=True, check=True).stdout.strip()
report['inputs'] = {str(path.relative_to(repo)): hashlib.sha256(path.read_bytes()).hexdigest() for path in [
    pathlib.Path(__file__), repo / 'scripts/FuryCalmSmokeWorld/Program.cs',
    repo / 'scripts/AuthoredCelestialSmokeWorld/native.py',
    repo / 'scripts/FuryCalmSmokeWorld/bin/Debug/net10.0/FuryCalmSmokeWorld.dll',
    repo / 'scripts/FuryCalmSmokeWorld/bin/Debug/net10.0/DatabaseSeeder.dll',
    repo / 'MudSharpCore/bin/Debug/net10.0/MudSharp.dll',
    repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.sql',
    repo / 'DatabaseSeeder/Assets/Database/BlankDatabaseSnapshot.manifest.json']}
process = session = None
transcripts = []
deadline = time.monotonic() + 900
os.environ['FUTUREMUD_ENVIRONMENT'] = 'test'


def sql(text):
    common = dict(capture_output=True, text=True, encoding='utf-8', creationflags=subprocess.CREATE_NO_WINDOW, timeout=60)
    identity = subprocess.run(instance['client'] + ['-e', 'SELECT @@datadir'], **common)
    assert identity.returncode == 0 and pathlib.Path(identity.stdout.strip()).resolve() == pathlib.Path(instance['data']).resolve()
    result = subprocess.run(instance['client'] + [instance['database'], '--raw', '-e', text], **common)
    assert result.returncode == 0, result.stderr
    return result.stdout.strip()


def check(label, condition, detail=''):
    report['assertions'].append(dict(name=label, passed=bool(condition), detail=detail))
    print(('PASS ' if condition else 'FAIL ') + label, flush=True)
    assert condition, label + ': ' + detail


def fixture(mode):
    assert process is None, 'Fixture mutations require a stopped MUD'
    env = os.environ.copy()
    env['FURY_CALM_SMOKE_CONNECTION'] = connection
    receipt = runtime / ('fixture-' + mode + '.json')
    result = subprocess.run(['dotnet', str(repo / 'scripts/FuryCalmSmokeWorld/bin/Debug/net10.0/FuryCalmSmokeWorld.dll'), instance['data'], str(receipt), mode],
                            cwd=repo, env=env, capture_output=True, text=True, encoding='utf-8', creationflags=subprocess.CREATE_NO_WINDOW,
                            timeout=600 if mode == 'failure-cases' else 240)
    (runtime / (mode + '.log')).write_text(helper.redact(result.stdout + result.stderr, [connection, password]), encoding='utf-8')
    check('native fixture ' + mode, result.returncode == 0, result.stderr[-2200:])
    return json.loads(receipt.read_text())


def command(text, expected=None, seconds=.25):
    assert time.monotonic() < deadline, 'Scenario deadline exceeded'
    answer = session.send(text, read_seconds=seconds)
    limit = time.monotonic() + 8
    while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < limit:
        answer += session.read_for(.25)
    if expected:
        check(text, re.search(expected, answer, re.I | re.S) is not None, answer[-2000:])
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
    global process, session
    command('impdebug flush', seconds=1)
    command('shutdown stop', seconds=2)
    process.wait(timeout=30)
    check('graceful server shutdown', process.returncode == 0, str(process.returncode))
    transcripts.append(session.full_transcript())
    session.close()
    session = None
    helper.stop_process_tree(process)
    process = None


def effect_data(character):
    command('impdebug flush', seconds=.6)
    return ET.fromstring(sql(f'SELECT EffectData FROM Characters WHERE Id={character}'))


def children(character, kind):
    return [x for x in effect_data(character).iter('Effect') if x.findtext('Type') == kind]


def resource(character):
    command('impdebug flush', seconds=.6)
    return float(sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={character} AND MagicResourceId={reserve}') or '0')


def possess(name):
    command('possess ' + name, 'take control')


def cast(spell, grade, target, expected=None):
    possess('qacaster')
    answer = command(f'qamagi cast "{spell}" grade {grade} on {target} via {capability}', expected, seconds=.5)
    command('return', seconds=.25)
    return answer


def remove_emotions(name):
    for _ in range(16):
        listing = command('effect list ' + name, 'Effects for')
        match = re.search(r'^\s*(\d+)\).*?(?:Roused Fury|Still Anger)', listing, re.I | re.M)
        if not match:
            return
        command(f'effect remove {name} {match.group(1)}', 'You remove the effect')
    raise AssertionError('Emotional fixture cleanup exceeded its bounded parent count')


def finish():
    global process, session
    if session:
        transcripts.append(session.full_transcript())
        session.close()
    if process:
        helper.stop_process_tree(process)
        report['mudStoppedByOwnedHandle'] = True
    report['serverProcesses'] = []
    for index, collector in enumerate(collectors):
        collector.thread.join(timeout=2)
        path = runtime / f'server-output-{index}.log'
        path.write_text('\n'.join(collector.lines), encoding='utf-8')
        report['serverProcesses'].append(dict(output=path.name, returncode=collector.proc.poll(),
                                            collectorStopped=not collector.thread.is_alive()))
    (runtime / 'transcript.txt').write_text(helper.redact('\n'.join(transcripts), [connection, password]), encoding='utf-8')
    (runtime / 'receipt.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    (root / 'smoke-latest.json').write_text(json.dumps(report, indent=2), encoding='utf-8')
    print(report['status'] + ': ' + str(runtime / 'receipt.json'))

def run_initial():
    global reserve, source, capability, fury_id, calm_id, fury_name, calm_name, caster, target, other, enemy
    report['qualificationMarker'] = 'fury-calm-slice-passed.json'
    try:
        install = fixture('rerun' if (root / 'fury-calm-bindings.json').exists() else 'provision')
        bindings = json.loads((root / 'fury-calm-bindings.json').read_text())
        reserve = bindings['ReserveResource']
        source = bindings['Utilities']['Resource']
        capability = next(x['Id'] for x in install['Capabilities'] if x['Name'].endswith('sorcerer'))
        spell_ids = {x['StableKey']: x['LogicalId'] for x in install['Spells']}
        fury_id = spell_ids['arm.spell.roused_fury']
        calm_id = spell_ids['arm.spell.still_anger']
        fury_name = sql(f'SELECT Name FROM MagicSpells WHERE Id={fury_id}')
        calm_name = sql(f'SELECT Name FROM MagicSpells WHERE Id={calm_id}')
        start()
        human = int(sql("SELECT Id FROM Races WHERE Name='Human'"))
        culture = int(sql('SELECT Id FROM Cultures ORDER BY Id LIMIT 1'))
        ethnicity = int(sql(f'SELECT Id FROM Ethnicities WHERE ParentRaceId={human} ORDER BY Id LIMIT 1'))
        template_ids = sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='Alaric Stone' ORDER BY Id").splitlines()
        check('unique owned caster template', len(template_ids) <= 1)
        if not template_ids:
            command('npc edit new simple', 'NPC|template')
            for setting, expected in [('race ' + str(human), 'race'), ('culture ' + str(culture), 'culture'), ('ethnicity ' + str(ethnicity), 'ethnicity'),
                                      ('gender male', 'gender'), ('birthday random 30', 'born'), ('name Alaric Stone', "name"),
                                      ('height 180cm', 'height'), ('weight 80kg', 'weight'), ('sdesc a qacaster human', 'description')]:
                command('npc set ' + setting, expected)
            command('npc set desc')
            command('An ordinary human in the disposable emotional spell qualification world.')
            command('@', seconds=.5)
            command('npc edit submit', 'submit')
            command('impdebug flush', seconds=.6)
            template_ids = sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='Alaric Stone'").splitlines()
            template = int(template_ids[0])
            command(f'npc review {template}', 'accept')
            command('accept edit', 'approv|accept')
        template = int(template_ids[0])
        if sql("SELECT COUNT(*) FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription='a qacaster human'") == '0':
            command(f'npc load {template}', 'load|human|appears')
        for name in ['qatarget', 'qaother', 'qaenemy']:
            if sql(f"SELECT COUNT(*) FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription='a {name} human'") == '1':
                continue
            clone_ids = sql(f"SELECT DISTINCT Id FROM NpcTemplates WHERE Name='{name}'").splitlines()
            check('unique owned ' + name + ' template', len(clone_ids) <= 1)
            if clone_ids:
                command(f'npc edit {clone_ids[0]}', 'edit')
            else:
                command(f'npc clone {template} "{name}"', 'clone|copy|template')
            command('npc set sdesc a ' + name + ' human', 'description')
            # Cloning an approved Simple template produces an approved native revision.
            command('impdebug flush', seconds=.6)
            clone = int(sql(f"SELECT DISTINCT Id FROM NpcTemplates WHERE Name='{name}'"))
            if clone_ids:
                command('npc edit submit', 'submit')
                command(f'npc review {clone}', 'accept')
                command('accept edit', 'approv|accept')
            if sql(f"SELECT COUNT(*) FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription='a {name} human'") == '0':
                command(f'npc load {clone}', 'load|human|appears')
        command('impdebug flush', seconds=1)
        actors = dict(line.split('\t') for line in sql("SELECT b.ShortDescription,c.Id FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription LIKE 'a qa% human'").splitlines())
        caster = int(actors['a qacaster human'])
        target = int(actors['a qatarget human'])
        other = int(actors['a qaother human'])
        enemy = int(actors['a qaenemy human'])
        report['objects'] = dict(caster=caster, target=target, other=other, enemy=enemy, fury=fury_id, calm=calm_id, capability=capability, reserve=reserve)
        (root / 'fury-calm-actors.json').write_text(json.dumps(report['objects'], indent=2), encoding='utf-8')
        remove_emotions('qatarget')
        for spell in [fury_id, calm_id]:
            command(f'magic spell edit {spell}', 'edit')
            command('magic spell set difficulty automatic', 'Automatic to cast')
            command('magic spell close')
        merit = int(sql("SELECT Id FROM Merits WHERE Name='Armageddon partial sorcerer caster'"))
        command(f'givemerit qacaster {merit}', 'granted|already')
        balance_before_enrolment = resource(caster)
        command(f'magic casting enrol {caster} {capability} Native disposable qualification', 'enrol|already')
        for spell in [fury_id, calm_id]:
            command(f'magic casting grant {caster} {capability} {spell} Native disposable qualification', 'grant|acquir|already')
            skill = int(sql(f'SELECT CastingTraitDefinitionId FROM MagicSpells WHERE Id={spell}'))
            command(f'skill level qacaster {skill} 90', 'skill|level|value')
        check('enrolment preserves existing reserve or starts empty', resource(caster) == balance_before_enrolment)
        stop()
        # Explicit fixture balance and mastered grades, scoped to this disposable NPC only.
        sql(f'INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES ({caster},{reserve},1000) ON DUPLICATE KEY UPDATE Amount=1000')
        sql(f'UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId={caster} AND MagicSpellId IN ({fury_id},{calm_id})')
        fixture('rerun')
        start()
        initial = resource(caster)
        refusal = cast(fury_name, 1, 'here')
        check('unsupported target refuses without payment', resource(caster) == initial and not children(target, 'SpellSourceFury'), refusal)
        health_before = command('health qatarget', 'Stamina:')
        cast(fury_name, 1, 'qatarget', 'paid|cast')
        fury = children(target, 'SpellSourceFury')
        check('paid Fury installed on actual native body', len(fury) == 1, ET.tostring(effect_data(target), encoding='unicode'))
        paid = resource(caster)
        check('Fury deducts exactly its grade-one minimum energy', initial - paid == 20, str(initial - paid))
        health_after = command('health qatarget', 'Stamina:')
        before_current, before_max = map(float, re.search(r'Stamina:\s*([\d,.]+)\s*/\s*([\d,.]+)', health_before).groups())
        after_current, after_max = map(float, re.search(r'Stamina:\s*([\d,.]+)\s*/\s*([\d,.]+)', health_after).groups())
        check('Fury increases stamina capacity without refilling current stamina', after_max > before_max and after_current <= before_current + 1, health_before + health_after)
        saved_before = effect_data(target)
        stop()
        offline_started = time.monotonic()
        restore = fixture('restore')
        start()
        reloaded = children(target, 'SpellSourceFury')
        check('cold restart after restore retains exact Fury source state', len(reloaded) == 1 and ET.tostring(reloaded[0]) == ET.tostring(fury[0]))
        restart_health = command('health qatarget', 'Stamina:')
        restart_max = float(re.search(r'Stamina:\s*[\d,.]+\s*/\s*([\d,.]+)', restart_health).group(1))
        check('restarted native body has active Fury capacity bonus', restart_max == after_max, restart_health)
        restored_parent = next(x for x in effect_data(target).findall('Effect') if x.findtext('Type') == 'MagicSpellParent')
        previous_parent = next(x for x in saved_before.findall('Effect') if x.findtext('Type') == 'MagicSpellParent')
        restored_remaining = int(restored_parent.findtext('Remaining'))
        previous_remaining = int(previous_parent.findtext('Remaining'))
        offline_seconds = time.monotonic() - offline_started
        check('scheduled Fury lifetime pauses across offline restore and restart', 0 < previous_remaining - restored_remaining < offline_seconds * 1000,
              json.dumps(dict(beforeMs=previous_remaining, afterMs=restored_remaining, offlineSeconds=offline_seconds)))
        check('cold restart retains deducted reserve', resource(caster) == paid)
        cast(calm_name, 1, 'qatarget', 'paid|cast')
        check('equal reciprocal Calm counter removes Fury without installing Calm', not children(target, 'SpellSourceFury') and not children(target, 'SpellSourceCalm'))
        cast(calm_name, 1, 'qatarget', 'paid|cast')
        check('paid Calm installs real native child', len(children(target, 'SpellSourceCalm')) == 1)
        stop()
        report['status'] = 'PASS'
    except BaseException as error:
        report['status'] = 'FAIL'
        report['error'] = helper.redact(str(error), [connection, password])
    finally:
        finish()


if __name__ == '__main__':
    run_initial()
    raise SystemExit(0 if report['status'] == 'PASS' else 1)
