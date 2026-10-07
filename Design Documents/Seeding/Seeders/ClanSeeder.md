# ClanSeeder

## Start here to add content

Edit [ClanSeeder.Templates.cs](../../../DatabaseSeeder/Seeders/ClanSeeder/ClanSeeder.Templates.cs): **SetupNavalOrganisationClan or another guarded template setup; AddRank / AddAppointment / AddPaygrade**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Set up a few common clan templates.
Baseline availability: **Enabled**. [ClanSeeder.cs](../../../DatabaseSeeder/Seeders/ClanSeeder/ClanSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- The Time seeder must have installed at least one clock.
- The Currency seeder must have installed at least one currency.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: SetupNavalOrganisationClan guards context.Clans.Any(Name == "Naval Organisation Template"), calls CreateTemplateClan then AddRank for Seaman Recruit and other ranks. CreateTemplateClan constructs Clan with IsTemplate=true, monthly pay interval and calendar/timezone fields, then adds context.Clans and saves. AddRank creates Rank plus RanksAbbreviations and RanksTitles rows and adds context.Ranks. Add a clan through a guarded Setup method, call CreateTemplateClan, then AddRank/AddAppointment/AddPaygrade/link helpers; add its setup call to SeedData and name to ShouldSeedData detection. A rank requires name, abbreviation, rank number, privileges and RankPath. Explicit outer transaction; stable template names required for idempotency and clone conventions in ClanSeeder AGENTS apply.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Naval Template`, add a guarded setup in ClanSeeder.Templates.cs with its own exact template name, call CreateTemplateClan, and add a minimal rank/abbreviation/title graph through AddRank. Additional declarative templates instead use `AdditionalClanTemplateSpecifications`, with automatic enumeration and detection; prefer that smaller seam when its existing spec supports the desired graph. Main `LegacyTemplateClanNames` handles legacy guarded setups. Include actual calendar/timezone and intended privileges, then wire main SeedData and ShouldSeedData. Follow the local ClanSeeder AGENTS clone/template conventions; do not quietly reuse an existing name to rewrite a customized template. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Additive / InstallMissing`.

Designed as an additive package for installing more stock clan templates.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [ClanSeederPrivilegeTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ClanSeederPrivilegeTests.cs)
- [Clan_Elections_and_External_Control.md](../../../Design%20Documents/Economy/Clan_Elections_and_External_Control.md)
- [Clan_Seeder_Template_Catalogue.md](../../../Design%20Documents/Seeding/Clan_Seeder_Template_Catalogue.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
