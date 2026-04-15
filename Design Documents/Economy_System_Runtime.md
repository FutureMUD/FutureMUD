# FutureMUD Economy System: Runtime

## Scope
This document explains the verified current runtime implementation of the FutureMUD economy system.

It focuses on the code centred on `MudSharp.Economy`, but also includes the adjacent runtime surfaces that are required to understand or extend that subsystem:

- interfaces in `FutureMUDLibrary`
- concrete runtime logic in `MudSharpCore`
- persistence models and migrations in `MudsharpDatabaseLibrary`
- loading and scheduling in `MudSharpCore/Framework/FuturemudLoaders.cs`
- command, FutureProg, effect, and game-item integration points

This document is intentionally current-state focused. Where it includes forward-looking guidance, that guidance is called out as inferred rather than presented as existing behavior.

## Layered Shape
FutureMUD's economy is not implemented as a single module with a single entry point. It is a distributed subsystem with one dominant runtime namespace and a supporting ring of contracts, persistence, commands, and item integrations.

| Layer | Current responsibility | Typical locations |
| --- | --- | --- |
| Contracts | Public interfaces and shared value types | `FutureMUDLibrary/Economy`, `FutureMUDLibrary/Economy/Currency`, `FutureMUDLibrary/Economy/Property`, `FutureMUDLibrary/GameItems/Interfaces` |
| Runtime | Concrete logic, editing, pricing, payment, financial periods, scheduling callbacks | `MudSharpCore/Economy`, `MudSharpCore/Commands/Modules/EconomyModule.cs`, `MudSharpCore/Commands/Helpers/EditableItemHelperEconomy.cs` |
| Persistence | EF Core tables for currencies, banks, shops, markets, property, shoppers, and related records | `MudsharpDatabaseLibrary/Models`, related migrations |
| Integration | Boot order, FutureProg registration, item components, effects, player and admin commands | `MudSharpCore/Framework/FuturemudLoaders.cs`, `MudSharpCore/FutureProg/Functions`, `MudSharpCore/GameItems/Components`, `MudSharpCore/Effects/Concrete` |

The design is interface-first in the sense used elsewhere in FutureMUD. Public surfaces such as `ICurrency`, `IEconomicZone`, `IShop`, `IMarket`, `IBank`, and property interfaces live in `FutureMUDLibrary`, while most behavior lives in the `MudSharpCore` implementations.

## Boot and Load Order
The economy system is loaded late in the boot sequence, after the world, future progs, currencies, clans, and item prototypes needed by its integrations are already present.

### Verified current boot order
- `LoadCurrencies()` runs early enough for later economy systems to reference live currencies.
- `LoadClans()` runs before economy because economic zones, banks, and property can refer to clans.
- `LoadEconomy()` runs late, after crafts and before markets, legal, jobs, and world items.
- `LoadMarkets()` runs immediately after `LoadEconomy()`.
- `LoadJobs()` runs after `LoadEconomy()`.
- new character materialisation is explicitly blocked during boot until `LoadNPCs()` begins, so pre-NPC loaders must not force a fresh `TryGetCharacter(...)` from the database.
- economy or other pre-NPC loaders that genuinely need character-dependent resolution must defer that work through `IPostCharacterLoadFinalisable.FinaliseLoading()`, which now runs immediately after `LoadNPCs()`.

### Verified current scheduled runtime hooks
- market populations receive an hourly heartbeat through the scheduler
- shops run hourly automatic tax payment through `Shop.DoAutopayShopTaxes`
- economic zones schedule financial-period closure using in-game date listeners rather than a fixed real-time scheduler

This late-load design matters because much of the economy depends on other loaded data:

- currencies are needed almost everywhere
- future progs gate account eligibility, shopper behavior, markets, and tax filters
- item prototypes are needed for merchandise, payment items, and stocked goods
- clans can own accounts, control economic zones, and hold property
- cells are needed for branches, auction houses, conveyancing, job-finding, probate offices, morgue offices, morgue storage, stockrooms, tills, and shop stalls

## Subsystem Map
### Currency
Currency is the value substrate for the whole system.

The current runtime model is:

- `ICurrency` owns divisions, coins, and description patterns
- `CurrencyDivision` defines unit size and parsing abbreviations
- `Coin` defines minted denominations
- `CurrencyDescriptionPattern` plus `CurrencyDescriptionPatternElement` define how amounts are rendered for different display styles
- `CurrencyGameItemComponent` and `ICurrencyPile` bridge abstract value into physical carried money

Important consequences of the current implementation:

- player-facing parsing is regex-driven through division abbreviations
- display is pattern-driven rather than hard-coded to one notation
- currencies support a global-base conversion factor, so worlds can compare values across multiple currencies without forcing a single in-character denomination
- shops, appraise logic, legal systems, arena finance, chargen roles, and craft products all consume currency data outside the economy namespace

### Economic Zones and Financial Periods
`IEconomicZone` is the main administrative hub of the economy runtime.

An economic zone currently owns or exposes:

- a primary currency
- sales taxes, profit taxes, and income taxes
- outstanding tax state per shop
- total held revenue and historical revenue snapshots
- a current financial period and retained prior periods
- the reference calendar, clock, time, and time zone used for financial periods
- conveyancing cells for property workflows
- job-finding cells for employment workflows
- probate-office cells for estate workflows
- a morgue office cell and morgue storage cell for corpse custody workflows
- a controlling clan
- estate settings, including a master estates-enabled toggle, and estate collection hooks

Financial periods are not passive records. The zone actively closes the current period, computes shop financial results, rolls revenue forward, prunes retained history, and schedules the next closure.

### Taxes
Taxes are zone-owned policy objects rather than global rules.

Current verified structure:

- sales taxes derive from `SalesTaxBase`
- profit taxes derive from `ProfitTaxBase`
- registered stock types are loaded through `TaxFactory`
- tax applicability can be filtered through FutureProgs

The stock runtime families are:

- `FlatSalesTax`
- `ValueAddedTax`
- `FlatProfitTax`
- `GrossProfitTax`
- `NetProfitTax`

The current implementation ties taxation most directly to shops, financial periods, and economic zones. Legal or civic meaning sits outside the tax classes themselves.

### Banking
Banking is implemented as a fully persisted runtime subsystem rather than a thin abstraction.

Current verified banking entities include:

- `Bank`
- `BankAccount`
- `BankAccountType`
- `BankTransaction`
- `BankManagerAuditLog`

Banks currently own:

- branches tied to cells
- a primary currency
- exchange rates and currency reserves
- bank accounts
- bank account types
- manager audit metadata

Account types are especially important because they hold much of the policy:

- open and close permission progs for characters, clans, and shops
- fees, interest, overdraw rules, and payment-item allowances

Banks and bank accounts also register FutureProg variable support, so the system is designed to be scripted as well as built.

### Shops, Merchandise, and Payments
Shops are one of the largest and most integrated pieces of the economy runtime.

The current abstraction is:

- abstract `Shop` implements shared policy
- `PermanentShop` and `TransientShop` provide the two built-in runtime variants
- `Merchandise` represents sale and buyback configuration for a prototype or variant
- `TransactionRecord` captures operational bookkeeping
- `EmployeeRecord` tracks staffing
- `LineOfCreditAccount` supports credit workflows

Verified shop responsibilities include:

- tracking stock and stocked-item locations
- pricing merchandise
- applying sales taxes
- handling buy and sell flows
- recording transactions
- drawing from cash, currency piles, bank accounts, or line of credit
- managing employees, clock-in state, managers, and proprietors
- optional market-linked pricing
- automatic tax payment
- automatic stocking and restocking hooks
- limited support for virtual shoppers

Payment methods are currently separate concrete strategy objects:

- `CashPayment`
- `OtherCashPayment`
- `ShopCashPayment`
- `BankPayment`
- `LineOfCreditPayment`

This separation is important for future extension because shop purchase logic consumes an `IPaymentMethod` rather than assuming one money source.

### Markets, Influences, Populations, and Shoppers
The market subsystem models macroeconomic pressure rather than only local shop stock.

Verified current parts:

- `Market`
- `MarketCategory`
- `MarketInfluence`
- `MarketInfluenceTemplate`
- `MarketPopulation`
- `ShopperBase`
- `SimpleShopper`

The current model works roughly like this:

- market categories classify goods, usually through tags or category mappings
- a market owns categories and active influences
- market influences adjust prices through typed impact data, which now includes supply pressure, demand pressure, and flat percentage price pressure
- market influences and influence templates can also target specific market populations with additive and multiplicative income-factor adjustments
- market populations define spending needs, base income, savings reserves, savings caps, and stress thresholds
- shoppers choose shops and items using FutureProg-driven selection rules
- shops can point at a market for pricing purposes

This gives FutureMUD two different but connected economic layers:

- local retail operations through shops
- macroeconomic pressure through markets, populations, and influences

### Property
Property is a distinct economy subsystem rather than an afterthought on clans or cells.

Verified current property entities include:

- `Property`
- `PropertyOwner`
- `PropertyKey`
- `PropertyLease`
- `PropertyLeaseOrder`
- `PropertySaleOrder`

The current implementation ties property to:

- an economic zone
- one or more cells
- ownership records for characters or clans
- sale and lease workflows
- conveyancing cells where player workflows are surfaced
- bank-account backed money movement for sale or rent collection

Verified load-time constraint:

- property owner consent reconstruction for sale and lease orders matches owners by raw owner id plus owner type, not by dereferencing `PropertyOwner.Owner`

Property therefore sits at the boundary between economy, clans, law, access control, and building.

### Auctions
Auctions are implemented through `AuctionHouse`.

Current verified runtime characteristics:

- an auction house belongs to an economic zone
- it is tied to a specific cell
- it routes proceeds into a bank account
- it is surfaced through economy commands rather than being embedded into shops
- standard player auction commands now work for both item and property lots, including estate-liquidation property lots whose names are ordinary property names rather than inventory items
- player-listed property auctions now sell the listing seller's ownership share in the property, and lot descriptions explicitly call out that they are ownership-share sales rather than sole-control sales
- winning property-share bids transfer the sold share directly at auction completion without requiring a fixed-price property sale order
- persisted active and unclaimed lots now defer seller and payout-target resolution until the post-NPC boot finalisation pass, so auction loading no longer materialises characters before jobs are available

This makes auctions a distinct sales venue with different operational assumptions from ordinary retail.

### Employment
Employment is implemented as a persisted economy subsystem, not as a lightweight clan payroll feature.

Verified runtime parts:

- `JobListingBase`
- `OngoingJobListing`
- `ActiveJobBase`
- `ActiveOngoingJob`
- `JobListingFactory`

Job listings currently include:

- an employer reference
- an economic zone
- an eligibility prog
- pay configuration, including pay currency
- active job records when a character takes the job

The system is integrated with job-finding cells on economic zones and with command workflows in `EconomyModule`.

### Estates
Estates are now a live persisted subsystem.

Verified current runtime behavior:

- death creates or reuses one open estate per economic zone that receives captured assets, but only when the zone has estates enabled
- zones that receive no captured assets do not create empty estates
- guests never produce estates even though they are characters
- NPC estates are gated by the game-wide `NpcEstateProg` static configuration and default to not producing estates when the configuration is missing or invalid
- living characters can create zone-local `EstateWill` records in advance and populate them with approved bequests before death
- estates advance from `Undiscovered` to `ClaimPhase` on the zone discovery timer or immediately through `estate open <id>`
- estate managers can manually move an estate into `Liquidating` with `estate liquidate <id>`
- all non-admin player-facing estate interaction is gated to probate-office cells configured on the economic zone
- claims, assessment, liquidation, auction integration, and finalisation all persist through the database layer
- item ownership is part of the runtime path for estate transfer and morgue recovery rather than a missing prerequisite
- approved claims only force liquidation when the claim actually needs cash settlement rather than an in-kind transfer
- unresolved cash distributions to characters are now persisted as estate payouts for later collection at the probate office

The estate system remains intentionally conservative about item capture:

- items already owned by someone other than the deceased are excluded from the estate
- unowned items on the deceased are included as presumed ownership
- known deceased-owned items are included as direct estate assets

Verified current assumed-value rules:

- property uses the current listed reserve price when actively for sale, otherwise the last sold value, and estate value scales by the deceased's ownership share
- bank accounts use their live balance, converted into the estate zone currency when necessary
- items use prototype inherent value first, then the average matching shop sale price in the same economic zone, then the average matching vending-machine sale price in the same economic zone, and finally zero if no price can be inferred

Verified current claim and finalisation behavior:

- claims may target a specific property, bank account, or item instead of only generic cash
- targeted claims default to the current assumed value of the referenced asset
- targeted non-secured claims can be resolved by transferring the claimed asset in kind instead of liquidating the whole estate
- duplicate approved targeted claims against the same asset are blocked
- line-of-credit debt alone no longer creates a new estate; it only attaches to an estate that already exists because assets were captured
- partial property ownership held by the deceased is inherited in kind and is not eligible for estate liquidation
- will bequests are represented as pre-approved targeted claims and are revalidated against the real captured asset set at death

### Morgues and Corpse Recovery
Economic zones now expose a two-room morgue model:

- morgue office for public command interaction
- morgue storage for corpse and belongings custody

This integrates economy and legal runtime behavior:

- `report body <corpse>` creates a legal-authority corpse-recovery job for the local enforcement zone
- patrol dispatch prefers patrol routes configured with the dedicated `CorpseRecovery` strategy when one exists
- corpse pickup now uses a dedicated corpse-recovery patrol strategy rather than the generic investigation strategy
- successful recovery echoes a configurable static string and then ends the patrol immediately
- patrol dispatch picks up the corpse and moves it into morgue storage
- morgue intake only strips possessions into a bundle and opens probate when an estate actually exists
- if no estate exists, the corpse is still stored and its belongings remain on the corpse
- `morgue` commands in the office list releasable corpses and personally owned belongings

## Important Runtime Flows
### Currency Parsing and Description
The runtime currently separates money input from money display.

Input path:

- user text is matched against division abbreviation regexes
- matching values are converted into base currency units
- currency piles and carried coins can then be counted or manipulated against those abstract amounts

Output path:

- code asks a currency to `Describe` a value using a pattern type
- the currency selects a description pattern and renders ordered elements
- division conversion and rounding behavior are per-element, not hard-coded globally

This design is why FutureMUD can support very different notation schemes without rewriting higher-level systems.

### Shop Pricing, Taxes, and Transactions
The current shop price flow is:

- merchandise provides an effective pre-tax price
- the owning economic zone applies any matching sales taxes
- the selected payment method determines which money source is debited
- the shop updates its balances and transaction records
- the zone later rolls those outcomes into financial-period results and outstanding taxes

Important current detail:

- the shop API already exposes a detailed-price surface and a deals surface
- volume deals are not yet implemented even though the API shape anticipates them

### Autopay and Financial Period Closure
Two loops matter operationally:

- hourly autopay of taxes for shops that allow it
- date-driven closing of financial periods inside each economic zone

During financial-period closure, the zone currently:

- finalizes the old period
- creates the next period
- computes gross revenue, net revenue, sales tax, and profits tax per shop
- updates outstanding balances, tax credits, and historical revenue
- prunes old retained periods
- schedules the next close event

### Market Heartbeats and Virtual Shoppers
Markets are not only static price tables.

Verified current active behavior:

- market populations receive a heartbeat on the scheduler
- populations recalculate stress against configured needs and thresholds
- population stress is now the uncovered budget shortfall after applying both income and any accumulated savings
- populations can accumulate savings when their effective income exceeds current market-adjusted costs, up to a per-population savings cap
- populations consume savings before any remaining shortfall becomes stress
- thresholds can execute progs on entry or exit
- shoppers can choose shops and items according to scripted weights and buy items through the shop-facing virtual shopper path

### Verified current market-price and income calculation shape
- category price multipliers are now calculated as the market formula result plus the summed flat percentage price adjustments for that category, clamped to zero or above
- item market pricing still selects the highest applicable category multiplier, so the flat percentage additions automatically flow through to shop pricing
- a population's effective income factor is now `(base income factor + additive impacts) * multiplicative impacts`, clamped to zero or above
- population savings and savings caps are stored as budget-cycle multiples rather than literal time spans

This means the current implementation already supports a light economic simulation layer without requiring every purchase to come from a live player.

## Integration Surfaces Outside `MudSharp.Economy`
### Commands
The economy command surface is broad and split between general economy commands and property-specific commands.

Important command homes:

- `MudSharpCore/Commands/Modules/EconomyModule.cs`
- `MudSharpCore/Commands/Modules/PropertyModule.cs`
- `MudSharpCore/Commands/Helpers/EditableItemHelperEconomy.cs`

The editable-item helper is a particularly important integration seam because it standardizes admin creation and editing for multiple economy object types.

### FutureProg
The economy system is script-aware.

Verified current FutureProg integration includes:

- built-in conversion functions for currencies and banks
- currency utility functions for counting, loading, giving, and taking money
- economy functions for market influences
- item ownership functions for checking direct ownership, property trust, clan-authorised property use, and ownership mutation
- FutureProg variable registration on banks, bank accounts, bank account types, currencies, markets, market categories, shops, and merchandise
- item variable registration for ownership metadata, including nullable character-or-clan item owners
- multiple permission and selection hooks in banks, jobs, shoppers, shops, taxes, and market data

This matters because many higher-level world rules are intended to be configured by builders rather than hard-coded into the runtime types.

### Game Items and Effects
Economy behavior also reaches into the item and effect systems.

Verified current item or effect integrations include:

- `CurrencyGameItemComponent`
- `BankPaymentGameItemComponent`
- `MarketGoodWeightGameItemComponent`
- `ShopStallGameItemComponent`
- `ItemOnDisplayInShop`
- persisted item ownership metadata on `GameItem`, used by economy commands, estate flows, and property-oriented FutureProg rules
- `RestockingMerchandise`
- `ShopStallNoGetEffect`

These integrations are how abstract systems such as currency, payment instruments, weighted market goods, and transient stalls become concrete in the world.

## Extension Seams
### Registered or factory-driven seams
Several extension seams are already designed for type registration rather than switch-heavy modification:

- `TaxFactory` for tax types
- `ShopperBase` loader registration for shopper types
- `JobListingFactory` for listing variants
- `Shop.LoadShop` for shop runtime types

### Builder and clone seams
Many economy objects support cloning or standard editable workflows:

- currencies
- markets and market categories
- market influence templates and influences
- market populations
- shoppers
- many builder-created runtime records through editable-item helpers

This is a strong signal that FutureMUD expects builders to author and duplicate economy content in-world, not only through seed data.

### Inferred design implication
When adding future economy features, the safest path is usually:

1. extend the public interface in `FutureMUDLibrary` if a new capability is genuinely cross-project
2. persist the new state in `MudsharpDatabaseLibrary`
3. hook the runtime type into existing factory or loader patterns where possible
4. expose the feature through builder commands rather than relying on manual database setup

That sequence is inferred from current implementation style rather than enforced by one base class, but it matches the design of the existing subsystem well.
