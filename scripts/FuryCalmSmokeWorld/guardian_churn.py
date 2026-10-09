"""N16 bounded chunks: 32 restart cycles, then 128 creatures in one process.

N16_STAGE=0..15 runs two cycles; N16_STAGE=16 runs the continuous batch.
Only the owned wrapper promotes each fresh marker after verified MySQL cleanup.
"""
import hashlib
import importlib.util
import json
import os
import pathlib
import re
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
stage = int(os.environ['N16_STAGE'])
assert 0 <= stage <= 16
s.report.update(scope='N16 normal paid guardian churn; bounded restart cycles and continuous 128-creature batch',
                qualificationMarker=f'guardian-churn-{stage:02d}-passed.json', n16Stage=stage,
                cycles=[], authoredTiming=dict(cycleLifetime='30*grade', corpseDecaySeconds=30,
                                               batchLifetimeSeconds=240, batchEarlyDeaths=64))
for name in ['scripts/FuryCalmSmokeWorld/guardian_churn.py',
             'MudSharpCore/Commands/Modules/StaffModule.Census.cs',
             'MudSharpCore/Framework/Scheduling/Scheduler.cs', 'MudSharpCore/Effects/EffectScheduler.cs',
             'MudSharpCore/Framework/Scheduling/HeartbeatManager.cs',
             'MudSharpCore/Framework/PerceivedItem.cs', 'MudSharpCore/Framework/PerceiverItem.cs',
             'MudSharpCore/Character/Character.Diagnostics.cs', 'FutureMUDLibrary/Framework/Scheduling/IScheduler.cs',
             'MudSharpCore/Magic/ArmageddonAirGuardianStock.cs', 'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs',
             'MudSharpCore/Magic/SpellEffects/CreateNPCEffect.Admission.cs',
             'MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.cs',
             'MudSharpCore/Magic/Lifecycle/SpellOwnedNpcService.Retirement.cs',
             'MudSharpCore/Magic/Lifecycle/SpellOwnedLifecycleStore.cs',
             'MudSharpCore/Effects/Concrete/SpellNpcGuardian.cs', 'MudSharpCore/Character/CharacterMovement.cs',
             'MudSharpCore/Character/CharacterArchiveService.cs', 'MudSharpCore/Character/PhysicalReferenceCodecs.cs',
             'MudSharpCore/Body/RetirementBodyEffects.cs', 'MudSharpCore/GameItems/Components/CorpseGameItemComponent.cs']:
    s.report['inputs'][name] = hashlib.sha256((s.repo / name).read_bytes()).hexdigest()

journal_columns = ('Id,SpellId,Grade,CreatorId,HEX(Family),Mode,CreatedUtc,DeadlineUtc,'
                   'HEX(Provenance),State,Reason,DeathObservedUtc,RemainsItemId,'
                   'RemainsRemovalRequestedUtc,RemainsNotificationAttemptedUtc,'
                   'RemainsNotificationCompletedUtc,UpdatedUtc,Version,HEX(Diagnostic)')
census_names = ['Actors', 'Cached actors', 'NPCs', 'Bodies', 'Main schedules', 'Effect schedules',
                'Second', 'Ten Second', 'Thirty Second', 'Minute', 'Hour', 'Fuzzy Five Second',
                'Fuzzy Ten Second', 'Fuzzy Thirty Second', 'Fuzzy Minute', 'Fuzzy Five Minute',
                'Fuzzy Ten Minute', 'Fuzzy Thirty Minute', 'Fuzzy Hour', 'Character',
                'Quit subscriptions', 'Deleted subscriptions', 'Join combat subscriptions',
                'Death subscriptions', 'Start move subscriptions', 'Stop move subscriptions',
                'Following character', 'Following instance', 'Followers']
guardian_original = corpse_original = permanent_description = permanent_names = None


def flush():
    s.command('impdebug flush', seconds=.5)


def wait_for(label, query, predicate, seconds=150):
    limit = min(s.deadline, time.monotonic() + seconds)
    answer = ''
    while time.monotonic() < limit:
        answer = s.sql(query)
        if predicate(answer):
            break
        s.session.read_for(.5)
    s.check(label, predicate(answer), answer)
    return answer


def census(character=None):
    output = s.command(f'debug census {character or s.caster}', 'Runtime census:', seconds=.5)
    counts = {}
    for name in census_names:
        match = re.search(r'^\s*' + re.escape(name) + r': ([0-9,]+)\s*$', output, re.M)
        if match:
            counts[name] = int(match.group(1).replace(',', ''))
    assert set(counts) == set(census_names), output
    return counts


def stable_census(label, expected, seconds=15):
    limit = min(s.deadline, time.monotonic() + seconds)
    samples = []
    consecutive = 0
    while time.monotonic() < limit:
        current = census()
        samples.append(current)
        consecutive = consecutive + 1 if current == expected else 0
        if consecutive == 3:
            break
        s.session.read_for(1)
    s.check(label, consecutive == 3, json.dumps(dict(expected=expected, samples=samples)))
    return samples


def identities(table, column='Id', where=''):
    return set(s.sql(f'SELECT {column} FROM {table} {where} ORDER BY {column}').splitlines())


def graphs():
    return dict(bodies=identities('Bodies'), npcs=identities('Npcs', 'CharacterId'),
                instances=identities('CharacterInstances'),
                characters=identities('Characters'), archives=identities('CharacterArchives', 'CharacterId'),
                items=identities('GameItems'), operations=identities('MagicCastingOperations'),
                lifecycles=identities('MagicSpellLifecycles'),
                claims=s.sql('SELECT LifecycleId,Kind,EntityId,Role FROM MagicSpellOwnedEntities ORDER BY LifecycleId,Kind,EntityId'))


def serial_graphs(value):
    return {key: sorted(item) if isinstance(item, set) else item for key, item in value.items()}


def histories():
    return (s.sql(f'SELECT {journal_columns} FROM MagicSpellLifecycles ORDER BY Id'),
            s.sql('SELECT LifecycleId,Kind,EntityId,Role FROM MagicSpellOwnedEntities ORDER BY LifecycleId,Kind,EntityId'))


def corpses():
    return identities('GameItems', where=f'WHERE GameItemProtoId={corpse_proto} AND GameItemProtoRevision={corpse_revision}')


def new_claims(before):
    result = {}
    new = identities('MagicSpellLifecycles') - before['lifecycles']
    for line in s.sql(f'SELECT e.LifecycleId,e.Kind,e.EntityId,e.Role FROM MagicSpellOwnedEntities e '
                      f'JOIN MagicSpellLifecycles l ON l.Id=e.LifecycleId WHERE l.SpellId={spell} ORDER BY e.LifecycleId,e.Kind').splitlines():
        identifier, kind, entity, role = line.split('\t')
        if identifier in new:
            assert role == '0' and kind not in result.setdefault(identifier, {})
            result[identifier][kind] = int(entity)
    s.check('every new lifecycle has exactly its original character/body claims',
            set(result) == new and all(set(value) == {'1', '3'} for value in result.values()))
    return result


def set_policy(mode, count, lifetime):
    s.command(f'magic spell edit {spell}', 'edit')
    s.command(f'magic spell set effect 1 lifecycle {mode}', 'lifecycle policy updated')
    s.command(f'magic spell set effect 1 count {count}', 'count updated')
    s.command(f'magic spell set effect 1 lifetime {lifetime}', 'lifetime expression updated')
    s.command('magic spell close')
    flush()


def foreign_item():
    before = identities('GameItems')
    s.command(f'item load {item_proto}', 'load|create|hands')
    flush()
    new = identities('GameItems') - before
    s.check('one ordinary foreign item created before the runtime baseline', len(new) == 1)
    item = int(new.pop())
    s.check('foreign item has no summon claim',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellOwnedEntities WHERE Kind=0 AND EntityId={item}') == '0')
    return item


def qualify_cycle(index, batch=False):
    mode = 'temporarycleanup' if batch or index % 4 < 2 else 'deathonexpiry'
    early = not batch and index % 2 == 0
    grade = 1 if batch else 1 + (index // 4) % 2
    count = 128 if batch else grade
    set_policy(mode, 128 if batch else 'grade', '240' if batch else '30*grade')
    item = foreign_item()
    # Ordinary retained items are already present in this baseline; they are not leaks.
    s.session.read_for(6)
    baseline = census()
    stable_census('pre-cast runtime census is stable', baseline)
    before = graphs()
    old_history = histories()
    corpse_baseline = corpses()
    energy = s.resource(s.caster)
    if batch:
        s.possess('qacaster')
        cast_output = s.command(f'qamagi cast "Air Guardian" grade {grade} on here via {s.capability}', seconds=.5)
        limit = min(s.deadline, time.monotonic() + 180)
        while 'The paid casting succeeded' not in cast_output and time.monotonic() < limit:
            cast_output += s.session.read_for(.5)
        s.check('continuous batch paid cast finishes within its explicit 180-second response budget',
                'The paid casting succeeded' in cast_output, cast_output[-2000:])
        s.command('return')
    else:
        cast_output = s.cast('Air Guardian', grade, 'here', 'The paid casting succeeded')
    after_energy = s.resource(s.caster)
    s.check('one exact nine-energy debit for the entire group', energy - after_energy == 9, cast_output)
    flush()
    claims = new_claims(before)
    s.check('requested independently owned output count', len(claims) == count)
    current = graphs()
    characters = {str(value['1']) for value in claims.values()}
    bodies = {str(value['3']) for value in claims.values()}
    s.check('exact new physical graphs have no unclaimed siblings',
            current['npcs'] == before['npcs'] | characters and current['bodies'] == before['bodies'] | bodies and
            current['characters'] == before['characters'] | characters and
            len(current['instances'] - before['instances']) == count and
            current['instances'] >= before['instances'] and
            len(current['operations'] - before['operations']) == 1 and current['operations'] >= before['operations'])
    ids = ','.join("'" + identifier + "'" for identifier in claims)
    char_ids = ','.join(characters)
    s.check('same qualified template, selected grade/mode and active deadline on every output',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles l JOIN MagicSpellOwnedEntities e ON e.LifecycleId=l.Id AND e.Kind=1 '
                  f'JOIN Npcs n ON n.CharacterId=e.EntityId WHERE l.Id IN ({ids}) AND n.TemplateId={template} '
                  f'AND l.Mode={1 if mode == "temporarycleanup" else 2} AND l.Grade={grade} AND l.CreatorId={s.caster} '
                  'AND l.State=0 AND l.DeathObservedUtc IS NULL AND l.DeadlineUtc>UTC_TIMESTAMP(6)') == str(count))
    rows = s.sql(f'SELECT Id,HEX(EffectData) FROM Characters WHERE Id IN ({char_ids}) ORDER BY Id')
    for row in rows.splitlines():
        character, xml = row.split('\t', 1)
        effects = [node for node in ET.fromstring(bytes.fromhex(xml)).iter('Effect') if node.findtext('Type') == 'SpellNpcGuardian']
        s.check('exact saved creator bond for output ' + character,
                len(effects) == 1 and effects[0].findtext('Effect/CreatorId') == str(s.caster))
    active = census()
    s.check('all outputs are live native actors NPCs and bodies',
            all(active[name] == baseline[name] + count for name in ['Actors', 'NPCs', 'Bodies']))
    s.check('creator bond and native follow subscriptions grow by their code-proven amounts',
            active['Followers'] == baseline['Followers'] + count and
            active['Quit subscriptions'] == baseline['Quit subscriptions'] + 2 * count and
            all(active[name] == baseline[name] + count for name in ['Deleted subscriptions', 'Join combat subscriptions',
                'Death subscriptions', 'Start move subscriptions', 'Stop move subscriptions']))
    s.command('give staff qaguardian', 'give|hand')
    flush()
    carried = s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}')
    recipient = next((identifier for identifier, value in claims.items() if str(value['3']) == carried), None)
    s.check('foreign goods reach one exact newly claimed body', recipient is not None)
    room = s.sql(f'SELECT LocationId FROM CharacterInstances WHERE CharacterId={claims[recipient]["1"]} '
                 f'AND BodyId={carried} AND IsPrimary=1')
    s.check('exact primary custody room exists', room.isdigit())
    # Each cycle exercises native engagement and creator protection before cleanup.
    s.command('force qaguardian hit qaother', 'engage.*combat')
    response = s.command('force qaenemy hit qacaster', seconds=.3)
    s.check('guardian responds to the native creator engagement',
            'qaguardian spirit' in response and ('switch' in response.lower() or 'engage' in response.lower()), response)
    s.command('peace', 'end .* combats')
    deaths = 64 if batch else count if early else 0
    for _ in range(deaths):
        s.command('kill qaguardian', seconds=.3)
    flush()
    if deaths:
        s.check('exact early-death output count and timestamp preceding original deadline',
                s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE Id IN ({ids}) '
                      'AND DeathObservedUtc IS NOT NULL AND DeathObservedUtc<DeadlineUtc') == str(deaths))
    if mode == 'deathonexpiry':
        wait_for('every death-on-expiry output correlates one native corpse',
                 f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE Id IN ({ids}) AND RemainsItemId IS NOT NULL',
                 lambda value: value == str(count), 110)
        flush()
        remains = s.sql(f'SELECT Id,RemainsItemId FROM MagicSpellLifecycles WHERE Id IN ({ids}) ORDER BY Id')
        correlated = set()
        for row in remains.splitlines():
            identifier, corpse = row.split('\t')
            correlated.add(corpse)
            xml = s.sql(f'SELECT i.Definition FROM GameItemComponents i JOIN GameItemComponentProtos p '
                        f'ON p.Id=i.GameItemComponentProtoId AND p.RevisionNumber=i.GameItemComponentProtoRevision '
                        f'WHERE i.GameItemId={corpse} AND p.Type=\'Corpse\'')
            definition = ET.fromstring(xml)
            s.check('native corpse has exact loader character/body references',
                    definition.findtext('OriginalCharacter') == str(claims[identifier]['1']) and
                    definition.findtext('OriginalBody') == str(claims[identifier]['3']))
        s.check('exact corpse identity set has no duplicates or uncorrelated remains',
                len(correlated) == count and corpses() - corpse_baseline == correlated)
        s.check('foreign goods remain in the exact corpse body until native decay',
                s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}') == carried)
    wait_for('all outputs reach durable terminal state in the current process',
             f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE Id IN ({ids}) AND State=3',
             lambda value: value == str(count), 400 if batch else 160)
    flush()
    s.check('natural and early deaths have the exact expected timestamp relation',
            s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE Id IN ({ids}) AND DeathObservedUtc>=DeadlineUtc') == str(count - deaths))
    after = graphs()
    s.check('all eligible heavy identity sets return exactly to baseline before restart',
            all(after[name] == before[name] for name in ['bodies', 'npcs', 'instances']))
    s.check('every retained canonical identity archive lifecycle operation and ordinary item is explained',
            after['characters'] == before['characters'] | characters and
            after['archives'] == before['archives'] | characters and
            after['lifecycles'] == before['lifecycles'] | set(claims) and
            after['operations'] == current['operations'] and after['items'] == before['items'])
    expected_claims = before['claims'].splitlines() + [f'{identifier}\t{kind}\t{entity}\t0'
        for identifier, value in claims.items() for kind, entity in value.items()]
    s.check('all original typed claims survive without new claim roles',
            sorted(after['claims'].splitlines()) == sorted(expected_claims))
    for identifier, value in claims.items():
        s.check('exact lightweight archive retains original body and lifecycle',
                s.sql(f'SELECT c.IsArchived,c.BodyId,a.OriginalBodyId,a.LifecycleId FROM Characters c '
                      f'JOIN CharacterArchives a ON a.CharacterId=c.Id WHERE c.Id={value["1"]}') ==
                f'1\tNULL\t{value["3"]}\t{identifier}')
    s.check('foreign goods survive in the exact original room after physical cleanup',
            s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}') == room and
            not s.sql(f'SELECT BodyId FROM Bodies_GameItems WHERE GameItemId={item}'))
    s.check('native remains return to the original identity set', corpses() == corpse_baseline)
    samples = stable_census('runtime collections schedules and subscriptions return to baseline before restart', baseline)
    terminal = histories()
    old_ids = before['lifecycles']
    s.check('all preceding terminal and permanent journals remain byte-identical',
            '\n'.join(row for row in terminal[0].splitlines() if row.split('\t', 1)[0] in old_ids) == old_history[0])
    record = dict(index=index, batch=batch, mode=mode, earlyDeaths=deaths, outputCount=count, grade=grade,
                  claims=claims, foreignItem=item, recipient=recipient, room=int(room),
                  baseline=baseline, active=active, terminalSamples=samples,
                  terminalJournals=terminal[0], terminalClaims=terminal[1],
                  graphs=serial_graphs(after), energyBefore=energy, energyAfter=after_energy)
    s.report['cycles'].append(record)
    # A restart happens on every cycle after the live-process census has passed.
    s.stop()
    s.start()
    flush()
    s.check('cold restart never changes terminal journals claims or physical identity sets',
            histories() == terminal and graphs() == after)
    s.check('cold restart never replays payment', s.resource(s.caster) == after_energy)
    record['restartSamples'] = stable_census('cold restart restores the same runtime baseline', baseline)
    s.check('cold restart preserves foreign custody and the ordinary permanent guardian',
            s.sql(f'SELECT RoomId FROM Rooms_GameItems WHERE GameItemId={item}') == room and
            s.sql(f'SELECT BodyId,IsArchived,(State & 64)<>0 FROM Characters WHERE Id={permanent_character}') ==
            f'{permanent_body}\t0\t0')
    record['restartVerified'] = True


try:
    prior = json.loads((s.root / 'guardian-lifecycle-modes-passed.json').read_text(encoding='utf-8'))
    s.check('N15 installed-world prerequisite and cleanup are qualified',
            prior['status'] == prior['runnerStatus'] == 'PASS' and prior['n15BuilderInstalledQualified'] and prior['cleanup']['mysqlStopped'])
    if stage:
        preceding = json.loads((s.root / f'guardian-churn-{stage-1:02d}-passed.json').read_text(encoding='utf-8'))
        s.check('preceding stage is fresh source-identical qualified proof',
                preceding['status'] == preceding['runnerStatus'] == 'PASS' and preceding['n16Stage'] == stage - 1 and
                preceding['revision'] == s.report['revision'] and preceding['inputs'] == s.report['inputs'] and
                preceding['cleanup']['mysqlStopped'] and all(row['restartVerified'] for row in preceding['cycles']))
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text(encoding='utf-8'))
    for name in ['caster', 'capability', 'reserve']:
        setattr(s, name, actors[name])
    spell = prior['objects']['guardianSpell']
    template = prior['objects']['guardianTemplate']
    guardian_original = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={spell}')
    fury_original = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors["fury"]}')
    definition = ET.fromstring(bytes.fromhex(guardian_original).decode('utf-8'))
    s.check('exact qualified stock and template retained',
            definition.findtext('StockIdentity') == 'arm.spell.air_guardian' and
            definition.findtext('Effects/Effect/NPCPrototypeId') == str(template))
    permanent = next(row for row in prior['createdLifecycles'] if row['mode'] == 'permanent')
    permanent_character, permanent_body = permanent['character'], permanent['body']
    permanent_description = s.sql(f'SELECT HEX(ShortDescription) FROM Bodies WHERE Id={permanent_body}')
    permanent_names = s.sql(f'SELECT HEX(Name),HEX(NameInfo) FROM Characters WHERE Id={permanent_character}').split('\t')
    s.check('only the explained ordinary permanent guardian remains before churn',
            s.sql(f'SELECT GROUP_CONCAT(c.Id ORDER BY c.Id) FROM Npcs n JOIN Characters c ON c.Id=n.CharacterId '
                  f'WHERE n.TemplateId={template} AND (c.State & 64)=0') == str(permanent_character) and
            s.sql(f'SELECT COUNT(*) FROM MagicSpellLifecycles WHERE SpellId={spell} AND State<>3') == '0')
    s.check('controlled grade seven and sufficient pre-existing paid reserve',
            s.sql(f'SELECT ControlledGrade FROM CharacterAcquiredSpells WHERE CharacterId={s.caster} AND MagicSpellId={spell}') == '7' and
            float(s.sql(f'SELECT Amount FROM Characters_MagicResources WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}')) >= 18)
    item_proto = int(s.sql("SELECT LogicalId FROM SeederManagedRecords WHERE StableKey='arm.item.charged_staff'"))
    rows = s.sql("SELECT DISTINCT p.Id,p.RevisionNumber,p.MorphTimeSeconds,IFNULL(p.MorphGameItemProtoId,'NULL'),HEX(p.MorphEmote) "
                 "FROM GameItemProtos p JOIN GameItemProtos_GameItemComponentProtos pc ON pc.GameItemProtoId=p.Id AND pc.GameItemProtoRevision=p.RevisionNumber "
                 "JOIN GameItemComponentProtos c ON c.Id=pc.GameItemComponentProtoId AND c.RevisionNumber=pc.GameItemComponentRevision WHERE c.Type='Corpse'").splitlines()
    s.check('one code-proven native corpse prototype', len(rows) == 1)
    corpse_original = rows[0].split('\t')
    corpse_proto, corpse_revision, old_seconds, old_target, old_emote = corpse_original
    s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds=30,MorphGameItemProtoId=NULL,MorphEmote=CONVERT(UNHEX('{b'$0 crumbles into dust.'.hex()}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
    s.report['objects'] = dict(actors, guardianSpell=spell, guardianTemplate=template,
                              permanentCharacter=permanent_character, permanentBody=permanent_body,
                              foreignPrototype=item_proto)
    s.start()
    # Distinguish the retained permanent fixture with its ordinary native description editor.
    # This affects only this body, never the approved template or provenance; restore bytes in finally.
    s.command('resdesc qaguardian', 'editor')
    s.command('a qan16permanent spirit')
    s.command('@', seconds=.5)
    # Administrative target keywords include the true personal name as well.
    s.command(f'rename {permanent_character} Qan16permanent', 'You rename')
    flush()
    s.check('only the exact ordinary permanent body is renamed for safe keyword targeting',
            s.sql(f'SELECT ShortDescription FROM Bodies WHERE Id={permanent_body}') == 'a qan16permanent spirit' and
            s.sql("SELECT COUNT(*) FROM Bodies WHERE ShortDescription='a qaguardian spirit'") == '0')
    # Staff keyword matching also accepts personal names. Prove the actual resolver
    # has no old recipient before ever issuing a destructive guardian command.
    s.command('force qaguardian look', 'You do not see them here to force')
    s.possess('qacaster')
    s.command('return')
    s.session.read_for(12)
    if stage == 16:
        qualify_cycle(32, batch=True)
    else:
        for index in range(stage * 2, stage * 2 + 2):
            qualify_cycle(index)
    s.stop()
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    try:
        # Reserve cleanup time only; no qualification assertion may gain a longer deadline.
        s.deadline = max(s.deadline, time.monotonic() + 40)
        if s.process is not None:
            s.stop()
        if corpse_original is not None:
            s.sql(f"UPDATE GameItemProtos SET MorphTimeSeconds={old_seconds},MorphGameItemProtoId={old_target},MorphEmote=CONVERT(UNHEX('{old_emote}') USING utf8mb4) WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}")
            s.check('native corpse policy restored exactly',
                    s.sql(f'SELECT MorphTimeSeconds,IFNULL(MorphGameItemProtoId,\'NULL\'),HEX(MorphEmote) FROM GameItemProtos WHERE Id={corpse_proto} AND RevisionNumber={corpse_revision}') == '\t'.join(corpse_original[2:]))
        if guardian_original is not None:
            s.sql(f"UPDATE MagicSpells SET Definition=CONVERT(UNHEX('{guardian_original}') USING utf8mb4) WHERE Id={spell}")
            s.check('guardian definition restored byte-for-byte', s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={spell}') == guardian_original)
            s.check('Fury definition remains byte-identical', s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={actors["fury"]}') == fury_original)
        if permanent_description is not None:
            s.sql(f"UPDATE Bodies SET ShortDescription=CONVERT(UNHEX('{permanent_description}') USING utf8mb4) WHERE Id={permanent_body}")
            s.check('ordinary permanent body description restored exactly',
                    s.sql(f'SELECT HEX(ShortDescription) FROM Bodies WHERE Id={permanent_body}') == permanent_description)
        if permanent_names is not None:
            s.sql(f"UPDATE Characters SET Name=CONVERT(UNHEX('{permanent_names[0]}') USING utf8mb4),NameInfo=CONVERT(UNHEX('{permanent_names[1]}') USING utf8mb4) WHERE Id={permanent_character}")
            s.check('ordinary permanent canonical name fields restored exactly',
                    s.sql(f'SELECT HEX(Name),HEX(NameInfo) FROM Characters WHERE Id={permanent_character}') == '\t'.join(permanent_names))
        s.report['inputsUnchanged'] = all(hashlib.sha256((s.repo / name).read_bytes()).hexdigest() == digest for name, digest in s.report['inputs'].items())
        s.check('all captured source and binary inputs remain unchanged', s.report['inputsUnchanged'])
    except BaseException as error:
        s.report['status'] = 'FAIL'
        s.report['cleanupError'] = s.helper.redact(str(error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
