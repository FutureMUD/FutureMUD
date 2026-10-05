# Charged devices: isolated implementation checkpoint

Authority: `Armageddon_Magic_Completion_Implementation_Brief.md`, Library
`libfile_a4606cd0097081918d6c9d9e220e6da0`, full supported text read (905 lines),
sections N22/N23. Base: `1ad24d636925da64e7a82ffa858b0af470e56b0a`.
The central progress ledger, repertoire disposition tables and native dispatch were not edited.

## Builder and player use

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
callbacks that mutate custody, entitlement, body or configuration. The final state-only
checks run after authored callbacks without executing another usability/filter callback.
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
The shared casting-service edit is the approved inert-outside-focus `Prepare` hook;
it passes the existing resolved target into lane-owned final validation.
Numerical-context optional grade/mastery XML remains backward compatible.
The general item registration audit classifies this new type as dependency-bound;
its fixed-count expectations increase by one. No generic stock item with placeholder
magic IDs is seeded.
