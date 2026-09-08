"""Summarise executed fixture receipts; never substitute fixture IDs for live IDs."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
VERIFY = ROOT / 'Design Documents/Verification'
fixtures = json.loads((VERIFY / 'CultureSeeder_Redesign_Installer_Fixtures.json').read_text())['Results']
if len(fixtures) != 10 or any(x['Status'] != 'passed' for x in fixtures):
    raise SystemExit('All ten era/order fixture receipts must pass before generating completion tables.')
eras = [x for x in fixtures if not x['CultureFirst']]
pools = {x['key']: x for x in json.loads((ROOT / 'Design Documents/Seeding/CultureSeederRedesignHandoff/data/targeted_name_corpora.json').read_text())['pools']}
evidence = []
for item in eras:
    for n in item['Report']['TargetedNames']:
        if n['SourceKey'].endswith('.shared'):
            continue
        # These fresh receipts have exactly one profile per gender. Both ordered
        # fields are emitted from the same profile collection by the C# importer.
        assert len(n['ProfileIds']) == len(n['GivenCounts'])
        for profile, gender in zip(n['ProfileIds'], n['GivenCounts']):
            label = {'2': 'male', '3': 'female'}[gender]
            entries = [x for x in pools[n['SourceKey']]['given_' + label] if item['Era'] in x['packs']]
            assert len(entries) == n['GivenCounts'][gender]
            evidence.extend(dict(Era=item['Era'], RepertoireKey=n['SourceKey'], NameCultureId=n['NameCultureId'],
                                 FixtureProfileId=profile, Gender=label, SuppliedEntry=x) for x in entries)
(VERIFY / 'CultureSeeder_Redesign_Naming_Evidence.json').write_text(json.dumps(dict(
    Scope='Derived binding receipt: exact supplied entry metadata associated with fresh C# fixture profile IDs. Not live MySQL IDs or new historical research.',
    Entries=evidence), ensure_ascii=False, indent=2) + '\n', encoding='utf-8')
lines = ['# CultureSeeder content receipt', '',
         'Generated from the executed C# installer fixtures. IDs in that fixture JSON belong to isolated InMemory databases. '
         'Actual Medieval MySQL IDs are separately recorded in `CultureSeeder_Redesign_Live_MySQL.json` and '
         '`CultureSeeder_Redesign_Live_Identities.json`. These counts do not certify demographic coverage or new historical attestation.', '',
         '## Era composition', '', '| Era | Social backgrounds | Retained/overlay identities | Resolved native bindings | Distinct languages | Optional groups | Directed edges |',
         '|---|---:|---:|---:|---:|---:|---:|']
for item in eras:
    r = item['Report']
    lines.append(f"| {item['Era']} | {len(r['CultureIds'])} | {len(r['EthnicityIds'])} | {sum(x['IsResolved'] for x in r['NativeBindings'])} | {len(set(r['LanguageIds'].values()))} | {len(r['Groups'])} | {len(r['Intelligibility'])} |")
groups = {g['StableKey'] for x in eras for g in x['Report']['Groups']}
cultures = {k for x in eras for k in x['Report']['CultureIds']}
overlays = {k for x in eras for k in x['Report']['EthnicityIds'] if k.startswith('ethnicity.')}
lines += ['', f'Across the five manifests: **{len(cultures)}** unique social backgrounds, **{len(overlays)}** canonical overlays and **{len(groups)}** consumer recipes. '
          'The JSON receipts enumerate each group, compiled eligibility reference, candidate trait ID and directed listener/target edge.', '',
          '## Targeted given-name counts', '',
          'Counts are installed local given-name entries, by eligible era and gender. The shared neutral profile is a separate union and is not added to male/female suggestion counts. '
          'A zero means no replacement for that gender; the original source-specific link remains. Entries retain the supplied evidence classes, dates, original spelling and lemma metadata in the handoff.', '',
          'The [naming evidence receipt](./CultureSeeder_Redesign_Naming_Evidence.json) associates each active supplied entry and its unchanged source/date/evidence fields with its era and fresh fixture profile ID.', '',
          '| Era | Repertoire | Male | Female |', '|---|---|---:|---:|']
for item in eras:
    for n in item['Report']['TargetedNames']:
        if n['SourceKey'].endswith('.shared'):
            continue
        lines.append(f"| {item['Era']} | {n['SourceKey']} | {n['GivenCounts'].get('2', 0)} | {n['GivenCounts'].get('3', 0)} |")
lines += ['', '## Explicit exclusions', '',
          'The feminine Old Prussian replacement remains inactive in every era. Small delivered local feminine selections (Latvian 14, Estonian 10, Romanian 17 across their supplied scopes) are disclosed; era-specific eligibility can yield smaller counts above. '
          'Documentary, editorial, dynastic, devotional and literary evidence are distinct. Existing profiles outside the targeted replacements remain preserved, not newly certified. '
          'No neighbouring name pool fills a gap, and no productive surname morphology, adopted-name selector or new historical research was added.', '',
          '### Retained native identities without supplied bindings', '',
          'These retained source records remain unavailable in the toolkit. Their original source is recoverable; absence of an approved native crosswalk is not a claim that a people or language did not exist. '
          'Many lie outside the selected geography. Builders must resolve any desired additional binding explicitly.', '']
for item in eras:
    unresolved = [x for x in item['Report']['NativeBindings'] if not x['IsResolved']]
    lines.append(f"- **{item['Era']}**: " + ('; '.join(x['SourceIdentity'] for x in unresolved) if unresolved else 'None.') )
lines += ['', '### Omitted group candidates', '']
omitted = [(x['Era'], g['StableKey'], c) for x in eras for g in x['Report']['Groups'] for c in g['OmittedCandidates']]
lines += [f'- {era}: {group}: {candidate}' for era, group, candidate in omitted] or ['None in these fresh full-content fixtures.']
output = VERIFY / 'CultureSeeder_Redesign_Content_Receipt.md'
output.write_text('\n'.join(lines) + '\n', encoding='utf-8')
print(f'{len(cultures)} backgrounds, {len(overlays)} overlays, {len(groups)} recipes summarised in {output.name}.')
