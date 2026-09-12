# Persistence-library instructions

Inherits [repository instructions](../AGENTS.md). This project owns the EF Core context, models, mappings and migrations for the FutureMUD MySQL database.

## Boundaries

- Model classes map database tables; keep gameplay logic out of this project.
- Extend generated models with partial classes rather than changing scaffolded output unnecessarily.
- `FuturemudDatabaseContext` configures MySQL and lazy-loading proxies. Connection strings come from runtime configuration; never commit credentials.

## When a schema/migration change is required

Use EF tooling to generate migrations, designers and the model snapshot. Do not hand-roll any of these artifacts.

Use the repository's pinned/local EF tool when available, or an installed tool compatible with the checked-out EF packages. Check the declared versions before installing or updating a missing tool; do not install an unpinned global latest version as an automatic first step. Respect environment permissions. If tooling cannot be made available, report the blocked generation/verification rather than fabricating migration artifacts.

From the repository root:

```text
dotnet ef migrations add <MigrationName> --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj
dotnet ef migrations remove --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj
```

Use `remove` only for the last unapplied migration. After generating/removing a migration, verify the migration `.cs`, corresponding `.Designer.cs`, and `FutureMUDContextModelSnapshot.cs` are mutually consistent, including intended additions/deletions. Review generated operations for the intended data and compatibility effects.

Applying a migration is a separate database mutation, not a required step for a read-only review or every model edit. When the task calls for updating a verified development database outside normal startup, use:

```text
dotnet ef database update --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj
```

Verify the target connection first. Do not apply migrations to an unknown/shared/production target merely to complete local verification. Use `MudsharpDatabaseLibrary Unit Tests` for isolated persistence/upgrade behaviour; report any live-database checks not performed.

## Economy persistence

For economy model/migration changes, consult/update the affected sections of `Design Documents/Economy/Economy_System_Runtime.md` and `Economy_System_Seeder_State_and_Gaps.md` (repository-relative). This covers currencies, banks, economic zones, taxes, markets, shoppers, shops, property, auctions, employment, and future estate persistence. Unrelated persistence changes do not require reading the economy documents.
