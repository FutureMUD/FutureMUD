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
- host adoption for shops, auction houses, combat arenas, banks, stables, hotels, and clans;
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

A clan or organisation that owns a business is not automatically the same employment host as that business. Parent organisations and operating units require an explicit relationship and scoped authority model. A clan may also be its own `IEmploymentHost` for headquarters-style staff, but that host does not merge or replace the separate employment hosts for shops, hotels, stables, or other businesses the clan owns.

## 4. User-story walkthrough and findings

## 4.1 Opening, application, and hire

### Current path

1. A manager creates an opening through `EmploymentHostState.CreateJobOpening` or the command adapter.
2. `EmploymentWorkerAI` enumerates host openings, applies candidate matching, host reputation, pathing, and raw nominal-pay ordering.
3. The AI creates an application through `EmploymentHostState.Apply`.
4. A manager accepts the application.
5. `EmploymentHostState.AcceptApplication` creates a contract from the opening terms only when the opening is still open/vacant and its runtime revision matches the application's offered revision.

### Implemented successfully

- manager authority is checked;
- opening capacity is checked per opening;
- candidate requirements can be represented;
- reservation wage and accepted payment methods are considered;
- application and hire records are persisted and audited;
- manager acceptance creates a real active contract.

### Gaps

- Runtime applications now record the offered opening revision and candidate profile; acceptance rejects closed/full/stale-revision applications and re-evaluates in-memory candidate profiles.
- Durable EF rows now store opening revision numbers, offered application revisions, candidate-profile snapshots, and accepted-contract origin opening/application ids; broader immutable term snapshots and candidate-consent state remain future work.
- Duplicate pending applications are prevented by the domain returning the existing pending application for the same candidate/opening.
- `NpcApplicationsOnly` is represented but is not consistently enforced as a domain rule.
- Position occupancy is now based on active or suspended contracts that originated from the opening, so ended contracts release seats and persisted origin references preserve occupancy across reloads.
- Worker AI candidate profiles now populate visible skill/derived-skill/theoretical-skill traits, knowledges, and candidate tags derived from race, ethnicity, culture, chargen roles, visible skill/knowledge tokens, and any `IHaveTags` implementation.
- Offer ranking and reservation-wage comparison now use shared hourly-equivalent normalisation for hourly, daily, weekly, and 30-day salary pay. Per-task, commission, and mixed pay still fall back to advertised minimum/fixed amounts until explicit earning and expected-duration models exist.
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

### Current Phase A progress

As of 2026-06-19, active tasks carry runtime provenance fields for source kind, source rule, source goal, creator principal, authorising principal, authorisation grant, priority, creation/due/assignment/start/completion timestamps, and idempotency correlation. Manual tasks, scheduled-rule spawns, and manager-goal spawns now mint explicit runtime grants. Worker AI claims pending tasks by priority descending, due time, creation time, and stable task id.

Payment authorisation now comes from the active task grant or an explicit context authorisation; completing an `authorise` action step is not by itself managerial approval. Manager-authored `authorise [<amount> for]` steps still contribute the runtime grant amount limit at task creation/spawn time, preserving existing bounded-finance workflows while moving approval authority to task provenance.

### Gaps

- Task provenance and authorisation grants are runtime-only; persisted active tasks load with compatibility host-system principals but fail closed for financial grant authority until schema-backed provenance/grant columns exist.
- Retry and escalation policy fields are still not modelled.
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

As of 2026-06-19, the employment `price` action supports exact merchandise base-price changes only. The former market mode is deprecated: the action catalogue and parser no longer expose it, direct execution fails closed, and persisted market-mode payloads load as deprecated steps for quarantine.

Employment automation must never create or update a `MarketInfluence`.

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

sale modify <deal id | name>
  target all | merchandise <which> | tag <which>
  adjustment <signed percentage>
  applies sell | buy | both
  eligibility <prog | none>
  cumulative <true | false>
  expires <datetime | duration>

sale cancel <deal id | name>
```

#### Volume deal

Use native `ShopDealType.Volume` with a minimum quantity rather than mutating merchandise prices.

### Persisted-data treatment

Existing persisted market-mode employment price steps must not be silently reinterpreted.

The startup compatibility pass now:

- pauses scheduled rules containing a market-mode price action;
- marks affected manager goals blocked with a builder-review diagnostic;
- blocks affected active tasks with a deprecation diagnostic;
- preserves the original payload on the deprecated action step for inspection;
- writes employment register entries for each quarantined rule, goal, or active task;
- records an implementor-facing report in those entries listing host, market, category, impacts, influence name, and expiry where the legacy payload exposes them.

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

As of 2026-06-19, contracts persist a `WorkSchedule`, `EmploymentDuration`, compensation terms, and payment method. Payroll still accrues durable payables on a daily/weekly/salary periodic cadence, but hourly pay now uses the configured schedule window duration where a start and end time exist rather than multiplying by all elapsed contract hours. `AnyTime` remains an explicit always-available fallback until formal attendance records are added.

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

As of 2026-06-19, host-state lifecycle evaluation expires fixed-term active or suspended contracts and payroll invokes that evaluation before accrual so final partial payables can be created.

Still add central evaluation for:

- seasonal activation and expiry;
- task-limited completion;
- suspension and resumption;
- host closure;
- renewal.

Ending a contract must continue preserving historical records and outstanding liabilities.

## 4.5 Cash and bank wage payment

### Current path

As of 2026-06-19, payroll settlement validates destination support for every accrued payable before moving any employer funds, then validates grouped employer funds by currency. Cash payables debit backed employer funds into durable claimable payroll escrow, and active or former employees may claim that funded cash later. Employee-bank and specified-bank payables require a destination account in the payable currency; successful settlement debits the employer and credits the destination through the native bank-account transaction path. Invalid destinations leave the payable outstanding and employer funds untouched. Payment-item and employer-float disbursement remain explicit unsupported failures rather than silent settlements.

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

As of 2026-06-19, payroll exposes employee-specific liability helpers:

```text
OutstandingLiabilitiesFor(employee)
MaximumOverdueDaysFor(employee)
MaximumOverdueDaysFor(employee, now)
```

Worker AI resignation now uses only the worker's own overdue payables. Host-wide arrears remain available for manager reporting, payroll conditions, and prospective-worker employer-reputation filtering. A broader `EmployerReliabilityScore(currency, period)` remains deferred until there is a richer labour-market reputation model.

## 4.6 Manager goals

### Current path

Goals persist type, configuration, required authority, status, priority, cadence, last evaluation, and correlation. Evaluation creates a goal-scoped active task and suppresses duplicates while an existing task remains active or blocked.

### Phase A progress

As of 2026-06-19, active manager goals evaluate in priority order, propagate their priority to spawned active tasks, and stamp spawned tasks with a manager-goal source principal and runtime authorisation grant. Duplicate suppression still uses the goal correlation idempotency key.

### Gaps

- Goal-created task provenance and grants are not yet schema-backed.
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

As of 2026-06-19, this exists as a runtime model for active tasks: `EmploymentTaskProvenance`, `EmploymentPrincipal`, and `EmploymentAuthorisationGrant` are stamped onto newly-created manual tasks, scheduled-rule spawns, and manager-goal spawns. Grants currently derive their amount caps from manager-authored `authorise` action steps at task creation/spawn time, and payment steps consume the task grant rather than the worker's later completion of the shell step. Durable grant persistence, revocation, counterparty limits, payment-source limits, redelegation, and parent-organisation scoping remain deferred.

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

As of 2026-06-19, `EmploymentHostOperationsScheduler` centralises scheduled-rule evaluation, manager-goal evaluation, payroll accrual, fixed-term contract lifecycle expiry, and active-task assignment audits for a host. Worker AI host-work evaluation now calls this scheduler rather than directly invoking scheduled-rule and manager-goal services. The scheduler returns countable result objects for tests and future command/daemon callers. Stale grant/reservation cleanup, retry/escalation policy, global host locking, and idempotent distributed scheduling remain deferred.

## 5.3 Worker matching and multi-employer behaviour

As of 2026-06-19, worker host selection remains intentionally simple but no longer ignores task priority across active contracts. Assigned/in-progress/blocked work for the worker still takes precedence; otherwise, pending task priority, due time, creation time, and stable task id are used before host name to choose among the worker's active hosts.

As of 2026-06-20, worker search profiles use live character traits, knowledges, and candidate tags; requirement matching is case-insensitive for skills, knowledges, and tags; reservation-wage checks and offer ranking use hourly-equivalent compensation; and prospective employers with older unpaid payroll liabilities sort behind equally paid, equally reachable employers before becoming unacceptable at the configured overdue threshold.

Complete candidate and worker profiles with:

- real skills (implemented for visible skill, derived-skill, and theoretical-skill traits in worker application profiles);
- real knowledges (implemented in worker application profiles);
- world-specific tags or traits (implemented for race, ethnicity, culture, chargen roles, visible skills, knowledges, and `IHaveTags` tokens; further world-specific tag policy remains authoring convention work);
- schedule availability;
- normalised effective compensation (implemented for matching/ranking using hourly-equivalent rates; explicit per-task, commission, and mixed expected earnings remain future work);
- travel burden (implemented for employment-search tie-breaking by commute path length; detailed schedule/route burden remains future work);
- employer reliability (implemented as overdue-payroll rejection and tie-breaking; richer reputation scoring remains future work);
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

- account-to-account transfers (same-currency linked-bank to target-bank transfer implemented 2026-06-19; cross-currency and cross-host variants remain);
- host-to-host settlements (same-currency supported-host virtual settlement implemented 2026-06-19; parent/consolidated settlement remains);
- physical payroll reserve and escrow (cash wage escrow implemented 2026-06-19; fuller wage state machine remains);
- employee-float issue, reconciliation, and recovery (physical-float issue/return/settle implemented 2026-06-19; richer recovery policy remains);
- paired transaction correlation (same-currency transfer/settlement records and source/target employment evidence implemented 2026-06-19; consolidated intercompany accounting remains);
- currency conversion policy (same-currency fail-closed policy implemented 2026-06-19; exchange-rate settlement remains deferred);
- atomic reservations and disbursements (current finance steps consume task reservations before native movements; fuller persistence transaction state remains);
- authorisation amount and counterparty limits (runtime amount, payment-source, and counterparty scopes implemented 2026-06-19; durable grant persistence/revocation and route-pair policy remain).

Employment ledger entries remain correlating audit evidence, not the canonical general ledger.

As of 2026-06-19, Epic 2 has live same-currency account-transfer and supported-host settlement paths. Employment action plans can use `transfer <amount> to <bank account id|bankcode:account|alias>` to move funds from the supported host's linked native bank account to another active native bank account, and `settle <amount> to <host type> <id|name>` to move backed same-currency value from one supported employment host to another. These steps require task payment authorisation plus a reservation, write native finance records where the host subsystem exposes them, record employment ledger/register evidence, and persist transaction/reservation references in active-task step state. Runtime grants now also carry amount, payment-source, and counterparty scopes for financial steps. Currency conversion, parent/consolidated settlement, durable grant persistence/revocation, and route-pair accounting remain deferred.

## Epic 3: Craft and production operations

Deliver:

- station capacity and queueing (configurable station capacity implemented 2026-06-19; persisted queue/retry policy remains);
- priority and multiple-worker scheduling (task priority already drives claim order; current-step capability dispatch and no-custody step-boundary requeue implemented 2026-06-19; persisted multi-assignee/custody handoff remains);
- production chains (basic sequential craft state links implemented 2026-06-19; automatic chain planning remains);
- intermediate output custody (craft outputs are adopted into task custody and later craft starts now record those carried items as task inputs; salvage views remain);
- failed-craft recovery and salvage (failed active crafts with employment state now adopt visible salvage into task custody and fail closed for manager review; richer manager-directed salvage actions remain);
- inspection and quality-control steps (`inspect` catalogue action implemented 2026-06-19 for task-custody item QC notes; richer quality scoring remains);
- materials replenishment (craft-materials manager goals with commodity/item thresholds spawn configured replenishment work; automatic supplier/batch choice remains);
- demand- and storage-aware batch sizing (explicit `batch` planning action implemented 2026-06-19; automatic demand forecasting and supplier/recipe quantity derivation remain).

## Epic 4: Vehicle, animal, and route logistics

Deliver:

- driver and vehicle assignment (`vehicle assign` implemented 2026-06-19 with task-scoped active assignment conflict checks);
- animal leading, riding, lodging, and return (`animal` action implemented 2026-06-19; leading is an auditable pathable plan, riding/lodging/return call native mount/stable APIs);
- cargo capacity and compartment selection (`vehicle cargo` now records compartment/projection and capacity-checks carried task items through the cargo projection container);
- persisted route plans (`route` now stores ordered route stop ids, path-checks each stop, and records durable route evidence);
- multi-stop batching (`routebatch` records total/per-stop allocation against ordered route stops and blocks oversized allocations);
- loading and unloading reservations (load/unload now write task-local reservation reserve/consume markers alongside loaded-asset state);
- fuel, feed, maintenance, and rest policies (`tripcheck` records manager-authored policies, optional route stops, and durable transport-policy evidence);
- recovery when a worker, vehicle, animal, or destination becomes unavailable (active-task assignment audit now requeues no-custody logistics work or blocks physical-custody work for manager review).

## Epic 5: Manager autonomy and administrative services

Deliver:

- goal priority, budgets, and risk limits (priority, runtime budget caps, active-task limits, and action-step caps implemented 2026-06-19; durable policy persistence and richer manager AI tuning remain);
- goal satisfaction and failure states (runtime `Satisfied` and `Failed` statuses implemented 2026-06-19; `AwaitingApproval`/paused UX and richer persistence policy remain);
- rich goal expressions (runtime expression trees over manager-goal conditions implemented 2026-06-19; durable expression persistence remains deferred);
- controlled scheduled-rule administration (task-executable pause/resume/cancel/evaluate implemented 2026-06-19; full create/edit remains deferred to nested rule-authoring design);
- controlled task retry, reassignment, and cancellation (task-executable retry/requeue/assign/cancel implemented 2026-06-19; hard completed/failed marking remains deferred);
- controlled goal mutation (task-executable evaluate/cancel/reactivate implemented 2026-06-19 for existing goals; definition create/edit, durable policy/expression persistence, and terminal completion semantics remain deferred);
- choice among purchasing, crafting, transfers, local repricing, sales, or accepting shortage;
- human-manager escalation when no authorised plan is viable.

Recursive administrative actions must call the owning task, rule, or goal service and preserve complete provenance.

## Epic 6: Deeper host-specific workflows

### Shops

- temporary sales and volume deals (implemented 2026-06-19 through native `ShopDeal` task actions);
- bounded local repricing (implemented 2026-06-19 for exact merchandise base-price changes; percentage/margin policy remains);
- supplier management (auditable cheapest stocked supplier selection implemented 2026-06-19; preferred-vendor contracts, supplier budgets, and cross-step supplier binding remain);
- stocktake and discrepancy handling (stocktake count/weight evidence implemented 2026-06-19; automatic discrepancy remediation remains);
- cash reconciliation (expected/virtual/physical cash evidence plus native float checking implemented 2026-06-19; automatic shortage recovery and escalation remain);
- internal and external stock transfer (permanent-shop stockroom restock bridge plus explicit cross-shop stock-transfer task actions implemented 2026-06-19; autonomous surplus discovery and transfer policy remain).

### Auction houses

- lot intake and custody (auction-house-owned carried task-custody item listing implemented 2026-06-19; third-party consignment intake remains);
- valuation and listing (explicit manager-authored reserve, optional buyout, and optional duration implemented 2026-06-19; automated valuation policy remains);
- auction scheduling (default/override duration and due-lot settlement implemented 2026-06-19; automatic scheduling policy remains);
- commission collection (native auction settlement path used by task-executable settlement 2026-06-19);
- seller payout (native seller payout/refund queue settlement used by task-executable settlement 2026-06-19; external payout failure remains native queue behaviour);
- buyer settlement (native paid-bid settlement used by task-executable settlement 2026-06-19; autonomous bidding/buyout remains native command behaviour);
- won-lot delivery (unclaimed movable lot claim into task custody implemented 2026-06-19; automatic recipient route selection remains).

### Combat arenas

- event scheduling (task-executable native arena event creation, phase advancement, and abort handling implemented 2026-06-19; recurring event policy and automated scheduling remain);
- competitor intake;
- purses and prizes;
- security and crowd tasks;
- stable and equipment preparation;
- medical and cleanup workflows.

### Banks

- teller staffing (branch post evidence implemented 2026-06-20; durable teller rosters remain future persistence work);
- cash and reserve balancing (task-executable bank reserve audit/deposit/withdraw implemented 2026-06-20 through native reserves plus the bank host virtual-cash ledger; per-branch vault balances remain future design work);
- inter-branch courier work (branch courier evidence implemented 2026-06-20 with native branch-location validation and pathing; routed physical cash custody remains future work);
- account-service tasks (account credit/status/close wrappers implemented 2026-06-20 through native bank-account methods and bank manager audit logs; rollover and customer-facing account-opening policy need a narrower service design before task automation);
- branch-specific staffing and controls (future design: requires branch-level staffing, vault, control, and reconciliation persistence rather than a global bank-only reserve model).

### Stables

- feeding, grooming, exercise, and inspection evidence (implemented 2026-06-20 through `stableadmin care`; durable feed inventory, welfare scoring, and automatic native feeding/grooming/exercise services remain future stable design work);
- animal transfer and return (implemented 2026-06-19 through `animal` lodge/return native stable APIs);
- feed replenishment (care-feed audit evidence implemented 2026-06-20; automatic feed stock replenishment remains purchasing/inventory policy work);
- ticket and account reconciliation (stay/ticket and account audit evidence implemented 2026-06-20 through `stableadmin stay` and `stableadmin account`; account payment/limits remain native stable account command work);
- capacity and welfare goals (future design: requires persisted stable capacity, care policy, welfare scoring, and feed/equipment state rather than employment-only notes).

### Hotels

- cleaning and room readiness evidence (implemented 2026-06-20 through `hoteladmin room`; durable room cleanliness/readiness/maintenance state remains future hotel persistence work);
- furnishing replacement (future design: should call hotel-room furnishing/deposit services rather than employment-only notes);
- check-in and check-out support (future design: should call native rent/complete-stay services with payment, tax, key, and deposit policy);
- patron-balance collection (patron-balance audit evidence implemented 2026-06-20 through `hoteladmin balance`; actual collection/payment workflow remains native hotel finance work);
- lost-property handling (expiry checks and lost-property audit evidence implemented 2026-06-20 through `hoteladmin lost`; physical release, auction/liquidation, and owner delivery remain native property/hotel flows);
- consumable replenishment (future purchasing/inventory policy work);
- maintenance escalation (maintenance audit evidence implemented 2026-06-20 through `hoteladmin room maintenance`; durable maintenance tickets remain future hotel operations work).

## Epic 7: Candidate and labour-market depth

Deliver:

- real skills, knowledges, and tags (implemented 2026-06-20 for worker application profiles and case-insensitive requirement matching);
- schedule and commute preferences (commute reachability and distance tie-breaking implemented 2026-06-20; schedule availability remains future design);
- wage normalisation (implemented 2026-06-20 for hourly-equivalent matching/ranking; per-task/commission/mixed expected earnings remain future design);
- employer reputation (implemented 2026-06-20 for overdue-payroll rejection and ranking; richer reputation history remains future design);
- retention and resignation choices (overdue-wage resignation already implemented; broader retention/personality choices remain future design);
- training and progression (future design: needs persisted training policy, skill advancement hooks, and employer-sponsored learning semantics);
- local labour availability and reservation-wage inputs (future design: needs region/zone labour supply, market-rate history, and schedule/reservation-wage authoring).

## Epic 8: Legacy bridges

Keep this optional and separately scoped:

- report legacy/new employment divergence (implemented 2026-06-20 through read-only `EmploymentLegacyBridgeReporter` and `impdebug employment legacy`);
- bootstrap selected legacy staff into contracts where useful (future design: requires explicit mapping, consent/authority, pay/schedule/role conversion, and rollback semantics);
- replace legacy commands only after equivalent common services exist (unchanged; no command replacement was performed in this epic);
- preserve the `IJobListing` and `IActiveJob` boundary until a dedicated convergence design is approved (preserved; the bridge reporter is intentionally outside `IEmploymentHost` and `EmploymentCommandService`).

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

As of 2026-07-01, `IClan` / `Clan` is a first-class `IEmploymentHost` with `EmploymentHostType.Clan` for organisation-host employment. Clan employment locations are all distinct cells in properties where the clan has any ownership share, plus admin-managed clan hall cells toggled with `clan hall <clan>` for non-property workplaces. Clan employment finance uses the clan bank account currency when present, otherwise existing contract compensation currency where available; paid opening/task/payroll authoring that needs a host currency must fail closed if neither source exists. Supported clan finance movements use `VirtualCashLedger` with the clan bank account as optional backing.

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

13. Account transfer and host settlement (same-currency native bank-account transfer and supported-host virtual settlement implemented; cross-currency and consolidated settlement remain).
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
19. A same-currency employment account transfer writes paired native bank-account transfer records, consumes a task reservation, writes employment evidence, and leaves both accounts untouched when the destination account is unsupported or uses another currency.
20. A same-currency host settlement debits one supported host, credits another supported host, writes paired native finance evidence and source/target employment evidence, and leaves both hosts untouched when the target host is unsupported or uses another currency.
21. A scoped payment authorisation grant can block an otherwise valid account transfer or host settlement when the payment source or counterparty is outside the grant scope, leaving native funds untouched.
22. A craft station can admit concurrent employment reservations up to its configured capacity, and blocks additional reservations once capacity is full without adding another station effect.
23. A multi-step active task can assign a worker for the current step, requeue at a no-custody step boundary when the next step requires another worker, and then assign an eligible worker for that next step.
24. A two-step craft production chain can adopt the first craft output into task custody and record that carried output as a task input when starting the second craft.
25. A failed native craft with prior employment craft state adopts visible salvage into task custody, records a failure diagnostic, and fails the active task for manager review rather than reporting normal completion.
26. A quality-control inspection step requires task-custody items, records the inspected item ids and inspection note, and leaves custody unchanged.
27. A craft-materials manager goal with a low commodity threshold spawns an authorised replenishment task with the configured purchase/material action plan and priority.
28. A demand- and storage-aware batch planning step records demand, storage capacity, chosen batch size, and rationale, and blocks plans whose batch size exceeds either demand or storage.
29. A vehicle assignment step records the driver and vehicle in durable task state, blocks a competing active task from assigning the same vehicle, and allows assignment again after the original task completes.
30. An animal operation task can record a pathable lead plan, call native mount riding, call native stable lodging, and call native stable return/release while preserving durable selected-resource evidence.
31. A vehicle cargo selection step records the cargo space, compartment, projection container, capacity validation state, and carried-item count; it fails closed when the selected cargo projection is missing, not a container, inaccessible, or cannot accept the task items already in custody.
32. A route planning step can persist an ordered list of route stops, blocks assignment when any stop is unreachable, records `operation=route` selected-resource evidence with stop ids and final destination, and round-trips those stops through active-task persistence.
33. A route-batch planning step records an ordered multi-stop allocation with total quantity, per-stop quantity, planned dispatched quantity, remainder, final stop, and route stop ids, and blocks plans whose per-stop allocation exceeds the total available quantity.
34. Load and unload steps record a task-local load reservation lifecycle: `op=reserve` when task items are loaded into a container or cargo projection, and `op=consume` when those loaded items are unloaded back into task custody, including task id, container id, item ids, and count.
35. Stable and hotel administration tasks can draft, execute, audit, and persist native-wrapper evidence for stable care/fee/stay/account work and hotel room/lost-property/patron-balance work without inventing hidden welfare or room-state persistence.
36. Worker applications populate real skill, knowledge, and tag candidate profiles; matching is case-insensitive for those profile fields; reservation-wage filtering and offer selection use normalised effective compensation; and prospective employers with overdue payroll are ranked or rejected according to reputation pressure.
37. A read-only legacy bridge report can compare loaded legacy `IJobListing`/`IActiveJob` records with employment-host contracts, report both legacy-only and new-model-only divergences, and preserve the `IEmploymentHost`/`EmploymentCommandService` boundary from the legacy PC-job interfaces.

### 2026-06-19 demand/storage batch-sizing progress

Epic 3 demand- and storage-aware batch sizing is implemented as an explicit executable production-planning step.

- The action catalogue now exposes `batch` with `batchsize` and `sizing` aliases under production work.
- Runtime execution validates the builder-authored batch plan shape: `demand <target> storage <capacity> size <quantity> <rationale>`.
- The step blocks when the planned size exceeds either stated demand or storage capacity, and records demand, storage, size, actor, and rationale in durable operational state when successful.
- Acceptance coverage verifies a valid batch plan completes with the expected state evidence and an oversized storage plan is rejected before assignment.

Design review note: this completes the original-design batch-sizing acceptance as an auditable planner primitive, not as autonomous forecasting. Automatic supplier choice, recipe-derived purchase quantities, and continuous demand forecasts remain manager-autonomy work rather than core task-board mechanics.

### 2026-06-19 vehicle assignment progress

Epic 4 driver and vehicle assignment is implemented as a task-scoped vehicle reservation step.

- The `vehicle` action now supports `vehicle assign <vehicle id|exterior item id>` in addition to the existing `vehicle cargo ...` cargo-space validation.
- Assignment validates that the vehicle is present, reachable, not disabled/destroyed, and not already assigned by another pending/assigned/in-progress/blocked active task.
- Successful assignment records `operation=vehicleassign`, `vehicle`, and `driver` in durable selected-resource state, with a matching operational payload and employment-register audit entry.
- Persistence stores vehicle operations with an operation kind, preserving legacy cargo-space records while allowing assignment records without cargo-space ids.
- Acceptance coverage verifies assignment evidence, competing active-task blocking, and release once the assigning task completes.

Design review note: this phase deliberately does not introduce a persistent fleet-dispatch table or autonomous driving. Active-task conflict checks are reconstructed from task operational state, which fits the current task-board model; route optimisation and richer cargo loading policy remain later transport work. Later unavailable-resource audit now revalidates completed vehicle evidence.

### 2026-06-19 stable-animal operation progress

Epic 4 animal leading, riding, lodging, and return is implemented as an executable `animal` logistics action.

- The action catalogue now exposes `animal` with `mount` and `stableanimal` aliases, covering `lead`, `ride`, `lodge`, and `return` forms.
- `animal lead` validates the handler can path to the animal and destination and records a durable lead plan; it does not yet perform autonomous animal movement.
- `animal ride` calls the native mount API after normal mountability validation.
- `animal lodge` calls the native stable `CanLodge`/`Lodge` flow and records the resulting stay id.
- `animal return` calls native stable release/return with optional fee waiver and records stable stay evidence.
- Acceptance coverage drives all four operations through the dispatcher and verifies native mount/stable calls plus durable selected-resource state.

Design review note: there is no separate native animal-leading service in this slice. The current implementation deliberately records lead intent and pathability as task evidence, while autonomous leading and richer stable-account/payment handling remain later transport/recovery work. Later unavailable-resource audit now revalidates completed animal and stable evidence.

### 2026-06-19 vehicle cargo capacity progress

Epic 4 cargo capacity and compartment selection is implemented through the existing vehicle cargo projection model.

- `vehicle cargo <vehicle id|exterior item id> <cargo id|cargo name>` now requires the selected cargo space to have a projection item with a usable `IContainer` component.
- Cargo selection continues to validate vehicle availability, cargo-space access, and worker pathing before execution.
- When the worker is already carrying task-custody items, cargo selection runs the projection container's native `CanPut` checks and blocks before the later load step if any carried item cannot fit.
- Successful selection records `operation=vehiclecargo`, `vehicle`, `cargo`, `compartment`, `projection`, `capacity`, and `carried` in durable selected-resource state.
- Acceptance coverage verifies successful compartment/projection evidence and over-capacity rejection after task-item collection.

Design review note: capacity is intentionally delegated to the projected cargo container rather than duplicating vehicle-specific bulk rules in employment code. Automatic load optimisation, route optimisation, and deeper cargo-space recovery policy remain later transport work; later load/unload reservation and unavailable-resource phases add task-local reservation evidence and vehicle revalidation.

### 2026-06-19 persisted route-plan progress

Epic 4 persisted route plans are implemented as ordered stops on the executable catalogue `route` action.

- `route` steps now carry `RouteStops` in addition to their final `TargetLocation`, preserving existing single-target route steps as one-stop plans.
- Builder authoring supports `tasks step route to <here|cell id> [then <here|cell id> ...] [description]`.
- Assignment validates that the worker can path to every route stop, not just the final destination.
- Execution records `operation=route`, ordered stop ids, final destination id, route note, and a human-readable stop chain in durable operational state.
- Persistence stores route stop ids in the catalogue shell payload and continues to hydrate older single-target records from `TargetLocationId`.
- Acceptance coverage verifies route evidence, waypoint parser support, blocked unreachable stops, and active-task persistence round-trip.

Design review note: this is still a route plan, not autonomous movement or optimisation. Later Epic 4 slices add multi-stop batching, load/unload reservation markers, trip-check policy evidence, and assignment-audit recovery for missing recorded destinations.

### 2026-06-19 multi-stop route-batching progress

Epic 4 multi-stop batching is implemented as an executable `routebatch` catalogue action.

- The action catalogue now exposes `routebatch` with `deliverybatch` and `multistop` aliases.
- Builder authoring supports `tasks step routebatch total <quantity> each <quantity> to <here|cell id> [then <here|cell id> ...] <rationale>` and requires at least two route stops.
- Assignment validates the route-batch quantity shape, blocks per-stop allocations whose planned total exceeds the available total, and path-checks every stop.
- Execution records `operation=routebatch`, actor, ordered stop ids, final stop, total, per-stop quantity, planned quantity, remainder, and rationale in durable step state.
- Acceptance coverage verifies routebatch catalogue presence, authoring syntax, successful multi-stop allocation evidence, and oversized allocation rejection.

Design review note: this phase records a manager-authored allocation plan for already-known stops. It does not optimise stop ordering, choose cargo, or perform delivery loops; later Epic 4 slices add load/unload reservation markers, trip-check policy evidence, and assignment-audit recovery for unavailable recorded resources.

### 2026-06-19 load-unload reservation progress

Epic 4 loading and unloading reservations are implemented as task-local reservation markers on existing load/unload operational state.

- Loading task-custody items into a target container or cargo projection now records `ReservationReference` with `op=reserve`, `type=load`, active task id, container id, item ids, and item count.
- Unloading task-loaded items now records a matching `op=consume` marker with the same container/item evidence shape.
- The existing `LoadedAssets` state remains the source of truth for reconstructing loaded item custody after reload; the reservation marker adds durable lifecycle/audit evidence rather than a second custody store.
- Acceptance coverage extends the load/unload transport test to verify reserve and consume markers for the loaded container and item count.

Design review note: this phase deliberately keeps load reservations task-local. Cross-task cargo capacity holds, expiry policies, and richer recovery for missing loaded assets remain future transport-management work; the later unavailable-resource phase preserves the existing block-for-manager-review behaviour whenever physical task custody exists.


### 2026-06-19 fuel-feed-maintenance-rest policy progress

Epic 4 fuel, feed, maintenance, and rest policies are implemented as an executable `tripcheck` logistics catalogue action.

- The action catalogue now exposes `tripcheck` with `routepolicy`, `logisticspolicy`, and `logisticscheck` aliases.
- Builder authoring supports `tasks step tripcheck fuel <policy> feed <policy> maintenance <policy> rest <policy> [to <here|cell id> [then <here|cell id> ...]] <rationale>`.
- Execution requires all four policy fields, path-checks any declared route stops, and writes durable `operation=tripcheck` selected-resource evidence with fuel, feed, maintenance, rest, stop, final-stop, and rationale data.
- Location hints include declared trip-check route stops so worker routing and manager inspection see the same ordered plan.
- Acceptance coverage verifies successful policy evidence, route-stop path blocking, catalogue metadata, and command-authoring syntax.

Design review note: this phase deliberately records a transport-policy gate rather than directly mutating native refuel, feed/eat, item-maintenance, stamina, or rest systems. The native surfaces are separate subsystem concerns with their own authority, payment, inventory, animal-care, and timing semantics. Direct autonomous refuelling, feeding, grooming, repair, rest scheduling, and recovery from unmet policy checks remain later native-service work; assignment-audit recovery now covers unavailable recorded workers, vehicles, animals, stables, and destinations.

### 2026-06-19 unavailable logistics resource recovery progress

Epic 4 recovery for unavailable workers, vehicles, animals, and recorded destinations is implemented through the active-task assignment audit.

- The existing worker audit still requeues assigned work when a worker is fired, dies, enters stasis, loses required authority, or loses usable employment AI before taking physical custody.
- Completed vehicle assignment and vehicle cargo evidence is now revalidated against the live vehicle catalogue; missing, destroyed, disabled, or unlocated vehicles trigger recovery.
- Completed animal and stable evidence is now revalidated against live character/stable catalogues; missing, dead, stasis, or unlocated animals and missing/unlocated stables trigger recovery.
- Completed `route`, `routebatch`, `tripcheck`, and animal-lead destination evidence is now revalidated against the live cell catalogue, so deleted or missing recorded destinations do not leave active work stranded.
- Recovery follows the existing custody rule: if the active task has no physical task-item custody, it is released back to `Pending` with `ActiveTaskRequeued` evidence; if task goods are carried or loaded, the task is `Blocked` for manager review with `ActiveTaskBlocked` evidence.
- Acceptance coverage verifies no-custody worker requeue, custody-protected worker block, destroyed-vehicle requeue, destroyed-vehicle-with-custody block, dead-animal requeue, and missing recorded-destination requeue.

Design review note: board-level assignment audit intentionally does not take an `IEmploymentTaskContext`, so it cannot re-run dynamic pathfinding for newly blocked paths. Dynamic path availability remains enforced by executable route, trip-check, movement, vehicle, and animal steps at assignment/execution time. The audit covers resources and destination records that can be resolved from durable task evidence without needing a live command context.
### 2026-06-19 manager-goal budget-risk policy progress

Epic 5 manager-goal priority, budget, and risk-limit baseline is implemented.

- Manager goals now carry a runtime `ManagerGoalPolicy` with per-currency budget limits and `ManagerGoalRiskLimits`.
- `IManagerGoalBoard.CreateGoal` rejects action plans whose payment total exceeds configured budgets, whose financial steps are unbounded without permission, or whose action count exceeds the configured cap.
- Goal evaluation revalidates policy before spawning work and skips spawning when the goal's active-task cap is already reached, recording `ManagerGoalEvaluated` evidence.
- Builder authoring exposes `goals draft budget <amount|none>` and `goals draft risk tasks|steps|unbounded ...`; draft action additions are rejected immediately if they would violate the draft policy.
- Acceptance coverage verifies creation-time budget and step-cap rejection, active-task risk throttling, and command/draft policy authoring.

Design review note: this phase intentionally keeps manager-goal policy runtime-only. Persisted goals loaded from the database currently use `ManagerGoalPolicy.Default`; durable policy columns/serialization and explicit assigned manager role remain later Epic 5 design work.

### 2026-06-19 manager-goal satisfaction-failure state progress

Epic 5 manager-goal satisfaction and failure runtime states are implemented.

- `ManagerGoalStatus` now has appended `Satisfied` and `Failed` values, preserving existing persisted status numbers.
- Evaluation marks unmet trigger conditions as `Satisfied` with the first unsatisfied condition diagnostic instead of leaving the goal ambiguously active.
- `Satisfied` is deliberately non-terminal: satisfied standing goals remain eligible for cadence evaluation and reactivate to `Active` when their conditions later require work.
- Goals with no action plan now enter `Failed` with a manager-readable diagnostic instead of repeatedly evaluating without a viable plan.
- If a manager-goal-spawned active task reaches `Failed`, the parent goal enters `Failed` and records the task failure diagnostic.
- Acceptance coverage verifies satisfied/reactivated goals, failed spawned-task propagation, and no-plan goal failure.

Design review note: `Satisfied` represents a recurring standing objective being currently fulfilled, not permanent completion. `Failed` is terminal until manager intervention through later edit/recreate workflows. `AwaitingApproval`, paused/suspended command ergonomics, and richer persisted outcome metadata remain later Epic 5 work.

### 2026-06-19 manager-goal rich-expression progress

Epic 5 manager-goal rich condition expressions are implemented for runtime-created and edited goals.

- `ManagerGoalConfiguration` now carries an optional `ConditionExpression` using the same expression tree model as scheduled rules.
- `IManagerGoalBoard.CreateGoal` validates the expression against the goal's condition list and host predicate catalogue before accepting a goal.
- Goal-required authority is now derived from the expression-referenced conditions and predicates, with `CreateScheduledRules` stripped from condition authority so manager-goal authors do not need scheduled-rule creation rights for read-only finance predicates.
- Goal evaluation uses `EmploymentConditionExpressionEvaluator`, so manager goals support numbered conditions, `and`, `or`, `not`, parentheses, and fail-closed named predicates while preserving implicit `All` semantics for goals with no expression.
- Builder authoring exposes `goals draft expression <expression>` and goal detail renders the effective expression beside conditions.
- Acceptance coverage verifies OR-based satisfaction/reactivation, invalid expression rejection, and command-service finalise/detail behaviour.

Design review note: this phase intentionally keeps manager-goal expressions runtime-only. `EmploymentManagerGoalRecord` has no expression JSON column today, so persisted goals loaded from the database currently use implicit `All` semantics. Durable expression persistence should be designed alongside the remaining manager-goal policy persistence work rather than slipped in as an isolated schema change.

### 2026-06-19 controlled scheduled-rule administration progress

Epic 5 controlled scheduled-rule administration is implemented for existing-rule lifecycle and evaluation operations.

- The employment action catalogue now exposes executable `rule pause|resume|cancel <#|name|id> [reason]` and `rule evaluate <#|name|id> [manual <key>]` actions.
- Draft parsing resolves rule selectors to durable rule GUIDs at authoring time, so persisted active tasks do not depend on sorted list row numbers.
- `ScheduledRuleAdministrationActionStep` calls the host task board's owning pause, resume, cancel, and evaluate services and requires `ModifyScheduledRules` authority.
- Execution writes scheduled-rule register evidence through the board, active-task step state, and an audit row for task-driven evaluation.
- Persistence round-trips operation, rule id/name, reason, and manual trigger metadata in action-step JSON.
- Acceptance coverage verifies catalogue metadata, draft parsing, dispatcher-driven pause, and manual-trigger evaluation.

Design review note: this phase deliberately excludes task-executable rule create/edit. Those operations require nested action-plan and condition-expression authoring, so they should be designed with later recursive task/goal mutation rather than bolted into this lifecycle slice.

### 2026-06-19 controlled active-task administration progress

Epic 5 controlled active-task retry, reassignment, and cancellation is implemented for existing active tasks.

- The employment action catalogue now exposes executable `admintask retry|requeue|cancel <#|name|id> [reason]` and `admintask assign <#|name|id> to <employee id|name> [reason]` actions.
- Draft parsing resolves task selectors to durable active-task GUIDs at authoring time and resolves assignment targets to active employee identities.
- `ActiveTaskAdministrationActionStep` calls host task-board services for retry, requeue, assignment, and cancellation, carrying `AssignTasks` or `CancelTasks` authority according to the operation.
- Task-board retry and requeue reset the current blocked/failed/in-progress step safely to pending, preserve audit evidence, and refuse to move tasks with recorded physical task-item custody.
- Task-board assignment requires an active employment contract and validates the target worker can execute the next step before assigning.
- Persistence round-trips operation, target task id/name, optional employee id/name, and reason in action-step JSON.
- Acceptance coverage verifies catalogue metadata, draft parsing, dispatcher-driven cancel, blocked-task retry, and requeue-then-assign behaviour.



Design review note: this phase deliberately leaves `marktask completed|failed|blocked` deferred. Hard state marking can bypass dispatcher step ownership, payment/reservation cleanup, craft-reservation release, and physical custody safeguards; it should only be designed with explicit recovery semantics rather than as a simple task-action shortcut.

### 2026-06-19 controlled manager-goal administration progress

Epic 5 controlled goal mutation is implemented for existing manager-goal lifecycle and evaluation operations.

- The employment action catalogue now exposes executable `goal evaluate|cancel|reactivate <#|type|description> [reason]` actions with delegated `ModifyManagerGoals` authority.
- Draft parsing resolves manager-goal selectors to durable runtime goal ids at authoring time and persists operation, goal id/description, and reason metadata in action-step JSON.
- `ManagerGoalAdministrationActionStep` calls the host manager-goal board for single-goal evaluation, cancellation, and reactivation rather than mutating goal state directly.
- Targeted evaluation reuses the same manager-goal policy, idempotency, provenance, and task-spawn path as normal host goal evaluation.
- Reactivation is limited to blocked, failed, satisfied, or suspended goals; cancelled and completed goals remain terminal.
- Acceptance coverage verifies catalogue metadata, draft parsing, dispatcher-driven goal evaluation, blocked-goal reactivation, and cancelled-goal terminal behaviour.

Design review note: this phase deliberately excludes task-executable manager-goal create/edit and hard completion marking. Those operations require nested condition/action/policy authoring, durable policy/expression persistence, and explicit semantics for replacing or completing standing objectives, so they should be designed as a separate recursive-administration slice rather than hidden in lifecycle mutation.

### 2026-06-19 shop supplier-selection progress

Epic 6 shop supplier management has its first task-executable planning slice implemented.

- The employment action catalogue now exposes executable `supplier` / `findsupplier` planning actions with delegated `ManageStockRules` authority and purchase-capable worker capability requirements.
- `supplier` uses the same merchandise, item-selector, commodity-descriptor, supplier-selector, max-price, and keyword grammar as executable `purchase`, then wraps the parsed purchase target as a supplier-selection step.
- `SupplierSelectionActionStep` calls the native employment purchase resolver in preview mode, selecting the cheapest currently stocked matching supplier without requiring employer funds, reservations, payment authorisation, or worker presence at the supplier location.
- Execution records selected supplier, merchandise, price, quantity/weight, and supplier locations in step operational state plus employment audit-register evidence.
- Persistence round-trips supplier-selection steps with the same purchase payload shape used by executable purchase steps.
- Acceptance coverage verifies catalogue metadata, draft parsing, cheapest-supplier preview selection, operational payload evidence, selected-resource location evidence, and register evidence.

Design review note: this phase deliberately avoids making `supplier` rewrite later `purchase` steps. Cross-step variable binding, preferred vendors, supplier account policies, supplier budgets, and standing supplier contracts require a separate task-state/binding design so supplier selection does not silently alter financial or stock authority.

### 2026-06-19 shop stocktake progress

Epic 6 shop stocktake is implemented for task-executable stock count and weight evidence.

- The employment action catalogue now exposes executable `stocktake all` and `stocktake merch <id|name>` actions with delegated `ManageStockRules` authority.
- Draft parsing is shop-host only, resolves targeted merchandise to durable merchandise ids at authoring time, and persists scope/selector/name metadata in action-step JSON.
- `ShopStocktakeActionStep` calls native shop stocktake APIs: item merchandise uses `StocktakeMerchandise`, commodity merchandise uses `StocktakeMerchandiseWeight`, and all-merchandise stocktakes aggregate both forms into per-step operational payloads.
- Each execution writes employment audit-register evidence and stores count/weight payloads on the active-task step state for later review.
- Acceptance coverage verifies catalogue metadata, draft parsing for all/targeted stocktake actions, dispatcher-driven targeted stocktake, all-merchandise stocktake, item count payloads, commodity weight payloads, and register evidence.

Design review note: this phase intentionally treats discrepancy handling as auditable stocktake evidence rather than automatic inventory mutation. Write-off, reorder, loss, supplier claim, and manager-escalation policies need a separate native stock-control design so the employment task layer does not invent stock authority outside the shop subsystem.

### 2026-06-19 shop cash-reconciliation progress

Epic 6 shop cash reconciliation is implemented as a task-executable evidence and native-checking slice.

- The employment action catalogue now exposes executable `cashreconcile` actions with `reconcilecash`, `cashcheck`, and `tillcheck` aliases.
- Draft parsing is shop-host only, records optional worker notes, and requires both delegated deposit and withdrawal cash authority plus `CanHandleCash` worker capability.
- `ShopCashReconciliationActionStep` snapshots expected shop cash, virtual shop cash, same-currency physical till/float cash, variance, and post-check variance, then calls the native `CheckFloat` shop routine.
- Execution records reconciliation evidence in step operational state and the employment audit register without directly moving money.
- Acceptance coverage verifies catalogue metadata, draft parsing, native `CheckFloat` invocation, operational payload evidence, selected-resource evidence, and register evidence.

Design review note: this phase deliberately leaves shortage recovery, skim/fill decisions, employee-liability policy, and escalation behaviour to explicit finance or stock-control actions. `cashreconcile` tells the manager what the shop subsystem thinks happened; it does not invent a new money-movement authority.

### 2026-06-19 shop stock-transfer progress

Epic 6 shop stock transfer is implemented for explicit permanent-shop stockroom movement and cross-shop transfer evidence.

- The employment action catalogue now exposes executable `stocktransfer` actions with `transferstock` and `stockmove` aliases.
- Draft parsing is permanent-shop-host only, resolves the target permanent shop, target merchandise, target destination, and optional destination container at authoring time.
- `ShopEmploymentTaskService` can now create external stock-transfer active tasks from a source permanent-shop stockroom to a target permanent-shop destination, using the existing retrieval step followed by the new stock-transfer finalisation step.
- `EmploymentTaskContext` supports explicit additional logistics locations so a source-shop task can deliver to authorised target-shop cells without broadening the normal host logistics boundary.
- `ShopStockTransferActionStep` delivers carried task stock, calls native shop `DisposeFromStock` and `AddToStock` paths to retag source stock as target merchandise, and writes paired source/target employment register evidence with the active task correlation id.
- The transfer step is non-financial: it does not create sale proceeds, store-account debt, or employment ledger profit entries.
- Acceptance coverage verifies catalogue metadata, draft parsing, existing internal restock behaviour, external transfer task creation, target-location delivery, source stock disposal, target stock addition, operational payload evidence, and paired register evidence.

Design review note: this phase intentionally requires explicit source merchandise, target shop, target merchandise, and destination selection. Automatic surplus detection, transfer-vs-purchase choice, sibling-company transfer policy, multi-hop routing, commodity splitting, and preferred replenishment contracts still belong to manager-planning and stock-control policy rather than the low-level movement primitive.

### 2026-06-19 auction-house design review

Epic 6 auction-house work exposed one implementation boundary that should be documented before code proceeds: the native auction model already owns active lots, bids, settlement results, seller payout/refund queues, and unclaimed won or unsold items, but it does not have a first-class consignment intake record, valuation workflow, seller-consent record, or employee custody ledger for third-party lots outside the existing player `auction sell`, estate liquidation, and hotel lost-property flows.

Implementation will therefore proceed in a native-wrapper order:

- `auctionlist` will list a carried task-custody item as an auction-house-owned lot with explicit reserve, optional buyout, and optional duration. This covers staff handling of abandoned, confiscated, estate, hotel, or auction-house-owned stock without pretending to solve third-party consignment contracts.
- `auctionsettle` will settle due active lots through native auction completion so commission, bidder settlement, seller payout, result, refund, and unclaimed-item records remain owned by `IAuctionHouse`.
- `auctionclaim` will claim a selected unclaimed movable lot into task custody so normal employment delivery steps can handle won-lot or unsold-lot delivery.

External customer intake, staff valuation policy, deposits, minimum commission policy, employee authority to bind a third-party seller, and automatic delivery-route selection remain later auction-management design work rather than hidden assumptions in the first primitive.

### 2026-06-19 auction-house native-wrapper progress

Epic 6 auction-house workflow support is implemented for the native-wrapper slice described above.

- The employment action catalogue now exposes executable `auctionlist`, `auctionsettle`, and `auctionclaim` actions with auction-specific authority, capability, and payment-authorisation metadata.
- Draft parsing is auction-house-host only, resolves active/unclaimed native lots for settlement and claim steps, and uses the standard employment item selector grammar for listing task-custody items.
- `AuctionLotListingActionStep` lists a carried task-custody item as an auction-house-owned native `AuctionItem` with explicit reserve, optional buyout, and optional duration, while blocking items already tracked by any auction house.
- `AuctionSettlementActionStep` settles all due lots or one due lot through `IAuctionHouse.SettleFinishedAuctions` / `SettleAuctionItem`, keeping bid settlement, commission, seller payout/refund queue, result, and unclaimed-item records in the native auction subsystem.
- `AuctionClaimActionStep` claims a selected unclaimed movable lot through the native auction-house claim path, places it at the auction house, and collects it into normal task custody for downstream delivery steps.
- `IAuctionHouse` now exposes due-lot settlement methods for service-backed callers; auction-house-owned self-payout is treated as paid so auction-house-owned employment lots do not create artificial seller liabilities back to the same host.
- Acceptance coverage verifies catalogue metadata, draft parsing, due-lot settlement, and auction-house-owned self-payout handling. The broader command-parser regression also verifies the adjacent load/unload destination parser while this area was touched.

Design review note: this phase intentionally does not implement third-party consignment contracts, staff valuation rules, seller deposits, minimum commission policy, authority to bind outside sellers, or automatic buyer/seller route selection. Those remain explicit auction-management design work. The first primitive is a native bridge for staff-managed auction-house stock and native lot settlement, not a silent replacement for player `auction sell` or estate/liquidation policy.

### 2026-06-19 combat-arena event-administration progress

Epic 6 combat-arena event scheduling is implemented for the first native lifecycle wrapper slice.

- The employment action catalogue now exposes executable `arenaevent` actions with `event`, `arenaadmin`, and `arenaeventadmin` aliases under administration work.
- Draft parsing is combat-arena-host only and supports `arenaevent create <event type id|name> at <date/time>`, `arenaevent phase <event id|name> <state>`, and `arenaevent abort <event id|name> <reason>`.
- `ArenaEventAdministrationActionStep` creates native arena events through `ICombatArena.CreateEvent`, advances active events through `IArenaLifecycleService.Transition`, and aborts active events through `ICombatArena.AbortEvent` while recording employment register and step-state evidence.
- Persistence stores the arena id, operation, selected event type/event, target state, scheduled time, and abort reason, and reloads arena steps through the live `CombatArenas` catalogue.
- Acceptance coverage verifies catalogue metadata, command drafting, native create/phase/abort calls, audit evidence, and active-task persistence round-trip.

Design review note: this phase deliberately covers event lifecycle administration rather than the full arena-operations workflow. Competitor intake, purses/prizes, event staffing/security, equipment/stable preparation, medical follow-up, cleanup, recurring event policy, and automatic scheduling remain explicit arena-management follow-up work.

### 2026-06-19 bank native-wrapper design review

Epic 6 bank implementation exposed a real model boundary: banks have global reserves, account transactions, manager audit logs, and branch locations, but they do not yet persist teller rosters, branch-specific vault balances, or branch-local control state.

- This phase will implement task-executable native wrappers for reserve audit/deposit/withdraw, account credit/status/close, and branch post/courier evidence.
- Reserve movement will reconcile native `IBank.CurrencyReserves` with the bank employment host's `VirtualCashLedger`, giving an auditable counterbalance without inventing per-branch vaults.
- Account-service actions will use native `IBankAccount` methods and bank manager audit logs where those services already exist.
- Rollover, account opening, physical cash-in-transit custody, teller rosters, and per-branch controls remain future bank-service design work.

### 2026-06-20 bank native-wrapper progress

Epic 6 bank native-wrapper work is implemented for the current bank model boundary.

- Added the executable `bankadmin` action catalogue entry and authoring parser for reserve audit/deposit/withdraw, account credit/status/close, and branch post/courier evidence.
- Added `BankAdministrationActionStep` with native reserve mutation, `VirtualCashLedger` counterbalancing, scoped financial authorisation, bank manager audit logs, employment ledger/register evidence, branch validation, and persisted step state.
- Added a native `IBank.RecordManagerAuditLog` wrapper so task automation records through the existing `BankManagerAuditLog` model instead of inventing a parallel audit table.
- Added `BankAccountCredit` employment ledger evidence for account-service credits while leaving status and close changes as operational audit records.
- Acceptance coverage verifies catalogue metadata, command drafting, reserve deposit/withdraw execution, account credit/status/close execution, branch post/courier audit evidence, and active-task persistence round-trip.

No new large design decision was required beyond the design-review boundary already recorded above. Teller rosters, per-branch vault balances, physical cash-in-transit custody, account opening, and rollover remain explicit bank-service design work for a later phase.

### 2026-06-20 stable and hotel native-wrapper design review

Epic 6 stable and hotel work exposed the same kind of native-model boundary as banks. Stables already own lodging, return/release, fee assessment, stays, accounts, tickets, and stable ledgers, but they do not persist feed stock, grooming/exercise completion, welfare scores, capacity policy, or equipment-readiness state. Hotels already have durable roots, rooms, rentals, patron balances, and lost-property records, but durable room cleanliness/readiness/maintenance state and autonomous check-in/check-out/payment collection policy still belong in the hotel/property services rather than an employment-only note.

This phase therefore implemented safe native/evidence wrappers instead of creating shadow state:

- `stableadmin care <inspect|feed|groom|exercise>` records care evidence against an active stable stay.
- `stableadmin fees <all|stay>` calls native stable fee assessment and records the result.
- `stableadmin stay` and `stableadmin account` record stay/ticket and account reconciliation evidence against native stable rows.
- `hoteladmin room <inspect|clean|ready|maintenance>` records room-operation evidence against native hotel rooms.
- `hoteladmin lost check` calls the native property lost-property expiry sweep, while `hoteladmin lost audit` records evidence against an existing lost-property bundle.
- `hoteladmin balance` records patron-balance reconciliation evidence against native hotel patron-balance rows.

### 2026-06-20 stable and hotel native-wrapper progress

Epic 6 stable/hotel native-wrapper work is implemented for the current stable and hotel model boundary.

- Added executable `stableadmin` and `hoteladmin` catalogue entries, aliases, authoring parsers, runtime action steps, operational payloads, and persistence payloads.
- Added execution validation that stable actions run from the owning stable, path to the stable, and target active stays or native accounts; hotel room actions validate owning hotel rooms and pathability, while lost-property and patron-balance actions validate native property rows.
- Added acceptance coverage for catalogue metadata, command drafting, stable care/fee/account execution, hotel room/lost-property/patron-balance execution, and active-task persistence round-trip. The hotel persistence payload stores the owning property id because persisted hotel ids and property ids are not guaranteed to remain identical.

No further large design decision is required in this phase. Durable stable welfare/feed/capacity state, durable hotel room cleanliness/readiness/maintenance tickets, autonomous check-in/check-out/payment collection, and physical lost-property release/auction workflows remain explicit native-service follow-up work.

### 2026-06-20 candidate and labour-market depth progress

Epic 7 is implemented for the candidate-profile and offer-ranking slice that can be safely supported by existing runtime data.

- `EmploymentWorkerAI` now builds candidate profiles from visible character skills, derived skills, theoretical skills, knowledges, race, ethnicity, culture, chargen roles, visible skill/knowledge tags, and optional `IHaveTags` tokens.
- Candidate requirement matching now treats skill, knowledge, and tag names case-insensitively while preserving capability and payment-method exactness.
- `EmploymentCompensationEvaluator` centralises hourly-equivalent pay comparison for hourly, daily, weekly, and 30-day salary cadences, and worker search now ranks openings by normalised effective pay instead of raw nominal amounts.
- Worker search now uses employer overdue-payroll state both as an accept/reject reputation threshold and as a tie-breaker between otherwise comparable openings.
- Reachability is still required, and commute path length is now a tie-breaker when choosing between matching openings.
- Acceptance coverage verifies real worker-AI skill/knowledge/tag application matching, normalised effective-pay ranking, and the existing reservation-wage/payment-method requirement filter.

Design review note: schedule availability, local labour supply, reservation-wage modelling, training/progression, personality preferences, accepted-duty preferences, fatigue/capacity, expected per-task/commission/mixed earnings, and historical employer reputation require new persistent policy and labour-market inputs. They remain explicit future design work rather than hidden worker-AI heuristics.

### 2026-06-20 legacy bridge progress

Epic 8 is implemented for the read-only divergence-reporting slice.

- Added `EmploymentLegacyBridgeReporter`, which compares loaded employment hosts/contracts with loaded legacy `IJobListing`/`IActiveJob` records without mutating either system.
- The reporter classifies legacy active jobs without matching employment-host contracts, employment-host contracts without matching legacy jobs, and legacy active jobs whose employer is outside the employment-host model.
- Added founder-only `impdebug employment legacy` / `impdebug employment divergence` / `impdebug employment bridge` output for migration audits.
- Hotel discovery in the report uses the existing `Property.ExistingHotel` pattern so the report does not lazy-create hotel roots.
- Acceptance coverage verifies both divergence directions and reruns the existing boundary tests that keep `IJobListing` and `IActiveJob` out of `IEmploymentHost` and `EmploymentCommandService`.

Design review note: automatic bootstrapping of selected legacy staff into new employment contracts remains deferred. It needs a dedicated conversion design for role/authority mapping, pay cadence conversion, schedule and duration mapping, duplicate prevention, manager authority/consent, persistence audit, and rollback before it can safely create contracts from legacy PC-job state.

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


### 2026-06-19 implementation progress

Phase A items 1 and 2, plus the first recommended local-pricing slice, are implemented.

- `price market` is removed from action catalogue syntax and the authoring parser; old direct steps execute fail-closed and record an audit diagnostic.
- Persisted market-mode price payloads now reload as deprecated price steps, pause affected scheduled rules, block affected active tasks, block affected manager goals, and write employment register diagnostics containing the preserved market/category/impact/influence/expiry details.
- Native `sale` employment actions now create, modify, and cancel `ShopDeal` rows through the shop-deal model, including sale and volume deal types, target all/merchandise/tag, applicability, eligibility prog, cumulative flag, and expiry.
- Bounded exact merchandise repricing remains available through `price merch <id|name> <amount>` and still records native shop price-adjustment evidence plus employment audit evidence.
- Verification added: parser rejection for `price market`, fail-closed deprecated step execution, persisted rule/task/goal quarantine on load, native shop deal create/modify/cancel, sale/volume draft parsing, and scheduled rule execution for temporary sale plus volume deals.

No new large design decision was required for this phase. The only scope clarification is that the compatibility report lives in employment register entries rather than a new review table; that matches the existing operational-audit model and keeps the review queue design deferred until a broader builder-review workflow is specified.
The wage-disbursement slice below closes the next Phase A item.

### 2026-06-19 wage-disbursement progress

Phase A item 3 is implemented for the payment methods that have native settlement support.

- Payroll settlement now validates all accrued payable destinations first, validates the aggregate employer funding requirement by currency, and only then performs per-payable disbursement.
- Cash payables debit backed employer funds into claimable payroll escrow and may become `ReadyToClaim` for active or former employees.
- Employee-bank and specified-bank payables require a destination bank account in the payable currency; successful settlement debits the employer and credits the account through native bank-account transactions and reserve updates.
- Failed or unsupported destinations leave the payable `Accrued` and keep employer funds untouched.
- Verification added: backed-fund settlement, failed-funding preservation, bank-credit settlement, failed-bank-destination preservation, and former-employee cash claimability.

No new large design decision was required for this phase. The fuller `FundingReserved` / `Funded` / `Disbursing` / `Failed` state machine remains a future persistence migration; the current implementation keeps existing payable statuses while enforcing the design's economic invariant that a payable is not closed until value reaches a bank destination or funded cash escrow.
The worked-time slice below closes the next Phase A item.

### 2026-06-19 worked-time progress

Phase A item 4 is implemented for the current schedule representation.

- `WorkSchedule` now calculates scheduled duration overlap for explicit start/end windows, including overnight windows and partial periods.
- Hourly payroll uses scheduled duration for contracts with configured schedule windows, so an eight-hour shift accrues eight hours of wages rather than twenty-four.
- Paid `PerTask`, `Commission`, and `Mixed` contracts/openings now fail at creation with an explicit earning-record requirement instead of silently producing no or misleading periodic payroll.
- Verification added: scheduled eight-hour hourly payroll, unsupported-cadence rejection for contracts and openings, and implementor payroll debug coverage using a scheduled hourly contract.

No new large design decision was required for this phase, but the implementation deliberately avoids adding `EmploymentTimeRecord` / `EmploymentEarning` persistence tables until the broader attendance, task-completion earning, and commission-attribution model is specified. The employee-arrears slice below closes the next Phase A item.

### 2026-06-19 employee-arrears progress

Phase A item 5 is implemented for worker resignation and payroll-liability queries.

- `IEmploymentPayroll` now exposes employee-filtered outstanding-liability and maximum-overdue-day helpers.
- `EmploymentWorkerAI` uses employee-specific overdue days before resigning for unpaid wages.
- Host-wide `OutstandingLiabilities` and `MaximumOverdueDays` remain unchanged for manager views, payroll conditions, and prospective employer reputation checks.
- Verification added: a worker with no overdue payables does not quit merely because a coworker has overdue wages, while the existing own-arrears resignation test still passes.

No new large design decision was required for this phase. The deferred design work is the broader employer reliability score, which should account for host-wide arrears over currency/period once labour-market reputation is modelled.
The application-and-lifecycle slice below closes the next Phase A item.

### 2026-06-19 application-and-lifecycle progress

Phase A item 6 is implemented for the runtime acceptance, seat, and fixed-term duration invariants covered by the current acceptance gates.

- Job openings now maintain a runtime revision number; modification and closure increment it.
- Applications record the offered runtime revision and in-memory candidate profile; acceptance rejects closed/full openings, stale revisions, and in-memory candidate profiles that no longer match.
- Duplicate pending applications for the same candidate/opening return the existing pending application rather than creating another domain row.
- Accepted contracts record their originating opening and application in memory; opening occupancy counts active/suspended originating contracts, so ending a contract releases the seat.
- Fixed-term contracts expire through host lifecycle evaluation, and payroll invokes that evaluation before accrual so final partial payables remain possible.
- Verification added: closed/stale application acceptance rejection, reopened seat after originating contract termination, and fixed-term final partial payroll.

No new large design decision was required. Follow-up review on 2026-06-20 added EF schema support for durable opening revisions, offered application revisions, candidate-profile snapshots, and contract origin columns, so revision and seat fidelity now survive reloads. Immutable term snapshots and candidate-consent state remain future work.
The task provenance, priority, and explicit-principal slice below closes Phase A item 7.

### 2026-06-19 task provenance, priority, and principal progress

Phase A item 7 is implemented for runtime task ordering and grant-backed payment authorization.

- Active tasks now expose runtime provenance: source kind, source rule/goal ids, created-by principal, authorised-by principal, authorisation grant, priority, due/created/assigned/started/completed timestamps, and correlation.
- Manual tasks, scheduled-rule spawns, and manager-goal spawns mint explicit runtime principals and authorisation grants rather than relying on a nullable actor as unlimited trust.
- Manager goals evaluate in priority order, propagate priority into spawned tasks, and stamp spawned work with manager-goal source/principal/grant metadata.
- Worker AI chooses among active-contract hosts by assigned work first, then pending task priority/due/created/id, and claims pending tasks within a host by that same priority/due/created/id order.
- Financial steps consume the active task grant or explicit context authorisation; a worker completing an `authorise` action step no longer unlocks employer spending by itself.
- Verification added: goal priority/provenance, worker task-claim priority within and across hosts, existing grant amount coverage, persisted-grant fail-closed reload behaviour, and the self-authorisation rejection case.

No new large design decision was required. This phase deliberately keeps provenance/grants runtime-only; loaded tasks without persisted grants fail closed for financial authority. Durable active-task provenance, grant revocation, retry/escalation policy, and host-wide budget/concurrency caps remain for later schema and scheduler work.
The central host operations scheduler slice below closes the Epic 1 scheduler requirement.

### 2026-06-19 central host operations scheduler progress

The Epic 1 host-operations scheduler requirement is implemented for current runtime services.

- Added `EmploymentHostOperationsScheduler` with host-level and gameworld-level evaluation entry points and countable result summaries.
- The scheduler evaluates scheduled rules, manager goals, payroll accrual, fixed-term contract lifecycle expiry, and active-task assignment audits through the existing owning services.
- Worker AI now routes host-work evaluation through the central scheduler rather than directly calling scheduled-rule and manager-goal boards.
- Verification added: one scheduler pass can spawn scheduled-rule work, spawn manager-goal work, accrue payroll, and expire a fixed-term contract.

No new large design decision was required. Remaining scheduler architecture work is host-wide locking/idempotency, stale grant/reservation cleanup, retry/escalation policy, and replacing hard-coded host discovery with capability-provider registration.
This closes the Epic 1 employment-semantics closure slice implemented in this pass. The native account-transfer slice below starts Epic 2.

### 2026-06-19 native account-transfer progress

Epic 2 account-to-account transfer is implemented for the currently supported same-currency bank-account path.

- `transfer <amount> to <bank account id|bankcode:account|alias>` is now an executable catalogue-backed employment action with `UseStoreAccount` authority, `CanUseBankAccount` capability, financial-step marking, payment-authorisation requirement, parser support, and persistence round-trip support.
- Execution resolves the employer linked bank account and target active native bank account through the host gameworld, requires a matching currency, rejects self-transfers, and calls the native paired transfer methods rather than writing employment-only accounting records.
- Successful execution consumes the task reservation, records `AccountTransfer` employment ledger evidence, writes a `PaymentAuthorisationUsed` register entry, and stores the native transaction/reservation references in step operational state.
- Verification added: successful transfer mutates both native bank-account balances and writes employment evidence; a different-currency destination blocks with funds untouched; the existing bank deposit/withdrawal regression still passes.

No new large design decision was required, but the implementation deliberately keeps this slice same-currency. Native bank commands can apply exchange rates, but employment grants and reservations currently authorise one amount/currency. Cross-currency transfers should wait for a source/destination amount policy and audit wording that makes both sides explicit. The next Epic 2 phase is host-to-host settlement and broader transfer/counterparty limits.

### 2026-06-19 host settlement progress

Epic 2 host-to-host settlement is implemented for same-currency supported employment hosts.

- `settle <amount> to <host type> <id|name>` is now an executable catalogue-backed employment action with `SettleHostAccounts` authority, `CanUseBankAccount` capability, financial-step marking, payment-authorisation requirement, parser support, and persistence round-trip support.
- Execution resolves the target employment host through the source host gameworld, rejects self-settlement, requires both hosts to use the same currency, consumes the task reservation, debits the source host's backed available funds, and credits the target host's virtual cash through native finance records.
- Successful execution records `HostSettlement` ledger evidence on both source and target hosts, writes source `PaymentAuthorisationUsed` and target audit register entries with the same task correlation, and stores transaction/reservation references in step operational state.
- Verification added: successful shop-to-shop settlement mutates source and target `VirtualCashLedger` balances and writes source/target employment evidence; a different-currency target blocks with both hosts untouched; the account-transfer and bank deposit/withdrawal regressions still pass.

No new large design decision was required, but this phase deliberately keeps settlement same-currency and host-local. Parent/child intercompany settlement, consolidated profit elimination, cross-currency conversion, and counterparty-specific grant limits still need the later organisation/accounting model.

### 2026-06-19 grant-scope and finance review progress

Epic 2 runtime grant scoping is implemented for the current same-currency finance paths.

- Review found that cash wage payroll escrow was already closed by the wage-disbursement slice: employer funds are debited into claimable payroll escrow and are not treated as settled until the employee can claim or receives a bank credit.
- Review found that the basic physical employee-float lifecycle is already executable: `physicalfloat issue`, `physicalfloat return`, and `physicalfloat settle` move task-custody cash through reservations, native bank/register/virtual-cash paths, and employment evidence. Richer recovery policy for lost, stolen, or partially returned employee float remains future work.
- Runtime `EmploymentAuthorisationGrant` now carries derived payment-source and counterparty scopes for financial steps in addition to authority and amount limits. Durable payment authorisation refuses account-transfer or host-settlement steps whose source or counterparty is outside the grant scope.
- Verification added: otherwise valid same-currency account transfers and host settlements block before native movement when the grant names a different counterparty; existing successful transfer/settlement and currency-mismatch regressions still pass.

No new large design decision was required. The implementation deliberately keeps scope derivation runtime-only and set-based, matching the current runtime grant model. Durable grant persistence, revocation, source/counterparty route-pair matrices, cross-currency exchange-rate policy, and parent/consolidated settlement remain deferred to the future organisation/accounting model.

### 2026-06-19 craft station capacity progress

Epic 3 station capacity is implemented for item-backed craft station reservations.

- Employment craft station reservations now read `EmploymentCraftStationReservationCapacity`; invalid, missing, or non-positive values preserve the prior capacity-one behaviour.
- Station reservation conflict checks now count active reservations from other task correlations and admit another task only while occupancy is below capacity. Full stations block with an occupancy diagnostic and do not add a new station reservation effect.
- Craft station step state records the capacity used alongside the reservation expiry for manager inspection.
- Verification added: default capacity-one stations still block a conflicting active reservation, while a configured capacity-two station accepts a second concurrent employment reservation and records the new task correlation.

Design review note: true persisted station queues, automatic retry, preemption, and escalation remain deferred. They should be implemented with the broader task retry/escalation policy rather than hidden inside the station validator.

### 2026-06-19 current-step scheduling and handoff progress

Epic 3 priority and multiple-worker scheduling is implemented for the safe no-custody handoff slice.

- Task dispatch now evaluates candidate AI capabilities against the next pending action step rather than the whole action plan, allowing a worker to claim and complete a step they can actually perform in a mixed-capability task.
- After a successful completed step, the dispatcher rehydrates the next step and releases the current worker back to `Pending` when that next step cannot be executed by the same worker and the worker is not carrying task items.
- Requeued step-boundary handoffs write `ActiveTaskRequeued` register evidence and preserve the existing blocked-for-manager-review behaviour when physical task custody exists.
- Verification added: a manager-authorised plan can be claimed by a planning worker for a shell step, requeued at the craft-station boundary with no carried items, and then claimed by a craft-capable worker.

Design review note: persisted sub-assignments, multiple simultaneous assignees, role-specific queues, and physical custody transfer are still large design surfaces. The current implementation deliberately avoids automatic reassignment when task items are in worker custody; that remains a manager-review recovery path until a custody handoff model is designed.

### 2026-06-19 production-chain custody progress

Epic 3 production chains are implemented for the basic sequential-craft custody slice.

- `craft-v2` operational state now records `TaskInputItemIds`, populated from carried task-custody items when a craft starts or resumes.
- A first craft step can complete, adopt newly produced items into task custody, and leave those item ids available for later craft steps through normal task-state hydration.
- A later craft step records the carried intermediate output ids in its craft state, giving manager inspection and future recovery/salvage code a durable chain link between the producer craft and the consumer craft.
- Verification added: a two-step craft chain adopts an intermediate output from the first craft, rehydrates it as carried task custody, and records that item id as a task input when starting the second craft.

Design review note: this is not an automatic production planner. Managers and scheduled rules still compose chain steps explicitly, and persisted queueing, demand-aware expansion, and manager-directed salvage remain later phases.

### 2026-06-19 failed-craft salvage progress

Epic 3 failed-craft recovery is implemented for failed native craft components that still have employment craft state.

- `TryStartCraft` now detects failed active craft components before attempting resume and routes them through a recovery path when the task has prior employment craft state.
- The recovery path adopts visible, non-active craft salvage items that were not present before the craft started, records them as task custody, stores `craft-status=failed`, and writes a failure diagnostic into step state.
- `CraftTriggerActionStep` interprets `craft-status=failed` as a failed-but-completed step, so the active task fails closed for manager review rather than repeatedly retrying or pretending the craft succeeded.
- Verification added: a dispatcher-started craft whose native component later fails adopts a salvage item, records the failed craft payload/diagnostic, and transitions the task to `Failed`.

Design review note: failed crafts without prior employment craft state still block instead of guessing salvage provenance. Manager-directed salvage selection, partial salvage choices, quality-based triage, and automatic rework planning remain later phases.

### 2026-06-19 inspection and quality-control progress

Epic 3 inspection and quality-control is implemented for task-custody item inspection notes.

- The task action catalogue now exposes executable `inspect`/`qc`/`quality` production actions with `ManageCraftRules` authority and `CanCraft` worker capability requirements.
- `inspect` requires the worker to have task-custody items, records an inspection payload, and persists an `operation=inspect` selected-resource record containing the inspected item ids without changing custody.
- Verification added: a worker retrieves a task item, completes an inspection step, records the item id/note in step state, and still retains task custody afterwards.

Design review note: this slice records auditable QC evidence but does not yet score item quality, split pass/fail groups, trigger rework, or route rejects. Those remain follow-up quality-control and manager-autonomy work.

### 2026-06-19 materials replenishment progress

Epic 3 materials replenishment is implemented for manager-goal-driven replenishment plans.

- The existing `craftmaterials` goal family combines commodity/item threshold conditions with authorised purchase, retrieve, commodity, and delivery action plans.
- Acceptance coverage now verifies a `MaintainCraftMaterialSupply` manager goal with a low commodity threshold creates a manager-goal-sourced replenishment task, preserves priority, and carries the configured authorise/reserve/purchase plan.

Design review note: this confirms the runtime automation seam, but it does not yet auto-select suppliers, derive purchase quantities from craft recipes, or tune batches from demand/storage. Those remain the demand-aware batch sizing phase and later manager autonomy work.
