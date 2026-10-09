"""N14: builder-installed living guardians, early death, timed remains decay and cold restart."""
import hashlib
import importlib.util
import json
import pathlib
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report['scope'] = 'N14 builder-installed Air Guardian early-death lifecycle; authored template/timing, not full catalogue or historical timing parity'
s.report['qualificationMarker'] = 'guardian-early-death-passed.json'
for name in ['scripts/FuryCalmSmokeWorld/guardian_early_death.py',
             'MudSharpCore/Magic/ArmageddonAirGuardianStock.cs', 'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs',
             'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.Admission.cs', 'MudSharpCore/Effects/Concrete/SpellNpcGuardian.cs',
             'MudSharpCore/Character/CharacterCombat.cs', 'MudSharpCore/Character/PhysicalReferenceCodecs.cs']:
    s.report['inputs'][name] = hashlib.sha256((s.repo / name).read_bytes()).hexdigest()
for name in ['MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.Retirement.cs',
             'FutureMUDLibrary/Magic/ISpellOwnedNpcService.cs',
             'MudSharpCore/Body/RetirementBodyEffects.cs', 'MudSharpCore/Character/CharacterArchiveService.cs']:
    s.report['inputs'][name] = hashlib.sha256((s.repo / name).read_bytes()).hexdigest()
corpse_original = None


def flush():
    s.command('impdebug flush', seconds=.5)


def wait_for(label, query, predicate, seconds=120):
    limit = time.monotonic() + seconds
    answer = ''
    while time.monotonic() < limit:
        answer = s.sql(query)
        if predicate(answer):
            break
        s.session.read_for(.5)
    s.check(label, predicate(answer), answer)
    return answer


def life_rows():
    raw = s.sql(f'SELECT l.Id,e.Kind,e.EntityId FROM MagicSpellLifecycles l JOIN MagicSpellOwnedEntities e ON e.LifecycleId=l.Id WHERE l.SpellId={spell} ORDER BY l.Id,e.Kind')
    result = {}
    for line in raw.splitlines():
        identifier, kind, entity = line.split('\t')
        result.setdefault(identifier, {})[kind] = int(entity)
    return result


def foreign_item_to():
    before = set(s.sql('SELECT Id FROM GameItems').splitlines())
    s.command(f'item load {item_proto}', 'load|create|hands')
    flush()
    new = set(s.sql('SELECT Id FROM GameItems').splitlines()) - before
    s.check('one ordinary foreign item created', len(new) == 1)
    item = int(new.pop())
    s.command('give staff qaguardian', 'give|hand')
    flush()
    number = s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}')
    s.check('foreign item actually carried by guardian',
            number.isdigit() and s.sql(f"SELECT COUNT(*) FROM Characters c JOIN Npcs n ON n.CharacterId=c.Id WHERE c.BodyId={number} AND n.TemplateId={template}") == '1')
    s.check('foreign item is outside all summon ownership claims',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item}') == '0')
    return item, int(number)


def verify_retired(identifier, claims):
    character, body = claims['1'], claims['3']
    wait_for('early-dead guardian completes physical retirement',
             f"SELECT State,Reason,Diagnostic FROM MagicSpellLifecycles WHERE Id='{identifier}'", lambda value: value.split('\t')[0] == '3')
    s.check('retirement removes exact NPC body and instance graph',
            s.sql(f'SELECT (SELECT COUNT(*) FROM Npcs WHERE CharacterId={character}),(SELECT COUNT(*) FROM Bodies WHERE Id={body}),(SELECT COUNT(*) FROM CharacterInstances WHERE CharacterId={character} OR BodyId={body})') == '0\t0\t0')
    s.check('canonical history and ownership attribution survive',
            s.sql(f"SELECT c.IsArchived,c.BodyId,a.OriginalBodyId,a.LifecycleId FROM Characters c JOIN CharacterArchives a ON a.CharacterId=c.Id WHERE c.Id={character}") == f'1\tNULL\t{body}\t{identifier}')
    s.check('both exact original ownership claims survive',
            s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind") == f'1\t{character}\n3\t{body}')


try:
    prior = json.loads((s.root / 'npc-archival-passed.json').read_text(encoding='utf-8'))
    s.check('typed-reference native retirement prerequisite qualified', prior['status'] == prior['runnerStatus'] == 'PASS' and prior['cleanup']['mysqlStopped'])
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text(encoding='utf-8'))
    for name in ['caster', 'capability', 'reserve']:
        setattr(s, name, actors[name])
    bindings = json.loads((s.root / 'fury-calm-bindings.json').read_text(encoding='utf-8'))
    school = bindings['Utilities']['School']
    resource = bindings['Utilities']['Resource']
    trait = int(s.sql(f"SELECT CastingTraitDefinitionId FROM MagicSpells WHERE Id={actors['fury']}"))
    untouched_fury = s.sql(f"SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors['fury']}")
    # The sole native corpse prototype receives an explicitly authored timed decay policy in this owned world.
    rows = s.sql("SELECT DISTINCT p.Id,p.RevisionNumber,p.MorphTimeSeconds,IFNULL(p.MorphGameItemProtoId,'NULL'),HEX(p.MorphEmote) FROM GameItemProtos p JOIN GameItemProtos_GameItemComponentProtos pc ON pc.GameItemProtoId=p.Id AND pc.GameItemProtoRevision=p.RevisionNumber JOIN GameItemComponentProtos c ON c.Id=pc.GameItemComponentProtoId AND c.RevisionNumber=pc.GameItemComponentRevision WHERE c.Type='Corpse'").splitlines()
    s.check('one native corpse prototype in owned world', len(rows) == 1)
    corpse_original = rows[0].split('\t')
    corpse_proto, corpse_revision, old_seconds, old_target, old_emote = corpse_original
    s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds=240,MorphGameItemProtoId=NULL,MorphEmote=CONVERT(UNHEX('{b'$0 crumbles into dust.'.hex()}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
    s.report['authoredTiming'] = dict(lifetimeSeconds='45*grade', corpseTimedDecaySeconds=240, offlineMorphPolicy='native paused remaining duration')
    s.start()
    source = int(s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaspawn'"))
    if not s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaguardian'"):
        s.command(f'npc clone {source} "qaguardian"', 'clone|copy|template')
        s.command('npc set sdesc a qaguardian spirit', 'description')
        s.command('npc edit close')
        flush()
    template = int(s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaguardian'"))
    existing = s.sql("SELECT Id FROM MagicSpells WHERE Name='Air Guardian'")
    if existing:
        definition = ET.fromstring(s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={int(existing)}'))
        s.check('retry reuses the exact builder-installed guardian stock', definition.findtext('StockIdentity') == 'arm.spell.air_guardian' and definition.findtext('Effects/Effect/NPCPrototypeId') == str(template))
        s.command(f'magic spell edit {existing}', 'edit')
    else:
        s.command(f'magic spell edit new stock air-guardian {school} {trait} {resource} {template} 45*grade', 'Created.*Air Guardian')
    s.command('magic spell set difficulty automatic', 'Automatic to cast')
    s.command('magic spell set effect 1 lifecycle deathonexpiry', 'lifecycle policy updated')
    s.command('magic spell set effect 1 lifetime 45*grade', 'lifetime expression updated')
    s.command('magic spell close')
    flush()
    spell = int(s.sql("SELECT Id FROM MagicSpells WHERE Name='Air Guardian'"))
    s.command(f'magic capability edit {s.capability}', 'edit')
    s.command(f'magic capability set casting entry add {spell}', 'updated|casting|already')
    s.command('magic capability close')
    s.command(f'magic casting grant {s.caster} {s.capability} {spell} N14 disposable native qualification', 'grant|acquir|already')
    s.stop()
    s.sql(f'UPDATE CharacterAcquiredSpells SET ControlledGrade=7 WHERE CharacterId={s.caster} AND MagicSpellId={spell}')
    s.sql(f'UPDATE Characters_MagicResources SET Amount=1000 WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}')
    # Ordinary native holdable item; explicitly exclude corpse, temporary stock and system-only components.
    item_proto = int(s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.item.charged_staff'"))
    s.report['objects'] = dict(actors, guardianSpell=spell, guardianTemplate=template, foreignPrototype=item_proto)
    s.start()
    wait_for('prior failed-run guardians are no longer living',
             f'SELECT COUNT(*) FROM MagicSpellLifecycles l JOIN MagicSpellOwnedEntities e ON e.LifecycleId=l.Id AND e.Kind=1 JOIN Npcs n ON n.CharacterId=e.EntityId JOIN Characters c ON c.Id=n.CharacterId WHERE l.SpellId={spell} AND (c.State & 64)=0',
             lambda value: value == '0', 120)
    before = s.resource(s.caster)
    before_lives = set(life_rows())
    operation_count = int(s.sql('SELECT COUNT(*) FROM MagicCastingOperations'))
    output = s.cast('Air Guardian', 2, 'here', 'The paid casting succeeded')
    after = s.resource(s.caster)
    s.check('controlled-seven grade-two cast pays one exact nine-energy minimum debit', before - after == 9,
            f'before={before}; after={after}; debit={before-after}; {output}')
    s.check('one casting operation for the group', int(s.sql('SELECT COUNT(*) FROM MagicCastingOperations')) == operation_count + 1)
    flush()
    lives = {key: value for key, value in life_rows().items() if key not in before_lives}
    s.check('requested grade creates two independently claimed native guardians', len(lives) == 2 and all(set(claims) == {'1', '3'} for claims in lives.values()))
    for identifier, claims in lives.items():
        s.check('guardian persists the exact typed creator bond',
                len(s.children(claims['1'], 'SpellNpcGuardian')) == 1 and s.children(claims['1'], 'SpellNpcGuardian')[0].findtext('Effect/CreatorId') == str(s.caster))
    saved_bonds = {claims['1']: ET.tostring(s.children(claims['1'], 'SpellNpcGuardian')[0]) for claims in lives.values()}
    s.stop()
    s.start()
    for character, saved in saved_bonds.items():
        s.check('living guardian creator bond survives cold restart', ET.tostring(s.children(character, 'SpellNpcGuardian')[0]) == saved)
    s.command('force qaguardian hit qaother', 'engage.*combat')
    guarded_attack = s.command('force qaenemy hit qacaster', seconds=.2)
    s.check('already-fighting guardian responds within the initiating native command', 'qaguardian spirit' in guarded_attack and 'switch' in guarded_attack.lower(), guarded_attack)
    s.command('peace', 'end .* combats')
    goods, carried_body = foreign_item_to()
    retained = next(key for key, value in lives.items() if value['3'] == carried_body)
    retained_claims = lives[retained]
    s.command('kill qaguardian', seconds=.7)
    flush()
    remains = s.sql(f"SELECT RemainsItemId FROM MagicSpellLifecycles WHERE Id='{retained}'")
    s.check('early death correlates a real native corpse', remains.isdigit())
    death_row = s.sql(f"SELECT DeathObservedUtc,RemainsItemId FROM MagicSpellLifecycles WHERE Id='{retained}'")
    wait_for('original summon deadline actually passes', f"SELECT DeadlineUtc<=UTC_TIMESTAMP(6) FROM MagicSpellLifecycles WHERE Id='{retained}'", lambda value: value == '1', 100)
    flush()
    s.check('expiry preserves the same early death and corpse without resurrection', s.sql(f"SELECT DeathObservedUtc,RemainsItemId FROM MagicSpellLifecycles WHERE Id='{retained}'") == death_row)
    s.check('configured corpse and foreign carried item survive original expiry',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={remains}),(SELECT COUNT(*) FROM Bodies_GameItems WHERE BodyId={retained_claims["3"]} AND GameItemId={goods})') == '1\t1')
    s.check('configured guardian stays dead through original expiry', s.sql(f'SELECT (State & 64)<>0 FROM Characters WHERE Id={retained_claims["1"]}') == '1')
    s.stop()
    s.start()
    flush()
    s.check('cold restart preserves exact early death and remains identity', s.sql(f"SELECT DeathObservedUtc,RemainsItemId FROM MagicSpellLifecycles WHERE Id='{retained}'") == death_row)
    s.check('cold restart preserves foreign item and dead heavy graph until decay',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={goods}),(SELECT COUNT(*) FROM Bodies WHERE Id={retained_claims["3"]}),(SELECT COUNT(*) FROM GameItems WHERE Id={remains})') == '1\t1\t1')
    wait_for('actual scheduled native corpse decay removes remains', f'SELECT COUNT(*) FROM GameItems WHERE Id={remains}', lambda value: value == '0', 280)
    verify_retired(retained, retained_claims)
    s.check('decay evacuates the foreign item intact to the native room',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={goods}),(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={goods}),(SELECT COUNT(*) FROM Cells_GameItems WHERE GameItemId={goods})') == '1\t0\t1')
    # The other grade-two guardian follows the ordinary expiry/remains path; qualify its eventual cleanup too.
    for identifier, claims in lives.items():
        if identifier != retained:
            verify_retired(identifier, claims)
    s.command(f'magic spell edit {spell}', 'edit')
    s.command('magic spell set effect 1 lifecycle temporarycleanup', 'lifecycle policy updated')
    s.command('magic spell close')
    before_lives = set(life_rows())
    before = s.resource(s.caster)
    output = s.cast('Air Guardian', 1, 'here', 'The paid casting succeeded')
    after = s.resource(s.caster)
    s.check('default-dissipation stock cast pays exact nine-energy debit', before - after == 9,
            f'before={before}; after={after}; debit={before-after}; {output}')
    flush()
    current = life_rows()
    new = set(current) - before_lives
    s.check('grade-one creates one separate guardian', len(new) == 1)
    identifier = new.pop()
    claims = current[identifier]
    default_goods, default_body = foreign_item_to()
    s.check('default foreign-item recipient matches exact ownership claim', default_body == claims['3'])
    s.command('kill qaguardian', seconds=.7)
    flush()
    s.check('default early death dissipates without a corpse', s.sql(f"SELECT RemainsItemId IS NULL,DeathObservedUtc IS NOT NULL FROM MagicSpellLifecycles WHERE Id='{identifier}'") == '1\t1')
    verify_retired(identifier, claims)
    s.check('default early dissipation also conserves foreign carried goods',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={default_goods}),(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={default_goods}),(SELECT COUNT(*) FROM Cells_GameItems WHERE GameItemId={default_goods})') == '1\t0\t1')
    created = dict(lives)
    created[identifier] = claims
    s.report['createdLifecycles'] = [dict(id=key, character=value['1'], body=value['3']) for key, value in created.items()]
    s.report['retainedRemains'] = dict(lifecycle=retained, corpse=int(remains), foreignItem=goods, deathRow=death_row)
    s.stop()
    s.report['status'] = 'PASS'
    s.report['n14BuilderInstalledQualified'] = True
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    try:
        if s.process is not None:
            s.stop()
        if corpse_original is not None:
            s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds={old_seconds},MorphGameItemProtoId={old_target},MorphEmote=CONVERT(UNHEX('{old_emote}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
            s.check('native corpse prototype policy restored exactly', s.sql(f"SELECT MorphTimeSeconds,IFNULL(MorphGameItemProtoId,'NULL'),HEX(MorphEmote) FROM GameItemProtos WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}") == '\t'.join(corpse_original[2:]))
        s.check('Fury stock definition remains byte-identical', s.sql(f"SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors['fury']}") == untouched_fury)
    except BaseException as cleanup_error:
        s.report['status'] = 'FAIL'
        s.report['cleanupError'] = s.helper.redact(str(cleanup_error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
