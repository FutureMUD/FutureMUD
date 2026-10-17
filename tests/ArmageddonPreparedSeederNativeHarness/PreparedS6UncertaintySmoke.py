"""Actual provider failure and canonical-owner conservation on disposable Player routes."""
import re
import time
import xml.etree.ElementTree as ET
from pathlib import Path


def run(host, admin, phase, data, mapped, flush):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s6_uncertainty']
    actor, body = int(cfg['actor']), int(cfg['body'])
    routes = {r['alias']: r for r in cfg['routes']}
    report = receipt.setdefault('s6_uncertainty', {'status': 'FAIL', 'limits': cfg['limits'], 'faults': [], 'casts': []})

    def state():
        return {'operations': sql(mapped(f'SELECT HEX(Id),Stage,ActorId,BodyId,MagicCapabilityId,MagicSpellId,ReserveId,HEX(Definition),HEX(Diagnostic) FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id'), (7, 8)),
                'acquired': sql(mapped(f'SELECT MagicSpellId,ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} ORDER BY MagicSpellId')),
                'resources': sql(mapped(f'SELECT MagicResourceId,Amount FROM CharactersMagicResources WHERE CharacterId={actor} ORDER BY MagicResourceId'))}

    player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':', 1)[1]), host['SECRETS'])
    triggers = []
    try:
        if phase == 'cold':
            before_login = state()
            require(before_login == report['saved'], 'Cold provider recovery changed exact paid receipts, reserve or grade before login.')
            projection = report['projection']
            require(sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{projection["life"]}"') == [['3']], 'Reboot did not retire the exact uncertain owned projection lifecycle.')
            require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={projection["instance"]}') and not sql(f'SELECT Id FROM Bodies WHERE Id={projection["body"]}'), 'Reboot retained or recreated uncertain temporary instance/body.')
            require(sql(f'SELECT BodyId FROM Characters WHERE Id={actor}') == [[str(body)]], 'Reboot replaced the canonical original body.')
        player.read_for(.5)
        for command in ('l', cfg['account']): player.send(command, read_seconds=.5)
        player.send(host['PASSWORD'], secret=True, read_seconds=.5)
        player.send('c', read_seconds=.5)
        player.send('1', read_seconds=1)
        require(sql(f'SELECT CAST(c.IsAdminAvatar AS UNSIGNED),g.AuthorityLevel FROM Characters c JOIN Accounts a ON a.Id=c.AccountId JOIN AuthorityGroups g ON g.Id=a.AuthorityGroupId WHERE c.Id={actor}') == [['0', '2']], 'N25 subject must be ordinary Player.')

        def balance(row):
            result = sql(mapped(f'SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={int(row["resource"])}'))
            return float(result[0][0]) if result else 0

        def fund(row):
            current = balance(row)
            if current >= 99: return
            previous = sql(mapped(f'SELECT HEX(Id) FROM MagicGatheringOperations WHERE OwnerId={actor}'))
            output = player.send(f'{row["invoked"]} gather {int(row["capability"])} draw {100-current:.4f}', read_seconds=.4)
            deadline = time.monotonic()+18
            while time.monotonic() < deadline and balance(row) < 99: output += player.read_for(.3)
            flush()
            require(balance(row) >= 99, 'N25 finite paid gathering did not fund the exact route: '+host['redact'](output))
            operations = sql(mapped(f'SELECT HEX(Id),Status,Kind,RequestedAmount,DestinationResourceId,StaminaCost,DamageCost,StunCost,PainCost,BodilyCostApplied,DestinationCredited,AccountingPersisted,NotificationCompleted FROM MagicGatheringOperations WHERE OwnerId={actor} ORDER BY CreatedUtc,Id'))
            added = [x for x in operations if [x[0]] not in previous]
            require(len(added) == 1 and added[0][1:3] == ['Completed', 'Self'] and
                    abs(float(added[0][3]) - (100-current)) < .00001 and int(added[0][4]) == int(row['resource']) and
                    float(added[0][5]) == 1 and all(float(x) == 0 for x in added[0][6:9]) and added[0][9:13] == ['1']*4,
                    'N25 funding lacks its exact completed owner/destination/amount and native bodily-debit receipt: '+str(added))
            report.setdefault('funding', []).append({'alias':row['alias'], 'receipt':added[0]})

        def command(row): return f'{row["invoked"]} cast "{row["name"]}" grade 1 on self via {int(row["capability"])}'

        def quarantine(row):
            before = state()
            output = player.send(command(row), read_seconds=.5)
            flush()
            require(state() == before and ('quarantined' in output or 'needs staff review' in output), 'Alternate route/body did not refuse the same canonical quarantine without payment: '+host['redact'](output))
            report.setdefault('quarantine_outputs', []).append(host['redact'](output))

        if phase == 'cold':
            quarantine(routes['alternate'])
            quarantine(routes['effect'])
            conserved = state()
            for fault in report['faults']:
                output = admin.send(f'magic casting resolve uncertainty {fault["guid"]} N25 same-operation conservative provider recovery', read_seconds=.5)
                require('Reconciled without replay, refund or new random roll' in output, 'Staff did not reconcile the original operation: '+host['redact'](output))
                again = admin.send(f'magic casting resolve uncertainty {fault["guid"]} N25 repeat acknowledgement must be idempotent', read_seconds=.3)
                require('already finalised' in again, 'Repeated reconciliation did not retain terminal identity.')
            flush()
            final = state()
            require(final['resources'] == conserved['resources'] and final['acquired'] == conserved['acquired'], 'Reconciliation refunded payment or duplicated grade/progress.')
            old = {r[0]: r for r in conserved['operations']}
            new = {r[0]: r for r in final['operations']}
            require(old.keys() == new.keys(), 'Reconciliation created a second casting operation.')
            for fault in report['faults']:
                identity = fault['hex']
                require(new[identity][1] == 'Reconciled' and new[identity][2:8] == old[identity][2:8], 'Reconciliation changed original actor/body, route, spell, reserve or captured payload.')
            require(sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE CreatorId={actor} AND SpellId={int(routes["effect"]["spell"])}') == [['1']], 'Recovery replayed the projection effect/lifecycle.')
            report.update(cold_status='PASS', reconciled=final)
            assertions.append('N25: cold actual provider quarantine, exact paid reserves/grade/GUIDs, same-ID idempotent staff reconciliation and reboot retirement conserve canonical body without replay/refund/second projection')
            return

        require(not state()['operations'] and not state()['acquired'] and not state()['resources'], 'N25 actor inherited casting or funding.')
        for row in routes.values():
            output = admin.send(f'magic casting enrol uncertainty {int(row["capability"])} isolated N25 native provider qualification', read_seconds=.3)
            require('enrol' in output.lower() or 'grant' in output.lower(), 'N25 route enrolment failed: '+host['redact'](output))
        flush()
        for alias in ('debit', 'effect', 'unrelated'): fund(routes[alias])

        def trigger(name, row, stage, success=False):
            # DELIMITER is a mysql client directive. Only the owned CLI helper executes this
            # bounded real provider fault; all NeedsReview updates remain permitted.
            predicate = f'NEW.CharacterId={actor} AND NEW.MagicSpellId={int(row["spell"])} AND NEW.Stage=\'{stage}\''
            if success: predicate += ' AND NEW.Definition LIKE \'%applied="true"%\''
            statement = '\nDELIMITER $$\n' + mapped(f"CREATE TRIGGER {name} BEFORE UPDATE ON MagicCastingOperations FOR EACH ROW BEGIN IF {predicate} THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='N25 actual provider {stage}'; END IF; END") + '$$\nDELIMITER ;\n'
            sql(statement)
            triggers.append(name)

        def cast(row, stage=None, successful=False):
            for attempt in range(3):
                old = {r[0] for r in state()['operations']}
                output = player.send(command(row), read_seconds=.5)
                flush()
                added = [r for r in state()['operations'] if r[0] not in old]
                report['casts'].append({'alias':row['alias'], 'attempt':attempt+1, 'output':host['redact'](output), 'operations':added})
                require(len(added) == 1, 'N25 cast did not produce exactly one operation: '+host['redact'](output))
                op = added[0]
                payload = ET.fromstring(op[7])
                if stage:
                    if successful and op[1] == 'Completed' and payload.get('applied') == 'false' and attempt < 2: continue
                    require(op[1] == 'NeedsReview' and f'N25 actual provider {stage}' in op[8], 'Requested actual provider failure was not durably quarantined: '+str(op))
                    durable_stage = 'Committed' if successful else 'PaymentMutated'
                    require(op[8].startswith(f'At {durable_stage}: ') and
                            payload.tag == 'Casting' and payload.get('version') == '1' and
                            payload.get('grade') == '1' and
                            payload.get('applied') is None and payload.get('outcome') is None,
                            'Provider failure did not preserve its exact last durable pre-effect receipt: '+str(op))
                    guid = re.search(r'Operation ([0-9a-fA-F-]{36}) requires staff reconciliation', output)
                    require(guid, 'No exact original operation GUID was returned for reconciliation.')
                    costs = [x for x in payload.findall('Cost') if int(x.get('resource')) == int(row['resource'])]
                    require(costs and abs(balance(row) - (float(costs[0].get('before')) - sum(float(x.get('amount')) for x in costs))) < .00001, 'Provider failure refunded, duplicated or failed the exact captured reserve debit.')
                    report['faults'].append({'hex':op[0], 'guid':guid[1], 'stage':stage, 'receipt':op})
                else:
                    if op[1] == 'Completed' and payload.get('applied') == 'false' and attempt < 2: continue
                    require(op[1] == 'Completed' and payload.get('applied') == 'true', 'Unrelated record was not usable during quarantine: '+str(op))
                return op
            raise RuntimeError('Bounded native ordinary failure attempts exhausted.')

        trigger('s6_n25_after_debit', routes['debit'], 'Committed')
        cast(routes['debit'], 'Committed')
        quarantine(routes['alternate'])
        trigger('s6_n25_after_effect', routes['effect'], 'EffectsExecuted', True)
        cast(routes['effect'], 'EffectsExecuted', True)
        lives = sql(f'SELECT HEX(Id),State FROM MagicSpellLifecycles WHERE CreatorId={actor} AND SpellId={int(routes["effect"]["spell"])}')
        require(len(lives) == 1 and lives[0][1] == '0', 'Uncertain successful projection did not retain one exact active lifecycle.')
        claims = sql(f'SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE HEX(LifecycleId)="{lives[0][0]}" ORDER BY Kind')
        owned = {int(k):int(v) for k,v in claims}
        require(2 in owned and 3 in owned and owned[3] != body, 'Uncertain projection did not own a real distinct body/instance.')
        report['projection'] = {'life':lives[0][0], 'instance':owned[2], 'body':owned[3], 'claims':claims}
        focused = player.send('focus 2', read_seconds=.3) + player.send('instances', read_seconds=.3)
        plain = re.sub(r'\x1b\[[0-9;]*m', '', focused)
        require(any(re.search(r'[|│]\s*2\s*[|│]', line) and 'Focused' in line for line in plain.splitlines()), 'Actual secondary focus failed: '+host['redact'](plain))
        quarantine(routes['alternate'])
        player.send('focus primary', read_seconds=.3)
        quarantine(routes['effect'])
        cast(routes['unrelated'])
        flush()
        report.update(status='PASS', saved=state())
        assertions.append('N25: real MySQL errors after debit and actual projection effect quarantine original GUIDs; alternate capability and actual secondary body cannot replay or repay; unrelated record remains usable')
    finally:
        for name in triggers: sql(f'DROP TRIGGER IF EXISTS {name}')
        (Path(receipt['runtime_paths'][-1]) / ('s6-uncertainty-'+phase+'-transcript.txt')).write_text(player.full_transcript(), encoding='utf-8')
        player.close()
