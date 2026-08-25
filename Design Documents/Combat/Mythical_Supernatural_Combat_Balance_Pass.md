# Mythical and Supernatural Combat Balance Pass

## Scope and fixed model

This pass extends the accepted ordinary-animal combat model to all 43 Mythical Animal Seeder races and all 46 Supernatural Seeder races. It does not add a move type, schema field or public runtime API. It uses the existing pain-tolerance, charge, breath, screech, multi-target, auxiliary-action and planar-interaction systems.

The named full-organic health expressions remain:

- HP: `100 + con * 3`
- pain: `50 + wil * 6`
- stun: `75 + con * 2 + wil * 3`

The source of truth is each seeder template's `CombatBalance` record. It owns the tier, ordinary-animal or humanoid baseline, pain tolerance, natural-armour quality, attack-profile key, charge eligibility and signature action. Attribute profiles are expressed as effective-stat targets relative to the stock average unmodified value of 11.

## Acceptance ladder

| Tier | Role | Primary benchmark |
| ---: | --- | --- |
| 0 | Nuisance | 0-5% versus entry pikeman |
| 1 | Minor threat | 5-20% versus entry pikeman |
| 2 | Serious threat | 25-45% versus entry pikeman |
| 3 | Elite threat | 50-70% versus entry; 20-40% versus veteran |
| 4 | Monster | 75-90% versus entry; 45-65% versus veteran; 15-35% versus three veterans |
| 5 | Great beast | 95-100% versus entry; 75-90% versus veteran; 35-55% versus three veterans |
| 6 | Party boss | 85-100% versus three veterans; 45-65% versus six veterans |
| 7 | Avatar | 75-90% versus six veterans; 40-60% versus ten veterans |

The veteran is a permanent clone of the original entry pikeman and exact inventory. Strength, Constitution, Agility, Dexterity, Willpower and Perception are raised by four up to racial caps. Pike and active defensive traits are raised by twenty up to their caps. Parties use exact simulator clones.

## Mythical catalogue ledger

| Tier | Stock races | Strength target | Pain tolerance | Health multiplier | Armour quality |
| ---: | --- | ---: | ---: | ---: | --- |
| 1 | Cockatrice, Selkie, Myconid, Dryad, Owlkin, Avian Person | 16-24 | 115% | 0.90 | Poor |
| 2 | Warg, Unicorn, Pegasus, Minotaur, Naga, Mermaid, Plantfolk, Hippocamp, Qilin | 28-52 | 145% | 1.15 | Substandard |
| 3 | Dire-Wolf, Hippogriff, Phoenix, Basilisk, Giant Beetle, Giant Ant, Giant Mantis, Giant Spider, Giant Scorpion, Giant Centipede, Giant Worm, Centaur, Garuda, Giant Eagle, Bunyip | 55-90 | 180% | 1.45 | Standard |
| 4 | Dire-Bear, Griffin, Manticore, Wyvern, Fell Beast, Ankheg, Ent, Pegacorn, Yacumama | 85-125 | 225% | 1.85 | Good |
| 5 | Colossal Worm, Huorn | 155-180 | 275% | 2.40 | Very Good |
| 6 | Dragon, Eastern Dragon | 250-285 | 300% | 3.25 | Great |

Analogue ordering is enforced by tests, including `Wolf < Warg < Dire-Wolf` and `Bear < Dire-Bear`. Grounded high-mass forms receive `Behemoth Charge`; aerial and aquatic forms retain their sweep, carry, drop, aquatic-charge or drowning actions.

`Dragon` and `Eastern Dragon` are adult regional party bosses. Western breath has range two, a heavier Aura coefficient, a 5.5-second delay and stronger ongoing fire. Eastern breath has range three, a lighter Aura coefficient, a 4.8-second delay and more stun pressure. Both can affect the primary victim plus three additional victims. Their bite, claw, horn, tail, wing and charge actions remain physical and Strength-scaled.

## Supernatural catalogue ledger

| Tier | Stock races | Pain family | Health multiplier | Armour quality |
| ---: | --- | --- | ---: | --- |
| 0 | Imp, Familiar | living/infernal 100% | 0.65 | Bad |
| 1 | Incubus, Succubus, Spirit, Ghost, Ancestral Spirit, Zombie, Skeleton | living 145%; spirits/undead 475-525% | 0.90 | Poor |
| 2 | Ishim, Fallen Ishim, Werewolf, Vampire, Lich, Ghoul, Mummy, Nature Spirit, Elemental Spirit, Specter | living 190%; spirits/undead 550-600% | 1.25 | Substandard |
| 3 | Cherubim, Bene Elohim, Fallen Cherubim, Fallen Bene Elohim, Fury, Hellhound, Werewolf Hybrid, Wraith, Demigod | living 235%; spirits/undead 625-675% | 2.50 | Good |
| 4 | Elohim, Malakhim, Seraphim, Fallen Elohim, Fallen Malakhim, Fallen Seraphim, Fiend | 280% | 10.00 | Excellent |
| 5 | Erelim, Hashmallim, Ophanim, Fallen Erelim, Fallen Hashmallim, Fallen Ophanim | 325% | 16.00 | Heroic |
| 6 | Chayot HaKodesh, Fallen Chayot HaKodesh, Balrog, Lesser God | 370% | 30.00 | Legendary |
| 7 | Greater God | 400% | 50.00 | Legendary |

Supernatural humanoid Strength remains morphology-led. Aura rises sharply for angelic, infernal, spirit and divine signature attacks; Willpower drives sonic, command, spirit and undead effects. `Balrog` is the deliberately physical exception at effective Strength 240. `Greater God` is an embodied avatar with effective Strength 145 and party-scale Aura rather than an omnipotent deity.

On worlds whose attribute catalogue does not define an `Aura`, `Luck` or `Spirit` attribute, the signature-expression binding falls back to Willpower. This preserves runnable stock content without silently adding a new core attribute to an established game; the template's Aura targets remain ready for worlds that do define one. Aura and Will expressions use quadratic rather than linear trait scaling, so a high choir or avatar separates cleanly from a minor manifested spirit even under that fallback. Seeder-owned supernatural attacks use explicit accuracy and defence difficulties and high selection weights; they do not inherit implementation-only animal-donor balance.

## Owned-field convergence

On every rerun, both seeders reconcile stock race health multiplier, pain tolerance, attributes and dice, strategy, natural armour, bodypart combat fields and natural-attack links. Supernatural attacks additionally reconcile every persisted combat field and their stock combat message. Existing named stock rows are updated; builder-created clones and unrelated custom links are preserved.

External anatomy carries at most one natural-armour layer. Bestial mythical and supernatural bodies use external bodypart armour with race quality and no duplicate race armour. Humanoid tissue keeps the normal humanoid race layer. Bones have zero direct hit weighting and retain bone armour; organs have zero direct weighting and no armour.

## Live fixture and batch ledger

The persistent `demo_dbo` fixtures and accepted batch evidence are recorded here during live validation. The seeder pass deliberately does not restore or roll back the database.

| Fixture | Source ID | Construction | Notes |
| --- | ---: | --- | --- |
| Entry pikeman | character #51 | Existing Agathe Herbert and exact inventory | Continuity fixture |
| Veteran hunter | NPC template #19 | Entry clone, +4 available core attributes, +20 pike/defence, outfit #5842 | Permanent current revision |
| Mythical natural fixtures | NPC templates #18 and #20-61 | One stock-race fixture per race | Permanent; #60 Dragon uses Beast Artillery |
| Supernatural natural fixtures | NPC templates #62-107 | One stock-race fixture per race | Permanent; corporeality controls remain separate from manifested results |

### Representative tuning batches

| Creature | Fixture | Opponent | Seed range | Creature result | Stop quality | Batch ID |
| --- | ---: | --- | --- | ---: | --- | --- |
| Warg | #18 | entry #51 | 41000-41019 | 40% | 20/20 completed | tuning capture |
| Cockatrice | #20 | entry #51 | 42000-42019 | 15% | 20/20 completed | tuning capture |
| Dire-Wolf | #34 | entry #51 | 43000-43019 | 60% | 20/20 completed | tuning capture |
| Dire-Bear | #49 | entry #51 | 44000-44019 | 90% | 20/20 completed | tuning capture |
| Imp | #62 | entry #51 | 45000-45019 | 0% | 20/20 completed | tuning capture |
| Werewolf | #73 | entry #51 | 46000-46019 | 35% | 18 completed, 2 event limits | `150586f9-1cfc-4a7c-8e87-707d22a47a6f` |
| Elohim, final quick tune | #90r6 | entry #51, ranged start | 47000-47019 | 70% | 20/20 completed, no runtime errors | `791b478b-4555-44cf-875e-2f6753c08a91` |

Earlier repeats of the Elohim block exposed and then proved fixes for failed-roll non-finite screech damage, incorrect screech stun evaluation, unbounded ally-inclusive target selection, stale donor accuracy/weighting, and a fixture whose explicit stored attributes remained at the cloned elephant values. Its same-seed progression was 0%, 5%, 20%, 45%, 50%, 55%, 65%, and finally 70% after the fixture was reconciled to the accepted racial profile. The final seeder profile gives magical signatures automatic attack success while retaining the authored target defence where the move type supports one. Seventy percent is the honest final quick-tune result, five points below the provisional monster band; it is retained as a known sampling/tuning limitation rather than reported as accepted. The invalid Dragon party batch `fb1a0ad8-86bf-4a8d-a3e6-315d28b26524` is retained as diagnostic evidence only: preflight reported no explicit combat setting and 19 runs reached the event limit, so it is not acceptance evidence.

### Two-pass convergence

The final in-place Mythical-then-Supernatural pass was immediately repeated. Canonical, ID-ordered full-row SHA-256 fingerprints were identical after both passes:

| Managed table | Rows | SHA-256 |
| --- | ---: | --- |
| `Races` | 293 | `3758C4F4B09A0B3A47B12E3FF49D62806E71C02A408A9BA1DEC2A2F81D84EEF0` |
| `Races_Attributes` | 1,746 | `003F016B180CF597F102F292D135EC6C2F611102AB59BC58E390B9825EB46E42` |
| `BodypartProto` | 1,965 | `E93B463521AB52D9E78C6DF368E6261E6870924C31F0B756D6CF456E41973DF9` |
| `WeaponAttacks` | 955 | `2EC88BFEAE74FF5EC4E3744752E03C23A0F0362F6BD51CEC4A738D3D89845ED8` |
| `Races_WeaponAttacks` | 2,445 | `D40C39625427B1A4D13CFA513DE7256D2A474B36E4317770EDFB056EE275B7E2` |
| `TraitExpression` | 468 | `7A68992F0099610BCFB8EA1CB47A8AD20A6D50B84EB30B77866A9F828AA16B24` |

There were no duplicate race, body-prototype, merit or combat-setting names after convergence. The database contains 24 pre-existing duplicate weapon-attack name groups outside this pass's owned-name set; stock-owned attack uniqueness and custom-link preservation are therefore enforced by the focused source/reconciliation tests rather than by an unsafe global delete or rename.

Final batch rows record the fixture IDs, seed ranges, win rates, stop reasons, duration, timeout count and deterministic fingerprint replay. A missing row is not an accepted benchmark. The representative batches above prove the tuning direction and runtime paths; they do not claim the full three-block acceptance matrix for every catalogue race.

## Known boundaries

Petrification, spell lists, vampire feeding, lich phylacteries, possession, resurrection and lunar transformation are not inferred by combat simulation. The stock Cockatrice and similar creatures are balanced only around actions they actually possess. No room-to-room charge displacement, dragon age catalogue or bespoke magic subsystem is introduced by this pass.
