# Seeded environmental exposure catalogue

This audit distinguishes qualitative material compatibility, authored game rates and gameplay calibration. The numerical rates below are game design choices; they are not medical thresholds, corrosion measurements or handling guidance.

## Reproduce the catalogue

The Debug-only capture runs the actual seeders against a newly named, owned disposable MySQL database. It imports the checked-in blank snapshot and enables the same EF lazy-loading proxies as the installer. It does not select or mutate an existing game database.

```powershell
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug -m:1 --no-restore
dotnet run --project DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-build --no-restore -- --capture-environmental-exposure --native
```

The capture uses the industrial-neutral replay's engine, anatomy, animal, robot, supernatural and workflow prerequisites, Antiquity cultural heritage, every available pre-industrial item era, modern UsefulSeeder equipment, and both optional exposure packs. The Industrial ItemSeeder currently refuses installation because its existing production-readiness gate rejects 464 food concepts. The capture does not bypass that guard. That package creates no unique persisted material, liquid or gas definitions; modern supplied-gas components are selected independently through UsefulSeeder. This exclusion therefore limits item installation evidence, rather than silently omitting an additional live fluid catalogue.

The resulting [JSON catalogue and matrix](Environmental_Exposure_Catalogue.native.json) and [CSV catalogue](Environmental_Exposure_Catalogue.native.csv) contain every material, liquid and gas row installed by that union. IDs are database-local stable row identities; seed ownership keys and stable reaction GUIDs distinguish authored identity from coincidental name matches. The JSON records the fixture database, replay profile, completed seeders and observed creating/modifying seeders. The CSV retains full versioned reaction XML, including consumption mode/rate, explicit exclusions and legacy coefficients. Rows also include aliases, hierarchical tags, material heat thresholds in Celsius, transmission definitions, body/race references, drug IDs and substance source bindings.

The family matrix contains all 17 authored profiles crossed with 20 explicit material dispositions. It describes the seeder's intended family response; individual installed XML remains authoritative after builder customisation. Unrecognised materials remain `unsupported/uncertain` with an explanation. A missing heat threshold means no authored direct-heat response, not immunity. Ordinary beverages, brines, culinary acids, fuels and drug carriers deliberately receive no inferred contact injury.

## Compatibility decisions and limits

| Material group | Review and exception policy |
| --- | --- |
| Living tissue, leather, wool/silk | Keep separate families. Alkaline and acidic reactions can differ; skin is not a proxy for every organic material. Anatomy material references are included in each material row. |
| Plant fibres, wood, paper | Distinguish porous transmission from damage susceptibility. Cloth may pass a fluid while surviving it. |
| Bone, horn, shell, carbonate rock | Bone/horn and carbonate have separate responses. Shell and limestone are not grouped with silicate rock. |
| Silicate rock, glass, ceramic | Hydrofluoric acid and the fantasy glass-eating profile have explicit responses. Other acids can be excluded. Lava does not imply stone immunity. |
| Ferrous, reactive metal, copper alloy, noble metal | Each has a distinct response family. Unrecognised alloy names remain uncertain; arbitrary ore or broad metal membership does not prove compatibility. |
| Resistant polymers, other polymers, elastomers | These are simplified stock families. Product formulation, concentration and temperature can change real compatibility. The stock compatible vessel uses a separately named authored material. |
| Oils, fuels, waxes, food | Existing fire, ingestion and drug behaviour remains independent. A combustible or poisonous substance is not automatically skin-corrosive. |
| Fantasy materials | Unknown existing fantasy materials remain uncertain. The new Accursed material/tag is an explicit setting choice used by sacred water, rather than a blanket rule against flesh. |

The [Parker chemical compatibility reference](https://promo.parker.com/parkerimages/promosite/Performance%20Materials/UNITED%20STATES/aspire/Aspire%20Support%20Layers%20and%20Chemical%20Compatibility.pdf) supports treating polymers and elastomers as substance-specific choices rather than universal barriers. This implementation intentionally simplifies those distinctions into reviewed stock families and explicit per-material exceptions.

The special glass/mineral and tissue hazards of hydrofluoric acid are qualitatively informed by the [CDC/NIOSH HF response card](https://www.cdc.gov/niosh/ershdb/emergencyresponsecard_29750030.html). Chlorine's contact and respiratory routes are informed by the [CDC/NIOSH chlorine card](https://www.cdc.gov/niosh/ershdb/emergencyresponsecard_29750024.html); sulfur dioxide's respiratory example uses the [NIOSH pocket guide](https://www.cdc.gov/niosh/npg/npgd0575.html). These sources do not establish any FutureMUD damage-per-second conversion.

The alkaline example follows the qualitative corrosive distinctions in [Health Canada's sodium hydroxide assessment](https://www.canada.ca/en/health-canada/services/environmental-workplace-health/occupational-health-safety/workplace-hazardous-materials-information-system/hazardous-substance-assessments/sodium-hydroxide.html). Culinary vinegar remains a deliberately safe contact default; the [FDA vinegar guidance](https://www.fda.gov/media/71937/download) is context for that separate food category, not evidence of a universal acid response. Nitrogen remains a negative control for contact injury while oxygen deprivation remains possible; see [OSHA's nitrogen hazard alert](https://www.osha.gov/sites/default/files/publications/OSHA3844.pdf).

## Authored profiles

All figures are full-reference-area, unit-intensity rates before armour, resistance and health-strategy processing. Actual concentration, exposed area, transmission and finite exhaustion reduce them.

The table's gas tissue rates describe **external contact**. Separate tissue inhalation rates are chlorine 18/s, ammonia 9/s, sulfur dioxide 6/s, steam/hot volcanic fumes 18/s (still requiring 70 C), and spectral miasma 21/s. The machine-readable matrix includes `InhalationResponse` separately. These are explicit game rates; they do not alter or bypass natural organ armour. Lower-rate profiles can be completely absorbed by stronger stock anatomies.

| Profile | Tissue damage/s | Distinct behaviour |
| --- | ---: | --- |
| Lava | 12 | Non-consuming; hazardous contamination persists until removed. Other reviewed materials have lower authored rates. |
| Scalding water | 2 | Source/environment temperature must be at least 70 C; ordinary drying applies. |
| Dilute cleaning acid | 0.05 | Mild compared with strong profiles; carbonate reacts more strongly than tissue. |
| Hydrochloric acid | 2 | Carbonate/bone/reactive-metal response differs from ferrous and soft materials. |
| Sulfuric acid | 3 | Separate soft-material, metal and polymer responses. |
| Hydrofluoric acid | 4 | Explicit glass/ceramic and silicate responses. |
| Sodium hydroxide solution | 2 | Alkaline; animal fibres/leather/reactive metals differ from plant fibres and noble metals. |
| Biological acid | 2.5 | Existing animal-skin target retained; original 125 damage / 175 pain per base volume preserved for Legacy mode. |
| Chlorine | 0.6 | External and inhaled injury; no liquid consumption. |
| Ammonia | 0.2 | Tissue irritation and a separate copper-alloy response. |
| Sulfur dioxide | 0.15 | Tissue route example; other reviewed material responses excluded. |
| Steam / hot volcanic fumes | 2 | Minimum 70 C; thermal channel shares external heat with ambient temperature by maximum, not addition. |
| Infernal lava | 18 | Fantasy; persistent, non-consuming; broader material response. |
| Alchemical universal solvent | 4 | Fantasy; consumes reagent; noble metals and resistant polymers excluded. |
| Glass-eating ooze acid | 0 | Fantasy; glass/ceramic receives 5/s, tissue explicitly excluded. |
| Spectral miasma | 1.5 | Fantasy gas injury independent of oxygen compatibility. |
| Sacred water | 0 | Fantasy; only the explicitly authored Accursed category receives 5/s. |

Consuming chemical/holy profiles normally debit 0.5 mL per reference second, with 2 mL/s on the reactive-metal family. Biological acid uses 1 mL/s. No profile turns total mixed volume into reactive-species volume. An exclusion performs no reaction work, damage or consumption.

Human seeder flesh thresholds previously contained the same unexplained 412.0389 value as bone. New stock tissue thresholds are explicitly authored as 55 C, with bone at 120 C. Existing custom thresholds are preserved. Thresholds elsewhere remain authored values or null; melting and ignition temperatures are not substituted. Stock tissue thermal response is 0.2 damage/(s*C) capped at 12/s; other recognised families use 0.02 capped at 4/s. These are direct-injury game rates and are unrelated to comfort-range or physiological hyperthermia settings.

## Protection, equipment and reruns

The natural pack adds mundane-presented burn salve and a respiratory draught; the fantasy pack adds external, vessel and respiratory wards. They use the existing reagent/spell lifecycle. Body protection does not protect inventory, respiratory resistance does not supply oxygen, and a body-wide effect does not imply a simulated coating.

The demonstration equipment includes porous acid-resistant cloth, sealed susceptible rubber and compatible storage polymer. Tunics and vessels include the ordinary destroyable/health components rather than changing every established item into a damageable item. Their component prerequisites are Human/UsefulSeeder content. Modern breathing equipment uses the existing rebreather/supply component contract.

`EnvironmentalExposureSeeder` uses persisted `SeederManagedRecord` identities and per-field last-value reconciliation. Same-option reruns reuse entries and relationships; missing owned entries are repaired. Builder changes, removed authored tags, same-name unowned objects and malformed reaction XML are preserved and reported. Reconciliation messages identify fields needing explicit builder resolution; deleting an ownership record is not a supported blanket refresh operation. Seed installation never changes an existing world's exposure mode or creates hazardous rooms.

Unchanged stock gas rules can be split into external and respiratory routes while retaining the original external identity. An edited combined rule is not overlapped by a new stock respiratory rule. If builders have already edited both rules into an overlap, both edits remain and the seeder reports that equal-priority inhalation stays inactive until the builder resolves it.

## Calibration and verification

The native fixture export currently contains 1,378 solids, 630 liquids and 70 gases (2,078 rows), plus 340 family interactions and all five demonstration equipment definitions. Its dispositions are 733 covered by a family rule, 618 intentionally non-hazardous, 18 hazardous, 64 context-specific and 645 unsupported/uncertain. Unsupported entries are retained in the audit; no reactions were fabricated merely to raise coverage. The successful exposure installation reported no conflicts or deferred entries.

The first replay exposed the liquid reaction column's 64 KiB limit. After the generated widening migration, a fresh full replay successfully imported the final blank snapshot into `futuremud_exposure_269767ee2eb3485db87fad96` and executed all 31 selected seeders. A subsequent exposure-only rerun installed the final respiratory calibration in that same owned database. The final machine files honestly record `ResumedExposureOnly: true`, 31 `BaselineCompletedSeeders`, the latest completed exposure seeder, and complete observed producer provenance merged from the full-install ownership receipt. There are no conflicts or deferred entries. The earlier fixture remains separate for live gameplay testing; neither path changes an established game.

The intended ordering is: short dilute-cleaner contact is much milder than strong acid; persistent lava is immediately dangerous; finite acid eventually stops reacting; compatible storage prevents vessel injury without protecting unrelated contents by fiat; targeted reagent resistance lowers injury without silently reducing chemical consumption. Human and non-human results depend on their selected health strategies, armour and anatomy.

Actual executed checks, fixture identities, observed injury bands and remaining limits are recorded in the [verification report](Environmental_Exposure_Verification.md). Successful catalogue export proves installed data and coverage, not gameplay calibration. Do not infer a validated time-to-incapacitation from this rate table.

For the isolated stock fixture, the short-exposure acceptance bands are 5–15 aggregate damage for the acid-pool sequence, 20–40 for the lava-pool sequence, and 10–30 for the 80 C heat sequence, on both the human and dog. The recorded observations fit those bands. The calibrated first chlorine sample of approximately 4.4 seconds produced 19–21 damage on each human respiratory organ and 4–5 on each dog respiratory organ; the protected human had none. Chlorine is consequently a severe first-breath hazard for the stock human, even while external contact remains much milder. These bands apply to the recorded command sequences and selected stock health/armour profiles, not an exact wall-clock lethality guarantee.
