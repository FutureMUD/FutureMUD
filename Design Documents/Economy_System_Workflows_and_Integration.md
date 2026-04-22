# FutureMUD Economy System: Workflows and Integration

## Scope
This document explains how builders and engine contributors work with the current economy system in practice.

It is written for a mixed audience:

- builders and admins who need to stand up or tune an economy
- contributors who need to add new economy features without bypassing existing patterns

Where guidance is based directly on current command and runtime behavior, it is presented as verified current state. Where guidance describes the safest extension path for future work, it is presented as inferred implementation guidance.

## Working With the Economy Today
The current economy implementation is rich and only partially seed-driven. In practice, builders combine stock seed packages with admin commands, editable-item workflows, and ordinary world-building content such as cells, items, progs, and clans.

### Primary command surfaces
| Surface | Current responsibility |
| --- | --- |
| `EconomyModule` | currencies, coins, banks, shops, auctions, jobs, markets, market influences, market categories, market populations, shoppers, player-facing buy and sell operations |
| `PropertyModule` | property sale, lease, ownership, keys, and conveyancing-facing workflows |
| `EditableItemHelperEconomy` | standardized admin creation and editing flows for property, auction houses, banks, coins, and shoppers |

### Practical implication
Most economy content is authored as world data, not hard-coded content. The engine supplies the abstractions and workflows, and builders connect them to a specific world through:

- chosen currencies
- cells
- item prototypes
- clans
- FutureProgs
- market category tags
- bank accounts and payment items

The current stock seeder path now covers two useful starting points:

- `CurrencySeeder` for currencies, divisions, coins, and parsing/description patterns
- `EconomySeeder` for a template economic zone shell, one market, market categories derived from `UsefulSeeder` market tags, seeded weighted combination-category examples for setting-agnostic family baskets, stock influence templates, broader era-specific populations including priestly and monastic households, and matching `SimpleShopper` records

## Minimum Viable Economy Setup
The current runtime supports a lot of optional depth, but the minimum viable path is smaller.

### Recommended build order
1. Seed or create at least one currency.
2. Seed `UsefulSeeder` market tags if the world wants the stock economy template package.
3. Run `EconomySeeder` if a builder-facing template market, populations, and shoppers would save setup time.
4. Review the seeded economic zone, market categories, populations, and shoppers and adjust them to fit the world's real geography, item tags, and intended simulation depth.
5. Add any initial sales or profit taxes to each live economic zone.
6. Create a bank if the world needs account-backed payments, property ownership, shop floats, or auction settlement.
7. Add bank account types before expecting players, clans, or shops to use accounts meaningfully.
8. Create at least one shop or other money sink/source if the world needs day-to-day commerce.
9. Point relevant shops at a market if pricing should reflect macroeconomic pressure rather than fixed local pricing only.
10. Add job-finding cells, jobs, and employers if the world will use the employment system.
11. Add conveyancing cells and property data if the world will use formal property ownership, sale, or leasing.
12. Decide whether estates are enabled for each zone, then add probate offices if players should interact with estates in the zone.
13. Add morgue office and morgue storage cells if the world will use corpse recovery and morgue claim workflows.

### Why this order fits the current implementation
- currencies are prerequisites for almost every downstream object
- economic zones are the tax and time hub
- the stock economy package depends on `UsefulSeeder` market tags plus the earlier time and currency layers
- banks unlock multiple other systems, including payment instruments and property administration
- markets, shoppers, and jobs all assume earlier layers already exist
- property and auctions depend heavily on cells, banks, and world-specific content

## What Builders Still Need To Hand-Build
Even with the existing abstractions, much of the economy remains world-authored content rather than ready-made stock configuration.

### Currently hand-built in most worlds
- tax regimes
- banks and account types
- exchange rates and currency reserves
- shops and their staffing
- merchandise catalogs
- payment-item prototypes and bank-payment items
- live economic-zone layout and market assignment strategy
- world-specific market populations and stress-threshold tuning
- shopper selection and item-choice progs beyond the stock tag-driven templates
- auction houses
- property portfolios and location mapping
- jobs, employers, and eligibility logic

### Why hand-building still dominates
The runtime is generic by design. Most of these systems depend on world-specific choices that the engine cannot safely assume:

- which cells matter
- which clans exist
- which items are sold
- who can open what account
- what counts as demand in a specific setting
- how much simulation depth the world actually wants

## Builder Workflow by Subsystem
### Currencies and Coins
Builders can:

- seed stock currencies through `CurrencySeeder`
- seed a stock economy template package through `EconomySeeder`
- create new currencies and coins through economy admin commands
- edit divisions, abbreviations, and description patterns
- set global-base conversion so cross-currency comparisons remain possible

Practical note:

- currency design is not only cosmetic
- regex abbreviations determine what players can type successfully
- description patterns determine how values are surfaced all across the engine
- the stock Pounds package now seeds historical compact `£sd` notation, using `d` forms below one shilling, slash forms from one shilling to less than one pound, full `£/s/d` forms above one pound, quarter-penny glyphs (`¼`, `½`, `¾`), and `–` for zero slots in slash notation

### Economic Zones and Taxes
Builders use economic zones to define:

- the local administrative currency
- the financial-period cadence
- tax policy
- whether the zone creates estates at all
- the cells used for conveyancing, job-finding, probate, and morgue workflows
- the clan, if any, that controls the zone

Tax creation is type-driven through the registered tax families, so the current builder workflow is already aligned with the factory pattern in the runtime.

### Banks and Bank Account Types
Banks are a natural early investment if the world intends to use:

- shops with bank-backed balances
- bank payment items
- property ownership or rent collection
- auctions
- clan finances

Current builder-facing bank work includes:

- creating the bank itself
- adding branches
- configuring exchange rates and reserves
- creating account types
- attaching open and close permission progs
- tuning fees, interest, and payment-item limits

### Markets, Categories, Influences, Populations, and Shoppers
The market system becomes useful when the world wants more than fixed shop pricing.

Current builder steps usually include:

- optionally start from the seeded template market created by `EconomySeeder`
- review or extend the generated market categories that follow `UsefulSeeder` market tags
- decide whether each category should remain standalone or become a combination category built from weighted child categories
- create one or more markets per economic region
- add market influence templates or live influences
- define or tune market populations and their spending needs, base income factor, current savings, savings cap, and stress flicker threshold
- create or tune shoppers, usually `SimpleShopper`, with scripted selection behavior
- point relevant shops at a market for pricing purposes

The population and shopper layers are where FutureProg dependence becomes especially important.

Practical note on the stock seeder package:

- the seeded market is a starting template, not a claim that the world should only have one market
- the seeded external templates are intended to be balanced examples and can be reused or edited
- the seeded population stress hooks already demonstrate the begin/end influence pattern through stock FutureProgs
- market influence templates and live influences can now express both flat percentage price pressure and direct income pressure on named populations
- combination categories let builders create higher-level sectors such as staple foods or luxury baskets without duplicating direct pricing data on the aggregate category itself
- combination-targeted influences are applied to the constituent standalone categories in normalized proportion, so builders can target a broad basket while still moving the underlying goods
- `mit show` and `mi show` now expose a leaf-expansion preview for those combination-targeted impacts, so builders can inspect the normalized leaf-category supply, demand, and flat-price effects before applying the template more broadly
- the stock seeder now uses that pattern directly for family categories such as `Medicine`, `Writing Materials`, `Clothing`, `Household Goods`, `Hospitality`, `Entertainment`, `Personal Services`, `Communications`, `Military Goods`, and `Professional Tools`
- seeded sector-wide external and stress templates now target either those aggregate family categories or the remaining standalone families, so builders can inspect working examples of combination-aware influence authoring
- tariff and subsidy style templates now model flat percentage price adjustments rather than trying to fake those effects through supply or demand alone
- the seeded populations assume medicine is a universal household need and now use seasonings tags such as `Salt` and `Spices`, writing-material tags such as `Wax Tablets`, `Parchment`, `Paper`, and `Ink`, plus hospitality / entertainment / communications / personal-service tags in later eras where appropriate
- seeded stress templates now model both demand contraction and some supply contraction tied to the sectors a stressed population plausibly anchors
- seeded populations now start with explicit income factors, non-zero savings caps, and a default `1%` stress flicker threshold so the savings mechanic and population hysteresis are visible without additional builder setup
- seeded money values are scaled against the selected currency package and a simple era baseline so builders start closer to plausible local price magnitudes
- the seeded shopper progs assume goods are tagged with the same market tags used by the generated categories

### Shops, Merchandise, Line of Credit, and Payment Methods
Shops are one of the most configuration-heavy economy subsystems.

Practical builder work currently includes:

- choose permanent versus transient shop behavior
- assign the economic zone and optional market
- set up bank accounts if the shop should hold money outside loose cash
- define merchandise rows and their pricing behavior
- configure stocking, display containers, and restock behavior
- configure line-of-credit accounts if credit sales should be allowed
- decide what payment methods the world wants players to use

Current payment methods already support:

- physical cash
- other cash handling paths
- bank-payment items backed by bank accounts
- line of credit

### Auctions
Auction houses currently fit worlds that want formal auction spaces separate from ordinary shops.

Builders need:

- a cell for the auction house
- an economic zone
- a settlement account

Practical estate note:

- probate liquidation can proceed without a configured auction house, but non-cash estate assets will then need manual handling because automatic liquidation listing has nowhere to post them

Current player-facing capability note:

- auction houses can now host both item lots and property-share lots, and ordinary character-listed property auctions sell whatever ownership share the listing character currently owns in that property

Contributor note:

- boot-time auction reconstruction now resolves seller and payout references in a post-character finalisation pass rather than in the constructor path

### Property and Employment
Property and jobs both depend strongly on world layout and institutions.

Property builders need:

- cells mapped into properties
- an economic zone
- owners or ownership rules
- sale or lease setup
- conveyancing cells so the player-facing workflow is discoverable
- clan ranks or appointments that should use clan-owned property items need the `Use Clan Property` privilege in addition to any broader management privileges

Contributor note:

- sale-order and lease-order consent loading must compare property owners by stored owner id and owner type rather than dereferencing `PropertyOwner.Owner` during boot

Job builders need:

- an employer
- a zone with job-finding cells
- pay currency and pay model
- an eligibility prog

## Integration Guidance for Future Features
This section is inferred implementation guidance based on current patterns.

### Adding a New Tax Type
Safest current pattern:

1. extend the appropriate tax base or interface
2. persist any new fields in database models and migrations
3. register the type in `TaxFactory`
4. surface it through the existing economic-zone builder flow

Why this fits the current design:

- economic zones already create taxes by registered type name
- taxes are already treated as policy objects rather than hard-coded branches

### Adding a New Shopper Type
Safest current pattern:

1. subclass `ShopperBase`
2. register database and builder loaders through the existing loader registration mechanism
3. keep shopper-specific configuration self-contained in the shopper definition payload
4. reuse existing market and shop abstractions rather than embedding direct purchase logic elsewhere

### Adding a New Payment Method
Safest current pattern:

1. implement or extend `IPaymentMethod` handling in the runtime
2. wire the new payment path into shop command parsing and validation
3. add an item component if the method needs a physical in-world instrument
4. keep shop logic consuming the abstract payment method instead of special-casing one venue

Current examples to emulate:

- `BankPayment`
- `CashPayment`

### Adding Pre-NPC Economy Loads That Reference Characters
Safest current pattern:

1. keep persisted references as raw ids and framework-item types during constructor load
2. do not call `TryGetCharacter(...)` in any loader that runs before `LoadNPCs()`
3. if the object must resolve character-backed references during boot, implement `IPostCharacterLoadFinalisable` and perform that work in `FinaliseLoading()`
4. keep the deferred finalisation idempotent so repeated calls are harmless during tests or recovery code
- `LineOfCreditPayment`

### Adding New Market Influence or Template Types
There are two current extension strategies:

- stay within the existing market influence data model and add new content through builder workflows
- extend runtime influence behavior only if the current impact model is genuinely insufficient

In both cases, preserve the separation between:

- reusable templates
- live influences applied to a market

Current practical guidance:

- use flat price pressure for tariffs, duties, subsidies, and other fees that should directly move the final multiplier
- use supply and demand pressure when the world event should still flow through the market formula
- use population income impacts when the event changes what households can spend rather than what goods cost
- use combination categories when builders want a tagged roll-up sector whose price should track a weighted basket of more concrete underlying categories
- remember that combination categories cache derived prices per market and refresh hourly or on data invalidation, so they are intentionally a little stale rather than recursively live on every lookup

### Adding New Currency Patterns
Currency extension should generally stay pattern-driven.

Safest current pattern:

- add or tune divisions and regex abbreviations
- add or tune description patterns and elements
- keep display logic inside currency pattern objects rather than scattering formatting code into consumers

### Adding Shop-Adjacent or Property-Adjacent Systems
If a future feature touches commerce, ownership, or taxation, it should usually integrate with the existing economy hub instead of inventing a parallel flow.

Good current integration anchors are:

- economic zones for time, tax, and locality
- bank accounts for stored value and settlement
- shops for retail movement and transaction records
- property for formal location ownership
- FutureProg for permission and selection policies
- item ownership metadata and item ownership FutureProg helpers for portable property, estate assets, and clan-owned equipment

## Verified Current Limits That Matter During Integration
These are not hypothetical concerns. They affect how safe it is to integrate new features right now.

### Volume deals are not live
The shop API shape anticipates deals, but the runtime does not currently implement them. New shop-adjacent work should not assume that `ShowDeals()` or deal-specific price flags reflect live behavior.

### Estates and morgues are zone-shaped workflows
Current economy-world setup now needs to account for both probate and morgue access points:

- economic zones can now opt out of creating new estates entirely with the `estates` toggle
- probate offices expose the player-facing `estate` command surface
- morgue offices expose the player-facing `morgue` command surface
- morgue storage rooms are back-room custody spaces and should not be used as public traffic cells
- the game-wide `NpcEstateProg` static configuration determines whether NPCs can enter the estate workflow at all, while guests are always excluded

The underlying workflow is intentionally split across economy and legal systems:

- the economic zone owns the probate and morgue locations
- the legal authority for the corpse's zone owns the pickup queue and patrol response
- estate claim timing is opened either by the normal discovery timer or by morgue intake/admin action
- corpses can still be recovered into morgue storage even when no estate is created, in which case belongings remain on the corpse
- corpse recovery patrol dispatch now prefers patrols configured for the dedicated corpse-recovery strategy
- probate offices are also where characters create will estates, manage bequests, and collect persisted cash payouts that could not be delivered straight into bank accounts

### Seeder depth is narrow
If a new feature depends on the broader economy existing out of the box, it will probably still need either:

- new seeder work
- a builder workflow that can tolerate a mostly manual economy setup

## Suggested Contributor Checklist
This is inferred workflow guidance, but it matches the current implementation well.

When extending the economy:

1. decide whether the new behavior belongs in an existing bounded context or a new one
2. add or adjust interfaces only when the capability is genuinely cross-project
3. update persistence and migrations for new runtime state
4. integrate with existing factories, loaders, or editable-item helpers where possible
5. expose builder-facing workflows rather than requiring raw database edits
6. update the economy design docs when runtime behavior or command surface changes
