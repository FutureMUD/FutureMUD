# RobotSeeder

## Start here to add content

Edit [RobotSeeder.Definitions.cs](../../../DatabaseSeeder/Seeders/RobotSeeder/RobotSeeder.Definitions.cs): **Templates; Bodies and Procedures partials own additional anatomy and operations**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs robotic races, chassis bodies, and robot maintenance content.
Baseline availability: **Enabled**. [RobotSeeder.cs](../../../DatabaseSeeder/Seeders/RobotSeeder/RobotSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: none in the concrete question sequence. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- Humanoid and animal body frameworks must already be installed.
- Human race foundations must already exist.
- Shared humanoid characteristic profiles must already exist.
- Core robot progs, corpse models, tool tags, and prerequisite attacks must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: RobotSeeder.Definitions.Templates has Robot Humanoid with BodyKey `Robot Humanoid`, role prose, ParentRaceName `Humanoid`, not playable, can use weapons, humanoid characteristics, hydraulic fluid, Robot Articulated Model, Normal size, CanClimb=true and CanSwim=false, description variants, Jab/Cross/Hook/Elbow/Bite/Snap Kick bodypart aliases and strategy Melee (Auto). SeedData calls SeedRaces; it resolves BodyKey and finds Race by Name. Missing rows are constructed and added to _context.Races, then saved; all templates then apply nonbreather settings, attributes, additional characteristics/ethnicity, descriptions, bodypart usages and natural attacks. The named parent is resolved from context; an absent body key can skip that template rather than throw. Existing races get selective refresh of strategy/armour/health, while many other fields remain. Add a RobotRaceTemplate conforming to the record signature in `Definitions.cs`; new body requires Bodies catalogue, new natural attack requires its shared attack source, and aliases must exist on body. Transaction wraps install.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked `Example Maintenance Humanoid`, start with Robot Humanoid in Templates and keep its actual Robot Humanoid body key and Humanoid parent binding unless supplying a different supported chassis. Author role prose, fluids/health/material/combat references and description variants. Inspect both the missing-race creation and existing-race selective update path. Additional chassis or maintenance procedures belong in Bodies/Procedures with matching references. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Idempotent / InstallMissing`.

Reruns install missing stock robot races, bodies, and procedures without duplicating existing entries.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [NonHumanCombatBalanceReconciliationTests.cs](../../../DatabaseSeeder%20Unit%20Tests/NonHumanCombatBalanceReconciliationTests.cs)
- [RobotSeederTemplateTests.cs](../../../DatabaseSeeder%20Unit%20Tests/RobotSeederTemplateTests.cs)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
