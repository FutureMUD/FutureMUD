# Fury and Calm prerequisite closure: coordination boundary

Inspection is bound to local source `a7440194ee5ecfe480c7c59c8d5671831c490310` in the isolated five-stock lane. The parent independently cleared Pierce execution `7108d334415965ed15233ca9bf6a3f1e3f3c4fe6` and documentation `a7440194ee5ecfe480c7c59c8d5671831c490310`; this note preserves that series unchanged. No new spell definition or runtime behavior is implemented by this inspection.

## Canonical acquisition path

The authoritative completion brief, Library `libfile_a4606cd0097081918d6c9d9e220e6da0`, rows33/45/46 at lines549/561/562, and committed `Armageddon_Sorcerer_Source_Tree.json` agree:

| Historical spell | Canonical stock name/key | Required parent/raw threshold | Opening/raw cap/printed minimum mana |
|---|---|---|---|
| Fury | Roused Fury / `arm.spell.roused_fury` | Dispel Magick / Unravel Enchantment, raw80 | 30 / 90 / 20 |
| Calm | Still Anger / `arm.spell.still_anger` | Fury / Roused Fury, raw80 | 30 / 90 / 7 |
| Heal | Mend Flesh / `arm.spell.mend_flesh` | Calm / Still Anger, raw80 | 30 / **60** / 20 |

Thus the smallest missing content batch is Roused Fury plus Still Anger, following already implemented Unravel Enchantment. Mend Flesh keeps its existing cap60 and qualified practice to controlled grade7. These are Sorcerer source-tree acquisition thresholds, not the older catalogue's proposed Draw Water/grade2/raw20 links. No central repertoire or acquisition record is rewritten here.

## Recovered historical behavior

Supported Library reads of `codedump.c`, Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, file `file_00000000438871faac40c5042679ce3a`, provide the following evidence. No cloud path or failed materialization transfer is assumed.

**Fury, lines87470-87590:** subtract the opposing Calm affect's power from the new grade. A stronger Calm is weakened in place and ends the cast; equal strength removes Calm and ends the cast; a surviving positive grade produces Fury. The endurance boost is a random integer between grade and integer(grade squared/2), with an Earth-plane grade bonus. Caster terrain controls duration: Air2*grade; City/Inside integer(2.5*grade); Hills/Mountain integer(3.5*grade); Thornlands integer(3.3*grade); Earth-plane4*grade; otherwise3*grade, with minimum1. Existing Fury accumulates duration to cap36 and copies the old affect's other fields, preserving its existing power/endurance modifier rather than taking the strongest new grade. The source comment says non-cumulative; the executable recast branch explicitly accumulates duration, so the comment is not evidence for replacement refresh. The source also has a Mul-race/Thodeliv exception for an additional aggressive rage effect; no native race/drug IDs can be invented from that setting rule.

**Calm, lines83142-83244:** undead, Quickening or Insomnia takes an early branch; the latter two spell affects are removed before returning without Calm. It then weakens/removes opposing Fury by reciprocal source-power subtraction. Remaining positive grade gives duration2*grade; failed historical save applies Calm through `stack_spell_affect` with cap24 and removes remaining Fury. After the save attempt, combat cessation and Mul-rage removal can still count as useful work even when no Calm child is applied. This ordering prevents treating a resisted child as proof that the whole cast did nothing. The source comments specify suppression until expiry or attack; the precise historical incoming-attack removal hook was not located in this bounded inspection and remains an evidence limit, not a verified implemented rule.

**Stacking and clock:** `stack_spell_affect`, lines81574-81592, adds same-type durations, retains maximum power and replaces old affects. Calm therefore has a distinct recast-strength rule from Fury. The previously recovered `RT_ZAL_HOUR=600`, inclusive expiry and remaining-unit conversion apply; native exact deadlines/offline restoration would need explicit adaptation rather than assuming configured world hours or historical offline behavior.

**Delivery, lines217219-217259 and219332-219369:** ordinary spells use the selected character; potion uses the caster; scroll has caster fallback; wand refuses object/missing character. Room/staff Fury includes eligible caster, whereas Calm excludes caster; both apply immortal/ethereal admission. This stock lane can own ordinary character/self definitions. Carriers and area delivery remain outside this batch.

The brief Appendix B calls Roused Fury a native rage primitive (line676). The old register proposes fixed intensity and60*grade durations. Those bounded native adaptations do not establish recovered reciprocal counters, the different stacking rules, endurance parity, or Calm's real combat cessation. They cannot silently substitute for the requested source-informed closure.

## Existing adapter evidence and minimal coordinated boundary

`RageSpellEffect` and `PacifismSpellEffect` create character-owned `SpellRageEffect` / `SpellPacifismEffect` children with fixed builder-editable intensity. Both concrete children persist intensity and use existing parent ownership/expiry. Rage exposes thresholds5/10; pacifism exposes thresholds strictly greater than5/10. They do not implement reciprocal source-grade subtraction, recast policy, combat cessation or incoming-attack cancellation, and their templates do not report operations through `IMagicSpellEffectOperation`.

On this source, `CharacterCombat.WillAttackTarget`, `CanEngage` and `WhyCannotEngage` query `Body.EffectsOfType<IPacifismEffect>()`; `StrategyBase` and the hunting guard also query body-only pacifism, while the spell adapter attaches to the character. Rage/truce and several strategies query character-local rage. This is concrete ownership/authority overlap with the parent's main combat repair, not a reason to attach duplicate child effects or add a private command guard.

Before this pair can qualify as real stock, coordinate these boundaries:

1. **Main combat-authority owner:** canonical applicable combined-effect admission for pacifism/rage, including queued/ongoing attacks and hostile magic; an authoritative way to stop only combat involving the selected calmed actor while preserving unrelated fights; and an incoming hostile attack notification/contract for any opted-in break-on-attack policy. Existing `ICombat.LeaveCombat`/`TruceRequested` alone do not prove all those semantics. The stock lane will not edit `CharacterCombat.cs`, shared strategy/command guards or AI to bypass the authority repair.
2. **Spell lane after coordination:** one optional editable emotional counter/lifetime policy with explicit source grade separate from native intensity/power, different Fury/Calm recast rules, bounded ownership/scheduler cleanup and mutation guards, and truthful reporting for counter reduction/removal versus applied child. Reuse current configured/prepared casting and existing paid quarantine; do not encode source grade as intensity or power enum order. Existing detection-specific lifetime policy must remain detection-specific unless a shared extension is explicitly coordinated.
3. **Setting content/adaptation:** selected eligibility/counter tags for undead, Quickening/Insomnia and exceptional rage; an explicit builder-authored native endurance/stamina mapping. `TraitBoostEffect` offers trait bonuses and `MaximumStaminaFor` uses a configured prog, but arbitrary attribute/check buffs do not prove the historical endurance effect. No Mul/race/drug-specific behavior or generic buff substitute is introduced by this lane.

After that boundary is agreed, the proposed owned files are new `ArmageddonRousedFuryStock.cs` and `ArmageddonStillAngerStock.cs`, a narrow stock-builder partial, dedicated emotional policy/operation partials or new files, spell-specific runtime tests, and a dedicated Fury/Calm native entry/project with entry receipts. Exact shared adapter hooks should be agreed before changing existing files. Installer-owned factory contributions, central dispatch/ledgers and the main worktree remain outside this ownership.

Required qualification is ordinary builder construction/save/reload; legitimate parent thresholds; paid low/high casts; stronger/equal/weaker reciprocal counters; recast cap/retained strength; before-payment target/policy refusal; truthful applied/no-change/resisted reporting and mastery; combat cessation and native harmful-action refusal; any agreed attack cancellation; independent-process reload, expiry and child cleanup. Only uniquely owned disposable databases/processes may be used.

## Outcome

The missing pair cannot currently be qualified using configuration alone without crossing the main combat-authority boundary. No executable files were touched and no native behavioral tests were run for this pair. This is a factual blocker/coordination checkpoint; neither spell nor the Mend Flesh acquisition path is marked complete. The cleared Pierce series and earlier Mend Flesh evidence remain intact.
