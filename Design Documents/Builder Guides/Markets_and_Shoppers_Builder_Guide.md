---
slug: markets-impacts-and-shoppers
category: Economy
summary: Connect tagged goods and shop prices to a regional market, stage a grain shortage, and schedule a virtual shopper with a budget.
---

# Markets, impacts and shoppers

A **market** sets price multipliers for categories of goods. A **market influence** changes the supply, demand or price inputs for a period of in-game time. A **shopper** is a scheduled virtual customer with a budget. It can remove stock and generate a sale at a trading shop. Set up pricing first so you can measure a shortage without a shopper buying your test stock in the meantime.

## Connect one product to a market

Use an Admin character, a test economic zone, and a permanent shop that already sells a single unstacked, tagged item, such as a sack of grain. Have a customer run `list` at the shop and record its ordinary price. For an easy comparison, use one sale item, a fixed base price and no promotional deal. Record the shop, zone and existing grain tag IDs. [Shop setup and merchandising](Shop_and_Banking_Builder_Guide.md)

Create a category for the grain and a market in the shop's economic zone. These are separate records. Creating a category needs an existing item tag. The tag belongs on the goods' item prototype. Use `item set tag <tag-id>` on an item revision if needed, approve it, and check the actual goods you stock.

```text
mc new <grain-tag-id> Harbour Staples
mc set eunder 50%
mc set eover 50%
mc close
market new <economic-zone-id> Harbour Market
market set category Harbour Staples
market show Harbour Market
market close
```

`market set category` toggles membership, so check the market before repeating it. At the shop, use `shop set market Harbour Market` and `shop info`. Have the customer check `list` again. With the default formula and no applicable influence, supply and demand are both 1, giving a multiplier of 1. A shop needs an explicit pricing market even if it shares the market's economic zone.

For a wider family such as provisions, a category can use `mc set type combination` and `mc set component <category> <weight>`. Inspect the constituent categories and weights before applying a shortage to the family. If several categories match an item, the market takes the highest matching price multiplier. An item with no match receives 1.

## Stage a grain shortage

Make a reusable influence template and then start an instance in Harbour Market. The template describes the effect. The instance supplies the affected market and dates. With no dates on `mi begin`, the instance starts now on the economic zone's reference calendar.

```text
mit new Harbour Grain Shortage
mit set impact "Harbour Staples" -20% 0% 0%
mit close
mi begin "Harbour Market" "Harbour Grain Shortage"
mi set duration 1.00:00:00
mi list market "Harbour Market"
market show Harbour Market
```

The three percentages are changes to supply, demand and the flat price adjustment, in that order. Here supply becomes 0.80 and demand stays 1.00. Under the default formula and the two 50% elasticity settings, the multiplier is **1.125**. The shortage relative to the smaller quantity is `(1.00 - 0.80) / 0.80 = 0.25`, then `1 + 0.50 × 0.25 = 1.125`. For a base price of 80, expect 90 before taxes, deals and currency rounding. Compare customer quotes on a tagged product and an untagged control.

Use `eunder` for shortages, when demand exceeds supply, and `eover` for gluts, when supply exceeds demand. If you tuned the values in an older build, inspect `mc show <category-id>` and use the reported oversupply and undersupply values to decide whether to retune the category.

A flat price impact adds to the computed multiplier. For example, `mit set impact "Harbour Staples" 0% 0% 10%` describes a ten-percentage-point adjustment with neutral supply and demand. Each setting replaces the impact row for that category. Use distinct templates for events you want to overlap. Check the market's actual `formula` before using the example arithmetic in a customised economy.

For a future event, use `mi begin <market-id> <template-id> "<start date and time>" "<end date and time>"` with dates valid in the zone. To edit a running instance select it with `mi edit <influence-id>`. Use `mi set impact` on that instance, `mi set until` to change its expiry, or `mi end <influence-id>` to end it early. Verify the baseline quote returns when no other active influence applies.

## Model household pressure

A market population describes households' category needs and the effect of prices on their budget. It has its own income factor, savings and stress thresholds. A virtual shopper has its own spending budget and schedule. Create each when you want both price pressure on households and actual sales at shops.

```text
marketpopulation new "Harbour Market" Harbour Households
marketpopulation set scale 250
marketpopulation set income 1
marketpopulation set savingscap 6
marketpopulation set savings 2
marketpopulation set need "Harbour Staples" <amount-in-zone-currency>
marketpopulation show Harbour Households
marketpopulation close
```

The need is an expenditure in the zone currency. Savings are measured in budget cycles. To model lost wages during an event, edit the live influence and use `mi set popimpact <population-id> -10% 1`. This applies a negative additive income impact with a neutral multiplier. Check the population's effective income and stress as its budget updates. Use `marketpopulation set stress add <threshold> <name> <start-prog-or-none> <end-prog-or-none>` when crossing a threshold should trigger a response. `set flicker` on the population configures the margin for leaving a stress state.

## Give a virtual shopper one shop and one purchase

For a predictable first trip, give the test shop just one eligible, unstacked item. Restrict the shopper to the shop's ID. A shopper selects from **trading, ready shops across its economic zone**. Several markets can share that zone.

Create a Boolean FutureProg with one Shop parameter named `candidate`. Use `prog edit new HarbourGrocerOnly`, `prog set return boolean`, `prog set parameter add candidate shop` and `prog set text`. Enter `return @candidate.id == 123`, replacing 123 with your shop ID, finish the editor with `@` and confirm compilation. [FutureProg for builders](FutureProg_Builder_Guide.md)

```text
shopper typehelp simple
shopper edit new simple <economic-zone-id> "Harbour Weekly Buyer"
shopper set shop HarbourGrocerOnly
shopper set budget <amount-in-zone-currency>
shopper set interval every 1 week
shopper set next <local-date> <local-time>
shopper show "Harbour Weekly Buyer"
shopper close
```

Choose a budget at least as large as the quoted price, and set the next trip a little ahead of the local game clock. New simple shoppers start with a zero budget, accept-all selection progs, equal weighting, and empty-shop skipping enabled. The `shop` and `item` progs decide eligibility. `shopweight` and `itemweight` return numbers for weighted random selection. Start with the default item progs. For a custom item filter, use a Boolean prog taking Item. For an item weight, use a Number prog taking Item. Both can also take Merchandise as the second parameter and the quoted price as a Number third parameter. Older saved item progs declared with Shop first receive the selected shop until you replace them.

After the trip, run `shopper log "Harbour Weekly Buyer"`. Look for the selected shop, a `buy` line for the item, and `finish`. Check the shop's sale record, stock count and proceeds. A virtual purchase consumes the sold item or reduces a stack, and the native sale can trigger merchandise auto-reorder. With reorder enabled, a full shelf after the trip can coexist with a completed sale.

| Observation | What to inspect |
| --- | --- |
| The influence appears but the quoted price stays level | Dates on the live instance, the shop's selected market, the product's tags, category membership, elasticity, and competing active influences. |
| Another shop gets the sale | The shopper's economic zone and Boolean shop-selection prog. A market's name is not a shop selector. |
| The shopper logs `noshop` | Check whether the shop is trading, ready, eligible and stocked. After correcting a failed run, inspect the next scheduled time and use `shopper set next` to schedule another trial. |
| A shop is selected but there is no `buy` entry | Inspect its available merchandise, shopper budget, quoted prices, and item selection or weighting progs. Test a single unstacked item first. |
| Cash changes but population stress does not | Inspect the population's own needs, savings, effective income and thresholds through `marketpopulation show`. Shopper spending and household budget evaluations run independently. |
