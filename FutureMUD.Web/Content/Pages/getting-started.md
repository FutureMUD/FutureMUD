---
title: Getting started
summary: From a release archive to your first local FutureMUD world.
---
## 1. Choose a supported runtime

Download the Engine and Database Seeder for your operating system from the downloads page. Current packages target Windows x64, Linux x64, and Linux ARM64. The archives are framework-dependent: Engine 1.55.0 requires .NET 10 and Database Seeder 2.3.0 requires .NET 9.

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

For development builds or source contributions, clone the [FutureMUD repository](https://github.com/FutureMUD/FutureMUD) and use the repository setup and test scripts.

On Linux, preserve or restore the Engine app host's executable bit after extracting an archive: `chmod +x MudSharp`.
