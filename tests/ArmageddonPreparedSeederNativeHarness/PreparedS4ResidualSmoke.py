"""Two bounded residual families; native commands and exact persisted custody, no stock gate edits."""
from pathlib import Path
import re
import time
import xml.etree.ElementTree as ET


def run(host, session, phase, data, mapped, cast, load, flush, effect, held):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    config = data['family_acceptance']
    report = receipt['family_acceptance']
    actor, body = int(data['character']), int(data['body'])
    origin = int(config['origin'])
    routes = {row['key']: row for row in config['routes']}
    report['support_reconciliation'] = data['support_reconciliation']

    def corpse_conserved():
        require(held(report['gear'], report['corpse_body']), 'Exact corpse foreign gear changed custody.')
        restored = sql(f'SELECT gi.Id,rg.RoomId FROM GameItems gi LEFT JOIN Rooms_GameItems rg ON rg.GameItemId=gi.Id WHERE gi.Id={report["corpse"]}')
        report.setdefault('closure_observations',[]).append({'phase':phase,'corpse_room':restored,'instances':sql(f'SELECT Id,BodyId FROM CharacterInstances WHERE Id={report["secondary"]}'),'gear_bodies':sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={report["gear"]}')})
        require(restored == [[str(report['corpse']),str(origin)]], 'Original corpse was not restored to its exact room: '+str(restored))
        require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={report["secondary"]}'), 'Animated secondary persisted after closure.')
        require(sql(f'SELECT Id FROM Bodies WHERE Id={report["corpse_body"]}') == [[str(report['corpse_body'])]], 'Original corpse body was removed.')
        xml = sql(f'SELECT HEX(EffectData) FROM GameItems WHERE Id={report["corpse"]}', (0,))[0][0]
        require(not effect(xml, 'SpellDeadSpeak'), 'Corpse retained dead-speak parent after closure.')

    if config['cases'] == 'pocket':
        run_pocket(host, session, phase, data, cast, flush, held)
        return

    if phase == 'cold':
        flush()
        if config['cases'] == 'voice':
            corpse_conserved()
            assertions.append('Voice: cold restart retains same original corpse/body/foreign gear, no animation or parent replay')
        else:
            for case in report['projections']:
                require(sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{case["life"]}"') == [['3']], 'Cold projection lifecycle was revived.')
                require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={case["instance"]}') and not sql(f'SELECT Id FROM Bodies WHERE Id={case["body"]}'), 'Cold projection retained owned temporary body/instance.')
                if case.get('source'):
                    require(sql(f'SELECT Id FROM GameItems WHERE Id={case["source"]}') == [[str(case['source'])]], 'Cold projection removed borrowed source.')
            assertions.append('Projection: cold completed journals and exact retired temporary graphs remain stable; borrowed Sand source survives')
        report['cold_status'] = 'PASS'
        return

    session.send('drop all', read_seconds=.3)
    session.send(f'goto {origin}', read_seconds=.3)
    for row in routes.values():
        session.send(f'givemerit me {int(row["merit"])}', read_seconds=.3)
        session.send(f'magic casting enrol {actor} {int(row["capability"])} bounded S4 residual acceptance', read_seconds=.3)
    peer = host['helper'].MudSocket(*session.sock.getpeername(), host['SECRETS'])
    try:
        peer.read_for(.5)
        for text in ('l', config['witness']['account']):
            peer.send(text, read_seconds=.5)
        peer.send(host['PASSWORD'], secret=True, read_seconds=.5)
        peer.send('c', read_seconds=.5)
        peer.send('1', read_seconds=2)
        looked = peer.send('look', read_seconds=.5)
        require('S4 family origin' in looked, 'Independent witness did not enter fixture room: '+host['redact'](looked))
        if config['cases'] == 'voice':
            corpse_actor, corpse_body = load(config['recipient_template'], 'sentinel', 'residual-corpse-source')
            session.send(f'item load {host["item_command_reference"](int(config["parcel_prototype"]))}', read_seconds=.3);flush()
            gear = int(sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={int(config["parcel_prototype"])} ORDER BY gi.Id DESC LIMIT 1')[0][0])
            session.send('give blank residual-corpse-source', read_seconds=.3);flush()
            require(held(gear,corpse_body), 'Original foreign gear transfer failed.')
            session.send('kill residual-corpse-source', read_seconds=.5);flush()
            corpses = sql("SELECT gi.Id,HEX(gc.Definition) FROM GameItems gi JOIN GameItemComponents gc ON gc.GameItemId=gi.Id JOIN GameItemComponentProtos p ON p.Id=gc.GameItemComponentProtoId AND p.RevisionNumber=gc.GameItemComponentProtoRevision WHERE p.Type='Corpse' ORDER BY gi.Id DESC",(1,))
            matches = [row for row in corpses if ET.fromstring(row[1]).findtext('OriginalCharacter') == str(corpse_actor)]
            require(len(matches)==1, 'Death did not create exact source corpse.')
            corpse = int(matches[0][0]);report.update(corpse=corpse,corpse_actor=corpse_actor,corpse_body=corpse_body,gear=gear)
            cast('arm.spell.voice_of_remains','corpse')
            instances = sql(f'SELECT Id,CharacterId,BodyId,HEX(EffectData) FROM CharacterInstances WHERE BodyId={corpse_body} AND IsPrimary=0',(3,))
            require(len(instances)==1 and instances[0][1:3]==[str(corpse_actor),str(corpse_body)], 'Voice did not animate exactly original identity/body.')
            secondary = int(instances[0][0]);report.update(secondary=secondary,animation_metadata=instances[0][3])
            metadata = ET.fromstring(instances[0][3]).find('AnimatedCorpse')
            require(metadata is not None and metadata.get('AnchorCharacterId')==str(actor) and metadata.get('CorpseItemId')==str(corpse), 'Voice animation provenance changed.')
            corpse_xml = sql(f'SELECT HEX(EffectData) FROM GameItems WHERE Id={corpse}',(0,))[0][0]
            report['corpse_parent'] = corpse_xml
            parents=effect(corpse_xml,'SpellDeadSpeak')
            require(len(parents)==1 and parents[0].findtext('AnimatedInstanceId')==str(secondary), 'Voice did not attach exact corpse-bound dead-speak parent.')
            # Instance EffectData stores instance metadata; the transient forwarding effect is
            # proven by independent native speech output below, not inferred from that column.
            require(not sql(f'SELECT GameItemId FROM Rooms_GameItems WHERE GameItemId={corpse}') and not sql(f'SELECT GameItemId FROM Bodies_GameItems WHERE GameItemId={corpse}'), 'Animated corpse remained exposed in floor/body custody.')
            assertions.append('Voice: paid exact stock cast animates original corpse identity/body with exact caster/corpse provenance, parent and speech proxy')
            count = sql(f'SELECT COUNT(*) FROM MagicCastingOperations WHERE CharacterId={actor}')[0][0]
            peer.read_for(.5)
            marker = 'S4 remains marker'
            said = session.send('say '+marker,read_seconds=.5)
            heard = peer.read_for(1)
            report.update(speech=host['redact'](said),witness_speech=host['redact'](heard),corpse_parent=corpse_xml)
            require('hollow borrowed voice' in heard and marker.lower() in heard.lower(), 'Witness did not receive native hollow speech relay: '+host['redact'](heard))
            require(sql(f'SELECT COUNT(*) FROM MagicCastingOperations WHERE CharacterId={actor}')[0][0]==count, 'Speech proxy created a new paid operation.')
            assertions.append('Voice: independent native witness hears linked speech through hollow recite output without another casting operation')
            deadline=time.monotonic()+75
            while time.monotonic()<deadline and sql(f'SELECT Id FROM CharacterInstances WHERE Id={secondary}'):
                session.read_for(1)
            flush();corpse_conserved()
            peer.read_for(.3)
            session.send('say S4 after remains closure',read_seconds=.3)
            after=peer.read_for(.5);report['after_closure_speech']=host['redact'](after)
            require('hollow borrowed voice' not in after, 'Retired Voice continued relaying speech.')
            assertions.append('Voice: real deadline retires exact animation/proxy, restores same corpse and foreign gear, and ends speech relay')
        else:
            peer.send('immortal',read_seconds=.3)
            report['projections']=[]
            for key in routes:
                source=None
                if key.endswith('sand_effigy'):
                    session.send(f'item load {host["item_command_reference"](int(config["sand_source_prototype"]))}',read_seconds=.3);flush()
                    sources=sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={int(config["sand_source_prototype"])} ORDER BY gi.Id DESC')
                    require(len(sources)==1,'Sand needs one exact held borrowed source.');source=int(sources[0][0])
                    session.send('dub effigy s4sandborrowedsource',read_seconds=.3);flush()
                    dubs=sql(f"SELECT TargetId,TargetType,Keywords FROM Dubs WHERE CharacterId={actor} AND TargetId={source} AND TargetType='GameItem'")
                    require(len(dubs)==1 and 's4sandborrowedsource' in dubs[0][2].split(), 'Sand source dub did not bind exact original item.')
                cast(key, 's4sandborrowedsource' if source else 'self')
                lives=sql(f'SELECT HEX(Id),State,HEX(Provenance) FROM MagicSpellLifecycles WHERE SpellId={int(routes[key]["spell"])} AND CreatorId={actor} AND Family="native-identity-projection-v1" ORDER BY CreatedUtc DESC LIMIT 1',(2,))
                require(len(lives)==1 and lives[0][1]=='0','Paid projection did not activate its lifecycle.')
                life=lives[0][0]
                claims=sql(f'SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE HEX(LifecycleId)="{life}" ORDER BY Kind')
                ids={int(kind):int(value) for kind,value in claims}
                require(2 in ids and 3 in ids and ids[3]!=body and (source is None or ids.get(0) != source),'Projection ownership claims include original body/source.')
                case={'key':key,'life':life,'instance':ids[2],'body':ids[3],'source':source,'claims':claims,'provenance':lives[0][2]};report['projections'].append(case)
                focused=session.send('focus 2',read_seconds=.3)+session.send('instances',read_seconds=.3)
                plain=re.sub(r'\x1b\[[0-9;]*m','',focused)
                case['focus_output']=host['redact'](plain)
                require(any(re.search(r'[|│]\s*2\s*[|│]',line) and 'Focused' in line for line in plain.splitlines()), 'Native focus did not select exact secondary: '+host['redact'](plain))
                if source:
                    session.send('focus primary',read_seconds=.3)
                    case['trigger_output']=host['redact'](session.send('drop new s4sandborrowedsource',read_seconds=.3));flush()
                    require(not held(source,body),'Native drop did not release exact borrowed Sand source.')
                    reason='7'
                else:
                    before=float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={body}')[0][0])
                    case['trigger_output']=host['redact'](peer.send('wound Admin random cellular 1 0 0',read_seconds=.3))
                    session.read_for(.3);flush()
                    damage=float(sql(f'SELECT IFNULL(SUM(CurrentDamage),0) FROM Wounds WHERE BodyId={body}')[0][0])
                    case['positive_anchor_damage']=damage-before
                    require(damage>before,'Native primary-anchor wound did not produce positive damage.')
                    reason='8'
                state=sql(f'SELECT State,Reason FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')
                require(state and state[0][0] in ('1','3') and state[0][1]==reason,'Actual bound native callback did not request exact retirement reason: '+str(state))
                focused=session.send('instances',read_seconds=.3);case['returned_focus_output']=host['redact'](focused)
                plain=re.sub(r'\x1b\[[0-9;]*m','',focused)
                require(any(re.search(r'[|│]\s*1\s*[|│]',line) and 'Focused' in line for line in plain.splitlines()), 'Native callback did not return focus to primary.')
                deadline=time.monotonic()+75
                while time.monotonic()<deadline and sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')!=[['3']]:
                    session.read_for(1)
                flush()
                require(sql(f'SELECT State,Reason FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')==[['3',reason]], 'Projection did not complete retirement: '+str(sql(f'SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE HEX(Id)="{life}"')))
                require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={ids[2]}') and not sql(f'SELECT Id FROM Bodies WHERE Id={ids[3]}'), 'Projection retained owned temporary body/instance.')
                if 0 in ids:require(not sql(f'SELECT Id FROM GameItems WHERE Id={ids[0]}'),'Sand retained created anchor.')
                require(sql(f'SELECT BodyId FROM Characters WHERE Id={actor}')==[[str(body)]], 'Projection replaced original identity/body.')
                if source:require(sql(f'SELECT Id FROM GameItems WHERE Id={source}')==[[str(source)]], 'Sand deleted borrowed source.')
                case['status']='PASS'
                assertions.append('Projection: '+key+' paid create/focus, native bound callback reason '+reason+(', primary focus retained after manual return' if source else ', callback returns focus to primary')+', exact temporary graph retirement')
                case['focus_scope'] = 'Manual primary focus before Sand source drop; common callback focus-return inherited from focused Unbodied case.' if source else 'Actual focused damage callback returned focus to primary.'
        report['status']='PASS'
    finally:
        (host['OUTPUT']/('residual-witness-'+phase+'.txt')).write_text(peer.full_transcript(),encoding='utf-8')
        peer.close()


def run_pocket(host, session, phase, data, cast, flush, held):
    sql, require, receipt, assertions = (host[k] for k in ('sql','require','receipt','assertions'))
    config=data['family_acceptance'];report=receipt['family_acceptance'];body=int(data['body'])
    def goods():
        return sql(f'SELECT Id,GameItemProtoId,GameItemProtoRevision,Quality,OwnerId,OwnerType FROM GameItems WHERE Id IN ({report["bag"]},{report["child"]}) ORDER BY Id')
    if phase=='cold':
        deadline=time.monotonic()+360
        while time.monotonic()<deadline and sql(f'SELECT State FROM MagicSpellLifecycles WHERE HEX(Id)="{report["life"]}"')!=[['3']]:
            session.read_for(1)
        flush()
        state=sql(f'SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE HEX(Id)="{report["life"]}"')
        report['cold_lifecycle']=state
        require(state and state[0][0]=='3','Unlocated occupied carrier failed real-deadline fallback evacuation: '+str(state))
        require(not sql(f'SELECT Id FROM GameItems WHERE Id={report["carrier"]}'),'Retired pocket carrier remains.')
        require(goods()==report['goods_before'],'Foreign nested goods identity/prototype/revision/quality/legal ownership changed.')
        require(sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={report["bag"]}')==[[str(config['fallback'])]],'Root bag did not reach actual configured fallback.')
        require(sql(f'SELECT ContainerId FROM GameItems WHERE Id={report["child"]}')==[[str(report['bag'])]],'Nested child left its exact original bag.')
        require(not sql(f'SELECT EntityId FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId IN ({report["bag"]},{report["child"]})'),'Pocket claimed foreign goods.')
        report.update(status='PASS',cold_status='PASS')
        assertions.append('Pocket: actual cold unlocated-carrier fallback selection, native nonempty nested evacuation, same foreign goods/ownership and completed carrier retirement')
        return
    session.send('drop all',read_seconds=.2)
    session.send(f'goto {int(config["origin"])}',read_seconds=.3)
    route=config['routes'][0]
    session.send(f'givemerit me {int(route["merit"])}',read_seconds=.2)
    session.send(f'magic casting enrol {int(data["character"])} {int(route["capability"])} bounded Pocket fallback acceptance',read_seconds=.2)
    def load_item(proto):
        session.send(f'item load {host["item_command_reference"](int(proto))}',read_seconds=.2);flush()
        rows=sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={int(proto)} ORDER BY gi.Id DESC LIMIT 1')
        require(len(rows)==1,'Native held fixture load failed.');return int(rows[0][0])
    source=load_item(config['parcel_prototype'])
    cast('arm.spell.folded_pocket','blank')
    lives=sql(f'SELECT HEX(l.Id),e.EntityId,HEX(l.Provenance),l.DeadlineUtc FROM MagicSpellLifecycles l JOIN MagicSpellOwnedEntities e ON e.LifecycleId=l.Id WHERE l.Family="native-item-pocket-v1" AND l.CreatorId={int(data["character"])} ORDER BY l.CreatedUtc DESC LIMIT 1',(2,))
    require(len(lives)==1,'Paid pocket did not create exactly claimed carrier.')
    life,carrier,provenance,deadline=lives[0];carrier=int(carrier)
    require(held(carrier,body),'Created pocket did not enter native hand custody.')
    session.send('drop new blank',read_seconds=.2)
    bag=load_item(config['bag_prototype']);session.send('drop new s4residualbag',read_seconds=.2)
    child=load_item(config['parcel_prototype'])
    session.send('put blank s4residualbag',read_seconds=.2)
    session.send('get s4residualbag',read_seconds=.2)
    report.update(life=life,carrier=carrier,bag=bag,child=child,source=source,provenance=provenance,deadline=deadline)
    # Carrier keyword comes from its exact native prototype, not a private replacement definition.
    carrier_proto=int(sql(f'SELECT GameItemProtoId FROM GameItems WHERE Id={carrier}')[0][0])
    keywords=sql(f'SELECT Keywords FROM GameItemProtos WHERE Id={carrier_proto} ORDER BY RevisionNumber DESC LIMIT 1')[0][0].split()
    selector=next((word for word in keywords if 'pocket' in word.lower()),keywords[0])
    report['put_output']=host['redact'](session.send('put s4residualbag '+selector,read_seconds=.2));flush()
    require(sql(f'SELECT ContainerId FROM GameItems WHERE Id={bag}')==[[str(carrier)]] and sql(f'SELECT ContainerId FROM GameItems WHERE Id={child}')==[[str(bag)]],'Native pocket did not accept exact nested foreign graph: '+report['put_output'])
    require(held(carrier,body),'Pocket fault preparation requires one body-held carrier.')
    report['goods_before']=goods();report['first_status']='PASS_PREPARED_FOR_COLD_FAULT'
    assertions.append('Pocket: paid exact stock grade-one carrier holds ordinary native bag/child with original ownership and real deadline')


def prepare_cold(host, data):
    if data.get('family_acceptance',{}).get('cases')!='pocket':return
    sql,require,receipt=(host[k] for k in ('sql','require','receipt'))
    report=receipt['family_acceptance'];carrier=int(report['carrier']);body=int(data['body'])
    require(sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={carrier}')==[[str(body)]], 'Cold fault lost exact sole source custodian.')
    require(not sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={carrier}') and sql(f'SELECT ContainerId FROM GameItems WHERE Id={carrier}')==[['NULL']], 'Cold fault needs exclusively body-held carrier.')
    require(all(row.get('stopped') and row.get('graceful') for row in host['processes']), 'Cold fault requires all first-boot MUD processes gracefully stopped.')
    sql(f'DELETE FROM Bodies_GameItems WHERE BodyId={body} AND GameItemId={carrier}')
    require(not sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={carrier}'), 'Cold fault failed to unlocate exact carrier.')
    report['cold_fault']='After graceful first shutdown only exact carrier body membership removed in owned disposable database; no lifecycle, deadline, goods or stock modification.'
