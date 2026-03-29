# Estate Probate and Item Ownership

## Scope

This document describes the current runtime behaviour for estate probate, item ownership, and the connected command surface that was added to the economy subsystem.

The implementation is intentionally zone-local. A deceased character may generate multiple estates, with one estate per economic zone that contains relevant assets.

## Probate Lifecycle

- When a character dies, the death path only creates estates for economic zones that have estates enabled and that actually receive captured assets.
- A separate estate is created for each economic zone that receives captured assets.
- If the same character dies again before an earlier estate in that zone is finalised or cancelled, the existing open estate is reused instead of creating a duplicate probate case.
- If a death produces no captured assets in a zone, no estate is created for that zone.
- Estates begin in `Undiscovered`.
- After the owning economic zone's `EstateDefaultDiscoverTime` expires, the estate moves to `ClaimPhase`.
- Administrators or authorised zone managers can move an `Undiscovered` estate directly into `ClaimPhase` with `estate open <id>`.
- Morgue intake opens probate immediately rather than waiting for the discovery timer, but only when an estate actually exists for the deceased in that zone.
- After the owning economic zone's `EstateClaimPeriodLength` expires:
  - estates with no approved claims finalise directly;
  - estates with approved claims only move to `Liquidating` if at least one approved claim actually requires liquidation.
- Administrators and authorised zone managers can move an estate into `Liquidating` manually with `estate liquidate <id>`.
- Liquidating estates do not finalise until their estate auction lots have completed and there are no active or unclaimed liquidation lots remaining.
- Sold-but-unclaimed estate item lots do not block estate finalisation once the sale has completed and proceeds have been recorded.
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

Assumed estate values are now derived per asset type:

- properties use the active listed sale reserve price when they are currently for sale, otherwise their last sold price;
- bank accounts use their current balance;
- items use a three-step fallback:
  - the prototype's inherent value when it is set;
  - the average sale price for matching merchandise in shops in the same economic zone;
  - the average sale price for matching vending-machine selections in the same economic zone;
- if no item value can be derived, it still falls back to zero.

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

Claims may now either be generic cash claims or targeted asset claims:

- targeted claims store a direct reference to an individual estate property, bank account, or game item;
- targeted claim amounts default to the asset's current assumed value;
- only one approved targeted claim may exist for a given asset at a time;
- targeted claims whose asset has already been transferred, liquidated, or otherwise disappeared can no longer be approved.

## Finalisation

The current finalisation flow behaves as follows:

- If there are no approved claims, estate assets are transferred in kind to the nominated heir.
- If no heir is nominated, the controlling clan of the economic zone is used as the fallback inheritor.
- If there are approved claims that require liquidation, the estate enters `Liquidating`.
- Approved targeted non-secured claims can now be satisfied by transferring the claimed asset in kind instead of forcing liquidation of the whole estate.
- Positive bank balances are withdrawn immediately into the estate liquidation pool and the underlying bank accounts are closed.
- Non-cash assets are listed on the economic zone's configured probate auction house.
- Administrators and controlling-clan members with `CanManageEstates` can manually list or relist liquidation assets with custom reserve and buyout prices.
- If an auction lot sells:
  - sold item lots transfer item ownership to the winning bidder when claimed;
  - sold property lots transfer ownership immediately through a direct ownership-transfer path that does not require a property sale order;
  - sale proceeds are routed automatically into the estate liquidation pool.
- If an auction lot fails to sell, the asset remains with the estate and is eligible for manual relisting or in-kind transfer when liquidation closes.
- When liquidation closes, the estate computes available distributable cash from liquidated assets.
- Approved claims are paid in secured-first order up to the available distributable cash.
- Secured bank-account claims are settled against the underlying bank account debt rather than being routed to generic zone revenue.
- Approved targeted claims that are paid during liquidation are capped to the value realised for the targeted asset where possible.
- Any residual amount is routed to the inheritor where a suitable bank account exists, otherwise it falls back to economic-zone revenue.
- Assets that have not been liquidated are still transferred in kind at finalisation.

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

Ownership inspection is privacy-aware for ordinary players:

- administrators can still see exact owners;
- players can see themselves and clan owners explicitly;
- other character or system owners are intentionally obscured as `someone`.

## Command Surface

### `heir`

Players can manage their nominated heir:

- `heir`
- `heir <character>`
- `heir clan <clan>`
- `heir clear`

Additional runtime notes:

- `heir` with no further arguments reports the currently nominated heir, if any;
- character heirs must be nominated through normal present-room targeting rather than by global ID or offline-name lookup;
- clan heirs still use normal clan lookup.

### `ownership`

Players and staff can inspect and manage item ownership:

- `ownership` or `own` shows visible items on the character and in the room, including visible nested contents, and respects closed opaque container visibility rules
- `ownership show <item>`
- `ownership claim <item>` for currently unowned items only
- `ownership claim deep [<held item>]` claims all held items, or one held item, plus contained items recursively when they are currently unowned
- `ownership clan <clan> <item>` for clan-managed public property
- `ownership clear <item>` admin only
- `ownership set character <character> <item>` admin only
- `ownership set clan <clan> <item>` admin only

When ordinary players inspect a specific item or list ownership, exact owners are only shown for themselves and clans they can identify; other owners are reported generically as `someone` unless the viewer is an administrator.

### `estate`

Probate interaction is provided through:

- `estate list`
- `estate show <id>`
- `estate claim <id> <amount> <reason>`
- `estate claim <id> asset <asset> <reason>`
- `estate approve <estate> <claim> [reason]`
- `estate reject <estate> <claim> <reason>`
- `estate listasset <estate> <asset> [<reserve>] [<buyout>]`
- `estate relist <estate> <asset> [<reserve>] [<buyout>]`
- `estate open <id>`
- `estate liquidate <id>`
- `estate finalise <id>`

`estate show` now also surfaces liquidation progress, active liquidation lots, unclaimed liquidation lots, completed liquidation results, and a visible error if the economic zone has no probate auction house configured.

All non-admin player use of the `estate` command now requires standing in a probate office configured on the relevant economic zone. LOOK output in those cells includes a reminder that the `ESTATE` command is available there. Administrators and estate managers remain exempt from the location restriction.

## Economic Zone Configuration

Economic zones now persist and expose probate timings through builder commands:

- `estates`
- `probate`
- `morgueoffice <here|none>`
- `morguestorage <here|none>`
- `estatediscovery <time>`
- `estateclaimperiod <time>`
- `estateauctionhouse <which>|none`

`estates` toggles whether the zone creates new estates at all. When disabled, deaths in the zone do not open new probate cases unless an existing open estate is already being reused.

Those values are shown in the economic zone's `show` output and are saved with the zone.

## Morgues and Corpse Recovery

Economic zones can now define both:

- a morgue office cell for player interaction;
- a morgue storage cell for corpse and belongings custody.

Players report bodies with `report body <corpse>`. The local legal authority for the corpse's zone creates a corpse-recovery job and dispatches a patrol when resources are available. On successful pickup:

- the corpse is moved into the economic zone's morgue storage room;
- a configurable static-string emote is echoed by the recovering enforcer;
- the patrol concludes immediately after the pickup is complete;
- possessions are stripped from the corpse and packed into a belongings bundle only if an estate actually exists;
- probate is opened immediately for the deceased's estate in that zone when one exists;
- if no estate exists, the corpse is still stored in the morgue and belongings remain on the corpse.

The public command surface is `morgue`, and it only works from a morgue office:

- `morgue list`
- `morgue claim corpse <which>`
- `morgue claim item <which>`

Claiming a corpse is limited to administrators, estate managers, and the direct inheritor. Claiming items from morgue bundles is limited to items already marked as owned by the claiming character.

## Persistence

The following persistence changes back the feature:

- `GameItem` now stores a generic owner reference.
- `BankAccount` now stores a generic framework-item owner reference alongside legacy owner columns.
- `Character` now stores a generic estate heir reference.
- `EconomicZone` now stores estate discovery and claim timing values, probate office cells, morgue office/storage cells, and a default probate auction house reference.
- `EconomicZone` now also stores whether estates are enabled for the zone.
- New tables/models exist for `Estate`, `EstateAsset`, and `EstateClaim`.
- New tables/models exist for corpse-recovery jobs and probate office locations.
- The migration backfills generic bank-account ownership from legacy character, clan, and shop owner columns.

## Auction Integration

Auction integration is now estate-aware:

- auction lots can represent either items or properties;
- auction seller identity and payout targets are generic framework-item references rather than character-only references;
- seller proceeds that cannot be paid immediately are persisted as owed balances and retried automatically by the auction house;
- auction XML persists both the new generic seller/asset metadata and legacy item/character attributes for compatibility with existing saved definitions;
- estate liquidation uses the economic zone's probate auction house for automatic liquidation listings;
- ordinary fixed-price property sales still continue to use `PropertySaleOrder` rather than the auction-house subsystem.

## Property Key Reclaiming

Property key reclamation now detaches returned key items from any prior inventory, container, or room location before they are handed back to the claimant or grouped into a bundle. This prevents stale containment references from surviving across saves and triggering duplicate-item protection on reload.
