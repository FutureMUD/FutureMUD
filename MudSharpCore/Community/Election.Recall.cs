using MudSharp.Character.Name;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;

#nullable enable

namespace MudSharp.Community;

public partial class Election
{
	private readonly Dictionary<string, bool> _recallLiegeApprovals = new();
	private static string RecallControlKey(IExternalClanControl control) =>
		FormattableString.Invariant($"{control.LiegeClan.Id}:{control.ControllingAppointment?.Id ?? 0}");

	public IEnumerable<(long ClanId, long? AppointmentId)> RecallConfirmationAuthorities => IsFinalised
		? _recallLiegeApprovals.Keys.Select(key =>
		{
			var parts = key.Split(':');
			var office = long.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
			return (long.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture), office == 0 ? (long?)null : office);
		})
		: ClanCommandUtilities.RecallControls(this).Select(x => (x.LiegeClan.Id, x.ControllingAppointment?.Id));

	private void RefreshRecallAuthorities()
	{
		if (!IsRecall || IsFinalised) return;
		var current = ClanCommandUtilities.RecallControls(this).Select(RecallControlKey).ToHashSet();
		foreach (var old in _recallLiegeApprovals.Keys.Where(x => !current.Contains(x)).ToList())
		{
			_recallLiegeApprovals.Remove(old);
			Changed = true;
		}
		foreach (var key in current)
			if (_recallLiegeApprovals.TryAdd(key, false)) Changed = true;
	}

	public void InvalidateRecallConfirmation(IExternalClanControl control)
	{
		if (!IsRecall || IsFinalised) return;
		var key = RecallControlKey(control);
		if (_recallLiegeApprovals.GetValueOrDefault(key))
		{
			_recallLiegeApprovals[key] = false;
			Changed = true;
		}
	}

	private bool RecallNeedsConfirmation =>
		(Appointment.RequiresElectionConfirmation && !IsConfirmed) ||
		_recallLiegeApprovals.Values.Any(x => !x);

	private bool ConfirmRecall(ICharacter actor)
	{
		RefreshRecallAuthorities();
		if (ClanCommandUtilities.CanConfirmElection(actor, Appointment)) IsConfirmed = true;
		foreach (var control in ClanCommandUtilities.RecallControls(this).Where(x => ClanCommandUtilities.CanManageExternalControl(actor, x)))
		{
			_recallLiegeApprovals[RecallControlKey(control)] = true;
		}
		Changed = true;
		AnnounceDecision(actor, RecallNeedsConfirmation ? "partially confirmed" : "confirmed");
		ElectionStage = ElectionStage.Preinstallation;
		CheckRecallStage();
		return true;
	}

	public bool IsRecall { get; protected set; }
	private double RecallThreshold { get; }
	public IClanMembership? RecallTarget => IsRecall ? _nominees.FirstOrDefault() : null;
	public bool RecallPassed => IsRecall && _votes.Count > 0 &&
		_votes.Where(x => x.Votes > 0).Sum(x => (long)x.Votes) /
		(double)_votes.Sum(x => Math.Abs((long)x.Votes)) > RecallThreshold;

	private bool CheckRecallStage()
	{
		if (IsFinalised) return true;
		RefreshRecallAuthorities();
		var target = RecallTarget;
		if (target is null || target.IsArchivedMembership || !target.Appointments.Contains(Appointment) ||
			Gameworld.ActiveJobs.Any(x => !x.IsJobComplete && x.Character.Id == target.MemberId &&
				x.Listing.ClanMembership == Appointment.Clan && x.Listing.ClanAppointment == Appointment))
		{
			CancelRecall();
			return true;
		}

		if (ElectionStage == ElectionStage.AwaitingConfirmation && RecallNeedsConfirmation ||
			Appointment.Clan.Calendar.CurrentDateTime < VotingEndDate) return false;

		if (RecallPassed && RecallNeedsConfirmation)
		{
			ElectionStage = ElectionStage.AwaitingConfirmation;
			_activeListener?.CancelListener();
			_activeListener = null;
			Changed = true;
			AnnounceRecall("passed and awaits confirmation; the holder remains in office");
			return false;
		}

		ElectionStage = ElectionStage.Finalised;
		IsFinalised = true;
		_activeListener?.CancelListener();
		_activeListener = null;
		Changed = true;
		if (RecallPassed)
		{
			// Do not let an already pending winning nomination immediately undo this removal.
			foreach (var election in Appointment.Elections.Where(x => !x.IsFinalised && !x.IsRecall).ToList())
			{
				election.WithdrawNomination(target);
			}
			Appointment.Clan.DismissAppointment(target, Appointment);
			Gameworld.Actors.Get(target.MemberId)?.OutputHandler.Send(
				$"You have been recalled from {Appointment.Name.ColourName()} in {Appointment.Clan.FullName.ColourName()}.");
		}
		AnnounceRecall(RecallPassed ? "passed; the holder has been removed" : "failed; the holder remains in office");
		return true;
	}

	private void CancelRecall()
	{
		ElectionStage = ElectionStage.Cancelled;
		IsFinalised = true;
		_activeListener?.CancelListener();
		_activeListener = null;
		Changed = true;
		AnnounceRecall("was cancelled because the holder no longer holds the office or holds it through employment");
	}

	private void AnnounceRecall(string result)
	{
		var echo = $"Recall #{Id} of {RecallTarget?.PersonalName.GetName(NameStyle.FullName).ColourName() ?? "the former holder"} as {Appointment.Name.ColourName()} in {Appointment.Clan.FullName.ColourName()} {result}.";
		Gameworld.SystemMessage(echo, ch => ClanCommandUtilities.CanConfirmElection(ch, this) ||
			ch.ClanMemberships.Any(x => !x.IsArchivedMembership && x.Clan == Appointment.Clan &&
				x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
		Gameworld.DiscordConnection?.NotifyAdmins(echo.RawText());
		if (Appointment.Clan.DiscordChannelId is { } channel)
			Gameworld.DiscordConnection?.NotifyCustomChannel(channel, "Appointment Recall", echo.RawText());
	}

	private string ShowRecall(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Recall #{Id.ToString("N0", actor)} - {RecallTarget?.PersonalName.GetName(NameStyle.FullName).ColourName()} as {Appointment.Name.ColourName()} in {Appointment.Clan.FullName.ColourName()}");
		sb.AppendLine($"Status: {ElectionStage.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Voting ends: {VotingEndDate.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine($"Removal requires more than {RecallThreshold.ToString("P2", actor).ColourValue()} of votes cast. Exactly reaching the threshold and no votes fail.");
		if (Appointment.ElectionConfirmationAppointment is { } authority)
			sb.AppendLine($"Confirmation authority: {authority.Name.ColourName()} in {authority.Clan.FullName.ColourName()}");
		foreach (var authorityEntry in RecallConfirmationAuthorities)
		{
			var key = FormattableString.Invariant($"{authorityEntry.ClanId}:{authorityEntry.AppointmentId ?? 0}");
			sb.AppendLine($"Liege confirmation: {Gameworld.Clans.Get(authorityEntry.ClanId)?.FullName.ColourName() ?? authorityEntry.ClanId.ToString(actor)} ({(_recallLiegeApprovals.GetValueOrDefault(key) ? "approved" : "pending")})");
		}
		if (ElectionStage == ElectionStage.AwaitingConfirmation)
			sb.AppendLine($"The holder remains in office pending {($"clan election confirm {Id}").ColourCommand()} or {($"clan election veto {Id}").ColourCommand()}.");
		else if (ElectionStage == ElectionStage.Vetoed)
			sb.AppendLine("The recall was vetoed; no removal occurred.");
		else if (ElectionStage == ElectionStage.Cancelled)
			sb.AppendLine("The recall was cancelled; no removal occurred through this ballot.");
		else if (IsFinalised)
			sb.AppendLine(RecallPassed ? "The recall passed and the holder was removed." : "The recall failed; the holder retained the office.");

		if (actor.IsAdministrator() || !Appointment.IsSecretBallot || ElectionStage != ElectionStage.Voting)
		{
			sb.AppendLine($"Remove: {_votes.Where(x => x.Votes > 0).Sum(x => (long)x.Votes).ToString("N0", actor).ColourValue()}; retain: {_votes.Where(x => x.Votes < 0).Sum(x => -(long)x.Votes).ToString("N0", actor).ColourValue()}");
		}
		if (actor.IsAdministrator() || !Appointment.IsSecretBallot)
		{
			foreach (var vote in _votes)
				sb.AppendLine($"  {vote.Voter.PersonalName.GetName(NameStyle.FullName).ColourName()}: {Math.Abs((long)vote.Votes).ToString("N0", actor)} to {(vote.Votes > 0 ? "remove" : "retain")}");
		}
		if (!IsFinalised && ElectionStage == ElectionStage.Voting)
			sb.AppendLine($"Use {($"clan vote {Id} yes|no").ColourCommand()} to vote for removal or retention.");
		return sb.ToString();
	}
}
