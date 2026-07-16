---
title: Database Seeder 3.0.0
summary: Expands the stock world-building packages and makes complete new-world seeding substantially more reliable across different setting choices.
date: 2026-07-16
tags: seeder, release, upgrade
---
**Upgrade note:** Database Seeder 3.0.0 targets .NET 10. Install the .NET 10 runtime before using the framework-dependent download.

## Expanded Starting Points

- Expanded the culture packages with substantially broader language, name, ethnicity, heritage, and height coverage for antiquity, medieval, Renaissance, early-modern, and modern settings.
- Added more complete agriculture foundations, including seasonal planting, fields, herds, forage profiles, and apiaries.
- Added stock animal-butchery profiles and made that package work with both detailed and simple skill selections.
- Expanded the time and celestial choices with additional calendar models, improved date suggestions, moon-view support, and gas-giant system options.
- Expanded the enabled economy, clan, health, weather, skill, and core starting packages with more useful templates and setting-aware defaults.

## Reliability

- Fixed several combinations of otherwise valid answers that could stop a complete seed, including simple skills, ARM-style attributes, optional distinctive features, Renaissance world-expansion cultures, and US-style celestial dates.
- Fixed duplicate-name handling for tags, gases, and placeholder objects so coherent catalogue data no longer causes installer crashes.
- Removed hidden ordering assumptions between enabled packages, including animal butchery's stock meat and skill prerequisites.
- Refreshed the blank database snapshot to the current migration set, making clean installation much faster and keeping snapshot refreshes safely targetable at a dedicated database.
- Exercised the complete enabled package sequence against ten clean databases spanning modern, antiquity, medieval, Renaissance, science-fiction, East Asian, Hijri, Middle-earth-inspired, and seasonal-calendar configurations, with a successful Engine startup after every completed seed.
