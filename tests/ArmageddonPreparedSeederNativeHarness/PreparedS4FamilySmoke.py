"""Bounded native mission/corpse acceptance; exact installed spells, disposable routes only."""
from pathlib import Path
import time
import importlib.util
import xml.etree.ElementTree as ET


def run(host, session, phase, data, mapped):
    sql, require, receipt, assertions = (host[k] for k in ('sql', 'require', 'receipt', 'assertions'))
    config = data['family_acceptance']
    if config.get('cases') == 'kindle':
        spec = importlib.util.spec_from_file_location('kindle_acceptance', host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS4KindleSmoke.py')
        module = importlib.util.module_from_spec(spec); spec.loader.exec_module(module)
        module.run(host, session, phase, data, mapped)
        return
    corpse_only = config.get('cases', 'all') == 'corpse'
    source = config['source']
    require(Path(source['path']).resolve() == host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/Program.S4FamilyFixture.cs'
            and host['sha'](source['path']) == source['sha256'], 'Family preparation source pin differs.')
    routes = {r['key']: r for r in config['routes']}
    actor, body = int(data['character']), int(data['body'])
    origin, destination = int(config['origin']), int(config['destination'])

    def flush():
        output = session.send('impdebug flush', read_seconds=.5)
        deadline = time.monotonic()+6
        while 'All queued saves have been flushed' not in output and time.monotonic() < deadline:
            output += session.read_for(.5)
        require('All queued saves have been flushed' in output, 'Family flush acknowledgement failed: '+host['redact'](output))
        return output

    def effect(xml, name):
        root = ET.fromstring(xml)
        return [e.find('Effect') for e in root.iter('Effect') if e.findtext('Type') == name]

    def npc(template):
        flush()
        rows = sql(f'SELECT c.Id,c.BodyId,c.Location,HEX(c.EffectData) FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE n.TemplateId={int(template)} ORDER BY c.Id DESC LIMIT 1', (3,))
        require(len(rows) == 1, 'Missing native NPC template '+str(template))
        return rows[0]

    def balance(row):
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={int(row["resource"])}),0)'))[0][0])

    def cast(key, target, flush_after=True, allow_failure=False, attempt=1):
        row = routes[key]
        flush()
        current = balance(row)
        if current < 99:
            session.send(f'{row["invoked"]} gather {int(row["capability"])} draw {100-current:.4f}', read_seconds=.5)
            deadline = time.monotonic()+18
            while time.monotonic() < deadline and balance(row) < 99:
                session.read_for(.5)
            require(balance(row) >= 99, 'Native finite gathering did not fund '+key)
        before = {r[0] for r in sql(f'SELECT HEX(Id) FROM MagicCastingOperations WHERE CharacterId={actor}')}
        output = session.send(f'{row["invoked"]} cast "{row["name"]}" grade 1 on {target} via {int(row["capability"])}', read_seconds=.5)
        if flush_after:
            output += flush()
        added = [r for r in sql(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor} AND MagicSpellId={int(row["spell"])}', (2,)) if r[0] not in before]
        report.setdefault('casts', []).append({'key':key, 'output':host['redact'](output), 'operations':added})
        if len(added) == 1 and added[0][1] == 'Completed' and ET.fromstring(added[0][2]).get('applied') == 'false' and attempt < 3:
            # A completed ordinary failed casting can be retried as a new paid operation.
            # Quarantined or ambiguous operations never enter this retry.
            return cast(key, target, flush_after, allow_failure, attempt+1)
        if allow_failure and (len(added) != 1 or added[0][1] != 'Completed'):
            return None
        require(len(added) == 1 and added[0][1] == 'Completed' and ET.fromstring(added[0][2]).get('applied') == 'true', 'Exact paid family cast did not apply: '+key+' '+host['redact'](output)+' '+str(added))
        return added[0][0]

    def load(template, selector, alias):
        name = sql(f'SELECT UniqueName FROM NPCTemplates WHERE Id={int(template)}')[0][0]
        output = session.send('npc load '+name, read_seconds=.5)
        row = npc(template)
        session.send(f'dub identity {selector} {alias}', read_seconds=.3)
        report.setdefault('fixtures', []).append({'template':template,'actor':row[0],'body':row[1], 'output':host['redact'](output)})
        return int(row[0]), int(row[1])

    def held(item, owner_body):
        return sql(f'SELECT GameItemId FROM Bodies_GameItems WHERE BodyId={int(owner_body)} AND GameItemId={int(item)}') == [[str(item)]]

    if config.get('cases') in ('projection', 'voice', 'pocket'):
        report = receipt.setdefault('family_acceptance', {'status':'FAIL', 'cases':config['cases'], 'scope':'Bounded installed-stock lifecycle sample; disposable admissions; stock gates disabled.'})
        spec = importlib.util.spec_from_file_location('s4_residual', host['REPO'] / 'tests/ArmageddonPreparedSeederNativeHarness/PreparedS4ResidualSmoke.py')
        module = importlib.util.module_from_spec(spec)
        spec.loader.exec_module(module)
        module.run(host, session, phase, data, mapped, cast, load, flush, effect, held)
        return

    if phase == 'cold':
        report = receipt['family_acceptance']
        flush()
        if report.get('courier',{}).get('status') == 'PASS':
            require(held(report['parcel'], report['recipient_body']), 'Cold restart changed delivered parcel custody.')
        require(held(report['corpse_gear'], report['corpse_body']), 'Cold restart changed exact corpse foreign gear.')
        require(sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={report["corpse"]}') == [[str(origin)]], 'Cold restart lost original corpse floor custody.')
        if report.get('possession_instance'):
            require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={report["possession_instance"]}'), 'Cold restart revived retired corpse possession.')
        if report.get('outsider',{}).get('status') == 'PASS':
            current = npc(config['outsider_template'])
            bonds = effect(current[3], 'SpellNpcControl')
            require(len(bonds) == 1 and bonds[0].findtext('ControlLost') == 'true', 'Cold restart rerolled or lost durable Outsider control loss.')
            assertions.append('S4 family: cold Outsider loss bit retained without reroll')
        report['cold_status'] = 'PASS' if not report.get('failed_cases') else 'PASS_SCOPED_WITH_RECORDED_FIRST_FAILURES'
        assertions.append('S4 family: cold qualified custody/corpse gear/original corpse and retired possession retained')
        require(not report.get('failed_cases'), 'Recorded first-boot family failures remain: '+str(report.get('failed_cases')))
        return

    report = receipt['family_acceptance'] = {'status':'FAIL','scope':('Exact Borrow the Dead grade-one possession/closure only' if corpse_only else 'Five exact installed grade-one spells')+' through separate disposable admissions; original package/Class/use/craft/Release gates disabled.', 'cases':config.get('cases','all'), 'random_scope':'Native RNG outcomes recorded; Outsider at most100 authorised follow orders, one loss opportunity per accepted order. No repeated successful campaign.'}
    session.send('drop all', read_seconds=.3)
    for row in routes.values():
        if corpse_only and row['key'] != 'arm.spell.borrow_the_dead':
            continue
        session.send(f'givemerit me {int(row["merit"])}', read_seconds=.3)
        session.send(f'magic casting enrol {actor} {int(row["capability"])} bounded S4 family acceptance', read_seconds=.3)
    if not corpse_only:
        session.send(f'goto {destination}', read_seconds=.3)
        session.send('peace permanent',read_seconds=.3)
        session.send(f'goto {origin}', read_seconds=.5)
        session.send('peace permanent',read_seconds=.3)
        quarry, quarry_body = load(config['quarry_template'], 'mount', 'family-quarry')
        cast('arm.spell.seeking_wind', 'family-quarry')
        seeker = npc(config['seek_template'])
        moved = session.send('force family-quarry north', read_seconds=.5)
        report['quarry_movement'] = host['redact'](moved)
        deadline = time.monotonic()+45
        samples=[]
        while time.monotonic() < deadline:
            session.read_for(2)
            row = npc(config['seek_template']); samples.append({'location':int(row[2]), 'effects':row[3]})
            if int(row[2]) == destination:
                break
        report['seek']={'actor':int(seeker[0]), 'samples':samples,
            'target_location':sql(f'SELECT Location FROM Characters WHERE Id={quarry}'),
            'tracks':sql(f'SELECT Id,CharacterId,CharacterInstanceId,RoomId,ToDirectionExitId,TrackIntensityVisual,TrackIntensityOlfactory FROM Tracks WHERE CharacterId={quarry} ORDER BY Id'),
            'terrain':sql(f'SELECT Id,CanHaveTracks,TrackIntensityMultiplierVisual,TrackIntensityMultiplierOlfactory FROM Terrains WHERE Id={int(config["trackable_terrain"])}')}
        if samples and samples[-1]['location'] == destination and report['seek']['target_location'] == [[str(destination)]]:
            report['seek']['status']='PASS'
            assertions.append('S4 family: paid exact Seeking Wind followed its authorised quarry along one actual native track/exit')
        else:
            report['seek']['status']='FAIL'
            report.setdefault('failed_cases',[]).append('Seeking Wind did not follow the exact quarry native track within45 seconds.')
        session.send(f'goto {destination}', read_seconds=.5)
        recipient, recipient_body = load(config['recipient_template'], 'sentinel', 'family-recipient')
        haunt_attempts=[]
        for attempt in range(3):
            cast('arm.spell.haunting_shape', 'family-quarry')
            deadline = time.monotonic()+18
            while time.monotonic() < deadline:
                session.read_for(1)
                haunt = npc(config['haunt_template'])
                missions=effect(haunt[3], 'SpellNpcMission')
                if missions and missions[0].findtext('Phase') in ('haunting','complete'):
                    break
            target_xml=sql(f'SELECT HEX(EffectData) FROM Characters WHERE Id={quarry}', (0,))[0][0]
            haunt_attempts.append({'attempt':attempt+1,'actor':int(haunt[0]),'effects':haunt[3],'target_effects':target_xml})
            if missions and missions[0].findtext('Phase') == 'haunting' and missions[0].findtext('FearParentIdentity') and 'SpellFear' in target_xml:
                break
            # A native resistance completes/releases the mission without fear. Preserve that expected
            # outcome and permit at most two additional paid samples; never replay an uncertain cast.
            if not missions or missions[0].findtext('Phase') != 'complete' or 'SpellFear' in target_xml:
                break
        report['haunt']={'attempts':haunt_attempts}
        if missions and missions[0].findtext('Phase') == 'haunting' and missions[0].findtext('FearParentIdentity') and 'SpellFear' in target_xml:
            report['haunt']['status']='PASS'
            assertions.append('S4 family: paid Haunt reached its native resistance branch and saved exact target fear-parent identity/child')
        else:
            report['haunt']['status']='FAIL'
            report.setdefault('failed_cases',[]).append('Haunt did not retain an exact native fear parent/child; resistance or application needs inspection.')
        session.send(f'item load {host["item_command_reference"](int(config["parcel_prototype"]))}', read_seconds=.3)
        flush()
        parcels=sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={int(config["parcel_prototype"])} ORDER BY gi.Id DESC')
        require(len(parcels)==1, 'Courier sample needs exactly one newly loaded held parcel.')
        parcel=int(parcels[0][0]); cast('arm.spell.wind_courier', 'blank to family-recipient')
        deadline=time.monotonic()+22
        while time.monotonic()<deadline and not held(parcel,recipient_body):
            session.read_for(1);flush()
        courier=npc(config['courier_template']);missions=effect(courier[3],'SpellNpcMission')
        report.update(parcel=parcel,recipient=recipient,recipient_body=recipient_body,courier={'actor':int(courier[0]),'effects':courier[3]})
        delivered=held(parcel,recipient_body) and not held(parcel,body) and not held(parcel,int(courier[1])) and missions and missions[0].findtext('Phase') in ('return','complete')
        if delivered:
            report['courier']['status']='PASS'
            assertions.append('S4 family: paid Courier picked up and delivered exactly one existing parcel to its authorised recipient, no duplicate/copy')
        else:
            report['courier']['status']='FAIL'
            report['courier']['positions']=sql(f'SELECT Id,Location,RoomLayer,State FROM Characters WHERE Id IN ({actor},{int(courier[0])},{recipient})')
            report['courier']['live_inventory']={who:host['redact'](session.send(f'prog execute {int(config["can_get_prog"])} {who}',read_seconds=.5)) for who in ('self','courier')}
            report['courier']['inventory_output']=host['redact'](session.send('inventory',read_seconds=.5))
            # Diagnostic only, after the mission assertion failed: never count this manual give as pickup.
            report['courier']['manual_give_diagnostic']=host['redact'](session.send('give blank courier',read_seconds=.5))
            flush()
            report['courier']['custody_after_diagnostic']=sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={parcel}')
            report.setdefault('failed_cases',[]).append('Courier native pickup/delivery failed; manual-give diagnostic is not qualifying evidence.')
            session.send('drop all',read_seconds=.3)
        cast('arm.spell.call_outsider','here')
        outsider=npc(config['outsider_template']); outcomes=[]
        for attempt in range(100):
            output=session.send('command outsider follow',read_seconds=.2);flush()
            outsider=npc(config['outsider_template']); bonds=effect(outsider[3],'SpellNpcControl')
            outcomes.append({'attempt':attempt+1,'output':host['redact'](output),'control':ET.tostring(bonds[0],encoding='unicode') if bonds else outsider[3]})
            if bonds and bonds[0].findtext('ControlLost')=='true':break
        report['outsider']={'actor':int(outsider[0]),'outcomes':outcomes,'status':'PENDING'}
        require(bonds and bonds[0].findtext('ControlLost')=='true','Outsider did not lose control within100 bounded native5%-per-grade opportunities; sample remains inconclusive.')
        session.send('command outsider stop',read_seconds=.2);flush();after=effect(npc(config['outsider_template'])[3],'SpellNpcControl')
        require(after and after[0].findtext('ControlLost')=='true' and after[0].findtext('Intent')=='none','Lost Outsider accepted a later order or reset control.')
        assertions.append('S4 family: paid Outsider lost control at its configured native opportunity, persisted loss and refused subsequent orders')
        report['outsider']['status']='PASS'
    session.send(f'goto {origin}',read_seconds=.5)
    corpse_actor,corpse_body=load(config['recipient_template'],'sentinel','family-corpse-source')
    session.send(f'item load {host["item_command_reference"](int(config["parcel_prototype"]))}',read_seconds=.3);flush()
    gear=int(sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={int(config["parcel_prototype"])} ORDER BY gi.Id DESC LIMIT 1')[0][0])
    session.send('give blank family-corpse-source',read_seconds=.3);flush()
    require(held(gear,corpse_body),'Corpse source did not receive exact foreign gear before death.')
    session.send('kill family-corpse-source',read_seconds=.5);flush()
    corpses=sql("SELECT gi.Id,HEX(gc.Definition) FROM GameItems gi JOIN GameItemComponents gc ON gc.GameItemId=gi.Id JOIN GameItemComponentProtos p ON p.Id=gc.GameItemComponentProtoId AND p.RevisionNumber=gc.GameItemComponentProtoRevision WHERE p.Type='Corpse' ORDER BY gi.Id DESC",(1,))
    matches=[r for r in corpses if ET.fromstring(r[1]).findtext('OriginalCharacter')==str(corpse_actor)]
    require(len(matches)==1,'Native death did not produce exactly the source corpse.')
    corpse=int(matches[0][0])
    report.update(corpse=corpse,corpse_body=corpse_body,corpse_actor=corpse_actor,corpse_gear=gear)
    if cast('arm.spell.borrow_the_dead','corpse',flush_after=False,allow_failure=True) is None:
        report['borrow']={'status':'FAIL','reason':'Paid operation quarantined; exact exception captured in owned native server log.'}
        report.setdefault('failed_cases',[]).append('Borrow the Dead paid operation needs staff review; possession/closure unqualified.')
        return
    instances=sql(f'SELECT Id,CharacterId,BodyId,HEX(EffectData) FROM CharacterInstances WHERE BodyId={corpse_body} AND IsPrimary=0',(3,))
    require(len(instances)==1,'Borrow did not create one exact secondary corpse instance.')
    secondary=int(instances[0][0]);metadata=ET.fromstring(instances[0][3]).find('PossessedCorpse')
    require(int(instances[0][1])==corpse_actor and metadata is not None and metadata.get('AnchorCharacterId')==str(actor) and metadata.get('CorpseItemId')==str(corpse) and metadata.get('OriginalBodyId')==str(corpse_body),'Borrow changed original identity/body or caster anchor provenance.')
    report.update(corpse=corpse,corpse_body=corpse_body,corpse_actor=corpse_actor,corpse_gear=gear,possession_instance=secondary,possession_metadata=instances[0][3])
    deadline=time.monotonic()+75
    while time.monotonic()<deadline and sql(f'SELECT Id FROM CharacterInstances WHERE Id={secondary}'):
        session.read_for(1)
    flush()
    require(not sql(f'SELECT Id FROM CharacterInstances WHERE Id={secondary}') and held(gear,corpse_body) and sql(f'SELECT Id FROM GameItems WHERE Id={corpse}')==[[str(corpse)]], 'Borrow closure did not retire exact secondary and conserve original corpse/body/gear.')
    restored=sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={corpse}')
    report['restored_corpse_rooms']=restored
    require(restored==[[str(origin)]], 'Borrow closure did not restore exact corpse floor custody: '+str(restored))
    report['borrow']={'status':'PASS'}
    assertions.append('S4 family: paid Borrow entered exact original corpse identity/body with separate caster anchor; deadline restored same corpse/foreign gear and retired secondary')
    session.send(f'goto {origin}',read_seconds=.3);flush()
    report['status']='FAIL' if report.get('failed_cases') else 'PASS'
