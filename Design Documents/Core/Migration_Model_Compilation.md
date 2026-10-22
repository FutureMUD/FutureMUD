# Migration model compilation

EF migration designers remain authoritative generated source in `MudsharpDatabaseLibrary/Migrations`. Their full historical models are required when EF generates upgrade SQL. Removing them, replacing them with the current model, or compacting migration history would change the supported upgrade contract.

The build projects their `BuildTargetModel` bodies into `MudSharp.MigrationModels.BatchNNNN` assemblies. `MudsharpDatabaseLibrary` still owns the migration classes, IDs, context attributes, operations and model snapshot. Generated forwarders call the corresponding helper. No custom EF migration discovery service is used, and the two historical designers without model methods remain empty.

`MigrationModels/partitions.json` assigns each designer exactly once. Batches have at most 16 MiB of normalized UTF-8 source. The projects disable parallel project builds and shared compilation to bound compiler memory; direct repository build commands also use `-m:1`. The build tool rejects missing, duplicate and oversized assignments and unsupported designer shapes. Generated projections live beneath each project's configuration/runtime-specific `obj` directory. The compiler tool is a build dependency, not part of the product package; the helper assemblies are runtime dependencies and must be included in published bundles.

## Adding or removing migrations

Use the normal EF workflow to generate the migration, designer and snapshot. Then refresh the checked-in partition inventory before the next build:

```text
dotnet run --project BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj -- update .
dotnet run --project BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj -- check .
```

Run these commands from the repository root. Commit the manifest and any generated batch project files with the migration. If removing a migration makes a batch obsolete, remove that obsolete batch project as reported by `check`. Do not manually edit generated historical designers to fit a batch. A single designer exceeding the limit or a new EF designer shape requires an explicit build-tool update and verification.

## Verification and future growth

The persistence suite covers projection preservation and rejected inputs alongside the existing migration and upgrade behavior tests. Packaging changes additionally require a representative single-file publish, checks that build tools do not ship, and native upgrade/startup validation. The initial rollout compares migration discovery, every historical target model, the current context, snapshot and generated SQL with the original unpartitioned assembly.

Batching bounds peak compilation memory. It does not remove historical model metadata or stop total build/package growth. [Issue #815](https://github.com/FutureMUD/FutureMUD/issues/815) tracks a future compaction with an explicit supported upgrade checkpoint, bridge release and recovery path. The current release retains direct historical upgrades.
