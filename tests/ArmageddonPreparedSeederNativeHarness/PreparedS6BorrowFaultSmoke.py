"""One exact installed borrowed-corpse retirement DELETE failure and recovery."""
import re
import time
import xml.etree.ElementTree as ET
from pathlib import Path


def run(host, admin, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    cfg = data['s6_borrow_fault']
    actor, body, room = (int(cfg[k]) for k in ('actor', 'body', 'room'))
    routes = {x['alias']: x for x in cfg['routes']}
    route = routes['borrow']
    report = receipt.setdefault('s6_borrow_fault', {'status': 'FAIL', 'limits': cfg['limits']})
    player = None
    trigger = 's6_borrow_retirement_delete'

    def flush():
        output = admin.send('impdebug flush', read_seconds=.4)
        deadline = time.monotonic()+6
        while 'All queued saves have been flushed' not in output and time.monotonic() < deadline:
            output += admin.read_for(.3)
        require('All queued saves have been flushed' in output, 'Borrow fault independent administrator flush failed: '+host['redact'](output))

    def state():
        return {
            'caster_body': sql(f'SELECT BodyId FROM Characters WHERE Id={actor}'),
            'operations': sql(mapped(f'SELECT HEX(Id),Stage,HEX(Definition),HEX(Diagnostic) FROM MagicCastingOperations WHERE CharacterId={actor} ORDER BY CreatedUtc,Id'), (2, 3)),
            'resources': sql(mapped(f'SELECT MagicResourceId,Amount FROM CharactersMagicResources WHERE CharacterId={actor} ORDER BY MagicResourceId')),
            'corpse_identity': sql(f"SELECT Id,BodyId FROM Characters WHERE Id={report['corpse_actor']}"),
            'corpse_item': sql(f"SELECT Id FROM GameItems WHERE Id={report['corpse']}"),
            'corpse_component': sql(f"SELECT HEX(gc.Definition) FROM GameItemComponents gc JOIN GameItemComponentProtos p ON p.Id=gc.GameItemComponentProtoId AND p.RevisionNumber=gc.GameItemComponentProtoRevision WHERE gc.GameItemId={report['corpse']} AND p.Type='Corpse'", (0,)),
            'corpse_rooms': sql(f"SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={report['corpse']}"),
            'gear': sql(f"SELECT BodyId,GameItemId FROM Bodies_GameItems WHERE GameItemId={report['gear']}"),
            'instances': sql(f"SELECT Id,CharacterId,BodyId,CAST(IsEmbodied AS UNSIGNED),HEX(EffectData) FROM CharacterInstances WHERE BodyId={report['corpse_body']} AND IsPrimary=0 ORDER BY Id", (4,)),
            'corpse_effects': sql(f"SELECT HEX(EffectData) FROM GameItems WHERE Id={report['corpse']}", (0,))}

    def conserved(current):
        require(current['caster_body'] == [[str(body)]] and current['corpse_identity'] == [[str(report['corpse_actor']),str(report['corpse_body'])]] and
                current['corpse_item'] == [[str(report['corpse'])]] and current['gear'] == [[str(report['corpse_body']),str(report['gear'])]],
                'Borrow failure lost/replaced original corpse/caster/body/foreign item: '+str(current))
        require(len(current['corpse_component']) == 1 and ET.fromstring(current['corpse_component'][0][0]).findtext('OriginalCharacter') == str(report['corpse_actor']), 'Borrow failure changed the exact corpse component identity.')
        corpses = sql("SELECT gi.Id,HEX(gc.Definition) FROM GameItems gi JOIN GameItemComponents gc ON gc.GameItemId=gi.Id JOIN GameItemComponentProtos p ON p.Id=gc.GameItemComponentProtoId AND p.RevisionNumber=gc.GameItemComponentProtoRevision WHERE p.Type='Corpse'", (1,))
        require([int(x[0]) for x in corpses if ET.fromstring(x[1]).findtext('OriginalCharacter') == str(report['corpse_actor'])] == [report['corpse']], 'Borrow failure duplicated the original corpse item.')

    def stable(current):
        # Corpse component decay is native time progression; preserve observations without equating timers.
        return {k: current[k] for k in ('caster_body','operations','resources','corpse_identity','corpse_item','corpse_rooms','gear','instances')}

    def balance():
        rows = sql(mapped(f"SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={int(route['resource'])}"))
        return float(rows[0][0]) if rows else 0

    if phase == 'cold':
        current = state()
        report['cold_before_recovery'] = current
        conserved(current)
        require(current['operations'] == report['saved']['operations'] and current['resources'] == report['saved']['resources'], 'Cold Borrow failure replayed/refunded paid activation.')
        for attempt in range(2):
            output = admin.send(f"instance retire {report['secondary']}", read_seconds=.5)
            flush()
            report.setdefault('cold_recovery', []).append({'attempt': attempt+1, 'output': host['redact'](output), 'state': state()})
        final = state()
        conserved(final)
        require(not final['instances'] and final['corpse_rooms'] == [[str(room)]], 'Borrow DELETE failure did not recover the exact original corpse floor custody and retire its secondary across cold/repeated recovery: '+str(final))
        require(stable(report['cold_recovery'][0]['state']) == stable(final) and final['operations'] == report['saved']['operations'] and final['resources'] == report['saved']['resources'], 'Repeated same-instance Borrow recovery was not idempotent or replayed/refunded.')
        report.update(status='PASS', cold_status='PASS', recovered=final)
        assertions.append('S6 Borrow fault: cold and twice same-instance recovery retain one original corpse/body/foreign item, original paid GUID/reserve, floor custody and no secondary/replay/refund')
        return

    try:
        require(sql(f'SELECT BodyId FROM Characters WHERE Id={actor}') == [[str(body)]], 'Borrow fixture has no exact original canonical caster body.')
        name = sql(f"SELECT UniqueName FROM NPCTemplates WHERE Id={int(cfg['npc_template'])}")[0][0]
        admin.send('npc load '+name, read_seconds=.5)
        flush()
        npc = sql(f"SELECT c.Id,c.BodyId FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE n.TemplateId={int(cfg['npc_template'])} ORDER BY c.Id DESC LIMIT 1")
        require(len(npc) == 1, 'Native installed Clay Sentinel source did not load.')
        report.update(corpse_actor=int(npc[0][0]), corpse_body=int(npc[0][1]))
        admin.send('dub identity sentinel s6-borrow-source', read_seconds=.3)
        admin.send(f"item load {host['item_command_reference'](int(cfg['item_prototype']))}", read_seconds=.3)
        flush()
        gear = sql(f"SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={int(data['body'])} AND gi.GameItemProtoId={int(cfg['item_prototype'])} ORDER BY gi.Id DESC LIMIT 1")
        require(len(gear) == 1, 'Native exact foreign item did not load.')
        report['gear'] = int(gear[0][0])
        admin.send('give blank s6-borrow-source', read_seconds=.3)
        flush()
        require(sql(f"SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={report['gear']}") == [[str(report['corpse_body'])]], 'Foreign gear was not given to the actual source before death.')
        admin.send('kill s6-borrow-source', read_seconds=.5)
        flush()
        corpses = sql("SELECT gi.Id,HEX(gc.Definition) FROM GameItems gi JOIN GameItemComponents gc ON gc.GameItemId=gi.Id JOIN GameItemComponentProtos p ON p.Id=gc.GameItemComponentProtoId AND p.RevisionNumber=gc.GameItemComponentProtoRevision WHERE p.Type='Corpse' ORDER BY gi.Id", (1,))
        matches = [x for x in corpses if ET.fromstring(x[1]).findtext('OriginalCharacter') == str(report['corpse_actor'])]
        require(len(matches) == 1, 'Native death did not yield exactly the original corpse.')
        report['corpse'] = int(matches[0][0])
        player = host['helper'].MudSocket('127.0.0.1', int(receipt['processes'][-1]['endpoint'].rsplit(':',1)[1]), host['SECRETS'])
        player.read_for(.5)
        for command in ('l', cfg['account']): player.send(command, read_seconds=.5)
        player.send(host['PASSWORD'], secret=True, read_seconds=.5)
        for command in ('c','1'): player.send(command, read_seconds=.7)
        require(sql(f'SELECT CAST(c.IsAdminAvatar AS UNSIGNED),g.AuthorityLevel FROM Characters c JOIN Accounts a ON a.Id=c.AccountId JOIN AuthorityGroups g ON g.Id=a.AuthorityGroupId WHERE c.Id={actor}') == [['0','2']], 'Borrow subject is not an ordinary non-admin Player.')
        for selected in routes.values():
            output = admin.send(f"magic casting enrol {actor} {int(selected['capability'])} isolated installed Borrow retirement fault", read_seconds=.3)
            require('enrol' in output.lower() or 'grant' in output.lower(), 'Borrow ordinary subject enrolment failed: '+host['redact'](output))
        flush()
        require(sql(mapped(f"SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={actor} AND MagicSpellId={int(route['spell'])}")) == [['1']], 'Borrow subject did not receive its exact canonical grade-one acquisition.')
        for attempt in range(3):
            current = balance()
            if current < 100:
                output = player.send(f"{route['invoked']} gather {route['capability']} draw {100-current:.4f}", read_seconds=.4)
                deadline = time.monotonic()+18
                while time.monotonic() < deadline and balance() < 100: output += player.read_for(.3)
                flush()
                require(balance() == 100, 'Borrow paid native funding did not reach100: '+host['redact'](output))
            before = {x[0] for x in state()['operations']}
            output = player.send(f"{route['invoked']} cast \"{route['name']}\" grade 1 on corpse via {route['capability']}", read_seconds=.7)
            flush()
            added = [x for x in state()['operations'] if x[0] not in before]
            report.setdefault('activation_attempts', []).append({'output':host['redact'](output), 'operations':added})
            require(len(added) == 1 and added[0][1] == 'Completed', 'Borrow activation did not durably complete before the terminal fault: '+str(added))
            if ET.fromstring(added[0][2]).get('applied') == 'true': break
        else: raise RuntimeError('Three bounded paid ordinary Borrow activation failures; retirement fault not reached.')
        active = state()
        conserved(active)
        require(len(active['instances']) == 1 and not active['corpse_rooms'], 'Borrow did not retain one exact hidden original corpse and actual secondary before fault.')
        secondary = int(active['instances'][0][0])
        metadata = ET.fromstring(active['instances'][0][4]).find('PossessedCorpse')
        require(metadata is not None and metadata.get('AnchorCharacterId') == str(actor) and metadata.get('CorpseItemId') == str(report['corpse']) and metadata.get('OriginalBodyId') == str(report['corpse_body']), 'Borrow changed exact caster/corpse metadata.')
        report.update(secondary=secondary, active=active)
        # The nontransactional observer records a SIGNAL attempt even though the real DELETE rolls back.
        sql('CREATE TABLE HarnessS6BorrowDeleteHits (Id BIGINT NOT NULL AUTO_INCREMENT PRIMARY KEY,InstanceId BIGINT NOT NULL) ENGINE=MyISAM')
        statement = f"\nDELIMITER $$\nCREATE TRIGGER {trigger} BEFORE DELETE ON CharacterInstances FOR EACH ROW BEGIN IF OLD.Id={secondary} AND OLD.CharacterId={report['corpse_actor']} AND OLD.BodyId={report['corpse_body']} AND OLD.IsPrimary=0 THEN INSERT INTO HarnessS6BorrowDeleteHits (InstanceId) VALUES (OLD.Id); SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT='S6 Borrow actual retirement DELETE'; END IF; END$$\nDELIMITER ;\n"
        sql(statement)
        deadline = time.monotonic()+85
        while time.monotonic() < deadline and not sql('SELECT InstanceId FROM HarnessS6BorrowDeleteHits'):
            player.read_for(.7)
        report['provider_hits'] = sql('SELECT InstanceId FROM HarnessS6BorrowDeleteHits ORDER BY Id')
        require(report['provider_hits'] and all(x == [str(secondary)] for x in report['provider_hits']), 'Actual Borrow terminal DELETE provider fault was not reached.')
        hold = state()
        report['hold'] = hold
        conserved(hold)
        require(len(hold['instances']) == 1 and int(hold['instances'][0][0]) == secondary, 'Rejected Borrow DELETE lost or duplicated its original secondary row.')
        require(hold['operations'] == active['operations'] and hold['resources'] == active['resources'], 'Borrow terminal failure replayed/refunded paid activation.')
        alternate = routes['alternate']
        output = player.send(f"{alternate['invoked']} cast \"{alternate['name']}\" grade 1 on corpse via {alternate['capability']}", read_seconds=.5)
        flush()
        after = state()
        report['alternate_after_hold'] = {'output':host['redact'](output), 'state':after}
        conserved(after)
        require(after['operations'] == hold['operations'] and after['resources'] == hold['resources'] and [x[:3] for x in after['instances']] == [x[:3] for x in hold['instances']], 'Alternate Borrow route duplicated instance/payment during terminal hold.')
        sql(f'DROP TRIGGER {trigger}')
        for attempt in range(2):
            output = admin.send(f'instance retire {secondary}', read_seconds=.5)
            flush()
            report.setdefault('live_recovery', []).append({'attempt':attempt+1, 'output':host['redact'](output), 'state':state()})
        report.update(status='AWAITING_COLD', saved=state())
        conserved(report['saved'])
        recovered = not report['saved']['instances'] and report['saved']['corpse_rooms'] == [[str(room)]] and stable(report['live_recovery'][0]['state']) == stable(report['saved'])
        report['live_recovery_status'] = 'PASS' if recovered else 'NOT_RECOVERED_PENDING_COLD'
        assertions.append('S6 Borrow fault: real exact-secondary DELETE rejection; same original corpse/body/foreign item and paid activation survive hold and alternate attempt; command outcomes retained without assuming recovery')
    finally:
        sql(f'DROP TRIGGER IF EXISTS {trigger}')
        if player is not None:
            (Path(receipt['runtime_paths'][-1])/('s6-borrow-fault-'+phase+'.txt')).write_text(player.full_transcript(), encoding='utf-8')
            player.close()
