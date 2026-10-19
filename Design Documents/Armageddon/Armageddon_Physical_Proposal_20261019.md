# Armageddon physical profiles: revised evidence and review gate

**Revised 19 October 2026. No profile activation is authorised.** The first proposal contained inherited unit anomalies and missed recoverable PC weight/height/age evidence. This revision supersedes its common 16–74 creation ages, broad BMI envelopes, and Giant consumption/breathing multipliers. The seven-profile roster, cultures and language grants remain settled.

## Recovered PC constraints

Primary source: supplied `codedump.c`, SHA-256 `e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f`. These are constraints from a historical code edition, not proof of current live-era settings.

| Profile | PC height band: code, inches (cm) | PC weight, approximately kg | Relative ageing factor |
| --- | --- | --- | ---: |
| Human | 62–78 (157.48–198.12) | 60–90 | 1.00 |
| City Elf | 74–90 (187.96–228.60); cached help instead 75–96 (190.50–243.84) | 70–90 | 1.16 |
| Desert Elf | Same code/help conflict as City Elf | 70–90 | 1.16 |
| Dwarf | 50–58 (127.00–147.32) | 80–100 | 2.20 |
| Half-Elf | 70–82 (177.80–208.28) | 60–90 | 1.08 |
| Half-Giant | 125–155 (317.50–393.70) | 750–900 | 1.80 |
| Mul | 60–72 (152.40–182.88), newly recovered | 100–150 | 0.75 |

PC height enforcement is at lines 141257–141319; weight enforcement at 141320–141375. Lines 142056–142062 define a Zalanthan stone as approximately one kilogram and character weight in ten-stone. These are not British stones. Recommend these recovered weight bounds for the next physical manifest, enforced through chargen weight progs in native grams. Use the recovered Mul band. Retain the explicitly disclosed Elf height edition choice for owner review; do not silently replace the cached help-based constraint. Generated/manual entries must share the same accepted bounds. Distribution means/standard deviations remain authored; no probability distribution was recovered.

NPC generators at 235646/235846 are called during mobile initialization (31563–31564) and conflict with PC ranges (Human `66+2d8`, Elf `78+2d8`). They do not override PC constraints.

## Ages: recovered scaling, missing absolute limits

`get_virtual_age` (81060–81100) computes `floor(years * 100 / denominator)`, with denominators Human100, Elf116, Dwarf220, Half-Elf108, Half-Giant180 and Mul75. These establish the relative factors above, **not verified lifespans**. Source age-category boundaries are virtual ages 0/19/29/40/60/80/120 (78776), with labels at 24553–24560 that do not map exactly to native lifecycle stages.

Source creation permits `floor(0.20 * race.max_age)` through `floor(0.85 * race.max_age)` (141381–141382; 142079–142080). `max_age` is read from missing external race data (35534). Neither a universal 16–74 window nor numeric racial lifespans are recovered. Direct official help retrieval was unavailable during this audit; no outside fantasy longevity was substituted.

The bootstrap Human stages 3/10/16/25/50/75 are existing native defaults, not deliberate recommendations for all races. If an authored translation is needed, scaling those defaults upward gives Human3/10/16/25/50/75; Elf4/12/19/29/58/87; Dwarf7/22/36/55/110/165; Half-Elf4/11/18/27/54/81; Half-Giant6/18/29/45/90/135; Mul3/8/12/19/38/57. This is a disclosed fallback candidate, not a recovered age policy or an independently justified creation window. Keep activation gated until the absolute age/lifecycle translation is accepted or better source data is recovered.

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

## Physiology audit: units and carried defaults

| Field | Existing Human baseline carried as inactive scaffolding | Evidence / recommendation status |
| --- | --- | --- |
| Breathing | Expression `7`; native lung strategy/Air | Builder says L/min, consumer uses one expression volume every10 real seconds. At default1 native volume unit/L, a supply loses42L/min. **Existing contract mismatch; not a recommendation of7L/min.** Withdraw Giant28 (would consume168L/min). |
| Breath holding | `90 + 5 * Endurance`, seconds | Native expression unit is seconds; inherited baseline, not recovered racial physiology. |
| Sweat | 0.8 **L per real minute**, before exertion scaling | Inherited anomaly; withdraw recommendation of this rate and Giant2.4. No hidden `/60`. |
| Hunger / thirst | 1.0 counter hours per game hour | Inherited native defaults; source does not justify Giant3.0. Food/water capacity and consumption rate are separate. |
| Satiation | 16 food / 8 drink hours | Helper derives these ceilings from12/6 fully-fed cadence; inherited baseline. |
| Temperature | 0–40°C; Air multiplier1 | Inherited baseline; no recovered immunity or reviewed desert adaptation. |
| Lift / drag | STR ×10000 / ×40000 native grams | Inherited baseline. Proposed Giant ×20000/×80000 was authored and remains unqualified. |
| Track intensity/ability | Visual/olfactory intensity1/1; ability1/0 | Inherited baseline; Giant1.5 intensity was authored and unqualified. |

Sweat is stored as litres/minute (`RaceBuilding.cs:2208–2224`) and withdrawn once each real-minute heartbeat (`BodyMovement.cs:265,369,377–395`; `HeartbeatManager.cs:22,563`). Heavy/VeryHeavy/ExtremelyHeavy multipliers0.4/0.7/0.9 make0.8 remove19.2/33.6/43.2L per real hour;2.4 would remove57.6/100.8/129.6. The later clothing factor0.5 does not reduce water withdrawal. A nominal0.8L/hour would require field0.013333…L/min (0.32/0.56/0.72L/hour after those multipliers); Giant2.4L/hour would require0.04. These are transparent authored tuning candidates, **not approved or source-backed physiology recommendations**. Fixed thirst-point loss is separate, so native gameplay must be measured.

Breathing evidence: builder label `RaceBuilding.cs:2303`; ten-second callback `BodyMovement.cs:258,365`; expression `Race.cs:1925–1930`; unchanged volume through `LungBreather.cs:77–78`, `RebreatherGameItemComponent.cs:318–325`, `GasContainerGameItemComponent.cs:340`, and `BreathingFilterGameItemComponent.cs:97–102`; default fluid conversion `DefaultStaticSettings.cs:706`. Preserve bootstrap data. Resolve the existing consumer contract explicitly before treating racial breathing tuning as qualified; do not hide it in a Giant multiplier.

Source hunger/thirst logic (79575–79654) includes Desert Elf environmental reductions but no universal Giant consumption ×3. Capacity is `5 * get_char_size` (234559–234627), where size is `floor((weight_in_tenstone + height_inches)/7.4)` (235633–235642). Stamina includes DesertElf+60/Giant+20/Mul+20 (81350–81364); City Elf's separate bonus was removed to avoid double counting movement reductions (81340–81348). These justify later explicit adaptations and tests, not an invented blanket multiplier.

## Remaining owner choices and activation boundary

Use recovered PC weight/Mul bounds; retain free-entry names, shared anatomy and zero added immunities. The genuine unresolved choices are the Elf height edition; authored attribute/size compression; absolute age/lifecycle translation while `max_age` is missing; and physiological tuning after the breathing-unit contract and needs cadence are measured. Those choices should be presented with the evidence above, not as approval of arbitrary universal defaults.

All seven owned profile rows and the common Elf parent remain unavailable. Desert Elf/Half-Giant/Mul additionally retain their explicit application-policy gate. Nonphysical language/culture/ethnicity/naming composition can proceed around this boundary. No existing Human/Admin records, existing world or player values are changed by this proposal.
