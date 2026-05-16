# Antiquity Medical Crafting Suite

## Scope

The antiquity medical suite adds low-tech health goods that use the existing treatment, drug delivery, prosthetic, and mobility runtime components. It covers wound dressings, cleaning and tending kits, sutures, surgical tools, herbal remedies, fumigation preparations, documentable medicine containers, crutches, canes, and simple prosthetics.

The suite is culture-neutral in visible craft names and echoes. Historical specificity lives in item descriptions, materials, knowledge metadata, and the selected remedy substances rather than in player-facing craft names.

## Seeded Items

Treatment supplies:

- Linen bandage rolls, honeyed dressings, vinegar wound cloths, yarrow styptic pads, wool compresses, splints, arm slings, tending kits, wound-cleaning kits, antiseptic kits, sutures, and suturing kits

Surgical tools:

- Bone suture needles, bronze probes, scalpels, forceps, arterial clamps, cautery irons, and bone saws

Herbal remedies:

- Willow bark tea packets, poppy latex draughts, mandrake draughts, ephedra brew packets, foxglove tinctures, mint infusions, honey poultices, garlic salves, aloe burn salves, henbane fumigation cones, and mandrake smoke cones

Mobility and prosthetics:

- Padded willow crutches, walking canes, peg legs, prosthetic feet, carved hands, bronze hook hands, and painted clay eyes

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

## Health Seeder Tie-Ins

`HealthSeeder` now includes additional primitive remedies for the antiquity suite:

- `Aloe Burn Salve`
- `Poppy Latex Draught`
- `Henbane Smoke`
- `Yarrow Styptic`

The drug delivery component seeding now covers inhaled drugs with `Smokeable_*` component prototypes, alongside the existing `Pill_*` and `TopicalCream_*` components.

## Forage Tie-Ins

Stock terrain forage profiles now expose medicinal yield types such as `medicinal-herbs`, `willow-bark`, `foxglove-leaves`, `mandrake-root`, `aloe-leaves`, `ephedra-stems`, and `aromatic-resins`. These are yield pools for builders and future foragable definitions; they do not force concrete herb objects into every terrain.
