# CombatSeeder

## Start here to add content

Edit [CombatSeeder.RangedWeapons.cs](../../../DatabaseSeeder/Seeders/CombatSeeder/CombatSeeder.RangedWeapons.cs): **SeedDataGuns / SeedDataMuskets for firearms; corresponding melee/armour/check/style partials own other content**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Attacks, Echoes, Weapon Types and all the fun stuff.
Baseline availability: **Enabled**. [CombatSeeder.cs](../../../DatabaseSeeder/Seeders/CombatSeeder/CombatSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `installmuskets`, `installguns`, `random`, `parryoption`, `skilloption`, `messagestyle`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- Attributes must already be seeded for combat formulas.
- Shared skill infrastructure (Skill Check, General Skill, Veterancy Skill, Skill Improver, AlwaysTrue and AlwaysFalse) must already exist.
- The Human seeder must have installed the Human race.
- UsefulSeeder crossbow spanning-tool tags must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: CombatSeeder.RangedWeapons.SeedDataGuns constructs RangedWeaponTypes named 9mm Pistol (Lethal, Pistols Fire/Operate traits, range 2, ammo Magazine/9x19mm Parabellum, capacity 1, expressions, firearm type, stamina, delays and hand flags), adds to context.RangedWeaponTypes and saves. The same method then adds the Pistol_9mm GameItemComponentProto. SeedData dispatches SeedDataRanged and conditionally SeedDataGuns/Muskets from answers. Add a firearm by adding a parallel complete block with distinct ranged type name, skill refs, ammo/load values, expressions/delays and matching serialized component definition; wire its era/question route if not already dispatched. Explicit transaction; existing combat foundation uses refresh helper on rerun.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Existing-world integration matters.** `SeedDataGuns` is a first-install path, or runs when `HasModernFirearms` is false. Adding a new gun block there does not install it on worlds already recognized as having modern firearms. A worked additional pistol must also extend the supported `EnsureModernFirearmSamples` reconciliation path and its dependency/readiness checks; follow the complete ranged type plus component pair, not only its display name. Verify both a fresh gun-enabled fixture and an existing modern-firearms fixture.

## Declared rerun and ownership contract

Metadata declares `Idempotent / FullReconcile`.

Reruns reconcile installed combat modules by their stable stock identities without removing builder content.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [CombatBalanceProfileHelperTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CombatBalanceProfileHelperTests.cs)
- [CombatSeederSourceTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CombatSeederSourceTests.cs)
- [CombatStrategySeederCompatibilityTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CombatStrategySeederCompatibilityTests.cs)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
