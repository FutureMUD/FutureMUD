# Independent emotional-policy checkpoint

This checkpoint implements source calculations, persisted emotional children and an unwired applicable character/body combat query. It does not create stock rows, wire existing combat/casting methods, or claim paid/native combat qualification. The cleared Pierce implementation and factory remain untouched. See [the preceding source boundary note](Armageddon_Fury_Calm_Boundary_Note.md) for the canonical Unravel Enchantment raw80 → Roused Fury raw80 → Still Anger raw80 → Mend Flesh acquisition path.

## Implemented independently

`Magic/Emotions/EmotionalSpellPolicy.cs` resolves reciprocal source-grade subtraction, explicit rational terrain duration/endurance mappings, the historical inclusive random helper, normalized whole-unit accumulation, and the distinct recast-strength rules. Fury copies old source/native fields and cap36; Calm retains maximum source grade and cap24. Source grade does not depend on intensity or the power enum. Invalid grades, unbounded durations, mismatched remaining/state pairs and conflicting equal-grade Calm mappings refuse rather than choosing by enumeration order.

`SpellEmotionalEffect.cs` provides persisted `SpellSourceFuryEffect` and `SpellSourceCalmEffect` children using existing `MagicSpellEffectBase`/parent persistence. Counter weakening changes only source grade; Fury's endurance/intensity/native power remain intact. Invalid persisted definitions remain readable and preserved for repair. Existing detection lifetime descriptor validation is reused for group/unit/cap representability; the detection-specific resolver and stock are unchanged. Native parent scheduling and its saved remaining duration remain the intended expiry authority; no independent timer or second deadline is added here.

`Combat/EmotionalCombatPolicy.cs` defines `IsPeaceful(ICharacter, bool superPeaceful=false)` and `IsRaging(ICharacter, bool superRaging=false)`. Both query applicable character/body effects, deduplicate references and retain each effect's existing threshold properties. No guard currently calls this new helper. Applicability exceptions propagate to the caller; they are not converted into permission.

`Body.EmotionalStamina.cs` adds one narrow concrete-body capacity recomputation called by the Fury child's attach/login/removal lifecycle. It evaluates the existing `MaximumStaminaFor` prog, validates a finite nonnegative capacity, updates the body's native maximum and clamps current stamina. It does not refill stamina, reset exertion, restart heartbeat timers or replace the world's capacity formula. Actual native capacity behavior still needs qualification against the selected world's prog.

## One proposed endurance mapping; no stock default yet

Select the world's actual endurance attribute explicitly and supply a positive editable **native attribute units per source endurance point** multiplier. The proposed initial multiplier is1. The child uses existing `ITraitBonusEffect`; `GetTrait` uses effective trait values, and the standard seeder's maximum-stamina prog is `100+GetTrait(endurance)*10`, so that particular prog converts one source point at multiplier1 into ten capacity units. A different world's prog can produce a different conversion or ignore the attribute entirely and must be checked. No attribute is selected by name or ID automatically.

Tradeoffs: this is a declared native mapping, not literal historical attribute-scale parity; all legitimate consumers of the selected effective attribute receive the bonus. Added capacity is not immediately spendable because current stamina is not filled. Removal clamps excess current stamina. The stock multiplier/default has not been authored pending the coordinator's mapping decision; tests use an explicit multiplier0.5 to prove units and persisted bonus independently of native power.

The historical `number(int from,int to)` helper at `codedump.c:236272-236278` returns `from` immediately when `to<from`, otherwise samples inclusive endpoints. Consequently `number(1, floor(1²/2))` yields exactly1 with no random draw. Grade2 gives2, grade3 gives3..4, grade7 gives7..24; an Earth-plane bonus adds the residual grade. Bounds must never be silently swapped.

## Exact minimal coordination proposal

The main owner keeps existing live files. The query replacement points are:

| File/method | Proposed call |
|---|---|
| `CharacterCombat.CanEngage`, `WhyCannotEngage` | `EmotionalCombatPolicy.IsPeaceful(this)` |
| `CharacterCombat.WillAttackTarget` | `EmotionalCombatPolicy.IsPeaceful(this, true)` |
| `CharacterCombat.CanTruce`, `WhyCannotTruce` | `EmotionalCombatPolicy.IsRaging(this)` |
| `StrategyBase.WillAttackTarget`, `WhyWontAttack` | `EmotionalCombatPolicy.IsPeaceful(ch, true)` |

The final execution guard remains in the main owner's existing `CombatBase.CombatAction(IPerceiver,ICombatMove)` authority check immediately before `ResolveMove`. New stock must not install a second command-authority framework or infer harmfulness from a command-name list. The existing main authority should use the same applicable query for whatever harmful move/spell classification it owns.

For cessation, use **existing** `ICombat.LeaveCombat(selectedActor)` / `SimpleMeleeCombat.LeaveCombat`: it destroys the selected actor's combat schedule, clears its combat/target/aim state and handles participants targeting that actor. Do not call `EndCombat` or request a voluntary truce. Native tests must prove that incoming opponents lose that exact target, unrelated fights remain, and queued moves cannot execute against a detached actor. A new global combat API is not proposed.

The source-ordered spell hook still needs allocation: after ward admission but **before target save**, subtract the opposing source grade and perform the source early return. For surviving positive Calm, invoke the existing native save with an explicitly authored residual-grade difficulty mapping; then run selective cessation even if no child is applied. Both `MagicSpell.CastSpellCore`'s `TargetWasRejected`/local application and `ResolvePreparedSpell` currently return on resistance before a template runs. Therefore a normal pacifism template alone cannot preserve this ordering. A narrow new `MagicSpell.Emotional.cs` resolver and effect template can own the cohort, native parent, reporting and snapshot guard, while the exact existing-method call sites are agreed before edits. Prepayment selection/admission can reuse the existing `IMagicSpellEffectPreparedSelection` confirmation path; payment, wards and quarantine remain current APIs. No live method is patched by this checkpoint.

## Calm break decision remains explicit

The bounded supported Library search inspected all26 `SPELL_CALM`, all7 `CHAR_AFF_CALM` and all18 `IS_CALMED` matches in the242,965-line dump. These establish application, harmful-action refusals, expiry/dispel and Fury cancellation, but no incoming-attack removal hook. This does not prove the complete historical game lacked such behavior outside the supplied dump. The source comment says until attacked; damage-only cancellation cannot stand in for that comment.

Proposed explicit choice: keep expiry/dispel/Fury cancellation as the recovered executable behavior unless the coordinator chooses an additional native **admitted hostile attack attempt** break policy. If that adaptation is chosen, the main owner supplies a single notification at its admitted attack execution boundary, including misses, excluding refused/stale commands. `IEffectRemoveOnDamage` is not the equivalent hook. No break policy is authored as stock default here.

## Remaining qualification

The first independent focused run passed28 new cases and the57 cleared Pierce cases; failed invocation/build diagnostics are retained. The final code-commit-bound receipt records subsequent execution and hashes separately. Mocked surrounding state, direct child XML roundtrips and pure policy tests are not proof of paid casts, selective native combat cessation, actual stamina-capacity changes, guard refusal, mastery or independent-process reload.

Roused Fury/Still Anger stock construction, source category/terrain/save mappings, reporting, configured/prepared wiring and a dedicated native harness remain the next batch after coordination. No stock completion, installed Mend Flesh acquisition closure, main merge, installer contribution or central ledger change is claimed.
