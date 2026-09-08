# CultureSeeder redesign implementation record

## Scope and authoritative inputs

The implementation starts at `420fa26a7cb2be11fbb7b973e7e5270976e95067`, a verified descendant of PR #730 commit `961efca81da0d788bbfb86ccf55fea313c9bf065`. The implementation commit is reported with the final delivery; this record is maintained in that same change.

The final handoff is restored under [CultureSeederRedesignHandoff](../Seeding/CultureSeederRedesignHandoff/AGENT_TASK.md). The original 22 data and four research JSON files pass the supplied **26/26** presence, parse and SHA-256 checks. The original catalogue validator passes **3,810/3,810** checks. These are input checks, not C#, MySQL or historical-attestation tests. Git attributes preserve their original bytes.

## Installed behavior

The menu offers Antiquity, Dark Ages, Medieval, Renaissance and Early Modern. Each choice composes an independent era manifest using shared source-qualified identities. Across the five plans there are 110 supplied social backgrounds, 68 overlays and 28 optional educational/contact group recipes. Historical saved-answer aliases remain; Modern and Middle-Earth retain their procedural workflows. An existing toolkit installation does not silently switch eras. The existing yes/no names, languages and heritage dimensions remain, without proficiency questions or new language/name choosers.

`CultureToolkitInstaller` evaluates the preserved procedural generators only in isolated InMemory source stages. It resolves existing identities before writes and uses the caller's transaction. The original live destructive naming helpers are not used for toolkit reconciliation. `SeederManagedRecord.SeedBaseline` stores the desired stock field baseline, never a builder's retained value. Unknown identities, deleted owned records and incompatible prog contracts are reported rather than guessed. An unresolved retained native binding keeps that ethnicity unavailable; the original source remains recoverable.

The configured mandatory free-skill prog grants fixed ethnic native languages, plus the supplied fixed cultural vernacular/education and explicit learned-background Literacy. Background values are the maximum applicable supplied factor times editable native base 200. Starting-value wrappers preserve the original zero-boost behavior for other skills and add the original `value(boosts) - value(0)` delta exactly once. Stock language caps receive a floor of 200; custom/shared caps remain authoritative and are reported. Stronger independent values remain the responsibility of the existing claims/value pipeline.

The consumer calls `ChargenSkillSelectionGroupSeeder.Upsert` with actual compiled `Boolean(Chargen)` eligibility objects. Optional bounds remain 0..1, with CountKnown and unique slot accounting. There is no new allocator, screen wrapper, claims format, cost model or builder command tree. A live check identified one adapter integration gap: generated templates passed no mandatory prog to the resolver. The existing adapter now resolves the configured prog through the same four-screen lookup used by chargen revalidation. Existing templates remain opted out. New language grants receive the eligible learner accent or an eligible native accent, while hand-authored values and selected accents remain intact.

Thirteen learned backgrounds grant Literacy. Writing knowledge also requires a selected language and the specifically authorised tradition. Community overrides and educational electives do not grant every compatible script. The old stock writing block is captured before acquisition references change; it applies only outside toolkit scope. Custom text and edited blocks remain protected. Large generated prog bodies are split into compiled helpers below MySQL's UTF-8 TEXT limit; public contracts stay unchanged.

Targeted naming uses dedicated local name cultures and gender profiles, with the existing ethnicity-first resolution. A separate shared neutral repertoire does not dilute male/female suggestions. All delivered source forms, lemmas, dates, source IDs and evidence classes remain in the checksum-covered targeted corpus and research files, linked through stable repertoire identities. No new per-entry research or historical citation was manufactured. Actual NameCulture/PersonalName/NamePicker tests cover compounds, empty bynames, six NameStyles, regex/XML round-trips and case preservation. NFC/Latin-1 tests exercise real .NET fallback buffers and network output; unsupported scalars remain visible diagnostics instead of silent question marks.

## Source preservation and editorial changes

[Original corpus inventory](../Seeding/CultureSeederOriginalCorpus/README.md) captures all 28 pre-refactor source files byte-for-byte and seven generated source packs. The source-pack-local totals are **362 profiles, 53,216 name elements and 2,313 accents**. The four historical staging modules contain 236 profile references which resolve to **206 distinct profiles, 35,513 elements and 105 naming structures**. These are different scopes and must not be added together.

The [preservation receipt](./CultureSeeder_Redesign_Source_Preservation.json) verifies the original files are recoverable and all seven generated pack checksums remain exact. Only the original `CultureSeeder.cs` entrypoint changed; the retained source methods remain. The [retained naming import](./CultureSeeder_Redesign_Retained_Naming_Import.json) verifies actual C# persistence and rerun against the archived profile payloads. Naming definitions differ only by the explicit presentation ledger; regex, styles and element rules remain preserved.

`DatabaseSeeder/Seeders/CultureToolkit/ReviewedSourceProse.json` records 62 source-qualified editorial changes with original and replacement text. It removes retrospective phrasing and seeder/research commentary from active accents/name guidance and rewrites four retained Greek blurbs in contemporary voice while preserving their source details. The original generator links and name-morphology advice remain in the archive rather than being newly recommended to players. It does not add new attestation. Research caveats stay in the handoff and receipts rather than player descriptions.

## Verification boundaries

The latest completed full fast checkpoint passed **4,598 tests**: library 488, expressions 23, seeder 836, core 3,055, persistence 43, Discord 22, converter 43, website 54 and terrain 34. Command: `scripts/test-unit.ps1`. After the final source-prose and baseline refinements, the complete seeder suite passed again (836/836). Debug and Release builds passed; actual PE metadata confirms the replay-profile type is present only in Debug. Focused counts overlap the full run and are not additive.

- [Installer fixtures](./CultureSeeder_Redesign_Installer_Fixtures.json): five eras in both seeder orders, actual `CultureSeeder.SeedData` followed by installer rerun. IDs are isolated InMemory IDs. All ten scenarios passed with stable counts and zero conflicts.
- [Optional fixtures](./CultureSeeder_Redesign_Optional_Fixtures.json): seven partial flag combinations per era, with full-content combinations covered above. Checks include rerun counts, dangling references, skipped-content reporting and overlay availability. All 35 scenarios passed, with stable reruns and no dangling references or false skipped-content claims.
- [Legacy upgrade fixture](./CultureSeeder_Redesign_Upgrade_Fixture.json): retained pre-refactor combined Dark Ages/Medieval source followed by the new Medieval installer. Preserved ambiguities are reported; absent unbaselined directed edges are not silently re-created as though deletion were impossible. The completed upgrade and rerun passed with seven preserved absent-edge ambiguities explicitly listed in its receipt.
- [Language import](./CultureSeeder_Redesign_Language_Import.json): all five C# archive-driven language/script/directed-edge imports and reruns. Actual one-way engine-language tests prove no reciprocal or transitive inference.
- Migration `20260907235156_SeederManagedRecordBaselines` was generated using EF. The model/snapshot checks passed. The bundled blank snapshot was refreshed through actual local MySQL migrations in the verified-new disposable database `futuremud_culture_snapshot_2145_20260908`. This was an export, not a separate SQL snapshot import.
- [Live MySQL import](./CultureSeeder_Redesign_Live_MySQL.json): actual migrations and stock CoreData, Time, Attribute, SkillPackage, Human, Chargen and Medieval Culture seeding into `futuremud_culture_live_2145_m01`, then a committed Culture rerun with stable counts. The culture attempt initially hit TEXT capacity; its transaction rolled back, the completed prerequisites remained, and the corrected Culture import resumed successfully. The separately retained migration-failure receipt names an earlier too-long database name that failed before content import.
- [Live identities](./CultureSeeder_Redesign_Live_Identities.json): read-only actual MySQL managed IDs, prog names/signatures and hashes. This query is not a compiler test.
- [Telnet/editor transcript](./CultureSeeder_Redesign_Telnet.txt): actual `chargenskillgroup show`, native-base prog execution and editor compilation at 200, 240 and back to 200. The first short session ended before the final restoration was saved. Later restart checks in [native 200](./CultureSeeder_Redesign_Native_200_Telnet.txt) verify the persisted restoration.
- [Native character pipeline](./CultureSeeder_Redesign_Native_200_Telnet.txt): a real generated Welsh NPC with English Nobility shows **Welsh 200/200, Middle English 180/200, Anglo-Norman French 150/200 and Latin 50/200** after restart. The earlier no-skill fixture is retained in the diagnostic transcripts; it was generated before the mandatory-prog adapter fix. This is NPC generation through the existing chargen evaluation/cap paths, not a claim of completing every player chargen screen over telnet.

No five-era live MySQL sweep, separate blank-SQL import, complete all-seeder Debug replay, full player chargen telnet walkthrough, climate suite or production deployment is claimed. Both-order/four-screen behavior, accounting, stronger independent values, asymmetric intelligibility and parser/encoding behavior have C# coverage. The live content import uses the typed profile's selected stock prerequisite steps through the shared executor, not a fabricated all-profile replay result.

The [test receipt](./CultureSeeder_Redesign_Tests.json) records exact commands, suite totals and explicitly unexecuted checks.

## Compiled prog identities

These are actual Medieval fixture IDs in `futuremud_culture_live_2145_m01`. All exact contracts reject `AcceptsAnyParameters`; generated IDs in other fixtures differ.

| ID | Name | Contract |
|---:|---|---|
| 54 | ChargenFreeSkills | Trait Collection(Toon ch) |
| 57 | ChargenFreeKnowledges | Knowledge Collection(Chargen ch) |
| 306 | CultureLanguageNativeBase | Number() |
| 307 | CultureLanguageBackgroundFactor | Number(Toon ch, Trait trait) |
| 308 | CultureFixedSkills | Trait Collection(Toon ch) |
| 309 | CultureLanguageStartingValue198 | Number(Toon ch, Trait trait, Number boosts) |
| 331 | CultureWritingPolicyApplies | Boolean(Chargen ch) |
| 332ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œ334 | CultureWritingKnowledgesPart1ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œ3 | Knowledge Collection(Chargen ch) |
| 335 | CultureWritingKnowledges | Knowledge Collection(Chargen ch) |

Script acquisition helpers are exactly Boolean(Chargen ch, Trait skill); group eligibility and non-native-accent helpers are Boolean(Chargen ch). The final generated character (#4) retains Welsh native accent #1412 and the original learner accents for its three acquired languages, as recorded in the persisted live receipt. All 140 live managed/integration prog identities and 3,975 managed entity bindings are enumerated in the live identity receipt. The installed group receipt gives each stable key, group ID, compiled eligibility ID, candidate trait IDs and omissions. Directed-edge receipts give listener and target keys/IDs, difficulty and source policy; no shared-script or transitive edges are inferred.

The [content receipt](./CultureSeeder_Redesign_Content_Receipt.md) tabulates naming counts by gender and era, every excluded retained native identity and omitted group candidate.

## Acceptance evidence map

| Criteria | Evidence |
|---|---|
| CF01, CF04 | Five era/order fixtures, actual resolved native IDs and explicit unavailable retained bindings |
| CF02 | Byte-exact source archive, generated inventory/checksums, retained C# naming and language imports, prose change ledger |
| CF03 | Three-way scalar/member/prog tests, PR730 group override tests, rerun fixtures and legacy-upgrade preservation receipt |
| CF05ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œCF07 | Actual compiled starting progs, live Welsh/English Nobility character values, lower-cap and independent-value regressions, original boost accounting |
| CF08ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œCF09 | All 28 consumer recipes; SG01ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œSG20 dependency regressions, four-screen tests and both seeder orders |
| CF10ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œCF12 | Compiled writing/acquisition helpers, old-block exclusion tests, owned-reference reconciliation and directed runtime/receipt checks |
| CF13 | Supplied prose policy, source-qualified 62-change ledger, active accent Latin-1 corpus checks |
| CF14ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œCF16 | Scoped repertoires, preserved exclusions, actual parser/NamePicker/NameStyle and .NET/network fallback tests |
| CF17 | 35 optional flag fixtures plus 10 full-content/order fixtures |
| CF18 | Default-off and deterministic adapter regressions, authoritative independent values, live opt-in generated NPC |
| CF19ÃƒÆ’Ã‚Â¢ÃƒÂ¢Ã¢â‚¬Å¡Ã‚Â¬ÃƒÂ¢Ã¢â€šÂ¬Ã…â€œCF20 | Separate C#, handoff, migration, MySQL and telnet receipts; final commit and unexecuted checks reported explicitly |

## Research exclusions

The feminine Old Prussian replacement remains inactive. Latvian 14, Estonian 10 and Romanian 17 are disclosed small local feminine selections across the delivered scopes; individual era filters can produce smaller selections. Documentary, editorial, dynastic, devotional and literary evidence remain separate. A saint's life date is not treated as a vernacular spelling date. Earlier unsupported replacements retain their original source-specific naming, and profiles outside these targeted changes remain preserved rather than newly certified. There is no unrelated filler, adopted-name selector or invented productive surname morphology.

The installer receipts enumerate every genuine unresolved retained native crosswalk. Those records remain unavailable; they are not replaced with modern peoplehoods or language stages. A missing approved binding is distinct from the former missing-JSON transfer problem, which is resolved by the successful 26-file check.
