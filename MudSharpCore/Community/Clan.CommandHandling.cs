using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace MudSharp.Community;

public partial class Clan
{
	private IClanMembership? ActorMembership(ICharacter actor)
	{
		return actor.ClanMemberships.FirstOrDefault(x => x.Clan == this);
	}

	private bool HasPrivilege(ICharacter actor, ClanPrivilegeType privilege, out IClanMembership? membership)
	{
		membership = ActorMembership(actor);
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(privilege) == true;
	}

	private bool CanViewMembers(ICharacter actor, IClanMembership? membership)
	{
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers) == true;
	}

	private bool CanViewOfficeHolders(ICharacter actor, IClanMembership? membership)
	{
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders) == true;
	}

	private bool CanViewAboveOwnRank(ICharacter actor, IClanMembership? membership)
	{
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure) == true;
	}

	private bool CanViewEqualRankOrLower(ICharacter actor, IClanMembership? membership)
	{
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructureEqualRankOrLower) == true;
	}

	private bool CanViewFinances(ICharacter actor, IClanMembership? membership)
	{
		return actor.IsAdministrator(PermissionLevel.Admin) ||
		       membership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewTreasury) == true;
	}

	private bool IsVisibleThroughAppointmentChain(IClanMembership? membership, IAppointment? appointment)
	{
		return ClanCommandUtilities.HoldsOrControlsAppointment(membership, appointment);
	}

	private IClanMembership? GetActiveMembership(string targetText, ICharacter actor, out ICharacter? targetActor)
	{
		targetActor = actor.TargetActor(targetText);
		if (targetActor is not null)
		{
			return targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == this && !x.IsArchivedMembership);
		}

		return Memberships
			.Where(x => !x.IsArchivedMembership)
			.GetByNameOrAbbreviation(targetText);
	}

	private IClanMembership? GetMembershipByName(string targetText)
	{
		return Memberships
			.Where(x => !x.IsArchivedMembership)
			.GetByNameOrAbbreviation(targetText);
	}

	private bool TryGetExternalControl(string appointmentText, string? liegeClanText, ICharacter actor,
		out IExternalClanControl? control, out IClan? liegeClan, out string error)
	{
		var matches = ExternalControls
			.Where(x => x.VassalClan == this)
			.Where(x => x.ControlledAppointment.Name.EqualTo(appointmentText) ||
			            x.ControlledAppointment.Name.StartsWith(appointmentText,
				            StringComparison.InvariantCultureIgnoreCase))
			.ToList();

		if (!matches.Any())
		{
			control = null;
			liegeClan = null;
			error = "There are no such appointments available in that clan.";
			return false;
		}

		if (!string.IsNullOrEmpty(liegeClanText))
		{
			matches = matches
				.Where(x => x.LiegeClan.FullName.EqualTo(liegeClanText) || x.LiegeClan.Alias.EqualTo(liegeClanText) ||
				            x.LiegeClan.FullName.StartsWith(liegeClanText, StringComparison.InvariantCultureIgnoreCase) ||
				            x.LiegeClan.Alias.StartsWith(liegeClanText, StringComparison.InvariantCultureIgnoreCase))
				.ToList();
		}

		if (!matches.Any())
		{
			control = null;
			liegeClan = null;
			error = "There is no such clan, or it is not a valid liege of the vassal clan.";
			return false;
		}

		if (matches.Count > 1)
		{
			var actorLiegeMatches = actor.IsAdministrator(PermissionLevel.Admin)
				? matches
				: matches.Where(x => actor.ClanMemberships.Any(y => y.Clan == x.LiegeClan)).ToList();
			if (actorLiegeMatches.Count == 1)
			{
				matches = actorLiegeMatches;
			}
			else
			{
				control = null;
				liegeClan = null;
				error =
					"The requested appointment is ambiguous, you must supply the name of the liege clan you wish to use.";
				return false;
			}
		}

		control = matches[0];
		liegeClan = control.LiegeClan;
		error = string.Empty;
		return true;
	}

	private bool CanManageExternalControl(ICharacter actor, IExternalClanControl control, IClan liegeClan,
		out IClanMembership? actorMembership)
	{
		actorMembership = liegeClan.Memberships.FirstOrDefault(x => x.MemberId == actor.Id);
		if (actor.IsAdministrator(PermissionLevel.Admin))
		{
			return true;
		}

		if (actorMembership is null)
		{
			return false;
		}

		if (actorMembership.NetPrivileges.HasFlag(ClanPrivilegeType.CanManageClanVassals))
		{
			return true;
		}

		return ClanCommandUtilities.HoldsOrControlsAppointment(actorMembership, control.ControllingAppointment);
	}

	private bool TryResolveElection(string electionOrAppointmentText, out IElection? election, out string error)
	{
		if (long.TryParse(electionOrAppointmentText, out var value))
		{
			election = Gameworld.Elections.Get(value);
			if (election is null || election.Appointment.Clan != this)
			{
				error = "There is no such election in that clan.";
				return false;
			}

			error = string.Empty;
			return true;
		}

		var appointment = Appointments.GetByIdOrName(electionOrAppointmentText);
		if (appointment is null)
		{
			election = null;
			error = $"{FullName.ColourName()} has no such appointment.";
			return false;
		}

		if (!appointment.IsAppointedByElection)
		{
			election = null;
			error = $"The position {appointment.Name.ColourName()} is not controlled by elections.";
			return false;
		}

		election = ClanCommandUtilities.GetNextOpenElection(appointment);
		if (election is null)
		{
			error = $"There is no open election for the position of {appointment.Name.ColourName()} in {FullName.ColourName()}.";
			return false;
		}

		error = string.Empty;
		return true;
	}

	private void ShowElectionNominees(ICharacter actor, IElection election, string? errorMessage = null)
	{
		var sb = new StringBuilder();
		if (!string.IsNullOrEmpty(errorMessage))
		{
			sb.AppendLine(errorMessage);
		}

		sb.AppendLine(
			$"There are the following nominations for the election of {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()}:");
		sb.AppendLine();
		foreach (var nominee in election.Nominees)
		{
			var dub = actor.Dubs.FirstOrDefault(x =>
				x.FrameworkItemType == "Character" && x.TargetId == nominee.MemberId && !x.WasIdentityConcealed);
			if (dub is not null)
			{
				sb.AppendLine(
					$"\t{nominee.PersonalName.GetName(NameStyle.FullName)} ({dub.LastDescription.ColourCharacter()})");
				continue;
			}

			sb.AppendLine($"\t{nominee.PersonalName.GetName(NameStyle.FullName)}");
		}

		actor.OutputHandler.Send(sb.ToString());
	}

	public void Show(ICharacter actor, StringStack command)
	{
		var actorMembership = ActorMembership(actor);
		if (!actor.IsAdministrator(PermissionLevel.Admin) && !IsTemplate &&
		    actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructure) != true &&
		    actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanStructureEqualRankOrLower) != true)
		{
			actor.OutputHandler.Send("You are not allowed to view the structure of that clan.");
			return;
		}

		switch (command.PopSpeech().ToLowerInvariant())
		{
			case "":
				ShowDefault(actor, actorMembership);
				return;
			case "rank":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which rank do you wish to view in that clan?");
					return;
				}

				var rank = Ranks.GetByIdOrName(command.SafeRemainingArgument);
				if (rank is null)
				{
					actor.OutputHandler.Send("There is no such rank for you to view.");
					return;
				}

				if (!actor.IsAdministrator(PermissionLevel.Admin) && !IsTemplate &&
				    !CanViewAboveOwnRank(actor, actorMembership) &&
				    (actorMembership is null || rank.RankNumber > actorMembership.Rank.RankNumber))
				{
					actor.OutputHandler.Send("There is no such rank for you to view.");
					return;
				}

				ShowRank(actor, rank);
				return;
			case "pay grade":
			case "paygrade":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which pay grade do you wish to view in that clan?");
					return;
				}

				var paygrade = Paygrades.GetByIdOrName(command.SafeRemainingArgument);
				if (paygrade is null)
				{
					actor.OutputHandler.Send("There is no such pay grade for you to view.");
					return;
				}

				if (!actor.IsAdministrator(PermissionLevel.Admin) && !IsTemplate &&
				    !CanViewAboveOwnRank(actor, actorMembership))
				{
					var paygradeRank = Ranks.FirstOrDefault(x => x.Paygrades.Contains(paygrade));
					if (paygradeRank is not null &&
					    (actorMembership is null || paygradeRank.RankNumber > actorMembership.Rank.RankNumber))
					{
						actor.OutputHandler.Send("There is no such pay grade for you to view.");
						return;
					}

					var paygradeAppointment = Appointments.FirstOrDefault(x => x.Paygrade == paygrade);
					if (paygradeAppointment is not null &&
					    !IsVisibleThroughAppointmentChain(actorMembership, paygradeAppointment))
					{
						actor.OutputHandler.Send("There is no such pay grade for you to view.");
						return;
					}
				}

				ShowPaygrade(actor, paygrade);
				return;
			case "appointment":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which appointment do you wish to view in that clan?");
					return;
				}

				var appointment = Appointments.GetByIdOrName(command.SafeRemainingArgument);
				if (appointment is null)
				{
					actor.OutputHandler.Send("There is no such appointment for you to view.");
					return;
				}

				if (!actor.IsAdministrator(PermissionLevel.Admin) && !IsTemplate &&
				    !CanViewAboveOwnRank(actor, actorMembership) &&
				    !IsVisibleThroughAppointmentChain(actorMembership, appointment))
				{
					actor.OutputHandler.Send("There is no such appointment for you to view.");
					return;
				}

				ShowAppointment(actor, appointment);
				return;
			case "external":
			case "externalcontrol":
			case "external control":
			case "control":
				if (command.IsFinished)
				{
					actor.OutputHandler.Send("Which clan's external control do you wish to view in that clan?");
					return;
				}

				var externalClanText = command.PopSpeech();
				if (command.IsFinished)
				{
					actor.OutputHandler.Send(
						"Which position in that clan do you wish to view the other clan's external control over?");
					return;
				}

				var externalPositionText = command.SafeRemainingArgument;
				var external = ExternalControls
					.Where(x => x.VassalClan == this)
					.Where(x => x.LiegeClan.FullName.EqualTo(externalClanText) || x.LiegeClan.Alias.EqualTo(externalClanText) ||
					            x.LiegeClan.FullName.StartsWith(externalClanText, StringComparison.InvariantCultureIgnoreCase) ||
					            x.LiegeClan.Alias.StartsWith(externalClanText, StringComparison.InvariantCultureIgnoreCase))
					.FirstOrDefault(x => x.ControlledAppointment.Name.EqualTo(externalPositionText) ||
					                     x.ControlledAppointment.Name.StartsWith(externalPositionText,
						                     StringComparison.InvariantCultureIgnoreCase));
				if (external is null)
				{
					actor.OutputHandler.Send("There is no such clan exerting any external control over that clan.");
					return;
				}

				ShowExternalControl(actor, external);
				return;
			default:
				actor.OutputHandler.Send("That is not a valid option for the clan view command.");
				return;
		}
	}

	public void ShowMembers(ICharacter actor)
	{
		var actorMembership = ActorMembership(actor);
		if (!actor.IsAdministrator(PermissionLevel.Admin) &&
		    actorMembership?.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewMembers) != true)
		{
			actor.OutputHandler.Send("You are not allowed to view the list of members for that clan.");
			return;
		}

		var members = (actor.IsAdministrator(PermissionLevel.Admin) || CanViewAboveOwnRank(actor, actorMembership)
			? Memberships.Where(x => !x.IsArchivedMembership)
			: Memberships.Where(x => !x.IsArchivedMembership &&
			                         actorMembership is not null &&
			                         x.Rank.RankNumber <= actorMembership.Rank.RankNumber))
			.OrderByDescending(x => x.Rank.RankNumber)
			.ThenBy(x => x.JoinDate)
			.ToList();

		actor.OutputHandler.Send(
			StringUtilities.GetTextTable(
				from member in members
				select new[]
				{
					member.PersonalName.GetName(NameStyle.FullName),
					member.Rank.Name.TitleCase(),
					member.Paygrade?.Abbreviation ?? "N/A",
					member.Appointments.Select(x => x.Name.TitleCase()).ListToString(conjunction: "", twoItemJoiner: ", "),
					Calendar.DisplayDate(member.JoinDate, CalendarDisplayMode.Short)
				},
				new[] { "Name", "Rank", "Paygrade", "Appointments", "Member Since" },
				actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 3));
	}

	public string DescribeElections(ICharacter actor)
	{
		var actorMembership = ActorMembership(actor);
		if (!actor.IsAdministrator(PermissionLevel.Admin) && !CanViewOfficeHolders(actor, actorMembership))
		{
			return $"You do not have sufficient privileges in {FullName.ColourName()} to view elections.";
		}

		var appointmentsWithElections = Appointments.Where(x => x.IsAppointedByElection).ToList();
		if (!appointmentsWithElections.Any())
		{
			return string.Empty;
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Elections in {FullName}".GetLineWithTitle(actor.LineFormatLength,
			actor.Account.UseUnicode, Telnet.BoldBlue, Telnet.BoldWhite));
		foreach (var appointment in appointmentsWithElections)
		{
			sb.AppendLine();
			sb.AppendLine(
				$"The {appointment.Name.ColourName()} position elects {appointment.MaximumSimultaneousHolders.ToString("N0", actor).ColourValue()} positions every {appointment.ElectionTerm.Describe(actor).ColourValue()}.");
			if (appointment.MaximumConsecutiveTerms <= 0 && appointment.MaximumTotalTerms <= 0)
			{
				sb.AppendLine("There are no term limits for electors.");
			}
			else if (appointment.MaximumConsecutiveTerms <= 0)
			{
				sb.AppendLine(
					$"There is a life-time term limit of {appointment.MaximumTotalTerms.ToString("N0", actor).ColourValue()} term{(appointment.MaximumTotalTerms == 1 ? "" : "s")}.");
			}
			else if (appointment.MaximumTotalTerms <= 0)
			{
				sb.AppendLine(
					$"There is a term limit of {appointment.MaximumConsecutiveTerms.ToString("N0", actor).ColourValue()} consecutive {(appointment.MaximumConsecutiveTerms == 1 ? "term" : "terms")}.");
			}
			else
			{
				sb.AppendLine(
					$"There is a life-time term limit of {appointment.MaximumTotalTerms.ToString("N0", actor).ColourValue()} term{(appointment.MaximumTotalTerms == 1 ? "" : "s")} and/or {appointment.MaximumConsecutiveTerms.ToString("N0", actor).ColourValue()} consecutive {(appointment.MaximumConsecutiveTerms == 1 ? "term" : "terms")}.");
			}

			sb.AppendLine(appointment.IsSecretBallot ? "This is a secret ballot." : "This is an open ballot and all votes cast are public.");

			var votes = appointment.NumberOfVotes(actor);
			sb.AppendLine(
				$"You {(appointment.CanNominate(actor).Truth ? "are" : "are not")} eligable to nominate and {(votes <= 0 ? "cannot vote" : $"have {votes.ToString("N0", actor).ColourValue()} vote{(votes == 1 ? "" : "s")}")} in elections for this position.");

			var primaryElection = ClanCommandUtilities.GetPrimaryOpenElection(appointment);
			var byElection = ClanCommandUtilities.GetFirstOpenByElection(appointment);
			if (byElection is not null)
			{
				switch (byElection.ElectionStage)
				{
					case ElectionStage.Preelection:
						sb.AppendLine(
							$"By-election #{byElection.Id.ToString("N0", actor)} for {byElection.NumberOfAppointments.ToString("N0", actor).ColourValue()} {(byElection.NumberOfAppointments == 1 ? "position" : "positions")} opening for nominations on {byElection.NominationStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
						break;
					case ElectionStage.Nomination:
						sb.AppendLine(
							$"By-election #{byElection.Id.ToString("N0", actor)} for {byElection.NumberOfAppointments.ToString("N0", actor).ColourValue()} {(byElection.NumberOfAppointments == 1 ? "position" : "positions")} is open for nominations, with voting commencing on {byElection.VotingStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
						break;
					case ElectionStage.Voting:
						sb.AppendLine(
							$"By-election #{byElection.Id.ToString("N0", actor)} for {byElection.NumberOfAppointments.ToString("N0", actor).ColourValue()} {(byElection.NumberOfAppointments == 1 ? "position" : "positions")} is open for voting, with votes closing on {byElection.VotingEndDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
						break;
				}
			}

			if (primaryElection is null)
			{
				if (byElection is null)
				{
					sb.AppendLine("There is no currently scheduled primary election for this position.");
				}

				continue;
			}

			switch (primaryElection.ElectionStage)
			{
				case ElectionStage.Preelection:
					sb.AppendLine(
						$"The next election will open for nominations on {primaryElection.NominationStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
					break;
				case ElectionStage.Nomination:
					sb.AppendLine(
						$"Election #{primaryElection.Id.ToString("N0", actor)} is open for nominations, with voting commencing on {primaryElection.VotingStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
					break;
				case ElectionStage.Voting:
					sb.AppendLine(
						$"Election #{primaryElection.Id.ToString("N0", actor)} is open for voting, with votes closing on {primaryElection.VotingEndDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
					break;
				case ElectionStage.Preinstallation:
					sb.AppendLine(
						$"Election #{primaryElection.Id.ToString("N0", actor)} has finished and the elected will commence their terms on {primaryElection.ResultsInEffectDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
					break;
			}
		}

		return sb.ToString();
	}

	public void ShowElectionHistory(ICharacter actor, StringStack command)
	{
		var actorMembership = ActorMembership(actor);
		if (!actor.IsAdministrator(PermissionLevel.Admin) && !CanViewOfficeHolders(actor, actorMembership))
		{
			actor.OutputHandler.Send($"You are not authorised to view elections in {FullName.ColourName()}.");
			return;
		}

		if (Appointments.All(x => !x.IsAppointedByElection))
		{
			actor.OutputHandler.Send($"{FullName.ColourName()} does not have elections for any positions.");
			return;
		}

		var appointments = Appointments.Where(x => x.IsAppointedByElection).ToList();
		if (!command.IsFinished)
		{
			var appointment = Appointments.GetByIdOrName(command.SafeRemainingArgument);
			if (appointment is null)
			{
				actor.OutputHandler.Send($"{FullName.ColourName()} has no such appointment.");
				return;
			}

			if (!appointment.IsAppointedByElection)
			{
				actor.OutputHandler.Send($"The position {appointment.Name.ColourName()} is not controlled by elections.");
				return;
			}

			appointments = [appointment];
		}

		var sb = new StringBuilder();
		foreach (var appointment in appointments)
		{
			if (sb.Length > 0)
			{
				sb.AppendLine();
			}

			sb.AppendLine($"Election history for the {appointment.Name.ColourName()} position in {FullName.ColourName()}:");
			sb.AppendLine();
			foreach (var election in appointment.Elections.OrderByDescending(x => x.ResultsInEffectDate))
			{
				sb.Append($"#{election.Id.ToString("N0", actor)}) {(election.IsByElection ? "By-Election" : "Primary election")} of {election.NumberOfAppointments.ToString("N0", actor)} positions");
				switch (election.ElectionStage)
				{
					case ElectionStage.Preelection:
						sb.AppendLine($" due to begin {election.NominationStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
						break;
					case ElectionStage.Nomination:
						sb.AppendLine($" open for nominations until {election.VotingStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
						break;
					case ElectionStage.Voting:
						sb.AppendLine($" open for voting until {election.VotingEndDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
						break;
					case ElectionStage.Preinstallation:
						sb.AppendLine($" closed, with electors taking office on {election.ResultsInEffectDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
						break;
					case ElectionStage.Finalised:
						sb.AppendLine($" finished ({election.Victors.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).DefaultIfEmpty("no victors".Colour(Telnet.Red)).ListToString(conjunction: "", twoItemJoiner: ", ")})");
						break;
				}
			}
		}

		actor.OutputHandler.Send(sb.ToString(), false, true);
	}

	public void Nominate(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an election ID or a position for which you wish to nominate.");
			return;
		}

		if (!TryResolveElection(command.SafeRemainingArgument, out var election, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		switch (election!.ElectionStage)
		{
			case ElectionStage.Preelection:
				actor.OutputHandler.Send(
					$"The nomination period for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()} will not begin until {election.NominationStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				return;
			case ElectionStage.Nomination:
				break;
			default:
				actor.OutputHandler.Send(
					$"The nomination period for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()} has closed.");
				return;
		}

		var actorMembership = ActorMembership(actor);
		if (actorMembership is null)
		{
			actor.OutputHandler.Send("You are not a member of that clan.");
			return;
		}

		if (election.Nominees.Any(x => x.MemberId == actor.Id))
		{
			actor.OutputHandler.Send(
				$"You are already a candidate for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
			return;
		}

		var (truth, nominationError) = election.Appointment.CanNominate(actor);
		if (!truth)
		{
			actor.OutputHandler.Send(nominationError);
			return;
		}

		election.Nominate(actorMembership);
		actor.OutputHandler.Send(
			$"You nominate yourself as a candidate for the position of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
	}

	public void WithdrawNomination(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an election ID or a position for which you wish to withdraw your nomination.");
			return;
		}

		if (!TryResolveElection(command.SafeRemainingArgument, out var election, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		switch (election!.ElectionStage)
		{
			case ElectionStage.Preelection:
				actor.OutputHandler.Send(
					$"The nomination period for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()} will not begin until {election.NominationStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				return;
			case ElectionStage.Nomination:
				break;
			default:
				actor.OutputHandler.Send(
					$"The nomination period for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()} has closed.");
				return;
		}

		if (!election.Nominees.Any(x => x.MemberId == actor.Id))
		{
			actor.OutputHandler.Send(
				$"You are not a candidate for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
			return;
		}

		var actorMembership = ActorMembership(actor);
		if (actorMembership is null)
		{
			actor.OutputHandler.Send("You are not a member of that clan.");
			return;
		}

		election.WithdrawNomination(actorMembership);
		actor.OutputHandler.Send(
			$"You have withdrawn your candidacy for the election of {election.Appointment.Title(actor).ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
	}

	public void Vote(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("You must either specify an election ID or a position for which you wish to vote.");
			return;
		}

		IElection? election;
		var firstArgument = command.PopSpeech();
		if (!TryResolveElection(firstArgument, out election, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var nomineeText = command.SafeRemainingArgument;

		switch (election!.ElectionStage)
		{
			case ElectionStage.Voting:
				break;
			case ElectionStage.Preelection:
			case ElectionStage.Nomination:
				actor.OutputHandler.Send(
					$"The voting period for the election of {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()} will not begin until {election.VotingStartDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}.");
				return;
			default:
				actor.OutputHandler.Send(
					$"The voting period for the election of {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()} has closed.");
				return;
		}

		var votes = election.Appointment.NumberOfVotes(actor);
		if (votes <= 0)
		{
			actor.OutputHandler.Send(
				$"You are not entitled to vote in the election of {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
			return;
		}

		if (string.IsNullOrWhiteSpace(nomineeText))
		{
			ShowElectionNominees(actor, election, "Which nominee do you want to cast your vote for?");
			return;
		}

		var nomineeName = election.Nominees.Select(x => x.PersonalName).GetName(nomineeText);
		if (nomineeName is null)
		{
			ShowElectionNominees(actor, election,
				$"The supplied name is not a valid candidate in the election for {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()}.");
			return;
		}

		var actorMembership = ActorMembership(actor);
		if (actorMembership is null)
		{
			actor.OutputHandler.Send("You are not a member of that clan.");
			return;
		}

		var voteChoice = election.Nominees.First(x => x.PersonalName == nomineeName);
		var verb = election.Votes.Any(x => x.Voter.MemberId == actor.Id) ? "change" : "cast";
		var particle = election.Votes.Any(x => x.Voter.MemberId == actor.Id) ? "to" : "for";
		election.Vote(actorMembership, voteChoice, votes);
		actor.OutputHandler.Send(
			$"You {verb} your {(votes == 1 ? "vote" : $"{votes.ToString("N0", actor)} votes")} in the election for {election.Appointment.Name.ColourName()} in {election.Appointment.Clan.FullName.ColourName()} {particle} {nomineeName.GetName(NameStyle.FullName)}.");
	}

	public void Appoint(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to give an appointment to?");
			return;
		}

		var targetText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("To which position do you want to appoint them?");
			return;
		}

		var appointmentText = command.SafeRemainingArgument;
		if (!HasPrivilege(actor, ClanPrivilegeType.CanAppoint, out var actorMembership))
		{
			actor.OutputHandler.Send("You are not allowed to appoint people to positions in that clan.");
			return;
		}

		var targetMembership = GetActiveMembership(targetText, actor, out var targetActor);
		if (targetMembership is null)
		{
			actor.OutputHandler.Send("There is no such member for you to appoint.");
			return;
		}

		var appointment = Appointments.GetByIdOrName(appointmentText);
		if (appointment is null)
		{
			actor.OutputHandler.Send("There is no such appointment in that clan.");
			return;
		}

		if (appointment.IsAppointedByElection)
		{
			actor.OutputHandler.Send(
				$"The position of {appointment.Name.TitleCase().ColourName()} is controlled by elections rather than direct appointments.");
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.Admin) && appointment.ParentPosition is not null &&
		    !IsVisibleThroughAppointmentChain(actorMembership, appointment.ParentPosition))
		{
			actor.OutputHandler.Send(
				$"The position of {appointment.Name.TitleCase().Colour(Telnet.Green)} can only be appointed by {(appointment.ParentPosition.MaximumSimultaneousHolders > 1 ? appointment.ParentPosition.Name.TitleCase().A_An(colour: Telnet.Green) : appointment.ParentPosition.Name.TitleCase().Colour(Telnet.Green))}.");
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.Admin) && appointment.MinimumRankToAppoint is not null &&
		    (actorMembership is null || appointment.MinimumRankToAppoint.RankNumber > actorMembership.Rank.RankNumber) &&
		    !IsVisibleThroughAppointmentChain(actorMembership, appointment.ParentPosition))
		{
			actor.Send("You must hold at least the rank of {0} before you can appoint them to that position.",
				appointment.MinimumRankToAppoint.Title(actor).TitleCase().Colour(Telnet.Green));
			return;
		}

		if (appointment.MinimumRankToHold is not null &&
		    appointment.MinimumRankToHold.RankNumber > targetMembership.Rank.RankNumber)
		{
			actor.Send("They must hold at least the rank of {0} before you can appoint them to that position.",
				appointment.MinimumRankToHold.Name.TitleCase().Colour(Telnet.Green));
			return;
		}

		if (targetMembership.Appointments.Contains(appointment))
		{
			actor.OutputHandler.Send("They have already been appointed to that position.");
			return;
		}

		if (!FreePosition(appointment))
		{
			actor.OutputHandler.Send(
				"They cannot be appointed to that position as there is a limited number of holders at any one time.");
			return;
		}

		targetMembership.Appointments.Add(appointment);
		targetMembership.Changed = true;
		if (targetActor is not null)
		{
			actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
					$"@ appoint|appoints $0 to the position of {appointment.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {FullName.TitleCase().Colour(Telnet.Green)}.",
					actor, targetActor),
				perceiver => perceiver is ICharacter pChar &&
				            (pChar.ClanMemberships.Any(x => x.Clan == this) ||
				             pChar.PermissionLevel >= PermissionLevel.JuniorAdmin)));
			return;
		}

		actor.Send("You appoint {0} to the position of {1} in {2}.",
			targetMembership.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green),
			appointment.Name.TitleCase().Colour(Telnet.Green), FullName.TitleCase().Colour(Telnet.Green));
	}

	public void Dismiss(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you want to dismiss from an appointment?");
			return;
		}

		var targetText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("From which position do you want to dismiss them?");
			return;
		}

		var appointmentText = command.SafeRemainingArgument;
		if (!HasPrivilege(actor, ClanPrivilegeType.CanDismiss, out var actorMembership))
		{
			actor.OutputHandler.Send("You are not allowed to dismiss people from positions in that clan.");
			return;
		}

		var targetMembership = GetActiveMembership(targetText, actor, out var targetActor);
		if (targetMembership is null)
		{
			actor.OutputHandler.Send("There is no such member for you to dismiss.");
			return;
		}

		var appointment = Appointments.GetByIdOrName(appointmentText);
		if (appointment is null)
		{
			actor.OutputHandler.Send("There is no such appointment in that clan.");
			return;
		}

		if (appointment.IsAppointedByElection)
		{
			actor.OutputHandler.Send(
				$"The position of {appointment.Name.TitleCase().ColourName()} is controlled by elections rather than direct appointments.");
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.Admin) && appointment.ParentPosition is not null &&
		    !IsVisibleThroughAppointmentChain(actorMembership, appointment.ParentPosition))
		{
			actor.OutputHandler.Send(
				$"The position of {appointment.Name.TitleCase().Colour(Telnet.Green)} can only be dismissed by {(appointment.ParentPosition.MaximumSimultaneousHolders > 1 ? appointment.ParentPosition.Name.TitleCase().A_An(colour: Telnet.Green) : appointment.ParentPosition.Name.TitleCase().Colour(Telnet.Green))}.");
			return;
		}

		if (!actor.IsAdministrator(PermissionLevel.Admin) && appointment.MinimumRankToAppoint is not null &&
		    (actorMembership is null || appointment.MinimumRankToAppoint.RankNumber > actorMembership.Rank.RankNumber) &&
		    !IsVisibleThroughAppointmentChain(actorMembership, appointment.ParentPosition))
		{
			actor.Send("You must hold at least the rank of {0} before you can dismiss them from that position.",
				appointment.MinimumRankToAppoint.Title(actor).TitleCase().Colour(Telnet.Green));
			return;
		}

		if (!targetMembership.Appointments.Contains(appointment))
		{
			actor.OutputHandler.Send("They have not been appointed to that position.");
			return;
		}

		var jobs = actor.Gameworld.ActiveJobs
			.Where(x => !x.IsJobComplete &&
			            x.Character.Id != targetMembership.MemberId &&
			            x.Listing.ClanMembership == this &&
			            x.Listing.ClanAppointment == appointment)
			.ToList();
		if (jobs.Any())
		{
			actor.OutputHandler.Send(
				$"{targetMembership.MemberCharacter.HowSeen(actor, true)} cannot be dismissed from that appointment as they hold it by virtue of the job{(jobs.Count == 1 ? "" : "s")} {jobs.Select(x => x.Name.ColourValue()).ListToString()}.");
			return;
		}

		if (targetActor is not null)
		{
			actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
					$"@ dismiss|dismisses $0 from the position of {appointment.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {FullName.TitleCase().Colour(Telnet.Green)}.",
					actor, targetActor),
				perceiver => perceiver is ICharacter pChar &&
				            (pChar.ClanMemberships.Any(x => x.Clan == this) ||
				             pChar.PermissionLevel >= PermissionLevel.JuniorAdmin)));
		}
		else
		{
			actor.Send("You dismiss {0} from the position of {1} in {2}.",
				targetMembership.PersonalName.GetName(NameStyle.FullName).Colour(Telnet.Green),
				appointment.Name.TitleCase().Colour(Telnet.Green), FullName.TitleCase().Colour(Telnet.Green));
		}

		DismissAppointment(targetMembership, appointment);
	}

	public void SubmitControl(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which appointment in that clan do you wish to submit to external control?");
			return;
		}

		var appointmentText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which other clan do you wish to submit control of that appointment to?");
			return;
		}

		var otherClanText = command.PopSpeech();
		string? controllingAppointmentText = null;
		var maximumNumber = 1;
		if (!command.IsFinished)
		{
			var extraText = command.PopSpeech();
			if (command.IsFinished)
			{
				if (!int.TryParse(extraText, out maximumNumber) || maximumNumber < 0)
				{
					controllingAppointmentText = extraText;
					maximumNumber = 1;
				}
			}
			else
			{
				controllingAppointmentText = extraText;
				if (!int.TryParse(command.SafeRemainingArgument, out maximumNumber) || maximumNumber < 0)
				{
					actor.OutputHandler.Send(
						"If you specify a maximum number of appointees for that external control, it must be a valid number.");
					return;
				}
			}
		}

		if (!HasPrivilege(actor, ClanPrivilegeType.CanSubmitClan, out _))
		{
			actor.OutputHandler.Send("You are not allowed to submit appointments to external control in that clan.");
			return;
		}

		var appointment = Appointments.GetByIdOrName(appointmentText);
		if (appointment is null)
		{
			actor.OutputHandler.Send("There is no such appointment in that clan for you to submit.");
			return;
		}

		var filledSlots = Memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(appointment));
		if (appointment.MaximumSimultaneousHolders > 0 &&
		    appointment.MaximumSimultaneousHolders - filledSlots - maximumNumber < 0)
		{
			actor.OutputHandler.Send(
				"There are insufficient free appointees for that appointment to submit that number to external control.");
			return;
		}

		var targetClan = actor.Gameworld.Clans.GetClan(otherClanText);
		if (targetClan is null)
		{
			actor.OutputHandler.Send("There is no such other clan for you to submit an appointment to.");
			return;
		}

		if (targetClan == this)
		{
			actor.OutputHandler.Send("You cannot submit one of a clan's own appointments to itself.");
			return;
		}

		IAppointment? controllingAppointment = null;
		if (!string.IsNullOrEmpty(controllingAppointmentText))
		{
			controllingAppointment = targetClan.Appointments.GetByIdOrName(controllingAppointmentText);
			if (controllingAppointment is null)
			{
				actor.OutputHandler.Send(
					$"{targetClan.FullName.ColourName()} has no such controlling appointment.");
				return;
			}
		}

		if (ExternalControls.Any(x => x.VassalClan == this && x.LiegeClan == targetClan && x.ControlledAppointment == appointment))
		{
			actor.OutputHandler.Send("That clan already exerts control over that appointment.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you wish to submit the control of appointments of the {appointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan? This is irreversible unless they decide to relinquish control. They can also transfer the control to others.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Submitting a position in a clan to the control of another",
			Keywords = new List<string> { "submit", "clan", "external" },
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to submit the control of appointments of the {appointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan.");
			},
			RejectAction = _ =>
			{
				actor.OutputHandler.Send(
					$"You decide not to submit the control of appointments of the {appointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan.");
			},
			AcceptAction = _ =>
			{
				using (new FMDB())
				{
					var dbitem = new MudSharp.Models.ExternalClanControl
					{
						ControlledAppointmentId = appointment.Id,
						VassalClanId = Id,
						LiegeClanId = targetClan.Id,
						ControllingAppointmentId = controllingAppointment?.Id,
						NumberOfAppointments = maximumNumber
					};
					FMDB.Context.ExternalClanControls.Add(dbitem);
					FMDB.Context.SaveChanges();
					new ExternalClanControl(dbitem, actor.Gameworld);
				}

				actor.OutputHandler.Send(string.Format("You submit control of {3}appointment {0} in {1} to {2}.",
					appointment.Name.TitleCase().ColourName(),
					FullName.TitleCase().ColourName(),
					targetClan.FullName.TitleCase().ColourName(),
					maximumNumber > 0 ? string.Format(actor, "{0:N0} appointees of ", maximumNumber) : ""));
			}
		}), TimeSpan.FromSeconds(120));
	}

	public void TransferControl(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which appointment in that clan do you wish to transfer from external control?");
			return;
		}

		var appointmentText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which other clan do you wish to transfer control of that appointment to?");
			return;
		}

		var otherClanText = command.PopSpeech();
		var liegeClanText = command.SafeRemainingArgument;
		if (!TryGetExternalControl(appointmentText, liegeClanText, actor, out var control, out var liegeClan, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		var targetClan = actor.Gameworld.Clans.GetClan(otherClanText);
		if (targetClan is null)
		{
			actor.OutputHandler.Send("There is no such other clan for you to transfer an appointment to.");
			return;
		}

		if (targetClan == this)
		{
			actor.OutputHandler.Send("You cannot transfer one of a clan's own appointments to itself.");
			return;
		}

		if (targetClan == liegeClan)
		{
			actor.OutputHandler.Send("You cannot transfer vassalage of one clan within the same clan.");
			return;
		}

		if (!CanManageExternalControl(actor, control!, liegeClan!, out _))
		{
			actor.OutputHandler.Send("You are not allowed to manage vassal positions in that clan.");
			return;
		}

		actor.OutputHandler.Send(
			$"Are you sure you wish to transfer the control of appointments of the {control!.ControlledAppointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan? This is irreversible unless they decide to relinquish control. They can also transfer the control to others.\n{Accept.StandardAcceptPhrasing}");
		actor.AddEffect(new Accept(actor, new GenericProposal
		{
			DescriptionString = "Transferring a position in a clan to the control of another",
			Keywords = new List<string> { "transfer", "clan", "external" },
			ExpireAction = () =>
			{
				actor.OutputHandler.Send(
					$"You decide not to transfer the control of appointments of the {control.ControlledAppointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan.");
			},
			RejectAction = _ =>
			{
				actor.OutputHandler.Send(
					$"You decide not to transfer the control of appointments of the {control.ControlledAppointment.Name.TitleCase().ColourName()} position in {FullName.TitleCase().ColourName()} to the {targetClan.FullName.TitleCase().ColourName()} clan.");
			},
			AcceptAction = _ =>
			{
				using (new FMDB())
				{
					var dbitem = new MudSharp.Models.ExternalClanControl
					{
						ControlledAppointmentId = control.ControlledAppointment.Id,
						VassalClanId = Id,
						LiegeClanId = targetClan.Id,
						NumberOfAppointments = control.NumberOfAppointments
					};
					foreach (var character in control.Appointees)
					{
						dbitem.ExternalClanControlsAppointments.Add(new ExternalClanControlsAppointment
						{
							VassalClanId = Id,
							LiegeClanId = targetClan.Id,
							ControlledAppointmentId = control.ControlledAppointment.Id,
							CharacterId = character.MemberId
						});
					}

					FMDB.Context.ExternalClanControls.Add(dbitem);
					FMDB.Context.SaveChanges();
					new ExternalClanControl(dbitem, actor.Gameworld);
				}

				ExternalControls.Remove(control);
				liegeClan!.ExternalControls.Remove(control);
				control.Delete();

				actor.OutputHandler.Send(string.Format("You transfer control of {3}appointment {0} in {1} to {2}.",
					control.ControlledAppointment.Name.TitleCase().ColourName(),
					FullName.TitleCase().ColourName(),
					targetClan.FullName.TitleCase().ColourName(),
					control.NumberOfAppointments > 0 ? string.Format(actor, "{0:N0} appointees of ", control.NumberOfAppointments) : ""));
			}
		}), TimeSpan.FromSeconds(120));
	}

	public void ReleaseControl(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which appointment in that clan do you wish to release from external control?");
			return;
		}

		var appointmentText = command.PopSpeech();
		var liegeClanText = command.SafeRemainingArgument;
		if (!TryGetExternalControl(appointmentText, liegeClanText, actor, out var control, out var liegeClan, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!CanManageExternalControl(actor, control!, liegeClan!, out _))
		{
			actor.OutputHandler.Send("You are not allowed to manage vassal positions in that clan.");
			return;
		}

		var safeControl = control!;
		var safeLiegeClan = liegeClan!;
		ExternalControls.Remove(safeControl);
		safeLiegeClan.ExternalControls.Remove(safeControl);
		safeControl.Delete();
		actor.OutputHandler.Send(
			$"You release control of appointment {safeControl.ControlledAppointment.Name.TitleCase().ColourName()} in {FullName.TitleCase().ColourName()} by {safeLiegeClan.FullName.TitleCase().ColourName()}.");
	}

	public void SetControllingAppointment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which appointment in that vassal clan do you wish to assign a controlling liege appointment to?");
			return;
		}

		var appointmentText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send(
				"Which appointment in the liege clan should control that vassal appointment, or none to clear it?");
			return;
		}

		var controllingAppointmentText = command.PopSpeech();
		var liegeClanText = command.SafeRemainingArgument;
		if (!TryGetExternalControl(appointmentText, liegeClanText, actor, out var control, out var liegeClan, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!CanManageExternalControl(actor, control!, liegeClan!, out _))
		{
			actor.OutputHandler.Send("You are not allowed to manage vassal positions in that clan.");
			return;
		}

		IAppointment? controllingAppointment = null;
		if (!controllingAppointmentText.EqualTo("none") && !controllingAppointmentText.EqualTo("clear"))
		{
			controllingAppointment = liegeClan!.Appointments.GetByIdOrName(controllingAppointmentText);
			if (controllingAppointment is null)
			{
				actor.OutputHandler.Send(
					$"{liegeClan.FullName.ColourName()} has no such appointment to use as the controlling appointment.");
				return;
			}
		}

		control!.ControllingAppointment = controllingAppointment;
		control.Changed = true;
		control.Save();
		actor.OutputHandler.Send(
			$"The {control.ControlledAppointment.Name.TitleCase().ColourName()} appointment in {FullName.TitleCase().ColourName()} is now controlled by {(controllingAppointment is null ? $"the {liegeClan!.FullName.TitleCase().ColourName()} clan as a whole" : $"{controllingAppointment.Name.TitleCase().ColourName()} in {liegeClan!.FullName.TitleCase().ColourName()}")}.");
	}

	public void AppointExternal(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you wish to appoint to a position in that vassal clan?");
			return;
		}

		var targetText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("To which position do you wish to appoint them?");
			return;
		}

		var appointmentText = command.PopSpeech();
		var liegeClanText = command.SafeRemainingArgument;
		if (!TryGetExternalControl(appointmentText, liegeClanText, actor, out var control, out var liegeClan, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!CanManageExternalControl(actor, control!, liegeClan!, out var actorMembership))
		{
			if (control!.ControllingAppointment is not null)
			{
				actor.Send("Only someone who holds the {0} position can appoint anyone to that vassal position.",
					control.ControllingAppointment.Name.TitleCase().Colour(Telnet.Green));
			}
			else
			{
				actor.Send("You are not authorised to appoint anyone to that vassal position.");
			}

			return;
		}

		var targetActor = actor.TargetActor(targetText);
		if (targetActor is null)
		{
			actor.OutputHandler.Send("You do not see anyone like that to appoint to a position.");
			return;
		}

		var targetMembership = Memberships.FirstOrDefault(x => !x.IsArchivedMembership && x.MemberId == targetActor.Id);
		if (targetMembership is not null && targetMembership.Appointments.Contains(control!.ControlledAppointment))
		{
			actor.OutputHandler.Send("They already hold that position, and so cannot be appointed again.");
			return;
		}

		if (control!.NumberOfAppointments > 0 && control.NumberOfAppointments <= control.Appointees.Count)
		{
			actor.OutputHandler.Send(
				"The maximum number of appointments to that position through that relationship has been reached. You must first dismiss existing appointees.");
			return;
		}

		if (targetMembership is null)
		{
			var rank = control.ControlledAppointment.MinimumRankToHold ?? Ranks.FirstMin(x => x.RankNumber);
			var archived = Memberships.FirstOrDefault(x => x.IsArchivedMembership && x.MemberId == targetActor.Id);
			if (archived is not null)
			{
				archived.IsArchivedMembership = false;
				archived.Changed = true;
				targetActor.AddMembership(archived);
				if (archived.Rank!.RankNumber < rank!.RankNumber)
				{
					archived.Rank = rank;
				}

				targetMembership = archived;
			}
			else
			{
				using (new FMDB())
				{
					var dbitem = new MudSharp.Models.ClanMembership
					{
						CharacterId = targetActor.Id,
						ClanId = Id,
						RankId = rank!.Id,
						PaygradeId = rank.Paygrades.Any() ? rank.Paygrades.First().Id : (long?)null,
						PersonalName = targetActor.CurrentName.SaveToXml().ToString(),
						JoinDate = Calendar.CurrentDate.GetDateString()
					};
					FMDB.Context.ClanMemberships.Add(dbitem);
					FMDB.Context.SaveChanges();
					targetMembership = new ClanMembership(dbitem, this, targetActor.Gameworld);
					targetActor.AddMembership(targetMembership);
					Memberships.Add(targetMembership);
				}
			}
		}
		else if (control.ControlledAppointment.MinimumRankToHold is not null &&
		         targetMembership.Rank.RankNumber < control.ControlledAppointment.MinimumRankToHold.RankNumber)
		{
			SetRank(targetMembership, control.ControlledAppointment.MinimumRankToHold);
		}

		using (new FMDB())
		{
			var dbappointment = FMDB.Context.ExternalClanControls.Find(Id, liegeClan!.Id, control.ControlledAppointment.Id);
			dbappointment!.ExternalClanControlsAppointments.Add(new ExternalClanControlsAppointment
			{
				CharacterId = targetActor.Id
			});
			FMDB.Context.SaveChanges();
		}

		control.Appointees.Add(targetMembership);
		targetMembership.Appointments.Add(control.ControlledAppointment);
		targetMembership.Changed = true;

		actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
				$"@ appoint|appoints $0 to the position of {control.ControlledAppointment.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {FullName.TitleCase().Colour(Telnet.Green)} on behalf of {liegeClan.FullName.TitleCase().Colour(Telnet.Green)}.",
				actor, targetActor),
			perceiver => perceiver is ICharacter pChar &&
			            (pChar.ClanMemberships.Any(x => x.Clan == this || x.Clan == liegeClan) ||
			             pChar.PermissionLevel >= PermissionLevel.JuniorAdmin)));
	}

	public void DismissExternal(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Who do you wish to dismiss from a position in that vassal clan?");
			return;
		}

		var targetText = command.PopSpeech();
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("From which position do you wish to dismiss them?");
			return;
		}

		var appointmentText = command.PopSpeech();
		var liegeClanText = command.SafeRemainingArgument;
		if (!TryGetExternalControl(appointmentText, liegeClanText, actor, out var control, out var liegeClan, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!CanManageExternalControl(actor, control!, liegeClan!, out _))
		{
			if (control!.ControllingAppointment is not null)
			{
				actor.Send("Only someone who holds the {0} position can dismiss anyone from that vassal position.",
					control.ControllingAppointment.Name.TitleCase().Colour(Telnet.Green));
			}
			else
			{
				actor.Send("You are not authorised to dismiss anyone from that vassal position.");
			}

			return;
		}

		var targetActor = actor.TargetActor(targetText);
		var targetMembership = targetActor is not null
			? targetActor.ClanMemberships.FirstOrDefault(x => x.Clan == this && !x.IsArchivedMembership)
			: control!.Appointees.FirstOrDefault(x =>
				x.PersonalName.GetName(NameStyle.FullName).Equals(targetText, StringComparison.InvariantCultureIgnoreCase));
		if (targetMembership is null || !control!.Appointees.Contains(targetMembership))
		{
			actor.OutputHandler.Send("There is no such member for you to dismiss that falls within your remit.");
			return;
		}

		using (new FMDB())
		{
			var dbappointment = FMDB.Context.ExternalClanControls.Find(Id, liegeClan!.Id, control.ControlledAppointment.Id);
			var dbAppointee = dbappointment!.ExternalClanControlsAppointments.FirstOrDefault(x => x.CharacterId == targetMembership.MemberId);
			if (dbAppointee is not null)
			{
				dbappointment.ExternalClanControlsAppointments.Remove(dbAppointee);
				FMDB.Context.SaveChanges();
			}
		}

		control.Appointees.Remove(targetMembership);
		DismissAppointment(targetMembership, control.ControlledAppointment);

		if (targetActor is not null)
		{
			actor.OutputHandler.Handle(new FilteredEmoteOutput(new Emote(
					$"@ dismiss|dismisses $0 from the position of {control.ControlledAppointment.Title(targetActor).TitleCase().Colour(Telnet.Green)} in {FullName.TitleCase().Colour(Telnet.Green)} on behalf of {liegeClan.FullName.TitleCase().Colour(Telnet.Green)}.",
					actor, targetActor),
				perceiver => perceiver is ICharacter pChar &&
				            (pChar.ClanMemberships.Any(x => x.Clan == this || x.Clan == liegeClan) ||
				             pChar.PermissionLevel >= PermissionLevel.JuniorAdmin)));
			return;
		}

		actor.OutputHandler.Send(
			$"You dismiss {targetMembership.PersonalName.GetName(NameStyle.FullName).TitleCase().Colour(Telnet.Green)} from the position of {control.ControlledAppointment.Name.TitleCase().Colour(Telnet.Green)} in {FullName.TitleCase().Colour(Telnet.Green)} on behalf of {liegeClan.FullName.TitleCase().Colour(Telnet.Green)}.");
	}

	private void ShowDefault(ICharacter actor, IClanMembership? actorMembership)
	{
		var canViewMembers = CanViewMembers(actor, actorMembership);
		var canViewOffices = CanViewOfficeHolders(actor, actorMembership);
		var canViewAboveOwnRank = CanViewAboveOwnRank(actor, actorMembership);
		var canViewFinances = CanViewFinances(actor, actorMembership);
		var sb = new StringBuilder();
		sb.AppendLine(FullName.TitleCase().GetLineWithTitleInner(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Alias: {Alias.Colour(Telnet.Green)}");
		sb.AppendLine($"Name: {Name.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine("Description:");
		sb.AppendLine();
		sb.AppendLine(Description.Wrap(80, "\t").NoWrap());
		sb.AppendLine();
		if (actor.IsAdministrator(PermissionLevel.Admin))
		{
			sb.AppendLine($"Discord Channel: {DiscordChannelId?.ToString("F0", actor).ColourValue() ?? "None".ColourError()}");
			sb.AppendLine($"Treasury Cells:\n{TreasuryCells.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("None").ListToLines(true)}");
			sb.AppendLine();
			sb.AppendLine($"Administration Cells:\n{AdministrationCells.Select(x => x.GetFriendlyReference(actor)).DefaultIfEmpty("None").ListToLines(true)}");
			sb.AppendLine();
		}

		sb.AppendLine("Ranks:");
		var ranks = (canViewAboveOwnRank
			? Ranks
			: Ranks.Where(x => x.RankNumber <= (actorMembership?.Rank.RankNumber ?? 0))).ToList();
		sb.AppendLine(
			canViewMembers
				? StringUtilities.GetTextTable(
					from rank in ranks
					orderby rank.RankNumber
					select new[]
					{
						rank.RankNumber.ToString(actor),
						rank.Name.TitleCase(),
						(rank.RankPath ?? "").TitleCase(),
						Memberships.Count(x => x.Rank == rank && !x.IsArchivedMembership).ToString(actor),
						rank.Titles.Except(rank.Name).Select(x => x.TitleCase()).ListToString(conjunction: "", twoItemJoiner: ", "),
						rank.Paygrades.Select(x => x.Abbreviation).ListToString(conjunction: "", twoItemJoiner: ", ")
					},
					new[] { "Rank#", "Name", "Path", "No.", "Alternate Names", "Paygrades" },
					actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 5,
					unicodeTable: actor.Account.UseUnicode)
				: StringUtilities.GetTextTable(
					from rank in ranks
					orderby rank.RankNumber
					select new[]
					{
						rank.RankNumber.ToString(actor),
						rank.Name.TitleCase(),
						(rank.RankPath ?? "").TitleCase(),
						rank.Titles.Except(rank.Name).Select(x => x.TitleCase()).ListToString(conjunction: "", twoItemJoiner: ", "),
						rank.Paygrades.Select(x => x.Abbreviation).ListToString(conjunction: "", twoItemJoiner: ", ")
					},
					new[] { "Rank#", "Name", "Path", "Alternate Names", "Paygrades" },
					actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 4,
					unicodeTable: actor.Account.UseUnicode));

		var appointments = (canViewAboveOwnRank
			? Appointments
			: Appointments.Where(x => IsVisibleThroughAppointmentChain(actorMembership, x))).ToList();
		sb.AppendLine("Appointments:");
		sb.AppendLine(
			canViewOffices
				? StringUtilities.GetTextTable(
					from appointment in appointments
					orderby appointment.MinimumRankToHold?.RankNumber ?? -1 descending,
						appointment.MinimumRankToAppoint?.RankNumber ?? -1 descending
					select new[]
					{
						appointment.Name.TitleCase(),
						appointment.MinimumRankToHold?.Name.TitleCase() ?? "None",
						appointment.MinimumRankToAppoint?.Name.TitleCase() ?? "None",
						appointment.MaximumSimultaneousHolders.ToString(actor),
						Memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(appointment)).ToString(actor),
						appointment.Paygrade?.Abbreviation ?? "None",
						appointment.ParentPosition?.Name.TitleCase() ?? "None",
						appointment.IsAppointedByElection.ToColouredString()
					},
					new[] { "Name", "Min Rank", "Appointer", "Max No.", "No.", "Paygrade", "Parent", "Elected?" },
					actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode)
				: StringUtilities.GetTextTable(
					from appointment in appointments
					orderby appointment.MinimumRankToHold?.RankNumber ?? -1 descending,
						appointment.MinimumRankToAppoint?.RankNumber ?? -1 descending
					select new[]
					{
						appointment.Name.TitleCase(),
						appointment.MinimumRankToHold?.Name.TitleCase() ?? "None",
						appointment.MinimumRankToAppoint?.Name.TitleCase() ?? "None",
						appointment.MaximumSimultaneousHolders.ToString(actor),
						appointment.Paygrade?.Abbreviation ?? "None",
						appointment.ParentPosition?.Name.TitleCase() ?? "None",
						appointment.IsAppointedByElection.ToColouredString()
					},
					new[] { "Name", "Min Rank", "Appointer", "Max No.", "Paygrade", "Parent", "Elected?" },
					actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));

		var paygrades = (canViewAboveOwnRank
			? Paygrades
			: Paygrades.Where(x =>
				Ranks.Any(y => y.Paygrades.Contains(x) && y.RankNumber <= (actorMembership?.Rank.RankNumber ?? 0)) ||
				Appointments.Any(y => y.Paygrade == x && IsVisibleThroughAppointmentChain(actorMembership, y)))).ToList();
		sb.AppendLine("Paygrades:");
		if (canViewMembers && canViewFinances)
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from paygrade in paygrades
				orderby paygrade.PayCurrency.Id, paygrade.PayAmount
				select new[]
				{
					paygrade.Abbreviation,
					paygrade.Name.TitleCase(),
					paygrade.PayCurrency.Name.TitleCase(),
					paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short),
					(Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
					 Memberships.Sum(x => x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))).ToString(actor),
					paygrade.PayCurrency.Describe(
						paygrade.PayAmount *
						(Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
						 Memberships.Sum(x => x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))),
						CurrencyDescriptionPatternType.Short)
				},
				new[] { "Abbreviation", "Name", "Currency", "Amount", "No.", "Total Per Pay" },
				actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 5,
				unicodeTable: actor.Account.UseUnicode));
		}
		else if (canViewMembers)
		{
			sb.AppendLine(StringUtilities.GetTextTable(
				from paygrade in paygrades
				orderby paygrade.PayCurrency.Id, paygrade.PayAmount
				select new[]
				{
					paygrade.Abbreviation,
					paygrade.Name.TitleCase(),
					paygrade.PayCurrency.Name.TitleCase(),
					paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short),
					(Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
					 Memberships.Sum(x => x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))).ToString(actor)
				},
				new[] { "Abbreviation", "Name", "Currency", "Amount", "No." },
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
		}
		else if (canViewFinances)
		{
			sb.AppendLine(StringUtilities.GetTextTable(
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
						(Memberships.Count(x => !x.IsArchivedMembership && x.Paygrade == paygrade) +
						 Memberships.Sum(x => x.Appointments.Count(y => !x.IsArchivedMembership && y.Paygrade == paygrade))),
						CurrencyDescriptionPatternType.Short)
				},
				new[] { "Abbreviation", "Name", "Currency", "Amount", "Total Per Pay" },
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
		}
		else
		{
			sb.AppendLine(StringUtilities.GetTextTable(
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
				actor.Account.LineFormatLength, colour: Telnet.Green, unicodeTable: actor.Account.UseUnicode));
		}

		sb.AppendLine();
		if (ExternalControls.Any(x => x.VassalClan == this))
		{
			sb.AppendLine("External Controls (Over This Clan):");
			sb.AppendLine(StringUtilities.GetTextTable(
				from external in ExternalControls.Where(x => x.VassalClan == this)
				select new[]
				{
					external.ControlledAppointment.Name.TitleCase(),
					external.LiegeClan.FullName.TitleCase(),
					external.ControllingAppointment?.Name.TitleCase() ?? "None",
					external.NumberOfAppointments > 0 ? external.NumberOfAppointments.ToString("N0", actor) : "Unlimited",
					external.Appointees.Count.ToString("N0", actor)
				},
				new[] { "Appointment", "Liege Clan", "Liege Appointment", "Max No.", "No." },
				actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 2,
				unicodeTable: actor.Account.UseUnicode));
			sb.AppendLine();
		}

		if (ExternalControls.Any(x => x.LiegeClan == this))
		{
			sb.AppendLine("External Controls (Under This Clan):");
			sb.AppendLine(StringUtilities.GetTextTable(
				from external in ExternalControls.Where(x => x.LiegeClan == this)
				select new[]
				{
					external.ControlledAppointment.Name.TitleCase(),
					external.VassalClan.FullName.TitleCase(),
					external.ControllingAppointment?.Name.TitleCase() ?? "None",
					external.NumberOfAppointments > 0 ? external.NumberOfAppointments.ToString("N0", actor) : "Unlimited",
					external.Appointees.Count.ToString("N0", actor)
				},
				new[] { "Appointment", "Vassal Clan", "Liege Appointment", "Max No.", "No." },
				actor.Account.LineFormatLength, colour: Telnet.Green, truncatableColumnIndex: 2,
				unicodeTable: actor.Account.UseUnicode));
			sb.AppendLine();
		}

		if (canViewFinances)
		{
			sb.AppendLine($"Default Bank Account: {ClanBankAccount?.AccountReference.Colour(Telnet.BoldCyan) ?? "None".Colour(Telnet.Red)}");
			if (ClanBankAccount is not null)
			{
				sb.AppendLine(
					$"Default Account Balance: {ClanBankAccount.Currency.Describe(ClanBankAccount.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
			}

			var currencyPiles = TreasuryCells
				.SelectMany(x => x.GameItems.SelectMany(y => y.RecursiveGetItems<ICurrencyPile>()))
				.GroupBy(x => x.Currency)
				.Select(x => (Currency: x.Key, Value: x.Sum(pile => pile.Coins.Sum(coin => coin.Item1.Value * coin.Item2))))
				.ToList();
			if (currencyPiles.Any(x => x.Value > 0.0M))
			{
				sb.AppendLine(
					$"Treasury Balance: {currencyPiles.Select(x => x.Currency.Describe(x.Value, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()).ListToString()}");
			}

			var accounts = actor.Gameworld.Banks.SelectMany(x => x.BankAccounts.Where(y => y.AccountOwner == this)).ToList();
			if (accounts.Any())
			{
				sb.AppendLine("Clan Bank Accounts:");
				sb.AppendLine();
				foreach (var account in accounts)
				{
					sb.AppendLine(
						$"\t{account.AccountReference.Colour(Telnet.BoldCyan)} - {account.Currency.Describe(account.CurrentBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
				}
			}

			sb.AppendLine();
			sb.AppendLine(
				$"Total Commitment Salary Per Payday: {Paygrades.Select(x => x.PayCurrency).Distinct().Select(x => x.Describe(Paygrades.Where(y => y.PayCurrency == x).Sum(y => y.PayAmount * (Memberships.Count(z => !z.IsArchivedMembership && z.Paygrade == y) + Memberships.Sum(z => z.Appointments.Count(v => !z.IsArchivedMembership && v.Paygrade == y)))), CurrencyDescriptionPatternType.Short)).Select(x => x.Colour(Telnet.Green)).ListToString()}");
		}

		sb.AppendLine($"Clan Calendar: {Calendar.ShortName.TitleCase().ColourValue()}");
		sb.AppendLine($"Payday Interval: {PayInterval.Describe(Calendar).ColourValue()}");
		sb.AppendLine($"Next Payday: {Calendar.DisplayDate(NextPay.Date, CalendarDisplayMode.Short).Colour(Telnet.Green)} at {Calendar.FeedClock.DisplayTime(NextPay.Time, TimeDisplayTypes.Immortal).Colour(Telnet.Green)}");
		actor.OutputHandler.Send(sb.ToString());
	}

	private void ShowRank(ICharacter actor, IRank rank)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Rank {rank.Name.TitleCase().Colour(Telnet.Green)} ID #{rank.Id.ToString().Colour(Telnet.Green)} - {FullName.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine();
		sb.Append(new[]
		{
			$"Name: {rank.Name.TitleCase().Colour(Telnet.Green)}",
			$"Rank Number: {rank.RankNumber.ToString("N0", actor).Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Rank Path: {(rank.RankPath ?? "").Colour(Telnet.Green)}",
			$"Insignia: {(rank.InsigniaGameItem is not null ? $"#{rank.InsigniaGameItem.Id} rev {rank.InsigniaGameItem.RevisionNumber}".Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine();
		sb.AppendLine("Abbreviations:");
		foreach (var item in rank.AbbreviationsAndProgs)
		{
			sb.AppendLine(
				$"\tAbbreviation: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 is not null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 is not null ? $" (#{item.Item1.Id:N0})" : "")}");
		}

		sb.AppendLine();
		sb.AppendLine("Rank Names:");
		foreach (var item in rank.TitlesAndProgs)
		{
			sb.AppendLine(
				$"\tName: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 is not null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 is not null ? $" (#{item.Item1.Id:N0})" : "")}");
		}

		sb.AppendLine();
		sb.AppendLine("Paygrades:");
		foreach (var item in rank.Paygrades)
		{
			sb.AppendLine(
				$"\tPaygrade {item.Abbreviation.Colour(Telnet.Green)} ({item.Name.TitleCase().Colour(Telnet.Green)}) - {item.PayCurrency.Describe(item.PayAmount, CurrencyDescriptionPatternType.Short)}.");
		}

		sb.AppendLine();
		sb.AppendLine("Privileges:");
		sb.AppendLine("\t" + rank.Privileges);
		actor.OutputHandler.Send(sb.ToString());
	}

	private void ShowPaygrade(ICharacter actor, IPaygrade paygrade)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Paygrade {paygrade.Name.TitleCase().Colour(Telnet.Green)} ID #{paygrade.Id.ToString().Colour(Telnet.Green)} - {FullName.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine();
		sb.Append(new[]
		{
			$"Abbreviation: {paygrade.Abbreviation.Colour(Telnet.Green)}",
			$"Name: {paygrade.Name.TitleCase().Colour(Telnet.Green)}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Pays {paygrade.PayCurrency.Describe(paygrade.PayAmount, CurrencyDescriptionPatternType.Short).Colour(Telnet.Green)} in the {paygrade.PayCurrency.Name.Colour(Telnet.Green)} currency.");
		actor.OutputHandler.Send(sb.ToString());
	}

	private void ShowAppointment(ICharacter actor, IAppointment appointment)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Appointment {appointment.Name.TitleCase().Colour(Telnet.Green)} ID #{appointment.Id.ToString().Colour(Telnet.Green)} - {FullName.TitleCase().Colour(Telnet.Green)}");
		sb.AppendLine();
		sb.Append(new[]
		{
			$"Name: {appointment.Name.TitleCase().Colour(Telnet.Green)}",
			$"Insignia: {(appointment.InsigniaGameItem is not null ? $"#{appointment.InsigniaGameItem.Id} rev {appointment.InsigniaGameItem.RevisionNumber}".Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Min Rank: {(appointment.MinimumRankToHold is not null ? appointment.MinimumRankToHold.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
			$"Min Appointer Rank: {(appointment.MinimumRankToAppoint is not null ? appointment.MinimumRankToAppoint.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.Append(new[]
		{
			$"Parent: {(appointment.ParentPosition is not null ? appointment.ParentPosition.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}",
			$"Max Holders: {appointment.MaximumSimultaneousHolders.ToString("N0", actor).Colour(Telnet.Green)}"
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
		foreach (var item in appointment.AbbreviationsAndProgs)
		{
			sb.AppendLine(
				$"\tAbbreviation: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 is not null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 is not null ? $" (#{item.Item1.Id:N0})" : "")}");
		}

		sb.AppendLine();
		sb.AppendLine("Appointment Names:");
		foreach (var item in appointment.TitlesAndProgs)
		{
			sb.AppendLine(
				$"\tName: {item.Item2.Colour(Telnet.Green)} FutureProg: {(item.Item1 is not null ? item.Item1.FunctionName.Colour(Telnet.Cyan).FluentTagMXP("send", $"href='show futureprog {item.Item1.Id}'") : "None".Colour(Telnet.Red))}{(item.Item1 is not null ? $" (#{item.Item1.Id:N0})" : "")}");
		}

		sb.AppendLine();
		sb.AppendLine(appointment.Paygrade is not null
			? $"Paygrade: {appointment.Paygrade.Abbreviation.Colour(Telnet.Green)} ({appointment.Paygrade.Name.TitleCase().Colour(Telnet.Green)}) - {appointment.Paygrade.PayCurrency.Describe(appointment.Paygrade.PayAmount, CurrencyDescriptionPatternType.Short)}."
			: $"Paygrade: {"None".Colour(Telnet.Red)}");
		sb.AppendLine();
		sb.AppendLine("Privileges:");
		sb.AppendLine("\t" + appointment.Privileges);
		actor.OutputHandler.Send(sb.ToString());
	}

	private void ShowExternalControl(ICharacter actor, IExternalClanControl external)
	{
		var sb = new StringBuilder();
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
			$"Controlling Appointment: {(external.ControllingAppointment is not null ? external.ControllingAppointment.Name.TitleCase().Colour(Telnet.Green) : "None".Colour(Telnet.Red))}"
		}.ArrangeStringsOntoLines(2, (uint)actor.Account.LineFormatLength));
		sb.AppendLine(
			$"Maximum Appointees: {(external.NumberOfAppointments > 0 ? external.NumberOfAppointments.ToString("N0", actor) : "Unlimited".Colour(Telnet.Red))}");
		sb.AppendLine();
		if (external.Appointees.Any())
		{
			sb.AppendLine("Current Appointees:");
			foreach (var appointee in external.Appointees)
			{
				sb.AppendLine("\t" + appointee.PersonalName.GetName(NameStyle.FullName));
			}
		}

		actor.OutputHandler.Send(sb.ToString());
	}
}
