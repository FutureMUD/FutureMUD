---
title: FutureMUD Database Seeder 3.8.0
summary: Adds the unified optional Armageddon setting package, shared Core tag foundations, new terrain presets and stock support for the accompanying combat and character systems.
date: 2026-10-22
tags: seeder, release, installation, content
---

**Compatibility:** Use Database Seeder 3.8.0 with Engine 2.15.0. Both require .NET 10 and MySQL 8.0. Framework-dependent packages target Windows x64, Linux x64 and Linux ARM64.

## Optional Armageddon setting

The Armageddon Setting installer brings together magic, mundane progression, heritage, cultures, languages and naming. Its separate early skill and combat choices provide the canonical foundations for a new Armageddon world. The package includes seven main guilds, 24 subclasses and 78 naming definitions, with stable ownership that preserves compatible builder edits and player values on supported reruns.

The stock magic installation is available in Release builds and remains optional, defaulting to No. It installs the 152-spell catalogue with its owned supporting content and configured progression. Approved fresh Classes-on installations include Defiler alongside the other magic classes. Optional Land gathering presets support shared regional ecological life and configured local fallback; builders must bind the sources they intend to use.

Seven new playable race profiles include configurable physical, physiological, lifecycle and appearance policies. Their player-admission gates remain closed, and the common Elf parent is nonplayable. Installing the package does not activate those profiles or convert an existing population.

Jihaen, Lirathan and Southern Templar content adds separate class progression, pressure-point attacks, psionic abilities, patron spell selection and reusable shrine, binding-gem, reserve-restorer and item-issuer examples. All three classes start disabled. Southern casting and shrine/gem mechanics also require their separate casting, authority and territory configuration; roles do not automatically grant faction membership or political authority. Some abilities are documented native adaptations, and exhaustive live Lirathan gameplay and balance checks remain outside the recorded qualification.

Historical magic-installer ownership is retained under the renamed Setting installer, preserving existing content references. The early Armageddon skill choice is a new-world alternative that refuses pre-existing player skills; it is not an automatic existing-world conversion.

## Core foundations and terrain presets

Core now installs the shared tag taxonomy previously supplied by Useful: 2,335 distinct paths retain separate identities even when leaf names repeat. This removes Useful as a dependency for packages that only need those shared tags. Existing IDs and builder relationships are preserved where reconciliation is supported; ambiguous legacy graphs are reported. Optional components and templates remain in their owning packages.

New Sewer and Ruins presets use the existing terrain and forage systems. Sewer sludge is descriptive and adds no automatic poisoning, infection or liquid hazard. Ruins offers finite, chance-based stone forage rather than guaranteed resources. The armour-only movement distinction remains deferred.

## Combat and skill content

Curated Backstab attacks cover selected weapons and creatures, including a high-stun Blackjack opening and a Medieval stock item. Fresh Combat installations supply Blind Fighting and the native bindings used by the new melee systems. Dual Wield, Haggle, Analyse eligibility, Peek, Direction Sense and guard reactions use dedicated checks or explicit skill bindings, while supported updates preserve existing values and builder configuration.

General installation does not enable every optional mechanic. The explicit fresh Armageddon choice enables its approved Pilot, Direction Sense, hostile Guarding and Haggling settings after their required foundations resolve; later builder opt-outs survive reruns.

## Installation and existing worlds

The bundled blank database now includes the Room schema and migrations through `20261021023928_ConfigurableRaceAgeDescriptions`, including regional gathering, vehicle barrier exposure and intrinsic racial merits. Earlier blank-snapshot mismatches have been corrected.

Back up the database, application, configuration and external files, stop the engine and other writers, and follow the accompanying engine's managed schema-upgrade process. Installing additional stock content is optional. Review each package's prerequisites and supported update mode; this release does not promise that every seeder can safely rerun over arbitrary builder changes.

Supported stock updates preserve owned identities and compatible customisations, or report conflicts for staff resolution. Stored custom law progs and historical imported content may require deliberate review rather than being replaced automatically. Never run blank-snapshot refresh against a world that must be retained: it drops and recreates its target database.

Representative fresh installations, preserving reruns and first/cold server boots have passing evidence. A populated LabMUD backup also passed the Room upgrade, identity and location preservation checks, full backup restoration after an interrupted structural change, and first/cold engine boots on Windows x64 with MySQL 8.0.45. These checks do not certify every older or customised database; rehearse your own upgrade and recovery before reopening a world.
