# Medieval Equipment Crafting Suite

The medieval equipment suite seeds a usable military baseline for every culture slice rather than only a signature weapon. Each of the 18 cultures receives armour, a weapon, a shield, and six supporting equipment pieces so builders can outfit guards, levies, knights, caravan escorts, ships, watch posts, and battlefield scenes without hand-building every visible layer.

The suite intentionally uses existing armour, weapon, shield, wearable, container, sheath, repair, and destroyable component prototypes. It does not add new combat rules.

## Catalogue Scope

For every culture key in `Medieval_Crafting_Audit.md`, the seeder creates:

- `medieval_military_{culture}_armour`
- `medieval_military_{culture}_helmet`
- `medieval_military_{culture}_padded_coif`
- `medieval_military_{culture}_sidearm_harness`
- `medieval_military_{culture}_arrow_quiver`
- `medieval_military_{culture}_field_pack`
- `medieval_military_{culture}_war_banner`
- `medieval_weapon_{culture}_{weapon}`
- `medieval_shield_{culture}`

This gives the v1 equipment slice 162 culture-shaped military prototypes, plus common crossbow stock:

- `medieval_weapon_common_crossbow`
- `medieval_weapon_common_crossbow_bolts`

Representative weapon stable references include `medieval_weapon_early_anglo_saxon_seax`, `medieval_weapon_norse_bearded_axe`, `medieval_weapon_norman_arming_sword`, `medieval_weapon_gaelic_long_spear`, `medieval_weapon_german_hre_war_hammer`, `medieval_weapon_seljuk_ayyubid_cavalry_mace`, `medieval_weapon_steppe_turkic_sabre`, and `medieval_weapon_song_china_dao`.

## Culture Equipment Matrix

| Surface | Purpose | Component Families |
| --- | --- | --- |
| Armour package | The culture's main combat silhouette: mail, lamellar, padded, or reinforced armour. | `Armour_Chainmail`, `Armour_Lamellar`, `Wear_Hauberk`, `Destroyable_Armour`. |
| Helmet | Head protection paired with the armour package. | `Wear_Hat`, armour behaviour, `Destroyable_Armour`. |
| Padded coif | Under-helmet or light head protection for guards and soldiers. | `Wear_Hat`, `Armour_HeavyClothing`, `Insulation_Major`. |
| Weapon | Signature regional melee weapon. | Existing melee weapon component selected by the culture profile. |
| Shield | Regional defensive surface. | Existing shield component, `Melee_Shield`, `Destroyable_Armour`. |
| Sidearm harness | Worn carriage for blades, axes, maces, spears, or officer sidearms. | `Sheath_Large`, `Wear_Waist`, `Beltable`. |
| Arrow quiver | Missile-carriage accessory even when the culture's primary weapon is not a bow. | `Container_Quiver`, `Wear_Shoulder`. |
| Field pack | Soldier's pack for rations, cord, wax, repair stock, and campaign papers. | `Container_Pack`, `Wear_Backpack`. |
| War banner | Unit, household, ship, guild, clan, or religious military marker. | Holdable textile prop with clothing destruction behaviour. |
| Common crossbow | Cross-culture medieval missile weapon for towns, castles, militias, hunts, and siege-adjacent scenes. | `Crossbow`, `Melee_Improvised Bludgeon`, `Destroyable_Weapon`. |
| Crossbow bolts | Short bolt stock for crossbow users. | `Ammo_BroadheadBolt`, stack support; separate quivers remain carriage containers. |

## Knowledge Gates

| Surface | Knowledge Pattern | Trait |
| --- | --- | --- |
| Armour and helmets | `Medieval Armour Pattern {culture}` | `Armourcrafting` |
| Padded coifs | `Medieval Armour Pattern {culture}` | `Tailoring` |
| Weapons | `Medieval Weapon Pattern {culture}` | `Weaponcrafting` |
| Shields | `Medieval Shield Pattern {culture}` | `Armourcrafting` |
| Harnesses, quivers, packs, banners | `Medieval Weapon Pattern {culture}` | `Leathermaking` or `Tailoring` |

Visible craft blurbs and echoes stay culture-neutral. Culture appears in knowledge gates, tags, stable references, and builder notes.

## Craft Inputs And Tools

The suite reuses existing equipment commodity tags and adds medieval intermediate steps:

- `Mail Wire Stock`, `Armour Ring Stock`, `Mail Panel Stock`, `Armour Lamella Stock`, and `Quilted Armour Padding`.
- `Weapon Blade Stock`, `Weapon Head Stock`, and `Weapon Shaft Stock`.
- `Crossbow Prod Stock`, `Crossbow Tiller Stock`, and `Crossbow Lockwork Stock` for crossbow manufacture.
- `Shield Board Stock` and `Shield Facing Stock`.
- `Scabbard Leather Stock`, `Prepared Leather Panel`, `Leather Strap`, `Garment Cloth`, `Spun Yarn`, and `Tool Blank Stock`.

Required tools are backed by shared historic and medieval production items: `Anvil`, `Hammer`, `Forge Tongs`, `Awl Punch`, `Sewing Needle`, `Shears`, `Drawplate`, `Armourer's Anvil`, `Planishing Hammer`, `Mail Riveting Tongs`, `Bow Press`, `Tillering Stick`, `Crossbow Tiller Jig`, and `Locksmith File Set`.

Medieval-only installs seed the base stock ladder for this suite: `Tool Blank Stock`, `Weapon Blade Stock`, `Weapon Head Stock`, `Weapon Shaft Stock`, `Fletching Stock`, `Military Cord Stock`, `Shield Board Stock`, `Shield Facing Stock`, `Armour Lamella Stock`, and the leather/textile/wood stock consumed by harnesses, padding, quivers, banners, and packs.

## Builder Workflows

Builders can use the culture equipment set as a stock outfitting spine:

- Select a culture profile for guards, caravan escorts, levy troops, ship crews, or officers.
- Combine military clothing with armour, helmet or padded coif, sidearm harness, shield, pack, and banner.
- Replace regional text or components later if a world wants stricter chronology or a narrower local kit.
- Use repair kits from the medieval slice for field maintenance scenes.

## Deferred Equipment Scope

Full plate armour, specialised horse barding, rules-aware tack, artillery, hand firearms, cranequins, windlasses, and goat's-foot lever workflows are left for later slices. The v1 goal is a complete wearable/visible kit plus stock-level mail and crossbow manufacture using stable existing runtime behaviour.
