# FutureMUD Subsystem Review Checklist

## Starting Points

- Begin from the public contract: interface, command method, builder command, factory, registry, FutureProg function, or seeder entry point.
- Follow call chains through concrete consumers before concluding behavior is correct.
- Search for command/help/docs/tests that name the subsystem; player-facing syntax and echoes are part of the product surface.
- Check existing design docs and update them when behavior, persistence, command syntax, or builder workflow changes.

## What To Look For

- Parser drift between documented syntax and actual command matching, especially optional amount/table/emote fragments.
- Runtime fall-throughs that choose to do nothing when a valid fallback action exists.
- Wrong target resolution, proximity checks, null fallbacks, missing ownership checks, or stale permission checks.
- Wrong or misleading player echoes, help text, colour cues, table output, and builder `show` output.
- Persistence mismatches in loaders, clone/copy paths, XML round trips, migrations, model snapshots, and rerun repair.
- Test fixture failures caused by missing mock services or namespace imports before treating them as product bugs.
- Hot-path inefficiencies where the user's request explicitly includes optimisation, while preserving semantics.

## Documentation Expectations

- If the user requests a design document, produce a real subsystem document rather than scattered notes.
- Split docs by audience when needed: runtime architecture, builder how-to, seeder-maintainer notes, and workflow/integration docs.
- For builder-facing docs, include practical command workflow, validation/preflight behavior, persistence implications, and current limitations.
- For public helpers, add or refresh XML docs when the user asks for detailed function documentation.

## Common FutureMUD Patterns

- Prefer `StringStack` / `PopSpeech()` for command parsing and `SafeRemainingArgument` for final free text unless raw quotes matter.
- Prefer `DescribeEnum()`, `Describe(voyeur)`, `ToColouredString()`, `ColourValue()`, `ColourError()`, `ColourCommand()`, and `ColourName()` for player-facing output.
- Use `StringUtilities.GetTextTable()` for tables and `ListToString()` / `ListToCommaSeparatedValues()` for lists.
- Treat persisted registries and singleton IDs as compatibility contracts, not casual enum-like lists.
- When touching item or economy systems, update the relevant design documents required by repository guidance.

## Verification Notes

- Use focused regression tests for every concrete bug fixed.
- If a test fixture fails before exercising the command logic, repair the fixture instead of weakening the product check.
- If a direct filtered test hides useful output, rerun with normal verbosity or a tighter fully-qualified test filter.
- If sandboxed builds hit project graph or file-lock issues, use single-node restore/build/test commands.
- End with `git diff --check` when files were edited.
