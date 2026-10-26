# Framework extension contracts and performance

The shared helpers in `FutureMUDLibrary/Framework` serve command parsing, player
output, collection selection and numeric formatting. Their XML source comments
are the per-method reference. Preserve invariant-culture comparisons, visible-text
markup rules, enumeration order and empty-input behaviour when optimising callers.

## Allocation boundaries

- `EqualToAny` keeps its `params string[]` entry point for existing binaries and
  explicit arrays. The additional `params ReadOnlySpan<string>` overload allows
  ordinary argument lists to avoid a temporary array with the repository compiler.
  Both use invariant-culture, case-insensitive equality and allow null candidates.
- `RawTextLength(ReadOnlySpan<char>)` counts visible UTF-16 characters without
  allocating. The string overload delegates to it and treats null as empty.
- `TryCopyRawText` writes into a caller-owned character span. It returns false when
  the buffer is too small and reports the prefix written. The destination may
  start at the same address as the source for in-place compaction; callers must
  otherwise avoid overlapping storage. It suppresses ANSI text through `m` and MXP
  text through the closing delimiter, including unterminated tails, as `RawText`
  historically did.
- `RawText` returns the original string when there is no markup. A changed result
  uses bounded stack storage or a returned array-pool buffer, plus the final string.
  `CollapseString` similarly removes only
  literal spaces, underscores and hyphens, returning the original if unchanged.
- `ReplaceFirst` avoids temporary prefix/suffix strings. Its output still requires
  a string allocation when a replacement occurs.
- `ListToString` enumerates streaming sources once and builds the output without copying the source,
  taking a sublist or allocating an article-prefixed string per item. Null items
  and articles consistently behave as empty text, including lists of three or
  more items. Separator, conjunction, two-item and Oxford-comma options retain
  their established formatting. Indexed sources are measured and copied directly
  into the final string without an intermediate builder.
- `GetRandomElement` accesses indexed collections directly; arrays and read-only
  spans have allocation-free overloads. Other enumerable sources are materialised
  once. Empty input returns default and consumes no random draw; nonempty input
  consumes one draw.
- Weighted selection enumerates its source once and still caches evaluated
  weights. Evaluators are called once per item. These APIs expect nonnegative
  finite weights and a positive total; they are not validators for arbitrary
  weight distributions.

Span overloads do not make string-producing APIs allocation-free. Colour wrappers
already concatenate into a returned string; adding parallel span signatures would
not remove that necessary allocation. Iterator-producing collection helpers also
retain their deferred-execution contracts instead of materialising every source
to obtain a span.

## Correctness boundaries

- `Odd` recognises negative odd integers.
- `IsPowerOfTwo` uses a constant-time bit test instead of a lazily populated hash
  set; negative values including `long.MinValue` are not powers of two. This also
  removes shared mutable initialisation state.
- Integer-evaluator weighted selection keeps the random threshold fractional,
  fixing the first-item bias caused by integer truncation. All four weighted
  overloads now agree for equivalent ordinary positive weights and seeded draws.
- `CollectionDictionary.ContainsValue` searches the values inside each keyed list,
  using the list's ordinary equality semantics. Its read-only view delegates to
  the same implementation.
- `PickRandom` selects distinct values, not distinct source positions. Repeated
  source entries retain their historical selection weighting. Impossible requests
  now throw instead of looping forever. Valid requests retain their random draw
  sequence; this intentionally retains rejection sampling rather than replacing
  it with a shuffle that would change seeded results.
- CSV formatting preserves leading, repeated and trailing empty fields. RFC mode
  quotes carriage returns as well as line feeds, separators and quotes. Null
  fields are empty fields.
- Composite-format validation uses `CompositeFormat.Parse` as the syntax and
  index-bound authority, then checks required argument presence without regex
  matches or temporary split strings. Aligned placeholders, empty format
  specifiers and real placeholders inside escaped literal braces count as
  present. Escaped placeholder-looking text does not. Negative expected counts
  and undersized mandatory masks return false.
- `WrapText` always advances after emitting a segment, including unbroken words
  and one-character widths. Effective content width is at least one character
  when indentation consumes the requested width.

## Verification

`FrameworkExtensionOptimisationTests` covers legacy ANSI/MXP state-machine parity,
span capacity and in-place copying, culture-sensitive comparisons, single-use
enumerables, list and CSV boundaries, required format arguments, wrap progress,
random selection and numeric/dictionary regressions. Its warmed allocation check
asserts zero managed allocations for the span search/copy/count and array random
paths. Before/after allocation comparisons retain the previous implementations
for representative `RawText`, replacement and list-formatting inputs. Timing is
reported as diagnostic evidence, without environment-sensitive speed assertions.

Run the owning library suite in Release configuration with the repository test
wrapper. Also build/run relevant runtime consumers because adding overloads can
change source overload resolution even when the original method is retained.

### Review and measurements, 26 October 2026

Source inspection covered the extension methods throughout the Framework tree;
all 306 public extensions and the private array shuffle have nonempty, well-formed
XML summaries. Static production call-site counts used for prioritisation included
approximately 1,407 `ListToString`, 642 `EqualToAny`, 255 `CollapseString`, 200
`GetRandomElement` and 126 `GetWeightedRandom` calls outside the library and test
projects. These counts are a routing aid, not runtime profiling.

The representative Release allocation comparison, over 10,000 calls per case,
reported:

| Helper | Previous bytes/call | Revised bytes/call |
| --- | ---: | ---: |
| `RawText` | 280 | 72 |
| `ReplaceFirst` | 200 | 88 |
| `ListToString` | 656 | 128 |

The warmed span search/copy/count and array random-selection loop allocated zero
managed bytes. The final timing sample was faster for all three string cases,
with tiered compilation disabled to compare optimised implementations. These are
small input-specific measurements, not end-to-end game-server throughput claims.

Validation evidence:

- Library Release suite: **829 passed, 0 failed, 0 skipped**;
  `.artifacts/test-runs/20261026T055646Z-cf6ddac933c2`.
- An earlier library run caught an obsolete assertion that negative three was not
  odd. The assertion was corrected to reflect the fixed arithmetic behaviour.
- A sandboxed core build stalled and was stopped. A targeted serial no-restore
  build outside the sandbox completed with zero errors; the successful build also
  compiled the core test project's seeder dependencies.
- Core Release suite: **8,380 passed, 1 failed, 0 skipped**;
  `.artifacts/test-runs/20261026T061246Z-bb9fa76b2d67`. The failure was
  `CombatSimulationRuntimeScope_DoesNotFlowSandboxServicesToChildWork` in
  `MudSharpCore Unit Tests/CombatSimulationTests.cs`: a child task observed the
  simulated clock. The clock/context path is unchanged and does not call the
  modified helpers.
- That test passed in an isolated rerun with default tiered-JIT settings:
  `.artifacts/test-runs/20261026T061831Z-2c486a081986`. The original full-suite
  failure is retained; the isolated pass does not turn the full run green.

Lower-use adjacent review candidates were left outside this optimisation slice:
`LongRandom` uses modulo-based sampling and needs an explicit equal/reversed/full
range contract before changing its draw sequence; `DetermineMonthDayOrder` scans
date-pattern letters without skipping quoted literals. For example, literal `M`
before a `dd/MM` pattern can be mistaken for a month field. The extrema helpers
also retain their existing buffering and selector/equality contracts rather than
silently replacing them with `MinBy`/`MaxBy` semantics.
