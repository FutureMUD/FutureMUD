---
slug: employment-hosts
category: Economy
summary: Staff a business, delegate authority, pay workers, and dispatch and recover NPC tasks, scheduled rules and manager goals.
---

# Employment hosts

An **employment host** is a business or organisation that can employ characters and organise work. The host owns its contracts, applications, payroll, staff board, task board and operational register. The underlying shop, bank, hospital or other business still owns the rules for its own stock, services and financial transactions.

Use the shared commands in this guide to build the employment layer. Host-specific guides explain which work makes sense at each business. The older `job` listings and the unified employment host commands have separate contracts and payroll.

## Find and inspect the host

Use `employment <host-type> <host-id-or-name> <command>` from wherever you have access. Quote multiword names and prefer IDs when names overlap. At a permanent shop, `shop contracts` and `shop tasks` are examples of local shortcuts. The explicit `employment` prefix is useful when your test involves several businesses.

| Host type | Which record to identify |
| --- | --- |
| `shop` | The shop record. For its stockroom, craft and supplier walkthrough, see [Shops, currencies and banking](Shop_and_Banking_Builder_Guide.md#run-a-shop-with-employment-hosts). |
| `bank`, `auction`, `arena`, `stable`, `hospital`, `clan` | The named business or organisation. Consult its own editor for locations, service policies and finances. |
| `hotel` | The property that operates the hotel. Use its property ID or name. |

Start with `status` to inspect the host and `contracts` to see who works there. Use `register` to review operational decisions and `employmentledger` to inspect recorded financial activity. Exact money and stock records also live in the native ledger of the system that moved them.

```text
employment shop "Workshop Market" status
employment shop "Workshop Market" contracts
employment shop "Workshop Market" payroll
employment shop "Workshop Market" register
```

## Set up staff and their authority

A **contract** is the active or historical relationship between an employee and the host. It records role, authority, compensation, schedule and term. Expiry and termination end active access while preserving the history and outstanding pay. A **role** supplies defaults and may require specific AI capabilities. The contract's actual grants determine what work the character can authorise or execute.

### Hire directly or through an opening

Use the host's direct hire commands where supplied. For example, `shop employ <nearby-character>` lets a manager offer a shop contract. The target accepts it. The simple offer is unpaid, with an indefinite term and any-time schedule. Use the host's contracts view to confirm the accepted role.

An **opening** advertises a role, hourly pay, vacancies and qualifications to NPC job seekers. The search-capable NPC applies if the role, pay, qualifications, host and route fit. The host reviews the application before employment starts. To set up a first paid opening, choose a wage in the host currency that meets your worker's reservation wage.

```text
employment shop "Workshop Market" openings create crafter <hourly-wage> 1
employment shop "Workshop Market" openings
employment shop "Workshop Market" applications
employment shop "Workshop Market" applications accept <application-id>
employment shop "Workshop Market" contracts
```

Check the opening's description before waiting for applications. Review requirements and available places. `applications reject <id> [reason]` declines a particular request. A changed opening can require a fresh application. Use a direct host offer for the player-facing employment example above.

### Give an NPC the EmploymentWorker AI

Author a dedicated AI in a test world. `EmploymentWorker` handles job searching and task execution. The character still needs ordinary movement, usable hands, training, tools and permission to do the work itself.

```text
ai typehelp employmentworker
ai edit new employmentworker WorkshopWorker
ai set host shop
ai set currency <host-currency>
ai set wage <minimum-hourly-wage>
ai show
ai close
ai add WorkshopWorker <nearby-npc>
ai npclist WorkshopWorker
```

Use `ai show` to check the accepted payment methods, capabilities, search, tasking, path range and cadence. The `ai set search`, `ai set tasking`, `ai set payment <method>` and `ai set capability <capability>` commands toggle their values. Inspect before changing them. The shared hiring pass checks NPC candidates on the five-minute heartbeat. The individual search cadence can defer a worker to a later pass. If search is disabled, an assigned employee can still execute tasks with tasking enabled.

Add only the capabilities the job needs. For example, `CanCraft` is needed for a crafter and `CanDeliverItems` for retrieval and delivery. Configure them on top of the NPC's physical ability and skills. To supply this AI to future copies, attach it to a template and approve the revision. [NPC templates, spawners and AI](NPC_Builder_Guide.md)

### Assign a permanent NPC for a controlled test

As an Admin, assign a compatible live primary NPC directly. Use `#<npc-id>` for an exact character ID. If you omit the hourly pay, the assignment is unpaid.

```text
employment shop "Workshop Market" contracts assign <npc-or-#id> employee
employment shop "Workshop Market" contracts
```

The NPC must have `EmploymentWorker` with a host filter that admits the shop and any capabilities required by the role. Select an appropriate role such as `crafter` after checking its required capabilities. Record the contract ID. An administrator can end the permanent assignment with `contracts fire <contract-id>`.

### Delegate narrowly

Use `contracts delegate <contract-id> show` to inspect actual grants. `grant` and `revoke` add or remove named authority, and `set` replaces the set. The administrator or authorised manager making the change must hold the authority being delegated. For example, `contracts delegate <contract-id> grant ManageDeliveryRoutes` grants delivery authority to that worker.

| Authority | Example of what it controls |
| --- | --- |
| `AssignTasks` | Dispatch and manage active work. |
| `CreateScheduledRules` and `CreateManagerGoals` | Create sources of future work. The plan must also fit the issuer's authority. |
| `ManageDeliveryRoutes` and `ManageCraftRules` | Execute appropriate movement, delivery, station and craft work. |
| `ManageStockRules` and `AdjustPrices` | Inspect or manage stock and native merchandise prices and deals through their corresponding actions. |
| `ApprovePurchases`, `UseStoreAccount`, `DepositBusinessCash`, `WithdrawBusinessCash` and `PayTaxes` | Separate spending and finance grants. A manager's ordinary staff and task access need not include them. |

Inspect both the issuer and worker when a task is refused. For an NPC, a matching AI capability is also required. A capability lets the NPC attempt that kind of action. The contract and task approval still control its permission to act.

## Communicate and pay staff

The staff board is for messages. Use `board` to list them, `board read <number>` to read a post and `board write <title>` to open the message editor. Posting requires `PostToHostBoard`. Submit with `@` on its own line. To make an NPC do work, create a task on the task board.

Payroll creates wage liabilities from eligible paid contracts, settles them through supported host payment sources, and lets employees claim cash held for them. A scheduled contract affects eligibility and wage accrual. Shop employees do not clock in through a separate register. Check the contract and host payroll views if availability or pay is unexpected.

```text
employment shop "Workshop Market" payroll
employment shop "Workshop Market" payroll run
employment shop "Workshop Market" payroll settle <payable> test wage
employment shop "Workshop Market" payroll
```

Accrue and settle as staff with the appropriate authority. Allow some eligible paid time to pass before running accrual. Start with one payable before using `settle all`. A supported cash settlement can make wages claimable with `payroll claim <payable>` from the employee's character. Bank settlement credits the configured account. Read the settlement result and employment ledger. An ended contract can still have wages owing. If a payment is refused, check the selected source, authorisation, recipient and balance before settling again.

## Dispatch a task

An **active task** is one job with an ordered sequence of steps. A stocking task can fetch an item, deliver it and write the outcome to the operational register. Work can advance over several ticks. A craft or a logistical action can wait for its own runtime operation. An NPC acts when it can claim or retain the work and reach the next step.

Use `tasks actions` and `tasks actions <action>` to inspect the current syntax, required grants, worker capabilities and whether an action executes real work or records evidence. Do the same with `tasks conditions` for trigger rules. Bare prototype IDs find goods of a type, `*<item-id>` selects a specific instance and `&<tag>` selects items by tag. Prefer exact IDs for fixtures that must move a known object.

The shop example below shows task authoring one step at a time. Use a spare ordinary item already placed in an accessible room and two connected test locations. Angle-bracket values are yours to supply.

```text
employment shop "Workshop Market" tasks draft new "Move test supplies"
employment shop "Workshop Market" tasks step getid 1 <prototype-id> from <source-room-id>
employment shop "Workshop Market" tasks step deliver to <destination-room-id>
employment shop "Workshop Market" tasks draft show
employment shop "Workshop Market" tasks draft finalise
employment shop "Workshop Market" tasks
employment shop "Workshop Market" tasks diagnose
```

Use an active worker who can retrieve and deliver items, has access to the source, and can reach the destination. Finalising creates live work. Inspect it with `tasks show <id-or-name>` and verify it completes and the item reaches the destination. Keep the item in its expected location until the task claims it. Ordinary delivery moves the item. A shop's stock profile changes only when the workflow also uses its native stocking action.

Use `tasks cancel <id-or-name> [reason]` to stop active work. Check the completed steps and any item or financial records before cancelling or creating replacement work. A cancelled delivery may already have collected the goods.

## Automate recurring work

| Tool | Use it when |
| --- | --- |
| **Active task** | Someone needs to do one known job now. Finalise it, inspect it and check the result. |
| **Scheduled rule** | A repeatable condition should produce a particular action plan. Give the rule a stable key and a cooldown. |
| **Manager goal** | You want to maintain a target such as materials, staffing, cash float, or crafted merchandise. Give the goal its policy, conditions and plan unless its type provides a generated plan. |

Scheduled rules can combine conditions with `and`, `or`, `not` and parentheses. Use `tasks rule diagnose <rule>` to inspect conditions without spawning work. `tasks rule evaluate <rule>` can create work and follows the rule's normal checks. A rule exposes separate pause, resume and cancel commands. Before changing or replacing a rule, inspect its active tasks. Changing future triggers leaves existing work to be reviewed on its own.

### Test a scheduled rule

Use a manual trigger for the first run so you decide when the condition can become true. The example creates a reporting task. Choose an unused key for the test.

```text
employment shop "Workshop Market" tasks rule draft new "Opening report"
employment shop "Workshop Market" tasks rule draft key workshop-opening-report
employment shop "Workshop Market" tasks rule draft cooldown 1h
employment shop "Workshop Market" tasks rule condition manual workshop-open-check
employment shop "Workshop Market" tasks rule step report confirm the opening check ran
employment shop "Workshop Market" tasks rule draft show
employment shop "Workshop Market" tasks rule draft finalise
employment shop "Workshop Market" tasks rule diagnose "Opening report" manual workshop-open-check
employment shop "Workshop Market" tasks rule evaluate "Opening report" manual workshop-open-check
```

Look for the generated task with `tasks`, then confirm its register entry after the worker finishes. The report demonstrates dispatch. An inspection action such as a shop stocktake is what actually measures goods. A second immediate evaluation should be held by the rule's active-work or cooldown check. Once the test works, use time or stock conditions for a rule that runs without that manual trigger.

### Use a manager goal for a target

Use `goals types <type>` to inspect authority, conditions and suggested actions for a goal. Draft it, inspect it, and set a deliberate priority, cadence and budget where appropriate before finalising. `goals show <goal>` explains the current target and evaluation state. For the shared stages applied to real shop stock, see [the shop employment walkthrough](Shop_and_Banking_Builder_Guide.md#run-a-shop-with-employment-hosts).

## Approve spending and handle interruptions

A task's spending approval is bound to its host and ordered plan. Approval can include a cumulative currency budget, payment source and permitted counterparties. Individual purchase maximums also apply. It needs a valid issuer who still has the required authority when the financial action executes. A scheduled rule or manager goal supplies the work and its provenance. It does not act as the financial authoriser.

For purchase plans, use explicit bounded amounts, an appropriate payment source and named suppliers when commissioning the first run. Use an authorised issuer and include `reserve <amount> for <description>` before any action that needs reserved funds. After the payment, `release all` frees unused funds held by that task. Verify the native shop, bank or counterparty record against the employment ledger and task's recorded custody.

Use `tasks approval show <task>` to inspect the grant. For interrupted financial work, inspect the native record and the task's own custody evidence before changing anything. If reconciliation is supported for the case, `tasks approval reconcile` records whether the step executed, using the grant ID, revision, step number, evidence and a reason. Reconciliation does not issue payment. Use an authorised `tasks retry <task> <reason>` only after the hold is resolved and any outstanding operation is safe to attempt. Completed steps are retained. Missing durable custody for a purchase or physical-float operation can leave the task held for separate repair.

The [employment recovery guide](../Economy/Employment_Restart_and_Recovery.md#approval-inspection-and-audited-recovery) has exact revoke, replace, reconcile and retry syntax and the limits of the live verification.

## Diagnose a host

| Symptom | Where to look |
| --- | --- |
| A candidate never applies | Check search mode, vacancy, host filter, pay and currency, accepted payment, qualifications and the hiring route. |
| An active worker never claims a task | Use `tasks diagnose` and `tasks show`. Check work schedule, role grants, AI capabilities, tasking, route, current assignments and the next step's preconditions. |
| A rule makes no work | Diagnose the actual rule. Check its condition values, cadence or cooldown, active tasks with its key, and the source issuer's authority. |
| A job is blocked after pickup or crafting | Follow the actual item custody and next destination. Restore access or inspect salvage before retrying. An exact item already reserved by one task cannot be promised to another. |
| A financial action is blocked | Inspect the grant, limits, source funds, payment route and any unresolved recovery hold. Check native payment evidence before any retry. |
| A worker finished, but the business did not change | Compare the task result with the native shop, bank or other host records. An inspection or reporting action can succeed by producing evidence alone. |

For the underlying contracts and the current scope of each host, see the [employment system reference](../Economy/Employment_System.md#21-business-specific-adoption-notes).
