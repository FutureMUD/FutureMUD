# ArmageddonMagicSeeder

## Start here to add content

Edit [ArmageddonReviewedUtilityContent.cs](../../../FutureMUDLibrary/Magic/Stock/ArmageddonReviewedUtilityContent.cs): **Reviewed pure content factory; ArmageddonMagicInstallPlan and installer bind and persist that contribution**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

The concrete `ArmageddonMagicSeeder` package is implemented in the entrypoint below.
Baseline availability: **Debug only; disabled in Release**. [ArmageddonMagicSeeder.cs](../../../DatabaseSeeder/Seeders/ArmageddonMagicSeeder/ArmageddonMagicSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

In Debug, [ArmageddonMagicSeeder.Questions.cs](../../../DatabaseSeeder/Seeders/ArmageddonMagicSeeder/ArmageddonMagicSeeder.Questions.cs) exposes `install-armageddon-partial` and `armageddon-prepared-bindings`; Release exposes none and remains disabled. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- No additional metadata prerequisite; concrete readiness/preflight still applies.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry Mend Flesh is ArmageddonReviewedUtilityContent.MendFlesh() in FutureMUDLibrary/Magic/Stock/ArmageddonReviewedUtilityContent.cs, key arm.spell.mend_flesh, Name, prose, duration 0, minimum energy 20, emote, and builder XML for character target/heal-worst-first/heal-overflow/amount 2*grade*grade. ArmageddonUtilitySpellContent holds key/name/description/duration/minimum/emote/definition factory. DEBUG builds set Enabled=true; Release sets false. ArmageddonMagicInstaller.Content dispatches reviewed spells to Contributions and WriteContent; inserts MagicSpells, TraitExpressions, optional FutureProg and SeederManagedRecords; Apply reconciles ownership, then writes blank device component/item rows. Add a reviewed spell by adding shared pure content definition, including dependencies and explicit external plan binding, and add it to Content(plan), then tests/docs. Mend Flesh uses an externally selected `MendEligibilityProg`; it does not generate that eligibility prog. The module installer uses a serializable relational transaction and distinguishes precommit rollback, unknown commit outcome and committed-but-confirmation-failed status. This does not promise atomicity across the full prepared-world orchestration. Exact field additions belong in shared content and install plan. Installer requires prepared world, explicit bindings and clean context; module installer uses ownership-aware reruns. Source does not create player enrollment/charge inventory.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

The reviewed-content JSON [manifest](../../../DatabaseSeeder/Assets/Manifests/Armageddon_Reviewed_Content.json) and [sorcerer source tree](../../Magic/Armageddon_Sorcerer_Source_Tree.json) are embedded support inputs. They are distinct from shared pure C# spell definitions and module install-plan bindings; do not promote draft source-tree entries merely by copying a name. Inspect the selected plan preflight and ownership diagnostics before attempting a prepared-world run.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Default No. Explicit opt-in reconciles owned partial modules with separate commits, preserving builder edits and historical ownership. A later failure leaves completed modules committed.

Prepared-world bindings are validated only on opt-in. Availability is reported from persisted capability admissions; this is not a full preset or player refresh.

Stable module keys own reviewed definitions only. Optional provisions require explicit existing native selections; unselected owned provisions are preserved. No stock scrolls, charges, classes, acquisition or reserve refill.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [ArmageddonPierceInstallerTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ArmageddonPierceInstallerTests.cs)
- [ArmageddonPreparedWorldSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ArmageddonPreparedWorldSeederTests.cs)
- [ArmageddonReleaseGateTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ArmageddonReleaseGateTests.cs)
- [Armageddon_Magic_Psionics_Gap_Report.md](../../../Design%20Documents/Magic/Armageddon_Magic_Psionics_Gap_Report.md)
- [README.md](../../../Design%20Documents/README.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
