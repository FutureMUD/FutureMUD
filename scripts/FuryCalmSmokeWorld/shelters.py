"""N17: occupied native shelters in the retained, owned loopback world."""
import hashlib
import importlib.util
import json
import math
import pathlib
import re
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report.update(scope='N17 builder-installed occupied shelters; explicit native timing and terrain policy',
                qualificationMarker='occupied-shelters-passed.json', createdLifecycles=[])
s.deadline = time.monotonic() + 1500
lifetimes = {'Spring Haven': 120, 'Burrow Refuge': 180, 'Sand Shelter': 210}
possessed = False
original_command = s.command


def tracked_command(text, expected=None, seconds=.25):
    global possessed
    if text == '1' and expected == 'Guest Lounge':
        login_room = s.sql('SELECT r.Id,o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id=1').split('\t')
        s.check('login uses the exact surviving native PC room', len(login_room) == 2, '\t'.join(login_room))
        s.report.setdefault('loginRooms', []).append(login_room)
        expected = re.escape(login_room[1])
    if text.startswith('possess '):
        possessed = True
    started = time.time()
    try:
        answer = original_command(text, expected, seconds)
        if text == 'return':
            possessed = False
        return answer
    finally:
        s.report.setdefault('commandTiming', []).append(dict(command=text, startedUtcEpoch=started,
                                                            elapsedSeconds=time.time() - started))


s.command = tracked_command
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/OwnedShelterFixtures.cs',
             *sorted((s.repo / 'MudSharpCore/Magic/Lifecycle').glob('SpellOwnedShelterService*.cs')),
             s.repo / 'MudSharpCore/Magic/SpellEffects/CreateShelterEffect.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()


def flush():
    s.command('impdebug flush', seconds=.7)


def wait_for(label, query, predicate, seconds=130):
    end = time.monotonic() + seconds
    while time.monotonic() < end:
        row = s.sql(query)
        if predicate(row):
            s.check(label, True, row)
            return row
        if s.session:
            s.session.read_for(.5)
        else:
            time.sleep(.5)
    s.check(label, False, s.sql(query))


def lives():
    return set(s.sql("SELECT Id FROM MagicSpellLifecycles WHERE Family='native-occupied-shelter-v1'").splitlines())


def cast(name):
    previous = lives()
    before = s.resource(s.caster)
    s.cast(name, 1, 'here', 'The paid casting succeeded')
    flush()
    new = lives() - previous
    s.check(name + ' pays exactly nine reserve units and creates one graph', len(new) == 1 and before - s.resource(s.caster) == 9)
    identifier = new.pop()
    claims = dict(line.split('\t') for line in s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind").splitlines())
    s.check('only exact declared topology and optional pool are claimed', set(claims) == ({'0', '4', '5', '6'} if name == 'Spring Haven' else {'4', '5', '6'}))
    record = dict(id=identifier, name=name, claims=claims)
    s.report['createdLifecycles'].append(record)
    s.check('native sheltered room is temporary with a usable indoor ground overlay',
            s.sql(f"SELECT r.Temporary+0,o.OutdoorsType,o.SafeQuit+0 FROM Rooms r JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE r.Id={claims['4']}") == '1\t0\t1')
    return record


def enter(name, keyword):
    s.possess(name)
    title = {'haven': 'QA Spring Haven', 'burrow': 'QA Burrow Refuge', 'shelter': 'QA Sand Shelter'}[keyword]
    arrival('enter ' + keyword, title)
    s.command('return')
    flush()


def arrival(command, title):
    answer = s.command(command, 'begin|move|enter|leave', seconds=.5)
    limit = time.monotonic() + 30
    while title not in answer and time.monotonic() < limit:
        answer += s.session.read_for(.5)
    s.check('native movement arrives at ' + title, title in answer, answer[-2000:])


def completed(record, destination, visitors, goods=()):
    identifier, claims = record['id'], record['claims']
    remaining = float(s.sql(f"SELECT TIMESTAMPDIFF(MICROSECOND,UTC_TIMESTAMP(6),DeadlineUtc)/1000000 FROM MagicSpellLifecycles WHERE Id='{identifier}'"))
    s.check('retirement wait stays within the fixed qualification policy', remaining <= max(lifetimes.values()))
    wait_for(record['name'] + ' retires through the journal',
             f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{identifier}'", lambda row: row.split('\t')[0] == '3',
             max(90, math.ceil(remaining) + 90))
    s.check('only the owned topology rows disappear',
            s.sql(f"SELECT (SELECT COUNT(*) FROM Rooms WHERE Id={claims['4']}),(SELECT COUNT(*) FROM Exits WHERE Id={claims['5']}),(SELECT COUNT(*) FROM RoomOverlays WHERE Id={claims['6']})") == '0\t0\t0')
    for visitor in visitors:
        s.check('visitor identity and body survive at the exact safe return room',
                s.sql(f'SELECT Location,BodyId IS NOT NULL,IsArchived+0 FROM Characters WHERE Id={visitor}') == f'{destination}\t1\t0')
        s.check('every physical instance of the visitor returns safely',
                s.sql(f'SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId={visitor} AND LocationId<>{destination}') == '0')
    for item in goods:
        s.check('unclaimed visitor goods survive in the exact return room',
                s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={item}),(SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}),(SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item})') == f'1\t{destination}\t0')
    record['terminal'] = s.sql(f"SELECT State,Reason,Version,HEX(Diagnostic) FROM MagicSpellLifecycles WHERE Id='{identifier}'")


def location(character):
    return int(s.sql(f'SELECT Location FROM Characters WHERE Id={character}'))


def relocate(name, room):
    s.command(f'goto {room}')
    s.command(f'transfer {name}')
    flush()


def foreign_staff(room):
    before = set(s.sql('SELECT Id FROM GameItems').splitlines())
    s.command(f'goto {room}')
    s.command(f'item load {staff}', 'load|hands|create')
    s.command('drop staff', 'drop')
    flush()
    new = set(s.sql('SELECT Id FROM GameItems').splitlines()) - before
    s.check('one ordinary foreign item created', len(new) == 1)
    return int(new.pop())


def destroy(record):
    room = int(record['claims']['4'])
    flush()
    s.check('public deletion targets the exact occupied temporary shelter',
            location(1) == room and s.sql(f"SELECT Temporary+0 FROM Rooms WHERE Id={room}") == '1' and
            s.sql(f"SELECT State FROM MagicSpellLifecycles WHERE Id='{record['id']}'") == '0')
    s.command('room delete', 'Are you sure')
    s.command('accept delete', 'delete')
    flush()


def draw_cup():
    body = int(s.sql('SELECT BodyId FROM Characters WHERE Id=1'))
    previous = s.sql(f"SELECT COUNT(*) FROM Bodies_GameItems b JOIN GameItems i ON i.Id=b.GameItemId WHERE b.BodyId={body} AND i.GameItemProtoId={fixture['CupPrototype']}")
    for _ in range(int(previous)):
        s.command('drop qadrawcup', 'drop')
        flush()
    s.check('retained fixture cups leave inventory without deletion',
            s.sql(f"SELECT COUNT(*) FROM Bodies_GameItems b JOIN GameItems i ON i.Id=b.GameItemId WHERE b.BodyId={body} AND i.GameItemProtoId={fixture['CupPrototype']}") == '0')
    before = set(s.sql('SELECT Id FROM GameItems').splitlines())
    s.command(f"item load {fixture['CupPrototype']}", 'load|hands|create')
    flush()
    new = set(s.sql('SELECT Id FROM GameItems').splitlines()) - before
    s.check('one new unambiguous native draw cup is carried', len(new) == 1)
    item = int(new.pop())
    s.check('the exact new cup has native inventory custody', s.sql(f'SELECT COUNT(*) FROM Bodies_GameItems WHERE BodyId={body} AND GameItemId={item}') == '1')
    return item


def volume(item):
    root = ET.fromstring(s.sql(f"SELECT c.Definition FROM GameItemComponents c JOIN GameItemComponentProtos p ON p.Id=c.GameItemComponentProtoId AND p.RevisionNumber=c.GameItemComponentProtoRevision WHERE c.GameItemId={item} AND p.Type IN ('Liquid Container','Puddle')"))
    # LiquidInstance.SaveToXml owns these exact attributes.
    return sum(float(node.attrib['amount']) for node in root.findall('.//Liquid[@amount]'))


def floor_volume(room):
    xml = s.sql(f"SELECT IFNULL(HEX(SurfaceLiquidData),'') FROM Rooms WHERE Id={room}")
    amount = sum(float(x.attrib['amount']) for x in ET.fromstring(bytes.fromhex(xml)).findall('.//Liquid[@amount]')) if xml else 0
    puddles = s.sql(f"SELECT DISTINCT i.Id FROM GameItems i JOIN Rooms_GameItems r ON r.GameItemId=i.Id JOIN GameItemComponents c ON c.GameItemId=i.Id JOIN GameItemComponentProtos p ON p.Id=c.GameItemComponentProtoId AND p.RevisionNumber=c.GameItemComponentProtoRevision WHERE r.RoomId={room} AND p.Type='Puddle'")
    return amount + sum(volume(int(item)) for item in puddles.splitlines())


def item_surface_volume(item):
    xml = s.sql(f"SELECT IFNULL(HEX(SurfaceLiquidData),'') FROM GameItems WHERE Id={item}")
    return sum(float(x.attrib['amount']) for x in ET.fromstring(bytes.fromhex(xml)).findall('.//Liquid[@amount]')) if xml else 0


def permits(character):
    xml = s.sql(f'SELECT HEX(EffectData) FROM Characters WHERE Id={character}')
    return [int(x.find('Effect/Cell').text) for x in ET.fromstring(bytes.fromhex(xml)).findall('Effect')
            if x.findtext('Type') == 'PermitWork' and x.find('Effect/Cell') is not None]


original_flags = None
drying_original = None
mount_binding_original = None
try:
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text())
    for key in ['caster', 'capability', 'reserve']:
        setattr(s, key, actors[key])
    bindings = json.loads((s.root / 'fury-calm-bindings.json').read_text())
    fixture = s.fixture('shelter-fixtures')
    mount_npc = int(s.sql(f"SELECT Id FROM Npcs WHERE CharacterId={actors['other']}"))
    mount_binding_original = s.sql(f"SELECT COUNT(*) FROM Npcs_ArtificialIntelligences WHERE NpcId={mount_npc} AND ArtificialIntelligenceId={fixture['MountAi']}")
    drying_original = s.sql("SELECT HEX(Definition) FROM StaticConfigurations WHERE SettingName='LiquidContaminationEffectDuration'")
    s.sql("UPDATE StaticConfigurations SET Definition='86400' WHERE SettingName='LiquidContaminationEffectDuration'")
    s.report['authoredTiming'] = dict(secondsPerGrade=lifetimes, maximumOccupants=2, initialLitresPerGrade=10,
                                     fixtureSurfaceDryingIntervalSeconds=86400, sourceParity=False)
    s.report['scenarioDeadlineSeconds'] = 1500
    anchor, fallback = fixture['Anchor'], fixture['Fallback']
    staff = int(s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.item.charged_staff'"))
    original_flags = s.sql(f'SELECT Id,Temporary+0 FROM Rooms WHERE Id IN ({anchor},{fallback}) ORDER BY Id')
    s.report['fixturePolicyBefore'] = dict(roomFlags=original_flags, dryingDefinitionHex=drying_original)
    baseline_reserve = float(s.sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}'))
    s.report['objects'] = dict(actors, **fixture)
    s.report['reserveBefore'] = baseline_reserve
    s.start()
    school, resource = bindings['Utilities']['School'], bindings['Utilities']['Resource']
    trait = int(s.sql(f"SELECT CastingTraitDefinitionId FROM MagicSpells WHERE Id={actors['fury']}"))
    for key, title, template in [('spring-haven', 'Spring Haven', 'Spring'), ('burrow-refuge', 'Burrow Refuge', 'Burrow'), ('sand-shelter', 'Sand Shelter', 'Sand')]:
        existing = s.sql(f"SELECT Id FROM MagicSpells WHERE Name='{title}'")
        if existing:
            s.command(f'magic spell edit {existing}', 'edit')
        else:
            suffix = f" {fixture['PoolPrototype']} {fixture['Water']} 10" if key == 'spring-haven' else ''
            lifetime = lifetimes[title]
            s.command(f"magic spell edit new stock {key} {school} {trait} {resource} {fixture[template]} {fixture['Terrain']} {fallback} {lifetime} 2{suffix}", 'Created.*' + title)
        s.command(f'magic spell set effect 1 lifetime {lifetimes[title]}', 'updated')
        s.command('magic spell set difficulty automatic', 'Automatic to cast')
        s.command('magic spell close')
        flush()
        spell = int(s.sql(f"SELECT Id FROM MagicSpells WHERE Name='{title}'"))
        s.command(f'magic capability edit {s.capability}', 'edit')
        s.command(f'magic capability set casting entry add {spell}', 'updated|already')
        s.command('magic capability close')
        s.command(f'magic casting grant {s.caster} {s.capability} {spell} N17 owned qualification', 'grant|acquir|already')
    # Controlled grade is a stopped qualification binding; no reserve is reset or refunded.
    s.stop()
    s.sql("UPDATE CharacterAcquiredSpells a JOIN MagicSpells m ON m.Id=a.MagicSpellId SET a.ControlledGrade=7 WHERE a.CharacterId=" + str(s.caster) + " AND m.Name IN ('Spring Haven','Burrow Refuge','Sand Shelter')")
    s.start()
    relocate('qacaster', fixture['Incompatible'])
    before, count = s.resource(s.caster), len(lives())
    for title in ['Spring Haven', 'Burrow Refuge', 'Sand Shelter']:
        s.cast(title, 1, 'here', 'terrain|does not admit')
    s.check('incompatible terrain refuses every shelter before payment or creation', s.resource(s.caster) == before and len(lives()) == count)
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    spring = cast('Spring Haven')
    room, pool = int(spring['claims']['4']), int(spring['claims']['0'])
    enter('qacaster', 'haven')
    s.check('ordinary non-cardinal entry reaches the new usable haven', location(s.caster) == room)
    goods = foreign_staff(room)
    s.command('permitwork qacaster 1 hour', 'authorise')
    s.command('look', 'QA Spring Haven')
    flush()
    s.check('native room-specific permit is durable before retirement', room in permits(s.caster))
    log_query = f"SELECT Id,IFNULL(AccountId,0),CharacterId,HEX(Command),DATE_FORMAT(Time,'%Y-%m-%d %H:%i:%s.%f'),IsPlayerCharacter+0 FROM CharacterLog WHERE RoomId={room} ORDER BY Id"
    # LogManager flushes the native command queue each world tick.
    audit_before = wait_for('native PC command logs exist inside the shelter', log_query, lambda row: bool(row), 15)
    audit_ids = ','.join(row.split('\t')[0] for row in audit_before.splitlines())
    s.report.setdefault('auditConservation', []).append(dict(before=audit_before, room=room, returnRoom=anchor))
    cup = draw_cup()
    initial = volume(pool)
    s.command('fill qadrawcup basin 1 litre', 'fill|pour')
    flush()
    s.check('native withdrawal conserves finite water into an ordinary vessel', 0 < volume(pool) < initial and abs(volume(pool) + volume(cup) - initial) < .001, f'initial={initial}; pool={volume(pool)}; cup={volume(cup)}')
    s.command('spill qadrawcup basin', 'You spill')
    flush()
    exterior = item_surface_volume(pool)
    s.check('ordinary native spill wets the owned basin exterior without refilling it', exterior > 0 and volume(cup) == 0)
    s.command('fill qadrawcup basin 1 litre', 'You fill|You pour')
    flush()
    s.command('empty qadrawcup', 'empty|pour')
    flush()
    conserved = floor_volume(anchor) + floor_volume(room) + volume(pool) + item_surface_volume(pool)
    s.command(f'goto {anchor}')
    s.command(f'goto {room}')
    destroy(spring)
    completed(spring, anchor, [s.caster, 1], [goods])
    flush()
    s.check('remaining pool water and foreign spilled water survive exactly once at return', abs(floor_volume(anchor) - conserved) < .001,
            f'before={conserved}; after={floor_volume(anchor)}')
    s.check('finite source is removed without deleting the borrowed draw cup', s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={pool}),(SELECT COUNT(*) FROM GameItems WHERE Id={cup})') == '0\t1')
    audit_after = s.sql(log_query.replace(f'RoomId={room}', f'Id IN ({audit_ids})'))
    s.check('shelter command log IDs and payloads survive topology removal', audit_after == audit_before and
            s.sql(f'SELECT COUNT(*) FROM CharacterLog WHERE Id IN ({audit_ids}) AND RoomId={anchor}') == str(len(audit_before.splitlines())))
    s.check('retirement removes exact shelter work permits without granting work at the anchor', room not in permits(s.caster) and anchor not in permits(s.caster))
    # Burrow occupied cold restart: preserve same deadline and native graph, then actual expiry.
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    burrow = cast('Burrow Refuge')
    enter('qacaster', 'burrow')
    s.check('burrow is native topology below the entrance', s.sql(f"SELECT r.Z<a.Z FROM Rooms r JOIN Rooms a ON a.Id={anchor} WHERE r.Id={burrow['claims']['4']}") == '1')
    goods = foreign_staff(int(burrow['claims']['4']))
    s.command(f'goto {anchor}')
    deadline = s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{burrow['id']}'")
    count, paid = len(lives()), s.resource(s.caster)
    s.command('goto 1')
    s.stop()
    s.start()
    s.check('occupied cold restart does not recreate topology, repay, or extend the deadline', len(lives()) == count and s.resource(s.caster) == paid and s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{burrow['id']}'") == deadline)
    claims = burrow['claims']
    s.check('same occupied room, overlay, entrance and goods survive the active restart',
            s.sql(f"SELECT (SELECT COUNT(*) FROM Rooms WHERE Id={claims['4']}),(SELECT COUNT(*) FROM RoomOverlays WHERE Id={claims['6']}),(SELECT COUNT(*) FROM Exits WHERE Id={claims['5']}),(SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={goods})") == f"1\t1\t1\t{claims['4']}" and location(s.caster) == int(claims['4']))
    s.command(f"goto {claims['4']}")
    s.possess('qacaster')
    arrival('leave outside', 'QA Shelter Clearing')
    s.command('return')
    flush()
    s.check('the same native entrance returns an occupant after restart', location(s.caster) == anchor)
    s.command(f'goto {anchor}')
    reentry_budget = s.sql(f"SELECT State,TIMESTAMPDIFF(MICROSECOND,UTC_TIMESTAMP(6),DeadlineUtc)/1000000 FROM MagicSpellLifecycles WHERE Id='{burrow['id']}'").split('\t')
    s.check('original Burrow deadline has time for native re-entry after restart',
            reentry_budget[0] == '0' and float(reentry_budget[1]) > 30, '\t'.join(reentry_budget))
    enter('qacaster', 'burrow')
    completed(burrow, anchor, [s.caster], [goods])
    # Sand capacity, forced entry, mount/rider, contents, weather classification and live expiry.
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    sand = cast('Sand Shelter')
    shelter = int(sand['claims']['4'])
    relocate('qatarget', anchor)
    relocate('qaother', anchor)
    s.command(f'ai add {fixture["MountAi"]} qaother')
    enter('qatarget', 'shelter')
    enter('qaother', 'shelter')
    s.possess('qacaster')
    s.command('enter shelter', 'cannot|full|closing|not able')
    s.command('return')
    flush()
    s.check('full shelter refuses ordinary entry', location(s.caster) == anchor)
    s.command(f'goto {shelter}')
    flush()
    s.check('forced goto also respects the same capacity', location(1) == anchor)
    # Admin is correctly refused while full; give goods from outside after a visitor leaves temporarily.
    relocate('qaother', anchor)
    goods = foreign_staff(shelter)
    s.command(f'goto {anchor}')
    enter('qaother', 'shelter')
    # Possession is local-only: possess outside, then enter normally while controlled.
    # Both rider and mount occupy the full shelter; the staff body stays outside.
    relocate('qatarget', anchor)
    s.possess('qatarget')
    arrival('enter shelter', 'QA Sand Shelter')
    # Room entry precedes Movement.FinalStep's native cleanup (up to five seconds).
    # Retry only the command manager's pre-mutation movement refusal, bounded by 15 seconds.
    mount_deadline = time.monotonic() + 15
    while True:
        mount_answer = s.command('mount qaother', seconds=.5)
        while not re.search(r'You must stop moving|You mount', mount_answer, re.I) and time.monotonic() < mount_deadline:
            mount_answer += s.session.read_for(.25)
        if 'You must stop moving before you can do that.' not in mount_answer or time.monotonic() >= mount_deadline:
            break
        s.session.read_for(.5)
    s.check('native movement finishes and the rider mounts through the normal command',
            bool(re.search(r'\bYou mount\b', mount_answer, re.I)), mount_answer)
    s.command('return')
    flush()
    s.check('a real native riding link exists before occupied expiry', s.sql(f"SELECT PositionTargetId,PositionTargetType FROM Characters WHERE Id={actors['target']}") == f"{actors['other']}\tCharacter")
    completed(sand, anchor, [actors['target'], actors['other']], [goods])
    s.check('retirement dismounts before moving either borrowed endpoint', s.sql(f"SELECT PositionTargetId IS NULL FROM Characters WHERE Id={actors['target']}") == '1')
    # No safe return: keep the closed topology and occupant until the configured fallback is repaired.
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    held = cast('Sand Shelter')
    enter('qacaster', 'shelter')
    s.command(f"goto {held['claims']['4']}")
    s.stop()
    s.sql(f'UPDATE Rooms SET Temporary=1 WHERE Id IN ({anchor},{fallback})')
    wait_for('absolute occupied deadline passes while offline', f"SELECT DeadlineUtc<=UTC_TIMESTAMP(6) FROM MagicSpellLifecycles WHERE Id='{held['id']}'", lambda row: row == '1', lifetimes['Sand Shelter'] + 15)
    s.start()
    s.check('unsafe return holds a closed recoverable topology and exact occupant', s.sql(f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{held['id']}'").startswith('1\tShelter retirement held:') and location(s.caster) == int(held['claims']['4']))
    s.check('offline PC reconnects to its exact retained closed shelter', location(1) == int(held['claims']['4']))
    s.command('room delete', 'Are you sure')
    s.command('accept delete', 'held for recovery')
    flush()
    s.check('held public deletion reports recovery and retains the exact native room', s.sql(f"SELECT COUNT(*) FROM Rooms WHERE Id={held['claims']['4']}") == '1')
    s.stop()
    s.sql(f'UPDATE Rooms SET Temporary=0 WHERE Id={fallback}')
    s.start()
    completed(held, fallback, [s.caster, 1])
    s.command('goto 1')
    s.stop()
    s.sql(f'UPDATE Rooms SET Temporary=0 WHERE Id={anchor}')
    s.start()
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    expired_spring = cast('Spring Haven')
    enter('qacaster', 'haven')
    spring_room, spring_pool = int(expired_spring['claims']['4']), int(expired_spring['claims']['0'])
    s.command(f'goto {spring_room}')
    second_cup = draw_cup()
    finite = volume(spring_pool)
    s.command('fill qadrawcup basin 100 litres', 'fill')
    flush()
    s.check('native draw cannot exceed the finite available source', volume(spring_pool) == 0 and abs(volume(second_cup) - finite) < .001)
    s.command('fill qadrawcup basin 1 litre', 'empty')
    flush()
    s.check('exhausted-source overdraw refuses without recreating quantity', volume(spring_pool) == 0 and abs(volume(second_cup) - finite) < .001)
    s.command(f'goto {anchor}')
    completed(expired_spring, anchor, [s.caster])
    relocate('qacaster', anchor)
    s.command(f'goto {anchor}')
    destroyed_sand = cast('Sand Shelter')
    enter('qacaster', 'shelter')
    s.command(f"goto {destroyed_sand['claims']['4']}")
    destroy(destroyed_sand)
    completed(destroyed_sand, anchor, [s.caster, 1])
    s.command('goto 1')
    s.stop()
    # Remove only the unused owned QA source room while both an NPC and offline PC occupy its burrow.
    missing = fixture['Incompatible']
    s.start()
    spell = int(s.sql("SELECT Id FROM MagicSpells WHERE Name='Burrow Refuge'"))
    terrain = int(s.sql(f'SELECT TerrainId FROM RoomOverlays WHERE Id=(SELECT CurrentOverlayId FROM Rooms WHERE Id={missing})'))
    s.command(f'magic spell edit {spell}')
    s.command(f'magic spell set effect 1 terrain {terrain}', 'updated')
    s.command('magic spell close')
    relocate('qacaster', missing)
    s.command(f'goto {missing}')
    lost_anchor = cast('Burrow Refuge')
    enter('qacaster', 'burrow')
    s.command(f"goto {lost_anchor['claims']['4']}")
    flush()
    s.check('PC and NPC occupy the exact burrow before cold shutdown', location(1) == location(s.caster) == int(lost_anchor['claims']['4']))
    s.stop()
    s.check('owned missing-anchor fixture has no visitors or goods before removal',
            s.sql(f'SELECT (SELECT COUNT(*) FROM Characters WHERE Location={missing}),(SELECT COUNT(*) FROM CharacterInstances WHERE LocationId={missing}),(SELECT COUNT(*) FROM Rooms_GameItems WHERE RoomId={missing})') == '0\t0\t0')
    s.sql(f'UPDATE Rooms SET CurrentOverlayId=NULL WHERE Id={missing}; DELETE FROM RoomOverlays WHERE RoomId={missing}; DELETE FROM Rooms WHERE Id={missing}')
    wait_for('original deadline passes without a server or anchor', f"SELECT DeadlineUtc<=UTC_TIMESTAMP(6) FROM MagicSpellLifecycles WHERE Id='{lost_anchor['id']}'", lambda row: row == '1', lifetimes['Burrow Refuge'] + 15)
    s.start()
    completed(lost_anchor, fallback, [s.caster, 1])
    s.command('goto 1')
    s.command(f'magic spell edit {spell}')
    s.command(f'magic spell set effect 1 terrain {terrain}', 'updated')
    s.command('magic spell close')
    s.stop()
    s.report['reserveAfter'] = float(s.sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}'))
    s.check('no reserve reset or refund; exactly seven paid casts', s.report['reserveBefore'] - s.report['reserveAfter'] == 63)
    s.report.update(status='PASS', milestoneQualified=True, n17BuilderInstalledQualified=True)
except BaseException as error:
    s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
finally:
    s.deadline = max(s.deadline, time.monotonic() + 45)
    if s.process is not None:
        try:
            if possessed:
                s.command('return', 'return.*body')
            s.command('goto 1')
            s.stop()
        except BaseException as error:
            s.report.update(status='FAIL', cleanupError=s.helper.redact(str(error), [s.connection, s.password]))
            s.helper.stop_process_tree(s.process)
            s.process = None
            if s.session is not None:
                s.session.close()
                s.session = None
    try:
        if original_flags is not None:
            for row in original_flags.splitlines():
                identifier, flag = row.split('\t')
                s.sql(f'UPDATE Rooms SET Temporary={flag} WHERE Id={identifier}')
        if drying_original is not None:
            s.sql(f"UPDATE StaticConfigurations SET Definition=CONVERT(UNHEX('{drying_original}') USING utf8mb4) WHERE SettingName='LiquidContaminationEffectDuration'")
            s.check('native drying policy restored exactly after stopped qualification', s.sql("SELECT HEX(Definition) FROM StaticConfigurations WHERE SettingName='LiquidContaminationEffectDuration'") == drying_original)
        if mount_binding_original == '0':
            s.sql(f"DELETE FROM Npcs_ArtificialIntelligences WHERE NpcId={mount_npc} AND ArtificialIntelligenceId={fixture['MountAi']}")
            s.check('qualification mount binding restored while stopped', s.sql(f"SELECT COUNT(*) FROM Npcs_ArtificialIntelligences WHERE NpcId={mount_npc} AND ArtificialIntelligenceId={fixture['MountAi']}") == '0')
        s.report['inputsUnchanged'] = all(hashlib.sha256((s.repo / name).read_bytes()).hexdigest() == digest for name, digest in s.report['inputs'].items())
        s.check('source and binaries remained frozen throughout the scenario', s.report['inputsUnchanged'])
    except BaseException as error:
        s.report.update(status='FAIL', cleanupError=s.helper.redact(str(error), [s.connection, s.password]))
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
