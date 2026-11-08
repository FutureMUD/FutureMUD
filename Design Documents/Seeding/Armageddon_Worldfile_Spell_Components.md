# Worldfile spell materials and named summons

Implemented 8 November 2026. This replaces the earlier four-family, reset-only binding. The one-shot Worldfile installation applies these policies; standalone Setting stock retains its existing material profiles. The walkthrough is unchanged.

## Components and supply

All 49 valid base component definitions and their effective reset/shop variants receive exact native sphere/rank tags. Vnum19113 has invalid sphere/rank data and remains a physical unbound definition with provenance. All twelve material families are supported. Higher ranks substitute without clamping. No placements, opening stock or crafting recipes are invented.

Alteration, Divination, Clerical and Teleportation retain their ascending tag ancestry. Other families use explicit numeric rank choices across independent root tags. Missing high-rank identities are added through rank six where required. Existing stock tags are never reparented; missing or modified stock identities stop the affected mapping or installation rather than being silently repaired. Unrelated tags and opening-stock item state are preserved.

## Material plans

Here `g` is native grade 1–7. Plans use directly carried native items, consuming one stack unit or a whole nonstackable component. Native casting admission, payment and relocation safety remain authoritative, including refusing invalid casts before spending materials.

| Native spell(s) | Requirement |
| --- | --- |
| Compelling Regard, Shadow Passage | Alteration `g−1` |
| Consuming Flame, Borrow the Dead | Necromancy `g−1` |
| Haunting Shape, Wind Courier, Draw Traveller, Seeking Wind | Conjuration `g−1` |
| Air Guardian | Conjuration `g−1`, grades 2–7 only |
| Earth Mount, Anchor Rune | Creation `g−1` |
| Storm Spear | Creation `max(g−3,0)` |
| Storm Locus | Nature `g−1` |
| Open Threshold, Join Traveller | Teleportation `g−1` |
| Walking Shadow | Illusion `g−1` |
| See the Unbodied | Divination `max(g−3,0)`; Shadow waives materials; Silt still refuses |
| Sheltering Veil | Clerical `g−1`; Water waives materials |
| Flame Knife, Mist Shield, Wind Shield | Conjuration 6 at grade 7 only |
| Sand Knife, Shadow Blade, Spark Lash | Creation 6 at grade 7 only |
| Kindle Enchantment | Both Enchantment 6 and Alteration 6 at grade 7 only |
| Exchange Places | Retain Teleportation and consume separate Alteration, both `g−1`, both directly carried |
| Call Outsider, ordinary route | Conjuration `g−1` |
| Call Outsider, named route | Conjuration 5 or higher, plus a retained ritual token |
| Return Tether | Optional Teleportation `g−1`, multiplying duration by `max(2,g)` |

Most costs exempt player Templars, Elementals, Fiends and immortals through boolean character progs. NPCs retain native requirements. Earth Mount also exempts the player Earth class. Kindle Enchantment exempts immortals only; Exchange Places exempts the classic Southern Templar player class and immortals. Plane-specific waivers remain intact.

Return Tether works without a component. An exempt caster must still carry a suitable component to earn the optional benefit but retains it. Its existing native `30*g` second delay becomes `30*g*max(2,g)` with the benefit. Return cancellation, journal ownership and endpoint safety remain native.

The audit follows active consumers in `sp_parser.c`, `magick.c` and registered `spells.c` wrappers. An unused spell-template stub and metadata hints do not create costs. The named Gate wrapper's fixed cost is not charged twice by its backend.

## Named Gate route

The external preparation script extracts ten template-backed rituals into `GateSummons`. The original C files, roster table and generated private packet remain outside Git. Older packets must be regenerated. The eleventh existing-character staff joke is excluded by agreement.

Call Outsider accepts an optional exact ritual name after the power argument. With no name it retains its native grade-based route. Named choices require grade 6 or 7, a matching directly carried token (kept), and the fixed Conjuration cost above. All listed alternative tokens qualify, including decorative/treasure objects rather than components.

Separate native summon templates copy the final imported definitions and add companion control. Reset templates are untouched. Existing loaded creatures of the named/base template refuse duplicate summoning before payment. None of the ten source definitions has a retained M reset, so no reset-template alias is omitted from that stock check.

Named lifetime is `(g + random integer 2..4) * 600` real seconds. Native temporary cleanup owns expiry/restart handling and conserves foreign possessions. A 1–100 roll below `3 + 40*n`, counting already controlled followers from this spell, breaks control and uses native hostile engagement; otherwise the creature follows and accepts native companion orders. Loss chances are 2%, 42%, 82%, then 100%. Uncertain persistence holds the created lifecycle for review rather than replaying creation.

## Verification

Serial library/runtime/seeder/test-project builds passed. Focused checks cover rank bindings, preserved state, generated plans, conditional/optional payment, retained tokens, named selection/refusals, plane-sensitive material wrappers, Exchange Places and return deadlines. The private package was regenerated and its ten rituals qualified against source races/tokens. This is not a full live MySQL installation or gameplay qualification. See `.artifacts/spell-components` for local test receipts, including the initial fixture failure and corrected rerun.
