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
- Soot ink cakes, small inkwells, and prepared black ink pots
- Scroll cases, document pouches, tablet wraps, and scroll ties

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
