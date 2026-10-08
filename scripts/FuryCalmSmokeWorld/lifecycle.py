"""Native source counters, callbacks, ownership, expiry and authored rerun preservation."""
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
s.report['scope'] = 'native emotional lifecycles and builder/player preservation'
s.report['qualificationMarker'] = 'fury-calm-lifecycle-passed.json'
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
original_fury = original_calm = None
raw = attribute = None
attribute_name = None


def definition(spell):
    return ET.fromstring(s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={spell}'))


def profile(spell):
    return definition(spell).find('.//SourceProfile')


def setting(spell, value):
    s.command(f'magic spell edit {spell}', 'edit')
    s.command('magic spell set ' + value)
    s.command('magic spell close')
    s.command('impdebug flush', seconds=.6)


def parent(character, kind):
    matches = [x for x in s.effect_data(character).findall('Effect')
               if x.findtext('Type') == 'MagicSpellParent' and any(y.findtext('Type') == kind for y in x.iter('Effect'))]
    s.check('one native owned ' + kind + ' parent', len(matches) == 1)
    return matches[0]


def state(character, kind):
    matches = s.children(character, kind)
    s.check('one native ' + kind + ' child', len(matches) == 1)
    return matches[0].find('Effect')


def paid(name, grade, target, cost):
    before = s.resource(s.caster)
    answer = s.cast(name, grade, target)
    s.check(f'exact paid {name} grade {grade} cost {cost}', before - s.resource(s.caster) == cost, answer)
    return answer


def prog(label, body, return_type, parameters):
    name = 'qafc' + s.runtime.name.split('-')[-1] + label
    s.command('prog edit new ' + name)
    s.command('prog set return ' + return_type, 'return type')
    for pname, ptype in parameters:
        s.command(f'prog set parameter add {pname} {ptype}')
    s.command('prog set text')
    for line in body:
        s.command(line)
    s.command('@', 'compiled successfully')
    s.command('impdebug flush', seconds=.6)
    identifier = int(s.sql(f"SELECT Id FROM FutureProgs WHERE FunctionName='{name}'"))
    return identifier, name


def probe(name, character):
    keyword = {s.target: 'qatarget', s.caster: 'qacaster', s.other: 'qaother', s.enemy: 'qaenemy'}[character]
    output = s.command(f'prog execute {name} {keyword}', 'It returned')
    match = re.search(r'It returned\s+(-?\d[\d,]*(?:\.\d+)?)', output)
    s.check('native trait probe returned a number', match is not None, output)
    return float(match.group(1).replace(',', ''))


try:
    prior = json.loads((s.root / 'fury-calm-slice-passed.json').read_text())
    failures = json.loads((s.root / 'fury-calm-installer-failures-passed.json').read_text())
    s.check('paid persistence and installer failure prerequisites', prior['status'] == failures['status'] == 'PASS' and
            prior['runnerStatus'] == failures['runnerStatus'] == 'PASS' and
            prior['cleanup']['mysqlStopped'] and failures['cleanup']['mysqlStopped'])
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text())
    s.report['objects'] = actors
    for name in ['caster', 'target', 'other', 'enemy', 'reserve', 'capability']:
        setattr(s, name, actors[name])
    s.fury_id, s.calm_id = actors['fury'], actors['calm']
    s.fury_name = s.sql(f'SELECT Name FROM MagicSpells WHERE Id={s.fury_id}')
    s.calm_name = s.sql(f'SELECT Name FROM MagicSpells WHERE Id={s.calm_id}')
    original_fury = profile(s.fury_id)
    original_calm = profile(s.calm_id)
    attribute = int(original_fury.attrib['trait'])
    attribute_name = s.sql(f'SELECT Name FROM TraitDefinitions WHERE Id={attribute}')
    s.start()
    s.remove_emotions('qatarget')
    # This owned actor was authored with Constitution 10. A retained failed
    # fixture used an unsupported numeric SETATTRIBUTE token after its callback.
    # Restore the documented fixture baseline through the real staff command.
    s.command(f'setattribute qatarget "{attribute_name}" 10', 'set')
    _, trait_probe = prog('trait', [f'return gettrait(@target, totrait({attribute}))'], 'Number', [('target', 'Character')])
    raw = probe(trait_probe, s.target)
    s.check('owned fixture raw attribute baseline is ten', raw == 10)
    s.report['fixtureOriginalAttribute'] = dict(trait=attribute, value=raw, recovery='native staff command')
    paid(s.calm_name, 1, 'qatarget', 7)
    saved = s.effect_data(s.target)
    saved_calm = next(x for x in saved.iter('Effect') if x.findtext('Type') == 'SpellSourceCalm')
    saved_parent = next(x for x in saved.findall('Effect') if x.findtext('Type') == 'MagicSpellParent' and
                        any(y.findtext('Type') == 'SpellSourceCalm' for y in x.iter('Effect')))
    s.stop()
    offline_started = time.monotonic()
    print('Waiting 31 seconds with the MUD stopped for the Calm offline schedule assertion.', flush=True)
    time.sleep(31)
    offline_seconds = time.monotonic() - offline_started
    s.start()
    loaded = s.children(s.target, 'SpellSourceCalm')
    s.check('cold restart preserves exact Calm child', len(loaded) == 1 and ET.tostring(loaded[0]) == ET.tostring(saved_calm))
    remaining_delta = float(saved_parent.findtext('Remaining')) - float(parent(s.target, 'SpellSourceCalm').findtext('Remaining'))
    s.check('Calm offline lifetime pauses', offline_seconds > 30 and 0 <= remaining_delta < min(30000, offline_seconds * 1000),
            json.dumps(dict(offlineSeconds=offline_seconds, activeDecrementMs=remaining_delta)))
    s.remove_emotions('qatarget')
    # Controlled grade 7: source efficiency is max(integer 50/(7-grade+1), minimum).
    # These independent expectations exercise the selected stock policy, not cost*grade.
    s.check('fixture retains controlled grade seven for both spells',
            s.sql(f'SELECT COUNT(*) FROM CharacterAcquiredSpells WHERE CharacterId={s.caster} AND ControlledGrade=7 AND MagicSpellId IN ({s.fury_id},{s.calm_id})') == '2')
    paid(s.fury_name, 2, 'qatarget', 20)
    first_state = state(s.target, 'SpellSourceFury')
    first_parent = parent(s.target, 'SpellSourceFury')
    s.check('native configured attribute bonus uses captured endurance units',
            probe(trait_probe, s.target) == raw + float(first_state.attrib['endurancePoints']) * float(first_state.attrib['unitsPerSourcePoint']))
    paid(s.fury_name, 1, 'qatarget', 20)
    recast_state = state(s.target, 'SpellSourceFury')
    recast_parent = parent(s.target, 'SpellSourceFury')
    s.check('weaker Fury recast retains child state and parent ownership',
            ET.tostring(first_state) == ET.tostring(recast_state) and
            first_parent.findtext('Effect/Identity') == recast_parent.findtext('Effect/Identity'))
    s.check('Fury recast extends within configured cap',
            float(first_parent.findtext('Remaining')) + 1000000 < float(recast_parent.findtext('Remaining')) <= 21600000)
    for _ in range(9):
        paid(s.fury_name, 1, 'qatarget', 20)
    s.check('native Fury recasts stop at the configured 36-unit cap',
            21570000 <= float(parent(s.target, 'SpellSourceFury').findtext('Remaining')) <= 21600000 and
            ET.tostring(state(s.target, 'SpellSourceFury')) == ET.tostring(first_state))
    paid(s.calm_name, 1, 'qatarget', 7)
    weakened = state(s.target, 'SpellSourceFury')
    s.check('weaker opposing Calm reduces only retained source grade', weakened.attrib['grade'] == '1' and
            weakened.attrib['endurancePoints'] == first_state.attrib['endurancePoints'] and not s.children(s.target, 'SpellSourceCalm'))
    paid(s.calm_name, 2, 'qatarget', 8)
    residual = state(s.target, 'SpellSourceCalm')
    s.check('stronger Calm applies residual grade after removing Fury', residual.attrib['grade'] == '1' and not s.children(s.target, 'SpellSourceFury'))
    paid(s.fury_name, 1, 'qatarget', 20)
    s.check('reciprocal Fury exhausts equal Calm without adding Fury', not s.children(s.target, 'SpellSourceFury') and not s.children(s.target, 'SpellSourceCalm'))
    paid(s.fury_name, 1, 'self', 20)
    s.check('supported self recipient owns Fury', len(s.children(s.caster, 'SpellSourceFury')) == 1)
    s.remove_emotions('qacaster')
    paid(s.fury_name, 1, 'qaother', 20)
    independent = s.children(s.other, 'SpellSourceFury')[0]
    independent_parent = parent(s.other, 'SpellSourceFury').findtext('Effect/Identity')
    paid(s.fury_name, 1, 'qatarget', 20)
    s.remove_emotions('qatarget')
    s.check('parent removal removes only owned recipient state', not s.children(s.target, 'SpellSourceFury') and
            ET.tostring(s.children(s.other, 'SpellSourceFury')[0]) == ET.tostring(independent) and
            parent(s.other, 'SpellSourceFury').findtext('Effect/Identity') == independent_parent)
    s.remove_emotions('qaother')
    s.check('removal restores ordinary native attribute', probe(trait_probe, s.target) == raw)
    refusing, _ = prog('refuse', [f'settrait(@target, totrait({attribute}), gettrait(@target, totrait({attribute})) + 1)', 'return false'],
                       'Boolean', [('target', 'Character'), ('caster', 'Character')])
    setting(s.fury_id, f'effect 1 eligibility {refusing}')
    balance = s.resource(s.caster)
    response = s.cast(s.fury_name, 1, 'qatarget')
    s.check('native callback refusal occurs before payment and preserves callback mutation',
            'eligibility' in response.lower() and s.resource(s.caster) == balance and
            not s.children(s.target, 'SpellSourceFury') and probe(trait_probe, s.target) == raw + 1, response)
    setting(s.fury_id, 'effect 1 eligibility ' + original_fury.attrib['eligibility'])
    s.command(f'setattribute qatarget "{attribute_name}" {raw}', 'set')
    s.check('fixture callback mutation restored by native command', probe(trait_probe, s.target) == raw)
    group = original_fury.find('Lifetime').attrib['group']
    setting(s.fury_id, f'effect 1 lifetime {group} 8 3')
    s.check('authored short native lifetime selected', profile(s.fury_id).find('Lifetime').attrib['seconds'] == '8')
    paid(s.fury_name, 1, 'qatarget', 20)
    expiring = s.children(s.target, 'SpellSourceFury')[0]
    short_parent = parent(s.target, 'SpellSourceFury')
    s.check('short retained Fury has a positive bounded deadline', 0 < float(short_parent.findtext('Remaining')) <= 24000)
    setting(s.fury_id, f'effect 1 lifetime {group} 9 3')
    balance = s.resource(s.caster)
    refused = s.cast(s.fury_name, 1, 'qatarget')
    s.check('changed retained mapping refuses before payment', s.resource(s.caster) == balance and
            ET.tostring(s.children(s.target, 'SpellSourceFury')[0]) == ET.tostring(expiring), refused)
    setting(s.fury_id, f'effect 1 lifetime {group} 8 3')
    limit = time.monotonic() + 35
    while s.children(s.target, 'SpellSourceFury') and time.monotonic() < limit:
        s.session.read_for(.5)
    s.check('scheduled expiry removes owned Fury child', not s.children(s.target, 'SpellSourceFury'))
    s.check('expiry restores ordinary native attribute', probe(trait_probe, s.target) == raw)
    setting(s.fury_id, f"effect 1 lifetime {group} {original_fury.find('Lifetime').attrib['seconds']} {original_fury.find('Lifetime').attrib['cap']}")
    paid(s.calm_name, 2, 'qatarget', 8)
    stronger_calm = state(s.target, 'SpellSourceCalm')
    calm_before = parent(s.target, 'SpellSourceCalm')
    paid(s.calm_name, 1, 'qatarget', 7)
    s.check('weaker Calm recast retains stronger source state and extends its deadline',
            ET.tostring(state(s.target, 'SpellSourceCalm')) == ET.tostring(stronger_calm) and
            float(parent(s.target, 'SpellSourceCalm').findtext('Remaining')) > float(calm_before.findtext('Remaining')) + 1000000)
    for _ in range(10):
        paid(s.calm_name, 1, 'qatarget', 7)
    s.check('native Calm recasts stop at the configured 24-unit cap',
            14370000 <= float(parent(s.target, 'SpellSourceCalm').findtext('Remaining')) <= 14400000 and
            state(s.target, 'SpellSourceCalm').attrib['grade'] == '2')
    s.remove_emotions('qatarget')
    calm_group = original_calm.find('Lifetime').attrib['group']
    setting(s.calm_id, f'effect 1 lifetime {calm_group} 8 3')
    paid(s.calm_name, 1, 'qatarget', 7)
    s.check('short native Calm has a positive two-unit deadline', 0 < float(parent(s.target, 'SpellSourceCalm').findtext('Remaining')) <= 16000)
    limit = time.monotonic() + 25
    while s.children(s.target, 'SpellSourceCalm') and time.monotonic() < limit:
        s.session.read_for(.5)
    s.check('scheduled expiry removes owned Calm child', not s.children(s.target, 'SpellSourceCalm'))
    setting(s.calm_id, f"effect 1 lifetime {calm_group} {original_calm.find('Lifetime').attrib['seconds']} {original_calm.find('Lifetime').attrib['cap']}")
    setting(s.fury_id, 'effect 1 intensity 2')
    s.check('authored builder profile override persisted', profile(s.fury_id).attrib['intensity'] == '2')
    paid(s.calm_name, 1, 'qatarget', 7)
    s.stop()
    s.check('all preservation checksum tables exist',
            s.sql("SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA=DATABASE() AND LOWER(TABLE_NAME) IN ('characters','bodies','characters_magicresources','characteracquiredspells','charactercastingenrolments','npcs','traits')") == '7')
    player_checksum = s.sql('CHECKSUM TABLE Characters, Bodies, Characters_MagicResources, CharacterAcquiredSpells, CharacterCastingEnrolments, Npcs, Traits EXTENDED')
    builder_definition = s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={s.fury_id}')
    installer = s.fixture('rerun')
    s.check('selected and null reruns preserve actual builder profile', s.sql(f'SELECT Definition FROM MagicSpells WHERE Id={s.fury_id}') == builder_definition)
    s.check('reruns preserve player and NPC tables', s.sql('CHECKSUM TABLE Characters, Bodies, Characters_MagicResources, CharacterAcquiredSpells, CharacterCastingEnrolments, Npcs, Traits EXTENDED') == player_checksum)
    s.report['installer'] = installer
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    if s.report['status'] != 'PASS' and original_fury is not None and original_calm is not None:
        try:
            if s.process is None:
                s.start()
            s.command('return')
            for name in ['qacaster', 'qatarget', 'qaother']:
                s.remove_emotions(name)
            for spell, original_profile in [(s.fury_id, original_fury), (s.calm_id, original_calm)]:
                lifetime = original_profile.find('Lifetime').attrib
                setting(spell, 'effect 1 eligibility ' + original_profile.attrib['eligibility'])
                setting(spell, 'effect 1 intensity ' + original_profile.attrib['intensity'])
                setting(spell, f"effect 1 lifetime {lifetime['group']} {lifetime['seconds']} {lifetime['cap']}")
                s.check('failure cleanup restores original native emotional mapping',
                        profile(spell).attrib['eligibility'] == original_profile.attrib['eligibility'] and
                        profile(spell).attrib['intensity'] == original_profile.attrib['intensity'] and
                        profile(spell).find('Lifetime').attrib == lifetime)
            if raw is not None:
                s.command(f'setattribute qatarget "{attribute_name}" {raw}', 'set')
                s.check('failure cleanup restores original native raw attribute', probe(trait_probe, s.target) == raw)
            s.stop()
            s.report['fixtureRestoration'] = 'PASS: temporary mappings and raw attribute restored; payment retained'
        except BaseException as cleanup_error:
            s.report['fixtureRestoration'] = 'FAIL: ' + s.helper.redact(str(cleanup_error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
