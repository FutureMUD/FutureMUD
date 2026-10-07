# TimeSeeder

## Start here to add content

Edit [TimeSeeder.Calendars.cs](../../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Calendars.cs): **SetupGregorian or the owning calendar-family builder**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Sets up Calendars and Clocks.
Baseline availability: **Enabled**. The entrypoint is [TimeSeeder.cs](../../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `secondsmultiplier`, `mode`, `startyear`, `ardaage`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Installer route and stored shape.** The `mode` prompt lists `gregorian-us` and others; validator accepts it. `SeedData` starts a DB transaction, ensures an UTC `Clock` definition using `secondsmultiplier`, then ensures UTC timezone. `EnsureClock`, `EnsureTimezone`, and `EnsureCalendar` are helpers in `TimeSeeder.cs:360-424`; `EnsureCalendar` derives alias from definition XML and reuses by canonical alias, otherwise creates a `Calendar` and adds to `context.Calendars`. For `mode=gregorian-us`, switch at line 157 calls `SetupGregorian(context, true, false, clock, answers)`. `SetupGregorian` is in `TimeSeeder.Calendars.cs:3430+`: builds a Gregorian calendar with date `january/01/{startyear}`, alias `gregorian`, US `MM/DD/YYYY` short format, American date prose, Gregorian leap-year rules (divisibility by 4 with the 100/400 century exceptions), month/week definitions, and saves the complete XML in `Calendar.Definition`; then passes calendar through `EnsureCalendar`. Thus definition XML, not a model class per month, carries calendar structure. Main method resolves primary alias, binds it to zones/shards with `SyncShardAndZoneTimeBindings`, saves and commits.

**Add-one recipe: add a new Gregorian display variant only if it is a separately selected stock mode.** For a new mode value `gregorian-custom`, add a prompt row in `SeederQuestions`, validator acceptance, `ResolvePrimaryCalendarAlias`, and `SeedData` switch case, then call `SetupGregorian` with the appropriate `useImperial/useCE` flags (or factor a distinct builder when semantics differ). Since the current helper hardcodes XML calendar alias `gregorian`, adding a second variant through that helper will resolve to/reuse the existing Gregorian alias rather than create a separately named calendar; change helper to accept alias or write a new definition builder if this is truly a second calendar. Also update `StockCalendarAliases` only if package detection's stock list requires the alias, and test alias uniqueness and matching calendar start-date semantics. To add an ordinary calendar type, follow its dedicated `Setup...` method in [TimeSeeder.Calendars.cs](../../../DatabaseSeeder/Seeders/TimeSeeder/TimeSeeder.Calendars.cs): create XML using `BuildCalendarDefinition` plus `MonthSpec`/algorithm/day-boundary helpers, then call `EnsureCalendar` with a valid date using one of those month aliases; register the `mode` value in prompt validator, alias mapping and switch. That three-point registration is required; adding XML alone is unreachable.

**Persistence/reconciliation.** Idempotent / repair-existing metadata says clocks/calendars/timezones/shard-zone bindings are reused/repaired by canonical identities, with stable stock names and aliases and no deletion of prior setups. `EnsureTimezone` reuses/upserts named timezone, `EnsureCalendar` alias lookup; inspect exact helper fields before altering semantics. Account prerequisite. Transaction has explicit Begin/Commit, but no catch/explicit rollback in shown entrypoint; do not claim code-level rollback handler. [TimeSeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/TimeSeederTests.cs) is the focused source test suite; no test run.

For a new single calendar family, `BuildCalendarDefinition` in the partial accepts alias, display labels/description, three formatting masks, era text, epoch/year/weekday values, weekday and `MonthSpec` sequences, algorithm XML, and day-boundary XML. It serializes child entries (`BuildMonthElement` writes alias, short/full names, nominal order, normal days and empty intercalary/special/nonweekday containers). A new family can reuse that builder when its calendar is expressible with those fields; leap, lunisolar, or special-day rules need the appropriate XML structure/algorithm and likely a dedicated `Setup...` as the existing special calendars demonstrate. After builder creation, caller must derive a syntactically valid current `Date` matching month aliases. `EnsureCalendar`'s canonical alias behavior means two package labels with same alias intentionally resolve to same stock row. `StockCalendarAliases` feeds `ShouldSeedData`: its source comments/readiness check distinguish seeded aliases from just-any-calendar custom worlds. Updates should keep stock alias stable, or migration/detection will treat a stock calendar as absent. The operator sees one positive integer for `startyear` only where a setup's prompt requests it; don't add a question key unless it is validated and passed through all setup methods.

For an alias-matched calendar, `EnsureCalendar` assigns `Date`, `Definition` and `FeedClockId` directly. A revised authored definition can replace existing calendar state on rerun; alias identity alone does not protect builder edits. Verify retained time/date intent before using the repair path.

## Declared rerun and ownership contract

Metadata declares `Idempotent / RepairExisting`.

Reruns reuse the stock time package by canonical clock, timezone, and calendar identities.

Reruns repair or complete stock clocks, calendars, timezones, and shard/zone bindings without deleting older setups.

Seeder-owned time records are tracked by stable names and aliases.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
