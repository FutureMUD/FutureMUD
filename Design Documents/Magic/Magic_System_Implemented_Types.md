# FutureMUD Magic System: Implemented Types

## Purpose
This document is the compact inventory of the currently implemented magic type surface.

It is intended for:

- agents that need a quick catalogue without reading the longer narrative docs
- developers checking whether a type already exists before adding one
- builders looking for valid builder tokens
- seeder authors checking which runtime token or model token a record should use

## Quick Map
- Read [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md) for the narrative explanation of schools, capabilities, resources, and regenerators.
- Read [Magic System: Powers](./Magic_System_Powers.md) for the narrative explanation of powers and their builder/runtime flow.
- Read [Magic System: Spells](./Magic_System_Spells.md) for the narrative explanation of spell triggers and spell effects.

## Capability Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `skilllevel` | `SkillLevelBasedMagicCapability` | Capability | Static `RegisterLoader` in `MudSharpCore/Magic/Capabilities/SkillLevelBasedMagicCapability.cs` via `MagicCapabilityFactory` | Yes | Grants school access, concentration rules, regenerators, and inherent powers based on trait thresholds |

## Resource Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `simple` | `SimpleMagicResource` | Resource | Switch dispatch in `MudSharpCore/Magic/Resources/BaseMagicResource.cs` | Yes | Prog-driven magic resource with holder-specific start rules, caps, and prompt colours |

## Regenerator Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `linear` | `LinearTimeBasedGenerator` | Regenerator | Switch dispatch in `MudSharpCore/Magic/Generators/BaseMagicResourceGenerator.cs` | Yes | Adds a fixed amount of one resource per minute |
| `state` | `StateGenerator` | Regenerator | Switch dispatch in `MudSharpCore/Magic/Generators/BaseMagicResourceGenerator.cs` | Yes | Adds one or more resources per minute based on boolean state progs |

## FutureProg Magic Helpers

| Function or property | Subsystem | Purpose |
| --- | --- | --- |
| `magicresourcelevel` | Resources | Reads a character, item, or room magic resource level by resource name/id or numeric id |
| `setmagicresource`, `setmagicresourcelevel` | Resources | Sets a character, item, or room magic resource level and returns the clamped result |
| `addmagicresource`, `addmagicresourcelevel` | Resources | Adds to a character, item, or room magic resource level and returns the clamped result |
| `subtractmagicresource`, `subtractmagicresourcelevel` | Resources | Subtracts from a character, item, or room magic resource level and returns the clamped result |
| `magiccapabilities(character)`, `character.magiccapabilities` | Capabilities | Returns the character's currently applicable magic capabilities |
| `knownspells(character)`, `character.knownspells` | Spells | Returns spells whose known-spell prog says the character knows them |
| `castablespells(character)`, `character.castablespells` | Spells | Returns ready, known spells that use a cast trigger |
| `castablespellsnow(character)`, `character.castablespellsnow` | Spells | Returns ready, known cast-trigger spells currently castable against the character at some permitted power |
| `cancastspell`, `cancastspellnow` | Spells | Checks general spell availability or current lockout/resource/material preconditions without performing the cast |
| `activespells`, `activespelleffects` | Spell effects | Reads retained `MagicSpellParent` state on a character |
| `spellremainingduration`, `spellduration`, `setspellduration`, `addspellduration`, `subtractspellduration`, `removespell` | Spell effects | Reads, changes, or removes active spell-parent effects for a specific spell on a character |

## Power Types
Current count: 32 power tokens, including 31 builder-creatable tokens and the non-builder `armor` runtime alias. V4 added 9 builder-creatable psionic powers, and the Old SOI parity slice added 7 more psionic power tokens in per-power files under `MudSharpCore/Magic/Powers/`.

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `armour` | `MagicArmourPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicArmourPower.cs` via `MagicPowerFactory` | Yes | Applies magical armour behavior |
| `armor` | `MagicArmourPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicArmourPower.cs` via `MagicPowerFactory` | No | Runtime compatibility alias for the armour power model |
| `anesthesia` | `MindAnesthesiaPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindAnesthesiaPower.cs` via `MagicPowerFactory` | Yes | Applies mental anesthesia behavior |
| `allspeak` | `AllspeakPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/AllspeakPower.cs` via `MagicPowerFactory` | Yes | Sustains spoken-language comprehension without adding permanent language knowledge or literacy |
| `babble` | `BabblePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/BabblePower.cs` via `MagicPowerFactory` | Yes | Applies hostile timed speech obfuscation before language comprehension can decode speech |
| `choke` | `ChokePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ChokePower.cs` via `MagicPowerFactory` | Yes | Applies choking or constriction behavior |
| `clairaudience` | `ClairaudiencePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ClairaudiencePower.cs` via `MagicPowerFactory` | Yes | Sustains contact-based remote hearing through another mind's location |
| `clairvoyance` | `ClairvoyancePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ClairvoyancePower.cs` via `MagicPowerFactory` | Yes | Renders a target's current location as remote LOOK output without moving the caster |
| `coerce` | `CoercePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/CoercePower.cs` via `MagicPowerFactory` | Yes | Applies mode-based psionic influence for stamina, hunger, thirst, or thought injection |
| `connectmind` | `ConnectMindPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ConnectMindPower.cs` via `MagicPowerFactory` | Yes | Creates or manages a mind connection |
| `dangersense` | `DangerSensePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/DangerSensePower.cs` via `MagicPowerFactory` | Yes | Sustains nearby hostile NPC sensing and a defensive combat bonus effect |
| `empathy` | `EmpathyPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/EmpathyPower.cs` via `MagicPowerFactory` | Yes | Transfers wounds from a target to the psion one at a time with bodypart remapping and safety stops |
| `hear` | `HearPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/HearPower.cs` via `MagicPowerFactory` | Yes | Sustains a listener for psionic thought and feeling traffic |
| `hex` | `HexPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/HexPower.cs` via `MagicPowerFactory` | Yes | Applies a timed hostile check penalty over selected check categories |
| `invisibility` | `InvisibilityPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/InvisibilityPower.cs` via `MagicPowerFactory` | Yes | Applies invisibility behavior |
| `magicattack` | `MagicAttackPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicAttackPower.cs` via `MagicPowerFactory` | Yes | Executes a direct magical attack |
| `magicksense` | `MagicksensePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicksensePower.cs` via `MagicPowerFactory` | Yes | Sustains `SenseMagical` perception and uses the existing magical aura display |
| `mindaudit` | `MindAuditPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindAuditPower.cs` via `MagicPowerFactory` | Yes | Audits or inspects a mind |
| `mindbarrier` | `MindBarrierPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindBarrierPower.cs` via `MagicPowerFactory` | Yes | Creates a mental barrier |
| `mindbroadcast` | `MindBroadcastPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindBroadcastPower.cs` via `MagicPowerFactory` | Yes | Broadcasts mind-to-mind communication |
| `mindconceal` | `MindConcealPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindConcealPower.cs` via `MagicPowerFactory` | Yes | Sustains a concealed mind-contact identity and audit difficulty modifier |
| `mindexpel` | `MindExpelPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindExpelPower.cs` via `MagicPowerFactory` | Yes | Expels a connection or presence |
| `mindlook` | `MindLookPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindLookPower.cs` via `MagicPowerFactory` | Yes | Observes through mind mechanics |
| `mindsay` | `MindSayPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindSayPower.cs` via `MagicPowerFactory` | Yes | Sends directed mind speech |
| `prescience` | `PresciencePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/PresciencePower.cs` via `MagicPowerFactory` | Yes | Opens an editor and posts the submitted question to a configured board |
| `projectemotion` | `ProjectEmotionPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ProjectEmotionPower.cs` via `MagicPowerFactory` | Yes | Injects an involuntary feeling into a target mind and eligible psionic listeners |
| `psychicbolt` | `PsychicBoltPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/PsychicBoltPower.cs` via `MagicPowerFactory` | Yes | Applies stun-only psychic damage through the normal health pipeline |
| `sense` | `SensePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/SensePower.cs` via `MagicPowerFactory` | Yes | Senses characters or items across a configured range |
| `sensitivity` | `SensitivityPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/SensitivityPower.cs` via `MagicPowerFactory` | Yes | Sustains magical or psychic perception, receives activity pings, and actively scans auras or capabilities |
| `suggest` | `SuggestPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/SuggestPower.cs` via `MagicPowerFactory` | Yes | Injects an involuntary thought, optionally wrapped with an emotional delivery |
| `telepathy` | `TelepathyPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/TelepathyPower.cs` via `MagicPowerFactory` | Yes | Telepathic communication or related perception |
| `trace` | `TracePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/TracePower.cs` via `MagicPowerFactory` | Yes | Inspects active mind links around a target mind while respecting concealment difficulty |
| n/a | `MagicPowerBase` | Power support | Shared base in `MudSharpCore/Magic/Powers/MagicPowerBase.cs` | No | Shared costs, progs, help text, crime handling, and builder support |
| n/a | `SustainedMagicPower` | Power support | Shared base in `MudSharpCore/Magic/Powers/SustainedMagicPower.cs` | No | Shared support for sustained powers |
| n/a | `MagicalMeleeAttackPower` | Power support | Shared base in `MudSharpCore/Magic/Powers/MagicalMeleeAttackPower.cs` | No | Shared support base for melee-style magical attacks |

## Spell Trigger Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `character` | `CastingTriggerCharacter` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacter.cs` via `SpellTriggerFactory` | Yes | Casts at a character target in the same room |
| `characterexit` | `CastingTriggerCharacterExit` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacterExit.cs` via `SpellTriggerFactory` | Yes | Casts at a same-room character plus a local exit, exposing the chosen `ICellExit` as an additional `exit` parameter |
| `characterprogroom` | `CastingTriggerCharacterProgRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacterProgRoom.cs` via `SpellTriggerFactory` | Yes | Casts at a character with prog-driven room targeting |
| `charactervicinity` | `CastingTriggerCharacterVicinity` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacterVicinity.cs` via `SpellTriggerFactory` | Yes | Casts at characters in a character's vicinity |
| `corpse` | `CastingTriggerCorpse` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCorpse.cs` via `SpellTriggerFactory` | Yes | Casts at a corpse target |
| `exit` | `CastingTriggerExit` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerExit.cs` via `SpellTriggerFactory` | Yes | Casts at a local exit or direction, targeting the shared `IExit` while also supplying the chosen `ICellExit` as an additional `exit` parameter |
| `item` | `CastingTriggerItem` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerItem.cs` via `SpellTriggerFactory` | Yes | Casts at an item target |
| `localitem` | `CastingTriggerLocalItem` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerLocalItem.cs` via `SpellTriggerFactory` | Yes | Casts at a local item target |
| `party` | `CastingTriggerParty` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerParty.cs` via `SpellTriggerFactory` | Yes | Casts across party members |
| `progcharacter` | `CastingTriggerProgCharacter` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgCharacter.cs` via `SpellTriggerFactory` | Yes | Casts at a world character target resolved by a FutureProg |
| `progcharacterroom` | `CastingTriggerProgCharacterRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgCharacterRoom.cs` via `SpellTriggerFactory` | Yes | Casts at a prog-resolved character and also supplies a prog-resolved room as an additional `room` parameter |
| `progitem` | `CastingTriggerProgItem` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgItem.cs` via `SpellTriggerFactory` | Yes | Casts at a world item target resolved by a FutureProg |
| `progitemroom` | `CastingTriggerProgItemRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgItemRoom.cs` via `SpellTriggerFactory` | Yes | Casts at a prog-resolved item and also supplies a prog-resolved room as an additional `room` parameter |
| `progroom` | `CastingTriggerProgRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgRoom.cs` via `SpellTriggerFactory` | Yes | Casts using a prog-driven room rule |
| `room` | `CastingTriggerRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerRoom.cs` via `SpellTriggerFactory` | Yes | Casts at the room or cell |
| `self` | `CastingTriggerSelf` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerSelf.cs` via `SpellTriggerFactory` | Yes | Casts on the caster |
| `vicinity` | `CastingTriggerVicinity` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerVicinity.cs` via `SpellTriggerFactory` | Yes | Casts across a vicinity target set |

## Spell Effect Types
V4 added 2 builder-creatable tag-aware ward effect tokens: `roomtagward` and `personaltagward`.

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `blindness` | `BlindnessEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs` via `SpellEffectFactory` | Yes | Applies blindness |
| `boost` | `TraitBoostEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TraitBoostEffect.cs` via `SpellEffectFactory` | Yes | Boosts a trait |
| `changecharacteristic` | `ChangeCharacteristicEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ChangeCharacteristicEffect.cs` via `SpellEffectFactory` | Yes | Changes a characteristic |
| `comprehendlanguage` | `ComprehendLanguageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants broad spoken and written language comprehension without overriding illiteracy or unknown-script checks |
| `createitem` | `CreateItemEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/CreateItemEffect.cs` via `SpellEffectFactory` | Yes | Creates an item |
| `createliquid` | `CreateLiquidEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/CreateLiquidEffect.cs` via `SpellEffectFactory` | Yes | Creates a liquid |
| `createnpc` | `CreateNPCEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/CreateNPCEffect.cs` via `SpellEffectFactory` | Yes | Creates an NPC |
| `curse` | `CurseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies a spell-owned curse tag that can be queried and dispelled |
| `cureblindness` | `RemoveBlindnessEffect` | Spell effect alias | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs` via `SpellEffectFactory` | Yes | Builder/load alias for `removeblindness` |
| `damage` | `DamageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DamageEffect.cs` via `SpellEffectFactory` | Yes | Deals damage |
| `deafness` | `DeafnessEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs` via `SpellEffectFactory` | Yes | Applies deafness |
| `detectethereal` | `DetectEtherealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants ethereal visual and sensing perception channels |
| `detectinvisible` | `DetectInvisibleEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants magical vision that can pierce ordinary invisibility |
| `detectmagick` | `DetectMagickEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants magical sensing and exposes detectable magical auras in normal descriptions |
| `detectpoison` | `DetectPoisonEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DetectPoisonEffect.cs` via `SpellEffectFactory` | Yes | Reports active and latent drug dosages on a character |
| `disease` | `DiseaseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Applies a configurable spell-owned systemic infection payload |
| `dispelinvisibility` | `RemoveInvisibilityEffect` | Spell effect alias | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Builder/load alias for `removeinvisibility` |
| `dispelmagic` | `DispelMagicEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DispelMagicEffect.cs` via `SpellEffectFactory` | Yes | Removes or shortens matching spell-parent effects by caster policy, spell, school/subschool, tag, illusion key, approved effect key, and optional strength contest |
| `destroyitem` | `DestroyItemEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Deletes item targets with purge-warning safeguards |
| `executeprog` | `ExecuteProgEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ExecuteProgEffect.cs` via `SpellEffectFactory` | Yes | Executes a supporting prog |
| `exitbarrier` | `ExitBarrierEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ExitBarrierEffect.cs` via `SpellEffectFactory` | Yes | Applies a persistent magical barrier to a targeted shared `IExit` so crossing that exit can be blocked |
| `featherfall` | `FeatherFallEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies configurable fall-distance and fall-damage mitigation |
| `forcedexitmovement` | `ForcedExitMovementEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ForcedExitMovementEffect.cs` via `SpellEffectFactory` | Yes | Forces a targeted character through the trigger-supplied `exit` when movement and crossing checks allow it |
| `forcedpathmovement` | `ForcedPathMovementEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Forces a targeted character through one or more same-direction exits when movement and crossing checks allow it |
| `fear` | `FearEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies magical fear that forces flee behaviour in combat |
| `flying` | `FlyingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants the ability to use existing flight movement checks |
| `glow` | `GlowEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/GlowEffect.cs` via `SpellEffectFactory` | Yes | Applies glow |
| `handsofwind` | `ForcedPathMovementEffect` | Spell effect alias | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Builder/load alias for `forcedpathmovement` |
| `heal` | `HealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/HealEffect.cs` via `SpellEffectFactory` | Yes | Heals damage |
| `healingrate` | `HealingRateSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/HealingRateSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters healing rate |
| `infravision` | `InfravisionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants infrared vision and a darkness difficulty floor |
| `insomnia` | `InsomniaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Prevents voluntary and magical sleep |
| `invisibility` | `InvisibilityEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs` via `SpellEffectFactory` | Yes | Applies invisibility |
| `itemdamage` | `ItemDamageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Applies ordinary item damage using configured damage, pain, stun, and damage-type formulas |
| `itemenchant` | `ItemEnchantEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Adds item aura text, glow, weapon/armour hooks, projectile bonuses, craft-tool bonuses, power/fuel modifiers, and optional item event progs |
| `levitate` | `LevitationEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Suspends a character or item, optionally moves it to a configured room layer, and prevents falling while active |
| `magictag` | `MagicTagEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Adds spell-owned key/value metadata for marks, anchors, runes, signatures, FutureProg queries, and tag-aware interdiction |
| `magicresourcedelta` | `MagicResourceDeltaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs` via `SpellEffectFactory` | Yes | Adds or removes a configured magic resource from a character, item, or room |
| `mend` | `MendEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MendEffect.cs` via `SpellEffectFactory` | Yes | Mends damage or wear |
| `needdelta` | `NeedDeltaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs` via `SpellEffectFactory` | Yes | Changes a need immediately |
| `needrate` | `NeedRateSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/NeedRateSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters need rate |
| `pacifism` | `PacifismSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/PacifismSpellEffect.cs` via `SpellEffectFactory` | Yes | Applies pacifism |
| `personalward` | `PersonalWardEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/PersonalWardEffect.cs` via `SpellEffectFactory` | Yes | Applies a school-based personal ward that can fail or reflect matching incoming or outgoing magic |
| `personaltagward` | `PersonalTagWardEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies a personal ward that fails or reflects matching incoming or outgoing magic by `magictag` key/value |
| `paralysis` | `ParalysisEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies forced paralysis through the health effect system |
| `poison` | `PoisonEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Applies a configurable spell-owned drug payload |
| `portal` | `PortalSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Creates effect-owned paired transient exits between the caster's room and a target room, room anchor, or item/object anchor |
| `rage` | `RageSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RageSpellEffect.cs` via `SpellEffectFactory` | Yes | Applies rage |
| `relocate` | `RelocateEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RelocateEffect.cs` via `SpellEffectFactory` | Yes | Relocates a target |
| `removeblindness` | `RemoveBlindnessEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/BlindnessEffect.cs` via `SpellEffectFactory` | Yes | Removes spell-owned blindness effects |
| `removecomprehendlanguage` | `RemoveComprehendLanguageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical language-comprehension effects |
| `removecurse` | `RemoveCurseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes matching magical curse effects |
| `removedetectethereal` | `RemoveDetectEtherealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical ethereal perception effects |
| `removedetectinvisible` | `RemoveDetectInvisibleEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical detect-invisible effects |
| `removedetectmagick` | `RemoveDetectMagickEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical detect-magick effects |
| `removedisease` | `RemoveDiseaseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Removes a matching spell-owned disease payload |
| `removefear` | `RemoveFearEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical fear effects |
| `removeflying` | `RemoveFlyingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical flight-granting effects |
| `removeinvisibility` | `RemoveInvisibilityEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes spell-owned invisibility effects without removing unrelated spell effects |
| `removeinfravision` | `RemoveInfravisionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical infravision effects |
| `removeinsomnia` | `RemoveInsomniaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical insomnia effects |
| `removemagictag` | `RemoveMagicTagEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Removes matching spell-owned magic tags by key and optional value |
| `removeparalysis` | `RemoveParalysisEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical paralysis effects |
| `removepoison` | `RemovePoisonEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Removes a matching spell-owned poison payload |
| `removeroomflag` | `RemoveRoomFlagEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomFlagEffect.cs` via `SpellEffectFactory` | Yes | Removes a configured magical room flag |
| `removesilence` | `RemoveSilenceEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical silence effects |
| `removesleep` | `RemoveSleepEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical sleep effects |
| `removewaterbreathing` | `RemoveWaterBreathingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical water-breathing effects |
| `resurrect` | `ResurrectionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ResurrectionEffect.cs` via `SpellEffectFactory` | Yes | Resurrects a target |
| `roomflag` | `RoomFlagEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomFlagEffect.cs` via `SpellEffectFactory` | Yes | Applies a configured magical room flag such as peaceful, no-dream, alarm, darkness, or ward tags |
| `roomatmosphere` | `RoomAtmosphereEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomAtmosphereEffect.cs` via `SpellEffectFactory` | Yes | Alters room atmosphere |
| `roomlight` | `RoomLightEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomLightEffect.cs` via `SpellEffectFactory` | Yes | Alters room light |
| `roomward` | `RoomWardEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomWardEffect.cs` via `SpellEffectFactory` | Yes | Applies a school-based room ward that can fail or reflect matching incoming or outgoing magic |
| `roomtagward` | `RoomTagWardEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TagWardSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies a room ward that fails or reflects matching incoming or outgoing magic by `magictag` key/value |
| `roomtemperature` | `RoomTemperatureEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RoomTemperatureEffect.cs` via `SpellEffectFactory` | Yes | Alters room temperature |
| `selfdamage` | `SelfDamageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/SelfDamageEffect.cs` via `SpellEffectFactory` | Yes | Damages the caster |
| `silence` | `SilenceEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies vocal silence without blocking telepathy |
| `sleep` | `SleepEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Forces magical sleep and keeps the target asleep until the last such effect ends |
| `spellarmour` | `SpellArmourEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/SpellArmourEffect.cs` via `SpellEffectFactory` | Yes | Applies spell-owned magical armour using the shared armour configuration stack |
| `staminadelta` | `StaminaDeltaSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StaminaDeltaSpellEffect.cs` via `SpellEffectFactory` | Yes | Changes stamina immediately |
| `staminaexpendrate` | `StaminaExpenditureSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StaminaExpenditureSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters stamina expenditure rate |
| `staminaregenrate` | `StaminaRegenRateSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StaminaRegenRateSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters stamina regeneration rate |
| `telepathy` | `TelepathySpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TelepathySpellEffect.cs` via `SpellEffectFactory` | Yes | Applies telepathic linkage |
| `teleport` | `TeleportEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TeleportEffect.cs` via `SpellEffectFactory` | Yes | Teleports the caster to a room or cell target |
| `teleporttarget` | `TeleportTargetEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TeleportTargetEffect.cs` via `SpellEffectFactory` | Yes | Teleports a target selected by the spell |
| `subjectivedesc` | `SubjectiveDescriptionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Adds caster-scoped subjective full-description replacement with priority and optional illusion key |
| `subjectivesdesc` | `SubjectiveSDescEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicPhase3Effects.cs` via `SpellEffectFactory` | Yes | Adds caster-scoped subjective short-description replacement with priority and optional illusion key |
| `transference` | `TransferenceEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WindSpellEffects.cs` via `SpellEffectFactory` | Yes | Swaps caster and target character locations, optionally including followers and room layers |
| `transformform` | `TransformFormEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/TransformFormEffect.cs` via `SpellEffectFactory` | Yes | Ensures or reuses a keyed alternate body form, applies first-creation defaults such as description patterns and transformation echo, contributes a priority-ranked forced transformation demand, and reuses the shared baseline-form revert path when the demand ends |
| `waterbreathing` | `WaterBreathingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants additional breathable fluids for the target |
| `weatherchange` | `WeatherChangeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherChangeEffect.cs` via `SpellEffectFactory` | Yes | Changes weather |
| `weatherchangefreeze` | `WeatherChangeFreezeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherChangeFreezeEffect.cs` via `SpellEffectFactory` | Yes | Changes and freezes weather state |
| `weatherfreeze` | `WeatherFreezeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherFreezeEffect.cs` via `SpellEffectFactory` | Yes | Freezes weather state |
| `weight` | `WeightSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeightSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters weight |

## Interdiction Support Types

| Token or API | Class or interface | Role |
| --- | --- | --- |
| `incoming`, `outgoing`, `both` | `MagicInterdictionCoverage` | Ward coverage enum that decides whether an interdiction catches incoming magic, outgoing magic, or both |
| `fail`, `reflect` | `MagicInterdictionMode` | Ward mode enum that decides whether a matching invocation fizzles or reflects where the runtime supports reflection |
| n/a | `IMagicInterdictionEffect` | Shared runtime contract used by room and personal wards so spells and powers can consult the same interception rules |
| n/a | `IMagicContextualInterdictionEffect` | Optional contextual interdiction contract that can inspect source, target, school, direction, and invocation tags |
| n/a | `MagicInterdictionContext` | Shared context object passed to contextual interdiction effects |
| n/a | `MagicInterdictionTag` | Key/value metadata pair used by tag-aware wards |
| n/a | `IMagicInterdictionTagProvider` | Template-side contract for spell effects that contribute contextual interdiction tags |
| n/a | `IExitBarrierEffect` | Shared runtime contract used by movement logic so magical exit barriers can block `Character.CanCross` |

## Engine V2 Support Types

| Token or API | Class or interface | Role |
| --- | --- | --- |
| `IExitManager.TransientExits` | `IEnumerable<IExit>` | Read-only enumeration of registered transient exits for builder/admin inspection |
| n/a | `IMagicPortalExit` | Optional metadata contract implemented by transient magical portals for source, destination, caster, spell, source effect, and portal command text |
| n/a | `IMagicProjectilePayloadEffect` | First-class projectile/ranged payload enhancement contract used by ammunition, power packs, and thrown weapons |
| n/a | `IMagicCraftToolEnhancementEffect` | First-class craft-tool enhancement contract for tool fitness, phase speed, and tool usage multipliers |
| n/a | `IMagicPowerOrFuelEnhancementEffect` | First-class powered-item enhancement contract for production, consumption, and fuel-use multipliers |
| n/a | `IMagicItemEventEffect` | First-class item event callback contract for enchanted items |
| n/a | `IMindContactConcealmentEffect` | Shared psionic identity-concealment contract used by mind links, mind speech, broadcasts, audits, expulsion text, and passive telepathic traffic |
| n/a | `IPreventFallingEffect` | Shared fall-prevention contract used by levitation and future suspension effects |
| n/a | `ILevitationEffect` | Spell-owned levitation marker for target-specific suspension |
| n/a | `IFallDamageMitigationEffect` | Shared fall mitigation contract exposing fall-distance and fall-damage multipliers |

## Engine V4 Support Types

| Token or API | Class or interface | Role |
| --- | --- | --- |
| n/a | `PsionicTrafficHelper` | Shared helper for involuntary psionic thought/feeling delivery, eligible listener forwarding, opt-out checks, blocked command roots, source/target messaging, and wiz-audit output |
| n/a | `IPrioritisedOverrideDescEffect` | Subjective-description priority and illusion-key contract used by body/item description override resolution and `dispelmagic illusion <key>` |
| n/a | `IBabbleSpeechEffect` | Speech-obfuscation contract used by communication strategies before language comprehension |
| n/a | `PsionicSustainedPowerEffectBase<TPower>` | Shared base for V4 sustained psionic effects |
| n/a | `BodypartMappingUtilities` | Shared bodypart mapping helper used by body-form wound migration and `empathy` wound transfer |
| n/a | `PsionicActivityNotifier` | Shared activity ping helper used by new psionic powers and consumed by `sensitivity` |
| n/a | `RemoteLookRenderer` | LOOK-style remote cell renderer used by `clairvoyance` |
| n/a | `MagicAllspeakEffect` | Sustained `IComprehendLanguageEffect` used by `allspeak` |
| n/a | `MagicMagicksenseEffect` | Sustained `SenseMagical` perception grant used by `magicksense` |
| n/a | `PsionicHearEffect` | Sustained `ITelepathyEffect` listener used by `hear` |
| n/a | `PsionicBabbleEffect` | Timed `IBabbleSpeechEffect` used by `babble` |
| n/a | `MagicClairaudienceConcentrationEffect` | Caster-side concentration marker for `clairaudience` |
| n/a | `PsionicClairaudienceEffect` | Remote-observation effect that forwards audible output only |
| n/a | `MagicDangerSenseEffect` | Sustained heartbeat effect that scans for danger and refreshes defensive edge bonuses |
| n/a | `DangerSenseDefensiveEdge` | Temporary defensive check bonus effect used by `dangersense` |
| n/a | `MagicHexEffect` | Saving check-penalty effect used by `hex` |
| n/a | `PsionicSensitivityEffect` | Sustained perception grant and activity listener used by `sensitivity` |

## Notes
- Schools are first-class records rather than subtype-driven types, so they are documented in the overview and backbone docs rather than listed here as a type family.
- Powers, triggers, and spell effects are all current-state inventories of registered runtime implementations.
- Builder-creatable means the inspected builder workflow can create an instance of the type directly, not merely load or edit one.
- Trigger-supplied additional parameters currently include `exit` for local-exit casts and `room` for prog-resolved remote-room casts.
- Phase 1 spell support also introduced additive `IEffect.PerceptionGranting`, spell-runtime query interfaces such as `ISilencedEffect`, `ISleepEffect`, `IFearEffect`, `IFlightEffect`, `IAdditionalBreathableFluidEffect`, `IDarksightEffect`, `IComprehendLanguageEffect`, and `ICurseEffect`, plus shared `MagicArmourConfiguration` for power and spell armour parity.
- `transformform` is the current builder-creatable spell effect for cached alternate-form provisioning and scripted transformation, including optional transformation emotes, first-creation description-pattern defaults, and configurable forced-transformation priority metadata.
- Wind spell support extends the runtime movement layer with `IFly.CanContinueFlying`, target-specific fall prevention, fall-damage mitigation, exit-path forced movement, precise invisibility removal, and caster/target transference.
- V4 psionic and perception support adds 9 power tokens, tag-aware wards, contextual interdiction tags, illusion priority/key matching, speech babbling, remote audible observation, and non-command psionic traffic/coercion delivery. The Old SOI parity slice adds 7 more psionic power tokens plus seeded variable checks for danger sense, empathy, hex, clairvoyance, prescience, sensitivity, and psychic bolt.
