# Armageddon Magic and Psionics Gap Report

## Scope

This report compares the Armageddon spell and psionic inventory in:

- `C:/Users/luker/Downloads/armageddon_magic_psionics_reference_second_pass.md`
- `C:/Users/luker/Downloads/codedump.c`

against the current FutureMUD magic subsystem as documented in:

- `Design Documents/Magic_System_Implemented_Types.md`
- `Design Documents/Magic_System_Spells.md`
- `Design Documents/Magic_System_Powers.md`

and the current runtime implementations under `MudSharpCore/Magic`.

## Counting Rules

- `Native now` means the behaviour can be authored today with existing spell effects or power types and no new C# runtime type.
- `Builder+Prog now` means the behaviour can be reached today with existing magic primitives plus FutureProg logic, NPC/item prototypes, or on-load content scaffolding. These are "close enough" paths, not always exact one-to-one reproductions.
- `Needs engine work` means the current magic surface is missing a required trigger, target type, spell effect, power type, or interception hook.
- Existing powers count as valid coverage even when Armageddon exposed the original ability as a spell. If you want strict `cast`-spell parity rather than "same subsystem can do it", several defensive entries would slide from `Native now` to `Needs engine work`.
- A handful of Armageddon entries are under-specified in the dump (`Daylight`, `Empower`, `Drown`, `Cause Disease`, `Acid Spray`, some passive psionics). I counted those conservatively unless the name clearly maps to an existing FutureMUD primitive.
- Status update as of 2026-04-21: the Phase 1 implementation slice from this report has now shipped. `teleport` accepts `room` / `rooms` triggers, the reusable Phase 1 statuses are implemented as standalone spell-effect templates with matching removals, `MagicResourceDeltaEffect`, `SpellArmourEffect`, and `roomflag` / `removeroomflag` are live, and the underlying runtime hooks for additive perception grants, selective poison or disease cleanup, and early room flags are in place.
- Status update as of 2026-05-01: the plane/corporeality and multiple-body-form work changes the blocker picture again. `IPlane`, `PlanarPresenceDefinition`, `planarstate`, `planeshift`, `removeplanarstate`, planar FutureProg functions, the `corporeality` admin command, and the `transformform` spell effect are live. Simple ethereal states, noncorporeal manifestation, single-active-body transformation, and straightforward "move this target to another plane" spells are now buildable with first-class primitives. Simultaneous bodies, remote vessels, possessed corpses, descriptor handoff, and persistent portal topology remain blocked.
- Status update as of 2026-05-01 Engine V1: generic magic tags, FutureProg tag queries, first-class item and corpse magic effects, transient paired magical portals, safe command-forcing, and caster-scoped subjective description overrides are now implemented as engine primitives. This moves marks, rune anchors, basic portals, item damage/destruction/enchantment, corpse preservation/consumption/spawn, and the safest coercion/subjective-description cases out of the blocked bucket. Persistent portal topology, general dispel, true possession/projection, passive psionic traffic, and identity-concealment systems remain open.
- Status update as of 2026-05-02 Engine V2: `dispelmagic`, `mindconceal`, transient portal inspection, item/object portal anchors, and first-class item enchantment hooks for projectile payloads, crafting tools, power/fuel modifiers, and item event progs are now live. Passive psionic traffic can use the existing `telepathy` flow with `mindconceal` identity policy. Persistent gates still use saved effects rather than a dedicated gate table, and true simultaneous-body possession/projection remains deferred.

> Note: the top-line parity counts and family summary in this report predate the Phase 1, Phase 2, plane/corporeality, and body-form implementation work. If exact current counts are needed, rerun the family-by-family classification pass. The implementation-plan and primitive-gap sections below reflect the current runtime state.

## Executive Summary

| Inventory | Native now | Builder+Prog now | Needs engine work |
| --- | ---: | ---: | ---: |
| Magic (152) | 43 | 49 | 60 |
| Psionics (37) | 10 | 8 | 19 |
| Total (189) | 53 | 57 | 79 |

Key takeaways:

- `110 / 189` Armageddon entries are reachable today if we allow ordinary FutureProg and prototype scaffolding.
- The current system is already strong at direct damage, healing, stamina/need adjustment, item or liquid conjuration, NPC summoning, invisibility, telepathy, self-only magical armour, planar state shifts, and single-active-body transformation.
- Phase 2 now closes three medium-difficulty primitive gaps: local exit targeting, prog-resolved summon-style remote targeting, and reusable room or personal wards with shared spell and power interception.
- The plane and body-form work moves several old blockers into the buildable bucket: `Ethereal`, `Detect Ethereal`, `Dispel Ethereal`, simple `Planeshift`, ghostly manifestation, and polymorph-style transformations can now use first-class effects rather than bespoke tags.
- The biggest remaining parity blockers are durable portal topology beyond saved effects, richer illusion stacking, advanced coercion policy, and true "dual body" mechanics like possession or shadow projection.
- Psionics are only partially covered today. The current mind-link stack handles contact, barriers, mind-looking, audits, expulsion, sense, messaging, direct mental attacks, passive thought/feeling traffic, and identity concealment, but advanced coercion, trace consequences, and projection-style powers still need new runtime support.

## Family Summary

| Family | Native now | Builder+Prog now | Needs engine work | Main blocker theme |
| --- | ---: | ---: | ---: | --- |
| Fire | 5 | 6 | 8 | Detection, dispels, object wards, burning-over-time |
| Water | 11 | 7 | 6 | Poison/disease lifecycle, silence, water-breathing |
| Earth / Stone | 7 | 8 | 7 | Sleep, burrow rooms, delayed callbacks, item destruction |
| Wind | 5 | 11 | 7 | Flying, long-range forced movement, detection or cleanse effects |
| Shadow | 3 | 5 | 9 | Ethereal state, anti-curse cleanses, fear, projection |
| Lightning | 5 | 7 | 3 | Sleep immunity, paralysis, tracked footprint effects |
| Void | 6 | 5 | 17 | Durable portal networks, possession, advanced corpse/vessel semantics, resource drain |
| Unspecified / incomplete magic | 1 | 0 | 3 | Source material is incomplete |
| Psionics | 10 | 8 | 19 | Advanced coercion policy, trace consequences, projection/remote-presence semantics |

## Where FutureMUD Is Already Strong

The current system already has good coverage for:

- direct hostile spells through `damage`, `selfdamage`, `staminadelta`, and `magicattack`
- curative and restorative spells through `heal`, `healingrate`, `mend`, `staminaregenrate`, `staminaexpendrate`, and `needdelta`
- conjuration through `createitem`, `createliquid`, and `createnpc`
- core telepathy through `connectmind`, `mindlook`, `mindaudit`, `mindexpel`, `mindsay`, `mindbroadcast`, and `sense`
- self-only visible protective magic through `armour`
- local exit-targeted barriers and shove-style movement through `exitbarrier` and `forcedexitmovement`
- school-based room and personal wards through `roomward` and `personalward`
- prog-resolved remote character or item targeting for summon-style spells through `progcharacter`, `progitem`, `progcharacterroom`, and `progitemroom`
- room ambience changes through `roomlight`, `roomtemperature`, `roomatmosphere`, and weather effects
- planar state changes through `planarstate`, `planeshift`, `removeplanarstate`, planar merits, planar drugs, and planar FutureProg helpers
- spell-driven alternate body forms through `transformform`, including stable form keys, first-creation race or description defaults, trauma handling, transformation echoes, and forced-transformation priority
- information-bearing spell metadata through `magictag` / `removemagictag` and FutureProg helpers `hasmagictag`, `magictagvalue`, `magictagvalues`, and `magictags`
- item/corpse magic through `itemdamage`, `destroyitem`, `itemenchant`, `corpsemark`, `corpsepreserve`, `corpseconsume`, and `corpsespawn`, including projectile, craft-tool, powered-item, fuel-use, and item event hooks for enchantments
- general spell cleanup through `dispelmagic`, including remove or shorten modes and criteria for caster policy, spell, school/subschool, magic tags, and approved effect/interface keys
- transient paired magical portals through `portal`, backed by effect-owned exit-manager registration rather than permanent database exits, with active inspection and caster-owned room or item/object anchors
- psionic identity concealment and passive thought/feeling traffic through `mindconceal` and the existing `telepathy` flow
- Coercion V1 through `forcecommand`, `subjectivedesc`, and `subjectivesdesc`

## Current Reclassification From Planes And Body Forms

The old blocker list bundled "ethereal", "projection", "possession", "planeshift", and "shape change" together. The live runtime now supports some of those as first-class mechanics, but not all of them.

| Old blocker theme | Current path forward | Still blocked |
| --- | --- | --- |
| Ethereal or noncorporeal state | Use `planarstate` / `planeshift` on characters, items, or other perceivables. Plane data handles room presentation, remote observation tags, perception, interaction checks, noncorporeal physiology, inventory propagation, and closed-door bypass where configured. | Plane-specific travel graphs, custom transition trauma, and any spell that requires a separate acting shell rather than changing the target's own planar presence. |
| Detect or dispel ethereal | Use `detectethereal` / `removedetectethereal` for perception grants, `removeplanarstate` for spell-owned or saved planar overlays, or `dispelmagic effect planarstate` when the spell should share general anti-magic rules. | Strength-contested dispel math beyond the existing spell/resist/ward flow. |
| Planeshift | Simple "target moves to configured plane/state" is now first-class with `planeshift`. | Multi-step planar travel with unsafe destinations or durable portal networks that should survive beyond an effect duration. |
| Shadowwalk-style movement | If the intended behaviour is "enter a shadow/astral/ethereal plane and move normally", use plane definitions plus `planeshift`. | If the intended behaviour is remote projection, leaving a body behind, or moving a second body independently, it remains blocked. |
| Polymorph, animal form, statue-like form, or spirit form | Use `transformform` for single-active-body transformation, with `Additional Body Form` merits for intrinsic or racial forms. | Turning a character into a true item, making two bodies act at once, using a corpse as the exact vessel, or continuously syncing spell XML to later form metadata. |
| Possession, disembodying, and send-shadow projection | Use `planeshift` or `transformform` only for simplified content where the original character becomes the new form/state. | True possession and projection still need simultaneous presence, command routing, source-body vulnerability, disconnect handling, staff visibility, and death semantics. |
| Marks, runes, anchors, and hidden magical facts | Use `magictag` / `removemagictag` plus the magic-tag FutureProg helpers for information-bearing metadata. The `portal` effect can consume caster-owned room or item/object anchors, and active portal/anchor state is inspectable with `magic portals` and `magic anchors`. | Behavioural effects should still get first-class support when the tag would be pretending to be combat, movement, durable portal topology, item damage, resource changes, or perception. |

## Main Gaps By Primitive

### 1. Reusable status application and removal

This was the biggest easy-win cluster, and it is now the main area where the current runtime moved forward.

Phase 1 shipped standalone spell-effect templates and matching standalone removal templates for:

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

These are intentionally separate builder-visible types rather than a single enum-driven status template. `poison` and `disease` are also spell-owned payloads with origin metadata so matching removers only clear the payload created by the configured spell effect.

The remaining gap inside this primitive family is now the unimplemented edge set rather than the basic reusable states:

- `feather fall`
- `detect poison`
- `insomnia`
- cure blindness
- strength-contested dispel math beyond the current `dispelmagic` criteria model

### 2. Magic and psionic resource delta effects

Armageddon uses mana-like and psionic-resource drains in several places.

Phase 1 shipped `MagicResourceDeltaEffect`, which works against the existing `IHaveMagicResource` abstraction and clamps character, item, and room resources to `[0, cap]`.

This unlocks the generic primitive needed for:

- `Feeblemind`
- `Aura Drain`
- `Dragon Drain`
- `Psionic Drain`
- parts of `Mindwipe`

The remaining work here is mostly spell-specific packaging and parity details rather than the absence of a first-class resource delta effect.

### 3. Exit and direction targeting

Status: implemented in the current runtime.

FutureMUD now has:

- `exit`
- `characterexit`
- `exitbarrier`
- `forcedexitmovement`

That cleanly covers the common "choose a local exit", "put a wall on that exit", and "shove the target through that exit" patterns.

This now unlocks:

- `Wall Of Fire`
- `Wall Of Thorns`
- `Wall Of Sand`
- `Wall Of Wind`
- `Blade Barrier`
- `Repel`

Remaining limitation:

- there is still no first-class `OpenOrClosedExitImpact` primitive, so exit state mutation beyond blocking passage still needs bespoke work

### 4. World-target movement, planar movement, and swap effects

Status: partially implemented in the current runtime.

The current trigger set now includes:

- `progcharacter`
- `progitem`
- `progcharacterroom`
- `progitemroom`

These close the cleanest summon-style gap by letting a spell resolve a remote character or item through a prog and, when needed, also provide a prog-resolved room for existing room-parameter spell effects such as `teleporttarget`.

This now unlocks or materially improves:

- `Summon`
- parts of `Travel Gate`
- parts of `Portal`
- simple `Planeshift` when the desired behaviour is a planar overlay on the target rather than a portal network

This still blocks or complicates:

- `Hands Of Wind` if it needs more than "move target to chosen room" semantics
- `Transference`
- persistent paired-gate topology for `Travel Gate` and `Portal`; transient paired exits and caster-owned room-tag anchors are now live

### 5. Room wards and anti-magic / anti-psionics interception

Status: implemented in the current runtime.

FutureMUD now has generic `roomward` and `personalward` spell effects backed by shared interception hooks that both spells and targeted powers consult.

Current coverage includes:

- school-based matching with optional child-school inclusion
- `incoming`, `outgoing`, or `both` coverage
- `fail` or `reflect` modes
- optional custom progs

This now unlocks:

- `Psionic Suppression`
- `Shield Of Nilaz`
- `Forbid Elements`
- `Turn Element`
- `Elemental Fog`
- `Dome`

Remaining limitation:

- wards are school-based rather than freeform tag-based, so future item- or rune-specific anti-magic still wants a deeper enchantment layer

### 6. Item and corpse enchantment, magic tags, and anchors

Status: implemented for Engine V2 metadata, destructive item magic, first-class item enchantment hooks, portal anchors, and corpse lifecycle helpers.

FutureMUD now has a generic information-bearing magic-tag effect. It is explicitly metadata, not a universal behaviour substitute:

- `magictag` / `removemagictag` attach spell-owned key/value metadata to characters, items, rooms, and other perceivables.
- The active effect exposes `Tag`, `Value`, `Caster`, and `Spell` through `IMagicTagEffect`.
- FutureProg helpers `hasmagictag`, `magictagvalue`, `magictagvalues`, and `magictags` let progs query anchors, rune keys, ritual state, signatures, and bespoke conditional effects.
- `portal` can use caster-owned magic-tag room anchors or item/object anchors as destination modes.

This is appropriate for:

- `Mark`
- `Create Rune`
- anchor state for later `Portal` or `Travel Gate`
- ritual prerequisites
- one-off builder-authored information read by a prog
- "this corpse has been spoken to" or "this item bears a magical signature" style facts

First-class item and corpse effects now cover the common runtime behaviours that should not be faked with tags:

- `itemdamage` applies normal item wound/health damage.
- `destroyitem` deletes item targets with purge-warning safeguards.
- `itemenchant` supplies visible aura text, optional glow, conditional-prog gating, weapon/armour combat hooks, projectile payload bonuses, craft-tool bonuses, power/fuel modifiers, and optional item event progs through first-class interfaces.
- `corpsemark` marks corpse items with magic metadata.
- `corpsepreserve` pauses corpse decay through the corpse heartbeat.
- `corpseconsume` cleanly deletes corpse items.
- `corpsespawn` loads configured NPCs or items at the corpse location and can optionally consume the corpse.

This now unlocks or materially improves:

- `Glyph`
- `Mark`
- `Vampiric Blade`
- `Rot Items`
- `Shatter`
- `Animate Dead`
- `Pseudo Death`
- `Hero Sword`
- `Sand Statue`

Remaining limitations:

- `dispelmagic` matches by authored criteria and uses the normal spell/ward/resist flow; there is not yet a separate strength-contested dispel formula layer.
- Persistent runes, standing portals, and durable world-topology edits should not be modelled as only temporary magic tags. Engine V2 keeps gates/runes as saved effects and transient exits rather than adding a gate table or permanent database exits.

### 7. Body transformation, dual-body, possession, and projection mechanics

Status: partially unblocked.

The `transformform` spell effect and the multiple-body-form system now give FutureMUD a first-class answer for temporary or persistent single-active-body transformations. This supports spells that can honestly be represented as "the character becomes a different body for a duration" rather than "the character controls another body while the original remains elsewhere."

This now unlocks or materially improves:

- polymorph-style magic
- animal, monster, elemental, ghost, or spirit body forms
- some simplified `Pseudo Death` or `Sand Statue` interpretations if they are authored as a body form rather than a true corpse/item state
- shadow or astral form spells where the caster becomes that form

The remaining hard cases are still not just timed effects. They need a coherent answer for agency, perception, inventory, death, disconnects, source-body vulnerability, and admin visibility.

This blocks:

- `Send Shadow`
- `Shadowwalk`
- `Possess Corpse`
- `Disembody`
- `Burrow` to a lesser extent
- pieces of `Portal` and `Planeshift`

`Shadowwalk`, `Disembody`, and `Planeshift` should be split by intended semantics. If they mean "change this target's planar presence", they are now buildable. If they mean "leave one body behind and operate another", they remain blocked by simultaneous-body mechanics.

### 8. Subjective perception and coercive psionics

Status: Coercion V1 is implemented, but deeper psionic identity and traffic systems remain open.

FutureMUD already has good mind-link primitives and now has:

- `forcecommand`, which runs as the target's own command context through `ExecuteCommand`, respects `IIgnoreForceEffect`, blocks staff/editor/account-destructive command roots, and emits wiz-only audit output.
- `subjectivedesc`, which applies a spell-owned full-description override.
- `subjectivesdesc`, which applies a spell-owned short-description override.
- caster-scoped subjective-description support through fixed-viewer handling.

This now covers the safest command-forcing and "one viewer sees a different description" cases. It does not yet cover:

- injecting emotions or compulsions
- hiding a psionic identity from trace/contact flows
- passive thought-traffic/eavesdropping powers
- robust refusal/consent policy beyond the hard safety block list
- illusion stacking policy beyond ordinary override-description precedence

This still blocks or complicates:

- `Conceal`
- `Masquerade`
- `Imitate`
- `Vanish`
- `Thoughtsense`
- `Immersion`

`Control`, `Compel`, `Suggest`, and `Coerce` are now buildable for non-staff, non-account-destructive command roots, but content authors should still treat them as policy-sensitive spells and pair them with clear messaging or setting rules.

## Prioritised Implementation Plan

### Phase 1: Easy Wins

Status: completed on 2026-04-21.

These are the changes with the best "entries unlocked per unit of work" ratio.

1. Fix the current teleport spell-effect mismatch.
   - Completed by updating `TeleportEffect` compatibility to `room` / `rooms` so it now lines up with its `ICell` target handling.

2. Add reusable Phase 1 statuses and removals.
   - Completed with standalone builder-visible spell-effect templates and standalone runtime effects for `silence`, `sleep`, `fear`, `paralysis`, `flying`, `waterbreathing`, `poison`, `disease`, `curse`, `detectinvisible`, `detectethereal`, `detectmagick`, `infravision`, and `comprehendlanguage`, plus the matching `remove...` spell effects.
   - This was implemented as discrete effect types rather than a single generic status enum so builder help, XML shape, and runtime semantics can stay explicit per status.

3. Add `MagicResourceDeltaEffect`.
   - Completed against the existing magic-resource abstraction, with resource mutations clamped to the holder's valid range.

4. Add a spell-effect equivalent of the current `MagicArmourPower`.
   - Completed by extracting `MagicArmourConfiguration` and using it from both `MagicArmourPower` and the new `SpellArmourEffect`.

5. Add a generic "room flag" effect and matching removal/dispel support.
   - Completed with `roomflag` / `removeroomflag` for `peaceful`, `nodream`, `alarm`, `darkness`, and `wardtag`.

### Phase 2: Medium-Difficulty Primitives

These are the next-best return once the basic statuses exist.

1. Add exit-or-direction targeting.
   - Completed in the current runtime.
   - FutureMUD now has `exit`, `characterexit`, `exitbarrier`, and `forcedexitmovement`.
   - This unlocks all wall spells plus `Repel`.

2. Add better remote targeting.
   - Partially completed in the current runtime.
   - FutureMUD now has prog-resolved character and item triggers, plus prog-resolved room parameters for summon-style spells.
   - This is now the cleanest path for `Summon` and the room-targeted portions of richer gate logic, but not yet full swap or portal-topology behavior.

3. Add first-class item enchantment and item damage effects.
   - Completed through Engine V2 with `itemdamage`, `destroyitem`, expanded `itemenchant`, generic `magictag`, and corpse-specific helpers.
   - This unlocks or materially improves `Glyph`, `Mark`, `Vampiric Blade`, `Rot Items`, `Shatter`, and a cleaner path for corpse magic.

4. Add room wards and personal ward effects with interception hooks.
   - Completed in the current runtime as school-based wards with shared spell and power interception hooks.
   - This unlocks `Forbid Elements`, `Turn Element`, `Shield Of Nilaz`, `Elemental Fog`, and `Dome`.

5. Add a command-safe psionic coercion framework.
   - Completed for Coercion V1 with `forcecommand`.
   - The current implementation executes through the target's own `ExecuteCommand`, respects `IIgnoreForceEffect`, blocks staff/editor/account-destructive roots, and emits wiz-only audit output.
   - Deeper consent/refusal semantics, emotion injection, trace consequences, and non-command coercion remain future work.

### Phase 3: Tricky Design Work

These are the parity items with the most engine-level uncertainty.

1. Projection and possession.
   - `Send Shadow`
   - `Shadowwalk`
   - `Possess Corpse`
   - `Disembody`
   - Status: single-active-body transformations are now supported through `transformform`; true projection and possession remain blocked.
   - Design questions:
     - Is the projected self a second body, a descriptor handoff, or a temporary NPC shell?
     - What happens to inventory, combat, death, and disconnects?
     - How do staff see the true relationship between source body and projection?

2. Portal topology and marked-anchor travel.
   - `Mark`
   - `Travel Gate`
   - `Portal`
   - `Create Rune`
   - Status: simple room-target teleport, simple planar shifting, caster-owned room or item/object tag anchors, transient effect-owned paired exits, and active portal/anchor inspection are live.
   - Remaining work:
     - persistent paired gates, standing rune networks, and portal objects beyond saved effects
     - richer destination safety models and world-topology persistence

3. Subjective illusions and perception overrides.
   - `Masquerade`
   - `Illusion`
   - `Imitate`
   - `Vanish`
   - `Phantasm`
   - `Mirage`
   - `Delusion`
   - `Shadowplay`
   - `Illuminant`
   - Status: caster-scoped subjective short/full description overrides are live through `subjectivesdesc` and `subjectivedesc`.
   - Remaining work:
     - objective room-state illusions
     - multi-viewer or group-scoped illusions
     - identity masking in psionic contact/trace flows
     - explicit stacking and priority rules beyond the existing override-description effect ordering

4. World-model-specific metaphysics.
   - `Determine Relationship`
   - `Solace`
   - `Dragon Bane`
   - `Planeshift`
   - `Cathexis`
   - These are only partly magic-system problems. They also require a clean model for "relationship to the land", elemental planes, and any clan- or tribe-keyed psionic identity mechanics.

## Engine V2 Shipped Runtime Slice

Engine V2 has shipped the deeper parity layer without jumping into simultaneous-body possession.

1. General dispel and effect shortening.
   - `dispelmagic` can remove or shorten matching spell-parent effects.
   - Matching supports specific spell, school, child school, caster policy, magic tag key/value, and approved effect/interface keys.
   - The default is caster-owned cleanup. Hostile dispels must opt in and then use the ordinary spell targeting, ward, and resistance flow.

2. Saved-effect portal topology.
   - `portal` still creates effect-owned transient exits, not permanent database exits.
   - Active portals are inspectable through `magic portals`, backed by `IExitManager.TransientExits` and `IMagicPortalExit`.
   - Active anchors are inspectable through `magic anchors [tag]`.
   - Anchor resolution now supports caster-owned room tags and caster-owned item/object tags, using the tagged item's current location.

3. Broader item enchantment hooks.
   - `itemenchant` now exposes first-class projectile/ranged payload bonuses, craft-tool bonuses, power/fuel modifiers, and optional item event progs.
   - These behaviours should not be authored as generic magic tags except where a tag is only supporting metadata.

4. Plane/form recipes and regression coverage.
   - `Ethereal`, `Dispel Ethereal`, `Planeshift`, shadow/astral walking, and polymorph-style configurations are covered by builder-loadable recipe tests.
   - The intended primitives are `planarstate`, `removeplanarstate`, `planeshift`, `dispelmagic`, and `transformform`.

5. Psionic identity and passive traffic.
   - `mindconceal` supplies the sustained identity-concealment effect and audit difficulty modifier.
   - `connectmind`, `mindsay`, `mindbroadcast`, `mindaudit`, `mindexpel`, and passive `think`/`feel` telepathy now consult the shared concealment contract.
   - Thoughtsense/Immersion-style passive traffic should use the existing `telepathy` `thinks` / `feels` / `thinkemote` flow.

6. Still deferred.
   - True possession, send-shadow projection, and body-left-behind disembodiment remain out of scope until the simultaneous-body model is designed.
   - The existing form system deliberately supports one active body. Possession/projection still needs command routing, remote presence, body vulnerability, inventory rules, death rules, reconnect behavior, and admin observability.

## Recommended Next Shipping Slice After Engine V2

The next work should focus on the pieces Engine V2 deliberately left outside the slice:

- durable portal/rune topology if saved effects and transient exits are not enough for standing gate networks
- strength-contested dispel math if author criteria plus normal spell resistance is not enough
- richer illusion stacking and perception policy
- advanced psionic coercion and trace consequences
- the simultaneous-body possession/projection model

## Appendix: Classification By Family

This appendix is the historical family-by-family classification from the first pass. Use the current-runtime sections above for planning decisions until the exact counts are refreshed.

### Fire

- Native now: `Fireball`, `Flamestrike`, `Demonfire`, `Fire Armor`, `Wall Of Fire`
- Builder+Prog now: `Rain Of Fire`, `Ball Of Light`, `Pyrotechnics`, `Parch`, `Fire Jambiya`, `Fire Seed`
- Needs engine work: `Detect Magick`, `Dispel Magick`, `Empower`, `Glyph`, `Tongues`, `Firebreather`, `Daylight`, `Immolate`

### Water

- Native now: `Create Water`, `Heal`, `Sanctuary`, `Calm`, `Invulnerability`, `Deafness`, `Health Drain`, `Create Wine`, `Intoxication`, `Sober`, `Wall Of Thorns`
- Builder+Prog now: `Oasis`, `Determine Relationship`, `Thunder`, `Wither`, `Mirage`, `Shield Of Mist`, `Healing Mud`
- Needs engine work: `Detect Poison`, `Poison`, `Cure Poison`, `Silence`, `Cure Disease`, `Breathe Water`

### Earth / Stone

- Native now: `Armor`, `Earthquake`, `Strength`, `Weaken`, `Fury`, `Repair Item`, `Wall Of Sand`
- Builder+Prog now: `Create Food`, `Sand Jambiya`, `Stone Skin`, `Show The Path`, `Mount`, `Godspeed`, `Sand Shelter`, `Golem`
- Needs engine work: `Sleep`, `Burrow`, `Feeblemind`, `Rewind`, `Alarm`, `Sand Statue`, `Shatter`

### Wind

- Native now: `Invisibility`, `Wind Armor`, `Wind Fist`, `Repel`, `Wall Of Wind`
- Builder+Prog now: `Teleport`, `Relocate`, `Sandstorm`, `Banishment`, `Guardian`, `Stalker`, `Delusion`, `Shield Of Wind`, `Messenger`, `Create Rune`, `Summon`
- Needs engine work: `Detect Invisible`, `Levitate`, `Hands Of Wind`, `Transference`, `Fly`, `Feather Fall`, `Dispel Invisibility`

### Shadow

- Native now: `Blind`, `Create Darkness`, `Shadow Armor`
- Builder+Prog now: `Restful Shade`, `Curse`, `Haunt`, `Shadow Sword`, `Shadowplay`
- Needs engine work: `Cure Blindness`, `Remove Curse`, `Fear`, `Infravision`, `Send Shadow`, `Ethereal`, `Detect Ethereal`, `Dispel Ethereal`, `Hero Sword`

### Lightning

- Native now: `Lightning Bolt`, `Refresh`, `Stamina Drain`, `Energy Shield`, `Regenerate`
- Builder+Prog now: `Slow`, `Chain Lightning`, `Lightning Storm`, `Quickening`, `Lightning Whip`, `Illuminant`, `Lightning Spear`
- Needs engine work: `Insomnia`, `Paralyze`, `Fluorescent Footsteps`

### Void

- Native now: `Psionic Suppression`, `Shield Of Nilaz`, `Forbid Elements`, `Turn Element`, `Elemental Fog`, `Blade Barrier`
- Builder+Prog now: `Gate`, `Pseudo Death`, `Dragon Bane`, `Portable Hole`, `Phantasm`
- Needs engine work: `Animate Dead`, `Mark`, `Dragon Drain`, `Charm Person`, `Solace`, `Aura Drain`, `Travel Gate`, `Planeshift`, `Possess Corpse`, `Portal`, `Identify`, `Psionic Drain`, `Disembody`, `Rot Items`, `Vampiric Blade`, `Dead Speak`, `Recite`

### Unspecified / Incomplete Magic

- Native now: `Puddle`
- Builder+Prog now: none
- Needs engine work or clearer source detail: `Drown`, `Cause Disease`, `Acid Spray`

### Psionics

- Native now: `Contact`, `Barrier`, `Locate`, `Probe`, `Expel`, `Sense Presence`, `Mindblast`, `Mesmerize`, `Rejuvenate`, `Dome`
- Builder+Prog now: `Empathy`, `Masquerade`, `Illusion`, `Disorient`, `Clairvoyance`, `Imitate`, `Project`, `Vanish`
- Needs engine work: `Trace`, `Cathexis`, `Allspeak`, `Mindwipe`, `Shadowwalk`, `Hear`, `Control`, `Compel`, `Conceal`, `Clairaudience`, `Suggest`, `Babble`, `Coerce`, `Magicksense`, `Thoughtsense`, `Beast Affinity`, `Wild Contact`, `Wild Barrier`, `Immersion`
