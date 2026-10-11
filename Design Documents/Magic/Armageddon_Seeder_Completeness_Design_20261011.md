# Armageddon seeder completeness design — 11 October 2026

This is the approved design work before N24. It specifies one complete, builder-editable Armageddon package. No game code, database, runtime stock or replay profile is changed by this checkpoint. The baseline is `aae875af09cf62e6324bb31eba79f58b72249e13`, after the bounded N23 recharge qualification. N24 remains unstarted.

The [machine-readable inventory](Armageddon_Seeder_Completeness_Design_20261011.json) contains all 154 candidates, all ten capability definitions, every Simple admission, the exact historical Sorcerer rows, and the automatic prototype catalogue. The [Simple progression TSV](Armageddon_Seeder_Completeness_Design_20261011_Simple_Progression.tsv) is the reviewable threshold list. These are design inputs for subsequent implementation, not a manifest currently loaded by DatabaseSeeder.

## Decisions and authority

Current user requirements supersede older installer proposals: Classic/Simple is a game-wide installation choice; each of seven elemental traditions and Sorcerer, Preserver and Defiler always gets a MagicCapability and capability merit; matching native Class roles are created automatically if the earlier seeder enabled classes. NPCs, items and supplies are authored automatically. There is no class opt-in, per-player mode, version picker, alternate installation or Classic/Simple conversion.

One installed package has ordinary manifest revisions for updates. Reruns repair/update that package in its fixed direction while preserving builder customisations and all player state. The earlier completion brief remains scope authority for 152 required spells, two optional additions, the exact 82-spell Sorcerer roster/four roots/12 supports and eight larger systems. No scroll stock and no invented numeric-ID reference scans. Neither this document nor older attached-file workflow wording grants merge, deployment or production database authority.

Read Attunement has the user's selected semantics: **elemental affinities and learned magical affiliations are reported separately**. Numeric effect/crafting/template defaults below are new editable FutureMUD proposals where historical native objects or setting identities are unavailable. A current factory is narrower evidence than full installed-stock qualification.

## Remaining source gate

The selected Sorcerer tree is exact: four spell roots open at 60/cap90; 78 other spells open at30; Mend caps at60 and other spells at90. Each child's unlock uses its parent's recorded threshold. All three Sorcerer variants use that same 82-spell tree and 12 supporting rows. [The retained source tree](Armageddon_Sorcerer_Source_Tree.json) records the selected 2013 tree/2016 archive provenance.

The supplied C dump/reference **does not include external elemental guild/subguild learn tables**. The C skill table proves spell definitions/families, not complete guild openings, caps, memberships or branching. This absence is recorded in [the integration evidence](Armageddon_Casting_Integration.md#supplied-historical-evidence). Old grade2/raw20 elemental edges are proposals, and required Earth Mount previously depended on optional Wardcraft. Those values are excluded from this design's Classic data.

Classic elemental numerical progression must wait for documented historical learn tables. No made-up fallback is approved. If those tables cannot be obtained, using labelled designed elemental tables would require a user decision because Classic was requested to use Armageddon data. All remaining design and independent implementation work can proceed; a complete Classic four-cell validation matrix and Release gate cannot pass while this source gate remains open. The existing seven elemental family memberships are labelled designed repertoire membership, as allowed by the original brief, and must be compared against any newly obtained learn tables.

## Installer and persistence contract

Extend the existing ArmageddonMagicSeeder and shared execution/answer/ownership framework. Retain one stable package identity and existing owned records; do not create a parallel seeder, conversion service or edition registry. A full installation preflight resolves the whole typed dependency graph, class configuration, attribute/skill/unit/material/liquid/language/check mappings, and exact approved native revisions before any package content commit.

Use a single validated `arm.package.installation` metadata sentinel in the existing SeederManagedRecord table (null LogicalId; dedicated reader so it is never treated as a missing entity). It records fixed mode, class setting and typed native bindings. Commit that sentinel and its shared-answer mirror atomically **before the first independently committed content module**. Generic answer memory is written after SeedData succeeds and stores latest answers; alone it cannot preserve choices through a mid-install failure. The dedicated package sentinel solves that boundary without new version-management machinery or player migration.

On resume/rerun, read the sentinel and receipts, require the same mode and class direction, and repair only eligible owned work. Contradictory answers fail before mutation. Missing/corrupt/ambiguous sentinel on an installed package is a specific diagnostic, never permission to guess from names/latest answers. A historical partial package with no fixed mode is the same installation: classify its existing typed owned records and current player state, record the initial full-install choice, and reuse eligible content without changing players or assuming old external bindings are owned.

| Question | When | Contract |
| --- | --- | --- |
| `install-armageddon` | Explicit installer selection | Yes/no, default No; decline leaves package/player content untouched. |
| `armageddon-progression-mode` | First full installation only | Classic/Simple, default Classic; fixed package setting, mirrored shared answer; later runs display it. |
| `armageddon-capacity-attribute` | Only when usual seeder mappings are ambiguous | Select one compatible native mental/spiritual body attribute; no hard-coded historical races. |
| `armageddon-native-bindings` | Only for ambiguous nonstandard world primitives | Validated typed mappings for native integration; never ask for summoned NPC/item prototypes. |

Resolve classes from the earlier ChargenSeeder `use-classes` answer **and the actual class picker storyboard**. The presence of a Class RoleType or existing class rows is insufficient: ChargenSeeder creates the RoleType even when classes are off. A disagreement produces a read-only diagnostic before install; the magic seeder does not rewrite global chargen. Saved initial configuration governs supported reruns; redesigning the MUD after installation is outside scope.

When classes are on, create exactly ten matching Class roles with the right capability merit, mode-specific initial skills/supports, and a supported canonical enrolment hook at character finalisation. Trait possession alone is not spell acquisition. When classes are off, create zero package Class roles and the same complete capabilities/merits/admissions/prototypes. Builders can attach the capability merit and invoke the same explicit enrolment route through normal grant/chargen tools. Installing never attaches or enrols existing players, refills reserves, opens their skills, resets clocks or places live NPCs.

## Progression and resources

Shared spell definitions retain native elemental school for wards/resistance/detection. Each capability owns admissions and gathering policy; no duplicate spell definitions for Sorcerer variants. Classic binds individual native spell skills to exact available source rows. Its 12 supporting skills/language/psionic rows retain exact source data, reuse proven native concepts, and stay scoped to enrolled capabilities. Component Crafting is a real support prerequisite for Read Enchantment. Do not require a second mandatory psionics package merely to author a native support row.

Simple creates one main magic skill per capability: opening10, raw cap100, a genuine starting SupportGrant and DefaultTrait binding. Automatic spell edges use a typed SupportTrait prerequisite against that main skill at the tables below. They do not use the old spell-grade2/raw20 prerequisites. Component Crafting remains a functional mundane support skill; it can be opened at10 with the route, and its source-derived threshold80 remains required for Read Enchantment. Other mundane weapon/language/support concepts remain ordinary native skills; they do not create additional per-spell magic proficiencies.

Simple root spells start at skill10 and controlled grade1. Other thresholds occupy15..90 in a source-inspired topological order, prioritising selected Sorcerer parent depth and source order where known and repertoire order otherwise. The concrete tables are the authored proposal. Source spell parents precede their children with strictly increasing thresholds in the Sorcerer variants. Siblings may share thresholds. Learning a spell never awards grade7 simply because the shared class skill is high.

Controlled grade stays per canonical identity/spell and begins at1 in both modes. Raw proficiency is separate. Grade mastery fractions0,0.20,0.40,0.55,0.70,0.85,0.95 use route cap-relative scale; grade7 is reachable at Classic cap90/Mend60 and Simple cap100. Mend's Classic cap60 must not trim the shared Simple main skill. Reuse event-driven acquisition and PR778 convergence; no new global polling scheduler or skill-possession knowledge shortcut. Shared trait/per-spell opportunity gates prevent multiple spells from multiplying training opportunities.

Create seven elemental reserve bindings and one canonical Sorcerer reserve shared by its three policy variants. Initial reserves are zero. Capacity uses the selected body's native mental/spiritual attribute through the existing cap prog, with the source-shaped `100+3*(attribute-13)` expression clamped to `2*attribute..12*attribute`, without historical race constants. Sorcerer allows configured Self/Gentle/Land, Preserver Self/Gentle and Defiler Land; these are explicit editable engineering policies. Elemental passive rate starts at2 native units/minute while conscious/focused, doubled at rest. No practice pool growth, body-change refill or duplicate pool per policy variant. Preserve existing environmental accounting and legal gathering restrictions.

Do not use the old blanket `B*grade` energy fields as final source authority. Each inventory row records a current-factory minimum, selected printed minimum, or an unresolved source header/function resolution. Printed Sorcerer minimum is not automatically the C skill-table minimum field. Full implementation must author all152 real minima and source-shaped efficiency through the shared quote service. Quiet is harder and dearer; initial proposal +1 difficulty and x2 designated energy, overreach +1/x1.5. Area target sets/costs are per spell and require source qualification; the old generic ally-excluding strings are not sufficient. Practice is target-free, paid30-second work retaining focus, no real manifestation/material generation; controlled/raw opportunities remain separate.

## Read Attunement and typed links

Elemental affinity is an explicit builder-authored typed property/provider, attached through known race/merit/creature sources and using the seven native elemental names. No name/species/skill inference. Each seeded creature bundle below receives its stated affinity; ordinary characters default to unassigned. Any small missing shared interface/persistence/builder surface must be implemented and documented rather than faked with output text.

Learned magical affiliations are a separate query of legitimate permanent capability enrolment and acquired-spell provenance. Display affiliation names and current active/inactive route permission separately from affinity; temporary item grants do not create learned affiliations. An inactive permanent route does not erase learned history. Queries are bounded by the target's normal visibility/permission/resistance rules, perform no enrolment/grant/training, and disclose no debug IDs or hidden progs. The spell's base source minimum is20 and proposed overlay duration60*grade seconds.

All references in installer/lifecycle/archive work use explicit owned manifest bindings, native model foreign keys or code-proven typed XML fields/serializers. No scanning definitions for numeric IDs, no guessed `*Id` reflection, no blanket AI/autobuilder classification. New AI/entity formats declare exact referenced types and known fields with tests at their consumer boundary; unknown payloads hold safely. A stable key is content identity, not permission to adopt a similarly named builder object.

## Automatic support catalogue

Every blueprint below is authored by the package without prototype questions. Builders decide who gains access and where supplies/NPC templates are made available; they do not finish anatomy, attacks, AI, recipes or effect mechanics. Reuse established native factories/component schemas, approved revision patterns and seeder body-generation helpers; derive independent owned magical races where necessary rather than modifying shared Human/Animal races.

Seven creature families each have seven approved Simple NPC templates, fully bound race/ethnicity/culture/anatomy and native stats. Initial authored stats are attribute10+2*grade, combat20+5*grade, natural damage3*grade/pain grade/stun0/stamina5. Ground/winged anatomy really supplies declared locomotion. Grade selection picks the template; it does not mutate a shared NPC prototype. All use120*grade seconds/TemporaryCleanup unless a per-entry rule says otherwise. Air Guardian creates grade independent guardians; Call Outsider creates one with an explicitly labelled grade-dependent native control risk. No starter equipment/reserve refill and no additional-body merits/prostheses/role adjustments that the native lifecycle rejects.

Earth Mount needs a MountAI with MaxRiders1 and exact creator rider-permission prog plus native rider-size/posture/co-location/slot gates. The BasicMount **item** component is unrelated to character riding. Guard/follow use the existing creator bond/native combat seams. Seeking Wind needs bounded tracking/pathing, Wind Courier actual item custody/delivery, Haunting Shape fear/visibility and Outsider control; unsupported actions require a bounded native AI/adapter, not a named but inert AI record. Raise Servitor uses the separate animatecorpse path with original body/identity/custody and Commandable+CombatEnd AI, not a newly spawned generic template.

Weapon blueprints supply actual MeleeWeapon attacks and shape/quality/weight: six timed Flame knives plus eight permanent burning staves; seven each Sand Knife, Shadow Blade, Champion Blade, Spark Lash and Storm Spear. Current Flame/Sand/Storm source-informed lifetime/placement rules override generic creation defaults. All new attack stats are authored adaptation (5+2*grade damage,2*grade pain,0stun,5stamina, Standard quality); unknown historical object stats are not reconstructed. Temporary output cannot mint salvage. Permanent Flame tier requires its actual Conjuration component and native salvage rules.

Foci/devices/supplies use native craft recipes and existing material/gather routes. Ten retained tradition foci, blank wand capacity5 and staff10 start uncharged and caster-only; one charge takes60seconds and the legitimate stored-power quote/materials. Inscribed seals/wood blanks/inert component blanks produce real consumable component ranks0..4 for the12 historical sphere names, plus the separate high Conjuration rank6 component. Higher rank supply crafting uses actual native Component Crafting thresholds/inputs; it is not a spell-learning prerequisite. No blanket reagent/focus requirement is applied to normal spells. Restorative Mud is one frozen topical dose with calibrated native exposure and a bounded healing budget; other candidate substances remain disabled until their vector/carrier/compatibility is authored and qualified.

Spring Haven, Burrow Refuge, Sand Shelter and Severing Refuge get authored native interior/terrain/entrance configurations without installing occupied rooms. Their proposed lifetime is300*grade seconds/capacity grade, including offline occupants; Spring Haven contains a finite paid0.5*grade litre water supply. Safe return uses the captured source or validated existing safe starting-location binding. Missing safe return holds a closed recoverable space with diagnostics. Severing Refuge's exact native incoming/outgoing elemental ward does not confer physical invulnerability. Folded Pocket uses2*grade kg full-content weight/300*grade seconds/Bearer access and conservative native item custody. Effigy/Walking Shadow use canonical secondary instances without cloned inventory/resources.

## Ownership, rerun and implementation gates

Extend the current SeederManagedRecordReconciler's field/link baselines and typed module contributions. New/missing owned rows, edited fields, conflicts and retired/tombstoned entries retain distinct dispositions. Preserve logical IDs/revisions and reserve historic claims. Use exact native typed component/NPC/role links in dependency closure. A name rename never loses ownership, a clone never receives the original claim/player state, and intentionally retired/deleted content is not silently resurrected. An unacknowledged commit is reconciled from durable receipts, never repeated blindly.

The class skill/merit link and approved NPC/component revisions need explicit owned-field/link manifests and tests, not a whole-row overwrite. Installation and reruns never change acquisition, enrolment, native traits, controlled grade, opportunity clocks, reserve balances, charged GUIDs, inventory, body/identity or item custody. Validate every relevant player-state table checksum in native N24; absence from a narrow handpicked list is not preservation proof. No migration/conversion or multi-version branches are added.

N24 follows the complete installed-stock dependency, not merely this document. The implementation slices below make the package concrete before N24's ownership/rerun proof. Relevant managed suites, frozen source receipts and ordinary-player native scenarios are required for code changes; this design checkpoint runs structural checks only. Preserve earlier N14–N23 receipts as bounded historical evidence and requalify any changed installed stock.

| Slice | Work and exit evidence |
| --- | --- |
| S1 — Fixed installer settings and typed preflight | question/replay contract, class-source agreement, owned settings and partial-resume boundaries, no player writes |
| S2 — Complete progression/capabilities/classes | full Simple tables and exact available Classic tables, ten capabilities/merits and conditional roles, natural grade/raw practice; Classic elemental source gate cleared |
| S3 — Automatic NPC/item/supply/template catalogue | all blueprints produce approved native entities, usable attacks/riding/guard/seek/courier, native recipes/gather supplies and exact typed dependency closure |
| S4 — Full spell catalogue and remaining adapters | all 152 required defining contracts, eight systems, all grades/source-informed material/timing/quiet/area data, separate Read Attunement facts; optional additions isolated |
| S5 — Production/device/substance and installed-world progression | qualified allowlists, real frozen-payload production and dose calibration, normal acquisition/practice/costs across both modes, no free fallback |
| S6 — Fresh-install completeness | four matrix cells, Debug/Release seeder and relevant suites, fresh disposable replay and second-replay refusal, ordinary nonadmin guide; Release entrypoint still gated |
| N24 — Full installer rerun qualification | edited/renamed/retired/deleted/cloned/interrupted stock with stable IDs and all player-state checksums conserved |
| N25 — Cross-feature uncertain failure qualification | paid effect/progression/lifecycle injected failure, other body/route/restart recovery, no replay/refund/duplicate state |
| FINAL — Whole-programme evidence and publication gate | per-entry complete ledger, progression/balance report, eight systems, no unexplained blockers; only then consider Release enablement |

The complete install matrix is Classic/classes off, Classic/classes on, Simple/classes off and Simple/classes on. Each creates all ten capabilities, correct zero/ten Class roles, every required stock definition/support/prototype, legitimate natural automatic acquisition and grade/raw practice, no seeded live NPCs or player mutation. Run seeder Debug/Release builds, focused question/replay/ownership tests and the full Seeder suite with baselines reported separately. On disposable newly migrated MySQL worlds, run supported fresh replay and prove a second Debug replay refuses without mutation. Ownership-aware installer reruns are a separate supported operation. Update every strict replay profile inventory/order/question ID deliberately, including inactive questions. Release enablement remains held until the whole-programme gates pass.

The only presently known decision/data dependency is the missing Classic elemental source progression. Remaining numeric balance tuning and dose/attack calibration are implementation validation work, not extra user questions about stock prototypes. Wardcraft/Renew Earth stay optional, unadmitted builder additions outside all required stock routes; no required spell depends on them.

## Simple progression tables

These are authored thresholds, not historical Classic numbers. Each table is complete for its capability. `Start` opens grade1; other rows unlock automatically from the main raw skill, subject to actual functional support such as Component Crafting80 for Read Enchantment.

### Earth

Main skill `arm.skill.earth_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Earthen Ward | 10 | Start |
| Sand Knife | 15 |  |
| Sapping Weight | 18 |  |
| Trace the Path | 22 |  |
| Return Tether | 26 |  |
| Sand Shelter | 30 |  |
| Clay Sentinel | 33 |  |
| Shatter Stone | 37 |  |
| Roused Fury | 41 |  |
| Sustain Meal | 45 |  |
| Earth Mount | 48 |  |
| Sand Barrier | 52 |  |
| Restore Object | 56 |  |
| Fleet Step | 60 |  |
| Burrow Refuge | 63 |  |
| Ground Tremor | 67 |  |
| Stone Skin | 71 |  |
| Threshold Alarm | 75 |  |
| Sand Effigy | 78 |  |
| Clouded Will | 82 |  |
| Heavy Slumber | 86 |  |
| Earthen Strength | 90 |  |

### Fire

Main skill `arm.skill.fire_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Sense Enchantment | 10 | Start |
| Unravel Enchantment | 15 |  |
| Cinder Rain | 19 |  |
| Pillar of Flame | 23 |  |
| Fireworks | 28 |  |
| Burning Draught Curse | 32 |  |
| Desiccate | 37 |  |
| Flame Mantle | 41 |  |
| Ember Seed | 45 |  |
| Daylight | 50 |  |
| Ember Lance | 54 |  |
| Flame Knife | 59 |  |
| Borrowed Tongues | 63 |  |
| Custodian Glyph | 67 |  |
| Hovering Light | 72 |  |
| Kindle Enchantment | 76 |  |
| Immolate | 81 |  |
| Flame Barrier | 85 |  |
| Consuming Flame | 90 |  |

### Water

Main skill `arm.skill.water_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Draw Water | 10 | Start |
| Unyielding Veil | 15 |  |
| Spring Haven | 17 |  |
| Thunderclap | 20 |  |
| Hush Hearing | 23 |  |
| Seal Voice | 26 |  |
| Vital Siphon | 29 |  |
| Thorn Barrier | 32 |  |
| Headlong Revel | 35 |  |
| Clear Head | 38 |  |
| Wither Growth | 40 |  |
| Distant Mirage | 43 |  |
| Mist Shield | 46 |  |
| Restorative Mud | 49 |  |
| Purge Disease | 52 |  |
| Drowning Grip | 55 |  |
| Sow Sickness | 58 |  |
| Caustic Spray | 61 |  |
| Gather Puddle | 64 |  |
| Draw Wine | 66 |  |
| Still Anger | 69 |  |
| Sense Toxin | 72 |  |
| Read Attunement | 75 |  |
| Water Breathing | 78 |  |
| Venom Touch | 81 |  |
| Mend Flesh | 84 |  |
| Purge Toxin | 87 |  |
| Sheltering Veil | 90 |  |

### Wind

Main skill `arm.skill.wind_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Gust Hands | 10 | Start |
| Scouring Gust | 15 |  |
| Exchange Places | 18 |  |
| Seeking Wind | 22 |  |
| Gentle Descent | 25 |  |
| Wind Hammer | 29 |  |
| False Presence | 32 |  |
| Wind Shield | 36 |  |
| Pierce Concealment | 40 |  |
| Veil from Sight | 43 |  |
| Waystep | 47 |  |
| Buoyant Lift | 50 |  |
| Reveal the Hidden | 54 |  |
| Banish Traveller | 57 |  |
| Driving Gust | 61 |  |
| Windborne Flight | 65 |  |
| Wind Mantle | 68 |  |
| Air Guardian | 72 |  |
| Draw Traveller | 75 |  |
| Wind Barrier | 79 |  |
| Join Traveller | 82 |  |
| Wind Courier | 86 |  |
| Anchor Rune | 90 |  |

### Shadow

Main skill `arm.skill.shadow_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Night Eyes | 10 | Start |
| Cooling Shade | 15 |  |
| Haunting Shape | 20 |  |
| Shadow Blade | 25 |  |
| Shadowplay | 30 |  |
| Shadow Mantle | 35 |  |
| Champion Blade | 40 |  |
| See the Unbodied | 45 |  |
| Walking Shadow | 50 |  |
| Shadow Passage | 55 |  |
| Shroud Eyes | 60 |  |
| Clear Eyes | 65 |  |
| Lift Hex | 70 |  |
| Clinging Hex | 75 |  |
| Draw into Flesh | 80 |  |
| Dread Presence | 85 |  |
| Deep Darkness | 90 |  |

### Lightning

Main skill `arm.skill.lightning_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Lightning Lance | 10 | Start |
| Second Breath | 15 |  |
| Wakeful Mind | 20 |  |
| Storm Locus | 26 |  |
| Luminous Trail | 32 |  |
| Quickened Healing | 38 |  |
| Spark Lash | 43 |  |
| Sky Lantern | 49 |  |
| Charged Aegis | 55 |  |
| Leadfoot | 61 |  |
| Leaping Lightning | 66 |  |
| Lightning Pace | 72 |  |
| Storm Spear | 78 |  |
| Steal Breath | 84 |  |
| Still Limbs | 90 |  |

### Void

Main skill `arm.skill.void_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Arcane Mark | 10 | Start |
| Silence the Mind | 15 |  |
| Devouring Touch | 17 |  |
| Compelling Regard | 20 |  |
| Unmaking Ward | 23 |  |
| Severing Refuge | 26 |  |
| Apex Bane | 28 |  |
| Bar the Elements | 31 |  |
| Turn the Elements | 34 |  |
| Folded Pocket | 37 |  |
| Veil of Elements | 40 |  |
| Cross the Veil | 42 |  |
| Borrow the Dead | 45 |  |
| Mind Scour | 48 |  |
| Phantasm | 51 |  |
| Unbodied Journey | 53 |  |
| Erode Object | 56 |  |
| Leeching Edge | 59 |  |
| Sow Sickness | 62 |  |
| Blade Barrier | 65 |  |
| Call Outsider | 67 |  |
| Feign Death | 70 |  |
| Open Threshold | 73 |  |
| Linked Threshold | 76 |  |
| Read Enchantment | 78 |  |
| Raise Servitor | 81 |  |
| Empty Aura | 84 |  |
| Voice of Remains | 87 |  |
| Echo Servant | 90 |  |

### Sorcerer

Main skill `arm.skill.sorcerer_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Sense Enchantment | 10 | Start |
| Unravel Enchantment | 10 | Start |
| Unyielding Veil | 10 | Start |
| Gust Hands | 10 | Start |
| Pierce Concealment | 15 |  |
| Veil from Sight | 15 |  |
| Draw Water | 16 |  |
| Roused Fury | 17 |  |
| Waystep | 18 |  |
| Buoyant Lift | 19 |  |
| Lightning Lance | 20 |  |
| Charged Aegis | 21 |  |
| Night Eyes | 22 |  |
| See the Unbodied | 23 |  |
| Walking Shadow | 24 |  |
| Shadow Passage | 25 |  |
| Reveal the Hidden | 26 |  |
| Sustain Meal | 27 |  |
| Earth Mount | 28 |  |
| Draw Wine | 29 |  |
| Shroud Eyes | 30 |  |
| Still Anger | 31 |  |
| Banish Traveller | 32 |  |
| Driving Gust | 33 |  |
| Arcane Mark | 34 |  |
| Windborne Flight | 35 |  |
| Ember Lance | 36 |  |
| Flame Knife | 37 |  |
| Sand Barrier | 38 |  |
| Blade Barrier | 39 |  |
| Wind Mantle | 40 |  |
| Clear Eyes | 41 |  |
| Lift Hex | 42 |  |
| Sense Toxin | 43 |  |
| Borrowed Tongues | 44 |  |
| Custodian Glyph | 45 |  |
| Clinging Hex | 46 |  |
| Read Attunement | 47 |  |
| Draw into Flesh | 48 |  |
| Hovering Light | 49 |  |
| Kindle Enchantment | 50 |  |
| Restore Object | 51 |  |
| Air Guardian | 52 |  |
| Call Outsider | 52 |  |
| Water Breathing | 53 |  |
| Venom Touch | 54 |  |
| Feign Death | 55 |  |
| Mend Flesh | 56 |  |
| Purge Toxin | 57 |  |
| Draw Traveller | 58 |  |
| Leadfoot | 59 |  |
| Open Threshold | 60 |  |
| Wind Barrier | 61 |  |
| Fleet Step | 62 |  |
| Linked Threshold | 63 |  |
| Join Traveller | 64 |  |
| Burrow Refuge | 65 |  |
| Ground Tremor | 66 |  |
| Leaping Lightning | 67 |  |
| Dread Presence | 68 |  |
| Immolate | 69 |  |
| Sheltering Veil | 70 |  |
| Stone Skin | 71 |  |
| Flame Barrier | 72 |  |
| Read Enchantment | 73 |  |
| Threshold Alarm | 74 |  |
| Sand Effigy | 75 |  |
| Wind Courier | 76 |  |
| Clouded Will | 77 |  |
| Deep Darkness | 78 |  |
| Heavy Slumber | 79 |  |
| Raise Servitor | 80 |  |
| Lightning Pace | 81 |  |
| Anchor Rune | 82 |  |
| Storm Spear | 83 |  |
| Empty Aura | 84 |  |
| Steal Breath | 85 |  |
| Consuming Flame | 86 |  |
| Earthen Strength | 87 |  |
| Still Limbs | 88 |  |
| Voice of Remains | 89 |  |
| Echo Servant | 90 |  |

### Preserver

Main skill `arm.skill.preserver_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Sense Enchantment | 10 | Start |
| Unravel Enchantment | 10 | Start |
| Unyielding Veil | 10 | Start |
| Gust Hands | 10 | Start |
| Pierce Concealment | 15 |  |
| Veil from Sight | 15 |  |
| Draw Water | 16 |  |
| Roused Fury | 17 |  |
| Waystep | 18 |  |
| Buoyant Lift | 19 |  |
| Lightning Lance | 20 |  |
| Charged Aegis | 21 |  |
| Night Eyes | 22 |  |
| See the Unbodied | 23 |  |
| Walking Shadow | 24 |  |
| Shadow Passage | 25 |  |
| Reveal the Hidden | 26 |  |
| Sustain Meal | 27 |  |
| Earth Mount | 28 |  |
| Draw Wine | 29 |  |
| Shroud Eyes | 30 |  |
| Still Anger | 31 |  |
| Banish Traveller | 32 |  |
| Driving Gust | 33 |  |
| Arcane Mark | 34 |  |
| Windborne Flight | 35 |  |
| Ember Lance | 36 |  |
| Flame Knife | 37 |  |
| Sand Barrier | 38 |  |
| Blade Barrier | 39 |  |
| Wind Mantle | 40 |  |
| Clear Eyes | 41 |  |
| Lift Hex | 42 |  |
| Sense Toxin | 43 |  |
| Borrowed Tongues | 44 |  |
| Custodian Glyph | 45 |  |
| Clinging Hex | 46 |  |
| Read Attunement | 47 |  |
| Draw into Flesh | 48 |  |
| Hovering Light | 49 |  |
| Kindle Enchantment | 50 |  |
| Restore Object | 51 |  |
| Air Guardian | 52 |  |
| Call Outsider | 52 |  |
| Water Breathing | 53 |  |
| Venom Touch | 54 |  |
| Feign Death | 55 |  |
| Mend Flesh | 56 |  |
| Purge Toxin | 57 |  |
| Draw Traveller | 58 |  |
| Leadfoot | 59 |  |
| Open Threshold | 60 |  |
| Wind Barrier | 61 |  |
| Fleet Step | 62 |  |
| Linked Threshold | 63 |  |
| Join Traveller | 64 |  |
| Burrow Refuge | 65 |  |
| Ground Tremor | 66 |  |
| Leaping Lightning | 67 |  |
| Dread Presence | 68 |  |
| Immolate | 69 |  |
| Sheltering Veil | 70 |  |
| Stone Skin | 71 |  |
| Flame Barrier | 72 |  |
| Read Enchantment | 73 |  |
| Threshold Alarm | 74 |  |
| Sand Effigy | 75 |  |
| Wind Courier | 76 |  |
| Clouded Will | 77 |  |
| Deep Darkness | 78 |  |
| Heavy Slumber | 79 |  |
| Raise Servitor | 80 |  |
| Lightning Pace | 81 |  |
| Anchor Rune | 82 |  |
| Storm Spear | 83 |  |
| Empty Aura | 84 |  |
| Steal Breath | 85 |  |
| Consuming Flame | 86 |  |
| Earthen Strength | 87 |  |
| Still Limbs | 88 |  |
| Voice of Remains | 89 |  |
| Echo Servant | 90 |  |

### Defiler

Main skill `arm.skill.defiler_casting`; opening10, cap100.

| Spell | Skill threshold | Starting |
| --- | ---: | --- |
| Sense Enchantment | 10 | Start |
| Unravel Enchantment | 10 | Start |
| Unyielding Veil | 10 | Start |
| Gust Hands | 10 | Start |
| Pierce Concealment | 15 |  |
| Veil from Sight | 15 |  |
| Draw Water | 16 |  |
| Roused Fury | 17 |  |
| Waystep | 18 |  |
| Buoyant Lift | 19 |  |
| Lightning Lance | 20 |  |
| Charged Aegis | 21 |  |
| Night Eyes | 22 |  |
| See the Unbodied | 23 |  |
| Walking Shadow | 24 |  |
| Shadow Passage | 25 |  |
| Reveal the Hidden | 26 |  |
| Sustain Meal | 27 |  |
| Earth Mount | 28 |  |
| Draw Wine | 29 |  |
| Shroud Eyes | 30 |  |
| Still Anger | 31 |  |
| Banish Traveller | 32 |  |
| Driving Gust | 33 |  |
| Arcane Mark | 34 |  |
| Windborne Flight | 35 |  |
| Ember Lance | 36 |  |
| Flame Knife | 37 |  |
| Sand Barrier | 38 |  |
| Blade Barrier | 39 |  |
| Wind Mantle | 40 |  |
| Clear Eyes | 41 |  |
| Lift Hex | 42 |  |
| Sense Toxin | 43 |  |
| Borrowed Tongues | 44 |  |
| Custodian Glyph | 45 |  |
| Clinging Hex | 46 |  |
| Read Attunement | 47 |  |
| Draw into Flesh | 48 |  |
| Hovering Light | 49 |  |
| Kindle Enchantment | 50 |  |
| Restore Object | 51 |  |
| Air Guardian | 52 |  |
| Call Outsider | 52 |  |
| Water Breathing | 53 |  |
| Venom Touch | 54 |  |
| Feign Death | 55 |  |
| Mend Flesh | 56 |  |
| Purge Toxin | 57 |  |
| Draw Traveller | 58 |  |
| Leadfoot | 59 |  |
| Open Threshold | 60 |  |
| Wind Barrier | 61 |  |
| Fleet Step | 62 |  |
| Linked Threshold | 63 |  |
| Join Traveller | 64 |  |
| Burrow Refuge | 65 |  |
| Ground Tremor | 66 |  |
| Leaping Lightning | 67 |  |
| Dread Presence | 68 |  |
| Immolate | 69 |  |
| Sheltering Veil | 70 |  |
| Stone Skin | 71 |  |
| Flame Barrier | 72 |  |
| Read Enchantment | 73 |  |
| Threshold Alarm | 74 |  |
| Sand Effigy | 75 |  |
| Wind Courier | 76 |  |
| Clouded Will | 77 |  |
| Deep Darkness | 78 |  |
| Heavy Slumber | 79 |  |
| Raise Servitor | 80 |  |
| Lightning Pace | 81 |  |
| Anchor Rune | 82 |  |
| Storm Spear | 83 |  |
| Empty Aura | 84 |  |
| Steal Breath | 85 |  |
| Consuming Flame | 86 |  |
| Earthen Strength | 87 |  |
| Still Limbs | 88 |  |
| Voice of Remains | 89 |  |
| Echo Servant | 90 |  |

## Classic supporting rows

These are exact selected source values, not Simple magic skills.

| Key | Parent | Unlock | Opening | Cap | Branches at |
| --- | --- | ---: | ---: | ---: | ---: |
| `arm.support.component_crafting` | `arm.spell.shadow_passage` | 80 | 30 | 90 | 80 |
| `arm.support.vloran` | `arm.spell.raise_servitor` | 80 | 40 | 100 | 90 |
| `arm.support.contact` | `root` | root | 20 | 80 | 70 |
| `arm.support.barrier` | `root` | root | 5 | 80 | 70 |
| `arm.support.expel` | `arm.support.barrier` | 70 | 5 | 80 | 70 |
| `arm.support.dual_wield` | `root` | root | 5 | 20 | 10 |
| `arm.support.shield_use` | `root` | root | 5 | 10 | 0 |
| `arm.support.forage` | `root` | root | 5 | 50 | 40 |
| `arm.support.cooking` | `root` | root | 5 | 80 | 70 |
| `arm.support.analyze` | `root` | root | 10 | 90 | 80 |
| `arm.support.two_handed` | `root` | root | 5 | 20 | 10 |
| `arm.support.gather` | `root` | root | 10 | 90 | 80 |

## Prototype blueprint inventory

Exact numeric/configuration data and typed dependency keys are in the JSON catalogue. Each row is a blueprint, not proof that an entity is currently installed.

| Stable key | Native purpose |
| --- | --- |
| `arm.race.earth_mount` | Independent editable derived native race/anatomy for Earth Mount |
| `arm.ai.earth_mount` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g1` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g2` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g3` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g4` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g5` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g6` | ground walking/running; one creator-approved rider |
| `arm.npc.earth_mount.g7` | ground walking/running; one creator-approved rider |
| `arm.race.clay_sentinel` | Independent editable derived native race/anatomy for Clay Sentinel |
| `arm.ai.clay_sentinel` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g1` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g2` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g3` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g4` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g5` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g6` | native walking; protects creator with actual native combat |
| `arm.npc.clay_sentinel.g7` | native walking; protects creator with actual native combat |
| `arm.race.air_guardian` | Independent editable derived native race/anatomy for Air Guardian |
| `arm.ai.air_guardian` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g1` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g2` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g3` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g4` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g5` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g6` | native flight and land movement; protects/follows creator |
| `arm.npc.air_guardian.g7` | native flight and land movement; protects/follows creator |
| `arm.race.seeking_wind` | Independent editable derived native race/anatomy for Seeking Wind |
| `arm.ai.seeking_wind` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g1` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g2` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g3` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g4` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g5` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g6` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.npc.seeking_wind.g7` | bounded native tracking/pathing; stops at blocked destinations |
| `arm.race.wind_courier` | Independent editable derived native race/anatomy for Wind Courier |
| `arm.ai.wind_courier` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g1` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g2` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g3` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g4` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g5` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g6` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.npc.wind_courier.g7` | one foreign carried message/item; permitted recipient delivery and return |
| `arm.race.haunting_shape` | Independent editable derived native race/anatomy for Haunting Shape |
| `arm.ai.haunting_shape` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g1` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g2` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g3` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g4` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g5` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g6` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.npc.haunting_shape.g7` | native walking; resistible fear; no copied player identity or intangible combat |
| `arm.race.outsider` | Independent editable derived native race/anatomy for Outsider |
| `arm.ai.outsider` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g1` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g2` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g3` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g4` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g5` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g6` | native walking and combat; grade-dependent loss of control |
| `arm.npc.outsider.g7` | native walking and combat; grade-dependent loss of control |
| `arm.culture.constructs` | Complete native template culture binding |
| `arm.ai.servitor` | Dynamic corpse animation |
| `arm.item.flame_knife.g1` | Usable native conjured weapon |
| `arm.item.flame_knife.g2` | Usable native conjured weapon |
| `arm.item.flame_knife.g3` | Usable native conjured weapon |
| `arm.item.flame_knife.g4` | Usable native conjured weapon |
| `arm.item.flame_knife.g5` | Usable native conjured weapon |
| `arm.item.flame_knife.g6` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_1` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_2` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_3` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_4` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_5` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_6` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_7` | Usable native conjured weapon |
| `arm.item.flame_knife.permanent_choice_8` | Usable native conjured weapon |
| `arm.item.sand_knife.g1` | Usable native conjured weapon |
| `arm.item.sand_knife.g2` | Usable native conjured weapon |
| `arm.item.sand_knife.g3` | Usable native conjured weapon |
| `arm.item.sand_knife.g4` | Usable native conjured weapon |
| `arm.item.sand_knife.g5` | Usable native conjured weapon |
| `arm.item.sand_knife.g6` | Usable native conjured weapon |
| `arm.item.sand_knife.g7` | Usable native conjured weapon |
| `arm.item.shadow_blade.g1` | Usable native conjured weapon |
| `arm.item.shadow_blade.g2` | Usable native conjured weapon |
| `arm.item.shadow_blade.g3` | Usable native conjured weapon |
| `arm.item.shadow_blade.g4` | Usable native conjured weapon |
| `arm.item.shadow_blade.g5` | Usable native conjured weapon |
| `arm.item.shadow_blade.g6` | Usable native conjured weapon |
| `arm.item.shadow_blade.g7` | Usable native conjured weapon |
| `arm.item.champion_blade.g1` | Usable native conjured weapon |
| `arm.item.champion_blade.g2` | Usable native conjured weapon |
| `arm.item.champion_blade.g3` | Usable native conjured weapon |
| `arm.item.champion_blade.g4` | Usable native conjured weapon |
| `arm.item.champion_blade.g5` | Usable native conjured weapon |
| `arm.item.champion_blade.g6` | Usable native conjured weapon |
| `arm.item.champion_blade.g7` | Usable native conjured weapon |
| `arm.item.spark_lash.g1` | Usable native conjured weapon |
| `arm.item.spark_lash.g2` | Usable native conjured weapon |
| `arm.item.spark_lash.g3` | Usable native conjured weapon |
| `arm.item.spark_lash.g4` | Usable native conjured weapon |
| `arm.item.spark_lash.g5` | Usable native conjured weapon |
| `arm.item.spark_lash.g6` | Usable native conjured weapon |
| `arm.item.spark_lash.g7` | Usable native conjured weapon |
| `arm.item.storm_spear.g1` | Usable native conjured weapon |
| `arm.item.storm_spear.g2` | Usable native conjured weapon |
| `arm.item.storm_spear.g3` | Usable native conjured weapon |
| `arm.item.storm_spear.g4` | Usable native conjured weapon |
| `arm.item.storm_spear.g5` | Usable native conjured weapon |
| `arm.item.storm_spear.g6` | Usable native conjured weapon |
| `arm.item.storm_spear.g7` | Usable native conjured weapon |
| `arm.item.hovering_light` | Automatic package-authored usable output |
| `arm.item.ember_seed` | Automatic package-authored usable output |
| `arm.item.sand_effigy_anchor` | Automatic package-authored usable output |
| `arm.item.folded_pocket` | Automatic package-authored usable output |
| `arm.item.haven_water` | Automatic package-authored usable output |
| `arm.item.restorative_mud` | Automatic package-authored usable output |
| `arm.item.food.bread` | Sustain Meal fallback pool |
| `arm.item.food.fruit` | Sustain Meal fallback pool |
| `arm.item.food.stew` | Sustain Meal fallback pool |
| `arm.liquid.wine` | Draw Wine fallback recipe |
| `arm.liquid.water` | Draw Water/Haven/Gather Puddle exact native water binding |
| `arm.item.drink_container` | Production and native fluid acceptance |
| `arm.focus.earth` | Practice and explicit focus casting |
| `arm.focus.fire` | Practice and explicit focus casting |
| `arm.focus.water` | Practice and explicit focus casting |
| `arm.focus.wind` | Practice and explicit focus casting |
| `arm.focus.shadow` | Practice and explicit focus casting |
| `arm.focus.lightning` | Practice and explicit focus casting |
| `arm.focus.void` | Practice and explicit focus casting |
| `arm.focus.sorcerer` | Practice and explicit focus casting |
| `arm.focus.preserver` | Practice and explicit focus casting |
| `arm.focus.defiler` | Practice and explicit focus casting |
| `arm.item.blank_wand` | Legitimate homogeneous spell production/recharge |
| `arm.item.blank_staff` | Legitimate homogeneous spell production/recharge |
| `arm.material.abjuration.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.abjuration.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.abjuration.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.abjuration.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.abjuration.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.alteration.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.alteration.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.alteration.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.alteration.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.alteration.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.clerical.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.clerical.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.clerical.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.clerical.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.clerical.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.conjuration.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.conjuration.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.conjuration.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.conjuration.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.conjuration.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.creation.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.creation.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.creation.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.creation.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.creation.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.divination.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.divination.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.divination.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.divination.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.divination.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.enchantment.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.enchantment.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.enchantment.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.enchantment.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.enchantment.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.illusion.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.illusion.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.illusion.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.illusion.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.illusion.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.invocation.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.invocation.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.invocation.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.invocation.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.invocation.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.nature.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.nature.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.nature.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.nature.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.nature.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.necromancy.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.necromancy.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.necromancy.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.necromancy.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.necromancy.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.teleportation.rank0` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.teleportation.rank1` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.teleportation.rank2` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.teleportation.rank3` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.material.teleportation.rank4` | Source-informed per-spell consumed component, never blanket reagent demand |
| `arm.item.component_blank` | Craft all component ranks |
| `arm.item.focus_blank` | Craft tradition foci and devices |
| `arm.item.binding_seal` | Focus/device recipes |
| `arm.binding.material.wood` | Typed native craft/gather input |
| `arm.binding.material.clay` | Typed native craft/gather input |
| `arm.roomtemplate.spring_haven` | Owned temporary topology and occupancy |
| `arm.roomtemplate.burrow_refuge` | Owned temporary topology and occupancy |
| `arm.roomtemplate.sand_shelter` | Owned temporary topology and occupancy |
| `arm.roomtemplate.severing_refuge` | Owned temporary topology and occupancy |
| `arm.effect.poison` | Native exposure and treatment |
| `arm.effect.disease` | Native infection and treatment |
| `arm.material.conjuration.rank6` | Flame Knife permanent tier |

## Complete spell/dependency inventory

Every entry retains its defining completion contract and provenance in the JSON. The effect rules below are current bounded factory data plus new proposed values; none marks a full installed spell as qualified. All listed dependencies resolve to the automatic catalogue. Empty prototype dependencies mean the spell uses native effects rather than creating an output, not that its native adapter is implemented.

| # | Spell/key | Required | Proposed effect configuration | Prototype dependencies |
| ---: | --- | --- | --- | --- |
| 1 | Sense Enchantment (`arm.spell.sense_enchantment`) | Yes | Recipient senses perceptible enchantments for 3000*grade seconds; Silt refuses; exclusive refresh. | Native contract/effect adapter |
| 2 | Ember Lance (`arm.spell.ember_lance`) | Yes | 5*grade native damage, 2*grade pain, 0 stun; one target; no implicit splash. | Native contract/effect adapter |
| 3 | Cinder Rain (`arm.spell.cinder_rain`) | Yes | 3*grade initial native damage; 2*grade burning damage every 10 seconds for 30*grade seconds. | Native contract/effect adapter |
| 4 | Hovering Light (`arm.spell.hovering_light`) | Yes | One real wearable light, 1800*grade seconds, explicit native wear slot and illumination; no free held focus. | `arm.item.hovering_light` |
| 5 | Unravel Enchantment (`arm.spell.unravel_enchantment`) | Yes | Character-target exact matching native effect removal with native contest; no heterogeneous effect deletion. | Native contract/effect adapter |
| 6 | Pillar of Flame (`arm.spell.pillar_of_flame`) | Yes | 8*grade native damage, 3*grade pain, 0 stun. | Native contract/effect adapter |
| 7 | Consuming Flame (`arm.spell.consuming_flame`) | Yes | 8*grade native damage, 3*grade pain, 0 stun. | Native contract/effect adapter |
| 8 | Kindle Enchantment (`arm.spell.kindle_enchantment`) | Yes | Weapon attack bonus grade for 60*grade seconds; does not alter base quality or stack identical owned bonuses. | Native contract/effect adapter |
| 9 | Custodian Glyph (`arm.spell.custodian_glyph`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 10 | Fireworks (`arm.spell.fireworks`) | Yes | One noninteractive perceptible scene for 60*grade seconds; no collision, equipment, identity or damage rights. | Native contract/effect adapter |
| 11 | Borrowed Tongues (`arm.spell.borrowed_tongues`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 12 | Burning Draught Curse (`arm.spell.burning_draught_curse`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 13 | Desiccate (`arm.spell.desiccate`) | Yes | ThirstPoints decrease by at most 5*grade; 2*grade native damage; native need bounds. | Native contract/effect adapter |
| 14 | Flame Knife (`arm.spell.flame_knife`) | Yes | Grades 1-6 select six distinct temporary burning knives; grade 7 chooses eight distinct permanent burning staves and consumes one directly carried Conjuration rank-6 descendant. Timed lifetime (grade+1)*1500*0.75 seconds; Water plane refuses; Shadow plane halves lifetime. | `arm.item.flame_knife.g1`, `arm.item.flame_knife.g2`, `arm.item.flame_knife.g3`, `arm.item.flame_knife.g4`, `arm.item.flame_knife.g5`, `arm.item.flame_knife.g6`, `arm.item.flame_knife.permanent_choice_1`, `arm.item.flame_knife.permanent_choice_2`, `arm.item.flame_knife.permanent_choice_3`, `arm.item.flame_knife.permanent_choice_4`, `arm.item.flame_knife.permanent_choice_5`, `arm.item.flame_knife.permanent_choice_6`, `arm.item.flame_knife.permanent_choice_7`, `arm.item.flame_knife.permanent_choice_8`, `arm.material.conjuration.rank6` |
| 15 | Flame Barrier (`arm.spell.flame_barrier`) | Yes | 60*grade seconds; traversal inflicts 3*grade configured native damage after admitted crossing; no arbitrary blocked-room links. | Native contract/effect adapter |
| 16 | Flame Mantle (`arm.spell.flame_mantle`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 17 | Ember Seed (`arm.spell.ember_seed`) | Yes | One authored prototype, 300*grade seconds; real native custody and owned cleanup. | `arm.item.ember_seed` |
| 18 | Daylight (`arm.spell.daylight`) | Yes | 20*grade lux, 60*grade seconds, native illumination only. | Native contract/effect adapter |
| 19 | Immolate (`arm.spell.immolate`) | Yes | 3*grade initial native damage; 2*grade burning damage every 10 seconds for 30*grade seconds. | Native contract/effect adapter |
| 20 | Draw Water (`arm.spell.draw_water`) | Yes | 0.5*grade litres, Water plane x2, open native drink container capped; Fire plane/Silt refuse; ordinary persistent water. | `arm.liquid.water`, `arm.item.drink_container` |
| 21 | Sense Toxin (`arm.spell.sense_toxin`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 22 | Mend Flesh (`arm.spell.mend_flesh`) | Yes | 2*grade*grade native damage budget, worst eligible wound first, overflow allowed; actual healing; undead and Void restrictions via explicit progs. | Native contract/effect adapter |
| 23 | Venom Touch (`arm.spell.venom_touch`) | Yes | One package-native poison, 0.001*grade grams at a declared contact vector; native exposure owns dose and physiological lifetime. | `arm.effect.poison` |
| 24 | Sheltering Veil (`arm.spell.sheltering_veil`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 25 | Purge Toxin (`arm.spell.purge_toxin`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | `arm.effect.poison` |
| 26 | Still Anger (`arm.spell.still_anger`) | Yes | Native paid selective Calm, source counters before residual save, prepared cessation before payment; incoming admitted hostility breaks it, including a missed attack; 60*grade seconds. | Native contract/effect adapter |
| 27 | Spring Haven (`arm.spell.spring_haven`) | Yes | One occupied native indoor shelter, 300*grade seconds, physical capacity grade including offline instances; finite initial water 0.5*grade litres, no reload refill, conserved collapse. | `arm.roomtemplate.spring_haven`, `arm.item.haven_water`, `arm.liquid.water` |
| 28 | Unyielding Veil (`arm.spell.unyielding_veil`) | Yes | 20*grade native protection budget, 60*grade seconds; ordinary armour resolution. | Native contract/effect adapter |
| 29 | Read Attunement (`arm.spell.read_attunement`) | Yes | Two separate bounded results: builder-assigned elemental affinities and learned magical affiliations. Ordinary characters have no inferred affinity; seeded magical creatures have explicit affinity. Pure read, no enrolment/grant/training. | Native contract/effect adapter |
| 30 | Thunderclap (`arm.spell.thunderclap`) | Yes | 3*grade native sonic damage plus 30*grade seconds of resistible deafness. | Native contract/effect adapter |
| 31 | Hush Hearing (`arm.spell.hush_hearing`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 32 | Seal Voice (`arm.spell.seal_voice`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 33 | Vital Siphon (`arm.spell.vital_siphon`) | Yes | 5*grade native damage; caster healing <= actual delivered eligible damage and <=5*grade. | Native contract/effect adapter |
| 34 | Thorn Barrier (`arm.spell.thorn_barrier`) | Yes | 60*grade seconds; traversal inflicts 3*grade configured native damage after admitted crossing; no arbitrary blocked-room links. | Native contract/effect adapter |
| 35 | Draw Wine (`arm.spell.draw_wine`) | Yes | 0.5*grade litres fallback wine, Water plane x2, compatible open native drink container capped; no expiry; Fire plane/Silt refuse. | `arm.liquid.wine`, `arm.item.drink_container` |
| 36 | Headlong Revel (`arm.spell.headlong_revel`) | Yes | Add 0.01*grade AlcoholLitres through native need/exposure accounting, bounded by native limits. | Native contract/effect adapter |
| 37 | Clear Head (`arm.spell.clear_head`) | Yes | Remove at most 0.01*grade AlcoholLitres, floor zero; does not purge unrelated substances. | Native contract/effect adapter |
| 38 | Wither Growth (`arm.spell.wither_growth`) | Yes | 5*grade native condition damage to an eligible material/object; no recursive contents destruction. | Native contract/effect adapter |
| 39 | Distant Mirage (`arm.spell.distant_mirage`) | Yes | One noninteractive perceptible scene for 60*grade seconds; no collision, equipment, identity or damage rights. | Native contract/effect adapter |
| 40 | Mist Shield (`arm.spell.mist_shield`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 41 | Restorative Mud (`arm.spell.restorative_mud`) | Yes | One topical dose with a maximum 10*grade wound-healing budget; calibrate package dose against native drug/exposure units before enabling. | `arm.item.restorative_mud` |
| 42 | Purge Disease (`arm.spell.purge_disease`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | `arm.effect.disease` |
| 43 | Water Breathing (`arm.spell.water_breathing`) | Yes | Inclusive random max(1,floor(grade/2))..3*grade units, unit 600 seconds, cumulative max 36 units and strongest grade; exact water liquid IDs. | `arm.liquid.water` |
| 44 | Earthen Ward (`arm.spell.earthen_ward`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 45 | Sustain Meal (`arm.spell.sustain_meal`) | Yes | grade independent items from three authored native food profiles; 1350*grade seconds each; real nutrition/bites. | `arm.item.food.bread`, `arm.item.food.fruit`, `arm.item.food.stew` |
| 46 | Ground Tremor (`arm.spell.ground_tremor`) | Yes | 3*grade native damage per distinct eligible admitted target; declared per-spell inclusion policy, one progression opportunity. | Native contract/effect adapter |
| 47 | Heavy Slumber (`arm.spell.heavy_slumber`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 48 | Earthen Strength (`arm.spell.earthen_strength`) | Yes | grade native trait bonus for 60*grade seconds; exact package-bound ordinary attribute. | Native contract/effect adapter |
| 49 | Sand Knife (`arm.spell.sand_knife`) | Yes | One usable sand knife, lifetime (grade+1)*1500*0.75 real seconds; seven distinct native grade prototypes; no permanent tier. | `arm.item.sand_knife.g1`, `arm.item.sand_knife.g2`, `arm.item.sand_knife.g3`, `arm.item.sand_knife.g4`, `arm.item.sand_knife.g5`, `arm.item.sand_knife.g6`, `arm.item.sand_knife.g7` |
| 50 | Stone Skin (`arm.spell.stone_skin`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 51 | Sapping Weight (`arm.spell.sapping_weight`) | Yes | -grade native trait penalty for 60*grade seconds; exact package-bound ordinary attribute. | Native contract/effect adapter |
| 52 | Trace the Path (`arm.spell.trace_the_path`) | Yes | Bounded path query of at most 10*grade permitted native room edges; no hidden-room or door bypass. | Native contract/effect adapter |
| 53 | Burrow Refuge (`arm.spell.burrow_refuge`) | Yes | One occupied enterable underground native shelter, 300*grade seconds, capacity grade; safe native re-entry/return/occupied closure. | `arm.roomtemplate.burrow_refuge` |
| 54 | Clouded Will (`arm.spell.clouded_will`) | Yes | Remove at most min(current,5*grade) from the exact configured reserve; no caster credit or negative balance. | Native contract/effect adapter |
| 55 | Roused Fury (`arm.spell.roused_fury`) | Yes | Native paid Fury and shared emotional bridge; explicit ordinary attribute bonus grade for 60*grade seconds; no stamina refill or combat reset. | Native contract/effect adapter |
| 56 | Earth Mount (`arm.spell.earth_mount`) | Yes | One approved native quadruped with MountAI and exact creator rider permission, 120*grade seconds; rider/body size/posture/slots gates and occupied expiry conservation. | `arm.npc.earth_mount.g1`, `arm.npc.earth_mount.g2`, `arm.npc.earth_mount.g3`, `arm.npc.earth_mount.g4`, `arm.npc.earth_mount.g5`, `arm.npc.earth_mount.g6`, `arm.npc.earth_mount.g7` |
| 57 | Sand Barrier (`arm.spell.sand_barrier`) | Yes | 60*grade seconds; selected native exit/door traversal denial, no damage. | Native contract/effect adapter |
| 58 | Return Tether (`arm.spell.return_tether`) | Yes | Record one physical position and return after 30*grade seconds through admitted relocation; no world-state rewind. | Native contract/effect adapter |
| 59 | Threshold Alarm (`arm.spell.threshold_alarm`) | Yes | 120*grade seconds; one notice per eligible native entry event; no polling or hidden identity disclosure. | Native contract/effect adapter |
| 60 | Fleet Step (`arm.spell.fleet_step`) | Yes | Movement-time multiplier 1/(1+0.05*grade), 60*grade seconds; normal posture/terrain gates. | Native contract/effect adapter |
| 61 | Sand Shelter (`arm.spell.sand_shelter`) | Yes | One occupied indoor native shelter, 300*grade seconds, capacity grade; native weather shelter and safe closure. | `arm.roomtemplate.sand_shelter` |
| 62 | Clay Sentinel (`arm.spell.clay_sentinel`) | Yes | One grade-specific approved NPC for 120*grade seconds unless overridden; explicit ownership/AI/count/control. | `arm.npc.clay_sentinel.g1`, `arm.npc.clay_sentinel.g2`, `arm.npc.clay_sentinel.g3`, `arm.npc.clay_sentinel.g4`, `arm.npc.clay_sentinel.g5`, `arm.npc.clay_sentinel.g6`, `arm.npc.clay_sentinel.g7` |
| 63 | Sand Effigy (`arm.spell.sand_effigy`) | Yes | One figurine anchor and immobile owned secondary presence, 60*grade seconds; shared identity/skills/resources, no inventory clone; collapse on damage/anchor loss. | `arm.item.sand_effigy_anchor` |
| 64 | Shatter Stone (`arm.spell.shatter_stone`) | Yes | 5*grade native condition damage to an eligible material/object; no recursive contents destruction. | Native contract/effect adapter |
| 65 | Restore Object (`arm.spell.restore_object`) | Yes | Restore at most 5*grade native condition damage; no quality improvement or material manufacture. | Native contract/effect adapter |
| 66 | Waystep (`arm.spell.waystep`) | Yes | One entity to one explicitly permitted native location; exact entry-specific target adapter, no universal range bypass. | Native contract/effect adapter |
| 67 | Pierce Concealment (`arm.spell.pierce_concealment`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 68 | Veil from Sight (`arm.spell.veil_from_sight`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 69 | Draw Traveller (`arm.spell.draw_traveller`) | Yes | One entity to one explicitly permitted native location; exact entry-specific target adapter, no universal range bypass. | Native contract/effect adapter |
| 70 | Join Traveller (`arm.spell.join_traveller`) | Yes | One entity to one explicitly permitted native location; exact entry-specific target adapter, no universal range bypass. | Native contract/effect adapter |
| 71 | Buoyant Lift (`arm.spell.buoyant_lift`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 72 | Scouring Gust (`arm.spell.scouring_gust`) | Yes | 3*grade native damage per distinct eligible admitted target; declared per-spell inclusion policy, one progression opportunity. | Native contract/effect adapter |
| 73 | Gust Hands (`arm.spell.gust_hands`) | Yes | One admitted native exit step except Gust Hands at most grade steps; failed movement has no forced relocation. | Native contract/effect adapter |
| 74 | Banish Traveller (`arm.spell.banish_traveller`) | Yes | One entity to one explicitly permitted native location; exact entry-specific target adapter, no universal range bypass. | Native contract/effect adapter |
| 75 | Air Guardian (`arm.spell.air_guardian`) | Yes | grade independent grade-specific protective/following air creatures; 120*grade seconds, exact creator bonds, TemporaryCleanup; conserve foreign goods/canonical history. | `arm.npc.air_guardian.g1`, `arm.npc.air_guardian.g2`, `arm.npc.air_guardian.g3`, `arm.npc.air_guardian.g4`, `arm.npc.air_guardian.g5`, `arm.npc.air_guardian.g6`, `arm.npc.air_guardian.g7` |
| 76 | Exchange Places (`arm.spell.exchange_places`) | Yes | One pair of permitted physical positions; identical positions produce NoChange. | Native contract/effect adapter |
| 77 | Windborne Flight (`arm.spell.windborne_flight`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 78 | Seeking Wind (`arm.spell.seeking_wind`) | Yes | One grade-specific approved NPC for 120*grade seconds unless overridden; explicit ownership/AI/count/control. | `arm.npc.seeking_wind.g1`, `arm.npc.seeking_wind.g2`, `arm.npc.seeking_wind.g3`, `arm.npc.seeking_wind.g4`, `arm.npc.seeking_wind.g5`, `arm.npc.seeking_wind.g6`, `arm.npc.seeking_wind.g7` |
| 79 | Gentle Descent (`arm.spell.gentle_descent`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 80 | Driving Gust (`arm.spell.driving_gust`) | Yes | One admitted native exit step except Gust Hands at most grade steps; failed movement has no forced relocation. | Native contract/effect adapter |
| 81 | Wind Barrier (`arm.spell.wind_barrier`) | Yes | 60*grade seconds; selected native exit/door traversal denial, no damage. | Native contract/effect adapter |
| 82 | Wind Mantle (`arm.spell.wind_mantle`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 83 | Wind Hammer (`arm.spell.wind_hammer`) | Yes | 5*grade native damage plus at most one admitted exit step. | Native contract/effect adapter |
| 84 | False Presence (`arm.spell.false_presence`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 85 | Wind Shield (`arm.spell.wind_shield`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 86 | Wind Courier (`arm.spell.wind_courier`) | Yes | One grade-specific approved NPC for 120*grade seconds unless overridden; explicit ownership/AI/count/control. | `arm.npc.wind_courier.g1`, `arm.npc.wind_courier.g2`, `arm.npc.wind_courier.g3`, `arm.npc.wind_courier.g4`, `arm.npc.wind_courier.g5`, `arm.npc.wind_courier.g6`, `arm.npc.wind_courier.g7` |
| 87 | Reveal the Hidden (`arm.spell.reveal_the_hidden`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | Native contract/effect adapter |
| 88 | Anchor Rune (`arm.spell.anchor_rune`) | Yes | One owner-scoped metadata mark for 300*grade seconds; typed exact anchor, no name/ID scan. | Native contract/effect adapter |
| 89 | Shroud Eyes (`arm.spell.shroud_eyes`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 90 | Clear Eyes (`arm.spell.clear_eyes`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | Native contract/effect adapter |
| 91 | Lift Hex (`arm.spell.lift_hex`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | Native contract/effect adapter |
| 92 | Dread Presence (`arm.spell.dread_presence`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 93 | Night Eyes (`arm.spell.night_eyes`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 94 | Walking Shadow (`arm.spell.walking_shadow`) | Yes | One focused native-exit mobile secondary presence, 60*grade seconds, max 10*grade room edges from primary anchor; vulnerable tranced primary body; no material combat/manipulation. | Native contract/effect adapter |
| 95 | Shadow Passage (`arm.spell.shadow_passage`) | Yes | One explicitly configured plane overlay for 60*grade seconds; no unrestricted plane or ward bypass. | Native contract/effect adapter |
| 96 | See the Unbodied (`arm.spell.see_the_unbodied`) | Yes | Self-only perception, add 1800*grade seconds, max 36 hours, strongest grade; Silt refuses; one directly carried Divination rank max(grade-3,0) unless explicit Shadow-plane exemption. | `arm.material.divination.rank0`, `arm.material.divination.rank1`, `arm.material.divination.rank2`, `arm.material.divination.rank3`, `arm.material.divination.rank4` |
| 97 | Cooling Shade (`arm.spell.cooling_shade`) | Yes | Reduce illumination by 10*grade lux and ambient temperature by 0.5*grade Celsius for 60*grade seconds; native bounds. | Native contract/effect adapter |
| 98 | Deep Darkness (`arm.spell.deep_darkness`) | Yes | Reduce illumination by 20*grade lux for 60*grade seconds, native lower bound. | Native contract/effect adapter |
| 99 | Clinging Hex (`arm.spell.clinging_hex`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 100 | Haunting Shape (`arm.spell.haunting_shape`) | Yes | One grade-specific approved NPC for 120*grade seconds unless overridden; explicit ownership/AI/count/control. | `arm.npc.haunting_shape.g1`, `arm.npc.haunting_shape.g2`, `arm.npc.haunting_shape.g3`, `arm.npc.haunting_shape.g4`, `arm.npc.haunting_shape.g5`, `arm.npc.haunting_shape.g6`, `arm.npc.haunting_shape.g7` |
| 101 | Shadow Blade (`arm.spell.shadow_blade`) | Yes | One usable approved melee prototype; ordinary placement and combat; explicit per-entry tier and lifetime overrides. | `arm.item.shadow_blade.g1`, `arm.item.shadow_blade.g2`, `arm.item.shadow_blade.g3`, `arm.item.shadow_blade.g4`, `arm.item.shadow_blade.g5`, `arm.item.shadow_blade.g6`, `arm.item.shadow_blade.g7` |
| 102 | Shadowplay (`arm.spell.shadowplay`) | Yes | One noninteractive perceptible scene for 60*grade seconds; no collision, equipment, identity or damage rights. | Native contract/effect adapter |
| 103 | Shadow Mantle (`arm.spell.shadow_mantle`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 104 | Draw into Flesh (`arm.spell.draw_into_flesh`) | Yes | One contract-selected native condition family at mapped power; never erase unrelated disease, drugs, wounds or effects. | Native contract/effect adapter |
| 105 | Champion Blade (`arm.spell.champion_blade`) | Yes | One usable approved melee prototype; ordinary placement and combat; explicit per-entry tier and lifetime overrides. | `arm.item.champion_blade.g1`, `arm.item.champion_blade.g2`, `arm.item.champion_blade.g3`, `arm.item.champion_blade.g4`, `arm.item.champion_blade.g5`, `arm.item.champion_blade.g6`, `arm.item.champion_blade.g7` |
| 106 | Lightning Lance (`arm.spell.lightning_lance`) | Yes | 5*grade native damage, 2*grade pain, 0 stun; one target; no implicit splash. | Native contract/effect adapter |
| 107 | Second Breath (`arm.spell.second_breath`) | Yes | Restore at most 5*grade stamina, clamped to native maximum. | Native contract/effect adapter |
| 108 | Wakeful Mind (`arm.spell.wakeful_mind`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 109 | Leadfoot (`arm.spell.leadfoot`) | Yes | Movement-time multiplier 1+0.05*grade, 60*grade seconds; native movement still required. | Native contract/effect adapter |
| 110 | Steal Breath (`arm.spell.steal_breath`) | Yes | Remove at most min(current,5*grade) stamina, floor zero. | Native contract/effect adapter |
| 111 | Still Limbs (`arm.spell.still_limbs`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 112 | Leaping Lightning (`arm.spell.leaping_lightning`) | Yes | At most min(grade+1,8) distinct eligible room targets; damage 5*grade*0.75**jump; no repeated target or improvement. | Native contract/effect adapter |
| 113 | Storm Locus (`arm.spell.storm_locus`) | Yes | 30*grade seconds; every 10 seconds apply 2*grade electrical damage to contract-selected distinct targets; one cast progression. | Native contract/effect adapter |
| 114 | Charged Aegis (`arm.spell.charged_aegis`) | Yes | 10*grade native protection budget, 60*grade seconds; material and damage-type profile supplied by package. | Native contract/effect adapter |
| 115 | Luminous Trail (`arm.spell.luminous_trail`) | Yes | Native tracking intensity 1+0.25*grade for 60*grade seconds; normal tracking permissions. | Native contract/effect adapter |
| 116 | Quickened Healing (`arm.spell.quickened_healing`) | Yes | Native healing multiplier 1+0.1*grade for 120*grade seconds; no immediate wound healing. | Native contract/effect adapter |
| 117 | Lightning Pace (`arm.spell.lightning_pace`) | Yes | Movement-time multiplier 1/(1+0.05*grade), 60*grade seconds; normal posture/terrain gates. | Native contract/effect adapter |
| 118 | Spark Lash (`arm.spell.spark_lash`) | Yes | One usable approved melee prototype; ordinary placement and combat; explicit per-entry tier and lifetime overrides. | `arm.item.spark_lash.g1`, `arm.item.spark_lash.g2`, `arm.item.spark_lash.g3`, `arm.item.spark_lash.g4`, `arm.item.spark_lash.g5`, `arm.item.spark_lash.g6`, `arm.item.spark_lash.g7` |
| 119 | Sky Lantern (`arm.spell.sky_lantern`) | Yes | One noninteractive perceptible scene for 60*grade seconds; no collision, equipment, identity or damage rights. | Native contract/effect adapter |
| 120 | Storm Spear (`arm.spell.storm_spear`) | Yes | One usable electrical spear, primary-hand placement or prepayment refusal, lifetime (24+rand(1,6))*90*grade*0.75 seconds. | `arm.item.storm_spear.g1`, `arm.item.storm_spear.g2`, `arm.item.storm_spear.g3`, `arm.item.storm_spear.g4`, `arm.item.storm_spear.g5`, `arm.item.storm_spear.g6`, `arm.item.storm_spear.g7` |
| 121 | Raise Servitor (`arm.spell.raise_servitor`) | Yes | One eligible corpse-derived native controlled servitor, 60*grade seconds; exact original corpse identity/body/custody, Commandable+CombatEnd AI and safe closure. | `arm.ai.servitor` |
| 122 | Call Outsider (`arm.spell.call_outsider`) | Yes | One grade-specific outsider for 120*grade seconds; native control check; proposed control-loss chance min(0.5,0.05*grade) on each admitted control opportunity, with no repeat/restart reroll. | `arm.npc.outsider.g1`, `arm.npc.outsider.g2`, `arm.npc.outsider.g3`, `arm.npc.outsider.g4`, `arm.npc.outsider.g5`, `arm.npc.outsider.g6`, `arm.npc.outsider.g7` |
| 123 | Arcane Mark (`arm.spell.arcane_mark`) | Yes | One owner-scoped metadata mark for 300*grade seconds; typed exact anchor, no name/ID scan. | Native contract/effect adapter |
| 124 | Feign Death (`arm.spell.feign_death`) | Yes | Named effect in the completion contract; duration 60*grade seconds; binary permission, no implied trait bonus. | Native contract/effect adapter |
| 125 | Silence the Mind (`arm.spell.silence_the_mind`) | Yes | One explicitly selected native magic-tag set for 30*grade seconds; no generic account/command suppression. | Native contract/effect adapter |
| 126 | Devouring Touch (`arm.spell.devouring_touch`) | Yes | 5*grade native damage; caster healing <= actual delivered eligible damage and <=5*grade. | Native contract/effect adapter |
| 127 | Compelling Regard (`arm.spell.compelling_regard`) | Yes | Resistible PC/NPC influence for 30*grade seconds; IC exact command whitelist, native command admission, no alias/indirect/OOC privilege bypass; per-spell PC toggle. | Native contract/effect adapter |
| 128 | Unmaking Ward (`arm.spell.unmaking_ward`) | Yes | One configured native school/tag match for 60*grade seconds, ordinary opposition; no all-effects reflection. | Native contract/effect adapter |
| 129 | Severing Refuge (`arm.spell.severing_refuge`) | Yes | One occupied indoor refuge, 300*grade seconds, capacity grade; exact owned incoming/outgoing seven-element native school ward in fail mode; physical hazards unchanged. | `arm.roomtemplate.severing_refuge` |
| 130 | Apex Bane (`arm.spell.apex_bane`) | Yes | Only explicit magical-creature tag/affinity eligibility; configured native resistance, min(56,8*grade) native damage; no universal nonhuman eligibility. | Native contract/effect adapter |
| 131 | Empty Aura (`arm.spell.empty_aura`) | Yes | Remove at most min(current,5*grade) from the exact configured reserve; no caster credit or negative balance. | Native contract/effect adapter |
| 132 | Linked Threshold (`arm.spell.linked_threshold`) | Yes | One two-endpoint native portal for 120*grade seconds; lawful location/plane/ward gates and safe occupied closure. | Native contract/effect adapter |
| 133 | Bar the Elements (`arm.spell.bar_the_elements`) | Yes | One configured native school/tag match for 60*grade seconds, ordinary opposition; no all-effects reflection. | Native contract/effect adapter |
| 134 | Turn the Elements (`arm.spell.turn_the_elements`) | Yes | 30*grade seconds; reflect at most one eligible native attack; lineage guard prevents loops. | Native contract/effect adapter |
| 135 | Folded Pocket (`arm.spell.folded_pocket`) | Yes | Bearer-access finite pocket, 2*grade kilograms full-content weight, 300*grade seconds; reject direct/hidden nesting, living cargo and cycles; foreign goods conserved on occupied collapse. | `arm.item.folded_pocket` |
| 136 | Veil of Elements (`arm.spell.veil_of_elements`) | Yes | Reduce illumination by 5*grade lux for 60*grade seconds; perceptual scene, no physical barrier. | Native contract/effect adapter |
| 137 | Cross the Veil (`arm.spell.cross_the_veil`) | Yes | One explicitly configured plane overlay for 60*grade seconds; no unrestricted plane or ward bypass. | Native contract/effect adapter |
| 138 | Borrow the Dead (`arm.spell.borrow_the_dead`) | Yes | One eligible corpse-linked presence for 60*grade seconds; restore exact identity/custody on closure. | Native contract/effect adapter |
| 139 | Open Threshold (`arm.spell.open_threshold`) | Yes | One two-endpoint native portal for 120*grade seconds; lawful location/plane/ward gates and safe occupied closure. | Native contract/effect adapter |
| 140 | Read Enchantment (`arm.spell.read_enchantment`) | Yes | One recipient overlay for 60*grade seconds; explicitly permitted information, no privileged debugging data. | Native contract/effect adapter |
| 141 | Blade Barrier (`arm.spell.blade_barrier`) | Yes | 60*grade seconds; traversal inflicts 3*grade configured native damage after admitted crossing; no arbitrary blocked-room links. | Native contract/effect adapter |
| 142 | Mind Scour (`arm.spell.mind_scour`) | Yes | 5*grade native damage, 2*grade pain, 0 stun; one target; no implicit splash. | Native contract/effect adapter |
| 143 | Phantasm (`arm.spell.phantasm`) | Yes | One noninteractive perceptible scene for 60*grade seconds; no collision, equipment, identity or damage rights. | Native contract/effect adapter |
| 144 | Unbodied Journey (`arm.spell.unbodied_journey`) | Yes | One canonical-identity secondary presence for 60*grade seconds; explicit anchor/focus/range and no material manipulation. | Native contract/effect adapter |
| 145 | Erode Object (`arm.spell.erode_object`) | Yes | 5*grade native condition damage to an eligible material/object; no recursive contents destruction. | Native contract/effect adapter |
| 146 | Leeching Edge (`arm.spell.leeching_edge`) | Yes | For 60*grade seconds, heal <=min(0.1*grade,0.5)*actual eligible weapon harm; no missed-hit credit. | Native contract/effect adapter |
| 147 | Voice of Remains (`arm.spell.voice_of_remains`) | Yes | One corpse-bound speech proxy for 60*grade seconds; only recorded permitted knowledge; no resurrection or account access. | Native contract/effect adapter |
| 148 | Echo Servant (`arm.spell.echo_servant`) | Yes | One speech relay for 60*grade seconds; no command dispatch and no recursive casting trigger. | Native contract/effect adapter |
| 149 | Drowning Grip (`arm.spell.drowning_grip`) | Yes | Resistible interruption of native breathing for at most 5*grade seconds; native hypoxia, no generic damage substitute. | Native contract/effect adapter |
| 150 | Sow Sickness (`arm.spell.sow_sickness`) | Yes | One package-native infection at declared vector for 60*grade seconds; exact owned source and removal, no automatic contagion multiplier. | `arm.effect.disease` |
| 151 | Caustic Spray (`arm.spell.caustic_spray`) | Yes | 3*grade native damage per distinct eligible admitted target; declared per-spell inclusion policy, one progression opportunity. | Native contract/effect adapter |
| 152 | Gather Puddle (`arm.spell.gather_puddle`) | Yes | 0.25*grade litres clamped to native capacity; ordinary fluid custody; source-informed Draw Water/Wine override this. | `arm.liquid.water`, `arm.item.drink_container` |
| 153 | Wardcraft (`arm.spell.wardcraft`) | Optional | One configured native school/tag match for 60*grade seconds, ordinary opposition; no all-effects reflection. | Native contract/effect adapter |
| 154 | Renew Earth (`arm.spell.renew_earth`) | Optional | Optional land treatment, total budget 2*grade, rate 0.1*grade, lifetime 120*grade seconds; native capacity/accounting limits. | Native contract/effect adapter |

## Design validation and limits

The [structural receipt](Armageddon_Seeder_Completeness_Validation_20261011.json) records exact inventory, source-row preservation, capability counts, Simple admission uniqueness/threshold reachability/source-parent ordering, prototype dependency resolution, optional isolation and relative-link checks. Input source hashes use UTF-8 without BOM and LF line endings so Git transport preserves their meaning; all other text is retained. No runtime build, database install, natural progression, combat, dose calibration, historical elemental learn-tree resolution or N24 gameplay pass is claimed by these checks.

Relevant implementation contracts: [casting design](Armageddon_Casting_Design.md), [completion scope/progress](Armageddon_Completion_Progress.md), [current continuation](Armageddon_Continuation_20261008.md), [partial installer guide](Armageddon_Partial_Installer_Builder_Guide.md), and the current source factories recorded per entry. The full installer guide must replace the present prepared-world prototype/binding burden when the implementation is delivered.
