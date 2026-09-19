---
name: futuremud-subsystem-quality-pass
description: Review, repair, document, and verify FutureMUD subsystems end to end. Use when the user asks for a quality, completeness, optimisation, enhancement, bug, logic, unfinished-code, parser, command, builder-workflow, or design-document pass over a FutureMUD subsystem, module, command surface, helper family, runtime feature, or cross-cutting implementation path.
---

# FutureMUD Subsystem Quality Pass

## Overview

Use this skill to turn a FutureMUD subsystem review into grounded code improvement: trace the real runtime path, fix clear defects, update docs/help when behavior changes, add focused tests, and verify the edited slice.

The user usually wants actionable implementation, not a detached critique, unless they explicitly ask for review-only output.

## Workflow

1. Scope the subsystem from the user's wording.
   - Treat phrases like "quality pass", "review for bugs", "easy improvements", "unfinished code", "clear optimisations", "be thorough", and "follow call chains" as permission to inspect adjacent consumers.
   - Respect explicit narrowing such as "minimal patch" or named files.

2. Read local guidance and relevant design docs first.
   - Check `AGENTS.md` inheritance.
   - Find existing `Design Documents/` coverage for the subsystem.
   - Read `references/review-checklist.md` when planning the pass.

3. Map the live code path before editing.
   - Start from the public contract or command entry point.
   - Trace interfaces, concrete implementations, factories/registries, command parsers, effects, persistence/loaders, seeder/default data, help text, and tests.
   - Trust current code and tests over stale comments, but update docs when the documentation is the stale part.

4. Prioritize findings.
   - Fix runtime bugs, parser/logic errors, persistence drift, unsafe fall-throughs, wrong echoes, missing validation, and obvious performance traps first.
   - Keep style-only cleanup out unless it directly helps the task.
   - If the requested approach is materially worse, say so once and propose the safer FutureMUD-shaped alternative.

5. Patch in the smallest coherent slice.
   - Preserve existing behavior unless the user asked for semantic change or the behavior is clearly defective.
   - Prefer existing helper APIs, builder conventions, `StringStack` parsing patterns, colour helpers, and command-output idioms.
   - For visible command/help changes, update docs/help/tests in the same pass.

6. Add verification.
   - Add focused regression tests for fixed defects and parser behavior.
   - Update or add design docs when the user asks for docs or the subsystem behavior changed materially.
   - Run targeted build/test commands from the repo guide; use single-node restore/build/test when sandboxed MSBuild is flaky.

7. Report with evidence.
   - Summarize fixes, docs, and tests.
   - Call out remaining risks or skipped validation.
   - Mention any deliberately out-of-scope improvements.

## Review Targets

Check these surfaces when they are relevant:

- Command syntax, parser branches, aliases, examples, help text, and player-facing echoes.
- Runtime state transitions, null/fallback behavior, event dispatch, effects, and scheduled/heartbeat behavior.
- Builder commands, editable-item helpers, validation, default values, and show output.
- Persistence models, migrations, loaders, XML serialization, clone/copy paths, and rerun repair logic.
- Seeder defaults, stock data, AI/hook registrations, FutureProg functions, and design docs.
- Tests, stubs, mocks, fixture setup, and exact verification commands.

## Verification Hints

Prefer focused tests first, then the relevant project suite:

```powershell
dotnet restore MudSharp.sln -m:1 -p:RestoreBuildInParallel=false -p:NuGetAudit=false
dotnet build MudSharpCore\MudSharpCore.csproj -c Debug --no-restore -m:1 -p:NoWarn=NU1902%3BNU1510
dotnet test 'MudSharpCore Unit Tests\MudSharpCore Unit Tests.csproj' -c Debug --no-restore -m:1
dotnet test 'DatabaseSeeder Unit Tests\DatabaseSeeder Unit Tests.csproj' -c Debug --no-restore -m:1
git diff --check
```

If a filtered test exits with little output, rerun with `--verbosity normal` or a tighter fully qualified name filter before assuming the product code is broken.
