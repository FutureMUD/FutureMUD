# Modern Firearms, Attachments, and Alternate Fire Modes

## Scope

Modern firearms built from the `Gun`, `InternalMagazineGun`, and `BoltAction` item components can opt into modular attachment slots and one or more finite fire modes. Historical `Musket` components retain their bore-based ramrod, sight, and bayonet model; the two systems intentionally do not share persistence or builder commands.

The public runtime contracts are `IFirearm`, `IFirearmAttachmentHost`, and `IFirearmAttachment`. Attachments are ordinary items. They can combine `FirearmAttachment` with existing components: an electric-light component makes an attached flashlight usable, an `IRangedWeapon` component makes an underbarrel launcher independently loadable and aimable, and an `IMeleeWeapon` component on a bayonet-category attachment supplies the host firearm's melee profile while installed.

Modern and musket bayonets deliberately use different compatibility components. A modern bayonet combines `FirearmAttachment` and `MeleeWeapon`: `FirearmAttachment` matches a `Bayonet` slot and form factor such as `modern-lug`, while `MeleeWeapon` defines the attacks, traits, classification, and damage profile. When installed, the host firearm's `IMeleeWeapon.WeaponType` dynamically delegates to the first installed bayonet-category attachment that has an `IMeleeWeapon`; detaching it immediately restores the firearm prototype's ordinary melee type. The older `BayonetAttachment` component remains the musket-only bore/style system and should not be added to a modern bayonet.

## Attachment Authoring

Firearm prototypes have no slots unless a builder adds them. Each slot has a unique name, a category, and one case-insensitive form factor:

```text
component set slot add <name> <category> <form-factor>
component set slot remove <name>
```

Categories are `optic`, `stock`, `grip`, `muzzle`, `barrel`, `underbarrel`, `side`, `bayonet`, and `other`.

Create an attachment with the `firearmattachment` component loader. Its builder options are:

```text
component set category <category>
component set formfactor <name>
component set require add <capability> [reason]
component set require remove <capability>
component set require clear
component set accuracy <bonus>
component set aim <bonus>
component set damage <multiplier>
component set range <multiplier>
component set recoil <multiplier>
component set stamina <multiplier>
component set delay <multiplier>
component set aimloss <multiplier>
component set loudness <enum-step offset>
component set fireemote <emote|none>
```

An attachment may support multiple form factors. Multipliers must be non-negative. Accuracy and aim bonuses add; damage, range, recoil, stamina, delay, and aim-loss values multiply; loudness offsets add and are clamped to the `AudioVolume` range. The first installed attachment in authored slot order with a fire-emote override supplies the trigger emote.

Sibling capability requirements protect composed attachment prototypes without hard-coding particular component classes. Missing requirements are shown as builder warnings after item-component attachment or detachment and in `item show`; they block submission and review approval. The stock modern bayonet mount requires `IMeleeWeapon`, the launcher mount requires `IRangedWeapon`, and the weapon-light mount requires `IProduceLight` plus `IProducePower`.

Players install and remove components with:

```text
attach <attachment> <firearm> [slot]
detach <firearm> <attachment>
```

The slot name may be omitted only when exactly one free compatible slot exists. Installation consumes the held attachment, makes the firearm its containment and spatial host, adds its weight and buoyancy to the firearm, and persists the item ID against the slot name.

## Fire Modes and Weapon Actions

Firearm prototypes always have at least one mode. Builders use:

```text
component set mode add <single|burst|automatic> <rounds> <recoil> <extra-stamina> <extra-delay>
component set mode remove <single|burst|automatic>
component set cycle <manual|self-loading>
```

Single mode must fire one round. Any mode is limited to 10 rounds per trigger. Automatic mode is a finite authored volley, not a scheduled held-trigger action. Manual actions can use only single fire; self-loading actions cycle between rounds until the selected volley is complete or ammunition runs out.

Players select a supported mode through the ordinary switch surface:

```text
switch <firearm> single
switch <firearm> burst
switch <firearm> automatic
switch <firearm> safe
switch <firearm> unsafe
```

The selected mode and safety state are live-item state. Recoil penalties accumulate by round within a volley. Extra stamina and recovery delay apply once to the trigger action according to the configured volley size.

## Shotgun and Multi-Projectile Ammunition

`AmmunitionType` adds three builder fields:

```text
ammunition set projectiles <1-32>
ammunition set scatter <inherit|arcing|ballistic|light|spread>
ammunition set spread <non-negative outcome penalty>
```

Legacy rows load as one projectile, inherit the weapon scatter strategy, and have no spread penalty. A cartridge creates one casing and between 1 and 32 independently resolved projectiles. The combat move performs one trigger accuracy check and one primary defense; later rounds and projectiles stage that outcome down using the selected fire mode's recoil and the ammunition's spread penalty.

Every projectile independently resolves cover, obstruction, damage, lodging, breakage, and scatter. `Spread` confines secondary selection to the impact cell and can strike any eligible perceiver other than the shooter and original target, including allies and bystanders. Ballistic ammunition retains the existing neighbouring-cell ricochet model. Collateral character and item impacts run the ordinary assault-with-a-deadly-weapon or vandalism crime checks.

## Attached Active Components

Attached items are included in `IGameItem.AttachedAndConnectedItems` and are targetable through `attachment@firearm` (the reverse order is also accepted by the general target parser). Manual load, unload, ready, unready, and aim discovery includes ranged weapons attached to a held or wielded firearm. Combat strategies likewise consider an attached ranged weapon only through a wielded host.

An underbarrel weapon keeps its own chamber, magazine, ranged type, aim state, and ammunition. Firing it does not consume the host firearm's ammunition or apply the host's selected fire mode. Other active attachment components, such as lights and switches, continue to use their existing commands once targeted.

Explosive launcher ammunition composes the fired projectile from `Bomb` plus `ImpactDetonator`. `Bomb` owns the explosion size, volume, proximity, emote, and damage expressions. `ImpactDetonator` is only the trigger policy: after the shared ammunition pipeline resolves the projectile's hit, cover interception, obstruction, miss, or scatter landing, it detonates the sibling `Bomb` at that resolved location. A detonatable projectile without `ImpactDetonator` retains its other trigger policy, such as a fuse or radio detonator.

`ImpactDetonator` declares a fixed `IDetonatable` sibling requirement, so an impact-trigger component cannot be approved on a projectile prototype without a bomb or another detonatable payload.

## Concrete Composed Examples

The combat seeder supplies four loadable demonstration prototypes in addition to their reusable component profiles:

| Unique item prototype | Component composition | Behaviour |
| --- | --- | --- |
| `ModernFirearms_Bayonet_Example` | `Holdable` + `Modern_Bayonet_Mount` + `Melee_Modern_Bayonet` | Fits a `modern-lug` bayonet slot. While installed, its melee weapon type replaces the host firearm's normal butt-strike profile. It remains a usable melee weapon when detached. |
| `ModernFirearms_Underbarrel_Launcher_Example` | `Holdable` + `Underbarrel_Launcher_Mount` + `Launcher_40mm_Underbarrel` | Fits a Picatinny underbarrel slot and remains an independent single-shot, manual-action ranged weapon. Target it through the attached-item syntax for load, ready, aim, and fire commands. |
| `ModernFirearms_Weapon_Light_Example` | `Holdable` + `Weapon_Light_Mount` + `ElectricLight_WeaponMounted` + `BatteryPowered_WeaponLight` | Fits a Picatinny underbarrel slot, accepts two CR123 battery items, and uses the ordinary switch command to turn its focused light on or off. It is an alternative to the launcher when the host has only one underbarrel slot. |
| `ModernFirearms_40mm_Grenade_Round_Example` | `Holdable` + `Stack_Number` + `Ammunition_40mm_Low_Velocity_Grenade` | Loads into the example launcher and creates a casing plus an impact-detonating `Bomb` projectile when fired. |

For example, after loading the rifle and attachment prototypes into play:

```text
attach bayonet rifle bayonet
detach rifle bayonet

attach launcher rifle underbarrel
load launcher@rifle
ready launcher@rifle

attach flashlight rifle underbarrel
switch flashlight@rifle on
switch flashlight@rifle off
```

The exact item keywords depend on skins or builder edits. The `attachment@firearm` form is useful when the attachment and host have distinct keywords; the general target parser also accepts the reverse chained form.

## Persistence and Compatibility

Attachment slots, fire modes, and cycle type are stored in component-prototype XML. Installed attachment IDs, selected fire mode, chamber state, and safety state are stored in live component XML. Unknown or missing legacy elements load as no slots, single fire, and the component family's historical default action (`Gun` self-loading, `InternalMagazineGun` and `BoltAction` manual).

The `FirearmAttachmentsAndProjectileAmmunition` migration adds `ProjectileCount`, nullable `ScatterType`, and `SpreadPenalty` to `AmmunitionTypes`. It does not add attachment tables because slots and installed item links follow the established component XML persistence model.

## Stock Samples

When modern guns are selected, the combat seeder idempotently ensures:

- `Shotgun_12_Gauge_Pump` and `Rifle_556_Select_Fire` component profiles;
- `12 Gauge Slug`, nine-projectile `12 Gauge 00 Buckshot`, `5.56x45mm Ball`, and arcing `40mm Low-Velocity Grenade` ammunition types;
- reflex optic, adjustable stock, vertical grip, 5.56 suppressor, weapon-light mount, underbarrel-launcher mount, and modern-bayonet mount attachment profiles.
- reusable melee, launcher, light, battery-power, bomb, impact-detonator, and grenade-ammunition component profiles, plus the four composed item prototypes listed above.

These are builder examples rather than a broad weapon catalogue. Builders may reuse individual profiles, clone the composed items, or substitute their own weapon types, battery formats, illumination values, and explosive damage expressions.
