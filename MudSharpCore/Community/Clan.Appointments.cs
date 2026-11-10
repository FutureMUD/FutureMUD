using MudSharp.Accounts;

#nullable enable

namespace MudSharp.Community;

public partial class Clan
{
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
