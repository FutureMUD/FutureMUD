"""S5 full installed-stock progression and real zero-to-paid passive funding.

One representative native Player practice per mode; stochastic gains are observed,
not forced. Exact branch edges are covered by deterministic shared-contract tests.
"""
import time
from datetime import datetime, timezone
import xml.etree.ElementTree as ET


def run(host, admin, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s5_progression']
    actor, body, cap, resource, trait, root, child = (int(cfg[k]) for k in
        ('actor', 'body', 'capability', 'resource', 'trait', 'root', 'child'))

    def flush():
        out = admin.send('impdebug flush', read_seconds=.4)
        stop = time.monotonic() + 6
        while 'All queued saves have been flushed' not in out and time.monotonic() < stop:
            out += admin.read_for(.4)
        require('All queued saves have been flushed' in out, 'S5 independent administrator flush timed out.')

    def balance():
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)'))[0][0])

    def raw():
        rows = sql(mapped(f'SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={trait}'))
        return None if not rows else float(rows[0][0])

    def state():
        return {
            'acquired': sql(mapped(f'SELECT MagicSpellId,ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} ORDER BY MagicSpellId')),
            'skills': sql(mapped(f'SELECT TraitDefinitionId,Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId IN ({','.join(str(int(x)) for x in cfg['skill_ids'])}) ORDER BY TraitDefinitionId')),
            'opportunities': sql(mapped(f'SELECT TraitDefinitionId,NextOpportunityUtc,StateVersion FROM CharacterMagicSkillOpportunities WHERE CharacterId={actor} AND TraitDefinitionId IN ({','.join(str(int(x)) for x in cfg['skill_ids'])}) ORDER BY TraitDefinitionId')),
            'operations': sql(mapped(f'SELECT HEX(Id),Stage,HEX(Definition),CreatedUtc FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id'), (0, 2)),
            'resources': sql(mapped(f'SELECT MagicResourceId,Amount FROM CharactersMagicResources WHERE CharacterId={actor} ORDER BY MagicResourceId'))}

    def practice_operations():
        return [x for x in state()['operations'] if ET.fromstring(x[2]).tag == 'Casting' and ET.fromstring(x[2]).get('mode') == 'Practice']

    def practice(player):
        return player.send(f'armearth practice "Earthen Ward" grade 1 via {cap}', read_seconds=.4)

    for gate in ('use_gate', 'craft_gate'):
        require(sql(f'SELECT HEX(FunctionText) FROM FutureProgs WHERE Id={int(cfg[gate])}', (0,)) == [['return false']], 'Stock use/craft availability changed.')

    if phase == 'cold':
        report = receipt['s5_progression']
        require(state() == report['saved'], 'Cold pre-login replay/refill changed exact S5 learned/progress/cost state.')
        assertions.append('S5 progression: pre-login cold SQL retains exact acquisitions, raw skills, opportunities, paid operation and resource balances')
    else:
        report = receipt['s5_progression'] = {'status': 'FAIL', 'mode': cfg['mode'], 'scope': cfg['limits'], 'funding': []}
        if cfg.get('normal_chargen'):
            require(balance() == 0 and raw() == float(cfg['opening']) and
                    sorted(int(x[0]) for x in state()['acquired']) == sorted(cfg['roots']),
                    'Normal Class creation did not retain zero energy and exact authored openings.')
        else:
            require(balance() == 0 and raw() is None and not state()['acquired'], 'New ordinary learner did not start with zero energy and no magic knowledge/skill.')
    authority = sql(mapped(f'SELECT CAST(c.IsAdminAvatar AS UNSIGNED),g.AuthorityLevel FROM Characters c JOIN Accounts a ON a.Id=c.AccountId JOIN AuthorityGroups g ON g.Id=a.AuthorityGroupId WHERE c.Id={actor}'))
    report.setdefault('authority_by_phase', {})[phase] = authority
    require(authority == [['0', '2']], 'S5 learner requires a non-admin avatar and Player account authority; observed: ' + repr(authority))
    player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
    try:
        player.read_for(1)
        for command in ('l', cfg['account']):
            player.send(command, read_seconds=1)
        player.send(host['PASSWORD'], secret=True, read_seconds=1)
        player.send('c', read_seconds=1)
        player.send('1', read_seconds=2)
        looked = player.send('look', read_seconds=1)
        require('Guest Lounge' in looked, 'Ordinary S5 player login failed: ' + host['redact'](looked))
        denied = player.send('impdebug flush', read_seconds=.3)
        require('All queued saves have been flushed' not in denied, 'Ordinary learner unexpectedly has administrator command authority.')
        if phase == 'first' and host.get('on_s5_player_login'):
            host['on_s5_player_login'](player)
        if phase == 'cold':
            flush()
            current = state()
            require(all(current[k] == report['saved'][k] for k in ('acquired', 'skills', 'opportunities', 'operations')), 'Player cold login replayed acquisition, raw gain, opportunity or production.')
            before = report['saved_balance']
            require(balance() in (before, before + 2, before + 4), 'Cold login refilled the pool beyond one native minute tick.')
            report.update(cold_status='PASS', cold_balance=balance())
            assertions.append('S5 progression: ordinary Player cold login preserves progress and paid terminal operation without replay/refund; only native passive increment allowed')
            return
        if not cfg.get('normal_chargen'):
            admin.send(f'magic casting enrol participant {cap} isolated full-stock S5 qualification', read_seconds=.5)
        flush()
        acquired = state()['acquired']
        require(sorted(int(x[0]) for x in acquired) == sorted(cfg['roots']) and all(x[1] == '1' for x in acquired),
                'Normal enrolment did not grant exactly the full stock roots at controlled grade one: ' + str(acquired))
        require(raw() == float(cfg['opening']) and all(int(x[0]) != child for x in acquired), 'Stock raw opening or unlearned branch changed.')
        report['opening'] = raw()
        report['starting_roots'] = acquired
        prior = state()
        out = practice(player)
        flush()
        require(state()['operations'] == prior['operations'] and balance() < 50, 'Insufficient ordinary reserve did not refuse before payment/operation: ' + host['redact'](out))
        assertions.append('S5 progression: normal full-stock enrolment grants only starting roots at grade one and native raw opening; empty reserve refuses paid practice')
        if cfg.get('normal_chargen') and cfg['capacity'] < 50:
            report.update(status='PASS', measurement_outcome='actual ordinary capacity cannot hold the first stock50 quote; no forced attribute or funding')
            return
        player.send('sit', read_seconds=.4)
        begin = time.monotonic()
        last = balance()
        report['before_rest_balance'] = last
        require(last in (0, 2, 4), 'Unexpected initial native passive increment before rest.')
        while balance() < 50 and time.monotonic() - begin < cfg['rest_funding_timeout_seconds']:
            player.read_for(8)
            player.send('look', read_seconds=.1)  # Keep the ordinary session active during real-minute funding.
            flush()
            current = balance()
            if current != last:
                report['funding'].append({'elapsed_seconds': round(time.monotonic()-begin, 3), 'before': last, 'after': current})
                require(abs(current-last-min(4, cfg['capacity']-last)) < 1e-7, 'Rest funding differs from the locked native two/minute times two increment: ' + str(report['funding']))
                last = current
        require(balance() >= 50, 'Unmodified native passive funding did not reach the first 50-energy practice within fifteen minutes.')
        report['funding_elapsed_seconds'] = round(time.monotonic()-begin, 3)
        report['funded_balance'] = balance()
        require(state()['operations'] == prior['operations'], 'Funding created or changed a casting operation.')
        assertions.append('S5 funding: actual zero-to-first-50-energy reserve from native real-minute regeneration at rest; no direct credit or gather fallback')
        player.send('stand', read_seconds=.3)
        raw_before = raw()
        reserve_before = balance()
        out = practice(player)
        flush()
        ops = practice_operations()
        require(len(ops) == 1 and ops[0][1] == 'Practising', 'Funded normal practice did not start: ' + host['redact'](out))
        payload = ET.fromstring(ops[0][2])
        costs = payload.findall('Cost')
        require(payload.get('mode') == 'Practice' and len(costs) == 1 and costs[0].get('resource') == str(resource)
                and float(costs[0].get('amount')) == 50 and payload.get('skillEligible') == 'true' and payload.get('masteryEligible') == 'false',
                'Practice did not pay exactly the stock grade-one quote from its own reserve with separate progress eligibility.')
        reserve_after = balance()
        require(abs(reserve_after - (reserve_before-50)) < 1e-7 or abs(reserve_after - (reserve_before-48)) < 1e-7,
                'Actual practice reserve debit was not fifty, allowing only one standing two-unit native tick: ' + str((reserve_before, reserve_after)))
        report['reserve_before_practice'], report['reserve_after_payment'] = reserve_before, reserve_after
        require(raw() == raw_before, 'Raw proficiency advanced before thirty-second work completed.')
        deadline = time.monotonic()+45
        while time.monotonic() < deadline:
            player.read_for(.8)
            flush()
            ops = practice_operations()
            if ops[0][1] != 'Practising':
                break
        require(len(ops) == 1 and ops[0][1] == 'Completed', 'Native scheduled practice did not complete: ' + str(ops))
        report['raw_before'], report['raw_after'] = raw_before, raw()
        require(raw() in (raw_before, raw_before+.5), 'Stock stochastic native practice awarded a non-stock raw gain.')
        require(all(x[1] == '1' for x in state()['acquired']) and all(int(x[0]) != child for x in state()['acquired']), 'One ordinary non-overreach practice granted a grade or premature branch.')
        opportunities = state()['opportunities']
        require(len(opportunities) == 1, 'Native paid practice did not persist the shared skill opportunity gate.')
        stamp = lambda text: datetime.fromisoformat(text.replace('Z', '+00:00')).replace(tzinfo=timezone.utc)
        created = stamp(ops[0][3])
        require(abs((stamp(payload.get('deadlineUtc'))-created).total_seconds()-30) < .001
                and abs((stamp(opportunities[0][1])-created).total_seconds()-60) < .001,
                'Native practice duration or shared opportunity gate differs from the locked thirty/sixty seconds.')
        effects = sql(f'SELECT HEX(EffectData) FROM Bodies WHERE Id={body} UNION ALL SELECT HEX(EffectData) FROM Characters WHERE Id={actor}', (0,))
        require(not any('MagicSpellParent' in x[0] or 'spellarmour' in x[0].lower() for x in effects), 'Target-free practice manifested stock armour.')
        report['operation'] = ops[0]
        report['paid'] = 50
        assertions.append('S5 progression: ordinary Player performs full-cost target-free thirty-second canonical practice; native zero-or-half-point gain, grade separation and persisted opportunity gate')
        report['status'] = 'PASS'
    finally:
        try:
            player.send('quit', read_seconds=1)
            flush()
            if phase == 'first' and report['status'] == 'PASS':
                report['saved'] = state()
                report['saved_balance'] = balance()
        finally:
            (host['Path'](host['receipt']['runtime_paths'][-1])/'s5-player-transcript.txt').write_text(player.full_transcript(), encoding='utf-8')
            player.close()
