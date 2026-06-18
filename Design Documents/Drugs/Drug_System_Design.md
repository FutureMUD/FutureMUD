# FutureMUD Drug System Design

## Purpose
This document describes the current FutureMUD drug implementation for developers. It is an implementation map, not a redesign proposal.

The content-facing companion is [Drug Builder Guide](./Drug_Builder_Guide.md).

## Implementation Map
| Surface | Primary types |
| --- | --- |
| Public contract | `FutureMUDLibrary/Health/IDrug.cs`, `FutureMUDLibrary/Health/DrugType.cs`, `FutureMUDLibrary/Health/IngestedDrugExtensions.cs` |
| Body state | `IBody.Dose(...)`, `Body.ActiveDrugDosages`, `Body.LatentDrugDosages`, `MudSharpCore/Body/Implementations/BodyDrugs.cs` |
| Concrete editable item | `MudSharpCore/Health/Drug.cs` |
| Persistence | `Drugs`, `DrugsIntensities`, `BodiesDrugDoses`, `Liquids.DrugId`, `Liquids.DrugGramsPerUnitVolume`, `Gases.DrugId`, `Gases.DrugGramsPerUnitVolume` |
| Material payloads | `IFluid`, `ILiquid`, `IGas`, `LiquidMixture`, `LiquidInstance` |
| Item delivery | `Pill`, `PreparedFood`, `TopicalCream`, `Smokeable`, `IncenseBurner`, `Syringe`, `IVBag`, inhaler components |
| Combat delivery | `EnvenomingAttack`, `WeaponPoisonDeliveryHelper`, `WeaponPoisonCoating` |
| Magic delivery | `PoisonEffect`, `RemovePoisonEffect`, `SpellPoisonEffect` |
| FutureProg | `todrug`, `drugintensity`, `drugeffectintensity`, drug dot references, character drug collections |
| Resistances | `ISpecificDrugResistanceMerit`, `IDrugEffectResistanceMerit` |
| Stock data | `HealthSeeder` drug catalogue and delivery components, `AnimalSeeder` venom profiles |

## Core Model
`IDrug` is an editable framework item and FutureProg variable. A drug definition is pharmacology only. Delivery comes from bodies, items, fluids, gases, attacks, spells, and scripts.

`IDrug` exposes:

- `DrugVectors`: a flag set of legal delivery vectors.
- `DrugTypes`: the effect families present on the drug.
- `IntensityPerGram`: the global potency multiplier.
- `RelativeMetabolisationRate`: how quickly active grams are removed relative to other drugs.
- `IntensityForType(type)`: `IntensityPerGram * relative effect intensity`.
- `AdditionalInfoFor<T>(type)`: typed payload data for effect families that need more than a scalar.
- `DescribeEffect(type, voyeur)`: builder/admin-facing effect text.
- `Clone(newName)`: creates a new persisted drug row copied from an existing definition.

`DrugVector` supports `Injected`, `Ingested`, `Inhaled`, and `Touched`. `DrugVector.None` is present as a zero value, but it is not useful as an authored delivery route.

`DrugType` supports:

- `Anesthesia`
- `Analgesic`
- `Immunosuppressive`
- `NeutraliseDrugEffect`
- `BodypartDamage`
- `Pacifism`
- `Rage`
- `Adrenaline`
- `StaminaRegen`
- `Nausea`
- `HealingRate`
- `MagicAbility`
- `NeutraliseSpecificDrug`
- `Paralysis`
- `Antibiotic`
- `OrganFunction`
- `VisionImpairment`
- `ThermalImbalance`
- `Antifungal`
- `PlanarState`

## Additional Info Payloads
Most drug types only need a scalar intensity. For those scalar-only effects, the runtime `ExtraInfo` payload is
intentionally `null` and is persisted as a null `AdditionalEffects` value; load, save, and clone paths should preserve
that null rather than inventing placeholder metadata. These drug types have extra payload classes in `IDrug.cs`:

| Drug type | Payload | Stored meaning |
| --- | --- | --- |
| `NeutraliseDrugEffect` | `NeutraliseDrugAdditionalInfo` | Space-separated `DrugType` integer ids to neutralise |
| `NeutraliseSpecificDrug` | `NeutraliseSpecificDrugAdditionalInfo` | Space-separated drug ids to neutralise |
| `BodypartDamage` | `BodypartDamageAdditionalInfo` | Bodypart or organ type ids to damage |
| `HealingRate` | `HealingRateAdditionalInfo` | Healing-rate intensity and healing-difficulty intensity |
| `MagicAbility` | `MagicAbilityAdditionalInfo` | Magic capability ids to grant |
| `OrganFunction` | `OrganFunctionAdditionalInfo` | Organ type ids to modify |
| `PlanarState` | `PlanarStateAdditionalInfo` | State, plane id, and default-plane visibility |

The concrete `Drug` loader parses the persisted `AdditionalEffects` string into these payload types. Missing or malformed id payload tokens are skipped by the list parsers rather than crashing load. `PlanarState` defaults to `noncorporeal` on the default plane if old or partial payload data is encountered.

## Persistence
`Drug` rows store the stable definition: name, vectors, intensity per gram, and metabolisation rate.

`DrugIntensity` rows are keyed by `(DrugId, DrugType)` and store the per-effect relative intensity plus the optional payload string. `Drug.Save()` rewrites all intensity rows for the drug rather than patching individual rows.

`BodyDrugDose` rows store active and latent dose state on a body. The persisted state includes body id, drug id, grams, original vector, and active flag. The runtime `Originator` field on `DrugDosage` is not persisted; it is useful for live systems such as spell poison effects that need to remove the dose they created.

Liquids and gases can both carry a drug payload through `IFluid.Drug` and `IFluid.DrugGramsPerUnitVolume`. This is material-level content, not part of the drug definition itself.

## Dosing And Heartbeat Flow
All normal dose entry points call `IBody.Dose(IDrug drug, DrugVector vector, double grams, object originator = null)`.

`Body.Dose(...)`:

1. Adds the grams to a matching latent dose with the same drug, original vector, and originator, or creates a new latent dose.
2. Marks drug state changed.
3. Ensures the ten-second drug heartbeat is running.

The body only keeps the drug heartbeat active while the body is active and there are active doses, latent doses, alcohol, or other `ICauseDrugEffect` sources.

On each drug heartbeat, the body:

1. Moves part of each latent dose into active dose state.
2. Applies all drug effects from active doses and external drug-effect sources.
3. Metabolises active doses down.
4. Checks health status.

Latent absorption rates per ten-second heartbeat are:

| Original vector | Latent-to-active rate |
| --- | --- |
| `Injected` | `0.5` |
| `Inhaled` | `0.4` |
| `Touched` | `0.05` |
| `Ingested` | `0.02` |

Active drug removal uses:

`LiverAlcoholRemovalKilogramsPerHour * 10000.0 / 3600.0 * drug.RelativeMetabolisationRate`

Lower relative metabolisation values last longer. Higher values clear faster.

`Body.Dose(...)` does not validate that the supplied vector is present in `drug.DrugVectors`. Validation is mostly performed by authoring surfaces and delivery components. Low-level callers and seeders should check vector compatibility explicitly.

## Effect Aggregation
`Body.ApplyDrugEffects()` builds two counters: net effect intensity by `DrugType`, and neutralising intensity by `DrugType`.

The order is:

1. Add nausea pressure from alcohol.
2. Build active grams per drug.
3. Apply `NeutraliseSpecificDrug` by subtracting grams from named target drugs before their effects are expanded.
4. Add `ICauseDrugEffect` contributions from other active effects.
5. Apply `ISpecificDrugResistanceMerit` multipliers to grams for each concrete drug.
6. Expand each drug's active grams into effect intensities using `IntensityForType(type)`.
7. Accumulate special payload data for healing, organ function, bodypart damage, magic capability, and planar state.
8. Apply `IDrugEffectResistanceMerit` multipliers to summed effect intensities.
9. Create, update, or remove concrete effects based on net intensity after neutralisation.

Most effect intensities are normalised by body mass using:

`netIntensity / (Weight * BaseWeightToKilograms * 0.001)`

Some effect families intentionally use different aggregation semantics:

- `HealingRate` accumulates configured healing-rate and difficulty payload values and applies them through `HealingRateDrug`.
- `MagicAbility` grants configured capabilities only when the accumulated capability intensity is greater than `1.0`.
- `BodypartDamage` creates passive cellular wounds based on configured bodypart types and `OrganDamagePerDrugIntensity`.
- `PlanarState` chooses the strongest active planar payload and applies that overlay if the net effect remains positive.
- `ThermalImbalance` pushes progress into the shared `ThermalImbalance` consequence model rather than storing a drug-specific overlay.

## Supported Drug Effects
| Drug type | Runtime behavior |
| --- | --- |
| `Analgesic` | Creates or updates `Analgesic`, which implements `IPainReductionEffect` with both flat and multiplicative pain reduction. |
| `Anesthesia` | Creates or updates `Anesthesia`, which applies broad check penalties through `ICheckBonusEffect` except for excluded time, wound close, recovery, healing, and infection checks. High values produce drowsiness and breathing-reflex messaging. |
| `Antibiotic` | Creates or updates `Antibiotic`, an `IInfectionResistanceEffect` for simple, infectious, necrotic, and gangrene infections. |
| `Antifungal` | Creates or updates `Antifungal`, an `IInfectionResistanceEffect` for fungal infections. |
| `Immunosuppressive` | Creates or updates `Immunosupressant`, which applies a negative immune bonus. |
| `NeutraliseDrugEffect` | Does not create an effect. Its payload names drug-effect families whose net intensities are reduced. |
| `NeutraliseSpecificDrug` | Does not create an effect. Its payload names concrete drugs whose active grams are reduced before effect expansion. |
| `BodypartDamage` | Applies passive cellular damage to matching organs, bodyparts, and bones using the configured payload types and static organ-damage settings. |
| `Pacifism` | Creates or updates `PacifismDrug`, which exposes `IPacifismEffect` and score/health messaging at higher intensities. |
| `Rage` | Creates or updates `MurderousRage`, which exposes `IRageEffect`, score/health messaging, and can force violent behavior at high intensity. |
| `Adrenaline` | Creates or updates `AdrenalineRush`; also applies heart support floors, thermal load, and possible cardiac cellular damage through static settings. |
| `StaminaRegen` | Immediately grants stamina each drug heartbeat from net normalised intensity. |
| `Nausea` | Creates or updates `Nausea`, which combines drug intensity with blood alcohol content and hunger/thirst state for check penalties, score text, and vomiting. |
| `HealingRate` | Creates or updates `HealingRateDrug`, setting a healing-rate multiplier and integer healing-difficulty stage bonus. |
| `MagicAbility` | Creates or updates `DrugInducedMagicCapability`, granting configured magic capabilities while sufficient active intensity remains. |
| `Paralysis` | Creates or updates `DrugInducedParalysis`, which implements forced paralysis at `DrugParalysisThreshold` and score/health messaging above half threshold. |
| `OrganFunction` | Creates or updates `OrganFunctionDrugEffect`, mapping configured organ types to organ-function bonuses. |
| `VisionImpairment` | Creates or updates `VisionImpairmentDrugEffect`; vision multiplier is clamped at `max(0, 1 - intensity)`. |
| `ThermalImbalance` | Applies progress to the shared thermal imbalance model if `TemperatureImbalanceEnabled` is true. |
| `PlanarState` | Creates or updates `DrugInducedPlanarStateEffect`, a priority-50 planar overlay that can make a body manifested or noncorporeal on a chosen plane. |

Important static configuration keys include:

- `AnasthesiaIntensityToBonusConversionRate`
- `AntibioticInfectionBonusToIntensityMultiplier`
- `AntifungalInfectionBonusToIntensityMultiplier`
- `ImmunosupressantImmuneBonusPerIntensity`
- `NauseaIntensityToBonusConversionRate`
- `DrugParalysisThreshold`
- `OrganDamageFloor`
- `OrganDamagePerDrugIntensity`
- `AdrenalineCheckPenaltyPerIntensity`
- `AdrenalineHeartSupportFloorPerIntensity`
- `AdrenalineHeartSupportMaximumFloor`
- `AdrenalineThermalGainPerIntensity`
- `AdrenalineCardiacDamagePerIntensity`

## Delivery Vectors
### Ingested
Ingested dosing is used by pills, prepared foods, drinks, fed foods, and liquid doses that are consumed.

Entry points include:

- `PillGameItemComponent.Swallow(...)`
- `PreparedFoodGameItemComponent.Eat(...)`
- `BodyNeeds.Drink(...)` and `BodyNeeds.SilentDrink(...)`
- `IngestedDrugExtensions.ApplyIngestedDrugDoses(...)`

`Pill` and `PreparedFood` authoring validates that the chosen drug supports `DrugVector.Ingested`. Liquid materials can be assigned any drug, but ingested dose helpers only apply the payload if the drug supports the ingested vector.

### Injected
Injected dosing is used by syringes, IV bags, envenoming natural attacks, weapon poison injected through wounds, some legal execution content, and magic poison effects configured with an injected vector.

Entry points include:

- `SyringeGameItemComponent.Inject(...)`
- `IVBagGameItemComponent` heartbeat infusion
- `EnvenomingAttackMove`
- `EnvenomingClinchAttack`
- `WeaponPoisonDeliveryHelper`
- `PoisonEffect` / `SpellPoisonEffect`

Living and brain-style health strategies apply the liquid's drug as `DrugVector.Injected` and then process the liquid's `LiquidInjectionConsequence`. `RobotHealthStrategy.InjectedLiquid(...)` ignores drug payloads on injected liquids and only accepts appropriate blood-replacement style liquids.

The injected-liquid health strategy path does not check `drug.DrugVectors.HasFlag(Injected)`. Builders and seeders should not rely on the vector flag as a safety gate for syringe, IV, or envenoming content.

### Inhaled
Inhaled dosing is used by smokeables, inhalers, incense burners, gas payloads, and magic poison effects configured with an inhaled vector.

Entry points include:

- `SmokeableGameItemComponent.Smoke(...)`
- `ExternalInhalerGameItemComponent.Puff(...)`
- `IntegratedInhalerGameItemComponent.Puff(...)`
- `IncenseBurnerGameItemComponent.PulseDrug(...)`
- `PoisonEffect` / `SpellPoisonEffect`

Smokeable and incense-burner prototypes validate that the drug supports `DrugVector.Inhaled`. Inhaler components also check the gas drug's inhaled flag before dosing. Gas material authoring itself does not enforce the flag.

### Touched
Touched dosing is used by topical creams and contact weapon poison delivery.

Entry points include:

- `TopicalCreamGameItemComponent.Apply(...)`
- `WeaponPoisonDeliveryHelper.DoseContact(...)`
- `PoisonEffect` / `SpellPoisonEffect`

Topical cream prototypes validate that the drug supports `DrugVector.Touched`. Weapon poison delivery filters coating mixtures by the touched flag before applying contact dosing.

Ordinary surface wetness is not a general-purpose skin-absorption system. Body and item surface liquids persist wetness, descriptions, residues, cleaning requirements, and liquid surface reactions. They do not automatically apply `DrugVector.Touched` doses unless a delivery component or helper explicitly calls `Body.Dose(...)`.

## Drug Liquids, Gases, Foods, And Tinctures
`IFluid` is the shared contract for liquids and gases. A fluid can carry one drug and a grams-per-unit-volume value.

For liquids:

- Drinking uses `ApplyIngestedDrugDoses(...)`, so only ingested-vector drugs are dosed.
- Injection uses `IHealthStrategy.InjectedLiquid(...)`, so living health strategies dose the drug as injected.
- Weapon poison uses the liquid mixture and filters for touched or injected vector compatibility.
- Prepared food can absorb liquid and convert ingested drug payloads into food drug doses.
- Liquid surface contamination and reactions are separate from drug dosing.

For gases:

- Inhalers consume gas and dose the gas drug if it supports the inhaled vector.
- Gas material authoring stores the payload; the inhaler decides whether it becomes a body dose.

Prepared foods store direct `FoodDrugDose` entries and optional stale/spoiled drug doses. Eating applies a proportion of those grams based on bites consumed. Food can also absorb liquid contamination or liquid exposure, adding ingredient ledger rows and ingested drug doses for compatible drug liquids.

Tinctures, draughts, teas, salves, poultices, ointments, smokes, and venom are content conventions rather than distinct pharmacology classes. For example:

- A tincture is usually an ingested drug plus a liquid material or prepared-dose item.
- A salve or poultice is usually a touched drug plus `TopicalCream`.
- Smoke or fumigation is usually an inhaled drug plus `Smokeable` or `IncenseBurner`.
- Venom is usually an injected and touched drug plus a liquid material and either `EnvenomingAttack` or weapon poison coating.

## Builder And Admin Surface
`drug` is the primary builder command. It is backed by `EditableItemHelper.DrugHelper` and `Drug.BuildingCommand(...)`.

Supported editing operations include:

- create, clone, edit, list, show, and close
- rename
- set global intensity
- set relative metabolism
- toggle vectors
- add, update, or remove effect intensities
- configure payload data for bodypart damage, organ function, healing rate, magic capability, neutralisation, specific-drug neutralisation, and planar state

`show drug <which>` and `show drugs` expose read-only display paths. Staff can use `drugs <target>` to inspect active and latent dosages through `DrugDosageDescriber`.

## FutureProg Surface
Drugs are `ProgVariableTypes.Drug`.

Lookup and inspection include:

- `todrug(number)` and `todrug(text)`
- `drugintensity(character, drug)`
- `drugeffectintensity(character, number)`
- character properties for active drugs, active amounts, latent drugs, and latent amounts
- drug dot references for id, name, intensity, metabolisation rate, vectors, types, and intensities

Current implementation caveats:

- `Drug.GetProperty` handles `itensities` rather than `intensities`, while the compiler/help advertises `intensities`.
- The character `latentdrugamounts` property currently sums matching active doses rather than latent doses.

Consumers that need exact dosage state from C# should read `Body.ActiveDrugDosages` and `Body.LatentDrugDosages` directly.

## Stock Seeding
`HealthSeeder` seeds a tech-level drug catalogue and then creates delivery component prototypes for supported vectors:

- `Pill_<DrugName>` for ingested-vector drugs.
- `TopicalCream_<DrugName>` for touched-vector drugs.
- `Smokeable_<DrugName>` for inhaled-vector drugs.

The stock catalogue includes primitive, pre-modern, and modern examples across analgesia, anesthesia, infection control, nausea control, organ support, paralysis, adrenaline, stamina support, healing-rate modification, immunosuppression, and neutralisation.

`AnimalSeeder` creates race-specific venom drugs, matching venom liquids, and envenoming natural attacks. Venom profiles typically use `Injected | Touched`, a liquid injection consequence, a high `DrugGramsPerUnitVolume`, and `EnvenomingAttack` additional info.

`Design Documents/Data/Seeded_Item_Components.json` is the maintained catalogue for seeded delivery component prototypes.

## Extension Guidelines
When adding a new drug effect family:

1. Add the `DrugType` enum value.
2. Add a `DrugAdditionalInfo` payload class only if the effect needs structured extra data.
3. Update `Drug.AdditionalInfoFor(...)`, builder help, builder commands, `Show(...)`, and `DescribeEffect(...)`.
4. Update `Body.ApplyDrugEffects()` to aggregate, apply resistance and neutralisation correctly, create/update effects, and remove effects when net intensity is gone.
5. Add or reuse concrete effect interfaces so other systems can query the result.
6. Update FutureProg docs if the new effect needs scripting support.
7. Add focused tests around parsing, builder setup, effect application, cleanup, and seeder examples.
8. Update this document and the builder guide.

When adding a new delivery mechanism:

1. Keep pharmacology on `IDrug`; keep delivery-specific state on the item, fluid, attack, spell, or command surface.
2. Validate vector compatibility in authoring and again at runtime where feasible.
3. Preserve grams as the common dose unit.
4. Decide whether the dose should be latent through `Body.Dose(...)` or immediate through an existing health-strategy pathway such as `InjectedLiquid(...)`.
5. Include originator objects when later removal or grouping matters.
6. Add a seeded example only when the content package owns both the drug and the delivery wrapper.

## Implemented Expansion Families
The following effect families are first-class drug types. They use the same `IDrug` intensity model as older effects, with structured `DrugAdditionalInfo` payloads where a single scalar is not enough.

### Coagulation And Bleeding Control
`DrugType.Coagulation` creates a `DrugCoagulationEffect` that implements `IBleedingModifierEffect`.

- Payload: external bleeding, wound reopen, and internal bleeding multipliers.
- Runtime hooks: `SimpleOrganicWound` external bleeding, wound reopen checks, `PeekBleed`, and internal-bleeding totals in living health strategies.
- Scope: organic bleeding only. Synthetic and robot leakage is intentionally unchanged.
- Caps: `DrugCoagulationMinimumMultiplier` and `DrugCoagulationMaximumMultiplier` default to `0.25x` and `3.0x`.

### Respiratory Drive And Hypoxia Modulation
`DrugType.Respiration` creates a `DrugRespirationEffect` that implements `IRespirationModifierEffect`.

- Payload: breathing-drive, hypoxia-damage, and airway-tolerance multipliers.
- Runtime hooks: lung, blowhole, and gill breathers use breathing-drive modifiers around anesthesia/depressant thresholds; lung and blowhole breathers use airway support for marginal lung/airway bleeding thresholds; living health strategies apply hypoxia damage multipliers.
- Hard limits: drugs do not create breathable fluid, replace missing organs, replace heart function, or bypass `IStopBreathing`.
- Caps: `DrugRespirationMinimumMultiplier` and `DrugRespirationMaximumMultiplier` default to `0.25x` and `3.0x`.

### Metabolic And Need-Rate Drugs
`DrugType.NeedRate` creates a `DrugNeedRateEffect` that implements `INeedRateEffect`.

- Payload: hunger, thirst, and drunkenness multipliers plus passive/active applicability flags.
- Runtime hooks: active and passive needs models consume the effect through their existing multiplier paths.
- Scope: `NoNeedsModel` remains a natural no-op.

### Sleep, Wakefulness, And Consciousness Thresholds
`DrugType.Arousal` creates one or more drug-caused arousal effects depending on its payload.

- Payload: mode flags, check modifier, sleep and knockout thresholds, pain/stun/anesthesia consciousness threshold multipliers, and stamina regeneration/cost multipliers.
- Modes: `SleepInducing`, `SleepPreventing`, `PassOutResistance`, `Knockout`, `Stimulant`, and `Sedative`.
- Runtime hooks: sleep-inducing drugs use normal sleep transitions when legal; sleep-preventing drugs block voluntary sleep with a clear prevention effect; pass-out resistance reuses `IPreventPassOut`; knockout mode uses the existing loss-of-consciousness health-status path.
- Protection: stimulants do not remove magical sleep or `NoWake` effects.

### Tolerance, Dependence, And Withdrawal
`DrugType.Dependence` is metadata rather than a direct moment-to-moment drug effect. It creates per-body exposure history for drugs that explicitly opt in.

- Payload: exposure gain/decay, tolerance threshold, minimum effectiveness, withdrawal onset/decay, affected drug types, and withdrawal symptoms.
- Persistence: `Bodies_DrugExposures` stores one row per `BodyId + DrugId` with exposure, peak exposure, withdrawal intensity, and last updated UTC time.
- Runtime hooks: drug heartbeats decay exposure from `LastUpdatedAtUtc`, add exposure from active matching doses, reduce effective intensity for configured affected effect types, and create withdrawal effects when exposure drops below onset after prior dependence.
- Withdrawal expression: symptoms reuse existing pathways such as check modifiers, need-rate effects, stamina multipliers, nausea, rage, and sleep prevention.
- Caps and gates: `DrugDependenceEnabled` disables the feature globally. Minimum tolerance effectiveness defaults to at least `25%` unless a drug explicitly configures a lower floor.
