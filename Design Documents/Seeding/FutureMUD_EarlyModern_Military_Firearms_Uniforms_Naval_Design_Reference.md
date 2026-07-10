# FutureMUD Early Modern Military, Firearms, Uniforms, and Naval Design Reference

## Scope

This branch owns Early Modern state-army, naval, uniform, firearm, ammunition, and period-survival combat goods beyond the shared military-support and gunpowder-support rows.

## Planned slices

- matchlock, snaphaunce, flintlock/firelock musket and pistol families;
- musket balls, loose shot, paper cartridges, cartridge boxes and slings, gunflints, worms, combination tools, and repair stock;
- plug and socket bayonets only after attachment semantics are approved;
- hangers, smallswords, sabres, boarding axes, and surviving pikes/halberds through suitable weapon types;
- uniform coats, waist belts, gorgets, sashes, drums, fifes, standards, camp kettles, campaign chests, and officer writing cases;
- speaking trumpets, signal flags, sea chests, shot lockers, sailcloth, rope, tarred buckets, boarding goods, and navigation kits;
- breastplates, cuirasses, gorgets, buff coats, helmets, and persistent regional shields without treating full plate as the default.

## Existing CombatSeeder firearm audit

`CombatSeeder.SeedDataMuskets()` currently seeds only two ranged weapon types: `Flintlock Pistol` and `Flintlock Musket`. It creates three pistol components (`Pistol_Flintlock65`, `Pistol_Flintlock55`, `Pistol_Flintlock45`), four musket components (`Musket_Flintlock80`, `Musket_Flintlock75`, `Musket_Flintlock70`, `Musket_Flintlock60`), seven `Musket Ball` ammunition grades from 0.8 to 0.45 bore, loose-ball and preassembled-cartridge components, and the conditional skills `Flintlocks`, `Pistols`, and `Muskets`.

The runtime `Musket` component already models loose powder, ball, optional wad, cartridge loading, ramrod/tap loading, cleaning, jamming, misfire, wet powder, and catastrophic failure. It does not model ignition-system-specific state, a burning match, wheel winding, frizzen/flint condition, bayonet attachment, buck-and-ball, shot spread, or firearm-specific repair assemblies.

## Required firearm gaps

Priority 0 is to make the existing seed safe to extend: replace hard-coded ranged-weapon database identifiers in component XML with the actual seeded `RangedWeaponTypes.Id`, make the early-gun package rerunnable/repair-capable, and remove the misleading use of `AmmunitionLoadType.Magazine` for a single muzzle-loaded charge if the runtime can accept a clearer load model.

Priority 1 for Renaissance survivals and Early Modern breadth:

- add matchlock and wheellock operating traits/types and ignition-appropriate emotes;
- add snaphaunce/flintlock/firelock variants without duplicating bore-only component bodies;
- add arquebus/caliver/early-musket and musket/carbine/fowling-piece/blunderbuss distinctions where range, handling, bore, or spread differs;
- add historically bounded pistol families, including wheellock and snaphaunce/flintlock;
- add paper cartridges as paper-cased ammunition rather than the current lead-cased item;
- add reusable helpers for bore/component/ammunition creation and idempotent upsert.

Priority 2 after mechanics are approved:

- bayonet attachment or a documented separate-weapon fallback;
- shot, buck-and-ball, and spread ammunition;
- match-cord consumption and weather exposure;
- gunflint wear, lock quality, gunsmith repair stock, proofing, and barrel-failure semantics.

## Dependencies and exclusions

The shared `preindustrial_firearms_*` rows must be reused. Exact skills, materials, and tags are audited in `FutureMUD_EarlyModern_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`. Historical firearm crafts use the live Gunsmithing/Gunsmith and Powdermaking/Powdermaker pairs; `Gunmaking` and `Munitioning` remain optional modern-package skills.

Exclude percussion caps, revolvers, metallic cartridges, repeating weapons, default mass rifling, modern shells/explosives, machine guns, ballistic armour, and modern uniforms.

## Acceptance criteria

- Every firearm prototype resolves a live ranged type, operating trait, ammunition grade, and component without numeric database-id assumptions.
- Ignition and loading prose match the selected family.
- Firearms, ammunition, and powder chains are optional, explicit, and rerunnable.
- Naval and uniform rows remain usable without installing unsupported firearm mechanics.
