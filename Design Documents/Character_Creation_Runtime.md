# Character Creation Runtime

## Purpose
The character creation runtime is the staged application pipeline that players use to create, price, submit, and later review a character application.

It lives primarily in `MudSharpCore/CharacterCreation` and is assembled around a single `Chargen` object plus one storyboard screen per `ChargenStage`.

## Core Runtime Model
### `Chargen`
- `Chargen` is the mutable application state.
- It stores the current stage, completed stages, selected race/culture/ethnicity, notes, roles, traits, merits, skill boosts, disfigurements, descriptions, and starting location.
- It serialises the entire in-progress application to XML through `SaveToXml()` and reloads that XML through `LoadFromXml()`.
- It recalculates application costs by asking each completed screen, plus the active screen, for `ChargenCosts`.

### `ChargenStoryboard`
- `ChargenStoryboard` is the global chargen graph loaded from `ChargenScreenStoryboards`.
- It resolves exactly one active storyboard implementation for each `ChargenStage`.
- It tracks:
  - screen order
  - default next-stage routing
  - per-stage dependencies
  - the current storyboard type for each stage
- Builders can reorder stages, add or remove dependencies, and swap screen implementations for stages that support more than one screen type.

### `ChargenScreenStoryboard`
- Each storyboard owns the builder-facing configuration for one stage.
- Each storyboard type knows how to:
  - load its XML definition
  - save its XML definition
  - show builder information
  - create a live `IChargenScreen` for a player
  - report chargen costs for the current application

### `IChargenScreen`
- A live screen handles one player-facing stage interaction.
- It renders the prompt for the current stage and processes input until that stage is complete.

## Stage Graph
The default stock graph is seeded as one canonical row per `ChargenStage`.

Important stages in the stock flow are:
- welcome
- special application mode
- role and heritage selection
- handedness, birthday, height, and weight
- name
- disfigurements
- merits or quirks
- attributes
- skills
- accents and knowledges
- characteristics and descriptions
- starting location
- notes
- submit
- menu

Not every stage has only one possible screen type. The most important alternatives are:
- `SelectMerits`: `MeritPicker` or `QuirkPicker`
- `SelectAttributes`: `AttributeOrderer` or `AttributePointBuy`
- `SelectSkills`: `SkillPicker` or `SkillCostPicker`
- `SpecialApplication`: the same stage can be configured to auto-short first-account applications

## Application Types
Chargen tracks `ApplicationType` explicitly:
- `Normal`
- `Simple`
- `Special`

Important notes:
- `Simple` is the short-form mode used by the special-application screen when first-account auto-short mode is enabled.
- `Special` is the explicit staff-reviewed mode that builders can reference in chargen FutureProg logic.
- The older `IsSpecialApplication` flag still exists for legacy compatibility, but the runtime now keeps it aligned with `ApplicationType`.

## Costs and Resources
Chargen costs are screen-driven.

Each storyboard can contribute one or more `(resource, cost)` pairs through `ChargenCosts`.

The runtime aggregates those costs by:
1. looking at completed stages plus the active stage
2. asking each storyboard for its current contribution
3. summing by `IChargenResource`

This means cost displays, submit validation, and review displays all come from the same screen-owned logic.

## Advice Aggregation
`Chargen.AllAdvice` is the combined advice stream for the current application.

It now aggregates advice from:
- selected race
- selected ethnicity
- selected culture
- selected roles

That makes chargen advice behave like a cross-cutting guide layer instead of a single-screen feature.

## Merits and Traits
Chargen implements `IHaveMerits` and `IHaveTraits`.

Practical implications:
- merits can be added and removed directly from the chargen model
- selected attributes and skill values present as traits for downstream logic
- FutureProg and runtime systems can interrogate chargen selections before a character exists in the world proper

### Merit-provided alternate forms
The chargen-selected merit list can now provision reusable alternate body forms through coded merits such as `Additional Body Form`.

Runtime behavior is:

- the merit contributes a first-creation `ICharacterFormSpecification`
- on character materialisation, character load, and live `AddMerit`, the runtime ensures the specified form exists
- if the character already has a sourced form mapping for that merit, the existing body is reused
- if no sourced mapping exists, the runtime first tries to adopt exactly one existing matching form before creating a new dormant body
- removing and later re-adding the merit reuses the cached form rather than deleting and recreating it
- merit provisioning alone does not auto-switch the character into the form

The merit's first-creation defaults can specify:

- race
- optional ethnicity
- optional gender
- initial alias
- initial sort order
- initial trauma mode
- initial voluntary-switch toggle
- initial voluntary can-switch and why-cant progs
- initial owner-visibility prog
- initial transformation echo
- initial short-description pattern
- initial full-description pattern

If no explicit short or full description pattern is supplied, the runtime tries to choose a random valid pattern for the target race/body configuration and only falls back to generic text when no valid pattern exists.

After a form exists, its per-character metadata is authoritative. Editing that form later through admin tools or FutureProg mutators does not get overwritten by the originating merit, including later edits to visibility, voluntary-switch rules, transformation echo, or description patterns.

## Review and Submission
Once an application is submitted:
- the current XML snapshot is stored in the database
- minimum approval authority is calculated from selected roles
- the review surface uses the same chargen object model to present the final application

The Discord review path also uses the same chargen runtime, rather than a separate export model.

## Persistence Notes
The chargen XML includes:
- current stage
- completed stages
- all selected appearance, heritage, role, note, and trait data
- selected skill boosts
- selected merits
- selected knowledges
- selected disfigurements, scars, tattoos, and prostheses
- selected starting location
- application type
- prior rejection history

## Builder-Facing Runtime Entry Points
The main in-game admin command surface is `chargen`.

Important commands include:
- `chargen overview`
- `chargen types`
- `chargen typehelp <type>`
- `chargen show <id|name>`
- `chargen edit <id|name>`
- `chargen set ...`
- `chargen reorder <stage> <after-stage>`
- `chargen changetype <stage> <new type>`

## Operational Guidance
- Treat storyboard XML as builder-owned once it has been customised for a real game.
- Prefer changing screen type in game with `chargen changetype` rather than rerunning the seeder to force a type conversion.
- Lock chargen before large graph changes so players do not sit inside screens that are being rebuilt.
