using Anthropic.SDK.Messaging;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems.Prototypes;
using MudSharp.NPC;
using MudSharp.TimeAndDate.Date;
using MudSharp.TimeAndDate.Time;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.Economy.Stables;

public class Stable : SavableKeywordedItem, IStable
{
	private readonly List<IEmployeeRecord> _employeeRecords = new();
	private readonly List<IStableStay> _stays = new();
	private readonly List<IStableAccount> _stableAccounts = new();
	private IEconomicZone _economicZone;
	private ICell _location;
	private long? _bankAccountId;
	private IBankAccount? _bankAccount;
	private bool _isTrading;
	private decimal _lodgeFee;
	private decimal _dailyFee;
	private IFutureProg? _lodgeFeeProg;
	private IFutureProg? _dailyFeeProg;
	private IFutureProg? _canStableProg;
	private IFutureProg? _whyCannotStableProg;

	public Stable(IEconomicZone zone, ICell location, IBankAccount? bankAccount, string name)
	{
		Gameworld = zone.Gameworld;
		_name = name;
		SetKeywordsFromSDesc(name);
		_economicZone = zone;
		_location = location;
		_bankAccountId = bankAccount?.Id;
		_bankAccount = bankAccount;
		_isTrading = true;

		using (new FMDB())
		{
			MudSharp.Models.Stable dbitem = new()
			{
				Name = name,
				EconomicZoneId = zone.Id,
				CellId = location.Id,
				BankAccountId = bankAccount?.Id,
				IsTrading = true,
				LodgeFee = 0.0M,
				DailyFee = 0.0M,
				EmployeeRecords = "<Employees/>"
			};
			FMDB.Context.Stables.Add(dbitem);
			FMDB.Context.SaveChanges();
			_id = dbitem.Id;
		}
	}

	public Stable(MudSharp.Models.Stable stable, IFuturemud gameworld)
	{
		Gameworld = gameworld;
		_id = stable.Id;
		_name = stable.Name;
		SetKeywordsFromSDesc(stable.Name);
		_economicZone = gameworld.EconomicZones.Get(stable.EconomicZoneId)!;
		_location = gameworld.Cells.Get(stable.CellId)!;
		_bankAccountId = stable.BankAccountId;
		_isTrading = stable.IsTrading;
		_lodgeFee = stable.LodgeFee;
		_dailyFee = stable.DailyFee;
		_lodgeFeeProg = gameworld.FutureProgs.Get(stable.LodgeFeeProgId ?? 0);
		_dailyFeeProg = gameworld.FutureProgs.Get(stable.DailyFeeProgId ?? 0);
		_canStableProg = gameworld.FutureProgs.Get(stable.CanStableProgId ?? 0);
		_whyCannotStableProg = gameworld.FutureProgs.Get(stable.WhyCannotStableProgId ?? 0);

		var employees = XElement.Parse(stable.EmployeeRecords);
		foreach (var item in employees.Elements())
		{
			_employeeRecords.Add(new EmployeeRecord(item, gameworld));
		}

		foreach (var stay in stable.Stays.OrderBy(x => x.Id))
		{
			_stays.Add(new StableStay(stay, this));
		}

		foreach (var account in stable.StableAccounts.OrderBy(x => x.Id))
		{
			_stableAccounts.Add(new StableAccount(account, this));
		}
	}

	public override string FrameworkItemType => "Stable";
	public IEconomicZone EconomicZone
	{
		get => _economicZone;
		set
		{
			if (_economicZone == value)
			{
				return;
			}

			AssessAllActiveStays();
			_economicZone = value;
			_bankAccount = null;
			_bankAccountId = null;
			Changed = true;
		}
	}

	public ICurrency Currency => EconomicZone.Currency;

	public ICell Location
	{
		get => _location;
		set
		{
			_location = value;
			Changed = true;
		}
	}

	public IBankAccount? BankAccount
	{
		get
		{
			if (_bankAccount is null && _bankAccountId is not null)
			{
				_bankAccount = Gameworld.BankAccounts.Get(_bankAccountId.Value);
			}

			return _bankAccount;
		}
		set
		{
			_bankAccount = value;
			_bankAccountId = value?.Id;
			Changed = true;
		}
	}

	public bool IsTrading => _isTrading;
	public bool IsReadyToDoBusiness => IsTrading &&
	                                  StableTicketGameItemComponentProto.ItemPrototype is not null;
	public decimal CashBalance => VirtualCashLedger.Balance(this, Currency);
	public decimal AvailableFunds => VirtualCashLedger.AvailableFunds(this, Currency, BankAccount);

	public decimal LodgeFee
	{
		get => _lodgeFee;
		set
		{
			AssessAllActiveStays();
			_lodgeFee = Math.Max(0.0M, value);
			_lodgeFeeProg = null;
			Changed = true;
		}
	}

	public decimal DailyFee
	{
		get => _dailyFee;
		set
		{
			AssessAllActiveStays();
			_dailyFee = Math.Max(0.0M, value);
			_dailyFeeProg = null;
			Changed = true;
		}
	}

	public IFutureProg? LodgeFeeProg
	{
		get => _lodgeFeeProg;
		set
		{
			AssessAllActiveStays();
			_lodgeFeeProg = value;
			Changed = true;
		}
	}

	public IFutureProg? DailyFeeProg
	{
		get => _dailyFeeProg;
		set
		{
			AssessAllActiveStays();
			_dailyFeeProg = value;
			Changed = true;
		}
	}

	public IFutureProg? CanStableProg
	{
		get => _canStableProg;
		set
		{
			_canStableProg = value;
			Changed = true;
		}
	}

	public IFutureProg? WhyCannotStableProg
	{
		get => _whyCannotStableProg;
		set
		{
			_whyCannotStableProg = value;
			Changed = true;
		}
	}

	public IEnumerable<IEmployeeRecord> EmployeeRecords => _employeeRecords;
	public IEnumerable<IStableStay> Stays => _stays;
	public IEnumerable<IStableStay> ActiveStays => _stays.Where(x => x.IsActive);
	public IEnumerable<IStableAccount> StableAccounts => _stableAccounts;

	public override void Save()
	{
		var dbitem = FMDB.Context.Stables.Find(Id);
		if (dbitem is null)
		{
			Changed = false;
			return;
		}

		dbitem.Name = Name;
		dbitem.EconomicZoneId = EconomicZone.Id;
		dbitem.CellId = Location.Id;
		dbitem.BankAccountId = BankAccount?.Id;
		dbitem.IsTrading = IsTrading;
		dbitem.LodgeFee = LodgeFee;
		dbitem.DailyFee = DailyFee;
		dbitem.LodgeFeeProgId = LodgeFeeProg?.Id;
		dbitem.DailyFeeProgId = DailyFeeProg?.Id;
		dbitem.CanStableProgId = CanStableProg?.Id;
		dbitem.WhyCannotStableProgId = WhyCannotStableProg?.Id;
		dbitem.EmployeeRecords = new XElement("Employees",
			from employee in EmployeeRecords
			select employee.SaveToXml()
		).ToString();
		Changed = false;
	}

	public bool IsEmployee(ICharacter actor)
	{
		return _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id);
	}

	public bool IsManager(ICharacter actor)
	{
		return actor.IsAdministrator() ||
		       _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id && x.IsManager);
	}

	public bool IsProprietor(ICharacter actor)
	{
		return actor.IsAdministrator() ||
		       _employeeRecords.Any(x => x.EmployeeCharacterId == actor.Id && x.IsProprietor);
	}

	public void AddEmployee(ICharacter actor)
	{
		if (IsEmployee(actor))
		{
			return;
		}

		_employeeRecords.Add(new EmployeeRecord(actor));
		Changed = true;
	}

	public void RemoveEmployee(IEmployeeRecord employee)
	{
		_employeeRecords.Remove(employee);
		Changed = true;
	}

	public void RemoveEmployee(ICharacter actor)
	{
		var record = _employeeRecords.FirstOrDefault(x => x.EmployeeCharacterId == actor.Id);
		if (record is not null)
		{
			RemoveEmployee(record);
		}
	}

	public void ClearEmployees()
	{
		_employeeRecords.Clear();
		Changed = true;
	}

	public void SetManager(ICharacter actor, bool isManager)
	{
		var record = _employeeRecords.First(x => x.EmployeeCharacterId == actor.Id);
		record.IsManager = isManager;
		Changed = true;
	}

	public void SetProprietor(ICharacter actor, bool isProprietor)
	{
		var record = _employeeRecords.First(x => x.EmployeeCharacterId == actor.Id);
		record.IsProprietor = isProprietor;
		Changed = true;
	}

	public void ToggleIsTrading()
	{
		_isTrading = !IsTrading;
		Changed = true;
	}

	public decimal QuoteLodgeFee(ICharacter mount, ICharacter owner)
	{
		return Math.Max(0.0M, LodgeFeeProg?.ExecuteDecimal(0.0M, mount, owner) ?? LodgeFee);
	}

	public decimal QuoteDailyFee(ICharacter mount, ICharacter owner)
	{
		return Math.Max(0.0M, DailyFeeProg?.ExecuteDecimal(0.0M, mount, owner) ?? DailyFee);
	}

	public void AssessFees(IStableStay stay)
	{
		if (!stay.IsActive)
		{
			return;
		}

		var mount = stay.Mount;
		var owner = stay.OriginalOwner;
		if (mount is null || owner is null)
		{
			return;
		}

		var now = EconomicZone.FinancialPeriodReferenceCalendar.CurrentDateTime;
		if (now.Date <= stay.LastDailyFeeDateTime.Date)
		{
			return;
		}

		var days = now.Date.DaysDifference(stay.LastDailyFeeDateTime.Date);
		if (days <= 0)
		{
			return;
		}

		var dailyFee = QuoteDailyFee(mount, owner);
		if (dailyFee > 0.0M)
		{
			stay.AddCharge(StableLedgerEntryType.DailyFee, dailyFee * days, null,
				$"{days.ToString("N0")} stabling day{(days == 1 ? string.Empty : "s")}");
		}

		stay.LastDailyFeeDateTime = now;
	}

	public void AssessAllActiveStays()
	{
		foreach (var stay in ActiveStays.ToList())
		{
			AssessFees(stay);
		}
	}

	public (bool Truth, string Reason) CanUseStable(ICharacter actor, ICharacter? mount)
	{
		if (!IsReadyToDoBusiness)
		{
			return (false, $"{Name.TitleCase()} is not presently ready to stable mounts.");
		}

		if (CanStableProg?.ExecuteBool(true, actor, mount) == false)
		{
			return (false, WhyCannotStableProg?.ExecuteString(actor, mount) ??
			               $"{Name.TitleCase()} is not willing to stable that mount for you.");
		}

		return (true, string.Empty);
	}

	public (bool Truth, string Reason) CanLodge(ICharacter actor, ICharacter mount)
	{
		if (mount == actor)
		{
			return (false, "You cannot stable yourself.");
		}

		if (mount is not INPC)
		{
			return (false, $"{mount.HowSeen(actor, true)} is not an NPC mount.");
		}

		if (mount.Location != Location)
		{
			return (false, $"{mount.HowSeen(actor, true)} is not here.");
		}

		if (!mount.CanEverBeMounted(actor))
		{
			return (false, mount.WhyCannotBeMountedBy(actor));
		}

		if (mount.State.HasFlag(CharacterState.Dead))
		{
			return (false, $"{mount.HowSeen(actor, true)} is dead.");
		}

		if (mount.State.HasFlag(CharacterState.Stasis))
		{
			return (false, $"{mount.HowSeen(actor, true)} is already in stasis.");
		}

		if (mount.Combat is not null)
		{
			return (false, $"{mount.HowSeen(actor, true)} is in combat.");
		}

		if (mount.Riders.Any())
		{
			return (false, $"{mount.HowSeen(actor, true)} is currently being ridden.");
		}

		return CanUseStable(actor, mount);
	}

	public IStableStay Lodge(ICharacter actor, ICharacter mount)
	{
		var stay = new StableStay(this, mount, actor);
		_stays.Add(stay);
		var lodgeFee = QuoteLodgeFee(mount, actor);
		stay.AddCharge(StableLedgerEntryType.LodgeFee, lodgeFee, actor, "Initial stabling fee");
		return stay;
	}

	public (bool Truth, string Reason) CanRedeem(ICharacter actor, IStableTicket ticket)
	{
		if (!IsReadyToDoBusiness)
		{
			return (false, $"{Name.TitleCase()} is not presently ready to redeem mounts.");
		}

		if (!ticket.IsValid || ticket.StableStay is null || ticket.StableStay.Stable != this)
		{
			return (false, "That stable ticket is not valid here.");
		}

		return (true, string.Empty);
	}

	public void Redeem(ICharacter actor, IStableStay stay)
	{
		AssessFees(stay);
		stay.Close(StableStayStatus.Redeemed, actor, "Redeemed by stable ticket");
		RestoreMount(stay);
	}

	public void Release(ICharacter actor, IStableStay stay, bool waiveFees)
	{
		AssessFees(stay);
		if (waiveFees)
		{
			stay.WaiveOutstanding(actor, "Waived by stable manager");
		}

		stay.Close(StableStayStatus.ReleasedByManager, actor,
			waiveFees ? "Released by stable manager with fees waived" : "Released by stable manager");
		RestoreMount(stay);
	}

	public void AddStableAccount(IStableAccount account)
	{
		_stableAccounts.Add(account);
	}

	public void RemoveStableAccount(IStableAccount account)
	{
		_stableAccounts.Remove(account);
		using (new FMDB())
		{
			var dbitem = FMDB.Context.StableAccounts.Find(account.Id);
			if (dbitem is not null)
			{
				FMDB.Context.StableAccounts.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	public IStableAccount? AccountByName(string text)
	{
		if (long.TryParse(text, out var id))
		{
			return _stableAccounts.FirstOrDefault(x => x.Id == id);
		}

		return _stableAccounts.FirstOrDefault(x => x.AccountName.EqualTo(text)) ??
		       _stableAccounts.FirstOrDefault(x => x.AccountName.StartsWith(text, StringComparison.InvariantCultureIgnoreCase));
	}

	public string Show(ICharacter actor)
	{
		AssessAllActiveStays();
		StringBuilder sb = new();
		sb.AppendLine($"Stable #{Id.ToString("N0", actor)} - {Name.TitleCase().ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Location: {Location.HowSeen(actor).ColourName()} (#{Location.Id.ToString("N0", actor)})");
		sb.AppendLine($"Economic Zone: {EconomicZone.Name.ColourName()}");
		sb.AppendLine($"Bank Account: {(BankAccount is null ? "None".ColourError() : BankAccount.AccountReference.ColourValue())}");
		sb.AppendLine($"Virtual Cash: {Currency.Describe(CashBalance, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Available Funds: {Currency.Describe(AvailableFunds, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine($"Open: {IsTrading.ToColouredString()}");
		sb.AppendLine($"Ready: {IsReadyToDoBusiness.ToColouredString()}");
		sb.AppendLine($"Lodge Fee: {(LodgeFeeProg is null ? Currency.Describe(LodgeFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue() : LodgeFeeProg.MXPClickableFunctionNameWithId())}");
		sb.AppendLine($"Daily Fee: {(DailyFeeProg is null ? Currency.Describe(DailyFee, CurrencyDescriptionPatternType.ShortDecimal).ColourValue() : DailyFeeProg.MXPClickableFunctionNameWithId())}");
		sb.AppendLine($"Can Stable Prog: {(CanStableProg is null ? "None".ColourError() : CanStableProg.MXPClickableFunctionNameWithId())}");
		sb.AppendLine($"Why Prog: {(WhyCannotStableProg is null ? "None".ColourError() : WhyCannotStableProg.MXPClickableFunctionNameWithId())}");
		sb.AppendLine($"Active Stays: {ActiveStays.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Historical Stays: {Stays.Count(x => !x.IsActive).ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Stable Accounts: {StableAccounts.Count().ToString("N0", actor).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Employees:");
		foreach (var employee in EmployeeRecords.OrderByDescending(x => x.IsProprietor).ThenByDescending(x => x.IsManager).ThenBy(x => x.Name.GetName(NameStyle.FullName)))
		{
			sb.AppendLine($"\t{employee.Name.GetName(NameStyle.FullName).ColourName()} [{(employee.IsProprietor ? "Proprietor" : employee.IsManager ? "Manager" : "Employee").ColourValue()}]");
		}

		return sb.ToString();
	}

	public string ShowToNonEmployee(ICharacter actor)
	{
		AssessAllActiveStays();
		var sb = new StringBuilder();

		sb.AppendLine(Name.GetLineWithTitleInner(actor, Telnet.Yellow, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"Open: {IsTrading.ToColouredString()}");
        sb.AppendLine($"Ready: {IsReadyToDoBusiness.ToColouredString()}");
		var (truth, error) = CanUseStable(actor, null);
		if (truth)
		{
			sb.AppendLine($"Can You Use: {"yes".ColourValue()}");
		}
		else
		{
			sb.AppendLine($"Can You Use: {"no".ColourError()} [{error.ColourCommand()}]");
		}

		var accounts = StableAccounts.Where(x => x.IsAuthorisedToUse(actor, 0.0M) != StableAccountAuthorisationFailureReason.NotAuthorisedAccountUser).ToList();
		var stays = ActiveStays.Where(x => x.OriginalOwnerId == actor.Id).ToList();

		sb.AppendLine();
		sb.AppendLine($"Active Stays:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from item in stays
			select new List<string>
			{
				item.Mount is not null ? item.Mount.HowSeen(actor, flags: PerceiveIgnoreFlags.TrueDescription) : "None",
				item.LodgedDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				(item.LodgedDateTime.Calendar.CurrentDateTime - item.LodgedDateTime).DescribePreciseBrief(actor),
				Currency.Describe(item.AmountOwing, CurrencyDescriptionPatternType.ShortDecimal)
            },
			new List<string>
			{
				"Mount",
				"Lodged Time",
				"Stay Duration",
				"Amount Owing"
			},
			actor,
			Telnet.Cyan
		));

		sb.AppendLine();
        sb.AppendLine($"Available Accounts:");
        sb.AppendLine(StringUtilities.GetTextTable(
            from item in accounts
            select new List<string>
            {
				item.Id.ToStringN0(actor),
				item.AccountName,
				item.AccountOwnerName.GetName(NameStyle.SimpleFull),
				item.IsAccountOwner(actor).ToColouredString(),
				Currency.Describe(item.Balance, CurrencyDescriptionPatternType.ShortDecimal),
                Currency.Describe(item.CreditAvailable, CurrencyDescriptionPatternType.ShortDecimal),
                Currency.Describe(item.CreditLimit, CurrencyDescriptionPatternType.ShortDecimal),
                Currency.Describe(item.MaximumAuthorisedToUse(actor), CurrencyDescriptionPatternType.ShortDecimal),
            },
            new List<string>
            {
				"Id",
				"Account Name",
				"Owner Name",
				"You Own?",
				"Balance",
				"Available Credit",
				"Credit Limit",
                "Your Limit"
            },
            actor,
            Telnet.Cyan
        ));

        return sb.ToString();
	}

    public string ShowStay(ICharacter actor, IStableStay stay)
	{
		AssessFees(stay);
		StringBuilder sb = new();
		sb.AppendLine($"Stable Stay #{stay.Id.ToString("N0", actor)} - {Name.TitleCase().ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Status: {stay.Status.DescribeEnum().ColourName()}");
		sb.AppendLine($"Mount: {(stay.Mount is null ? $"#{stay.MountId.ToString("N0", actor)}".ColourValue() : stay.Mount.HowSeen(actor).ColourName())}");
		sb.AppendLine($"Original Lodger: {(stay.OriginalOwnerName?.GetName(NameStyle.FullName) ?? $"#{stay.OriginalOwnerId.ToString("N0", actor)}").ColourName()}");
		sb.AppendLine($"Lodged: {stay.LodgedDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine($"Last Fee Assessment: {stay.LastDailyFeeDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue()}");
		sb.AppendLine($"Closed: {(stay.ClosedDateTime is null ? "Still Active".ColourValue() : stay.ClosedDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short).ColourValue())}");
		sb.AppendLine($"Ticket Item: {(stay.TicketItemId?.ToString("N0", actor).ColourValue() ?? "None".ColourError())}");
		sb.AppendLine($"Amount Owing: {Currency.Describe(stay.AmountOwing, CurrencyDescriptionPatternType.ShortDecimal).ColourValue()}");
		sb.AppendLine();
		sb.AppendLine("Ledger:");
		sb.AppendLine(StringUtilities.GetTextTable(
			from entry in stay.LedgerEntries
			select new[]
			{
				entry.Id.ToString("N0", actor),
				entry.MudDateTime.ToString(CalendarDisplayMode.Short, TimeDisplayTypes.Short),
				entry.EntryType.DescribeEnum(),
				entry.Amount == 0.0M ? "-" : Currency.Describe(entry.Amount, CurrencyDescriptionPatternType.ShortDecimal),
				entry.ActorName ?? "-",
				entry.Note
			},
			new[] { "#", "Date", "Type", "Amount", "Actor", "Note" },
			actor.LineFormatLength,
			colour: Telnet.Yellow,
			unicodeTable: actor.Account.UseUnicode
		));
		return sb.ToString();
	}

	public void Delete()
	{
		using (new FMDB())
		{
			var dbitem = FMDB.Context.Stables.Find(Id);
			if (dbitem is not null)
			{
				FMDB.Context.Stables.Remove(dbitem);
				FMDB.Context.SaveChanges();
			}
		}
	}

	private void RestoreMount(IStableStay stay)
	{
		var mount = stay.Mount;
		if (mount is null)
		{
			return;
		}

		if (!Gameworld.Actors.Has(mount))
		{
			Gameworld.Add(mount, true);
		}

		Location.Login(mount);
	}
}
