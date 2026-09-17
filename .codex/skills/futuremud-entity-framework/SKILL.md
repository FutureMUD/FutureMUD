---
name: futuremud-entity-framework
description: Use when changing FutureMUD Entity Framework code-first models, DbContext mappings, MySQL persistence, EF migrations, or the DatabaseSeeder blank-database snapshot. Generate migrations with dotnet ef, keep migration artifacts and snapshots aligned, and verify model and seed-snapshot parity.
---

# FutureMUD Entity Framework

Make code-first model and mapping changes before creating a migration. Treat the generated migration, its designer, the EF model snapshot, and the DatabaseSeeder blank snapshot as one release unit.

## Read first

- Read the repository `AGENTS.md`, `MudsharpDatabaseLibrary/AGENTS.md`, the affected project instructions, and its design document.
- Keep persistence-only concerns in `MudsharpDatabaseLibrary`; put game behaviour in the owning runtime project.
- Use the existing partial-context layout: `FuturemudDatabaseContext.cs` owns `DbSet`s, while `Database/FuturemudDatabaseContextConfiguring*.cs` and focused partial files own mapping. Follow the nearest table's pattern.
- Preserve table names, composite keys, revisions, delete behaviour, charset/collation, and value generation unless intentionally migrating them. Do not assume an `Id` is database-generated; inspect its mapping.
- Update relevant runtime load/save, builder/API surfaces, design docs, and tests when a persisted contract changes.

## Code-first workflow

1. Change the model class, `DbSet`, and mapping first. Make nullability and defaults explicit so existing rows can upgrade safely.
2. Add or adapt focused persistence/runtime tests before generating the migration when practical.
3. Build the current model assembly. Never run EF against stale `--no-build` output.

```powershell
dotnet build MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
```

4. Check model parity. A non-zero result is expected only when the intentional model change still needs a migration.

```powershell
dotnet ef migrations has-pending-model-changes --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build
```

5. Generate the migration; never hand-write a migration, matching designer, or `FutureMUDContextModelSnapshot.cs`.

```powershell
dotnet ef migrations add DescriptivePascalCaseName --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build
```

6. Review generated `Up` and `Down`, then confirm all three artifacts changed together: `Migrations/<timestamp>_Name.cs`, `<timestamp>_Name.Designer.cs`, and `FutureMUDContextModelSnapshot.cs`. The designer must retain its `DbContext` and `Migration` attributes.
7. Re-run `has-pending-model-changes`; it must report no changes. Use `dotnet ef migrations list` if discovery is in doubt.

Use `dotnet ef migrations remove` only for the last un-applied migration. First rebuild, inspect `git status`, and confirm it is the newest migration: stale `--no-build` output can remove the wrong uncommitted migration.

Use `dotnet ef database update` only for an explicitly authorised disposable/development database. Never print or commit connection strings. If the tool is missing, install official `dotnet-ef`; do not fabricate generated files.

## DatabaseSeeder blank snapshot

Every migration that changes a clean database must be represented in `DatabaseSeeder/BlankDatabaseSnapshot.sql` and `BlankDatabaseSnapshot.manifest.json`.

Prefer a full refresh after migration and model-parity checks. The refresher recreates its snapshot database, so use a dedicated disposable database only—never a game database. Set `FUTUREMUD_SNAPSHOT_CONNECTION_STRING` outside source control when the default local snapshot connection is unsuitable; never echo its value.

```powershell
dotnet run --project DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-build -- --refresh-blank-snapshot
```

Verify that the manifest's `LatestMigrationId` equals the latest discovered migration, the SQL records it in `__efmigrationshistory`, the required schema is present, and table names added to the maintained MySQL dump use its lower-case casing. Confirm the manifest product version matches `DatabaseSeeder.csproj` and its timestamp reflects a real refresh or verified update.

If local MySQL TLS/SSPI or `auth_gssapi_client` blocks a full refresh, do not weaken machine-global authentication or rewrite EF migrations. Generate and review an idempotent script for the exact missing range, append only the reviewed delta with compatible lower-case table identifiers, and update the manifest only once the dump contains the target migration.

```powershell
dotnet ef migrations script FromMigrationId ToMigrationId --idempotent --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj --no-build --output snapshot-delta.sql
```

Remove temporary scripts when they are no longer needed; do not commit them unless intentionally maintained.

## Verification

Run focused snapshot tests, the whole fast gate for cross-cutting persistence work, and a whitespace check:

```powershell
dotnet test 'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1 --filter 'FullyQualifiedName~BlankDatabaseSnapshotTests' -p:NoWarn=NU1902%3BNU1510
scripts/test-unit.ps1
git diff --check
```

Report model parity, migration generation, snapshot refresh/import, and tests separately. A migration list needs no database connection; applied status and snapshot-import verification do, and must not be claimed when unavailable.
