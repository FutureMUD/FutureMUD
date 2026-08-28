# Stock Terrain Catalogue

## Purpose

`CoreDataSeeder.Terrain.cs` is the source of truth for the terrain catalogue installed by the core database seeder. The catalogue is intentionally broader than a single historical or geographic setting: it supplies indoor and urban spaces, roads, global terrestrial biomes, water, room-scale vehicle interiors, extraterrestrial environments, and supernatural realms.

The seeder is additive on rerun. It adds terrain names that are absent and repairs stock-owned atmosphere, forage-profile, and gravity links where safe, but it does not overwrite an existing terrain with the same name because builders may have customised that row.

## Presentation contract

Every stock terrain has three independent map values:

- `TerrainANSIColour` is an xterm colour index from the standard 16-colour subset (`0`–`15`). Dark/bright green distinguishes wooded and open fertile land; yellow and olive distinguish arid or rocky land; blue and cyan distinguish deep and shallow water; red distinguishes volcanic and infernal terrain; grey distinguishes vacuum and lunar terrain; bold white is the default for urban interiors and built spaces.
- `TerrainEditorColour` is a six-digit RGB colour. Related terrain families can share a thematic hue because the editor also renders the short code.
- `TerrainEditorText` is a unique one- or two-character code. Explicit codes are used for dense indoor families; the seeder deterministically allocates unused codes for the rest of the catalogue.

The ANSI value is numeric because the map renderer uses xterm `38;5;<index>` colour sequences. Its thematic palette corresponds to the colours builders know from `SubstituteANSIColour`: for example, dark green is index `2`, bright green is `10`, dark blue is `4`, bright blue is `12`, yellow is `11`, dark red is `1`, bright red is `9`, dark grey is `8`, and bold white is `15`.

## Mechanical scale

`MovementRate` contributes to movement delay, so a larger number is slower. Stock values follow this scale:

| Family | Typical movement rate | Typical stamina | Intent |
| --- | ---: | ---: | --- |
| Indoor, vehicle, and spaceship rooms | `0.5`–`0.75` | `2`–`5` | Compact authored spaces with short travel distances |
| Urban streets and public spaces | `0.75`–`1.5` | `7`–`10` | Fast city-scale movement |
| Roads and trails | `1.0`–`1.75` | `10` | Efficient rural travel |
| Open wilderness | `2.0`–`3.5` | `12`–`20` | Larger implied distance and navigation effort |
| Forest, mountain, wetland, desert, and shore | `3.0`–`5.0` | `18`–`30` | Difficult footing, cover, or large wilderness scale |

Open ground has a higher hide difficulty and a low spot floor. Dense forest, wetland, cave, shadow, and cluttered cargo terrain makes hiding easier and raises the spot floor. Stock terrain definitions use these values as minimum environmental difficulties rather than replacing the rest of the check system.

Track defaults are also part of the stock definition. Natural, rural, littoral, planetary, and selected supernatural ground can hold tracks. Sand, snow, and wet ground strengthen visual tracks; forests and wetlands strengthen olfactory tracks. Hard urban interiors, vehicles, open water, vacuum, astral voids, celestial realms, and dreamscapes do not receive ordinary tracks by default.

## Behaviour models

Terrain behaviour strings must use the models documented in the [Room Layer System Primer](../World/Room_Layer_System_Primer.md). In particular:

- ordinary enclosed vehicle compartments use `indoors`;
- `Rooftop` uses `rooftopsonly`, because the cell represents the elevated roof surface rather than the street below;
- gas-giant atmosphere uses `cliff`, providing air layers without a ground surface;
- water and cave-water terrain names bind their appropriate stock liquid;
- open space and astral void terrain use zero gravity, while lunar, asteroid, and planetary surfaces retain normal gravity until a low-gravity model exists.

## Catalogue coverage

The stock families include:

- **Built and vehicle:** domestic, commercial, administrative, industrial, street, generic vehicle interior/passenger/cargo rooms, ship corridor/cabin/cargo/engine rooms, spaceship corridor/cabin/bridge/engineering/cargo/airlock rooms, artificial habitats, and a zero-g spaceship compartment.
- **Global terrestrial:** grassland, savannah, steppe, prairie, tundra, Sahel, thorn scrub, wadi, dry riverbed, montane grassland, alpine meadow, karst, rice paddy, terraced field, tropical dry forest, monsoon forest, cloud forest, bamboo forest, temperate and tropical forests, wetland, desert, volcanic, glacial, cave, cenote, coast, river, lake, and ocean terrain.
- **Extraterrestrial:** lunar surface forms, asteroid surface, orbital through intergalactic space, airless and habitable planet surfaces, alien forest, frozen, carbon-dioxide, methane, and volcanic planet surfaces, and gas-giant atmosphere.
- **Supernatural:** astral expanse and void, fae glade and wilds, shadow realm and labyrinth, heavenly realm and celestial palace, hellscape and infernal fortress, and dreamscape.

The stock catalogue aims for reusable environmental building blocks rather than exhaustive local landform vocabulary. Builders should add setting-specific terrains when a world needs a distinct mechanical profile, map identity, atmosphere, or tag contract; purely synonymous prose normally belongs in overlays and room descriptions.

## Tags and forage

Terrain tags are hierarchical. The seeder persists only the most specific explicitly selected tags and relies on ancestry for broader matches, avoiding rows such as both `Urban` and its `Residential` child. Functional sibling tags such as `Diggable Soil`, `Foragable Clay`, `Foragable Sand`, `Wetland`, `Arid`, `Glacial`, `Volcanic`, and `Vacuum` remain alongside the terrain-family tag where applicable.

Natural additions receive stock forage profiles when their ecology is represented by existing forage yields. Artificial, vacuum, toxic-atmosphere, barren extraterrestrial, and most supernatural terrain intentionally remain without a stock forage profile.

## Verification

`CoreDataSeederTerrainTests` protects the catalogue contract by checking:

- additive reruns and unique names;
- atmosphere and gravity assignments;
- valid ANSI and RGB colours plus unique editor codes;
- representative movement, concealment, and track gradients;
- rooftop, water, cave, and air-only behaviour modes;
- vehicle, global-biome, extraterrestrial, and supernatural coverage;
- absence of redundant direct ancestor tags;
- stock forage-profile coverage and representative yields.
