# FutureProg Execution Runtime

## Builder source editing

Submitting `prog set append` retains the existing source and appends the editor text, adding a line break when needed. An empty append leaves the source unchanged. `prog set text` replaces the source. Cancelling either editor leaves the source unchanged; submitted source follows the normal compilation path. Prog deletion remains unsupported while persisted callers cannot be safely reassigned automatically.

## Durable Vancian notifications

`ExecuteWithStatus` reports execution/parameter-conversion errors and deliberately runs notifications without cached results. Vancian selection and refresh operations durably mark callbacks as invoking before calling this entry point; a failed or interrupted callback requires inspection and is never automatically replayed on reload. This leaves ordinary `Execute` and its existing caching semantics intact. The [Vancian API](../Magic/Vancian_Magic_FutureProg.md) uses typed functions and the same guarded service as player commands and crafting.

## Purpose

Persisted program source uses MySQL `mediumtext`, preserving the existing UTF-8 collation and required-value contract. Large generated world-population programs can exceed the former 64 KB `text` limit; this expands source storage without changing the language, compiler or execution model. Migration rollback to `text` requires sources to fit that smaller column first.

`loadeditems()` and `loadedcharacters()` return snapshots of currently loaded world entities, without loading offline database records. The character snapshot uses the combined active actor collection, including both players and NPCs. Characters expose `npctemplateid` (zero for players) for native population predicates. `setlockpattern(item, number)` sets native key/lock patterns using whole numbers from zero through Int32.MaxValue; invalid inputs leave patterns unchanged.

`LocationOnGameLoadFinished` fires for loaded rooms after persisted NPCs are available, allowing native boot population hooks to inspect existing actors before creating replacements. The global heartbeat starts after boot-completion callbacks and Discord startup, so time spent executing boot population hooks does not accumulate overdue heartbeat ticks.

`setliquid(item, liquidId)` retains its full-fill/invalid-ID-empty behaviour. The three-argument overload `setliquid(item, liquidId, volume)` sets an exact final volume in base fluid units, accepts explicit zero emptying, and rejects invalid IDs or negative/nonfinite/over-capacity volumes before changing contents.

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

Reference-type `null()` values can be represented by a null `IProgVariable`. Assignment to a previously cleared reference therefore uses the compiler-checked assignment type when the existing value has no runtime type; clearing a variable does not prevent later reassignment, including from nested scopes. Compile-time type compatibility remains enforced.

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

`removeitem` accepts a compatible element type and removes the first matching value from a local collection, leaving later duplicates intact. Primitive wrappers compare by their underlying value, text compares case-insensitively, and an absent value leaves the collection unchanged. Both `removeitem values value` and `removeitem @values value` use this behaviour; incompatible types are rejected at compilation.

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

The `startcombat` built-in assigns its world to the new `ProgCombat` before joining either participant. Each join uses that world's scheduler before invoking the join callback. A regression checks both schedules, the shared combat and one callback per participant in order. Optional callback name lookup retains the existing abbreviation semantics; callers can use an explicitly absent name when no callback is desired.

- Script syntax, typing, persistence, null/error behaviour, and built-in semantics remain compatible.
- Parameter/local lookup and direct dot-property access remain case insensitive.
- `FullyStatic` caches successful value and null results; failed executions remain retryable.
- `StaticByParameters` remains uncached.
- Recursion limits active depth and always unwinds.
- Loop-local declarations begin each iteration absent from the local scope.
- Collection and dictionary adapters avoid redundant materialisation where ownership is internal.
- Optimisations that fail their benchmark gate are removed rather than retained speculatively.
## Native physical-state and furniture functions

`drunkennesslevel(character)` reads native needs categories as severity 0 through 5: sober, buzzed, tipsy, drunk, very drunk, and blackout/paralytic. `isoffhand(character, item)` checks the item's actual current left/right holding or wielding location against the character's dominant hand; neutral inventory slots return false. `spendstamina(character, amount)` uses native affordability and expenditure, returning false without spending for invalid or unaffordable amounts.

`attachchair(chairItem, tableItem)` attaches a native unassigned chair through the table's normal capacity checks and persisted chair relationship. The same existing assignment succeeds without another mutation; a chair assigned elsewhere is refused. Identical parents and containment cycles are refused before changing either item. This supplies a prog action for existing furniture, not another seating or speech system.

Needs queries/actions and threshold-event contracts are described in [AI events and hooks](../AI/Event_System_for_AI_and_Hooks.md). Saved combat-delay effects created by `addcombattempoeffect` are described in [combat strategy runtime](../Combat/Combat_Strategy_Runtime.md).

## Electronic Access-Control Item Functions

Trusted FutureProgs can mutate credential state without physical tools or power and do not emit authentication pulses. Keypads expose `keypadcode(item)` and `setkeypadcode(item, code)`. Biometric readers expose `biometricadd`, `biometricremove`, `biometricclear`, `biometricallows`, and `biometricids`. Cards expose `keycardaddcode`, `keycardremovecode`, `keycardclearcodes`, `keycardhascode`, and `keycardcodes`; readers use the parallel `keycardreader...` functions. Mutation functions return true only for a valid state change, while incompatible query items return empty text, false, or an empty collection.

Typical hooks rotate a keypad with `setkeypadcode(@door, "4821")`, enroll or revoke an identity when access policy changes, issue several zone codes to one card with repeated `keycardaddcode`, and synchronize a reader by clearing then adding the current policy codes. Scheduled revocation should remove the credential from both issued cards and accepting readers when policy requires both sides to forget it.


## Saved native character statuses

Trusted progs can create spell-independent statuses without inventing a caster or parent spell. `addblindnesseffect`, `addsilenceeffect`, `addflighteffect`, `addvisualinviseffect`, `adddetectinvisibleeffect`, `adddetectetherealeffect`, `adddetectmagickeffect`, `addinfravisioneffect`, `addsneakeffect`, `addhideeffect`, `adddetectpoisoneffect`, `addclimbingeffect` and `addpacifismeffect` accept a character and optional duration in seconds. Omitted/zero duration is indefinite. They return the new effect, an existing matching effect without stacking or refreshing, or null for invalid input. Pacifism attaches to the body consumed by native combat admission; other statuses attach to the character. Hiding uses the native terrain hide check. Flight grants capability; callers separately request a flight posture. Climbing grants native climbing ability without inventing a starting grip or posture. Poison sensing uses the native toxin-inspection contract.

`addmovementtempoeffect(character, multiplier)` creates a saved native movement-delay multiplier within 0.1–10. `addspottingeffect(character [, bonus])` and `addlisteningeffect(character [, bonus])` grant their corresponding native check bonus while awake, conscious and outside combat; omitted bonus is the configured `CheckBonusPerDifficultyLevel`. Explicit bonuses must be within -1000–1000. Matching awareness statuses do not stack or refresh.

Listening awareness also permits adjacent-room ordinary speech through one authored exit at the same layer. Native volume attenuation and listening checks apply; closed doors increase difficulty. Self-loops and duplicate exits cannot duplicate delivery. Nearby NPC speech events follow the existing raw-text/language event contract and run after local speech events. NPC output-handler terminal delivery is not required for an NPC to witness speech. Yelling, shouting and radio transmission retain their existing propagation.

`adddamageprotectioneffect(character, multiplier, hits)` creates a saved post-armour body-damage protection. Multiplier must be within 0–1 and hits a nonnegative integer; zero hits means unlimited. Matching configurations are not stacked or refreshed. One qualifying hit consumes a finite charge before removal callbacks, including when callbacks reenter damage. See [damage pipeline](../Health/Damage_System_Design.md) for packet, preview and health-cost boundaries.

`applyplanarstate(target, state, plane, crosscloseddoors, canmanifest, candissipate)` adds explicit permissions to the existing three-argument operation. Existing overloads retain their defaults. The Armageddon onload mapping uses false for all three permissions; no permission to cross magical barriers is implied.

## Native relationship and item queries

Character `.following` returns its current native follow target. `.bodyguarding` returns only an already loaded target, while `.bodyguardingid` retains the assigned ID (zero for none). Neither query loads offline characters. `.isguarding` reports native character, item or exit guarding. NPC `bodyguard` inspection, clearing and retargeting likewise avoid loading an absent former target merely to notify it.

`targetitem(viewer, items, keyword)` selects a visible item from the supplied item collection using native keyword and numbered-target rules, returning null if none matches. This supports carried-stock dialogue without creating merchandise or replacing real item custody with prototype counts.


## Witnessed legal reports and handover

`reportcrime(law, criminal, witness, victim, item, context)` records and reports one applicable witnessed violation through its native legal authority, including laws without automatic hooks. Returns the crime or null; victim/item may be null. `enforceablecrime(character, authority)` selects the native patrol arrestability/identity candidate or null. `issubduedby(target, holder)` checks actual attached native controlled grappling, rather than generic helplessness. `handovercriminal(holder, target, receiver, authority)` validates and cleans physical custody before invoking native remand and belongings handling, returning whether intake occurred. See [law runtime](../Economy/Law_System_Runtime.md) for callback, permission and refusal boundaries.


`setroomweather(location, weatherEventId)` sets a saved native room-weather effect, deduplicating identical settings; zero clears it and restores regional conditions. Invalid IDs, cross-world rooms and events requiring independent five-second processing are refused. It changes local current weather only; regional forecasts and the temperature baseline remain regional. The Armageddon sandstorm controller keeps its route, random chances, position registers and deposit loading in generated progs.

`depositroomresidue(location, solidId, amount, maximum)` adds dry native surface residue to exposed bodies and belongings, bounded by each recipient's existing amount of that material. Amounts use base mass units. Invalid materials, nonpositive amounts/caps and sheltered rooms refuse. The action uses ordinary native surface persistence, descriptions and cleaning; no new saved dust-marker effect is created.

## Paid liquid refills and NPC delivery

`liquidrefillcost(item, liquidId, unitVolume, unitPrice)` returns the price of the missing volume, rounded up to a whole base currency unit; zero means full and -1 means invalid, closed, projection-only or incompatible contents. The vessel must support `IPreparedLiquidContainer` (implemented by the ordinary liquid-container component); machinery, implants and projected grid sources are not accepted by this sale helper. Only an empty vessel or one whose constituents have the requested origin liquid is compatible. Existing matching constituents are preserved when the refill is added. Invalid/fractional/out-of-range liquid IDs are rejected.

`purchaseliquidrefill(buyer, seller, item, currency, liquidId, unitVolume, unitPrice)` uses that same quote and native exact held-cash transfer. The vessel must remain directly held or wielded by the buyer, open, unchanged and accessible; the awake noncombat seller must remain colocated. Refusal returns false without filling. Currency and the prepared refill are published before notifications.

`purchasenpc(buyer, seller, templateId, currency, amount)` returns one delivered NPC or null on refusal. It accepts approved simple accountless native templates supported by the existing deferred native creation adapter. Preparation, native admission and final cash checks precede publication; an abandoned private candidate is released and its owned database rows removed. A successful animal is placed beside the buyer without assigning ownership or forcing mounting. Call native `canmount`/`mount` separately when immediate riding is desired. Postcommit observer errors do not reverse a completed purchase.

## Empty-vehicle relocation

`vehicleoccupied(item)` includes live and offline compartment residents, but permits cargo. `relocateemptyvehicle(item, location, unhitchCharacters)` moves one existing empty stationary vehicle between ordinary rooms and can release draft characters at its origin. `purchasevehicletransfer(buyer, seller, item, location, currency, amount)` uses a prepared native cash participant to publish payment and relocation together; receipt policy remains in the calling prog. Both relocation actions preserve actual vehicle/interior/cargo identities and refuse occupied vehicles, active journeys, route rooms and attached vehicle trains. See the [vehicle contract](../Vehicle_System.md#prog-relocation-of-empty-vehicles).
