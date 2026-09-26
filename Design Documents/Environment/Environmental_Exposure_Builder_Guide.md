# Environmental Exposure Builder Guide

This guide describes the environmental exposure controls available to builders. It covers liquid surface reactions, gas contact and inhalation, material transmission, ambient heat, mode/configuration, migration, and a reproducible isolated setup. The demonstration steps below are instructions only; they are not a verified live-server transcript.

See [Environmental Exposure Design](Environmental_Exposure_Design.md) for ownership, rates, route resolution, finite-source accounting and lifecycle details, and [the catalogue audit](Environmental_Exposure_Seeder_Audit.md) for seeded material/fluid coverage.

## Enablement and migration

The migration `20260926124947_AddEnvironmentalExposureDefinitions` adds nullable `Materials.ExposureInfo` and `Gases.SurfaceReactionInfo`. Existing liquid reaction XML remains in its existing column and is independently versioned. The migration adds definitions; it does not create hazardous rooms or automatically turn on continuous exposure.

`20260926145630_ExpandLiquidExposureDefinitions` widens the existing liquid reaction column to `mediumtext`. A complete exact-material catalogue can exceed the previous 64 KiB `text` limit. Both migrations are required; existing reaction XML is retained.

`EnvironmentalExposureMode` accepts `Disabled`, `Legacy`, or `Enabled`. Fresh Core setup asks whether to enable environmental exposure and stores `Enabled` for yes or `Disabled` for no. Databases with no usable setting default to `Legacy`.

* `Disabled` disables exposure reaction damage and reaction consumption. Wetness, ordinary fluid delivery, washing/drying, fire, drugs and magical substances retain their existing behavior.
* `Legacy` keeps the old finite-contact liquid coefficients available. Their values are damage, pain and stun amounts per base fluid volume; they are not rates per second. It does not enable continuous v2 gas or ambient-heat injury.
* `Enabled` activates v2 continuous reactions and ambient heat. Unconverted v1 liquid definitions keep their finite-contact compatibility behavior. Use `convert` to preserve the old XML while explicitly authoring separate v2 rates.

Optional Environmental Exposure seeder packs do not change an existing world's mode. Install them only after core materials and liquids exist. The questions independently offer natural/industrial hazards and preparations, and fantasy hazards and preparations. Reruns are designed to preserve builder edits; name collisions without this seeder's ownership record are reported rather than adopted as seeder-owned content. Seeder-installed rates are game-balance starting points, not health-profile calibration.

## Reaction editor

On a liquid or gas definition, use its normal `set` editor and `reaction` subcommand. The command syntax is shared:

```text
liquid edit <id|name>
liquid set reaction add <existing-tag>
liquid set reaction <number> name <text>
liquid set reaction <number> channel <text>
liquid set reaction <number> category <text>
liquid set reaction <number> routes <flags>
liquid set reaction <number> material <solid>|none
liquid set reaction <number> tag <tag>
liquid set reaction <number> priority <integer>
liquid set reaction <number> type <damage-type>
liquid set reaction <number> damage|pain|stun <rate>
liquid set reaction <number> consume none|<volume>
liquid set reaction <number> spent <liquid>|none
liquid set reaction <number> mintemp|maxtemp <temperature>|none
liquid set reaction <number> applicability|intensity|notification <prog>|none
liquid set reaction <number> message <text>|none
liquid set reaction <number> exclude
liquid set reaction <number> convert <damage> <pain> <stun>
liquid set reaction test <material>
liquid set reaction test actor|item <target> <route|pool|interior> <seconds> [volume]
```

For gas, replace `liquid` with `gas`. `add` creates an inactive v2 rule with zero rates; finish its identity, route, selection and rates before relying on it. Gas rules cannot consume liquid or produce a spent liquid. Route flags are `LiquidContact`, `GasContact`, and `Inhalation` (comma-separated as needed). Ambient heat comes from material heat thresholds and world settings; it is not an authored reaction route. Ingestion and injection remain reagent/substance delivery paths.

Rules select an exact material and/or hierarchical material tags. Higher priority wins within one channel; exact material selection outranks tag selection at equal priority. Equal-priority equally specific overlaps are diagnosed and quarantined. Distinct channels may combine. Toggle `exclude` for an explicit no-reaction rule. Reactions report validation errors as inactive in `show` output.

For v2, damage, pain and stun are output amounts per reference exposure second, before contact work and global scale. `EnvironmentalExposureScale` multiplies resulting injury only; it does not scale physical work or liquid consumption. `consume` accepts a fluid volume per reference exposure second (for example `2mL`); it is a per-liquid–material reaction choice. `none` means no reaction consumption. With consumption enabled, finite owned liquid can exhaust itself; replenishing environmental pools do not pretend to be finite. A `spent` liquid replaces consumed volume one-for-one and is inert with respect to reagent charges. Damage resistance does not prevent consumption.

Ingestion and injection remain under their existing drug, poison, food and substance-delivery systems. The `Liquids` setting is the environmental liquid-contact gate; it does not enable the new reaction machinery for ingestion or injection.

The applicability prog returns Boolean, intensity returns Number, and notification returns Void. They receive the target, body/part/material identities, source/category/route, location, layer, strength, seconds and quantity; notification additionally receives committed damage, pain, stun and consumption. Invalid or failing configured evaluation is handled closed with a bounded diagnostic. Diagnostic commands do not execute conditional progs.

`show` displays rule IDs, version, legacy status, route/category/channel, selectors, rates, consumption, temperature limits, scripts and validation errors. A legacy v1 line explicitly reports amounts per base volume.

## Diagnostic commands

`reaction test <material>` is a read-only rule selection report for the current source. It shows the mode, selected routes, matching rules, conflicts, material transmission, threshold and whether continuous v2 behavior is inactive. It evaluates a full reference surface for one second.

`reaction test actor self LiquidContact 3 10mL` is an example prediction for a three-second finite liquid-contact exposure. `actor` accepts `self` or a visible actor; `item` accepts a visible item. A target diagnostic accepts a named route, or the special `pool` and `interior` contexts, a positive duration up to 600 seconds and optional positive finite liquid volume with units. It reports current layers/materials, area/transmission, selected rules and a predicted outcome. It does not settle active exposure, apply injury, or debit a source. Treat it as a current-state dry run, not a future guarantee; changing gear, materials, location or source changes the result.

For inhalation it also reports the effective breathing requirement, whether this is the actor's selected body, held-breath time and actual breathing fluid. A hypothetical surrounding-gas dose is not proof that the actor inhaled it: a held breath, clean apparatus supply, blocked airway or disabled mode can prevent delivery. Evaluate while `Enabled` to inspect normal armour prediction. Stock gases have separate respiratory rates because mild skin-contact rates can be entirely absorbed by organ armour.

## Materials, protection and heat

Edit a solid with the primary `material` builder. For example, `material edit <material>` followed by the commands below configures transport and direct heat response:

```text
material set transmission liquid 0.25
material set transmission gas 0.10
material set transmission thermal 0.50
material set transmission soak 0.02
material set heatdamage 80C
material set thermalresponse slope 0.05
material set thermalresponse cap 20
```

Transmission and soak fractions accept 0 through 1. `heatdamage` accepts an absolute temperature or `none`. Thermal slope is damage per second per Celsius degree; cap is maximum damage per second. Use `material set thermalresponse slope default` and `... cap default` to return either override to the world setting.

Material exposure properties separately control liquid, gas and thermal transmission and retained inward-soak rate. These are transmission settings, not damage resistance. A susceptible material can have low transmission, and a highly transmitting material can still be reaction-resistant. No liquid film or wetness-based protection is created.

`material set thermalresponse intensity <prog>|none` optionally selects a numeric ambient-strength hook using the same twelve arguments as reaction intensity. Category is `heat`, route is `AmbientHeat`, quantity is zero and strength is exposed area times transmission. Results are clamped to 0..100; failed, missing or invalid configured progs suppress the ambient contribution. The diagnostic never executes this hook and marks its prediction conditional.

Worn layers are evaluated outside-in; containers expose their contents according to openness and route transmission, while their owned interior liquid can react with the vessel itself. Material reaction resistance and equipment transmission are separate from natural/spell armour and exposure-resistance effects.

Ambient heat reads the physical cell's effective `CurrentTemperature(null)` in base Celsius. For a solid with `HeatDamagePoint`, the authored rate is `max(0, Celsius - threshold) × ThermalSlope`, capped by `ThermalCap`; area, transmission, elapsed seconds and global scale then apply. If the material has no threshold, there is no inferred ambient-heat immunity or injury from that material threshold. World `EnvironmentalExposureHeatSlope` and `EnvironmentalExposureHeatCap` provide fallback slope and cap. This is direct ambient injury. It is distinct from physiological `ThermalImbalance`, which models body temperature regulation. Lava's liquid contact reaction is independent of atmospheric heat.

To author a protection effect on a spell, open the normal `magic spell` editor and run these spell-editor commands, replacing the active spell with the desired character- or item-targeted spell:

```text
magic spell
effect add exposureresistance
effect 1 multiplier 0.25
effect 1 routes LiquidContact,GasContact
effect 1 category chemical
effect 1 part 0
```

The multiplier is a damage multiplier from 0 to 100; zero prevents the matching injury. Route flags can include `LiquidContact`, `GasContact`, `Inhalation`, and `AmbientHeat`. Category `*` matches all categories; part `0` means the whole target. Exposure-resistance effects can select route, category and optionally bodypart. External liquid/gas contact and inhalation are separately selectable. These effects reduce injury after physical reaction work; they do not reduce consumption or seal a surface. Reagent/substance delivery remains the way to apply preparations, including non-magically presented preparations.

## Runtime static configuration

`EnvironmentalExposureOptions` reads the following `EnvironmentalExposure*` settings. Values are clamped to their supported ranges; malformed numeric values use the listed fallback, and malformed/missing booleans default true.

| Setting suffix | Default | Purpose |
|---|---:|---|
| `Mode` | `Legacy` | `Disabled`, `Legacy`, or `Enabled` |
| `Liquids` | `true` | environmental liquid-contact gate; ingestion/injection are handled by existing delivery systems |
| `Gases` | `true` | external gas contact gate |
| `Inhalation` | `true` | respiratory route gate, independent of gas contact |
| `Heat` | `true` | ambient heat gate |
| `Characters` / `Items` | `true` / `true` | target-kind gates |
| `Scale` | `1` | global injury multiplier |
| `Interval` / `Substep` | `1` / `0.25` seconds | service cadence and bounded integration step |
| `MaximumInterval` | `60` seconds | maximum elapsed gap processed per advance |
| `MinimumVolume` | `1e-9` base volume | finite-mixture numerical tail cutoff |
| `HeatSlope` / `HeatCap` | `0.05` / `20` | ambient-heat fallback curve |
| `SplashReferenceLitres` | `0.1` litres | bounded reference dose for finite splash work |
| `CloudStrengthCap` | `1` | combined environmental contact strength cap across local clouds |
| `MessageInterval` | `15` seconds | continuing exposure message throttle |

Each local trap gas cloud has independent `contactstrength` from 0 through 1, separate from drug/reagent volume. Active clouds in a cell/layer are combined and scaled proportionally when their total exceeds `EnvironmentalExposureCloudStrengthCap`. This cap governs aggregate environmental cloud contact; inhalation remains independently enabled/disabled by `EnvironmentalExposureInhalation`.

Common messages use `EnvironmentalExposureWarning`, `EnvironmentalExposureContinuing`, `EnvironmentalExposureItemDeterioration`, `EnvironmentalExposureExhausted`, `EnvironmentalExposureProtection` and `EnvironmentalExposureDiagnostic`. Warning/protection/exhaustion take `{0}` as the reaction or liquid name; continuing takes reaction and part; deterioration takes item and reaction; diagnostic takes the reported detail. A reaction's `message` overrides the continuing/deterioration template. Messages are throttled; a separate exhaustion throttle allows an exhausted source to be reported after injury in the same resolution.

## Isolated demonstration setup

Use a disposable development database/world. Apply the migration through the repository's ordinary database upgrade process, select `Enabled` in fresh Core setup (or set the static configuration in the development world), and install the natural/industrial and optional fantasy Environmental Exposure packages after prerequisites are seeded. Do not use a production world for this exercise. The commands below are setup instructions, not a transcript; names in angle brackets must be replaced with IDs/names that exist in the test world. They assert no output or injury result.

First create a test terrain from an existing terrain, then assign it to a cell in an overlay package following the [room-building package workflow](../Building/Room_Building_Builder_Guide.md):

```text
terrain clone <existing-terrain> "Exposure Shallows"
terrain edit "Exposure Shallows"
terrain set model shallowwater <test-liquid>
cell package new "Exposure Demo"
cell new
cell set name "Exposure Test Pool"
cell set terrain "Exposure Shallows"
cell set type outdoors
cell package submit "Creates an isolated exposure test cell."
cell package review list
cell package review <package-id>
accept edit "Approved for isolated exposure testing."
cell package swap <package-id>
```

The terrain model form is `shallowwater <liquid>` and sets the terrain's water fluid and water/surface/air layers. The package must be approved and swapped before its cell overlay becomes current. The natural seeder provides hydrofluoric acid with a glass-reactive profile; hydrochloric acid explicitly excludes glass. The optional `exposure_incompatible_flask` is glass, while `exposure_compatible_flask` is a polymer that excludes hydrofluoric acid. Use hydrofluoric acid with the glass flask to inspect a finite vessel reaction, or use the compatible polymer to inspect the explicit exclusion. For continuous terrain contact, use a test liquid with an authored `LiquidContact` reaction and stand in the appropriate pool layer; for finite puddle contact, use a test vessel:

```text
item load exposure_incompatible_flask
loadliquid <test-liquid> <loaded-flask>
```

The optional natural package seeds the demonstration prototypes when their source material and required components are available. Keep a finite amount in the flask to examine its interior contact; `empty <loaded-flask> <amount>` pours that amount onto the ground as a finite puddle. `empty <loaded-flask>` pours the remaining mixture. Item loading and `loadliquid` are administrative/test setup operations. A target diagnostic can be run from the edited liquid using `reaction test item <loaded-flask> interior 3 <amount>`.

To set up a thermal comparison, edit a disposable solid using the `material set transmission ...`, `material set heatdamage ...`, and `material set thermalresponse ...` commands above. To author route protection, add the `exposureresistance` spell effect shown above; the optional seeded preparations are `alchemical burn salve`, `alchemical respirant draught`, `elemental ward tincture`, `vessel ward tincture`, and `breath ward draught` (availability depends on the selected seeder packs).

For local gas contact, configure a natural trap gas payload with `contactstrength` such as `0.5`; the trap payload parameter accepts `0..1` or `none`. Compare that cloud to surrounding atmosphere in the isolated test world, with inhalation independently controlled. The cloud cap applies only when simultaneous local clouds exceed the configured total. Stop by removing the test hazard/equipment and restoring the world's previous mode/settings. Record actual runtime observations separately if this setup is later exercised.

### Executed atmosphere and preparation example

The disposable live fixture used the following atmosphere package workflow. Replace IDs with those returned in your own world:

```text
cell package new "Exposure Gas Demo"
cell set atmosphere gas <chlorine-id>
cell package submit "Isolated exposure smoke fixture."
cell package review <package-id>
accept edit "Approved disposable smoke fixture."
cell package swap <package-id>
gas edit <chlorine-id>
gas set reaction test actor <subject> Inhalation 10
cell package swap <original-air-package-id>
```

Begin with healthy subjects breathing air and zero held breath. The existing implementor command `impdebug heartbeat 10second` can explicitly fire a breathing heartbeat in this test world. It is a test aid, not a player action or a substitute for measuring elapsed contact. After the first actual chlorine sample, another heartbeat while holding breath left the respiratory wounds unchanged. Human and dog wounds persisted through a server restart. External chemical burns still occurred on the preparation-protected subject.

The native preparation sequence was:

```text
item load exposure_compatible_flask
loadliquid <alchemical-respirant-draught-id> <flask>
open <flask>
give <flask> <subject>
force <subject> drink 10ml flask
sniff <subject>
```

The non-caster received an Inhalation/chemical multiplier of 0.25. In this stock human, the remaining dose was fully absorbed by natural organ armour; it did not provide breathable oxygen. The effect and its substance activation survived restart. External ward activation was separately verified through the existing `exposecharactertoliquid(character, liquid-id, volume-text, bodypart-text)` FutureProg function, applying 1 mL to the abdomen. Applying that amount only to a hand may retain too little for the preparation's minimum dose.

The acid/lava pool, compatible/incompatible finite vessels, heat, reagent and respiratory observations are quantified in the [verification report](Environmental_Exposure_Verification.md). A pre-weakened glass flask was used for the destruction/spill check; an intact polymer flask retained its contents. Those observations should not be interpreted as a claimed destruction time for a healthy glass vessel.

## Boundaries

This system does not add thermodynamic object temperatures, heat transfer, phase changes, pressure/flow, gas diffusion, acid-base chemistry, structural erosion, protective films or automatic ignition. It does not make oxygen uptake itself chemical immunity and does not reinterpret ordinary drug/reagent quantity as gas concentration. Continuous environmental exposure also does not replace poison, food, drug or substance delivery for ingestion/injection.
