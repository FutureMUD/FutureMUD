# Capability-configured Self, Gentle and Land gathering

Gathering is an optional, capability-owned, timed magic action. A capability with no `Gathering` XML section has no gathering methods; no class, school, race, resource generator, spell or passive effect implicitly grants one. The three explicit routes are `Self`, `Gentle` and destructive `Land`. Land converts only configured local ambient or profile-authorised native sources into personal credit, with a positive ecological scar. It adds no restoration, spell practice or spell-backed-power route.

The separate [acceptance map](Magic_Gathering_Acceptance.md) identifies the automated coverage and its boundaries. See [Environmental Magic Runtime](Environmental_Magic_Runtime.md) for the environmental source contract.

## Capability authoring

Open a normal skill-level or Vancian-derived capability in the existing capability editor and use its `gather` surface:

```text
magic capability edit <capability>
magic capability set gather list
magic capability set gather add self <alias> <presentation name>
magic capability set gather add gentle <alias> <presentation name>
magic capability set gather add land <alias> <presentation name>
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

After fresh full-plan validation, Gentle checks both the legacy source receipt and indexed Land `ambient:<id>` participant immediately before commitment, then calls `TryDebit(cell, source, quotedSourceDebit, out error)` exactly once. A Land parent that becomes unresolved during Gentle's wait therefore blocks payment even if its ecological child is confirmed. It never uses raw cell setters, negative `Add`, `Set`, a generator, `Pump`, a fabricated ecological operation, yield conversion, scar damage or pressure. Coordinator-owned settlement may legitimately change other environmental state at that debit, but receipt source debit is the transfer accounting authority and is not inferred from a raw room balance delta.

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

## Destructive Land methods

Land remains a capability method and uses the same player commands, duration and body-price policies above. Its ordered source list has one to sixteen entries across funding and mandatory collateral. Builder commands are `magic capability set gather land <method> source add <selector> <units-per-mana>` and `... collateral add <selector> <units-per-mana>`. A selector is `ambient <resource>`, `forage <key>`, `crop`, `woodland`, or `pasture`; orchard uses the crop identity. `source <index> remove|move|ratio|ratioprog|optional` edits an entry. `damage`, `pressure`, `damageprog`, `pressureprog`, `crophealth`, `woodlandhealth`, the corresponding `*prog` settings, and `message actorstart|observerstart|actorcomplete|observercomplete|actorcancel|observercancel` configure the remaining Land terms. The builder show/list views expose configured entries and quote failures.

Every entry's number is logical local source units per destination unit, not a raw whole native debit. Dynamic ratio progs receive the ordinary six gathering arguments plus the stable source-entry key as text. Dynamic scar and pressure progs receive the six arguments plus a number dictionary. Its `funding:<selector>`, `collateral:<selector>`, and `total:<selector>` keys contain logical units for each declared source, including zero for a legitimately unused source. `damage` is a positive static scar coefficient per destination unit; `pressure` is non-negative and defaults to the scar coefficient when unset. Dynamic results replace those coefficients. The ecological child receives one total scar and pressure request; it does not convert unlike vegetation units into one implicit sum. An optional source can be absent only when its declaration is valid and it is explicitly marked optional; collateral is always mandatory.

For example, a wild-plant method can reserve one unit of `forage:herbs` collateral and then fund five personal units from `forage:berries`, with positive scar. Both forage keys are debited from the same physical cell. An agricultural method can fund one personal unit with `ambient:1` at 0.5 units and `crop` at 0.5 units, while also charging crop health. Crop, woodland and pasture accounting may prepay a whole native unit and retain an unused decimal fraction on the field. A later action spends that fraction only while the same crop or field lifecycle remains valid. Killing the crop discards its remainder; it cannot transfer to a replacement. Quotes and timed preparations do not debit or advance native owners. At completion, the exact captured logical mix is repriced against fresh native plans; harmless growth does not select a new, cheaper source order.

The effective physical-cell environmental profile must explicitly authorise every native selector and have valid organic configuration. Its protection prog uses the same actor and canonical character owner as gathering permission. An unresolved ecological operation or invalid dynamic penalty refuses Land before costs. Ambient outputs used for funding are checked individually; unrelated output formulas are not needed for a native-only quote. At commitment the coordinator validates the complete bounded ambient and native group before the first gathering-caused mutation. Its own ambient payment cannot revoke a native or second ambient entry's already-approved opening formula; later independent actions evaluate the resulting environment afresh. Native owner arithmetic and lifecycle checks remain exact, while unexpected partial payment remains in a reviewable receipt. Source scarcity, lifecycle replacement, field assessment pending, changed body price, missing destination headroom or policy refusal prevents credit.

Land requires a positive scar addition that actually increases the stored binary64 scar at the current magnitude. Quote and completion refuse an ineffective addition before payment, and the ecological child defensively confirms the representable increment. Natural repair settled before the child is separate from Land damage. Land first persists one parent receipt with indexed `(cell, source)` participants, then debits ambient and native owners, applies optional vegetation health loss, and records one linked ecological child. The parent stores versioned captured/applied source allocations, whole debit, opening/closing prepaid fractions, health loss and discarded fractions, ecological amounts and stage. The physical source checkpoint precedes body costs and personal credit. An unresolved parent quarantines its owner and affected sources. A pending ecological child independently blocks the cell's ecological route; acknowledging the gathering receipt cannot clear that separate freeze. Nonoverlapping sources remain available when the ecology route is resolved. Staff acknowledgement records review but cannot retry, refund or grant. Existing Self/Gentle receipts remain readable. `magic gathering show <guid>` includes Land accounting. The owner-only `magicgatherdetails(character, text)` returns a read-only number dictionary with `funding:`, `collateral:`, `total:`, `paid:`, `wholeDebit:`, `openingPrepaid:`, `closingPrepaid:`, `healthLoss:`, `discardedPrepaid:`, `scar`, `pressure`, `credited`, `ecologyApplied`, and `accountingPersisted` keys. An unrelated token returns an empty dictionary. Repeating a detail query or completion cannot execute another transfer.

When the effective environmental profile has `organic scaraddendum on`, the visible addendum is derived from the cell's current scar and pressure bands at look time, after normal visibility checks. It is off by default and does not rewrite a builder's base room prose or reveal who performed a remote action. Same-cell Land commits serialize their shared ecological participant; an unresolved receipt quarantines its actual source owners, while the independent environmental child freezes the cell only if its ecological outcome is pending. Land adds no per-cell heartbeat or global source scan; it uses the coordinator's physical cell and field index. The feature does not cover remote sources, inventory or animal drain, rejuvenation spells, or automatic replay of uncertain effects.

### Task 3B native verification record

At the working-tree base `0f0d242b9968940dad1cc117a5e62b3a5458f738`, the 20 September 2026 `Run-IsolatedAcceptance.ps1 -LandOnly` run used a fresh marker-owned `futuremud_land_20260920045109_a7701191f0` database on an isolated MySQL 8.0.45 loopback instance. The harness completed in 41,695 ms, reported `nativeHarnessExit=0`, deleted only its owned database, and shut down and removed only its temporary MySQL instance. World catalogues and weather are controlled fixture inputs; `Cell`, `AgricultureField`, environmental coordinator, gathering service, receipt stores, `SaveManager` and EF/MySQL reads are production paths. The Land player command probe calls the real `MagicModule.MagicGeneric` adapter with a character at player permission; command-tree dispatch is separately exercised in the core unit suite.

The real mixed operation `9b3fbeea-0d01-4389-bf30-8e48f8aada15` used cell 9 and field 7: crop prepaid fell from 0.50 to 0.25, forage herbs from 89.75 to 89.25, personal credit became 1.00, and the separate ecological child was `11441f2d-210c-4cf7-b937-4fb5be187f93`. Operation `e56fd4c6-af9e-409a-bb48-e673492b409b` paid two forage keys in the same cell, herbs 0.25 to zero and berries 100.00 to 99.25, reaching credit 2.00. Operation `f8ab2c12-7793-4918-969e-19df9bbc9a1b` paid 0.50 ambient plus 0.125 crop prepaid, leaving crop prepaid 0.125 and credit 3.00. Fresh database contexts checked each success before unrelated flushing.

Two further native body-priced Land operations used no notification callback and brought the same wound to damage/pain/stun 3/4/5 and then 6/8/10, with credit 4.00 and 5.00. A separate reader process reconstructed that character, wound, crop fraction, supplies and linked receipts; repeating completion refused. A player command operation `c906e98c-2d95-4e55-a9bf-9c1e7e65aaad` raised credit to 6.00 and scar to 19.00; forage recovery factor was 0.05. An existing explicit repair lowered scar to 1.00 and restored the future factor to 0.95, without immediate forage or personal credit. Overdraw and interruption refused. Turning off a live pollination candidate changed the delivered crop-health baseline from 4 to 1, made the dynamic factor invalid and refused another player Land command; native stock, prepaid fraction, revision and credit stayed at 9, 0.125, 5 and 6.

The successful Land crop operation `e71c82dc-e067-4dc1-abfd-86d21ac4f513` left 0.875 prepaid, lowered native crop health from 49 to 48, and reached credit 7. An ordinary craft reserved five native crop units, harvest discarded the 0.875 fraction, and replanting the same definition advanced generation 1 to 2 with zero prepaid; both the old debit plan and a timed gathering action against the old generation refused. A controlled MySQL trigger then rejected operation `8580f56a-c9c8-4f36-b056-e1b1aad93889` at the field owner checkpoint. An independent context observed crop stock 100, prepaid zero, unchanged credit 7, a `NeedsReview` parent and one linked ecological child. After removing the owned trigger, flushing the already-applied owner persisted stock 99 and prepaid 0.875 while credit remained 7 and replay refused. The subsequent zero-stock probe `ee6085bf-5229-4f1d-837a-66b406e70fd5` drew only 0.125 prepaid, leaving whole stock zero, prepaid 0.750 and credit 8. This is deterministic provider failure evidence, not a power or network failure claim. The schema change is migration `20260920025846_LandGatheringSourceAccounting`; it adds versioned Land detail, ecological child identity and indexed participant rows. The blank snapshot and manifest carry the matching fresh-install schema.

The final complete `Run-IsolatedAcceptance.ps1` run used separate owned `futuremud_gather_gc_20260920051633_6e71e81d9b` and `futuremud_land_20260920051717_7b92d246e8` databases on MySQL 8.0.45. It passed legacy native wound cases GC-P01 to GC-P03, Task 3A native cases, and Land L-P01 to L-P06 plus L-T15, L-T16, L-T33 and L-T37. The vegetation cap probe `27cf9a29-27e4-46ca-834e-a5ab950054be` paid 0.50 ambient collateral before crop health fell 49 to 48; the effective ambient maximum and balance were 48, while credit rose only to 7. The field checkpoint probe `3df5b6d9-3e25-4a39-b3fa-53adc21ccb7b` left persisted crop stock at 100 and credit at 7 on provider rejection, then retry saved stock 99 and prepaid 0.875 without replay. A separate forage row update failure preserved its specialised dirty flag and queue; retry saved berries 96.25 to 95.25 without extra credit or child. The final growth probe `906cb3f7-9cdd-4ff7-b54a-56c064ca5727` used a daily tick to grow crop stock 0 to 1 during the timed action, then paid only the captured 0.125 crop fraction and reached credit 9. The complete runner reported `nativeHarnessExit=0`, deleted both owned databases, and shut down and removed its temporary MySQL instance.

### Task 3B correction verification record

On 20 September 2026, correction source based on checkout `69ca42f45cd359c860d881cc0e644bd6782caf66` passed `dotnet build MudSharpCore/MudSharpCore.csproj -c Debug -m:1 --no-restore` and the matching Release build, each with zero warnings and errors. The focused `dotnet test "MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj" -c Debug -m:1 --no-restore --filter "FullyQualifiedName~Gathering|FullyQualifiedName~EnvironmentalMagic|FullyQualifiedName~NativeOrganic|FullyQualifiedName~SaveManagerFailureRecovery|FullyQualifiedName~SimpleLivingDirectHealthCost|FullyQualifiedName~MagicFutureProg|FullyQualifiedName~MagicModule"` passed 261/261; the same Release command passed 261/261. `dotnet test "MudsharpDatabaseLibrary Unit Tests/MudsharpDatabaseLibrary Unit Tests.csproj" -c Debug -m:1 --no-restore --filter "FullyQualifiedName~MagicGatheringOperationModel"` passed 5/5. `scripts/test-unit.ps1` passed 5,730 tests across nine projects: shared library 505, expression 36, seeder 1,380, core 3,596, persistence library 60, bot 22, converter 43, web 54 and TerrainPlanner 34. `git diff --check` passed. No schema change or migration was needed for the existing legacy source column and indexed participant table.

The final full `Temporary Scratch App/GatheringNativePersistenceHarness/Run-IsolatedAcceptance.ps1` run used fresh marker-owned `futuremud_gather_gc_20260920120200_42501e88a9` and `futuremud_land_20260920120241_f9a8aeff7d` databases on an isolated loopback MySQL 8.0.45 instance. It passed the earlier GC, 3A, and Land probes and all three correction probes with `nativeHarnessExit=0`. Independent EF readers observed `C3B-P01`: a `NeedsReview` Land parent with null legacy source, one indexed ambient participant and one confirmed child, no pending child, and the waiting second actor's persisted ambient balance 4 to 4, credit 0 to 0, wounds 0 to 0, with no Gentle accounting row. Its owned MySQL trigger rejected the parent acknowledgement after ecological confirmation; this is provider fault injection, not evidence of operating-system crash or network-partition recovery.

`C3B-P02` completed one initially valid mixed ambient/forage group: ambient 5 to 0, berries 5 to 0, personal credit 10 to 20 and one confirmed ecological child; a later independent gather refused the now-invalid factor. The unit reproduction uses `forage:herbs`; the persisted native fixture uses `forage:berries` with the same production-factor dependency. `C3B-P03` independently observed no transfer at scar 1,000,000,000,000 with requested damage 0.000001, then after explicit repair observed an affordable scar increase of 0.000001, ambient 2 to 1, credit 9 to 10 and one child. Each native success was checked before an unrelated global flush. The runner deleted only its two owned databases and gracefully shut down and removed only its owned temporary MySQL instance. Controlled world catalogues and weather remained fixture inputs; source mutation, compiled formula, receipt index, native save, gathering credit and independent database observations used production paths.

The corrective commit was then rebased onto current `master` `174ed2a9a60bd1c53ef909498ef2c3281865cb7e`, which changed repository test reporting and guidance but none of the gathering, coordinator or native harness files. On that rebased source, `scripts/test-unit.ps1 -OutputMode Compact` reported `PASS` with 5,747 passed, zero failed, zero skipped across ten projects and `source stable: True`. Its saved receipt is `.artifacts/test-runs/20260920T120500Z-3efdaea2bbda/summary.json` (local generated evidence, not committed). The native observations above were executed on the identical gathering and harness source immediately before the rebase. The added TestReporting suite accounts for the difference from the earlier nine-project gate.
