# FutureMUD verification and documentation map

Paths and commands below are relative to the repository root. Read the section needed for the task; this is not a checklist to execute in full.

## Choose the test owner

| Suite | Run for |
| --- | --- |
| `FutureMUDLibrary Unit Tests` | Shared extensions, helpers, abstractions and value objects. |
| `ExpressionEngine Unit Tests` | Parser, custom functions and formula semantics. |
| `DatabaseSeeder Unit Tests` | Seeded content, templates, repeatability, workflow and source invariants. |
| `MudSharpCore Unit Tests` | Runtime/gameplay, FutureProg, AI, arenas and engine services. |
| `MudsharpDatabaseLibrary Unit Tests` | EF models/mappings, upgrade coordination and persistence helpers. |
| `DiscordBotCore Unit Tests` | Bot commands, protocol and message formatting. |
| `RPI Engine Worldfile Converter Tests` | Legacy formats, conversions, validation and fixture compatibility. |
| `FutureMUD.Web.Tests` | Website endpoints, release/documentation publishing and security boundaries. |
| `MudSharpCore Climate Tests` | Slow seeded-weather and climate/analyzer regressions; opt-in, not part of the normal fast pass. |

Run dependent suites when the change crosses their contracts. A shared-helper change can warrant runtime tests; an isolated shared helper does not automatically require a whole-engine boot. Seeder and release skills retain their specific required gates.

## Commands and environment

Use Windows-native `dotnet` or the PowerShell scripts on Windows; use the paired shell scripts on POSIX. Inspect a script before supplying flags it does not declare.

Restore the affected project/filter when its packages are not already available. For a broad solution restore:

```text
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
```

The audit switch is a permitted local workaround for blocked NuGet vulnerability lookups, not evidence that dependencies are safe. Do not disable auditing for dependency/security-audit work.

Examples after a successful restore:

```text
dotnet build MudSharpCore/MudSharpCore.csproj -c Debug --no-restore -m:1
dotnet build DatabaseSeeder/DatabaseSeeder.csproj -c Debug --no-restore -m:1
dotnet test "DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj" -c Debug --no-restore -m:1
dotnet test "MudSharpCore Climate Tests/MudSharpCore Climate Tests.csproj" -c Debug --no-restore -m:1 --filter WeatherSeederClimateTests
```

Select the command for the affected product; these examples do not require building both products for every edit.

| Windows script | Purpose | POSIX equivalent |
| --- | --- | --- |
| `scripts\test-unit.ps1` | Broad fast unit-test pass; excludes the dedicated climate suite. | `scripts/test-unit.sh` |
| `scripts\test-unit-core.ps1` | Core runtime tests only. | `scripts/test-unit-core.sh` |
| `scripts\test-unit-climate.ps1` | Dedicated slow climate suite. | `scripts/test-unit-climate.sh` |
| `scripts\test.ps1` | Smoke-build path. | `scripts/test.sh` |
| `scripts\setup.ps1` | Repo-local SDK bootstrap, only when needed. | `scripts/setup.sh` |

The test scripts use `--no-restore`; ensure the relevant restore has succeeded first. Ordinary source/documentation edits do not justify an SDK bootstrap.

Sandbox issues to recognise:

- `FutureMUD_Analyzers.Vsix` requires Visual Studio extension targets; prefer targeted builds when those targets are absent.
- Parallel MSBuild graph walks can fail in sandboxed runs. Keep `-m:1` and, for restores, `-p:RestoreBuildInParallel=false`.
- `NU1900` can indicate blocked audit access rather than a compilation error. The repository also permits local `-p:NoWarn=NU1902%3BNU1510` outside package-audit work. Keep the scope local, disclose a relevant suppression, and do not commit a blanket warning suppression to make verification look clean.
- An environment/tooling failure is not a successful build. Report the exact failed check and what remains unverified.

For prose/skill metadata changes, check the diff, referenced paths and any YAML frontmatter/metadata. Use an available validator when applicable; do not assume a machine-specific skill-validator script exists. `git diff --check` is useful for all text edits.

## Design-document routing

Use `Design Documents/README.md` to find an unknown owner; go directly to a known document. Update documentation when the described contract changes, without rewriting unrelated sections.

### Items

Under `Design Documents/Items/`:

| Document | Read/update for |
| --- | --- |
| `Item_System_Overview.md` | Cross-cutting architecture and terminology. |
| `Item_System_Runtime_Model.md` | Runtime item lifecycle and state. |
| `Item_System_Component_Authoring.md` | Component implementation and authoring contracts. |
| `Item_System_Content_Workflows.md` | Builder workflows, templates, revisions, skins and groups. |
| `Item_System_Presentation_and_Integration.md` | Presentation and connections to other systems. |

This includes `MudSharpCore/GameItems`, `FutureMUDLibrary/GameItems`, item builder helpers/commands, item-related FutureProg functions, skins, groups and `Item Templates/`. Cross-cutting changes may need several documents, but a narrow edit does not require all five.

### Economy

Under `Design Documents/Economy/`:

| Document | Read/update for |
| --- | --- |
| `Economy_System_Runtime.md` | Runtime/domain contracts and persistence semantics. |
| `Economy_System_Workflows_and_Integration.md` | Commands, shared interfaces, FutureProg and other integrations. |
| `Economy_System_Seeder_State_and_Gaps.md` | Stock seeding, ownership, migrations/update implications and product gaps. |

This includes economy namespaces, editable-item helpers, related components/effects, persistence and seeder content. Apply the more specific project instructions as well.
