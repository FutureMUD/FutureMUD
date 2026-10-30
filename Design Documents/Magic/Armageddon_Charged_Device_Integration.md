# Charged devices: runtime and installed-world checkpoints

## Worldfile compatibility audit — 30 October 2026

The mapping below was approved after the compatibility audit. The current tranche adds optional native reservoir devices and imports the four functional placed staves. The original frozen-charge mode remains available. Walkthrough deployment is separate from this implementation.

### Actual supplied devices

The pinned worldfile package has 25 type-3 wands and 17 type-4 staves. Typed O/G/E/P references outside excluded zones 57/76 identify ten distinct definitions (eleven reset rows because Vnum6310 occurs twice). None is directly listed in the packaged shop production/new-item arrays, forage entries or the sole craft's inputs/outputs. This does not rule out every dynamic script supply route.

| Vnum | Source item | Type | Maximum/current Mana | Maximum power field | Spell / effective reset state |
| --- | --- | --- | --- | --- | --- |
| 25109 | a gnarled thuja staff | Staff | 2000 / 1934 | 8 | Rain of Fire (21) |
| 25606 | a mechanical steel cannon | Staff | 2000 / 2000 | 7 | Fireball (11) |
| 25914 | a dragon-headed obsidian staff | Staff | 3000 / 3000 | 5 | Animate Dead (28) |
| 65509 | an iron torc | Staff | 500 / 500 | 8 | Banishment (45); P reset in zone 13 |
| 6310 | a twisted agafari crook | Staff | 0 / 0 | 0 | No spell |
| 30037 | the mantle of orbs | Wand | 0 / 0 | 0 | Levitate (26), but no capacity or usable power |
| 30043 | the world helm | Wand | 0 / 0 | 0 | No spell |
| 30044 | the crown of insight | Wand | 0 / 0 | 0 | No spell |
| 30045 | a crystal wedge | Wand | 0 / 0 | 0 | No spell |
| 45430 | a flower-tipped wand | Wand | 0 / 0 | 0 | No spell; P reset in zone 45 |

Base and effective reset values agree for these records. Values 0/1/2/3 mean maximum Mana, maximum power, current Mana and spell ID. None has the source `[ARTIFACT]` marker. Reset extraction must include P records and distinguish G's three-field prefix from E/P's four-field prefix; raw argument searches or G/E-only scans give incorrect counts.

### Source activation and recharge

`other.c:3370+` donates a positive integer amount of personal Mana into an equipped wand/staff at 1:1, refusing insufficient personal Mana or a resulting reservoir above capacity. It does not require acquired knowledge of the stored spell or use casting components. `handler.c:1942+` searches every equipment slot, including worn jewellery; the message about holding the item is narrower than its actual lookup.

`other.c:3490–3714` uses an equipped item and a quoted power word. The seven words are wek, yuqa, kral, een, pav, sul and mon. Although the Power array is zero-based, `parser.c:260+ old_search_block` increments its match counter before returning: invocation levels are **1–7**, not 0–6. Therefore maximum-power field 8 admits all seven words, but is not an executable eighth grade. It must still be retained for the cost calculation.

Wands resolve one local character or local/carried/equipped object. Staves pass no named target; each spell's staff branch determines the recipient set. There is no acquired-spell requirement or generic item-control skill roll. Silence/Babble prevents a valid power word. Reservoir debit occurs before the spell handler, so handler-level refusal can still consume energy. Native wards, resistance and delivery restrictions should remain rather than copying unsafe legacy dispatch.

Wand cost is `600 + 200*(level-1)`. Nonartifact wands clamp maximum capacity to at most `1100 + 100*maximumPower` before checking affordability, then lose another 100 capacity on a paid activation. The stored current energy is not clamped with the maximum. Staff capacity does not erode. Wands impose a one-pulse wait; the staff path adds no equivalent wait here.

Staff cost is `10 * mana_amount_level(maximumPower, level, spellId)` (`sp_parser.c:1934+`), including integer division and the spell's minimum-Mana floor. The helper first increments maximumPower. Exact costs by legal word, useful for a proposed reservoir mode:

| Device | wek | yuqa | kral | een | pav | sul | mon |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 25109 / 65509 | 50 | 60 | 70 | 80 | 100 | 120 | 160 |
| 25606 | 200 | 200 | 200 | 200 | 200 | 200 | 250 |
| 25914 | 80 | 100 | 120 | 160 | 250 | unavailable | unavailable |

### Native payload compatibility

Existing `ChargedMagicDevice` has a homogeneous bank of fixed-grade frozen spell charges (capacity 1–100), optional activation eligibility/checks, persistent reservation/consumption and explicit focus mode. Wand/staff kind currently changes presentation/default capacity, not target fanout. Release accepts one reachable target and rejects groups. Recharge requires an acquired casting route, pays normal native spell costs and components per charge, and requires reproducible stored potency. Instances and copies start empty; there is no seeder-ready authored reservoir or initial-payload constructor. Initial stock needs explicit authored device provenance, not a fictitious character producer.

All four functional source payloads need native carrier work, not merely item XML:

| Source payload | Source staff behaviour | Native equivalent and blocker |
| --- | --- | --- |
| Rain of Fire | Requires open sky; room attack with additional source caster/down-room consequences | Cinder Rain (`arm.spell.cinder_rain`): configured area recipients, identity-safe damage and retained burning. Device release has no area route; stored compatibility rejects identity-safe damage and burning. |
| Fireball | Attacks every eligible other non-immortal occupant, including allies | Ember Lance (`arm.spell.ember_lance`): native single-target damage exists, but its identity-safe prepared-victim mode is explicitly excluded from stored delivery. Staff fanout is also absent. |
| Animate Dead | Tries each corpse directly in room contents | Raise Servitor (`arm.spell.raise_servitor`): exact-corpse lifecycle and control exist, but `animatecorpse` is excluded from device/snapshot compatibility; room-corpse selection needs an adapter. |
| Banishment | Tries each eligible other non-immortal occupant | Banish Traveller (`arm.spell.banish_traveller`): resistance and weighted travel exist, but prepared travel and post-payment destination sampling are excluded from stored delivery; staff fanout needs an adapter. |

Source staff dispatch is in `spells.c:123+`, `298+`, `2431+`, `4384+`. Rain of Fire's underlying source effect (`magick.c:11569+`) can also hurt the caster for one-third damage, affect a room below, burn equipment, and apply chance-based immolation. Installed Cinder Rain instead uses its already-authored native damage/burning and local-other recipient policy. Reusing that payload is a deliberate adaptation, not exact source parity.

Native gates: `MagicCastingService.Devices.cs:33–48` combines a device-effect whitelist with `ScrollSpellCompatibility.Errors`; `StoredSpellSnapshot.Devices.cs` captures a producer-bound frozen payload. `ScrollSpellCompatibility` explicitly refuses identity-safe damage, burning, animatecorpse and banishtraveller. Removing whitelist entries alone would not provide the required prepared targets, selected grade, retained context or lifecycle.

### Approved mapping

1. Prefer a reusable optional **energy-reservoir mode** within native devices over converting these four staves to fixed charges. Preserve per-instance current/maximum energy, fixed payload, selectable grades 1–7 and configurable per-grade costs; reuse existing custody, journal and persistence protections. Existing frozen-charge devices retain their mode. Seed the source integer reservoirs/costs and use 1 native Mana per donated source unit. This preserves pooled donations and weaker/more numerous versus stronger/fewer uses.
2. Use native `Anyone` activation eligibility without an extra control roll, and a configurable equipped-item access policy so the torc works while worn. Keep speech, physical manipulation, no-magic, wards and resistance. Donation needs available Mana, not acquired knowledge of the payload; no focus fallback or spell-learning grant.
3. Preserve source staff recipient scope through reusable character-area and room-corpse delivery adapters, with one device debit per activation and per-target eligibility/resistance. Fireball and Banishment include eligible allies and exclude user/staff. Do not turn the cannon into a single-target wand or animate only one corpse silently.
4. Reuse installed native spell effects and formulas for the initial tranche. Explicitly retain Cinder Rain's existing adaptation unless separately approved to restore source self-damage, downward spill and equipment burning. Preserve its source open-sky activation requirement.
5. Use native preflight refusal without spending energy when the command/location is invalid or no eligible recipients exist; once committed, spend once even if every eligible target resists. This intentionally improves on the source's debit-before-handler refusals.
6. Leave the six zero-capacity/invalid devices inert and do not invent supply for unplaced definitions. They are not merely empty rechargeable magic devices. Defer wand capacity erosion until there is an admitted functional wand to exercise it, or explicitly approve a reusable degradation policy now. No placed functional staff needs erosion.

The approved tranche includes all four functional staves, including corpse animation and multi-recipient banishment. The maximum-power-8 field affects the cost profile, but exposes only the seven actual words. Wand capacity erosion remains deferred.

Evidence is source/code and pinned-data inspection only. No runtime tests or live database operations were performed for this audit.

Authority: `Armageddon_Magic_Completion_Implementation_Brief.md`, Library
`libfile_a4606cd0097081918d6c9d9e220e6da0`, full supported text read (905 lines),
sections N22/N23. Base: `1ad24d636925da64e7a82ffa858b0af470e56b0a`.
That historical isolated lane did not edit the central ledger, repertoire tables or native dispatch. The later N22 installed-world qualification below updates the continuation ledger and ordinary command registration.

## Builder and player use

### Energy reservoirs

`ChargedMagicDevice` now has an optional `EnergyReservoir` mode. It references one native spell and one resource, an initial and maximum energy amount, and a positive integer cost for each admitted grade. Each instance persists its current energy and reservation with the existing checksum and compare-and-swap persistence. Newly loaded prototypes use configured initial energy; copying an existing item produces an empty reservoir. Incompatible mode/capacity revisions refuse without discarding stored state.

```text
magicdevice show <item>
magicdevice use <item> <grade or power word>
magicdevice donate <item> <whole amount>
```

Grades 1–7 also accept `wek`, `yuqa`, `kral`, `een`, `pav`, `sul`, `mon`. Donation transfers the configured resource at 1:1 and refuses requests beyond capacity or the donor's available balance. Reservoir use does not require acquiring the spell and does not teach it. The Armageddon prototypes use `Anyone` eligibility and no control check, with equipped access including worn items. Native physical and speech restrictions still apply.

Builder settings are `reservoir mode energyreservoir`, `reservoir spell <spell>`, `reservoir resource <resource>`, `reservoir energy <maximum> <initial>`, `reservoir cost <grade> <cost>`, `reservoir clearcosts`, `reservoir worn yes|no`, `reservoir delivery othercharacters|roomcorpses|nativearea`, and `reservoir opensky yes|no`. Delivery uses native effect preparation and eligibility, rather than expanding the frozen scroll whitelist. One journalled debit covers the whole activation. Invalid commands, unavailable grades, invalid locations and empty eligible target sets refuse before spending; committed resistance or ward outcomes do not refund energy.

The worldfile seeder installs the four mapped placed staves on base/effective prototypes and already-created opening stock. It leaves the six inert devices and unplaced definitions without invented magic supply. Source cost curves and initial energy are recorded above. The Cinder Rain staff requires open sky and retains the installed spell's native damage/burning adaptation. Wand erosion is not implemented in this tranche.

Reservoir configuration currently admits charged-only roles without an additional control check. Qualified delivery adapters cover identity-safe damage, native damage/burning area policies without chain-lightning shared sampling, durable corpse animation and weighted banishment. An arbitrary spell or additional caster payload does not become compatible merely by assigning its ID. The original frozen-charge whitelist is unchanged.

Implementation verification: runtime and seeder builds pass. The focused seeder suite passes six tests, including exact costs, initial capacities, four active mappings, six inert exclusions and neighbouring rune/bandaging regressions. The final focused runtime selection passes 183 tests. Runtime regression receipts are under `.artifacts/walkthrough/reservoir-*`; they cover old charge banks, persisted energy and reservations, empty copying, revision compatibility, resource-callback custody changes, worn donations, no-target refusal, all-target resistance, single-payment room attacks and native banishment/burning. No live database update or walkthrough restart was performed. Build logs include existing warnings and unavailable NuGet vulnerability-feed warnings.

### Frozen charges and focus mode

Attach the registered `ChargedMagicDevice` component and an ordinary holdable component
to a nonstacking item. Its instances start empty; copying produces an empty bank.
Set `kind wand` (default capacity 5) or `kind staff` (10), `role charged|focus|dual`,
and explicitly whitelist each carrier payload with `spell add <spell>`.
Use `checkconfig` before submitting the component revision.

The default activation requirement is `eligibility caster`, evaluated from the body's
current canonical runtime magic capabilities. Builders may choose `anyone`, `magictype`
with an exact `capability`, or `acquiredspell`. Class labels, item ownership and journal
history do not create casting entitlement. `usable <prog>` accepts a compiled
boolean(character,item) policy. `check <trait> <difficulty> <threshold>` configures an
optional committed control check; failure spends one charge and suppresses improvement.
`mingrade <0-7>` adds a current acquired/controlled-grade requirement to any eligibility
mode; zero disables it. The acquired route and entitlement are checked again at use.

`capacity`, `seconds` and `plan add|remove` configure the bank and its additional supplies.
The default is 60 seconds per charge. Production uses the admitted spell's actual
grade-aware components and resource cost once per charge, plus the item plan once per
charge. This is a builder-editable cost policy using the existing configured reserve;
it is not a recovered historical Armageddon mana/degradation formula.

Hold or wield the device, quote multiword selectors, and use:

```text
magicdevice show <item>
magicdevice charge <item> <capability> <spell> <grade> <missing-charge-count>
magicdevice charged <item> <complete-target-selector>
magicdevice focus <item> <capability> <spell> <grade> <targets>
```

Use `self` for self triggers. Charged and personal focus casting are explicit modes;
a depleted bank refuses charged activation. Focus invokes ordinary paid acquired
casting and retains its ordinary progression rules. The coordinator-approved single
`Prepare` hook adds the focus to protected receipt inputs and rechecks exact actor,
body, item/prototype, custody, configuration and activation eligibility at final payment
boundaries. A destructive native component plan cannot consume that focus.

## Production, potency and release

Production is target-free. It requires a current enabled configured route, admitted
acquired knowledge, controlled grade, valid proficiency, physical casting ability,
payment and a feasible aggregate plan that protects the destination. It calls a
skill-level snapshot capture; no Vancian capability or Vancian enrolment is fabricated.
The capture stores producer trait values/bonuses, grade, controlled grade, scalar
bindings, duration and payload configuration. Costs and component plans are removed
from the detached release spell. The stored reliable outcome is Pass; live opposed
outcome and authored numerical randomness remain part of the release adapter.

Existing nonempty banks require matching configuration, grade and raw producer
proficiency at least the recorded raw proficiency. Exact numerical-input equivalence
qualifies. A stronger deterministic producer also qualifies for damage/heal/duration
when every defining raw and effective trait input is at least the original input and
the formula produces at least the stored result for **every** legal opposed degree.
The bank retains its original snapshot rather than mixing stronger new charges.
Temporary bonuses cannot substitute for decreased defining raw traits. Nonmonotonic
formulas that produce weaker output refuse. Stochastic formulas and ambiguous scalar
adapters currently require exact reproduction; generalized stronger-producer recharge
for those adapters remains an explicit limitation.

Current initial carrier adapters allow damage, heal, mend, trait boost, glow,
blindness/removal, deafness, invisibility/removal, silence/removal, sleep/removal,
paralysis/removal and water breathing/removal. Both wand and staff currently resolve
one complete, reachable target. Group/fanout targets refuse before commitment.
Identity/control-transfer, topology, concentration, extra unresolved target context
and unlisted payloads require their own future audited carrier adapters.
No scroll stock or universal compatibility opt-in is introduced.

Charged release uses the existing prepaid invocation adapter with a detached stored
spell. It spends no personal casting resource, acquires no spell and awards no skill
or mastery opportunity. Live target resistance, wards, reflection and applicability
run through the existing spell execution path. Item entitlement, physical access,
speech and the complete target are checked again inside commitment.

## Transaction and restart boundaries

Production writes `DevicePaying` before reservation/payment. The entire batch pays
up front, then enters `DeviceProducing`. The transient action observes custody,
movement, combat, state and logout; completion rechecks route/body/location/configuration
and reproducibility. Interruption leaves no output and refunds nothing. Source
lockouts apply when this work completes or is interrupted, so the work does not
cancel itself because of its own lockout. `DeviceFilling` precedes durable bank fill.
There is no restart reconstruction of a paid action timer.

Activation commits an insert-only `DeviceConsuming` row keyed by the individual
charge GUID. The database primary key arbitrates simultaneous claims: a duplicate
insert is refused, and an existing receipt/tombstone is preserved. A random claim
marker prevents uncertainty handling from amending another attempt's receipt.
Journal claim and bank update are separate durable boundaries; effects follow both.
An unknown claim result or any later exception yields quarantine and staff review,
without automatic retry, effect replay or refund.

Each reservation, fill and consumption uses an isolated, parameterized compare-and-swap
of the exact component XML bytes and expected prototype identity/revision. The component's
ordinary `Save` override uses that same writer; its prototype-update override atomically
compares the old bank/revision and updates the revision. Transient reservations and
refusal releases never queue a generic bank save. Conflicting hosts are disabled and
dequeued, preserving the durable winner. The base `CheckPrototypeForUpdate` signature
is virtual by coordinator approval; its default implementation is unchanged.
The checksum covers the full stored numerical/provenance/charge bank, and invalid
original XML remains available as evidence. A checksum is corruption detection,
not protection from a database administrator who deliberately rewrites and re-signs XML.
Copied charge GUIDs are additionally blocked by durable claim tombstones.

`NeedsReview` receipts and persisted reservations intentionally require staff
reconciliation. Generic receipt reconciliation never completes paid production or
recreates missing charges; stranded reservations require explicit inspected item
repair. Terminal receipts remain historical evidence.

## Verification and integration ownership

Focused tests cover entitlement, copying, homogeneous recharge, depletion, explicit
focus, callback changes, live wards/resistance, suppressed improvement and injected
fault boundaries. Review regressions execute actual usability and target-filter
callbacks that mutate custody, entitlement, body or configuration. Callback admission
and final structural checks are separate. The latter executes no eligibility, capacity,
usability, filter, visibility, manipulation, planar, physical or speech policy.
Native regressions use compiled `silentdrop`/`removemerit` policies at the final charged
and focus boundaries, two independently loaded host processes, real ordinary
SaveManager flushes and interface-dispatched same-host/stale-host prototype updates.
The lane-native project is
`tests/ArmageddonChargedDeviceNativeHarness/ArmageddonChargedDeviceNativeHarness.csproj`;
it links existing substrate read-only and selects a distinct entrypoint/output path.
`Run-IsolatedDevices.ps1` creates a fresh `futuremud-device-mysql_<GUID>` process/data
directory and randomly named owned database, verifies UUID/port/datadir/marker boundaries,
and shuts down/deletes only those owned resources. It never uses a user's database.

Final test receipts and source/assembly fingerprints are supplied separately in the
lane evidence directory. Preserve earlier failures as well as final passing runs.
Native results use a real engine character/body, item/component, skill/reserve and
database, with a controlled surrounding world/check catalogue. They are not a full
player-login/Telnet or production-world acceptance certificate.

The integrator should add the verified results and explicit limitations to the
central progress ledger. No schema or `IMagicCastingService` changes are needed.
The approved inert-outside-focus `Prepare` hook passes the existing resolved target
into lane-owned validation. The later review adds an approved `Execution` hook after
area selection and before `Paying`, and wraps native payment in the dedicated admission
scope. Two one-line shared hooks complete the contract: `TryGetCap` consults an active
exact debit admission, and `Character.UseResource` enters that debit after canonical
owner forwarding. Ordinary/direct casting enters no batch and retains its original
capacity evaluation and native debit behavior. No `MagicSpell` readiness or area predicate
code changes are included; the stock lane owns those changes.
Numerical-context optional grade/mastery XML remains backward compatible.
The general item registration audit classifies this new type as dependency-bound;
its fixed-count expectations increase by one. No generic stock item with placeholder
magic IDs is seeded.

## Callback boundary and native payment contract

Charged release resolves the final target, evaluates visibility/planar reach, usability,
physical/speech and current caster entitlement, then validates the captured structural
state before claiming a GUID. Focus completes route/target policies during `Prepare`;
after final area selection it separately evaluates current payment capacities and final
reach/eligibility, then validates captured balances, resource definitions, canonical
holders, capability definitions, actor/body/owner, prototype/configuration, held custody,
reservation and target location/layer/body. Production uses the same payment boundary;
completion performs route/potency callbacks before its structural check and durable fill.
Authored mutations are not rolled back; refusal adds no engine payment, charge change
or journal receipt. Capability grants removed by a successful applicability Prog are
also detected by the captured raw merit/capability-effect inputs.

The structural getter audit for this source checkpoint is:

- `Character.Body`, instance identity/primary owner and character Location/RoomLayer
  are stored references/fields. Secondary identity getters forward stored identity
  references. Character/Body merit and effect enumeration does not call `Applies`.
- Body held/wielded enumeration reads `_heldItems`/`_wieldedItems`. Item component
  lookup, Deleted, bank fields and prototype XML serialization do not execute policies.
- Item target Location/RoomLayer follow stored native spatial-host/custody references:
  vehicle cargo/access projections, trap reservation host/layer, chair/belt/automation
  mount/connectable/door/inventory/container links. Their native getter graph does not
  evaluate FutureProgs, effect applicability, visibility or reach. Character and cell
  target spatial getters are stored state. Target reach is evaluated earlier.
- Resource balance reads use the stored resource counter and canonical owner;
  configured-reserve detection reads catalogue policy IDs. Prototype/resource policy
  metadata reads and JSON/XML serialization execute no authored policies.
- Native `UseResource` finite checks, available-balance check, reserve cap clamp,
  subtraction, fractional amount behavior and `ResourcesChanged`/save-queue notification
  remain unchanged. SaveManager enqueue and SQL receipt/component CAS writes execute
  no authored Prog. The already-persisted actor/body/item IDs are captured before policies.

Admission captures exact holder/resource references, amount, pre-policy balance, current
cap result and resource/capability definitions. Any policy-caused balance, holder, body or
configuration change refuses before `Paying`. Raw trait values/references are also captured,
so a cap policy cannot weaken producer proficiency or change an attribute-cap input behind
an accepted result. `Trait.RawValue` reads the stored field; effective `Value`/`MaxValue`
are excluded from the structural pass. Insufficient/non-finite inputs refuse too.
The payment scope is installed **after** the callback-free structural check and receipt
write, immediately around the existing native `pay()` call (or production's native debit
loop), and disposed on success or exception. Each native debit must enter with the exact
holder/resource/amount; only then may one capacity query consume its result. Unrelated
capacity queries remain live, even with an active batch. Wrong, repeated or nested debit,
reused scope and inherited execution on another thread refuse rather than falling back
to policy execution. A nested batch is refused during admission, before any new `Paying`
receipt, as well as at scope installation. This preserves native canonical forwarding and accounting without
a parallel resource-debit implementation. Material/effect work remains under the existing
committed/uncertain journal protocol; failures never trigger automatic replay or refund.

The dedicated native regressions additionally execute a real capability merit applicability
Prog and a real `SimpleMagicResource.ResourceCapProg` that drop custody only at the last
evaluation while returning true/100. Positive native direct/focus/production controls,
canonical-owner forwarding, above-cap clamping, fractional debit and dirty-state database
persistence exercise the equivalence contract. Scope mismatches, consumption, exception
restoration, concurrent escape, insufficient balance and callback mutations have focused
unit coverage. Final exact-source fingerprints/results are supplied outside the repository.

The physical-item quarantine check now runs after normal usability admission and at the
final structural boundary for charged use, focus payment and production. It inspects
unresolved receipt item IDs across owners without executing visibility, manipulation or
authored policies. A durable consume/fill failure therefore follows the item when custody
passes to another eligible canonical identity; an owner-only spell check is insufficient.
The dedicated native regressions create a separate persisted character and real body,
transfer banks with one/two remaining charges after those failures, and verify immediate
and fresh-process refusal with exact bank XML, next GUID, reservation, balances, effects
and journal bytes unchanged. Explicit staff reconciliation then permits the legitimate
recipient to release a charge. Reload descriptors use uniquely named disposable files;
each reader runs against this lane's owned database and process paths. Focused cases also
cover a new global item receipt created by the last target callback before commitment.
Earlier fixture trials (Windows argument-size limit and an injected fault left active for
the reconciliation positive control) are preserved in the external evidence.

## N22 ordinary-player activation — 11 October 2026

The [N22 checkpoint](Armageddon_Item_Activation_Checkpoint_20261011.md) adds a full
server/MySQL/Telnet replay with genuine ordinary Player accounts, native Human
chargen/approval, separate authenticated sessions and exact primary body/instance
identities. It passes **870 assertions**; current Core/Library passes **7,201
tests**, zero failures/skips. This is a separate scope from the controlled native
harness above. Read the machine-readable checkpoint for exact inputs and limits.

`magicdevice` now registers once in the shared InventoryModule. Its former
MagicModule-only registration made the documented player command unavailable;
the existing parser and service remain. Player/NPC/Guide/Admin dispatch and the
staff environment permission boundary have regressions. Stock Dual/Caster
wand/staff retain their Mend whitelist, capacity 5/10 and 60-second production.
Non-casters refuse. Native revisions on one private clone prove Anyone and the
independent minimum controlled grade; `mingrade 0` permits the non-caster's charged
release without giving personal acquisition or focus entitlement.

Five native-produced charges pay 250 once. Transfer, a producer change from
database skill 60 (native capped raw 50)/grade 3 to raw 30/grade 1 and cold restart retain exact banks and potency.
Each release heals 18 and claims exactly one GUID with a completed actual
recipient/item receipt, without either player's reserve or progression changing.
Depletion refuses implicit focus. A separate material-bearing private focus
refuses absent/ground material and both occupied hands, then consumes the exact worn carried item, pays 50
and heals 2 through ordinary grade 1 casting, retaining ordinary progression.
Final cold load proves no charge recreation or duplicate debit. Four server
phases exit zero with collectors stopped; owned MySQL stops and its port closes.
Stock component definitions/revisions and captured capability restoration pass.

Explicit grants, deterministic skill/control/reserve setup, one-second private
production and an Automatic private focus do not qualify natural stock progression
or affordability. Only stored Mend is invoked here. The full N23 recharge matrix was pending at this N22 checkpoint; its later
qualification follows below. All carrier adapters and provider-failure acceptance
remain pending. No installer
content, schema or blank snapshot changes; whole-programme and Release remain off.

## N23 legitimate recharge — 11 October 2026

The [N23 checkpoint](Armageddon_Recharge_Checkpoint_20261011.md) qualifies
ordinary-player top-up of a partially depleted bank, with **7,208 managed tests**
and **1052 full-server assertions**. Current route, acquired knowledge,
controlled grade and reproducible raw potency are required again. An entitled
alternate admitted capability with stronger deterministic potency can recharge;
the bank retains its original snapshot, producer provenance and old GUID order.
Capacity, grade, different payload, reserve and aggregate material shortages
refuse without payment or journal mutation.

Eight charges are created for 400 reserve; five releases retain the original
18-healing budget and exact GUID/recipient receipts. Two real per-charge supplies
are consumed before timed private work completes. Production grants no
manifestation or progression. Paid cancellation and a deliberate owned-MUD crash
each retain their 50 debit without output/refund. Cold restart preserves the
stranded reservation and refuses use/recharge pending explicit staff review.
Five clean phases and one forced stop are separate evidence; all collectors and
owned MySQL close. Stock revisions and capability restoration pass. No runtime
implementation change is needed by this matrix. Explicit fixture grants and
reserve/skill conditioning leave natural stock progression and affordability
unqualified. N24 installer rerun safety, N25 provider faults and wider readiness
remain pending; whole-programme and Release gates remain false.
