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
| `HealthSeeder` | Seeds medical knowledges, surgical procedures and phases, and a very small drug set | Present but disabled with `Enabled => false` |
| `HumanSeeder` | Seeds human health strategies, corpse models, blood models, population blood models, race defaults, chargen needs settings, and breathing-related race flags | This is where a large amount of practical health setup currently lives |
| `AnimalSeeder` | Seeds animal corpse models, health strategies, blood models, population blood models, race defaults, and multiple breathing model assignments | Also carries a large amount of effective health setup |
| `CoreDataSeeder` | Seeds the stock `GameItem` health strategy and some related static strings such as death messaging | Important for item damage support |
| `ItemSeeder` | Seeds named medical and health-adjacent items such as bandages, splints, tourniquets, suturing tools, prosthetics, and cannula items | Low-tech medical play is well represented here |
| `UsefulSeeder` | Seeds many medical component prototypes, including treatment items, cannula definitions, IV support, and prosthetic components | More component-heavy than `ItemSeeder` |
| `SkillSeeder` | Seeds the skills behind many health-related checks | Medical action quality depends on this seeding |
| `SkillPackageSeeder` | Seeds the package-level distribution of medical and rescue checks | Helps determine who can actually use the health systems well |

## Dedicated Health Seeder State
### Verified current state
`HealthSeeder` currently has `Enabled => false`.

That matters because it means:

- the repo contains a substantial stock surgery framework that is not automatically installed
- the repo contains only a minimal stock drug set in that same disabled seeder
- core race and item health defaults are currently assembled elsewhere

## Tech-Level Surgery Matrix
The disabled `HealthSeeder` still provides the clearest picture of the stock surgical content model.

| Tech level | Seeded knowledges | Seeded procedures |
| --- | --- | --- |
| Primitive | `Medicine` | `Hasty Triage`, `Crude Physical`, `Primitive Stitching`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Trauma Control`, `Organ Extraction`, `Bone Setting` |
| Pre-modern | `Chiurgery`, `Physical Medicine` | `Hasty Triage`, `Triage`, `Crude Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Trauma Control`, `Organ Extraction`, `Intestinal Resection`, `Bone Setting` |
| Modern | `Diagnostic Medicine`, `Clinical Medicine`, `Surgery` | `Hasty Triage`, `Triage`, `Crude Physical`, `Physical`, `Stitch Up`, `Exploratory Surgery`, `Arm Amputation`, `Leg Amputation`, `Replantation`, `Cannulation`, `Decannulation`, `Trauma Control`, `Organ Extraction`, `Organ Transplant`, `Intestinal Resection`, `Liver Resection`, `Lung Resection`, `Bone Setting`, `Install Implant`, `Remove Implant`, `Configure Implant Power`, `Configure Implant Interface` |

### Seeder intent
The prompt text in `HealthSeeder` makes the intended capability progression explicit:

- primitive: no replantation, transplantation, resection, or implants
- pre-modern: no replantation, transplantation, or implants
- modern: full procedure set

## Seeded Medical Items and Drugs
### Dedicated health seeder
`HealthSeeder` seeds only two explicit drugs:

- `General Anaesthetic`
- `Basic Analgesic`

This is much narrower than the drug runtime supports.

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
- `Suture_Kit`, `Suture_Kit_Good`, `Suture_Single`
- cannula component prototypes
- IV-related component content
- a large prosthetic component set

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
| `Necrotic`, `Infectious`, `FungalGrowth` infection types | Not implemented as concrete runtime classes in current inventory | Enum surface is broader than implementation |
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

### Disabled or inactive stock setup
- `HealthSeeder` is disabled.

### Runtime broader than implementation
- `InfectionType` exposes `Necrotic`, `Infectious`, and `FungalGrowth`, but the current infection class inventory contains only `Infection`, `SimpleInfection`, and `Gangrene`.
- `DrugType` exposes more effect families than the stock seeded drugs use.
- `DrugType.Adrenaline` and `DrugType.Paralysis` were not found in the current runtime drug-effect handling search.
- Multiple health strategy families exist in runtime without matching stock seeded strategy definitions.

### Explicit TODO or partial markers
- `LungBreather` has TODOs around effect integration and breath sources other than rebreathers.
- `Infection` contains a TODO around loading virulence.
- `SimpleInfection` contains TODOs about applying concrete nausea effects.
- `HealthModule` contains a TODO for dislocation handling in `relocate`.
- `HealthModule` contains a TODO about soft-coded tending duration.
- `HealthModule` contains a TODO noting that one health-reporting path needs a better player version.
- `SimpleOrganicWound` reports `AntiInflammatory` treatment as not implemented.

### Implementation inconsistencies that extension work should note
- `ConfigureImplantInterfaceProcedure` currently reports `SurgicalProcedureType.ConfigureImplantPower` for its `Procedure` property. That looks inconsistent with the surrounding factory and check wiring.

## Supported but Unseeded
These are verified runtime features that exist in the codebase but are not broadly reflected in the stock seeding.

- advanced health strategy families for constructs, robots, and simpler living models
- non-decaying corpse models
- high-tech medical items such as defibrillators, rebreathers, breathing filters, external organs, and complex implants
- a broad drug system with many effect families but only two stock seeded example drugs
- implant installation, removal, and configuration surgery content without matching broad stock implant item content

## Logical Extension Candidates
The following are inference-based extension paths. They are not claims about current behavior; they are the most natural next steps suggested by the current design.

- enable or split the current `HealthSeeder` so surgery and drugs can be installed independently of race seeders
- add stock drugs for the already-supported effect families such as antibiotics, organ support, thermal imbalance, and magic capability
- add concrete infection classes for the unused infection enum values
- add broad stock content for defibrillators, rebreathers, external organs, and implant power or interface ecosystems
- expand breathing-effect integration so environmental and status effects can participate more explicitly
- formalize flaw-style health templates if builders want negative baseline traits distinct from merits

## Main Takeaways
The current stock repo has a rich health runtime but a split and uneven stock setup.

The most important practical consequences are:

- a lot of effective health setup lives outside `HealthSeeder`
- surgery is stocked more thoroughly than drugs, but the surgery seeder is disabled
- low-tech treatment and prosthetic play are well represented in seeded items
- higher-tech medical play is mostly a runtime capability awaiting fuller stock content
