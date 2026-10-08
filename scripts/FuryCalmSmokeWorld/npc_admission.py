"""Bounded native NPC prepayment admission in the retained, owned restored world."""
import hashlib
import importlib.util
import json
import pathlib
import time
import xml.etree.ElementTree as ET

spec = importlib.util.spec_from_file_location('fury_calm_smoke', pathlib.Path(__file__).with_name('smoke.py'))
s = importlib.util.module_from_spec(spec)
spec.loader.exec_module(s)
s.report['scope'] = 'known-invalid NPC lifetime refusal before payment and real selected-grade creation/expiry'
s.report['qualificationMarker'] = 'npc-admission-passed.json'
s.report['inputs'][str(pathlib.Path(__file__).relative_to(s.repo))] = hashlib.sha256(pathlib.Path(__file__).read_bytes()).hexdigest()
s.report['archiveClassificationInputs'] = {
    name: hashlib.sha256((s.repo / name).read_bytes()).hexdigest() for name in [
        'MudSharpCore/Character/CharacterArchiveService.cs',
        'MudSharpCore/Character/NpcArchiveReferencePolicy.cs',
        'MudSharpCore/Character/NpcArchiveReferencePolicy.Agriculture.cs']}
original_hex = None
family = 'qa-npc-' + s.runtime.name.split('-')[-1]


def write_definition(hex_text):
    assert s.process is None, 'Authored fixture changes require a stopped MUD'
    s.sql(f"UPDATE MagicSpells SET Definition=CONVERT(UNHEX('{hex_text}') USING utf8mb4) WHERE Id={s.fury_id}")
    s.check('exact authored fixture bytes persisted', s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}') == hex_text.upper())


def configure(template, formula):
    document = ET.fromstring(bytes.fromhex(original_hex))
    trigger = document.find('Trigger')
    s.check('native fixture has a direct trigger', trigger is not None)
    trigger.clear()
    trigger.set('type', 'room')
    ET.SubElement(trigger, 'MinimumPower').text = '0'
    ET.SubElement(trigger, 'MaximumPower').text = '10'
    effects = document.find('Effects')
    s.check('native fixture has target effects', effects is not None)
    effects.clear()
    effect = ET.SubElement(effects, 'Effect', {'type': 'createnpc'})
    ET.SubElement(effect, 'NPCPrototypeId').text = str(template)
    ET.SubElement(effect, 'OnLoadProg').text = '0'
    lifecycle = ET.SubElement(effect, 'Lifecycle', {'version': '1', 'mode': 'TemporaryCleanup'})
    ET.SubElement(lifecycle, 'Family').text = family
    ET.SubElement(lifecycle, 'Seconds').text = formula
    # Scalar bindings belong to the replaced payload. Payment, grade, plan and admission stay authored.
    for parent in document.iter():
        for child in list(parent):
            if child.tag == 'ScalarBindings':
                parent.remove(child)
    write_definition(ET.tostring(document, encoding='utf-8').hex())


def snapshot():
    s.command('impdebug flush', seconds=.6)
    counts = s.sql('SELECT (SELECT COUNT(*) FROM Characters),(SELECT COUNT(*) FROM Bodies),'
                 '(SELECT COUNT(*) FROM Npcs),(SELECT COUNT(*) FROM MagicSpellLifecycles),'
                 '(SELECT COUNT(*) FROM MagicSpellOwnedEntities),(SELECT COUNT(*) FROM MagicCastingOperations),'
                 '(SELECT COUNT(*) FROM CharacterMagicSkillOpportunities)')
    records = {table: s.sql(f'SELECT * FROM {table} WHERE CharacterId={s.caster} ORDER BY {key}')
               for table, key in [('MagicCastingOperations', 'Id'),
                                  ('CharacterMagicSkillOpportunities', 'TraitDefinitionId'),
                                  ('CharacterAcquiredSpells', 'MagicSpellId'),
                                  ('CharacterCastingEnrolments', 'CapabilityIdentity')]}
    return dict(counts=counts, records=records)


wait_retired = s.wait_npc_retired

try:
    for marker in ['fury-calm-slice-passed.json', 'fury-calm-installer-failures-passed.json',
                   'fury-calm-lifecycle-passed.json', 'fury-calm-combat-passed.json']:
        prior = json.loads((s.root / marker).read_text())
        s.check('qualified prerequisite ' + marker, prior['status'] == prior['runnerStatus'] == 'PASS' and prior['cleanup']['mysqlStopped'])
    expected_uuid = next(line.split('=', 1)[1].strip() for line in
                         (pathlib.Path(s.instance['data']) / 'auto.cnf').read_text().splitlines()
                         if line.startswith('server-uuid='))
    s.check('owned instance UUID matches retained data', s.sql('SELECT @@server_uuid') == expected_uuid)
    actors = json.loads((s.root / 'fury-calm-actors.json').read_text())
    s.report['objects'] = actors
    for name in ['caster', 'target', 'other', 'enemy', 'reserve', 'capability']:
        setattr(s, name, actors[name])
    s.fury_id = actors['fury']
    s.fury_name = s.sql(f'SELECT Name FROM MagicSpells WHERE Id={s.fury_id}')
    original_hex = s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}')
    (s.runtime / 'original-fury.xml').write_bytes(bytes.fromhex(original_hex))
    s.report['originalDefinitionSha256'] = hashlib.sha256(bytes.fromhex(original_hex)).hexdigest()
    s.report['archiveReferenceEvidence'] = s.sql('SELECT Id,HEX(Definition) FROM AgricultureOperations ORDER BY Id')
    s.report['archiveCropReferenceEvidence'] = s.sql('SELECT Id,HEX(Definition) FROM AgricultureCropDefinitions ORDER BY Id')
    s.start()
    candidates = s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaspawn'").splitlines()
    s.check('unique private native template', len(candidates) <= 1)
    if not candidates:
        source = s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='Alaric Stone'")
        s.check('one approved source native template', source.isdigit())
        s.command(f'npc clone {source} "qaspawn"', 'clone|copy|template')
        s.command('npc set sdesc a qaspawn human', 'description')
        s.command('npc edit close')
        s.command('impdebug flush', seconds=.6)
    template = int(s.sql("SELECT DISTINCT Id FROM NpcTemplates WHERE Name='qaspawn'"))
    s.report['objects']['spawnTemplate'] = template
    s.check('native approved simple template', s.sql(f'SELECT Type FROM NpcTemplates WHERE Id={template}') == 'Simple')
    s.stop()
    for formula in ['0', '-1', '1/0', '1e30']:
        configure(template, formula)
        s.start()
        before = snapshot()
        reserve_before = s.resource(s.caster)
        acquisition = s.sql(f'SELECT * FROM CharacterAcquiredSpells WHERE CharacterId={s.caster} AND MagicSpellId={s.fury_id}')
        output = s.cast(s.fury_name, 1, 'here')
        s.check('native known-invalid lifetime refuses: ' + formula, any(word in output.lower() for word in ['lifetime', 'finite', 'positive', 'overflow']), output)
        s.check('invalid lifetime has no payment: ' + formula, s.resource(s.caster) == reserve_before)
        s.check('invalid lifetime creates no rows, operation or opportunity: ' + formula, snapshot() == before)
        s.report.setdefault('invalidAdmissionSnapshots', []).append(dict(formula=formula,
            sha256=hashlib.sha256(json.dumps(before, sort_keys=True).encode()).hexdigest(), unchanged=True))
        s.check('invalid lifetime preserves acquisition: ' + formula,
                s.sql(f'SELECT * FROM CharacterAcquiredSpells WHERE CharacterId={s.caster} AND MagicSpellId={s.fury_id}') == acquisition)
        s.stop()
    s.report['createdLifecycles'] = []
    for formula in ['grade*45', '45+outcome*5']:
        configure(template, formula)
        s.start()
        before = s.resource(s.caster)
        previous = set(s.sql(f"SELECT Id FROM MagicSpellLifecycles WHERE Family='{family}'").splitlines())
        output = s.cast(s.fury_name, 1, 'here')
        s.check('native admitted creation pays exact grade-one cost: ' + formula, before - s.resource(s.caster) == 20, output)
        created = set(s.sql(f"SELECT Id FROM MagicSpellLifecycles WHERE Family='{family}'").splitlines()) - previous
        s.check('one actual owned native creation: ' + formula, len(created) == 1)
        identifier = created.pop()
        row = s.sql(f"SELECT Grade,CreatorId,Mode,State,TIMESTAMPDIFF(MICROSECOND,CreatedUtc,DeadlineUtc) FROM MagicSpellLifecycles WHERE Id='{identifier}'").split('\t')
        s.check('selected-grade creator and temporary lifecycle provenance', row[:4] == ['1', str(s.caster), '1', '0'], str(row))
        seconds = int(row[4]) / 1000000
        s.check('actual resolved finite lifetime: ' + formula, seconds == 45 if formula == 'grade*45' else 45 <= seconds <= 75, str(seconds))
        s.check('exact created character and body ownership claims',
                s.sql(f"SELECT GROUP_CONCAT(Kind ORDER BY Kind) FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' AND Role=0") == '1,3')
        claims = dict(line.split('\t') for line in s.sql(f"SELECT Kind,EntityId FROM MagicSpellOwnedEntities WHERE LifecycleId='{identifier}' ORDER BY Kind").splitlines())
        character, body = int(claims['1']), int(claims['3'])
        s.check('real approved template NPC graph is exposed',
                s.sql(f"SELECT COUNT(*) FROM Npcs n JOIN Characters c ON c.Id=n.CharacterId JOIN Bodies b ON b.Id=c.BodyId JOIN MagicSpellOwnedEntities e ON e.Kind=1 AND e.EntityId=c.Id WHERE e.LifecycleId='{identifier}' AND n.TemplateId={template} AND b.ShortDescription='a qaspawn human'") == '1')
        s.command('look', 'qaspawn')
        s.report['createdLifecycles'].append(dict(id=identifier, formula=formula, seconds=seconds, character=character, body=body))
        s.stop()
    s.report['prepaymentCasesPassed'] = True
    # Keep the original compaction gate: a held graph is retained as failed qualification.
    s.start()
    for life in s.report['createdLifecycles']:
        wait_retired(life['id'], life['character'], life['body'])
    s.stop()
    s.report['status'] = 'PASS'
except BaseException as error:
    s.report['status'] = 'FAIL'
    s.report['error'] = s.helper.redact(str(error), [s.connection, s.password])
finally:
    if original_hex is not None:
        try:
            if s.process is not None:
                s.stop()
            write_definition(original_hex)
            s.report['fixtureRestoration'] = 'PASS: exact original Fury definition restored; payments retained'
            s.start()
            s.command(f'magic spell show {s.fury_id}', 'Roused Fury|Source Fury')
            s.stop()
            s.check('original definition survives restored runtime reload', s.sql(f'SELECT HEX(Definition) FROM MagicSpells WHERE Id={s.fury_id}') == original_hex)
        except BaseException as cleanup_error:
            s.report['status'] = 'FAIL'
            s.report['fixtureRestoration'] = 'FAIL: ' + s.helper.redact(str(cleanup_error), [s.connection, s.password])
    s.finish()
raise SystemExit(0 if s.report['status'] == 'PASS' else 1)
