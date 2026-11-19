---
slug: loans-mortgages-and-pawnbroking
category: Economy
summary: Build in-game lending products, write underwriting progs, redeem pawned goods, close purchase mortgages and handle repayments or collateral recovery.
---

# Loans, mortgages and pawnbroking

A **lender** is the bank or shop that advances the money. A **product** specifies the terms it can offer and which applications it will accept. An **offer** quotes terms to one borrower. Acceptance creates the active **loan** and records any security. Changes to a product affect what the institution can offer going forward. Check a signed loan's statement for the terms already agreed.

Alderbridge’s bank lends to guild members and finances a warehouse purchase. Its pawnbroker advances money against a silver ring. A pledge goes into lender custody, while a mortgage puts the property in the buyer’s name with a lien on the title. Borrowers see their balance, next payment date and current payoff. The examples use crowns (`cr`) with denominations that can represent the example amounts. Use the currency syntax of your own zone. [Shops, currencies and banking](Shop_and_Banking_Builder_Guide.md) · [Clan property and conveyancing](Clan_Builder_Guide.md#own-the-town-hall-and-lease-out-a-workshop)

## Enable lending and prepare the lender

A staff member with the relevant administrative permission sets `LoansEnabled` to `true` in the editor opened by `editstaticconfiguration LoansEnabled`. This turns on new applications worldwide. Each lender makes an offer available by publishing an individual product. At a bank branch, work as a bank manager. At a shop, use its proprietor or a shop manager. If a room has both lenders, qualify management commands with `loan manage bank <bank-id> ...` or `loan manage shop <shop-id> ...`.

| Product | Use |
| --- | --- |
| `unsecured` | Advance funds based on the borrower's eligibility and exposure. No asset is pledged. |
| `pawn` | Advance against one held and owned item. The lender takes custody at acceptance. |
| `property` | Advance against a property the borrower already owns outright. |
| `mortgage` | Finance a listed property's purchase as part of conveyancing. The buyer pays a contribution, the lender advances the rest, and the closing puts the lien on the new title. |

Start with a lender in the same economic zone as the property and use the zone's currency throughout. Leave enough actual money for funding. At a bank branch, use `bank manager balance` to inspect reserves and liabilities and `bank manager deposit <amount>` to add cash to reserves. The bank's lending capacity excludes positive customer deposit obligations and the product's liquidity floor. A shop can use its accessible shop cash, including virtual cash, or a same-currency bank account owned by the shop. With an account, check its available balance, withdrawal fees and its bank's reserves. Keep the lender’s spendable funds separate from its maximum advance. Choose a bank or shop product sized to the reserves you can set aside.

### Set the repayment rhythm

The rate applies once per named in-game interval on outstanding principal. For example, 5% on `40cr` accrues `2cr` over one full week if no principal is repaid. A shorter interval changes how quickly interest accrues. Both due dates and grace use the lender’s zone calendar.

| `minimum` | What the borrower owes |
| --- | --- |
| `none` | One balloon at maturity. Earlier repayment is available. |
| `interest` | Accrued interest each interval, then all remaining principal and interest at maturity. |
| `principal` | Interest plus the configured principal `instalment` each interval, then the remaining balance at maturity. Set a positive instalment before publishing. |

Use `loan manage product edit <id>` to review the main terms and `loan manage product enable <id>` to publish. `loan manage product disable <id>` withdraws pending requests and offers for that product. Existing loans can still be repaid, including when `LoansEnabled` is turned off. Applications and written offers have a seven-real-day expiry. The advance, security, underwriting and funds are checked again at acceptance. Keep enough room in reserves or shop cash for all the offers you expect borrowers to take up.

## Give the guild a small unsecured loan

Alderbridge’s bank offers guild members a short loan for workshop supplies. Write `AlderGuildLoanEligible` with a Boolean return and `AlderGuildLoanLimit` with a Number return. Give both progs the same four inputs in this order. Each monetary input and the Number result use the lender’s base currency units.

| Input | Type | What the lender passes |
| --- | --- | --- |
| `borrower` | Character | The applicant. |
| `requested` | Number | The proposed advance. |
| `appraisal` | Number | The collateral appraisal. Unsecured applications pass zero. |
| `exposure` | Number | Principal the borrower already owes this lender across its products. |

Use `prog edit new <name>`, `prog set return <type>`, and `prog set parameter add <name> <type>` for each input. Use `prog set text` to enter each body, then submit the editor. Leave the guild check dynamic so it can respond to membership changes. Replace `<guild-id>` with the guild’s numeric clan ID. [FutureProg authoring](FutureProg_Builder_Guide.md) · [Clan membership](Clan_Builder_Guide.md)

The eligibility body for `AlderGuildLoanEligible` follows.

```text
return IsClanMember(@borrower, ToClan(<guild-id>))
```

The cap for `AlderGuildLoanLimit` follows.

```text
return 75
```

As a manager at the bank, build the product and use the ID returned by the first command.

```text
loan manage product new "Guild Supply Loan" unsecured
loan manage product edit <supply-product> set interval every 1 week
loan manage product edit <supply-product> set periods 2
loan manage product edit <supply-product> set rate 2%
loan manage product edit <supply-product> set minimum principal
loan manage product edit <supply-product> set instalment 25cr
loan manage product edit <supply-product> set grace 7
loan manage product edit <supply-product> set maxunsecured 100cr
loan manage product edit <supply-product> set exposureunsecured 120cr
loan manage product edit <supply-product> set unsecuredeligibility AlderGuildLoanEligible
loan manage product edit <supply-product> set unsecuredmaximum AlderGuildLoanLimit
loan manage product edit <supply-product> set automatic true
loan manage product enable <supply-product>
```

A guild member with `60cr` already outstanding has at most `60cr` of room under the exposure cap. The `75cr` prog ceiling can reduce an advance but cannot raise that limit. For pawn, property and mortgage products, use `securedeligibility` and `securedmaximum` in the same way. Use `none` to clear a prog. A missing, failing or incompatible configured prog prevents the lender from offering the advance.

At the issuing branch, a member requests `50cr` and checks the written offer before accepting. They can take coins or use `into <bankcode>:<account>` to deposit the advance into their own active account in the loan currency.

```text
loan offers "Guild Supply Loan"
loan ask <supply-product> 50cr
loan <offer-id>
loan accept <offer-id>
loan <loan-id>
```

After one full weekly interval, with no earlier payment, the first due amount is `26cr`. That is `1cr` of interest and `25cr` of principal. At the issuing branch, `loan pay <loan-id>` quotes the amount due today. Add an amount for a partial repayment or use `all` for the current payoff. Add `from <bankcode>:<account>` to pay from an authorised account, then enter `accept payment`. The receipt shows how much remains due. `loan history <loan-id>` shows the payment and its source.

## Open a pawn counter and redeem a ring

Use a trading shop with at least `60cr` of accessible shop cash for this example. A product floor of `10cr` leaves at least that much behind when granting an advance. If your shop uses a bank account, open a shop-owned account with `bank openshop <account-type> <shop>`, link it at the counter with `shop bank <bankcode>:<account>`, and set product `funding account` instead.

At the shop counter, create the draft and substitute its returned ID wherever you see `<pawn-product>`.

```text
loan manage product new "Silver Ring Pawn" pawn
loan manage product edit <pawn-product> set interval every 1 week
loan manage product edit <pawn-product> set periods 4
loan manage product edit <pawn-product> set rate 5%
loan manage product edit <pawn-product> set minimum none
loan manage product edit <pawn-product> set grace 7
loan manage product edit <pawn-product> set recourse false
loan manage product edit <pawn-product> set mincollateral 50cr
loan manage product edit <pawn-product> set coverage 1.25
loan manage product edit <pawn-product> set maxsecured 50cr
loan manage product edit <pawn-product> set exposuresecured 60cr
loan manage product edit <pawn-product> set floor 10cr
loan manage product edit <pawn-product> set funding cash
loan manage product edit <pawn-product> set auctionhouse none
loan manage product edit <pawn-product> set automatic true
loan manage product enable <pawn-product>
loan manage product
```

This product has a balloon payment after four in-game weeks, seven in-game days of grace and no deficiency collection after collateral settlement. `automatic true` permits self-service offers within policy. Set it to `false` to queue applications for manager review. `coverage 1.25` means `125cr` of appraised collateral for each `100cr` advanced. A ring appraised at `60cr` supports at most `48cr` before other checks. `maxsecured` caps this advance and `exposuresecured` caps the principal owed to this lender after the advance.

Use an ordinary durable ring prototype with a positive cost and an undamaged ring. The appraisal uses the prototype cost in global base currency, adjusted for the item's condition and converted to the lender's currency. Set its prototype cost so a ring in full condition appraises at `60cr` in Alderbridge’s currency. The item needs to be a single held instance with the borrower as its registered owner. Use `ownership show ring` to inspect it and `ownership claim ring` if it is unowned. Filled containers, currency piles, bulk commodities and transforming or temporary-value items are unsuitable pledges.

As an ordinary borrower at the counter, request `40cr`, read the quote, then accept the displayed offer.

```text
loan offers
loan pawn ring 40cr using <pawn-product>
loan accept <offer-id>
loans
loan <loan-id>
```

Acceptance transfers the original ring into lending custody and pays the advance. The loan statement identifies the pledged ring by item ID. It becomes available to collect after payoff. At four full weekly boundaries with no earlier payment, the illustrative payoff is `48cr`. Repaying earlier changes the interest. Read the game quote and pay at the issuing lender.

```text
loan pay <loan-id> all
accept payment
loan collect <loan-id>
loan history <loan-id>
```

Collection releases the original ring to its borrower. Its item ID matches the pledge shown on the statement. If the borrower pays by autopay, the ring also waits for collection at the issuing counter.

## Finance a warehouse purchase

Use a listed **South Warehouse** priced at `1000cr` with a last sale value of at least `1000cr`. At the conveyancer have the seller run `property sell "South Warehouse" 1000cr` and have each title holder use `property approvesale "South Warehouse"` as needed. Choose an unleased property with clear title, returned keys, and no hosted business or competing listing. The mortgage uses the lower of the last sale value and the asking price as the appraisal. The buyer starts with `200cr` for the contribution. The bank needs room to advance `800cr` after reserving for its obligations and the product's floor.

At the bank branch, create and enable the product. Use the ID of the local auction house in the same economic zone. This commits the lender to auctioning the warehouse if it later takes possession.

```text
loan manage product new "Warehouse Mortgage" mortgage
loan manage product edit <mortgage-product> set interval every 1 month
loan manage product edit <mortgage-product> set periods 8
loan manage product edit <mortgage-product> set rate 1%
loan manage product edit <mortgage-product> set minimum principal
loan manage product edit <mortgage-product> set instalment 100cr
loan manage product edit <mortgage-product> set grace 7
loan manage product edit <mortgage-product> set recourse true
loan manage product edit <mortgage-product> set coverage 1.25
loan manage product edit <mortgage-product> set maxsecured 800cr
loan manage product edit <mortgage-product> set exposuresecured 800cr
loan manage product edit <mortgage-product> set floor 100cr
loan manage product edit <mortgage-product> set auctionhouse <auction-house-id>
loan manage product edit <mortgage-product> set automatic false
loan manage product enable <mortgage-product>
```

At the bank branch, the buyer requests financing. A separate manager uses `loan manage` to find the pending request and approves it with a reason. The buyer can review the resulting offer with `loan <offer-id>` before closing at a conveyancer in the same zone.

```text
loan buy "South Warehouse" with 200cr using <mortgage-product>
```

```text
loan manage
loan manage approve <request-id> Verified purchase price and contribution
```

```text
property buy "South Warehouse" loan <offer-id>
property preview "South Warehouse"
loan <loan-id>
```

The closing command accepts the mortgage offer and attempts settlement immediately. Review the quote before issuing it. You can append `from <bankcode>:<account>` for the buyer's contribution. When settlement succeeds, the sellers receive the price, the buyer receives title and the lender's `800cr` lien is recorded. A seller without an account can use `property proceeds` at the conveyancer to inspect the money held for them. If a price, consent or contribution has changed, read the closing refusal, inspect the listing, and ask for a new quote if the terms have changed.

### Repay the mortgage and set up autopay

The first whole month's expected payment is `108cr`, consisting of `8cr` interest and `100cr` principal. For a buyer paying by hand, `loan pay <loan-id> 60cr` followed by `accept payment` at that boundary pays the `8cr` interest and `52cr` principal. The statement then shows `748cr` principal and `48cr` still due for that instalment. The payoff already includes arrears.

Enable automatic payments before a scheduled due date with `loan autopay <loan-id> from <bankcode>:<account>`. The account must be active, in the loan currency and authorised for withdrawal. Run `loan autopay <loan-id>` to inspect the mandate. Autopay attempts the full amount due, including arrears, even while the borrower is offline. Allow room for any withdrawal fee and compare the bank withdrawal with the payment in `loan history <loan-id>`.

If the account falls short, the statement shows the failed attempt and the amount still due. Refill the account and use `loan autopay <loan-id> retry` to make an immediate attempt. Use `loan autopay <loan-id> off` to stop automatic repayments. [Autopay and recovery behaviour](../Economy/Economy_System_Workflows_and_Integration.md#automatic-loan-payments)

### Lend against property the borrower already owns

Use a separate `property` product when a customer needs funds against a property they already own outright. Set its secured limits and repayment terms, then enable it. For example, a privately owned bare yard with a last sale value of `500cr` supports up to `400cr` at coverage `1.25` before other checks. Clear ordinary sale or auction listings and prior liens before offering the loan. A property hosting a shop, hotel, hospital, or stable is ineligible.

```text
loan ask <property-product> 300cr against "North Yard"
loan <offer-id>
loan accept <offer-id>
property preview "North Yard"
```

The borrower stays on the title and receives the advance at the lender. The lien limits transfers and changes to leases until the loan is released. Existing tenancy is recorded when the property is pledged. If the lease changes before acceptance, quote again. Payoff releases the lien. The purchase mortgage above instead closes at conveyancing and sends the bank’s advance towards the seller’s price.

## Handle a late payment or recover collateral

When a borrower misses a payment, the statement shows the oldest unpaid instalment and the cure deadline. The borrower can pay the amount due before repossession. `loan manage` lists the lender’s open applications, active accounts and recoveries. At the issuing bank or shop, an independent manager can inspect a specific account with `loan <loan-id>` and review its postings with `loan history <loan-id>`.

### Reschedule a warehouse instalment

If a warehouse buyer loses a month of trade, the manager can offer a new schedule. The original schedule still applies while the proposal is outstanding. The borrower reviews the proposed terms and accepts or declines at the lender. Use an audit reason that explains what is changing.

```text
loan manage plan <loan-id> 8 100cr Resume normal payments after the wharf reopens
loan <loan-id>
loan accept <loan-id>
loan history <loan-id>
```

| Manager action | How it affects the account |
| --- | --- |
| `loan manage plan <loan-id> <periods> <principal-per-period> <reason>` | Offers a new schedule at the existing interval. Use `0` for interest-only instalments. |
| `loan manage pause <loan-id> <days> <mode> <reason>` | Offers to defer dues for the specified number of days using mode `interest` to continue accruing interest or `free` to suspend it. |
| `loan manage refinance <loan-id> <periods> <principal-per-period> <reason>` | Offers replacement terms with outstanding interest rolled into principal. The histories record the old and new loan IDs. |
| `loan manage writeoff <loan-id> <reason>` | Offers a manager confirmation to write off an active balance and release security. Confirm with `accept writeoff`. |

### Recover the ring or foreclose on the warehouse

For a pawn the borrower chooses to leave unredeemed, review the balance after the cure deadline. With `auctionhouse none`, the shop settles at the agreed appraisal. The manager needs capacity to take the original item and the shop needs funds for any borrower equity. When a ring appraised at `60cr` secures an unpaid balance of `50cr`, retention clears the debt and leaves `10cr` for the borrower to collect.

```text
loan manage enforce <loan-id> Pawn pledge past the cure deadline
accept enforce
loan history <loan-id>
```

The borrower uses `loan collect <loan-id>` at the lender to take any available surplus, or `loan collect <loan-id> into <bankcode>:<account>` to deposit it. A pawn recovered this way can go into ordinary merchandise stock once the manager has taken custody. [Stock existing goods](Shop_and_Banking_Builder_Guide.md#stock-existing-goods)

For an auction-backed product, choose `auctionhouse <auction-house-id>` before originating the loan. Use a house in the lender’s economic zone. Enforcement lists the original pledge. The borrower’s account stays in recovery until the proceeds clear, and auction pickup is a separate step for the winner. If the lot ends without a sale, use `loan manage relist <loan-id> <reason>` and `accept relist`. A `35cr` net sale on a `50cr` non-recourse pawn writes off the `15cr` shortfall. On a recourse product, that deficiency remains collectible as unsecured debt.

The warehouse mortgage uses the same `enforce` command after the cure deadline. Title moves to the bank for recovery and the auction lists the whole property. An existing lease stays in place. Rent collected during recovery goes towards the debt and then any surplus. On a sale, the buyer receives title through the auction and the settlement applies the net proceeds to the loan. With `auctionhouse none`, the bank keeps the property at the agreed appraisal and must fund any surplus before taking title.

Unsecured loans can be repaid, rescheduled or written off. For a deceased borrower eligible for probate, review the estate claim and collateral before pursuing settlement. The estate can have a separate claim or entitlement to surplus and the borrower’s normal collection route changes. [Collateral recovery](../Economy/Lending_System.md#collateral-default-and-recovery) · [Death and probate](../Economy/Lending_System.md#death-and-probate)

## Diagnose a refused or incomplete loan

| Symptom | Check |
| --- | --- |
| The counter shows no offers | Check `LoansEnabled`, the current branch or shop, product publication and the selected lender. |
| The lender will offer less | Compare the appraisal and coverage with the per-advance cap, total exposure, liquidity floor and executable funding. |
| The ring is in hand but the pawn is refused | Check registered ownership, single-item quantity, prototype cost, condition and currency conversion. |
| A mortgage is approved but will not close | Check the offer ID, current sale order, conveyancing location, buyer contribution, lender funds, full title and outstanding keys. |
| The amount due stays unpaid after an automatic attempt | Inspect the mandate and loan failure reason. Recheck account status, balance, fees, withdrawal authority and bank reserves, then retry. |
| A paid loan still lists an item or surplus | Review the history and collect from the issuing lender. Check whether an auction or another settlement still has a payout outstanding. |
