# FutureMUD Magic System: Spells

## Purpose
This document explains how magic `spells` work in FutureMUD.

Spells are the data-driven half of the magic subsystem. A spell is assembled from a trigger, effect lists, costs, checks, emotes, and timing rules instead of from one bespoke hard-coded command class.

This document is aimed at:

- agents that need the spell lifecycle and extension points quickly
- developers adding new spell trigger or spell effect types
- builders authoring spells with the `magic spell` workflow
- seeder authors planning manual spell data

## Quick Map
- Read the runtime lifecycle section if you are debugging spell behavior.
- Read the builder workflow section if you are authoring content with `magic spell`.
- Read the developer extension section if you are adding a new trigger or effect type.
- Read [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-trigger-types) and [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-effect-types) if you only need the current type list.

## What A Spell Is
A spell is a persisted runtime object that combines:

- a school
- a known-spell prog
- a trigger
- optional trigger-supplied additional parameters
- target-facing spell effects
- optional caster-only spell effects
- casting cost expressions per resource
- an inventory plan for material requirements
- casting and resist settings
- effect duration settings
- emotes and output flags
- post-cast lockout rules

Important files and contracts are:

- contract: `FutureMUDLibrary/Magic/IMagicSpell.cs`
- runtime object: `MudSharpCore/Magic/MagicSpell.cs`
- trigger contract: `FutureMUDLibrary/Magic/IMagicTrigger.cs`
- effect contract: `FutureMUDLibrary/Magic/IMagicSpellEffectTemplate.cs`
- trigger registry: `MudSharpCore/Magic/SpellTriggerFactory.cs`
- effect registry: `MudSharpCore/Magic/SpellEffectFactory.cs`
- builder command surface: `MudSharpCore/Commands/Modules/MagicModule.cs`, `MudSharpCore/Commands/Helpers/EditableItemHelperMagic.cs`
- persistence: `MudsharpDatabaseLibrary/Models/MagicSpell.cs`

## Runtime Spell Lifecycle
### 1. Known-spell gating
A spell has a `SpellKnownProg`.

This prog is used by the player-facing school command surface to determine:

- whether the spell appears in `<schoolverb> spells`
- whether the player can request spell help
- whether the player can cast the spell through `<schoolverb> cast ...`

### 2. Trigger resolution
Each spell has one trigger, unless it is still incomplete in builder state.

The trigger decides:

- how the spell is invoked
- whether it yields a target
- whether that target can fail to resolve
- what target type string the trigger exposes to the effect compatibility rules
- the allowed `SpellPower` range

If the trigger implements `ICastMagicTrigger`, it can be used through the school verb's `cast` flow.

### 3. Cost validation
Before a spell resolves, `MagicSpell.CastSpell` validates:

- all configured resource costs
- all material requirements in the inventory plan

Cost expressions are evaluated with spell-specific variables such as power and self-targeting state.

### 4. Material validation
Spell materials are expressed through an `InventoryPlanTemplate`.

The spell checks whether the plan is feasible before casting. Failure can come from:

- not enough free hands
- not enough wielders
- missing items

### 5. Target-null handling
If the trigger can fail to yield a target and at least one effect requires one, the spell can emit a target-null emote instead of proceeding.

This is one of the readiness rules checked before the spell is considered game-ready.

### 6. Casting check
Spells use:

- `CheckType.CastSpellCheck` for the caster
- `CheckType.ResistMagicSpellCheck` for resisting targets

The spell can specify:

- casting trait
- casting difficulty
- minimum success threshold
- optional resist trait
- optional resist difficulty

If the casting outcome is below the threshold, the fail-casting emote is shown and the spell does not proceed.

### 7. Casting emote and committed costs
Once the spell passes its casting threshold, the runtime:

- spends its normal costs
- executes its material-consumption plan
- applies any lockouts
- emits the normal casting emote

This remains true even if a downstream ward blocks the spell from taking hold on a target.

### 8. Interdiction and reflection
Before target-side spell effects are applied, `MagicSpell.CastSpell` now consults the shared interdiction layer.

Current interdiction sources are:

- room wards
- personal wards

These wards match by school, optionally include child schools, and can be further gated by a custom prog.

Current behavior is:

- `fail` wards block the target-side spell application
- `reflect` wards only retarget ordinary `character` spells back at the caster
- non-character-targeted invokes downgrade `reflect` to `fail`
- self-targeted character casts do not reflect back onto the same caster; they also downgrade to `fail`

### 9. Resist check
If a resisting target beats the caster's result, the target-resisted emote is shown and the spell does not apply to that target.

### 10. Emotes and output flags
Spells support:

- casting emote
- fail casting emote
- target emote
- target resisted emote
- target null emote
- separate output flags for casting and target emotes

These are central to spell readiness. A spell without a required casting emote is not ready for game.

### 11. Duration calculation
If any applied effect is not instantaneous, the spell needs an effect-duration expression.

That expression can use variables derived from the casting result, including success degrees and chosen power.

### 12. Effect and caster-effect application
Spells maintain two effect lists:

- `SpellEffects`
- `CasterSpellEffects`

At cast time:

- target-side effects are applied to the resolved target or targets
- caster-side effects are applied to the caster
- a `MagicSpellParent` effect groups the child spell effects together on the target

If all effects are instantaneous, the parent effect is not retained.

Some effects now also rely on trigger-supplied additional parameters:

- `exit` from the `exit` and `characterexit` triggers
- `room` from the `progcharacterroom` and `progitemroom` triggers

### 13. Exclusivity and lockouts
Spells can enforce:

- an `ExclusiveDelay`, which blocks all spell casting for a time
- a `NonExclusiveDelay`, which blocks same-school spells for a time
- exclusive applied effects, which replace prior parent effects from the same spell instead of stacking

### 14. Ready-for-game validation
`MagicSpell.ReadyForGame` verifies that the spell has enough data to function safely.

Current readiness checks include:

- trigger present
- casting emote present
- a target-yielding trigger when any effect requires a target
- a target-null emote when the trigger may fail to yield a target
- an effect-duration expression when any effect is non-instantaneous
- trigger and effect compatibility
- casting trait present

## Builder Workflow
### Entry point
The builder entry point is `magic spell`.

Core workflow:

- `magic spell list`
- `magic spell edit new <name> <school>`
- `magic spell clone <old> <new>`
- `magic spell edit <which>`
- `magic spell show`

Discovery helpers:

- `magic spell triggers`
- `magic spell triggerhelp <type>`
- `magic spell effects`
- `magic spell effecthelp <type>`

### Core spell editing
Current shared spell editing includes:

- `name <name>`
- `blurb <text>`
- `description`
- `school <school>`
- `prog <prog>`
- `exclusivedelay <seconds>`
- `nonexclusivedelay <seconds>`
- `castemote <emote>`
- `targetemote <emote>`
- `failcastemote <emote>`
- `targetresistemote <emote>`
- `targetnullemote <emote>`
- `emoteflags <flags>`
- `targetemoteflags <flags>`
- `trait <skill/attribute>`
- `difficulty <difficulty>`
- `threshold <outcome>`
- `resist none`
- `resist <trait> <difficulty>`
- `duration <trait expression>`
- `exclusiveeffect`

### Trigger workflow
Trigger authoring is:

- `magic spell set trigger new <type> [...]`
- `magic spell set trigger set ...`

The spell object delegates subtype editing back into the selected trigger instance.

### Trigger-supplied additional parameters
Triggers can attach named `SpellAdditionalParameter` values to the cast so spell effects can consume extra context without changing the primary target type.

Current shared names are:

- `exit` from `exit` and `characterexit`, carrying the chosen `ICellExit`
- `room` from `progcharacterroom` and `progitemroom`, carrying the prog-resolved `ICell`

### Effect workflow
Target-side effect authoring is:

- `magic spell set effect add <type> [...]`
- `magic spell set effect remove <##>`
- `magic spell set effect <##> ...`

Caster-side effect authoring is:

- `magic spell set castereffect add <type> [...]`
- `magic spell set castereffect remove <##>`
- `magic spell set castereffect <##> ...`

### Phase 1 standalone status and room primitives
The Phase 1 parity slice adds a new family of spell effects, but it does not do so through one generic enum-driven status template.

Builder-visible status and cleanup tokens are discrete effect types:

- `silence` / `removesilence`
- `sleep` / `removesleep`
- `fear` / `removefear`
- `paralysis` / `removeparalysis`
- `flying` / `removeflying`
- `waterbreathing` / `removewaterbreathing`
- `poison` / `removepoison`
- `disease` / `removedisease`
- `curse` / `removecurse`
- `detectinvisible` / `removedetectinvisible`
- `detectethereal` / `removedetectethereal`
- `detectmagick` / `removedetectmagick`
- `infravision` / `removeinfravision`
- `comprehendlanguage` / `removecomprehendlanguage`

Key runtime semantics:

- `teleport` now advertises compatibility with `room` / `rooms` triggers and continues to self-teleport the caster to an `ICell`. `teleporttarget` is unchanged.
- `silence` blocks vocal speech through communication strategies but does not block telepathy or other non-vocal channels.
- `sleep` forces `Sleep()` on apply and only stops blocking wakefulness when the last magical sleep effect is removed.
- `fear` forces flee mode in combat while active.
- `paralysis` uses the forced-paralysis health hook.
- `flying` extends the normal flight checks without forcing a flying position state.
- `waterbreathing` extends breathing logic by granting additional breathable fluids.
- `poison` and `disease` create spell-owned payloads with origin metadata, and the matching removal effects only clear matching payloads created by those spell effects.
- `detectmagick`, `detectinvisible`, `detectethereal`, and `infravision` rely on additive `IEffect.PerceptionGranting` support rather than replacing a target's base perception flags.
- `detectmagick` also exposes active magical auras from ordinary character, item, and room description flows whenever the perceiver can sense magic.
- `comprehendlanguage` bypasses language comprehension checks, but not illiteracy or unknown-script gating.

Other Phase 1 primitives:

- `magicresourcedelta` works against any `IHaveMagicResource` target and relies on the holder's normal clamping rules.
- `spellarmour` reuses the shared `MagicArmourConfiguration` model so spell armour and power armour stay in sync.
- `roomflag` / `removeroomflag` is the early room-state primitive for `peaceful`, `nodream`, `alarm`, `darkness`, and `wardtag`.

### Phase 1.5 form provisioning and spell-driven transformation
The `transformform` spell effect is the body-form provisioning bridge for magic content.

It does not create a one-off temporary shell every cast. Instead it:

- resolves a stable `FormKey`
- ensures a cached alternate form exists for the target character, creating it on first use if necessary
- applies only first-creation defaults from the spell definition for race, ethnicity, gender, alias, sort order, trauma mode, voluntary-switch settings, and visibility prog
- reuses the same provisioned form on later casts with the same spell id plus `FormKey`
- switches the target into that form with scripted switching rules
- stores the previous body id in the applied child effect so expiry can attempt to revert cleanly

Current revert behavior on spell expiry is:

- first try the remembered prior form
- if that form is gone or no longer structurally valid, try the first other owned form that passes scripted switch validation
- if no fallback works, leave the current form in place and emit a staff-facing system warning

Important builder implications:

- creation defaults apply only the first time a keyed form is created
- later admin or FutureProg edits to that form's alias, trauma mode, visibility, or voluntary rules remain authoritative
- hidden forms stay hidden from the owner's `form` list unless they are the current form
- spell expiry does not delete the cached form; the spell source only provisions and reuses it

The `transformform` builder effect currently supports:

- `formkey <text>`
- `race <which>`
- `ethnicity <which>|clear`
- `gender <which>|clear`
- `alias <text>|clear`
- `sort <number>|clear`
- `trauma <auto|transfer|stash>`
- `allow [true|false]`
- `canprog <prog>|clear`
- `whycantprog <prog>|clear`
- `visibleprog <prog>|clear`

### Material workflow
Material requirements are authored through the spell's inventory plan:

- `magic spell set material add ...`
- `magic spell set material delete <#>`

This is the main builder-facing material-component path for spells.

### Cost workflow
Resource costs are authored as expressions per resource:

- `magic spell set cost <resource> <trait expression>`
- `magic spell set cost <resource> remove`

These expressions are evaluated at cast time using spell-specific variables.

## Seeder and Data Author Workflow
There is no dedicated magic spell seeder in `DatabaseSeeder`.

Current seeder implications:

- spells must currently be seeded manually into `MagicSpells`
- the `Definition` field is XML-backed and contains the trigger, costs, effects, caster effects, and material plan
- referenced resources, school, progs, trait expressions, and traits must already exist
- effect and trigger type tokens must match registered runtime types exactly

Recommended manual data order:

1. school
2. resources
3. traits and trait expressions
4. spell-known prog and any supporting target-filter or helper progs
5. spell

## Developer Extension Workflow
### Adding a new trigger type
Triggers implement `IMagicTrigger` and are registered through `SpellTriggerFactory`.

Recommended steps:

1. add a new concrete trigger class under `MudSharpCore/Magic/SpellTriggers`
2. implement:
   - load constructor from XML
   - builder default constructor or builder-load helper
   - `SaveToXml`
   - `Clone`
   - `BuildingCommand`
   - `Show`
   - `ShowPlayer`
   - `DoTriggerCast` if it is a cast trigger
   - `TriggerYieldsTarget`
   - `TriggerMayFailToYieldTarget`
   - `TargetTypes`
3. add static `RegisterFactory`
4. call:
   - `SpellTriggerFactory.RegisterBuilderFactory(<token>, ...)`
   - `SpellTriggerFactory.RegisterLoadTimeFactory(<token>, ...)`
5. make sure the builder help text clearly explains target behavior

Important implementation note:

- `SpellTriggerFactory` discovers trigger registrations by scanning `IMagicTrigger` implementers and invoking static `RegisterFactory`

### Adding a new spell effect type
Spell effects implement `IMagicSpellEffectTemplate` and are registered through `SpellEffectFactory`.

Recommended steps:

1. add a new concrete effect class under `MudSharpCore/Magic/SpellEffects`
2. implement:
   - load constructor from XML
   - builder default or builder factory
   - `SaveToXml`
   - `BuildingCommand`
   - `Show`
   - `Clone`
   - `IsInstantaneous`
   - `RequiresTarget`
   - `IsCompatibleWithTrigger`
   - `GetOrApplyEffect`
3. add static `RegisterFactory`
4. call:
   - `SpellEffectFactory.RegisterLoadTimeFactory(<token>, ...)`
   - `SpellEffectFactory.RegisterBuilderFactory(<token>, ...)`
5. supply a precise builder help string, compatibility list, and default XML shape

Important implementation note:

- `SpellEffectFactory` discovers effect registrations by scanning `IMagicSpellEffectTemplate` implementers and invoking static `RegisterFactory`

### Design guidance
- Use triggers to own invocation and targeting rules.
- Use spell effects to own the applied result.
- Keep trigger target types and effect compatibility aligned.
- If an effect needs trigger-side context such as an exit or remote room, prefer a named `SpellAdditionalParameter` instead of overloading the main target.
- Prefer adding a new spell effect over a new spell type when the behavior is "existing cast flow, new result."
- Prefer adding a new trigger over a new spell effect when the behavior is "new invocation or targeting pattern."
- If a spell effect provisions an alternate form, use a stable `FormKey` and treat spell XML as first-creation defaults rather than ongoing authoritative metadata.

## Current Implemented Trigger Types
| Token | Class | Summary |
| --- | --- | --- |
| `character` | `CastingTriggerCharacter` | Casts at a character target in the same room |
| `characterexit` | `CastingTriggerCharacterExit` | Casts at a same-room character plus a local exit and supplies the chosen `exit` parameter |
| `characterprogroom` | `CastingTriggerCharacterProgRoom` | Casts at a character with prog-driven room targeting |
| `charactervicinity` | `CastingTriggerCharacterVicinity` | Casts at characters in a character's vicinity |
| `corpse` | `CastingTriggerCorpse` | Casts at a corpse target |
| `exit` | `CastingTriggerExit` | Casts at a local exit or direction and supplies the chosen `exit` parameter |
| `item` | `CastingTriggerItem` | Casts at an item target |
| `localitem` | `CastingTriggerLocalItem` | Casts at a local item target |
| `party` | `CastingTriggerParty` | Casts across party members |
| `progcharacter` | `CastingTriggerProgCharacter` | Casts at a world character target resolved by a FutureProg |
| `progcharacterroom` | `CastingTriggerProgCharacterRoom` | Casts at a prog-resolved character and also supplies a prog-resolved `room` parameter |
| `progitem` | `CastingTriggerProgItem` | Casts at a world item target resolved by a FutureProg |
| `progitemroom` | `CastingTriggerProgItemRoom` | Casts at a prog-resolved item and also supplies a prog-resolved `room` parameter |
| `progroom` | `CastingTriggerProgRoom` | Casts using a prog-driven room rule |
| `room` | `CastingTriggerRoom` | Casts at the room or cell |
| `self` | `CastingTriggerSelf` | Casts on the caster |
| `vicinity` | `CastingTriggerVicinity` | Casts across a vicinity target set |

## Current Implemented Spell Effect Types
| Token | Class | Summary |
| --- | --- | --- |
| `blindness` | `BlindnessEffect` | Applies blindness |
| `boost` | `TraitBoostEffect` | Boosts a trait |
| `changecharacteristic` | `ChangeCharacteristicEffect` | Changes a characteristic |
| `comprehendlanguage` | `ComprehendLanguageEffect` | Grants broad spoken and written language comprehension without overriding literacy or script limits |
| `createitem` | `CreateItemEffect` | Creates an item |
| `createliquid` | `CreateLiquidEffect` | Creates a liquid |
| `createnpc` | `CreateNPCEffect` | Creates an NPC |
| `curse` | `CurseEffect` | Applies a spell-owned curse effect |
| `damage` | `DamageEffect` | Deals damage |
| `deafness` | `DeafnessEffect` | Applies deafness |
| `detectethereal` | `DetectEtherealEffect` | Grants ethereal visual and sensing perception |
| `detectinvisible` | `DetectInvisibleEffect` | Grants magical vision that can pierce ordinary invisibility |
| `detectmagick` | `DetectMagickEffect` | Grants magical sensing and visible aura readouts in ordinary descriptions |
| `disease` | `DiseaseEffect` | Applies a configurable spell-owned systemic infection |
| `executeprog` | `ExecuteProgEffect` | Executes a supporting prog |
| `exitbarrier` | `ExitBarrierEffect` | Applies a persistent magical barrier to a targeted exit |
| `forcedexitmovement` | `ForcedExitMovementEffect` | Forces a targeted character through the trigger-supplied `exit` when movement is legal |
| `fear` | `FearEffect` | Applies magical fear that enforces flee behaviour in combat |
| `flying` | `FlyingEffect` | Grants flight eligibility through the normal movement checks |
| `glow` | `GlowEffect` | Applies glow or light-style effect |
| `heal` | `HealEffect` | Heals damage |
| `healingrate` | `HealingRateSpellEffect` | Alters healing rate |
| `infravision` | `InfravisionEffect` | Grants infrared vision and a darkness difficulty floor |
| `invisibility` | `InvisibilityEffect` | Applies invisibility |
| `magicresourcedelta` | `MagicResourceDeltaEffect` | Adds or removes a configured magic resource from a character, item, or room |
| `mend` | `MendEffect` | Mends damage or wear |
| `needdelta` | `NeedDeltaEffect` | Changes a need immediately |
| `needrate` | `NeedRateSpellEffect` | Alters need rate |
| `pacifism` | `PacifismSpellEffect` | Applies pacifism |
| `personalward` | `PersonalWardEffect` | Applies a school-based personal ward that can fail or reflect matching incoming or outgoing magic |
| `paralysis` | `ParalysisEffect` | Applies magical paralysis through the forced-paralysis hook |
| `poison` | `PoisonEffect` | Applies a configurable spell-owned drug payload |
| `rage` | `RageSpellEffect` | Applies rage |
| `relocate` | `RelocateEffect` | Relocates a target |
| `removecomprehendlanguage` | `RemoveComprehendLanguageEffect` | Removes magical language-comprehension effects |
| `removecurse` | `RemoveCurseEffect` | Removes matching magical curse effects |
| `removedetectethereal` | `RemoveDetectEtherealEffect` | Removes magical ethereal perception effects |
| `removedetectinvisible` | `RemoveDetectInvisibleEffect` | Removes magical detect-invisible effects |
| `removedetectmagick` | `RemoveDetectMagickEffect` | Removes magical detect-magick effects |
| `removedisease` | `RemoveDiseaseEffect` | Removes a matching spell-owned disease payload |
| `removefear` | `RemoveFearEffect` | Removes magical fear effects |
| `removeflying` | `RemoveFlyingEffect` | Removes magical flight-granting effects |
| `removeinfravision` | `RemoveInfravisionEffect` | Removes magical infravision effects |
| `removeparalysis` | `RemoveParalysisEffect` | Removes magical paralysis effects |
| `removepoison` | `RemovePoisonEffect` | Removes a matching spell-owned poison payload |
| `removeroomflag` | `RemoveRoomFlagEffect` | Removes a configured magical room flag |
| `removesilence` | `RemoveSilenceEffect` | Removes magical silence effects |
| `removesleep` | `RemoveSleepEffect` | Removes magical sleep effects |
| `removewaterbreathing` | `RemoveWaterBreathingEffect` | Removes magical water-breathing effects |
| `resurrect` | `ResurrectionEffect` | Resurrects a target |
| `roomflag` | `RoomFlagEffect` | Applies a configured magical room flag such as peaceful, no-dream, alarm, darkness, or ward tags |
| `roomatmosphere` | `RoomAtmosphereEffect` | Alters room atmosphere |
| `roomlight` | `RoomLightEffect` | Alters room light |
| `roomward` | `RoomWardEffect` | Applies a school-based room ward that can fail or reflect matching incoming or outgoing magic |
| `roomtemperature` | `RoomTemperatureEffect` | Alters room temperature |
| `selfdamage` | `SelfDamageEffect` | Damages the caster |
| `silence` | `SilenceEffect` | Applies vocal silence without blocking telepathy |
| `sleep` | `SleepEffect` | Forces magical sleep |
| `spellarmour` | `SpellArmourEffect` | Applies spell-owned magical armour using the shared armour configuration |
| `staminadelta` | `StaminaDeltaSpellEffect` | Changes stamina immediately |
| `staminaexpendrate` | `StaminaExpenditureSpellEffect` | Alters stamina expenditure rate |
| `staminaregenrate` | `StaminaRegenRateSpellEffect` | Alters stamina regeneration rate |
| `telepathy` | `TelepathySpellEffect` | Applies telepathic linkage |
| `teleport` | `TeleportEffect` | Teleports the caster to a room or cell target |
| `teleporttarget` | `TeleportTargetEffect` | Teleports a target selected by the spell |
| `waterbreathing` | `WaterBreathingEffect` | Grants additional breathable fluids |
| `weatherchange` | `WeatherChangeEffect` | Changes weather |
| `weatherchangefreeze` | `WeatherChangeFreezeEffect` | Changes and freezes weather state |
| `weatherfreeze` | `WeatherFreezeEffect` | Freezes weather state |
| `weight` | `WeightSpellEffect` | Alters weight |

## Ward Effects
`roomward` and `personalward` are the reusable spell-authored interdiction primitives.

Both support these builder commands:

- `school <school>`
- `mode fail|reflect`
- `coverage incoming|outgoing|both`
- `subschools`
- `prog <prog>|none`

The optional custom prog can use either of these signatures:

- `(character source, perceivable owner) -> bool`
- `(character source, perceivable owner, magicschool school) -> bool`

Reflection is intentionally narrow:

- only ordinary `character` spell casts retarget back to the caster
- all other spell target types downgrade `reflect` to `fail`

## Important Current-State Notes
- Spells are more builder-composable than powers, but they still rely on registered C# trigger and effect implementations.
- The trigger/effect registries are the extension point; there is no fully script-defined spell-effect system.
- Readiness validation is a major part of spell authoring. If a spell is incomplete, it will show a builder error rather than quietly misbehaving.
- Caster effects are separate from ordinary effects and apply to the caster after the target-side application path.
- Target-side spell application now checks room and personal wards after the casting emote and before any target-side effect is applied.
- Status-style spell effects are not currently modelled as one generic "status enum" template. The builder-visible Phase 1 statuses are intentionally separate effect types so XML shape, help text, and runtime semantics can differ cleanly per effect.

## Related Reading
- [Magic System Overview](./Magic_System_Overview.md)
- [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md)
- [Magic System: Powers](./Magic_System_Powers.md)
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-trigger-types)
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-effect-types)
