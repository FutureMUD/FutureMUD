# Loans, mortgages and pawnbroking

**Design proposal for review.** A character borrows from a specific bank or shop. The establishment supplies the money, the borrower accepts dated terms, and both see the same balance, next payment and consequences of default. A cash loan, a mortgage and a pawn ticket share one loan record. Their differences are where the advance goes, who holds the security, and what the lender can recover.

## What a player would do

The [economy workflow guide](Economy_System_Workflows_and_Integration.md#offering-and-using-loans) documents implemented commands and product underwriting fields. This design also describes target behaviours and acceptance scenarios.

At a pawnbroker, Mara types `loan pawn ring`. All prices, rates and periods here are illustrative world data.

```text
Copper Street Pawnbroker will lend you 40 crowns against your silver ring.
You receive 40 crowns now. We hold your ring until you repay.
Interest is 5% per market week on unpaid principal. No weekly payment is required.
Redeem by the end of week 4 for 48 crowns if you make no earlier payments.
You have 1 more market week to redeem. Interest continues during that week.
After that we may take the ring and auction it. You receive any surplus.
You owe nothing further if the sale falls short.
To agree, type LOAN ACCEPT 17. To walk away, do nothing.
```

Mara accepts, receives the coins and can later type `loans`, `loan 17` or `loan pay 17 all`. The last command quotes today's exact redemption price and asks her to confirm before taking payment. At the issuing shop she gets the same ring back. If her hands are full, the receipt tells her how to collect it later.

Jo finds South Warehouse listed for 1,000 crowns. At a participating bank, `loan buy "South Warehouse" with 200 crowns` requests financing for the remaining 800. The bank either quotes immediately or sends the request to a manager. Jo takes the approved offer number to the conveyancer and types `property buy "South Warehouse" loan 28`. One confirmation pays the sellers 1,000, registers Jo as owner and secures the bank's 800 against the warehouse. Jo never has to handle the advance or return to pledge the property.

## Scope and opt-in

- **World.** Add a proposed `LoansEnabled` setting, initially false, with no stock products or seeder prerequisites. With no contracts, a disabled world adds no loan polling or loan prompts to normal gameplay. Switching off originations later retires offers. Repayment, amendments, claims, release and recovery remain available on existing loans. Prevent removal of the last usable service location, lender or required currency until outstanding contracts, pickups and payouts have a route to completion.
- **Lender.** A builder enables lending on a particular bank or shop. Its managers can then publish products or issue individual offers within limits. A bank branch serves that bank. A shop offering pawns serves its own book from its trading premises and can use its own funds in a world without banks. A shop's linked bank account is a funding choice. The shop remains the creditor. A shop and bank sharing a room keep distinct permissions and books.
- **First-release contract.** One character borrower, one institutional lender, one zone and its native currency, positive principal, a rate (zero is valid), maturity and a recovery policy. Security is optional. Accept one whole item or one whole property. An existing property pledge requires sole title. A purchase mortgage conveys full title to one borrower and may pay multiple sellers. Defer fractional pledges, multiple liens, guarantors, cross-zone contracts, clan borrowers and private character lenders.
- **Underwriting.** Staff approval is the default. Self-service needs a per-loan limit, total exposure limit per borrower and available lender funds. Secured self-service also requires an approved valuation and maximum advance as a proportion of value. For purchases use the lower of the approved price and valuation. An unknown value goes to a manager. Record the appraisal, who or what produced it, and its date. Fixed products and staff appraisals require no programming. Optional FutureProgs may determine eligibility or value. Recheck limits, security, funds and authority at acceptance.

## Proposed command surface

| Command | Player or staff experience |
| --- | --- |
| `loans` / `loan <id>` | Recall your own summary or statement anywhere. Show collateral, principal left, interest, amount needed to become current, next due date, maturity, payoff today and the next action. Include a pending offer, recovery, pickup or surplus where applicable. |
| `loan offers` / `loan offers <product>` | At a lender, list available products and explain them in the zone's currency and calendar. Show one applicable command example. |
| `loan ask <product> <amount> [against <asset>]` | Request an ordinary advance or pledge an already-owned property or item. Return an offer, a pending staff request, or an actionable reason. |
| `loan pawn <held item>` / `loan buy "<property>" with <down payment>` | Shortcuts for a pawn or purchase mortgage. If several lenders/products qualify, present named choices and wait for selection. |
| `loan accept <id>` / `loan decline <id>` | Accept the quoted terms or walk away. For a mortgage this shows the conveyancer and the exact closing command. The binding confirmation happens there. The offer states expiry, amount received, payment destination, schedule, grace, security, recourse and disposal terms. A quote reserves neither funds nor property. |
| `loan pay <id> [amount or all] [from <account>]` | At the lender, omit the amount to pay what is due, including arrears. With nothing due, show the next date and how to prepay. Default to cash, or use an explicitly named, authorised same-currency account. Quote exact tender, applicable account charges and resulting balance. If the amount exceeds payoff, offer the exact figure. |
| `loan collect <id>` / `loan history <id>` | Collect a released pledge or surplus at the named counter. View receipts, amendments and collection status. Repeating a completed collection shows its receipt. |
| `loan manage` at the lender | Staff see requests, overdue cases, recoveries and failed payouts. `loan manage offer <request>` prepares terms. The `plan`, `pause`, `refinance`, `enforce`, `relist` and `writeoff` actions take a loan ID and preview their effects. Record the reason and confirmation. |

Use one reference from request through active loan. Link `bank loans` and `shop loans` to contextual help. Disambiguate by name or ID before acting. A cash loan pays out at the counter by default. Borrowers can select their own active, same-currency account in the offer. For a mortgage the borrower selects cash or an authorised account for their contribution at the conveyancer. Confirmations identify the source, destination and amounts, and changing a material term requires a fresh confirmation.

Keep statements private to the borrower and authorised staff of that lender. Persist notices for login or the next statement, including a missed payment, exact cure deadline and place to redeem. Default can be evaluated while a character is offline. Make refusals actionable, for example “The warehouse price changed to 1,100 crowns. Ask West Bank for a new quote with LOAN BUY.”

## Terms and repayments

A product is a template. Acceptance freezes the lender, borrower, zone clock/calendar, currency, amount, security, interest rule, instalment rule, first due date, maturity, grace, recourse and disposal terms. Editing tomorrow's offer never rewrites yesterday's contract. Arrears are the overdue portion of the debt. Adding arrears to the payoff would charge twice.

| Minimum rule | Due at each agreed boundary | At maturity |
| --- | --- | --- |
| None | Zero. Voluntary payments welcome. | All remaining principal and interest form one balloon. |
| Interest only | Unpaid interest billed through that date. | Remaining principal and unpaid interest. |
| Interest plus principal | Billed interest plus a fixed principal instalment, capped at principal left. | Remaining balance. A smaller instalment may leave a disclosed balloon. |

Use simple interest on outstanding principal, with a rate **per named in-game interval**. For part of a period, accrue `principal × period rate × elapsed fraction of that actual interval`. Split the calculation at payments and amendments. Use decimal arithmetic, carry fractional remainders between postings and round final settlement once to the disclosed currency quantum. Cash products need a mintable quantum and exact cash/change at settlement. Dates use the zone reference clock and calendar with a `RecurringInterval` anchored at closing, including for uneven months. Unpaid interest accumulates separately. Defer compounding and late fees.

Pay oldest billed interest first, then oldest due principal, then unbilled accrued interest and finally principal prepayment. Track which instalments a payment satisfies. Early principal reduces future interest and may shorten the term. Future instalments still come due while principal remains. At an exact due boundary bill through that instant before applying a payment. Honour payments through the stated cure deadline. Stop interest at payoff, write-off, death or repossession. Catch up from the last processed boundary in bounded batches on restart. Bring a loan current through the relevant timestamp before quoting, accepting payment, enforcing or closing its financial period.

For Jo's 800-crown mortgage at 1% per month plus 100 principal, the first payment is 108. If Jo pays 60 on the due date, 8 covers interest and 52 reduces principal. The statement shows **748 principal left and 48 still due**. Paying the other 48 that day brings principal to 700. The next full month's instalment is 107. If Jo misses the 48, it retains its original deadline when the next bill is issued.

### Manager interventions

Use the bank manager check or the shop's manager/proprietor authority, scoped to the creditor. Staff cannot approve their own borrowing. Limit amounts, rates, pauses and write-offs by lender policy. Record actor, reason, old and new terms, timestamp and borrower acceptance.

A manager can also start a named offer for a present customer without a prior application. For example, `loan manage offer Jo` asks for the product, amount, security and any permitted variations, then sends Jo the terms and acceptance command. A manager's offer cannot debit Jo or bind Jo's property until Jo accepts.

- A **payment plan** proposes replacement dates and amounts. Show which arrears are rescheduled, what remains due and the new cure date. Acceptance supersedes future obligations and preserves payment history.
- A **pause** states its start/end, whether simple interest continues, when deferred sums fall due and whether maturity moves. Suspend enforcement during the pause. Resume once against the agreed dates. A manager may immediately grant more time before enforcement. A change in payment obligations requires borrower acceptance.
- A **refinance** quotes the old payoff and replacement contract side by side. Apply the new advance directly to the old balance, transfer security without an unpledged instant and disburse only explicitly agreed additional cash. Show any unpaid interest being rolled into principal and track it separately from cash received. Start with the same lender, borrower, currency and security. A superseded or expired offer changes neither contract.

## Collateral, default and recovery

**Pledged goods.** Record the persistent item ID, legal owner, description/condition and custody at acceptance. Require a whole held item with registered borrower ownership. Initially decline filled containers, currency, perishable or transforming goods and assets already held for another purpose. Tell the player why. Keep the original item in durable off-floor custody outside ordinary stock, employee access, sales, restocking and stack merging. Only the borrower or estate representative can redeem against the ticket. A missing item opens a dispute, suspends enforcement and prevents the lender collecting on lost security. Release custody to the borrower or their estate on settlement, with a durable pickup if immediate handover fails.

**Real estate.** Register the lien against the stable property ID and borrower's title, independent of an owner-row ID. The borrower keeps ownership, occupation and rent subject to existing leases. Disclose the lien on title inspection. Enforce it in the property domain for sales, divestment, auctions, scripted transfers, probate and builder changes. A sale or gift outside an authorised payoff, release or recovery explains how to clear the lien. Require lender consent to a new lease or discretionary extension that changes the security. Honour rights already granted to tenants.

At mortgage closing, lock the offer, current sale order, seller shares/consents, buyer contribution, lender funds and lien slot. Revalidate price, auction conflicts, keys and lease state. Pay the existing sellers in their recorded proportions to durable recipients, including bankless sellers whose old ownership rows will be retired. Transfer title and register the lien in the same recoverable operation. A second buyer, withdrawn consent or failed debit cannot leave an unsecured advance. Initially decline collateral where institutional foreclosure would invalidate control of a hosted shop, stable or other business. Ordinary property transfer can change staffing when an unleased property changes hands. Explain the exclusion before quoting and guard adding such a business during the pledge.

**From late payment to possession.** An unpaid scheduled amount becomes overdue and displays the shortfall and grace deadline. A balloon cannot miss an instalment before maturity. After grace the loan becomes eligible for enforcement. Default to an explicit manager action, with automatic enforcement optional on published terms. Recheck the balance, effective amendments, forbearance and probate hold under the same lock as payment. A payment that brings the loan current before repossession cancels eligibility. At repossession freeze the payoff and suspend ordinary collection during recovery. An unsecured default has no asset to seize. It remains eligible for payment, a plan, an estate claim or an authorised write-off.

**Possession and auction.** On secured default, transfer the item or property title to the lender **for recovery**, with the borrower's right to surplus recorded. Preserve tenancy, bonds, keys and access rules. Credit rent received during recovery against the frozen balance and then to surplus. List the same identified item or whole property through the existing auction house, with the recovery as dedicated payout target. Validate a compatible house before publishing an auction-backed product. A no-bid lot remains in lender custody for relisting. On a winning bid, normal property title transfers at completion or the buyer collects the actual item through auction pickup. An unclaimed winning item cannot be sold a second time.

If rent or an estate payoff clears the debt **before the sale commits**, cancel the recovery lot, refund any held bid, restore title and keys to the borrower or estate, and release the lien in one settlement. The public listing discloses this cancellation condition. Preserve tenancy and pay out any excess. Serialise that operation with auction completion. If the sale has already committed, the buyer keeps the asset and the former owner's remaining entitlement is settled in money.

Deduct the existing auction charges once. Apply the **cleared net proceeds** to outstanding interest, then principal. Return the surplus to the borrower or estate. A queued payout is money owed by the auction house. Track lot completion, cleared proceeds and buyer pickup separately. For Mara, five full weeks bring the frozen debt to 50 crowns. If the auction yields 70 after charges, the lender gets 50 and Mara gets 20. If it yields 35, the non-recourse pawn closes with a 15 write-off. A recourse contract would instead leave exactly 15 unsecured. Persist a collectible surplus if the recipient lacks a usable account.

For a setting without auctions, offer **retain in settlement**. State a cash-equivalent settlement value on the signed offer. At repossession the lender credits that value against the debt and must fund any surplus before taking unrestricted ownership. Write off any non-recourse shortfall. The pawnbroker can then stock the item for ordinary resale, or the bank can hold the real estate as its own property. For an auction-backed contract, switching to retention requires a fresh settlement agreement. If a house closes, allow reassignment to a compatible venue. A stranded case can also end through an audited funded settlement or release/write-off.

## Death and probate

The loan's zone owns its claim. At the death timestamp, freeze the debt, revoke unaccepted offers and look up loans by durable borrower identity. Discover off-body pledges from their custody records even if death occurred elsewhere. In an enabled zone, join the existing will or open estate, or create a case for an eligible deceased borrower with outstanding debt and no other assets. One loan never produces competing claims against several zone estates.

1. Register one system claim keyed by estate and loan, with the frozen payoff, any pledged asset, amounts already recovered and a loan-ledger link. A signed contract is automatically admitted. An estate manager can dispute the amount or title and resolve it with an audit reason. Dispute suspends finalisation. Repeated death notifications, discovery ticks and subsequent payments update the same claim.
2. Before **every** finalisation path, including manual finalisation and direct bequest, reconcile open loans, outstanding proceeds and security. Let the estate pay and release the asset. Otherwise designate one recovery owner. If a lender's auction is already running, probate waits for its settlement. If probate sells, it uses the same recovery operation. Exclude reserved collateral from a second listing or ordinary transfer to an heir. A will can pass the released asset or residual entitlement.
3. Give the secured loan priority **only over its own collateral**. Return surplus to the estate. Only a contractual deficiency or originally unsecured balance enters the general pool, behind existing secured claims and in the existing order among unsecured claims. Cap each payment against the unpaid balance. Payment in full releases the lien. An exhausted estate closes with an explicit shortfall/write-off. Heirs take no personal debt.
4. With probate disabled or the deceased ineligible, record the outcome. Contractual recovery against existing security may proceed after the agreed deadline. Preserve the cure deadline for an overdue loan and use maturity plus grace for a loan that was current. Hold any surplus or released item as unclaimed for the borrower. An administrator may record the world's succession decision, designate a recipient and issue a collectible receipt at the lender. Until then the entitlement stays on the books and the lender cannot appropriate it. Write off unrecoverable personal debt with an audit entry. For an eligible estate, death after payoff or sale but before pickup registers the released item or surplus as an asset. A lender manager dying or leaving office changes authority only. The establishment owns the receivable.

Today's targeted estate claims can be revalued to the asset value. Existing finalisation orders approved claims against pooled cash by secured flag and date. A 200-crown mortgage must remain a 200-crown debt even if the building is valued at 1,000. A stable loan link, priority limited to pledged proceeds, and the finalisation check are all necessary for secured lending.

## Money, bank property, reporting and taxes

Use funded lending. As a conservative bank default, cap a new advance at same-currency reserves less positive deposit obligations, committed payouts not already counted and a configured liquidity floor, with a minimum of zero. Recheck exposure caps at closing. A shop chooses either its available shop cash balance or a same-currency shop-owned account at setup. Lending cannot draw an overdraft. Respect actual and expected shop cash on cash settlement. No customer deposit or manager's personal account is an institutional lending budget.

| Event | Balance-sheet treatment | Reporting |
| --- | --- | --- |
| Cash advance of 100 | Bank reserves or shop cash fall 100 and principal receivable rises 100. Borrower gains 100 cash and owes 100. | Financing movement, zero sales or profit. |
| Bank advances 100 into its customer's deposit | Receivable and deposit liability each rise 100. Physical reserves stay level. For a destination at another bank, settle reserves between the banks and credit the destination once. | Report the actual deposit and changed reserve coverage. The receivable adds no money supply. |
| Repayment of 25 principal and 3 interest | Reduce principal by 25. Collect 28 in cash or extinguish 28 of same-bank deposits. Interbank payments settle reserves in the other direction. Clear interest receivable of 3. | Principal is a financing return. Only 3 is realised interest income. |
| Repossession, auction or retention | Show assets held for recovery separately from free bank property or shop merchandise. On sale clearing, settle the receivable and record any surplus payable. On retention, exchange the receivable for an owned asset at its settlement value and fund surplus. | Count the property or item once. Auction proceeds have one exchange event. Later ordinary resale of a retained asset uses its settlement value as cost basis. |
| Write-off, pause or refinance | Distinguish unrecovered advanced capital, unpaid interest and interest rolled into a new balance. A pause moves no cash. A refinance links old and new balances and retains the origin of capitalised interest. | Separate principal loss from interest never received. Rollover by itself produces zero realised interest or new spendable funds. |

**Bank manager balance.** Report liquid reserves, customer deposit liabilities, principal outstanding, accrued interest receivable, losses, restricted recoveries, unrestricted property and surplus payable by currency. Add recognised receivables and unrestricted property at carrying value to net position, subtract liabilities, and keep recovery appraisal values informational until realised. Origination alone cannot improve net position. Show arrears and default separately so managers can distinguish a valuable loan book from cash available for withdrawals.

**Economy analytics.** Correlate financing legs with one operation ID. Count the underlying property or auction exchange once even when financed. Show outstanding principal, arrears, interest and realised losses by lender, zone, currency and period. Exclude loan receivables, liabilities and collateral appraisals from broad money and reserve coverage. For net wealth, subtract borrower debt once and attribute the receivable to the lender. Show property gross and equity distinctly. Protect legacy gross measures and historical snapshots. A refinance without a cash leg has zero external payment volume. Detailed holdings retain existing permissions.

**Tax policy.** Proposed basis is cash-realised lending income. Where enabled by a zone, feed cash interest into lender profit, including rolled interest only when actually collected. Permit realised advanced-principal losses as deductions only when configured. Principal, deposits, uncollected interest, appraisals and surplus payable contribute no taxable turnover or profit. An ordinary subsequent shop sale follows its applicable taxes. Keep sale tax and auction charges attached to the actual transaction, charged once. For shops, feed financing adjustments into the existing period result separately from retail turnover. Banks currently lack an equivalent profit-tax subject. Add a bank lending period result and explicitly applicable bank profit-tax policy/adapter before offering bank taxation. Existing shop-specific progs retain shop inputs. Attribute realised events once to a financial period and post late corrections as audited adjustments. A world choosing no lending tax still gets accurate books.

## Implementation boundaries

| Current FutureMUD behaviour | Design implication |
| --- | --- |
| Banks persist reserves by currency, customer accounts and manager authority. Shop credit finances purchases from that shop. | Give the loan service a bank or shop funding adapter. Leave overdrafts and purchase credit on their existing lifecycles. [Bank](../../MudSharpCore/Economy/Banking/Bank.cs) · [Bank account](../../MudSharpCore/Economy/Banking/BankAccount.cs) · [Shop](../../MudSharpCore/Economy/Shops/Shop.cs) · [Shop credit](../../MudSharpCore/Economy/Shops/LineOfCreditAccount.cs) |
| The conveyancer collects payment, credits owners by share and calls `SellProperty`. Direct title transfer and divestment also exist. | Extract a recoverable closing operation and guard the lien inside the property domain. [Property commands](../../MudSharpCore/Commands/Modules/PropertyModule.cs) · [Property and title](../../MudSharpCore/Economy/Property/Property.cs) |
| Auctions already handle items and property shares, immediate or deferred seller payments, property transfer and item pickup. The deferred payout aggregates by recipient. | Persist the recovery ID on a lot and individual payable. Add a loan-aware proceeds recipient and an unsold route to institutional custody. [Auction settlement](../../MudSharpCore/Economy/Auctions/AuctionHouse.cs) |
| Estates discover zone assets, create claims for overdrafts/fees and purchase credit, and resolve claims or distribute assets in kind. | Add stable claim identity, a debt amount independent of appraisal, and hooks at death, asset capture and finalisation. [Estate runtime](../../MudSharpCore/Economy/Estates/Estate.cs) · [Probate and item ownership](Estate_Probate_and_Item_Ownership.md) |
| Zone financial close aggregates shop transactions. Economy snapshots distinguish deposits, overdraft debt, reserves, cash and property. | Extend period results, activity and holdings at their owning services. Label when historical loan coverage begins. [Economic zone](../../MudSharpCore/Economy/EconomicZone.cs) · [Analytics](../../MudSharpCore/Economy/Analytics/EconomyAnalyticsService.cs) · [Related inter-business finance proposal](Employment%20Hosts%20Implementation%20Review%20and%20Roadmap.md) |

**Builder workflow.** Select the bank or shop, enable lending, choose its funding source and floor, then create one product. Choose its advance limits, approval mode, term and minimum payment. Choose security and disposal only for a secured product. Preview a sample offer and repayment schedule before publication. For example, a shop can publish just a four-week, cash-only, non-recourse pawn with agreed retention value. It can operate entirely from the shop counter. Disabling a product withdraws unaccepted quotes and leaves signed terms available for servicing.

**Records and integrity.** Put shared contracts under the library's economy namespace, lifecycle and commands in core, and additive mappings/migrations in the database library. Persist lender settings/products, an offer and its accepted terms, versioned amendments, dated obligations, financial postings with principal/interest allocations, and a collateral record with custody, recovery, estate claim and pickup/payout references. Use durable borrower identity independent of the live body. Index open debt by borrower/zone, next action by zone/date, portfolio by lender/status. Enforce uniqueness for active pledge per asset, active lot per recovery, claim per estate/loan and each operation/posting key. Reconcile balances to the immutable journal. Existing worlds start with empty loan tables.

Origination, payment, refinancing, seizure and clearing each need one settlement across the affected loan, funds, title and custody records. Existing helpers that save independently cannot simply be chained. Make them join the transaction or persist a resumable operation with reserved assets/funds and idempotent steps. While recovery is pending, another buyer, creditor or collector cannot consume those assets. Synchronise in-memory state and send receipts after commit. Timed and manual actions share the same operations. Keep receipt, state and amount so a crash after debit, title transfer, auction completion or confirmation recovers to one outcome.

**Delivery order.** First implement opt-in bank lending, repayment, manager amendments, unsecured probate and accounting. Then add pawn custody, shop lending, recovery settlement and secured probate together. Introduce purchase mortgages and foreclosure after title, key, tenancy and atomic closing checks pass. Publish a product type only once players can service it through payoff, death or default.

## Acceptance scenarios for implementation

1. With lending off, existing banking, shop credit, property and probate still work. Enabling one lender shows only its published products. Retiring its last product leaves payment, pickup and pending estate claims operational.
2. A bankless pawnbroker lends 40 actual crowns against one ring. A second pledge, stock sale or unauthorised pickup fails. Redeeming after logout returns the same item once. Exercise cash shortfall, auction surplus, non-recourse loss and retention followed by retail resale.
3. Two buyers attempt the same financed purchase. One obtains title encumbered once and the sellers receive their shares once. Change the price, exhaust funds, or interrupt closing and reconcile funds, title, keys and lien. Direct divestment and auction listing cannot bypass security. An existing tenant keeps their lease through foreclosure. Let rent clear the debt while a bid is held and verify one refund, one restoration of title and the correct surplus.
4. Exercise interest-only, principal-plus-interest and balloon terms, partial and early payments, uneven months, grace at the boundary, both pause policies and restart catch-up. Compare against continuous operation. Replaying a refinance or enforcing concurrently with payment produces one outcome.
5. Test death with an off-body pawn, a prior will, an existing auction, no other assets, and a second zone estate. Exercise probate-disabled death, dispute, a deferred auction payout and an insolvent estate. No bequest or second lot consumes reserved collateral. An unrelated creditor can reach the surplus and has no claim on the secured portion.
6. Reconcile cash, same-bank deposit, interbank transfer, financed sale, interest collection, refinance, retention/resale and write-off across reserves, deposits, receivables, borrower debt, tax, wealth and exchange volume. Replay sale clearing and financial close after interruption and verify no duplicated cash, tax, item or title.

### Reproducing a late settlement failure

Use a disposable MySQL world with a borrower holding cash, one repayable loan and an approved purchase mortgage. Record the coin pile IDs and denominations, bank reserves, seller balances, title shares, sale order, contract status, lien and posting counts. Using a seller separate from the lender makes each money movement measurable.

Install this trigger in the test schema. Substitute the repayment and mortgage contract IDs for 101 and 102.

```sql
DELIMITER //
CREATE TRIGGER reject_test_loan_posting BEFORE INSERT ON LendingPostings
FOR EACH ROW
BEGIN
    IF (NEW.ContractId = 101 AND NEW.Kind = 'Payment')
       OR (NEW.ContractId = 102 AND NEW.Kind = 'Advance') THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Injected settlement failure';
    END IF;
END//
DELIMITER ;
```

1. Run `loan pay 101 all` and `accept payment` at the lender. At the conveyancer run `property buy "<property name>" loan 102`. Both commands should report failure.
2. Allow the normal save queue to run and reconnect. The borrower must retain the same cash and debt. The sellers must retain their title shares and sale order. Compare reserves, seller balances, liens and postings against the recorded baseline.
3. Run `DROP TRIGGER reject_test_loan_posting` and retry both commands. The payoff should debit the borrower and credit the lender once. The closing should debit the contribution and advance, credit each seller's share, transfer the title and activate one lien. Repeat the confirmation or closing and verify all balances and posting counts again.
4. Repeat the mortgage with `from <account>` to include withdrawal fees, account history and paying-bank reserves. For cash change, include a payment that consumes an entire foreign-owned pile and verify that the borrower owns the returned change.
