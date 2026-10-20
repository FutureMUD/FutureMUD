# Armageddon physical profiles: revised evidence and review gate

**Revised 20 October 2026. No profile activation is authorised.** Luke selected the character-creation Elf height and recovered age bands, without invented lifespan limits. Publication remains last. The first proposal contained inherited unit anomalies and missed recoverable PC weight/height/age evidence. This revision supersedes its common 16–74 creation ages, broad BMI envelopes, and Giant consumption/breathing multipliers. The seven-profile roster, cultures and language grants remain settled.

## Recovered PC constraints

Primary source: supplied `codedump.c`, SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`. These are constraints from a historical code edition, not proof of current live-era settings.

| Profile | PC height band: code, inches (cm) | PC weight, approximately kg | Relative ageing factor |
| --- | --- | --- | ---: |
| Human | 62–78 (157.48–198.12) | 60–90 | 1.00 |
| City Elf | **Approved: 74-90 (187.96-228.60)** | 70-90 | 1.16 |
| Desert Elf | **Approved: 74-90 (187.96-228.60)** | 70–90 | 1.16 |
| Dwarf | 50–58 (127.00–147.32) | 80–100 | 2.20 |
| Half-Elf | 70–82 (177.80–208.28) | 60–90 | 1.08 |
| Half-Giant | 125–155 (317.50–393.70) | 750–900 | 1.80 |
| Mul | 60–72 (152.40–182.88), newly recovered | 100–150 | 0.75 |

PC height enforcement is at lines 141257–141319; weight enforcement at 141320–141375. Lines 142056–142062 define a Zalanthan stone as approximately one kilogram and character weight in ten-stone. These are not British stones. Recommend these recovered weight bounds for the next physical manifest, enforced through chargen weight progs in native grams. Use the recovered Mul band. Both Elf profiles use the owner-selected character-creation range, 74-90 inches inclusive (141264-141270), repeated in creation help (141948-141954). The cached 75-96-inch help edition is not selected. Recording the choice here does not install a new distribution or enable a profile. Generated/manual entries must share the same accepted bounds. Distribution means/standard deviations remain authored; no probability distribution was recovered.

NPC generators at 235646/235846 are called during mobile initialization (31563–31564) and conflict with PC ranges (Human `66+2d8`, Elf `78+2d8`). They do not override PC constraints.

## Ages: approved recovered categories, separate creation limits

`get_virtual_age` (81060-81100) computes `floor(years * 100 / denominator)`: Human 100, both Elf profiles 116, Dwarf 220, Half-Elf 108, Half-Giant 180 and Mul 75. These establish relative ageing factors, **not lifespans**. Labels are young, adult, mature, middle-aged, old and ancient (24553-24559).

The actual `age_class` selector (81048-81056) makes virtual ages 1-19 young, 20-29 adult, 30-40 mature, 41-60 middle-aged and 61-80 old. It stops before the 120 entry in `year_limit` (78776), returning index 6 otherwise. Display callers clamp this to index 5, ancient (72063-72066; 151222-151225). Thus **81+ displays ancient; 120 is a sentinel, not a lifespan or death limit**. These whole MUD-year display bands follow from integer scaling:

| Profile | Young | Adult | Mature | Middle-aged | Old | Ancient |
| --- | --- | --- | --- | --- | --- | --- |
| Human | 1-19 | 20-29 | 30-40 | 41-60 | 61-80 | 81+ |
| City Elf | 2-23 | 24-34 | 35-47 | 48-70 | 71-93 | 94+ |
| Desert Elf | 2-23 | 24-34 | 35-47 | 48-70 | 71-93 | 94+ |
| Dwarf | 3-43 | 44-65 | 66-90 | 91-134 | 135-178 | 179+ |
| Half-Elf | 2-21 | 22-32 | 33-44 | 45-65 | 66-87 | 88+ |
| Half-Giant | 2-35 | 36-53 | 54-73 | 74-109 | 110-145 | 146+ |
| Mul | 1-14 | 15-22 | 23-30 | 31-45 | 46-60 | 61+ |

Years truncating to virtual 0 also fall through to index 6 and display ancient: an apparent source edge-case defect, not a recovered infant policy. Numeric `age_effect` uses raw index 6 and subtracts `eco_bonus` before categorising (81116-81128); the display bands do not authorise copying those stat effects. Appearance may use apparent race.

Source creation separately accepts integer ages `floor(0.20 * race.max_age)` through `floor(0.85 * race.max_age)`, inclusive (141380-141394; 142079-142107). `max_age` is read from missing external race data (35534). Neither numeric creation limits nor racial lifespans follow from the scaling factors. Luke's decision does not require recovering lifespan limits; no invented maximum or universal 16-74 window is proposed. Direct official help retrieval was unavailable during this audit; no outside fantasy longevity was substituted.

The bootstrap Human lifecycle thresholds 3/10/16/25/50/75 are existing native defaults. The previous proposal to scale them across races is withdrawn: it was authored and does not implement the approved recovered categories. Native lifecycle translation must distinguish these six display categories from native childhood/death mechanics; no new death limit, infant policy or activation is approved by the age decision. This documentation update does not alter runtime lifecycle fields.

The [native lifecycle mapping decision](Armageddon_Age_Lifecycle_Mapping_20261020.md) records the exact remaining semantic gate: native juvenile stages and four adult stages cannot represent the six recovered descriptions directly, stock chargen progs derive playable limits from lifecycle fields, and native lookup currently gives a shared boundary to the earlier category. It proposes a coarse mechanical mapping while preserving all descriptive bands separately, without silently equating Young with adulthood or choosing new juvenile/playable limits. No native translation is installed pending that decision.

## Attribute and size proposals still requiring review

The `arm` attribute set supplies Strength/Endurance/Agility/Wisdom. The draft native compression below remains authored: four raw attributes, `3d6+1`, raw cap20 and total budget48; effective ceilings are raw20 plus bonus. The numerical stat roster in the recovered reference collection is unauthenticated and internally inconsistent. It does not validate these offsets. Desert Elf stamina parity also cannot be claimed from its Endurance adjustment alone.

| Profile | STR / END / AGI / WIS bonuses | Standing / sitting / prone size | Bodypart size modifier |
| --- | --- | --- | ---: |
| Human | 0 / 0 / 0 / 0 | Normal / Small / VerySmall | 0 |
| City Elf | −2 / −2 / +3 / +1 | Large / Normal / VerySmall | 0 |
| Desert Elf | −2 / −1 / +3 / +1 | Large / Normal / VerySmall | 0 |
| Dwarf | +2 / +3 / −2 / −3 | Small / VerySmall / Tiny | −1 |
| Half-Elf | −1 / −1 / +1 / +1 | Normal / Small / VerySmall | 0 |
| Half-Giant | +10 / +5 / −4 / −5 | VeryLarge / Large / Normal | +2 |
| Mul | +4 / +4 / −1 / 0 | Normal / Small / VerySmall | 0 |

The Giant/Mul extra totals are deliberate proposed advantages behind their existing application gates; no equal-balance claim is made. Recommend shared Organic Humanoid anatomy and health strategy, bodypart-health/pain multipliers1.0, and no added armour or poison/magic immunity. Ordinal sizes, effective health, encumbrance and body fit require focused qualification at the corrected weights before activation. Human blood-volume formula inherited by Humanoid descendants (HumanSeeder:445–454) extrapolates beyond its Human calibration for Half-Giants.

## Physiology audit: baseline, proposal error and runtime contract

| Field | Existing Human baseline copied into inactive scaffolding | Status |
| --- | --- | --- |
| Breathing | Race expression 7; `simple` lung strategy; Air multiplier 1 | Existing advertised L/min versus per-tick native-volume contract defect. Withdraw proposed Giant 28. Saved strategy key is `simple`, not `lung`. |
| Breath holding | `90 + 5 * Endurance`, seconds | Inherited baseline, not recovered racial physiology. |
| Sweat | 0.8 litres per real minute before exertion scaling | Label and minute consumer agree. Earlier endorsement and Giant 2.4 amplification were erroneous proposals, now withdrawn. |
| Hunger / thirst | 1.0 counter hours per game hour | Inherited baseline; no source justification for Giant 3.0. Capacity and consumption rate are separate. |
| Satiation | 16 food / 8 drink hours | Helper derives these from 12/6 fully-fed cadence; inherited baseline. |
| Temperature | 0-40 degrees C | Inherited baseline; no reviewed desert adaptation or immunity. |
| Lift / drag | STR x10000 / x40000 native grams | Inherited baseline; proposed Giant x20000/x80000 remains unqualified. |
| Track intensity/ability | Visual/olfactory intensity 1/1; ability 1/0 | Inherited baseline; proposed Giant 1.5 intensity remains unqualified. |

### Sweat: erroneous proposal, not a minute-label defect

**20 October correction integrated locally:** the approved authored defaults are now0.02 L/min for Human, common Elf parent, both Elf profiles, Dwarf and Half-Elf; Half-Giant0.10; Mul0.03. Their Heavy/VeryHeavy/ExtremelyHeavy hourly values are respectively0.48/0.84/1.08,2.40/4.20/5.40 and0.72/1.26/1.62. See [the sourced roster and adaptation limits](../Seeding/Armageddon_Sweat_Rates_Integration.md) and [252-row common inventory](../Seeding/Seeded_Sweat_Rates.tsv). These are game adaptations, not recovered biological measurements. Owned three-way reconciliation preserves custom rate/liquid/availability values, including zero. All incomplete profiles remain disabled. The old values below describe the historical defect and original acceptance source, not the corrected new defaults.

HumanSeeder sets `SweatRateInLitresPerMinute=0.8` on Organic Humanoid, Organic Animal and Human (1858/1917/1981). The builder parses a volume in one minute and converts native fluid units to litres (`RaceBuilding.cs:2208-2224`). `StaminaMinuteHeartbeat` subtracts the field times exertion multiplier from water needs when active, alive and above Normal long-term exertion (`BodyMovement.cs:369-395`). Heartbeats use real elapsed seconds (`HeartbeatManager.cs:22,563`), not game-clock hours. Starting stamina ticks also immediately invokes the callback (`BodyMovement.cs:265-267`); steady-state hourly figures below exclude that possible initial extra withdrawal.

| Long-term exertion | Multiplier | Existing 0.8: L/min | Existing 0.8: L/real hour | Withdrawn Giant 2.4: L/real hour |
| --- | ---: | ---: | ---: | ---: |
| Heavy | 0.4 | 0.32 | 19.2 | 57.6 |
| VeryHeavy | 0.7 | 0.56 | 33.6 | 100.8 |
| ExtremelyHeavy | 0.9 | 0.72 | 43.2 | 129.6 |

The later clothing factor 0.5 occurs after `WaterLitres=-sweatTotal` and does not halve water loss. Fixed thirst-point loss (`-5/12 * multiplier`) is separate. The proposal mistakenly endorsed an inherited minute-scale magnitude and then increased it. There is no evidence here that the minute callback runs 60 times too fast. For dimensional illustration only, nominal 0.8L/hour requires field 0.8/60=0.013333L/min, yielding 0.32/0.56/0.72L/hour after exertion scaling; nominal 2.4L/hour requires 0.04L/min. **These examples are not approved tuning targets.** No sweat runtime or global balance change is included.

### Breathing: existing advertised-rate/consumer contract defect

**20 October correction integrated locally:** separate source commit `4ce29a8f5a0f7687247c28db6947ee2febac8a81` now makes consumption elapsed L/min with explicit native-fluid conversion, no startup charge, and excluded paused time. Expression7 now means7 L/min from a sufficient finite supply at the default fluid scale. Existing expressions and rate data are unchanged. The audit below describes the historical implementation. Combined affected tests remain pending; this is not native gameplay qualification. Insufficient tanks still use whole-interval withdrawal; pending fractions are memory-only and rate/equipment are sampled at callbacks.

The builder advertises **L/minute** (`RaceBuilding.cs:2286-2303`). `Race.BreathingRate` evaluates the expression times the fluid-specific rate (`Race.cs:1925-1930`), with no tick-duration or unit conversion. `DoBreathing` runs every 10 real seconds, also immediately on startup (`BodyMovement.cs:258-260,365`). `LungBreather.cs:77-78` passes the result unchanged to the source, and the rebreather forwards it (`RebreatherGameItemComponent.cs:318-325`). The gas container subtracts it from `GasVolumeAtOneAtmosphere` (`GasContainerGameItemComponent.cs:332-342`), stored in native fluid base units as shown by its builder (`GasContainerGameItemComponentProto.cs:149-169`). Filters budget the same supplied volume (`BreathingFilterGameItemComponent.cs:97-102`).

At default `BaseFluidUOMToLitres=1` (`DefaultStaticSettings.cs:706`) and Air multiplier 1, explicit Human expression 7 consumes 7L/tick x6 ticks/minute=**42L/minute**, or 2520L/hour, from a finite supply. Withdrawn Giant 28 would consume 168L/minute, or 10080L/hour. Nondefault unit scales change those litre totals. This establishes finite-tank/filter consumption, not depletion of ordinary ambient air. HumanSeeder's separate global `DefaultBreatheVolumeExpression` is `max(2,7-(0.15*con:<healthTraitId>))` (269-270), but its explicit Race expressions are 7 (1861/1920/1984); changing only that global fallback would not correct existing explicit rows.

This is a pre-existing rate-contract defect. Evidence alone does not determine whether historic intended balance was the label or the consumer. Recommended direction is a separate narrow decision: choose the canonical rate unit, make tick duration and native-fluid conversion explicit, update builder text with that contract, and verify finite-tank/filter duration while reviewing preservation of existing world values. If L/minute is selected, per-tick native volume is `rate * (10/60) / BaseFluidToLitres` (7 becomes 1.166667 units/tick at default scale). **No automatic global division, migration or runtime redesign is authorised or implemented here.**

### What reached a game

New Giant sweat 2.4, breathing 28 and consumption x3 were proposal text only: **never installed or enabled**. The heritage installer copies complete Human baseline fields into the new scaffolds and seeds all seven profiles plus common Elf with `AlwaysFalse` availability on a fresh install (`ArmageddonHeritage.cs:129-145,187,205-219`). Thus inherited 0.8/7 values exist on unavailable scaffold rows. Those values predate this work in shared Human data and were present on temporary qualification characters in owned disposable test databases. This change did not modify existing Human/Admin records, publish or activate an existing world. No live-world database audit was performed, so this does not establish that earlier worlds lack the stock anomaly. Profile activation remains gated.

Source hunger/thirst logic (79575–79654) includes Desert Elf environmental reductions but no universal Giant consumption ×3. Capacity is `5 * get_char_size` (234559–234627), where size is `floor((weight_in_tenstone + height_inches)/7.4)` (235633–235642). Stamina includes DesertElf+60/Giant+20/Mul+20 (81350–81364); City Elf's separate bonus was removed to avoid double counting movement reductions (81340–81348). These justify later explicit adaptations and tests, not an invented blanket multiplier.

## Remaining owner choices and activation boundary

Use recovered PC weight/Mul bounds; retain free-entry names, shared anatomy and zero added immunities. Use approved 74-90-inch Elf creation heights and recovered age bands; do not invent lifespan limits. Remaining review covers authored attribute/size compression and gameplay qualification, translation of recovered categories to native lifecycle mechanics without invented death/infant rules, and physiological tuning after the breathing-unit contract and needs cadence are measured. Missing lifespan values do not authorise invented maxima. The prior native age-scaling fallback and racial physiology amplifications remain withdrawn.

All seven owned profile rows and the common Elf parent remain unavailable. Desert Elf/Half-Giant/Mul additionally retain their explicit application-policy gate. Nonphysical language/culture/ethnicity/naming composition can proceed around this boundary. No existing Human/Admin records, existing world or player values are changed by this proposal.
