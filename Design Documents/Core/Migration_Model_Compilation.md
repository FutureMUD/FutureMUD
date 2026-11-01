# Migration model compilation

EF migration designers remain authoritative generated source in `MudsharpDatabaseLibrary/Migrations`. Their full historical models are required when EF generates upgrade SQL. Removing them, replacing them with the current model, or compacting migration history would change the supported upgrade contract.

The build projects their `BuildTargetModel` bodies into `MudSharp.MigrationModels.BatchNNNN` assemblies. `MudsharpDatabaseLibrary` still owns the migration classes, IDs, context attributes, operations and model snapshot. Generated forwarders call the corresponding helper. No custom EF migration discovery service is used, and the two historical designers without model methods remain empty.

`MigrationModels/partitions.json` assigns each designer exactly once. Batches have at most 16 MiB of normalized UTF-8 source. The projects disable parallel project builds and shared compilation to bound compiler memory; direct repository build commands also use `-m:1`. The build tool rejects missing, duplicate and oversized assignments and unsupported designer shapes. Generated projections live beneath each project's configuration/runtime-specific `obj` directory. The compiler tool is a build dependency, not part of the product package; the helper assemblies are runtime dependencies and must be included in published bundles.

## Adding or removing migrations

`MudSharp.sln` must include every migration batch project and `BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj`. Visual Studio's NuGet restore uses loaded solution projects; a command-line restore can succeed with missing solution entries while the IDE reports `NU1105`. Keep these projects loaded and enabled in every solution build configuration, mapping the game-specific Debug configurations to ordinary Debug and all platforms to Any CPU. The same batching optimisation is used by Visual Studio and CI.

Design-time compilation does not build the compiler tool or generate/include historical model projections. Migration operation partials still supply the migration types to IntelliSense; discovery attributes and `BuildTargetModel` implementations are supplied during real builds. This avoids loading hundreds of MiB of historical model source into the editor. Do not use design-time output as a runtime or migration artifact.

The generation targets disable Visual Studio's [fast up-to-date shortcut](https://github.com/dotnet/project-system/blob/main/docs/up-to-date-check.md) for the database library and batch projects. Their external inputs and target-created `Compile` items were otherwise invisible to that shortcut, allowing new forwarders to reference stale helper assemblies. Real builds always reach the generator's content-validated cache; unchanged generated source retains its timestamp, so MSBuild can still skip compilation. Normal project-reference ordering builds helpers before their forwarders. No manually deleted `bin`/`obj` directories are required for normal migration additions.

For everyday Visual Studio use, open `MudSharp.Engine.slnf` (26 loaded projects) or `MudSharp.Development.slnf` (107, including the Seeder and engine). These filters explicitly load the complete dependency tree for NuGet restore, including runtime helpers, while excluding tests, benchmarks, VSIX tooling and unrelated products. The 80 catalogue projects and 20 migration helpers are product dependencies, not CI-only projects. `MudSharp.sln` remains the complete solution and the existing unit-test filters remain available. Counts describe the current tree and will grow with new required partitions.

`BuildInParallel` and `RestoreBuildInParallel` default to false for project-reference walks. Visual Studio schedules solution projects independently of those properties, so the solution also contains a topologically ordered, solution-only dependency chain for the 100 catalogue/migration helper projects. This bounds simultaneous heavy helper compilers without adding assembly references or changing publish dependencies. It trades clean-build parallelism for predictable memory use. Direct command-line and CI builds should continue using `-m:1`.

Refresh/check the filters and helper build ordering after project dependency changes:

```text
python scripts/update-development-solutions.py
python scripts/update-development-solutions.py --check
```

The script uses standard-library Python on Windows and POSIX. CI checks the generated layout on both operating systems. To build the same development selection from the command line:

```text
dotnet build MudSharp.Development.slnf -c Debug -m:1
```

`python scripts/test-build-architecture.py` exercises the actual generation targets with a disposable miniature migration tree after the Debug compiler tool has been built. It verifies clean/cached design-time exclusion, added/changed/deleted migration inputs, missing output repair and unchanged output timestamps. CI runs these probes after the default test gate. They complement the projection tests; they do not measure interactive Visual Studio responsiveness.

Use the normal EF workflow to generate the migration, designer and snapshot. Then refresh the checked-in partition inventory before the next build:

```text
dotnet run --project BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj -- update .
dotnet run --project BuildTools/MigrationModelCompiler/MigrationModelCompiler.csproj -- check .
```

Run these commands from the repository root. Commit the manifest and any generated batch project files with the migration. Add new batches to the solution with `dotnet sln MudSharp.sln add --solution-folder MigrationModels MigrationModels/BatchNNNN/BatchNNNN.csproj`, and verify their configuration mappings. If removing a migration makes a batch obsolete, remove that obsolete batch project as reported by `check` and remove its solution entry with `dotnet sln MudSharp.sln remove MigrationModels/BatchNNNN/BatchNNNN.csproj`. Then run `python scripts/update-development-solutions.py` and commit the solution/filter changes too. Do not manually edit generated historical designers to fit a batch. A single designer exceeding the limit or a new EF designer shape requires an explicit build-tool update and verification.

## Verification and future growth

The persistence suite covers projection preservation and rejected inputs alongside the existing migration and upgrade behavior tests. Packaging changes additionally require a representative single-file publish, checks that build tools do not ship, and native upgrade/startup validation. The initial rollout compares migration discovery, every historical target model, the current context, snapshot and generated SQL with the original unpartitioned assembly.

Batching bounds peak compilation memory. It does not remove historical model metadata or stop total build/package growth. [Issue #815](https://github.com/FutureMUD/FutureMUD/issues/815) tracks a future compaction with an explicit supported upgrade checkpoint, bridge release and recovery path. The current release retains direct historical upgrades.
