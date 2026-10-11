"""N23 ordinary-player recharge, homogeneous potency, payments and interrupted-work restart."""
from __future__ import annotations

import hashlib
import importlib.util
import json
import math
import os
import pathlib
import re
import socket
import subprocess
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('n23_devices', pathlib.Path(__file__).with_name('devices.py'))
d = importlib.util.module_from_spec(spec); spec.loader.exec_module(d)
s = d.s
s.deadline = time.monotonic() + 2700
s.report.update(scope=__doc__, milestoneQualified=False)
for path in [pathlib.Path(__file__), s.repo / 'scripts/FuryCalmSmokeWorld/RechargeFixtures.cs',
             s.repo / 'MudSharpCore/Magic/Vancian/StoredSpellSnapshot.Devices.cs',
             s.repo / 'MudSharpCore Unit Tests/ChargedMagicDeviceRechargeTests.cs']:
    s.report['inputs'][str(path.relative_to(s.repo))] = hashlib.sha256(path.read_bytes()).hexdigest()
base_keyword = d.keyword
d.keyword = lambda device: 'qan23interrupt' if device == 'interrupt' else base_keyword(device)
mysql = None
base_rows = d.rows


def guarded_rows(query):
    assert query.lstrip().upper().startswith('SELECT '), 'Only read-only JSON observations may retry'
    for attempt in range(3):
        try:
            return base_rows(query)
        except json.JSONDecodeError as error:
            records = s.report.setdefault('malformedRowsetReads', [])
            artifact = s.runtime / f'malformed-row-{len(records) + 1}.txt'
            artifact.write_text(error.doc, encoding='utf-8')
            records.append(dict(query_sha256=hashlib.sha256(query.encode()).hexdigest(), attempt=attempt + 1,
                error=str(error), row_sha256=hashlib.sha256(error.doc.encode()).hexdigest(),
                row_bytes=len(error.doc.encode()), artifact=artifact.name))
            if attempt == 2:
                raise


d.rows = guarded_rows


def bank(device):
    value = d.bank(device); root = ET.fromstring(value['definition'])
    return {**value, 'reservation': root.get('reservation'),
            'producer': {k: root.get(k) for k in ('capability', 'trait', 'raw')}}


def journals():
    d.flush()
    return d.rows('SELECT JSON_ARRAY(Id,Stage,CharacterId,ActorId,BodyId,MagicCapabilityId,MagicSpellId,ReserveId,Definition,Diagnostic) '
                  f"FROM MagicCastingOperations WHERE CharacterId IN ({d.character('caster')},{d.character('reader')}) ORDER BY Id")


def observe(device):
    return dict(bank=bank(device), states={role: d.state(role) for role in ('caster', 'reader')},
                damage={role: d.damage(role) for role in ('caster', 'reader')}, journals=journals())


def refuse(label, device, role, capability, spell, grade=3, count=1, expected='cannot'):
    before = observe(device)
    answer = d.command(role, f'magicdevice charge {d.keyword(device)} {capability} {spell} {grade} {count}', expected, seconds=.8)
    after = observe(device)
    s.check(label + ': exact bank, both players, wounds and operation rows remain unchanged', before == after, answer[-1800:])
    s.report.setdefault('refusals', []).append(dict(case=label, device=device, actor=d.character(role),
        capability=capability, spell=spell, grade=grade, count=count, before=before, after=after))


def begin(device, role, capability, count=1):
    before = observe(device)
    d.command(role, f"magicdevice charge {d.keyword(device)} {capability} {d.fixture['Mend']} 3 {count}",
              'Production costs are paid', seconds=.8)
    current = journals(); prior = {x[0] for x in before['journals']}
    added = [x for x in current if x[0] not in prior]
    s.check('one exact production operation identifies actual producer body and route', len(added) == 1 and
            added[0][2:8] == [d.character(role), d.fixture['Players'][role]['Instance'], d.fixture['Players'][role]['Body'],
                             capability, d.fixture['Mend'], d.fixture['Reserve']], str(added))
    row = added[0]; payload = ET.fromstring(row[8])
    s.check('production receipt links exact item, count and per-charge reserve debit',
            payload.get('kind') == 'DeviceProduction' and int(payload.get('count')) == count and
            int(payload.find('Item').get('id')) == d.devices[device] and
            [(int(x.get('holder')), int(x.get('resource')), float(x.get('amount'))) for x in payload.findall('Cost')] ==
            [(d.character(role), d.fixture['Reserve'], 50 * count)])
    s.check('production batch pays exactly once before completion', d.reserve(role) ==
            next(x[1] for x in before['states'][role]['reserves'] if x[0] == d.fixture['Reserve']) - 50 * count)
    return before, row[0]


def produce(device, role, capability, count=1, materials=()):
    before, operation = begin(device, role, capability, count)
    if materials:
        s.check('material payment is observed before timed work completes',
                next(x for x in journals() if x[0] == operation)[1] == 'DeviceProducing')
        for material in materials:
            s.check('per-charge material is already consumed during paid unfinished work',
                    d.scalar(f'SELECT COUNT(*) FROM GameItems WHERE Id={material}', 'paid material survivor count') == 0)
        s.check('material debit precedes charge fill', next(x for x in journals() if x[0] == operation)[1] == 'DeviceProducing' and
                bank(device)['charges'] == before['bank']['charges'])
    end = min(s.deadline, time.monotonic() + 60 * count + 30)
    while time.monotonic() < end:
        current = bank(device)
        if current['reservation'] is None and len(current['charges']) == len(before['bank']['charges']) + count: break
        d.players[role].read_for(1)
    old = before['bank']; prefix = current['charges'][:len(old['charges'])]
    added = current['charges'][len(old['charges']):]
    s.check('successful recharge preserves old GUIDs and appends exactly the unique requested charges',
            prefix == old['charges'] and len(added) == count and len(set(current['charges'])) == len(current['charges']))
    if old['charges']:
        s.check('recharge preserves the original payload, provenance and producer threshold',
                current['snapshot'] == old['snapshot'] and current['producer'] == old['producer'])
    after = observe(device)
    expected_states = json.loads(json.dumps(before['states']))
    for value in expected_states[role]['reserves']:
        if value[0] == d.fixture['Reserve']: value[1] -= 50 * count
    s.check('production changes only its paid reserve; neither player gains knowledge, traits or mastery',
            expected_states == after['states'] and before['damage'] == after['damage'])
    row = next(x for x in after['journals'] if x[0] == operation)
    s.check('production has one completed receipt and no retained reservation', row[1] == 'Completed' and current['reservation'] is None)
    for material in materials:
        s.check('per-charge material is consumed exactly once', d.scalar(f'SELECT COUNT(*) FROM GameItems WHERE Id={material}', 'material survivor count') == 0)
    s.report.setdefault('production', []).append(dict(device=device, actor=d.character(role), capability=capability,
        count=count, operation=operation, paid=50 * count, newGuids=added, materials=list(materials), before=before, after=after))


def material():
    item = d.load(d.fixture['Material'], 'caster')
    d.command('caster', 'drop qan22material', 'You drop')
    d.flush()
    s.check('private production supply is a real nearby item',
            d.scalar(f"SELECT COUNT(*) FROM GameItems i JOIN Rooms_GameItems r ON r.GameItemId=i.Id WHERE i.Id={item} AND r.RoomId={d.fixture['Room']} AND i.ContainerId IS NULL", 'ground supply count') == 1)
    return item


def load_interrupt():
    prototype = d.fixture['InterruptDevice']
    d.command('admin', f'item load {prototype}', 'load|create'); d.flush()
    item = int(d.scalar(f'SELECT Id FROM GameItems WHERE GameItemProtoId={prototype} ORDER BY Id DESC LIMIT 1', 'new interruption item'))
    d.command('admin', "give qan23interrupt qan22caster", 'You give'); d.flush()
    s.check('interruption item enters exact producer body custody',
            s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') == str(d.fixture['Players']['caster']['Body']))
    d.devices['interrupt'] = item


def native_cap(role):
    details = s.report['nativeSkillCap']
    answer = d.command('admin', f"te test {details['expression']} {d.fixture['Players'][role]['Keyword']}", 'Result:')
    result = re.search(r'Result:\s*([0-9]+(?:\.[0-9]+)?)', answer)
    assert result, 'Native cap evaluation must return a present numeric result'
    value = float(result.group(1))
    s.check('native skill cap permits the bounded raw inputs for ' + role, math.isfinite(value) and value >= 49)
    details.setdefault('evaluations', []).append(dict(role=role, character=d.character(role), cap=value, output=answer))
    return value


def verify_prior_failed_fixture():
    source = os.environ.get('N23_FAILED_FIXTURE_RECEIPT')
    if not source: return
    path = pathlib.Path(source).resolve()
    assert path.is_relative_to(s.root.resolve()), 'Failed fixture receipt must belong to this owned world'
    prior = json.loads(path.read_text()); f = prior['objects']; old = prior['production'][0]['after']['bank']
    assert prior['status'] == 'FAIL' and not prior['milestoneQualified'] and old['producer']['raw'] == '50'
    assert prior['error'].startswith(f"reader: magicdevice charge wand {f['AlternateCapability']} {f['Mend']} 3 1:")
    candidates = [x for x in journals() if x[2] == f['Players']['reader']['Character'] and
                  x[5:7] == [f['AlternateCapability'], f['Mend']]]
    assert len(candidates) == 1, 'Only the exact prior failed fixture operation may be verified'
    row = candidates[0]; payload = ET.fromstring(row[8])
    assert payload.get('kind') == 'DeviceProduction' and payload.find('Item').get('id') == str(old['item'])
    query = f"SELECT JSON_ARRAY(Definition) FROM GameItemComponents WHERE Id={old['component']} AND GameItemId={old['item']}"
    definitions = d.rows(query); assert len(definitions) == 1
    bank_before = definitions[0][0]; reserve_before = d.reserve('reader')
    root = ET.fromstring(bank_before); snapshot = root.find('StoredSpell'); snapshot.tail = None
    expected_charges = [x for x in old['charges'] if x not in {y['charge'] for y in prior['releases']}]
    s.check('prior failed fixture retains its exact old payload, remaining GUIDs and paid debit',
            [x.text for x in root.findall('Charge')] == expected_charges and ET.tostring(snapshot, encoding='unicode') == old['snapshot'] and
            reserve_before == next(x[1] for x in prior['production'][0]['before']['states']['reader']['reserves'] if x[0] == f['Reserve']) - 50)
    if row[1] == 'Completed':
        s.check('prior failed work was natively cancelled during disconnect without charges or refund',
                root.get('reservation') is None and 'no charges or refund' in row[9])
        disposition = 'Already terminal native cancellation; no staff acknowledgement needed'
    else:
        s.check('prior failed item reservation identifies that exact incomplete operation',
                row[1] == 'DeviceProducing' and root.get('reservation') == row[0])
        d.command('admin', f"magic casting resolve {row[2]} {row[0]} N23 failed cap-clamped fixture acknowledged; retain payment and old bank", 'Reconciled without replay, refund')
        disposition = 'Exact incomplete operation acknowledged without changing its retained bank or payment'
    after = next(x for x in journals() if x[0] == row[0])
    s.check('prior failed fixture disposition preserves its exact bank and paid balance',
            after[1] == ('Completed' if row[1] == 'Completed' else 'Reconciled') and after[8] == row[8] and d.rows(query) == definitions and d.reserve('reader') == reserve_before)
    s.report['priorFailedFixtureDisposition'] = dict(receipt=str(path.relative_to(s.repo)), disposition=disposition,
        receiptSha256=hashlib.sha256(path.read_bytes()).hexdigest(), operation=row[0], before=row, after=after,
        reserveBefore=reserve_before, reserveAfter=d.reserve('reader'), bankDefinition=bank_before)


def crash(operation):
    d.flush(); current = bank('interrupt'); row = next(x for x in journals() if x[0] == operation)
    s.check('forced-stop prerequisite is paid incomplete reserved work',
            current['reservation'] == operation and row[1] == 'DeviceProducing')
    index = len(s.collectors) - 1
    s.helper.stop_process_tree(s.process); s.process.wait(timeout=30)
    s.check('intentional forced stop terminates only the owned MUD handle', s.process.returncode is not None and s.process.returncode != 0)
    s.report['expectedForcedStops'] = [dict(index=index, operation=operation, returncode=s.process.returncode,
        reason='N23 crash during paid recharge before completion; MySQL remains owned and running')]
    for role, player in d.players.items(): s.transcripts.append(role + '\n' + player.full_transcript()); player.close()
    d.players.clear(); s.transcripts.append(s.session.full_transcript()); s.session.close()
    s.process = s.session = None


def run():
    global mysql
    try:
        binary = pathlib.Path(r'C:\Program Files\MySQL\MySQL Server 8.0\bin'); data = pathlib.Path(s.instance['data']).resolve()
        assert data == (s.root / 'mysql/data').resolve()
        with socket.socket() as probe: assert probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0, 'Owned port is occupied'
        log = (s.runtime / 'mysql-process.log').open('w', encoding='utf-8')
        mysql = subprocess.Popen([str(binary / 'mysqld.exe'), '--no-defaults', '--basedir=' + str(binary.parent), '--datadir=' + str(data),
            '--port=' + str(s.instance['port']), '--bind-address=127.0.0.1', '--mysqlx=0', '--character-set-server=utf8mb4',
            '--collation-server=utf8mb4_unicode_ci', '--max-allowed-packet=64M', '--log-error=' + str(s.root / 'mysql/mysql.err')],
            stdout=log, stderr=log, creationflags=subprocess.CREATE_NO_WINDOW)
        end = time.monotonic() + 60
        while time.monotonic() < end:
            try: s.sql('SELECT 1'); break
            except AssertionError: time.sleep(.25)
        d.fixture = s.fixture('recharge-fixtures'); s.report['objects'] = d.fixture
        f = d.fixture; caster = d.character('caster'); reader = d.character('reader')
        cap = d.rows(f"SELECT JSON_ARRAY(t.ExpressionId,e.Expression) FROM TraitDefinitions t JOIN TraitExpression e ON e.Id=t.ExpressionId WHERE t.Id={f['Trait']}")
        s.check('native skill cap follows its exact trait-expression foreign key', len(cap) == 1)
        s.report['nativeSkillCap'] = dict(trait=f['Trait'], expression=cap[0][0], formula=cap[0][1],
            original=45, weaker=44, stronger=49, reason='Native Skill.Value is capped; stored database values alone do not prove runtime raw proficiency')
        s.report['readerAcquisitionBeforeNegativeSetup'] = d.rows(f'SELECT JSON_ARRAY(MagicSpellId,ControlledGrade,Provenance,StateVersion) FROM CharacterAcquiredSpells WHERE CharacterId={reader} AND MagicSpellId={f["Mend"]}')
        s.sql(f'DELETE FROM CharacterAcquiredSpells WHERE CharacterId={reader} AND MagicSpellId={f["Mend"]}')
        stock_query = f"SELECT JSON_ARRAY(Id,RevisionNumber,Definition) FROM GameItemComponentProtos WHERE Id={f['StockComponent']} ORDER BY RevisionNumber"
        stock = d.rows(stock_query)
        d.start(prepare=True)
        for role in ('caster', 'reader'): native_cap(role)
        verify_prior_failed_fixture()
        for role in ('caster', 'reader'):
            d.command('admin', 'cure ' + f['Players'][role]['Keyword'] + ' all', 'remove all.*wounds')
            d.wound(role)
        for target, merit in [('qan22caster', f['Merit']), ('qan22reader', f['AlternateMerit']), ('qan22reader', f['WrongMerit'])]:
            d.command('admin', f'givemerit {target} {merit}', 'grant|already')
        d.command('admin', f"magic capability edit {f['Capability']}", 'edit')
        for spell in (f['Mend'], f['Focus']):
            d.command('admin', f'magic capability set casting entry add {spell}', 'updated|already|admitted')
            d.command('admin', f"magic capability set casting entry trait {spell} {f['Trait']}", 'updated')
            d.command('admin', f'magic capability set casting entry skill {spell} 30 60 relative', 'updated')
        d.command('admin', 'magic capability close')
        for target, capability in [(caster, f['Capability']), (reader, f['AlternateCapability']), (reader, f['WrongCapability'])]:
            d.command('admin', f'magic casting enrol {target} {capability} N23 explicit fixture route', 'Enrolled|already')
        for spell in (f['Mend'], f['Focus']):
            d.command('admin', f"magic casting grant {caster} {f['Capability']} {spell} N23 explicit fixture grant", 'grant|acquir|already')
        d.stop()
        s.sql(f"UPDATE CharacterTraits SET Value=45 WHERE CharacterId={caster} AND TraitDefinitionId={f['Trait']}")
        s.sql(f"UPDATE CharacterAcquiredSpells SET ControlledGrade=3 WHERE CharacterId={caster} AND MagicSpellId IN ({f['Mend']},{f['Focus']})")
        for actor in (caster, reader):
            s.sql(f"INSERT INTO Characters_MagicResources (CharacterId,MagicResourceId,Amount) VALUES ({actor},{f['Reserve']},1000) ON DUPLICATE KEY UPDATE Amount=1000")
        d.start()
        d.devices['wand'] = d.load(f['Stock']['wand'], 'caster')
        produce('wand', 'caster', f['Capability'], 2)
        s.check('initial bank proves actual native raw proficiency forty-five', bank('wand')['producer']['raw'] == '45')
        d.activate('wand', 'caster'); d.transfer('wand', 'caster', 'reader')
        refuse('incompatible admitted route', 'wand', 'reader', f['WrongCapability'], f['Mend'], expected='does not explicitly admit')
        refuse('original capability is unavailable to recipient', 'wand', 'reader', f['Capability'], f['Mend'], expected='not currently available')
        refuse('unacquired stored spell', 'wand', 'reader', f['AlternateCapability'], f['Mend'], expected='not acquired')
        for spell in (f['Mend'], f['Focus']):
            d.command('admin', f"magic casting grant {reader} {f['AlternateCapability']} {spell} N23 explicit alternate grant", 'grant|acquir|already')
        refuse('insufficient controlled grade', 'wand', 'reader', f['AlternateCapability'], f['Mend'], expected='explicit overreach')
        d.stop()
        s.sql(f"UPDATE CharacterTraits SET Value=44 WHERE CharacterId={reader} AND TraitDefinitionId={f['Trait']}")
        s.sql(f"UPDATE CharacterAcquiredSpells SET ControlledGrade=3 WHERE CharacterId={reader} AND MagicSpellId IN ({f['Mend']},{f['Focus']})")
        d.start()
        s.check('weaker producer has grade three but raw proficiency below the recorded forty-five',
                d.scalar(f"SELECT Value FROM CharacterTraits WHERE CharacterId={reader} AND TraitDefinitionId={f['Trait']}", 'weaker raw skill') == 44 and
                bank('wand')['producer']['raw'] == '45' and
                d.scalar(f"SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={reader} AND MagicSpellId={f['Mend']}", 'weaker controlled grade') == 3)
        refuse('insufficient reproducible raw potency', 'wand', 'reader', f['AlternateCapability'], f['Mend'], expected='cannot prove at least')
        d.stop()
        s.sql(f"UPDATE CharacterTraits SET Value=49 WHERE CharacterId={reader} AND TraitDefinitionId={f['Trait']}")
        d.start()
        s.check('alternate producer raw forty-nine is above the bank threshold and below the native cap',
                d.scalar(f"SELECT Value FROM CharacterTraits WHERE CharacterId={reader} AND TraitDefinitionId={f['Trait']}", 'stronger raw skill') == 49 and
                bank('wand')['producer']['raw'] == '45' and native_cap('reader') >= 49)
        refuse('incompatible grade in a nonempty bank', 'wand', 'reader', f['AlternateCapability'], f['Mend'], grade=2, expected='cannot prove at least')
        produce('wand', 'reader', f['AlternateCapability'], 2)
        d.transfer('wand', 'reader', 'caster'); produce('wand', 'caster', f['Capability'])
        refuse('request exceeds missing capacity', 'wand', 'caster', f['Capability'], f['Mend'], count=2, expected='missing charge capacity')
        d.transfer('wand', 'caster', 'reader')
        before_condition = d.reserve('reader'); frozen = bank('wand'); d.stop()
        s.sql(f"UPDATE Characters_MagicResources SET Amount=49 WHERE CharacterId={reader} AND MagicResourceId={f['Reserve']}")
        s.report['reserveConditioning'] = dict(actor=reader, before=before_condition, after=49, fixtureWithdrawn=before_condition - 49,
            reason='Explicit insufficient-reserve negative input; never restored or used to refund production')
        d.start()
        s.check('cold load preserves the completed top-ups and exact bank', bank('wand') == frozen)
        refuse('insufficient reserve for one legitimate recharge', 'wand', 'reader', f['AlternateCapability'], f['Mend'], expected='Insufficient.*production reserve')
        d.transfer('wand', 'reader', 'caster'); d.command('caster', 'drop wand', 'You drop')
        d.devices['private'] = d.load(f['PrivateDevice'], 'caster')
        first = material()
        refuse('aggregate supplies cannot fund two charges with one item', 'private', 'caster', f['Capability'], f['Mend'], count=2,
               expected='material plan|same item quantity')
        s.check('refused aggregate plan preserves its real material', d.scalar(f'SELECT COUNT(*) FROM GameItems WHERE Id={first}', 'short supply survivor') == 1)
        second = material(); produce('private', 'caster', f['Capability'], 2, materials=(first, second))
        refuse('different whitelisted payload in an existing bank', 'private', 'caster', f['Capability'], f['Focus'], expected='cannot prove at least')
        d.command('caster', 'drop qan22device', 'You drop'); d.command('caster', 'get wand', 'You get')
        # Consume the stock bank before deliberately quarantining this producer's
        # spell/trait/reserve inputs with unresolved crashed production.
        for _ in range(4): d.activate('wand', 'caster')
        d.command('caster', 'drop wand', 'You drop'); load_interrupt()
        produce('interrupt', 'caster', f['Capability'])
        before, cancelled = begin('interrupt', 'caster', f['Capability'])
        d.command('caster', 'stop', r'You stop charging a magical device\.')
        after_cancel = observe('interrupt')
        s.check('native interruption preserves all old charges and refunds nothing',
                after_cancel['bank'] == before['bank'] and d.reserve('caster') ==
                next(x[1] for x in before['states']['caster']['reserves'] if x[0] == f['Reserve']) - 50)
        cancelled_row = next(x for x in after_cancel['journals'] if x[0] == cancelled)
        s.check('interrupted paid work is terminal without manufactured charges', cancelled_row[1] == 'Completed' and 'no charges or refund' in cancelled_row[9])
        s.report['cancelledWork'] = dict(operation=cancelled, paid=50, before=before, after=after_cancel)
        before_crash, pending = begin('interrupt', 'caster', f['Capability']); crash(pending)
        d.start()
        crash_state = observe('interrupt')
        s.check('crash retains old bank, paid reserve and exact stranded reservation',
                crash_state['bank']['charges'] == before_crash['bank']['charges'] and
                crash_state['bank']['snapshot'] == before_crash['bank']['snapshot'] and crash_state['bank']['reservation'] == pending and
                d.reserve('caster') == next(x[1] for x in before_crash['states']['caster']['reserves'] if x[0] == f['Reserve']) - 50)
        reservation_refusal = r'The captured device custody, bank or reservation changed\.'
        refuse('persisted incomplete work refuses automatic recharge', 'interrupt', 'caster', f['Capability'], f['Mend'], expected=reservation_refusal)
        old = bank('interrupt'); states = {role: d.state(role) for role in ('caster', 'reader')}
        d.command('caster', 'magicdevice charged qan23interrupt self', reservation_refusal)
        s.check('persisted incomplete work cannot release previously stored charges', old == bank('interrupt') and
                states == {role: d.state(role) for role in states})
        s.report['incompleteWork'] = dict(operation=pending, paid=50, before=before_crash, after=observe('interrupt'))
        s.report['finalBanks'] = {device: bank(device) for device in d.devices}
        s.report['finalReserves'] = {role: d.reserve(role) for role in ('caster', 'reader')}
        s.check('exact production, paid interruption and fixture reserve ledger balances', s.report['finalReserves'] == {'caster': 600, 'reader': 49})
        produced = [guid for row in s.report['production'] for guid in row['newGuids']]
        consumed = [row['charge'] for row in s.report['releases']]
        remaining = [guid for row in s.report['finalBanks'].values() for guid in row['charges']]
        s.check('every produced GUID is unique and conserved as one spent receipt or one remaining charge',
                len(produced) == 8 and len(set(produced)) == 8 and len(consumed) == 5 and len(set(consumed)) == 5 and
                len(remaining) == 3 and len(set(remaining)) == 3 and set(produced) == set(consumed) | set(remaining) and not set(consumed) & set(remaining))
        s.check('stock component revisions remain unchanged', stock == d.rows(stock_query))
        d.stop()
        s.check('all captured native source and binary inputs remained unchanged', all(
            hashlib.sha256((s.repo / path).read_bytes()).hexdigest() == digest for path, digest in s.report['inputs'].items()))
        s.report.update(status='PASS', milestoneQualified=True)
    except BaseException as error:
        s.report.update(status='FAIL', milestoneQualified=False, error=s.helper.redact(str(error), [s.connection, s.password]))
    finally:
        for role, player in d.players.items(): s.transcripts.append(role + '\n' + player.full_transcript()); player.close()
        try: s.finish()
        finally:
            if d.fixture:
                try:
                    original = d.fixture['CapabilityDefinition']
                    s.sql(f"UPDATE MagicCapabilities SET Definition=CONVERT(UNHEX('{original.encode('utf-8').hex()}') USING utf8mb4) WHERE Id={d.fixture['Capability']}")
                    s.check('exact pre-run stock capability definition is restored', s.sql(f"SELECT HEX(Definition) FROM MagicCapabilities WHERE Id={d.fixture['Capability']}") == original.encode('utf-8').hex().upper())
                    s.report['capabilityDefinitionRestored'] = True
                except BaseException as error:
                    s.report.setdefault('cleanupErrors', []).append(s.helper.redact(str(error), [s.connection, s.password]))
                    s.report.update(status='FAIL', milestoneQualified=False)
            if mysql:
                try: s.sql('SHUTDOWN')
                except AssertionError: pass
                s.helper.stop_process_tree(mysql); s.report['mysqlStoppedByOwnedHandle'] = mysql.poll() is not None
                with socket.socket() as probe: s.report['mysqlPortClosed'] = probe.connect_ex(('127.0.0.1', s.instance['port'])) != 0
                if not s.report['mysqlStoppedByOwnedHandle'] or not s.report['mysqlPortClosed']: s.report.update(status='FAIL', milestoneQualified=False)
            expected = {x['index']: x['returncode'] for x in s.report.get('expectedForcedStops', [])}
            if any(not x['collectorStopped'] or x['returncode'] != expected.get(i, 0) for i, x in enumerate(s.report['serverProcesses'])):
                s.report.update(status='FAIL', milestoneQualified=False)
            (s.runtime / 'receipt.json').write_text(json.dumps(s.report, indent=2), encoding='utf-8')
    return 0 if s.report['status'] == 'PASS' else 1


if __name__ == '__main__': raise SystemExit(run())
