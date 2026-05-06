# Clan Command Surface and Permissions

## Scope

This document describes the intended runtime command architecture for clans after the clan command refactor, with an emphasis on:

- moving player-facing clan operations onto `IClan` / `MudSharp.Community.Clan`;
- keeping `ClanModule` as an intent router instead of a mega-command implementation;
- making administrator short-circuit behaviour explicit; and
- documenting how clan privileges, appointment authority, and vassal control interact.

This document covers runtime command handling, not the full clan builder/edit surface.

## Architectural Split

The clan runtime now follows a thinner command-module pattern:

- `MudSharpCore/Commands/Modules/ClanModule.cs`
  - parses the top-level player intent;
  - resolves the target clan from player-visible scope; and
  - forwards the remaining argument stream to the clan entity.
- `FutureMUDLibrary/Community/IClan.cs`
  - exposes the clan-owned runtime command surface.
- `MudSharpCore/Community/Clan.CommandHandling.cs`
  - contains the concrete clan-owned implementations.
- `MudSharpCore/Community/Clan.Finance.cs`
  - contains clan-owned finance commands for appointment budgets, balance sheets, and payroll history.
- `MudSharpCore/Community/ClanCommandUtilities.cs`
  - contains shared helper logic for appointment-chain authority, election helper selection, term-limit checks, and appointment-capacity calculations.

The goal is that runtime operations which are fundamentally “things a clan does” live with the clan, rather than in a module that must manually re-implement clan rules for every subcommand.

## Clan-Owned Runtime Commands

The following runtime actions are now owned by `IClan` / `Clan`:

- `Show`
- `ShowMembers`
- `DescribeElections`
- `ShowElectionHistory`
- `Nominate`
- `WithdrawNomination`
- `Vote`
- `Appoint`
- `Dismiss`
- `SubmitControl`
- `ReleaseControl`
- `TransferControl`
- `SetControllingAppointment`
- `AppointExternal`
- `DismissExternal`
- `BudgetCommand`
- `ShowBalanceSheet`
- `ShowPayrollHistory`

This is intentionally focused on operational clan behaviour. Builder-oriented commands that mutate multiple related entities or templates still live in `ClanModule` for now.

## Permission Model

### Administrator short-circuit

Administrators at `PermissionLevel.Admin` are intended to bypass almost all runtime clan permission restrictions.

That short-circuit now consistently applies to:

- clan structure visibility;
- member visibility;
- election viewing;
- direct appointment and dismissal;
- vassal control management; and
- external appointment management.

Admin access is treated as authoritative override rather than just “another privilege flag”.

### Membership privilege checks

Normal clan authority is resolved from `IClanMembership.NetPrivileges`.

Net privileges combine:

- rank privileges; and
- appointment privileges held by that membership.

That means a character can gain runtime clan authority through rank, through appointment, or both.

### Visibility privileges

The command surface distinguishes between:

- `CanViewMembers`
- `CanViewClanOfficeHolders`
- `CanViewClanStructure`
- `CanViewClanStructureEqualRankOrLower`
- `CanViewTreasury`
- `CanCreateBudgets`

The view logic now consistently uses those distinctions so that:

- member rosters do not automatically imply full structure visibility;
- office-holder visibility does not automatically imply financial visibility; and
- lower-trust members can be limited to equal-or-lower-rank visibility when configured.

Budget creation and closure use `CanCreateBudgets`. Reviewing budgets, budget audit rows, balance sheets, and payroll history uses `CanViewTreasury`, which is the clan-level financial-information privilege. Budget drawdown also allows the holder of the budgeted appointment, or a superior appointment in that appointment chain, to draw from that appointment's active budget without granting broad treasury visibility.

### Appointment-chain authority

Some clan actions are intentionally not pure privilege-flag checks.

For appointment hierarchies, authority can also flow through the parent appointment chain. This is used for cases where a role may manage subordinate offices even if the actor does not hold a broad clan-management privilege.

The shared helper is:

- `ClanCommandUtilities.HoldsOrControlsAppointment`

That rule is used in places such as:

- direct appointment to subordinate positions;
- direct dismissal from subordinate positions; and
- appointment creation/edit flows that are restricted to “under own” authority.

Appointment budget drawdowns also use the appointment-chain helper, so a character who holds or controls the budgeted office can draw from that budget without needing broad budget-creation authority.

## Runtime Finance Commands

### Appointment budgets

`clan budget <clan> list`

`clan budget <clan> view <budget>`

`clan budget <clan> audit [<budget>]`

`clan budget <clan> create <appointment> <amount> "<interval>" <name>`

`clan budget <clan> draw <budget> <amount> <reason>`

`clan budget <clan> close <budget>`

Appointment budgets are recurring-period allowances backed by the clan's default bank account. Each budget stores its recurring interval, current period window, period drawdown, and drawdown audit rows. Period windows roll forward lazily when the budget is reviewed or used; a new period resets the tracked drawdown while preserving historical transactions.

Creating or closing a budget requires `CanCreateBudgets`. Drawing from a budget requires administrator authority, `CanCreateBudgets`, or appointment-chain authority over the assigned appointment. A successful drawdown withdraws from the clan bank account, issues a currency pile to the actor, and records the actor, amount, period window, bank balance after withdrawal, and audit reason.

### Balance sheet

`clan balance <clan>`

`clan balance sheet <clan>`

The balance sheet is gated by `CanViewTreasury`. It summarises clan-owned bank accounts, property value and lease revenue, leased-property commitments, shop current-period gross and net results, shop cash and bank balances, controlled economic-zone revenues, payroll commitments, budget commitments, budget remaining, and backpay liability.

### Payroll history

`clan payroll <clan>`

`clan payroll <clan> member <who>`

`clan payroll <clan> rank <rank>`

`clan payroll <clan> appointment <appointment>`

Payroll history is gated by `CanViewTreasury`. It records payroll accruals from normal payday processing, manual backpay adjustments, and pay collection events. Review commands can filter by individual member, rank, or appointment, and show both summary totals and recent audit rows.

## External Control Permissions

External clan control has two distinct authority models:

- clan-level control via `CanManageClanVassals`; or
- appointment-level control via a configured controlling appointment in the liege clan.

If a control relationship has a controlling appointment configured, characters who hold that liege appointment are treated as authorised to manage the controlled vassal appointment, even without the broad vassal-management privilege.

This is the core support for relationships such as:

- a mayor appointing a chief of police in another clan;
- a king appointing a duke or governor in a subordinate domain clan; or
- a parent religious or military office appointing a subordinate office across organisational boundaries.

## Runtime Commands for External Control

### Submit

`clan submit <clan> <position> <new liege> [<liege appointment>] [<max appointees>]`

Creates an external-control relationship from the vassal clan to the liege clan.

If a liege appointment is supplied, the control becomes appointment-scoped rather than only clan-scoped.

### Vassal control

`clan vassal control <clan> <position> <liege appointment|none> [<liege clan>]`

Sets or clears the controlling appointment on an existing external-control relationship.

This command is important because the runtime data model already supported controlling appointments, but the command surface previously did not expose a robust way to author or change them.

### Vassal appoint / dismiss

`clan vassal appoint <who> <clan> <position> [<liege clan>]`

`clan vassal dismiss <who> <clan> <position> [<liege clan>]`

These commands operate through the external-control relationship and obey either:

- broad vassal-management authority; or
- the configured controlling appointment.

## Known Boundaries

The refactor does not yet move the full clan builder/edit surface onto `IClan`. In particular, commands that are primarily administrative authoring tools still remain in `ClanModule`.

That is intentional for now. The priority of this pass was:

- operational runtime correctness;
- permission consistency;
- election and external-control reliability; and
- reducing the amount of duplicated runtime rule logic in `ClanModule`.
