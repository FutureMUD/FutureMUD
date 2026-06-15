# FutureMUD Drug Builder Guide

## Purpose
This guide is for builders, seeder developers, and AI agents creating drug content in FutureMUD.

Use [Drug System Design](./Drug_System_Design.md) when you need implementation details.

## Mental Model
A drug is the pharmacology. It says what the substance does, which delivery routes can work, how strong it is per gram, and how quickly it clears.

A delivery wrapper is what gets the drug into a body. Pills, foods, liquids, gases, smokes, salves, syringes, venoms, weapon coatings, incense burners, and spells are all delivery wrappers.

Build drug content in this order:

1. Define the drug.
2. Choose the legal vectors.
3. Add effect families and any required payloads.
4. Create one or more delivery wrappers.
5. Test the actual route in-game or with a focused seeder/unit test.

## Delivery Vectors
| Vector | Use for | Common wrappers | Notes |
| --- | --- | --- | --- |
| `Ingested` | Pills, teas, draughts, tinctures, edible poison, medicated food, drinkable liquids | `Pill`, `PreparedFood`, drinkable `ILiquid`, feed/drink flows | Slowest absorption. Drinking applies only if the drug has this vector. |
| `Injected` | Syringes, IV medication, venom fangs, poisoned weapon wounds, injected spells | `Syringe`, `IVBag`, `EnvenomingAttack`, weapon poison coating, spell poison | Fast absorption. Injected-liquid health strategies do not enforce the vector, so authoring discipline matters. |
| `Inhaled` | Smoke, vapour, anesthetic gas, inhalers, incense/fumigation | `Smokeable`, `IncenseBurner`, inhaler gas canisters, gas materials | Fast absorption. Smokeable, incense, and inhaler runtime paths check this vector. |
| `Touched` | Salves, poultices, contact poison, topical anesthetic | `TopicalCream`, contact weapon poison | Slow topical absorption. Ordinary wetness alone does not dose drugs. |

## Creating A Drug
Use the `drug` builder command.

Typical flow:

```text
drug edit new Foxglove Tincture
drug set intensity 50%
drug set metabolism 6%
drug set ingested
drug set type intensity OrganFunction 25%
drug set type organfunction Heart
drug set type intensity Nausea 25%
drug set type intensity VisionImpairment 10%
drug show
drug close
```

Useful commands:

```text
drug list
drug list *Analgesic
drug show <drug>
drug edit <drug>
drug edit new <name>
drug clone <old> <new name>
drug set name <name>
drug set intensity <%>
drug set metabolism <%>
drug set injected
drug set ingested
drug set inhaled
drug set touch
drug set type intensity <DrugType> <%>
```

Set an effect intensity to `0%` or less to remove that effect type.

## Effect Families
| Effect | Builder use | Extra setup |
| --- | --- | --- |
| `Analgesic` | Pain relief | None |
| `Anesthesia` | Sedation and broad check penalties | None |
| `Antibiotic` | Resistance to ordinary, infectious, necrotic, and gangrene infections | None |
| `Antifungal` | Resistance to fungal infections | None |
| `Immunosuppressive` | Immune penalty | None |
| `NeutraliseDrugEffect` | Antiemetics, antipyretics, broad antagonists by effect family | `drug set type neutralise <DrugType>` |
| `NeutraliseSpecificDrug` | Overdose reversal or antidotes for named drugs | `drug set type neutralisespecific <drug>` |
| `BodypartDamage` | Organ/tissue toxic damage | `drug set type damage <bodypart type>` |
| `Pacifism` | Calm, euphoria, reduced violence drive | None |
| `Rage` | Irritation through forced violence at high intensity | None |
| `Adrenaline` | Heart racing, general activity penalties, heart support, heat/cardiac stress | None |
| `StaminaRegen` | Immediate stamina gain per drug tick | None |
| `Nausea` | Nausea, check penalties, vomiting at high effective intensity | None |
| `HealingRate` | Healing speed and difficulty changes | `drug set type healingrate <rate%> <difficulty%>` |
| `MagicAbility` | Temporary magic capabilities | `drug set type magic <capability>` |
| `Paralysis` | Numbness and forced paralysis past threshold | None |
| `OrganFunction` | Functional bonus or support to organ types | `drug set type organfunction <organ type>` |
| `VisionImpairment` | Reduced vision multiplier | None |
| `ThermalImbalance` | Heat/cold imbalance pressure through the shared temperature model | None |
| `PlanarState` | Manifested or noncorporeal planar overlay | `drug set type planar <corporeal|noncorporeal> [plane] [visible]` |

The required extra setup commands only work after you add a matching effect intensity.

## Potency And Duration
There are three separate knobs:

- `drug set intensity <%>` controls whole-drug potency per gram.
- `drug set type intensity <type> <%>` controls one effect family's share of that potency.
- `drug set metabolism <%>` controls how fast active grams clear.

Lower metabolism lasts longer. Higher metabolism clears faster.

Most dose wrappers ask for grams. A drug with high intensity, high type intensity, and high grams will be strong. A drug with low metabolism will stay active longer.

## Drug Liquids And Tinctures
There is no separate tincture class. A tincture is usually an ingested drug delivered through a liquid.

Recommended pattern:

1. Create an ingested drug such as `Foxglove Tincture`.
2. Create or clone a liquid for the tincture.
3. Set taste, smell, water/alcohol values, and draught prog if needed.
4. Set the drug and dose ratio on the liquid.
5. Put the liquid into a bottle, vial, cup, syringe, IV bag, or craft output as appropriate.

Typical material commands:

```text
liquid edit <liquid>
liquid set drug Foxglove Tincture
liquid set drugvolume 5%
liquid set taste 2 bitter metallic
liquid set smell 1 sharp herbal
liquid set alcohol 0.20
liquid show
```

`liquid set drugvolume` accepts a ratio-style percentage and displays the resulting payload as grams per litre.

Important caveats:

- Liquid authoring does not check the drug vector.
- Drinking a liquid only doses drugs with `Ingested`.
- Injecting a liquid through living health strategies doses the liquid drug as injected even if the drug definition does not advertise `Injected`.
- Liquid injection consequences are separate from drug effects. Set them deliberately for syringe, IV, and venom liquids.

For a drinkable medicine, make the drug `Ingested`. For an injectable medicine, make the drug `Injected` and set an appropriate liquid injection consequence. For something that can be both swallowed and injected, set both vectors and balance the dose carefully.

## Drug Foods
Prepared food can carry drug doses directly.

Use direct food drug doses when the whole serving has a known payload, such as a medicated cake, poisoned ration, or bitter herbal bolus.

Prepared food also absorbs liquids. If a food absorbs a drug liquid, compatible ingested drug payloads become food drug doses.

Content guidance:

- Use `PreparedFood` drug doses for designed edible medicines or poisons.
- Use liquid absorption for emergent contamination or recipes involving soaking, steeping, glazing, or poisoning food with a liquid.
- Use stale/spoiled drug doses for effects that should only appear after freshness changes.
- Keep the dose per serving explicit. Eating a fraction of the serving applies the same fraction of the drug grams.

## Pills And Capsules
Use `Pill` for a one-use swallowed item. The item is deleted when swallowed.

Prototype commands:

```text
comp edit new pill Pill_Opioid_Analgesic
comp set drug Opioid Analgesic
comp set dose 0.5g
comp set prog clear
```

The chosen drug must support `Ingested`.

Use pills for tablets, capsules, pellets, measured draught lumps, or any simple swallowed medicine where a fixed dose is enough.

## Salves, Poultices, Ointments, And Creams
Use `TopicalCream` for a topical item that is applied to a bodypart.

Prototype commands:

```text
comp edit new topicalcream TopicalCream_Aloe_Burn_Salve
comp set quantity 50g
comp set drug add "Aloe Burn Salve" 0.1 0.75
comp set prog clear
```

The drug must support `Touched`.

The `drug add` numbers are:

- grams of drug per gram of cream
- absorption fraction from `0` to `1`

Runtime dose is:

`amount applied * grams per gram * absorption fraction`

Topical application requires access to the target bodypart. Blocking worn items can prevent application.

## Smokeables
Use `Smokeable` for a held item that must be lit and smoked by the user.

Prototype commands:

```text
comp edit new smokeable Smokeable_Henbane_Smoke
comp set fuel 900
comp set drag 30
comp set drug "Henbane Smoke"
comp set dose 0.05g
comp set playerdesc The bitter herbal smell of henbane clings to this individual.
comp set roomdesc The bitter herbal smell of henbane smoke hangs in the air here.
```

The drug must support `Inhaled`.

Use smokeables for cigarettes, pipes, herbs, fumes inhaled directly from a burning item, and other personal inhaled doses.

## Incense And Fumigation
Use `IncenseBurner` for room-scale or area-scale scent and optional inhaled drug pulses.

Relevant prototype commands:

```text
comp set drug <inhaled drug>
comp set dose <weight>
comp set pulse <seconds>
comp set drugrange <rooms>
comp set range <rooms>
comp set linger <multiplier>
comp set tag <fuel tag>
```

The drug must support `Inhaled`.

Each pulse doses characters in affected cells. Dose falls off by distance as `grams per pulse / (distance + 1)`. Drug dosing range is capped by the scent range.

Use incense for temple smoke, anesthetic vapour rooms, fumigation, ritual narcotics, or poisonous gas-like content when it should be tied to burning fuel and room ambience rather than gas canisters.

## Inhalers And Drug Gases
A gas can carry a drug payload just like a liquid. Inhaler components consume gas and dose the character if the gas drug supports `Inhaled`.

Typical gas setup:

```text
gas edit <gas>
gas set drug Bronchodilator
gas set drugvolume 2%
gas set smell 1 sharp medicinal
gas show
```

Use inhalers when the item should consume a gas supply per puff. Use smokeables when the item itself burns. Use incense burners when the source doses a room or vicinity.

## Syringes, IVs, And Injectable Liquids
Syringes and IV bags deliver liquids through `IHealthStrategy.InjectedLiquid(...)`.

Player command:

```text
inject <item> <target> <bodypart> [<amount>]
```

The target must be helpless, self-injecting, or willing to accept the intervention. The bodypart must be accessible.

For injectable medicine:

1. Give the drug the `Injected` vector.
2. Put the drug on a liquid.
3. Set liquid dose ratio.
4. Set the liquid injection consequence.
5. Put the liquid in a syringe, IV bag, or other `IInject`/infusion item.

Remember that the health strategy also processes the liquid itself. A harmless injected carrier should usually be `Benign`. Blood products and IV fluids should use the relevant blood, hydrating, or blood-volume consequences.

## Venoms And Weapon Poisons
Venom normally combines three pieces:

1. A drug, usually `Injected | Touched`.
2. A liquid carrying that drug.
3. A delivery path such as `EnvenomingAttack`, `dip`, or poison `apply`.

Natural envenoming attacks use `EnvenomingAttack` additional data:

- venom liquid
- maximum quantity
- minimum wound severity

Weapon poison uses liquid coating on melee weapons or ammunition. The liquid must carry a drug with `Touched` or `Injected`.

Player-facing poison commands include:

```text
apply <source> to <weapon> [volume <amount>]
apply <source> to <ammo> [count <number|all>] [volume <amount>]
dip <weapon> in <source>
dip <ammo> in <source> [count <number|all>]
```

Injected weapon poison delivery depends on wound severity, wound nature, and damage-type multipliers. Contact poison is the fallback if no injected delivery succeeds and the drug supports `Touched`.

## Magic Poison And Removal
Magic spell effects can add or remove drug payloads through the spell-effect builders:

```text
effect poison
effect set drug <drug>
effect set vector <vector>
effect set formula <expr>

effect removepoison
effect set drug <drug>
effect set vector <vector>
effect set formula <expr>
```

The poison effect stores an originator object so removal can target the dose it created. Use this route when the delivery is magical, curse-like, or script-managed rather than item-mediated.

## Seeder Patterns
Use `HealthSeeder` as the primary stock drug catalogue reference. Its pattern is:

1. Upsert named drugs by stable name.
2. Rewrite stock-owned intensity rows for those drugs.
3. Add `Pill_`, `TopicalCream_`, and `Smokeable_` component prototypes only for vectors the drug supports.

Use `AnimalSeeder` as the venom reference. Its pattern is:

1. Create a race-specific venom drug.
2. Create a matching liquid with `Drug`, `DrugGramsPerUnitVolume`, taste/smell text, and injection consequence.
3. Attach that liquid to an `EnvenomingAttack`.

Use `ItemSeeder.Rework.AntiquityMedical` as an example of item prototypes that consume the stock drug delivery component names, such as `Pill_Willow_Bark_Tea`, `TopicalCream_Honey_Poultice`, and `Smokeable_Henbane_Smoke`.

## Testing Checklist
Before considering a drug content slice complete:

- `drug show <drug>` lists the intended vectors and effect families.
- Every effect family with required payloads has that payload configured.
- Every delivery wrapper uses a compatible vector.
- Liquid and gas materials have sensible `DrugGramsPerUnitVolume` values.
- Injectable liquids have deliberate injection consequences.
- Food doses are per serving and scale correctly with partial eating.
- Smoke, incense, and inhalers apply grams per drag, pulse, or puff as intended.
- Weapon poison liquids have touched or injected vectors and enough liquid capacity.
- Stock seeder additions are idempotent and update the maintained seeded component catalogue when required.
