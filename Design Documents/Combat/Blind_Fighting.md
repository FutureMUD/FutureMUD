# Blind Fighting

Blind fighting limits physical melee accuracy and active physical defences against a known opponent when sight is inadequate. It does not discover opponents, grant senses, change range or bypass a move's eligibility requirements. Adequate sight uses the existing check unchanged.

## Enable setting

`BlindFightingEnabled` is a boolean static configuration independent of the check formula. Its runtime default is `false`: existing worlds and upgrades with no stored setting keep pre-feature combat behaviour. Fresh CoreDataSeeder bootstrap installs `true` when the setting is absent; CombatSeeder then supplies the skill and check. Explicit stored values are preserved, and rerunning CoreDataSeeder on an installed world does not enable the feature or add a missing enable setting.

Use `show config BlindFightingEnabled` to inspect it, and `editstaticconfig BlindFightingEnabled` to edit the value to `true` or `false`. Changes take effect on subsequent checks without a restart. Invalid values are treated as disabled by combat assessment.

When disabled, combat uses the original checks without the blind-fighting TN cap, sight-difficulty floor, merit adjustment, auxiliary learning/branching check, or visibility assessment. Legacy vision penalties, native check caching, normal skill advancement and strategy previews retain their pre-feature behaviour. Disabling does not remove or rewrite the configured check, skill, merits or sight settings.

## Check calculation

`CombatCheck` wraps explicitly selected combat checks. The original check keeps its concrete roller, prerequisites, check type, trait expression, bonuses, burden, outcome and normal learning difficulty. `CheckEvaluationContext`, carried in the existing custom-parameter API, constrains resolution only:

```
base TN = min(original expression TN, BlindFightingCheck expression TN)
resolution difficulty = harder(prepared normal difficulty, prepared blind-fighting difficulty)
```

The original check's difficulty modifier is applied once, after the TN comparison. Normal combat burden and external bonuses remain on the normal check. The blind-fighting difficulty receives applicable signed merit stages, then ordinary innate and BlindFightingCheck-specific bonuses. The harder prepared difficulty wins. The legacy `BlindnessCheckBonus` adjustment is suppressed only for covered impaired-sight checks, avoiding a second vision adjustment; unrelated and adequate-sight checks retain their former behaviour.

All-difficulty results retain their original keys and shared roll set. Every threshold receives the same cap and difficulty floor. Normal advancement remains at the original reference difficulty (subject to the check's existing bonus handling and maximum improvement difficulty). Strategy previews use the same constraints without learning. OGL's shared innate bonus remains a direct DC adjustment rather than also staging the blind-fighting floor; BonusAbsent retains its absence of bonuses. Contextual Timebound checks bypass the unqualified outcome cache because it stores neither rolls nor sight constraints, and do not populate that cache. Unconstrained checks and the auxiliary learning check retain their native caching.

After a covered check actually evaluates, one separate ordinary `BlindFightingCheck` is used for learning/branching, at its own sight/merit difficulty and with its own outcome. That outcome never participates in the combat contest. The original weapon/tool trait is never supplied to this auxiliary check. Expression parameter improvement/branching flags, templates, learning models and continuation guards still apply. Previews, rejected limb prerequisites, cached results that do not evaluate, and constant expressions have no auxiliary learning roll. Existing combat reward checks are unchanged.

## Expression configuration and installation

Blind fighting is an ordinary configurable trait-expression check, not a hard-coded skill lookup. Fresh CombatSeeder installations create the `Blind Fighting` combat skill and a `BlindFightingCheck` expression such as `blindfighting:123`, with the actual seeded trait ID. An untrained character contributes zero for a missing skill; normal learnability and branching configuration govern acquisition. The skill has its own cap expression, initially matching Brawling's cap formula.

`BlindFightingCheck` uses persisted check type ID 242. ID 227 remains the shipped `HaggleCheck`; installing blind fighting preserves existing haggling checks and expressions.

The general skill seeders defer this check to CombatSeeder. Existing worlds booting without seeding receive a missing-check default of `100`. Existing checks and builder configuration are never replaced by the upgrade loader or blind-fighting seeding helper. An existing constant/custom check does not cause the seeder to invent a skill. This does not make the entire foundational CombatSeeder safe to rerun.

Use `show checks` to locate the check's trait expression and the normal `traitexpression`/`te` editor to alter it. While the feature is enabled, replacing a skill term with `100` removes skill-dependent limitation for ordinary base TNs up to 100, and the constant has no traits to improve or branch. The formula does **not** disable the mechanic: ordinary base TNs above 100 are still capped at 100, and the harder blind-fighting difficulty still applies. Use `BlindFightingEnabled=false` for a complete bypass. A larger constant can remove the TN cap over a larger range; difficulty policy is configured independently below. The reserved expression variable `variable` is not bound to the original combat skill.

## Sight and configurable difficulties

Assessment uses the opponent's current location and `IlluminationSightDifficulty`, including racial illumination multipliers, darksight merits and darksight effects. It uses `CanSee` with `IgnoreSpotting`, so assessment cannot generate spotting rolls or reveal a hider. Actual inability to see, zero functional vision, and effective pitch darkness use the unseen band. Administrator perception remains exempt. Partial eye impairment in otherwise adequate light retains existing behaviour.

The current general `Body.CanSee` implementation tests darkness under `IgnoreDark`, contrary to that flag's documented intent. Combat assessment therefore independently checks effective illumination rather than relying solely on `CanSee`. This feature does not rewrite general perception or target acquisition.

These static configurations store numeric `Difficulty` enum values (Automatic 0 through Impossible 10):

| Configuration | Default | Meaning |
| --- | --- | --- |
| `BlindFightingMinimumSightDifficulty` | 6 / Hard | First effective lighting band requiring blind fighting |
| `BlindFightingSightHardDifficulty` | 4 / Easy | Blind fighting in the Hard sight band |
| `BlindFightingSightVeryHardDifficulty` | 5 / Normal | Very Hard sight band |
| `BlindFightingSightExtremelyHardDifficulty` | 6 / Hard | Extremely Hard sight band |
| `BlindFightingSightInsaneDifficulty` | 7 / Very Hard | Insane sight band |
| `BlindFightingUnseenDifficulty` | 8 / Extremely Hard | Fully unseen or Impossible sight band |

Changing these does not change the underlying light model. Invalid difficulty configuration values fall back to the documented default.

## Merit

The `Blind Fighting` merit type exposes `stages <number>`, from -10 to 10. Negative values make the blind-fighting difficulty easier; positive values make it harder. Applicable distinct merit IDs stack additively. The total is applied and clamped to Automatic–Impossible before bonus adjustment and comparison with the normal difficulty. The merit cannot make a normal check easier than it would have been with adequate sight. It uses the existing character merit applicability, chargen, parent-merit, XML persistence and clone mechanisms.

## Coverage and boundaries

Covered accuracy and active opposition:

- Weapon and natural/unarmed attacks; block, parry, dodge and desperate variants. Inherited mounted, charge-impact, ambush, backstab and special melee hit paths use the same checks.
- Aimed weapon/natural attacks within a clinch and their dodge responses.
- Starting a clinch or grapple and the initial counter/dodge response.
- Wards, opposing a ward, ward free counterattacks and their dodge responses. `WardIgnore` remains a risk/courage check.
- Rescue contests, auxiliary attack accuracy, disarm/facing-change opposition and attacker/defender advantage contests.
- Physical block/parry/dodge against melee magic powers. Spell/psychic checks themselves remain unchanged.

Excluded: established-contact grapple extension, hold/escape, break-clinch, wrench/strangle/takedown contests; approach/disengagement/navigation; ranged attacks and their defences; penetration and damage; post-hit strength/balance/stagger/forced-movement/status resistance; recovery/stamina; passive magical absorption and rolled magical defences. Auxiliary position, delay and stamina resistance retain their original checks. Helpless/exhausted responses do not acquire new defence rolls. Non-character targets remain unchanged.

Multi-target melee attacks assess each actual opponent separately. Backstab's historical unseen opening is never used as a current sight flag; assessment cannot consume or refresh it. Backstab still requires attacker sight, and a defender who can now see an attacker revealed during engagement receives no blind-fighting limitation solely because an opening was captured earlier.
