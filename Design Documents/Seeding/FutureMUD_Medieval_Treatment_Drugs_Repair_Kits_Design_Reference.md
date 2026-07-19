# FutureMUD Medieval Treatment Items, Drugs, and Repair Kits Design Reference

**Status:** implemented item-catalogue source for the medieval treatment, drug, mobility, prosthetic, and repair package.
**Date:** 27 June 2026
**Era band:** approximately 500-1300 CE
**Main catalogue size:** 183 item prototypes.
**Implementation files:** `DatabaseSeeder/Seeders/ItemSeeder.MedievalMedical.cs` and `DatabaseSeeder/Seeders/ItemSeeder.MedievalRepairKits.cs`.

This document merges the dependency design reference with the finished catalogue. The dependency sections preserve the authoring contract and validated seeded assets; the catalogue sections are the exact final item rows used by the medieval item seeder. The standalone CSV companion is intentionally not embedded as a project resource; its `sdesc` and `fdesc` values are represented here and in the direct `CreateItem(...)` calls.

## Implementation Summary

- `SeedMedievalMedicalAndApothecaryItems` owns the 150 treatment, apothecary, drug-delivery, mobility, casualty-transport, and prosthetic item prototypes.
- `SeedMedievalRepairKits` owns the 33 repair-kit and repair-supply item prototypes.
- Each item prototype is represented by exactly one direct `CreateItem(...)` call, with no generated in-repo data table or embedded CSV/resource dependency.
- The dependency seeders for drugs, liquids, medicine-vessel components, incense components, specialist repair-kit components, tags, and exact medical materials are treated as prerequisites rather than reseeded by this item-catalogue pass.

## Dependency And Authoring Reference

## 1. Review outcome

The attached post-Codex exports have been checked against the previous v1 requirements.

| Export | Current count | Review result |
| --- | ---: | --- |
| `Seeded_Item_Components.json` | 1,388 component prototypes | Required medicine vessels, incense burners, wrapper components, and specialist repair kits are present. |
| `Seeded_Liquids.json` | 88 liquids | All eight medieval medicinal liquids are present and tagged as medicine liquids. |
| `Item_Component_Types.json` | 190 component types | `IncenseBurner` is now present with container/lightable interfaces documented. |
| `Seeded_Materials.json` | 882 materials | Sufficient exact solid materials exist for the implemented item catalogue. |
| `SeededTagHierarchy.csv` | 1,900 tag paths | Medical craft-stock, medicine-market, fumigation, styptic, antidote, and specialist repair tags are present. |

There were no duplicate key names found in the component, liquid, component-type, material, or tag exports. No blocking data additions were required for the item catalogue.

### 1.1 Seed additions requested before catalogue authoring

**Completed in the core material pass.**

The catalogue-authoring review identified seven exact materials that should exist in core data rather than being substituted with abstractions:

- `alum`
- `ephedra`
- `foxglove`
- `gut`
- `henbane`
- `mandrake`
- `yarrow`

These are now required source assets for the item catalogue. Use them where the item identity depends on a specific named medicine stock. Use broader abstractions such as `herb`, `leaf`, `spice`, `salt`, `honey`, `beeswax`, `sinew`, `linen`, `silk`, `ceramic`, or `glass` only for generic mixtures, neutral carrier stock, packaging, or vessels.

### 1.2 Completed dependency workstreams

The previous A-E coding workstreams should now be treated as complete for catalogue-authoring purposes:

- `HealthSeeder` has a `medieval` tech level.
- The medieval drug catalogue exists.
- The eight medicinal liquids exist.
- Generic and default-loaded medicine-vessel components exist.
- Curated medical `IncenseBurner` components exist.
- Specialist glass, paper/parchment, lacquer, cordage, and composite-bow repair components exist.
- The required tag and metadata exports have been refreshed.

The item-catalogue implementation MUST NOT reseed drugs, liquids, tags, item components, or component types. It creates item prototypes that reference the exported names.

---

## 2. Source-of-truth files for the item catalogue

Use these current project files as the validation boundary:

| File | Use in the item catalogue |
| --- | --- |
| `Item_Authoring_Guidelines.md` | Description, size, quality, material, tag, skin, and component authoring rules. |
| `FutureMUD_Item_Seeder_Working_Guidelines.md` | Paste-ready `CreateItem(...)` conventions, exact-name validation, C# syntax, weights, and costs. |
| `Seeded_Item_Components.json` | Exact component prototype names. |
| `Item_Component_Types.json` | Component type behaviour and exclusive interfaces. |
| `Seeded_Materials.json` | Exact solid material names for the `material` argument. |
| `Seeded_Liquids.json` | Exact liquid names, mainly for default-loaded medicine-vessel awareness. |
| `SeededTagHierarchy.csv` | Exact item tag paths. |
| `Drug_System_Design.md` | Drug vectors, delivery wrappers, liquids, smoke, and incense behaviour. |
| `Drug_Builder_Guide.md` | Builder-facing drug/delivery vocabulary and safety notes. |
| Current medieval clothing, writing, military, and household references | Shared culture-family model, status/role conventions, and description style. |

### 2.1 Expected item-seeder location

The catalogue is implemented as focused medieval rework partials:

```text
DatabaseSeeder/Seeders/ItemSeeder.MedievalMedical.cs
DatabaseSeeder/Seeders/ItemSeeder.MedievalRepairKits.cs
```

Each row is implemented as a direct `CreateItem(...)` call in the same style as the other direct-call medieval partials. The rows are directly auditable as normal `CreateItem(...)` arguments.

Recommended stable-reference prefixes:

- `medieval_medical_` for wound care, tools, mobility aids, prosthetics, and medical support goods.
- `medieval_drug_` for medicine, drug-delivery, apothecary, and fumigation goods.
- `medieval_repair_` for repair kits and repair supplies.

---

## 3. Runtime model the catalogue must respect

### 3.1 Drugs and delivery wrappers are separate

A `Drug` row defines pharmacology. Catalogue items deliver the drug through components or through drug-bearing liquid contents.

The catalogue should use these delivery paths:

| Delivery form | Runtime path | Catalogue use |
| --- | --- | --- |
| Drinkable tea, draught, infusion, brew, tincture, tonic, or syrup | Drug-bearing `ILiquid` inside a `LiquidContainer` | Use default-loaded `LContainer_Medicine_*` components. |
| Solid swallowed dose | `Pill_<DrugName>` | Describe as bolus, pellet, lozenge, electuary ball, wrapped dose, or compressed herbal cake. Do not describe as a modern tablet or capsule. |
| Salve, poultice, styptic, ointment, burn dressing | `TopicalCream_<DrugName>` | May be combined with `Bandage_*` when the item is also a dressing. |
| Personal smoke | `Smokeable_<DrugName>` | Use for held cones, rolls, prepared bowls, or smoking pellets directly smoked by one user. |
| Room-scale fumigation | `IncenseBurner_<DrugName>` | Use for censers, braziers, fumigation bowls, and prepared burners. The burner component carries the drug, not the fuel item. |

Ordinary liquid wetness does not apply touched-vector drug dosing. Use `TopicalCream` when the item must deliver a touched drug.

### 3.2 Component exclusivity rules that matter here

The catalogue MUST avoid conflicting component combinations.

| Component family | Interfaces to watch | Authoring consequence |
| --- | --- | --- |
| `LiquidContainer` | `ILiquidContainer`, `ILockable`, `IOpenable` | Use only one liquid-container component per item. Do not also add a dry `Container` unless the item is intentionally multi-compartment and the runtime supports the combination. |
| `IncenseBurner` | `IIncenseBurner`, `IContainer`, `ILightable` | Do not also add ordinary `Container_*` or another lightable component. A portable censer may still use `Holdable` and a destroyable component. |
| `Smokeable` | `ILightable`, `ISmokeable` | Do not combine with torch, lamp, incense, or other lightable behaviour. |
| `Treatment` | `ITreatment` | Use one treatment component per item. A treatment component may be combined with a compatible `TopicalCream`. |
| `RepairKit` | `IRepairKit` | Use one repair-kit component per item. Do not stack multiple repair-kit components. |
| `Immobilising` | `IImmobilise`, `IWearable` | Splints and slings using `Limb_Immobilising` are wearable; do not also add unrelated wearable profiles. |
| `Crutch` | `ICrutch` | Crutches are held mobility aids; `Holdable` is appropriate. |
| `DragAid` | `IDragAid` | Stretchers, slings, sleds, and travois may be `Holdable`; do not add container behaviour unless the item truly stores contents. |

### 3.3 Fumigation fuel warning

`IncenseBurner` stores the drug on the burner component, not on each fuel item. A packet of fumigation fuel should usually be an inert item tagged:

```text
Functions / Material Functions / Medical Craft Stock / Fumigation Stock
```

That tag lets the fuel be accepted by a compatible incense burner. The fuel item’s public text should not promise a specific hidden drug effect unless it is deliberately paired with a specific burner item or setting. For v1, drug-bearing fumigation items should normally be the burner/censer/brazier itself.

---

## 4. Exact validated assets available for catalogue rows

### 4.1 Treatment components

Use these exact treatment components where appropriate:

| Component | Catalogue posture |
| --- | --- |
| `Bandage_Poor` | Crude or dirty wrapping. Use sparingly. |
| `Bandage_Simple` | Ordinary single-use bandage. |
| `Bandage_Good` | Good single-use bandage or dressing. |
| `Bandage_Great` | High-quality dressing, especially honeyed or carefully prepared. |
| `Clean_Single` | Single-use wound-cleaning cloth, swab, scraping cloth, vinegar cloth, or wash cloth. |
| `Clean_Kit` | Multi-use wound-cleaning kit. |
| `Antiseptic_Single` | Single-use antiseptic-style wound cleaning. Avoid modern “disinfectant” language. |
| `Antiseptic_Kit` | Multi-use honey/vinegar/wine-style antiseptic dressing kit. |
| `Tend_Single` | Single-use wound-tending compress or packing. |
| `Tend_Kit` | Multi-use tending kit. |
| `Suture_Single` | Single-use suture thread/needle stock. |
| `Suture_Kit` | Ordinary multi-use suturing kit. |
| `Suture_Kit_Good` | Higher-quality surgical or court/infirmary suturing kit. |
| `FieldMedkit` | Multipurpose field medkit; use for military/campaign chests or rolls only. |
| `Treatment_AntiInflammatory_Single` | Use cautiously; acceptable for poultice/compress style anti-inflammatory examples. |
| `Treatment_AntiInflammatory_Kit` | Use cautiously; acceptable for stocked poultice kit examples. |
| `Treatment_AdminUnlimited` | Administrative only. MUST NOT appear in normal player-facing medieval catalogue rows. |

### 4.2 Period-usable drug wrapper components

Use these exact components for strict medieval catalogue items:

| Vector/form | Component names |
| --- | --- |
| Ingested solid doses | `Pill_Willow_Bark_Tea`, `Pill_Mandrake_Draught`, `Pill_Mint_Infusion`, `Pill_Ephedra_Brew`, `Pill_Foxglove_Tincture`, `Pill_Poppy_Latex_Draught`, `Pill_Mint_and_Ginger_Tonic`, `Pill_Theriac_Electuary` |
| Ingested but not preferred | `Pill_Garlic_Salve` exists for vector compatibility, but the catalogue should normally present garlic as a salve rather than a swallowed “garlic salve” pill. |
| Touched/topical | `TopicalCream_Honey_Poultice`, `TopicalCream_Garlic_Salve`, `TopicalCream_Aloe_Burn_Salve`, `TopicalCream_Yarrow_Styptic`, `TopicalCream_Herbal_Burn_Salve`, `TopicalCream_Alum_Styptic` |
| Personal smoke | `Smokeable_Mandrake_Draught`, `Smokeable_Henbane_Smoke`, `Smokeable_Bronchial_Smoke`, `Smokeable_Soporific_Fumes` |
| Room-scale fumigation | `IncenseBurner_Mandrake_Draught`, `IncenseBurner_Henbane_Smoke`, `IncenseBurner_Bronchial_Smoke`, `IncenseBurner_Soporific_Fumes` |

The catalogue MUST NOT use the following late, modern, or non-baseline wrappers in the strict 500–1300 CE default set:

- `Pill_Laudanum`
- `Smokeable_Ether_Anaesthetic`
- `Pill_Digitalis_Tincture`
- `Pill_Distilled_Antiseptic`
- `TopicalCream_Distilled_Antiseptic`
- `TopicalCream_Curare_Paste`
- modern antibiotics, anaesthetics, bronchodilators, anticoagulants, immunosuppressants, overdose-reversal products, or healing accelerants
- `Smokeable_Cigar`, `Smokeable_Cigarillo`, and `Smokeable_PipeBowl` unless the setting explicitly includes medieval New World tobacco contexts outside the current culture brief

`Mould Poultice` remains excluded from the strict default catalogue because the mechanical antibiotic presentation is too strong for the baseline package. It may be a later permissive-history or fantasy toggle.

### 4.3 Medicinal liquids and medicine-vessel components

The current liquid export contains:

| Liquid | Use |
| --- | --- |
| `willow bark tea` | analgesic tea |
| `mandrake draught` | sedative/anaesthetic draught |
| `mint infusion` | nausea-control infusion |
| `ephedra brew` | stimulant brew |
| `foxglove tincture` | dangerous heart-support tincture |
| `poppy latex draught` | strong analgesic/sedative draught |
| `mint and ginger tonic` | stronger nausea-control tonic |
| `theriac syrup` | broad electuary/syrup presentation |

Use these exact vessel components:

| Component | Catalogue use |
| --- | --- |
| `LContainer_Medicine_Vial_30ml` | Empty/refillable small medicine vial. |
| `LContainer_Medicine_Bottle_100ml` | Empty/refillable small medicine bottle. |
| `LContainer_Medicine_Flask_250ml` | Empty/refillable medicine flask. |
| `LContainer_Medicine_Willow_Bark_Tea_250ml` | Ready-to-use willow-bark tea flask. |
| `LContainer_Medicine_Mandrake_Draught_100ml` | Ready-to-use mandrake draught bottle. |
| `LContainer_Medicine_Mint_Infusion_250ml` | Ready-to-use mint infusion flask. |
| `LContainer_Medicine_Ephedra_Brew_250ml` | Ready-to-use ephedra brew flask. |
| `LContainer_Medicine_Foxglove_Tincture_30ml` | Ready-to-use foxglove tincture vial. |
| `LContainer_Medicine_Poppy_Latex_Draught_100ml` | Ready-to-use poppy-latex draught bottle. |
| `LContainer_Medicine_Mint_and_Ginger_Tonic_250ml` | Ready-to-use mint-and-ginger tonic flask. |
| `LContainer_Medicine_Theriac_Syrup_100ml` | Ready-to-use theriac syrup bottle. |

These components are opaque, closable, refillable liquid containers. Their descriptions may say stoppered, sealed with wax, capped, or tied shut, but MUST NOT imply once-only opening or modern tamper-evident packaging.

The `weightInGrams` field for the item is the empty vessel’s inherent weight. Its cost should include the value of the default-loaded medicinal contents.

### 4.4 Mobility, transport, and prosthetic components

| Component family | Exact components | Catalogue use |
| --- | --- | --- |
| Crutch | `Crutch` | Walking crutch, staff-crutch, forked crutch. |
| Splint/sling | `Limb_Immobilising` | Splints, padded boards, arm slings, fracture bindings. |
| Casualty transport | `DragAid_Stretcher`, `DragAid_Sling`, `DragAid_Travois`, `DragAid_Harness`, `DragAid_Sled` | Stretchers, litters, sleds, travois, hauling harnesses. |
| Prosthetics | existing left/right arm, hand, leg, foot, eye, knee, wrist, ankle, and functional variants | Use a restrained medieval subset; non-functional or simple functional wooden/leather prosthetics should dominate. |

Prosthetic hands and feet should usually be plain wood, leather, bronze, iron, or strapwork. Use functional prosthetic components only when the public object actually represents a usable hook, peg, grip, or foot rather than an ornamental filler.

### 4.5 Repair-kit components

Common repair families available:

| Family | Components |
| --- | --- |
| Ceramic | `Repair_Ceramic_Poor`, `Repair_Ceramic`, `Repair_Ceramic_Good` |
| Cloth | `Repair_Cloth_Poor`, `Repair_Cloth`, `Repair_Cloth_Good` |
| Hard organic | `Repair_Hard_Organic_Poor`, `Repair_Hard_Organic`, `Repair_Hard_Organic_Good` |
| Leather | `Repair_Leather_Poor`, `Repair_Leather`, `Repair_Leather_Good` |
| Metal | `Repair_Metal_Poor`, `Repair_Metal`, `Repair_Metal_Good` |
| Metal armour | `Repair_Metal_Armour_Poor`, `Repair_Metal_Armour`, `Repair_Metal_Armour_Good` |
| Metal tools | `Repair_Metal_Tool_Poor`, `Repair_Metal_Tool`, `Repair_Metal_Tool_Good` |
| Metal weapons | `Repair_Metal_Weapon_Poor`, `Repair_Metal_Weapon`, `Repair_Metal_Weapon_Good` |
| Stone | `Repair_Stone_Poor`, `Repair_Stone`, `Repair_Stone_Good` |
| Wood | `Repair_Wood_Poor`, `Repair_Wood`, `Repair_Wood_Good` |
| Universal | `Repair_Universal_Poor`, `Repair_Universal`, `Repair_Universal_Good` |

Specialist repair families now available:

| Family | Components | Targeting rule |
| --- | --- | --- |
| Glass | `Repair_Glass_Poor`, `Repair_Glass`, `Repair_Glass_Good` | Repairs exact target materials `glass`, `silicate glass`, `soda-lime glass`, and `lead glass`. |
| Paper/parchment | `Repair_Paper_Poor`, `Repair_Paper`, `Repair_Paper_Good` | Repairs exact target materials `paper`, `parchment`, and `papyrus`. |
| Lacquerware | `Repair_Lacquer_Poor`, `Repair_Lacquer`, `Repair_Lacquer_Good` | Repairs exact target material `lacquer`. |
| Cordage | `Repair_Cordage_Poor`, `Repair_Cordage`, `Repair_Cordage_Good` | Repairs target items tagged `Functions / Repairing / Cordage`. |
| Composite bow | `Repair_Composite_Bow_Poor`, `Repair_Composite_Bow`, `Repair_Composite_Bow_Good` | Repairs target items tagged `Functions / Repairing / Composite Bow`. |

`Repair_Universal*` should not be used for ordinary medieval player-facing kits unless the item is deliberately broad, rare, or administrative.

Important tag distinction: the `Functions / Repairing / Cordage` and `Functions / Repairing / Composite Bow` tags are target-item tags for repairability. They are not automatically required on the repair kit itself. Repair kit items should usually carry market tags such as `Market / Repair Supplies / Specialist Repair Supplies` rather than the target leaf tag unless the kit itself is also a cordage or composite-bow object.

---

## 5. Tagging model

Use exact tag paths. Do not invent new leaves during catalogue authoring.

### 5.1 Medicine and treatment tags

| Tag | Use |
| --- | --- |
| `Market / Medicine / Simple Medicine` | Cheap household remedies, simple bandages, crude dressings. |
| `Market / Medicine / Standard Medicine` | Ordinary professional or town-market medicines and kits. |
| `Market / Medicine / High-Quality Medicine` | Court, monastery, hospital, elite apothecary, or high-grade prepared goods. |
| `Market / Medicine / Herbal Medicine` | Plant-derived remedies, salves, teas, smoke medicines, electuaries. |
| `Market / Medicine / Apothecary Goods` | Jars, vials, bottles, syrups, compound medicines, apothecary presentation goods. |
| `Market / Medicine / Treatment Supplies` | Bandages, compresses, splints, tending kits, cleaning supplies. |
| `Market / Medicine / Surgical Supplies` | Suture kits, probes, forceps, scalpels, cautery tools, surgical rolls. |
| `Market / Medicine / Prosthetics and Mobility` | Crutches, slings, stretchers, prosthetics, mobility aids. |
| `Functions / Medical Treatment / Bandage` | Physical bandage/dressing items. |
| `Functions / Medical Treatment / Splint` | Splints and fracture immobilisers. |
| `Functions / Medical Treatment / Tend Kit` | Tending and wound-packing items. |
| `Functions / Medical Treatment / Wound Cleaning` | Cleaning cloths, wash kits, scraping kits. |
| `Functions / Medical Treatment / Antiseptic Dressing` | Honey/vinegar/wine-style wound dressings; avoid modern disinfectant prose. |
| `Functions / Medical Treatment / Suture Kit` | Suturing supplies and kits. |
| `Functions / Medical Treatment / Prosthetic` | Prosthetic limbs and fitted artificial parts. |
| `Functions / Medical Treatment / Mobility Aid` | Crutches, slings, stretchers, litters, travois. |
| `Functions / Medical Treatment / Herbal Remedy` | Drug-bearing herbal items. |
| `Functions / Medical Treatment / Fumigation` | Censers, braziers, smoke bowls, fumigation goods. |
| `Functions / Medical Treatment / Styptic` | Yarrow, alum, pressure, and bleeding-control items. |
| `Functions / Medical Treatment / Antidote` | Theriac and poison-symptom remedy presentation. |

### 5.2 Medical craft-stock tags

Use these for raw stock, refill, or craft-input items. They are not necessary on every finished item.

- `Functions / Material Functions / Medical Craft Stock / Dressing Stock`
- `Functions / Material Functions / Medical Craft Stock / Poultice Stock`
- `Functions / Material Functions / Medical Craft Stock / Salve Stock`
- `Functions / Material Functions / Medical Craft Stock / Decoction Stock`
- `Functions / Material Functions / Medical Craft Stock / Fumigation Stock`
- `Functions / Material Functions / Medical Craft Stock / Suture Stock`
- `Functions / Material Functions / Medical Craft Stock / Splint Stock`
- `Functions / Material Functions / Medical Craft Stock / Prosthetic Stock`
- `Functions / Material Functions / Medical Craft Stock / Surgical Tool Blank`
- `Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock`

`Fumigation Stock` is mechanical for fuel acceptance by `IncenseBurner`.

### 5.3 Tool tags

Use these exact tool tags when applicable:

- `Functions / Tools / Medical Tools / Medicine Strainer`
- `Functions / Tools / Medical Tools / Ointment Spatula`
- `Functions / Tools / Medical Tools / Cupping Vessel`
- `Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle`
- `Functions / Tools / Surgical Tools / Arterial Clamp`
- `Functions / Tools / Surgical Tools / Bonesaw`
- `Functions / Tools / Surgical Tools / Cautery Iron`
- `Functions / Tools / Surgical Tools / Forceps`
- `Functions / Tools / Surgical Tools / Scalpel`
- `Functions / Tools / Surgical Tools / Surgical Probe`
- `Functions / Tools / Surgical Tools / Surgical Suture Needle`

Cupping vessels, cautery irons, leech jars, and bloodletting bowls may be present as tools or props, but do not claim unsupported automated treatment effects.

### 5.4 Repair tags

Market tags for repair supplies:

- `Market / Repair Supplies`
- `Market / Repair Supplies / General Repair Supplies`
- `Market / Repair Supplies / Specialist Repair Supplies`
- `Market / Repair Supplies / Weapon and Armour Repair Supplies`

Target-item repairability tags:

- `Functions / Repairing / Glass`
- `Functions / Repairing / Paper and Parchment`
- `Functions / Repairing / Lacquerware`
- `Functions / Repairing / Cordage`
- `Functions / Repairing / Composite Bow`

The catalogue should use target-item tags only when the item itself is intended to be repairable by those tag-restricted kits.

---

## 6. Materials and destroyable components

The primary material must be an exact seeded solid material. A liquid name is never valid as the primary material.

### 6.1 Recommended exact materials

| Use | Preferred exact materials |
| --- | --- |
| Bandages, dressings, wrappings | `linen`, `wool`, `cotton`, `hemp`, `silk` |
| Generic salves, poultices, electuaries | `honey`, `beeswax`, `herb`, `leaf`, `spice`, `resin`, `salt` |
| Named medicinal herb stock | `yarrow`, `mandrake`, `henbane`, `foxglove`, `ephedra` |
| Mineral styptic or mordant stock | `alum` |
| Suture/thread stock | `gut`, `sinew`, `silk`, `linen`, `hemp` |
| Splints and crutches | `willow`, `wood`, `oak`, `bamboo`, `reed` |
| Prosthetics | `wood`, `willow`, `oak`, `leather`, `bronze`, `wrought iron`, `bone`, `horn` |
| Medicine vessels | `ceramic`, `glazed ceramic`, `fired clay`, `glass`, `silicate glass`, `soda-lime glass`, `lead glass`, `horn`, `wood`, `leather` |
| Apothecary labels, packets, scrolls | `paper`, `parchment`, `papyrus`, `linen` |
| Censers and braziers | `bronze`, `wrought iron`, `ceramic`, `fired clay`, `stone` |
| Repair kits | `leather`, `wood`, `linen`, `bronze`, `wrought iron`, `sinew`, `resin`, `beeswax`, `gut` |
| Lacquerware repair targets | `lacquer` as the primary material if the item should be repairable by `Repair_Lacquer*` |
| Glass repair targets | `glass`, `silicate glass`, `soda-lime glass`, or `lead glass`; avoid modern `borosilicate glass` and `tempered glass` in medieval defaults. |
| Paper repair targets | `paper`, `parchment`, or `papyrus` |

The previously missing named medical materials are now part of the required core material set: `alum`, `ephedra`, `foxglove`, `gut`, `henbane`, `mandrake`, and `yarrow`. Catalogue rows should use those exact material names when the item is specifically that stock. Continue to use seeded abstractions such as `herb`, `leaf`, `spice`, or `sinew` only for generic or blended stock.

### 6.2 Destroyable choice guide

| Item material/form | Preferred destroyable component |
| --- | --- |
| Cloth pads, herb packets, salve pots, small pouches | `Destroyable_Misc` |
| Paper, parchment, papyrus packets or labels | `Destroyable_Paper` |
| Glass bottles, vials, cups, fragile jars | `Destroyable_Glassware` |
| Bronze, iron, heavy metal tools and censers | `Destroyable_HeavyMetal` |
| Heavy wooden crutches, stretchers, chests, repair boxes | `Destroyable_WoodenHeavy` |
| Large installed chests, infirmary furniture, heavy fixtures | `Destroyable_Furniture` |
| Weapons/armour repair examples only where item is actually weapon/armour | `Destroyable_Weapon`, `Destroyable_Armour` |

Every ordinary player-facing item should have one suitable destroyable component unless there is a specific reason not to.

---

## 7. Culture and regional variation model

The package remains shared-first. Most medical mechanics should be shared across cultures; regional flavour belongs in materials, vessels, presentation, labels, skins, and institutional context.

### 7.1 Culture families covered

Use the same broad medieval family model as the other package references:

- Early Anglo-Saxon / Insular
- Late Anglo-Saxon / Anglo-Danish
- Norse
- Norman / Anglo-Norman
- High Medieval Britain / Marcher
- Gaelic / Welsh / Highland
- Carolingian / Frankish
- Capetian / Low Countries
- German / HRE / Alpine-North Italian
- Iberian Christian
- Andalusi / Maghrebi
- Byzantine
- Abbasid / Persianate
- Fatimid Egypt / Ifriqiya
- Seljuk / Ayyubid / early Mamluk
- Rus / Novgorod
- Steppe Turkic / Cuman / Mongol-adjacent
- North Indian / Rajput
- South Indian / Chola
- Song China
- Goryeo Korea
- Heian / Kamakura Japan

These are authoring lenses, not labels that must appear in public item text.

### 7.2 When to create a separate regional prototype

A separate prototype is justified when at least one of these changes materially:

- attached component or delivery mechanism;
- default medicinal liquid;
- vessel capacity or reseal behaviour;
- primary material or repair target;
- silhouette, handling, or whether it is portable/fixed;
- fuel type or fumigation behaviour;
- institutional role, such as field roll versus infirmary chest;
- target craft, such as lacquer repair versus parchment repair;
- player-facing use category, such as household remedy versus professional surgical kit.

Use skins or description variants when only these change:

- local herb names;
- medical-theory language;
- decorative motifs;
- labels, script, seal, owner, maker, or institution;
- status markers;
- minor surface finish;
- precise assortment inside a kit that has the same mechanics.

### 7.3 Regional emphasis table

| Region/tradition | Emphasise | Usually shared |
| --- | --- | --- |
| Western and northern Europe | Monastic infirmary chests, barber-surgeon rolls, linen dressings, wool compresses, field splints, wine/vinegar cleaning, suture kits. | Bandage mechanics, cleaning/tending/suture components, willow/mint/poppy medicines. |
| Byzantine and eastern Christian | Ceramic and glass medicine jars, compound electuaries, hospital presentation, bronze tools, cautery and surgical-tool skins. | Core drug definitions, treatment components, medicine-vessel capacities. |
| Islamic Mediterranean, Near East, and North Africa | Apothecary goods, theriac, syrups, labelled jars, fine glass/ceramic bottles, fumigation burners, market pharmacy presentation. | Drug rows, liquids, vessel mechanics, topical mechanics. |
| South Asia | Medicated-oil style presentation, cotton dressings, bronze vessels, herbal powders, temple/court healer kits. | Analgesic, healing, nausea-control, splint, and repair mechanics. |
| Song/Goryeo/Heian-Kamakura East Asia | Paper herb packets, ceramic/porcelain-style jars using seeded ceramic/glass materials, lacquer cases, moxa/cautery as inert tools, paper/lacquer repair kits. | Core decoction, topical, bandage, mobility, and repair mechanics. |
| Steppe, Rus, caravan, and mounted contexts | Compact rolls, leather flasks, dried dose packets, horse/tack-oriented repair, cordage and composite-bow repair, travois. | Drug definitions, splints, wound kits, drag-aid components. |
| Naval and river travel | Sealed vessels, compact wound rolls, cordage, wood, cloth, and leather repair supplies. | Treatment components, medicine liquids, repair-kit mechanics. |

---

## 8. Catalogue grouping plan

The implemented catalogue authors rows in auditable branches and lands at **183 item prototypes**, within the target complete-catalogue range of roughly **180-240 item prototypes**.

| Branch | Target range | Contents |
| --- | ---: | --- |
| Shared wound consumables | 25–40 | Bandage rolls, compresses, dressings, wound cloths, styptic pads, burn dressings, suture singles. |
| Treatment kits and medical cases | 20–35 | Cleaning kits, tending rolls, suture kits, field medkits, infirmary chests, apothecary cases, veterinary kits. |
| Durable medical tools | 25–40 | Mortars, pestles, strainers, spatulas, probes, needles, forceps, scalpels, cautery irons, cupping vessels, leech jars. |
| Drug-bearing topical, oral, liquid, smoke, and fumigation items | 45–70 | Salves, poultices, styptics, electuaries, boluses, lozenges, medicine vessels, smokeables, censers. |
| Mobility, casualty transport, and prosthetics | 20–35 | Splints, slings, crutches, stretchers, litters, travois, peg legs, prosthetic hands/feet/eyes. |
| Common repair kits | 30–45 | Cloth, leather, wood, hard-organic, ceramic, stone, metal, armour, weapon, and tool repair kits. |
| Specialist repair kits | 20–35 | Glass, paper/parchment, lacquer, cordage, composite-bow repair kits and regionally appropriate variants. |
| Raw stock and refills | 10–25 | Dressing stock, poultice stock, salve stock, decoction stock, fumigation stock, suture stock, splint stock, repair stock. |

The catalogue does not need equal regional coverage in every branch. Drug vessels, apothecary goods, fumigation, lacquer repair, paper repair, and composite-bow repair deserve more regional specificity than ordinary bandages or crutches.

---

## 9. Component recipe patterns for catalogue authoring

These are patterns, not final rows.

### 9.1 Wound care and topical medicine

| Item form | Component pattern | Notes |
| --- | --- | --- |
| Plain bandage | `Holdable`, `Bandage_Simple` or `Bandage_Good`, destroyable | Add `Functions / Medical Treatment / Bandage` and `Market / Medicine / Treatment Supplies`. |
| Crude bandage | `Holdable`, `Bandage_Poor`, destroyable | Use for field scraps, dirty cloths, low-status or emergency goods. |
| Honeyed dressing | `Holdable`, `Bandage_Great`, `TopicalCream_Honey_Poultice`, destroyable | Valid treatment-plus-topical stack. Avoid modern antiseptic claims. |
| Yarrow styptic pad | `Holdable`, `Bandage_Good`, `TopicalCream_Yarrow_Styptic`, destroyable | Add `Functions / Medical Treatment / Styptic`. |
| Alum styptic pad or block | `Holdable`, `Bandage_Good` or `TopicalCream_Alum_Styptic`, destroyable | Use exact `alum` material where the visible item identity is alum; use linen or container material for pads and vessels. |
| Aloe or herbal burn dressing | `Holdable`, `Bandage_Good`, `TopicalCream_Aloe_Burn_Salve` or `TopicalCream_Herbal_Burn_Salve`, destroyable | Good for South Asian, Islamicate, Byzantine, and general apothecary contexts. |
| Garlic salve pot | `Holdable`, `TopicalCream_Garlic_Salve`, destroyable | Usually material `ceramic`, `fired clay`, `beeswax`, or `honey`. |
| Cleaning cloth | `Holdable`, `Clean_Single`, destroyable | May mention vinegar, wine, clean water, or scraping in prose. |
| Cleaning kit | `Holdable`, `Clean_Kit`, optional dry `Container_*`, destroyable | Do not add liquid-container unless it is a dedicated vessel item. |
| Suture spool or packet | `Holdable`, `Suture_Single`, destroyable | Use exact `gut` where the visible item identity is cleaned gut suture; use `sinew`, `silk`, `linen`, or `hemp` for other suture stock. |
| Suture kit | `Holdable`, `Suture_Kit` or `Suture_Kit_Good`, optional dry `Container_Pouch`, destroyable | Bronze/bone needles may be visible in prose. |

### 9.2 Swallowed medicines and medicinal vessels

| Item form | Component pattern | Notes |
| --- | --- | --- |
| Electuary ball / bolus / lozenge | `Holdable`, relevant `Pill_*`, `Destroyable_Misc` | Do not call it a tablet or capsule. |
| Theriac electuary | `Holdable`, `Pill_Theriac_Electuary`, `Destroyable_Misc` | Add `Functions / Medical Treatment / Antidote` and apothecary/herbal medicine tags. |
| Empty medicine vial | `Holdable`, `LContainer_Medicine_Vial_30ml`, suitable destroyable | Primary material may be glass, ceramic, horn, wood, or leather as appropriate. |
| Empty medicine bottle/flask | `Holdable`, `LContainer_Medicine_Bottle_100ml` or `LContainer_Medicine_Flask_250ml`, destroyable | Good for refillable apothecary or infirmary vessels. |
| Ready-to-use tea/draught/tincture/flask | `Holdable`, one default-loaded `LContainer_Medicine_*`, destroyable | Do not add a second liquid container. Cost includes contents. |
| Dangerous tincture vial | `Holdable`, `LContainer_Medicine_Foxglove_Tincture_30ml`, destroyable | Prose should signal careful measure or warning marks without omniscient pharmacology. |

### 9.3 Smoke and fumigation

| Item form | Component pattern | Notes |
| --- | --- | --- |
| Personal smoke cone or roll | `Holdable`, relevant `Smokeable_*`, `Destroyable_Misc` | The user directly smokes it. Do not add `IncenseBurner`. |
| Prepared fumigation censer/brazier | `Holdable`, relevant `IncenseBurner_*`, suitable destroyable | The burner carries the drug. Do not add a dry `Container_*` or second lightable component. |
| Fumigation fuel packet | `Holdable`, `Destroyable_Misc`; tag `Functions / Material Functions / Medical Craft Stock / Fumigation Stock` | Fuel is accepted by burners but does not itself carry the drug. Keep public text generic unless intentionally paired. |
| Infirmary/temple fumigation bowl | `IncenseBurner_Bronchial_Smoke`, `IncenseBurner_Henbane_Smoke`, or `IncenseBurner_Soporific_Fumes` | Use source-room dosing only. Avoid hidden drug claims in visible prose. |

### 9.4 Mobility and casualty transport

| Item form | Component pattern | Notes |
| --- | --- | --- |
| Splints | `Holdable`, `Limb_Immobilising`, destroyable | Padded wood/reed/willow; add splint/treatment tags. |
| Arm sling | `Holdable`, `Limb_Immobilising`, destroyable | Textile material; add mobility and splint tags. |
| Crutch | `Holdable`, `Crutch`, wood destroyable | May have forked, padded, or staff-like forms. |
| Stretcher/litter | `Holdable`, `DragAid_Stretcher`, destroyable | Large/normal-to-large item; material linen/wood. |
| Drag sling | `Holdable`, `DragAid_Sling`, destroyable | Smaller casualty-dragging variant. |
| Travois | `Holdable`, `DragAid_Travois`, wood destroyable | Steppe, frontier, campaign, or rough-travel contexts. |
| Prosthetic | `Holdable`, one `Prosthetic_*`, destroyable | Use restrained period-credible prosthetic forms; avoid modern sockets or materials. |

### 9.5 Repair kits

| Item form | Component pattern | Notes |
| --- | --- | --- |
| Cloth repair kit | `Holdable`, one `Repair_Cloth*`, optional `Container_Pouch`, destroyable | Needles, thread, patches, awls. |
| Leather repair kit | `Holdable`, one `Repair_Leather*`, optional `Container_Pouch`, destroyable | Awls, thonging, waxed thread, patches. |
| Wood repair kit | `Holdable`, one `Repair_Wood*`, optional `Container_Pouch` or `Container_Trunk`, destroyable | Pegs, wedges, glue, clamps, carving tools. |
| Hard-organic repair kit | `Holdable`, one `Repair_Hard_Organic*`, destroyable | Bone, horn, antler, shell style repairs. |
| Metal tool/weapon/armour repair kit | `Holdable`, matching `Repair_Metal_*`, destroyable | Use `Market / Repair Supplies / Weapon and Armour Repair Supplies` for weapon/armour kits. |
| Glass repair kit | `Holdable`, one `Repair_Glass*`, destroyable | Lead came, resin, cloth pads, polishing powder, spare panes. |
| Paper/parchment repair kit | `Holdable`, one `Repair_Paper*`, optional document container, `Destroyable_Paper` or `Destroyable_Misc` | Patches, paste, smoothing bone, thread. |
| Lacquer repair kit | `Holdable`, one `Repair_Lacquer*`, destroyable | Lacquer, polishing cloth, resin, fine brush. |
| Cordage repair kit | `Holdable`, one `Repair_Cordage*`, destroyable | Splicing fid, hemp, wax, tarred line. |
| Composite-bow repair kit | `Holdable`, one `Repair_Composite_Bow*`, destroyable | Sinew, horn shims, glue/resin, clamps, wraps. |

---

## 10. Description and terminology rules

Medieval medical items should be sensory and inspectable. Describe visible construction, material, stain, smell, labels, wrapping, closures, wear, and handling.

Avoid modern clinical language in public descriptions:

- Use “clean”, “washed”, “boiled”, “vinegar-sharp”, “honeyed”, “medicated”, “herbal”, “bitter”, “resinous”, “fumigating”, or “carefully measured”.
- Avoid “sterile”, “antibiotic”, “disinfectant”, “anaesthesiology”, “tablet”, “capsule”, “dosage form”, “aseptic”, “clinical”, and “pharmaceutical-grade”.
- Do not state hidden mechanics such as exact drug grams, drug vectors, pulse intervals, or wound treatment bonuses.
- Dangerous drugs should be signalled through small measured vessels, warning knots, sealed lids, bitter odour, cautionary labels, or careful packaging rather than omniscient claims.
- A description may say an item is “used by healers for...” but should not guarantee modern efficacy.

Writing blocks are optional. Use them only when a label, seal, script, or maker’s mark should be readable by language/script skill. Do not assume a universal medieval language/script set unless that package has already been seeded for the setting.

---

## 11. Size, weight, quality, and cost guidance

Costs are in farthings. These are starting ranges used by catalogue authoring, not hard economy balance.

| Item class | Typical size | Typical empty weight | Typical quality | Cost guidance |
| --- | --- | ---: | --- | ---: |
| Single cloth, swab, small pad | `VerySmall` | 20–100g | `Poor`–`Good` | 1–8m |
| Bandage roll / compress / dressing packet | `Small` | 80–250g | `Standard`–`Great` | 3–20m |
| Suture packet / styptic packet / bolus packet | `Tiny`–`VerySmall` | 5–80g | `Standard`–`Good` | 3–40m |
| Salve pot / poultice pot / small jar | `VerySmall`–`Small` | 80–350g | `Standard`–`Good` | 8–60m |
| Medicine vial/bottle/flask | `VerySmall`–`Small` | 40–450g empty vessel weight | `Standard`–`Good` | 8–120m depending on contents |
| Treatment roll / suture kit / cleaning kit | `Small`–`Normal` | 250–1500g | `Standard`–`Good` | 16–120m |
| Field medkit / infirmary chest | `Normal`–`Large` | 2–15kg | `Standard`–`VeryGood` | 80–400m |
| Crutch / splints / sling | `Small`–`Normal` | 150–1500g | `Standard` | 4–50m |
| Stretcher / litter / travois | `Normal`–`Large` | 2–10kg | `Standard` | 20–160m |
| Simple prosthetic | `Small`–`Normal` | 200–2500g | `Poor`–`Good` | 20–200m |
| Repair pouch/roll | `Small`–`Normal` | 300–2500g | `Poor`–`Good` | 12–160m |
| Specialist repair chest | `Normal`–`Large` | 2–12kg | `Standard`–`VeryGood` | 60–360m |

For default-loaded medicine vessels, the `weightInGrams` field is the vessel’s empty inherent weight. The component-loaded liquid adds runtime contents separately. Cost should include vessel plus medicine.

---

## 12. Catalogue exclusions and anti-patterns

The item catalogue MUST avoid these unless the user explicitly changes scope:

- New data-seeding tasks for drugs, liquids, components, component types, tags, or materials.
- Modern syringes, IV bags, blood bags, defibrillators, inhalers, oxygen gear, electronic diagnostic equipment, trauma shears, disposable plastic supplies, blister packs, and modern sterile dressings.
- Late/modern drug wrappers such as laudanum, ether, standardized digitalis, distilled antiseptic, curare, antibiotics, modern bronchodilators, opioids, anticoagulants, immunosuppressants, and healing accelerants.
- `Treatment_AdminUnlimited` in player-facing items.
- `Repair_Universal*` as ordinary baseline medieval kits.
- Tobacco smoking items unless the setting explicitly supports a medieval New World context.
- Descriptions claiming unsupported mechanics for bloodletting, leeching, cupping, moxa, cautery, or antidote administration.
- Multiple components that provide the same exclusive interface.
- Liquid names as primary solid materials.
- Unseeded material names.
- Region-by-region duplication of ordinary bandages, compresses, and simple kits when a skin would suffice.

---

## 13. Row-order used for catalogue implementation

Rows are authored in this order for easier review:

1. Raw stock/refill items and simple inert supplies.
2. Single-use wound consumables.
3. Treatment kits and medical rolls.
4. Durable medical tools.
5. Drug-bearing topical items.
6. Swallowed solid-dose items.
7. Empty and filled medicinal vessels.
8. Smokeable and fumigation items, including generic fuel stock.
9. Mobility aids and casualty transport.
10. Prosthetics.
11. Common repair kits.
12. Specialist repair kits.
13. Any fixed or large institutional chests, shelves, or infirmary fixtures.

Within each branch, author shared/base items first, then regional or status variants.

---

## 14. Acceptance checklist for catalogue rows

The final catalogue was checked so that every row satisfies all of the following:

- `uniqueReference` is lowercase snake case and uses the correct prefix.
- `noun`, `sdesc`, `ldesc`, and `fdesc` follow the authoring guidelines.
- The item’s material is exact and appears in `Seeded_Materials.json`.
- Every tag is exact and appears in `SeededTagHierarchy.csv`.
- Every component is exact and appears in `Seeded_Item_Components.json`.
- Portable items include `Holdable` unless there is a deliberate reason not to.
- Every ordinary item has one suitable destroyable component.
- Component combinations do not violate exclusive interfaces.
- Drug-bearing prose matches the attached delivery component or default-loaded liquid container.
- Fumigation fuel text does not claim mechanics that only exist on `IncenseBurner` components.
- Repair kit items use only one `RepairKit` component.
- Cordage and composite-bow target tags are used deliberately and not sprayed onto unrelated repair kits.
- Filled medicine vessel cost includes contents, but item weight remains inherent vessel weight.
- No strict-period excluded drugs or modern components appear.
- Regional variants change something substantive or are deferred to skins.

---

## 15. Implementation readiness statement

The package has passed the item-catalogue authoring pass. There are no blocking gaps in liquids, tags, component types, item components, or materials. The material gap review was resolved by promoting `alum`, `ephedra`, `foxglove`, `gut`, `henbane`, `mandrake`, and `yarrow` into core seeded materials, so the implemented catalogue uses those exact names rather than substituting broader abstractions where a named medicinal stock is intended. The catalogue is written as ordinary `CreateItem(...)` rows using the exact assets listed above, with no further dependency work assumed.

---

## Finished Catalogue Reference

## 1. Catalogue contract

The catalogue is written as ordinary portable item prototypes for the medieval item seeder. Unless a row states otherwise, use `ldesc: null`, `hideFromPlayers: false`, and no morph or destroyed-item reference. Finished goods are skinnable; raw stock and refill materials are marked as not skinnable in the catalogue metadata.

The catalogue assumes the following exact medical materials are available in core data and should be used directly: `alum`, `ephedra`, `foxglove`, `gut`, `henbane`, `mandrake`, and `yarrow`. These are visible item identities and should not be substituted with generic `herb`, `leaf`, `spice`, `sinew`, or `salt` when a row explicitly names them.

Drug-bearing items use the already-seeded runtime delivery mechanisms: `Pill_*` for swallowed solid doses, `TopicalCream_*` for topical salves and medicated dressings, `LContainer_Medicine_*` for drinkable liquid remedies, `Smokeable_*` for personal smoke, and `IncenseBurner_*` for room-scale fumigation. Fumigation fuel packets are inert stock accepted by burners; the burner component carries the drug effect.

Repair-kit rows use exactly one `Repair_*` component. Cordage and composite-bow repairability tags remain target-item tags and are not automatically applied to the repair kits themselves.

## 2. Catalogue fields

Each catalogue row below provides enough information to implement a `CreateItem(...)` row: unique reference, noun, short description, size, quality, inherent weight, base cost, primary material, tags, components, skinnability, and full description.

Column notes:

- **Specs** are `Size / Quality / weight grams / cost farthings / skin`.
- **Tags** and **Components** are pipe-delimited exact seeded names.
- **FDesc** is the intended full description text.

## 3. Main catalogue

### 3.1 Raw stock and refills

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_medical_clean_linen_dressing_stock` | bundle | a bundle of clean linen strips | Small / Standard / 220g / 5m / no skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Material Functions / Medical Craft Stock / Dressing Stock \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc | A tight bundle of narrow linen strips is tied with plain cord. The cloth has been washed, dried, and folded with the ends aligned for quick tearing into dressings. It looks like stock for a healer rather than ordinary rag cloth. |
| `medieval_medical_wool_wound_packing_stock` | bundle | a bundle of clean wool packing | Small / Standard / 180g / 4m / no skin | `wool` | Market / Medicine / Treatment Supplies \| Functions / Material Functions / Medical Craft Stock / Dressing Stock \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc | This bundle holds soft locks of pale wool wrapped inside a linen cloth. The wool is combed loose and springy, suitable for pressure packing or padding heavier dressings. A faint sheepy smell remains under the cleaner cloth scent. |
| `medieval_medical_cotton_dressing_stack` | stack | a stack of cotton dressing squares | Small / Good / 140g / 8m / no skin | `cotton` | Market / Medicine / Treatment Supplies \| Functions / Material Functions / Medical Craft Stock / Dressing Stock \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc | Flat cotton squares are stacked into a neat packet and tied through the middle. The fabric is softer than plain linen and cut to an even size for covering grazes, burns, or salved wounds. The edges show a little fraying from knife-cut preparation. |
| `medieval_medical_honeyed_poultice_cloth_stock` | packet | a sticky packet of honeyed cloths | VerySmall / Good / 160g / 12m / no skin | `honey` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Poultice Stock \| Functions / Medical Treatment / Antiseptic Dressing | Holdable \| Destroyable_Misc | Several folded cloths sit inside a waxed wrap, their fibres darkened by honey. The packet smells sweet and faintly floral, with enough tackiness that the folds pull apart reluctantly. It is clearly prepared as dressing stock rather than table linen. |
| `medieval_medical_vinegar_cleaning_cloth_bundle` | bundle | a bundle of vinegar-sharp cloths | VerySmall / Standard / 120g / 6m / no skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc | Small linen cloths are wrapped in a waxed outer rag to keep their sharp vinegar smell from fading. The cloth is damp but not dripping, and several corners are folded outward for quick grip. It is meant for wiping dirt, old blood, and crust from wounds. |
| `medieval_drug_willow_bark_bundle` | bundle | a bundle of shaved willow bark | VerySmall / Standard / 90g / 5m / no skin | `willow` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Decoction Stock \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc | Thin strips of willow bark are shaved into curling ribbons and tied with hemp thread. The inner bark shows pale tan fibres with a dry, faintly bitter smell. It is ready to steep, grind, or portion into simple medicine stock. |
| `medieval_drug_yarrow_styptic_bundle` | bundle | a bundle of dried yarrow heads | VerySmall / Standard / 60g / 6m / no skin | `yarrow` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Medical Treatment / Styptic | Holdable \| Destroyable_Misc | Small dried yarrow heads and feathered leaves are bundled with a thin linen tie. The herb has faded to dusty green and straw, but the bitter green scent is still present when handled. It is suitable for crushing into pads, powders, or styptic dressings. |
| `medieval_drug_mandrake_root_packet` | packet | a packet of cut mandrake root | Tiny / Good / 45g / 24m / no skin | `mandrake` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Material Functions / Medical Craft Stock / Decoction Stock | Holdable \| Destroyable_Paper | Pieces of pale mandrake root lie in a small parchment packet closed with thread. The roots are irregular, tough, and earthy, with a sharp bitter odour that clings to the paper. A red warning knot marks the packet as something to measure carefully. |
| `medieval_drug_henbane_leaf_packet` | packet | a packet of dried henbane leaves | Tiny / Good / 35g / 22m / no skin | `henbane` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Material Functions / Medical Craft Stock / Fumigation Stock | Holdable \| Destroyable_Paper | Dried henbane leaves are packed into a narrow paper fold, dark and brittle against the pale wrapping. The packet smells musty and acrid, and a black tie marks it apart from ordinary kitchen herbs. It is prepared for smoke or fumigation rather than cooking. |
| `medieval_drug_ephedra_twig_packet` | packet | a packet of dried ephedra twigs | Tiny / Standard / 50g / 14m / no skin | `ephedra` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Material Functions / Medical Craft Stock / Decoction Stock | Holdable \| Destroyable_Misc | Jointed green-brown twigs are bundled inside a small linen packet. The pieces are trimmed to a length that can be measured out by handful or pinch. The smell is dry, grassy, and faintly resinous. |
| `medieval_drug_foxglove_leaf_packet` | packet | a warning-tied foxglove packet | Tiny / Good / 30g / 28m / no skin | `foxglove` | Market / Medicine / Apothecary Goods \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper | Several dark, crinkled foxglove leaves sit in a waxed paper packet bound with a red thread. The packet is small and carefully folded, as if meant for apothecary hands rather than common kitchen use. The dry leaves have a dusty, green-bitter smell. |
| `medieval_drug_alum_styptic_powder_packet` | packet | a packet of pale alum powder | Tiny / Standard / 40g / 10m / no skin | `alum` | Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Styptic | Holdable \| Destroyable_Paper | A small parchment packet holds pale mineral powder that clumps slightly where it has drawn damp from the air. The fold is tight and clean, with a faint chalky dust along the crease. It is sized for careful pinches rather than bulk trade. |
| `medieval_drug_poppy_latex_resin_cake` | cake | a dark poppy-latex resin cake | Tiny / Good / 35g / 40m / no skin | `resin` | Market / Medicine / Apothecary Goods \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc | This small resinous cake is wrapped in oiled cloth, dark brown and tacky at the edges. It has a heavy bitter smell and has been pressed into a flat disk for shaving tiny amounts. The careful wrapping suggests valued medicine rather than ordinary spice. |
| `medieval_drug_garlic_salve_herb_packet` | packet | a pungent garlic-salve packet | VerySmall / Standard / 80g / 5m / no skin | `garlic` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Salve Stock \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc | Crushed garlic and dry salve herbs are folded into a thick linen packet. The smell is strong, sharp, and savoury enough to cut through the beeswax around it. It is plainly a healer’s stock packet, not a kitchen bundle. |
| `medieval_drug_mint_ginger_decoction_packet` | packet | a mint-and-ginger decoction packet | Tiny / Standard / 55g / 8m / no skin | `mint` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Decoction Stock \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper | Dried mint leaves and sliced ginger are mixed in a neat paper packet. The packet smells bright, warming, and sweet-sharp, with the ingredients visible through the loose fold at the top. It is ready to steep into a small pot of tonic. |
| `medieval_drug_theriac_spice_electuary_stock` | jar | a jar of theriac spice stock | VerySmall / Good / 220g / 70m / no skin | `spice` | Market / Medicine / Apothecary Goods \| Functions / Material Functions / Medical Craft Stock / Herbal Remedy Stock \| Functions / Medical Treatment / Antidote | Holdable \| Destroyable_Misc | A small glazed jar contains a dark, sticky blend of honey, spice, and powdered medicine stock. The lid is cloth-capped and sealed around the rim with beeswax. Its smell is rich, bitter, sweet, and resinous all at once. |
| `medieval_medical_gut_suture_hank` | hank | a hank of cleaned gut suture | Tiny / Standard / 35g / 10m / no skin | `gut` | Market / Medicine / Surgical Supplies \| Functions / Material Functions / Medical Craft Stock / Suture Stock \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Single | Cleaned gut thread is wound into a careful hank and tied to a small bone toggle. The strands are pale, slightly translucent, and uneven in thickness, but twisted firmly enough for coarse surgical use. A linen wrap keeps the thread from tangling. |
| `medieval_medical_willow_splint_blank_bundle` | bundle | a bundle of willow splint blanks | Normal / Standard / 850g / 8m / no skin | `willow` | Market / Medicine / Treatment Supplies \| Functions / Material Functions / Medical Craft Stock / Splint Stock \| Functions / Medical Treatment / Splint | Holdable \| Destroyable_WoodenHeavy | Straight pieces of willow are shaved smooth and tied into a narrow bundle. Each strip is drilled near the ends for future cord ties, but none have padding yet. They are raw stock for fracture splints rather than finished treatment gear. |

### 3.2 Single-use wound consumables

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_medical_rough_linen_bandage` | bandage | a rough linen bandage | VerySmall / Poor / 55g / 2m / skin | `linen` | Market / Medicine / Simple Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Poor | This strip of linen has been torn rather than cut, leaving frayed edges and a few stubborn creases. It is serviceable enough for emergency binding but plainly not fine infirmary stock. Small knots in the weave make it feel coarse under the fingers. |
| `medieval_medical_clean_linen_bandage` | bandage | a clean linen bandage | VerySmall / Standard / 80g / 4m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Simple | A narrow length of clean linen is rolled around itself into a compact bandage. The cloth is plain, pale, and closely woven, with enough length for wrapping an arm, hand, or smaller wound. Its loose end is tucked neatly under the roll. |
| `medieval_medical_narrow_linen_bandage_roll` | roll | a narrow linen bandage roll | Small / Good / 140g / 8m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Good | This long, narrow bandage is rolled tight around a small reed core. The linen has been cut evenly and pressed flat, with only a few loose threads along one edge. It is suited to controlled wrapping rather than hurried field tearing. |
| `medieval_medical_wool_pressure_compress` | compress | a dense wool pressure compress | Small / Standard / 130g / 5m / skin | `wool` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Single | Clean wool is packed into a thick pad and held inside a simple linen cover. The compress is springy but firm, made to be pressed hard over a bleeding or bruised place. Its corners are tied down so the wool does not spill loose. |
| `medieval_medical_cotton_wound_compress` | compress | a soft cotton wound compress | Small / Good / 95g / 7m / skin | `cotton` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Single | Several layers of cotton are folded into a soft square and lightly stitched at the edges. The pad is clean, pale, and more delicate than a wool compress, with enough body to absorb without feeling bulky. It has the look of careful apothecary preparation. |
| `medieval_medical_silk_wound_dressing` | dressing | a fine silk wound dressing | VerySmall / Great / 50g / 24m / skin | `silk` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Great | Fine silk is folded around a small inner pad and tied with two narrow threads. The fabric is smooth, light, and closely woven, making the dressing seem more courtly than field-made. It is compact enough to tuck into a physician’s case. |
| `medieval_medical_honeyed_linen_dressing` | dressing | a honeyed linen wound dressing | VerySmall / Good / 90g / 14m / skin | `linen` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Antiseptic Dressing \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Great \| TopicalCream_Honey_Poultice | This folded dressing is darkened with honey and pressed into a tidy, sticky pad. A waxed wrap keeps it from smearing the healer’s pouch, and the sweet smell is obvious as soon as it is handled. The linen underneath remains visible at the corners. |
| `medieval_medical_vinegar_wound_cloth` | cloth | a vinegar-soaked wound cloth | Tiny / Standard / 45g / 4m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Clean_Single | A small square of linen is damp with sharp vinegar and folded inside a waxed scrap. The cloth is meant for wiping rather than wrapping, with one corner left dry enough to grip. It smells clean in a sour, practical way. |
| `medieval_medical_wine_rinsed_wound_cloth` | cloth | a wine-rinsed wound cloth | Tiny / Standard / 50g / 5m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Clean_Single | This folded cloth is stained faint rose-brown from watered wine. It is wrapped in plain linen and smells of sour fruit and old cask, not perfume. The preparation is tidy enough for a travelling healer’s roll. |
| `medieval_medical_yarrow_styptic_pad` | pad | a yarrow-stuffed styptic pad | VerySmall / Standard / 55g / 9m / skin | `yarrow` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Styptic \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Yarrow_Styptic | Crushed yarrow is packed between two small linen faces, making a green-bitter pad meant to be pressed down hard. Flecks of herb show through the weave near the seams. A simple thread tie keeps the pad from opening in a pouch. |
| `medieval_medical_alum_styptic_pad` | pad | an alum-dusted styptic pad | VerySmall / Standard / 50g / 10m / skin | `alum` | Market / Medicine / Apothecary Goods \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Alum_Styptic | A small folded pad has been dusted with pale alum powder and wrapped to keep the mineral from spilling. The cloth feels dry and chalky through the outer fold. It is made for quick pressure on a cut rather than broad bandaging. |
| `medieval_medical_aloe_burn_dressing` | dressing | an aloe-slick burn dressing | VerySmall / Good / 85g / 15m / skin | `linen` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Aloe_Burn_Salve | This dressing is folded around a cool, slick herbal salve and sealed in waxed cloth. The surface has a greenish stain where the medicine has soaked into the linen. It is shaped for laying flat over a burned patch of skin. |
| `medieval_medical_herbal_burn_dressing` | dressing | a green herbal burn dressing | VerySmall / Good / 90g / 16m / skin | `herb` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Herbal_Burn_Salve | A neat linen pad is filled with a soft green salve and folded into a waxed wrapper. Its smell is grassy, bitter, and faintly resinous. The cloth is broad enough to cover a palm-sized burn without being bulky. |
| `medieval_medical_garlic_salve_cloth` | cloth | a garlic-salve wound cloth | Tiny / Standard / 45g / 7m / skin | `garlic` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Garlic_Salve | This small wound cloth has been smeared with a pungent garlic salve and folded under a beeswaxed cover. The smell is strong enough to announce it before the packet is opened. It is intended for laying over a small wound rather than wrapping a limb. |
| `medieval_medical_wound_packing_wad` | wad | a linen wound-packing wad | VerySmall / Standard / 70g / 4m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Single | Strips of linen are folded and twisted into a soft wad that can be pushed into an awkward wound. The cloth is plain and unmedicated, with the fibres left loose enough to swell when damp. It is wrapped in another scrap to keep it clean. |
| `medieval_medical_field_bandage_packet` | packet | a field bandage packet | Small / Standard / 180g / 8m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Bandage_Simple \| Tend_Single | A rough leather tie holds together a bandage, a small compress, and two short binding cords. The packet is compact and plain, made to fit a belt pouch or saddlebag. Dust marks on the outer cloth suggest hard campaign use. |
| `medieval_medical_monastic_dressing_packet` | packet | a monastic dressing packet | Small / Good / 170g / 12m / skin | `linen` | Market / Medicine / Treatment Supplies \| Market / Medicine / Standard Medicine \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Bandage_Good \| Clean_Single | Several careful dressings are folded inside a plain white linen cover marked with a small cross in brown thread. The packet is orderly rather than ornamental, with each pad stacked squarely. It smells faintly of honey, vinegar, and stored cloth. |
| `medieval_medical_apothecary_folded_dressing` | dressing | an apothecary folded dressing | VerySmall / Good / 75g / 13m / skin | `linen` | Market / Medicine / Apothecary Goods \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Honey_Poultice | This dressing is folded into a crisp square and held by a small paper band. Herbal flecks and a faint resin smell mark it as something prepared behind an apothecary counter. The cloth is clean, even, and easy to open one-handed. |
| `medieval_medical_horse_wound_bandage` | bandage | a broad horse wound bandage | Small / Standard / 260g / 10m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Good | A broad strip of strong linen is rolled around a stout wooden pin. It is wider and heavier than human dressings, with enough length to bind a leg or flank on a large animal. The cloth is plain but tough, made for stable use. |
| `medieval_medical_plastered_poultice_pad` | pad | a plastered herbal poultice pad | Small / Standard / 140g / 10m / skin | `herb` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Tend_Single \| TopicalCream_Honey_Poultice | A thick pad of cloth is plastered with a green-brown herbal paste and folded in on itself. The paste has dried enough not to drip, but it remains pliant under the fingers. The smell is earthy, sour, and medicinal. |
| `medieval_medical_linen_suture_packet` | packet | a packet of linen sutures | Tiny / Standard / 25g / 6m / skin | `linen` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Single | Waxed linen thread is wound onto a small slip of wood and tucked inside a folded cloth. The thread is coarse but clean, with a needle-hole awl mark beside it for quick threading. It is a simple single-use surgical supply. |
| `medieval_medical_silk_suture_card` | card | a card of fine silk sutures | Tiny / Good / 18g / 18m / skin | `silk` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Single | Fine silk thread is wrapped around a thin parchment card and secured with a careful cross-tie. The thread is smoother and more regular than ordinary linen suture, with a soft sheen in the light. A small needle is tucked under the last loop. |
| `medieval_medical_gut_suture_spool` | spool | a small gut suture spool | Tiny / Standard / 40g / 14m / skin | `gut` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Single | Clean gut thread is wound around a little bone spool with care. The strand is pale, strong-looking, and slightly uneven, the sort of material used when a healer wants thread that belongs in the body rather than ordinary stitching. A linen cover protects the spool. |
| `medieval_medical_bronze_suture_needle` | needle | a curved bronze suture needle | Tiny / Standard / 12g / 8m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit \| Functions / Tools / Surgical Tools / Surgical Suture Needle | Holdable \| Destroyable_HeavyMetal \| Suture_Single | This small bronze needle has a smooth curve, a drilled eye, and a polished point. It is thicker than a seamstress needle and shaped for gripping with forceps or fingers. The metal has darkened slightly where it rests in its cloth wrap. |
| `medieval_medical_bone_suture_needle` | needle | a polished bone suture needle | Tiny / Standard / 10g / 5m / skin | `bone` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit \| Functions / Tools / Surgical Tools / Surgical Suture Needle | Holdable \| Destroyable_Misc \| Suture_Single | A small curved needle has been carved from bone and rubbed smooth. The eye is broad and the point is serviceable rather than delicate, suited to coarse stitching. It rests inside a narrow scrap of leather to protect the tip. |
| `medieval_medical_cautery_dressing_pad` | pad | a thick cautery dressing pad | Small / Standard / 150g / 6m / skin | `wool` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Single | This heavy pad is built from wool wrapped in linen, with darkened scorch marks along one face. It is meant to follow hot-iron work, not to serve as an ordinary bandage. The bulk and smoky smell make its purpose visible. |

### 3.3 Treatment kits and cases

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_medical_wound_cleaning_kit` | kit | a wound-cleaning kit | Small / Standard / 520g / 18m / skin | `leather` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Clean_Kit \| Container_Pouch | This compact kit holds folded cloths, a blunt scraper, and a small spatula inside a leather roll. The pieces are plain but tidy, meant for removing dirt and old blood before heavier treatment begins. The roll ties closed with two waxed cords. |
| `medieval_medical_vinegar_wash_kit` | kit | a vinegar wound-wash kit | Small / Standard / 620g / 22m / skin | `leather` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Clean_Kit | A leather pouch holds waxed cloths, wiping swabs, and space for a small sour-smelling wash vessel. Its outer flap is stained where damp cloth has rested against it. The kit is practical and compact, designed for repeated cleaning rather than display. |
| `medieval_medical_honey_vinegar_antiseptic_kit` | kit | a honey-and-vinegar dressing kit | Normal / Good / 850g / 34m / skin | `leather` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Antiseptic Dressing \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Antiseptic_Kit \| TopicalCream_Honey_Poultice | This kit gathers honeyed pads, vinegar cloths, and a little wooden spatula in a waxed leather wrap. The mixture of sweet and sharp smells is unmistakable. Everything is arranged to clean a wound and then cover it quickly. |
| `medieval_medical_plain_tending_roll` | roll | a plain wound-tending roll | Normal / Standard / 540g / 18m / skin | `linen` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Kit \| Container_Pouch | A roll of linen contains compresses, strips, packing cloths, and a few simple ties. It is not a surgical set, but it gives a healer enough supplies for routine tending. The outside is marked by use, with darker patches at the fold lines. |
| `medieval_medical_travel_tending_roll` | roll | a travel healer’s tending roll | Normal / Good / 900g / 36m / skin | `leather` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| Tend_Kit \| Clean_Kit | This leather-backed roll opens to reveal folded cloths, narrow bandages, a scraping spatula, and several waxed packets. It is built to travel, with loops that keep every piece from falling out on the road. The outer hide is scuffed from saddlebag use. |
| `medieval_medical_suturing_kit` | kit | a compact suturing kit | Small / Standard / 360g / 32m / skin | `leather` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Kit \| Container_Pouch | A small leather roll holds thread, needles, a bronze probe, and tiny wiping cloths. The needles sit in a stiffened flap and the thread is wound on bone and wood spools. It is plain but complete enough for ordinary stitching work. |
| `medieval_medical_fine_suturing_case` | case | a fine suturing case | Small / Good / 420g / 72m / skin | `leather` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Suture_Kit_Good \| Container_Pouch | This narrow case is covered in dark leather and fitted with small loops for silk, gut, and linen sutures. Bronze and bone needles sit separately so their points do not rub dull. It is a careful surgical supply, made for an infirmary or court physician. |
| `medieval_medical_field_surgeon_roll` | roll | a field surgeon’s leather roll | Normal / Standard / 1600g / 85m / skin | `leather` | Market / Medicine / Surgical Supplies \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| FieldMedkit \| Suture_Kit | Heavy leather folds around bandages, sutures, a probe, and small metal tools. The roll is stained and repaired, but the straps still hold the contents firmly. It is meant to be opened beside a wounded person rather than displayed on a shelf. |
| `medieval_medical_battlefield_medkit` | kit | a battlefield medicine kit | Normal / Standard / 2400g / 120m / skin | `leather` | Market / Medicine / Surgical Supplies \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit \| Functions / Medical Treatment / Wound Cleaning | Holdable \| Destroyable_Misc \| FieldMedkit \| Clean_Kit \| Tend_Kit | This stout shoulder kit contains broad bandages, packed compresses, splints, sutures, and several waxed remedy packets. The leather is scuffed and darkened by travel, with a wide strap for carrying across a camp. It is built for trauma rather than household sickness. |
| `medieval_medical_infirmary_chest` | chest | a stocked infirmary medicine chest | Large / Good / 9500g / 260m / skin | `wood` | Market / Medicine / Standard Medicine \| Market / Medicine / Treatment Supplies \| Market / Medicine / Surgical Supplies \| Market / Medicine / Apothecary Goods | Holdable \| Destroyable_WoodenHeavy \| FieldMedkit \| Container_Trunk | This wooden chest is divided into small compartments for bandages, thread, salve pots, and medicine vessels. The lid is plain but sturdy, with scratches from repeated opening. It is heavy enough to belong in an infirmary more than on a traveller’s belt. |
| `medieval_medical_monastic_infirmary_chest` | chest | a monastic infirmary chest | Large / Good / 11000g / 300m / skin | `oak` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Treatment Supplies \| Market / Medicine / Surgical Supplies \| Market / Medicine / Herbal Medicine | Holdable \| Destroyable_WoodenHeavy \| FieldMedkit \| Container_Trunk | A pale wooden chest holds ordered rows of cloth, simple salves, suture spools, and labelled packets. A modest cross is cut into the lid rather than painted brightly. The whole piece is careful, clean, and practical, suited to a monastery sickroom. |
| `medieval_drug_apothecary_remedy_case` | case | an apothecary remedy case | Normal / Good / 1350g / 96m / skin | `leather` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Container_Pouch | This case opens into small cells for vials, paper packets, pellets, and salve pots. The inner dividers are stained by herbs and resin, and the leather outer shell has a careful flap closure. It is meant for prepared remedies rather than surgery. |
| `medieval_medical_caravan_medicine_roll` | roll | a caravan medicine roll | Normal / Standard / 1250g / 70m / skin | `leather` | Market / Medicine / Treatment Supplies \| Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Misc \| Tend_Kit \| Clean_Kit \| Container_Pouch | A long leather roll carries compact dressings, bitter herb packets, waxed salves, and a few empty vessels. The ties are reinforced for travel, and the outside smells of dust, horse, and old spice. It is a practical kit for long roads and poor access to town healers. |
| `medieval_medical_shipboard_wound_chest` | chest | a shipboard wound chest | Large / Standard / 7200g / 150m / skin | `wood` | Market / Medicine / Treatment Supplies \| Market / Medicine / Surgical Supplies | Holdable \| Destroyable_WoodenHeavy \| FieldMedkit \| Container_Trunk | This small chest is tar-darkened at the seams and packed with cloth rolls, cord ties, salve pots, and scraping tools. The wood is heavy for its size, made to survive damp decks. Salt marks show on the iron fittings. |
| `medieval_medical_horse_physick_kit` | kit | a horse physick kit | Normal / Standard / 1800g / 72m / skin | `leather` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility | Holdable \| Destroyable_Misc \| Tend_Kit \| Bandage_Good | Broad dressings, strong ties, hoof cloths, and large cleaning pads are packed inside this worn leather kit. It is scaled for animals rather than people, with stout thread and heavy linen. Stable smells of leather, vinegar, and horsehair cling to it. |
| `medieval_medical_burn_care_box` | box | a burn-care dressing box | Normal / Good / 1550g / 80m / skin | `wood` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Tend Kit \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_WoodenHeavy \| Tend_Kit \| TopicalCream_Herbal_Burn_Salve | This small wooden box carries slick salve cloths, soft pads, and waxed packets for covering burned skin. The contents smell of honey, green herbs, and beeswax. Its compartments are shallow so the dressings do not crush each other. |
| `medieval_medical_styptic_box` | box | a styptic powder box | Small / Good / 360g / 45m / skin | `wood` | Market / Medicine / Apothecary Goods \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic | Holdable \| Destroyable_WoodenHeavy \| TopicalCream_Yarrow_Styptic | A palm-sized wooden box contains folded pads, yarrow bundles, and pale mineral powder packets. The lid fits tightly to keep the contents dry. Everything inside is arranged for small cuts, shaving wounds, and quick pressure rather than broad nursing. |
| `medieval_medical_fracture_care_bundle` | bundle | a fracture-care splint bundle | Normal / Standard / 900g / 24m / skin | `willow` | Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Limb_Immobilising | Straight wooden splints, linen pads, and tie cords are bundled in a heavy cloth sleeve. The pieces are pre-smoothed and ready to bind around a limb after setting. It is bulky but far faster than carving splints after the injury. |
| `medieval_medical_cupping_kit` | kit | a small cupping kit | Small / Standard / 700g / 34m / skin | `glass` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Medical Tools / Cupping Vessel | Holdable \| Destroyable_Glassware \| Container_Pouch | Several small cups, a scraping cloth, and a simple lamp plate are wrapped inside a padded pouch. The cups are visible through gaps in the cloth, their rims polished smooth from repeated use. It is a healer’s tool kit, though it carries no automatic treatment of its own. |
| `medieval_medical_cautery_kit` | kit | a wrapped cautery kit | Normal / Standard / 1500g / 48m / skin | `leather` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Cautery Iron | Holdable \| Destroyable_Misc \| Container_Pouch | A narrow iron, thick pads, and a little charcoal packet are secured in a heavy leather wrap. Dark scorch marks stain the inner layer where the hot tool has been set down before. The kit looks harsh, deliberate, and practical. |

### 3.4 Durable medical tools

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_medical_linen_medicine_strainer` | strainer | a linen medicine strainer | VerySmall / Standard / 80g / 5m / skin | `linen` | Market / Professional Tools / Standard Tools \| Market / Medicine / Herbal Medicine \| Functions / Tools / Medical Tools / Medicine Strainer | Holdable \| Destroyable_Misc | Tight linen is stretched over a small willow hoop and stitched in place. The weave is close enough to catch grit, resin flecks, and steeped herbs from medicine liquids. The hoop has a small cord loop for hanging beside a hearth or apothecary bench. |
| `medieval_medical_stone_mortar_and_pestle` | mortar | a stone mortar and pestle | Small / Standard / 1450g / 12m / skin | `stone` | Market / Professional Tools / Standard Tools \| Market / Medicine / Apothecary Goods \| Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle | Holdable \| Destroyable_Misc | This compact stone mortar is paired with a blunt pestle worn smooth by grinding. Its bowl is darkened by old herbs and mineral powders, with a few chips along the rim. It is heavy enough to stay steady while crushing medicine stock. |
| `medieval_medical_ceramic_apothecary_mortar` | mortar | a glazed ceramic apothecary mortar | Small / Good / 900g / 28m / skin | `glazed ceramic` | Market / Professional Tools / Standard Tools \| Market / Medicine / Apothecary Goods \| Functions / Tools / Cooking / Cooking Utensils / Mortar and Pestle | Holdable \| Destroyable_Misc | This small glazed mortar has a pale bowl and a matching pestle with a rounded end. Herbal stains have settled into hairline scratches inside the glaze. It is cleaner and finer than rough stone, suited to an apothecary counter. |
| `medieval_medical_bronze_ointment_spatula` | spatula | a bronze ointment spatula | Tiny / Standard / 35g / 10m / skin | `bronze` | Market / Professional Tools / Standard Tools \| Market / Medicine / Apothecary Goods \| Functions / Tools / Medical Tools / Ointment Spatula | Holdable \| Destroyable_HeavyMetal | A slim bronze spatula has one flattened end for lifting salve and one rounded end for stirring. The handle is narrow, plain, and slightly dark from age. It is small enough to live inside a medicine roll. |
| `medieval_medical_bone_ointment_spatula` | spatula | a polished bone ointment spatula | Tiny / Standard / 22g / 5m / skin | `bone` | Market / Professional Tools / Standard Tools \| Market / Medicine / Apothecary Goods \| Functions / Tools / Medical Tools / Ointment Spatula | Holdable \| Destroyable_Misc | This spatula is carved from pale bone and rubbed smooth along its rounded blade. A shallow groove down the handle gives the fingers purchase when working with slick salves. It is light, cheap, and easy to clean by scraping. |
| `medieval_medical_horn_dosing_spoon` | spoon | a small horn dosing spoon | Tiny / Standard / 20g / 6m / skin | `horn` | Market / Medicine / Apothecary Goods \| Market / Professional Tools / Standard Tools | Holdable \| Destroyable_Misc | A little spoon of polished horn has a shallow bowl and a short handle. The rim is smoothed thin enough for careful pouring, but not marked with exact measures. Its dark translucent colour gives it a humble apothecary look. |
| `medieval_medical_glass_cupping_vessel` | cup | a small glass cupping vessel | VerySmall / Standard / 120g / 16m / skin | `glass` | Market / Professional Tools / Standard Tools \| Market / Medicine / Surgical Supplies \| Functions / Tools / Medical Tools / Cupping Vessel | Holdable \| Destroyable_Glassware | This rounded glass cup has a thick lip and a slightly smoky tint. The base is broad enough to sit upright, while the mouth is polished smooth. It is fragile but clear enough to show the skin beneath when pressed down. |
| `medieval_medical_clay_cupping_cup` | cup | a clay cupping cup | VerySmall / Standard / 160g / 8m / skin | `fired clay` | Market / Professional Tools / Standard Tools \| Market / Medicine / Surgical Supplies \| Functions / Tools / Medical Tools / Cupping Vessel | Holdable \| Destroyable_Misc | A small fired-clay cup has a rounded belly, a thick rim, and soot darkening near the base. It is opaque and sturdier than glass, with a rougher finish. A healer could warm it quickly over a flame. |
| `medieval_medical_leech_jar` | jar | a small leech jar | Small / Standard / 750g / 22m / skin | `glazed ceramic` | Market / Medicine / Apothecary Goods \| Market / Medicine / Surgical Supplies | Holdable \| Destroyable_Misc \| Container_Pouch | This glazed ceramic jar has a perforated lid tied down with cord. Water marks stain the inside, and the rim is broad enough for a hand to reach in carefully. It is a medical storage vessel rather than a drinking jar. |
| `medieval_medical_bronze_scalpel` | scalpel | a bronze surgical scalpel | Tiny / Standard / 65g / 18m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Scalpel | Holdable \| Destroyable_HeavyMetal | A small bronze blade is set into a plain handle and honed to a narrow working edge. The blade is leaf-shaped and darkened near the socket from age and oil. It is a surgical knife, not a weapon meant for battle. |
| `medieval_medical_wrought_iron_surgical_knife` | knife | a wrought-iron surgical knife | VerySmall / Standard / 85g / 14m / skin | `wrought iron` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Scalpel | Holdable \| Destroyable_HeavyMetal | This small wrought-iron knife has a narrow blade and a wrapped wooden grip. The edge is keen but plain, with tool marks still visible along the spine. It is built for incision and scraping rather than ornament. |
| `medieval_medical_bronze_forceps` | forceps | a pair of bronze forceps | VerySmall / Standard / 90g / 16m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Forceps | Holdable \| Destroyable_HeavyMetal | These bronze forceps are narrow, springy, and flattened at the tips. The metal shows polished bright places where fingers press them together. They are useful for grasping cloth, splinters, or small pieces during wound work. |
| `medieval_medical_bronze_surgical_probe` | probe | a bronze surgical probe | Tiny / Standard / 35g / 12m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Surgical Probe | Holdable \| Destroyable_HeavyMetal | A slender bronze probe has a rounded end at one side and a flattened spatulate end at the other. The shaft is straight, smooth, and slightly dark in the middle. It is made for feeling, lifting, and applying rather than cutting. |
| `medieval_medical_bronze_arterial_clamp` | clamp | a bronze arterial clamp | VerySmall / Standard / 125g / 22m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Arterial Clamp | Holdable \| Destroyable_HeavyMetal | This bronze clamp has flattened jaws and a crude locking bend behind them. It is not delicate, but the bite is firm and the spring returns when released. The tool is meant for serious wound work where hands alone are not enough. |
| `medieval_medical_small_bonesaw` | saw | a small surgical bonesaw | Small / Standard / 360g / 34m / skin | `wrought iron` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Bonesaw | Holdable \| Destroyable_HeavyMetal | A narrow metal saw blade is fixed into a wooden grip, with fine teeth running along one edge. The blade is shorter than a carpenter’s saw and easier to control close to the body. Oil darkens the join between metal and handle. |
| `medieval_medical_bronze_cautery_iron` | iron | a bronze cautery iron | Small / Standard / 420g / 28m / skin | `bronze` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools \| Functions / Tools / Surgical Tools / Cautery Iron | Holdable \| Destroyable_HeavyMetal | A bronze working end sits on a long handle meant to keep the hand away from heat. The tip is flattened, dark, and smoke-stained, while the grip has been wrapped in old leather. It is a stern-looking surgical tool. |
| `medieval_medical_wrought_iron_cautery_brazier` | brazier | a small cautery brazier | Normal / Standard / 2100g / 36m / skin | `wrought iron` | Market / Medicine / Surgical Supplies \| Market / Professional Tools / Standard Tools | Holdable \| Destroyable_HeavyMetal | This small wrought-iron brazier has a shallow fire bowl and three short feet. The rim is blackened by charcoal and ash, but there is no special incense fitting. It is meant to heat tools at the edge of a work space. |
| `medieval_medical_apothecary_balance_scale` | scale | a small apothecary balance scale | Small / Good / 650g / 60m / skin | `bronze` | Market / Medicine / Apothecary Goods \| Market / Professional Tools / Standard Tools | Holdable \| Destroyable_HeavyMetal | A compact balance scale folds into a little wooden case with two shallow pans. The beam is fine bronze and the weights sit in their own recesses. It is delicate enough for powders, resins, and measured medicine stock. |
| `medieval_medical_surgical_needle_case` | case | a narrow surgical needle case | Tiny / Standard / 45g / 12m / skin | `bone` | Market / Medicine / Surgical Supplies \| Functions / Medical Treatment / Suture Kit | Holdable \| Destroyable_Misc \| Container_Sachet | This narrow case is carved from bone and fitted with a tight stopper. The inside is long enough for curved needles and slim probes, while the outside is polished by handling. A cord loop lets it hang inside a surgical roll. |
| `medieval_medical_apothecary_sorting_tray` | tray | a shallow apothecary sorting tray | Normal / Standard / 850g / 18m / skin | `wood` | Market / Medicine / Apothecary Goods \| Market / Professional Tools / Standard Tools | Holdable \| Destroyable_WoodenHeavy \| Container_Tray | A shallow wooden tray is divided into small sections for herbs, powders, and tiny packets. The surface is stained by saffron, resin, and dark medicine dust. It is light enough to lift but broad enough to work on beside a mortar. |

### 3.5 Drug-bearing medicines, vessels, smoke, and fumigation

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_drug_honey_poultice_pot` | pot | a pot of honey poultice | VerySmall / Standard / 220g / 16m / skin | `fired clay` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Antiseptic Dressing | Holdable \| Destroyable_Misc \| TopicalCream_Honey_Poultice | A small fired-clay pot is capped with waxed cloth over a sticky honey poultice. Sweetness leaks through the wrap, mingled with a faint herbal smell. A flat spatula mark remains in the surface of the mixture. |
| `medieval_drug_garlic_salve_pot` | pot | a pungent garlic salve pot | VerySmall / Standard / 190g / 14m / skin | `ceramic` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Garlic_Salve | This thumb-sized ceramic pot is sealed with beeswax and cloth. The garlic smell pushes through the seal, sharp and earthy. Dark green flecks of herb cling beneath the rim where the salve has been stirred. |
| `medieval_drug_aloe_burn_salve_jar` | jar | a jar of aloe burn salve | VerySmall / Good / 240g / 32m / skin | `glazed ceramic` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Aloe_Burn_Salve | A small glazed jar contains a cool green salve under a neat cloth cap. The contents are glossy and smooth where the jar has been levelled. It smells mild, fresh, and faintly bitter. |
| `medieval_drug_herbal_burn_salve_pot` | pot | a pot of green burn salve | VerySmall / Good / 230g / 36m / skin | `ceramic` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Herbal_Burn_Salve | This squat pot holds a thick green-brown salve with a resin sheen. A strip of linen is tied over the top and darkened by oil at the edges. The smell is grassy, bitter, and slightly smoky. |
| `medieval_drug_yarrow_styptic_cake` | cake | a pressed yarrow styptic cake | Tiny / Standard / 40g / 8m / skin | `yarrow` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Yarrow_Styptic | Crushed yarrow has been pressed into a small greenish cake and wrapped in linen. The surface is rough and fibrous, with a bitter smell that clings to the cloth. It is shaped to be rubbed or broken into a dressing. |
| `medieval_drug_alum_styptic_cone` | cone | a pale alum styptic cone | Tiny / Standard / 35g / 10m / skin | `alum` | Market / Medicine / Apothecary Goods \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic | Holdable \| Destroyable_Misc \| TopicalCream_Alum_Styptic | A small cone of pale mineral paste has dried hard and chalky. It is wrapped in parchment with the tip protected by a twist of cloth. The surface leaves faint white dust on the fingers when handled. |
| `medieval_drug_yarrow_linen_styptic_roll` | roll | a yarrow-linen styptic roll | VerySmall / Standard / 75g / 11m / skin | `linen` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic \| Functions / Medical Treatment / Bandage | Holdable \| Destroyable_Misc \| Bandage_Good \| TopicalCream_Yarrow_Styptic | A narrow linen roll is dusted with crushed yarrow through its inner layers. The outside looks like a normal small bandage, but the green flecks show at the tucked end. It smells dry, bitter, and clean. |
| `medieval_drug_alum_wool_pressure_pad` | pad | an alum-dusted wool pressure pad | Small / Standard / 125g / 12m / skin | `wool` | Market / Medicine / Apothecary Goods \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Styptic \| Functions / Medical Treatment / Tend Kit | Holdable \| Destroyable_Misc \| Tend_Single \| TopicalCream_Alum_Styptic | Packed wool is wrapped in linen and dusted inside with pale alum powder. The pad is thicker than a common dressing and meant to be pressed down firmly. A dry mineral smell mixes with the wool. |
| `medieval_drug_honey_aloe_burn_dressing` | dressing | a honey-and-aloe burn dressing | VerySmall / Good / 95g / 28m / skin | `linen` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Bandage \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Bandage_Great \| TopicalCream_Aloe_Burn_Salve | This soft dressing is slick with a honeyed green salve and folded into a waxed wrapper. It is moist but not dripping, with a sweet smell beneath the plant bitterness. The pad is broad enough for a careful burn covering. |
| `medieval_drug_garlic_honey_poultice_packet` | packet | a garlic-honey poultice packet | Tiny / Standard / 65g / 9m / skin | `honey` | Market / Medicine / Herbal Medicine \| Market / Medicine / Treatment Supplies \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| TopicalCream_Garlic_Salve | A waxed cloth packet holds a thick mixture of honey, garlic, and crushed herbs. The smell is both sweet and sharp, with a sticky stain darkening the fold. It is small enough for a single dressing rather than a jar of repeated use. |
| `medieval_drug_willow_bark_lozenge_packet` | packet | a packet of willow-bark lozenges | Tiny / Standard / 50g / 10m / skin | `willow` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Willow_Bark_Tea | Several dark, bitter lozenges are wrapped in a small paper fold. Their surfaces are rough with powdered bark, honey, and a little spice to bind them. The packet is meant for measured swallowing rather than chewing as sweets. |
| `medieval_drug_mandrake_sleep_pellets` | packet | a packet of mandrake pellets | Tiny / Good / 35g / 36m / skin | `mandrake` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Mandrake_Draught | Tiny dark pellets sit inside a red-tied paper packet, each one rolled from powdered root and honey. The packet smells bitter and earthy even when closed. A careful hand has made the pellets nearly equal in size. |
| `medieval_drug_mint_lozenge_packet` | packet | a packet of mint lozenges | Tiny / Standard / 45g / 8m / skin | `mint` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Mint_Infusion | Pale green lozenges are folded into a little paper packet with mint crumbs caught at the creases. They smell fresh, sweet, and cooling. The pieces are plain medicine-shop fare rather than confectionery. |
| `medieval_drug_ephedra_honey_cakes` | packet | a packet of ephedra honey cakes | Tiny / Standard / 55g / 18m / skin | `ephedra` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Ephedra_Brew | Small honey-bound cakes are speckled with chopped ephedra and wrapped in oiled paper. The cakes are dry at the surface but still sticky at the edges. Their smell is grassy, sweet, and faintly resinous. |
| `medieval_drug_foxglove_warning_beads` | packet | a red-tied packet of foxglove beads | Tiny / Good / 24g / 45m / skin | `foxglove` | Market / Medicine / Apothecary Goods \| Market / Medicine / High-Quality Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Foxglove_Tincture | Several tiny dark beads are tied into a warning-marked packet with red thread. The beads are very small, carefully rolled, and bitter-smelling. The presentation makes clear that these are not ordinary household lozenges. |
| `medieval_drug_poppy_latex_bolus` | bolus | a dark poppy-latex bolus | Tiny / Good / 25g / 50m / skin | `resin` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Pill_Poppy_Latex_Draught | A small dark bolus is wrapped in oiled cloth and tied shut with thread. It feels resinous under the wrapping, and its bitter smell is heavy and unmistakable. The size is deliberately small, as though meant to be measured with care. |
| `medieval_drug_mint_ginger_pastilles` | packet | a packet of mint-ginger pastilles | Tiny / Standard / 55g / 14m / skin | `ginger` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Paper \| Pill_Mint_and_Ginger_Tonic | Round pastilles of mint, ginger, and honey are stacked in a paper fold. They smell warming and fresh at once, with ginger fibres visible in the surface. The packet is sized for a traveller or sickroom attendant. |
| `medieval_drug_theriac_electuary_ball` | ball | a dark theriac electuary ball | Tiny / Good / 40g / 75m / skin | `spice` | Market / Medicine / Apothecary Goods \| Market / Medicine / High-Quality Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Antidote | Holdable \| Destroyable_Paper \| Pill_Theriac_Electuary | A glossy ball of dark electuary is wrapped in a square of oiled parchment. The smell is rich with honey, spice, bitterness, and resin. It is carefully rounded and sealed as a valued apothecary dose. |
| `medieval_drug_theriac_travel_packet` | packet | a sealed theriac travel packet | VerySmall / Good / 75g / 120m / skin | `spice` | Market / Medicine / Apothecary Goods \| Market / Medicine / High-Quality Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Antidote | Holdable \| Destroyable_Paper \| Pill_Theriac_Electuary | This small packet contains several pea-sized electuary doses separated by waxed folds. The outer wrapping is tied twice and smells strongly of spice. It is made for cautious travel use rather than an open shop jar. |
| `medieval_drug_poppy_honey_sleep_bolus` | bolus | a poppy-honey sleep bolus | Tiny / Good / 30g / 54m / skin | `honey` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| Pill_Poppy_Latex_Draught | Honey, resin, and powdered herb have been rolled into a dark, tacky bolus. It is wrapped alone in oiled cloth and tied with a black thread. The smell is heavy, sweet, and bitter enough to discourage casual tasting. |
| `medieval_drug_empty_glass_medicine_vial` | vial | an empty glass medicine vial | VerySmall / Standard / 65g / 18m / skin | `glass` | Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Glassware \| LContainer_Medicine_Vial_30ml | This small glass vial has a narrow neck and a plain stopper fitted with wax around the rim. The glass is faintly green and thick for its size. It is empty, clean, and ready for a measured medicine. |
| `medieval_drug_empty_ceramic_medicine_bottle` | bottle | an empty ceramic medicine bottle | Small / Standard / 180g / 16m / skin | `ceramic` | Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Misc \| LContainer_Medicine_Bottle_100ml | A little ceramic bottle has a rounded shoulder, narrow mouth, and cork stopper under a cloth tie. The outer surface is unglazed except for a smooth lip. It is sturdy enough for travel and opaque enough to protect its contents from casual inspection. |
| `medieval_drug_empty_leather_medicine_flask` | flask | an empty leather medicine flask | Small / Standard / 260g / 20m / skin | `leather` | Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Misc \| LContainer_Medicine_Flask_250ml | This small leather flask has a wooden stopper and a waxed seam along one side. It is soft-sided, dark from oiling, and fitted with a cord loop. It is made for travelling medicine rather than table drink. |
| `medieval_drug_horn_medicine_vial` | vial | a small horn medicine vial | VerySmall / Standard / 55g / 14m / skin | `horn` | Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Misc \| LContainer_Medicine_Vial_30ml | A polished horn vial is capped with a tight wooden plug and sealed with a strip of cloth. The translucent brown walls hide most of the contents but catch light at the thin edges. It is a humble alternative to glass. |
| `medieval_drug_willow_bark_tea_flask` | flask | a flask of willow-bark tea | Small / Standard / 280g / 22m / skin | `leather` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Willow_Bark_Tea_250ml | A small stoppered flask is filled with a dark bitter tea. The outside is plain leather over a rigid liner, with a willow chip tied to the neck as a sign. The vessel smells faintly of bark and steeped water. |
| `medieval_drug_mandrake_draught_bottle` | bottle | a bottle of mandrake draught | VerySmall / Good / 190g / 58m / skin | `ceramic` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Mandrake_Draught_100ml | A small opaque ceramic bottle is stoppered and tied with a red warning thread. Bitter earthy smells cling around the waxed mouth. The bottle is compact and carefully sealed, meant for a measured draught rather than table use. |
| `medieval_drug_mint_infusion_flask` | flask | a flask of mint infusion | Small / Standard / 270g / 18m / skin | `leather` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Mint_Infusion_250ml | This small leather flask carries a pale herbal infusion and a tied sprig of mint at the stopper. The vessel is travel-worn but clean, with wax pressed into the seam. A fresh green smell escapes when it is handled. |
| `medieval_drug_ephedra_brew_flask` | flask | a flask of ephedra brew | Small / Standard / 285g / 34m / skin | `leather` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Ephedra_Brew_250ml | A dark leather flask is stoppered tightly over a bitter herbal brew. Dried jointed twigs are tied to the neck for identification. The vessel is compact, practical, and clearly meant for a road kit or healer’s pack. |
| `medieval_drug_foxglove_tincture_vial` | vial | a red-marked foxglove tincture vial | VerySmall / Good / 70g / 65m / skin | `glass` | Market / Medicine / Apothecary Goods \| Market / Medicine / High-Quality Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Glassware \| LContainer_Medicine_Foxglove_Tincture_30ml | This narrow glass vial is sealed with wax and tied with red thread around the neck. A dark tincture clings to the inside in a thin line. The small size and careful markings warn that it is a measured medicine. |
| `medieval_drug_poppy_latex_draught_bottle` | bottle | a bottle of poppy-latex draught | VerySmall / Good / 190g / 78m / skin | `ceramic` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Poppy_Latex_Draught_100ml | A small stoppered bottle holds a darkened draught with a bitter resin smell at the mouth. The wax seal is thick and slightly stained. Its compact form suggests careful dosing rather than casual drinking. |
| `medieval_drug_mint_ginger_tonic_flask` | flask | a flask of mint-and-ginger tonic | Small / Standard / 280g / 30m / skin | `leather` | Market / Medicine / Herbal Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Herbal Remedy | Holdable \| Destroyable_Misc \| LContainer_Medicine_Mint_and_Ginger_Tonic_250ml | This small flask is filled with a warming herbal tonic and tied with a narrow green cord. The stopper smells of mint and ginger even before opening. Its leather covering is worn smooth by travel. |
| `medieval_drug_theriac_syrup_bottle` | bottle | a bottle of theriac syrup | VerySmall / Good / 210g / 120m / skin | `glazed ceramic` | Market / Medicine / Apothecary Goods \| Market / Medicine / High-Quality Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Antidote | Holdable \| Destroyable_Misc \| LContainer_Medicine_Theriac_Syrup_100ml | A small glazed bottle is sealed with wax over a dark, thick syrup. Spice, honey, and resin smells gather around the neck. The bottle is decorated only by a dark thread and a careful apothecary knot. |
| `medieval_drug_plain_medicine_bottle_set` | set | a set of empty medicine bottles | Small / Standard / 520g / 42m / skin | `ceramic` | Market / Medicine / Apothecary Goods | Holdable \| Destroyable_Misc \| LContainer_Medicine_Bottle_100ml | Three small ceramic bottles sit together in a padded linen wrap. Each bottle has a stopper, a cloth tie, and enough space for a simple mark or label. The set is empty and ready for an apothecary or infirmary shelf. |
| `medieval_drug_mandrake_smoke_cone` | cone | a mandrake smoke cone | Tiny / Good / 25g / 28m / skin | `mandrake` | Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Misc \| Smokeable_Mandrake_Draught | A dark little cone of powdered root and resin is wrapped in thin paper. It smells earthy and bitter, with flecks of pale mandrake visible in the pressed surface. The cone is made for personal smoke rather than broad room fumigation. |
| `medieval_drug_henbane_smoke_roll` | roll | a henbane smoke roll | Tiny / Good / 20g / 30m / skin | `henbane` | Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Misc \| Smokeable_Henbane_Smoke | Dried henbane leaves are rolled into a narrow paper twist and tied at both ends. The roll is dark, brittle, and acrid-smelling even before it is lit. A black thread marks it apart from ordinary incense. |
| `medieval_drug_bronchial_smoke_pellets` | packet | a packet of bronchial smoke pellets | Tiny / Standard / 35g / 22m / skin | `resin` | Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Herbal Remedy \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Misc \| Smokeable_Bronchial_Smoke | Several small pellets of herb and resin sit in a waxed packet. They smell sharp, resinous, and green, with a little dust clinging to the paper. The pellets are meant to be smoked one at a time. |
| `medieval_drug_soporific_smoke_cone` | cone | a soporific smoke cone | Tiny / Good / 24g / 42m / skin | `resin` | Market / Medicine / Apothecary Goods \| Market / Medicine / Herbal Medicine \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Misc \| Smokeable_Soporific_Fumes | This small black cone is pressed from bitter herbs and resin and wrapped alone in oiled paper. Its smell is heavy, sweet, and unsettling. A dark warning knot closes the packet. |
| `medieval_drug_plain_fumigation_pastilles` | packet | a packet of fumigation pastilles | Tiny / Standard / 80g / 12m / skin | `resin` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Fumigation Stock \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Misc | Small brown pastilles of resin, herb, and charcoal are folded in a linen packet. They have a dry incense smell and crumble slightly at the edges. The packet is made as fuel stock for a fumigation vessel, not as a medicine by itself. |
| `medieval_drug_resin_herb_fumigation_packet` | packet | a resin-herb fumigation packet | VerySmall / Standard / 120g / 18m / skin | `resin` | Market / Medicine / Herbal Medicine \| Functions / Material Functions / Medical Craft Stock / Fumigation Stock \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_Paper | Chunks of resin and bitter herbs are mixed in a waxed paper packet. The pieces are irregular but dry, with charcoal dust darkening the fold. It is marked as fumigation stock rather than food or perfume. |
| `medieval_drug_mandrake_fumigation_censer` | censer | a bronze mandrake fumigation censer | Small / Good / 850g / 90m / skin | `bronze` | Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Fumigation \| Market / Medicine / Herbal Medicine | Holdable \| Destroyable_HeavyMetal \| IncenseBurner_Mandrake_Draught | This small bronze censer has pierced vents and a darkened bowl for burning fumigation stock. Bitter earthy smoke stains the inner rim, and a short handle lets it be moved carefully. It is a drug-bearing fumigation vessel, not an ordinary incense ornament. |
| `medieval_drug_henbane_fumigation_bowl` | bowl | a ceramic henbane fumigation bowl | Small / Good / 780g / 86m / skin | `ceramic` | Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Fumigation \| Market / Medicine / Herbal Medicine | Holdable \| Destroyable_Misc \| IncenseBurner_Henbane_Smoke | A squat ceramic bowl is pierced around the upper wall and blackened inside. The outside is plain, but a black cord around the foot warns that it is not ordinary household incense gear. Its shape keeps the smoke low and close. |
| `medieval_drug_bronchial_fumigation_censer` | censer | a herbal bronchial fumigation censer | Small / Good / 900g / 78m / skin | `bronze` | Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Fumigation \| Market / Medicine / Herbal Medicine | Holdable \| Destroyable_HeavyMetal \| IncenseBurner_Bronchial_Smoke | This bronze censer is broad, low, and pierced with crescent vents around the lid. Old greenish smoke stains the seams where the lid lifts free. It is built for room fumigation with sharp herbal smoke. |
| `medieval_drug_soporific_sickroom_brazier` | brazier | a soporific sickroom brazier | Small / Good / 1100g / 130m / skin | `bronze` | Market / Medicine / High-Quality Medicine \| Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Fumigation | Holdable \| Destroyable_HeavyMetal \| IncenseBurner_Soporific_Fumes | A small bronze brazier stands on three feet with a perforated cover over the bowl. Dark, sweet-smelling soot marks the vents, and a black thread is tied to the handle. It is meant for controlled sickroom fumigation rather than cheerful household scent. |
| `medieval_drug_portable_fumigation_casket` | casket | a portable fumigation casket | Small / Standard / 720g / 64m / skin | `ceramic` | Market / Medicine / Apothecary Goods \| Functions / Medical Treatment / Fumigation \| Market / Medicine / Herbal Medicine | Holdable \| Destroyable_Misc \| IncenseBurner_Mandrake_Draught | This small ceramic casket has holes cut through the lid and a fire-darkened cup inside. The outer walls are wrapped with a thin leather carrying strap. It is compact enough for travel while still functioning as a fumigation vessel. |

### 3.6 Mobility, casualty transport, and prosthetics

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_medical_padded_willow_splints` | splints | a pair of padded willow splints | Normal / Standard / 420g / 14m / skin | `willow` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Limb_Immobilising | Two straight willow boards are smoothed, drilled for ties, and padded with folded linen. The pair is light but stiff enough to hold a forearm or shin in place. Old pressure marks show where cords have been knotted before. |
| `medieval_medical_reed_finger_splints` | splints | a bundle of reed finger splints | VerySmall / Standard / 80g / 5m / skin | `reed` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| Limb_Immobilising | Thin reed splints are bundled with narrow linen ties, each one trimmed for a finger or toe. The pieces are light, smooth, and slightly flexible. They look meant for small fractures rather than heavy limb binding. |
| `medieval_medical_padded_leg_splints` | splints | a pair of padded leg splints | Normal / Standard / 950g / 24m / skin | `wood` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Limb_Immobilising | Long wooden splints are lined with wool padding and tied with broad linen straps. They are longer and heavier than arm splints, with enough stiffness for a lower leg. The padding is replaceable but worn smooth in places. |
| `medieval_medical_linen_arm_sling` | sling | a plain linen arm sling | Small / Standard / 140g / 5m / skin | `linen` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| Limb_Immobilising | A broad triangle of linen is hemmed at the edges and fitted with two simple ties. The cloth is soft where it would cradle an arm against the body. It is plain enough for any infirmary or household chest. |
| `medieval_medical_leather_shoulder_sling` | sling | a leather shoulder support sling | Small / Good / 260g / 16m / skin | `leather` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| Limb_Immobilising | This sling uses soft leather straps and a linen cradle to hold an injured arm tight against the chest. The shoulder band is broad, spreading weight more comfortably than a thin cord. It is made for longer wear than a simple cloth triangle. |
| `medieval_medical_bamboo_travel_splints` | splints | a set of bamboo travel splints | Normal / Standard / 520g / 18m / skin | `bamboo` | Market / Medicine / Treatment Supplies \| Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Splint \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Limb_Immobilising | Split bamboo lengths are smoothed and stacked inside a cloth sleeve. The pieces are light, stiff, and easy to carry on the road. Linen cords threaded through the sleeve keep the set together. |
| `medieval_medical_forked_oak_crutch` | crutch | a forked oak walking crutch | Normal / Standard / 1250g / 18m / skin | `oak` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Crutch | This crutch is cut from oak with a natural fork padded by wrapped wool. The shaft is smoothed by hand and slightly polished where it is gripped. It is sturdy but heavier than a simple cane. |
| `medieval_medical_padded_willow_crutch` | crutch | a padded willow crutch | Normal / Standard / 820g / 20m / skin | `willow` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Crutch | A light willow crutch has a carved fork, a leather grip, and a linen pad tied across the top. The shaft flexes slightly but remains straight. It is easier to carry than oak and made for everyday convalescence. |
| `medieval_medical_staff_crutch` | crutch | a staff-like healer’s crutch | Normal / Standard / 900g / 16m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Crutch | This straight wooden crutch looks almost like a walking staff, with a side grip and a padded upper brace. The end is capped in worn leather to keep it from splitting. It is simple, portable, and useful on uneven roads. |
| `medieval_medical_canvas_field_stretcher` | stretcher | a canvas field stretcher | Large / Standard / 5200g / 28m / skin | `linen` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| DragAid_Stretcher | Two wooden poles run through a broad linen-canvas bed stitched at the edges. The handles are worn smooth, and the cloth sags slightly between them. It is made for carrying wounded people over streets, camps, or fields. |
| `medieval_medical_wooden_litter` | litter | a simple wooden casualty litter | Large / Standard / 7600g / 45m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| DragAid_Stretcher | This wooden litter has side rails, a slatted bed, and cord lashings to keep the frame from twisting. It is heavier than a canvas stretcher but steadier under a large patient. The rails show handwear from repeated carrying. |
| `medieval_medical_drag_sling` | sling | a casualty drag sling | Normal / Standard / 2100g / 24m / skin | `leather` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| DragAid_Sling | A broad leather-and-linen sling is fitted with long hauling straps and a reinforced cradle. It is meant to pull someone clear when four carriers are not available. The underside is scuffed from contact with ground and floors. |
| `medieval_medical_wooden_travois` | travois | a wooden casualty travois | Large / Standard / 6500g / 32m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| DragAid_Travois | Two long wooden poles are lashed together with crosspieces and a net of hemp cord. The frame narrows at one end for pulling by hand or beast. It is rough, field-made, and useful across open ground. |
| `medieval_medical_winter_casualty_sled` | sled | a low casualty sled | Large / Standard / 8200g / 52m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| DragAid_Sled | This low sled has upturned wooden runners and a lashed linen bed between them. The front is fitted with a hauling rope, and the runners are polished by use. It is made for snow, mud, or smooth floors rather than stairs. |
| `medieval_medical_wounded_harness` | harness | a wounded-man hauling harness | Normal / Standard / 1800g / 36m / skin | `leather` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_Misc \| DragAid_Harness | A set of broad leather straps is fitted with buckles, loops, and a chest cradle. It lets rescuers drag or support a wounded person without gripping clothing alone. The leather is stiff but well oiled. |
| `medieval_medical_wooden_peg_foot` | foot | a simple wooden peg foot | Small / Standard / 650g / 45m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Prosthetic_RFoot | A carved wooden peg foot is shaped to strap below the leg with a leather cuff. The end is broad and worn flat from ground contact. It is functional in the simplest way, with no attempt to mimic toes. |
| `medieval_medical_left_lower_leg_prosthesis` | leg | a left wooden lower-leg prosthesis | Normal / Standard / 1800g / 95m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Prosthetic_LLowerLeg | This left lower-leg prosthesis is carved from wood and fitted with leather straps at the top. The shaft narrows toward a blunt walking end. Padding inside the cuff has been replaced more than once. |
| `medieval_medical_right_lower_leg_prosthesis` | leg | a right wooden lower-leg prosthesis | Normal / Standard / 1800g / 95m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Prosthetic_RLowerLeg | This right lower-leg prosthesis combines a wooden shaft, leather cuff, and simple walking end. The fittings are practical rather than beautiful, with visible peg holes and cord repairs. It is meant for use, not display. |
| `medieval_medical_left_plain_hand_prosthesis` | hand | a plain left wooden hand | Small / Standard / 420g / 60m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Prosthetic_LHand | A left wooden hand is carved into a fixed open shape and fitted with a leather wrist socket. The fingers do not move, but the palm is shaped enough to fill a sleeve or rest against an object. The surface is rubbed smooth but undecorated. |
| `medieval_medical_right_hook_hand` | hook | a right bronze hook hand | Small / Good / 520g / 130m / skin | `bronze` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_HeavyMetal \| Prosthetic_RHand_Functional | A bronze hook is mounted into a leather wrist cuff with buckled straps. The hook is blunt at the inside curve and polished bright where it would catch cord or handle. It is one of the more practical artificial hands in the catalogue. |
| `medieval_medical_left_glass_eye` | eye | a small left glass eye | Tiny / Good / 18g / 80m / skin | `glass` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic | Holdable \| Destroyable_Glassware \| Prosthetic_LEye | A small glass eye is shaped as a smooth convex piece with a dark painted centre. It is not lifelike in close inspection, but it has enough shine and colour to fill an empty socket under normal light. A padded scrap of cloth protects it. |
| `medieval_medical_right_ornamental_foot` | foot | an ornamental right prosthetic foot | Small / Good / 560g / 75m / skin | `wood` | Market / Medicine / Prosthetics and Mobility \| Functions / Medical Treatment / Prosthetic \| Functions / Medical Treatment / Mobility Aid | Holdable \| Destroyable_WoodenHeavy \| Prosthetic_RFoot_Ornamental | This right prosthetic foot is carved from wood with a stylised instep and simple toe marks. The shape is more presentable than practical, with leather straps set for fitting to a lower-leg piece. It has been polished to a dark sheen. |

### 3.7 Repair kits and repair supplies

| Unique reference | Noun | SDesc | Specs | Material | Tags | Components | FDesc |
| --- | --- | --- | --- | --- | --- | --- | --- |
| `medieval_repair_rough_cloth_mending_pouch` | pouch | a rough cloth-mending pouch | Small / Poor / 320g / 12m / skin | `linen` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Cloth_Poor \| Container_Pouch | This small pouch holds coarse thread, mismatched patches, bone needles, and a lump of beeswax. It is poor but useful for keeping clothing and cloth gear together. The outer pouch is itself patched in two places. |
| `medieval_repair_cloth_mending_roll` | roll | a cloth-mending roll | Small / Standard / 480g / 24m / skin | `linen` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Cloth \| Container_Pouch | A linen roll opens to show thread, patches, needles, and small shears wrapped in ordered rows. The supplies are ordinary but clean and easy to find. It is suitable for household clothing, tents, and cloth packs. |
| `medieval_repair_fine_cloth_mending_case` | case | a fine cloth-mending case | Small / Good / 560g / 65m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Cloth_Good \| Container_Pouch | This neat leather case holds dyed silk thread, fine linen patches, polished needles, and a small smoothing bone. The interior loops keep the tools from tangling. It is fit for better garments and household textiles. |
| `medieval_repair_rough_leather_repair_pouch` | pouch | a rough leather repair pouch | Small / Poor / 620g / 18m / skin | `leather` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Leather_Poor \| Container_Pouch | This pouch carries heavy thonging, scrap leather, a blunt awl, and waxed thread. The tools are crude and scarred, but they are enough for a field repair. It smells of oil, hide, and dust. |
| `medieval_repair_leather_repair_roll` | roll | a leather repair roll | Normal / Standard / 1100g / 45m / skin | `leather` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Leather \| Container_Pouch | A sturdy leather roll holds awls, thonging, waxed thread, spare patches, and a small burnisher. Each tool has a simple loop to keep it in place. It is a standard kit for belts, bags, shoes, and harness. |
| `medieval_repair_saddler_leather_case` | case | a saddler’s leather repair case | Normal / Good / 1450g / 95m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Leather_Good \| Container_Pouch | This better case is stocked with fine awls, strong waxed cord, dyed leather patches, and polished bone tools. The stitching is neat and the contents are chosen for more careful repair work. It suits a saddler or prosperous leatherworker. |
| `medieval_repair_simple_wood_repair_pouch` | pouch | a simple wood-repair pouch | Small / Poor / 700g / 16m / skin | `wood` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Wood_Poor \| Container_Pouch | Small pegs, wedges, glue scrap, and a blunt carving knife are kept in this rough pouch. The contents are cheap but useful for tightening loose wooden goods. Wood dust has settled into the seams. |
| `medieval_repair_woodworker_repair_box` | box | a woodworker’s repair box | Normal / Standard / 2400g / 62m / skin | `wood` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Wood \| Container_Pouch | This small wooden box holds pegs, wedges, resin glue, clamps, cord, and a few small shaping tools. The lid is scarred by use as a work surface. It is meant for ordinary furniture, handles, and wooden gear. |
| `medieval_repair_joiner_repair_chest` | chest | a joiner’s fine repair chest | Large / Good / 6200g / 150m / skin | `oak` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Wood_Good \| Container_Pouch | A well-fitted oak chest contains matched pegs, wedges, glue cakes, clamps, chisels, and smoothing tools. The compartments are tidy and labelled by shape rather than writing. It is built for careful repair of valued wooden goods. |
| `medieval_repair_pot_menders_pouch` | pouch | a pot-mender’s ceramic repair pouch | Small / Standard / 900g / 28m / skin | `ceramic` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Ceramic \| Container_Pouch | This pouch contains cord clamps, resin, spare sherds, and small smoothing stones for cracked pots. The contents are humble and dusty. It looks like something carried by a travelling mender rather than a wealthy workshop. |
| `medieval_repair_fine_ceramic_mending_case` | case | a fine ceramic mending case | Small / Good / 1050g / 64m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Ceramic_Good \| Container_Pouch | A padded case contains better resin, fine clamps, smooth sherd patches, and polishing cloths for glazed wares. The supplies are arranged to avoid scratching fragile surfaces. It is a careful kit for good household vessels. |
| `medieval_repair_stone_mason_patch_kit` | kit | a stone mason’s patch kit | Normal / Standard / 2600g / 50m / skin | `stone` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Stone \| Container_Pouch | A heavy kit holds wedges, stone dust, resin, small chisels, and rough cloth for patching chipped stone goods. It is too weighty for delicate work but sturdy enough for everyday repairs. The pouch is powdered grey inside. |
| `medieval_repair_bone_horn_mending_roll` | roll | a bone-and-horn mending roll | Small / Standard / 680g / 42m / skin | `bone` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Hard_Organic \| Container_Pouch | A cloth roll holds small drills, sinew, resin, horn shavings, and polished bone slips. It is meant for combs, handles, fittings, and other hard organic pieces. The smell is dry, sharp, and faintly gluey. |
| `medieval_repair_fine_hard_organic_case` | case | a fine hard-organic repair case | Small / Good / 720g / 88m / skin | `horn` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Hard_Organic_Good \| Container_Pouch | This neat case contains matched horn slivers, bone patches, fine cord, and smooth polishing tools. Each small piece is wrapped so it does not scratch the next. It is meant for valued horn, bone, shell, or antler goods. |
| `medieval_repair_blacksmith_metal_kit` | kit | a blacksmith’s metal repair kit | Normal / Standard / 3100g / 70m / skin | `wrought iron` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal \| Container_Pouch | A heavy kit contains small clamps, rivets, wire, wedges, and dark repair stock for common metal goods. The pouch is scorched in places and leaves a soot smell on the hands. It is practical rather than portable elegance. |
| `medieval_repair_metal_tool_repair_roll` | roll | a metal tool repair roll | Normal / Standard / 1800g / 76m / skin | `leather` | Market / Repair Supplies / General Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal_Tool \| Container_Pouch | This roll carries rivets, wedges, pins, a file, binding wire, and small grips for repairing metal tools. The leather is dark with oil and the file has its own protective flap. It is meant for tools, not weapons or armour. |
| `medieval_repair_fine_metal_tool_case` | case | a fine metal tool repair case | Normal / Good / 2100g / 140m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal_Tool_Good \| Container_Pouch | A sturdy case holds better files, fitted rivets, small clamps, and polished metal pins. Everything sits in tight loops so the edges do not damage the leather. It is a workshop-quality kit for valued tools. |
| `medieval_repair_armourers_field_kit` | kit | an armourer’s field repair kit | Normal / Standard / 2600g / 95m / skin | `leather` | Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal_Armour \| Container_Pouch | This heavy kit holds rivets, leather lacing, wire, buckles, and a small hammer wrapped in cloth. It is designed for damaged armour on campaign. The outside bears dents and dark metal stains. |
| `medieval_repair_weapon_mending_roll` | roll | a weapon-mending repair roll | Normal / Standard / 1700g / 85m / skin | `leather` | Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal_Weapon \| Container_Pouch | A leather roll contains wedges, pins, grip cord, rivets, and small metal fittings for weapon repairs. The tools are compact and hard-worn. It is aimed at keeping field weapons usable, not forging new blades. |
| `medieval_repair_fine_weapon_mending_case` | case | a fine weapon-mending case | Normal / Good / 2100g / 160m / skin | `leather` | Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Metal_Weapon_Good \| Container_Pouch | This fitted case holds fine rivets, polished pins, strong binding wire, grip leather, and small finishing tools. The supplies are carefully sorted and better made than a field kit. It belongs to a serious armoury or weapon shop. |
| `medieval_repair_glass_poor_lead_patch_pouch` | pouch | a rough glass patching pouch | Small / Poor / 850g / 30m / skin | `lead` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Glass_Poor \| Container_Pouch | This poor pouch holds scrap lead came, resin, cloth pads, and a few cloudy glass chips. It is enough for crude patches on small panes or vessels. The contents clink softly through the padding. |
| `medieval_repair_glass_menders_case` | case | a glass-mender’s repair case | Normal / Standard / 1500g / 85m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Glass \| Container_Pouch | A padded case contains lead strips, resin, smooth cloth, spare glass pieces, and small setting tools. The compartments keep hard edges away from the glass. It is a standard kit for fragile vessels and panes. |
| `medieval_repair_fine_glass_menders_case` | case | a fine glass-mender’s case | Normal / Good / 1700g / 160m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Glass_Good \| Container_Pouch | This carefully padded case holds clean lead, fine resin, polished cloth, and clearer glass repair pieces. Every fragile part is wrapped separately. It is meant for costly glassware and careful shop work. |
| `medieval_repair_poor_paper_patch_packet` | packet | a poor paper patch packet | VerySmall / Poor / 160g / 10m / skin | `paper` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Paper_Poor \| Container_Pouch | This packet contains rough scraps of paper, thin paste, and a little smoothing stick. The materials are mismatched, but they can patch a torn document or wrapper. Paste stains have stiffened one corner of the packet. |
| `medieval_repair_document_repair_pouch` | pouch | a document repair pouch | Small / Standard / 360g / 38m / skin | `parchment` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Paper \| Container_Pouch | A small pouch carries paper and parchment patches, paste, thread, and a polished bone smoother. The tools are light and carefully wrapped. It is meant for everyday repair of records, labels, and book leaves. |
| `medieval_repair_scriptorium_repair_case` | case | a scriptorium repair case | Small / Good / 520g / 96m / skin | `parchment` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Paper_Good \| Container_Pouch | This fine case holds sorted parchment patches, smooth paste, linen thread, a bone folder, and small weights. The contents are dry, clean, and carefully protected from damp. It suits a scriptorium or chancery more than a road kit. |
| `medieval_repair_lacquer_patch_box` | box | a lacquerware repair box | Small / Standard / 680g / 70m / skin | `lacquer` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Lacquer \| Container_Pouch | A small wooden box contains resin, lacquer scrap, fine cloth, and a soft brush wrapped in paper. The inside is stained dark and glossy from old repairs. It is meant for bowls, boxes, and cases with lacquered surfaces. |
| `medieval_repair_fine_lacquer_case` | case | a fine lacquer repair case | Small / Good / 760g / 145m / skin | `lacquer` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Lacquer_Good \| Container_Pouch | This lacquered case holds wrapped brushes, polished cloth, resin, and carefully kept lacquer stock. The supplies are protected from dust and sorted by colour and finish. It is a specialist kit for valued lacquerware. |
| `medieval_repair_rope_splicing_pouch` | pouch | a rope-splicing repair pouch | Small / Standard / 720g / 32m / skin | `hemp` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Cordage \| Container_Pouch | A tough pouch holds a wooden fid, wax, hemp fibres, and short lengths of cord. Tar and rope dust darken the seams. It is made for repairing lines, nets, and heavy cordage. |
| `medieval_repair_shipwright_cordage_case` | case | a shipwright’s cordage repair case | Normal / Good / 1500g / 90m / skin | `hemp` | Market / Repair Supplies / Specialist Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Cordage_Good \| Container_Pouch | This larger case contains several fids, wax, tarred line, spare hemp, and binding cord. It smells of rope, pitch, and sea air. The tools are chosen for serious cordage work rather than small household string. |
| `medieval_repair_rough_composite_bow_pouch` | pouch | a rough composite-bow repair pouch | Small / Poor / 520g / 40m / skin | `sinew` | Market / Repair Supplies / Specialist Repair Supplies \| Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Composite_Bow_Poor \| Container_Pouch | This small pouch holds sinew, horn scraps, resin glue, and tight wrapping cord. The stock is crude and irregular, but chosen for emergency bow repairs. It smells of glue, hide, and old horn. |
| `medieval_repair_composite_bow_repair_roll` | roll | a composite-bow repair roll | Normal / Standard / 1250g / 110m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies \| Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Composite_Bow \| Container_Pouch | A leather roll carries sinew backing, horn shims, glue cakes, clamps, and binding thread. The pieces are slim and carefully protected from damp. It is made for maintaining composite bows, not ordinary wooden self bows. |
| `medieval_repair_master_bowyer_repair_case` | case | a master bowyer’s repair case | Normal / Good / 1500g / 220m / skin | `leather` | Market / Repair Supplies / Specialist Repair Supplies \| Market / Repair Supplies / Weapon and Armour Repair Supplies | Holdable \| Destroyable_Misc \| Repair_Composite_Bow_Good \| Container_Pouch | This fine case holds sorted horn slips, prepared sinew, resin glue, wrapping silk, and small clamps. The contents are dry, orderly, and expensive-looking. It is a specialist repair case for valued composite bows. |

## 4. Implementation validation checklist

Before implementing these rows as seeder code, verify the following:

- Every `unique_reference` remains lowercase snake case and globally unique.
- Every component in the catalogue exists in `Seeded_Item_Components.json`.
- Every tag in the catalogue exists in `SeededTagHierarchy.csv`.
- Every primary material exists in `Seeded_Materials.json` or in the core medical material additions: `alum`, `ephedra`, `foxglove`, `gut`, `henbane`, `mandrake`, `yarrow`.
- No row uses modern medical language, modern drug wrappers, tobacco wrappers, `Treatment_AdminUnlimited`, or `Repair_Universal*`.
- No row combines multiple components from the same exclusive runtime interface family.
- Every ordinary portable row includes `Holdable`; every ordinary row has an appropriate destroyable component.
- Filled medicine-vessel rows use the vessel’s empty inherent weight, while their cost includes the value of the loaded contents.
- Fumigation fuel packets carry only the `Fumigation Stock` tag and inert components; drug mechanics remain on `IncenseBurner_*` rows.
