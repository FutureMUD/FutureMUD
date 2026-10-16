# Armageddon S3 catalogue checkpoint — 12 October 2026

Historical supply/weapon checkpoint. Its counts, implementation state and unexercised surfaces describe that earlier slice. The later composite S3 acceptance receipt supersedes its current-status statements without changing the original evidence below.

S3 is in progress. This checkpoint implements and qualifies the supply, shelter-template and melee-weapon portions of the approved catalogue. It does not mark the 210-blueprint catalogue complete. The normal interactive seeder still performs S1/S2; these S3 batch APIs will be connected to the composite installation when the remaining catalogue and dependency decisions are resolved. Release and staged spell admissions remain disabled.

## Native content

| Owned output | Count |
| --- | ---: |
| Ranked component supplies | 61 |
| Base supplies (component blank, focus blank, binding seal) | 3 |
| Native supply crafts and Component Crafting gate progs | 64 each |
| Component tags | 61 |
| Native component-blank forage definitions | 1 |
| Autobuilder room templates | 4 |
| Melee item prototypes | 49 |
| Native weapon types, attacks, combat messages and melee components | 49 each |
| Constant damage/pain/stun expressions for weapons | 147 |

The 64 supply items and 49 weapons produce 113 approved native item prototypes. Two shared holdable/stackable components serve supplies; weapons use that retained holdable and their own melee components. No live items, NPCs or Rooms are installed.

Supply recipes consume exact native commodity kilograms or whole units of the component blank. Durations, quantities and Component Crafting thresholds come from the approved inventory, including high Conjuration rank 6 at crafting 90, seven blanks and seven minutes. Native recipe progs use the numeric `ToTrait(id)` overload, so builder renaming cannot break the prerequisite.

Component blanks have a finite clay gathering route attached through native current forage-profile, yield and terrain foreign keys. Shared profile yield limits and terrain assignments are not overwritten. The four shelters are actual `room by terrain` autobuilder definitions bound to retained compatible Cave/Hallway terrain IDs. Spell-owned capacity, wards, creation and cleanup are later payload work.

Weapons have usable native melee attacks with the approved grade-specific damage, pain, zero stun and stamina values. Their components point to owned weapon types and the installed combat skills through retained typed native bindings. Knife/blade/spear use the native Knife/Longsword/Short Spear traits. The authored staff and lash adaptation uses Club and Improvised Bludgeon combat traits; it adds no mundane class skill or grant. Staff/lash prototype material defaults to the fixed Wood binding; other shapes use Stone. These defaults remain builder-editable and do not claim historical object parity. S4 supplies spell creation, placement and lifetime bindings.

## Ownership and updates

`ArmageddonStockCatalogue` reads the embedded approved inventory in declared dependency order. Only known schema fields participate in reference resolution: race, AI, culture, material and recipe-input fields. Narrative numbers and arbitrary definition text never become identity links.

`ArmageddonCatalogueOwnership` reconciles an explicit whitelist of native scalar fields and child collections. It retains exact native IDs and revisions, refuses missing/retired/noncurrent/competing identities and unowned name collisions, and uses the shared three-way stock reconciler to preserve builder edits. Each batch commits content and ownership together in a Serializable transaction. A later batch failure leaves earlier committed batches available for an owned retry; there is no claim of one transaction across all S3 families.

`ArmageddonCatalogueLinks` stores typed references to shared material, terrain and weapon-type rows without claiming those rows. Builder renaming retains the bound ID. An additive forage join is reconciled separately, so a builder-removed join is not silently recreated.

Native craft input/product timestamps are ordering keys persisted in MySQL DATETIME. Their timezone marker is normalised in current, desired and historical baseline values. Compatible stock updates change existing native children in place, preserving the input IDs saved by active crafts. Changes to input/product type or layout, or phase layout, fail explicitly and require a native craft revision instead of replacing referenced children.

## Qualification

The [companion receipt](Armageddon_Catalogue_S3_Checkpoint_20261012.json) records the final source-stable managed and disposable MySQL runs. Managed coverage includes all four Classic/Simple and classes-on/off combinations, exact source values, typed reference failures, native FutureProg execution at every recipe threshold, native attack-expression evaluation, component readiness, preserved builder edits and retained craft children. The MySQL lane runs the full Medieval replay and second-run refusal before the S3 batches, then proves rollback, lost-acknowledgement recovery, no-op rerun conservation, builder edits, stock repair and fresh-process factory loading for both supplies and weapons.

The final Debug Seeder run `20261011T210910Z-4dcafd806773` passed **1,673 tests**, with zero failures and three platform skips. The Release Seeder build completed with zero warnings/errors. The combined MySQL run `20261011T211322Z_0753351c2f9b466895f6ed97019da5a0` exited zero; both runs recorded stable source fingerprints. The owned database/server were cleaned up. Earlier supply-only runs are superseded by these combined results.

This is persistence and native-factory qualification. Actual craft execution, actual wield/combat moves, complete MUD boot with this new stock catalogue and complete-stock gameplay have not been exercised here. External material, terrain and trait references in the controlled readers use bounded mocks. A saved active craft resuming after repair and the phase-layout rejection path have not been separately executed. Native child-ID preservation and input-type rejection are covered.

## Remaining S3 work and decision dependencies

Creature race/body/ethnicity/culture bundles, the 49 NPC templates and their riding/guard/seeking/courier/haunting/Outsider behaviours remain to implement and qualify. Courier mapping establishes that native grasping forepaws can coexist with separate standing pads on four leg limbs, and that parcel pickup/delivery can use the normal Body.Get/Body.Give custody path.

The other focus/device, meal/liquid/light/trigger/projection/pocket and medical families also remain. Three native representation/ordering choices were pending at this checkpoint. The user subsequently resolved them in the [S3 native decisions](Armageddon_Catalogue_S3_Decisions_20261012.md): use native infections, defer foci/devices with spell dependencies to S4, and translate calories to satiation without a calorie model. The original questions were:

- Native diseases are spell infection parameters; there is no standalone native disease-definition entity. Choose a retained typed infection profile or an added builder-editable disease-profile system.
- Approved charged-device/focus definitions require real spell whitelist IDs. Choose to create real disabled spell records alongside these S3 dependencies and finish payload qualification in S4, or keep those devices under design until S4. Fabricated IDs and no-op spell placeholders are excluded.
- Native Food stores satiation hours, not calories. Choose the explicit five-hour representation for the approved 500-calorie meal using 2,400 calories/day, or extend the engine with a calorie model.

No answer has been inferred. S4 spell hydration and activation, S5 payload/carrier qualification, S6 fresh-install completeness, N24 ownership/rerun checks and the final readiness gates remain later work.

## Deferred adjacent finding

Existing ItemSeeder craft gates at `ItemSeeder.Crafting.cs:758,3019,3067` interpolate numeric trait IDs into the text `ToTrait("id")` overload, which resolves trait names. The S3 recipe gates use and exercise the numeric overload. The adjacent ItemSeeder gates were not changed or certified by this checkpoint.
