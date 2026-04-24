# FutureMUD Health System: Adjacencies and Items

## Scope
This document covers health-adjacent systems that are tightly coupled to the body and medical runtime:

- breathing
- needs
- corpses and severed bodyparts
- game items that directly participate in health
- cross-cutting effects that glue long-running behavior together

## Breathing
### Core model
Breathing is a strategy-driven subsystem exposed through `IBreathingStrategy`.

Breathing matters because it directly participates in:

- whether a character can stay conscious and alive
- held-breath timing
- hypoxia and suffocation damage
- the value of rebreathers and breathable environments
- organ-function dependencies such as lung, trachea, and heart performance

### Strategy families
| Strategy | Current role | Stock seeding status |
| --- | --- | --- |
| `LungBreather` | Standard lung-based breathing with support for breathable fluids, organ checks, and rebreathers | Seeded for humans and many animals |
| `GillBreather` | Water or fluid-breathing model | Seeded through animal race setup |
| `BlowholeBreather` | Specialized aquatic or semi-aquatic breathing model | Seeded through animal race setup |
| `NonBreather` | Bodies that do not breathe | Runtime support present; can be assigned by race configuration |
| `PartlessBreather` | Bodies without the normal organ-part assumptions of a lung breather | Runtime support present; can be assigned by race configuration |

### Current implementation notes
`LungBreather` currently considers:

- whether the environment provides breathable fluid
- lung, heart, and trachea function
- internal bleeding implications
- anesthesia and related impairment
- `IStopBreathing` effects that can forcibly prevent breathing
- gas support from any `IProvideGasForBreathing` source, not only rebreathers

## Needs
### Core model
Needs are separated from wounds but still participate in health because they alter long-term survival and body-state management.

`INeedsModel` is the contract that controls ongoing hunger, thirst, alcohol, water, and related fulfillment or decay behavior.

Race data controls both the rate and capacity of hunger and thirst. `HungerRate` and `ThirstRate` multiply the active heartbeat decay speed, while `MaximumFoodSatiatedHours` and `MaximumDrinkSatiatedHours` cap how many positive satiation hours a character of that race can store. The player-facing labels scale from these caps: food becomes peckish/full/absolutely stuffed at 25/50/75% of the food limit, and drink becomes not thirsty/sated at 50/75% of the drink limit. Builders edit these through the race-building `hunger`, `thirst`, `hungerlimit`, and `thirstlimit` options.

### Needs model families
| Needs model | Current behavior | Current stock usage |
| --- | --- | --- |
| `NoNeedsModel` | No active need decay | Explicitly used for some defaults, chargen settings, and all dead characters |
| `PassiveNeedsModel` | Supports change when something acts on the needs directly, but does not drive active decay in the same way as the full model | Runtime support present |
| `ActiveNeedsModel` | Full ticking hunger, thirst, alcohol, and water behavior with merit and effect modifiers | Runtime support present |

### Health interaction
Needs influence health indirectly rather than by creating `IWound` objects.

They matter because:

- some bodies should temporarily ignore needs, especially dead characters
- drug and alcohol state interacts with the needs model
- `NeedRateChangingMerit` modifies the speed of needs
- `DelayedNeedsFulfillment` and related effects delay or smooth out need resolution

When a character dies, `CharacterHealth` explicitly switches the character to `NoNeedsModel`. When resurrected, the needs model is reloaded from chargen or configured defaults.

## Corpses and Severed Bodyparts
### Death flow
When a character dies:

- the character state becomes dead and deceased
- the race's corpse model is consulted
- if the model is configured to create corpses, a corpse item is created through `CorpseGameItemComponentProto.CreateNewCorpse`
- the body is transitioned to dead state
- the player-facing dead character is pushed to `NoNeedsModel`

This means death is both a character-state change and a world-item creation event.

### Corpse model support
Current runtime corpse model types:

| Corpse model | Role | Stock seeding status |
| --- | --- | --- |
| `StandardCorpseModel` | Decaying corpse model with terrain-based decay and decay-state descriptions | Seeded in stock human and animal content |
| `NonDecayingCorpseModel` | Static corpse description model with no decay progression | Runtime support present; not broadly stock seeded |

### Severed bodyparts
Severed parts are part of the same conceptual system as corpses. Wounds can reference severed bodyparts, corpse models can describe severed remains, and cleanup or restoration flows may need to manage them explicitly.

From a gameplay perspective, severing makes injury persistence visible and materially changes later surgery and corpse-state handling.

## Game Items in the Health System
### Items as wound owners
FutureMUD does not reserve health logic only for characters. `GameItem` implements `IHaveWounds`.

That means game items can:

- have a health strategy
- accumulate wounds
- use wound severity logic
- be repaired or destroyed through wound processing

Stock item content uses the `GameItem` health strategy seeded in `CoreDataSeeder`.

### Health-adjacent item component families
| Interface or component | Role in health play |
| --- | --- |
| `ITreatment` and `TreatmentGameItemComponent` | Lets an item function as a bandage, suture kit, cleaning tool, or other wound-treatment tool |
| `ICorpse` and `CorpseGameItemComponent` | Turns items into corpses and severed remains with owner and decay context |
| `ICannula` and `CannulaGameItemComponent` | Supports vascular access and cannulation procedures |
| `IVBagGameItemComponent` | Supports infused liquids and IV-style medical logistics |
| `IProvideGasForBreathing` and `RebreatherGameItemComponent` | Supplies breathable gas to bodies that need it |
| `BreathingFilterGameItemComponent` | Modifies the breathing stream through equipment |
| `IDefibrillator` and `DefibrillatorGameItemComponent` | Equipment-gated electrical rescue support |
| `IExternalOrganFunction` and `ExternalOrganGameItemComponent` | External artificial-organ support for body systems |
| `IImplant` and `ImplantBaseGameItemComponent` | Core implant behavior shared by specialized implant types |
| `IOrganImplant` and `ImplantOrganGameItemComponent` | Implanted artificial organs and organ augmentation |
| `ImplantPowerPlantGameItemComponent`, `ImplantPowerRouterGameItemComponent`, `ImplantPowerSupplyGameItemComponent` | Power infrastructure for advanced implants |
| `ImplantRadioGameItemComponent`, `ImplantTraitChangerGameItemComponent`, container-style implant components | Advanced implant payloads that sit adjacent to the health framework |
| Prosthetic component prototypes in seeders | Replacement limbs and eyes that restore or replace severed functionality |

### Design implication
The item system makes health extensible without constantly adding new commands. Once the correct interfaces and components exist, bodies, surgeries, breathing, and treatment code can discover and use those items through shared contracts.

## Cross-Cutting Effects
Effects are the main glue layer that turns health actions into ongoing states.

### Key effect groups
| Effect group | Concrete effects | Runtime role |
| --- | --- | --- |
| Wound-care actions | `CleaningWounds`, `TendingWounds`, `RepairingWounds`, `RelocatingBone`, `AntisepticProtection`, `AntiInflammatoryTreatment`, `SupressWoundMessages` | Timed treatment actions, antiseptic persistence, bodypart-scoped pain relief, and output control |
| Rescue and circulation | `PerformingCPR`, `CPRTarget`, `StablisedOrganFunction` | Tracks who is performing CPR, who is receiving it, and whether organ function has been temporarily restored |
| Drug-driven modifiers | `HealingRateDrug`, `ThermalImbalance`, `DrugThermalImbalance` (legacy load path), `DrugInducedMagicCapability`, `PacifismDrug`, `OrganFunctionDrugEffect`, `VisionImpairmentDrugEffect`, `AdrenalineRush`, `AdrenalineHeartSupportEffect`, `DrugInducedParalysis`, `InfectionNausea` | Converts drug totals and infection state into persistent character-state changes, including shared staged hypo or hyperthermia consequences |
| Long-term health modifiers | `HealingRateEffect`, `LimbMissingBoneEffect`, `SurgeryFinalisationRequired` | Connects wounds, missing structure, and surgery follow-up to the wider effect system |
| Need timing | `DelayedNeedsFulfillment` | Delays or stages need resolution instead of resolving it instantly |

### Why effects matter
Without effects, health would be mostly instant command resolution. Effects let FutureMUD represent:

- ongoing CPR instead of a one-line revive
- staged wound cleaning and tending
- lingering antiseptic protection
- temporary organ stabilization
- active drug states that continue to matter between commands

That is one of the main reasons the health system feels integrated into the rest of the engine rather than like a separate minigame.

## Verified Current Behavior Summary
The adjacent systems are not optional extras. In current FutureMUD they are part of health proper:

- breathing determines whether a body can remain viable
- needs determine whether a body has long-term upkeep pressure
- corpses and severed parts persist death and injury into the world state
- items are active participants in treatment, life support, and replacement anatomy
- effects carry long-running medical state between commands

## Seeded Defaults Summary
Stock content currently provides:

- breathing model setup through race seeders
- standard corpse models
- low-tech prosthetics, bandages, splints, sutures, and cannula items
- IV and prosthetic component prototypes

By contrast, many higher-tech medical item systems are present in runtime but are not broadly seeded by default.

## Gaps and Extension Pressure
The clearest extension paths in this area are:

- broader stock use of advanced implant and external-organ systems
- stock defibrillator and rebreather item content
- more worlds taking advantage of passive and active needs outside the current default paths
