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
| Item-mediated drug delivery | `apply`, `inject` | Use held items such as creams and syringes to deliver drugs through targeted bodypart interactions |
| Surgery and implants | `surgery` | Run formal procedures and manage implant systems |
| Rescue medicine | `cpr`, `defibrillate` | Attempt to restore breathing or circulation when a patient is non-responsive |
| Administrative or force tools | `infect`, `cure`, `sever`, `unsever`, `exsanguinate`, `installimplant`, `powerimplant`, `connectimplants`, `implants` | Directly manipulate health state for testing, administration, or special gameplay flows |

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
| `Tend` | `SimpleOrganicWound` | `tend` | `TendWoundCheck`, tending effects, treatment items that support tending | Improves later healing rather than acting as immediate closure; the timed wound-care flow can also use anti-inflammatory treatment items when the wound qualifies |
| `Remove` | `SimpleOrganicWound`, `RobotWound` | `dislodge` | `RemoveLodgedObjectCheck` | Required before several other treatment types become valid |
| `Relocation` | `BoneFracture` | `relocate` | `RelocateBoneCheck`, `RelocatingBone` effect | Handles relocation of displaced fractures, including when the operator targets an external bodypart that contains the broken bone |
| `Set` | `BoneFracture` | Item-driven immobilization | Splints, casts, or other immobilizing items | Implemented as item support; `BoneFracture.Treat` explicitly rejects direct `Set` calls |
| `SurgicalSet` | `BoneFracture` | Surgery | `SurgicalSetCheck`, surgical procedure support | Reinforces or surgically sets fractures |
| `Repair` | `RobotWound`, item-health paths | `repair` | Repair tools and repair-capable items | Mechanical analogue to organic closure and healing |
| `Mend` | Organic, fracture, robot, and other wound paths where supported | Usually magical or special recovery effects | Healing checks and non-mundane effects | Represents supernatural or otherwise exceptional recovery |
| `AntiInflammatory` | `SimpleOrganicWound`, `BoneFracture` | `tend` when suitable treatment items are available | Bodypart-scoped `AntiInflammatoryTreatment` effect, anti-inflammatory-capable treatment items, currently resolved with `CleanWoundCheck` quality logic in the wound-care flow | Reduces pain and swelling without closing damage, and persists as a timed saving effect on the treated bodypart |

### Treatment sequencing
The system intentionally enforces medical order of operations.

Common organic sequence:

1. Stop bleeding with `Trauma`.
2. Remove foreign objects if needed.
3. Clean or antiseptically treat the wound.
4. Close the wound if appropriate.
5. Tend the wound to improve healing quality and, when appropriate, apply anti-inflammatory relief to reduce lingering pain.

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
Stock surgery content is authored in `HealthSeeder`, including knowledges, procedures, procedure phases, and tech-level variants. The stock seeder is now enabled and installs a release-ready surgery set for the selected tech level, with optional mammal veterinary procedures when the stock quadruped body exists.

The seeded surgery matrix is detailed in [Seeder_and_Gaps.md](./Seeder_and_Gaps.md).

### Verified partial areas
Current surgery-related observations that matter for design:

- the framework is broad and clearly intended for continued expansion
- stock surgery data now covers a much broader release-ready baseline, including primitive, pre-modern, modern, and basic veterinary tracks
- stock inventory plans for surgical phases are intentionally limited to guaranteed seeded tool tags such as scalpels, bonesaws, forceps, arterial clamps, and surgical suture needles
- procedures now enforce target body prototypes at runtime, which is what allows human and quadruped stock procedures to coexist without accidental cross-body use
- `SurgicalProcedureFactory` has a default `NotImplementedException` path for unsupported procedure types
- `ConfigureImplantInterfaceProcedure` now reports the correct `ConfigureImplantInterface` procedure type
- the player-facing `surgery show` path now exposes richer procedure, requirement, and phase detail rather than relying on a staff-only style summary

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

### Topical cream items
`TopicalCreamGameItemComponent` is the stock cream-style item component for bodypart application.

- It is used through the generic `apply <item> <target> <bodypart> [<amount>]` command rather than through `HealthModule`.
- Each cream prototype stores a total quantity plus one or more drug payload definitions, each with grams-per-gram and absorption fraction values.
- Every configured drug must support the `Touched` vector, and successful application doses the target body through that vector.
- Cream prototypes can optionally specify an `OnApplyProg` futureprog. When present, the runtime executes it with the treated character, the applied bodypart description text, and the actual amount of cream applied.
- Prototype persistence lives in the component definition XML, including total quantity, configured drugs, and the optional `OnApplyProg` id. Runtime item state still persists the remaining cream quantity per-instance.

On each drug heartbeat, the body:

1. processes latent doses into active ones
2. sums direct drug effects and drug-like effects from other sources
3. applies resistance merits and neutralization effects
4. creates or updates persistent effects such as analgesia, pacifism, thermal imbalance, or organ-function modifiers
5. metabolizes active drugs down over time

Drug-driven thermal load now feeds the same generic `ThermalImbalance` state that environmental exposure uses, so its symptoms and lethal path are shared with ordinary hypothermia and hyperthermia. If `TemperatureImbalanceEnabled` is disabled in static configs, those drug contributions are skipped entirely.

Adrenaline and paralysis are transient overlays derived from active dosage totals rather than long-term saved treatment state. By contrast, anti-inflammatory bedside care is a saving effect bound to a bodypart and survives persistence until it expires.

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
| `Antifungal` | Implemented |
| `OrganFunction` | Implemented |
| `VisionImpairment` | Implemented |
| `ThermalImbalance` | Implemented |
| `Adrenaline` | Implemented through adrenaline rush, heart-support, thermal, and cardiac-stress effects |
| `Paralysis` | Implemented through a drug-driven forced-paralysis effect |

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

This is a mature builder-facing surface, and the stock seeding now exercises it with a wider catalogue of herbal remedies, pre-modern compounds, and modern pharmaceuticals.

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
- an enabled `HealthSeeder` that installs tech-level surgery content, including optional mammal veterinary procedures
- a broader stock drug catalogue covering pain control, anesthesia, infection control, healing support, organ support, nausea control, paralysis, adrenaline, and overdose reversal

## Gaps and Extension Pressure
The strongest current extension pressure points are:

- more stock drugs that use the remaining runtime drug framework, including concrete stock examples for rage, magic-facing, and more niche specialist effects
- more complete stock support for implant and defibrillator workflows
