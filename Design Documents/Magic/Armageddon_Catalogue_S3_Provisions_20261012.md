# Armageddon S3 provisions and native decisions — 12 October 2026

This continues the [first S3 catalogue checkpoint](Armageddon_Catalogue_S3_Checkpoint_20261012.md), with the user's [native decisions](Armageddon_Catalogue_S3_Decisions_20261012.md) applied. S3 remains in progress; this is a qualified provision batch, not the completed creature/catalogue gate. Normal interactive installation still performs S1/S2, and full package/Release activation remains disabled.

## Implemented stock

Three approved Sustain Meal fallback prototypes use native Food components: flatbread, dried fruit and sustaining stew. Each weighs 0.25 kg, has five bites, supplies five native satiation hours and 0.1 litre of water. The 500-calorie source value is translated at the approved 2,400/day reference; no runtime calorie field or model is introduced. Partial and exhausted component reloads retain the saved bite count.

The native closable supply bottle holds one litre and starts empty. Its native Liquid Container component has no default liquid, so reload does not refill it. Stock red wine is a real native Liquid with 0.88 litre water and 0.12 litre alcohol per litre, and density 990 kg/m³. Water remains a retained typed alias to the installed native liquid; the package does not claim or overwrite that shared liquid. These are prototypes/definitions only; no live items or player nutrition are installed.

The ten foci and two device blanks, with their twelve recipes, have an explicit S4 disposition in the catalogue. Real spell dependencies will be bound alongside the S4 spell definitions. The existing component/focus blanks and binding seal remain S3 supplies. Disease uses native infections; no separate disease-profile subsystem or fabricated disease entity is created. Type expansion, if needed for the approved symptoms, belongs to the native infection implementation.

## Shared liquid mass correction

Provision qualification found that LiquidMixture multiplied litre-based volume by density stored in kg/m³ without the cubic-metre conversion. One litre of water therefore contributed 1,000 kg rather than one kg. The shared calculation now converts litres to m³ before converting kilograms to native weight units. Wine at the approved density contributes 0.99 kg per litre, including its native container component weight.

This changes the computed mass of existing fluid contents to the documented physical units. Container, puddle, fuel and other fluid-bearing components use the same value. Mass is recomputed from saved volume and density; no migration or rewrite of stored constituents is required. Tests cover water/wine/dense fluids, different base-unit configurations, volume changes and saved-mixture reload.

## Ownership and qualification

The provision batch uses the existing explicit catalogue ownership whitelist, typed aliases and three-way stock reconciler. It commits atomically with ownership, retains native IDs/revisions and builder edits, and refuses incompatible/retired/missing claims. Shared material renaming retains its bound ID. Liquid scalar fields are explicitly enumerated rather than discovered by name or ID text scans.

The final Debug run `20261011T223827Z-691b499e82a0` passed **8,906 tests**, with zero failures and three platform skips: Seeder 1,680, Library 763 and Core 6,463. Release Seeder and Debug native-harness builds passed with zero errors (144 and 26 warnings respectively; neither log has warnings in the new provision files).

The combined disposable MySQL run `20261011T224634Z_39207a4132dc4ac1b9fadee5f14d8b16` exited zero. It covered the full fresh Medieval replay/second-run refusal, the earlier supply/weapon batches, provision rollback and lost-ack recovery, no-op all-table checksum conservation, builder edits and renamed shared material bindings, and fresh-process native food/container/liquid loading. Both final runs recorded stable source fingerprints. The owned database/server were deleted and TEMP/TMP restored.

The [companion receipt](Armageddon_Catalogue_S3_Provisions_20261012.json) records exact tested inputs, final runs and retained failed attempts. Earlier invalid test dispatch and fixture compilation failures are not passing evidence. Actual eating/drinking, complete MUD boot, full S3 creature behavior and complete-stock gameplay remain unexercised by this batch.

## Continuation boundary

The next S3 implementation work is the seven independently owned anatomy/race/ethnicity bundles, one culture and 49 NPC templates, with actual mount/guard/seeking/courier/haunting/Outsider behavior. The current body clone helper provides anatomy construction but no typed ownership reconciliation and omits alignment/orientation hit rows; those native relationships must be covered when used.

Remaining light, Haven water, projection/pocket and medical content must use real native factories and lifetime/grade bindings. Ember Seed cannot be certified by ordinary Bomb/Fuse components: those do not provide a frozen spell grade, exact creator attribution or durable one-shot detonation. Restorative Mud cannot be certified by a continuous HealingRate drug alone: it needs the approved one-dose healing budget and carrier calibration. These remain explicit adapter obligations, alongside S4 spell hydration and S5 carrier qualification.
