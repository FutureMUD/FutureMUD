#!/usr/bin/env python3
"""Validate delivered authoring data, not the C# engine or historical attestation."""
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import sys
import unicodedata

ERAS = ['antiquity', 'darkages', 'medieval', 'renaissance', 'earlymodern']
REQUIRED = [
 'README.md','AGENT_TASK.md','01_SCOPE_AND_FINDING_DISPOSITION.md',
 '02_IMPLEMENTATION_BRIEF.md','03_CONTENT_CATALOGUE.md','04_ACCEPTANCE_AND_VERIFICATION.md',
 'data/finding_disposition.json','data/native_binding_patch.json','data/accent_era_policy.json',
 'data/targeted_name_corpora.json','data/name_playability_policy.json','data/regression_cases.json',
 'data/runtime_resource_manifest.json','research/sources.json','research/original_name_evidence.json',
 'research/name_expansion_ledger.json','research/RESEARCH_METHOD.md',
 'tools/validate_handoff.py','tools/extract_embedded_pack.py'
]

def validate(root: Path) -> dict:
    failures: list[str] = []
    checks = 0
    def check(condition: bool, message: str) -> None:
        nonlocal checks
        checks += 1
        if not condition: failures.append(message)
    def read(name: str):
        return json.loads((root / name).read_text(encoding='utf-8'))
    for name in REQUIRED: check((root/name).is_file(), 'Missing file: '+name)
    if failures: return {'passed':False,'data_checks':checks,'failures':failures}
    for path in root.rglob('*.json'):
        try: json.loads(path.read_text(encoding='utf-8'))
        except (ValueError, UnicodeError) as e: check(False, f'Invalid JSON {path}: {e}')
        else: check(True, str(path))
    pools = read('data/targeted_name_corpora.json')['pools']
    policy = read('data/name_playability_policy.json')
    sources = read('research/sources.json')
    source_ids = {s['id'] for s in sources}
    check(len(source_ids)==len(sources),'Duplicate source ID')
    original = {e['key']:e for e in read('research/original_name_evidence.json')['entries']}
    check(len(original)==389,'Expected 389 preserved original evidence entries')
    all_entries = {}
    active_counts = []
    new_entries = 0
    for pool in pools:
        key=pool['key']
        check(all(r in ERAS for r in pool['packs']),key+' invalid pool era')
        check(bool(pool['description'].strip()),key+' missing player description')
        check(all(ord(c)<=255 for c in pool['description']),key+' non-Latin1 description')
        for sex in ('male','female'):
            entries = pool['given_'+sex]
            recipe=pool['production_recipe']['profiles'][sex]
            check(recipe['enabled'],key+' disabled '+sex)
            check(recipe['given_dice']=='1' and recipe['byname_dice']=='0',key+' changed dice contract')
            check(recipe['given_pool']==[e['key'] for e in entries if e['playable_packs']],key+' recipe membership mismatch '+sex)
            for entry in entries:
                ek=entry['key'];check(ek not in all_entries,'Duplicate entry '+ek);all_entries[ek]=entry
                text=entry['display']
                check(bool(text.strip()),ek+' blank display')
                check(text==unicodedata.normalize('NFC',text),ek+' display not NFC')
                check(all(ord(c)<256 and ord(c)>=32 and not (127<=ord(c)<=159) for c in text),ek+' unsupported display character')
                check(entry['source_id'] in source_ids,ek+' unknown source')
                check(set(entry['playable_packs']).issubset(pool['packs']),ek+' playable era outside pool')
                check(set(entry['weight_by_era'])==set(entry['playable_packs']),ek+' effective weights do not match eligibility')
                check(all(isinstance(w,int) and w>0 for w in entry['weight_by_era'].values()),ek+' nonpositive/noninteger weight')
                check(bool(entry['family_key']),ek+' absent family key')
                for basis in entry['editorial_basis']:
                    check(set(basis.get('model_source_ids',[])).issubset(source_ids),ek+' unknown model source')
                if ek in original:
                    check(all(entry.get(f)==v for f,v in original[ek].items()),ek+' original evidence was rewritten')
                else:
                    new_entries+=1
                    check(entry['date'] is None and entry['source_form'] is None,ek+' invented attestation date/form')
                    check(entry['packs']==[],ek+' invented evidence era')
                    check(entry['evidence_type'] in ['editorial-reconstruction','regional-borrowing'],ek+' unlabelled speculation')
            for era in pool['packs']:
                active=[e for e in entries if era in e['playable_packs']]
                families={e['family_key'].casefold() for e in active}
                displays={e['display'].casefold() for e in active}
                check(len(families)>=20,f'{key}/{era}/{sex} below 20 distinct families')
                check(len(displays)==len(active),f'{key}/{era}/{sex} duplicate active display')
                check(len({e['lemma'] for e in active})==len(active),f'{key}/{era}/{sex} duplicate active lemma')
                active_counts.append({'pool':key,'era':era,'gender':sex,'entries':len(active),'families':len(families)})
    check(set(original).issubset(all_entries),'Original entry lost')
    check(new_entries==196,'Expected 196 new authored profile entries')
    check(len(all_entries)==585,'Expected 585 total evidence/profile entries')
    check(len(active_counts)==58,'Expected 58 pool/era/gender cells')
    required={(r['pool'],r['pack'],r['gender']) for r in policy['requirements']}
    actual={(r['pool'],r['era'],r['gender']) for r in active_counts}
    check(required==actual,'Playable floor coverage matrix differs from requirements')
    for key in policy['birthname_exclusion']:check(not all_entries[key]['playable_packs'],key+' byname is active as BirthName')
    mappings=read('data/native_binding_patch.json')['exact_source_bindings']
    check(len(mappings)==14,'Expected14 native rows')
    seen=set()
    for row in mappings:
        check(row['source_identity']==f"source.{row['source_pack']}.ethnicity.{row['source_ethnicity']}",'Wrong source identity')
        check(row['proficiency']=='native' and len(row['languages'])==1,'Wrong fixed native grant')
        for era in row['packs']:
            identity=(row['source_pack'],row['source_ethnicity'],era)
            check(identity not in seen,'Overlapping exact source rule');seen.add(identity)
        if row.get('description_override'):
            check(all(ord(c)<256 for c in row['description_override']),'Non-Latin1 live ethnicity description')
            for word in ('design','toolkit','attest','gameplay','modern '):
                check(word not in row['description_override'].lower(),'Author note leaked into live ethnicity prose')
    check(sum(r['source_pack']=='earthantiquity' for r in mappings)==13,'Expected13 Antiquity source omissions')
    accents=read('data/accent_era_policy.json')
    def accent_eras(module:str,language:str,name:str,group:str):
        exact=[r for r in accents['exact_accent_overrides'] if (r['source_pack'].casefold(),r['source_language'].casefold(),r['accent_name'].casefold())==(module.casefold(),language.casefold(),name.casefold())]
        if exact:return set(exact[0]['allowed_packs'])
        langs=[r for r in accents['source_language_overrides'] if r['source_pack']==module and r['source_language']==language]
        allowed=set(langs[0]['allowed_packs'] if langs else accents['source_module_defaults'][module])
        for marker in accents['late_tradition_markers']:
            if marker['marker'].casefold() in name.casefold() or marker['marker'].casefold() in group.casefold():allowed &= set(marker['allowed_packs'])
        return allowed
    check('antiquity' in accent_eras('earthrenaissanceeurope','Latin','Classical','standard'),'Classical Latin reuse lost')
    check(accent_eras('earthrenaissanceeurope','Latin','Neo-Classical','standard')=={'renaissance','earlymodern'},'Neo-Classical mask wrong')
    check('antiquity' not in accent_eras('earthdarkagesandmedieval','Latin','Carolingian','Medieval Latin'),'Carolingian Antiquity leak')
    check('darkages' in accent_eras('earthrenaissanceeurope','Welsh','Northern','Welsh'),'Reusable Welsh tradition lost')
    check('darkages' not in accent_eras('earthrenaissanceworldexpansion','Persian','Safavid Court','court'),'Late marker did not restrict override')
    resources=read('data/runtime_resource_manifest.json')
    expected_count = resources['expected_required_count']
    check(expected_count == 30 and len(resources['required_resource_names']) == expected_count and
          len(set(resources['required_resource_names'])) == expected_count,
          'Resource manifest must declare 30 distinct names (28 round-two inputs plus two accent-role inputs)')
    check({'data.accent_era_policy.json', 'data.name_playability_policy.json',
           'data.stock_accent_roles.json', 'data.historical_foreign_accents.json'}.issubset(resources['required_resource_names']),
          'Round-two or subsequent accent-role policy resources absent')
    cases=read('data/regression_cases.json')['cases']
    check([c['id'] for c in cases]==[f'R2-{i:02d}' for i in range(1,27)],'Regression case IDs incomplete')
    # Integrity is a separate transport check, not inflated into the authoring-assertion count.
    hashed=0
    manifest=root/'MANIFEST.sha256'
    if manifest.exists():
        for line in manifest.read_text().splitlines():
            digest,rel=line.split('  ',1)
            path=root/rel
            if not path.is_file() or hashlib.sha256(path.read_bytes()).hexdigest()!=digest:failures.append('Checksum mismatch: '+rel)
            hashed+=1
    return {'passed':not failures,'scope':'Authoring data and optional transport checksums only; no C# or database execution.',
      'data_checks':checks,'checksum_files_checked':hashed,'original_entries_preserved':len(original),'new_authored_profile_entries':new_entries,
      'total_entries':len(all_entries),'playable_cells':len(active_counts),'native_binding_rows':len(mappings),'active_counts':active_counts,'failures':failures}

def main()->int:
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--root',type=Path,default=Path(__file__).resolve().parents[1])
    parser.add_argument('--write-report',type=Path)
    args=parser.parse_args()
    try: result=validate(args.root.resolve())
    except (OSError,ValueError,KeyError,TypeError) as exc:
        print(f'Validation could not complete: {exc}',file=sys.stderr);return 2
    text=json.dumps(result,ensure_ascii=False,indent=2)+'\n'
    if args.write_report:args.write_report.write_text(text,encoding='utf-8')
    print(json.dumps({k:v for k,v in result.items() if k!='active_counts'},ensure_ascii=False,indent=2))
    return 0 if result['passed'] else 1
if __name__=='__main__':raise SystemExit(main())
