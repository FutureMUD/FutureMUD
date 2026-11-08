# Armageddon worldfile item flags and interaction restrictions

This translation uses native optional item components and saved effects. No original ArmSolo files or worldfile data are included in the repository. Installation is one-shot and applies after effective/reset/shop variants have been created. Existing opening stock receives component state and effect XML while retaining its other effects.

| Source field | Native mapping |
| --- | --- |
| Wear bit 0 absent (not TAKE) | Deny ordinary pickup, retain Holdable and initial scripted/outfit loading. |
| NO_DROP | Deny dropping and direct giving; allow container placement. |
| NO_SHEATHE | Deny ordinary sheathing; initial authored equipment has an explicit bypass. |
| NO_ES | Require native two-handed wielding for items which already support wielding. Non-wieldable props are not turned into weapons. |
| HIDDEN | Omit ordinary room/group/scan listings, but allow explicit keywords. Not native stealth, no search roll, and not cleared by pickup. |
| INVISIBLE | Saved visual detection requirement, revealed by native magical vision. |
| ETHEREAL | Saved ethereal-detection requirement, also revealed to occupants of the configured Shadow plane. Physical handling remains material. |
| ANTI_MORT | Existing saved AdminInvis effect. |
| GLOW | Descriptive glow, no light. |
| HUM | Descriptive/local sound using native hearing; equipped/carried aggregate appears with character descriptions. No heartbeat or cross-room propagation. |
| MAGIC | Intrinsic native magic-aura classification; no invented sustained spell. |
| GLYPH | One-shot actual-pickup callback: warning, stored strength d4 HP and d6 stun, clear intrinsic magic, abort that pickup. All four reset-referenced glyph definitions store strength zero. State survives saving and copying. |
| NOSAVE | Native Transient Item retirement on inventory logout/database reload, without invented decay time. |
| DISINTEGRATE | Explicit retirement on bulk drop and configured carried-inventory supernatural disposal. Single drops and ordinary death do not destroy it. |
| Ammo/poison/tool/vehicle flags | Retain their existing owning import mappings; this pass does not duplicate them. |
| EVIL, LOCK, HARNESS, DARK, FLAME and remaining markers | Retain original masks in seeder provenance. No invented alignment, lock state, harness or illumination mechanics. EVIL's legacy curse classification is not an ordinary combat alignment. |

## Source boundaries and adaptations

Source HIDDEN suppresses implicit room listings and bulk selection, not `can_see_obj`. FutureMUD has no ordinary bulk `get all` command to translate; explicit `get` remains available. Native normal inventory and explicit inspection remain unchanged.

Source NO_ES also prevents belt sheathing and one-handed skinning. This pass maps its shared two-hand requirement through native wielding anatomy; it does not invent two-handed use of non-wieldable tools or change native skinning. Explicit NO_SHEATHE maps ordinary sheathing. These source subcases remain recorded limitations rather than claims of full NO_ES parity.

Source DISINTEGRATE has four consumers: `drop all`; carried inventory of an undead summoned/corpse-possessed character's special death path; carried inventory on Shadow transition; and carried inventory during permanent corpse-host replacement. Worn equipment bypasses destruction in the special death/host-replacement paths. The native policy wires bulk-drop, non-propagating Shadow inventory ejection, and a death hook on the ten retained NPC definitions marked both undead (act bit 15) and summoned (affected-by bit 14), including reset variants. The explicit `TransientBodyDeath` and `HostBodyChange` prog triggers remain available for dynamically created corpse-possession and host-replacement content; they must not be attached indiscriminately to native ordinary death or every form switch. Corpse-host replacement is not reconstructed by this item-flags pass.

Native feasibility/preparation remains side-effect free. Pickup callbacks execute in the actual body action, with native reach/custody/command continuation revalidation. Combat command admission may reject an otherwise infeasible pickup before queuing it; this is a native admission difference from the source's earlier glyph check. Invulnerability, source curse removal and spell-driven ETHEREAL/ETHEREAL_OK item conversion are separate spell-policy consumers, not consequences inferred from an item marker alone.

NOSAVE items may have persistence rows during play but are retired when restored; this intentionally preserves disappearance at restore boundaries instead of suppressing all native database writes. Native item deletion/custody safety remains authoritative.


## Verification

Serial library/runtime/seeder/test-project builds passed. The focused runtime suite passed 60 tests, including permission persistence, pickup callback custody, partial-pickup callback helper, explicit retirement revalidation, outfit loading, native hearing (including deaf wearers), existing custody and stored-item delivery. Two importer checks passed with the external private packet, covering effective variants, preserved opening effects/components and real native compilation of generated progs. Initial failures (sheath auto-selection, mock reference membership, and an `additem` parser-prefix collision) were repaired and their receipts retained. This is automated evidence; no live installation, logout/restart or walkthrough qualification was performed.
