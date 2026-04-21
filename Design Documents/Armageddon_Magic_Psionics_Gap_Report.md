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

> Note: the top-line parity counts in this report predate the Phase 1 implementation work. If exact current counts are needed, rerun the family-by-family classification pass. The implementation-plan and primitive-gap sections below reflect the current runtime state.

## Executive Summary

| Inventory | Native now | Builder+Prog now | Needs engine work |
| --- | ---: | ---: | ---: |
| Magic (152) | 32 | 48 | 72 |
| Psionics (37) | 9 | 8 | 20 |
| Total (189) | 41 | 56 | 92 |

Key takeaways:

- `97 / 189` Armageddon entries are reachable today if we allow ordinary FutureProg and prototype scaffolding.
- The current system is already strong at direct damage, healing, stamina/need adjustment, item or liquid conjuration, NPC summoning, invisibility, telepathy, and self-only magical armour.
- The biggest parity blockers are reusable status effects, status removal, exit-or-direction targeting, room or character anti-magic wards, item/corpse enchantment, magic-resource drain, and "dual body" mechanics like possession or shadow projection.
- Psionics are only partially covered today. The current mind-link stack handles contact, barriers, mind-looking, audits, expulsion, sense, messaging, and direct mental attacks, but most coercion, concealment, remote eavesdropping, and passive-traffic powers still need new runtime support.

## Family Summary

| Family | Native now | Builder+Prog now | Needs engine work | Main blocker theme |
| --- | ---: | ---: | ---: | --- |
| Fire | 4 | 6 | 9 | Detection, dispels, object wards, burning-over-time |
| Water | 10 | 7 | 7 | Poison/disease lifecycle, silence, water-breathing |
| Earth / Stone | 6 | 8 | 8 | Sleep, burrow rooms, delayed callbacks, item destruction |
| Wind | 3 | 10 | 10 | World-target movement, flying, exit walls, shove effects |
| Shadow | 3 | 5 | 9 | Ethereal state, anti-curse cleanses, fear, projection |
| Lightning | 5 | 7 | 3 | Sleep immunity, paralysis, tracked footprint effects |
| Void | 0 | 5 | 23 | Anti-magic, portals, possession, item/corpse enchantment |
| Unspecified / incomplete magic | 1 | 0 | 3 | Source material is incomplete |
| Psionics | 9 | 8 | 20 | Command-forcing, concealment, room wards, passive traffic |

## Where FutureMUD Is Already Strong

The current system already has good coverage for:

- direct hostile spells through `damage`, `selfdamage`, `staminadelta`, and `magicattack`
- curative and restorative spells through `heal`, `healingrate`, `mend`, `staminaregenrate`, `staminaexpendrate`, and `needdelta`
- conjuration through `createitem`, `createliquid`, and `createnpc`
- core telepathy through `connectmind`, `mindlook`, `mindaudit`, `mindexpel`, `mindsay`, `mindbroadcast`, and `sense`
- self-only visible protective magic through `armour`
- room ambience changes through `roomlight`, `roomtemperature`, `roomatmosphere`, and weather effects

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
- general dispel / effect shortening

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

Armageddon has a lot of "choose an exit" or "force movement through an exit" magic. Current FutureMUD spell triggers do not expose an exit or direction target.

This blocks or complicates:

- `Wall Of Fire`
- `Wall Of Thorns`
- `Wall Of Sand`
- `Wall Of Wind`
- `Blade Barrier`
- `Repel`
- the shove rider on `Sandstorm`

### 4. World-target movement and swap effects

The current trigger set is good at self, same-room character, room, room-via-prog, and character-plus-room. It is weak at "pick a remote character anywhere meaningful and then move them, pull them, or swap with them".

This blocks or complicates:

- `Summon`
- `Hands Of Wind`
- `Transference`
- parts of `Travel Gate`
- parts of `Portal`

### 5. Room wards and anti-magic / anti-psionics interception

FutureMUD can currently cast spells into rooms, but it does not yet have a reusable interception layer for "magic of school X is blocked here" or "incoming psionics reflect or fail while this ward stands".

Phase 1 did ship an early `roomflag` / `removeroomflag` primitive covering `peaceful`, `nodream`, `alarm`, `darkness`, and `wardtag`. That is enough for early ambience and low-complexity room-state magic, but not yet for interception.

This blocks:

- `Psionic Suppression`
- `Shield Of Nilaz`
- `Forbid Elements`
- `Turn Element`
- `Elemental Fog`
- `Dome`

### 6. Item and corpse enchantment as first-class spell targets

Armageddon leans heavily on item-state mutation and corpse-state mutation. FutureMUD can create items and affect some item properties indirectly, but it does not have a generic enchant-or-tag-item spell effect family.

This blocks or complicates:

- `Glyph`
- `Mark`
- `Vampiric Blade`
- `Rot Items`
- `Shatter`
- `Animate Dead`
- `Pseudo Death`
- `Hero Sword`
- `Sand Statue`

### 7. Dual-body, possession, and projection mechanics

These are the hardest parity items. They are not just "apply a timed effect"; they need a coherent answer for agency, perception, inventory, death, disconnects, and admin visibility.

This blocks:

- `Send Shadow`
- `Shadowwalk`
- `Possess Corpse`
- `Disembody`
- `Burrow` to a lesser extent
- pieces of `Portal` and `Planeshift`

### 8. Subjective perception and coercive psionics

FutureMUD already has good mind-link primitives, but it does not yet have a reusable system for:

- forcing player commands
- injecting emotions or compulsions
- hiding a psionic identity from trace/contact flows
- giving one perceiver a fake appearance while leaving everyone else on the real one

This blocks:

- `Control`
- `Compel`
- `Suggest`
- `Coerce`
- `Conceal`
- `Masquerade`
- `Imitate`
- `Vanish`
- `Thoughtsense`
- `Immersion`

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
   - New trigger or target type for `exit` / `direction`.
   - New spell effects for `ExitBarrier`, `ForcedExitMovement`, and maybe `OpenOrClosedExitImpact`.
   - This unlocks all wall spells plus `Repel`.

2. Add better remote targeting.
   - A trigger that can resolve a world target character or item through a prog, not just a room.
   - This is the cleanest path for `Summon`, `Hands Of Wind`, `Transference`, richer gate logic, and some messenger or stalker variants.

3. Add first-class item enchantment and item damage effects.
   - `EnchantItemEffect`
   - `DamageItemEffect`
   - `TagItemEffect` or generic effect metadata on items/corpses
   - This unlocks `Glyph`, `Mark`, `Vampiric Blade`, `Rot Items`, `Shatter`, and a cleaner path for corpse magic.

4. Add room wards and personal ward effects with interception hooks.
   - This should be generic by school or tag, not hard-coded to a single elemental family.
   - This unlocks `Forbid Elements`, `Turn Element`, `Shield Of Nilaz`, `Elemental Fog`, and `Dome`.

5. Add a command-safe psionic coercion framework.
   - One reusable power family should cover same-room domination, contact-range compulsion, suggestion, and coercion.
   - It needs explicit policy hooks for player agency, logging, staff review, and refusal messages.

### Phase 3: Tricky Design Work

These are the parity items with the most engine-level uncertainty.

1. Projection and possession.
   - `Send Shadow`
   - `Shadowwalk`
   - `Possess Corpse`
   - `Disembody`
   - Design questions:
     - Is the projected self a second body, a descriptor handoff, or a temporary NPC shell?
     - What happens to inventory, combat, death, and disconnects?
     - How do staff see the true relationship between source body and projection?

2. Portal topology and marked-anchor travel.
   - `Mark`
   - `Travel Gate`
   - `Portal`
   - `Create Rune`
   - Design questions:
     - Are anchors objects, locations, or both?
     - Are portals represented as temporary exits, room effects, or paired perceivables?
     - How do you validate destination safety, plane compatibility, and persistence?

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
   - Design questions:
     - Is the illusion objective room state or per-viewer filtered state?
     - Do effects target a room, a perceiver, or a perceivable as seen by one perceiver?
     - How should stacked illusions resolve?

4. World-model-specific metaphysics.
   - `Determine Relationship`
   - `Solace`
   - `Dragon Bane`
   - `Planeshift`
   - `Cathexis`
   - These are only partly magic-system problems. They also require a clean model for "relationship to the land", elemental planes, and any clan- or tribe-keyed psionic identity mechanics.

## Recommended Next Shipping Slice

Phase 1 is complete. If the goal is now to keep maximising "Armageddon-feeling parity" quickly, the next recommended order is:

1. Exit targeting plus forced movement.
2. Item enchantment and corpse-tagging.
3. Room wards and anti-magic interception.
4. Better remote targeting for movement and swap effects.
5. Psionic command/control framework.
6. Projection, possession, and portal topology.

That order keeps chasing the highest-yield missing primitives now that the foundational Phase 1 spell building blocks are online.

## Appendix: Classification By Family

### Fire

- Native now: `Fireball`, `Flamestrike`, `Demonfire`, `Fire Armor`
- Builder+Prog now: `Rain Of Fire`, `Ball Of Light`, `Pyrotechnics`, `Parch`, `Fire Jambiya`, `Fire Seed`
- Needs engine work: `Detect Magick`, `Dispel Magick`, `Empower`, `Glyph`, `Tongues`, `Firebreather`, `Wall Of Fire`, `Daylight`, `Immolate`

### Water

- Native now: `Create Water`, `Heal`, `Sanctuary`, `Calm`, `Invulnerability`, `Deafness`, `Health Drain`, `Create Wine`, `Intoxication`, `Sober`
- Builder+Prog now: `Oasis`, `Determine Relationship`, `Thunder`, `Wither`, `Mirage`, `Shield Of Mist`, `Healing Mud`
- Needs engine work: `Detect Poison`, `Poison`, `Cure Poison`, `Silence`, `Wall Of Thorns`, `Cure Disease`, `Breathe Water`

### Earth / Stone

- Native now: `Armor`, `Earthquake`, `Strength`, `Weaken`, `Fury`, `Repair Item`
- Builder+Prog now: `Create Food`, `Sand Jambiya`, `Stone Skin`, `Show The Path`, `Mount`, `Godspeed`, `Sand Shelter`, `Golem`
- Needs engine work: `Sleep`, `Burrow`, `Feeblemind`, `Wall Of Sand`, `Rewind`, `Alarm`, `Sand Statue`, `Shatter`

### Wind

- Native now: `Invisibility`, `Wind Armor`, `Wind Fist`
- Builder+Prog now: `Teleport`, `Relocate`, `Sandstorm`, `Banishment`, `Guardian`, `Stalker`, `Delusion`, `Shield Of Wind`, `Messenger`, `Create Rune`
- Needs engine work: `Detect Invisible`, `Summon`, `Levitate`, `Hands Of Wind`, `Transference`, `Fly`, `Feather Fall`, `Repel`, `Wall Of Wind`, `Dispel Invisibility`

### Shadow

- Native now: `Blind`, `Create Darkness`, `Shadow Armor`
- Builder+Prog now: `Restful Shade`, `Curse`, `Haunt`, `Shadow Sword`, `Shadowplay`
- Needs engine work: `Cure Blindness`, `Remove Curse`, `Fear`, `Infravision`, `Send Shadow`, `Ethereal`, `Detect Ethereal`, `Dispel Ethereal`, `Hero Sword`

### Lightning

- Native now: `Lightning Bolt`, `Refresh`, `Stamina Drain`, `Energy Shield`, `Regenerate`
- Builder+Prog now: `Slow`, `Chain Lightning`, `Lightning Storm`, `Quickening`, `Lightning Whip`, `Illuminant`, `Lightning Spear`
- Needs engine work: `Insomnia`, `Paralyze`, `Fluorescent Footsteps`

### Void

- Native now: none
- Builder+Prog now: `Gate`, `Pseudo Death`, `Dragon Bane`, `Portable Hole`, `Phantasm`
- Needs engine work: `Animate Dead`, `Mark`, `Psionic Suppression`, `Dragon Drain`, `Charm Person`, `Shield Of Nilaz`, `Solace`, `Aura Drain`, `Travel Gate`, `Forbid Elements`, `Turn Element`, `Elemental Fog`, `Planeshift`, `Possess Corpse`, `Portal`, `Identify`, `Blade Barrier`, `Psionic Drain`, `Disembody`, `Rot Items`, `Vampiric Blade`, `Dead Speak`, `Recite`

### Unspecified / Incomplete Magic

- Native now: `Puddle`
- Builder+Prog now: none
- Needs engine work or clearer source detail: `Drown`, `Cause Disease`, `Acid Spray`

### Psionics

- Native now: `Contact`, `Barrier`, `Locate`, `Probe`, `Expel`, `Sense Presence`, `Mindblast`, `Mesmerize`, `Rejuvenate`
- Builder+Prog now: `Empathy`, `Masquerade`, `Illusion`, `Disorient`, `Clairvoyance`, `Imitate`, `Project`, `Vanish`
- Needs engine work: `Trace`, `Cathexis`, `Allspeak`, `Mindwipe`, `Shadowwalk`, `Hear`, `Control`, `Compel`, `Conceal`, `Dome`, `Clairaudience`, `Suggest`, `Babble`, `Coerce`, `Magicksense`, `Thoughtsense`, `Beast Affinity`, `Wild Contact`, `Wild Barrier`, `Immersion`
