# Fury and Calm paid casting and optional installation

This continuation implements bounded source Fury/Calm invocation and development-only installer mappings. See the [continuation checkpoint](Armageddon_Continuation_20261008.md) for revisions and verification. The full Armageddon programme remains incomplete and its package is disabled in Release.

## Supported invocation

`sourcefury` and `sourcecalm` are registered spell-effect templates. Use one exclusive target effect, a character/self trigger, a seven-grade configured casting route, duration expression `0`, no caster effects, no area profile and no ordinary opposed trait. The emotional profile supplies its own retained lifetime and Calm save configuration. Unsupported shapes refuse before payment; direct effect application, prepared attack/trap payloads, scrolls and charged-device storage remain unsupported.

Preparation captures the actual caster/recipient bodies, Room/IRoom identity and layer, native caster terrain, source profile, native trait/prog references, exact retained source-emotional parent/child cohort and deadlines. Reflections have a separately admitted caster recipient. Confirmations reuse the captured endurance draw and combat ticket. Callback-free checks run immediately before payment; callback-bearing eligibility and output/save boundaries are checked again before effects change. A changed cohort, binding or physical identity preserves replacement state and produces the configured service's refusal or paid `NeedsReview` result.

Only `SpellSourceFuryEffect` and `SpellSourceCalmEffect` participate in reciprocal source counters. A stronger opposing grade is reduced by the incoming grade; equal strength removes it and exhausts the incoming spell; weaker opposition is removed and the residual incoming grade proceeds. Countering occurs before Calm's residual-grade save. Generic rage, pacifism and psychic emotion effects are independent.

Fury uses the configured ordinary attribute and positive native units per source endurance point. Its source grade, native power, intensity and endurance draw remain distinct. The ordinary body stamina-capacity path is reconciled without granting or refilling current stamina. Fury recasts retain the existing parent/child, power and modifier while extending the schedule, avoiding an intermediate removal clamp.

Positive residual Calm prepares selective cessation before payment whenever the recipient is in combat. Unsupported combat cannot be paid and then substituted with blanket teardown. A counter that exhausts the incoming Calm does not require cessation. After the original caster's casting roll and residual-grade save, even resisted Calm performs its admitted cessation. It preserves unrelated fights and callback-created replacement combat. A replacement prevents applying a Calm child to the new fight and quarantines the paid operation. A useful counter or cessation is reported as applied; a pure resisted cast is rejected; an unchanged capped recast is `NoChange`.

Calm opts into the existing admitted incoming hostile-attack notification. Its stock break policy removes only that owned child, regardless of the later hit/miss result. Refused attacks emit no notification. A sibling child remains owned by its parent. This implementation adds no new attack-family hook qualification.

## Lifetime and persistence

Stock units are 600 seconds. Fury caps at 36 units and Calm at 24. Fury terrain durations are Air `2g`, City/Inside `floor(2.5g)`, Hills/Mountain `floor(3.5g)`, Thornlands `floor(3.3g)`, Earth `4g`, with fallback `3g`. The inclusive endurance draw is `g..floor(g*g/2)`; inverted bounds return `g`, and Earth adds `g`. Calm uses `2g`. Remaining time is rounded up to source units on recast. Calm retains the stronger source state; equal source grades with conflicting native mappings refuse before payment.

The resolved duration must fit both `TimeSpan` and the current native UTC deadline before payment. An oversized configurable cap is accepted when the actual selected duration remains schedulable.

Each effect has an ordinary scheduled `MagicSpellParent` with child-owned source state. An empty parent is attached first; its child becomes serializable only after entering the owner's effects. Attachment failure removes an empty wrapper, preventing save/load from replaying an unapplied child. Parent expiry/removal uses ordinary child cleanup. Child XML retains grade, power, intensity, endurance, attribute mapping, unit scale and Calm attack-break policy. Older valid version-one Calm children default to the approved break policy; malformed definitions remain available for repair.

Managed tests exercise the native scheduler and actual parent/child XML loader. The [native qualification receipt](Armageddon_Fury_Calm_Native_Qualification_20261009.json) records four passing installed-world phases using the maintained disposable MySQL runner and Telnet helper. They cover paid casting, production backup/restore, both effects' cold restart and paused offline schedule, configured trait probes, counters, retained recasts, both caps, removal and scheduled expiry. Full programme balance and high-volume acceptance remain separate gates.

## Optional prepared-world mappings

Add `Emotions` to the existing strict binding document; omitted/null preserves installed owned Fury/Calm definitions without reconciling them. The [builder guide](Armageddon_Partial_Installer_Builder_Guide.md#optional-furycalm-mappings) specifies the fields. No new question IDs, defaults or replay answers are introduced. Standard Debug replay still declines the package. Release remains disabled.

`FuryAttribute` may be null only for unambiguous inference. The installer reuses SkillPackageSeeder's Constitution/Physique/Endurance/Body aliases and requires exactly one ordinary body-owned attribute. Missing, ambiguous or derived-only matches require an explicit override. No runtime name inference occurs. Units, intensities, seven distinct native terrain IDs, Calm save trait and seven named save difficulties must be authored. Eligibility progs must compile as Boolean `(target character, caster character)` and remain external builder-owned records.

The six managed rows are two spells, two duration expressions and two cost expressions under `reviewed-fury-calm`. They use the shared field-level `SeederManagedRecordReconciler` integration also used by Water/See. Names, profile XML, expression overrides and capability admission removals survive reruns. Missing/retired rows, unknown versions/keys, competing ownership and unowned name/stock-identity collisions block reconciliation; rows are never adopted or resurrected automatically.

Composition uses owned source skills from the tradition bootstrap, commits this contribution independently and then reconciles the final tradition plan once. The Unravel → Fury → Calm → Mend chain uses the exact source openings, parent thresholds and caps, including Mend's cap 60. Earlier completed modules remain committed if a later module fails; lost acknowledgement reports committed-but-unconfirmed status and fresh reruns resolve the same identities. No player acquisition, mastery, enrolment, classes, charges, item instances or resource/stamina refill is created.

When Pierce is already installed, composition validates its four retained ownership records and includes its spell identity in the bootstrap plan. This prevents the bootstrap from temporarily dropping an admission that final composition restores, which previously changed ownership audit timestamps on an otherwise unchanged rerun. Invalid retained records block before any composed module mutates.

With provisions, Water/See and Emotions selected, the managed fixture has **217 records, 12 payload definitions and 12 stored source admissions per variant**, leaving 70 of the 82 Sorcerer spells without stored admission. Omitting Emotions from a world where it has never been installed retains the historical 211/10/9 configuration. These are installer coverage counts, not 154-candidate completion or gameplay proof.

## Acceptance boundary

| Surface | Managed evidence | Native gate |
| --- | --- | --- |
| Fury | Paid grade lifetimes, terrain/endurance policy, independent source/native state, reciprocal counters, no refill/removal capacity, capped recast and deadline refusal | PASS: paid casting, configured trait/no refill, exact retained child, production restore, cold restart, counters, recasts, cap, removal and expiry |
| Calm | Paid residual save using original caster roll, counter-before-save, prepayment unsupported cessation, captured-ticket cessation on resistance, replacement-combat preservation, admitted notification removal, capped recast | PASS: paid casting, selective cessation preserving unrelated fights, admitted missed KICK, actual failed-roll resistance, callback refusal/replacement, durable quarantine/reconciliation, lifecycle and cold restart |
| Installer | Strict JSON, inference/override diagnostics, 217/12/12 composition, builder edits/admission removal, selected/null reruns, tombstones/deletion/version/ownership and lost acknowledgement | Explicit 217/12/12 selected/null reruns, actual uncommitted payload rollback, committed lost acknowledgement and stable retry: PASS. The stopped failure fixture restores its exact original builder definition |

Undead, Quickening, Insomnia and Mul/Thodeliv historical exceptions remain explicitly unmapped. Native intensities, attribute units, save difficulties and terrain proxies are authored adaptations. Eligibility progs are available for world policy, but this checkpoint does not certify complete historical exception parity. Specialized combat families retain the exclusions in the [shared hook receipt](Armageddon_Emotional_Combat_Hooks.md).

Native callback qualification exposed `startcombat` constructing a `ProgCombat` without its world. The factory now supplies the world before scheduling either participant. A managed regression verifies both schedules and ordered join callbacks; the installed callback cases exercise the corrected factory. Earlier phase hashes remain in their receipts and are not relabelled as executions on the final binary. Earlier fixture mistakes and failed attempts are retained, including superseded body-only skill-cleanup claims. The accepted combat run uses effective owner scope, explicit controlled recovery and exact restoration checks.
