# FutureMUD Health System: Medical Interactions

## Scope
This document covers the player-facing and builder-facing interaction surfaces around health:

- health commands in `HealthModule`
- treatment items and treatment types
- wound care flow
- surgery and surgical procedures
- CPR and defibrillation
- drugs, dosing, metabolism, and drug-driven effects
- economy-mediated hospital service requests

## Command Surface
The `HealthModule` command set is the main interaction layer for live medical play.

| Command group | Current commands | Main purpose |
| --- | --- | --- |
| Inspection and assessment | `vitals`, `health`, `wounds`, `wound`, `triage` | Read visible health state, pulse and breathing, or deeper wound information |
| Bedside wound care | `bind`, `cleanwounds`, `suture`, `tend`, `relocate`, `dislodge`, `repair` | Stabilize bleeding, clean or close wounds, tend healing, relocate bones, remove lodged objects, repair robot-like injuries |
| Item-mediated drug delivery | `apply`, `inject` | Use held items such as creams and syringes to deliver drugs through targeted bodypart interactions |
| Surgery and implants | `surgery` | Run formal procedures, robot maintenance procedures, and implant systems |
| Economy-mediated service requests | `hospital` | Procure treatment through hospital employment tasks and paid medical-service requests |
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
| `Repair` | `RobotWound`, item-health paths | `repair` | Repair tools and repair-capable items | Mechanical analogue to organic closure and healing; this is the bedside layer, not the full robot surgery replacement path |
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

Common robot sequence:

1. Stabilize fluid leakage with `Trauma` or `repair`, depending on what the wound currently allows.
2. Remove lodged debris if needed.
3. Use bedside `repair` for ordinary mechanical damage that does not require opened chassis access.
4. Escalate to robot surgery when the job requires exploratory access, chassis closure, limb detachment or reattachment, or organ extraction and replacement.

That sequencing is one of the main gameplay-design choices in the subsystem.

The delayed `bind`, `suture`, `tend`, and item-using `cleanwounds` flows execute their inventory plan when the action begins and again before each delayed wound step. If the needed treatment item is already held or wielded the per-step execution is a no-op; if a consumable was used up, the next step can pick up a replacement before treating the next wound. Health status recalculation evaluates wounds, updates bodypart and organ damage effects, and then rechecks consciousness and other character state so newly critical organ function is reflected immediately.

### Economy-mediated hospital requests
The `hospital` command lives on the economy command surface, but it deliberately reuses the health runtime instead of defining a separate medical model. Hospital services can queue employment tasks for binding, wound cleaning, wound closing, tending, bone relocation, bone setting, configured surgical procedures, configured implant procedures, surgical-family procedure services, blood donation, blood transfusion, and automatically maintained stabilisation and full-treatment requests.

Hospital services have an offering mode. Standalone-capable services appear in player service lists and can be requested directly. Combined-only services stay hidden from ordinary patient lists but remain visible to hospital managers and can be used as component services for stabilisation or full-treatment planning and usage billing. Standalone-and-combined services do both. Explicit surgical-family services mirror `SurgicalProcedureType` families such as triage, detailed examination, exploratory surgery, trauma control, organ stabilisation/extraction/transplant, amputation, replantation, surgical bone setting, cannulation, decannulation, invasive finalisation, implant procedures, and prosthetic fitting. These service rows do not need one hand-authored service per organ or bodypart. If no specific procedure is configured, the hospital runner resolves a matching procedure for the patient's body prototype and the doctor's ability, then uses request-specific `hospital request <service> params <target>` arguments or automatic bodypart/organ defaults where possible.

When a hospital worker executes a queued `hospitalservice` task, bedside services use the same treatment types and command routes described above. Theatre-preferring services make the medical worker path to the patient first; stabilisation and full-treatment services also require an operating theatre even if older service data does not have the preference flag set. When the worker reaches the patient, the hospital flow escorts both of them to the reserved theatre with source-room, destination-room, and direct patient echoes and then starts treatment. Post-treatment recovery or waiting-room routing also echoes in both the origin and destination rooms. Non-theatre services that intentionally treat in place emit an in-room emote before treatment begins. Stabilisation and full-treatment services advance through staged `bind`, `suture`, `cleanwounds`, `tend`, and `relocate` command phases rather than directly mutating wounds in one employment tick. Those commands keep their ordinary delays, blockers, item plans, room echoes, and treatment restrictions. If useful treatment stock exists and the reserved theatre does not already contain the needed staged treatment supplies, the hospital can prepare a bundled treatment kit through the supply step before the doctor starts the medical step; later hospital ticks inspect carried bundle contents, delivered bundle contents, and theatre-staged treatment supplies as available supplies rather than sending the worker back to fetch the same treatment items again. Manager theatre-staging goals can pre-deliver reusable tools, explicit service equipment, and implicit wound-care treatment supplies from supply rooms into operating theatres, so urgent services can bypass the service-specific supply step when those items are already present. Treatment stock lying loose in a supply room is not considered usable by the medical step until it is collected, so an assigned worker who has reached the supply room should collect the supplies before returning to the patient. Supply and service execution fail the request cleanly if the patient dies, enters stasis, leaves configured hospital locations, leaves the reserved theatre after preparation, or becomes unseen by the assigned worker while colocated. Cleaning and tending phases run once per request phase, so repeatable antiseptic, cleaning, and anti-inflammatory possibilities do not cause an endless loop over the same wound. If a combined request has no valid visible treatment, it fails with that reason. Completed stabilisation, full-treatment, surgical, and other multi-step hospital procedures emit a completion echo before post-treatment routing. Surgery and implant services call the configured `ISurgicalProcedure`; surgery can administer a configured injectable anesthesia drug at a calculated intensity before starting procedures that need an unconscious patient. If a service has an anesthesia cannulation procedure, cannulation runs first as a staged surgical prep, then the anesthetic is primed from a prepared IV liquid container through a drip into the installed cannula before the main surgery begins. Implant services can create a configured implant item, pass it as a procedure argument, and then continue through configured implant power and interface follow-up procedures. Staged surgery requests remain open until `SurgicalProcedureEffect` completes or aborts, so hospital completion, failure, and recovery routing reflect the actual procedure outcome rather than the task-start moment.

Hospital surgery now prepares its anatomical target before starting the surgical effect. Automatic selection ignores bodyparts forbidden by the selected procedure and prefers an already exposed valid site. Routine blood donation and transfusion choose only peripheral limb sites unless the request explicitly supplies `target <bodypart>`. When a bodypart-specific procedure has an `exposed` phase requirement, the worker removes only the worn items necessary to expose that site, using the same covering and removal rules as `strip`. Removed patient belongings are retained against the hospital request, bundled with the standard temporary pile item when multiple pieces were removed, and returned to the patient's recovery or waiting room after completion or abandonment. If a staged or immediate theatre procedure then aborts or terminally fails, the patient receives the recorded failure reason and is moved to recovery, or to the waiting room when no recovery room is configured, rather than being left in the operating theatre.

Blood donation and transfusion services use the ordinary cannula, drip, IV bag, blood, and liquid-container runtime. Donation is a staged IV workflow: the worker validates donor safety and prepared gear, starts or reuses cannulation, connects an IV-capable container through a drip to the cannula, switches the container to drain mode, and waits for the IV bag heartbeat to physically collect blood or for the donor safe minimum/container capacity to stop the draw. Cannulation and decannulation callbacks for blood services remain in that IV workflow even when the same cannulation procedure is also configured for anesthesia preparation on the service. Donation availability and supply selection only require a drain-capable IV blood container with some compatible spare capacity, not one container large enough for the full configured donation target, so stock 250ml bags can collect a partial 0.5L target draw and finish at bag capacity. Blood donation never charges the donor. The service price is the default flat payout after successful collection; a matching blood-stock policy instead uses its target-aware per-litre price. The hospital preflights reserve or linked-bank funds, then neutralises and disconnects the gear, decannulates when it inserted the line, records the actual collected volume, and pays at the end. If funds become transiently unavailable after collection, the request remains in a retryable payout phase and does not complete unpaid. Transfusion is also staged. It chooses exact recipient blood type first, then compatible non-matching donor blood using `IBloodtype.IsCompatibleWithDonorBlood`, can consume multiple IV containers sequentially, rejects incompatible donor blood, and caps restoration at the recipient's total blood volume. Standalone transfusion fails without enough compatible IV blood. Combined stabilisation and full-treatment may instead drip prepared liquids whose injection consequence is `BloodVolume` when compatible blood is not available in sufficient volume.

Hospital requests can target a visible patient brought to the hospital. Conscious third-party patients are asked to accept treatment, while unconscious or helpless patients are presumed to consent for emergency care. Once a request is queued, the assigned hospital worker receives a request-scoped treatment permission so the ordinary wound-care commands can run without opening a second accept prompt for each delayed command phase. Payment, prepaid credit, usage billing, cancellation, and medical debt payoff are handled by the economy-side hospital records; the resulting clinical action still flows through the ordinary health system. The requester or patient can use `hospital cancel [<#>]` for an active request, while hospital managers can cancel any active request. Cancellation stops the linked employment task and aborts any staged hospital surgery effect, but it does not refund amounts already paid or charged for completed or partially completed work. Services that require recovery prefer routing patients to a recovery room after completion, using the waiting room only as a fallback when no recovery room is configured; helpless patients are placed on a suitable recovery bed where possible.

Managers and proprietors use `hospital operations` as the live clinical status view for this flow. It ties room roles, theatre reservations, active service requests, assigned employment tasks, current task steps, patient locations, blocker diagnostics, and task-recorded resource reservations into a single hospital command output. Long operational views such as services, requests, ledger entries, and operations are presented as multi-line record blocks so clinical text, patient names, resource reservations, and diagnostics do not depend on very wide terminal tables.

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
- an opt-in or opt-out bodypart restriction list
- phase-level requirements such as an exposed target bodypart

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
| Prosthetic fitting | `InstallProstheticProcedure` | Skill-gated fitting of supported prosthetic items to compatible visible severed bodyparts |

### Seeder relationship
Stock surgery content is now authored in two places:

- `HealthSeeder` installs the organic human and veterinary catalogue, including knowledges, procedures, procedure phases, and tech-level variants
- `RobotSeeder` installs robot knowledges and the stock robot maintenance suite for articulated, quadruped, insectoid, and utility robot bodies

The seeded surgery matrix is detailed in [Health Seeder State and Gaps](./Health_Seeder_State_and_Gaps.md).

### Robot maintenance suite
The stock robot procedures are intentionally parallel to the organic surgery model rather than a separate ad hoc repair minigame.

The current seeded robot suite covers:

- diagnostics
- maintenance examination
- exploratory maintenance
- leak control
- chassis closure
- robot organ extraction
- robot organ replacement
- limb detachment
- limb reattachment

These procedures target the robot base bodies installed by `RobotSeeder`. Because the runtime now honours `CountsAs` when matching procedure bodyparts and organs, wheel, track, and similar derived chassis variants can use the intended stock robot procedures without needing duplicate definitions per race.

### Bedside repair versus robot surgery
The stock repo now has a clearer split between robot bedside care and robot surgery:

- `repair` remains the ordinary bedside command for robot wounds and other repair-capable damage states
- robot surgery is for opened-chassis work, formal leak control, diagnostic access, organ swaps, and major assembly changes
- organic surgery definitions do not substitute for robot surgery, and robot procedures target robot-compatible body prototypes rather than the human or veterinary bodies

### Verified partial areas
Current surgery-related observations that matter for design:

- the framework is broad and clearly intended for continued expansion
- stock surgery data now covers a much broader release-ready baseline, including primitive, pre-modern, modern, and basic veterinary tracks
- stock inventory plans for surgical phases are intentionally limited to guaranteed seeded tool tags such as scalpels, bonesaws, forceps, arterial clamps, and surgical suture needles
- procedures now enforce target body prototypes at runtime, which is what allows human and quadruped stock procedures to coexist without accidental cross-body use
- `SurgicalProcedureFactory` has a default `NotImplementedException` path for unsupported procedure types
- `ConfigureImplantInterfaceProcedure` now reports the correct `ConfigureImplantInterface` procedure type
- the player-facing `surgery show` path now exposes richer procedure, requirement, and phase detail rather than relying on a staff-only style summary
- both player and administrator `surgery show` output state the effective bodypart restriction and whether any phase requires the target to be exposed

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
| `PlanarState` | Implemented through a drug-driven planar corporeality overlay |
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
- configure planar corporeality state and plane

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
- formal human prosthetic fitting procedures for the primitive, pre-modern, and modern medical tiers
- a broader stock drug catalogue covering pain control, anesthesia, infection control, healing support, organ support, nausea control, paralysis, adrenaline, and overdose reversal

## Gaps and Extension Pressure
The strongest current extension pressure points are:

- more stock drugs that use the remaining runtime drug framework, including concrete stock examples for rage, magic-facing, and more niche specialist effects
- more complete stock support for implant and defibrillator workflows
