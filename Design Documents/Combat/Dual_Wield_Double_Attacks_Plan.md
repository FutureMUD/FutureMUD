# Dual Wield double attacks: design and implementation plan

Status: approved design implemented on the dedicated branch, 16 October 2026; verification limitations remain. Luke subsequently requested “Start this one”, then PR publication and sequenced merge. The approved mechanics remain normative. See the [runtime guide](./Dual_Wield_Double_Attacks.md) and [verification receipt](./Dual_Wield_Double_Attacks_Verification.md). Merge waits for Haggle and Analyse receipts; deployment is not authorised.

## Revision and scope

| Item | Value |
| --- | --- |
| Host | Percival (`PERCIVAL`) |
| Source repository | `C:\Users\Luke\source\repos\FutureMUD` |
| Planning branch | `codex/dual-wield-double-attacks` |
| Fetched base | `origin/master`, `890abfd8c443abec70b10e2bc895535928257f5d` |
| Worktree | `C:\Users\luker\Documents\Codex\2026-10-16\task-3\FutureMUD-dual-wield` |
| Plan | `Design Documents/Combat/Dual_Wield_Double_Attacks_Plan.md` |

The source repository was clean on local `master` (`2c89414cf119e538a5a3613f4ded72db25a25292`), two commits behind the fetched remote. The new worktree starts at the fetched commit. Its branch is separate from `codex/armageddon-continuation-20261008` and `codex/blind-fighting`; neither existing worktree is an implementation source or edit target.

The original planning task authorised the worktree, branch and documents only. Luke's later “Start this one” expanded the task to implementation and proportionate verification, followed by publication and sequenced merge authorisation. Dual Wield merges third, after the parent supplies Haggle and Analyse merge receipts. Deployment remains outside scope. Native restore and representative telnet verification passed in an owned disposable world; the verification receipt records the accepted approval retry, evidence and fixture limitations. There is no dependency on unpublished Armageddon changes. The wider nonmagic mapping exercise remains separate.

## Approved behaviour

- A dedicated double-attack move competes in ordinary automatic weighted attack selection while the actor dual wields. It does not convert every ordinary attack into two strikes.
- The two weapons attack the same captured target. Each strike uses its own weapon-family skill and obtains a fresh, independently resolved defence.
- One Dual Wield coordination roll modifies the attack difficulty of both strikes. Failure does not gate the second strike.
- Charge attacker stamina for strikes actually executed. Resolve one attacker recovery action, initially using 1.5 times the slower attack delay, with a balancing configuration.
- Stop the second strike if the first strike or its counter incapacitates either participant, or if the second weapon becomes unusable.
- Preserve the semantics of existing ordinary dual-only attacks. Current handedness classification can label **both** weapons as dual wielded. There is no privileged main/off hand. `Dagger Dual Feint` is an existing single-hit profile.

## Approved coordination, profile scope and untrained defaults

On 16 October 2026 Luke replied **“Agree”** to the question specifying a Normal coordination check, exceptional success making both attacks one stage easier, ordinary success leaving them unchanged, failure making them one stage harder (two on major failure), ordinary single-target profiles excluding special manoeuvres, and untrained attempts at the major-failure penalty without automatic skill acquisition. These three defaults and the corresponding stage table are approved:

1. **Coordination policy:** one Normal check modifies both attacks according to the table below. Failure never gates the second strike.
2. **First-release profile scope:** ordinary single-target damage strikes only, excluding specialised moves. The initial eligibility rules below also keep existing `DualWieldOnly` profiles such as `Dagger Dual Feint` outside the pair, preserving their existing single-hit semantics.
3. **Untrained use:** allow the attempt; missing required Dual Wield skill resolves as Major Fail, applying +2 stages to both strikes. Do not grant or branch the missing skill. A trained-only eligibility gate is not the chosen policy.

| Coordination outcome | Signed difficulty stages on each attack |
| --- | --- |
| Major Pass | `-1` (one stage easier) |
| Pass | `0` |
| Minor Pass | `0` |
| Minor Fail | `+1` (one stage harder) |
| Fail | `+1` (one stage harder) |
| Major Fail, including missing Dual Wield | `+2` (two stages harder) |

Recommended engineering/balancing defaults for implementation: preselect both child profiles before the outer weighted draw; require their summed stamina at admission; choose the lead with an unbiased draw frozen with the pair; give the pair 0.25 times the eligible ordinary attacks' total selection weight; permit at most one Dual Wield improvement opportunity per pair; and use the first child's ordinary recovery if only it executes. These recommendations are available starting points, not additional mechanics approved by Luke's “Agree”. They do not require another product-design decision before implementation. The approved 1.5 delay multiplier and coordination table are distinct from these optional tuning/engineering choices.

Here, “no automatic skill branching” means the pair does not acquire **missing Dual Wield** through its coordination check. Preserve existing world-owned unlock trees when an already-known skill improves. `BranchingImprover` may evaluate those trees independently of a check's missing-trait branching flag. A prohibition on all downstream unlocks would require a separate learning-policy change and is not recommended for this feature.

## Verified integration map

Paths and line anchors below refer to the recorded base; relocate symbols before implementation or rebase.

| Existing surface | Verified contract and planned integration |
| --- | --- |
| [IMeleeWeapon.cs](../../FutureMUDLibrary/GameItems/Interfaces/IMeleeWeapon.cs), `HandednessForWeapon`, lines 11-28 | A one-hand weapon with another non-shield melee weapon returns `DualWieldOnly`; both may qualify. Use actual wielding and item identity, never a main/off-hand assumption. |
| [WeaponAttack.cs](../../MudSharpCore/Combat/WeaponAttack.cs), `HandednessMatches` / `UsableAttack`, lines 103-142 | One-hand profiles are usable while dual wielding. Preserve position, intention, target-shape and usability-prog gates for each child. |
| [StandardMeleeStrategy.cs](../../MudSharpCore/Combat/Strategies/StandardMeleeStrategy.cs), `PossibleAttacksForWeapon`, line 839; `AttemptUseWeapon`, line 878 | Add pair candidates to the existing weapon channel before its final weighted selection at line 965. Preserve ordinary attack weights, manual-command AI multipliers, preferred intentions, bodypart preference and exhaustion fallbacks. |
| Same strategy, `HandleWeaponAttackRolled`, line 1051; `HasViableMeleeAttack`, line 1078 | Inherited melee strategies and magic fallbacks reach this selection. Clinch and fixed/manual actions use separate paths; do not silently change those paths. |
| [CombatMoveFactory.cs](../../MudSharpCore/Combat/CombatMoveFactory.cs), `CreateWeaponAttack`, lines 9-44 | The current factory wraps multi-target profiles. Expose a narrow single-target child factory or directly construct the eligible `MeleeWeaponAttack` children. Never nest the multi-target wrapper. |
| [MeleeWeaponAttack.cs](../../MudSharpCore/Combat/Moves/MeleeWeaponAttack.cs), lines 49-76, 79-116 | Owns strike stamina, delay and effective difficulty; its roll uses `Weapon.WeaponType.AttackTrait`. Add an immutable per-instance coordination stage adjustment, leaving the shared profile untouched. |
| [WeaponAttackMove.cs](../../MudSharpCore/Combat/Moves/WeaponAttackMove.cs), `ProcessWardFreeAttack`, line 664 | A child can include an independent ward counter. Complete that exchange before the next-strike guard; do not apply the attacker's coordination modifier to its opponent's counter. |
| [CombatBase.cs](../../MudSharpCore/Combat/CombatBase.cs), `CombatAction`, lines 381-477 | Owns response admission, attacker/defender stamina, on-use program, scoring, blood spray, tactic notification, status and recovery scheduling. Add explicit sequence handling and share per-child completion with ordinary actions. |
| Same file, `ScoringResolutionsFor`, line 362; `FireOnUseProg`, line 370; `GetCombatDelay`, line 329 | Retain actual child move/response/result for scoring. Invoke each executed child's on-use and blood handling once. Call recovery once for the aggregate. There is no `ResolveCombatAction` method at this base. |
| [MultiTargetCombatMove.cs](../../MudSharpCore/Combat/Moves/MultiTargetCombatMove.cs), `ResolveMove`, line 51; `ResolveOwned`, line 71 | Useful resolution-ledger and ownership pattern. Its membership-only loop, different targets, primary-only attacker cost/on-use and deferred completion are unsuitable for a same-target pair. Do not subclass or reuse its loop unchanged. |
| [CommandExecutionScope.cs](../../MudSharpCore/NPC/AI/CommandExecutionScope.cs), lines 135-175 | Preserve owned execution, committed-work accounting and authority checks through callbacks. `ResolveOwned(parent, child, responseFactory)` supplies the inheritance pattern. Command authority is additional to physical validity. |
| [ICombatMove.cs](../../FutureMUDLibrary/Combat/ICombatMove.cs), lines 11-51 | Existing results have no executed-child/accounting contract. Design the shared sequence capability and immutable child-resolution record here before concrete runtime orchestration. |
| [CombatSeeder.MeleeWeapons.cs](../../DatabaseSeeder/Seeders/CombatSeeder/CombatSeeder.MeleeWeapons.cs), lines 558-566; [CombatSeeder.cs](../../DatabaseSeeder/Seeders/CombatSeeder/CombatSeeder.cs), lines 793-801 | Both stock and additive compatibility definitions contain the single-hit `Dagger Dual Feint`. Preserve their move type, handedness, weighting, damage, narrative and one-strike behaviour. |

## Candidate construction and weighted selection

Implement a dedicated `DualWieldDoubleAttackMove` containing a frozen plan for two distinct weapon item IDs, two attack profile IDs, the captured physical target/combat and execution order. It is a new runtime action composed of ordinary strikes, not a reinterpretation of a persisted `WeaponAttack` row. The first version needs no persisted double-attack profile, new attack-family enum value, or automatic manual verb. Both children retain `UseWeaponAttack` for defence, messaging and weapon skill purposes.

Initial eligibility implementing the approved ordinary single-target scope:

- The actor and target are characters in the same combat, colocated, in valid ordinary melee contact, able to participate, and permitted by current strategy policy. Use current `WillAttack`/settings, vehicle-boundary, position and authority checks.
- Two distinct, usable, wielded `IMeleeWeapon` items, each held in one hand; neither is a shield, implant or prosthetic. Check actual items and hand counts, including races with more than two arms. Having a preferred dual-wield loadout is neither sufficient nor required if the actual equipment qualifies.
- Both profiles are usable `UseWeaponAttack`, `MaximumTargets == 1`, and `HandednessOptions` is `Any` or `OneHandedOnly`. Exclude `DualWieldOnly`, `SwordAndBoardOnly`, two-hand profiles, clinch, backstab, coup de grace, downed/control/mounted/charge attacks, magic/psychic/natural attacks, and intrinsic area attacks for this first version. A future opt-in extension can broaden this without changing existing profiles silently.
- Apply allowed weapon classifications, required/forbidden intentions, positive finite effective weight, required positions, target bodypart/shape compatibility, usability progs and manual-command AI exclusions independently to each child. No speculative check or preview can improve a skill or consume resources.

Keep the normal weapon-channel percentage and existing ordinary single candidates. Do not add an unrelated top-level strategy percentage or a post-selection “extra hit” roll. Extend the candidate representation in `AttemptUseWeapon` to hold either an ordinary attack or a complete pair.

Recommended weight algorithm:

1. Build ordinary candidates with their existing effective weights. Respect their existing preferred-intention/bodypart preference decisions; a pair must satisfy the resulting preference requirements with both children. Preserve all existing ordinary filtering behaviour.
2. Form eligible unordered combinations of distinct weapons and their usable profiles. Require `CanSpendStamina(costA + costB)`, using both ordinary power-move multipliers. Exclude unaffordable combinations before sampling.
3. Weight each feasible profile combination by `weightA * weightB`. Sample one complete pair from those combinations without performing any combat/check side effects. Sort item/profile IDs for stable enumeration; an injected/seeded random source makes sampling reproducible. Do not count both `(A,B)` and `(B,A)` as different candidate mass.
4. Freeze its lead with one unbiased 50/50 draw. This draw chooses order, never success or availability of the second hit. No static “lowest ID always attacks first” or main/off-hand skill advantage. Replay/tests retain the selected order; there is no execution-time reorder after seeing the first result.
5. Add **one aggregate pair candidate** with weight `0.25 * sum(weight of ordinary candidates participating in at least one feasible pair)`. Draw alongside the unchanged ordinary candidates. With only eligible singles, this recommends a 20% pair share; excluded singles reduce that share naturally. Normalize pair sampling separately so adding many profiles or more arms does not multiply the pair's aggregate chance.

Freeze both profiles before the outer draw; do not select the second after seeing a hit or a coordination result. Do not retain mutable collection enumerators across authored progs. A straightforward bounded combination scan is sufficient initially; avoid materialising an unbounded cartesian list. Sample with accumulated weights, reject non-finite products, and cover a representative multi-arm case. No global RNG redesign is needed.

An unaffordable pair never removes affordable single attacks. Preserve `TooExhaustedMove` only where the existing candidate/exhaustion contract warrants it. Disabling the feature, invalid configuration, or failure to find a partner must preserve the ordinary candidate path, its random consumption where practical, and its fallbacks.

## Coordination check and configuration

Append `CheckType.DualWieldCoordinationCheck` without renumbering existing published values. Publication integration assigns **230**, preserving Haggle 227 and Analyse 229 in the authorised merge sequence. The earlier isolated native fixture used unpublished 227; its receipt remains historical. Blind Fighting #792 is unchanged and requires its own later integration review. Include coordination in `IsPhysicalActivityCheck`, `IsOffensiveCombatAction` and `IsTargettedHostileCheck`, passing the captured opponent for normal contextual bonuses and merits. Exclude it from `IsVisionInfluencedCheck`. Current sight belongs to the two ordinary attack checks and their defences. Do not consume/copy `OffensiveAdvantage` for coordination; that remains the ordinary strikes' resource.

Use the existing `Checks -> TraitExpression -> CheckTemplate` schema. Runtime obtains the dedicated check, not a trait by display name. Example world binding: `dualwield:123{nobranch}`, where 123 is the selected world's skill ID. The formula is builder configurable and can include attributes or other modifiers. Use exactly one improvement-enabled Dual Wield parameter; supplementary trait parameters should be `{noimprove,nobranch}`. Do not supply the same coordination trait again through the `variable` argument. A valid binding resolves a character-skill ID through the dedicated check, has no duplicate improvement-enabled trait aliases, and uses the reviewed missing-trait/branching policy. A missing, generic-fallback or parameterless placeholder check is unbound and offers no pair candidates even if its enable setting is true. Validate this without runtime name lookup and diagnose it once per configuration change. Preserve unsupported custom formulas for builder review rather than rewriting them.

Create a dedicated Standard check template with improvement enabled, required missing-trait failure, and missing-trait branching disabled. The general `Skill Check Fail If Missing` template is not sufficient unchanged: its branching policy follows the seeder's global answer. An untrained actor must receive Major Fail, rather than gaining free coordination from a generic fallback formula. Do not call the check in candidate generation or strategy previews. Roll once after the selected pair passes live admission, immediately before the first attack attempt. If a later response callback cancels the attempt, do not reroll coordination; normal check learning already performed remains one opportunity, while no unexecuted weapon strike is charged.

Apply the resulting **signed stages** to each child's effective attack difficulty after the ordinary tool/profile difficulty, clamping with existing difficulty helpers to Automatic-Impossible. Cache that adjusted difficulty per child so every `attackRoll[CheckDifficulty]` access uses the same key. A coordination failure never suppresses child two. Damage expressions, weapon-family traits, penetration, defence difficulty and recovery difficulty are not directly altered by this coordination modifier. Ordinary offensive advantage remains consumed by the first ordinary strike under current semantics; do not clone it onto both strikes.

Planned static configuration, stored through existing facilities. Names and configuration plumbing are engineering recommendations; the coordination difficulty, six outcome values and 1.5 recovery multiplier are approved initial values:

| Key | Initial value/policy and approval status |
| --- | --- |
| `DualWieldDoubleAttacksEnabled` | Recommended rollout: runtime/upgrade default `false`; explicit fresh-install enablement only after valid binding. A false/invalid value bypasses the entire candidate path. |
| `DualWieldDoubleAttackWeightMultiplier` | Recommended `0.25`; finite, nonnegative; zero disables pair selection. Optional balancing. |
| `DualWieldDoubleAttackRecoveryMultiplier` | Approved initial `1.5`; finite, positive and configurable. |
| `DualWieldCoordinationDifficulty` | Approved initial `5` / Normal; valid `Difficulty` integer 0-10. |
| `DualWieldCoordinationMajorPassStages` | Approved initial `-1` |
| `DualWieldCoordinationPassStages` | Approved initial `0` |
| `DualWieldCoordinationMinorPassStages` | Approved initial `0` |
| `DualWieldCoordinationMinorFailStages` | Approved initial `1` |
| `DualWieldCoordinationFailStages` | Approved initial `1` |
| `DualWieldCoordinationMajorFailStages` | Approved initial `2`; also used for missing Dual Wield. |

Outcome stage values must be integers in -10..10. Validate settings before use and provide an actionable configuration diagnostic. Invalid enable/weight/binding disables only paired candidates; invalid tuning uses the documented safe default with a diagnostic. `None`/`NotTested` is not a coordination success: abort an uncommitted pair without costs rather than inventing a second check. Use existing static-setting cache invalidation for edits; do not add a second stale cache.

Add defaults in [DefaultStaticSettings.cs](../../FutureMUDLibrary/Framework/DefaultStaticSettings.cs). The existing `show config`, `editstaticconfig`, `show checks` and `traitexpression` builder facilities can expose the settings/formula; implementation documentation must include a worked native-skill binding. No EF tables, columns, model snapshot, migration or blank-snapshot refresh are required for this recommended first version. Persisted additions are check/template/expression/configuration rows and any missing stock coordination trait. Adding profile-level opt-ins later would be a separate schema/authoring decision.

## Sequential execution and accounting

Add a small shared sequence contract in `FutureMUDLibrary/Combat` before its runtime implementation: immutable planned children, a separate preflight stamina budget, and an ordered resolution ledger containing actual child, selected response/accounting subject, effective response or fallback chain, result, committed/executed state and applied accounting. `CombatMoveResult.Irrelevant` alone cannot express whether a callback already committed work. Capture commitment for each child before its scope is disposed; the parent's previously committed first strike must not make a skipped second strike count as executed. Avoid changing one mutable `StaminaCost` property from “planned sum” into “actual total”.

`CombatBase.CombatAction` must explicitly recognise a sequence before asking for a root defence. Keep the scheduler and whole-action lifecycle there. Route sequence children through a narrow ordinary-resolution/completion helper owned by the combat host; the dedicated move supplies frozen children and continuation guards. The shared contract must make child completion synchronous before requesting the next child.

Provide one host **resolution-only** service, exposed through a shared combat capability (proposed `ICombatSequenceResolver`). A direct sequence `ResolveMove` delegates to that service, and `CombatAction` invokes the same service without calling back into the sequence's `ResolveMove`. It enters/preserves the parent execution scope, resolves children and their accounting, and returns the aggregate/ledger. It never ends whole-action effects, notifies tactics, handles combat ending or schedules recovery; those belong exclusively to `CombatAction`. Direct resolution fails uncommitted if its captured combat host is unavailable. This defines the direct `ICombatMove` path without recursion, a second implementation or double scheduling.

Extract only the needed ordinary pipeline from the current code. Preserve normal actions and existing multi-target accounting; do not migrate every composite or redesign combat. The parent is not an `IWeaponAttackMove` masquerading as its first weapon. In particular, it receives no separate weapon on-use invocation, defence stamina, blood spray, scoring entry or stamina charge.

Sequence:

1. The outer host performs normal participation/authority checks and whole-action effect ending once. Capture the exact combat, actor, target and physical weapon identities; `CharacterTargets` must expose only the captured victim. Do not let the base move's implicit current target create a second target accidentally.
2. Revalidate both selected weapons/profiles and enough stamina for the complete pair. If preflight fails, execute no strike and make no coordination roll; ordinary idle/unexecuted handling applies. Do not silently replace the selected pair with a different attack.
3. Roll coordination once and construct the two ordinary single-target children with that immutable adjustment. For each child, obtain `target.ResponseToMove(child, actor)` afresh. After response callbacks, recheck physical validity and authority immediately before resolution. Use `CommandExecutionScope.ResolveOwned(parent, child, responseFactory)` or an equivalent preserving its ownership/checkpoint semantics.
4. Resolve the child through its existing weapon path, including hostile admission, unseen-opening consumption, wards/counters, magical interception, damage, equipment condition, exertion, advantage consumption and passive wound processing. Never call a defence's `ResolveMove` separately; ordinary attack resolution owns the contest.
5. Complete the child before continuing: normalize an irrelevant/unused response only when it has no committed defence work. A late `Irrelevant` result cannot erase an already executed attack or committed defence/resource consumption. Charge the child's ordinary attacker and defence costs exactly once, respecting `RaceUsesStamina` and `UsesStaminaWithResult`. Preserve the ordinary response-accounting subject even when a ward selects an internal fallback or `MagicDefenseMove` delegates accounting to its fallback; retain the response chain needed to explain it. Do not charge both layers contrary to ordinary semantics or lose a committed magical reaction. Preserve internal block-failure stamina and parry/dodge delays. Account for committed work even if an actor leaves combat during its callback.
6. Under the existing continuation and participation gates, fire that child's `OnUseAttackProg`, score its actual move/response/result, and resolve its weapon blood/poison completion once. Preserve callback order. Do not replay unsafe progs or mutations after command authority/physical participation is lost. The ledger records completed costs independently of whether later callbacks were allowed.
7. Before child two, run the complete live continuation guard below. Its defence is selected only after child one's damage, resource spending and completion callbacks. If invalid, record child two as skipped, with no attack/defence roll, stamina, on-use, blood handling or score.
8. Aggregate executed results/wounds for the parent; success means any executed child succeeded. Preserve individual attacker/defender outcomes and self-wounds in the ledger. Notify `ICombatTacticEffect.MoveResolved` once for the action. Audit tactic consumers so a new composite does not become an invisible action to them. Perform normal combat status/end handling and schedule at most one attacker recovery if the participant/combat still exists.

Choose one scoring owner: the proposed completion helper scores each child, so the outer host must skip parent scoring. Alternatively generalise `ScoringResolutionsFor` and defer scoring, but do not do both; deferred scoring must still precede the next child's callback-sensitive guard if scoring can mutate state. The recommended helper keeps all per-child completion together.

Do not recursively call `CombatAction` for the two strikes: that would end action effects, notify tactics, process combat status and schedule attacker recovery twice. Do not reuse `MultiTargetCombatMove`'s primary-only on-use/accounting policy. Regress its existing once-per-arc attacker cost and on-use behaviour when extracting common code.

### Live continuation guard

Immediately before each strike, after response selection, and after child-one completion:

- Both exact physical actors still participate in the original combat; neither is dead, unconscious, paralysed or otherwise unable to act under current state/health rules. Use `CharacterState.IsAble()`/explicit incapacitation, not only `CheckCombatStatus()`, which can retain helpless victims.
- The captured target still satisfies current attack policy, co-location, layer/contact/vehicle boundary, legal engagement and target eligibility. A combat end, transfer, retarget, movement, grapple/clinch transition or callback invalidation stops the remaining strike. Do not acquire another victim or follow a corpse/replacement body.
- The captured child weapon still exists, is functional, is wielded with the required hand count, and its profile is still usable with the current posture/bodyparts/prog result. If the pair ceases to be a valid two-weapon setup, stop. Never substitute a newly wielded weapon or use an invalid arm.
- The actor can still pay this child's ordinary cost after first-child spending and any counter/callback drains; command authority/continuation remains valid. Autonomous moves also need these physical guards because an unbound `TryContinue()` may return true.

Policy/usability guards themselves can invoke authored callbacks. After **all** response and guard callbacks, immediately before child resolution, perform a final callback-free check of the captured combat/body/weapon identities, membership, ability and required equipment state. Do not introduce another authored callback between that check and admission. `ResolveOwned`'s authority checkpoints alone do not supply this physical protection.

These guards stop child two after incapacitation even if ordinary strategy settings would allow attacking a helpless target. Missing defence resources alone do not cancel the attack: obtain the target's normal exhausted/helpless or alternate defence. Each strike may receive a different defence, expend a separate magical reaction/charge, or trigger an ordinary counter. A miss/warded/blocked attempt still counts as executed when the ordinary strike semantics charge it.

## Recovery and partial execution

For two executed strikes, use `pairBaseDelay = recoveryMultiplier * max(childA.BaseDelay, childB.BaseDelay)`. Recommended aggregate recovery difficulty is the harder **actual completed result** difficulty. Call `GetCombatDelay` once, so the normal recovery check, recovery trait expression, combat speed and scheduling modifiers apply once. Do not roll recovery for each child and then take their maximum; that changes the approved timing model.

Recommended partial policy: one executed strike uses that strike's ordinary base delay and result recovery difficulty; zero executed strikes has no weapon cost or attack recovery reward and follows the host's unexecuted/idle handling. Capture delay/cost information safely while the weapon is valid rather than dereferencing a destroyed component later. Never create a recovery schedule for an actor who has left the combat or died. Existing defender-side delays/counter effects remain ordinary per-strike consequences, separate from the single attacker recovery.

## Fresh worlds, existing worlds and Armageddon

The coordination check and global configuration fit current persistence; the rollout must distinguish installation from upgrade:

- [FuturemudLoaders.cs](../../MudSharpCore/Framework/FuturemudLoaders.cs), `LoadChecks`, lines 4889-4930, currently creates unknown checks with an ordinary skill template and constant `50`. Add a dedicated safe coordination fallback instead: feature disabled, a nonbranching placeholder, and a diagnostic directing builders to bind the check. An upgrade must not silently make coordination skill independent.
- Update both [SkillPackageSeeder.cs](../../DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs), `SeedChecks`, line 561, and legacy [SkillSeeder.cs](../../DatabaseSeeder/Seeders/SkillSeeder/SkillSeeder.cs), `SeedChecks`, line 137, so generic switch defaults do not claim this check. Ensure their placeholder/deferred ownership is explicit.
- CombatSeeder should ensure/bind one coordination skill across **all** supported skill packages using an additive ownership-aware helper. `Dual-Wield` exists in the non-gerund legacy list; `Dual-Wielding` is currently created only in the SOI weapon package. Current core checks run before weapon seeding, so simply binding the new check early would miss that later SOI skill. Move skill assurance to the universal path or bind at a deliberate final step.
- For a fresh world, reuse the package's native trait where appropriate or create one compatible coordination skill, cap/decorator/improver and check; enable paired candidates only once that configuration is valid. Recommend existing skill-family cap/improvement conventions, not a new Armageddon-wide cap policy. Stock admission/chargen/NPC access must remain explicit; do not grant existing characters the skill automatically.
- For existing worlds, preserve configured checks, skill caps, values, templates and explicit static settings. Provide a documented, explicit binding/update path that validates numeric trait IDs, does not overwrite custom formulas, and can be rerun safely for its owned additions. Do not declare the entire foundational CombatSeeder safe to rerun or rerun it as an upgrade shortcut. Rollback is disabling the feature; do not delete custom traits or rewrite player values.
- [Armageddon_Sorcerer_Source_Tree.json](../Magic/Armageddon_Sorcerer_Source_Tree.json), lines 1172-1183, records `arm.support.dual_wield` with opening 5/raw cap 20. That is source mapping, not a shipped combat binding. Published [ArmageddonTraditionInstaller.Source.cs](../../DatabaseSeeder/Seeders/ArmageddonMagicSeeder/ArmageddonTraditionInstaller.Source.cs), lines 53-63, does not enrol that support entry. Bind the check to the world's chosen native skill independently; do not implement support-tree acquisition, import its cap, or depend on the continuation branch.

Use the existing expression parameter/template controls to prevent missing Dual Wield acquisition. `CheckLearningScope` protects authority/stale writes; it is not a once-per-pair learning deduper. One coordination `Check` call and one improvement-enabled binding give the intended opportunity limit. Weapon skills continue their ordinary per-strike improvement; a pair of the same family retains its normal existing learning/caching rules.

## Blind Fighting PR #792 compatibility

[PR #792](https://github.com/FutureMUD/FutureMUD/pull/792) was verified **OPEN**, not merged, on 16 October 2026; head `8cd0da25470c5588923b3eefaeb983605ee69c5c`. Its branch was read only. Implement against published master, then refresh PR/master status at implementation time.

The proposed PR replaces selected attack/defence calls with `CombatCheck` and passes `CheckEvaluationContext` constraints. If it lands first, keep both children on that ordinary wrapper path: apply coordination to each child's normal attack difficulty, then let current sight impose its own cap/difficulty floor. Assess the actual opponent/sight afresh per strike and defence; do not cache one parent visibility result. Coordination is not another blind-fighting check. Preserve normal all-difficulty dictionary keys and wrapper learning semantics. Do not copy raw `Gameworld.GetCheck` calls over a merged wrapper.

Likely overlap: `MeleeWeaponAttack`, `WeaponAttackMove`, `StandardMeleeStrategy`, `CheckEnum`, `FuturemudLoaders`, static defaults and skill/combat seeders. Resolve the new enum ordinal against the merged tail, preserve both dedicated loader/seed policies and feature flags, and test their composition. Blind Fighting can remain disabled or absent without changing this feature's approved contract; this planning branch neither merges nor cherry-picks that PR.

## Implementation sequence authorised by the later request

1. Use the approved mechanics, Normal coordination table, ordinary single-target scope and untrained policy recorded above. Refresh master/PR status and rebase the implementation branch deliberately, retaining the planning-base evidence. Use the labelled engineering recommendations unless implementation evidence warrants an adjustment; surface a change to approved behaviour rather than reopening settled choices.
2. Define the shared sequence/child-resolution and separate budget/accounting contract; add focused lifecycle tests before extracting the ordinary completion helper. Keep legacy multi-target behaviour stable.
3. Add the dedicated move and immutable child difficulty adjustment; implement owned sequential resolution, live guards, per-child completion and single recovery. Add interruption, counter and defence-resource tests.
4. Extend only the ordinary automatic weapon selection path with frozen pair candidates and normalized weighting. Add deterministic selection/eligibility tests; preserve fixed/manual/clinch paths and fallbacks.
5. Add appended check/configuration, safe missing-check loading, native trait bindings and minimal additive installer support. Batch configuration/content verification rather than reseeding per tweak. If new workflow questions/replay contracts become necessary, use the repository database-seeder skill and its additional gates.
6. Update runtime and builder/seeder documentation with final controls and limitations. Run the bounded automated and native checks below; review accounting and enum conflicts before publication. Publication is authorised; merge waits for the Haggle and Analyse receipts, and deployment requires a later request.

## Proportionate verification plan

No gameplay, database or automated code tests are claimed for this planning task. Later implementation should use focused tests for new behaviour and a representative native smoke, not an exhaustive cross-product of weapons, strategies, check systems and settings.

### Automated mechanics and regression coverage

Add focused runtime tests (suggested new `DualWieldDoubleAttackTests` / `DualWieldDoubleAttackSelectionTests` in `MudSharpCore Unit Tests`) using deterministic profile/defence results and injectable RNG:

- Frozen pair competes with ordinary attacks; both children use their own weapon trait and fixed target, one coordination roll stages both difficulties, failure still executes the second valid strike, and each defence is selected/resolved independently.
- Table-drive the six coordination outcomes in one test; test clamping once at the boundaries. Prove zero selection/preview learning, at most one coordination improvement opportunity, no missing-skill acquisition, numeric binding surviving trait rename, and ordinary weapon-family learning remaining intact.
- Prove exactly-once attacker/defender accounting, ordinary block-failure costs, separate magical defence capacity/counters, per-child on-use/scoring/blood completion, one tactic notification, and one recovery check/schedule. Include an unused/irrelevant defence and a committed callback interruption; a wound count alone is insufficient evidence.
- Cover representative stops: target incapacitated by strike one; actor incapacitated by its ward counter; second weapon disarmed/destroyed or invalid arm; combat exit/retarget/movement/authority revoked in a callback; insufficient remaining stamina. Batch guards as data-driven cases around one fixture rather than duplicating full worlds. Assert skipped child has no resource/check/effect side effects.
- Check full-pair delay with unequal weapon delays and actual recovery results, first-only ordinary recovery, and no schedule after combat exit. Cover the first strike missing while the second still proceeds.
- Selection checks: actual dual classification on both weapons, no duplicate `(A,B)/(B,A)` mass, unbiased frozen lead via deterministic RNG endpoints, normalized mass with additional profiles/multi-arm weapons, combined affordability, forbidden/preferred intention and targeted-bodypart handling, single-hit Dual Feint unchanged, single-weapon/two-hand/shield/multitarget exclusions, disabled/unbound/invalid settings preserve ordinary behaviour.

Reuse/regress [MultiTargetCombatMoveTests.cs](../../MudSharpCore%20Unit%20Tests/MultiTargetCombatMoveTests.cs), [CombatStrategyRuntimeTests.cs](../../MudSharpCore%20Unit%20Tests/CombatStrategyRuntimeTests.cs), [CombatStrategyLoadoutTests.cs](../../MudSharpCore%20Unit%20Tests/CombatStrategyLoadoutTests.cs), and the ordered-NPC command/learning tests where the completion helper/authority paths change. If PR #792 has merged, add a small composition fixture for enabled impaired sight and disabled legacy behaviour; retain its own existing wrapper tests rather than recreating that entire matrix.

### Batched configuration/content checks

In one configuration suite/run, cover both skill-seeder routes, supported combat package variants, fresh binding, pre-feature upgrade placeholder, an existing native/renamed skill, custom configured check preservation, explicit disable preservation, invalid tuning/binding, and idempotence of the narrow additive helper. Use in-memory/isolated context tests where appropriate; verify persisted rows/links rather than source-text assertions alone. Extend the existing combat seeder/source/compatibility tests where useful. No broad new seed catalogue or EF migration is planned.

After focused iteration, run the affected runtime, shared-library and seeder suites once against stable inputs. The repository's compact runner selects projects explicitly and saves receipts:

```powershell
.\scripts\test-unit.ps1 -OutputMode Compact -Project @(
  'MudSharpCore Unit Tests/MudSharpCore Unit Tests.csproj',
  'FutureMUDLibrary Unit Tests/FutureMUDLibrary Unit Tests.csproj',
  'DatabaseSeeder Unit Tests/DatabaseSeeder Unit Tests.csproj'
)
git diff --check
```

Restore only as required, use native Windows tooling and single-node builds (`-m:1` for direct .NET commands), and avoid the full solution/VSIX by default. Add persistence/expression suites only if their implementations actually change. Read `.codex/references/test-execution.md` for filters and `.artifacts/test-runs` receipts; rerun only affected failures after a repair or when new evidence warrants it.

### Representative native smoke on Percival

Use [.agents/skills/futuremud-mud-tester/SKILL.md](../../.agents/skills/futuremud-mud-tester/SKILL.md) and its **repository-local** [scripts/mud_session.py](../../.agents/skills/futuremud-mud-tester/scripts/mud_session.py). The older global script path printed in that skill is absent on this host; pass the actual checkout path. Database restore, native launch and Telnet are part of later implementation verification, not forbidden procedures. None are run during this planning task.

1. Select and record one known-good disposable pre-feature local world/backup, its provider, fixture account and free port. Stop its owned MUD first; back up any state that matters. Restore the `.sql` backup with local MySQL Workbench or `mysql` into that named disposable database. Use `FUTUREMUD_TEST_CONNECTION_STRING` for secrets, never a password in command lines, transcripts or committed fixtures. Do not touch another task's server/database.
2. Build the tested worktree's runtime once and record SHA/diff/binary/backup identities. Verify the restored world boots with the feature disabled and ordinary single-hit/dual-feint behaviour intact. Bind the coordination check explicitly to the native trait, apply the reviewed settings, then enable it through builder facilities. A fresh installer check, if needed, uses one separate disposable database/install, not repeated foundational reseeding.
3. Use one scripted session/scenario with two mixed weapon families and unequal delays, a known opponent and controlled defence loadout. Demonstrate ordinary and pair choices in the same automatic channel, the two strike messages to one target, distinct family/check evidence, separate defence resource spending, correct total stamina and one recovery interval. Use deterministic test instrumentation where timing/randomness needs an exact assertion; a short random transcript cannot prove the outcome table or selection probabilities.
4. Reuse the fixture for one first-strike incapacitation interruption, asserting no second attack/defence cost/message/on-use. Units cover the broader guard/counter variants. If Blind Fighting has merged, add one impaired-sight pair and one disabled-feature observation in this same batch. Restore/clean fixture changes only when needed, not between every assertion.
5. Run the native helper from the tested worktree, wait for its ready marker, log in and capture redacted Telnet output and relevant structured check/state evidence. An indicative invocation after the exact command/assertion script exists is:

```powershell
python .\.agents\skills\futuremud-mud-tester\scripts\mud_session.py `
  --repo '<tested-worktree>' --account '<fixture-account>' `
  --commands-file '<local-scenario-file>' --transcript '<local-evidence-file>'
```

Set connection/credential values outside tracked files. Inspect the helper's current options before selecting a port or supplying flags; launch/stop only the scenario's owned process. Keep logs outside tracked source under an ignored artifact directory. Record each assertion PASS/FAIL/BLOCKED/INCONCLUSIVE, exact tested inputs, omitted checks and cleanup. A boot/login/build success alone does not prove paired combat correctness. Stop dependent steps on a failed prerequisite; do not repeatedly restore/reseed until a random demonstration passes.

## Planning-task evidence and remaining boundary

Evidence gathered: repository/worktree/branch inspection; `git fetch origin master`; source and repository-skill inspection against the recorded base; read-only PR #792 status and branch-diff inspection; Luke's explicit “Agree” approving the three defaults on 16 October 2026. Planning verification checks document links, references, whitespace and the final diff only. Future implementation/test paths and planned configuration keys above are intentionally labelled as such.

**Blocking design choices: none.** Luke's implementation request has been received. Selection frequency, lead order, improvement-opportunity enforcement, partial recovery and rollout use the concrete defaults above. World-specific numeric binding and the disposable smoke fixture are operational inputs rather than unsettled product mechanics. The verification receipt records completed native verification and remaining test limitations. PR #792 remains an integration check, not a feature dependency.

The later “Start this one” authorised implementation, now present on this branch. The verification receipt records automated results, inherited snapshot failures and representative native smoke/restart results, followed by publication integration and its merge gate. Only the owned disposable native world was mutated. No production database mutation, merge or deployment has occurred.
