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
- room tag wards
- personal tag wards

School wards match by school, optionally include child schools, and can be further gated by a custom prog. Tag wards match configured `magictag` key/value metadata exposed by spell effects that implement the contextual interdiction tag provider.

Current behavior is:

- `fail` wards block the target-side spell application
- `reflect` wards only retarget ordinary `character` spells back at the caster
- non-character-targeted invokes downgrade `reflect` to `fail`
- self-targeted character casts do not reflect back onto the same caster; they also downgrade to `fail`
- a blocked or resisted target aborts that invoke before caster-side spell effects are applied

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

In builder display, `magic spell show` presents target-side effects and caster-side effects as separate titled sections. Each effect owns its own builder-facing summary through `Show(ICharacter)`, which should return a compact multi-line description beginning with the effect name and followed by labelled rows for its configured values. `MagicSpell.Show` only numbers, indents, and wraps those returned lines; it must not parse packed one-line effect summaries.

Some effects now also rely on trigger-supplied additional parameters:

- `exit` from the `exit` and `characterexit` triggers
- `room` from the `progcharacterroom` and `progitemroom` triggers

### 13. FutureProg inspection and control
FutureProg can inspect and manipulate the live spell state without going through player command parsing.

Character dot-properties expose:

- `magiccapabilities`
- `knownspells`
- `castablespells`
- `castablespellsnow`

The matching built-in functions are useful when the source character is not already the object being dereferenced:

- `magiccapabilities(character)`
- `knownspells(character)`
- `castablespells(character)`
- `castablespellsnow(character)`
- `cancastspell(character, spell)`
- `cancastspellnow(character, spell[, target][, power])`

`cancastspell` checks the general command surface: capability school access, known-spell prog, spell readiness, and cast-trigger availability. `cancastspellnow` also checks current lockouts, available magic resources, material requirements, target-null rules, and the requested or any permitted spell power. It does not perform the random casting check, consume resources, execute material plans, run crimes, or consult target wards.

Active spell parent effects on a character are exposed through:

- `activespells(character)`
- `activespelleffects(character)`
- `activespelleffects(character, spell)`
- `spellremainingduration(character, spell)` / `spellduration(character, spell)`
- `setspellduration(character, spell, duration)`
- `addspellduration(character, spell, duration)`
- `subtractspellduration(character, spell, duration)`
- `removespell(character, spell)`

Duration helpers operate on retained `MagicSpellParent` effects. The retrieval helper returns the longest remaining scheduled duration, or zero when no matching effect is scheduled. Set can turn an unscheduled spell-parent effect into a scheduled one; add and subtract only alter already scheduled spell-parent effects. Remove clears the parent and its owned child effects.

### 14. Exclusivity and lockouts
Spells can enforce:

- an `ExclusiveDelay`, which blocks all spell casting for a time
- a `NonExclusiveDelay`, which blocks same-school spells for a time
- exclusive applied effects, which replace prior parent effects from the same spell instead of stacking

### 15. Ready-for-game validation
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
- `planarstate` / `removeplanarstate`
- `planeshift`

Key runtime semantics:

- `teleport` now advertises compatibility with `room` / `rooms` triggers and continues to self-teleport the caster to an `ICell`. `teleporttarget` is unchanged.
- `silence` blocks vocal speech through communication strategies but does not block telepathy or other non-vocal channels.
- `sleep` forces `Sleep()` on apply and only stops blocking wakefulness when the last magical sleep effect is removed.
- `fear` forces flee mode in combat while active.
- `paralysis` uses the forced-paralysis health hook.
- `flying` extends the normal flight checks without forcing a flying position state.
- `waterbreathing` extends breathing logic by granting additional breathable fluids.
- `poison` and `disease` create spell-owned payloads with origin metadata, and the matching removal effects only clear matching payloads created by those spell effects. Poison cleanup also recognises matching legacy originless saved payloads so older rows can be repaired without removing unrelated active poisons.
- `detectmagick`, `detectinvisible`, `detectethereal`, and `infravision` rely on additive `IEffect.PerceptionGranting` support rather than replacing a target's base perception flags.
- `detectmagick` also exposes active magical auras from ordinary character, item, and room description flows whenever the perceiver can sense magic.
- `comprehendlanguage` bypasses language comprehension checks, but not illiteracy or unknown-script gating.
- `planarstate` and `planeshift` apply spell-owned planar presence overlays through the corporeality system. `removeplanarstate` clears saved and spell-owned planar overlays from the target.

Other Phase 1 primitives:

- `magicresourcedelta` works against any `IHaveMagicResource` target and relies on the holder's normal clamping rules.
- `spellarmour` reuses the shared `MagicArmourConfiguration` model so spell armour and power armour stay in sync.
- `roomflag` / `removeroomflag` is the early room-state primitive for `peaceful`, `nodream`, `alarm`, `darkness`, and `wardtag`.

Persistence and numeric-safety invariants for current primitives:

- XML loaders for builder-authored effects must tolerate legacy element names where that is unambiguous, while new saves should emit one canonical shape. For example, `createliquid` now saves `LiquidId` and still loads older `Liquid` rows.
- Builder-controlled effect parameters that later feed `TimeSpan`, movement loops, need deltas, or weather/lifecycle state must reject `NaN`, infinity, and impractically large values before persistence. Loaders clamp legacy rows into bounded defaults instead of letting malformed data crash the game loop.
- `telepathy` spell effects apply to the spell caster as the listener for the intended target only; they do not become global passive listeners when no applicability prog is present.

### Wind movement and fall-control primitives
The Wind parity slice adds first-class movement and fall-control effects rather than modelling them as generic tags.

Builder-visible Wind tokens are:

- `levitate`
- `featherfall`
- `removeinvisibility` / `dispelinvisibility`
- `forcedpathmovement` / `handsofwind`
- `transference`

Key runtime semantics:

- `levitate` applies a spell-owned levitation effect to a character or item, prevents normal fall checks while active, can optionally move the target to a configured room layer, and adds description text.
- `featherfall` applies configurable fall-distance and fall-damage multipliers. It mitigates impact harm while leaving normal descent and fall exits intact.
- `removeinvisibility` removes only `SpellInvisibilityEffect` child effects from the target. `dispelinvisibility` is a builder/load alias that saves as `removeinvisibility`.
- `forcedpathmovement` uses a `characterexit` trigger's `exit` parameter, validates each exit with the normal movement and crossing checks, and can force a target through one or more same-direction exits. `handsofwind` is a builder/load alias that saves as `forcedpathmovement`.
- `forcedpathmovement` caps repeated same-direction movement at a small bounded step count so player-triggered wind spells cannot monopolise the command loop.
- `transference` swaps the caster and target character locations, optionally including followers, dragged targets, riders, and room-layer swapping.
- Magical flight expiry now calls the flight-continuation check instead of the start-flight check, so a target only drops when no remaining spell, immwalk, or physical flight source can sustain them.
- `dispelmagic effect invisibility|flight|levitation|featherfall` can target these Wind statuses through the general dispel flow.

### Phase 1.5 form provisioning and spell-driven transformation
The `transformform` spell effect is the body-form provisioning bridge for magic content.

It does not create a one-off temporary shell every cast. Instead it:

- resolves a stable `FormKey`
- ensures a cached alternate form exists for the target character, creating it on first use if necessary
- applies only first-creation defaults from the spell definition for race, ethnicity, gender, alias, sort order, trauma mode, voluntary-switch settings, visibility prog, transformation echo, and body-specific description patterns
- reuses the same provisioned form on later casts with the same spell id plus `FormKey`
- contributes a mandatory transformation demand into the character's shared forced-transformation resolver
- uses configurable priority band and priority offset values so overlapping spell, merit, and future forced sources resolve consistently
- preserves the character's baseline free form while any mandatory demand is active so expiry can revert through stacked transformations cleanly

Current revert behavior when spell-driven transform effects end is:

- if another forced demand still wins, the character moves into that other demand's form
- otherwise first try the saved baseline form from before the first mandatory transformation began
- if that form is gone or no longer structurally valid, try the first other owned form that passes scripted switch validation
- if no fallback works, leave the current form in place and emit a staff-facing system warning

Important builder implications:

- creation defaults apply only the first time a keyed form is created
- later admin or FutureProg edits to that form's alias, trauma mode, visibility, or voluntary rules remain authoritative
- later admin or FutureProg edits to that form's transformation echo and description patterns also remain authoritative
- hidden forms stay hidden from the owner's `form` list unless they are the current form
- spell expiry does not delete the cached form; the spell source only provisions and reuses it
- overlapping forced sources are resolved centrally in this default order: merit or intrinsic, drug or chemical, spell or power, admin forced
- if no explicit short or full description pattern is supplied, the runtime tries to pick a random valid pattern for the target form and only falls back to generic text when no valid pattern exists
- switching emits the form's configured transformation echo after the new body has been stabilised; `default` uses the `DefaultFormTransformationEcho` static string and a blank echo suppresses the emote entirely
- custom transformation echoes are sanitised before being stored or emitted so brace characters cannot break emote parsing
- switch activation intentionally delays normal health and consequence feedback until organ functions and positioning have been recalculated, preventing transient `can't breathe` or `tumble to the ground` noise during valid transformations

The `transformform` builder effect currently supports:

- `formkey <text>`
- `race <which>`
- `ethnicity <which>|clear`
- `gender <which>|clear`
- `alias <text>|clear`
- `sort <number>|clear`
- `trauma <auto|transfer|stash>`
- `echo <text>|default|none`
- `allow [true|false]`
- `canprog <prog>|clear`
- `whycantprog <prog>|clear`
- `visibleprog <prog>|clear`
- `sdescpattern <pattern>|random|clear`
- `fdescpattern <pattern>|random|clear`
- `priorityband <merit|drug|spell|admin>`
- `priorityoffset <number>`

The `bodybackup` builder effect uses the same stable form provisioning idea for clone or sleeve magic, but it does not force a transformation while active. It readies the keyed form at the recipient's current cell and room layer as a non-permanent death-transfer target. Builders can configure the provisioned race/ethnicity/gender/alias/sort order, backup priority, non-final old-body remains context, whether the backup is consumed on use, and old-location, new-location, and self echoes.

### Simultaneous body instance spell effects
The multi-body and possession slices add spell effects that create additional active world actors instead of switching the caster's only current body:

- `astralprojection` provisions or reuses a keyed astral form, spawns a temporary player-focusable secondary instance at the anchor's current cell/layer, applies astral planar presence, anchors the primary body according to the configured anchor policy, and collapses only the projection when the effect ends.
- `createcopy` provisions or reuses a keyed magical-copy form, spawns a cell-local secondary instance, optionally makes it player-focusable, optionally applies an intangible/planar presentation, never copies inventory implicitly, and collapses without creating body remains.
- `createclone` provisions or reuses a keyed physical-clone form, spawns a tangible cell-local secondary instance, optionally makes it player-focusable, uses body-local inventory only, and treats clone death as non-final for the owning identity.
- `possessbody` targets a non-player character and provisions or reuses a caster-owned possessed-shell form keyed by the source target. It copies the target body's race, ethnicity, gender, short description, and full description into the shell, spawns the shell at the target's current cell/layer as a player-focusable `PossessedBody` instance, and stores anchor/source-target metadata for cleanup and audit. It rejects player characters, guests, dead targets, and already-active possessed shells for the same caster.
- `seizebody` targets a living character and temporarily moves the caster's controller into the target's actual body. NPC control is paused and restored on cleanup; PC victims are moved into a bound spectator context that receives possession status messages without command authority. It rejects guests, self/same-identity targets, dead targets, administrators unless configured, nested possession, and casters already sustaining another possession.
- `possesscorpse` targets a visible corpse item with an `OriginalBody`, hides that item, spawns a temporary `PossessedCorpse` actor under the corpse's original identity using the same body, and restores the same mutated corpse item at the animated actor's final cell/layer when the spell ends. It rejects nested possession, already-hidden/possessed corpse items, unsupported skeletal/final-remains policies, and unavailable original bodies.
- `animatecorpse` targets a visible corpse item with an `OriginalBody`, hides that item, spawns a temporary `AnimatedCorpse` scripted-AI actor under the corpse's original identity using the same body, attaches builder-selected AIs such as commandable or aggressive, and restores the same mutated corpse item at the animated actor's final cell/layer when the spell ends. It rejects already-animated/possessed corpse items, unsupported skeletal/final-remains policies, unavailable original bodies, and effects with no configured AIs.

The same-identity effects reuse the same owned-form provisioning conventions as `transformform`: stable `formkey`, first-creation race/ethnicity/gender/alias/sort defaults, and optional description replacement patterns. `possessbody` derives its shell demographics and descriptions from the target each time the target-keyed form is ensured. Unlike `transformform`, these effects materialise a non-primary `CharacterInstance` row and a loaded cell-local actor that remains out of the global character, NPC, and actor caches. `seizebody` and `possesscorpse` instead use the direct possession control service: the caster's controller is attached to a non-owned live or corpse actor, the caster remains anchored to their primary body, and cleanup is driven by the owning spell effect.

`possessbody` remains the safe shell model for NPC-derived vessels. Use `seizebody` when the fiction requires direct control of an actual living body, including hostile PC possession. Use `possesscorpse` when the fiction requires the caster to command the same corpse item as a possessed vessel. Use `animatecorpse` when the fiction requires a zombie-like NPC with configured AIs. Both corpse effects hide and restore the same corpse item rather than producing a second set of remains. These effects are temporary, spell-bound, non-persistent across reboot, and collapse on dispel, relevant death/logout cleanup, or actor retirement.

Builder-facing examples and command sequences are in [Multiple Body Forms and Instances Builder Guide](../Characters/Multiple_Body_Forms_and_Instances_Builder_Guide.md).

### Engine V2 dispels, portals, item enchantments, and recipes
Engine V2 adds a deeper parity layer. True body-left-behind projection and copy/clone gameplay now live in the simultaneous body instance effects above rather than in planar overlays alone.

General dispels use `dispelmagic`. It can either remove matching spell-parent effects or shorten their scheduled duration. Matching can be restricted by:

- specific spell
- magic school, optionally including child schools
- caster policy: own, any, or others
- magic tag and optional tag value
- approved effect key such as `spell`, `invisibility`, `flight`, `levitation`, `featherfall`, `burning`, `trackmark`, `magictag`, `itemenchant`, `portal`, `planarstate`, `roomward`, `personalward`, `exitbarrier`, `subjectivedesc`, `phantomillusion`, `transformform`, `possessbody`, `seizebody`, `possesscorpse`, `animatecorpse`, `projectile`, `crafttool`, `powerfuel`, or `itemevent`
- keyed subjective or phantom illusions through `illusion <key>`
- optional strength contest

The default policy is caster-owned cleanup. Hostile dispels must be explicitly configured with hostile matching and then travel through the ordinary spell targeting, ward, and resistance flow.

Strength-contested dispels are off by default. When enabled with `contest`, the dispel compares the dispel cast strength (`power + opposed outcome + bonus`) against the saved target spell strength (`power + opposed outcome`). Ties succeed. Legacy saved spell effects load as standard-strength effects with no opposed-outcome bonus.

The Engine V3 edge-status slice adds:

- `detectpoison`: instantly reports a character's active and latent drug dosages, including drug IDs, localized mass, and latent delivery vector.
- `insomnia` / `removeinsomnia`: prevents voluntary sleep and blocks magical sleep while active.
- `removeblindness` / `cureblindness`: removes spell-owned blindness; `cureblindness` saves as `removeblindness`.

The Engine V4 psionic/perception slice adds:

- contextual interdiction tags from `magictag`, so tag-aware wards can inspect spell metadata rather than only school/subschool.
- `roomtagward` / `personaltagward`, which reuse ward coverage and fail/reflect modes but match `magictag` key/value metadata.
- subjective-description priority, illusion keys, audience scopes, clan gates, and viewer FutureProg gates on `subjectivedesc` / `subjectivesdesc`.
- `phantomillusion`, which adds non-interactive room LOOK text for scoped viewers without creating targetable actors, items, or perceivables.
- `dispelmagic illusion <key>`, which targets keyed subjective-description and phantom room illusions through the general dispel flow.

Illusion audience scopes are shared between `subjectivedesc`, `subjectivesdesc`, and `phantomillusion`: `caster`, `target`, `everyone`, `samecell`, `samezone`, `party`, and `clan`. `viewerprog <prog|none>` can add a boolean FutureProg gate with parameters `(perceivable)`, `(perceivable, perceiver)`, or `(perceivable, perceiver, character)`, where the optional character is the caster when available. `clan <which|none>` supplies the clan id for `scope clan`. Existing `FixedViewer` XML remains valid; `FixedViewer=true` loads as `scope caster`, while `FixedViewer=false` loads as `scope everyone`.

`phantomillusion` is deliberately presentation-only. It lives as an `IDescriptionAdditionEffect` on the room/cell, stacks after ordinary room addenda by priority and effect id, and is only visible in room LOOK output for eligible viewers. It cannot be found by normal target resolution, combat targeting, inventory handling, or emote token lookup because it never creates a physical perceivable.

The persistent sensory/combat slice adds:

- `burning` / `ignite`: spell-owned recurring burning for characters or items, with configurable per-tick damage, pain, stun, thermal load, oxidation requirement, and visible addenda.
- `trackmark` / `tracktrail`: spell-owned track intensity modification for characters, with visual/olfactory multipliers or bonuses and optional magically-marked track circumstances.
- `dispelmagic effect burning` and `dispelmagic effect trackmark` for targeted cleanup.

Burning and need-delta effects share the spell numeric-safety invariant: persisted non-finite values fall back to safe defaults, large finite values are clamped to bounded maxima, and builders reject unsafe values before saving.

Weather-freeze effects own the freeze counter they acquire. Immediate freezes and "next transition" freezes both clean up through the same active effect, so expiry or removal unfreezes owned weather state or unsubscribes a pending transition listener.

Simple portals remain saved spell effects, not database exits. The `portal` effect creates paired transient exits registered with `IExitManager`; active magical portals expose `IMagicPortalExit` metadata and can be inspected with `magic portals`. Anchor tags can be placed on rooms or items/objects with `magictag`; `portal` resolves caster-owned room anchors first, then caster-owned item anchors by using the item location. If more than one caster-owned anchor matches the configured tag/value, the lowest-ID matching room or item is selected deterministically instead of treating duplicate anchors as a runtime error. Builders can inspect active anchors with `magic anchors [tag]`.

Durable portal/rune topology is now first-class magic data. `MagicPortalNetworks`, endpoints, and explicit links are persisted in the database, loaded after rooms/items, and materialised at runtime as topology-managed transient exits rather than permanent `Exits` rows. Builders manage standing networks with `magic portalnetwork` / `magic portalnet`, including active state, cross-zone policy, portal command text, room or directly placed item endpoints, explicit links, and `refresh` repair. The `portalnetwork` spell effect can create an endpoint at the caster room, target room, or target item and optionally link it to an existing endpoint key. Permanent casts may also update an existing endpoint key. Spell-created topology is cleaned up when the spell effect ends unless the effect is marked `permanent`.

V1 topology links are bidirectional because the runtime projection uses the existing bidirectional `TransientExit` primitive. Item endpoints are active only while the referenced item is directly located in a room; carried, contained, or inventory-held rune items are treated as invalid until placed again.

`itemenchant` now has first-class hooks beyond visible aura text, glow, weapon bonuses, and armour reduction:

- projectile quality, damage, pain, and stun bonuses
- craft tool fitness, phase speed, and tool usage multipliers
- powered-item production, consumption, and fuel-use multipliers
- optional item event type and callback prog

Use `magictag` for metadata and lookup facts only. Runtime behaviours like projectile payloads, crafting bonuses, powered-item modifiers, and item event callbacks should use the first-class enchantment hooks.

Current recipe guidance:

- `Ethereal`: use `planarstate` or `planeshift` with a noncorporeal plane definition.
- `Dispel Ethereal`: use `removeplanarstate`, or `dispelmagic` restricted to `effect planarstate` / the relevant school.
- `Planeshift`: use `planeshift` when the target itself moves into the configured planar state.
- Shadow or astral walking without a left-behind body: use plane definitions plus `planeshift` when the caster becomes the walker.
- Astral projection with a left-behind body: use `astralprojection` and configure its form, plane, anchor policy, and echoes.
- Magical mirror images or tangible clones: use `createcopy` or `createclone` depending on whether the secondary should collapse illusion-style or behave as a body-local physical clone.
- First-slice possession of a non-player target: use `possessbody` when the caster should focus a caster-owned shell derived from the target's body demographics and descriptions.
- Direct hostile control of a live body: use `seizebody` when the caster should command the target's actual body, with PC victims held in the bound spectator context and NPC controllers restored afterward.
- Player-commanded corpse possession preserving the same corpse item: use `possesscorpse` when the corpse item should be hidden while the caster commands its mutated `OriginalBody`, then restored when the effect collapses.
- AI zombie animation preserving the same corpse item: use `animatecorpse` when the corpse item should be hidden while its mutated `OriginalBody` rises as a temporary scripted-AI actor with configured AIs, then restored when the effect collapses.
- Polymorph: use `transformform` with a stable `FormKey` and first-creation body-form defaults.

Still-deferred boundary: broader send-shadow control, shadow-identity recognition, and campaign-specific legal/social responsibility require policy beyond the current possession metadata. Body-left-behind projection, magical copies, physical clones, NPC-derived possessed shells, direct live-body possession, same-corpse-item possession, and AI corpse animation are implemented through the body-instance and possession-control layers, with command routing, source-body vulnerability, reconnect behavior, dispel cleanup, and admin observability handled by those layers where applicable.

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

Durable portal networks are authored after rooms/items exist. If spells create topology, create the target `MagicPortalNetwork` first, then configure the spell's `portalnetwork` effect with the network and endpoint keys it should maintain.

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
- Effect `Show(ICharacter)` methods should use the shared spell-effect presentation helper to return effect-owned multi-line builder summaries. Keep labels and values in the concrete effect rather than encoding a single packed line for `MagicSpell.Show` to interpret.

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
The V4 spell-side catalogue adds 2 tag-aware ward tokens: `roomtagward` and `personaltagward`. The persistent sensory/combat slice adds `burning`, `ignite`, `trackmark`, and `tracktrail`. The body-instance slices add `astralprojection`, `createcopy`, `createclone`, `possessbody`, `seizebody`, `possesscorpse`, and `animatecorpse`. The objective/group-scoped illusion slice adds audience-scoped subjective descriptions and `phantomillusion`.

| Token | Class | Summary |
| --- | --- | --- |
| `astralprojection` | `AstralProjectionSpellEffect` | Ensures or reuses a keyed astral form, spawns a temporary focusable projection, and anchors the primary body |
| `animatecorpse` | `AnimateCorpseSpellEffect` | Animates a corpse item's `OriginalBody` as a temporary AI-controlled `AnimatedCorpse` actor with configured AIs and restores the same corpse item when the effect ends |
| `blindness` | `BlindnessEffect` | Applies blindness |
| `boost` | `TraitBoostEffect` | Boosts a trait |
| `bodybackup` | `BodyBackupSpellEffect` | Ensures or reuses a keyed alternate body form and readies it as a death backup with configurable non-final remains context and transfer echoes |
| `burning` | `BurningEffect` | Applies spell-owned recurring burning to characters or items, with configurable per-tick damage, pain, stun, thermal load, oxidation requirement, and visible addenda |
| `changecharacteristic` | `ChangeCharacteristicEffect` | Changes a characteristic |
| `comprehendlanguage` | `ComprehendLanguageEffect` | Grants broad spoken and written language comprehension without overriding literacy or script limits |
| `createclone` | `CloneSpellEffect` | Ensures or reuses a keyed tangible clone form and spawns a cell-local physical-clone secondary instance |
| `createcopy` | `CopySpellEffect` | Ensures or reuses a keyed magical-copy form and spawns a cell-local copy secondary instance |
| `createitem` | `CreateItemEffect` | Creates an item |
| `createliquid` | `CreateLiquidEffect` | Creates a liquid |
| `createnpc` | `CreateNPCEffect` | Creates an NPC |
| `curse` | `CurseEffect` | Applies a spell-owned curse effect |
| `cureblindness` | `RemoveBlindnessEffect` | Builder/load alias for `removeblindness` |
| `damage` | `DamageEffect` | Deals damage |
| `deafness` | `DeafnessEffect` | Applies deafness |
| `detectethereal` | `DetectEtherealEffect` | Grants ethereal visual and sensing perception |
| `detectinvisible` | `DetectInvisibleEffect` | Grants magical vision that can pierce ordinary invisibility |
| `detectmagick` | `DetectMagickEffect` | Grants magical sensing and visible aura readouts in ordinary descriptions |
| `detectpoison` | `DetectPoisonEffect` | Reports active and latent drug dosages on a character |
| `disease` | `DiseaseEffect` | Applies a configurable spell-owned systemic infection |
| `dispelinvisibility` | `RemoveInvisibilityEffect` | Builder/load alias for `removeinvisibility` |
| `dispelmagic` | `DispelMagicEffect` | Removes or shortens matching saved spell effects by caster policy, spell, school/subschool, magic tag, illusion key, approved effect key, or optional strength contest |
| `destroyitem` | `DestroyItemEffect` | Deletes item targets with purge-warning safeguards |
| `executeprog` | `ExecuteProgEffect` | Executes a supporting prog |
| `exitbarrier` | `ExitBarrierEffect` | Applies a persistent magical barrier to a targeted exit |
| `featherfall` | `FeatherFallEffect` | Applies configurable fall-distance and fall-damage mitigation |
| `forcedexitmovement` | `ForcedExitMovementEffect` | Forces a targeted character through the trigger-supplied `exit` when movement is legal |
| `forcedpathmovement` | `ForcedPathMovementEffect` | Forces a targeted character through one or more same-direction exits when movement is legal |
| `fear` | `FearEffect` | Applies magical fear that enforces flee behaviour in combat |
| `flying` | `FlyingEffect` | Grants flight eligibility through the normal movement checks |
| `glow` | `GlowEffect` | Applies glow or light-style effect |
| `handsofwind` | `ForcedPathMovementEffect` | Builder/load alias for `forcedpathmovement` |
| `heal` | `HealEffect` | Heals damage |
| `healingrate` | `HealingRateSpellEffect` | Alters healing rate |
| `infravision` | `InfravisionEffect` | Grants infrared vision and a darkness difficulty floor |
| `insomnia` | `InsomniaEffect` | Prevents voluntary and magical sleep |
| `invisibility` | `InvisibilityEffect` | Applies invisibility |
| `ignite` | `BurningEffect` | Builder/load alias for `burning` |
| `itemdamage` | `ItemDamageEffect` | Damages an item with configured damage, pain, stun, and damage type |
| `itemenchant` | `ItemEnchantEffect` | Adds aura/glow, weapon/armour bonuses, projectile payload bonuses, craft-tool bonuses, power/fuel modifiers, and optional item event progs |
| `levitate` | `LevitationEffect` | Suspends a character or item, optionally moves it to a configured room layer, and prevents falling while active |
| `magictag` | `MagicTagEffect` | Adds spell-owned key/value metadata for marks, anchors, runes, signatures, FutureProg queries, and tag-aware interdiction |
| `magicresourcedelta` | `MagicResourceDeltaEffect` | Adds or removes a configured magic resource from a character, item, or room |
| `mend` | `MendEffect` | Mends damage or wear |
| `needdelta` | `NeedDeltaEffect` | Changes a need immediately |
| `needrate` | `NeedRateSpellEffect` | Alters need rate |
| `pacifism` | `PacifismSpellEffect` | Applies pacifism |
| `personalward` | `PersonalWardEffect` | Applies a school-based personal ward that can fail or reflect matching incoming or outgoing magic |
| `personaltagward` | `PersonalTagWardEffect` | Applies a personal ward that fails or reflects matching incoming or outgoing magic by `magictag` key/value |
| `phantomillusion` | `PhantomIllusionEffect` | Adds non-interactive phantom room LOOK text for eligible scoped viewers, with priority, colour, viewer-prog, clan, and illusion-key controls |
| `planarstate` | `PlanarStateSpellEffect` | Applies a corporeal or noncorporeal planar overlay to the target |
| `planeshift` | `PlanarStateSpellEffect` | Moves the target into a configured corporeal or noncorporeal planar state |
| `paralysis` | `ParalysisEffect` | Applies magical paralysis through the forced-paralysis hook |
| `poison` | `PoisonEffect` | Applies a configurable spell-owned drug payload |
| `portal` | `PortalSpellEffect` | Creates an effect-owned paired transient portal between the caster's room and a target room, room anchor, or item/object anchor |
| `possessbody` | `PossessBodySpellEffect` | Creates a caster-owned, player-focusable possessed shell derived from a non-player character target |
| `possesscorpse` | `PossessCorpseSpellEffect` | Animates a corpse item's `OriginalBody` as a temporary `PossessedCorpse` actor and restores the same corpse item when the effect ends |
| `rage` | `RageSpellEffect` | Applies rage |
| `relocate` | `RelocateEffect` | Relocates a target |
| `removeblindness` | `RemoveBlindnessEffect` | Removes spell-owned blindness effects |
| `removecomprehendlanguage` | `RemoveComprehendLanguageEffect` | Removes magical language-comprehension effects |
| `removecurse` | `RemoveCurseEffect` | Removes matching magical curse effects |
| `removedetectethereal` | `RemoveDetectEtherealEffect` | Removes magical ethereal perception effects |
| `removedetectinvisible` | `RemoveDetectInvisibleEffect` | Removes magical detect-invisible effects |
| `removedetectmagick` | `RemoveDetectMagickEffect` | Removes magical detect-magick effects |
| `removedisease` | `RemoveDiseaseEffect` | Removes a matching spell-owned disease payload |
| `removefear` | `RemoveFearEffect` | Removes magical fear effects |
| `removeflying` | `RemoveFlyingEffect` | Removes magical flight-granting effects |
| `removeinvisibility` | `RemoveInvisibilityEffect` | Removes spell-owned invisibility effects without removing unrelated spell effects |
| `removeinfravision` | `RemoveInfravisionEffect` | Removes magical infravision effects |
| `removeinsomnia` | `RemoveInsomniaEffect` | Removes magical insomnia effects |
| `removemagictag` | `RemoveMagicTagEffect` | Removes matching spell-owned magic tags by key and optional value |
| `removeparalysis` | `RemoveParalysisEffect` | Removes magical paralysis effects |
| `removeplanarstate` | `RemovePlanarStateSpellEffect` | Removes spell-owned and saved planar-state overlays |
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
| `roomtagward` | `RoomTagWardEffect` | Applies a room ward that fails or reflects matching incoming or outgoing magic by `magictag` key/value |
| `roomtemperature` | `RoomTemperatureEffect` | Alters room temperature |
| `selfdamage` | `SelfDamageEffect` | Damages the caster |
| `seizebody` | `SeizeBodySpellEffect` | Temporarily transfers caster command into a living target body, with PC spectator context and NPC control restoration |
| `silence` | `SilenceEffect` | Applies vocal silence without blocking telepathy |
| `sleep` | `SleepEffect` | Forces magical sleep |
| `spellarmour` | `SpellArmourEffect` | Applies spell-owned magical armour using the shared armour configuration |
| `staminadelta` | `StaminaDeltaSpellEffect` | Changes stamina immediately |
| `staminaexpendrate` | `StaminaExpenditureSpellEffect` | Alters stamina expenditure rate |
| `staminaregenrate` | `StaminaRegenRateSpellEffect` | Alters stamina regeneration rate |
| `telepathy` | `TelepathySpellEffect` | Applies telepathic linkage |
| `teleport` | `TeleportEffect` | Teleports the caster to a room or cell target |
| `teleporttarget` | `TeleportTargetEffect` | Teleports a target selected by the spell |
| `trackmark` | `TrackMarkEffect` | Alters a character's future visual and olfactory tracks and can mark created tracks with a magical trace |
| `tracktrail` | `TrackMarkEffect` | Builder/load alias for `trackmark` |
| `transference` | `TransferenceEffect` | Swaps the caster and target character locations, optionally including followers and room layers |
| `subjectivedesc` | `SubjectiveDescriptionEffect` | Adds audience-scoped subjective full-description replacement with priority, optional clan/viewer-prog gates, and optional illusion key |
| `subjectivesdesc` | `SubjectiveSDescEffect` | Adds audience-scoped subjective short-description replacement with priority, optional clan/viewer-prog gates, and optional illusion key |
| `transformform` | `TransformFormEffect` | Ensures or reuses a keyed alternate body form and applies a priority-ranked forced transformation demand |
| `waterbreathing` | `WaterBreathingEffect` | Grants additional breathable fluids |
| `weatherchange` | `WeatherChangeEffect` | Changes weather |
| `weatherchangefreeze` | `WeatherChangeFreezeEffect` | Changes and freezes weather state |
| `weatherfreeze` | `WeatherFreezeEffect` | Freezes weather state |
| `weight` | `WeightSpellEffect` | Alters weight |

## Ward Effects
`roomward` and `personalward` are the reusable school-based spell-authored interdiction primitives. `roomtagward` and `personaltagward` are the tag-aware variants for magic that carries `magictag` metadata.

School wards support these builder commands:

- `school <school>`
- `mode fail|reflect`
- `coverage incoming|outgoing|both`
- `subschools`
- `prog <prog>|none`

Their optional custom prog can use either of these signatures:

- `(character source, perceivable owner) -> bool`
- `(character source, perceivable owner, magicschool school) -> bool`

Tag wards support these builder commands:

- `tag <tag> [value]`
- `value <value|none>`
- `mode fail|reflect`
- `coverage incoming|outgoing|both`
- `prog <prog>|none`

Their optional custom prog can use either of these signatures:

- `(character source, perceivable owner) -> bool`
- `(character source, perceivable owner, text tag, text value) -> bool`

Reflection is intentionally narrow:

- only ordinary `character` spell casts retarget back to the caster
- all other spell target types downgrade `reflect` to `fail`

## Important Current-State Notes
- Spells are more builder-composable than powers, but they still rely on registered C# trigger and effect implementations.
- The trigger/effect registries are the extension point; there is no fully script-defined spell-effect system.
- Readiness validation is a major part of spell authoring. If a spell is incomplete, it will show a builder error rather than quietly misbehaving.
- Caster effects are separate from ordinary effects and apply to the caster after the target-side application path.
- Target-side spell application now checks room, personal, room-tag, and personal-tag wards after the casting emote and before any target-side effect is applied.
- Status-style spell effects are not currently modelled as one generic "status enum" template. The builder-visible Phase 1 statuses are intentionally separate effect types so XML shape, help text, and runtime semantics can differ cleanly per effect.

## Related Reading
- [Magic System Overview](./Magic_System_Overview.md)
- [Magic System: Capabilities, Resources, and Generators](./Magic_System_Capabilities_Resources_and_Generators.md)
- [Magic System: Powers](./Magic_System_Powers.md)
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-trigger-types)
- [Magic System: Implemented Types](./Magic_System_Implemented_Types.md#spell-effect-types)
