# FutureMUD Early Modern Item Seeder Master Era Design Reference

> Post-shared-baseline edition. The common pre-industrial item layer is implemented; this document now defines the Early Modern-specific delta and historical-admission policy.

## Executive summary

This reference covers `EarlyModern`, the Enlightenment and early modern period roughly 1600-1750 CE. It remains a standalone era reference rather than a continuation section inside the Renaissance document.

The shared pre-industrial baseline is now live and is automatically installed when the Early Modern era is selected. The Early Modern-specific entrypoint, `SeedEarlyModernItems()`, exists but remains the place for future era-specific catalogue work. This document should therefore specify the delta beyond the live aliases and shared stock rather than recreating cross-era writing goods, trade containers, tools, military supports, printing equipment, navigation/science props, gunpowder-support objects, or global-trade packaging.

The Early Modern-specific seeder should emphasize:

- State armies, formal uniforms, actual matchlock/snaphaunce/flintlock/firelock weapons, ammunition, paper cartridges, naval warfare goods, and military administration beyond the shared support layer.
- Stronger urban, commercial, bureaucratic, legal, postal, and financial paper culture.
- Newspapers, gazettes, journals, specialised printed forms, bills of exchange, shipping papers, insurance papers, passports, and archive systems beyond the shared print vocabulary.
- Coffeehouses, tea, chocolate, tobacco, sugar, punch, porcelain, glassware, mirrors, clocks, and elite/urban domestic display.
- Scientific, optical, measuring, navigation, survey, natural-history, and Enlightenment collecting material culture beyond the shared twelve-item instrument set.
- Intensified Atlantic, Indian Ocean, Pacific, and Eurasian trade, colonial/contact-zone objects, plantation commodities, and export systems, while keeping sensitive historical contexts intentional.
- Ottoman, Safavid/post-Safavid, Mughal, Qing, Edo, Joseon, South-east Asian, African, Indigenous American, and colonial American material cultures as active contemporaries.

## Scope and era model

- Era token: `EarlyModern`.
- Era-selection key: `earlymodern`.
- Approximate chronological band: 1600-1750 CE.
- Era-specific rows should normally use `Era / Early Modern Era`; shared dependencies retain `Era / Pre-Industrial Era`.
- Chronology should be treated as a family of anchored contexts rather than a single universal year.
- This era may reuse Renaissance survivals, but its defaults should feel later: state bureaucracy, larger print ecology, commodity consumption, scientific instruments, and stronger global trade systems.
- The presence of a shared baseline item is catalogue availability, not proof of universal use in every Early Modern culture.
- Builder-facing labels may use historical inspiration names. Public item fields should remain form-based and in-world.

## Relationship to the Renaissance reference

The Early Modern seeder is not the second half of the Renaissance document. Both era selections receive the implemented shared pre-industrial baseline and then call their own era-specific entrypoint.

The live dispatcher sequence is conceptually:

```text
SeedSharedPreIndustrialBaselineItems(); // automatic for antiquity, medieval, renaissance, earlymodern
SeedEarlyModernItems();                 // Early Modern-specific rows
```

A general `EnsureRenaissanceSurvivalsWhereStillCredible()` helper is no longer part of the required dependency model. The `preindustrial_*` layer covers the broad cross-era forms. Once the Renaissance catalogue exists, individual Renaissance-specific items may be reused explicitly where they remain credible after 1600; the complete Renaissance branch should not be invoked as an Early Modern dependency.


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
- Do not create an Early Modern duplicate merely to obtain an Early Modern prefix, builder note, or cosmetic presentation.
- Ordinary portable goods include `Holdable`; fixed room features, installed architecture, terrain props, and permanent infrastructure omit it.
- Liquids and gases are not substitutes for the solid primary material argument.
- Do not claim functionality without a matching component.
- Skins vary presentation, not component behaviour. Where a dedicated shared row is currently non-skinnable, resolve any desired skin policy centrally rather than cloning the item in this branch.
- Public text avoids seeder mechanics, builder-facing labels, and unsupported behaviour claims.
- Era-specific crafts, outfits, shops, and manifests must gate historically or culturally restricted shared stock rather than assuming every automatically seeded row is common.

## Reuse from earlier seeders

### Default cross-era dependency rule

Prefer the live shared stable references:

- `preindustrial_writing_*` for shared manuscript, paper, ledger, and writing-implement forms.
- `preindustrial_trade_*` for non-regional trade containers, lockboxes, and shared commodity packaging.
- `preindustrial_door_*` for the promoted ordinary door and gate forms.
- `preindustrial_clothing_*` for the three promoted general accessories.
- `preindustrial_tool_*` and `preindustrial_workshop_*` for promoted workshop tools and fixtures.
- `preindustrial_military_support_*` for sheaths, scabbards, quivers, belts, racks, and armour-display supports.
- `preindustrial_time_*` and `preindustrial_water_*` for promoted timekeeping and civic-water forms.

There is no implemented `preindustrial_document_*` or `preindustrial_household_*` family. Writing rows use `preindustrial_writing_*`; the current household-like shared coverage is primarily non-regional trade containers, promoted doors, time/water fixtures, and selected accessories. Generic household furniture remains a separate design problem.

The `historic_*` workshop foundation and `primary_production_*` package are already seeded by the shared entrypoint and should not be re-invoked inside `SeedEarlyModernItems()`.

### Direct reuse outside the baseline

Direct use of `antiquity_*`, `medieval_*`, or future `renaissance_*` rows should be item-specific and exceptional. If a form is broadly useful across Renaissance and Early Modern and has no alias, extend the shared baseline rather than maintaining two duplicate prototypes.

### Reuse mainly through skins

- Renaissance art, classical revival, courtly trim, heraldry, local names, guild marks, inscriptions, and textile motifs should mostly persist as skins.
- Indigenous, colonial, export, and hybrid forms need prototypes only where material, form, component, production technology, or gameplay role changes.

### Deliberately not supplied by the shared baseline

The Early Modern branch still owns, where appropriate:

- coats, waistcoats, stays, mantuas, wigs, uniform systems, footwear, and other period-specific clothing;
- actual firearms, ammunition, cartridges, bayonets, and naval/military weapon systems;
- newspapers, gazettes, postal/financial/legal formats, and larger print institutions beyond the shared minimum;
- coffeehouse, tea/chocolate/tobacco service culture and general household furniture;
- microscopes, barometers, thermometers, clocks/watches, and advanced scientific/natural-history systems;
- culture-specific, colonial, mission, plantation, company, and contact-zone objects.

## Shared-first branch architecture

### Default rule

Create shared EarlyModern prototypes wherever the item can plausibly serve multiple cultures with only presentation changes. This era has a very strong shared global-trade layer: barrels, casks, ledgers, printed forms, tea chests, sugar loaves, tobacco bundles, porcelain crates, coffee sacks, shipping papers, lockboxes, naval stores, and scientific instruments often travel across cultures.

### When to make a distinct culture-family prototype

Create a separate prototype when one or more of the following is true:

- The silhouette is visibly distinct enough that a skin would mislead.
- The component differs.
- The object belongs to a different institutional system, such as postal, military, court, mosque, temple, coffeehouse, plantation, academy, guild, company, or colonial administration.
- The production technology differs.
- The primary material changes behaviour or craft use.
- Contact-zone or export status changes the object's form or packaging.
- The item is a strong builder-facing regional anchor.

### What skins should carry

Skins should carry:

- local names
- regimental, company, guild, household, or dynastic marks
- ship names and port marks
- exact printed titles and dates
- language/script
- seal impressions
- heraldry
- textile motifs
- porcelain/glaze/lacquer patterns
- maker marks
- furniture carving styles
- imported/export presentation
- commodity labels
- status variation

## Culture coverage table

| Inspiration family | Reference anchor | Coverage boundary | EarlyModern material-culture focus | Shared grouping |
|---|---:|---|---|---|
| French / Baroque court and urban | c. 1650-1750 | Bourbon France and French-influenced elite/urban culture | coats, waistcoats, gowns, wigs, lace, mirrors, clocks, furniture, court goods, military uniforms | Shared Western European Baroque |
| Dutch Republic / Low Countries | c. 1600-1720 | Dutch urban, maritime, mercantile, colonial networks | print, maps, shipping papers, household interiors, trade chests, ceramics, tobacco, naval goods | Shared Atlantic / mercantile |
| English / British Stuart-Georgian | c. 1600-1750 | England, Scotland, Ireland, British Atlantic world | coats, breeches, stays, wigs, coffeehouse goods, legal/admin paper, naval stores, colonial trade | Shared British Atlantic |
| Iberian / Portuguese-Spanish empires | c. 1600-1750 | Iberia and Atlantic/Pacific colonial worlds | Catholic goods, colonial administration, silver trade, sugar/cacao/tobacco packaging, maritime goods | Shared Iberian colonial |
| German / HRE / Austrian | c. 1600-1750 | imperial towns, Austrian/Habsburg contexts, central Europe | firearms, uniforms, clocks, guild tools, printed books, urban furnishings, military papers | Shared Central European |
| Italian states | c. 1600-1750 | Italian courts, cities, maritime states, papal contexts | art goods, glass, furniture, church goods, print, music sheets, court clothing | Shared Italian / Catholic |
| Scandinavian / Baltic | c. 1600-1750 | Denmark-Norway, Sweden, Baltic trade and military contexts | naval stores, Lutheran furnishings, uniforms, timber, tar, trade packaging, household goods | Shared Northern / Baltic |
| Polish-Lithuanian / Hungarian frontier | c. 1600-1750 | eastern European noble/frontier/military worlds | cavalry gear, sabre-like swords via components/skins, coats/robes, firearms, saddlery, trade goods | Shared Central/Eastern frontier |
| Russian / Petrine and post-Petrine | c. 1600-1750 | Muscovite to early Imperial Russia | caftans and westernising clothing, icons, chancery, uniforms, firearms, trade, fur goods | Shared Russian / northern |
| Ottoman | c. 1600-1750 | Anatolia, Balkans, Levant, eastern Mediterranean, North African overlaps | coffeehouse goods, kaftans, ceramics, firearms, chancery, mosque/madrasa goods, trade wares | Shared Ottoman-Islamicate |
| Maghrebi / North African | c. 1600-1750 | Morocco, Algeria, Tunisia, Libya-facing worlds | robes, leather, ceramics, firearms, corsair/naval goods, Islamic urban goods, trade packaging | Shared North African / Mediterranean |
| Safavid / post-Safavid Persianate | c. 1600-1750 | Iran, Caucasus, Persianate successor contexts | carpets, ceramics, manuscripts/print-adjacent paper, robes, metalwork, firearms, coffee/tea goods | Shared Persianate |
| Mughal / Indo-Persian | c. 1600-1750 | Mughal north India and Indo-Persian court worlds | textiles, jamas/robes, turbans, jewellery, manuscripts, weapons, hookah/tobacco goods, court furniture | Shared Indo-Persian |
| Maratha / Rajput / Deccan | c. 1650-1750 | regional South Asian polities and warrior/court contexts | cotton/silk, turbans, arms, saddlery, court goods, trade chests, paper administration | Shared South Asian regional |
| South Indian / coastal trade | c. 1600-1750 | Coromandel, Malabar, Tamil, Telugu, Mysore, coastal trade | cotton textiles, chintz/calico, brass vessels, maritime trade, temple goods, company contact | Shared South Asian maritime |
| Qing China | c. 1644-1750 | early Qing court, scholar, merchant, export contexts | porcelain, tea, silk, lacquer, paper, furniture, scholar goods, export packing | Shared Qing / East Asian |
| Late Ming survival / transition | c. 1600-1650 | transitional China where useful | late Ming furniture, porcelain, books, scholar goods, maritime trade | Shared East Asian transition |
| Joseon Korea | c. 1600-1750 | Joseon court, scholar, commoner, military contexts | paper, ceramics, robes, hats, scholar goods, storage, firearms/contact goods | Shared East Asian literati |
| Edo Japan | c. 1600-1750 | Tokugawa Japan, urban merchant, warrior, tea, print contexts | kosode, hakama, kamishimo, lacquer, tea, woodblock print, books, urban goods, swords | Shared Edo Japanese |
| Ryukyu and maritime East Asia | c. 1600-1750 | Ryukyu and trade-facing island networks | lacquer, textiles, tribute goods, trade boxes, ceramics, maritime contact | Shared maritime East Asian |
| Mainland South-east Asian courts | c. 1600-1750 | Ayutthaya, Burmese, Khmer/Lan Xang, Vietnamese contexts | lacquer, bronze, temple/court goods, textiles, firearms contact, trade packaging | Shared South-east Asian mainland |
| Maritime South-east Asian trade worlds | c. 1600-1750 | Malay, Indonesian, Philippine, spice and colonial contact zones | spice trade, Islamic/coastal goods, textiles, ceramics, boats, company trade goods | Shared maritime South-east Asian |
| Inner Asian / steppe frontier | c. 1600-1750 | Kazakh, Tatar, Uzbek, Manchu frontier, Mongol contexts | felt, leather, horse gear, caravan goods, firearms contact, Qing/Russian/Ottoman trade objects | Shared steppe and caravan |
| West African court and Atlantic trade | c. 1600-1750 | Benin, Yoruba, Akan/Gold Coast, early Asante-facing contexts | brass, ivory, beads, textiles, court regalia, firearms/trade goods, weights and measures | Shared West African / Atlantic |
| Kongo / Angola / West Central Africa | c. 1600-1750 | Kongo, Angola, Portuguese/Atlantic contact worlds | Christian/contact goods, textiles, metalwork, trade beads, ivory, colonial documents | Shared West Central African contact |
| Sahelian / Hausa / Islamic West Africa | c. 1600-1750 | Songhai successors, Hausa states, trans-Saharan networks | leather, manuscripts, robes, horse gear, metalwork, trade and scholarly goods | Shared Sahelian Islamic |
| Ethiopian / Red Sea | c. 1600-1750 | Ethiopian highlands and Red Sea networks | Christian liturgical goods, manuscripts, shields, trade goods, textiles, firearms contact | Shared Red Sea |
| Swahili Coast / Indian Ocean Africa | c. 1600-1750 | East African coast and Indian Ocean commerce | Islamic urban goods, imported ceramics, beads, maritime trade, ivory, cloth, containers | Shared Indian Ocean |
| Spanish colonial Americas | c. 1600-1750 | New Spain, Peru, missions, towns, mines, Indigenous/colonial intersections | mission goods, silver, cacao, tobacco, legal paper, textiles, firearms, administrative goods | Shared Spanish colonial |
| Portuguese Brazil / Atlantic plantation | c. 1600-1750 | Brazil and Portuguese Atlantic | sugar, tobacco, Brazilwood/logwood-like dyestuffs, plantation packaging, Catholic goods, colonial admin | Shared Portuguese Atlantic |
| English / French / Dutch colonial North America | c. 1600-1750 | Atlantic colonies, forts, settlements, trade posts | trade goods, firearms, iron tools, documents, tavern goods, plantation crops, Indigenous contact goods | Shared colonial North America |
| Indigenous North American regional families | c. 1600-1750 | broad regional placeholder pending deeper subreferences | hide, baskets, beadwork, wampum-like goods, pipes, trade cloth, metal tools/firearms contact | Shared Indigenous / contact |
| Mesoamerican colonial and Indigenous | c. 1600-1750 | colonial New Spain and Indigenous continuities | cacao, cotton, paper/codex survival, church goods, craft goods, colonial admin | Shared Mesoamerican colonial |
| Andean colonial and Indigenous | c. 1600-1750 | colonial Andes and Indigenous continuities | camelid textiles, silver/mining goods, quipu survival if approved, church/admin goods, ceramics | Shared Andean colonial |
| Caribbean / Atlantic plantation | c. 1600-1750 | Caribbean colonial and contact worlds | sugar, rum, tobacco, plantation tools/containers, maritime trade, colonial admin | Shared Caribbean / plantation |
| Global maritime and chartered-company trade | c. 1600-1750 | cross-cultural overlay | company ledgers, bills of lading, shipping papers, tea chests, spice chests, sugar casks, naval stores, maps, instruments | Shared global trade |

## Shared cross-cultural groupings

| Shared grouping | Use case |
|---|---|
| Shared Western European Baroque / Enlightenment | coats, waistcoats, breeches, gowns, wigs, lace, furniture, clocks, mirrors, printed/admin goods |
| Shared Atlantic / colonial | shipping papers, trade chests, casks, plantation packaging, mission/admin goods, tavern goods, firearms contact |
| Shared global maritime / company trade | navigation instruments, shipboard trunks, ledgers, manifests, bills of lading, commodity packaging |
| Shared Ottoman-Islamicate | coffee, ceramics, kaftans, sashes, mosque/madrasa goods, chancery, urban trade |
| Shared Persianate / Indo-Persian | robes, carpets, manuscripts, metalwork, court goods, weapons, tobacco/hookah goods |
| Shared East Asian export/literati | tea, porcelain, lacquer, brush/ink/paper, scholar goods, export crates, books |
| Shared South Asian textile trade | cotton, calico, chintz, dyes, bales, brass vessels, court textiles, company-contact goods |
| Shared coffeehouse / public house | cups, pots, pipes, benches, tables, newspapers, pamphlets, account boxes, serving trays |
| Shared natural philosophy / scientific | telescope, microscope, specimen jar, cabinet drawer, balance, barometer, thermometer, survey kit |
| Shared plantation / cash-crop | sugar, tobacco, cotton, indigo, cacao, coffee, rum, molasses, barrels, sacks, press/refining tools |
| Shared Indigenous / contact | trade beads, metal tools, trade cloth, firearms contact, hybrid storage, pipes, baskets, local continuities |

## Item-domain guidance

### Clothing and accessories

The live baseline contributes only three generic accessories: `preindustrial_clothing_plain_leather_belt`, `preindustrial_clothing_iron_buckled_leather_belt`, and `preindustrial_clothing_simple_woven_sash`. Early Modern shirts, shifts, stays, bodices, coats, waistcoats, breeches, mantuas, gowns, uniforms, wigs, hats, footwear, and regional clothing systems remain era-specific work.

EarlyModern clothing should not be treated as late Renaissance clothing with extra trim. Target coverage:

- Western/central European: shirts, shifts, stays, bodices, coats, waistcoats, breeches, petticoats, mantuas, gowns, riding habits, cloaks, capes, stockings, buckled shoes, boots, wigs, tricorns, cocked hats, bonnets, caps, gloves, fans, muffs, aprons, uniforms and liveries.
- Ottoman, Persianate, Mughal, South Asian, Qing, Joseon, Edo, South-east Asian, African, Indigenous American, and colonial hybrid clothing where silhouette and textile practice differ.
- Professional and institutional overlays: soldier, sailor, officer, servant/livery, merchant, lawyer/notary, clerk, printer, scientist/natural philosopher, apothecary, surgeon, coffeehouse worker, domestic servant, plantation overseer/workwear if in scope.
- Avoid public historical labels unless the object-form term is useful. Use skins for livery colours, regimental marks, household badges, local names, exact trims, and status variants.

### Military, firearms, uniforms, naval gear, and support

Do not recreate the live shared gunpowder-support rows: `preindustrial_firearms_powder_horn`, `preindustrial_firearms_powder_flask`, `preindustrial_firearms_priming_flask`, `preindustrial_firearms_shot_pouch`, `preindustrial_firearms_match_cord_bundle`, `preindustrial_firearms_musket_wadding_packet`, `preindustrial_firearms_bullet_mould`, `preindustrial_firearms_ramrod`, `preindustrial_firearms_touchhole_pick`, and `preindustrial_firearms_cleaning_rod`. The baseline also supplies 52 `preindustrial_military_support_*` aliases.

Early Modern should become the principal branch for preindustrial firearm combat systems and organised military material culture:

- Actual matchlock, snaphaunce, flintlock/firelock musket and pistol families where components allow.
- Musket balls, loose shot, paper cartridges, cartridge boxes, cartridge slings, worms, gunflints, combination gun tools, and repair/maintenance items not already in the shared support set.
- Bayonets, plug/socket bayonet variants if component-safe, hanger swords, smallswords, sabres via suitable components/skins, boarding axes, and surviving pikes/halberds.
- Uniform coats, waist belts, gorgets, sashes, drums, fifes, standards, camp kettles, campaign chests, and officer writing cases.
- Naval support: speaking trumpets, signal flags, sea chests, powder barrels, shot lockers, sailcloth, rope, tarred buckets, boarding goods, and navigational kits.
- Armour survivals: breastplates, cuirasses, gorgets, buff coats, helmets, and culturally persistent shields; full plate should not be the battlefield default.

Boundary:

- Exclude percussion caps, rifled mass-firearms as the default, revolvers, metallic cartridges, repeating weapons, modern artillery shells, modern explosives, machine guns, ballistic armour, and modern uniforms.
- Do not use bomb/explosive components without a separately approved and mechanically grounded blackpowder/explosive branch.

### Writing, print, books, documents, administration, and finance

The baseline already supplies 39 `preindustrial_writing_*` aliases and the minimum print/paper-administration set: hand press, type case, composing stick, chase, inking balls, drying rack, broadside, pamphlet, almanac, blank form, and printed map sheet. Use those references for the generic forms.

Early Modern should extend them into a much larger publishing, postal, legal, commercial, financial, and administrative ecology:

- Newspapers, gazettes, printed sermons, proclamations, playbills, tickets, journals, natural-philosophy papers, atlases, music sheets, catalogues, and printed books whose format or component differs from the shared pamphlet/almanac/broadside/map.
- Letters, period-safe envelopes or folded-letter packets, postal bags, courier satchels, deed boxes, archive systems, filing bundles, dispatch packets, and seal/security goods beyond existing aliases.
- Daybooks, ship logs, port books, customs registers, plantation/company/notary ledgers, legal briefs, warrants, passes, passports, indentures, contracts, bills of lading, bills of exchange, insurance policies, and proto-share papers.
- Inkstands, sand shakers, penknives, sealing-wax goods, portable writing desks, clerk kits, and office furniture not already present.
- Movable type, punches/matrices, formes, press furniture, rollers where period-appropriate, paper reams, bookshop shelving, pamphlet racks, and distribution/storage goods beyond the shared press set.

Boundary:

- Exclude fountain pens, ballpoints, staples, paperclips, rubber erasers, typewriters, modern file folders, binders, carbon paper, and other later office systems.

### Household, urban, tavern, coffeehouse, and public-house goods

The shared layer already supplies 147 non-regional `preindustrial_trade_*` container/lockbox aliases, promoted ordinary doors, and global packaging for tea, coffee, cacao, tobacco, sugar, spices, indigo, porcelain, glass bottles, silk, and cotton. Use those stable references for the package itself. It does not supply the consumption/service complex or a general household-furniture catalogue.

Early Modern-specific additions should include:

- Coffee cups, coffee pots, grinders/roasters, serving trays, coffeehouse benches/tables, account boxes, and newspaper/pamphlet display.
- Teapots, tea bowls/cups, tea caddies distinct from the shared shipping chest, sugar bowls, strainers, and porcelain table services.
- Chocolate pots, cacao cups, molinillo-like stirrers where appropriate, punch bowls, ladles, and drinking glasses.
- Tobacco pipes, pipe cases, tobacco boxes, snuff boxes, spittoons where desired, and hookah/narghile systems in relevant cultures.
- Mirrors, framed pictures, clocks, cabinets, chests of drawers, escritoires, portable desks, bookcases, display cabinets, glass-fronted cupboards, fire screens, candle snuffers, chandeliers, sconces, and lanterns.
- Tavern/public-house goods: tankards, mugs, bottles, jugs, taps/spigots if component-safe, dice/cards, scoreboards, benches, and counters.
- Consumption-ready sugar, tea, coffee, cacao, tobacco, rum, and related goods; the shared containers alone do not represent their contents.

Do not create a second tea chest, coffee sack, cacao sack, tobacco bale, sugar hogshead, spice chest, porcelain crate, or bottle crate unless the form or component is materially different.

### Science, navigation, optics, survey, and natural philosophy

The shared catalogue already contains a magnetic compass, dividers, cross-staff, mariner's astrolabe, chart case, measuring chain, plane table, spectacles, magnifying lens, draw-tube telescope, balance scales, and specimen jar. Use those stable references for the generic forms.

Early Modern should become the first major science/instrument branch by adding the missing instrument families and institutional systems:

- Microscopes, lens cases, lens blanks, optical tubes/stands, and specialised optical workshop goods.
- Barometers, thermometers, pendulum clocks, pocket watches, hourglasses, sundials, and their cases/stands.
- Backstaves, quadrants, octants near the late edge, parallel rulers, measuring rods, protractors, sectors, weight sets, and culture-specific navigation or surveying forms.
- Globes, celestial globes, armillary spheres, and carefully date-bounded orrery-like devices where supported.
- Herbarium volumes, mineral boxes, shell drawers, insect cases, collecting nets, sample vials, cabinet drawers, labelled boxes, and specimen-storage furniture beyond the shared jar.
- Alembic/retort props, balances and weights beyond the shared generic scales, mortars, pestles, laboratory glass, medicine chests, and surgical kits.

The shared instrument rows are mainly descriptive or container-backed. Functional navigation, optics, timekeeping, measurement, and scientific effects require explicit component decisions rather than prose claims.

### Agriculture, food, drink, plantation, and commodity goods

The shared baseline supplies reusable packaging for tea, coffee, cacao, tobacco, sugar, spices, indigo, porcelain, bottles, silk, and cotton. It does not seed the crops, edible/drinkable goods, processing outputs, plantation systems, labour regimes, or trade crafts.

Early Modern agriculture and commodity work should include:

- Maize, potato, sweet potato, cassava, cacao, tobacco, sugar cane, coffee, tea, indigo, cochineal, cotton, rice variants, spices, and medicinal botanicals.
- Sugar loaves, molasses, rum, tobacco leaf/twist, snuff, coffee beans and roasted coffee, cacao beans/nibs and chocolate blocks, tea bricks/cakes, indigo cakes, cochineal packets, cotton bolls/fibre, and other processed outputs.
- Food/drink service and consumable items for coffee, tea, chocolate, punch, wine, ale/beer, spirits where permitted, vinegar, oils, and syrups.
- Cargo marks, bills of lading, warehouse tags, company/port handling goods, and region-specific crafts that use the shared packages.

Plantation, enslavement, mission, and colonial-company contexts must be represented intentionally and separately from a neutral list of commodities. Do not fold coercive labour systems into generic trade flavour.

### Religious, learned, and institutional goods

Include:

- Church, chapel, synagogue, mosque, madrasa, temple, shrine, mission, college, academy, coffeehouse, guild, company, court, and colonial office furnishings/document goods.
- Printed devotional books, prayer books, hymnals, Qur'an/book stands where appropriate, scripture boxes, icon/altar furnishings, mission registers, donation boxes, alms chests, incense goods, offering vessels.
- Learned and scientific institutions: lecture props, demonstration instruments, cabinets, specimen storage, globes, maps, papers, archive shelves.

## Primary Industry Seeder impacts

### Already resolved by the shared baseline

- `SeedPrimaryProductionToolsAndProps()` is automatically invoked for Early Modern selection.
- The historic workshop foundation and 77 promoted cross-period tools/fixtures are available without Early Modern duplicates.
- Shared props exist for printing, navigation/survey, optical/scientific work, gunpowder handling, and global-trade packaging.

These are item primitives only; they do not implement complete resource, craft, manufacturing, military, scientific, or commodity chains.

### Still required or subject to audit

- Blackpowder, saltpetre/nitre, brimstone/sulfur, charcoal grades, lead shot, cartridge paper, gunflints, and production/safety chains.
- Gunsmithing: barrels, locks, springs, screws, stock blanks, actual firearm components, repair tools, and ammunition manufacture.
- Printing: type metal, movable type, composing/locking materials, press hardware, oil-based inks, paper reams, and functional publishing crafts.
- Glass and optics: clear/lens/mirror glass, telescope and microscope lens systems, polishing powders, and optician crafts.
- Clockmaking and instruments: brass gears, springs, dials, pendulums, clock/watch cases, and instrument production.
- Porcelain and ceramics: porcelain, kaolin/china clay, glaze, stoneware, faience/Delft-like and export ceramic support.
- Textile and dye industries: cotton, calico/chintz, broadcloth, lace, ribbon, velvet, satin, taffeta, indigo, cochineal, and logwood.
- Cash-crop processing: sugar refining, molasses/rum production, tobacco curing, coffee roasting, cacao processing, and tea packing.
- Maritime stores: sailcloth, rope, tar, pitch, oak staves, barrels, blocks, tackle, canvas, and caulking.

## Useful Seeder impacts

### Already resolved

- `Pre-Industrial Era` and `Early Modern Era` tags are live in the maintained tag hierarchy.
- The shared print, navigation/science, gunpowder-support, global-trade, tool, writing, container, door, time/water, and military-support stock uses maintained materials, tags, and components.
- Early Modern selection automatically receives the common primary-production and historic workshop packages.

### Still required

Add or validate non-duplicative toolkits for:

- clerk/notary/merchant/company factor
- printer/bookseller/bookbinder
- cartographer/navigator/surveyor
- natural philosopher/scientific instrument user
- optician/lens grinder
- clockmaker/watchmaker
- gunsmith/firelock maintainer
- powder handler
- soldier/officer campaign kit
- sailor/shipwright/sailmaker/ropemaker/cooper
- apothecary/surgeon
- coffeehouse/tavern keeper
- tea/chocolate service
- plantation/cash-crop processing if in scope
- colonial office/mission registrar

Before creating a toolkit or tool, check the shared alias catalogue and named stock. A kit can be a container of descriptive objects, but functional tools need exact tool components.

## Agriculture Seeder impacts

The shared baseline does not seed crops, foods, drinks, plantation production, or processing chains. It supplies reusable packaging that those systems can consume or output.

Add or validate:

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
- spices: clove, nutmeg, mace, pepper, cinnamon, cardamom, saffron, allspice
- medicinal botanicals and apothecary plants

Processing outputs should include sugar loaf, molasses, rum, tobacco leaf, tobacco twist, snuff, coffee bean, roasted coffee, cacao bean, cacao nibs, chocolate cake/block, tea brick/cake, cotton bale, indigo cake, cochineal packet, and spice sachets. Use the appropriate `preindustrial_trade_*` packaging stable reference rather than creating an Early Modern-only sack, bale, chest, crate, or hogshead for the same form.

## Recommended branch references to produce after this master

The shared baseline reference and alias catalogue already exist and should be treated as dependencies rather than recreated.

1. `FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md`
2. `FutureMUD_EarlyModern_Clothing_Accessories_Design_Reference.md`
3. `FutureMUD_EarlyModern_Military_Firearms_Uniforms_Naval_Design_Reference.md`
4. `FutureMUD_EarlyModern_Writing_Print_Administration_Finance_Design_Reference.md`
5. `FutureMUD_EarlyModern_Household_Coffeehouse_Tavern_Trade_Design_Reference.md`
6. `FutureMUD_EarlyModern_Science_Navigation_Optics_Measurement_Design_Reference.md`
7. `FutureMUD_EarlyModern_Agriculture_Food_Drink_Commodities_Design_Reference.md`
8. `FutureMUD_EarlyModern_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`
9. `FutureMUD_EarlyModern_Culture_Manifest_Reference.md`

The admission manifest should map shared rows to cultures, date anchors, institutions, professions, shops, military systems, and crafts. It must not clone item prototypes.

## Exclusions

Exclude by default:

- Duplicates of any implemented `preindustrial_*`, `historic_*`, or `primary_production_*` row merely to gain an Early Modern prefix or cosmetic variation.
- Industrial Revolution machinery unless a later branch extends the date.
- Steam engines as default item technology.
- Telegraphy, railways, powered factories, electricity, batteries, and modern communications.
- Percussion caps, revolvers, metallic cartridges, repeating rifles, machine guns, and modern artillery shells.
- Modern explosives and bombs unless a separate approved branch handles them safely and component-correctly.
- Modern stationery, typewriters, carbon paper, paperclips, staples, binders, modern envelopes, fountain pens, and ballpoint pens.
- Modern medical equipment, modern laboratory equipment, photography, and electrical scientific devices.
- Industrial gases and modern chemical-plant equipment.
- Universal use of shared commodity packaging, firearms support, printing, or scientific instruments without culture/date/institution admission.

## Open implementation decisions

1. Whether flintlock, snaphaunce, matchlock, musket, pistol, and firelock distinctions need new components or can be represented safely with existing ranged-weapon systems.
2. Whether paper cartridges, musket balls, loose shot, and blackpowder should be functional ammunition/consumables, craft stock, or descriptive goods; the shared baseline deliberately contains support items only.
3. Whether bayonets can attach mechanically to firearms or must be separate dagger/spear-like items.
4. Whether the shared printing props need functional press/publishing crafts and whether newspapers, gazettes, bills, passports, and financial papers need specialised readable/writeable components.
5. Whether scientific instruments need functional components, and which new systems are justified for microscope, barometer, thermometer, clock/watch, surveying, and natural-history use.
6. Whether tea, coffee, chocolate, tobacco, and sugar consumption goods are core or optional modules; their packaging is already shared stock.
7. How explicitly to handle plantation, slavery, mission, company, and colonial-administration goods in the default seeder.
8. Whether generic household furniture should receive a later shared pre-industrial expansion or remain independently authored in Renaissance and Early Modern branches.
9. Whether the dedicated new shared-stock rows should remain non-skinnable or have that policy revised centrally.
10. How Early Modern culture/shop/craft manifests will gate automatically seeded shared stock so availability is not mistaken for universal prevalence.

## Validation target

Before implementation, each Early Modern-specific row or manifest should confirm:

- The shared pre-industrial dispatcher remains the sole common-baseline entrypoint.
- No proposed row duplicates one of the 342 aliases or 44 named shared-stock targets.
- Crafts, outfits, shops, and manifests reference the shared `preindustrial_*` stable reference where one exists rather than a source `medieval_*`, `antiquity_*`, or duplicate `renaissance_*` reference.
- Shared rows are historically admitted by date, culture, institution, profession, military system, or trade context before they are exposed as normal stock.
- Era-specific rows use stable Early Modern references and appropriate era tags.
- Public descriptions are form-based and avoid unsupported historical or mechanical claims.
- Materials, tags, and components exactly match maintained seeded data.
- Portable items include `Holdable`; fixtures omit it.
- Component-dependent claims are backed by actual components.
- Culture-specific prototypes are justified by form, material, component, institution, production technology, or historically significant contact-zone role.
- Skins, not prototypes, handle most local names, colours, motifs, heraldry, inscriptions, company marks, regimental marks, and status variation.
- Sensitive contact-zone, plantation, enslavement, mission, and colonial objects are intentionally included rather than casually implied.
- Shared baseline verification tests continue to pass, including idempotency, stable-reference uniqueness, valid dependencies, portability rules, lifecycle rewrites, no firearm/explosive components in support rows, and clean medieval-writing strings.
