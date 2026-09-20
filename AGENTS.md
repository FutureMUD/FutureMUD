# FutureMUD repository instructions

These rules apply throughout the repository. For a path you change, also read the applicable project/module `AGENTS.md`; more specific rules override these defaults. Do not load instructions from unrelated subtrees.

## Product and architecture

FutureMUD is a configurable engine for Roleplay Intensive MUDs, not one particular game. Prefer reusable, builder-configurable systems over hard-coded setting policy. Coded simulation, persistent consequences, and player-facing narrative should support one another; end users should not need to program to build and host a game.

Use the SDK, target frameworks, language version, and package versions declared by the checked-out repository (`global.json` if present, project files, and shared build/package properties). Do not infer them from a version number in prose or upgrade them merely because a newer release exists. Prefer modern C# supported by those settings.

| Area | Owns |
| --- | --- |
| `FutureMUDLibrary/` | Shared interfaces, extensions, helpers, and value objects; mirrors runtime domain namespaces. |
| `MudSharpCore/` | Console game server, networking, concrete runtime/gameplay implementations, command modules. |
| `MudsharpDatabaseLibrary/` | EF Core context, models, mappings, and migrations for MySQL/Pomelo. |
| `ExpressionEngine/` | `IExpression` / NCalc-based formula evaluation and custom functions. |
| `DatabaseSeeder/` | Interactive installer, initial stock content, supported additional installs and reruns. |
| `DiscordBotCore/` | Standalone Discord integration and the engine bridge. |
| `RPI Engine Worldfile Converter/` | Legacy-content importer into an already-seeded world; not a replacement seeder. |
| `FutureMUD.Web/` | Website, release/download manifests, and documentation publishing. |
| `Temporary Scratch App/` | Prototypes and data utilities; not a substitute for regression tests. |

Other projects may be tools, experiments, or separate products. Inspect their project files and relevant documentation rather than assuming that every project ships with the engine. Release product ownership is defined by `FutureMUD.Web/Configuration/release-products.json`.

## Code conventions

- Use tabs, top-of-file `using` directives, and file-scoped namespaces in new files. Use PascalCase for types/members and `_camelCase` for private fields.
- Enable nullable reference types in new files and annotate nullable references. Prefer clear `var`, early validation returns, interpolation, and verbatim strings when escaping would obscure the text.
- Put shared interfaces and extensions in `FutureMUDLibrary`, under the appropriate domain. Design a new shared capability's interface before its runtime implementation. Use established C# patterns without adding unnecessary abstraction.
- Prefer readable LINQ; place chained operations on separate lines. Query syntax is useful when it substantially clarifies a query, such as tabular presentation. Avoid LINQ allocations where a demonstrated hot path requires otherwise.
- Prefer `async`/`await` and `Task`/`Task<T>` for I/O, respecting the owning subsystem's threading/lifecycle constraints. Do not introduce `async void` for ordinary I/O methods.
- In `MudSharpCore`, access `FMDB.Context` only within the appropriate `using (new FMDB())` scope. Context/model/migration ownership remains in `MudsharpDatabaseLibrary`; the core `FMDB` helper is the runtime access exception.

## Engine contracts and player-facing output

`IFrameworkItem` supplies the common 64-bit ID, name, and type identity. `IPerceivable` extends it for world entities that participate in perception; `IPerceiver` extends that for entities that perceive output. Prefer `ICell`/`Cell` over the legacy `IRoom`/`Room` distinction; a design's “room” normally means a cell.

Use the established `MudSharp.Framework` helpers rather than reimplementing formatting or command parsing:

- Localise numbers, dates, times, currencies, and other values to the receiving `IPerceiver` when known. A formatting receiver is conventionally called `voyeur`; perceivers implement `IFormatProvider`.
- Use `DescribeEnum()` for enum display, `TimeSpan.Describe(voyeur)` for durations, and `ToColouredString()` for booleans.
- Use `ColourValue()` / green for values, `ColourError()` / red for errors and warnings, `ColourCommand()` / yellow for syntax, user input and emote templates, and `ColourName()` / cyan for names and enum labels. Other colours use `Colour(ANSIColour)` and the `Telnet` constants.
- Prefer `StringBuilder` for large output, `StringUtilities.GetTextTable()` for tables, and `ListToString()` or `ListToCommaSeparatedValues()` for lists.
- Use `StringStack` for stepwise input parsing. Prefer `PopSpeech()`; use raw `Pop()` only when raw-token semantics are needed. Prefer `SafeRemainingArgument` for final free text, but retain raw `RemainingArgument` when quotes matter to forwarded commands, regexes, or emote markup.
- Never put side-effecting `Pop*()` calls in LINQ predicates/selectors. Capture the argument once, or deliberately peek and then pop.

For commands, apply `MudSharpCore/Commands/AGENTS.md` as well; command uniqueness, registration, permission and help rules live there.

## Load references for the surface being changed

| Task | Reference |
| --- | --- |
| Emote text or `IEmote` output | [Emote system](Design%20Documents/Markup/Emote_System.md) |
| Character description markup | [Character descriptions](Design%20Documents/Markup/Character_Description_System.md) |
| Human seeder description patterns | [Human seeder patterns](Design%20Documents/Markup/Human_Seeder_Description_Patterns.md) |
| Room/cell description markup | [Room description markup](Design%20Documents/Markup/Room_Description_Markup.md) |
| Room-building commands/workflows | [Room building guide](Design%20Documents/Building/Room_Building_Builder_Guide.md) |
| Subsystem design or changed behaviour | Relevant document in `Design Documents/`; use [the index](Design%20Documents/README.md) when its location is unknown. |
| Test selection, sandbox build issues, item/economy document routing | [Verification and documentation map](.codex/references/verification-and-docs.md), only the relevant section. |

Update the owning design documents in the same task when runtime behaviour, commands, persistence, or a documented contract changes. Read/update the affected sections, not every document in the subsystem. A behaviour-preserving refactor or cosmetic edit does not require an unrelated documentation rewrite. Mention substantive documentation changes in the result.

Repository skills are stored under `.codex/skills/`. Use a skill when its description matches the task; do not load all skills for ordinary repository work. Workflow/replay changes use `futuremud-database-seeder`; climate-specific tuning or runtime/analyzer work uses `futuremud-climate-seeder`; product release operations use `futuremud-release`.

## Verification

Choose checks from the changed behaviour and its dependants, not merely the repository name. Shared-library, expression, seeder, runtime, persistence-library, bot, converter, and website tests have separate owning suites. Long-running weather regressions belong in `MudSharpCore Climate Tests`, not the default core suite.

Use the paired repository scripts (`.ps1` on Windows, `.sh` on POSIX) where practical. For direct .NET commands, use `-m:1`; restore only when needed and use `--no-restore` for subsequent builds/tests. A full solution build is not the default: the VSIX project may require Visual Studio SDK targets absent from the environment. See the verification reference for exact commands and sandbox-specific exceptions.

For documentation/instruction-only changes, validate paths, syntax, and the diff; run code tests only when the edit affects executable behaviour or a specific contract requires them. For code changes, run the relevant tests and broaden to dependent suites when justified. Report the checks actually run, their outcomes, and any blocked or unverified behaviour; do not treat an unavailable environment as a passing test.

## Delegation

Use subagents for bounded work that benefits from cheaper execution, independent review, or keeping noisy output out of the main thread. Small tasks stay single-agent; do not create a mandatory planning/review/testing chain.

The coordinator owns implementation, integration and final acceptance. Use `fm_test_runner` for substantial or noisy execution of a specified automated-test plan; small checks may run directly through the compact interface. The coordinator owns test adequacy, regression-test authoring and repairs. Use `fm_reviewer` for focused code review, `fm_docs_researcher` for factual lookup, `fm_qa_runner` when test selection or adaptive verification requires judgement, and `fm_smoke_runner` only for an executable in-game scenario with explicit assertions and stop conditions. Keep the built-in `explorer` for bounded code mapping. Do not request premium child models or higher effort without a concrete reason. Do not duplicate verification or mutate tested inputs concurrently; return bounded receipts and retrieve more evidence only for a specific question.

Give each child an objective, exact files/revision or scenario, permitted actions, evidence requirements and a stopping budget. Point to relevant skills/references instead of copying the whole conversation. For a substantial handoff, use the relevant section of [.codex/references/agent-delegation.md](.codex/references/agent-delegation.md).

Only the coordinator delegates or escalates; children return blockers and evidence, not further agents. Avoid duplicate investigation. Use one source-code writer per worktree and one operator per test server/database; do not rebuild or change a running test's inputs. Parallel writes require explicitly disjoint ownership and isolated state.

QA agents do not repair code or weaken assertions. The coordinator diagnoses/fixes failed checks and requests a focused rerun. Preserve failures and unexecuted checks in the final result. A blocked check, ambiguous transcript or successful build is not proof that gameplay works.

### Escalation

Use `fm_escalation_specialist` for one unresolved root cause or high-risk design decision, not merely because a task is large. First distinguish missing evidence, tools, permissions or requirements from a reasoning bottleneck. Do not force cheaper-model failures before an obviously high-risk consultation.

The coordinator may request one bounded specialist consultation without a separate user confirmation when the existing task scope and budget permit. Supply the exact question, revision/working-tree scope, evidence, attempted approaches and outcomes, constraints and completion boundary. Do not copy the entire conversation. Prefer a targeted handoff; check whether the harness also inherits parent context.

The specialist advises; the coordinator owns implementation and final acceptance, and QA owns test execution. Do not duplicate the same investigation while the specialist works. Keep at most one premium escalation active per task; follow up on new evidence, not repeated speculation. This is an operating policy, not a hard token limit.

When the coordinator already uses Astra at high or greater effort, use the specialist only for a specifically useful independent assessment, not as a nominal capability upgrade. If premium reasoning is needed throughout the remaining task, prefer continuing with an appropriately configured coordinator over repeated specialist handoffs.
