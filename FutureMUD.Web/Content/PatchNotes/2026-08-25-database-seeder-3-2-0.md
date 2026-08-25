---
title: Database Seeder 3.2.0
summary: Expands ready-to-run restaurant, wildlife, trap and combat starting content, with a current blank database snapshot.
date: 2026-08-25
tags: seeder, release, upgrade, content
---
**Upgrade note:** Database Seeder 3.2.0 targets .NET 10. Install the .NET 10 runtime before using the framework-dependent download.

## A More Complete Living-World Catalogue

The stock animal catalogue now provides practical wildlife and managed-group foundations: habitat-aware animals, coordinated social groups, shelters, deterministic race recommendations and durable ecology metadata. Builders can begin with useful wild and managed examples rather than constructing every behavioural profile from scratch.

Animal, mythical and supernatural catalogues now share a reviewed combat foundation. Stock creatures receive consistent physical attributes, pain tolerance, armour, bodypart weights, natural attacks and signature actions appropriate to their role. Rerunning the owned stock package reconciles those maintained fields without overwriting builder-created content.

## Playable Sites And Physical Hazards

Restaurant-supporting seed content now accompanies the Engine's staffed dine-in and takeaway workflows, giving new worlds practical starting definitions for food preparation and service.

The seeder also includes the complete physical trap foundation: templates, triggers, ordered payloads, component dependencies and spent-item handling for mechanical, magical and natural hazards. This makes the Engine's regular `traptemplate` and `trap` workflow usable from a new installation without hand-assembling its prerequisites.

## Installation And Maintenance

The bundled blank database snapshot is current through `20260825022721_AddRacePainToleranceMultiplier`, allowing a clean installation to begin from the maintained schema baseline. It remains guarded by migration validation, so the installer falls back safely to normal migrations whenever a snapshot is not current.

Stock package dependencies, idempotent reconciliation and clearer prerequisite diagnostics continue to keep the catalogue suitable both for new worlds and for maintainers adding supported content to an existing world.
