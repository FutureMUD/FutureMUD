# NuGet dependency maintenance

## Scope and version selection

The 18 November 2026 inventory covers the engine, shared migration props, standalone products, developer tools, native harnesses, and test projects. The legacy Configuration Tool uses classic NuGet restore. The historical projects in `MigrationBackup` are retained as archived inputs.

Select the newest listed stable release that supports the consuming framework and host. Check NuGet registration for deprecation and listing status, then audit the resolved graph, including transitive dependencies. The package download index also contains unlisted releases.

## Compatibility decisions

- **Entity Framework.** Use EF Core 11.0.0, Microting.EntityFrameworkCore.MySql 11.0.0 and MySqlConnector 3.0.0 together. The provider specifies EF Relational 11.0.x. Keep the migration batches, in-memory tests and dotnet-ef tool on the same EF release.
- **OpenAI and email.** The engine uses OpenAI 2.15.0 and MailKit 5.0.0. Drive, Gmail and OAuth clients use the 1.78.0 Google API libraries.
- **Discord.** Keep the DSharpPlus family on 4.5.3. The 5.0.0 packages are unlisted. NuGet marks the core package and several extensions as deprecated for critical bugs.
- **Tests.** MSTest uses adapter and framework 4.5.3 with Microsoft.NET.Test.Sdk 18.12.0. Terrain Planner uses exact exception assertions and data rows on ordinary test methods. MudClient uses xunit.v3.mtp-off 4.0.3 with the Visual Studio adapter to preserve the existing VSTest entry point.
- **NCalc.** NCalcSync 7.4.0 supplies the expression engine. Custom functions evaluate arguments through `FunctionData.Evaluate(index)` to preserve the calling context.
- **Roslyn and Visual Studio.** The migration compiler and seeder source tests use Roslyn 5.12.0 with .NET SDK 11.0.100. The analyzer, code fixes and their net48 harness use 5.9.0 so that Visual Studio 18.10, which hosts Roslyn 5.10, can load them. Keep the optional VSIX minimum at 18.10.
- **Platform libraries:** The net11.0 engine no longer references the separate System.ComponentModel.Annotations, System.Data.DataSetExtensions or System.ValueTuple packages; the framework supplies these APIs.
- **MySQL client:** Connector/NET removed the deprecated `SslMode=None` value. Local Debug connection templates and smoke harnesses now use `SslMode=Disabled`, preserving their existing no-TLS development setup and compatibility with both MySql.Data and MySqlConnector. The upgrade/backup client boundary also accepts the legacy `None` spelling in existing configuration, parsing connection-string options before translating it to `Disabled`. Other TLS modes, the driver's default, and quoted credentials are preserved; saved configuration is not rewritten.
- **Upgrade service:** Swashbuckle 10 uses the Microsoft.OpenApi namespace and document-bound security references. OAuth scope requirements are preserved. The unused Microsoft.AspNetCore.OpenApi package was removed; this service uses Swashbuckle's pipeline rather than AddOpenApi/MapOpenApi.
- **Legacy Configuration Tool.** Its .NET Framework project references Entity Framework 6.5.2. The unused iTextSharp dependency was removed.

Migration batches reference the same EF Relational version as the main persistence library. The optional Item Seeder manifest captures its project and source fingerprint. After this update an in-memory recapture reproduced all 43,825 entries, including their ownership fingerprints.

## Rechecking security and currency

Restore with `NuGetAudit=true` and `NuGetAuditMode=all`, then inspect both direct and transitive packages with `dotnet list <solution-or-project> package --vulnerable --include-transitive --no-restore`. Run `--outdated` separately to check direct package currency. An audit fetch failure is an unknown result, not evidence of no vulnerabilities. Do not disable auditing or suppress NU190x warnings for this task.

The website security workflow also pins the approved BouncyCastle.Cryptography version in its dependency-boundary check. Update that exact-version approval alongside the website package reference; it now permits 2.7.0. The engine/database exclusions and enforced NuGet vulnerability audit remain active.

Open `MudSharpCore.ClimateTests.slnf` for the climate regressions. The filter uses `ArmageddonSeeder.sln` because the suite exercises the optional Armageddon weather installer alongside the shared climate models. Both climate test scripts still target the same project.

The classic VSIX project needs Visual Studio's MSBuild and installed VSSDK targets. For CLI package inventory, use an equivalent temporary solution containing the SDK projects; the VSIX project itself has no NuGet references. On a restricted Windows account, a writable per-run `NUGET_HTTP_CACHE_PATH` can avoid HTTP-cache access failures without changing package sources or credentials.

## Verification, 18 November 2026

- All 168 active SDK projects built in Debug on Windows. NuGet's direct and transitive vulnerability check reported no findings across that inventory. The classic Configuration Tool restored EF 6.5.2; its missing legacy common-library project and existing XAML code-behind compilation errors still prevent building the application.
- EF reported no pending model changes and generated idempotent SQL containing all 298 historical migrations. A disposable MySQL 8.0.45 instance imported the committed blank snapshot. EF then rolled back the latest migration, reapplied it, and repeated the update. The migration history matched the expected IDs after every step.
- The existing climate filter ran all 12 Armageddon weather regression cases successfully. The extended persistence suite passed 368 tests. MudClient passed 136, Terrain Planner passed 39, and the opt-in reporting process tests passed 32. The Item Seeder fast suite passed 499 tests. The Armageddon Seeder fast suite passed 435 and skipped 98.
- Live OpenAI, Google, email, and Discord service calls were outside this verification.

## Verification, 24 October 2026

- Visual Studio MSBuild completed the full Debug solution build. Focused native test-project builds also passed; the final identity-assertion updates were subsequently compiled and tested through the CLI.
- A cold Visual Studio design-time build, with project-reference building disabled, created the migration compiler and 23 generated model files in a fresh configuration.
- The full ten-project fast test run exposed seven mock-identity failures, 32 sandbox temporary-directory failures, and one stale manifest fingerprint. After the assertion/fingerprint fixes and using a writable workspace-local `TEMP`/`TMP`, the four affected suites passed all 9,272 cases and the focused manifest/replay check passed all 20 cases. Combined with the unaffected full-run results, all 11,937 executed cases passed; three Unix filesystem-permission cases were skipped on Windows. The full run and both focused reruns each recorded stable source. Original failure evidence is retained locally.
- The DatabaseSeeder Release build passed with zero errors; existing compiler warnings remain.
- EF's pending-model check reported no model changes. The local Swagger endpoint returned OpenAPI 3.0 with the OAuth2 definition and all three required scopes intact.
- The final direct/transitive vulnerability audit covered 124 SDK projects and reported zero known vulnerable packages and no audit errors. At the time of the October audit, the Pomelo dependency limited EF Core to 9.0.x.

This verification used Windows and native Visual Studio MSBuild, not an interactive IDE session. Live MySQL replay, external provider API calls, VSIX installation, Unix-only behavior, and hosted CI were not exercised.

## Sources

- [Microting MySQL provider dependencies](https://www.nuget.org/packages/Microting.EntityFrameworkCore.MySql/11.0.0)
- [MSTest v4 migration](https://learn.microsoft.com/dotnet/core/testing/unit-testing-mstest-migration-v3-v4)
- [NCalc v7 migration](https://github.com/ncalc/ncalc/blob/master/docs/articles/project/migrate_v6_to_v7.md)
- [Swashbuckle v10 migration](https://github.com/domaindrivendev/Swashbuckle.AspNetCore/blob/master/docs/migrating-to-v10.md)
- [MySQL Connector/NET 9.6 breaking changes](https://dev.mysql.com/doc/relnotes/connector-net/en/news-9-6-0.html)
- [Cryptography advisory GHSA-2fmv-rv2c-59x7](https://github.com/advisories/GHSA-2fmv-rv2c-59x7)
- [Cryptography advisory GHSA-846f-f764-76mx](https://github.com/advisories/GHSA-846f-f764-76mx)

## Direct-package inventory, 24 October 2026

| Package | Before | Selected |
| --- | --- | --- |
| Anthropic.SDK | 5.8.0 | 5.10.0 |
| BenchmarkDotNet | 0.15.8 | 0.15.8 |
| BenchmarkDotNet.Annotations | 0.15.8 | 0.15.8 |
| BouncyCastle.Cryptography | 2.6.2 | 2.7.0 |
| C5 | 3.0.0 | 3.0.0 |
| Dapper | 2.1.66 | 2.1.89 |
| DSharpPlus | 4.5.0 | 4.5.3 |
| DSharpPlus.CommandsNext | 4.5.0 | 4.5.3 |
| DSharpPlus.Interactivity | 4.5.0 | 4.5.3 |
| DSharpPlus.Rest | 4.5.0 | 4.5.3 |
| Google.Apis.Drive.v3 | 1.73.0.3996 | 1.77.0.4301 |
| Google.Apis.Gmail.v1 | 1.74.0.4162 | 1.77.0.4295 |
| Google.Apis.Oauth2.v2 | 1.68.0.1869 | 1.74.0.1869 |
| Humanizer | 3.0.1 | 3.0.10 |
| JetBrains.Annotations | 2026.2.0 | 2026.2.0 |
| MailKit | 4.17.0 | 4.18.2 |
| Microsoft.AspNetCore.Authentication.Google | 7.0.5 | 10.0.13 |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.0 | 10.0.13 |
| Microsoft.AspNetCore.OpenApi | 7.0.2 | Removed |
| Microsoft.CodeAnalysis.Analyzers | 3.11.0 | 5.9.0 |
| Microsoft.CodeAnalysis.CSharp | 4.13.0 | 5.9.0 |
| Microsoft.CodeAnalysis.CSharp.Workspaces | 4.13.0 | 5.9.0 |
| Microsoft.EntityFrameworkCore | 9.0.11 | 9.0.21 |
| Microsoft.EntityFrameworkCore.Design | 9.0.11 | 9.0.21 |
| Microsoft.EntityFrameworkCore.InMemory | 9.0.11 | 9.0.21 |
| Microsoft.EntityFrameworkCore.Proxies | 9.0.11 | 9.0.21 |
| Microsoft.EntityFrameworkCore.Relational | 9.0.11 | 9.0.21 |
| Microsoft.EntityFrameworkCore.Tools | 9.0.11 | 9.0.21 |
| Microsoft.Extensions.DependencyInjection | 10.0.9 | 10.0.13 |
| Microsoft.NET.Test.Sdk | 17.9.0 | 18.11.0 |
| Moq | 4.20.70, 4.20.72 | 4.21.0 |
| morelinq | 4.4.0 | 4.4.0 |
| Mscc.GenerativeAI | 1.5.1 | 3.1.0 |
| Mscc.GenerativeAI.Google | 1.5.1 | 3.1.0 |
| MSTest.TestAdapter | 2.2.10 | 4.5.1 |
| MSTest.TestFramework | 2.2.10 | 4.5.1 |
| MySql.Data | 9.3.0 | 26.10.0 |
| MySqlBackup.NET | 2.6.5 | 2.7.1 |
| NCalcSync | 6.4.0 | 7.2.0 |
| Newtonsoft.Json | 13.0.4 | 13.0.4 |
| OpenAI | 2.13.0 | 2.14.0 |
| Pomelo.EntityFrameworkCore.MySql | 9.0.0 | 9.0.0 |
| Pomelo.EntityFrameworkCore.MySql.Design | 1.1.2 | Removed |
| Serilog | 4.1.0 | 4.4.0 |
| Serilog.Extensions.Logging | 8.0.0 | 10.0.0 |
| Serilog.Sinks.Console | 6.0.0 | 6.1.1 |
| Swashbuckle.AspNetCore | 6.4.0 | 10.3.1 |
| System.ComponentModel.Annotations | 5.0.0 | Removed |
| System.Data.DataSetExtensions | 4.5.0 | Removed |
| System.Diagnostics.EventLog | 8.0.0 | 10.0.13 |
| System.ValueTuple | 4.6.1 | Removed |
| TimeSpanParserUtil | 1.2.0 | 1.2.0 |
| TimeZoneConverter | 7.2.0 | 8.0.0 |
