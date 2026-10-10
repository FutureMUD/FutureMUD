# Emotional runtime integration allocation

Historical preparation/planning record. Its allocation/refusal statements describe that checkpoint. Current paid runtime, factory registration and optional installer mappings are documented in [Fury/Calm runtime](Armageddon_Fury_Calm_Runtime.md) and the [2026-10-08 continuation](Armageddon_Continuation_20261008.md). Native acceptance remains unrun; old receipts are preserved as history.

Planning packet only. Luke approved Fury's attribute bonus using the existing seeder inference, owned by the installer. Stock owns the reviewed Fury/Calm effects and lifetime. Calm breaks on an admitted incoming hostile attack, including a miss. MAIN owns the narrow attack-admission notification and cessation of the exact hostile pair. This checkpoint changes no emotional runtime hooks.

The proposed notification takes the exact physical attacker, intended character recipient and an identity for the admitted attack operation. Emit it once after the operation's final eligibility/authority check and before hit, miss or damage resolution. Notify only recipient effects that opt into this contract. Recheck authority and the captured physical identities after effect callbacks; preserve independent defender attacks. An attack refused before admission emits nothing. Each admitted child of a multi-target action qualifies separately. Direct component attacks need an explicit operation identity and admission receipt; a move's target enumeration alone is insufficient.

| Callsite | MAIN integration boundary |
| --- | --- |
| CombatBase.cs381–420 | Shared membership/authority and defender-response routing. Individual moves still refuse after this point, so an unconditional notification here is premature. |
| MeleeWeaponAttack.cs89–113; NaturalAttackMove.cs78–98 | After vehicle/authority admission, before the attack check. Include failed native rolls. |
| NaturalRangedAttackMoveBase.cs195–233 | After target/range admission for the actual character recipient, before its attack check. |
| MagicPowerAttackMove.cs46–62 | After target/duplicate/CanInvokePower admission, before committed payment/use/check; subsequent misses still break Calm. |
| RangedWeaponAttackBase.cs130–178/315–318; FirearmBaseGameItemComponent.cs460–493 | The move's early check is not final component admission. Empty guns return without shooting. The actual chambered-shot authority/commit/detach boundary is the representative firearm seam; preserve ResolveIndependent countershots. A failed ranged roll still fires. |
| MultiTargetCombatMove.cs59–74 | Admit and notify each actual child; do not notify all wrapper targets in advance. |

The cessation helper takes the exact physical pair and expected combat. Clear only their mutual hostility, with checks around callback-bearing setters/removal. Removing the calmed victim can already retarget its former opponent: re-read that opponent's current combat and target after removal. Leave a third-party target or callback-created replacement combat intact. Do not use blanket EndCombat/truce. PerceiverItem.CombatTarget removes target-change effects before writing; its Combat setter calls OnLeaveCombat before replacing the field. MAIN must guard the exact state at those commits rather than overwriting a callback's replacement state. SimpleMeleeCombat.LeaveCombat66–72 and CharacterCombat.CheckCombatStatus288–300 reacquire targets; ProgCombat.LeaveCombat63–88 also runs a prog.

Required bounded MAIN acceptance: admitted miss versus refused attack; notification callback revoking/retiring the attacker; independent defender attack; two-person cessation; third opponent; victim-removal callback retargeting the opponent; callback-created replacement combat. Stock separately proves actual paid Calm removal of only its owned effect and ordinary lifetime/save/reload. Existing PsychicEmotionEffect peacefulness is unchanged. OnWounded cannot qualify misses. Dedicated attack variants and direct spell/component routes need their own integration receipts before universal coverage is claimed.

Read-only mapping fingerprints (SHA-256, source coordinator owns subsequent qualification):

```text
MudSharpCore/Combat/CombatBase.cs cc89df99600502d0b715117face9c18a4f1b52d226006bb943fecd4a973e28dc
MudSharpCore/Combat/SimpleMeleeCombat.cs 576f65a25fc83bb8595a5c8d8e1c8b246250fb09a70e4e7f3d7ff3679ada10cc
MudSharpCore/Combat/ProgCombat.cs 563e70baed3675cee0a25be91faa3957cb80103e89518f37aa0177e3f00796d5
MudSharpCore/Character/CharacterCombat.cs 135a311b88ab8029ce9e13bf29ff043cc72fad49e5da96ed5eff70b1f12e6299
MudSharpCore/Framework/PerceiverItem.cs 1a8a3b28a26005eabedf9cd6ba7f1d259fb06a61a391166cc04b6d730e10b6c5
MudSharpCore/Combat/Moves/CombatMoveBase.cs 9f549c234f4b9e034897fe41124643cfa05add2e33cf7ae6c7b2d60a0546a7b6
MudSharpCore/Combat/Moves/MeleeWeaponAttack.cs 8b7e0155149d40b5f1ae52f6ad9e73042bc742d465070921fd14ccb81457be9a
MudSharpCore/Combat/Moves/NaturalAttackMove.cs 2b2a13b86a950d67a1a1cfe41e03ae2731e8439f3b3d4f4156f114546911a0e3
MudSharpCore/Combat/Moves/NaturalRangedAttackMoveBase.cs 5959b25652b906cb6127bf02032e2fdcf825abf8a5d885989c7733fe893bca23
MudSharpCore/Combat/Moves/RangedWeaponAttackBase.cs 0bfa88228da8af575200777834acefc247aa377b630386a81d75d7c09e693f3f
MudSharpCore/Combat/Moves/MagicPowerAttackMove.cs a6c56f13af8efbb472bfc42fa108eb8f0b463a48d1845f4877f5df95bbdcb813
MudSharpCore/Combat/Moves/MultiTargetCombatMove.cs 9464a2749878b05d8e0f45865f8d577235a236c86f3e7e3358b8c31b3170a05f
MudSharpCore/GameItems/Components/FirearmBaseGameItemComponent.cs 6e3d34e0578ee87a80b843fc9e6d2b2a6bc7a4d695ad8937cf7a93b9671639e0
MudSharpCore/Effects/Concrete/PsychicEmotionEffect.cs 6c1da57fe2f612526999fdc6399379d1bb98b3f544995e9532e81d56f7b3cfb3
```


## Implementation follow-up

The planning evidence above is retained. The shared runtime allocation is now implemented in the [emotional combat hooks dependency checkpoint](Armageddon_Emotional_Combat_Hooks.md), with bounded managed/native qualification and explicit unsupported caller scope. Actual stock emotional activation remains pending.
