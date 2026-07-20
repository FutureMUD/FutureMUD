# FutureMUD Renaissance Household Furniture Expansion Catalogue Index — Volume I

## Scope

This index governs **200 proposed prototypes** and carries the shared row, component-code, tag-code, and culture-admission contract for its linked section catalogues. The item rows are split by furniture/container family for maintainability; together the linked sections are the complete volume.

No row is omitted or duplicated by the split.
## Row contract

Rows are implementation-oriented catalogue targets rather than full descriptions. Every row inherits `Era / Renaissance Era` and the exact culture tag represented by its culture code. Public short descriptions remain form-based; the culture labels are builder-facing admission metadata.

A row is:

```text
stable_reference|sdesc|material|size/quality|empty_weight_g/cost_farthings|component_code|culture_code|tag_codes|variation_basis
```

Portable folding furniture includes `Holdable`. Fixed furniture omits it. Each row has exactly one container provider; table and bench behaviour may coexist because they do not provide another container interface.

Size codes: `T=Tiny`, `VS=VerySmall`, `S=Small`, `N=Normal`, `L=Large`, `VL=VeryLarge`, `H=Huge`, `EN=Enormous`.

Quality codes: `P=Poor`, `SS=Substandard`, `S=Standard`, `G=Good`, `VG=VeryGood`, `GR=Great`, `E=Excellent`.

Costs are in farthings and weights are empty grams.

## Component codes

| Code | Exact components | Intended behaviour |
| --- | --- | --- |
| `FX01` | `Destroyable_Furniture`; `Container_Wardrobe` | fixed wardrobe storage |
| `FX02` | `Destroyable_Furniture`; `Container_Armoire` | fixed armoire storage |
| `FX03` | `Destroyable_Furniture`; `LockingContainer_PreIndustrial_LargeCabinet` | fixed built-in-lock large cabinet |
| `FX04` | `Destroyable_Furniture`; `Container_Armor_Stand` | fixed furniture-grade armour stand |
| `FX05` | `Destroyable_WoodenHeavy`; `Container_Armor_Stand` | fixed heavy wooden armour stand |
| `FX06` | `Destroyable_HeavyMetal`; `Container_Armor_Stand` | fixed heavy-metal armour stand |
| `FX07` | `Destroyable_Furniture`; `Container_Weapon_Rack` | fixed furniture-grade weapon rack |
| `FX08` | `Destroyable_WoodenHeavy`; `Container_Weapon_Rack` | fixed heavy wooden weapon rack |
| `FX09` | `Destroyable_HeavyMetal`; `Container_Weapon_Rack` | fixed heavy-metal weapon rack |
| `FX10` | `Destroyable_Furniture`; `LockingContainer_PreIndustrial_LargeCabinet` | fixed built-in-lock weapon cabinet |
| `FX11` | `Destroyable_Furniture`; `Container_Display_Case` | fixed transparent display case |
| `FX12` | `Destroyable_Furniture`; `Container_Glass_Cabinet` | fixed glass-fronted cabinet |
| `FX13` | `Destroyable_Furniture`; `Container_Display_Shelves` | fixed open display shelving |
| `FX14` | `Destroyable_Furniture`; `LockingContainer_PreIndustrial_DisplayCabinet` | fixed transparent built-in-lock display cabinet |
| `FX26` | `Destroyable_Furniture`; `Container_Cupboard` | fixed cupboard storage |
| `FX30` | `Destroyable_Furniture`; `Container_Large_Cabinet` | fixed large cabinet storage |
| `FX31` | `Destroyable_Furniture`; `Container_Small_Cabinet` | fixed small cabinet storage |
| `FX33` | `Destroyable_Furniture`; `LockingContainer_PreIndustrial_SmallCabinet` | fixed built-in-lock small cabinet |
| `FX34` | `Holdable`; `Destroyable_Furniture`; `Container_Weapon_Rack` | portable weapon rack |
| `FX35` | `Holdable`; `Destroyable_Furniture`; `Container_Armor_Stand` | portable armour stand |
| `FX39` | `Holdable`; `Destroyable_Furniture`; `Container_Wardrobe` | portable wardrobe |

## Tag codes

| Code | Exact tags added by the row |
| --- | --- |
| `FU-S` | `Functions / Container`; `Functions / Household Items / Household Furniture`; `Market / Household Goods / Simple Furniture` |
| `FU-N` | `Functions / Container`; `Functions / Household Items / Household Furniture`; `Market / Household Goods / Standard Furniture` |
| `FU-L` | `Functions / Container`; `Functions / Household Items / Household Furniture`; `Market / Household Goods / Luxury Furniture` |
| `TR-S` / `TR-N` / `TR-L` | Container plus simple, standard, or luxury household-wares market tag |
| `PC-S` / `PC-N` / `PC-L` | Personal container plus simple, standard, or luxury household-wares market tag |
| `DW-S` / `DW-N` / `DW-L` | Domestic container/ware plus simple, standard, or luxury household-wares market tag |
| `OPEN` | `Functions / Container / Open Container` |
| `WATER` | `Functions / Container / Watertight Container` |
| `POROUS` | `Functions / Container / Porous Container` |
| `MIL` | `Market / Military Goods` |
| `COURT`, `RELIGIOUS`, `GUILD`, `MARITIME`, `PERFORMANCE`, `SERVICE` | Matching live `Institution / ...` tag |

## Culture codes and admission

| Code | Culture grouping | Exact inherited tag | Date gate | New rows |
| --- | --- | --- | ---: | ---: |
| `WER` | Western European Renaissance | `Culture / Renaissance / Shared / Western European Renaissance` | 1450-1600 | 37 |
| `IBA` | Iberian Atlantic | `Culture / Renaissance / Shared / Iberian Atlantic` | 1450-1600 | 30 |
| `CEN` | Central European | `Culture / Renaissance / Shared / Central European` | 1450-1600 | 29 |
| `NBA` | Northern Baltic | `Culture / Renaissance / Shared / Northern Baltic` | 1450-1600 | 19 |
| `CEF` | Central Eastern Frontier | `Culture / Renaissance / Shared / Central Eastern Frontier` | 1500-1600 | 19 |
| `EON` | Eastern Orthodox Northern | `Culture / Renaissance / Shared / Eastern Orthodox Northern` | 1500-1600 | 19 |
| `OTT` | Ottoman Islamicate | `Culture / Renaissance / Shared / Ottoman Islamicate` | 1450-1600 | 31 |
| `PIP` | Persianate Indo-Persian | `Culture / Renaissance / Shared / Persianate Indo-Persian` | 1450-1600 | 28 |
| `SAS` | South Asian | `Culture / Renaissance / Shared / South Asian` | 1450-1600 | 28 |
| `EAL` | East Asian Literati | `Culture / Renaissance / Shared / East Asian Literati` | 1400-1600 | 33 |
| `JPN` | Japanese | `Culture / Renaissance / Shared / Japanese` | 1450-1600 | 29 |
| `MEA` | Maritime East Asian | `Culture / Renaissance / Shared / Maritime East Asian` | 1450-1600 | 18 |
| `SEM` | South-east Asian Mainland | `Culture / Renaissance / Shared / South-east Asian Mainland` | 1450-1600 | 18 |
| `MSEA` | Maritime South-east Asian | `Culture / Renaissance / Shared / Maritime South-east Asian` | 1450-1600 | 23 |
| `STP` | Steppe and Caravan | `Culture / Renaissance / Shared / Steppe and Caravan` | 1450-1600 | 19 |
| `ACA` | African Court Atlantic | `Culture / Renaissance / Shared / African Court Atlantic` | 1450-1600 | 23 |
| `SAI` | Sahelian Islamic | `Culture / Renaissance / Shared / Sahelian Islamic` | 1450-1600 | 17 |
| `RSE` | Red Sea | `Culture / Renaissance / Shared / Red Sea` | 1450-1600 | 17 |
| `IND` | Indian Ocean | `Culture / Renaissance / Shared / Indian Ocean` | 1450-1600 | 23 |
| `MES` | Mesoamerican | `Culture / Renaissance / Shared / Mesoamerican` | 1400-1600 | 19 |
| `AND` | Andean | `Culture / Renaissance / Shared / Andean` | 1400-1600 | 19 |
| `CAR` | Caribbean Contact | `Culture / Renaissance / Shared / Caribbean Contact` | 1450-1600 | 15 |
| `NAC` | North American Contact | `Culture / Renaissance / Shared / North American Contact` | 1450-1600 | 15 |
| `COL` | Colonial Atlantic | `Culture / Renaissance / Shared / Colonial Atlantic` | 1500-1600 | 28 |
| `MAR` | Global Maritime | `Culture / Renaissance / Shared / Global Maritime` | 1450-1600 | 44 |

## Catalogue sections

| Section | Rows | File |
| --- | ---: | --- |
| Wardrobes, armoires, garment cabinets, and linen presses | 55 | [FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Wardrobes.md](./FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Wardrobes.md) |
| Armour stands, armour trees, and harness supports | 45 | [FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Armour_Stands.md](./FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Armour_Stands.md) |
| Weapon racks, shield racks, and lockable armoury cabinets | 45 | [FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Weapon_Racks.md](./FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Weapon_Racks.md) |
| Display cabinets, cases, and open display shelving | 55 | [FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Display.md](./FutureMUD_Renaissance_Household_Furniture_Expansion_Catalogue_I_Display.md) |

**Volume total: 200 rows.**

## Index validation

- The linked files contain exactly 200 rows in the declared family split.
- Stable references and short descriptions are unique across the complete 600-row expansion.
- Component codes, tag codes, culture codes, size codes, and quality codes resolve through this index.
