---
title: FutureMUD Database Seeder 3.11.0
summary: New worlds can use role-based skill and knowledge grants, skip merits and background notes, and reserve detailed appearance choices for special applications.
date: 2026-11-25
tags: seeder, release, character creation
---

- **Role-first applications** use the configured free skill and knowledge grants. Applicants still make any required skill-group choices. **Race-first applications** keep the choice of a standard or paid-boost skill picker and the knowledge picker based on selected skills. Only race-first setup asks which skill picker to install.
- The merits question now accepts `neither`, alongside `merit` and `quirk`. Choose it to skip player selection of merits and flaws.
- Both application paths roll initial characteristics from the selected ethnicity and let players customise individual values. Simple and normal applications skip disfigurement selection. Special applications retain the interactive picker.
- New setups skip background notes. Builders who want to collect them can switch the notes stage to `NotePicker` and configure its prompts. Rerunning the character-creation seeder preserves previously configured screen types and definitions.
- Fixed a MySQL installation failure when the Mythical Animal Seeder refreshed a race’s breathing profile. Stock races retain their air and water breathing multipliers across the refresh.

Use Engine 2.19.0 with worlds created by this version so the Engine recognises the new notes screen. The bundled blank database includes the migration for Discord economy risk subscriptions.
