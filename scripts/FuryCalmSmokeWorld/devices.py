"""N22 full-server charged devices, ordinary player eligibility, frozen potency and paid focus."""
from __future__ import annotations

import argparse
import hashlib
import importlib.util
import json
import math
import pathlib
import re
import socket
import subprocess
import sys
import time
import xml.etree.ElementTree as ET

sys.dont_write_bytecode = True
spec = importlib.util.spec_from_file_location('n22_base', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec); spec.loader.exec_module(s)
s.deadline = time.monotonic() + 1800
s.report.update(scope='N22 full-server charged wand/staff activation and explicit paid focus', milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/DeviceFixtures.cs',
             s.repo / 'MudSharpCore/bin/Debug/net10.0/FutureMUDLibrary.dll',
             s.repo / 'MudSharpCore/Magic/Casting/MagicCastingService.Devices.cs',
             s.repo / 'MudSharpCore/Commands/Modules/MagicModule.Devices.cs',
             s.repo / 'MudSharpCore/Commands/Modules/InventoryModule.Devices.cs',
             s.repo / 'MudSharpCore/GameItems/Components/ChargedMagicDeviceGameItemComponent.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
players = {}; fixture = None; mysql = None; devices = {}
base_command = s.command
base_sql = s.sql


def observed_sql(query):
    # Keep the maintained helper's owned-datadir validation and mutation semantics.
    result = base_sql(query)
    with (s.runtime / 'sql-observations.jsonl').open('a', encoding='utf-8') as evidence:
        evidence.write(json.dumps(dict(query_sha256=hashlib.sha256(query.encode()).hexdigest(),
            query_kind=query.split()[0], result_bytes=len(result.encode()), result_sha256=hashlib.sha256(result.encode()).hexdigest())) + '\n')
    return result


s.sql = observed_sql


def scalar(query, purpose):
    # A missing CLI result is unavailable evidence, never a zero. Only a read may retry.
    assert query.startswith('SELECT ')
    answer = s.sql(query)
    if not answer:
        s.report.setdefault('scalarReadRetries', []).append(dict(purpose=purpose,
            query_sha256=hashlib.sha256(query.encode()).hexdigest(), first_result='EMPTY'))
        answer = s.sql(query)
    s.check('required ' + purpose + ' scalar observation is present', bool(answer))
    return float(answer)


def command(role, text, expected=None, seconds=.3):
    previous = s.session
    if role != 'admin': s.session = players[role]
    try:
        answer = base_command(text, seconds=seconds)
        limit = time.monotonic() + 8
        while expected and not re.search(expected, answer, re.I | re.S) and time.monotonic() < limit:
            answer += s.session.read_for(.25)
        if expected: s.check(role + ': ' + text, bool(re.search(expected, answer, re.I | re.S)), answer[-2200:])
        return answer
    finally: s.session = previous


def flush(): command('admin', 'impdebug flush', 'queued saves.*flushed', seconds=.5)


def rows(query):
    answer = s.sql(query)
    if not answer:
        count = scalar('SELECT COUNT(*) FROM (' + query + ') n22_required_rows', 'empty-rowset count')
        if count:
            s.report.setdefault('rowsetReadRetries', []).append(dict(
                query_sha256=hashlib.sha256(query.encode()).hexdigest(), first_result='EMPTY', expected_rows=count))
            answer = s.sql(query)
            s.check('required nonempty rowset observation is present', bool(answer))
    return [json.loads(line) for line in answer.splitlines()] if answer else []


def character(role): return fixture['Players'][role]['Character']


def state(role):
    flush(); identifier = character(role)
    result = {}
    for name, table, columns, order in [
        ('traits', 'CharacterTraits', 'TraitDefinitionId,Value,AdditionalValue', 'TraitDefinitionId'),
        ('acquired', 'CharacterAcquiredSpells', 'MagicSpellId,AcquiredUtc,Provenance,ControlledGrade,ProfileVersion,NextMasteryUtc,StateVersion', 'MagicSpellId'),
        ('opportunities', 'CharacterMagicSkillOpportunities', 'TraitDefinitionId,NextOpportunityUtc,StateVersion', 'TraitDefinitionId'),
        ('enrolments', 'CharacterCastingEnrolments', 'CapabilityIdentity,MagicCapabilityId,EnrolledUtc,CompletedStartingGrantVersion', 'MagicCapabilityId'),
        ('reserves', 'Characters_MagicResources', 'MagicResourceId,Amount', 'MagicResourceId')]:
        result[name] = rows(f'SELECT JSON_ARRAY({columns}) FROM {table} WHERE CharacterId={identifier} ORDER BY {order}')
    return result


def reserve(role):
    flush(); return scalar(f"SELECT Amount FROM Characters_MagicResources WHERE CharacterId={character(role)} AND MagicResourceId={fixture['Reserve']}", role + ' reserve')


def bank(role):
    flush(); item = devices[role]
    result = rows(f"SELECT JSON_ARRAY(c.Id,c.GameItemComponentProtoId,c.GameItemComponentProtoRevision,c.Definition) FROM GameItemComponents c JOIN GameItemComponentProtos p ON p.Id=c.GameItemComponentProtoId AND p.RevisionNumber=c.GameItemComponentProtoRevision WHERE c.GameItemId={item} AND p.Type='ChargedMagicDevice'")
    assert len(result) == 1
    component, prototype, revision, definition = result[0]; root = ET.fromstring(definition)
    snapshot = root.find('StoredSpell')
    if snapshot is not None: snapshot.tail = None
    return dict(item=item, component=component, prototype=prototype, revision=revision, definition=definition,
                charges=[x.text.lower() for x in root.findall('Charge')],
                snapshot=ET.tostring(snapshot, encoding='unicode') if snapshot is not None else None)


def keyword(role): return role if role in ('wand', 'staff') else 'qan22device'


def start(prepare=False):
    title = s.sql('SELECT o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id=1')
    with socket.socket() as listener:
        listener.bind(('127.0.0.1', 0)); port = listener.getsockname()[1]
    (s.runtime / 'Connection.config').write_text(f'127.0.0.1\n{port}\n\n', encoding='utf-8')
    s.process = s.helper.launch_mud(s.repo, s.runtime, argparse.Namespace(provider='MySql.Data.MySqlClient', startup_timeout=180), s.connection, [s.connection, s.password])
    s.session = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password]); s.session.read_for(.5)
    command('admin', 'l'); command('admin', 'Admin'); s.session.send(s.password, secret=True, read_seconds=.5)
    command('admin', 'c'); command('admin', '1', re.escape(title), seconds=2)
    command('admin', 'immortal', 'already immortal|become immortal'); command('admin', 'impdebug freezetime', 'freeze all')
    command('admin', f"goto {fixture['Room']}", 'QA N22|already')
    for role in ('caster', 'reader'):
        p = fixture['Players'][role]; players[role] = s.helper.MudSocket('127.0.0.1', port, [s.connection, s.password])
        players[role].read_for(.5); command(role, 'l'); command(role, p['AccountName'])
        player_title = s.sql(f"SELECT o.RoomName FROM Characters c JOIN Rooms r ON r.Id=c.Location JOIN RoomOverlays o ON o.Id=r.CurrentOverlayId WHERE c.Id={p['Character']}")
        players[role].send(s.password, secret=True, read_seconds=.5); command(role, 'c'); command(role, '1', re.escape(player_title), seconds=2)
        if prepare:
            command(role, 'drop all')
            command('admin', 'transfer ' + p['Keyword'], 'You transfer|already in the same location')
            command(role, 'look', 'QA N22')
        effects = ET.fromstring(s.sql(f"SELECT EffectData FROM Characters WHERE Id={p['Character']}"))
        s.check(role + ' has a real player session without admin sight', not any(x.findtext('Type') == 'AdminSight' for x in effects.iter('Effect')))


def stop():
    flush()
    for role, player in players.items():
        s.transcripts.append(role + '\n' + player.full_transcript()); player.close()
    players.clear(); s.stop()


def load(prototype, recipient):
    command('admin', f'item load {prototype}', 'load|create'); flush()
    item = int(s.sql(f'SELECT Id FROM GameItems WHERE GameItemProtoId={prototype} ORDER BY Id DESC LIMIT 1'))
    selector = 'qan22material' if prototype == fixture['Material'] else 'qan22device' if prototype == fixture['PrivateDevice'] else 'wand' if prototype == fixture['Stock']['wand'] else 'staff'
    command('admin', f"give {selector} {fixture['Players'][recipient]['Keyword']}", 'You give')
    flush()
    s.check('new item enters exact player custody', s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') == str(fixture['Players'][recipient]['Body']))
    return item


def transfer(device, source, recipient):
    before = bank(device)
    command(source, f"give {keyword(device)} {fixture['Players'][recipient]['Keyword']}", 'You give')
    s.check('transfer retains exact charge bank and changes actual body custody', bank(device) == before and
            s.sql(f"SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={devices[device]}") == str(fixture['Players'][recipient]['Body']))


def produce(device, count):
    before = reserve('caster'); before_state = state('caster'); old = bank(device)
    command('caster', f"magicdevice charge {keyword(device)} {fixture['Capability']} {fixture['Mend']} 3 {count}", 'paid|producing|production|begun', seconds=.8)
    s.check('initial production pays exactly fifty reserve per grade-three charge', reserve('caster') == before - 50 * count)
    end = time.monotonic() + count * 60 + 30
    while time.monotonic() < end:
        current = bank(device)
        if len(current['charges']) == count: break
        players['caster'].read_for(1)
    s.check('paid production fills exactly the requested homogeneous bank', len(current['charges']) == count and not old['charges'] and len(set(current['charges'])) == count)
    after_state = state('caster'); before_state['reserves'] = after_state['reserves']
    s.check('production grants no acquisition or proficiency/mastery opportunity', before_state == after_state)
    snapshot = ET.fromstring(current['snapshot'])
    s.check('payload records actual producer and source spell', int(snapshot.attrib['creator']) == character('caster') and int(snapshot.attrib['spell']) == fixture['Mend'])
    s.report.setdefault('production', []).append(dict(device=device, beforeReserve=before, afterReserve=reserve('caster'), bank=current))


def wound(role):
    target = fixture['Players'][role]['Keyword']
    # HumanSeeder's rbuttock has no organ coverage; Burning does not fracture its covered bone.
    # Native armour/body processing varies the actual wound budget. Only fixture preparation
    # may retry; never repeat an activation or payment to obtain a passing result.
    preparations = []
    for requested in (60, 80):
        command('admin', f'cure {target} all', 'remove all.*wounds')
        s.check('native cure clears the previous complete wound budget', damage(role) == 0)
        command('admin', f'wound {target} rbuttock burning {requested} 0 0', 'causing the following wounds')
        flush()
        observed = damage(role)
        wounds = rows(f"SELECT JSON_ARRAY(w.Id,w.CurrentDamage,p.Name) FROM Wounds w LEFT JOIN BodypartProto p ON p.Id=w.BodypartProtoId WHERE w.BodyId={fixture['Players'][role]['Body']}")
        preparations.append(dict(requested=requested, observed=observed, wounds=wounds))
        s.check('native wound preparation affects only the exact external body part',
                len(wounds) == 1 and wounds[0][2] == 'rbuttock' and
                math.isclose(observed, wounds[0][1], rel_tol=0, abs_tol=1e-7), str(wounds))
        if observed >= 18: break
    s.report.setdefault('woundBaselines', []).append(dict(role=role, requested=requested,
        observed=observed, wounds=wounds, preparations=preparations))
    s.check('native wound supplies one exact-body external wound with sufficient healing budget',
            len(wounds) == 1 and wounds[0][2] == 'rbuttock' and observed >= 18, str(wounds))


def damage(role):
    flush(); return scalar(f"SELECT COALESCE(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={fixture['Players'][role]['Body']}", role + ' wound total')


def activate(device, role, refusal=None):
    wound(role); before_damage = damage(role); before = bank(device); states = {x: state(x) for x in ('caster', 'reader')}
    answer = command(role, f'magicdevice charged {keyword(device)} self', refusal or 'Exactly one device charge was spent', seconds=.7)
    after = bank(device); healed = before_damage - damage(role)
    s.check('charged activation preserves both players reserves and full progression rows', states == {x: state(x) for x in states})
    if refusal:
        s.check('refused activation keeps the exact bank and wounds intact', before == after and healed == 0, answer)
        if before['charges']: s.check('refusal creates no charge receipt', s.sql(f"SELECT COUNT(*) FROM MagicCastingOperations WHERE Id='{before['charges'][0]}'") == '0')
    else:
        s.check('successful release heals exactly the frozen grade-three budget', math.isclose(healed, 18, rel_tol=0, abs_tol=1e-7), str(healed))
        s.check('successful release consumes exactly its first captured charge GUID', after['charges'] == before['charges'][1:] and after['snapshot'] == before['snapshot'])
        charge = before['charges'][0]
        journal = rows(f"SELECT JSON_ARRAY(Stage,CharacterId,ActorId,BodyId,ReserveId,Definition,Diagnostic) FROM MagicCastingOperations WHERE Id='{charge}'")
        s.check('exact charge has one completed native receipt with actual recipient identity', len(journal) == 1 and journal[0][:5] ==
                ['Completed', character(role), fixture['Players'][role]['Instance'], fixture['Players'][role]['Body'], 0] and journal[0][6] == 'Succeeded', str(journal))
        payload = ET.fromstring(journal[0][5])
        s.check('receipt links the exact item and charge through known schema fields', int(payload.find('Item').attrib['id']) == devices[device] and payload.attrib['charge'].lower() == charge)
        s.report.setdefault('releases', []).append(dict(device=device, actor=character(role), charge=charge, healed=healed, journal=journal))


def revise(settings):
    command('admin', f"component edit {fixture['PrivateComponent']}", 'open.*editing')
    for setting in settings: command('admin', 'component set ' + setting, 'configuration updated')
    command('admin', 'component edit submit N22 owned policy', 'submit')
    command('admin', f"component review {fixture['PrivateComponent']}", 'reviewing')
    command('admin', 'accept edit N22 owned policy', 'approve|current|accept')
    command('admin', 'component update', 'Are you sure'); command('admin', 'accept components', 'maintenance|begin')
    end = time.monotonic() + 30
    revision = int(s.sql(f"SELECT MAX(RevisionNumber) FROM GameItemComponentProtos WHERE Id={fixture['PrivateComponent']}"))
    while time.monotonic() < end:
        if bank('private')['revision'] == revision: break
        s.session.read_for(.5)
    s.check('native revision dispatch updates only the private policy while retaining charges', bank('private')['revision'] == revision)
    s.report.setdefault('privatePolicyRevisions', []).append(dict(settings=settings, revision=revision, bank=bank('private')))


def focus(refusal=None, material=None):
    wound('caster'); before = reserve('caster'); health = damage('caster'); old = bank('private')
    answer = command('caster', f"magicdevice focus qan22device {fixture['Capability']} {fixture['Focus']} 1 self", refusal or 'paid casting succeeded', seconds=.8)
    s.check('explicit focus does not refill or spend the depleted charge bank', bank('private') == old)
    if refusal: s.check('focus refusal pays nothing and heals nothing', reserve('caster') == before and damage('caster') == health, answer)
    else:
        s.check('explicit focus follows ordinary grade-one payment and healing', reserve('caster') == before - 50 and math.isclose(health - damage('caster'), 2, rel_tol=0, abs_tol=1e-7))
        s.check('ordinary focus consumes the exact selected carried material', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={material}') == '0')
    s.report.setdefault('focusAttempts', []).append(dict(refused=bool(refusal), material=material,
        beforeReserve=before, afterReserve=reserve('caster'), healed=health-damage('caster'), bank=bank('private')))


def run():
    global mysql, fixture
    try:
        binary = pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'); data = pathlib.Path(s.instance['data']).resolve()
        assert data == (s.root / 'mysql/data').resolve()
        with socket.socket() as probe: assert probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0, 'Owned port is occupied'
        log = (s.runtime / 'mysql-process.log').open('w', encoding='utf-8')
        mysql = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent), '--datadir=' + str(data), '--port=' + str(s.instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4', '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M', '--log-error=' + str(s.root / 'mysql/mysql.err')], stdout=log, stderr=log, creationflags=subprocess.CREATE_NO_WINDOW)
        end = time.monotonic() + 60
        while time.monotonic() < end:
            try: s.sql('SELECT 1'); break
            except AssertionError: time.sleep(.25)
        fixture = s.fixture('device-fixtures'); s.report['objects'] = fixture
        stock_definitions = rows("SELECT JSON_ARRAY(p.Id,p.RevisionNumber,p.Definition) FROM GameItemComponentProtos p WHERE p.Id IN (" +
                                 str(fixture['StockComponent']) + "," + s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.component.charged_staff'") + ") ORDER BY p.Id,p.RevisionNumber")
        start(prepare=True)
        for role in ('caster', 'reader'):
            command('admin', 'cure ' + fixture['Players'][role]['Keyword'] + ' all', 'remove all.*wounds')
            s.check('ordinary fixture starts with no retained failed-attempt wounds', damage(role) == 0)
            wound(role)
            command('admin', 'cure ' + fixture['Players'][role]['Keyword'] + ' all', 'remove all.*wounds')
        command('admin', f"givemerit qan22caster {fixture['Merit']}", 'grant|already')
        command('admin', f"magic capability edit {fixture['Capability']}", 'edit')
        for spell in (fixture['Mend'], fixture['Focus']):
            command('admin', f'magic capability set casting entry add {spell}', 'updated|already|admitted')
            command('admin', f"magic capability set casting entry trait {spell} {fixture['Trait']}", 'updated')
            command('admin', f'magic capability set casting entry skill {spell} 30 60 relative', 'updated')
        command('admin', 'magic capability close')
        command('admin', f"magic casting enrol {character('caster')} {fixture['Capability']} N22 deterministic setup", 'Enrolled|already')
        for spell in (fixture['Mend'], fixture['Focus']): command('admin', f"magic casting grant {character('caster')} {fixture['Capability']} {spell} N22 deterministic setup", 'grant|acquir|already')
        stop()
        s.sql(f"UPDATE CharacterTraits SET Value=60 WHERE CharacterId={character('caster')} AND TraitDefinitionId={fixture['Trait']}")
        s.sql(f"UPDATE CharacterAcquiredSpells SET ControlledGrade=3 WHERE CharacterId={character('caster')} AND MagicSpellId IN ({fixture['Mend']},{fixture['Focus']})")
        for role in ('caster', 'reader'): s.sql(f"INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES ({character(role)},{fixture['Reserve']},1000) ON DUPLICATE KEY UPDATE Amount=1000")
        start()
        for device, prototype, count in [('wand', fixture['Stock']['wand'], 2), ('staff', fixture['Stock']['staff'], 1), ('private', fixture['PrivateDevice'], 2)]:
            devices[device] = load(prototype, 'caster'); produce(device, count)
            if device == 'wand': activate(device, 'caster')
            transfer(device, 'caster', 'reader'); activate(device, 'reader', 'currently entitled caster')
            if device != 'private': transfer(device, 'reader', 'caster')
            if device == 'wand': command('caster', 'drop wand', 'You drop')
            if device == 'staff': command('caster', 'drop staff', 'You drop')
        # Reload the stock carriers into the caster's hands using actual native custody.
        command('caster', 'get wand', 'You get'); command('caster', 'get staff', 'You get')
        before_revision = bank('private')
        revise(['eligibility anyone', 'mingrade 3', f"spell add {fixture['Focus']}"])
        s.check('builder revision preserves the original prepaid payload and GUID order', bank('private')['charges'] == before_revision['charges'] and bank('private')['snapshot'] == before_revision['snapshot'])
        activate('private', 'reader', 'additional currently acquired controlled grade')
        revise(['mingrade 0'])
        frozen = {x: bank(x) for x in devices}; balances = {x: reserve(x) for x in players}
        stop()
        s.sql(f"UPDATE CharacterTraits SET Value=30 WHERE CharacterId={character('caster')} AND TraitDefinitionId={fixture['Trait']}")
        s.sql(f"UPDATE CharacterAcquiredSpells SET ControlledGrade=1 WHERE CharacterId={character('caster')} AND MagicSpellId IN ({fixture['Mend']},{fixture['Focus']})")
        start()
        s.check('cold restart retains exact transferred banks and deducted reserves', frozen == {x: bank(x) for x in devices} and balances == {x: reserve(x) for x in players})
        s.check('producer proficiency and controlled grade really changed', s.sql(f"SELECT Value FROM CharacterTraits WHERE CharacterId={character('caster')} AND TraitDefinitionId={fixture['Trait']}") == '30' and
                s.sql(f"SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={character('caster')} AND MagicSpellId={fixture['Mend']}") == '1')
        for device, role in [('wand', 'caster'), ('staff', 'caster'), ('private', 'reader'), ('private', 'reader')]: activate(device, role)
        for device, role in [('wand', 'caster'), ('staff', 'caster'), ('private', 'reader')]: activate(device, role, 'Choose focus mode explicitly')
        before = state('reader'); old = bank('private')
        command('reader', f"magicdevice focus qan22device {fixture['Capability']} {fixture['Focus']} 1 self", 'capability|acquir|entitl')
        s.check('Anyone charged eligibility grants no personal focus route or progression', state('reader') == before and bank('private') == old)
        command('caster', 'drop wand', 'You drop'); command('caster', 'drop staff', 'You drop')
        transfer('private', 'reader', 'caster'); focus('material|inventory|plan|item')
        material = load(fixture['Material'], 'caster'); command('caster', 'drop qan22material', 'You drop'); focus('material|inventory|plan|item')
        s.check('ground focus material survives refused payment', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={material}') == '1')
        command('caster', 'get qan22material', 'You get'); focus('functioning free hand', material=material)
        s.check('held material survives the ordinary free-hand refusal', s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={material}') == '1')
        command('caster', 'wear qan22material'); flush()
        material_custody = rows(f'SELECT JSON_ARRAY(BodyId,WearProfile,Wielded) FROM Bodies_GameItems WHERE GameItemId={material}')
        s.check('native wear keeps exact material body-owned and worn with the focus hand free',
                len(material_custody) == 1 and material_custody[0][0] == fixture['Players']['caster']['Body'] and
                material_custody[0][1] is not None and material_custody[0][2] is None and
                s.sql(f'SELECT COUNT(*) FROM GameItems WHERE Id={material} AND ContainerId IS NULL') == '1', str(material_custody))
        s.report['materialCustody'] = dict(item=material, body=fixture['Players']['caster']['Body'], worn=material_custody)
        focus(material=material)
        final_banks = {x: bank(x) for x in devices}; final_reserves = {x: reserve(x) for x in players}
        s.report.update(finalBanks=final_banks, finalReserves=final_reserves)
        stop(); start()
        s.check('final cold load proves no charge recreation or duplicate personal debit', final_banks == {x: bank(x) for x in devices} and final_reserves == {x: reserve(x) for x in players})
        s.check('stock wand/staff component revisions and definitions stayed unchanged', stock_definitions == rows("SELECT JSON_ARRAY(p.Id,p.RevisionNumber,p.Definition) FROM GameItemComponentProtos p WHERE p.Id IN (" + str(fixture['StockComponent']) + "," + s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.component.charged_staff'") + ") ORDER BY p.Id,p.RevisionNumber"))
        stop()
        s.check('all captured native inputs remained unchanged', all(hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error: s.report.update(status='FAIL', error=s.helper.redact(str(error), [s.connection, s.password]))
    finally:
        for role, player in players.items(): s.transcripts.append(role + '\n' + player.full_transcript()); player.close()
        try: s.finish()
        finally:
            if fixture:
                try:
                    original = fixture['CapabilityDefinition']
                    s.sql(f"UPDATE MagicCapabilities SET Definition=CONVERT(UNHEX('{original.encode('utf-8').hex()}') USING utf8mb4) WHERE Id={fixture['Capability']}")
                    s.check('owned fixture restores the exact original capability definition after server shutdown',
                            s.sql(f"SELECT HEX(Definition) FROM MagicCapabilities WHERE Id={fixture['Capability']}") == original.encode('utf-8').hex().upper())
                    s.report['capabilityDefinitionRestored'] = True
                except BaseException as error:
                    diagnostic = s.helper.redact(str(error), [s.connection, s.password])
                    s.report.setdefault('cleanupErrors', []).append(diagnostic)
                    s.report.setdefault('error', diagnostic)
                    s.report.update(status='FAIL', milestoneQualified=False)
            if mysql:
                try: s.sql('SHUTDOWN')
                except AssertionError: pass
                s.helper.stop_process_tree(mysql); s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
                with socket.socket() as probe: s.report['mysqlPortClosed'] = probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0
                if not s.report['mysqlPortClosed']: s.report.update(status='FAIL', milestoneQualified=False, error='Owned MySQL port remained open')
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')
    return 0 if s.report['status'] == 'PASS' else 1


if __name__ == '__main__': raise SystemExit(run())
