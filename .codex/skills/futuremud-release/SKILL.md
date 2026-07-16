---
name: futuremud-release
description: Prepare, tag, publish, and verify stable FutureMUD product releases. Use when Codex is asked to bump or validate a release version, create an Engine, Seeder, Discord Bot, Terrain Planner, or Terrain API release tag, monitor the product publishing workflow, or verify a release on futuremud.com.
---

# FutureMUD Release

Read `AGENTS.md`, `FutureMUD.Web/Configuration/release-products.json`, and `Design Documents/Core/FutureMUD_Release_Process.md` before changing or publishing a release.

## Prepare

1. Identify exactly one manifest product and require a three-part `X.Y.Z` version.
2. Refresh `master`; keep unrelated work out of the release branch.
3. Update `Version`, `AssemblyVersion`, and `FileVersion` where present.
4. Update compatibility copy, release notes, documentation, and configuration affected by that product.
5. Run the manifest's targeted tests. For Engine, also export and validate the documentation catalogue.
6. When packaging changed, locally publish a representative runtime using the manifest's framework-dependent, single-file, native-bundling, untrimmed settings.
7. Publish the preparation through the normal PR flow when requested.

## Select the release commit

Use the exact commit on `master` containing the final version. Prefer the commit title `Version update to X.Y.Z`. If preparation was squash-merged, use the squash commit, not the pre-merge branch commit.

Verify the project version at that commit. Stop if the commit includes a different version or unrelated later changes.

## Tag and publish

1. Build the tag from the manifest's `tagPrefix` plus the exact version.
2. Verify the remote tag does not already exist.
3. Create one annotated tag at the exact release commit.
4. Peel the tag and verify its target before pushing.
5. Push only that tag. Never move, reuse, delete, or force-push a release tag.
6. Do not use `backfill-products.yml` for new releases; it is a fixed historical allowlist.
7. Monitor `publish-products.yml` through version validation, every runtime package, smoke publishing, and production promotion.

Treat tag creation and push as the release action. If the user's request was preparation-only, stop before creating the tag and report the intended commit and tag.

## Verify

Confirm:

- `https://futuremud.com/downloads` shows the product version and declared runtimes;
- each archive has a matching SHA-256 link;
- `/downloads/{product}/latest/{runtime}` redirects to the versioned archive;
- Engine documentation reports the released version and source commit;
- the workflow used the exact tagged commit.

Report the tag, commit SHA, workflow URL, runtime set, and production download page.
