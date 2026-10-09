# LawSeeder

## Start here to add content

Edit [LawSeeder.cs](../../../DatabaseSeeder/Seeders/LawSeeder/LawSeeder.cs): **SetupClasses switch for a legal class; SetupLaws and enforcement helpers for the corresponding families**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Sets up Legal Enforcement, Laws, and some related AI..
Baseline availability: **Enabled**. [LawSeeder.cs](../../../DatabaseSeeder/Seeders/LawSeeder/LawSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `name`, `currency`, `createai`, `separatepowers`, `punishmentlevel`, `classes`, `religiouslaws`, `penaltyunits`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- The Currency seeder must have installed at least one currency.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: SeedData resolves the user-selected AuthorityName, creates/upserts LegalAuthority, then SetupClasses receives keys. For "immune", SetupClasses creates an authority-scoped FutureProg named IsLegalClass{collapsedAuthority}Immune and a LegalClass named Immune bound to that authority/prog; EnsureLegalClass writes context.LegalClasses by authority/name. Add a class by adding a normalized key to the SetupClasses input and a switch case that defines prog text/parameters plus class properties; adding only a key does nothing. Many other package helpers add WitnessProfiles, EnforcementAuthorities and AI. Questions choose authority/world context; explicit transaction and multiple saves.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked class key `exampleexempt`, add it to the selected class list and implement its SetupClasses case beside immune. Supply a real authority-scoped predicate prog, signature and LegalClass properties using the existing helper. The `classes` validator rejects unknown normalized keys, so add the key there and to displayed help; extend enforcement/law policy consumers where the class must participate. Verify its authority/name identity and associated prog; giving a new key the immune predicate is an illustrative starting point, not an approved justice policy. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Stock tiered-law applicability separates class membership selection from punishment rank. `Felon` and `Criminal` retain selection priorities above `Citizen`, but the generated offender/victim comparison maps their authority-scoped class IDs to ranks 20 and 19, below `Slave` and above `Pet`/`Other`. Other classes use their configured priorities. This prevents criminal status selecting the weaker "Against Inferior" variant against citizens or non-citizens, while preserving classification and the intended privileged-class variants. The generated progs remain builder-editable; `LegalClassOutranks` itself still compares membership priority and is not used for this stock comparison.

Existing worlds retain stored applicability progs until their owners update them. Reinstalling the same stock authority through the existing `RepairExisting` seeder refreshes these progs and other managed stock fields; owners preserving customised laws should edit the affected applicability progs instead. A server restart alone does not reconcile seeded law content.

Metadata declares `Idempotent / RepairExisting`.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [Law_System_Runtime.md](../../../Design%20Documents/Economy/Law_System_Runtime.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
