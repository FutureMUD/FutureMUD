# TrapSeeder

## Start here to add content

Edit [TrapSeeder.cs](../../../DatabaseSeeder/Seeders/TrapSeeder/TrapSeeder.cs): **EnsureTemplates and Definition / Trigger / Component / Payload**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Adds trap checks, component tags, a Traps skill, and safe example templates.
Baseline availability: **Enabled**. [TrapSeeder.cs](../../../DatabaseSeeder/Seeders/TrapSeeder/TrapSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Skill templates, trait decorators, improvers, and stock FutureProgs must already exist.
- Core liquid and gas catalogues must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry Tripwire Alarm is passed to EnsureTemplate with Mechanical/Safe, ExitTraversal trigger, Trigger and Signal Trap Payload components (85/95 recovery), EmitSignal value 1. EnsureTemplate prefixes StockPrefix, looks up full name, creates TrapTemplate with max+1 ID and EditableItem or refreshes Definition/status, then adds to `context.TrapTemplates` only for a missing row; caller saves. Add via EnsureTemplate using source helpers Definition/Trigger/Component/Payload and available tags. Any matching StockPrefix name yields `MayAlreadyBeInstalled` even if other entries are absent; this coarse marker does not prove catalogue completeness. Explicit transaction with catch/rollback; keyed templates overwrite stock definition.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Tripwire Alarm`, add an EnsureTemplate call beside Tripwire Alarm with a distinct stock name and compatible Mechanical/Safe, ExitTraversal, trigger/signal component and EmitSignal payload shape. Review recovery chances and required component tags rather than inventing serialized type strings. Check approved editable/template identity and Definition XML; same-name stock templates are rewritten. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reconcile the dedicated Traps skill, seven trap checks, and Stock Trap templates without overwriting builder-owned templates.

Stock trap definitions use the Stock Trap prefix; other templates are never modified.

The package owns only the Traps skill/check mappings and Stock Trap prefixed templates.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [TrapSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/TrapSeederTests.cs)
- [Trap_System.md](../../../Design%20Documents/Core/Trap_System.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
