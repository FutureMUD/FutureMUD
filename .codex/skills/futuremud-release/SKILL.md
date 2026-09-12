---
name: futuremud-release
description: "Prepare, publish, or verify a stable FutureMUD product release: product versions, release tags, publishing workflows and production downloads. Skip ordinary feature work, generic CI failures and unrelated dependency bumps. A preparation or verification request does not authorise tagging or publishing."
---

# FutureMUD Release

Work on the phase the user requested: preparation, publication, or read-only verification. Do not automatically advance from one phase to the next. Handle one manifest product per release action.

## Authority and task context

Use `FutureMUD.Web/Configuration/release-products.json` for product/project ownership, tag prefixes, runtimes, publishing settings and targeted checks. For preparation/publication, read the applicable sections of `Design Documents/Core/FutureMUD_Release_Process.md`; for a narrow verification query, consult those sections only when the manifest and observed release evidence leave ambiguity. Apply the repository instructions without rereading already-loaded guidance.

A product release requires a three-part `X.Y.Z` version. Resolve product/version/phase from the request and repository evidence before a mutating action. If publication authority or the release target remains ambiguous, report the missing decision and stop before creating a tag.

## Preparation only

- Inspect the current `master` baseline and keep unrelated work out of release preparation. Do not reset or overwrite an existing worktree to refresh it.
- Update the product's `Version`, `AssemblyVersion` and `FileVersion` where present.
- Update compatibility text, release notes, documentation and configuration affected by that product.
- Run the manifest's targeted tests. For Engine, also export and validate the documentation catalogue.
- If packaging changes, locally publish a representative runtime with the manifest's framework-dependent, single-file, native-bundling, untrimmed settings.
- Use the normal PR flow when requested. Stop before tag creation unless publication was also explicitly requested.

Report the prepared product/version, checks and intended next release action, not a claim that the product has shipped.

## Publication: select and verify the commit

Use the exact `master` commit containing the final version. A title such as `Version update to X.Y.Z` is a useful locator, not proof: verify the project version and contents at the candidate SHA. For a squash merge, use the merged squash commit, not the pre-merge branch commit. Stop on a version mismatch or unrelated later changes.

With publication authorised:

1. Form the tag from the manifest `tagPrefix` and exact version.
2. Verify the remote tag does not already exist.
3. Create one annotated tag at the exact release commit, then peel it and verify the target before pushing.
4. Push only that tag. Never move, reuse, delete or force-push a release tag.
5. Use the normal `publish-products.yml` path; `backfill-products.yml` is a fixed historical allowlist, not a new-release workflow.
6. Inspect workflow progress/results through version validation, every runtime package, smoke publishing and production promotion.

Treat tag creation and push as the release action. If the workflow is unfinished or fails, report the observed state and remaining gates; do not represent tagging or an intermediate job as a verified production release. Do not promise unattended monitoring unless the active environment actually supports it.

## Read-only verification

Inspect the requested product/version without editing project files, creating tags or rerunning publication merely to gather evidence. For complete release verification, confirm:

- `https://futuremud.com/downloads` lists the product version and declared runtimes;
- every archive has a matching SHA-256 link;
- `/downloads/{product}/latest/{runtime}` redirects to the versioned archive;
- Engine documentation reports the released version and source commit;
- the workflow used the exact tagged commit and reached production promotion.

For a narrow question, inspect the relevant subset and state the scope rather than implying all checks passed. A full verification receipt reports the product/version, tag, commit SHA, workflow URL/status, runtime set, production download page and any missing evidence.
