# Sustain Meal and Draw Wine integration note

This is a local two-spell content checkpoint on the provision selection dependency. It adds no central progress ledger, repertoire disposition, shared native harness dispatch, topology, devices, charm or combat-authority work. Earlier nine stock spells and their receipts remain history. Source files and test evidence are bound separately in `Armageddon_Provision_Verification_Receipt.json`.

## Authority and recovered rules

The authoritative brief is Library `libfile_a4606cd0097081918d6c9d9e220e6da0`, `Armageddon_Magic_Completion_Implementation_Brief.md` (905 lines), read through supported Library tools. Historical authority is Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, `codedump.c` (242965 lines). The earlier supported executor materialization attempt failed in the Windows helper; no cloud path was assumed and no denied-transfer workaround was used. Supported readable source-line responses supplied the authority.

`spell_create_food`, lines 83514–83643, independently draws `number(0,2)` once per level, places each chosen object on the ground, marks NOSAVE and schedules removal. Actual precedence is:

| Order | Source predicate | Historical object choices |
| --- | --- | --- |
| 1 | Silt sector | 644, 20940, 72197 |
| 2 | Plains Ox race | 1304, 1305, 1307 |
| 3 | Halfling race | 155, 55162, 55160 |
| 4 | Templar guild | 156, 50321, 50071 |
| 5 | Water Elementalist guild | 45466, 45509, 37452 |
| 6 | Tribe 24 | 157, 64150, 6398 |
| 7 | Defiler guild | 39103, 20908, 72255 |
| Last | Default | 151, 49039, 126 |

Historical object statistics/nutrition and correspondence to native FutureMUD race/guild/clan identities are unavailable. They are not claimed as recovered. Builders supply real approved food prototypes and native boolean(character) predicates in this order. Initial ordinary stock construction explicitly takes three native fallback foods; the optional editable profile adapter supports ordered pools of 1–32 distinct approved food prototypes. A narrowed/extended pool is an authored native adaptation, not a claim that the source had that pool. Food is one item per grade, with separate item-owned expiry and lifecycle provenance. Ordinary saved persistence protects consumed quantities and deadlines across restart; this deliberately adapts source NOSAVE.

The source timer is `level*30*60` EVENT units. `new_event` (45700–45787) stores raw delta; `heap_update` (45990–46045) advances the event heap by `time_step`. `game_loop` (169210–169294) calls `heap_update(4)` every `WAIT_SEC` pulses, `WAIT_SEC=12` (25051–25062). `OPT_USEC=250000` (168118–168125); the loop's timed `select` pacing is at 168940–169092. Twelve nominal 250ms pulses are three seconds for four event units: 0.75 seconds per unit, hence **1350*grade seconds**, grade seven **9450 seconds**. Native absolute deadlines remove nominal three-second heap batching and historical load delay. Affect-duration units are a different mechanism. The brief's source-following policy and finite food-expiry requirement override treating earlier recollections of indefinite food/water as absolute constraints.

`spell_create_wine`, lines 83697–83788, refuses Fire Plane, creates five units per level (ten on Water Plane) and clamps to remaining drink-container capacity. With positive volume it chooses Silt slime, then recipes in this order:

| Order | Source predicate | Recipe |
| --- | --- | --- |
| 1 | Tribe 24 | Horta wine |
| 2 | Tribe 14 | Gloth |
| 3 | Templar guild | Ocotillo wine |
| 4 | Tribe 53 | Spice brandy |
| 5 | Tribe 9 | Badu |
| 6 | Tribe 45 | Jik |
| 7 | Tribe 62 | Spiced Ginka wine |
| Last | Default | Wine |

The implementation takes authored native wine/liquid profiles and predicates; it does not equate historical numerical tribe/guild IDs to native IDs. The actual source compatibility conditional tests `LIQ_WINE`, despite its preceding comment mentioning water. Source nonempty incompatible liquid becomes slime. The coordinator approved refusing incompatible contents and Silt before payment, following the brief's safe creation policy and the reviewed Draw Water adaptation. Native recipes permit their own selected liquid; optional explicit compatible-liquid IDs remain builder choices. The authored 0.1-litre source-unit mapping gives **0.5*grade litres**, optional Water plane doubles it. This follows actual five-unit source behavior, not the superseded catalogue's smaller wine proposal. Wine has no magical expiry; native liquid persistence, freshness and consumption rules apply.

Wrappers at 217342–217385 (food) and 217536–217590 (wine) contain separate device, direct satiation/intoxication or other-recipient routes. These are outside this ordinary room/item cast lane. Neither spell invents those routes. Source repertoire admissions are opening 30, raw cap 90, minimum energy 7; Draw Water's raw cap 80 is not copied into Draw Wine. Actual native stock cost reporting is 7 at grade one and 50 at controlled grade seven with the configured efficiency profile.

## Normal builder workflow

Use `magic spell edit new stock sustain-meal <school> <casting skill> <resource> <food1> <food2> <food3>` or `magic spell edit new stock draw-wine <school> <casting skill> <resource> <wine> <bonus plane|none>`. Quote names containing spaces. Both create ordinary builder-editable MagicSpell definitions, costs, grade profiles, target triggers and adapters; they do not enrol a capability automatically.

While editing use `effect 1 foodprofile <order> <boolean(character) prog|always> <food prototypes...>` or `effect 1 recipe <order> <boolean(character) prog|always> <liquid>`. Use `<order> remove` to delete a row. The initial order-32 always fallback can remain while more specific profiles are added. All configured candidates must be valid even when a different profile matches. Show includes the exact authored profile/recipe XML and validation diagnostic. Set native predicates and prototype/liquid identities from the actual world rather than past numerical IDs. Add a casting-capability entry with opening 30, cap 90 and relative grade thresholds, and enrol/acquire through the existing service.

## Dedicated qualification and integration requests

`ProvisionStockNativeHarness` has its own project/entrypoint/runner. Its normal stock builder assertions include duplicate refusal, compiled native predicates, normal builder profile edits, SQL definition reload and actual paid low/high casting with persisted applied/cost receipts. It tests missing/unselected candidates, closed/full/incompatible/Silt/Fire targets, changed first match before payment, all three food choices under deterministic seeds, two fresh-copy token adoptions with a throwing random source, actual partial/full eating, wine volume/alcohol consumption, deferred needs across restart, custody without deadline reset, exact independent expiry, paid partial creation and origin replay refusal. Surrounding world/check catalogues are controlled; it is not full Telnet/login acceptance. Earlier four and five stock regressions run through this binary against separate owned disposable databases.

No registry dispatch additions are needed in production beyond the narrowly edited stock-builder partial. If a parent wants shared native dispatch, integrate the dedicated provision entrypoint explicitly; none was edited here. Parent must preserve its charged-device admission/pay wrapper while merging the allocated Prepare/Execution dependency. The conditional source model is implemented; exact historical food/liquid profile parity remains unavailable and world-specific mappings remain a builder task. Do not convert these limits into a claim that one unconditional historical output was recovered.
