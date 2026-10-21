# Intrinsic racial merits

Races can grant body-scoped character merits through `Races_Merits`. A body exposes its explicit merits plus its race's direct and inherited grants, deduplicated by merit ID. Character merits combine explicit character assignments with the selected body's effective merits. Intrinsic grants are evaluated dynamically; they are not copied into `PerceiverMerits`, selectable chargen quirks or new form templates. Removing a direct race link leaves any parent grant in force. Existing explicit assignments remain saved independently.

Use `race set merit <id|name>` to toggle a direct grant. Race show distinguishes direct and effective intrinsic merits. Character merit builders can use `scope body|character`; changing a directly linked racial merit to character scope is refused until its race links are removed. These are generic engine capabilities and have no racial-name checks.

## Stamina Capacity

The `Stamina Capacity` type adds a finite signed `bonus` and applies a finite nonnegative `multiplier` (entered as a percentage). Maximum stamina is `(base + sum of applicable bonuses) * product of applicable multipliers`, clamped at zero, followed by the existing equipped-item bonus. Recovery and movement cost are independent. A capacity change clamps excess current stamina without refilling it or recording the clamp as expenditure. Inactive-body applicability is evaluated against that body with the original body restored after evaluation, including exceptions.

XML fields are `Bonus` (default0) and `Multiplier` (default1). Invalid nonfinite values and negative multipliers are rejected on load with an actionable error. The new type defaults to Body scope.

## Environmental Needs

The `Environmental Needs` type supplies optional number(character) progs for passive hunger-rate, thirst-rate and water-loss deltas. Builder commands are `hungerprog`, `thirstprog` and `waterprog` with a prog ID/name or `none`. Hunger/thirst use satiation hours per game hour; water uses litres per game hour. Signed credits can cancel loss, but combined net loss cannot become a grant. Each configured prog ID executes once per evaluation, even if multiple channels use it; the context is reevaluated on the next needs heartbeat.

Optional `maxhunger` and `maxthirst` cap the combined baseline/environment rate before existing constant needs merits and effects multiply it. `none` removes a cap. Missing or incompatible progs, invalid numeric results and invalid configuration produce a visible configuration warning and neutral contribution; a failed configured channel does not apply its cap. A zero prog ID deliberately permits a cap without a delta. Missing positive IDs and malformed/negative prog references survive unrelated saves for builder repair; only an explicit channel edit clears an invalid reference. XML fields are `HungerProg`, `ThirstProg`, `WaterProg`, `MaximumHungerRate` and `MaximumThirstRate`.

Explicit food/drink fulfilment, sweat and other physical withdrawals retain their existing paths. An unconfigured world has no environmental contribution. The merit does not assign a basal `Body.WaterLossLitresPerHour`; the current production property has no tracked assignment. A world must provide a meaningful water-loss prog before claiming additional passive litre loss. Source condition points are not automatically interpreted as litres.

Cold character loading preserves saved blood volume before capacity is calculated, so active-needs hydration normalisation sees the saved partial volume rather than an uninitialised zero. Once capacity is available, excess blood is clamped including zero-capacity bodies. The existing empty-blood fallback for living characters is unchanged.

## Movement and existing consumers

`Movement Stamina Multiplier` now contributes to walking, swimming, flying, floating and climbing costs, including vertical and maintenance costs. Mode-specific swimming/flying merits compose once per ID and honour applicability. Missing/invalid stored multipliers are neutral; deliberate zero remains valid. Temperature and bodypart-health consumers also deduplicate effective merits before applying them.

## Armageddon stock use

The Setting package grants environmental thirst to all seven playable profiles: ordinary handling for six, Desert Elf handling for that profile. The common Elf parent has no environmental merit, avoiding double application. Desert Elf receives+60 stamina; Half-Giant and Mul receive+20. Half-Giant receives existing `Needs Rate Change` hunger3/thirst3/alcohol1, with underlying race rates1 and food/drink capacity48/24. No City Elf capacity or new movement discount is added.

Owned merit definitions, direct race links and numeric progs use existing three-way reconciliation. Initial adoption retains custom links; subsequent builder additions/removals and definitions survive reruns. Terrain bindings are stored by ID in the stock baseline, so renaming a terrain does not change its meaning. Missing bound terrains require explicit resolution.

The stock environment progs adapt the recovered severe/moderate terrain, shelter, native Morning/Afternoon and below-standing conditions. Shelter credits clamp loss at zero. Core terrain expansions are authored mappings. Native time-of-day has no Highsun band, and source condition points do not define litres: stock WaterProg is deliberately zero. This is a documented native adaptation, not an exact reproduction of the source engine's clock or physiology. All eight Armageddon race rows remain unavailable pending the separate physical/lifecycle activation work.
