# Accelerated Combat Simulation

## Purpose

The accelerated combat simulator is a founder diagnostic for testing ordinary combat strategies without waiting for wall-clock combat delays. It supports two or more named teams and accepts both loaded characters and current NPC templates. The simulator uses the real combat move, health, effect, item and heartbeat code; it is not a separate statistical combat model.

The workflow is staged under `impdebug combatsim`. A founder selects a scene, adds combatants to teams, reviews preflight warnings, and then runs the staged scenario. The completed session retains a summary and a bounded transcript until it is replaced or cleared. The same staged scenario can also be run as a bounded batch across a deterministic sequence of seeds to compare aggregate outcomes.

## Runtime model

The command executes synchronously on the command/game-loop thread. While it runs, ordinary connection input, game-loop scheduling, clock advancement and save-loop work do not run. A flow-local runtime scope replaces the game world's scheduler, effect scheduler, heartbeat manager and save manager for simulation code. A virtual `TimeProvider` advances directly to the next due main or effect schedule, and an ambient seeded `Random` removes variation from both the engine's shared random source and ExpressionEngine `rand`/`drand`/`dice` functions. The scope is active before source effects are serialised and before characters or NPC templates are materialised, so load hooks see the simulation clock and seeded random source.

Gameplay code that needs current UTC uses the ambient `RuntimeClock.UtcNow`. Outside an isolated runtime scope it delegates to `TimeProvider.System`; inside a simulation it returns advancing virtual time. Operational timestamps such as login history, builder edit dates and persistence audit chronology deliberately remain on the system clock, as do the wall-clock execution guards.

The simulation starts a private heartbeat at one virtual-second intervals. After its snapshot or template-load data is applied, each materialised participant completes the body-login lifecycle inside that private scope. This calculates organ function and subscribes stamina, bleeding, healing, drug, breathing, biological and other heartbeat processes to the private manager without firing the broader character-entered-game hooks. Combat recovery schedules and expiring effects share the virtual clock. Execution stops when no more than one active team remains, no schedule remains, the configured virtual-time, fired-event or wall-clock guard is reached, or no combat-relevant participant state changes for 30 virtual seconds and 1,000 fired schedules. The latter is reported as a stalemate with an actionable no-input warning rather than consuming the whole event budget.

Team combat uses `SimpleMeleeCombat` with a simulation-only target policy. When an actor needs a target, the policy chooses an engageable, able member of another team. This preserves the normal move selection and response pipeline while allowing fights larger than one-on-one.

## Scene and combatant materialisation

The scene is a transient negative-ID `Cell` that borrows the selected cell's room and overlay environment but owns its characters and items independently. It has no exits, so strategies that require actual room-to-room flight may reach a guard limit rather than successfully flee. Saved effects on the source cell are cloned when possible.

Loaded-character sources are never placed in combat. The simulator creates a non-player transient character with the source's identity template, race, traits, descriptions, merits, blood, stamina, active and latent drug doses, position, combat settings, strategy, saved character/body effects, wounds, infections, and deep copies of worn, wielded and held inventory. Wounds are reconstructed through the target body's health strategy and then assigned their live damage, pain, shock, stun and bleeding values; infections retain their type, intensity, immunity, virulence, stage and wound/bodypart relationship. Special wound implementation state, lodged-item relationships, unsaved non-saving effects, clan/party/project state and arbitrary external relationships are not guaranteed to clone exactly.

NPC-template sources are materialised as simulation-only NPCs. Template load additions and the on-load FutureProg run first; the participant body is then initialised and `NPCOnGameLoadFinished` runs in the same order as ordinary NPC spawning. These extension points are also part of the non-transactional risk boundary described below.

## Outcomes and reporting

Per-combatant terminal outcomes are:

- death;
- incapacitation, including ordinary non-able character states;
- full grapple control (`IBeingGrappled.UnderControl`);
- successful departure while using the flee strategy;
- another combat withdrawal;
- survival on the winning team; or
- stalemate when a guard ends the run.

`Surrendered` remains part of the report contract, but the current automatic combat strategy pipeline has no general surrender transition to emit it. Fully manual combat, manual movement/position management, fully manual ranged or inventory management, zero automatic attack weighting, and NPC on-load progs are surfaced during preflight because they can stall or broaden a no-input simulation.

The individual report includes run ID, seed, winning team, stop reason, virtual and wall-clock duration, fired-event count, participant state, blood and stamina ratios, wounds, terminal outcome, and a full versioned execution fingerprint. The current `v1` fingerprint is a SHA-256 digest over materialisation order, seeded random draws, private scheduler ticks, simulation output and the canonical terminal state. It excludes wall-clock duration, run IDs and transient object IDs. Transcript entries carry virtual timestamps and participant labels. The default limits are 30 virtual minutes, 100,000 fired schedules, 10,000 transcript entries, and 60 wall-clock seconds.

## Batch tournaments

`batch` reuses the currently staged scene and combatants for 1 to 100 sequential runs. It starts from the staged seed unless `seed` is supplied, and increases it by one unless `step` is supplied; a zero step is permitted when intentionally replaying the same seed. The batch validates that every generated seed remains a 32-bit integer. One UTC epoch is captured at the start of the batch and reused by every run, preventing real elapsed time between repetitions from changing absolute-time-sensitive gameplay decisions. Separate batch invocations intentionally capture new epochs.

Each run retains only its compact result, not its transcript, so a 100-run tournament cannot consume transcript memory. The aggregate report shows team wins and win rates, run-status counts, combatant-outcome counts, total, average and range virtual duration, and both summed simulation and whole-batch wall-clock duration. The final individual result remains available through `report`, but its batch transcript is intentionally empty. `batchreport` repeats the aggregate report; `batchreport runs [<start>] [<count>]` pages the per-run seed, status, winner, timing, event count and short trace fingerprint. `batchreport trace <run> <random|state|materialisation> [<start>] [<count>]` pages the captured diagnostic trace for one run, making the first divergence in an intentionally repeated seed inspectable without retaining full combat transcripts.

When a batch intentionally repeats a seed (normally by using `step 0`), the aggregate report groups those runs and compares their full execution fingerprints. Matching runs are marked as replay matches; a mismatch is a prominent warning and can be inspected through the paged run report. For engine-controlled scenarios with unchanged staged sources and a shared batch epoch, matching fingerprints are the exact-replay criterion. The fingerprint is diagnostic evidence, not a way to make external hooks, direct Dapper writes, unseeded random sources, or asynchronous work deterministic.

The batch has a hard ten-minute aggregate wall-clock guard in addition to the per-run wall-clock guard. If the aggregate guard expires, completed runs are still reported and the report explains that remaining runs were skipped. A batch has the same production confirmation and warning/`force` requirements as an individual run.

## Persistence and safety boundary

The live save queue is left untouched while the simulation runs. Simulation code receives a flow-local `FMDB` context and a private save manager, so pre-existing pending saves neither enter the simulation transaction nor prevent a run when an unrelated live save is currently invalid. The simulation-specific EF context suppresses every `SaveChanges` call, including persistence initiated directly by a combat-time tick; an EF transaction remains a defence-in-depth rollback boundary. The simulation save manager likewise discards ordinary saves. Actors, bodies, items and the transient cell newly registered during the run are removed from the game-world registries in cleanup.

Legality queries still run, including `ActLawfully` decisions, but actual crime creation is suppressed by the same flow-local runtime policy. A simulation therefore cannot add crimes to a legal authority or the game-world registry, notify witnesses or victims, invoke crime storyteller hooks, or queue crime persistence. Ordinary execution on other flows is unaffected.

This is a development-first diagnostic, not a complete process sandbox. The following cannot be guaranteed to unwind:

- SQL issued through the separate `FMDB.Connection` Dapper connection;
- email, Discord, files, HTTP calls or other external services;
- hooks, AI or FutureProgs that mutate pre-existing in-memory objects;
- work started asynchronously that outlives the simulation scope.

Simulation character death uses a reduced death path, so it does not post death-board entries, update statistics, send email/Discord notifications, create estates or destroy a live character. Other authored hooks and progs remain the responsibility of the tester.

`FUTUREMUD_ENVIRONMENT` values `development`, `dev`, `test`, `testing` and `local` are treated as non-production. The variable being unset, `production`, `staging`, or any unrecognised value is treated as production. Every production run requires the literal `confirm-production` option; it is deliberately not remembered. Preflight warnings additionally require `force`.

## Founder workflow

```text
impdebug combatsim new [<cell>]
impdebug combatsim add character <loaded character> team <team>
impdebug combatsim add template <NPC template> team <team> [count <number>]
impdebug combatsim remove <slot>
impdebug combatsim set scene <cell>
impdebug combatsim set seed <number>
impdebug combatsim set maxtime <timespan>
impdebug combatsim set maxevents <number>
impdebug combatsim set transcript <number>
impdebug combatsim show
impdebug combatsim validate
impdebug combatsim run [force] [confirm-production]
impdebug combatsim batch <runs> [seed <start>] [step <increment>] [force] [confirm-production]
impdebug combatsim batchreport [runs [<start>] [<count>]]
impdebug combatsim batchreport trace <run> <random|state|materialisation> [<start>] [<count>]
impdebug combatsim report
impdebug combatsim transcript [<start>] [<count>]
impdebug combatsim clear
```

For comparable runs within one batch, keep the source state, active NPC-template revision and selected scene unchanged; use a zero seed step for an exact seeded replay. The seed controls the engine's shared random source, expression evaluation uses call-local parameter values rather than shared mutable dictionaries, and all runs share the batch epoch. Repeated runs should therefore have the same `v1` execution fingerprint unless authored or external code escapes those controlled services. Separate invocations use their own starting epoch. `report` repeats the summary; `transcript` defaults to the first 100 entries and accepts a one-based start plus a maximum count of 1,000.
