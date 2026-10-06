# Armageddon emotional combat hooks

This dependency checkpoint continues published master `39eb9d32d2012f56ac3ef33c441b5dac7c3535e3` in the isolated `codex/fury-calm-runtime-hooks` worktree. It implements the shared combat hooks allocated to MAIN. The earlier [allocation packet](Armageddon_EmotionalRuntime_Hook_Contract.md) remains historical planning evidence. The actual Fury/Calm effects, paid casting bridge, lifetime/save/load and installer attribute inference remain stock/installer-owned. Fury does not refill stamina. The Armageddon package stays disabled and the whole 82-spell/12-support/154-candidate/25-native-scenario plan remains incomplete.

## Admitted hostile attack notification

`IAdmittedHostileAttackEffect.OnAdmittedHostileAttack(AdmittedHostileAttack)` is optional. Its payload contains the exact physical attacker and character recipient plus an execution-local operation Guid. Recipient and body effects are snapshotted and deduplicated by reference. Only effects still owned by that exact recipient or body and still applicable are called. Applicability and notification callbacks are followed by authority, body/focus/instance, embodiment, state, combat/target generation, cell/layer and exact caller-admission checks. A callback which moves or replaces the participants refuses remaining stale shot/attack work; its independent writes remain intact.

The operation identity is created at actual admission, never on a queued action or wrapper. A move and its accepted firearm component share an executing attempt; a component called directly creates its own. Repeated component deliveries to one physical recipient in that attempt deduplicate. Actual multi-target child moves each establish their own attempt and Guid. Independent defender countershots use their independent execution context. The scoped context is restored on disposal; there is no persistent identity cache or saved field.

| Route | Admission boundary |
| --- | --- |
| `MeleeWeaponAttack`, `NaturalAttackMove` | After the first authority gate and pre-admission callbacks, before the attack check. A subsequent miss qualifies. |
| `NaturalRangedAttackMoveBase` | After character/range admission, before the attack check. |
| `MagicPowerAttackMove` | After `CanInvokePower` and authority admission, before committed power use. |
| `RangedWeaponAttackBase` | Carries the attempt to the actual component; it does not notify merely because a target was enumerated. |
| `FirearmBaseGameItemComponent.Fire` | Exact accepted chambered round, casing/cycle preparation and authority are validated before notification and again before commitment. Empty/refused triggers emit nothing. An admission callback may have been delivered even if its independent chamber replacement then refuses the shot. Neither captured nor replacement round is consumed by stale shot work. |

No production effect opts into this interface in this checkpoint. The bounded native probe removes only itself via normal effect ownership APIs and checks an unrelated sibling survives.

## Prepared selective cessation

`ICombat` and ordinary `LeaveCombat` callers retain their existing API. `SimpleMeleeCombat` and `ProgCombat` advertise optional `ICombatSelectiveCessation`:

```csharp
ICombatCessationAdmission? PrepareCessation(IPerceiver subject, IPerceiver? opponent, ICombat expectedCombat);
CombatCessationChanges CeaseCombatFor(ICombatCessationAdmission admission);
```

Preparation requires the exact current combat, physical native Character subject, its exact current target (including null), native opponent if present, exact membership and native incoming targeters. It rejects unsupported participants before the future casting bridge can commit payment. The opaque ticket belongs to its issuing combat. `IsCurrent` performs pure native identity/version/membership checks; it does not invoke policy, effect, output or lazy identity-save callbacks. Same-reference combat/target/Aim and melee assignments advance the private combat mutation generation, so ABA cannot restore an old admission. An incoming targeter added after preparation also invalidates it. Tickets are nonpersistent and single-use; foreign/stale/consumed tickets return `None`.

Application removes only the captured subject from the original combat. Old selected/end/target-change effects, old combat schedule and raw combat/target/melee/Aim fields are captured and detached before gameplay observers. The old Aim invalidation subscription is removed before callbacks; its captured release runs in `finally`, never against a replacement Aim. Each remaining effect removal, leave notification, native event, Prog callback and delay step checks the expected mutation generation. New replacement combat, target, melee, Aim, selected action and schedule survive.

Each unchanged captured incoming targeter loses only its captured subject target, then runs native `AcquireTarget`. A chosen third party or callback-created replacement wins immediately. An unchanged targetless incoming participant leaves only when nobody still targets it. Uncaptured participants are never recursively ended; terminal end callbacks run only when the original combat is actually empty. This is incoming-hostility cleanup, not a blanket truce or a requirement that the original opponent was mutually hostile.

Flags describe observed writes: `SubjectRemoved` means the original captured subject was removed; `OpponentPairCleared` additionally means the captured opponent's incoming subject target was cleared. They do not certify that both actors remain out of all combat after callbacks. Observer exceptions propagate; the ticket stays consumed and committed detach/schedule removal stays committed. Captured Aim release is guaranteed across effect-removal failure, but universal continuation of later notifications after arbitrary exceptions is not promised.

## Qualification and remaining acceptance

See [the source-bound verification receipt](Armageddon_EmotionalCombatHooks_Verification.json) and [whole-plan progress](Armageddon_Completion_Progress.md). Focused managed checks cover identity/dedup, ownership migration, authority, body focus, target/combat ABA, exact issuer, inapplicable end effects, old Aim invalidation/release, callback replacements, third-party acquisition, actual Prog callback and exception controls. Native acceptance is recorded separately, with immutable source/binary capsules and owned disposable MySQL cleanup receipts.

- [x] Preserve existing APIs and unrelated local work; isolate the published baseline.
- [x] Implement optional admission notification and issuer-bound pure cessation ticket.
- [x] Verify managed hook and dependent queued/combat/magic/natural-ranged contracts.
- [x] Verify bounded native miss/refusal, multi-target, firearm, countershot and selective cessation controls; see the final packet receipt.
- [ ] Connect the actual owned Fury/Calm effects and paid casting bridge, refusing unsupported cessation before payment.
- [ ] Verify actual paid Calm effect removal on admitted incoming misses, including ownership/lifetime/save/reload.
- [ ] Verify Fury attribute/endurance behavior using approved installer mapping, without stamina refill.
- [ ] Qualify other attack/component families before claiming universal hostile-attack coverage.
- [ ] Complete remaining stock/source/support/installer and whole-plan native acceptance.

The native fixture is a bounded catalogue host with actual native actors, moves, component operations, effect handlers and schedules. It is not a full server/Telnet boot. Natural/magic hooks have managed coverage; this native packet focuses on melee, the declared internal-magazine firearm, countershot and Simple/Prog cessation. Specialized overrides bypassing these base paths, bows/crossbows, Musket/artillery, auxiliary/contact maneuvers, direct spell/DoT routes and arbitrary custom combats/perceivers remain unqualified. Full Fast, full world/Telnet, stock paid emotional activation, stock reload and an exhaustive attack matrix are NOT_RUN for this checkpoint. No version choice, publication, merge, deployment or shared/production database work is included.
