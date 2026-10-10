"""N20 native finite pockets: payment, custody, cold reload and full collapse in the owned loopback world."""
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
spec = importlib.util.spec_from_file_location('n20_base', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.deadline = time.monotonic() + 1500
s.report.update(scope='bounded N20 builder-installed finite pocket native custody', milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/PocketFixtures.cs',
             s.repo / 'MudSharpCore/bin/Debug/net10.0/FutureMUDLibrary.dll',
             *sorted((s.repo / 'MudSharpCore/Magic/Lifecycle').glob('SpellOwnedPocket*.cs')),
             s.repo / 'MudSharpCore/GameItems/Components/FoldedPocketGameItemComponent.cs',
             s.repo / 'MudSharpCore/GameItems/SpellPocketContainment.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
mysql = None
fixture = places = None
spell = name = None
goods = []
original_command = s.command


def command(text, expected=None, seconds=.25):
    if fixture:
        for key, keyword in fixture['Keywords'].items(): text = text.replace('qan20' + key, keyword)
    expected = {'get': r'\bYou (get|pick up|take)\b', 'put': r'\bYou put\b', 'drop': r'\bYou drop\b',
                'give': r'\bYou give\b', 'junk': r'\bYou junk\b'}.get(expected, expected)
    return original_command(text, expected, seconds)


s.command = command


def flush():
    s.command('impdebug flush', seconds=.4)


def start():
    title = s.sql('SELECT o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id=1')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0)); port = listener.getsockname()[1]
    (s.runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    s.process = s.helper.launch_mud(s.repo, s.runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), s.connection, [s.connection, s.password])
    s.session = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password])
    s.session.read_for(.5); s.command('l'); s.command('Admin'); s.session.send(s.password, secret=True, read_seconds=.5)
    s.command('c'); s.command('1', re.escape(title), seconds=2); s.command('impdebug freezetime', 'freeze all')


def reserve():
    flush()
    return float(s.sql('SELECT Amount FROM Characters_MagicResources WHERE CharacterId=1 AND MagicResourceId=5'))


def branching_progress(character):
    # Read the concrete IncreasedBranchChance save fields, preserving canonical learning metadata.
    effects = ET.fromstring(s.sql(f'SELECT EffectData FROM Characters WHERE Id={character}'))
    rows = [x for x in effects.findall('Effect') if x.findtext('Type') == 'IncreasedBranchChance']
    assert len(rows) <= 1
    if not rows: return dict(skills={}, knowledges={})
    definition = rows[0].find('Effect')
    return dict(skills={x.findtext('SkillId'): int(x.findtext('Attempts')) for x in definition.findall('Skills/Skill')},
                knowledges={x.findtext('KnowledgeId'): float(x.findtext('Lessons')) for x in definition.findall('Knowledges/Knowledge')})


def cast(target, cost=12):
    before = reserve(); s.command('mortal')
    answer = s.command(f'qamagi cast "{name}" grade 1 on {target} via 5', seconds=.8)
    s.command('immortal'); after = reserve()
    s.check('exact native pocket payment', before - after == cost, f'{before}->{after}; {answer}')
    s.report.setdefault('paidCasts', []).append(dict(target=target, before=before, after=after, cost=cost, answer=answer))


def edit(text):
    s.command(f'magic spell edit {spell}', 'edit'); s.command('magic spell set effect 1 ' + text, 'updated'); s.command('magic spell close')


def creation(role='pocket', expected_capacity=None):
    cast('qan20focus'); flush()
    life = s.sql(f'SELECT Id FROM MagicSpellLifecycles WHERE SpellId={spell} ORDER BY CreatedUtc DESC LIMIT 1'); uuid.UUID(life)
    row = s.sql(f"SELECT Kind,Role,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{life}'").split('\t')
    s.check('pocket claims exactly its new carrier', len(row) == 3 and row[:2] == ['0', '0'], str(row))
    carrier = int(row[2]); deadline = s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{life}'")
    anchor = ET.fromstring(s.sql(f"SELECT Provenance FROM MagicSpellLifecycles WHERE Id='{life}'"))
    s.check('binding retains exact borrowed focus and authored capacity', int(anchor.findtext('SourceItem')) == s.report['objects']['focus'] and
            float(anchor.findtext('Pocket/CapacityPerGrade')) == (fixture['Capacity'] if expected_capacity is None else expected_capacity))
    s.report.setdefault('lifecycles', []).append(dict(id=life, carrier=carrier, role=role, deadline=deadline, binding=ET.tostring(anchor, encoding='unicode')))
    bodies = s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={carrier}')
    if not bodies: s.command('get qan20' + role, 'get', seconds=.5)
    flush(); s.check('new carrier enters exact ordinary hand custody', s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={carrier}') == s.sql('SELECT BodyId FROM Characters WHERE Id=1'))
    return life, carrier


def load(key):
    proto = fixture['Prototypes'][key]
    s.command(f'item load {proto}', 'load|create', seconds=.5); flush()
    item = int(s.sql(f'SELECT Id FROM GameItems WHERE GameItemProtoId={proto} ORDER BY Id DESC LIMIT 1'))
    if key != 'focus':
        goods.append(item)
        s.report.setdefault('foreignItems', []).append(dict(id=item, role=key, prototype=proto,
            original=s.sql(f'SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,\'\') FROM GameItems WHERE Id={item}')))
    return item


def conserved(item, parent=None, room=None):
    flush()
    row = s.sql(f'SELECT COUNT(*),IFNULL(MAX(ContainerId),0) FROM GameItems WHERE Id={item}')
    s.check('foreign identity and parent conserved', row == f'1\t{parent or 0}', row)
    floors = s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}')
    s.check('foreign item has exact floor custody', floors == (str(room) if room else ''), floors)
    s.check('foreign goods never acquire pocket ownership', s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item}') == '0')
    original = next(x for x in s.report['foreignItems'] if x['id'] == item)['original']
    s.check('foreign prototype quality and legal owner survive', s.sql(f'SELECT GameItemProtoId,GameItemProtoRevision,Quality,IFNULL(OwnerId,0),IFNULL(OwnerType,\'\') FROM GameItems WHERE Id={item}') == original)


def complete(life, carrier, maximum=120):
    end = time.monotonic() + maximum
    while time.monotonic() < end:
        row = s.sql(f"SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE Id='{life}'")
        if row.split('\t')[0] == '3': break
        s.session.read_for(.5)
    s.check('carrier retirement completes', row.split('\t')[0] == '3', row)
    s.check('only owned carrier removed', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={carrier}') == '0')
    keyword = fixture['Keywords'][next(x for x in s.report['lifecycles'] if x['id'] == life)['role']]
    inventory = s.command('inventory', seconds=.5)
    s.check('retired carrier leaves live player inventory', keyword.lower() not in inventory.lower(), inventory)
    next(x for x in s.report['lifecycles'] if x['id'] == life)['terminal'] = row


def run():
    global mysql, fixture, places, spell, name
    try:
        binary = pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'); data = pathlib.Path(s.instance['data']).resolve()
        assert data == (s.root / 'mysql/data').resolve()
        with socket.socket() as probe: assert probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0, 'Owned port is occupied'
        log = (s.runtime / 'mysql-process.log').open('w', encoding='utf-8')
        mysql = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent), '--datadir=' + str(data),
            '--port=' + str(s.instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
            '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M', '--log-error=' + str(s.root / 'mysql/mysql.err')],
            stdout=log, stderr=subprocess.STDOUT, creationflags=subprocess.CREATE_NO_WINDOW)
        end = time.monotonic() + 45
        while time.monotonic() < end:
            try: s.sql('SELECT 1'); break
            except AssertionError: time.sleep(.5)
        s.sql('SELECT 1'); s.fixture('projection-fixtures'); places = s.fixture('shelter-fixtures'); fixture = s.fixture('pocket-fixtures')
        start(); s.command(f"goto {places['Anchor']}", 'QA Shelter Clearing|already'); s.command('transfer qatarget')
        # Remove only the earlier qualification's known item-independent marker parents.
        for marker in ['QA_N19_Mark', 'QA_N19_FireProbe']:
            listing = s.command('effect list qatarget', 'Effects for')
            match = re.search(r'^\s*(\d+)\).*?' + re.escape(marker), listing, re.M | re.I)
            if match: s.command('effect remove qatarget ' + match.group(1), 'remove the effect')
        s.command(f"magic spell edit new stock folded-pocket 5 297 4 {fixture['Prototypes']['pocket']} \"2 kilograms\" Normal 600 Bearer {places['Fallback']}", 'Created.*Folded Pocket')
        spell = int(s.sql("SELECT Id FROM MagicSpells WHERE Name='Folded Pocket' ORDER BY Id DESC LIMIT 1")); name = 'QA_N20_Pocket_' + uuid.uuid4().hex[:6]
        s.command('magic spell set name ' + name, 'name|renamed|called'); s.command('magic spell set difficulty automatic', 'Automatic to cast'); s.command('magic spell close')
        s.command('magic capability edit 5', 'edit'); s.command(f'magic capability set casting entry add {spell}', 'updated|already'); s.command('magic capability close')
        s.command(f'magic casting grant 1 5 {spell} N20 native qualification', 'grant|acquir|already'); s.stop()
        s.sql('UPDATE CharacterTraits SET Value=90 WHERE CharacterId=1 AND TraitDefinitionId=297')
        s.sql('INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES (1,5,1000) ON DUPLICATE KEY UPDATE Amount=1000')
        s.sql(f'UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId=1 AND MagicSpellId={spell}')
        s.report['objects'] = dict(player=1, recipient=3, spell=spell, fixtures=fixture, rooms=places)
        start(); s.command('drop all'); focus = load('focus'); s.report['objects']['focus'] = focus
        cast('qatarget', 0); life, carrier = creation(); s.command('drop qan20pocket', 'drop')
        s.check('pocket is items-only with no created topology or character claim', s.sql(f"SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE LifecycleId='{life}' AND Kind<>0") == '0')
        s.command('enter qan20pocket', seconds=.5); flush()
        s.check('living actor cannot enter pocket storage', s.sql('SELECT Location FROM Characters WHERE Id=1') == str(places['Anchor']))
        bag = load('bag'); s.command('drop qan20focus', 'drop'); child = load('child')
        s.command('put qan20child qan20bag', 'put'); s.command('put qan20bag qan20pocket', 'put'); s.command('get qan20focus', 'get')
        conserved(bag, carrier); conserved(child, bag)
        overflow = load('overflow'); answer = s.command('put qan20overflow qan20pocket', seconds=.5)
        conserved(bag, carrier); conserved(child, bag)
        s.check('over-capacity insertion retains held item', s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={overflow}') == '1', answer)
        answer = s.command('put qan20overflow qan20bag', seconds=.5)
        s.check('nested bag cannot bypass outer capacity', s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={overflow}') == '1', answer)
        s.command('drop qan20overflow', 'drop'); edit('prototype ' + str(fixture['Prototypes']['inner'])); edit('capacity 10 kilograms')
        inner_life, inner = creation('inner', 5 * fixture['Capacity'])
        s.command('drop qan20focus', 'drop'); s.command('get qan20pocket', 'get')
        answer = s.command('put qan20pocket qan20inner', seconds=.5)
        flush()
        s.check('direct pocket nesting refuses with spare capacity', s.sql(f'SELECT ContainerId IS NULL FROM GameItems WHERE Id={inner}') == '1' and
                s.sql(f'SELECT ContainerId IS NULL FROM GameItems WHERE Id={carrier}') == '1' and
                s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={carrier}') == '1', answer)
        s.command('drop qan20pocket', 'drop'); cast(fixture['Keywords']['inner'], 0)
        outer = load('outer'); s.command('put qan20inner qan20outer', 'put')
        s.command('get qan20bag qan20pocket', 'get'); s.command('drop qan20bag', 'drop')
        answer = s.command('put qan20outer qan20pocket', seconds=.5)
        flush()
        s.check('pocket hidden inside ordinary bag refuses nesting', s.sql(f'SELECT ContainerId FROM GameItems WHERE Id={inner}') == str(outer) and
                s.sql(f'SELECT ContainerId IS NULL FROM GameItems WHERE Id={outer}') == '1', answer)
        s.command('get qan20bag', 'get'); s.command('put qan20bag qan20pocket', 'put')
        s.command('get qan20inner qan20outer', 'get'); s.command('junk qan20inner', 'junk'); complete(inner_life, inner)
        edit('prototype ' + str(fixture['Prototypes']['pocket'])); edit('capacity 2 kilograms')
        s.command('drop qan20outer', 'drop'); s.command('get qan20focus', 'get'); s.command('get qan20pocket', 'get'); s.command('give qan20pocket qatarget', 'give')
        flush(); s.check('full carrier transfers to another native body', s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={carrier}') == s.sql('SELECT BodyId FROM Characters WHERE Id=3'))
        paid = reserve(); original = s.sql(f"SELECT DeadlineUtc,Provenance FROM MagicSpellLifecycles WHERE Id='{life}'")
        s.stop(); start()
        s.check('cold restart preserves deadline binding and paid reserve', s.sql(f"SELECT DeadlineUtc,Provenance FROM MagicSpellLifecycles WHERE Id='{life}'") == original and reserve() == paid)
        conserved(bag, carrier); conserved(child, bag)
        s.command('force qatarget drop qan20pocket'); s.command('get qan20pocket', 'get'); flush()
        # Reject only evacuation's exact floor join, leaving manual withdrawal available.
        trigger = 'qa_n20_' + uuid.uuid4().hex[:10]
        s.sql(f"DELIMITER $$\nCREATE TRIGGER {trigger} BEFORE INSERT ON Rooms_GameItems FOR EACH ROW BEGIN IF NEW.GameItemId={bag} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='Owned N20 custody refusal'; END IF; END$$\nDELIMITER ;")
        try:
            s.command('junk qan20pocket', 'not empty|ACCEPT'); s.command('accept', 'prevent safe collapse|remains in your custody', seconds=1)
            limit = time.monotonic() + 90
            while time.monotonic() < limit:
                held = s.sql(f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{life}'")
                if 'error occurred while saving' in held.lower(): break
                s.session.read_for(.5)
            s.check('injected persistence failure reaches evacuation transaction', held.startswith('1\t') and 'error occurred while saving' in held.lower(), held)
            conserved(bag, carrier); conserved(child, bag)
            s.check('failed evacuation retains held carrier and durable retirement hold', s.sql(f"SELECT State FROM MagicSpellLifecycles WHERE Id='{life}'") == '1' and
                    s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={carrier}') == '1')
            s.command('drop qan20focus', 'drop'); s.command('get qan20bag qan20pocket', 'get')
            flush(); s.check('retiring pocket still permits withdrawal', s.sql(f'SELECT ContainerId IS NULL FROM GameItems WHERE Id={bag}') == '1')
            answer = s.command('put qan20bag qan20pocket', seconds=.5)
            s.check('retiring pocket refuses new deposits', s.sql(f'SELECT ContainerId IS NULL FROM GameItems WHERE Id={bag}') == '1', answer)
        finally: s.sql('DROP TRIGGER ' + trigger)
        s.command('drop qan20bag', 'drop'); s.command('junk qan20pocket', seconds=1); complete(life, carrier)
        conserved(bag, room=places['Anchor']); conserved(child, bag); s.command('get qan20focus', 'get')
        # Fresh full carrier collapses early using the same borrowed focus after restart/movement.
        s.command(f"goto {places['Fallback']}", 'already|QA'); edit('lifetime 600'); life2, carrier2 = creation(); s.command('drop qan20pocket', 'drop')
        full = load('full'); s.command('put qan20full qan20pocket', 'put'); s.command('get qan20pocket', 'get')
        s.command('junk qan20pocket', 'not empty|ACCEPT'); s.command('accept', seconds=1); complete(life2, carrier2)
        conserved(full, room=places['Fallback'])
        # Creator access is independent of the carrier's current bearer.
        edit('access Creator'); life_creator, carrier_creator = creation(); s.command('drop qan20pocket', 'drop')
        creator_full = load('full'); s.command('put qan20full qan20pocket', 'put'); s.command('get qan20pocket', 'get')
        s.command('transfer qatarget'); s.command('give qan20pocket qatarget', 'give'); flush()
        answer = s.command('force qatarget get qan20full qan20pocket', seconds=.7); flush()
        s.check('Creator access refuses withdrawal by a different bearer', s.sql(f'SELECT ContainerId FROM GameItems WHERE Id={creator_full}') == str(carrier_creator), answer)
        s.command('force qatarget drop qan20pocket'); s.command('get qan20pocket', 'get'); s.command('drop qan20focus', 'drop')
        s.command('get qan20full qan20pocket', 'get'); flush()
        s.check('canonical creator can withdraw after carrier transfer', s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={creator_full}') == '1')
        s.command('put qan20full qan20pocket', 'put'); s.command('get qan20focus', 'get')
        s.command('junk qan20pocket', 'not empty|ACCEPT'); s.command('accept', seconds=1); complete(life_creator, carrier_creator)
        conserved(creator_full, room=places['Fallback']); edit('access Bearer')
        # Occupied cold-loaded expiry follows the original absolute deadline.
        edit('lifetime 75.0000001'); life3, carrier3 = creation(); s.command('drop qan20pocket', 'drop'); full2 = load('full')
        s.command('put qan20full qan20pocket', 'put'); s.command('get qan20pocket', 'get'); s.command('transfer qatarget'); s.command('give qan20pocket qatarget', 'give')
        flush(); deadline = s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{life3}'"); paid = reserve()
        s.report['recipientProgressBefore'] = branching_progress(3)
        s.stop(); start(); s.check('full transferred cold pocket retains absolute deadline and payment', s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{life3}'") == deadline and reserve() == paid)
        complete(life3, carrier3, 130); conserved(full2, room=places['Fallback'])
        s.check('occupied expiry uses expiry reason', s.sql(f"SELECT Reason FROM MagicSpellLifecycles WHERE Id='{life3}'") == '0')
        s.report['recipientProgressAfter'] = branching_progress(3)
        s.check('recipient branching history survives cold occupied expiry', s.report['recipientProgressBefore'] == s.report['recipientProgressAfter'])
        s.check('borrowed casting focus survives every carrier collapse', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={focus}') == '1' and
                s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={focus}') == '0')
        s.check('all captured foreign goods survive exactly once', all(s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={item}') == '1' for item in goods))
        s.stop(); s.check('all captured native inputs remain unchanged', all(hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error:
        s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
    finally:
        try: s.finish()
        finally:
            if mysql:
                try: s.sql('SHUTDOWN')
                except AssertionError: pass
                s.helper.stop_process_tree(mysql); s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
                end = time.monotonic() + 30; closed = False
                while time.monotonic() < end:
                    with socket.socket() as probe: closed = probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0
                    if closed: break
                    time.sleep(.5)
                s.report['mysqlPortClosed'] = closed
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')


if __name__ == '__main__':
    run()
    raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
