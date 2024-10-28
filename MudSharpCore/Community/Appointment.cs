using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;
using MudSharp.Commands.Trees;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;

namespace MudSharp.Community;

public class Appointment : SaveableItem, IAppointment
{
	protected Appointment()
	{
	}

	public Appointment(MudSharp.Models.Appointment appointment, IClan clan)
	{
		Gameworld = clan.Gameworld;
		Clan = clan;
		_id = appointment.Id;
		_name = appointment.Name;
		Paygrade = appointment.PaygradeId.HasValue
			? clan.Paygrades.FirstOrDefault(x => x.Id == appointment.PaygradeId.Value)
			: null;
		Privileges = (ClanPrivilegeType)appointment.Privileges;
		FameType = (ClanFameType)appointment.FameType;
		if (appointment.InsigniaGameItemId.HasValue && appointment.InsigniaGameItemRevnum.HasValue)
		{
			InsigniaGameItem = Gameworld.ItemProtos.Get(appointment.InsigniaGameItemId.Value,
				appointment.InsigniaGameItemRevnum.Value);
		}

		MaximumSimultaneousHolders = appointment.MaximumSimultaneousHolders;
		MinimumRankToHold = appointment.MinimumRankId.HasValue
			? clan.Ranks.FirstOrDefault(x => x.Id == appointment.MinimumRankId.Value)
			: null;
		MinimumRankToAppoint = appointment.MinimumRankToAppointId.HasValue
			? clan.Ranks.FirstOrDefault(x => x.Id == appointment.MinimumRankToAppointId.Value)
			: null;

		foreach (var item in appointment.AppointmentsTitles.OrderBy(x => x.Order))
		{
			TitlesAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0), item.Title));
		}

		foreach (var item in appointment.AppointmentsAbbreviations.OrderBy(x => x.Order))
		{
			AbbreviationsAndProgs.Add(Tuple.Create(Gameworld.FutureProgs.Get(item.FutureProgId ?? 0),
				item.Abbreviation));
		}

		IsAppointedByElection = appointment.IsAppointedByElection;
		ElectionTerm = TimeSpan.FromMinutes(appointment.ElectionTermMinutes ?? 0);
		ElectionLeadTime = TimeSpan.FromMinutes(appointment.ElectionLeadTimeMinutes ?? 0);
		NominationPeriod = TimeSpan.FromMinutes(appointment.NominationPeriodMinutes ?? 0);
		VotingPeriod = TimeSpan.FromMinutes(appointment.VotingPeriodMinutes ?? 0);
		MaximumConsecutiveTerms = appointment.MaximumConsecutiveTerms ?? 0;
		MaximumTotalTerms = appointment.MaximumTotalTerms ?? 0;
		IsSecretBallot = appointment.IsSecretBallot ?? false;
		CanNominateProg = Gameworld.FutureProgs.Get(appointment.CanNominateProgId ?? 0L);
		NumberOfVotesProg = Gameworld.FutureProgs.Get(appointment.NumberOfVotesProgId ?? 0L);
		WhyCantNominateProg = Gameworld.FutureProgs.Get(appointment.WhyCantNominateProgId ?? 0L);
	}

	public void LoadElections(IEnumerable<Models.Election> elections)
	{
		foreach (var election in elections)
		{
			_elections.Add(new Election(election, this));
		}

		if (IsAppointedByElection)
		{
			if (_elections.All(x => x.IsFinalised || x.IsByElection))
			{
				var byElection = _elections.FirstOrDefault(x => x.IsByElection && !x.IsFinalised);
				if (MaximumSimultaneousHolders >
				    Clan.Memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(this)))
				{
					_elections.Add(new Election(this, false, MaximumSimultaneousHolders,
						byElection != null ? byElection.ResultsInEffectDate + ElectionTerm : null));
				}
				else
				{
					_elections.Add(new Election(this, false, MaximumSimultaneousHolders,
						byElection != null
							? byElection.ResultsInEffectDate + ElectionTerm
							: Clan.Calendar.CurrentDateTime + ElectionTerm));
				}
			}
		}
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Appointments.Find(Id);
			dbitem.Name = Name;
			dbitem.Privileges = (long)Privileges;
			dbitem.MaximumSimultaneousHolders = MaximumSimultaneousHolders;
			dbitem.PaygradeId = Paygrade?.Id;
			dbitem.MinimumRankId = MinimumRankToHold?.Id;
			dbitem.FameType = (int)FameType;

			if (InsigniaGameItem != null)
			{
				dbitem.InsigniaGameItemId = InsigniaGameItem.Id;
				dbitem.InsigniaGameItemRevnum = InsigniaGameItem.RevisionNumber;
			}
			else
			{
				dbitem.InsigniaGameItemId = null;
				dbitem.InsigniaGameItemRevnum = null;
			}

			if (IsAppointedByElection)
			{
				dbitem.IsAppointedByElection = true;
				dbitem.IsSecretBallot = IsSecretBallot;
				dbitem.ElectionTermMinutes = ElectionTerm.TotalMinutes;
				dbitem.ElectionLeadTimeMinutes = ElectionLeadTime.TotalMinutes;
				dbitem.NominationPeriodMinutes = NominationPeriod.TotalMinutes;
				dbitem.VotingPeriodMinutes = VotingPeriod.TotalMinutes;
				dbitem.MaximumConsecutiveTerms = MaximumConsecutiveTerms;
				dbitem.MaximumTotalTerms = MaximumTotalTerms;
				dbitem.CanNominateProgId = CanNominateProg?.Id;
				dbitem.NumberOfVotesProgId = NumberOfVotesProg?.Id;
				dbitem.WhyCantNominateProgId = WhyCantNominateProg?.Id;
			}
			else
			{
				dbitem.IsAppointedByElection = false;
				dbitem.IsSecretBallot = null;
				dbitem.ElectionTermMinutes = null;
				dbitem.ElectionLeadTimeMinutes = null;
				dbitem.NominationPeriodMinutes = null;
				dbitem.VotingPeriodMinutes = null;
				dbitem.MaximumConsecutiveTerms = null;
				dbitem.MaximumTotalTerms = null;
				dbitem.CanNominateProgId = null;
				dbitem.NumberOfVotesProgId = null;
				dbitem.WhyCantNominateProgId = null;
			}

			FMDB.Context.AppointmentsAbbreviations.RemoveRange(dbitem.AppointmentsAbbreviations);
			var order = 0;
			foreach (var item in AbbreviationsAndProgs)
			{
				var abbreviation = new AppointmentsAbbreviations
				{
					FutureProgId = item.Item1?.Id,
					Abbreviation = item.Item2,
					Order = order++
				};
				dbitem.AppointmentsAbbreviations.Add(abbreviation);
			}

			FMDB.Context.AppointmentsTitles.RemoveRange(dbitem.AppointmentsTitles);
			order = 0;
			foreach (var item in TitlesAndProgs)
			{
				var title = new AppointmentsTitles
				{
					FutureProgId = item.Item1?.Id,
					Title = item.Item2,
					Order = order++
				};
				dbitem.AppointmentsTitles.Add(title);
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Appointment";

	#region IAppointment Members

	private readonly List<IElection> _elections = new();
	public IEnumerable<IElection> Elections => _elections;

	public void AddElection(IElection election)
	{
		_elections.Add(election);
	}

	public void RemoveElection(IElection election)
	{
		_elections.Remove(election);
	}

	public void CheckForByElections()
	{
		if (!IsAppointedByElection)
		{
			return;
		}

		if (Elections.Any(x =>
			    !x.IsFinalised && x.ElectionStage != ElectionStage.Preelection && !x.IsByElection))
		{
			return;
		}

		var count = Clan.Memberships.Count(x => !x.IsArchivedMembership && x.Appointments.Contains(this));
		if (count < MaximumSimultaneousHolders)
		{
			var election = new Election(this, true, MaximumSimultaneousHolders - count, null);
			_elections.Add(election);
		}
	}

	public void SetName(string name)
	{
		_name = name;
	}

	public string Abbreviation(ICharacter character)
	{
		return
			AbbreviationsAndProgs.First(x => (bool?)x.Item1?.Execute(character) ?? true)
			                     .Item2;
	}

	public bool IsAppointedByElection { get; set; }
	public TimeSpan ElectionTerm { get; set; }
	public TimeSpan ElectionLeadTime { get; set; }
	public TimeSpan NominationPeriod { get; set; }
	public TimeSpan VotingPeriod { get; set; }
	public int MaximumConsecutiveTerms { get; set; }
	public int MaximumTotalTerms { get; set; }
	public bool IsSecretBallot { get; set; }

	public IFutureProg CanNominateProg { get; set; }
	public IFutureProg WhyCantNominateProg { get; set; }

	public (bool Truth, string Error) CanNominate(ICharacter character)
	{
		if (CanNominateProg?.Execute<bool?>(character) != true)
		{
			return (false,
				WhyCantNominateProg?.Execute(character)?.ToString() ??
				"You do not meet the eligibility requirements for that nomination.");
		}

		if (MaximumConsecutiveTerms > 0)
		{
			var pastElections = Elections
			                    .Where(x => x.IsFinalised && !x.IsByElection)
			                    .OrderByDescending(x => x.ResultsInEffectDate)
			                    .Take(MaximumConsecutiveTerms)
			                    .ToList();
			if (pastElections.Count >= MaximumConsecutiveTerms &&
			    pastElections.All(x => x.Victors.Any(y => y.MemberId == character.Id)))
			{
				return (false,
					$"You have reached the limit of {MaximumConsecutiveTerms.ToString("N0", character).ColourValue()} consecutive terms as {Title(character).ColourValue()}.");
			}
		}

		if (MaximumTotalTerms > 0)
		{
			if (Elections
			    .Where(x => x.IsFinalised && !x.IsByElection)
			    .Count(x => x.Victors.Any(y => y.MemberId == character.Id)) > MaximumTotalTerms)
			{
				return (false,
					$"You have reached the life-time limit of {MaximumTotalTerms.ToString("N0", character).ColourValue()} total terms as {Title(character).ColourValue()}.");
			}
		}

		return (true, string.Empty);
	}

	public IFutureProg NumberOfVotesProg { get; set; }

	public int NumberOfVotes(ICharacter character)
	{
		return NumberOfVotesProg?.ExecuteInt(character) ?? 0;
	}

	IEnumerable<string> IAppointment.Abbreviations
	{
		get { return AbbreviationsAndProgs.Select(x => x.Item2); }
	}

	public string Title(ICharacter character)
	{
		return TitlesAndProgs.First(x => (bool?)x.Item1?.Execute(character) ?? true).Item2;
	}

	IEnumerable<string> IAppointment.Titles
	{
		get { return TitlesAndProgs.Select(x => x.Item2); }
	}

	public List<Tuple<IFutureProg, string>> AbbreviationsAndProgs { get; } = new();

	public List<Tuple<IFutureProg, string>> TitlesAndProgs { get; } = new();

	public void FinaliseLoad(MudSharp.Models.Appointment appointment)
	{
		if (appointment.ParentAppointmentId.HasValue)
		{
			ParentPosition = Clan.Appointments.FirstOrDefault(x => x.Id == appointment.ParentAppointmentId);
		}
	}

	public IClan Clan { get; set; }

	public IPaygrade Paygrade { get; set; }

	public IRank MinimumRankToHold { get; set; }

	public IRank MinimumRankToAppoint { get; set; }

	public int MaximumSimultaneousHolders { get; set; }

	public IAppointment ParentPosition { get; set; }

	public ClanPrivilegeType Privileges { get; set; }

	public IGameItemProto InsigniaGameItem { get; set; }

	public ClanFameType FameType { get; set; }

	#endregion

	#region IFutureProgVariable Members

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(Name);
			case "paygrade":
				return Paygrade;
			case "minrank":
				return MinimumRankToHold;
			case "minappointer":
				return MinimumRankToAppoint;
			case "parent":
				return ParentPosition;
			case "holders":
				return new NumberVariable(Clan.Memberships.Count(x => x.Appointments.Contains(this)));
			case "maxholders":
				return new NumberVariable(MaximumSimultaneousHolders);
			case "onlinemembers":
				return
					new CollectionVariable(
						Gameworld.Characters.Where(
							         x => x.ClanMemberships.Any(y => y.Clan == Clan && y.Appointments.Contains(this)))
						         .ToList(), ProgVariableTypes.Character);
			default:
				throw new NotSupportedException("Invalid IFutureProgVariable request in Rank.GetProperty");
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.ClanAppointment;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "paygrade", ProgVariableTypes.ClanPaygrade },
			{ "minrank", ProgVariableTypes.ClanRank },
			{ "minappointer", ProgVariableTypes.ClanRank },
			{ "parent", ProgVariableTypes.ClanAppointment },
			{ "holders", ProgVariableTypes.Number },
			{ "maxholders", ProgVariableTypes.Number },
			{ "onlinemembers", ProgVariableTypes.Collection | ProgVariableTypes.Character }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the appointment" },
			{ "name", "The name of the appointment" },
			{ "paygrade", "The paygrade associated with the appointment (can be null)" },
			{ "minrank", "The minimum rank required to hold the appointment (can be null)" },
			{ "minappointer", "The minimum rank required to appoint to this appointment (can be null)" },
			{ "parent", "The parent rank controlling appointments to this appointment (can be null)" },
			{ "holders", "A list of all the current holders of this appointment" },
			{ "maxholders", "The maximum number of simultaneous holders (0 if unlimited)" },
			{ "onlinemembers", "A list of all online characters with this appointment" }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.ClanAppointment,
			DotReferenceHandler(), DotReferenceHelp());
	}

	#endregion
}