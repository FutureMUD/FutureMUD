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
- markets refresh their cached category-pricing snapshots on that same hourly cadence before population heartbeats run
- shops run hourly automatic tax payment through `Shop.DoAutopayShopTaxes`
- economic zones schedule financial-period closure using in-game date listeners rather than a fixed real-time scheduler
- approved hotel-room rental state on properties is checked by property-owned heartbeat logic, which completes expired stays and advances unclaimed lost-property bundles toward auction or liquidation

This late-load design matters because much of the economy depends on other loaded data:

- currencies are needed almost everywhere
- future progs gate account eligibility, shopper behavior, markets, and tax filters
- item prototypes are needed for merchandise, payment items, and stocked goods
- clans can own accounts, control economic zones, and hold property
- cells are needed for branches, auction houses, conveyancing, job-finding, probate offices, morgue offices, morgue storage, stockrooms, tills, and shop stalls
- cells are also needed for stables, because each stable is a single advertised location where mounts are lodged and redeemed

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
- hotel rental taxes
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
- hotel taxes implement `IHotelTax`
- registered stock types are loaded through `TaxFactory`
- tax applicability can be filtered through FutureProgs

The stock runtime families are:

- `FlatSalesTax`
- `ValueAddedTax`
- `FlatProfitTax`
- `GrossProfitTax`
- `NetProfitTax`
- `HotelPercentageTax`
- `HotelFlatTax`

The current implementation ties taxation most directly to shops, hotel-room rentals, financial periods, and economic zones. Legal or civic meaning sits outside the tax classes themselves.

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

### Virtual Establishment Balances and Ledgers
Several economy systems can now operate without a live settlement bank account by using a persisted virtual cash reserve. Player-facing cash remains physical currency items at the character edge; establishment cash is stored as `VirtualCashBalance` rows keyed by owner type, owner id, and currency.

Every successful movement through the generic reserve helper writes a `VirtualCashLedgerEntry` with the real timestamp, optional MUD timestamp, actor, counterparty, currency, signed amount, balance after, source and destination kind, optional linked bank account, optional reference entity, and reason text. Bank-account ledgers remain authoritative for account activity; the virtual ledger records the domain reason and links back to the bank account where a movement used one.

Current users of this shared reserve/ledger path include auction houses, stables, hospitals, hotel rentals, property-owner revenues, clan virtual treasuries, legal-authority fine and bail revenue, economic-zone retained revenue, estate fallback liquidation, and job coffer/audit movements. Arenas keep their existing `VirtualBalance` column but now write virtual-cash ledger rows for arena cash and bank settlement movements.

Ledger review is intentionally bounded. Command surfaces can request fewer rows, but large requested counts are clamped before database materialisation and text-table generation.

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
- exact-stock buy flows for system-authored purchases that must buy preselected stocked items rather than a keyword-selected variant
- recording transactions
- drawing from cash, currency piles, bank accounts, or line of credit
- managing employees, clock-in state, managers, and proprietors
- optional market-linked pricing
- automatic tax payment
- automatic stocking and restocking hooks
- shared employment-host contracts, openings, host staff board, scheduled rules, active tasks, and manager goals through the unified employment dispatcher
- limited support for virtual shoppers

Converter integration note:

- the RPI Engine Worldfile Converter now maps legacy keeper-attached shop data to `PermanentShop` records
- RPI `shop_vnum` becomes the FutureMUD shopfront cell, and RPI `store_vnum` becomes the permanent shop stockroom cell
- RPI delivery object vnums become fixed-price, auto-reordering `Merchandise` rows when the matching item prototypes were already imported
- live NPC shopkeeper employment/AI attachment, broad legacy buyback categories, and RPI economy flag matrices remain follow-up work rather than new shop runtime behavior

Payment methods are currently separate concrete strategy objects:

- `CashPayment`
- `OtherCashPayment`
- `ShopCashPayment`
- `BankPayment`
- `LineOfCreditPayment`

This separation is important for future extension because shop purchase logic consumes an `IPaymentMethod` rather than assuming one money source.

Runtime safety invariants:

- shop purchases revalidate stock selection and payment authority immediately before removing stock, including delayed confirmation purchases
- permanent-shop public stock comes from shopfront-held items and shallow shopfront/stockroom display paths, not private workshops or deeply nested private containers
- merchandise repricing permits ordinary reductions and rejects unsafe markup multipliers rather than allowing decimal arithmetic to crash command handling
- item preview follows normal container visibility rules; closed opaque containers do not reveal contents through shop preview

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

- market categories now come in two modes: standalone categories that price themselves directly and combination categories that derive price from weighted child categories
- both standalone and combination categories can still classify goods through tags or category mappings
- a market owns categories and active influences
- market influences adjust prices through typed impact data, which now includes supply pressure, demand pressure, and flat percentage price pressure
- if an influence targets a combination category, the impact is expanded through that category's normalized component weights until it reaches standalone leaf categories
- builder-facing `show` output for market influences and market influence templates now includes a leaf-expansion preview for any combination-targeted impacts, listing the normalized weight and redistributed supply, demand, and flat-price effect for each leaf category
- market influences and influence templates can also target specific market populations with additive and multiplicative income-factor adjustments
- market populations define spending needs, base income, savings reserves, savings caps, stress thresholds, and a hysteresis / flicker threshold for stress-point demotion
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
- `HotelRoom`
- `HotelRoomRental`
- `HotelLostProperty`

The current implementation ties property to:

- an economic zone
- one or more cells
- ownership records for characters or clans
- sale and lease workflows
- conveyancing cells where player workflows are surfaced
- bank-account or virtual-reserve backed money movement for sale or rent collection
- agriculture field work permissions when a field cell belongs to a property
- approved hotel-room rental workflows through the `roomrent` command

Verified load-time constraint:

- property owner consent reconstruction for sale and lease orders matches owners by raw owner id plus owner type, not by dereferencing `PropertyOwner.Owner`

Verified current hotel-room rental behavior:

- hotel-room rental is separate from leasing a whole property but is stored on `Property`
- a property can request, surrender, or hold an approved hotel license through its `HotelLicenseStatus`
- economic-zone managers approve requested hotel licenses
- approved hotel properties can rent listed rooms with either a configured hotel bank account or the property's virtual hotel reserve
- hotel rooms are property cells with their own daily price, security deposit, minimum duration, maximum duration, assigned property keys, and furnishing list
- property owners or authorised clan property managers configure hotel rooms, room keys, furnishing markers, bans, eligibility progs, lost-property retention, and the optional hotel bank account through `roomrent`
- guests pay room rent, security deposit, and calculated hotel taxes up front; proceeds are credited to the hotel bank account when present, otherwise to the property's virtual hotel reserve, and the property tracks outstanding hotel taxes until the manager remits them
- checkout returns held room keys, assesses missing keys and furnishing loss or damage against the deposit, and records any remaining patron balance with the hotel
- a negative patron balance blocks future rentals at that hotel until paid
- non-furnishing items left in the room are bundled into hotel lost property, logged out of the world, and can later be claimed by the patron
- lost-property bundles can be extended or physically released by hotel managers while still held
- unclaimed lost-property bundles are listed on the zone's estate auction house when one exists, otherwise on another zone auction house if available; if no suitable auction path resolves, the bundle is liquidated for inferred intrinsic value and deleted
- hotel lost-property bundles are included in estate asset capture when the owner dies, so inherited bundles can be claimed after estate transfer

Persistence note:

- hotel roots are stored in `Hotels`, with hotel rooms, room keys, furnishings, active rentals, patron balances, bans, and lost-property bundles normalized into dedicated hotel tables rather than the old property XML payload

Property therefore sits at the boundary between economy, clans, law, access control, and building.

### Stables and Mount Lodging
Stables are persisted economy venues for temporarily lodging NPC mounts.

Verified current stable entities include:

- `Stable`
- `StableStay`
- `StableStayLedgerEntry`
- `StableAccount`
- `StableAccountUser`

The current implementation ties each stable to:

- one economic zone and its currency
- one cell, advertised in room description and survey output
- an optional nominated bank account for receipts, with a stable-level virtual cash reserve used when the account is absent
- optional fixed or FutureProg-driven lodge and daily fees
- optional FutureProg access control and failure text
- employee records for proprietors, managers, and employees
- stable-specific accounts, where positive balance is prepaid credit and negative balance is debt up to a credit limit

Lodging a mount creates a `StableStay`, charges the lodge fee immediately, creates a singleton-generated stable ticket item, records a ledger entry, and quits the mount out of the active world. During boot, NPC loading excludes mount character ids that belong to active stable stays, so stabled mounts remain offline and in stasis until redeem or manager release restores them. Whole-day daily fees accrue against open stays by the economic zone calendar. Fee policy changes assess all open stays before applying the new policy, so older days are not repriced retroactively.

Redeeming requires a valid stable ticket component whose stored stay id, ticket item id, and token still match an active stay. The redeemer may differ from the original lodger, but outstanding fees must be settled first by cash, bank-payment item, or an authorised stable account. Cash receipts are credited to the stable reserve if there is no linked bank account. Manager release closes the stay, invalidates existing tickets by changing the token, and logs the mount back into the stable location. Managers can waive outstanding fees or leave debt in the stay history.

Stable and hospital location ownership integrate with property. A stable inside a property is claimed alongside shops when a lease or sale produces a single character controller. A hospital whose waiting, theatre, supply, recovery, or staff-room cells belong to a property is also claimed by the leaseholder or sole character controller as proprietor; clinical staff contracts remain intact, while prior proprietor contracts are replaced. Clan and multi-owner cases remain manual in the same way as shops.

### Hospitals and Medical Services
Hospitals are persisted economy venues for paid medical work, rather than a shop subclass. They expose a shop-like player surface while creating unified employment tasks for NPC medical workers.

Verified current hospital entities include:

- `Hospital`
- `HospitalLocation`
- `HospitalService`
- `HospitalServiceRequest`
- `HospitalPatientDebtAccount`
- `HospitalBloodStockPolicy`

The current implementation ties each hospital to:

- one economic zone and its currency
- optional bank-account income routing, with the hospital virtual cash reserve used when no matching bank account is present
- waiting-room, operating-theatre, supply-area, recovery-room, and staff-room cell roles
- active service definitions for binding, wound cleaning, wound closing, wound tending, bone relocation, bone setting, configured surgical procedures, implant procedures, blood donation, blood transfusion, and automatically maintained stabilisation and full-treatment requests
- per-service equipment requirements using the employment item-selector model, marked as reusable tools or consumables for supply preparation and manager stock automation, optional recovery routing, configurable blood volume, configured anesthesia drug, target intensity, and optional cannulation procedure for IV drip anesthesia, and optional implant power/interface follow-up procedures
- per-blood-type target stock and paid-donation price policies, with command support for applying a target or price to all blood types at once
- per-patient medical-debt accounts with a maximum debt ceiling; positive account balances are prepaid credit
- shared unified-employment contracts, openings, task boards, scheduled rules, manager goals, staff boards, payroll, and ledgers

Patients use `hospital services`, `hospital service <#|name>`, `hospital request <service> [for <target>] [cash|debt|with <payment item>]`, `hospital cancel [<#>]`, `hospital debt [person]`, and `hospital debt pay <amount> [for <target>] [cash|with <payment item>]`. Hospital waiting rooms participate in the central LOOK addendum block and advertise `HOSPITAL SERVICES` when ambient service-location hints are enabled. Hospital managers and proprietors can also use `hospital operations` to review room, theatre, active procedure, staff action, blocker, and reserved-resource status in wrapped multi-line blocks rather than wide operational tables, plus `hospital cash`, `hospital deposit <amount>`, `hospital withdraw <amount>`, and `hospital ledger [count]` to review, fund, withdraw from, and audit the hospital virtual cash balance; `hospital ledger` also uses bounded multi-line ledger blocks. Withdrawals deliberately draw only from virtual cash, not the linked bank account. Service lists show a compact availability status; stocked-material and no-task-capable-medical-NPC failures appear as currently unavailable before any payment is taken. A service request can target the requester or another visible patient. Conscious third-party patients receive a consent proposal; unconscious or otherwise helpless patients are presumed to consent for emergency treatment. The requester, patient, managers, proprietors, and administrators can cancel an active request; cancellation cancels the linked employment task and aborts any staged hospital surgery effect, but amounts already paid or charged remain owed and no refund is issued for completed or partially completed work. Payment can be immediate cash, a bank-payment item, waived by authorised hospital staff, or medical debt when the service allows debt and the patient's account limit permits the charge. Debt payments can also create prepaid hospital credit for later account-funded services. Stabilisation and full treatment are shown as usage-billed services rather than fixed-price services and charge the patient's hospital debt account from the actual component services performed unless the request is waived.

A successful request creates a hospital active employment task. Services with configured equipment insert a `HospitalSupplyPreparationActionStep` before the medical step. Hospital manager goals can also maintain procedure supplies: `hospitalconsumables` purchases consumable deficits for a configured number of active-service procedure repeats, while `hospitaltools` maintains reusable tool counts and treats supply-room stock, theatre-held tools, and tools carried by active medical staff as available. Supply workers need `PrepareMedicalSupplies` and `CanPrepareHospitalSupplies`; doctors with medical and supply authority are fallback candidates only when no active, reachable, unassigned non-medical supply worker can take the work. The supply step collects the configured items from one hospital supply room and delivers them to the reserved treatment theatre through the task-item custody APIs. Command-routed bedside hospital services without explicit equipment insert the same supply-preparation step when useful `ITreatment` stock exists, allowing an orderly to deliver a small relevant bundle before the doctor escorts the patient; carried bundle contents count as held task supplies so the worker does not keep returning to the supply room for items already in the bundle.

The medical step needs `PerformMedicalServices` and `CanPerformMedicalServices`. A service is not advertised as currently available unless at least one active, able, unassigned medical NPC employee has a task-enabled `EmploymentWorkerAI` with the `CanPerformMedicalServices` capability for that hospital. Theatre-preferring services reserve an empty operating theatre; full-treatment and stabilisation services also force theatre use even if an older service row was saved without the theatre preference flag. Empty means no other active hospital request reserves it and no unrelated non-staff occupant is present. The availability check ignores the same request by request id or linked employment task id, and it treats the request patient as related by character identity so the patient cannot block their own procedure. The assigned medical worker first paths to the patient, then the service step transfers both patient and worker into the reserved theatre with source and destination echoes. If a non-theatre service intentionally treats in place, the worker emits an in-room emote before beginning. If no empty theatre exists for a theatre-required service, the task blocks rather than silently falling back to an occupied room.

The action runner uses existing health systems rather than a parallel treatment model. Bedside hospital services start the same player command routes used by live characters (`bind`, `suture`, `cleanwounds`, `tend`, and `relocate`) and then leave the employment task in progress while those commands' normal delayed effects, blockers, item plans, room echoes, and restrictions resolve. Combined stabilisation and full-treatment services advance one command-routed phase at a time, persisting phase state on the employment step; cleaning and tending phases are marked complete after one command effect, so repeatable antiseptic, cleaning, and anti-inflammatory treatments do not restart indefinitely on the same request. If a requested combined service has no valid visible wounds, fractures, or blood-loss need, it fails with that reason rather than completing silently. Configured surgery or implant services start the selected `ISurgicalProcedure`. Surgical services that need an unconscious patient can administer a configured injectable anesthesia drug at a calculated intensity before the procedure starts; if a cannulation procedure is configured, the hospital starts cannulation as a staged prep procedure, then connects a prepared IV liquid container through a drip to the installed cannula, primes a calculated anesthetic volume through the liquid injection path, sets the drip rate, and starts the bag in drip mode. Implant services can create a configured implant prototype, pass it into the install procedure, and then continue through configured implant power and interface procedures before completing the hospital request. Staged surgical requests remain in progress until `SurgicalProcedureEffect` completes or aborts, then the hospital request is completed or failed from the effect callback.

Blood donation removes the configured volume from the donor while respecting a safe minimum blood percentage, then stores a `BloodLiquidInstance` in a supplied liquid container. If the hospital is below its target stock level for that donor blood type, the configured per-litre policy pays the donor from hospital cash reserves or the linked bank account up to the target-fill amount. Blood transfusion consumes compatible blood from a prepared, held, or in-room liquid container, rejects incompatible donor blood through `IBloodtype.IsCompatibleWithDonorBlood`, and caps restoration at the recipient's total blood volume.

If a service requires recovery, post-treatment routing sends helpless patients to the first recovery room and tries to place them on a suitable bed-like item; conscious patients return to the first waiting room. If the configured destination is unavailable, the patient remains in place and the request records an audit note. Terminal hospital execution failures mark the service request failed and fail the active employment task so the assigned worker is eligible for other work; recoverable blockers such as missing prepared supplies remain blocked with the reason visible on the task list and detail views.

### Auctions
Auctions are implemented through `AuctionHouse`.

Current verified runtime characteristics:

- an auction house belongs to an economic zone
- it is tied to a specific cell
- it routes proceeds into an optional bank account or its virtual cash reserve
- it is surfaced through economy commands rather than being embedded into shops
- configured flat and percentage auction fees are retained by the auction house, with sellers receiving net proceeds
- auction-house building commands, including revenue-account and fee configuration, are administrator-only and reject non-admin callers before editing state
- bid receipts are retained in the auction house reserve first; refunds and seller payouts draw from virtual cash before falling back to the linked bank account
- standard player auction commands now work for both item and property lots, including estate-liquidation property lots whose names are ordinary property names rather than inventory items
- hotel lost-property bundles can be listed as item lots by the property when their retention period expires
- buyout purchases immediately settle the lot rather than waiting for the normal auction end tick
- player-listed property auctions now sell the listing seller's ownership share in the property, and lot descriptions explicitly call out that they are ownership-share sales rather than sole-control sales
- winning property-share bids transfer the sold share directly at auction completion without requiring a fixed-price property sale order
- persisted active and unclaimed lots now defer seller and payout-target resolution until the post-NPC boot finalisation pass, so auction loading no longer materialises characters before jobs are available

This makes auctions a distinct sales venue with different operational assumptions from ordinary retail.

### Legacy PC-Facing Jobs
Legacy jobs are implemented as a persisted economy subsystem, not as a lightweight clan payroll feature.

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

The system is integrated with job-finding cells on economic zones and with command workflows in `EconomyModule`. It remains separate from the newer unified employment-host model described below.

### Unified Employment Hosts and Task Dispatch
Unified employment is the newer host-facing employment and operations layer. It is intentionally independent from `IJobListing`, `IActiveJob`, job-finding cells, job coffers, and the `job` command.

Verified runtime parts include:

- `IEmploymentHost` and host-state shells for shops, auction houses, combat arenas, banks, stables, hospitals, durable hotel roots, and clans
- persisted employment host state keyed by host type and host id
- persisted staff `IBoard` references for host communication, separate from task routing
- employment contracts, job openings, runtime opening revision/application profile guards, applications, compensation terms, payment methods, delegated authority, payroll liabilities, manager goals, scheduled rules, action plans, active tasks, step operational state, employment register rows, and employment ledger rows
- `EmploymentWorkerAI` for NPC application, workplace travel, task claiming, action-step execution, payroll claiming, and arrears-driven resignation. Hospital staff use the same AI: `host hospital|clinic|infirmary` narrows job searching, while `capability doctor|medical` maps to `CanPerformMedicalServices` and `capability orderly|nurse|supplies` maps to `CanPrepareHospitalSupplies`.
- shared `employment` command adapters, including `employment clan <clan> ...`, plus local host aliases on `shop`, `stable`, `hospital`, `bank`, `auction`, `arena`, and `roomrent`
- scheduled-rule authoring and condition catalogues for manual, time, stock, account, item, commodity, shop-account, register-float, tax, and weather conditions
- action catalogues for retrieval, delivery, logistics, planning, authorisation/reservation, board posts, command steps, store-account payment, tax payment, bank/virtual-cash movement, shop/register float, physical task-custody cash, exact-stock shop purchases, and native craft start/resume/output custody, and hospital service or hospital-administration actions

Current operational boundaries:

- task routing uses `IEmploymentTaskBoard`; the staff `IBoard` is only an employee/manager communication surface
- financial actions require delegated authority plus explicit authorisation/reservation state and write employment audit evidence while reusing native finance records where available
- executable shop-purchase tasks must be completed at a supplier shop location; commodity merchandise stays on the weighted purchase path and is not eligible for count-priced merchandise or item-selector purchases
- payroll accrual expires fixed-term contracts before evaluation, uses explicit schedule windows for hourly guaranteed-hours contracts, rejects unsupported paid cadences until explicit earning records exist, worker resignation uses employee-specific arrears, and settlement validates every accrued payable destination before funds move, debits backed employer funds before cash payables become claimable, and credits employee or specified bank-account payables through native bank transactions; hosts without a native finance adapter must explicitly expose a currency-backed finance surface
- item movement uses inventory plans where possible, with narrow fallbacks for behaviours the inventory-plan API does not model cleanly; physical employment logistics are constrained to the host's work locations and physical cash steps only consume task-custody currency piles
- clan employment work locations are all distinct cells in properties where the clan has any ownership share plus admin-managed clan hall cells for non-property workplaces
- clan employment finance uses the clan bank account currency when present, otherwise an existing contract compensation currency where available; paid opening/task/payroll authoring that needs a host currency blocks if neither source exists, and supported clan cash movement uses `VirtualCashLedger` with the clan bank account as optional backing
- worker AI does not autonomously retry active tasks once they enter `Blocked`; blocked logistics tasks preserve any task-item custody for manager review instead of producing repeated delivery attempts
- proprietor contracts cannot be terminated by non-admin managers, and manager goals must require the combined authority of the declared goal, its conditions, and its action plan
- durable hotel roots and hotel room/rental/furnishing/lost-property internals are persisted through normalized hotel tables; hospitals persist their own service, room-role, request, and patient-debt tables
- central scheduled-rule evaluation runs once per minute and worker AI can also evaluate its current host before claiming work
- legacy shop/stable employee XML, bank/arena manager lists, and the PC-facing job system are not migrated into the new contracts by default

### Clan Finance, Budgets, and Payroll History
Clan finance is implemented across the clan and economy layers. Clans can nominate a default bank account, use physical treasury rooms, and now maintain per-currency virtual treasury balances for payroll float and appointment-budget drawdowns.

Appointment budgets are persisted as `ClanBudget` rows with:

- a clan
- an assigned appointment
- an optional backing bank account and a currency
- an amount per recurring period
- the current period start, end, and drawdown
- an active flag

Budget drawdowns are persisted as `ClanBudgetTransaction` rows and also write virtual treasury ledger records. Each transaction records the budget, actor, optional backing bank account, currency, amount, MUD time, period window, balance after withdrawal, and audit reason. The runtime rolls the current period forward lazily when a budget is reviewed or used; historical drawdown rows remain available for audit.

Clan balance-sheet review is a reporting surface rather than a separate ledger. It aggregates currently loaded economy state from clan-owned bank accounts, virtual treasury balances, owned and leased properties, property lease revenue and commitments, shops tied to clan property or clan bank accounts, controlled economic-zone revenues, payroll commitments, budget commitments, and outstanding backpay.

Clan payroll history is persisted separately from the employment/job subsystem. `ClanPayrollHistory` rows are written when clan payday processing accrues pay, when a character collects owed clan pay, and when authorised users make manual backpay adjustments. This gives clans an audit trail for their rank, appointment, and individual payroll activity without changing the broader `JobListingBase` / `OngoingJobListing` employment model.

Unified clan employment is separate from this legacy clan payroll history. Clan-host contracts, openings, task boards, scheduled rules, manager goals, employment ledgers, and payroll liabilities live in the unified employment-host model; clan rank/appointment payroll remains the existing clan payday system.

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
- native `ShopDeal` pricing supports sale and volume deals; employment automation can create, modify, and cancel those shop-owned deals without mutating market influences

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
- markets rebuild cached category-pricing snapshots hourly and also invalidate those caches immediately when category, market, or influence data changes
- populations recalculate stress against configured needs and thresholds
- population stress is now the uncovered budget shortfall after applying both income and any accumulated savings
- populations can accumulate savings when their effective income exceeds current market-adjusted costs, up to a per-population savings cap
- populations consume savings before any remaining shortfall becomes stress
- stress thresholds can execute progs on entry or exit
- falling stress uses a per-population hysteresis buffer so a threshold stays active until stress drops below `threshold - flicker threshold`, preventing rapid on/off oscillation around the boundary
- shoppers can choose shops and items according to scripted weights and buy items through the shop-facing virtual shopper path

### Verified current market-price and income calculation shape
- category price multipliers are now calculated as the market formula result plus the summed flat percentage price adjustments for that category, clamped to zero or above
- standalone categories still calculate supply, demand, and flat-price pressure directly from active influences
- combination categories now read cached per-market weighted averages of their constituent categories rather than recursively querying live child prices on every access
- item market pricing still selects the highest applicable category multiplier, and combination categories participate in that same max-selection logic when their tags match an item
- the stock seeder now demonstrates this with family-level combination categories such as `Medicine`, `Writing Materials`, `Clothing`, `Intoxicants`, `Household Goods`, `Hospitality`, `Entertainment`, `Personal Services`, `Communications`, `Military Goods`, and `Professional Tools`, using non-equal stock weights over the current `UsefulSeeder` direct child tags while leaving more world-specific baskets such as `Nourishment` for builders to tune themselves; the medicine family weights now include herbal remedies, treatment supplies, surgical supplies, and prosthetics/mobility as direct components
- seeded sector-wide templates target aggregate combination families for their direct weighted components, and still target deeper standalone descendants under those families so specific categories such as weapon, ammunition, prepared-meal, or service subtypes are not left without baseline external pressure examples
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
- `StableTicketGameItemComponent`
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
