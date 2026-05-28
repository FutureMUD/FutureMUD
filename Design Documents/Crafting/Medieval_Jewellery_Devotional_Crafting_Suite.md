# Medieval Jewellery Devotional Crafting Suite

The medieval jewellery and devotional suite gives builders portable status goods and religious objects for daily wear, pilgrimage, household piety, court display, shrines, chapels, guilds, and personal keepsakes. It is broader than a luxury-only jewellery pass: many medieval characters should have some combination of fasteners, badges, beads, tokens, rings, or devotional containers.

## Culture Devotional Stock

For every culture key, the seeder creates:

- `medieval_devotional_{culture}_pilgrim_token`

These tokens are deliberately flexible. They can represent pilgrim badges, amulets, shrine tokens, household blessing pieces, court or office charms, and local devotional markers. They use wearable necklace behaviour and culture tags so builders can narrow them later.

## Common Jewellery And Devotional Catalogue

| Family | Stable References |
| --- | --- |
| Prayer and devotion | `medieval_devotional_wooden_rosary`, `medieval_devotional_icon_pendant`, `medieval_devotional_pilgrim_badge`, `medieval_devotional_scripture_tablet` |
| Reliquaries and keepsakes | `medieval_devotional_reliquary_locket`, `medieval_devotional_reliquary_box` |
| Fasteners and practical jewellery | `medieval_jewellery_silver_brooch`, `medieval_jewellery_bronze_ring_pin`, `medieval_jewellery_enamel_disc_brooch` |
| Status jewellery | `medieval_jewellery_inlaid_belt_mount`, `medieval_jewellery_court_circlet`, `medieval_jewellery_silver_finger_ring` |

## Component Use

| Surface | Components |
| --- | --- |
| Neck-worn devotional goods | `Wear_Necklace`, `Destroyable_Misc` or `Destroyable_HeavyMetal`. |
| Cloak fasteners and badges | `Wear_Shoulder` for shoulder or cloak placement. |
| Rings | `Wear_Ring`. Office signets live in the writing/administration suite. |
| Circlets | `Wear_Hat` with metal destruction behaviour. |
| Offering basin | `OfferingReceiver_Antiquity_VotiveBasin` until medieval-specific receiver variants are worth splitting. |
| Reliquary locket and box | Small container behaviour via `Container_Pouch`. |
| Belt fittings | `Wear_Waist` and `Beltable` so builders can attach or place them with belt stock. |

## Crafting

Crafts are registered through `SeedMedievalJewelleryDevotionalCrafts()` and use `Medieval Workshop Practice`.

| Material/Surface | Trait | Inputs And Tools |
| --- | --- | --- |
| Wooden beads, icon pendants, and tablets | `Carpentry` | Wood stock, cord, awl. |
| Bronze and silver brooches, rings, belt mounts, circlets, reliquaries, and tokens | `Blacksmithing` or `Silversmithing` | Metal tool stock, anvil, hammer. |
| Reliquary containers | `Silversmithing` | Metal stock with container components on the final item. |

## Builder Workflows

Use this suite to outfit:

- Peasants and travellers with pilgrim badges, tokens, bead strands, and simple pendants.
- Merchants, burghers, and guild members with brooches, rings, belt mounts, and badges.
- Nobles and courts with circlets, enamel brooches, reliquaries, and silver rings.
- Clergy and monastic spaces with rosaries, reliquary boxes, devotional tablets, and icon pendants.
- Shrines, chapels, monasteries, relic processions, alms scenes, and pilgrimage routes.

Future passes can add gemstone setting, ivory carving, enamel recipes, inscription behaviour, culture-specific iconography, and economy-tiered jewellery chains.
