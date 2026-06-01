# Medieval Equipment and Military Crafting Suite

The medieval equipment suite should provide culturally distinct military loadouts rather than one representative armour, weapon, and shield per culture.

The current scaffold creates broad military packages and a standard accessory set. The second pass must add explicit military catalogues that can equip multiple roles within each culture.

## Design Principle

Medieval military culture is not a single weapon. Each culture should have enough equipment to support guards, levy infantry, elite retainers, cavalry, archers, crossbowmen, town militias, frontier fighters, ship crews, steppe riders, and palace troops where appropriate.

## Minimum Culture Military Surface

Each culture should receive at least eight explicit military/equipment items, excluding the existing generic armour, weapon, shield, padded coif, sidearm harness, quiver, field pack, and war banner.

Each culture should include a mix of:

| Slot | Examples |
| --- | --- |
| Primary weapon | sword, axe, spear, mace, sabre, dao, lance |
| Sidearm | seax, knife, short sword, dagger, baton |
| Missile weapon or ammo | bow, crossbow, arrows, bolts, composite bow, javelin bundle |
| Shield | round shield, kite shield, heater shield, adarga, dhal, rattan shield |
| Helmet/head protection | nasal helm, conical helm, kettle hat, turban-helm, lamellar cap |
| Body armour or padded layer | mail shirt, mail hauberk, lamellar coat, padded aketon, quilted coat |
| Belt/harness/scabbard/quiver | weapon belt, bowcase, gorytos, quiver, sidearm harness |
| Campaign or guard accessory | banner, spurs, field pack, armour stand, guard belt |

## Culture Loadout Directions

Exact stable references are listed in `Medieval_Culture_Catalogue.md`.

| Culture Cluster | Military Direction |
| --- | --- |
| Anglo-Saxon / Anglo-Danish | seaxes, long seaxes, spears, axes, bossed round shields, mail, nasal/spangenhelm-style helmets |
| Norse | bearded axes, broad axes, spears, round shields, mail, conical helmets, hunting bows, gorytoi/quivers |
| Norman / British / Capetian | arming swords, lances, spears, kite/heater shields, mail hauberks, aketons, crossbows |
| Gaelic / Welsh | long spears, javelins, small hide targes, light padded coats, hunting bows, leather bracers |
| Carolingian / Frankish | spathae, spears, large round shields, mail shirts, palace guard gear |
| German / HRE | war hammers, arming swords, town spears, heater shields, militia crossbows, plate-reinforced fittings |
| Iberian / Andalusi / Fatimid / Seljuk | adargas/dhals, saifs, spears, maces, composite bows, lamellar or padded coats |
| Byzantine | parameria, spears, oval shields, lamellar corselets, mail coifs, military belts |
| Rus / Steppe | axes, sabres, spears, composite bows, lamellar coats, fur-trimmed helmets, bowcases |
| Song China | dao, qiang spears, crossbows, rattan shields, lamellar vests, padded military robes |

## Craft Inputs

Use production-chain stock deliberately:

| Equipment | Suggested Inputs |
| --- | --- |
| swords, seaxes, dao, sabres | `Weapon Blade Stock`, grip/shaft stock, `Leather Strap` |
| axes, spears, maces, hammers | `Weapon Head Stock`, `Weapon Shaft Stock`, `Leather Strap` |
| bows | `Weapon Shaft Stock` or bow-specific stave stock, `Military Cord Stock` |
| crossbows | `Crossbow Tiller Stock`, `Crossbow Prod Stock`, `Crossbow Lockwork Stock`, `Military Cord Stock` |
| shields | `Shield Board Stock`, `Shield Facing Stock`, metal boss/fittings |
| mail | `Mail Panel Stock`, `Armour Ring Stock`, `Quilted Armour Padding` |
| lamellar | `Armour Lamella Stock`, `Military Cord Stock`, `Quilted Armour Padding` |
| padded armour | `Quilted Armour Padding`, `Garment Cloth`, `Spun Yarn` |
| helmets | metal `Tool Blank Stock`, leather straps/padding |
| quivers, gorytoi, scabbards, belts | `Scabbard Leather Stock`, `Prepared Leather Panel`, `Leather Strap` |

## Craft Naming

Use explicit final craft names:

```text
forge an Anglo-Danish long seax
assemble a Norman kite shield
rivet a Rus mail shirt
build a Song military crossbow
sew a Seljuk bowcase-and-quiver belt
```

Do not use generic `forge military weapon regional pattern NN` for explicit culture equipment.

## Test Requirements

Add tests that verify:

- Each culture has at least 8 explicit military/equipment references beyond the generic scaffold.
- Crossbow products consume crossbow-specific stock.
- Mail products consume mail stock.
- Lamellar products consume lamella stock.
- Final craft names do not use `regional pattern` for explicit items.
- Exact stable references appear in `Medieval_Culture_Catalogue.md`.
