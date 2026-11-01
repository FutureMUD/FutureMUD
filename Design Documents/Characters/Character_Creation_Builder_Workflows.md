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

Use `chargenskillgroup` to author generic bounded skill entitlements separately from storyboards. See [Chargen Skill Selection Groups](./Chargen_Skill_Selection_Groups.md) for the complete command reference and a non-language exactly-two example.

Use the `chargen` command family.

The normal workflow is:
1. Review the current graph with `chargen overview`.
2. Inspect a screen with `chargen show <id|name>`.
3. Open it with `chargen edit <id|name>`.
4. Change its properties with `chargen set ...`.
5. Close the editing session with `chargen close`.

Use `chargen types` and `chargen typehelp <type>` before swapping a stage to a different screen implementation.

For NamePicker, `chargen set stylesprog <prog|none>` controls optional naming styles. The prog returns a name-culture collection and accepts chargen-compatible parameters (including Toon or any-parameter progs). Existing ethnicity/upbringing fallback remains available. `chargen set allowunicode` remains a separate control. Name-culture builder options `preservecase`, `exactgender` and `typographic` toggle their opt-in XML policies; absent options retain legacy behavior. See [naming runtime](Character_Creation_Runtime.md#structured-naming-and-encoding).

## Changing Screen Type
Some stages support alternative storyboard types.

Common examples are:
- merits versus quirks
- ordered attributes versus point-buy attributes
- simple skill picking versus paid skill boosts
- sequential, simple, or rerollable characteristic picking

Use `chargen changetype <stage> <type>` for these changes.

For `SelectCharacteristics`, choose `RerollableCharacteristicPicker` to show an ethnicity-based roll that players can reroll, lock, or customise. The screen's `intro` builder command toggles a separate blurb step; it is on by default. When it is off, a nonblank blurb appears above the rolled values. The `blurb` builder command edits that text. A player uses `lock <characteristic>`, `basiclock <characteristic>`, and `unlock <characteristic>` on the main selection screen. A basic lock is available when the current basic value has multiple eligible specific values.

For descriptions, `chargen changetype SelectDescription CustomDescriptionPicker` makes every applicant write a short and full description. The existing `DescriptionPicker` honours its `custom` and `patterns` settings, and automatically uses custom entry for a race with no characteristic definitions. Switching between the two description types preserves the blurbs and the existing type's saved settings; `CustomDescriptionPicker` always enforces custom entry while it is active. The characteristic stage automatically completes with an empty selection for a race with no definitions.

Important note:
- type changes can discard or reinterpret screen-specific configuration
- they should be treated as a rebuild operation, not a harmless cosmetic change

## Skipping Optional Selection Screens

Use `chargen changetype SelectAccents AccentSkipper`, `chargen changetype SelectMerits QuirkSkipper`, or `chargen changetype SelectDisfigurements DisfigurementSkipper` to replace a selection stage with an automatically completed screen. These stages display no prompt. Configure them through `chargen edit <screen>` and `chargen set ...` as usual.

All skipper progs are optional and take one `chargen` parameter. Use a prog ID or name, or `none` to clear the setting:

| Screen | Setting | Prog return type |
| --- | --- | --- |
| AccentSkipper | `accents <prog\|none>` | Accent collection |
| QuirkSkipper | `quirks <prog\|none>` | Merit collection |
| DisfigurementSkipper | `missingbodyparts <prog\|none>` | Void |
| DisfigurementSkipper | `prostheses <prog\|none>` | Void |
| DisfigurementSkipper | `scars <prog\|none>` | Void |
| DisfigurementSkipper | `tattoos <prog\|none>` | Void |

AccentSkipper clears old accents and discards a native-language choice if the character no longer knows that language. Like AccentPicker, it automatically grants the sole chargen-available accent for a known language. It then grants distinct accents returned by the prog for known languages, even if those accents are not available for manual selection. For any unassigned language, it uses the existing native/foreign/fallback accent rules among chargen-available accents. With no prog, those automatic selections still run. The character's explicit native-language choice, then ethnicity, culture, and skill levels determine the native-language fallback.

For example, an accent collection prog can return the native accent appropriate to `@ch.ethnicity`; the mapping belongs in that prog. QuirkSkipper clears previous selected merits and flaws, then grants distinct character merits returned by its prog. With no prog it leaves an empty selection. Automatic accent and quirk grants do not use manual pick limits or availability gates and do not cost chargen resources.

DisfigurementSkipper preserves existing selections and initializes missing selection lists, matching the disfigurement picker. It migrates legacy tattoo selections into the current tattoo list without duplicates. Its hooks run in this order: missing bodyparts, prostheses, scars, tattoos. Each hook modifies the chargen using these Boolean-returning FutureProg functions (false means an invalid or duplicate selection):

- `AddChargenMissingBodypart(chargen, "bodypart name or ID")`: adds a severable, non-vital external bodypart as a missing root.
- `AddChargenProsthesis(chargen, itemprototype)`: grants a prosthetic compatible with the body and a missing root; equivalent limb targets cannot receive two prostheses. Use `ItemPrototype(id)` to resolve an item prototype.
- `AddChargenScar(chargen, "bodypart", "damage type", "wound severity")`: generates a scar using the normal wound-scar rules. An optional fifth text parameter names a surgical procedure type for a surgical scar. Matching part/cause/severity selections are not added twice.
- `AddChargenTattoo(chargen, templateId, "bodypart")`: adds a fitting tattoo with default text-slot values. An optional fourth parameter is a text dictionary keyed by slot name; it supplies custom text using the slot's default writing settings. Required custom slots must be supplied and text must fit the slot length.

Scars and tattoos require a present external bodypart; tattoos also obey template shape and size restrictions. These automatic grants bypass manual chargen availability and resource costs. The hooks may run again when the stage is revisited, so keep other scripted side effects safe to repeat. With all hooks unset, existing disfigurements remain and no new ones are added.

Switching back to AccentPicker, QuirkPicker, MeritPicker, or DisfigurementPicker creates fresh picker settings. Configure blurbs, limits, allowed disfigurement sections and prosthetic choices before reopening chargen to players.

## Reordering and Dependencies
The storyboard graph has both order and dependency rules.

Use `chargen reorder` only when the new order still respects stage dependencies.

Use `chargen reorder <screen> <after-screen>` to move a screen immediately after another screen. A screen cannot follow itself. Choosing its existing predecessor leaves the graph unchanged. Successful moves update both the displayed order and the next-stage links, and both survive saving and reloading; terminal stages retain their existing termination behaviour.

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

## Suggested Skills
The `SkillPicker` storyboard can pre-select editable skill suggestions from earlier chargen choices. While editing that storyboard, use:

- `chargen set suggestedskills <prog>` to configure a FutureProg returning a collection of traits from one `chargen` parameter
- `chargen set suggestedskills none` to clear it

Suggested skills are not free grants. They consume normal picks and resources and remain removable on the skill screen. Keep profession or background mapping in the prog; the screen itself only filters its results to distinct, currently selectable skills and applies them in returned order up to the pick limit. If the picks prog depends on current chargen state, its allowance is recalculated for every suggested skill and again before the player can finish the screen.

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
- it can optionally force the character into that form whenever the merit applies
- forced merit transforms have a configurable priority band, priority offset, and fuzzy applicability recheck cadence
- the owning character keeps the provisioned body cached even if the merit is later removed or temporarily unavailable
- re-adding the same merit reuses the cached sourced form instead of generating duplicates
- the merit still provisions the reusable body even when automatic transformation is disabled

If you omit the form's short or full description pattern, the runtime will try to assign a random valid pattern for the provisioned form on first creation. If no valid pattern exists, it falls back to generic stored description text instead.

Use the owner-visibility prog when you want a form to exist for later scripted use without being openly listed to the player yet, such as hidden lycanthropy or drug/spell-gated transformations.

Recommended builder pattern for lycanthropy-style content:

- use the merit's normal `applicabilityprog` for the moon, curse, drug, or other condition
- set `autotransform true`
- set `priorityband merit`
- set `recheck hour` for slow world-state conditions such as lunar cycles, or `recheck minute` for shorter lived scripted gates

If multiple mandatory sources overlap, the engine resolves them by priority band first, so a spell-configured sheep form can temporarily override a full-moon merit form and then drop back to the merit form when the spell expires.

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
