# Kindle / Empower source and adopted native mapping — 15 October 2026

Luke approved replacing the weapon-only Kindle adaptation with an Empower-style item enchantment granting a selected attribute or resource-capacity bonus. Combat benefits flow through the selected native attribute. This supersedes both the conflicting weapon-damage wording and the flat attack-bonus implementation. Capacity increases do not refill reserves; there is no casting-permission option.

## Source identity and combat trace

`Armageddon_Repertoire.json` `/entries/7` and `Armageddon_Seeder_Completeness_Design_20261011.json` ordinal8 map `arm.spell.kindle_enchantment` to source skill47, Empower. The available local source is `C:/Users/luker/Downloads/codedump.c`, SHA256 `E44778F6A5FB9FD02DB5C3FF78D4C468B93AE6C854A058D5AF769DC0A95CD59F`.

The source spell name at19317, `SPELL_EMPOWER` at166258 and table dispatch174661–174663 lead to `cast_empower`218552–218735. Carried-item and modifier selection is at218600–218637; eight stat/capacity choices are at218581–218590 and mapped at218653–218684. The resolved amount/location is stored on the item at218713–218714. The separate casting-permission branch218686–218700 is excluded from this approved native change.

`equip_char`50305 applies the item's modifier through `affect_modify`, and unequip50329 removes it; `affect_total`49384–49420 recomputes equipment contributions. Strength49220–49221 and Agility49224–49225 modify native attributes. Source `GET_OFFENSE`239586–239587 includes both Strength and Agility, while `GET_DEFENSE`239589–239590 includes Agility twice. Actual combat evaluates these at12937–12938 and13385–13401. Melee damage separately adds the Strength damage-table result at13477, with two-handed adjustments13486/13494. Therefore the original selected Strength bonus can affect both accuracy and damage; there is no single direct flat weapon-damage or attack-only source bonus to copy.

## Adopted content

The source seven-grade table is retained in native units:

| Choice | Grades1–7 |
| --- | --- |
| Mana capacity | 1,2,4,6,8,10,12 |
| Health capacity | 1,1,2,2,3,6,8 |
| Stun capacity | 1,1,2,2,3,6,8 |
| Stamina capacity | 1,2,4,8,14,18,20 |
| Strength | 0,0,0,1,2,3,4 |
| Agility | 0,0,0,1,2,3,4 |
| Wisdom | 0,0,0,1,1,2,3 |
| Endurance | 0,0,1,2,3,4,5 |

Zero bonuses refuse before payment. The existing stock cost minimum33 and duration60*grade remain. Native timed expiry/dispel removes the enchantment, preserving the existing item's identity, prototype, quality, custody and foreign contents. The source's item destruction, random explosion, grade-seven permanence, non-save flag, glow/hum and casting permission are not introduced by this bounded change.

Strength reuses the ordinary retained Strength binding. Agility reuses the existing ordinary Agility/Dexterity binding. Wisdom maps explicitly to the installation's already selected native capacity attribute (which may be Wisdom or Aura). Endurance maps to one existing ordinary Constitution/Physique/Endurance/Body attribute through a retained alias; ambiguous or absent native attributes stop installation. No new stats are created or adopted.

Source health/stun are represented as native health/stun wound-tolerance thresholds, leaving all existing wound damage/stun unchanged. Source stamina raises native maximum stamina and source mana raises only the explicitly named configured character reserve. FutureMUD has distinct pools: `mana-fire`, `mana-water`, etc. name the exact installed reserve. The seven elemental pools are distinct; Sorcerer, Preserver and Defiler share the existing Sorcerer pool (`mana-sorcerer`). Options are derived from the actual progression resource keys, without inventing separate Preserver/Defiler reserves. The synthetic source-energy cost resource is never a capacity target. Raising a cap adds no current points, and lowering it uses existing native reconciliation/clamping. The equipped item's recipient does not gain a casting route, enrolment or capability from the bonus.

The player selects `<enchantment choice> <carried item>`. The item may be enchanted while carried, and contributes while actually worn or wielded. Held-only, contained, dropped, deleted or transferred items cannot contribute through a stale former-holder reference. An exclusive same-spell parent preserves native non-stacking behavior on the same item; the ordinary parent owns the saved selected bonus and remaining duration.

Verification and acceptance are recorded in the S4 checkpoint and portable verification JSON. This decision record establishes the approved contract; it does not itself claim executed qualification.
