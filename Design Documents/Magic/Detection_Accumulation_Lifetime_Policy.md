# Editable detection accumulation

This optional effect-template contract adds source-style accumulation to one `detectinvisible` effect. Absent policy preserves ordinary duration, exclusive and nonexclusive behavior. It uses existing casting, parent ownership, operation reporting, effect persistence and scheduling; there is no database migration or separate timer. Stock opt-in and its qualification are recorded separately.

## Authoring

Use `spell set effect 1 lifetime accumulate <group> <seconds-per-unit> <maximum-units>` or `spell set effect 1 lifetime off` through the normal effect builder. The existing spell duration expression supplies the new increment. The supported shape is one detection effect, no other target/caster effects, and exclusive replacement. The single effect may be target-side or caster-side. An increment must be finite, positive, representable and an exact whole number of configured units. Outcome-dependent `degrees`/`success` increments are refused because their value is unavailable before payment.

Template XML contains an optional `LifetimePolicy` element with version1, mode `accumulate`, group, unitSeconds, maximumUnits and retainStrongestGrade=true. Invalid XML is retained for builder repair and refused, rather than silently treated as ordinary duration. Policy/group edits conflicting with live grouped parents refuse until those parents expire or are deliberately handled by the builder; there is no hidden migration.

The shared interface and immutable policy/state records live in `FutureMUDLibrary/Magic/IMagicSpellEffectLifetimePolicy.cs`. Existing mandatory spell-template and parent interfaces are unchanged. `DetectInvisibleEffect.Lifetime.cs` implements XML, builder and presentation hooks; `MagicSpell.Lifetime.cs` owns the concrete resolution and guard.

## Group, strength and payment

Grouping is recipient-local by an explicit source-group key, across spell IDs and casters. Untagged detection and other groups remain independent of opted-in cleanup. Each replacement has a fresh parent UUID and current spell/caster provenance. Its optional persisted lifetime state records the policy and strongest source grade separately from native `SpellPower`; the mapped power belonging to a stronger prior grade is retained. Equal-grade prior parents with conflicting mapped powers are refused, rather than selected by enumeration order.

An explicit selected grade is required. Configured casting supplies it through the existing casting copy. The prepared route must likewise receive a selected-grade copy; an unbound legacy release is refused instead of deriving source grade from an editable power ordinal. No charged-carrier or snapshot format is changed by this boundary.

`Prepare` validates live policy/recipients before payment, and repeats existing preparation under the normal invocation guard. `CastSpellCore` captures exact parent references, UUIDs, persisted metadata, child references and absolute schedule deadlines before commitment. It admits the caster as a possible reflected recipient for character targeting. The payment closure confirms that captured cohort; resolution confirms it again after payment. A callback-added parent, changed timer or changed ownership invalidates the operation instead of becoming a guessed replacement.

The finaliser attaches and proves the new parent, child and native schedule before removing previous parents. It rechecks the remaining captured cohort and replacement between every removal callback. Newly created same-cast parents are excluded. A retained partial child receives its bounded parent schedule while previous parents remain for reconciliation. Paid ambiguous mutation uses the existing quarantine/review-required flow, with no automatic replay or refund.

Lifetime resolution belongs only to the phase whose template opts in. For a caster-only policy the empty primary phase preserves its admitted cohort; existing untagged empty-phase behavior remains unchanged. The prepared route captures target and caster cohorts before duration evaluation, resistance and output callbacks, then confirms the relevant snapshot before parent construction. The retained diagnostic `pierce-policy-caster-diagnostic-01` showed the previous prepared empty phase losing source grade7 to grade1; subsequent regressions cover both configured/prepared caster-only construction and post-admission callback mutation on both sides.

Absent-policy prepared parents also retain their original zero transient `ResolvedDuration` field. Their existing scheduler duration is unchanged. Opted-in prepared parents alone receive the resolved policy duration; configured casting retains its pre-existing duration initialization.

Configured application reporting is deferred until parent attachment and cleanup succeed. A paid replacement with the same observed absolute deadline and retained grade/power reports no intended application; extending its deadline or increasing its strength reports applied. The comparison uses actual scheduler expiry, not OriginalDuration or merely allocated children. No-change casts therefore do not earn mastery application samples.

## Historical clock and native lifecycle adaptation

Historical `codedump.c` Library `libfile_befb4ae78d1c8191aaa2640c49912d9d` documents detect-invisibility duration5*grade and cap48 at84574-84608; `stack_spell_affect` at81574-81592 adds same-affect durations, retains maximum power and replaces all previous affects. RT_ZAL_HOUR=600 at24979. `affect_to_char` at49438 sets expiry to initiation + unit*duration - 1 second, and `affect_update` at172611 reconstructs positive remaining duration with integer(delta/unit)+1.

The policy captures proven native absolute expiry for mutation checks and reads remaining time through `IEffectScheduler.RemainingDuration`, computes positive remaining units with ceiling(remainingSeconds/unitSeconds), adds the validated increment and clamps to the configured cap. Boundary tests compare integer source-clock examples with that formula. Native exact deadlines normalize the historical inclusive one-second endpoint, including the final second and native subsecond precision. They do not claim literal historical scheduler polling behavior.

Native world hours are independently configurable; the stock's600-second unit is a historical real-time mapping, not proof that every configured world clock has that hour length. Parent persistence uses the existing saved `Remaining` milliseconds and cached schedule restoration, plus optional lifetime metadata. Native offline/cached time pauses/restores the saved remaining duration; this is an explicit existing-engine lifecycle adaptation, not a new assertion of historical offline behavior. No second absolute expiry is persisted by the policy.

Malformed parent lifetime XML is preserved with an error rather than silently deleting historical records. Matching malformed metadata, unknown grouping, duplicate UUIDs, mixed/uncertain children, permanent or unobservable schedules and conflicting policies refuse before payment. Tests and the entry-specific native receipt provide qualification; this design document alone does not mark stock complete.
