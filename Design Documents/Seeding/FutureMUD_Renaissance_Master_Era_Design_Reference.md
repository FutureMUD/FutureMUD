# FutureMUD Renaissance Item Seeder Master Era Design Reference

> Post-shared-baseline edition. The common pre-industrial item layer is implemented; this document now defines the Renaissance-specific delta and historical-admission policy.

## Executive summary

This reference covers `Renaissance`, the Renaissance period roughly 1400-1600 CE. It remains a standalone era reference rather than a combined Renaissance/EarlyModern document.

The shared pre-industrial baseline is now live and is automatically installed when the Renaissance era is selected. The Renaissance-specific entrypoint, `SeedRenaissanceItems()`, exists but remains the place for future era-specific catalogue work. Consequently, this reference should not re-author ordinary cross-era tools, writing goods, trade containers, military supports, time/water fixtures, or the dedicated shared print/navigation/gunpowder-support/trade-packaging stock already supplied by the baseline.

The Renaissance-specific seeder should emphasize:

- Late medieval survivals only where no shared alias already covers the durable form.
- Fitted court and urban clothing in western and central Europe, plus distinct contemporary clothing systems elsewhere.
- Classical-revival, humanist, artistic, civic, and court material culture.
- Culture-specific print production, printed documents, maps, music, engraving, and book-trade goods beyond the shared print baseline.
- Transitional armour, pike-and-shot warfare, actual matchlock/wheellock firearms, and ammunition mechanics beyond the shared support objects.
- Expanding maritime trade, first-contact goods, and early colonial/contact-zone material culture, without treating shared packaging as universally prevalent.
- Strong coverage of Ottoman, Safavid, Mughal/Timurid/Deccan, Ming, Joseon, Japanese, South-east Asian, African, and American contemporaries rather than treating the period as Europe-only.

## Scope and era model

- Era token: `Renaissance`.
- Era-selection key: `renaissance`.
- Approximate chronological band: 1400-1600 CE.
- Era-specific rows should normally use `Era / Renaissance Era`; shared dependencies retain `Era / Pre-Industrial Era`.
- Chronology should be treated as a family of anchored contexts, not a single universal year.
- The Renaissance document may include late medieval survivals and early-modern precursors, but should not normalize fully eighteenth-century forms.
- The presence of a shared baseline item is catalogue availability, not proof of common use in every Renaissance culture.
- Builder-facing labels may use real-world historical inspiration names. Public item fields should remain form-based and in-world.

## Relationship to the EarlyModern reference

The Renaissance and EarlyModern references remain separate documents. They share the implemented pre-industrial baseline, but each era owns its culture table, historical-admission rules, item-domain delta, exclusions, and branch implementation.

The live dispatcher sequence is conceptually:

```text
SeedSharedPreIndustrialBaselineItems(); // automatic for antiquity, medieval, renaissance, earlymodern
SeedRenaissanceItems();                 // Renaissance-specific rows
```

The Renaissance seeder does not depend on `SeedEarlyModernItems()`. EarlyModern may later reuse particular Renaissance-specific stable references where the form remains credible after 1600, but it should do so explicitly and idempotently rather than invoking the complete Renaissance branch.

A broad `EnsureRenaissanceSurvivalsWhereStillCredible()` helper is not currently required. The shared `preindustrial_*` layer is the default cross-era dependency; any additional Renaissance survival should be reviewed item by item once the Renaissance catalogue exists.


## Implemented shared pre-industrial baseline

The shared pre-industrial baseline is now implemented and is part of the runtime seeder contract.

`SeedReworkItems()` invokes `SeedSharedPreIndustrialBaselineItems()` whenever the selected era string contains `antiquity`, `medieval`, `renaissance`, or `earlymodern`. The shared entrypoint seeds the established `historic_*` workshop foundation and `primary_production_*` tools before the compatibility aliases and newly authored `preindustrial_*` stock. Existing source-era stable references are retained; later eras should depend on the shared aliases rather than renaming or deleting the source rows.

### Implemented catalogue

| Implemented family | Current coverage |
|---|---:|
| `preindustrial_writing_*` writing surfaces, books, ledgers, and implements | 39 aliases |
| `preindustrial_trade_*` non-regional trade containers and lockboxes | 147 aliases |
| `preindustrial_door_*` and general `preindustrial_clothing_*` accessories | 17 aliases |
| `preindustrial_tool_*` and `preindustrial_workshop_*` fixtures and tools | 77 aliases |
| `preindustrial_military_support_*` carrying, storage, belt, rack, and display goods | 52 aliases |
| `preindustrial_time_*` and `preindustrial_water_*` civic/timekeeping forms | 10 aliases |
| Printing and paper-administration named stock | 11 targets |
| Navigation, surveying, optics, and scientific named stock | 12 targets |
| Gunpowder-support named stock | 10 targets |
| Global-trade packaging named stock | 11 targets |

The alias layer contains **342 compatibility aliases**. The named shared-stock list contains **44 targets**: 43 dedicated new rows plus `preindustrial_trade_spice_chest`, which is supplied by the compatibility alias of `medieval_locking_trade_spice_chest`.

### Implemented stock boundaries

- Shared aliases copy the source form, material, size, quality, weight, cost, components, and functional/market tags; source-era tags are replaced by `Era / Pre-Industrial Era`.
- Alias lifecycle links are rewritten to shared aliases rather than falling back to medieval-only targets.
- The dedicated printing, navigation, optical, scientific, gunpowder-support, and global-trade rows use existing components only. Unsupported specialist behaviour remains descriptive.
- The gunpowder-support group contains powder vessels, pouches, match cord, wadding, a bullet mould, ramrod, touch-hole pick, and cleaning rod. It deliberately contains no firearm, ammunition, bomb, or explosive component.
- Fixed dedicated fixtures omit `Holdable`. Portable dedicated rows include it. Source-derived aliases preserve the source catalogue's portability convention.
- Dedicated new shared rows are currently non-skinnable; aliases preserve the source row's skin setting. Any change to that policy should be made centrally rather than by cloning cosmetic era variants.
- The baseline deliberately does not promote castle-specific gates, medieval underclothing, medieval combat weapons or armour, regional `medieval_regional_*` containers, or culture-specific colonial/contact-zone packages.

### Historical-admission rule

Automatic seeding means that a shared prototype is available to builders and later content; it does **not** assert that every supported era, culture, settlement, shop, or profession commonly uses it. Era and culture manifests, craft availability, shop stock, and local world-building must control actual prevalence. This is especially important for telescopes, movable-type printing equipment, gunpowder support, and tea/coffee/cacao/tobacco/sugar packaging.

The implementation source of truth is:

- `Design Documents/Seeding/PreIndustrial_Item_Seeder_Design_Reference.md`
- `Design Documents/Seeding/PreIndustrial_Item_Seeder_Alias_Catalogue.md`

## Seeder and project grounding rules

- Use the project-standard `CreateItem(...)` call shape for implementation.
- Use exact seeded solid materials for `material`.
- Use exact seeded components and exact seeded hierarchical tags.
- Use the implemented `preindustrial_*` stable reference wherever a shared alias or named shared-stock row already exists.
- Do not create a Renaissance duplicate merely to obtain a Renaissance prefix, different builder note, or cosmetic presentation.
- Ordinary portable goods include `Holdable`; fixed room features, installed architecture, terrain props, and permanent infrastructure omit it.
- Liquids and gases are not substitutes for the solid primary material argument.
- Do not claim functionality without a matching component.
- Skins vary presentation, not component behaviour. Where a dedicated shared row is currently non-skinnable, resolve any desired skin policy centrally rather than cloning the item in this branch.
- Public text avoids seeder mechanics, builder-facing labels, and unsupported behaviour claims.
- Era-specific crafts, outfits, shops, and manifests must gate historically restricted shared stock rather than assuming every automatically seeded row is common.

## Reuse from Antiquity and Medieval seeders

### Default cross-era dependency rule

Use the shared stable references rather than source-era references whenever an alias exists:

- `preindustrial_writing_*` for shared manuscript, paper, ledger, and writing-implement forms.
- `preindustrial_trade_*` for non-regional trade containers, lockboxes, and shared commodity packaging.
- `preindustrial_door_*` for the promoted ordinary door and gate forms.
- `preindustrial_clothing_*` for the three promoted general accessories.
- `preindustrial_tool_*` and `preindustrial_workshop_*` for promoted workshop tools and fixtures.
- `preindustrial_military_support_*` for sheaths, scabbards, quivers, belts, racks, and armour-display supports.
- `preindustrial_time_*` and `preindustrial_water_*` for the promoted timekeeping and civic-water forms.

The `historic_*` workshop foundation and `primary_production_*` package are already seeded by the shared entrypoint and should not be re-invoked inside `SeedRenaissanceItems()`.

### Direct earlier-era reuse outside the baseline

Direct use of an `antiquity_*` or `medieval_*` source row should now be exceptional. It is appropriate only when:

- no shared alias exists;
- the form remains credible in the intended Renaissance context;
- the row is not one of the baseline's explicit exclusions; and
- promoting it to the shared baseline would be broader than the actual use case.

If the object would serve both Renaissance and EarlyModern broadly, extend the shared baseline instead of creating two era copies.

### Reuse mainly through skins

- Antiquity clothing should remain era-bound, but classical-revival motifs can inform Renaissance pageantry, theatre, court display, scholarly costume, decorative armour, and ceremonial art skins.
- Medieval heraldry, manuscript decoration, guild marks, devotional marks, local textile motifs, and household symbols should generally remain skins unless the object form or component changes.

### Deliberately not supplied by the shared baseline

The Renaissance branch still owns, where appropriate:

- fitted Renaissance clothing and underlayers beyond the three generic shared accessories;
- actual firearms, firearm ammunition, and pike-and-shot combat equipment;
- transitional plate armour and Renaissance helmet/shield forms;
- culture-specific religious, court, artistic, scientific, household, and contact-zone objects;
- ordinary household furniture not present in the shared alias catalogue;
- regional containers excluded from the non-regional trade alias rule.

## Shared-first branch architecture

### Default rule

Create shared Renaissance prototypes wherever the item can plausibly serve multiple cultures with only presentation changes. Use culture-specific prototypes when form, material, production technology, component use, institution, or gameplay role changes.

### When to make a distinct culture-family prototype

Create a separate prototype when one or more of the following is true:

- The silhouette is visibly distinct enough that a skin would be misleading.
- The component differs.
- The primary material changes behaviour or craft use.
- The object belongs to a distinct institution or production system.
- The object is a strong builder-facing regional anchor.
- The object reflects contact-zone or trade/export status through form, packaging, or inscription.

### What skins should carry

Skins should carry:

- local names
- exact textile motifs
- heraldry
- guild marks
- house or clan marks
- maker marks
- dynastic or court marks
- inscriptions and script
- seal impressions
- painted or carved decoration
- glaze and lacquer patterns
- devotional symbols
- imported/export presentation
- status variation

## Culture coverage table

| Inspiration family | Reference anchor | Coverage boundary | Renaissance material-culture focus | Shared grouping |
|---|---:|---|---|---|
| Italian Renaissance city-states | c. 1450-1550 | Florence, Venice, Milan, Genoa, Rome-facing urban/court worlds | fitted garments, civic luxury, humanist books, print, banking, art tools, armour, glass, maritime trade | Shared Western European Renaissance |
| Iberian Atlantic | c. 1450-1600 | Castile, Aragon, Portugal, early Atlantic empire | maritime equipment, Catholic furnishings, court clothing, colonial/contact goods, firearms, maps, trade chests | Shared Iberian / Atlantic |
| French / Burgundian / Low Countries | c. 1450-1600 | Burgundy, France, Flanders, Low Countries | court textiles, urban guild goods, print, armour, tapestries, luxury household goods | Shared Franco-Flemish / Western European |
| Tudor / Elizabethan British | c. 1500-1600 | England, Wales, Scotland, Ireland-facing urban/court contexts | doublets, gowns, ruffs, merchant goods, legal paper, maritime goods, household display | Shared British Isles Renaissance |
| German / HRE / Swiss | c. 1450-1600 | imperial towns, Swiss cantons, Reformation centres | print shops, pike/halberd/pike-and-shot equipment, clocks, guild tools, armour | Shared Central European |
| Scandinavian / Baltic | c. 1450-1600 | Denmark, Sweden, Norway, Baltic trade towns | naval stores, timber, Lutheran furnishings, wool, trade containers, military equipment | Shared Northern / Baltic |
| Polish-Lithuanian / Hungarian | c. 1500-1600 | Central/eastern European noble, frontier, and town contexts | sabres via skins/components, cavalry gear, robes/coats, firearms contact, court textiles | Shared Central/Eastern frontier |
| Muscovite / Russian | c. 1500-1600 | Muscovy and northern/eastern trade networks | caftans, furs, icons, chancery goods, trade chests, firearms contact | Shared Eastern Orthodox / northern |
| Ottoman classical | c. 1450-1600 | Anatolia, Balkans, eastern Mediterranean, North African overlaps | kaftans, turbans, sashes, ceramics, mosque/madrasa goods, chancery, coffee precursor goods, cavalry/firearms | Shared Ottoman-Islamicate |
| Safavid / Persianate | c. 1500-1600 | Iran, Caucasus, Persianate courts and trade | carpets, manuscripts, metalwork, court robes, turbans, ceramics, cavalry equipment | Shared Persianate |
| Timurid / early Mughal / Deccan | c. 1450-1600 | Central/South Asian courtly and Indo-Persian worlds | cotton/silk textiles, turbans, robes, court weapons, manuscripts, jewellery, trade goods | Shared Indo-Persian |
| Vijayanagara / South Indian court and temple | c. 1450-1600 | South India, Deccan interactions, temple/court environments | draped and semi-tailored garments, brass vessels, temple goods, cotton and silk, jewellery, maritime trade | Shared South Asian |
| Ming China | c. 1400-1600 | Ming court, scholar, merchant, artisan, maritime/export contexts | porcelain, silk, lacquer, brush-and-paper goods, woodblock print, furniture, tea goods | Shared East Asian |
| Joseon Korea | c. 1400-1600 | Joseon court, scholar, commoner, military contexts | paper, books, ceramics, scholar goods, robes, hats, storage, bows/firearms contact | Shared East Asian literati |
| Muromachi / Sengoku / Momoyama Japan | c. 1450-1600 | late medieval through unification-era Japan | kosode, hakama, kataginu, armour, swords, lacquer, tea goods, castle/warrior and merchant goods | Shared Japanese |
| Ryukyu and maritime East Asia | c. 1450-1600 | Ryukyu and trade-facing islands | lacquer, textiles, ceramics, tribute/trade goods, storage, maritime containers | Shared maritime East Asian |
| Mainland South-east Asian courts | c. 1450-1600 | Ayutthaya, Burma, Khmer/Lan Xang, Vietnam | lacquer, bronze, textiles, temple goods, court objects, elephant/war gear where supported | Shared South-east Asian mainland |
| Maritime South-east Asian sultanates | c. 1450-1600 | Malay, Indonesian, Philippine contact-zone contexts | spice packaging, Islamic coastal goods, boats, textiles, ceramics, trade chests | Shared maritime South-east Asian |
| Steppe / Inner Asian | c. 1450-1600 | Tatar, Uzbek, Kazakh, Mongol, Manchu-frontier contexts | felt, leather, horse gear, bow/cavalry legacy, firearms contact, caravan storage | Shared steppe and caravan |
| West African court and forest kingdoms | c. 1450-1600 | Benin, Yoruba, Akan/Gold Coast early contexts | brass, ivory, beads, regalia, textiles, court display, Atlantic contact goods | Shared West African court |
| Sahelian Islamic West Africa | c. 1450-1600 | Songhai, Hausa, trans-Saharan networks | leather, manuscripts, robes, metalwork, horse gear, caravan goods | Shared Sahelian Islamic |
| Ethiopian / Red Sea | c. 1450-1600 | Ethiopian highlands and Red Sea exchange | Christian liturgical goods, manuscripts, shields, textiles, trade goods | Shared Red Sea |
| Swahili Coast | c. 1450-1600 | East African Indian Ocean towns | Islamic urban goods, coral-stone domestic goods, beads, imported ceramics, maritime trade | Shared Indian Ocean |
| Mesoamerican | c. 1400-1600 | Mexica/Aztec, Maya/Postclassic, Mixtec and related contexts | cotton, featherwork, cacao, stone, codices, ritual vessels, first-contact goods | Shared Mesoamerican |
| Andean / Inka | c. 1400-1600 | Inka and early post-conquest Andean contexts | camelid textiles, quipu, ceramics, metalwork, featherwork, storage, colonial contact goods | Shared Andean |
| Caribbean / Taíno and Atlantic contact | c. 1450-1550 | Caribbean contact zones | cotton, wood, shell, baskets, hammocks, early colonial trade goods | Shared Caribbean contact |
| North American Indigenous contact | c. 1450-1600 | broad regional placeholder pending later deeper pass | hides, baskets, pipes, wampum-like goods, trade metal contact items | Shared North American contact |
| Early Iberian colonial Americas | c. 1500-1600 | Spanish and Portuguese colonial/contact zones | mission goods, trade chests, firearms, iron tools, sugar/cacao/tobacco/cotton packaging | Shared colonial Atlantic |
| Global maritime trade | c. 1450-1600 | cross-cultural overlay | shipboard goods, dockside storage, export crates, charts, compasses, commodity sacks, customs boxes | Shared global maritime |

## Shared cross-cultural groupings

Use these as builder-facing grouping overlays across the above cultures:

| Shared grouping | Use case |
|---|---|
| Shared Western European Renaissance | common fitted clothing, urban furniture, print, legal/account documents, art tools, armour, trade goods |
| Shared Reformation / Catholic institutional | church, chapel, reliquary, lectern, altar, pamphlet, iconoclasm/Counter-Reformation-adjacent goods where form changes |
| Shared Ottoman-Islamicate | kaftans, sashes, turbans, low furniture, metal lamps, ceramics, chancery, mosque/madrasa goods |
| Shared Persianate / Indo-Persian | court robes, turbans, carpets, manuscripts, metalwork, jewellery, cavalry goods |
| Shared East Asian literati | brush, inkstone, paper, scrolls, stitched books, scholar desks, storage boxes, ceramics |
| Shared Japanese | kosode/hakama variants, lacquer, tea goods, warrior equipment, compact storage |
| Shared South Asian | cotton/silk textiles, brass vessels, temple/scholastic goods, manuscript/storage, jewellery |
| Shared maritime trade | chests, casks, crates, sacks, bales, port documents, charts, compasses, trade samples |
| Shared first-contact / colonial | imported iron tools, beads, glass, firearms, mission goods, colonial papers, hybrid containers |
| Shared African court / Atlantic | brass, ivory, beads, court regalia, textiles, trade goods |
| Shared steppe and caravan | felt, leather, horse gear, compact storage, bows, trade pouches, courier cases |

## Item-domain guidance

### Clothing and accessories

The live baseline contributes only three general clothing accessories: `preindustrial_clothing_plain_leather_belt`, `preindustrial_clothing_iron_buckled_leather_belt`, and `preindustrial_clothing_simple_woven_sash`. Do not treat this as a wardrobe. Renaissance underlayers, silhouettes, footwear, headwear, court dress, workwear, and regional clothing remain era-specific work.

Renaissance clothing should be split into domain references rather than inflated in this master document. Target coverage:

- Western and central European fitted clothing: shirts, chemises, smocks, doublets, jerkins, bodices, gowns, kirtles, hose, trunk hose, breeches, codpiece-adjacent variants if approved, farthingales, ruffs, partlets, cloaks, capes, flat caps, tall hats, coifs, veils, gloves, masks, and fans.
- Ottoman, Safavid, Indo-Persian, and South Asian robes, kaftans, jamas, turbans, sashes, slippers, sandals, shawls, and courtly textile layers.
- Ming, Joseon, Japanese, Ryukyuan, and South-east Asian forms where silhouette differs: robes, jackets, skirts, trousers, hakama, kosode, kataginu, scholar caps, court headgear, tropical wraps, and lacquered or textile accessories.
- African court, Sahelian, Ethiopian, Swahili, Mesoamerican, Andean, Caribbean, and North American Indigenous garment families where distinct textile, hide, featherwork, beadwork, or draped/wrapped form justifies new prototypes.
- Urban/professional overlays: printer, scholar, merchant, notary, sailor, artisan, apothecary, artist, musician, actor/pageant, guard, court servant.

Avoid making every national style a new base item. Most colour, trim, local name, household mark, heraldry, religious sign, and textile motif variation belongs in skins.

### Military, firearms, armour, and support

Do not recreate the live shared gunpowder-support rows: `preindustrial_firearms_powder_horn`, `preindustrial_firearms_powder_flask`, `preindustrial_firearms_priming_flask`, `preindustrial_firearms_shot_pouch`, `preindustrial_firearms_match_cord_bundle`, `preindustrial_firearms_musket_wadding_packet`, `preindustrial_firearms_bullet_mould`, `preindustrial_firearms_ramrod`, `preindustrial_firearms_touchhole_pick`, and `preindustrial_firearms_cleaning_rod`. The baseline also supplies 52 `preindustrial_military_support_*` aliases for sheaths, scabbards, quivers, belts, racks, and armour displays.

Renaissance military goods should add the combat systems and period-specific forms served by that shared support layer:

- Actual matchlock arquebus, caliver, early musket, handgonne-like survival, and wheellock pistol families where components allow.
- Musket balls, loose shot, and historically suitable paper-cartridge or charge-packet forms once ammunition and consumable mechanics are settled.
- Bandoliers or apostle-style charge sets, gun rests, firearm slings, worms, vent tools not already covered, and specialist gunsmithing equipment.
- Pike, bill, halberd, partisan, glaive, spear, lance, rapier, sidesword, main-gauche, and complex-hilt sword precursors where component-safe.
- Transitional armour: full or partial plate harness, breastplate/backplate, faulds, tassets, gorgets, burgonets, morions, cabassets, armets, and close-helmet variants where component and period policy permit.
- Tournament, pageant, ceremonial, and display armour as skinnable finished goods, without implying superior mechanics merely from decoration.
- Bucklers, rotella-like round shields, target/targe variants, pavises, dhal/adarga continuities, and regional hide/wicker shields.

Boundary:

- Keep modern firearms, metallic cartridges, percussion caps, revolvers, rifles, machine guns, grenades, modern ballistic armour, and modern explosives out of this branch.
- Do not claim explosive, armour-piercing, attachment, loading, or firing behaviour without components.

### Writing, print, books, documents, and administration

The baseline already supplies 39 `preindustrial_writing_*` aliases and the minimum shared print/paper-administration set: hand press, type case, composing stick, chase, inking balls, drying rack, broadside, pamphlet, almanac, blank form, and printed map sheet. These rows are a minimum vocabulary, not a complete Renaissance print economy.

Use the shared references for ordinary parchment/paper sheets, letters, ledgers, codices, quills, reed pens, brushes, styluses, the common press equipment, and the generic printed forms. Add Renaissance-specific rows only where format, component, production technology, institution, or historical role differs:

- Warrants, decrees, port records, customs papers, notarial instruments, sketchbooks, commonplace books, pilot books, portolan charts, indulgence-like forms, printed prayer books, and printed music.
- Movable type sorts, punches/matrices or type moulds, furniture for locking a forme, press tympan/frisket assemblies, press furniture, ink tables, and other production goods not represented by the shared set.
- Woodblocks, copper plates, burins, etching needles, engraving tools, ink daubers, block-carving knives, and printed images.
- East Asian brush, ink, paper, stitched-book, scroll, and woodblock systems where their form or production workflow differs from the European press model.
- Seal matrices, sealing media, archive furniture, bookshop display, courier systems, and institution-specific record storage beyond existing writing/trade aliases.

Boundary:

- Newspapers, regular periodical journals, mature postal systems, stock certificates, modern envelopes, clipboards, staples, paperclips, fountain pens, and graphite-pencil default stationery belong mainly to EarlyModern or later.

### Household, urban, tavern, and trade goods

The shared layer already supplies 147 non-regional `preindustrial_trade_*` container/lockbox aliases, promoted ordinary doors, and named packaging for tea, coffee, cacao, tobacco, sugar, spices, indigo, porcelain, glass bottles, silk, and cotton. Use those stable references when the package form is the same. The shared layer does not constitute a complete household-furniture or tableware catalogue.

Renaissance-specific additions should include:

- Cassoni and other period-significant chests, cupboards, sideboards, wardrobes, writing desks, counting tables, bookcases, display shelves, carved storage, and elite merchant or guild furniture where an existing alias is not a sufficient form.
- Majolica/faience/Iznik-adjacent ceramic wares, early porcelain export wares, glassware, mirrors, apothecary jars, scent containers, table carpets, wall hangings, framed pictures, maps, devotional panels, candlesticks, chandeliers, lanterns, and sconces.
- Tavern and inn service: tankards, mugs, jugs, serving trays, counters, benches, gaming goods, tally boards, and till/cash systems where supported.
- Shipboard, dockside, customs, warehouse, and export goods that differ from the shared package by component, capacity, installation, or institution.

Tea chests, coffee sacks, cacao sacks, tobacco bales, and similar shared stock should be admitted only in cultures and dates where they are plausible; their automatic presence in the catalogue is not a default Renaissance household assumption.

### Art, craft, and workshop goods

The baseline already supplies the historic workshop foundation, the primary-production package, and 77 promoted workshop fixtures and tools. Check the alias catalogue before adding any forge, kiln, loom, papermaking, bookbinding, woodworking, coopering, armourer, jeweller, pottery, masonry, or glassworking tool. New rows should represent a genuinely missing form, tool component, production technology, or period-specific specialist use.

The Renaissance deserves a separate art/craft guidance branch.

Include:

- Easels, panels, canvases if material support exists, stretched frames, palettes, brushes, pigment shells, grinding slabs, mullers, drawing boards, chalk, charcoal, silverpoint props, compasses, proportional dividers, sculptor's tools, carving tools, wax models, plaster casts, moulds, pattern books.
- Goldsmith, jeweller, engraver, printer, bookbinder, glazier, clockmaker, armourer, gunsmith, sailmaker, ropemaker, and apothecary toolkits.

### Science, navigation, optical, and measurement goods

The shared catalogue already contains a magnetic compass, dividers, cross-staff, mariner's astrolabe, chart case, measuring chain, plane table, spectacles, magnifying lens, draw-tube telescope, balance scales, and specimen jar. Use those stable references rather than creating Renaissance-prefixed copies.

Renaissance-specific additions should concentrate on missing forms and supporting systems:

- Quadrants, rules, sector-like proportional instruments, measuring rods, separate weight sets, sandglasses, sundials, mechanical clocks, globes, armillary spheres, sounding leads, lead lines, pilot books, and logbooks.
- Instrument cases, stands, calibration/marking tools, lens-grinding tools, chart tables, and workshop stock where not already covered by shared tools.
- Culture-specific navigation or astronomical instruments where a shared European-looking form would be misleading.

Most shared instrument rows are descriptive or container-backed rather than full scientific mechanics. Historical admission remains mandatory. In particular, `preindustrial_optics_telescope` should normally be restricted to the very late edge around 1600; it is not a general fifteenth- or sixteenth-century object. Microscopes, barometers, thermometers, mature scientific cabinets, and Enlightenment instrument sets belong mainly to EarlyModern.

### Agriculture, food, drink, and trade commodities

The shared baseline supplies packaging—not crops, foods, drinks, or processing chains—for tea, coffee, cacao, tobacco, sugar, spices, indigo, porcelain, glass bottles, silk, and cotton. Use those `preindustrial_trade_*` references when the package form is appropriate.

Renaissance agriculture and commodity work should include:

- Old World staples already supported by earlier Agriculture seeders, with era-specific processing or market forms only where needed.
- Early global/colonial commodities: sugar, cacao, tobacco, maize, potato, sweet potato, cassava, cotton expansion, indigo, cochineal, logwood, spices, tea, coffee, and rice variants.
- Processing outputs: sugar loaves and molasses, cacao beans/nibs, tobacco leaf/twist, cotton fibre/bales, indigo cakes, cochineal packets, tea cakes/bricks, coffee beans, spice packets, and dye stock.
- Region- and date-specific crafts linking those outputs to the shared chests, sacks, bales, boxes, crates, and hogsheads.

The coffeehouse/tea-house/chocolate-house social-material complex belongs mainly to EarlyModern, except for region-specific earlier tea and coffee traditions. Packaging availability must not be used to backdate consumption culture.

### Religious, learned, and institutional goods

Include Renaissance-specific institutional material:

- Church and chapel goods, mosque/madrasa goods, synagogue goods, temple/shrine goods, devotional panels, reliquaries, lecterns, book stands, scripture boxes, alms chests, incense burners, offering vessels, rosary/prayer-bead-like objects where accessory support exists.
- Reformation and Counter-Reformation material should be handled as document/print/furnishing variants where object form changes: pamphlets, broadsheets, portable devotional books, altarpiece/panel forms, censored or iconoclastic damage states if later approved.

## Primary Industry Seeder impacts

### Already resolved by the shared baseline

- `SeedPrimaryProductionToolsAndProps()` is now automatically invoked for Renaissance selection.
- The historic workshop foundation and 77 promoted cross-period tools/fixtures are available without Renaissance duplicates.
- Shared props exist for print production, navigation/survey, optical/scientific work, gunpowder handling, and global-trade packaging.

These facts provide item vocabulary only. They do not create complete material-production chains, crafts, resource systems, or specialist mechanics.

The foundational gap patch now supplies exact type metal, printing ink, glass blank, taffeta, ribbon, calico/chintz, logwood, cochineal, tobacco, and processed cash-crop materials, plus historical specialist skill pairs for printing, gunsmithing, powdermaking, optics, clockmaking, instruments, navigation, cartography, surveying, and engraving. These foundations resolve names and commodity inputs; they do not replace the complete chains below.

### Still required or subject to audit

- Papermaking: rag bales, pulp, mould-and-deckle, press felts, paper reams, sizing, and paper-production crafts.
- Printing: movable type item stock, type moulds, formes/tympan/frisket where distinct, and functional printing crafts using the live type-metal and printing-ink materials.
- Gunpowder: saltpetre/nitre, brimstone/sulfur, charcoal grades, blackpowder commodity/craft stock, lead shot, and safe production chains.
- Firearms and armour: barrels, locks, springs, screws, stock blanks, actual weapon components, ammunition, and period-specific armour manufacture.
- Glass and optics: clear/lens/mirror glass stock, lens grinding and polishing, and any functional optical component work.
- Ceramics: porcelain, faience/majolica/stoneware/earthenware/glaze production and cobalt-pigment support.
- Textiles: finished stock and crafts using the live broadcloth, velvet, satin, taffeta, lace, ribbon, linen, cotton, silk, felt, canvas, calico, and chintz materials.
- Maritime stores: sailcloth, rope, tar, pitch, resin, oak staves, casks, blocks, tackle, and caulking.
- Dyes and pigments: processing crafts using the live indigo, cochineal, madder, woad, logwood, saffron, cinnabar, azurite, malachite, and other approved stocks.

## Useful Seeder impacts

### Already resolved

- `Pre-Industrial Era` and `Renaissance Era` tags are live in the maintained tag hierarchy.
- Shared print, navigation/science, gunpowder-support, global-trade, tool, writing, container, door, time/water, and military-support rows use maintained materials, tags, and components.
- Renaissance selection automatically receives the common primary-production and historic workshop packages.
- Historical specialist skills now resolve for movable-type printing, gunsmithing, powdermaking, lensmaking, clockmaking, instrument making, navigation, cartography, surveying, and engraving in either naming mode.

### Still required

Add or validate non-duplicative toolkits for:

- printer
- bookbinder
- papermaker
- scribe/notary/clerk
- cartographer/navigator
- artist/painter/engraver
- surveyor
- armourer
- gunsmith
- powder handler
- apothecary
- sailmaker
- ropemaker
- cooper
- glazier/lens grinder
- jeweller/goldsmith

Before creating a toolkit or tool, check for a `preindustrial_tool_*`, `preindustrial_workshop_*`, writing alias, or dedicated shared-stock row. Each kit should only claim component-backed behaviour. Descriptive kits may use ordinary containers plus `Holdable`; functional tool items need exact tool components.

## Agriculture Seeder impacts

The shared baseline supplies reusable packaging rather than crops or processing chains. The Agriculture seeder now supplies all named crop foundations below, including tobacco, cardamom, allspice, logwood, explicit medicinal crops, cacao-bean and mace secondary outputs, and cochineal through nopal cultivation.

Validated stock crop coverage:

- maize
- potato
- sweet potato
- cassava
- cacao
- tobacco
- sugar cane
- coffee bean
- tea leaf / tea cake
- indigo
- cochineal
- logwood
- cotton crop
- rice regional variants
- clove, nutmeg, mace, pepper, cinnamon, cardamom and other spices
- medicinal herbs, roots, gums, resins, and apothecary botanicals

Material foundations now exist for sugar loaf, molasses, cacao beans/nibs, tobacco leaf/twist, cotton fibre, indigo cake, cochineal, tea bricks/cakes, roasted coffee, snuff, and chocolate blocks. Finished items, packets/bales, and processing crafts remain branch work. Where a matching shared package exists, Agriculture and trade crafts should output or use that `preindustrial_trade_*` reference instead of creating a Renaissance-only container.

## Recommended branch references to produce after this master

The shared baseline reference and alias catalogue already exist and should be treated as dependencies rather than recreated.

1. `FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md`
2. `FutureMUD_Renaissance_Clothing_Accessories_Design_Reference.md`
3. `FutureMUD_Renaissance_Military_Firearms_Armour_Design_Reference.md`
4. `FutureMUD_Renaissance_Writing_Print_Administration_Design_Reference.md`
5. `FutureMUD_Renaissance_Household_Urban_Trade_Design_Reference.md`
6. `FutureMUD_Renaissance_Art_Craft_Science_Navigation_Design_Reference.md`
7. `FutureMUD_Renaissance_Agriculture_Food_Drink_Commodities_Design_Reference.md`
8. `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`
9. `FutureMUD_Renaissance_Culture_Manifest_Reference.md`

The admission manifest should map shared rows to cultures, date anchors, institutions, professions, shops, and crafts. It must not clone item prototypes.

## Exclusions

Exclude by default:

- Duplicates of any implemented `preindustrial_*`, `historic_*`, or `primary_production_*` row merely to gain a Renaissance prefix or different cosmetic presentation.
- Mature eighteenth-century uniforms.
- Flintlock-dominant state-army equipment as the default.
- Percussion-cap firearms, revolvers, rifles, metallic cartridges, and modern firearms.
- Steam engines, telegraphy, railways, industrial machinery, electric power, and powered devices.
- Modern stationery, modern office goods, modern postal envelopes, binders, staples, paperclips, and fountain pens.
- Mature Enlightenment scientific apparatus unless explicitly admitted as late-c.1600 edge cases.
- Industrial gases and modern chemical apparatus.
- Universal use of shared telescope, coffee, tea, cacao, tobacco, sugar, or gunpowder-support stock without culture/date admission.

## Open implementation decisions

1. Whether matchlock, wheellock, arquebus, caliver, and early musket need new firearm components or can be approximated safely with existing ranged-weapon components.
2. Whether blackpowder is a solid material, commodity item, craft-only ingredient, or specialised consumable; the shared baseline supplies support containers/tools only.
3. Whether the shared hand press and associated printing props need functional crafting components and production workflows.
4. Whether printed broadsides, pamphlets, almanacs, maps, music, and forms need distinct readable/writeable components or fixed writing-content support.
5. Which Renaissance object-form terms are acceptable in public item names: `doublet`, `jerkin`, `ruff`, `farthingale`, `arquebus`, `morion`, `cassone`, `majolica`, `portolan`, and similar terms.
6. Whether colonial/contact-zone goods are core or optional modules.
7. Whether navigation and scientific instruments require functional mechanics or remain descriptive/tool-tagged objects.
8. Whether generic household furniture should receive a later shared pre-industrial expansion or remain independently authored in Renaissance and EarlyModern branches.
9. Whether the dedicated new shared-stock rows should remain non-skinnable or have that policy revised centrally.
10. How Renaissance culture/shop/craft manifests will gate automatically seeded shared stock so catalogue availability is not mistaken for universal prevalence.

## Validation target

Before implementation, each Renaissance-specific row or manifest should confirm:

- The shared pre-industrial dispatcher remains the sole common-baseline entrypoint.
- No proposed row duplicates one of the 342 aliases or 44 named shared-stock targets.
- Crafts, outfits, shops, and manifests reference the shared `preindustrial_*` stable reference where one exists rather than a source `medieval_*` or `antiquity_*` reference.
- Shared rows are historically admitted by date, culture, institution, profession, or trade context before they are exposed as normal stock.
- Era-specific rows use stable Renaissance references and appropriate era tags.
- Public descriptions are form-based and avoid unsupported historical or mechanical claims.
- Materials, tags, and components exactly match maintained seeded data.
- Portable items include `Holdable`; fixtures omit it.
- Component-dependent claims are backed by actual components.
- Culture-specific prototypes are justified by form, material, component, institution, or production technology.
- Skins, not prototypes, handle most local names, colours, motifs, heraldry, inscriptions, and status variation.
- Shared baseline verification tests continue to pass, including idempotency, stable-reference uniqueness, valid dependencies, portability rules, lifecycle rewrites, no firearm/explosive components in support rows, and clean medieval-writing strings.
