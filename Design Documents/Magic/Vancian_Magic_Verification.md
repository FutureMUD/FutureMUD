# Vancian magic verification and acceptance

Review remediation baseline: PR #745 head `fd90915ef318c2d24cfc89304ae037486e5d0779`, integrated locally with master `a48d9198` on `codex/pr745-review-fixes`. The following results are from the repaired combined tree on 12–13 September 2026 (Australia/Sydney). The seven existing `MagicSpellResolutionTests` and independent group-target loop are preserved. Local results do not establish hosted CI status.

## Delivered scope

The feature includes the configurable `vancian` capability, stable repertoire/allowance identities, canonical-owner Selected choices and permission/notification hooks, saved loadouts and independent last pattern, finite memorised/spontaneous and explicit at-will allowances, all three recovery modes, instance-owned spellbooks, timed paid transcription/inscription, prepaid single-use scrolls, numerical snapshots, stored-effect reload, typed FutureProg/craft entry points, and administrative diagnosis/repair.

Feature projects are FutureMUDLibrary, MudSharpCore, MudsharpDatabaseLibrary, DatabaseSeeder (installer snapshot), and their affected core/database/seeder test projects. Review repairs also preserve UTC register timestamps and classify spellbook/scroll prototypes as requiring authored world content in the catalogue audit. No production content, character grants, seeded classes or stock books/scrolls were installed. Live checks use uniquely named disposable databases; no existing game database was modified. There is no new dependency or target-framework change.

Public entry points are the school `vancian` branch, `spellbook`, `spellscroll`, `magic vancian` administration, spell level/scroll metadata and 27 typed function overloads. See the [player guide](Vancian_Magic_Player_Guide.md), [builder guide](Vancian_Magic_Builder_Guide.md) and [API contracts](Vancian_Magic_FutureProg.md). The implementation uses an XML aggregate per identity/capability and a separate durable operation journal rather than a table per selected spell/slot. Stable GUIDs distinguish rules, allowances, plans and charges; counters and versions are not aliases or shared caster-level values.

## Local verification

The actual toolchain is .NET SDK 10.0.401, net10.0 product/test targets, EF Core 9.0.11 and Pomelo 9.0.0. Windows builds and restores use one MSBuild node. Product builds use `--no-restore -m:1 -p:NoWarn=NU1902%3BNU1510`.

| Gate | Result |
| --- | --- |
| Engine Debug | Passed |
| Engine Release | Passed |
| DatabaseSeeder Debug | Passed |
| DatabaseSeeder Release | Passed |
| Expanded focused core filter: Vancian, MagicSpellResolutionTests, MagicEngineV4Tests.SpellAdapter, VariableRegister, GameItemComponentRegistrationAuditTests and DocumentationCatalogueExporterTests | 135 passed, zero failed/skipped |
| EF `migrations has-pending-model-changes --no-build` with database project and engine startup | Passed: no pending model changes |
| Standard `scripts/test-unit.ps1` | 5,387 passed, 13 failed, zero skipped; two component-registration/catalogue failures were subsequently repaired and verified in the focused and complete core reruns |
| Full core rerun after final command/allocation repairs | 3,292 passed, five failures outside the changed code, zero skipped; 3,297 total |
| Five remaining core failures run alone without Vancian tests | The same five failures reproduce; their source matches the integrated master |
| Focused installer snapshot tests after SQL encoding repair | Six passed, zero failed/skipped |
| Fresh MySQL snapshot import and upgrade from master | Both passed; Vancian delta applied twice without duplicate history or schema |
| Real MySQL ledger, charge and callback probes | Eight checks passed, including concurrent charge claims, stale aggregate writes, precommit recovery, postcommit no-refund and callback/register durability |
| Fresh stock world / engine startup | Medieval Debug replay completed in the disposable upgraded database; rebuilt server reached its ready marker and accepted telnet login |
| Real server process termination during inscription | Passed: MySQL retained the Reserved slot/item before restart; ordinary player access cancelled it once, restored that slot and left the scroll blank. A separately completed inscription remained charged with its slot spent. |

The automated tests use the real service, ordinary capability/spell/component constructors and loaders, actual trigger parsers and spell resolution, real inventory plans and the real FutureProg compiler. Boundaries use an injected clock, policy/check results, detached optimistic state store and item persistence callback. The memory store round-trips state XML on reads/commits. Callback regressions execute the real compiled register-setting program, save through the real SaveManager/VariableRegister and reload an independent EF context; failure injection covers both the save and completion-record boundaries for known changes, refresh and inscription. This automated coverage is distinct from the additional real MySQL probes described below.

The remaining core failures reproduce in isolation on unchanged test source. The first four were also reported by the original implementation; the naming fixture is present in the integrated master:

- `MudDateTimeTests.TimezoneBuilder_CanEditAliasNameAndOffset`: expected `Offset Two`, actual `OFfset TWo`.
- `WeatherControllerBuilderTests.ParseWeatherControllerCreationArguments_ZoneName_ResolvesZone`: expected `ZZZ Pilot Weather`, actual `ZZZ PIlot WEather`.
- `EmploymentCommandServiceTests.EmploymentCommandService_BoardPostAllowedWithAuthorityRecordsRegister`: expected `Stock`, actual output contains `STock`.
- `KnowledgePickerBySkillScreenTests.FormatAvailableKnowledges_LongKnowledgeDescriptions_WrapAsSeparateNumberedRows`: existing formatting assertion fails.
- `CultureToolkitNamingTests.NamePickerDisplay_ProfileWithoutOptionalBynamePool_DoesNotRequestMissingElement`: the fixture adds an ID-zero item to the guarded registry.

The seeder suite has 1,371 passes and six failures in current-base catalogue/outfit tests: `RichEntries_PersistReloadRerunAndUpdateWithoutDuplicates`, `LegacyOutfitFingerprint_UpgradesOnlyUnchangedDefaultComposition(False)`, `SourceFingerprint_TracksRelocatedPartialContents`, `RuntimeManifestLoad_RejectsOlderContractEvenWhenEntriesAreOtherwiseValid`, `MedievalClothingSeeder_ImplementsReferenceCatalogueInOrderWithAuthoritativeReuse`, and `ItemSeeder_AntiquityComponentGapItems_RerunDoesNotDuplicateAndUsesReportComponents`. These concern outfit colours/fingerprints, fixture directories and catalogue expectations, rather than Vancian state or snapshot import.

The other standard suites passed: library 505, expressions 23, database 47, Discord 22, converter 43, website 54 and terrain planner 34. The database suite includes the Vancian model/migration tests. A separate historical checkout was not built. No old regression test was removed, weakened or excluded. The dedicated climate simulation is outside this change's scope. Builds retain the repository's permitted local NU1902/NU1510 suppression; existing analyzer and nullable warnings remain.

The reviewed hosted merge had a missing `using System` in `ComputerMediaBuiltInApplicationExecutorTests`. That fix is present in the integrated master, and every local standard-suite project now builds. These repairs have not been pushed and no new GitHub Actions run is claimed.

## Review findings and direct regression coverage

`VR` means `VancianReviewRegressionTests`; `VP` means `VancianPersistenceReviewTests`. All listed cases passed in the expanded focused run.

| Review | Correction | Executable coverage |
| --- | --- | --- |
| R1 | Spell-backed powers require one complete paid Vancian route at the requested computed power when no legitimate legacy route exists. | `VR.SpellBackedPower_UsesOneFiniteCastingAndCannotIncreaseItsPowerOrReuseIt`; `VR.SpellBackedPower_UnpreparedBookFormulaAndAmbiguousAllowancesRefuse`; `VR.SpellBackedPower_LegitimateLegacyRouteStillUsesItsNormalCheckAndCosts` |
| R2 | Ordinary casting and scroll release permit conscious, focused combat actors; preparation and writing retain stationary restrictions. | `VR.Combat_AllOrdinaryAllowanceModesCastButTimedWorkRefuses` (memorised, spontaneous, at-will); `X.ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce` now uses a combat actor |
| R3 | Flush supported engine saves before acknowledging callback completion; failures remain quarantined, with no callback replay. Preserve timestamp kind/precision. | `VP.Callback_RealRegisterAndIndependentReloadAgreeWithDurableCompletion` (nine known/refresh/inscription success/failure cases); `VP.RegisterTimestamp_RoundTripPreservesUtcPrecisionAndLegacyValuesRemainUtc` |
| R4 | Lazy recovery cancels only journalled, abandoned Reserved writing and restores matching previous unspent slots/items once. | `VP.Restart_AbandonedPrecommitWritingReleasesExactSlotAndItemOnce` (inscription/transcription); `VP.Restart_PostcommitOrIndeterminateWritingNeverRefundsOrCreatesCharge` |
| R5 | Allocate alternative items/remaining quantities across combined plans, retaining the allocation for execution and protecting retained tools. | `VR.CombinedMaterials_TwoCostsConsumeTwoUnitsExactlyOnce` (distinct items/stack); `VR.CombinedMaterials_RetainedToolIsAllocatedSeparatelyFromConsumedItem`; existing `I.ProductionPlanDetectsSharedQuantityUnderpaymentBeforeAnyConsumption` |
| R6 | Valid retained book patterns remain part of information-only knowledge after every copy is spent. | `VR.RetainedBookPattern_RemainsKnownAfterLastCastButInvalidReferencesDoNot` |
| R7 | Capability-scoped help evaluates every available route through SpellNumericalContext; unavailable routes show unresolved costs. | `VR.SpellHelp_UsesTheSameRouteContextAsActualExpenditure` (upcast/at-will); `VR.SpellHelp_MultipleCapabilitiesShowIndependentCostsAndScopedHelpUsesSelectedCapability` |

Live non-admin acceptance exposed an additional command-tree defect: `spellbook` and `spellscroll` were defined in the admin-only MagicModule. Their sole command registrations now belong to the common InventoryModule, with the same service-backed handlers and help. `VR.WritingCommands_AreAvailableThroughRealPlayerAndNpcCommandTrees` checks player/NPC/guide/admin registration and executes both item-inspection commands through the actual player dispatcher. The allocation search also has a 10,000-choice limit to keep heavily overlapping impossible plans from monopolising the game loop; `VR.CombinedMaterials_HeavilyOverlappingImpossiblePlanRefusesWithoutSpending` verifies conservative refusal without consumption.

## Schema and live environment

EF generated `20260912061254_VancianMagic`, its designer and the model snapshot together. It adds `MagicSpells.SpellLevel` default zero and `ScrollInscriptionAllowed` default false; `CharacterMagicCapabilityStates` with a composite identity/capability key and concurrency version; and `VancianMagicOperations` with durable charge identities and item references. The operation table deliberately has no cascading foreign keys so consumption evidence outlives deleted items/owners. Deleting an actual character/capability record cascades its aggregate, while merely losing a capability grant does not.

The installer dump retains the EF-generated Vancian delta and the complete current-base schema. Live import identified and removed two inherited in-file byte-order-mark artefacts before migration transactions, including a mojibake marker; the snapshot regression rejects either form. The manifest and migration-history row match the Vancian migration. This is a verified update of the maintained dump, rather than a newly generated full export.

On MySQL 8.0.45 at local port 3307, the current dump imported successfully into `pr745_fresh_102bc3355850`. A second disposable database, `pr745_upgrade_aa82596fe626`, imported the integrated master dump (with only those encoding defects normalized) and applied the Vancian delta twice. Both have one Vancian migration-history row, the new spell columns and the current-base liquid freshness columns. The original implementation's connection failures are historical and do not describe this remediation run.

Additional opt-in probes use the real `VancianStateStore`, MySQL transactions, `SaveManager`, compiled callback and `VariableRegister`. Independent contexts reject a stale aggregate write; two concurrent charge claims produce one durable winner; a new service cancels Reserved work once and never refunds Committing work; callback completion and its cooldown survive independent database reload; and a failed callback save leaves a durable NeedsReview quarantine. Actor/world boundaries in these eight probes are mocked, with minimal disposable parent fixtures. The separate seeded-server process-termination check below uses real loaded characters and items. Neither establishes an atomic transaction across arbitrary inventory/effect mutations and SQL.

## Non-admin seeded-server acceptance

The Medieval Debug replay seeded a fresh world in the disposable upgraded database. Test-only authoring supplied the documented policy progs, a Review Arcane school, Wizard/Cleric/Sorcerer capabilities, a second Wizard with EveryRefresh, three ordinary self-targeted stamina spells, and instance books/blank scrolls. No selected state, prepared slots or charges were inserted by the fixture. The gameplay sessions used the real Player command tree and account authority, with the admin avatar flag disabled; the fixture account's name remained Admin. The server listened only on loopback port 44545.

Preparation and sleep qualification used shortened two/three-second durations, and writing normally took three seconds. The process-termination case used a 30-second inscription so the saved Reserved boundary could be verified before restarting. To test loss of source access, only the two fixture books' placement links were withdrawn while the server was stopped; their formulae and all capability state were retained. A separate empty book was placed for the subsequent transcription test.

| Player scenario | Observed result |
| --- | --- |
| Selected policies and callback persistence | Selection committed through the documented hook; the UTC register timestamp survived fresh processes. An unchanged draft commit explicitly skipped its hook, and the independent MySQL value remained identical. |
| Memorised copies and expenditure | Two Ember Bolt copies could be spent separately. Prepared displays retained each spent position and the independent last pattern across reconnects/restarts. |
| Finite cantrip versus at-will | First-level Spark used a finite copy at Strong power; subsequent at-will Spark remained level zero / Standard. |
| Sleep recovery | Observed sleep followed by waking automatically created two Sorcerer slots. Cleric required preparation after sleep. Both spent exactly two castings; another Sorcerer cast was refused. |
| Book copying | A borrowed source book remained intact; its high-level formula was added to another instance after timed copying. A later charged-scroll transcription consumed its source exactly once. |
| Retained knowledge and both book policies | After all Greater Ember copies were spent and both source books became inaccessible, ordinary school spell help still found it. PatternChangesOnly restored the identical last pattern; changed book subpatterns were refused. EveryRefresh refused both identical and changed plans without the required formulae. |
| Finite and at-will inscription | Finite first/fifth-level and at-will production created the expected single stored charges; finite slots became Spent, and inscription did not release the spell. |
| Player cancellation | Cancelling an in-progress at-will inscription stopped its action and left the same scroll blank. |
| Unknown high-level scroll transcription | A Cleric whose maximum allowance was level one and whose personal slots were spent copied Greater Ember (level five) into an empty book. The source scroll disappeared without activation or a control check. |
| Scroll release with no personal slots | A Sorcerer with both slots spent released stored Spark. Separate upcast scrolls read through the weaker Sorcerer/Cleric routes failed their control checks, were consumed, and could not be used again. |
| Precommit process termination | MySQL held operation `150a880c-9a97-4aec-b6ec-bbdfe8cd20d1` as Reserved with matching scroll reservation after forced process termination. Normal player access in a new process cancelled it, restored only its previous prepared slot and left the scroll blank. Repeated access made no further change. |
| Completed work across restart | A separately completed inscription retained its stored charge and spent slot. Subsequent successful transcription consumed that charge rather than refunding the original casting. |

The live spells used empty material plans and no resource cost so these sessions do not establish live inventory-payment atomicity, measured damage/healing potency or reader attribution after changing/removing the creator. Combat casting, group resistance/wards, creator-independent numerical effects, and combined real inventory-plan consumption have direct automated integration coverage, but were not all repeated in this telnet world. The complete extended gameplay matrix is therefore not claimed as passed. The [builder walkthrough](Vancian_Magic_Builder_Guide.md) and [player guide](Vancian_Magic_Player_Guide.md) retain the full acceptance sequence.

After acceptance, the test server was stopped and both uniquely named disposable databases were removed. Local redacted transcripts and probe output remain in the ignored `MudSharpCore/bin/pr745-review` scratch directory. All 24 local links across the seven Vancian documents and the final diff whitespace check pass.

## Approved decision crosswalk

The 24 section-2 decisions are covered by the section-20 tests below.

| Decision | Verification IDs |
| --- | --- |
| VAN-01 | CAP-01–CAP-03 |
| VAN-02 | KNW-01, KNW-04, CAP-06 |
| VAN-03 | KNW-02–KNW-07 |
| VAN-04 | KNW-01, KNW-05 |
| VAN-05 | MEM-01–MEM-03 |
| VAN-06 | MEM-01–MEM-02 |
| VAN-07 | MEM-04–MEM-07 |
| VAN-08 | BOK-01–BOK-02 |
| VAN-09 | BOK-03–BOK-06 |
| VAN-10 | CAP-05 |
| VAN-11 | CAP-04, REF-07 |
| VAN-12 | CAP-02, MEM-03, POW-02 |
| VAN-13 | POW-01, POW-03 |
| VAN-14 | POW-02, INS-02 |
| VAN-15 | INS-01–INS-07 |
| VAN-16 | SCR-01, SCR-06 |
| VAN-17 | SCR-02–SCR-05 |
| VAN-18 | SCR-08–SCR-16 |
| VAN-19 | SCR-05–SCR-07, SCR-17, PER-02–PER-03 |
| VAN-20 | INS-02, INS-08 |
| VAN-21 | CAP-06–CAP-08, REF-04–REF-07, PER-04 |
| VAN-22 | INT-01–INT-03 |
| VAN-23 | POW-04, INT-05 |
| VAN-24 | CAP-04, REF-01, DOC-01–DOC-02 |

## Requirement-to-test inventory

The following maps every section-20 ID to executable constituent coverage. A mapping is not a claim that mocked boundaries ran against MySQL or that a complete interactive walkthrough occurred. Explicit live limits remain under PER-05 and DOC-02. Data-driven methods cover multiple cases.

Class abbreviations (all core tests unless stated):

| Key | Test class |
| --- | --- |
| C | VancianCapabilityTests |
| S | VancianStateTests |
| R | VancianRecoveryTests |
| N | VancianSnapshotTests |
| E | VancianEffectTests |
| I | VancianItemTests |
| X | VancianIntegrationTests |
| P | VancianExampleProgTests |
| D | VancianMagicModelTests in MudsharpDatabaseLibrary Unit Tests |

### Capability, access and selections

| ID | Executable coverage | Result / limit |
| --- | --- | --- |
| CAP-01 | `C.RegisteredCapabilityRoundTripsClonesFreshKeysAndKeepsLegacyLoader` | Passed in local runtime/model tests. |
| CAP-02 | `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies` | Passed in local runtime/model tests. |
| CAP-03 | `C.UnsupportedBookModesBrokenLinksAndBadSchemaDisableWithoutOverwriting` | Passed in local runtime/model tests. |
| CAP-04 | `C.EveryPolicyContractRejectsWrongReturnAndParameterTypes`; `C.InvalidProgressionNeverGrantsCapacity`; `C.RecursiveCandidatePolicyFailsClosedAndCanRecoverAfterEditing` | Passed in local runtime/model tests. |
| CAP-05 | `D.UpgradeDefaultsDoNotGrantKnowledgeOrScrollEligibility`; `S.AtWillAndSpontaneous_AreDifferentAndLevelZeroDoesNotGrantItself` | Passed in local runtime/model tests. |
| CAP-06 | `R.IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants`; `X.CapabilityIdentitiesHaveSeparateSelectionsAndGenerationEvenWithIdenticalRuleKeys` | Passed in local runtime/model tests. |
| CAP-07 | `C.RegisteredCapabilityRoundTripsClonesFreshKeysAndKeepsLegacyLoader`; `S.PreparedCasting_SurvivesSelectionAndBookLoss_ButNotStructuralEdit` | Passed in local runtime/model tests. |
| CAP-08 | `S.CapacityDropAndReturn_DoesNotEraseSpentStateOrMintNewSlots` | Passed in local runtime/model tests. |
| KNW-01 | `S.Queries_DoNotCreateCapacityOrPersistence`; `S.WholeKnownCommit_NoOpReorderingSkipsHooks_StaleDraftRefuses`; `S.CrossBucketTransfer_IsARealChangeEvenWhenAggregateSpellsMatch` | Passed in local runtime/model tests. |
| KNW-02 | `X.FixedRepertoireRequiresStaffAndRejectsInvalidWholeCapabilityChangesAtomically` | Passed in local runtime/model tests. |
| KNW-03 | `R.WholeCapabilityHooksUseImmutableSnapshotsAndCanonicalOwnerExactlyOnce` | Passed in local runtime/model tests. |
| KNW-04 | `S.WholeKnownCommit_NoOpReorderingSkipsHooks_StaleDraftRefuses`; `S.CrossBucketTransfer_IsARealChangeEvenWhenAggregateSpellsMatch` | Passed in local runtime/model tests. |
| KNW-05 | `X.FixedRepertoireRequiresStaffAndRejectsInvalidWholeCapabilityChangesAtomically`; `S.WholeKnownCommit_NoOpReorderingSkipsHooks_StaleDraftRefuses` | Passed in local runtime/model tests. |
| KNW-06 | `P.EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; `R.IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants`; `R.WholeCapabilityHooksUseImmutableSnapshotsAndCanonicalOwnerExactlyOnce` | Passed in local runtime/model tests. |
| KNW-07 | `S.KnownCallbackFailure_CommitsOnceAndQuarantinesFurtherChanges`; `S.PersistedIndeterminateCallbackRemainsBlockedBeyondDisplayHistoryWithoutReplay` | Passed in local runtime/model tests. |
| KNW-08 | `S.PreparedCasting_SurvivesSelectionAndBookLoss_ButNotStructuralEdit`; `X.NonCasterAndRevokedPayloadRefuseButUnknownSpellWithAllSlotsSpentReleases` | Passed in local runtime/model tests. |

### Loadouts, recovery and power

| ID | Executable coverage | Result / limit |
| --- | --- | --- |
| MEM-01 | `S.PlansAndLastPattern_AreIndependentOfSpentSlotsAndDeletedNames`; `N.DirectVancianInvocationUsesSyntheticOutcomeAndExactFiniteDebit` | Passed in local runtime/model tests. |
| MEM-02 | `X.AllPlanEditsLeaveCurrentCopiesUntouchedAndEmptyPlanCreatesUnassignedSlots` | Passed in local runtime/model tests. |
| MEM-03 | `X.AllPlanEditsLeaveCurrentCopiesUntouchedAndEmptyPlanCreatesUnassignedSlots` | Passed in local runtime/model tests. |
| MEM-04 | `S.PatternChangesOnly_UsesMultisetPerRuleAndRequiresEveryFormulaAfterAChange`; `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies` | Passed in local runtime/model tests. |
| MEM-05 | `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies` | Passed in local runtime/model tests. |
| MEM-06 | `S.PatternChangesOnly_UsesMultisetPerRuleAndRequiresEveryFormulaAfterAChange`; `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies` | Passed in local runtime/model tests. |
| MEM-07 | `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies`; `I.UnusableOrFullBooksRefuseWithoutFormulaOrPayment` | Passed in local runtime/model tests. |
| REF-01 | `R.ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh`; `R.AutomaticWakeRefusalRetainsCreditForExplicitRetryWithoutHeartbeatSpam`; `R.AwakeProjectionInterruptsSleepImmediatelyAndIntervalsAreIdentityOwned`; `N.DirectVancianInvocationUsesSyntheticOutcomeAndExactFiniteDebit` | Passed in local runtime/model tests. |
| REF-02 | `R.ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh` | Passed in local runtime/model tests. |
| REF-03 | `S.SavedPlanChangeDuringPreparation_RefusesWithoutPartialRefresh`; `S.PatternChangesOnly_UsesMultisetPerRuleAndRequiresEveryFormulaAfterAChange` | Passed in local runtime/model tests. |
| REF-04 | `R.InvalidSleepAndOfflineGapsNeverEarnCredit`; `R.ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh` | Passed in local runtime/model tests. |
| REF-05 | `R.AwakeProjectionInterruptsSleepImmediatelyAndIntervalsAreIdentityOwned` | Passed in local runtime/model tests. |
| REF-06 | `R.AutomaticWakeRefusalRetainsCreditForExplicitRetryWithoutHeartbeatSpam`; `R.ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh` | Passed in local runtime/model tests. |
| REF-07 | `S.CapacityDropAndReturn_DoesNotEraseSpentStateOrMintNewSlots`; `R.IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants`; `X.CapabilityIdentitiesHaveSeparateSelectionsAndGenerationEvenWithIdenticalRuleKeys` | Passed in local runtime/model tests. |
| POW-01 | `S.PowerSaturatesWithoutClampingIntoTriggerRange`; `S.AtWillCeilingIsSpellSpecificAndFiniteCantripUpcastDoesNotAlterAtWillPower` | Passed in local runtime/model tests. |
| POW-02 | `S.AtWillCeilingIsSpellSpecificAndFiniteCantripUpcastDoesNotAlterAtWillPower`; `I.AtWillInscriptionCanStockpileWhileSpontaneousInscriptionDebitsChosenSlot` | Passed in local runtime/model tests. |
| POW-03 | `S.PowerSaturatesWithoutClampingIntoTriggerRange`; `N.LegacyDirectRouteCannotBypassVancian_AndMixedCapabilityRemainsValid` | Passed in local runtime/model tests. |
| POW-04 | `N.DirectVancianInvocationUsesSyntheticOutcomeAndExactFiniteDebit`; `X.ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce`; `MagicSpellResolutionTests` | Passed in local runtime/model tests. |

### Books and inscription

| ID | Executable coverage | Result / limit |
| --- | --- | --- |
| BOK-01 | `I.InstancePayloadsRoundTrip_BookCopiesAreIndependent_ScrollCopiesStartBlank` | Passed in local runtime/model tests. |
| BOK-02 | `I.UnusableOrFullBooksRefuseWithoutFormulaOrPayment`; `I.DuplicateFormulaRefusalPreservesScroll_AndBorrowedBookCopyPreservesSource` | Passed in local runtime/model tests. |
| BOK-03 | `X.BookCopyPaysActualProductionMaterialOnceAndCancellationPaysNothing` | Passed in local runtime/model tests. |
| BOK-04 | `I.DuplicateFormulaRefusalPreservesScroll_AndBorrowedBookCopyPreservesSource`; `I.UnusableOrFullBooksRefuseWithoutFormulaOrPayment`; `X.BookCopyPaysActualProductionMaterialOnceAndCancellationPaysNothing` | Passed in local runtime/model tests. |
| BOK-05 | `I.HighLevelScrollTranscriptionConsumesScrollWithoutSlotsOrActivationChecks` | Passed in local runtime/model tests. |
| BOK-06 | `I.HighLevelScrollTranscriptionConsumesScrollWithoutSlotsOrActivationChecks`; `I.FailureAfterScrollConsumptionNeverResurrectsChargeOrDuplicatesFormula` | Passed in local runtime/model tests. |
| INS-01 | `I.FiniteInscriptionReservesThenDebitsOnce_AndNeverReleasesSpellEffects`; `I.AtWillInscriptionCanStockpileWhileSpontaneousInscriptionDebitsChosenSlot`; `X.AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain` | Passed in local runtime/model tests. |
| INS-02 | `I.AtWillInscriptionCanStockpileWhileSpontaneousInscriptionDebitsChosenSlot`; `X.AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain` | Passed in local runtime/model tests. |
| INS-03 | `I.InscriptionOfDamagingSpellNeverExecutesItsEffectAndMovedBlankCancels`; `I.FiniteInscriptionReservesThenDebitsOnce_AndNeverReleasesSpellEffects` | Passed in local runtime/model tests. |
| INS-04 | `I.ProductionPlanDetectsSharedQuantityUnderpaymentBeforeAnyConsumption` | Passed in local runtime/model tests. |
| INS-05 | `I.RealActionEventsCancelInscriptionAndReleaseExactReservation`; `I.InscriptionCancellationKeepsBlankAndExactUnspentCasting`; `I.FiniteInscriptionReservesThenDebitsOnce_AndNeverReleasesSpellEffects` | Passed in local runtime/model tests. |
| INS-06 | `X.ScriptReservationAndPlayerInscriptionUseTheSameGuardedTimedDebit` | Passed in local runtime/model tests. |
| INS-07 | `I.InscriptionOfDamagingSpellNeverExecutesItsEffectAndMovedBlankCancels`; `I.RealActionEventsCancelInscriptionAndReleaseExactReservation` | Passed in local runtime/model tests. |
| INS-08 | `I.AtWillInscriptionCanStockpileWhileSpontaneousInscriptionDebitsChosenSlot` | Passed in local runtime/model tests. |

### Scroll activation and potency

| ID | Executable coverage | Result / limit |
| --- | --- | --- |
| SCR-01 | `X.NonCasterAndRevokedPayloadRefuseButUnknownSpellWithAllSlotsSpentReleases` | Passed in local runtime/model tests. |
| SCR-02 | `S.ScrollCeiling_IgnoresSpentSlotsButRequiresPositiveUsableCapacity`; `S.AtWillCeilingIsSpellSpecificAndFiniteCantripUpcastDoesNotAlterAtWillPower` | Passed in local runtime/model tests. |
| SCR-03 | `S.ScrollCeiling_IgnoresSpentSlotsButRequiresPositiveUsableCapacity`; `S.AtWillCeilingIsSpellSpecificAndFiniteCantripUpcastDoesNotAlterAtWillPower`; `I.ScrollOverLevelFailureDestroysChargeWithoutSpellEffects` | Passed in local runtime/model tests. |
| SCR-04 | `X.CharacterAndExitParametersSurviveActualScrollAndFiniteTargetParsing`; `X.RemovedStoredPolicyReferenceRefusesBeforeAnyConsumption`; `X.NonCasterAndRevokedPayloadRefuseButUnknownSpellWithAllSlotsSpentReleases`; `N.CorruptSnapshotBindingIsRejectedBeforeRelease` | Passed in local runtime/model tests. |
| SCR-05 | `I.ScrollOverLevelFailureDestroysChargeWithoutSpellEffects` | Passed in local runtime/model tests. |
| SCR-06 | `X.AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain`; `X.ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce` | Passed in local runtime/model tests. |
| SCR-07 | `X.ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce`; `MagicSpellResolutionTests` | Passed in local runtime/model tests. |
| SCR-08 | `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution`; `N.SnapshotRoundTripPreservesSourceConfigurationAndCreatorNumbersAfterEdits` | Passed in local runtime/model tests. |
| SCR-09 | `E.RetainedArmourAndParentRoundTripAfterSourceDeletionWithoutLosingReaderOrNumbers`; `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution` | Passed in local runtime/model tests. |
| SCR-10 | `N.CaptureDoesNotEvaluateFormula_AndRejectsNonFiniteCreatorBindings`; `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution` | Passed in local runtime/model tests. |
| SCR-11 | `N.NumericBindings_FreezePerLocationAndBonusContext_KeepTargetOutcomeLive`; `N.NumericContextsAreIsolated_AndExtendedOptionsAreStored` | Passed in local runtime/model tests. |
| SCR-12 | `N.SnapshotRoundTripPreservesSourceConfigurationAndCreatorNumbersAfterEdits`; `X.RemovedStoredPolicyReferenceRefusesBeforeAnyConsumption`; `N.CorruptSnapshotBindingIsRejectedBeforeRelease` | Passed in local runtime/model tests. |
| SCR-13 | `E.RetainedArmourAndParentRoundTripAfterSourceDeletionWithoutLosingReaderOrNumbers` | Passed in local runtime/model tests. |
| SCR-14 | `N.CompatibilityManifestCoversEveryRegisteredTypeAndRequiredFamilies` | Passed in local runtime/model tests. |
| SCR-15 | `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution`; `E.StoredStatusAndPerceptionFamiliesCreateRealReaderOwnedEffects`; `E.StoredBoostAndRemovalKeepFrozenScalarsAndApplyToLiveTarget`; `E.RetainedArmourAndParentRoundTripAfterSourceDeletionWithoutLosingReaderOrNumbers`; `N.ActorOwnedMovementUsesReaderAndFrozenConfiguration`; `X.CharacterAndExitParametersSurviveActualScrollAndFiniteTargetParsing` | Passed in local runtime/model tests. |
| SCR-16 | `N.NumericContextsAreIsolated_AndExtendedOptionsAreStored`; `N.NumericBindings_FreezePerLocationAndBonusContext_KeepTargetOutcomeLive` | Passed in local runtime/model tests. |
| SCR-17 | `I.InstancePayloadsRoundTrip_BookCopiesAreIndependent_ScrollCopiesStartBlank`; `I.ScrollPrototypeRejectsContainerAndStackableCompositionsInEitherOrder`; `I.ScrollReleaseConsumesBeforeEffects_AndTombstonePreventsStaleReloadReplay` | Passed in local runtime/model tests. |

### Integration, persistence and documentation

| ID | Executable coverage | Result / limit |
| --- | --- | --- |
| INT-01 | `N.LegacyDirectRouteCannotBypassVancian_AndMixedCapabilityRemainsValid` | Passed in local runtime/model tests. |
| INT-02 | `S.Queries_DoNotCreateCapacityOrPersistence`; `X.NonCasterAndRevokedPayloadRefuseButUnknownSpellWithAllSlotsSpentReleases`; `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies` | Passed in local runtime/model tests. |
| INT-03 | `MagicSpellResolutionTests`; `MagicFutureProgFunctionTests`; `MagicCombatPowerTests`; `MagicalSubstanceTests`; `MagicPhase2Tests`; `MagicEngineV4Tests` | Passed in local runtime/model tests. |
| INT-04 | `S.Queries_DoNotCreateCapacityOrPersistence`; `X.AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain`; `P.EveryPublicVancianFunctionCompilesWithItsExactDeclaredTypes`; `MagicFutureProgFunctionTests.CanCastSpellFunctions_UseGeneralAndCurrentSpellChecks` | Passed in local runtime/model tests. |
| INT-05 | `N.DirectVancianInvocationUsesSyntheticOutcomeAndExactFiniteDebit`; `X.ScrollGroupWithFirstTargetResistedOrWardedStillAffectsLaterTargetsAndConsumesOnce`; `X.CharacterAndExitParametersSurviveActualScrollAndFiniteTargetParsing` | Passed in local runtime/model tests. |
| PER-01 | `S.PlansAndLastPattern_AreIndependentOfSpentSlotsAndDeletedNames`; `R.IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants`; `S.PersistedIndeterminateCallbackRemainsBlockedBeyondDisplayHistoryWithoutReplay`; `R.ContinuousSleepCreditSurvivesCancelledPreparationAndIsConsumedOnlyAtRefresh` | Passed in local runtime/model tests. |
| PER-02 | `I.FailureAfterDurableInscriptionDebitCannotCreateOrRetryAFreeCharge`; `I.FailureAfterScrollConsumptionNeverResurrectsChargeOrDuplicatesFormula` | Passed in local runtime/model tests. |
| PER-03 | `I.ScrollReleaseConsumesBeforeEffects_AndTombstonePreventsStaleReloadReplay`; `I.FailureAfterScrollConsumptionNeverResurrectsChargeOrDuplicatesFormula`; `X.EffectFailureAfterCommitLeavesDiagnosticAndNeverReplaysFiniteOrScrollCasting` | Passed in local runtime/model tests. |
| PER-04 | `S.ReentrantKnownMutation_IsRejectedBeforeHookCanReplaceOuterState`; `S.WholeKnownCommit_NoOpReorderingSkipsHooks_StaleDraftRefuses`; `R.IdentityInstancesShareSelectionsAndSpentLedgerWithoutDuplicatingGrants`; `X.ScriptReservationAndPlayerInscriptionUseTheSameGuardedTimedDebit` | Passed in local runtime/model tests. |
| PER-05 | `D.UpgradeDefaultsDoNotGrantKnowledgeOrScrollEligibility`; `D.IdentityCapabilityKeyHasConcurrencyAndTombstonesOutliveDeletedOwnersAndItems`; `BlankDatabaseSnapshotTests`; disposable MySQL and seeded-server probes above | Model/default/snapshot tests, real MySQL upgrade/fresh import and seeded-server precommit process-termination recovery pass. Arbitrary inventory/effect atomicity is not established. |
| PER-06 | `S.InvalidPersistedSchema_DisablesWithoutSilentlyCreatingEmptyState`; `C.UnsupportedBookModesBrokenLinksAndBadSchemaDisableWithoutOverwriting`; `X.RemovedStoredPolicyReferenceRefusesBeforeAnyConsumption` | Passed in local runtime/model tests. |
| DOC-01 | `P.EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; `P.EveryPublicVancianFunctionCompilesWithItsExactDeclaredTypes`; `N.AllFutureProgContractsAreRegisteredWithTypedReturns`; `X.ScriptReservationAndPlayerInscriptionUseTheSameGuardedTimedDebit`; `X.PublicHelpAndScrollInspectionDoNotCreateStateOrActivateTheCharge` | Compiler/runtime contracts pass; command help and documentation inspected. |
| DOC-02 | `P.EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies`; `X.BookCopyPaysActualProductionMaterialOnceAndCancellationPaysNothing`; `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution` | Runtime constituents and the stated non-admin Wizard/Cleric/Sorcerer book/scroll cases pass. Extended live material-payment, combat and creator/reader effect cases remain unexecuted. |

## Supported scope and extension limits

The [complete compatibility inventory](Vancian_Scroll_Compatibility.md) lists 125 registered load/builder names. Required numeric damage/healing, boost, status/perception, retained magical armour and actor-owned movement families have real adapters. Arbitrary script execution, identity/body creation, nested traps, topology, complex environmental controllers and other unaudited effects fail readiness before inscription or activation payment. They remain available to ordinary spell authoring.

Partially observed sleep is intentionally not durable, and an observation gap over 15 seconds resets that partial episode. Completed qualification is durable and consumed once by successful recovery. Indeterminate committed work requires staff inspection; the journal cannot atomically transact external progs, inventory/world mutations and SQL together. Its ordering conservatively permits a paid operation to need repair while preventing a free or replayed casting. Runtime operation history is capped at 1,000 displayed entries, while unresolved-state and consumed-charge checks are uncapped indexed queries.

The seven review corrections are implemented and exercised through direct regressions. Live database probes and normal-player seeded-server sessions establish the specifically recorded schema, ledger, register and gameplay results. Remaining broader-suite failures, hosted CI and extended live gameplay limits are explicitly separate from those passing checks.
