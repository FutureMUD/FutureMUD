"""Pure deterministic selection/contract checks; this module performs no native or SQL I/O."""
from __future__ import annotations
import hashlib
import math
import re
import xml.etree.ElementTree as ET

SEED = 20261013
SAFE_TARGETS = {
    'arm.spell.read_attunement': 'self', 'arm.spell.night_eyes': 'self',
    'arm.spell.second_breath': 'self', 'arm.spell.daylight': 'here',
    'arm.spell.quickened_healing': 'self', 'arm.spell.earthen_strength': 'self',
}
EXPECTED_CHILDREN = {'arm.spell.night_eyes': 'SpellInfravision', 'arm.spell.daylight': 'SpellRoomLight',
                     'arm.spell.quickened_healing': 'SpellHealingRate', 'arm.spell.earthen_strength': 'SpellTraitBoost'}
REFERENCE_TABLES = {
    'MagicSchool': 'MagicSchools', 'TraitDefinition': 'TraitDefinitions',
    'TraitExpression': 'TraitExpression', 'MagicResource': 'MagicResources',
    'FutureProg': 'FutureProgs', 'NpcTemplate': 'NPCTemplates',
    'GameItemProto': 'GameItemProtos', 'GameItemSkin': 'GameItemSkins',
    'Liquid': 'Liquids', 'Material': 'Materials', 'Tag': 'Tags',
    'MagicCapability': 'MagicCapabilities', 'Terrain': 'Terrains',
    'BodyProto': 'BodyProtos', 'Race': 'Races', 'CharacteristicDefinition': 'CharacteristicDefinitions',
    'AutobuilderRoomTemplate': 'AutobuilderRoomTemplates', 'Room': 'Rooms',
    'ArtificialIntelligence': 'ArtificialIntelligences', 'Plane': 'Planes',
}

def require(value, message):
    if not value: raise ValueError(message)

def positive(value, label):
    require(type(value) is int and value > 0, label + ' requires a positive typed native identity.')
    return value

def reference_schema(rows):
    result = {}
    for row in rows:
        kind = row['kind']
        require(kind in REFERENCE_TABLES and kind not in result, 'Unknown or duplicate typed EF kind.')
        for field in ('table', 'id_column'):
            require(isinstance(row[field], str) and re.fullmatch(r'[A-Za-z_][A-Za-z_0-9]*', row[field]), 'Unsafe typed EF identifier.')
        result[kind] = row
    require(result, 'Missing explicit typed EF reference schema.')
    return result

def definition_contract(entry):
    key = entry['key']
    require(isinstance(key, str) and re.fullmatch(r'arm\.spell\.[a-z_]+', key), 'Invalid canonical spell key.')
    positive(entry['id'], key)
    require(isinstance(entry['name'], str) and re.fullmatch(r'[A-Za-z][A-Za-z ]+', entry['name']), 'Unsafe canonical command name.')
    root = ET.fromstring(entry['definition'])
    require(root.findtext('StockIdentity') == key, 'Canonical StockIdentity differs: ' + key)
    controlled = root.find('ControlledPower')
    require(controlled is not None and controlled.get('schema') == '1', 'Missing controlled-grade contract: ' + key)
    grades = controlled.findall('Grade')
    require([int(row.get('number', '0')) for row in grades] == list(range(1, 8)), 'Not exactly grades 1..7: ' + key)
    require(all(math.isfinite(float(row.get('skill', 'nan'))) and math.isfinite(float(row.get('difficulty', 'nan'))) for row in grades), 'Nonfinite grade contract: ' + key)
    costs = root.findall('Costs/Cost')
    require(len(costs) == 1 and int(costs[0].get('resource', '0')) == positive(entry['cost_resource'], 'Cost resource') and
            int(costs[0].get('expression', '0')) == positive(entry['cost_expression'], 'Cost expression'), 'Exact cost references differ: ' + key)
    for field in ('school', 'trait', 'duration_expression', 'known_prog'):
        positive(entry[field], field)
    refs = {(row['kind'], positive(row['id'], row['kind'])) for row in entry['typed_references']}
    require(all(kind in REFERENCE_TABLES for kind, _ in refs), 'Unsupported typed reference kind: ' + key)
    expected = {('MagicSchool', entry['school']), ('TraitDefinition', entry['trait']),
                ('TraitExpression', entry['duration_expression']), ('FutureProg', entry['known_prog']),
                ('MagicResource', entry['cost_resource']), ('TraitExpression', entry['cost_expression'])}
    if entry.get('resisting_trait', 0): expected.add(('TraitDefinition', positive(entry['resisting_trait'], 'Resisting trait')))
    # Explicit native XML labels only; no names/species or guessed numeric authority.
    labels = {'TargetFilterProg': 'FutureProg', 'FilterProg': 'FutureProg', 'OnLoadProg': 'FutureProg',
              'NPCPrototypeId': 'NpcTemplate', 'ItemPrototypeId': 'GameItemProto', 'ItemSkinId': 'GameItemSkin',
              'LiquidId': 'Liquid', 'Resource': 'MagicResource'}
    for node in root.iter():
        if node.tag in labels and node.text and node.text.strip().isdigit() and int(node.text) > 0:
            expected.add((labels[node.tag], int(node.text)))
        if node.tag == 'Template' and node.get('grade'):
            expected.add(('NpcTemplate', int(node.text or '0')))
        if node.tag == 'Effect' and node.get('trait'):
            expected.add(('TraitDefinition', int(node.get('trait'))))
    require(expected.issubset(refs), 'Prepared typed references omit native definition dependencies: ' + key)
    require(not root.findall('CasterEffects/Effect'), 'Unexpected stock caster effects: ' + key)
    return root, sorted(refs)

def select_campaign(config, installed):
    require(config.get('version') == 1 and config.get('seed') == SEED, 'Campaign version/seed differs.')
    limit = config['sample_limit']
    require(type(limit) is int and 1 <= limit <= 4, 'Campaign sample_limit must be 1..4.')
    candidates = config['candidates']
    keys = [row['key'] for row in candidates]
    require(len(set(keys)) == len(keys) and set(keys).issubset(SAFE_TARGETS), 'Duplicate or unsafe campaign candidate.')
    for row in candidates:
        key = row['key']; entry = installed[key]
        require(row['spell'] == entry['id'] and row['target'] == SAFE_TARGETS[key], 'Campaign uses another spell/target: ' + key)
        if key == 'arm.spell.read_attunement': continue # Preserved original paid Read assertions.
        for field in ('capability', 'merit', 'trait', 'school', 'resource', 'source'):
            positive(row[field], field)
        require(row['trait'] == entry['trait'] and row['school'] == entry['school'], 'Campaign source trait/school drift: ' + key)
        require(isinstance(row['invoked'], str) and re.fullmatch(r'[a-z][a-z0-9]*', row['invoked']), 'Unsafe school command.')
        cost = row['grade_one_cost']
        require(type(cost) in (int, float) and math.isfinite(cost) and 0 < cost <= 100, 'Campaign cost exceeds finite Self-gather budget.')
        require(row.get('gather_alias', 'draw') == 'draw', 'Campaign needs the explicit Self draw fixture.')
        stamina = row.get('gather_stamina_cost', 1)
        require(type(stamina) in (int, float) and math.isfinite(stamina) and 0 < stamina <= 20, 'Campaign requires its explicit finite stamina expenditure.')
        require(type(row['duration_seconds']) in (int, float) and math.isfinite(row['duration_seconds']) and row['duration_seconds'] >= 0, 'Invalid expected grade-one duration.')
    return sorted(candidates, key=lambda row: (hashlib.sha256(f'{SEED}:{row["key"]}'.encode()).hexdigest(), row['key']))[:limit]

def selection_receipt(selected):
    return {'seed': SEED, 'selection_algorithm': 'sha256(seed:key), ordinal key tie-break',
            'selected': [{'key': row['key'], 'spell': row['spell'], 'target': row['target'], 'grade': 1,
                          'coverage': 'inherited-original-read' if row['key'] == 'arm.spell.read_attunement' else 'direct-paid-route',
                          'outcome': 'PENDING'} for row in selected]}

if __name__ == '__main__':
    import unittest
    class ContractTests(unittest.TestCase):
        def rows(self):
            return [{'key': key, 'spell': index + 1, 'target': target, 'capability': 10 + index, 'merit': 20 + index,
                     'trait': 30, 'school': 40, 'resource': 50, 'source': 60, 'invoked': 'armtest',
                     'grade_one_cost': 50, 'duration_seconds': 60} for index, (key, target) in enumerate(SAFE_TARGETS.items())]
        def test_repeatable_order_independent_bounded_selection(self):
            rows = self.rows(); installed = {row['key']: {'id': row['spell'], 'trait': 30, 'school': 40} for row in rows}
            a = select_campaign({'version': 1, 'seed': SEED, 'sample_limit': 4, 'candidates': rows}, installed)
            b = select_campaign({'version': 1, 'seed': SEED, 'sample_limit': 4, 'candidates': list(reversed(rows))}, installed)
            self.assertEqual([row['key'] for row in a], [row['key'] for row in b]); self.assertEqual(4, len(a))
            self.assertTrue(all(row['grade'] == 1 for row in selection_receipt(a)['selected']))
        def test_invalid_configuration_refuses_before_native_io(self):
            rows = self.rows(); installed = {row['key']: {'id': row['spell'], 'trait': 30, 'school': 40} for row in rows}
            for limit in (0, 5, True):
                with self.assertRaises(ValueError): select_campaign({'version': 1, 'seed': SEED, 'sample_limit': limit, 'candidates': rows}, installed)
            for bad in (float('nan'), float('inf'), 0, 101):
                changed = [dict(row, grade_one_cost=bad) for row in rows]
                with self.assertRaises(ValueError): select_campaign({'version': 1, 'seed': SEED, 'sample_limit': 4, 'candidates': changed}, installed)
        def test_receipt_keeps_inherited_coverage_explicit(self):
            record = selection_receipt(self.rows())
            self.assertEqual('inherited-original-read', record['selected'][0]['coverage'])
            self.assertTrue(all(row['outcome'] == 'PENDING' for row in record['selected']))
        def test_exact_definition_identity_grade_and_typed_dependencies(self):
            entry = {'key': 'arm.spell.night_eyes', 'id': 1, 'name': 'Night Eyes', 'school': 2, 'trait': 3,
                     'duration_expression': 4, 'known_prog': 5, 'cost_resource': 6, 'cost_expression': 7,
                     'typed_references': [{'kind': kind, 'id': identity} for kind, identity in
                         [('MagicSchool',2), ('TraitDefinition',3), ('TraitExpression',4), ('FutureProg',5), ('MagicResource',6), ('TraitExpression',7)]]}
            definition = '<Definition><StockIdentity>arm.spell.night_eyes</StockIdentity><Costs><Cost resource="6" expression="7" /></Costs><ControlledPower schema="1">' + ''.join(f'<Grade number="{grade}" skill="0" difficulty="0" />' for grade in range(1,8)) + '</ControlledPower><CasterEffects /></Definition>'
            entry['definition'] = definition
            self.assertEqual(6, len(definition_contract(entry)[1]))
            for broken in (definition.replace('night_eyes','second_breath'), definition.replace('number="7"','number="8"'), definition.replace('resource="6"','resource="8"')):
                with self.assertRaises(ValueError): definition_contract(dict(entry, definition=broken))
            with self.assertRaises(ValueError): definition_contract(dict(entry, typed_references=entry['typed_references'][:-1]))
        def test_only_declared_typed_ef_identifiers_are_admitted(self):
            row = {'kind': 'Room', 'table': 'Rooms', 'id_column': 'Id'}
            self.assertEqual(row, reference_schema([row])['Room'])
            for changed in (dict(row, table='Rooms;DROP TABLE Characters'), dict(row, kind='GuessedKind'), dict(row, id_column='Id` OR 1=1')):
                with self.assertRaises(ValueError): reference_schema([changed])
            with self.assertRaises(ValueError): reference_schema([row, row])
    unittest.main()
