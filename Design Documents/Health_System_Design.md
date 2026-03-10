# FutureMUD Health System Design

## Purpose
This document describes the current state of the FutureMUD health system as implemented in the repository on 2026-03-10. It is a runtime and gameplay reference, not a forward-looking redesign.

The intended audience is:

- Human readers who need a coherent explanation of how health works in play and how it is configured.
- Agentic readers who need a map from gameplay concepts to the interfaces, commands, item types, effects, and seeders that implement them.

## Reading Guide
This is the head document for the health system set:

- [Core_Runtime.md](./Core_Runtime.md) covers `IBody`, biology, health strategies, wounds, infections, and health-related merits.
- [Medical_Interactions.md](./Medical_Interactions.md) covers bedside treatment, surgery, CPR, defibrillation, drugs, and the main command surface.
- [Adjacencies_and_Items.md](./Adjacencies_and_Items.md) covers breathing, needs, corpses, severed bodyparts, implants, artificial organs, rebreathers, cannulae, prosthetics, and cross-cutting effects.
- [Seeder_and_Gaps.md](./Seeder_and_Gaps.md) covers how stock content is currently seeded and where the current implementation is partial, broader than the stock data, or a natural extension point.

## Core Mental Model
FutureMUD health is not a single subsystem. It is a set of cooperating systems centered on `IBody` and `IHaveWounds`.

At the highest level:

1. A race and body definition establish anatomy, organs, bones, blood, breathing, corpse behavior, and health defaults.
2. Damage enters through an `IHaveWounds` owner and is interpreted by an `IHealthStrategy`.
3. The body routes damage to bodyparts, bones, and organs, creating concrete `IWound` instances.
4. Wounds store local state such as damage, pain, stun, shock, bleeding, lodged objects, and treatment progress.
5. Organic wounds can acquire `IInfection` instances that advance over time and may spread or damage tissue.
6. Drugs, breathing, blood volume, needs, merits, and temporary effects continuously modify health outcomes.
7. Bedside treatment, surgery, CPR, defibrillation, and implants change wound state or restore system function.
8. When thresholds are crossed, the owner becomes paralyzed, unconscious, passes out, or dies.
9. Death creates corpse state and moves the character to a no-needs state until resurrection or cleanup.

This design makes health heavily simulation-oriented. The system cares about where damage landed, what bodypart or organ was affected, whether bleeding was controlled, whether the patient can breathe, what drugs are active, what items are available, and what medical knowledge and checks exist in the world.

## Subsystem Map
| Subsystem | Core runtime types | Player or admin surface | Stock seeded defaults |
| --- | --- | --- | --- |
| Bodies and anatomy | `IBody`, race bodypart and organ definitions, `Body`, `BodyBiology` | Character health, wound visibility, organ function, death, severing | Seeded mainly in `HumanSeeder` and `AnimalSeeder` |
| Health strategies | `IHealthStrategy`, strategy classes in `MudSharpCore/Health/Strategies` | Governs damage conversion, severity, prompts, healing, status | `BrainHitpoints`, `ComplexLiving`, and `GameItem` are stock seeded |
| Wounds | `IWound`, `SimpleOrganicWound`, `BoneFracture`, `RobotWound` | `wounds`, `wound`, treatment commands, healing ticks | Organic and fracture play are stock reachable; robot support exists in runtime |
| Infections | `IInfection`, `Infection`, `SimpleInfection`, `Gangrene` | Wound tags, infection progression, antibiotics, antiseptic handling | No broad content seeding beyond whatever wounds and treatments enable |
| Treatments | `ITreatment`, `TreatmentGameItemComponent`, `TreatmentType` | `bind`, `cleanwounds`, `suture`, `tend`, `relocate`, `dislodge`, `repair` | Bandages, splints, sutures, prosthetics, cannulae seeded in item seeders |
| Surgery | `ISurgicalProcedure`, procedure classes and phases | `surgery`, implant commands, medical knowledges and checks | Stock surgery data exists in `HealthSeeder`, but that seeder is disabled |
| Drugs | `IDrug`, `Drug`, `BodyDrugs`, drug effects | Drug dosing, metabolism, builder editing, medical items | Only two stock drugs in `HealthSeeder`: anaesthetic and analgesic |
| Breathing | `IBreathingStrategy`, breathing strategy classes | Suffocation, held breath, rebreathers, breathable fluids | Human and animal seeders assign breathing models |
| Needs | `INeedsModel`, needs model classes, need effects | Hunger, thirst, alcohol, water, no-needs cases | `NoNeeds` is explicitly seeded in several default paths |
| Corpses and severed parts | `ICorpse`, corpse model classes, corpse item component | Death aftermath, decay, severed bodypart persistence | Standard corpse models are seeded; non-decaying support exists in runtime |
| Health-adjacent items | Cannula, IV, rebreather, defibrillator, implant, prosthetic, external organ components | Emergency care, surgery, organ replacement, breathing support | Low-tech items are seeded broadly; higher-tech components are mostly runtime-only |
| Effects | Concrete effects in `MudSharpCore/Effects/Concrete` | Long-running actions, temporary modifiers, organ stabilization, drug overlays | Many are runtime glue rather than directly seeded content |

## End-to-End Flow
### 1. Baseline definition
Characters start from race data. That data chooses the body prototype, blood model, corpse model, breathing model, whether the race needs to breathe, and a default health strategy.

### 2. Damage intake
Damage reaches either a character body or an item implementing `IHaveWounds`. The chosen `IHealthStrategy` determines how much wound state is created and how severe it is.

### 3. Anatomical routing
For characters, `BodyBiology` further routes damage through armor, bodyparts, bones, organs, and severing logic. The result is not just hit point loss; it is a set of localized wound objects with different downstream consequences.

### 4. Ongoing health evaluation
Heartbeat-driven health ticks evaluate blood loss, organ performance, breathing, temperature, and the aggregate effects of wounds. Health status can escalate from normal to paralyzed, unconscious, passed out, or dead.

### 5. Secondary complications
Organic wounds can become dirty, infected, or gangrenous. Breathing failure causes its own damage path. Needs and drugs introduce slow-burn modifiers rather than single-event outcomes.

### 6. Medical intervention
Players and staff can inspect, stabilize, clean, close, tend, repair, relocate, surgically treat, or magically mend wounds. The system cares about sequence. For example, bleeding often must be controlled before cleaning or closure, and fractures must be relocated before they can be set.

### 7. Rescue and life support
When breathing or circulation fails, CPR and defibrillation try to temporarily restore function. Rebreathers, external organs, cannulae, IV support, and organ implants extend the system into equipment-supported survival.

### 8. Death and aftermath
If the body cannot sustain life, the character dies, a corpse may be created from the race's corpse model, the body is transitioned to dead state, and the character's needs model is forcibly switched to `NoNeeds`.

## Gameplay Design Perspective
The health system is designed as a simulation layer that supports roleplay rather than a fast abstract combat minigame.

Key gameplay consequences:

- Injury location matters. A damaged limb, organ, or bone is materially different from a generic hit.
- Stabilization matters. Stopping bleeding, removing lodged objects, cleaning wounds, and then closing them is a meaningful sequence.
- Time matters. Health, infection, drug, and needs systems all rely on heartbeat progression.
- Skill and knowledge matter. Treatment quality depends on checks, medical procedures, and seeded knowledges rather than only on item possession.
- Equipment matters. Medical play is partly about having the right item types and components, not just issuing commands.
- Death has persistence consequences. Corpses, severed parts, and no-needs dead characters make death part of the world state, not just a combat flag.

## Builder and Admin Perspective
The health system is partly data-driven and partly hard-coded.

Builders and administrators currently configure or consume health through:

- Race data and seeder-created anatomy, blood, breathing, and corpse definitions.
- Editable drugs and their vectors, intensities, and effect payloads.
- Surgical procedures, their phases, knowledges, checks, inventory plans, and progs.
- Treatment-capable item component prototypes and health-adjacent game item components.
- Static strings and flags such as death messages and whether CPR is enabled.
- Skills, checks, and knowledges seeded outside the immediate health runtime code.

One important current-state fact is that the dedicated `HealthSeeder` is disabled. In practice, stock health behavior is assembled across multiple seeders rather than coming from one fully active medical setup step.

## Important Current-State Constraints
These are central to understanding the current implementation:

- There is no separate flaw subsystem. Health-related "flaws" are currently represented by merits, negative merit configurations, selective applicability, or other gameplay data.
- Runtime support is broader than stock content. Several health strategies, corpse models, item systems, infection enum values, and drug enum values exist without matching stock seed data.
- The health system is intentionally cross-cutting. Bodies, items, effects, skills, static strings, and seeders all participate.

## Recommended Use
Use this document set in two passes:

1. Read this head document to get the mental model and subsystem boundaries.
2. Use the linked sub-documents to drill into runtime detail, player interactions, adjacent systems, and seeder state.
