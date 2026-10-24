# NuGet dependency maintenance

## Scope and version selection

The 24 October 2026 upgrade covers `MudSharp.sln`, its project dependencies, and the shared migration-batch props. Separate MudClient/TerrainPlanner solutions, archived `MigrationBackup` projects, and the legacy analyzer test project outside the solution are not part of this inventory. The checked-in project files remain the authoritative version declarations.

Select the newest **listed stable** NuGet release that supports the consuming framework and provider/host contracts. The flat-container index includes unlisted packages: DSharpPlus 5.0.0 is unlisted, whereas 4.5.3 is the current selection. Keep related package families aligned across production and test projects. Do not add arbitrary direct references solely to force every transitive dependency onto a different major version.

## Compatibility decisions

- **Entity Framework:** Pomelo.EntityFrameworkCore.MySql 9.0.0 is the latest stable provider and constrains EF Core Relational to 9.0.x. All EF runtime, tooling, migration-helper and in-memory test dependencies use 9.0.21. EF Core 10 requires a compatible stable provider first. The obsolete Pomelo.EntityFrameworkCore.MySql.Design 1.1.2 reference was removed; current tooling uses Microsoft.EntityFrameworkCore.Design. No database schema change is intended.
- **MSTest:** Adapter and framework use 4.5.1 with Microsoft.NET.Test.Sdk and the existing VSTest/TRX reporting flow. Removed `ThrowsException` assertions become `ThrowsExactly`, including asynchronous assertions, retaining exact exception-type semantics. Attribute-based expected exceptions are asserted around the operation itself. Obsolete `DataTestMethod` attributes become `TestMethod`, retaining their data rows.
- **NCalc:** Custom functions evaluate arguments through `FunctionData.Evaluate(index)` in NCalc 7, preserving evaluation context and nested custom functions. Public FutureMUD expression APIs and error-handling behavior remain unchanged.
- **Roslyn/Visual Studio:** Roslyn packages use 5.9.0. The verified compiler host is .NET SDK 10.0.401 (Roslyn 5.9), and the installed Visual Studio 2026 18.10 host supplies Roslyn 5.10. The optional VSIX declares 18.10 as its supported baseline instead of claiming compatibility with VS 2015/2017; its managed payload declares both AMD64 and ARM64 installation targets. Older compiler hosts and ARM64 installation have not been verified. This does not change the engine's target framework.
- **Platform libraries:** The net10.0 engine no longer references the separate System.ComponentModel.Annotations, System.Data.DataSetExtensions or System.ValueTuple packages; the framework supplies these APIs.
- **MySQL client:** Connector/NET removed the deprecated `SslMode=None` value. Local Debug connection templates and smoke harnesses now use `SslMode=Disabled`, preserving their existing no-TLS development setup and compatibility with both MySql.Data and MySqlConnector. The upgrade/backup client boundary also accepts the legacy `None` spelling in existing configuration, parsing connection-string options before translating it to `Disabled`. Other TLS modes, the driver's default, and quoted credentials are preserved; saved configuration is not rewritten.
- **Upgrade service:** Swashbuckle 10 uses the Microsoft.OpenApi namespace and document-bound security references. OAuth scope requirements are preserved. The unused Microsoft.AspNetCore.OpenApi package was removed; this service uses Swashbuckle's pipeline rather than AddOpenApi/MapOpenApi.
- **Security:** MailKit/MimeKit upgrades resolve System.Security.Cryptography.Pkcs 10.0.13 transitively. This addresses the advisories found on 10.0.0 without adding a separate version override.

Migration batches explicitly reference the same EF Relational patch as the main persistence library. Seeder source-analysis tests reference the Roslyn NuGet package instead of SDK-relative assembly paths, so Visual Studio MSBuild and the CLI resolve the same dependency. The scratch host excludes sources belonging to its nested standalone harness projects and has its own harmless entry point; those harnesses must be built and run through their own project files.

Tests that require the same mocked entity now use `AreSame`, preserving their reference-identity contract under MSTest's updated equality handling. Updating DatabaseSeeder's package references also changes its project-file fingerprint in the generated item manifest. Only that fingerprint is refreshed; all 43,825 entries and the original payload capture time are unchanged.

## Rechecking security and currency

Restore with `NuGetAudit=true` and `NuGetAuditMode=all`, then inspect both direct and transitive packages with `dotnet list <solution-or-project> package --vulnerable --include-transitive --no-restore`. Run `--outdated` separately to check direct package currency. An audit fetch failure is an unknown result, not evidence of no vulnerabilities. Do not disable auditing or suppress NU190x warnings for this task.

The classic VSIX project needs Visual Studio's MSBuild and installed VSSDK targets. For CLI package inventory, use an equivalent temporary solution containing the SDK projects; the VSIX project itself has no NuGet references. On a restricted Windows account, a writable per-run `NUGET_HTTP_CACHE_PATH` can avoid HTTP-cache access failures without changing package sources or credentials.

## Verification, 24 October 2026

- Visual Studio MSBuild completed the full Debug solution build. Focused native test-project builds also passed; the final identity-assertion updates were subsequently compiled and tested through the CLI.
- A cold Visual Studio design-time build, with project-reference building disabled, created the migration compiler and 23 generated model files in a fresh configuration.
- The full ten-project fast test run exposed seven mock-identity failures, 32 sandbox temporary-directory failures, and one stale manifest fingerprint. After the assertion/fingerprint fixes and using a writable workspace-local `TEMP`/`TMP`, the four affected suites passed all 9,272 cases and the focused manifest/replay check passed all 20 cases. Combined with the unaffected full-run results, all 11,937 executed cases passed; three Unix filesystem-permission cases were skipped on Windows. The full run and both focused reruns each recorded stable source. Original failure evidence is retained locally.
- The DatabaseSeeder Release build passed with zero errors; existing compiler warnings remain.
- EF's pending-model check reported no model changes. The local Swagger endpoint returned OpenAPI 3.0 with the OAuth2 definition and all three required scopes intact.
- The final direct/transitive vulnerability audit covered 124 SDK projects and reported zero known vulnerable packages and no audit errors. The direct-package currency check offered only EF Core 10 updates, excluded by the provider constraint above.

This verification used Windows and native Visual Studio MSBuild, not an interactive IDE session. Live MySQL replay, external provider API calls, VSIX installation, Unix-only behavior, and hosted CI were not exercised.

## Sources

- [Pomelo supported EF Core versions](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
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
