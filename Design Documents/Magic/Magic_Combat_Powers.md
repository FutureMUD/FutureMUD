# Magical and Psychic Combat Powers

## Runtime and persistence

`magicattack` retains its existing aliases and weapon attack profile. Older XML loads as melee, with damage enabled and no control effects. New definitions add `AttackRange`, `RangeInRooms`, `DealsDamage`, `AttackEmote` and typed `AttackEffects`. Builders can clone and save these definitions without a database migration.

Invoke the configured verb through the power's school command. Manual attacks queue a selected combat action; strategies also select eligible powers through their magic and psychic attack channels. The move rechecks access, resources, stamina, target visibility, movement restrictions and range before committing. Invalidated queued attacks cost nothing. A committed attack pays once, including when it misses or is resisted.

Ranged magic uses the natural ranged targeting contract, including sight, cover, room layers, RouteCell distance and vehicle boundaries. It needs neither ammunition nor a natural weapon. Psychic connections do not extend this range. Pull only establishes melee contact with a colocated opponent; it never transports characters between cells.

An attack must hit before any control effect is attempted. Each effect then independently opposes the casting trait with the existing defender check. Resolution order is clinch breaking, disarm, stagger, knockdown, then movement. Applicability is rechecked between effects. Pull and pushback cannot coexist on one power.

| Rider | Defender check | Result |
|---|---|---|
| BreakClinch | ResistBreakClinch | Existing clinch cleanup and cooldown |
| Disarm | OpposeForcedMovementCheck | Removes an eligible wielded item and applies no-get delay |
| Stagger | StaggeringBlowDefense | Stagger effect and combat delay |
| Knockdown | OpposeForcedMovementCheck | Existing combat knockdown, including mount handling |
| Pushback | OpposePushbackCheck | Existing forced movement breaks melee contact |
| Pull | OpposeForcedMovementCheck | Colocated participants enter melee |

Strength scales the successful contest; duration controls temporary effects where applicable. Control-only powers set damage off. Their successful effects do not require wounds.

## Active magical defenses

`magicdefense` implements `IMagicDefensePower`. Its saving sustained effect owns remaining charges, capacity and scheduler duration. Reloading restores that state instead of replenishing it. Cancellation, depletion, expiry, loss of access, failed upkeep and concentration disruption remove the effect and its upkeep subscription.

| Mode | Defensive opportunity |
|---|---|
| Opposed | Casting check prevents the attack on success |
| Charged | Same opposed check; success spends one interception charge |
| Absorption | Intercepts damage using a finite pool; overflow reaches the body |

Only one power replaces the ordinary defense for an attack. Multiple active powers are alternatives. Threat flags opt into weapon, natural, ranged, magic and dedicated control responses; combined classifications require the corresponding flags. Unsupported moves receive their ordinary response. Mental intrusion retains its own defenses.

Reaction resource and stamina costs are paid when the defense commits. A failed opposed attempt costs resources but retains interception charges. Misses do not drain capacity. Absorption removes the largest damage/pain/stun channel from the pool and scales every channel proportionally before ordinary armour. Full interception stops attached riders; overflow permits their resistance checks. Absorption cannot oppose a control-only attack.

Selection compares casting chances with available mundane defenses. Applicable absorption scores as certain interception while it has capacity. Explicit defense preferences take priority; mundane defense wins equal scores unless magic is preferred. Builders can filter damage types, require sight of the attacker, require the attacker to see the defender, require an upright position or free hands, and supply a Boolean eligibility prog taking defender and attacker. Projectile filters conservatively require all reported projectile damage types to be eligible.

Players explicitly activate a defense using its begin verb and cancel it using its end verb. There is no automatic activation.

* `combat defense magic` prefers available magical defenses.
* `combat powerdefense` lists active powers, reaction costs and remaining protection.
* `combat powerdefense off` cancels all active magical defenses and their upkeep.

## Builder examples

Create an attack profile with `weaponattack`, then create a `magicattack` power using the school, casting trait and profile. In the power editor:

```text
magic power set range ranged 3
magic power set damage false
magic power set rider Knockdown Normal 1 2
magic power set rider Knockdown success $1 reel|reels under $0's force and fall|falls!
magic power set rider Knockdown resist $1 resist|resists $0's telekinetic pressure.
```

`range melee` restores melee targeting. `rider <type> remove` removes a rider. `attackemote <text>` changes the attack echo. Existing intention, defense, cost and invocation-prog controls remain available.

Create an equipment-free parry with:

```text
magic power edit new magicdefense "Advanced Psionics" "Telekinetic Parry" "Psionic Discipline"
magic power set beginverb kineticguard
magic power set endverb endkineticguard
magic power set mode Opposed
magic power set cost kineticguard Focus 2
magic power set cost reaction Focus 2
magic power set stamina 1
magic power set duration 120000
```

The inherited sustained duration expression is in milliseconds. Set upkeep, concentration, requirements and echoes using the power's sustained and defense help. Threat and damage-type commands toggle the selected flag/type.

For phantom doubles, use `mode Charged`, `charges 3`, and `attackervision true`. Supply an eligibility prog when the setting has additional illusion immunity rules. Use success echoes to describe a double vanishing. The doubles are finite reactions, not separately targetable entities.

For an earth-conditioned barrier, use `mode Absorption`, `capacity 30`, opt into the desired physical threat families and set `eligibility <prog>`. The Boolean prog receives `(defender, attacker)` and can check terrain or other setting-specific conditions. Configure `hands` and `upright` as appropriate. An earth-wall echo describes the interception; this power does not create persistent terrain or block unrelated movement.

Activation/cancellation echoes use `$0` (defender). Success/failure echoes additionally use `$1` (attacker).

## Advanced Psionics defaults

`PsionicStockContent.CombatPowers` is the canonical tuning source. Only Advanced Psionics receives these powers. Basic Psionics, existing `psychicbolt`, capability grants and authored customisations are preserved. Seeder reruns insert missing stock definitions and capability entries without replacing existing content.

| Power | Band | Focus activation/attack | Reaction | Protection or range |
|---|---:|---:|---:|---|
| Force Strike | 20 | 3 | — | Melee damage |
| Force Lance | 20 | 5 | — | Ranged damage, 3 rooms |
| Psychic Trip | 40 | 4 | — | Knockdown, 1 room |
| Concussive Pulse | 40 | 4 | — | Stagger, 1 room |
| Repulse | 40 | 5 | — | Melee pushback |
| Wrench | 40 | 6 | — | Disarm, 1 room |
| Draw Foe | 40 | 4 | — | Local pull |
| Break Hold | 40 | 4 | — | Break caster's clinch |
| Kinetic Parry | 20 | 2 | 2 | Opposed reaction |
| Phantom Doubles | 60 | 8 | 1 | 3 interceptions |
| Kinetic Barrier | 60 | 10 | 1 | 30 capacity |

Attacks take 3 seconds and 2 stamina. Damage/pain are `4 + degree`; stun is `6 + 2 * degree`. Riders use Normal resistance, strength 1 and 2 seconds where relevant. Defenses last 120 seconds, consume 1 Focus/minute upkeep, 1 concentration point, a -2 sustain penalty and 1 stamina/reaction. These finite costs target the 100-Focus economy: ranged reach costs more than melee, reusable parry pays per attempt, and doubles/barriers front-load their finite protection. Builders can tune these defaults for their world's ordinary attack profiles and resource regeneration.

Runtime tests cover stock XML loading/round trips, legacy definitions, independent resistance, commitment costs, charge conservation, barrier scaling, preference and effect lifecycle. Seeder tests cover package isolation, additive reruns, authored-content preservation, identities and bounded stock costs. Physical walls, independently targetable decoys and area attacks remain outside this feature.

## Attached spell effects

An attack can reference a separate spell in the same school with `magic power set spell <spell>` and choose its level with `magic power set spellpower <level>`. Use `spell none` to detach it. The spell's `effect` list applies to the struck target and its `castereffect` list applies to the attacker. Character attacks require an `attackcharacter` trigger; item attacks require `attackitem`. These triggers supply exactly one target of the declared type, are not cast commands, and keep their spells out of player spell lists even if a known-spell prog would otherwise grant access.

Configure the payload using the ordinary spell editor:

```text
magic spell edit new "Force Strike Payload" "Advanced Psionics"
magic spell set trigger new attackcharacter
magic spell set trait "Psionic Discipline"
magic spell set effect add blindness
magic spell set duration <duration-expression>
magic spell set castereffect add magicresourcedelta
magic spell set castereffect 1 resource Focus
magic spell set castereffect 1 formula 1
magic power edit "Advanced Psionics: Force Strike"
magic power set spell "Force Strike Payload"
magic power set spellpower Standard
```

A payload must be ready and target-compatible before an attack can commit. It does not require a casting emote, spell knowledge, material components, spell resource costs or casting cooldowns: the attack supplies the paid action. It retains the spell's target resistance, target echoes, effect duration, parent effects and cleanup. Persistent caster effects also require a duration expression. Duration `degrees` and effect outcome use the successful attack check; the configured spell power supplies `power`. As with prepared trap spells, resisting the payload stops its target and caster effect lists.

Misses and full magical interception stop the entire payload. Character attacks resolve damage and control riders before applying it. A control-only attack may carry a spell without any built-in riders. Native attack damage controls absorption eligibility; payload-only attacks use opposed defenses and the spell's resistance. Spell costs are not added to attack costs; builders should budget the payload into the power's invocation cost.

## Magical item smashing

`magicsmash` is a separate power type with a dedicated `MagicPowerSmashItem` combat move. Create it with the same casting-trait and attack-profile arguments as `magicattack`:

```text
magic power edit new magicsmash "Advanced Psionics" "Shatter" "Psionic Discipline" <attack-profile>
magic power set verb shatter
magic power set cost shatter Focus 5
magic power set spell "Item Shatter Payload"
```

The optional spell must use `attackitem`; its caster effects still need character-compatible templates. Character control riders are unavailable. The move targets visible local items, supports the inherited ranged configuration, rejects destroyed/deleted targets and items carried directly by another character, and revalidates before paying. In combat it queues normally. Outside combat it resolves immediately, spends stamina once and imposes a delay shared by magical smash powers. It does not require a wielded weapon or cause weapon/bodypart recoil damage.

Successful item hits apply their spell payload before native smash damage, so caster effects still resolve when that damage destroys the item. Native damage uses the existing smash formula and ordinary item damage processing; attempts participate in vandalism checks. Item targets do not receive character defenses. This is not a way to use character control riders on equipment.

## Verification record

The default fast suites passed: 2,744 core tests, 755 seeder tests and 687 tests in the other seven suites (4,186 total). The 29 focused combat tests also cover legacy tape load/save compatibility, typed attack triggers, target and caster effect execution, payload reference persistence, cost-preserving verb edits and magical item-smash validation. Core and seeder Debug builds succeeded.

All eleven powers were installed in a uniquely named disposable MySQL clone; two consecutive reruns preserved their IDs. The legacy Tape compatibility loader resolved the earlier startup blocker, and the world booted successfully. Live builder commands created and attached both payload contexts, kept the payload spells out of player lists, and activated/listed/cancelled Kinetic Parry. An item smash costing 3 Focus with a +1 Focus caster effect changed Focus from 100 to 98; an immediate retry was refused without cost; a later damaging smash changed it from 98 to 96. These are functional checks, not comprehensive live balance calibration of the stock suite.

A queued, damage-free Force Lance delivered its character-target payload in live combat. The target echo rendered, the +1 Focus caster effect ran after the 5 Focus attack cost (100 to 96), and the attack spent 2 stamina. The test combat was then ended with the administrator peace command.
