"""Native selective cessation, admitted miss, resistance and replacement callbacks."""
import hashlib
import importlib.util
import json
import pathlib
import re
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report['scope'] = 'native selective combat cessation, admitted missed attack and callback quarantine'
s.report['qualificationMarker'] = 'fury-calm-combat-passed.json'
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
original = None
actor_state = {}
difficulty_names = ['Automatic', 'Trivial', 'ExtremelyEasy', 'VeryEasy', 'Easy', 'Normal', 'Hard', 'VeryHard', 'ExtremelyHard', 'Insane', 'Impossible']


def setting(value):
    s.command(f'magic spell edit {s.calm_id}', 'edit')
    s.command('magic spell set ' + value)
    s.command('magic spell close')
    s.command('impdebug flush', seconds=.6)


def prog(label, body, parameters):
    name = 'qafc' + s.runtime.name.split('-')[-1] + label
    s.command('prog edit new ' + name)
    s.command('prog set return Boolean', 'return type')
    for pname, ptype in parameters:
        s.command(f'prog set parameter add {pname} {ptype}')
    s.command('prog set text')
    for line in body:
        s.command(line)
    s.command('@', 'compiled successfully')
    s.command('impdebug flush', seconds=.6)
    return int(s.sql(f"SELECT Id FROM FutureProgs WHERE FunctionName='{name}'")), name


def combat(name):
    return s.command('combat status ' + name)


def linked(output, actor, opponent):
    return re.search(r'(?im)^.*\b' + actor + r'\b.*(?:melee with|attacking|fighting).*\b' + opponent + r'\b', output) is not None


def no_combat(name):
    result = combat(name)
    s.check(name + ' is outside combat', 'not engaged in combat' in result.lower(), result)


def cease_fixture():
    # Administrative cleanup only; the selected spell assertion precedes this command.
    s.command('peace', 'end .* combats')
    names = ['qacaster', 'qatarget', 'qaother', 'qaenemy']
    if 'guard' in s.report['objects']:
        names.append('qaguard')
    limit = time.monotonic() + 30
    while True:
        cooling = [name for name in names if 'Unable to voluntarily engage in combat' in s.command('effect list ' + name, 'Effects for')]
        if not cooling:
            break
        s.check('native engage recovery remains within thirty-second bound', time.monotonic() < limit, ','.join(cooling))
        s.session.read_for(.5)


def paid_calm():
    before = s.resource(s.caster)
    output = s.cast(s.calm_name, 1, 'qatarget')
    s.check('native Calm deducts exactly seven energy', before - s.resource(s.caster) == 7, output)
    return output


def start_expression(reference, opponent, join_name='', leave_name=''):
    # GetByName("") abbreviates the first world prog. Use an explicitly absent
    # name for omitted callbacks and record that separate native API finding.
    absent = 'qafcabsent' + s.runtime.name.split('-')[-1]
    participant = f'@target.location.characters.First(ch, @ch.id == {opponent})'
    return f'startcombat("{reference}", "{reference}", "sparring", true, @target, {participant}, "{join_name or absent}", "{leave_name or absent}", "{absent}", "{absent}", "{absent}")'


def skill_value(character, trait):
    # TraitDefinition normalizes legacy body-scoped skills to character scope.
    # Match that runtime contract rather than reading obsolete body skill rows.
    scope = s.sql(f'SELECT CASE WHEN Type IN (0,2,4) AND OwnerScope=0 THEN 1 ELSE OwnerScope END FROM TraitDefinitions WHERE Id={trait}')
    s.check('trait has an explicit body or character owner scope', scope in ('0', '1'), str(trait))
    if scope == '1':
        return s.sql(f'SELECT Value FROM CharacterTraits WHERE CharacterId={character} AND TraitDefinitionId={trait}')
    return s.sql(f'SELECT Value FROM Traits WHERE BodyId=(SELECT BodyId FROM Characters WHERE Id={character}) AND TraitDefinitionId={trait}')


def restore_skill(name, character, trait, value):
    s.command(f'skill level {name} {trait} {value}' if value != '' else f'skill remove {name} {trait}')
    s.command('impdebug flush', seconds=.6)
    s.check('restored native trait value or absence at its real owner', skill_value(character, trait) == value,
            f'{name}/{trait}: expected {value!r}')


def restore_actors():
    defense_names = {0: 'none', 1: 'dodge', 2: 'block', 4: 'parry', 8: 'magic'}
    for name, saved in actor_state.items():
        s.possess(name)
        s.command(f"combat set {saved['setting']}", 'setting')
        s.command('combat defense ' + defense_names[saved['defense']], 'prefer')
        s.command('return')
        for trait, value in saved['skills'].items():
            restore_skill(name, saved['id'], trait, value)
        s.command('impdebug flush', seconds=.6)
        s.check('restored native combat settings and defense for ' + name,
                s.sql(f"SELECT CurrentCombatSettingId,PreferredDefenseType FROM Characters WHERE Id={saved['id']}") ==
                f"{saved['setting']}\t{saved['defense']}")


def recover_failed_fixture(actors):
    global actor_state
    path = s.root / 'fury-calm-fixture-recovery.json'
    if not path.exists():
        return
    recovery = json.loads(path.read_text())
    previous_path = (s.root / recovery['failedReceipt']).resolve()
    s.check('recovery receipt belongs to this owned world', previous_path.is_relative_to(s.root))
    previous = json.loads(previous_path.read_text())
    s.check('recovery concerns a failed owned invocation', previous['status'] == 'FAIL' and
            all(previous['objects'][key] == actors[key] for key in actors))
    actor_state = previous['originalActorSettings']
    restore_actors()
    actor_state = {}
    for value in recovery['spellSettings']:
        setting(value)
    recovered = ET.fromstring(s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={s.calm_id}')).find('.//SourceProfile')
    expected = recovery['expectedSpellSettings']
    s.check('failed fixture recovery restores persisted Calm save and casting settings',
            int(recovered.find("Saves/Grade[@number='1']").attrib['difficulty']) == expected['saveDifficulty'] and
            s.sql(f'SELECT MinimumSuccessThreshold,CastingDifficulty FROM MagicSpells WHERE Id={s.calm_id}') ==
            f"{expected['threshold']}\t{expected['difficulty']}")
    for row in recovery['skills']:
        restore_skill(row['name'], actors[row['actor']], row['trait'], row['value'])
    s.report['priorFixtureRecovery'] = recovery
    s.report['priorFixtureRecovery']['verified'] = True
    path.rename(s.root / ('fixture-recovery-' + s.runtime.name + '.json'))


try:
    for prerequisite in ['slice', 'installer-failures', 'lifecycle']:
        prior = json.loads((s.root / f'fury-calm-{prerequisite}-passed.json').read_text())
        s.check(prerequisite + ' prerequisite', prior['status'] == 'PASS' and prior['runnerStatus'] == 'PASS' and prior['cleanup']['mysqlStopped'])
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text())
    s.report['objects'] = actors
    for name in ['caster', 'target', 'other', 'enemy', 'reserve', 'capability']:
        setattr(s, name, actors[name])
    s.fury_id, s.calm_id = actors['fury'], actors['calm']
    s.calm_name = s.sql(f'SELECT Name FROM MagicSpells WHERE Id={s.calm_id}')
    s.start()
    recover_failed_fixture(actors)
    original = ET.fromstring(s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={s.calm_id}')).find('.//SourceProfile')
    threshold = int(s.sql(f'SELECT MinimumSuccessThreshold FROM MagicSpells WHERE Id={s.calm_id}'))
    original_difficulty = int(s.sql(f'SELECT CastingDifficulty FROM MagicSpells WHERE Id={s.calm_id}'))
    threshold_names = ['None', 'NotTested', 'MajorFail', 'Fail', 'MinorFail', 'MinorPass', 'Pass', 'MajorPass']
    cast_skill = int(s.sql(f'SELECT CastingTraitDefinitionId FROM MagicSpells WHERE Id={s.calm_id}'))
    original_skill = skill_value(s.caster, cast_skill)
    save_trait = int(original.attrib['trait'])
    original_save_skill = skill_value(s.target, save_trait)
    s.report['originalSpellSettings'] = dict(profile=ET.tostring(original, encoding='unicode'),
        threshold=threshold, difficulty=original_difficulty, castTrait=cast_skill, castValue=original_skill,
        saveTrait=save_trait, saveValue=original_save_skill)
    # Funding is fixture preparation with a stopped server, never a cast refund.
    before_funding = s.resource(s.caster)
    if before_funding < 100:
        s.stop()
        s.sql(f'UPDATE Characters_MagicResources SET Amount=1000 WHERE CharacterId={s.caster} AND MagicResourceId={s.reserve}')
        s.report['fixtureFunding'] = dict(before=before_funding, after=1000, purpose='bounded combat qualification', refund=False)
        s.start()
        s.check('explicit stopped-server fixture funding', s.resource(s.caster) == 1000)
    absent = 'qafcabsent' + s.runtime.name.split('-')[-1]
    s.check('omitted callback sentinel names resolve no world prog', s.sql(f"SELECT COUNT(*) FROM FutureProgs WHERE FunctionName LIKE '{absent}%'") == '0')
    s.report['adjacentFinding'] = 'StartCombat optional empty callback names abbreviate the first FutureProg through All.GetByName and can fail signature admission. Explicitly absent names select no callback in this fixture; production repair is deferred.'
    for name in ['qacaster', 'qatarget', 'qaother', 'qaenemy']:
        s.remove_emotions(name)
    cease_fixture()
    guard_count = int(s.sql("SELECT COUNT(*) FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription='a qaguard human'"))
    s.check('owned guard actor is absent or unique', guard_count in (0, 1))
    if not guard_count:
        template = int(s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='Alaric Stone'"))
        s.command(f'npc clone {template} "qaguard"', 'clone|copy|template')
        s.command('npc set sdesc a qaguard human', 'description')
        s.command('impdebug flush', seconds=.6)
        guard_template = int(s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaguard'"))
        s.command(f'npc load {guard_template}', 'load|human|appears')
        s.command('impdebug flush', seconds=.6)
    guard = int(s.sql("SELECT c.Id FROM Characters c JOIN Bodies b ON b.Id=c.BodyId WHERE b.ShortDescription='a qaguard human'"))
    s.report['objects']['guard'] = guard
    s.report['nativeCombatDefinitions'] = dict(
        skills=s.sql("SELECT Id,Name,Alias,Type FROM TraitDefinitions WHERE Name IN ('Brawling','Brawl','Dodging','Dodge')"),
        checks=s.sql('SELECT c.Type,e.Expression FROM Checks c JOIN TraitExpression e ON e.Id=c.TraitExpressionId WHERE c.Type IN (48,49) ORDER BY c.Type'))
    settings_seed = int(s.sql('SELECT MIN(Id) FROM CharacterCombatSettings WHERE GlobalTemplate=1'))
    # Resolve the actual native check bindings. This replay contains both Dodge
    # and Dodging, so a name/alias heuristic cannot establish which is used.
    check_rows = s.report['nativeCombatDefinitions']['checks'].splitlines()
    bindings = {int(row.split('\t')[0]): re.fullmatch(r'\w+:(\d+)', row.split('\t')[1]) for row in check_rows}
    s.check('native attack and defense checks each bind one real skill', set(bindings) == {48, 49} and all(bindings.values()), json.dumps(s.report['nativeCombatDefinitions']))
    brawling = int(bindings[48].group(1))
    dodge = int(bindings[49].group(1))
    for name, identifier in [('qatarget', s.target), ('qaother', s.other), ('qaenemy', s.enemy), ('qaguard', guard), ('qacaster', s.caster)]:
        values = s.sql(f'SELECT CurrentCombatSettingId,PreferredDefenseType FROM Characters WHERE Id={identifier}').split('\t')
        s.check('native actor has restorable combat settings', len(values) == 2 and values[0] != 'NULL' and int(values[1]) in [0, 1, 2, 4, 8])
        actor_state[name] = dict(id=identifier, setting=int(values[0]), defense=int(values[1]), skills={
            trait: skill_value(identifier, trait)
            for trait in ([brawling, dodge] if name != 'qacaster' else [])})
    s.report['originalActorSettings'] = actor_state
    # Each actor receives private settings through public commands. Suppress autonomous
    # attacks; a selected HIT remains a real native admitted attack and resolution.
    for name in ['qatarget', 'qaother', 'qaenemy', 'qaguard', 'qacaster']:
        s.possess(name)
        owned_setting = s.sql(f"SELECT Id FROM CharacterCombatSettings WHERE CharacterOwnerId={actor_state[name]['id']} AND GlobalTemplate=0 AND Name LIKE 'QA fury-calm-% {name}' ORDER BY Id LIMIT 1")
        if owned_setting:
            s.command(f'combat set {owned_setting}', 'setting')
        else:
            s.command(f'combat clone {settings_seed} "QA {s.runtime.name} {name}"', 'clone')
        s.command('combat config melee FullDefense', 'Changed the melee strategy')
        s.command('combat defense dodge', 'dodg')
        s.command('return')
    for name in ['qatarget', 'qaother', 'qaenemy', 'qaguard']:
        s.command(f'skill level {name} {brawling} 0')
        s.command(f'skill level {name} {dodge} 99')
    s.command('force qatarget hit qaguard')
    s.command('force qaother hit qaenemy')
    s.command('force qaguard hit qaother')
    before = combat('qaother')
    s.check('native four-party combat has the selected and unrelated target links',
            all(name in before.lower() for name in ['qatarget', 'qaguard', 'qaother', 'qaenemy']) and
            linked(before, 'qatarget', 'qaguard') and linked(before, 'qaother', 'qaenemy') and linked(before, 'qaenemy', 'qaother'), before)
    paid_calm()
    no_combat('qatarget')
    after = combat('qaother')
    s.check('Calm preserves unrelated native target links in the same combat',
            linked(after, 'qaother', 'qaenemy') and linked(after, 'qaenemy', 'qaother') and 'qatarget' not in after.lower(), after)
    s.check('selectively ceased recipient owns Calm', len(s.children(s.target, 'SpellSourceCalm')) == 1)
    cease_fixture()
    s.remove_emotions('qatarget')

    attempts = []
    for trial in range(8):
        paid_calm()
        s.check('Calm exists immediately before incoming attack', len(s.children(s.target, 'SpellSourceCalm')) == 1)
        s.possess('qaguard')
        s.command('hit qatarget', 'engage .* in combat')
        s.command('return')
        s.check('engagement alone preserves Calm before the selected attack', len(s.children(s.target, 'SpellSourceCalm')) == 1)
        s.possess('qaguard')
        output = s.command('kick qatarget', seconds=.25)
        limit = time.monotonic() + 8
        while not re.search(r'dodges? out of the way|hit on|\bmiss(?:es)?\b', output, re.I) and time.monotonic() < limit:
            output += s.session.read_for(.25)
        s.command('return')
        lines = [line for line in output.splitlines() if re.search(r'dodges? out of the way|hit on|\bmiss(?:es)?\b', line, re.I)]
        s.check('observed first completed admitted attack', bool(lines), output)
        first = lines[0]
        missed = bool(re.search(r'dodges? out of the way|\bmiss(?:es)?\b', first, re.I)) and not re.search(r'hit on|attempts? to dodge', first, re.I)
        removed = not s.children(s.target, 'SpellSourceCalm')
        attempts.append(dict(trial=trial, firstAttack=first, missed=missed, calmRemoved=removed))
        cease_fixture()
        s.remove_emotions('qatarget')
        if missed:
            s.check('Calm breaks on an admitted incoming attack that misses', removed, first)
            break
    s.report['missAttempts'] = attempts
    s.check('bounded scenario actually observed a missed first attack', any(x['missed'] and x['calmRemoved'] for x in attempts))

    # FORCE is a native FutureProg statement. It gives the two initial ProgCombat
    # participants real target links inside the callback before its first schedule.
    _, join_guard = prog('join', [f'if (@joiner.id == {guard})', f'force @joiner.location.characters.First(ch, @ch.id == {s.target}) "hit qaguard"', 'end if', 'return true'],
                         [('joiner', 'Character'), ('reference', 'Text')])
    _, begin = prog('begin', ['return ' + start_expression('QAFuryCalmResistance', guard, join_guard)], [('target', 'Character')])
    setting('effect 1 save 1 Automatic')
    setting('threshold MajorFail')
    setting('difficulty Insane')
    s.command(f'skill level qacaster {cast_skill} 0')
    s.command(f'skill level qatarget {save_trait} 99')
    s.command(f'prog execute {begin} qatarget', 'It returned True')
    resisting_combat = combat('qatarget')
    s.check('native resisted-case combat has the intended target link before casting',
            'QAFuryCalmResistance' in resisting_combat and linked(resisting_combat, 'qatarget', 'qaguard'), resisting_combat)
    paid_calm()
    no_combat('qatarget')
    s.check('resisted native Calm ceases combat without retaining a child', not s.children(s.target, 'SpellSourceCalm'))
    operation = ET.fromstring(s.sql(f'SELECT Definition FROM MagicCastingOperations WHERE CharacterId={s.caster} AND MagicSpellId={s.calm_id} ORDER BY UpdatedUtc DESC LIMIT 1'))
    s.check('resistance uses an actual admitted failed original caster roll', operation.attrib['outcome'] in ['MajorFail', 'Fail', 'MinorFail'], ET.tostring(operation, encoding='unicode'))
    cease_fixture()
    setting('effect 1 save 1 ' + difficulty_names[int(original.find("Saves/Grade[@number='1']").attrib['difficulty'])])
    setting('threshold ' + threshold_names[threshold])
    setting('difficulty ' + difficulty_names[original_difficulty])
    restore_skill('qacaster', s.caster, cast_skill, original_skill)
    restore_skill('qatarget', s.target, save_trait, original_save_skill)

    drift, _ = prog('drift', ['return ' + start_expression('QAFuryCalmDrift', guard, join_guard)], [('target', 'Character'), ('caster', 'Character')])
    setting('effect 1 eligibility ' + str(drift))
    balance = s.resource(s.caster)
    refusal = s.cast(s.calm_name, 1, 'qatarget')
    s.check('combat-changing eligibility callback refuses before payment and preserves its combat',
            s.resource(s.caster) == balance and 'QAFuryCalmDrift' in combat('qatarget') and not s.children(s.target, 'SpellSourceCalm'), refusal)
    setting('effect 1 eligibility ' + original.attrib['eligibility'])
    cease_fixture()

    _, join_caster = prog('joinnew', [f'if (@joiner.id == {s.caster})', f'force @joiner.location.characters.First(ch, @ch.id == {s.target}) "hit qacaster"', 'end if', 'return true'],
                          [('joiner', 'Character'), ('reference', 'Text')])
    _, leave = prog('replace', [f'if (@target.id != {s.target})', 'return false', 'end if',
                               'return ' + start_expression('QAFuryCalmReplacement', s.caster, join_caster)],
                    [('target', 'Character'), ('reference', 'Text')])
    _, begin_replace = prog('beginreplace', ['return ' + start_expression('QAFuryCalmOriginal', guard, join_guard, leave)], [('target', 'Character')])
    s.command(f'prog execute {begin_replace} qatarget', 'It returned True')
    s.check('native callback-bearing original combat exists', 'QAFuryCalmOriginal' in combat('qatarget'))
    paid_calm()
    replacement = combat('qatarget')
    s.check('paid cessation callback preserves replacement combat and applies no new Calm child',
            'QAFuryCalmReplacement' in replacement and linked(replacement, 'qatarget', 'qacaster') and not s.children(s.target, 'SpellSourceCalm'), replacement)
    ids = s.sql(f"SELECT Id FROM MagicCastingOperations WHERE CharacterId={s.caster} AND Stage='NeedsReview'").splitlines()
    s.check('actual paid operation is durably quarantined', len(ids) == 1)
    operation_id = ids[0]
    s.report['quarantinedOperation'] = operation_id
    diagnostic = s.sql(f"SELECT Diagnostic FROM MagicCastingOperations WHERE Id='{operation_id}'")
    s.check('quarantine records the callback replacement boundary', 'cessation callbacks replaced combat' in diagnostic, diagnostic)
    balance = s.resource(s.caster)
    refused = s.cast(s.calm_name, 1, 'qatarget')
    s.check('quarantine refuses a fresh cast without another payment', s.resource(s.caster) == balance and 'review' in refused.lower(), refused)
    cease_fixture()
    s.stop()
    s.start()
    s.check('cold restart preserves paid quarantine and exact deducted reserve',
            s.sql(f"SELECT Stage FROM MagicCastingOperations WHERE Id='{operation_id}'") == 'NeedsReview' and s.resource(s.caster) == balance)
    s.check('cold restart does not replay a quarantined Calm child', not s.children(s.target, 'SpellSourceCalm'))
    refused = s.cast(s.calm_name, 1, 'qatarget')
    s.check('restarted quarantine refuses before payment', s.resource(s.caster) == balance and 'review' in refused.lower(), refused)
    s.command(f'magic casting resolve {s.caster} {operation_id} Disposable native callback reviewed; no refund or replay', 'Reconciled')
    s.check('staff reconciliation retains payment and does not replay effects',
            s.sql(f"SELECT Stage FROM MagicCastingOperations WHERE Id='{operation_id}'") == 'Reconciled' and
            s.resource(s.caster) == balance and not s.children(s.target, 'SpellSourceCalm'))
    cease_fixture()
    restore_actors()
    s.stop()
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    if s.report['status'] != 'PASS' and original is not None:
        try:
            if s.process is None:
                s.start()
            s.command('return')
            setting('effect 1 eligibility ' + original.attrib['eligibility'])
            setting('effect 1 save 1 ' + difficulty_names[int(original.find("Saves/Grade[@number='1']").attrib['difficulty'])])
            setting('threshold ' + threshold_names[threshold])
            setting('difficulty ' + difficulty_names[original_difficulty])
            restore_skill('qacaster', s.caster, cast_skill, original_skill)
            restore_skill('qatarget', s.target, save_trait, original_save_skill)
            for _ in range(3):
                cease_fixture()
            restore_actors()
            s.stop()
            restored = ET.fromstring(s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={s.calm_id}')).find('.//SourceProfile')
            s.check('failure cleanup restores original native Calm callback and save mappings',
                    restored.attrib['eligibility'] == original.attrib['eligibility'] and
                    restored.find("Saves/Grade[@number='1']").attrib == original.find("Saves/Grade[@number='1']").attrib)
            s.check('failure cleanup restores native casting settings and skill',
                    s.sql(f'SELECT MinimumSuccessThreshold,CastingDifficulty FROM MagicSpells WHERE Id={s.calm_id}') == f'{threshold}\t{original_difficulty}' and
                    skill_value(s.caster, cast_skill) == original_skill)
            s.check('failure cleanup restores native target save skill row or absence',
                    skill_value(s.target, save_trait) == original_save_skill)
            s.report['fixtureRestoration'] = 'PASS: temporary Calm settings and caster skill restored; payment retained'
        except BaseException as cleanup_error:
            s.report['fixtureRestoration'] = 'FAIL: ' + s.helper.redact(str(cleanup_error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
