# Clan Elections and External Control

## Scope

This document describes the current design of clan elections and external appointment control, including the key invariants that runtime code now enforces.

## Election Model

Election-backed appointments are represented by:

- `IAppointment` / `Appointment`
- `IElection` / `Election`

Each election-backed appointment can have:

- one primary open election for the regular term cycle; and
- zero or more by-elections for unfilled positions.

Shared election-selection helpers live in:

- `MudSharpCore/Community/ClanCommandUtilities.cs`

## Election Lifecycle

The election lifecycle is:

1. `Preelection`
2. `Nomination`
3. `Voting`
4. `Preinstallation`
5. `Finalised`

The stage transitions are time-driven from the appointment configuration:

- nomination period;
- voting period;
- election lead time; and
- election term.

The runtime now uses the correct period when announcing election start:

- the nomination-start message reports `NominationPeriod`;
- it no longer incorrectly reports `VotingPeriod`.

## Nomination Eligibility

Nomination eligibility is evaluated by `Appointment.CanNominate`.

A nomination is allowed only if all of the following pass:

- the nomination FutureProg, if any;
- the maximum consecutive term rule, if configured; and
- the maximum total term rule, if configured.

Members who already hold an appointment can stand again in the regular election cycle, but they cannot nominate in a by-election for an unfilled seat in that same appointment. A by-election cannot grant a duplicate copy of the same appointment, so allowing current holders to win would leave the vacancy unresolved.

The term-limit checks now use shared helper logic and correctly treat the total-term limit as inclusive:

- reaching the configured maximum total terms blocks further nomination;
- it is no longer possible to nominate for one extra term because of a strict `>` check.

By-elections are ignored for the consecutive and total regular-term limit helpers.

## Election Visibility and Voting

Election display and command access use clan office-holder visibility rules.

The default `clan elections` display includes election ids for scheduled, nomination, voting, and preinstallation primary elections, and for all open by-elections. Players need these ids for `clan nominate`, `clan withdrawnomination`, `clan vote`, and `clan election view` when an appointment name is ambiguous or when multiple elections exist for the same appointment.

The refactor also fixes several runtime election bugs:

- election history permission checks now use the intended logic instead of blocking valid non-admin viewers;
- `clan election history <election id>` resolves the election's appointment history, matching the documented player syntax;
- nomination, withdrawal, and voting command resolution correctly handles either election ids or clan-plus-appointment targeting;
- actor/member lookups during election finalisation use `MemberId` rather than the membership object id;
- secret-ballot display resolves dub data from the elected member id rather than the membership id.

## Election Outcome Behaviour

Election victors are determined from recorded votes at or after `Preinstallation`.

Important behaviours:

- nominees with no votes are not elected;
- if fewer victors exist than positions, the runtime schedules a by-election;
- if no nominees exist at voting start, the runtime finalises the empty election and creates a by-election;
- by-elections remain separate from the regular term cycle.

By-elections fill uncovered appointment vacancies rather than replacing the whole office. During by-election finalisation, existing holders of the same appointment keep their positions unless they are separately removed by another workflow. Victors are only installed while free appointment slots remain, so a stale or manually superseded by-election cannot overfill the appointment.

The appointment vacancy check also accounts for already-open by-elections. If an existing by-election already covers the vacant slots, repeated dismissal or membership-cleanup paths do not create duplicate by-elections for the same vacancy. If a primary election is already close enough to seat winners no later than a newly-created by-election would, the vacancy check leaves the primary election to resolve the office instead of scheduling a redundant by-election.

## External Control Model

External control is represented by `IExternalClanControl` / `ExternalClanControl`.

Each relationship has:

- a `VassalClan`
- a `LiegeClan`
- a `ControlledAppointment` in the vassal clan
- an optional `ControllingAppointment` in the liege clan
- a maximum number of appointment slots granted to the liege clan
- the list of current appointees occupying those granted slots

This model supports both:

- clan-wide liege authority; and
- office-specific liege authority.

## Supported Workflow

### Create control

Use:

- `clan submit <clan> <position> <new liege> [<liege appointment>] [<max appointees>]`

If the optional liege appointment is supplied, the control relationship becomes tied to that liege office.

### Adjust controlling appointment

Use:

- `clan vassal control <clan> <position> <liege appointment|none> [<liege clan>]`

This is the command path for setting or clearing appointment-scoped liege authority after the relationship already exists.

### Appoint or dismiss through the relationship

Use:

- `clan vassal appoint ...`
- `clan vassal dismiss ...`

These commands only operate on characters who are actually within the relationship’s appointee set.

## External Control Invariants

### Ambiguity handling

If multiple liege clans could match the same controlled appointment, the runtime requires the liege clan to be supplied explicitly.

### Two-sided relationship cleanup

External controls are referenced from both clans:

- the vassal clan’s `ExternalControls`;
- the liege clan’s `ExternalControls`.

Release and transfer logic now removes the relationship from both sides instead of leaving stale references behind on the liege clan.

### Appointee integrity

External dismiss logic now verifies that the target membership is actually in the relationship’s `Appointees` list before mutating the database and clan state.

This prevents removing arbitrary clan members from the appointment when they were not appointed through that specific external relationship.

### Capacity accounting

Appointment-capacity calculations must account for:

- current in-clan holders of the appointment; and
- unfilled appointment slots already reserved to liege clans.

The shared capacity helper now enforces:

- archived members do not count as active holders;
- unrelated external-control relationships do not reserve capacity; and
- already-filled external slots do not reserve extra capacity twice.

This fixes a long-standing bug where capped appointments could appear to have free space when they did not.

## Transfer Semantics

`clan transfer` moves the control relationship to a new liege clan.

The controlling appointment is not automatically rebound across clans, because a liege appointment id from the old liege clan is not valid in the new liege clan.

After transfer, the new liege can use:

- `clan vassal control ...`

to bind the relationship to a new liege appointment if appointment-scoped authority is desired.

## Remaining Practical Notes

- Multi-word appointment names should be quoted when ambiguity is possible.
- Multi-liege ambiguity should be resolved by supplying the liege clan explicitly.
- The builder/editor surface for authoring clans is still partly module-owned, but runtime external-control behaviour is now documented and command-addressable.
