# Estate Probate and Item Ownership

## Scope

This document describes the current runtime behaviour for estate probate, item ownership, and the connected command surface that was added to the economy subsystem.

The implementation is intentionally zone-local. A deceased character may generate multiple estates, with one estate per economic zone that contains relevant assets.

## Probate Lifecycle

- When a character dies, the death path creates estates automatically.
- A separate estate is created for each economic zone that receives captured assets.
- Estates begin in `Undiscovered`.
- After the owning economic zone's `EstateDefaultDiscoverTime` expires, the estate moves to `ClaimPhase`.
- After the owning economic zone's `EstateClaimPeriodLength` expires, the estate is finalised automatically.
- Economic zones now persist both timing values and evaluate estate status progression during their normal calendar day update hook.

## Captured Assets

The current death capture pass collects the following into the relevant zone estate:

- Properties owned by the deceased in that economic zone.
- Bank accounts owned by the deceased whose bank belongs to that economic zone.
- Items on the deceased body when the death occurs in a zone that can be resolved.

Item capture follows conservative ownership rules:

- If an item already has a registered owner and that owner is not the deceased, it is excluded from the estate.
- If an item has no registered owner and is on the deceased, it is added as a presumed-ownership estate asset.
- If an item is known to be owned by the deceased, it is added as a normal estate asset.

## Claims

Claims are now first-class persistent records attached to estates.

Each claim records:

- claimant as a generic framework item reference;
- optional target asset reference;
- amount;
- reason;
- secured flag;
- assessment status and optional status reason;
- claim date.

System-generated secured claims are currently created for:

- negative bank account positions and accrued account fees;
- outstanding line-of-credit balances.

Player claims are submitted with the `estate claim` command during the claim phase. Claims can then be approved or rejected by administrators or members of the economic zone's controlling clan who hold the `CanManageEstates` privilege.

## Finalisation

The current finalisation flow behaves as follows:

- If there are no approved claims, estate assets are transferred in kind to the nominated heir.
- If no heir is nominated, the controlling clan of the economic zone is used as the fallback inheritor.
- If there are approved claims, the estate computes available distributable cash from liquidated assets plus positive bank balances already attached to the estate.
- Approved claims are paid in secured-first order up to the available distributable cash.
- Any residual amount is routed to the inheritor where a suitable bank account exists, otherwise it falls back to economic-zone revenue.
- Assets that have not been liquidated are still transferred in kind at finalisation.

This means the current implementation supports direct transfer and cash claim settlement, but it does not yet perform a full auction-driven liquidation of items or real estate before finalisation.

## Item Ownership

Game items now have durable generic ownership references.

Supported owners currently include:

- characters;
- clans;
- shops;
- estates.

Ownership is stamped automatically in these known flows:

- shop purchases set the buyer as the owner;
- selling items to a shop sets the shop as the owner;
- claiming auction items now sets ownership to the winning bidder, or back to the seller if the item was unsold;
- estate transfers set the inheritor as the owner for transferred items.

## Command Surface

### `heir`

Players can manage their nominated heir:

- `heir`
- `heir <character>`
- `heir clan <clan>`
- `heir clear`

### `ownership`

Players and staff can inspect and manage item ownership:

- `ownership show <item>`
- `ownership claim <item>` for currently unowned items only
- `ownership clan <clan> <item>` for clan-managed public property
- `ownership clear <item>` admin only
- `ownership set character <character> <item>` admin only
- `ownership set clan <clan> <item>` admin only

### `estate`

Probate interaction is provided through:

- `estate list`
- `estate show <id>`
- `estate claim <id> <amount> <reason>`
- `estate approve <estate> <claim> [reason]`
- `estate reject <estate> <claim> <reason>`
- `estate finalise <id>`

## Economic Zone Configuration

Economic zones now persist and expose probate timings through builder commands:

- `estatediscovery <time>`
- `estateclaimperiod <time>`

Those values are shown in the economic zone's `show` output and are saved with the zone.

## Persistence

The following persistence changes back the feature:

- `GameItem` now stores a generic owner reference.
- `BankAccount` now stores a generic framework-item owner reference alongside legacy owner columns.
- `Character` now stores a generic estate heir reference.
- `EconomicZone` now stores estate discovery and claim timing values.
- New tables/models exist for `Estate`, `EstateAsset`, and `EstateClaim`.

## Auction Integration

Auction integration is currently partial:

- auction-claimed items now update their owner correctly;
- estate-specific auction listings and real-estate auction listings are not yet wired into the auction house runtime.

That means the ownership model is auction-aware, but estate liquidation is not yet fully auction-house driven.
