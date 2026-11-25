---
title: FutureMUD Database Seeder 3.10.0
summary: Adds 32 Catholic clan templates, updates first-account setup and prepares new worlds for Engine 2.18.0.
date: 2026-11-25
tags: seeder, release, clans
---

Use .NET 11 and MySQL 8.0 with Engine 2.18.0. Fresh worlds start from a blank database snapshot that includes the new Engine migrations.

- The Clan package adds 32 Catholic templates covering institutions such as churches, dioceses, parishes and religious orders. Builders can stage a copy, choose its governance profile, bind it to the live institutions in their setting and activate it when ready. Templates also support forms of address for ranks and offices.
- First-account setup uses the Engine’s new passphrase policy and Argon2id storage. Enter `none` for the development implementor’s email address if the world runs without email. New password-change email templates omit the password.
- Setting authors can supply reusable published performances and rites through the shared seeder helper. It checks the work and its eligibility rules before installation.
