---
title: FutureMUD Database Seeder 3.5.1
summary: Fixes installation failures caused by existing static settings and Human Seeder water references.
date: 2026-09-06
tags: seeder, release, bugfix
---
**Compatibility:** Use Database Seeder 3.5.1 with Engine 2.12.1. The bundled blank database snapshot retains the current migration baseline; this update introduces no schema changes.

## Core Installation

Core seeding now preserves static configuration values already supplied by database migrations or the blank snapshot and adds only missing defaults. This prevents duplicate-setting collisions during installation.

## Human Installation

Human blood, sweat and their dried residues now resolve their solvent from the stock water liquid by name instead of assuming that water has database ID 1. This fixes installation failures when water has a different ID. If the required water liquid is missing, the seeder gives an explicit instruction to run Core Data first.

Use [/downloads](/downloads) for archives and checksums and [/getting-started](/getting-started) for installation instructions.
