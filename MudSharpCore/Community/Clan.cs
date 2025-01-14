using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Intervals;
using MudSharp.TimeAndDate.Listeners;

namespace MudSharp.Community;

public class Clan : SaveableItem, IClan
{
	private static IFutureProg CanCreateClanProg;
	private static IFutureProg OnCreateClanProg;

	internal Clan(MudSharp.Models.Clan clan, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = clan.Id;
		_name = clan.Name;
		Alias = clan.Alias;
		FullName = clan.FullName;
		Description = clan.Description;
		Calendar = Gameworld.Calendars.Get(clan.CalendarId);
		ShowClanMembersInWho = clan.ShowClanMembersInWho;
		ShowFamousMembersInNotables = clan.ShowFamousMembersInNotables;
		Sphere = clan.Sphere;
		IsTemplate = clan.IsTemplate;
		_paymasterId = clan.PaymasterId;
		_paymasterItemProtoId = clan.PaymasterItemProtoId;
		MaximumPeriodsOfUncollectedBackPay = clan.MaximumPeriodsOfUncollectedBackPay;
		_clanBankAccountId = clan.BankAccountId;
		OnPayProg = Gameworld.FutureProgs.Get(clan.OnPayProgId ?? 0);
		_payInterval = new RecurringInterval
		{
			Type = (IntervalType)clan.PayIntervalType,
			IntervalAmount = clan.PayIntervalModifier,
			Modifier = clan.PayIntervalOther
		};
		var payTime = Calendar.FeedClock.GetTime(clan.PayIntervalReferenceTime);
		_nextPay = new MudDateTime(
			PayInterval.GetNextDate(Calendar, Calendar.GetDate(clan.PayIntervalReferenceDate)), payTime,
			payTime.Timezone);
		foreach (var cell in clan.ClansTreasuryCells)
		{
			var gameCell = gameworld.Cells.Get(cell.CellId);
			gameCell.CellRequestsDeletion -= TreasuryCellRequestsDeletion;
			gameCell.CellRequestsDeletion += TreasuryCellRequestsDeletion;
			_treasuryCells.Add(gameCell);
		}

		foreach (var cell in clan.ClansAdministrationCells)
		{
			var gameCell = gameworld.Cells.Get(cell.CellId);
			gameCell.CellRequestsDeletion -= AdminCellRequestsDeletion;
			gameCell.CellRequestsDeletion += AdminCellRequestsDeletion;
			_administrationCells.Add(gameCell);
		}

		DiscordChannelId = clan.DiscordChannelId;
	}

	private void AdminCellRequestsDeletion(object sender, EventArgs e)
	{
		_administrationCells.Remove((ICell)sender);
		Changed = true;
	}

	private void TreasuryCellRequestsDeletion(object sender, EventArgs e)
	{
		_treasuryCells.Remove((ICell)sender);
		Changed = true;
	}

	public override void Save()
	{
		using (new FMDB())
		{
			var clan = FMDB.Context.Clans.Find(Id);
			clan.Name = Name;
			clan.FullName = FullName;
			clan.Alias = Alias;
			clan.CalendarId = Calendar.Id;
			clan.Description = Description;
			clan.ShowClanMembersInWho = ShowClanMembersInWho;
			clan.ShowFamousMembersInNotables = ShowFamousMembersInNotables;
			clan.Sphere = Sphere;
			clan.IsTemplate = IsTemplate;
			clan.PayIntervalType = (int)PayInterval.Type;
			clan.PayIntervalModifier = PayInterval.IntervalAmount;
			clan.PayIntervalOther = PayInterval.Modifier;
			clan.PayIntervalReferenceDate = NextPay.Date.GetDateString();
			clan.PayIntervalReferenceTime = NextPay.Time.GetTimeString();
			clan.BankAccountId = _clanBankAccountId;
			clan.PaymasterId = _paymasterId;
			clan.PaymasterItemProtoId = _paymasterItemProtoId;
			clan.MaximumPeriodsOfUncollectedBackPay = MaximumPeriodsOfUncollectedBackPay;
			clan.OnPayProgId = OnPayProg?.Id;
			clan.DiscordChannelId = DiscordChannelId;
			FMDB.Context.ClansAdministrationCells.RemoveRange(clan.ClansAdministrationCells);
			foreach (var cell in AdministrationCells)
			{
				clan.ClansAdministrationCells.Add(new Models.ClanAdministrationCell { Clan = clan, CellId = cell.Id });
			}

			FMDB.Context.ClansTreasuryCells.RemoveRange(clan.ClansTreasuryCells);
			foreach (var cell in TreasuryCells)
			{
				clan.ClansTreasuryCells.Add(new Models.ClanTreasuryCell { Clan = clan, CellId = cell.Id });
			}

			FMDB.Context.SaveChanges();
		}

		Changed = false;
	}

	public override string FrameworkItemType => "Clan";

	public void SetRank(IClanMembership membership, IRank newRank)
	{
		membership.Rank = newRank;
		membership.Changed = true;
	}

	public void SetPaygrade(IClanMembership membership, IPaygrade newPaygrade)
	{
		membership.Paygrade = newPaygrade;
		membership.Changed = true;
	}

	public void RemoveMembership(IClanMembership membership)
	{
		membership.IsArchivedMembership = true;
		var oldAppointments = membership.Appointments.ToList();
		membership.Appointments.Clear();
		foreach (var appointment in oldAppointments)
		{
			appointment.CheckForByElections();
		}

		membership.Changed = true;
		Gameworld.Actors.Get(membership.MemberId)?.RemoveMembership(membership);
		Gameworld.CachedActors.Get(membership.MemberId)?.RemoveMembership(membership);
	}

	public void DismissAppointment(IClanMembership membership, IAppointment appointment)
	{
		membership.Appointments.Remove(appointment);
		membership.Changed = true;
		appointment.CheckForByElections();
	}

	public bool FreePosition(IAppointment appointment)
	{
		return appointment.MaximumSimultaneousHolders < 1 ||
		       appointment.MaximumSimultaneousHolders -
		       Memberships.Where(x => x.Appointments.Contains(appointment))
		                  .Sum(
			                  x =>
				                  ExternalControls.Any()
					                  ? ExternalControls.Sum(y => y.NumberOfAppointments - y.Appointees.Count)
					                  : 0) >= 1;
	}

	public bool FreePosition(IAppointment appointment, IClan liegeClan)
	{
		var external =
			ExternalControls.FirstOrDefault(
				x =>
					x.LiegeClan == liegeClan && x.VassalClan == this &&
					x.ControlledAppointment == appointment);

		return (external?.NumberOfAppointments - external?.Appointees.Count ?? 0) > 0;
	}

	public static bool CanCreateClan(ICharacter character)
	{
		return CanCreateClanProg?.ExecuteBool(character) ?? true;
	}

	public static void SetupClans(IFuturemud gameworld)
	{
		var stringValue = gameworld.GetStaticConfiguration("PlayersCanCreateClansProg");
		if (stringValue != null && long.TryParse(stringValue, out var value))
		{
			CanCreateClanProg = gameworld.FutureProgs.Get(value);
		}

		stringValue = gameworld.GetStaticConfiguration("OnCreateClanProg");
		if (stringValue != null && long.TryParse(stringValue, out value))
		{
			OnCreateClanProg = gameworld.FutureProgs.Get(value);
		}
	}

	protected virtual void ProcessPays(object[] arguments)
	{
		foreach (var member in Memberships)
		{
			if (member.Paygrade != null)
			{
				member.AwardPay(member.Paygrade.PayCurrency, member.Paygrade.PayAmount);
			}

			foreach (var appointment in member.Appointments.Where(x => x.Paygrade != null))
			{
				member.AwardPay(appointment.Paygrade.PayCurrency, appointment.Paygrade.PayAmount);
			}
		}

		_nextPay = new MudDateTime(PayInterval.GetNextDateExclusive(Calendar, NextPay.Date), NextPay.Time,
			NextPay.TimeZone);
		_paydayListener = CreatePaydayListener();
		Changed = true;
	}

	public class ClanFactory
	{
		public static IClan LoadClan(MudSharp.Models.Clan clan, IFuturemud gameworld)
		{
			return new Clan(clan, gameworld);
		}
	}

	#region IClan Members

	public bool IsTemplate { get; set; }

	public bool ShowClanMembersInWho { get; set; }

	public string Sphere { get; set; }
	public bool ShowFamousMembersInNotables { get; set; }
	public ulong? DiscordChannelId { get; set; }
	public string Alias { get; set; }

	private string _fullName;

	public string FullName
	{
		get => _fullName;
		set
		{
			_fullName = value;
			_name = value;
		}
	}

	IEnumerable<string> IHaveMultipleNames.Names => [Name, FullName, Alias];

	public string Description { get; set; }

	private readonly List<ICell> _treasuryCells = new();
	public IEnumerable<ICell> TreasuryCells => _treasuryCells;

	private readonly List<ICell> _administrationCells = new();
	public IEnumerable<ICell> AdministrationCells => _administrationCells;

	public void AddTreasuryCell(ICell cell)
	{
		_treasuryCells.Add(cell);
		Changed = true;
	}
	public void RemoveTreasuryCell(ICell cell)
	{
		_treasuryCells.Remove(cell);
		Changed = true;
	}
	public void AddAdministrationCell(ICell cell)
	{
		_administrationCells.Add(cell);
		Changed = true;
	}
	public void RemoveAdministrationCell(ICell cell)
	{
		_administrationCells.Remove(cell);
		Changed = true;
	}

	private long? _clanBankAccountId;
	private IBankAccount _clanBankAccount;

	public IBankAccount ClanBankAccount
	{
		get { return _clanBankAccount ??= Gameworld.BankAccounts.Get(_clanBankAccountId ?? 0L); }
		set
		{
			_clanBankAccount = value;
			_clanBankAccountId = value?.Id;
			Changed = true;
		}
	}

	private readonly List<IRank> _ranks = new();
	public virtual List<IRank> Ranks => _ranks;

	public List<IAppointment> Appointments { get; } = new();

	public List<IClanMembership> Memberships { get; } = new();

	private readonly List<IPaygrade> _paygrades = new();
	public virtual List<IPaygrade> Paygrades => _paygrades;

	public virtual List<IExternalClanControl> ExternalControls { get; } = new();

	private RecurringInterval _payInterval;

	public RecurringInterval PayInterval
	{
		get => _payInterval;
		set
		{
			_payInterval = value;
			_paydayListener?.CancelListener();
			_paydayListener = CreatePaydayListener();
			Changed = true;
		}
	}

	private MudDateTime _nextPay;

	public MudDateTime NextPay
	{
		get => _nextPay;
		set
		{
			_nextPay = value;
			_paydayListener?.CancelListener();
			_paydayListener = CreatePaydayListener();
			Changed = true;
		}
	}

	private long? _paymasterId;
	private ICharacter _paymaster;

	/// <summary>
	///     If not null, the Paymaster must be present for pay to be collected
	/// </summary>
	public ICharacter Paymaster
	{
		get
		{
			if (_paymaster != null)
			{
				return _paymaster;
			}

			_paymaster = Gameworld.TryGetCharacter(_paymasterId ?? 0, true);
			return _paymaster;
		}
		set
		{
			_paymaster = value;
			_paymasterId = value?.Id;
			Changed = true;
		}
	}

	private long? _paymasterItemProtoId;
	private IGameItemProto _paymasterItemProto;

	/// <summary>
	///     If not null, an instance of the paymaster item proto must exist in the location for pay to be collected
	/// </summary>
	public IGameItemProto PaymasterItemProto
	{
		get
		{
			if (_paymasterItemProto != null)
			{
				return _paymasterItemProto;
			}

			_paymasterItemProto = Gameworld.ItemProtos.Get(_paymasterItemProtoId ?? 0);
			return _paymasterItemProto;
		}
		set
		{
			_paymasterItemProto = value;
			_paymasterItemProtoId = value?.Id;
			Changed = true;
		}
	}

	/// <summary>
	///     Executed when someone collects pay in this clan
	/// </summary>
	public IFutureProg OnPayProg { get; set; }

	public int? MaximumPeriodsOfUncollectedBackPay { get; set; }

	private ICalendar _calendar;

	public ICalendar Calendar
	{
		get => _calendar;
		set
		{
			if (_calendar is not null && _calendar != value && value is not null)
			{
				Changed = true;
				_calendar = value;
				NextPay = NextPay.ConvertToOtherCalendar(value);
				foreach (var membership in Memberships)
				{
					membership.JoinDate = membership.JoinDate.ConvertToOtherCalendar(value);
					membership.Changed = true;
				}

				foreach (var election in Appointments.SelectMany(x => x.Elections))
				{
					election.NominationStartDate = election.NominationStartDate?.ConvertToOtherCalendar(value);
					election.ResultsInEffectDate = election.ResultsInEffectDate?.ConvertToOtherCalendar(value);
					election.VotingEndDate = election.VotingEndDate?.ConvertToOtherCalendar(value);
					election.VotingStartDate = election.VotingStartDate?.ConvertToOtherCalendar(value);
					election.Changed = true;
				}

				return;
			}
			_calendar = value;
			Changed = true;
		}
	}

	public virtual void FinaliseLoad(MudSharp.Models.Clan clan, IEnumerable<Models.ClanMembership> memberships)
	{
#if DEBUG
		var sw = new Stopwatch();
		sw.Start();
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Loading #5paygrades#0 [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		foreach (var item in clan.Paygrades)
		{
			_paygrades.Add(new Paygrade(item, this));
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Loading ranks [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		var rankNumber = 0;
		foreach (var item in clan.Ranks.OrderBy(x => x.RankNumber))
		{
			_ranks.Add(new Rank(item, this) { RankNumber = rankNumber++ });
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Loading appointments [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		var staging = new Dictionary<IAppointment, MudSharp.Models.Appointment>();
		foreach (var item in clan.Appointments)
		{
			var newAppointment = new Appointment(item, this);
			Appointments.Add(newAppointment);
			staging.Add(newAppointment, item);
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Finalising appointments [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		foreach (var item in staging)
		{
			item.Key.FinaliseLoad(item.Value);
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Loading memberships [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		foreach (var item in memberships.Where(x => x.ClanId == Id))
		{
			Memberships.Add(new ClanMembership(item, this, Gameworld));
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Loading elections [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		if (!IsTemplate)
		{
			foreach (var item in Appointments)
			{
				var elections = FMDB.Context.Elections.Where(x => x.AppointmentId == item.Id)
				                    .Include(x => x.ElectionNominees).Include(x => x.ElectionVotes).AsEnumerable();
				item.LoadElections(elections);
			}
		}

#if DEBUG
		ConsoleUtilities.WriteLine($"...Clan #6{FullName}#0...Setting up listener [#2{sw.ElapsedMilliseconds}ms#0]");
#endif
		_paydayListener = CreatePaydayListener();
	}

	private ITemporalListener CreatePaydayListener()
	{
		return PayInterval.CreateListenerFromInterval(Calendar, NextPay.Date, NextPay.Time, NextPay.TimeZone, ProcessPays, new object[] { }, $"Clan Payday Listener for {FullName.ColourName()}");
	}

	public void Disband(ICharacter disbander)
	{
		Gameworld.SaveManager.Abort(this);
		using (new FMDB())
		{
			Gameworld.SaveManager.Flush();
			var dbclan = FMDB.Context.Clans.Find(Id);
			if (dbclan == null)
			{
				disbander.Send("That clan has already been disbanded.");
				return;
			}

			FMDB.Context.Clans.Remove(dbclan);
			FMDB.Context.SaveChanges();
		}

		var emote = new EmoteOutput(new Emote($"@ have|has disbanded the clan \"{FullName}\", effective immediately.",
			disbander));
		foreach (var member in Memberships)
		{
			var ch = Gameworld.Actors.Get(member.MemberId);
			ch?.RemoveMembership(member);
			ch?.OutputHandler.Send(emote);
			Gameworld.SaveManager.Abort(member);
		}

		Gameworld.Destroy(this);
	}

	public void DeleteRank(IRank rank)
	{
		var ordered = Ranks.OrderBy(x => x.RankNumber).ToList();
		var newRank =
				ordered.LastOrDefault(x => x.RankNumber < rank.RankNumber && x.RankPath == rank.RankPath) ??
				ordered.FirstOrDefault(x => x.RankNumber > rank.RankNumber && x.RankPath == rank.RankPath) ??
				ordered.LastOrDefault(x => x.RankNumber < rank.RankNumber) ??
				ordered.First(x => x.RankNumber > rank.RankNumber)
			;
		Gameworld.SaveManager.Flush();
		foreach (var member in Memberships)
		{
			if (member.Rank == rank)
			{
				member.SetRank(newRank);
			}
		}

		foreach (var item in Appointments)
		{
			if (item.MinimumRankToAppoint == rank)
			{
				item.MinimumRankToAppoint = newRank;
			}

			if (item.MinimumRankToHold == rank)
			{
				item.MinimumRankToHold = newRank;
			}
		}

		Ranks.Remove(rank);
		rank.Delete();
	}

	public void DeleteAppointment(IAppointment appointment)
	{
		Gameworld.SaveManager.Flush();
		foreach (var member in Memberships)
		{
			if (member.Appointments.Contains(appointment))
			{
				member.Appointments.Remove(appointment);
				member.Changed = true;
			}
		}

		foreach (var item in Appointments)
		{
			if (item.ParentPosition == appointment)
			{
				item.ParentPosition = null;
				item.Changed = true;
			}
		}

		foreach (var item in ExternalControls.ToList())
		{
			if (item.ControlledAppointment == appointment)
			{
				item.Delete();
				ExternalControls.Remove(item);
			}

			if (item.ControllingAppointment == appointment)
			{
				item.Delete();
				ExternalControls.Remove(item);
			}
		}
	}

	public void DeletePaygrade(IPaygrade paygrade)
	{
		Gameworld.SaveManager.Flush();
		foreach (var member in Memberships)
		{
			if (member.Paygrade == paygrade)
			{
				member.Paygrade = null;
				member.Changed = true;
			}
		}
	}

	private ITemporalListener _paydayListener;

	#endregion

	#region IFutureProgVariable Members

	public IProgVariable GetProperty(string property)
	{
		switch (property.ToLowerInvariant())
		{
			case "id":
				return new NumberVariable(Id);
			case "name":
				return new TextVariable(FullName);
			case "alias":
				return new TextVariable(Alias);
			case "description":
				return new TextVariable(Description);
			case "ranks":
				return new CollectionVariable(new List<IRank>(Ranks), ProgVariableTypes.ClanRank);
			case "paygrades":
				return new CollectionVariable(new List<IPaygrade>(Paygrades), ProgVariableTypes.ClanPaygrade);
			case "appointments":
				return new CollectionVariable(new List<IAppointment>(Appointments),
					ProgVariableTypes.ClanAppointment);
			case "bankaccount":
				return ClanBankAccount;
			case "onlinemembers":
				return
					new CollectionVariable(
						Gameworld.Characters.Where(x => x.ClanMemberships.Any(y => y.Clan == this)).ToList(),
						ProgVariableTypes.Character);
			default:
				return null;
		}
	}

	public ProgVariableTypes Type => ProgVariableTypes.Clan;

	public object GetObject => this;

	private static IReadOnlyDictionary<string, ProgVariableTypes> DotReferenceHandler()
	{
		return new Dictionary<string, ProgVariableTypes>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", ProgVariableTypes.Number },
			{ "name", ProgVariableTypes.Text },
			{ "alias", ProgVariableTypes.Text },
			{ "description", ProgVariableTypes.Text },
			{ "ranks", ProgVariableTypes.Collection | ProgVariableTypes.ClanRank },
			{ "paygrades", ProgVariableTypes.Collection | ProgVariableTypes.ClanPaygrade },
			{ "appointments", ProgVariableTypes.Collection | ProgVariableTypes.ClanAppointment },
			{ "onlinemembers", ProgVariableTypes.Collection | ProgVariableTypes.Character },
			{ "bankaccount", ProgVariableTypes.BankAccount }
		};
	}

	private static IReadOnlyDictionary<string, string> DotReferenceHelp()
	{
		return new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "id", "The Id of the clan" },
			{ "name", "The name of the clan" },
			{ "alias", "The alias of the clan" },
			{ "description", "The full description of the clan" },
			{ "ranks", "An ordered list of the ranks in the clan" },
			{ "paygrades", "A list of all of the paygrades in the clan" },
			{ "appointments", "A list of all of the appointments in the clan" },
			{ "onlinemembers", "A list of all of the online characters who are clan members" },
			{ "bankaccount", "The bank account for the clan, if one is set. Otherwise null." }
		};
	}

	public static void RegisterFutureProgCompiler()
	{
		ProgVariable.RegisterDotReferenceCompileInfo(ProgVariableTypes.Clan, DotReferenceHandler(),
			DotReferenceHelp());
	}

	internal static void CreatedClan(ICharacter actor, Clan newClan)
	{
		OnCreateClanProg?.Execute(actor, newClan);
	}

	#endregion
}