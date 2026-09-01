# FutureProg Execution Runtime

## Purpose

FutureProg is compiled ahead of execution and is routinely used in high-frequency game checks. The runtime therefore treats execution time and transient allocation as first-class constraints while preserving script syntax, strong typing, null/error behaviour, case-insensitive variable access, persistence, and built-in semantics.

The type representation and persistence contract are documented separately in [FutureProg Type System](./FutureProg_Type_System.md).

## Compiled Program Shape

`FutureProg.Compile()` produces an immutable statement array for execution. Function and statement nodes retain their pre-resolved return types, canonical variable/property names, and exact operator dispatch where applicable. Authored parameter names remain unchanged in `NamedParameters`; a parallel execution descriptor array stores canonical lower-case names and types for direct indexed binding.

Built-in compiler registrations are indexed by case-insensitive canonical function name. Signature matching materialises parameter types once and checks only registrations for that name. Collection-extension compilers use the same canonical-name lookup. Statement selection walks registrations once without creating a temporary list, and function parameters are compiled once rather than through a multiply-enumerated lazy sequence.

Compilation remains a lower priority than execution. Compiler changes must preserve existing diagnostics and are retained only when they do not regress representative compilation.

## Invocation And Frames

`Execute` and `ExecuteWithRecursionProtection` share the same binder and executor. The binder reads arguments by index, uses precomputed parameter descriptors, creates an exactly sized root dictionary, translates each argument to one `IProgVariable`, installs the typed `return` variable, and executes the statement array by index. Parameterless progs use a return-only frame and create a name dictionary lazily only if a statement declares another root variable.

`IVariableSpace` remains name based. Root names and compiler-produced local names are canonical lower case, while authored casing is retained for display and external schemas. Dot-reference nodes likewise store a canonical property name, and direct `GetProperty` calls retain ordinal case-insensitive compatibility.

Lexical blocks use `LocalVariableSpace` over their parent scope. `for`, `foreach`, and `while` create one local scope per loop execution and clear its local entries before each iteration. Parent assignments still flow to the parent, while iteration-only declarations reset exactly as they did when every iteration allocated a fresh scope.

### Slot-frame prototype

A slot-backed root-frame prototype with compiled variable handles was measured after the local runtime fixes. It reduced trivial root-frame allocation, but failed the incremental landing gate: versus the local-fixes build it made `ForeachLoop` about 19% slower, `CollectionAny` about 8% slower, and `CollectionWhereSelect` about 5% slower. It was discarded. The runtime therefore remains on the simpler name-based frame architecture.

## Static Execution Cache

`FutureProgStaticType.FullyStatic` caches the first successful result, including a successful `null`. Normal `return` statements and fall-through completion use the same success-bearing path, so either can publish a cache value. Errors and exceptions are not cached.

First execution is serialised by a per-prog lock. The result is written before the volatile published flag, and later hits are lock free. Concurrent first callers therefore observe one coherent result. The cache resets when `StaticType` changes and whenever the prog recompiles.

`StaticByParameters` deliberately remains uncached. An argument-keyed cache could retain characters, items, locations, and connected world graphs without a safe bound or ownership rule.

## Recursion

Recursion depth is thread-local and represents active protected calls, not lifetime call count. `ExecuteWithRecursionProtection` increments before execution and decrements in `finally`, including error and exception paths. Calls deeper than 250 active protected invocations terminate through the existing error path.

Ordinary sequential protected calls therefore begin at depth zero. Nested user-prog invocation uses the protected entry point and participates in the same active-depth budget.

## Collections, Dictionaries, And Ownership

Input adapters normalise collection elements to `IProgVariable` once. They avoid an intermediate wrapper list before `CollectionVariable` performs its required normalisation. Dictionary input is translated directly into the dictionary owned by `DictionaryVariable`, and collection-dictionary translation uses explicit loops rather than nested iterator pipelines.

Typed result adapters unwrap `IProgVariable.GetObject` consistently for collections, dictionaries, and collection dictionaries. Primitive text, number, and boolean values therefore have the same result shape across all three adapters.

Execution frames and loop scopes own only transient variable references and are not pooled. Clearing a reused local scope drops its entries before the next iteration. Compiled function nodes retain their most recent `Result`, as in the established architecture; compiled ASTs are therefore not general-purpose object-ownership boundaries and may retain the last referenced entity until that node next executes. This pass adds no long-lived memoisation beyond the bounded fully-static result.

## Thread Assumptions

Normal compiled function and statement nodes contain mutable result/error fields and follow the engine's established serial execution model. This pass does not make arbitrary non-static prog execution concurrently re-entrant. Recursion tracking is thread-local, and fully-static first-result publication is explicitly thread safe because that path may be reached concurrently.

## Operator And Type Dispatch

Arithmetic-adjacent assignment, equality, and ordering nodes precompute exact dispatch information at compile time. Hot execution paths use exact type checks where the compiler has already constrained the operation. `ProgVariableTypes` compatibility removes collection/dictionary modifiers without allocating new `BigInteger` masks, and legacy enum dispatch uses the generic non-boxing `Enum.IsDefined` path.

These changes do not alter `ProgVariableTypes`, its `v1:<hex-mask>` persistence, compatibility direction, or the `ProgVariableTypeCode` bridge.

## Benchmark Procedure

`MudSharp Benchmarks.FutureProgExecutionBenchmarks` uses `MemoryDiagnoser` and covers invocation, logic, branching, dot chains, loops, collection extensions, result translation, nested calls, static hits, type operations, and representative compilation.

Run it from the repository root in Release mode:

```powershell
dotnet run -c Release --no-restore --project "MudSharp Benchmarks\MudSharp Benchmarks.csproj" -- --filter "*FutureProgExecutionBenchmarks*" --job short
```

The retained-change gate is at least 5% less time or 10% fewer allocations in a relevant benchmark, with no representative regression above 3%. A slot-frame design additionally requires at least 20% less time and 30% fewer allocations across invocation, branch/loop, and nested-call groups, with no regression above 5%.

### 2026-08-12 baseline and retained result

Both runs used .NET 10.0.10, BenchmarkDotNet 0.15.8, the same Windows machine, and the ShortRun job. The baseline was a clean snapshot of `2664cbc0`. Times are means; percentages are retained result versus baseline.

| Benchmark | Baseline | Retained | Time | Allocation |
| --- | ---: | ---: | ---: | ---: |
| Zero parameter | 46.288 ns / 384 B | 18.563 ns / 184 B | -59.9% | -52.1% |
| One parameter | 144.235 ns / 664 B | 132.069 ns / 640 B | -8.4% | -3.6% |
| Multiple parameters | 320.901 ns / 1,072 B | 274.635 ns / 864 B | -14.4% | -19.4% |
| Logic and branch | 426.218 ns / 1,296 B | 279.242 ns / 960 B | -34.5% | -25.9% |
| Dot-reference chain | 204.422 ns / 816 B | 191.622 ns / 792 B | -6.3% | -2.9% |
| Foreach loop | 82.413 us / 65,841 B | 8.056 us / 21,680 B | -90.2% | -67.1% |
| Collection `any` | 10.002 us / 17,496 B | 3.103 us / 10,560 B | -69.0% | -39.6% |
| Collection `where` + `select` | 18.504 us / 38,857 B | 10.214 us / 28,520 B | -44.8% | -26.6% |
| Collection indexing | 6.172 us / 11.22 KB | 1.522 us / 7.71 KB | -75.3% | -31.3% |
| Collection return adapter | 7.082 us / 14,744 B | 2.318 us / 11,032 B | -67.3% | -25.2% |
| Dictionary return adapter | 2.105 us / 5,648 B | 1.894 us / 3,832 B | -10.0% | -32.2% |
| Fully-static cache hit | 45.957 ns / 384 B | 2.548 ns / 0 B | -94.5% | -100.0% |
| Type compatibility | 253.342 ns / 120 B | 186.932 ns / 0 B | -26.2% | -100.0% |
| Legacy type dispatch | 8.763 ns / 24 B | 6.095 ns / 0 B | -30.4% | -100.0% |
| Simple compilation | 11.481 us / 13,056 B | 10.712 us / 13,088 B | -6.7% | +0.2% |
| Representative compilation | 71.597 us / 155,264 B | 69.019 us / 154,203 B | -3.6% | -0.7% |

The zero-parameter retained result is the isolated ShortRun captured after the return-only frame was added, and collection indexing was measured in an isolated ShortRun after that case was added to the harness. The other retained rows come from the final complete 17-case run immediately before those isolated measurements. The parameterless frame does not participate in the parameterised, collection, type, or compilation cases.

The baseline nested-call benchmark could not complete because the old recursion counter accumulated across benchmark invocations. After the fix it completed at 189.415 ns and 736 B per invocation.

## Performance Invariants

- Script syntax, typing, persistence, null/error behaviour, and built-in semantics remain compatible.
- Parameter/local lookup and direct dot-property access remain case insensitive.
- `FullyStatic` caches successful value and null results; failed executions remain retryable.
- `StaticByParameters` remains uncached.
- Recursion limits active depth and always unwinds.
- Loop-local declarations begin each iteration absent from the local scope.
- Collection and dictionary adapters avoid redundant materialisation where ownership is internal.
- Optimisations that fail their benchmark gate are removed rather than retained speculatively.
## Electronic Access-Control Item Functions

Trusted FutureProgs can mutate credential state without physical tools or power and do not emit authentication pulses. Keypads expose `keypadcode(item)` and `setkeypadcode(item, code)`. Biometric readers expose `biometricadd`, `biometricremove`, `biometricclear`, `biometricallows`, and `biometricids`. Cards expose `keycardaddcode`, `keycardremovecode`, `keycardclearcodes`, `keycardhascode`, and `keycardcodes`; readers use the parallel `keycardreader...` functions. Mutation functions return true only for a valid state change, while incompatible query items return empty text, false, or an empty collection.

Typical hooks rotate a keypad with `setkeypadcode(@door, "4821")`, enroll or revoke an identity when access policy changes, issue several zone codes to one card with repeated `keycardaddcode`, and synchronize a reader by clearing then adding the current policy codes. Scheduled revocation should remove the credential from both issued cards and accepting readers when policy requires both sides to forget it.
