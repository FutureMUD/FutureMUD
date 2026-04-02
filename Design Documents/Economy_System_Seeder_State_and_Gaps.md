# FutureMUD Economy System: Seeder State and Gaps

## Scope
This document explains:

1. what economy content is currently seeded
2. what economy content could be added to the seeder relatively easily from the current implementation
3. what economy content exists in runtime but is a poor seeder candidate without more design work
4. the highest-priority current gaps in the economy subsystem

This document deliberately separates verified current state from recommended future direction.

## Current Seeder Reality
### Verified current state
The current repository has two dedicated economy seeders:

- `CurrencySeeder`
- `EconomySeeder`

`CurrencySeeder` currently provides:

- stock currency packages such as dollars, pounds, fantasy, roman, bits, and Gondor
- stock divisions, coins, description patterns, and parsing abbreviations
- supporting FutureProg content for some currency pattern applicability
- additive rerun behavior so multiple currencies can coexist

`EconomySeeder` currently provides one stock economy template package per selected era. Each run creates or repairs:

- a new stock `EconomicZone` shell tied to a chosen currency and physical zone
- a stock market attached to that zone
- market categories for every seeded tag beneath the `UsefulSeeder` `Market` tag root, including intermediate and leaf tags
- a reusable library of external market influence templates grouped by sector family, with substantially broader positive and negative scenario coverage than the first pass
- era-specific market populations, including priestly and monastic households, with later eras now drawing on hospitality, entertainment, communications, personal-service, and related market tags where appropriate
- population-stress influence templates and their helper FutureProgs, with stress now reducing lower-priority demand while also contracting supply in the sectors that stressed populations plausibly sustain
- one seeded `SimpleShopper` per seeded population, with user-selected budget scale and seeded expenditures now scaled by both era assumptions and the chosen currency package

The current stock eras are:

- Classical Age
- Feudal Age
- Medieval Age
- Early Modern Age

Current prerequisites for `EconomySeeder` are:

- at least one account
- at least one currency
- at least one clock and calendar
- at least one physical zone
- the full `UsefulSeeder` stock market-tag vocabulary beneath the `Market` root, not just the root tag itself

Current rerun behavior for `EconomySeeder` is additive and repair-friendly:

- rerunning the same era refreshes stock-owned categories, templates, populations, shoppers, and helper progs without duplicating them
- running a different era installs another stock package alongside the existing one
- deleting a stock-owned asset and rerunning restores it if the canonical seeded name still belongs to the package

### What is not currently seeded as a dedicated economy package
- taxes
- banks
- bank account types
- shops
- auction houses
- property
- employment data

### Practical implication
The runtime implementation is still broader than the seeder coverage. Much of the economy is therefore:

- runtime-ready
- builder-editable
- persistable
- only partially packaged into stock world-generation content

## Seeder Opportunity Matrix
The classifications below are conservative. "Easy" means the current implementation already provides enough structure that seeding the content would be mainly packaging and dependency work, not a new product design problem.

| Candidate | Classification | Why it is seed-friendly or not |
| --- | --- | --- |
| Additional currency packs and pattern packs | Easy | The runtime model is mature, persistence exists, `CurrencySeeder` already establishes the pattern, and currencies are mostly self-contained content |
| Economic-zone templates | Easy | Economic zones already have a stable constructor and persistence model, and their core fields are clear even if world-specific cells still need follow-up tuning |
| Stock tax presets | Easy | Tax creation is factory-driven, stock tax types already exist, and common presets can be seeded as content without changing runtime architecture |
| Bank templates | Easy | Banks already have stable persistence, builder support, and explicit child collections for branches, rates, reserves, and account types |
| Bank account type templates | Easy | Account types have a stable constructor, clear persistence model, and mature builder-facing configuration for fees, interest, and permission progs |
| Stock market categories | Easy | Categories are light-weight, persisted, and intended as reusable classification content |
| Stock market influence templates | Easy | Templates already exist as a separate persisted concept and are a natural seed-data candidate |
| Stock market populations | Easy | The runtime model is already data-driven, with persisted need and stress definitions plus optional progs |
| Stock shopper templates, especially `SimpleShopper` | Easy | Shopper loaders are registered by type, shopper definitions are persisted, and `SimpleShopper` already externalizes its behavior into configured progs |
| Markets tied to seeded economic zones | Easy | A stock baseline now exists through `EconomySeeder`, although builders still need to decide how many zones and markets their live world should ultimately keep |
| Jobs tied to seeded employers or clans | Possible | Runtime support exists, but good stock jobs require seeded institutions, currencies, and reusable eligibility progs |
| Auction houses | Possible | The runtime is ready, but auction houses depend on chosen cells and settlement accounts, so they are best seeded only once a world layout exists |
| Shops | Poor candidate without more design work | The runtime exists, but meaningful shop content depends on cells, stockrooms, tills, merchandise selection, item prototypes, payment items, and world-specific retail design |
| Properties | Poor candidate without more design work | Property is location-specific, owner-specific, and strongly coupled to the world's map and institutions |
| Estates and morgues | Poor candidate without more design work | The runtime now exists, but setup still depends on world-specific cells, clans, legal authorities, auction houses, and ownership expectations |

## Easy Seeder Candidates in More Detail
### Additional currency packs and pattern packs
This is the clearest economy seeding win because:

- a dedicated seeder already exists
- the data model is explicit and content-heavy rather than behavior-heavy
- currencies are useful in almost every world
- additive install behavior is already normal here

### Economic-zone templates
This is a good candidate for stock presets rather than one canonical answer.

Examples of feasible seed content from the current implementation:

- a no-tax frontier zone
- a simple retail-tax zone
- a zone with short financial periods for fast-moving economies
- a clan-controlled zone template with sane defaults

Why it is seed-friendly now:

- the core state is persisted already
- the constructor already establishes a financial period
- the zone's policy role is clear
- estates can now be explicitly enabled or disabled per zone as part of that policy shell
- cells can be left for builder customization even if the zone shell is seeded

### Stock tax presets
Current runtime support already distinguishes tax family from tax content.

That makes it straightforward to seed examples such as:

- flat retail tax
- VAT-style tax
- gross-profit tax
- net-profit tax

Why it is seed-friendly now:

- tax types are factory-registered
- applicability is already prog-gated where needed
- taxes are naturally attached to economic zones as content

### Bank templates and account-type templates
These are practical seeding candidates because the runtime model is already explicit about what a bank needs and what an account type controls.

Feasible stock examples:

- high-street bank
- clan treasury bank
- merchant credit bank
- basic checking account
- savings account
- high-fee mercantile account

Why they are seed-friendly now:

- stable constructors exist
- builder-facing abstractions already exist
- persistence is complete
- account-type policy is already data-driven

### Stock market categories, influence templates, populations, and shoppers
The market and shopper subsystems are also good seeder targets because they are already designed as configurable data plus progs.

Feasible seed content:

- broad food, seasonings, medicine, writing-material, luxury, industrial, military, logistics, and raw-material categories
- event-style influence templates such as harvest failure, bumper harvest, embargo, caravan surplus, piracy, mining trouble, and war mobilisation
- sample populations representing commoners, merchants, martial households, priestly households, monastic households, literate middling households in later eras, and elites across multiple historical eras
- reusable `SimpleShopper` templates seeded as live stock shoppers with scale-adjusted budgets

Why they are seed-friendly now:

- categories and templates are reusable content by design
- market population needs and stress thresholds are already serialized data
- shopper behavior is explicitly configured through progs rather than baked into code paths

Current stock package limits:

- the seeder creates a template market, not a complete retail economy
- the seeded shopper progs are intentionally broad and tag-driven rather than world-specific retail logic
- the seeded populations are builder-friendly archetypes, not a claim of historical simulation completeness
- seeded money values are intended as builder-facing baselines, not audited historical wage tables; the seeder now normalizes them against era and currency assumptions so stock packages start closer to plausible local price scales

## Possible but World-Dependent Seeder Candidates
### Markets tied to seeded economic zones
The stock template now proves this path is technically straightforward, but live-world adoption still needs a world-level decision about:

- how many economic zones exist
- what categories matter
- what price multipliers feel appropriate

### Jobs tied to seeded employers
Jobs could be seeded effectively if they are bundled with:

- seeded clans or employers
- stock eligibility progs
- default pay currencies

Without that surrounding content, seeded jobs risk becoming disconnected examples rather than useful stock data.

### Auction houses
Auction houses need:

- a real cell
- a real settlement account
- a clear place in the world's commercial geography

So they are practical only once the seeding workflow can reference world-specific cells and accounts safely.

## Poor Candidates Without More Design Work
### Shops
Shops are feature-rich, but good stock shops are not just a matter of creating records.

A useful shop needs:

- a location strategy
- stockrooms and tills
- chosen item prototypes
- merchandise setup
- payment-item policy
- staffing expectations
- optional market linkage

This is all possible, but it stops being "easy seeding" and starts becoming opinionated world construction.

### Properties
Properties are even more world-specific because the seeded content would need to answer:

- which cells belong to which property
- who owns them at install time
- what is for sale
- what is for lease
- how keys and access are distributed

### Estates and Morgues
Estates and morgues remain a poor seeder candidate, but for a different reason than before: the runtime exists and is operational, yet the configuration is highly institutional and map-specific.

Seed content cannot safely guess:

- which economic zones should expose probate offices
- where morgue offices and storage rooms belong on the map
- which legal authorities should answer corpse-recovery reports
- what clans, auction houses, heirs, and NPC-estate FutureProg rules should sit behind local probate practice

## Major Gaps and Priorities
### Estates are no longer blocked, but still not stock-seed friendly
The current gap is builder packaging, not subsystem existence.

The key blocker is not claim timing or payout mechanics. It is deciding what counts as a deceased character's property with enough fidelity that the game does not produce bad outcomes in ordinary MUD play.

Important adjacent implications:

- clan or organisational ownership of issued items
- crime-system theft detection and property assumptions
- minimizing false positives when players move, borrow, stash, share, or handle items in expected ways

The subsystem is now live enough for ordinary runtime use:

- estate creation can be disabled per economic zone when a world does not want probate at all
- deaths with no captured assets no longer create empty estates
- guests are always excluded from estate creation, and NPC estate creation is now controlled by a game-wide static prog setting
- asset valuation now follows current sale state, bank balance, and local market price inference
- claims can target individual estate assets and can sometimes be resolved by in-kind transfer instead of liquidation
- living characters can create wills in advance, pre-approve bequests, and later collect deferred estate cash payouts from a probate office
- corpse recovery can still store bodies in a morgue even when no estate is created

The remaining seeder problem is not runtime viability. It is that probate and morgue setup still depend on world-specific cells, institutions, legal authorities, and auction-house choices.

### Economic-zone calendar reassignment is currently broken
The current builder path for changing the economic-zone calendar sets up the accept flow and then throws `NotImplementedException`.

That makes this a concrete runtime defect in an exposed admin workflow rather than a theoretical gap.

### Shop deals and volume pricing are placeholder-only
The current shop runtime exposes deal-related surfaces, but:

- `ShowDeals()` currently reports "Coming soon."
- price methods contain explicit `TODO` markers for volume deals
- detailed price info currently always reports no live volume deals

This matters because the API and UI shape imply a capability that the runtime does not yet actually provide.

### Seeder coverage is much narrower than runtime coverage
The codebase still contains more economy runtime than stock seeding. That gap matters because:

- builders must hand-author much of the economy even in otherwise seeded worlds
- new economy-dependent features cannot assume stock world support
- documentation and seeding strategy need to stay aligned so this does not look more complete than it is

### Automated test coverage is extremely thin
The current economy test coverage is concentrated in `ShopTests.cs`.

There is little or no automated coverage for:

- currency parsing and description behavior
- bank fees, account permissions, and transfers
- tax calculation and financial-period rollover
- markets, populations, influences, and shoppers
- property workflows
- auctions
- job lifecycle behavior

This is a high-priority quality gap because the economy subsystem is heavily stateful and cross-cutting.

### Currency description-pattern persistence needs review
`CurrencyDescriptionPattern.Save()` currently leaves a `TODO` marker after saving the parent record fields.

That does not by itself prove data loss, but it does mark a persistence path that should be reviewed before treating the currency pattern editing surface as fully mature.

## Recommended Documentation Posture
This is inferred guidance for future maintainers.

When extending the seeder side of the economy:

- prefer packaging mature, data-driven economy objects first
- keep additive and repairable behavior for stock-owned economy packages whenever stable names make that safe
- avoid seeding world-specific retail or property content until the seeder has a clean story for cells, items, and institutions
- treat probate and morgue seeding as a world-integration problem rather than a missing runtime feature
- keep the economy design docs updated whenever new seeder support changes the practical setup path
