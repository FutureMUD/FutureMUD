# Seeding staged performances

A setting package can offer a published script that organisers can select immediately or copy into a draft. The organiser supplies the actual cast, rooms, exits, equipment and booking time in the running game. [Builder walkthrough](../Building/Staged_Performance_Builder_Guide.md)

## Create the work

1. Choose a stable key such as `my-setting.procession`. Use lowercase ASCII letters, digits, periods or hyphens. Include a package prefix separated by a period. Keep this key when rerunning the package.
2. Build a `PerformanceWork` with a display name, symbolic places, roles and an ordered list of cues. Define route legs, props, failure direction and cleanup policies as needed. Each required role needs a cue. The cue reference in the builder guide applies to the seeder as well.
3. If a role has an eligibility rule, resolve its installed FutureProg and save the prog and parameter rows before referencing its ID. The prog must return Boolean and accept a Character argument. A public rule is the default. For a work intended for staff staging with a private rule, explicitly pass `trustedStaffAuthored: true`. Players copying such a work must replace or clear private rules before they can publish it.
4. Call the helper from the owning package. Sources linked into SeederFoundations use the engine assembly alias shown below. The method returns `Created` or `AlreadyInstalled`.

```csharp
extern alias EngineCompiler;

using DatabaseSeeder.Seeders.Utilities;
using EngineCompiler::MudSharp.RPG.Performances;
using MudSharp.Database;

// Call from the setting package after constructing its definition.
static StagedPerformanceSeedResult InstallWork(
	FuturemudDatabaseContext context, PerformanceWork definition)
{
	return StagedPerformanceSeederHelper.EnsureWork(
		context, "my-setting.procession", definition);
}
```

Populate `Roles` with `PerformanceRole` entries. Each needs a `Key`, a starting `Mark` from `Places`, a `Mode` and a `Required` flag. For example, a lantern bearer could have mode `PerformanceMode.Either`. For a pickup cue, set `Verb = PerformanceVerb.Get`, `Role` to the bearer's key, `Place` to the pickup mark and `Target` to a declared prop key. In the DTO, use an empty string for unused cue fields. The dash is an in-game editor convention.

## Persistence and reruns

The helper copies its input, runs the game's structural validator and checks persisted eligibility rules. On a successful insert it calls `context.SaveChanges()` on the supplied context, so include it in the package's normal transaction. It writes a `StagedPerformanceRecord` with kind `work`, revision 1, published status, owner ID zero and the stable `SeedKey`. The runtime uses the row ID as the work's ID. Editing a published revision or copying a work starts a draft without that key.

A repeat with the same key and display name preserves the stored script, including local changes. A duplicate key, a changed name for an installed key, an unpublished matching work, or a new key colliding with an existing work's name raises an error. Inspect the existing work before deciding whether the package needs a migration. Existing productions reference a specific revision.

Run one installer at a time against a database. The key is stored in definition JSON and has no database uniqueness constraint. Deploy the helper alongside a runtime that understands the key. Installing the helper alone adds no catalogue entries.

## Supply a setting-specific help topic

The built-in `help performance` and `help stage` entries belong to their command modules. To install an additional topic about a particular rite or venue, choose a distinct name and call `HelpfileSeederHelper.EnsureHelpfile(context, name, update)` in the same utilities namespace. This finds or creates the named topic case-insensitively. The callback runs on every invocation. Choose the fields your package owns, and save the context after updating them.

For a new topic, set `Category`, `Subcategory`, `Keywords`, `TagLine`, `PublicText`, `LastEditedBy` and `LastEditedDate`. Use `SubstituteANSIColour()` from `MudSharp.Framework` on colour-marked text before assigning `PublicText`. A null `RuleId` makes the topic public. To restrict the entire topic, supply a Boolean prog accepting a Toon. For a public article with a staff-only section, store that section in `HelpfilesExtraTexts` with a Boolean rule accepting a Character. Set its display order, and update or reuse that entry on reruns so readers see it once.
