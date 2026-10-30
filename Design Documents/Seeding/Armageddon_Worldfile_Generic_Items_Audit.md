# Armageddon generic item audit — 30 October 2026

This is a source-consumer audit, not an assertion that every retained object is actively reachable. Reset counts below use only the object-reference slot `Arguments[1]` in O/G/E/P records, excluding zones 57 and 76. Room, limit and container arguments are not object placements. Definitions are raw package counts; installation still applies the reviewed playground and broken-enterable exclusions. No new reset, supply, player state or builder data is created by this audit.

## Implemented writing implements

`comm.c:2325+` requires a visible held `ITEM_PEN` and an `ITEM_NOTE` with pages, then checks the selected writing language. It neither consumes ink nor reduces a pen charge field. All seven retained pen definitions have zero in all six value fields.

The importer uses the existing `ScribingImplement` component with `TotalUses=0`, its native nonconsuming mode. Vnums 1595 and 20039 are solid stone/bone styluses; the other five are quills. Black writing is an authored presentation default, not a recovered pigment field. Native writing/drawing, literacy, script and language checks remain in control. This adds no separate ink economy. Reset variants and directly persisted opening shop stock receive the component and its ordinary saved state.

| Vnum | Source description | Retained reset references | Direct shop records |
| --- | --- | ---: | --- |
| 1506 | a quill pen | 23 | 25, 26, 49, 117 |
| 1595 | a pen pen made from soft stone | 0 | none |
| 17046 | a beautiful quill pen | 1 | none |
| 20039 | a thin bone stylus | 2 | 20, 25 |
| 20174 | a black vestric-quill pen | 6 | none |
| 71701 | a long-feathered quill pen | 0 | none |
| 72917 | an extravagant, feather pen | 1 | none |

Zero references in these columns are not proof that a definition can never be supplied by another route. The importer enables the definition without inventing its placement.

## Remaining consumer mappings

| Type | Raw definitions; retained reset references / distinct vnums | Source behaviour and current boundary |
| --- | --- | --- |
| 2 Component | 50; 468 / 31 | `sp_parser.c:1261+` matches sphere (`value[0] == sphere-1`) and sufficient power (`value[1] >= power-1`); the consuming helper destroys a matching carried component. Reset-placed components in the four existing ranked native families (Alteration, Divination, Clerical, Teleportation) now receive their installed rank tags. Other spheres and unplaced definitions remain unmapped; spell requirements are not changed. |
| 3 Wand / 4 Staff | 25 / 17 definitions | Native charged devices exist. Source spell IDs, charging, resource costs and multi-payload policy remain deferred. |
| 7 Tool | 60 definitions | Exact `value[0]` subtype matters (`core_structs.h:615+`). Subtypes include climbing aids, traps, disarming, snares, bandages and lockpicks. Generic physical tools remain usable as craft inputs where already mapped; special command consumers require individual adaptation. |
| 10 Potion | 62 definitions | Healing fruit 20070 is implemented through the installed native healing effect and eligibility prog. Mixed and unknown spell payloads remain wholly deferred; no partial potion is silently installed. |
| 16 Note | 113 definitions | Blank paper and explicit-language authored pages are native. Language-ID-0 prose remains unchanged by decision; invalid page numbering, seal/teaching semantics and external private note files are not silently converted. See the remaining-gap inventory for explicit exception vnums. |
| 21 Pen | 7; 33 / 5 | Implemented as native scribing implements, as detailed above. |
| 26 Chit | 13; 61 / 2 | Existing custom-clothing and wagon-storage services have their own native receipt flows. Placed source chits are 1598 and 1599; 1599 still needs its exact service association traced. Do not treat a blank spawned ticket as a valid commission or entitlement. |
| 29 Teleport | 9; 8 / 8 | These are spell destination anchors, **not** objects whose use teleports the holder. `magick.c:14901+` targets the anchor's current room and requires the same plane. `utility.c:4736+` also restricts who can perceive a rune, using magic detection and source metadata. Native destination/visibility integration remains to be mapped; the objects' presence alone is not parity. |
| 30 Fire | 2; 0 / 0 | Source values encode lit state, duration and heat; fire has container/periodic consumers in `handler.c` and `object.c`. No direct retained reset places either definition. Dynamic creation and native fire mapping need a separate audit. |
| 31 Mask | 57; 24 / 12 | Placed Vnum14788 and Vnum25414 now use native identity obscuring with their explicit `[mask_desc]` text. Nine further placed masks retain the wearer full description through the optional native fallback; Vnum14304 remains excluded. See the bounded mask mapping below. |
| 33 Herb | 66 definitions | `skills.c:7125+` and `7456+` consume herbs (and some foods) in brewing. The poison carrier/identity work does not by itself install this brewing workflow. Preserve herb identities and audit recipes and output rules before creating medical effects. |
| 35 Scent | 2; 0 / 0 | The misleading type name is used by syringe loading/injection (`other.c:2575+`, `2701+`), not a demonstrated tracking-scent system. The two syringe definitions are not reset-placed; no injection consumer was established by this audit, so no injection behaviour is invented. |
| 36 Playable | 4; 2 / 1 | The only reset-placed prototype is dice 13602, already covered by native roll/throw/look/examine progs. Other playable definitions need their source consumer and reachability established before adding games. |

Decorative types such as art/treasure/other are not automatically missing behaviours merely because they have a generic physical component. Conversely, wearable/holdable coverage does not prove a special consumer is implemented.

## Bandaging requires a deliberate adaptation

Source `skills.c:248–350` requires tool subtype 5. It heals current HP on success, can injure or kill on failure, and consumes the bandage after an attempted eligible treatment. Skill and current-HP thresholds control eligibility. Potency `value[1] >= 8` can instead remove magical or generic poison with sufficient skill. A native bleed-stopping `Treatment` kit alone would omit these HP-model and poison consequences, so no such partial mapping is installed here.

The placed-mask fallback milestone is implemented for nine further definitions, with Vnum14304 excluded pending callback evidence. Source bandaging, charged-device mappings, mixed potions and personal-mount relationships remain parked; this slice does not author any of their semantics.

## Bounded placed-mask mapping

`utility.c:2276–2286` changes the visible short description for a mask worn on the head or face. `info.c:2054+` uses the exact bracketed `[mask_desc]` lookup for a head mask, falling back to the original character description if absent. `handler.c:418+` does not turn an unbracketed `mask_desc` keyword into that bracketed lookup.

The first mapping activates Vnum14788 (anakore mask) and Vnum25414 (hooded white tragedy mask), including effective reset variants. The native `IdentityObscurer` component reuses each existing wearable and uses native height/frame descriptors plus the actual mask in its short description. Full description is the source prose; effective reset keyword prose takes precedence. Applying the full prose to either supported native head/face wear profile is the native adaptation. No separate event hooks or onload effects are needed: native consumers inspect worn items, ignore a globally transparent wearable, and stop applying the component when the item is removed. Native stack/description precedence remains unchanged. Opening-stock instances receive the native component state.

Vnums 14308, 35909, 50391, 58453, 58454, 58456, 58458, 58459 and 58460 now opt into native `PreserveFullDescription`. This saved IdentityObscurer option defaults false when absent, so existing components retain their behaviour. Builders toggle it with `preservefull`; their authored full override is retained for later reuse. When enabled the component returns no full-description override, allowing the existing body-description pipeline to use the wearer's normal pattern/prose. Short-description masking, worn/transparent activation, recognition, and stack precedence are unchanged. Vnum14304 remains excluded pending genuine callback evidence; its unbracketed `mask_desc` is not activated.

This is still a one-shot setting seeder, not a live upgrade/upsert. Existing identity-obscurer components are skipped rather than overwritten if the helper is invoked again. Existing wearable profiles, item effect state, source descriptions, characters, and builder edits are not reconciled. Shared-script policy is unchanged.

The component uses the native default Insane disguise difficulty, but this is not proof of universal recognition enforcement: `CharacterRecognitionAuthority` handles worn obscurers, while legacy `Character.IdentityIsObscured`, `IdentityIsObscuredTo`, and `TryToDetermineIdentity` still have unimplemented paths. This content slice does not redesign those engine APIs or claim that every old identity consumer is fixed.

## Verification and publication

Checkpoint `21ccf8bb6` published the preceding poison/walkthrough/native-content work to draft PR #828. This audit and the scribing-implement slice are a separate follow-up. The focused seeder check covers all seven pens, a reset variant, opening-stock persistence, native quill/stylus identity and nonconsuming configuration, alongside the explicit-language note integration. No live walkthrough restart or database mutation is part of this work.

The subsequent mask slice passed its focused integration test, including native component loading/description substitution, worn/held/removed and transparent activation, exact source and reset prose, opening-stock persistence, unplaced/policy-deferred exclusions, and preservation of a builder-edited component on repeated helper invocation. Seeder and test builds passed; initial test compilation errors were corrected and retained in local receipts. No unrelated engine suite, live installation or gameplay test is claimed. Review found no actionable defect; the head-versus-face full-description adaptation is documented above.


The optional-fallback follow-up passed two focused mask tests: all eleven admitted base masks plus an authored reset variant, persisted opening-stock components, unchanged builder edits (including opting back out), legacy XML defaults, builder toggle/save/reload, and native short/full-description and recognition activation. Library, runtime, seeder and test builds passed. An initially incomplete test fixture and a premature run against its old assembly failed before the corrected completed-build run passed; receipts remain under `.artifacts/walkthrough/mask-fallback*`. No live deployment or database update was performed.


## Ranked spell-component integration

Source `skills.h:819–830` defines sphere identifiers; `sp_parser.c:1261+` compares the stored zero-based sphere and minimum power. The installed native spell catalogue already consumes ranked tags in four families. The importer now binds retained reset-placed type-2 objects, including effective reset variants, to those owned native tags: values[0] 1/6/8/10 map to Alteration/Divination/Clerical/Teleportation. Values[1] supplies the rank. Ascending ancestry admits stronger supplies for lower requirements. Source ranks above the installed family's maximum use its highest supported tag (5 for Alteration/Teleportation, 4 for Divination/Clerical); no higher spell grade is enabled.

This is an item-to-existing-consumer mapping, not a universal component requirement. Existing native spell plans retain their consumption, eligibility and grade policies. No spells, recipes, shops, supplies or new placements are created. Other spheres, invalid ranks and unplaced definitions are unchanged. Missing native tags or builder-modified ancestry are reported and left unchanged; unrelated prototype tags are retained. These prototype tags also apply to already-persisted opening-stock items without rewriting item state. This remains a one-shot installation, not a reconciliation promise.

The source craft package contains only the broken charred-meat sample (missing output), with TOOL_TYPE none. Therefore adding generic tool recipes would invent content. Source lockpicks also have no identified native picking consumer, and climbing aids require an explicit native bonus conversion; neither is silently treated as complete.

Fresh validation for this slice: seeder and test builds passed; the focused packaged-component integration test passed, including effective reset ranks, unplaced/unsupported exclusions, persisted tag links, duplicate prevention, custom tags, modified ancestry and unchanged opening item effects. No live database or gameplay test was run.
