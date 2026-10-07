# CurrencySeeder

## Start here to add content

Edit [CurrencySeeder.cs](../../../DatabaseSeeder/Seeders/CurrencySeeder/CurrencySeeder.cs): **SeedDollars or the matching named currency package helper**. This is an authored C# seam unless the data-source section below identifies linked/embedded inputs or generated outputs. Do not edit emitted database rows or exported inventories as a substitute for the source definition.

## Purpose, operator choices and prerequisites

Set up a currency (or currencies) for your game.
Baseline availability: **Enabled**. The entrypoint is [CurrencySeeder.cs](../../../DatabaseSeeder/Seeders/CurrencySeeder/CurrencySeeder.cs); `SeederQuestions`, `SeedData` and `ShouldSeedData` define operator filtering/validation, execution and legacy readiness. No manual Program registration is needed.

Operator question IDs: `currency`. Exact accepted values, conditional visibility and defaults are defined in the linked `SeederQuestions`; the trace below explains the relevant choice path.

Metadata prerequisite descriptions from [SeederMetadataRegistry](../../../DatabaseSeeder/SeederMetadataRegistry.cs):

- The Core seeder must have created at least one account.

These predicates supplement concrete `ShouldSeedData` and helper preflights; declared ordering types are listed in the [dependency map](Dependency_Map.md).

## Inputs, representative trace and practical authoring guide

**Existing entry trace.** `SeederQuestions` exposes `currency` and explicitly tells the installer this is an additive choice. The accepted `dollars` value dispatches in `SeedData` to `SeedDollars(context, errors)` (`CurrencySeeder.cs:54-83`). In that helper (line 179), it creates `Currency { Name="Dollars" }`, adds it to `context.Currencies`, and saves immediately to obtain its generated ID. It ensures helper FutureProg `IsLessThanOneHundred`, with parameter `number` of numeric type, then creates currency divisions: “cent” conversion rate 1 and “dollar” rate 100, both with abbreviation regexes and persistence through `context.CurrencyDivisions.Add`. It then creates multiple currency-description patterns/elements and coins. A real coin example (“penny”) initializes name, short/full descriptions, value `1.0M`, parent `Currency`, weight, `GeneralForm="coin"`, plural word; `context.Coins.Add(coin)` and `SaveChanges` follow (the later “nickel”, “dime”, “quarter”, and notes repeat this shape). Parent navigation properties supply currency/denomination relationships; there are separate description pattern and element tables.

**Add-one recipe: add a further coin to Dollars.** In `SeedDollars`, append a `Coin` initializer beside the existing penny/nickel/dime/quarter entries with a unique `Name`, noun-matching short/full description, `Value` expressed in the currency's base unit (the code's penny = 1, nickel = 5, dime = 10, quarter = 25), `Currency=currency`, `Weight`, `GeneralForm` (`coin` or `note`), and `PluralWord`; add with `context.Coins.Add` and follow the same `SaveChanges` cadence. Then check any currency description patterns lower in the same helper: if the new unit should be printable/parsible using that pattern, add the correct `CurrencyDescriptionPatternElement` entries and division reference there too; merely adding a coin does not create a new denomination or parser syntax. To add a full new *currency package* instead, add a new canonical answer to the validation switch, a `SeedData` switch arm and named helper following the 7 existing package helpers. The switch currently accepts `soi` etc.; adding a helper without both registration points makes it unreachable. Neither addition uses an external asset or installer registration list.

**Reruns/failure.** `SafeToRunMoreThanOnce=true` and registry metadata says `Additive / InstallMissing`, but the legacy `SeedData` path shown here seeds selected package content directly rather than a managed reconciler. Avoid promising that rerunning the same currency answer is duplicate-safe unless test/source confirms that specific package; its stated purpose is installing other stock currencies. It starts/commits a transaction but does not use `using`/catch rollback. It accumulates `errors` and commits before returning errors/warnings, so return text can report warnings after committed partial work. Core account prerequisite; `ShouldSeedData` returns `ExtraPackagesAvailable` if any currency exists. Source tests [CurrencySeederTests.cs](../../../DatabaseSeeder%20Unit%20Tests/CurrencySeederTests.cs) must be consulted if documenting exact repeat run expectations; no tests executed here.

The Dollars route includes one `IsLessThanOneHundred` prog that is inserted only if absent, but a full currency package includes description patterns in addition to coins. Existing real name “dollar” is both a division with conversion rate 100 and a note/coin unit; adding another division must use the package's established base conversion scale and abbreviation regex conventions. Description pattern code wires parsed elements to `CurrencyDivision` and special values (e.g. `cent`), so the `CurrencyDivision` and `CurrencyDescriptionPatternElement` are different persistence concerns. If the requested change is only adding a new coin, a generated coin ID is not used as the parser's denomination; it is a coin inventory prototype and has its own `Coin` row. Every `context.SaveChanges` in this helper flushes partial work, but all occur inside the enclosing database transaction, so a fatal exception semantics differ from a normal `errors` collection result. Do not recommend adding a save solely to get a coin ID without confirming references need it.

**Worked new currency package: Example Tokens.** `SeedBits` (line 1675) is the smallest adjacent one-denomination package to follow. Add `tokens` to the `currency` prompt/validator, add `case "tokens": SeedExampleTokens(context, errors); break;` in `SeedData`, and implement that helper using the complete `SeedBits` pattern. Set currency name `Example Tokens`, division name `token`, conversion rate `1.0M`, and an abbreviation regex accepting the chosen singular/plural tokens. Adapt every `CurrencyDescriptionPatternType` branch and element text/pluralization to `token`; retain the negative-prefix and rounding contracts. Add the corresponding `Coin` with intended descriptions, weight, `GeneralForm`, plural word and base-unit value, attached to this new currency. The helper must create currency, division/abbreviations, all supported description-pattern graphs and coins, not only rename one Coin. No global numeric currency ID is authored. Check `tokens` validation/dispatch, parser and formatting examples for positive/negative/fractional values, coin value/weight, and same-package rerun behavior with focused tests. The base helper inserts a new currency directly; copying it does not make the new package duplicate-safe. Do not present this illustrative recipe as historical currency research.

## Declared rerun and ownership contract

Metadata declares `Additive / InstallMissing`.

Designed as an additive package for installing more stock currencies.

Those are metadata declarations. The source trace above identifies actual lookup/field/transaction behavior; matching names alone do not prove field ownership or preservation. [The shared executor](README.md#shared-execution-contract) handles exceptions and answer persistence but supplies no cross-seeder transaction.

## Verification and related references

This guide was checked by static source reading; its addition recipe was not executed. No build, database or runtime result is claimed. For a future content change, add focused source/loader/serialization or selected-path first-install/rerun regression checks as appropriate, following the [verification map](../../../.codex/references/verification-and-docs.md). A source invariant is not proof of installed gameplay behavior.

See [repeatability strategy](../DatabaseSeeder_Repeatability_Strategy.md), [coverage and reviewed evidence](Coverage_and_Evidence.md), and the relevant linked test/source references above.
