# Pre-Industrial Shared Item Seeder Design Reference

## Scope

The shared pre-industrial baseline is the reusable item layer for the Antiquity, Medieval, Renaissance, and Early Modern selections in `ItemSeeder`. It allows later eras to reuse persistent workshop, writing, trade, civic, and military-support forms without renaming or directly seeding broad earlier-era catalogues.

The implementation preserves all existing `antiquity_*`, `medieval_*`, `historic_*`, and `primary_production_*` stable references. Cross-era compatibility rows use new `preindustrial_*` stable references and retain source attribution in the seeder definition and manifest without adding stock-only builder comments.

## Dispatch and repeatability

`SeedReworkItems()` calls `SeedSharedPreIndustrialBaselineItems()` when the selected era string contains any of:

- `antiquity`
- `medieval`
- `renaissance`
- `earlymodern`

The shared entrypoint seeds the established `historic_*` workshop foundation and `primary_production_*` tools before the `preindustrial_*` catalogue. Renaissance and Early Modern retain separate era-specific entrypoints for future content.

All rows use the normal stable-reference `CreateItem(...)` path. Existing prototypes are found by case-insensitive `UniqueName`, updated with missing stock metadata, and reused, so rerunning one era or moving between supported eras does not create duplicate shared rows.

Established installs may choose the Items seeder's `blackpowder` repair scope to reconcile the shared firearm and artillery support rows, the selected Renaissance or Early Modern weapon rows, and the physical gunpowder craft without walking the unrelated catalogue. This focused mode retains the same manifest ownership checks and transaction boundary as a full run, and does not retire records outside its selected slice. It may add the new functional tags to customized legacy stock, but does not overwrite any other customized field or relationship.

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

The source-attribution field retained in the seeder definition is:

```text
Shared pre-industrial alias derived from <source stable reference>; original <source era> reference retained for compatibility.
```

This field is not persisted into `GameItemProto.BuilderNotes`. A rerun removes an older seeded copy of the line while preserving unrelated builder-authored notes.

## New shared stock

The baseline also satisfies 49 requested shared stock concepts:

- 11 printing and paper-administration forms
- 12 navigation, surveying, optical, and scientific forms
- 15 firearm- and artillery-support forms
- 11 global-trade packaging forms

`preindustrial_trade_spice_chest` is represented by the compatibility alias of `medieval_locking_trade_spice_chest`. The broad trade-alias rule and the new-stock list assign the same stable reference to those two concepts; using the source-derived lockable chest preserves idempotency, supplies real container behaviour, and avoids a duplicate unique name.

Consequently, the live admission inventory contains 390 unique stable references: 342 compatibility aliases and 48 shared-authored prototypes.

## Era admission manifests

Installing the shared catalogue makes prototypes available to builders; it does not make every prototype ordinary in every era or culture. The populated admission manifests record one explicit decision for every live shared stable reference:

- [Medieval Shared Baseline Admission Manifest](./FutureMUD_Medieval_Shared_Baseline_Admission_Manifest.md)
- [Renaissance Shared Baseline Admission Manifest](./FutureMUD_Renaissance_Shared_Baseline_Admission_Manifest.md)
- [Early Modern Shared Baseline Admission Manifest](./FutureMUD_EarlyModern_Shared_Baseline_Admission_Manifest.md)

Each manifest contains 390 unique rows and records culture/contact scope, date window, admitting context, availability, trade/contact status, and component reality. `Not admitted` is a completed historical decision. The manifests are regenerated from live source truth with `scripts/generate-preindustrial-admission-manifests.ps1`; `-Check` fails when a committed manifest drifts from the alias catalogue, authored item specifications, or admission rules.

## Component boundaries

New printing, navigation, and optical forms do not claim mechanics that the component catalogue does not implement. Firearm and artillery support rows use the canonical functional tags consumed by the physical loading drill; unsupported specialist tools remain descriptive `Holdable` props. Paper items use writing/paper components only where the existing pre-modern component is appropriate. Packaging uses existing container components.

No firearms, cartridges, bombs, explosives, or ammunition-component rows are introduced by this baseline. Powder horns, flasks, pouches, match cord, musket and artillery wadding, fuses, moulds, rods, sponges, vent tools, and linstocks are support goods only.

Fixed fixtures omit `Holdable`. Source-derived movable/installable fixtures preserve the source catalogue's component convention.

## Tags and source dependencies

`UsefulSeeder` owns the shared `Pre-Industrial Era` and `Early Modern Era` tags. It also owns the timekeeping, civic-fixture, primary-production-tool, and component prototypes consumed by this baseline. The maintained catalogues are synchronized in:

- `Design Documents/Data/SeededTagHierarchy.csv`
- `Design Documents/Data/Seeded_Item_Components.json`

The antiquity watch candle alias uses the canonical seeded `beeswax` material rather than the older component-gap source's dynamically-created `wax` name.

## Jewellery and door catalogue extension

`SeedSharedPreIndustrialJewelleryAndDoorHardware()` now adds 60 genuinely cross-era jewellery/devotional prototypes and 90 portable ordinary door, lock, key, latch, and fitting prototypes. They are catalogued in [Pre-Industrial Jewellery and Doors](./FutureMUD_PreIndustrial_Jewellery_Doors_Design_Reference.md) and are deliberately separate from the 390-row admission-manifest inventory, which remains the compatibility-alias and prior named-stock inventory.

The extension supplies only portable forms with existing wearable, door, warded-lock, key, and latch component profiles. It does not create an installed exit, a matched-key system, castle/town-gate leaves, or a portcullis; the loose `Latch_Portcullis_Pawl` fitting is retained as supported workshop hardware only. Its 60/90 rows contribute to the same four-era availability contract as the later Renaissance-owned common layer.

## Explicit exclusions

The baseline does not promote:

- castle-specific portcullises, tower-stair doors, or town-gate leaves
- medieval underclothing or silhouette-specific garments
- medieval weapons or armour as combat equipment
- regional `medieval_regional_*` containers
- culture-specific or colonial/contact-zone packages as universal Early Modern stock

## Verification contract

`PreIndustrialBaselineTests` and `PreIndustrialAdmissionManifestTests` check:

- all four supported era selections dispatch the shared baseline
- alias and new stable references are unique and lowercase underscore identifiers
- all 342 alias sources remain present
- all 49 requested new stock names exist
- every material, tag, and component maps to maintained seeded source truth
- portable new rows are holdable and fixed fixtures are not
- alias lifecycle targets do not fall back to medieval rows
- gunpowder-support rows have no firearm, ammunition, bomb, or explosive components
- medieval writing tag/component strings contain no backticks
- all three era admission manifests contain the exact 390-row live inventory
- the jewellery/door extension is checked independently against its generated catalogue, description, dependency, culture-admission, and direct-craft contracts
- every admission record has all required decision fields and the correct source
- historically sensitive printing, firearms, telescope, and commodity-package gates remain explicit
- generated manifests contain no policy-template or incomplete-work placeholders

