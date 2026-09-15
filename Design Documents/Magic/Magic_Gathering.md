# Capability-configured Self and Gentle gathering

Gathering is an optional, capability-owned, timed magic action. A capability with no `Gathering` XML section has no gathering methods; no class, school, race, resource generator, spell or passive effect implicitly grants one. This feature implements only `Self` and `Gentle`. It does not add land gathering, channelling, yield consumption, ecological restoration, spell practice, or a Vancian/spell-backed-power route.

The separate [acceptance map](Magic_Gathering_Acceptance.md) identifies the automated coverage and its boundaries. See [Environmental Magic Runtime](Environmental_Magic_Runtime.md) for the environmental source contract.

## Capability authoring

Open a normal skill-level or Vancian-derived capability in the existing capability editor and use its `gather` surface:

```text
magic capability edit <capability>
magic capability set gather list
magic capability set gather add self <alias> <presentation name>
magic capability set gather add gentle <alias> <presentation name>
magic capability set gather show <method>
magic capability set gather remove <method>
```

The full `set` form is:

```text
magic capability set gather set <method> destination <resource>
magic capability set gather set <method> source <resource>
magic capability set gather set <method> min|max|duration|ratio <number>
magic capability set gather set <method> stamina|minstamina|damage|pain|stun <number>
magic capability set gather set <method> healthseverity <severity>
magic capability set gather set <method> permission|durationprog|staminaprog|damageprog|painprog|stunprog|onsuccess <prog|none>
magic capability set gather set <method> alias|name <text>
```

`destination` must name a player-capable magic resource. A `Gentle` method also needs a location-capable `source`; `Self` has no source. Amount bounds and duration must be finite and positive. Costs must be finite and non-negative. A Self method needs a real bodily cost after its cost progs are evaluated; an accidental all-zero configuration is unavailable rather than a free generator.

`ratio` means **source units consumed per destination unit credited**. It defaults to `1.0`. For example, an amount of 10 with a ratio of 1.5 requires an exact 15 source-unit debit and grants exactly 10 destination units, or grants nothing.

Each method has a stable GUID key and a structural version. Reordering or renaming does not change its identity. Changing any resource, amount, duration, cost or prog setting increments the version and invalidates a live precommit action. A clone receives fresh gathering method GUIDs; it never inherits a live action. Vancian clone remapping is intentionally scoped to its repertoire/allowance subtree and the gathering subtree separately, so their unrelated `key` attributes cannot corrupt one another.

Malformed gathering XML is retained and reported as a gathering configuration error. It neither grants a free method nor silently deletes the malformed authoring. A missing section remains the legacy empty-method behaviour.

## Programs and calculation context

Policies are evaluated for every quote; gathering does not cache state-dependent permission or numeric programs. The immutable argument order is:

| Hook | Exact signature |
| --- | --- |
| Permission | `boolean(character actor, character owner, magiccapability capability, text methodKeyText, number amount, location location)` |
| Duration/stamina/damage/pain/stun | `number(character actor, character owner, magiccapability capability, text methodKeyText, number amount, location location)` |
| Post-success | `void(character actor, character owner, magiccapability capability, text methodKeyText, number actualAmount, location location, text operationIdText)` |

`actor` is the captured active character instance and `owner` is its canonical identity owner. A failed, recursively re-entered, uncompiled or wrongly typed policy fails closed. Numeric results must be finite; duration is positive and costs are non-negative. The post-success program is not a quote-time policy and is not invoked by previews, refusals or cancellation.

## Player and script surfaces

Gathering remains within an owned school's existing player/NPC verb; there is no global `gather` command:

```text
<schoolverb> gather <capability> methods
<schoolverb> gather <capability> preview <method> <amount>
<schoolverb> gather <capability> <method> <amount>
<schoolverb> gather cancel
```

Capability names with spaces work when quoted. `methods` only lists methods actually present on a capability currently held by the actor. A school alone does not grant gathering. Preview reports destination gain, exact source debit where relevant, bodily costs, and real-time duration without revealing privileged environmental diagnostics.

FutureProg uses the same service and cannot bypass its token, quote, timing, ownership or debit checks:

| Function | Return | Parameters |
| --- | --- | --- |
| `canmagicgather` | boolean | character, magiccapability, text method, number amount |
| `beginmagicgather` | text token or empty | character, magiccapability, text method, number amount |
| `completemagicgather` | boolean | character, text token |
| `cancelmagicgather` | boolean | character, text token |
| `magicgatherstatus` | text status or empty | character, text token |

## Quotes, actions, and interruptions

Preview, `methods`, and action start use a full quote. A quote validates the actual actor, canonical owner, held capability, method identity/version, requested full amount, destination headroom, bodily capacity, policy outputs and (for Gentle) the current local source. It also checks ordinary `general` and `movement` action blockers from both the character and current body. It creates no receipt, balance, source registration, environmental dirty mark, scheduler deadline, production sample, pump, save or callback.

Starting captures the actor, canonical owner, body, full spatial location, capability/method key and version, amount, prices, duration, source cell/profile identity and profile definition revision. The live preparation uses a monotonic clock and is transient only. It cannot resume after a restart.

At completion the complete quote is evaluated again. A changed body, spatial location/layer/route position, capability, method definition, focus relationship, policy, cost, source binding/profile, body capacity, destination headroom, sleep/incapacity/death state, action blocker or source stock cancels before payment. Movement, combat engagement, layer changes, body changes, logout, deletion, death and normal action/state interruptions remove the preparation. During the wait, the service ignores only the exact live `MagicGatheringTimedAction` while checking those blockers; a separate character or body effect still interrupts it. Only one live action may exist for a canonical identity across every capability and instance.

Routine environmental production, natural scar repair and normal balance changes are not configuration changes. A source profile removal, replacement, disablement or incompatible definition version is. Gathering does not reserve room stock while waiting: competing actions are resolved at controlled completion and only fully funded actions may commit.

## Self and Gentle commitment

Self never inspects or mutates environmental state. It converts configured bodily costs into an exact personal credit. Stamina preserves the configured minimum remainder. Damage, pain and stun are independent optional native-health channels. Health-priced methods require a compatible strategy and an explicit maximum allowed severity.

The selected health strategy must explicitly support direct independent health costs. Its side-effect-free planner evaluates compatible body parts in stable ID order, validates its native modifiers, existing cellular wound compatibility, local and total condition caps, severity ceiling and relevant active pain/consciousness modifiers, then captures one target part and (where applicable) the exact existing wound. It prices the native inputs that produce the quoted visible units; a strategy or state that cannot make that prediction safely is refused before stamina, source or destination changes. The captured plan is revalidated while waiting and immediately before payment, so a changed target/body condition cancels rather than choosing another random part. The supported organic strategy applies the cost as `Cellular` damage through its direct plan, preserving all damage, pain and stun channels without armour-negatable hostile damage semantics. Each applied channel must change by the exact quoted amount, and the destination credit is also checked after mutation; an unsupported body, immunity, clamp, amplification or lifecycle refusal is not a successful payment.

Gentle begins with one pure `IEnvironmentalMagicService.Inspect(cell)` snapshot and resolves only the explicitly configured source in the captured physical cell. It requires a managed, valid output and finite non-negative recorded `Balance`/`Maximum`; spendable stock is `min(Balance, Maximum)`. It neither estimates nor materialises pending production. A full dormant room or zero-rate room with recorded stock remains eligible; an empty, unbound, disabled, invalid or replaced binding does not fall back to legacy resource spending.

After fresh full-plan validation, Gentle calls `TryDebit(cell, source, quotedSourceDebit, out error)` exactly once. It never uses raw cell setters, negative `Add`, `Set`, a generator, `Pump`, a fabricated ecological operation, yield conversion, scar damage or pressure. Coordinator-owned settlement may legitimately change other environmental state at that debit, but receipt source debit is the transfer accounting authority and is not inferred from a raw room balance delta.

## Receipts, persistence, and staff recovery

Before the first gathering-caused debit or bodily cost, the service creates a durable `MagicGatheringOperations` marker. It records the captured identities, full amounts, source debit, cost channels, completion flags, diagnostic and UTC timestamps. It is not another spendable resource pool and offers no automatic refund or replay.

| Status | Meaning |
| --- | --- |
| `Committing` | Durable pre-effect marker exists. |
| `Cancelled` | A final exact Gentle debit refused before any transfer effect. |
| `Invoking` | Transfer accounting was saved and the post-success hook is awaiting acknowledgement. |
| `Completed` | Required accounting and any callback acknowledgement were saved. |
| `NeedsReview` | An effect, persistence boundary, callback or final acknowledgement became uncertain. No payout, refund or callback is retried automatically. |
| `Acknowledged` | Staff recorded review; it deliberately does not replay a payout or callback. |

After a bodily health cost is applied, the service persists exactly the native wound objects it changed before it records `BodilyCostApplied`, credits a destination or saves final accounting. Newly created wounds use native late initialisation; existing wounds are saved in a bounded isolated checkpoint. This does not perform a global save-manager flush, including for stamina-only gathering. If that checkpoint fails, the wounds remain dirty and the durable operation is quarantined with no payout or replay. This boundary is independent of `OnGathered`.

After source debit, bodily costs and destination credit, the service verifies each effect and saves the actor/body/cell accounting and receipt in the controlled persistence boundary. A credit clamp, failed save, failure after debit, callback failure, or lost final acknowledgement is quarantined rather than reported as a normal complete result. Component dirty flags are restored if final accounting fails so normal saving can recover the actual state; the receipt remains authoritative until staff review. An unresolved receipt blocks its canonical owner. For Gentle it also temporarily quarantines the affected `(cell, source resource)` pair, preventing a later gatherer from normalising an uncertain source with another transfer. Staff may inspect and acknowledge without side effects:

```text
magic gathering unresolved [<character-id>]
magic gathering show <operation-guid>
magic gathering acknowledge <operation-guid>
```

`OnGathered` executes once only after transfer accounting succeeds. Its register effects are flushed before the receipt becomes `Completed`. Failure to acknowledge that hook leaves a distinguishable review state; acknowledgement never reruns it.

## Disposable-world authoring checklist

Use a disposable database/world, not an existing game database. Create a location-capable resource for the Gentle source and a player-capable resource for the destination. Create and bind an `environmental` profile through `magic regenerator ... environmental` and `magic environment ...`, place recorded source stock in a physical cell, author a capability method, and grant that capability through the ordinary game-specific character path. Then use a non-admin account to exercise:

1. Self completion and the configured stamina-only price.
2. Gentle completion at a ratio such as 1.5.
3. Movement/layer interruption, source exhaustion and destination-headroom refusal.
4. Server restart while a precommit action is live, confirming no resume or credit.
5. A health-priced method with both a new and existing cellular wound, confirming the exact body condition changes survive a restart; then repeat with an unsupported strategy or a changed target/body condition and confirm no debit or credit.
6. A `general` and a body-owned `movement` action blocker at both start and during the timed wait, confirming no payment; then a normal action to confirm its own gathering blocker does not cancel it.

Automated coverage exercises stamina, captured compatible health plans, native `Cellular` wound tracking and the bounded wound checkpoint for independent damage, pain and stun. It is not a claim that every live health strategy has been accepted; builders must validate their selected strategy and nonlethal configuration in the disposable world.
