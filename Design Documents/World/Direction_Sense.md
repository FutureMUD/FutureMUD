# Direction Sense movement

Direction Sense is an opt-in engine movement mechanic for an initiating manual horizontal compass command (north, northeast, east, southeast, south, southwest, west or northwest), on foot, mounted, or while driving a controlled vehicle. It is inactive in existing worlds by default. Named/non-cardinal exits, up/down, direct or scripted exit movement, follower reactions, vehicle route actions and autopilot do not start orientation checks.

## Command outcomes

The requested exit must resolve, be visible to the initiator under the existing exit visibility rules, and pass movement readiness before orientation is attempted. The ordinary rejection path handles invalid or blocked requests without a Direction Sense roll.

When the configured obscuration predicate is true:

1. An initial success uses the requested exit and retains its explicitly supplied safety override.
2. Initial failure followed by recovery success makes no progress.
3. Two failures choose one safe, visible, usable, unintended horizontal compass exit. If none exists, make no progress.

Only the initial check can improve the skill. Recovery uses a separate non-improving check. Missing skills cannot branch from either check. The rider or vehicle controller supplies the trait; mounts, passengers, followers, tow trains and party members do not roll Direction Sense independently.

Candidate selection excludes hidden exits, non-cardinal exits, fall exits, unavailable transitions and exits the actual movement cohort cannot safely use. It retains ordinary mount, party/LeaveNoneBehind, lawful movement and vehicle/tow/propulsion eligibility. Selection never carries the requested direction's `!` override to an alternative. Readiness is checked without constructing a movement, rolling mount control, committing propulsion, rolling tow catastrophe or consuming stamina. Execution revalidates the selected exit; a rejected alternative reports only `You lose your bearings and make no progress.` and is not replaced with another roll or selection.

A no-progress sequence never creates a movement or consumes normal movement stamina. An optional delay is applied to both `move` and `drive`.

## World configuration

| Static configuration | Inactive default | Purpose |
| --- | --- | --- |
| `DirectionSenseEnabled` | `false` | Explicit world activation. |
| `DirectionSenseObscurationProg` | `0` | Optional boolean FutureProg ID with parameters `(character, location)`; a valid prog overrides the built-in trigger. The location is the vehicle's exterior room when driving. With no valid override, use the built-in trigger. |
| `DirectionSenseMinimumSightDifficulty` | `VeryHard` | Minimum effective light/weather sight difficulty that starts orientation. |
| `DirectionSenseMaximumObscuringWeatherLightMultiplier` | `0.3` | Maximum light multiplier for severe obscuring atmospheric weather. |
| `DirectionSenseDifficulty` | `Normal` | Initial check difficulty. |
| `DirectionSenseRecoveryDifficulty` | `Normal` | Recovery check difficulty. |
| `DirectionSenseNoProgressDelaySeconds` | `10` | No-progress time cost in seconds; never a movement stamina charge. Zero disables it. |

Luke approved these starting defaults on 17 October 2026. Existing worlds remain disabled until explicitly activated and correctly bound. Guarding's separate two-second blocked-exit delay is unchanged. No historical Wisdom arithmetic, leadership shortcut or sunslit exception is introduced.

The built-in trigger starts orientation when the body cannot see the supplied surroundings, when functional vision is completely lost, or when `IlluminationSightDifficulty(origin)` reaches the configured severity. Effective illumination includes the character's race light perception, darksight merits/effects, room light sources and weather light multiplier. The stock light model reaches VeryHard below 2 effective lux; the engine threshold is a configurable difficulty, so custom light models retain their own bands.

Exposed outdoor precipitation uses the existing world `MinimumSightDifficulty{Precipitation}` settings against the same threshold, without importing terrain Spot difficulty. Stock torrential rain/heavy snow are VeryHard; heavy rain is Normal and blizzard is Insane. Severe atmospheric weather triggers outdoors or in a climate-exposed interior when it has an authored atmosphere gas, obscures the sky, and its light multiplier is at/below the configured maximum. The seeded ordinary/choking dust multipliers are 0.7/0.3; thus severe dust can trigger in bright conditions even when precipitation is unchanged. Protected interiors do not inherit an outdoor storm-only trigger; their actual poor light/vision can still trigger orientation.

The optional prog can specialize or replace those conditions for a particular world. A valid false result explicitly suppresses the default trigger. A missing or incompatible override falls back to the built-in trigger. Exit visibility is always validated separately, because it controls whether a particular exit is known and does not measure whether surroundings can be seen sufficiently.

## Checks and installer binding

`DirectionSenseCheck` is CheckType **234**, with stable key `arm.check.direction_sense.initial`. `DirectionSenseRecoveryCheck` is CheckType **235**, with stable key `arm.check.direction_sense.recovery`. Both bind the stable trait key `arm.skill.direction_sense`.

The stock skill installers reserve nonbranching, zero-expression placeholders rather than claiming a magic or generic skill. The later Armageddon non-magic package must resolve and preflight its owned trait ID early, then call `DirectionSenseSeederHelper.BindOwnedTrait(context, ownedTraitIds)` before activation/chargen. This branch does not install the complete Armageddon non-magic roster or grant skills. That package is not present on this integration base.

The helper exposes `TraitKey`, `CheckBindings`, `InitialCheckKey` and `RecoveryCheckKey` as the early package binding contract. It refuses a missing/invalid character skill or conflicting existing checks/templates before changing either check. It binds the initial expression with `{nobranch}` and recovery with `{nobranch,noimprove}`. The initial template uses ordinary improving skill-check difficulty modifiers; the recovery template copies `Skill Check No Improvement` modifiers while explicitly disabling improvement and branching. Both fail if the bound trait is absent.

Repeated binding preserves compatible check/expression/template IDs, custom difficulty modifiers, trait caps, display-name changes and player values. Conflicting builder formulas are preserved and require explicit review. Placeholder replacement creates a new expression instead of modifying a potentially shared expression row. The helper never changes world activation or any character trait row. Runtime also refuses fallback/mismatched or unsafe check policies without rolling.

Legacy MySQL parameter bit columns default to true. New bindings explicitly persist their improvement/branch flags after insert, within the caller's transaction or an owned relational transaction, so the initial parameter cannot branch and the recovery parameter can neither branch nor improve after reload.

No schema migration is required. Existing worlds remain disabled and retain their existing traits/check bindings until explicitly configured. Before integrating parallel feature branches, recheck enum uniqueness and the common skill installer switch cases; the contemporaneous Peek IDs are 236–237 and Guarding IDs are 240–241.

## Qualification boundary

Managed tests exercise the outcome sequence, one initial/recovery call, invalid recovery/fallback policy, candidate filtering, no-progress behavior, command-parser routing, silent party readiness and additive seeder binding/rerun refusal.

Native gameplay qualification must be scheduled with the parent task because Armageddon S6 uses native resources concurrently. Allocated disposable database: `direction_sense_20261017_ds234`; allocated game port: `4534`. Obtain an immutable clean base from the coordinator and restore it once into the owned database, with separate MySQL data and binaries. Never reseed or operate S6's database, fixtures, listeners or processes. Do not treat managed tests, a build or production-loaded in-process checks as evidence that a real client flow passed. Start only after the coordinator releases the slot, and verify owned-resource cleanup before handing it to the next task.

The scheduled native scenario should batch representative walking, mounted party and manual vehicle intended/no-progress/wrong-direction cases, invalid/hidden/fall/unsafe exits, forced intended input, LeaveNoneBehind, late readiness failure, follower/occupant/tow no-reroll, route/autopilot exclusion, restart/ID preservation and the configured darkness/dust threshold boundaries. Verify no-progress stamina, ten-second delay and generic error output explicitly.
