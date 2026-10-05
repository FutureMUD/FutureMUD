# See the Unbodied: conditional Divination payment proposal

This proposal is based on `d3480a5a6ac64af1544b0d1efd306802f0cd72f4` and the source qualification committed there. Conditional component selection is awaiting coordinator allocation. No payment or selection implementation is included in this checkpoint. The separately allocated ethereal reporting/lifetime adapter is now verified at `3cb199240ab1b308e10b653c0ee259abb70367d0`.

The smallest proposed change needs **zero edits to existing shared payment/selection APIs**. `MagicSpell.InventoryPlanTemplate` already has a public setter; invocation copies already expose the selected grade internally. `IMagicSpellEffectPreparedSelection` already supports capture, immutable reuse and confirmation before debit. Native consumed actions already support a runtime primary selector and `BindSelectedGrade`. The configured service constructs its inventory plan after effect selection and captures actual selected input identities for its existing payment journal.

## Proposed owned files

| File | Purpose |
|---|---|
| `MudSharpCore/Magic/SpellEffects/DetectEtherealEffect.SourceSelection.cs` (new partial) | Optional persisted source scope; implement the existing prepared-selection interface only when explicitly configured. Capture/freeze self recipient, grade, caster cell/overlay/native terrain identity and definition, authored plan/configuration and the Shadow exemption. Build an invocation-local selected plan through existing APIs. |
| `MudSharpCore/Magic/SpellEffects/DetectEtherealEffect.Lifetime.cs` and `DetectEtherealEffect.Operation.cs` (owned partials from the adapter checkpoint) | Add source-scope XML/builder hooks and post-debit frozen-environment confirmation through the new source-selection partial. Preserve the verified generic operation/lifetime behavior for absent source scope. |
| `MudSharpCore/Magic/ArmageddonSeeTheUnbodiedStock.cs` (new) | Builder-editable source stock, explicit Silt and Shadow native identities, exclusive ethereal detection, `1800*grade`, cap36/group and carried Divination rank contract. Pierce **raw80** unlocks See at **opening30**, cap90; the new skill does not require its own raw80 for acquisition. |
| `MudSharpCore/Magic/MagicSpell.ArmageddonSeeTheUnbodiedBuilder.cs` (new partial) | Narrow stock authoring helpers if existing public builder methods cannot express the authored definition. No shared dispatch registration change. |
| `MudSharpCore Unit Tests/SeeTheUnbodiedSourceSelectionTests.cs` (new) | Environment, rank, custody, configuration and payment-boundary tests. |
| Dedicated new native harness partial/project/entrypoint | Paid casts, exact consumption and saved/reloaded perception/expiry evidence; no shared harness dispatch edit. |

No changes are proposed to `MagicCastingService*.cs`, `IMagicSpellEffectPreparedSelection.cs`, `MagicSpell.Casting.cs`, `MagicSpell.cs`, inventory-plan core, topology, installer registry or central ledgers. The separate allocated reporting/lifetime checkpoint extends the existing typed lifetime bridge; it does not implement this proposal.

## Selection and consumption contract

Source configuration must require a dedicated quantity-one, directly carried Divination consume action with `GradeRank offset=-3` and five explicit hierarchical rank tags0-4. Grades1/2/3 require rank0; grades4/5/6/7 require ranks1/2/3/4. Higher descendants qualify; unrelated goods do not. Low grades never waive the outside-Shadow component. Native consumption of one quantity unit, rather than extraction of an entire historical stack object, is the declared engine adaptation. Other authored payment actions must be preserved; the adapter may omit only the precisely validated dedicated component action.

The existing capability prerequisite builder can express `casting prerequisite add <See spell> <Pierce spell> 1 80`: grade1 is its ordinary acquisition baseline and raw80 belongs to Pierce. Source-shaped efficiency with minimum7 and scale1 charges grade1 **7** and grade7 **50** when the acquired controlled grade is7; the printed minimum is not a flat cost at every grade. No new energy curve is proposed.

Silt refusal precedes the Shadow exemption. Silt and Shadow must bind explicit native terrain definitions, with their source-sector meaning documented; no guessed names, historical integers or implicit plane topology. Use raw cell/overlay/terrain state at commitment, not a callback that can change policy during debit. The exact mapping surface must be checked against native terrain APIs before implementation.

Only an invocation copy receives the selected plan. Preserve the original authored plan XML for configuration equivalence. Deep-clone the selected plan and bind its consumed actions to the already selected grade, since replacing the copy's plan after `CastingCopy` otherwise loses its runtime grade binding. Outside Shadow, retain the component action and compose its primary selector with pure checks of the frozen environment. In Shadow, omit just that component action. Do not mutate the builder's persistent spell or shared plan in either branch.

An immutable prepared token freezes the environment, branch, source scope/configuration, authored plan and grade. `TryReusePreparedSelection` and `TryConfirmPreparedSelection` must reject drift instead of choosing a new branch or substituting inputs. Existing service admission then revalidates the actual selected component identity, custody, quantity and rank before payment; existing native consumption performs the mutation once. No component extraction belongs in eligibility or the effect callback.

After debit, the effect operation reconfirms the frozen environment before granting perception. It must not require the consumed item to remain present. Outside Shadow, the composed pure selector also checks environment during native consumption revalidation. A post-debit environment or component change follows the existing `NeedsReview` quarantine path: no guessed replacement, free outside-Shadow grant, duplicate consumption, automatic replay or refund. An empty Shadow plan cannot acquire an outside-Shadow effect after a post-debit move.

## Required qualification before stock acceptance

Tests must cover normal builder construction/edit/clone/reload; Pierce raw79 refusal/raw80 acquisition at opening30; all seven consumed rank thresholds; Shadow no-consumption; Silt/self/absent-component refusal before debit; native quantity conservation; custody/environment/configuration drift before payment and after debit; actual cost/application receipts; old-cohort retention on paid uncertainty; no duplicate/refund/replay after restart; and cumulative ethereal expiry/perception with concrete parent/child reload. A separately allocated implementation must demonstrate these against fingerprinted final source and assemblies in lane-unique disposable native fixtures. This proposal itself claims no conditional-payment acceptance.
