using MudSharp.Character;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Listeners;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Character.Name;

namespace MudSharp.Community;
#nullable enable
public class Election : SaveableItem, IElection
{
	public override string FrameworkItemType => "Election";

	public Election(IAppointment appointment, bool isByElection, int numberOfAppointments,
		MudDateTime? endOfCurrentTerm)
	{
		Gameworld = appointment.Gameworld;
		Appointment = appointment;
		IsByElection = isByElection;
		IsFinalised = false;
		NumberOfAppointments = numberOfAppointments;
		ElectionStage = ElectionStage.Preelection;
		if (endOfCurrentTerm is null)
		{
			var now = appointment.Clan.Calendar.CurrentDateTime;
			NominationStartDate = now;
			VotingStartDate = now + appointment.NominationPeriod;
			VotingEndDate = VotingStartDate + appointment.VotingPeriod;
			ResultsInEffectDate = IsByElection ? VotingEndDate : VotingEndDate + appointment.ElectionLeadTime;
		}
		else
		{
			ResultsInEffectDate = endOfCurrentTerm;
			VotingEndDate = endOfCurrentTerm - appointment.ElectionLeadTime;
			VotingStartDate = VotingEndDate - appointment.VotingPeriod;
			NominationStartDate = VotingStartDate - appointment.NominationPeriod;
		}

		using (new FMDB())
		{
			var dbitem = new Models.Election
			{
				AppointmentId = appointment.Id,
				IsByElection = IsByElection,
				IsFinalised = IsFinalised,
				NumberOfAppointments = NumberOfAppointments,
				NominationStartDate = NominationStartDate.GetDateTimeString(),
				VotingStartDate = VotingStartDate.GetDateTimeString(),
				VotingEndDate = VotingEndDate.GetDateTimeString(),
				ResultsInEffectDate = ResultsInEffectDate.GetDateTimeString(),
				ElectionStage = (int)ElectionStage
			};
			FMDB.Context.Elections.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}

		Gameworld.Add(this);
		SetupListeners(ElectionStage.Preelection);
	}

	public Election(Models.Election election, IAppointment appointment)
	{
		Gameworld = appointment.Gameworld;
		Appointment = appointment;
		IsByElection = election.IsByElection;
		IsFinalised = election.IsFinalised;
		NumberOfAppointments = election.NumberOfAppointments;
		NominationStartDate = new MudDateTime(election.NominationStartDate, Gameworld);
		VotingStartDate = new MudDateTime(election.VotingStartDate, Gameworld);
		VotingEndDate = new MudDateTime(election.VotingEndDate, Gameworld);
		ResultsInEffectDate = new MudDateTime(election.ResultsInEffectDate, Gameworld);
		ElectionStage = (ElectionStage)election.ElectionStage;
		_id = election.Id;
		foreach (var nominee in election.ElectionNominees)
		{
			_nominees.Add(Appointment.Clan.Memberships.First(x => x.MemberId == nominee.NomineeId));
		}

		foreach (var vote in election.ElectionVotes)
		{
			_votes.Add((Appointment.Clan.Memberships.First(x => x.MemberId == vote.VoterId),
				Appointment.Clan.Memberships.First(x => x.MemberId == vote.NomineeId), vote.NumberOfVotes));
		}

		Gameworld.Add(this);
		SetupListeners(ElectionStage);
	}

	public override void Save()
	{
		var dbitem = FMDB.Context.Elections.Find(Id);
		dbitem.IsFinalised = IsFinalised;
		dbitem.ElectionStage = (int)ElectionStage;
		FMDB.Context.ElectionNominees.RemoveRange(dbitem.ElectionNominees);
		foreach (var nominee in _nominees)
		{
			dbitem.ElectionNominees.Add(new ElectionNominee
				{ Election = dbitem, NomineeId = nominee.MemberId, NomineeClanId = Appointment.Clan.Id });
		}

		FMDB.Context.ElectionVotes.RemoveRange(dbitem.ElectionVotes);
		foreach (var vote in _votes)
		{
			dbitem.ElectionVotes.Add(new ElectionVote
			{
				Election = dbitem, NomineeId = vote.Nominee.MemberId, NomineeClanId = Appointment.Clan.Id,
				VoterId = vote.Voter.MemberId, VoterClanId = Appointment.Clan.Id, NumberOfVotes = vote.Votes
			});
		}

		Changed = false;
	}

	public IAppointment Appointment { get; protected set; }

	public bool IsByElection { get; protected set; }
	public bool IsFinalised { get; protected set; }
	public int NumberOfAppointments { get; protected set; }
	public MudDateTime NominationStartDate { get; protected set; }
	public MudDateTime VotingStartDate { get; protected set; }
	public MudDateTime VotingEndDate { get; protected set; }
	public MudDateTime ResultsInEffectDate { get; protected set; }
	public ElectionStage ElectionStage { get; protected set; }

	private readonly List<IClanMembership> _nominees = new();
	public IEnumerable<IClanMembership> Nominees => _nominees;

	private readonly List<(IClanMembership Voter, IClanMembership Nominee, int Votes)> _votes = new();
	public IEnumerable<(IClanMembership Voter, IClanMembership Nominee, int Votes)> Votes => _votes;

	public bool CheckElectionStage()
	{
		var now = Appointment.Clan.Calendar.CurrentDateTime;
		if (now >= VotingEndDate && ElectionStage < ElectionStage.Preinstallation)
		{
			ElectionStage = ElectionStage.Preinstallation;
			var votes = VotesByNominee;
			var victors = Victors.ToList();
			string victorsText;
			if (victors.Any())
			{
				if (victors.Count < NumberOfAppointments)
				{
					victorsText =
						$"{victors.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToString()} {(victors.Count == 1 ? "was the successful nominee" : "were the successful nominees")}. There are still {(NumberOfAppointments - victors.Count).ToString("N0").ColourValue()} positions remaining, and so a by-election will be held.";
				}
				else
				{
					victorsText =
						$"{victors.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToString()} {(victors.Count == 1 ? "was the successful nominee" : "were the successful nominees")}.";
				}
			}
			else
			{
				victorsText = "there were no successful nominees. A by-election will be held.";
			}

			var echo =
				$"The election for the position of {Appointment.Name.ColourValue()} in {Appointment.Clan.FullName.ColourName()} has concluded, and {victorsText}";
			Gameworld.SystemMessage(echo,
				ch => ch.ClanMemberships.Any(x =>
					x.Clan == Appointment.Clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
			Gameworld.DiscordConnection.NotifyAdmins(echo.RawText());
			if (Appointment.Clan.DiscordChannelId is not null)
			{
				Gameworld.DiscordConnection?.NotifyCustomChannel(Appointment.Clan.DiscordChannelId.Value,
					$"Voting Result Notification", echo.RawText());
			}

			Changed = true;
			if (now < ResultsInEffectDate && Appointment.ElectionLeadTime > TimeSpan.Zero)
			{
				SetupListeners(ElectionStage);
				return false;
			}
		}

		if (now >= ResultsInEffectDate)
		{
			ElectionStage = ElectionStage.Finalised;

			var votes = VotesByNominee;
			var victors = Victors.ToList();
			foreach (var member in Appointment.Clan.Memberships.Where(x => x.Appointments.Contains(Appointment))
			                                  .ToList())
			{
				if (victors.Contains(member))
				{
					continue;
				}

				var ch = Gameworld.Actors.Get(member.Id);
				ch?.OutputHandler.Send(
					$"Your term as {Appointment.Title(ch).ColourValue()} in {Appointment.Clan.FullName.ColourName()} has ended.");
				member.Appointments.Remove(Appointment);
				member.Changed = true;
			}

			foreach (var victor in victors)
			{
				if (victor.Appointments.Contains(Appointment))
				{
					continue;
				}

				victor.Appointments.Add(Appointment);
				victor.Changed = true;
				var ch = Gameworld.Actors.Get(victor.Id);
				ch?.OutputHandler.Send(
					$"Your term as {Appointment.Title(ch).ColourValue()} in {Appointment.Clan.FullName.ColourName()} has begun.");
			}

			var appointees = Appointment.Clan.Memberships.Count(x => x.Appointments.Contains(Appointment));

			// Do we need a run-off election?
			if (appointees < NumberOfAppointments)
			{
				var officeHolders = Appointment.Clan.Memberships.Count(x => x.Appointments.Contains(Appointment));
				var runoff = new Election(Appointment, true, Appointment.MaximumSimultaneousHolders - officeHolders,
					Appointment.Clan.Calendar.CurrentDateTime + Appointment.NominationPeriod +
					Appointment.VotingPeriod + Appointment.ElectionLeadTime);
				Appointment.AddElection(runoff);
			}

			if (!IsByElection)
			{
				var next = new Election(Appointment, false, Appointment.MaximumSimultaneousHolders,
					ResultsInEffectDate + Appointment.ElectionTerm);
				Appointment.AddElection(next);
			}

			_activeListener = null;
			IsFinalised = true;
			Changed = true;
			return true;
		}

		if (now >= VotingStartDate)
		{
			if ((!IsByElection && Nominees.Count() > NumberOfAppointments) || (IsByElection &&
			                                                                   Nominees.Count() >
			                                                                   NumberOfAppointments -
			                                                                   Appointment.Clan.Memberships.Count(x =>
				                                                                   x.Appointments
					                                                                   .Contains(Appointment))))
			{
				var echo =
					$"Voting has begun in the election for the position of {Appointment.Name.ColourValue()} in {Appointment.Clan.FullName.ColourName()}. The nominees are {Nominees.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToString()}.\nThe voting period will last for {Appointment.VotingPeriod.Describe().ColourValue()}.";
				Gameworld.SystemMessage(echo,
					ch => ch.ClanMemberships.Any(x =>
						x.Clan == Appointment.Clan &&
						x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
				Gameworld.DiscordConnection?.NotifyAdmins(echo.RawText());
				if (Appointment.Clan.DiscordChannelId is not null)
				{
					Gameworld.DiscordConnection?.NotifyCustomChannel(Appointment.Clan.DiscordChannelId.Value,
						$"Voting Start Notification", echo.RawText());
				}
			}
			else if (Nominees.Any())
			{
				var echo =
					$"Voting has begun in the election for the position of {Appointment.Name.ColourValue()} in {Appointment.Clan.FullName.ColourName()}. The nominees are {Nominees.Select(x => x.PersonalName.GetName(NameStyle.FullName).ColourName()).ListToString()}.\nThe voting period will last for {Appointment.VotingPeriod.Describe().ColourValue()}.\n{"As there are more positions up for election than there are nominees, any nominee who receives at least one vote will be automatically elected.".Colour(Telnet.Red)}";
				Gameworld.SystemMessage(echo,
					ch => ch.ClanMemberships.Any(x =>
						x.Clan == Appointment.Clan &&
						x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
				Gameworld.DiscordConnection?.NotifyAdmins(echo.RawText());
				if (Appointment.Clan.DiscordChannelId is not null)
				{
					Gameworld.DiscordConnection?.NotifyCustomChannel(Appointment.Clan.DiscordChannelId.Value,
						$"Voting Start Notification", echo.RawText());
				}
			}
			else
			{
				var echo =
					$"There were no nominees in the election for the position of {Appointment.Name.ColourValue()} in {Appointment.Clan.FullName.ColourName()}. A by-election will be automatically triggered.";
				Gameworld.SystemMessage(echo,
					ch => ch.ClanMemberships.Any(x =>
						x.Clan == Appointment.Clan &&
						x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
				Gameworld.DiscordConnection?.NotifyAdmins(echo.RawText());
				if (Appointment.Clan.DiscordChannelId is not null)
				{
					Gameworld.DiscordConnection?.NotifyCustomChannel(Appointment.Clan.DiscordChannelId.Value,
						$"Runoff Election Notification", echo.RawText());
				}

				var runoff = new Election(Appointment, true, NumberOfAppointments,
					Appointment.Clan.Calendar.CurrentDateTime + Appointment.NominationPeriod +
					Appointment.VotingPeriod + Appointment.ElectionLeadTime);
				Appointment.AddElection(runoff);
				ElectionStage = ElectionStage.Finalised;
				Changed = true;
				_activeListener = null;
				IsFinalised = true;
				return true;
			}

			ElectionStage = ElectionStage.Voting;
			Changed = true;
			SetupListeners(ElectionStage);
			return false;
		}

		if (now >= NominationStartDate)
		{
			var echo =
				$"An election has begun for the position of {Appointment.Name.ColourValue()} in {Appointment.Clan.FullName.ColourName()}.\nThe nomination period will last for {Appointment.VotingPeriod.Describe().ColourValue()}, after which voting will commence.";
			Gameworld.SystemMessage(echo,
				ch => ch.ClanMemberships.Any(x =>
					x.Clan == Appointment.Clan && x.NetPrivileges.HasFlag(ClanPrivilegeType.CanViewClanOfficeHolders)));
			Gameworld.DiscordConnection?.NotifyAdmins(echo.RawText());
			if (Appointment.Clan.DiscordChannelId is not null)
			{
				Gameworld.DiscordConnection?.NotifyCustomChannel(Appointment.Clan.DiscordChannelId.Value,
					$"Election Start Notification", echo.RawText());
			}

			ElectionStage = ElectionStage.Nomination;
			Changed = true;
			SetupListeners(ElectionStage);
			return false;
		}

		return false;
	}

	public void Nominate(IClanMembership nominee)
	{
		if (!_nominees.Contains(nominee))
		{
			_nominees.Add(nominee);
			Changed = true;
		}
	}

	public void WithdrawNomination(IClanMembership nominee)
	{
		_nominees.Remove(nominee);
		_votes.RemoveAll(x => x.Nominee == nominee);
		Changed = true;
	}

	public void Vote(IClanMembership voter, IClanMembership nominee, int votes)
	{
		_votes.RemoveAll(x => x.Voter == voter);
		_votes.Add((voter, nominee, votes));
		Changed = true;
	}

	public void CancelElection()
	{
		_activeListener?.CancelListener();
		_activeListener = null;
		Gameworld.SaveManager.Abort(this);
		Gameworld.Destroy(this);
		Appointment.RemoveElection(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbitem = FMDB.Context.Elections.Find(Id);
			if (dbitem != null)
			{
				FMDB.Context.Elections.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	private ITemporalListener? _activeListener;

	public void SetupListeners(ElectionStage stage)
	{
		switch (stage)
		{
			case ElectionStage.Preelection:
				_activeListener = ListenerFactory.CreateDateTimeListener(NominationStartDate,
					objects => { CheckElectionStage(); }, new object[] { });
				break;
			case ElectionStage.Nomination:
				_activeListener = ListenerFactory.CreateDateTimeListener(VotingStartDate,
					objects => { CheckElectionStage(); }, new object[] { });
				break;
			case ElectionStage.Voting:
				_activeListener = ListenerFactory.CreateDateTimeListener(VotingEndDate,
					objects => { CheckElectionStage(); }, new object[] { });
				break;
			case ElectionStage.Preinstallation:
				_activeListener = ListenerFactory.CreateDateTimeListener(ResultsInEffectDate,
					objects => { CheckElectionStage(); }, new object[] { });
				break;
			case ElectionStage.Finalised:
				break;
		}
	}

	public string Show(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine(
			$"Election #{Id.ToString("N0", actor)} - {(IsByElection ? "By-Election" : "Election")} for {Appointment.Name.ColourName()} in {Appointment.Clan.FullName.ColourName()}");
		sb.AppendLine($"Status: {ElectionStage.DescribeEnum().ColourValue()}");
		sb.AppendLine($"No. Positions: {NumberOfAppointments.ToString("N0", actor).ColourValue()}");
		sb.AppendLine(
			$"Nominations Open: {NominationStartDate.ToString(CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine(
			$"Voting Begins: {VotingStartDate.ToString(CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine(
			$"Voting Ends: {VotingEndDate.ToString(CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine(
			$"Appointments Happen: {ResultsInEffectDate.ToString(CalendarDisplayMode.Short, TimeAndDate.Time.TimeDisplayTypes.Short).ColourValue()}");
		switch (ElectionStage)
		{
			case ElectionStage.Preelection:
				sb.AppendLine();
				sb.AppendLine("The election has not yet begun.");
				break;
			case ElectionStage.Nomination:
				if (!_nominees.Any())
				{
					sb.AppendLine($"There are not yet any nominations.");
				}
				else
				{
					sb.AppendLine("Nominees:");
					foreach (var nominee in _nominees)
					{
						var desc = actor.IsAdministrator()
							? (Gameworld.Actors.FirstOrDefault(x => x.Id == nominee.MemberId) ??
							   Gameworld.CachedActors.FirstOrDefault(x => x.Id == nominee.MemberId))?.HowSeen(actor)
							: null ??
							  actor.Dubs
							       .FirstOrDefault(x =>
								       x.TargetType == "Character" && x.TargetId == nominee.MemberId &&
								       !x.WasIdentityConcealed)?.LastDescription.ColourCharacter();
						if (!string.IsNullOrEmpty(desc))
						{
							sb.AppendLine(
								$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} ({desc}){(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
						}
						else
						{
							sb.AppendLine(
								$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
						}
					}
				}

				break;
			case ElectionStage.Voting:
				if (!_nominees.Any())
				{
					sb.AppendLine($"There were no valid nominations.");
				}
				else
				{
					if (actor.IsAdministrator() || !Appointment.IsSecretBallot)
					{
						sb.AppendLine("Nominees:");
						var votes = VotesByNominee;
						foreach (var nominee in _nominees.OrderByDescending(x => votes[x]))
						{
							var desc = actor.IsAdministrator()
								? (Gameworld.Actors.FirstOrDefault(x => x.Id == nominee.MemberId) ??
								   Gameworld.CachedActors.FirstOrDefault(x => x.Id == nominee.MemberId))?.HowSeen(actor)
								: null ??
								  actor.Dubs
								       .FirstOrDefault(x =>
									       x.TargetType == "Character" && x.TargetId == nominee.MemberId &&
									       !x.WasIdentityConcealed)?.LastDescription.ColourCharacter();
							if (!string.IsNullOrEmpty(desc))
							{
								sb.AppendLine(
									$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} ({desc}) - {(votes[nominee] == 1 ? "1 vote" : $"{votes[nominee].ToString("N0", actor)} votes")}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
							}
							else
							{
								sb.AppendLine(
									$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} - {(votes[nominee] == 1 ? "1 vote" : $"{votes[nominee].ToString("N0", actor)} votes")}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
							}
						}
					}
					else
					{
						sb.AppendLine("Nominees:");
						foreach (var nominee in _nominees)
						{
							var desc = actor.IsAdministrator()
								? (Gameworld.Actors.FirstOrDefault(x => x.Id == nominee.MemberId) ??
								   Gameworld.CachedActors.FirstOrDefault(x => x.Id == nominee.MemberId))?.HowSeen(actor)
								: null ??
								  actor.Dubs
								       .FirstOrDefault(x =>
									       x.TargetType == "Character" && x.TargetId == nominee.Id &&
									       !x.WasIdentityConcealed)?.LastDescription.ColourCharacter();
							if (!string.IsNullOrEmpty(desc))
							{
								sb.AppendLine(
									$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} ({desc}){(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
							}
							else
							{
								sb.AppendLine(
									$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}");
							}
						}
					}
				}

				break;
			case ElectionStage.Preinstallation:
			case ElectionStage.Finalised:
				if (!_nominees.Any())
				{
					sb.AppendLine($"There were no valid nominations.");
				}
				else
				{
					sb.AppendLine("Nominees:");
					var votes = VotesByNominee;
					var victors = Victors;
					foreach (var nominee in _nominees.OrderByDescending(x => votes[x]))
					{
						var desc = actor.IsAdministrator()
							? (Gameworld.Actors.FirstOrDefault(x => x.Id == nominee.MemberId) ??
							   Gameworld.CachedActors.FirstOrDefault(x => x.Id == nominee.MemberId))?.HowSeen(actor)
							: null ??
							  actor.Dubs
							       .FirstOrDefault(x =>
								       x.TargetType == "Character" && x.TargetId == nominee.MemberId &&
								       !x.WasIdentityConcealed)?.LastDescription.ColourCharacter();
						if (!string.IsNullOrEmpty(desc))
						{
							sb.AppendLine(
								$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} ({desc}) - {(votes[nominee] == 1 ? "1 vote" : $"{votes[nominee].ToString("N0", actor)} votes")}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}{(victors.Contains(nominee) ? " [Elected]".Colour(Telnet.Green) : "")}");
						}
						else
						{
							sb.AppendLine(
								$"\t{nominee.PersonalName.GetName(NameStyle.FullName).ColourName()} - {(votes[nominee] == 1 ? "1 vote" : $"{votes[nominee].ToString("N0", actor)} votes")}{(actor.IsAdministrator() ? $" - Char ID {nominee.MemberId.ToString("N0", actor)}" : "")}{(victors.Contains(nominee) ? " [Elected]".Colour(Telnet.Green) : "")}");
						}
					}

					if (actor.IsAdministrator() || !Appointment.IsSecretBallot)
					{
						sb.AppendLine();
						sb.AppendLine("Votes:");
						foreach (var vote in _votes.OrderBy(x => x.Voter.PersonalName.GetName(NameStyle.FullName)))
						{
							sb.AppendLine(
								$"\t{vote.Voter.PersonalName.GetName(NameStyle.FullName).ColourName()} {vote.Votes.ToString("N0", actor)} {(vote.Votes == 1 ? "vote" : "votes")} for {vote.Nominee.PersonalName.GetName(NameStyle.FullName).ColourName()}");
						}
					}
				}

				break;
		}

		return sb.ToString();
	}

	public Counter<IClanMembership> VotesByNominee
	{
		get
		{
			var counter = new Counter<IClanMembership>();
			foreach (var vote in _votes)
			{
				counter[vote.Nominee] += vote.Votes;
			}

			return counter;
		}
	}

	public IEnumerable<IClanMembership> Victors
	{
		get
		{
			if (ElectionStage < ElectionStage.Preinstallation)
			{
				return Enumerable.Empty<IClanMembership>();
			}

			return VotesByNominee
			       .OrderByDescending(x => x.Value)
			       .ThenBy(x => x.Key.JoinDate)
			       .Select(x => x.Key)
			       .Take(NumberOfAppointments)
			       .ToList();
		}
	}
}
#nullable restore