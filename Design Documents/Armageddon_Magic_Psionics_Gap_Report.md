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
- Important current-state caveat: the current `teleport` spell effect is effectively miswired. `MudSharpCore/Magic/SpellEffects/TeleportEffect.cs` only advertises compatibility with `character` triggers but actually expects an `ICell` target. I therefore counted teleport-style caster movement as `Builder+Prog now` unless that small bug is fixed.

## Executive Summary

| Inventory | Native now | Builder+Prog now | Needs engine work |
| --- | ---: | ---: | ---: |
| Magic (152) | 43 | 49 | 60 |
| Psionics (37) | 10 | 8 | 19 |
| Total (189) | 53 | 57 | 79 |

Key takeaways:

- `110 / 189` Armageddon entries are reachable today if we allow ordinary FutureProg and prototype scaffolding.
- The current system is already strong at direct damage, healing, stamina/need adjustment, item or liquid conjuration, NPC summoning, invisibility, telepathy, and self-only magical armour.
- Phase 2 now closes three medium-difficulty primitive gaps: local exit targeting, prog-resolved summon-style remote targeting, and reusable room or personal wards with shared spell and power interception.
- The biggest parity blockers are reusable status effects, status removal, item/corpse enchantment, magic-resource drain, richer portal or anchor topology, coercive psionics, and "dual body" mechanics like possession or shadow projection.
- Psionics are only partially covered today. The current mind-link stack handles contact, barriers, mind-looking, audits, expulsion, sense, messaging, and direct mental attacks, but most coercion, concealment, remote eavesdropping, and passive-traffic powers still need new runtime support.

## Family Summary

| Family | Native now | Builder+Prog now | Needs engine work | Main blocker theme |
| --- | ---: | ---: | ---: | --- |
| Fire | 5 | 6 | 8 | Detection, dispels, object wards, burning-over-time |
| Water | 11 | 7 | 6 | Poison/disease lifecycle, silence, water-breathing |
| Earth / Stone | 7 | 8 | 7 | Sleep, burrow rooms, delayed callbacks, item destruction |
| Wind | 5 | 11 | 7 | Flying, long-range forced movement, detection or cleanse effects |
| Shadow | 3 | 5 | 9 | Ethereal state, anti-curse cleanses, fear, projection |
| Lightning | 5 | 7 | 3 | Sleep immunity, paralysis, tracked footprint effects |
| Void | 6 | 5 | 17 | Portals, possession, item/corpse enchantment, resource drain |
| Unspecified / incomplete magic | 1 | 0 | 3 | Source material is incomplete |
| Psionics | 10 | 8 | 19 | Command-forcing, concealment, passive traffic, identity masking |

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

## Main Gaps By Primitive

### 1. Reusable status application and removal

This is the single biggest easy-win cluster.

Missing reusable states include:

- `silence`
- `sleep`
- `fear` / forced flee
- `paralysis` / frozen
- `flying`
- `feather fall`
- `water breathing`
- `poison`
- `disease`
- `curse`
- `detect magick`
- `detect poison`
- `detect invisible`
- `detect ethereal`
- `infravision` / enhanced dark vision
- `tongues` / `allspeak`
- `insomnia`

Missing reusable cleanses include:

- cure blindness
- cure poison
- cure disease
- remove curse
- dispel invisibility
- dispel ethereal
- general dispel / effect shortening

This one family unlocks or materially improves a very large share of the remaining inventory.

### 2. Magic and psionic resource delta effects

Armageddon uses mana-like and psionic-resource drains in several places. FutureMUD currently has stamina and need deltas, but no first-class "modify magical resource pool" spell effect.

Blocked or only partially covered entries include:

- `Feeblemind`
- `Aura Drain`
- `Dragon Drain`
- `Psionic Drain`
- parts of `Mindwipe`

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

### 4. World-target movement and swap effects

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

This still blocks or complicates:

- `Hands Of Wind` if it needs more than "move target to chosen room" semantics
- `Transference`
- persistent paired-gate or anchor topology for `Travel Gate` and `Portal`

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

These are the changes with the best "entries unlocked per unit of work" ratio.

1. Fix the current teleport spell-effect mismatch.
   - `TeleportEffect` should either accept `room` triggers or be split into a self-teleport and target-teleport pair.
   - This immediately makes `Teleport` and `Relocate` cleaner and lowers the implementation cost of later gate-style spells.

2. Add a generic timed-status spell effect family plus a generic status-removal family.
   - Start with `silence`, `sleep`, `fear`, `paralysis`, `flying`, `waterbreathing`, `poison`, `disease`, `curse`, `detectinvisible`, `detectethereal`, `detectmagick`, `infravision`, and `comprehendlanguage`.
   - Add matching cleanse or dispel effects.
   - This unlocks a very large set of basic Armageddon staples without bespoke code per spell.

3. Add `MagicResourceDeltaEffect`.
   - This should work against the existing FutureMUD magic-resource abstraction rather than inventing a hard-coded mana field.
   - This unlocks the resource-drain half of `Feeblemind`, `Aura Drain`, `Dragon Drain`, and `Psionic Drain`.

4. Add a spell-effect equivalent of the current `MagicArmourPower`, or let spells invoke that reusable armour behavior.
   - This keeps protective effects in the ordinary spell authoring pipeline instead of forcing each one to become a bespoke power.
   - It also makes builder-side parity with `Armor`, `Sanctuary`, `Invulnerability`, and the elemental armour suite cleaner.

5. Add a generic "room flag" effect and matching removal/dispel support.
   - This is the shortest path for `Alarm`, `Restful Shade`, `Create Darkness`, and the early versions of room wards.

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
   - `EnchantItemEffect`
   - `DamageItemEffect`
   - `TagItemEffect` or generic effect metadata on items/corpses
   - This unlocks `Glyph`, `Mark`, `Vampiric Blade`, `Rot Items`, `Shatter`, and a cleaner path for corpse magic.

4. Add room wards and personal ward effects with interception hooks.
   - Completed in the current runtime as school-based wards with shared spell and power interception hooks.
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

## Recommended First Shipping Slice

The current runtime already includes exit targeting, summon-style remote targeting, and generic room or personal wards. If the goal is to maximise "Armageddon-feeling parity" quickly from here, I would ship in this order:

1. Teleport fix plus generic status application/removal.
2. Magic-resource deltas.
3. A reusable armour spell effect.
4. Item enchantment and corpse-tagging.
5. Psionic command/control framework.
6. Projection, possession, and portal topology.
7. Deeper anchor or marked-destination gate work.

That order gets the broadest number of iconic elemental spells online before tackling the truly knotty psionic and void-magic mechanics that still lack a shared engine answer.

## Appendix: Classification By Family

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
