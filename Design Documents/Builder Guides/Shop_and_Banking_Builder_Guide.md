---
slug: shops-currencies-and-banking
category: Economy
summary: Configure permanent shops and mobile stalls, apply zone taxes, and choose how to handle cash, ownership and banking.
---

# Shops, currencies and banking

An **economic zone** supplies the currency and tax rules for a shop. The zone determines which sales taxes apply to a purchase and at what rates. It also sets any profit taxes assessed on the shop over a financial period. A **merchandise profile** tells a shop what it sells and how to price it. **Stock** is the actual merchandise available for a customer to buy.

Start with an existing zone and review its currency and taxes together. A new currency needs denomination, coin and amount-parsing rules before you can test checkout.

## Choose a permanent shop or a mobile stall

| Type | When to use it | How it works |
| --- | --- | --- |
| **Permanent shop** | A storefront, workshop or other business with a fixed address | `shop create <name> <zone>` starts the shop in your current room. Use `shop extend` to add adjoining shopfronts, a stockroom or a workshop. Include the premises in a property if you want ownership or leasing to govern who can run the shop. |
| **Mobile stall** | A market trader who sets up in different places | `shop createstall <name> <zone>` creates the business. Use `shop setupstall <stall> <shop>` to link a visible ShopStall item while the stall is closed. Open it in the trading room with `shop open <shop>`. Use `shop close <shop>` before moving the stall. The business's economic zone still governs its currency and taxes after a move. |

Use a stall prototype that can be moved if the trader needs to travel with it. Test opening, closing and relocating the stall with the intended stock before opening it to players. An open stall cannot be picked up.

## Choose how the shop stores money

A shop can hold and spend a **virtual cash balance** in its zone's currency without a physical till or a linked bank account. Managers and administrators can inspect it with `shop info`. A shop loan, for example, can fund that balance. A linked bank account has its own balance and transaction history.

Ordinary player coin checkout puts physical coins into a till or stockroom. For a permanent shop without a till, give it a stockroom to receive those payments. A virtual balance can fund payouts, but it does not give coin receipts a place to go.

Add a physical till if you want coins to be accessible in the shop and theft from the till to be part of play. Prefer a `CashRegister` item, or `LockingCashRegister` if staff should be able to lock access to the contents. Use `comp typehelp cashregister` to inspect the component and `shop till <register-keyword>` to designate the placed register. The same container cannot serve as the shop's display cabinet and till. Check who can open the register and reach its contents in the room.

## Put a permanent shop on a property

For a permanent shop, put the premises inside a property in the same economic zone. The shop is associated with the property through their shared rooms. Open the property's builder editor with `property edit <property>`, stand in a shop room that is missing from the property, and use `property set room` to add it. This command toggles membership, so check the property's rooms before repeating it.

At one of the property's economic zone conveyancing locations, an authorised owner or leaseholder can run `property claimshops <property>`. While a lease is active, the leaseholder has the right to claim. Claiming installs the claimant as proprietor and replaces the current staff roster, so check the associated businesses and arrange any handover before issuing it. The same command also claims associated stables and hospitals.

## Sell a scarf in a permanent shop

Use a disposable retail room that belongs to no shop, an adjoining test room for a stockroom, an approved scarf prototype and a player character with test coins. Have a second character available to act as proprietor for the banking part. Replace every angle-bracket argument with a value from your game.

### Check the zone, taxes and currency

```text
ez list
ez show <test-zone>
currency show <zone-currency>
```

Record the currency, amount format, and sales taxes that apply to the scarf. Check any merchandise tags or other eligibility rules used by the zone's taxes. The merchandise command takes a pre-tax price. Check the final price in the shop listing and give the tester coins for that amount. Review profit taxes separately when you inspect the shop's finances over a period.

### Create the shop and give it a stockroom

As an administrator, stand in the retail room and create the shop.

```text
shop create "Workshop Market" <test-zone>
shop extend <direction-to-stockroom> stockroom
shop info
```

A new permanent shop starts trading. This setup directs the physical coin receipts to the stockroom. To test a one-room shop with a till instead, place a register in the retail room and designate it with `shop till <register-keyword>`.

The new shop has no staff by default. For property-backed ownership, add the premises to the property and have the intended owner or leaseholder claim the business. In a disposable room with no property, use `shop proprietor <character-keyword>` as an administrator and confirm with `accept`. The role command toggles, so inspect the existing role before repeating it.

### Add a merchandise profile and one item in stock

As a builder, create the profile by item prototype ID or unique name. A shop manager without Admin privileges can use a visible or held item to select its prototype when creating a profile. Quote an amount with spaces so the merchandise command treats it as one price.

```text
shop merch new scarf workshop_wool_scarf <pre-tax-price>
shop merch show scarf
shop merch close
item load workshop_wool_scarf
shop stock scarf
shop merch list
```

Use your own Current prototype in place of `workshop_wool_scarf` if you built something else. `shop stock` targets a live item in the room or your hands and marks it as shop-owned merchandise. If the loaded scarf is in your hands, put it where customers or staff should find it. Where two profiles could match, supply the merchandise keyword after the item target. [Building items and equipment](Item_Authoring_Guidelines.md)

### Buy as a player and trace the sale

Before the sale, inspect `shop economy` as an administrator and record the shop's physical cash and stock. Have the customer stand in the trading room with enough of the correct physical coins.

```text
count
list
buy scarf
inventory
count
```

Expect one scarf to leave stock, the purchaser to receive it, and the coins to go into the stockroom or designated register. Using exact coins on the first test separates buying from change-making. Check `shop ledger` as a manager or administrator and `shop economy` as an administrator. Compare the stock, sale, recorded tax and physical cash balance. Repeat with no stock or with insufficient coins to check that the failure gives the customer a useful reason and leaves stock and cash as they were.

## Stock existing goods

### Stock one item or stack

Bring the goods into the shop and create a matching merchandise profile first. An employee or administrator can stock a local or held item. The first argument selects the item and the optional second argument selects its merchandise profile. For a stackable item this stocks the selected stack, including its quantity.

```text
shop stock <item-keyword>
shop stock <item-keyword> <merchandise-keyword>
```

Use the first form when the default profile is correct. Use the second when the same item could match several profiles, such as different finishes or skins. These are alternative commands for an item. `shop dispose <item-keyword>` takes an item out of the shop's stock. Check the final location as well as the stock counts when moving goods.

### Stock a whole delivery and check the count

Place all saleable goods on the shop premises or inside the linked stall, as appropriate. Clear personal goods from those spaces first. As a shop employee, run `shop autostock` to scan for unstocked items that match the merchandise profiles. For a permanent shop the scan includes items inside containers across the shop rooms. For a stall it scans the stall's contents. Review the items the command says it added.

```text
shop autostock
shop merch list
```

`shop merch list` is a manager or administrator view showing each profile's **On Display** and **In Store** counts. Compare both columns with the delivery and use ordinary `list` to confirm what a customer sees. If a match was skipped, stock it explicitly and choose the intended profile.

Administrator use of `shop autostock` also creates replenishment stock up to the configured minimums before scanning existing items. To register a physical delivery without generating replacements, run the bulk command as a regular shop employee or use `shop stock` on individual items as an administrator. The order matters if unmarked stock is already on the premises.

## Reorder automatically after a sale

An administrator can configure auto-reordering on a merchandise profile. For the scarf, use a minimum of five and a reorder cost of 50% of the selling price.

```text
shop merch edit scarf
shop merch set reorder 50% 5
shop merch show scarf
shop merch close
```

You can supply a fixed cost in the shop currency in place of `50%`. Quote a price with spaces. `shop merch set reorder <cost-or-percentage> <minimum-quantity> [<minimum-weight>]` also accepts a weight threshold for goods sold by weight. The minimum quantity is the refill count used when generating replacement items.

Stock the initial five scarves manually or run `shop autostock` as an administrator to fill the configured minimum. Then buy one as a test customer. With reorder enabled, an ordinary sale that reduces stock below the minimum generates replacements from the authored merchandise. Check the counts with `shop merch list` and look for the restock expense in `shop ledger`. This is built-in replenishment and runs independently of the shopkeeper AI.

New copies normally use the merchandise's configured prototype and load arguments. `shop merch set preserve` toggles copying from items just sold when available. Use this deliberately when variants need to carry forward. Check the replacement's appearance or fit and where it landed, especially if the preferred display container is full or the shop is a stall.

Use `shop merch set reorder off` to stop sale-triggered reordering. The saved minimum remains available to an administrator running `shop autostock`. For a shop supplied entirely by players, keep sale-triggered reordering off and use the manual delivery workflow.

## Run a sale or volume discount

Shop managers, proprietors and administrators can configure deals while inside the shop. A sale applies whenever its conditions are met. A volume deal also requires a minimum purchase quantity of at least two. Create deals while the shop is closed to customers. A new deal starts as an active, cumulative 10% discount on all merchandise, so set its target before reopening.

This example gives 15% off scarves for two in-game days.

```text
shop deals new "Workshop Scarf Sale"
shop deals set target merchandise scarf
shop deals set type sale
shop deals set applies sell
shop deals set adjustment -15%
shop deals set expiresin 2 days
shop deals show "Workshop Scarf Sale"
shop deals close
```

`sell` means the shop sells to the customer. `buy` means the shop buys from the customer. For a promotion across several products, use `shop deals set target tag <tag>` to match the merchandise prototypes by tag, or `shop deals set target all` for the whole shop. Use `shop deals set prog <prog>` to add an eligibility rule. The rule returns Boolean and accepts Character, Shop, Merchandise, Number and MudDateTime in that order. [FutureProg for builders](FutureProg_Builder_Guide.md)

For a bulk discount, edit the deal and use `shop deals set type volume 3`. Stock enough scarves to compare `preview 2 scarf` and `preview 3 scarf` after reopening. `deals scarf` shows the active offers, while `preview` shows the selected purchase and its price. The zone calculates applicable sales tax on the adjusted price.

Cumulative adjustments are added together. If several eligible deals are marked non-cumulative, the shop picks the best one for the customer in the direction of trade and adds it to any cumulative deals. Inspect `shop deals show <deal>` before using `shop deals set cumulative`, which toggles stacking. Set expiry deliberately and use `shop deals list` to check the result. To end an offer early, select it with `shop deals edit <deal>`, then run `shop deals delete` and confirm.

## Offer a shop line of credit

A line of credit belongs to one shop and lets authorised characters charge purchases up to an outstanding balance limit. It has its own balance and is managed from the shop. Use the shop manager's account commands in the premises of a permanent shop.

### Give an individual an account

Have the customer in the shop so the manager can target them.

```text
shop account create "Tailor Credit" <customer-keyword> <account-limit>
shop account show "Tailor Credit"
```

The account owner is automatically authorised. Set a limit in the shop currency. Have the customer buy one stocked item and check the balance, then make a cash payment towards what they owe.

```text
buy scarf credit "Tailor Credit"
shop accountstatus "Tailor Credit"
shop payaccount "Tailor Credit" <payment-amount>
shop accountstatus "Tailor Credit"
```

Expect the balance to rise by the purchase price, including applicable sales tax, then fall by the payment amount. The shop needs its normal cash receiving setup for repayments. Test a purchase that would exceed the remaining account limit and confirm it is refused.

### Give a clan's buyers access to one account

Shop credit uses a character as the account owner. Nominate the clan's treasurer or quartermaster as owner and authorise each buyer by character. The name can identify the clan, while spending authority comes from the account's user list.

```text
shop account create "River Guild Supplies" <quartermaster-keyword> <account-limit>
shop account authorise "River Guild Supplies" <purchaser-keyword> <per-purchase-limit>
shop account show "River Guild Supplies"
```

Give each buyer an explicit positive per-purchase limit in the shop currency. Each purchase must fit within that amount and the remaining shared account credit. Have one authorised buyer use `buy scarf credit "River Guild Supplies"` and verify that an unauthorised character is refused. `deals "~River Guild Supplies" scarf` lets an authorised user inspect account-eligible offers. Include the tilde inside the quotes for a multiword account name. When an authorised buyer charges the account, shopper eligibility for deals uses the account owner's identity.

Use `shop account userlimit <account> <user-name> <amount>` to change a buyer's limit, `shop account deauthorise <account> <user-name>` to remove a buyer, and `shop account suspend <account>` to toggle new charges for the whole account. Review the authorised users whenever clan purchasing duties change. A clan-owned bank account created with `bank openclan <type> <clan>` is a separate banking arrangement.

## Set up a shopkeeper NPC

The `Shopkeeper` AI is intended to react to sales in permanent shops and move **existing, already stocked goods** from the stockroom to the shopfront. It looks for a preferred merchandise container or other designated display space. It also checks display placement on the hourly tick. It relies on the shop's employment state, physical access and available stock. Ordinary `buy` and `sell` commands handle checkout, and merchandise auto-reordering controls creation of replacement stock.

### Build and assign a test shopkeeper

Use a disposable permanent shop with a shopfront, an adjoining stockroom and a container that has room for the example merchandise. Stand in the shopfront to designate a local cabinet and optionally choose it for this merchandise.

```text
shop display <cabinet-keyword>
shop merch edit scarf
shop merch set container <cabinet-keyword>
shop merch close
```

Load a Current NPC template with free hands and usable exits between the shopfront and stockroom. Give the NPC an active shop employment contract. An administrator can offer `shop employ <npc-keyword>` while both characters are in the shop, use `possess <npc-keyword>` to run `accept employ` as the NPC, then `return` to their staff character. Verify the contract with `shop contracts`. The AI uses the active employee list, so a separate clock-in step is unnecessary.

As an administrator, create the AI and attach it to the live NPC.

```text
ai typehelp shopkeeper
ai edit new shopkeeper WorkshopShopkeeper
ai close
ai add WorkshopShopkeeper <npc-keyword>
ai npclist WorkshopShopkeeper
```

This configures one live test NPC. To add it to future copies, use `npc set ai add WorkshopShopkeeper` while editing the template, then submit and review the revision. Future copies still need their own employment setup. The standard AI editor does not expose custom greeting or purchase callbacks for this type. [NPC templates, spawners and AI](NPC_Builder_Guide.md)

### Check what the current AI can do

First, stock a scarf near the cabinet and observe whether the employed NPC can put it on display. Check that the NPC can reach, hold and place the item, and that the container has space. Hourly display placement uses the employee's current room.

**Stockroom restocking has a current engine defect.** A sale can request a trip, but the shopkeeper reads a route queue the restocking effect never initialises. Treat AI-driven trips as unverified and keep this setup in a test shop. For day-to-day stocking, use the manual or automatic reorder workflows above. If a shopkeeper stops after a sale, check the restocking effect and server log as well as stock, access and the employee contract.

## Run a shop with employment hosts

Use `EmploymentWorker` for planned shop work such as buying ingredients, crafting sale goods, delivering stock, counting inventory and administering prices. Start with a permanent test shop. The examples use the local `shop` shortcuts. You can also target the shop explicitly with `employment shop <shop-id-or-name>`. [Employment hosts](Employment_Hosts_Builder_Guide.md)

### Prepare the workshop, supplier and worker

1. Use a reachable workshop room with the required tools. Add it to the premises with `shop extend <direction> workshop` if it adjoins the shop. Decide where raw inputs are delivered and where sale stock is displayed. A separate input shelf or container makes counts easier to check.
2. Prepare a Current craft that consumes one identifiable input, such as a bundle of wool, and produces one scarf that matches the `scarf` merchandise profile. Use that recipe's actual prototype and craft IDs in the following commands. Check the craft with a character at the workshop before delegating it. Enable stock generation by crafting only after checking any existing merchandise auto-reorder settings. Two separate replenishment systems can both act on the same shortage. [Crafting workflows](Crafting_System_Builder_Workflows.md)
3. Stock an ordinary supplier shop with multiple copies of the exact input in the same currency. Check its trading status, price including tax, stock and path from your shop. Use the exact supplier ID for the first run. Check that your own shop has enough accessible funds.
4. Give the NPC an active `crafter` contract and `EmploymentWorker` AI with tasking enabled. Check for `CanCraft`, `CanDeliverItems` and `CanPurchaseCommodities`. The purchase capability also covers discrete items. The NPC needs the craft's normal skills, hands and equipment. Use `shop contracts delegate <contract-id> show` to review its grants. The worker needs `ManageCraftRules` and `ManageDeliveryRoutes` for production and delivery, and `ApprovePurchases` and `ManageStockRules` to buy goods and manage the task's funds reservation. Grant spending access only to the employees who need it.

For a disposable fixture, an administrator can attach and configure the AI, then assign the NPC directly with `shop contracts assign #<npc-id> crafter`. Confirm the actual worker's role, capability and employment authority before finalising a task. If nothing claims it, `shop tasks diagnose` shows why.

### Test one materials purchase and delivery

Choose a small cap in your shop currency that covers the total price of five inputs, including tax. Use it for `authorise`, `reserve` and the purchase's `max`. Finalising under the proper authority issues the plan's bounded approval. During execution, the reservation sets aside funds for the purchase and `release all` frees any unused part of that task's reservation. Check the approval and source funds before running the plan.

```text
shop tasks draft new "Buy workshop wool"
shop tasks step authorise <batch-cap> for workshop wool
shop tasks step reserve <batch-cap> for workshop wool
shop tasks step purchase 5 item <wool-input-prototype-id> from <supplier-shop-id> max <batch-cap>
shop tasks step release all
shop tasks step deliver to <workshop-room-id>
shop tasks draft show
shop tasks draft finalise
```

The worker uses the native shop purchase and takes the purchased items into the task's custody. The delivery step places them in the workshop. If crafts look for inputs inside a particular bin, use `deliver to <workshop-room-id> container *<bin-item-id>` and check capacity and access. Verify the supplier's sale, the buyer's employment ledger and the exact goods in the workshop. `shop tasks show "Buy workshop wool"` shows the steps, supplier, financial reference and delivery evidence. See `shop tasks actions purchase` for exact live item, tag and weighted commodity selector forms.

### Test one craft and register the output for sale

Use a separate task so the only carried item at the final step is the intended sale output. The craft runs once and uses the usual input, tool, position and permission checks. The craft step adopts completed outputs into task custody. `stocktransfer to self` delivers them to your own shop and registers them under the specified merchandise profile. A plain `deliver` is useful for inputs and other goods that should simply be placed in the room.

```text
shop tasks draft new "Make a scarf for stock"
shop tasks step station <workshop-room-id>
shop tasks step craft <scarf-craft-id>
shop tasks step stocktransfer to self merch scarf destination <shopfront-room-id>
shop tasks draft show
shop tasks draft finalise
```

You can add `container *<display-container-id>` to the stock transfer. Use a workshop ID that resolves to a room and a destination belonging to this shop. Use one recipe whose successful outputs all belong to the same merchandise profile. The transfer works on the task's carried items, so inspect custody before reusing the plan for a recipe with mixed outputs. Failed craft salvage needs inspection before any retry.

Check `shop tasks show "Make a scarf for stock"`, then `shop merch list` and customer `list`. Expect the raw input to be consumed, the job to complete, and the scarf to appear as sale stock at the chosen destination. Try one purchase as a player and compare the resulting sale and stock count. Keep physical overflow and storage access in mind when commissioning a larger batch.

### Maintain materials and finished stock automatically

Give each goal one job. The materials goal orders inputs when workshop supply drops. The finished-stock goal runs the craft when the shop is short of scarves and an input is available. Both use the one-off plans you have already checked. Start with a named supplier, a bounded budget and a conservative cadence. Use an authorised issuer and inspect the drafted conditions and steps before finalising.

```text
shop goals draft new craftmaterials "Keep workshop wool supplied"
shop goals draft cadence 1h
shop goals draft budget <batch-cap>
shop goals condition item <wool-input-prototype-id> in <workshop-room-id> below 2
shop goals step authorise <batch-cap> for workshop wool
shop goals step reserve <batch-cap> for workshop wool
shop goals step purchase 5 item <wool-input-prototype-id> from <supplier-shop-id> max <batch-cap>
shop goals step release all
shop goals step deliver to <workshop-room-id>
shop goals draft show
shop goals draft finalise
```

```text
shop goals draft new craftstock "Keep scarves in stock"
shop goals draft cadence 30m
shop goals condition stock merch scarf below 3
shop goals condition item <wool-input-prototype-id> in <workshop-room-id> atleast 1
shop goals step station <workshop-room-id>
shop goals step craft <scarf-craft-id>
shop goals step stocktransfer to self merch scarf destination <shopfront-room-id>
shop goals draft show
shop goals draft finalise
```

If you switched to a specific input bin or display cabinet in the one-off test, carry that selector into the goal's conditions and steps. The two conditions on the scarf goal are both required by default. The quantity ordered and the production recipe are authored explicitly. This setup makes one scarf per craft task. The cadence and repeated stock checks control when it makes more.

Use `shop goals show` with the goal's ID or name to inspect both goals. `shop goals evaluate` evaluates the host's goals, so check what else is active before using it. Verify a low-material evaluation produces a capped order and that delivery makes the craft condition eligible. Verify the finished-stock goal stops producing work once the shelf threshold is met. Inspect `shop tasks`, `shop register` and `shop employmentledger` alongside the native merchandise and supplier records. The budget limits the generated work. Review repeated orders over time when deciding which cadence and quantities the business can afford.

When a supplier cannot satisfy the order, inspect the blocked task before replacing it. Change to `from any` only if the goal is meant to shop across eligible reachable suppliers. Keep the maximum price and material selector. Use `shop tasks actions supplier` or the action catalogue to find the supplier-selection preview your build exposes. If a payment was interrupted, reconcile it from the real ledger and custody evidence before retrying. [Employment task approvals and recovery](Employment_Hosts_Builder_Guide.md#approve-spending-and-handle-interruptions)

### Delegate store management

Use native shop commands for the initial premises, property association, merchandise, till or stockroom, and credit setup. Then use employment tasks for the routines the manager or worker should carry out. The examples below are action clauses to add with `shop tasks step` or `shop goals step`. Use `shop tasks actions <action>` to see its actual permissions and capability requirements.

| Routine | Start with |
| --- | --- |
| Count stock or move a delivery onto sale stock | `stocktake all` or `stocktake merch scarf` records the observed counts or weight. `stocktransfer` moves carried task stock to a permanent shop and registers it under the chosen merchandise. Check differences before any write-off. |
| Set prices or manage a promotion | `price merch scarf <amount>` changes that shop's base price. `sale create "Market day scarves" target merchandise scarf adjustment -10% type sale applies sell` creates a native deal. Add an expiry for a temporary offer. The same action family can modify or cancel the deal. |
| Review cash and maintain a till | `cashreconcile opening float` compares cash and the expected float and records the result. Use the float goals and actions only where the shop has an appropriate physical register. Bank balance work uses its separate host cash and bank actions. |
| Pay taxes or a supplier's store account | `paytax <amount>` and `storepay <supplier-shop-id> account <account-name> amount <amount>` use their native payment records. Give the plan an approval and reserve the funds before the payment step, as in the purchase example. |
| Manage staffing and wages | Use `shop contracts`, `shop openings`, `shop applications` and `shop payroll`. The `staffing` goal and `jobopening` action can manage openings. A manager role still needs the delegated financial and task authorities used by its plans. |
| Review outcomes and recover work | Read the task's step states, `shop register` and `shop employmentledger`. Reconcile those with the shop's own sale, stock or tax records. Use the shared recovery workflow for blocked finance or uncertain custody. |

Try one management action before scheduling it. For example, use `shop tasks create "Count the shop" stocktake all` and inspect the resulting evidence in `shop tasks show "Count the shop"`. Use the `pricemargin`, `taxes`, `accounts`, `staffing` and cash or bank balance goal types when their condition and action catalogues fit the business policy. Keep the staff board for messages and use live tasks to carry instructions to NPC workers.

## Connect a shop bank account

You need a branch whose bank uses the same currency and an account type allowing shop owners. Reuse an existing branch and type if available. At a branch, inspect `bank types` and `bank preview <type>`. Review fees before you use the type for the first deposit.

To create a test bank, stand in the room you want to register as its branch. Use Admin authority to configure it.

```text
bank edit new "Workshop Bank" WORKSHOP <test-zone>
bank set branch
bank set type add "Workshop Trade"
bank set type "Workshop Trade" canopen shop AlwaysTrue
bank set type view "Workshop Trade"
bank close
```

The branch command toggles your current room as a branch. A shop-opening prog must return Boolean for one Shop argument, or accept any parameters. `AlwaysTrue` makes the type available to all shops and is appropriate for this isolated example. Use a narrower policy on a shared bank. Creating a type alone does not supply a shop eligibility rule. [FutureProg for builders](FutureProg_Builder_Guide.md)

At the branch, switch to the shop proprietor and open the account on the shop's behalf. Use the resulting account number, and use the currency name and amount format you checked earlier.

```text
set currency <zone-currency>
bank openshop <account-type> "Workshop Market"
bank accounts
bank deposit <account-number> <amount>
bank account show <account-number>
bank transactions <account-number>
```

For the deposit, the proprietor needs to be holding the actual coins. Inspect the account history for the deposit and any fees. At the shop, link the account as the authorised proprietor or an administrator with access to the account.

```text
shop bank WORKSHOP:<account-number>
shop info
```

Replace `WORKSHOP` with the bank's actual code. The account belongs to the shop. A manager can open it on the shop's behalf. Changing the link requires proprietor or administrator authority and access to the account.

Run another cash sale and compare the ledger, physical cash and bank transactions. The coin payment goes into the stockroom or till. A bank deposit is a separate authorised action at a branch. When diagnosing the shop's ability to pay out, inspect its virtual cash, physical cash and any linked bank account.

## Optional payment items

If the game enables `KeycardPaymentsEnabled` and the bank issues appropriate payment items, test `buy scarf with <payment-item>` with freshly stocked merchandise. The buyer needs an authorised item tied to an account in the shop currency. The shop also needs a compatible linked bank account. The account type controls the payment-item prototype and number that can be issued. Use `bank requestitem <account-number>` at the branch and inspect the resulting transaction on the authorised accounts.

## Troubleshooting

| Symptom | What to check |
| --- | --- |
| The price is refused or cash payment cannot settle | Use the zone currency's amount format. Check the final price after tax, actual coin denominations, and the shop's ability to make change. |
| The item is visible in the room but absent from `list` | A merchandise profile alone is a price rule. Stock an actual matching item with `shop stock`. Inspect the active profile if several match. |
| Checkout says the shop is closed | Check trading status in `shop info`. An authorised employee or administrator can use `shop open <shop>`. |
| Checkout cannot place cash | Check that a till or stockroom is configured and available. The virtual balance is a separate source of funds. |
| The final price differs from the merchandise price | Check the economic zone's applicable sales taxes as well as shop pricing rules. |
| An expected promotion is missing | Check merchandise or tag, direction of trade, volume threshold, eligibility and in-game expiry. |
| A buyer cannot charge a clan's account | Check the character is on the authorised list and verify the per-purchase limit, remaining account credit and suspension status. |
| The property claim is refused | Use a conveyancing location in the correct zone and check the property's rooms and current lease. During an active lease, the authorised leaseholder can claim. |
| Bank refuses to open the account | Confirm the character manages the shop, the bank has the requested type, and the type's Shop policy accepts this shop. |
| A deposit or payment item is refused | Verify branch, account status, authorisation, and currency. For deposits, hold the actual coins. For payment items, check the setting, the issued item and the shop's bank link. |

For region-wide setup, markets and other services, see the [economy workflows](../Economy/Economy_System_Workflows_and_Integration.md). Use the [command reference](https://futuremud.com/docs/commands) for the broader shop and banking command surfaces.
