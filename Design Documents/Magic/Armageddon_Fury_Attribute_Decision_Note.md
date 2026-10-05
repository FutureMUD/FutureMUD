# Fury: approved attribute mapping and remaining qualification

Luke approved an attribute bonus for Fury in message `Sentinel_782e8d38f3188191b2829f4dcce1b8c1`: "I agree that an attribute bonus is the expected effect. In the seeder it should be reasonably easy to infer which attribute they have used (as is done elsewhere in the SkillPackageSeeder for example)". The coordinator requested automatic inference when clear, a diagnostic and builder override when missing or ambiguous, and a configurable runtime attribute definition. This supersedes the pending-mapping wording in the earlier preserved groundwork documents. This note implements no Fury stock or seeder change; it accompanies the completed ethereal component checkpoint before a lane change.

## What endurance meant in Armageddon

The supplied `codedump.c`, Library `libfile_befb4ae78d1c8191aaa2640c49912d9d`, file `file_00000000438871faac40c5042679ce3a`, distinguishes the attribute from the resource:

| Source evidence | Meaning |
|---|---|
| Fury lines87544-87545: `af.modifier = end_boost`, `af.location = CHAR_APPLY_END` | Fury applies an endurance attribute modifier. |
| Attribute handler lines49232-49233: `SET_END(ch, GET_END(ch) + mod)` | The affect changes effective endurance. |
| Accessors lines237049-237057 and macros239571-239572 | Effective endurance is `ch->tmpabilities.end`. |
| Macro239614: `GET_MOVE(ch)` is `ch->points.move` | Current spendable move/stamina is a separate resource. |
| Macro239616: `GET_MAX_MOVE(ch)` calls `move_limit(ch)` | Maximum move is calculated. |
| `move_limit` line81304: `100 + end_app[GET_END(ch)].move_bonus + ch->points.move_bonus` | Endurance contributes to derived maximum move through a lookup table. Other adjustments also apply. |

Fury does not directly refill `points.move`. The native attribute-bonus category therefore matches the recovered source. Choosing a native attribute, its unit scale and the configured stamina-capacity prog remains an explicit engine adaptation; the historical formula does not establish a universal ten-stamina-per-point conversion. Bounded supported Library read/find snapshots are preserved at `.artifacts/test-runs/fury-attribute-decision-source-evidence.json`. No full-file transfer or cloud filesystem path is claimed.

The follow-up lifecycle trace confirms `affect_to_char` (49429-49445) calls `affect_modify` and `affect_total`. `affect_total` (49385-49422) resets temporary abilities from base abilities, reapplies equipment/spell modifiers and clamps the effective attributes to `MAX_ABILITY`; it writes no current move. `affect_remove` removes the affect and calls that same rebuild (through49507). Maximum move is calculated on demand, not updated as a stored maximum by `CHAR_APPLY_END`: `move_limit` adds race adjustments, caps the pre-spice total at1000, then multiplies by `spice_factor` (81379-81383). Current move writes use the separate `set_move` (238882-238890), which clamps the requested value to zero through the currently calculated maximum; `adjust_move` delegates to it. Thus Fury attachment/removal changes derived capacity without itself refilling or immediately clamping current move. The native immediate capacity reconciliation/clamp on removal is an explicit adaptation. Exact bounded snapshots for this follow-up are preserved at `.artifacts/test-runs/fury-apply-end-lifecycle-source-evidence.json`.

## Source behavior retained by the proposal

Fury first subtracts opposing Calm power. Stronger Calm is weakened and ends the cast; equal power removes Calm and ends the cast; positive residual power produces Fury. Its endurance roll is inclusive `number(grade, integer(grade*grade/2))`, with an additional grade on the Earth plane. Grade1's inverted bounds return1 under the recovered random helper; grade7 is7-24, or14-31 on Earth.

Caster terrain determines duration: Air2*grade; City/Inside integer(2.5*grade); Hills/Mountain integer(3.5*grade); Thornlands integer(3.3*grade); Earth4*grade; otherwise3*grade, minimum1. Recasting accumulates duration to36 source hours while retaining the old affect's power and endurance modifier. This executable behavior overrides the contradictory non-cumulative comment. The source clock is600 seconds per source hour; exact native deadlines and offline restoration are adaptations. The Mul/Thodeliv aggressive-rage exception needs setting mappings and is not permission to invent native race or drug IDs.

Canonical acquisition is Unravel Enchantment raw80 -> Roused Fury opening30/cap90 -> Still Anger at Fury raw80 -> Mend Flesh at Calm raw80. Mend Flesh retains cap60 and its previously verified practice to controlled grade7. Printed minimum mana is20 for Fury and7 for Calm. Older catalogue proposals do not replace the recovered source tree or these counter/recast rules.

## Current native support and seeder convention

`SpellSourceFuryEffect` already exposes a configurable attribute through `ITraitBonusEffect`: effective bonus is source endurance points times positive builder-configurable `UnitsPerSourcePoint`. The native maximum-stamina prog can observe that effective trait. `Body.ReconcileEmotionalStaminaCapacity` recomputes capacity through that prog and clamps current stamina; it does not refill stamina. The corrected departing-effect marker excludes only the removed Fury during native removal, preserving other bonuses. The verified expiry/dispel fixture uses native effect ownership/scheduling and an explicit capacity prog; it does not prove a seeded world's prog or paid Fury stock.

`DatabaseSeeder/Seeders/SkillPackageSeeder/SkillPackageSeeder.cs`, lines563-573, builds a case-insensitive dictionary of attribute definitions (types1/3), then infers its constitution attribute in order Constitution, Physique, Endurance, Body. The same convention recurs at lines1426-1436. Reuse that semantic inference style for Fury rather than a hardcoded database ID. The subsequent implementation must establish a clear mapping, report missing/ambiguous definitions, retain an explicit builder override, and keep the runtime definition editable. This inspection settles neither an exact seeder algorithm nor a new numeric stock multiplier.

The previously discussed alternatives were a one-native-unit-per-source-point attribute mapping, a scaled attribute mapping, and a separate capacity-only bonus. Luke has now selected the attribute approach. A capacity-only bonus would diverge from the historical attribute effect. Configurable units let a world express its own attribute scale; no unapproved stock default is silently authored in this checkpoint.

## Combat boundary and qualification still required

Calm's approved break rule, message `Sentinel_17ba194d1fe481918a659d2d5b9a0972`, is an admitted incoming hostile attack attempt, including a miss. The exact historical incoming removal hook remains unrecovered; this is an approved engine adaptation. Main combat authority owns admission, incoming-attempt notification and selective cessation. The spell lane must consume the agreed hooks without adding duplicate guards or stopping unrelated fights.

Fury/Calm ordinary builder construction, seeding, paid low/high casts, source counters/recasts, reporting/mastery, configured stamina prog, independent-process persistence, selective combat cessation and attack-break behavior remain to be qualified. Generic fixed-intensity rage/pacifism alone does not supply those semantics. No shared combat API, main-owned guard, central ledger, installer registry or shared native dispatch is changed here.

Supporting committed context: [source and acquisition boundary](Armageddon_Fury_Calm_Boundary_Note.md), [emotional policy integration](Armageddon_Emotional_Policy_Integration.md), [groundwork verification](Armageddon_Emotional_Groundwork_Verification.md), [native removal correction](Armageddon_Emotional_Stamina_Removal_Fix.md), [selective cessation review](Armageddon_Emotional_Cessation_Review_Note.md), and [casting hook proposal](Armageddon_Emotional_Casting_Hook_Proposal.md). These are preserved historical records; the approvals recorded above supersede their pending-decision wording.
