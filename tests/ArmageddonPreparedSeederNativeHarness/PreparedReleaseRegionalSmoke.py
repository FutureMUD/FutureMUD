"""Original Release regional-first Land gathering with native local fallback and cold conservation."""
import json
import math
import re
import time


def release_definition_matches(table, expected, actual, phase):
    if actual == expected:
        return True
    # A normal ecology checkpoint exports these previously implicit defaults.
    # Accept only this exact insertion on cold boot, preserving every other byte.
    if (phase != 'cold' or table != 'MagicGenerators'
            or not expected.startswith('<Definition version="1">')
            or '<Organic ' not in expected or expected.count('  <Outputs>') != 1
            or '<DecayReferenceUtc' in expected or '<DecayIntegral' in expected):
        return False
    newline = '\r\n' if '\r\n' in expected else '\n'
    defaults = ('  <DecayReferenceUtc>1970-01-01T00:00:00.0000000+00:00</DecayReferenceUtc>'
                + newline + '  <DecayIntegral>0</DecayIntegral>' + newline)
    return actual == expected.replace('  <Outputs>', defaults + '  <Outputs>', 1)


def run(host, admin, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['regional_land']
    actor, body, cap, resource, area, a, b = (int(cfg[k]) for k in ('actor', 'body', 'capability', 'resource', 'area', 'a', 'b'))
    report = receipt.setdefault('regional_land', {'status': 'FAIL', 'limits': cfg['limits'], 'actions': []})

    def flush():
        out = admin.send('impdebug flush', read_seconds=.4)
        end = time.monotonic() + 6
        while 'All queued saves have been flushed' not in out and time.monotonic() < end:
            out += admin.read_for(.3)
        require('All queued saves have been flushed' in out, 'Regional administrator flush timed out.')

    def state():
        return {
            'pool': sql(mapped(f"SELECT ProfileId,Priority,Revision,Stock,Capacity,Initialised,Inherit FROM RegionalGatheringPools WHERE OwnerKind='Area' AND OwnerId={area}")),
            'operations': sql(mapped(f"SELECT HEX(Id),Status,Kind,RequestedAmount,DamageCost,StunCost,StaminaCost,PainCost,BodilyCostApplied,DestinationCredited,AccountingPersisted,NotificationCompleted,EcologicalApplied,HEX(LandDetailJson) FROM MagicGatheringOperations WHERE OwnerId={actor} ORDER BY Id"), (13,)),
            'reserve': sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}')),
            'forage': sql(mapped(f"SELECT RoomId,Yield FROM RoomsForagableYields WHERE RoomId IN ({a},{b}) AND ForagableType='armageddon-life' ORDER BY RoomId")),
            'ecology': sql(mapped(f'SELECT RoomId,ScarDamage,RecentPressure FROM RoomEnvironmentalStates WHERE RoomId IN ({a},{b}) ORDER BY RoomId')),
            'skills': sql(mapped(f'SELECT TraitDefinitionId,Value FROM CharacterTraits WHERE CharacterId={actor} ORDER BY TraitDefinitionId')),
            'participants': sql(mapped(f'SELECT HEX(p.OperationId),p.RoomId,p.SourceKey FROM MagicGatheringParticipants p JOIN MagicGatheringOperations o ON o.Id=p.OperationId WHERE o.OwnerId={actor} ORDER BY p.OperationId,p.SourceKey'))}

    if phase == 'cold':
        require(state() == report['saved'], 'Regional pre-login cold state changed/refilled/replayed.')
        report['cold_status'] = 'PASS'
        assertions.append('Regional Land: pre-login cold pool, terminal receipts, reserve, native forage, scars, skills and participant custody conserved exactly')
        return

    require(not state()['operations'] and not state()['pool'], 'New regional scenario inherited payment or pool state.')
    require(all(float(x[0]) == 0 for x in state()['reserve']), 'Regional learner inherited energy.')
    authority = sql(mapped(f'SELECT CAST(c.IsAdminAvatar AS UNSIGNED),g.AuthorityLevel FROM Characters c JOIN Accounts ac ON ac.Id=c.AccountId JOIN AuthorityGroups g ON g.Id=ac.AuthorityGroupId WHERE c.Id={actor}'))
    require(authority == [['0', '2']], 'Regional learner is not an ordinary Player.')
    admin.send(f'goto {a}', read_seconds=.4)
    player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
    try:
        player.read_for(1)
        player.send('l', read_seconds=1); player.send(cfg['account'], read_seconds=1)
        player.send(host['PASSWORD'], secret=True, read_seconds=1)
        player.send('c', read_seconds=1); player.send('1', read_seconds=2)
        looked = player.send('look', read_seconds=.5)
        require('Guest Lounge regional source' in looked, 'Regional ordinary Player login failed: '+host['redact'](looked))
        admin.send(f'magic casting enrol participant {cap} regional Release qualification', read_seconds=.5)
        flush()
        require(sql(mapped(f"SELECT Value FROM CharacterTraits WHERE CharacterId={actor} AND TraitDefinitionId={int(cfg['trait'])}")) == [['10']], 'Regional normal enrolment did not grant Gather10.')
        admin.send(f"magic environment regional area {area} {int(cfg['profile'])}", read_seconds=.4)
        flush()
        require(state()['pool'][0][3:5] == ['6', '6'], 'Two fertile rooms did not initialise one six-life Area pool: '+str(state()['pool']))
        require(not state()['reserve'] or all(float(x[0]) == 0 for x in state()['reserve']), 'Enrolment/binding refilled personal energy.')
        previous_ash = False
        for index, amount in enumerate((50, 1, 1, 1, 1, 1, 50)):
            room = b if index == 6 else a
            if index == 6:
                movement_start = time.monotonic()
                moved = player.send('north', read_seconds=.4)
                end = movement_start + 30
                while time.monotonic() < end:
                    flush()
                    if sql(mapped(f'SELECT Location FROM Characters WHERE Id={actor}')) == [[str(b)]]:
                        break
                    moved += player.read_for(.5)
                require(sql(mapped(f'SELECT Location FROM Characters WHERE Id={actor}')) == [[str(b)]],
                        'Player did not cross the physical room exit within30seconds: '+host['redact'](moved))
                # Native Movement.FinalStep clears movement after a final delay capped at five seconds.
                # Observe that normal delay rather than shortening movement or teleporting the Player.
                moved += player.read_for(6)
                require('Regional local fallback' in player.send('look', read_seconds=.4),
                        'Player persisted in the destination but did not perceive the fallback room.')
                report['physical_move_wait_seconds'] = round(time.monotonic()-movement_start, 3)
                admin.send(f'goto {b}', read_seconds=.3)
            maxima_text = admin.send(f"prog execute {int(cfg['probe'])} participant", read_seconds=.3)
            match = re.search(r'GATHERVALUES ([0-9.|-]+) ENDGATHER', maxima_text)
            require(match, 'Native regional current-body probe failed: '+host['redact'](maxima_text))
            maxima = [float(x) for x in match[1].split('|')]
            old = state()
            preview = player.send(f"{cfg['invoked']} gather {cap} preview wildland {amount}", read_seconds=.3)
            flush()
            require(state() == old, 'Regional preview mutated durable stock/payment state.')
            output = player.send(f"{cfg['invoked']} gather {cap} wildland {amount}", read_seconds=1.5)
            end = time.monotonic() + 8
            while time.monotonic() < end:
                flush(); current = state()
                new = [x for x in current['operations'] if x[0] not in {y[0] for y in old['operations']}]
                if len(new) == 1 and new[0][1] in ('Completed', 'NeedsReview'): break
                output += player.read_for(.3)
            report['last_output'] = host['redact'](output)
            report['last_state'] = current
            require(len(new) == 1, 'Regional action did not create one receipt: '+host['redact'](output))
            op = new[0]
            require(op[1:3] == ['Completed', 'Land'] and op[8:13] == ['1']*5, 'Regional paid/callback accounting failed: '+str(op))
            detail = json.loads(op[13])
            if index < 6:
                require(detail['RegionalDebited'] and detail['RegionalSource']['Units'] == 1 and not detail['AppliedSources'], 'Regional action did not pay exactly one shared unit first.')
                require(float(current['pool'][0][3]) == 5-index, 'Shared pool did not lose exactly one unit.')
            else:
                require(not detail['RegionalDebited'] and detail['RegionalSource'] is None and len(detail['AppliedSources']) == 1, 'Local fallback incorrectly spent shared stock.')
                require(detail['AppliedSources'][0]['Selector'] == 'forage:armageddon-life' and detail['AppliedSources'][0]['TotalUnits'] == 1, 'Fallback did not pay one native forage life.')
                require([str(b), '2'] in current['forage'] and all(row[1] == '3' for row in current['forage'] if row[0] == str(a)), 'Approved native fallback life did not decrease3->2: '+str(current['forage']))
                require(float(current['pool'][0][3]) == 0, 'Fallback refilled/spent the empty shared pool.')
            expected_damage = math.ceil(amount*(1.5-.01*maxima[6]+.005*maxima[7]))*.1*(1.05 if previous_ash else 1)
            require(math.isclose(float(op[4]), expected_damage, abs_tol=1e-7) and float(op[7]) == 0, 'Approved Land health formula changed: '+str(op[4]))
            require(math.isclose(float(op[5]), min(maxima[4], maxima[1]*expected_damage/maxima[0]), abs_tol=1e-7), 'Native proportional stun price differs.')
            require(math.isclose(float(op[6]), min(maxima[5], maxima[2]*expected_damage/maxima[0]), abs_tol=1e-7), 'Native proportional stamina price differs.')
            expected_reserve = 105 if index == 6 else 50+index
            require(current['reserve'] == [[str(expected_reserve)]], 'Regional reserve conservation failed.')
            report['actions'].append({'amount': amount, 'room': room, 'maxima': maxima, 'preview': host['redact'](preview), 'operation': op})
            previous_ash = index < 5
            if index == 5:
                before_refusal = state()
                player.send(f"{cfg['invoked']} gather {cap} wildland 1", read_seconds=.8); flush()
                require(state() == before_refusal, 'No-matching-local-source refusal changed durable state.')
        flush()
        report['saved'] = state()
        require(report['saved']['ecology'] == [[str(a), '6', '0'], [str(b), '1', '0']], 'Regional scars were not applied once per action to the physical room.')
        report['status'] = 'PASS'
        assertions.append('Regional Land: original Release Sorcerer normal enrolment; shared Area6->0 across six paid actions; no-matching-source refusal; physical exit and approved native forage3->2 fallback; original health/stun/stamina formulas and reserve0->105')
    finally:
        try:
            # Restore the administrator's Guest Lounge-named location for the shared cold-login protocol.
            admin.send(f'goto {a}', read_seconds=.3)
            player.send('quit', read_seconds=1)
            flush()
            if report['status'] == 'PASS': report['saved'] = state()
        finally:
            (host['Path'](receipt['runtime_paths'][-1])/'regional-player-transcript.txt').write_text(player.full_transcript(), encoding='utf-8')
            player.close()
