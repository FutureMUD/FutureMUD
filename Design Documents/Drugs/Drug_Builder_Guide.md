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
| `Modifier` | Reversible trait, HP, exact resource capacity, offensive combat and defensive combat modifiers | `drug set type modifier <threshold> <trait|none> <trait bonus> <max HP bonus> <resource|none> <capacity bonus> [offence defence]` |
| `Overdose` | Lethal threshold from active grams and a current native trait; optional shared group combines active mass across configured drugs | `drug set type overdose <trait> <grams per trait point> [group]` |
| `Symptoms` | Recurring physical/sensory episodes and persistent symptom stages | `drug set type symptoms <option> ...`; see below |
| `Coagulation` | Hemostatic and anticoagulant bleeding modifiers | `drug set type coagulation <external%> <reopen%> <internal%>` |
| `Respiration` | Breathing-drive, hypoxia, and airway-support modifiers | `drug set type respiration <drive%> <hypoxia%> <airway%>` |
| `NeedRate` | Hunger, thirst, and drunkenness-rate modifiers | `drug set type needrate <hunger%> <thirst%> <drunkenness%> <passive\|active\|both>` |
| `Arousal` | Sleep, wakefulness, pass-out resistance, knockout, and consciousness thresholds | `drug set type arousal <mode\|check\|sleep\|knockout\|thresholds\|stamina> ...` |
| `Dependence` | Persistent exposure, tolerance, withdrawal and reversible exposure-scaled modifiers | `drug set type dependence <exposure\|tolerance\|withdrawal\|affected\|symptoms\|relapse\|capacity\|modifier> ...` |

The required extra setup commands only work after you add a matching effect intensity.

FutureProg can queue a direct native dose with `dosedrug(character, drug, grams, vector)`. The helper accepts positive finite grams and one advertised vector (`Inhaled`, `Ingested`, `Injected`, or `Touched`), and requires a living character's physical current body. It adds a latent dose and runs normal vector absorption, metabolism, dependence, and effects. The helper does not consume an item; a prog that represents eating or using a spice must decrement or delete its source item separately.

## Recurring symptom profiles

Start with `drug set type intensity Symptoms 100%`. Stage zero exists initially. For example:

```text
drug set type symptoms trait Endurance
drug set type symptoms chance 0 1.0 / Max(1, resistance + 1)
drug set type symptoms stamina 0 5
drug set type symptoms echo 0 You break into a feverish sweat.
```

`chance`, `damage`, `pain`, `stun`, `stamina` and `transition` accept `<stage> <formula>`. Inputs are `intensity`, `resistance`, `fullness`, `sleeping`, `unconscious` and `stage`. Chances are 0–1 probabilities per configured trial; damage and resource amounts are per successful episode. `interval <seconds>` sets that trial interval (1–86,400 real seconds; default one). Normal absorption and metabolism determine how long symptoms remain active. Include `intensity` when stronger doses should cause stronger or more frequent episodes.

Use `addstage` to append a stage, `initial <stage>` to select the initial stage, and `next <stage> <destination> ...` for transition destinations. Each destination defaults to weight one; `weight <stage> <destination> <formula>` supplies a custom nonnegative weight. An empty destination list disables transitions. `transition <stage> <formula>` sets their probability. Stages are zero-based. `awake <stage> true|false` gates transitions and episodes. `sleepfatal <stage> true|false` enables death only on a successful episode during sleep; it defaults to false. `damagetype <stage> <type>` selects native damage type, default Cellular. `echo <stage> <text>` appends a private sensory message; `echo <stage> clear` clears messages. `trait none` disables the resistance input. `drug show` displays the profile.

Stage state persists while positive physical drug exposure remains, including temporary neutralisation. Cure or full clearance removes it. Restart preserves the stage without offline episodes. Dose refreshes and cures do not create episodes; the ten-second drug heartbeat processes elapsed trials. Private hallucination text does not create real items, attacks or room events.

Permanent consequences are optional:

```text
drug set type symptoms onset Wisdom "0.001" -1 1
drug set type symptoms perdose true
drug set type symptoms expiry Wisdom "1" -1 0
```

`onset|expiry <trait> <quoted chance formula> <signed permanent amount> [minimum raw value]` sets one consequence for that trait and phase; zero amount removes it. The minimum defaults to zero. `onsetclear` and `expiryclear` clear a phase. Changes modify the owned raw value through normal persistence; they do not incorporate temporary bonuses. Default onset mode runs once on first active exposure. `perdose true` runs onset consequences immediately on each accepted administration, before absorption, so a later cure cannot undo the permanent change. Natural expiry happens only after final metabolism clearance; cure, purge and staff sobriety avoid it. `drug show` displays interval, phase rules and amounts.

FutureProg can run `resistpoison(character, difficulty)` before an exposure. It returns true on a native poison-save pass; it does not administer or remove drugs. The dedicated check defaults to Endurance/Constitution times five without improvement and can be edited through the normal check/formula builders. Existing configured checks are preserved. A custom setting without a recognised attribute must bind its zero default formula explicitly. Use `dosedrug` only on the intended failed-save branch, and only for routes that are supposed to permit resistance.

## Expansion Payload Commands
Add the effect family first:

```text
drug set type intensity Coagulation 70%
```

Then configure its payload.

Coagulation uses multipliers. Values below `100%` reduce bleeding or reopen risk; values above `100%` increase them:

```text
drug set type coagulation 40% 50% 80%
drug set type coagulation 180% 160% 150%
```

Respiration also uses multipliers. `drive` improves or depresses breathing drive, `hypoxia` changes hypoxia damage, and `airway` supports or worsens marginal airway thresholds:

```text
drug set type respiration 120% 75% 160%
drug set type respiration 65% 135% 90%
```

Respiration drugs cannot make an unbreathable environment breathable and cannot replace missing organs, heart function, or effects that stop breathing.

Need-rate drugs use multipliers plus an applicability mode:

```text
drug set type needrate 70% 120% 90% both
drug set type needrate 100% 140% 100% active
```

Arousal drugs are edited one field at a time:

```text
drug set type arousal mode SleepInducing
drug set type arousal mode Sedative
drug set type arousal check -10%
drug set type arousal sleep 0.60
drug set type arousal knockout 1.10
drug set type arousal thresholds 100% 95% 85%
drug set type arousal stamina 85% 110%
```

`mode` toggles a flag. Valid flags are `SleepInducing`, `SleepPreventing`, `PassOutResistance`, `Knockout`, `Stimulant`, and `Sedative`.

Dependence drugs are also edited one field at a time:

```text
drug set type dependence exposure 1.00 0.10
drug set type dependence tolerance 5.00 30%
drug set type dependence withdrawal 2.50 0.18
drug set type dependence affected Analgesic
drug set type dependence affected Respiration
drug set type dependence symptoms -15% 120% 125% 70% 125% 45% 10% 0.20
drug set type dependence relapse 200%
drug set type dependence capacity 80% 70%
drug set type dependence modifier 10 Endurance -1 0 0 0 0 0
drug set type modifier 1 Endurance 1 0 0 0 2 0
drug set type overdose Endurance 0.002
```

The `symptoms` values are check modifier, hunger multiplier, thirst multiplier, stamina regeneration multiplier, stamina cost multiplier, nausea intensity, rage intensity, and sleep-prevention threshold.

Dependence is persistent gameplay state. Use it for drugs where tolerance and withdrawal are intended story or balance consequences, not as a default for every medicine.

Modifier steps use the highest configured threshold at or below the current normalized active intensity or dependence exposure, independently for each trait, HP, resource and combat target. They are discrete steps, not cumulative tiers. Values are additive; author penalties as negative bonuses. `Modifier` effects are temporary and disappear when the dose clears. Dependence modifier steps track persisted exposure and reverse as exposure decays. `capacity` applies multiplicative reductions to maximum HP and all player magic-resource capacities while withdrawal intensity is present. Relapse gain applies when exposure is below its historical peak; the default is `100%`.

Existing effect families can cover other source spice behavior without extra drug types: use `Analgesic` for Thodeliv's pain relief and `StaminaRegen` for Krentakh's immediate stamina recovery.

Overdose checks active grams against `current effective trait value × grams per trait point` on the body's ordinary drug update. With no group, each configured drug is checked separately. Drugs configured with the same case-insensitive group name contribute their active grams together; each member's own trait and grams-per-point threshold is still evaluated, and multiple doses of one drug are counted once as their summed mass. It calls the native character death path. Leave the Overdose payload unset to disable lethal overdose.

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

### Poison consequences and cleansing

Symptom stages can add independently timed health-capacity consequences and native incapacitation policies:

```text
drug set type symptoms maxhp 0 -damage
drug set type symptoms maxhpseconds 0 5400
drug set type symptoms capacityonly 0 true
drug set type symptoms sleepatexhaustion 0 true
drug set type symptoms blockrecovery 0 true
drug set type symptoms unconsciousrecoveryonly 0 true
drug set type symptoms ceasecombat 0 true
drug set type symptoms immune Vampire
```

These are independent settings; enable only the ones appropriate to the drug. `damage` means the same episode's actual rolled damage. Capacity-only modifiers need an HP strategy with `separatecapacitypenalties true` to avoid counting capacity loss as additional current-HP damage. Other strategies preserve their existing modifier rules. HP models can optionally enable `usestun`, configure `maxstun`/`healstun`, and enable `usecellulardamage` independently of heart simulation. Recovery blocking affects natural online/offline stun recovery, including only unconscious recipients when configured; direct healing remains available.

The `removepoison` spell effect accepts ordered `group <quoted drug names or IDs...>`, `firstgroup true` and `aftermath true`. Grouping Skellebain variants makes them one cure family. Default spells still remove all configured matching toxins; ordered mode removes only the first present family. The optional fourth argument of `hasdrugdosage`/`purgedrugdosages` includes independent timed aftereffects. Omitting it leaves them intact. `removeweaponpoison(item)` removes coatings; `remapliquids` optionally accepts `requireOpen` and `requireCompleteMapping`, allowing magic to cleanse closed vessels while retaining unmapped contents. Player application should retain the default restrictive flags.

## Prog-controlled weapon coatings

`setweaponpoison(item, deliveryProgId)` adds a persistent, single-use scripted coating to one uncoated melee weapon or ammunition item. The delivery prog must accept `(character attacker, character victim, item weapon)` and administer its chosen dose itself. `hasweaponpoison(item)` includes both liquid and scripted coatings. A qualifying damaging wound spends the scripted charge before the prog runs, so resisted exposures still spend it. Ordinary native liquid-coating defaults are unchanged. Scripted policies can use an empty carrier mixture, survive save/load/morph, and transfer one charge to a separate projectile rather than duplicating across multi-projectile ammunition. A player-facing prog must validate custody/access; `canmanipulateitem(character,item)` exposes native manual-action/access and action-blocking checks. `actioncheck(character, checkType, difficulty)` runs an existing configured native check and returns check degrees.

The Armageddon worldfile package wires these helpers into `poison <held item> [with <held vial>]`; its full source-specific rules are documented in [the poison mapping](../Seeding/Armageddon_Worldfile_Poison_Effects.md).

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

Use `ItemSeeder.AntiquityMedical` as an example of item prototypes that consume the stock drug delivery component names, such as `Pill_Willow_Bark_Tea`, `TopicalCream_Honey_Poultice`, and `Smokeable_Henbane_Smoke`.

### Administration policy

Drug-wide administration policy is separate from effect types. Defaults retain ordinary proportional dosing. Use `fixeddose <grams>` to configure a positive fixed mass, then `dosebehaviour FixedDose` to use it for each positive administration. `dosebehaviour IgnoreAdditionalExposure` refuses another exposure while any positive active or latent dose of that drug remains. Combine flags with `dosebehaviour FixedDose, IgnoreAdditionalExposure`; use `dosebehaviour Normal` to restore ordinary behaviour. Fixed dosing never turns zero, negative or invalid input into an exposure. `drug show` displays the policy and configured mass. Progs can inspect `dosebehaviour`, `fixeddosegrams` and `fixedadministration`.

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

## Configuring an infection

Create a separate symptom drug using ordinary effect intensities, modifiers and staged Symptoms. It must not have an Infection effect. Set no delivery vectors if it should only be used internally as a symptom profile. Then edit the exposure drug:

```text
drug set type intensity Infection 100%
drug set type infection name Marsh Fever
drug set type infection symptoms "Marsh Fever Symptoms"
drug set type infection recovery FixedDuration
drug set type infection duration 30000
drug set type infection severity 1
drug set type infection symptomscale 1
drug set type infection exposure 1
drug set type infection spread 0.0001
drug set type infection symptomspread true
drug set type infection episode 1
drug set type infection cure true
```

`duration` and `interval` use real seconds. `exposure`, `spread` and `episode` are probabilities from zero to one, not percentages. `symptomspread true` makes successful recurring symptom episodes trigger transmission trials; false uses the configured interval and episode probability. `spread` is the probability per other eligible room occupant during a trial. Duplicate exposures do not restart an existing illness.

Use `recovery Immune` for native immune progression and antibiotic resistance; `difficulty` sets the native virulence difficulty and `virulence` its multiplier. FixedDuration preserves its authored lifetime and does not clear early through immune/antibiotic progression. With `cure true`, existing targeted drug cures also remove the infection identified by this exposure drug. Natural completion may run symptom expiry consequences; explicit cures never do. Infection symptom profiles reuse pharmacological effects without physical doses, dependence or overdose accumulation. Symptoms formulas now also expose `stamina` (native current stamina).

### Involuntary symptom outcomes

Use `drug set type symptoms outcomeadd <stage> <weight> <action>` with `None`, `Sit`, `DropReadiedItems`, `ReleaseGrapple`, `Babble` or `Residue`. One outcome is chosen per successful episode; weights are relative. A `None` outcome with no text retains the stage's random sensory echo. Stages and outcomes use zero-based indexes shown by drug inspection.

Edit with `drug set type symptoms <option> <stage> <outcome> <value>`:

- `outcomeecho`: private text, or `clear`.
- `outcomeemote`: native emote with `$0` as the victim, or `clear`.
- `outcomeweight`: positive relative weight.
- `outcomeseconds`: babbling duration, up to 86,400 real seconds.
- `outcomerelease`: `true`/`false` to also release the victim's grapple.
- `outcomeresidue`: `<material> <kilograms> [clothing wear-location name fragment]`; coats the body and matching worn items.
- `outcomeremove`: remove that outcome (no value).

Babbling does not refresh existing babbling. Grip loss drops wielded equipment through native inventory rules, leaving general inventory intact. Residue uses native washing. Symptom formulas accept `stamina` as well as intensity, resistance, fullness, sleeping, unconscious, stage and damage.
