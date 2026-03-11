# FutureMUD Health System: Seeder State and Gaps

## Scope
This document explains two things:

1. How the current stock repo seeds health-relevant data.
2. Which runtime options are broader than the current stock setup, partially implemented, or obvious extension points.

This document is intentionally split into verified current-state observations and inferred extension candidates.

## Seeder Map
The health system is not seeded from only one place.

| Seeder | Verified health contribution | Current-state note |
| --- | --- | --- |
| `HealthSeeder` | Seeds medical knowledges, surgical procedures and phases, a broad tech-level drug catalogue, and optional mammal veterinary procedures | Enabled as a release-ready stock medical seeder |
| `HumanSeeder` | Seeds human health strategies, corpse models, blood models, population blood models, race defaults, chargen needs settings, and breathing-related race flags | This is where a large amount of practical health setup currently lives |
| `AnimalSeeder` | Seeds animal corpse models, health strategies, blood models, population blood models, race defaults, and multiple breathing model assignments | Also carries a large amount of effective health setup |
| `CoreDataSeeder` | Seeds the stock `GameItem` health strategy and some related static strings such as death messaging | Important for item damage support |
| `ItemSeeder` | Seeds named medical and health-adjacent items such as bandages, splints, tourniquets, suturing tools, prosthetics, and cannula items | Low-tech medical play is well represented here |
| `UsefulSeeder` | Seeds many medical component prototypes, including treatment items, cannula definitions, IV support, and prosthetic components | More component-heavy than `ItemSeeder` |
| `SkillSeeder` | Seeds the skills behind many health-related checks | Medical action quality depends on this seeding |
| `SkillPackageSeeder` | Seeds the package-level distribution of medical and rescue checks | Helps determine who can actually use the health systems well |

## Dedicated Health Seeder State
### Verified current state
`HealthSeeder` is currently enabled.

That matters because it means:

- the repo automatically offers a stock surgery framework for the selected medical tech level
- the stock repo now ships a broader default drug catalogue instead of only two example drugs
- the seeder can optionally install basic veterinary content for stock mammal bodies alongside the human set
- core race and item health defaults are still assembled across multiple seeders rather than only here

## Tech-Level Surgery Matrix
The enabled `HealthSeeder` provides the clearest picture of the stock surgical content model.

| Tech level | Seeded knowledges | Seeded procedures |
| --- | --- | --- |
| Primitive | `Medicine` plus optional `Animal Medicine` | Human: `Hasty Triage`, `Crude Physical`, `Primitive Stitching`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Trauma Control`, `Organ Extraction`, `Crude Organ Repair`, `Trepanation`, `Windpipe Repair`, `Bone Setting`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Crude Physical`, `Veterinary Stitching`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |
| Pre-modern | `Chiurgery`, `Physical Medicine` plus optional `Veterinary Medicine` and `Veterinary Chiurgery` | Human: `Hasty Triage`, `Triage`, `Crude Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Trauma Control`, `Organ Extraction`, `General Organ Repair`, `Trepanation`, `Cardiac Repair`, `Liver Resection`, `Splenic Repair`, `Gastric Repair`, `Intestinal Resection`, `Kidney Repair`, `Lung Resection`, `Tracheal Repair`, `Esophageal Repair`, `Spinal Stabilisation`, `Inner Ear Repair`, `Bone Setting`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Triage`, `Veterinary Physical`, `Veterinary Stitch Up`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |
| Modern | `Diagnostic Medicine`, `Clinical Medicine`, `Surgery` plus optional `Veterinary Medicine` and `Veterinary Surgery` | Human: `Hasty Triage`, `Triage`, `Crude Physical`, `Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Digit Amputation`, `Replantation`, `Cannulation`, `Decannulation`, `Trauma Control`, `Organ Extraction`, `Organ Transplant`, `General Organ Repair`, `Brain Surgery`, `Cardiac Repair`, `Liver Resection`, `Splenic Repair`, `Gastric Repair`, `Intestinal Resection`, `Kidney Repair`, `Lung Resection`, `Tracheal Repair`, `Esophageal Repair`, `Spinal Stabilisation`, `Inner Ear Repair`, `Bone Setting`, `Install Implant`, `Remove Implant`, `Configure Implant Power`, `Configure Implant Interface`. Veterinary: `Veterinary Hasty Triage`, `Veterinary Triage`, `Veterinary Physical`, `Veterinary Stitch Up`, `Veterinary Exploratory Surgery`, `Veterinary Trauma Control`, `Veterinary Bone Setting`, `Foreleg Amputation`, `Hindleg Amputation` |

### Seeder intent
The prompt text in `HealthSeeder` makes the intended capability progression explicit:

- primitive: no replantation, transplantation, resection, or implants
- pre-modern: no replantation, transplantation, or implants
- modern: full procedure set

## Seeded Medical Items and Drugs
### Dedicated health seeder
`HealthSeeder` now seeds a tech-level specific drug catalogue rather than only two examples.

Primitive examples include:

- `Willow Bark Tea`
- `Mandrake Draught`
- `Honey Poultice`
- `Garlic Salve`
- `Mint Infusion`
- `Ephedra Brew`
- `Foxglove Tincture`

Pre-modern examples include:

- `Laudanum`
- `Ether Anaesthetic`
- `Mould Poultice`
- `Distilled Antiseptic`
- `Mint and Ginger Tonic`
- `Digitalis Tincture`
- `Curare Paste`
- `Herbal Burn Salve`
- `Bronchial Smoke`

Modern examples include:

- `General Anaesthetic`
- `Opioid Analgesic`
- `Muscle Relaxant`
- `Local Anaesthetic`
- `Broad-Spectrum Antibiotic`
- `Antifungal Course`
- `Antiemetic`
- `Immunosuppressant`
- `Adrenaline Shot`
- `Bronchodilator`
- `Cardiac Support Agent`
- `Healing Accelerant`
- `Antipyretic`
- `Overdose Antagonist`

These stock drugs now exercise a much larger share of the runtime drug framework, including analgesia, anesthesia, antibiotics, antifungals, nausea control, paralysis, adrenaline, stamina support, organ support, healing-rate modifiers, immunosuppression, and drug neutralisation.

This is much broader than the old stock seeding, but it is still narrower than the full runtime drug framework.

### Item seeders
The item seeders cover a broad low-tech treatment layer.

Representative seeded content includes:

- bandages
- splints
- crutches
- tourniquets
- prosthetics such as peg legs, hook hands, replacement arms, legs, feet, eyes, and hands
- suturing kits and needles
- forceps and related minor surgical tools
- cannula items

`UsefulSeeder` also seeds treatment component prototypes and support content such as:

- `Bandage_Simple`, `Bandage_Good`, `Bandage_Great`
- cannula component prototypes
- IV-related component content
- a large prosthetic component set

The current `HealthSeeder` inventory plans are intentionally limited to tags that are already seeded for stock play and are cross-checked before installation:

- `Scalpel`
- `Bonesaw`
- `Forceps`
- `Arterial Clamp`
- `Surgical Suture Needle`

Diagnostic and physical-examination procedures are therefore intentionally tool-light in stock seeding rather than depending on a larger clinical kit that may not exist in base content.

### Skills and checks
The stock seeding for skills and packages includes many health-relevant checks, including:

- `SutureWoundCheck`
- `CleanWoundCheck`
- `MedicalExaminationCheck`
- `TriageCheck`
- `TraumaControlCheck`
- `PerformCPR`
- `Defibrillate`
- `OrganExtractionCheck`
- `OrganTransplantCheck`
- `CannulationProcedure`
- `DecannulationProcedure`
- `OrganStabilisationCheck`
- `TendWoundCheck`
- `RelocateBoneCheck`
- `SurgicalSetCheck`

That means the repo already treats medical play as skill-gated, even before world builders customize it.

## Runtime Option Matrix
The following table separates runtime capability from stock seeding.

| Runtime option | Stock seeding status | Note |
| --- | --- | --- |
| `BrainHitpointsStrategy` | Seeded | Used in human and animal seeders |
| `ComplexLivingHealthStrategy` | Seeded | Used in human and animal seeders |
| `GameItemHealthStrategy` | Seeded | Seeded in `CoreDataSeeder` |
| `SimpleLivingHealthStrategy` | Not stock seeded | Runtime class exists |
| `ConstructHealthStrategy` | Not stock seeded | Runtime class exists |
| `BrainConstructHealthStrategy` | Not stock seeded | Runtime class exists |
| `RobotHealthStrategy` | Not stock seeded | Runtime class exists |
| `Simple` infection | Runtime implemented | Reached through organic wound logic rather than a named stock seeding block |
| `Gangrene` infection | Runtime implemented | Reached through infection progression rather than dedicated stock content |
| `Necrotic`, `Infectious`, `FungalGrowth` infection types | Runtime implemented | Concrete infection classes now exist, but stock seeding still does little to showcase them |
| `StandardCorpseModel` | Seeded | Human and animal seeders create standard corpse models |
| `NonDecayingCorpseModel` | Not broadly stock seeded | Runtime support exists |
| `NoNeeds`, `Passive`, `Active` needs models | Runtime supported | `NoNeeds` is the most explicit stock default in current seeding paths |
| `Lung`, `Gill`, `Blowhole`, `NonBreather`, `Partless` breathing strategies | Runtime supported | Animal seeding demonstrates multiple breathing assignments |
| Broad drug effect families | Mostly unseeded | Runtime drug system is much richer than stock seeded drugs |
| Defibrillator items | Not found in broad stock item seeding search | Runtime support exists through `IDefibrillator` |
| Rebreathers and breathing filters | Not found in broad stock item seeding search | Runtime support exists through component types |
| External organs and advanced implants | Not found in broad stock item seeding search | Runtime support exists through component types and surgery classes |

## Verified Partial or Inactive Areas
The following findings are directly verified in code and matter when describing the current state of the system.

### Runtime broader than implementation
- `DrugType` still exposes more effect families than the stock seeded drugs use, but the gap is much smaller than before.
- Multiple health strategy families exist in runtime without matching stock seeded strategy definitions.
- Seeded strategy XML still only emits the older minimal field set; newer optional strategy tuning properties now load with fallback defaults when omitted.

### Recently resolved runtime gaps
- `LungBreather` now integrates stop-breathing effects and non-rebreather `IProvideGasForBreathing` sources.
- `Infection` now persists virulence multiplier data instead of dropping it on load or save.
- `SimpleInfection` now applies a concrete nausea effect.
- `HealthModule.relocate` now resolves fractures from targeted external bodyparts rather than only direct bone targets.
- The old soft-coded tending helper path has been removed in favor of the effect-driven wound-care flow.
- The player-facing `surgery show` path now exposes richer procedure detail.
- `SimpleOrganicWound` and `BoneFracture` both support anti-inflammatory treatment through bodypart-scoped pain-reduction effects.
- `ConfigureImplantInterfaceProcedure` now reports the correct procedure enum.
- `SurgicalProcedure` now enforces `TargetBodyType` when checking whether a procedure can be started, so human and quadruped surgical content can coexist safely.

## Supported but Unseeded
These are verified runtime features that exist in the codebase but are not broadly reflected in the stock seeding.

- advanced health strategy families for constructs, robots, and simpler living models
- expanded strategy tuning XML for health thresholds, bleed handling, hypoxia, blood recovery, kidney waste, and spleen cleanup
- non-decaying corpse models
- high-tech medical items such as defibrillators, rebreathers, breathing filters, external organs, and complex implants
- some remaining drug effect families such as explicit rage or magic-oriented stock examples
- implant installation, removal, and configuration surgery content without matching broad stock implant item content

## Logical Extension Candidates
The following are inference-based extension paths. They are not claims about current behavior; they are the most natural next steps suggested by the current design.

- extend the stock drug catalogue into more niche effect families such as rage, vision control, and magic-facing substances where a game setting wants them
- add broad stock content for defibrillators, rebreathers, external organs, and implant power or interface ecosystems
- formalize flaw-style health templates if builders want negative baseline traits distinct from merits

## Main Takeaways
The current stock repo has a rich health runtime but a split and uneven stock setup.

The most important practical consequences are:

- a lot of effective health setup lives outside `HealthSeeder`
- the stock repo now includes a release-ready medical seeder with tech-level surgery, drugs, and basic mammal veterinary support
- low-tech treatment and prosthetic play are well represented in seeded items
- higher-tech medical play is mostly a runtime capability awaiting fuller stock content
