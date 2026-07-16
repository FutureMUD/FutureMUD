---
title: Getting started
summary: From a release archive to your first local FutureMUD world.
---
## Minimum requirements

- **.NET 10 runtime.** Follow the Microsoft Learn installation guides for [Windows](https://learn.microsoft.com/en-us/dotnet/core/install/windows) or [Linux](https://learn.microsoft.com/en-us/dotnet/core/install/linux).
- **MySQL Server 8.0.** Use a dedicated database and account for the game.
- **An email server.** FutureMUD needs access to an email server through which it can send account and system email.
- **Approximately 1 GB of disk space.** SSD or server-grade storage is preferred.
- **Approximately 1 GB of RAM.** Larger or busier worlds may require more memory and storage.

These figures are a practical baseline for a small installation. Capacity needs grow with world size, logging, backups, and player load.

## 1. Choose a supported runtime

Download the Engine and Database Seeder for your operating system from the downloads page. Current packages target Windows x64, Linux x64, and Linux ARM64. Archives are framework-dependent and require the appropriate .NET runtime.

## 2. Prepare MySQL

Create an empty MySQL database and a dedicated database user. Keep the connection credentials outside source control and restrict the account to the database used by the game.

## 3. Run the Database Seeder

The seeder asks how you want the initial world configured, creates the database content, and writes supporting startup configuration. You can run it again later to install additional supported content.

## 4. Start the Engine

Launch MudSharp from the generated working directory. The engine applies supported schema migrations, loads the world, and listens on the configured TCP address. Connect with a MUD client and follow the administrator and builder help.

## Continue with the reference

- Browse the command reference for player, builder, and administrator syntax.
- Use the FutureProg function and type references while scripting.
- Consult item-component help when authoring item prototypes.
- Review the patch notes before upgrading an existing installation.

For source inspection and release provenance, browse the public [FutureMUD repository](https://github.com/FutureMUD/FutureMUD). Public visibility does not make the project open source or grant contribution or redistribution permission beyond the license. Discuss bugs, support questions, or proposed changes on the [FutureMUD Discord](https://discord.gg/fyKnckr4PG).

On Linux, preserve or restore the Engine app host's executable bit after extracting an archive: `chmod +x MudSharp`.
