# Disfigurement System Design

## Purpose
This document is the implementation and maintenance reference for the FutureMUD disfigurement system.

The builder-facing seeder handoff companion lives in `Disfigurement_Seeder_Builder_Reference.md`.

It covers:

1. The shared disfigurement model used by tattoos and scars.
2. The runtime flow for tattooing and automatic scar generation.
3. Builder, admin, chargen, persistence, and display behaviour.
4. The seeder scaffolding for adding stock tattoo and scar templates.
5. Guidance for tuning, testing, and safely extending the subsystem in future.

The disfigurement system is intentionally optional. A game may choose to seed no tattoo templates, no scar templates, or neither. The engine must treat that as a valid configuration and quietly do nothing rather than erroring.

## Core Concepts

### Shared abstraction
At the interface level:

- `IDisfigurement` is the common runtime abstraction for a concrete mark on a specific bodypart.
- `IDisfigurementTemplate` is the shared authoring abstraction for reusable templates.
- `IScar` and `ITattoo` are the concrete runtime mark types.
- only tattoos retain a reusable template type (`ITattooTemplate`)

In practice:

- a tattoo template is the builder-authored reusable design
- a disfigurement is the instance attached to a body
- tattoos are intentionally applied
- scars are chosen in chargen, added administratively, or generated automatically from healing
- scars now persist their generated description and metadata directly rather than referring back to a scar template

### Optional subsystem rule
The engine must always assume that a world may have:

- no tattoo templates
- automatic scarring disabled entirely
- no chargen storyboard that permits scar selection

All of those cases are valid and should resolve to a no-op, not a warning or exception.

## Runtime Architecture

### Key code locations
- `FutureMUDLibrary/Body/Disfigurements/`
  - interfaces for disfigurements and templates
- `MudSharpCore/Body/Disfigurements/`
  - tattoo template implementations
  - concrete scar and tattoo runtime instances
  - factory loading
  - scar generation logic
- `MudSharpCore/Body/Implementations/BodyParts.cs`
  - body-level storage, add/remove, save/load of tattoos and scars
- `MudSharpCore/Commands/Modules/BuilderModule.cs`
  - builder OLC for tattoo and scar templates
  - tattoo inscription workflow
- `MudSharpCore/Commands/Modules/StaffModule.cs`
  - admin tattoo / scar give and remove commands
- `DatabaseSeeder/Seeders/`
  - shared disfigurement seeder utilities
  - human / animal / mythical seeder hook files

### Loading and persistence
Disfigurement templates are stored as `DisfigurementTemplate` records with an XML `Definition` payload.

Only tattoos are expected to use that storage going forward. Legacy scar template rows may still exist in old developer databases, but the runtime no longer depends on them.

Runtime loading works like this:

1. The game loads the database `DisfigurementTemplate`.
2. `DisfigurementFactory` dispatches by `Type`.
3. `TattooTemplate` parses the XML into strongly typed fields.

Body instances persist separately:

- body tattoos are serialized into the body record tattoo XML
- body scars are serialized into the body record scar XML
- loaded scars restore race/body context so size and presentation logic still work after reload
- scar XML now stores the generated descriptive fields directly

The system remains XML-backed rather than introducing dedicated scar-generation database columns. That is deliberate. The surgery-origin scar metadata needed for offline healing is stored in wound extra-info XML instead of a schema migration.

## Shared Template Behaviour

### Bodypart constraints
Both scars and tattoos can constrain where they apply by bodypart shape. This is intentionally shape-based rather than bodypart-name-based at runtime, because it lets a single template work across races with comparable anatomy.

Seeders also support bodypart aliases. Those aliases are resolved at seeding time into the canonical bodypart shape IDs for the relevant body.

Use:

- bodypart shapes when the template should apply broadly across anatomies
- bodypart aliases when authoring stock content for a specific seeded body and you want the seeder to resolve the right shapes automatically

### Chargen support
Both scars and tattoos can be:

- allowed in chargen
- gated by an optional FutureProg
- assigned chargen resource costs

This lets a game use them as narrative flavour, social markers, or culture-specific selections.

### Description overrides
Both template types support optional special characteristic override strings:

- a plain participle-style form
- a `with ...` style form

These are used by sdesc/tag presentation when the mark should produce a more specific descriptor than the generic heavily-scarred or heavily-inked presentation.

These fields are persisted in XML and must round-trip exactly. When touching this area, keep the save and load element names in sync and maintain backwards-compatible reads for older typoed field names where needed.

## Tattoos

### What tattoos model
Tattoos are deliberate artwork applied to a target bodypart. A tattoo template describes:

- minimum bodypart size
- allowed bodypart shapes
- required tattooist knowledge, if any
- minimum tattooist skill
- acceptable ink colours with weights
- optional named text slots for reusable written text
- optional chargen availability and costs
- optional sdesc override strings

### Tattoo text slots
Tattoo templates now support named text slots for reusable written text. Template descriptions reference these slots with `$template{slotName}` tokens, which are expanded into generated `writing{...}` markup at tattoo render time.

This is the intended solution for reusable tattoos whose wording changes between instances, such as:

- heart-and-banner tattoos with different names
- devotional tattoos with personalised phrases
- insignia with custom mottos

Each text slot stores:

- a stable slot name
- a maximum text length
- whether custom text is required in interactive workflows
- fallback language
- fallback script
- fallback writing style flags
- fallback colour
- fallback minimum reading skill
- fallback readable text
- fallback unreadable alternate text

Rendering is deliberately two-step:

1. resolve `$template{slotName}` tokens to generated `writing{...}` markup using the tattoo instance value, or the slot fallback if no custom value exists
2. run the normal written-language substitution for the viewer

Existing literal `writing{...}` markup remains fully valid in tattoo descriptions for backwards compatibility and for tattoos that do not need templated custom text.

### Builder workflow
Tattoos have full builder support through the `tattoo` command family:

- list, show, edit, create, clone, set, and submit templates
- admin review of submitted templates

Important builder-facing properties include:

- `size`
- `bodypart`
- `knowledge`
- `skill`
- `ink`
- `chargen`
- `chargenprog`
- `textslot`
- `override`

Tattoo text slots are builder-authored separately from the description text itself. Builders define the slot and then reference it with `$template{slotName}` in the short and full description.

The `textslot` editor supports:

- add and remove
- required or optional behaviour
- maximum length
- fallback language and script
- fallback style flags
- fallback colour
- fallback minimum skill
- fallback readable text
- fallback alternate text

Template submission validation rejects tattoo templates that reference undefined text slots.

### In-game tattoo workflow
Tattooing is a deliberate action workflow rather than an instant attach:

1. A tattooist starts work with `tattoo inscribe`.
2. The actor needs the expected tooling and ink resources.
3. If the tattoo has required text slots, the text values must be supplied before work begins.
4. Progress is tracked across repeated work ticks.
5. A partially completed tattoo has staged descriptions driven by static configuration text.
6. The work can be resumed with `tattoo continue`.
7. When complete, the final tattoo instance is attached to the target body.

Tattoo text can currently be supplied in two ways during inscription:

- typed text for a named slot
- copied text from an existing writing by writing id

Typed text uses the template slot fallback metadata for language, script, style, colour, and minimum reading skill.

Copied text preserves the source writing metadata. Tattooists are allowed to copy writing they can target even if they cannot read it, but this marks the tattoo with a major unreadable-copy penalty that makes the inking check substantially harder.

Supporting static configuration includes things like:

- `TattooInkLiquid`
- `TattooistTrait`
- `TattooNeedleTag`
- `TattooInkTotalPerSize`
- `TattooingTicksPerSize`
- `InkingTattooTickDurationSeconds`
- `TattooSkillPerDifficulty`

### Tattoo rendering and consistency rule
Tattoo presentation is now expected to use a single tattoo-specific resolution path:

- resolve `$template{slotName}` placeholders
- preserve existing literal `writing{...}` markup
- always pass the final result through written-language parsing

That rule applies to tattoo short descriptions, full descriptions, keyword extraction, chargen previews, and in-progress tattoo displays. Raw template descriptions should not be shown directly for tattoos.

### Tattoo data guidance
Good tattoo templates should:

- target shapes that make anatomical sense
- use minimum bodypart size to prevent tiny-bodypart nonsense
- use knowledge gates for cultural, ritual, faction, or specialist designs
- use minimum skill to distinguish simple work from expert work
- list only plausible ink colours and weight them intentionally

Avoid using tattoos as a proxy for scars or other injuries. They should stay deliberate, authored, and intentional.

## Scars

### What scars model
Scars are healed remnants of injuries or surgery. A scar instance stores:

- generated short and full descriptions
- size offset relative to the bodypart
- distinctiveness
- whether the scar came from damage or surgery
- damage type, severity, and optional surgery type
- optional sdesc override strings

The runtime chooses those values programmatically from the wound context and from static scar-generation configuration.

### How automatic scar generation works
Automatic scar generation runs when eligible wounds are removed during healing.

High-level flow:

1. Confirm `ScarringEnabled` is true.
2. Confirm the wound belongs to an external bodypart still on the body.
3. Compute the overall chance that a scar happens from the wound context plus global tuning values.
4. If the roll succeeds, generate a scar description, size step, distinctiveness, and any characteristic overrides from:
   - bodypart shape
   - damage or surgery origin
   - severity
   - static scar orientation configuration
5. Attach the generated scar instance to the body.

### Scar chance math
For an ordinary organic damage wound, the implemented structure is:

`chance = clamp(matrix damage base + tended modifier + infection modifier + cleanliness modifier + antiseptic modifier + closure-state modifier)`

For an organic surgery-origin wound:

`chance = clamp(matrix surgery base + tended modifier + surgery check modifier + infection modifier + cleanliness modifier + antiseptic modifier + closure-state modifier)`

For a serialized healing wound with reduced state available:

- damage-origin healing wounds use matrix damage base + tending
- surgery-origin healing wounds use matrix surgery base + surgery check + tending

The current clamp is:

- lower bound: `0.0`
- upper bound: `ScarGenerationChanceClampMaximum`

- `ScarGenerationOverallChanceUpperBound`

### Scar tuning inputs
Scar generation has two layers of tuning:

1. Static configuration chance matrices
2. Global static configuration modifiers

Static configuration owns the baseline matrices and the previous hard-coded coefficients:

- `ScarringEnabled`
- `ScarGenerationOverallChanceUpperBound`
- `ScarGenerationChanceClampMaximum`
- `ScarGenerationChanceMatrix`
- `ScarOrientationByBodypartShape`
- `ScarChargenOptionsCount`
- `ScarGenerationOrganicSurgerySeverityPerLevel`
- `ScarGenerationOrganicSurgeryHadInfectionModifier`
- `ScarGenerationOrganicSurgeryCleanedModifier`
- `ScarGenerationOrganicSurgeryUncleanModifier`
- `ScarGenerationOrganicSurgeryAntisepticModifier`
- `ScarGenerationOrganicSurgeryClosedModifier`
- `ScarGenerationOrganicSurgeryTraumaControlledModifier`
- `ScarGenerationOrganicSurgeryOpenModifier`
- `ScarGenerationOrganicDamageSeverityPerLevel`
- `ScarGenerationOrganicDamageHadInfectionModifier`
- `ScarGenerationOrganicDamageCleanedModifier`
- `ScarGenerationOrganicDamageUncleanModifier`
- `ScarGenerationOrganicDamageAntisepticModifier`
- `ScarGenerationOrganicDamageClosedModifier`
- `ScarGenerationOrganicDamageTraumaControlledModifier`
- `ScarGenerationOrganicDamageOpenModifier`
- `ScarGenerationHealingSurgerySeverityPerLevel`
- `ScarGenerationHealingDamageSeverityPerLevel`
- `ScarGenerationTendedOutcomeMajorPassModifier`
- `ScarGenerationTendedOutcomePassModifier`
- `ScarGenerationTendedOutcomeMinorPassModifier`
- `ScarGenerationTendedOutcomeMinorFailModifier`
- `ScarGenerationTendedOutcomeFailModifier`
- `ScarGenerationTendedOutcomeMajorFailModifier`
- `ScarGenerationTendedOutcomeDefaultModifier`
- `ScarGenerationSurgeryCheckDegreesAtLeastThreeModifier`
- `ScarGenerationSurgeryCheckDegreesTwoModifier`
- `ScarGenerationSurgeryCheckDegreesOneModifier`
- `ScarGenerationSurgeryCheckDegreesZeroModifier`
- `ScarGenerationSurgeryCheckDegreesMinusOneModifier`
- `ScarGenerationSurgeryCheckDegreesMinusTwoModifier`
- `ScarGenerationSurgeryCheckDegreesMinusThreeOrLessModifier`

### Interpreting the scar math
The chance matrix should represent the baseline likelihood that a wound of a given origin and severity scars at all.

The static modifiers then control the world-level style:

- positive severity modifiers make worse injuries scar more often
- positive infection / poor treatment modifiers make bad care scar more often
- negative antiseptic / closed / strong tending / good surgical outcome modifiers reward good care

This split is intentional:

- static scar data controls description-shape flavour and orientation pools
- world tuning controls how forgiving or gritty the healing model feels overall

### Scar data guidance
Good scar generation data should:

- map bodypart shapes to orientation pools that make anatomical sense
- keep the descriptive families broad enough to work across many wounds
- reserve special characteristic overrides for genuinely notable facial scarring
- keep the chance matrix moderate and let the global tuning do most of the world-style work

As a rule of thumb:

- templates define eligibility and flavour
- static config defines campaign tone

## Builder and Admin Surface

### Builder OLC
Only tattoos still support the revisable-item workflow.

The `scar` builder command is now informational only and points builders toward the generated-scar model plus the staff and chargen workflows that consume it.

### Admin commands
Staff have direct manual intervention tools:

- tattoos can be given, finished, and otherwise managed through the existing tattoo support
- scars can be applied with `givescar <target> <bodypart> <damage|surgery> <type> <severity>`
- scars can be removed with `removescar`

Interactive tattoo-creation paths must respect required tattoo text slots:

- chargen prompts for required slot values before a tattoo choice is finalised
- `givetattoo` accepts named slot values when creating tattoos administratively
- tattoo inscription accepts named slot values before work begins

Optional slots may be omitted and will use their fallback values.

Manual scars remain valid even if automatic scarring is disabled.

## Seeder Scaffolding

### Design goals
The seeder support for disfigurements was added to make stock content easy to add later without forcing every game to use it.

The scaffolding therefore has these requirements:

- no stock templates are required
- empty definition lists must no-op cleanly
- rerunning the seeder should not duplicate seeded templates
- human, animal, and mythical seeders should all have extension points

### Shared utility
`SeederDisfigurementTemplateUtilities` remains the core authoring helper for tattoo templates.

It provides:

- `SeederDisfigurementTemplateDefinition`
- `SeederTattooTemplateDefinition`
- `SeederTattooTextSlotDefinition`
- `SeedTemplates(...)`
- `HasMissingDefinitions(...)`

Responsibilities of the utility:

- resolve bodypart shapes and aliases
- resolve chargen costs and progs
- resolve tattoo knowledge and ink colours
- resolve tattoo text-slot fallback language, script, and colour names
- create or update current template records by stable `(type, name)` identity
- keep seeded records idempotent on rerun

For the current resolver value catalogue and builder-friendly authoring guidance, see `Disfigurement_Seeder_Builder_Reference.md`.

### Seeder hook points
The system is intentionally split by seeder:

- `HumanSeeder.Disfigurements.cs`
  - stock tattoo templates
- `AnimalSeeder.Disfigurements.cs`
  - per-race optional tattoo template collections
- `MythicalAnimalSeeder.Disfigurements.cs`
  - per-race optional tattoo template collections

This split matches how anatomy differs between seeded bodies.

### How to author good seeder data
For humans:

- add reusable human-centric template definitions to the dedicated human lists
- prefer shape names for broad templates and aliases for specific anatomical intent

For animals and mythical creatures:

- attach template lists to the relevant race template definition
- keep the template coupled to the body key it was authored against
- prefer alias-based authoring when a feature is body-layout-specific

General authoring guidelines:

- stable names matter because seeder idempotency uses `(type, name)`
- descriptions should read well both in chargen and in live description output
- do not seed templates that are too setting-specific unless the seeder package explicitly owns that setting tone
- avoid seeding very broad scars with high baseline chance because they can crowd out more specific templates
- keep seeded tattoo colours plausible for the setting and available material catalogue

### Example seeder thinking
A good seeded facial scar should usually:

- resolve to face-compatible shapes only
- allow only the damage types that should plausibly create it
- set a low to moderate base chance
- rely on bad-care and severity modifiers to make it common in brutal worlds

A good seeded ritual tattoo should usually:

- require a meaningful knowledge
- require non-trivial skill
- list a focused ink palette
- limit bodypart shape and size appropriately

## Guidelines for Future Changes

### Preserve optionality
Whenever changing this subsystem, keep the no-template and disabled-system cases first-class.

Do not:

- assume any scar templates exist
- assume any tattoo templates exist
- assume animal or mythical seeders populate any disfigurements
- throw because a wound healed without producing a scar

### Keep load and save symmetric
Most disfigurement state is XML-backed. Whenever adding fields:

1. add them to the interface if they are part of the public contract
2. add them to the implementation
3. save them
4. load them
5. cover them with round-trip tests

If backwards compatibility matters, support both the old and new XML element names during reads.

### Prefer interface-first changes
This subsystem spans shared interfaces, runtime classes, commands, and the seeder. If a new concept is real enough to be reused, put it in the interface first and let the downstream implementations follow.

### Be cautious with scar probability changes
Changing scar math can have very visible world-tone effects. Before changing formulas:

- decide whether the change belongs in template data or static config
- avoid sneaking world-tone changes into code constants
- think about multi-template overlap because the overall roll uses complementary probability
- think about offline healing and wound serialization, not only online wound objects

### Prefer seeded specificity over giant catch-all templates
Very broad templates seem convenient, but they flatten world flavour and make the weighted-choice model less expressive. Multiple narrow templates usually produce better outcomes than one generic omnipresent scar or tattoo.

### Test expectations
Changes in this area should usually consider:

- template XML round-trip coverage
- body scar / tattoo persistence round-trip coverage
- `DisfigurementFactory` type dispatch coverage
- tattoo text-slot fallback and custom-value rendering coverage
- chargen persistence of selected tattoo text values
- scar-generation behaviour with `ScarringEnabled` off
- scar-generation description output for representative bodypart shapes and damage families
- seeder idempotency and empty-definition behaviour

## Practical Maintenance Checklist
When fixing or extending the disfigurement system:

1. Check whether the change affects the shared interfaces.
2. Check both template XML save and load paths.
3. Check builder OLC, admin surface, and chargen behaviour.
4. Check persistence of live body instances.
5. Check optional no-template behaviour.
6. Check seeder repeatability if stock data is involved.
7. Update this document if the runtime, workflows, persistence, or tuning model changed.

## Current System Summary
- Tattoos are a fully authorable and actively inscribable deliberate disfigurement type.
- Scars are a fully authorable disfigurement type that can now also arise automatically from healing and surgery recovery.
- Automatic scarring is globally toggleable and safely dormant in games that seed no scar templates.
- Seeder scaffolding exists for human, animal, and mythical disfigurement templates, but ships empty by default so games can opt in deliberately.
