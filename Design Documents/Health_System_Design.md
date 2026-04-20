# FutureMUD Health System Design

## Purpose
This document describes the current state of the FutureMUD health system as implemented in the repository after the April 2026 combat-balance profile work. It is a runtime and gameplay reference, not a forward-looking redesign.

The intended audience is:

- Human readers who need a coherent explanation of how health works in play and how it is configured.
- Agentic readers who need a map from gameplay concepts to the interfaces, commands, item types, effects, and seeders that implement them.

## Reading Guide
This is the head document for the health system set:

- [Damage_System_Design.md](./Damage_System_Design.md) covers end-to-end damage flow, seeded damage formulas, armour layering, bodypart and internal routing, and worked examples.
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
| Bodies and anatomy | `IBody`, race bodypart and organ definitions, `Body`, `BodyBiology` | Character health, wound visibility, organ function, death, severing, sever formulas | Seeded mainly in `HumanSeeder`, `AnimalSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder` |
| Health strategies | `IHealthStrategy`, strategy classes in `MudSharpCore/Health/Strategies` | Governs damage conversion, severity, prompts, healing, status | `BrainHitpoints`, `ComplexLiving`, `Robot`, `BrainConstruct`, and `GameItem` all have stock seeded entries |
| Wounds | `IWound`, `SimpleOrganicWound`, `BoneFracture`, `RobotWound` | `wounds`, `wound`, treatment commands, healing ticks | Organic, fracture, and robot play are all now stock reachable |
| Infections | `IInfection`, `Infection`, `SimpleInfection`, `InfectiousInfection`, `NecroticInfection`, `FungalInfection`, `Gangrene` | Wound tags, infection progression, antibiotics, antifungal handling, spread, and tissue damage | No broad content seeding beyond whatever wounds and treatments enable |
| Treatments | `ITreatment`, `TreatmentGameItemComponent`, `TreatmentType` | `bind`, `cleanwounds`, `suture`, `tend`, `relocate`, `dislodge`, `repair` | Bandages, splints, sutures, prosthetics, cannulae seeded in item seeders |
| Surgery | `ISurgicalProcedure`, procedure classes and phases | `surgery`, implant commands, medical knowledges and checks | `HealthSeeder` installs the stock organic surgery set, and `RobotSeeder` installs the stock robot maintenance suite |
| Drugs | `IDrug`, `Drug`, `BodyDrugs`, drug effects | Drug dosing, metabolism, builder editing, medical items, adrenaline support, and paralysis | `HealthSeeder` now installs a broader stock catalogue spanning low-tech remedies through modern pharmaceuticals |
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

Temperature imbalance is now more than descriptive state. Mild and moderate hypo or hyperthermia increase fatigue pressure and slow movement, while severe and critical stages begin to impair organ function. The lethal path is intentionally delayed: only sustained time in critical hypothermia or critical hyperthermia ramps organ penalties far enough to reliably threaten life.

### 5. Secondary complications
Organic wounds can become dirty, infected, gangrenous, fungal, infectious, or necrotic. Breathing failure causes its own damage path. Needs and drugs introduce slow-burn modifiers rather than single-event outcomes.

### 6. Medical intervention
Players and staff can inspect, stabilize, clean, close, tend, apply anti-inflammatory care, repair, relocate, surgically treat, or magically mend wounds. The system cares about sequence. For example, bleeding often must be controlled before cleaning or closure, fractures must be relocated before they can be set, and bodypart-scoped anti-inflammatory care is used to reduce ongoing pain rather than to close damage.

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
- Bodypart prototypes, including optional sever formulas that override the legacy numeric sever threshold when present.
- Editable health strategies through `healthstrategy` / `hs`, including `types`, `typehelp`, `edit new <type> <name>`, and `clone`.
- Editable drugs and their vectors, intensities, and effect payloads.
- Surgical procedures, their phases, knowledges, checks, inventory plans, and progs.
- Treatment-capable item component prototypes and health-adjacent game item components.
- Static strings and flags such as death messages and whether CPR is enabled.
- Skills, checks, and knowledges seeded outside the immediate health runtime code.

One important current-state fact is that the dedicated `HealthSeeder` is now enabled. In practice, stock health behavior is still assembled across multiple seeders rather than coming from one medical step alone, but surgery and drugs now have a release-ready stock entry point.

Another important current-state fact is that `HealthSeeder` now behaves as a rerunnable stock package rather than a strict one-shot bootstrap. The seeded medical knowledges, procedures, phases, and drugs are reused by stable names, same-tech reruns repair missing stock rows in place, and higher-tech reruns add or refresh higher-tech stock content without deleting lower-tech content.

Another important current-state fact is that robot health is no longer only a dormant runtime capability. The stock repo now seeds robot races, robot bodies, robot strategies, robot corpse models, and robot maintenance procedures through `RobotSeeder`, while still reusing the existing humanoid and selected animal body semantics for compatibility.

Combat balance in stock content is also tied back to these health thresholds rather than tuned in isolation. The shared damage tiers seeded by `CombatSeeder` are calibrated so that a standard-quality weapon at nominal strength produces ordinary hits in the routine wound bands, while dedicated finishing moves remain exceptional relative to the stock severity ranges used by the default human strategies.

Another important current-state fact is that combat-facing anatomy tuning is now a shared stock-world choice rather than a separate question in every combat seeder. `HumanSeeder` asks once for `combat-balance-profile`, records the answer through the shared `SeederChoice` flow, and later `CombatSeeder`, `AnimalSeeder`, `MythicalAnimalSeeder`, and `RobotSeeder` reuse that answer automatically.

Current balance-profile behavior:

- `stock` keeps the older FutureMUD combat formulas and still asks the legacy damage-randomness question where relevant
- `combat-rebalance` suppresses that randomness question and rewrites stock-owned bodypart HP, hit chances, natural armour formulas, and shared damage expressions in place while keeping the same named attack and combat-message catalogues
- reruns refresh stock-owned rows for the selected profile instead of seeding duplicate races, bodies, or attack suites

## Tuning Surfaces
The current runtime exposes health tuning in two different ways, depending on where the behavior lives.

Bodypart severing now spans both of those modes:

- the legacy path still uses the stored numeric sever threshold
- builders can instead set `severformula <expression>` on a bodypart prototype
- when a sever formula is present, runtime evaluation receives `damage` and numeric `damagetype` parameters and a result of `>= 1` severs the part
- clearing the formula returns the bodypart to the legacy threshold path

### Health strategy properties
Direct tuning numbers inside `IHealthStrategy` implementations are now treated as optional strategy properties loaded from the `HealthStrategies.Definition` XML.

Builders edit those definitions through the ordinary non-revisable builder pipeline. `BaseHealthStrategy` now supplies the shared editable surface for common commands such as `name` and `lodge`, while each concrete strategy registers its own builder loader, type blurb, and type help text. New strategies are created by type, cloned through the standard editable helper path, and persist their strategy-specific XML plus any linked `TraitExpression` records.

Strategy builder UX now follows two additional conventions:

- ratio-like values such as percentages, multipliers, fractions, and thresholds are shown and entered as percentages rather than raw normalized doubles
- fluid-volume-like values are shown and entered through the `UnitManager`, so builders use their local unit preferences instead of raw litres
- the lodge expression is presented as a `1d100` threshold check, matching the runtime roll

Important current examples include:

- `BrainHitpoints`, `Construct`, and `BrainConstruct` critical injury thresholds.
- `Robot` bleed-message cooldown, power-core critical threshold, hydraulic-fluid paralysis threshold, and prompt handling for the race-configured circulatory liquid.
- `SimpleLiving` and `ComplexLiving` bleed-message cooldowns, internal-bleed decay settings, airway and digestive blood symptom thresholds, cardiac and hypoxia thresholds, contaminant cleanup rates, blood regeneration rates, and critical-injury breakpoints.
- `ComplexLiving` kidney-waste and spleen-cleanup thresholds, plus the chance of aggravating an existing fracture when fresh bone-breaking damage lands on the same bone.

Compatibility rule:

- existing strategy definitions do not need to contain the new XML elements
- when an element is absent, the runtime falls back to the previous hard-coded value
- this preserves the old gameplay behavior for existing worlds unless builders opt in to editing the values

### Static health configuration
Direct tuning numbers in wounds, infections, and other non-strategy health classes are now exposed through global static configurations with defaults in `DefaultStaticSettings`.

Important current groups include:

- wound tending and natural-healing multipliers
- wound sleep and needs penalties
- wound treatment difficulty scaling from repeated failed attempts, antiseptic protection durations, and non-organic repair-degradation risk
- anti-inflammatory duration and pain-reduction values
- external bleeding, reopening, painful-wound thresholds, and minimum blood-floor values
- infection starting intensity, damage-type multipliers, severity multipliers, and cleaning protection chances
- shared infection stage thresholds, visibility thresholds, nausea scaling, and infectious spread modifiers
- bone-fracture healing multipliers, infection multipliers, and treatment progress bonuses
- temperature-imbalance enablement, progression thresholds, movement-delay and stamina multipliers, and the severe-to-critical organ-failure ramp used by hypothermia and hyperthermia

Persistence rule:

- these values live in `StaticConfigurations`
- if a value is missing, `Futuremud.GetStaticConfiguration` inserts the default on first access
- this means older worlds automatically gain the new settings without requiring a hand-written migration

The thermal master switch is `TemperatureImbalanceEnabled`. When it is `false`, environmental and drug-driven temperature imbalance stop creating new staged consequences, health strategy lookups report normal temperature, and the rest of the health runtime ignores thermal penalties.

## Important Current-State Constraints
These are central to understanding the current implementation:

- There is no separate flaw subsystem. Health-related "flaws" are currently represented by merits, negative merit configurations, selective applicability, or other gameplay data.
- Runtime support is still broader than stock content. Several health strategies, corpse models, item systems, and some drug effect families exist without matching stock seed data.
- The health system is intentionally cross-cutting. Bodies, items, effects, skills, static strings, and seeders all participate.

## Recommended Use
Use this document set in two passes:

1. Read this head document to get the mental model and subsystem boundaries.
2. Use the linked sub-documents to drill into runtime detail, player interactions, adjacent systems, and seeder state.
# Addendum: Fire and Corrosive Contact

## Fire Profiles

The health system now supports combat-driven burning via `FireProfile` and the `OnFire` effect.

`FireProfile` defines:

- damage type
- damage / pain / stun per tick
- thermal load per tick
- spread chance
- minimum atmospheric oxidation
- self-oxidising behavior
- extinguish tags

`OnFire` is a scheduled effect that:

- applies periodic burning damage
- escalates thermal imbalance / hyperthermia pressure
- stops when the local atmosphere cannot sustain the profile unless the profile is self-oxidising
- can spread to nearby perceivables

## Atmospheric Support

`IGas` and `Gas` now expose `OxidationFactor`. This value is used by burning effects to determine whether ongoing combustion can continue in a given cell atmosphere.

## Corrosive / Contact Liquids

Liquids can now define `SurfaceReactionInfo`, loaded into `ILiquidSurfaceReaction` entries. These reactions are tag-driven and are evaluated during:

- `LiquidContamination` ticks for items
- `BodyLiquidContamination` ticks for bodies

Each reaction can apply:

- damage
- pain
- stun

This allows effects such as acids attacking flesh but not glass, holy water harming specific tagged creatures, or custom race/material vulnerabilities using existing contamination delivery.

## Persistence Changes

Health-adjacent persistence now includes:

- `Gases.OxidationFactor`
- `Liquids.SurfaceReactionInfo`

The attack system also stores `WeaponAttacks.OnUseProgId`, which is relevant for cooldown-driven elemental attacks such as breath weapons.
