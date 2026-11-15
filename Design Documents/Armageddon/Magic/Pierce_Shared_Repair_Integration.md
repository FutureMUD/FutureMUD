# Pierce shared runtime repairs and remaining lifetime dependency

The coordinator explicitly assigned this lane the existing `Body.CanSee` blindness branch and the exclusive parent-removal calls in `MagicSpell.CastSpellCore`'s local `ApplySpellEffect.FinaliseParent` and `MagicSpell.ResolvePreparedSpell`. Production changes are limited to these three lines. There is no global removal-default, interface, schema, payment, generic perception or stock-factory change.

## Shared repairs

- `Body/Implementations/BodyPerception.cs:188` now checks body-local or actor-local `AffectedBy<IBlindnessEffect>()`. Existing `Effect.IsEffectType<T>()`/`Applies()` semantics are retained. The check remains inside the existing non-sensor-array branch, after the unchanged administrator/self/ignore and inventory/exit exemptions.
- `Magic/MagicSpell.cs:1826` and `:2035` now explicitly pass `fireRemovalAction:true` when removing old exclusive parents. Existing `MagicSpellParent.RemovalEffect` removes old children under its existing removal guard and capacity scope. The new head has not yet been attached to the target when this selection is captured, so it is not selected for removal. Nonexclusive stacking and the global default `false` are unchanged.

The dedicated `pierce-shared-runtime` native mode verifies applicable character-owned blindness before and after detection, body-owned blindness, false applicability, self/ignore exemptions, configured and prepared exclusive cleanup, old-child removal callbacks observing the new child intact, old schedule removal, configured and prepared nonexclusive independent deadlines, sibling-preserving expiry, unchanged opt-in global removal, and fresh-process current-parent reload/expiry. It uses the lane's own guarded disposable MySQL instance and dedicated output paths. This mode qualifies shared runtime repairs, not historical stock completeness.

The preceding `Armageddon_Pierce_Concealment_*` receipt/verification/integration files are immutable historical records of source `57dcb2c0`. Their runtime failures are superseded only by the newer shared-repair execution. Their replacement-refresh adaptation does not acquire content approval from the runtime fixes.

## No authority for replacing historical accumulation

The authoritative brief is Library `libfile_a4606cd0097081918d6c9d9e220e6da0`. Line24 says old catalogue proposals are not individually approved; line311 requires actual stacking configuration; Appendix C line786 requires source matching where evidenced. No inspected brief passage explicitly authorizes replacing Pierce's historical accumulation with simple refresh. Source detail is available and native scheduling can express accumulation, so the existing replacement definition remains provisional and must not be marked stock-clear.

Historical `codedump.c`, Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, establishes:

- `spell_detect_invisibility`, 84574–84608: duration `5*level`, Silt refusal, recipient invisibility perception and cap48 via `stack_spell_affect`.
- `stack_spell_affect`, 81574–81591: sum previous duration(s), retain maximum previous/new power, clamp to maximum, remove previous affects and install one replacement.
- `RT_ZAL_HOUR = 600`, line24979. `affect_to_char`, line49438, derives absolute expiry from initiation plus this constant times duration, minus one second. `affect_update`, 172611 onward, reconstructs remaining duration as `delta/RT_ZAL_HOUR + 1` while live. The distinct `SECS_PER_MUD_HOUR = 100`, line25068, is used for another update path and is not the spell expiry conversion.

Thus five source hours map to 3,000 real seconds per grade and cap48 to 28,800 real seconds. Native exact `TimeSpan` deadlines normalize the source's inclusive one-second endpoint convention. Source remaining duration is quantized to whole source hours, not merely an arbitrary continuous refresh.

FutureMUD's world clock is independently configurable: `TimeAndDate/Time/Clock.cs` loads `SecondsPerMinute`, `MinutesPerHour` and `InGameSecondsPerRealSecond`; `ClockManager` advances according to the latter. There is no universal native 600-second hour. The currently authored expression is a declared historical real-time conversion; it must not be described as proof of five current-world hours under every builder clock configuration.

## Minimal dependency to coordinate before accumulation work

There is presently no editable effect-template lifetime policy that can resolve a target's accumulated deadline and strongest retained parent power before its parent is constructed. `DetectInvisibleEffect.Apply` can attach/report a child, but the parent uses an init-only `ResolvedDuration` and each enclosing route later attaches it with its independently computed `duration`. Scheduling a second private timer or rescheduling the old parent only would conflict with exclusive finalisation.

A bounded proposed addition is an optional effect-template lifetime-policy contract, used before parent creation in those same two owned routes. It should supply resolved parent duration and parent power while leaving paid/captured cast grade and effect application power unchanged. Only the Pierce stock opts in; missing policy preserves existing behavior and XML compatibility. Its normal builder controls should expose accumulation, source unit seconds and maximum units.

Source policy should select live parents of this exact spell on the recipient, derive remaining source-hour units from their proven native expiry (source-style quantization), add `5*grade`, clamp48, and retain the maximum existing/new parent power. It then uses ordinary one-parent/child replacement and the repaired cleanup. Expiry uncertainty, permanent/missing timers, conflicting policies, no-op reporting at an unchanged cap/deadline/power, and save/reload of edited policy need explicit tests and a coordinated contract. No new API or parallel lifecycle implementation is added in this checkpoint.

Pierce content remains BLOCKED on that dependency/design choice. Silt admission before payment, no ethereal grant, no blindness cure, source opening30/cap90/minimum energy7 and separately authored acquisition/delivery policy remain unchanged. No central progress/repertoire ledger, installer, historical receipt, shared native dispatch or remote operation is edited.
