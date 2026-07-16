# FutureMUD Release Process

## Purpose

Use this procedure for stable releases of the five products in `FutureMUD.Web/Configuration/release-products.json`. Stable product tags are the only routine publishing trigger. FutureMUD does not publish nightly or incremental builds.

## Product tags

| Product | Project | Tag |
| --- | --- | --- |
| Engine | `MudSharpCore/MudSharpCore.csproj` | `engine-vX.Y.Z` |
| Database Seeder | `DatabaseSeeder/DatabaseSeeder.csproj` | `seeder-vX.Y.Z` |
| Discord Bot | `DiscordBotCore/DiscordBotCore.csproj` | `discordbot-vX.Y.Z` |
| Terrain Planner | `Terrain Planner Core/Terrain Planner Core.csproj` | `terrainplanner-vX.Y.Z` |
| Engine API / Terrain API | `Terrain API/Terrain API.csproj` | `terrainapi-vX.Y.Z` |

The tag version must contain exactly three numeric parts and must exactly equal the project's MSBuild `Version` property. Keep `AssemblyVersion` and `FileVersion` aligned when the project declares them.

## Prepare a release

1. Start from current `master` with no unrelated changes.
2. Select the product in `release-products.json` and verify its project path, runtime matrix, tests, archive name, and packaging flags.
3. Update the project's `Version`, `AssemblyVersion`, and `FileVersion` to the intended `X.Y.Z`.
4. Update compatibility text, release notes, documentation, and configuration affected by the release. In particular, keep the Downloads and Getting Started runtime requirements accurate.
5. Run the product's targeted tests from the manifest. For Engine releases, also run the database-free documentation export and confirm that all five metadata families are populated.
6. Publish at least one representative runtime locally with the manifest settings when packaging changed. Normal packages are framework-dependent, single-file, untrimmed, include native libraries in the bundle, and embed symbols.
7. Merge the release preparation. The commit on `master` that contains the final version is the release commit. Prefer the title `Version update to X.Y.Z` for that exact commit.

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

The tag starts `.github/workflows/publish-products.yml`. It validates the project version, runs the declared tests, builds each runtime, exports Engine documentation when applicable, tests the resumable publishing API against a temporary store, and atomically promotes the release to futuremud.com.

The manual action in `backfill-products.yml` is restricted to specifically allowlisted historical releases. Do not add new routine releases to that workflow.

## Publish with Codex

Invoke the repo-local skill with a product and version, for example:

```text
Use $futuremud-release to prepare and publish Engine 2.0.0.
```

The skill follows the same checks, uses the exact merged version commit, creates one annotated product tag, monitors the workflow, and verifies production. It must not invent an incremental build, use the historical backfill path, or move an existing tag.

## Verify production

1. Confirm the workflow's prepare, every runtime package, smoke publish, and production publish jobs succeeded.
2. Confirm `https://futuremud.com/downloads` shows the expected product version and runtime set.
3. Open each `.sha256` link and confirm its filename matches the archive.
4. Confirm `/downloads/{product}/latest/{runtime}` redirects to the new versioned URL with no-store caching.
5. For Engine, confirm the public documentation reports the new Engine version and source revision.
6. On Linux, ensure the extracted app host is executable (`chmod +x MudSharp` when required).

The website exposes only the latest release. The replaced release remains private for 24 hours as the rollback slot.
