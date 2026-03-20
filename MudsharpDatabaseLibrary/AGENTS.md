# Scope

This document defines the **specific rules for Project MudSharpDatabaseLibrary**.  
It inherits from the [Solution-Level AGENTS.md](../AGENTS.md).

## Inheritance

* Rules here **extend or override** solution-level rules.
* Precedence: **Module > Project > Solution**.

## Purpose of Project
Houses the Entity Framework Core `DbContext`, models, and migrations for the FutureMUD database.

## Key Architectural Principles
* Model classes map directly to database tables; avoid introducing game logic here.
* Use partial classes to extend generated models without altering scaffolded code.
* `FuturemudDatabaseContext` configures MySQL with lazy-loading proxies.
* Migrations reside in the `Migrations/` folder and should be created via EF Core tooling.
* Connection strings are provided at runtime—do not commit credentials.

## Migration Workflow

* Never hand-roll EF migrations, migration designers, or the model snapshot. Always use the EF tooling so the migration `.cs`, matching `.Designer.cs`, and `FutureMUDContextModelSnapshot.cs` stay in sync.
* If `dotnet ef` is not available on the machine, install it first with `dotnet tool install --global dotnet-ef`.
* From the repository root, create migrations with `dotnet ef migrations add <MigrationName> --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj`.
* Remove the last un-applied migration with `dotnet ef migrations remove --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj`.
* Apply migrations explicitly with `dotnet ef database update --project MudsharpDatabaseLibrary/MudsharpDatabaseLibrary.csproj --startup-project MudSharpCore/MudSharpCore.csproj` when you need to update a development database outside the normal application startup path.
* After generating or removing a migration, verify that all three artifacts changed together: the migration `.cs`, the matching `.Designer.cs`, and `FutureMUDContextModelSnapshot.cs`.

## Notes

* All modules inherit both the solution-level and project-level rules unless explicitly overridden.
