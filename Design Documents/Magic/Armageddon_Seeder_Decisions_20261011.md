# Armageddon final seeder decisions — 11 October 2026

The user approved all five decision-gate configurations, with one overriding correction: **exclude non-spell class skills except required prerequisites**. This closes the content design gate. The [decision record](Armageddon_Seeder_Decisions_20261011.json), [stock inventory](Armageddon_Seeder_Completeness_Design_20261011.json), [Classic rows](Armageddon_Seeder_Completeness_Design_20261011_Classic_Progression.tsv) and [Simple rows](Armageddon_Seeder_Completeness_Design_20261011_Simple_Progression.tsv) are the implementation inputs. Research remains evidence; the direct human approval selects stock policy.

## Identity and supplemental admissions

Void Portal Gate uses the existing **Linked Threshold / Travel Gate** anchored travel effect: Arcane Mark → Linked Threshold → Open Threshold. This is a deliberate FutureMUD identity selection, not a proven historical alias. No new spell identity is created.

These 23 memberships complete the researched roster without enlarging the 82-spell Sorcerer roster. Each child opens at 30/cap 90 and controlled grade 1. Parent threshold is that guild binding's cap minus10: normally 80, or 50 after Water Mend Flesh. These authored stock admissions remain separately labelled from historical guild claims.

| Capability | Added spell | Parent |
| --- | --- | --- |
| Fire | Daylight | Hovering Light |
| Water | Wither Growth | Thorn Barrier |
| Water | Mist Shield | Sheltering Veil |
| Water | Restorative Mud | Mend Flesh |
| Water | Purge Disease | Purge Toxin |
| Water | Water Breathing | Draw Water |
| Water | Drowning Grip | Water Breathing |
| Water | Sow Sickness | Venom Touch |
| Water | Caustic Spray | Venom Touch |
| Water | Gather Puddle | Draw Water |
| Earth | Trace the Path | Return Tether |
| Earth | Clay Sentinel | Earth Mount |
| Earth | Shatter Stone | Earthen Strength |
| Shadow | Champion Blade | Shadow Blade |
| Void | Devouring Touch | Empty Aura |
| Void | Severing Refuge | Bar the Elements |
| Void | Apex Bane | Turn the Elements |
| Void | Cross the Veil | Linked Threshold |
| Void | Unbodied Journey | Feign Death |
| Void | Erode Object | Arcane Mark |
| Void | Leeching Edge | Devouring Touch |
| Void | Echo Servant | Voice of Remains |
| Void | Sow Sickness | Erode Object |

## Simple progression

Each elemental capability uses its final Classic spell roster and six source starting spells. One main skill opens at 10/cap 100. Roots unlock at 10; other spells sort by parent depth, then source order, with supplemental siblings in the table order above. For rank i among N nonroots, unlock is `15 + floor(75*(i-1)/(N-1))`, spanning 15..90. The 246 existing Sorcerer/Preserver/Defiler spell rows remain unchanged. Admission begins controlled grade 1; raw proficiency and controlled grade remain separate.

## Required non-spell support only

The exact 94-row Sorcerer source contains one non-spell parent of a spell: **Read Enchantment requires Component Crafting 80**. Component Crafting also supplies the native recipes for required magical components. Keep that support skill. The threshold 80 applies only to the three Sorcerer variants, not elemental Read Enchantment.

Classic installs Component Crafting at its source/researched guild position, opening30/cap 90. Simple grants it on legitimate permanent capability enrolment for all ten capabilities, opening10/cap 90. Simple never grants Classic per-spell proficiency traits. Installing/rerunning content never grants skills or spells to existing players.

Vloran is an after-spell language side grant, not a spell prerequisite, so it is deferred. Contact, Barrier, Expel, Dual Wield, Shield Use, Forage, Cooking, Analyze, Two Handed and Gather are also deferred in both modes. Their historical rows remain evidence for the later non-magical class/full-skills pass. Native NPC combat stats and required magic-supply crafting remain part of the magic package.

Classic has 428 install rows: 172 elemental spell memberships, seven elemental crafting bindings and three variants of 82 Sorcerer spells plus one crafting support each. Simple has 418 spell admissions and ten crafting grants. All 152 required canonical spells have routes; Wardcraft and Renew Earth remain optional and unadmitted. All 154 candidates and 210 prototype blueprints remain in the completion inventory.

## Final stock defaults and implementation boundary

Current per-entry effect magnitudes/durations and minimum energy, reserve rates/capacity coefficients, NPC/item stats, recipes and device capacities/timings are final builder-editable defaults. The JSON records their canonical value hash and exact fields against baseline `fe2f750133bcb5a56645390db0d57227a90c4713`; this checkpoint makes no numerical rebalance. Historical unknowns and authored origins remain visible. Native unit conversion and correctness qualification implement these settings; a material balance change requires a new explicit human decision.

Existing approved incantations remain selected. Legacy generic energy/area heuristics and unsupported carrier candidates do not become qualified by this approval. Read Attunement affinities and learned affiliations remain separate. One fixed game-wide mode, ten capabilities and conditional matching classes, automatic NPC/item supplies, typed links and preservation of builder edits/player state remain binding.

No content-design question remains. S1 fixed installer settings and typed preflight is next, followed by S2–S6 implementation, then N24. This checkpoint changes design artifacts only. N23 remains the latest bounded runtime qualification; installed learning, full stock behaviour, historical parity and Release readiness retain their implementation gates.
