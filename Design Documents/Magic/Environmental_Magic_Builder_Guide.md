# Environmental Magic Builder Guide

An `environmental` regenerator is a reusable profile for resources held by physical cells. It uses the existing magic-resource balances, one environmental maximum for each configured resource, and persistent cell damage. It applies only after an explicit cell binding or terrain default is configured. No stock setting content is installed.

The world coordinator advances useful production and optional natural scar repair. See [Environmental Magic Runtime](Environmental_Magic_Runtime.md) for scheduling, persistence and integration contracts, and the [acceptance mapping](Environmental_Magic_Acceptance.md) for regression coverage and execution boundaries. [Capability-configured Self and Gentle gathering](Magic_Gathering.md) is a separate consumer of the exact managed mana-debit surface. [Native organic yields](Native_Organic_Yields_and_Ecological_Penalties.md) add explicit source authorisation, staff inspection, native owner debits and scar-sensitive positive production for the later Land method. Task 3A still adds no player Land access, payout, channelling or rejuvenation spell.

## Author a profile

Use an existing resource that supports locations. The actual creation order places the resource before the profile name:

```text
magic regenerator edit new environmental <resource> <name>
magic regenerator list
magic regenerator show <profile>
magic regenerator edit <profile>
magic regenerator clone <profile> <new name>
magic regenerator set name <new name>
magic regenerator close
```

A new profile has one output, base capacity 100, base rate 1 per real minute, a recent-pressure half-life of 3,600 real seconds, and zero natural scar repair. The default maximum and rate expressions are `basecapacity` and `baserate`.

While editing a profile, use:

```text
magic regenerator set output add <resource>
magic regenerator set output remove <resource>
magic regenerator set output <resource> maximum <formula>
magic regenerator set output <resource> rate <formula>
magic regenerator set output <resource> basecapacity <number>
magic regenerator set output <resource> baserate <number>
magic regenerator set input <name> forage <yield key> <scale>
magic regenerator set input <name> agriculture <source> <scale>
magic regenerator set input <name> prog <prog> <scale>
magic regenerator set input remove <name>
magic regenerator set halflife <seconds>
magic regenerator set repair <amount per real minute>
magic regenerator set idle <seconds|default>
```

Each resource occurs at most once. Half-life must be at least one second; an optional idle interval is from one through 3,600 seconds. Repair zero disables natural repair. Inputs and outputs have finite count limits enforced by the profile. A missing reference, invalid expression, unsupported resource holder, duplicate output or invalid numeric result disables the affected environmental route with diagnostics.

Changing a profile retains existing room balances and damage. A valid lower maximum discards excess energy at the owning managed mutation/configuration/advance boundary. A higher maximum increases capacity only. Removing an output or disabling a profile does not delete stored balances or damage. A newly attached output starts empty; existing stored zero balances remain empty on restart.

## Formulas and input units

The engine collects one input snapshot for the needed bindings, evaluates each maximum, and then evaluates its rate. Maximum expressions cannot use `balance` or `maximum`. Rate expressions may use both. The same environmental maximum is used for cap inspection, ordinary additions, FutureProg resource changes, scheduler production and managed debits.

| Input | Meaning and units |
| --- | --- |
| `basecapacity` | This output's configured resource capacity |
| `baserate` | This output's configured resource production per real minute |
| `scardamage` | The cell's persistent remaining scar damage |
| `pressure` | Current exponentially decayed, severity-weighted recent pressure |
| `hasdefile` | 1 when a destructive-use timestamp exists, otherwise 0 |
| `minutessincedefile` | Real minutes since the last destructive use; use `hasdefile` to distinguish absence |
| `balance` | Current recorded resource balance; rate expressions only |
| `maximum` | The just-computed maximum; rate expressions only |

Named inputs have an explicit finite scale. The binding multiplies its source value by that scale; it never automatically adds unlike units. Input names are case-insensitive. Formulae must yield finite, non-negative maxima and rates. A deliberate zero is valid; a configuration/calculation error is not treated as zero capacity and must not destroy stored resources. Random expression functions are rejected.

Agriculture sources are `hasfield`, `hascrop`, `haswoodland`, `crophealth`, `cropyieldpotential`, `woodlandhealth`, `woodlandyieldpotential`, `pasture`, and `fieldcondition`. Presence values are 0 or 1; the health/yield/pasture/condition scores are 0-100. Absent field/crop/woodland values contribute zero. Bind a presence input when a missing field must differ from a present field with zero health.

Forage mana inputs read the named yield in its native yield-point units. No effective forage profile contributes neutral zero. An existing profile without the configured key is a configuration error. Pure reads project the current profile's cap/new-key rules without modifying stored forage pools, recovering forage, changing yield heartbeats or saving. A separately declared organic source may be observed/planned and debited through its native owner, and a separately configured penalty may suppress its positive native recovery; merely reading a mana input grants neither behavior.

## Organic sources and ecological penalties

Organic conversion is opt-in per canonical source. Use:

```text
magic regenerator set organic sources
magic regenerator set organic source add forage <yield-key>
magic regenerator set organic source add crop|woodland|pasture
magic regenerator set organic source <selector> uses <uses|any>
magic regenerator set organic source <selector> definitions <IDs|any>
magic regenerator set organic source remove <selector>
magic regenerator set organic penalty <channel> <formula|none>
magic regenerator set organic protection <prog|none>
```

Canonical selectors are `forage:<normalised-key>`, `crop`, `woodland` and `pasture`. Crop covers both an annual crop and an orchard in the shared native crop slot. A profile permits at most 32 declarations and rejects duplicate canonical selectors. Empty definition/use restrictions mean any compatible current definition/use for that declared source; they do not enable undeclared channels. Where a field and wild forage represent the same plants, declare only the intended native owner.

Penalty tokens are `forage`, `crophealth`, `cropyield`, `woodlandhealth`, `woodlandyield`, `pasture`, `cropinitial`, `woodlandinitial` and `pastureinitial`. Formulae must be deterministic and return a finite multiplier from zero through one. Inputs and units are documented in the [native-yields design](Native_Organic_Yields_and_Ecological_Penalties.md). A missing formula is neutral. An invalid configured result contributes no uncertain positive growth to that channel and is diagnosed; it never turns into unrestricted growth.

The optional protection prog is validated now for the later destructive operation and is not invoked by Task 3A. It returns boolean and accepts character actor, character owner, magic capability, text method key, numeric requested amount and location cell, in that order.

Inspect current owners without awarding or consuming anything:

```text
magic environment yields here
magic environment yields repair here all
```

The repair form is only for malformed field accounting. It clears unsafe fraction/progress evidence around unchanged native stock; it does not replenish vegetation, remove scars or grant mana.

Optional progs must return a number, accept exactly one location, and use `NotStatic` caching mode. They must perform only read-only operations. Do not call resource mutations, staff operations or `invalidateenvironment` from an environmental input prog. Native source changes wake the coordinator; external policy dependencies need the explicit dirty helper or bounded periodic reconciliation. A current managed resource mutation always validates its necessary inputs before allowing a credit or debit.

## Bind cells and terrains

These commands use the admin `magic` surface:

```text
magic environment cell <here|cell id> <profile>
magic environment cell <here|cell id> inherit
magic environment cell <here|cell id> disabled
magic environment terrain <terrain> <profile>
magic environment terrain <terrain> none
magic environment show [here|<cell id>]
magic environment recheck [here|<cell id>]
magic environment diagnostics
```

The ordinary terrain editor also supports `terrain set environment <profile|none>`. `terrain show` displays the default environmental profile and preserves a diagnostic for a saved missing profile ID.

One effective profile applies to each physical cell. An explicit cell profile overrides the terrain default. `disabled` suppresses inheritance. `inherit` removes that override and uses the current effective terrain's default. Cell state survives overlays, profile replacement, disable/reattach and restart. A terrain-wide edit is processed incrementally; touching a managed balance observes its current binding and cap immediately.

`show` displays effective profile, inputs, balance, maximum, rate, scars, pressure, last destructive-use time and errors. It does not advance production or alter scheduling. `recheck` requests coalesced work and grants no energy. `diagnostics` reports coordinator queues, activity, budgets, evaluation/write counts and ages.

## Staff damage and repair

```text
magic environment damage <here|cell id> <damage> <pressure> <operation guid> <reason>
magic environment repair <here|cell id> <amount> <operation guid> <reason>
```

Amounts must be finite and non-negative, with a positive requested change. Supply a new non-empty GUID for each distinct operation; repeat the same GUID and request when retrying that operation. The service records the actor ID, reason, requested changes and result. An operation identity cannot apply its damage or repair twice.

Damage increases scars and records destructive use. Pressure is a severity-weighted accumulator with exponential decay under the profile's half-life. Changing that half-life preserves the prior decay history. Repair clamps at zero and reports the amount actually removed. It grants no mana, restores no crop/forage supply and leaves the historical destructive-use timestamp unchanged. Any necessary downward resource-cap reconciliation is a separate managed-resource consequence.

## FutureProg contract

| Function | Return and behavior |
| --- | --- |
| `environmentcap(location, resource)` | Number; pure current environmental maximum |
| `environmentrate(location, resource)` | Number; pure rate per real minute |
| `environmentscardamage(location)` | Number; persistent remaining scar damage |
| `environmentpressure(location)` | Number; current pressure projection without saving it |
| `environmentlastdefile(location)` | Datetime in UTC; the default datetime, year 1, when no event exists |
| `environmenthasdefile(location)` | Boolean; distinguishes an absent destructive-use timestamp |
| `invalidateenvironment(location)` | Boolean; trusted explicit policy invalidation, with no production or repair |

The `resource` argument has text name/ID and numeric ID overloads, matching existing resource-helper conventions. An invalid location, unavailable coordinator, unbound output or invalid environmental calculation reports a FutureProg runtime error. Scalar state queries do not evaluate resource input progs. Use `environmenthasdefile` before interpreting a possibly absent date because FutureProg's default datetime is a value, not a CLR null.

Existing `magicresourcelevel`, `setmagicresource`, `addmagicresource` and `subtractmagicresource` continue to use the cell's existing balance. Managed changes enforce the environmental maximum and immediately reconsider scheduling eligibility. These helpers and pure cap checks do not authorize spending unrecorded accrued production.

## Test configuration example

This is disposable test configuration, not seeded world content. It assumes an existing location-capable resource named `Test Essence`, a test terrain named `Test Meadow`, and a forage profile with a valid `food` yield key when the optional forage example is used.

```text
magic regenerator edit new environmental "Test Essence" "Test Grove"
magic regenerator set output "Test Essence" basecapacity 100
magic regenerator set output "Test Essence" baserate 1
magic regenerator set output "Test Essence" maximum max(0, basecapacity - scardamage)
magic regenerator set output "Test Essence" rate baserate
magic environment terrain "Test Meadow" "Test Grove"
magic environment cell here inherit
magic environment show here
magic environment damage here 10 3 1b0fa318-4f58-454d-a752-c06ff0cdbf87 disposable smoke damage
magic environment repair here 4 f95d38cf-7ae7-4524-a88b-c118de2cdbed disposable smoke repair
magic environment show here
```

For an optional forage-dependent rate, explicitly bind its units:

```text
magic regenerator edit "Test Grove"
magic regenerator set input localfood forage food 0.1
magic regenerator set output "Test Essence" rate baserate * localfood
```

For a field-dependent capacity, bind both presence and condition:

```text
magic regenerator set input fieldexists agriculture hasfield 1
magic regenerator set input fieldquality agriculture fieldcondition 0.01
magic regenerator set output "Test Essence" maximum max(0, basecapacity * fieldexists * fieldquality - scardamage)
```

Finish a disposable acceptance run by saving and restarting through the normal server lifecycle, inspecting the retained balance/damage, then using `magic environment cell here disabled`. A shutdown interval grants no production or natural repair; destructive-event age can still advance.
