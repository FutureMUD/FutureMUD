# CultureSeeder round-two verification

Verified locally on Windows with .NET SDK 10.0.302 and disposable local MySQL databases. Base: PR #732 (`2bbcdff3ed52e200ef17faa61dc3af0d9e9f3b86`). Implementation commit: `e052681cf6efd6170c3d776afd9ff8b45474fec1`. Branch: `codex/culture-toolkit-round2`; unmerged.

## Execution layers

The original handoff validator passed 6,951 checks (20 manifest files, 389 original entries preserved, 196 additions, 585 total entries, 58 playable cells and 14 native mappings). The updated runtime catalogue validator passed 4,066 checks. These are authoring checks, not game execution.

The normal `scripts/test-unit.ps1` pass succeeded across all nine fast projects. Final affected-suite reruns and focused counts are recorded below. Climate tests were excluded as prescribed by the repository. No persistence model or migration was changed.

| Suite | Executed | Result |
|---|---:|---|
| FutureMUDLibrary | 492 | passed |
| ExpressionEngine | 23 | passed |
| DatabaseSeeder | 853 | passed |
| MudSharpCore | 3060 | passed |
| MudsharpDatabaseLibrary | 43 | passed |
| DiscordBotCore | 22 | passed |
| RPI Engine Worldfile Converter | 43 | passed |
| FutureMUD.Web | 54 | passed |
| TerrainPlannerBlazor | 34 | passed |

Focused final checks: 65 seeder CultureToolkit tests and 25 core CultureToolkit/ChargenSkillSelectionGroupIntegration tests. The full fast script passed before the final added negative/era-policy assertions; the final complete seeder and core reruns supersede their earlier counts. Unchanged suites did not need another rebuild. Final suite totals cover 4,624 tests, with no failures or skips.

Seeder Debug and Release builds succeeded. Actual PE metadata confirms `DebugSeederReplayProfiles` is present only in Debug; see [build contracts](CultureSeederRound2/build-contracts.json). Existing unrelated compiler warnings remain; no new build error is accepted.

## Commands

```powershell
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
& scripts/test-unit.ps1
dotnet test 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1 -p:UseSharedCompilation=false -p:NoWarn=NU1902%3BNU1510
dotnet test 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1 -p:UseSharedCompilation=false -p:NoWarn=NU1902%3BNU1510
dotnet test 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -c Debug --no-build --no-restore -m:1 --filter FullyQualifiedName~CultureToolkit
dotnet test 'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj' -c Debug --no-build --no-restore -m:1 --filter 'FullyQualifiedName~CultureToolkit|FullyQualifiedName~ChargenSkillSelectionGroupIntegration'
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Release --no-restore -m:1 -p:UseSharedCompilation=false -p:NoWarn=NU1902%3BNU1510
python 'Design Documents/Seeding/CultureSeederRound2Handoff/tools/validate_handoff.py'
python 'Design Documents/Seeding/CultureSeederRedesignHandoff/tools/validate_catalogue.py'
python scripts/CultureToolkitSourceProbe/round2_content_receipt.py
git diff --check
```

The probe was built with `dotnet build scripts/CultureToolkitSourceProbe/CultureToolkitSourceProbe.csproj -c Debug --no-restore -m:1 -p:UseSharedCompilation=false -p:NoWarn=NU1902%3BNU1510 -o .appdata/round2-complete`. Separate local output directories avoided Windows locks from running probes. Modes below were run through its DLL with a process-local connection setting loaded from the installed test helper; credentials were neither passed on the command line nor committed.

## Relational and optional installations

Each live receipt records database creation/migration, stock CoreData/Time/Attribute/SkillPackage/Human/Chargen prerequisites, the real CultureSeeder entrypoint and a committed same-era rerun. These are independent databases, not five relabelled runs in one world. The stock prerequisite selection comes from the typed Debug replay profile; this does not claim a complete all-seeder replay.

| Era | Local database | Receipt |
|---|---|---|
| Antiquity | `futuremud_culture_live_2145_r2a0909` | [actual import and rerun](CultureSeederRound2/antiquity-live.json) |
| Dark Ages | `futuremud_culture_live_2145_r2d0909` | [actual import and rerun](CultureSeederRound2/darkages-live.json) |
| Medieval | `futuremud_culture_live_2145_r2m0909` | [actual import and rerun](CultureSeederRound2/medieval-live.json) |
| Renaissance | `futuremud_culture_live_2145_r2r0909` | [actual import and rerun](CultureSeederRound2/renaissance-live.json) |
| Early Modern | `futuremud_culture_live_2145_r2e0909` | [actual import and rerun](CultureSeederRound2/earlymodern-live.json) |

Read-only `--round2-live-graphs <live-receipt> <graph-receipt>` captures every managed script's actual MySQL language membership, linked trait IDs and acquisition body after the committed rerun; every stock predicate set matches its graph. These receipts are linked from the content report. This is structural database verification, distinct from the actual runtime prog execution tests.

Invocation: `--live-import <database> <era> <receipt>`. Medieval's first Culture transaction exposed a null optional Albanian description override; after correcting null handling, `--live-resume` used its exact failed receipt to resume from already-completed stock prerequisites. No reset or production database mutation was used.

`--round2-optional <receipt>` separately exercises names-only, languages-only and heritage-without-language installation, then reruns each, for all five eras: [15 isolated fixture results](CultureSeederRound2/optional-fixtures.json). These use actual retained source evaluation and installer code, but their writes are EF InMemory and do not establish relational semantics.

`--round2-builder-rerun <darkages-live-receipt> <report>` performs two actual MySQL full reruns with a removed script member, custom language/trait membership, altered/deleted/added names and custom accent pointer. It then intentionally fails prog compilation after reconciliation inside a new caller-owned transaction. Counts, original acquisition body and original membership are verified after rollback: [builder and atomicity receipt](CultureSeederRound2/builder-rerun.json). Every fixture mutation was rolled back; the clean imported database was retained.

## R2-26 live acceptance — 2026-09-12

An isolated disposable MySQL target, `futuremud_culture_live_2145_r226b`, was created from the Debug replay profile and seeded through the real CultureSeeder entrypoint for Medieval. Its committed same-era rerun completed with zero culture conflicts: [fresh import and rerun receipt](CultureSeederRound2/r226-live-import-passed.summary.json). `labmud_dbo` was not accessed or modified.

The live MUD ran against that target. The builder proof temporarily removed and restored English from the Latin script's designed-language associations: [script association edit](CultureSeederRound2/r226-builder-script-edit.txt). A normal English Nobility application was [saved pending](CultureSeederRound2/r226-chargen-pending-menu.txt), [reopened](CultureSeederRound2/r226-chargen-resume-normal.txt), named Edmund Ashford, [submitted](CultureSeederRound2/r226-chargen-submit.txt), [approved](CultureSeederRound2/r226-admin-application-approved.txt), and reconnected; the final read-only receipt and reconnect transcript record the expected native Middle English plus Carolingian Latin, Anglian Middle English and Crude Anglo-Norman French accents: [live final receipt](CultureSeederRound2/r226-live-final-receipt.summary.json), [reconnect](CultureSeederRound2/r226-edmund-reconnect.txt).

The stock world intentionally has no generic enabled chargen skill group. To exercise the required group phase without changing shipped seeder content, a target-only `R226Education` group and compiled `Boolean(chargen)` eligibility prog were created through the live builder and validated: [fixture](CultureSeederRound2/r226-skillgroup-prog-fixture.txt). A second ordinary English Nobility application selected Brawling through that group, selected Scribe through the normal open-skill picker, then selected the three accents above: [group and open skill](CultureSeederRound2/r226-chargen-educated-group-and-open-skill.txt), [accents](CultureSeederRound2/r226-chargen-educated-accents.txt).

During the first disposable rerun, retained accents with source-language endpoints unavailable in the selected earlier era were incorrectly reported as conflicts. `CultureToolkitLanguageSeeder` now recognizes explicitly excluded later-era language specifications, leaves those endpoints unassociated, and continues to flag missing in-era endpoints. The focused affected set passes 42 tests, including the new earlier/later-era and builder-association coverage; the complete `CultureToolkit` seeder subset passes 96 tests.

## Acceptance mapping

Test classes below are under `DatabaseSeeder Unit Tests` unless prefixed `Core`. “Passed” describes the concrete execution listed, not an assertion of the separate interactive check.

| Case | Result | Concrete evidence |
|---|---|---|
| R2-01 | passed | Five era composition data rows in `CultureToolkitCatalogueTests`; five actual CultureSeeder imports. No new legacy migration, reset, alias or version-lock code. |
| R2-02 | passed | Core `EveryFreshSocialCultureUsesItsActualNativeStartingHook` executes every stock social culture's actual hook for all five eras, actual `Skill` clamp, native 200, boost delta and non-language delegation. `WelshEnglishNobilityExecutesEditableMaxBasesAndOriginalBoostDeltaExactlyOnce`, cap and partition runtime tests cover tiers, stronger independent values and maxima. |
| R2-03 | passed | `CultureToolkitScriptSeederTests` deletes English, reruns twice and executes the real compiled prog: false. MySQL builder receipt separately proves relational preservation. |
| R2-04 | passed | Same test executes the custom noncanonical language's real linked trait: true; builder receipt captures actual added language/trait IDs and graph. |
| R2-05 | passed | Same test deletes the final member, reruns and executes the empty graph's `return false`. |
| R2-06 | passed | Same test preserves custom knowledge pointer and independently edited generated body. The receipt does not equate a custom policy with stock graph eligibility. |
| R2-07 | passed | MySQL `AtomicCompilationRollback=true`; deliberate missing-function compilation error after membership reconciliation, followed by rollback and original graph/body/count assertions. |
| R2-08 | passed | `SyntheticLearnerSupportsLanguageOnlyFullAndRepeatedLanguageOnlyOrdering(false)` creates source-less Early Modern English and executes learner and local availability for native/non-native ethnicity IDs. |
| R2-09 | passed | Core `ChargenSkillSelectionGroupIntegrationTests` drives the generated adapter for acquired and ethnic-native speakers, blocked learner diagnostics and preserved manual accents, across existing group/open skill screens. This is runtime adapter execution, not a telnet-generated NPC. |
| R2-10 | passed | Synthetic learner test's first language-only phase uses an empty native set and executes usable learner/default policy; later phases install native restrictions. |
| R2-11 | passed | Synthetic test's `true` data row supplies the unchanged PR732 AlwaysFalse baseline and verifies repair; genuine source false remains effective, edited combined body/null default and custom pointers survive. |
| R2-12 | passed | Synthetic test executes language-only -> full -> language-only -> full with stable two-accent IDs and persisted native restrictions. Fifteen optional integrated fixtures provide broader era coverage. |
| R2-13 | passed | Seven `RetainedLatinAvailabilityExecutesEraPolicyAndKeepsDisabledFallbacks` cases compile and execute Classical, Liturgical, Neo-Classical and Carolingian policies in allowed/forbidden eras. Five MySQL receipts retain each source row and selected policy. |
| R2-14 | passed | Forbidden Latin fixture retains ineligible source native, creates canonical local fallback and preserves a builder-disabled fallback on rerun. |
| R2-15 | passed | Synthetic ordering/source restriction tests and MySQL two-rerun receipt preserve combined body/default/pointer edits without duplicate owners or recursive predicates. |
| R2-16 | passed | Antiquity live receipt resolves all 56 source identities. Core all-cultures test additionally executes fixed-skill and starting hooks for all 13 supplied additions with their exact reference keys. |
| R2-17 | passed | Medieval/Renaissance/Early Modern live binding receipts resolve the exact retained Albanian source identity to canonical `albanian`; optional null description/group fields preserve source values. |
| R2-18 | passed | `ExactSourceCrosswalkRejectsDuplicatesAndReportsInvalidReferencesWithoutEnablingUnrelatedRows` checks duplicate source/name/era diagnostics, misspelled reference and unrelated unresolved identity. Existing source preflight tests reject missing source definitions and ambiguous edited bindings before writes. |
| R2-19 | passed | All 58 post-filtered stock cells checked in catalogue tests and both validators; exact distinct-family/display/weight and evidence-class counts in content receipt. |
| R2-20 | passed | Core generation test exercises enabled Old Prussian female profiles in four eras; persisted name definitions/profiles and links are included in fresh imports and same-era reruns. Counts are 20 Dark Ages and 26 later. |
| R2-21 | passed | Handoff validator and content-receipt renderer compare every original field of all 389 entries against PR #732; 196 additions retain reconstruction/borrowing labels, Botezata is excluded from playable birth names. |
| R2-22 | passed | `CultureToolkitNameSeederTests` plus actual MySQL two-rerun fixture preserve deleted name, weight 7 and added BuilderExample weight 3. Stock floor does not restore builder deletions. |
| R2-23 | passed | Core naming tests parse every active full form, render and XML-reload it, exercise random profiles, original compound/byname fixtures and real NamePicker with Unicode disabled. Live imports persist the corresponding database profile rows. |
| R2-24 | passed | Five independent relational full imports/reruns plus 15 optional integrated fixtures; see scope distinctions above. |
| R2-25 | passed | Explicit 30-name resource contract, including the two accent-role resources; validator and runtime-contract test reject missing inputs and unrelated substitutes. Invalid source/crosswalk references fail separately. |
| R2-26 | passed | Fresh isolated MySQL import/rerun, live builder script edit, saved/reopened normal application, target-only skill-group/open-skill fixture, accent selection, named submission/approval and reconnect are recorded in the R2-26 live-acceptance evidence above. |

## Failures corrected during verification and limits

Initial checks exposed incorrect FutureProg negation syntax, null optional Albanian prose handling, and source-stage EF provider-cache accumulation under repeated fixtures. Each was corrected and the affected checks rerun. The relational rollback probe also needed to materialize managed records before issuing nested MySQL lookups; the same precaution now applies to accent ownership lookup. Test setup corrections supplied retained-source trait fixtures and persisted IDs before runtime name XML round-trips.

The final receipts supersede these preliminary failures. The later-era accent-endpoint rerun defect identified during the R2-26 fixture is corrected and covered by focused tests plus the clean rerun receipt. No unresolved implementation defect is currently known. No hosted CI, PR merge, production deployment or running-server hot reload is claimed.
