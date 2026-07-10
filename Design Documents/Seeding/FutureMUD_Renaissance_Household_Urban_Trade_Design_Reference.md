# FutureMUD Renaissance Household, Urban, and Trade Design Reference

## Scope

This branch owns Renaissance household furniture, tableware, urban, tavern, guild/merchant, dockside, customs, warehouse, and culture-specific trade goods not supplied by shared aliases.

## Planned slices

- cassoni and distinct chests, cupboards, sideboards, wardrobes, writing desks, counting tables, bookcases, display shelves, carved storage, and guild/merchant furniture;
- majolica/faience/Iznik-adjacent wares, early export porcelain, glassware, mirrors, apothecary jars, scent containers, table carpets, hangings, pictures, maps, devotional panels, lighting, and display goods;
- tavern tankards, mugs, jugs, trays, counters, benches, gaming/tally goods, and supported till/cash systems;
- shipboard, dockside, customs, warehouse, and export goods whose capacity, component, installation, or institutional role differs from shared packages.

## Reuse and dependency boundary

Reuse the 147 non-regional `preindustrial_trade_*` aliases and named tea, coffee, cacao, tobacco, sugar, spice, indigo, porcelain, bottle, silk, and cotton packaging. Automatic installation does not make those packages normal in every Renaissance place or date.

Oak, walnut, leather, linen, wool, paper, brass, copper, pewter, glass, porcelain, faience, earthenware, and stoneware are live materials. Exact dependencies and gaps are maintained in `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`.

## Acceptance criteria

- A new prototype differs in form, component, capacity, installation, institution, or production technology.
- Fixed furniture omits `Holdable`; portable wares include it.
- Culture/date admission prevents backdating global commodity consumption from packaging alone.
