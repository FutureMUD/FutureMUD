# Seeder test policy

This policy applies to Database Seeder, Item Seeder, optional setting installers, and their shared foundations and framework. Seeded content is editable game content. A content change should not require updating a unit test merely to repeat the new catalogue.

## Admission rule

Keep tests of helper functions, parsing, validation, dependency planning, orchestration, serialization, ownership, reconciliation, and persistence behaviour. Prefer small fixtures with deliberately supplied inputs and independently expected results.

Keep integration tests that execute installation, load or compile generated artifacts, exercise supported selections, reject missing prerequisites before writing, or demonstrate safe reruns. Observe the first installation and compare its identities or persisted state with the rerun. Require nonempty results where an empty installation could otherwise pass. Do not replace a catalogue total with a lower-bound quota.

Remove assertions that freeze stock names, roster sizes, era or family allocations, prose, balance numbers, nutrition, material properties, authored formulas, or exported catalogue hashes. Searching source or documentation for task-specific content is also insufficient evidence of functionality. Marking a snapshot `Integration` does not make it valuable.

Numbers and names in a synthetic fixture are legitimate expected results. For example, a parser must preserve a supplied quantity, and a rerun must preserve the builder name that the test deliberately set. These differ from repeating whatever today's stock catalogue happens to contain.

## Catalogue exceptions

A catalogue-dependent test needs a concrete failure mode and a reason that smaller functional coverage does not cover it. Current justified examples are:

| Surviving coverage | Failure prevented |
| --- | --- |
| Runtime name-culture regex captures, format elements and dice references | A newly edited definition can fail during character name generation. |
| Generated FutureProg compilation and execution | A serialized program can install successfully but fail to compile or enforce its supplied gates. |
| Native factories and configuration loaders, including trap payload validation | Producer and consumer schemas can diverge despite syntactically valid XML. |
| Clothing anatomy, colour binding, runtime wear-profile and outfit layering integration | A shared reference or body-location conflict can break many dependent garments or outfits. |
| Typed dependency closure and admitted outfit references | New content can refer to missing, excluded, or incompatible entities. |
| Compiled content assembly string-heap budget and dependency direction | Content growth can exceed a platform compilation limit or introduce a backwards installer dependency. |
| Independent typed/legacy recipe serializer comparison | A supported input, tool or product variant can serialize differently across the migration boundary. |

These exceptions permit checks of executable compatibility and relationships. They do not justify fixed inventory totals, duplicated catalogue values, editorial standards, or task-completion checklists inside the same test.

## Review and execution

Review mixed tests assertion by assertion. Preserve builder-edit protection, identity stability, collision rejection, interrupted-install recovery, and no-partial-write checks. Remove orphaned snapshot fixtures after their callers disappear. Keep shared fixtures used by other projects or partial classes.

Full installer matrices and exhaustive runtime compatibility checks use the existing `Integration` category and explicit extended-test selection. Fast helper and small persistence tests remain in the normal pass. Passing EF InMemory tests does not establish MySQL constraint behaviour or a successful live game installation.

The November 2026 cleanup removes catalogue snapshots across all three seeder test projects and narrows mixed installation tests to their functional assertions. It changes test coverage, not installed content or production behaviour. Historical content-acceptance inventories belong in their owning content documents or audit tooling, rather than fixed unit-test expectations.
