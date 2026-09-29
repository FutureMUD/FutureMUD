# Environmental and substance exposure

Environmental exposure is a builder-authored contact model. It resolves liquid, external gas, respiratory fluid and ambient heat through ordinary wounds, while leaving fire, oxygen uptake, physiological temperature imbalance, drugs and reagent activation as their own systems. It does not simulate chemistry, object temperatures, cooling, phase changes or protective liquid films.

## Modes and migration

`EnvironmentalExposureMode` is `Disabled`, `Legacy` or `Enabled`. The default for databases without an explicit selection is `Legacy`. The fresh Core seeder asks whether to enable the system; answering no writes `Disabled`. Installing either optional content pack in an existing database never changes the mode.

* Disabled prevents reaction damage and reaction consumption at every exposure entry point. Ordinary wetness, washing, drying, drugs, fire and substances continue to work.
* Legacy dispatches preserved version-one liquid contact coefficients. Those coefficients remain amounts per base fluid volume, never damage per second. Gas and ambient heat do not acquire continuous injury in this mode.
* Enabled resolves version-two reactions continuously. Unconverted version-one definitions retain their finite-contact compatibility path. Explicit conversion records the original XML payload and authors new rates separately.

The schema adds nullable `Materials.ExposureInfo` and `Gases.SurfaceReactionInfo`, and widens `Liquids.SurfaceReactionInfo` to `mediumtext` for catalogue-wide material rules. Liquid reaction XML, body surface XML and saved spell effects are independently versioned. Old aggregate body wetness distributes once over the current external anatomy. No persisted wall-clock gap becomes an injury interval on restart.

## Ownership and geometry

`BodySurfaceLiquidState` owns a `SurfaceLiquidState` for each stable external bodypart ID. Its aggregate is a read-only view of the original liquid instances, not cloned resources. Weight, saturation and reagent enumeration therefore see each retained quantity once. Part removal or anatomy changes redistribute orphaned wetness over remaining external parts while preserving quantity, species and reagent lot identity. The aggregate legacy API remains available for ordinary callers.

Body area is the non-negative relative hit chance of a part divided by the sum over external parts. Internal respiratory patches use equal shares over the real respiratory anatomy; partless respiration uses external area as the existing abstract body surface. Item area is a bounded size-derived reference area. These are gameplay contact weights, not square metres.

A source is either replenishing or finite:

| Source | Owner and accounting |
| --- | --- |
| Terrain immersion | Replenishing contact; no fictional pool debit. Only bounded new wetness is retained. |
| Splash / scripted delivery | One finite mutable mixture, traversed outside to inside under one delivery batch. |
| Retained contamination | The actual bodypart or item surface mixture; reaction debits remain savable. |
| Ground puddle | The cell surface state; simultaneous contact patches share its budget. |
| Vessel interior | The container component's actual owned mixture; no copy is charged instead. |
| Atmosphere | Persistent room fluid with strength one. |
| Local cloud | Independent saved source identity and contact strength, separate from drug quantity. |
| Breath | The selected breathing fluid and actual supply/airflow result. |

The effective cell atmosphere gives applicable explicit atmosphere effects priority over an exposed weather gas override, then falls back to the permanent overlay. Weather gas reaches outdoors and climate-exposed interiors; windows and sealed interiors retain their air. Underwater ambient gas contact is excluded and breathing continues to resolve the terrain water fluid. Weather/effect transitions settle the old exposure before refreshing registered affected cells. [Weather forecasts and hazardous weather](../World/Weather_System.md) documents the builder settings and stock dust variants.

Captured contact patches reject stale bodyparts, moved targets, changed materials, deleted items and changed containment. The simulation registers active targets, settles elapsed time before movement/equipment/environment/effect changes, and evaluates bounded substeps. Repeated LOOK, description, weight or saturation reads do not apply injury.

The default heartbeat interval is one second with 0.25-second integration substeps. Constant rates retain exact elapsed-time work; finite concentration, inward soaking and consequences are recalculated between substeps. Builders can reduce the substep for a steeper authored system at the cost of more anatomy and health processing. Geometry and saturated-immersion refresh avoid repeated whole-body scans; unchanged static reaction selection is cached within an advance, while script results and callback-sensitive mode gates remain live.

Saving and display reads do not advance wetness, weather or exposure. The clock tracks ordinary wet surfaces even in Disabled and Legacy modes so drying does not depend on inspection. Tracked occupied/wet cells update rain and surface drying on a five-second cadence, once for each physical layer. Exposure settlement is deferred while the save manager drains its queue, retaining the elapsed interval for the next simulation advance. This prevents serialization from producing a new generation of wounds and keeping the save queue alive indefinitely.

Registry loading does not create the exposure service or evaluate partially loaded targets. Boot-time advances only reset the clock; the complete world registers its initial active set before the heartbeat starts. Loading saved contamination therefore starts a new interval without an offline injury backlog.

## Rates and finite depletion

A version-two rule has a stable GUID, route flags, category, channel, priority, material/tag selectors, optional temperature bounds, damage type, damage/pain/stun rates and optional scripts. A higher-priority rule wins within a channel. Equally specific equal-priority overlaps are quarantined with a diagnostic; explicit exclusions perform neither injury nor consumption. Different authored channels may coexist.

For a liquid component, contact intensity is area × transmission × component concentration × wetted fraction. Replenishing contact is fully wetted; finite contact uses owned quantity relative to local capacity. Damage is rate × contact work, where work is intensity × elapsed seconds. Splash work has a global bounded reference dose, so splitting one action into many callbacks cannot multiply it.

Consumption belongs to the liquid–material rule: `None` or `PerExposure`. Requested consumption is work × consumption rate. Simultaneous consuming patches draw proportionally from the same component budget. Non-consuming patches sharing that component stop when another patch exhausts it. Sequential physical layers debit the outer stage before passing real remainder inward. The resolver re-evaluates changing mixtures in bounded substeps and clamps numerical tails at the configured minimum quantity.

Consumption occurs before callbacks and independently of wounds or resistance. Zero-damage consuming rules are valid. An optional spent liquid replaces only consumed volume one-for-one and carries no active reagent charges. Other mixture species survive. Drying and residue production remain separate operations.

`OrdinaryDrying` and `PersistentUntilRemoved` are independent retained-hazard choices. Stock lava uses the latter: oxygen, water-style evaporation and the presence of a flame are not prerequisites for its contact damage. Removal, washing, transfer or a separately authored consuming rule can still remove it.

## Equipment, containers and health

Materials have independent liquid, gas and thermal transmission fractions and a retained inward-soak rate. Resistance to injury does not imply sealing. Wetness provides no intrinsic armour: a saturated surface exchanges a bounded quantity with new incoming liquid and returns displaced wetness to the real finite stream.

Worn layers are visited outside to inside. A destroyed layer stops shielding subsequent contact. Held and wielded items receive their own contact. Open containers expose contents; closed containers use the corresponding route transmission. Container contents attack their vessel interior even when the lid is closed. Already contaminated enclosed items continue their own retained exposure.

Resolved injury carries `ExposureDamageContext`, including route, source, category, channel, reference interval and actual body identity. The body health path skips the already-applied clothing traversal and impact-only bone/sever routing, retains applicable natural and spell armour, and writes wounds to the injured body rather than the controller's currently selected body. Armour formulas evaluate continuous rates against a reference second before restoring the interval quantity; per-source deterministic expression randomness avoids changing protection merely by splitting a tick.

After armour, the concrete health strategy accumulates compatible continuous exposure into marked ordinary wounds. Their saved `ExposureKey` identifies route, source, reaction, channel, actual body and part; elapsed time and current strength are excluded. Each increment applies the wound type's bodypart modifiers once and updates severity, descriptions, treatment state and bleeding immediately. Strategies with a per-wound bodypart cap fill existing capacity before creating deterministic overflow wounds, so substeps cannot discard delivered damage or generate a wound per timer callback. Source identity and overflow survive normal wound XML persistence. Ordinary combat wounds never merge into these keyed wounds, and distinct sources remain separate. Simple item and construct wounds retain their existing uncapped modifier rules.

Simultaneous body patches collect their resulting wounds for one normal health/status pass per actual body per substep. Wound quantities change immediately; nested reaction batches join the current substep. Corpse wounds use the original body even if its controller selected another body. Ordinary item health and destruction are processed immediately, so a failed layer or vessel cannot continue shielding later contacts. This avoids repeating a full limb/organ status traversal for every contacted part.

A corpse owns the exposure clock for its original anatomy. Its original body is removed from the independent active set, while retained part mixtures continue on that anatomy. New splashes, immersion, gas and heat use those same parts and materials at the corpse's physical location; clothing remains a separate contacted layer. This prevents both null-part injury loss and duplicate living-body/corpse ticks. Corpses never create respiratory samples.

## Breathing, gas and heat

Successful oxygen uptake is not chemical immunity. A functioning open airway can sample an unbreathable fluid before the existing held-breath/suffocation model takes over. Held breath, stopped respiration, failed gas withdrawal and non-breathing strategies do not invent inhalation. A successful clean supplied breath excludes the ambient/cloud inhalation source while leaving external contact intact. Lung, gill and blowhole strategies target their actual respiratory parts; partless strategies use the abstract external-body path. Normal ingestion/injection reactions are not implemented; those vectors remain supported by ordinary reagent/drug delivery.

Gas contact and respiratory contact are separate routes and tissues. Old saved clouds with no contact-strength metadata default to zero new contact hazard. Drug/reagent dose is never treated as a gas concentration.

Stock tissue gas definitions use disjoint external and inhalation rules. Their respiratory rates are calibrated against existing organ armour, whose flat dissipation otherwise absorbs the milder external rates completely. This changes authored data, not ordinary armour or oxygen requirements. The stock chlorine inhalation rate is 18 per reference second; three equal organs receive 6/s before armour. Human quality-2 organ armour leaves 4.5/s per organ; dog quality-5 armour leaves 1/s. A 0.25 respiratory preparation reduces both below those floors. Seeder upgrades keep the old external GUID and add a deterministic respiratory GUID. Customised combined rules are preserved and their stock split is deferred; pre-existing conflicting custom rules are reported for explicit builder resolution.

Repeated cloud identities contribute once. A cloud has contact strength from zero to one; combined local cloud strength is proportionally limited by `EnvironmentalExposureCloudStrengthCap`. The separate gas-contact and inhalation toggles do not change existing drug amounts.

Heat uses `Cell.CurrentTemperature(null)` and the physical room state, not a builder's unpublished preview. Base temperature is Celsius. A null `HeatDamagePoint` means no authored ambient injury threshold, not inferred immunity. The curve is `min(cap, max(0, temperature - threshold) * slope)`, multiplied by area, transmission and elapsed time. Material overrides fall back to the explicit world slope/cap. Alternative thermal descriptions of the same atmosphere are reconciled per injury channel; liquid lava contact is independent of atmospheric heat and oxygen.

A material can configure an ambient intensity prog with the same twelve-argument exposure context. It receives category `heat`, route `AmbientHeat`, physical area/transmission as strength and zero liquid quantity. Its finite non-negative result is capped at 100; missing, invalid or failed configured progs suppress that material's ambient injury. Diagnostics skip the prog and identify the conditional result. Intensity hooks cannot commit injury after disabling the central mode.

## Protective preparations

`exposureresistance` is an ordinary spell effect usable by the existing substance/reagent system on characters or items. It selects routes, category and optionally a part, and scales injury after physical reaction work. It does not reduce chemical consumption, create a film or require spellcasting knowledge. External and respiratory protection must be selected separately.

Body-delivered preparations remember the source body ID. Maintained doses enumerate that body's retained lots even if the character changes its active body. Timed and maintained effects keep the existing duration, clearance, suppression, stacking and charge rules. One delivery split over bodyparts remains one reagent activation batch.

## FutureProg contract

The ordered applicability/intensity inputs are documented in the builder guide. Applicability is Boolean. Intensity is a finite non-negative multiplier bounded by the resolver; it scales reaction work and consumption together. Exposure resistance is the damage-only modifier. A failed/missing configured script fails that rule closed with a bounded diagnostic. Mutable script answers are not cached. Post-resolution notification receives committed damage/pain/stun and actual consumed volume after debit. Notification failure never repeats work. Resolver reentrancy prevents a callback from recursively resolving the same source transaction.

## Related documents

* [Builder guide and isolated demo](Environmental_Exposure_Builder_Guide.md)
* [Catalogue audit](Environmental_Exposure_Seeder_Audit.md)
* [Verification report](Environmental_Exposure_Verification.md)
* [Magical substances](../Magic/Magical_Substances.md)
* [Damage system](../Health/Damage_System_Design.md)
