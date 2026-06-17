# FutureMUD Agriculture System Overview

## Purpose
Agriculture is a first-class subsystem for fields, crops, pastures, herds, and managed woodlands. It deliberately sits beside crafting, projects, weather, property, terrain, and NPC systems rather than replacing any of them.

The core abstraction is `IAgricultureField`: zero or one field can exist in a cell, and multiple fields are represented by multiple cells. A field holds visible 0-100 scores, an active use, optional crop, perennial crop, herd, or woodland state, and optional apiary adjunct state.

## Design Goals
- Give farmers meaningful choices without requiring soil-science simulation.
- Use projects for labour, tools, materials, and group work.
- Use weather and terrain as coarse inputs, not as a second climate engine.
- Let builders seed useful defaults, then tune the local agricultural fiction.
- Support arable fields, gardens, orchards, pasture, and managed woodland/coppice in the same field model.

## Main Concepts
| Concept | Runtime role |
| --- | --- |
| Field profile | Default score package and allowed uses for new fields. |
| Field | Persisted cell-bound state, one per cell at most. |
| Crop definition | Builder-editable crop parameters for annual crops and perennial orchard/vineyard crops, including planting windows, seed requirements, and commodity outputs. |
| Apiary state | Optional hive installation on a field, independent of the primary field use, producing honeycomb, honey, beeswax, and pollination support. |
| Herd definition | Abstract livestock definition with animal-unit pressure, secondary commodity outputs, and optional NPC template for drawdown. |
| Woodland definition | Managed tree stand or coppice definition with establishment timings, harvest cycle timings, and commodity outputs. |
| Operation | Project-backed field operation such as sowing, drainage, weeding, harvesting, grazing, coppicing, felling, or clearing land. Woodland operations can release and consume yield. |

## Supported Field Uses
`AgricultureFieldUse` currently supports:

- `Fallow`: improvement, recovery, and preparation.
- `Crop`: annual crops and garden crops that are sown, harvested, and cleared back to fallow.
- `Orchard`: perennial crops such as orchards, vineyards, nut groves, and plantation crops that establish over multiple cycles and remain in place after harvest.
- `Pasture`: abstract herds and grazing pressure.
- `Woodland`: coppice, pollard, timber stands, managed groves, and land clearing.

A field has one primary use at a time. Temporary transitions, such as grazing fallow land or stubble, are modelled through operations that change the field's use.

Apiaries are not a primary field use. They are nullable field state so hives can coexist with fallow ground, crops, orchards, pasture, or managed woodland. Apiary operations use `AllowedUses` to run across those primary uses without making bees compete with the land use they support.

## Player Surface
The player-facing command is `field`.

Key workflows:

- `field` or `field look` inspects the field in the current cell.
- `field start <operation> [target]` starts the project backing an agriculture operation.
- `field harvest` finds an applicable harvest operation for the current crop.
- `field herd draw <herd> [count]` materialises live NPC livestock from an abstract herd when the herd definition has a template.
- `field herd absorb <npc> <herd>` removes an eligible NPC from the live world and adds it to the abstract herd.
- `field herd drive <herd> <direction> [count]` moves abstract animals into an adjacent fallow or pasture field when the destination is unowned or the actor is authorised there.

## Builder Surface
Administrators can use `field` to manage cell fields and agriculture definitions:

- `field create [profile]`, `field delete`, `field set <score> <0-100>`, `field reset`, and `field tick`.
- `field profile list|show|create|set|delete`.
- `field crop list|show|create|set|delete`.
- `field herds list|show|create|set|delete`.
- `field woodland list|show|create|set|delete`.
- `field operation list|show|create|set|delete`.
- `field scoretype list|set|disable` configures optional custom field score slots for non-standard settings.
- `terrain set agriculture <profile|none>` sets the default agriculture profile for future field creation on that terrain.

Terrain defaults do not create fields automatically. They are a builder convenience for `field create` and FutureProg field creation.

## Integrations
- Projects supply the labour/material structure and long-running work cadence.
- Agriculture project completion records Farming-weighted worker contribution, including skilled supervision, and uses it to adjust field effects, crop yield, seed recovery, and commodity quality.
- Weather supplies coarse daily moisture, stress, and growth pressure.
- Crop planting windows use the local weather season when one exists, with static celestial-year group windows as a fallback for games without regional climate setup.
- Commodity piles carry harvested crop, seed stock, secondary herd products, and woodland products into the item economy.
- Apiaries can release raw honeycomb, pressed honey, and rendered beeswax commodities, and happy colonies can improve pollination-sensitive crop and orchard yield.
- Herd product operations can release milk, wool, eggs, and manure from established pasture herds, including horse-herd milk for kumis-facing pastoral play.
- Crafts can require and consume live crop or woodland yield through the `field` input type.
- Generic commodity crafts can convert tagged agricultural yield into seed-tagged commodity piles.
- Terrain supplies a default field profile.
- Property gates field work when the cell belongs to a property.
- FutureProg can inspect, create, delete, mutate, score, and start work on fields.
- NPC templates are used only when abstract herd stock is deliberately drawn down into live animals.

Custom agriculture score slots are reserved in the `AgricultureScoreType` enum as `Custom1` through `Custom12`. They are disabled by default and named per game through static configuration, which lets fantasy or science-fiction games model unusual growing pressures without schema changes or stock crop assumptions.

## Seed Content
`AgricultureSeeder` installs stock profiles, specific crop definitions, herd definitions, specific managed woodland definitions, operation definitions, and project templates. The stock project templates include general field labour and an optional Farming-based supervision role so skilled farmers can improve the final result without being the only source of labour. The stock package is still builder-editable, but it now covers many common crop, herd, woodland, and apiary products so new games can start from concrete commodities instead of broad placeholder categories.

The stock profile set includes both ready-use cultivated land and rougher land-expansion starting points such as old fallows, scrub, wetland margins, heavy clay flats, eroded slopes, salt marsh edges, woodland clearings, and an `Apiary Yard` profile for dedicated beeyards. It also includes a deliberate cultivated-field quality ladder from exhausted cropland through model farms and paradise gardens, plus international profiles for chinampas, Andean raised fields, Sahel dryland fields, monsoon rice terraces, flood-recession fields, mangrove rice polders, desert wadis, laterite gardens, cold boreal clearings, and other marginal or specialist land forms.

The crop catalogue spans common temperate staples, tropical and wetland crops, dryland cereals and pulses, archaic crops, Americas and African staples, medieval-to-early-industrial vegetables and fodder crops, orchard fruit, nuts, vines, berry beds, perennial vegetable beds, and plantation spices or fibres, with concrete pollination metadata on crops that benefit from or require bee support. Stock herds are species-specific rather than mixed placeholders, covering cattle, oxen, buffalo, bison, sheep, goats, pigs, horses, donkeys, mules, camels, llamas, alpacas, reindeer, rabbits, guinea pigs, common poultry, game birds, peafowl, and ratites. Stock crops and woodlands include coriander, cumin, saffron crocus, black pepper, indigo, pomegranate, walnut, madder, weld, alkanet, henna, kermes, orchil, lac, gum, resin, thatch, fibre, charcoal, and useful parkland or plantation paths used by processing crafts.
