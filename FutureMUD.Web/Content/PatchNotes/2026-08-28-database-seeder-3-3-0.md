---
title: Database Seeder 3.3.0
summary: Expands the stock world with reusable NPC skill packages, a modern combat catalogue and far broader terrain foundations, while making supported reruns more resilient.
date: 2026-08-28
tags: seeder, release, upgrade, content, npcs
---
**Upgrade note:** Database Seeder 3.3.0 targets .NET 10. Apply the normal database upgrades before seeding, and update to Engine 2.9.0 to use the accompanying runtime support.

## More Capable NPC Foundations

The stock catalogue now gives builders a practical starting point for the Engine's reusable NPC Skill Packages. Fresh worlds receive the combat traits and dependencies that package checks need, while established worlds can rerun the supported package content without losing the intended ownership boundaries. The profiles also account for the more complex physical and athletic combinations used by RPI-style characters, so a package can describe a believable role rather than merely a flat list of skills.

## From Bowstrings To Breech Loaders

The combat catalogue has been widened and rebalanced as a coherent toolkit. It now includes fourteen modern firearm archetypes alongside breech-loading howitzers and mortars, ammunition, explosives, modern armour, attachments and cover. Builders have a much more useful set of examples for a modern or mixed-era world, while the updated primitive and melee foundations keep the catalogue internally consistent.

## Terrain That Describes More Worlds

The stock terrain catalogue now reaches beyond ordinary outdoor ground. It includes vehicle, ship and spaceship settings, global and extraterrestrial biomes, and supernatural spaces such as astral, fae, shadow, celestial, infernal and dream terrain. The existing terrain entries were also reviewed for presentation colour, editor colour and code, movement, stamina, concealment, tracking and behavioural metadata, so the expanded choices read and behave like a maintained catalogue rather than a loose collection of labels.

## Safer Supported Reruns

Several foundational reconciliation paths now handle their real-world edge cases more carefully. Core material tags resolve through their intended hierarchy, stock terrain atmospheres can be repaired without conflicting database readers, and skill-package checks resolve the persisted skills already present in an established world. These improvements keep supported reruns additive and predictable without recreating canonical tags or overwriting unrelated builder content.

The bundled blank database snapshot is current through `20260828014622_AddNPCSkillPackages`, so a new installation begins from the same schema baseline as this release and safely falls back to normal migrations whenever that baseline is not applicable.

Use [/downloads](/downloads) for release archives and checksums, and [/getting-started](/getting-started) for installation requirements.
