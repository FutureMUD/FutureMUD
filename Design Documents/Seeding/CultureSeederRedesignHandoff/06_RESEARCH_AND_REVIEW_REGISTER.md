# 6. Research and review register

> Round-two update: the corrective brief in [CultureSeederRound2Handoff](../CultureSeederRound2Handoff/AGENT_TASK.md) supersedes earlier naming activation gates and resource totals below. The runtime catalogue now has 28 required inputs and 585 name entries, including 389 unchanged evidence entries and 196 approved fictional profile entries. All 58 playable gender/era cells meet the 20-family floor; Old Prussian feminine profiles are enabled. Earlier evidence limitations remain historical notes, not activation blockers.


## 6.1 What is evidence and what is authored

Source observations, modelling decisions and implementation requirements are separate. Era envelopes, skill values, equal random weights and mutual-intelligibility difficulties are authored game defaults. They are not empirical historical measurements. Contemporary player prose must not incorporate this administrative qualification.

The original corpus is retained without new historical certification. All 389 original entries preserve their evidence fields. Round two adds 196 explicitly reconstructed or borrowed profile entries; none is claimed as an attested full form. A documentary date, saint's lifetime and editorial gameplay-era selection are distinct facts.

## 6.2 Current delivered entry totals

| Repertoire | Masculine entries | Feminine entries |
|---|---:|---:|
| Finnish Household Names | 58 | 51 |
| Lithuanian Household Names | 46 | 59 |
| Latvian Household Names | 30 | 20 |
| Estonian Household Names | 40 | 40 |
| Old Prussian Personal Names | 30 | 26 |
| Romanian Household Names | 34 | 37 |
| Coptic Christian Names | 34 | 40 |
| Syriac Christian Names | 20 | 20 |

Totals include evidence-only entries and do not establish exhaustive independent historical lemma counts. The [content receipt](../../Verification/CultureSeeder_Round2_Content_Receipt.md) gives active family/display counts and evidence-class totals per gender/era. The supplied finite forms resolve the gameplay gaps without productive morphology or spelling-variant inflation.

## 6.3 Review outcomes

### NG01 — Baltic and Finnic production repertoires

Status: **resolved-by-approved-playable-reconstruction**.
The existing shared Finno-Ugric naming key does not validate Lithuanian, Latvian, Prussian, Estonian and Finnish inventories.
Use targeted_name_corpora.json and name_playability_policy.json. The approved finite reconstructed forms enable Old Prussian feminine profiles and complete all required Baltic/Finnic cells. Historical-evidence limitations remain nonblocking research notes; do not infer productive suffix rules.
Sources: N01, N02, N03, H22, N04, N05, N06, N07.

### NG02 — Romanian/Vlach names

Status: **resolved-by-approved-playable-reconstruction**.
A Western Slavic fallback is not an adequate Romanian repertoire.
Use the delivered expanded household repertoire with explicit playable eras and editorial weights. Original dynastic and inscription provenance remains unchanged; no claim of representative commoner frequencies.
Sources: N08, N09, N10, N11, N12, N13.

### NG03 — Coptic and Syriac Christian name profiles

Status: **resolved-by-approved-playable-reconstruction**.
Arabic political-template profiles can contain an inappropriate religious repertoire.
Use separate Coptic and Syriac pools. Preserve the distinction between documentary names and devotional/literary repertoire. No claim that a saint floruit dates a vernacular spelling or represents ordinary-name frequencies.
Sources: N14, N15, N16, N17.

### NG04 — Gender-sensitive and optional name grammar

Status: **authored-pattern-fixtures-delivered-engine-tests-required**.
Existing morphology must survive consolidation; a surname ending is not a universally productive rule.
No new productive gender morphology. Reuse reviewed atomic names, optional bynames and existing proven forms. Run naming_pattern_tests.json through actual parser/renderer. No adopted-name selector.
Sources: U02, R04, R06, N01, N05.

### LG01 — Ambiguous legacy language labels

Status: **required-binding-check**.
Greek, English and Persian may denote different stages in different source modules.
Bind by source context and canonical stage. Do not upsert based on label equality alone.
Sources: R03, R12, R13, R17, R18.

### LG02 — Circassian, Avestan and ancient Egyptian elective dependencies

Status: **required-before-showing-option**.
Those retained-catalogue candidate names are not newly specified language records here.
Resolve an actual suitable record or leave the optional candidate visibly unimplemented; never pretend an unrelated language is equivalent.
Sources: R18, H08, H11, H24.

### CG01 — Regional chronology and social labels

Status: **builder-guidance-not-a-global-blocker**.
Era toolkits deliberately overlap and contain material from different centuries.
Retain applicability notes, regional variants and title changes; do not invent a single date of peoplehood emergence.
Sources: H08, H10, H13, H16.

### EG01 — Latin-1 fallback and name entry

Status: **required-engine-tests**.
Unsupported non-decomposing letters become question marks, and the fallback buffer is shared.
Implement the supplied map/NFC/isolation policy; verify the actual .NET encoder and chargen round-trip before enabling new unsupported source spellings.
Sources: R14, R16, T01, T02.

### EG02 — Generic skill-group dependency and profile preservation

Status: **generic-feature-merged-preservation-still-in-scope**.
PR730 implements the generic feature. Original profile upserts still need non-destructive reconciliation in this CultureSeeder work.
Use ChargenSkillSelectionGroupSeeder.Upsert; do not rebuild groups, screens or allocator. Preserve names and builder edits through the separately specified source/baseline ledger.
Sources: R730A, R730B, R03.

## 6.4 Verification boundaries

PR #730 is an implemented dependency. Its uploaded report records local automated test results, but live MySQL migration/snapshot import and interactive telnet/editor checks were not executed. Those results are not rerun or independently certified by this handoff. The agent must verify the final committed baseline and its own changes.

The Python handoff validator checks references, encoding, policies, generated profile structure and data consistency. It is not a historical-attestation oracle, a C# test, a FutureProg compiler or a migration execution. Read the warnings and inactive-profile status even when structural checks pass.

## 6.5 Source register

Each entry states the actual use of the source. An abstract-only source does not license names or facts absent from that abstract. Repository references describe code; they are not historical evidence.

### R01 — DatabaseSeeder/Seeders/ItemSeeder.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/ItemSeeder.cs

Current selectable item eras; Dark Ages is within Medieval, not a separate menu option.
Kind: repository. Accessed: 2026-09-07.

### R02 — DatabaseSeeder/Seeders/CultureSeeder.Packs.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Packs.cs

Pack dispatch, legacy MedievalEurope alias, independent optional content.
Kind: repository. Accessed: 2026-09-07.

### R03 — DatabaseSeeder/Seeders/CultureSeeder.Shared.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Shared.cs

Name-based upserts, destructive replacement of existing profile elements, language traits, skill caps, culture naming links.
Kind: repository. Accessed: 2026-09-07.

### R04 — DatabaseSeeder/Seeders/CultureSeeder.Names.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Names.cs

Existing name cultures, inventories, weights and name patterns: preserve by reference, not reproduced in this handoff.
Kind: repository. Accessed: 2026-09-07.

### R05 — DatabaseSeeder/Seeders/CultureSeeder.NameDefaults.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.NameDefaults.cs

Current ethnicity/name mappings and generic fallback inventories.
Kind: repository. Accessed: 2026-09-07.

### R06 — DatabaseSeeder/Seeders/CultureSeeder.Packs.DarkAgesAndMedieval.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Packs.DarkAgesAndMedieval.cs

Existing medieval naming and heritage bundles; preserve inventories and reference dates.
Kind: repository. Accessed: 2026-09-07.

### R07 — DatabaseSeeder/Seeders/CultureSeeder.Packs.RenaissanceWorld.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Packs.RenaissanceWorld.cs

Existing Near Eastern naming content within the wider world catalogue.
Kind: repository. Accessed: 2026-09-07.

### R08 — DatabaseSeeder/Seeders/CultureSeeder.Heritage.Antiquity.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Heritage.Antiquity.cs

Existing antiquity ethnicity and culture catalogue; retained, with targeted corrections.
Kind: repository. Accessed: 2026-09-07.

### R09 — DatabaseSeeder/Seeders/CultureSeeder.Heritage.MedievalEurope.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Heritage.MedievalEurope.cs

Legacy Renaissance ethnicity granularity and regional cultures.
Kind: repository. Accessed: 2026-09-07.

### R10 — DatabaseSeeder/Seeders/CultureSeeder.Heritage.DarkAges.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Heritage.DarkAges.cs

Existing regional medieval variants; reuse rather than rebuilding peoplehood for each era.
Kind: repository. Accessed: 2026-09-07.

### R11 — DatabaseSeeder/Seeders/CultureSeeder.Languages.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Languages.cs

Script acquisition progs currently compare skill display names; language and accent helpers.
Kind: repository. Accessed: 2026-09-07.

### R12 — DatabaseSeeder/Seeders/CultureSeeder.Languages.DarkAges.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Languages.DarkAges.cs

Preserve medieval languages and all authored accent descriptions.
Kind: repository. Accessed: 2026-09-07.

### R13 — DatabaseSeeder/Seeders/CultureSeeder.Languages.RenaissanceWorld.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Languages.RenaissanceWorld.cs

Preserve Near Eastern language and accent content in the world expansion.
Kind: repository. Accessed: 2026-09-07.

### R14 — MudSharpCore/CharacterCreation/Screens/NamePickerScreen.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/CharacterCreation/Screens/NamePickerScreen.cs

Ethnicity-first naming selection, Latin-1 input restriction, name validation, name construction.
Kind: repository. Accessed: 2026-09-07.

### R15 — MudSharpCore/CharacterCreation/Screens/SkillPickerScreen.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/CharacterCreation/Screens/SkillPickerScreen.cs

Automatic free skills cannot be toggled; free-skills and suggested-skills FutureProg hooks.
Kind: repository. Accessed: 2026-09-07.

### R16 — FutureMUDLibrary/Framework/StringExtensions.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/FutureMUDLibrary/Framework/StringExtensions.cs

Latin1EncoderFallback: FormD, mark stripping, shared fallback buffer, unmapped letters become question marks.
Kind: repository. Accessed: 2026-09-07.

### R17 — DatabaseSeeder/Seeders/CultureSeeder.Languages.MedievalEurope.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Languages.MedievalEurope.cs

Existing Renaissance-era European languages and accents: preserve source material.
Kind: repository. Accessed: 2026-09-07.

### R18 — DatabaseSeeder/Seeders/CultureSeeder.Languages.Antiquity.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/DatabaseSeeder/Seeders/CultureSeeder.Languages.Antiquity.cs

Existing ancient languages, accents and scripts: preserve source material.
Kind: repository. Accessed: 2026-09-07.

### R19 — Design Documents/Seeding/Culture_Seeder_Heritage_Pack_Reference.md

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/Design%20Documents/Seeding/Culture_Seeder_Heritage_Pack_Reference.md

Existing conceptual separation of ethnicity and culture; existing global content is not to be deleted.
Kind: repository. Accessed: 2026-09-07.

### R20 — Design Documents/Seeding/Culture_Seeder_Language_Pack_Reference.md

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/Design%20Documents/Seeding/Culture_Seeder_Language_Pack_Reference.md

Existing script/knowledge reconciliation and naming/language coverage policy.
Kind: repository. Accessed: 2026-09-07.

### H01 — Dictionary of Medieval Names from European Sources: project scope

https://dmnes.org/about

Given-name attestation methodology, period 500-1600; not blanket validation of every legacy name.
Kind: research. Accessed: 2026-09-07.

### H02 — PASE: database selectors and examples

https://pase.ac.uk/help/pase-database/

Primary-record prosopography; names, status, office and occupation are separate but overlapping descriptors.
Kind: research. Accessed: 2026-09-07.

### H03 — PASE: database statistics

https://pase.ac.uk/about/database-statistics/

Unequal survival of evidence, including substantially fewer recorded women; do not infer lack of actual names.
Kind: research. Accessed: 2026-09-07.

### H04 — University of Nottingham: languages used in medieval documents

https://www.nottingham.ac.uk/manuscriptsandspecialcollections/researchguidance/medievaldocuments/languages.aspx

English, French and Latin differentiated by education and social context; Law French survives beyond everyday Anglo-Norman.
Kind: research. Accessed: 2026-09-07.

### H05 — Australian National Maritime Museum: Vikings and the Viking Age

https://www.sea.museum/en/learn/resource/vikings

Norse household, landholding, free and unfree status distinctions.
Kind: research. Accessed: 2026-09-07.

### H06 — British Museum: slavery in ancient Rome

https://www.britishmuseum.org/exhibitions/nero-man-behind-myth/slavery-ancient-rome

Enslaved and freed status, variation in experience, and continuing patronal obligations.
Kind: research. Accessed: 2026-09-07.

### H07 — British Museum: Roman Republican freed couple, 1867,0508.55

https://www.britishmuseum.org/collection/object/G_1867-0508-55

Greek origin and Roman civic naming can coexist; a specific example, not a universal naming formula.
Kind: research. Accessed: 2026-09-07.

### H08 — Encyclopaedia Iranica: Class System iii

https://www.iranicaonline.org/articles/class-system-iii/

Parthian and Sasanian social differentiation; regional variation and documentary limits.
Kind: research. Accessed: 2026-09-07.

### H09 — Encyclopaedia Iranica: Class System iv

https://www.iranicaonline.org/articles/class-system-iv/

Medieval Islamic Persian social strata; not an interchangeable copy of European estates.
Kind: research. Accessed: 2026-09-07.

### H10 — Encyclopaedia Iranica: Dehqan

https://www.iranicaonline.org/articles/dehqan/

Landed dehqan identity and its changing meaning over time.
Kind: research. Accessed: 2026-09-07.

### H11 — Metropolitan Museum: The Art of the Mamluk Period

https://www.metmuseum.org/de/essays/the-art-of-the-mamluk-period-1250-1517

Mamluk military institution, Turkic and Circassian recruitment, acquired Arabic instruction.
Kind: research. Accessed: 2026-09-07.

### H12 — Metropolitan Museum: The Greater Ottoman Empire, 1600-1800

https://www.metmuseum.org/ko/essays/the-greater-ottoman-empire-1600-1800

Imperial institutions, urban commerce and continuing local/ethnic cultural life.
Kind: research. Accessed: 2026-09-07.

### H13 — Wilanow Palace Museum: Sarmatism

https://wilanow-palac.pl/en/knowledge/sarmatism

Polish-Lithuanian noble cultural formation; claimed Sarmatian descent is ideology, not an ethnicity assignment.
Kind: research. Accessed: 2026-09-07.

### H14 — Wilanow Palace Museum: Sarmatism and European culture

https://wilanow-palac.pl/en/knowledge/sarmatism-and-european-culture

Noble political ideals and cultural borrowing; use of the modern scholarly label must be explained.
Kind: research. Accessed: 2026-09-07.

### H15 — Encyclopaedia Iranica: Early New Persian

https://www.iranicaonline.org/articles/persian-language-1-early-new-persian/

Middle/New Persian distinction; continued variation, learned use and multiple writing traditions.
Kind: research. Accessed: 2026-09-07.

### H16 — Len Scales, The Shaping of German Identity: publisher description

https://www.cambridge.org/core/books/shaping-of-german-identity/C6134B0992E7162F0636841FE9EC6CE2

Supports a late-medieval broad German identity; not evidence for a single precise ethnogenesis date.
Kind: research. Accessed: 2026-09-07.

### H17 — Oxford: Lexicon of Greek Personal Names

https://lgpn.web.ox.ac.uk/

Ancient naming evidence and regional coverage; use for individual checks, not automatic modern-Greek back-projection.
Kind: research. Accessed: 2026-09-07.

### H18 — Oxford: LGPN Egypt

https://lgpn.web.ox.ac.uk/search-lgpn-egypt-online

Greek/Egyptian personal names with individual provenance and dating.
Kind: research. Accessed: 2026-09-07.

### H19 — Merchant Adventurers Hall, York: institutional history

https://merchantshallyork.org/the-hall/

Merchant guild and civic institutional setting; not proof that all merchants knew Latin.
Kind: research. Accessed: 2026-09-07.

### H20 — University course resource: English language timeline

https://www.csun.edu/~sk36711/WWW/medlit/english_lang_timeline.html

Approximate Old/Middle English chronological distinction; era labels in this handoff are gameplay labels.
Kind: research. Accessed: 2026-09-07.

### T01 — Unicode: Latin-1 Supplement names list

https://www.unicode.org/charts/nameslist/c_0080.html

ISO-8859-1 repertoire is not Windows-1252; supported versus unsupported name characters.
Kind: technical. Accessed: 2026-09-07.

### T02 — Unicode: block ranges

https://unicode.org/charts/nameslist/mainList.html

Latin Extended letters and combining marks are outside Latin-1.
Kind: technical. Accessed: 2026-09-07.

### H21 — Medieval Names Archive: Baltic bibliography

https://s-gabriel.org/names/baltic.shtml

Research leads for distinct Baltic and Finnic naming; linked individual sources need their own review.
Kind: research. Accessed: 2026-09-07.

### H22 — Rebecca Lucas: Some names from 15-17th century Latvia

https://www.s-gabriel.org/names/ffride/latvian_bynames.html

Dated forms in German-language records, distinguished from modern Latvian headings; a small feminine inventory is not proof that the culture is poorly documented.
Kind: research. Accessed: 2026-09-07.

### H23 — Liviu Marius Ilie: Nunnery Life in 16th Century Wallachia

https://www.czasopisma.uni.lodz.pl/sceranea/article/view/17567

Musa/Magdalina illustrates a change of name on entering monastic life; abstract and references consulted, not the full paper.
Kind: research. Accessed: 2026-09-07.

### H24 — Trismegistos: People and Names

https://www.trismegistos.org/ref/index.php/

Primary-record naming resource for Egypt; public results are restricted, so this handoff does not claim a complete Coptic extraction.
Kind: research. Accessed: 2026-09-07.

### H25 — Romanian Names from the Basarab Line

https://www.s-gabriel.org/names/arina/basarab.html

Dated elite Romanian naming leads, not a representative all-class name-frequency dataset.
Kind: research. Accessed: 2026-09-07.

### U01 — Luke: requested handoff revisions, 7 September 2026

None

Authority for VeryHard through Insane calibration; contemporary in-world descriptions; generic skill-group picks after free skills and before open selection. Not an external historical source.
Kind: user-design-requirement. Accessed: 2026-09-07.

### R21 — FutureMUDLibrary/RPG/Checks/ICheck.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/FutureMUDLibrary/RPG/Checks/ICheck.cs

Verified difficulty enum: VeryHard=7, ExtremelyHard=8, Insane=9, Impossible=10.
Kind: repository. Accessed: 2026-09-07.

### R22 — MudSharpCore/Communication/Language/Language.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/Communication/Language/Language.cs

Verified listener-owned directed links and absent-link Impossible result.
Kind: repository. Accessed: 2026-09-07.

### R23 — FutureMUDLibrary/Communication/Language/LanguageInfo.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/FutureMUDLibrary/Communication/Language/LanguageInfo.cs

Repository excerpt verifies listening difficulty uses max(accent, utterance, intelligibility) and the known-language linked trait.
Kind: repository. Accessed: 2026-09-07.

### R24 — MudSharpCore/Character/CharacterCommunication.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/Character/CharacterCommunication.cs

Repository excerpt identifies writing/reading use of directed language intelligibility; literacy/script checks must remain intact.
Kind: repository. Accessed: 2026-09-07.

### R25 — MudSharpCore/CharacterCreation/Screens/SkillCostPickerScreen.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/CharacterCreation/Screens/SkillCostPickerScreen.cs

Existing point-based screen identified as an integration target; new group-stage behaviour is a requirement, not an implemented feature.
Kind: repository. Accessed: 2026-09-07.

### R26 — MudSharpCore/CharacterCreation/Screens/SkillSkipperScreen.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/CharacterCreation/Screens/SkillSkipperScreen.cs

Existing skip-open-selection screen and FreeSkillsProg integration target; must not bypass required group choices.
Kind: repository. Accessed: 2026-09-07.

### R27 — MudSharpCore/CharacterCreation/Screens/SkillBoostSkipperScreen.cs

https://github.com/FutureMUD/FutureMUD/blob/f06a9eff0e14b19f7ca41b0372738ec3a7078c8b/MudSharpCore/CharacterCreation/Screens/SkillBoostSkipperScreen.cs

Existing boost-screen integration target; preserve paid-boost semantics after group selections.
Kind: repository. Accessed: 2026-09-07.

### MI01 — Gooskens et al. (2018), Mutual intelligibility between closely related languages in Europe

https://www.tandfonline.com/doi/full/10.1080/14790718.2017.1350185

Primary research on spoken comprehension, exposure and asymmetry in modern European languages. Supports methodological caution, not any historical engine score.
Kind: primary-research. Accessed: 2026-09-07.

### MI02 — Gooskens and Swarte (2017), Linguistic and extra-linguistic predictors of mutual intelligibility between Germanic languages

https://www.cambridge.org/core/journals/nordic-journal-of-linguistics/article/linguistic-and-extralinguistic-predictors-of-mutual-intelligibility-between-germanic-languages/65851C6257507D865362296D0FCFE02C

Primary research separates exposure and linguistic distance, spoken and written tests. Historical scores in this packet remain authored approximations.
Kind: primary-research. Accessed: 2026-09-07.

### MI03 — Golubovic and Gooskens (2015), Mutual intelligibility between West and South Slavic languages

https://link.springer.com/article/10.1007/s11185-015-9150-9

Primary research concerns modern West/South Slavic test languages, not an empirical reconstruction of medieval Slavic comprehension or Russian-Ukrainian engine scores.
Kind: primary-research. Accessed: 2026-09-07.

### U02 — Final D1-D6 decisions

conversation:final-decisions

Ethnic native grants; 200-based minimal-code proficiency; delegated general electives; learned literacy; no adopted names; finish targeted research.
Kind: user decision. Accessed: 2026-09-07.

### R730A — Merged generic group seeder helper

https://github.com/FutureMUD/FutureMUD/blob/961efca81da0d788bbfb86ccf55fea313c9bf065/DatabaseSeeder/Seeders/Utilities/Chargen/ChargenSkillSelectionGroupSeeder.cs

Exact Upsert boundary and compiled prog contracts.
Kind: repository implementation. Accessed: 2026-09-07.

### R730B — Skill selection groups completion guide

https://github.com/FutureMUD/FutureMUD/blob/961efca81da0d788bbfb86ccf55fea313c9bf065/Design%20Documents/Characters/Chargen_Skill_Selection_Groups.md

Already implemented PR730 dependency; no new chooser infrastructure.
Kind: repository implementation guide. Accessed: 2026-09-07.

### N01 — Finnish names in historical records — Lea Viljanen

https://heraldry.sca.org/names/FinnishNamesArticle.htm

Individual dated forms transcribed from Finnish medieval/early modern records, including Latin and Swedish documentary spellings. Not a census of ethnic Finns.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N02 — Kaunas names, 1522–1591 — Rebecca Lucas after Alma Ragauskaite

https://www.s-gabriel.org/names/ffride/kaunaslocnam.html

Multilingual town records. Keep recorded nominative spellings and distinguish reconstructed nominatives.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N03 — Women in Grand Duchy of Lithuania registers — after Jurate Cirunaite

https://www.s-gabriel.org/names/ffride/lithuanianwomenasmenv.html

Registers of 1528, 1565, 1567 and 1631; Christian and Muslim repertoires distinguished. Political geography is not proof of Lithuanian vernacular ethnicity.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N04 — Estonian names from trade and tax records — after Juri Kivimae

https://s-gabriel.org/names/ffride/kivimae-names.html

Documentary spellings, chiefly sixteenth century; mixed Finnic and Low German recording traditions.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N05 — Estonian naming patterns — Aryanhwy merch Catmael

https://heraldry.sca.org/names/eepatterns.html

Local records, particles and byname examples; editorially reconstructed examples are not individual attestations.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N06 — Ernst Lewy, Die altpreussischen Personennamen (1904)

https://archive.org/stream/diealtpreussisch00lewy/diealtpreussisch00lewy_djvu.txt

Public-domain personal-name corpus. Selected forms from sections 63–67; individual documentary dates not independently extracted.
Kind: historical linguistic monograph, public domain. Accessed: 2026-09-07.

### N07 — Ivoska, Social Status and Identification of Prussian Women (2023)

https://journals.lki.lt/actalinguisticalithuanica/article/view/2237

Abstract inspected; full article could not be retrieved. It supplies NO name entries in this handoff.
Kind: research lead, abstract only. Accessed: 2026-09-07.

### N08 — Romanian names — Sara L. Uckelman

https://www.ellipsis.cx/~liana/names/other/romanian.html

Dated ruler-name compilation; explicitly elite-biased, not a representative commoner frequency list.
Kind: original research / edited historical evidence. Accessed: 2026-09-07.

### N09 — Names from the family of Basarab — Jennifer Edwards

https://www.s-gabriel.org/names/arina/basarab.html

Dynastic family-name compilation; dates and vernacular normalisations are not manuscript diplomatic transcriptions.
Kind: historical-name compilation. Accessed: 2026-09-07.

### N10 — Neagoe Basarab and his family — National History Museum portrait catalogue

https://galeriaportretelor.ro/item/neagoe-basarab-si-familia-sa/

Museum identification of sixteenth-century family portrait; Milita, Anghelina, Ruxandra and Stanca. Milita is a Serbian-origin court member.
Kind: museum collection catalogue. Accessed: 2026-09-07.

### N11 — Three Hierarchs monastery foundation inscription

https://doxologia.ro/pisania-manastirii-sfintii-trei-ierarhi

Published inscription text naming Tudosca, Maria and Rucsandra, 1639.
Kind: institutional transcription of historical inscription. Accessed: 2026-09-07.

### N12 — Domnita Balasa church

https://doxologia.ro/biserica-domnita-balasa

Institutional history of Balasa (1693–1752), foundation of 1743–44; no nineteenth-century donors imported.
Kind: institutional historical account. Accessed: 2026-09-07.

### N13 — Matei Basarab and Elina — National History Museum

https://galeriaportretelor.ro/item/matei-basarab-si-doamna-elina-2/

Museum portrait identification: Elina (c.1598–1653), seventeenth-century fresco.
Kind: museum collection catalogue. Accessed: 2026-09-07.

### N14 — Coptic Church Synaxarion — annual calendar

https://www.copticchurch.net/synaxarium/all/en

Names in the Coptic commemorative tradition. This is evidence for devotional repertoire, not birth frequencies, ethnic origin or a dated vernacular manuscript spelling.
Kind: primary evidence of a living liturgical tradition. Accessed: 2026-09-07.

### N15 — Syriaca.org — Syriac Reference Portal

https://syriaca.org/

Individual records identify Syriac literary/hagiographic names and primary editions. Saint floruit is NOT the date of the surviving text or the name spelling.
Kind: scholarly reference database. Accessed: 2026-09-07.

### N16 — Trismegistos Names — public metadata

https://www.trismegistos.org/name/

Specific publicly indexed name records only. No restricted dataset was downloaded. Egyptian documentary location/language does not alone prove religion or ethnicity.
Kind: documentary name database. Accessed: 2026-09-07.

### N17 — Marana and Cyra — Monastic Matrix

https://arts.st-andrews.ac.uk/monasticmatrix/vitae/cyra

Theodoret-related record of two women ascetics; literary personal names, not population frequencies.
Kind: research database drawing on primary hagiography. Accessed: 2026-09-07.

### N18 — Eugenio Lujan — Language and writing among the Lusitanians

https://www.researchgate.net/publication/332491879_Language_and_writing_among_the_Lusitanians

Author chapter abstract: Lusitanian inscriptions, Latin alphabet and distinct language treatment. No phonetic reconstruction or intelligibility rate taken from the abstract.
Kind: author research chapter abstract. Accessed: 2026-09-07.
