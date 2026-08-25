# Animal Combat Balance: Second Pass

## Scope and acceptance fixture

This pass rebalances ordinary AnimalSeeder species against the existing `demo_dbo` fixture. Mythical animals and other non-humans are regression-only. It adds no merit type and no room-to-room charge displacement.

The primary fixture is elephant character `Arjuna` (#50) against `Agathe Herbert` (#51). Agathe's attributes and inventory were preserved: sallet, brigandine pieces, standard, vambraces, gauntlets, greaves, pike and pavise. Combat simulation deep-copying retained all nine items. Arjuna's accepted effective Strength is `120` (raw `12`, racial `+108`) and effective Willpower is `32` (raw `12`, racial `+20`). Both were cured before causal transcript runs.

The database backup used for convergence was `demo_dbo_animal_balance_20260825.sql` (106,773,050 bytes). The accepted full-organic formulas are:

- hit points: `100 + con * 3`
- pain: `50 + wil * 6`
- stun: `75 + con * 2 + wil * 3`

## Accepted ordinary-animal anchors

| Species | Strength bonus | Will bonus | Pain tolerance | Primary 100-run result versus pikeman |
| --- | ---: | ---: | ---: | ---: |
| Mouse | -10 | -4 | 80% | 0% |
| Wolf | +12 | +10 | 130% | 6% |
| Bear | +43 | +15 | 175% | 38% |
| Rhino | +88 | +19 | 200% | 73% |
| Hippo | +88 | +20 | 175% | 64% |
| Elephant | +108 | +20 | 200% | 76-78% |
| Mammoth | +133 | +22 | 185% | regression anchor |
| Oliphant | +198 | +24 | 200% | mythical regression only |

The builder experiments changed one dimension at a time. Representative accepted commands were `race set attributebonus strength 108`, `race set attributebonus willpower 20`, `race set paintolerance 200%`, and the equivalent species-specific values. The final charge correction used `weaponattack edit 912` followed by `weaponattack set intention trip disadvantage aggressive`, leaving the move with `Attack` and `Wound`; charge-owned code applies its knockdown and advantage effects.

## Deterministic batches

All listed runs completed with no runtime errors, timeouts or stalemates.

| Matchup | Seeds | Runs | Animal result |
| --- | --- | ---: | ---: |
| Elephant versus pikeman | 20,000-20,099 | 100 | 77% |
| Elephant versus pikeman | 21,000-21,099 | 100 | 77% |
| Elephant versus pikeman | 22,000-22,099 | 100 | 76% |
| Mouse versus pikeman | 23,000-23,099 | 100 | 0% |
| Wolf versus pikeman | 24,000-24,099 | 100 | 6% |
| Bear versus pikeman | 30,000-30,099 | 100 | 38% |
| Rhino versus pikeman | 31,000-31,099 | 100 | 73% |
| Hippo versus pikeman | 27,000-27,099 | 100 | 64% |
| Elephant versus three cloned pikemen | 32,000-32,099 | 100 | 3% |

Mirror batches at seed 33,000 produced elephant 79 / human 18 in both team assignments; the remaining bouts ended without a winning team, so there was no side advantage. A same-batch `seed 34000 step 0` replay matched exactly at 35 virtual seconds, 35 events and fingerprint prefix `v2:8e07a50644b6`. After restore and seeding, batch `5478289e-b5a5-49e5-a3cc-f5c5bc67838e` at seed 20,000 produced elephant 78%, human 18%, with all 100 runs completed. Same-batch post-seed replay `a6c02b80-ce88-458e-b94a-21c4654c5bf2` also matched.

## Charge proof

The simulator now accepts `range ranged`, permitting a real outside-melee opening. Seed 35,000 completed in 35 virtual seconds with team 1 winning. The transcript shows Agathe attempting to keep Arjuna at bay with the pike, failing the receive-charge response, attempting the fallback dodge, being hit on the belly, passing out from pain and being reeled by the impact. The move is available only for an unmounted attacker at least one size category larger; this fixture is `VeryLarge` versus `Normal`. A two-category gap or major success causes knockdown. Behemoth Charge is absent from ordinary melee selection and is prioritised over other ranged-capable natural attacks when the creature is eligible to close.

## Seeder convergence

The restored baseline received the migration, HumanSeeder `combat-rebalance`, and AnimalSeeder `full` with the existing full non-human health model. A second AnimalSeeder pass produced identical checksums:

| Managed table | Checksum |
| --- | ---: |
| Races | 1,568,241,852 |
| WeaponAttacks | 3,782,287,887 |
| Races_WeaponAttacks | 3,563,003,844 |
| BodypartProto | 962,005,244 |
| TraitExpression | 774,925,629 |
| ArmourTypes | 833,447,532 |

The converged database has one named Behemoth Charge attack, 32 ordinary-animal race/bodypart links, zero weighted bones and zero externally armoured organs. The rerun path no longer rebuilds existing bodies, compounds `MaxLife`/hit weights, overwrites bones as flesh, or leaves managed attack speed, difficulty and weighting stale.

## Known limitations

- The exact outcome bands are fixture-specific balance targets, not guarantees for every armour material, combat setting or weapon catalogue.
- Separate simulator invocations capture separate UTC epochs; exact fingerprint claims use repeated seeds within one batch.
- Direct Dapper calls and authored external hooks remain outside the simulator rollback guarantee.
- Mythical catalogue values, new pain-resistance merits and cell-to-cell charge displacement remain out of scope.
