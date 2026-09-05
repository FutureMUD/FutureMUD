# Psychic Powers, Impressions and Witness Memory

## Optional installation and access

The repeatable **Psionics** seeder installs Basic Psionics (`psi`) and Advanced Psionics (`apsi`). Declining the package writes no magic content. Reruns install missing named records, preserve builder edits and reject incompatible identities. The canonical power tuning is `FutureMUDLibrary/Magic/PsionicStockContent.cs`.

Neither package assigns characters, merits, chargen choices or automatic access hooks. Configure access explicitly with the normal capability-granting merit or effect workflows. The Psionic Discipline skill is unavailable in chargen and does not branch automatically. Capability unlocks use bands 0, 20, 40, 60 and 80. Advanced Psionics also includes its own basic link operations so its advanced powers can use links within their own school.

Focus has a cap of 100 and regenerates five points per real minute while conscious. Sitting, reclining, lounging or lying down out of combat and movement doubles regeneration. The shared regenerator is attached through capabilities. Builders can change the linear regenerator's `conscious` and `restmultiplier` options. The package does not give everyone a resource pool.

Projection, possession, levitation and the scoped illusion are spell-backed powers. Use the wrapper verb followed by `insignificant` and ordinary targeting arguments. Their known-spell progs query `HasMagicPower(character, powerId)` and grant nothing. Both normal casting and the wrapper retain spell checks, materials, delays, costs and cleanup; the wrapper does not charge a second power cost.

## Investigation and support

`psychometry <here|item>` reads only recorded facts. Success controls detail; feelings require a major success. Item custody duration is measured from timestamps, with unknown beginnings stated explicitly. A major success may identify a carrier subject to psychic identity concealment. Mixed stacks explicitly lose attributable automatic provenance.

`somaticsense <target>` gives broad condition and, on better results, pain and wound severity. `dreamsend <target> <text>` sends bounded, sanitised dream text to an eligible sleeping linked mind. It does not queue offline delivery or execute dream commands.

`guardmind <target>` maintains protection for a willing linked beneficiary. The caster carries concentration and upkeep. `guardmind <target> expel` assists expulsion of an incoming presence. `disruptconcentration <target>` challenges one maintained effect. `transferfocus <target> lend|siphon` transfers only available donor resource into recipient headroom; the configured loss is deducted from the transferred amount.

## Impressions switch and storage

`EnablePsychometricImpressions` defaults to **false** in fresh and upgraded worlds. False disables automatic recording, authored-clue creation and all gameplay reading of item/cell impressions, including the `PsychometricImpressions(owner)` FutureProg query. Stored payloads remain saved; old timed records expire against their original timestamps. The seeder does not turn the feature on.

Changing the switch through the static configuration workflow changes an observation epoch. The next custody observation starts with an unknown beginning rather than counting the disabled interval as continuous custody. No world-wide item scan is needed.

Cell histories hold at most 32 magic, 32 violence/death and four feeling records. Magic and violence expire after 24 real hours; feelings after ten minutes. Repeated participant/category activity coalesces for 30 seconds, but deaths remain distinct. Text is bounded to 256 characters. Route-cell readings respect recorded position and layer. Ambient events are recorded once on the cell, not copied onto nearby objects.

Item histories hold current effective custody, four previous periods and eight automatic direct-involvement records. Carried containers pass effective custody to their affected contents. Authored clues are separate, with a limit of eight. Split/copy operations preserve recorded facts; merges with differing histories mark mixed provenance. Recording uses lazy saved effects and ordinary save handling, without per-impression timers or immediate database writes.

Staff use `psychometrichistory` for enabled state, active payload count, recorded/coalesced counts, evictions and processing time. `psychometrichistory <here|item> <clue>` authors a clue. Character links and character psionic traces remain independent of this switch. New cell activity uses the bounded history instead of adding a timed trace effect for each event.

## Selective forgetting and legal evidence

`forgetting <target> skill|knowledge|recognition <subject>` suppresses effective access without deleting learned records. Skill suppression also blocks improvement. Expiry or dispelling restores the unchanged underlying information.

Use `forgetting incidents` to list incidents the caster personally knows, then `forgetting <target> witness <incident number>`. These player references come from the known-incident list, not unrestricted crime IDs. The target need not actually be a witness for the targeting message to be valid; failure messages do not disclose hidden witness lists. Legal recall is keyed by stable character identity, so changing bodies does not evade it.

`forgetting virtual <incident number>` acts at the scene of a known incident from the last day, checking each recorded virtual witness profile separately. No simulated NPCs are created. The power charges per attempted source and stops when resources run out. Virtual profiles describe the existing bystander abstraction, not individually simulated inhabitants.

The power's `permanent` option explicitly enables permanent witness forgetting. The seeded example leaves it false. To author a permanent variant, clone the example and enable that option deliberately. Permanent forgetting has no expiry and requires staff restoration; ordinary dispelling can restore temporary character suppression.

`Crime.WitnessMemory` stores an XML collection of real-identity or virtual-profile sources, incident location, recall, report delivery and due time, identity knowledge, reliability and audit provenance. Loading older crime rows converts existing character witness IDs into available-recall records; no historical virtual profiles or pending reports are fabricated.

**Delivered reports remain valid.** Forgetting does not clear known crimes, identity evidence, charges, convictions or another witness's recall. New reports consult recall. Staff use `witnessmemory <crime id>` to inspect the records and audit, and `witnessmemory <crime id> restore <witness number>` to restore recall explicitly.

`VNPCWitnessReportDelaySeconds` defaults to **0**, preserving immediate reports. A setting of **120** gives psychic characters time to act before delivery. Reporting chance, identity knowledge and reliability are resolved when the event is witnessed. Only successful pending reports are scheduled. Temporary suppression defers delivery; permanent forgetting prevents delivery. Restarts recover overdue work, and delivered/cancelled checks prevent duplicate evidence. Finalised, resolved or removed incidents stop pending delivery.

## Social and intrusive powers

`psychiccircle begin|invite|dismiss|end` defaults to eight participants with per-member upkeep on the leader. Builders can set `circlelimit <2-64>`, including the leader. Invitees use `psicircle accept|decline`, then `psicircle say <message>` or `psicircle leave`. Membership grants no powers, backlog or access to private thoughts.

`psychicfeedback begin|end` maintains a defence. Builders choose `feedbackmode warning|resource|stun`; stun is capped at ten. Reactions act on the original mental action rather than starting another mental action, preventing feedback recursion.

`emotion <target> read|fear|calm|courage|agitation|affinity|aversion` reads represented emotions or applies a bounded effect. Affinity and aversion are available through `PsychicDisposition(character, subject)` for authored AI decisions. They confer no commands, prices, property rights, affiliations or legal privileges.

`attentionsuppression` creates observer-specific noticing checks; deliberate looking contests suppression and hostile actions end it. `delayedsuggestion` plants one thought or emotion triggered by a delay, entering a selected cell, encountering a selected person or entering combat. Protection checks run again when it activates. Its payload is never interpreted as a victim command.

`telekinesis <item> get|move|open|close` operates locally on visible unattended objects and supported openable mechanisms. It also supports `switch <setting>`, `select <option>`, `empty [destination or amount]`, `pour <destination> [amount]`, `fill <source> [amount]`, and `put <container>`. Liquid amounts use the world's volume units; omission transfers as much as fits. Emptying an item container uses its normal overflow-to-ground behaviour, while an unknown named destination is rejected.

The builder `amount` setting specifies maximum mass in kilograms (default 10), converted from the world's configured base weight units. Mass, spatial reach, planar interaction, custody, access rules, containment, anchoring and locks constrain manipulation. Both source and destination must be eligible. Closed vessels, incompatible mixtures, full destinations, denied switch/select settings and anchored objects remain protected. Component operations are prepared before the invocation is charged, then executed through their ordinary APIs. Manipulation never runs a general player command or grants remote tool use. Direct impressions are recorded on affected items and ambient activity once on the cell.

### Existing-power quality corrections

Mind barriers now apply when their applicability prog returns true, and their modifier is used when connecting minds. Barrier bonuses describe the modifier to the intruder's check: negative values protect the mind. Opposed mental resistance converts that sign appropriately. Stock barriers use -15. Builders who deliberately compensated for the old inverted applicability check should review their custom progs. Mind contact pays its attempt cost before resistance and feedback, including attempts stopped by a barrier. Exclusive contacts inspect outgoing connections, and `last` avoids reconnecting to a mind already linked.

Power builders reject non-finite numeric settings. Invocation/upkeep XML rejects negative resource costs, and sustained concentration cannot be negative. Hex duration validation preserves the previous value on rejection; empathy and danger-sense intervals reject values outside TimeSpan bounds. Choke resistance intervals use the same one-day maximum as anaesthesia. Existing registered power tokens and finite custom configurations remain compatible.

## Existing power terminology

- **Audit** examines incoming presences in the user's own mind, subject to concealment.
- **Empathy** transfers wounds; emotional attunement is a separate power.
- **Suggest** delivers a thought, with no command execution or obedience.
- **Prescience** is a staff-mediated vision request, not automatic future prediction.
- **Trace** reads permitted active links and residual character traces. Item/cell investigation is separately gated by the impressions switch.

## Persistence and verification

The EF migration is `20260905072550_PsychicWitnessMemory`, generated with `dotnet ef`. Its designer, model snapshot and bundled blank database snapshot move together. Psychic saved effects retain origin power IDs for detection and dispelling. Live links and circles end when their participating session lifecycles end; finite independent suppression and suggestion effects use the normal saved-effect schedule.

### Validation recorded 5 September 2026

The default unit-test script passed all nine suites. The final core rerun, including spell-adapter forwarding and telekinetic manipulation regressions, passed 2,712 tests (4,148 across the default suites); the other suites passed 470 library, 23 expression, 749 seeder, 41 persistence, 22 Discord, 43 converter, 54 website and 34 terrain tests. Seeder Debug and Release builds passed. `dotnet ef migrations has-pending-model-changes` reported no model drift, and the blank-snapshot parser/parity tests passed after regeneration against a disposable database.

Recorder benchmarks used distinct mock item instances in 10,000- and 100,000-item registries. Ten thousand recorder updates took 87.34 ms and 133.16 ms respectively in the recorded run. Tests also assert that recording never accesses the world item registry, so unrelated item count introduces no enumeration into the operation. These are focused recorder benchmarks, not measurements of loading or saving a fully populated live world. A 1,000-source pending-report burst verified callback-only processing without enumerating world crimes. Separate tests cover immediate delivery, overdue recovery, duplicate callbacks, suppression deferral, permanent recall loss and preservation of delivered state.

A disposable clone installed both schools, 43 power definitions and four backing spells; a repeat install preserved the existing definitions. A local-MUD walkthrough verified default-disabled diagnostics, runtime enablement, authored clues, feeling capture, graded psychometry, feedback start/end, circle creation/messaging/end, spell-backed levitation, and disabling both reading and clue creation again while retaining stored payloads. The walkthrough used an explicitly provisioned administrator test character; seeding itself granted no character access. No production database or deployment was changed.

The extended telekinesis walkthrough verified pouring between cups, emptying liquid onto the ground, and switching a flashlight off. An already-on switch was rejected through its normal component restrictions. This walkthrough caught and corrected a base-weight-to-kilograms mismatch; regression cases cover gram-based and kilogram-based worlds, boundary mass, and invalid values. Switch/select preparation, denied operations, liquid capacity, and invalid empty destinations also have focused tests.
