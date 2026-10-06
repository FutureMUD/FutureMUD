---
title: FutureMUD Database Seeder 3.7.0
summary: Adds optional world-simulation content and aligns fresh installations with Engine 2.14.0.
date: 2026-10-06
tags: seeder, release, installation, environment, creatures
---

**Compatibility:** Use Database Seeder 3.7.0 with Engine 2.14.0. Both require .NET 10 and MySQL 8.0. Framework-dependent packages target Windows x64, Linux x64 and Linux ARM64.

## Optional environmental and celestial content

Fresh Core setup asks whether to enable environmental exposure. Separate optional packs provide exposure definitions, protective equipment and preparations for builders to use and balance. The natural pack includes lava, scalding water and acids, with heat or chemical damage shaped by the exposed materials and their protection. Installing these packs does not enable exposure in an existing world or create hazardous rooms; updates preserve builder edits and report conflicts.

The Celestial Seeder can add authored sun, moon and other celestial examples alongside the existing physical packages. Examples include a simple repeating sun-and-moon cycle, in the spirit of Minecraft's sky. An artistic sun can instead rise only partway, pause near the horizon and fall back into darkness. The authored option defaults to no, and examples remain unattached until a builder chooses where to use them. Reruns add missing examples while preserving existing identities and edited definitions.

## Weather knowledge, installation and safe updates

Standard skill packages add Meteorology and the checks used for weather forecasts. This supplies the skill framework; it does not automatically grant forecasting knowledge to existing characters.

The Weather Seeder offers a fresh installation path with thunderstorm and dust variants, plus an explicit hazards update for existing stock climates. The update adds lightning and dust to verified stock definitions while preserving builder customisations, reporting conflicts and leaving weather controllers on their existing climates. Existing weather is not replaced by rerunning a fresh installation, and new controllers still need assignment to the world's zones.

## Predator and Monster AI stock

The Wildlife Catalogue adds species-specific predator recommendations, supporting combat settings and trap behaviours for ambushers, aerial hunters, web and burrow predators, and venom hunters. Existing NPC combat settings are not automatically replaced. Clone stock definitions before customising them where the package updates its owned records.

Mythical and Supernatural installations add reusable Monster AI profiles and race recommendations. These are additional choices for builders: they do not convert existing NPCs or needs models, attach controllers automatically, or supply missing attacks and equipment.

## Installation

The bundled blank database includes the current engine schema through the spell-owned remains-removal migration, supporting the accompanying casting and lifecycle systems.

## Upgrading an existing world

Back up the database, application and configuration, stop the engine, and use the normal backed-up schema upgrade path. Running the seeder is optional when you do not want additional supported stock content. Check each selected package's prerequisites and update guidance; this release does not make every seeder safe to rerun against arbitrary builder changes. Never use blank-snapshot refresh on a world that must be retained.

Keep a verified pre-upgrade backup outside automatic backup pruning and test restoration on an isolated database server. Reopen the world only after checking migrations, ordinary character login, inventory, saving and a cold restart. Rollback restores the pre-upgrade database and its matching old engine/configuration together.

Upgrade and explicit restore were rehearsed from an Engine 2.13.0 / Seeder 3.6.1 historical database on Windows x64 and MySQL 8.0.45. That evidence is not a general certificate for every older database or for crash and multiple-failure recovery.

Use [/downloads](/downloads) for archives and checksums and [/getting-started](/getting-started) for installation instructions.
