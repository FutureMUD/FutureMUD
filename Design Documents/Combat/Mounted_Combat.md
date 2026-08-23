# Mounted Combat

## Scope

Mounted combat is the shared shock-and-mobility layer for riders and controlled vehicles. It covers animal mounts, chariots, motorbikes and other ground vehicles, flying mythical mounts, and swimming mounts or surface-water vehicles. The implementation deliberately resolves all of these through one mounted context so that charge checks, response logic, strategy cycling, messages, and size/momentum consequences cannot drift into separate special cases.

The runtime entry point is `IMountedCombatService`. A combatant receives a mounted context only while they are the primary rider of an animal mount or the current controller and occupant of an intact, operational vehicle. A passenger or secondary rider cannot originate the conveyance's charge.

## Domains And Momentum

The service classifies the conveyance at the moment the move resolves:

| Domain | Runtime condition | Seeded check | Charge message type |
| --- | --- | --- | --- |
| Ground mount | Primary rider; mount is not flying or swimming | `MountedChargeCheck` | `MountedCharge` |
| Aerial mount | Mount is in `PositionFlying` | `AerialMountedChargeCheck` | `AerialMountedCharge` |
| Aquatic mount | Mount is swimming, floating in water, or in a swimming layer | `AquaticMountedChargeCheck` | `AquaticMountedCharge` |
| Ground vehicle | Controller occupies an intact non-water vehicle | `VehicleChargeCheck` | `VehicleCharge` |
| Aquatic vehicle | Controller occupies an intact surface-water vehicle | `AquaticVehicleChargeCheck` | `AquaticVehicleCharge` |

Animal-mount momentum is derived from the mount's movement time. Vehicle momentum uses authored route speed where available and otherwise falls back to the active propulsion profile's move time. Both are bounded before they become check bonuses. Effective impact size comes from the ridden mount's riding-context size or the vehicle exterior item's size.

Flying and swimming charges begin one difficulty harder than ordinary ground charges. This represents the extra control required to preserve a three-dimensional or fluid attack line; their separate checks let a world tune that assumption without changing the move.

## Charge Resolution And Responses

An ordinary unmounted `ChargeToMeleeMove` retains its historical range-closing behaviour. A mounted context changes the move into an opposed shock action:

1. The attacker rolls the domain check with momentum and relative-size modifiers.
2. The defender rolls `OpposeMountedChargeCheck`; bracing makes this easier and contributes a flat defense bonus.
3. A winning charge grants offensive advantage, penalises the defender's defensive advantage, and may knock down or unseat a target when the conveyance is substantially larger, the charge wins strongly, or the target is themselves mounted or aboard a vehicle.
4. A charge-only wielded attack is delivered after the impact. Couched lances are preferred over other mounted weapon attacks.
5. An animal mount may also deliver one authored domain impact attack. This is a trample on land, an aerial sweep-through while flying, or an aquatic charge while swimming.

Ground trampling is only considered when the mount is larger than the target. This keeps "ride over" behaviour for small opponents without turning every mounted engagement into an automatic extra hoof attack. Vehicles do not borrow animal natural attacks; their size and momentum still contribute to the opposed impact and knockdown result.

Defensive strategies retain the existing receive-charge, stand-and-fire, and ordinary skirmish responses. Mounted strategies add two responses:

- `EvadeMountedChargeMove` opposes the charge using mobility. A successful defender stays out of melee and gains defensive advantage; a failed evasion lets the attacker establish melee and deliver the impact and mounted weapon follow-up.
- `CounterMountedChargeMove` is available to another controlling rider or driver. Both sides roll their domain charge checks. The loser can be knocked down or unseated and gives the winner offensive advantage.

Missing dedicated checks or combat-message rows in an upgraded world fail safely to `GenericSkillCheck` and built-in domain prose. Fresh worlds receive the dedicated checks and messages from the Combat Seeder.

## Mount Knockdowns And Rider Injury

When an animal mount becomes sprawled, every rider is removed before the mount can remain prone. Each rider rolls `AvoidMountFallCheck`; saddle and stirrup stability modifies the check, while knockdown force, relative size, and mount-to-rider weight ratio set its difficulty. This produces three consequential results:

- a strong result throws the rider clear on their feet;
- a marginal result drops the rider beside the mount and can apply ordinary fall damage;
- a failed result leaves the rider sprawled and crushed beneath the mount.

Crushing damage is blunt trauma targeted at a bodypart. It scales with knockdown success degrees, size difference, and the mount-to-rider weight ratio, with a major failure increasing the result. Damage is bounded so exceptionally large mythical mounts remain dangerous without producing unbounded numeric results. Upgraded worlds that do not yet contain `AvoidMountFallCheck` use `GenericSkillCheck` until their checks are reseeded.

## Authored Attack Types

Mounted attacks are deliberately charge-only and are excluded from the ordinary melee attack pool:

- `CouchedLanceAttack`: existing couched-lance follow-up, preferred when available.
- `MountedWeaponAttack`: another rider/driver weapon follow-up, such as the seeded Mounted Sabre Cut.
- `MountedTrampleAttack`: a larger ground mount's natural impact against a smaller opponent.
- `AerialSweepAttack`: a flying mount's pass-through natural attack.
- `AquaticChargeAttack`: a swimming mount's natural impact.

The Animal Seeder creates the three natural attacks and links Mounted Trample into suitable large herbivore loadouts. The Mythical Animal Seeder gives Aerial Sweep Through to Griffins and Hippogriffs and Aquatic Charge to Hippocamps. Foundational weapon seeding gives Short Spear and Long Spear lower-damage, harder mounted thrusts, and gives Longsword and Two Handed Sword lower-grade mounted cuts. Training equivalents preserve training damage and intentions. The Early Modern combat dependency adds the superior Couched Charge to lance types and Mounted Sabre Cut to sabre types; it filters the foundational fallback attacks while cloning donor attacks so specialist weapons receive exactly one purpose-built mounted move.

## Strategies

Three strategy modes are available for both preferred melee and preferred ranged strategy slots:

- `MountedCharge` closes and commits to shock combat. It can counter-charge an incoming mounted attacker. Controlled vehicles bypass the character's ordinary walking-position gate when the vehicle and target are colocated.
- `MountedSkirmish` fights from range and converts an incoming mounted charge into a mounted evasion when it still controls a conveyance.
- `MountedHitAndRun` charges from range, then uses an opposed mounted disengage at melee to sweep clear and rebuild another pass. This is not a full flee: combat continues and the opponent may check the disengage.

Fresh worlds also receive the global templates `Cavalry Charge`, `Mounted Skirmisher`, and `Mounted Hit and Run`. Players and builders editing a combat setting can select the underlying modes with `combat config melee <mode>` and `combat config ranged <mode>`; the seeded templates are preferable when a coherent complete preset is wanted.

## Skills And Checks

The complex skill package now includes `Driving` / `Drive`, described as control of carts, chariots, motorbikes, and other driven vehicles. Fresh Combat Seeder formulas use:

- Riding plus Veterancy for ground, aerial, and aquatic animal-mount charges;
- Driving plus Veterancy for ground-vehicle charges;
- Seafaring plus Veterancy for surface-water vehicle charges;
- Balance/Dodge plus Veterancy to oppose a mounted charge;
- Riding plus Balance for avoiding injury when a mount sprawls.

The older Skill Seeder retains generic variable formulas for the new check types so legacy installation paths remain complete.

## Vehicle Boundaries

A vehicle charge requires the character to be the vehicle's controller, an occupant, and backed by an intact exterior item. This is separate from `AquaticVehicleAttack`: a charge targets another character and resolves shock, spacing, and knockdown, while an aquatic vehicle natural attack targets a craft exterior and tests its occupants' stability. Neither path introduces hull damage.

## Verification Expectations

Changes to mounted combat should build both the core and Database Seeder projects and run the core and seeder unit suites. At minimum, tests should cover domain classification, primary-controller authority, strategy factory/classification, canonical seeded strategies, animal/mythical loadout links, mount-sprawl dismount and crush outcomes, fallback-versus-specialist weapon attack separation, and the Driving skill. Live validation should boot a local MUD, confirm the combat strategy names are parser-visible, and exercise an actual mounted charge and mount knockdown when the selected database contains a suitable mount or vehicle fixture.
