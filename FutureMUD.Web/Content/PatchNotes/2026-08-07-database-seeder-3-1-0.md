---
title: Database Seeder 3.1.0
summary: Brings the ItemSeeder package to its first publicly usable release, with a broad historical catalogue and the latest repeatable-installation and project foundations.
date: 2026-08-07
tags: seeder, release, upgrade
---
**Upgrade note:** Database Seeder 3.1.0 targets .NET 10. Install the .NET 10 runtime before using the framework-dependent download.

Database Seeder is the guided installer and starting-content catalogue for FutureMUD. This release brings its ItemSeeder package to its first publicly usable state: the item catalogue is broad enough to provide a practical starting foundation for builders, while remaining maintainable and dependency-aware.

## ItemSeeder: A Public Starting Catalogue

- ItemSeeder covers the practical stock items a new world needs: clothing, outfits, armour, weapons, ammunition, firearms, artillery, tools, containers, household goods, jewellery, medical repair, food, production chains, vehicles, and the supporting materials, liquids, gases, tags, and components they require.
- Its historical catalogue spans Antiquity, Medieval, Renaissance, and Early Modern settings, with broader culture-aware clothing, military, household, craft, food, medical-repair, jewellery, and door content.
- Maintained admission manifests and dependency-aware package definitions keep the catalogue coherent, so seeded outputs arrive with the materials, components, tools, wear profiles, and prerequisite definitions needed to use them.
- Physical item workflows include real loading and attachment prerequisites for firearms and ammunition, as well as capacity foundations for seeded belt-like equipment.
- The ItemSeeder's output and coverage checks make it suitable as a public starting catalogue that builders can extend and customise through normal FutureMUD workflows.

## Other DatabaseSeeder Improvements

- The interactive workflow guides choices for setting, era, culture, calendar, climate, skills, economy, health, combat, crafting, vehicles, clans, celestial systems, and other core world foundations.
- Culture packages provide languages, names, ethnicities, heritages, heights, character-creation defaults, and setting-aware starting options across Antiquity, Medieval, Renaissance, Early Modern, modern, science-fiction, East Asian, Hijri, and other supported themes.
- Core world packages cover attributes, skills, traits, races, character generation, weather, time, celestial bodies, gases, liquids, materials, tags, currencies, clans, health, animals, agriculture, food, cooking, and economy foundations.
- Project foundations include labour queues, scheduling, launch entries, contribution merits, and the supporting seeded definitions needed for worlds that use project-based work.

## Installation and Rerun Safety

- Stock packages are designed for deterministic, repeatable installation and can reconcile seeder-owned records safely when a package is run again.
- Explicit dependency ordering and prerequisite diagnostics identify missing foundations before dependent packages run, while staged progress reporting makes long installs understandable.
- Seeder-managed record provenance protects stock ownership boundaries and helps distinguish maintained defaults from builder customisations.
- A maintained blank-database snapshot accelerates clean installation and is guarded against stale migration history; the bundled snapshot is current through `20260805124030_ProjectQueueSchedulingAndLaunchEntries`.
- Generated Windows and Linux startup launchers are ready to run the installed game with the selected database configuration.

No additional setup is required for this update beyond the normal .NET 10 runtime requirement. The seeded content is a starting foundation: builders can continue to customise, extend, and replace it through FutureMUD's normal in-engine workflows.
