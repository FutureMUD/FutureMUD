using MudSharp.Accounts;

#nullable enable

namespace MudSharp.Community;

public partial class Clan
{
	public void Recall(ICharacter actor, StringStack command)
	{
		var targetText = command.PopSpeech();
		var appointment = Appointments.GetByIdOrName(command.SafeRemainingArgument);
		var membership = ActorMembership(actor);
		if (membership is not { IsArchivedMembership: false } || appointment is null ||
			!appointment.IsAppointedByElection || !appointment.CanBeRecalled ||
			appointment.CanInitiateRecallProg?.ExecuteBool(actor) != true)
		{
			actor.OutputHandler.Send("You are not eligible to initiate a recall for that appointment.");
			return;
		}

		var target = GetActiveMembership(targetText, actor, out _);
		if (target is null || !target.Appointments.Contains(appointment))
		{
			actor.OutputHandler.Send("That person does not currently hold that appointment.");
			return;
		}
		if (appointment.Elections.Any(x => x.IsRecall && !x.IsFinalised && x.RecallTarget == target))
		{
			actor.OutputHandler.Send("There is already an open recall ballot for that holder and appointment.");
			return;
		}
		if (appointment.RecallVotesProg is null || appointment.VotingPeriod <= TimeSpan.Zero)
		{
			actor.OutputHandler.Send("That appointment must have a recall voting prog and a positive voting period configured first.");
			return;
		}
		if (Gameworld.ActiveJobs.Any(x => !x.IsJobComplete && x.Character.Id == target.MemberId &&
			x.Listing.ClanMembership == this && x.Listing.ClanAppointment == appointment))
		{
			actor.OutputHandler.Send("That person holds the appointment through employment and cannot be recalled from it.");
			return;
		}

		var recall = new Election(appointment, false, 1, null, target);
		appointment.AddElection(recall);
		actor.OutputHandler.Send($"You initiate recall #{recall.Id.ToString("N0", actor).ColourValue()}. Use {($"clan vote {recall.Id} yes|no").ColourCommand()} to vote for removal or retention.");
		Gameworld.SystemMessage($"A recall ballot (#{recall.Id}) has opened for {target.Name.ColourName()} as {appointment.Name.ColourName()} in {FullName.ColourName()}. Voting lasts {appointment.VotingPeriod.Describe(actor).ColourValue()}.",
			ch => ch.ClanMemberships.Any(x => !x.IsArchivedMembership && x.Clan == this &&
				x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
	}

	private void VoteRecall(ICharacter actor, IElection election, string choice)
	{
		election.CheckElectionStage();
		if (election.IsFinalised || election.ElectionStage != ElectionStage.Voting)
		{
			actor.OutputHandler.Send("Voting in that recall ballot has closed.");
			return;
		}
		var member = ActorMembership(actor);
		var votes = member is { IsArchivedMembership: false }
			? election.Appointment.RecallVotesProg?.ExecuteInt(actor) ?? 0 : 0;
		if (votes <= 0)
		{
			actor.OutputHandler.Send("You are not eligible to vote in that recall ballot.");
			return;
		}
		if (!choice.EqualTo("yes") && !choice.EqualTo("no"))
		{
			actor.OutputHandler.Send("Vote YES to remove the holder or NO to retain them.");
			return;
		}
		election.Vote(member!, election.RecallTarget, choice.EqualTo("yes") ? votes : -votes);
		actor.OutputHandler.Send($"You cast {votes.ToString("N0", actor).ColourValue()} {(votes == 1 ? "vote" : "votes")} to {(choice.EqualTo("yes") ? "remove" : "retain")} the holder in recall #{election.Id.ToString("N0", actor)}.");
	}

	public void ConfirmElection(ICharacter actor, StringStack command, bool veto)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Specify an election ID or appointment.");
			return;
		}
		if (!TryResolveElection(command.SafeRemainingArgument, out var election, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		if (!ClanCommandUtilities.CanConfirmElection(actor, election!.Appointment))
		{
			actor.OutputHandler.Send("You do not hold the appointment required to confirm or veto that election.");
			return;
		}

		if (!(veto ? election.Veto(actor) : election.Confirm(actor)))
		{
			actor.OutputHandler.Send("That election is not awaiting confirmation.");
			return;
		}

		actor.OutputHandler.Send($"You {(veto ? "veto" : "confirm")} election #{election.Id.ToString("N0", actor).ColourValue()}.");
	}

	public void Resign(ICharacter actor, StringStack command)
	{
		var membership = ActorMembership(actor);
		if (membership is null || membership.IsArchivedMembership)
		{
			actor.OutputHandler.Send("You are not an active member of that clan.");
			return;
		}

		var appointment = Appointments.GetByIdOrName(command.SafeRemainingArgument);
		if (appointment is null || !membership.Appointments.Contains(appointment))
		{
			actor.OutputHandler.Send("You do not hold any such appointment in that clan.");
			return;
		}

		if (!appointment.CanResign)
		{
			actor.OutputHandler.Send("That appointment does not permit resignation.");
			return;
		}

		if (Gameworld.ActiveJobs.Any(x => !x.IsJobComplete &&
			CharacterInstanceIdentityComparer.SameIdentity(x.Character, actor) &&
			x.Listing.ClanMembership == this && x.Listing.ClanAppointment == appointment))
		{
			actor.OutputHandler.Send("You hold that appointment through employment. Leave the job before resigning the appointment.");
			return;
		}

		// A resignation also withdraws any outstanding candidacy, so a pending result cannot reinstall the member.
		foreach (var election in appointment.Elections.Where(x => !x.IsFinalised).ToList())
		{
			election.WithdrawNomination(membership);
		}

		DismissAppointment(membership, appointment);
		actor.OutputHandler.Send($"You resign as {appointment.Title(actor).ColourName()} in {FullName.ColourName()}.");
	}
}
