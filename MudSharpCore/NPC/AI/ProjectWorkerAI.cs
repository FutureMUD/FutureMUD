using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.Work.Projects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

#nullable enable

namespace MudSharp.NPC.AI;

public class ProjectWorkerAI : PathingAIBase
{
	private sealed record ProjectWorkCandidate(
		IActiveProject Project,
		IProjectLabourRequirement Labour,
		decimal HourlyPay,
		decimal EffectiveGlobalPay,
		int Distance);

	public static void RegisterLoader()
	{
		RegisterAIType("ProjectWorker", (ai, gameworld) => new ProjectWorkerAI(ai, gameworld));
		RegisterAIBuilderInformation("projectworker",
			(gameworld, name) => new ProjectWorkerAI(gameworld, name),
			new ProjectWorkerAI().HelpText);
	}

	private ProjectWorkerAI()
	{
		SetDefaults();
	}

	protected ProjectWorkerAI(IFuturemud gameworld, string name) : base(gameworld, name, "ProjectWorker")
	{
		SetDefaults();
		Currency = DefaultCurrency();
		OpenDoors = true;
		UseKeys = true;
		UseDoorguards = true;
		CloseDoorsBehind = true;
		MoveEvenIfObstructionInWay = true;
		DatabaseInitialise();
	}

	protected ProjectWorkerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	public decimal MinimumHourlyPay { get; private set; }
	public ICurrency? Currency { get; private set; }
	public uint MaxPathRange { get; private set; }
	public bool SearchEnabled { get; private set; }
	public bool ClaimPayments { get; private set; }
	public bool DepositToBank { get; private set; }
	public TimeSpan SearchCadence { get; private set; }
	public long? BankAccountTypeId { get; private set; }

	private void SetDefaults()
	{
		MinimumHourlyPay = 0.0M;
		MaxPathRange = 50;
		SearchEnabled = true;
		ClaimPayments = true;
		DepositToBank = false;
		SearchCadence = TimeSpan.FromMinutes(10);
		BankAccountTypeId = null;
	}

	protected override void LoadFromXML(XElement root)
	{
		SetDefaults();
		base.LoadFromXML(root);

		Currency = Gameworld.Currencies.Get(long.Parse(root.Element("Currency")?.Value ?? "0")) ?? DefaultCurrency();
		MinimumHourlyPay = decimal.TryParse(root.Element("MinimumHourlyPay")?.Value, out var wage)
			? Math.Max(0.0M, wage)
			: 0.0M;
		MaxPathRange = uint.TryParse(root.Element("MaxPathRange")?.Value, out var range) && range > 0
			? range
			: 50;
		SearchEnabled = bool.TryParse(root.Element("SearchEnabled")?.Value, out var searchEnabled)
			? searchEnabled
			: true;
		ClaimPayments = bool.TryParse(root.Element("ClaimPayments")?.Value, out var claimPayments)
			? claimPayments
			: true;
		DepositToBank = bool.TryParse(root.Element("DepositToBank")?.Value, out var depositToBank)
			? depositToBank
			: false;
		SearchCadence = double.TryParse(root.Element("SearchCadenceMinutes")?.Value, out var cadence) &&
		                cadence > 0.0
			? TimeSpan.FromMinutes(cadence)
			: TimeSpan.FromMinutes(10);
		BankAccountTypeId = long.TryParse(root.Element("BankAccountType")?.Value, out var typeId) && typeId > 0
			? typeId
			: null;
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Currency", Currency?.Id ?? 0L),
			new XElement("MinimumHourlyPay", MinimumHourlyPay),
			new XElement("MaxPathRange", MaxPathRange),
			new XElement("SearchEnabled", SearchEnabled),
			new XElement("ClaimPayments", ClaimPayments),
			new XElement("DepositToBank", DepositToBank),
			new XElement("SearchCadenceMinutes", SearchCadence.TotalMinutes),
			new XElement("BankAccountType", BankAccountTypeId ?? 0L),
			new XElement("OpenDoors", OpenDoors),
			new XElement("UseKeys", UseKeys),
			new XElement("SmashLockedDoors", SmashLockedDoors),
			new XElement("CloseDoorsBehind", CloseDoorsBehind),
			new XElement("UseDoorguards", UseDoorguards),
			new XElement("MoveEvenIfObstructionInWay", MoveEvenIfObstructionInWay)
		).ToString();
	}

	public override string Show(ICharacter actor)
	{
		var accountType = BankAccountTypeId.HasValue ? Gameworld.BankAccountTypes.Get(BankAccountTypeId.Value) : null;
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine();
		sb.AppendLine($"Currency: {Currency?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Minimum Hourly Pay: {DescribeMinimumPay().ColourValue()}");
		sb.AppendLine($"Max Path Range: {MaxPathRange.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Search Enabled: {SearchEnabled.ToColouredString()}");
		sb.AppendLine($"Claim Payments: {ClaimPayments.ToColouredString()}");
		sb.AppendLine($"Deposit Claims: {DepositToBank.ToColouredString()}");
		sb.AppendLine($"Account Type: {accountType?.Name.ColourName() ?? "none".ColourError()}");
		sb.AppendLine($"Search Cadence: {SearchCadence.Describe(actor).ColourValue()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3currency <currency>#0 - sets the currency used to parse and display minimum pay
	#3pay <amount>#0 - sets the minimum hourly project pay this worker will accept
	#3range <exits>#0 - sets maximum project path range
	#3search#0 - toggles autonomous project searching
	#3claim#0 - toggles autonomous project payment claiming
	#3deposit#0 - toggles depositing claimed project payments to bank accounts
	#3account <none|account type>#0 - sets an account type the worker may open for deposits
	#3cadence <minutes>#0 - sets the scan cadence after failed project searches";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "pay":
			case "minimum":
			case "minimumhourly":
			case "minimumhourlypay":
			case "wage":
				return BuildingCommandMinimumPay(actor, command);
			case "range":
			case "path":
			case "pathrange":
				return BuildingCommandRange(actor, command);
			case "search":
				SearchEnabled = !SearchEnabled;
				Changed = true;
				actor.OutputHandler.Send($"This AI will {SearchEnabled.NowNoLonger()} search for paid projects.");
				return true;
			case "claim":
			case "collect":
				ClaimPayments = !ClaimPayments;
				Changed = true;
				actor.OutputHandler.Send($"This AI will {ClaimPayments.NowNoLonger()} claim project payments.");
				return true;
			case "deposit":
			case "bank":
				DepositToBank = !DepositToBank;
				Changed = true;
				actor.OutputHandler.Send($"This AI will {DepositToBank.NowNoLonger()} deposit claimed project payments to a bank account when possible.");
				return true;
			case "account":
			case "accounttype":
				return BuildingCommandAccountType(actor, command);
			case "cadence":
			case "scan":
				return BuildingCommandCadence(actor, command);
		}

		return base.BuildingCommand(actor, command.GetUndo());
	}

	private bool BuildingCommandCurrency(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send($"Which currency should this project worker use for minimum pay? The valid currencies are {Gameworld.Currencies.Select(x => x.Name.ColourName()).ListToString()}.");
			return false;
		}

		var currency = Gameworld.Currencies.GetByIdOrName(command.SafeRemainingArgument);
		if (currency is null)
		{
			actor.OutputHandler.Send("There is no such currency.");
			return false;
		}

		Currency = currency;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now parse and display minimum project pay in {currency.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandMinimumPay(ICharacter actor, StringStack command)
	{
		if (Currency is null)
		{
			actor.OutputHandler.Send("You must first set a currency for this AI's minimum pay.");
			return false;
		}

		if (command.IsFinished || !Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var wage) ||
		    wage < 0.0M)
		{
			actor.OutputHandler.Send($"What non-negative hourly project pay in {Currency.Name.ColourName()} should this worker require?");
			return false;
		}

		MinimumHourlyPay = wage;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now require at least {DescribeMinimumPay().ColourValue()} per project hour.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !uint.TryParse(command.SafeRemainingArgument, out var range) || range == 0)
		{
			actor.OutputHandler.Send("What positive maximum path range should this project worker use?");
			return false;
		}

		MaxPathRange = range;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now search within {MaxPathRange.ToString("N0", actor).ColourValue()} exits.");
		return true;
	}

	private bool BuildingCommandAccountType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which bank account type should this worker open if it needs a deposit account? Use none to clear.");
			return false;
		}

		if (command.SafeRemainingArgument.EqualToAny("none", "clear", "off"))
		{
			BankAccountTypeId = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will no longer open a configured bank account type for project payment deposits.");
			return true;
		}

		var accountType = Gameworld.BankAccountTypes.GetByIdOrName(command.SafeRemainingArgument);
		if (accountType is null)
		{
			actor.OutputHandler.Send("There is no such bank account type.");
			return false;
		}

		BankAccountTypeId = accountType.Id;
		DepositToBank = true;
		Changed = true;
		actor.OutputHandler.Send($"This AI may now open {accountType.Name.ColourName()} accounts for project payment deposits.");
		return true;
	}

	private bool BuildingCommandCadence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, actor, out var minutes) ||
		    minutes <= 0.0)
		{
			actor.OutputHandler.Send("How many positive minutes should this worker wait after failing to find a paid project?");
			return false;
		}

		SearchCadence = TimeSpan.FromMinutes(minutes);
		Changed = true;
		actor.OutputHandler.Send($"This AI will now wait {SearchCadence.Describe(actor).ColourValue()} after an unsuccessful project search.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.MinuteTick)
		{
			return HandleMinuteTick((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
		}

		if (type == EventType.HourTick)
		{
			return HandleHourTick((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Any(x => x is EventType.MinuteTick or EventType.HourTick) || base.HandlesEvent(types);
	}

	private bool HandleMinuteTick(ICharacter worker)
	{
		if (!IsGenerallyAble(worker) || worker.Combat is not null)
		{
			return false;
		}

		if (worker.CurrentProject.Project is not null)
		{
			if (ShouldLeaveCurrentProject(worker))
			{
				worker.CurrentProject.Project.Leave(worker);
				DebugWorker(worker, "left current project because it no longer meets payment requirements.");
				return true;
			}

			CheckPathingEffect(worker, true);
			return ResolvePathTarget(worker) is not null;
		}

		if (!SearchEnabled || worker.AffectedBy<ProjectWorkerSearchCooldownEffect>())
		{
			return false;
		}

		var candidate = FindBestCandidate(worker);
		if (candidate is null)
		{
			AddSearchCooldown(worker);
			DebugWorker(worker, "found no reachable paid project.");
			return false;
		}

		if (!ReferenceEquals(candidate.Project.Location, worker.Location))
		{
			CheckPathingEffect(worker, true);
			DebugWorker(worker,
				$"pathing to project #{candidate.Project.Id:N0} {candidate.Project.Name} for {candidate.HourlyPay:N2}/hour.");
			return true;
		}

		candidate.Project.Join(worker, candidate.Labour);
		DebugWorker(worker,
			$"joined project #{candidate.Project.Id:N0} {candidate.Project.Name} labour {candidate.Labour.Name} for {candidate.HourlyPay:N2}/hour.");
		return true;
	}

	private bool HandleHourTick(ICharacter worker)
	{
		if (!ClaimPayments || !IsGenerallyAble(worker) || worker.Combat is not null)
		{
			return false;
		}

		var result = ClaimProjectPayments(worker, out var message);
		if (result)
		{
			DebugWorker(worker, message.StripANSIColour());
		}

		return result;
	}

	private bool ClaimProjectPayments(ICharacter worker, out string message)
	{
		if (!DepositToBank)
		{
			return ProjectPaymentService.TryClaimOutstanding(worker, null, out message);
		}

		var payables = ProjectPaymentService.OutstandingPayablesFor(worker);
		if (!payables.Any())
		{
			message = "The worker has no outstanding project payments to claim.";
			return false;
		}

		var messages = new List<string>();
		var acted = false;
		foreach (var currencyId in payables.Select(x => x.CurrencyId).Distinct().OrderBy(x => x))
		{
			var currency = Gameworld.Currencies.Get(currencyId);
			if (currency is null)
			{
				continue;
			}

			var account = ResolveDepositAccount(worker, currency);
			if (account is null)
			{
				continue;
			}

			if (ProjectPaymentService.TryClaimOutstanding(worker, account, out var depositMessage))
			{
				acted = true;
				messages.Add(depositMessage);
			}
		}

		if (ProjectPaymentService.OutstandingPayablesFor(worker).Any() &&
		    ProjectPaymentService.TryClaimOutstanding(worker, null, out var cashMessage))
		{
			acted = true;
			messages.Add(cashMessage);
		}

		message = messages.ListToString(separator: "\n", conjunction: "", twoItemJoiner: "\n");
		return acted;
	}

	protected override bool IsPathingEnabled(ICharacter character)
	{
		return ResolvePathTarget(character) is not null;
	}

	protected override (ICell? Target, IEnumerable<ICellExit>) GetPath(ICharacter ch)
	{
		var target = ResolvePathTarget(ch);
		if (target is null || ReferenceEquals(target, ch.Location))
		{
			return (null, Enumerable.Empty<ICellExit>());
		}

		return (target, ch.PathBetween(target, MaxPathRange, GetSuitabilityFunction(ch)).ToList());
	}

	private ICell? ResolvePathTarget(ICharacter worker)
	{
		if (worker.CurrentProject.Project is { } currentProject)
		{
			return currentProject.Location is not null && !ReferenceEquals(currentProject.Location, worker.Location)
				? currentProject.Location
				: null;
		}

		if (!SearchEnabled || worker.AffectedBy<ProjectWorkerSearchCooldownEffect>())
		{
			return null;
		}

		var candidate = FindBestCandidate(worker);
		return candidate?.Project.Location is not null && !ReferenceEquals(candidate.Project.Location, worker.Location)
			? candidate.Project.Location
			: null;
	}

	private bool ShouldLeaveCurrentProject(ICharacter worker)
	{
		var (project, labour) = worker.CurrentProject;
		if (project is null || labour is null)
		{
			return false;
		}

		var rate = project.LabourPaymentRateFor(labour);
		if (rate <= 0.0M)
		{
			return true;
		}

		if (project.PaymentCurrency is null || EffectiveGlobalPay(project.PaymentCurrency, rate) < MinimumGlobalPay())
		{
			return true;
		}

		var tickPay = rate * (decimal)Gameworld.GetStaticDouble("ProjectProgressMultiplier");
		return project.CashBalance < tickPay;
	}

	private ProjectWorkCandidate? FindBestCandidate(ICharacter worker)
	{
		var minimumGlobal = MinimumGlobalPay();
		var tickMultiplier = (decimal)Gameworld.GetStaticDouble("ProjectProgressMultiplier");
		return worker.Gameworld.ActiveProjects
		             .OfType<ILocalProject>()
		             .SelectMany(project => project.CurrentPhase.LabourRequirements.Select(labour => (Project: (IActiveProject)project, Labour: labour)))
		             .Where(x => x.Project.Location is not null)
		             .Where(x => x.Labour.CharacterIsQualified(worker))
		             .Where(x => x.Project.ActiveLabour.Count(y => y.Labour == x.Labour) <
		                         x.Labour.MaximumSimultaneousWorkers)
		             .Select(x => (
			             x.Project,
			             x.Labour,
			             Rate: x.Project.LabourPaymentRateFor(x.Labour),
			             Currency: x.Project.PaymentCurrency))
		             .Where(x => x.Currency is not null && x.Rate > 0.0M)
		             .Where(x => EffectiveGlobalPay(x.Currency!, x.Rate) >= minimumGlobal)
		             .Where(x => x.Project.CashBalance >= x.Rate * tickMultiplier)
		             .Select(x => new ProjectWorkCandidate(
			             x.Project,
			             x.Labour,
			             x.Rate,
			             EffectiveGlobalPay(x.Currency!, x.Rate),
			             CommuteDistance(worker, x.Project.Location)))
		             .Where(x => x.Distance < int.MaxValue)
		             .OrderByDescending(x => x.EffectiveGlobalPay)
		             .ThenBy(x => x.Distance)
		             .ThenBy(x => x.Project.Name)
		             .ThenBy(x => x.Labour.Name)
		             .FirstOrDefault();
	}

	private decimal MinimumGlobalPay()
	{
		return Currency is null
			? MinimumHourlyPay
			: MinimumHourlyPay * Currency.BaseCurrencyToGlobalBaseCurrencyConversion;
	}

	private static decimal EffectiveGlobalPay(ICurrency currency, decimal amount)
	{
		return amount * currency.BaseCurrencyToGlobalBaseCurrencyConversion;
	}

	private int CommuteDistance(ICharacter worker, ICell? cell)
	{
		if (cell is null || ReferenceEquals(worker.Location, cell))
		{
			return 0;
		}

		var path = worker.PathBetween(cell, MaxPathRange, GetSuitabilityFunction(worker))?.ToList() ??
		           new List<ICellExit>();
		return path.Any() ? path.Count : int.MaxValue;
	}

	private void AddSearchCooldown(ICharacter worker)
	{
		if (SearchCadence <= TimeSpan.Zero)
		{
			return;
		}

		worker.AddEffect(new ProjectWorkerSearchCooldownEffect(worker), SearchCadence);
	}

	private IBankAccount? ResolveDepositAccount(ICharacter worker, ICurrency targetCurrency)
	{
		if (!DepositToBank)
		{
			return null;
		}

		var existing = worker.Gameworld.BankAccounts
		                     .Where(x => x.IsAccountOwner(worker))
		                     .Where(x => x.AccountStatus == BankAccountStatus.Active)
		                     .Where(x => x.Currency == targetCurrency)
		                     .OrderBy(x => x.Id)
		                     .FirstOrDefault();
		if (existing is not null)
		{
			return existing;
		}

		if (BankAccountTypeId is not long accountTypeId)
		{
			return null;
		}

		var accountType = Gameworld.BankAccountTypes.Get(accountTypeId);
		if (accountType is null)
		{
			return null;
		}

		if (accountType.Bank.PrimaryCurrency != targetCurrency)
		{
			return null;
		}

		var canOpen = accountType.CanOpenAccount(worker);
		if (!canOpen.Truth)
		{
			DebugWorker(worker,
				$"could not open project payment deposit account type {accountType.Name}: {canOpen.Reason}");
			return null;
		}

		var account = accountType.OpenAccount(worker);
		DebugWorker(worker, $"opened bank account {account.AccountReference} for project payment deposits.");
		return account;
	}

	private ICurrency? DefaultCurrency()
	{
		return Gameworld.Currencies.Get(Gameworld.GetStaticLong("DefaultCurrencyID")) ??
		       Gameworld.Currencies.FirstOrDefault();
	}

	private string DescribeMinimumPay()
	{
		return Currency is null
			? MinimumHourlyPay.ToString("N2")
			: Currency.Describe(MinimumHourlyPay, CurrencyDescriptionPatternType.ShortDecimal);
	}

	private void DebugWorker(ICharacter worker, string message)
	{
		worker.Gameworld?.DebugMessage($"[ProjectWorkerAI:{Name}] {worker.Name} #{worker.Id:N0}: {message}");
	}
}
