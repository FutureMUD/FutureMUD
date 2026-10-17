# Armageddon Info Master List — source review, 11 October 2026

The user supplied this reference and explicitly paused S1 to review and preserve it for magic and later class work. The complete [original text](Armageddon_Info_Master_List_20261011.txt) is archived byte for byte. The [structured extraction and comparison](Armageddon_Info_Master_List_Review_20261011.json) and [skill-table TSV](Armageddon_Info_Master_List_Skill_Tables_20261011.tsv) retain original line numbers and printed values.

This is source evidence, not an implementation instruction or automatic amendment of the [locked seeder decisions](../../Magic/Armageddon_Seeder_Decisions_20261011.md). S1 implementation is paused at `76afe8f94553615b40a428cf39551018fb5ff465`; no S1 source code, tests or database installation were changed before the pause. N23 remains the latest bounded runtime checkpoint. The review/archive itself changes documentation only.

## Preservation and provenance

- Supplied path: `C:/Users/luker/OneDrive/Armageddon Info Master List.txt`.
- Archive: 40,439 bytes, 1,619 lines, UTF-8 without BOM, CRLF line endings, final newline retained.
- SHA-256: `4dd92656d32068e5e2726c15c1a59bb4586d2e99bc41f59a5e8bef9136524c93`.
- File modification timestamp: `2026-04-21T00:11:37.7003465Z`; this is filesystem metadata, not a publication date.
- The text supplies no author, source URL or dated version for the skill tables. Its later commentary mentions a code dump and a transition from brew to craft. Those comments do not establish the origin or historical version of the earlier tables.
- A scoped `.gitattributes` rule prevents line-ending conversion or whitespace cleanup of the original. Extraction and review files are separate from the preserved text.

## What the file contains

All line ranges below refer to the archived original.

| Section | Lines | Contents |
| --- | --- | --- |
| Subguild list | 1–26 | 24 subguild rosters, including Acrobat, Forester, Linguist, crafting roles, Guard, Nomad, Thief, Hunter, Physician and Caravan Guide |
| Race attributes | 32–166 | 19 race/creature profiles with Strength, Agility, Wisdom and Endurance dice expressions and displayed ranges |
| Poisons | 169–188 | Claimed effects and acquisition notes for bloodburn, grishen, peraine and terradin |
| Cure tablets | 193–254 | Descriptions, effects and ingredient/location notes; several explicitly uncertain entries |
| Spice | 258–313 | Nine distinct named spices; Krelez occurs twice with different colour descriptions |
| Combat description values | 316–339 | 11 printed damage bands, from Barely Grazing to Unspeakable |
| Guild skill tables | 342–1492 | 15 tables, 545 rows; openings, caps, printed `go on` values, dotted parent structure and minimum mana where supplied |
| Poison/cure code excerpts and commentary | 1494–1619 | 16 taste cases, six antidote and five poison recipes, herb/cure fields and strength notes |

| Guild table | Heading line | Total rows | Rows with printed minimum mana |
| --- | ---: | ---: | ---: |
| Water Elementalist | 346 | 35 | 25 |
| Stone Elementalist | 420 | 32 | 21 |
| Fire Elementalist | 488 | 31 | 21 |
| Wind Elementalist | 554 | 36 | 26 |
| Lightning Elementalist | 630 | 29 | 19 |
| Drov Elementalist | 692 | 29 | 19 |
| Void Elementalist | 754 | 33 | 22 |
| Psionicist | 824 | 34 | 0 |
| Sorcerer | 896 | 94 | 82 |
| Pick-pocket | 1088 | 23 | 0 |
| Burglar | 1138 | 28 | 0 |
| Warrior | 1198 | 32 | 0 |
| Assassin | 1266 | 29 | 0 |
| Ranger | 1328 | 36 | 0 |
| Merchant | 1404 | 44 | 0 |

The seven elemental tables contain **153 spell memberships**, seven Component Crafting rows and one Vloran row, plus 64 other non-spell rows. Each elemental table has six starting spells. The Sorcerer table contains 82 spells and 12 non-spell rows. Across the elemental and Sorcerer tables, the 235 mana-bearing rows map to 134 existing canonical spells. The Psionicist table includes psychic powers and mundane support; its lack of `min_mana` is not evidence that those powers have no resource cost.

## Comparison with the locked magic design

Comparison inputs are the [research adoption](../../Magic/Armageddon_Elementalist_Research_Adoption_20261011.json), [exact Sorcerer tree](../../Magic/Armageddon_Sorcerer_Source_Tree.json), [Classic table](../../Magic/Armageddon_Seeder_Completeness_Design_20261011_Classic_Progression.tsv) and [stock inventory](../../Magic/Armageddon_Seeder_Completeness_Design_20261011.json) at the paused revision. Their hashes are recorded in the review JSON.

**Sorcerer agrees completely.** All 94 rows have the same order, openings, caps, printed branching values, minimum mana where supplied, and dotted parents as the existing source tree. This corroborates that compilation without independently authenticating its historical origin.

**The selected elemental parent graph agrees completely.** All 157 research rows are present: 149 spells, seven Component Crafting rows and Vloran. Canonical parent links and the six-spell starting sets agree for every guild. In particular, Stone has Rewind → Identify → Repair Item (458–464), and Void prints Mark → Travel Gate → Portal (756–760). The latter directly names Travel Gate in that position, strengthening the selected Linked Threshold mapping. Gate remains a separate table entry; these tables do not explain its runtime effect or prove that every guild's Mount should share one implementation.

The main differences are numeric. The 157 matched research rows have **114 opening differences**: 107 spell rows and all seven Component Crafting rows. Water Heal adds one cap/branch-value difference; Health Drain inherits one parent-threshold difference under the table interpretation described below.

| Point | Locked stock design | New file | Original lines |
| --- | --- | --- | --- |
| Most elemental spell branches | Open 30 | Open 40 | 346–820 |
| Water Heal / Mend Flesh | Open 60, cap 60, branches 50 | Open 60, cap 90, `go on` 80 | 368 |
| Water Health Drain / Vital Siphon | Parent threshold 50, open 30 | Parent's printed `go on` 80, open 40 | 368–370 |
| Lightning Gate / Call Outsider | Open 30 | Open 60 despite being a branch | 648–650 |
| Lightning Identify / Read Enchantment | Open 30 | Open 20 | 678 |
| Water Mist Shield | Open 30 | Open 10 | 392 |
| Elemental Component Crafting | Open 30 in all seven | Open 20 in six; open 40 in Void; all cap 90 | 398, 460, 528, 600, 676, 704, 810 |

Water Heal's cap 90 is distinct from Sorcerer Heal's cap 60 (988). The former reconstruction had reused the lower cap for Water; the new source records a guild-specific difference.

The file also supplies **four spell memberships absent from the selected research graph**:

| Guild and spell | Printed parent and values | Current locked disposition |
| --- | --- | --- |
| Water Breathe Water / Water Breathing (386) | Sanctuary / Sheltering Veil; open 40, cap 90, mana 20 | Already included as a supplement, but after Draw Water, open 30 |
| Water Shield of Mist / Mist Shield (392) | Wall of Thorns / Thorn Barrier; open 10, cap 90, mana 20 | Already included as a supplement, but after Sheltering Veil, open 30 |
| Lightning Ball of Light / Hovering Light (670) | Energy Shield / Charged Aegis; open 40, cap 90, mana 7 | No Lightning membership in the locked tables; the canonical spell already exists |
| Void Vampiric Blade / Leeching Edge (802) | Phantasm; open 40, cap 90, mana 25 | Already included as a supplement, but after Devouring Touch, open 30 |

These are membership/progression differences, not four new spell identities. Twenty of the 23 approved supplemental elemental memberships remain absent from this source. Eighteen of the 152 required canonical spells have no mana-bearing row anywhere in this file; the JSON lists them. Those authored routes still need their separate provenance if retained.

**Minimum mana also needs a deliberate comparison with native casting policy.** All printed values for a given canonical spell agree across this file's guilds. For 55 elemental spell memberships, representing 55 distinct spells, the printed minimum mana differs from the locked `minimum_energy`. Examples: Silence 33 versus 5; Guardian 33 versus 9; Wind Fist 33 versus 7; Mist Shield 20 versus 6. The JSON records all 55 differences. A printed minimum is not a formula for every grade, quiet/area variant, capacity coefficient or timing. No stock energy configuration was changed by this review.

Simple progression remains an authored one-skill design; the file supplies no Simple unlock thresholds. If its parent changes and new Lightning membership are adopted, the corresponding Simple ordering/roster will need recalculation under the approved rule. The game-wide mode choice, conditional classes, required-support-only policy and automatic NPC/items remain user decisions.

## Interpretation and source limits

The extraction preserves `st`, `max`, `min_mana` and `go on` verbatim as numeric fields. Dotted depth links a row to the nearest preceding row one level shallower within its guild. `inferred_parent_threshold` copies that parent's printed `go on`, following the existing Sorcerer-tree interpretation; it is explicitly derived, not an additional printed field. A root's own `go on` is retained even though the root has no parent threshold. Name links use exact existing source/tree names with case normalization, not fuzzy matching or numeric-ID scans.

This file does not provide elemental effect algorithms, magnitudes, durations, full spell-grade cost formulae, components, incantations or NPC/item definitions. Those still require the existing code-backed sources and approved adaptations. It contains historical-looking material, but gives no basis to claim present-day Armageddon parity.

There are internal quality caveats for future race/content work. Human Wisdom is printed as `4d4+4` with range 8–19 (36–39), although ordinary dice arithmetic gives 8–20. Desert Elf Wisdom is printed as `4d4+5` with range 20–29 (162–165), although ordinary arithmetic gives 9–21. Gaj is explicitly marked as probably changed (140). Several cure entries say their effect is unknown; location claims are setting-specific. The later poison/cure commentary explicitly assumes that some old flavour rules survived subsequent changes (1582). Preserve these claims and uncertainties rather than silently correcting the archive or treating all sections as equally established.

## Resume checkpoint

The next implementation milestone remains S1: durable fixed installation settings and typed native-foundation preflight. Its earlier read-only investigation found that shared seeder answers normally save only after `SeedData` returns, so fixed choices need their own durable package record before independently committed content modules. Chargen's actual question is `class`; the planned `use-classes` shared key does not yet exist, and class use can also be checked through the typed RolePicker storyboard. Those findings remain investigation, not implemented code.

Before resuming S1 implementation, reconcile the new source findings with the locked Classic membership/value tables and any resulting Simple ordering changes. This archive establishes a reusable reference for that reconciliation and for the later Psionicist, mundane class, subguild, race, crafting and poison passes. Existing exclusions of non-spell class skills still apply; their presence in the reference does not re-enable them.
