---
slug: hospitality
category: Economy
summary: Build a roadside inn with rentable rooms, a working restaurant and mount boarding. Configure property keys, menus, kitchen staff, stable tickets and the three service ledgers.
---

# Building hotels, restaurants and stables

At Alderbridge's Wayfarer Inn, a courier can leave a horse in the yard, order supper in the common room and rent a bed upstairs. The **hotel** runs short stays on a licensed property. The **restaurant** is a shop with a menu, tables and a service queue. The **stable** boards NPC mounts at a single room and issues tickets for their return. The innkeeper can run all three, and each service tracks its own charges and settlement.

## Lay out the Wayfarer Inn

| Room | What to build there |
| --- | --- |
| Common room | A reception counter, the restaurant's first service room and a real oak table with seats. |
| Kitchen | A usable hearth, storage for ingredients and tools, a rack for reusable plates and shelves for takeaway packaging. Give staff a clear route back to the tables. |
| Landing | A route from the common room to the bedrooms. |
| River Room and Willow Room | Two private guest rooms with individual locks, keys and furnishings. |
| Stable yard | A service point off the road with space to bring in and collect a mount. Connect it to the common room. |

Put all six rooms in one property using Alderbridge's economic zone. The zone supplies the currency, calendar and applicable tax rules. The examples use crowns such as `5cr`. Substitute your zone's amount format and use real room, prototype and account references in place of angle-bracket fields.

As an administrator in the common room, create the property with an account owned by the intended innkeeper. The account establishes initial ownership. You can also start with an existing inn property. To link each additional room, stand in it and run `property set room` once while editing the inn. Check the room list before repeating that toggle.

```text
property edit new "Wayfarer Inn" <zone-id> 600cr <innkeeper-bankcode:account>
```

As an administrator in the common room, create the restaurant and close it while setting up.

```text
restaurant create "Wayfarer Inn Dining Room" <zone-id>
shop close "Wayfarer Inn Dining Room"
```

Move to the yard, create the stable and close it until its fees are ready. Use `none` for the stable's virtual reserve or supply a bank account in the zone currency.

```text
stable create "Wayfarer Inn Stables" <zone-id> none
stable close
```

The innkeeper then goes to the zone's conveyancing office and runs `property claimshops "Wayfarer Inn"`. This assigns the restaurant shop and stable to the innkeeper. Do this before hiring staff, since claiming can replace the existing roster. On leased premises the authorised leaseholder can claim these businesses. Use the authorised property owner for the hotel licence and room configuration. [Room building](Room_Building_Builder_Guide.md) · [Property and conveyancing](Clan_Builder_Guide.md#own-the-town-hall-and-lease-out-a-workshop)

## Set up the guestrooms

### Register rooms, keys and furnishings

As the innkeeper, stand in the River Room and add it at `20cr` per day with a `15cr` security deposit and a one- to seven-day stay.

```text
roomrent room add "Wayfarer Inn" 20cr 15cr 1d 7d River Room
```

Then stand in the Willow Room and add it at `15cr` per day with a `10cr` deposit.

```text
roomrent room add "Wayfarer Inn" 15cr 10cr 1d 7d Willow Room
```

Fit each bedroom door with a lock and pair it with its own matching key. If needed, a locksmith can use `locksmith pair <lock> <key>` or the `<parent> <lock> <key>` form for a lock attached to an item or door. Check each key opens its intended room before registering it. At the zone's conveyancing office, hold each key when you register it as a property key.

```text
property addkey "Wayfarer Inn" <held-river-key> "River Room Key" 5cr
property addkey "Wayfarer Inn" <held-willow-key> "Willow Room Key" 5cr
roomrent key add "Wayfarer Inn" "River Room" River Room Key
roomrent key add "Wayfarer Inn" "Willow Room" Willow Room Key
```

The property entry records the physical key and replacement charge. Assigning it to the guestroom tells the hotel to issue it at check-in. The physical lock controls access. Give each room a different key so the inn can let both rooms independently.

Place a bed and washbasin in the River Room. From inside the room, mark the actual items that the inn expects to find at checkout. Repeat for the Willow Room. The optional value is the replacement charge in the zone currency.

```text
roomrent furnish add here here bed 18cr
roomrent furnish add here here washbasin 8cr
roomrent furnish list here here
roomrent room list "Wayfarer Inn"
```

A new room is listed by default. Use `roomrent room listed "Wayfarer Inn" "River Room"` to take it off the market while redecorating, then use the same toggle to list it again. Use `roomrent room price`, `roomrent room deposit` or `roomrent room length` with the property and room to adjust the offer. [Building items and equipment](Item_Authoring_Guidelines.md)

### License the hotel and set up the reserve

The property owner runs `roomrent request "Wayfarer Inn"`. An administrator or an officer with the controlling clan's economic-zone management privilege then runs `roomrent approve "Wayfarer Inn"`. The innkeeper can inspect the hotel with `roomrent show "Wayfarer Inn"`.

The hotel can handle lodging proceeds and deposits through a virtual reserve. Use `roomrent ledger "Wayfarer Inn"` to review it and `roomrent deposit "Wayfarer Inn" <amount>` to add held cash. Keep funds available for deposit refunds. If you want the hotel to use a bank account, link one in the zone currency that the innkeeper is authorised to use with `roomrent bank "Wayfarer Inn" <bankcode:account>`. Use `none` to clear it.

If Alderbridge taxes lodging, its authorised council officer can create a rental tax. This example charges 5% on room rent. Use the zone's existing tax rules if they already cover lodging.

```text
clan economiczone <zone-id> hoteltax add "Lodging Duty" hotelpercent
clan economiczone <zone-id> hoteltax "Lodging Duty" rate 5%
```

Under that rule, a two-day River Room stay costs `40cr` rent, `15cr` deposit and `2cr` lodging tax. Rental charges round up to whole days, with at least one day billed. The innkeeper remits the collected tax with `roomrent taxes "Wayfarer Inn"`. The restaurant's sales taxes follow the zone's shop-tax rules. [Economic zone taxes and revenue](Clan_Builder_Guide.md#fund-the-town-from-an-economic-zone)

For a seasonal restriction, bind a Boolean FutureProg with `roomrent prog "Wayfarer Inn" <prog>`. It can take the guest as a single Character argument or take no arguments. Use `none` to clear it. Use `roomrent ban` and `roomrent unban` for a particular patron. Outstanding hotel debt can also prevent a fresh stay. [Writing FutureProgs](FutureProg_Builder_Guide.md)

### Check in, check out and handle lost property

Put the check-in syntax in the common-room description or on an inn sign. A guest can pay in cash or append a same-currency authorised account in `bankcode:account` form.

```text
roomrent list "Wayfarer Inn"
roomrent rent "Wayfarer Inn" "River Room" 2d
```

The courier receives the registered key. Before checking out, they take their belongings and hold the issued key so checkout can return it automatically.

```text
roomrent checkout "Wayfarer Inn" "River Room"
```

Checkout deducts charges for missing keys and missing or damaged furnishings. If the deductions exceed the deposit, the guest can pay the balance with `roomrent pay "Wayfarer Inn"`. If a key goes missing, check whether the lock needs to be re-keyed. A replacement charge settles the bill, while physical access still depends on the lock.

Guests can recover an unpaid refund or held belongings with `roomrent claim "Wayfarer Inn"`. The innkeeper can set the holding period with `roomrent retention "Wayfarer Inn" <timespan>` and inspect or extend individual bundles with `roomrent lost list "Wayfarer Inn"` and `roomrent lost extend "Wayfarer Inn" <bundle-id> <timespan>`. After the holding period expires, the hotel can auction or liquidate unclaimed property. Communicate the period to guests and show them where to make a claim.

## Turn the common room into a restaurant

### Set up the dining room and kitchen

Creating the restaurant registered the common room for service. From that room, add the kitchen and register the table.

```text
restaurant room add kitchen <kitchen-room-id>
restaurant table add oak
```

Place the oak table in the common room before registering it. It needs both `Table` and `Container` components. Its container holds the food that staff bring, so give it usable capacity. Use `restaurant room add service <room>` for another dining area or `restaurant room add internal <room>` for a washroom where guests should keep their table participation. The inn keeps its bedrooms and yard outside the restaurant.

The restaurant uses ordinary shop finance. It can use virtual cash, or you can place a `CashRegister` item and designate it with `shop till <register>`. Give the restaurant its own bank account if you want bank-backed checkout. [Shops, cash and banking](Shop_and_Banking_Builder_Guide.md)

From the kitchen, assign actual containers to hold ingredients, tools, reusable plates, takeaway boxes and outer bags. Put stock into those containers and give workers access to them. One container can serve more than one role.

```text
restaurant storage add ingredients pantry
restaurant storage add tools toolrack
restaurant storage add servingware platerack
restaurant storage add takeawaycontainers boxshelf
restaurant storage add takeawaybags bagbasket
restaurant set takeawaybag <bag-prototype>
```

### Offer bread and cook pies to order

Use current item prototypes for the bread and the rabbit hand pie. Give the cook a valid craft that produces the same pie prototype sold on the menu. For example, it uses flour, rabbit meat and a hearth. The merchandise profile supplies the price. Stock the bread as a physical item and put recipe ingredients in the kitchen pantry for the pie. [Authoring crafts](Crafting_System_Builder_Workflows.md) · [Stocking existing items](Shop_and_Banking_Builder_Guide.md#stock-existing-goods)

```text
shop merch new bread <bread-prototype> 3cr
shop merch close
shop merch new pie <rabbit-pie-prototype> 8cr
shop merch close
shop stock bread
restaurant menu add bread
restaurant menu add pie
restaurant menu show pie
```

Menu entries start active with dine-in and takeaway enabled. The bread can be served unchanged. Toggle the pie inactive while setting up its fulfilment. In `plate` mode the cook makes the pie and puts it in a reusable serving container. For takeaway, the inn uses the configured inner box for the pie and the restaurant-wide outer bag. Choose prototypes with enough capacity for each item they need to hold.

```text
restaurant menu set pie active
restaurant menu set pie mode plate
restaurant menu set pie craft <rabbit-pie-craft-id>
restaurant menu set pie plate <serving-plate-prototype>
restaurant menu set pie package <pie-box-prototype>
restaurant menu set pie prep 3m
restaurant menu set pie desc A hot rabbit hand pie with roots and thyme.
restaurant menu set pie active
restaurant menu show pie
```

Use `unaltered` for stock served as it is, `open` for stock with an openable component, `craft` to make a dish without requiring reusable plating, and `package` for packing a finished item for takeaway. Settle any existing orders and deactivate an entry before changing its fulfilment. Use the `dinein` and `takeaway` toggles to restrict a dish to one service. The prep setting estimates the wait. The cook still needs the actual recipe inputs, tools and time.

### Serve cider by the mug

The Wayfarer sells a `250ml` serving of cider for `2cr`. Use a holdable mug prototype with a `LiquidContainer` component, capacity of at least `250ml` and no default liquid. The cider merchandise profile points to the mug prototype. Each order counts filled mugs, so `order cider 2` asks for two servings. The innkeeper keeps this drink dine-in only so the mugs can be collected from the tables. [Building items and equipment](Item_Authoring_Guidelines.md)

```text
shop merch new cider <cider-mug-prototype> 2cr
shop merch close
restaurant menu add cider
restaurant menu set cider active
restaurant menu set cider mode unaltered
restaurant menu set cider takeaway
restaurant menu set cider desc A quarter-litre of orchard cider in a stoneware mug.
restaurant menu set cider active
restaurant menu show cider
```

In the kitchen, hold an open jug of cider and place an empty, open mug within reach. Pour a measure, then register that filled mug as stock. Repeat for each serving and keep the filled mugs in the kitchen until the server needs them. Stock only filled mugs because the menu uses the merchandise count for availability. Each order takes a stocked mug and serves it with its contents.

```text
pour 250ml jug into mug
shop stock mug cider
```

The mug is delivered as the order's product. Have staff collect, rinse and refill it by hand after the guest leaves, then stock it again. `restaurant service clear` handles tracked serving containers such as the pie plate. For a drink prepared to order, a `liquid` craft product can produce the matching mug filled to a set volume. Bind that craft in `craft` mode and give the recipe its own inputs to consume supplies. [Authoring crafts](Crafting_System_Builder_Workflows.md)

### Hire the cook and server

With the staff present, the innkeeper uses `restaurant chef <cook>` and `restaurant server <server>` to create their contracts. Staff with active contracts can work through the queue during their shifts. The cook runs the regular pie craft and supplies its finished output to preparation. The server brings the ready dish and clears empty plates. For bread, preparation draws from stock without a craft.

```text
restaurant service list
restaurant service prepare <order-id> with <finished-pie>
restaurant service serve <order-id>
restaurant service clear oak
```

For NPC service, give the cook and server `EmploymentWorker` AI. Give the cook `CanCraft` and the server `CanDeliverItems`, plus active contracts for the restaurant. Keep the kitchen and tables accessible and give the cook the skills and equipment the recipe calls for. The server carries the order to the table and returns empty plates to kitchen storage. Leave the restaurant's `automation` and `simulate` settings off for the inn's staffed service. Those administrator toggles enable built-in order fulfilment and its optional simulated craft fallback. When an order waits, `restaurant service list` shows whether it is queued, being prepared or ready.

### Send an NPC to the market for supplies

The Wayfarer sends its buyer to Alderbridge Market Provisions, a shop in the market square. Give the supplier real sale stock in the restaurant's currency and make sure the buyer can walk there and back. Use the same ingredient prototypes as the pie recipe. For this example the cook uses one bag of flour and one portion of rabbit per pie, both configured as exact-item craft inputs. The supplier also sells the same bread prototype the inn puts on its menu. If the supplier is linked to a pricing market, set your spending caps against its current customer prices, including tax. [Markets, impacts and shoppers](Markets_and_Shoppers_Builder_Guide.md)

**Equip and employ the buyer.** Use a nearby NPC with free hands and capacity to carry the purchases. As an administrator, create a dedicated `EmploymentWorker` AI. A new worker already has `CanDeliverItems` and tasking enabled. Add `CanPurchaseCommodities`, which also covers buying discrete items.

```text
ai edit new employmentworker WayfarerBuyer
ai set host shop
ai set currency <zone-currency>
ai set capability CanPurchaseCommodities
ai show
ai close
ai add WayfarerBuyer <nearby-buyer-npc>
```

Inside the inn, assign the buyer to the restaurant as an employee. Use the NPC's character ID and the resulting contract ID. Inspect the grants before adding any the contract still needs.

```text
shop contracts assign #<buyer-npc-id> employee
shop contracts
shop contracts delegate <buyer-contract-id> show
shop contracts delegate <buyer-contract-id> grant ManageStockRules
shop contracts delegate <buyer-contract-id> grant ApprovePurchases
shop contracts delegate <buyer-contract-id> grant ManageDeliveryRoutes
```

These grants let the buyer reserve restaurant funds, purchase and deliver. The innkeeper who creates the goals also needs authority to create manager goals and approve the work. Fund the restaurant's linked bank account or its virtual cash balance. The buyer uses the restaurant's funds. Keep enough available for the full reservations shown below, plus room for other commitments. [Employment hosts and delegated authority](Employment_Hosts_Builder_Guide.md#delegate-narrowly) · [Link a shop bank account](Shop_and_Banking_Builder_Guide.md#connect-a-shop-bank-account)

**Keep enough ingredients for twelve pies.** In the common room as the innkeeper, set up the goal against the market shop's ID. The example allows up to `30cr` for each ingredient purchase line and `60cr` across the generated plan.

```text
shop goals draft new restaurantingredients Refill the pie pantry
shop goals draft cadence 1h
shop goals draft budget 60cr
shop goals condition restaurantstock 12 from <market-shop-id> max 30cr
shop goals draft show
shop goals draft finalise
```

If the pantry contains eight bags of flour and five portions of rabbit, the goal plans to buy four bags and seven portions. It generates the authorisation, reservation, purchase and delivery steps. The buyer brings the ingredients to the designated kitchen pantry. With several ingredient containers, stock is counted across them and deliveries go to the usable container with the lowest item ID. The target applies to each active craft-backed menu item, so adding another recipe can raise the total needed. Exact items, tagged items and supported commodity inputs can be purchased automatically. Direct liquid inputs and commodity material-tag selectors need a different supply plan.

**Buy finished bread for the menu.** Use a separate goal for items served straight from merchandise stock. This example orders six loaves when fewer than three are in stock. The transfer step registers them as the inn's `bread` merchandise so restaurant orders can draw from them. Use the market shop's bread prototype and a reachable kitchen room.

```text
shop goals draft new craftmaterials Keep the bread shelf stocked
shop goals draft cadence 1h
shop goals draft budget 18cr
shop goals condition stock merch bread below 3
shop goals step authorise 18cr for market bread
shop goals step reserve 18cr for market bread
shop goals step purchase 6 item <bread-prototype> from <market-shop-id> max 18cr
shop goals step release all
shop goals step stocktransfer to self merch bread destination <kitchen-room-id>
shop goals draft show
shop goals draft finalise
```

Finish by opening the dining room with `shop open "Wayfarer Inn Dining Room"`. The goals check stock on their configured cadence. To see why an order has or has not been placed, inspect `shop goals show <goal-id>` and `shop tasks show <task-id>`. Use `shop tasks diagnose` when a task is waiting for a worker. A closed supplier, missing stock, a price above the cap, unavailable funds or a blocked route can stop the purchase. Compare `shop employmentledger` with the supplier's sale, the actual contents of the pantry and `shop merch list` for the bread. If a purchase appears to have gone through before a task stopped, check the existing payment and item custody before retrying. [Spending approvals and recovery](Employment_Hosts_Builder_Guide.md#approve-spending-and-handle-interruptions)

### Seat the courier and settle the bill

The courier browses the menu, takes the oak table and orders supper. Companions can join the same table. The guest who places an order remains liable even if they order it for someone else.

```text
menu
order table oak
order pie
order cider
```

When the server brings supper and sets the mug on the table, the courier can drink from it directly with `drink mug on oak`. Both orders appear on the table bill. The courier settles it in the common room after supper.

```text
bill
bill pay mine
bill close
```

`bill pay all` covers the entire outstanding table balance. `bill split equal` suggests contributions without changing liability. Close the table once the bill is fully paid. Going to the yard or the bedrooms leaves the restaurant and ends table participation, so have guests settle before they leave. For a pie to eat on the road, `order takeaway pie cash` prepays the order. Staff hand it over when it is ready.

## Open the stable yard

### Set boarding charges and permissions

Once the innkeeper has claimed the business at the conveyancer, they set the fees and open the stable from the yard.

```text
stable fee lodge 4cr
stable fee daily 2cr
stable open
stable
```

The lodge fee is paid at drop-off. Daily charges accrue as the zone calendar advances to a new date. A stay crossing midnight can incur a daily fee before a full day has elapsed. Use `stable ledger` for reserve transactions and `stable show <stay-id>` for one stay's charges. To link an account later, the innkeeper uses `stable bank <bankcode:account>`.

For a stable that boards only eligible mounts, bind `stable set can <allow-prog> <denial-prog>`. The Boolean allow prog and optional Text denial prog receive `(customer, mount)` as Character arguments. Individual rates can use `stable fee lodge prog <prog>` or `stable fee daily prog <prog>` with a Number prog taking `(mount, owner)`. Pay attention to the different parameter order. Fixed rates are sufficient for the Wayfarer.

### Lodge and collect the same horse

Before checking in, the courier brings a mountable NPC horse to the yard, dismounts, asks for a quote and lodges it.

```text
stable quote horse
stable lodge horse cash
```

The horse goes into stable custody and leaves the active room. The engine creates a ticket for that exact stay. After checkout, the courier brings it back to the yard and settles the balance. If two daily fees accrued, they pay `4cr` at collection in addition to the `4cr` drop-off fee.

```text
stable redeem ticket cash
```

The same horse returns to the yard and redemption consumes the ticket. If the courier loses it, the manager can find the stay with `stable list active`, verify the claim against `stable show <stay-id>` and use `stable release <stay-id>`. Add `waive` only to forgive the outstanding fees. Ticketless release is a manager exception to the usual settlement process.

### Give a regular caravan credit

A stable credit account extends terms to a regular customer. It is independent of the stable's bank account and restaurant shop credit. Have the caravan quartermaster and a rider present when the innkeeper creates the account and authorises the rider.

```text
stable account create "River Caravan" <quartermaster> 30cr
stable account authorise "River Caravan" <courier> 10cr
```

The courier can now use `stable lodge horse account "River Caravan"` or `stable redeem ticket account "River Caravan"` within their spending authority. The quartermaster can review the balance with `stable accountstatus "River Caravan"` and use `stable payaccount "River Caravan" 10cr cash` to make a payment or prepay. Check both the shared account limit and the rider's authority.

## Run the inn as one place

Show the arrival route on the inn sign. Travellers take their mounts to the yard, book a room at the common-room counter and take supper at a table. Before setting out they settle their meal bill, check out with the room key and redeem their mount. The innkeeper can review the three ledgers with `roomrent ledger "Wayfarer Inn"`, `shop ledger` and `stable ledger`.

The licensed hotel, restaurant shop and stable are separate employment hosts. Use `roomrent` for hotel employment, the restaurant's `shop` employment records for the cook and server, and `stable` for the yard staff. Authorise staff for each venue they operate. Use `roomrent tasks actions` and `stable tasks actions` to see the native work each venue can assign. [Employment host commands and delegated authority](Employment_Hosts_Builder_Guide.md)

| What the innkeeper sees | Where to look |
| --- | --- |
| A bedroom is absent from `roomrent list` | Check the licence and room listing. For a refused booking, check occupancy, duration, bans, the eligibility prog and the guest's balance. |
| The guest has a key but cannot enter | Check the physical lock pairing. The hotel assignment controls which key the guest receives. |
| Diners can see a table but cannot start service | Check that the room is a service room and that the local item is a registered Table and Container. |
| The pie is listed but service has stalled | Inspect the queue, craft and stock. Confirm that workers can reach the kitchen and table, perform the craft and find its inputs and containers. |
| The stable refuses the horse | Bring the NPC mount to the yard, dismount and check that the horse can be mounted and passes the yard's access policy. |
| The courier cannot recover the horse | Check the venue, original ticket and amount owing. Inspect the active stay before issuing a ticketless release. |
