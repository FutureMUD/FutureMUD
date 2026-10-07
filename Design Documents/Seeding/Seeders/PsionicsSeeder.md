# PsionicsSeeder

## Start here to add content

Edit [PsionicStockContent.cs](../../../FutureMUDLibrary/Magic/PsionicStockContent.cs): **PsionicStockPower catalogue, supported definition type and matching PsionicPlayerContent help**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Optional Basic and Advanced Psionics, with unassigned capabilities..
Baseline availability: **Enabled**. [PsionicsSeeder.cs](../../../DatabaseSeeder/Seeders/PsionicsSeeder/PsionicsSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `install-psionics`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- A skill scaffold must already exist.
- The organic Human race must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry stock connectmind/contact is PsionicStockPower("connectmind","contact",0,true,2,300,legacy help) in FutureMUDLibrary/Magic/PsionicStockContent.cs. Name/help derive from PsionicPlayerContent. SeedData iterates stock powers for Basic/Advanced, creates MagicPower with generated XML and adds context.MagicPowers; then links powers/bands into MagicCapabilities.Definition. Add a new power requires canonical stock entry, player-facing help entry, a supported Type branch in PsionicStockContent.Definition/runtime and appropriate installer support; do not simply add unknown Type. Existing independently edited strings are conditionally preserved, while known placeholders refresh. Prereqs Human and a trait; readiness checks only Basic school, potentially hiding partial Advanced stock. Explicit transaction.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Existing capabilities are a separate path.** Adding a canonical stock power may create its `MagicPower` row, but the existing-capability update currently admits `connectback` and the combat-power catalogue explicitly. A worked new supported power must extend that reconciliation/admission path as well as the new-capability constructor, or existing capabilities will not gain the link. Verify both power identity/definition and the resulting capability XML on first install and rerun.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Installs missing psionics, updates recognised stock names and player help, preserves builder edits and reports conflicting identities.

Optional schools and unassigned capabilities only; never grants character access.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [PsionicCombatSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/PsionicCombatSeederTests.cs)
- [PsionicPowerSeederSourceTests.cs](../../../DatabaseSeeder%20Unit%20Tests/PsionicPowerSeederSourceTests.cs)
- [PsionicsSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/PsionicsSeederTests.cs)
- [Armageddon_Magic_Psionics_Gap_Report.md](../../../Design%20Documents/Magic/Armageddon_Magic_Psionics_Gap_Report.md)
- [Psionics_Seeder.md](../../../Design%20Documents/Seeding/Psionics_Seeder.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
