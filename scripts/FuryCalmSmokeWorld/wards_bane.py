"""N19 paid casts, native wards/wounds and occupied refuge retirement in an owned loopback world."""
from __future__ import annotations
import argparse
import hashlib
import importlib.util
import json
import pathlib
import re
import socket
import subprocess
import sys
import time
import uuid
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True
spec = importlib.util.spec_from_file_location('n19_base', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.deadline = time.monotonic() + 1500
s.report.update(scope='bounded N19 builder stocks with authored ward vocabulary and creature predicate', milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/WardBaneFixtures.cs',
             *sorted((s.repo / 'MudSharpCore/Magic/Lifecycle').glob('SpellOwnedShelterService*.cs')),
             s.repo / 'MudSharpCore/Effects/Concrete/SpellShelterWard.cs',
             s.repo / 'MudSharpCore/Magic/ArmageddonBaneStock.cs',
             s.repo / 'MudSharpCore/bin/Debug/net10.0/FutureMUDLibrary.dll',
             s.repo / 'FutureMUDLibrary/Magic/SpellShelterWardConfiguration.cs',
             s.repo / 'FutureMUDLibrary/Magic/Stock/ArmageddonBaneContent.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
mysql = None
names = {}
ids = {}
fixture = None


def flush():
    s.command('impdebug flush', seconds=.4)


def start():
    title = s.sql('SELECT o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id=1')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    (s.runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    s.process = s.helper.launch_mud(s.repo, s.runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), s.connection, [s.connection, s.password])
    s.session = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password])
    s.session.read_for(.5)
    s.command('l'); s.command('Admin'); s.session.send(s.password, secret=True, read_seconds=.5)
    s.command('c'); s.command('1', re.escape(title), seconds=2)
    s.command('impdebug freezetime', 'freeze all')


def reserve():
    flush()
    return float(s.sql('SELECT Amount FROM Characters_MagicResources WHERE CharacterId=1 AND MagicResourceId=5'))


def cast(spell, target, cost, expected=None):
    before = reserve()
    s.command('mortal')
    answer = s.command(f'qamagi cast "{spell}" grade 1 on {target} via 5', expected, seconds=.8)
    s.command('immortal')
    after = reserve()
    s.check('exact payment for ' + spell, before - after == cost, f'{before}->{after}; {answer}')
    s.report.setdefault('paidCasts', []).append(dict(spell=spell, target=target, before=before, after=after, cost=cost, answer=answer))
    return answer


def wounds():
    flush()
    values = tuple(map(float, s.sql('SELECT IFNULL(SUM(CurrentDamage),0),IFNULL(SUM(CurrentPain),0),IFNULL(SUM(CurrentStun),0) FROM Wounds WHERE BodyId=(SELECT BodyId FROM Characters WHERE Id=3)').split('\t')))
    s.report.setdefault('nativeWoundSamples', []).append(dict(damage=values[0], pain=values[1], stun=values[2]))
    # Native DamageEffect counts real damage, pain or stun changes as an applied
    # wound. Armour/bodypart processing can convert Arcane damage to pain/stun.
    return sum(values)


def native_tag(tag):
    flush()
    effects = ET.fromstring(s.sql('SELECT EffectData FROM Characters WHERE Id=3'))
    return any(child.findtext('Type') == 'SpellMagicTag' and child.findtext('Effect/Tag') == tag
               for parent in effects.findall('Effect') if parent.findtext('Type') == 'MagicSpellParent'
               for child in parent.findall('Effect/Children/Effect'))


def current_life():
    flush()
    life = s.sql(f"SELECT Id FROM MagicSpellLifecycles WHERE SpellId={ids['refuge']} ORDER BY CreatedUtc DESC LIMIT 1")
    uuid.UUID(life)
    claims = dict((int(kind), int(entity)) for kind, entity in (x.split('\t') for x in s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{life}'").splitlines()))
    s.check('refuge claims only its room, entrance and overlay', set(claims) == {4, 5, 6}, str(claims))
    ward = ET.fromstring(s.sql(f'SELECT EffectData FROM Rooms WHERE Id={claims[4]}')).findall('Effect')
    s.check('new room already persists its one exact owned ward', len(ward) == 1 and ward[0].findtext('Type') == 'SpellShelterWard' and ward[0].findtext('Effect/Lifecycle') == life)
    s.report.setdefault('lifecycles', []).append(dict(id=life, claims=claims))
    return life, claims


def travel(command, room):
    title = s.sql(f'SELECT o.RoomName FROM Rooms r JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE r.Id={room}')
    answer = s.command(command, 'begin|enter|leave', seconds=.5)
    end = time.monotonic() + 65
    while title not in answer and time.monotonic() < end:
        answer += s.session.read_for(.5)
    s.check('native entrance movement arrives', title in answer, answer[-2000:])
    s.session.read_for(6)
    flush()
    s.check('native actor and primary frame share the arrived room', s.sql(f'SELECT c.Location,i.LocationId FROM Characters c JOIN CharacterInstances i ON i.CharacterId=c.Id AND i.IsPrimary=1 WHERE c.Id=1') == f'{room}\t{room}')


def complete(life, claims, item=None, maximum=100):
    end = time.monotonic() + maximum
    row = ''
    while time.monotonic() < end:
        row = s.sql(f"SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE Id='{life}'")
        if row.split('\t')[0] == '3': break
        s.session.read_for(.5)
    s.check('refuge lifecycle completes without a hold', row.split('\t')[0] == '3', row)
    s.report['lifecycles'][-1]['terminal'] = row
    for kind, table in [(4, 'Rooms'), (5, 'Exits'), (6, 'RoomOverlays')]:
        s.check('only owned ' + table + ' row removed', s.sql(f'SELECT COUNT(*) FROM {table} WHERE Id={claims[kind]}') == '0')
    for actor in [1, 3]:
        s.check('borrowed actor and body survive at safe anchor', s.sql(f"SELECT Location,BodyId IS NOT NULL,IsArchived+0 FROM Characters WHERE Id={actor}") == f"{fixture['Anchor']}\t1\t0")
        s.check('all borrowed physical instances are evacuated', s.sql(f"SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId={actor} AND LocationId<>{fixture['Anchor']}") == '0')
    if item:
        s.check('foreign goods survive with exact floor custody and no lifecycle ownership', s.sql(f"SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={item}),(SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}),(SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item})") == f"1\t{fixture['Anchor']}\t0")


def edit(spell, text, expected=None):
    s.command(f'magic spell edit {spell}', 'edit')
    s.command('magic spell set ' + text, expected)
    s.command('magic spell close')


def prepare_prior_attempts(foreign_spell):
    assert s.process is None
    # A failed hold test retains a genuine entrance. Shorten only this fixture's
    # exact parent schedule while stopped; native expiry performs the removal.
    prior = {}
    for receipt in s.root.glob('fury-calm-*/receipt.json'):
        record = json.loads(receipt.read_text())
        if 'N19' not in record.get('scope', ''): continue
        for life in record.get('lifecycles', []):
            uuid.UUID(life['id'])
            room = int(life['claims']['4'])
            prior[life['id']] = room
    for life, room in prior.items():
        row = s.sql(f"SELECT State FROM MagicSpellLifecycles WHERE Id='{life}'")
        if row == '3': continue
        s.check('prior attempt has its exact recorded room claim', s.sql(f"SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE LifecycleId='{life}' AND Kind=4 AND EntityId={room}") == '1')
        if s.sql(f'SELECT COUNT(*) FROM Rooms WHERE Id={room}') == '0': continue
        effects = ET.fromstring(s.sql(f'SELECT EffectData FROM Rooms WHERE Id={room}'))
        parents = [x for x in effects.findall('Effect') if x.findtext('Type') == 'MagicSpellParent' and x.findtext('Effect/Spell') == str(foreign_spell)]
        if parents:
            s.check('prior room has only the exact disposable foreign probe parent', len(parents) == 1)
            parents[0].find('Remaining').text = '1'
            encoded = ET.tostring(effects, encoding='unicode').replace("'", "''")
            s.sql(f"UPDATE Rooms SET EffectData='{encoded}' WHERE Id={room}")
        s.report.setdefault('priorAttemptRecovery', []).append(dict(lifecycle=life, room=room, probe_native_seconds=1 if parents else None))
    return prior


def await_prior_attempts(prior):
    for life in prior:
        end = time.monotonic() + 300
        while s.sql(f"SELECT State FROM MagicSpellLifecycles WHERE Id='{life}'") != '3' and time.monotonic() < end:
            s.session.read_for(.5)
        s.check('prior attempt retires through native recovery', s.sql(f"SELECT State FROM MagicSpellLifecycles WHERE Id='{life}'") == '3')


def run():
    global mysql, fixture
    try:
        binary = pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin')
        data = pathlib.Path(s.instance['data']).resolve()
        assert data == (s.root / 'mysql/data').resolve()
        with socket.socket() as probe:
            assert probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0, 'Owned port is already occupied'
        log = (s.runtime / 'mysql-process.log').open('w', encoding='utf-8')
        mysql = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent), '--datadir=' + str(data),
            '--port=' + str(s.instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
            '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M', '--log-error=' + str(s.root / 'mysql/mysql.err')],
            stdout=log, stderr=subprocess.STDOUT, creationflags=subprocess.CREATE_NO_WINDOW)
        end = time.monotonic() + 45
        while time.monotonic() < end:
            try: s.sql('SELECT 1'); break
            except AssertionError: time.sleep(.5)
        s.sql('SELECT 1')
        fixture = s.fixture('shelter-fixtures')
        extras = s.fixture('projection-fixtures')
        content = s.fixture('ward-bane-fixtures')
        prior = prepare_prior_attempts(content['Spells']['QA_N19_ForeignWard'])
        start()
        await_prior_attempts(prior)
        s.command(f"goto {fixture['Anchor']}", 'QA Shelter Clearing|already')
        s.command('transfer qatarget')
        suffix = uuid.uuid4().hex[:6]
        for key, tail, title in [
            ('severing-refuge', f"{fixture['Sand']} {fixture['Terrain']} {fixture['Fallback']} 240 8 tag qa-n19-fire Both", 'Severing Refuge'),
            ('apex-bane', f"{content['Eligibility']} 297 Impossible 8 40 Arcane", 'Apex Bane')]:
            s.command(f'magic spell edit new stock {key} 5 297 4 {tail}', 'Created.*' + title)
            identifier = int(s.sql(f"SELECT Id FROM MagicSpells WHERE Name='{title}' ORDER BY Id DESC LIMIT 1"))
            name = 'QA_N19_' + key.replace('-', '_') + '_' + suffix
            s.command(f'magic spell set name {name}', 'name|renamed|called')
            s.command('magic spell set difficulty automatic', 'Automatic to cast')
            if key == 'apex-bane':
                s.command('magic spell set targetresistemote $1 resists the N19 bane.', 'resist')
            s.command('magic spell close')
            ids['refuge' if key == 'severing-refuge' else 'bane'] = identifier
            names['refuge' if key == 'severing-refuge' else 'bane'] = name
        for key, identifier in content['Spells'].items(): ids[key] = identifier; names[key] = key
        for identifier in ids.values():
            s.command('magic capability edit 5', 'edit')
            s.command(f'magic capability set casting entry add {identifier}', 'updated|already')
            s.command('magic capability close')
            s.command(f'magic casting grant 1 5 {identifier} N19 native qualification', 'grant|acquir|already')
        s.stop()
        s.sql('INSERT INTO CharacterTraits (CharacterId,TraitDefinitionId,Value,AdditionalValue) SELECT 1,TraitDefinitionId,Value,AdditionalValue FROM CharacterTraits WHERE CharacterId=2 ON DUPLICATE KEY UPDATE Value=VALUES(Value),AdditionalValue=VALUES(AdditionalValue)')
        s.sql('UPDATE CharacterTraits SET Value=90 WHERE CharacterId=1 AND TraitDefinitionId=297')
        s.sql('INSERT INTO CharacterTraits (CharacterId,TraitDefinitionId,Value,AdditionalValue) VALUES (3,297,90,0) ON DUPLICATE KEY UPDATE Value=90')
        s.sql('INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES (1,5,1000) ON DUPLICATE KEY UPDATE Amount=1000')
        s.sql('UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId=1 AND MagicSpellId IN (' + ','.join(map(str, ids.values())) + ')')
        s.report['objects'] = dict(player=1, recipient=3, spells=ids, fixtures=fixture, predicate=content['Eligibility'])
        start()
        listing = s.command('effect list qatarget', 'Effects for')
        match = re.search(r'^\s*(\d+)\).*?QA_N19_Mark', listing, re.M | re.I)
        if match:
            s.command('effect remove qatarget ' + match.group(1), 'remove the effect')
        s.check('ineligible recipient begins without the exact fixture category tag', not native_tag('qa-n19-creature'))
        before = wounds()
        cast(names['bane'], 'qatarget', 0, 'refused|cannot|not.*target|not suitable')
        s.check('ineligible creature has no native wound mutation', wounds() == before)
        cast('QA_N19_Mark', 'qatarget', 8, 'paid|succeed|cast')
        s.check('native tag delivery makes the recipient eligible', native_tag('qa-n19-creature'))
        cast(names['bane'], 'qatarget', 12, 'paid|succeed|cast')
        s.check('eligible resisted attack delivers actual native wounds', wounds() > before)
        # Explicit native resistance test. Automatic target check opposes a harder casting check.
        edit(ids['bane'], 'resist 297 Automatic', 'targets make')
        edit(ids['bane'], 'difficulty hard', 'Hard to cast')
        resisted = False
        for attempt in range(6):
            before = wounds()
            answer = cast(names['bane'], 'qatarget', 12, 'paid|cast|fail|resists')
            if 'resists the N19 bane' in answer:
                s.check('native resistance prevents additional wounds while preserving payment', wounds() == before)
                resisted = True; break
        s.check('native opposed resistance was actually exercised', resisted)
        edit(ids['bane'], 'resist 297 Impossible', 'targets make')
        edit(ids['bane'], 'difficulty automatic', 'Automatic to cast')
        cast(names['refuge'], 'here', 9, 'paid|cast|succeed')
        life, claims = current_life()
        travel('enter refuge', claims[4])
        s.command('transfer qatarget')
        before = wounds()
        cast('QA_N19_FireProbe', 'qatarget', 8, 'paid|ward|fail')
        s.check('selected tag ward prevents the actual native tag effect', not native_tag('qa-n19-fire'))
        cast('QA_N19_Mark', 'qatarget', 8, 'paid|cast|succeed')
        cast(names['bane'], 'qatarget', 12, 'paid|cast|succeed')
        s.check('authored tag ward allows an unselected eligible attack', wounds() > before)
        before_items = set(s.sql('SELECT Id FROM GameItems').splitlines())
        s.command(f"item load {extras['ForeignPrototype']}", 'load|create|hands')
        s.command('drop qan18foreign', 'drop'); flush()
        new_items = set(s.sql('SELECT Id FROM GameItems').splitlines()) - before_items
        s.check('exact foreign visitor item created', len(new_items) == 1)
        item = int(new_items.pop())
        cast('QA_N19_ForeignWard', 'here', 8, 'paid|cast|succeed')
        s.command('room delete', 'Are you sure'); s.command('accept delete', 'Deletion.*held for recovery'); flush()
        held = s.sql(f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{life}'")
        s.check('foreign room effect holds occupied topology before evacuation', held.startswith('1\t') and 'Foreign room hooks or effects' in held, held)
        s.check('held room still contains borrowed occupants', s.sql('SELECT Location FROM Characters WHERE Id=1') == str(claims[4]))
        effects = ET.fromstring(s.sql(f'SELECT EffectData FROM Rooms WHERE Id={claims[4]}'))
        s.check('held room retains the exact foreign native probe independently of its owned ward',
                any(x.findtext('Type') == 'MagicSpellParent' and x.findtext('Effect/Spell') == str(ids['QA_N19_ForeignWard']) for x in effects.findall('Effect')))
        complete(life, claims, item, maximum=160)
        # The next invocation snapshots a school ward; the earlier tag ward is immutable.
        edit(ids['refuge'], 'effect 1 ward school 5', 'ward configuration updated')
        edit(ids['refuge'], 'effect 1 ward coverage Incoming', 'ward configuration updated')
        edit(ids['refuge'], 'effect 1 lifetime 300', 'updated')
        cast(names['refuge'], 'here', 9, 'paid|cast|succeed')
        life, claims = current_life()
        travel('enter refuge', claims[4]); s.command('transfer qatarget')
        before = wounds()
        cast(names['bane'], 'qatarget', 12, 'paid|ward|fail')
        s.check('incoming school ward prevents eligible native damage', wounds() == before)
        paid = reserve()
        deadline = s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{life}'")
        ward_data = s.sql(f'SELECT HEX(EffectData) FROM Rooms WHERE Id={claims[4]}')
        count = s.sql(f"SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={ids['refuge']}")
        s.stop(); start()
        s.check('occupied cold restart preserves exact deadline, ward, topology count and paid reserve',
            s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{life}'") == deadline and
            s.sql(f'SELECT HEX(EffectData) FROM Rooms WHERE Id={claims[4]}') == ward_data and reserve() == paid and
            s.sql(f"SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={ids['refuge']}") == count)
        before = wounds(); cast(names['bane'], 'qatarget', 12, 'paid|ward|fail')
        s.check('cold-loaded ward remains operational', wounds() == before)
        travel('leave outside', fixture['Anchor']); travel('enter refuge', claims[4])
        complete(life, claims, maximum=360)
        s.check('occupied expiry retires through the original absolute deadline', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '0')
        s.stop()
        s.check('all captured native inputs remain unchanged', all(hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error:
        s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
    finally:
        try: s.finish()
        finally:
            if mysql:
                try:
                    s.sql('SHUTDOWN')
                except AssertionError:
                    pass
                s.helper.stop_process_tree(mysql)
                s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
                end = time.monotonic() + 30
                while time.monotonic() < end:
                    with socket.socket() as probe:
                        closed = probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0
                    if closed: break
                    time.sleep(.5)
                s.report['mysqlPortClosed'] = closed
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')


if __name__ == '__main__':
    run()
    raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
