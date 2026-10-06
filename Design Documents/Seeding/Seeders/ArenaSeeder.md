# ArenaSeeder

## Start here to add content

Edit [ArenaSeeder.cs](../../../DatabaseSeeder/Seeders/ArenaSeeder/ArenaSeeder.cs): **SeedData EnsureEventType calls, matching sides and allowed-class mappings**. Keep source identity and dependent references together. The worked entry below is the smallest existing path to follow before adding a new definition.

## Purpose, operator choices and prerequisites

Installs an example combat arena with gladiator, boxing, wrestling, champion, and animal formats..
Baseline availability: **Enabled**. [ArenaSeeder.cs](../../../DatabaseSeeder/Seeders/ArenaSeeder/ArenaSeeder.cs) owns `SeederQuestions`, `SeedData` and `ShouldSeedData`. Reflection discovery needs no manual registration.

Operator question IDs: `arena-name`, `arena-zone`. Read the linked `SeederQuestions` filters and validators for accepted values, defaults and conditional visibility; required database bindings cannot be replaced with guessed numeric IDs.

Metadata prerequisite descriptions:

- At least one economic zone must exist.

These are checks, not an installation guarantee. The [dependency map](Dependency_Map.md) distinguishes hard ordering edges from soft ordering hints.

## Inputs, representative trace and practical authoring guide

Named entry: SeedData resolves answer-selected economic zone and arena name (default Grand Coliseum); creates prog rows, arena row and stock classes; calls EnsureEventType for Duel with BYO=true, 600 registration/120 prep/900 time limit, FixedOdds and NoElimination; then builds two sides and allowed classes. EnsureEventType uses keyed context.ArenaEventTypes upsert by ArenaId+Name and writes timing/betting/elimination/prog/fee/Elo properties. Add a format with EnsureEventType call then EnsureEventTypeSide calls and ReplaceAllowedClasses mappings; an event type alone is incomplete. Existing named event fields are refreshed. Transaction catches exceptions and rolls back.

All named fields and calls above are authored source. The addition steps are static instructions; this documentation task did not add content or execute them. Preserve existing identities, supply all constructor/helper arguments from the adjacent entry, and trace every referenced body, material, trait, tag, component, prog or model to its owning prerequisite. A new unsupported runtime type requires its implementation and registration as well as a source row.

**Worked addition.** For a worked format `Example Duel`, add an EnsureEventType call next to Duel with deliberately chosen time limits, betting/elimination and progs, then create its two sides and allowed-class mappings, and add its exact name to `StockArenaEventTypes` readiness detection. Preserve the selected arena/economic-zone bindings. Verify the event graph rather than only ArenaEventTypes, and follow the arena design for physical cells and operational provisioning not performed by this template installer. These instructions were source-checked but not executed.

Detailed runtime/builder contracts: [Combat Arenas Design](../../Combat/Combat_Arenas_Design.md).

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the same named arena package and refresh stock-owned combatant classes, event types, event sides, and helper progs.

Live arena configuration such as room links, finances, schedules, ratings, and events is preserved.

The trace above describes actual identity, write and transaction behavior. Metadata is a declared contract, not proof of field-level preservation. The [shared executor](README.md#shared-execution-contract) records answers after `SeedData`; it cannot undo commits or earlier saves that have already completed.

## Verification and related references

The example was checked against source at the pinned baseline. No build, database or gameplay result is claimed. For a future addition, verify first install, selected-path admission, rerun and builder customization boundaries using the owning focused suite; inspect failure and partial-install behavior rather than assuming the success message proves completeness. Follow the [verification map](../../../.codex/references/verification-and-docs.md).

Related source tests and design references:

- [ArenaSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/ArenaSeederTests.cs)

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md) and [coverage and reviewed evidence](Coverage_and_Evidence.md).
