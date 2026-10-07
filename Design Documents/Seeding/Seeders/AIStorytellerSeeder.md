# AIStorytellerSeeder

## Start here to add content

Edit [AIStorytellerSeeder.cs](../../../DatabaseSeeder/Seeders/AIStorytellerSeeder/AIStorytellerSeeder.cs): **Inline reference-document content and EnsureReferenceDocument invocation**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Adds one complete AI storyteller example package.
Baseline availability: **Enabled**. [AIStorytellerSeeder.cs](../../../DatabaseSeeder/Seeders/AIStorytellerSeeder/AIStorytellerSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `install`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- The Core seeder must have created at least one account.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: SeedData creates or loads AIStoryteller named Example Narrative Steward, sets model/prompts/progs/heartbeat flags and saves to AIStorytellers. It then calls EnsureReferenceDocument with AI Storyteller Primer and inline contents. Helper upserts AIStorytellerReferenceDocuments by name and assigns Description, FolderName, DocumentType, Keywords, DocumentContents and restrictions. Add a reference with a name constant, ShouldSeedData missing-document check, a SeedData call with all metadata/content and helper; same-name content is overwritten on rerun. Source content is inline C# prose. Transaction is explicit; package may be offered for extra/missing stock. No dedicated AIStoryteller test file located.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked reference `Example Local Primer`, add a constant and inline reviewed contents, an EnsureReferenceDocument call with full folder/type/keywords/restrictions metadata, and the missing-document readiness check. Verify AIStorytellerReferenceDocuments, then the storyteller access/reference integration. Same-name documents are rewritten; use a distinct identity for customization. This does not establish live model/provider availability. These instructions were source-checked but not executed.

Detailed runtime/builder contracts: [AI Storyteller Design](../../AI/AI_Storyteller_Design.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

This package is designed to be rerun safely.

Reruns reuse and update existing stock storyteller sample records.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
