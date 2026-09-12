# FutureProg contract and authoring instructions

Inherits [repository instructions](../../AGENTS.md) and [shared-library instructions](../AGENTS.md).

This folder owns shared FutureProg contracts and types. Concrete parsing, execution, built-in functions, registration and persistence implementations belong in `MudSharpCore/FutureProg`, not this shared library.

## Start from the calling contract

For a prog or contract change, establish the required return type, ordered parameter types, caller/event, visibility and expected side effects from the live call site. Do not guess a signature from the script's name or translate C# syntax into FutureProg by analogy.

For shared interface/type changes, trace the affected compiler/runtime implementations and consumers. Preserve compatibility unless a breaking change is explicitly part of the task, and update affected metadata, help, tests and design documentation together.

## Reference routing

The former long-form agent file is preserved as [the language guide](LANGUAGE_GUIDE.md). Use its table of contents to read the relevant section; do not load the whole manual for every task. It is reference material, not a standing instruction to complete its outline or documentation backlog. Its status note and source-line citations can be historical: verify behaviour in the current source.

Repository-relative source map:

| Question | Inspect |
| --- | --- |
| Shared types, variable contracts and dot properties | `FutureMUDLibrary/FutureProg/`, including `IProgVariable.cs` and the affected type. |
| Syntax, compilation, execution, caching or variables | Relevant code under `MudSharpCore/FutureProg/`; start with the affected statement/function or `FutureProg.cs`. |
| Built-in names, overloads and help metadata | The specific registration/implementation under `MudSharpCore/FutureProg/Functions/`. |
| Builder command syntax, compile/execute and signature editing | `MudSharpCore/Commands/Modules/ProgModule.cs`. |
| Builder lookup, visibility and signature compatibility | `MudSharpCore/FutureProg/ProgLookupFromBuilderInput.cs` and its caller. |
| Event payload order or hook compatibility | `FutureMUDLibrary/Events/AGENTS.md`, event metadata, and the relevant dispatch/hook. |
| Seeded examples | The owning seeder under `DatabaseSeeder/Seeders/`, not the whole catalogue. |

## Important constraints

- Parameter and event payload order is semantic, not cosmetic. Check both metadata and runtime callers.
- Verify available functions/properties and their types against current registrations. General-purpose lambda syntax is not interchangeable with the specialised collection-extension form.
- Keep temporary execution state distinct from persistent variable-register data. Inspect write paths and ownership before introducing persistent side effects.
- Respect existing recursion/iteration guards. Static-result caching is appropriate only when its input/state assumptions actually hold; inspect the current implementation rather than assuming all “static” modes behave alike.
- A compile success does not establish runtime correctness or safe side effects. Do not execute a mutating prog against live game data as a casual validation step.

For emitted text, read the relevant [emote](../../Design%20Documents/Markup/Emote_System.md), [character-description](../../Design%20Documents/Markup/Character_Description_System.md), or [room-markup](../../Design%20Documents/Markup/Room_Description_Markup.md) reference.

Validate the changed surface: shared-contract tests where appropriate, `MudSharpCore Unit Tests` for compiler/runtime behaviour, and seeder tests for seeded progs. Use in-game compile/execute checks only in an appropriate test environment when required. Report the signature/behaviour changed, tests actually run, and any runtime validation still outstanding.
