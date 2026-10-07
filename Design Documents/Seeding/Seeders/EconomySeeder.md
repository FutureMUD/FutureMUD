# EconomySeeder

## Start here to add content

Edit [EconomySeeder.cs](../../../DatabaseSeeder/Seeders/EconomySeeder/EconomySeeder.cs): **EnsureMarketCategories and FamilyElasticityMap for category rules; the Useful market-tag catalogue owns tag-derived category names**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs a stock economic zone shell, market categories, influences, populations and shoppers.
Baseline availability: **Enabled**. [EconomySeeder.cs](../../../DatabaseSeeder/Seeders/EconomySeeder/EconomySeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `era`, `currency`, `zone`, `shopper-scale`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.
- The Currency seeder must have installed at least one currency.
- The Time seeder must have installed at least one clock and calendar.
- At least one physical zone must exist.
- UsefulSeeder market tags must already exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: each descendant market tag is treated as a named category in EnsureMarketCategories. The existing Useful tag path `Market > Nourishment > Staple Food` yields the leaf `MarketCategory` named `Staple Food`. EnsureNamedEntity(context.MarketCategories, tag.Name, ...) upserts; fields set description with Economy Category prefix, family elasticity, type 0, XML Tags referencing tag.Id and empty Components for the initial leaf/category pass; a later pass turns selected family categories into Type 1 combinations with component weights, so these are not all final category fields. It saves. Add a market category through the market tag hierarchy and, when needed, update FamilyElasticityMap or StockCombinationCategoryWeights; it is not a direct static MarketCategory list. SeedData also ensures economic zone/market/populations and influence templates using era/currency/zone answers. Explicit transaction and name-based reconciliation. Follow the three linked Economy design documents below.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked category `Example Preserved Food`, add the intended child under the Useful market-tag hierarchy, then decide whether existing family elasticity/category combination rules are sufficient or require a reviewed mapping. On the selected era route inspect the resolved descendant tag, MarketCategory tag XML and later market/population references. Do not append a standalone MarketCategory initializer: this path derives category identity from tags. These instructions were source-checked but not executed.

## Declared rerun and ownership contract

Metadata declares `Additive / RepairExisting`.

Reruns install missing stock economy packages for other eras and can restore missing stock-owned market categories, influence templates, populations, shoppers, and helper progs.

Rerunning the same era refreshes the seeded template market, populations, shopper definitions, and stress helper progs without creating duplicates.

Stock economy content is tracked by stable era-specific names plus a shared EconomySeeder prefix for helper records.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [EconomySeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/EconomySeederTests.cs)
- [Economy_System_Runtime.md](../../../Design%20Documents/Economy/Economy_System_Runtime.md)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
