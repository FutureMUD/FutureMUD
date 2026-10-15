"""S5: paid canonical production, native one-gram topical calibration and cold conservation.

This lane does not certify natural stock progression, other substances or all grades.
"""
from pathlib import Path
import time
import xml.etree.ElementTree as ET


def run(host, session, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s5_dose']
    actor, body, proto, component, resource = (int(x) for x in
        (data['character'], data['body'], cfg['prototype'], cfg['component'], cfg['resource']))
    source = cfg['source']
    require(Path(source['path']).resolve() == host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/Program.S5DoseFixture.cs'
            and host['sha'](source['path']) == source['sha256'], 'S5 dose preparation source differs.')
    # Keep the existing focused receipt envelope; the explicit stage/scope distinguishes S5.
    report = receipt.get('focus_devices')

    def flush():
        out = session.send('impdebug flush', read_seconds=.4)
        stop = time.monotonic() + 6
        while 'All queued saves have been flushed' not in out and time.monotonic() < stop:
            out += session.read_for(.4)
        require('All queued saves have been flushed' in out, 'Native save flush did not acknowledge within six seconds.')

    def balance():
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)'))[0][0])

    def doses():
        return [int(row[0]) for row in sql(f'SELECT Id FROM GameItems WHERE GameItemProtoId={proto} ORDER BY Id')]

    def binding(item):
        rows = sql(f'SELECT HEX(Definition) FROM GameItemComponents WHERE GameItemId={item} AND GameItemComponentProtoId={component}', (0,))
        require(len(rows) == 1, 'The exact topical component is missing.')
        return ET.fromstring(rows[0][0])

    def damage():
        return float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={body} AND Id={int(report["wound"])}')[0][0])

    def confirm_other_wounds():
        rows = sql(f'SELECT Id,CurrentDamage FROM Wounds WHERE BodyId={body} AND Id<>{int(report["wound"])} ORDER BY Id')
        expected = report['other_wounds']
        require(len(rows) == len(expected) and all(int(row[0]) == int(old[0]) and abs(float(row[1])-float(old[1])) < 1e-7
                for row, old in zip(rows, expected)), 'Topical calibration changed an unselected wound: ' + str((expected, rows)))

    def operations():
        return sql(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id', (0, 2))

    def confirm_dose(item):
        dose = binding(item)
        require(dose.get('Grade') == '1' and dose.get('CreatorId') == str(actor)
                and dose.get('SpellId') == str(cfg['spell']) and dose.get('Spent') == 'false', 'Produced dose lost paid grade/creator/spell or is spent.')
        grams = float(dose.get('Grams')) * 1000 / float(cfg['kilograms'])
        require(abs(grams - 1) < 1e-9 and dose.get('ReferenceGrams') == '1' and dose.get('BudgetPerGrade') == '10',
                'Persisted native mass does not calibrate to the locked one-gram, ten-unit grade-one dose.')
        return dose

    def apply(item, alias):
        pickup = session.send('get ' + alias, read_seconds=.3)
        flush()
        require(sql(f'SELECT GameItemId FROM Bodies_GameItems WHERE BodyId={body} AND GameItemId={item}') == [[str(item)]],
                'The exact produced dose is not in native held custody: ' + host['redact'](pickup))
        before, funds, old_ops = damage(), balance(), operations()
        # A partial physical dose must refuse without consuming or healing.
        out = session.send(f'apply {alias} me "{report["part"]}" 0.5g', read_seconds=.3)
        flush()
        require(damage() == before and item in doses(), 'A half-dose healed or consumed the full-dose carrier: ' + host['redact'](out))
        confirm_dose(item)
        out = session.send(f'apply {alias} me "{report["part"]}"', read_seconds=.3)
        flush()
        after = damage()
        require(abs(before - after - 10) < 1e-7 and item not in doses(),
                'Native full-dose application did not consume exactly one item and heal exactly ten wound units: '
                + str((before, after, doses())) + ' ' + host['redact'](out))
        session.send(f'apply {alias} me "{report["part"]}"', read_seconds=.3)
        flush()
        require(damage() == after and item not in doses(), 'A spent item replayed healing.')
        confirm_other_wounds()
        require(balance() == funds and operations() == old_ops, 'Topical application changed reserve or created another casting operation.')
        report.setdefault('applications', []).append({'item': item, 'before': before, 'after': after, 'healed': before-after, 'grams_consumed': 1})
        return after

    for gate in ('use_gate', 'craft_gate'):
        rows = sql(f'SELECT HEX(FunctionText) FROM FutureProgs WHERE Id={int(data["focus_devices"][gate])}', (0,))
        require(rows == [['return false']], 'Stock use/craft availability changed.')

    if phase == 'cold':
        flush()
        require(report['first'] not in doses() and report['second'] in doses(), 'Cold restart resurrected a spent dose or lost the unspent dose.')
        dose = confirm_dose(report['second'])
        require(dose.get('LifecycleId') == report['second_lifecycle'] and balance() == report['balance'], 'Cold restart changed dose origin or refilled reserves.')
        require(len(operations()) == report['operation_count'], 'Cold restart replayed production.')
        report['cold_damage_after_login'] = damage()
        require(abs(report['cold_damage_after_login'] - report['remaining_damage']) < 1e-7, 'Cold restart changed the calibrated wound remainder: ' + str((report['remaining_damage'], report['cold_damage_after_login'])))
        confirm_other_wounds()
        assertions.append('S5 topical: cold retains exact unspent one-gram origin/grade, spent absence, paid balance and wound remainder without production replay')
        apply(report['second'], 's5dosetwo')
        require(balance() == report['balance'], 'Topical release charged personal reserve.')
        report.update(status='PASS', cold_status='PASS')
        assertions.append('S5 topical: cold-loaded dose consumes once and heals exactly ten more native wound units; half-dose and repeated use refuse')
        return

    report = receipt['focus_devices'] = {'status': 'FAIL', 'stage': 'S5', 'scenario': 'restorative-mud', 'mode': cfg['mode'],
        'scope': cfg['limits'], 'applications': [], 'production_attempts': []}
    require(not doses(), 'Pinned calibration fixture unexpectedly contains pre-existing mud items.')
    session.send('drop all', read_seconds=.3)
    session.send(f'givemerit me {int(cfg["merit"])}', read_seconds=.3)
    session.send(f'magic casting enrol {actor} {int(cfg["capability"])} isolated S5 topical calibration', read_seconds=.3)
    flush()
    controlled = sql(mapped(f'SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} AND MagicSpellId={int(cfg["spell"])}'))
    require(controlled == [['1']], 'Normal enrolment did not acquire grade one independently of raw proficiency.')
    session.send(f'item load {host["item_command_reference"](int(cfg["anchor"]))}', read_seconds=.3)
    flush()
    anchor = sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems bi ON bi.GameItemId=gi.Id WHERE bi.BodyId={body} AND gi.GameItemProtoId={int(cfg["anchor"])}')
    require(len(anchor) == 1, 'Need exactly one ordinary held item anchor.')
    session.send('dub blank s5doseanchor', read_seconds=.3)
    flush()
    require(sql(f"SELECT TargetId FROM Dubs WHERE CharacterId={actor} AND TargetType='GameItem' AND TargetId={anchor[0][0]} AND Keywords LIKE '%s5doseanchor%'") == anchor, 'Native anchor alias does not bind the exact held item.')
    for name, alias in (('first', 's5doseone'), ('second', 's5dosetwo')):
        for attempt in range(3):
            current = balance()
            if current < 99:
                session.send(f'{cfg["invoked"]} gather {int(cfg["capability"])} draw {100-current:.4f}', read_seconds=.4)
                stop = time.monotonic() + 18
                while time.monotonic() < stop and balance() < 99:
                    session.read_for(.5)
                require(balance() >= 99, 'Finite paid qualification gathering failed to fund production.')
            before, old_items = balance(), set(doses())
            old_ops = {row[0] for row in operations()}
            out = session.send(f'{cfg["invoked"]} cast "Restorative Mud" grade 1 on s5doseanchor via {int(cfg["capability"])}', read_seconds=.4)
            flush()
            added = [row for row in operations() if row[0] not in old_ops]
            require(len(added) == 1 and added[0][1] == 'Completed', 'Canonical dose production did not complete a paid operation: ' + host['redact'](out))
            casting = ET.fromstring(added[0][2])
            costs = casting.findall('Cost')
            require(len(costs) == 1 and costs[0].get('resource') == str(resource) and float(costs[0].get('amount')) == 50
                    and abs(balance() - (before - 50)) < 1e-7, 'Production did not pay exactly the grade-one source efficiency cost from Water reserve.')
            produced = set(doses()) - old_items
            report['production_attempts'].append({'operation': added[0][0], 'paid': 50, 'applied': casting.get('applied'), 'produced': sorted(produced)})
            if produced:
                break
        require(len(produced) == 1, 'Three bounded paid attempts did not create exactly one dose.')
        item = produced.pop()
        dose = confirm_dose(item)
        report[name] = item
        report[name + '_lifecycle'] = dose.get('LifecycleId')
        session.send('get ' + ('mud' if name == 'first' else '2.mud'), read_seconds=.3)
        session.send('dub mud ' + alias, read_seconds=.3)
        flush()
        require(sql(f"SELECT TargetId FROM Dubs WHERE CharacterId={actor} AND TargetType='GameItem' AND TargetId={item} AND Keywords LIKE '%{alias}%'") == [[str(item)]], 'Native dose alias did not bind exact newly produced item.')
        session.send('drop ' + alias, read_seconds=.3)
        flush()
    assertions.append('S5 topical: two actual acquired-route grade-one casts each pay exactly50 Water energy and create one independently bound native one-gram dose')
    # The native loader materialised exact preboot wound rows. This qualifies
    # topical healing, without making a stochastic attack/armour claim.
    session.send('drop s5doseanchor', read_seconds=.3)
    flush()
    wounds = sql(f'SELECT w.Id,p.Name,w.CurrentDamage FROM Wounds w JOIN BodypartProto p ON p.Id=w.BodypartProtoId WHERE w.BodyId={body} AND w.CurrentDamage>0 ORDER BY w.Id')
    report['prepared_wounds'] = wounds
    report['wound_setup'] = cfg['wound_setup']
    selected = [row for row in wounds if int(row[0]) == int(cfg['calibration_wound']) and row[1] == 'abdomen']
    require(len(selected) == 1 and abs(float(selected[0][2])-30) < 1e-7, 'The exact loaded abdomen calibration wound changed before application: ' + str(wounds))
    report['wound'], report['part'] = int(selected[0][0]), selected[0][1]
    report['other_wounds'] = sql(f'SELECT Id,CurrentDamage FROM Wounds WHERE BodyId={body} AND Id<>{report["wound"]} ORDER BY Id')
    require(len(report['other_wounds']) == 1 and int(report['other_wounds'][0][0]) == int(cfg['foreign_wound'])
            and abs(float(report['other_wounds'][0][1])-1) < 1e-7, 'Foreign prepared control wound differs.')
    report['remaining_damage'] = apply(report['first'], 's5doseone')
    report['balance'] = balance()
    report['operation_count'] = len(operations())
    confirm_dose(report['second'])
    report['status'] = 'PASS'
    assertions.append('S5 topical: native half-dose refusal; exact full gram consumed before ten-unit wound healing; repeated application cannot heal again; second origin remains unspent')
