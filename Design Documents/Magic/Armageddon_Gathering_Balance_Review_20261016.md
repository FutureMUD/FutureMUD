# Armageddon gathering: revised balance and concrete bindings

**16 October source-fidelity supersession:** Luke selected source-backed bodily Self and destructive, terrain-priced Land, with no Armageddon Gentle or invented ambient pool. Generic Gentle remains. This supersedes the earlier gathering proposal; see [the source translation record](Armageddon_Gathering_Source_Translation_20261016.md). Luke retained Sorcerer Self/Land, Preserver Self and Defiler Land; source-supported numeric translation and implementation qualification remain pending.

Proposal only; no values below are approved or implemented. This supersedes the numerical recommendation in Library proposal libfile_60b0bbb2e1948191a71be9d2fb96aad6. The independent inactive-installer matrix continues unchanged.

## Why revise the first rate

I deliberately matched the locked standing elemental rate, but that was too conservative for active gathering. One reserve unit per30 seconds is2/minute. A50-cost cast requires25 minutes of active work; elemental passive recovery is25 minutes standing or12.5 minutes resting. Defiler has only Land and no elemental passive generator, so that would be its regular funding burden. The30-second recommendation was my calibration choice, not an approved preset or a fixture constant.

The native controlled-cost curve matters more than printed minima: a cast at its controlled grade costs50 at stock scale1. Lower-grade casts can become cheaper down to their individual minimum; overreach is more expensive. The locked initial reserve remains0. Raw attribute13 gives capacity100; raw25 gives136. Those pools hold two50-cost casts, with0 or36 left. Faster gathering does not increase capacity or make an over-capacity quote payable.

| Recovery route | Units/minute | Time to50 | Time to100 | Time to136 |
| --- | ---: | ---: | ---: | ---: |
| Locked elemental standing passive | 2 | 25min | 50min | 68min |
| Locked elemental resting passive | 4 | 12min30s | 25min | 34min |
| Revised proposed Self/Gentle | 10 | 5min | 10min | 13min36s |
| Revised proposed Land | 20 | 2min30s | 5min | 6min48s |

These are ideal gathering/production times, excluding interruptions, command/tick overhead and unavailable bodily/source stock. Gentle must have actual recorded room stock; its pool is shared by local gatherers. Native profile outputs begin empty, with no installation/restart refill.

## Proposed method defaults

Keep Sorcerer Self/Gentle/Land, Preserver Self/Gentle, Defiler Land only; no Sorcerer-family passive generator.

| Method | Amount bounds and duration | Price |
| --- | --- | --- |
| Self |1..10 reserve/action; duration6*amount seconds | Stamina=amount; minimum remaining stamina0; damage/pain/stun0; health severity None |
| Gentle |1..10 reserve/action; duration6*amount seconds |1 environmental essence per reserve; no bodily or vegetation debit |
| Land |1..10 reserve/action; duration3*amount seconds | Ordered crop, woodland, pasture funding;1 native yield-score point per reserve; scar1 and pressure1 per reserve; no collateral, forage, ambient or additional crop/woodland-health prices |

Use the existing typed duration and stamina-price Prog hooks for proportional prices/time; larger batches must not create a faster rate or cheaper stamina conversion. No new runtime economics are needed. AllowAbsent=true on the three declared organic entries skips only native explicitly Absent sources; exhausted/invalid/unauthorised sources retain existing refusal rules. Native0..100 score points are not kilograms or individual plants. At1:1, a50-cost cast consumes50 available points; a room with100 available points can fund two such casts before native recovery. This is a substantial ecological cost, deliberately paired with Land's faster rate and subject to approval.

## What Gentle takes, and concrete generic bindings

In ordinary play, environmental essence means a room's stored ambient magical potential: a shared local pool of charges. One charge transfers to one personal reserve unit. It is not a crop, kilogram, herb or hidden player reserve. Gentle drains that pool without directly damaging plants or adding scar/pressure. Land spends actual eligible native vegetation/yield stock and damages the location. Organic stock does not automatically refill the ambient pool; only its explicitly configured profile production does.

Propose creating these owned stock identities, resolving their real database IDs internally:

- Resource key arm.resource.environmental_essence, name Armageddon environmental essence, native simple resource with LocationResource only.
- Profile key arm.environment.generic_land, name Armageddon living land, native environmental generator outputting that resource.
- Destination remains the existing canonical Sorcerer reserve shared by the three variants.

Concrete proposed profile values: base ambient capacity100, base production10/minute, initial balance0; pressure half-life3600 seconds; natural scar repair0.1/minute; magical scar-repair ceiling1/minute; default coordinator idle interval; scar description addendum on. Define F=max(0,1-scardamage/100)/(1+pressure/100). Ambient maximum=100*F and production=10*F. Apply F to all eight native crop/woodland/pasture positive-recovery and initialisation channels; no forage channel. Existing native field mechanics still own vegetation recovery. These are new world/balance choices, not inherited approved constructor defaults.

| Coverage | Exact generic binding |
| --- | --- |
| Crop/orchard locations | Same living-land profile; crop declaration permits Crop and Orchard uses, any compatible native crop definition |
| Woodland locations | Same profile; woodland declaration permits Woodland use, any compatible native woodland definition |
| Pasture locations | Same profile; pasture declaration permits Pasture use; no definition IDs |
| Barren or indoor locations | Unbound or explicitly Disabled; no stock ambient output or Land source |

All three organic declarations live in the one profile. A missing field, incompatible current use or missing eligible living crop/woodland is Absent, not fabricated stock. A forest description alone supplies no native woodland stock. Builders opt actual eligible rooms/terrains into the named profile using native binding tools; no numerical IDs or programs need be supplied by Luke. Installation creates the reusable definitions unbound and does not rewrite existing rooms, terrains or live field inventories. If automatic terrain assignment is desired, that is a separate explicit world-binding decision.

The proposed scar/pressure factor makes destructive use reduce local Gentle capacity/production and future vegetation recovery. A50-scar loss takes500 real minutes of active environmental advancement to repair naturally at0.1/minute; pressure halves each hour. This deliberately encourages moving to fresh ground or legitimate restoration. No offline recovery or immediate vegetation refill is promised.

## Availability and validation boundary

Every configured stock gathering method must reference a dedicated owned deny Prog, proposed key arm.prog.gathering_available, returning false with exact signature boolean(character actor, character owner, magiccapability capability, text methodKey, number amount, location room), AcceptsAnyParameters=false. Validate it before installing methods and refuse an incompatible enabled stock gate while availability is held. CastingPolicy.Enabled=false is not the gathering gate. Currently the ten canonical capabilities have no Gathering methods, so they grant no gathering; a configured-method integration must add and test this separate deny permission before publication/activation.

Balance approval would authorise content configuration, not enable availability. Representative actual MySQL/MUD funding, source depletion, ecological consequence, interruption, first/cold accounting and deny-permission checks would still be required. Current S5 elemental funding evidence is unchanged.

Evidence: ControlledSpellEfficiency.cs:13; MagicCastingService.cs:173; locked Armageddon_Seeder_Decisions_20261011.json resource_and_casting_defaults; Environmental_Magic_Builder_Guide.md; Magic_Gathering.md; Native_Organic_Yields_and_Ecological_Penalties.md; EnvironmentalMagicGenerator.Organic.cs:334; EnvironmentalMagicCoordinator.Organic.cs:175; EnvironmentalMagicCoordinator.Advancement.cs:25. No new implementation, build or native run was performed for this proposal.
