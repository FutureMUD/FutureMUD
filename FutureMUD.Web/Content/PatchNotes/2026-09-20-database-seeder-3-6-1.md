---
title: FutureMUD Database Seeder 3.6.1
summary: Expands historical culture and accent stock, improves safe reruns, and updates the blank database for Engine 2.13.0.
date: 2026-09-20
tags: seeder, release, culture, languages, installation
---

**Compatibility:** Use Database Seeder 3.6.1 with Engine 2.13.0. Its bundled blank database snapshot includes the latest migration for Land gathering source accounting.

## Historical Cultures, Languages and Accents

The CultureSeeder now installs source-preserving historical language, script, naming and accent material across its supported eras. Native-language roles are tied to the appropriate ethnicity and era, while learned languages use eligible learner or foreign accents. Targeted name pools have broader playable coverage. Reruns reconcile recognised stock-owned records and retain builder changes, including custom associations, names and accent settings; unresolved source bindings are reported instead of guessed.

## Installation and Stock Content

Seeder-owned files and database assets are packaged with their owning seeders while discovery remains automatic. The installer gives clearer prerequisite and rerun information, and Debug replay profiles validate the enabled seeder order and answers before changing a database. Renaissance outfits now include their required shared Medieval clothing dependencies. The bundled blank database snapshot is aligned with the current Engine schema.

The later era ItemSeeder catalogue has gained planning, dependency and validation groundwork, but Industrial and subsequent ordinary-item packages remain unavailable for selection until their stock passes the activation gates.

Use [/downloads](/downloads) for archives and checksums and [/getting-started](/getting-started) for installation instructions.
