"""Exact paid installed Kindle capacity sample; native equipment, cold retention and expiry."""
import re,time,xml.etree.ElementTree as ET

def run(host,session,phase,data,mapped):
    sql,require,receipt,assertions=(host[k] for k in ('sql','require','receipt','assertions'))
    config=data['family_acceptance']; route=config['routes'][0]
    actor,body=int(data['character']),int(data['body'])
    resource=int(route['resource']); proto=int(config['kindle_prototype'])
    def flush():
        out=session.send('impdebug flush',read_seconds=.4); stop=time.monotonic()+6
        while 'All queued saves have been flushed' not in out and time.monotonic()<stop:out+=session.read_for(.4)
        require('All queued saves have been flushed' in out,'Kindle flush acknowledgement failed.');return out
    def balance():
        return float(sql(mapped(f'SELECT IFNULL((SELECT Amount FROM CharactersMagicResources WHERE CharacterId={actor} AND MagicResourceId={resource}),0)'))[0][0])
    def capacity():
        out=session.send(route['invoked'],read_seconds=.3)
        plain=re.sub(r'\x1b\[[0-9;]*m','',out)
        matches=re.findall(r'You currently have ([\d,.]+)/([\d,.]+) '+re.escape(config['kindle_resource_name'])+r'\.',plain)
        require(len(matches)==1,'Native school status did not expose exact reserve capacity: '+host['redact'](plain))
        return float(matches[0][1].replace(',',''))
    def child():
        rows=sql(f'SELECT HEX(EffectData) FROM GameItems WHERE Id={report["item"]}',(0,))
        require(len(rows)==1,'Kindle destroyed borrowed original item.')
        root=ET.fromstring(rows[0][0]);return [e.find('Effect') for e in root.iter('Effect') if e.findtext('Type')=='SpellEquippedItemBonus']
    def identity():
        return sql(f'SELECT Id,GameItemProtoId,GameItemProtoRevision,Quality,OwnerId,OwnerType FROM GameItems WHERE Id={report["item"]}')
    if phase=='cold':
        report=receipt['family_acceptance'];flush()
        require(len(child())==1,'Cold restart lost selected capacity bonus before its real deadline.')
        require(capacity()==report['base_capacity']+1,'Cold equipped resource capacity bonus changed.')
        require(balance()==report['final_balance'],'Cold restart refilled or changed current reserve.')
        require(identity()==report['identity'],'Cold restart changed exact original item/prototype/quality/ownership.')
        assertions.append('Kindle: cold exact selected resource, resolved amount, equipped capacity and current balance retained')
        stop=time.monotonic()+75
        while time.monotonic()<stop and child():session.read_for(2);flush()
        require(not child(),'Kindle did not expire at its real 60-second deadline.')
        require(capacity()==report['base_capacity'],'Expired Kindle still increased resource capacity.')
        require(balance()==report['final_balance'] and identity()==report['identity'],'Expiry refilled reserves or mutated original item.')
        require(not sql(f'SELECT EntityId FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={report["item"]}'),'Kindle claimed existing foreign item ownership.')
        require(len(sql(f'SELECT Id FROM MagicCastingOperations WHERE CharacterId={actor}'))==report['operation_count'],'Cold retention/expiry manufactured another paid cast.')
        report.update(status='PASS',cold_status='PASS');assertions.append('Kindle: real deadline removes only enchantment; same borrowed item, quality, ownership and reserve balance conserved')
        return
    report=receipt['family_acceptance']={'status':'FAIL','cases':'kindle','scope':'Paid grade-one source-mana capacity, native equipment, cold/expiry; other source choices and all grades have focused owning regressions.',
        'kindle_reconciliation':data['kindle_reconciliation'],'support_reconciliation':data['support_reconciliation']}
    session.send('drop all',read_seconds=.2);session.send(f'goto {int(config["origin"])}',read_seconds=.2)
    session.send(f'givemerit me {int(route["merit"])}',read_seconds=.2)
    session.send(f'magic casting enrol {actor} {int(route["capability"])} bounded Kindle acceptance',read_seconds=.3)
    session.send(f'item load {host["item_command_reference"](proto)}',read_seconds=.3);flush()
    rows=sql(f'SELECT gi.Id FROM GameItems gi JOIN Bodies_GameItems b ON b.GameItemId=gi.Id WHERE b.BodyId={body} AND gi.GameItemProtoId={proto}')
    require(len(rows)==1,'Kindle needs one exact ordinary held item.');report['item']=int(rows[0][0]);report['identity']=identity()
    # Native player selectors derive from visible descriptions, not builder prototype keywords.
    report['dub_output']=host['redact'](session.send('dub prop s4kindleprop',read_seconds=.3));flush()
    dubs=sql(f"SELECT TargetId,Keywords FROM Dubs WHERE CharacterId={actor} AND TargetType='GameItem' AND TargetId={report['item']}")
    require(len(dubs)==1 and 's4kindleprop' in dubs[0][1].split(), 'Kindle native dub did not bind the exact loaded prop: '+report['dub_output'])
    report['base_capacity']=capacity()
    for attempt in range(3):
        current=balance()
        if current<99:
            session.send(f'{route["invoked"]} gather {int(route["capability"])} draw {100-current:.4f}',read_seconds=.3)
            stop=time.monotonic()+18
            while time.monotonic()<stop and balance()<99:session.read_for(.4)
            require(balance()>=99,'Native finite gathering did not fund Kindle.')
        before=balance();ops={r[0] for r in sql(f'SELECT HEX(Id) FROM MagicCastingOperations WHERE CharacterId={actor}')}
        out=session.send(f'{route["invoked"]} cast "{route["name"]}" grade 1 on mana-fire s4kindleprop via {int(route["capability"])}',read_seconds=.3);flush()
        added=[r for r in sql(f'SELECT HEX(Id),Stage,HEX(Definition) FROM MagicCastingOperations WHERE CharacterId={actor}',(2,)) if r[0] not in ops]
        report.setdefault('attempts',[]).append({'output':host['redact'](out),'operations':added,'balance_before':before,'balance_after':balance()})
        require(len(added)==1 and added[0][1]=='Completed','Kindle did not complete an exact paid operation: '+str(added)+' '+host['redact'](out))
        if ET.fromstring(added[0][2]).get('applied')=='true':break
    require(ET.fromstring(added[0][2]).get('applied')=='true','No ordinary paid Kindle attempt applied.')
    casting=ET.fromstring(added[0][2]);costs=casting.findall('Cost')
    # ControlledSpellEfficiency.Cost(1,1) is50;33 is the configured lower-bound floor.
    expected_cost=50.0
    require(casting.get('grade')=='1' and casting.get('controlledGrade')=='1' and len(costs)==1 and costs[0].get('resource')==str(resource) and float(costs[0].get('amount'))==expected_cost and float(costs[0].get('before'))==before, 'Kindle paid record differs from exact equal-grade source-efficiency policy.')
    report['paid_cost']=expected_cost
    require(abs(balance()-(before-expected_cost))<1e-7,'Kindle changed current reserve beyond its exact equal-grade 50-unit price (minimum floor33).')
    report['final_balance']=balance(); require(capacity()==report['base_capacity'],'Held-only enchanted item changed capacity.')
    selected=child();require(len(selected)==1 and selected[0].findtext('Kind')=='MagicResource' and selected[0].findtext('TargetId')==str(resource) and selected[0].findtext('Bonus')=='1','Saved Kindle bonus does not match exact selected source choice and grade.')
    assertions.append('Kindle: exact stock paid cast, source mana grade-one amount and only the exact equal-grade 50-unit cost (configured minimum33); held-only item gives no capacity bonus')
    session.send('wield s4kindleprop',read_seconds=.3);flush();require(capacity()==report['base_capacity']+1,'Native wield did not activate selected capacity.')
    require(balance()==report['final_balance'],'Wield refilled current reserve.')
    session.send('drop s4kindleprop',read_seconds=.3);flush();require(capacity()==report['base_capacity'],'Dropped item retained capacity contribution.')
    session.send('get s4kindleprop',read_seconds=.3);flush();require(capacity()==report['base_capacity'],'Picked-up held item granted equipped bonus.')
    session.send('wield s4kindleprop',read_seconds=.3);flush();require(capacity()==report['base_capacity']+1,'Re-wield lost selected bonus.')
    require(identity()==report['identity'] and balance()==report['final_balance'],'Equipment cycle changed original item or reserve.')
    report['operation_count']=len(sql(f'SELECT Id FROM MagicCastingOperations WHERE CharacterId={actor}'))
    report['status']='PASS';assertions.append('Kindle: native wield/drop/get/re-wield gates exact equipped capacity without refill or item mutation')
