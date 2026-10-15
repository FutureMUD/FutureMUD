"""Focused real first/cold checks; no Exchange campaign and no package admissions."""
from pathlib import Path
import time
import xml.etree.ElementTree as ET


def run(host, session, phase, data, mapped):
    sql, require, receipt, assertions = (host[key] for key in ('sql', 'require', 'receipt', 'assertions'))
    configuration = data['remaining_stock']
    source = configuration['source']
    require(Path(source['path']).resolve() == host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/Program.RemainingStockFixture.cs'
            and host['sha'](source['path']) == source['sha256'], 'Remaining fixture source pin differs.')
    routes = {row['key']: row for row in configuration['routes']}
    actor, body = int(data['character']), int(data['body'])
    home, destination = int(configuration['home']), int(configuration['destination'])
    report = receipt.setdefault('remaining_stock', {})
    # mysql batch output can retain carriage returns from Windows XML. HEX is
    # the existing harness transport for multiline persisted definitions.
    probe = "CONCAT('<Effects>',CHAR(13),CHAR(10),'<Type>transport-probe</Type></Effects>')"
    report['xml_transport'] = {'raw_rows': sql('SELECT ' + probe),
                               'hex_rows': sql('SELECT HEX(' + probe + ')', (0,))}
    require(report['xml_transport']['hex_rows'] == [['<Effects>\r\n<Type>transport-probe</Type></Effects>']],
            'Persisted XML transport did not preserve Windows newlines.')

    def flush():
        require('All queued saves have been flushed' in session.send('impdebug flush', read_seconds=.3), 'Remaining stock save flush failed.')

    def location():
        flush()
        return int(sql(f'SELECT Location FROM Characters WHERE Id={actor}')[0][0])

    def balance(row):
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={int(row["resource"])}),0)'))[0][0])

    def operations(row):
        return sql(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} AND MagicSpellId={int(row["spell"])} ORDER BY CreatedUtc,Id', (0, 2))

    def cast(row, target, maximum=4):
        for attempt in range(maximum):
            flush()
            current = balance(row)
            if current < 99:
                session.send(f'{row["invoked"]} gather {int(row["capability"])} draw {100-current:.4f}', read_seconds=.5)
                deadline = time.monotonic() + 18
                while time.monotonic() < deadline and balance(row) < 99:
                    session.read_for(.5)
                require(balance(row) >= 99, 'Bounded native gathering did not fund the next paid operation.')
            before = operations(row)
            output = session.send(f'{row["invoked"]} cast "{row["name"]}" grade {int(row["grade"])} on {target} via {int(row["capability"])}', read_seconds=.5)
            flush()
            added = [x for x in operations(row) if x[0] not in {old[0] for old in before}]
            require(len(added) == 1 and added[0][1] == 'Completed', 'Remaining cast did not complete one paid operation: ' + host['redact'](output))
            payload = ET.fromstring(added[0][2])
            report.setdefault('attempts', {}).setdefault(row['key'], []).append({'operation': added[0][0], 'payload': added[0][2], 'output': host['redact'](output)})
            if payload.get('applied') == 'true':
                return added[0][0]
            opposed = row['key'] in ('arm.spell.silence_the_mind', 'arm.spell.compelling_regard')
            require(payload.get('outcome') in ('MajorFail', 'Fail', 'MinorFail') or opposed,
                    'A paid remaining operation had a non-roll rejection: ' + added[0][2])
        raise RuntimeError('Four bounded native paid attempts did not apply ' + row['key'])

    return_route = routes['arm.spell.return_tether']
    storm = routes['arm.spell.storm_locus']
    waystep = routes['arm.spell.waystep']
    if phase == 'cold':
        require(location() == destination, 'Cold boot changed the saved exact caster location.')
        require(sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{report["return_origin"]}"') == [['3']], 'Cold boot reopened completed Return Tether.')
        lives = sql(f'SELECT State,Version,RoomHazardLastPulseUtc,RoomHazardNextPulseUtc FROM MagicSpellLifecycles WHERE HEX(Id)="{report["storm_origin"]}"')
        require(len(lives) == 1 and int(lives[0][0]) in (0, 3) and int(lives[0][1]) >= report['storm_version'], 'Cold hazard journal lost its durable cursor.')
        require(all(balance(row) == report['balances'][key] for key, row in routes.items()), 'Cold effects refilled or charged casting reserves.')
        before = len(operations(waystep)); funds = balance(waystep)
        session.send(f'{waystep["invoked"]} cast "Waystep" grade 1 on random via {int(waystep["capability"])}', read_seconds=.5); flush()
        require(location() == destination and balance(waystep) == funds and len(operations(waystep)) == before,
                'Cold Waystep should refuse an empty eligible pool before payment.')
        report['cold_status'] = 'PASS'
        assertions.append('S4 stock remaining: cold journal cursors and completed return retained, reserves unchanged, empty pool refused before payment')
        host['stock_definition_loading'](session, phase, data, mapped)
        return

    host['stock_definition_loading'](session, phase, data, mapped)
    require(location() == home, 'Remaining scenario did not start at its exact home.')
    for row in routes.values():
        session.send(f'givemerit me {int(row["merit"])}', read_seconds=.3)
        session.send(f'magic casting enrol {actor} {int(row["capability"])} disposable remaining-stock acceptance', read_seconds=.5); flush()
        acquired = sql(mapped(f'SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} AND MagicSpellId={int(row["spell"])}'))
        require(acquired == [[str(row['grade'])]] and balance(row) == 0, 'Explicit fixture enrolment did not acquire without refill.')
    session.send('drop all', read_seconds=.5)
    cast(return_route, 'self')
    lives = sql(f"SELECT HEX(Id) FROM MagicSpellLifecycles WHERE SpellId={int(return_route['spell'])} AND CreatorId={actor} AND Family='delayed-return'")
    require(len(lives) == 1, 'Return Tether did not persist one exact journal.')
    report['return_origin'] = lives[0][0]
    cast(waystep, 'random'); require(location() == destination, 'Mapped Waystep did not move the actual native body.')
    deadline = time.monotonic() + 68
    while time.monotonic() < deadline:
        if location() == home: break
        time.sleep(1)
    require(location() == home and sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{report["return_origin"]}"') == [['3']],
            'Delayed native return did not reach the recorded home and complete exactly once.')
    assertions.append('S4 stock remaining: paid Waystep moved the exact body through its mapped pool and journalled Return Tether returned it once')
    session.send(f'goto {destination}', read_seconds=.5); require(location() == destination, 'Disposable hazard destination was not reached.')
    template = sql(f'SELECT UniqueName FROM NPCTemplates WHERE Id={int(configuration["target_template"])}')[0][0]
    session.send('npc load ' + template, read_seconds=.5); flush()
    target = sql(f'SELECT c.BodyId,c.Id FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE n.TemplateId={int(configuration["target_template"])} AND c.Location={destination} ORDER BY c.Id DESC LIMIT 1')
    require(len(target) == 1, 'Native electrical hazard target was not loaded.')
    target_body = int(target[0][0])
    target_actor = int(target[0][1])
    session.send('dub identity mount remaining-traveller', read_seconds=.4); flush()
    require(sql(f"SELECT WasIdentityConcealed FROM Dubs WHERE CharacterId={actor} AND TargetId={target_actor} AND TargetType='Character'") == [['0']],
            'Ordinary native dub did not establish unobscured identity provenance.')
    cast(routes['arm.spell.silence_the_mind'], 'remaining-traveller')
    flush()
    silence_xml = sql(f'SELECT HEX(EffectData) FROM Characters WHERE Id={target_actor}', (0,))[0][0]
    report['silence_observation'] = {'target_character': target_actor, 'target_body': target_body,
                                   'saved_character_effects': silence_xml}
    if 'SpellPowerTagSuppression' not in silence_xml:
        report['silence_observation'].update(
            native_effects=host['redact'](session.send('effect list remaining-traveller', read_seconds=.4)),
            body_effects=sql(f'SELECT HEX(EffectData) FROM Bodies WHERE Id={target_body}', (0,)),
            instance_effects=sql(f'SELECT Id,HEX(EffectData) FROM CharacterInstances WHERE CharacterId={target_actor}', (1,)))
    require('SpellPowerTagSuppression' in silence_xml, 'Paid Silence did not save its exact target child.')
    cast(routes['arm.spell.compelling_regard'], 'remaining-traveller')
    require('You compel the permitted action.' in session.send('compel remaining-traveller look', read_seconds=.4), 'Native exact IC order was not admitted.')
    require('cannot obey' in session.send('compel remaining-traveller l', read_seconds=.3), 'Native alias order was admitted.')
    assertions.append('S4 stock remaining: paid resisted Silence saved its bound child; paid Compelling Regard admitted exact native LOOK and refused its alias')
    session.send(f'goto {home}', read_seconds=.4); require(location() == home, 'Threshold fixture did not return to its source.')
    cast(routes['arm.spell.open_threshold'], 'traveller remaining-traveller')
    require('SpellOpenThreshold' in sql(f'SELECT HEX(EffectData) FROM Rooms WHERE Id={home}', (0,))[0][0], 'Paid threshold did not save its paired portal child.')
    session.send('enter threshold', read_seconds=3); require(location() == destination, 'Native threshold traversal did not move the exact body to its permitted destination.')
    assertions.append('S4 stock remaining: recognised destination opened one paid native threshold and actual ENTER traversed it')
    prototype = int(data['nilaz_gate']['item_prototype'])
    session.send(f'item load {host["item_command_reference"](prototype)}', read_seconds=.5); flush()
    held = sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bgi ON bgi.GameItemId=gi.Id WHERE gi.GameItemProtoId={prototype} AND bgi.BodyId={body} ORDER BY gi.Id DESC LIMIT 1')
    require(len(held) == 1, 'Native marked threshold lacks its exact carried leaf.')
    marked_item = int(held[0][0])
    require('you claim ownership' in session.send('ownership claim leaf', read_seconds=.5).lower(), 'Native leaf ownership was not established.')
    cast(routes['arm.spell.arcane_mark'], 'leaf'); flush()
    saved = ET.fromstring(sql(f'SELECT HEX(EffectData) FROM GameItems WHERE Id={marked_item}', (0,))[0][0])
    require(any(node.text == 'SpellMagicTag' for node in saved.iter('Type')), 'Paid native Arcane Mark was not saved.')
    session.send(f'goto {home}', read_seconds=.5); require(location() == home, 'Marked threshold source was not reached.')
    cast(routes['arm.spell.open_threshold'], 'mark leaf'); flush()
    saved = ET.fromstring(sql(f'SELECT HEX(EffectData) FROM GameItems WHERE Id={marked_item}', (0,))[0][0])
    require(not any(node.text == 'SpellMagicTag' for node in saved.iter('Type')), 'Threshold did not durably debit the final exact Arcane Mark use.')
    assertions.append('S4 stock remaining: actual MySQL marked threshold consumed its final saved Arcane Mark use exactly once')
    session.send(f'goto {destination}', read_seconds=.5); require(location() == destination, 'Hazard room was not regained after marked portal acceptance.')
    def wounds(): return float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={target_body}')[0][0])
    before_wounds = wounds(); cast(storm, 'here')
    lives = sql(f"SELECT HEX(Id),Version FROM MagicSpellLifecycles WHERE SpellId={int(storm['spell'])} AND CreatorId={actor} AND Family='room-electrical-hazard'")
    require(len(lives) == 1, 'Storm Locus did not persist one exact journal.')
    report['storm_origin'] = lives[0][0]
    samples = report.setdefault('storm_samples', [])
    deadline = time.monotonic() + 15
    while time.monotonic() < deadline:
        flush()
        cursor = sql(f'SELECT Version,RoomHazardLastPulseUtc,RoomHazardNextPulseUtc FROM MagicSpellLifecycles WHERE HEX(Id)="{report["storm_origin"]}"')[0]
        observed_wounds = wounds()
        samples.append({'cursor': cursor, 'wounds': observed_wounds,
                        'journal': sql(f'SELECT State,Diagnostic,CreatedUtc,DeadlineUtc,UpdatedUtc FROM MagicSpellLifecycles WHERE HEX(Id)="{report["storm_origin"]}"')})
        if cursor[1] != 'NULL' and observed_wounds > before_wounds: break
        time.sleep(.5)
    if cursor[1] == 'NULL' or wounds() <= before_wounds:
        report['storm_failure_context'] = {
            'room_effects': sql(f'SELECT HEX(EffectData) FROM Rooms WHERE Id={destination}', (0,)),
            'target': sql(f'SELECT Location,State,HEX(EffectData) FROM Characters WHERE Id={target_actor}', (2,)),
            'native_target_effects': host['redact'](session.send('effect list remaining-traveller', read_seconds=.3)),
            'native_save_queue': host['redact'](session.send('impdebug saves', read_seconds=.3))}
    require(cursor[1] != 'NULL' and cursor[2] != 'NULL', 'Real MySQL hazard did not commit its due pulse; see storm_samples.')
    require(wounds() > before_wounds, 'Committed native electrical pulse delivered no persisted wounds; see storm_samples.')
    report.update(storm_version=int(cursor[0]), storm_wounds=wounds()-before_wounds,
                  balances={key: balance(row) for key, row in routes.items()}, status='PASS')
    assertions.append('S4 stock remaining: real MySQL Storm Locus cursor committed before a native electrical wound pulse')
