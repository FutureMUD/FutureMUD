# 1. Final design decisions

## 1.1 Authoritative decisions

This revision incorporates the user's final D1–D6 answers. It supersedes earlier recommendations wherever they differ. Source-derived observations, authored game defaults and remaining evidence limitations are kept separate in this handoff.

| Decision | Final instruction |
|---|---|
| D1 | Ethnicity determines native language through fixed era-specific bindings. No upbringing override, home-language chooser, working-language chooser or optional bilingual entitlement stage. |
| D2 | Use existing starting-value FutureProgs with minimal engineering. Native base 200; retained factors yield Fluent 180, Educated 150, Conversational 100 and Elementary 50. Builders edit progs in game. No proficiency subsystem, new tier database entities or seeder tuning questionnaire. |
| D3 | Author broad educational and neighbouring/contact-language options. Use the 28 supplied optional group recipes and explicit mandatory background grants. No universal free second language and no arbitrary global cap that discards listed entitlements. |
| D4 | Explicit learned/scribal/clerical backgrounds may grant Literacy. Spoken languages do not grant literacy. Script knowledge follows specified writing traditions rather than every associated script. |
| D5 | Existing ethnicity-first naming remains. No new adopted-name choices, profile-selection stage or simultaneous legal/birth/religious-name feature. |
| D6 | Research and provide targeted name content here. Evidence limitations remain explicit; Codex is not instructed to fabricate names or to perform the historical research. |

The five level names are catalogue metadata, not new player stats. Fluent 180 retains the previously proposed 0.90 ratio and sits near the user's approximately-200 fluency balance point; the editable prog may put it at 200 instead. No cap or success percentage is implied by these labels.

## 1.2 Era and geography policy

Keep five overlapping toolkits: Antiquity (classical antiquity to about 500), Dark Ages (about 500–1100), Medieval (about 1000–1400), Renaissance (1400–1600), Early Modern (1600–1750). Do not enforce a single reference year or promise that all entries coexist at every point. Dark Ages is a distinct culture option; the current item implementation groups that period under Medieval.

Coverage is Europe, Anatolia, the Levant, Mesopotamia, Arabia, Iran, the Caucasus, Egypt, the Maghreb and adjoining Pontic/Black Sea steppe. Preserve existing out-of-scope material in source/legacy content rather than deleting it, but do not expand this tranche into a world catalogue.

One selected culture era is installed. Reusing older languages inside that pack does not mean installing multiple culture packs. Existing Modern and Middle-Earth behaviour is not to be retuned by this task.

## 1.3 Identity and preservation

Ethnicity describes period-appropriate peoplehood; culture describes a regional, civic, social-estate or learned background. Keep them independently selectable. A culture does not itself award a legal status, title, property, membership, occupation or personality. New ethnicity labels are not biological-stat packages.

Reuse identity and naming content where genuinely continuous, with explicit context-sensitive variants where necessary. Ancient Veneti and later Venetians, and city-Roman and Greek-Roman identities, must not merge merely because old labels coincide. Existing detailed regional identities override broad source-template defaults.

Keep original names, weights, gender associations, regexes, name styles and accent data recoverable. A correction changes an active default or a presentation; it does not erase the authored source. Baseline-aware three-way reconciliation preserves builder edits on rerun.

## 1.4 Presentation and encoding

All active descriptions, name-element blurbs, accent suffixes and unknown-language text are contemporary in-world prose. Sources, designer rationale, caveats, confidence and implementation instructions live in separate fields. Retained material must pass the same prose review without losing valid local detail.

Use Latin-1 display forms, retaining supported accents and letters. Keep original Unicode/NFC spellings in research. Apply the reviewed fallback map and do not merge identities because their lossy displays collide. Do not silently enable unrestricted Unicode names or add a new encoding configuration question.

## 1.5 Language rules

Stable language-stage keys are distinct from era-dependent labels. A label change must also reconcile linked traits and generated references. Older retained languages are not automatic skills for speakers of descendants.

Intelligibility uses explicit directed edges. Positive defaults are VeryHard=7, ExtremelyHard=8 or Insane=9; absent/Impossible is not a weak positive edge. No transitive closure, shared-script inference or inference of learned fluency. Accent difficulty is independent.

Mandatory native/cultural awards are merged by trait identity and strongest base. Optional choices use PR #730's CountKnown and one-slot-per-skill rules. Native languages may receive explicit known credits; no duplicate skill or cumulative fluency bonus results.

## 1.6 Implemented dependency and remaining limitations

PR #730 supplies generic skill groups, all four screen adapters, persistence, accounting, builder commands and generated-template opt-in. Do not repeat that work. Implement only its CultureSeeder consumer and other content integration described in document 2.

The targeted research supplies eight bounded repertoires. The feminine Old Prussian replacement is not complete and must remain inactive. Small Latvian/Estonian/Romanian feminine samples are disclosed; none is padded with orthographic variants. This is an explicit delivery limitation, not authorisation to disguise an unrelated pool as completed research.
