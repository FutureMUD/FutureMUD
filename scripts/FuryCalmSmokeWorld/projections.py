"""Bounded N18 qualification using an actual PC and the tester's owned disposable world."""
from __future__ import annotations

import argparse
import hashlib
import importlib.util
import json
import pathlib
import socket
import subprocess
import sys
import time
import uuid

sys.dont_write_bytecode = True

spec = importlib.util.spec_from_file_location('owned_projection_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report.update(scope='bounded N18 builder-installed native identity projections', milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/OwnedProjectionFixtures.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
s.deadline = time.monotonic() + 1500
mysql = None
fixture = None


def start():
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0))
        port = listener.getsockname()[1]
    (s.runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    s.process = s.helper.launch_mud(s.repo, s.runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), s.connection, [s.connection, s.password])
    s.session = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password])
    s.session.read_for(.5)
    s.command('l'); s.command('Admin')
    s.session.send(s.password, secret=True, read_seconds=.5)
    s.command('c'); s.command('1', 'Guest Lounge|QA N18 Room', seconds=2)
    s.command('impdebug freezetime', 'freeze all')


def flush():
    s.command('impdebug flush', seconds=.4)


def reserve():
    flush()
    return float(s.sql('SELECT Amount FROM Characters_MagicResources WHERE CharacterId=1 AND MagicResourceId=5') or '0')


def current_life():
    flush()
    row = s.sql("SELECT Id,State FROM MagicSpellLifecycles WHERE CreatorId=1 AND Family='native-identity-projection-v1' ORDER BY CreatedUtc DESC LIMIT 1")
    identifier, state = row.split('\t')
    claims = dict((int(kind), int(entity)) for kind, entity in (line.split('\t') for line in s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}'").splitlines()))
    return identifier, claims


def collapsed(identifier, claims, maximum=90):
    end = time.monotonic() + maximum
    row = ''
    while time.monotonic() < end:
        row = s.sql(f"SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE Id='{identifier}'")
        if row.split('\t')[0] == '3':
            break
        s.session.read_for(.5)
    s.check('projection lifecycle completes without a hold', row.split('\t')[0] == '3', row)
    s.report.setdefault('lifecycles', []).append(dict(id=identifier, claims=claims, retirement=row))
    # Entity kinds are the existing typed journal enum, not inferred numeric text matches.
    for kind, table in [(3, 'Bodies'), (2, 'CharacterInstances'), (0, 'GameItems')]:
        if kind in claims:
            s.check('owned ' + table + ' row removed', s.sql(f'SELECT COUNT(*) FROM {table} WHERE Id={claims[kind]}') == '0')
    answer = s.command('focus', 'currently focused')
    s.check('focus returned to primary', 'primary' in answer.lower(), answer)
    s.check('canonical player and primary body retained', s.sql('SELECT COUNT(*) FROM Characters c JOIN Bodies b ON b.Id=c.BodyId JOIN CharacterInstances i ON i.CharacterId=c.Id AND i.IsPrimary=1 AND i.BodyId=b.Id WHERE c.Id=1') == '1')


def cast(spell, energy, expected=True):
    before = reserve()
    count = s.sql("SELECT COUNT(*) FROM MagicSpellLifecycles WHERE CreatorId=1 AND Family='native-identity-projection-v1' AND State<>3")
    s.command('mortal')
    answer = s.command(f'qamagi cast "{spell}" grade 1 on me via 5', 'takes shape|refused|must finish|requires|cannot|must be|not admitted', seconds=.5)
    s.command('immortal')
    after = reserve()
    s.check('paid cast deducts exact authored energy' if expected else 'refused cast does not deduct energy', before - after == (energy if expected else 0), f'{before}->{after}; {answer}')
    if not expected:
        s.check('duplicate refusal creates no additional lifecycle', s.sql("SELECT COUNT(*) FROM MagicSpellLifecycles WHERE CreatorId=1 AND Family='native-identity-projection-v1' AND State<>3") == count)
    return answer


def travel(direction, room, instance):
    answer = s.command(direction, 'begin.*away', seconds=1)
    end = time.monotonic() + 65
    while f'QA N18 Room {room}' not in answer and time.monotonic() < end:
        answer += s.session.read_for(.5)
    s.check('native exit movement arrives in the authored room', f'QA N18 Room {room}' in answer, answer[-2500:])
    # Complete the normal post-arrival movement phase before issuing the next move.
    s.session.read_for(6)
    flush()
    s.check('exact secondary location matches native arrival', s.sql(f'SELECT LocationId FROM CharacterInstances WHERE Id={instance}') == str(fixture['Rooms'][room]))
    s.check('primary anchor remains in its original room', s.sql('SELECT LocationId FROM CharacterInstances WHERE CharacterId=1 AND IsPrimary=1') == str(fixture['Rooms'][0]))


def policy(spell, lifetime):
    s.command('focus primary')
    s.command(f'magic spell edit {spell}', 'edit')
    s.command(f'magic spell set effect 1 lifetime {lifetime}', 'policy updated')
    s.command('magic spell close')


def run():
    global mysql, fixture
    try:
        try:
            s.sql('SELECT 1')
        except AssertionError:
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
                try:
                    s.sql('SELECT 1'); break
                except AssertionError:
                    time.sleep(.5)
            s.sql('SELECT 1')
        fixture = s.fixture('projection-fixtures')
        start()
        s.command(f"goto {fixture['Rooms'][0]}", 'QA N18 Room 0|already there')
        merit = int(s.sql("SELECT Id FROM Merits WHERE Name='Armageddon partial sorcerer caster'"))
        s.command(f'givemerit me {merit}', 'granted|already')
        s.command('magic casting enrol 1 5 N18 real PC qualification', 'enrol|already')
        names = {}
        ids = {}
        suffix = uuid.uuid4().hex[:6]
        for key, plane, tail, title in [
            ('sand-effigy', fixture['MaterialPlane'], f"{fixture['EffigyPrototype']} 120 1 10", 'Sand Effigy'),
            ('walking-shadow', fixture['ShadowPlane'], '600 1 1 10 false', 'Walking Shadow')]:
            s.command(f'magic spell edit new stock {key} 5 297 4 {plane} {tail}', 'Created.*' + title)
            spell = int(s.sql(f"SELECT Id FROM MagicSpells WHERE Name='{title}' ORDER BY Id DESC LIMIT 1"))
            name = 'QA_N18_' + key.replace('-', '_') + '_' + suffix
            s.command(f'magic spell set name {name}', 'name|called|renamed')
            s.command('magic spell set difficulty automatic', 'Automatic to cast')
            s.command('magic spell close')
            s.command('magic capability edit 5', 'edit')
            s.command(f'magic capability set casting entry add {spell}', 'updated|already')
            s.command('magic capability close')
            s.command(f'magic casting grant 1 5 {spell} N18 owned PC qualification', 'grant|acquir|already')
            ids[key] = spell; names[key] = name
        s.command('skill level me 297 90', 'skill|level|value')
        s.stop()
        # Explicit disposable fixture, with the server stopped: capacity, reserve and mastered grade.
        s.sql('INSERT INTO CharacterTraits (CharacterId,TraitDefinitionId,Value,AdditionalValue) SELECT 1,TraitDefinitionId,Value,AdditionalValue FROM CharacterTraits WHERE CharacterId=2 ON DUPLICATE KEY UPDATE Value=VALUES(Value),AdditionalValue=VALUES(AdditionalValue)')
        s.sql('UPDATE CharacterTraits SET Value=90 WHERE CharacterId=1 AND TraitDefinitionId=297')
        s.sql('INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES (1,5,1000) ON DUPLICATE KEY UPDATE Amount=1000')
        s.sql(f"UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId=1 AND MagicSpellId IN ({ids['sand-effigy']},{ids['walking-shadow']})")
        baseline = s.sql('SELECT TraitDefinitionId,Value,AdditionalValue FROM CharacterTraits WHERE CharacterId=1 ORDER BY TraitDefinitionId')
        s.report['objects'] = dict(player=1, spells=ids, fixtures=fixture)
        start()
        sand = ids['sand-effigy']; shadow = ids['walking-shadow']
        cast(names['sand-effigy'], 10)
        life, claims = current_life()
        cast(names['sand-effigy'], 0, False)
        s.command('instances', 'Focusable')
        s.command('focus 2', 'focus|effigy', seconds=.5)
        s.command('north', 'cannot move from its anchor')
        s.command('get qasandeffigy', 'cannot|projected|not able|do not see|not physically present')
        s.command('focus primary', 'focus', seconds=.5)
        s.command('purge qasandeffigy', 'purge|delete|destroy|collapse', seconds=1)
        collapsed(life, claims)
        s.check('exact effigy destruction caused anchor severance', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '7')
        # Native damage to the secondary body, with no artificial effect implementation.
        cast(names['sand-effigy'], 10)
        life, claims = current_life()
        s.command('wound effigy random cellular 1', seconds=1)
        collapsed(life, claims)
        s.check('native secondary-body damage caused collapse', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '8')
        cast(names['sand-effigy'], 10)
        life, claims = current_life()
        s.command('kill effigy', seconds=1)
        collapsed(life, claims)
        s.check('native lethal projection collapse preserves the primary', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '8')
        # A real foreign native effect holds all rows until explicitly detached.
        cast(names['sand-effigy'], 10)
        life, claims = current_life()
        s.command('focus 2', 'focus|effigy')
        s.command('immortal')
        s.command('focus primary', 'focus')
        s.command('purge qasandeffigy', seconds=1)
        held = s.sql(f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{life}'")
        s.check('foreign projection effect holds retirement before row deletion', held.startswith('1\t') and 'Foreign projection effects' in held, held)
        for kind, table in [(3, 'Bodies'), (2, 'CharacterInstances'), (0, 'GameItems')]:
            s.check('held owned ' + table + ' row is retained', s.sql(f'SELECT COUNT(*) FROM {table} WHERE Id={claims[kind]}') == '1')
        listing = s.command('effect list effigy', 'Effects for')
        match = s.re.search(r'^\s*(\d+)\).*?Admin Sight', listing, s.re.I | s.re.M)
        s.check('native effect listing identifies the introduced foreign effect', match is not None, listing)
        s.command('effect remove effigy ' + match.group(1), 'You remove the effect')
        collapsed(life, claims)
        # Real scheduled expiry while controlling the immobile instance.
        policy(sand, 30)
        cast(names['sand-effigy'], 10)
        life, claims = current_life()
        s.command('focus 2', 'focus|effigy')
        collapsed(life, claims)
        policy(sand, 120)
        cast(names['walking-shadow'], 10)
        life, claims = current_life()
        s.command('focus 2', 'focus|shadow')
        travel('north', 1, claims[2])
        s.command('north', 'beyond.*anchor range')
        s.command('immortal')
        s.command('goto ' + str(fixture['Rooms'][2]), 'only through native exits|beyond|cannot')
        s.command('mortal')
        travel('south', 0, claims[2])
        s.command('get qasandeffigy', 'cannot|projected|not able|do not see|not physically present')
        s.check('projection shares canonical skill rows', s.sql('SELECT TraitDefinitionId,Value,AdditionalValue FROM CharacterTraits WHERE CharacterId=1 ORDER BY TraitDefinitionId') == baseline)
        paid = reserve()
        s.command('focus primary', 'focus')
        before = {int(x) for x in s.sql('SELECT Id FROM GameItems').splitlines()}
        s.command('item load ' + str(fixture['ForeignPrototype']), 'load|created', seconds=1)
        flush()
        created = {int(x) for x in s.sql('SELECT Id FROM GameItems').splitlines()} - before
        s.check('native ordinary foreign item was created once', len(created) == 1)
        foreign = created.pop()
        s.command('focus 2', 'focus|shadow')
        # Graceful shutdown while focused, then cold reconciliation of DespawnOnReboot ownership.
        s.stop()
        s.check('live shadow graph persists for actual cold recovery', s.sql(f'SELECT COUNT(*) FROM CharacterInstances WHERE Id={claims[2]}') == '1')
        s.check('foreign inventory fixture has no spell ownership claim', s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={foreign}') == '0')
        # Explicit custody fixture using the native Bodies_GameItems relation, with
        # the owned MUD stopped. This exercises recovery of externally introduced goods.
        s.sql(f'START TRANSACTION; DELETE FROM Bodies_GameItems WHERE GameItemId={foreign}; DELETE FROM Rooms_GameItems WHERE GameItemId={foreign}; INSERT INTO Bodies_GameItems (BodyId,GameItemId,EquippedOrder,WearProfile,Wielded) VALUES ({claims[3]},{foreign},0,NULL,NULL); COMMIT;')
        start()
        collapsed(life, claims)
        s.check('foreign goods survive cold projection retirement', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={foreign}') == '1')
        s.check('foreign goods return to the recorded native anchor room', s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={foreign}') == str(fixture['Rooms'][0]))
        s.check('foreign goods leave projection body custody exactly once', s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={foreign}') == '0')
        s.report['foreignCustody'] = dict(item=foreign, projectionBody=claims[3], destination=fixture['Rooms'][0])
        s.check('restart preserves paid reserve', reserve() == paid)
        s.check('cold recovery uses the reboot retirement policy', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '9')
        cast(names['walking-shadow'], 10)
        life, claims = current_life()
        paid = reserve()
        s.command('focus 2', 'focus|shadow')
        s.command('quit', r'C\) Connect to a character', seconds=2)
        s.command('c'); s.command('1', 'QA N18 Room', seconds=2)
        collapsed(life, claims)
        s.check('native logout retires the projection', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '4')
        s.check('logout preserves paid reserve', reserve() == paid)
        # Damage to the preserved primary body severs the controlled shadow safely.
        cast(names['walking-shadow'], 10)
        life, claims = current_life()
        s.command('focus 2', 'focus|shadow')
        s.command('focus primary', 'focus')
        s.command('wound me random cellular 1', seconds=1)
        collapsed(life, claims)
        s.check('native primary-body damage caused collapse', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life}'") == '8')
        policy(shadow, 30)
        cast(names['walking-shadow'], 10)
        life, claims = current_life()
        s.command('focus 2', 'focus|shadow')
        collapsed(life, claims)
        s.check('canonical skills preserved after both lifecycle variants', s.sql('SELECT TraitDefinitionId,Value,AdditionalValue FROM CharacterTraits WHERE CharacterId=1 ORDER BY TraitDefinitionId') == baseline)
        s.stop()
        s.check('all captured qualification inputs remain unchanged', all(hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error:
        s.report['status'] = 'FAIL'; s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
    finally:
        s.finish()
        if mysql:
            s.helper.stop_process_tree(mysql)
            s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')


if __name__ == '__main__':
    run()
    raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
