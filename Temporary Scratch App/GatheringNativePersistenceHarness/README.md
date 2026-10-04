# Gathering native-persistence acceptance harness

This narrow disposable-database harness covers native persistence acceptance for health-priced Self gathering and native organic field accounting. It is not part of the normal test suite or a general test platform.

From the repository root, build and run it with:

```powershell
dotnet build 'Temporary Scratch App\GatheringNativePersistenceHarness\GatheringNativePersistenceHarness.csproj' -c Debug --no-restore -m:1
& 'Temporary Scratch App\GatheringNativePersistenceHarness\Run-IsolatedAcceptance.ps1'
```

The runner starts a newly initialised MySQL instance on a loopback-only random port. It creates fresh, separately owned `futuremud_gather_gc_<timestamp>_<random>` and `futuremud_land_<timestamp>_<random>` databases after checking that each name is unused, imports the supported blank snapshot, and passes the isolated server connection only through `FUTUREMUD_GATHERING_TEST_CONNECTION`. The loopback-only disposable server uses no TLS because it has no trusted certificate. The harness never reads normal game configuration or chooses an existing database.

It executes the production `MagicGatheringService`, `SimpleLivingHealthStrategy`, native `SimpleOrganicWound` persistence and receipt store. GC-P01 uses no `OnGathered` callback and creates a wound; GC-P02 starts from a persisted cellular wound at 7 damage, 11 pain and 13 stun and requires the independent reader to observe 10, 15 and 18; GC-P03 launches a separate reader process that reconstructs the character, body and receipt service without a save-manager flush, logout, shutdown, healing tick or resource regeneration.

The land run first covers Y-T02 with production environmental-profile builder creation, organic authoring, `SaveManager` persistence, reload, clone, independent clone editing and a second reload. It then executes production `AgricultureField` accounting and lifecycle methods, `SaveManager`, EF/MySQL persistence, `AgricultureFieldInput` reservation, annual harvest, native herd grazing and a separate reader process. Y-P01 observes the conservative whole debit and prepaid fraction independently, then reconstructs the field and spends the remainder exactly once. Y-P02 replaces a crop, runs and reloads native ticks, and verifies saved lifecycle/progress. Y-P03 interleaves fractional plans with real craft, harvest and grazing consumers so stale plans cannot reuse physical stock. A provider trigger also forces one field checkpoint to fail and proves rollback plus `SaveManager` retry before the trigger is removed. The harness substitutes only unrelated world catalogues, deterministic weather and the ecological scalar factor; real coordinator damage/repair behavior is covered by the owning environmental integration tests (Y-P04).

The correction probes add C-P01 for saved pending pasture allocations at factors 0.5 and 0, followed by native establishment, owner checkpoint, independent database read and reconstruction. They also construct new fields through the production constructor and independently read their first insert before pasture establishment. C-P02 checks a retained orchard harvest near capacity with separate prepaid and positive-recovery fractions across an owner checkpoint and later reconstructed tick. C-P03 checks the real coordinator against persisted forage and crop owners before and after dynamic invalidity and correction.

The runner executes `--land-run` in a separate marker-owned `futuremud_land_` database. C-P03 uses a saved environmental profile, production coordinator, real cell and field, persisted overlay and forage profile, and independent database reads around refused and accepted conversion. Its fixture controls an apiary candidate for the current crop baseline. Pass `-LandOnly` to the PowerShell runner for a focused land rerun in a new isolated instance; build the harness first after source edits.

The Land extension authors a capability through the production editor and completes crop plus forage, two forage keys in one cell, ambient plus fractional crop, and two native wound-priced actions. Each success is checked through a fresh MySQL context before an unrelated flush. A separate reader process reloads the recipient, wound, field fraction, sources and linked parent/child receipts, then refuses replay. The ordinary player command adapter is exercised with a finite draw, overdraw and interrupted timed action. The environmental profile's forage suppression is measured after the draw and after an existing explicit repair operation. A pollinated crop's current recovery factor is then invalidated; the Land command refuses without a native or personal transfer. A later successful fractional Land crop draw binds an ambient maximum to crop health: paid ambient collateral precedes health loss, the effective maximum falls, and credit stays exact. A real craft reservation, annual harvest and replanting discard the old paid fraction and generation. A controlled MySQL `AgricultureFields` update trigger rejects one gathering owner checkpoint; independent reads confirm unchanged persisted stock, fraction and credit, then the owner retry persists the already-applied state without replay. A zero-stock crop spends only an existing prepaid fraction and remains living. A separate `Cells_ForagableYields` update trigger rejects a forage owner checkpoint; its dirty flag and queue survive, and retry saves the single debit. An ordinary crop daily tick during a timed Land action grows native stock; completion uses a fresh plan for its captured allocation. `L-P06` and `L-T37` are deterministic provider fault injections, not network or power failure simulations.

The ARM-02 casting extension uses `-CastingOnly` to exercise production builders, the non-admin school command, casting and gathering services, canonical second-body skill/resource ownership, detached Stone Skin scalars, delivered Ember Lance wounds and Wardcraft prerequisites. Separate reader processes verify persistence and conservative recovery after payment/effect/progression faults, including a real provider-trigger failure after mastery sampling. The minimal living anatomy includes real functioning brain/heart prototypes; check outcomes and mastery randomness remain controlled. This is native command/service persistence acceptance, not a full installed-world Telnet smoke. See [the casting handover](../../Design%20Documents/Magic/Configurable_Casting_Handover.md) for requirements and evidence. `-RefreshSnapshot` additionally refreshes the maintained blank database snapshot in a separate disposable `fm_snap_*` schema; build DatabaseSeeder first.

The output records the base revision and dirty/clean worktree state, MySQL version, generated database names, fixture and operation IDs, expected/observed channels, and cleanup result. Cleanup first verifies the temporary server's data directory and the database ownership marker; it removes only resources created by that run.

The phase2A practice extension is included in `-CastingOnly`, or selected alone with
`-PracticeOnly` after rebuilding the harness. It uses real native Character/Body/Skill
and ClassicImprovement objects, an authored enrolment Prog, the non-admin practice command,
and MySQL operation/resource/trait persistence. A controlled successful check and mastery
sample with an accelerated clock prove raw 30-to-60 and controlled 1-to-7 through paid
practice alone. A restrictive native difficulty interval blocks real gain; a permissive
one permits it. A nearby native injured/warded actor, damage/caster templates and forbidden
resistance lookup check practice purity. Stop, focus reset, speech loss, capability loss
and quit signals retain full payment. Actual native silence is added to body or character
and removed or expired entirely between heartbeats; required speech loss must retain payment
and both deadlines, write `PracticeInterrupted`, and perform no check or progression.
Speech-free profiles also complete while silence remains. The minimal anatomy's communication
adapter uses the production silence predicate; full vocal anatomy/volume remains unqualified.
Native body-part effects similarly exercise live manipulation loss/restoration and hand-free
policy, while brief native paralysis is checked before a health-state refresh.
A separate process reconstructs pending prepaid
work, checks shared deadlines and refuses automatic resume/refund/reroll; staff recovery
leaves the old timer unable to overwrite its terminal receipt. Its material plan is
explicitly empty; material execution/finalisation is covered by focused automated tests.
This controlled cap60 fixture is not an installed Mend Flesh, full hostile-AI scenario,
real-time scheduler or Telnet/login stock qualification.

The Phase3D1 corpse-animation extension is selected with `-CorpseAnimationOnly`
after rebuilding Debug. It exercises paid native grade-3 casting, an actual selected
CombatEnd AI and compiled resource Prog, same-corpse expiry/dispel/death/Quit,
exact ownership, activation and provider failure recovery, guarded placement,
ordinary saves and separate-process cold recovery. Source anatomy and room catalogues
are controlled. Callback fault injection uses native item/body methods; stale cell-join
loss is an explicit database simulation, not a native Cell.Save execution. The old
host remains quiescent after restart recovery. Final-death NPC corpses are qualified;
PC and nonfinal corpses, installed stock AI/combat profiles, arbitrary callbacks,
simultaneous-host recovery and high-volume N16 remain unqualified. Use the stage
verification receipt for exact passed checks and retained failed exploratory probes.

The Phase3D1P2 extension in the same packet uses the real EffectScheduler and its
virtual clock for paid animations with ordinary parent durations of 30, 180 and 300
seconds against an independent 180-second lifecycle. A native glow sibling verifies
ordinary duration is neither extended nor shortened. Native proxy dispel is exercised
at 15 seconds and after the short parent's ordinary expiry at 60 seconds. These expiry
checks do not call the lifecycle reconciliation worker. The duplicate activation
marker describes sequential calls; concurrent activation and cleanup races remain
unqualified. Cell/save/combat fixture limitations above still apply.

The same corpse packet also runs `--corpse-animation-saved-parent-run` in a separate
owned database. It saves a real paid parent/child, checks inert active boot loading,
and injects a completion-update refusal after restoration commits while paid XML remains
unsaved. Fresh processes load that XML during disallowed boot and allowed runtime,
before and after journal completion. They assert one corpse construction, exact original
identity/body/foreign gear, unchanged deadline and deferred cleanup through the real
effect scheduler. The final reader normally saves removal of stale XML. Intermediate
readers deliberately do not flush the corpse, and the producer remains quiescent;
this qualifies a controlled crash boundary, not simultaneous-host recovery or power loss.

These readers retain a positive persisted corpse morph duration and verify both native
morph schedules during blocked boot, unchanged timing through recovery and remaining
duration on normal save. Separate `--corpse-animation-active-future-run` and
`--corpse-animation-active-expired-run` databases cold-load still-active paid parents
and complete actual Logout/Expiry recovery after boot. They assert exact secondary
retirement, unchanged canonical/body/item IDs and foreign gear, no scripted AI
construction or resource-program replay, and durable stale XML removal. The same
corpse packet runs these modes after the pending/completed checkpoint matrix.
