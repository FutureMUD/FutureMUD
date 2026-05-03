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
- `Needs engine primitive` means the current magic surface is missing a concrete trigger, target type, spell effect, power type, hook, or formula, but the existing character, room, item, perception, combat, plane, or form systems are enough to host it. These are implementation tasks, not architectural blockers.
- `Needs supporting system` means the effect cannot be represented honestly until a broader runtime model is added or reworked. Examples include simultaneous active bodies, durable portal topology, objective multi-viewer illusions, or world-specific metaphysics.
- Existing powers count as valid coverage even when Armageddon exposed the original ability as a spell. If you want strict `cast`-spell parity rather than "same subsystem can do it", several defensive entries would slide from `Native now` to `Needs engine primitive`.
- A handful of Armageddon entries are under-specified in the dump (`Daylight`, `Empower`, `Drown`, `Cause Disease`, `Acid Spray`, some passive psionics). The first pass counted those conservatively unless the name clearly mapped to an existing FutureMUD primitive.
- The original first-pass counts used a single `Needs engine work` bucket. This revision keeps those historical lists in the appendix, but current planning uses the two-way split above.
- Status reviewed on 2026-05-03 against `Magic_System_Implemented_Types.md`, `Magic_System_Spells.md`, `Magic_System_Powers.md`, and the registered runtime types under `MudSharpCore/Magic`. Exact family-by-family counts were not recomputed in this pass; the stale top-line counts have therefore been removed from the planning sections. V4 added 9 builder-registered psionic power tokens and 2 builder-registered tag-aware ward effect tokens.

## Executive Summary

The major correction from the old report is that "requires engine work" is no longer one thing.

| Current category | Meaning | Examples |
| --- | --- | --- |
| Buildable now | Existing spell effects, powers, triggers, planes, body forms, tags, wards, portals, or progs can author the behaviour today. | `Ethereal`, simple `Planeshift`, `Mark`, basic `Portal`, `Thoughtsense`, `Immersion`, `Conceal`, `Glyph`, `Vampiric Blade` |
| Needs engine primitive | A new effect, power, hook, or formula is needed, but the surrounding model already exists. | `feather fall`, `detect poison`, cure blindness, `insomnia`, strength-contested dispels, open/close exit mutation, burn-over-time, footprint tracking |
| Needs supporting system | The spell implies a runtime model that FutureMUD does not yet have. | true possession/projection, body-left-behind disembodiment, durable gate/rune topology, objective multi-viewer illusions, land/elemental relationship metaphysics |

Key takeaways:

- The old headline parity counts are no longer suitable for planning. They predate the status, ward, exit, plane/form, tag, item/corpse, portal, dispel, enchantment, and psionic identity work.
- The current system is strong at direct damage, healing, stamina/need adjustment, item or liquid conjuration, NPC summoning, invisibility, telepathy, self-only magical armour, planar state shifts, and single-active-body transformation.
- Previous phases closed three medium-difficulty primitive gaps: local exit targeting, prog-resolved summon-style remote targeting, and reusable room or personal wards with shared spell and power interception.
- The plane and body-form work moves several old blockers into the buildable bucket: `Ethereal`, `Detect Ethereal`, `Dispel Ethereal`, simple `Planeshift`, ghostly manifestation, and polymorph-style transformations can now use first-class effects rather than bespoke tags.
- The biggest remaining architecture blockers are durable portal topology beyond saved effects, objective or group-scoped illusion policy, world-specific metaphysics, and true "dual body" mechanics like possession or shadow projection.
- Psionics are now better covered than the first pass suggested. The current mind-link stack handles contact, barriers, mind-looking, audits, expulsion, sense, messaging, direct mental attacks, passive thought/feeling traffic, identity concealment, trace inspection, psionic hearing, clairaudience, language comprehension, babbling, magical sensing, emotion/thought injection, and non-command coerce modes. Durable trace consequences and projection-style powers still need supporting-system work.

## Current Family Themes

| Family | Mostly buildable now | Needs engine primitive | Needs supporting system |
| --- | --- | --- | --- |
| Fire | damage, light, fire walls, fire-themed items | burn-over-time, object-specific wards, source-specific `Empower` or `Daylight` semantics if the dump requires more than light/boosts | none obvious from the current report |
| Water | healing, need/liquid manipulation, poison/disease application and removal, silence, water breathing | `detect poison`, exact `Drown` semantics if not just hypoxia/damage | land/relationship metaphysics for `Oasis` / `Determine Relationship` if they must model Armageddon's setting rules |
| Earth / Stone | armour, trait boosts, sand walls, golems, item repair/damage/destruction, spell-owned sleep | `Burrow` if it needs temporary room creation or hidden exits, `Rewind` delayed callbacks, statue/item-form edge cases | persistent constructed burrow topology if burrows must become real world structure |
| Wind | invisibility, teleport/relocate, local exit movement, walls, flight status | `feather fall`, `Hands Of Wind` if it needs special long-range forced movement, `Transference` swap semantics | projection-style `Shadowwalk` if interpreted as remote operation |
| Shadow | blindness, darkness, curse/fear/infravision, ethereal states, dispel ethereal, hero/sword item magic | cure blindness, richer fear/curse variants if source-specific rules are needed | send-shadow projection and body-left-behind shadow walking |
| Lightning | direct attacks, stamina effects, paralysis | `insomnia`, footprint tracking such as `Fluorescent Footsteps` | none obvious from the current report |
| Void | wards, portals, marks/runes, corpse preservation/consumption/spawn, resource drains, item enchantments | strength-contested dispel math, exact `Identify`/`Dead Speak`/`Recite` surfaces if they need first-class UX | durable portal/rune topology, possession, disembodiment, setting-specific `Solace` / `Dragon Bane` / `Cathexis` |
| Unspecified / incomplete magic | `Puddle`; `Cause Disease` if the dump only requires disease application | `Acid Spray`, exact `Drown` if not covered by existing damage/need/breathing primitives | source clarification may be needed before classification |
| Psionics | contact, barriers, locate/probe/expel/sense, mindblast, rejuvenate, dome, telepathy, passive traffic, identity concealment, `Trace`, `Hear`, `Clairaudience`, `Allspeak`, `Babble`, `Magicksense`, `Project Emotion`, `Suggest`, `Coerce`, and animal/wild contact variants via `connectmind` eligibility progs | content-specific `Cathexis`, `Mindwipe`, or beast/wild wrappers if they require unique UX beyond existing links and policy hooks | projection/remote-presence semantics, durable psionic trace/consequence models, and objective multi-viewer illusion state |

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
- Wind movement and fall-control effects through `levitate`, `featherfall`, `forcedpathmovement` / `handsofwind`, `transference`, and `removeinvisibility` / `dispelinvisibility`
- V3 edge statuses through `detectpoison`, `insomnia`, `removeinsomnia`, `removeblindness` / `cureblindness`, and optional strength-contested `dispelmagic`
- Coercion V1 through `forcecommand`, `subjectivedesc`, and `subjectivesdesc`
- V4 psionic and perception policy through `trace`, `hear`, `clairaudience`, `allspeak`, `babble`, `magicksense`, `projectemotion`, `suggest`, and `coerce`, plus shared traffic/audit delivery, `connectmind` eligibility progs, subjective-description priority/key handling, and tag-aware `roomtagward` / `personaltagward`

## Previous Work

This section replaces the scattered status updates that used to live in the counting rules and implementation plan.

### Foundation: Phase 1 easy wins

Completed on 2026-04-21.

- Fixed the `teleport` spell-effect mismatch so `TeleportEffect` accepts `room` / `rooms` triggers and lines up with its `ICell` target handling.
- Added standalone builder-visible status effects and matching removals for silence, sleep, fear, paralysis, flying, water breathing, poison, disease, curse, detect invisible, detect ethereal, detect magick, infravision, and comprehend language.
- Added `MagicResourceDeltaEffect` against the existing `IHaveMagicResource` abstraction, with character, item, and room resources clamped to valid ranges.
- Added `SpellArmourEffect` by sharing `MagicArmourConfiguration` with the existing armour power.
- Added `roomflag` / `removeroomflag` for `peaceful`, `nodream`, `alarm`, `darkness`, and `wardtag`.

Deferred from this pass and still relevant: edge status primitives such as `feather fall`, `detect poison`, `insomnia`, cure blindness, and strength-contested dispel math.

### Related plane and body-form work

Completed before Engine V1.

- Added `IPlane`, `PlanarPresenceDefinition`, `planarstate`, `planeshift`, `removeplanarstate`, planar FutureProg helpers, planar merits/drugs, and the `corporeality` admin command.
- Added `transformform` as the magic bridge into the multiple-body-form system.
- This made simple ethereal states, noncorporeal manifestation, single-active-body transformation, and straightforward "move this target to another plane" spells buildable.

Deferred from this pass and still relevant: simultaneous active bodies, remote vessels, possessed corpses, descriptor handoff, source-body vulnerability, reconnect behaviour, and durable planar travel/topology beyond normal movement or saved effects.

### Engine V1: tags, item/corpse magic, portals, and coercion

Completed on 2026-05-01.

- Added `magictag` / `removemagictag` and FutureProg helpers `hasmagictag`, `magictagvalue`, `magictagvalues`, and `magictags`.
- Added first-class item and corpse effects: `itemdamage`, `destroyitem`, `itemenchant`, `corpsemark`, `corpsepreserve`, `corpseconsume`, and `corpsespawn`.
- Added transient paired magical portals through `portal`, backed by effect-owned transient exits rather than permanent database exits.
- Added safe command-forcing through `forcecommand`, with staff/editor/account-destructive roots blocked and wiz-only audit output.
- Added caster-scoped subjective short/full description overrides through `subjectivesdesc` and `subjectivedesc`.

Deferred from V1 but later resolved in V2: general dispel/shorten support, portal inspection and item anchors, richer item-enchantment hooks, and psionic identity/passive-traffic policy.

Deferred from V1 but later resolved in V4: advanced non-command coercion policy and subjective illusion priority/dispel keys.

Deferred from V1 and still relevant: durable portal/rune topology, objective or group-scoped illusions, durable trace consequences, and true possession/projection.

### Engine V2: dispels, richer enchantments, portal inspection, and psionic identity

Completed on 2026-05-02.

- Added `dispelmagic`, which removes or shortens matching spell-parent effects by caster policy, spell, school/subschool, magic tag, and approved effect/interface keys.
- Added active portal inspection through `magic portals`, active anchor inspection through `magic anchors [tag]`, and item/object anchor resolution for `portal`.
- Expanded `itemenchant` with projectile/ranged payload bonuses, craft-tool bonuses, power/fuel modifiers, and optional item event progs.
- Added builder-loadable plane/form recipe coverage for `Ethereal`, `Dispel Ethereal`, `Planeshift`, shadow/astral walking, and polymorph-style configurations.
- Added `mindconceal` and wired shared concealment policy into mind contact, mind speech/broadcast, mind audit/expel, and passive `think`/`feel` telepathy.

Deferred from V2 but later resolved in V3 or V4: strength-contested dispel formulas, richer subjective-illusion priority and dispel policy, advanced non-command coercion primitives, and tag-aware ward matching.

Deferred from V2 and still relevant: persistent gate/rune topology if saved effects are not enough, durable trace consequences, objective or group-scoped illusion state, and the simultaneous-body possession/projection model.

### Engine V4: psionic and perception policy layer

Completed on 2026-05-03.

- Added builder-registered psionic powers `trace`, `hear`, `clairaudience`, `allspeak`, `babble`, `magicksense`, `projectemotion`, `suggest`, and `coerce`.
- Added shared psionic traffic and coercion policy for involuntary thought/feeling delivery, listener forwarding, opt-out checks, blocked command roots, source/target messaging, and wiz-audit output.
- Added a builder-visible `psionic` toggle to `MagicPowerBase`, persisting through the existing `IsPsionic` XML field and using the psionics crime type.
- Added contextual interdiction metadata and tag-aware `roomtagward` / `personaltagward` effects that match configured `magictag` key/value metadata.
- Added subjective-description priority and illusion-key support, including `dispelmagic illusion <key>` matching for keyed subjective illusions.
- Added a target-eligibility prog to `connectmind`, so animal, wild, or setting-specific contact variants can be expressed without new hard-coded link powers.

V4 count deltas: +9 builder power tokens, +2 builder spell-effect tokens, +7 shared policy/support types. The V4 psionic/perception engine-primitive backlog is complete; the remaining psionic/perception blockers are supporting-system problems rather than ordinary power/effect registration work.

## Current Reclassification From Planes And Body Forms

The old blocker list bundled "ethereal", "projection", "possession", "planeshift", and "shape change" together. The live runtime now supports some of those as first-class mechanics, but not all of them.

| Old blocker theme | Buildable now | Needs engine primitive | Needs supporting system |
| --- | --- | --- | --- |
| Ethereal or noncorporeal state | Use `planarstate` / `planeshift` on characters, items, or other perceivables. Plane data handles room presentation, remote observation tags, perception, interaction checks, noncorporeal physiology, inventory propagation, and closed-door bypass where configured. | Custom transition trauma or special dispel contests if a spell needs more than the normal spell/resist/ward flow. | A separate acting shell rather than changing the target's own planar presence. |
| Detect or dispel ethereal | Use `detectethereal` / `removedetectethereal`, `removeplanarstate`, or `dispelmagic effect planarstate`. | Strength-contested dispel math beyond the current `dispelmagic` criteria model. | None unless the dispel depends on a broader plane travel graph or world metaphysics. |
| Planeshift | Simple "target moves to configured plane/state" is first-class with `planeshift`. | Destination safety formulas or extra transition effects. | Multi-step planar travel graphs, unsafe destination modelling, or durable portal networks that should survive beyond an effect duration. |
| Shadowwalk-style movement | If the behaviour is "enter a shadow/astral/ethereal plane and move normally", use plane definitions plus `planeshift`. | Minor movement/detection wrappers around plane shifting. | Remote projection, leaving a body behind, or moving a second body independently. |
| Polymorph, animal form, statue-like form, or spirit form | Use `transformform` for single-active-body transformation, with `Additional Body Form` merits for intrinsic or racial forms. | Turning a target into a true item-like state, or adding extra form metadata synchronisation. | Two bodies acting at once, using a corpse as the exact vessel, or descriptor handoff between independent bodies. |
| Possession, disembodying, and send-shadow projection | Simplified content can use `planeshift` or `transformform` where the original character becomes the new form/state. | A future possession/projection effect can be built after the model exists. | True possession and projection require simultaneous presence, command routing, source-body vulnerability, disconnect handling, staff visibility, inventory rules, and death semantics. |
| Marks, runes, anchors, and hidden magical facts | Use `magictag` / `removemagictag` plus the magic-tag FutureProg helpers. `portal` can consume caster-owned room or item/object anchors, and active portal/anchor state is inspectable with `magic portals` and `magic anchors`. | Behavioural effects should get first-class support when the tag would be pretending to be combat, movement, item damage, resource changes, or perception. | Persistent rune networks, standing portals, and durable world-topology edits if saved effects and transient exits are not sufficient. |

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

The V3 edge set in this primitive family is now implemented:

- `detect poison` uses `detectpoison`, an instantaneous character-targeted spell effect that reports active and latent drug dosages.
- `insomnia` / `removeinsomnia` prevents voluntary sleep and blocks magical sleep from taking hold.
- `cure blindness` uses `removeblindness` or its load/builder alias `cureblindness`.
- strength-contested dispels are available as an opt-in `dispelmagic` mode layered over the existing criteria model.

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

Remaining work:

- Needs engine primitive: `Hands Of Wind` if it needs more than "move target to chosen room" semantics, and `Transference` if it requires a true swap effect.
- Needs supporting system: persistent paired-gate topology for `Travel Gate` and `Portal`; transient paired exits and caster-owned room or item anchors are already live.

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

V4 extension:

- `roomtagward` and `personaltagward` add tag-aware interdiction that matches `magictag` key/value metadata while reusing the same fail/reflect and coverage model.

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

Remaining work:

- Needs supporting system: `Send Shadow`, possession-style `Shadowwalk`, `Possess Corpse`, and body-left-behind `Disembody`.
- Needs engine primitive or content clarification: `Burrow`, if it only needs temporary room creation or hidden exits rather than true world-topology persistence.
- Needs supporting system if interpreted durably: pieces of `Portal` and `Planeshift` that require persistent topology or plane travel graphs rather than changing the target's current planar presence.

`Shadowwalk`, `Disembody`, and `Planeshift` should be split by intended semantics. If they mean "change this target's planar presence", they are now buildable. If they mean "leave one body behind and operate another", they remain blocked by simultaneous-body mechanics.

### 8. Subjective perception, coercive psionics, and passive mind traffic

Status: Coercion V1, psionic identity concealment, passive thought/feeling traffic, and the V4 psionic/perception policy layer are implemented. The older report text that listed identity hiding, passive traffic, basic trace/hear/clairaudience powers, and non-command coercion primitives as open blockers is stale.

FutureMUD already has good mind-link primitives and now has:

- `forcecommand`, which runs as the target's own command context through `ExecuteCommand`, respects `IIgnoreForceEffect`, blocks staff/editor/account-destructive command roots, and emits wiz-only audit output.
- `subjectivedesc`, which applies a spell-owned full-description override.
- `subjectivesdesc`, which applies a spell-owned short-description override.
- caster-scoped subjective-description support through fixed-viewer handling.
- `mindconceal`, which supplies sustained identity concealment and an audit difficulty modifier.
- passive `think` / `feel` / `thinkemote` traffic through `telepathy`, with concealment consulted before identities are exposed.
- `trace`, which inspects active mind links around a target mind and respects `mindconceal` audit difficulty and unknown-identity output.
- `hear`, which sustains psionic thought/feeling listening over configured scope without becoming ordinary room audio.
- `clairaudience`, which sustains contact-based remote hearing through another mind's location and forwards audible output only.
- `allspeak`, which grants sustained spoken-language comprehension through `IComprehendLanguageEffect` without permanent language skill or literacy bypass.
- `babble`, which applies hostile timed speech obfuscation before language comprehension can decode the speech.
- `magicksense`, which grants sustained `SenseMagical` perception through the existing magical aura display.
- `projectemotion`, `suggest`, and `coerce`, which share traffic/coercion policy for involuntary emotion/thought delivery, listener forwarding, opt-out checks, and audit output.
- subjective-description priority and illusion keys for predictable stacking and keyed `dispelmagic`.
- `roomtagward` and `personaltagward`, which interdict by `magictag` key/value metadata rather than only school/subschool.
- `connectmind` target eligibility progs, so animal, wild, or setting-specific contact variants can be configured directly.

This now covers:

- `Conceal`, when the desired behaviour is hidden mental identity.
- `Thoughtsense` and `Immersion`, when the desired behaviour is passive thought/feeling eavesdropping.
- the safest command-forcing cases for `Control` and `Compel`, so long as content stays within non-staff, non-account-destructive command roots.
- non-command `Suggest`, `Project Emotion`, and common `Coerce` modes that should alter thought, feeling, stamina, hunger, or thirst rather than run a victim command.
- single-viewer description changes for parts of `Masquerade`, `Imitate`, and `Vanish`.

It does not yet cover:

- robust refusal/consent policy beyond opt-out effects, ordinary targeting checks, and hard safety block lists.
- durable trace consequences beyond live link inspection and `mindconceal` making audits harder.
- objective room-state illusions or multi-viewer/group-scoped illusions that need a general perception-overlay model.

That means the remaining work splits cleanly:

- Needs engine primitive: none from the V4 psionic/perception policy slice. Content-specific `Cathexis`, `Mindwipe`, or beast/wild wrappers may still need dedicated UX if progs and existing link effects are not enough.
- Needs supporting system: projection-style psionics, durable trace consequence models if traces must persist as world facts, and objective multi-viewer illusions if they need a general perception-overlay framework.

## Future Work

### Needs engine primitive only

These tasks should be ordinary implementation work inside the existing magic, perception, item, movement, combat, or psionic surfaces.

- Add remaining edge status/detection/removal effects: `feather fall`, `detect poison`, `insomnia` or sleep-immunity, cure blindness, and any source-specific anti-status variants the Armageddon list requires.
- Add strength-contested dispel math on top of `dispelmagic` for content that wants explicit caster-vs-caster strength contests rather than the current authored criteria plus normal targeting/resist/ward flow.
- Add exit state mutation beyond barriers, such as opening, closing, sealing, or unlocking a targeted exit when `exitbarrier` is not the right model.
- Add burn-over-time, footprint-tracking, and similar persistent sensory/combat effects for spells such as `Immolate` and `Fluorescent Footsteps`.
- Add swap or long-range movement effects if `Transference` and `Hands Of Wind` require more than `teleporttarget`, `relocate`, `prog...room`, or `forcedexitmovement`.
- Add specific information powers/effects for `Identify`, `Dead Speak`, and `Recite` where existing progs or metadata are not sufficient.
- Add dedicated `Beast Affinity`, `Wild Contact`, or `Wild Barrier` wrappers only if `connectmind` eligibility progs, existing barriers, and ordinary content naming are not expressive enough.

### Needs supporting system or rework

These are the remaining true blockers. They should not be represented as one-off spell effects until the supporting model exists.

- Simultaneous-body possession and projection: `Send Shadow`, possession-style `Shadowwalk`, `Possess Corpse`, and body-left-behind `Disembody` need command routing, perception routing, source-body vulnerability, inventory rules, combat/death rules, reconnect behaviour, and staff observability.
- Durable portal/rune topology: standing gates, persistent rune networks, portal objects, and topology edits that must survive beyond saved spell effects need a deliberate persistence and lifecycle model.
- Objective or group-scoped illusions: if illusions must alter room state for multiple observers, stack with other illusions, and expose consistent dispel/priority rules, they need a general perception-overlay policy rather than only `subjectivedesc` / `subjectivesdesc`.
- World-specific metaphysics: `Determine Relationship`, `Solace`, `Dragon Bane`, `Cathexis`, and richer `Planeshift` interpretations need a model for land/elemental relationships, plane travel graphs, and any clan/tribe/identity consequences.
- Durable psionic trace consequences: the live `trace` power inspects active links, but traces that persist as world facts or feed staff/audit consequences need a shared psionic trail model.

## Next Logical Steps

### V3: high-return primitive cleanup

The next slice should finish the small, unglamorous primitives that no longer need design debate:

- Add `feather fall`, `detect poison`, `insomnia`/sleep immunity, cure blindness, burn-over-time, and footprint-tracking effects with focused spell-effect tests.
- Add exit state mutation if the Armageddon wall/utility set needs open/close/seal behaviour beyond `exitbarrier`.
- Add contested-dispel math as an optional `dispelmagic` mode or companion effect, keeping the current criteria-based cleanup as the default.
- Add builder-loadable recipe tests that prove formerly blocked Fire, Water, Wind, Shadow, Lightning, and Void examples load against the current primitives.
- Refresh the family-by-family classification counts after V3, because the old first-pass numbers are no longer actionable.

### V4: psionic and perception policy layer

Status: completed on 2026-05-03.

- `trace`, `hear`, `clairaudience`, `allspeak`, `babble`, `magicksense`, `projectemotion`, `suggest`, and `coerce` are builder-registered powers.
- `connectmind` now has an eligibility prog for animal, wild, or setting-specific mind-contact variants.
- Psionic traffic now has a shared delivery/refusal/listener/audit helper rather than each effect hand-rolling policy.
- Subjective descriptions now have priority and illusion-key handling, and `dispelmagic` can target keyed illusions.
- `roomtagward` and `personaltagward` are available for tag-aware anti-magic.

### V5: supporting-system buildout

V5 should tackle the remaining architecture blockers after V3/V4 make the easy and medium gaps boring:

- simultaneous-body possession and projection
- objective or group-scoped illusion state that changes room facts for multiple observers
- durable psionic trace/trail consequences that survive beyond live link inspection
- persistent rune/gate/portal topology if saved effects and transient exits are not enough
- world-specific metaphysics such as land relationships, elemental patronage, or clan-keyed psionic consequences

`Control` and `Compel` remain policy-sensitive when authored as command-forcing effects. `Suggest`, `Project Emotion`, and most `Coerce` variants now have non-command delivery paths, but content authors should still pair them with clear IC/OOC policy and staff-facing audit expectations.

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
   - Extended in V4 with `projectemotion`, `suggest`, and mode-based `coerce`.
   - The command-forcing implementation executes through the target's own `ExecuteCommand`, respects `IIgnoreForceEffect`, blocks staff/editor/account-destructive roots, and emits wiz-only audit output.
   - The V4 non-command path shares opt-out checks, listener delivery, and audit output. Durable trace consequences and broader consent-policy models remain future work.

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
   - Status: caster-scoped subjective short/full description overrides are live through `subjectivesdesc` and `subjectivedesc`, now with priority and illusion-key handling.
   - Remaining work:
     - objective room-state illusions
     - multi-viewer or group-scoped illusions

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

Engine V3 has now shipped the small edge-status slice that sat outside Engine V2: poison detection, insomnia, cure blindness, and optional strength-contested dispel matching. The next remaining work should focus on the larger pieces Engine V2 deliberately left outside the slice:

- durable portal/rune topology if saved effects and transient exits are not enough for standing gate networks
- richer illusion stacking and perception policy
- advanced psionic coercion and trace consequences
- the simultaneous-body possession/projection model

## Appendix: Classification By Family

This appendix is the historical family-by-family classification from the first pass. It is retained as provenance for the original gap analysis, not as the current blocker list. Many entries below have since moved into `Native now` or `Builder+Prog now`; use the current-runtime sections above for planning until exact counts are refreshed.

### Fire

- Native now: `Fireball`, `Flamestrike`, `Demonfire`, `Fire Armor`, `Wall Of Fire`
- Builder+Prog now: `Rain Of Fire`, `Ball Of Light`, `Pyrotechnics`, `Parch`, `Fire Jambiya`, `Fire Seed`
- Historical needs engine work: `Detect Magick`, `Dispel Magick`, `Empower`, `Glyph`, `Tongues`, `Firebreather`, `Daylight`, `Immolate`

### Water

- Native now: `Create Water`, `Heal`, `Sanctuary`, `Calm`, `Invulnerability`, `Deafness`, `Health Drain`, `Create Wine`, `Intoxication`, `Sober`, `Wall Of Thorns`
- Builder+Prog now: `Oasis`, `Determine Relationship`, `Thunder`, `Wither`, `Mirage`, `Shield Of Mist`, `Healing Mud`
- Historical needs engine work: `Detect Poison`, `Poison`, `Cure Poison`, `Silence`, `Cure Disease`, `Breathe Water`

### Earth / Stone

- Native now: `Armor`, `Earthquake`, `Strength`, `Weaken`, `Fury`, `Repair Item`, `Wall Of Sand`
- Builder+Prog now: `Create Food`, `Sand Jambiya`, `Stone Skin`, `Show The Path`, `Mount`, `Godspeed`, `Sand Shelter`, `Golem`
- Historical needs engine work: `Sleep`, `Burrow`, `Feeblemind`, `Rewind`, `Alarm`, `Sand Statue`, `Shatter`

### Wind

- Native now: `Invisibility`, `Detect Invisible`, `Levitate`, `Hands Of Wind`, `Transference`, `Fly`, `Feather Fall`, `Dispel Invisibility`, `Wind Armor`, `Wind Fist`, `Repel`, `Wall Of Wind`
- Builder+Prog now: `Teleport`, `Relocate`, `Sandstorm`, `Banishment`, `Guardian`, `Stalker`, `Delusion`, `Shield Of Wind`, `Messenger`, `Create Rune`, `Summon`
- Historical needs engine work: `Detect Invisible`, `Levitate`, `Hands Of Wind`, `Transference`, `Fly`, `Feather Fall`, `Dispel Invisibility`

### Shadow

- Native now: `Blind`, `Create Darkness`, `Shadow Armor`
- Builder+Prog now: `Restful Shade`, `Curse`, `Haunt`, `Shadow Sword`, `Shadowplay`
- Historical needs engine work: `Cure Blindness`, `Remove Curse`, `Fear`, `Infravision`, `Send Shadow`, `Ethereal`, `Detect Ethereal`, `Dispel Ethereal`, `Hero Sword`

### Lightning

- Native now: `Lightning Bolt`, `Refresh`, `Stamina Drain`, `Energy Shield`, `Regenerate`
- Builder+Prog now: `Slow`, `Chain Lightning`, `Lightning Storm`, `Quickening`, `Lightning Whip`, `Illuminant`, `Lightning Spear`
- Historical needs engine work: `Insomnia`, `Paralyze`, `Fluorescent Footsteps`

### Void

- Native now: `Psionic Suppression`, `Shield Of Nilaz`, `Forbid Elements`, `Turn Element`, `Elemental Fog`, `Blade Barrier`
- Builder+Prog now: `Gate`, `Pseudo Death`, `Dragon Bane`, `Portable Hole`, `Phantasm`
- Historical needs engine work: `Animate Dead`, `Mark`, `Dragon Drain`, `Charm Person`, `Solace`, `Aura Drain`, `Travel Gate`, `Planeshift`, `Possess Corpse`, `Portal`, `Identify`, `Psionic Drain`, `Disembody`, `Rot Items`, `Vampiric Blade`, `Dead Speak`, `Recite`

### Unspecified / Incomplete Magic

- Native now: `Puddle`
- Builder+Prog now: none
- Historical needs engine work or clearer source detail: `Drown`, `Cause Disease`, `Acid Spray`

### Psionics

- Native now: `Contact`, `Barrier`, `Locate`, `Probe`, `Expel`, `Sense Presence`, `Mindblast`, `Mesmerize`, `Rejuvenate`, `Dome`, `Trace`, `Allspeak`, `Hear`, `Clairaudience`, `Suggest`, `Babble`, `Coerce`, `Magicksense`, `Thoughtsense`, and `Immersion`
- Builder+Prog now: `Empathy`, `Masquerade`, `Illusion`, `Disorient`, `Clairvoyance`, `Imitate`, `Project`, `Vanish`, `Beast Affinity`, `Wild Contact`, and `Wild Barrier`
- Historical needs engine work still requiring content-specific decision or broader systems: `Cathexis`, `Mindwipe`, `Shadowwalk`, and true projection/possession interpretations
