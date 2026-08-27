# Ranged Weapon Balance Pass

## Purpose and scope

This pass balances primitive and modern ranged combat around the real combat engine and `impdebug combatsim`, while deliberately allowing modern firearms, explosives, and artillery to remain lethal. It covers aim acquisition and loss, range, posture, stamina, loading, fire modes, melee fireability, attachments, ammunition, armour interaction, cover, throwing weapons, explosives, and artillery.

The stock modern package is setting-neutral. Its names describe broad weapon archetypes rather than real manufacturers or models, so game owners can reskin it without inheriting a specific setting.

## Seeded representative arsenal

The modern firearms seeder installs fourteen shoulder and hand weapons: compact and service 9 mm pistols, a magnum revolver, a 9 mm submachine gun, compact and service 5.56 mm rifles, a 7.62 mm battle rifle, a 7.62 mm precision rifle, an anti-materiel rifle, pump and semi-automatic 12-gauge shotguns, 5.56 mm and 7.62 mm belt-fed machine guns, and a shoulder-fired anti-armour launcher.

The artillery catalogue adds 105 mm and 155 mm breech-loading howitzers plus 60 mm and 120 mm drop-fire mortars. Ammunition includes ball, armour-piercing, expanding, buckshot, slug, 40 mm grenade, rocket, mortar, and high-explosive artillery examples. The explosive catalogue includes fragmentation and concussion grenades, a plastic explosive charge, directional and pressure anti-personnel mines, and an anti-vehicle mine.

Soft ballistic armour and a hard-plate carrier provide representative modern protection alongside the existing unarmoured and medieval armour fixtures. Seeded modern attachment examples cover reflex, low-power variable, and precision optics; adjustable stocks; vertical grips; bipods and machine-gun tripods; calibre-appropriate suppressors; a muzzle brake; a laser aiming module; a powered weapon light; an underbarrel grenade launcher; and a modern bayonet.

## Balance model

### Aim, range, and posture

Aim gain uses the weapon type's accuracy expression and aim difficulty, then applies configurable combat/out-of-combat outcome multipliers and posture multipliers. Standing is the neutral baseline, kneeling improves aim acquisition by 10%, and prone improves it by 25%. Shooter movement and ordinary target movement each remove 33% aim; a target closing directly toward the shooter removes 15%.

`RangedWeaponTypes.MinimumFiringPositionStateId` persists the least permissive position from which a weapon may fire. Pistols and revolvers are standing-capable and fireable in melee. Ordinary longarms, shotguns, and submachine guns are not fireable in melee; their authored position and wield requirements reflect their class. Emplaced artillery is standing-operated. The builder surface exposes the value through `rangedweapon set position <state>`.

Range still uses the normal room-based ranged model. The combat simulator additionally accepts exact initial separation in metres, which is especially useful when the staged source is a RouteCell. Class-specific accuracy expressions make compact handguns degrade sooner than service rifles, precision rifles, and crew-served weapons.

### Stamina, loading, and firing

Stamina-to-fire, per-load-stage stamina, recovery delay, aim loss per shot, magazine capacity, and fire-mode recoil remain authored on the ranged weapon type or firearm component. Burst and automatic modes are finite volleys with cumulative per-round recoil rather than free sustained fire. Heavy weapons impose more loading and firing effort and normally require two hands or emplacement.

Modern breech-loading artillery and drop-fire mortars no longer inherit muzzle-loading linstock/fuse behaviour. Artillery loading performs the configured `ArtilleryLoadCheckDifficulty`; only a major failure aborts the loading action, while lesser failed outcomes complete with their normal time and stamina cost.

### Damage and protection

Projectile damage remains ammunition-led, allowing the same firearm to produce materially different results with ball, armour-piercing, expanding, buckshot, slug, or explosive ammunition. Impact-fused ammunition now resolves its detonator after the normal projectile impact, including when the projectile lodges in a target. High-explosive artillery projectiles are therefore not destroyed before their bomb component resolves.

The live target matrix covered unarmoured humans, medieval armour, soft modern armour, hard modern armour, deer, elephant, dragon, and additional mythical fixtures. Handgun fire was strongly lethal to humans but ineffective against the dragon in the recorded 20-shot trial. The 105 mm howitzer broke the dragon's left patella, detonated, knocked it down, and rendered it unconscious while the server remained stable through the resulting high-volume wound processing.

### Cover and attachments

Seeded cover includes portable and installed examples with enough directional spread for open-field, firing-line, and ambush scenarios. Combat-simulator staging now copies source-cell cover into the transient cell, and a participant may start using a selected cover item. This exercises the real cover bonus and ranged obstruction paths rather than approximating them statistically.

Attachment bonuses remain deliberately bounded and compositional. Optics improve accuracy or aim acquisition, supports trade mobility/setup for recoil control, suppressors reduce report while slightly affecting handling, the muzzle brake reduces recoil at the cost of greater report, and laser/light/launcher/bayonet examples require their corresponding host capabilities.

## Static configuration

The settings replacing engine magic numbers are `RangedAimStandingMultiplier` (`1.0`), `RangedAimKneelingMultiplier` (`1.10`), `RangedAimProneMultiplier` (`1.25`), the five `CombatAim*Multiplier` values (`0.03` through `0.50`), the six `OutOfCombatAim*Multiplier` values (`0.05` through `1.0`), `RangedAimShooterMovementLoss` (`0.33`), `RangedAimTargetMovementLoss` (`0.33`), `RangedAimTargetMovingTowardLoss` (`0.15`), and `ArtilleryLoadCheckDifficulty` (`4`).

## Simulator workflow

The ranged-specific staging options are available on both character and template participants:

```text
impdebug combatsim add template <template> team <team> range ranged metres <distance> aim <0-1> [cover <item>]
impdebug combatsim set <number> metres <distance>
impdebug combatsim set <number> aim <0-1>
impdebug combatsim set <number> cover <item|none>
impdebug combatsim run [force] [confirm-production]
impdebug combatsim report
impdebug combatsim transcript
```

Initial aim, exact separation, cover selection, copied cover definitions, and RouteCell topology are part of the execution fingerprint. This makes repeated tuning runs comparable and prevents a range or cover change from masquerading as an exact replay.

## Bugs corrected during live tuning

- firearm deep copies now preserve safety, selected mode, wield/chamber state, magazines, internal rounds, and installed attachments;
- artillery direct hits select a valid body part when the attack result did not provide one;
- firearm and artillery impacts resolve impact detonators after the projectile outcome;
- explosion propagation detects cyclic containment/connection/lodgement graphs;
- simultaneous severing and post-death witness routing no longer dereference missing body parts or locations;
- exit-trap explosive payloads restore detached installed components to their spatial anchor before detonation;
- trap payload delays accept both XML durations such as `PT0S` and invariant time spans, and the seeder now emits invariant values.

## Live evidence and spatial caveat

Raw transcripts are retained in [Ranged Weapon Live Test Logs](./Ranged_Weapon_Live_Test_Logs/). The two publication captures are derived from the exact howitzer and tripwire transcripts in that directory.

The direct howitzer showcase placed the gun crew and dragon in one ordinary cell, so the crew and bystanders were also inside the high-explosive blast. Real game artillery scenarios should separate the gun and target with rooms or RouteCell distance; the same-cell result is intentionally retained as evidence that explosive propagation is not granting the firing crew immunity.
