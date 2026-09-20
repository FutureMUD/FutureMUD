# Native Organic Yields and Ecological Penalties

## Delivery identity and boundary

- Assignment: `03A_Native_Yields_and_Ecological_Penalties.md`
- Title: **Task 3A — Native organic yield accounting and ecological penalties**
- Task ID: `MAGIC-LAND-YIELDS-03A`
- Assignment revision: 2, 15 September 2026
- Checked-out implementation base: `13b495103da639114670d3759f9647519f446bdb`
- Attachment's inspected baseline: `e9d9ec800ced7441b9d835f7be9993d25f43d686`

This delivery makes explicitly configured forage, crop/orchard, woodland and pasture stock available through pure observations and controlled native-owner debits. It also lets persistent environmental scars suppress configured positive native production. It does not add a player Land method, a personal-resource payout, another ecological store, or another scheduler.

**This Task 3A delivery was the native-owner prerequisite.** The subsequent [Land gathering method](Magic_Gathering.md) now uses its source plans and penalties; Task 3A itself did not grant a player payout.

## Ownership model

The environmental profile owns only authorisation and formula definitions. Physical stock and all source-unit accounting stay on the native owner:

| Source | Physical identity | Native stock | Accounting owner |
| --- | --- | --- | --- |
| Forage | physical cell, normalised key and effective forage-profile revision | fractional forage yield points | `Cell` |
| Crop/orchard | field and current crop generation | integer crop yield potential | `AgricultureField` |
| Woodland | field and current stand generation | integer woodland yield potential | `AgricultureField` |
| Pasture | field and current pasture-use generation | integer pasture biomass score | `AgricultureField` |

Crop and orchard are aliases for the same native crop slot and therefore resolve to one identity. Apiaries, herds, minerals, salvage, inventory, corpses, characters and carried plants are not organic conversion sources.

The environmental coordinator remains the only world-local environmental service. It resolves the effective profile and locates agriculture with its existing cell index. Native forage and agriculture schedules remain responsible for recovery. No source inspection, debit or penalty decision enumerates all cells or all fields, registers a new heartbeat, or evaluates a mana output.

## Profile authoring

An environmental definition may contain one optional versioned `Organic` subtree. Its absence is the compatibility default: no conversion source is authorised and every native production factor is neutral (`1.0`). Organic validation errors are reported separately and do not disable otherwise valid legacy mana outputs.

A profile accepts at most 32 canonical declarations. Canonical selectors are `forage:<normalised-key>`, `crop`, `woodland` and `pasture`. Multiple forage keys may be declared; field kinds may occur only once. Empty definition and use restrictions mean any compatible native definition/use for that explicitly declared channel, not automatic channel enablement.

The editor grammar is:

```text
magic regenerator set organic sources
magic regenerator set organic source add forage <yield-key>
magic regenerator set organic source add crop|woodland|pasture
magic regenerator set organic source <selector> uses <uses|any>
magic regenerator set organic source <selector> definitions <IDs|any>
magic regenerator set organic source remove <selector>
magic regenerator set organic penalty <channel> <formula|none>
magic regenerator set organic protection <prog|none>
```

The nine penalty channel tokens are `forage`, `crophealth`, `cropyield`, `woodlandhealth`, `woodlandyield`, `pasture`, `cropinitial`, `woodlandinitial` and `pastureinitial`. `none` removes a formula and restores neutral behavior.

Each formula is deterministic, evaluates only its referenced inputs and must produce a finite value in `[0,1]`. Its raw inputs are:

| Input | Meaning and unit |
| --- | --- |
| `scardamage` | persistent remaining physical-cell scar damage |
| `pressure` | current decayed severity-weighted environmental pressure |
| `hasdefile` | 1 if a destructive-use time exists, otherwise 0 |
| `minutessincedefile` | real minutes since destructive use, or 0 when absent |
| `nativestock` | current affected native stock units |
| `nativehealth` | current crop/woodland health score, otherwise 0 |
| `nativeyield` | current crop/woodland yield or affected forage/pasture stock |
| `nativecapacity` | native maximum in the same units as stock |
| `fieldcondition` | current field condition score, otherwise 0 |
| `baselineincrease` | the positive native production allowance before suppression; for forage this is the full configured hourly rate, before remaining capacity is considered |

A penalty formula may also use a declared named environmental input. Only referenced named inputs are read. Random functions are rejected. Recursive environmental cap/penalty evaluation fails closed.

An optional protection prog is definition-only in Task 3A. Its exact signature is:

```text
boolean(character actor, character owner, magiccapability capability,
        text methodKeyText, number requestedAmount, location cell)
```

Task 3A validates and displays the prog but never invokes it to authorise a payout.

## Pure observations and debit boundary

`IEnvironmentalMagicService.InspectOrganicSources` and `InspectOrganicSource` return `NativeOrganicSourceSnapshot`. Status is explicit:

- `Available`: eligible source with positive native stock.
- `Exhausted`: eligible live source with zero native stock. A field source can still have separately reported
  prepaid credit usable for a sufficiently small conversion.
- `Absent`: valid profile/declaration but no compatible live vegetation.
- `Unauthorised`: the canonical selector is not declared.
- `Invalid`: malformed profile/accounting, reference or penalty state.
- `Indeterminate`: a captured owner/lifecycle state cannot be trusted for commitment.

The snapshot contains native stock, field prepaid credit, distinct positive-recovery remainders, lifecycle identity, a source change token, environmental profile identity/revision, and an actionable diagnostic. It is a pure observation: no profile synchronisation, random forage selection, tick, save flag, heartbeat, dirty mark, resource output, operation receipt or callback is produced.

`TryPlanOrganicDebit` validates the current snapshot and creates a short-lived `NativeOrganicDebitPlan`. `TryApplyOrganicDebit` resolves the source again and rejects a changed profile, declaration, owner token, lifecycle or stock before invoking exactly one native-owner compare-and-apply operation. Both gateway calls evaluate configured, currently applicable recovery penalties with current scar, pressure, native and required named inputs. Forage checks its hourly replenishment channel; crop checks health and yield recovery; woodland checks health and yield recovery; pasture checks recovery. Initialisation channels apply when a fresh allocation is established, rather than to an already established source. A zero factor is valid; an invalid factor makes the snapshot `Invalid` while retaining its raw stock and diagnostic. The gateway repeats this check on apply so a scar or prog change after planning cannot bypass it. Low-level native owner compare-and-apply methods are accounting operations; callers requiring environmental conversion permission use the coordinator gateway. A refusal changes neither stock nor accounting.

These plans are deliberately not idempotent, reservations, receipts, or transactions across source and destination holders. They grant no mana. Task 3B must put the native mutation inside its existing durable gathering receipt and payout flow.

## Fractional integer-source accounting

For crop, woodland and pasture, `Q` is the integer stock, `C` is the saved prepaid source fraction, and `d` is a positive logical source-unit debit. Amounts use bounded decimal arithmetic: the accepted inclusive debit range is `0.000000001` through `1000000000000` source units. Planning and apply both calculate:

```text
newWholeDebit = max(0, ceiling(d - C))
require newWholeDebit <= Q
newC = C + newWholeDebit - d
```

The whole debit and `C` update are one owner mutation. Ordinary crafting, harvest and grazing see only the already-reduced `Q`; every later actor, method and destination shares the same `C`. Prepaid credit is a source-unit remainder only. It cannot be harvested, refunded, converted into a maximum, or become a personal balance.

Four debits of `0.25` from `Q=10,C=0` therefore leave `Q=9,C=0`, exactly like one debit of `1.0`. The first debit immediately reduces native stock to nine, preventing an ordinary consumer from reusing the paid unit. A live source at `Q=0,C>0` may still fund a sufficiently small request.

## Field persistence and lifecycle

`AgricultureField.Definition` carries one optional versioned environmental-organic accounting subtree. Version 2 adds the `pastureAssessment` pending/assessed marker; version 1 and absent extensions load as already assessed legacy allocations. It saves monotonic generation counters, per-source revision/credit, and distinct crop-health, crop-yield, woodland-health, woodland-yield and pasture-biomass progress remainders in the same owner checkpoint as the integer stock. No schema migration is required.

Legacy live sources receive a stable legacy generation; absent sources remain generation zero until established. Generation counters remain saved after removal so a same-definition replacement cannot reuse an old identity. Malformed extension XML is retained for inspection, disables only new accounting for its affected source, and is changed only by an explicit staff repair. Ordinary unrelated agriculture remains usable.

Lifecycle behavior is:

- Save/load and environmental or capability definition edits retain accounting.
- Ordinary harvest/craft/grazing/score fluctuations retain the generation and prepaid credit.
- A retained perennial orchard harvest keeps the crop generation.
- Annual harvest, uproot, crop death/removal and replacement planting clear predecessor credit/progress; the next planting has a new crop generation.
- Woodland death/removal/replacement clears predecessor accounting; a replacement has a new woodland generation.
- Leaving pasture for an incompatible use clears predecessor accounting; re-establishment has a new pasture generation.
- Field deletion/recreation receives a new owner ID and cannot inherit accounting.

None of these transitions removes physical-cell scars.

## Ecological production suppression

Suppression is applied once to each positive baseline contribution:

```text
realisedPositive = positiveBaseline * factor
```

Positive contributions are factored while native losses remain whole, with each owner's clamp order retained. For example, `+4` recovery and `-3` stress at factor `0.25` produces `+1-3` where that native operation combines them. Crop growth-day, orchard cycle, woodland-age and spoilage/overripe calendars are unchanged. Rainfall losses, disease, grazing demand, nutrient depletion, costs and already-reduced harvest outputs are not multiplied.

Integer production keeps a separate decimal progress remainder in `[0,1)`, realizes only whole native points, and discards unusable progress at a full cap, dead/absent source, invalid formula, or lifecycle replacement. A retained orchard harvest combines its suppressed positive work bonus with the same harvest's fixed 20-point loss before the final 0–100 clamp: opening 100 and bonus 5 close at 85, while opening 5 and bonus 10 close at 0. A crop tick retains the native order: clamp the nutrient contribution first, then clamp the pollination contribution. Health likewise clamps its ordinary +1 before pollination. Thus yield 0 with nutrient -1 and pollination +2 ends at 2; yield 100 with nutrient +1 and pollination -2 ends at 98. Positive recovery is suppressed without reducing genuine negative contributions, and later losses never create headroom for an earlier positive step. A positive-only operation at full stock clears unusable progress. It never banks server downtime. Forage is already fractional: multiply the full native hourly rate by the factor, then clamp the addition to remaining capacity. Thus stock 99, maximum 100, hourly 10 and factor 0.25 reach 100 on one tick.

Conversion inspection uses the native owner's pure current recovery context. For a non-stressed living crop, crop health includes ordinary +1 and any active positive pollination health bonus; crop yield includes the positive nutrient sign and active positive pollination yield bonus. Stressed or terminal crops have no current positive recovery context. Woodland health applies only when living and unstressed; woodland yield additionally requires the current establishment day to be complete. Pasture recovery requires a later operation's positive score delta, so there is no present periodic contribution to evaluate at conversion inspection. Forage uses its full ordinary hourly production rate. These context reads neither advance production nor mutate stock, progress, revisions or scheduling. The coordinator's apiary candidate index supplies current pollination without a per-conversion scan of all agriculture fields.

Sowing, orchard planting, woodland establishment and pasture establishment apply the matching configured initial factor where the native operation assigns productive values. A new fallow field records its staged pasture default as pending in the field definition XML. The first operation that makes it productive pasture assesses that default together with any positive establishment delta exactly once. A staged 50 plus 20 at factor 0.5 becomes 35. A zero or invalid factor leaves no unassessed default available for conversion. The saved marker becomes assessed with the resulting stock; later re-entry does not refill or reassess unchanged stock. Older fields without the marker are treated as already assessed so retained legacy stock is not reduced on load. Positive crop/woodland recovery, pollination bonuses, and later positive pasture operation deltas use their corresponding recovery factors. Negative outcome components remain whole. Repair through `IEnvironmentalMagicService.ApplyOperation` changes later factors only; it does not add current stock, resurrect vegetation or recreate harvested output.

An invalid configured formula contributes no uncertain positive recovery to its affected channel and emits a throttled diagnostic. Normal negative stress continues. Disabled, unbound and missing-organic configurations retain baseline native behavior.

## Staff inspection and repair

```text
magic environment yields [here|<cell id>]
magic environment yields repair <here|cell id> [crop|woodland|pasture|all]
```

Inspection lists canonical selector, status, physical identity/generation, native stock, prepaid fraction, recovery remainders, current factor and diagnostics. Repair only discards malformed accounting for the requested field channel and establishes safe zero-credit accounting around the unchanged native stock. It grants no magic resource, restores no vegetation and removes no scar.

## Task 3B extension points and save caveats

Task 3B should use these exact public surfaces:

- `IEnvironmentalMagicService.InspectOrganicSource(s)` for previews and staff display.
- `IEnvironmentalMagicService.TryPlanOrganicDebit` for a pure full-amount quote.
- `IEnvironmentalMagicService.TryApplyOrganicDebit` once, only inside the existing parent gathering commitment/receipt flow.
- `NativeOrganicDebitPlan` as a transient validation token, never as the durable receipt.
- `ICell.TryConsumeYield(NativeForageYieldSnapshot, ...)` and `IAgricultureField.TryApplyNativeOrganicDebit` remain owner-only integration points; player/FutureProg code must not call them directly.

Applying a native debit marks its owner dirty but does not save some unrelated environmental receipt. The Task 3B parent operation must checkpoint the changed native owner before payout acknowledgement and quarantine an uncertain save exactly as it does for other native costs. Agriculture stock and accounting are written together by the field save. Forage stock uses the cell's existing forage-yield checkpoint. `IRecoverableSaveFailure` is the narrow owner hook used by `SaveManager` after a failed provider commit: a cell restores every dirty facet and its pre-attempt expected environmental revision before the entire attempted batch is requeued. The hook does not make a failed write successful, provide a transaction receipt, or prove that an ambiguous external provider rollback succeeded. Task 3B must still treat an uncertain commit as uncertain.

## Builder recipes

### Wild forage

With an existing environmental profile and a forage profile containing `herbs`:

```text
magic regenerator edit "Test Grove"
magic regenerator set organic source add forage herbs
magic regenerator set organic penalty forage "max(0, 1 - scardamage / 100)"
magic environment cell here "Test Grove"
magic environment yields here
```

At zero scars, a baseline hourly `+4` restores `+4`. At 75 scar damage it restores `+1`; unrelated forage keys retain their native rates. Repairing scars improves only a later hourly increment. This authoring does not create a player Land method.

### Agricultural field

For a field that should expose its current crop/orchard but not overlapping wild forage:

```text
magic regenerator edit "Test Grove"
magic regenerator set organic source add crop
magic regenerator set organic source crop uses crop orchard
magic regenerator set organic source crop definitions any
magic regenerator set organic penalty crophealth "max(0, 1 - scardamage / 100)"
magic regenerator set organic penalty cropyield "max(0, 1 - scardamage / 100)"
magic regenerator set organic penalty cropinitial "max(0, 1 - scardamage / 100)"
magic environment yields here
```

Neutral ground realizes normal positive crop recovery and initial productive values. Damaged ground suppresses only positive contributions; disease/stress and harvest costs remain whole. Replanting creates a new generation with no predecessor credit. No wild forage source is declared in this recipe, avoiding double accounting for builder-authored representations of the same vegetation.

## Verification record

The durable executed commands, fixtures, quantities, timings and cleanup result for this delivery are recorded in [Native Organic Yields Verification](Native_Organic_Yields_Verification.md). The requirement-to-test map is [Native Organic Yields Acceptance](Native_Organic_Yields_Acceptance.md).
