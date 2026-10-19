# Armageddon physical profiles: owner-review proposal, 19 October 2026

**Status: proposed values only; no profile activation authorised by this document.** The seven-profile roster, five cultures, two-city backgrounds and native-language policy are already approved. This proposal addresses the remaining physical/balance gate. Every numerical value below is an authored FutureMUD adaptation unless explicitly identified as recovered evidence. It is not a reconstruction of missing source race data.

## Recovered constraints

| Profile | Recovered physical constraint and relative ability |
| --- | --- |
| Human | Approximately 62–78in (157.48–198.12cm); unmodified Strength/Endurance/Agility/Wisdom; broad appearance variation. |
| City Elf | Approximately 75–96in (190.50–243.84cm); slender/light frame; weaker/less sturdy, more agile and somewhat wiser than Human. |
| Desert Elf | Same biological Elf ancestry and stature; long-distance running lifestyle; the same relative elven attribute descriptions. |
| Dwarf | Approximately 50–58in (127.00–147.32cm); short, broad, muscular, hairless; stronger/more enduring, less agile/wise than Human. |
| Half-Elf | Approximately 70–82in (177.80–208.28cm); strength/endurance between Human and Elf; moderated elven agility/wisdom. |
| Half-Giant | Approximately 125–155in (317.50–393.70cm); exceptional strength/endurance, low agility/wisdom; ordinary smaller humanoid clothing generally does not fit. |
| Mul | Slightly shorter than Human, compact/muscular, hairless, pointed ears, exceptional strength/ruggedness; no recovered numerical height band. |

These are approximate helpfile bands, not gender-specific hard limits or probability distributions. Ages, weight distributions, native ordinal sizes, physiological multipliers and reliable numerical racial stats were not recovered. `boot_races` loads absent `data_files/races`; the separate printed roster contains contradictory dice/ranges (including Human and Desert Elf Wisdom). Dwarf resistance prose establishes no approved numerical immunity. Do not import Dark Sun longevity or stat tables to fill these gaps.

## Proposed native attribute profile

Use the existing Attribute Seeder **`arm`** option: `Strength/str`, `Endurance/end`, `Agility/agi`, `Wisdom/wis`. Source Strength maps to native Strength; Endurance to native Endurance (punishment/fitness); Agility to native Agility (speed/flexibility); Wisdom to native Wisdom (learning capacity). This is an explicit semantic choice, not a mapping of Wisdom onto intelligence, perception or willpower from another attribute set. Other attribute sets require a separately reviewed crosswalk before these adjustments are applied.

For all seven, propose `DiceExpression = 3d6+1`, native raw `IndividualAttributeCap = 20`, raw `AttributeTotalCap = 48` for the four primary attributes. Use race `AttributeBonus` for the following adjustments; do not add bonuses into rolled values as well. The native attribute-ordering screen balances the raw sum to 48, so this is a common creation budget, not a maximum expected random sum. No individual per-attribute cap field exists on `RacesAttributes`.

| Profile | STR | END | AGI | WIS | Bonus sum | Proposed effective ceilings (raw 20 + each racial bonus) |
| --- | ---: | ---: | ---: | ---: | ---: | --- |
| Human | 0 | 0 | 0 | 0 | 0 | 20 / 20 / 20 / 20 |
| City Elf | −2 | −2 | +3 | +1 | 0 | 18 / 18 / 23 / 21 |
| Desert Elf | −2 | −1 | +3 | +1 | +1 | 18 / 19 / 23 / 21 |
| Dwarf | +2 | +3 | −2 | −3 | 0 | 22 / 23 / 18 / 17 |
| Half-Elf | −1 | −1 | +1 | +1 | 0 | 19 / 19 / 21 / 21 |
| Half-Giant | +10 | +5 | −4 | −5 | +6 | 30 / 25 / 16 / 15 |
| Mul | +4 | +4 | −1 | 0 | +7 | 24 / 24 / 19 / 20 |

These are deliberately compressed native adjustments, not copied source dice. The extra Desert Elf Endurance is a modest lifestyle adaptation; running still consumes stamina. Mul agility −1 is an authored bulk tradeoff, not recovered lore. Half-Giant/Mul retain a deliberate advantage behind the approved application gate; the sum is disclosed rather than disguised as equal balance. Validate raw/effective stat display and health/stamina/lifting consumers with one representative of each adjusted shape before enabling; do not claim the ceilings are independently enforced fields or later advancement limits.

## Proposed age and height/weight defaults

For **every profile**, propose explicit inheritance of bootstrap age thresholds `Child 3 / Youth 10 / YoungAdult 16 / Adult 25 / Elder 50 / Venerable 75` years. This is a conservative common native lifecycle pending owner review, not evidence that all species age identically. Set the initial playable creation window to 16–74 years if the selected age screen supports that bound; no child characters or mechanical immortality are implied. Any different lifespan needs an explicit revised matrix, not generic fantasy assumptions.

Propose one height/weight model per physical profile, shared by Male, Female, Neuter and NonBinary model references wherever that gender is supported by the installation. This explicitly proposes **no numerical dimorphism**, because none was recovered; it does not expand the configured selectable genders. Every supported gender must have a non-null link. All distributions are symmetric (`skewness = 0`), height in cm and BMI-like frame index in kg/m²; `Bmimultiplier = 0.1` yields native grams from cm. The frame index is a generation parameter, not a human medical classification.

| Profile | Height mean ± SD; proposed bounds (cm) | Frame index mean ± SD; proposed bounds | Weight at both means (kg) | Implied bounded weight envelope (kg) |
| --- | --- | --- | ---: | --- |
| Human | 177.80 ± 6.77; 157.48–198.12 | 23 ± 3; 17–32 | 72.7 | 42.2–125.6 |
| City Elf | 217.17 ± 8.89; 190.50–243.84 | 18.5 ± 2; 14–25 | 87.2 | 50.8–148.6 |
| Desert Elf | Same as City Elf | Same as City Elf | 87.2 | 50.8–148.6 |
| Dwarf | 137.16 ± 3.39; 127.00–147.32 | 36 ± 4; 28–48 | 67.7 | 45.2–104.2 |
| Half-Elf | 193.04 ± 5.08; 177.80–208.28 | 22 ± 2.5; 17–30 | 82.0 | 53.7–130.1 |
| Half-Giant | 355.60 ± 12.70; 317.50–393.70 | 50 ± 7; 35–70 | 632.3 | 352.8–1085.0 |
| Mul | 170.18 ± 5.93; 152.40–187.96 | 34 ± 3.5; 25–45 | 98.5 | 58.1–159.0 |

All means/SDs/frame indices/weight envelopes and Mul's 60–74in band are authored. The other proposed height bounds operationalise approximate recovered bands; that hard-bound choice itself needs owner acceptance. Weight is generated from the chosen height and frame index, not an independent normal distribution: `kg = (cm / 100)^2 × index`. Keep `MeanWeight/StddevWeight` null so random generation and the native standard-deviation selection path use the same model. Half-Giant's larger index expresses a much heavier frame than a scaled slender Human, without inventing a new anatomy.

**Implementation gate:** native `HeightWeightModel` stores means/SDs but no min/max fields and unbounded random normals can escape these envelopes. The selected chargen height/weight path must enforce the reviewed bounds (including manual entries), or the owner must explicitly accept soft bounds instead. Do not claim a seeded model alone enforces the table. Native internal weights are grams; convert kg when constructing verification assertions.

## Proposed size, body, health and physiology

| Profile | Standing / sitting / prone (`SizeCategory`) | Bodypart size modifier / health multiplier | Physiology package |
| --- | --- | --- | --- |
| Human | Normal 6 / Small 5 / VerySmall 4 | 0 / 1.0 | H |
| City Elf | Large 7 / Normal 6 / VerySmall 4 | 0 / 1.0 | H |
| Desert Elf | Large 7 / Normal 6 / VerySmall 4 | 0 / 1.0 | H; Endurance adjustment above only |
| Dwarf | Small 5 / VerySmall 4 / Tiny 3 | −1 / 1.0 | H |
| Half-Elf | Normal 6 / Small 5 / VerySmall 4 | 0 / 1.0 | H |
| Half-Giant | VeryLarge 8 / Large 7 / Normal 6 | +2 / 1.0 | G |
| Mul | Normal 6 / Small 5 / VerySmall 4 | 0 / 1.0 | H |

All ordinal assignments and posture reductions are authored. A long prone Elf shares Human vertical coverage without becoming biologically shorter. Bodypart size is a separate deliberate setting: Half-Giant +2/Dwarf −1 affects part and equipment relationships, while character posture controls immersion. Size categories are ordinal, not metre conversions. Sample wading/silt coverage, prone airways, exit fit and representative torso/hand/foot equipment; a single Human-sized armour prototype need not fit every profile. No new armour corpus or automatic resizing is implied.

**Shared body/health foundation:** reuse native Organic Humanoid anatomy, blood/blood-model, sweat liquid, corpse model, health strategy and natural-armour foundation. Retain existing healthy humanoid organs/limbs and weapon/defence support. Propose `BodypartHealthMultiplier = 1.0` and `PainToleranceMultiplier = 1.0` for every profile: Endurance already changes health-related inputs; do not silently multiply health again. Preserve the selected native health-attribute bindings (the `arm` physical traits) and test actual health totals. Common unavailable Elf parent shares the same biological body; City/Desert children supply the distinct reviewed mechanical profiles. No separate elven species, extra limbs, racial armour bonus or poison/magic immunity.

**H = explicit Human-baseline inheritance:** breathe ordinary Air at multiplier 1; `NeedsToBreathe = true`, native lung breathing; breathing-volume expression `7`; hold-breath expression `90 + 5 × Endurance` using its actual trait ID (bootstrap's local formula variable name `con` does not change that binding). Temperature range 0–40°C; hunger/thirst rates 1.0; existing Human satiation limits from the 12h food / 6h drink cadence helper; stamina enabled; sweat 0.8 native litres/minute. Lift `Strength × 10000` grams; drag `Strength × 40000` grams. Visual/olfactory track intensity 1/1 and tracking ability 1/0. Retain native swimming/climbing and sleep-position defaults. These exact inheritance choices establish no desert heat immunity, thirst immunity, endurance exemption or terrain bypass; the existing sweat calibration is inherited explicitly, not claimed as recovered biological realism.

**G = authored large-frame differences from H:** hunger/thirst rates 3.0/3.0; sweat 2.4 litres/minute; breathing-volume expression `28`; lift `Strength × 20000` grams and drag `Strength × 80000` grams; visual/olfactory track intensity 1.5/1.5. All other H values, including satiation ceilings, temperature band, breath-hold duration, stamina use and tracking ability, remain explicit inheritance. These are provisional balance adaptations, not mass-proportional physiology. Review actual needs cadence and stamina/encumbrance with a roughly 632kg character; check that increased intake is feasible with native satiation/consumption. Do not advertise Half-Giant readiness until that sample passes.

## Availability and acceptance gate

Keep every newly incomplete physical profile unavailable while installing its owned records. This document does not alter existing builder-owned Human data or activate an existing world. Human/City Elf/Dwarf/Half-Elf may become selectable only after owner acceptance of their values, valid gender/model/body/attribute bindings and a focused chargen/gameplay sample. Desert Elf/Half-Giant/Mul additionally require the already-approved explicit application/approval policy binding; default unavailable until bound. The common Elf parent always remains unavailable. Do not invent karma costs, fertility/slavery/crime state, clan membership, magic-class bans or powers.

Approval should cover the four-stat adjustments/budget, common age inheritance, gender model sharing and distribution/bounds choice, posture/equipment consequences, and H/G physiological settings. Record accepted revisions against this proposal before enabling. Qualification should sample: ordinary Human; slender Elf and smaller Dwarf for fit/coverage; Half-Giant for fit/needs/lifting; Mul for health/stamina; one Desert Elf run for finite stamina. Static coverage verifies all seven references/genders without an exhaustive race × gender × class test matrix. Installation, gameplay qualification and owner approval remain distinct ledger states.

No roster/culture/language/naming decision is reopened. Preserve `SETTING_NATIVE_LANGUAGE_PLAN_20261018.tsv` native 200 and explicit secondary 100/60 claims, and the approved culture compatibility matrix. Retain free-entry personal names; install no random-name corpus or new script through this proposal.

## Sources and native contracts inspected

- Planning directory: `C:\Users\luker\Documents\Codex\2026-10-13\task`. `EXECUTION_PHYSICAL_HERITAGE_20261019.md`; `ARMAGEDDON_HERITAGE_PROPOSAL_20261018.md`; `SETTING_HERITAGE_PROPOSAL_20261018.tsv`; `SETTING_HERITAGE_DECISIONS_20261018.tsv`; `SETTING_NATIVE_LANGUAGE_PLAN_20261018.tsv`.
- Recovered help bodies: `heritage_recovered_20261018/research_cache/{humans,city_elves,desert_elves,dwarves,half-elves,half-giants,muls}.txt`, approximately lines 241–248. Verified-cache hashes and source-era limits: `HERITAGE_RECOVERY_PROVENANCE_20261018.json`. These support stature/descriptive directions, not authored numerical tables above.
- `HERITAGE_RAW_SOURCE_EXCERPTS_20261018.txt`: raw `codedump.c` lines 35591–35616 (external race-file loader), 138644–138679 (historical menu), 79181–79248 (language grants). Raw SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`; no claim of current live rules.
- Native models: [Race database fields](../../MudsharpDatabaseLibrary/Models/Race.cs), [racial attributes](../../MudsharpDatabaseLibrary/Models/RacesAttributes.cs), [height/weight fields](../../MudsharpDatabaseLibrary/Models/HeightWeightModel.cs), [runtime race inheritance](../../MudSharpCore/Character/Heritage/Race.cs), [runtime height/weight generation](../../MudSharpCore/NPC/Templates/HeightWeightModel.cs).
- Native seed/selection contracts: [Attribute Seeder](../../DatabaseSeeder/Seeders/AttributeSeeder/AttributeSeeder.cs) `arm` at ~1650; [Human Seeder](../../DatabaseSeeder/Seeders/HumanSeeder/HumanSeeder.cs) models at ~1031 and race foundations at ~1809–2029; [attribute ordering](../../MudSharpCore/CharacterCreation/Screens/AttributeOrdererScreen.cs) ~201–280; [size enum](../../FutureMUDLibrary/GameItems/SizeCategory.cs); [water/silt posture and coverage](../World/Very_Shallow_Water_and_Silt.md).

Documentation-only proposal. No code, database, availability or world changes; no gameplay qualification claimed.
