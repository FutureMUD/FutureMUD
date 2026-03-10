# FutureMUD Health System: Medical Interactions

## Scope
This document covers the player-facing and builder-facing interaction surfaces around health:

- health commands in `HealthModule`
- treatment items and treatment types
- wound care flow
- surgery and surgical procedures
- CPR and defibrillation
- drugs, dosing, metabolism, and drug-driven effects

## Command Surface
The `HealthModule` command set is the main interaction layer for live medical play.

| Command group | Current commands | Main purpose |
| --- | --- | --- |
| Inspection and assessment | `vitals`, `health`, `wounds`, `wound`, `triage` | Read visible health state, pulse and breathing, or deeper wound information |
| Bedside wound care | `bind`, `cleanwounds`, `suture`, `tend`, `relocate`, `dislodge`, `repair` | Stabilize bleeding, clean or close wounds, tend healing, relocate bones, remove lodged objects, repair robot-like injuries |
| Surgery and implants | `surgery`, `installimplant`, `powerimplant`, `connectimplants`, `implants` | Run formal procedures and manage implant systems |
| Rescue medicine | `cpr`, `defibrillate` | Attempt to restore breathing or circulation when a patient is non-responsive |
| Administrative or force tools | `infect`, `cure`, `sever`, `unsever`, `exsanguinate` | Directly manipulate health state for testing, administration, or special gameplay flows |

## Treatments
### Core model
Treatments are not hard-coded to individual items. Instead, items expose treatment behavior through `ITreatment` and `TreatmentGameItemComponent`.

At runtime, a treatment item answers three questions:

- does it support a given `TreatmentType`
- how does it modify the base difficulty
- how many uses remain

That allows the same wound logic to work with many item definitions, while builders can vary quality and available treatment modes through component prototypes.

### Treatment matrix
| Treatment type | Supported wound families | Typical player surface | Typical check or item support | Notes |
| --- | --- | --- | --- | --- |
| `Trauma` | `SimpleOrganicWound`, `RobotWound` | `bind`, trauma-oriented procedures | Treatment-capable bandages, tourniquets, trauma kits, `TraumaControlCheck` in stock skill data | Controls bleeding or fluid leakage |
| `Clean` | `SimpleOrganicWound` | `cleanwounds` | Cleaning-capable treatment items, `CleanWoundCheck` in stock skill data | Usually blocked while the wound is still bleeding |
| `Antiseptic` | `SimpleOrganicWound` | `cleanwounds` or antiseptic item use | Antiseptic-capable treatment items | Adds antiseptic protection after cleaning succeeds |
| `Close` | `SimpleOrganicWound`, `RobotWound` | `suture`, some surgery phases | Suture items, sealing tools, `SutureWoundCheck` for organic closure | Requires prior stabilization |
| `Tend` | `SimpleOrganicWound` | `tend` | `TendWoundCheck`, tending effects, treatment items that support tending | Improves later healing rather than acting as immediate closure |
| `Remove` | `SimpleOrganicWound`, `RobotWound` | `dislodge` | `RemoveLodgedObjectCheck` | Required before several other treatment types become valid |
| `Relocation` | `BoneFracture` | `relocate` | `RelocateBoneCheck`, `RelocatingBone` effect | Handles relocation of displaced fractures; command file still has a dislocation TODO |
| `Set` | `BoneFracture` | Item-driven immobilization | Splints, casts, or other immobilizing items | Implemented as item support; `BoneFracture.Treat` explicitly rejects direct `Set` calls |
| `SurgicalSet` | `BoneFracture` | Surgery | `SurgicalSetCheck`, surgical procedure support | Reinforces or surgically sets fractures |
| `Repair` | `RobotWound`, item-health paths | `repair` | Repair tools and repair-capable items | Mechanical analogue to organic closure and healing |
| `Mend` | Organic, fracture, robot, and other wound paths where supported | Usually magical or special recovery effects | Healing checks and non-mundane effects | Represents supernatural or otherwise exceptional recovery |
| `AntiInflammatory` | Surface exists, but not implemented for normal organic play | None of the stock command flows | Enum slot only | `SimpleOrganicWound` currently reports this as not implemented |

### Treatment sequencing
The system intentionally enforces medical order of operations.

Common organic sequence:

1. Stop bleeding with `Trauma`.
2. Remove foreign objects if needed.
3. Clean or antiseptically treat the wound.
4. Close the wound if appropriate.
5. Tend the wound to improve healing quality.

Common fracture sequence:

1. Relocate the fracture.
2. Immobilize it with `Set`, or surgically reinforce it with `SurgicalSet`.
3. Allow healing ticks to progress.

That sequencing is one of the main gameplay-design choices in the subsystem.

## Surgical Procedures
### Runtime model
Surgery is modeled as first-class content through `ISurgicalProcedure` and concrete procedure classes.

Each procedure can define:

- a procedure type
- medical knowledge requirements
- a check type
- emotes and descriptive text
- multiple timed phases
- inventory plans for required tools or held items
- progs for usability, completion, abort, and special effects

This makes surgery much more data-driven than ordinary wound treatment.

### Procedure families in current runtime
| Family | Concrete classes | Role |
| --- | --- | --- |
| Diagnosis | `TriageProcedure`, `MedicalExaminationProcedure` | Reveal hidden wound and stability information |
| Wound access and closure | `ExploratorySurgery`, `InvasiveProcedureFinalisation`, `TraumaControlProcedure` | Open, inspect, control trauma, and finalize invasive work |
| Limb surgery | `AmputationProcedure`, `ReplantationProcedure` | Remove or reattach limbs and major bodyparts |
| Bone surgery | `SurgicalSettingProcedure` | Surgically set or reinforce fractures |
| Vascular access | `CannulationProcedure`, `DecannulationProcedure` | Add or remove cannula-based access |
| Organ surgery | `OrganExtractionProcedure`, `OrganStabilisationProcedure`, `OrganTransplantProcedure` | Remove, stabilize, or replace organs |
| Implant surgery | `InstallImplantProcedure`, `RemoveImplantProcedure`, `ConfigureImplantPowerProcedure`, `ConfigureImplantInterfaceProcedure` | Install and configure implant systems |

### Seeder relationship
Stock surgery content is authored in `HealthSeeder`, including knowledges, procedures, and procedure phases. However, `HealthSeeder.Enabled` is currently `false`, so the runtime framework is broader than the stock data most worlds receive by default.

The seeded surgery matrix is detailed in [Seeder_and_Gaps.md](./Seeder_and_Gaps.md).

### Verified partial areas
Current surgery-related observations that matter for design:

- the framework is broad and clearly intended for continued expansion
- stock surgery data is much more complete than stock drug data
- `SurgicalProcedureFactory` has a default `NotImplementedException` path for unsupported procedure types
- `ConfigureImplantInterfaceProcedure` currently reports `ConfigureImplantPower` as its `Procedure` enum value, which looks like an implementation inconsistency rather than an intentional design distinction

## CPR and Defibrillation
### CPR
`cpr` is an action-based rescue mechanic, not a one-shot command.

At runtime:

- CPR is gated by the `CPRAllowed` static setting
- the rescuer gains a `PerformingCPR` effect
- the patient gains `CPRTarget`
- CPR consumes stamina and repeats on a timed heartbeat
- successful CPR can add `StablisedOrganFunction`, temporarily restoring enough function for survival

### Defibrillation
`defibrillate` requires a held item that exposes `IDefibrillator`.

The defibrillator component checks for conditions such as:

- available power
- appropriate bodily access, including chest exposure
- whether heart function is in a state where defibrillation can help

This is not a generic revive mechanic. It is a device-based attempt to restore a particular physiological failure mode.

## Drugs
### Core model
Drugs are editable runtime objects via `IDrug` and `Drug`.

Each drug defines:

- allowed delivery vectors through `DrugVector`
- intensity per gram
- relative metabolization rate
- one or more `DrugType` effect families
- extra payload data for drug types that need it, such as magic capabilities, organ types, or neutralized drugs

### Delivery and metabolism
The body distinguishes:

- latent dosages, which are still entering the system
- active dosages, which are already affecting the body

Different vectors move from latent to active at different rates:

- injected doses are fast
- inhaled doses are also fast
- ingested and touched doses are slower

On each drug heartbeat, the body:

1. processes latent doses into active ones
2. sums direct drug effects and drug-like effects from other sources
3. applies resistance merits and neutralization effects
4. creates or updates persistent effects such as analgesia, pacifism, thermal imbalance, or organ-function modifiers
5. metabolizes active drugs down over time

### Drug effect coverage
| Drug effect family | Current runtime state |
| --- | --- |
| `Anesthesia` | Implemented in body drug processing |
| `Analgesic` | Implemented in body drug processing |
| `Immunosuppressive` | Implemented in body drug processing |
| `NeutraliseDrugEffect` | Implemented in preprocessing logic |
| `NeutraliseSpecificDrug` | Implemented in preprocessing logic |
| `BodypartDamage` | Implemented |
| `Pacifism` | Implemented |
| `Rage` | Implemented |
| `StaminaRegen` | Implemented |
| `Nausea` | Implemented |
| `HealingRate` | Implemented |
| `MagicAbility` | Implemented |
| `Antibiotic` | Implemented |
| `OrganFunction` | Implemented |
| `VisionImpairment` | Implemented |
| `ThermalImbalance` | Implemented |
| `Adrenaline` | Enum exists, no matching handling found in current runtime search |
| `Paralysis` | Enum exists, no matching handling found in current runtime search |

### Builder-facing drug editing
`Drug.BuildingCommand` exposes a rich editing surface:

- rename the drug
- change intensity and metabolism
- toggle delivery vectors
- set effect intensities
- configure bodypart damage targets
- configure magic capability grants
- configure drug neutralization targets
- configure specific-drug neutralization targets
- configure healing-rate modifiers
- configure organ-function targets

This is a mature builder-facing surface even though the stock seeding currently uses only two generic drugs.

## Verified Current Behavior Summary
Medical interaction in FutureMUD is a layered system:

- commands choose targets and invoke checks or timed effects
- wound classes decide whether a treatment is valid
- treatment items modify difficulty and availability
- surgeries provide a heavier, knowledge-driven content framework
- rescue mechanics rely on body state and equipment rather than on universal revive commands
- drugs are ongoing system modifiers, not only consumable buffs

## Seeded Defaults Summary
The current stock experience includes:

- bedside commands in `HealthModule`
- treatment-capable low-tech medical items from `ItemSeeder` and `UsefulSeeder`
- stock skills and checks for common treatment and rescue flows
- a disabled but extensive stock surgery seeder
- only two seeded drugs in the dedicated health seeder

## Gaps and Extension Pressure
The strongest current extension pressure points are:

- anti-inflammatory treatment support
- more stock drugs that use the already broad runtime drug framework
- enabling or reworking stock surgery seeding
- more complete stock support for implant and defibrillator workflows
