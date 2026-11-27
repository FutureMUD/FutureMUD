---
slug: property-and-conveyancing
category: Economy
summary: Give a city lasting title to its civic buildings, lease its market hall, admit tenants and tradespeople, and transfer property by gift, sale, auction or inheritance.
---

# Property, leasing and conveyancing

Who owns the town hall when the mayor dies? Who can take over its market counter when a merchant's lease ends? Who may enter the public gallery, and who may change the lock on the records office? These are four different decisions. FutureMUD lets the builder draw the boundaries of an estate and lets characters exercise the rights attached to it in play.

Start with [what the property controls](#what-the-property-controls). Then [build Alderbridge Market Hall](#build-alderbridge-market-hall), [lease it to a merchant](#lease-the-hall-to-a-merchant), [admit a clerk and a locksmith](#admit-a-clerk-and-a-locksmith), and [choose how title changes hands](#five-ways-title-can-change-hands).

## What the property controls

| Decision | What to build or assign |
| --- | --- |
| Which rooms change hands together? | A named **property** in an economic zone, with a set of rooms. Use one property for each unit that can be owned or leased independently. A room belongs to one property. |
| Who holds title and receives income? | One or more **ownership shares**, held by characters or clans. Owners can nominate accounts for their revenue. Co-owners consent to offering the whole property for sale or lease. |
| Who operates the place today? | A **lease order** offers terms. A **lease** is the resulting agreement with one character or clan for a term. The owner retains title. The active leaseholder has the exclusive claim to returned keys and attached businesses, and can name individual tenants. |
| Who may come inside? | The property's criminal-code setting, any private-room controller, the visitor's recognised relationships, and local trespass law. A door, a lock and a matching key control physical passage separately. |
| Where does business happen? | A **conveyancing office** handles sales, leases, keys and claims for its economic zone. An **auction house** runs competitive sales. A **probate office** handles estates and wills. One building can house several services. Each role needs its own setup. |

**Use civic title even when you intend to keep the building forever.** Give the city clan its courthouse, records office and market hall. A new mayor with the right clan privilege can administer them without the previous mayor signing over a personal asset. The mayor's death leaves city title intact. Clan-owned property rooms also count as workplaces for the clan employment host. A city can collect rent on the market hall while keeping its courthouse permanently off the market. Creating a property creates neither a sale listing nor a lease offer. [Clans and town government](Clan_Builder_Guide.md) · [Employment hosts](Employment_Hosts_Builder_Guide.md)

Draw the boundary around the rights you want to transfer. If the city lets the market hall as one concession, include its shop counter and stockroom. Give the civic archive its own property if the city must retain control of it. If four shopkeepers should lease stalls independently, give each letting its own property and keep the shared concourse under city management. The lease and the business claim both operate at property scale.

## Build Alderbridge Market Hall

The council already has a clan called `alder`, a clan-owned bank account and an economic zone using crowns. In the commands below, substitute your zone ID, account references and local currency notation for the examples in `cr`. The builder lays out the premises. A councillor with `CanManageClanProperty` acts for the landlord. Mara, a merchant, takes the lease in her own name.

1. **Open the conveyancing office.** Stand at the land registry counter. Run `ez edit <zone-id>` and `ez show <zone-id>`. If this room is absent from the conveyancing list, run `ez set realty`. It toggles the current room. At a working office the room description announces the service, and `properties` shows the local offerings and interests.
2. **Create the title.** Stand in the unassigned public arcade, the first room of the property. Use the council's account. The account's owner becomes the initial owner; putting the mayor's personal account here would put the hall in the mayor's personal estate. The initial value is a valuation in the zone currency. No one pays that amount at creation.
3. **Add the rooms.** Walk to the sales counter and then to the stockroom. At each, use `property set room` once with the hall selected. This toggles membership. Inspect the room list before repeating it. The last room cannot be removed until another has been added.
4. **Decide where the public may go.** A new property's criminal-code setting starts on. For a mixed public and private building, inspect `property show "Market Hall"` and use `property set lawful` once **if the setting is on** to turn off property-wide enforcement. Leave the public arcade and customer counter unmarked. In the stockroom, use `room private property "Market Hall"` and inspect `room private show`. The stockroom now has its own access check, while ordinary shoppers can enter the arcade without needing a property relationship. For a wholly restricted building, keeping property-wide enforcement on is a simpler design.

The title-creation commands for step 2 are:

```text
property edit new "Market Hall" <zone-id> 1200cr <council-bankcode>:<account-number>
property set desc
```

In the editor, describe what an interested tenant is acquiring, for example “The covered arcade includes the east sales counter and its lockable rear stockroom. The council archive across the lane is a separate holding.” Submit with `@` on a line of its own. After the room work, use `property show "Market Hall"` and `property close`.

To make a permanent civic building, repeat the title-creation step from its first room using the same clan account. Stop after assigning rooms and access. A public courtroom can have an open gallery and a private records room. Clan hall, treasury and administration designations still have to be assigned to the rooms that perform those functions. Economic-zone custody, which gives the clan fiscal powers, is another separate decision. [Rooms and private controllers](Room_Building_Builder_Guide.md#example-private-property) · [Justice](Justice_System_Builder_Guide.md)

## Lease the hall to a merchant

At conveyancing, the councillor registers a **real, matching key** while holding it. Registration takes that key into custody. Give it a name the tenant will recognise and a replacement cost the council can justify. The example offers rent of 20 crowns every fourteen days, a bond of 10 crowns, and a term between 28 and 84 days. The two bare numbers in `leaselength` are days in the time-span format used by that editor.

```text
property addkey "Market Hall" key "stockroom key" 2cr
property makelease "Market Hall" 20cr 10cr
property leaselength "Market Hall" 28 84
property approvelease "Market Hall"
property preview "Market Hall"
```

All ownership interests must consent. For an unleased property, return outstanding registered keys before offering it. A property pledged on an active loan cannot take a new lease. If the council wants a different payment frequency, set it on the order before approval, for example `property interval "Market Hall" every 1 weeks`; the quoted rent is then per new interval. Inspect automatic renewal, relisting and rekeying before signing. `property autorenewlease`, `property autorelist` and `property autorekey` each take the property name and toggle a different setting. Rekeying changes physical access at the end of an unrenewed letting.

At that same office, the merchant can see the terms, take a six-week lease and later make a rent payment:

```text
properties
property preview "Market Hall"
property lease "Market Hall" "6 weeks" <merchant-bankcode>:<account-number>
property pay "Market Hall" 20cr <merchant-bankcode>:<account-number>
```

Signing the example costs **50 crowns** up front: the 10-crown bond plus two 20-crown rent periods. Omit the account to use accessible cash. The lease starts on the zone calendar, the registered keys are issued, and the personal lease immediately claims attached businesses. For a clan concession, an authorised officer uses `clan lease <clan> "Market Hall" "6 weeks" [<account>]`. The clan becomes the leaseholder. Arrange who will be its operating representative before taking the lease.

**What “exclusive” means here.** A second character cannot lease the hall while the contract is active. Even the city owner cannot use `property claimkeys` or `property claimshops` over the merchant. The city still owns its share, receives rent, can manage future offers and remains recognised for entry to property-controlled rooms. A listed tenant can enter and a keyholder can operate a matching lock. Neither of those facts gives them the merchant's power to claim the business. Selling the title during the term leaves the lease in place.

**Manage the term.** Use `property preview "Market Hall"` for the lease and payment position. The council can use `property setbank "Market Hall" <account>` for its share and `property revenue "Market Hall" ledger` to inspect virtual property revenue. Cancelling a lease **order** with `property cancellease "Market Hall"` removes the offer. The current contract runs separately. The initial rent goes to the owners; the active lease starts with a payment balance of zero. Later rent payments and charges appear on that balance. An active lease set to renew can roll over when its balance is nonnegative and no bond has been claimed. The current termination command accepts a zero balance as well as a negative one. Check that rent is actually overdue before using it. For a default, review the balance and any damage, lodge a justified `property claimbond "Market Hall" <amount>` while the lease is active, then use `property terminatelease "Market Hall"` and read its confirmation. At departure return issued keys with `property returnkey <held-key>`. After expiry, the outgoing leaseholder uses `property returnbond "Market Hall"`; arrears, damage claims and replacement costs for missing keys can reduce the return.

### Hand over the attached businesses

At the zone's conveyancing office, the eligible operator uses `property claimshops "Market Hall"`. During an active lease that is the leaseholder or an authorised officer of the leaseholding clan. Otherwise it is an authorised owner, including an officer of an owning clan. The claimant is the **character who runs the command**. The command takes in all qualifying businesses, so check the property's rooms and agree who will staff them before signing or claiming.

| Connection to the property | What the claim does |
| --- | --- |
| A shop associated with a property room | Rebuilds the shop employee roster with the claimant as proprietor. A restaurant operated through its permanent shop follows the shop connection. |
| A stable based in a property room | Clears the stable employee roster and appoints the claimant as proprietor and manager. |
| A hospital with any registered location in the property | Grants or retains a proprietor contract for the claimant. Other nonpermanent active or suspended proprietor contracts are ended. Permanent proprietor contracts and ordinary clinical jobs are separate. |
| Hotel rooms, farm fields, clan facilities and keys | These use property in their own ways. There is no hotel, field or clan-office appointment in this claim. Claim returned keys separately with `property claimkeys "Market Hall"`. |

A new personal lease and a personal lease renewal can run the handover automatically. On a whole-title transfer without a lease, a sole individual owner can become the operator automatically. Clan or shared title needs a deliberate choice of representative. Treat a repeat claim as a fresh staffing decision. If the merchant is taking over an existing restaurant, record the intended staff before the handover and reappoint them as needed. [Permanent shops](Shop_and_Banking_Builder_Guide.md) · [Restaurants, hotels and stables](Hospitality_Builder_Guide.md) · [Hospitals](Hospital_Builder_Guide.md)

## Admit a clerk and a locksmith

**The clerk needs regular access.** With clerk Nell present, the merchant enters `property tenant "Market Hall" add Nell`. Use `property tenant "Market Hall" list` to review the register and `property tenant "Market Hall" remove Nell` when the arrangement ends. This recognises Nell in rooms controlled by the property. Give her the stockroom key separately if she must open its door. Declare individual characters. A shop employment contract by itself gives access to a room marked private **for that employment host**. Choose the host as the controller for an employee-only workshop, or the property for a tenant's premises. Each private room has one controller.

**The locksmith needs an afternoon.** Have the merchant and locksmith meet in the public arcade, which belongs to the hall and has no private-room marker. As the merchant, use `permitworkproperty locksmith 4 hours`. This is the property-wide grant an ordinary owner or leaseholder can issue from there. The locksmith can now enter the property-controlled stockroom without the automatic trespass check treating them as an intruder. The permission expires. It creates no lease, business role or key. An active hotel guest similarly has access to their rented room alone through their booking.

Where you stand matters. Inside a marked private room, `permitwork locksmith 4 hours` grants a permit for that room and its controller, which can also be recognised in other rooms marked for the same controller. A manager can do this for an employment-host controller. On an unmarked site an authorised local enforcer can use `permitwork` for the current room. For the hall-wide grant above, use the property form as the ordinary merchant in the unmarked arcade. Trespass depends on the applicable law in the destination's enforcement zone. A character acting lawfully may be stopped before entry. A matched permit settles the access question but does not unlock a door.

### Let the locksmith replace a lock lawfully

Taking a door or lock off its mount asks a second question: does a local **BreakAndEnter** law apply to this action? The uninstall command sends the door or lockable object to the legal authority. A work permit alone does **not** exempt the action from that law. A city that licenses repairs can express its exception in the law's Boolean applicability prog.

For example, Alderbridge allows a currently permitted worker to service the stockroom door **from inside the marked stockroom**. Suppose the stockroom has room ID `4102` and its installed door has live item ID `7305`. Replace both with your installed IDs. For a new policy prog, use `prog edit new AlderbridgeBreakAndEnterApplies`, set the return type to `boolean`, then use `prog set parameter add` for `offender character`, `victim character` and `fixture item`, in that order. Enter this body with `prog set text`:

```text
if (isnull(@fixture))
  return true
end if
if (@offender.location.id == 4102 and @fixture.id == 7305)
  if (privatepropertyaccessreason(@offender.location, @offender) == "A current work permit authorises access.")
    return false
  end if
end if
return true
```

Here `false` means the law does not apply. The access function exposes the reason as text; the example matches the engine's work-permit response specifically. Mere permission to enter as a tenant or ally therefore does not authorise the repair. A door has a different live ID after replacement, so update the designated fixture when the door itself changes. For multiple civic buildings, use a register or a builder-maintained set of fixtures. Keep the room, fixture and permit checks together.

Submit and compile the prog. With the correct authority selected (`legalauthority edit "Alderbridge Bench"`), attach it using `legalauthority set law <break-and-enter-law-id> prog AlderbridgeBreakAndEnterApplies`. If that law already has an applicability prog, incorporate this exception in it and preserve its existing conditions. Check other overlapping laws and authorities too. For the physical work, unlock and open the door with its proper key. From the stockroom side, use `uninstall <exit> <old-lock>`, then `install <held-new-lock> <exit>`. For a whole door use `uninstall <exit>`, then `install <held-new-door> <exit>`. Allow each action to finish. Supply the matching new key and update the registry's keys and replacement costs when needed. [Justice and conditional laws](Justice_System_Builder_Guide.md) · [FutureProg for builders](FutureProg_Builder_Guide.md) · [Building a lockable door](Item_Authoring_Guidelines.md)

## Five ways title can change hands

Keep the civic hall and the merchant's lease in mind, but give the merchant **Mara's Warehouse** as a separate, personally owned property. Below are alternative futures for the warehouse. Choose the one that matches the story. A councillor cannot auction city title as personal wealth, and a leaseholder cannot leave the landlord's building to their children.

### 1. Give a share directly

Mara wants her apprentice Ellis to become a co-owner today. They go together to the zone's conveyancing office. Mara enters `property divest "Mara's Warehouse" 25% Ellis`, reads the proposed share and uses `accept`. No purchase price or market listing is involved. The percentage is **of Mara's existing stake**. If she owns 60% of the warehouse, a 25% divestment gives Ellis 15% of the whole. Use 100% to give the recipient all of the donor's stake. The recipient can then set a revenue account for their interest.

For a gift to the town, the recipient can instead be `*alder`. If title belongs to the town and the council is giving a share to Ellis, its authorised officer uses `clan divest alder "Market Hall" 25% Ellis` at conveyancing. The merchant's lease of the hall is a separate record. Resolve an active mortgage lien before attempting a voluntary transfer.

After a complete divestment, check the ownership display before relying on it to exclude the former owner. The current engine deletes the surrendered ownership record from storage but can keep the old owner authorised in the running world until the property reloads.

### 2. Sell the whole building at a fixed price

Mara and her co-owner want 800 crowns for the warehouse and will accept the first eligible buyer at that price. At conveyancing, Mara uses `property sell "Mara's Warehouse" 800cr`. Each ownership interest consents using `property approvesale "Mara's Warehouse"` through its owner or authorised officer. Inspect the order with `property preview "Mara's Warehouse"`. Return outstanding registered keys before listing a vacant property.

The buyer visits the same zone's office, uses `properties` and `property preview "Mara's Warehouse"`, then enters `property buy "Mara's Warehouse" <buyer-bankcode>:<account-number>`. They can omit the account to pay in cash. The buyer takes the entire title and the vendors receive proceeds in proportion to their shares. A mayor buying for the city uses `clan buy alder "Mara's Warehouse" <clan-bankcode>:<account-number>`; paying from a clan account on a personal purchase is a different decision from naming the clan as buyer. Check any continuing lease and arrange the business handover separately. For a buyer borrowing the purchase price, follow the [mortgage closing example](Lending_Builder_Guide.md).

### 3. Auction one owner's interest

If the city needs an auction house, stand in the auction room and use `auction edit new "Alderbridge Exchange" <zone-id> <bankcode>:<account-number>`. Use `none` in place of the account for a cash-only house. Inspect `auction show "Alderbridge Exchange"` for the house ID, fees and listing time. This same house can handle the voluntary sale below and, once the zone designates it, probate sales.

Mara owns half the warehouse and wants competitive bids for **her half**. At an auction house in the warehouse's zone, she enters the following and confirms the offer. Choose a reserve above the house's flat fee. Here 400 crowns is the reserve and 600 is an optional buyout.

```text
auction sell property "Mara's Warehouse" 400cr cash 600cr
accept
```

Use an active payout account in place of `cash` if preferred. The listing selects the seller's **entire personal share**. A co-owner's interest stays with that co-owner. Check for an existing property sale order, an outstanding auction of the same share, a lien, or unreturned keys on a vacant property before listing.

At the auction house, Ellis uses `auctions`, `auction preview <lot>` and `auction bid <lot> 450cr`. Alternatively, `auction buyout <lot>` takes the available buyout. On a completed sale the winning bidder receives the share directly. There is no deed item for the winner to collect. The house accounts for its fees; Mara can use `auction refund` to collect cash proceeds owed to her. The other half of the warehouse and any active tenancy are still there.

### 4. Sell a deceased owner's property to satisfy creditors

Suppose Mara owned the **whole** warehouse when she died and an approved debt must be settled in cash. Set up the registry before estates begin to arrive. In the relevant zone, the builder uses `ez show <zone-id>` to inspect whether estates are enabled, the discovery delay, claim period, probate rooms and designated probate auction house. With the zone selected using `ez edit <zone-id>`:

- At the probate desk use `ez set probate` if this room is absent from the list. A conveyancing designation by itself does not establish probate.
- Use `ez set estateauctionhouse <auction-house-id>` to select an existing house in the same economic zone. Set this up even if no citizen has yet offered a voluntary auction. An estate can enter liquidation without having a house available to list its assets.
- Use `ez set estates` only if you need to toggle new estate creation. `ez set estatediscovery <time>` and `ez set estateclaimperiod <time>` control how much in-world time officials and creditors have. Give a responsible officer of the zone's controlling clan `CanManageEstates` if probate will be player-run.

At death the engine captures eligible assets by zone. A deceased player's local estate begins undiscovered and opens for claims after the discovery delay. An authorised official can open it earlier with `estate open <estate-id>`. NPC estates require the world's `NpcEstateProg` to admit the deceased NPC. City-clan property never becomes a deceased mayor's personal asset.

At the probate desk a creditor uses `estate list`, `estate show <estate-id>` and, for example, `estate claim <estate-id> 180cr Unpaid warehouse repairs`. An official reviews the evidence and uses `estate approve <estate-id> <claim-id>` or `estate reject <estate-id> <claim-id> <reason>`. At the end of the claim period, an approved claim requiring cash can put the estate into liquidation. An authorised official also has `estate liquidate <estate-id>` to start that process deliberately. A death or a creditor's unapproved request alone does not force a sale.

During liquidation the estate gathers available bank funds and can list eligible assets at its designated auction house. Use `estate show <estate-id>` to distinguish the **estate asset ID** from the **auction lot**. If the warehouse needs a manual listing, an official can use `estate listasset <estate-id> <asset-id> 650cr 900cr`; for an unsold lot use `estate relist <estate-id> <asset-id> 600cr`. Bidders use the ordinary auction commands. The winner receives title, the proceeds go to the estate, and secured claims take priority when available funds are distributed. An unsold lot has no buyer. Pending settlements can keep the estate open. A character owed distributable cash can collect held funds with `estate payout [<estate-id>]` at the probate office.

**The boundary for co-owners matters.** If Mara owned only half the warehouse, probate cannot force that half to auction. Her share passes in kind to the inheritor or an approved targeted claimant. Ellis keeps any share he already owned. A voluntary auction during Mara's lifetime could have sold her half under the rules above. [Loans and secured claims](Lending_Builder_Guide.md) · [Estate and probate reference](../Economy/Estate_Probate_and_Item_Ownership.md)

### 5. Leave the warehouse to an heir or a named beneficiary

Mara wants her remaining assets to go to Ellis, but wants the warehouse itself to become a city granary. While both are present, she uses `heir Ellis` for her general heir. `heir clan alder` instead names the city as general heir, and `heir clear` removes the nomination. General inheritance does not require a separate will for every building.

For the granary, Mara goes to the **probate office in the warehouse's zone**:

```text
estate create
estate list
estate show <will-id>
estate bequeath <will-id> <warehouse-asset-id> clan alder For the municipal granary
```

`estate create` creates or refreshes the living character's local will and its asset snapshot. Use the warehouse's asset ID from that will. A bequest creates a pre-approved **targeted claim**; `estate revoke <will-id> <claim-id>` removes it if plans change. Title stays with Mara during her lifetime. Probate rechecks what she actually owned at death, so a will cannot give away a building she has since sold.

When the estate can settle in kind, the city receives the specified interest and Ellis inherits the residue. A valid targeted bequest does not by itself demand an auction. With no applicable bequest or cash claims, a nominated heir can simply inherit the property directly. The zone's controlling clan is the fallback inheritor when no heir is nominated. With outstanding debts, follow the estate and its claims through probate before promising a particular cash distribution. A city that already owned the hall reaches the same change of mayor without any probate transaction at all.
