# Medieval Jewellery and Devotional Crafting Suite

The medieval jewellery/devotional suite should support personal adornment, religious identity, pilgrimage, office authority, and luxury display.

The current scaffold has common devotional and jewellery objects plus a generic culture devotional token. The second pass should stop relying on `regional devotional token` clones and add explicit culture-specific worn objects.

## Design Principle

Personal adornment and devotional goods should be a compact but visible way to show culture, status, office, and faith.

## Common Surface To Retain

Common medieval items may include:

- Wooden prayer bead strands
- Cloak brooches
- Reliquary lockets
- Ring pins
- Enamelled disc brooches
- Belt mounts
- Court circlets
- Icon pendants
- Pilgrim badges
- Reliquary boxes
- Devotional tablets
- Offering basins
- Silver finger rings

These are useful as common stock, but a generic `regional devotional token` should not satisfy explicit culture catalogue requirements.

## Culture-Specific Worn Targets

Culture-specific jewellery/devotional additions should be linked to the explicit culture catalogue.

Examples:

| Culture | Worn Jewellery/Devotional Direction |
| --- | --- |
| Early Anglo-Saxon | disc brooches, bead necklaces, cloak pins, small wooden crosses |
| Anglo-Danish | ring pins, seax-belt mounts, simple crosses, oath tokens |
| Norse | oval brooch pairs, bead strings, Thor/saint-adjacent amulet props where setting-appropriate, trade pendants |
| Norman / Capetian / High British | cloak clasps, pilgrim badges, reliquary pendants, belt mounts, signet rings |
| Gaelic / Welsh | ring pins, shrine cloth pins, bardic brooches, pastoral amulet cords |
| Byzantine | enamel pendants, icon pendants, court belt plaques, pectoral crosses |
| Andalusi / Fatimid / Abbasid / Seljuk | amulet cords, inscribed pendants, signet rings, tiraz/belt fittings, prayer beads |
| Rus / Novgorod | Orthodox pendant cords, icon pendants, fur-trade seal tags, belt mounts |
| Steppe Turkic | horseman amulets, belt plaques, bowcase tags, fur-cap ornaments |
| Song China | official badge props, scholar pendants, jade-like ornaments where material exists, seal-chop cords |

## Craft Inputs

Use appropriate stock:

| Product Type | Suggested Inputs |
| --- | --- |
| bronze/silver/gold jewellery | metal `Tool Blank Stock`, anvil, hammer, pliers/small tools where available |
| enamelled items | metal stock plus glass/pigment/luxury stock where available |
| bead strands | bead stock, cord/yarn stock |
| pendants | metal/wood/bone stock, cord stock |
| reliquary lockets/boxes | metal stock plus container component, optional sealable behaviour |
| signets/stamps | `SealStamp` component and metal stock |
| belt mounts | metal stock plus beltable/wear components |

## Test Requirements

Add tests that verify:

- Generic `regional devotional token` references are excluded from explicit culture item counts.
- Each culture has at least one explicit worn jewellery/devotional item when the household/devo target is implemented.
- Items using seal authority use `SealStamp` where appropriate.
- Exact stable references appear in `Medieval_Culture_Catalogue.md`.
