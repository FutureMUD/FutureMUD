# Vancian magic verification and acceptance

Implementation baseline and current Git HEAD: `0d5163c5d8ec06cc70a3af65100be3316dc8c97d`. The implementation is an uncommitted worktree change; no historical source-appendix revision was copied over this checkout. The seven existing `MagicSpellResolutionTests` and the independent group-target loop are preserved.

## Delivered scope

The feature includes the configurable `vancian` capability, stable repertoire/allowance identities, canonical-owner Selected choices and permission/notification hooks, saved loadouts and independent last pattern, finite memorised/spontaneous and explicit at-will allowances, all three recovery modes, instance-owned spellbooks, timed paid transcription/inscription, prepaid single-use scrolls, numerical snapshots, stored-effect reload, typed FutureProg/craft entry points, and administrative diagnosis/repair.

Changed projects are FutureMUDLibrary, MudSharpCore, MudsharpDatabaseLibrary, DatabaseSeeder (installer snapshot), and their affected core/database/seeder test projects. No production content, character grants, seeded classes or stock books/scrolls were installed. No game database was modified. There is no new dependency or target-framework change.

Public entry points are the school `vancian` branch, `spellbook`, `spellscroll`, `magic vancian` administration, spell level/scroll metadata and 27 typed function overloads. See the [player guide](Vancian_Magic_Player_Guide.md), [builder guide](Vancian_Magic_Builder_Guide.md) and [API contracts](Vancian_Magic_FutureProg.md). The implementation uses an XML aggregate per identity/capability and a separate durable operation journal rather than a table per selected spell/slot. Stable GUIDs distinguish rules, allowances, plans and charges; counters and versions are not aliases or shared caster-level values.

## Local verification

The actual toolchain is .NET SDK 10.0.401, net10.0 product/test targets, EF Core 9.0.11 and Pomelo 9.0.0. Windows builds and restores use one MSBuild node. Product builds use `--no-restore -m:1 -p:NoWarn=NU1902%3BNU1510`.

| Gate | Result |
| --- | --- |
| Engine Debug | Passed |
| Engine Release | Passed |
| DatabaseSeeder Debug | Passed |
| DatabaseSeeder Release | Passed |
| Focused core filter `FullyQualifiedName~Vancian\|FullyQualifiedName~MagicSpellResolutionTests` | 94 passed, zero failed/skipped: 87 Vancian cases plus seven existing group-resolution cases |
| EF `migrations has-pending-model-changes --no-build` with database project and engine startup | Passed: no pending model changes |
| Standard `scripts/test-unit.ps1` | 4,801 passed, four core formatting failures, zero skipped; includes every standard suite |
| Full core rerun after final help/diagnostic changes | 3,189 passed, the same four formatting failures, zero skipped; 3,193 total |
| Focused installer snapshot tests after final SQL whitespace cleanup | Six passed, zero failed/skipped |
| Patch hygiene / documentation links | `git diff --check` passed; all 102 decision/test IDs, executable references and local Vancian document links checked |

The tests use the real service, ordinary capability/spell/component constructors and loaders, actual trigger parsers and spell resolution, real inventory plans and the real FutureProg compiler. Boundaries use an injected clock, policy/check results, detached optimistic state store and item persistence callback. The in-memory store deliberately round-trips state XML on reads/commits. Failure injection covers durable slot debit before charge persistence, consumed scrolls before destination formula publication, stale reloaded components and unreplayed callback states. These tests do not substitute for a real MySQL transaction/crash exercise or telnet walkthrough.

The initial standard gate passed 4,786 tests and failed four existing core formatting expectations. Running those four alone, without loading any Vancian tests, reproduces all four failures. Their tests and the casing helper are unchanged. The final gate includes the subsequently added integration cases. The unrelated failures are:

- `MudDateTimeTests.TimezoneBuilder_CanEditAliasNameAndOffset`: expected `Offset Two`, actual `OFfset TWo`.
- `WeatherControllerBuilderTests.ParseWeatherControllerCreationArguments_ZoneName_ResolvesZone`: expected `ZZZ Pilot Weather`, actual `ZZZ PIlot WEather`.
- `EmploymentCommandServiceTests.EmploymentCommandService_BoardPostAllowedWithAuthorityRecordsRegister`: expected `Stock`, actual output contains `STock`.
- `KnowledgePickerBySkillScreenTests.FormatAvailableKnowledges_LongKnowledgeDescriptions_WrapAsSeparateNumberedRows`: existing formatting assertion fails.

This establishes independent reproduction on unchanged code; a separate historical checkout was not built. No old regression test was removed, weakened or excluded from the standard gate. The dedicated climate simulation is outside this feature's scope. Subsequent help/diagnostic changes were followed by the focused suite, both Release builds and the complete core suite. The other standard suites passed: library 500, expressions 23, seeder 893, database 46, Discord 22, converter 43, website 54 and terrain planner 34. The database suite includes both new Vancian model/migration tests.

## Schema and live environment

EF generated `20260912061254_VancianMagic`, its designer and the model snapshot together. It adds `MagicSpells.SpellLevel` default zero and `ScrollInscriptionAllowed` default false; `CharacterMagicCapabilityStates` with a composite identity/capability key and concurrency version; and `VancianMagicOperations` with durable charge identities and item references. The operation table deliberately has no cascading foreign keys so consumption evidence outlives deleted items/owners. Deleting an actual character/capability record cascades its aggregate, while merely losing a capability grant does not.

The normal snapshot refresh could not establish TLS in the sandbox (`SSL Authentication Error`, Windows security package has no credentials). Automatic approval review rejected an escalated attempt to recreate the existing named snapshot database, because it would overwrite an external database without explicit authorization. That recreation was not performed. The installer dump instead received the reviewed, EF-generated idempotent delta from `20260911080024_MagicalSubstances` to the new migration, with exact table identifiers converted to the maintained dump's lower-case convention. The manifest and migration-history row match the new migration. This is an updated dump, not a fresh database export/import.

A subsequent **read-only** connection probe (`SELECT VERSION()`, no schema selected or changed) was allowed outside the sandbox. MySQL rejected the configured development credentials: `Access denied for user 'futuremud'@'localhost'`. This is the current live-verification blocker, independent of the denied database recreation. Credentials were not printed or saved in a new file. No alternate game database was used.

Consequently these activities remain **unexecuted**: real upgrade from the previous schema, fresh installer snapshot import, real EF ledger/charge concurrency and crash recovery, server startup on a fresh seeded world, and the complete non-admin Wizard/Cleric/Sorcerer walkthrough. They require a working connection configured outside source control and new uniquely named disposable databases. Do not reset an existing world to perform them. The [builder walkthrough](Vancian_Magic_Builder_Guide.md) identifies prerequisites and the [player guide](Vancian_Magic_Player_Guide.md) gives the command sequence. This report makes no live acceptance claim.

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
| PER-05 | `D.UpgradeDefaultsDoNotGrantKnowledgeOrScrollEligibility`; `D.IdentityCapabilityKeyHasConcurrencyAndTombstonesOutliveDeletedOwnersAndItems`; `BlankDatabaseSnapshotTests` | Model/default/snapshot tests pass; real MySQL upgrade and fresh import are unexecuted. |
| PER-06 | `S.InvalidPersistedSchema_DisablesWithoutSilentlyCreatingEmptyState`; `C.UnsupportedBookModesBrokenLinksAndBadSchemaDisableWithoutOverwriting`; `X.RemovedStoredPolicyReferenceRefusesBeforeAnyConsumption` | Passed in local runtime/model tests. |
| DOC-01 | `P.EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; `P.EveryPublicVancianFunctionCompilesWithItsExactDeclaredTypes`; `N.AllFutureProgContractsAreRegisteredWithTypedReturns`; `X.ScriptReservationAndPlayerInscriptionUseTheSameGuardedTimedDebit`; `X.PublicHelpAndScrollInspectionDoNotCreateStateOrActivateTheCharge` | Compiler/runtime contracts pass; command help and documentation inspected. |
| DOC-02 | `P.EveryDocumentedPolicyCompilesAndExecutesItsDeclaredContract`; `X.MixedCantripsAndMemorisedBooksUseSeveralAccessibleInstanceBooksAndBothRecoveryPolicies`; `X.BookCopyPaysActualProductionMaterialOnceAndCancellationPaysNothing`; `E.StoredDamageAndHealingUseCreatorTraitsAndLiveOpposedOutcomeWithReaderAttribution` | Runtime constituents pass; the complete non-admin MUD walkthrough is unexecuted. |

## Supported scope and extension limits

The [complete compatibility inventory](Vancian_Scroll_Compatibility.md) lists 125 registered load/builder names. Required numeric damage/healing, boost, status/perception, retained magical armour and actor-owned movement families have real adapters. Arbitrary script execution, identity/body creation, nested traps, topology, complex environmental controllers and other unaudited effects fail readiness before inscription or activation payment. They remain available to ordinary spell authoring.

Partially observed sleep is intentionally not durable, and an observation gap over 15 seconds resets that partial episode. Completed qualification is durable and consumed once by successful recovery. Indeterminate committed work requires staff inspection; the journal cannot atomically transact external progs, inventory/world mutations and SQL together. Its ordering conservatively permits a paid operation to need repair while preventing a free or replayed casting. Runtime operation history is capped at 1,000 displayed entries, while unresolved-state and consumed-charge checks are uncapped indexed queries.

All supported local features are implemented. Live persistence and non-admin acceptance remain unverified for the environmental reasons above; they are not reported as passing or deferred feature development.
