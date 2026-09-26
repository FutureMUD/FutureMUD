# Environmental magic runtime

Environmental magic is an opt-in physical-cell resource producer. Its reusable definition is the `environmental` magic regenerator. It adds neither a second mana balance nor its own player gathering route. The separate [capability-configured gathering feature](Magic_Gathering.md) uses exact managed debits for Gentle and combines configured local managed and native sources for destructive Land. It owns no second coordinator, generator or environmental balance. [Native organic yields and ecological penalties](Native_Organic_Yields_and_Ecological_Penalties.md) describe the profile-authorised forage/agriculture conversion and recovery suppression used by Land. See [the builder guide](Environmental_Magic_Builder_Guide.md) for commands and test configuration.

## Ownership and persistence

`Cell.EnvironmentBindingMode` resolves an explicit profile, explicit disabled state, or the current physical overlay's terrain default. An observer's draft overlay and room layer do not create separate pools. Assignments survive restart. Missing profiles, resources, incompatible holders, invalid definitions, and calculation failures remain diagnosable; they do not select a replacement.

Balances remain in `Cell.MagicResourceAmounts` / `CellsMagicResources`. First attachment of an output starts at zero unless the cell already has that resource, including an explicitly saved zero. Removing or replacing a profile preserves balances. Environmental definitions never acquire a base-generator per-holder delegate; character and item attachment is refused. Existing `linear` and `state` scheduling and unbound resource policies retain their previous behaviour.

`CellEnvironmentalStates` holds one versioned record per physical `CellId`: scars, last destructive-use UTC time, bounded severity-weighted pressure, its UTC reference, and decay anchor. Scars are not effects and survive overlay changes, dispel, detach, replacement, and restart. `EnvironmentalMagicOperations` stores receipts for explicit damage/repair, including earned spell treatment steps. Ordinary ambient regeneration and natural recovery create no receipt.

Pressure is `Panchor * 2^(-elapsedHalfLives)`, with an accumulator ceiling of `1e12`. The profile saves a cumulative half-life integral and its last setting-change time. Each pressure-bearing cell anchors into that integral, preserving old/new decay settings through incremental bulk edits and restart without an unbounded edit history. Detached state uses its saved half-life. Repair removes only remaining scars, returns the actual amount, and preserves pressure history and last destructive-use time.

Explicit operations commit the caller's GUID receipt, ecological state, current resource balances, and binding/pressure-profile dependencies in one isolated database transaction. An uncertain outcome freezes ecological and resource writes until the same operation identity is confirmed. A confirmed duplicate returns its saved receipt without replacing newer live deferred work. Failed confirmation never automatically replays a write. Ordinary regeneration and natural repair use the existing deferred save manager, with a checked persisted revision to refuse stale overwrites.

## One maximum and exact resource boundaries

For a configured output, `SimpleMagicResource.ResourceCap(cell)` replaces that pair's ordinary cap with the environmental maximum. Other resources and holders retain their cap prog. `TryInspectResource(cell, resource, out result)` identifies managed pairs and returns a pure current calculation. Invalid results are explicit; `ResourceCap` returns `NaN` for an invalid managed calculation, not a legitimate zero. `CanUseResource` requires valid inputs and enough recorded stock below the current maximum. Queries never award production, initialise outputs, dirty cells, or move deadlines.

`TryMutateResource` handles credits, sets, and exact debits. Normal cell resource operations and registered FutureProg set/add/subtract functions call it. It resolves newly inherited assignments immediately, validates current inputs, projects the prior accepted online segment, validates all outputs, then applies the changes and updates eligibility. A valid lower cap discards excess energy. Larger capacity does not refill it. A zero-delta request enforces a changed lower cap; unchanged full balances do not enqueue saves.

Capability-configured Gentle gathering uses `TryDebit(cell, resource, amount, out error)`: success requires a managed pair, current valid inputs, and the entire requested recorded amount. Failure makes no partial debit. Its caller validates the whole source/destination operation before debiting; this is not a transaction across arbitrary world objects. The coordinator owns no gathering command. Its separate native-organic plan/apply surface routes one validated mutation to the physical forage or field owner, grants no resource, and is not a cross-holder transaction.

## Pure input evaluation

Definitions compile expressions and resolve references per revision. Valid profiles allow at most eight outputs and 32 named inputs. The coordinator collects only referenced sources, reads native values once per evaluation, shares an immutable snapshot across outputs, evaluates maxima, then rates. The strict expression API distinguishes errors from legitimate zero.

Built-ins are `scardamage`, `pressure`, `hasdefile`, and `minutessincedefile`; each output supplies `basecapacity` and `baserate`. Only rates may use `balance` and `maximum`. An absent defile time has presence and age zero. Named forage/agriculture values have explicit scales. Agriculture scores use their raw native units (normally 0–100); absent fields/crops/woodland contribute zero, with explicit presence sources available.

`ICell.TryPeekForagableYield` projects the effective forage profile without synchronising, refilling, or scheduling yields. Its richer native snapshot also captures the physical cell, normalised key, effective forage-profile identity/revisions and owner change token for compare-and-apply. A new valid key projects its profile maximum; an existing depleted key stays depleted subject to a lower maximum. An absent forage subsystem contributes zero, while a missing key in a configured profile is an error. Ordinary synchronisation remains at load/configuration/consumption/recovery boundaries.

The world-local field-by-cell index is built once and maintained on field creation, replacement, and deletion. Input evaluation does not scan all agriculture fields. Delayed deletion of an old field cannot remove its replacement.

Optional numeric location progs must be compiled and `NotStatic`, and builders must author them as read-only policies. The engine does not infer dependencies or make arbitrary progs pure. Environmental cap recursion and attempts to mutate the same cell during input evaluation are rejected. Non-finite values and negative maxima/rates fault the managed route without destroying stock. Scalar `InspectState` queries execute no input progs.

## Native organic source and penalty boundary

The optional versioned `Organic` profile subtree is intentionally separate from mana output validity. A missing subtree authorises no native conversion and makes every native production factor neutral. Malformed source/protection configuration makes Land accounting unavailable without disabling legacy mana outputs. An invalid factor suppresses only the affected uncertain positive production; normal negative stress continues.

`InspectOrganicSources` and `InspectOrganicSource` resolve canonical forage/crop/woodland/pasture declarations through the effective physical-cell binding. Agriculture uses the coordinator's existing field index. The observation reports available/exhausted/absent/unauthorised/invalid/indeterminate state, stock, prepaid fraction, production remainders, lifecycle and owner/profile revision tokens. It performs no mana-output evaluation and accepts no scheduling sample.

`TryPlanOrganicDebit` is pure. `TryApplyOrganicDebit` repeats authorisation, applicable configured recovery-penalty validity with current native/scar/pressure inputs, identity, stock and exact arithmetic validation before one native-owner mutation. Invalid dynamic factors reject planning and apply while raw inspection retains the stock and diagnostic; a factor of zero remains valid. Crop, woodland and pasture prepay a whole integer unit and retain the unused decimal source fraction on their field. Forage uses the actual fractional cell stock. These calls create no ecological receipt and grant no personal balance; the later parent gathering operation must supply its own durable receipt and persistence boundary.

Land's bounded batch validates all native plans before the first mutation. After that boundary, each distinct physical owner is checked against the captured stock, fraction and lifecycle. A shared forage source revision may advance when the first key is consumed; the next key uses its fresh owner token without re-evaluating conversion formulas against the batch's own changes. Unexpected owner changes after group validation leave the durable parent for staff review. `InspectOrganicProfile` and targeted `TryInspectLandResource` let a Land quote validate profile protection and only declared ambient outputs, without evaluating unrelated output formulas. Environmental pending operations remain a separate quarantine that gathering acknowledgement cannot clear.

Penalty formula inputs are raw scalar/native values, including scar damage, current pressure, current native stock/health/yield/capacity, field condition and the positive baseline contribution. Only dependencies for the requested channel are evaluated. A penalty lookup never calls a mana cap or output evaluation, and the common recursion guard fails closed if a named prog tries to re-enter environmental calculation.

## Central scheduling and time

### Bounded spell treatment

The room/rooms spell effect `rejuvenateland` gradually repairs existing scars through an independent indexed lane of the same coordinator. It can work in full, dormant, zero-output or denuded rooms and when an unrelated output/organic formula is invalid. Admission needs a physical cell, valid scalar ecology, an effective profile and valid repair policy. Profile identity replacement or disabling ends a treatment.

Profiles accept `magicalrepaircap <non-negative-number|none>`, independently of `repair` (natural recovery). Missing/`none` is uncapped beyond the captured spell rate. Zero disables new treatments and terminates existing ones. Malformed/negative/non-finite persisted values fault repair only; they do not disable valid legacy mana outputs. Editing a positive cap settles the old segment at the edit boundary and applies the new cap prospectively. An unobserved custom edit closes its unknown segment without backdating the new rate.

Each treatment captures its spell, caster and acting instance IDs, profile identity, rate, budget capped to existing scars, and finite resolved duration. One physical cell permits one active or unresolved treatment across all spells/casters/layers. Admission happens before any target children or exclusive cleanup, so a refused composite target leaves its prior parent and siblings intact. Costs follow ordinary per-invocation rules; rejection after payment adds no refund. Duplicate cells in one invocation cannot multiply work.

Only eligible online monotonic time earns repair. The one-minute lane clips short/final intervals and delayed visits to saved lifetime and budget. Conservative subtraction never reports a decrement larger than its allowance. Sub-precision earned work is bounded and carried without minting ineffective receipts. Natural settlement precedes explicit repair and never spends spell budget. Reaching zero ends the treatment immediately, including through another ecological operation, so subsequent damage cannot reuse it.

`LandRejuvenationTreatments` stores the authoritative versioned checkpoint; child XML stores its identity only. A step first persists its exact ecological request and sequence. The native ecological transaction then commits scars, balances, receipt and acknowledged treatment budget/lifetime together. A lost response freezes the step under the same ID. The timed lane may retry that exact request after rollback is proven; staff confirmation never replays repair or executes a policy. Foreign pending operations remain owned by their original action. Cancellation persists a tombstone, including when acknowledgement is unresolved.

Saving a parent checkpoints earned online work/lifetime without applying repair. Loading waits for parent/cell attachment and authoritative state, then starts a fresh monotonic epoch. It never grants downtime work or restores a full budget from stale XML. Independent mode never loads its creator. Local or continuation modes require the original active instance; absent/invalid maintenance terminates work. Normal expiry accounts its final interval once. Dispel/manual removal discard uncommitted elapsed work and remove only the treatment child, retaining siblings and permanent completed repair.

Use `magic environment treatments [here|<cell id>]` to inspect captured IDs/rate, current cap, budget, lifetime, repaired total, sequence, pending receipt and diagnostics, including removed unresolved children. `magic environment treatments confirm <here|cell id> <treatment guid>` reconciles durable evidence without applying new repair. Ordinary descriptions, effect inspection and compatibility checks are pure observations.

Example: author budget `12`, rate `2`, duration `600` seconds and profile `magicalrepaircap 1`. With scar `20`, after `120` online seconds scar is `18`, budget `10`, and explicit repair `2`. Dispel preserves scar `18`. With maximum `max(0, 100 - scardamage)` and zero regeneration, capacity increases without adding energy. Native stock/prepaid fractions, harvested items, pressure and last-defile history remain unchanged by repair; only later native recovery reads improved scar-dependent factors. See [the spell editor](Magic_System_Spells.md#bounded-land-rejuvenation) and [acceptance evidence](Land_Rejuvenation_Verification.md).

### Ambient scheduling

One world-local coordinator subscribes once to the existing `SecondHeartbeat`, after cell/magic/forage/agriculture loading, and unsubscribes on disposal. It reuses the existing heartbeat scheduler entry. There are no environmental timers, worker mutation loops, per-cell schedules, or per-output callbacks.

Registration is separate from useful work. A positive rate below maximum produces; natural repair maintains once per cell when scars remain. Full/zero-rate/zero-repair cells become dormant. Pressure decay alone causes no periodic writes. Invalid profiles retain faulted registrations with bounded audit/recheck paths.

Indexed central production/audit sets, one coalesced dirty-list entry per cell, and a bounded discovery cursor divide the work. The discovery cursor includes previously unconfigured cells so terrain defaults can activate them. The pump rotates fairly among five lanes (production, audit, dirty, discovery and spell treatments) without copying, sorting, or scanning the whole population every invocation. Detach/delete/rebind removes obsolete work; dirty marking preserves active deadlines. Stable cell phases spread initial evaluations and idle audits.

An injectable `TimeProvider` separates monotonic online elapsed time from historical UTC age. A prior accepted rate/cap sample owns its interval until the next real evaluation. Native notifications request that evaluation without running progs. Newly detected conditions apply prospectively. Managed mutations evaluate immediately and close the old segment before changing the recorded balance. Pure inspection samples are never accepted as production samples.

This is sampled integration, not analytic evaluation of arbitrary progs. Passive source changes can lag until dirty work is processed; unobservable dormant dependencies can lag until the audit. The previous accepted sample covers that detection interval. New favourable rates/capacity are never backdated across a dormant/zero-rate interval. Budget-delayed active work retains its full unaccounted online interval, settles once, and saturates at the old cap. Full time cannot be banked for a later debit. Duplicate zero-elapsed callbacks grant nothing. Reload, reattachment, and suspension start fresh online epochs; shutdown/detached time grants neither mana nor natural repair.

Bounded rounding remainders retain sub-precision earned increments while online, so very small positive production and repair rates eventually change a representable balance/scar. There is at most one remainder per configured output and one for shared repair. Full outputs, exhausted scars, zero rates, faults, removal, and fresh epochs discard their corresponding remainders. These are transient numerical accounting, not a second spendable pool or a saved downtime clock.

## Invalidation and future integrations

| Owner | Notification |
| --- | --- |
| Managed resource mutations | Immediate affected-cell validation and final eligibility update |
| Damage/repair/pressure | One durable explicit-operation boundary, then cap/eligibility reconciliation |
| Forage consumption/recovery/profile synchronisation | Coalesced `Forage` dirty reason on actual relevant changes |
| Agriculture tick/operations/yield debit/pasture/condition setters | Coalesced `Agriculture` dirty reason on exposed-value changes |
| Field creation/replacement/destruction | Update field index and mark its cell |
| Cell lifecycle/current overlay terrain changes | Register, remove, or resolve the physical cell |
| Terrain defaults and environmental profile edits | Bounded discovery; interactions see the new definition immediately |
| Source definitions and FutureProg compilation/cache-mode edits | Reference invalidation and bounded discovery |
| Unobservable policy dependencies | Rolling audit, shorter idle cadence, or trusted `invalidateenvironment(location)` |

`MarkDirty(cell, reason)` is the subsequent features' narrow wake surface; it coalesces and never awards energy. `Pump()` owns accounted advancement. Capability-configured Gentle gathering uses the exact debit surface and adds no environmental/treatment heartbeat per room. The explicit operation identity/receipt boundary owns quantified scar changes. Unrelated spell-effect expiration and player-action scheduling are unchanged.

## Deployment and evidence

Defaults: `EnvironmentalMagicActiveSeconds=60`, `EnvironmentalMagicAuditSeconds=3600`, `EnvironmentalMagicMaximumCellVisits=1024`, `EnvironmentalMagicMaximumOutputWork=8192`, `EnvironmentalMagicBudgetMilliseconds=5`. These are finite positive deployment budgets, not measured capacity promises. Profiles may shorten idle checks. A soft overrun stops taking further work; synchronous input progs cannot be preempted.

The staff configuration editor rejects invalid scheduler values before saving them. If a database was independently edited to contain invalid budgets, startup retains safe defaults and reports the rejected configuration. Each pump reserves room for one complete profile (up to eight outputs) before admitting another cell, so the output limit is never exceeded even when a profile is replaced during discovery.

`magic environment diagnostics` reports registration/activity/fault counts, queue sizes, evaluations, input progs, actual writes, pump times, budget-limited pumps, ready/audit ages, and representative errors/slow progs. Errors are aggregated and throttled by profile. Regeneration and natural repair use deferred saving; scheduling clocks, pressure projections, and unchanged visits are not writes.

Structural tests use controlled time and actual cells, loaders, resources, and heartbeat delivery. The opt-in harness records hardware/runtime, warm-up, population/output/policy matrix, allocations, callback work, saves, latency, throughput, and ages. See the [measured performance and supported envelope](Environmental_Magic_Performance.md), [requirement mapping](Environmental_Magic_Acceptance.md), and [executed verification](Environmental_Magic_Verification.md). The 30,000-cell fully active heavy cases exceed strict minute cadence under the default soft budget; this measured limitation is explicit.
