"""N15: normal builder-installed guardian lifecycle modes in the retained owned world."""
import hashlib
import importlib.util
import json
import pathlib
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report['scope'] = 'N15 builder-installed same-template guardian lifecycle modes; authored timing, not full catalogue parity'
s.report['qualificationMarker'] = 'guardian-lifecycle-modes-passed.json'
input_paths = [
    'scripts/FuryCalmSmokeWorld/guardian_lifecycle_modes.py',
    'MudSharpCore/Magic/ArmageddonAirGuardianStock.cs',
    'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs',
    'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.Admission.cs',
    'MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.cs',
    'MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.Retirement.cs',
    'MudSharpCore/Magic/Lifecycle/SpellOwnedLifecycleStore.cs',
    'MudSharpCore/Effects/Concrete/SpellNpcGuardian.cs',
    'MudSharpCore/Body/RetirementBodyEffects.cs',
    'MudSharpCore/Character/CharacterArchiveService.cs',
    'MudSharpCore/Character/PhysicalReferenceCodecs.cs',
    'MudSharpCore/GameItems/Components/CorpseGameItemComponent.cs',
    'FutureMUDLibrary/Magic/ISpellOwnedLifecycle.cs',
    'MudsharpDatabaseLibrary/Models/MagicSpellLifecycle.cs',
]
for name in input_paths:
    s.report['inputs'][name] = hashlib.sha256((s.repo / name).read_bytes()).hexdigest()
corpse_original = guardian_original = untouched_fury = None
s.report['createdLifecycles'] = []

# Exact columns from MagicSpellLifecycle; no reflection or guessed reference fields.
journal_columns = ('Id,SpellId,Grade,CreatorId,HEX(Family),Mode,CreatedUtc,DeadlineUtc,'
                   'HEX(Provenance),State,Reason,DeathObservedUtc,RemainsItemId,'
                   'RemainsRemovalRequestedUtc,RemainsNotificationAttemptedUtc,'
                   'RemainsNotificationCompletedUtc,UpdatedUtc,Version,HEX(Diagnostic)')


def flush():
    s.command('impdebug flush', seconds=.5)


def wait_for(label, query, predicate, seconds=120):
    limit = time.monotonic() + seconds
    answer = ''
    while time.monotonic() < limit:
        answer = s.sql(query)
        if predicate(answer):
            break
        if s.session is not None:
            s.session.read_for(.5)
        else:
            time.sleep(.5)
    s.check(label, predicate(answer), answer)
    return answer


def life_rows():
    result = {}
    raw = s.sql(f'SELECT l.Id,e.Kind,e.EntityId FROM MagicSpellLifecycles l '
                f'JOIN MagicSpellOwnedEntities e ON e.LifecycleId=l.Id WHERE l.SpellId={spell} ORDER BY l.Id,e.Kind')
    for line in raw.splitlines():
        identifier, kind, entity = line.split('\t')
        result.setdefault(identifier, {})[kind] = int(entity)
    return result


def journal(identifier):
    return s.sql(f"SELECT {journal_columns} FROM MagicSpellLifecycles WHERE Id='{identifier}'")


def claims_row(identifier):
    return s.sql(f"SELECT Kind,EntityId,Role FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind")


def native_graphs():
    return (s.sql(f'SELECT n.CharacterId,c.BodyId FROM Npcs n JOIN Characters c ON c.Id=n.CharacterId '
                  f'WHERE n.TemplateId={template} ORDER BY n.CharacterId'),
            s.sql('SELECT Id FROM Bodies ORDER BY Id'))


def corpses():
    return set(s.sql(f'SELECT Id FROM GameItems WHERE GameItemProtoId={corpse_proto} '
                     f'AND GameItemProtoRevision={corpse_revision}').splitlines())


def cast_mode(mode, mode_number):
    s.command(f'magic spell edit {spell}', 'edit')
    s.command(f'magic spell set effect 1 lifecycle {mode}', 'lifecycle policy updated')
    if mode != 'permanent':
        s.command('magic spell set effect 1 lifetime 45*grade', 'lifetime expression updated')
    s.command('magic spell close')
    flush()
    before_lives = set(life_rows())
    before_npcs, before_bodies = native_graphs()
    before_operations = int(s.sql('SELECT COUNT(*) FROM MagicCastingOperations'))
    before = s.resource(s.caster)
    output = s.cast('Air Guardian', 1, 'here', 'The paid casting succeeded')
    after = s.resource(s.caster)
    s.check(mode + ' pays one exact nine-energy minimum debit', before - after == 9,
            f'before={before}; after={after}; debit={before-after}; {output}')
    s.check(mode + ' records one casting operation',
            int(s.sql('SELECT COUNT(*) FROM MagicCastingOperations')) == before_operations + 1)
    flush()
    rows = life_rows()
    added = set(rows) - before_lives
    s.check(mode + ' creates one independently claimed native guardian', len(added) == 1)
    identifier = added.pop()
    claims = rows[identifier]
    s.check(mode + ' owns only the exact character and body', set(claims) == {'1', '3'})
    s.check(mode + ' has exactly two original creation claims',
            claims_row(identifier) == f'1\t{claims["1"]}\t0\n3\t{claims["3"]}\t0')
    after_npcs, after_bodies = native_graphs()
    s.check(mode + ' creates exactly one native NPC and body without unclaimed siblings',
            set(after_npcs.splitlines()) == set(before_npcs.splitlines()) | {f'{claims["1"]}\t{claims["3"]}'} and
            set(after_bodies.splitlines()) == set(before_bodies.splitlines()) | {str(claims['3'])})
    s.check(mode + ' records selected mode and caster',
            s.sql(f"SELECT Mode,CreatorId,Grade FROM MagicSpellLifecycles WHERE Id='{identifier}'") == f'{mode_number}\t{s.caster}\t1')
    s.check(mode + ' uses the same approved native template',
            s.sql(f'SELECT n.TemplateId,c.BodyId,c.IsArchived,(c.State & 64)<>0 FROM Npcs n '
                  f'JOIN Characters c ON c.Id=n.CharacterId WHERE c.Id={claims["1"]}') == f'{template}\t{claims["3"]}\t0\t0')
    effects = s.children(claims['1'], 'SpellNpcGuardian')
    s.check(mode + ' persists the exact native creator bond',
            len(effects) == 1 and effects[0].findtext('Effect/CreatorId') == str(s.caster))
    location = s.sql(f'SELECT LocationId FROM CharacterInstances WHERE CharacterId={claims["1"]} '
                     f'AND BodyId={claims["3"]} AND IsPrimary=1')
    s.check(mode + ' has one exact persisted primary spawn room', location.isdigit())
    s.report['createdLifecycles'].append(dict(id=identifier, mode=mode, character=claims['1'], body=claims['3'], room=int(location),
                                            activatedJournal=journal(identifier), claims=claims_row(identifier)))
    return identifier, claims


def give_foreign(claims):
    before = set(s.sql('SELECT Id FROM GameItems').splitlines())
    s.command(f'item load {item_proto}', 'load|create|hands')
    flush()
    added = set(s.sql('SELECT Id FROM GameItems').splitlines()) - before
    s.check('one ordinary foreign item created', len(added) == 1)
    item = int(added.pop())
    s.command('give staff qaguardian', 'give|hand')
    flush()
    s.check('foreign goods reach the exact current guardian body',
            s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') == str(claims['3']))
    s.check('foreign goods have no summon ownership claim',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item}') == '0')
    return item


def verify_completed(identifier, claims, goods):
    wait_for('natural expiry reaches one completed lifecycle',
             f"SELECT State,Diagnostic FROM MagicSpellLifecycles WHERE Id='{identifier}'",
             lambda value: value.split('\t')[0] == '3')
    s.wait_npc_retired(identifier, claims['1'], claims['3'])
    s.check('expiry death is observed once after the original deadline',
            s.sql(f"SELECT DeathObservedUtc>=DeadlineUtc,Reason FROM MagicSpellLifecycles WHERE Id='{identifier}'") == '1\t0')
    s.check('foreign goods survive expiry in the native room',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={goods}),'
                  f'(SELECT COUNT(*) FROM Bodies_GameItems WHERE GameItemId={goods}),'
                  f'(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={goods})') == '1\t0\t1')
    room = s.report['createdLifecycles'][-1]['room']
    s.check('foreign goods reach the exact original guardian room',
            s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={goods}') == str(room))
    flush()
    terminal = journal(identifier)
    original_claims = claims_row(identifier)
    s.check('exact ownership claims remain after completion',
            original_claims == f'1\t{claims["1"]}\t0\n3\t{claims["3"]}\t0')
    operation_count = s.sql('SELECT COUNT(*) FROM MagicCastingOperations')
    identities = set(life_rows())
    graphs = native_graphs()
    s.stop()
    s.start()
    flush()
    s.check('cold restart never reopens or repeats a terminal transition', journal(identifier) == terminal)
    s.check('cold restart retains both exact ownership claims', claims_row(identifier) == original_claims)
    s.check('cold restart never replays paid casting or creation',
            s.sql('SELECT COUNT(*) FROM MagicCastingOperations') == operation_count and set(life_rows()) == identities and
            native_graphs() == graphs)
    s.check('cold restart keeps the retired graph absent and foreign goods intact',
            s.sql(f'SELECT (SELECT COUNT(*) FROM Npcs WHERE CharacterId={claims["1"]}),'
                  f'(SELECT COUNT(*) FROM Bodies WHERE Id={claims["3"]}),'
                  f'(SELECT COUNT(*) FROM GameItems WHERE Id={goods}),'
                  f'(SELECT COUNT(*) FROM Rooms_GameItems WHERE GameItemId={goods})') == '0\t0\t1\t1')
    s.check('cold restart preserves foreign goods in that same original room',
            s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={goods}') == str(room))
    s.report['createdLifecycles'][-1].update(terminalJournal=terminal, foreignItem=goods,
                                          terminalRestartJournal=journal(identifier))


try:
    prior = json.loads((s.root / 'guardian-early-death-passed.json').read_text(encoding='utf-8'))
    s.check('builder-installed N14 prerequisite is qualified',
            prior['status'] == prior['runnerStatus'] == 'PASS' and prior['n14BuilderInstalledQualified'] and prior['cleanup']['mysqlStopped'])
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text(encoding='utf-8'))
    for name in ['caster', 'capability', 'reserve']:
        setattr(s, name, actors[name])
    spell = int(s.sql("SELECT Id FROM MagicSpells WHERE Name='Air Guardian'"))
    guardian_original = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={spell}')
    untouched_fury = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors["fury"]}')
    definition = ET.fromstring(bytes.fromhex(guardian_original).decode('utf-8'))
    template = int(definition.findtext('Effects/Effect/NPCPrototypeId'))
    template_name = s.sql(f'SELECT DISTINCT Name FROM NpcTemplates WHERE Id={template}')
    s.check('same exact builder-installed stock and approved template retained',
            definition.findtext('StockIdentity') == 'arm.spell.air_guardian' and
            spell == prior['objects']['guardianSpell'] and template == prior['objects']['guardianTemplate'] and bool(template_name),
            f'template={template}; prior={prior["objects"]["guardianTemplate"]}; name={template_name}')
    s.check('no prior living guardian obscures exact recipient selection',
            s.sql(f'SELECT COUNT(*) FROM Npcs n JOIN Characters c ON c.Id=n.CharacterId '
                  f'WHERE n.TemplateId={template} AND (c.State & 64)=0') == '0')
    s.check('controlled grade-seven fixture and sufficient reserve already present',
            s.sql(f'SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={s.caster} AND MagicSpellId={spell}') == '7' and
            float(s.sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}')) >= 27)
    item_proto = int(s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.item.charged_staff'"))
    rows = s.sql("SELECT DISTINCT p.Id,p.RevisionNumber,p.MorphTimeSeconds,IFNULL(p.MorphGameItemProtoId,'NULL'),HEX(p.MorphEmote) "
                 "FROM GameItemProtos p JOIN GameItemProtos_GameItemComponentProtos pc ON pc.GameItemProtoId=p.Id AND pc.GameItemProtoRevision=p.RevisionNumber "
                 "JOIN GameItemComponentProtos c ON c.Id=pc.GameItemComponentProtoId AND c.RevisionNumber=pc.GameItemComponentRevision WHERE c.Type='Corpse'").splitlines()
    s.check('one exact native corpse prototype in owned world', len(rows) == 1)
    corpse_original = rows[0].split('\t')
    corpse_proto, corpse_revision, old_seconds, old_target, old_emote = corpse_original
    s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds=60,MorphGameItemProtoId=NULL,MorphEmote=CONVERT(UNHEX('{b'$0 crumbles into dust.'.hex()}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
    s.report['authoredTiming'] = dict(temporaryLifetimeSeconds='45*grade', corpseTimedDecaySeconds=60,
                                     permanentObservedBeyondSeconds=45, offlineMorphPolicy='native paused remaining duration')
    s.report['objects'] = dict(actors, guardianSpell=spell, guardianTemplate=template, foreignPrototype=item_proto)
    s.start()

    corpse_baseline = corpses()
    identifier, claims = cast_mode('temporarycleanup', 1)
    goods = give_foreign(claims)
    deadline = s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{identifier}'")
    s.check('temporary cleanup starts active with an absolute deadline and no death',
            s.sql(f"SELECT State,DeadlineUtc>UTC_TIMESTAMP(6),DeathObservedUtc IS NULL FROM MagicSpellLifecycles WHERE Id='{identifier}'") == '0\t1\t1')
    s.stop()
    s.start()
    s.check('temporary cleanup cold restart preserves its original deadline',
            s.sql(f"SELECT DeadlineUtc FROM MagicSpellLifecycles WHERE Id='{identifier}'") == deadline)
    verify_completed(identifier, claims, goods)
    s.check('temporary cleanup suppresses native remains',
            s.sql(f"SELECT RemainsItemId IS NULL FROM MagicSpellLifecycles WHERE Id='{identifier}'") == '1' and
            corpses() - corpse_baseline == set())

    corpse_baseline = corpses()
    identifier, claims = cast_mode('deathonexpiry', 2)
    goods = give_foreign(claims)
    wait_for('death-on-expiry observes natural native death',
             f"SELECT DeathObservedUtc IS NOT NULL FROM MagicSpellLifecycles WHERE Id='{identifier}'", lambda value: value == '1')
    flush()
    remains = s.sql(f"SELECT RemainsItemId FROM MagicSpellLifecycles WHERE Id='{identifier}'")
    s.check('death-on-expiry creates one real native corpse', remains.isdigit())
    s.check('death-on-expiry creates exactly one corpse without uncorrelated duplicates',
            corpses() - corpse_baseline == {remains})
    component = s.sql(f"SELECT i.Definition FROM GameItemComponents i JOIN GameItemComponentProtos p ON p.Id=i.GameItemComponentProtoId AND p.RevisionNumber=i.GameItemComponentProtoRevision WHERE i.GameItemId={remains} AND p.Type='Corpse'")
    corpse = ET.fromstring(component)
    s.check('known native corpse fields reference the exact character and body',
            corpse.findtext('OriginalCharacter') == str(claims['1']) and corpse.findtext('OriginalBody') == str(claims['3']))
    s.check('death-on-expiry retains foreign goods in the exact dead body until decay',
            s.sql(f'SELECT (SELECT COUNT(*) FROM GameItems WHERE Id={remains}),'
                  f'(SELECT COUNT(*) FROM Bodies_GameItems WHERE BodyId={claims["3"]} AND GameItemId={goods})') == '1\t1')
    s.report['createdLifecycles'][-1]['corpse'] = int(remains)
    wait_for('actual scheduled corpse decay removes the exact remains',
             f'SELECT COUNT(*) FROM GameItems WHERE Id={remains}', lambda value: value == '0', 100)
    verify_completed(identifier, claims, goods)
    s.check('completed death-on-expiry leaves no new or duplicate corpse after restart',
            corpses() - corpse_baseline == set())

    identifier, claims = cast_mode('permanent', 0)
    permanent_row = journal(identifier)
    permanent_claims = claims_row(identifier)
    bond = ET.tostring(s.children(claims['1'], 'SpellNpcGuardian')[0])
    s.check('permanent activation completes provenance without any expiry or death',
            s.sql(f"SELECT State,DeadlineUtc IS NULL,DeathObservedUtc IS NULL,Reason IS NULL FROM MagicSpellLifecycles WHERE Id='{identifier}'") == '3\t1\t1\t1')
    count_before = s.sql('SELECT COUNT(*) FROM MagicCastingOperations')
    identities = set(life_rows())
    graphs = native_graphs()
    s.stop()
    wait_for('permanent observation passes the authored temporary lifetime while offline',
             f"SELECT CreatedUtc+INTERVAL 45 SECOND<UTC_TIMESTAMP(6) FROM MagicSpellLifecycles WHERE Id='{identifier}'", lambda value: value == '1', 60)
    s.start()
    flush()
    s.check('permanent survives beyond temporary lifetime as the same living native graph',
            s.sql(f'SELECT n.TemplateId,c.BodyId,c.IsArchived,(c.State & 64)<>0 FROM Npcs n '
                  f'JOIN Characters c ON c.Id=n.CharacterId WHERE c.Id={claims["1"]}') == f'{template}\t{claims["3"]}\t0\t0')
    s.check('permanent cold restart retains byte-identical provenance and claims',
            journal(identifier) == permanent_row and claims_row(identifier) == permanent_claims)
    s.check('permanent cold restart restores the same native creator bond',
            ET.tostring(s.children(claims['1'], 'SpellNpcGuardian')[0]) == bond)
    s.check('permanent cold restart never replays creation or payment',
            s.sql('SELECT COUNT(*) FROM MagicCastingOperations') == count_before and set(life_rows()) == identities and
            native_graphs() == graphs)
    s.report['createdLifecycles'][-1].update(restartedJournal=journal(identifier), retainedAsOrdinaryDurableNpc=True)
    s.stop()
    s.report['status'] = 'PASS'
    s.report['n15BuilderInstalledQualified'] = True
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    try:
        if s.process is not None:
            s.stop()
        if corpse_original is not None:
            s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds={old_seconds},MorphGameItemProtoId={old_target},MorphEmote=CONVERT(UNHEX('{old_emote}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
            s.check('native corpse prototype policy restored exactly',
                    s.sql(f"SELECT MorphTimeSeconds,IFNULL(MorphGameItemProtoId,'NULL'),HEX(MorphEmote) FROM GameItemProtos WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}") == '\t'.join(corpse_original[2:]))
        if guardian_original is not None:
            s.sql(f"UPDATE MagicSpells SET Definition=CONVERT(UNHEX('{guardian_original}') USING utf8mb4) WHERE Id={spell}")
            s.check('guardian builder definition restored byte-for-byte after stopped scenario',
                    s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={spell}') == guardian_original)
        if untouched_fury is not None:
            s.check('Fury stock definition remains byte-identical',
                    s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors["fury"]}') == untouched_fury)
        s.report['inputsUnchanged'] = all(hashlib.sha256((s.repo / name).read_bytes()).hexdigest() == digest
                                         for name, digest in s.report['inputs'].items())
        s.check('all native source and binary inputs remain unchanged', s.report['inputsUnchanged'])
    except BaseException as cleanup_error:
        s.report['status'] = 'FAIL'
        s.report['cleanupError'] = s.helper.redact(str(cleanup_error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
