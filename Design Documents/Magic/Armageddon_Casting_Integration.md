# ARM-01 evidence register and runtime integration map

Task `MAGIC-ARMAGEDDON-ARM-01`, 27 September 2026. Audited checkout:
`ec361d9c2911b31e4a09284acea8e574b9c7ebfa`. The checkout matched remote master when
the assignment was scoped. The brief's planning revision was
`f2adb2cd43a6f993e141d488db91fea6ed1cc486`; observations below refer to the audited
checkout, not an assumption that the planning revision is still current.

Read with [the contract](Armageddon_Casting_Design.md), [candidate repertoire](Armageddon_Repertoire.md),
[machine-readable register](Armageddon_Repertoire.json), [test receipt](Armageddon_Casting_Verification.json)
and [proposed ARM-02 brief](ARM02_Configurable_Casting_Implementation_Brief.md).

## Evidence types and authority

| Type | Meaning | Records in this package |
| --- | --- | --- |
| U: user-selected requirement | The conversation/approved ARM-01 execution plan selects a design direction; this does not authorise production implementation. | Extend configurable capability; both native proficiency arrangements; separate acquisition/grade; names and formulas; quiet/area; practice trains both; charged and focus item roles; trained non-caster activation. |
| P: proposed choice | Concrete recommendation awaiting owner approval. | Numeric tables, exact roster/memberships/prerequisites, reduced historical effects, stock carrier whitelist, disabled scroll learning/production improvement, persistence and ARM-02 boundaries. |
| O: observed implementation | Source method at the stated revision, with a narrower claim than complete gameplay parity. | Runtime map below and each entry's `runtime_evidence`. |
| T: executed test | Native test result establishes the assertions in that test, at this revision. | Two disjoint focused runs, 234 passed, no failed/skipped/inconclusive rows. |
| H: historical source | A supplied reference or available dump describes historical behaviour. | Skill IDs, coded elemental association, referenced offsets; never an assertion about a current live game. |
| X: unresolved evidence/gap | Evidence is unavailable, implementation is absent, or a proposed adaptation still needs acceptance. | External guild trees, exact named dump identity, new casting/progression/practice/item routes and every complete candidate-spell acceptance. |

The assignment's suggested new `channelling` type was a recommendation to evaluate.
Luke selected extending existing configurable capabilities where feasible. The selected
design uses optional policy on `skilllevel`; it does not claim that policy exists today.
The [decision summary](Armageddon_Casting_Decisions.md) separates that selection from
unapproved balance, roster and omission decisions.

## Supplied historical evidence

The files were read from `C:/Users/luker/Downloads/`. They are external evidence, not
new stock data committed to the repository:

| File | Identity and interpretation |
| --- | --- |
| `01_ARM01_Casting_Rules_and_Coverage_Design_Brief.md` | Edition 1, 27 September 2026, task `MAGIC-ARMAGEDDON-ARM-01`. Its execution boundary is documents and proposed briefs. |
| `armageddon_magic_psionics_reference_second_pass.md` | All 152 unique magic `skill[]` sections accounted for. Psionic entries are outside this spell roster. The JSON stores the reference hash and each section's source line. |
| `codedump.c` | 8,354,060 bytes, 242,965 lines. SHA-256 `E44778F6A5FB9FD02DB5C3FF78D4C468B93AE6C854A058D5AF769DC0A95CD59F`. |
| `codedump(1).c` | The exact filename cited by the reference was not present. The available renamed dump matches the sampled offsets below; byte identity to the absent file cannot be proved. |
| External guild/subguild learn-tree files | Not supplied. `boot_guilds` loads external data; the C skill table is not a complete guild prerequisite/learnlist database. |
| Separate pipeline-status attachment / existing ARM-02 assignment | Not located in the bounded supplied-file search. Repository delivery documents provide reported status; this package authors a new proposed ARM-02 brief. |

Verified, one-based offsets in the **available `codedump.c`**:

| Offset | Verified content and limit |
| --- | --- |
| 19269 | `skill_name` table; source-name provenance, not a proposed player-facing name catalogue. |
| 24213–24219 | Reach words include `nil`; interpreting it requires the reach enum and cast switch below. |
| 34366 | `boot_guilds`; learn-tree data is external. |
| 48062 | Guild enum; not sufficient evidence of all spells acquired by each guild. |
| 132782–132794; 133059–133102 | Charging/use paths use stored item mana and caster spell information. The proposed fixed number of captured-payload charges is an adaptation, not an exact copy of that item model. |
| 132875 | `cmd_use`; item activation is separate from ordinary casting. |
| 166970–166976 | Reach enum, including `REACH_NONE`; `nil` is not merely a failure token. |
| 173408–173478; 173487–173506 | Formula parsing and reach-skill selection. Proposed generic formula words are original configurable data. |
| 173690; 173877–173879; 174396–174398 | Room reach adjusts effective power; it is not uniformly just a target-count multiplier. |
| 173692 | `cmd_cast` entry point. |
| 174013–174015 | Quiet/room mana cost multiplication by two; this is H evidence, not authority for FutureMUD balance. |
| 174031–174033; 174306–174326; 174361–174388 | `REACH_NONE` avoids target resolution and manifestation dispatch while improvement occurs in the casting flow. This is a practice analogue, not proof of ARM-01's side-effect-free timed practice contract. |
| 174472 | `skill[]` definitions; source families and function binding can be checked here. |
| 233317; 234644 | `is_magicker` and `is_guild_elementalist`; classification helpers, not complete learn trees. |

The coded families total Fire 19, Water 24, Earth/Stone 22, Wind 23, Shadow 17,
Lightning 15, Void 28 and unspecified 4. There is one register record per source ID;
Wardcraft and Renew Earth are separately marked proposed additions. An explicit
Sorcerer comment is labelled as such; other Sorcerer memberships and every prerequisite
edge are proposals. Sharing a source family never proves historical learnability.

Some second-pass summaries are weaker than the underlying code: scheduled cleanup may
be mistaken for effect creation/delay, and some conjured-weapon notes point at unrelated
formula snippets. Unbound disease/drowning/acid/puddle entries and the Daylight normal
no-op are explicitly identified in the register. No complete historical or live-game
parity claim follows from this inventory or the sampled offsets.

## Registration and primitive coverage

Current authorities are [MagicCapabilityFactory](../../MudSharpCore/Magic/Capabilities/MagicCapabilityFactory.cs),
[SpellEffectFactory](../../MudSharpCore/Magic/SpellEffectFactory.cs),
[SpellTriggerFactory](../../MudSharpCore/Magic/SpellTriggerFactory.cs), and
[ScrollSpellCompatibility.Inventory / Errors / Bind](../../MudSharpCore/Magic/Vancian/ScrollSpellCompatibility.cs#L49).
The [existing scroll inventory](Vancian_Scroll_Compatibility.md) records explicit
supported, actor-context-only and unsupported decisions. Both load and builder
registrations are checked by the executed
`VancianSnapshotTests.CompatibilityManifestCoversEveryRegisteredTypeAndRequiredFamilies`.
This proves inventory coverage, not that every registered effect is portable or that
every candidate spell is implemented.

The JSON records an actual `GetOrApplyEffect`, `CreateEffect`, `CreateWardEffect` or
`RemoveEffects` method for every cited token, never just factory registration.
Inherited status dispatch is
`CharacterSpellEffectTemplateBase.GetOrApplyEffect` and
`CharacterSpellEffectRemovalTemplateBase.CreateEffect` in
[StandaloneStatusSpellEffects.cs](../../MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs#L107).
Tag wards dispatch through
[TagWardSpellEffectBase.GetOrApplyEffect](../../MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs#L51)
into their concrete target-specific constructors. These foundations still require
configured parameters, lawful targeting, duration, source references and route integration.

Representative distinctions that materially affect the roster:

| Candidate behaviour | Observed boundary | Register decision |
| --- | --- | --- |
| Spatial Relocate / Join Traveller | Current `relocate` is wound treatment; `teleport` is the spatial primitive. | Use the actual teleport route with destination policy. |
| Historical Gate / Call Outsider | Historical spell summons an actor; a portal token does not establish this. | NPC creation plus missing summon control/lifecycle. |
| Invisible/ethereal/magical detection, flight and infravision | Concrete native status implementations exist; previous generic gap descriptions are not current absence evidence. | Trace actual status constructors and perception tests, retain native limitations. |
| Purge Toxin / Purge Disease | Removal templates match spell effects by configuration. Their removal callbacks withdraw linked drug doses or the tracked infection; they do not cure all unrelated medical state. | Explicit limited adaptations. See `RemovePoisonEffect.RemoveEffects`, `RemoveDiseaseEffect.RemoveEffects` and `SpellPoisonEffect.RemovalEffect` / `SpellDiseaseEffect.RemovalEffect` in [the status runtime](../../MudSharpCore/Effects/Concrete/SpellEffects/StandaloneSpellStatusEffects.cs#L441). |
| Poison / disease application | Effect-owned dosing/infection has an initial callback and removal lifecycle. | Timed ownership; not an instantaneous, unowned permanent disease claim. |
| Intoxication / sobriety | [NeedDeltaEffect.GetOrApplyEffect](../../MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs#L154) takes `AlcoholLitres`, not arbitrary drunkenness points. | Proposed signed 0.01 litres per grade, native clamps; hydration uses `ThirstPoints`. |
| Draw Water / Draw Wine | Container creation has a capacity/null-path concern for an empty mixture; a creation token does not prove the proposed container operation. | Small correction candidate; native empty-container proof required in a later approved slice. |
| Siphon / lifesteal | Independent damage and heal effects do not measure actual delivered damage as one conserved transfer. | Small missing composite/hook, not claimed complete. |
| Temporary knives, summons and shelters | Creating an item/NPC does not establish expiry, rider/occupant safety, inventory conservation or creator command authority. | Bounded missing lifecycle or larger supporting system. |
| Quiet, area, practice, charged wand/staff | Ordinary effect registration supplies none of these invocation contracts by itself. | Common later-slice gaps. |

Effect-only classifications: 38 existing primitive/configuration, 65 existing primitive
plus bounded policy/content, 43 small missing primitive, 8 larger supporting system.
These are 154 candidate records, **not completion percentages**. The approved-deferral
classification has zero entries because the owner has not approved omissions/substitutes.
Every complete roster acceptance is `NOT_RUN_PROPOSED_CONTENT`, including entries with
usable primitives. No new exact gap implementation cards are authorised by this register.

## Runtime integration map

All method references below use revision `ec361d9c2911b31e4a09284acea8e574b9c7ebfa`.
T1 and T2 identify the executed runs below; O-only means inspected source with no claim
that an ARM-01-specific regression already exists.

| Surface / exact source method | Observed contract | Required integration / proof limit |
| --- | --- | --- |
| [MagicCapabilityFactory.RegisterLoader / LoadCapability](../../MudSharpCore/Magic/Capabilities/MagicCapabilityFactory.cs#L15) | Reflection discovers loader registrations. `skilllevel` and `vancian` are concrete capability types. | Optional configured policy on existing type; no per-element factories. O-only for the new XML. |
| [SkillLevelBasedMagicCapability model constructor / SaveToXml / BuildingCommand](../../MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs) and [IMagicCapability](../../FutureMUDLibrary/Magic/IMagicCapability.cs) | School, power thresholds, regenerators and concentration are existing capabilities. Current power editing enforces its school. | New optional repertoire policy contract; do not expand `Powers` into implicit spell acquisition. Missing ARM-02. |
| [VancianMagicCapability](../../MudSharpCore/Magic/Capabilities/VancianMagicCapability.cs) | Inherits skill-level capability while owning distinct allowance/state rules. | Do not accidentally enable the new route via shared XML loading. Preserve separate Vancian capability. T1/T2 verify existing independence only. |
| [Character.Capabilities / CheckResources / LoadMagic / SaveMagic](../../MudSharpCore/Character/CharacterMagic.cs#L28) | Non-admin capabilities come from applicable merits and effects; administrators see all. Generator reconciliation adds missing balance at zero; load has its own starting-resource path. | Non-admin fixtures mandatory; explicit permanent enrolment and canonical configured-reserve holder/reconciliation needed. Existing checks do not prove no duplication across the new route. |
| [DrugInducedMagicCapability.NewCapabilities / RemovalEffect](../../MudSharpCore/Effects/Concrete/DrugInducedMagicCapability.cs#L18); [CharacterForms](../../MudSharpCore/Character/CharacterForms.cs) body transition | Addition can call `CheckResources`; removal/form transitions are not a complete central reconciliation contract. | Reconcile on expiry/removal/focus/body transitions; suspend access without clearing progress. O-only lifecycle gap. |
| [Character.Identity and secondary constructor](../../MudSharpCore/Character/CharacterInstances.cs#L46) | Secondary constructor copies references with `_characterTraits.AddRange(identity._characterTraits)`. Existing trait objects can be shared while later list membership diverges. | Do not claim native acquisition through a second body is already coherent. Canonical add/remove/lookup integration and native persistence tests required. |
| [Character.AddTrait / RemoveTrait / SetTraitValue](../../MudSharpCore/Character/CharacterTraits.cs#L229), [SkillDefinition](../../MudSharpCore/Body/Traits/Subtypes/SkillDefinition.cs) | Trait scope routes character-owned skills versus body-owned attributes. Ordinary form change differs from a simultaneous instance. | Preserve body-owned routing; acquire configured native skill once on canonical identity. O-only for the gap. |
| [CharacterInstanceIdentityComparer and physical resolver](../../FutureMUDLibrary/Character/CharacterInstanceIdentityComparer.cs), [CharacterInstanceFocusService.Focus](../../MudSharpCore/Character/CharacterInstanceFocusService.cs) | Identity and physical instance are intentionally different; focus checks reject inappropriate secondary instances. | T1 proves focus handoff/refusals, not canonical future grade/cooldown persistence. Library comparer tests were inspected, not executed in ARM-01. |
| [Check.HandleStandardCheck](../../MudSharpCore/RPG/Checks/Check.cs#L99), [Skill.TraitUsed](../../MudSharpCore/Body/Traits/Subtypes/Skill.cs#L34), [ClassicImprovement.GetImprovement](../../MudSharpCore/Body/Traits/Improvement/ClassicImprovement.cs#L357) | Checks call native trait improvement and can branch. Expression traits and the supplied trait may both be used. Native no-gain is added after the chance/difficulty gates, before evaluating gain amount, on the supplied owner. | Invocation-local suppression/owner seam prevents duplicate improvement; evaluate bodily bonuses on actor and mutate canonical skill once. The 60-second opportunity record is new, not existing `NoTraitGain`. |
| [BranchingImprover.GetImprovement](../../MudSharpCore/Body/Traits/Improvement/BranchingImprover.cs#L69) | Native branching relates skills through its improver and caller; it is not capability-scoped acquired-spell state. | Keep native skills; new reverse-index acquisition evaluates only affected authorised admission edges. O-only new progression. |
| [GameModule.Teach](../../MudSharpCore/Commands/Modules/GameModule.cs#L1033), [SkillLessonProposal.Accept](../../MudSharpCore/Body/Traits/SkillLessonProposal.cs#L20), [SkillDefinition.CanTeach / CanLearn](../../MudSharpCore/Body/Traits/Subtypes/SkillDefinition.cs#L150) | Skill lessons use proposal, permission and learning hooks. Skill ownership does not distinguish spells sharing one trait. | Keep skill lessons; explicit spell lesson is a later proposed slice. No new lesson test executed. |
| [MagicModule school command dispatcher](../../MudSharpCore/Commands/Modules/MagicModule.cs#L100) | Resolves school, lists known/ready spells, dispatches trigger or Vancian route. | Add explicit configured route and acquisition-aware listings; reject unsupported later-mode syntax in ARM-02. O-only new grammar. |
| [MagicSpell.CharacterKnowsSpell / HasLegacyRoute / CastSpell](../../MudSharpCore/Magic/MagicSpell.cs#L1379) | Knowledge combines legitimate legacy/Vancian sources. `HasLegacyRoute` currently counts same-school non-Vancian capability plus known prog; cast triggers call the core method directly. | Exclude configured-only capabilities from this legacy grant check. Direct-trigger and same-school true-prog negative tests are explicitly required; command-only routing is insufficient. T2 tests existing Vancian exclusion, not this absent configured policy. |
| [MagicSpell.CanCastSpell / CastSpell internal pipeline](../../MudSharpCore/Magic/MagicSpell.cs#L1432) | Quotes global casting-trait costs, checks inventory, commits before cast check, resolves groups, wards/resistance and once-only caster effects. | Route-local trait/resource binding, shared commit callback, no second debit/check; add actual-effect success signal for mastery because ordinary aggregation permits empty-group success. T1 resolution tests preserve these distinctions. |
| [MagicSpell.InvocationCopy](../../MudSharpCore/Magic/MagicSpell.Snapshots.cs#L23), [SpellNumericalContext.Capture / Evaluate](../../MudSharpCore/Magic/Vancian/SpellNumericalContext.cs), [ContextualSpellExpression](../../MudSharpCore/Magic/Vancian/SpellNumericalContext.cs#L85) | Detached definitions and per-field/context numerical capture exist for audited stored spells. They do not supply arbitrary route-local trait/resource/grade overrides. | Reuse isolation approach, add explicit validated binding for required fields; never mutate catalogue spell. T2 verifies frozen creator numbers, live outcome and independent contexts only. |
| [SpellPowerInvocation.For](../../MudSharpCore/Magic/SpellPowerInvocation.cs#L26), [SpellBackedPower.UseCommand](../../MudSharpCore/Magic/Powers/SpellBackedPower.cs#L55) | Independent power uses actor/spell invocation context; it is not a repertoire-casting grant. | Preserve independent grant/check/cost semantics and scope context correctly around detached copies. T1 independent-power cases. |
| [InventoryPlan.PlanIsFeasible / ExecuteWholePlan / FinalisePlan](../../MudSharpCore/GameItems/Inventory/Plans/InventoryPlan.cs#L73), [MagicSpell plan execution](../../MudSharpCore/Magic/MagicSpell.cs#L1522) | Actual plan feasibility and execution allocate retained/consumed inventory; mutation is not a transaction over world effects and MySQL. | Normal and practice have separate plans; preserve combined material allocation. Uncertain partial mutation cannot be treated as an unpaid retry. T1 combined-material and failed-cast cases. |
| [CanCastSpellFunction.Execute / registration](../../MudSharpCore/FutureProg/Functions/Magic/MagicResourceAndSpellFunctions.cs#L340) | Existing `cancastspell` / `cancastspellnow` inspect their existing knowledge/readiness/resource contracts. Other functions mutate resources separately. | New pure acquired/grade/route queries and guarded paid invocation; do not silently redefine old function semantics. T2 compiles existing signatures and tests current query/resource behavior. |
| [MagicGatheringService.AccessError / ActionError / Quote / CreditDestination](../../MudSharpCore/Magic/Gathering/MagicGatheringService.cs#L637) | Delivered gathering owns Self/Gentle/Land costs, focus/action gates and receipts; destination balance/cap/credit currently uses its actor. | For configured reserves resolve canonical holder consistently at quote, credit, persistence and casting debit; body costs stay on actor. T2 exercises current gathering Progs, not future shared-holder integration. |
| [VancianMagicService.Cast](../../MudSharpCore/Magic/Vancian/VancianMagicService.Casting.cs), [ActivateScroll](../../MudSharpCore/Magic/Vancian/VancianMagicService.Scrolls.cs#L45), [StoredSpellSnapshot.Capture / CreateSpell](../../MudSharpCore/Magic/Vancian/StoredSpellSnapshot.cs) | Guarded paid commit and stored potency/references exist. Scroll release is not another payment of production costs. | New charged wand/staff and trained-noncaster eligibility remain absent integrations. Retain source validation, opt-in and reader attribution. T1/T2 portable and uncertainty tests only for existing routes. |
| [SubstanceSpellResolver.Errors / CanApply](../../MudSharpCore/Magic/SubstanceSpellResolver.cs#L43) | Carrier-specific supported effects, dose lifetime, target and caster-effect restrictions; not the same admission as scrolls. | Separate limited manufacturing/capture contract. T1 dose/ward tests do not establish ordinary acquired casting or wand manufacturing. |
| [SupernaturalSeeder](../../DatabaseSeeder/Seeders/SupernaturalSeeder/SupernaturalSeeder.cs) | Existing supernatural content installer is an ownership/integration surface, not an installed version of this candidate catalogue. | Later approved preset installer owns stable keys and conflict-safe updates. ARM-01 makes no seeder or game-data changes. |

## Required behaviour disposition

Every item has either narrow existing evidence or a named missing/deferred integration.
“Later slice” here is a proposed implementation boundary, not an approved roster omission.

| Requirement / design example | Existing evidence | Missing work / acceptance owner |
| --- | --- | --- |
| A-D01; A01, explicit cross-school spell, preserved native school | Capability and school/trigger paths inspected; T1 independent allowance costs. | ARM-02 acquisition whitelist, route-local binding, non-admin Earth/Sorcerer proof. |
| A-D02; A02, knowledge with empty reserve; no book/scroll free route | T1 `VancianKnowledge_DoesNotGrantSpellBackedPowerOrFreeDirectCasting`, retained book/inspection tests. | ARM-02 pure acquired query and configured-only legacy bypass rejection. |
| A-D03/A-D04; A03, controlled grades, lower casting, next-grade overreach | Existing `SpellPower` and native difficulties inspected. | Entire controlled-grade state, thresholds, costs and one-roll advancement are new ARM-02. |
| A04, temporary removal/restoration and recharge isolation | `CheckResources` and removal/form source inspected. | ARM-02 event reconciliation/enrolment and persistence; no duplicated pool, roots or generators. |
| A05/A08, two bodies and both proficiency configurations | T1 focus/initialisation tests; actual trait-list sharing source. | ARM-02 secondary acquisition fix, canonical clocks, same-trait distinct acquisition, separate-trait isolation. Existing tests do not prove these. |
| A-D05, resource price/secondary cost, paid failed check | T1 cast-resolution and normal-cost power tests. | ARM-02 same-resource aggregation, one receipt/debit and independent capability entitlement. |
| A-D06; A07, scoped prerequisite and teaching | Branching/lesson hooks inspected. | ARM-02 affected-edge acquisition; spell teaching later. No trait-only cross-tradition grant. |
| A-D07; A06, pure timed practice | Historical nil reach is an analogue only. | Later practice service/plan/cancellation tests; never call manifestation or inspect target wards. |
| A-D08; A09, names/formulas, quiet and area | Historical parser/reach offsets, current group/ward T1. | Later parser-to-one-intent integration, physical speech/perception, explicit per-spell area adapter, once-only learning. |
| A-D09; A10/A11, charged/focus selection and trained non-caster | T1/T2 stored snapshot, charge commit, reader attribution and no-repay tests. | Later charged component/production/skill eligibility; explicit role, no depleted-charge fallback, no acquisition. |
| Portable subset and substances | T1 carrier/dose tests; T2 manifest completeness. | Approve roster pairings and implement production; unsupported retained effects remain rejected. |
| A12, uncertain shared state across alternate routes | Existing Vancian effect-failure/tombstone tests demonstrate narrower no-replay behavior. | ARM-02 receipt stages, shared-state quarantine, post-roll/save injection and restart reconciliation. No current durable ARM-02 proof. |
| A-D10; A13, installer stable ownership/rerun | Existing installer and stock conventions inspected. | Later seeding slice with collision, customised field, repeated grant/charge tests; no stock writes now. |
| Self/Gentle/Land, native organic accounting and rejuvenation | Delivered implementation/docs; T2 exercises current Land-gather Prog integration. | Reuse as dependencies. Full environmental suite/native acceptance not rerun for ARM-01; retain 04C gate separately. |

## Executed baseline and limits

Two specified, non-overlapping selections from `MudSharpCore Unit Tests` ran serially on
unchanged runtime/test source. Both used the repository compact reporter and native TRX,
Debug / `net10.0`, SDK `10.0.401`, single-node build/test, `-FailOnSkipped`. Their summaries
reported complete execution, complete counts and stable within-run source fingerprints.
No cancelled, superseded or unexecuted result is counted as a pass.

| Receipt | Result | Coverage |
| --- | --- | --- |
| T1 `20260927T032154Z-72913fe48504` | 207 passed; 0 failed/skipped/inconclusive | Resolution 7; Vancian review 15, integration 15, item 21; substances 32; instance focus 8/initialisation 3; information 5; wind 7; phase 2 12/phase 3 8; engine V2 9/V3 11/V4 43; sensory/combat 5; portal topology 6. |
| T2 `20260927T034128Z-9cbd6ee7faae` | 27 passed; 0 failed/skipped/inconclusive | Vancian snapshot 10; spell phase one 8; magic FutureProg 9. |

The [JSON receipt](Armageddon_Casting_Verification.json) preserves exact filters,
commands, source fingerprints, per-class counts, native report hashes/counters and each
executed native result name. Raw `summary.json`, `test-results.json`, process logs and
TRX remain in `.artifacts/test-runs/<run-id>/` locally, not checked in as production data.
T1 used `scripts/test-unit.ps1`; T2 used `scripts/test-unit-core.ps1` with no restore.
The wrapper's local `NuGetAudit=false` and `NoWarn=NU1902;NU1510` switches are recorded;
these runs are gameplay/unit checks, not dependency-security verification.

Representative assertion-level evidence, all passed:

| Test source and method | What it supports |
| --- | --- |
| [MagicSpellResolutionTests](../../MudSharpCore%20Unit%20Tests/MagicSpellResolutionTests.cs): `CastSpell_CastingCheckFails_CommitsCostsMaterialsAndLockoutBeforeFailing` | Paid failure follows actual commitment. |
| Same source: `CastSpell_GroupFirstMemberIsWardBlocked_LaterMembersResolveWithoutAResistanceCheck`, `CastSpell_GroupAllMembersReject_ProcessesAllAndLeavesCommittedInvocationFailed` | One rejected member does not terminate group processing; all rejected remains failed after payment. |
| Same source: `CastSpell_EmptyGroup_PreservesCasterEffectsAndSuccessfulInvocation`, `CastSpell_InstantaneousTargetEffectWithoutAChild_StillResolvesAndRunsCasterEffects` | Existing aggregation semantics; absence of a returned persistent child is not proof of no instantaneous effect. ARM-02 must distinguish no-op/empty mastery eligibility explicitly. |
| [VancianReviewRegressionTests](../../MudSharpCore%20Unit%20Tests/VancianReviewRegressionTests.cs): `SpellBackedPower_RemainsIndependentAfterVancianSlotsAreExhausted`, `SpellBackedPower_LegitimateLegacyRouteStillUsesItsNormalCheckAndCosts`, `VancianKnowledge_DoesNotGrantSpellBackedPowerOrFreeDirectCasting` | Current independently granted power and Vancian knowledge boundaries. |
| Same source: `CombinedMaterials_RetainedToolIsAllocatedSeparatelyFromConsumedItem`, `CombinedMaterials_HeavilyOverlappingImpossiblePlanRefusesWithoutSpending` | Real retained/consumed allocation and refusal; does not provide a practice plan automatically. |
| [VancianIntegrationTests](../../MudSharpCore%20Unit%20Tests/VancianIntegrationTests.cs): `PublicHelpAndScrollInspectionDoNotCreateStateOrActivateTheCharge`, `AvailabilityAndCastingUseLevelBasedCostsAndScrollReleaseNeverPaysThemAgain` | Existing read-only inspection and prepaid release. |
| Same source: `EffectFailureAfterCommitLeavesDiagnosticAndNeverReplaysFiniteOrScrollCasting` | Existing uncertain effect failure preserves diagnostics/no replay; not future progression-save proof. |
| [VancianItemTests](../../MudSharpCore%20Unit%20Tests/VancianItemTests.cs): `ScrollReleaseConsumesBeforeEffects_AndTombstonePreventsStaleReloadReplay`, `InstancePayloadsRoundTrip_BookCopiesAreIndependent_ScrollCopiesStartBlank` | Existing payload/tombstone and copy semantics; unit fixture round-trip is not a new native database probe. |
| [VancianSnapshotTests](../../MudSharpCore%20Unit%20Tests/VancianSnapshotTests.cs): `NumericBindings_FreezePerLocationAndBonusContext_KeepTargetOutcomeLive`, `NumericContextsAreIsolated_AndExtendedOptionsAreStored`, `LegacyDirectRouteCannotBypassVancian_AndMixedCapabilityRemainsValid` | Scoped capture/reader boundaries and existing Vancian legacy exclusion. |
| [MagicalSubstanceTests](../../MudSharpCore%20Unit%20Tests/MagicalSubstanceTests.cs): `Ward_BlockedActivationSpendsCharge_AndFreshDoseStillWorks` | Existing dose charge/ward behavior. |
| [CharacterInstanceFocusServiceTests](../../MudSharpCore%20Unit%20Tests/CharacterInstanceFocusServiceTests.cs): `Focus_ValidSecondary_SetsControllerContextToTarget`, `CanFocus_OtherIdentity_ReturnsFailure` | Actual focus service selects/refuses instances. No new acquisition/cooldown test is implied. |
| [MagicFutureProgFunctionTests](../../MudSharpCore%20Unit%20Tests/MagicFutureProgFunctionTests.cs): `CanCastSpellFunctions_UseGeneralAndCurrentSpellChecks`, `GatheringFutureProgFunctions_CompileWithTheirExactDeclaredTypes` | Existing query contracts and typed gathering signatures. |
| [MagicEngineV3Tests](../../MudSharpCore%20Unit%20Tests/MagicEngineV3Tests.cs): `SleepEffect_DoesNotApplyWhenInsomniaBlocksSleep`, `DetectPoisonEffect_ReportsActiveAndLatentDrugDosages` | Concrete effect admission/information assertions, narrower than a complete roster spell. |

Factory/XML tests among the broader selections establish construction/round-trip facts
only. The register does not claim that passing them proves target effects, bodily
permissions, full historical semantics or native database durability.

Not run: the full solution, full environmental/04C suite, native MySQL acceptance,
Telnet gameplay, new acquired/grade state, new practice/variants/items and all 154
candidate-content scenarios. ARM-01 introduced no runtime feature to exercise. The
proposed implementation slices require executable regressions and native evidence when
those features actually exist; these omissions are not passing acceptance results.

## Retained 04C acceptance gate

[Land_Rejuvenation_Verification.md, Task 4C section](Land_Rejuvenation_Verification.md#task-4c-checkpoint-deadline-correction--27-september-2026)
reports that its initial save/pump-order reproductions failed twice, then corrected run
`20260926T234024Z-ad38eac683f4` passed 382/382 (74 rejuvenation and 308 related tests),
with stable fingerprint
`51c63fb14c9003872d774477591be9838a21409b6a96c48489b40277d5932606`.
It records retaining an indexed treatment's `DueAt` during persistence-only checkpoints
and removing an indexed registration before key mutation. Those are **reported prior
correction results**. ARM-01 neither re-executes that acceptance nor performs a fresh
review of the correction. Explicit 04C acceptance remains necessary for the full preset.

## Package checks and stop boundary

Validation checks JSON parsing/required structure, the exact 152 source IDs plus two
marked additions, unique stable keys/names, allowed classifications/dispositions,
membership evidence labels, same-tradition prerequisite references and acyclic graphs.
The Markdown is deterministically rendered from the JSON and checked for agreement;
short historical names and implementation-method references are validated separately.
Package links, source paths/line bounds and documentation diff are checked after edits.

Independent read-only review identified cross-route uncertainty, direct-trigger legacy
bypass acceptance, fixed-scalar grade binding and no-op versus applied-operation reporting
gaps. All four are now explicit in the contract and ARM-02 brief.
It also caught a multiline source-name extraction error; the corrected parser rejects
multiline names. Documentation validation is distinct from runtime acceptance.

Only `Design Documents/` files belong to the final change. Local build/test reports and
authoring helpers remain ignored under `.artifacts/test-runs/`. No production source,
schema, seed stock or game database was changed. No commit, publication, merge or
deployment is part of this delivery. All follow-on briefs remain proposals.
