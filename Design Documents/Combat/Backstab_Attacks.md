# Backstab Attacks

Backstabs are single-target weapon or natural attacks available through normal automatic combat selection and explicitly through the `backstab` command. They use the ordinary attack, defense, damage, pain, stun, and combat recovery machinery, with optional resistance-based secondary effects and armour handling layered onto a normal attack.

## Eligibility and opening

The attacker must be able to see and reach a character target, have combat settings, and have a usable authored backstab attack. The attacker must be on the target's rear combat facing or have an unused unseen opening against that target. A command can initiate combat when the target cannot see the attacker; an existing combat engagement is also valid when the rear-facing or unused-opening condition is met. The command accepts `backstab <target>` or `backstab` to use the current combat target.

The engine captures whether the target could see the attacker when a new encounter begins, before combat-start effects remove concealment. This per-target opening survives joining or merging into an existing fight. It is spent on the first actual hostile attack attempt against that target, even if the attack misses. Merely hiding again, changing targets, or merging combats does not refresh it. Rear-facing eligibility is independent of the unseen opening.

Automatic backstabs participate in the existing weapon and natural attack channels and retain their normal percentages and attack weights. The command searches currently available weapon attacks in weapon-source preference order, then usable racial natural attacks. Within an eligible source it selects by the attacks' existing positive `Weighting`; zero-weight attacks are excluded. Normal weapon and natural-attack usability, position, bodypart, stamina, and defense checks still apply.

The seeded predator opening uses a backstab before the existing `AmbushAttack` when the hunter has closed into melee range. Leopard and Jaguar receive the natural `Predatory Back Rake`; if it is not usable, the hunt proceeds through its existing ambush path, including legal cross-layer ingress where the authored ambush attack permits it. This is initial stock tuning, not a claim about live balance or a general policy for every predator.

## Seeded attack profiles

Clinch attacks spend the same unseen opening before their attack roll, including unsuccessful attempts; entering a clinch does not preserve a second first-attack opportunity.

The following are the initial authored profiles. `N` means Normal. Reel and advantage/disadvantage values apply per failure degree of the victim's Normal `StaggeringBlowDefense` check; penetration and worn-armour bypass modify the hit independently of that resistance check. Unlisted optional effects are zero. Damage, pain, stun, stamina cost, base delay and weight retain the meanings used by the ordinary attack system; these values have not been established as live balance results.

| Attack | Type and recipients | Damage / pain / stun | Stamina / delay / weight | Secondary effect |
| --- | --- | --- | --- | --- |
| Knife Backstab | Weapon; knife attacks | N / N / N | 5 / 1.1 s / 200 | Reel 0.25 s; target offensive disadvantage 5 |
| Dagger Backstab | Weapon; dagger attacks | Good / Good / Good | 4 / 1 s / 200 | Penetration bonus 10; 10% worn-armour bypass |
| Sapping Blow | Blackjack | 0.25 N / 0.5 N / 2 N | 4 / 1.1 s / 300 | Head-only; reel 0.75 s; target offensive disadvantage 5 |
| Predatory Back Rake | Natural; Leopard and Jaguar | 1.5 × AnimalClaw / same / same | 5 / 1.3 s / 200 | Attacker offensive advantage 5; target defensive disadvantage 5 |
| Manticore Raking Backstab | Natural; Manticore | 1.5 × AnimalClaw / same / same | 6 / 1.5 s / 150 | Reel 0.5 s; target offensive disadvantage 5 |
| Werewolf Savaging Backstab | Natural; Werewolf Hybrid | 1.25 × existing physical formula / same / same | 6 / 1.3 s / 150 | Target offensive and defensive disadvantage 5 each |

The weapon `BackstabAttack` profile uses Normal attack difficulty, Easy dodge and parry difficulties, Very Easy block difficulty, and Easy/Hard recovery difficulties. Natural backstab clones retain the donor claw attack's ordinary difficulties, positions, and quality. Their authored multipliers scale the inherited physical damage, pain, and stun formulas.

The separate Medieval sample weapon is a Blackjack weapon type with Club skill, one-handed use, reach 1, parry modifier -1, parry stamina cost 2, and NonLethal damage type. Its ordinary `Bash` and `Close Bash` attacks each use 0.25 N damage, 0.5 N pain, and N stun, with stamina 3 and weight 100; their base delays are 0.8 s and 0.6 s respectively. `Sapping Blow` targets the native `Head Back` bodypart shape. The seeded human anatomy has no generic `Head` shape; the installer refuses a missing `Head Back` prerequisite before creating attack expressions. The sample item has stable ID `medieval_military_blackjack_leather_wrapped`; it is a 450 g lead blackjack of Small size and Standard quality, with base value 12. These are authored catalogue defaults, not price or combat-balance measurements.

## Authoring

Create a weapon attack with `weaponattack new BackstabAttack`, or create an unarmed/natural attack with `weaponattack new BackstabAttackUnarmed`. The attack type is fixed after creation. Configure the regular attack fields as for any weapon attack: especially the damage, pain, and stun formulas, base delay, attack difficulty, stamina, and weight. The ordinary damage, pain, and stun formulas are unchanged; backstab configuration does not multiply them.

For a weapon attack, add it to the desired weapon type through that type's attack builder. For a natural attack, attach it to a race and bodypart, for example `weaponattack natural <race> <attack> <bodypart> <quality>`. The attack must not already belong to a weapon type to be used as a natural attack. Use the same authored attack properties and message setup as other attacks.

While editing a backstab attack, `weaponattack set` accepts these additional options:

| Setting | Builder syntax | Meaning |
| --- | --- | --- |
| Resistance | `resist <difficulty>` | Difficulty for the victim's independent `StaggeringBlowDefense` check. Defaults to `Normal`. |
| Reel | `reel <seconds>` | Combat scheduler delay per failure degree. Defaults to zero. |
| Attacker advantage | `attackeradvantage <offence> <defence>` | Attacker offensive and defensive advantage gained per failure degree. Both default to zero. |
| Target disadvantage | `targetdisadvantage <offence> <defence>` | Victim offensive and defensive advantage reductions per failure degree. Both default to zero. |
| Penetration | `penetration <bonus>` | External bonus to the ordinary melee armour-penetration check. Defaults to zero. |
| Worn armour bypass | `armourbypass <percentage>` | Independent chance to skip all worn armour for this hit, from 0% to 100%. Defaults to zero. |

For example, `weaponattack set resist Hard`, `weaponattack set reel 1.5`, `weaponattack set attackeradvantage 0.2 0.1`, `weaponattack set targetdisadvantage 0.3 0.2`, `weaponattack set penetration 5`, and `weaponattack set armourbypass 25%` configure a moderate secondary effect and a 25% worn-armour bypass chance. Omit any option that should remain inactive. Backstabs always affect one target; attempts to configure a larger target count are rejected.

These fields are saved in the weapon attack's existing `AdditionalInfo` XML. They require no schema migration and do not add a separate Backstab skill or change stock attack balance.

## Resolution

The chosen move resolves through the normal weapon or natural attack path. The victim can use ordinary combat defense. The attack's authored damage, pain, and stun formulas and the standard armour pipeline still determine the base result. A positive penetration bonus supplies an external bonus to the existing melee penetration check. Separately, the configured bypass chance can mark the damage to skip every worn armour item. Neither path bypasses natural armour, magic protection, prosthetics, or a successful block; a successful block still resolves normally.

After a successful move, the engine performs at most one `StaggeringBlowDefense` check for the victim, and only when the victim received positive damage, pain, or stun and at least one secondary effect is configured. The check uses the configured resistance difficulty (default `Normal`). On a failure, each configured reel or advantage/disadvantage amount is multiplied by the failure degrees. Reel delays the victim's normal combat schedule, scaled by the global combat-speed multiplier; it does not replace the attack's normal recovery timing. A pass, failed attack, or hit with no positive victim damage/pain/stun applies no secondary effects.


## Specific proficiencies and sapping

A Backstab attack can carry optional `<Proficiency>trait-id</Proficiency>` and `<Command>backstab|sap</Command>` fields. Legacy attacks without a proficiency retain their ordinary weapon/natural check. A configured proficiency must resolve and be known with a positive effective value; it supplies only that attack's hit check. It never replaces the weapon family's ordinary attack skill. Both selection and queued execution enforce it, including hostile-admission and defence callbacks and before wound delivery. A revoked skill before an attempt consumes no opening; revocation after a roll prevents further damage but retains the attempted action's cost.

The `sap` command selects only attacks configured for `sap`; `backstab` selects the other opening attacks. Both use the existing unseen-opening/rear-facing admission, native engagement, armour and defence paths. Builders can set `proficiency <skill|none>` and `command <backstab|sap>`; cloning/save preserve both. The new Armageddon Combat install assigns Backstab to its Knife/Dagger opening profiles and Sap to the head-targeted Blackjack profile, retaining their distinct piercing/penetration versus low physical/high stun damage. Subsequent Combat reruns are read-only. No ordinary knife or blackjack hit check is rebound.
