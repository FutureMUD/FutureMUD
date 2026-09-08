"""Report exact supplied native rules against the isolated source inventory. No inferred fallback."""
import json
from pathlib import Path

root = Path(__file__).resolve().parents[2]
handoff = root / 'Design Documents/Seeding/CultureSeederRedesignHandoff/data'
read = lambda name: json.loads((handoff / name).read_text(encoding='utf-8'))
stages = json.loads((root / 'Design Documents/Verification/CultureSeeder_Redesign_Source_Staging.json').read_text(encoding='utf-8'))
assert all(x['Status'] == 'staged' for x in stages), 'Source staging must succeed before binding audit.'
sources = {x['SourcePack']: x for x in stages}
legacy = read('legacy_native_language_rules.json')
defaults = read('ethnicity_language_defaults.json')
overlays = read('ethnicity_reframing_overlay.json')
selectors = read('vernacular_selectors.json')
by_ethnicity = {x['ethnicity']: x['by_era'] for x in defaults['defaults']}
scope = {
    'antiquity': ['earthantiquity'],
    'darkages': ['earthdarkagesandmedieval'],
    'medieval': ['earthdarkagesandmedieval', 'earthrenaissanceeurope'],
    'renaissance': ['earthrenaissanceeurope', 'earthrenaissanceworldexpansion'],
    'earlymodern': ['earthrenaissanceeurope', 'earthrenaissanceworldexpansion'],
}
results = []
for era, modules in scope.items():
    for module in modules:
        for entry in sources[module]['Ethnicities']:
            name = entry['Name']
            native = []
            rule = None
            if module == 'earthantiquity' and name in legacy['antiquity_ethnicity_bindings']:
                native = [legacy['antiquity_ethnicity_bindings'][name]]
                rule = 'antiquity-source-rule'
            for override in defaults['retained_regional_overrides']:
                if native or name not in override['match_names']:
                    continue
                if 'language' in override:
                    native = [override['language']]
                elif 'selector' in override:
                    selected = selectors[override['selector']]['by_era'].get(era)
                    native = [selected] if selected else []
                else:
                    native = by_ethnicity[override['ethnicity_default']].get(era, [])
                if native:
                    rule = 'retained-regional-override'
            matches = [x for x in overlays if era in x['packs'] and name in [x['label'], *x['legacy_aliases']]]
            if not native and len(matches) == 1:
                native = by_ethnicity[matches[0]['key']].get(era, [])
                if native:
                    rule = 'exact-overlay-label-or-authored-alias'
            template_languages = {legacy['historical_template_bindings'][x] for x in entry['NamingStructures']
                                  if x in legacy['historical_template_bindings']}
            if not native and len(template_languages) == 1:
                native = sorted(template_languages)
                rule = 'supplied-source-template-rule'
            results.append({
                'era': era, 'source_identity': f'{module}:ethnicity:{name}',
                'source_name': name, 'source_naming_structures': entry['NamingStructures'],
                'native_references': native, 'binding_rule': rule,
                'status': 'rule-resolved-language-ids-pending' if native else 'source-crosswalk-required',
            })
output = {'scope': 'Exact supplied rule audit against source-stage definitions. Module composition is an audit scope, not an activated pack. Canonical and legacy language references still require actual installed source-qualified IDs.', 'bindings': results}
path = root / 'Design Documents/Verification/CultureSeeder_Redesign_Native_Source_Audit.json'
path.write_text(json.dumps(output, indent=2, ensure_ascii=False) + '\n', encoding='utf-8')
for era in scope:
    rows = [x for x in results if x['era'] == era]
    missing = [x for x in rows if not x['native_references']]
    print(f'{era}: {len(rows) - len(missing)}/{len(rows)} exact rules; {len(missing)} source crosswalks still required.')
    for row in missing:
        print('  ' + row['source_identity'] + ' | ' + ', '.join(row['source_naming_structures']))
