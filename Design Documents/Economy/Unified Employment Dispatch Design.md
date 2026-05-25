# Unified Employment, Manager Goals, and Task Dispatch Design

## 1. Purpose

The economy system currently has several independent concepts that are all variations of the same idea: a business or institution has proprietors, managers, employees, money, inventory, duties, operating policies, and recurring work. Shops, auction houses, arenas, banks, stables, and hotel businesses each expose some subset of these behaviours, but they do not share a common employment and task-dispatch model.

This design introduces a unified employment layer, a manager-goal layer, and a composable task-dispatch layer.

The employment layer answers: who can hire, who is employed, under what terms, with what authority, and how they are paid.

The manager-goal layer answers: what standing business objectives have been delegated to managers, what authority they require, and when manager AIs should create or adjust operational work.

The task-dispatch layer answers: what conditions create work, what action steps make up that work, who can perform each step, how money/items/vehicles/accounts are authorised, and how the resulting activity is recorded.

The immediate intent is to create a foundation that can support store deliveries, stock purchasing, cash handling, crafting triggers, employee and manager delegation, board communication, and future organisation-specific work without duplicating the same mechanics in each business system.

## 2. Boundary with the existing PC-facing job system

The existing PC-facing job system is out of scope for this change.

Current code has PC-facing job concepts such as `IJobListing`, `IActiveJob`, job-finding cells, payroll/coffer arrangements, and the `job` command surface. Those systems are intended for player-character job discovery, active PC jobs, and related payroll workflows. They should be treated as similar but unrelated systems whose semantics do not define the new host-level employment model.

This design should not require `IJobListing`, `IActiveJob`, job-finding cells, payroll/coffer code, or the `job` commands to be replaced, renamed, refactored, or made to implement the new employment-host model.

A future project may bridge the systems so that a PC-facing job can grant employee, manager, proprietor, or other employment status in an `IEmploymentHost`. That bridge is deliberately not part of this implementation slice. Until such a bridge exists, the new employment system should define its own contracts, roles, employment openings, authority model, payment terms, applications, and task eligibility without depending on current PC job semantics.

Terminology note: this document uses **employment opening** for the new host-level labour-market posting. The design should avoid legacy job-posting names for new concepts unless a later bridge explicitly needs them.

## 3. Scope

### In scope for the first production slice

- A common employment-host interface named `IEmploymentHost`.
- Host shells first: shops, auction houses, arenas, banks, stables, and hotels expose `IEmploymentHost` with contracts, authority, employment openings, a real host board reference, task board, manager-goal board, and operational register support before real dispatcher execution is required.
- Common domain objects for employment contracts, roles, manager authority, employment openings, applications, payment terms, working hours, and employment duration.
- A simple employment status model with no probation concept.
- Manager delegation: authorised managers can manage employees within their granted authority, including hiring, firing, creating employment openings, and eventually assigning or creating tasks.
- NPC-facing employment openings, with requirements expressed in skills, knowledges, and AI capability types.
- An NPC employment-seeking AI behaviour that can find suitable employment openings, compare offers, apply, and refuse offers below its reservation wage.
- Employee payment handling: cash payment, payment to an employee bank account, specified employer or employee account arrangements, and future support for employee payment items or floats.
- A host-level reference to a real persisted `IBoard` that employees can view and post to without requiring a separate in-world board item link.
- A composable task-board model based on scheduled rules, condition types, action plans, and active task instances.
- Initial condition and action-step definitions for stock thresholds, time triggers, manual orders, purchasing, delivery/logistics, command execution, bank deposit/withdrawal, store-account payment, board posting, and craft triggering. These definitions should be designed and persisted where needed during the host-shell slice, but actual dispatcher execution can be deferred to a later milestone.
- Manager goals that allow manager AIs to create active tasks or scheduled rules in pursuit of business objectives such as maintaining stock, paying accounts, paying taxes, or adjusting prices.
- Integration with existing financial ledgers/transaction records plus a new operational employment/task/register layer for employment, wages, manager actions, task orders, purchasing, cash movements, store-account payments, board-post actions, and task outcomes.
- Separate persisted `IHotel`/`Hotel` entities as economy hosts, with properties remaining the ownership, location, and access-control anchor for hotel rooms.
- Command compatibility where practical, using adapters/shims during adoption if direct replacement would create avoidable regressions.

### Explicitly out of scope

- Changes to existing `IJobListing`, `IActiveJob`, job-finding cell, payroll/coffer, or `job` command behaviour.
- Migration of legacy employment data.
- Preservation of obsolete legacy employee, manager, proprietor, wage, or employment records.
- Migration of current property-hosted hotel XML into separate hotel entities unless it is deliberately included in the first hotel persistence milestone.
- Perfect optimisation of employee routing, vehicle loading, delivery batching, or market arbitrage.
- Full autonomous hiring strategy for every business type.
- Sophisticated labour economics, worker unions, employment law, morale systems, or probationary employment beyond a basic reservation wage and market-rate linkage.
- Unbounded task scripting. Arbitrary command execution must be permissioned, auditable, and constrained.

## 4. Legacy data stance

Legacy employment data does not need to be migrated into the new contract model. Existing MUDs using the engine are not relying on the current employment-like mechanics in a way that should constrain this redesign. It is acceptable for legacy employment records to be discarded, ignored, or made obsolete by the new system.

The implementation should still avoid unnecessary crashes when loading old saved data. The preferred stance is:

- Preserve non-employment business data where it remains meaningful, such as shop inventory, bank account balances, auction listings, room definitions, hotel/property state, and tax/accounting records that are not part of obsolete employment state.
- Initialise new employment contracts, employment openings, task boards, manager goals, host board references, and operational registers from clean defaults.
- Do not import existing shop/stable employee XML, bank manager lists, or arena manager lists into new employment contracts.
- Do not spend implementation time building elaborate legacy employment-data migration code.
- If obsolete fields remain in save files, either ignore them or map them only when doing so is trivial.
- If removing obsolete fields is cleaner than adapting them, removal is acceptable provided the new baseline loads and operates correctly.

This stance applies specifically to legacy employment-like data. It is not permission to destroy unrelated economic state.

## 5. Design principles

1. **One employment model, many host adapters.** Each business type may keep its specific behaviour, but staff, roles, employment openings, manager goals, boards, and task assignment should route through common services.
2. **Relationships are records, not flags.** Employment should be represented by an explicit contract/relationship object rather than scattered character references or ad hoc owner fields.
3. **Authority is delegated, not implied.** A manager can only do what their role/contract permits, and cannot delegate authority they do not possess.
4. **Tasks are composable action plans.** The system should not depend on large hardcoded end-to-end task types that each combine a trigger and a full workflow. Scheduled rules evaluate conditions; active tasks execute ordered action steps.
5. **Manager goals create operational work.** Some business objectives are better represented as delegated manager goals than as simple condition/action schedules.
6. **Tasks are auditable work orders.** Any task that spends money, moves inventory, pays wages, posts official notices, alters accounts, or changes prices must write through the existing financial evidence systems and the new operational register as appropriate.
7. **Scheduled rules spawn active work.** A standing rule such as "buy butter when stock is low" should be distinct from the active task that an employee is currently performing.
8. **Command compatibility before command replacement.** Existing player-facing commands can become wrappers around common services where practical.
9. **Economic exploit resistance.** NPC workers should reject implausibly low pay, employer spending must be authorised, and payments must be traceable.

## 6. Terminology

- **Employment host / `IEmploymentHost`:** A shop, auction house, arena, bank, stable, hotel, or future organisation that can employ NPCs or player characters through the new employment model.
- **Hotel / `IHotel`:** A separate persisted economy host for hotel business state. A hotel links to property/cell data for ownership, location, and access control, but owns hotel rooms, rentals, patron balances, lost property, tax configuration, manager workflows, bank account, virtual reserve, and operational state.
- **Employee agent:** A character, usually an NPC for this design, capable of accepting work and performing action steps.
- **Employment contract:** The active or historical relationship between an employment host and an employee agent, including role, pay, schedule, duration, status, payment method, and authority.
- **Employment role:** Employee, manager, proprietor, or a more specific host role. Roles may grant default authority, but the final authority must be explicit.
- **Manager authority:** The set of actions a manager can perform on behalf of the host, such as hiring employees, firing employees, changing pay within limits, creating employment openings, approving store-account use, creating scheduled rules, or assigning tasks.
- **Employment opening / `IEmploymentOpening`:** A posted position with requirements, pay terms, schedule, duration, role type, and application rules. It is unrelated to `IJobListing` and `IActiveJob`.
- **Host board / `IBoard`:** A real persisted board record referenced by an `IEmploymentHost`, visible to employees through their employment relationship rather than through a physical board item.
- **Manager goal:** A standing objective delegated to a manager AI, such as maintaining minimum stock levels or keeping store accounts paid. Manager goals may create scheduled rules, create active tasks, alter prices, or post board notices when the manager has authority.
- **Scheduled task rule:** A recurring, conditional, or manual rule that evaluates condition types and spawns active tasks from an action plan.
- **Condition type:** A reusable predicate or trigger input, such as time of day, stock below threshold, account balance below threshold, tax due, store account overdue, manual order, or market price condition.
- **Action plan:** An ordered, composable set of action steps that can be executed by one or more employees.
- **Action step:** A single operation in an action plan, such as move to location, find supplier, reserve funds, purchase commodity, load container, deliver item, trigger craft, deposit money, pay account, execute command, or write board post.
- **Active task:** A concrete work item created from a scheduled rule, manager goal, proprietor order, manager order, or system event.
- **Task dispatcher:** The service that evaluates active tasks, chooses eligible employees, assigns work, advances action steps, and records outcomes.
- **Financial evidence:** Existing financial records used for tax/accounting, such as bank account transactions, shop transaction records, stable ledgers, arena finance records, `VirtualCashLedger`, and other host-specific money movement records. This design should reuse these instead of introducing a parallel generic financial ledger.
- **Register:** New operational audit record: employment openings, applications, hires/fires, manager actions, goal changes, task creation, task assignment, task completion/failure, board posts, and authorisations.

## 7. Core interfaces and domain model

The exact names should follow the existing codebase style. The following is intentionally C#-shaped.

```csharp
public interface IEmploymentHost
{
    long Id { get; }
    string EmploymentHostName { get; }
    EmploymentHostType EmploymentHostType { get; }
    IMarket? Market { get; }

    IBoard HostBoard { get; }
    IEmploymentRegister EmploymentRegister { get; }

    ITaskBoard TaskBoard { get; }
    IManagerGoalBoard ManagerGoalBoard { get; }

    IReadOnlyCollection<IEmploymentContract> EmploymentContracts { get; }
    IReadOnlyCollection<IEmploymentOpening> EmploymentOpenings { get; }

    bool CanEmploy(ICharacter candidate, EmploymentRole role, out string reason);
    IEmploymentContract Hire(ICharacter candidate, EmploymentOffer offer, ICharacter? authorisedBy);
    void Fire(IEmploymentContract contract, EmploymentTerminationReason reason, ICharacter? authorisedBy);
    bool HasAuthority(ICharacter actor, EmploymentAuthority authority);
}
```

Recommended supporting interfaces/classes:

```csharp
public interface IEmploymentContract
{
    long Id { get; }
    IEmploymentHost Employer { get; }
    ICharacter Employee { get; }
    EmploymentRole Role { get; }
    EmploymentStatus Status { get; }
    EmploymentAuthoritySet Authority { get; }
    CompensationTerms Compensation { get; }
    WorkSchedule Schedule { get; }
    EmploymentDuration Duration { get; }
    PaymentMethod PaymentMethod { get; }
    DateTimeOffset StartedAt { get; }
    DateTimeOffset? EndsAt { get; }
    EmploymentTerminationReason? EndReason { get; }
}

public enum EmploymentStatus
{
    Active,
    Suspended,
    Ended
}

public sealed class EmploymentOpening
{
    public long Id { get; init; }
    public IEmploymentHost Employer { get; init; }
    public EmploymentRole Role { get; init; }
    public EmploymentRequirementSet Requirements { get; init; }
    public CompensationTerms Compensation { get; init; }
    public WorkSchedule Schedule { get; init; }
    public EmploymentDuration Duration { get; init; }
    public EmploymentOpeningStatus Status { get; set; }
    public int MaxPositions { get; init; }
    public bool NpcApplicationsOnly { get; init; }
}

public sealed class EmploymentRequirementSet
{
    public IReadOnlyCollection<SkillRequirement> Skills { get; init; }
    public IReadOnlyCollection<KnowledgeRequirement> Knowledges { get; init; }
    public IReadOnlyCollection<AICapabilityRequirement> Capabilities { get; init; }
    public IReadOnlyCollection<TagRequirement> Tags { get; init; }
}
```

`IEmploymentHost` is a business-facing interface, not a character-facing one. Characters are candidates, employees, managers, or proprietors of an employment host; they are not the host itself.

## 8. Employment contracts

An employment contract should be the single source of truth for an active or historical employment relationship. It should store:

- Employer reference.
- Employee reference.
- Role: employee, manager, proprietor, or business-specific role.
- Status: active or ended.
- End reason where applicable: fired, resigned, expired, host closed, manually cancelled, or system-cancelled.
- Compensation terms.
- Schedule/hours.
- Duration: indefinite, fixed term, seasonal/limited term, or task-limited.
- Payment method: cash, bank account, specified account, payment item, or employer float.
- Manager authority set.
- Created/started/ended timestamps.
- Created by / terminated by / last modified by references for auditability.

Do not model probationary employment in this design. If a future system needs probation, it can be added as a policy or contract term later; it should not complicate the baseline status model.

Contracts should be historical. Firing or expiry should end the active contract, not delete it. This is important for tax records, wage disputes, task attribution, and audit trails.

Applications and offers should be separate from employment contracts. A candidate who has applied to an employment opening or received an offer does not need a contract in a pending or probationary state until the offer is accepted and employment begins.

## 9. Roles and manager authority

Roles should be separated from permissions. A "manager" role is only useful if it has an explicit authority set. Example authorities:

- `ViewEmployees`
- `HireEmployees`
- `FireEmployees`
- `CreateEmploymentOpenings`
- `ModifyEmploymentOpenings`
- `SetPayWithinBand`
- `AssignTasks`
- `CancelTasks`
- `CreateScheduledRules`
- `ModifyScheduledRules`
- `CreateManagerGoals`
- `ModifyManagerGoals`
- `ApprovePurchases`
- `UseStoreAccount`
- `WithdrawBusinessCash`
- `DepositBusinessCash`
- `ManageStockRules`
- `ManageCraftRules`
- `ManageDeliveryRoutes`
- `AdjustPrices`
- `PayTaxes`
- `PostToHostBoard`
- `ModerateHostBoard`

Manager rules:

- A manager can only exercise authority granted by their contract.
- A manager cannot grant another employee authority they do not have.
- A manager cannot fire, demote, or overrule a proprietor unless explicitly allowed by the host type.
- A manager cannot create a manager goal that would require authority they do not possess.
- A manager cannot create a scheduled rule or active task that uses employer money, stock, accounts, vehicles, animals, rooms, or store credit unless their authority permits that use.
- Any manager action that changes employment, money, inventory, accounts, prices, boards, goals, or tasks must write a register entry, and financial actions must write ledger entries.

## 10. Employment openings

An employment opening should be the canonical way for NPC workers to discover host-level work. It should support:

- Employer.
- Role type: employee, manager, proprietor-like operator if appropriate.
- Required skills and minimum thresholds.
- Required knowledges.
- Required AI capability types, such as `CanPurchaseCommodities`, `CanDeliverItems`, `CanUseBankAccount`, `CanUseVehicles`, `CanCraft`, `CanExecuteCommandTask`, `CanPostToBoard`, or `CanManagePrices`.
- Required tags/traits where the existing character system supports them.
- Rate of pay.
- Pay type: hourly, daily, weekly, per-task, salary, commission, mixed.
- Market-rate linkage: fixed value, market-rate multiplier, market-rate floor, or market-rate plus premium.
- Hours/schedule.
- Employment duration.
- Number of positions.
- Whether the opening is NPC-only for now.
- Application policy: auto-accept best qualified, require manager approval, require proprietor approval, or first-qualified.

Recommended wage representation:

```csharp
public sealed class CompensationTerms
{
    public MoneyAmount? FixedRate { get; init; }
    public MarketRateBinding? MarketBinding { get; init; }
    public PayCadence Cadence { get; init; }
    public MoneyAmount? MinimumEffectivePay { get; init; }
    public PaymentSource EmployerPaymentSource { get; init; }
}
```

An employment opening should not be allowed to offer a non-positive wage unless it is explicitly marked as unpaid/volunteer and NPC employment-seeking AI is allowed to ignore it by default.

## 11. NPC employment-seeking AI

Create an AI behaviour that periodically searches for employment openings. The behaviour should:

1. Find visible employment openings in the relevant market, region, or reachable world area.
2. Filter openings by requirements: skills, knowledges, capabilities, tags, availability, and pathing.
3. Calculate effective pay after schedule, distance/travel burden, payment reliability, and any market-rate binding.
4. Compare effective pay against the NPC's reservation wage.
5. Apply to the best qualifying opening.
6. Accept offers only if the final contract still satisfies reservation-wage and payment-method constraints.
7. Prefer payment to an existing bank account if available, open a bank account if allowed and needed, otherwise accept cash or a specified payment item if supported.

Reservation wage should avoid the exploit where players set the absolute minimum salary and absorb excess unemployed workers. Suggested formula:

```text
reservation_wage = max(
    species_or_culture_minimum,
    subsistence_basket_cost_per_pay_period,
    skill_market_rate * skill_multiplier,
    local_unemployment_adjusted_floor
)
```

Where the existing market system has population and price data, use it. If not, begin with fixed defaults and make the calculation injectable/configurable.

The AI should record rejected openings and reasons in a lightweight way to avoid thrashing: below minimum wage, lacks capability, cannot path, no acceptable payment method, employer full, opening closed, or application rejected.

## 12. Payment and banking

Employees should support these payment modes:

- **Cash:** employer pays physical currency to employee.
- **Employee bank account:** wages are deposited into an account owned by or assigned to the employee.
- **Specified bank account:** contract names the account to receive pay, subject to validation.
- **Payment item / float:** employer gives an item, purse, token, or cash float used for tasks or wages.
- **Hold wages / unpaid balance:** if immediate payment fails, a payable liability is recorded rather than silently losing the debt.

NPC employment-seeking AI should be able to:

- Use an existing account.
- Open an account at a suitable bank if the bank system supports this and the NPC can path/interact with it.
- Accept cash if banking is unavailable.
- Reject jobs requiring an unacceptable payment method.

All wage payments should write through the appropriate existing financial systems. Bank payments should create account transaction records. Cash or virtual payments should use the existing host-specific money records, such as shop transaction records, stable ledgers, arena finance records, or `VirtualCashLedger` as appropriate. The new employment layer should add operational register correlation, not a parallel general-purpose wage ledger.

## 13. Host board

Every `IEmploymentHost` should expose a reference to a real persisted `IBoard` directly through the host. Employees should be able to view and post to this board according to their employment contract and authority without requiring a separate item link to an in-world board object.

The host board can be used for:

- Internal instructions.
- Manager announcements.
- Employee task notes.
- Delivery or stock warnings.
- Automated posts from action plans.
- Audit-friendly summaries of important operational changes.
- Optional player-facing notices if the host type permits them.

The host board should support at least:

- View permissions by role/authority.
- Post permissions by role/authority.
- Optional moderation authority for managers/proprietors.
- Posts linked to employment contract, manager goal, scheduled rule, active task, or action step where applicable.
- Register entries for official or automated board posts.

Recommended interface shape:

```csharp
public interface IEmploymentHostBoardAccess
{
    IBoard HostBoard { get; }
    bool CanViewBoard(ICharacter actor);
    bool CanPostToBoard(ICharacter actor, BoardPostType postType);
    IBoardPost PostToBoard(ICharacter actor, BoardPostRequest request);
}
```

The board is part of the host's operational surface and should be backed by the existing persisted board implementation. Employees should not need to know or carry a physical board reference to use it.

## 14. Task model: conditions, action plans, and active tasks

The task system should be composable. Avoid creating large monolithic task types such as “purchase butter task” or “carrot delivery task” that encode both the trigger condition and the full workflow as a special case.

Instead, use this structure:

```text
Scheduled task rule
  -> evaluates one or more condition types
  -> instantiates an active task from an action plan
  -> dispatcher assigns eligible employee(s)
  -> employee executes ordered action steps
  -> existing financial records and operational register entries are written throughout
```

One-off manager or proprietor orders can skip the scheduled-rule layer and create an active task directly from an action plan.

A scheduled task rule should contain:

- Trigger type: periodic evaluation, time-based recurrence, event hook, or manual evaluation.
- Conditions: one or more condition types combined with `all`, `any`, or a small explicit expression model.
- Action plan template to instantiate when conditions are satisfied.
- Cooldown/idempotency key to prevent duplicate task storms.
- Priority.
- Required employee capabilities.
- Required authorisations and payment source.
- Register classification and existing financial-record correlation.

An active task should contain:

- Employer.
- Created by / ordered by.
- Source: scheduled rule, manager goal, manager order, proprietor order, board order, or system.
- Action plan and current action-step state.
- Required capabilities.
- Required authority/payment source.
- Assigned employee or employee group.
- Priority.
- State.
- Failure/blocker reason.
- Created/assigned/started/completed timestamps.
- Ledger/register correlation IDs.

Suggested lifecycle:

```text
Draft -> Ready -> Assigned -> InProgress -> Blocked -> Completed
                              \-> Failed
Ready/Assigned/InProgress ----> Cancelled
```

## 15. Condition types

Condition types are reusable predicates or trigger inputs. They should be small enough to compose and inspect. Initial condition types should include:

### Time and recurrence conditions

- At a specific time of day.
- On a recurrence schedule.
- Before or after a due date.
- During working hours.

### Inventory and stock conditions

- Stock below threshold.
- Stock above threshold.
- Specific room/container inventory below threshold.
- Required item/prototype missing.
- Perishable stock approaching expiry, if the existing item system supports it.

### Account, tax, and payment conditions

- Business cash above or below threshold.
- Bank account balance above or below threshold.
- Store account balance due or overdue.
- Tax due or approaching due date.
- Wage liability outstanding.

### Market and price conditions

- Market price above/below target.
- Supplier available for a commodity/prototype.
- Store profitability below target.
- Price margin outside a configured band.

### Resource and logistics conditions

- Vehicle/animal/container available.
- Required room accessible.
- Required route/path available.
- Required craft station or tool available.

### Workload and staffing conditions

- No active task already exists for the same purpose.
- Employee with required capability available.
- Host has fewer employees/managers than a target staffing level.
- Open job positions remain unfilled.

### Manual and event conditions

- Manager/proprietor ordered this work once.
- Board post or business event requested this work.
- External system event occurred.

Conditions should be serialisable, inspectable, and usable by both scheduled rules and manager goals where appropriate.

## 16. Action step types

Action step types are the composable operations that make up action plans. Each action step should define:

- Required actor capabilities.
- Required authority.
- Required resources.
- Preconditions.
- Execution behaviour.
- Success output.
- Failure/blocker reasons.
- Existing financial-record effects and operational register effects.

Initial action step families should include the following.

### Planning and lookup actions

- Find supplier for commodity/item/prototype.
- Select source stock location.
- Select destination room/container.
- Select suitable container.
- Select suitable vehicle/animal/consist.
- Calculate route/path.
- Estimate price or total task cost.

### Authorisation and reservation actions

- Authorise employer payment source.
- Reserve employer funds.
- Reserve store credit.
- Reserve stock.
- Reserve container, vehicle, animal, or room.
- Release reservation.

### Movement and logistics actions

- Move to location.
- Pick up item/commodity.
- Put item/commodity in room/container/stock location.
- Load container or vehicle.
- Unload container or vehicle.
- Deliver quantity to destination.
- Return vehicle/animal/container.
- Transfer money or item to a specific character, room, container, or account endpoint.

### Purchasing and account actions

- Purchase commodity/item/prototype.
- Pay store account.
- Deposit money in bank.
- Withdraw money from bank.
- Transfer between authorised accounts.
- Take employee payment item or float.
- Return unused float.

### Production actions

- Trigger craft.
- Reserve craft inputs.
- Use craft station/tool.
- Move craft outputs to destination.

### Communication and command actions

- Write post to host board.
- Execute allowlisted command at location.
- Report task completion or failure.

### Administrative actions

- Create employment opening.
- Close employment opening.
- Create scheduled task rule.
- Create active task.
- Adjust shop price within authority.
- Mark task blocked/failed with reason.

Administrative actions are especially important for manager goals. They must always check manager authority and write register entries.

## 17. Action plan examples

These examples illustrate composition. They should not be implemented as indivisible end-to-end task types.

### Low-stock purchase rule

Rule:

```text
When butter stock in the shop stock room is below 5 kg, create work to buy 10 kg of butter and return it to the stock room.
```

Conditions:

- `StockBelowThreshold(stockLocation: ShopStockRoom, commodity: Butter, threshold: 5 kg)`
- `NoActiveTaskWithIdempotencyKey("restock-butter")`

Action plan:

1. Find supplier for butter.
2. Authorise employer payment source or store account.
3. Move to supplier.
4. Purchase 10 kg of butter.
5. Move to shop stock room.
6. Put butter in stock room.
7. Write optional host-board post summarising the restock.

### Daily delivery route

Rule:

```text
At 8am every day, use a donkey and cart to deliver 50 kg of carrots to five locations, then return.
```

Conditions:

- `TimeOfDay(08:00)`
- `StockAtLeast(Carrot, 250 kg)`
- `VehicleAvailable(Cart)`
- `AnimalAvailable(Donkey)`
- `NoActiveTaskWithIdempotencyKey("daily-carrot-route")`

Action plan:

1. Reserve carrot stock.
2. Select donkey and cart.
3. Reserve donkey and cart.
4. Load 250 kg carrots into cart.
5. For each destination: move to destination, deliver 50 kg carrots, record delivery.
6. Return cart and donkey.
7. Release reservations.
8. Write optional host-board route completion post.

### Craft replenishment

Rule:

```text
When widgets fall below threshold, produce 5 widgets.
```

Conditions:

- `StockBelowThreshold(Widget, threshold)`
- `CraftInputsAvailable(WidgetRecipe, quantity: 5)`
- `CraftStationAvailable(WidgetRecipe)`

Action plan:

1. Reserve craft inputs.
2. Move to craft station.
3. Trigger craft for 5 widgets.
4. Move outputs to stock destination.
5. Record material use and output movement.

### Bank cash handling

One-off order:

```text
Take excess till cash, deposit it in the bank, and post confirmation to the host board.
```

Action plan:

1. Authorise cash movement.
2. Take cash from till.
3. Move to bank.
4. Deposit money in account.
5. Return to host.
6. Write board post with deposit summary.

### Command execution at location

One-off order:

```text
Go to this location and execute an allowlisted command.
```

Action plan:

1. Validate command and arguments.
2. Validate location and path.
3. Move to location.
4. Execute command.
5. Record result.

Command execution is powerful and risky. It must be restricted by allowlisted commands or command categories, required authority, argument validation, location/pathing validation, and a register entry containing command, actor, employer, target location, and result.

## 18. Dispatcher eligibility and execution

The dispatcher should only assign a task if an employee can complete the action plan or the next assignable portion of it. Eligibility checks should include:

- The employee has an active contract with the employer.
- The employee is on duty or otherwise available.
- The employee has the required AI capability types.
- The employee has required skills/knowledge where relevant.
- The employee can path to required origin, purchase location, delivery location, bank, workshop, room, or board endpoint.
- The employee can access required rooms, containers, vehicles, animals, accounts, store credit, or host board.
- The employee can carry or transport the required weight/bulk/volume, or can obtain a suitable container/vehicle through earlier action steps.
- The employer has an authorised payment source for any purchase, withdrawal, store-account payment, wage payment, or tax payment.
- The action plan can create required financial records and operational register entries.
- No other active task has reserved the same exclusive resource unless sharing is allowed.

If no employee is eligible, the task should become `Blocked`, with a reason such as no qualified employee, no path, insufficient funds, no store sells commodity, unavailable vehicle, unauthorised account, missing craft station, no board-post permission, or missing manager authority.

The dispatcher should be able to block at the action-step level. For example, an employee may be eligible to load goods but not eligible to withdraw money. The system can either keep the whole active task blocked or split work into sub-assignments if that becomes necessary later.

## 19. Manager goals

Manager goals are standing objectives delegated to manager AIs. They are similar to scheduled task rules because they are evaluated periodically or in response to events, but they are higher-level and may produce multiple operational changes rather than one fixed action plan.

A manager goal should contain:

- Employer.
- Assigned manager role or specific manager.
- Goal type.
- Goal configuration.
- Required authority.
- Priority.
- Evaluation cadence.
- Constraints and limits.
- Current state.
- Last evaluation time/result.
- Register correlation ID.

Recommended interface shape:

```csharp
public interface IManagerGoalBoard
{
    IReadOnlyCollection<IManagerGoal> Goals { get; }
    IManagerGoal CreateGoal(ManagerGoalDefinition definition, ICharacter authorisedBy);
    void CancelGoal(IManagerGoal goal, ICharacter cancelledBy, string reason);
    void EvaluateGoals(DateTimeOffset now);
}

public interface IManagerGoal
{
    long Id { get; }
    IEmploymentHost Employer { get; }
    ManagerGoalType GoalType { get; }
    EmploymentAuthoritySet RequiredAuthority { get; }
    ManagerGoalStatus Status { get; }
    ManagerGoalConfiguration Configuration { get; }
}
```

Initial manager goal types should include:

### Maintain minimum stock levels

The manager monitors configured commodities/items/prototypes and creates scheduled rules or active tasks to purchase, craft, or move stock when levels fall below target.

### Keep shop accounts paid

The manager monitors store-account balances and due dates, then creates payment tasks when balances exceed thresholds or approach due dates.

### Pay business taxes

The manager monitors tax liabilities and due dates, then creates payment tasks or board warnings before tax deadlines.

### Adjust prices to keep shop profitable

The manager reviews purchase costs, market prices, sales velocity, and configured margin targets, then adjusts prices within authority limits.

### Maintain business cash and bank balances

The manager creates deposit, withdrawal, or transfer tasks to keep till cash, floats, and bank balances within configured ranges.

### Maintain staffing levels

The manager creates, modifies, or closes employment openings when the host has too few employees/managers with required capabilities.

### Maintain hotel operations

The manager monitors hotel room occupancy, rental payments, cleaning/maintenance needs, guest notices, lost property, and supply levels, then creates appropriate active tasks or board posts.

Manager goals may be less composable than scheduled task rules. Some goal types can be hardcoded objective types because they involve policy decisions rather than a simple condition/action sequence. Even so, goal outputs should use the same shared services: create active tasks, create scheduled rules, create employment openings, adjust prices, pay accounts, or post to the host board through permissioned actions.

Manager goal evaluation must never bypass authority checks. A manager AI may only create work that the manager could have created through commands.

## 20. Financial records and operational registers

Separate existing financial records from new operational register entries. The employment/task system should not introduce a parallel general-purpose financial ledger.

### Existing financial records

Financial actions should write through the systems that already own the relevant money movement. Examples include:

- Bank account transactions for deposits, withdrawals, transfers, and wage payments into accounts.
- Shop transaction records for shop purchases, sale proceeds, tills, store-account payments, and stock-related money movement.
- Stable ledger entries for boarding, rental, animal-care, transport, and related stable operations.
- Arena finance records for event payouts, fees, takings, maintenance, and arena-specific employment payments.
- `VirtualCashLedger` for virtual reserves, cash abstractions, and host-level value movement where a concrete account or transaction record is not appropriate.
- Existing tax/accounting records for tax liabilities, tax payments, and reportable business activity.

Where those systems allow metadata or notes, employment/task actions should correlate the financial record with:

- Employer/business ID.
- Actor/employee ID where applicable.
- Task ID, action-step ID, manager-goal ID, or employment-contract ID where applicable.
- Counterparty.
- Amount/currency/commodity valuation.
- Tax category.
- Timestamp.
- Narrative/description.

If an existing financial system cannot hold all correlation data directly, the operational register should record the missing correlation while referencing the canonical financial record.

### Register entries

Register entries are new shared operational records used for employment and task traceability:

- Employment opening created/modified/closed.
- Application submitted/accepted/rejected.
- Employee hired/fired/suspended/reactivated.
- Manager authority granted/changed/revoked.
- Host-board post created/edited/deleted where operationally meaningful.
- Manager goal created/modified/evaluated/cancelled.
- Scheduled task rule created/modified/deleted.
- Active task created/assigned/started/blocked/completed/failed/cancelled.
- Action step started/completed/failed where useful.
- Payment authorisation granted/used/revoked.
- Command-execution actions executed.
- Price adjustments performed by managers.

Every financial task must create canonical financial evidence in the owning subsystem and operational register correlation where appropriate. Every employment, authority, task, board, or manager-goal transition that matters operationally must create register evidence.

## 21. Business-specific adoption notes

### Shops

Shops are the highest priority because deliveries and stock purchases are the motivating use case. Shops should expose employees, managers, proprietors, store accounts, tills/cash handling, stock thresholds, delivery routes, price adjustments, and host-board notices through the common model. Existing shop commands should become wrappers around employment services where practical.

### Auction houses

Auction houses should expose managers and employees through the same model. Potential future action plans include item intake, listing support, payment settlement, commission handling, and delivery of won lots. For the first slice, focus on employment contracts, manager authority, host board reference, operational register compatibility, and correlation with existing auction-house financial records.

### Arenas

Arenas should use the model for managers and employees who operate events, payouts, scheduling, maintenance, or contestant handling. First slice should adopt staff ownership/management semantics; event-specific task automation can be deferred.

### Banks

Banks are both employers and payment infrastructure. Be careful to avoid circular assumptions: employee wage deposits may use banks, but bank employment must not depend on a working employment-seeking flow to initialise. Bank employees may include tellers, managers, and couriers. Account transaction records are the canonical financial evidence for bank money movement and should correlate with operational register entries where possible.

### Stables

Stables should use common employees/managers for stable hands, animal care, rental/boarding management, and transport-related work. The action-step design should be compatible with stable-managed animals, carts, and consists.

### Hotels

Hotels should be separate persisted economy entities implementing `IHotel` and `IEmploymentHost`. Properties remain the ownership, location, and access-control anchor for rooms and areas, but hotel business state belongs to the hotel subsystem rather than `Property.HotelDefinition`.

Hotel business state includes hotel rooms, rental offerings, active rentals, patron balances, lost property, taxes, manager workflows, host board, bank account, virtual reserve, task board, manager-goal board, and operational register. Hotels may employ clerks, cleaners, maintenance workers, security, managers, and couriers. Potential manager goals and action plans include maintaining room readiness, collecting unpaid rent, posting guest or staff notices, buying cleaning supplies, moving linen or fuel, depositing takings, reconciling patron balances, managing lost property, and paying taxes.

Current property-hosted hotel XML should remain loadable until the hotel split is implemented. Migrating `Property.HotelDefinition` into separate persisted `Hotel` records is an explicit later implementation concern unless it is deliberately included in the first hotel persistence milestone.

## 22. Adoption strategy

1. **Discovery pass.** Locate all existing proprietor, manager, employee, staff, wage, payroll, store-account, bank-account, property hotel, delivery, and task-like code paths.
2. **Confirm PC-facing job boundary.** Identify references to `IJobListing`, `IActiveJob`, job-finding cells, payroll/coffers, and `job` commands only to avoid accidental coupling; do not refactor them as part of this project.
3. **Introduce neutral domain types.** Add `IEmploymentHost`, contracts, authority, employment openings, host board references, manager goals, condition types, action steps, action plans, operational registers, and task boards without changing existing behaviour more than necessary.
4. **Create hotel entities.** Add separate persisted `IHotel`/`Hotel` economy hosts and define their property linkage before moving hotel business state out of property-hosted XML.
5. **Adopt host shells.** Make shops, auction houses, arenas, banks, stables, and hotels expose `IEmploymentHost` with contracts, authority, employment openings, host board reference, task board, manager-goal board, and operational register support before implementing full dispatcher execution.
6. **Reset obsolete employment state.** Initialise the new employment model from clean defaults. Ignore or remove legacy employment data rather than migrating it into contracts.
7. **Centralise commands/services gradually.** Route hire/fire/list/manage commands through common services while preserving user-facing syntax where practical.
8. **Add employment openings.** Implement postings, applications, candidate matching, and manager/proprietor approval.
9. **Add NPC employment-seeking AI.** Start with simple periodic search/apply/accept behaviour and a configurable reservation wage.
10. **Add host board access.** Expose the host `IBoard` reference to employees and add the board-post action step.
11. **Add task board and dispatcher.** Implement condition evaluation, action-plan instantiation, active task lifecycle, action-step execution, eligibility checks, and minimal task execution.
12. **Add manager goals.** Add the goal model and initial evaluators that create active tasks or scheduled rules through the same permissioned services.
13. **Wire financial records and registers.** Ensure every financial action uses the owning subsystem's records and every operational action creates the correct register record.
14. **Retire duplicate logic.** Remove or deprecate old ad hoc staff handling after the new model compiles, tests pass, and player-facing command paths are covered.

## 23. Data and save compatibility

Save compatibility should mean the world can still load and use the new baseline. It does not mean legacy employment state must survive.

- Obsolete employment records may be discarded.
- Old proprietor/manager/employee fields may be ignored or reset unless trivially useful.
- New employment contracts, employment openings, manager goals, task boards, host board references, and operational registers may start empty.
- Existing non-employment economic data should remain authoritative where possible.
- Existing financial records should remain authoritative; new entries should append rather than recreate history.
- Current `Property.HotelDefinition` data should remain loadable until the hotel split migration is implemented. Moving that XML into separate `Hotel` persistence is an explicit migration task, not an implicit employment-data import.
- Any intentional legacy employment data loss should be reflected in code comments, release notes, or schema notes so future maintainers understand it was deliberate.

## 24. Invariants

- Existing `IJobListing`, `IActiveJob`, job-finding cell, payroll/coffer, and `job` command systems are not dependencies of the new employment-host model.
- Every `IEmploymentHost` has a reference to a persisted host `IBoard`, a task board, a manager-goal board, and an operational register.
- Every active employee has exactly one active contract for a given employer/role combination unless the design explicitly allows multiple positions.
- Employment status remains simple: active, suspended, or ended.
- No manager action succeeds without authority.
- No manager goal creates work that the assigned manager lacks authority to create.
- No scheduled rule spawns work unless its conditions are satisfied.
- No active task spends employer money without a payment authorisation.
- No financial task completes without canonical financial evidence in the owning subsystem and operational register correlation where appropriate.
- No employment/task/manager-goal/board state transition that matters operationally occurs without a register entry.
- Scheduled task rules must be idempotent and must not spawn unlimited duplicate active tasks.
- Action plans are composed from action steps; end-to-end workflows should not become isolated one-off task types unless there is a strong host-specific reason.
- An NPC cannot accept an employment offer below its reservation wage unless explicitly configured to allow it.
- Firing/termination preserves new-system historical records.
- Legacy employment data preservation is not required.

## 25. Testing and validation

Minimum tests for the first implementation slice:

- `IEmploymentHost` implementations or adapters exist for shops, auction houses, arenas, banks, stables, and hotels.
- `IHotel`/`Hotel` exists as a separate persisted economy host linked to properties for ownership, location, and access control.
- The new model does not require existing `IJobListing`, `IActiveJob`, job-finding cell, payroll/coffer, or `job` command systems.
- Old worlds or fixtures with obsolete employment data load without requiring employment-data migration.
- Hire/fire operations create, activate, suspend, and end contracts correctly.
- Employment status does not include probationary state.
- Manager authority checks permit and deny the right actions.
- Employment openings validate requirements, pay, schedule, role, duration, and application limits.
- NPC candidate matching filters by skills, knowledges, AI capabilities, availability, pathing if available, and reservation wage.
- Payment method selection handles bank account, specified account, cash fallback, and payment failure/liability.
- Every `IEmploymentHost` exposes a persisted `IBoard` reference that employees can view/post to according to permissions without a physical board item reference.
- A write-board-post action step creates a board post and operational register entry.
- Scheduled task rule evaluation composes condition types and spawns an active task exactly once per trigger/cooldown window.
- Action plans can contain multiple action steps and preserve step state.
- Dispatcher eligibility blocks impossible tasks with useful reasons.
- Purchase, deposit, withdraw, store-account-payment, craft-trigger, command-execution, and board-post action steps create required financial records and operational register records.
- Manager goals can create active tasks or scheduled rules only when the manager has authority.
- Existing shop/auction/arena/bank/stable/hotel management commands still compile and either work directly or clearly route to the new service.

Recommended integration scenario:

1. Create a shop with a manager and one NPC employee.
2. Post an employment opening requiring commodity purchase capability.
3. Have a qualifying NPC apply and be hired.
4. Give the shop a host board.
5. Set a scheduled rule: if butter stock is below 5 kg, run an action plan that buys 10 kg and places it in the stock room.
6. Trigger the stock condition.
7. Verify an active task is created, an eligible employee is assigned, the purchase is authorised, the item is moved, a board post can be written, and required financial records plus operational register entries exist.
8. Add a manager goal to maintain stock levels and verify the manager AI creates equivalent operational work through authorised services rather than bypassing permissions.

## 26. Implementation status section for Codex to maintain

Codex should update this section during implementation.

### Completed

- Pending.

### Deferred

- Pending.

### Blockers / decisions needed

- Pending.

## 27. Suggested Codex goal

```text
/goal Implement the first production slice of the Unified Employment, Manager Goals, and Task Dispatch system described in Design Documents/Economy/Unified Employment Dispatch Design.md. Read that doc first and treat its Scope, invariants, adoption strategy, and acceptance criteria as the source of truth.

Outcome: shops, auction houses, arenas, banks, stables, and hotels expose shared `IEmploymentHost` host shells and use common services for contracts, hiring/firing, role/manager authority, employment openings, payment methods, persisted host `IBoard` references, operational register recording, task boards, and manager goals. Add separate persisted `IHotel`/`Hotel` economy entities linked to properties for ownership, location, and access control. Keep existing `IJobListing`, `IActiveJob`, job-finding cell, payroll/coffer, and `job` command systems out of scope.

Build the task model as composable condition types plus action-step plans, not monolithic end-to-end task types. Add enough stock/time/manual/account conditions and purchase, movement/delivery, craft-trigger, command, bank deposit/withdrawal, store-account payment, and board-post action steps to compile and test the core dispatcher path.

Constraints: legacy employment data does not need migration and may be discarded/ignored, but old worlds should still load where practical; preserve unrelated economic data and accounting semantics; current property-hosted hotel XML should remain loadable until the hotel split migration is implemented; managers may only exercise delegated authority; employees must not spend employer money, use store credit, alter prices, post official notices, or create tasks without auditable authorisation, canonical financial records, and operational register entries where applicable.

Process: work in small milestones. Start with host shells before real dispatcher execution. After each milestone, run relevant build/tests and fix failures before widening scope. Add/adjust tests for host adoption, independence from existing PC-facing job systems, simple employment status, manager permissions, employment openings/candidate matching, NPC reservation wage, payment method selection, host-board access, board-post action step, condition-to-active-task spawning, action-plan step state, dispatcher eligibility, manager goals, existing financial record reuse, and operational register entries.

Done when: all minimum host types expose `IEmploymentHost`, existing tests pass, new tests cover the core behaviours above, and the design doc implementation status section lists completed/deferred items. If codebase structure makes any requirement infeasible, stop only after documenting the blocker, evidence, attempted approaches, and the narrowest follow-up goal needed.
```
