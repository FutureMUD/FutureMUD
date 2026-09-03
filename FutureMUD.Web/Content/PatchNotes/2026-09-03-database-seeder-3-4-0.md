---
title: Database Seeder 3.4.0
summary: Brings signing, contemporary appliances, media and security systems into the stock catalogue while making new installations and supported development replays more dependable.
date: 2026-09-03
tags: seeder, release, upgrade, content, modern
---
**Upgrade note:** Database Seeder 3.4.0 targets .NET 10. Apply the normal database upgrades before seeding, and update to Engine 2.10.0 to use the accompanying runtime support.

## A World Ready To Sign

The stock culture path now provides the foundations for signed-language worlds alongside its established spoken-language content. Fresh installations can create and discover signed languages, their varieties and the related character capabilities needed by Engine 2.10.0, giving builders a coherent starting point rather than a collection of isolated records.

## Contemporary Content That Works Together

The supplied item catalogue now reaches further into the everyday technology of a contemporary or science-fiction setting. It includes practical examples for refrigeration, drying and portable power as well as connected audio/video media, recording, cameras, monitors, speakers, storage and supporting cabling. The catalogue also covers the accompanying A/V and computer implants, so character-facing equipment follows the same media model as installed devices.

Electronic access-control components provide ready-made examples for keypads, biometric readers, keycards, scanners and writers. Clothing and outfit coverage has also been strengthened with missing prototypes, presentation skins and administrative outfit templates, allowing stock examples to show how a complete, intentional look is assembled.

## More Dependable Starts And Replays

Fresh-database setup is more resilient around the bundled blank snapshot. The importer now creates the requested target database before restore and correctly processes appended migration scripts, including MySQL procedure batches. That means a current snapshot can take the fast path without silently losing its migration delta, while the normal migration path remains available whenever a snapshot is unsuitable.

Debug builds now offer guided Medieval, Renaissance and Early Modern replay profiles for a complete known seeder sequence against a fresh local database. The profiles validate their enabled seeders, dependency order and required answers before making changes, making them a useful development and regression tool without exposing the workflow in production builds.

The bundled blank database snapshot is current through `20260830121659_AddMediaRecordingStorage`, so new installations begin from the same database schema baseline as this release and safely fall back to normal migrations whenever that baseline is not applicable.

Use [/downloads](/downloads) for release archives and checksums, and [/getting-started](/getting-started) for installation requirements.
