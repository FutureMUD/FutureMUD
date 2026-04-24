# Multiple Body Form System

## Purpose

The multiple body form system lets one character own more than one physical body and switch which body is currently active. The initial design driver was transformation content such as werewolves, ghosts, astral forms, robots, animal polymorphs, and other cases where the same character identity needs a different anatomy, health model, appearance, or racial package.

The current implementation is deliberately conservative: one character has one active body at a time. Inactive bodies are dormant form records, not independent room occupants. This keeps transformation semantics tractable while preserving the original separation between `ICharacter` as identity and `IBody` as physical embodiment.

## Design Goals

- Preserve existing single-body character behaviour unless a character explicitly gains extra forms.
- Keep character identity, skills, merits, account ownership, culture, and social continuity above the body layer.
- Let different forms have different race, ethnicity, gender, anatomy, handedness, attributes, health strategy, description pattern, and inventory fit.
- Support voluntary, scripted, and forced transformations without letting low-level body switching bypass structural safety.
- Cache provisioned forms so repeated merit, spell, and prog triggers reuse the same body instead of recreating it.
- Give builders enough metadata to hide forms, gate voluntary use, customise transformation echoes, and control forced transformation priority.
- Avoid introducing remote bodies, simultaneous bodies, or astral shells until the base ownership and switching model is stable.

## Core Concepts

### Character

The character is the stable player or NPC identity. Character-scoped state includes skills, theoretical skills, derived skills, merits, name, account, culture, knowledge, and form ownership. `IHaveABody.Body` remains a compatibility alias for `ICharacter.CurrentBody`.

### Body

The body is the physical runtime object. It owns anatomy, organs, limbs, wounds, attributes, derived attributes, health strategy, breathing, inventory slots, implants, prosthetics, bodypart state, and body description pattern selection.

The same character can own multiple bodies, but only `CurrentBody` is active in the world in the current phase.

### Form

A form is an owned body plus per-character metadata. Runtime form contracts live in [ICharacter.cs](../FutureMUDLibrary/Character/ICharacter.cs), and the concrete loading, saving, creation, and switching orchestration lives primarily in [CharacterForms.cs](../MudSharpCore/Character/CharacterForms.cs).

Form metadata currently includes:

| Field | Purpose |
| --- | --- |
| `Alias` | Builder and player-facing identifier, such as `wolf`, `ghost`, or `robot`. |
| `SortOrder` | Display and deterministic tie-break ordering. |
| `TraumaMode` | Whether trauma transfers, stashes, or auto-selects based on health compatibility. |
| `TransformationEcho` | Optional emote shown on switch; `null` uses the default static string, empty suppresses echo. |
| `AllowVoluntarySwitch` | Enables or disables player-initiated switching into this target form. |
| `CanVoluntarilySwitchProg` | Optional boolean FutureProg gate for voluntary switching. |
| `WhyCannotVoluntarilySwitchProg` | Optional text FutureProg for voluntary denial messages. |
| `CanSeeFormProg` | Optional boolean FutureProg controlling whether the owner can see the form in the `form` command. |
| Description patterns | Optional short and full description patterns for the owned body. |

### Form Source

A form source is a stable mapping between a character and the content that provisioned a form. Source mappings are stored separately from form metadata so that a merit or spell can find the same body again without owning the form's later edits.

Supported source identities are defined in [CharacterFormProvisioning.cs](../FutureMUDLibrary/Character/CharacterFormProvisioning.cs):

| Source Type | Current Use |
| --- | --- |
| `Merit` | `SourceId` is the merit id. Used by Additional Body Form merits. |
| `SpellEffect` | `SourceId` is the spell id and `SourceKey` is the spell builder's form key. Used by `transformform`. |
| `Prog` | `SourceKey` is supplied by the caller. Used by `EnsureForm(...)` FutureProg functions. |

Creation defaults are applied only when the form is first created. If a later source trigger reuses an existing mapped form, the current per-character form metadata remains authoritative.

### Form Specification

A form specification is the shared creation request used by admin commands, merits, spell effects, and FutureProg. It includes race, optional ethnicity, optional gender, initial alias, sort order, trauma mode, transformation echo, voluntary access rules, visibility prog, and description pattern defaults.

`ICharacter.EnsureForm(...)` is the central provisioning path. It:

1. Reuses an existing body from `CharacterBodySources` when a stable source mapping exists.
2. Otherwise adopts exactly one existing form that matches alias plus race, ethnicity, and gender.
3. Otherwise creates a new dormant body by cloning the character template and applying the form specification.

### Switch Intent

`BodySwitchIntent` describes why a switch is being attempted:

| Intent | Meaning |
| --- | --- |
| `Voluntary` | Player-initiated. Requires visible owner-facing resolution, `AllowVoluntarySwitch`, optional can-switch prog success, and structural switch validity. |
| `Scripted` | Content-initiated, usually spells, effects, or progs. Bypasses voluntary form rules but still requires structural switch validity. |
| `Forced` | Admin or mandatory transformation path. Bypasses voluntary rules and can cancel or recompute transient gameplay state, but still cannot ignore structural transfer failure. |

### Trauma Mode

`BodySwitchTraumaMode` controls what happens to wounds and other health-system state:

| Mode | Behaviour |
| --- | --- |
| `Automatic` | Transfers if both health strategies can safely exchange state; otherwise stashes incompatible trauma on the dormant form. |
| `Transfer` | Attempts to remap compatible trauma to the target form. Structural remapping failure prevents the switch. |
| `Stash` | Leaves trauma and compatible health effects on the old dormant form in stasis and sanitises incompatible target health state. |

The stasis model is important for organic to inorganic switches. A bleeding human wound should not keep bleeding while the character is in a robot form, and robot oil loss should not become an organic infection when switching back.

## Current Implementation

### Persistence

The current implementation adds or uses these main persistence concepts:

| Persistence Surface | Purpose |
| --- | --- |
| `Characters.BodyId` | The active body id. |
| `CharacterBodies` | Character-owned form rows and form metadata. |
| `CharacterBodySources` | Stable source-to-body mappings for merit, spell, and prog provisioning. |
| `Bodies` | Physical body records, including per-body handedness and description pattern ids. |
| `CharacterTraits` | Character-scoped skill trait storage. |
| `TraitDefinitions.OwnerScope` | Distinguishes character-scoped skills from body-scoped attributes. |
| Effect XML | Stores active spell transform body ids and forced transformation baselines. |

Skills, derived skills, and theoretical skills are character-scoped. Attributes and derived attributes remain body-scoped. This lets a wolf form have different strength or agility without erasing the character's language, weapon, craft, or knowledge skills.

Corpses and severed-part style systems snapshot or retain the source body identity rather than resolving anatomy through the character's current form later. This prevents butchery, surgery, and remains descriptions from changing because the character transformed after the corpse or bodypart item was created.

### Loading and Compatibility

Single-body characters are normalised into a default form at load time. The default form uses the current body, defaults voluntary switching to allowed, and uses the body prototype name as its alias.

`IHaveABody.Body` remains available for older call sites, but new form-aware code should prefer `ICharacter.CurrentBody`, `ICharacter.Bodies`, and the form APIs.

### Switching Pipeline

Switch preparation and application are implemented in [BodyFormSwitching.cs](../MudSharpCore/Body/Implementations/BodyFormSwitching.cs) and coordinated by [CharacterForms.cs](../MudSharpCore/Character/CharacterForms.cs).

The high-level switch flow is:

1. Resolve the target form.
2. Apply visibility and voluntary rules when the request is player-initiated.
3. Select the effective trauma mode.
4. Build a body switch plan.
5. Reject unsafe transfers, such as bound restraints, unmappable severed roots, incompatible implants, or incompatible prosthetics.
6. Transfer or stash trauma according to the plan.
7. Move inventory where possible, dropping ordinary items that no longer fit and rejecting restraints that cannot safely transfer.
8. Recalculate body helpers, organ functions, breathing, stamina, and health consequences with health feedback suppressed during the switch.
9. Set `CurrentBody`, activate the new body, suspend the old body, emit the transformation echo, and fire `CurrentBodyChanged`.

Body switching does not use `Body.Quit()`. Dormant bodies stop health, drug, stamina, and breathing ticks and are treated as non-present shells until reactivated.

### Anatomy Mapping

Bodypart mapping uses strict continuity first and relaxed best-fit matching where appropriate.

Strict mapping prefers:

1. `IBodypart.CountsAs(...)`
2. Matching bodypart id
3. Matching bodypart type, alignment, orientation, and shape

Relaxed mapping is used for wounds, scars, tattoos, infections, and some temporary treatment effects. It scores likely equivalents such as brain to positronic brain, heart to power core, lungs to gills or blowholes, eyes or ears to sensor arrays, and similar organ or bodypart classes. More structural state, such as severed roots, implants, prosthetics, and replanted parts, remains stricter because a bad mapping there would create more serious state corruption.

### Health and Trauma

Form switching supports both transfer and stasis.

Transfer mode remaps compatible wounds, part infections, scars, tattoos, severed roots, implants, prosthetics, active and latent drugs, blood state, held breath time, stamina, and selected treatment effects. Internal bleeding, antiseptic treatment, anti-inflammatory treatment, and replanted bodypart effects are recreated on mapped bodyparts with their remaining duration.

Stash mode leaves trauma on the old body, caches scheduled effects, stops ongoing body ticks, clears restraints and direct inventory state, and sanitises the target body for its health strategy. Incompatible wounds and organic-only health effects are removed from the active target form, and lost fluids can be restored when no compatible bleed sources remain. Health feedback is suppressed during the switch so players do not see transient "dying", "can't breathe", or organ recovery spam caused by intermediate recalculations.

### Description and Transformation Echoes

Each form can have its own short and full description patterns. If a form is created without explicit patterns, the engine chooses a random valid pattern for the new body's race and description type. This prevents transformed bodies from using invalid variables from the character's old body, such as human skin colour markup on a shark or robot form.

Transformation echoes are per-form metadata:

| Value | Behaviour |
| --- | --- |
| `null` | Use `DefaultFormTransformationEcho` from static configuration. |
| Empty string | Suppress the transformation echo. |
| Text | Use the custom emote text. |

The default static string is currently `@ transform|transforms into $1.`, where `$1` is the new body in the emote context.

### Player Command Surface

The player-facing command is intentionally small:

| Command | Behaviour |
| --- | --- |
| `form` | Lists forms the owner can see, plus the current form even if hidden. Shows current and availability state. |
| `form <alias>` | Attempts a voluntary switch into a visible form. Hidden forms do not resolve by guessed alias. |

The player command is an engine fallback and validation surface. Games can wrap or hide it behind powers, rituals, racial commands, or custom command trees.

### Admin Command Surface

The implementor-oriented command surface lives in [CharacterInformation.cs](../MudSharpCore/Commands/Modules/CharacterInformation.cs):

| Command | Behaviour |
| --- | --- |
| `body addform <character> <race> [ethnicity] [gender]` | Adds a dormant form for testing and administration. |
| `body formset <character> <form> alias ...` | Edits the form alias. |
| `body formset <character> <form> trauma ...` | Edits trauma mode. |
| `body formset <character> <form> echo ...` | Sets, clears to default, or suppresses transformation echo. |
| `body formset <character> <form> allow ...` | Edits voluntary access. |
| `body formset <character> <form> canprog ...` | Edits the voluntary can-switch prog. |
| `body formset <character> <form> whycantprog ...` | Edits the voluntary denial-message prog. |
| `body formset <character> <form> visibleprog ...` | Edits the visibility prog. |
| `body formset <character> <form> sdescpattern ...` | Sets, randomises, or clears the short description pattern. |
| `body formset <character> <form> fdescpattern ...` | Sets, randomises, or clears the full description pattern. |
| `body switch <character> <form>` | Forces a switch for testing and staff intervention. |

### Merit Provisioning

The `Additional Body Form` merit type is implemented by [AdditionalBodyFormMerit.cs](../MudSharpCore/RPG/Merits/CharacterMerits/AdditionalBodyFormMerit.cs).

The merit provisions a form when the character is created, loaded, or receives the merit. Removing the merit does not delete the cached body. Re-adding the merit reuses the existing source-mapped body.

Merit builders can configure:

| Builder Setting | Purpose |
| --- | --- |
| race, ethnicity, gender | The provisioned form's body identity. |
| alias and sort order | Initial form presentation. |
| trauma mode | First-creation trauma handling default. |
| echo | Default, custom, or suppressed transformation echo. |
| voluntary rules | Initial voluntary access and gating progs. |
| visibility prog | Initial owner-facing visibility rule. |
| description patterns | Initial valid short and full description patterns. |
| auto-transform | Whether the merit contributes a forced transformation while applicable. |
| priority band and offset | How the forced transformation ranks against other mandatory demands. |
| recheck cadence | How often applicability is rechecked while the character is online. |

Auto-transforming merits are the current racial or intrinsic transformation path. A werewolf merit can be applicable only during the full moon, provision a wolf-man form, force the character into it while applicable, and revert when applicability ends.

### Spell Provisioning

The `transformform` spell effect is implemented by [TransformFormEffect.cs](../MudSharpCore/Magic/SpellEffects/TransformFormEffect.cs) and its active runtime effect [SpellTransformFormEffect.cs](../MudSharpCore/Effects/Concrete/SpellEffects/SpellTransformFormEffect.cs).

The spell effect ensures or reuses a keyed form and contributes a forced transformation demand while the spell effect applies. The spell stores the prior body id so it can revert after expiry or save/load. If the prior body is unavailable or cannot be switched into, the resolver attempts another valid owned form. If no fallback works, the character remains in the current form and staff-facing diagnostics are emitted rather than deleting the cached form.

Spell builders can configure the same first-creation form metadata as merits, plus a stable `FormKey`, priority band, and priority offset. Repeated casts of the same spell and key reuse the same body.

### Forced Transformation Resolution

Mandatory transformations are resolved in [CharacterForcedTransformations.cs](../MudSharpCore/Character/CharacterForcedTransformations.cs).

The resolver gathers active forced transformation demands from:

- Auto-transforming additional-body-form merits whose applicability prog currently applies.
- Active `SpellTransformFormEffect` effects.

Demands are sorted by priority band, priority offset, source tie-breaker, and form ordering. The current priority bands are:

| Band | Intended Meaning |
| --- | --- |
| `MeritOrIntrinsic` | Racial, intrinsic, or always-on identity transforms. |
| `DrugOrChemical` | Reserved for drug and chemical transformations. |
| `SpellOrPower` | Magic, powers, and temporary supernatural effects. |
| `AdminForced` | Staff or debug-level forced transformations. |

The `DrugOrChemical` band exists so content can rank chemical transformations consistently, but the forced-demand resolver currently has first-class demand collection for merits and spell transform effects. Drug or liquid content can still use FutureProg form functions or spell/effect plumbing, but a dedicated drug transformation demand source is future work.

When a mandatory transform first takes control, the system records a baseline body in `ForcedTransformationBaselineEffect`. When the winning demand changes or no demands remain, the character is switched to the new winning target or reverted toward the recorded baseline.

Merit applicability is rechecked by a single registered character delegate using fuzzy minute or fuzzy hour cadence. This avoids a global sweep over all characters while still supporting time-sensitive transforms such as full moons.

### FutureProg Surface

Form FutureProg functions use character plus alias or body id. The current phase deliberately does not introduce `ProgVariableTypes.Body`.

Query and switch functions:

| Function | Behaviour |
| --- | --- |
| `HasForm(character, form)` | True if the character owns the form. |
| `CurrentForm(character)` | Returns the current form alias. |
| `CurrentFormId(character)` | Returns the current body id. |
| `CanSeeForm(character, form)` | True if owner-facing visibility allows the form. |
| `CanSwitchForm(character, form)` | Checks voluntary switching plus structural validity. |
| `WhyCantSwitchForm(character, form)` | Returns the voluntary or structural denial message. |
| `SwitchForm(character, form)` | Attempts a voluntary switch. |
| `ForceSwitchForm(character, form)` | Attempts a non-voluntary switch path. |

Provisioning and metadata functions:

| Function | Behaviour |
| --- | --- |
| `EnsureForm(character, sourceKey, race[, ethnicity[, gender]])` | Ensures a prog-keyed form and returns the body id. |
| `SetFormAlias` / `SetFormSortOrder` | Edit presentation metadata. |
| `SetFormTraumaMode` | Edit trauma mode by name or numeric enum. |
| `SetFormTransformationEcho` / `ClearFormTransformationEcho` | Edit echo behaviour. |
| `SetFormAllowVoluntary` | Edit voluntary access. |
| `SetFormVisibilityProg` / `ClearFormVisibilityProg` | Edit owner-facing visibility. |
| `SetFormCanSwitchProg` / `ClearFormCanSwitchProg` | Edit voluntary eligibility. |
| `SetFormWhyCantProg` / `ClearFormWhyCantProg` | Edit voluntary denial text. |
| `SetFormShortDescriptionPattern` / `RandomiseFormShortDescriptionPattern` / `ClearFormShortDescriptionPattern` | Edit short description pattern. |
| `SetFormFullDescriptionPattern` / `RandomiseFormFullDescriptionPattern` / `ClearFormFullDescriptionPattern` | Edit full description pattern. |

## Supported User Stories

### A Player Voluntarily Changes Form

A character owns a visible form with voluntary access enabled. They type `form wolf`, pass the target form's optional can-switch prog, pass structural switch validation, and transform in place. Their player sees the transformation echo and the new form becomes their current body.

### A Hidden Form Is Kept Secret

A character owns a werewolf form with a false visibility prog. The `form` command does not list it, and `form werewolf` returns the generic no-such-form style response. When a full moon merit becomes applicable, the forced transformation can still resolve the hidden form because scripted and admin resolution can see all owned forms.

### Staff Adds a Test Form

An implementor uses `body addform` to add a robot, wolf, bird, ghost, or other race form to a live character. The new form is dormant, has voluntary access disabled unless staff enables it, and persists across save/load.

### A Racial Merit Grants a Reusable Form

A builder creates an `Additional Body Form` merit for a race that can transform. Characters who gain the merit receive a cached form. Removing and re-adding the merit reuses the same body id and keeps any admin or prog edits already made to that character's form metadata.

### A Full Moon Merit Forces Transformation

A werewolf merit has an applicability prog that is true during the full moon and auto-transform enabled. While the prog is true, the merit contributes a forced transformation demand. The resolver switches the character into the wolf-man form. When the prog becomes false, the resolver reverts them toward the recorded baseline form.

### A Spell Temporarily Overrides Another Form

A character is currently forced into a wolf-man form by a merit. A spell with a higher priority band or offset applies `transformform` into a sheep. The forced resolver picks the higher-priority spell demand. When the spell expires, the resolver re-evaluates and returns the character to the still-applicable wolf-man form, or to baseline if no forced demands remain.

### Organic and Inorganic Forms Do Not Corrupt Each Other

A wounded human switches into a robot form with `Automatic` trauma mode. Because the health strategies are incompatible, organic wounds remain in stasis on the human body and robot health state is sanitised. Bleeding, infection, and organic internal bleeding do not continue ticking while the robot form is active. Switching back restores the organic form's own stashed trauma.

### Content Scripts Create and Edit Forms

A FutureProg can call `EnsureForm` with a stable source key, then set alias, visibility, trauma mode, transformation echo, description patterns, and voluntary progs. Later calls with the same key reuse the same body.

### Reboot Does Not Lose Active Transform State

Owned forms, form metadata, source mappings, character-scoped skills, body-scoped attributes, spell transform runtime data, and forced transformation baselines are persisted. A character who reboots while transformed should load with the same current form and still be able to revert when the transform demand ends.

## Authoring Guidance

- Use a merit for inherent or racial forms that should exist as part of the character's identity.
- Use a merit with `autotransform` for rule-driven mandatory states such as full moons, curses, divine marks, or species biology.
- Use `transformform` for temporary magic or power effects that should create or reuse a specific spell-owned form and then revert.
- Use `EnsureForm` and form metadata setters for bespoke scripted content, event tools, or systems that do not yet have a dedicated coded source.
- Use visibility progs for surprise forms, conditional discovery, drug-induced awareness, or forms that should be visible only while a prerequisite effect is active.
- Prefer `Automatic` trauma mode unless a content concept has a specific reason to force transfer or stasis.
- Give animal, robot, ghost, and other non-human forms their own description patterns instead of relying on the source character's description pattern.
- Use priority bands for broad precedence and offsets for local ordering inside the same band. Avoid relying on alias ordering except as a deterministic tie-breaker.
- Treat `body addform` as a staff test and repair tool, not the final in-play acquisition workflow.

## Current Boundaries

- Only one body is active at a time.
- Inactive bodies do not occupy rooms, perceive, act, fight, or receive independent effects.
- Remote vessel jumping, astral projection with a shell left behind, and ghost bodies that coexist with corpses are not implemented.
- Source mappings find cached bodies; they do not continuously sync form metadata from the source definition.
- Hidden forms are hidden only from owner-facing player resolution. Scripts, admin commands, and forced transform resolution can still see all owned forms.
- Drug and chemical transformation priority is modelled, but a dedicated first-class drug transformation demand source is not yet implemented.
- Form APIs use aliases and body ids rather than a `Body` FutureProg variable type.

## Future Work Planned

### Dedicated Drug and Chemical Transform Source

Add a first-class drug or liquid effect that can provision a form, contribute a `DrugOrChemical` forced transformation demand, and automatically withdraw that demand as metabolism or dosing changes. This should reuse the same source mapping, priority, baseline, echo, trauma, and description-pattern mechanics already used by merits and spells.

### Builder-Facing Priority Introspection

Add admin output that explains all currently active forced transformation demands on a character, their priority bands, offsets, winning target, baseline body, and next recheck cadence. This will help builders debug "why am I a sheep instead of a wolf" cases.

### Richer Reversion Policies

Currently forced transforms share a baseline-and-fallback strategy. Future content may need per-source reversion options, such as revert only if still in my form, revert to a named form, do not revert, or restore the highest surviving lower-priority demand.

### Simultaneous Bodies and Remote Presence

Astral projection, ghosts with corpses left behind, possession, puppeting, and remote vessels require a larger presence model. That future slice needs to answer which body perceives, which body receives commands, which body is locatable, which body owns combat state, and how effects target identity versus embodiment.

### Form Acquisition Workflows

Merits, spells, admin commands, and FutureProg are enough for the current phase. A future workflow could add builder-friendly packages for racial transformation suites, curse templates, transformation rituals, and lifecycle hooks for form unlocks in play.

### Explicit Bodypart Mapping Authoring

The current mapper has strict matching plus coded best-fit scoring. Builders may eventually need explicit per-race or per-bodyprototype mapping tables for special anatomy, especially for high-fidelity transformations, cybernetic conversion, or surgical continuity.

### Expanded Test Harnesses

The current regression coverage should grow around live health ticking, long-duration stasis, repeated save/load while transformed, spell expiry fallbacks, merit cadence behaviour, and edge cases with restraints, combat, mounted movement, surgery, dragging, and carried characters.

### Deeper Character and Body Layer Cleanup

Multiple forms exposed older assumptions where character and body data were intertwined. Future cleanup should continue moving identity state to character scope and embodiment state to body scope, with compatibility aliases only where legacy code still needs them.

## Related Documents

- [Character Creation Runtime](Character_Creation_Runtime.md)
- [Character Creation Builder Workflows](Character_Creation_Builder_Workflows.md)
- [Character Description System](Character_Description_System.md)
- [Magic System Spells](Magic_System_Spells.md)
- [Magic System Implemented Types](Magic_System_Implemented_Types.md)
- [Health System Design](Health_System_Design.md)
