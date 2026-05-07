using Humanizer;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq.Extensions;
using MudSharp.Accounts;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Payment;
using MudSharp.Economy.Property;
using MudSharp.Editor;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Time;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TimeSpanParserUtil;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Appointment = MudSharp.Models.Appointment;
using ClanMembership = MudSharp.Community.ClanMembership;

#nullable enable
#nullable disable warnings

namespace MudSharp.Commands.Modules;

public class ClanModule : Module<ICharacter>
{
    private const string TemplateCloneElectorateAllMembersCanNominateProgName =
        "TemplateCloneElectorate_AllMembers_CanNominate";
    private const string TemplateCloneElectorateAllMembersWhyCantNominateProgName =
        "TemplateCloneElectorate_AllMembers_WhyCantNominate";
    private const string TemplateCloneElectorateAllMembersVotesProgName =
        "TemplateCloneElectorate_AllMembers_Votes";
    private const string TemplateCloneElectorateCouncillorCanNominateProgName =
        "TemplateCloneElectorate_Councillor_CanNominate";
    private const string TemplateCloneElectorateCouncillorWhyCantNominateProgName =
        "TemplateCloneElectorate_Councillor_WhyCantNominate";
    private const string TemplateCloneElectorateCouncillorVotesProgName =
        "TemplateCloneElectorate_Councillor_Votes";
    private const string TemplateCloneElectorateBoardCanNominateProgName =
        "TemplateCloneElectorate_Board_CanNominate";
    private const string TemplateCloneElectorateBoardWhyCantNominateProgName =
        "TemplateCloneElectorate_Board_WhyCantNominate";
    private const string TemplateCloneElectorateBoardVotesProgName =
        "TemplateCloneElectorate_Board_Votes";
    private const string CloneElectionCanNominateProgPrefix = "CloneElectionCanNominate_";
    private const string CloneElectionVotesProgPrefix = "CloneElectionVotes_";

    private ClanModule()
        : base("Clan")
    {
        IsNecessary = true;
    }

    public static ClanModule Instance { get; } = new();

    public static IClan? GetTargetClan(ICharacter actor, string targetText, bool includeTemplatesForPlayers = false)
    {
        IEnumerable<IClan> clans;
        if (actor.IsAdministrator())
        {
            clans = actor.Gameworld.Clans;
        }
        else
        {
            clans = actor.ClanMemberships.Select(x => x.Clan);
            if (includeTemplatesForPlayers)
            {
                clans = clans.Concat(actor.Gameworld.Clans.Where(x => x.IsTemplate));
            }
        }

        if (long.TryParse(targetText, out long id) && actor.IsAdministrator())
        {
            return clans.FirstOrDefault(x => x.Id == id);
        }

        clans = clans.ToList();

        return
            clans.FirstOrDefault(x => x.FullName.EqualTo(targetText)) ??
            clans.FirstOrDefault(x => x.Alias.EqualTo(targetText)) ??
            clans.FirstOrDefault(x => x.Alias.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase)) ??
            clans.FirstOrDefault(x => x.FullName.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase));
    }

    private static IEnumerable<IClan> GetExternallyAccessibleClans(ICharacter actor)
    {
        if (actor.IsAdministrator(PermissionLevel.Admin))
        {
            return actor.Gameworld.Clans;
        }

        return actor.ClanMemberships
                    .Select(x => x.Clan)
                    .Concat(
                        actor.ClanMemberships.SelectMany(
                            x => x.Clan.ExternalControls.Where(y => y.LiegeClan == x.Clan).Select(y => y.VassalClan)))
                    .Distinct()
                    .ToList();
    }

    private static IClan? GetTargetExternallyAccessibleClan(ICharacter actor, string targetText)
    {
        var clans = GetExternallyAccessibleClans(actor).ToList();
        return clans.FirstOrDefault(x => x.FullName.EqualTo(targetText)) ??
               clans.FirstOrDefault(x => x.Alias.EqualTo(targetText)) ??
               clans.FirstOrDefault(x => x.Alias.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase)) ??
               clans.FirstOrDefault(x => x.FullName.StartsWith(targetText, StringComparison.InvariantCultureIgnoreCase));
    }

    public const string ClanHelpText =
        @"This clan is used to both interact with and create or edit clans.  Clans can represent organisations, companies, hierarchies, fellowships and many other types of communities.

To see what clans you are a part of (or for admins to see all clans), use the following commands:

	#3clans#0 - shows all clans that you are a member of
	#3clans templates#0 - shows all template clans
	#3clan list#0 - a tabular view of all your clans (or all clans for admins)

The #3clan list#0 command can accept the following filter arguments:

	#6+<keyword>#0 - show only clans with this text in their name
	#6-<keyword>#0 - show only clans without this text in their name

The following clan sub-commands are used to interact with clans:

	#3clan invite <person> <clan> [<rank path>]#0 - invites a person to a clan
	#3clan castout <person> <clan>#0 - kicks a person out of a clan
	#3clan promote <person> <clan> [<rank>]#0 - promotes a person (1 rank or to the specified rank)
	#3clan demote <person> <clan> [<rank>]#0 - demotes a person (1 rank or to the specified rank)
	#3clan paygrade promote <person> <clan>#0 - promotes a person 1 paygrade rank
	#3clan paygrade demote <person> <clan>#0 - demotes a person 1 paygrade rank
	#3clan appoint <person> <clan> <appointment>#0 - appoints a person to a clan appointment
	#3clan dismiss <person> <clan> <appointment>#0 - dismisses a person from a clan appointment
	#3clan reportdead <clan> <person>#0 - reports a person as dead or long-term missing
	#3clan pay <person> <clan> <how much>#0 - manually adds backpay owing to a person
	#3clan budget <clan> <...>#0 - manages appointment budgets for a clan
	#3clan balance <clan>#0 - reviews a clan balance sheet
	#3clan payroll <clan> [member|rank|appointment <which>]#0 - reviews payroll history
	#3clan maxpay <clan> <max multiple backpay>#0 - sets the maximum backpay permissable for a clan
	#3clan payinterval <clan> ""<every x days|weeks|months|years>""#0 - sets the pay interval
	#3clan payinterval <clan> ""<every x days|weeks|months|years>"" [<date time>]#0 - sets the pay interval and the reference date time
	#3clan view <clan>#0 - views information about a clan
	#3clan members <clan>#0 - views the member list for a clan
	#3clan treasury <clan> [toggle]#0 - sets your current location as a treasury cell for a clan (ADMIN ONLY)
	#3clan treasury <clan> balance|ledger [count]#0 - reviews the clan's virtual treasury
	#3clan treasury <clan> deposit|withdraw <amount>#0 - moves cash into or out of the clan's virtual treasury
	#3clan admin <clan>#0 - sets your current location as an admin cell for a clan (ADMIN ONLY)
	#3clan vassal appoint <who> <clan> <position> [<liege clan>]#0 - appoints a person to a clan appointment in a vassal clan
	#3clan vassal dismiss <who> <clan> <position> [<liege clan>]#0 - dismisses a person from a clan appointment in a vassal clan
	#3clan vassal control <clan> <position> <liege appointment|none> [<liege clan>]#0 - sets which liege appointment controls a vassal appointment
	#3clan submit <clan> <position> <new liege> [<liege appointment>] [<max appointees>]#0 - submits a clan appointment to the control of an external clan
	#3clan release <vassal clan> <position> [<liege clan>]#0 - releases a vassal clan from your clan's control
	#3clan transfer <vassal clan> <position> <new liege> [<old liege>]#0 - transfers a vassal clan relationship to a new clan
	#3clan disband <clan>#0 - permanently disbands and deletes a clan - warning, cannot be undone
	#3clan economiczone list#0 - lists all economic zones which your clans control
	#3clan economiczone <which> <...>#0 - interacts with an economic zone your clan has control over
	#3clan properties#0 - shows which real estate properties your clan owns
	#3clan lease <clan> <property> <term>#0 - leases a property on behalf of a clan
	#3clan lease <clan> <property> <term> <bankcode>:<accn>#0 - leases a property on behalf of a clan
	#3clan buy <clan> <property>#0 - buys a property on behalf of the clan
	#3clan buy <clan> <property> <bankcode>:<accn>#0 - buys a property on behalf of the clan
	#3clan divest <clan> <property> <percentage> <target>#0 - divests ownership in a clan property to an individual
	#3clan divest <clan> <property> <percentage> *<other clan>#0 - divests ownership in a clan property to another clan

The following clan sub-commands pertain to elections and voting:

	#3clan elections [<clan>]#0 - views all elections
	#3clan election view <election id> | <clan> <appointment>#0 - views a specific election
	#3clan election history <election id> | <clan> <appointment>#0 - views the history of elections for a particular clan or position
	#3clan nominate <election id> | <clan> <appointment>#0 - nominates yourself as a candidate in an election
	#3clan withdrawnomination <election id> | <clan> <appointment>#0 - withdraws yourself as a candidate from an election
	#3clan vote <election id> | <clan> <appointment> <who>#0 - votes in an election

The command to create a new clan is as follows:

	#3clan create new <alias> <name>#0 - creates an empty clan
	#3clan create template <template> <alias> <name>#0 - creates a clan from a clan template
	#3clan templates#0 - shows a list of clan templates

In order to edit properties, positions, ranks etc of a clan one must:

	#3clan edit <clan>#0

All of the following commands must happen with an edited clan selected:

	#3clan create rank <name> <abbreviation> before|after <other>#0 - creates a new rank
	#3clan create paygrade <name> <abbreviation> <currency> <amount>#0 - creates a new paygrade
	#3clan create appointment <name> <abbreviation> [under <other>]#0 - creates a new appointment
	#3clan remove rank <rank>#0 - deletes a rank
	#3clan remove appointment <appointment>#0 - deletes an appointment
	#3clan remove paygrade <paygrade>#0 - deletes a paygrade
	#3clan set name <name>#0 - sets the clan's name
	#3clan set alias <alias>#0 - sets the clan's alias
	#3clan set desc#0 - drops you into an editor to set clan's description
	#3clan set rank ...#0 - edits clan ranks. See command for more info.
	#3clan set paygrade ...#0 - edits paygrades. See command for more info.
	#3clan set appointment ...#0 - edits appointments. See command for more info.
	#3clan set sphere <which>#0 - sets the sphere for this clan (admin only)
	#3clan set template#0 - toggles this clan being a template clan (admin only)
	#3clan set notable#0 - toggles this clan appearing in WHO (admin only)
	#3clan set notablemembers#0 - toggles the famous members of this clan appearing in NOTABLES (admin only)
	#3clan set bankaccount <account>|none#0 - sets or clears a bank account for this clan
	#3clan set calendar <which>#0 - changes the calendar that the clan uses";

    [PlayerCommand("Clan", "clan")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [HelpInfo("clan", ClanHelpText, AutoHelp.HelpArgOrNoArg)]
    protected static void Clan(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());

        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "pay":
                ClanPay(actor, ss);
                return;
            case "budget":
                ClanBudget(actor, ss);
                return;
            case "balance":
            case "balancesheet":
                ClanBalanceSheet(actor, ss);
                return;
            case "payroll":
                ClanPayroll(actor, ss);
                return;
            case "invite":
                ClanInvite(actor, ss);
                return;
            case "backpay":
                ClanBackpay(actor, ss);
                return;
            case "castout":
                ClanCastout(actor, ss);
                return;
            case "promote":
                ClanPromote(actor, ss);
                return;
            case "demote":
                ClanDemote(actor, ss);
                return;
            case "paygrade":
                ClanPaygrade(actor, ss);
                return;
            case "payinterval":
                ClanPayInterval(actor, ss);
                return;
            case "view":
            case "show":
                ClanView(actor, ss);
                return;
            case "create":
                ClanCreate(actor, ss);
                return;
            case "remove":
                ClanRemove(actor, ss);
                return;
            case "edit":
                ClanEdit(actor, ss);
                return;
            case "set":
                ClanSet(actor, ss);
                return;
            case "submit":
                ClanSubmit(actor, ss);
                return;
            case "release":
                ClanRelease(actor, ss);
                return;
            case "disband":
                ClanDisband(actor, ss);
                return;
            case "appoint":
                ClanAppoint(actor, ss);
                return;
            case "dismiss":
                ClanDismiss(actor, ss);
                return;
            case "transfer":
                ClanTransfer(actor, ss);
                return;
            case "members":
                ClanMembers(actor, ss);
                return;
            case "vassal":
                ClanVassal(actor, ss);
                return;
            case "reportdead":
                ClanReportDead(actor, ss);
                return;
            case "maxbackpay":
            case "max backpay":
            case "max pay":
            case "maxpay":
            case "cap":
            case "cap backpay":
            case "cap pay":
            case "capbackpay":
            case "cappay":
                ClanMaxBackpay(actor, ss);
                return;
            case "treasury":
                if (!actor.IsAdministrator())
                {
                    goto default;
                }

                ClanTreasury(actor, ss);
                return;
            case "admin":
            case "administration":
                if (!actor.IsAdministrator())
                {
                    goto default;
                }

                ClanAdministration(actor, ss);
                return;
            case "election":
                ClanElection(actor, ss);
                return;
            case "elections":
                ClanElections(actor, ss);
                return;
            case "vote":
                ClanVote(actor, ss);
                return;
            case "nominate":
                ClanNominate(actor, ss);
                return;
            case "withdrawnomination":
                ClanWithdrawNomination(actor, ss);
                return;
            case "economiczone":
            case "ez":
                ClanEconomicZone(actor, ss);
                return;
            case "properties":
                ClanProperties(actor, ss);
                return;
            case "lease":
                ClanLease(actor, ss);
                return;
            case "buy":
            case "buyproperty":
                ClanBuyProperty(actor, ss);
                return;
            case "divest":
            case "divestownership":
                ClanDivestOwnership(actor, ss);
                return;
            case "templates":
                ClanTemplates(actor, ss);
                return;
            case "list":
                ClanList(actor, ss);
                return;
            default:
                actor.OutputHandler.Send(ClanHelpText.SubstituteANSIColour(), nopage: true);
                return;
        }
    }

    private static void ClanBudget(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to manage budgets for?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.BudgetCommand(actor, command);
    }

    private static void ClanBalanceSheet(ICharacter actor, StringStack command)
    {
        if (!command.IsFinished && command.PeekSpeech().EqualTo("sheet"))
        {
            command.PopSpeech();
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to review the balance sheet for?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.ShowBalanceSheet(actor);
    }

    private static void ClanPayroll(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to review payroll history for?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.ShowPayrollHistory(actor, command);
    }

    private static void ClanRemove(ICharacter actor, StringStack ss)
    {
        IClan clan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().FirstOrDefault()?.EditingItem;
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not editing any clans.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        switch (ss.PopForSwitch())
        {
            case "rank":
                if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
                    !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateRanks))
                {
                    actor.OutputHandler.Send("You are not allowed to remove ranks in that clan.");
                    return;
                }

                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which rank do you want to remove?");
                    return;
                }

                IRank rank = clan.Ranks.GetByIdOrName(ss.SafeRemainingArgument);
                if (rank is null)
                {
                    actor.OutputHandler.Send($"There is no such rank in the {clan.FullName.ColourName()} clan.");
                    return;
                }

                if (clan.Ranks.Count == 1)
                {
                    actor.OutputHandler.Send("You cannot delete the last rank. All clans must have at least one rank.");
                    return;
                }

                clan.DeleteRank(rank);
                actor.OutputHandler.Send($"You delete the rank {rank.Name.ColourValue()} in the clan {clan.FullName.ColourName()}.");
                return;
            case "paygrade":
                if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
                    !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreatePaygrades))
                {
                    actor.OutputHandler.Send("You are not allowed to remove paygrades in that clan.");
                    return;
                }

                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which paygrade do you want to remove?");
                    return;
                }

                IPaygrade paygrade = clan.Paygrades.GetByIdOrName(ss.SafeRemainingArgument);
                if (paygrade is null)
                {
                    actor.OutputHandler.Send($"There is no such paygrade in the {clan.FullName.ColourName()} clan.");
                    return;
                }

                clan.DeletePaygrade(paygrade);
                actor.OutputHandler.Send($"You delete the paygrade {paygrade.Name.ColourValue()} in the clan {clan.FullName.ColourName()}.");
                return;
            case "appointment":
                if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
                    !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments))
                {
                    actor.OutputHandler.Send("You are not allowed to remove appointments in that clan.");
                    return;
                }

                if (ss.IsFinished)
                {
                    actor.OutputHandler.Send("Which appointment do you want to remove?");
                    return;
                }

                IAppointment appointment = clan.Appointments.GetByIdOrName(ss.SafeRemainingArgument);
                if (appointment is null)
                {
                    actor.OutputHandler.Send($"There is no such appointment in the {clan.FullName.ColourName()} clan.");
                    return;
                }

                clan.DeleteAppointment(appointment);
                actor.OutputHandler.Send($"You delete the appointment {appointment.Name.ColourValue()} in the clan {clan.FullName.ColourName()}.");
                return;
            default:
                actor.OutputHandler.Send("You must specify #3rank#0, #3appointment#0 or #3paygrade#0.");
                return;
        }
    }

    private static void ClanPayInterval(ICharacter actor, StringStack ss)
    {
        IClan clan = GetTargetClan(actor, ss.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreatePaygrades))
        {
            actor.OutputHandler.Send("You are not allowed to manage the payment affairs of that clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"What should the pay interval be for this clan?\n{"Use forms like: every <x> hours|days|weekdays|weeks|months|years [<from time>], every month on day 15, or every month on the 5th or last Wednesday".ColourCommand()}");
            return;
        }

        if (!RecurringInterval.TryParse(ss.PopSpeech(), clan.Calendar, out RecurringInterval interval, out string intervalError))
        {
            actor.OutputHandler.Send(
                $"That is not a valid interval: {intervalError}\n{"Use forms like: every <x> hours|days|weekdays|weeks|months|years [<from time>], every month on day 15, or every month on the 5th or last Wednesday".ColourCommand()}");
            return;
        }

        if (ss.IsFinished)
        {
            clan.PayInterval = interval;
            actor.OutputHandler.Send($"The {clan.FullName.ColourName()} will now pay on a cycle of {interval.Describe(clan.Calendar).ColourValue()}.");
            return;
        }

        if (!MudDateTime.TryParse(ss.SafeRemainingArgument, clan.Calendar, clan.Calendar.FeedClock, actor, out MudDateTime dt, out string error))
        {
            MudDate date = clan.Calendar.CurrentDateTime.Date;
            MudTime time = clan.Calendar.CurrentDateTime.Time;
            IMudTimeZone tz = clan.Calendar.CurrentDateTime.TimeZone;
            actor.OutputHandler.Send($"That is not a valid date and time for the {clan.Calendar.FullName.ColourName()} calendar and {clan.Calendar.FeedClock.Name.ColourName()} clock.\n{error}");
            return;
        }

        MudDateTime next = interval.GetNextDateTime(dt);
        clan.PayInterval = interval;
        clan.NextPay = next;
        actor.OutputHandler.Send($"The {clan.FullName.ColourName()} will now pay on a cycle of {interval.Describe(clan.Calendar).ColourValue()} with the next pay happening at {next.ToString(CalendarDisplayMode.Long, TimeDisplayTypes.Short).ColourValue()}.");
    }
    private static void ClanList(ICharacter actor, StringStack ss)
    {
        IEnumerable<IClan> clans;
        if (actor.IsAdministrator())
        {
            clans = actor.Gameworld.Clans;
        }
        else
        {
            clans = actor.ClanMemberships.Select(x => x.Clan).Distinct();
        }
        while (!ss.IsFinished)
        {
            string text = ss.PopSpeech().ToLowerInvariant();
            if (text.StartsWith('+') && text.Length > 1)
            {
                text = text[1..];
                clans = clans.Where(x => x.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase) || x.FullName.Contains(text, StringComparison.InvariantCultureIgnoreCase));
                continue;
            }
            if (text.StartsWith('-') && text.Length > 1)
            {
                text = text[1..];
                clans = clans.Where(x => !x.Name.Contains(text, StringComparison.InvariantCultureIgnoreCase) && !x.FullName.Contains(text, StringComparison.InvariantCultureIgnoreCase));
                continue;
            }
            actor.OutputHandler.Send($"The text {text.ColourCommand()} is not a valid filter option.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine($"Clans:");
        sb.AppendLine();
        if (actor.IsAdministrator())
        {
            TimeSpan ts = TimeSpan.FromDays(30);
            sb.AppendLine(StringUtilities.GetTextTable(
            from clan in clans
            let membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            select new List<string>
            {
                clan.Id.ToString("N0", actor),
                clan.FullName,
                clan.Alias,
                clan.Memberships.Count().ToString("N0", actor),
                clan.Memberships.Count(x => x.MemberCharacter.State.IsAlive()).ToString("N0", actor),
                clan.Memberships.Count(x => (DateTime.UtcNow - x.MemberCharacter.LastLogoutDateTime) < ts).ToString("N0", actor),
                clan.ShowClanMembersInWho.ToColouredString(),
                clan.ShowFamousMembersInNotables.ToColouredString(),
                clan.Sphere ?? ""
            },
            new List<string>
            {
                "Id",
                "Name",
                "Alias",
                "Members",
                "Alive",
                "Active",
                "Who?",
                "Notables?",
                "Sphere"
            },
            actor,
            Telnet.Green
        ));
        }
        else
        {
            sb.AppendLine(StringUtilities.GetTextTable(
            from clan in clans
            let membership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            select new List<string>
            {
                clan.Id.ToString("N0", actor),
                clan.FullName,
                clan.Alias,
                membership?.Rank.Title(actor) ?? "",
                membership?.Paygrade?.Name ?? "",
                membership?.Appointments.Select(x => x.Title(actor)).ListToCommaSeparatedValues(", ") ?? "",
                membership?.PersonalName.GetName(NameStyle.FullName) ?? ""
            },
            new List<string>
            {
                "Id",
                "Name",
                "Alias",
                "My Rank",
                "My Paygrade",
                "My Appointments",
                "Knows Me As"
            },
            actor,
            Telnet.Green
        ));
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanTemplates(ICharacter actor, StringStack ss)
    {
        StringBuilder sb = new();
        sb.AppendLine($"Clan Templates:");
        sb.AppendLine();
        sb.AppendLine(StringUtilities.GetTextTable(
            from clan in actor.Gameworld.Clans
            where clan.IsTemplate
            select new List<string>
            {
                clan.Id.ToString("N0", actor),
                clan.FullName,
                clan.Description
            },
            new List<string>
            {
                "Id",
                "Name",
                "Description"
            },
            actor,
            Telnet.Green
        ));
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanProperties(ICharacter actor, StringStack ss)
    {
        StringBuilder sb = new();
        List<IProperty> owned = actor.Gameworld.Properties.Where(x =>
                             x.PropertyOwners.Any(y =>
                                 y.Owner is IClan clan && actor.ClanMemberships.Any(z => z.Clan == clan)) &&
                             x.IsAuthorisedOwner(actor))
                         .ToList();
        if (owned.Any())
        {
            sb.AppendLine($"Your clans own the following properties:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from property in owned
                let owner = property.PropertyOwners.First(x =>
                    x.Owner is IClan clan && actor.ClanMemberships.Any(y =>
                        y.Clan == clan && y.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanProperty)))
                select new List<string>
                {
                    property.Name,
                    property.LastChangeOfOwnership?.Date.Display(CalendarDisplayMode.Short) ?? "Never",
                    property.Lease == null
                        ? "No"
                        : $"{property.EconomicZone.Currency.Describe(property.Lease.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.Lease.Interval.Describe(property.EconomicZone.FinancialPeriodReferenceCalendar)} until {property.Lease.LeaseEnd.Date.Display(CalendarDisplayMode.Short)}",
                    property.SaleOrder != null
                        ? property.EconomicZone.Currency.Describe(property.SaleOrder.ReservePrice,
                            CurrencyDescriptionPatternType.Short)
                        : "No",
                    ((IClan)owner.Owner).FullName,
                    owner.ShareOfOwnership.ToString("P2", actor)
                },
                new List<string>
                {
                    "Name",
                    "Last Sold",
                    "Leased?",
                    "For Sale",
                    "Clan",
                    "Stake"
                },
                actor.LineFormatLength,
                colour: Telnet.Green,
                unicodeTable: actor.Account.UseUnicode
            ));
        }
        else
        {
            sb.AppendLine("None of your clans own any properties.");
        }

        sb.AppendLine();

        List<IProperty> leased = actor.Gameworld.Properties.Where(x =>
                              x.Lease is not null && !x.Lease.Leaseholder.Equals(actor) &&
                              x.IsAuthorisedLeaseHolder(actor))
                          .ToList();
        if (leased.Any())
        {
            sb.AppendLine("Your clans are leasing the following properties:");
            sb.AppendLine(StringUtilities.GetTextTable(
                from property in leased
                select new List<string>
                {
                    property.Name,
                    property.Lease!.LeaseEnd.Date.Display(CalendarDisplayMode.Short),
                    $"{property.EconomicZone.Currency.Describe(property.Lease.LeaseOrder.PricePerInterval, CurrencyDescriptionPatternType.Short)} {property.Lease.LeaseOrder.Interval.Describe(property.EconomicZone.FinancialPeriodReferenceCalendar)}",
                    property.EconomicZone.Currency.Describe(property.Lease.PaymentBalance,
                        CurrencyDescriptionPatternType.Short),
                    property.Lease!.Interval.GetNextDateTime(property.Lease!.LastLeasePayment).Date
                            .Display(CalendarDisplayMode.Short),
                    (property.Lease.AutoRenew && property.LeaseOrder == property.Lease.LeaseOrder).ToString(),
                    ((IClan)property.Lease.Leaseholder).FullName
                },
                new List<string>
                {
                    "Name",
                    "Lease End",
                    "Price",
                    "Balance",
                    "Next Due",
                    "Autorenew",
                    "Clan"
                },
                actor.LineFormatLength,
                colour: Telnet.Green,
                unicodeTable: actor.Account.UseUnicode
            ));
        }
        else
        {
            sb.AppendLine("None of your clans are leasing any properties.");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanLease(ICharacter actor, StringStack ss)
    {
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
        if (ez == null)
        {
            actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to lease property in the name of?");
            return;
        }

        IClan clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(ss.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (!actor.ClanMemberships.First(x => x.Clan == clan).NetPrivileges
                  .HasFlag(ClanPrivilegeType.CanManageClanProperty) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send(
                $"You are not authorised to manage the properties of the {clan.FullName.ColourName()} clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which property do you want to lease? Please see {"PROPERTIES".MXPSend("properties", "The properties command")} for a list of properties.");
            return;
        }

        List<IProperty> properties = actor.Gameworld.Properties
                              .Where(x => x.EconomicZone == ez && x.LeaseOrder?.ListedForLease == true).ToList();
        IProperty property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
        if (property == null)
        {
            actor.OutputHandler.Send("There is no such property currently available for lease.");
            return;
        }

        if (property.Lease is not null)
        {
            actor.OutputHandler.Send(
                $"The {property.Name.ColourName()} property is already leased until {property.Lease.LeaseEnd.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"How long do you want to lease this property for?\nYou may lease for any duration between {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()} and {property.LeaseOrder!.MaximumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()}.");
            return;
        }

        if (!TimeSpanParser.TryParse(ss.PopSpeech(), Units.Weeks, Units.Weeks, out TimeSpan duration))
        {
            actor.OutputHandler.Send(
                $"That was not a valid time span.\n{"Note: Years and Months are not supported, use Weeks or Days in that case".ColourCommand()}");
            return;
        }

        if (duration < property.LeaseOrder!.MinimumLeaseDuration ||
            duration > property.LeaseOrder.MaximumLeaseDuration)
        {
            actor.OutputHandler.Send(
                $"You may only lease for a duration between {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()} and {property.LeaseOrder!.MinimumLeaseDuration.Humanize(3, actor.Account.Culture).ColourValue()}.");
            return;
        }

        decimal amount = Math.Truncate(property.LeaseOrder!.BondRequired + 2.0M * property.LeaseOrder.PricePerInterval);
        // Cash payment
        if (ss.IsFinished)
        {
            OtherCashPayment payment = new(ez.Currency, actor);
            if (payment.AccessibleMoneyForPayment() < amount)
            {
                actor.OutputHandler.Send(
                    $"You aren't holding enough money to lease {property.Name.ColourValue()} as it costs {ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}\nNote: This amount is a minimum of two payment periods in rent plus bond.\nYou are only holding {ez.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
                return;
            }

            payment.TakePayment(amount);
        }
        else
        {
            (IBankAccount account, string error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
            if (account == null)
            {
                actor.OutputHandler.Send(error);
                return;
            }

            if (!account.IsAuthorisedAccountUser(actor))
            {
                actor.OutputHandler.Send("You are not an authorised user for that bank account.");
                return;
            }

            (bool truth, string accountError) = account.CanWithdraw(amount, true);
            if (!truth)
            {
                actor.OutputHandler.Send(accountError);
                return;
            }

            account.WithdrawFromTransaction(amount, $"Lease of {property.Name}");
            account.Bank.CurrencyReserves[ez.Currency] -= amount;
        }

        decimal depositAmount = amount - property.LeaseOrder.BondRequired;

        foreach (IPropertyOwner owner in property.PropertyOwners)
        {
            owner.CreditRevenue(ez.Currency, depositAmount * owner.ShareOfOwnership, actor, property,
                $"Lease of {property.Name}", ez.FinancialPeriodReferenceCalendar.CurrentDateTime);
        }

        property.Lease = property.LeaseOrder.CreateLease(clan, duration);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ lease|leases the property {property.Name.ColourName()} on behalf of {clan.FullName.ColourName()}.",
            actor, actor)));
        if (property.PropertyKeys.Any())
        {
            IGameItem givenItem;
            if (property.PropertyKeys.Count() == 1)
            {
                givenItem = property.PropertyKeys.Single().GameItem;
                property.PropertyKeys.Single().IsReturned = false;
                givenItem.Login();
            }
            else
            {
                List<IGameItem> list = new();
                foreach (IPropertyKey key in property.PropertyKeys)
                {
                    list.Add(key.GameItem);
                    key.IsReturned = false;
                    key.GameItem.Login();
                }

                givenItem = PileGameItemComponentProto.CreateNewBundle(list);
                actor.Gameworld.Add(givenItem);
            }

            if (actor.Body.CanGet(givenItem, 0))
            {
                actor.Body.Get(givenItem, silent: true);
            }
            else
            {
                givenItem.RoomLayer = actor.RoomLayer;
                actor.Location.Insert(givenItem, true);
                actor.OutputHandler.Send($"You couldn't hold {givenItem.HowSeen(actor)}, so it is on the ground.");
            }
        }
    }

    private static void ClanBuyProperty(ICharacter actor, StringStack ss)
    {
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
        if (ez == null)
        {
            actor.OutputHandler.Send("Your current location is not a conveyancing location for any economic zones.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to lease property in the name of?");
            return;
        }

        IClan clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(ss.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (!actor.ClanMemberships.First(x => x.Clan == clan).NetPrivileges
                  .HasFlag(ClanPrivilegeType.CanManageClanProperty) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send(
                $"You are not authorised to manage the properties of the {clan.FullName.ColourName()} clan.");
            return;
        }

        List<IProperty> properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.SaleOrder?.ShowForSale == true)
                              .ToList();
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which property are you looking to buy? Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
            return;
        }

        IProperty property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
        if (property == null)
        {
            actor.OutputHandler.Send(
                $"There is no such property. Please see the {"PROPERTIES".MXPSend("properties", "The properties command")} command for a list of available properties.");
            return;
        }

        decimal amount = Math.Truncate(property.SaleOrder!.ReservePrice);
        // Cash payment
        if (ss.IsFinished)
        {
            OtherCashPayment payment = new(ez.Currency, actor);
            if (payment.AccessibleMoneyForPayment() < amount)
            {
                actor.OutputHandler.Send(
                    $"You aren't holding enough money to purchase {property.Name.ColourValue()} as it costs {ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).ColourValue()}.\nYou are only holding {ez.Currency.Describe(payment.AccessibleMoneyForPayment(), CurrencyDescriptionPatternType.Short).ColourValue()}.");
                return;
            }

            payment.TakePayment(amount);
        }
        else
        {
            (IBankAccount account, string error) = Economy.Banking.Bank.FindBankAccount(ss.SafeRemainingArgument, null, actor);
            if (account == null)
            {
                actor.OutputHandler.Send(error);
                return;
            }

            if (!account.IsAuthorisedAccountUser(actor))
            {
                actor.OutputHandler.Send("You are not an authorised user for that bank account.");
                return;
            }

            (bool truth, string accountError) = account.CanWithdraw(amount, true);
            if (!truth)
            {
                actor.OutputHandler.Send(accountError);
                return;
            }

            account.WithdrawFromTransaction(amount, $"Purchase of {property.Name}");
            account.Bank.CurrencyReserves[ez.Currency] -= amount;
        }

        foreach (IPropertyOwner owner in property.PropertyOwners)
        {
            owner.CreditRevenue(ez.Currency, amount * owner.ShareOfOwnership, actor, property,
                $"Sale of {property.Name}", ez.FinancialPeriodReferenceCalendar.CurrentDateTime);
        }

        property.SellProperty(clan);
        actor.OutputHandler.Handle(new EmoteOutput(new Emote(
            $"@ purchase|purchases the property {property.Name.ColourName()} for $1 on behalf of {clan.FullName.ColourName()}.",
            actor, actor, new DummyPerceivable(ez.Currency.Describe(amount, CurrencyDescriptionPatternType.Short)))));

        if (property.PropertyKeys.Any(x => !x.IsReturned))
        {
            IGameItem givenItem;
            if (property.PropertyKeys.Count(x => !x.IsReturned) == 1)
            {
                givenItem = property.PropertyKeys.Single(x => !x.IsReturned).GameItem;
                property.PropertyKeys.Single(x => !x.IsReturned).IsReturned = false;
                givenItem.Login();
            }
            else
            {
                List<IGameItem> list = new();
                foreach (IPropertyKey key in property.PropertyKeys.Where(x => !x.IsReturned))
                {
                    list.Add(key.GameItem);
                    key.IsReturned = false;
                    key.GameItem.Login();
                }

                givenItem = PileGameItemComponentProto.CreateNewBundle(list);
                actor.Gameworld.Add(givenItem);
            }

            if (actor.Body.CanGet(givenItem, 0))
            {
                actor.Body.Get(givenItem, silent: true);
            }
            else
            {
                givenItem.RoomLayer = actor.RoomLayer;
                actor.Location.Insert(givenItem, true);
                actor.OutputHandler.Send($"You couldn't hold {givenItem.HowSeen(actor)}, so it is on the ground.");
            }
        }
    }

    private static void ClanDivestOwnership(ICharacter actor, StringStack ss)
    {
        IEconomicZone ez = actor.Gameworld.EconomicZones.FirstOrDefault(x => x.ConveyancingCells.Contains(actor.Location));
        if (ez == null)
        {
            actor.OutputHandler.Send(
                "Your current location is not a conveyancing location for any economic zones.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("From which clan do you want to divest ownership of a property?");
            return;
        }

        IClan clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(ss.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (!actor.ClanMemberships.First(x => x.Clan == clan).NetPrivileges
                  .HasFlag(ClanPrivilegeType.CanManageClanProperty) && !actor.IsAdministrator())
        {
            actor.OutputHandler.Send(
                $"You are not authorised to manage the properties of the {clan.FullName.ColourName()} clan.");
            return;
        }

        List<IProperty> properties = actor.Gameworld.Properties.Where(x => x.EconomicZone == ez && x.IsAuthorisedOwner(actor))
                              .ToList();
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which property are you looking to divest ownership of? Please see the {"CLAN PROPERTIES".MXPSend("clan properties", "The properties command")} command for a list of available properties.");
            return;
        }

        IProperty property = properties.GetFromItemListByKeywordIncludingNames(ss.PopSpeech(), actor);
        if (property == null)
        {
            actor.OutputHandler.Send(
                $"The clan {clan.FullName.ColourName()} does not own such a property. Please see the {"CLAN PROPERTIES".MXPSend("clan properties", "The properties command")} command for a list of available properties.");
            return;
        }

        if (property.PropertyOwners.All(x => x.Owner != clan))
        {
            actor.OutputHandler.Send(
                $"The clan {clan.FullName.ColourName()} does not own {property.Name.ColourName()}. If you want to divest ownership of your own property, use the PROPERTY DIVEST command instead.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What percentage of the clan {clan.FullName.ColourName()}'s ownership stake in {property.Name.ColourName()} do you want to divest?");
            return;
        }

        if (!ss.PopSpeech().TryParsePercentageDecimal(actor.Account.Culture, out decimal value))
        {
            actor.OutputHandler.Send($"The text {ss.Last.ColourCommand()} is not a valid percentage.");
            return;
        }

        if (value <= 0.0M || value > 1.0M)
        {
            actor.OutputHandler.Send(
                $"The percentage must be greater than {0.0.ToString("P", actor).ColourValue()} and less than or equal to {1.0.ToString("P", actor).ColourValue()}.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send($"To whom do you want to divest ownership in {property.Name.ColourName()}?");
            return;
        }

        string text = ss.SafeRemainingArgument;
        if (text[0] == '*' && text.Length > 1)
        {
            text = text[1..];
            IClan tclan = actor.Gameworld.Clans.GetClan(text);
            if (tclan == null)
            {
                actor.OutputHandler.Send($"There is no such clan as {text.ColourCommand()}.");
                return;
            }

            if (clan == tclan)
            {
                actor.OutputHandler.Send("You cannot divest ownership of a clan's property to the clan itself.");
                return;
            }

            actor.OutputHandler.Send(
                $"Are you sure that you want to divest {value.ToString("P", actor).ColourValue()} of your ownership in {property.Name.ColourName()} to the clan {tclan.FullName.ColourName()}?\n{Accept.StandardAcceptPhrasing}");
            actor.AddEffect(new Accept(actor, new GenericProposal
            {
                AcceptAction = text =>
                {
                    if (property.PropertyOwners.All(x => x.Owner != clan))
                    {
                        actor.OutputHandler.Send(
                            $"The clan {clan.FullName.ColourName()} no longer owns any part of the property {property.Name.ColourName()}.");
                        return;
                    }

                    actor.OutputHandler.Handle(new EmoteOutput(new Emote(
                        "@ divest|divests $1 of $4 ownership of $2 to $3.", actor, actor,
                        new DummyPerceivable(voyeur => value.ToString("P", voyeur).ColourValue()),
                        new DummyPerceivable(property.Name, customColour: Telnet.Cyan),
                        new DummyPerceivable(tclan.FullName, customColour: Telnet.Cyan),
                        new DummyPerceivable($"{clan.FullName}'s", customColour: Telnet.Cyan))));
                    property.DivestOwnership(property.PropertyOwners.First(x => x.Owner == clan), value, tclan);
                },
                RejectAction = text =>
                {
                    actor.OutputHandler.Send(
                        $"You decide not to divest {clan.FullName.ColourName()}'s ownership in the property {property.Name.ColourName()}.");
                },
                ExpireAction = () =>
                {
                    actor.OutputHandler.Send(
                        $"You decide not to divest {clan.FullName.ColourName()}'s ownership in the property {property.Name.ColourName()}.");
                },
                Keywords = new List<string> { "divest", "property", "ownership" },
                DescriptionString = $"divesting ownership of the {property.Name} property"
            }), 120.Seconds());
            return;
        }

        ICharacter target = actor.TargetActor(text);
        if (target == null)
        {
            actor.OutputHandler.Send("You don't see anyone like that.");
            return;
        }

        actor.OutputHandler.Send(
            $"Are you sure that you want to divest {value.ToString("P", actor).ColourValue()} of {clan.FullName.ColourName()}'s ownership in {property.Name.ColourName()} to {(target == actor ? "yourself" : target.HowSeen(actor))}?\n{Accept.StandardAcceptPhrasing}");
        actor.AddEffect(new Accept(actor, new GenericProposal
        {
            AcceptAction = text =>
            {
                if (property.PropertyOwners.All(x => x.Owner != clan))
                {
                    actor.OutputHandler.Send(
                        $"The clan {clan.FullName.ColourName()} no longer own any part of the property {property.Name.ColourName()}.");
                    return;
                }

                actor.OutputHandler.Handle(new EmoteOutput(new Emote(
                    "@ divest|divests $1 of $4 ownership of $2 to $3=0.", actor, actor,
                    new DummyPerceivable(voyeur => value.ToString("P", voyeur).ColourValue()),
                    new DummyPerceivable(property.Name, customColour: Telnet.Cyan), target,
                    new DummyPerceivable($"the clan {clan.FullName}'s", customColour: Telnet.Cyan))));
                property.DivestOwnership(property.PropertyOwners.First(x => x.Owner == clan), value, target);
            },
            RejectAction = text =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to divest {clan.FullName.ColourName()}'s ownership in the property {property.Name.ColourName()}.");
            },
            ExpireAction = () =>
            {
                actor.OutputHandler.Send(
                    $"You decide not to divest {clan.FullName.ColourName()}'s ownership in the property {property.Name.ColourName()}.");
            },
            Keywords = new List<string> { "divest", "property", "ownership" },
            DescriptionString = $"divesting ownership of the {property.Name} property"
        }), 120.Seconds());
    }

    private static void ClanEditNotableMembers(ICharacter actor, StringStack ss, IClan clan)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only admins may use this command.");
            return;
        }

        clan.ShowFamousMembersInNotables = !clan.ShowFamousMembersInNotables;
        clan.Changed = true;
        actor.OutputHandler.Send(
            $"The {clan.Name.ColourName()} clan will {(clan.ShowFamousMembersInNotables ? "now" : "no longer")} show famous individuals in the notables command.");
    }

    private static void ClanEditDiscord(ICharacter actor, StringStack ss, IClan clan)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only admins may use this command.");
            return;
        }

        if (ss.SafeRemainingArgument.EqualTo("none"))
        {
            clan.DiscordChannelId = null;
            clan.Changed = true;
            actor.OutputHandler.Send("This clan will no longer have a discord channel for broadcasting events.");
            return;
        }

        if (!ulong.TryParse(ss.SafeRemainingArgument, out ulong discordChannelId))
        {
            actor.OutputHandler.Send("That is not a valid discord channel ID.");
            return;
        }

        clan.DiscordChannelId = discordChannelId;
        clan.Changed = true;
        actor.OutputHandler.Send(
            $"This clan will now broadcast events to the discord channel {discordChannelId.ToString("F0", actor).ColourName()}.");
    }

    private static void ClanEditSphere(ICharacter actor, StringStack ss, IClan clan)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only admins may use this command.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"You must either specify a sphere for this clan, or use {"clear".ColourCommand()} to clear an existing one.");
            return;
        }

        if (ss.SafeRemainingArgument.EqualTo("clear"))
        {
            clan.Sphere = null;
            clan.Changed = true;
            actor.OutputHandler.Send($"The {clan.Name.ColourName()} clan no longer belongs to a sphere.");
            return;
        }

        clan.Sphere = ss.SafeRemainingArgument.TitleCase();
        clan.Changed = true;
        actor.OutputHandler.Send(
            $"The {clan.Name.ColourName()} clan now belongs to the {clan.Sphere.Colour(Telnet.Magenta)} sphere.");
    }

    private static void ClanEconomicZone(ICharacter actor, StringStack ss)
    {
        List<IClanMembership> clans = actor.ClanMemberships
                         .Where(x => x.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageEconomicZones)).ToList();
        List<IEconomicZone> zones = actor.Gameworld.EconomicZones.Where(x => clans.Any(y => y.Clan == x.ControllingClan)).ToList();
        if (!zones.Any())
        {
            actor.OutputHandler.Send(
                "You are not a member of any clan that has controlled economic zones or which you have any privileges with those zones.");
            return;
        }

        string cmd = ss.PopSpeech();
        if (cmd.EqualTo("list"))
        {
            StringBuilder sb = new();
            sb.AppendLine("List of Clans with Economic Zones or Privileges:");
            foreach (IClanMembership clan in clans)
            {
                sb.AppendLine();
                sb.AppendLine(clan.Clan.FullName.ColourName());
                foreach (IEconomicZone zone in zones.Where(x => x.ControllingClan == clan.Clan))
                {
                    sb.AppendLine($"\t#{zone.Id.ToString("N0", actor)} - {zone.Name.ColourName()}");
                }
            }

            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        IEconomicZone ezone = long.TryParse(cmd, out long value)
            ? zones.FirstOrDefault(x => x.Id == value)
            : zones.FirstOrDefault(x => x.Name.EqualTo(cmd)) ?? zones.FirstOrDefault(x =>
                x.Name.StartsWith(cmd, StringComparison.InvariantCultureIgnoreCase));
        if (ezone == null)
        {
            actor.OutputHandler.Send(
                "None of the clans that you have economic zone privileges for have control of any such zone.");
            return;
        }

        ezone.BuildingCommandFromClanCommand(actor, ss);
    }

    private static void ClanReportDead(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you want to report someone as dead?");
            return;
        }

        IClan clan = GetTargetClan(actor, ss.PopSpeech());
        if (clan is null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        List<WitnessedClanMemberDeath> effects = actor.CombinedEffectsOfType<WitnessedClanMemberDeath>().Where(x => x.Clan == clan).ToList();
        List<IClanMembership> effectMemberships = effects
                                .SelectNotNull(x => x.ClanMember.ClanMemberships.FirstOrDefault(y => y.Clan == x.Clan))
                                .ToList();

        if (ss.IsFinished)
        {
            StringBuilder sb = new();
            sb.AppendLine("Who is it that you want to report dead?");

            if (effectMemberships.Any())
            {
                sb.AppendLine();
                sb.AppendLine("You have witnessed the death of the following clan members:");
                sb.AppendLine();
                foreach (IClanMembership member in effectMemberships)
                {
                    sb.AppendLine(
                        $"\t{member.PersonalName.GetName(NameStyle.FullWithNickname).ColourName()} [{member.Rank.Title(actor.Gameworld.TryGetCharacter(member.MemberId, true)).ColourValue()}]");
                }
            }

            actor.OutputHandler.Send(sb.ToString());
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        List<IClanMembership> members;
        if (actor.IsAdministrator())
        {
            members = clan.Memberships
                          .Where(x => !x.IsArchivedMembership)
                          .OrderByDescending(x => x.Rank.RankNumber)
                          .ThenBy(x => x.JoinDate)
                          .ToList();
        }
        else if (actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure))
        {
            members = clan.Memberships
                          .Where(x => !x.IsArchivedMembership)
                          .OrderByDescending(x => x.Rank.RankNumber)
                          .ThenBy(x => x.JoinDate)
                          .ToList();
        }
        else if (actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers))
        {
            members = clan.Memberships
                          .Where(x => !x.IsArchivedMembership && x.Rank.RankNumber <= actorMembership.Rank.RankNumber)
                          .OrderByDescending(x => x.Rank.RankNumber)
                          .ThenBy(x => x.JoinDate)
                          .ToList();
        }
        else
        {
            members = effectMemberships
                      .OrderByDescending(x => x.Rank.RankNumber)
                      .ThenBy(x => x.JoinDate)
                      .ToList();
        }

        IClanMembership targetMembership = members.GetByNameOrAbbreviation(ss.SafeRemainingArgument);
        if (targetMembership is null)
        {
            actor.OutputHandler.Send(
                $"You don't know anyone like that to report dead in the {clan.FullName.ColourName()} clan.");
            return;
        }

        ICharacter target = actor.Gameworld.TryGetCharacter(targetMembership.MemberId, true);
        if (targetMembership.IsArchivedMembership)
        {
            actor.OutputHandler.Send(
                $"{targetMembership.Name.ColourName()} has already been reported as dead in {clan.FullName.ColourName()}.");
            actor.RemoveAllEffects<WitnessedClanMemberDeath>(x => x.ClanMember == target, true);
            return;
        }

        if (!effectMemberships.Contains(targetMembership) && !actor.IsAdministrator() &&
            actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanReportDead) == false)
        {
            actor.OutputHandler.Send(
                "You do not know that person to be dead, nor are you permitted to report missing people dead in that clan.");
            return;
        }

        // Report to Discord
        actor.Gameworld.DiscordConnection.NotifyAdmins(
            $"**{actor.PersonalName.GetName(NameStyle.FullName)}** has reported **{target.PersonalName.GetName(NameStyle.FullName)}** as dead in the **{clan.FullName}** clan.");
        // Report to Admins
        actor.Gameworld.SystemMessage(
            new EmoteOutput(new Emote("$0 report|reports that $1 has died in the $2 clan.", actor, actor, target,
                new DummyPerceivable(clan.FullName, customColour: Telnet.Cyan))), true);
        // Report to Clan Members
        actor.OutputHandler.Handle(new FilteredEmoteOutput(
            new Emote("$0 report|reports that $1 has died in the $2 clan.", actor, actor, target,
                new DummyPerceivable(clan.FullName, customColour: Telnet.Cyan)),
            perc => perc is ICharacter ch && ch.ClanMemberships.Any(x => x.Clan == clan),
            flags: OutputFlags.SuppressObscured));
        // Write to a Board

        clan.RemoveMembership(targetMembership);
        actor.RemoveAllEffects<WitnessedClanMemberDeath>(x => x.ClanMember == target, true);
    }

    private static void ClanWithdrawNomination(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "You must either specify a clan and a position, or an ID number of an election for which you wish to withdraw your nomination.");
            return;
        }

        var firstArg = ss.PopSpeech();
        if (long.TryParse(firstArg, out var value))
        {
            var election = actor.Gameworld.Elections.Get(value);
            if (election == null || (!actor.IsAdministrator() && !actor.ClanMemberships.Any(x => x.Clan == election.Appointment.Clan)))
            {
                actor.OutputHandler.Send("There is no such election in any of your clans.");
                return;
            }

            election.Appointment.Clan.WithdrawNomination(actor, new StringStack(firstArg));
            return;
        }

        var clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(firstArg);
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which position within {clan.FullName.ColourName()} do you want to withdraw your nomination from?");
            return;
        }

        clan.WithdrawNomination(actor, ss);
    }

    private static void ClanNominate(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "You must either specify a clan and a position, or an ID number of an election for which you wish to nominate.");
            return;
        }

        var firstArg = ss.PopSpeech();
        if (long.TryParse(firstArg, out var value))
        {
            var election = actor.Gameworld.Elections.Get(value);
            if (election == null || (!actor.IsAdministrator() && !actor.ClanMemberships.Any(x => x.Clan == election.Appointment.Clan)))
            {
                actor.OutputHandler.Send("There is no such election in any of your clans.");
                return;
            }

            election.Appointment.Clan.Nominate(actor, new StringStack(firstArg));
            return;
        }

        var clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(firstArg);
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which position within {clan.FullName.ColourName()} do you want to nominate yourself for?");
            return;
        }

        clan.Nominate(actor, ss);
    }

    private static void ClanVote(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                "You must either specify a clan and a position, or an ID number of an election for which you wish to vote.");
            return;
        }

        var firstArg = ss.PopSpeech();
        if (long.TryParse(firstArg, out var value))
        {
            var election = actor.Gameworld.Elections.Get(value);
            if (election == null || (!actor.IsAdministrator() && !actor.ClanMemberships.Any(x => x.Clan == election.Appointment.Clan)))
            {
                actor.OutputHandler.Send("There is no such election in any of your clans.");
                return;
            }

            election.Appointment.Clan.Vote(actor, new StringStack($"{firstArg} {ss.RemainingArgument}".Trim()));
            return;
        }

        var clan = actor.ClanMemberships.Select(x => x.Clan).GetClan(firstArg);
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not a member of any such clan.");
            return;
        }

        if (ss.IsFinished)
        {
            actor.OutputHandler.Send(
                $"Which position within {clan.FullName.ColourName()} do you want to vote for?");
            return;
        }

        clan.Vote(actor, ss);
    }

    private static void ClanElections(ICharacter actor, StringStack ss)
    {
        List<IClan> clans =
            (actor.IsAdministrator() ? actor.Gameworld.Clans : actor.ClanMemberships.Select(x => x.Clan)).ToList();
        if (!ss.IsFinished)
        {
            string text = ss.SafeRemainingArgument;
            IClan clan = clans.FirstOrDefault(x => x.FullName.EqualTo(text)) ??
                       clans.FirstOrDefault(x => x.Alias.EqualTo(text)) ??
                       clans.FirstOrDefault(x =>
                           x.Alias.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)) ??
                       clans.FirstOrDefault(x =>
                           x.FullName.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
            if (clan == null)
            {
                actor.OutputHandler.Send("There is no such clan that you can show elections for.");
                return;
            }

            string description = clan.DescribeElections(actor);
            actor.OutputHandler.Send(string.IsNullOrWhiteSpace(description)
                ? $"There are no elections in {clan.FullName.ColourName()}."
                : description);
            return;
        }

        if (!actor.IsAdministrator())
        {
            clans = clans.Where(x =>
                actor.ClanMemberships.First(y => y.Clan == x).NetPrivileges
                     .HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)).ToList();
        }

        StringBuilder sb = new();
        foreach (IClan clan in clans)
        {
            string description = clan.DescribeElections(actor);
            if (string.IsNullOrWhiteSpace(description))
            {
                continue;
            }

            sb.Append(description);
        }

        if (sb.Length == 0)
        {
            sb.AppendLine("There are no elections in any of your clans.");
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanElection(ICharacter actor, StringStack ss)
    {
        switch (ss.PopSpeech().ToLowerInvariant())
        {
            case "view":
                ClanElectionView(actor, ss);
                return;
            case "history":
                ClanElectionHistory(actor, ss);
                return;
        }

        actor.OutputHandler.Send(
            $"The valid syntaxes for this command are {"clan election view <id>".ColourCommand()}, {"clan election history <id>".ColourCommand()}, and {"clan election history <clan> [<position>]".ColourCommand()}.");
    }

    private static void ClanElectionView(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which election would you like to view? You must supply an ID number.");
            return;
        }

        if (!long.TryParse(ss.PopSpeech(), out long value))
        {
            actor.OutputHandler.Send("You must supply a valid ID number for the election.");
            return;
        }

        IElection election = actor.Gameworld.Elections.Get(value);
        if (election == null || (!actor.IsAdministrator() && !actor.ClanMemberships.Any(x =>
                x.Clan == election.Appointment.Clan &&
                x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders))))
        {
            actor.OutputHandler.Send("There is no such election.");
            return;
        }

        actor.OutputHandler.Send(election.Show(actor));
    }

    private static void ClanElectionHistory(ICharacter actor, StringStack ss)
    {
        if (ss.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to view election history for?");
            return;
        }

        var targetText = ss.PopSpeech();
        if (long.TryParse(targetText, out var value))
        {
            var election = actor.Gameworld.Elections.Get(value);
            if (election == null || (!actor.IsAdministrator() && !actor.ClanMemberships.Any(x =>
                    x.Clan == election.Appointment.Clan &&
                    x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders))))
            {
                actor.OutputHandler.Send("There is no such election.");
                return;
            }

            election.Appointment.Clan.ShowElectionHistory(actor, new StringStack(election.Appointment.Id.ToString()));
            return;
        }

        IClan clan = GetTargetClan(actor, targetText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator()
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.ShowElectionHistory(actor, ss);
    }

    [PlayerCommand("Clans", "clans")]
    [HelpInfo("clan",
        "This command is used to view clans that you are a member of, or template clans. The syntax is either CLANS or CLANS TEMPLATES",
        AutoHelp.HelpArg)]
    protected static void Clans(ICharacter actor, string command)
    {
        StringStack ss = new(command.RemoveFirstWord());
        if (ss.PopSpeech().EqualTo("template") || ss.Last.EqualTo("templates"))
        {
            ClansTemplates(actor);
            return;
        }

        if (!actor.ClanMemberships.Any())
        {
            actor.OutputHandler.Send("You are not affiliated with any clans.");
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("You are a member of the following clans:");
        sb.AppendLine();
        foreach (IClanMembership clan in actor.ClanMemberships)
        {
            sb.AppendLine(
                $"{clan.Clan.FullName.TitleCase().Colour(Telnet.Cyan)} [alias: {clan.Clan.Alias.ToLowerInvariant().ColourValue()}]");
            sb.AppendLine(
                $"You hold the rank of {clan.Rank.Title(actor).TitleCase().Colour(Telnet.Green)}{(string.IsNullOrEmpty(clan.Rank.RankPath) ? "" : $", which is {clan.Rank.RankPath.TitleCase().A_An(colour: Telnet.Green)} rank")}.");
            if (clan.Paygrade != null)
            {
                sb.AppendLine(
                    $"You hold the {clan.Paygrade.Name.TitleCase().Colour(Telnet.Green)} pay grade, and are paid {clan.Paygrade.PayCurrency.Describe(clan.Paygrade.PayAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} {clan.Clan.PayInterval.Describe(clan.Clan.Calendar).Colour(Telnet.Green)}.");
            }

            foreach (IAppointment appointment in clan.Appointments)
            {
                sb.AppendLine(
                    $"You have been appointed to the position of {appointment.Title(actor).TitleCase().Colour(Telnet.Green)}{(appointment.Paygrade != null ? $", which is paid {appointment.Paygrade.PayCurrency.Describe(appointment.Paygrade.PayAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} extra" : "")}.");
            }

            if (clan.ManagerId != null)
            {
                IClanMembership managerMembership = clan.Clan.Memberships.FirstOrDefault(x => x.MemberId == clan.ManagerId);
                if (managerMembership != null)
                {
                    sb.AppendLine(
                        $"Your manager is {managerMembership.PersonalName.GetName(NameStyle.FullName)}.");
                }
            }

            sb.AppendLine(
                $"You joined this clan on {clan.Clan.Calendar.DisplayDate(clan.JoinDate, CalendarDisplayMode.Long).Colour(Telnet.Green)}");
            sb.AppendLine(
                $"This clan knows you by the name {clan.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green)}.");
            if (clan.BackPayDiciontary.Sum(x => x.Value) > 0)
            {
                sb.AppendLineFormat(actor, "This clan owes you {0} in backpay.",
                    clan.BackPayDiciontary.Select(
                            x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green))
                        .ListToString());
            }
            else
            {
                sb.AppendLine("This clan does not owe you any backpay.");
            }

            if ((actor.IsAdministrator() || clan.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewTreasury)) &&
                clan.Clan.TreasuryCells.Any())
            {
                IEnumerable<ICurrencyPile> currencyItems = clan.Clan.TreasuryCells.SelectMany(x => x.GameItems)
                                        .SelectMany(x => x.RecursiveGetItems<ICurrencyPile>());
                List<(ICurrency Currency, decimal Total)> currencyBalances = currencyItems.GroupBy(x => x.Currency).Select(x =>
                    (Currency: x.Key, Total: x.Sum(y => y.Coins.Sum(z => z.Item2 * z.Item1.Value)))).ToList();
                sb.AppendLine(
                    $"The clan's treasury balance is currently {currencyBalances.Select(x => x.Currency.Describe(x.Total, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)).ListToString()}.");
            }

            sb.AppendLine();
        }

        actor.OutputHandler.Send(sb.ToString(), false);
    }

    private static void ClansTemplates(ICharacter actor)
    {
        actor.Send(
            $"You can employ the following clan templates when creating a clan. Use {"clan view <alias>".Colour(Telnet.Yellow)} to review them in more detail.\n");
        actor.Send(StringUtilities.GetTextTable(
            from clan in actor.Gameworld.Clans
            where clan.IsTemplate
            select new[]
                { clan.Alias.ToLowerInvariant(), clan.FullName.TitleCase(), clan.Description.ProperSentences() },
            new[] { "Alias", "Name", "Description" },
            actor.LineFormatLength,
            colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode, truncatableColumnIndex: 2
        ));
    }

    [PlayerCommand("Payday", "payday")]
    [RequiredCharacterState(CharacterState.Conscious)]
    [NoCombatCommand]
    [NoHideCommand]
    [NoMovementCommand]
    [HelpInfo("Payday",
        "This command is used to check which paydays you have, how much backpay you are owed, and to collect your pay when in the presence of your clan's paymaster. The syntax is simply PAYDAY for all 3 scenarios.",
        AutoHelp.HelpArg)]
    protected static void Payday(ICharacter actor, string command)
    {
        List<IClanMembership> memberships = actor.ClanMemberships.Where(x => x.Clan.AdministrationCells.Contains(actor.Location))
                               .ToList();

        if (!memberships.Any() || memberships.All(x => x.BackPayDiciontary.Sum(y => y.Value) <= 0))
        {
            // Not in any clan's administrative cells, just show what paydays they have
            if (!actor.ClanMemberships.Any())
            {
                actor.Send("You are not in any clans.");
                return;
            }

            if (actor.ClanMemberships.All(x => x.Paygrade == null && x.Appointments.All(y => y.Paygrade == null)))
            {
                actor.Send("None of your clans pay you anything.");
                return;
            }

            StringBuilder sb = new();
            sb.AppendLine("You have the following clan paydays:");
            foreach (
                IClanMembership clan in
                actor.ClanMemberships.Where(
                    x => x.Paygrade != null || x.Appointments.Any(y => y.Paygrade != null)))
            {
                sb.AppendLine();
                Dictionary<ICurrency, decimal> pays = new();
                if (clan.Paygrade != null)
                {
                    pays[clan.Paygrade.PayCurrency] = clan.Paygrade.PayAmount;
                }

                foreach (IAppointment appointment in clan.Appointments.Where(x => x.Paygrade != null))
                {
                    if (!pays.ContainsKey(appointment.Paygrade.PayCurrency))
                    {
                        pays[appointment.Paygrade.PayCurrency] = 0;
                    }

                    pays[appointment.Paygrade.PayCurrency] += appointment.Paygrade.PayAmount;
                }

                sb.AppendLine(string.Format(actor, @"{0} pays you {1} {2}. 
Your next payday is {3}.
{4}
{5}",
                        clan.Clan.FullName.TitleCase().Colour(Telnet.Cyan),
                        pays.Select(
                                x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.Short)
                                      .Colour(Telnet.Green))
                            .ListToString(),
                        clan.Clan.PayInterval.Describe(clan.Clan.Calendar),
                        clan.Clan.Calendar
                            .DisplayDate(clan.Clan.NextPay.Date, CalendarDisplayMode.Long)
                            .Colour(Telnet.Green), clan.BackPayDiciontary.Any(x => x.Value > 0)
                            ? $"They owe you {clan.BackPayDiciontary.Select(x => x.Key.Describe(x.Value, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)).ListToString()} in backpay."
                            : "They do not owe you any backpay.", clan.Clan.AdministrationCells.Any()
                            ? $"You can collect your pay at {clan.Clan.AdministrationCells.Select(x => x.HowSeen(actor, true, flags: PerceiveIgnoreFlags.IgnoreLayers | PerceiveIgnoreFlags.IgnoreCanSee)).ListToString(conjunction: "or ")}"
                            : "You do not know where you would go to collect your payday.")
                // TODO - not physical methods of payment e.g. bank transfer
                );
            }

            actor.Send(sb.ToString());
            return;
        }

        bool paymentMade = false;
        foreach (IClanMembership membership in memberships)
        {
            if (!membership.BackPayDiciontary.Any(x => x.Value > 0))
            {
                continue;
            }

            if (membership.Clan.Paymaster != null)
            {
                if (!actor.Location.LayerCharacters(actor.RoomLayer).Contains(membership.Clan.Paymaster))
                {
                    actor.Send(
                        $"The paymaster for {membership.Clan.FullName.Colour(Telnet.Green)} is not in, and so you cannot collect your pay.");
                    continue;
                }

                if (membership.Clan.Paymaster.State != CharacterState.Awake)
                {
                    actor.Send(
                        $"{membership.Clan.Paymaster.HowSeen(actor, true)} cannot pay you in {membership.Clan.FullName.Colour(Telnet.Green)} because {membership.Clan.Paymaster.ApparentGender(actor).Subjective()} is not conscious.");
                    continue;
                }

                if (membership.Clan.OnPayProg == null)
                {
                    actor.Send(
                        $"You cannot collect your pay in {membership.Clan.FullName.Colour(Telnet.Green)} because the clan has not been correctly configured. Contact an administrator.");
                    continue;
                }
            }
            else if (membership.Clan.PaymasterItemProto != null)
            {
                if (actor.Location.LayerGameItems(actor.RoomLayer)
                         .All(x => x.Prototype != membership.Clan.PaymasterItemProto))
                {
                    actor.Send(
                        $"You cannot collect your pay in {membership.Clan.FullName.Colour(Telnet.Green)} because the clan has not been correctly configured. Contact an administrator.");
                    continue;
                }
            }

            List<ICurrencyPile> currencyPiles =
                membership.Clan.TreasuryCells.SelectMany(
                    x => x.GameItems.SelectMany(y => y.RecursiveGetItems<ICurrencyPile>())).ToList();

            Dictionary<ICurrency, Dictionary<ICurrencyPile, Dictionary<ICoin, int>>> coinsToTake = new();
            Dictionary<ICurrency, Dictionary<ICoin, int>> coinsToLoad = new();
            Dictionary<ICurrency, Dictionary<ICoin, int>> changeToLoad = new();
            foreach (KeyValuePair<ICurrency, decimal> pay in membership.BackPayDiciontary.Where(x => x.Value > 0).ToList())
            {
                Dictionary<ICurrencyPile, Dictionary<ICoin, int>> coins = pay.Key.FindCurrency(currencyPiles, pay.Value);
                decimal amount = coins.Sum(x => x.Value.Sum(y => y.Value * y.Key.Value));
                if (amount < pay.Value)
                {
                    var floatAmount = pay.Value - amount;
                    var bankAccount = membership.Clan.ClanBankAccount?.Currency == pay.Key
                        ? membership.Clan.ClanBankAccount
                        : null;
                    if (!VirtualCashLedger.Debit(membership.Clan, pay.Key, floatAmount, actor, membership.Clan,
                            "PayrollCash", "Payroll float", bankAccount, null, out var error))
                    {
                        actor.Send(
                            "{0} does not have enough money to pay what they owe you in the {1} currency at the moment. {2}",
                            membership.Clan.FullName.Colour(Telnet.Green), pay.Key.Name.Colour(Telnet.Green), error);
                        continue;
                    }

                    ICurrencyPile newPile = CurrencyGameItemComponentProto
                                  .CreateNewCurrencyPile(pay.Key,
                                      pay.Key.FindCoinsForAmount(floatAmount, out _))
                                  .GetItemType<ICurrencyPile>();
                    coins.Add(newPile, newPile.Coins.ToDictionary(x => x.Item1, x => x.Item2));
                }

                paymentMade = true;

                if (!coinsToTake.ContainsKey(pay.Key))
                {
                    coinsToTake[pay.Key] = new Dictionary<ICurrencyPile, Dictionary<ICoin, int>>();
                }

                foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> pile in coins)
                {
                    coinsToTake[pay.Key].Add(pile.Key, pile.Value);
                }

                if (!coinsToLoad.ContainsKey(pay.Key))
                {
                    coinsToLoad[pay.Key] = new Dictionary<ICoin, int>();
                }

                if (amount > pay.Value)
                {
                    Dictionary<ICoin, int> changeCoins = pay.Key.FindCoinsForAmount(amount - pay.Value, out _);
                    if (changeCoins.Any())
                    {
                        if (!changeToLoad.ContainsKey(pay.Key))
                        {
                            changeToLoad[pay.Key] = new Dictionary<ICoin, int>();
                        }

                        foreach (KeyValuePair<ICoin, int> coin in changeCoins)
                        {
                            if (changeToLoad[pay.Key].ContainsKey(coin.Key))
                            {
                                changeToLoad[pay.Key][coin.Key] += coin.Value;
                            }
                            else
                            {
                                changeToLoad[pay.Key][coin.Key] = coin.Value;
                            }
                        }
                    }
                }

                Dictionary<ICoin, int> payCoins = pay.Key.FindCoinsForAmount(pay.Value, out _);
                foreach (KeyValuePair<ICoin, int> coin in payCoins)
                {
                    if (coinsToLoad[pay.Key].ContainsKey(coin.Key))
                    {
                        coinsToLoad[pay.Key][coin.Key] += coin.Value;
                    }
                    else
                    {
                        coinsToLoad[pay.Key][coin.Key] = coin.Value;
                    }
                }
            }

            if (membership.Clan.OnPayProg != null)
            {
                membership.Clan.OnPayProg.Execute(actor,
                    (IPerceiver)membership.Clan.Paymaster ??
                    actor.Location.LayerGameItems(actor.RoomLayer)
                         .FirstOrDefault(x => x.Prototype == membership.Clan.PaymasterItemProto));
            }
            else
            {
                actor.OutputHandler.Handle(new EmoteOutput(new Emote(
                    $"@ collect|collects &0's pay for {membership.Clan.FullName.Colour(Telnet.Green)}.", actor,
                    actor)));
            }

            foreach (KeyValuePair<ICurrency, Dictionary<ICurrencyPile, Dictionary<ICoin, int>>> currency in coinsToTake)
            {
                foreach (KeyValuePair<ICurrencyPile, Dictionary<ICoin, int>> item in currency.Value)
                {
                    if (!item.Key.RemoveCoins(item.Value.Select(x => Tuple.Create(x.Key, x.Value))))
                    {
                        item.Key.Parent.Delete();
                    }
                }
            }

            foreach (KeyValuePair<ICurrency, Dictionary<ICoin, int>> currency in changeToLoad)
            {
                if (!currency.Value.Any())
                {
                    continue;
                }

                IGameItem changeItem =
                    CurrencyGameItemComponentProto.CreateNewCurrencyPile(
                        currency.Key, currency.Value.Select(x => Tuple.Create(x.Key, x.Value)));
                membership.Clan.TreasuryCells.First().Insert(changeItem);
            }

            foreach (KeyValuePair<ICurrency, Dictionary<ICoin, int>> currency in coinsToLoad)
            {
                IEnumerable<Tuple<ICoin, int>> payAmount = currency.Value.Select(x => Tuple.Create(x.Key, x.Value)).ToList();
                decimal paidAmount = payAmount.Sum(x => x.Item1.Value * x.Item2);
                membership.BackPayDiciontary[currency.Key] = 0;
                membership.Changed = true;
                ClanPayrollHistoryEntry.Create(membership, currency.Key, -paidAmount,
                    ClanPayrollHistoryType.PayCollected, "Payroll collected", actor: actor);
                IGameItem newItem = CurrencyGameItemComponentProto.CreateNewCurrencyPile(currency.Key, payAmount);
                actor.Gameworld.Add(newItem);
                actor.OutputHandler.Send(
                    $"You are paid {newItem.HowSeen(actor)} ({currency.Key.Describe(paidAmount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}).");
                if (actor.Body.CanGet(newItem, 0))
                {
                    actor.Body.Get(newItem, silent: true);
                }
                else
                {
                    newItem.RoomLayer = actor.RoomLayer;
                    actor.Location.Insert(newItem, true);
                    newItem.SetPosition(PositionUndefined.Instance, PositionModifier.None, actor, null);
                    actor.Send("You cannot hold {0}, so you set it down.", newItem.HowSeen(actor));
                }
            }
        }

        if (!paymentMade)
        {
            actor.OutputHandler.Send("You were unable to collect any of your paydays.");
        }
    }

    #region Clan Sub-Commands

    private static void ClanBackpay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Whose backpay do you want to alter?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to alter their backpay?");
            return;
        }

        string clanText = command.PopSpeech();
        List<IClan> clans = (actor.IsAdministrator(PermissionLevel.Admin)
            ? actor.Gameworld.Clans
            : actor.ClanMemberships.Select(x => x.Clan)).ToList();

        IClan clan =
            clans.FirstOrDefault(x => x.FullName.Equals(clanText, StringComparison.InvariantCultureIgnoreCase)) ??
            clans.FirstOrDefault(x => x.Alias.Equals(clanText, StringComparison.InvariantCultureIgnoreCase));
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanGiveBackpay))
        {
            actor.OutputHandler.Send("You are not allowed to alter the backpay of people in that clan.");
            return;
        }

        ICharacter targetActor = actor.TargetActor(targetText);
        IClanMembership targetMembership = targetActor != null
            ? targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            : clan.Memberships.FirstOrDefault(
                x =>
                    x.PersonalName.GetName(NameStyle.FullName)
                     .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for whom you can alter the backpay.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "How much backpay should they be awarded? Hint: Use negative numbers for taking away pay.");
            return;
        }

        decimal amount = actor.Currency.GetBaseCurrency(command.SafeRemainingArgument, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send(
                "That is not a valid amount of currency in your currently selected transaction currency.");
            return;
        }

        if (!targetMembership.BackPayDiciontary.ContainsKey(actor.Currency))
        {
            if (amount < 0)
            {
                actor.OutputHandler.Send(
                    $"{targetMembership.PersonalName.GetName(NameStyle.GivenOnly)} isn't currently owed any backpay in that currency.");
                return;
            }

            targetMembership.BackPayDiciontary[actor.Currency] = 0;
        }

        targetMembership.BackPayDiciontary[actor.Currency] += amount;
        targetMembership.Changed = true;
        ClanPayrollHistoryEntry.Create(targetMembership, actor.Currency, amount,
            ClanPayrollHistoryType.ManualAdjustment,
            $"Manual backpay adjustment by {actor.PersonalName.GetName(NameStyle.FullName)}", actor: actor);
        actor.OutputHandler.Send(
            $"You change the backpay owing to {targetMembership.PersonalName.GetName(NameStyle.FullName)} by {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)}, with their current backpay sitting at {actor.Currency.Describe(targetMembership.BackPayDiciontary[actor.Currency], CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)}.");
    }

    private static void ClanInvite(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to invite to a clan?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Into which clan do you wish to invite them?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanInduct))
        {
            actor.OutputHandler.Send("You are not allowed to induct new members into that clan.");
            return;
        }

        ICharacter targetActor = actor.TargetActor(targetText);
        if (targetActor == null)
        {
            actor.OutputHandler.Send("You do not see that person to invite into your clan.");
            return;
        }

        IClanMembership targetMembership = targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (targetMembership is { IsArchivedMembership: false })
        {
            actor.OutputHandler.Handle(
                new EmoteOutput(new Emote("$0 is already a member of " + clan.FullName.TitleCase() + ".", actor,
                    targetActor)), OutputRange.Personal);
            return;
        }

        string rankPathText = command.PopSpeech();
        if (!string.IsNullOrEmpty(rankPathText) &&
            !clan.Ranks.Any(
                x => x?.RankPath?.Equals(rankPathText, StringComparison.InvariantCultureIgnoreCase) ?? false))
        {
            actor.OutputHandler.Send("There is no such rank path to induct them into.");
            return;
        }

        IRank rank;
        if (string.IsNullOrEmpty(rankPathText))
        {
            rank = clan.Ranks.OrderBy(x => x.RankNumber).FirstOrDefault();
        }
        else
        {
            rank = clan.Ranks.OrderBy(x => x.RankNumber).FirstOrDefault(x => x.RankPath?.EqualTo(rankPathText) == true);
        }

        if (rank == null)
        {
            actor.OutputHandler.Send("There are no valid ranks to induct them into.");
            return;
        }

        if (actorMembership != null && rank.RankNumber > actorMembership.Rank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot induct somebody into a higher rank than yourself.");
            return;
        }

        if (
            targetActor.EffectsOfType<IProposalEffect>()
                       .SelectNotNull(x => x.Proposal as ClanInviteProposal)
                       .Any(x => x.Clan == clan))
        {
            actor.Send("{0} is already considering an offer from that clan.", targetActor.HowSeen(actor, true));
            return;
        }

        targetActor.AddEffect(new Accept(targetActor, new ClanInviteProposal
        {
            Clan = clan,
            Rank = rank,
            Recruiter = actor,
            Recruit = targetActor
        }), TimeSpan.FromMinutes(2));
        targetActor.OutputHandler.Send(
            $"{actor.HowSeen(targetActor, true)} has invited you to join {clan.FullName.TitleCase().Colour(Telnet.Green)} at the rank of {rank.Title(targetActor).TitleCase().Colour(Telnet.Green)}. \nType {"accept".Colour(Telnet.Yellow)} to accept, or {"decline".Colour(Telnet.Yellow)} to decline.");
        actor.OutputHandler.Send(
            $"You invite {targetActor.HowSeen(actor)} to join {clan.FullName.TitleCase().Colour(Telnet.Green)} at the rank of {rank.Title(targetActor).TitleCase().Colour(Telnet.Green)}.");
    }

    private static void ClanCastout(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to cast out from a clan?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("From which clan do you wish to cast them out?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCastout))
        {
            actor.OutputHandler.Send("You are not allowed to cast people out of that clan.");
            return;
        }

        IClanMembership targetMembership = null;
        ICharacter targetActor = null;
        IClanMembership membership =
            clan.Memberships.FirstOrDefault(x =>
                !x.IsArchivedMembership && x.PersonalName.GetName(NameStyle.FullName).EqualTo(targetText));
        if (membership is not null)
        {
            targetMembership = membership;
        }
        else
        {
            targetActor = actor.TargetActor(targetText);
            if (targetActor == null)
            {
                actor.OutputHandler.Send("You do not see that person to cast out from your clan.");
                return;
            }

            targetMembership = targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
            if (targetMembership is null)
            {
                actor.OutputHandler.Send(
                    new EmoteOutput(new Emote("$0 is not a member of " + clan.FullName.TitleCase() + ".", actor,
                        targetActor)));
                return;
            }
        }

        if (actorMembership != null && targetMembership.Rank.RankNumber > actorMembership.Rank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot cast out somebody of a higher rank than yourself.");
            return;
        }

        List<IActiveJob> jobs = actor.Gameworld.ActiveJobs
                        .Where(x =>
                            !x.IsJobComplete &&
                            x.Character.Id != targetMembership.MemberId &&
                            x.Listing.ClanMembership == clan
                        ).ToList();
        if (jobs.Any())
        {
            actor.OutputHandler.Send(
                $"{targetMembership.MemberCharacter.HowSeen(actor, true)} cannot be dismissed from this clan as they hold their clan membership by virtue of the job{(jobs.Count == 1 ? "" : "s")} {jobs.Select(x => x.Name.ColourValue()).ListToString()}.");
            return;
        }

        actor.Gameworld.SystemMessage(
            $"{actor.PersonalName.GetName(NameStyle.FullWithNickname)} casts {targetMembership.PersonalName.GetName(NameStyle.FullWithNickname)} out of {clan.FullName.TitleCase()}.",
            true);

        if (targetActor != null)
        {
            actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
                    $"@ cast|casts $0 out of {clan.FullName.TitleCase().Colour(Telnet.Green)}.",
                    actor, targetActor),
                perceiver =>
                {
                    return perceiver is ICharacter pChar &&
                           (pChar.ClanMemberships.Any(
                                x => x.Clan == clan) ||
                            pChar.IsAdministrator());
                }
            ));
        }

        clan.RemoveMembership(targetMembership);
    }

    private static void ClanPromote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to promote?");
            return;
        }

        string targetText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to promote them?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanPromote))
        {
            actor.OutputHandler.Send("You are not allowed to promote people in that clan.");
            return;
        }

        ICharacter targetActor = actor.TargetActor(targetText);
        IClanMembership targetMembership = targetActor != null
            ? targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            : clan.Memberships.FirstOrDefault(
                x => !x.IsArchivedMembership &&
                     x.PersonalName.GetName(NameStyle.FullName)
                      .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for you to promote.");
            return;
        }

        IRank newRank = null;
        if (command.IsFinished)
        {
            newRank =
                clan.Ranks.OrderBy(x => x.RankNumber)
                    .FirstOrDefault(
                        x =>
                            x.RankNumber > targetMembership.Rank.RankNumber &&
                            x.RankPath == targetMembership.Rank.RankPath);
            if (newRank == null)
            {
                if (clan.Ranks.OrderBy(x => x.RankNumber).Any(x => x.RankNumber > targetMembership.Rank.RankNumber))
                {
                    actor.OutputHandler.Send("There are no valid ranks on the same rank path to promote them to.");
                    return;
                }

                actor.OutputHandler.Send("There is no valid rank to promote them to.");
                return;
            }
        }
        else
        {
            string rankText = command.PopSpeech();
            newRank = clan.Ranks.FirstOrDefault(x => x.Name.EqualTo(rankText) ||
                                                     x.Abbreviations.Any(y => y.EqualTo(rankText)) ||
                                                     x.Titles.Any(y => y.EqualTo(rankText)));
            if (newRank == null)
            {
                actor.OutputHandler.Send("There is no such rank to promote them to.");
                return;
            }
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            actorMembership.Rank.RankNumber < newRank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot promote them to a higher rank than yourself.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            actorMembership.Rank.RankNumber == newRank.RankNumber &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanPromoteToOwnRank))
        {
            actor.OutputHandler.Send("You do not have permission to promote people to the same rank as yourself.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !newRank.RankPath.EqualTo(targetMembership.Rank.RankPath) &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanChangeRankPath)
           )
        {
            actor.OutputHandler.Send(
                "You do not have permission to change the rank path of a member of your organisation.");
            return;
        }


        clan.SetRank(targetMembership, newRank);
        clan.SetPaygrade(targetMembership, newRank.Paygrades.FirstOrDefault());
        if (targetActor != null)
        {
            actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
                    $"@ promote|promotes $0 to the rank of {newRank.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.",
                    actor, targetActor),
                perceiver =>
                {
                    return perceiver is ICharacter pChar &&
                           (pChar.ClanMemberships
                                 .Any(x => x.Clan == clan) ||
                            pChar
                                .PermissionLevel >=
                            PermissionLevel.JuniorAdmin);
                }
            ));
        }
        else
        {
            actor.Send("You promote {0} to the rank of {1} in {2}.",
                targetMembership.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green),
                newRank.Name.TitleCase().Colour(Telnet.Green), clan.FullName.TitleCase().Colour(Telnet.Green));
        }
    }

    private static void ClanDemote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to demote?");
            return;
        }

        string targetText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to demote them?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanDemote))
        {
            actor.OutputHandler.Send("You are not allowed to demote people in that clan.");
            return;
        }

        ICharacter targetActor = actor.TargetActor(targetText);
        IClanMembership targetMembership = targetActor != null
            ? targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            : clan.Memberships.FirstOrDefault(
                x => !x.IsArchivedMembership &&
                     x.PersonalName.GetName(NameStyle.FullName)
                      .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for you to demote.");
            return;
        }

        IRank newRank = null;
        if (command.IsFinished)
        {
            newRank =
                clan.Ranks.OrderBy(x => x.RankNumber)
                    .LastOrDefault(
                        x =>
                            x.RankNumber < targetMembership.Rank.RankNumber &&
                            x.RankPath == targetMembership.Rank.RankPath);
            if (newRank == null)
            {
                if (clan.Ranks.OrderBy(x => x.RankNumber).Any(x => x.RankNumber > targetMembership.Rank.RankNumber))
                {
                    actor.OutputHandler.Send("There are no valid ranks on the same rank path to demote them to.");
                    return;
                }

                actor.OutputHandler.Send("There is no valid rank to demote them to.");
                return;
            }
        }
        else
        {
            string rankText = command.PopSpeech();
            newRank = clan.Ranks.FirstOrDefault(x => x.Name.EqualTo(rankText) ||
                                                     x.Abbreviations.Any(y => y.EqualTo(rankText)) ||
                                                     x.Titles.Any(y => y.EqualTo(rankText)));
            if (newRank == null)
            {
                actor.OutputHandler.Send("There is no such rank to demote them to.");
                return;
            }
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            actorMembership.Rank.RankNumber < targetMembership.Rank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot demote people of a higher rank than yourself.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            actorMembership.Rank.RankNumber == targetMembership.Rank.RankNumber &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanDemoteOwnRank))
        {
            actor.OutputHandler.Send("You cannot demote people of the same rank as yourself.");
            return;
        }

        if (targetMembership.Rank.RankNumber <= newRank.RankNumber)
        {
            actor.OutputHandler.Send(
                "You cannot demote people to a higher or equal rank to what they already hold.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !newRank.RankPath.EqualTo(targetMembership.Rank.RankPath) &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanChangeRankPath)
           )
        {
            actor.OutputHandler.Send(
                "You do not have permission to change the rank path of a member of your organisation.");
            return;
        }

        List<IActiveJob> jobs = actor.Gameworld.ActiveJobs
                        .Where(x =>
                            !x.IsJobComplete &&
                            x.Character.Id != targetMembership.MemberId &&
                            x.Listing.ClanMembership == clan &&
                            x.Listing.ClanRank is not null &&
                            x.Listing.ClanRank.RankNumber > newRank.RankNumber
                        ).ToList();
        if (jobs.Any())
        {
            actor.OutputHandler.Send(
                $"{targetMembership.MemberCharacter.HowSeen(actor, true)} cannot be demoted any further as they hold their clan rank by virtue of the job{(jobs.Count == 1 ? "" : "s")} {jobs.Select(x => x.Name.ColourValue()).ListToString()}.");
            return;
        }

        clan.SetRank(targetMembership, newRank);
        clan.SetPaygrade(targetMembership, newRank.Paygrades.FirstOrDefault());
        if (targetActor != null)
        {
            actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
                    $"@ demote|demotes $0 to the rank of {newRank.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.",
                    actor, targetActor),
                perceiver =>
                {
                    return perceiver is ICharacter pChar &&
                           (pChar.ClanMemberships.Any(
                                x => x.Clan == clan) ||
                            pChar.PermissionLevel >=
                            PermissionLevel.JuniorAdmin);
                }
            ));
        }
        else
        {
            actor.Send("You demote {0} to the rank of {1} in {2}.",
                targetMembership.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green),
                newRank.Name.TitleCase().Colour(Telnet.Green), clan.FullName.TitleCase().Colour(Telnet.Green));
        }
    }

    private static void ClanPaygradePromote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Whose paygrade do you wish to increase?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to increase their paygrade?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.PermissionLevel >= PermissionLevel.Admin
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanIncreasePaygrade))
        {
            actor.OutputHandler.Send("You are not allowed to increase paygrades in that clan.");
            return;
        }

        ICharacter targetActor = actor.TargetActor(targetText);
        IClanMembership targetMembership = targetActor != null
            ? targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)
            : clan.Memberships.FirstOrDefault(
                x => !x.IsArchivedMembership &&
                     x.PersonalName.GetName(NameStyle.FullName)
                      .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for you to increase the paygrade.");
            return;
        }

        if (!targetMembership.Rank.Paygrades.Any())
        {
            actor.Send("The rank of {0} does not have any associated paygrades.",
                targetMembership.Rank.Name.TitleCase().Colour(Telnet.Green));
            return;
        }

        if (targetMembership.Rank.Paygrades.Last() == targetMembership.Paygrade)
        {
            actor.OutputHandler.Send("They are already at the maximum paygrade for their rank.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) &&
            targetMembership.Rank.RankNumber >= actorMembership.Rank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot increase the paygrade of members at your own rank or higher.");
            return;
        }

        IPaygrade newPaygrade =
            targetMembership.Rank.Paygrades.ElementAtOrDefault(
                targetMembership.Rank.Paygrades.IndexOf(targetMembership.Paygrade) + 1);
        if (newPaygrade == null)
        {
            actor.OutputHandler.Send("There is no valid paygrade to give to them.");
            return;
        }

        clan.SetPaygrade(targetMembership, newPaygrade);
        if (targetActor != null)
        {
            actor.OutputHandler.Send(
                $"You increase {targetActor.HowSeen(actor, type: DescriptionType.Possessive)} paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newPaygrade.Name.TitleCase().Colour(Telnet.Green)}.");
            targetActor.OutputHandler.Send(
                $"{actor.HowSeen(targetActor, true)} increases your paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newPaygrade.Name.TitleCase().Colour(Telnet.Green)}.");
        }
        else
        {
            actor.Send("You increase the paygrade of {0} in {2} to {1}.",
                targetMembership.PersonalName.GetName(NameStyle.FullName),
                newPaygrade.Name.TitleCase().Colour(Telnet.Green), clan.FullName.TitleCase().Colour(Telnet.Green));
        }
    }

    private static void ClanPaygradeDemote(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Whose paygrade do you wish to decrease?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to decrease their paygrade?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanDecreasePaygrade))
        {
            actor.OutputHandler.Send("You are not allowed to decrease paygrades in that clan.");
            return;
        }

        IClanMembership targetMembership;
        ICharacter targetActor = actor.TargetActor(targetText);
        if (targetActor != null)
        {
            targetMembership = targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        }
        else
        {
            targetMembership =
                clan.Memberships.FirstOrDefault(
                    x =>
                        !x.IsArchivedMembership &&
                        x.PersonalName.GetName(NameStyle.FullName)
                         .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
        }

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for you to decrease the paygrade.");
            return;
        }

        if (!targetMembership.Rank.Paygrades.Any())
        {
            actor.Send("The rank of {0} does not have any associated paygrades.",
                targetMembership.Rank.Name.TitleCase().Colour(Telnet.Green));
            return;
        }

        if (targetMembership.Rank.Paygrades.First() == targetMembership.Paygrade)
        {
            actor.OutputHandler.Send("They are already at the minimum paygrade for their rank.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) &&
            targetMembership.Rank.RankNumber >= actorMembership.Rank.RankNumber)
        {
            actor.OutputHandler.Send("You cannot decrease the paygrade of members at your own rank or higher.");
            return;
        }

        IPaygrade newPaygrade =
            targetMembership.Rank.Paygrades.ElementAtOrDefault(
                targetMembership.Rank.Paygrades.IndexOf(targetMembership.Paygrade) - 1);
        if (newPaygrade == null)
        {
            actor.OutputHandler.Send("There is no valid paygrade to give to them.");
            return;
        }

        List<IActiveJob> jobs = actor.Gameworld.ActiveJobs
                        .Where(x =>
                            !x.IsJobComplete &&
                            x.Character.Id != targetMembership.MemberId &&
                            x.Listing.ClanMembership == clan &&
                            x.Listing.ClanPaygrade is not null &&
                            targetMembership.Rank.Paygrades.Contains(x.Listing.ClanPaygrade) &&
                            x.Listing.ClanPaygrade.PayAmount > newPaygrade.PayAmount
                        ).ToList();
        if (jobs.Any())
        {
            actor.OutputHandler.Send(
                $"{targetMembership.MemberCharacter.HowSeen(actor, true)} cannot have their paygrade set so low as they hold their clan paygrade by virtue of the job{(jobs.Count == 1 ? "" : "s")} {jobs.Select(x => x.Name.ColourValue()).ListToString()}.");
            return;
        }

        clan.SetPaygrade(targetMembership, newPaygrade);
        if (targetActor != null)
        {
            actor.OutputHandler.Send(
                $"You decrease {targetActor.HowSeen(actor, type: DescriptionType.Possessive)} paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newPaygrade.Name.TitleCase().Colour(Telnet.Green)}.");
            targetActor.OutputHandler.Send(
                $"{actor.HowSeen(targetActor, true)} decreases your paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newPaygrade.Name.TitleCase().Colour(Telnet.Green)}.");
        }
        else
        {
            actor.Send("You decrease the paygrade of {0} in {2} to {1}.",
                targetMembership.PersonalName.GetName(NameStyle.FullName),
                newPaygrade.Name.TitleCase().Colour(Telnet.Green), clan.FullName.TitleCase().Colour(Telnet.Green));
        }
    }

    private static void ClanPaygrade(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to increase or decrease that person's paygrade?");
            return;
        }

        string promoteText = command.PopSpeech();
        switch (promoteText.ToLowerInvariant())
        {
            case "promote":
            case "increase":
                ClanPaygradePromote(actor, command);
                return;
            case "demote":
            case "decrease":
                ClanPaygradeDemote(actor, command);
                return;
            default:
                actor.OutputHandler.Send(
                    "You can either increase or decrease a person's paygrade. Specify which one you want to do.");
                return;
        }
    }

    private static void ClanViewDefault(ICharacter actor, IClan clan, IClanMembership actorMembership)
    {
        bool canViewMembers = actor.IsAdministrator() ||
                             actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers);
        bool canViewOffices = actor.IsAdministrator() ||
                             actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders);
        bool canViewAboveOwnRank = actor.IsAdministrator() ||
                                  actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure);
        bool canViewFinances = actor.IsAdministrator() ||
                              actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewTreasury);
        StringBuilder sb = new();
        sb.AppendLine(clan.FullName.TitleCase().GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
        sb.AppendLine();
        sb.AppendLine($"Alias: {clan.Alias.Colour(Telnet.Green)}");
        sb.AppendLine($"Name: {clan.Name.TitleCase().Colour(Telnet.Green)}");
        sb.AppendLine("Description:");
        sb.AppendLine();
        sb.AppendLine(clan.Description.Wrap(80, "\t").NoWrap());
        sb.AppendLine();
        if (actor.IsAdministrator())
        {
            sb.Append(
                $"Discord Channel: {clan.DiscordChannelId?.ToString("F0", actor).ColourValue() ?? "None".ColourError()}");
            sb.AppendLine();
            sb.AppendLine(
                $"Treasury Cells:\n{clan.TreasuryCells.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("None").ListToLines(true)}");
            sb.AppendLine();
            sb.AppendLine(
                $"Administration Cells:\n{clan.AdministrationCells.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("None").ListToLines(true)}");
            sb.AppendLine();
        }

        sb.AppendLine("Ranks:");
        List<IRank> ranks = (canViewAboveOwnRank
            ? clan.Ranks
            : clan.Ranks.Where(x => x.RankNumber <= (actorMembership?.Rank.RankNumber ?? 0))).ToList();
        if (canViewMembers)
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from rank in ranks
                    orderby rank.RankNumber
                    select new[]
                    {
                        rank.RankNumber.ToString(actor),
                        rank.Name.TitleCase(),
                        (rank.RankPath ?? "").TitleCase(),
                        clan.Memberships.Count(x => x.Rank == rank && !x.IsArchivedMembership).ToString(actor),
                        rank.Titles.Except(rank.Name)
                            .Select(x => x.TitleCase())
                            .ListToString(conjunction: "", twoItemJoiner: ", "),
                        rank.Paygrades.Select(x => x.Abbreviation).ListToString(conjunction: "", twoItemJoiner: ", ")
                    },
                    new[] { "Rank#", "Name", "Path", "No.", "Alternate Names", "Paygrades" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }
        else
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from rank in ranks
                    orderby rank.RankNumber
                    select new[]
                    {
                        rank.RankNumber.ToString(actor),
                        rank.Name.TitleCase(),
                        (rank.RankPath ?? "").TitleCase(),
                        rank.Titles.Except(rank.Name)
                            .Select(x => x.TitleCase())
                            .ListToString(conjunction: "", twoItemJoiner: ", "),
                        rank.Paygrades.Select(x => x.Abbreviation).ListToString(conjunction: "", twoItemJoiner: ", ")
                    },
                    new[] { "Rank#", "Name", "Path", "Alternate Names", "Paygrades" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }

        bool IsInParentAppointmentPosition(IAppointment appointment)
        {
            if (actorMembership == null)
            {
                return false;
            }

            if (actorMembership.Appointments.Contains(appointment))
            {
                return true;
            }

            if (appointment.ParentPosition != null)
            {
                return IsInParentAppointmentPosition(appointment.ParentPosition);
            }

            return false;
        }

        List<IAppointment> appointments =
            (canViewAboveOwnRank ? clan.Appointments : clan.Appointments.Where(x => IsInParentAppointmentPosition(x)))
            .ToList();
        sb.AppendLine("Appointments:");
        if (canViewOffices)
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from appointment in appointments
                    orderby
                        appointment.MinimumRankToHold?.RankNumber ?? -1 descending,
                        appointment.MinimumRankToAppoint?.RankNumber ?? -1
                            descending
                    select new[]
                    {
                        appointment.Name.TitleCase(),
                        appointment.MinimumRankToHold != null ? appointment.MinimumRankToHold.Name.TitleCase() : "None",
                        appointment.MinimumRankToAppoint != null
                            ? appointment.MinimumRankToAppoint.Name.TitleCase()
                            : "None",
                        appointment.MaximumSimultaneousHolders.ToString(actor),
                        clan.Memberships.Count(x => x.Appointments.Contains(appointment)).ToString(actor),
                        appointment.Paygrade != null ? appointment.Paygrade.Abbreviation : "None",
                        appointment.ParentPosition != null ? appointment.ParentPosition.Name.TitleCase() : "None",
                        appointment.IsAppointedByElection.ToColouredString()
                    },
                    new[] { "Name", "Min Rank", "Appointer", "Max No.", "No.", "Paygrade", "Parent", "Elected?" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }
        else
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from appointment in appointments
                    orderby
                        appointment.MinimumRankToHold?.RankNumber ?? -1 descending,
                        appointment.MinimumRankToAppoint?.RankNumber ?? -1
                            descending
                    select new[]
                    {
                        appointment.Name.TitleCase(),
                        appointment.MinimumRankToHold != null ? appointment.MinimumRankToHold.Name.TitleCase() : "None",
                        appointment.MinimumRankToAppoint != null
                            ? appointment.MinimumRankToAppoint.Name.TitleCase()
                            : "None",
                        appointment.MaximumSimultaneousHolders.ToString(actor),
                        appointment.Paygrade != null ? appointment.Paygrade.Abbreviation : "None",
                        appointment.ParentPosition != null ? appointment.ParentPosition.Name.TitleCase() : "None",
                        appointment.IsAppointedByElection.ToColouredString()
                    },
                    new[] { "Name", "Min Rank", "Appointer", "Max No.", "Paygrade", "Parent", "Elected?" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }

        List<IPaygrade> paygrades = (canViewAboveOwnRank
            ? clan.Paygrades
            : clan.Paygrades.Where(x =>
                clan.Ranks.Any(y => y.Paygrades.Contains(x) && y.RankNumber <= actorMembership.Rank.RankNumber) ||
                clan.Appointments.Any(y => y.Paygrade == x && IsInParentAppointmentPosition(y)))).ToList();
        sb.AppendLine("Paygrades:");
        if (canViewMembers && canViewFinances)
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from paygrade in paygrades
                    orderby paygrade.PayCurrency.Id, paygrade.PayAmount
                    select new[]
                    {
                        paygrade.Abbreviation,
                        paygrade.Name.TitleCase(),
                        paygrade.PayCurrency.Name.TitleCase(),
                        paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short),
                        (clan.Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
                         clan.Memberships.Sum(x =>
                             x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade)))
                        .ToString(actor),
                        paygrade.PayCurrency.Describe(
                            paygrade.PayAmount *
                            (clan.Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
                             clan.Memberships.Sum(x =>
                                 x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))),
                            CurrencyDescriptionPatternType.Short)
                    },
                    new[] { "Abbreviation", "Name", "Currency", "Amount", "No.", "Total Per Pay" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }
        else if (canViewMembers)
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from paygrade in paygrades
                    orderby paygrade.PayCurrency.Id, paygrade.PayAmount
                    select new[]
                    {
                        paygrade.Abbreviation,
                        paygrade.Name.TitleCase(),
                        paygrade.PayCurrency.Name.TitleCase(),
                        paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short),
                        (clan.Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
                         clan.Memberships.Sum(x =>
                             x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade)))
                        .ToString(actor)
                    },
                    new[] { "Abbreviation", "Name", "Currency", "Amount", "No." },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }
        else if (canViewFinances)
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from paygrade in paygrades
                    orderby paygrade.PayCurrency.Id, paygrade.PayAmount
                    select new[]
                    {
                        paygrade.Abbreviation,
                        paygrade.Name.TitleCase(),
                        paygrade.PayCurrency.Name.TitleCase(),
                        paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short),
                        paygrade.PayCurrency.Describe(
                            paygrade.PayAmount *
                            (clan.Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
                             clan.Memberships.Sum(x =>
                                 x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))),
                            CurrencyDescriptionPatternType.Short)
                    },
                    new[] { "Abbreviation", "Name", "Currency", "Amount", "Total Per Pay" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }
        else
        {
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from paygrade in paygrades
                    orderby paygrade.PayCurrency.Id, paygrade.PayAmount
                    select new[]
                    {
                        paygrade.Abbreviation,
                        paygrade.Name.TitleCase(),
                        paygrade.PayCurrency.Name.TitleCase(),
                        paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short)
                    },
                    new[] { "Abbreviation", "Name", "Currency", "Amount" },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 5,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
        }

        sb.AppendLine();
        if (clan.ExternalControls.Any(x => x.VassalClan == clan))
        {
            sb.AppendLine("External Controls (Over This Clan):");
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from external in clan.ExternalControls.Where(x => x.VassalClan == clan)
                    select new[]
                    {
                        external.ControlledAppointment.Name.TitleCase(),
                        external.LiegeClan.FullName.TitleCase(),
                        external.ControllingAppointment != null
                            ? external.ControllingAppointment.Name.TitleCase()
                            : "None",
                        external.NumberOfAppointments > 0
                            ? external.NumberOfAppointments.ToString("N0", actor)
                            : "Unlimited",
                        external.Appointees.Count.ToString("N0", actor)
                    },
                    new[] { "Appointment", "Liege Clan", "Liege Appointment", "Max No.", "No." },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 2,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
            sb.AppendLine();
        }

        if (clan.ExternalControls.Any(x => x.LiegeClan == clan))
        {
            sb.AppendLine("External Controls (Under This Clan):");
            sb.AppendLine(
                StringUtilities.GetTextTable(
                    from external in clan.ExternalControls.Where(x => x.LiegeClan == clan)
                    select new[]
                    {
                        external.ControlledAppointment.Name.TitleCase(),
                        external.VassalClan.FullName.TitleCase(),
                        external.ControllingAppointment != null
                            ? external.ControllingAppointment.Name.TitleCase()
                            : "None",
                        external.NumberOfAppointments > 0
                            ? external.NumberOfAppointments.ToString("N0", actor)
                            : "Unlimited",
                        external.Appointees.Count.ToString("N0", actor)
                    },
                    new[] { "Appointment", "Vassal Clan", "Liege Appointment", "Max No.", "No." },
                    actor.Account.LineFormatLength,
                    colour: Telnet.Green,
                    truncatableColumnIndex: 2,
                    unicodeTable: actor.Account.UseUnicode
                )
            );
            sb.AppendLine();
        }

        if (canViewFinances)
        {
            sb.AppendLine(
                $"Default Bank Account: {clan.ClanBankAccount?.AccountReference.Colour(Telnet.BoldCyan) ?? "None".Colour(Telnet.Red)}");
            if (clan.ClanBankAccount is not null)
            {
                sb.AppendLine(
                    $"Default Account Balance: {clan.ClanBankAccount.Currency.Describe(clan.ClanBankAccount.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
            }

            List<(ICurrency Currency, decimal Value)> currencyPiles =
                clan.TreasuryCells
                    .SelectMany(x => x.GameItems.SelectMany(y => y.RecursiveGetItems<ICurrencyPile>()))
                    .GroupBy(x => x.Currency)
                    .Select(x => (Currency: x.Key,
                        Value: x.Sum(pile => pile.Coins.Sum(coin => coin.Item1.Value * coin.Item2))))
                    .ToList();
            if (currencyPiles.Any(x => x.Value > 0.0M))
            {
                sb.AppendLine(
                    $"Treasury Balance: {currencyPiles.Select(x => x.Currency.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");
            }

            List<IBankAccount> accounts = actor.Gameworld.Banks.SelectMany(x => x.BankAccounts.Where(y => y.AccountOwner == clan))
                                .ToList();
            if (accounts.Any())
            {
                sb.AppendLine("Clan Bank Accounts:");
                sb.AppendLine();
                foreach (IBankAccount account in accounts)
                {
                    sb.AppendLine(
                        $"\t{account.AccountReference.Colour(Telnet.BoldCyan)} - {account.Currency.Describe(account.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
                }
            }

            sb.AppendLine();
            sb.AppendLine(
                $"Total Commitment Salary Per Payday: {clan.Paygrades.Select(x => x.PayCurrency).Distinct().Select(x => x.Describe(clan.Paygrades.Where(y => y.PayCurrency == x).Sum(y => y.PayAmount * (clan.Memberships.Count(z => !z.IsArchivedMembership && z.Paygrade == y) + clan.Memberships.Sum(z => z.Appointments.Count(v => !z.IsArchivedMembership && v.Paygrade == y)))), CurrencyDescriptionPatternType.Short)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
        }

        sb.AppendLine($"Clan Calendar: {clan.Calendar.ShortName.TitleCase().ColourValue()}");
        sb.AppendLine($"Payday Interval: {clan.PayInterval.Describe(clan.Calendar).ColourValue()}");
        sb.AppendLine($"Next Payday: {clan.Calendar.DisplayDate(clan.NextPay.Date, CalendarDisplayMode.Short).Colour(Telnet.Green)} at {clan.Calendar.FeedClock.DisplayTime(clan.NextPay.Time, TimeDisplayTypes.Immortal).Colour(Telnet.Green)}");
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanViewRank(ICharacter actor, IClan clan, IClanMembership actorMembership, IRank rank)
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Format("Rank {0} ID #{2} - {1}", rank.Name.TitleCase().Colour(Telnet.Green),
            clan.FullName.TitleCase().Colour(Telnet.Green), rank.Id.ToString().Colour(Telnet.Green)));
        sb.AppendLine();
        sb.Append(
            new[]
            {
                $"Name: {rank.Name.TitleCase().Colour(Telnet.Green)}",
                string.Format(actor, "Rank Number: {0}", rank.RankNumber.ToString("N0", actor).Colour(Telnet.Green))
            }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.Append(new[]
        {
            $"Rank Path: {(rank.RankPath ?? "").Colour(Telnet.Green)}",
            $"Insignia: {(rank.InsigniaGameItem != null ? string.Format(actor, "#{0} rev {1}", rank.InsigniaGameItem.Id, rank.InsigniaGameItem.RevisionNumber).Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.AppendLine();
        sb.AppendLine("Abbreviations:");
        foreach (Tuple<IFutureProg, string> item in rank.AbbreviationsAndProgs)
        {
            sb.AppendLine(
                $"\tAbbreviation: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 != null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 != null ? string.Format(actor, " (#{0:N0})", item.Item1.Id) : "")}");
        }

        sb.AppendLine();
        sb.AppendLine("Rank Names:");
        foreach (Tuple<IFutureProg, string> item in rank.TitlesAndProgs)
        {
            sb.AppendLine(
                $"\tName: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 != null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 != null ? string.Format(actor, " (#{0:N0})", item.Item1.Id) : "")}");
        }

        sb.AppendLine();
        sb.AppendLine("Paygrades:");
        foreach (IPaygrade item in rank.Paygrades)
        {
            sb.AppendLine(
                $"\tPaygrade {item.Abbreviation.Colour(Telnet.Green)} ({item.Name.TitleCase().Colour(Telnet.Green)}) - {item.PayCurrency.Describe(item.PayAmount, CurrencyDescriptionPatternType.Short)}.");
        }

        sb.AppendLine();
        sb.AppendLine("Privileges:");
        sb.AppendLine("\t" + rank.Privileges);
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanViewPaygrade(ICharacter actor, IClan clan, IClanMembership actorMembership,
        IPaygrade paygrade)
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Format("Appointment {0} ID #{2} - {1}", paygrade.Name.TitleCase().Colour(Telnet.Green),
            clan.FullName.TitleCase().Colour(Telnet.Green), paygrade.Id.ToString().Colour(Telnet.Green)));
        sb.AppendLine();
        sb.Append(
            new[]
                {
                    $"Abbreviation: {paygrade.Abbreviation.Colour(Telnet.Green)}",
                    string.Format(actor, "Name: {0}", paygrade.Name.TitleCase().Colour(Telnet.Green))
                }
                .ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.AppendLine(
            $"Pays {paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} in the {paygrade.PayCurrency.Name.Colour(Telnet.Green)} currency.");
    }

    private static void ClanViewAppointment(ICharacter actor, IClan clan, IClanMembership actorMembership,
        IAppointment appointment)
    {
        StringBuilder sb = new();
        sb.AppendLine(string.Format("Appointment {0} ID #{2} - {1}",
            appointment.Name.TitleCase().Colour(Telnet.Green), clan.FullName.TitleCase().Colour(Telnet.Green),
            appointment.Id.ToString().Colour(Telnet.Green)));
        sb.AppendLine();
        sb.Append(new[]
        {
            $"Name: {appointment.Name.TitleCase().Colour(Telnet.Green)}",
            $"Insignia: {(appointment.InsigniaGameItem != null ? string.Format(actor, "#{0} rev {1}", appointment.InsigniaGameItem.Id, appointment.InsigniaGameItem.RevisionNumber).Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.Append(new[]
        {
            $"Min Rank: {(appointment.MinimumRankToHold != null ? appointment.MinimumRankToHold.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
            $"Min Appointer Rank: {(appointment.MinimumRankToAppoint != null ? appointment.MinimumRankToAppoint.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.Append(new[]
        {
            $"Parent: {(appointment.ParentPosition != null ? appointment.ParentPosition.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
            string.Format(actor, "Max Holders: {0}",
                appointment.MaximumSimultaneousHolders.ToString("N0", actor).Colour(Telnet.Green))
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.AppendLine();
        if (appointment.IsAppointedByElection)
        {
            sb.AppendLine();
            sb.AppendLine($"Elections ({(appointment.IsSecretBallot ? "by secret ballot" : "by open voting")}):");
            sb.AppendLine();
            sb.Append(new[]
            {
                $"Term: {appointment.ElectionTerm.Describe(actor).ColourValue()}",
                $"Lead Time: {appointment.ElectionLeadTime.Describe(actor).ColourValue()}"
            }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
            sb.AppendLine();
            sb.Append(new[]
            {
                $"Nomination Period: {appointment.NominationPeriod.Describe(actor).ColourValue()}",
                $"Voting Period: {appointment.VotingPeriod.Describe(actor).ColourValue()}"
            }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
            sb.AppendLine();
            sb.Append(new[]
            {
                $"Term Limit (Consecutive): {appointment.MaximumConsecutiveTerms.ToString("N0", actor).ColourValue()}",
                $"Term Limit (Total): {appointment.MaximumTotalTerms.ToString("N0", actor).ColourValue()}"
            }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
            sb.AppendLine();
            sb.Append(new[]
            {
                $"Nominee Prog: {appointment.CanNominateProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}",
                $"Votes Prog: {appointment.NumberOfVotesProg?.MXPClickableFunctionNameWithId() ?? "None".Colour(Telnet.Red)}"
            }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        }

        sb.AppendLine("Abbreviations:");
        foreach (Tuple<IFutureProg, string> item in appointment.AbbreviationsAndProgs)
        {
            sb.AppendLine(
                $"\tAbbreviation: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 != null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 != null ? string.Format(actor, " (#{0:N0})", item.Item1.Id) : "")}");
        }

        sb.AppendLine();
        sb.AppendLine("Appointment Names:");
        foreach (Tuple<IFutureProg, string> item in appointment.TitlesAndProgs)
        {
            sb.AppendLine(string.Format(actor, "\tName: {0} FutureProg: {1}{2}",
                item.Item2.Colour(Telnet.Green),
                item.Item1 != null
                    ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send",
                        $"href='show futureprog {item.Item1.Id}'")
                    : "None".Colour(Telnet.Red),
                item.Item1 != null ? string.Format(actor, " (#{0:N0})", item.Item1.Id) : ""
            ));
        }

        sb.AppendLine();
        if (appointment.Paygrade != null)
        {
            sb.AppendLine(string.Format(actor, "Paygrade: {0} ({1}) - {2}.",
                appointment.Paygrade.Abbreviation.Colour(Telnet.Green),
                appointment.Paygrade.Name.TitleCase().Colour(Telnet.Green),
                appointment.Paygrade.PayCurrency.Describe(appointment.Paygrade.PayAmount,
                    CurrencyDescriptionPatternType.Short)
            ));
        }
        else
        {
            sb.AppendLine($"Paygrade: {"None".Colour(Telnet.Red)}");
        }


        sb.AppendLine();
        sb.AppendLine("Privileges:");
        sb.AppendLine("\t" + appointment.Privileges);
        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanViewExternalControl(ICharacter actor, IClan clan, IClanMembership actorMembership,
        IExternalClanControl external)
    {
        StringBuilder sb = new();
        sb.AppendLine("External Appointment");
        sb.AppendLine();
        sb.AppendLine(new[]
        {
            $"Vassal Clan: {external.VassalClan.FullName.TitleCase().Colour(Telnet.Green)}",
            $"Controlled Appointment: {external.ControlledAppointment.Name.TitleCase().Colour(Telnet.Green)}"
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.AppendLine(new[]
        {
            $"Liege Clan: {external.LiegeClan.FullName.TitleCase().Colour(Telnet.Green)}",
            $"Controlling Appointment: {(external.ControllingAppointment != null ? external.ControllingAppointment.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
        }.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
        sb.AppendLine(
            $"Maximum Appointees: {(external.NumberOfAppointments > 0 ? external.NumberOfAppointments.ToString("N0", actor) : "Unlimited".Colour(Telnet.Red))}");
        sb.AppendLine();
        if (external.Appointees.Any())
        {
            sb.AppendLine("Current Appointees:");
            foreach (IClanMembership appointee in external.Appointees)
            {
                sb.AppendLine("\t" + appointee.PersonalName.GetName(NameStyle.FullName));
            }
        }

        actor.OutputHandler.Send(sb.ToString());
    }

    private static void ClanView(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            if (actor.AffectedBy<BuilderEditingEffect<IClan>>())
            {
                IClan editingclan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().First().EditingItem;
                editingclan.Show(actor, new StringStack(string.Empty));
                return;
            }

            actor.OutputHandler.Send("Which clan do you wish to view?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech(), true);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.Show(actor, command);
    }

    #region ClanCreate Subcommands

    private static void ClanCreateNew(ICharacter actor, StringStack command)
    {
        if (!actor.IsAdministrator() && !Community.Clan.CanCreateClan(actor))
        {
            actor.OutputHandler.Send("You are not allowed to create clans.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify an alias for your new clan.");
            return;
        }

        string aliasText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new clan?");
            return;
        }

        string nameText = command.SafeRemainingArgument.TitleCase();

        if (
            actor.Gameworld.Clans.Any(
                x =>
                    x.Alias.Equals(aliasText, StringComparison.InvariantCultureIgnoreCase) ||
                    x.FullName.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a clan with that name or alias. You must supply one that is unique.");
            return;
        }

        using (new FMDB())
        {
            Models.Clan dbclan = new();
            FMDB.Context.Clans.Add(dbclan);
            dbclan.Alias = aliasText;
            dbclan.Name = nameText;
            dbclan.FullName = nameText;
            dbclan.Description = "An undescribed clan.";
            dbclan.CalendarId = actor.Culture.PrimaryCalendar.Id;
            dbclan.PayIntervalType = 0;
            dbclan.PayIntervalModifier = 1;
            dbclan.PayIntervalOther = 0;
            dbclan.PayIntervalOtherSecondary = 0;
            dbclan.PayIntervalFallback = 0;
            dbclan.PayIntervalReferenceDate = actor.Culture.PrimaryCalendar.CurrentDate.GetDateString();
            dbclan.PayIntervalReferenceTime = "0:0:0";

            Models.Rank dbrank = new();
            FMDB.Context.Ranks.Add(dbrank);
            dbrank.Clan = dbclan;
            dbrank.Name = "Member";
            dbrank.Privileges = (long)ClanPrivilegeType.None;
            RanksTitle dbrankaddress = new();
            dbrankaddress.Title = "Member";
            dbrankaddress.Rank = dbrank;
            dbrankaddress.Order = 0;
            FMDB.Context.RanksTitles.Add(dbrankaddress);
            RanksAbbreviations dbrankabbrev = new();
            dbrankabbrev.Rank = dbrank;
            dbrankabbrev.Order = 0;
            dbrankabbrev.Abbreviation = "Mbr";
            FMDB.Context.RanksAbbreviations.Add(dbrankabbrev);
            Appointment dbpos = new();
            FMDB.Context.Appointments.Add(dbpos);
            dbpos.Clan = dbclan;
            dbpos.Name = "Founder";
            dbpos.MaximumSimultaneousHolders = 1;
            dbpos.Privileges = (long)ClanPrivilegeType.All;
            AppointmentsTitles dbposaddress = new();
            dbposaddress.Title = "Founder";
            dbposaddress.Appointment = dbpos;
            dbposaddress.Order = 0;
            FMDB.Context.AppointmentsTitles.Add(dbposaddress);
            AppointmentsAbbreviations dbposabbrev = new();
            dbposabbrev.Appointment = dbpos;
            dbposabbrev.Abbreviation = "Fdr";
            dbposabbrev.Order = 0;
            FMDB.Context.AppointmentsAbbreviations.Add(dbposabbrev);

            FMDB.Context.SaveChanges();
            Community.Clan newClan = new(dbclan, actor.Gameworld);
            newClan.FinaliseLoad(dbclan, Enumerable.Empty<Models.ClanMembership>());
            actor.Gameworld.Add(newClan);

            Models.ClanMembership dbMembership = new();
            FMDB.Context.ClanMemberships.Add(dbMembership);
            dbMembership.CharacterId = actor.Id;
            dbMembership.Clan = dbclan;
            dbMembership.ClanMembershipsAppointments.Add(new ClanMembershipsAppointments
            { ClanMembership = dbMembership, Appointment = dbpos });
            dbMembership.RankId = dbrank.Id;
            dbMembership.PersonalName = actor.CurrentName.SaveToXml().ToString();
            dbMembership.JoinDate = actor.Culture.PrimaryCalendar.CurrentDate.GetDateString();
            FMDB.Context.SaveChanges();
            ClanMembership newMembership = new(dbMembership, newClan, actor.Gameworld);
            newClan.Memberships.Add(newMembership);
            actor.AddMembership(newMembership);

            actor.Send("You create the clan {0}, alias {1}.", newClan.FullName.TitleCase().Colour(Telnet.Green),
                newClan.Alias.Colour(Telnet.Green));
        }
    }

    private static void ClanCreateTemplate(ICharacter actor, StringStack command)
    {
        if (!actor.IsAdministrator() && !Community.Clan.CanCreateClan(actor))
        {
            actor.OutputHandler.Send("You are not allowed to create clans.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you want to use as a template for your new clan?");
            return;
        }

        string clanText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("You must specify an alias for your new clan.");
            return;
        }

        string aliasText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to your new clan?");
            return;
        }

        string nameText = command.SafeRemainingArgument.TitleCase();

        IEnumerable<IClan> allClans = actor.IsAdministrator()
            ? actor.Gameworld.Clans
            : actor.Gameworld.Clans.Where(x => x.IsTemplate);
        IClan clan =
            allClans.FirstOrDefault(x => x.FullName.Equals(clanText, StringComparison.InvariantCultureIgnoreCase)) ??
            allClans.FirstOrDefault(x => x.Alias.Equals(clanText, StringComparison.InvariantCultureIgnoreCase));
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no such clan for you to copy as a template.");
            return;
        }

        if (
            actor.Gameworld.Clans.Any(
                x =>
                    x.Alias.Equals(aliasText, StringComparison.InvariantCultureIgnoreCase) ||
                    x.FullName.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a clan with that name or alias. You must supply one that is unique.");
            return;
        }

        using (new FMDB())
        {
            Models.Clan dbclan = new()
            {
                Alias = aliasText,
                Name = nameText,
                FullName = nameText,
                Description = clan.Description,
                CalendarId = clan.Calendar.Id,
                PayIntervalType = (int)clan.PayInterval.Type,
                PayIntervalModifier = clan.PayInterval.IntervalAmount,
                PayIntervalOther = clan.PayInterval.Modifier,
                PayIntervalOtherSecondary = clan.PayInterval.SecondaryModifier,
                PayIntervalFallback = (int)clan.PayInterval.OrdinalFallbackMode,
                PayIntervalReferenceDate = clan.Calendar.CurrentDate.GetDateString(),
                PayIntervalReferenceTime = clan.NextPay.Time.GetTimeString(),
                ShowClanMembersInWho = clan.ShowClanMembersInWho,
                ShowFamousMembersInNotables = clan.ShowFamousMembersInNotables,
                Sphere = clan.Sphere,
                MaximumPeriodsOfUncollectedBackPay = clan.MaximumPeriodsOfUncollectedBackPay,
                OnPayProgId = clan.OnPayProg?.Id
            };
            FMDB.Context.Clans.Add(dbclan);

            foreach (IPaygrade paygrade in clan.Paygrades)
            {
                Models.Paygrade dbpaygrade = new();
                FMDB.Context.Paygrades.Add(dbpaygrade);
                dbpaygrade.Clan = dbclan;
                dbclan.Paygrades.Add(dbpaygrade);
                dbpaygrade.CurrencyId = paygrade.PayCurrency.Id;
                dbpaygrade.Name = paygrade.Name;
                dbpaygrade.PayAmount = paygrade.PayAmount;
                dbpaygrade.Abbreviation = paygrade.Abbreviation;
            }

            foreach (IRank rank in clan.Ranks)
            {
                Models.Rank dbrank = new();
                FMDB.Context.Ranks.Add(dbrank);
                dbclan.Ranks.Add(dbrank);
                dbrank.Name = rank.Name;
                dbrank.RankPath = rank.RankPath;
                dbrank.RankNumber = rank.RankNumber;
                dbrank.FameType = (int)rank.FameType;
                if (rank.InsigniaGameItem != null)
                {
                    dbrank.InsigniaGameItemId = rank.InsigniaGameItem.Id;
                    dbrank.InsigniaGameItemRevnum = rank.InsigniaGameItem.RevisionNumber;
                }

                dbrank.Privileges = (long)rank.Privileges;
                int order = 0;
                foreach (Tuple<IFutureProg, string> address in rank.TitlesAndProgs)
                {
                    RanksTitle dbrankaddress = new();
                    dbrankaddress.Title = address.Item2;
                    dbrankaddress.Order = order++;
                    dbrankaddress.FutureProgId = address.Item1?.Id;
                    dbrankaddress.Rank = dbrank;
                    FMDB.Context.RanksTitles.Add(dbrankaddress);
                }

                order = 0;
                foreach (Tuple<IFutureProg, string> abbreviation in rank.AbbreviationsAndProgs)
                {
                    RanksAbbreviations dbrankabbrev = new();
                    dbrankabbrev.Abbreviation = abbreviation.Item2;
                    dbrankabbrev.Order = order++;
                    dbrankabbrev.FutureProgId = abbreviation.Item1?.Id;
                    dbrankabbrev.Rank = dbrank;
                    FMDB.Context.RanksAbbreviations.Add(dbrankabbrev);
                }

                order = 0;
                foreach (IPaygrade paygrade in rank.Paygrades)
                {
                    RanksPaygrade dbrankpaygrade = new();
                    dbrankpaygrade.Rank = dbrank;
                    dbrankpaygrade.Paygrade =
                        dbclan.Paygrades.First(
                            x => x.Name == paygrade.Name);
                    dbrankpaygrade.Order = order++;
                    FMDB.Context.RanksPaygrades.Add(dbrankpaygrade);
                }
            }

            List<Tuple<Appointment, string>> stagedAppointments = new();
            List<(IAppointment SourceAppointment, Appointment DbAppointment)> clonedAppointments = new();
            foreach (IAppointment appointment in clan.Appointments)
            {
                Appointment dbappointment = new();
                dbappointment.Name = appointment.Name;
                dbappointment.Clan = dbclan;
                dbappointment.Privileges = (long)appointment.Privileges;
                dbappointment.FameType = (int)appointment.FameType;
                dbappointment.MaximumSimultaneousHolders = appointment.MaximumSimultaneousHolders;
                dbappointment.IsAppointedByElection = appointment.IsAppointedByElection;
                dbappointment.ElectionTermMinutes =
                    appointment.IsAppointedByElection ? appointment.ElectionTerm.TotalMinutes : null;
                dbappointment.ElectionLeadTimeMinutes =
                    appointment.IsAppointedByElection ? appointment.ElectionLeadTime.TotalMinutes : null;
                dbappointment.NominationPeriodMinutes =
                    appointment.IsAppointedByElection ? appointment.NominationPeriod.TotalMinutes : null;
                dbappointment.VotingPeriodMinutes =
                    appointment.IsAppointedByElection ? appointment.VotingPeriod.TotalMinutes : null;
                dbappointment.MaximumConsecutiveTerms =
                    appointment.IsAppointedByElection ? appointment.MaximumConsecutiveTerms : null;
                dbappointment.MaximumTotalTerms =
                    appointment.IsAppointedByElection ? appointment.MaximumTotalTerms : null;
                dbappointment.IsSecretBallot =
                    appointment.IsAppointedByElection ? appointment.IsSecretBallot : null;
                dbappointment.CanNominateProgId = appointment.CanNominateProg?.Id;
                dbappointment.WhyCantNominateProgId = appointment.WhyCantNominateProg?.Id;
                dbappointment.NumberOfVotesProgId = appointment.NumberOfVotesProg?.Id;
                if (appointment.InsigniaGameItem != null)
                {
                    dbappointment.InsigniaGameItemId = appointment.InsigniaGameItem.Id;
                    dbappointment.InsigniaGameItemRevnum = appointment.InsigniaGameItem.RevisionNumber;
                }

                if (appointment.MinimumRankToHold != null)
                {
                    dbappointment.MinimumRank =
                        dbclan.Ranks.First(
                            x =>
                                x.Name == appointment.MinimumRankToHold.Name);
                }

                if (appointment.MinimumRankToAppoint != null)
                {
                    dbappointment.MinimumRankToAppoint =
                        dbclan.Ranks.First(
                            x =>
                                x.Name == appointment.MinimumRankToAppoint.Name);
                }

                if (appointment.Paygrade != null)
                {
                    dbappointment.Paygrade =
                        dbclan.Paygrades.First(
                            x =>
                                x.Name == appointment.Paygrade.Name);
                }

                FMDB.Context.Appointments.Add(dbappointment);
                int order = 0;
                foreach (Tuple<IFutureProg, string> abbreviation in appointment.AbbreviationsAndProgs)
                {
                    AppointmentsAbbreviations dbabbrev = new();
                    dbabbrev.Abbreviation = abbreviation.Item2;
                    dbabbrev.Order = order++;
                    dbabbrev.FutureProgId = abbreviation.Item1?.Id;
                    dbabbrev.Appointment = dbappointment;
                    FMDB.Context.AppointmentsAbbreviations.Add(dbabbrev);
                }

                order = 0;
                foreach (Tuple<IFutureProg, string> address in appointment.TitlesAndProgs)
                {
                    AppointmentsTitles dbaddress = new();
                    dbaddress.Title = address.Item2;
                    dbaddress.Order = order++;
                    dbaddress.FutureProgId = address.Item1?.Id;
                    dbaddress.Appointment = dbappointment;
                    FMDB.Context.AppointmentsTitles.Add(dbaddress);
                }

                if (appointment.ParentPosition != null)
                {
                    stagedAppointments.Add(Tuple.Create(dbappointment, appointment.ParentPosition.Name));
                }

                clonedAppointments.Add((appointment, dbappointment));
            }

            foreach (Tuple<Appointment, string> appointment in stagedAppointments)
            {
                appointment.Item1.ParentAppointment =
                    dbclan.Appointments.First(
                        x => x.Name == appointment.Item2);
            }

            FMDB.Context.SaveChanges();
            RebindTemplateElectionPlaceholderProgs(actor, clan, dbclan, clonedAppointments);
            FMDB.Context.SaveChanges();
            Community.Clan newClan = new(dbclan, actor.Gameworld);
            newClan.FinaliseLoad(dbclan, Enumerable.Empty<Models.ClanMembership>());
            actor.Gameworld.Add(newClan);

            Models.ClanMembership dbMembership = new();
            FMDB.Context.ClanMemberships.Add(dbMembership);
            dbMembership.CharacterId = actor.Id;
            dbMembership.Clan = dbclan;

            IAppointment targetAppointment = newClan.Appointments.FirstOrDefault(x => x.Privileges == ClanPrivilegeType.All);
            if (targetAppointment != null)
            {
                dbMembership.ClanMembershipsAppointments.Add(new ClanMembershipsAppointments
                { ClanMembership = dbMembership, AppointmentId = targetAppointment.Id });
            }

            Models.Rank targetRank = dbclan.Ranks.AsEnumerable().FirstMax(x => x.RankNumber);
            dbMembership.RankId = targetRank.Id;

            RanksPaygrade targetpaygrade = targetRank.RanksPaygrades.OrderBy(x => x.Order).LastOrDefault();
            if (targetpaygrade != null)
            {
                dbMembership.PaygradeId = targetpaygrade.PaygradeId;
            }

            dbMembership.PersonalName = actor.CurrentName.SaveToXml().ToString();
            dbMembership.JoinDate = actor.Culture.PrimaryCalendar.CurrentDate.GetDateString();
            FMDB.Context.SaveChanges();
            ClanMembership newMembership = new(dbMembership, newClan, actor.Gameworld);
            newClan.Memberships.Add(newMembership);
            actor.AddMembership(newMembership);
            Community.Clan.CreatedClan(actor, newClan);

            actor.Send("You create the clan {0}, alias {1} from template clan {2}.",
                newClan.FullName.TitleCase().Colour(Telnet.Cyan), newClan.Alias.Colour(Telnet.Green),
                clan.FullName.TitleCase().Colour(Telnet.Cyan));
        }
    }

    private static void RebindTemplateElectionPlaceholderProgs(ICharacter actor, IClan sourceClan, Models.Clan clonedClan,
        IEnumerable<(IAppointment SourceAppointment, Appointment DbAppointment)> appointments)
    {
        foreach ((IAppointment sourceAppointment, Appointment dbAppointment) in appointments)
        {
            if (!sourceAppointment.IsAppointedByElection)
            {
                continue;
            }

            TemplateElectionRebindMode rebindMode = GetTemplateElectionRebindMode(sourceAppointment.CanNominateProg?.FunctionName,
                sourceAppointment.CanNominateProg?.FunctionText,
                sourceAppointment.NumberOfVotesProg?.FunctionName,
                sourceAppointment.NumberOfVotesProg?.FunctionText);
            if (rebindMode == TemplateElectionRebindMode.None)
            {
                continue;
            }

            string canNominateText = BuildTemplateElectionCanNominateText(clonedClan.Id, rebindMode);
            string whyCantNominateText = BuildTemplateElectionWhyText(rebindMode);
            string numberOfVotesText = BuildTemplateElectionVotesText(clonedClan.Id, rebindMode);

            Models.FutureProg canNominateProg = CreateAndRegisterCloneElectionProg(actor,
                $"CloneElectionCanNominate_{clonedClan.Id}_{dbAppointment.Id}",
                $"Generated nomination eligibility prog for {dbAppointment.Name} in cloned clan {clonedClan.FullName}.",
                ProgVariableTypes.Boolean, canNominateText);
            Models.FutureProg whyCantNominateProg = CreateAndRegisterCloneElectionProg(actor,
                $"CloneElectionWhyCantNominate_{clonedClan.Id}_{dbAppointment.Id}",
                $"Generated nomination failure explanation prog for {dbAppointment.Name} in cloned clan {clonedClan.FullName}.",
                ProgVariableTypes.Text, whyCantNominateText);
            Models.FutureProg numberOfVotesProg = CreateAndRegisterCloneElectionProg(actor,
                $"CloneElectionVotes_{clonedClan.Id}_{dbAppointment.Id}",
                $"Generated voting eligibility prog for {dbAppointment.Name} in cloned clan {clonedClan.FullName}.",
                ProgVariableTypes.Number, numberOfVotesText);

            dbAppointment.CanNominateProgId = canNominateProg.Id;
            dbAppointment.WhyCantNominateProgId = whyCantNominateProg.Id;
            dbAppointment.NumberOfVotesProgId = numberOfVotesProg.Id;
        }
    }

    private static MudSharp.Models.FutureProg CreateAndRegisterCloneElectionProg(ICharacter actor, string functionName,
        string functionComment, ProgVariableTypes returnType, string functionText)
    {
        Models.FutureProg dbprog = new()
        {
            FunctionName = functionName,
            FunctionComment = functionComment,
            FunctionText = functionText,
            ReturnType = (long)returnType,
            Category = "Clan",
            Subcategory = "Template Clone",
            Public = false,
            AcceptsAnyParameters = false,
            StaticType = (int)FutureProgStaticType.NotStatic
        };
        dbprog.FutureProgsParameters.Add(new FutureProgsParameter
        {
            FutureProg = dbprog,
            ParameterIndex = 0,
            ParameterName = "ch",
            ParameterType = (long)ProgVariableTypes.Character
        });
        FMDB.Context.FutureProgs.Add(dbprog);
        FMDB.Context.SaveChanges();

        FutureProg.FutureProg runtimeProg = new(dbprog, actor.Gameworld);
        if (!runtimeProg.Compile())
        {
            throw new InvalidOperationException(
                $"Generated clone election prog {functionName} failed to compile: {runtimeProg.CompileError}");
        }

        actor.Gameworld.Add(runtimeProg);
        return dbprog;
    }

    private static string BuildTemplateElectionCanNominateText(long clanId, TemplateElectionRebindMode mode)
    {
        return mode switch
        {
            TemplateElectionRebindMode.AllMembers => $"return IsClanMember(@ch, ToClan({clanId}))",
            TemplateElectionRebindMode.Councillor =>
                $"return IsClanMember(@ch, ToClan({clanId}), ToRank(ToClan({clanId}), \"Councillor\"))",
            TemplateElectionRebindMode.Board =>
                $"return IsClanMember(@ch, ToClan({clanId}), ToRank(ToClan({clanId}), \"Board\"))",
            _ => "return false"
        };
    }

    private static string BuildTemplateElectionVotesText(long clanId, TemplateElectionRebindMode mode)
    {
        return mode switch
        {
            TemplateElectionRebindMode.AllMembers => $@"if (IsClanMember(@ch, ToClan({clanId})))
 return 1
end if
return 0",
            TemplateElectionRebindMode.Councillor => $@"if (IsClanMember(@ch, ToClan({clanId}), ToRank(ToClan({clanId}), ""Councillor"")))
 return 1
end if
return 0",
            TemplateElectionRebindMode.Board => $@"if (IsClanMember(@ch, ToClan({clanId}), ToRank(ToClan({clanId}), ""Board"")))
 return 1
end if
return 0",
            _ => "return 0"
        };
    }

    private static string BuildTemplateElectionWhyText(TemplateElectionRebindMode mode)
    {
        return mode switch
        {
            TemplateElectionRebindMode.AllMembers =>
                @"return ""Only members of this clan may nominate for this office.""",
            TemplateElectionRebindMode.Councillor =>
                @"return ""Only councillors may nominate for this office.""",
            TemplateElectionRebindMode.Board =>
                @"return ""Only board members may nominate for this office.""",
            _ => @"return ""You are not eligible to nominate for this office."""
        };
    }

    private static TemplateElectionRebindMode GetTemplateElectionRebindMode(string canNominateProgName,
        string canNominateProgText, string numberOfVotesProgName, string numberOfVotesProgText)
    {
        if (canNominateProgName == TemplateCloneElectorateAllMembersCanNominateProgName &&
            numberOfVotesProgName == TemplateCloneElectorateAllMembersVotesProgName)
        {
            return TemplateElectionRebindMode.AllMembers;
        }

        if (canNominateProgName == TemplateCloneElectorateCouncillorCanNominateProgName &&
            numberOfVotesProgName == TemplateCloneElectorateCouncillorVotesProgName)
        {
            return TemplateElectionRebindMode.Councillor;
        }

        if (canNominateProgName == TemplateCloneElectorateBoardCanNominateProgName &&
            numberOfVotesProgName == TemplateCloneElectorateBoardVotesProgName)
        {
            return TemplateElectionRebindMode.Board;
        }

        if ((canNominateProgName?.StartsWith(CloneElectionCanNominateProgPrefix,
                 StringComparison.InvariantCultureIgnoreCase) ?? false) ||
            (numberOfVotesProgName?.StartsWith(CloneElectionVotesProgPrefix,
                 StringComparison.InvariantCultureIgnoreCase) ?? false))
        {
            TemplateElectionRebindMode inferredMode = InferTemplateElectionRebindModeFromProgText(canNominateProgText, numberOfVotesProgText);
            if (inferredMode != TemplateElectionRebindMode.None)
            {
                return inferredMode;
            }
        }

        return TemplateElectionRebindMode.None;
    }

    private static TemplateElectionRebindMode InferTemplateElectionRebindModeFromProgText(string canNominateProgText,
        string numberOfVotesProgText)
    {
        string combinedText = $"{canNominateProgText}\n{numberOfVotesProgText}";
        if (combinedText.IndexOf(@"""Councillor""", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            return TemplateElectionRebindMode.Councillor;
        }

        if (combinedText.IndexOf(@"""Board""", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            return TemplateElectionRebindMode.Board;
        }

        if (combinedText.IndexOf("IsClanMember(@ch, ToClan(", StringComparison.InvariantCultureIgnoreCase) != -1 &&
            combinedText.IndexOf("ToRank(", StringComparison.InvariantCultureIgnoreCase) == -1)
        {
            return TemplateElectionRebindMode.AllMembers;
        }

        return TemplateElectionRebindMode.None;
    }

    private enum TemplateElectionRebindMode
    {
        None,
        AllMembers,
        Councillor,
        Board
    }

    private static void ClanCreateRank(ICharacter actor, StringStack command)
    {
        IClan clan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().FirstOrDefault()?.EditingItem;
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not editing any clans.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateRanks))
        {
            actor.OutputHandler.Send("You are not allowed to create ranks in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new rank?");
            return;
        }

        string rankNameText = command.PopSpeech();
        if (clan.Ranks.Any(x => x.Name.Equals(rankNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a rank with that name in that clan. Rank names must be unique.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What default abbreviation do you want to give your new rank?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to put the new rank before or after an existing rank?");
            return;
        }

        bool before;
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "before":
                before = true;
                break;
            case "after":
                before = false;
                break;
            default:
                actor.OutputHandler.Send("Do you want to put the new rank before or after an existing rank?");
                return;
        }

        if (command.IsFinished)
        {
            actor.Send("{0} which rank do you want to insert your new rank?", before ? "Before" : "After");
            return;
        }

        string targetRankText = command.PopSpeech();
        IRank targetRank =
            clan.Ranks.FirstOrDefault(
                x => x.Name.Equals(targetRankText, StringComparison.InvariantCultureIgnoreCase));
        if (targetRank == null)
        {
            actor.OutputHandler.Send("There is no such rank for you to use as a reference for your new rank.");
            return;
        }

        IRank rankBefore;
        List<IRank> ranksAfter;
        if (before)
        {
            rankBefore = clan.Ranks.OrderBy(x => x.RankNumber).TakeWhile(x => x != targetRank).LastOrDefault();
            ranksAfter = clan.Ranks.OrderBy(x => x.RankNumber).SkipWhile(x => x != targetRank).ToList();
        }
        else
        {
            rankBefore = targetRank;
            ranksAfter = clan.Ranks.OrderBy(x => x.RankNumber).SkipWhile(x => x != targetRank).Skip(1).ToList();
        }

        using (new FMDB())
        {
            Models.Rank dbrank = new();
            FMDB.Context.Ranks.Add(dbrank);
            dbrank.ClanId = clan.Id;
            dbrank.RankNumber = ranksAfter.Any()
                ? ranksAfter.Min(x => x.RankNumber)
                : clan.Ranks.Max(x => x.RankNumber) + 1;
            dbrank.Name = rankNameText;
            dbrank.Privileges = (long)(rankBefore?.Privileges ?? ClanPrivilegeType.None);
            RanksAbbreviations dbrankabbrev = new();
            dbrankabbrev.Abbreviation = abbreviationText;
            dbrankabbrev.Rank = dbrank;
            FMDB.Context.RanksAbbreviations.Add(dbrankabbrev);
            RanksTitle dbrankaddress = new();
            dbrankaddress.Title = rankNameText;
            dbrankaddress.Rank = dbrank;
            FMDB.Context.RanksTitles.Add(dbrankaddress);
            FMDB.Context.SaveChanges();

            Community.Rank newRank = new(dbrank, clan);
            clan.Ranks.Add(newRank);
            if (!ranksAfter.Any() && actorMembership != null)
            {
                clan.SetRank(actorMembership, newRank);
                actor.OutputHandler.Send(
                    $"You create the rank of {newRank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}, and promote yourself to the new position.");
            }
            else
            {
                actor.OutputHandler.Send(
                    $"You create the rank of {newRank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
            }
        }

        foreach (IRank rank in ranksAfter)
        {
            rank.RankNumber++;
            rank.Changed = true;
        }
    }

    private static void ClanCreatePaygrade(ICharacter actor, StringStack command)
    {
        IClan clan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().FirstOrDefault()?.EditingItem;
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not editing any clans.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreatePaygrades))
        {
            actor.OutputHandler.Send("You are not allowed to create paygrade in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new paygrade?");
            return;
        }

        string paygradeNameText = command.PopSpeech();
        if (clan.Paygrades.Any(x => x.Name.Equals(paygradeNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a paygrade with that name in that clan. Paygrade names must be unique.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What abbreviation do you want to give your new paygrade?");
            return;
        }

        string abbreviationText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which currency do you want your new paygrade to use?");
            return;
        }

        string currencyText = command.PopSpeech();
        ICurrency currency = long.TryParse(currencyText, out long value)
            ? actor.Gameworld.Currencies.Get(value)
            : actor.Gameworld.Currencies.FirstOrDefault(
                x => x.Name.Equals(currencyText, StringComparison.InvariantCultureIgnoreCase));
        if (currency == null)
        {
            actor.OutputHandler.Send("That is not a valid currency for your new paygrade to use.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much should your new paygrade pay?");
            return;
        }

        string amountText = command.PopSpeech();
        decimal amount = currency.GetBaseCurrency(amountText, out bool success);
        if (!success)
        {
            actor.OutputHandler.Send("That is not a valid amount of money in that currency.");
            return;
        }

        using (new FMDB())
        {
            Models.Paygrade dbpaygrade = new();
            FMDB.Context.Paygrades.Add(dbpaygrade);
            dbpaygrade.Name = paygradeNameText;
            dbpaygrade.Abbreviation = abbreviationText;
            dbpaygrade.CurrencyId = currency.Id;
            dbpaygrade.ClanId = clan.Id;
            dbpaygrade.PayAmount = amount;
            FMDB.Context.SaveChanges();

            Community.Paygrade newPaygrade = new(dbpaygrade, clan);
            clan.Paygrades.Add(newPaygrade);
            actor.OutputHandler.Send(
                $"You create the {newPaygrade.Name.TitleCase().Colour(Telnet.Green)} paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }
    }

    private static void ClanCreateAppointment(ICharacter actor, StringStack command)
    {
        IClan clan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().FirstOrDefault()?.EditingItem;
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not editing any clans.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointmentsUnderOwn))
        {
            actor.OutputHandler.Send("You are not allowed to create appointments in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to the new appointment?");
            return;
        }

        string appointmentNameText = command.PopSpeech();
        if (
            clan.Appointments.Any(
                x => x.Name.Equals(appointmentNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already an appointment with that name in that clan. Appointment names must be unique.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What default abbreviation do you want to give your new appointment?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        IAppointment targetAppointment = null;
        if (!command.IsFinished)
        {
            if (command.Peek().Equals("under", StringComparison.InvariantCultureIgnoreCase))
            {
                command.PopSpeech();
            }

            string targetAppointmentText = command.PopSpeech();
            targetAppointment =
                clan.Appointments.FirstOrDefault(
                    x => x.Name.Equals(targetAppointmentText, StringComparison.InvariantCultureIgnoreCase));
            if (targetAppointment == null)
            {
                actor.OutputHandler.Send(
                    "There is no such appointment under which you can place this new appointment.");
                return;
            }
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin) &&
            actorMembership is not null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments))
        {
            if (targetAppointment is null)
            {
                actor.OutputHandler.Send(
                    "You may only create appointments beneath one of your own controlled appointments in that clan.");
                return;
            }

            if (!ClanCommandUtilities.HoldsOrControlsAppointment(actorMembership, targetAppointment))
            {
                actor.OutputHandler.Send(
                    "You may only create appointments beneath one of your own controlled appointments in that clan.");
                return;
            }
        }

        using (new FMDB())
        {
            Appointment dbappointment = new();
            FMDB.Context.Appointments.Add(dbappointment);
            dbappointment.ClanId = clan.Id;
            dbappointment.Name = appointmentNameText;
            dbappointment.Privileges = (long)ClanPrivilegeType.None;
            AppointmentsAbbreviations dbrankabbrev = new()
            {
                Abbreviation = abbreviationText,
                Appointment = dbappointment
            };
            FMDB.Context.AppointmentsAbbreviations.Add(dbrankabbrev);
            AppointmentsTitles dbrankaddress = new()
            {
                Title = appointmentNameText,
                Appointment = dbappointment
            };
            FMDB.Context.AppointmentsTitles.Add(dbrankaddress);

            if (targetAppointment != null)
            {
                dbappointment.ParentAppointmentId = targetAppointment.Id;
            }

            FMDB.Context.SaveChanges();

            Community.Appointment newAppointment = new(dbappointment, clan);
            newAppointment.FinaliseLoad(dbappointment);
            clan.Appointments.Add(newAppointment);
            actor.OutputHandler.Send(
                $"You create the appointment of {newAppointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }
    }

    #endregion ClanCreate Subcommands

    private static void ClanCreate(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to create a clan from a template, or brand new?");
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "template":
                ClanCreateTemplate(actor, command);
                break;
            case "new":
                ClanCreateNew(actor, command);
                break;
            case "rank":
                ClanCreateRank(actor, command);
                break;
            case "paygrade":
                ClanCreatePaygrade(actor, command);
                break;
            case "appointment":
                ClanCreateAppointment(actor, command);
                break;
            default:
                actor.OutputHandler.Send("You must either create a clan from a template, or use the new keyword.");
                return;
        }
    }

    #region ClanSet Subcommands

    private static void ClanEditProperty(ICharacter actor, StringStack command, string whichProperty)
    {
        IClan clan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().FirstOrDefault()?.EditingItem;
        if (clan == null)
        {
            actor.OutputHandler.Send("You are not editing any clans.");
            return;
        }

        switch (whichProperty)
        {
            case "name":
                ClanEditName(actor, command, clan);
                break;
            case "alias":
                ClanEditAlias(actor, command, clan);
                break;
            case "desc":
            case "description":
                ClanEditDescription(actor, command, clan);
                break;
            case "template":
                ClanEditTemplate(actor, command, clan);
                break;
            case "payday":
                ClanEditPayday(actor, command, clan);
                break;
            case "rank":
                ClanEditRank(actor, command, clan);
                break;
            case "appointment":
                ClanEditAppointment(actor, command, clan);
                break;
            case "paygrade":
                ClanEditPaygrade(actor, command, clan);
                break;
            case "notable":
                ClanEditNotable(actor, command, clan);
                break;
            case "sphere":
                ClanEditSphere(actor, command, clan);
                return;
            case "notablemembers":
                ClanEditNotableMembers(actor, command, clan);
                return;
            case "bankaccount":
                ClanEditBankAccount(actor, command, clan);
                return;
            case "discord":
                ClanEditDiscord(actor, command, clan);
                return;
            case "calendar":
                ClanEditCalendar(actor, command, clan);
                return;
        }
    }

    private static void ClanEditCalendar(ICharacter actor, StringStack command, IClan clan)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which calendar do you want this clan to use?");
            return;
        }

        ICalendar calendar = actor.Gameworld.Calendars.GetByIdOrName(command.SafeRemainingArgument);
        if (calendar is null)
        {
            actor.OutputHandler.Send("There is no such calendar.");
            return;
        }

        if (calendar == clan.Calendar)
        {
            actor.OutputHandler.Send($"The {clan.FullName.TitleCase().ColourName()} clan already uses the {calendar.ShortName.ColourValue()} calendar.");
            return;
        }

        clan.Calendar = calendar;
        actor.OutputHandler.Send($"The {clan.FullName.TitleCase().ColourName()} clan now uses the {calendar.ShortName.ColourValue()} calendar.");
    }

    private static void ClanEditBankAccount(ICharacter actor, StringStack command, IClan clan)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which bank account do you want to set as the clan bank account?");
            return;
        }

        if (!actor.IsAdministrator() && actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)?.NetPrivileges
                                             .HasFlag(ClanPrivilegeType.CanManageBankAccounts) != true)
        {
            actor.OutputHandler.Send(
                $"You are not authorised to manage bank accounts for the {clan.FullName.ColourName()} clan.");
            return;
        }

        if (command.SafeRemainingArgument.EqualToAny("none", "clear", "remove"))
        {
            clan.ClanBankAccount = null;
            actor.OutputHandler.Send(
                $"The {clan.FullName.ColourName()} clan no longer has a default bank account and will use treasury cash where possible.");
            return;
        }

        (IBankAccount account, string error) = Economy.Banking.Bank.FindBankAccount(command.SafeRemainingArgument, null, actor);
        if (account is null)
        {
            actor.OutputHandler.Send(error);
            return;
        }

        if (!account.IsAuthorisedAccountUser(actor))
        {
            actor.OutputHandler.Send("You are not authorised to use that bank account.");
            return;
        }

        clan.ClanBankAccount = account;
        actor.OutputHandler.Send(
            $"The {clan.FullName.ColourName()} clan will now use the {account.AccountReference.Colour(Telnet.BoldCyan)} account for payroll and other default transactions.");
    }

    private static void ClanEditNotable(ICharacter actor, StringStack command, IClan clan)
    {
        if (!actor.IsAdministrator(PermissionLevel.SeniorAdmin))
        {
            actor.Send(
                "Only a Senior Administrator or higher can declare that a clan is notable enough to appear on the who list.");
            return;
        }

        if (clan.ShowClanMembersInWho)
        {
            clan.ShowClanMembersInWho = false;
            clan.Changed = true;
            actor.Send("You declare that {0} are no longer notable enough to appear on the who list.",
                clan.FullName.ColourName());
        }
        else
        {
            clan.ShowClanMembersInWho = true;
            clan.Changed = true;
            actor.Send("You declare that {0} is now a sufficiently notable clan to appear on the who list.",
                clan.FullName.ColourName());
        }
    }

    private static void ClanEditName(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanEditClanDetails))
        {
            actor.OutputHandler.Send("You are not allowed to edit the details of that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name do you wish to give to the clan?");
            return;
        }

        string newNameText = command.PopSpeech();
        if (
            actor.Gameworld.Clans.Any(
                x => x.FullName.Equals(newNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send("There is already a clan that goes by that name. Your new name must be unique.");
            return;
        }

        actor.Gameworld.SystemMessage(
            $"{actor.PersonalName.GetName(NameStyle.SimpleFull)} changes the name of clan {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newNameText.TitleCase().Colour(Telnet.Green)}.",
            true);

        actor.OutputHandler.Send(
            $"You change the name of {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newNameText.TitleCase().Colour(Telnet.Green)}");

        clan.FullName = newNameText;
        clan.Changed = true;
    }

    private static void ClanEditAlias(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanEditClanDetails))
        {
            actor.OutputHandler.Send("You are not allowed to edit the details of that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new alias do you wish to give to the clan?");
            return;
        }

        string newNameText = command.PopSpeech();
        if (actor.Gameworld.Clans.Any(x => x.Alias.Equals(newNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a clan that goes by that alias. Your new alias must be unique.");
            return;
        }

        actor.Gameworld.SystemMessage(
            $"{actor.PersonalName.GetName(NameStyle.SimpleFull)} changes the alias of clan {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newNameText.Colour(Telnet.Green)}.",
            true);

        actor.OutputHandler.Send(
            $"You change the alias of {clan.FullName.TitleCase().Colour(Telnet.Green)} to {newNameText.Colour(Telnet.Green)}");

        clan.Alias = newNameText;
        clan.Changed = true;
    }

    private static void PostClanEditDescription(string text, IOutputHandler handler, object[] objects)
    {
        IClan clan = (IClan)objects.ElementAt(0);
        clan.Description = text;
        clan.Changed = true;
        handler.Send(
            $"You change the description of clan {clan.FullName.TitleCase().Colour(Telnet.Green)} to:\n{text.Wrap(80, "\t")}");
    }

    private static void CancelClanEditDescription(IOutputHandler handler, object[] objects)
    {
        handler.Send("You decline to edit the description of the clan.");
    }

    private static void ClanEditDescription(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanEditClanDetails))
        {
            actor.OutputHandler.Send("You are not allowed to edit the details of that clan.");
            return;
        }

        actor.OutputHandler.Send(
            $"Replacing:\n{clan.Description.Wrap(80, "\t")}\n\nEnter your description in the editor below.");
        actor.EditorMode(PostClanEditDescription, CancelClanEditDescription, 1.0, null, EditorOptions.None,
            new object[] { clan });
    }

    private static void ClanEditTemplate(ICharacter actor, StringStack command, IClan clan)
    {
        if (!actor.IsAdministrator())
        {
            actor.OutputHandler.Send("Only administrators can flag a clan to be a template clan.");
            return;
        }

        clan.IsTemplate ^= clan.IsTemplate;
        clan.Changed = true;
        actor.Send(clan.IsTemplate ? "{0} will now be a template clan." : "{0} will no longer be a template clan.",
            clan.FullName.TitleCase().Colour(Telnet.Green));
    }

    private static void ClanEditPayday(ICharacter actor, StringStack command, IClan clan)
    {
        actor.OutputHandler.Send("Coming soon.");
    }

    #region ClanEditRank SubCommands

    private static void ClanEditRankName(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name do you want to give that rank?");
            return;
        }

        string newNameText = command.PopSpeech();
        if (clan.Ranks.Any(x => x.Name.Equals(newNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send("There is already a rank with that name. New rank names must be unique.");
            return;
        }

        actor.OutputHandler.Send(
            $"You rename the {rank.Name.TitleCase().Colour(Telnet.Green)} rank to {newNameText.TitleCase().Colour(Telnet.Green)}.");
        rank.SetName(newNameText);
    }

    private static void ClanEditRankReorder(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to move this rank before or after another rank?");
            return;
        }

        bool before;
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "before":
                before = true;
                break;
            case "after":
                before = false;
                break;
            default:
                actor.OutputHandler.Send("Do you want to move this rank before or after another rank?");
                return;
        }

        if (command.IsFinished)
        {
            actor.Send("Which other rank do you want to move this rank {0}?", before ? "before" : "after");
            return;
        }

        string rankText = command.PopSpeech();
        IRank otherRank = long.TryParse(rankText, out long value)
            ? clan.Ranks.FirstOrDefault(x => x.Id == value)
            : clan.Ranks.FirstOrDefault(x => x.Name.Equals(rankText, StringComparison.InvariantCultureIgnoreCase));
        if (otherRank == null)
        {
            actor.Send("There is no such rank that you can move this rank {0}.", before ? "before" : "after");
            return;
        }

        if (otherRank == rank)
        {
            actor.OutputHandler.Send("You cannot move a rank before or after itself.");
            return;
        }

        List<IRank> ranksAfter =
            (before
                ? clan.Ranks.OrderBy(x => x.RankNumber).SkipWhile(x => x != otherRank)
                : clan.Ranks.OrderBy(x => x.RankNumber).SkipWhile(x => x != otherRank).Skip(1)).Except(rank).ToList();
        int newRankNumber = ranksAfter.Any()
            ? ranksAfter.Min(x => x.RankNumber)
            : clan.Ranks.Max(x => x.RankNumber) + 1;
        foreach (IRank item in ranksAfter)
        {
            item.RankNumber++;
            item.Changed = true;
        }

        rank.RankNumber = newRankNumber;
        rank.Changed = true;
        clan.Ranks.Sort((x, y) => x.RankNumber.CompareTo(y.RankNumber));
        clan.Changed = true;
        actor.OutputHandler.Send(
            $"You move rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)} to be {(before ? "before" : "after")} {otherRank.Name.TitleCase().Colour(Telnet.Green)}.");
    }

    private static void ClanEditRankPaygrade(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add or remove a paygrade from that rank?");
            return;
        }

        bool add;
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                add = true;
                break;
            case "remove":
                add = false;
                break;
            default:
                actor.OutputHandler.Send("Do you want to add or remove a paygrade from that rank?");
                return;
        }

        if (command.IsFinished)
        {
            actor.Send("Which paygrade do you want to {0} that rank?", add ? "add to" : "remove from");
            return;
        }

        IPaygrade paygrade = long.TryParse(command.PopSpeech(), out long value)
            ? clan.Paygrades.FirstOrDefault(x => x.Id == value)
            : clan.Paygrades.FirstOrDefault(
                  x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase)) ??
              clan.Paygrades.FirstOrDefault(
                  x => x.Abbreviation.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (paygrade == null)
        {
            actor.Send("There is no such paygrade to {0} that clan.", add ? "add to" : "remove from");
            return;
        }

        if (add && rank.Paygrades.Contains(paygrade))
        {
            actor.OutputHandler.Send("That rank already contains that paygrade.");
            return;
        }

        if (!add && !rank.Paygrades.Contains(paygrade))
        {
            actor.OutputHandler.Send("That rank does not contain that paygrade.");
            return;
        }

        if (add)
        {
            rank.Paygrades.Add(paygrade);
            rank.Paygrades.Sort((x, y) => x.PayAmount.CompareTo(y.PayAmount));
            actor.OutputHandler.Send(
                $"You add the {paygrade.Name.TitleCase().Colour(Telnet.Green)} paygrade to the {rank.Name.TitleCase().Colour(Telnet.Green)} rank.");
        }
        else
        {
            rank.Paygrades.Remove(paygrade);
            actor.OutputHandler.Send(
                $"You remove the {paygrade.Name.TitleCase().Colour(Telnet.Green)} paygrade from the {rank.Name.TitleCase().Colour(Telnet.Green)} rank.");
        }

        rank.Changed = true;
    }

    private static void ClanEditRankAbbreviation(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add, remove, reorder or set a rank abbreviation?");
            return;
        }

        string actionText = command.PopSpeech().ToLowerInvariant();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which abbreviation are you referring to?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        switch (actionText)
        {
            case "add":
                IFutureProg prog = null;
                if (!command.IsFinished)
                {
                    prog = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (prog == null)
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    if (!prog.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }
                }

                if (
                    rank.Abbreviations.Any(
                        x => x.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase)))
                {
                    actor.OutputHandler.Send(
                        "There is already an abbreviation like that for that rank. Abbreviations must be unique.");
                    return;
                }

                rank.AbbreviationsAndProgs.Add(Tuple.Create(prog, abbreviationText));
                actor.OutputHandler.Send(
                    $"You add the abbreviation {abbreviationText.Colour(Telnet.Green)} to rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}{(prog != null ? $" with prog {prog.FunctionName} (#{prog.Id})" : "")}.");
                rank.Changed = true;
                return;
            case "remove":
                Tuple<IFutureProg, string> target =
                    rank.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (target == null)
                {
                    actor.OutputHandler.Send("There is no such abbreviation for that rank which you can remove.");
                    return;
                }

                if (rank.AbbreviationsAndProgs.Count == 1)
                {
                    actor.OutputHandler.Send("You cannot delete the last abbreviation for a rank.");
                    return;
                }

                rank.AbbreviationsAndProgs.Remove(target);
                rank.Changed = true;
                actor.OutputHandler.Send(
                    $"You remove the abbreviation {target.Item2.Colour(Telnet.Green)} from rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "reorder":
                Tuple<IFutureProg, string> targetAbbreviation2 =
                    rank.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation2 == null)
                {
                    actor.OutputHandler.Send("There is no such abbreviation for that rank which you can reorder.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What position do you want to move this abbreviation to?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int value2))
                {
                    actor.OutputHandler.Send("What position do you want to move this abbreviation to?");
                    return;
                }

                if (value2 < 0 || value2 > rank.AbbreviationsAndProgs.Count)
                {
                    actor.OutputHandler.Send("That is not a valid position to move that abbreviation to.");
                    return;
                }

                rank.AbbreviationsAndProgs.Remove(targetAbbreviation2);
                rank.AbbreviationsAndProgs.Insert(value2, targetAbbreviation2);
                rank.Changed = true;
                actor.OutputHandler.Send(
                    $"You move the abbreviation {targetAbbreviation2.Item2.Colour(Telnet.Green)} to the {value2.ToOrdinal()} position for rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "set":
                Tuple<IFutureProg, string> targetAbbreviation =
                    rank.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation == null)
                {
                    actor.OutputHandler.Send(
                        "There is no such abbreviation for that rank which you can set a prog for.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "Do you want to clear the futureprog for that abbreviation, or set a specific one?");
                    return;
                }

                string progText = command.PopSpeech();
                if (progText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    int oldPosition = rank.AbbreviationsAndProgs.IndexOf(targetAbbreviation);
                    rank.AbbreviationsAndProgs.Remove(targetAbbreviation);
                    rank.AbbreviationsAndProgs.Insert(oldPosition,
                        Tuple.Create((IFutureProg)null, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        $"You clear the prog from abbreviation {targetAbbreviation.Item2.Colour(Telnet.Green)} for rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                }
                else
                {
                    IFutureProg newProg = null;
                    newProg = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (newProg == null)
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    if (!newProg.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    int oldPosition = rank.AbbreviationsAndProgs.IndexOf(targetAbbreviation);
                    rank.AbbreviationsAndProgs.Remove(targetAbbreviation);
                    rank.AbbreviationsAndProgs.Insert(oldPosition, Tuple.Create(newProg, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        string.Format("You add the prog {3} to abbreviation {0} for rank {1} in {2}.",
                            targetAbbreviation.Item2.Colour(Telnet.Green),
                            rank.Name.TitleCase().Colour(Telnet.Green),
                            clan.FullName.TitleCase().Colour(Telnet.Green),
                            newProg.FunctionName.Colour(Telnet.Cyan)
                        ));
                }

                rank.Changed = true;
                return;

            default:
                actor.OutputHandler.Send("Do you want to add, remove, reorder or set a rank abbreviation?");
                return;
        }
    }

    private static void ClanEditRankTitle(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add, remove, reorder or set a rank title?");
            return;
        }

        string actionText = command.PopSpeech().ToLowerInvariant();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which title are you referring to?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        switch (actionText)
        {
            case "add":
                IFutureProg prog = null;
                if (!command.IsFinished)
                {
                    prog = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (prog == null)
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }

                    if (!prog.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }
                }

                if (rank.Titles.Any(x => x.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase)))
                {
                    actor.OutputHandler.Send(
                        "There is already a title like that for that rank. Titles must be unique.");
                    return;
                }

                rank.TitlesAndProgs.Add(Tuple.Create(prog, abbreviationText));
                actor.OutputHandler.Send(
                    $"You add the title {abbreviationText.Colour(Telnet.Green)} to rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}{(prog != null ? $" with prog {prog.FunctionName} (#{prog.Id})" : "")}.");
                rank.Changed = true;
                return;
            case "remove":
                Tuple<IFutureProg, string> target =
                    rank.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (target == null)
                {
                    actor.OutputHandler.Send("There is no such title for that rank which you can remove.");
                    return;
                }

                if (rank.TitlesAndProgs.Count == 1)
                {
                    actor.OutputHandler.Send("You cannot delete the last title for a rank.");
                    return;
                }

                rank.TitlesAndProgs.Remove(target);
                rank.Changed = true;
                actor.OutputHandler.Send(
                    $"You remove the title {target.Item2.Colour(Telnet.Green)} from rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "reorder":
                Tuple<IFutureProg, string> targetAbbreviation2 =
                    rank.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation2 == null)
                {
                    actor.OutputHandler.Send("There is no such title for that rank which you can reorder.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What position do you want to move this title to?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int value2))
                {
                    actor.OutputHandler.Send("What position do you want to move this title to?");
                    return;
                }

                if (value2 < 0 || value2 >= rank.TitlesAndProgs.Count)
                {
                    actor.OutputHandler.Send("That is not a valid position to move that title to.");
                    return;
                }

                rank.TitlesAndProgs.Remove(targetAbbreviation2);
                rank.TitlesAndProgs.Insert(value2, targetAbbreviation2);
                rank.Changed = true;
                actor.OutputHandler.Send(
                    $"You move the title {targetAbbreviation2.Item2.Colour(Telnet.Green)} to the {value2.ToOrdinal()} position for rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "set":
                Tuple<IFutureProg, string> targetAbbreviation =
                    rank.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation == null)
                {
                    actor.OutputHandler.Send("There is no such title for that rank which you can set a prog for.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("Do you want to clear the prog for that title, or set a specific one?");
                    return;
                }

                string progText = command.PopSpeech();
                if (progText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    int oldPosition = rank.TitlesAndProgs.IndexOf(targetAbbreviation);
                    rank.TitlesAndProgs.Remove(targetAbbreviation);
                    rank.TitlesAndProgs.Insert(oldPosition,
                        Tuple.Create((IFutureProg)null, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        $"You clear the prog from title {targetAbbreviation.Item2.Colour(Telnet.Green)} for rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                }
                else
                {
                    IFutureProg newProg = null;
                    newProg = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (newProg == null)
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    if (!newProg.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    int oldPosition = rank.TitlesAndProgs.IndexOf(targetAbbreviation);
                    rank.TitlesAndProgs.Remove(targetAbbreviation);
                    rank.TitlesAndProgs.Insert(oldPosition, Tuple.Create(newProg, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(string.Format("You add the prog {3} to title {0} for rank {1} in {2}.",
                        targetAbbreviation.Item2.Colour(Telnet.Green),
                        rank.Name.TitleCase().Colour(Telnet.Green),
                        clan.FullName.TitleCase().Colour(Telnet.Green),
                        newProg.FunctionName.Colour(Telnet.Cyan)
                    ));
                }

                rank.Changed = true;
                return;

            default:
                actor.OutputHandler.Send("Do you want to add, remove, reorder or set a rank title?");
                return;
        }
    }

    private static void ClanEditRankFame(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What should be the fame type of this rank? The valid options are {Enum.GetNames<ClanFameType>().Select(x => x.ColourValue()).ListToString()}.");
            return;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<ClanFameType>(out ClanFameType fame))
        {
            actor.OutputHandler.Send(
                $"That is not a valid fame type. The valid options are {Enum.GetNames<ClanFameType>().Select(x => x.ColourValue()).ListToString()}.");
            return;
        }

        rank.FameType = fame;
        rank.Changed = true;
        actor.OutputHandler.Send($"This rank will now have a fame level of {fame.DescribeEnum(true).ColourValue()}.");
    }

    private static void ClanEditRankPrivileges(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add or remove privileges from this rank?");
            return;
        }

        bool add;
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                add = true;
                break;
            case "remove":
                add = false;
                break;
            default:
                actor.OutputHandler.Send("Do you want to add or remove privileges from this rank?");
                return;
        }

        ClanPrivilegeType privileges = ClanPrivilegeType.None;
        while (!command.IsFinished)
        {
            string text = command.PopSpeech();
            if (!text.TryParseEnum<ClanPrivilegeType>(out ClanPrivilegeType privilege))
            {
                actor.Send("There is no such privilege as {0}.", text);
                actor.OutputHandler.Send(
                    $"The valid choices are as follows:\n{Enum.GetValues(typeof(ClanPrivilegeType)).OfType<ClanPrivilegeType>().Select(x => $"\t{x.DescribeEnum().ColourValue()}").ListToCommaSeparatedValues("\n")}");
                return;
            }

            privileges |= privilege;
        }

        if (privileges == ClanPrivilegeType.None)
        {
            actor.Send("Which privileges did you want to {0} this rank?", add ? "add to" : "remove from");
            actor.OutputHandler.Send(
                $"The valid choices are as follows:\n{Enum.GetValues(typeof(ClanPrivilegeType)).OfType<ClanPrivilegeType>().Select(x => $"\t{x.DescribeEnum().ColourValue()}").ListToCommaSeparatedValues("\n")}");
            return;
        }

        if (add)
        {
            rank.Privileges |= privileges;
            actor.OutputHandler.Send(
                $"You add privileges {privileges} to rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }
        else
        {
            rank.Privileges &= ~privileges;
            actor.OutputHandler.Send(
                $"You remove privileges {privileges} from rank {rank.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }

        rank.Changed = true;
    }

    private static void ClanEditRankPath(ICharacter actor, StringStack command, IClan clan, IRank rank)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new rank path do you want to give that rank?");
            return;
        }

        string newNameText = command.PopSpeech();
        if (newNameText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            rank.RankPath = string.Empty;
            actor.Send("You clear the rank path from the {0} rank.", rank.Name.TitleCase().Colour(Telnet.Green));
        }
        else
        {
            actor.OutputHandler.Send(
                $"You rename the {rank.Name.TitleCase().Colour(Telnet.Green)} rank to {newNameText.TitleCase().Colour(Telnet.Green)}.");
            rank.RankPath = newNameText;
        }

        rank.Changed = true;
    }

    #endregion ClanEditRank SubCommands

    private static string _clanRankHelp = @"These are the valid options for this subcommand:

	#3name <name>#0 - renames this rank
	#3path <path>|clear#0 - sets or clears the path of this rank
	#3fame <fame>#0 - sets the fame level for this rank
	#3reorder before|after <other>#0 - swaps the order of this rank in the rank tree
	#3privileges add <which>#0 - adds privileges to this rank
	#3privileges remove <which>#0 - removes privileges from this rank
	#3paygrade add <which>#0 - adds a paygrade for this rank
	#3paygrade remove <which>#0 - removes a paygrade from this rank
	#3title add <prog> <title>#0 - adds a new title with a specified prog
	#3title remove <title>#0 - removes a title
	#3title swap <one> <two>#0 - swaps the evaluation order of two titles
	#3title set <which> <prog>#0 - changes the prog for a particular title
	#3abbreviation add <prog> <title>#0 - adds a new abbreviation with a specified prog
	#3abbreviation remove <title>#0 - removes an abbreviation
	#3abbreviation swap <one> <two>#0 - swaps the evaluation order of two abbreviations
	#3abbreviation set <which> <prog>#0 - changes the prog for a particular abbreviation".SubstituteANSIColour();

    private static void ClanEditRank(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator() && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateRanks))
        {
            actor.OutputHandler.Send("You do not have permission to edit ranks in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which rank do you want to edit?");
            return;
        }

        string targetText = command.PopSpeech();
        IRank rank =
            clan.Ranks.FirstOrDefault(x => x.Name.Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
        if (rank == null)
        {
            actor.OutputHandler.Send("There is no such rank for you to edit.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(_clanRankHelp);
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                ClanEditRankName(actor, command, clan, rank);
                break;
            case "rankpath":
            case "rank path":
            case "path":
                ClanEditRankPath(actor, command, clan, rank);
                break;
            case "reorder":
                ClanEditRankReorder(actor, command, clan, rank);
                break;
            case "paygrade":
                ClanEditRankPaygrade(actor, command, clan, rank);
                break;
            case "abbreviation":
                ClanEditRankAbbreviation(actor, command, clan, rank);
                break;
            case "title":
                ClanEditRankTitle(actor, command, clan, rank);
                break;
            case "privileges":
                ClanEditRankPrivileges(actor, command, clan, rank);
                break;
            case "fame":
                ClanEditRankFame(actor, command, clan, rank);
                break;
            default:
                actor.OutputHandler.Send(_clanRankHelp);
                return;
        }
    }

    #region ClanEditAppointment Subcommands

    private static void ClanEditAppointmentFame(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                $"What should be the fame type of this appointment? The valid options are {Enum.GetNames<ClanFameType>().Select(x => x.ColourValue()).ListToString()}.");
            return;
        }

        if (!command.SafeRemainingArgument.TryParseEnum<ClanFameType>(out ClanFameType fame))
        {
            actor.OutputHandler.Send(
                $"That is not a valid fame type. The valid options are {Enum.GetNames<ClanFameType>().Select(x => x.ColourValue()).ListToString()}.");
            return;
        }

        appointment.FameType = fame;
        appointment.Changed = true;
        actor.OutputHandler.Send(
            $"This appointment will now have a fame level of {fame.DescribeEnum(true).ColourValue()}.");
    }

    private static void ClanEditAppointmentName(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What new name do you want to give that appointment?");
            return;
        }

        string newNameText = command.PopSpeech();
        if (clan.Appointments.Any(x => x.Name.Equals(newNameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a appointment with that name. New appointment names must be unique.");
            return;
        }

        actor.OutputHandler.Send(
            $"You rename the {appointment.Name.TitleCase().Colour(Telnet.Green)} appointment to {newNameText.TitleCase().Colour(Telnet.Green)}.");
        appointment.SetName(newNameText);
    }

    private static void ClanEditAppointmentAbbreviation(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add, remove, reorder or set an appointment abbreviation?");
            return;
        }

        string actionText = command.PopSpeech().ToLowerInvariant();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which abbreviation are you referring to?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        switch (actionText)
        {
            case "add":
                IFutureProg prog = null;
                if (!command.IsFinished)
                {
                    prog = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (prog == null)
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    if (!prog.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }
                }

                if (
                    appointment.Abbreviations.Any(
                        x => x.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase)))
                {
                    actor.OutputHandler.Send(
                        "There is already an abbreviation like that for that appointment. Abbreviations must be unique.");
                    return;
                }

                appointment.AbbreviationsAndProgs.Add(Tuple.Create(prog, abbreviationText));
                actor.OutputHandler.Send(
                    $"You add the abbreviation {abbreviationText.Colour(Telnet.Green)} to appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}{(prog != null ? $" with prog {prog.FunctionName} (#{prog.Id})" : "")}.");
                appointment.Changed = true;
                return;
            case "remove":
                Tuple<IFutureProg, string> target =
                    appointment.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (target == null)
                {
                    actor.OutputHandler.Send(
                        "There is no such abbreviation for that appointment which you can remove.");
                    return;
                }

                if (appointment.AbbreviationsAndProgs.Count == 1)
                {
                    actor.OutputHandler.Send("You cannot delete the last abbreviation for a appointment.");
                    return;
                }

                appointment.AbbreviationsAndProgs.Remove(target);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"You remove the abbreviation {target.Item2.Colour(Telnet.Green)} from appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "swap":
            case "reorder":
                Tuple<IFutureProg, string> targetAbbreviation2 =
                    appointment.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation2 == null)
                {
                    actor.OutputHandler.Send(
                        "There is no such abbreviation for that appointment which you can reorder.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What position do you want to move this abbreviation to?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int value2))
                {
                    actor.OutputHandler.Send("What position do you want to move this abbreviation to?");
                    return;
                }

                if (value2 < 0 || value2 > appointment.AbbreviationsAndProgs.Count)
                {
                    actor.OutputHandler.Send("That is not a valid position to move that abbreviation to.");
                    return;
                }

                appointment.AbbreviationsAndProgs.Remove(targetAbbreviation2);
                appointment.AbbreviationsAndProgs.Insert(value2, targetAbbreviation2);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"You move the abbreviation {targetAbbreviation2.Item2.Colour(Telnet.Green)} to the {value2.ToOrdinal()} position for appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "set":
                Tuple<IFutureProg, string> targetAbbreviation =
                    appointment.AbbreviationsAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation == null)
                {
                    actor.OutputHandler.Send(
                        "There is no such abbreviation for that rank which you can set a prog for.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "Do you want to clear the futureprog for that abbreviation, or set a specific one?");
                    return;
                }

                string progText = command.PopSpeech();
                if (progText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    int oldPosition = appointment.AbbreviationsAndProgs.IndexOf(targetAbbreviation);
                    appointment.AbbreviationsAndProgs.Remove(targetAbbreviation);
                    appointment.AbbreviationsAndProgs.Insert(oldPosition,
                        Tuple.Create((IFutureProg)null, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        $"You clear the prog from abbreviation {targetAbbreviation.Item2.Colour(Telnet.Green)} for appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                }
                else
                {
                    IFutureProg newProg = null;
                    newProg = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (newProg == null)
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    if (!newProg.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such Futureprog.");
                        return;
                    }

                    int oldPosition = appointment.AbbreviationsAndProgs.IndexOf(targetAbbreviation);
                    appointment.AbbreviationsAndProgs.Remove(targetAbbreviation);
                    appointment.AbbreviationsAndProgs.Insert(oldPosition,
                        Tuple.Create(newProg, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        string.Format("You add the prog {3} to abbreviation {0} for appointment {1} in {2}.",
                            targetAbbreviation.Item2.Colour(Telnet.Green),
                            appointment.Name.TitleCase().Colour(Telnet.Green),
                            clan.FullName.TitleCase().Colour(Telnet.Green),
                            newProg.FunctionName.Colour(Telnet.Cyan)
                        ));
                }

                appointment.Changed = true;
                return;

            default:
                actor.OutputHandler.Send("Do you want to add, remove, reorder or set an appointment abbreviation?");
                return;
        }
    }

    private static void ClanEditAppointmentTitle(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add, remove, reorder or set an appointment title?");
            return;
        }

        string actionText = command.PopSpeech().ToLowerInvariant();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which title are you referring to?");
            return;
        }

        string abbreviationText = command.PopSpeech();

        switch (actionText)
        {
            case "add":
                IFutureProg prog = null;
                if (!command.IsFinished)
                {
                    prog = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (prog == null)
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }

                    if (!prog.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }
                }

                if (
                    appointment.Titles.Any(
                        x => x.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase)))
                {
                    actor.OutputHandler.Send(
                        "There is already a title like that for that appointment. Titles must be unique.");
                    return;
                }

                appointment.TitlesAndProgs.Add(Tuple.Create(prog, abbreviationText));
                actor.OutputHandler.Send(
                    $"You add the title {abbreviationText.Colour(Telnet.Green)} to appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}{(prog != null ? $" with prog {prog.FunctionName} (#{prog.Id})" : "")}.");
                appointment.Changed = true;
                return;
            case "remove":
                Tuple<IFutureProg, string> target =
                    appointment.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (target == null)
                {
                    actor.OutputHandler.Send("There is no such title for that appointment which you can remove.");
                    return;
                }

                if (appointment.TitlesAndProgs.Count == 1)
                {
                    actor.OutputHandler.Send("You cannot delete the last title for a appointment.");
                    return;
                }

                appointment.TitlesAndProgs.Remove(target);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"You remove the title {target.Item2.Colour(Telnet.Green)} from appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "swap":
            case "reorder":
                Tuple<IFutureProg, string> targetAbbreviation2 =
                    appointment.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation2 == null)
                {
                    actor.OutputHandler.Send("There is no such title for that appointment which you can reorder.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("What position do you want to move this title to?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int value2))
                {
                    actor.OutputHandler.Send("What position do you want to move this title to?");
                    return;
                }

                if (value2 < 0 || value2 > appointment.TitlesAndProgs.Count)
                {
                    actor.OutputHandler.Send("That is not a valid position to move that address to.");
                    return;
                }

                appointment.TitlesAndProgs.Remove(targetAbbreviation2);
                appointment.TitlesAndProgs.Insert(value2, targetAbbreviation2);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"You move the title {targetAbbreviation2.Item2.Colour(Telnet.Green)} to the {value2.ToOrdinal()} position for appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                return;
            case "set":
                Tuple<IFutureProg, string> targetAbbreviation =
                    appointment.TitlesAndProgs.FirstOrDefault(
                        x => x.Item2.Equals(abbreviationText, StringComparison.InvariantCultureIgnoreCase));
                if (targetAbbreviation == null)
                {
                    actor.OutputHandler.Send("There is no such title for that rank which you can set a prog for.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("Do you want to clear the prog for that title, or set a specific one?");
                    return;
                }

                string progText = command.PopSpeech();
                if (progText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
                {
                    int oldPosition = appointment.TitlesAndProgs.IndexOf(targetAbbreviation);
                    appointment.TitlesAndProgs.Remove(targetAbbreviation);
                    appointment.TitlesAndProgs.Insert(oldPosition,
                        Tuple.Create((IFutureProg)null, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        $"You clear the prog from title {targetAbbreviation.Item2.Colour(Telnet.Green)} for appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
                }
                else
                {
                    IFutureProg newProg = null;
                    newProg = long.TryParse(command.PopSpeech(), out long value)
                        ? actor.Gameworld.FutureProgs.Get(value)
                        : actor.Gameworld.FutureProgs.FirstOrDefault(
                            x => x.FunctionText.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
                    if (newProg == null)
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }

                    if (!newProg.Public && !actor.IsAdministrator())
                    {
                        actor.OutputHandler.Send("There is no such prog.");
                        return;
                    }

                    int oldPosition = appointment.TitlesAndProgs.IndexOf(targetAbbreviation);
                    appointment.TitlesAndProgs.Remove(targetAbbreviation);
                    appointment.TitlesAndProgs.Insert(oldPosition, Tuple.Create(newProg, targetAbbreviation.Item2));
                    actor.OutputHandler.Send(
                        string.Format("You add the prog {3} to title {0} for appointment {1} in {2}.",
                            targetAbbreviation.Item2.Colour(Telnet.Green),
                            appointment.Name.TitleCase().Colour(Telnet.Green),
                            clan.FullName.TitleCase().Colour(Telnet.Green),
                            newProg.FunctionName.Colour(Telnet.Cyan)
                        ));
                }

                appointment.Changed = true;
                return;

            default:
                actor.OutputHandler.Send("Do you want to add, remove, reorder or set an appointment title?");
                return;
        }
    }

    private static void ClanEditAppointmentPaygrade(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which paygrade do you want to give to that appointment?");
            return;
        }

        string paygradeText = command.PopSpeech();
        if (paygradeText.Equals("clear", StringComparison.InvariantCultureIgnoreCase) ||
            paygradeText.Equals("none", StringComparison.InvariantCultureIgnoreCase))
        {
            if (appointment.Paygrade == null)
            {
                actor.OutputHandler.Send("That appointment does not currently have any paygrade to clear.");
                return;
            }

            actor.OutputHandler.Send(
                $"You clear the paygrade from appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
            appointment.Paygrade = null;
            appointment.Changed = true;
            return;
        }

        IPaygrade targetPaygrade =
            clan.Paygrades.FirstOrDefault(
                x => x.Name.Equals(paygradeText, StringComparison.InvariantCultureIgnoreCase)) ??
            clan.Paygrades.FirstOrDefault(
                x => x.Abbreviation.Equals(paygradeText, StringComparison.InvariantCultureIgnoreCase));
        if (targetPaygrade == null)
        {
            actor.OutputHandler.Send("There is no such paygrade to give to this appointment.");
            return;
        }

        if (appointment.Paygrade == targetPaygrade)
        {
            actor.OutputHandler.Send("The appointment already has that paygrade.");
            return;
        }

        actor.OutputHandler.Send(
            $"You change the {appointment.Name.TitleCase().Colour(Telnet.Green)} appointment in {clan.FullName.TitleCase().Colour(Telnet.Green)} to have a paygrade of {targetPaygrade.Name.TitleCase().Colour(Telnet.Green)}.");
        appointment.Paygrade = targetPaygrade;
        appointment.Changed = true;
    }

    private static void ClanEditAppointmentMinRank(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which minimum rank do you want to assign to that appoinment?");
            return;
        }

        string rankText = command.PopSpeech();
        if (rankText.Equals("none", StringComparison.InvariantCultureIgnoreCase) ||
            rankText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            if (appointment.MinimumRankToHold == null)
            {
                actor.OutputHandler.Send("That appointment does not have a minimum rank to clear.");
                return;
            }

            actor.OutputHandler.Send(
                $"You clear the minimum rank requirement from appoint {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
            appointment.MinimumRankToHold = null;
            appointment.Changed = true;
            return;
        }

        IRank rank =
            clan.Ranks.FirstOrDefault(x => x.Name.Equals(rankText, StringComparison.InvariantCultureIgnoreCase));
        if (rank == null)
        {
            actor.OutputHandler.Send("There is no such rank to assign as a minimum rank for that appointment.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin))
        {
            IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
            if (actorMembership != null &&
                !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
                actorMembership.Rank.RankNumber < rank.RankNumber)
            {
                actor.OutputHandler.Send(
                    "Your permissions in this clan are insufficient to create a rank requirement higher than your own on any child position.");
                return;
            }
        }

        if (appointment.MinimumRankToHold == rank)
        {
            actor.OutputHandler.Send("That appointment already has the same minimum rank requirement.");
            return;
        }

        actor.OutputHandler.Send(
            $"You set the minimum rank required to hold the {appointment.Name.TitleCase().Colour(Telnet.Green)} appointment in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {rank.Name.TitleCase().Colour(Telnet.Green)}.");
        appointment.MinimumRankToHold = rank;
        appointment.Changed = true;
    }

    private static void ClanEditAppointmentMinAppointer(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which minimum appointer rank do you want to assign to that appoinment?");
            return;
        }

        string rankText = command.PopSpeech();
        if (rankText.Equals("none", StringComparison.InvariantCultureIgnoreCase) ||
            rankText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            if (appointment.MinimumRankToAppoint == null)
            {
                actor.OutputHandler.Send("That appointment does not have a minimum appointer rank to clear.");
                return;
            }

            actor.OutputHandler.Send(
                $"You clear the minimum appointer rank requirement from appoint {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
            appointment.MinimumRankToAppoint = null;
            appointment.Changed = true;
            return;
        }

        IRank rank =
            clan.Ranks.FirstOrDefault(x => x.Name.Equals(rankText, StringComparison.InvariantCultureIgnoreCase));
        if (rank == null)
        {
            actor.OutputHandler.Send(
                "There is no such rank to assign as a minimum appointer rank for that appointment.");
            return;
        }

        if (!actor.IsAdministrator(PermissionLevel.Admin))
        {
            IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
            if (actorMembership != null &&
                !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
                actorMembership.Rank.RankNumber < rank.RankNumber)
            {
                actor.OutputHandler.Send(
                    "Your permissions in this clan are insufficient to create an appointer rank requirement higher than your own on any child position.");
                return;
            }
        }

        if (appointment.MinimumRankToAppoint == rank)
        {
            actor.OutputHandler.Send("That appointment already has the same minimum appointer rank requirement.");
            return;
        }

        actor.OutputHandler.Send(
            $"You set the minimum appointer rank required to hold the {appointment.Name.TitleCase().Colour(Telnet.Green)} appointment in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {rank.Name.TitleCase().Colour(Telnet.Green)}.");
        appointment.MinimumRankToAppoint = rank;
        appointment.Changed = true;
    }

    private static void ClanEditAppointmentParent(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which parent appointment do you want to assign to that appoinment?");
            return;
        }

        string appointmentText = command.PopSpeech();
        if (appointmentText.Equals("none", StringComparison.InvariantCultureIgnoreCase) ||
            appointmentText.Equals("clear", StringComparison.InvariantCultureIgnoreCase))
        {
            if (appointment.ParentPosition == null)
            {
                actor.OutputHandler.Send("That appointment does not have a parent appointment to clear.");
                return;
            }

            actor.OutputHandler.Send(
                $"You clear the parent appointment from appoint {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
            appointment.ParentPosition = null;
            appointment.Changed = true;
            return;
        }

        IAppointment targetAppointment =
            clan.Appointments.FirstOrDefault(
                x => x.Name.Equals(appointmentText, StringComparison.InvariantCultureIgnoreCase));
        if (targetAppointment == null)
        {
            actor.OutputHandler.Send("There is no such appointment to assign as the parent of that appointment.");
            return;
        }

        if (targetAppointment == appointment)
        {
            actor.OutputHandler.Send("You cannot assign an appointment as its own parent.");
            return;
        }

        actor.OutputHandler.Send(
            $"You assign appointment {targetAppointment.Name.TitleCase().Colour(Telnet.Green)} as the parent of appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        appointment.ParentPosition = targetAppointment;
        appointment.Changed = true;
    }

    private static void ClanEditAppointmentMaxHolders(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How many members should that appointment be allowed at any one time?");
            return;
        }

        if (!int.TryParse(command.PopSpeech(), out int value))
        {
            actor.OutputHandler.Send(
                "You must specify a number of simultaneous members for that appointment. Use 0 for unlimited.");
            return;
        }

        if (value < 1)
        {
            if (appointment.MaximumSimultaneousHolders < 1)
            {
                actor.OutputHandler.Send(
                    "There is no restriction on that appointment's number of simultaneous members to remove.");
                return;
            }

            actor.OutputHandler.Send(
                $"The appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)} may now have an unlimited number of members at any time.");
            appointment.MaximumSimultaneousHolders = 0;
            appointment.Changed = true;
            return;
        }

        // TODO - living members
        if (value < clan.Memberships.Count(x => x.Appointments.Contains(appointment)))
        {
            actor.OutputHandler.Send(
                "There are presently more holders of that appointment than the new limit. Dismiss some first.");
            return;
        }

        if (value <
            clan.Memberships.Count(x => x.Appointments.Contains(appointment)) +
            clan.ExternalControls.Sum(
                x => x.ControlledAppointment == appointment ? x.NumberOfAppointments - x.Appointees.Count : 0))
        {
            actor.OutputHandler.Send("Making that change would violate the appointment rights of a liege clan.");
            return;
        }

        actor.OutputHandler.Send(string.Format(actor,
            "The appointment {0} in {1} may now have only {2:N0} members at a time.",
            appointment.Name.TitleCase().Colour(Telnet.Green),
            clan.FullName.TitleCase().Colour(Telnet.Green),
            value
        ));
        appointment.MaximumSimultaneousHolders = value;
        appointment.Changed = true;
    }

    private static void ClanEditAppointmentPrivileges(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Do you want to add or remove privileges from this appointment?");
            return;
        }

        bool add;
        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "add":
                add = true;
                break;
            case "remove":
                add = false;
                break;
            default:
                actor.OutputHandler.Send("Do you want to add or remove privileges from this appointment?");
                return;
        }

        ClanPrivilegeType privileges = ClanPrivilegeType.None;
        while (!command.IsFinished)
        {
            string text = command.PopSpeech();
            if (!text.TryParseEnum<ClanPrivilegeType>(out ClanPrivilegeType privilege))
            {
                actor.OutputHandler.Send(
                    $"There is no such privilege as {text}.\nThe valid choices are:{ClanPrivilegeType.All.GetSingleFlags().Select(x => x.DescribeEnum().ColourValue()).ListToLines(true)}");
                return;
            }

            privileges |= privilege;
        }

        if (privileges == ClanPrivilegeType.None)
        {
            actor.Send("Which privileges did you want to {0} this appointment?", add ? "add to" : "remove from");
            return;
        }

        if (add)
        {
            appointment.Privileges |= privileges;
            actor.OutputHandler.Send(
                $"You add privileges {privileges} to appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }
        else
        {
            appointment.Privileges &= ~privileges;
            actor.OutputHandler.Send(
                $"You remove privileges {privileges} from appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)}.");
        }

        appointment.Changed = true;
    }

    private static void ClanEditAppointmentElections(ICharacter actor, StringStack command, IClan clan,
        IAppointment appointment)
    {
        string helpText =
            "You may use the following options:\n\tenable - enables elections\n\tnone - removes elections\n\tsecret - toggles secret voting\n\tterm <days> - sets the term length\n\tlead <days> - sets the number of days after votes closed that appointments happen\n\tnomination <days> - sets the length of the nomination period\n\tvoting <days> - sets the length of the voting period\n\tterms <consecutive no|0> <total no|0> - sets term limits\n\tnominate <prog> <why> - sets the nomination eligablity prog and \"why\" fail message prog\n\tvote <prog> - sets the vote count prog";
        if (command.IsFinished)
        {
            actor.OutputHandler.Send(helpText);
            return;
        }

        string text = command.PopSpeech().ToLowerInvariant();
        if (!appointment.IsAppointedByElection)
        {
            if (!text.EqualTo("enable"))
            {
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment in the {clan.FullName.ColourName()} clan is not currently controlled by elections. The only option you can use with this command is ENABLE.");
                return;
            }

            int term = command.IsFinished ? 365 :
                int.TryParse(command.PopSpeech(), out int days) && days > 0 ? days : 365;

            appointment.IsAppointedByElection = true;
            appointment.ElectionTerm = TimeSpan.FromDays(term);
            appointment.ElectionLeadTime = TimeSpan.FromDays(7);
            appointment.NominationPeriod = TimeSpan.FromDays(7);
            appointment.VotingPeriod = TimeSpan.FromDays(7);
            appointment.NumberOfVotesProg = null;
            appointment.CanNominateProg = null;
            appointment.MaximumConsecutiveTerms = 0;
            appointment.MaximumTotalTerms = 0;
            appointment.IsSecretBallot = false;
            if (appointment.MaximumSimultaneousHolders <= 0)
            {
                appointment.MaximumSimultaneousHolders = 1;
            }

            appointment.Changed = true;
            actor.OutputHandler.Send(
                $"The {appointment.Name.ColourName()} appointment is now controlled by elections.");
            Community.Election byElection =
                new(appointment, true, appointment.MaximumSimultaneousHolders, null);
            appointment.AddElection(byElection);
            appointment.AddElection(new Community.Election(appointment, false, appointment.MaximumSimultaneousHolders,
                byElection.ResultsInEffectDate + appointment.ElectionTerm));
            return;
        }

        switch (text)
        {
            case "none":
                appointment.IsAppointedByElection = false;
                appointment.ElectionTerm = TimeSpan.Zero;
                appointment.ElectionLeadTime = TimeSpan.Zero;
                appointment.NominationPeriod = TimeSpan.Zero;
                appointment.VotingPeriod = TimeSpan.Zero;
                appointment.NumberOfVotesProg = null;
                appointment.CanNominateProg = null;
                appointment.MaximumConsecutiveTerms = 0;
                appointment.MaximumTotalTerms = 0;
                appointment.IsSecretBallot = false;
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment is no longer controlled by elections.");
                foreach (IElection election in appointment.Elections.Where(x => !x.IsFinalised).ToArray())
                {
                    election.CancelElection();
                }

                return;
            case "secret":
                appointment.IsSecretBallot = !appointment.IsSecretBallot;
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment election is {(appointment.IsSecretBallot ? "now" : "no longer")} a secret ballot.");
                return;
            case "term":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("How many days should the term last for?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int value) || value <= 0)
                {
                    actor.OutputHandler.Send("That is not a valid number of days.");
                    return;
                }

                appointment.ElectionTerm = TimeSpan.FromDays(value);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment election is now for a term of {$"{value.ToString("N0", actor)} day{(value == 1 ? "" : "s")}".ColourValue()}.");
                return;
            case "lead":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("How many days should elections finish before the elected take office?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out value) || value < 0)
                {
                    actor.OutputHandler.Send("That is not a valid number of days.");
                    return;
                }

                appointment.ElectionLeadTime = TimeSpan.FromDays(value);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment election now finishes {$"{value.ToString("N0", actor)} day{(value == 1 ? "" : "s")}".ColourValue()} before appointees take office.");
                return;
            case "nomination":
            case "nominations":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("How many days should the nomination period last for?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out value) || value <= 0)
                {
                    actor.OutputHandler.Send("That is not a valid number of days.");
                    return;
                }

                appointment.NominationPeriod = TimeSpan.FromDays(value);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment election now has a nomination period of {$"{value.ToString("N0", actor)} day{(value == 1 ? "" : "s")}".ColourValue()}.");
                return;
            case "voting":
            case "votes":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("How many days should the voting period last for?");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out value) || value <= 0)
                {
                    actor.OutputHandler.Send("That is not a valid number of days.");
                    return;
                }

                appointment.VotingPeriod = TimeSpan.FromDays(value);
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment election now has a voting period of {$"{value.ToString("N0", actor)} day{(value == 1 ? "" : "s")}".ColourValue()}.");
                return;
            case "terms":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "How many terms should be the consecutive term limit? Use 0 for no limits.");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int consecutive) || consecutive < 0)
                {
                    actor.OutputHandler.Send(
                        "You must enter a valid number of consecutive terms, or use 0 for no limits.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send("How many total terms should be the limit? Use 0 for no limits.");
                    return;
                }

                if (!int.TryParse(command.PopSpeech(), out int total) || total < 0)
                {
                    actor.OutputHandler.Send("You must enter a valid number of total terms, or use 0 for no limits.");
                    return;
                }

                appointment.MaximumConsecutiveTerms = consecutive;
                appointment.MaximumTotalTerms = total;
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment now has a term limit of {consecutive.ToString("N0", actor).ColourValue()} consecutive or {total.ToString("N0", actor).ColourValue()} total terms.");
                return;
            case "nominate":
            case "nominate prog":
            case "nominate_prog":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "Which prog do you want to use to determine if a character can nominate for an election?");
                    return;
                }

                IFutureProg prog = long.TryParse(command.PopSpeech(), out long lvalue)
                    ? actor.Gameworld.FutureProgs.Get(lvalue)
                    : actor.Gameworld.FutureProgs.GetByName(command.Last);
                if (prog == null)
                {
                    actor.OutputHandler.Send("There is no such prog.");
                    return;
                }

                if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean))
                {
                    actor.OutputHandler.Send(
                        "The prog must return a boolean, and the prog that you specified does not.");
                    return;
                }

                if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
                {
                    actor.OutputHandler.Send(
                        "The prog must be compatible with a single character parameter, and the prog that you specified is not.");
                    return;
                }

                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "Which prog do you want to use to give a failure message if the character can't nominate for an election?");
                    return;
                }

                IFutureProg whyprog = long.TryParse(command.PopSpeech(), out lvalue)
                    ? actor.Gameworld.FutureProgs.Get(lvalue)
                    : actor.Gameworld.FutureProgs.GetByName(command.Last);
                if (whyprog == null)
                {
                    actor.OutputHandler.Send("There is no such prog.");
                    return;
                }

                if (!whyprog.ReturnType.CompatibleWith(ProgVariableTypes.Text))
                {
                    actor.OutputHandler.Send(
                        "The why prog must return text, and the prog that you specified does not.");
                    return;
                }

                if (!whyprog.MatchesParameters(new[] { ProgVariableTypes.Character }))
                {
                    actor.OutputHandler.Send(
                        "The why prog must be compatible with a single character parameter, and the prog that you specified is not.");
                    return;
                }

                appointment.CanNominateProg = prog;
                appointment.WhyCantNominateProg = whyprog;
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine nomination eligability and {whyprog.MXPClickableFunctionNameWithId()} to give an error message.");
                return;
            case "vote":
            case "voteprog":
            case "vote prog":
            case "vote_prog":
            case "votingprog":
            case "voting prog":
            case "voting_prog":
                if (command.IsFinished)
                {
                    actor.OutputHandler.Send(
                        "Which prog do you want to use to determine how many votes a character gets in an election?");
                    return;
                }

                prog = long.TryParse(command.PopSpeech(), out lvalue)
                    ? actor.Gameworld.FutureProgs.Get(lvalue)
                    : actor.Gameworld.FutureProgs.GetByName(command.Last);
                if (prog == null)
                {
                    actor.OutputHandler.Send("There is no such prog.");
                    return;
                }

                if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Number))
                {
                    actor.OutputHandler.Send(
                        "The prog must return a number, and the prog that you specified does not.");
                    return;
                }

                if (!prog.MatchesParameters(new[] { ProgVariableTypes.Character }))
                {
                    actor.OutputHandler.Send(
                        "The prog must be compatible with a single character parameter, and the prog that you specified is not.");
                    return;
                }

                appointment.NumberOfVotesProg = prog;
                appointment.Changed = true;
                actor.OutputHandler.Send(
                    $"The {appointment.Name.ColourName()} appointment will now use the prog {prog.MXPClickableFunctionNameWithId()} to determine number of votes.");
                return;
            default:
                actor.OutputHandler.Send(helpText);
                return;
        }
    }

    #endregion ClanEditAppointment Subcommands

    private static string _clanAppointmentHelp = @"The valid options for this subcommand are as follows:

	#3name <name>#0 - changes the name of the appointment
	#3paygrade <which>|clear#0 - sets or clears the paygrade for an appointment
	#3minrank <rank>|clear#0 - sets or clears the minimum rank to hold this appointment
	#3minappointer <rank>|clear#0 - sets or clears the minimum rank to appoint this appointment
	#3parent <appointment>|clear#0 - sets or clears the parent appointment that controls this one
	#3maxholders <amount>|0#0 - sets the maximum number of people who can hold an abbreviation (use 0 for unlimited)
	#3privileges add <which>#0 - adds a privilege to the appointment
	#3privileges remove <which>#0 - removes a privilege to the appointment
	#3title add <prog> <title>#0 - adds a new title with a specified prog
	#3title remove <title>#0 - removes a title
	#3title swap <one> <two>#0 - swaps the evaluation order of two titles
	#3title set <which> <prog>#0 - changes the prog for a particular title
	#3abbreviation add <prog> <title>#0 - adds a new abbreviation with a specified prog
	#3abbreviation remove <title>#0 - removes an abbreviation
	#3abbreviation swap <one> <two>#0 - swaps the evaluation order of two abbreviations
	#3abbreviation set <which> <prog>#0 - changes the prog for a particular abbreviation
	#3fame <fame type>#0 - sets the level of fame for this appointment

	#3election enable#0 - enables elections
	#3election none#0 - removes elections
	#3election secret#0 - toggles secret voting
	#3election term <days>#0 - sets the term length
	#3election lead <days>#0 - sets the number of days after votes closed that appointments happen
	#3election nomination <days>#0 - sets the length of the nomination period
	#3election voting <days>#0 - sets the length of the voting period
	#3election terms <consecutive no|0> <total no|0>#0 - sets term limits
	#3election nominate <prog> <why>#0 - sets the nomination eligablity prog and ""why"" fail message prog
	#3election vote <prog>#0 - sets the vote count prog";

    private static void ClanEditAppointment(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator() && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointmentsUnderOwn))
        {
            actor.OutputHandler.Send("You do not have permission to edit appointments in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which appointment do you want to edit?");
            return;
        }

        string targetText = command.PopSpeech();
        IAppointment appointment =
            clan.Appointments.FirstOrDefault(
                x => x.Name.Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
        if (appointment == null)
        {
            actor.OutputHandler.Send("There is no such appointment for you to edit.");
            return;
        }

        if (!actor.IsAdministrator() && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
            !ClanCommandUtilities.HoldsOrControlsAppointment(actorMembership, appointment.ParentPosition))
        {
            actor.OutputHandler.Send(
                "You do not have permission to edit appointments other than those that fall under your own positions in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(_clanAppointmentHelp.SubstituteANSIColour());
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                ClanEditAppointmentName(actor, command, clan, appointment);
                break;
            case "abbreviation":
                ClanEditAppointmentAbbreviation(actor, command, clan, appointment);
                break;
            case "title":
                ClanEditAppointmentTitle(actor, command, clan, appointment);
                break;
            case "paygrade":
                ClanEditAppointmentPaygrade(actor, command, clan, appointment);
                break;
            case "minrank":
            case "minimum rank":
                ClanEditAppointmentMinRank(actor, command, clan, appointment);
                break;
            case "minappointer":
            case "minimum appointer":
                ClanEditAppointmentMinAppointer(actor, command, clan, appointment);
                break;
            case "parent":
                ClanEditAppointmentParent(actor, command, clan, appointment);
                break;
            case "maximum holders":
            case "max holders":
            case "maxholders":
                ClanEditAppointmentMaxHolders(actor, command, clan, appointment);
                break;
            case "privileges":
                ClanEditAppointmentPrivileges(actor, command, clan, appointment);
                break;
            case "elections":
            case "election":
                ClanEditAppointmentElections(actor, command, clan, appointment);
                break;
            case "fame":
                ClanEditAppointmentFame(actor, command, clan, appointment);
                break;
            default:
                actor.OutputHandler.Send(_clanAppointmentHelp.SubstituteANSIColour());
                return;
        }
    }

    #region ClanEditPaygrade SubCommands

    private static void ClanEditPaygradeName(ICharacter actor, StringStack command, IClan clan, IPaygrade paygrade)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What name do you want to give to this paygrade?");
            return;
        }

        string nameText = command.PopSpeech();
        if (clan.Paygrades.Any(x => x.Name.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a paygrade with that name. The paygrade's name must be unique.");
            return;
        }

        actor.OutputHandler.Send(
            $"You change the name of the {paygrade.Name.TitleCase().Colour(Telnet.Green)} paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {nameText.TitleCase().Colour(Telnet.Green)}.");
        paygrade.SetName(nameText);
        paygrade.Changed = true;
    }

    private static void ClanEditPaygradeAbbreviation(ICharacter actor, StringStack command, IClan clan,
        IPaygrade paygrade)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What abbreviation do you want to give to this paygrade?");
            return;
        }

        string nameText = command.PopSpeech();
        if (clan.Paygrades.Any(x => x.Abbreviation.Equals(nameText, StringComparison.InvariantCultureIgnoreCase)))
        {
            actor.OutputHandler.Send(
                "There is already a paygrade with that abbreviation. The paygrade's abbreviation must be unique.");
            return;
        }

        actor.OutputHandler.Send(
            $"You change the abbreviation of the {paygrade.Name.TitleCase().Colour(Telnet.Green)} paygrade in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {nameText.TitleCase().Colour(Telnet.Green)}.");
        paygrade.Abbreviation = nameText;
        paygrade.Changed = true;
    }

    private static void ClanEditPaygradeCurrency(ICharacter actor, StringStack command, IClan clan,
        IPaygrade paygrade)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What currency do you wish to set for this paygrade?");
            return;
        }

        ICurrency currency = long.TryParse(command.PopSpeech(), out long value)
            ? actor.Gameworld.Currencies.Get(value)
            : actor.Gameworld.Currencies.FirstOrDefault(
                x => x.Name.Equals(command.Last, StringComparison.InvariantCultureIgnoreCase));
        if (currency == null)
        {
            actor.OutputHandler.Send("There is no such currency for you to give to this paygrade.");
            return;
        }

        if (currency == paygrade.PayCurrency)
        {
            actor.OutputHandler.Send("That is already the currency of that paygrade.");
            return;
        }

        actor.OutputHandler.Send(
            $"You change the currency of paygrade {paygrade.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {paygrade.PayCurrency.Name.TitleCase().Colour(Telnet.Green)} from {currency.Name.TitleCase().Colour(Telnet.Green)}.");

        paygrade.PayCurrency = currency;
        paygrade.Changed = true;
    }

    private static void ClanEditPaygradeAmount(ICharacter actor, StringStack command, IClan clan, IPaygrade paygrade)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much do you want this paygrade to pay out?");
            return;
        }

        string currencyText = command.PopSpeech();
        decimal newAmount = paygrade.PayCurrency.GetBaseCurrency(currencyText, out bool success);
        if (!success)
        {
            actor.Send("That is not a valid amount of money in the {0} currency.",
                paygrade.PayCurrency.Name.TitleCase().Colour(Telnet.Green));
            return;
        }

        actor.OutputHandler.Send(
            $"You {(newAmount > paygrade.PayAmount ? "raise" : "lower")} the pay of paygrade {paygrade.Name.TitleCase().Colour(Telnet.Green)} in {clan.FullName.TitleCase().Colour(Telnet.Green)} to {paygrade.PayCurrency.Describe(newAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} from {paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)}.");

        paygrade.PayAmount = newAmount;
        paygrade.Changed = true;
    }

    #endregion ClanEditPaygrade SubCommands

    private static string _clanPaygradeHelp = @"The valid options for this subcommand are:

	#3name <name>#0 - sets a new name
	#3abbreviation <abbreviation>#0 - sets a new abbreviation
	#3currency <which>#0 - changes the paygrade's currency
	#3amount <how much>#0 - changes how much the paygrade pays";

    private static void ClanEditPaygrade(ICharacter actor, StringStack command, IClan clan)
    {
        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanCreatePaygrades))
        {
            actor.OutputHandler.Send("You are not allowed to edit paygrades in that clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which paygrade do you want to edit?");
            return;
        }

        string targetText = command.PopSpeech();
        IPaygrade paygrade =
            clan.Paygrades.FirstOrDefault(
                x => x.Name.Equals(targetText, StringComparison.InvariantCultureIgnoreCase)) ??
            clan.Paygrades.FirstOrDefault(
                x => x.Abbreviation.Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
        if (paygrade == null)
        {
            actor.OutputHandler.Send("There is no such paygrade for you to edit.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(_clanPaygradeHelp.SubstituteANSIColour());
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "name":
                ClanEditPaygradeName(actor, command, clan, paygrade);
                return;
            case "abbreviation":
            case "abbrev":
            case "abbrv":
                ClanEditPaygradeAbbreviation(actor, command, clan, paygrade);
                return;
            case "currency":
                ClanEditPaygradeCurrency(actor, command, clan, paygrade);
                return;
            case "amount":
            case "pay":
                ClanEditPaygradeAmount(actor, command, clan, paygrade);
                return;
            default:
                actor.OutputHandler.Send(_clanPaygradeHelp.SubstituteANSIColour());
                return;
        }
    }

    #endregion ClanEdit Subcommands

    private static void ClanEdit(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            if (actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().Any())
            {
                IClan editingclan = actor.CombinedEffectsOfType<BuilderEditingEffect<IClan>>().First().EditingItem;
                ClanViewDefault(actor, editingclan, actor.ClanMemberships.FirstOrDefault(x => x.Clan == editingclan));
                return;
            }

            actor.OutputHandler.Send("What clan do you wish to edit?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator()
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        if (!actor.IsAdministrator())
        {
            IClanMembership membership = actor.ClanMemberships.First(x => x.Clan == clan);
            ClanPrivilegeType netPrivs = membership.NetPrivileges;
            if (!netPrivs.HasFlag(ClanPrivilegeType.CanEditClanDetails) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanCreateRanks) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanCreateAppointments) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanCreateAppointmentsUnderOwn) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanCreatePaygrades) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanManageBankAccounts) &&
                !netPrivs.HasFlag(ClanPrivilegeType.CanManageEconomicZones))
            {
                actor.OutputHandler.Send("You are not allowed to edit that clan.");
                return;
            }
        }

        actor.RemoveAllEffects(x => x is BuilderEditingEffect<IClan>);
        actor.AddEffect(new BuilderEditingEffect<IClan>(actor) { EditingItem = clan });
        actor.OutputHandler.Send($"You are now editing the clan {clan.FullName.TitleCase().ColourName()}.");
    }

    private static void ClanSet(ICharacter actor, StringStack command)
    {
        string text = command.PopSpeech().ToLowerInvariant();
        switch (text)
        {
            case "name":
            case "alias":
            case "desc":
            case "description":
            case "template":
            case "payday":
            case "rank":
            case "appointment":
            case "paygrade":
            case "notable":
            case "sphere":
            case "notablemembers":
            case "bankaccount":
            case "discord":
            case "calendar":
                ClanEditProperty(actor, command, text);
                return;
        }

        actor.OutputHandler.Send(@"You can use the following options with this building command:

	#3name <name>#0 - sets the clan's name
	#3alias <alias>#0 - sets the clan's alias
	#3desc#0 - drops you into an editor to set clan's description
	#3rank ...#0 - edits clan ranks. See command for more info.
	#3paygrade ...#0 - edits paygrades. See command for more info.
	#3appointment ...#0 - edits appointments. See command for more info.
	#3sphere <which>#0 - sets the sphere for this clan (admin only)
	#3template#0 - toggles this clan being a template clan (admin only)
	#3notable#0 - toggles this clan appearing in WHO (admin only)
	#3notablemembers#0 - toggles the famous members of this clan appearing in NOTABLES (admin only)
	#3bankaccount <account>#0 - sets a bank account for this clan
	#3discord <none>|<id>#0 - sets a discord channel for the clan
	#3calendar <which>#0 - changes the calendar that the clan uses".SubstituteANSIColour());
    }

    private static void ClanSubmit(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you wish to submit to external control?");
            return;
        }

        string clanText = command.PopSpeech();

        IClan clan = GetTargetClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which appointment in that clan do you wish to submit to external control?");
            return;
        }

        clan.SubmitControl(actor, command);
    }

    private static void ClanDisband(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.Send("Which clan do you want to disband?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        if (!actor.IsAdministrator(PermissionLevel.Admin) && actorMembership != null &&
            !actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanDisbandClan))
        {
            actor.OutputHandler.Send("You are not allowed to disband that clan.");
            return;
        }

        actor.AddEffect(new Accept(actor, new GenericProposal(
            text => { clan.Disband(actor); },
            text => { actor.Send($"You decide not to disband {clan.FullName.Colour(Telnet.Green)}."); },
            () => { actor.Send($"You decide not to disband {clan.FullName.Colour(Telnet.Green)}."); },
            "",
            "disband", "clan"
        )), TimeSpan.FromSeconds(120));
        actor.Send(
            $"Do you really want to disband the clan {clan.FullName.Colour(Telnet.Green)}? Please type {"accept".Colour(Telnet.Yellow)} to do so, or {"decline".Colour(Telnet.Yellow)} to decide not to. Warning: This action is permanent and irreversible.");
    }

    private static void ClanAppoint(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to give an appointment to?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to give them an appointment?");
            return;
        }

        string clanText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("To which position do you want to appoint them?");
            return;
        }

        IClan clan = GetTargetClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.Appoint(actor, new StringStack($"\"{targetText}\" {command.RemainingArgument}"));
    }

    private static void ClanPay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to alter the backpay of?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to alter backpay?");
            return;
        }

        string clanText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("How much backpay do you want to give them?");
            return;
        }

        string backpayText = command.SafeRemainingArgument;

        IClan clan = GetTargetClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        IClanMembership actorMembership = actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);

        if (!actor.IsAdministrator(PermissionLevel.Admin) &&
            actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanGiveBackpay) != true)
        {
            actor.OutputHandler.Send("You are not allowed to give backpay for people in that clan.");
            return;
        }

        IClanMembership targetMembership;
        ICharacter targetActor = actor.TargetActor(targetText);
        if (targetActor != null)
        {
            targetMembership = targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == clan);
        }
        else
        {
            targetMembership =
                clan.Memberships.FirstOrDefault(
                    x => !x.IsArchivedMembership &&
                         x.PersonalName.GetName(NameStyle.FullName)
                          .Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
        }

        if (targetMembership == null)
        {
            actor.OutputHandler.Send("There is no such member for you to give backpay to.");
            return;
        }

        decimal amount = actor.Currency.GetBaseCurrency(backpayText, out bool success);
        if (!success)
        {
            actor.Send(
                $"That is not a valid amount of currency in the {actor.Currency.Name.Colour(Telnet.Green)} currency to backpay someone.");
            return;
        }

        targetMembership.AwardPay(actor.Currency, amount);
        ClanPayrollHistoryEntry.Create(targetMembership, actor.Currency, amount,
            ClanPayrollHistoryType.ManualAdjustment,
            $"Manual backpay adjustment by {actor.PersonalName.GetName(NameStyle.FullName)}", actor: actor);
        actor.Send(
            $"You give {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.Long)} in backpay to {targetMembership.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green)} in the {clan.FullName.Colour(Telnet.Green)} clan.");
    }

    private static void ClanDismiss(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you want to dismiss from an appointment?");
            return;
        }

        string targetText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which clan do you wish to dismiss them from an appointment?");
            return;
        }

        string clanText = command.PopSpeech();
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("From which position do you want to dismiss them?");
            return;
        }

        IClan clan = GetTargetClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.Dismiss(actor, new StringStack($"\"{targetText}\" {command.RemainingArgument}"));
    }

    private static void ClanTransfer(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you wish to release from external control?");
            return;
        }

        string clanText = command.PopSpeech();

        IClan clan = GetTargetExternallyAccessibleClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member (or member of a liege) of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which appointment in that clan do you wish to transfer from external control?");
            return;
        }

        clan.TransferControl(actor, command);
    }

    private static void ClanRelease(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan do you wish to release from external control?");
            return;
        }

        string clanText = command.PopSpeech();

        IClan clan = GetTargetExternallyAccessibleClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member (or member of a liege) of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which appointment in that clan do you wish to release from external control?");
            return;
        }

        clan.ReleaseControl(actor, command);
    }

    private static void ClanMembers(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("For which clan do you want to see a list of members?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        clan.ShowMembers(actor);
    }

    private static void ClanVassalAppoint(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to appoint to a position in a vassal clan?");
            return;
        }

        string targetText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which vassal clan do you wish to appoint someone?");
            return;
        }

        string clanText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("To which position do you wish to appoint them?");
            return;
        }

        IClan clan = GetTargetExternallyAccessibleClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member (or member of a liege) of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("To which position do you wish to appoint them?");
            return;
        }

        clan.AppointExternal(actor, new StringStack($"\"{targetText}\" {command.RemainingArgument}"));
    }

    private static void ClanVassalDismiss(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Who do you wish to dismiss from a position in a vassal clan?");
            return;
        }

        string targetText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which vassal clan do you wish to dismiss someone?");
            return;
        }

        string clanText = command.PopSpeech();

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("From which position do you wish to dismiss them?");
            return;
        }

        IClan clan = GetTargetExternallyAccessibleClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member (or member of a liege) of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send("From which position do you wish to dismiss them?");
            return;
        }

        clan.DismissExternal(actor, new StringStack($"\"{targetText}\" {command.RemainingArgument}"));
    }

    private static void ClanVassalControl(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("In which vassal clan do you wish to alter the controlling appointment?");
            return;
        }

        string clanText = command.PopSpeech();
        IClan clan = GetTargetExternallyAccessibleClan(actor, clanText);
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.Admin)
                ? "There is no such clan."
                : "You are not a member (or member of a liege) of any such clan.");
            return;
        }

        if (command.IsFinished)
        {
            actor.OutputHandler.Send(
                "Which appointment in that vassal clan do you wish to alter the controlling appointment for?");
            return;
        }

        clan.SetControllingAppointment(actor, command);
    }

    private static void ClanVassal(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("What do you want to do with a vassal clan?");
            return;
        }

        switch (command.PopSpeech().ToLowerInvariant())
        {
            case "appoint":
                ClanVassalAppoint(actor, command);
                break;
            case "dismiss":
                ClanVassalDismiss(actor, command);
                break;
            case "control":
                ClanVassalControl(actor, command);
                break;
            default:
                actor.OutputHandler.Send("That is not a valid option to use with the clan vassal command.");
                return;
        }
    }

    private static void ClanTreasury(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("Which clan's treasury do you want to manage?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return;
        }

        if (!command.IsFinished)
        {
            switch (command.PopForSwitch())
            {
                case "balance":
                case "show":
                case "view":
                    ClanTreasuryBalance(actor, clan);
                    return;
                case "deposit":
                case "add":
                    ClanTreasuryDeposit(actor, clan, command);
                    return;
                case "withdraw":
                case "remove":
                    ClanTreasuryWithdraw(actor, clan, command);
                    return;
                case "ledger":
                case "history":
                    ClanTreasuryLedger(actor, clan, command);
                    return;
                case "toggle":
                case "cell":
                    break;
                default:
                    actor.OutputHandler.Send(
                        "Use BALANCE, DEPOSIT <amount>, WITHDRAW <amount>, LEDGER [count], or TOGGLE.");
                    return;
            }
        }

        if (clan.TreasuryCells.Contains(actor.Location))
        {
            clan.RemoveTreasuryCell(actor.Location);
            clan.Changed = true;
            actor.Send(
                $"Your current location is no longer a treasury cell for the {clan.FullName.Colour(Telnet.Green)} clan.");
            return;
        }

        clan.AddTreasuryCell(actor.Location);
        clan.Changed = true;
        actor.Send($"Your current location is now a treasury cell for the {clan.FullName.Colour(Telnet.Green)} clan.");
    }

    private static bool CanViewClanTreasury(ICharacter actor, IClan clan)
    {
        return actor.IsAdministrator() ||
               actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)?.NetPrivileges
                    .HasFlag(ClanPrivilegeType.CanViewTreasury) == true;
    }

    private static bool CanManageClanTreasury(ICharacter actor, IClan clan)
    {
        return actor.IsAdministrator() ||
               actor.ClanMemberships.FirstOrDefault(x => x.Clan == clan)?.NetPrivileges
                    .HasFlag(ClanPrivilegeType.CanManageBankAccounts) == true;
    }

    private static void ClanTreasuryBalance(ICharacter actor, IClan clan)
    {
        if (!CanViewClanTreasury(actor, clan))
        {
            actor.OutputHandler.Send($"You are not authorised to view the treasury for {clan.FullName.ColourName()}.");
            return;
        }

        var currencies = actor.Gameworld.Currencies
                              .Select(x => (Currency: x, Balance: VirtualCashLedger.Balance(clan, x)))
                              .Where(x => x.Balance != 0.0M)
                              .ToList();
        if (!currencies.Any() && actor.Currency is not null)
        {
            currencies.Add((actor.Currency, 0.0M));
        }

        if (!currencies.Any())
        {
            actor.OutputHandler.Send($"{clan.FullName.ColourName()} has no virtual treasury balances.");
            return;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            currencies.OrderBy(x => x.Currency.Name).Select(x => new List<string>
            {
                x.Currency.Name.TitleCase(),
                x.Currency.Describe(x.Balance, CurrencyDescriptionPatternType.ShortDecimal),
                x.Currency.Describe(VirtualCashLedger.AvailableFunds(clan, x.Currency,
                    clan.ClanBankAccount?.Currency == x.Currency ? clan.ClanBankAccount : null),
                    CurrencyDescriptionPatternType.ShortDecimal)
            }),
            new List<string> { "Currency", "Virtual Cash", "Available" },
            actor.LineFormatLength,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode));
    }

    private static void ClanTreasuryDeposit(ICharacter actor, IClan clan, StringStack command)
    {
        if (!CanManageClanTreasury(actor, clan))
        {
            actor.OutputHandler.Send($"You are not authorised to manage the treasury for {clan.FullName.ColourName()}.");
            return;
        }

        if (actor.Currency is null)
        {
            actor.OutputHandler.Send("You must set an active currency first.");
            return;
        }

        if (command.IsFinished ||
            !actor.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) ||
            amount <= 0.0M)
        {
            actor.OutputHandler.Send($"How much {actor.Currency.Name.ColourName()} do you want to deposit?");
            return;
        }

        var payment = new OtherCashPayment(actor.Currency, actor);
        var accessible = payment.AccessibleMoneyForPayment();
        if (accessible < amount)
        {
            actor.OutputHandler.Send(
                $"You only have {actor.Currency.Describe(accessible, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} available.");
            return;
        }

        payment.TakePayment(amount);
        VirtualCashLedger.Credit(clan, actor.Currency, amount, actor, actor, "Cash",
            $"Clan treasury cash deposit for {clan.FullName}");
        actor.OutputHandler.Send(
            $"You deposit {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} into the virtual treasury for {clan.FullName.ColourName()}.");
    }

    private static void ClanTreasuryWithdraw(ICharacter actor, IClan clan, StringStack command)
    {
        if (!CanManageClanTreasury(actor, clan))
        {
            actor.OutputHandler.Send($"You are not authorised to manage the treasury for {clan.FullName.ColourName()}.");
            return;
        }

        if (actor.Currency is null)
        {
            actor.OutputHandler.Send("You must set an active currency first.");
            return;
        }

        if (command.IsFinished ||
            !actor.Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var amount) ||
            amount <= 0.0M)
        {
            actor.OutputHandler.Send($"How much {actor.Currency.Name.ColourName()} do you want to withdraw?");
            return;
        }

        if (!VirtualCashLedger.Debit(clan, actor.Currency, amount, actor, actor, "CashWithdrawal",
                $"Clan treasury cash withdrawal for {clan.FullName}",
                clan.ClanBankAccount?.Currency == actor.Currency ? clan.ClanBankAccount : null,
                null, out var error))
        {
            actor.OutputHandler.Send(error);
            return;
        }

        IGameItem cash = CurrencyGameItemComponentProto.CreateNewCurrencyPile(actor.Currency,
            actor.Currency.FindCoinsForAmount(amount, out _));
        if (actor.Body.CanGet(cash, 0))
        {
            actor.Body.Get(cash, silent: true);
        }
        else
        {
            cash.RoomLayer = actor.RoomLayer;
            actor.Location.Insert(cash, true);
            actor.OutputHandler.Send("You couldn't hold the money, so it is on the ground.");
        }

        actor.OutputHandler.Send(
            $"You withdraw {actor.Currency.Describe(amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()} from the virtual treasury for {clan.FullName.ColourName()}.");
    }

    private static void ClanTreasuryLedger(ICharacter actor, IClan clan, StringStack command)
    {
        if (!CanViewClanTreasury(actor, clan))
        {
            actor.OutputHandler.Send($"You are not authorised to view the treasury for {clan.FullName.ColourName()}.");
            return;
        }

        var count = 25;
        if (!command.IsFinished && (!int.TryParse(command.SafeRemainingArgument, out count) || count <= 0))
        {
            actor.OutputHandler.Send("How many ledger entries do you want to review?");
            return;
        }

        var entries = VirtualCashLedger.LedgerEntries(clan, count).ToList();
        if (!entries.Any())
        {
            actor.OutputHandler.Send($"{clan.FullName.ColourName()} does not have any virtual treasury ledger entries.");
            return;
        }

        actor.OutputHandler.Send(StringUtilities.GetTextTable(
            entries.Select(x =>
            {
                var currency = actor.Gameworld.Currencies.Get(x.CurrencyId) ?? actor.Currency;
                return new List<string>
                {
                    x.RealDateTime.ToString("g", actor),
                    x.ActorName ?? string.Empty,
                    currency?.Describe(x.Amount, CurrencyDescriptionPatternType.ShortDecimal) ?? x.Amount.ToString("N2", actor),
                    currency?.Describe(x.BalanceAfter, CurrencyDescriptionPatternType.ShortDecimal) ?? x.BalanceAfter.ToString("N2", actor),
                    $"{x.SourceKind}->{x.DestinationKind}",
                    x.Reason
                };
            }),
            new List<string> { "When", "Actor", "Amount", "Balance", "Route", "Reason" },
            actor.LineFormatLength,
            colour: Telnet.Green,
            unicodeTable: actor.Account.UseUnicode));
    }

    private static void ClanAdministration(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("For which clan do you want to toggle an administration cell?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send("There is no such clan.");
            return;
        }

        if (clan.AdministrationCells.Contains(actor.Location))
        {
            clan.RemoveAdministrationCell(actor.Location);
            clan.Changed = true;
            actor.Send(
                $"Your current location is no longer an administration cell for the {clan.FullName.Colour(Telnet.Green)} clan.");
            return;
        }

        clan.AddAdministrationCell(actor.Location);
        clan.Changed = true;
        actor.Send(
            $"Your current location is now a administration cell for the {clan.FullName.Colour(Telnet.Green)} clan.");
    }

    private static void ClanMaxBackpay(ICharacter actor, StringStack command)
    {
        if (command.IsFinished)
        {
            actor.OutputHandler.Send("For which clan do you want to set or clear a maximum backpay?");
            return;
        }

        IClan clan = GetTargetClan(actor, command.PopSpeech());
        if (clan == null)
        {
            actor.OutputHandler.Send(actor.IsAdministrator(PermissionLevel.JuniorAdmin)
                ? "There is no such clan."
                : "You are not a member of any such clan.");
            return;
        }

        if (!actor.IsAdministrator() && !actor.ClanMemberships.First(x => x.Clan == clan).NetPrivileges
                                              .HasFlag(ClanPrivilegeType.CanCreatePaygrades))
        {
            actor.Send("You need the CanCreatePaygrades privilege in a clan to be able to set a backpay cap.");
            return;
        }

        if (command.IsFinished)
        {
            actor.Send(
                "You must either specify CLEAR to clear the maximum backpay and make it uncapped, or specify a multiple of the individual's per-period pay to cap backpay at.");
            return;
        }

        if (command.Peek().EqualTo("clear"))
        {
            clan.MaximumPeriodsOfUncollectedBackPay = null;
            clan.Changed = true;
            actor.Send($"The clan {clan.FullName.Colour(Telnet.Green)} will no longer have a cap on backpay.");
            return;
        }

        if (!int.TryParse(command.PopSpeech(), out int value))
        {
            actor.Send(
                "You must either specify CLEAR to clear the maximum backpay and make it uncapped, or specify a multiple of the individual's per-period pay to cap backpay at.");
            return;
        }

        if (value < 1)
        {
            actor.Send("Backpay must be capped at a number at least a full pay interval.");
            return;
        }

        clan.MaximumPeriodsOfUncollectedBackPay = value;
        clan.Changed = true;
        actor.Send(
            $"The clan {clan.FullName.Colour(Telnet.Green)} will now cap backpay at {value:N0} times the total pay for each pay interval.");
    }

    #endregion Clan Sub-Commands

    [PlayerCommand("Notables", "notables")]
    [HelpInfo("notables",
        "The notables command is used to view notable individuals that all people would be aware of in your society, like leaders and public figures. The syntax is simply NOTABLES.",
        AutoHelp.HelpArg)]
    protected static void Notables(ICharacter actor, string command)
    {
        string sphere =
            actor.Gameworld.FutureProgs.Get(actor.Gameworld.GetStaticLong("CharacterSphereProgId"))?.Execute(actor)
                 ?.ToString() ?? string.Empty;
        List<IClan> sphereClans = actor.Gameworld.Clans
                               .Where(x => x.ShowFamousMembersInNotables)
                               .Where(x => x.Sphere == null || x.Sphere.EqualTo(sphere))
                               .ToList();
        StringBuilder sb = new();
        foreach (IClan clan in sphereClans)
        {
            List<IClanMembership> sphereNotables = clan.Memberships
                                     .Where(x => !x.IsArchivedMembership)
                                     .Where(x => x.Rank.FameType != ClanFameType.None ||
                                                 x.Appointments.Any(y => y.FameType != ClanFameType.None))
                                     .ToList();
            if (!sphereNotables.Any())
            {
                continue;
            }

            if (sb.Length != 0)
            {
                sb.AppendLine();
            }

            sb.AppendLine($"Notables from the {clan.Name.ColourName()} clan:");
            sb.AppendLine();
            foreach (IClanMembership notable in sphereNotables)
            {
                List<IAppointment> appointments = notable.Appointments.Where(x => x.FameType != ClanFameType.None).ToList();
                bool showDesc = notable.Rank.FameType == ClanFameType.NameAndDescription ||
                               appointments.Any(x => x.FameType == ClanFameType.NameAndDescription);
                if (showDesc)
                {
                    sb.Append(
                        $"\t{notable.PersonalName.GetName(NameStyle.FullName).ColourName()} ({actor.Gameworld.TryGetCharacter(notable.MemberId, true).HowSeen(null, flags: PerceiveIgnoreFlags.IgnoreCanSee | PerceiveIgnoreFlags.IgnoreSelf | PerceiveIgnoreFlags.IgnoreDisguises | PerceiveIgnoreFlags.IgnoreLoadThings)}): ");
                }
                else
                {
                    sb.Append($"\t{notable.PersonalName.GetName(NameStyle.FullName).ColourName()}: ");
                }

                List<string> strings = new();
                if (notable.Rank.FameType != ClanFameType.None)
                {
                    strings.Add(notable.Rank.Title(actor).ColourValue());
                }

                if (appointments.Any())
                {
                    strings.AddRange(appointments.Select(x => x.Title(actor).ColourValue()));
                }

                sb.AppendLine(strings.ListToString());
            }
        }

        if (sb.Length == 0)
        {
            actor.OutputHandler.Send("You aren't aware of any notable individuals.");
            return;
        }

        actor.OutputHandler.Send(sb.ToString());
    }
}
