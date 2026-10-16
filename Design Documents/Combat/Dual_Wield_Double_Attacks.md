# Dual Wield double attacks

Ordinary automatic melee selection can offer a coordinated pair alongside single attacks. Two distinct weapons attack the same captured target. Each strike retains its weapon-family skill, profile and fresh defence. The [approved plan](./Dual_Wield_Double_Attacks_Plan.md) defines the mechanics; the [verification receipt](./Dual_Wield_Double_Attacks_Verification.md) records checks, limitations and the Haggle/Analyse merge gate.

## Coordination and selection

One Normal `DualWieldCoordinationCheck` supplies signed difficulty stages to both attacks:

| Outcome | Stages |
| --- | --- |
| Major Pass | -1 |
| Pass / Minor Pass | 0 |
| Minor Fail / Fail | +1 |
| Major Fail | +2 |

Missing Dual Wield produces Major Fail without granting/branching the skill. Failure never gates strike two. A known coordination skill gets at most one improvement opportunity through one improvement-enabled parameter. Existing world-owned advancement trees and ordinary per-strike weapon advancement remain in effect.

Eligible profiles are ordinary `UseWeaponAttack`, single-target, with `Any` or `OneHandedOnly` handedness. Both items must be functional one-hand wielded melee weapons with usable arms. Shields, implants/prosthetics, special manoeuvres, multi-target attacks and existing dual-only profiles are excluded. `Dagger Dual Feint` remains one hit. Both weapons may be classified as dual wielded; neither has a privileged main/off-hand role.

Normal classifications, intentions, bodypart preferences and usability still apply. Manual/fixed and clinch actions do not gain a second strike. Affordable unordered profile combinations are sampled using the product of ordinary effective weights; both profiles and an unbiased lead order are frozen before the outer draw. One pair candidate has mass `weight multiplier * sum(weights of participating ordinary candidates)`. At 0.25 with only eligible singles this gives a 20% pair share, without multiplying chance for extra profiles or arms.

## Completion, resources and recovery

Admission requires stamina for both selected strikes, but only executed attempts pay. Each defence retains normal costs, failure spending, delays, magical resources and counters. Committed costs stay with captured physical bodies rather than replacements. Each child completes ordinary on-use, scoring and blood/poison handling under continuation guards; parent costs/effects are not repeated. A lethal hit retains completion while stopping the next strike. Tactics are notified once.

Two strikes use `1.5 * max(first delay, second delay)`, the harder completed recovery difficulty and one normal recovery roll. One strike uses its ordinary delay/result difficulty. An unexecuted pair returns to idle without weapon cost or attack recovery.

Live guards recheck bodies, original combat, target, contact, movement/clinch/grapple/blocking state, equipment, usability, authority and remaining stamina after callbacks. Incapacitation from a hit/counter, disarming, destruction, invalid arms, movement, retargeting, body replacement or combat exit stops the remaining strike. No victim/weapon substitution occurs. Defence exhaustion can yield an ordinary helpless response rather than cancelling a valid attack.

## Configuration and world installation

| Static configuration | Default |
| --- | --- |
| `DualWieldDoubleAttacksEnabled` | Runtime/upgrades false; fresh combat install true after binding |
| `DualWieldDoubleAttackWeightMultiplier` | 0.25 |
| `DualWieldDoubleAttackRecoveryMultiplier` | 1.5 |
| `DualWieldCoordinationDifficulty` | 5 / Normal |
| `DualWieldCoordinationMajorPassStages` | -1 |
| `DualWieldCoordinationPassStages` | 0 |
| `DualWieldCoordinationMinorPassStages` | 0 |
| `DualWieldCoordinationMinorFailStages` | 1 |
| `DualWieldCoordinationFailStages` | 1 |
| `DualWieldCoordinationMajorFailStages` | 2 |

Inspect with `show config <name>`; edit with the existing `editstaticconfig <name>` editor. Disabling bypasses pairing/coordination. Weight must be finite/nonnegative (zero or invalid disables), recovery finite/positive, difficulty integer 0-10, stages integer -10..10. Invalid tuning uses documented defaults with a deduplicated diagnostic; invalid enable/binding leaves ordinary attacks available.

Fresh CombatSeeder ensures a native coordination skill across supported packages, a Standard required-trait/nonbranching check and numeric binding. CoreData defers only the enable key until combat binds it, preserving explicit stored values. Both general skill routes create an unbound zero placeholder. Existing worlds stay disabled; a missing startup check receives a dedicated safe template and zero placeholder. No existing character gets the skill automatically. No EF schema migration or blank-snapshot refresh is required.

Existing-world builder workflow:

1. Find `DualWieldCoordinationCheck` and its expression ID with `show checks`; find the intended native character skill's numeric ID.
2. Use `te edit <expression-id>` then `te set formula dualwield:<skill-id>{nobranch}`. Inspect/save and close the normal editor. Supplementary attributes require `{noimprove,nobranch}`, for example `dualwield:123{nobranch} + dexterity:4{noimprove,nobranch}/10`.
3. Verify the template fails if any required trait is missing and prohibits missing-trait branching. A valid expression has one improvement-enabled character skill, no missing references/duplicate learning aliases and no branching parameters. Enable the flag and verify a representative pair. Disable to roll back selection without deleting traits or changing values.

Installer integrations may use `DualWieldCoordinationSeederHelper.BindExistingSkill(context, traitId, enableFeature: false)`. It validates IDs, replaces only a literal-zero placeholder, preserves custom formulas/settings and never grants skills. True enablement supplies only a missing flag, preserving explicit false. Do not rerun foundational CombatSeeder as an upgrade shortcut.

Armageddon uses the same explicit native binding; source-tree opening/cap/enrollment and unpublished continuation work are not dependencies. Publication integration assigns `DualWieldCoordinationCheck = 230`, leaving Haggle 227 and Analyse 229 intact. The earlier isolated native fixture used the unpublished 227 assignment; it is historical evidence, not a production migration contract. No published Dual Wield ID is renumbered. Blind Fighting #792 remains absent at this base and is not changed here. Preserve its ordinary `CombatCheck` wrappers if merged later, assessing sight per child/defence. Coordination is not vision influenced.
