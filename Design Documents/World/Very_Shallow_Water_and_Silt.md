# Very shallow water and silt

These opt-in terrain behaviours represent a supporting ground surface covered by liquid or particulate material. They retain `GroundLevel`, `InAir` and `HighInAir`; they do not add a spatial layer or change existing `shallowwater` and `deepwater` models.

## Builder configuration

```text
terrain edit <terrain>
terrain set model veryshallowwater <liquid id or quoted name> <size> [clear|opaque] [norun|run]
terrain set model silt <size> [clear|opaque] [norun|run]
```

Use the existing size categories, such as `Small`, `Normal`, `Large` or `VeryLarge`. The size is the largest bottom-resting subject completely covered by the medium. It is an ordinal approximation, not a depth in metres. The model string persists the liquid ID, size and optional settings; no additional anatomy or database migration is required.

Very shallow water is clear by default. Silt is opaque by default. Both allow ordinary gait selection unless `norun` is specified. This optional setting caps ground travel to the body's slowest native gait, including custom gait names; it does not guess which aliases mean running. The selected ground gait can subsequently be changed normally. Configure deep and shallow silt as separate terrains: neither running restrictions nor other source-specific rules are inherited from a common terrain name.

Invalid liquids, undefined sizes and unknown options are refused before a builder changes the terrain. A malformed saved definition fails loading explicitly rather than silently becoming dry ground. Switching to another model clears the old immersion and liquid settings.

## Categorical coverage

Items use native item size. Bodies use existing standing, sitting or prone size according to posture, including the ordinary physical size context. Exit-fit and ranged-target size modifiers are not immersion height.

| Subject size minus immersion size | Bottom contact | Upright bodypart bands covered |
| --- | --- | --- |
| Zero or less | Full | All external parts |
| One | Partial | Lowest, Low, Centre, High |
| Two | Partial | Lowest, Low, Centre |
| Three | Partial | Lowest, Low |
| Four or more | Partial | Lowest |

Sitting/kneeling lowers leg contact. Flat postures flatten the anatomy bands; prone/prostrate airways are treated as low contact. The positional-size change is not applied twice. Special Appendage/Irrelevant orientations use existing limb/parent anatomy with a central fallback. These defaults deliberately trade exact geometry for useful behaviour without extra race/bodypart building.

Actual placement takes priority. Flight, valid supported occupancy, mounted travel, containment and flotation are resolved before bottom coverage. Held and worn goods receive liquid through the contacted bodyparts and ordinary garment/container protection. A small held item is not submerged just because its item size is small. Corpses use their original body's anatomy anchored at the corpse's physical site.

## Movement and breathing

An ordinary actor one or two categories larger than the medium wades. Ground travel takes 1.5 times its ordinary terrain duration and stamina. Three or more categories larger walks normally while retaining lower-part contact. A fully covered ordinary actor requires native swimming for ordinary liquid traversal. No new posture, body speed rows, skill or persisted MovementType flag is introduced; legacy movement filters retain their classifications.

Successful surface swimming is distinct from resting on the bottom. A small swimmer keeps the ordinary exposed airway at the surface. The existing stay-afloat check and stamina cost apply. Failure or incapacity settles the actor onto the bed at `GroundLevel`; contact and breathing then resolve from that posture. There is no nonexistent underwater layer to fall into. A fall into very shallow water lands on the bed and uses ordinary fall damage, without deep-water cushioning or further submerged strata.

Movement previews, committed travel and longitudinal route travel use the same categorical wading policy. The mount is the motive actor; a rider does not pay the mount's wading cost. Dismount, disembark, lost support and posture changes reassess the newly exposed actor.

Only the liquid behaviour supplies terrain liquid contact. Existing wetness, reaction modes, respiratory strategies and breathing equipment retain ownership of their effects. Mouth, blowhole and gill contact use the immersed anatomy result; a fully covered partless breather uses the liquid. Exposure is settled before placement/posture changes and stale respiratory samples are cleared. Silt never sets a liquid atmosphere, supplies liquid wetness or enables drowning merely through categorical coverage.

## Items and vehicles

Clear liquid leaves submerged items visually eligible. Opaque media hide fully covered loose bottom items through the ordinary canonical visual predicate. Partial items, carried/contained goods and actual floating/supported items are not blanket-hidden. Ordinary Hide/Spot and physical reach remain separate. Administrator and explicit IgnoreCanSee bypasses retain their existing behaviour.

Item buoyancy determines surface versus bed placement in very shallow liquid. Negative buoyancy stops at the local bed rather than inventing a lower room layer. Spills/drips into the ambient water do not create a second finite puddle on top of it. Rain does not automatically change immersion size. No new current, tide or flooding simulation is implied.

SurfaceWater vehicle environments accept both legacy surface water and the new liquid ground surface. The appended Silt vehicle environment accepts only silt surfaces at both endpoints. Valid occupancy requires an intact correctly placed vehicle. Neither being near a boat nor using a position target supplies support. Vehicle draft and loaded displacement are not inferred from immersion size.

Dropping, visual occlusion and burial remain different operations. This feature does not create buried-item custody, copy recovered items or introduce a new Forage output route. Existing rooms, stock swamp terrains, source-specific creature rules and ecological stock are not automatically migrated or rebound.
