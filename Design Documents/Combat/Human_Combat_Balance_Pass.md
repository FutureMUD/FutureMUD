# Human Combat Balance Pass

## Scope

This pass used `impdebug combatsim` against the local `demo_dbo` data to compare human melee loadouts, tactics, armour levels and skill bands. It deliberately tested historically suspect combinations, especially warding polearms and dual daggers, without requiring a universal rock-paper-scissors model.

The permanent fixtures created in `demo_dbo` are development evidence rather than stock game content. Engine and seeder changes are the portable source of truth. The accompanying `Human_Combat_Balance_*.txt` files are an execution ledger from the live tuning session; they include intermediate and diagnostic commands and must not be replayed as an idempotent setup script.

## Fixture matrix

The mid-tier fixtures use Strength 20, Constitution 17, Dexterity 10, Intelligence 17, Willpower 13, Perception 14 and relevant combat skills at 52 unless otherwise noted.

| NPC template | Loadout or role | Combat setting |
| ---: | --- | --- |
| #108 | dual daggers, no armour | Dual Wielder (Auto) |
| #109 | sword and shield, no armour | Shielder (Auto) |
| #110 | pike, no armour | Polearm Warder (Auto) |
| #111 | two-handed sword, no armour | Zweihander (Auto) |
| #112 | mace and shield, no armour | Shielder (Auto) |
| #113 | spear and shield, no armour | Shielder (Auto) |
| #114 | axe and shield, no armour | Shielder (Auto) |
| #115 | unarmed grappler | Grappler (Auto) |
| #116 | unarmed pit fighter | Pit Fighter (Auto) |
| #117 | pike, no armour | Zweihander (Auto), used as the non-warding control |
| #118-#121 | heavy-armour variants | sword and shield, mace and shield, dual daggers, two-handed sword |
| #122/#125 | newbie, skill 20 | sword and shield / dual daggers |
| #123/#126 | capped, skill 80 | sword and shield / dual daggers |
| #124/#127 | god-tier, skill 120 and optimised attributes | sword and shield / dual daggers |
| #128 | dual daggers, clinching | Dual Wield Clincher (Auto) |
| #129/#130 | unarmed tactical controls | Swarmer (Auto) / Outboxer (Auto) |
| #131 | long spear, no armour | Polearm Warder (Auto) |

The weapon-only outfit templates are #5843-#5850. Pike, two-handed-sword and long-spear entries use the explicit `wielded` placement so oversized weapons are not rejected by the ordinary held-item path.

## Accepted comparison batches

These results use twenty-run batches except for the final dual-wield comparisons, which use thirty consecutive seeds. Percentages are directional evidence for this data set, not universal balance guarantees.

| Match-up | Result |
| --- | --- |
| dual daggers vs sword and shield | dual 11/30, shield 19/30 |
| dual daggers vs two-handed sword | dual 10/30, two-handed sword 20/30 |
| dual daggers vs warding pike | dual 7/30, pike 23/30 |
| heavy dual daggers vs heavy sword and shield | dual 0/30, shield 26/30, 4 time limits |
| warding pike vs sword and shield | 10/20 each |
| standard pike vs sword and shield | pike 12/20, shield 8/20 |
| sword and shield vs two-handed sword | shield 12/20, two-handed sword 8/20 |
| heavy two-handed sword vs heavy sword and shield | two-handed sword 20/20 |
| mace and shield vs sword and shield | mace 2/20, sword 13/20, 5 time limits |
| spear and shield vs axe and shield | spear 0/20, axe 18/20, 2 time limits |
| newbie sword and shield vs mid-tier | mid-tier 20/20 |
| mid-tier sword and shield vs capped | capped 14/20, mid-tier 5/20, 1 time limit |
| capped sword and shield vs god-tier | god-tier 20/20 |
| outboxer vs swarmer | outboxer 13 wins, swarmer 2 wins, 2 stalemates; 18 completed outcomes |

The accepted dagger batches were run only after the simulator transcript proved that both daggers were wielded and the new dual-only feint was actually selected. Earlier one-weapon and oversized-weapon batches are diagnostic evidence only and are excluded from the table.

## Findings

### Dual wielding

Dual wielding was not merely weak: dual-only attacks were structurally unreachable because handedness was inferred from one weapon at a time. Automatic inventory management also requested only one melee weapon. Correcting both paths moved dual daggers from near-total irrelevance to a credible aggressive style. The new `Dagger Dual Feint` gives it a distinctive fast, low-stamina attack that pressures parry and block without invalidating dodge.

The final match-ups do not reproduce an unchecked dual-dagger meta. Sword and shield, a two-handed sword and a warding pike all remain favoured, while heavy armour is a decisive dagger counter. Dual daggers nevertheless win often enough in unarmoured fights to be strategically relevant.

### Warding and reach

The polearm warder beat dual daggers 23/30, but split evenly with sword and shield. The non-warding pike performed slightly better than the warder against sword and shield in this sample. Warding is therefore a strong answer to short weapons rather than a universal best setting. The dedicated `Polearm Warder` templates prevent this tactic from being coupled accidentally to an incompatible preferred loadout.

### Armour and attack families

Heavy armour changes the problem substantially. Daggers have a hard counter and shield-on-shield fights can run for much of the thirty-minute virtual limit. The heavy two-handed sword's clean result shows that high-impact weapons still resolve heavily armoured fights; the attrition is not a general inability to damage armour.

The low mace result and the spear's 0/20 result against axe and shield deserve a future, larger weapon-family pass. They are not sufficient grounds by themselves to globally increase blunt or spear damage because shield defence, attack selection and the specific stock prototypes all contribute to the result.

### Skills and unarmed tactics

Skill progression is intentionally powerful and monotonic in the tested sword-and-shield ladder. The capped-to-god-tier jump is especially decisive. Unarmed grappling and pit-fighting combinations produced many virtual-time limits, while outboxing clearly led swarming. These warrant a separate unarmed-control pass with shorter diagnostic limits and move-selection traces; they were not disguised as completed wins or used to tune weapon damage.

## Engine and content changes

- Dual-wield automatic inventory plans now request and ready two melee weapons.
- A weapon already wielded in the wrong grip is deliberately unwielded and rewielded instead of leaving the plan unsatisfied.
- Attack handedness recognises a second wielded non-shield melee weapon. One-handed attacks remain legal while dual wielding, and dual-only attacks become selectable.
- Outfit templates support an explicit `wielded` placement. This safely materialises pikes and other items too large for the held-item path.
- `Dagger Dual Feint` and its matching success/failure message are installed by the Combat Seeder and reconciled on rerun.
- `Dual Wielder`, `Dual Wield Clincher` and `Polearm Warder`, with manual and fully automatic variants, are installed and reconciled as humanoid global templates.

## Mace and spear follow-up

The dedicated follow-up retained the same mid-tier, no-armour controls and used thirty deterministic seeds per accepted matchup. Mace and shield finished 12/30 against sword and shield, 12/30 against axe and shield, and 15/30 against standard dual daggers. In heavy armour, mace and shield finished 16/30 against sword and shield. All 120 runs completed, so the mace is now a credible peer rather than the prior near-irrelevant option without becoming a universal answer.

Maces are now lethal, reach 2 weapons. Their ordinary swings use the normal damage band with slightly higher stamina and recovery commitments, while the overhead swing uses the good band. `Mace Concussive Blow` adds a deliberately lower-weight one-handed staggering option with matching success and failure messages. Training maces are correctly classified as training weapons and share the corrected reach without receiving lethal damage expressions.

The stock plain short spear was `Large`, which forced two-handed wielding despite its description, `Short Spear` component, shield-line attacks and sword-and-board fixture. It is now `Normal`. A transcript proved that the repaired fixture wielded both spear and shield before its aggregate results were accepted. The short spear warder then finished 9/30 against axe and shield, 12/30 against sword and shield, and 12/30 against standard dual daggers. From ranged start it finished 21 wins, 8 losses and 1 mutual incapacitation against axe and shield, preserving reach as a meaningful situational advantage.

The apparent first clincher result was also rejected: the retained NPC had no outfit and still used generic `Clincher (Auto)`. After repairing it, a transcript proved that both daggers equipped, the clinch was entered, `Dagger Dual Feint` fired and `Spear Shaft Shove` fired. The repaired dual-dagger clincher won 22/30 against the spear warder. This supplies a clear close-range counter to warding without making standard daggers automatically superior.

`Spear Butt Strike` and `Spear Shaft Shove` give shield-side short spears close-range recovery tools; equivalent two-handed attacks are installed for long spears. `Spear Warder` manual and automatic templates pair warding with sword-and-board and shield preference. While authoring the shove message, combat-message validation crashed because weapon, unarmed and clinch pushback move types were absent from the validator dispatch. The validator now accepts all three forms, with focused regression tests.

The long-spear warder finished 22/30 against sword and shield and 20/30 against the repaired dual-dagger clincher. This is strong enough that no further global spear damage increase is warranted. It is not evidence of an unbeatable style: the sample is only two matchups, its large weapon has loadout constraints, and the short spear already shows that losing ranged initiative or being forced into a clinch changes the result substantially. A future extension should test long spear against heavy armour and dedicated skirmishing before considering any further adjustment.

The deterministic long-spear replay also exposed a missing `ResistBreakClinch` combat message. A fallback success/failure pair is now installed by the Combat Seeder. The first repair replay caught reversed actor and defender roles and then incorrect defender-side verb agreement; the final replay renders “she fails to keep you trapped” / “you fail to keep ... trapped” correctly and proceeds to select `Long Spear Shaft Shove`. This final transcript check closed both the missing-message bug and direct execution coverage for the new long-spear recovery attack.

The invalid pre-repair spear and clincher batches remain diagnostic evidence only and must not be used in balance comparisons.

## Reproduction guidance

Use current NPC-template revisions and stage one template on each team in the same cell. Validate before every batch, retain the default thirty-minute virtual limit for final comparisons, and use at least twenty sequential seeds. Inspect a single full transcript whenever a loadout or attack family changes; an aggregate win rate is invalid if the intended weapon was left in the source cell or if the intended attack never became eligible.

For deadlock investigation, lower `maxtime`, retain enough transcript entries to see repeated move selection, and inspect per-run stop reasons. A time limit is not a win and should remain separate from an engine-reported stalemate.
