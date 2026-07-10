# FutureMUD Renaissance Military, Firearms, and Armour Design Reference

## Scope

This branch owns Renaissance pike-and-shot combat systems, actual early firearms and ammunition, transitional armour, period weapons, and regional shield forms beyond the shared support catalogue.

## Planned slices

- matchlock arquebus, caliver, early musket, handgonne survival, and wheellock pistol/long-gun families where mechanics allow;
- musket balls, loose shot, charge packets or historically bounded paper cartridges after ammunition policy is approved;
- bandoliers/apostle charge sets, gun rests, slings, worms, vent tools, and gunsmithing stock;
- pikes, bills, halberds, partisans, glaives, spears, lances, rapiers, sideswords, main-gauches, and complex-hilt precursors;
- plate harness and partial armour, breast/back plates, faulds, tassets, gorgets, burgonets, morions, cabassets, armets, and close helmets;
- bucklers, rotella-like shields, targets/targes, pavises, dhal/adarga continuities, and regional hide/wicker shields.

## Existing CombatSeeder firearm audit

The optional early-gun package in `CombatSeeder.SeedDataMuskets()` seeds only `Flintlock Pistol` and `Flintlock Musket` ranged types. It creates three flintlock pistol components in 0.65, 0.55, and 0.45 bore; four flintlock musket components in 0.80, 0.75, 0.70, and 0.60 bore; seven `Musket Ball` ammunition grades; loose-ball and preassembled-cartridge components; and `Flintlocks`, `Pistols`, and `Muskets` skills.

The `Musket` runtime supports loose powder, ball, optional wad, cartridges, cleaning, ramrod/tap loading, wet powder, jams, misfires, and catastrophic failures. It does not model a burning match, wheel winding, ignition-family condition, separate priming operations, bayonet attachment, shot spread, or gunsmith assemblies.

## Renaissance gaps and recommendations

The present seed is chronologically unsuitable as the Renaissance default because it is flintlock-only. Priority 0 is to remove hard-coded ranged-weapon database identifiers from component XML, make early-gun seeding rerunnable/repair-capable, and generate component/ammunition definitions through idempotent helpers.

Priority 1 is a matchlock operating family plus matchlock arquebus, caliver, and early-musket ranged types; a wheellock operating family plus pistol and selected long-gun types; ignition-appropriate emotes and reliability formulas; and lead-ball/loose-powder/wadding support that reuses the live shared rows.

Priority 2 is handgonne survival only if the runtime can represent its handling, bandolier/apostle charge sets, gun rests, and regional/contact variants. Flintlock stock should be excluded from ordinary Renaissance admission except a separately justified late-edge experimental context.

Paper cartridges should not be assumed universal, and the current cartridge item incorrectly uses lead as its case material. A paper or charge-packet definition is required for period-correct stock.

## Dependencies and acceptance

Exact skills/materials/tags are audited in `FutureMUD_Renaissance_PrimaryIndustry_UsefulSeeder_Impact_Reference.md`. Renaissance crafts use the live Gunsmithing/Gunsmith and Powdermaking/Powdermaker pairs; `Gunmaking` and `Munitioning` remain modern-option skills.

Every firearm row must resolve a live ranged type, operate/fire trait, ammunition grade, and component without numeric database-id assumptions. Ignition prose and mechanics must match. Exclude flintlock dominance, percussion caps, revolvers, rifles as the default, metallic cartridges, modern firearms/explosives, and unsupported attachment behaviour.
