# Character Creation Seeder

## Purpose
`ChargenSeeder` installs the stock character-creation scaffold for a new game.

It does three broad jobs:
- seeds optional chargen resources
- seeds chargen helper FutureProgs and static configuration
- seeds the canonical storyboard graph and default starting-location role

## Prerequisites
The stock chargen seeder expects:
- at least one account
- the `Human` race

In practical terms, a fully usable chargen also benefits from later content packages such as:
- `CultureSeeder`
- `SkillSeeder` or `SkillPackageSeeder`
- `StockMeritsSeeder`

## Seeder Questions
The stock answers currently cover:
- whether to seed an RPP-style account resource
- the name and alias for that resource
- whether to seed Build Points
- whether classes are in use
- whether subclasses are in use
- whether chargen is role-first or race-first
- whether attributes use ordered rolls or point-buy
- whether skills use simple picks or paid boosts
- whether merits/flaws or quirks are used
- whether custom descriptions are allowed

## What Gets Seeded
### Resources
Optional stock resources:
- RPP or karma-style resource
- Build Points

### Static configuration
The seeder also owns:
- `SpecialApplicationCost`
- `SpecialApplicationResource`

These are seeded even when no paid chargen resource exists, in which case they are set to `0`.

### Helper FutureProgs
Always-seeded helpers include:
- age bounds
- height bounds
- weight bounds
- free skills
- skill pick counts
- knowledge pick counts
- free knowledges

Conditional helpers include:
- attribute point-buy helpers when the seeded attribute stage is `AttributePointBuy`
- skill boost pricing when the seeded skill stage is `SkillCostPicker`

### Storyboards
The seeder installs one canonical storyboard row per `ChargenStage`.

The stock graph includes:
- welcome
- special application
- roles, race, ethnicity, gender, culture
- handedness, birthday, height, weight, name
- disfigurements
- merits or quirks
- attributes
- skills
- accents
- knowledges
- characteristics
- descriptions
- starting location
- notes
- submit
- menu

### Default role
The seeder creates the default starting-location role named `Default Starting Location`.

It now assigns the first available account as the poster instead of assuming account id `1`.

## Stock Answer Effects
### Role-first versus race-first
This only changes the early ordering of role and heritage stages.

### Merits versus quirks
This chooses whether the stock `SelectMerits` stage starts as:
- `MeritPicker`
- `QuirkPicker`

### Attribute mode
This chooses whether `SelectAttributes` starts as:
- `AttributeOrderer`
- `AttributePointBuy`

If point-buy is selected:
- Build Points use a more generous stock paid-boost setup
- RPP-only setups use a smaller paid-boost setup
- no-resource setups seed free-only point-buy by setting extra paid boosts to zero

### Skill mode
This chooses whether `SelectSkills` starts as:
- `SkillPicker`
- `SkillCostPicker`

If paid boosts were requested but no chargen resource exists, the seeder now falls back to `SkillPicker` so the stock graph remains playable.

## Special Applications
The stock special-application screen now documents the real engine behavior:
- the screen marks the application type
- builders can use that type inside FutureProg gating
- a stock surcharge is configured through static settings
- no stock text promises automatic bonus RPP or other hidden behavior

## Repeatability
`ChargenSeeder` is intended to be rerunnable.

Current repeatability rules are:
- chargen resources are upserted by stable names and aliases
- helper progs are upserted by function name
- special-application static settings are upserted by setting name
- the default starting-location role is upserted by stable name
- storyboard ownership is one canonical row per `ChargenStage`

## What Reruns Repair
Reruns can repair:
- missing stock stages
- duplicate stock rows for the same stage
- missing helper progs
- missing special-application settings
- missing default starting-location role
- missing stage dependencies

## What Reruns Preserve
If a stage already has storyboard XML, the seeder preserves that existing definition rather than overwriting it with stock placeholder text.

That is intentional so builders do not lose their chargen copy and tuning just because they reran the package.

## Builder Follow-Up Checklist
After seeding, builders should usually:
1. Replace placeholder blurbs on the welcome, role, description, and note screens.
2. Replace the guest-lounge starting-location placeholder.
3. Decide whether the stock special-application surcharge is correct.
4. Review attribute and skill costs if using paid chargen resources.
5. Seed cultures, skills, merits, and other real content packages before opening chargen to players.
