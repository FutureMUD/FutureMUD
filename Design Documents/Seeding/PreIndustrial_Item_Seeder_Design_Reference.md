# Pre-Industrial Shared Item Seeder Design Reference

## Scope

The shared pre-industrial baseline is the reusable item layer for the Antiquity, Medieval, Renaissance, and Early Modern selections in `ItemSeeder`. It allows later eras to reuse persistent workshop, writing, trade, civic, and military-support forms without renaming or directly seeding broad earlier-era catalogues.

The implementation preserves all existing `antiquity_*`, `medieval_*`, `historic_*`, and `primary_production_*` stable references. Cross-era compatibility rows use new `preindustrial_*` stable references and source-attribution builder notes.

## Dispatch and repeatability

`SeedReworkItems()` calls `SeedSharedPreIndustrialBaselineItems()` when the selected era string contains any of:

- `antiquity`
- `medieval`
- `renaissance`
- `earlymodern`

The shared entrypoint seeds the established `historic_*` workshop foundation and `primary_production_*` tools before the `preindustrial_*` catalogue. Renaissance and Early Modern retain separate era-specific entrypoints for future content.

All rows use the normal stable-reference `CreateItem(...)` path. Existing prototypes are found by case-insensitive `UniqueName`, updated with missing stock metadata, and reused, so rerunning one era or moving between supported eras does not create duplicate shared rows.

## Alias catalogue

The baseline installs 342 compatibility aliases:

- 39 writing surfaces, books, ledgers, and implements
- 147 non-regional trade containers and lockboxes
- 17 doors and general clothing accessories
- 77 workshop fixtures and cross-period tools
- 52 sheaths, quivers, belts, weapon racks, and armour-display supports
- 10 antiquity timekeeping and water/civic fixtures

The complete source-to-alias mapping is maintained in [Pre-Industrial Item Seeder Alias Catalogue](./PreIndustrial_Item_Seeder_Alias_Catalogue.md).

Alias rows copy the source item's form, material, size, quality, weight, cost, components, and functional/market tags. Antiquity and Medieval era tags are replaced with `Era / Pre-Industrial Era`. Lifecycle references between wrapped items are rewritten to their shared aliases so a pre-industrial lit fixture does not morph into a medieval-only prototype.

The builder note format is:

```text
Shared pre-industrial alias derived from <source stable reference>; original <source era> reference retained for compatibility.
```

## New shared stock

The baseline also provides the following 44 requested shared stock names:

- 11 printing and paper-administration forms
- 12 navigation, surveying, optical, and scientific forms
- 10 gunpowder-support forms
- 11 global-trade packaging forms

`preindustrial_trade_spice_chest` is represented by the compatibility alias of `medieval_locking_trade_spice_chest`. The broad trade-alias rule and the new-stock list assign the same stable reference to those two concepts; using the source-derived lockable chest preserves idempotency, supplies real container behaviour, and avoids a duplicate unique name.

## Component boundaries

New printing, navigation, optical, and gunpowder-support forms do not claim mechanics that the component catalogue does not implement. Unsupported specialist tools are descriptive `Holdable` props. Paper items use writing/paper components only where the existing pre-modern component is appropriate. Packaging uses existing container components.

No firearms, cartridges, bombs, explosives, or ammunition-component rows are introduced by this baseline. Powder horns, flasks, pouches, match cord, wadding, moulds, and cleaning tools are support goods only.

Fixed fixtures omit `Holdable`. Source-derived movable/installable fixtures preserve the source catalogue's component convention.

## Tags and source dependencies

`UsefulSeeder` owns the shared `Pre-Industrial Era` and `Early Modern Era` tags. It also owns the timekeeping, civic-fixture, primary-production-tool, and component prototypes consumed by this baseline. The maintained catalogues are synchronized in:

- `Design Documents/Data/SeededTagHierarchy.csv`
- `Design Documents/Data/Seeded_Item_Components.json`

The antiquity watch candle alias uses the canonical seeded `beeswax` material rather than the older component-gap source's dynamically-created `wax` name.

## Explicit exclusions

The baseline does not promote:

- castle-specific portcullises, tower-stair doors, or town-gate leaves
- medieval underclothing or silhouette-specific garments
- medieval weapons or armour as combat equipment
- regional `medieval_regional_*` containers
- culture-specific or colonial/contact-zone packages as universal Early Modern stock

## Verification contract

`PreIndustrialBaselineTests` checks:

- all four supported era selections dispatch the shared baseline
- alias and new stable references are unique and lowercase underscore identifiers
- all 342 alias sources remain present
- all 44 requested new stock names exist
- every material, tag, and component maps to maintained seeded source truth
- portable new rows are holdable and fixed fixtures are not
- alias lifecycle targets do not fall back to medieval rows
- gunpowder-support rows have no firearm, ammunition, bomb, or explosive components
- medieval writing tag/component strings contain no backticks

