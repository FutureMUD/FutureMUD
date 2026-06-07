# Antiquity Medical Crafting Suite

## Scope

The antiquity medical suite adds low-tech health goods that use the existing treatment, drug delivery, prosthetic, and mobility runtime components. It covers wound dressings, cleaning and tending kits, sutures, surgical tools, herbal remedies, fumigation preparations, documentable medicine containers, crutches, canes, and simple prosthetics.

The suite is culture-neutral in visible craft names and echoes. Historical specificity lives in item descriptions, materials, knowledge metadata, and the selected remedy substances rather than in player-facing craft names.

## Seeded Items

Treatment supplies:

- Linen bandage rolls, honeyed dressings, vinegar wound cloths, yarrow styptic pads, wool compresses, splints, arm slings, tending kits, wound-cleaning kits, antiseptic kits, sutures, and suturing kits

Surgical tools:

- Bone suture needles, bronze probes, scalpels, forceps, arterial clamps, cautery irons, and bone saws

Preparation tools:

- Linen medicine strainers
- Stone mortar and pestle sets

Herbal remedies:

- Willow bark tea packets, poppy latex draughts, mandrake draughts, ephedra brew packets, foxglove tinctures, mint infusions, honey poultices, garlic salves, aloe burn salves, henbane fumigation cones, and mandrake smoke cones

Mobility and prosthetics:

- Padded willow crutches, walking canes, peg legs, prosthetic feet, carved hands, bronze hook hands, and painted clay eyes

Source-audited final product catalogue:

| Stable Reference | Source Catalogue Name |
| --- | --- |
| `antiquity_aloe_burn_salve_pot` | an aloe burn salve pot |
| `antiquity_bone_suture_needle` | a polished bone suture needle |
| `antiquity_bronze_arterial_clamp` | a bronze arterial clamp |
| `antiquity_bronze_bone_saw` | a bronze surgical bone saw |
| `antiquity_bronze_cautery_iron` | a bronze cautery iron |
| `antiquity_bronze_forceps` | a pair of bronze forceps |
| `antiquity_bronze_hook_hand` | a bronze hook hand |
| `antiquity_bronze_scalpel` | a bronze surgical scalpel |
| `antiquity_bronze_surgical_probe` | a bronze surgical probe |
| `antiquity_carved_prosthetic_foot` | a carved wooden prosthetic foot |
| `antiquity_carved_prosthetic_hand` | a carved wooden prosthetic hand |
| `antiquity_ephedra_brew_packets` | some ephedra brew packets |
| `antiquity_foxglove_tincture_vial` | a foxglove tincture vial |
| `antiquity_garlic_salve_pot` | a garlic salve pot |
| `antiquity_gut_suture_spool` | a spool of plain gut suture |
| `antiquity_henbane_fumigation_cone` | a henbane fumigation cone |
| `antiquity_herbalist_pouch` | a labelled herbalist's pouch |
| `antiquity_honey_antiseptic_kit` | a honey and vinegar antiseptic kit |
| `antiquity_honey_poultice_pot` | a small honey poultice pot |
| `antiquity_honeyed_linen_dressing` | a honeyed linen wound dressing |
| `antiquity_linen_arm_sling` | a linen arm sling |
| `antiquity_linen_bandage_roll` | a roll of clean linen bandage |
| `antiquity_linen_tending_kit` | a linen-wrapped tending kit |
| `antiquity_mandrake_draught_vial` | a mandrake draught vial |
| `antiquity_mandrake_smoke_cone` | a mandrake smoke cone |
| `antiquity_mint_infusion_bundle` | a mint infusion bundle |
| `antiquity_padded_wooden_peg_leg` | a padded wooden peg leg |
| `antiquity_painted_clay_eye` | a painted clay prosthetic eye |
| `antiquity_poppy_latex_draught` | a stoppered poppy latex draught |
| `antiquity_simple_walking_cane` | a simple walking cane |
| `antiquity_surgical_tool_roll` | a leather surgical tool roll |
| `antiquity_suturing_kit` | a compact suturing kit |
| `antiquity_vinegar_wound_cloth` | a vinegar-soaked wound cloth |
| `antiquity_willow_bark_packets` | some willow bark tea packets |
| `antiquity_willow_crutch` | a padded willow crutch |
| `antiquity_wooden_splint_pair` | a pair of padded wooden splints |
| `antiquity_wool_compress` | a packed wool wound compress |
| `antiquity_wound_cleaning_kit` | a wound cleaning kit |
| `antiquity_yarrow_styptic_pad` | a yarrow styptic pad |

## Crafting Chains

Commodity crafts create upstream medical stock before final item crafts:

- `Dressing Stock`
- `Poultice Stock`
- `Salve Stock`
- `Decoction Stock`
- `Fumigation Stock`
- `Suture Stock`
- `Splint Stock`
- `Prosthetic Stock`
- `Surgical Tool Blank`
- `Herbal Remedy Stock`

Final crafts are registered from `SeedAntiquityMedicalCrafts()` and target the actual antiquity item prototypes by stable reference. The suite uses the current knowledge-gated `AddCraft` overload so the deterministic appear/can-use/why-cannot-use progs are upserted through the shared craft helper.

Surgical-tool stock now requires the lit smelting-furnace tool state rather than a generic hot-fire tag. The medical stock recipes that boil, warm, or steep ingredients require both a cooking pot and an in-room `Fire` tool, so they use the same lit-hearth convention as the rest of the low-heat craft suite. Herbal remedy stock uses seeded medicine strainers and mortar-and-pestle tools as current prerequisites. Crafts to make those newly introduced preparation tools are the explicit second-pass boundary unless they become current product targets.

## Health Seeder Tie-Ins

`HealthSeeder` now includes additional primitive remedies for the antiquity suite:

- `Aloe Burn Salve`
- `Poppy Latex Draught`
- `Henbane Smoke`
- `Yarrow Styptic`

The drug delivery component seeding now covers inhaled drugs with `Smokeable_*` component prototypes, alongside the existing `Pill_*` and `TopicalCream_*` components.

## Forage Tie-Ins

Stock terrain forage profiles now expose medicinal yield types such as `medicinal-herbs`, `willow-bark`, `foxglove-leaves`, `mandrake-root`, `aloe-leaves`, `ephedra-stems`, and `aromatic-resins`. These are yield pools for builders and future foragable definitions; they do not force concrete herb objects into every terrain.
