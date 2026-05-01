# RPI Item Conversion Mapping

## Purpose

The converter now treats the archived `objs.*` files as the authoritative source for pass-one item reconstruction. It parses the original RPI object records into typed payloads, then maps those records onto the closest seeded FutureMUD component combinations without introducing importer-only runtime behaviour.

## Parser Model

Each object prototype is loaded into `RpiItemRecord`, which preserves:

- provenance: source file, zone, vnum, and stable `SourceKey`
- raw header flags: type, extra bits, wear bits
- raw `Oval0`-`Oval5` values and the numeric tail
- optional `desc_keys` and ink colour strings
- all affect rows, clan rows, poison rows, and extra descriptions
- parse warnings and any unparsed legacy trailer lines

Typed payloads are derived on top of the raw values so the importer can make functional decisions without losing the original source semantics.

## Conversion Outcomes

Three outcomes are used by `FutureMUDItemTransformer`:

- `FunctionalImport`: the item maps to a meaningful FutureMUD component combination
- `PropImport`: the item becomes a tagged prop because the first pass cannot safely infer full behaviour
- `DeferredBehaviorPropImport`: the item has likely gameplay behaviour beyond the current importer pass, so it is preserved as a tagged prop with explicit warnings

## Functional Families

The current pass aims to import the following families functionally:

- wearables and armour
- melee weapons, bows, sling-like weapons, ammunition, shields
- containers, quivers, sheaths, keyrings
- lights, lanterns, drink containers, fountains
- food, keys, repair kits, locksmithing tools
- parchment and books
- writing implements when compatible stock components exist

## Mapping Heuristics

### Wearables

- Wear bits drive the primary wearable slot selection.
- `desc_keys`, keywords, and descriptions refine the slot choice.
- Known seeded wearable names are preferred, for example `Wear_Skullcap`, `Wear_Tunic`, `Wear_Hauberk`, `Wear_Gauntlets`, and `Wear_Shoes`.
- Blindfold and belt-like behaviour add supporting components when appropriate.

### Armour

- Wear-slot mapping is resolved first.
- `Oval0` and `Oval1` are interpreted as armour value and armour family.
- RPI armour families map broadly as:
  - Fur -> `Armour_HeavyClothing`
  - Quilted -> `Armour_UltraHeavyClothing`
  - Leather -> `Armour_BoiledLeather`
  - Scale -> `Armour_MetalScale`
  - Mail -> `Armour_Chainmail`
  - Plate -> `Armour_Platemail`

### Weapons

- Weapon skill, keywords, handedness, and thrown flags pick the nearest seeded FutureMUD weapon component.
- Existing stock names such as `Melee_Longsword`, `Melee_Axe`, `Melee_Short Spear`, `Longbow`, `Crossbow`, and `Throwing_Knife` are used rather than inventing converter-only component names.

### Containers and Carriers

- Ordinary containers map by keyword and broad capacity to stock container components.
- Quivers, sheaths, and keyrings map to their dedicated stock components.
- Lockable containers add a locking container component when the archived data indicates lock behaviour.

### Liquids and Lights

- Lantern-like descriptions prefer `Lantern`; otherwise torches default to `Torch_1Hour`.
- Drink containers and fountains resolve a FutureMUD liquid reference when the archived liquid id maps cleanly.
- The raw liquid id is always preserved in the exported data even when the FutureMUD liquid name cannot be resolved.

### Food and Tools

- Food uses the first compatible seeded food component exposed by the baseline catalog.
- Repair kits use the archived repair target type plus quality heuristics to choose components such as `Repair_Metal_Armour`, `Repair_Metal_Weapon`, or `Repair_Cloth`.
- Lockpicks use the existing locksmithing component family when available.

### Writing Media

- Parchment and books use stock paper and book component families.
- Writing implements try known stock components such as `Biro_Black`, `Biro_Blue`, `Biro_Red`, and `Pencil_Black`.
- Standalone ink currently falls back to a prop because this pass does not assume a dedicated seeded ink reservoir component exists.

## Deferred and Prop Families

The following families intentionally stay out of functional import in pass one:

- tickets, merchant tickets, room rentals
- NPC-only objects and dwellings
- skulls and similarly behaviour-heavy corpse-adjacent items
- low-confidence commodity and prop families such as treasure, trash, ore, herbs, spices, salves, poison, timber, cloth, ingots, and similar resource items

These still preserve provenance, raw ovals, warnings, and any optional source metadata in export output.

## CLI Modes

`Program.cs` now exposes three runner modes:

- `analyze-items`
  - parses the corpus
  - runs conversion
  - optionally validates against a seeded FutureMUD baseline
  - prints per-type totals, per-status totals, warning codes, and missing dependency summaries
- `export-items`
  - emits stable JSON containing both the parsed source record and the converted definition
- `apply-items`
  - validates converted items against the baseline catalog
  - defaults to dry-run unless `--execute` is supplied
  - stamps imported items with an `RPIIMPORT|...` provenance marker in `EditableItem.BuilderComment`

## Notes

- Baseline catalog lookups tolerate duplicate seeded component, material, tag, liquid, and trait names by choosing the lowest-ID row deterministically.
- Missing seeded dependencies are surfaced as validation issues rather than silently dropped.
- The importer deliberately avoids creating new runtime behaviour; if a mapping needs a runtime item system that FutureMUD does not already expose, the item remains a prop in this pass.
- Export JSON is the recommended interchange format if this work later migrates into code generation for `DatabaseSeeder`.
