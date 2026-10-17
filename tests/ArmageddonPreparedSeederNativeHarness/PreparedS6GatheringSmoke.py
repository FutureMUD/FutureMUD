"""Installed source gathering, separate enabled copy and real native life owner."""
import datetime
import math
import re
import time
import xml.etree.ElementTree as ET


def run(host, admin, player, phase, data, mapped, flush):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s6_gathering']
    actor, body, room, cap, resource = (int(cfg[k]) for k in ('actor', 'body', 'room', 'capability', 'resource'))
    report = receipt.setdefault('s6_gathering', {'status': 'FAIL', 'limits': cfg['limits'], 'actions': []})

    def state():
        return {
            'operations': sql(mapped(f"SELECT HEX(Id),Status,Kind,RequestedAmount,DamageCost,StunCost,StaminaCost,PainCost,BodilyCostApplied,DestinationCredited,AccountingPersisted,NotificationCompleted,EcologicalApplied,IFNULL(HEX(LandDetailJson),''),Diagnostic FROM MagicGatheringOperations WHERE OwnerId={actor} ORDER BY CreatedUtc,Id")),
            'reserve': sql(mapped(f"SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}")),
            'gather_skill': sql(mapped(f"SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={int(cfg['trait'])}")),
            'field': sql(mapped(f"SELECT Pasture,HEX(Definition) FROM AgricultureFields WHERE Id={int(cfg['field'])}"), (1,)),
            'ecology': sql(mapped(f"SELECT ScarDamage,RecentPressure FROM RoomEnvironmentalStates WHERE RoomId={room}")),
            'registers': sql(mapped(f"SELECT ReferenceProperty,HEX(ValueDefinition) FROM VariableValues WHERE (ReferenceTypeDefinition='{cfg['character_type']}' AND ReferenceId={actor} AND ReferenceProperty IN ('armageddon_ecology','armageddon_gather_until')) OR (ReferenceTypeDefinition='{cfg['location_type']}' AND ReferenceId={room} AND ReferenceProperty='armageddon_ash_until') ORDER BY ReferenceProperty"), (1,)),
            'room_effects': sql(mapped(f"SELECT HEX(EffectData) FROM Rooms WHERE Id={room}"), (0,)),
            'wounds': sql(mapped(f"SELECT Id,OriginalDamage,CurrentDamage,CurrentStun,CurrentPain,WoundType FROM Wounds WHERE BodyId={body} ORDER BY Id"))}

    require(sql(f"SELECT HEX(FunctionText) FROM FutureProgs WHERE Id={int(cfg['gate'])}", (0,)) == [['return false']], 'Stock gathering gate was enabled.')
    if phase == 'cold':
        # Called after Player login; immutable receipts, reserve, ecological owner and
        # callback registers must survive. Current wound/stamina may naturally heal.
        current = state()
        for key in ('operations', 'reserve', 'gather_skill', 'field', 'ecology', 'registers', 'room_effects'):
            require(current[key] == report['saved'][key], 'Cold source gathering changed ' + key)
        require('Grey, lifeless ash collects on the ground.' in player.send('look', read_seconds=.3), 'Cold source ash addendum missing.')
        require(len(current['operations']) == 3, 'Cold login replayed a gathering callback or credit.')
        report['cold_status'] = 'PASS'
        assertions.append('S6 gathering: exact cold receipts, reserve, one-unit life debits, scars, cooldown/ash registers and single ash addendum persist without replay')
        return

    before = state()
    report['initial'] = before
    require(not before['operations'] and all(float(row[0]) == 0 for row in before['reserve']) and not before['registers'], 'Source learner inherited nonzero funding, paid operations or callback state: ' + str(before))
    require(not sql(mapped(f"SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={int(cfg['trait'])}")), 'Source learner inherited Gather skill.')
    admin.send(f'magic casting enrol participant {cap} isolated source gathering qualification', read_seconds=.4)
    flush()
    require(sql(mapped(f"SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={int(cfg['trait'])}")) == [['10']], 'Normal enrolment did not grant source Gather opening10.')
    for original, method, amount in ((True, 'self', 1), (True, 'land', 1), (False, 'self', .5), (False, 'self', 1000)):
        old = state()
        selected = int(cfg['stock_capability']) if original else cap
        player.send(f"{cfg['invoked']} gather {selected} {method} {amount}", read_seconds=.4)
        flush()
        require(state() == old, 'Refused source gathering changed payment/receipt/land state.')
    assertions.append('S6 gathering: retained stock false gate and fractional/oversized-request refusals preserve payment, receipt and land state')

    def probe():
        out = admin.send(f"prog execute {int(cfg['probe'])} participant", read_seconds=.3)
        matched = re.search(r'GATHERVALUES ([0-9.|-]+) ENDGATHER', out)
        require(matched, 'Native current-body probe failed: ' + host['redact'](out))
        return [float(x) for x in matched[1].split('|')]

    previous_ash = None
    first_cooldown = None
    for kind, amount, expected_damage, expected_reserve, expected_pasture, expected_scar in (
            ('self', 1, 2, 1, 10, 0), ('land', 1, .2, 2, 9, 1), ('land', 2, .315, 4, 8, 2)):
        maxima = probe()
        old = state()
        previews = player.send(f"{cfg['invoked']} gather {cap} preview {kind} {amount}", read_seconds=.3)
        flush()
        previewed = state()
        require(all(previewed[key] == old[key] for key in old if key != 'wounds'), 'Source preview mutated durable accounting state.')
        output = player.send(f"{cfg['invoked']} gather {cap} {kind} {amount}", read_seconds=1.2)
        deadline = time.monotonic() + 8
        while time.monotonic() < deadline:
            flush()
            current = state()
            if len(current['operations']) > len(old['operations']) and current['operations'][-1][1] in ('Completed', 'NeedsReview'): break
            output += player.read_for(.3)
        report['last_state'] = current
        report['last_probe'] = maxima
        report['last_output'] = host['redact'](output)
        require(len(current['operations']) == len(old['operations']) + 1, 'Source gather did not create exactly one receipt: ' + host['redact'](output))
        operation = current['operations'][-1]
        require(operation[1:3] == ['Completed', kind.title()] and float(operation[3]) == amount and operation[8:12] == ['1'] * 4,
                'Source paid/callback receipt failed: ' + str(operation[:13]) + ' output=' + host['redact'](output))
        require(math.isclose(float(operation[4]), expected_damage, abs_tol=1e-7) and float(operation[7]) == 0, 'Source HP price or independent pain cost differs.')
        # The source proportional channels use the price before HP floor clamping.
        require(math.isclose(float(operation[5]), min(maxima[4], maxima[1] * expected_damage / maxima[0]), abs_tol=1e-7), 'Source proportional native stun price differs.')
        require(math.isclose(float(operation[6]), min(maxima[5], maxima[2] * expected_damage / maxima[0]), abs_tol=1e-7), 'Source proportional native stamina price differs.')
        require(current['reserve'] == [[str(expected_reserve)]] and int(current['field'][0][0]) == expected_pasture, 'Source reserve/life conservation failed.')
        old_original = sum(float(x[1]) for x in old['wounds'])
        new_original = sum(float(x[1]) for x in current['wounds'])
        require(current['wounds'] and all(x[5] == 'SimpleOrganic' for x in current['wounds']) and
                math.isclose(new_original - old_original, expected_damage, abs_tol=1e-7),
                'Native cellular original-damage total did not increase by exactly the paid amount.')
        if kind == 'land':
            require(current['ecology'] == [[str(expected_scar), '0']], 'Land total scar/pressure accounting differs.')
            require(operation[12] == '1', 'Land ecological child was not acknowledged.')
            registers = {x[0]: ET.fromstring(x[1]).text for x in current['registers']}
            require(registers['armageddon_ecology'] == '-1', 'Repeated Land action changed alignment twice during cooldown.')
            ash = datetime.datetime.fromisoformat(registers['armageddon_ash_until'].replace('Z', '+00:00'))
            cooldown = registers['armageddon_gather_until']
            if previous_ash is None:
                delta = (ash - datetime.datetime.now(datetime.timezone.utc)).total_seconds()
                require(255 < delta <= 271, 'Nominal source ash duration differs from270 real seconds per mana.')
                until = datetime.datetime.fromisoformat(cooldown.replace('Z', '+00:00'))
                require(580 < (until - datetime.datetime.now(datetime.timezone.utc)).total_seconds() < 1810, 'Source Gather affect cooldown outside skill10 native window.')
                first_cooldown = cooldown
            else:
                require((ash - previous_ash).total_seconds() == 540 and cooldown == first_cooldown, 'Repeat Land did not extend ash additively while retaining the alignment cooldown.')
            previous_ash = ash
            effect = ET.fromstring(current['room_effects'][0][0])
            require(sum(1 for x in effect.iter('AdditionalText') if x.text == 'Grey, lifeless ash collects on the ground.') == 1, 'Ash addendum missing or duplicated.')
            require('Grey, lifeless ash collects on the ground.' in player.send('look', read_seconds=.3), 'Native ash addendum not visible.')
        report['actions'].append({'kind': kind, 'amount': amount, 'body_before': maxima, 'receipt': operation[:13], 'wounds_before': old['wounds'], 'wounds_after': current['wounds']})
        assertions.append(f'S6 gathering: paid {kind} amount{amount}, exact native bodily price, reserve{expected_reserve}, life{expected_pasture} and scar{expected_scar}')
    report['saved'] = state()
    report['status'] = 'PASS'
