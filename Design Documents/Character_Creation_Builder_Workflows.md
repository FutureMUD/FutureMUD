# Character Creation Builder Workflows

## Purpose
This document describes the practical builder workflow for maintaining chargen once the runtime and stock seeder are in place.

## The Main Rule
The seeder gives you a usable scaffold, not a finished game-specific chargen.

Builders are expected to customise:
- blurbs
- role catalogues
- race, culture, and ethnicity content
- starting locations
- costs
- approval logic
- application gating FutureProgs

## Working With Storyboards
Use the `chargen` command family.

The normal workflow is:
1. Review the current graph with `chargen overview`.
2. Inspect a screen with `chargen show <id|name>`.
3. Open it with `chargen edit <id|name>`.
4. Change its properties with `chargen set ...`.
5. Close the editing session with `chargen close`.

Use `chargen types` and `chargen typehelp <type>` before swapping a stage to a different screen implementation.

## Changing Screen Type
Some stages support alternative storyboard types.

Common examples are:
- merits versus quirks
- ordered attributes versus point-buy attributes
- simple skill picking versus paid skill boosts

Use `chargen changetype <stage> <type>` for these changes.

Important note:
- type changes can discard or reinterpret screen-specific configuration
- they should be treated as a rebuild operation, not a harmless cosmetic change

## Reordering and Dependencies
The storyboard graph has both order and dependency rules.

Use `chargen reorder` only when the new order still respects stage dependencies.

Typical dependency examples are:
- birthday depends on culture
- height depends on race and gender
- weight depends on height
- starting location depends on roles

If a reorder would move a screen ahead of something it depends on, the command blocks the change.

## Application Modes
Builders should decide how much chargen differentiation they want between:
- normal applications
- simple applications
- special applications

The stock special-application screen does not, by itself, grant extra options.

Instead, it marks the application type so your FutureProgs can:
- allow restricted races or roles
- waive or add costs
- skip heavy stages for simple applications
- demand stronger review gates for special applications

## Costs
Screen-owned costs are the preferred pattern.

Examples:
- role costs belong on role selection content
- merit or quirk costs belong on those items
- attribute point-buy costs belong on the attribute screen
- special-application cost belongs on the special-application screen via static configuration

This keeps the cost display and submit validation in sync.

## Resources
Chargen resources are general-purpose account resources.

The stock package can seed:
- an RPP-style resource
- Build Points

Builders should later decide:
- which resource gates premium choices
- whether special applications cost a resource surcharge
- whether paid skill boosts and paid attribute boosts are appropriate at all

## Starting Locations
The stock package seeds one default starting-location role and one guest-lounge location entry.

Before going live, builders should:
- replace the guest-lounge placeholder
- add real starting areas
- add any commencement progs
- decide whether the screen should skip automatically when there is only one valid choice

## Content Follow-Up After Seeding
The chargen scaffold becomes much more useful after running related seeders:
- `CultureSeeder`
- `SkillSeeder` or `SkillPackageSeeder`
- `StockMeritsSeeder`

Without those follow-up packages, many chargen screens will still exist but have little or no meaningful content to present.

## Merit-Driven Alternate Forms
If you want chargen or racial selection to grant a reusable transformation body, use a coded merit such as `Additional Body Form` rather than an ad hoc spell-only workflow.

Builder expectations for this merit family are:

- the merit defines first-creation defaults for the provisioned form
- it can specify race, ethnicity, gender, alias, sort order, trauma mode, voluntary-switch rules, an owner-visibility prog, an optional transformation echo, and optional short/full description patterns
- the owning character keeps the provisioned body cached even if the merit is later removed or temporarily unavailable
- re-adding the same merit reuses the cached sourced form instead of generating duplicates
- the merit itself provisions access only; it does not force an immediate switch into that form

If you omit the form's short or full description pattern, the runtime will try to assign a random valid pattern for the provisioned form on first creation. If no valid pattern exists, it falls back to generic stored description text instead.

Use the owner-visibility prog when you want a form to exist for later scripted use without being openly listed to the player yet, such as hidden lycanthropy or drug/spell-gated transformations.

## Safe Rerun Expectations
Rerunning `ChargenSeeder` is intended to repair missing stock pieces, not to be the primary builder workflow for changen redesigns.

The safe expectations are:
- missing stock stages can be restored
- missing helper progs can be restored
- duplicate stock storyboard rows are collapsed back to one canonical row per stage
- existing storyboard XML is preserved when that stage already has content

The unsafe expectation is:
- do not assume rerunning the seeder will migrate a heavily customised screen to a different storyboard type for you

If you want a different screen type, use the in-game chargen editing tools and then rebuild that screen deliberately.
