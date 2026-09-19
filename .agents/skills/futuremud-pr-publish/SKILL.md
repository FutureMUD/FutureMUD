---
name: futuremud-pr-publish
description: Publish completed FutureMUD worktree changes through the normal GitHub flow. Use when the user asks Codex to create, update, publish, or merge a pull request for FutureMUD after the implementation is already ready, including data-maintenance, seeder, converter, runtime, review-fix, or merge-conflict follow-up branches in Codex worktrees.
---

# FutureMUD PR Publish

## Overview

Use this skill to carry a completed FutureMUD change from local worktree state through branch, commit, push, pull request, merge, and final live-state verification.

This skill is procedural guardrail knowledge for the FutureMUD repository, especially the Windows Codex worktree setup where Git metadata may live outside the writable project folder.

## Trigger Check

Use this skill when all are true:

1. The repository is FutureMUD.
2. The user asked to publish, create a PR, update a PR, merge a PR, or resolve a PR merge state.
3. The implementation is already complete or the requested merge-conflict/review-fix work is complete enough to publish.

Pause and finish implementation first when the branch still has unresolved code work, review comments, compile failures that must be addressed, or unmerged conflicts.

## Preflight

Gather these facts before changing Git state:

```powershell
git status --short --branch
git rev-parse HEAD
git remote -v
gh auth status
gh repo view --json nameWithOwner,defaultBranchRef
```

Confirm:

1. The default branch is still `master`.
2. The working tree contains only intended changes, or identify any unrelated user changes and leave them alone.
3. `HEAD` is on a usable branch or is detached.
4. Local validation has already run, or state clearly what still needs validation before publishing.

## Publish Flow

1. Create or confirm the branch.
   - Use the repo convention `codex/<short-task-name>` for new branches.
   - If `HEAD` is detached, create the branch before staging or committing.

2. Refresh from the base branch when the user asked for it or when fixing merge issues.
   - Fetch `origin master`.
   - Merge or otherwise incorporate `origin/master` according to the active task.
   - A merge-conflict task is not done until `git status` no longer shows an in-progress merge and the conflict resolution has been committed and pushed.

3. Stage only the intended files.
   - Do not stage unrelated user changes.
   - Use `git diff --check` on edited files when practical.

4. Commit if needed.
   - Use a concise task-specific commit message.
   - If the requested change already exists in a commit, do not create an empty duplicate commit.

5. Push the branch.
   - Use `git push -u origin <branch>` for a new branch.
   - If local Git metadata writes fail on paths like `FETCH_HEAD`, `ORIG_HEAD.lock`, or `index.lock`, treat it as a Windows/Codex permission boundary rather than a content problem.

6. Create or update the PR.
   - Use `master` as the base unless live repo metadata says otherwise.
   - If a PR already exists for the current branch, update/check that PR instead of opening a duplicate.
   - Include the validation performed and the important files changed in the PR body.

7. Check live PR state.
   - Query PR number, URL, head branch, base branch, mergeability, status, and head SHA.
   - A fresh PR can briefly report unclear mergeability. Refresh live state before assuming it is blocked.

8. Merge when requested.
   - If a merge API expects `expected_head_sha`, pass the full 40-character SHA from `git rev-parse HEAD` or the PR head ref, not the short hash.
   - If `gh pr merge` completes server-side but fails during local cleanup because another `master` worktree exists, trust the live PR state and clean up only safe remote/local leftovers.

9. Verify the result.
   - Confirm the PR state is `MERGED`.
   - Fetch or inspect `origin/master` so the final answer can report the merge commit or final live state.

## Fallbacks

If `gh auth status` reports an invalid token, stop retrying unauthenticated PR commands. Push the branch if possible, then use the available GitHub connector/API path for PR creation, live-state checks, and merge.

If network or filesystem sandboxing blocks a required Git/GitHub step, request the minimum permission needed for the exact command. Explain that the non-elevated workaround is either unavailable or would stop before the requested publish/merge outcome.

If a commit belongs to a branch whose PR was already merged, create a fresh branch from current `origin/master`, cherry-pick the commit, push that branch, and open a new PR.

## Final Response

Report:

1. Branch name.
2. PR number and URL.
3. Merge state and merge commit when available.
4. Validation run or validation not run.
5. Any notable caveat, such as GitHub CLI cleanup failing after the server-side merge had already succeeded.

If staging, committing, branch creation, pushing, or PR creation succeeded in the Codex app, include the matching app directive in the final response after the action has actually succeeded.
