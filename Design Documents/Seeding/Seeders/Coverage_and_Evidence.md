# Coverage and reviewed evidence

## Baseline and review method

Published master `39eb9d32d2012f56ac3ef33c441b5dac7c3535e3` was verified with `git ls-remote origin refs/heads/master` on 2026-10-06, then used for an isolated documentation worktree. No later source deltas were reconciled. A local documentation commit contains these guides; its parent remains this exact revision. Runtime source, content assets and databases were not changed.

Pass one used the supported economical mapping option **`gpt-6-luna`**. No `Luna 6.1` identifier was advertised. The coordinator synthesized the guides, and two bounded independent **`gpt-6.1-sol`** reviews checked the 15 foundation guides plus Item/shared docs, then the other 18 concrete packages. No Astra was selected. Review was representative source tracing, not exhaustive inspection of every row or historical/balance calibration.

Every concrete package below had an existing named entry or actual operator-input path located and followed to its EF write/helper chain. Review corrected inaccurate helper names, identity values, generated-source seams, question lists and rerun admission instructions. Source-backed facts and static deductions are distinguished below. Add-one instructions are **unexecuted** maintainer recipes, including illustrative names/physical values; they do not admit new content. Disabled source paths are not installation receipts.

## Concrete-seeder coverage matrix

The linked guides identify exact source files/symbols, prerequisites, choices and failure/ownership boundaries. “Reviewed” means static source review of the representative route, not a passed test or verified gameplay.

| Guide / source entrypoint | Representative | Reviewed transform/write boundary |
| --- | --- | --- |
| [AIStorytellerSeeder](AIStorytellerSeeder.md) · [source](../../../DatabaseSeeder/Seeders/AIStorytellerSeeder/AIStorytellerSeeder.cs) | AI Storyteller Primer | Inline prose → EnsureReferenceDocument → AIStorytellerReferenceDocuments; named metadata/content overwritten. |
| [AgricultureSeeder](AgricultureSeeder.md) · [source](../../../DatabaseSeeder/Seeders/AgricultureSeeder/AgricultureSeeder.cs) | Wheat | CropSeed / Yield → EnsureCrop → AgricultureCropDefinitions XML; no outer transaction. |
| [AnimalButcherySeeder](AnimalButcherySeeder.md) · [source](../../../DatabaseSeeder/Seeders/AnimalButcherySeeder/AnimalButcherySeeder.cs) | global:offal | BuildGlobalItems → EnsureItems → EnsureProducts → EnsureProfiles → race assignments. |
| [AnimalSeeder](AnimalSeeder.md) · [source](../../../DatabaseSeeder/Seeders/AnimalSeeder/AnimalSeeder.cs) | Ant | GetInsectRaceTemplates → SeedInsectoid → SeedAnimalRaces → AddRace and supporting joins. |
| [ArenaSeeder](ArenaSeeder.md) · [source](../../../DatabaseSeeder/Seeders/ArenaSeeder/ArenaSeeder.cs) | Duel | EnsureEventType → sides and allowed classes → ArenaEventTypes graph; arena-scoped names. |
| [ArmageddonMagicSeeder](ArmageddonMagicSeeder.md) · [source](../../../DatabaseSeeder/Seeders/ArmageddonMagicSeeder/ArmageddonMagicSeeder.cs) | Mend Flesh | Shared factory → ArmageddonMagicInstaller.Content → contributions/WriteContent → MagicSpells, expressions and provenance. Debug/prepared-only. |
| [AttributeSeeder](AttributeSeeder.md) · [source](../../../DatabaseSeeder/Seeders/AttributeSeeder/AttributeSeeder.cs) | SOI Strength | Selected initial shape → TraitDefinition Type 1 and linked decorator/improver. Existing-foundation branch does not create new traits. |
| [CelestialSeeder](CelestialSeeder.md) · [source](../../../DatabaseSeeder/Seeders/CelestialSeeder/CelestialSeeder.cs) | Sun; authored RailSun | Physical XML / authored preset compilation → Celestials; separate identity/attachment behavior. |
| [ChargenSeeder](ChargenSeeder.md) · [source](../../../DatabaseSeeder/Seeders/ChargenSeeder/ChargenSeeder.cs) | Welcome | AddStage → canonical storyboard/dependencies; duplicates removed, nonempty XML and differing nonblank type retained. |
| [ClanSeeder](ClanSeeder.md) · [source](../../../DatabaseSeeder/Seeders/ClanSeeder/ClanSeeder.cs) | Naval Organisation Template / Seaman Recruit | Templates partial → CreateTemplateClan / AddRank → Clans, Ranks, abbreviations/titles. |
| [CombatSeeder](CombatSeeder.md) · [source](../../../DatabaseSeeder/Seeders/CombatSeeder/CombatSeeder.cs) | 9mm Pistol / Pistol_9mm | SeedDataGuns → ranged type and component; existing-world EnsureModernFirearmSamples is distinct. |
| [CookingSeeder](CookingSeeder.md) · [source](../../../DatabaseSeeder/Seeders/CookingSeeder/CookingSeeder.cs) | baked apple / bake apple | PreparedFood component → EnsureItem → EnsureBakedAppleCraft → item/craft/phase/input/product graph. |
| [CoreDataSeeder](CoreDataSeeder.md) · [source](../../../DatabaseSeeder/Seeders/CoreDataSeeder/CoreDataSeeder.cs) | silver | Inner SeedMaterialsBase AddMaterial → Materials / MaterialsTags. Outer helper has a distinct existing-name policy. |
| [CultureSeeder](CultureSeeder.md) · [source](../../../DatabaseSeeder/Seeders/CultureSeeder/CultureSeeder.cs) | legacy English; historical Latin | Legacy AddLanguage/Accent graph; toolkit embedded JSON/bindings → writer-managed cap/trait/language/accent graph. |
| [CurrencySeeder](CurrencySeeder.md) · [source](../../../DatabaseSeeder/Seeders/CurrencySeeder/CurrencySeeder.cs) | Dollars / penny | SeedDollars → currency, divisions/abbreviations, pattern/element graph and coins. Same-package duplicate safety is not established. |
| [EconomySeeder](EconomySeeder.md) · [source](../../../DatabaseSeeder/Seeders/EconomySeeder/EconomySeeder.cs) | Staple Food | Useful Market tag leaf → EnsureMarketCategories → MarketCategory XML; later family combination pass differs. |
| [EnvironmentalExposureSeeder](EnvironmentalExposureSeeder.md) · [source](../../../DatabaseSeeder/Seeders/EnvironmentalExposureSeeder/EnvironmentalExposureSeeder.cs) | lava | Profiles → classified material response → stable reaction XML keys → ReconcileRules → liquid/gas overlay. |
| [HealthSeeder](HealthSeeder.md) · [source](../../../DatabaseSeeder/Seeders/HealthSeeder/HealthSeeder.cs) | Hasty Triage | Selected surgery helper → AddSurgicalProcedure → named procedure/phase replacement; addition requires explicit new identity. |
| [HumanSeeder](HumanSeeder.md) · [source](../../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.cs) | abdomen | SetupBodyparts → CreateBodypart → BodypartProtos. Existing Humanoid refresh bypasses initial anatomy. |
| [ItemSeeder](ItemSeeder.md) · [source](../../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.cs) | bronze hand saw and path matrix below | Inline/generated/TSV/graph sources → preflight → ownership helpers → item/craft/outfit/skin/vehicle graphs. |
| [LawSeeder](LawSeeder.md) · [source](../../../DatabaseSeeder/Seeders/LawSeeder/LawSeeder.cs) | Immune | Selected authority → SetupClasses → authority-scoped membership prog and LegalClass. |
| [MythicalAnimalSeeder](MythicalAnimalSeeder.md) · [source](../../../DatabaseSeeder/Seeders/MythicalAnimalSeeder/MythicalAnimalSeeder.cs) | Giant Ant | Missing-race creation → SeedRace/support graph; existing defaults, breathing/mobility/diet and combat also refresh. |
| [PrimaryProductionSeeder](PrimaryProductionSeeder.md) · [source](../../../DatabaseSeeder/Seeders/PrimaryProductionSeeder/PrimaryProductionSeeder.cs) | Prospect for Iron Deposits | Projects → EnsureProject → phase/labour/material/actions and ResourceDiscoveryDefinition; disabled source emission only. |
| [PsionicsSeeder](PsionicsSeeder.md) · [source](../../../DatabaseSeeder/Seeders/PsionicsSeeder/PsionicsSeeder.cs) | connectmind / contact | Shared stock content → MagicPower XML → capability links. Existing-capability additions have explicit limited admission. |
| [RobotSeeder](RobotSeeder.md) · [source](../../../DatabaseSeeder/Seeders/RobotSeeder/RobotSeeder.cs) | Robot Humanoid | Robot body key / Humanoid parent binding → SeedRaces → race/support graph; missing body can skip a template. |
| [SkillPackageSeeder](SkillPackageSeeder.md) · [source](../../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs) | Swimming | SkillDetails → selected name/cap model → trait expression/definition and optional helpfile. |
| [SkillSeeder](SkillSeeder.md) · [source](../../../DatabaseSeeder/Seeders/SkillSeeder/SkillSeeder.cs) | installer-entered example name | Nonblank CSV → normalized name/cap → EnsureSkillDefinition; no source stock-name catalogue. |
| [StockMeritsSeeder](StockMeritsSeeder.md) · [source](../../../DatabaseSeeder/Seeders/StockMeritsSeeder/StockMeritsSeeder.cs) | Steady Presence | Blueprint/effect factory → EnsureCharacterMerit → named Merit scalar/XML rewrite. |
| [SupernaturalSeeder](SupernaturalSeeder.md) · [source](../../../DatabaseSeeder/Seeders/SupernaturalSeeder/SupernaturalSeeder.cs) | Vampire | Template → SeedOrRefreshRace → race/needs/attribute/ethnicity/characteristic/attack/description graph. |
| [TimeSeeder](TimeSeeder.md) · [source](../../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.cs) | gregorian-us | SetupGregorian → serialized calendar with Gregorian leap rules → EnsureCalendar → clock/shard bindings. |
| [TrapSeeder](TrapSeeder.md) · [source](../../../DatabaseSeeder/Seeders/TrapSeeder/TrapSeeder.cs) | Tripwire Alarm | EnsureTemplates → Definition / components / payload → approved TrapTemplate; coarse prefix readiness. |
| [UsefulSeeder](UsefulSeeder.md) · [source](../../../DatabaseSeeder/Seeders/UsefulSeeder/UsefulSeeder.cs) | Container_Table | CreateContainer → CreateItemProto → AddGameItemComponent → approved component; existing rows returned unchanged. |
| [WeatherSeeder](WeatherSeeder.md) · [source](../../../DatabaseSeeder/Seeders/WeatherSeeder/WeatherSeeder.cs) | temperate_mid_winter and generated events | AddSeason / derived AddEvent → seasons/events/models/regional climates; staged saves, no outer transaction. |
| [WildlifeCatalogueSeeder](WildlifeCatalogueSeeder.md) · [source](../../../DatabaseSeeder/Seeders/WildlifeCatalogueSeeder/WildlifeCatalogueSeeder.cs) | Leopard → Wildlife - Tree Ambush Hunter | PredatorProfileFor → EnsureAnimalProfiles → named ArtificialIntelligence XML; shared controller identity. |

## Item architecture coverage

| Materially distinct path | Source-checked representative / authoring authority | Remaining boundary |
| --- | --- | --- |
| Antiquity inline | antiquity_bronze_hand_saw; household helper → CreateItem | Recipe/craft admission explicitly separate. |
| Medieval inline | medieval_locking_trade_clerk_coin_lockbox → CreateItem | Container component behavior belongs to Useful. |
| Shared new stock / aliases | powder horn → GunpowderSupport; CreatePreIndustrialAlias → stable stock and attribution | Source and alias identities remain distinct. |
| Renaissance / Early Modern ordinary inline | civic warrant / gazette → EraCatalogueItemSpec → SeedStraightforwardEraCatalogueItems | Selected-era dispatch and dependency preflight checked. |
| Renaissance generated household | Canonical tables + description overlay → Python parser → generated specs | Inputs are authored reference tables. |
| Early Modern generated household | tea caddy in Python LEGACY/forms/cultures → new_specs → Markdown and C# | Generated reference tables are outputs; intentional counts apply. |
| Generated military | Era table/profile parsing + supported-component ledger → specs / armour outfit integration | Unsupported dependencies remain filtered. |
| Generated jewellery / doors | Renaissance ring → Python construction/mappings → reference/CSV/C# outputs | Emitted reference rows are not authoring inputs. |
| Prepared food / intermediates / liquids | Shared hearth wheat loaf → embedded TSV → food component/item/craft; liquid reuse path inspected | Item helper direct writes may bypass customization preservation. |
| Medical / repair | REN cupping glass → generated TSV → 134+44 loader contracts → era specs | Early Modern includes Renaissance stock; counts and Health preflight apply. |
| Clothing / skins / outfits | fine pleated kalasiris → generated supplement → item/skin/outfit graph | Mixed physical-base and reference-table authorities. |
| Vehicles | Antiquity handcart factory → exterior/components/VehicleProto and child graph | Vehicle graph is not only an item row. |
| Industrial ordinary | tools-repair-machinery plain scrap → embedded 24-column loader → profile/admission filter → CreateItem | Price evidence is authoring input, not automatic currency conversion. |
| Industrial clothing development | All 13 related TSVs are header-only; loader/ownership helpers inspected | No existing garment persistence example exists at this revision. Graph recipe stops at documented development boundary. |
| Industrial food development | industrialised_food_staples_001 → validated concept/dependency/evidence graph → production-readiness rejection | DependencyReviewed concepts are not ProductionReady; no completed food-item emission to trace. |
| Modern / Nuclear / Information | Registry entries, aliases and later vehicle spec families inspected | Non-selectable ordinary eras; no activation inferred. |
| Black-powder update / manifest capture | Smaller SeedData scope, ownership inventory and capture rollback paths inspected | No capture or replay run; packaged manifest/fingerprint contract source-reviewed. |

The full worked recipes and source links live in [ItemSeeder](ItemSeeder.md), keeping one owning guide rather than competing era instructions.

## Observations for coordinator issue triage

These are observations, not repairs or newly filed issues. Prioritize them with relevant existing work and reproduce only in an authorized disposable fixture.

| Observation | Evidence level / source | Implication |
| --- | --- | --- |
| Industrial selection and food readiness disagree operationally | **Verified source:** selectable registry in [IndustrialisedArchitecture](../../../DatabaseSeeder/Seeders/ItemSeeder.IndustrialisedArchitecture.cs); EnsureProductionReadyForSeeding in [IndustrialisedFoodCatalogue](../../../DatabaseSeeder/Seeders/IndustrialisedFoodCatalogue.cs), called by item preflight | Selecting an era is insufficient evidence that the package can complete with current development data. No activation repair attempted. |
| Food writes after customized item lookup | **Static inference:** UpsertPreIndustrialFoodItem in [PreIndustrialFoodCatalogue partial](../../../DatabaseSeeder/Seeders/ItemSeeder/ItemSeeder.PreIndustrialFoodCatalogue.cs) reassigns fields/links after CreateItem may return a customized row | A targeted rerun/preservation test is needed to decide whether intended ownership protection is bypassed. |
| Root ItemSeeder helper files fall outside fingerprint globs | **Verified enumeration, inferred consequence:** ComputeSourceFingerprint in [ItemSeederManifest](../../../DatabaseSeeder/ItemSeederManifest.cs) scans ItemSeeder directory and root Industrialised*.cs | Some root ItemSeeder.*.cs edits may not invalidate a stale manifest fingerprint. |
| Metadata reconciliation names are broader than structural rerun paths | **Verified source:** Attribute ReconcileExistingAttributes and Human existing-Humanoid fast branch | New attributes/anatomy are not automatically installed by these reruns; documented recipes require matching fresh/upgrade verification. |
| Coarse readiness markers | **Verified source:** Trap Any stock prefix and Psionics Basic-school-only check | MayAlreadyBeInstalled is not a catalogue completeness proof; missing stock needs focused assessment. |
| Same-package Currency insertion | **Verified source:** SeedDollars directly adds Currency and children without package-name reconciliation | Additive means other packages can be selected; same-package duplicate safety is not a verified guarantee. |
| Whole-run transaction assumption is unsafe | **Verified source:** [SeederExecutionService](../../../DatabaseSeeder/SeederExecutionService.cs) saves answers after SeedData; Weather, Agriculture, Cooking, AnimalButchery and PrimaryProduction stage saves without an outer transaction | A later failure can leave earlier work, and committed content can outlive answer-memory failure. |
| Item component ownership wording is overly broad | **Verified source:** food and vehicle paths create component prototypes | Ordinary item consumption prose does not describe every ItemSeeder domain. |

## Verification receipt and remaining gaps

Static review covered all 34 concrete classes, the abstract skill support path, reflection/metadata/executor/answer memory, packaged resources and each Item architecture above. Link/heading, code-fence, Mermaid structural and diff-check results are recorded below after final validation.

No generators, manifest captures, .NET builds/tests, database reads/writes, migration execution, game servers or expensive qualification suites ran. Relative links were checked for target existence and Markdown heading fragments; Mermaid checks cover fence/header/edge structure only, **not diagram rendering**. Future addition instructions require their owning focused regressions and selected first-install/rerun/preservation checks. No assertion is made that every catalogue entry is correct or that installed gameplay works.

No production garment/food example can be fabricated for the development paths above. Independent review is representative: all law policies, economic lifecycle paths, body/anatomy/procedure variants, storyteller prompt fields and exact field-level customizations were not exhaustively audited. The guides defer detailed contracts to existing owning documents. Later baseline changes and live execution remain for a separately coordinated task.

Validation receipt (2026-10-06): **34/34 guides present**, **37 new Markdown files**, **644 relative links** and **35 heading fragments** resolved, **3 Mermaid fences** passed structural checks, with no control characters or unbalanced code fences. The 2 existing-document edits were diff-reviewed: the design index links this guide set, and the shared era architecture now distinguishes Industrial selectability from production readiness. Final Git diff checks apply to the documentation-only staged change; no executable validation was run. All concrete findings from the two review passes and focused rechecks were integrated by the coordinator.
