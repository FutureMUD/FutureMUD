# 2. Coding-agent implementation brief

## 2.1 Remaining scope and dependency

Implement the full CultureSeeder content redesign using PR #730 (`961efca81da0d788bbfb86ccf55fea313c9bf065`) or a descendant. The generic group subsystem is already implemented. Do not recreate its entities, migration, builder commands, allocator, screen wrappers, acquisition-claim model, base/boost accounting or NPC adapter. [R730A, R730B]

Remaining work: five-era composition; source-qualified shared identities; non-destructive original corpus retention; contemporary prose; canonical language labels and directed intelligibility; fixed ethnicity/culture skill grants; editable starting-value progs; 28 generic-group consumers; literacy/script integration; targeted name repertoire integration and encoding fixes.

Explicit exclusions: new home-language, cultural-language or adopted-name selectors; ancestry simulation; new proficiency-level entities/UI; seeder balance questionnaires; a second skill-group implementation; live era switching; a full social/legal simulator. No repository change is made by this handoff itself.

The source corpus remains in the repository. The agent must export it before refactoring. Targeted name material is supplied in `data/targeted_name_corpora.json`; its stated evidence scope and inactive Old Prussian feminine replacement must be respected. Other original profiles remain preserved rather than replaced without evidence.

## 2.2 Proposed composition model

Use a manifest with four separate concepts:

| Concept | Example | Requirement |
|---|---|---|
| Canonical entity key | `english.old` | Stable across presentation changes; never use a lossy transliteration as a key. |
| Legacy binding | source module + `Old English` | Binds the right existing row. Ambiguous labels require source/context inspection. |
| Pack presentation | `Saxon`, `English`, `Old English` | Controls the installed label, not the historical identity. |
| Content references | accent keys, naming profile, scripts | Resolve after all selected-pack entities are bound. |

Names such as `Roman`, `Greek`, `English` and `Venetian` are especially unsafe global identity keys. Ancient Veneti and Renaissance Venetians must not be merged just because the old label matches. Existing modern and Middle-Earth packages remain separate.

The ItemSeeder managed-record approach is a candidate infrastructure, not a requirement to copy its full multi-era framework. A dedicated stable-key registry or equivalently explicit binding manifest is sufficient. All aliases must be type-qualified: an ethnicity alias is not a language alias.

### Composition order

1. Read existing data and resolve prerequisites; create no content yet.
2. Load the chosen pack manifest and required shared definitions.
3. Bind legacy and canonical entities. Report ambiguous bindings before writing.
4. Create genuinely missing approved entities, preserving existing logical IDs.
5. Apply presentation labels and approved corrections.
6. Reconcile accent, script, intelligibility, name-profile and gender/name links non-destructively.
7. Install fixed grant/prog data and consumer skill-group definitions and marked stock FutureProg integration.
8. Validate all references, generated examples, encodings and choice reachability; commit only after successful validation.

Only one selected pack is involved. Older language stages included by that pack are not separate culture-pack installations.

## 2.3 Preserve the existing corpus

Before changing structure, export a lossless manifest of the original seeder-generated data, including every profile element's text, usage, weight and gender association; every name-pattern minimum/maximum and regex; and every accent's name, suffixes, description, group and difficulty.

The same corpus after the refactor must be recoverable by stable key. A correction may move an element out of a default profile, but must retain its original value and a reason in the source/legacy catalogue. The change ledger must distinguish `retain`, `alias`, `correct`, `move-default`, `new`, `legacy-only` and `review-gated`.

`CultureSeeder.Shared.cs` presently replaces random-profile dice and element collections on upsert. Do not reuse this path unchanged for preserved or builder-modified records. [R03]

For reruns, compare the last seeded value, the current value and the proposed new seeded value where a baseline exists. Apply an update to unchanged stock fields; preserve a builder edit and report conflict. For installations without a baseline, default to preserving ambiguous existing content rather than assuming everything belongs to the seeder. A source snapshot or backup is not a substitute for this write policy.

Apply `data/player_prose_policy.json` to all active descriptions, including retained accents and name-element blurbs. Preserve original wording in the source ledger while retaining its valid setting/phonetic details in contemporary prose. Never concatenate metadata into Description.

Do not remove additional scripts or languages already installed by a builder. A new pack-specific availability filter can hide a stock default without deleting its record.

## 2.4 Language labels affect more than the language table

Both language and linked trait names currently participate in name-based upserts; skill-cap expression names also incorporate the label. `AddScript` builds acquisition progs using string comparisons against `@skill.Name`. Updating only `Language.Name` will leave stale references. [R03, R11]

Resolve canonical key to Language ID and linked Trait ID. Then update all stock presentation-dependent references in one reconciliation step: language label, trait display name, generated cap-expression label where appropriate, script acquisition rules, grant maps and generated help text. Generated logic should preferably use resolved traits/IDs rather than display text. Builder-authored scripts must not be globally search-and-replaced.

A compatibility alias is lookup metadata, not a duplicate language or trait. If two canonical stages genuinely exist, they remain two separate competencies even when both have historically been called English. No pack may install two language labels that collide case-insensitively.

Retain older stages on their own keys. An English character in Early Modern does not receive Old English merely because that record was retained for scholars and manuscripts. Intelligibility is specified by the exact directed settings in `data/mutual_intelligibility.json`, not guessed percentages; reconcile original settings under section 2.4.1.

### 2.4.1 Directed mutual-intelligibility settings

The supplied catalogue has **177 pairs / 354 directed edges**, with an additional **16 conditional retained-language pairs**. The core range is `Difficulty.VeryHard` (7), `Difficulty.ExtremelyHard` (8) and `Difficulty.Insane` (9). These are the user's gameplay scale, not linguistic measurement. `Difficulty.Impossible` (10) means no allowance; do not collapse it into Insane. Accent difficulties remain independent and may be easier than Very Hard. [U01, R21]

For every row bind `listener_language` to `ListenerLanguageId` and `target_language` to `TargetLanguageId`. The first is the language the listener knows. The current historical helper's `from/to` naming is misleading for this convention: `EnsureMutualIntelligability` performs `Upsert(target, source)` for its first direction. Prefer an explicitly named listener/target wrapper or direct correctly keyed upsert, and test a one-way fixture even though the authored core defaults are symmetric. [R03, R22]

The listening path uses `max(accentDifficulty, utteranceDifficulty, mutualDifficulty)` with a known language's linked trait. Do not add the difficulty values, turn them into percentages, use the target language's unowned trait, or find a shortest path through intermediary languages. Existing writing paths also consult language links; keep their literacy and actual-script checks. [R23, R24]

Install edges only after language-stage binding and pack composition. No self-edges, alias edges, family-resolver edges or automatic shared-script links. A missing pair has no new row. Conditional legacy pairs require a genuine retained language and correct provenance; Ukrainian must not be manufactured by relabelling Ruthenian.

Preserve source values before modifying stock data. An explicit matrix setting overrides an unchanged stock row. For an unlisted genuine positive stock row retain the relationship with `max(VeryHard, previousDifficulty)` for previous values through Insane; preserve disabled/Impossible settings. Apply three-way conflict protection to builder edits. The resolved report must enumerate retained exceptions and their final directed scores, so the final installation contains no undocumented inherited setting. Do not globally retune unrelated Modern, Middle-Earth or signed-language packs.


## 2.5 Fixed grants: no new chooser

### Inputs and outputs

Inputs are selected pack, ethnicity, culture and the requested installed trait, plus the existing independent/ordinary/group ownership model where the engine already uses it. There is no saved home-language answer, naming-convention answer or custom educational-stage state.

Resolve native languages from `data/ethnicity_language_defaults.json`. Exact local/source-qualified variants override broad templates; `data/legacy_native_language_rules.json` supplies retained-template and ancient bindings. An entry beginning `legacy:` means an existing genuine source-qualified record, never an instruction to fabricate a new record by label. Inventory all retained in-scope identities and report unresolved bindings before activation. Do not default every unresolved identity to English, Latin or its political ruler's language.

For each selected ethnicity, grant its fixed native language(s) through the mandatory free-skill path. A specific culture can add its fixed vernacular at Fluent. `home` means the already-determined ethnic language. An era/regional selector is now a compile-time content resolver, not a player group. Prefer the ethnic language when it is a suitable member of the defined regional set; otherwise use the documented fixed regional default. Old alternative lists are builder reference only.

Add the explicitly listed mandatory educational languages and learned Literacy. Resolve to actual trait IDs, deduplicate, and union with other mandatory/independent skills. A culture grants no wealth, rank or active legal status.

### Existing prog integration

Generate a stock-owned helper returning the fixed native/cultural free skill collection. Reconcile its call into the actual configured `FreeSkillsProg` path, preserving unrelated existing grants and builder content. It must work when CultureSeeder and ChargenSeeder run in either order. Keep changes within owned marker/baseline boundaries; warn rather than overwrite an unrecognised custom body.

The generic resolver evaluates eligibility against a detached mandatory baseline. No group can qualify itself from provisional choices. D1 removes the former need to pass a home-language decision between stages; do not weaken the protection or reintroduce ordered dependencies. [R730B]

## 2.6 Proficiency using existing starting-value progs

Use the existing `Culture.SkillStartingValueProg` convention, normally Number(Toon ch, Trait trait, Number boosts). Verify the actual installed caller and signature rather than assuming the generic group helper's strict Boolean signatures also apply here.

| Catalogue tier | Factor | Default base |
|---|---:|---:|
| Native | 1.00 | 200 |
| Fluent | 0.90 | 180 |
| Educated | 0.75 | 150 |
| Conversational | 0.50 | 100 |
| Elementary | 0.25 | 50 |

These are skill values, not comprehension percentages. Seed editable FutureProg bodies as initial content. The seeder must not ask builders to select numerical levels or serve as the later tuning interface.

`data/language_starting_value_prog_contract.json` defines the small integration contract. A `CultureLanguageNativeBase` helper returns 200. A background-factor helper examines ethnicity, culture and trait references and returns the greatest qualifying factor. Use resolved IDs rather than labels. The helper is editable in-game.

For a language with a qualifying background, the wrapper returns:

```text
backgroundBase = editableNativeBase * maximumQualifyingBackgroundFactor
boostDelta = preservedOriginalProg(ch, trait, boosts)
           - preservedOriginalProg(ch, trait, 0)
result = backgroundBase + boostDelta
```

This is algorithmic pseudocode, not a promised compiled FutureProg listing. Preserve the actual original prog by identity, guard against wrapping a wrapper recursively, and verify where the engine currently applies boosts and caps. Do not apply a second boost outside that existing path. Non-language skills and languages without a qualifying background delegate unchanged to the original prog.

The simple helper uses background eligibility, not a new group-provenance function. Therefore a language for which this background offers education gets the same base when bought through open skills rather than obtained through the free elective. This is intentional simplification under D2. Eligibility does not itself grant the skill: the language must actually be selected or awarded.

Retain stronger independently supplied values and PR #730's existing value ownership. Test a native grant, cultural overlap, optional elective, ordinary purchase and a stronger hand-authored NPC value. Do not derive tier qualification from current `SkillValues` or group picks that would create a circular entitlement.

The 200 base must survive stock cap handling. New or verified unchanged seed-owned language-cap definitions must accommodate 200; a floor of 200 is permitted while retaining the existing scaling above it. Establish ownership and unchanged stock status by baseline/source comparison before editing an existing cap. Do not silently overwrite a builder-modified cap: report a lower effective cap and show the capped result. A live test must establish that stock native languages actually start at 200, not merely that a helper returned it before later clamping.

No new proficiency subsystem is authorised. A small compilation/wiring helper is allowed. If the installed caller cannot support the wrapper, identify the exact obstruction; do not compensate by inventing database tier entities or a new UI.

## 2.7 Literacy, script knowledge and accents

Thirteen supplied backgrounds explicitly grant Literacy. Others do not. Use `data/literacy_script_grants.json` for learned traditions, primary-script defaults and community overrides. The installed Literacy trait and existing skill value/boost rules are reused; no new literacy-scale model.

A script award requires Literacy, an actual selected language and a specifically authorised writing tradition. A spoken skill with multiple possible scripts does not award all of them. Electing a learned language may make its specified educational script available; it does not create general literacy if the background did not grant it.

The existing stock script block in `ChargenFreeKnowledges` may grant every script compatible with any selected language. Do not add this narrower block alongside it and accidentally retain the broad union. Reconcile only the culture-pack-owned stock block, using existing preservation rules, while leaving custom text and unrelated packs intact. A shared script may retain its full designed-language membership even though free acquisition is restricted.

Script knowledge is a knowledge, not an ordinary skill-group member. Use broad writing systems; retain genuine legacy systems such as Etruscan or Glagolitic even though they are not subdivisions of Latin. No forced new shorthand catalogue.

Use an appropriate native accent default/eligible native set for ethnic grants and preserve the existing learner accent path for acquired languages. Keep all authored native/regional/foreign accent descriptions and difficulties; a VeryHard minimum for language-to-language intelligibility is not a minimum accent difficulty. Do not turn extinct retained languages into native options merely because they exist in the catalogue.

## 2.8 Naming and encoding

Keep the current ethnicity-first `NameCultureForGender` resolution. Do not implement a player-facing adopted-name or random-profile chooser. Reuse proven stock naming structures and sex-specific forms; new pools use a dedicated local name-culture identity when necessary to prevent the NamePicker from mixing every profile attached to generic Simple/Given and Family.

The targeted repertoire file supplies explicit male/female entries, source form, Latin-1 display form, lemma key, evidence class, date scope and provisional weight. Link each eligible local profile directly by ethnicity/gender/pack. Select one lemma before its spelling variant. Do not boost frequency by treating aliases as independent names. Do not treat a saint's life date as the date of a recorded vernacular name spelling.

Only the stated temporal scope is authorised. A sixteenth-century Christian corpus must not silently replace an early pagan naming tradition. When an early-period replacement is not supported, preserve the existing source-specific profile and label it as retained, not newly verified. The Old Prussian feminine replacement is inactive and may not be filled from neighbouring peoples.

New optional bynames default to zero random occurrences unless a supplied valid local byname recipe is explicitly selected by the profile. An empty byname must not leave extra spaces. Preserve existing useful byname inventories; the small new atom lists are not a reason to delete them. No new productive gender-suffix rule, marriage-name assumption, regnal numbering or saint prefix is authorised.

Run `data/naming_pattern_tests.json` against actual `NameCulture`/`PersonalName` code for all existing NameStyles and regex round-trips. Multiword values may be one name element. Do not mechanically replace spaces with hyphens or apply English AP-title-case to already reviewed name tokens. Constructed test names are not claims of attested full persons.

Apply `data/latin1_fallback_specification.json`: NFC at the whole-string boundary; preserve already supported Latin-1 characters; reviewed mappings for unsupported non-decomposing letters; safe independent fallback buffers; no silent question marks. Store original Unicode in research, not a lossy identity key. Test .NET encoder behaviour and NamePicker's existing Unicode-off validation; better output fallback does not automatically change input policy.

## 2.9 Optional installation and reruns

Keep the existing yes/no dimensions for names, languages and heritage. Do not add proficiency or group-count questions. Handle all supported combinations: missing optional language packages must not generate dangling free-skill/group/script references; names omitted means existing sane structure fallback, not a false claim of a newly researched name pool.

Group definitions are self-contained consumers and can be seeded before or after storyboards. Install only selected-era eligible candidates. Candidate aliases resolve to distinct actual traits. Report omitted optional unresolved candidates; mandatory missing references fail preflight. A group with no eligible candidates and minimum zero is skippable, not an entitlement to an unrelated skill.

Preserve approved characters. Rerunning does not strip independently known languages, rename player identities or retroactively replace personal names. Freshly generated suggestions and pending applications follow current definitions and the dependency's review/revalidation rules.

## 2.10 Consume the implemented generic skill groups

The implementation boundary is:

```csharp
DatabaseSeeder.Seeders.Utilities.Chargen
    .ChargenSkillSelectionGroupSeeder.Upsert(context, definition, conflicts);
```

`SkillGroupSeedDefinition` fields are StableKey, Name, Description, MinimumPicks, MaximumPicks, DisplayOrder, ExistingSkillPolicy, Enabled, Members, EligibilityProg, MemberEligibilityProg. Members are resolved installed trait models. Eligibility references are compiled runtime `IFutureProg` objects, not merely database rows. [R730A]

Group eligibility must be exactly Boolean(chargen), with `AcceptsAnyParameters=false`; an optional member filter is exactly Boolean(chargen, trait). A broadly compatible Toon prog is not automatically interchangeable at this helper boundary. Reuse the existing loader/test construction path or add a small seeder compilation adapter. Verify all referenced progs are installed and compile.

Seed the 28 definitions in `data/skill_groups.json` using the candidate recipes in `data/language_choice_groups.json`. They are optional 0..1, CountKnown, unique-across-groups. Their free award consumes no ordinary pick or base skill-point charge. Existing resource prerequisites and paid boosts remain unchanged. Use fixed culture/ethnicity eligibility; never require another pending group selection.

Existing in-game `chargenskillgroup` commands already edit descriptions, members, bounds, eligibility, policy, order, enabled state and retirement. Document the relevant generated progs for builders; do not add a parallel command tree. Three-way Upsert preserves builder overrides and reports conflicts; a matching name does not grant ownership.

Generated templates stay opted out unless explicitly configured through the merged NPC adapter. Do not create a second randomiser or choose new values every time a template is displayed. Stronger hand-authored values remain authoritative.

SG01–SG20 are dependency regression tests, not a task to implement generic groups a second time. Live MySQL migration/import and telnet/editor execution were not established by the supplied completion report and remain verification obligations, not unimplemented historical content. [R730B]

## 2.11 Acceptance and completion reports

| ID | Required proof |
|---|---|
| CF01 | All five era options create coherent self-contained manifests with source-qualified identities. |
| CF02 | A lossless pre/post inventory shows original name, weight, usage, gender, regex, style and accent recoverability. |
| CF03 | Two-pass seeding does not duplicate identities; builder-edited names, group counts and prog bodies survive with conflicts reported. |
| CF04 | Every retained active ethnicity has a fixed native binding; no home/upbringing chooser exists. |
| CF05 | Welsh + Medieval English Nobility produces Welsh 200 and the specified acquired English/French/Latin bases without duplicate awards. |
| CF06 | Native=200 is verified after the actual stock cap/value pipeline; custom lower caps are preserved and reported. |
| CF07 | Native/cultural/elective overlap takes maximum base; paid boost delta applies once; non-languages retain original behaviour. |
| CF08 | All 28 group recipes resolve through PR730, all four screens and both seeder orders; no replacement group subsystem. |
| CF09 | Optional groups remain optional, CountKnown credits are explicit, and no slot can satisfy two groups. |
| CF10 | Learned backgrounds receive Literacy and selected traditions; a language with several scripts does not grant all of them. |
| CF11 | Label changes update linked traits and owned script/grant references without rewriting arbitrary builder scripts. |
| CF12 | Every directed intelligibility edge is reported with listener/target/difficulty; one-way fixture proves direction; no transitive/shared-script edges. |
| CF13 | All active player prose is contemporary; research caveats stay outside descriptions and suffixes. |
| CF14 | Eight targeted repertoires are consumed within stated scope. No unrelated filler, fake attestation or active Old Prussian feminine replacement. |
| CF15 | Existing ethnicity-first naming remains; no adopted-name chooser. Parser/renderer tests include compounds, empty bynames and all NameStyles. |
| CF16 | Actual Latin-1/NFC/fallback concurrency tests pass; no question marks or lossy identity merges. |
| CF17 | Optional names/languages/heritage permutations do not generate dangling references or falsely certify skipped content. |
| CF18 | Existing NPC templates remain opted out; opt-in deterministic generation retains stronger independent values. |
| CF19 | Normal and focused C# tests, generated prog compilation and migration/snapshot checks are reported separately from handoff validation. |
| CF20 | Completion report lists commit, tests actually run, unverified MySQL/telnet checks, retained source counts, active group/trait IDs and all evidence exclusions. |

Return a changed-file summary; a manifest/reference-resolution report; the original-corpus preservation diff; resolved directed intelligibility and language-grant tables; generated prog identities; naming profile counts by gender and era; test commands/results; and explicit remaining gaps. Do not claim a structurally valid JSON repertoire is historically exhaustive or independently attested in every era.
