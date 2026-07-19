# Antiquity Writing Implements And Documents Crafting Suite

## Scope

The antiquity writing suite seeds functional scribal goods rather than relying on generic modern paper objects. It covers papyrus and parchment sheets, scrolls, codices, wax tablets, clay tablets, wooden writing blocks, ostraca, reed pens, quills, ink brushes, charcoal sticks, styluses, small scribal knives, ink goods, and compact document containers.

The suite is antiquity-first and culture-neutral in visible craft names and echoes. Knowledge rows can still name the historical craft family, but craft names shown to players avoid culture names.

## Runtime Components

`ScribingImplement` implements `IWritingImplement` for ancient and pre-modern writing tools. It stores the implement type, writing colour or colour characteristic, and a finite use counter. A total use value of `0` marks non-consuming tools such as styluses.

`InscribableSurface` implements `IWriteable` and `IReadable` for non-paper surfaces. It persists writing and drawing ids in component XML, enforces a character capacity, and accepts only configured `WritingImplementType` values. Wax and clay tablets can require styluses, while wooden blocks and ostraca can allow charcoal, brush, quill, reed pen, stylus, or chisel-style use as configured.

Existing `PaperSheet` and `Book` components remain the right surface for papyrus, parchment, scrolls, and codices.

## Seeded Items

Writing surfaces and document forms:

- Loose papyrus and parchment sheets, papyrus bundles, parchment bifolia and quires
- Papyrus and parchment scrolls
- Parchment codices backed by the existing book runtime
- Wax tablets, diptychs, and triptychs
- Unfired and fired clay tablets, clay tablet envelopes
- Smoothed wooden writing blocks and ostraca

Implements and support goods:

- Reed pens, quills, ink brushes, charcoal sticks
- Bone, bronze, and reed styluses
- Bronze pen knives and scraper knives
- Papyrus strip knives, pressing boards, burnishing shells
- Parchment scraping knives, stretching frames, and pumice
- Bookbinder's needles and punches
- Scroll roller rods and smoothing stones
- Quill curing sand, wax spatulas, pigment mullers, and pigment shells
- Soot ink cakes, small inkwells, and prepared black ink pots
- Scroll cases, document pouches, tablet wraps, and scroll ties

Source-audited final product catalogue:

| Stable Reference | Source Catalogue Name |
| --- | --- |
| `antiquity_bone_stylus` | a polished bone stylus |
| `antiquity_bronze_pen_knife` | a small bronze pen knife |
| `antiquity_bronze_scraper_knife` | a bronze parchment scraper |
| `antiquity_bronze_stylus` | a bronze stylus |
| `antiquity_cedar_scroll_case` | a cedar scroll case |
| `antiquity_charcoal_writing_stick` | a charcoal writing stick |
| `antiquity_clay_tablet_envelope` | a clay tablet envelope |
| `antiquity_fired_clay_tablet` | a fired clay writing tablet |
| `antiquity_hinged_wax_diptych` | a hinged wax tablet diptych |
| `antiquity_ink_brush` | a fine ink brush |
| `antiquity_leather_codex_pouch` | a leather document pouch |
| `antiquity_linen_tablet_wrap` | a linen tablet wrap |
| `antiquity_liquid_black_ink_pot` | a small inkwell of black ink |
| `antiquity_loose_papyrus_sheet` | a loose papyrus sheet |
| `antiquity_loose_parchment_sheet` | a loose parchment sheet |
| `antiquity_papyrus_scroll_tie` | a narrow linen scroll tie |
| `antiquity_papyrus_sheet_bundle` | a bundle of loose papyrus sheets |
| `antiquity_parchment_bifolium` | a folded parchment bifolium |
| `antiquity_parchment_codex` | a small parchment codex |
| `antiquity_parchment_quire` | a nested parchment quire |
| `antiquity_parchment_scroll` | a parchment scroll |
| `antiquity_potsherd_ostracon` | a smoothed potsherd ostracon |
| `antiquity_quill_pen` | a trimmed quill pen |
| `antiquity_reed_pen` | a cut reed pen |
| `antiquity_reed_stylus` | a sharpened reed stylus |
| `antiquity_sealed_papyrus_scroll` | a tied and sealed papyrus scroll |
| `antiquity_simple_papyrus_scroll` | a simple papyrus scroll |
| `antiquity_small_inkwell` | a small clay inkwell |
| `antiquity_smoothed_wooden_writing_block` | a smoothed wooden writing block |
| `antiquity_soot_ink_cake` | a soot-black ink cake |
| `antiquity_unfired_clay_tablet` | an unfired clay writing tablet |
| `antiquity_wax_triptych` | a three-leaf wax tablet triptych |
| `antiquity_wax_writing_tablet` | a waxed wooden writing tablet |

## Crafting Chains

Commodity crafts create the upstream stock used by final crafts:

- `Papyrus Sheet Stock`
- `Parchment Sheet Stock`
- `Tablet Blank`
- `Waxed Tablet Board`
- `Ink Stock`
- `Pen Blank`
- `Scrollmaking Stock`
- `Bookbinding Stock`

Final crafts are knowledge-gated through the shared `AddCraft` overload used by the antiquity craft suites. The helper upserts the deterministic `ItemSeederAppear...`, `ItemSeederCanUse...`, and `ItemSeederWhyCannotUse...` progs for the selected skill and knowledge gate. The suite does not seed bespoke `HasLiteracy`, `HasHandwriting`, or `HasPainting` progs.

## Discovery Boundary

The household craft suite still covers older document furniture and generic household containers. New writing-surface and writing-implement stable references are excluded from household dynamic discovery so final crafts are not duplicated.

## One-Step-Back Boundary

The May 2026 one-step-back pass seeds every writing/book/pigment tool currently named by `ItemSeeder.Crafting.AntiquityWriting.cs` as a `TagTool` prerequisite. Those tool prototypes are stock prerequisites for the current suite. Crafts to make the newly introduced tool prototypes themselves remain a second-pass task unless they are already part of a current finished-product craft family.
