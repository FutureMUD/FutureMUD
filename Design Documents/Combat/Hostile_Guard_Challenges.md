# Explicit hostile guard challenges

This optional add-on is disabled by the per-world static boolean `ArmageddonHostileGuardAttemptsEnabled`, whose default is `false`. Installing skills or upgrading the server never enables it. Ordinary exit and item guarding, ally/exemption rules, person protection, manual Rescue and ranged interposition retain their existing behaviour.

## Player commands

| Command | Meaning |
| --- | --- |
| `north` | Ordinary movement; existing guard and safety restrictions apply. |
| `north !` | Existing safe-movement override; guards still block ordinarily. |
| `north !!` | Challenge an eligible exit guard; retain safe movement. |
| `north ! !!` | Explicitly request both operations. |
| `get [quantity] gem [chest] !!` | Challenge guards on the resolved room item or its containing chain. |
| `open chest !!`, `open north !!`, `open door north !!` | Challenge guards on the item or installed door being opened. Opening never authorises passage through an exit guard. |

The reserved standalone `!!` token follows the target/container and precedes an optional parenthesised emote. Duplicate, embedded, quoted-as-a-name or misplaced markers are rejected before target resolution. Explicit challenges against unguarded or already-permitted targets are rejected without a roll. Currency/weight gets, combat gets, corpses, another character's belongings, automation panels, component unloads, controlled vehicles and queued movement are outside this version. Automatic following and other commands do not inherit challenge authority.

## Reaction and authority

Each currently blocking guard must be present, able, colocated in the same room/layer and able to see the challenger. Existing exemptions still apply. Eligible guards react in ascending character ID order, once per real action, using Normal difficulty. Any successful guard blocks the whole challenged action. An intercepted exit attempt gives the challenger a two-second movement delay; intercepted item actions add no special delay. The attempt itself causes no crime or damage. Executed movement/item actions retain their existing law checks and consequences.

Preflight validates the ordinary action while isolating only the identified guard effects. It does not roll. All eligible reactions then resolve once; access previews and later checks consult the resulting typed `IGuardChallengeContext`. The context binds the actor/body, origin/layer, exact exit and actual movement cohort, or exact item/source ancestry and guard-effect instances. Room access and exit crossing consult it only at their guard predicates. It is never an effect or an `ItemCanGetIgnore`/safe-movement flag.

Movement retains the context through its delayed intermediate check and retires it on crossing/cancellation. Item bodies recheck ordinary access immediately before the transfer/open mutation and retire authority before subsequent item events. Changed source, actor/body, guard eligibility/effect, cohort, closed/locked doors, barriers, visibility, reach, planar interaction, posture, stamina, hands/weight and inventory permissions cannot be waived by a guard reaction. Newly arriving guards are not covered. Stale or redirected authority fails closed without another roll.

The runtime uses a narrowly scoped ambient adapter to carry this typed object through existing body/component access APIs. Unrelated movement creates its own ordinary scope; automatic followers never capture a leader's already-bound challenge. The object cannot authorise a different exit, item, actor or subsequent action.

## Checks, binding and upgrades

`GuardingExitChallengeCheck = 240` and `GuardingItemChallengeCheck = 241` are physical, vision-influenced general activities, outside offensive/defensive combat classifications. Both use one Guarding skill, with no challenger skill. Their stock template is `Guarding Challenge Check`: Standard method, ordinary difficulty modifiers, trait improvement enabled, branching disabled, and `DoNotFail` for a missing trait.

Missing-check startup/seeding adds unbound base-target `15` formulas and missing default-off configuration only. It creates no alias skill, grants no player traits and does not replace existing formulas/templates/settings. A world operator binds both formulas to the world's existing Guarding character-skill ID:

```text
max(15, guarding:<GuardingTraitId>{nobranch})
```

Use the existing trait-expression builder to edit the two expressions shown by `show checks`, or use the additive installer API `GuardingCheckSeederHelper.BindExistingSkill(context, traitId, out message)` while the world is offline. The installer validates an explicit existing character skill ID, preflights both checks, preserves custom expressions/templates, and replaces a stock placeholder expression rather than mutating a row another check might share. Repeating an unchanged binding preserves IDs and player values. It never enables the feature. Full Armageddon installers may call this helper with their owned Guarding trait ID; a matching name is not automatic ownership.

The `15` is a base target-number floor before difficulty/situational modifiers. It is not a guaranteed 15% final probability. Standard checks add their configured difficulty modifier after evaluating the expression and use the normal outcome-degree rolls. Only the reacting guard is the checkee and receives the ordinary improvement opportunity/cooldown; the challenger does not gain or branch a skill.

## Direction Sense integration

The string movement entry point resolves the visible intended exit before creating `HostileGuardChallenge.ForExit`. Direction Sense must resolve its actual outcome before this seam. No-progress outcomes never create/resolve a challenge. Misdirection must not transfer an intended-exit token to an alternate guarded exit or identify hidden destinations/guards. The context's exit identity and origin/cohort validation remain mandatory after the branches are integrated.
