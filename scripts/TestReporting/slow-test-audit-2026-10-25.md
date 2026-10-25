# Slow-test audit and default selection - 25 October 2026

Baseline source for this change: `4b730b2c1cd12fd588336758538c824a24965f96`. Timing evidence: successful [Unit Tests run 39947698782](https://github.com/FutureMUD/FutureMUD/actions/runs/39947698782), source `8fe785bc1ad2837d7bf33845fea53d8f6d9490b1`, 23 October 2026. Both OS artifacts contain 11,833 case records. Linux has 39 cases above ten seconds; Windows has 33. The table groups DataRows by method and reports each method's slowest observed row. A dash means that method did not exceed ten seconds on that OS. These are earlier hosted observations, not measurements of the edited source.

## Decisions

No test is deleted and no production runtime behavior changes. Default selection filters `Integration`, `Benchmark`, `ClimateSeeder` and the pre-existing `PsychicBenchmark` category. Fast focused in-memory EF, source, serialization and boundary checks stay enabled. Real cross-module installer/stock matrices and complete provider models are integration checks even when their database is in memory or no server is contacted.

All stock support and prepared-world installer classes also opt in: their individual cases are mostly below ten seconds, but each repeatedly builds/installs cross-module graphs (100.9 s and 51.7 s total Windows case time respectively). The three four-era economy tests share one expensive fixture, so they move together. The reporter's real process runner, Git pipe and platform-wrapper classes move together; pure TRX, receipt and selection tests stay fast.

Celestial and psychic timing/large-registry measurement harnesses opt in as benchmarks, including the already-tagged PsychicBenchmark cases. Climate calibration was already outside the fast manifest; it now also has category protection for plain solution-wide runs, and its weekly schedule is removed. Manual climate and extended workflows retain explicit hosted entry points.

Ten seconds is a review threshold, not an environment-dependent pass/fail assertion. The CI summary lists new outliers for review. All consumers of the shared full MySQL model and historical whole-schema SQL generation move together; otherwise excluding one cold-start assertion merely transfers model setup to another case. Source/operation-only methods in the same files stay fast. Classifying complete EF model/query checks as integration preserves their real provider contracts; changing them into source-text checks or timing a warmed model would conceal their actual setup cost.

## Every observed method over ten seconds

| Method | Linux max (s) | Windows max (s) | Decision |
| --- | ---: | ---: | --- |
| `ArmageddonStockProgressionRuntimeTests.ApprovedGraph_ClassEnrolmentNaturalEdgesAndRestart_RetainGradeOneAndRawCaps` | 80.64 | 70.88 | Integration: exercises the complete stock acquisition graph and restart. Stop reconciling after acquisition completes; retain all four mode/tradition rows. |
| `ArmageddonStockSpellInstallerTests.CompleteInstall_MaterialisesStockAndPreservesInstallationAvailabilityAndRerun` | 65.14 | 42.26 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `PreIndustrialFoodCatalogueCraftSeedingTests.CatalogueCrafts_SeedTwiceWithoutDuplicateCraftsOrSelectorPrograms` | 41.92 | 41.31 | Integration: installs 547 crafts and 322 selector progs across three scopes, compiles all selectors, or repeats/repairs the full install. Preserve complete catalogue coverage. |
| `HistoricalClothingColourTests.AllVariableCapableInventoryRows_ResolveTheirActualStockChannelsAndCompleteProfiles` | 35.52 | 21.86 | Integration: seeds stock and exhaustively binds every variable-capable clothing row/profile. Keep all 364 rows; focused binding/validation tests remain fast. |
| `ArmageddonStockSpellInstallerTests.BoundConsumables_UseExactOwnedPrototypesPaidGradeAndOneUseLifecycle` | 31.33 | 23.08 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonStockSpellInstallerTests.Install_FixedDirectionCanonicalDefaultsStableChildrenAndClosedAdmissions` | 24.56 | 16.88 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `PreIndustrialFoodCatalogueCraftSeedingTests.CatalogueCrafts_RerunRepairsManagedSelectorPrograms` | 22.47 | 19.00 | Integration: installs 547 crafts and 322 selector progs across three scopes, compiles all selectors, or repeats/repairs the full install. Preserve complete catalogue coverage. |
| `Tests.EfTerrainPlannerDataSourceTests.AccountLookupFiltersBeforeProjectingThePlannerRecord` | 22.08 | 10.73 | Integration: constructs the production MySQL provider model and translates an actual query. No server connection, but whole-model/query integration cost warrants opt-in. |
| `ArmageddonStockSpellInstallerTests.Templars_FreshInstallCompilesProgsAndPreservesBuilderChangesOnRerun` | 21.86 | 18.62 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonStockSpellInstallerTests.Kindle_LegacyOwnedWeaponDefinitionReconcilesInPlace_AndRerunIsUnchanged` | 21.30 | 16.49 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonCatalogueInstallerTests.Composite_LaterFailureResumesAndPreservesBuilderMedicalAndControllerEdits` | 20.14 | 12.83 | Integration: real composite catalogue install/rerun or staged-failure resume. Preserve all mode/class combinations and persistence assertions. |
| `CelestialDefinitionModelTests.Definition_AllowsDenseAuthoredSources_WithoutChangingExistingEncoding` | 19.22 | 13.42 | Integration: builds the complete production EF design-time MySQL model for a mapping contract. Cold model cost is real; retain the assertion without faking the model. |
| `PsionicCombatSeederTests.Install_AdvancedSuiteCoversAllMechanics_WithoutBasicAccess` | 18.66 | 11.12 | Integration: install/rerun tests invoke the real PsionicsSeeder; pure stock cost/defense budget checks remain fast. |
| `ArmageddonStockSpellInstallerTests.RegionalLand_PresetsCoverTerrainFamiliesAndRerunPreservesOwnersAndBuilderEdits` | 16.28 | 13.10 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonCatalogueInstallerTests.Composite_CompleteFixedDirectionRetainsAllIdentitiesOnRerun` | 14.96 | 12.34 | Integration: real composite catalogue install/rerun or staged-failure resume. Preserve all mode/class combinations and persistence assertions. |
| `ArmageddonStockSpellInstallerTests.FixedConcealmentAndWaterBreathing_UseExactViewerAndWaterBindings` | 14.40 | 11.32 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonStockSpellInstallerTests.Rerun_PreservesBuilderFieldsAndExpressionIdentity` | 14.15 | 10.06 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `ArmageddonStockSpellInstallerTests.PrimitiveBindings_ExactNativeAttributesViewerPolicyAndVoidPayloadRemainDistinctFromCosts` | 13.73 | 11.54 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `MagicalSubstanceTests.IndependentSurfacePortions_BoundScheduledParents_AndWashingAllowsFreshExposure` | 13.55 | - | Integration after optimisation: reduce 1,000 exposures to MaximumParents + 1 (257), preserving cap rejection, washing, expiry and fresh exposure assertions. The local full run still measured 10.19 s, so this exhaustive production-cap scenario opts in; smaller exposure regressions stay fast. |
| `ArmageddonStockSpellInstallerTests.StoneSkin_OrdinaryDexterityFallbackIsExplicitAndRetainedAfterAgilityAppears` | 13.49 | 10.91 | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `EconomySeederTests.SeedData_LiterateHouseholds_IncludeWritingMaterialsNeeds` | 13.14 | - | Integration: all three readers of the shared four-era installed world opt in together; excluding just the first reader would transfer its startup cost to another test. |
| `ArmageddonStockSpellInstallerTests.FocusDevices_LockedTwelveHaveNativeRecipesRealAllowListsAndDisabledGates` | 12.76 | - | Integration: actual stock spell installation/rerun over the seeded prerequisite world. Cheap manifest and small snapshot fidelity contracts remain fast. |
| `PreIndustrialFoodCatalogueCraftSeedingTests.CatalogueCrafts_SeedAllSupportedScopesWithResolvableProductsAndSelectorIds` | 11.84 | 11.32 | Integration: installs 547 crafts and 322 selector progs across three scopes, compiles all selectors, or repeats/repairs the full install. Preserve complete catalogue coverage. |
| `IndustrialisedClothingManifestCaptureTests.NonemptyAuthoredGraph_CapturesItemsSkinsOutfitsAndCraftsWithoutInstalledPrerequisites` | 11.35 | - | Integration: CaptureManifest runs real SeedData across five era catalogues and fingerprints repository sources despite the small clothing overlay. |
| `Tests.GitProcessTests.InheritedPipeRejectsIncompleteEvidenceAndBoundsCleanup` | 10.59 | 10.67 | Integration: intentionally retains a descendant pipe through the ten-second cleanup budget. Keep real process/drain coverage and production deadlines unchanged. |
| `Tests.RunnerTests.IncompleteReportsAndZeroTestsNeverPass` | - | 10.38 | Integration: launches six real CLI/child-host scenarios including an intentional timeout. Keep all assertions; parser/receipt/selection unit tests stay fast. |

## Current catalogue workloads beyond the earlier CI baseline

Current-source inspection found newer exhaustive catalogue contracts absent from the successful 23 October run. These use real seeder capture/serialization paths and are explicitly Integration:

- `ItemSeederUnifiedCatalogueTests.FullCapture_ExportsAllAggregateFamiliesAndSupplementaryFields`: captures 43,825 definitions across 13 aggregate families.
- `FullCatalogue_AllPreindustrialSelectionsHaveIdenticalDefinitions`: performs separate captures and frozen-oracle comparisons for all 15 nonempty era combinations.
- `ItemSeederCompiledStockTests.IndustrialSelection_EveryTechnologyProfilePreservesFullDefinitions`: eight full technology-profile captures/comparisons.
- `ItemSeederCompiledInventoryTests.CompiledInventory_IsCachedAndMatchesTheAuthoritativeCapture`: rebuilds the entire derived inventory for parity with the compiled projection.
- `ItemSeederCompiledRecipeTests.CompiledRecipes_EveryChildSerializesLikeTheIndependentLegacyParser` and `CompiledRecipes_ValidateEverySelectedDefinitionAndPreserveFixedCounts`: qualify 49,623 native serializer children and/or validate/digest all 14,040 recipes.
- `SelectedEra_OutfitReferencesAreActuallySeeded` and `DocumentedOutfitManifests_AllItemReferencesHaveSeedSources`: real selected/full-era installations for outfit reference coverage.
- Full frozen source-graph comparisons and the Roslyn audit of every authored catalogue source file: exhaustive data qualification. Small literal-guard, representative writer, ownership, preservation, cache lookup, fingerprint and binding tests stay fast.

The newer contracts retain every case, definition, digest and assertion. Repeated full-world captures are intentional integration qualification, so they are not reduced to representative samples. Current catalogue case timings were unavailable in the first local run because the seeder host did not complete before its existing run deadline; no invented per-case measurements are supplied.

## Execution and verification

Use the [test execution reference](../../.codex/references/test-execution.md) for default, focused extended, direct dotnet and Visual Studio commands. The broad/core wrappers intersect user filters with the fast exclusions unless `-IncludeExtended` / `--include-extended` is supplied. The explicit climate wrapper enables extended selection automatically. Directory.Build.props selects the default runsettings for direct .NET and IDE test execution; an explicitly supplied runsettings or native VSTest filter can override that selection.

The first local default attempt, `20261025T024148Z-5a9e782aef2d`, reached its 45-minute end-to-end deadline. It built all ten projects (30m53s summed build time, including a 22m23s seeder/catalogue rebuild), then timed out in the seeder host after 842.4 seconds of seeder execution. It is **FAIL**, with incomplete counts (9,723 passed, 18 failed), no seeder TRX, and no final source fingerprint; source stability is unknown because capture was cancelled at the deadline. It is not a passing default gate or a complete seeder timing baseline. The newer exhaustive catalogue scenarios above were still selected in this initial attempt and now require opt-in.

This attempt caught an invalid XML comment in the new extended settings; the comment was corrected without changing its selection semantics. Its 16 sandbox website failures cleared on a direct outside-sandbox no-build rerun: all 54 cases passed. The untouched `RoomReferenceCorrectionTests.EffectFactories_CustomSerializer_RefusesAndBuiltInInventoryMatchesSource` failed with 267 registered factories versus 264 audited entries. Source comparison identifies `PsionicDisorientation`, `PsionicSensePresence` and `PsionicVanish` as absent from `MudsharpDatabaseLibrary/Database/RoomReferenceEffectTypes.cs`; changing that migration serializer safety inventory is deferred as an unrelated behavioral decision. The failing check remains enabled in the default pass.

Raw historical receipts are retained under `.artifacts/slow-test-audit/hosted-{linux,windows}/`; local receipts and logs are under `.artifacts/slow-test-audit/`. Final validation results follow below.

## Complete revised run and final follow-up

The revised outside-sandbox run `20261025T033149Z-dbb76936073e` completed in **560.49 seconds (9m20s)**, with 101 seconds of summed incremental builds and 649 seconds of summed test-host phases (hosts overlap). Source fingerprints match: `aadaa24fe676086e071608d533a825b1890c5e25cb739ef38f7fcf2a5b52bf56`. Counts are complete: **11,586 passed, 2 failed, 3 skipped**. All 8,381 selected core cases and all 54 website cases passed. The three Windows skips require native Unix filesystem permissions.

Two newly measured complete weather installer checks remained above ten seconds and now also require Integration opt-in:

| Method | Local seconds | Decision |
| --- | ---: | --- |
| `ArmageddonWeatherTests.Install_NineBlocksHeatingConnectedDryGraphs_AndPreservingPartialRecovery` | 70.63 | Real Time/Celestial/Weather/Armageddon install plus repeated repair/rerun and full connected climate graphs. Preserve all nine season blocks and recovery assertions. |
| `WeatherTimingTests.FreshInstall_NativeTablesAndPeriods_AreNotRetimedByReinstallOrHazards` | 32.43 | Full native weather installation and hazard/reinstall preservation. Keep pure periodic-curve/native-clock conversion and preflight checks fast. |

No other case exceeded ten seconds in that complete run. Its default seeder host completed in 456.84 seconds; the core host took 164.09 seconds. These measurements precede the two weather exclusions above and the fixture correction below; they are not presented as timings of those final edits.

The second failure was an incorrect scope in `ArmageddonAttributeAdaptationTests.ArmAttributeInstall_UsesFourExistingAttributesAndTheApprovedRawBudget`: it asserted every trait identity stayed unchanged, although Heritage intentionally installs language **skills**. The test now compares raw attribute (`Type == 1`) identities before/after. Its four-attribute, racial-link, budget and bonus assertions are retained. This is a test-only correction; no installer behavior or audited effect serializer inventory is changed. The known effect-inventory failure remains enabled.

## Final accepted verification

After the final two weather exclusions and raw-attribute fixture correction, the affected seeder project passed in run `20261025T034413Z-c5ed414323e4`: **1,967 passed, 0 failed, 3 Unix-permission skips**, with matching source fingerprints `b1216203e12afb82de095919a2fd0ba4432ff88d722b18ad5762ed36557899d2`. Build: 23.1 seconds; tests: **354.3 seconds (5m54s)**; end-to-end: 379.7 seconds. No case exceeded ten seconds. The slowest was `AttributeAdoption_PreservesLinksCreatedBeforeCollectionOwnership` at **4.80 seconds**. The corrected attribute test passed.

Combining this affected-suite rerun with the unchanged nine project results from the complete run gives the latest default selection **11,585 passed, 1 pre-existing effect-inventory failure, 3 skipped (11,589 total)**. This is an aggregate of the named runs, not a claim that one new ten-project run was performed after the final seeder-only edits. The effect-inventory failure remains visible in the default pass. Each latest selected case has native duration evidence and none exceeded ten seconds. No production source or migration safety policy was changed to conceal a failure.

Explicit opt-in and native settings were exercised separately on the final code:

| Check | Result |
| --- | --- |
| Direct `dotnet test` reporter project, no filter/override | 11 passed; integration cases excluded by the default runsettings |
| Direct reporter with `IncludeExtendedTests=true` | 45 passed, including the real subprocess/timeout/pipe/wrapper harnesses |
| Reduced full progression graph and cap-saturation workloads, explicit opt-in | 5 passed; all four progression DataRows plus saturation retained |
| Real stock spell prerequisite/install/rerun matrix, explicit opt-in | 4 passed; both progression modes and class choices retained |
| Actual Bash opt-in/filter forwarding | 3 passed; stable-source run `20261025T035454Z-5e50c8254a41`, `include_extended=true` |
| Climate build | Passed; four existing MSTEST0052 warnings |
| Climate discovery only | 0 default case names; 36 with explicit opt-in; no simulation execution |

Opt-in artifacts are under `.artifacts/slow-test-audit/opt-in/`. PowerShell parsing, Git Bash syntax, actionlint 1.7.12 for all three changed workflows, both runsettings/props XML, and CI timing summaries using real and synthetic receipts passed. The summary checks cover over-ten-second cases, absent/null timings, escaped test names and absent receipts. Final diff whitespace/path checks passed. Owned dotnet/testhost/Bash processes were stopped or completed; none remained at the end of verification.

The expensive full catalogue/era/profile matrices and climate simulations were deliberately not rerun in full. They retain their assertions and have explicit local/manual hosted entry points; their complete opt-in campaigns remain unverified in this task. No hosted Linux/Windows workflow was dispatched and no Visual Studio UI run was performed. Native Linux-only permission behavior remains unverified on this Windows host. The 22m23s clean seeder/catalogue compilation cost remains separate from test execution; these selection changes do not claim to solve that build cost. Broad Compact/Json extended runs can require an explicit larger `TimeoutSeconds`/`--timeout-seconds` budget than the default 30 minutes; Human mode and the manual extended workflow retain their own documented deadlines.

## Effect-inventory follow-up authorised for publication

The user subsequently requested repair of the remaining effect-inventory failure before publication. The three serializers and their shared base were audited: `PsionicDisorientation` saves Power/Penalty, `PsionicSensePresence` saves Power plus concentration/school state, and `PsionicVanish` adds only a distance-scope enum. Its observer cache is transient. None persists a historical containing-Room reference. The audited inventory now includes all three factories; unknown factories still refuse.

Three focused regression rows preserve their actual payload shapes unchanged both at the top level and nested under `MagicSpellParent`, with power/school IDs deliberately colliding with parent/playable Room IDs and formatting/CDATA retained. The owning migration design document records this disposition. Earlier failure reports above remain historical evidence, not the final state of this repair.

The repaired default persistence-library suite passed in run `20261025T043225Z-65d9050a9b10`: **217 passed, 0 failed, 0 skipped**, source stable (`e4598e1655061d4da983776e36d2baea42195a0e91d45d432e028b1a1dfe3ef8`), build 19.1 seconds, tests 11.9 seconds, total 33.4 seconds. No case exceeded ten seconds. The formerly failing source-inventory test and all three new preservation rows passed. Independent read-only serializer review found no defects. Combining the latest per-project checks gives **11,589 passed, 0 failed, 3 Windows-only Unix-permission skips (11,592 total)**; this is an aggregate of the identified full run and affected-project reruns. Hosted PR checks are assessed separately during publication.
