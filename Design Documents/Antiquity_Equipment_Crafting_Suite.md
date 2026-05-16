# Antiquity Equipment Crafting Suite

This document records the craft suite that closes the remaining non-household antiquity gaps from `ItemSeeder.Rework.Antiquity.cs`: common clothing/accessories, textile and leather craft tools, non-leather armour, weapons, shields, ammunition, slings, and the construction hardware used by doors and gates.

## Target Surface

- 29 antiquity craft-tool prototypes from `SeedAntiquityClothing`.
- 18 common culture-neutral clothing/accessory prototypes from `SeedAntiquityClothing`.
- 76 non-leather armour prototypes from `SeedAntiquityArmour`.
- 117 weapon, shield, ammunition, sling, and military-accessory prototypes from `SeedAntiquityWeaponsShieldsAccessories`.
- Shared `Door Hardware Stock` used by the expanded household/door suite.

Existing jewellery, culture-specific textile clothing, leather clothing, leather armour, leather containers, leather furnishings, and household goods stay on their existing suites. The equipment suite discovers military goods dynamically from `Market / Military Goods` tags and excludes stable references already handled by the culture/leather suites.

## Implementation

The implementation lives in `DatabaseSeeder/Seeders/ItemSeederCrafting.AntiquityEquipment.cs` and is registered from `SeedCrafts()` through `SeedAntiquityEquipmentCrafts()`.

The suite follows the same stock pattern as the textile, leather, jewellery, and household work:

- Shared upstream commodity crafts run first.
- Final crafts produce the exact seeded prototype by ID and short description.
- Variable products copy colour characteristics from commodity inputs where the item has variable colour components.
- Craft names and visible echoes are generated from sanitised short descriptions, not stable references, so visible craft text remains culture-neutral.
- Knowledge-gated `AddCraft` overloads are used for both upstream and final crafts.

## Knowledge Gates

| Knowledge | Used For |
| --- | --- |
| `Ancient Equipment Crafting` | Shared construction and equipment stock, especially door and gate hardware. |
| `Ancient Weaponcrafting` | Weapon blade/head stock, shafts, bow staves, fletching stock, cord stock, ammunition, bows, slings, spears, blades, hafted weapons, and wooden weapons. |
| `Ancient Armourcrafting` | Helmet bowls, armour plates, rings, scales, lamellae, textile padding, shields, and final armour assembly. |
| `Ancient Toolmaking` | Metal, wooden, textile, and leather tool blanks plus final craft-tool prototypes. |
| `Ancient Common Clothing Crafting` | Culture-neutral common clothing and accessories left outside the culture garment suites. |

## Commodity Tags

All equipment stock tags are seeded under `Functions / Material Functions / Antiquity Equipment Stock` and exported to `Design Documents/SeededTagHierarchy.csv`.

| Tag | Purpose |
| --- | --- |
| `Weapon Blade Stock` | Blanks for swords, knives, daggers, and other blade weapons. |
| `Weapon Head Stock` | Spearheads, javelin heads, axe heads, mace heads, arrowheads, bolts, bullets, and bosses. |
| `Weapon Shaft Stock` | Spears, javelins, lances, arrows, bolts, clubs, staffs, and hafts. |
| `Bow Stave` | Worked bow blanks. |
| `Fletching Stock` | Light fletching material for arrows and bolts. |
| `Military Cord Stock` | Cordage for slings, bindings, bowstrings, lacing, and equipment ties. |
| `Shield Board Stock` | Wooden shield bodies. |
| `Shield Facing Stock` | Leather or hide shield faces that can carry colour variables. |
| `Helmet Bowl Stock` | Raised or formed helmet shells. |
| `Armour Plate Stock` | Plate, cuirass, greave, bracer, and similar metal armour stock. |
| `Armour Ring Stock` | Mail ring stock. |
| `Armour Scale Stock` | Scale armour stock. |
| `Armour Lamella Stock` | Lamellar armour stock. |
| `Armour Textile Padding` | Layered textile padding for armour and armour linings. |
| `Tool Blank Stock` | Reusable metal, wooden, textile, or leather stock for craft tools. |
| `Door Hardware Stock` | Hinges, straps, latch plates, bars, and fittings for doors and gates. |

## Verification

`ItemSeederAntiquityRemainingCraftingTests` parses `ItemSeeder.Rework.Antiquity.cs`, applies the same coverage rules as the seeder, and fails if a current prototype is not covered by either an existing suite or the new equipment/expanded household pass. It also asserts upstream commodity tags, knowledge-gated registration, culture-neutral visible string construction, and the corrected fresh-install `HasWeaponcrafting` / `HasArmourcrafting` source definitions.
