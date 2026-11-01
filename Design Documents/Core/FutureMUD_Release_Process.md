# FutureMUD Release Process

## Purpose

Use this procedure for stable releases of the products in `FutureMUD.Web/Configuration/release-products.json`. Stable product tags are the only routine publishing trigger. FutureMUD does not publish nightly or incremental builds.

## Product tags

| Product | Project | Tag |
| --- | --- | --- |
| Engine | `MudSharpCore/MudSharpCore.csproj` | `engine-vX.Y.Z` |
| Database Seeder | `DatabaseSeeder/DatabaseSeeder.csproj` | `seeder-vX.Y.Z` |
| Discord Bot | `DiscordBotCore/DiscordBotCore.csproj` | `discordbot-vX.Y.Z` |
| Terrain Planner & Engine API | `TerrainPlanner/TerrainPlanner.Server/TerrainPlanner.Server.csproj` | `terrainplanner-vX.Y.Z` |
| Web MUD Client | `MudClient/MudWebSocketProxy/MudWebSocketProxy.csproj` | `mudclient-vX.Y.Z` |

The tag version must contain exactly three numeric parts and must exactly equal the project's MSBuild `Version` property. Keep `AssemblyVersion` and `FileVersion` aligned when the project declares them.

## Prepare a release

1. Start from current `master` with no unrelated changes.
2. Select the product in `release-products.json` and verify its project path, runtime matrix, tests, archive name, and packaging flags.
3. Update the project's `Version`, `AssemblyVersion`, and `FileVersion` to the intended `X.Y.Z`.
4. Update compatibility text, release notes, documentation, and configuration affected by the release. In particular, keep the Downloads and Getting Started runtime requirements accurate.
5. Run the product's targeted tests from the manifest. For Engine releases, also run the database-free documentation export and confirm that all five metadata families are populated.
6. Publish at least one representative runtime locally with the manifest settings when packaging changed. Normal packages are framework-dependent, single-file, untrimmed, include native libraries in the bundle, and embed symbols.
7. Merge the release preparation. The commit on `master` that contains the final version is the release commit. Prefer the title `Version update to X.Y.Z` for that exact commit.

For Web MUD Client and Terrain Planner releases, verify every published runtime package includes the self-contained deployment verifier and platform deployment wrappers. The protected production environment signs the current update manifest; live verification must include the latest manifest and signature endpoints as well as every archive and checksum link. Terrain API is retired and `terrainapi-v*` is no longer a publishing trigger.

For Database Seeder releases, refresh the bundled blank database snapshot whenever its manifest does not name the latest migration. Run the seeder with `--refresh-blank-snapshot`; maintainers who cannot use the default local snapshot database can set `FUTUREMUD_SNAPSHOT_CONNECTION_STRING` to a disposable database connection string. The refresh command drops and recreates the database named by that connection string, so never point it at a database that must be retained.

Do not tag an earlier feature commit, an unmerged PR head, or a later unrelated commit. If a PR is squash-merged, tag the squash commit on `master`.

## Publish manually

Replace the example product, version, and commit with the intended release:

```powershell
$tag = "engine-v2.0.0"
$releaseCommit = git rev-parse master

git show "$releaseCommit:MudSharpCore/MudSharpCore.csproj"
git ls-remote --exit-code --tags origin "refs/tags/$tag"
git tag -a $tag $releaseCommit -m "FutureMUD Engine 2.0.0"
git rev-list -n 1 $tag
git push origin "refs/tags/$tag"
```

`git ls-remote` should report no existing tag before creation; its non-zero result is expected in that case. Never move, replace, or force-push a release tag. Verify that `git rev-list -n 1` is the exact version commit before pushing.

The tag starts `.github/workflows/publish-products.yml`. It validates the project version, runs the declared tests, builds each runtime, exports Engine documentation when applicable, tests the resumable publishing API against a temporary store, and atomically promotes the release to futuremud.com. Framework-dependent single-file packages pass both `--self-contained false` and `PublishSelfContained=false` so referenced executable projects use the same publish mode. After a successful production promotion from a tag push, it posts one announcement to the FutureMUD Discord patch-notes channel with a link to the canonical website patch note. Manual retry runs do not post another announcement.

The declared Release tests run once on each native runner OS (`win-x64` and `linux-x64`). The `linux-arm64` package is cross-published on an x64 Linux runner; repeating the same x64 tests there supplies no ARM execution coverage. Every runtime package still gates smoke publishing and production promotion. Tests and publishing restore their own required dependency graphs. Concurrent runs for the same release tag are serialized without cancelling an active release; production promotion also retains its per-product concurrency boundary. Tag pushes do not additionally launch the general Debug unit-test matrix.

Website deployment restores with dependency auditing enforced, runs the Release website tests, then publishes the same framework-dependent managed build with `--no-build --no-restore`. The production job requires that qualification job to succeed. The separate Website Security workflow retains its dependency review, dependency-boundary checks and CodeQL analysis.

The Discord webhook is stored as the `FUTUREMUD_RELEASE_DISCORD_WEBHOOK_URL` secret in the protected `production` environment. Never put the webhook URL in the workflow, repository, command output, or Actions logs. If the Discord notification fails, the step reports a warning without changing the result of a release that has already been published.

The manual action in `backfill-products.yml` is restricted to specifically allowlisted historical releases. Do not add new routine releases to that workflow.

Single-file archive publishing shuts down build servers after the tests, then uses one MSBuild node with serial restore and project builds and separate compiler processes. This bounds retained compiler state across the test and runtime-specific compilations; all analyzers remain enabled. An infrastructure interruption does not qualify a package: retry the existing immutable tag through the manual `release_tag` input after any publishing-workflow correction has reached `master`. Keep the tagged product source unchanged and require the same tests, runtime matrix, smoke test and production promotion on the retry.

## Windows signing and free distribution options

An unsigned portable executable can be blocked by Windows Smart App Control. Building successfully or publishing one file does not prove that Windows permits it to run. Record any block separately from database and unit-test results; do not disable security policy or add exclusions as a release workaround.

Microsoft provides free signing for **MSIX packages submitted through the Microsoft Store**, with free individual developer registration. This requires a Store submission, certification and MSIX packaging; it does not sign the portable EXE distributed from futuremud.com. Assess console launch, database connectivity, writable configuration/backup locations and the required .NET runtime before adopting that distribution path. See [Microsoft's code-signing options](https://learn.microsoft.com/en-us/windows/apps/package-and-deploy/code-signing-options).

SignPath Foundation offers free signing to qualifying projects using an OSI-approved licence. FutureMUD's current CC BY-NC-ND licence does not meet that requirement. Public source visibility alone is insufficient; do not change licensing merely to enable a signing integration. See [SignPath's conditions](https://signpath.org/terms) and the [FutureMUD licensing page](../../FutureMUD.Web/Content/Pages/license.md).

For local development, the ordinary unbundled build can be run with `dotnet DatabaseSeeder/bin/Release/net10.0/DatabaseSeeder.dll` from the repository root after building. This is separate from the one-executable release qualification. No paid signing provider, signing credentials, certificate trust changes or Store submission is configured by the catalogue migration.

## Publish with Codex

Invoke the repo-local skill with a product and version, for example:

```text
Use $futuremud-release to prepare and publish Engine 2.0.0.
```

The skill follows the same checks, uses the exact merged version commit, creates one annotated product tag, monitors the workflow, and verifies production. It must not invent an incremental build, use the historical backfill path, or move an existing tag.

## Verify production

The persistence library compiles historical migration models in bounded helper assemblies; see [Migration model compilation](Migration_Model_Compilation.md). Release bundles must include those runtime helpers and exclude the build-only compiler tool. Engine 2.15.0 and Seeder 3.8.0 were unpublished attempts; their immutable tags are retained, and the corrected releases use 2.15.1 and 3.8.1.

Engine 2.16.0 also stopped before production promotion because its component and spell compatibility audits failed. Retain that immutable tag; Engine 2.16.1 contains the audit corrections and is its replacement release.

1. Confirm the workflow's prepare, every runtime package, smoke publish, and production publish jobs succeeded.
2. Confirm `https://futuremud.com/downloads` shows the expected product version and runtime set.
3. Open each `.sha256` link and confirm its filename matches the archive.
4. Confirm `/downloads/{product}/latest/{runtime}` redirects to the new versioned URL with no-store caching.
5. For Engine, confirm the public documentation reports the new Engine version and source revision.
6. On Linux, ensure the extracted app host is executable (`chmod +x MudSharp` when required).

The website exposes only the latest release. The replaced release remains private for 24 hours as the rollback slot.
