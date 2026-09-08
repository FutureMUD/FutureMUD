#!/usr/bin/env python3
"""Check this handoff's data. Not a C# test or an attestation validator.

Usage: python tools/validate_catalogue.py [path/to/handoff]
Only the Python standard library is required.
"""
from __future__ import annotations
import json, re, sys, unicodedata
from collections import Counter
from pathlib import Path
from typing import Any


def main(root: Path) -> int:
    checks: list[dict[str, Any]]=[]
    errors: list[str]=[]
    warnings: list[str]=[]
    def check(label: str, passed: bool, detail: str='') -> None:
        checks.append(dict(check=label,passed=bool(passed),detail=detail))
        if not passed: errors.append(label+(': '+detail if detail else ''))
    def load(file: str) -> Any:
        return json.loads((root/file).read_text(encoding='utf-8'))
    for path in sorted(root.rglob('*.json')):
        if path.name=='validation_report.json':continue
        try:json.loads(path.read_text(encoding='utf-8'));check('JSON '+str(path.relative_to(root)),True)
        except (ValueError,OSError) as exc:check('JSON '+str(path.relative_to(root)),False,str(exc))
    if errors:return 1
    eras={x['key'] for x in load('data/eras.json')}
    cultures=load('data/social_cultures.json'); eth=load('data/ethnicity_reframing_overlay.json')
    langs=load('data/language_identity_and_labels.json');scripts=load('data/script_policy.json')
    groups=load('data/skill_groups.json');choices=load('data/language_choice_groups.json')
    plans=load('data/naming_reuse_ledger.json');sources=load('research/sources.json')
    gates=load('research/review_gates.json');defaults=load('data/ethnicity_language_defaults.json')
    spec=load('data/skill_group_selection_specification.json');policy=load('data/language_grant_policy.json')
    corpus=load('data/targeted_name_corpora.json');selectors=load('data/vernacular_selectors.json')
    lk={x['key']:x for x in langs if x['entity_kind']=='language-specification'}
    sk={x['key'] for x in scripts};ek={x['key'] for x in eth};ck={x['key'] for x in cultures}
    src={x['id'] for x in sources};tiers=policy['proficiency_scale']
    for label,rows,key in [('cultures',cultures,'key'),('ethnicities',eth,'key'),('languages',langs,'key'),('scripts',scripts,'key'),('groups',groups,'key'),('names',plans,'key'),('sources',sources,'id'),('pools',corpus['pools'],'key')]:
        values=[r[key].casefold() for r in rows];check('Unique '+label,len(values)==len(set(values)))
    for row in cultures+eth+langs+scripts+plans+gates:
        check('Source references '+row.get('key',row.get('id','?')),set(row.get('sources',[]))<=src)
        check('Era references '+row.get('key',row.get('id','?')),set(row.get('packs',[]))<=eras)
    for era in sorted(eras):
        names=[r['labels'][era].casefold() for r in langs if era in r.get('labels',{})]
        check('Unique era labels '+era,len(names)==len(set(names)))
    for row in langs:
        check('Language scripts '+row['key'],set(row['scripts'])<=sk)
    for row in scripts:check('Script languages '+row['key'],set(row['languages'])<=set(lk))
    check('Family resolver is not a skill',next(x for x in langs if x['key']=='south-slavic')['entity_kind']=='family-resolver')
    def ref_ok(key: str) -> bool:return key in lk or key.startswith('legacy:')
    def selected(selector: str,era: str,native: str|None=None) -> list[str]:
        if selector=='home':return [native] if native else []
        if selector.startswith('language:'):return [selector[9:]]
        row=selectors[selector]
        if row['mode']=='exact-community-legacy-binding':return [native] if native and native.startswith('legacy:') else ['legacy:Serbo-Croatian']
        primary=row.get('by_era',{}).get(era)
        alternatives=row.get('builder_reference_alternatives_by_era',{}).get(era,[])
        if native and native in [primary,*alternatives]:return [native]
        return [primary] if primary else []
    for key,row in selectors.items():
        check('No chooser '+key,row.get('player_choice') is False)
        check('No stale alternatives '+key,'alternatives_by_era' not in row)
        for era,language in row.get('by_era',{}).items():check('Selector '+key+'/'+era,era in eras and language in lk and era in lk[language]['labels'])
    check('One default row per overlay',len(defaults['defaults'])==len(eth) and {x['ethnicity']for x in defaults['defaults']}==ek)
    for row in defaults['defaults']:
        check('Fixed native '+row['ethnicity'],row['player_choice'] is False and row['proficiency']=='native' and bool(row['by_era']))
        for era,keys in row['by_era'].items():check('Native references '+row['ethnicity']+'/'+era,era in eras and bool(keys) and len(keys)==len(set(keys)) and all(ref_ok(k)and(k not in lk or era in lk[k]['labels'])for k in keys))
    for row in cultures:
        s=row['vernacular_selector'];check('Culture selector '+row['key'],s in selectors or s.startswith('language:')and s[9:]in lk)
        for era in row['packs']:
            if s!='home':check('Cultural binding '+row['key']+'/'+era,bool(selected(s,era)))
        check('Culture electives '+row['key'],set(row.get('optional_language_choices',[]))<=set(choices))
        check('No name selector '+row['key'],'No adopted-name' in row['name_profile_policy'])
        for grant in row['additional_language_grants']:check('Grant '+row['key']+'/'+grant['language'],grant['language']in lk and grant['proficiency']in tiers and grant['condition']in ['all-listed-packs',*eras])
    check('Merged group dependency',spec['dependency']['pull_request']==730 and spec['status']=='implemented-dependency-consumer-only')
    check('No derived chooser recipes',spec['derived_group_recipes']==[])
    check('No new chooser contract',not any(spec['home_and_cultural_languages'][x]for x in ['home_chooser','upbringing_override','working_language_chooser']))
    for row in groups:
        key=row['membership_recipe']['source_key'];q=choices.get(key,{})
        check('Group consumer '+row['key'],key in choices and row['minimum_picks']==0 and row['maximum_picks']==1 and row['existing_skill_policy']=='CountKnown')
        check('Group eligibility '+row['key'],set(row['eligibility']['culture_packs'])<=ck and set(row['eligibility']['ethnicity_packs'])<=ek)
        check('Group free base '+row['key'],row['award']['skill_pick_cost']==0 and row['award']['base_skill_point_cost']==0)
        check('Group language recipes '+row['key'],all(ref_ok(k)for k in q.get('languages',[]))and all(s in selectors for s in q.get('selectors',[])))
        check('Optional recipe bounds '+row['key'],q.get('minimum_picks')==0 and q.get('maximum_picks')==1)
    check('No seeder tuning questions',load('data/language_starting_value_prog_contract.json')['new_seeder_questions'] is False)
    check('No new proficiency entities',load('data/language_starting_value_prog_contract.json')['new_proficiency_database_entities'] is False)
    expected={'native':200,'fluent':180,'educated':150,'conversational':100,'elementary':50}
    check('Native and scaled tiers',policy['native_base']==200 and policy['proficiency_defaults']==expected and all(abs(200*tiers[k]-v)<1e-9 for k,v in expected.items()))
    # Data-level grant fixtures, deliberately not engine execution.
    def values(ethnicity: str,culture_label: str,era: str,prior:dict|None=None)->dict:
        native=next(x for x in defaults['defaults']if x['ethnicity']==ethnicity)['by_era'][era]
        culture=next(x for x in cultures if x['label']==culture_label);out=dict(prior or {})
        def grant(key: str,tier: str)->None:out[key]=max(out.get(key,0),expected[tier])
        for key in native:grant(key,'native')
        for key in selected(culture['vernacular_selector'],era,native[0]):grant(key,culture['cultural_vernacular_proficiency'])
        for g in culture['additional_language_grants']:
            if g['condition']in ['all-listed-packs',era]:grant(g['language'],g['proficiency'])
        return out
    check('Welsh noble values',values('ethnicity.welsh','English Nobility','medieval')=={'welsh':200,'english.middle':180,'french.anglonorman':150,'latin':50})
    check('Native and cultural overlap',values('ethnicity.english','English Nobility','medieval')=={'english.middle':200,'french.anglonorman':150,'latin':50})
    check('Independent stronger value',values('ethnicity.welsh','English Nobility','medieval',{'latin':250})['latin']==250)
    check('No automatic dead English',set(values('ethnicity.english','English Nobility','earlymodern'))=={'english.earlymodern','latin'})
    # Directed mutual intelligibility and mirrored per-language tables.
    mi=load('data/mutual_intelligibility.json');edges=mi['directed_edges'];em={(x['listener_language'],x['target_language']):x for x in edges}
    check('MI preserved counts',len(edges)==354 and len(mi['pairs'])==177 and len(em)==len(edges))
    for row in edges:
        a,b=row['listener_language'],row['target_language'];valid={'VeryHard':7,'ExtremelyHard':8,'Insane':9}
        check('MI '+row['key'],a in lk and b in lk and a!=b and valid.get(row['difficulty'])==row['difficulty_value'] and all(e in lk[a]['labels']and e in lk[b]['labels']for e in row['packs']))
    for row in mi['pairs']:
        a,b=row['first_language'],row['second_language'];check('MI paired directions '+row['key'],em[a,b]['difficulty']==row['first_understands_second']and em[b,a]['difficulty']==row['second_understands_first'])
    for key,row in lk.items():
        mirrored={(x['target_language'],x['difficulty'])for x in row['mutual_intelligibility']['links']}
        actual={(b,r['difficulty'])for(a,b),r in em.items()if a==key}
        check('MI mirror '+key,mirrored==actual)
    # Player text scope and Latin-1.
    fallback=load('data/latin1_fallback_specification.json');mapping=fallback['explicit_mappings']
    def latin1(text: str)->str:
        out=''
        for char in unicodedata.normalize('NFC',text):
            if ord(char)<256:out+=char
            elif char in mapping:out+=mapping[char]
            else:
                candidate=unicodedata.normalize('NFC',''.join(c for c in unicodedata.normalize('NFD',char)if unicodedata.category(c)!='Mn'))
                if not candidate or any(ord(c)>255 for c in candidate):raise ValueError('Unsupported '+repr(char))
                out+=candidate
        return out
    def prose(key: str,text: str)->None:
        check('Player text '+key,bool(text)and text==unicodedata.normalize('NFC',text)and all(ord(c)<=255 for c in text))
        check('No admin in prose '+key,not re.search(r'\b(seeder|gameplay|this design|implementation|modern-day|modern Tuscany|coding agent)\b',text,re.I))
    for row in cultures:prose(row['key'],row['description'])
    for row in eth:prose(row['key'],row['replacement_description'])
    for key,row in lk.items():
        prose(key,row['description']);prose(key+'/accent',row['fallback_accent_if_no_legacy_accent']['description'])
    for row in scripts:prose(row['key']+'/script',row['description'])
    for row in groups:prose(row['key'],row['description'])
    for test in fallback['tests']:check('Encoding fixture '+test['input'],latin1(test['input'])==test['expected'])
    # Targeted name integrity. This cannot prove the cited historical facts.
    all_entries=[]
    for pool in corpus['pools']:
        prose(pool['key'],pool['description'])
        counts={}
        for gender in ['male','female']:
            entries=pool['given_'+gender];counts[gender]=len({x['lemma'].casefold()for x in entries})
            check('Pool unique entries '+pool['key']+'/'+gender,len({x['key']for x in entries})==len(entries))
            check('Pool recipe '+pool['key']+'/'+gender,pool['production_recipe']['profiles'][gender]['given_pool']==[x['key']for x in entries])
            for entry in entries:
                all_entries.append(entry)
                check('Name '+entry['key'],entry['source_id']in src and bool(entry['source_form'])and bool(entry['locator'])and entry['display']==unicodedata.normalize('NFC',entry['display'])and all(ord(c)<=255 for c in entry['display'])and '?'not in entry['display']and set(entry['packs'])<=set(pool['packs'])and entry['weight']>0)
            if counts[gender]<20:warnings.append(f"{pool['label']}: {counts[gender]} {gender} groups; bounded evidence, no quota padding.")
        check('Pool stated counts '+pool['key'],counts==pool['lemma_counts'])
    check('Name global keys unique',len({x['key']for x in all_entries})==len(all_entries))
    pr=next(x for x in corpus['pools']if x['key']=='names.target.old-prussian')
    check('Old Prussian female gap not hidden',not pr['given_female']and not pr['production_recipe']['profiles']['female']['enabled']and 'inactive' in (root/'README.md').read_text())
    fixtures=load('data/naming_pattern_tests.json')
    check('No adopted name selector',fixtures['no_adopted_name_screen'] is True and all(x['player_adopted_name_choice']is False for x in plans))
    for f in fixtures['fixtures']:check('Name rendering fixture '+f['birth'],' '.join(x for x in [f['birth'],f['byname']]if x)==f['expected_full'])
    literacy=load('data/literacy_script_grants.json')
    check('Learned literacy count',len(literacy['automatic_backgrounds'])==13 and sum(c['automatic_literacy']for c in cultures)==13)
    for row in literacy['automatic_backgrounds']:check('Learned script references '+row['culture'],row['culture']in ck and set(row['fixed_script_keys'])<=sk)
    stats=load('data/catalogue_statistics.json')
    check('Actual statistics',stats['social_culture_count']==len(cultures)and stats['language_specification_count']==len(lk)and stats['targeted_given_name_entry_count']==len(all_entries)and stats['source_count']==len(sources)and stats['generic_derived_group_recipe_count']==0)
    docs=['README.md','AGENT_TASK.md','01_DESIGN_DECISIONS.md','02_IMPLEMENTATION_BRIEF.md','03_SOCIAL_CULTURE_CATALOGUE.md','04_LANGUAGE_AND_SCRIPT_CATALOGUE.md','05_NAMING_AND_ETHNICITY_PLAN.md','06_RESEARCH_AND_REVIEW_REGISTER.md']
    for file in docs:
        text=(root/file).read_text(encoding='utf-8');check('Document '+file,len(text)>300)
        for target in re.findall(r'`((?:data|research|tools)/[^`]+\.(?:json|py))`',text):check('Document file ref '+target,(root/target).is_file())
    text=(root/'02_IMPLEMENTATION_BRIEF.md').read_text()
    check('Obsolete builder implementation removed','### 2.10.1 Scope and schema'not in text and 'home-language selection, persisted'not in text)
    warnings+=['No new feminine Old Prussian pool is established. Its replacement remains inactive.','Dates and sources are research metadata; this validator does not establish historical attestation or demographics.','C# tests, generated FutureProg compilation, live MySQL and telnet/editor execution were not run by this validator.','Source-qualified legacy references must be resolved against the actual repository/database during implementation.']
    report={'revision':'2026-09-07-final-decisions','status':'passed-with-disclosed-evidence-limitations'if not errors else 'failed','check_count':len(checks),'passed':sum(x['passed']for x in checks),'failed':len(errors),'errors':errors,'warnings':warnings,'counts':stats,'scope':'Handoff data/reference/policy checks, not engine or historical-attestation verification.','checks':checks}
    (root/'validation_report.json').write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps({k:report[k]for k in ['status','check_count','passed','failed','errors','warnings']},ensure_ascii=False,indent=2))
    return 1 if errors else 0
if __name__=='__main__':
    try:raise SystemExit(main(Path(sys.argv[1]).resolve()if len(sys.argv)>1 else Path(__file__).resolve().parents[1]))
    except (OSError,ValueError,KeyError,TypeError)as exc:
        print('Validation could not complete: '+str(exc),file=sys.stderr);raise SystemExit(2)
