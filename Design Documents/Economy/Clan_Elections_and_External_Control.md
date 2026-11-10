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

Offices that require confirmation pause at `AwaitingConfirmation` after voting closes. Approval returns them to `Preinstallation` until the scheduled installation date (or installs immediately if that date has passed). A veto ends the ballot in the terminal `Vetoed` stage. Existing persisted stage numbers remain unchanged.

### Confirmation and discretionary appointment

Use the appointment editor after selecting a clan:

```text
clan edit parish
clan set appointment "parish priest" election confirmation diocese bishop
clan set appointment "parish priest" election direct
```

`election confirmation <clan> <appointment>` names one specific office, in the same clan or another clan. No external-control relationship or reserved appointment slot is needed. Active holders of that exact office may confirm or veto the **whole election result**, including a multi-seat result. Parent-office authority and general clan privileges do not confer this power. Administrators retain an override. The confirming office cannot be the elected office itself, and live clans cannot depend on a template office.

```text
clan elections
clan election view <id>
clan election confirm <id>
clan election veto <id>
```

Confirming officeholders can discover and view the elections they oversee even without membership in the elected clan. This access does not grant nomination, voting, unrelated election visibility, or general access to that clan's structure. Login reminders identify outstanding confirmations. Secret ballots retain their usual restrictions on individual votes.

Election winners do not hold the office while confirmation is pending. Existing holders remain until a confirmed primary result replaces them. If withdrawal or resignation changes a result that was already approved during the lead time, that approval is revoked, its installation listener is cancelled, and the new result requires confirmation again. Confirmation does not shorten the configured lead time. If it arrives late, installation happens immediately and the next finite term is measured from the late installation rather than scheduling a term in the past.

A veto preserves the rejected ballot and votes in history with no victors, installs nobody, and leaves existing holders in place. A vetoed regular election starts a replacement regular ballot; a vetoed by-election rechecks vacancies and starts a replacement only for uncovered seats.

`election direct` toggles whether the confirming office also has direct appointment **and dismissal** authority. It defaults to off. When enabled, holders use ordinary `clan appoint <person> <clan> <appointment>` and `clan dismiss ...`, including across clans. Rank-to-hold restrictions, job restrictions and capacity still apply. A direct appointment fills a vacancy; dismiss an existing holder first when the office is full. Direct appointment does not itself veto a pending result: veto it explicitly if the chosen replacement should supersede that ballot. Finite directly appointed holders remain subject to the regular election cycle.

`election confirmation none` clears the authority and disables its direct-appointment power. Resolve pending results before changing the confirmation office. Referenced confirmation offices (or their clans) cannot be deleted/disbanded until their confirmation references are cleared. Disabling elections cancels open ballots and clears confirmation settings.

### Resignation

All appointments permit resignation by default, including existing appointments upgraded from older worlds:

```text
clan resign <clan> <appointment>
clan set appointment <appointment> resignation
```

The editor command toggles the permission. Resignation removes only the actor's office, retains clan membership/rank and other appointments, withdraws outstanding candidacy for that office, and runs the existing vacancy/by-election logic. Non-elected offices simply become vacant. Officeholders whose appointment comes from a current job must leave that job first. Resignation and membership removal also remove the holder from applicable external-control appointee lists; reserved external slots remain reserved rather than becoming electable seats.

### Indefinite elected tenure

```text
clan set appointment <appointment> election term none
```

`term 0` is equivalent. A zero `ElectionTerm` denotes indefinite tenure; it is distinct from the limits on the number of terms a person can win. Holders continue until death, resignation or another authorised removal. No recurring primary election is created on installation or reload. Vacancies trigger by-elections while preserving other holders. Death of the primary character vacates these offices; death/retirement of secondary bodies does not remove the original character's offices. Resurrection does not restore a vacated office.

Changing tenure cancels a scheduled, unstarted primary election and checks vacancies; an active primary ballot must finish first. Setting a positive term restores a regular cycle. Existing by-elections remain intact and installation always honours free capacity.

### Persistence and presentation

`Appointments` stores the optional confirming-office foreign key, `CanDirectlyAppoint` (default false), and `CanResign` (default true). `Elections.IsConfirmed` persists approvals, while the election stage records pending/vetoed outcomes. A pending result survives restart without installing anyone. Startup loads all clan offices and external-control reservations before resuming elections, so load order cannot bypass confirmation or start ballots for reserved seats. The migration and blank bootstrap snapshot carry compatible defaults for existing worlds.

Template copying retains resignation, tenure and discretionary-appointment settings. An authority inside the source template is rebound to the cloned office; references to offices in other clans retain their identity. Clan overview, appointment detail, election lists/history, personal clan output, score and login reminders expose the relevant policy or pending state.

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
