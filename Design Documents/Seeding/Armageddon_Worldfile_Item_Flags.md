# Armageddon worldfile item flags and interaction restrictions

This translation uses native optional item components and saved effects. No original ArmSolo files or worldfile data are included in the repository. Installation is one-shot and applies after effective/reset/shop variants have been created. Existing opening stock receives component state and effect XML while retaining its other effects.

| Source field | Native mapping |
| --- | --- |
| Wear bit 0 absent (not TAKE) | Deny ordinary pickup, retain Holdable and initial scripted/outfit loading. |
| NO_DROP | Deny dropping and direct giving; allow container placement. These two restrictions are explicitly removable by Lift Hex. |
| NO_SHEATHE | Deny ordinary sheathing; initial authored equipment has an explicit bypass. |
| NO_ES | Require native two-handed wielding for items which already support wielding. Non-wieldable props are not turned into weapons. |
| HIDDEN | Omit ordinary room/group/scan listings, but allow explicit keywords. Not native stealth, no search roll, and not cleared by pickup. |
| INVISIBLE | Saved visual detection requirement, revealed by native magical vision. |
| ETHEREAL / ETHEREAL_OK | Saved ethereal item affinity: detection-gated visibility / permanent transition readiness. Physical handling remains material. |
| ANTI_MORT | Existing saved AdminInvis effect. |
| GLOW | Descriptive glow, no light. |
| HUM | Descriptive/local sound using native hearing; equipped/carried aggregate appears with character descriptions. No heartbeat or cross-room propagation. |
| MAGIC | Intrinsic native magic-aura classification; no invented sustained spell. |
| GLYPH | One-shot actual-pickup callback: warning, stored strength d4 HP and d6 stun, clear intrinsic magic, abort that pickup. All four reset-referenced glyph definitions store strength zero. State survives saving and copying. |
| NOSAVE | Native Transient Item retirement on inventory logout/database reload, without invented decay time. |
| DISINTEGRATE | Explicit retirement on bulk drop and configured carried-inventory supernatural disposal. Single drops and ordinary death do not destroy it. |
| Ammo/poison/tool/vehicle flags | Retain their existing owning import mappings; this pass does not duplicate them. |
| EVIL | A cleansable item curse marker, without invented alignment or additional restrictions. |
| LOCK, HARNESS, DARK, FLAME and remaining markers | Retain original masks in seeder provenance. No invented lock state, harness or illumination mechanics. |

## Source boundaries and adaptations

Source HIDDEN suppresses implicit room listings and bulk selection, not `can_see_obj`. FutureMUD has no ordinary bulk `get all` command to translate; explicit `get` remains available. Native normal inventory and explicit inspection remain unchanged.

Source NO_ES also prevents belt sheathing and one-handed skinning. This pass maps its shared two-hand requirement through native wielding anatomy; it does not invent two-handed use of non-wieldable tools or change native skinning. Explicit NO_SHEATHE maps ordinary sheathing. These source subcases remain recorded limitations rather than claims of full NO_ES parity.

Source DISINTEGRATE has four consumers: `drop all`; carried inventory of an undead summoned/corpse-possessed character's special death path; carried inventory on Shadow transition; and carried inventory during permanent corpse-host replacement. Worn equipment bypasses destruction in the special death/host-replacement paths. The native policy wires bulk-drop, non-propagating Shadow inventory ejection, and a death hook on the ten retained NPC definitions marked both undead (act bit 15) and summoned (affected-by bit 14), including reset variants. The explicit `TransientBodyDeath` and `HostBodyChange` prog triggers remain available for dynamically created corpse-possession and host-replacement content; they must not be attached indiscriminately to native ordinary death or every form switch. Corpse-host replacement is not reconstructed by this item-flags pass.

Native feasibility/preparation remains side-effect free. Pickup callbacks execute in the actual body action, with native reach/custody/command continuation revalidation. Combat command admission may reject an otherwise infeasible pickup before queuing it; this is a native admission difference from the source's earlier glyph check. Invulnerability bypass and spell-driven ETHEREAL/ETHEREAL_OK conversion use the explicit item-magic policies below.

## Item-directed magic continuation

Source `spell_remove_curse` clears EVIL and NO_DROP on an object without a power contest. Its ordinary Curse wrapper does not create object curses. The importer therefore marks existing EVIL/NO_DROP definitions and variants as cursed, and designates only the NO_DROP-derived Drop/Give restrictions as cleansable. Persisted opening stock receives the same component configuration. Lift Hex's native optional item-cleansing mode leaves hidden/sheathing/pickup restrictions, other permission components and unrelated effects intact. Ordinary character curse removal is unchanged. The saved component records permanent removal; copies and reloads do not restore the curse.

The source audit also established a permanent, owner-agnostic one-shot Glyph; general Dispel Magic removes it at source grade 3 or above. Character Invulnerability bypasses the pickup trigger without consuming it. The installed finite Unyielding Veil is the native mapping for that historical spell, not general damage immunity. Ethereal item casting has two permanent stages, a strict weight limit of 3 kg per grade under the established import conversion, and grade-percent destruction risk per successful stage. Equipment transition recursively filters container contents. Both source behaviours are enabled for the worldfile installer. Its Custodian Glyph uses the optional permanent pickup-hazard template; the native finite ownership mode remains available for other settings. Unravel Enchantment retains contested removal of spell-owned effects and separately removes intrinsic item states in source order: invisibility, glyph, other presentation/ethereal states, then magic aura. A glyph blocks further intrinsic cleanup below grade 3. Neither dispel nor glyph discharge lifts curse-derived interaction restrictions.

Shadow Passage accepts character and local item targets. Items advance from ready to detection-gated ethereal visibility; already ethereal items cannot repeat the risk roll. A successful destruction roll invokes native deletion, with custody safeguards. Character casting retains the installed native planar lifetime and opts into retaining ready equipment. Non-ready descendants are ejected from the original inventory container trees; ready children stay in their immediate parent, even when that parent is dropped. The ordinary native transition default remains full inventory ejection without this recursive filtering. Returning to a different effective plane clears direct equipment visibility while preserving readiness. Draw Into Flesh similarly clears item visibility without erasing readiness.

Inventory callbacks preserve the transition's captured boundary: newly acquired items are neither ejected nor retired as carried DISINTEGRATE items, transferred equipment keeps its new holder, and detached items drain to the original room/layer even if the bearer moves. Visibility callbacks cannot reactivate removed or dispelled affinities. Native held, wielded (including two-handed), worn and nested-container regressions are recorded in the [item-magic handover](Armageddon_Item_Magic_Handover_20261110.md).

Item attunement requires native sight and physical reach independently of keyword selection, with live admission rechecked before payment and callback-free containment/custody fences afterward. This excludes unseen container contents and inaccessible items while preserving the existing glyph convention that exposed, reachable equipment on another character does not require inventory-manipulation consent.

The imported Glyph keeps its empty component plan. Shadow Passage consumes one directly carried Alteration component of sufficient selected-grade rank, subject to the generated native material-exemption policy. Both item stages use that same ordinary payment path. If the selected target would itself be fully consumed as a spell component, configured casting refuses before payment. A spent/dispelled Glyph can be cast again on the same item; a third item attunement stage is refused without another payment or destruction roll.

Source initial presentation/invisibility/magic effects install once per item, using saved, copyable initialization markers; subsequent reloads do not recreate dispelled effects. Glyphs and affinity retain saved spent/dispelled markers, including on native copying/splitting, so their onload helpers do not rearm them. Initial equipment and scripted creation still bypass actual pickup callbacks.

NOSAVE items may have persistence rows during play but are retired when restored; this intentionally preserves disappearance at restore boundaries instead of suppressing all native database writes. Native item deletion/custody safety remains authoritative.


## Verification

Serial library/runtime/seeder/test-project builds passed. The focused runtime suite passed 60 tests, including permission persistence, pickup callback custody, partial-pickup callback helper, explicit retirement revalidation, outfit loading, native hearing (including deaf wearers), existing custody and stored-item delivery. Two importer checks passed with the external private packet, covering effective variants, preserved opening effects/components and real native compilation of generated progs. Initial failures (sheath auto-selection, mock reference membership, and an `additem` parser-prefix collision) were repaired and their receipts retained. This is automated evidence; no live installation, logout/restart or walkthrough qualification was performed.


The item-directed magic continuation passed 49 focused engine tests and seven Armageddon seeder tests, with no failures or skips in those selections. These cover curse cleansing, mixed targeting/reflection, glyph activation/immunity/persistence/copying, intrinsic dispel order, two-stage attunement/weight/risk, custody-sensitive transitions, and generated-prog compilation using the external private package. Verification used an isolated checkout at `5048963c` plus this tranche's exact files because unrelated clan/Discord work was changing concurrently. Final engine receipt: `20261110T002456Z-8f8d8aee34ed`; seeder receipt: `20261110T001858Z-e58f75f75d6d`, under the local isolated checkout's `.artifacts/curse-gates`. Earlier compile/fixture failures were repaired and their receipts retained. No live walkthrough, full world installation, or deployment was performed.
