"""Targeted Simple/Class UI and ordinary-body funding; no N24 or ash wait."""
import importlib.util
import json
import math
import os
import re
import time
import xml.etree.ElementTree as ET
from pathlib import Path


def run(host, admin, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s6_ui_funding']
    report = receipt.setdefault('s6_ui_funding', {'status': 'FAIL', 'limits': cfg['limits']})
    require(os.environ.get('FUTUREMUD_S6_MEASUREMENT_SCOPE') == 'ui-funding', 'Wrong targeted measurement scope.')
    ui_files = json.loads(Path(os.environ['FUTUREMUD_S6_UI_COMMANDS']).read_text(encoding='utf-8-sig'))

    def load(name):
        spec = importlib.util.spec_from_file_location(name, host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness' / (name + '.py'))
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)
        return module

    def flush():
        out = admin.send('impdebug flush', read_seconds=.3)
        deadline = time.monotonic() + 6
        while 'All queued saves have been flushed' not in out and time.monotonic() < deadline:
            out += admin.read_for(.3)
        require('All queued saves have been flushed' in out, 'Measurement save flush timed out.')

    def clean(text):
        return re.sub(r'\x1b\[[0-?]*[ -/]*[@-~]', '', text)

    def login(account):
        player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
        player.read_for(1)
        for command in ('l', account): player.send(command, read_seconds=.6)
        player.send(host['PASSWORD'], secret=True, read_seconds=.7)
        for command in ('c', '1'): player.send(command, read_seconds=1)
        require('Guest Lounge' in player.send('look', read_seconds=.5), 'Ordinary UI-created character did not enter the prepared room.')
        return player

    def observe(player, ui):
        actor, body = int(ui['actor']), int(ui['body'])
        name = sql(mapped(f'SELECT Name FROM Characters WHERE Id={actor}'))[0][0]
        require(re.fullmatch(r'[A-Za-z]+', name), 'Native observation requires the explicit unambiguous personal name.')
        output = clean(admin.send(f"prog execute {int(data['s6_gathering']['probe'])} {name}", read_seconds=.3))
        found = re.search(r'GATHERVALUES ([0-9.|-]+) ENDGATHER', output)
        require(found, 'Current-body native health observation failed: ' + host['redact'](output))
        health = [float(x) for x in found[1].split('|')]
        output = clean(admin.send(f"prog execute {int(cfg['capacity_probe'])} {name}", read_seconds=.3))
        found = re.search(r'CAPACITYINPUT ([0-9.-]+) ENDCAPACITY', output)
        require(found, 'Effective native capacity attribute observation failed: ' + host['redact'](output))
        effective = float(found[1])
        status = clean(player.send(ui['invoked'], read_seconds=.3))
        found = re.search(r'You currently have ([0-9,.]+)/([0-9,.]+) ' + re.escape(ui['resource_name']) + r'\.', status)
        require(found, 'Actual native capacity output was absent: ' + host['redact'](status))
        raw = sql(mapped(f"SELECT Value FROM Traits WHERE BodyId={body} AND TraitDefinitionId={int(ui['capacity_attribute'])}"))
        policy = ET.fromstring(sql(mapped(f"SELECT HEX(Definition) FROM MagicResources WHERE Id={int(ui['resource'])}"), (0,))[0][0]).find('AttributeCapacity')
        require(policy is not None and policy.get('basis') == 'effective' and policy.get('attribute') == str(ui['capacity_attribute']), 'Installed native capacity basis differs.')
        cap = float(found[2].replace(',', ''))
        expected = max(2*effective, min(12*effective, 100+3*(effective-13)))
        require(math.isclose(cap, expected, abs_tol=.011), 'Displayed native capacity differs from locked selected-attribute expression.')
        return {'actor': actor, 'body': body, 'attribute': ui['capacity_attribute'], 'raw_attribute': float(raw[0][0]),
                'effective_attribute': effective, 'basis': policy.get('basis'), 'native_capacity': cap,
                'native_reserve': float(found[1].replace(',', '')), 'max_hp_stun_stamina': health[:3],
                'remaining_hp_stun_stamina': health[3:6], 'native_gather': health[6]}

    chargen = load('PreparedS6ChargenSmoke')
    states = {}
    for key, storage in (('sorcerer', 's6_chargen'), ('earth', 's6_earth_chargen')):
        ui = data[storage]
        ui['commands_file'] = ui_files[key]
        ui['transcript_name'] = storage + '-transcript.txt'
        states[key] = chargen.run(host, admin, phase, dict(data, s6_chargen=ui), mapped, flush)
        ui.update(actor=receipt[storage]['actor'], body=receipt[storage]['body'])
    sorc = data['s6_chargen']
    actor, body, cap, resource, trait = (int(sorc[k]) for k in ('actor', 'body', 'capability', 'resource', 'trait'))
    land = data['s6_gathering']

    def sorcerer_state():
        return {
            'gathering': sql(mapped(f'SELECT HEX(Id),Status,Kind,RequestedAmount,DamageCost,StunCost,StaminaCost,PainCost,BodilyCostApplied,DestinationCredited,AccountingPersisted,NotificationCompleted,EcologicalApplied FROM MagicGatheringOperations WHERE OwnerId={actor} ORDER BY CreatedUtc,Id')),
            'field': sql(mapped(f"SELECT Pasture,HEX(Definition) FROM AgricultureFields WHERE Id={int(land['field'])}"), (1,)),
            'ecology': sql(mapped(f"SELECT ScarDamage,RecentPressure FROM RoomEnvironmentalStates WHERE RoomId={int(land['room'])}")),
            'wounds': sql(mapped(f'SELECT Id,OriginalDamage,CurrentDamage FROM Wounds WHERE BodyId={body} ORDER BY Id'))}

    if phase == 'cold':
        current = sorcerer_state()
        require(all(current[k] == report['sorcerer']['saved'][k] for k in ('gathering', 'field', 'ecology')), 'Cold ordinary source funding replayed payment, yield or scar.')
        player = login(sorc['account'])
        try:
            flush()
            require(sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}')) == report['sorcerer']['saved_reserve'], 'Cold Sorcerer login refilled/refunded its earned reserve.')
        finally:
            player.send('quit', read_seconds=.5)
            player.close()
        load('PreparedS5ProgressionSmoke').run(host, admin, phase, dict(data, s5_progression=report['earth_config']), mapped)
        report['cold_status'] = 'PASS'
        assertions.append('S6 targeted funding: exact ordinary UI identities, paid source/practice receipts and progress survive cold without replay/refund')
        return

    player = login(sorc['account'])
    sorc_report = report['sorcerer'] = {'status': 'FAIL'}
    try:
        measured = observe(player, sorc)
        sorc_report['body'] = measured
        require(measured['native_reserve'] == 0 and measured['native_gather'] == 10, 'Ordinary Sorcerer did not start unfunded with Gather10.')
        old = sorcerer_state()
        for method in ('self', 'land'):
            output = clean(player.send(f"{sorc['invoked']} gather {cap} preview {method} 50", read_seconds=.4))
            flush()
            current = sorcerer_state()
            require(all(current[k] == old[k] for k in ('gathering', 'field', 'ecology')), 'Full-price source preview mutated accounting.')
            sorc_report[method + '_preview'] = host['redact'](output)
        eligible = 'body price stamina' in sorc_report['land_preview']
        sorc_report['land_affordable'] = eligible
        if eligible:
            output = player.send(f"{sorc['invoked']} gather {cap} land 50", read_seconds=1.2)
            deadline = time.monotonic()+8
            while time.monotonic() < deadline:
                flush()
                current = sorcerer_state()
                if current['gathering'] and current['gathering'][-1][1] in ('Completed', 'NeedsReview'): break
                output += player.read_for(.3)
            require(len(current['gathering']) == 1, 'One ordinary full-price Land action did not produce one receipt: ' + host['redact'](output))
            op = current['gathering'][0]
            require(op[1:4] == ['Completed', 'Land', '50'] and op[8:] == ['1']*5, 'Ordinary Land payment/callback did not complete: ' + str(op))
            require(math.isclose(float(op[4]), 7, abs_tol=1e-7) and float(op[7]) == 0, 'Gather10/ecology0 fertile Land50 native HP/pain cost differs.')
            hp, stun, stamina = measured['max_hp_stun_stamina']
            require(math.isclose(float(op[5]), min(measured['remaining_hp_stun_stamina'][1], stun*7/hp), abs_tol=1e-7) and
                    math.isclose(float(op[6]), min(measured['remaining_hp_stun_stamina'][2], stamina*7/hp), abs_tol=1e-7), 'Ordinary proportional native channels differ.')
            require(float(current['field'][0][0]) == 9 and current['ecology'] == [['1', '0']], 'One Land action did not debit one yield and one scar.')
            require(math.isclose(sum(float(x[1]) for x in current['wounds']) - sum(float(x[1]) for x in old['wounds']), 7, abs_tol=1e-7), 'Ordinary native wound original-damage increase differs from seven.')
            require(sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}')) == [['50']], 'Ordinary Land did not fund exactly50.')
            sorc_report['paid_gathering'] = current
            prior_raw = float(sql(mapped(f'SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={trait}'))[0][0])
            started = time.monotonic()
            output = player.send(f"{sorc['invoked']} practice \"{cfg['sorcerer_practice_name']}\" grade 1 via {cap}", read_seconds=.4)
            deadline = time.monotonic()+45
            while time.monotonic() < deadline:
                flush()
                rows = sql(mapped(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} AND MagicSpellId={int(cfg["sorcerer_practice"])} ORDER BY CreatedUtc'), (2,))
                practice = [x for x in rows if ET.fromstring(x[2]).get('mode') == 'Practice']
                if practice and practice[0][1] != 'Practising': break
                player.read_for(.6)
            require(len(practice) == 1 and practice[0][1] == 'Completed' and time.monotonic()-started >= 29, 'Earned Sorcerer reserve did not pay one native thirty-second practice: ' + host['redact'](output))
            payload = ET.fromstring(practice[0][2])
            costs = payload.findall('Cost')
            require(len(costs) == 1 and costs[0].get('resource') == str(resource) and float(costs[0].get('amount')) == 50 and payload.get('masteryEligible') == 'false', 'Ordinary Sorcerer practice altered the stock quote or grade eligibility.')
            after_raw = float(sql(mapped(f'SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={trait}'))[0][0])
            require(after_raw in (prior_raw, prior_raw+.5), 'Ordinary Sorcerer practice awarded non-stock raw gain.')
            require(sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}')) == [['0']], 'Ordinary Sorcerer practice did not spend exactly its earned50.')
            sorc_report.update(practice=practice[0], raw_before=prior_raw, raw_after=after_raw, paid=50)
            assertions.append('S6 ordinary Sorcerer: actual normal-body full-price Self/Land previews; Land50 costs seven HP plus native proportional channels, one yield/scar, then pays canonical thirty-second practice')
        else:
            reason = sorc_report['land_preview']
            capacity_refusal = measured['native_capacity'] < 50 and 'safe destination-resource headroom' in reason
            health_refusal = measured['remaining_hp_stun_stamina'][0] < 7 and 'configured gathering policy does not currently permit' in reason
            require(capacity_refusal or health_refusal, 'Land50 failed for an unqualified reason, rather than measured ordinary affordability: '+reason)
            sorc_report['measurement_outcome'] = 'locked-rule Land50 refusal preserved without forced funding'
        sorc_report['status'] = 'PASS'
    finally:
        try:
            player.send('quit', read_seconds=.5)
            flush()
            sorc_report['saved'] = sorcerer_state()
            sorc_report['saved_reserve'] = sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}'))
        finally:
            (Path(receipt['runtime_paths'][-1])/'s6-ordinary-sorcerer.txt').write_text(player.full_transcript(), encoding='utf-8')
            player.close()
    earth = data['s6_earth_chargen']
    earth_config = dict(data['s5_progression'])
    earth_config.update({k: earth[k] for k in ('actor', 'body', 'account', 'capability', 'merit', 'resource', 'trait', 'roots', 'invoked', 'capacity_attribute', 'resource_name')})
    earth_config.update(normal_chargen=True, opening=earth['expected_skills'][str(earth['trait'])], skill_ids=list(map(int, earth['expected_skills'])),
                        limits='Actual normal Simple Earth Class UI-created Player body, native unmodified attributes, zero authored reserve, real minute regeneration and canonical paid practice; no administrator enrolment, direct credit or forced gains.')

    def on_earth_login(player):
        measured = observe(player, earth)
        report['earth_body'] = measured
        earth_config.update(capacity=measured['native_capacity'], capacity_raw=measured['raw_attribute'])

    earth_host = dict(host, on_s5_player_login=on_earth_login)
    load('PreparedS5ProgressionSmoke').run(earth_host, admin, phase, dict(data, s5_progression=earth_config), mapped)
    report['earth_config'] = earth_config
    for key, storage in (('sorcerer', 's6_chargen'), ('earth', 's6_earth_chargen')):
        receipt[storage]['saved'] = states[key]()
    report['status'] = 'PASS'
