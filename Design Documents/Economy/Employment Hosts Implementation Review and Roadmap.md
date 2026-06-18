# Employment Hosts Implementation Review and Roadmap

**Status:** Proposed follow-up design and implementation roadmap  
**Reviewed against:** `master`, 18 June 2026  
**Primary companion document:** [Unified Employment, Manager Goals, and Task Dispatch Design](./Unified%20Employment%20Dispatch%20Design.md)

## 1. Purpose

This document records a code-level review of the current Employment Host implementation, identifies places where the implementation does not yet satisfy the intended design semantics, and defines the recommended implementation sequence for completing and extending the system.

It is deliberately a companion to the original unified employment design rather than a replacement for it. The original document remains the architectural baseline. Where this document makes an explicit corrective decision, that decision should be incorporated into the original design when the relevant implementation work is undertaken.

The review considered the following user journeys through the design and implementation:

- a manager creates an employment opening, an NPC applies, and the manager hires them;
- a scheduled rule creates replenishment work and an NPC purchases and delivers stock;
- a manager reprices merchandise or runs a temporary sale;
- an employee works a scheduled shift and earns hourly wages;
- an employee is paid in cash or to a bank account;
- a manager goal creates and prioritises operational work;
- a parent organisation or clan owns and manages multiple subordinate businesses.

This is a static source review. It does not assert that the full repository currently compiles or that every runtime path has been exercised against a live database.

## 2. Current implementation assessment

The implementation is substantial and follows the intended architecture in many important respects.

The current system includes:

- a shared `IEmploymentHost` contract and durable `IEmploymentHostState`;
- employment contracts, explicit authority, openings, applications, and historical termination state;
- persisted host boards, operational register entries, employment ledger entries, and payroll liabilities;
- action plans, active tasks, scheduled rules, reusable conditions, expressions, predicates, and templates;
- manager goals that create work through the task board;
- `EmploymentWorkerAI` job search, application, host selection, task claiming, pathing, and execution;
- native or partially native integrations for shop purchases, bank movements, store accounts, tax payments, physical and register float, crafting, merchandise repricing, and opening administration;
- host adoption for shops, auction houses, combat arenas, banks, stables, and hotels;
- normalized persistence for the common employment spine;
- custody, host-location, authority-loss, and blocked-task protections added after the initial logistics implementation.

The major architectural decisions remain correct:

1. Task routing belongs to `IEmploymentTaskBoard`; the host `IBoard` is a communication surface only.
2. Employment relationships are explicit historical records rather than scattered flags.
3. Authority is delegated and should be checked at authoring, assignment, and execution boundaries.
4. Financial and inventory actions should call the subsystem that owns the affected state.
5. Employment audit evidence should correlate native records rather than replace native accounting.
6. Legacy `IJobListing` and `IActiveJob` remain separate until a deliberate bridge is designed.

The principal concern is that several implemented domain types are currently more expressive than their runtime semantics. Schedules, duration, payment methods, market-linked pay, task priority, and goal priority exist in the model, but important runtime paths either ignore or simplify them.

## 3. Corrective design decisions

The following decisions are normative for future Employment Host work.

### 3.1 Employment automation must never create or modify a Market Influence

`MarketInfluence` represents regional or macroeconomic pressure. It is an inappropriate target for a shop employee, scheduled rule, or manager goal.

Employment automation may:

- read market prices and market multipliers as conditions or planning inputs;
- reprice merchandise owned by the employing shop;
- create, modify, expire, or cancel a native shop deal or sale;
- post a recommendation for a privileged policy actor.

Employment automation must not:

- create a `MarketInfluence`;
- modify a `MarketInfluence`;
- make a permanent regional price change through a local business task;
- use a shop manager's `AdjustPrices` authority as authority over the market subsystem.

Market influences should remain builder-, implementor-, world-event-, government-policy-, or explicitly privileged FutureProg operations.

### 3.2 A payable is not settled until value reaches the employee or funded escrow

Employer debit alone is not wage payment.

A wage payable may only become paid or settled when one of the following is true:

- the employee's nominated bank account has been credited through the native bank subsystem;
- physical cash has been placed into a durable payroll escrow and is claimable by the employee;
- the payment item has been created or transferred to the employee;
- another explicitly supported payment adapter has completed both sides of the transfer.

A failed destination payment must leave the payable outstanding. It must not silently close the liability.

### 3.3 Hourly pay must be based on worked or guaranteed hours

Contract age is not worked time. An hourly contract must not accrue twenty-four hours of wages merely because one in-game day elapsed.

Hourly earnings must be based on durable attendance or timesheet evidence, or on a clearly modelled guaranteed-hours term. Schedules and attendance therefore become prerequisites for correct hourly payroll.

### 3.4 Automated authority must be explicit rather than represented by `null`

Scheduled rules, goals, and host system operations should use an explicit employment principal and durable authorisation grant. `null` must not mean an unlimited trusted actor.

### 3.5 Priority must affect actual runtime ordering

Priority is not useful if it is only persisted or displayed. Goal evaluation, task creation, task claiming, and concurrency decisions must use it consistently.

### 3.6 Ownership and employment are related but distinct

A clan or organisation that owns a business is not automatically the same employment host as that business. Parent organisations and operating units require an explicit relationship and scoped authority model.

## 4. User-story walkthrough and findings

## 4.1 Opening, application, and hire

### Current path

1. A manager creates an opening through `EmploymentHostState.CreateJobOpening` or the command adapter.
2. `EmploymentWorkerAI` enumerates host openings, applies candidate matching, host reputation, pathing, and raw nominal-pay ordering.
3. The AI creates an application through `EmploymentHostState.Apply`.
4. A manager accepts the application.
5. `EmploymentHostState.AcceptApplication` creates a contract from the opening's current terms.

### Implemented successfully

- manager authority is checked;
- opening capacity is checked per opening;
- candidate requirements can be represented;
- reservation wage and accepted payment methods are considered;
- application and hire records are persisted and audited;
- manager acceptance creates a real active contract.

### Gaps

- The application does not snapshot or version the offered terms. Acceptance uses whatever terms the opening has at acceptance time.
- Acceptance does not explicitly require the opening to remain open and accepting applications.
- Candidate requirements are not re-evaluated at acceptance.
- Duplicate pending applications are prevented by worker behaviour, not by a domain or database invariant.
- `NpcApplicationsOnly` is represented but is not consistently enforced as a domain rule.
- Position occupancy is based on accepted applications rather than current contracts occupying the opening. A terminated hire can continue consuming a position permanently.
- Worker AI candidate profiles currently provide empty skill, knowledge, and tag collections.
- Offer ranking and reservation-wage comparison use raw nominal amounts without normalising hourly, daily, weekly, salary, commission, or mixed pay.
- Payment selection exists as a helper, but acceptance does not reliably resolve and persist the employee's actual destination account.

### Required changes

Introduce opening revisions and application snapshots:

```text
EmploymentOpeningRevision
- OpeningId
- RevisionNumber
- Requirements
- Compensation
- Schedule
- Duration
- PaymentPolicy
- Authority
- ApplicationPolicy
- CreatedAt
- CreatedBy

EmploymentApplication
- OfferedRevisionId
- TermsSnapshot
- CandidateDecisionState
- CandidateConsentAt
```

Acceptance must atomically:

1. verify the opening is open and accepting applications;
2. enforce the configured concurrent-seat policy;
3. re-evaluate the candidate;
4. resolve the actual payment destination;
5. require renewed consent if material terms changed;
6. create the contract with origin application, opening, and revision references;
7. mark the application accepted.

For normal openings, `MaxPositions` should mean concurrent occupied seats unless the opening is explicitly configured as non-replenishing. Ending a contract should release its seat.

## 4.2 Scheduled stock replenishment

### Current path

A scheduled rule can evaluate stock or commodity conditions, create an active task, reserve or authorise funds, purchase through the native shop sale path, take custody of items, and deliver them to a host location.

### Implemented successfully

- scheduled rules are durable and idempotent;
- rich condition expressions are supported;
- active tasks preserve per-step operational state;
- worker assignment validates contract, authority, capability, and next-step execution;
- purchases use native shop paths where supported;
- physical custody is tracked and audited;
- host-location boundaries are enforced for physical logistics;
- tasks can block safely for manager review.

### Gaps

- Workers select pending tasks alphabetically by task name rather than by priority, due time, age, or operational urgency.
- The system does not model host-wide concurrency, spending, storage, or supplier budgets.
- A blocked task can remain indefinitely without an explicit retry or escalation policy.
- One assigned employee is expected to perform the whole action plan even when approval, purchasing, driving, crafting, and delivery ought to be separate roles.

### Required changes

Add task provenance and policy fields:

```text
EmploymentActiveTask
- SourceKind
- SourceRuleId
- SourceGoalId
- CreatedByPrincipal
- AuthorisedByPrincipal
- AuthorisationGrantId
- Priority
- DueAt
- CreatedAt
- AssignedAt
- StartedAt
- CompletedAt
- RetryPolicy
- EscalationPolicy
```

Workers should claim tasks by:

1. priority descending;
2. due time ascending;
3. creation time ascending;
4. stable task identifier.

Host policy should cap concurrent tasks, reserved funds, purchase volume, and repeated retries.

## 4.3 Merchandise repricing and temporary sales

### Current path

The existing `price` action supports exact merchandise base-price changes and a market mode that creates or updates a `MarketInfluence`.

The market mode is the wrong implementation and must be removed.

### Replacement design

Split local pricing into explicit shop-owned actions.

#### Permanent merchandise repricing

```text
price merch <merchandise> exact <amount>
price merch <merchandise> change <signed percentage>
price merch <merchandise> margin <target percentage>
```

Required safeguards:

- shop host only;
- merchandise must belong to that shop;
- lower and upper price bounds;
- maximum percentage movement per evaluation;
- optional minimum gross margin;
- cooldown and hysteresis for scheduled use;
- native shop transaction or price-adjustment evidence;
- employment register correlation.

#### Temporary sale

Use the existing `IShopDeal` and `ShopDeal` subsystem:

```text
sale create <name>
  target all | merchandise <which> | tag <which>
  adjustment <signed percentage>
  applies sell | buy | both
  eligibility <prog | none>
  cumulative <true | false>
  expires <datetime | duration>
```

#### Volume deal

Use native `ShopDealType.Volume` with a minimum quantity rather than mutating merchandise prices.

### Persisted-data treatment

Existing persisted market-mode employment price steps must not be silently reinterpreted.

A migration or startup compatibility pass should:

- pause scheduled rules containing a market-mode price action;
- mark affected goals as requiring builder review;
- block affected active tasks with a deprecation diagnostic;
- preserve the original payload for inspection;
- write an employment register entry;
- provide an implementor report listing host, market, category, impacts, influence name, and expiry.

Exact merchandise repricing steps may remain loadable if their target and safety limits remain valid.

### Invocation policy

Action metadata should state where an action can be used:

```csharp
[Flags]
public enum EmploymentActionInvocationSource
{
    ManualTask = 1,
    ScheduledRule = 2,
    ManagerGoal = 4,
    HostSystem = 8
}
```

An action being executable must not automatically make it schedulable or goal-authorable.

## 4.4 Scheduled shift and hourly wages

### Current path

Contracts persist a `WorkSchedule`, `EmploymentDuration`, compensation terms, and payment method. Payroll accrues periodic payables based on elapsed time since contract start or the prior payable.

### Critical gap

For hourly pay, the current period is one day and the amount is the hourly rate multiplied by all elapsed hours. This treats every hour of contract existence as worked time.

Schedules and duration are currently mostly descriptive. They do not determine whether an employee is on duty, how much time was worked, or when a fixed-term contract expires.

### Required earning model

Introduce durable earning evidence:

```text
EmploymentTimeRecord
- ContractId
- EmployeeId
- ShiftDefinitionId
- ScheduledStart
- ScheduledEnd
- ActualStart
- ActualEnd
- ScheduledDuration
- WorkedDuration
- ApprovalState
- Source
- CorrelationId

EmploymentEarning
- ContractId
- EarningType
- Quantity
- Rate
- Amount
- Currency
- EarningPeriodStart
- EarningPeriodEnd
- SourceTaskId
- SourceTransactionId
- CorrelationId
```

Pay cadence rules:

- `Hourly`: approved worked or guaranteed hours multiplied by the hourly rate.
- `Daily`: qualifying day or shift, not arbitrary elapsed time.
- `Weekly`: host-calendar week and configured work obligations.
- `Salary`: host-calendar pay period with clear proration rules.
- `PerTask`: earning created on qualifying task completion.
- `Commission`: earning created from attributable native transactions.
- `Mixed`: composed fixed, hourly, task, and commission components.
- `Unpaid`: no payable, but attendance and task attribution may still be recorded.

Unsupported compensation combinations must be rejected at opening or contract creation rather than producing zero or misleading payroll.

### Operational schedule

Replace the current descriptive schedule with a recurring schedule capable of expressing:

- host calendar, clock, and timezone;
- weekdays or recurring calendar intervals;
- one or more shifts per period;
- overnight shifts;
- flexible, on-call, or always-available work;
- grace periods;
- required versus optional attendance;
- guaranteed hours where applicable.

Worker AI should claim ordinary work only while on duty. Emergency or on-call tasks should be explicitly marked.

### Contract lifecycle

Add central evaluation for:

- fixed-term expiry;
- seasonal activation and expiry;
- task-limited completion;
- suspension and resumption;
- host closure;
- renewal.

Ending a contract must continue preserving historical records and outstanding liabilities.

## 4.5 Cash and bank wage payment

### Current path

Payroll settlement validates and debits employer funds. Cash payables for active employees may become claimable. Other payables may be marked settled without a real destination transfer.

### Required state machine

```text
Accrued
-> FundingReserved
-> Funded
-> ReadyToClaim       // physical cash escrow
-> Disbursing         // account or item payment
-> Paid

Any state may move to Failed with a durable reason.
A failed payment remains a liability.
```

### Payment adapters

Define a service boundary such as:

```csharp
public interface IEmploymentWageDisbursement
{
    PaymentMethodKind MethodKind { get; }
    bool CanDisburse(EmploymentPayable payable, out string reason);
    EmploymentDisbursementResult Disburse(EmploymentPayable payable, EmploymentPrincipal principal);
}
```

Implementations:

- **Cash:** debit the employer into durable payroll escrow. Active or former employees can claim it. Claiming creates physical currency and consumes escrow.
- **Employee bank account:** validate the account, debit the employer, and credit the employee through native account transactions.
- **Specified bank account:** validate the named destination, ownership or authorisation, currency, and account status.
- **Payment item:** create or transfer the actual item; retain the payable on failure.
- **Employer float:** issue custody-tracked value with reconciliation obligations.

`PaymentSource` must select the employer-side asset. It must not remain passive metadata.

### Atomicity and idempotency

Each payable disbursement must use an idempotency key derived from the payable or correlation ID.

The transaction sequence is:

1. validate the destination;
2. reserve employer funds;
3. execute both sides of the transfer;
4. commit native financial evidence;
5. mark the payable paid;
6. write employment audit evidence.

Failure must release or compensate reservations and leave the payable unpaid.

### Employee-specific arrears

Add:

```text
OutstandingLiabilitiesFor(employeeId)
MaximumOverdueDaysFor(employeeId)
EmployerReliabilityScore(currency, period)
```

An employee should resign based on their own overdue wages. Host-wide arrears may still affect prospective-worker reputation.

## 4.6 Manager goals

### Current path

Goals persist type, configuration, required authority, status, priority, cadence, last evaluation, and correlation. Evaluation creates a goal-scoped active task and suppresses duplicates while an existing task remains active or blocked.

### Gaps

- goals are not evaluated in priority order;
- priority is not propagated into tasks;
- goal-created work uses an implicit system actor;
- goals have limited state beyond active or cancelled;
- broad manager autonomy is not implemented;
- goal evaluation currently depends in part on employed worker activity rather than a dedicated host operations loop;
- goal conditions use flat `All` semantics rather than the full scheduled-rule expression model.

### Required changes

Add goal states such as:

```text
Active
Satisfied
Blocked
AwaitingApproval
Failed
Paused
Cancelled
```

Add:

- rich condition expressions;
- budget and risk constraints;
- explicit assigned manager role or contract;
- durable authorisation grant;
- priority propagation;
- escalation after repeated failure;
- controlled choice between alternative action plans.

A manager AI must continue to create authorised operational work rather than mutating economic subsystems directly.

## 5. Cross-cutting remediation plan

## 5.1 Explicit principals and authorisation grants

Replace nullable actors with:

```text
EmploymentPrincipal
- Character
- EmploymentContract
- ScheduledRule
- ManagerGoal
- HostSystem
- Implementor
```

A durable `EmploymentAuthorisationGrant` should contain:

```text
- IssuerPrincipal
- OwningHost
- PermittedActionKinds
- Scope selectors
- Maximum amount and currency
- Payment source
- Supplier or counterparty restrictions
- ValidFrom
- ExpiresAt
- CanRedelegate
- Revocation state
- CorrelationId
```

Manual tasks, scheduled rules, goals, and parent organisations should issue grants. Worker steps consume grants. A worker completing an `authorise` step must not be treated as managerial approval.

## 5.2 Central host operations scheduler

Create a central `EmploymentHostOperationsScheduler` responsible for:

- scheduled-rule evaluation;
- manager-goal evaluation;
- payroll accrual;
- contract lifecycle evaluation;
- opening occupancy reconciliation;
- stale authorisation and reservation cleanup;
- blocked-task retry and escalation;
- host-wide evaluation locking and idempotency.

Worker AI should be responsible only for:

- employment search and application;
- attendance or duty reporting;
- selecting among active contracts;
- claiming and executing work;
- travel and custody;
- claiming physical pay.

This avoids duplicated evaluation paths and allows a business with no currently ticking worker to continue accruing payroll, expiring contracts, and evaluating management policy.

## 5.3 Worker matching and multi-employer behaviour

Complete candidate and worker profiles with:

- real skills;
- real knowledges;
- world-specific tags or traits;
- schedule availability;
- normalised effective compensation;
- travel burden;
- employer reliability;
- accepted duties and host types;
- personality and preference modifiers;
- fatigue or capacity where supported.

A worker with multiple contracts should choose a host based on current duty, assigned work, priority, commute, and availability rather than host name.

## 5.4 Host capability providers

The current implementation requires multiple type switches for discovery, locations, finance, tax, and workplace selection. Before introducing many new hosts, define registries and capability providers:

```text
IEmploymentHostRegistry
IEmploymentLocationProvider
IEmploymentFinanceProvider
IEmploymentInventoryProvider
IEmploymentClockProvider
IEmploymentTaxProvider
IEmploymentAccountingProvider
IEmploymentWorkplaceProvider
```

Use an extensible host key:

```text
EmploymentHostKey
- ProviderType
- ProviderId
```

Do not assign multiple unrelated future systems to `EmploymentHostType.Other` without a stronger discriminator.

## 5.5 Security regression baseline

Preserve regression coverage for the previously remediated employment security classes:

- arbitrary source and destination cells;
- specific live item retrieval outside host control;
- remote purchasing from inaccessible shops;
- count-priced purchases of commodity merchandise;
- personal employee cash being mistaken for task float;
- arbitrary return or delivery containers;
- duration and cooldown overflow;
- custody loss after reload, theft, destruction, firing, death, or authority loss;
- cross-host resource access;
- blocked-task retry loops.

The host boundary should become a formal capability policy rather than remain an incidental collection of host-specific checks.

## 6. Remaining original-design implementation epics

The following sequence supersedes a purely feature-by-feature approach. Correct employment semantics must precede broader automation.

## Epic 1: Employment semantics closure

Deliver:

- operational schedules and attendance;
- duration expiry and suspension;
- cadence-correct earnings;
- market-rate wage resolution;
- real payment disbursement;
- application revisions and seat lifecycle;
- task provenance and priority;
- explicit system principals;
- employee-specific arrears;
- central host scheduling.

## Epic 2: Native finance closure

Deliver:

- account-to-account transfers;
- host-to-host settlements;
- physical payroll reserve and escrow;
- employee-float issue, reconciliation, and recovery;
- paired transaction correlation;
- currency conversion policy;
- atomic reservations and disbursements;
- authorisation amount and counterparty limits.

Employment ledger entries remain correlating audit evidence, not the canonical general ledger.

## Epic 3: Craft and production operations

Deliver:

- station capacity and queueing;
- priority and multiple-worker scheduling;
- production chains;
- intermediate output custody;
- failed-craft recovery and salvage;
- inspection and quality-control steps;
- materials replenishment;
- demand- and storage-aware batch sizing.

## Epic 4: Vehicle, animal, and route logistics

Deliver:

- driver and vehicle assignment;
- animal leading, riding, lodging, and return;
- cargo capacity and compartment selection;
- persisted route plans;
- multi-stop batching;
- loading and unloading reservations;
- fuel, feed, maintenance, and rest policies;
- recovery when a worker, vehicle, animal, or destination becomes unavailable.

## Epic 5: Manager autonomy and administrative services

Deliver:

- goal priority, budgets, and risk limits;
- goal satisfaction and failure states;
- rich goal expressions;
- controlled scheduled-rule administration;
- controlled task retry, reassignment, and cancellation;
- controlled goal mutation;
- choice among purchasing, crafting, transfers, local repricing, sales, or accepting shortage;
- human-manager escalation when no authorised plan is viable.

Recursive administrative actions must call the owning task, rule, or goal service and preserve complete provenance.

## Epic 6: Deeper host-specific workflows

### Shops

- temporary sales and volume deals;
- bounded local repricing;
- supplier management;
- stocktake and discrepancy handling;
- cash reconciliation;
- internal and external stock transfer.

### Auction houses

- lot intake and custody;
- valuation and listing;
- auction scheduling;
- commission collection;
- seller payout;
- buyer settlement;
- won-lot delivery.

### Combat arenas

- event scheduling;
- competitor intake;
- purses and prizes;
- security and crowd tasks;
- stable and equipment preparation;
- medical and cleanup workflows.

### Banks

- teller staffing;
- cash and reserve balancing;
- inter-branch courier work;
- account-service tasks;
- branch-specific staffing and controls.

### Stables

- feeding, grooming, exercise, and inspection;
- animal transfer and return;
- feed replenishment;
- ticket and account reconciliation;
- capacity and welfare goals.

### Hotels

- cleaning and room readiness;
- furnishing replacement;
- check-in and check-out support;
- patron-balance collection;
- lost-property handling;
- consumable replenishment;
- maintenance escalation.

## Epic 7: Candidate and labour-market depth

Deliver:

- real skills, knowledges, and tags;
- schedule and commute preferences;
- wage normalisation;
- employer reputation;
- retention and resignation choices;
- training and progression;
- local labour availability and reservation-wage inputs.

## Epic 8: Legacy bridges

Keep this optional and separately scoped:

- report legacy/new employment divergence;
- bootstrap selected legacy staff into contracts where useful;
- replace legacy commands only after equivalent common services exist;
- preserve the `IJobListing` and `IActiveJob` boundary until a dedicated convergence design is approved.

## 7. Parent organisations, clans, and multi-business employment

## 7.1 Recommended split

Introduce `IEmploymentOrganisation` alongside `IEmploymentHost`.

An employment organisation is the legal, political, or controlling entity. An employment host is an operating unit where work is performed.

Examples of organisations:

- clan;
- character-owned company;
- government department;
- guild, temple, trust, family, or partnership;
- future corporate entity.

Examples of operating hosts:

- shop;
- hotel;
- stable;
- warehouse;
- property-management office;
- workshop or factory;
- farm or mine;
- bank branch.

An organisation may also be an employment host when it employs headquarters staff such as directors, treasurers, accountants, stewards, quartermasters, or regional managers.

## 7.2 Proposed data model

```text
EmploymentOrganisation
- OrganisationType
- OrganisationId
- Name
- TreasuryPolicyId
- DefaultEconomicZoneId
- EnabledAsEmploymentHost

EmploymentBusinessUnit
- OrganisationId
- ChildHostKey
- OwnershipShare
- ControlShare
- ControlMode
- EffectiveFrom
- EffectiveTo
- WorkingCapitalFloor
- SurplusSweepThreshold
- DeficitFundingLimit
- TaxPolicyId

EmploymentScopedAuthorityGrant
- IssuerPrincipal
- HolderContractId or ClanAppointmentId
- ScopeKind
- ScopeSelector
- Authorities
- AmountLimit
- CurrencyId
- ValidFrom
- ExpiresAt
- CanRedelegate
```

Ownership share and control share must be separate. A minority owner may receive distributions without controlling employment. A controller may manage a business under lease or appointment without owning all economic benefit.

## 7.3 Parent-level management

A controlling organisation may:

- appoint or remove proprietors and managers in controlled subsidiaries;
- grant regional authority over selected child hosts;
- view consolidated staffing, payroll, liabilities, cash, inventory, and blocked work;
- create goals scoped to a child, subtree, host type, or business tag;
- fund payroll or working-capital deficits;
- approve capital expenditure;
- sweep distributable surplus;
- arrange inter-business loans;
- coordinate procurement and stock transfers;
- escalate manager vacancies or persistent losses;
- close, sell, acquire, or restructure a business unit.

Operational work must still be created on the child host's task board. The parent supplies a scoped authorisation grant. It does not bypass child-host controls.

## 7.4 Clan integration

Clan membership and clan appointment are not automatically employment contracts.

A clan appointment may grant organisational authority without wages. A paid officer should have both an appointment or scoped grant and an employment contract.

Existing clan budgets and treasury facilities can provide spending ceilings, but an employment task should consume an explicit authorisation grant linked to the relevant appointment budget rather than receiving unrestricted treasury access.

Example mappings:

- clan treasurer appointment: organisational finance authority, possibly unpaid;
- paid chief financial officer: parent-host employment contract plus finance grant;
- regional manager: parent employment contract plus subtree-scoped operating authority;
- shop proprietor: child-host contract created or controlled by the parent organisation;
- honorary office: clan role only, no employment or spending authority.

## 7.5 Inter-business finance

Add explicit transaction kinds:

- capital contribution;
- dividend or profit distribution;
- intercompany loan;
- loan repayment and interest;
- management fee;
- shared-service charge;
- inventory transfer;
- tax payment.

Every intercompany movement must produce paired entries with a common correlation ID. Consolidated reporting must eliminate internal transfers so moving money between sibling businesses does not create artificial revenue or profit.

Payroll and statutory liabilities must be protected before surplus sweeps.

## 7.6 Profit tax and business accounting

Profit tax must not be calculated from the employment ledger. The employment ledger is incomplete as a profit-and-loss source.

A proper accounting layer requires at least:

```text
Revenue
- Cost of goods sold
- Wages
- Rent and property costs
- Supplies and operating costs
- Interest
- Depreciation or capital treatment, where modelled
- Permitted deductions
= Taxable profit
```

Keep distinct:

1. sales or transaction tax collected during sales;
2. statutory profit tax paid to an economic zone or government;
3. parent management fees for services;
4. dividends or surplus distributions to owners;
5. intercompany loans, whose principal is neither revenue nor expense.

A parent clan taking a share of profit is normally a dividend or management fee, not a tax.

## 7.7 Portfolio goals

Useful parent-level goals include:

- maintain a payroll reserve of a configured number of periods in each child;
- fund children below their working-capital floor;
- sweep distributable cash above a threshold;
- ensure every child has a proprietor or manager;
- escalate repeated payroll arrears;
- maintain portfolio liquidity or profit targets;
- transfer surplus stock before buying externally;
- consolidate common-supply purchasing;
- reserve for and pay periodic profit tax;
- file period-end accounts;
- fund property maintenance and capital projects;
- close or divest persistently unprofitable units;
- limit exposure to one market, district, or commodity;
- reconcile aged intercompany balances.

Parent goals require cycle detection, per-period budgets, child-scoped idempotency, and consolidated-reporting rules.

## 8. Additional Employment Host candidates

## 8.1 Property or landlord-management host

A property-management host can employ agents for:

- rent and arrears administration;
- inspections;
- repairs and maintenance;
- cleaning and groundskeeping;
- security;
- furnishing replacement;
- lease preparation and tenant communication;
- contractor procurement.

A property host may be subordinate to an organisation while a hotel, shop, or stable on the property remains a separate operating unit.

## 8.2 Warehouse or distribution centre

A warehouse fills the gap between shops and transport:

- receiving;
- put-away;
- picking;
- packing;
- stocktaking;
- internal transfer;
- dispatch;
- loss and damage investigation;
- reorder and capacity goals.

## 8.3 Workshop, factory, or manufactory

After craft-capacity work, production hosts can manage:

- materials acquisition;
- production queues;
- station assignment;
- tool maintenance;
- quality inspection;
- output storage;
- waste and by-products;
- batch planning.

## 8.4 Farm, ranch, plantation, mine, quarry, or forestry operation

These hosts naturally use schedules, seasons, and task-limited contracts:

- planting, tending, and harvesting;
- feeding, breeding, and animal care;
- extraction and processing;
- haulage;
- tool and supply maintenance;
- weather-triggered work;
- seasonal hiring.

## 8.5 Merchant caravan, ship, train, or transport company

After vehicle and animal logistics are complete:

- crew assignments;
- route planning;
- cargo custody;
- loading and unloading;
- guard duties;
- maintenance;
- fuel, feed, and supplies;
- passenger or freight operations.

## 8.6 Construction project

A project host is a strong use for fixed-term and task-limited work:

- project budget;
- milestones;
- material procurement;
- craft and labour tasks;
- inspections;
- completion and automatic host closure.

## 8.7 Civic and institutional hosts

Government departments, hospitals, temples, schools, guard organisations, and charities can use the common model when they have a treasury or budget, work locations, staff contracts, and repeatable operational services.

A market itself should not become an employment host merely to permit market-influence mutation. A government or trade office may employ policy staff, but the privileged market-policy operation remains separate.

## 9. Recommended implementation sequence

### Phase A: correctness and economic safety

1. Remove and quarantine employment `price market` actions.
2. Add native shop-deal employment actions.
3. Repair payable state and wage disbursement.
4. Add worked-time and cadence-aware earnings.
5. Make arrears employee-specific.
6. Complete application, position, schedule, and duration lifecycle.
7. Add task provenance, priority, and explicit principals.

### Phase B: runtime architecture

8. Centralise host operations evaluation.
9. Make worker AI schedule-aware and multi-employer aware.
10. Normalise compensation for matching and ranking.
11. Populate real candidate skills, knowledges, and tags.
12. Replace hard-coded discovery, location, finance, tax, and clock switches with capability providers.

### Phase C: original deferred systems

13. Account transfer and host settlement.
14. Craft capacity, production chains, and recovery.
15. Vehicle and animal logistics.
16. Controlled recursive administration.
17. Broader manager autonomy.
18. Deeper host-specific workflows.

### Phase D: enterprise and new hosts

19. Introduce `IEmploymentOrganisation`.
20. Add parent/child business ownership and control.
21. Add scoped cross-host authority.
22. Integrate clan budgets and treasury.
23. Add intercompany finance.
24. Add accounting periods and profit taxation.
25. Add portfolio goals.
26. Pilot with one clan controlling a property plus a shop or hotel.
27. Add property, warehouse, production, and primary-industry hosts.

## 10. Acceptance gates

The remediation and extension work is not complete until automated tests establish the following behaviours.

1. No employment task, scheduled rule, or manager goal can create or modify a `MarketInfluence`.
2. A temporary sale creates a native `ShopDeal`, affects only its configured target, and expires or cancels correctly.
3. An eight-hour shift accrues eight hours of hourly wages rather than twenty-four.
4. A bank wage creates an employer debit and employee credit atomically.
5. A failed destination payment leaves the payable outstanding.
6. A former employee can claim funded cash wages.
7. An employee does not resign because another employee's wages are overdue.
8. Unsupported pay cadences are rejected or produce explicit earnings.
9. An application cannot be accepted against closed, full, changed-without-consent, or no-longer-matching terms.
10. A position reopens under the configured seat policy when its originating contract ends.
11. Goal and task priority changes actual evaluation and assignment order.
12. Every automated task records its source, creator, authoriser, and authorisation grant.
13. A worker cannot approve their own employer spending merely by completing an `authorise` action step.
14. A parent manager can operate only authorised child hosts and financial amounts.
15. Cross-host transfers write paired records and do not create consolidated profit.
16. Payroll and statutory liabilities are protected before parent surplus sweeps.
17. Adding a new host family does not require independently editing worker discovery, scheduler discovery, location, finance, tax, and clock type switches.
18. All prior physical logistics, purchase, custody, and float security regressions remain covered.

## 11. Principal code touchpoints

The first remediation work is expected to involve at least:

- `FutureMUDLibrary/Economy/Employment/IEmploymentHost.cs`
- `FutureMUDLibrary/Economy/Employment/EmploymentDomain.cs`
- `FutureMUDLibrary/Economy/Employment/EmploymentTasks.cs`
- `FutureMUDLibrary/Economy/Employment/EmploymentActionCatalog.cs`
- `FutureMUDLibrary/Economy/Employment/EmploymentManagerGoalCatalog.cs`
- `MudSharpCore/Economy/Employment/EmploymentHostState.cs`
- `MudSharpCore/Economy/Employment/EmploymentTaskBoard.cs`
- `MudSharpCore/Economy/Employment/EmploymentActionSteps.cs`
- `MudSharpCore/Economy/Employment/EmploymentFinanceService.cs`
- `MudSharpCore/Economy/Employment/EmploymentPersistenceStore.cs`
- `MudSharpCore/Economy/Employment/EmploymentScheduledRuleEvaluationService.cs`
- `MudSharpCore/Economy/Employment/EmploymentHostAccessExtensions.cs`
- `MudSharpCore/NPC/AI/EmploymentWorkerAI.cs`
- `MudSharpCore/Economy/Shops/ShopDeal.cs`
- `MudSharpCore/Economy/Shops/Shop.Deals.cs`
- `MudSharpCore/Community/Clan.Finance.cs`
- property ownership and revenue services;
- employment command and authoring services;
- employment, worker AI, shop, payroll, and persistence test suites.

## 12. First recommended implementation slice

The first slice should be **local pricing boundary correction** because it is narrow, high confidence, and prevents local employment automation from mutating regional economic state.

Deliverables:

1. Remove `price market` from the employment action catalogue and parser.
2. Fail closed when loading or executing a persisted employment market-price mutation.
3. Add a compatibility report and rule/goal/task quarantine path.
4. Add native `ShopDeal` create, modify, cancel, and expiry actions with explicit invocation-source policy.
5. Retain bounded exact merchandise repricing.
6. Amend the unified employment design text that currently describes market-influence mutation as intentional.
7. Add regression tests proving employment automation cannot add or update `MarketInfluence` objects.
8. Add end-to-end tests for scheduled temporary sales and volume deals.

The next slice should then address wage disbursement correctness before adding further finance or enterprise automation.
