---
title: FutureMUD Database Seeder 3.9.0
summary: Separates optional item and Armageddon products from world foundations, simplifies celestial setup and updates supported stock content.
date: 2026-11-09
tags: seeder, release, installation, content
---

**Compatibility:** Use Database Seeder 3.9.0 with Engine 2.17.0. Both require .NET 10 and MySQL 8.0. Framework-dependent packages target Windows x64, Linux x64 and Linux ARM64.

## World foundations and optional content

Database Seeder now concentrates on creating worlds and installing their foundational systems. Stock item installation moves to the separately downloaded FutureMUD Item Seeder 1.0.0. Run Database Seeder first, install the required foundations, then use Item Seeder to install or update supported stock items.

Armageddon setting content and the historical Worldfile installer have moved into their own separate product. Interested parties should contact Japheth for information about the Armageddon product.

Moving these installers does not remove content already installed in a world. Supported stock ownership and identity records remain available to their owning product.

## Setup and stock improvements

- Celestial setup uses package selection, clearer defaults and fewer unnecessary follow-up questions. A single installed calendar is selected automatically.
- Updated stock AI Storyteller setup supports the Engine's modernised execution and supported stock upgrades.
- Mythical and supernatural person words use appropriate race eligibility rules. Supported stock updates preserve custom eligibility configuration.
- Missing pigment colour definitions are restored.
- Character-creation setup correctly skips the ethnicity screen when only one choice is available.

## Installation and existing worlds

The bundled blank database includes migrations through `20261106083212_RangedWeaponSecondaryDamage`, covering the newer stable-stay and ranged-weapon schema changes.

Back up the database, application, configuration and external files, stop the engine and other writers, and follow the matching Engine's managed upgrade process. Rehearse the upgrade and recovery against a copy before reopening the world.

Installing additional stock content remains optional. Review each package's prerequisites and supported update behaviour before rerunning it; not every seeder supports reconciliation over arbitrary builder changes. Stock eligibility changes require the appropriate seeder update and an engine restart to appear in an existing world.

Use [/downloads](/downloads) for packages and checksums and [/getting-started](/getting-started) for setup guidance.
