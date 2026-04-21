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

## Power Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `armour` | `MagicArmourPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicArmourPower.cs` via `MagicPowerFactory` | Yes | Applies magical armour behavior |
| `armor` | `MagicArmourPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicArmourPower.cs` via `MagicPowerFactory` | No | Runtime compatibility alias for the armour power model |
| `anesthesia` | `MindAnesthesiaPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindAnesthesiaPower.cs` via `MagicPowerFactory` | Yes | Applies mental anesthesia behavior |
| `choke` | `ChokePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ChokePower.cs` via `MagicPowerFactory` | Yes | Applies choking or constriction behavior |
| `connectmind` | `ConnectMindPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/ConnectMindPower.cs` via `MagicPowerFactory` | Yes | Creates or manages a mind connection |
| `invisibility` | `InvisibilityPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/InvisibilityPower.cs` via `MagicPowerFactory` | Yes | Applies invisibility behavior |
| `magicattack` | `MagicAttackPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MagicAttackPower.cs` via `MagicPowerFactory` | Yes | Executes a direct magical attack |
| `mindaudit` | `MindAuditPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindAuditPower.cs` via `MagicPowerFactory` | Yes | Audits or inspects a mind |
| `mindbarrier` | `MindBarrierPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindBarrierPower.cs` via `MagicPowerFactory` | Yes | Creates a mental barrier |
| `mindbroadcast` | `MindBroadcastPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindBroadcastPower.cs` via `MagicPowerFactory` | Yes | Broadcasts mind-to-mind communication |
| `mindexpel` | `MindExpelPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindExpelPower.cs` via `MagicPowerFactory` | Yes | Expels a connection or presence |
| `mindlook` | `MindLookPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindLookPower.cs` via `MagicPowerFactory` | Yes | Observes through mind mechanics |
| `mindsay` | `MindSayPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/MindSayPower.cs` via `MagicPowerFactory` | Yes | Sends directed mind speech |
| `sense` | `SensePower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/SensePower.cs` via `MagicPowerFactory` | Yes | Senses characters or items across a configured range |
| `telepathy` | `TelepathyPower` | Power | Static `RegisterLoader` in `MudSharpCore/Magic/Powers/TelepathyPower.cs` via `MagicPowerFactory` | Yes | Telepathic communication or related perception |
| n/a | `MagicPowerBase` | Power support | Shared base in `MudSharpCore/Magic/Powers/MagicPowerBase.cs` | No | Shared costs, progs, help text, crime handling, and builder support |
| n/a | `SustainedMagicPower` | Power support | Shared base in `MudSharpCore/Magic/Powers/SustainedMagicPower.cs` | No | Shared support for sustained powers |
| n/a | `MagicalMeleeAttackPower` | Power support | Shared base in `MudSharpCore/Magic/Powers/MagicalMeleeAttackPower.cs` | No | Shared support base for melee-style magical attacks |

## Spell Trigger Types

| Builder/runtime token | Class | Subsystem | Where registered or dispatched | Builder-creatable | Purpose |
| --- | --- | --- | --- | --- | --- |
| `character` | `CastingTriggerCharacter` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacter.cs` via `SpellTriggerFactory` | Yes | Casts at a character target in the same room |
| `characterprogroom` | `CastingTriggerCharacterProgRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacterProgRoom.cs` via `SpellTriggerFactory` | Yes | Casts at a character with prog-driven room targeting |
| `charactervicinity` | `CastingTriggerCharacterVicinity` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCharacterVicinity.cs` via `SpellTriggerFactory` | Yes | Casts at characters in a character's vicinity |
| `corpse` | `CastingTriggerCorpse` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerCorpse.cs` via `SpellTriggerFactory` | Yes | Casts at a corpse target |
| `item` | `CastingTriggerItem` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerItem.cs` via `SpellTriggerFactory` | Yes | Casts at an item target |
| `localitem` | `CastingTriggerLocalItem` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerLocalItem.cs` via `SpellTriggerFactory` | Yes | Casts at a local item target |
| `party` | `CastingTriggerParty` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerParty.cs` via `SpellTriggerFactory` | Yes | Casts across party members |
| `progroom` | `CastingTriggerProgRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerProgRoom.cs` via `SpellTriggerFactory` | Yes | Casts using a prog-driven room rule |
| `room` | `CastingTriggerRoom` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerRoom.cs` via `SpellTriggerFactory` | Yes | Casts at the room or cell |
| `self` | `CastingTriggerSelf` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerSelf.cs` via `SpellTriggerFactory` | Yes | Casts on the caster |
| `vicinity` | `CastingTriggerVicinity` | Spell trigger | Static `RegisterFactory` in `MudSharpCore/Magic/SpellTriggers/CastingTriggerVicinity.cs` via `SpellTriggerFactory` | Yes | Casts across a vicinity target set |

## Spell Effect Types

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
| `damage` | `DamageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DamageEffect.cs` via `SpellEffectFactory` | Yes | Deals damage |
| `deafness` | `DeafnessEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/DeafnessEffect.cs` via `SpellEffectFactory` | Yes | Applies deafness |
| `detectethereal` | `DetectEtherealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants ethereal visual and sensing perception channels |
| `detectinvisible` | `DetectInvisibleEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants magical vision that can pierce ordinary invisibility |
| `detectmagick` | `DetectMagickEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants magical sensing and exposes detectable magical auras in normal descriptions |
| `disease` | `DiseaseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Applies a configurable spell-owned systemic infection payload |
| `executeprog` | `ExecuteProgEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/ExecuteProgEffect.cs` via `SpellEffectFactory` | Yes | Executes a supporting prog |
| `fear` | `FearEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies magical fear that forces flee behaviour in combat |
| `flying` | `FlyingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants the ability to use existing flight movement checks |
| `glow` | `GlowEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/GlowEffect.cs` via `SpellEffectFactory` | Yes | Applies glow |
| `heal` | `HealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/HealEffect.cs` via `SpellEffectFactory` | Yes | Heals damage |
| `healingrate` | `HealingRateSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/HealingRateSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters healing rate |
| `infravision` | `InfravisionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants infrared vision and a darkness difficulty floor |
| `invisibility` | `InvisibilityEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/InvisibilityEffect.cs` via `SpellEffectFactory` | Yes | Applies invisibility |
| `magicresourcedelta` | `MagicResourceDeltaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MagicResourceDeltaEffect.cs` via `SpellEffectFactory` | Yes | Adds or removes a configured magic resource from a character, item, or room |
| `mend` | `MendEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/MendEffect.cs` via `SpellEffectFactory` | Yes | Mends damage or wear |
| `needdelta` | `NeedDeltaEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/NeedDeltaEffect.cs` via `SpellEffectFactory` | Yes | Changes a need immediately |
| `needrate` | `NeedRateSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/NeedRateSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters need rate |
| `pacifism` | `PacifismSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/PacifismSpellEffect.cs` via `SpellEffectFactory` | Yes | Applies pacifism |
| `paralysis` | `ParalysisEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Applies forced paralysis through the health effect system |
| `poison` | `PoisonEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Applies a configurable spell-owned drug payload |
| `rage` | `RageSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RageSpellEffect.cs` via `SpellEffectFactory` | Yes | Applies rage |
| `relocate` | `RelocateEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/RelocateEffect.cs` via `SpellEffectFactory` | Yes | Relocates a target |
| `removecomprehendlanguage` | `RemoveComprehendLanguageEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical language-comprehension effects |
| `removecurse` | `RemoveCurseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes matching magical curse effects |
| `removedetectethereal` | `RemoveDetectEtherealEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical ethereal perception effects |
| `removedetectinvisible` | `RemoveDetectInvisibleEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical detect-invisible effects |
| `removedetectmagick` | `RemoveDetectMagickEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical detect-magick effects |
| `removedisease` | `RemoveDiseaseEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.Configured.cs` via `SpellEffectFactory` | Yes | Removes a matching spell-owned disease payload |
| `removefear` | `RemoveFearEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical fear effects |
| `removeflying` | `RemoveFlyingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical flight-granting effects |
| `removeinfravision` | `RemoveInfravisionEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Removes magical infravision effects |
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
| `waterbreathing` | `WaterBreathingEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/StandaloneStatusSpellEffects.cs` via `SpellEffectFactory` | Yes | Grants additional breathable fluids for the target |
| `weatherchange` | `WeatherChangeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherChangeEffect.cs` via `SpellEffectFactory` | Yes | Changes weather |
| `weatherchangefreeze` | `WeatherChangeFreezeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherChangeFreezeEffect.cs` via `SpellEffectFactory` | Yes | Changes and freezes weather state |
| `weatherfreeze` | `WeatherFreezeEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeatherFreezeEffect.cs` via `SpellEffectFactory` | Yes | Freezes weather state |
| `weight` | `WeightSpellEffect` | Spell effect | Static `RegisterFactory` in `MudSharpCore/Magic/SpellEffects/WeightSpellEffect.cs` via `SpellEffectFactory` | Yes | Alters weight |

## Notes
- Schools are first-class records rather than subtype-driven types, so they are documented in the overview and backbone docs rather than listed here as a type family.
- Powers, triggers, and spell effects are all current-state inventories of registered runtime implementations.
- Builder-creatable means the inspected builder workflow can create an instance of the type directly, not merely load or edit one.
- Phase 1 spell support also introduced additive `IEffect.PerceptionGranting`, spell-runtime query interfaces such as `ISilencedEffect`, `ISleepEffect`, `IFearEffect`, `IFlightEffect`, `IAdditionalBreathableFluidEffect`, `IDarksightEffect`, `IComprehendLanguageEffect`, and `ICurseEffect`, plus shared `MagicArmourConfiguration` for power and spell armour parity.
