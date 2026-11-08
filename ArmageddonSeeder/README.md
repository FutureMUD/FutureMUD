# FutureMUD Armageddon Seeder

Open `ArmageddonSeeder.sln` for setting, worldfile and installer-test development. This independently versioned product contains Armageddon Setting and Armageddon Worldfile. It uses `SeederFramework` for execution/questions/answer memory and `SeederFoundations` for reusable setup packages. Neither installer executable references another executable. Generic foundational source remains under `DatabaseSeeder/Seeders` and is compiled once by SeederFoundations; optional setting policies and their manifests are compiled only here.

## Install

1. Use the matching Database Seeder to create or migrate a database schema. For a new Armageddon world, leave skill, combat and character foundations for the Armageddon host rather than installing an incompatible generic world first.
2. Run `dotnet ArmageddonSeeder.dll`. The host checks schema compatibility before displaying packages or writing answers. Its foundation packages provide the existing Armageddon skill, class, time, celestial and combat choices. Install prerequisites in dependency order, then Armageddon Setting.
3. To import the historical world, supply the separately prepared packet directory through `FUTUREMUD_ARMAGEDDON_DATA_DIRECTORY`, then select Armageddon Worldfile. It asks no questions and remains one-shot. Missing data/prerequisites and an existing worldfile installation are refused without changing the world.

Existing canonical Armageddon installations retain package names, question IDs, ownership keys, progress settings and builder customisations. Moving executables does not authorise conversion of an incompatible generic skill/combat package. Setting repair/resume and one-shot worldfile installation retain their different policies.

Debug offers the maintained `armageddon-native` replay on a freshly migrated, unseeded local database only. The generic Database Seeder profiles cover generic foundations; they no longer include optional Armageddon steps. Product replay and its development connection helpers are absent from Release. Existing native harness prepared-world fixtures have product-local exhaustive replay projections; source-hash receipts must be regenerated when their inputs change.

## Build and package

```
dotnet build ArmageddonSeeder.sln -c Debug -m:1
dotnet test "ArmageddonSeeder Unit Tests/ArmageddonSeeder Unit Tests.csproj" -c Debug -m:1
pwsh -File scripts/Publish-ArmageddonSeeder.ps1 -Runtime win-x64
```

The packaging script supports win-x64, linux-x64 and linux-arm64. It creates a framework-dependent, single-file Release archive and SHA-256 file locally; .NET 10 is required. It never copies the private packet, promotes website downloads, uploads public release assets or sends announcements. Match the seeder with the engine/database schema from the same implementation. No production release is performed by changing this project.

Original ArmSolo source, worldfiles and generated private packets remain external to the repository and executable. Public CI can build/run synthetic and native-configuration checks; corpus-dependent tests explicitly skip when the packet is unavailable. Opt-in local tests can use that environment variable. Do not upload the packet as a CI artifact.
