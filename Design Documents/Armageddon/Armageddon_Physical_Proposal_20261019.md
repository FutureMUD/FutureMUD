# Armageddon physical profiles: revised evidence and review gate

**Current 21 October status:** the complete proposed physical/lifecycle/appearance policy is approved, implemented and qualified. The historical proposal and corrections below preserve their provenance; the final instruction and [evidence ledger](Armageddon_Setting_Implementation_20261019.md) supersede pending-decision text. Race admission and publication remain held.

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

PC height enforcement is at lines 141257–141319; weight enforcement at 141320–141375. Lines 142056–142062 define a Zalanthan stone as approximately one kilogram and character weight in ten-stone. These are not British stones. Recommend these recovered weight bounds for the next physical manifest, enforced through chargen weight progs in native grams. Use the recovered Mul band. Both Elf profiles use the owner-selected character-creation range, 74-90 inches inclusive (141264-141270), repeated in creation help (141948-141954). The cached 75-96-inch help edition is not selected. Recording the choice here does not install a new distribution or enable a profile. Generated/manual entries must share the same accepted bounds. Distribution means/standard deviations remain authored; no probability distribution was recovered. On21 October Luke locked the height/weight bounds and explicitly waived tests of these established ranges. This is not a posture-size or physiology approval.

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

The [native lifecycle mapping decision](Armageddon_Age_Lifecycle_Mapping_20261020.md) records the adult mapping approved by Luke on21 October and the remaining juvenile/chargen policy gate: native juvenile stages and four adult stages cannot represent the six recovered descriptions directly, stock chargen progs derive playable limits from lifecycle fields, and native lookup currently gives a shared boundary to the earlier category. The approved coarse mechanical mapping preserves all descriptive bands separately, without silently equating Young with adulthood or choosing new juvenile/playable limits. Implementation and the native category-boundary repair remain pending; no native translation is installed.

## Approved attributes and posture-size investigation

The `arm` attribute set supplies Strength/Endurance/Agility/Wisdom. Luke approved the authored native compression on21 October: four raw attributes, `3d6+1`, raw cap20 and total budget48; effective ceilings are raw20 plus bonus. The numerical stat roster in the recovered reference collection is unauthenticated and internally inconsistent. It does not validate these offsets. Desert Elf stamina parity also cannot be claimed from its Endurance adjustment alone.

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

HumanSeeder sets `SweatRateInLitresPerMinute=0.8` on Humanoid, Organic Humanoid and Human (1858/1917/1981). The builder parses a volume in one minute and converts native fluid units to litres (`RaceBuilding.cs:2208-2224`). `StaminaMinuteHeartbeat` subtracts the field times exertion multiplier from water needs when active, alive and above Normal long-term exertion (`BodyMovement.cs:369-395`). Heartbeats use real elapsed seconds (`HeartbeatManager.cs:22,563`), not game-clock hours. Starting stamina ticks also immediately invokes the callback (`BodyMovement.cs:265-267`); steady-state hourly figures below exclude that possible initial extra withdrawal.

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

Use recovered PC weight/Mul bounds; retain free-entry names, shared anatomy and zero added immunities. Use approved 74-90-inch Elf creation heights and recovered age bands; do not invent lifespan limits. Remaining review covers authored attribute/size compression and gameplay qualification, implementation of the approved adult lifecycle mapping, remaining juvenile/chargen policy without invented death/infant rules, and physiological tuning after the breathing-unit contract and needs cadence are measured. Missing lifespan values do not authorise invented maxima. The prior native age-scaling fallback and racial physiology amplifications remain withdrawn.

All seven owned profile rows and the common Elf parent remain unavailable. Desert Elf/Half-Giant/Mul additionally retain their explicit application-policy gate. Nonphysical language/culture/ethnicity/naming composition can proceed around this boundary. No existing Human/Admin records, existing world or player values are changed by this proposal.

## 21 October decisions and remaining physiology recommendation

Owner approvals: height/weight bounds and attribute offsets above are locked. The four-attribute layout is preferred, not required. Native attribute installation now adapts existing raw body-owned traits by semantic name, retaining stable IDs, parent health flags, approved raw cap20/dice3d6+1 and twelve raw-budget points per selected attribute (48 for Arm). Combined Body/Physique receives rounded mean STR/END, Split physical subattributes map independently, and raw Intelligence supplies WIS only when raw Wisdom is absent. Derived traits are not adjusted again; other traits remain neutral. No duplicate traits are created. Builder names, cap/bonus/dice/health choices and removed links are preserved. Initial adoption also preserves any pre-existing custom racial attribute links. This slice does not activate races or certify a native database installation.

Luke also approved the prior chat's item5: conservative source-supported racial appearance and ordinary creation minima at mapped adulthood (Human20; both Elves24; Dwarf44; Half-Elf22; Half-Giant36; Mul15), with younger exceptions governed explicitly. Numeric upper creation bounds and unrecovered juvenile subdivisions were not supplied by that approval. No new lifespan is inferred.

Posture investigation found stock Human/Humanoid/Organic Humanoid and the other explicit seeded race families use the same category for standing/sitting/prone. Native fields can differ, but their contextual selection affects exits, grapples/impacts and explosions, not only appearance. Recommend the owner's bounded two-category option: Human/Half-Elf/Mul Normal/Small/Small; both Elves Large/Normal/Normal; Dwarf Small/VerySmall/VerySmall; Half-Giant VeryLarge/Large/Large. Keep the proposed bodypart modifiers separate. This selects the bounded two-category option permitted by Luke's instruction after investigating stock practice; his subsequent approval except hunger/thirst confirms it. It is not yet installed and does not change shared stock. No three-category baseline update is recommended.

Original physiology candidate (subsequent21 October approval covers non-hunger/thirst items below; implementation remains pending): shared Organic Humanoid anatomy/health, health and pain multipliers1.0, no added armour/resistance/immunity; native carrying formulas driven by the approved strength bonuses, with no extra Giant lift/drag multiplier; existing tracking defaults; corrected L/min breathing contract with existing7L/min baseline and ordinary breath-holding formula; already-approved sweat defaults retained. Use ordinary hunger/thirst counter rates1.0. Propose Half-Giant food/water storage3x ordinary capacity (48/24 satiated hours against current16/8 baseline), without a matching3x consumption rate; this is an authored midpoint adaptation of source body-dimension capacity, not its exact per-character formula. Other profiles retain ordinary storage initially.

For stamina, propose explicit additive capacity60 for Desert Elf,20 for Half-Giant and20 for Mul, with ordinary recovery and no extra City Elf capacity. Those amounts reproduce source bonus numbers in native units as an authored first adaptation, not proof of equal balance. Keep capacity, recovery and movement cost distinct. For Desert Elf hydration, recommend environment-conditioned terrain/shelter adjustments rather than a blanket0.5 thirst rate. Recovered source halves terrain penalties, doubles shelter credits and clamps final condition loss differently; its shared base/time terms are not halved. Retain heat-stress mechanics rather than grant heat immunity. Half-Giant blood-volume extrapolation remains an implementation audit item, without an arbitrary racial health multiplier.

Recovered physiology evidence: original codedump.c at79575-79654 (thirst conditions),81296-81383 (capacity bonuses),234559-234622 (food/water limits),235633-235642 (dimension-derived size). File SHA-256 remains e44778f6a5fb9fd02db5c3ff78d4c468b93ae6c854a058d5af769dc0a95cd59f. Source capacity is max(10,5*floor((weight_in_tenstone+height_inches)/7.4)); native ordinal posture size is not this variable. Implementation should preserve source conditions where the existing native needs model supports them and identify any required narrow extension before claiming fidelity.
## 21 October racial-merit exploration and physiology approval

Luke approved the preceding proposal except hunger/thirst, asked for faster Giant consumption in addition to3x capacity, and requested exploration of racial merits (including a new type if needed) for Desert Elf hydration and other adaptations. Approved non-hunger/thirst data remains shared Organic Humanoid anatomy/health; health/pain1.0; no added armour/resistance/immunity; stamina capacity+60 Desert Elf/+20 Half-Giant/+20 Mul with ordinary recovery and no City Elf addition; corrected baseline breathing7L/min/ordinary breath holding; approved sweat rates; ordinary carrying expressions/tracking and0-40C temperature range. The two-category posture table is also confirmed. None of this exploration changes runtime source, an existing-world database or availability.

Giant recommendation: use existing executable Needs Rate Change merit with hunger3/thirst3/alcohol1, retaining underlying Race hunger/thirst rates1 to avoid multiplying twice. Pair with3x satiation capacity. This keeps ordinary satiation-clock duration approximately unchanged while requiring3x incoming food/drink satiation points. It does not establish equal whole-body dehydration time: native Race.ThirstRate changes the drink counter only, while merit thirst also modifies basal WaterLitres loss; sweat and other explicit water losses are separate. WaterLitres capacity is blood-volume based. A source search found no tracked production assignment to Body.WaterLossLitresPerHour, so that needs a narrow audit before claiming physical hydration qualification. No rate is installed by this recommendation.

Merit architecture: there is no native race/ethnicity-to-merit relation today. Recommend generic intrinsic race-merit links, inherited and deduplicated by identity, contributing body-scoped effective merits dynamically from the active body's race. Keep these separate from explicit PerceiverMerits, selectable chargen quirks and copied form templates; no hidden roles, login regranting or membership system. Preserve builder grant additions/removals/merit definitions through typed ownership. Audit existing consumers and form-template copying: Character.Merits already includes Body.Merits, while temperature/bodypart-health consumers concatenate body merits again. Address relevant duplicate aggregation before claiming new body-scoped uses.

Desert Elf: existing Needs Rate Change plus boolean applicability supports crude fixed environmental tiers, but cannot compute a contextual numeric modifier. Recommend a generic contextual/environmental needs merit backed by character-aware numeric FutureProg. For the recovered terrain/shelter pattern, introduce an opt-in environmental thirst-loss component (native passive needs currently has none), modifying only that component while leaving shared base/time terms distinct. Ordinary worlds retain current behaviour without the opt-in. Use existing room/terrain/tag/environment data, not hard-coded racial names. A simpler overall-rate contextual approximation is possible, but must not be called the exact source mechanic. Shelter may reduce loss to zero, never create drinking water. Keep sweat separate and preserve positive drink quantities.

Other candidates: new generic Stamina Capacity merit for the approved flat capacity additions, consumed centrally by MaximumStaminaFor and clamped without refilling current stamina on changes; existing speed/swimming/flying merits only if corresponding changes are approved; needs capacity can remain native Race fields for v1, with a separate generic capacity merit only if independently useful. Movement Stamina Multiplier is registered but currently has no runtime interface consumer, so it is not executable reusable walking-endurance support yet. Temperature has an interface/consumer but no concrete implementation found; neutral approved temperature data needs no merit. Keep morphology, raw stat bonuses, health baseline, sweat, breathing and tracking as Race data, avoiding duplicate or no-op merits.

Evidence inspected: Needs Rate Change and INeedRateChangingMerit; ActiveNeedsModel48-82; ChangingNeedsModelBase14-17/53-79/146; CharacterMeritBase applicability; CharacterTemplateMerits11-51; CharacterTraits126; CharacterForms134; BodyBiology2109-2111; BodyParts917-920; Character.MaximumStaminaFor1969-1971. Exploration was read-only with respect to source and databases; no new tests or gameplay certificate. Exact multiplier definitions, environmental component wiring, racial links and new merit types remain implementation proposals rather than completed functionality.

## Approved racial-merit implementation,21 October

Luke subsequently instructed implementation of the racial merits, the found fixes and the proposed merit types. The resulting [generic intrinsic-merit contract](../Characters/Intrinsic_Racial_Merits.md) supersedes the read-only proposal status above: body-scoped inherited race links, separate explicit assignments, Stamina Capacity and Environmental Needs types, executable generic movement-cost support, and duplicate-consumer/form-copy repairs are implemented. The Setting grants approved+60/+20/+20 stamina, Giant hunger3/thirst3/alcohol1 with48/24 storage and base rates1, and separate ordinary/Desert Elf environmental thirst definitions. Terrain IDs and builder changes are retained through ownership reconciliation.

The environmental progs preserve recovered severe/moderate penalties, separate sector/indoors credits, below-standing credit and total loss clamps using native conditions. Mapped Core terrain expansions are authored. Native Morning/Afternoon receives+1; no native Highsun band exists, so an invented noon interval is not installed. Stock water-loss prog remains zero because source condition points provide no litre conversion and the existing basal-water property has no tracked production assignment. Generic numeric water-loss support is executable for explicitly configured worlds; this does not establish source-exact hydration or whole-body dehydration balance. Sweat, drinking and physical water withdrawals remain separate.

The implementation ledger records verification separately. Height/weight ranges were not tested, no incomplete race was enabled, and full remaining physical/lifecycle wiring and activation are separate work. All changes remain local; no existing-world database or publication action is included.


## Final implementation instruction, 21 October

Luke approved the remaining juvenile and independent creation-age policy recorded in the lifecycle note and authorised implementation. All prior physical, posture, attribute, appearance and physiology decisions remain settled. Physical/lifecycle/appearance wiring is implemented and qualified; earlier pending-decision statements above are historical. The two-category posture table supersedes the original three-category proposal. Admission and publication remain held. The approved profile height/weight ranges were not range-tested; native body fixtures use fixed representative model means.

Seven neutral authored physical models, seven age describers, six preserving creation-bound wrappers, five conservative authored shape values and seven appearance profiles are installed. Native bodies retain their identity, birthday, attributes, appearance, needs, partial blood and stamina through two cold readers. Human blood-volume extrapolation for Giants remains an adaptation rather than a biological balance claim. Shared profile records remain intact; their dynamic All profiles include newly defined shape values by normal engine semantics, so the available pools/random distributions are not claimed unchanged or ancestry-exclusive. The narrowed Elf/frame and hairless profiles supply their explicit defaults, with ordinary styling unable to regrow hair in the hairless profiles.
