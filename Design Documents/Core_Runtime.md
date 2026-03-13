# FutureMUD Health System: Core Runtime

## Scope
This document covers the core runtime model for health in FutureMUD:

- `IBody` and the body implementation
- anatomy, organ, bone, blood, and health-state processing
- health strategies
- wound families
- infections
- health-related merits and flaw-like configurations

## Primary Contracts
| Contract | Role in the runtime |
| --- | --- |
| `IBody` | Central integration point for a character's anatomy, wounds, blood, drugs, breathing, infections, organs, and needs |
| `IHaveWounds` | Shared surface for anything that can take wounds, including characters and game items |
| `IHealthStrategy` | Chooses how damage becomes wounds, how severity is calculated, and how health status is evaluated over time |
| `IWound` | Concrete localized injury state, including damage, bleeding, pain, stun, shock, treatment logic, and healing |
| `IInfection` | Secondary complication attached to a wound and bodypart, with its own tick, spread, and damage behavior |

## `IBody` as the Runtime Hub
`IBody` is the single most important health interface. It is the place where FutureMUD stops treating health as abstract hit points and starts treating it as an anatomical simulation.

In current runtime terms, `IBody` owns or exposes:

- current wounds and visible wounds
- bodyparts, bones, and organs
- blood volume and blood-model data
- active and latent drugs
- infections
- breathing state and breathing strategy
- needs model access
- health ticks, healing ticks, and status checks
- severing, corpse relationships, and death-adjacent state

The implementation is spread across `Body.cs` and partials such as `BodyBiology.cs`, `BodyNeeds.cs`, and `BodyDrugs.cs`.

`Character` mostly delegates health-facing interfaces to `Body`. That means the character-level public surface is thin, while the real runtime behavior lives in the body implementation.

## Body Biology and Health Processing
### Anatomy
Health is built on the race and body prototype systems. A race selects:

- a body layout
- organs and organ types
- bones and bone relationships
- bodypart properties such as implant space, infectability, damage multipliers, pain multipliers, and hypoxia sensitivity
- blood model and population blood model
- corpse model
- breathing model
- default health strategy

This makes biology highly data-dependent before any wound logic is even reached.

### Damage Routing
For characters, damage handling is not a single subtraction. The body implementation considers:

- natural resistance or armor interaction
- target bodypart
- whether secondary organ or bone damage should occur
- severing rules
- bleeding consequences
- the wound family appropriate to the target and strategy

The resulting wound object stores both original and current damage, plus current pain, stun, shock, bleed status, lodged items, and treatment state.

### Blood and Organ Function
The body runtime continuously tracks more than visible wounds:

- current versus total blood volume
- heart, kidney, liver, spleen, and other organ function
- internal bleeding and other hidden trauma
- consequences of low circulation for prompts, health checks, and death

Temperature imbalance now participates in this same layer. Mild and moderate stages remain mostly symptomatic, but severe and especially critical hypothermia or hyperthermia apply organ-function penalties through the effect system. That means thermal injury is reversible while exposure is corrected, but can still become fatal if a body is left in critical extremes for long enough.

This is why medical commands such as `vitals`, `triage`, and surgery are meaningful. They are reading and acting on underlying simulated state, not only on visible wound descriptions.

### Health Ticks and Status
Health is advanced over time, not only when a wound is created.

`IHealthStrategy.PerformHealthTick` and `EvaluateStatus` cooperate with `BodyBiology` to produce the major state transitions:

- no change
- paralyzed
- unconscious
- pass out
- dead

Those outcomes depend on aggregate wound state, blood volume, organ performance, breathing status, and sometimes merit or effect modifiers.

For `ComplexLivingHealthStrategy`, temperature exposure is now processed as a staged continuum:

- very mild stages are informational only
- mild and moderate stages slow movement and worsen stamina efficiency
- severe stages begin small organ impairment without being immediately lethal
- critical stages ramp organ penalties over sustained exposure until ordinary brain and heart failure rules can take over

This entire runtime branch is gated by the static configuration `TemperatureImbalanceEnabled`.

## Health Strategies
`IHealthStrategy` is the runtime policy object that decides how an owner experiences damage and healing.

### Strategy responsibilities
Each strategy is responsible for:

- turning `IDamage` plus a bodypart into one or more `IWound` instances
- rating wound severity
- defining health prompts and condition reporting
- setting maximum health, pain, and stun pools
- deciding healing tick amounts
- performing health evaluation and organ-side effects

### Strategy families in current runtime
| Strategy class | Typical owner type | Current role | Stock seeding status |
| --- | --- | --- | --- |
| `BrainHitpointsStrategy` | Characters | Organic model with a simpler overall survivability frame and strong brain/vital emphasis | Seeded in `HumanSeeder` and `AnimalSeeder` |
| `ComplexLivingHealthStrategy` | Characters | Full organic simulation model for living beings with richer organ and wound interaction | Seeded in `HumanSeeder` and `AnimalSeeder` |
| `GameItemHealthStrategy` | Game items | Damage model for items that can be wounded or broken | Seeded in `CoreDataSeeder` |
| `SimpleLivingHealthStrategy` | Characters | Simpler living-organic runtime alternative | Runtime support only in stock repo |
| `ConstructHealthStrategy` | Characters or entities | Nonliving construct model | Runtime support only in stock repo |
| `BrainConstructHealthStrategy` | Characters or entities | Construct model with brain-style vital targeting | Runtime support only in stock repo |
| `RobotHealthStrategy` | Characters or entities | Mechanical or synthetic model with robot wound semantics | Runtime support only in stock repo |
| `BaseHealthStrategy` | Shared base | Shared implementation infrastructure, not a stock selectable endpoint by itself | Not seeded directly |

### Builder surface
Health strategies now participate in the ordinary non-revisable editable-item workflow. Builders use `healthstrategy` (alias `hs`) to list, show, edit, create, and clone strategy definitions.

Type discovery is driven by `BaseHealthStrategy` registrations rather than hard-coded command switches:

- `healthstrategy types` lists the registered strategy families with blurbs.
- `healthstrategy typehelp <type>` shows the registered help text for a family before creation.
- `healthstrategy edit new <type> <name>` invokes the registered builder loader for that type.
- `healthstrategy clone <which> <name>` duplicates the strategy definition and clones any linked `TraitExpression` records used by that strategy.

Shared editable commands such as `name` and `lodge` live on `BaseHealthStrategy`. Concrete strategies handle their own subtype-specific builder commands and then fall back to the base implementation.

### Design implication
Health strategies are how FutureMUD keeps one set of command surfaces usable across very different bodies. The same command module can inspect wounds on a human, a robot, or a game item, while the strategy and wound family decide what those actions actually mean.

## Wounds
### `IWound`
`IWound` is a rich contract rather than a label.

An `IWound` can expose:

- the affected bodypart
- a severed bodypart reference
- lodged objects
- bleed status
- original and current damage
- current pain, stun, and shock
- severity
- infection state
- treatment gates through `CanBeTreated`, `WhyCannotBeTreated`, and `Treat`
- healing and exertion behavior

### Wound families in current runtime
| Wound class | Role |
| --- | --- |
| `SimpleWound` | Core wound base behavior shared by multiple concrete wound paths |
| `HealingSimpleWound` | Shared healing-oriented behavior for wounds that naturally tick toward recovery |
| `SimpleOrganicWound` | Main organic character wound model, including bleeding, infection, cleaning, tending, closure, and lodged-object logic |
| `BoneFracture` | Bone-specific injury model with relocation, immobilization, and surgical reinforcement stages |
| `RobotWound` | Mechanical wound model with leakage, sealing, repair, and lodged-object support |

### Wound design considerations
The wound model cares about sequence and local state. Examples:

- Organic wounds usually cannot be cleaned or tended while actively bleeding.
- Wounds with lodged objects block several other treatments until the object is removed.
- Wounds must generally be trauma-controlled before they can be closed.
- Bone fractures must be relocated before non-surgical immobilization is valid.
- Robot wounds use fluid-leak and repair semantics rather than organic infection and antiseptic logic.

This gives FutureMUD a deliberately procedural medical game loop instead of a flat "use heal item" mechanic.

## Infections
### Runtime model
An infection is a secondary entity tied to a wound and bodypart. It has:

- a type
- intensity
- virulence and immunity values
- persisted virulence difficulty and virulence multiplier data
- stage progression
- optional spread behavior
- optional direct damage output
- wound-tag reporting for examinations

Infections are advanced by infection ticks rather than only by treatment commands.

### Implemented infection families
| Infection type surface | Current concrete support | Notes |
| --- | --- | --- |
| `Simple` | `SimpleInfection` | Generic organic infection progression |
| `Gangrene` | `Gangrene` | Specialized severe infection path |
| `Necrotic` | `NecroticInfection` | Infectious spread model plus necrotic damage ticks |
| `Infectious` | `InfectiousInfection` | Simple infection model plus proximity-based spread to other characters |
| `FungalGrowth` | `FungalInfection` | Similar to simple infection, but resisted by antifungal effects rather than antibiotics and accelerated by wet or hot conditions |

### Infection interactions
Infections currently interact with:

- wound cleanliness and antiseptic treatment
- infection resistance merits
- antibiotics and antifungal resistance effects through the drug system
- triage and examination reporting
- bodypart-specific wound state
- proximity and contact exposure for infectious families
- direct necrotic damage for necrotic families

### Current implementation detail
The infection runtime now includes the previously partial pieces that mattered most to play:

- `Infection` persists both virulence difficulty and virulence multiplier.
- `SimpleInfection` applies and removes a concrete nausea effect as infection intensity changes.
- `InfectiousInfection` uses real local proximities when attempting to spread to nearby characters.
- `NecroticInfection` converts infection intensity into ongoing necrotic damage.
- `FungalInfection` progresses much faster when the affected area is wet or the patient is running hot.

## Health-Related Merits and Flaw-Like Configurations
There is no separate flaw subsystem in the current repository. Health-related disadvantages are represented through the merit system, negative or selective merit data, or ordinary character configuration.

### Relevant merit classes
| Merit class | Health impact |
| --- | --- |
| `AdditionalBodypartMerit` | Adds bodyparts and therefore changes anatomical and wound routing possibilities |
| `AllInfectionResistanceMerit` | Broad resistance to infection |
| `BoneHealthMerit` | Changes fracture resilience or bone-health behavior |
| `DrugEffectResistanceMerit` | Reduces intensity of whole drug-effect families |
| `SpecificDrugResistanceMerit` | Reduces intensity of individual drugs |
| `NeedRateChangingMerit` | Changes hunger, thirst, alcohol, or similar need rates |
| `OrganHitChanceReductionMerit` | Makes organ hits less likely |
| `SurgeryFinalisationMerit` | Modifies invasive procedure closure or recovery support |
| `WillToLiveMerit` | Modifies survivability-relevant thresholds such as hypoxia and vital recovery behavior |
| `FixedBloodTypeMerit` | Locks or constrains blood typing |

### Design implication
These merits are not cosmetic. They plug directly into infection resistance, drug intensity adjustment, need decay, wound routing, and survival thresholds. In practice they are part of the health model, not merely part of character flavor.

## Verified Current Behavior Summary
The current health runtime is built around these truths:

- `IBody` is the integration center.
- `IHealthStrategy` defines how damage is interpreted.
- `IWound` stores local injury state and treatment logic.
- `IInfection` is a live secondary system, not just a wound tag.
- Merits materially alter how health systems behave.

## Seeded Defaults Summary
Stock seeders currently bias toward:

- organic character play
- `BrainHitpoints` and `ComplexLiving` strategies
- standard corpses
- broad anatomical detail for humans and animals

Detailed stock surgery and drugs are handled separately and are described in [Seeder_and_Gaps.md](./Seeder_and_Gaps.md).

## Gaps and Extension Pressure
The current runtime already suggests several extension paths:

- broader stock use and tuning of the now-implemented infection families
- broader stock use of non-organic health strategies
- clearer negative-health templates if a dedicated flaw subsystem is ever added
- broader stock content that actually uses the newer infection, breathing, and recovery runtime hooks
