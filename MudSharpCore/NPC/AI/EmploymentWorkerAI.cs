using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Models;

#nullable enable

namespace MudSharp.NPC.AI;

public class EmploymentWorkerAI : PathingAIBase
{
	private readonly EmploymentTaskDispatcher _dispatcher = new();
	private HashSet<PaymentMethodKind> _acceptedPaymentMethods = new();
	private HashSet<EmploymentAICapability> _capabilities = new();

	public static void RegisterLoader()
	{
		RegisterAIType("EmploymentWorker", (ai, gameworld) => new EmploymentWorkerAI(ai, gameworld));
		RegisterAIBuilderInformation("employmentworker",
			(gameworld, name) => new EmploymentWorkerAI(gameworld, name),
			new EmploymentWorkerAI().HelpText);
	}

	private EmploymentWorkerAI()
	{
		SetDefaults();
	}

	protected EmploymentWorkerAI(IFuturemud gameworld, string name) : this(gameworld, name, true)
	{
	}

	private EmploymentWorkerAI(IFuturemud gameworld, string name, bool initialiseDatabase) : base(gameworld, name, "EmploymentWorker")
	{
		SetDefaults();
		Currency = DefaultCurrency();
		OpenDoors = true;
		UseKeys = true;
		UseDoorguards = true;
		CloseDoorsBehind = true;
		MoveEvenIfObstructionInWay = true;
		if (initialiseDatabase)
		{
			DatabaseInitialise();
		}
	}

	protected EmploymentWorkerAI(ArtificialIntelligence ai, IFuturemud gameworld) : base(ai, gameworld)
	{
	}

	public decimal ReservationWage { get; private set; }
	public ICurrency? Currency { get; private set; }
	public IReadOnlySet<PaymentMethodKind> AcceptedPaymentMethods => _acceptedPaymentMethods;
	public IReadOnlySet<EmploymentAICapability> Capabilities => _capabilities;
	public EmploymentHostType? HostTypeFilter { get; private set; }
	public uint MaxPathRange { get; private set; }
	public bool SearchEnabled { get; private set; }
	public bool TaskingEnabled { get; private set; }
	public TimeSpan SearchCadence { get; private set; }

	private void SetDefaults()
	{
		ReservationWage = 0.0M;
		_acceptedPaymentMethods = new HashSet<PaymentMethodKind> { PaymentMethodKind.Cash };
		_capabilities = new HashSet<EmploymentAICapability> { EmploymentAICapability.CanDeliverItems };
		HostTypeFilter = null;
		MaxPathRange = 50;
		SearchEnabled = true;
		TaskingEnabled = true;
		SearchCadence = TimeSpan.FromMinutes(10);
	}

	protected override void LoadFromXML(XElement root)
	{
		SetDefaults();
		base.LoadFromXML(root);

		var currencies = Gameworld.Currencies;
		Currency = currencies?.Get(long.Parse(root.Element("Currency")?.Value ?? "0")) ?? DefaultCurrency();
		ReservationWage = decimal.TryParse(root.Element("ReservationWage")?.Value, out var wage)
			? Math.Max(0.0M, wage)
			: 0.0M;
		MaxPathRange = uint.TryParse(root.Element("MaxPathRange")?.Value, out var range) && range > 0
			? range
			: 50;
		SearchEnabled = bool.TryParse(root.Element("SearchEnabled")?.Value, out var searchEnabled)
			? searchEnabled
			: true;
		TaskingEnabled = bool.TryParse(root.Element("TaskingEnabled")?.Value, out var taskingEnabled)
			? taskingEnabled
			: true;
		SearchCadence = double.TryParse(root.Element("SearchCadenceMinutes")?.Value, out var cadence) && cadence > 0.0
			? TimeSpan.FromMinutes(cadence)
			: TimeSpan.FromMinutes(10);

		var hostTypeText = root.Element("HostTypeFilter")?.Value;
		HostTypeFilter = !string.IsNullOrWhiteSpace(hostTypeText) &&
		                 !hostTypeText.EqualTo("any") &&
		                 TryParseHostType(hostTypeText, out var hostType)
			? hostType
			: null;

		_acceptedPaymentMethods = new HashSet<PaymentMethodKind>();
		foreach (var element in root.Element("AcceptedPaymentMethods")?.Elements("Method") ?? [])
		{
			if (element.Value.TryParseEnum(out PaymentMethodKind method))
			{
				_acceptedPaymentMethods.Add(method);
			}
		}

		if (!_acceptedPaymentMethods.Any())
		{
			_acceptedPaymentMethods.Add(PaymentMethodKind.Cash);
		}

		_capabilities = new HashSet<EmploymentAICapability>();
		foreach (var element in root.Element("Capabilities")?.Elements("Capability") ?? [])
		{
			if (element.Value.TryParseEnum(out EmploymentAICapability capability))
			{
				_capabilities.Add(capability);
			}
		}

		if (!_capabilities.Any())
		{
			_capabilities.Add(EmploymentAICapability.CanDeliverItems);
		}
	}

	protected override string SaveToXml()
	{
		return new XElement("Definition",
			new XElement("Currency", Currency?.Id ?? 0L),
			new XElement("ReservationWage", ReservationWage),
			new XElement("MaxPathRange", MaxPathRange),
			new XElement("SearchEnabled", SearchEnabled),
			new XElement("TaskingEnabled", TaskingEnabled),
			new XElement("SearchCadenceMinutes", SearchCadence.TotalMinutes),
			new XElement("HostTypeFilter", HostTypeFilter?.ToString() ?? "Any"),
			new XElement("AcceptedPaymentMethods",
				_acceptedPaymentMethods.Select(x => new XElement("Method", x))),
			new XElement("Capabilities",
				_capabilities.Select(x => new XElement("Capability", x))),
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
		var sb = new StringBuilder(base.Show(actor));
		sb.AppendLine();
		sb.AppendLine($"Currency: {Currency?.Name.ColourName() ?? "None".ColourError()}");
		sb.AppendLine($"Reservation Wage: {DescribeReservationWage().ColourValue()}");
		sb.AppendLine($"Accepted Payment: {_acceptedPaymentMethods.Select(x => x.DescribeEnum().ColourName()).ListToString()}");
		sb.AppendLine($"Capabilities: {_capabilities.Select(x => x.DescribeEnum().ColourName()).ListToString()}");
		sb.AppendLine($"Host Filter: {(HostTypeFilter?.DescribeEnum().ColourName() ?? "any".ColourValue())}");
		sb.AppendLine($"Max Path Range: {MaxPathRange.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Search Enabled: {SearchEnabled.ToColouredString()}");
		sb.AppendLine($"Tasking Enabled: {TaskingEnabled.ToColouredString()}");
		sb.AppendLine($"Search Cadence: {SearchCadence.Describe(actor).ColourValue()}");
		return sb.ToString();
	}

	protected override string TypeHelpText => $@"{base.TypeHelpText}
	#3currency <currency>#0 - sets the currency used to parse and display reservation wages
	#3wage <amount>#0 - sets the minimum nominal pay this worker will accept
	#3payment <method>#0 - toggles an accepted payment method
	#3capability <capability>#0 - toggles an AI task capability
	#3host <any|type>#0 - restricts this worker to a host type
	#3range <exits>#0 - sets maximum job/task path range
	#3search#0 - toggles autonomous job searching
	#3tasking#0 - toggles autonomous task claiming/execution
	#3cadence <minutes>#0 - sets job-search scan cadence";

	public override bool BuildingCommand(ICharacter actor, StringStack command)
	{
		switch (command.PopForSwitch())
		{
			case "currency":
				return BuildingCommandCurrency(actor, command);
			case "wage":
			case "reservation":
			case "reservationwage":
				return BuildingCommandReservationWage(actor, command);
			case "payment":
			case "pay":
			case "method":
				return BuildingCommandPayment(actor, command);
			case "capability":
			case "cap":
				return BuildingCommandCapability(actor, command);
			case "host":
			case "hosttype":
				return BuildingCommandHostType(actor, command);
			case "range":
			case "path":
			case "pathrange":
				return BuildingCommandRange(actor, command);
			case "search":
				SearchEnabled = !SearchEnabled;
				Changed = true;
				actor.OutputHandler.Send($"This AI will {SearchEnabled.NowNoLonger()} search for employment openings.");
				return true;
			case "tasking":
			case "tasks":
				TaskingEnabled = !TaskingEnabled;
				Changed = true;
				actor.OutputHandler.Send($"This AI will {TaskingEnabled.NowNoLonger()} claim and execute employment tasks.");
				return true;
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
			actor.OutputHandler.Send($"Which currency should this employment worker use for reservation wages? The valid currencies are {Gameworld.Currencies.Select(x => x.Name.ColourName()).ListToString()}.");
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
		actor.OutputHandler.Send($"This AI will now parse and display reservation wages in {currency.Name.ColourName()}.");
		return true;
	}

	private bool BuildingCommandReservationWage(ICharacter actor, StringStack command)
	{
		if (Currency is null)
		{
			actor.OutputHandler.Send("You must first set a currency for this AI's reservation wage.");
			return false;
		}

		if (command.IsFinished || !Currency.TryGetBaseCurrency(command.SafeRemainingArgument, out var wage) || wage < 0.0M)
		{
			actor.OutputHandler.Send($"What non-negative reservation wage in {Currency.Name.ColourName()} should this worker require?");
			return false;
		}

		ReservationWage = wage;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now require at least {DescribeReservationWage().ColourValue()}.");
		return true;
	}

	private bool BuildingCommandPayment(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out PaymentMethodKind method))
		{
			actor.OutputHandler.Send($"Which payment method should be toggled? The valid values are {Enum.GetValues<PaymentMethodKind>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (_acceptedPaymentMethods.Contains(method))
		{
			if (_acceptedPaymentMethods.Count == 1)
			{
				actor.OutputHandler.Send("This AI must accept at least one payment method.");
				return false;
			}

			_acceptedPaymentMethods.Remove(method);
			Changed = true;
			actor.OutputHandler.Send($"This AI will no longer accept {method.DescribeEnum().ColourName()} payment.");
			return true;
		}

		_acceptedPaymentMethods.Add(method);
		Changed = true;
		actor.OutputHandler.Send($"This AI will now accept {method.DescribeEnum().ColourName()} payment.");
		return true;
	}

	private bool BuildingCommandCapability(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !command.SafeRemainingArgument.TryParseEnum(out EmploymentAICapability capability))
		{
			actor.OutputHandler.Send($"Which AI capability should be toggled? The valid values are {Enum.GetValues<EmploymentAICapability>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		if (_capabilities.Contains(capability))
		{
			if (_capabilities.Count == 1)
			{
				actor.OutputHandler.Send("This AI must have at least one employment task capability.");
				return false;
			}

			_capabilities.Remove(capability);
			Changed = true;
			actor.OutputHandler.Send($"This AI no longer has the {capability.DescribeEnum().ColourName()} capability.");
			return true;
		}

		_capabilities.Add(capability);
		Changed = true;
		actor.OutputHandler.Send($"This AI now has the {capability.DescribeEnum().ColourName()} capability.");
		return true;
	}

	private bool BuildingCommandHostType(ICharacter actor, StringStack command)
	{
		if (command.IsFinished)
		{
			actor.OutputHandler.Send("Which host type should this worker search for? Use any to clear the filter.");
			return false;
		}

		var text = command.SafeRemainingArgument;
		if (text.EqualTo("any") || text.EqualTo("all"))
		{
			HostTypeFilter = null;
			Changed = true;
			actor.OutputHandler.Send("This AI will now consider any employment host type.");
			return true;
		}

		if (!TryParseHostType(text, out var hostType))
		{
			actor.OutputHandler.Send($"That is not a valid employment host type. The valid values are {Enum.GetValues<EmploymentHostType>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return false;
		}

		HostTypeFilter = hostType;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now only consider {hostType.DescribeEnum().ColourName()} employment hosts.");
		return true;
	}

	private bool BuildingCommandRange(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !uint.TryParse(command.SafeRemainingArgument, out var range) || range == 0)
		{
			actor.OutputHandler.Send("What positive maximum path range should this worker use?");
			return false;
		}

		MaxPathRange = range;
		Changed = true;
		actor.OutputHandler.Send($"This AI will now search and task within {MaxPathRange.ToString("N0", actor).ColourValue()} exits.");
		return true;
	}

	private bool BuildingCommandCadence(ICharacter actor, StringStack command)
	{
		if (command.IsFinished || !double.TryParse(command.SafeRemainingArgument, actor, out var minutes) || minutes <= 0.0)
		{
			actor.OutputHandler.Send("How many positive minutes should this worker wait between job searches?");
			return false;
		}

		SearchCadence = TimeSpan.FromMinutes(minutes);
		Changed = true;
		actor.OutputHandler.Send($"This AI will now scan for employment every {SearchCadence.Describe(actor).ColourValue()}.");
		return true;
	}

	public override bool HandleEvent(EventType type, params dynamic[] arguments)
	{
		if (type == EventType.MinuteTick)
		{
			return HandleMinuteTick((ICharacter)arguments[0]) || base.HandleEvent(type, arguments);
		}

		return base.HandleEvent(type, arguments);
	}

	public override bool HandlesEvent(params EventType[] types)
	{
		return types.Any(x => x == EventType.MinuteTick) || base.HandlesEvent(types);
	}

	internal bool HandleMinuteTick(ICharacter character)
	{
		if (!IsGenerallyAble(character) || character.Combat is not null)
		{
			return false;
		}

		var host = ActiveEmploymentHost(character);
		if (host is null)
		{
			return SearchEnabled && TryApplyForEmployment(character);
		}

		if (TaskingEnabled && TryClaimOrAdvanceTask(character, host))
		{
			return true;
		}

		CheckPathingEffect(character, true);
		return ResolvePathTarget(character) is not null;
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

	private bool TryApplyForEmployment(ICharacter candidate)
	{
		if (candidate.AffectedBy<EmploymentWorkerSearchCooldownEffect>() ||
		    HasPendingEmploymentApplication(candidate))
		{
			return false;
		}

		var profile = BuildCandidateProfile(candidate);
		var opening = EmploymentHosts(candidate.Gameworld)
		              .Where(HostMatchesFilter)
		              .SelectMany(host => host.JobOpenings.Select(job => (Host: host, Opening: job)))
		              .Where(x => x.Opening.Status == JobOpeningStatus.Open && x.Opening.AcceptsApplications)
		              .Where(x => !HasCurrentApplication(candidate, x.Host, x.Opening))
		              .Where(x => !HasRecentRejectedApplication(candidate, x.Host, x.Opening))
		              .Where(x => EmploymentCandidateMatcher.IsMatch(x.Opening, profile, out _))
		              .Where(x => CanReach(candidate, PrimaryWorkCell(x.Host)))
		              .OrderByDescending(x => x.Opening.Compensation.NominalAmount)
		              .ThenBy(x => x.Host.EmploymentHostName)
		              .FirstOrDefault();

		AddSearchCooldown(candidate);
		if (opening.Host is null)
		{
			return false;
		}

		opening.Host.Employment.Apply(opening.Opening, profile);
		return true;
	}

	private bool TryClaimOrAdvanceTask(ICharacter worker, IEmploymentHost host)
	{
		var task = AssignedTaskFor(worker, host);
		var context = task is null ? null : ContextFor(worker, host, task);
		if (task is null)
		{
			var profile = BuildCandidateProfile(worker);
			foreach (var pending in host.TaskBoard.ActiveTasks
			                            .Where(x => x.Status == EmploymentTaskStatus.Pending)
			                            .OrderBy(x => x.Name))
			{
				context = ContextFor(worker, host, pending);
				if (!_dispatcher.TryAssignTask(pending, [profile], context, out _,
					    blockWhenNoCandidateMatches: false))
				{
					RemoveTaskContext(worker, host, pending);
					continue;
				}

				task = pending;
				break;
			}
		}

		if (task is null || context is null)
		{
			return false;
		}

		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			RemoveTaskContext(worker, host, task);
			return false;
		}

		var hintedLocation = NextStepLocation(task, context, worker);
		if (hintedLocation is not null && !ReferenceEquals(hintedLocation, worker.Location))
		{
			CheckPathingEffect(worker, true);
			return false;
		}

		var result = _dispatcher.AdvanceTask(task, context);
		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			RemoveTaskContext(worker, host, task);
		}

		return result.Success || !result.Completed;
	}

	private IEmploymentHost? ActiveEmploymentHost(ICharacter worker)
	{
		return EmploymentHosts(worker.Gameworld)
		       .Where(HostMatchesFilter)
		       .Where(host => host.EmploymentContracts.Any(x =>
			       x.Employee.Id == worker.Id &&
			       x.Status == EmploymentStatus.Active))
		       .OrderByDescending(host => host.TaskBoard.ActiveTasks.Any(x =>
			       x.AssignedEmployee?.Id == worker.Id &&
			       x.Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked))
		       .ThenBy(host => host.EmploymentHostName)
		       .FirstOrDefault();
	}

	private EmploymentCandidateProfile BuildCandidateProfile(ICharacter candidate)
	{
		return new EmploymentCandidateProfile(
			candidate,
			ReservationWage,
			new Dictionary<string, double>(),
			new HashSet<string>(),
			new HashSet<EmploymentAICapability>(_capabilities),
			new HashSet<string>(),
			_acceptedPaymentMethods.ToList(),
			Currency);
	}

	private ICurrency? DefaultCurrency()
	{
		var currencies = Gameworld.Currencies;
		return currencies?.Get(Gameworld.GetStaticLong("DefaultCurrencyID")) ??
		       currencies?.FirstOrDefault();
	}

	private string DescribeReservationWage()
	{
		return Currency is null
			? ReservationWage.ToString("N2")
			: Currency.Describe(ReservationWage, CurrencyDescriptionPatternType.ShortDecimal);
	}

	private IEmploymentActiveTask? AssignedTaskFor(ICharacter worker, IEmploymentHost host)
	{
		return host.TaskBoard.ActiveTasks
		           .Where(x => x.AssignedEmployee?.Id == worker.Id)
		           .Where(x => x.Status is EmploymentTaskStatus.Assigned or EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked)
		           .OrderBy(x => x.Name)
		           .FirstOrDefault();
	}

	private EmploymentTaskContext ContextFor(ICharacter worker, IEmploymentHost host, IEmploymentActiveTask task)
	{
		var existing = worker.EffectsOfType<EmploymentWorkerTaskContextEffect>()
		                     .FirstOrDefault(x => x.Matches(host, task));
		if (existing is not null)
		{
			return existing.Context;
		}

		var context = new EmploymentTaskContext(host, usePhysicalItemMovement: true);
		worker.AddEffect(new EmploymentWorkerTaskContextEffect(worker, host, task, context));
		return context;
	}

	private static void RemoveTaskContext(ICharacter worker, IEmploymentHost host, IEmploymentActiveTask task)
	{
		worker.RemoveAllEffects<EmploymentWorkerTaskContextEffect>(x => x.Matches(host, task), true);
	}

	private ICell? ResolvePathTarget(ICharacter worker)
	{
		var host = ActiveEmploymentHost(worker);
		if (host is null)
		{
			return null;
		}

		var task = AssignedTaskFor(worker, host);
		if (task is not null)
		{
			var context = ContextFor(worker, host, task);
			var target = NextStepLocation(task, context, worker);
			if (target is not null && !ReferenceEquals(target, worker.Location))
			{
				return target;
			}
		}

		var workplace = PrimaryWorkCell(host);
		return workplace is not null && !ReferenceEquals(workplace, worker.Location) ? workplace : null;
	}

	private ICell? NextStepLocation(IEmploymentActiveTask task, IEmploymentTaskContext context, ICharacter worker)
	{
		if (task is not EmploymentActiveTask concrete)
		{
			return null;
		}

		var index = concrete.NextStepIndex;
		if (index < 0 || index >= task.ActionPlan.Steps.Count)
		{
			return null;
		}

		if (task.ActionPlan.Steps[index] is not IEmploymentActionStepLocationHint hint)
		{
			return null;
		}

		return hint.ExecutionLocationHints(context, worker)
		           .Where(x => x is not null)
		           .Where(x => ReferenceEquals(x, worker.Location) || CanReach(worker, x))
		           .OrderBy(x => ReferenceEquals(x, worker.Location) ? 0 : 1)
		           .ThenBy(x => x.Id)
		           .FirstOrDefault();
	}

	private bool HasPendingEmploymentApplication(ICharacter candidate)
	{
		return EmploymentHosts(candidate.Gameworld).Any(host =>
			host.Employment.Applications.Any(x =>
				x.Candidate.Id == candidate.Id &&
				x.Status == JobApplicationStatus.Pending));
	}

	private bool HasCurrentApplication(ICharacter candidate, IEmploymentHost host, IJobOpening opening)
	{
		return host.Employment.Applications.Any(x =>
			x.Candidate.Id == candidate.Id &&
			x.Opening.Id == opening.Id &&
			x.Status is JobApplicationStatus.Pending or JobApplicationStatus.Accepted);
	}

	private bool HasRecentRejectedApplication(ICharacter candidate, IEmploymentHost host, IJobOpening opening)
	{
		var rejected = host.Employment.Applications
		                   .Where(x => x.Candidate.Id == candidate.Id)
		                   .Where(x => x.Opening.Id == opening.Id)
		                   .Where(x => x.Status == JobApplicationStatus.Rejected)
		                   .OrderByDescending(x => x.AppliedAt)
		                   .FirstOrDefault();
		if (rejected is null)
		{
			return false;
		}

		var memoryWindow = SearchCadence * 6.0;
		var age = DateTimeOffset.UtcNow - rejected.AppliedAt;
		if (age >= memoryWindow)
		{
			return false;
		}

		var existing = candidate.EffectsOfType<EmploymentWorkerRejectedOpeningEffect>()
		                        .Any(x => x.Matches(host, opening));
		if (!existing)
		{
			candidate.AddEffect(new EmploymentWorkerRejectedOpeningEffect(candidate, host, opening), memoryWindow - age);
		}

		return true;
	}

	private void AddSearchCooldown(ICharacter candidate)
	{
		if (SearchCadence <= TimeSpan.Zero)
		{
			return;
		}

		candidate.AddEffect(new EmploymentWorkerSearchCooldownEffect(candidate), SearchCadence);
	}

	private bool HostMatchesFilter(IEmploymentHost host)
	{
		return HostTypeFilter is null || host.EmploymentHostType == HostTypeFilter.Value;
	}

	private bool CanReach(ICharacter worker, ICell? cell)
	{
		return cell is null ||
		       ReferenceEquals(worker.Location, cell) ||
		       worker.PathBetween(cell, MaxPathRange, GetSuitabilityFunction(worker)).Any();
	}

	private static ICell? PrimaryWorkCell(IEmploymentHost host)
	{
		return host switch
		{
			IPermanentShop shop => shop.ShopfrontCells.FirstOrDefault() ?? shop.StockroomCell ?? shop.WorkshopCell,
			IShop shop => shop.CurrentLocations.FirstOrDefault(),
			IAuctionHouse auctionHouse => auctionHouse.AuctionHouseCell,
			ICombatArena arena => arena.WaitingCells.FirstOrDefault() ??
			                      arena.ArenaCells.FirstOrDefault() ??
			                      arena.ObservationCells.FirstOrDefault(),
			IBank bank => bank.BranchLocations.FirstOrDefault(),
			IStable stable => stable.Location,
			IHotel hotel => hotel.Locations.FirstOrDefault(),
			_ => null
		};
	}

	private static IEnumerable<IEmploymentHost> EmploymentHosts(IFuturemud gameworld)
	{
		foreach (var shop in gameworld.Shops ?? Enumerable.Empty<IShop>())
		{
			yield return shop;
		}

		foreach (var auction in gameworld.AuctionHouses ?? Enumerable.Empty<IAuctionHouse>())
		{
			yield return auction;
		}

		foreach (var arena in gameworld.CombatArenas ?? Enumerable.Empty<ICombatArena>())
		{
			yield return arena;
		}

		foreach (var bank in gameworld.Banks ?? Enumerable.Empty<IBank>())
		{
			yield return bank;
		}

		foreach (var stable in gameworld.Stables ?? Enumerable.Empty<IStable>())
		{
			yield return stable;
		}

		foreach (var property in gameworld.Properties ?? Enumerable.Empty<MudSharp.Economy.Property.IProperty>())
		{
			if (property.Hotel is not null)
			{
				yield return property.Hotel;
			}
		}
	}

	private static bool TryParseHostType(string text, out EmploymentHostType hostType)
	{
		switch (text.CollapseString().ToLowerInvariant())
		{
			case "auction":
			case "auctionhouse":
				hostType = EmploymentHostType.AuctionHouse;
				return true;
			case "shop":
			case "store":
				hostType = EmploymentHostType.Shop;
				return true;
			case "arena":
			case "combatarena":
				hostType = EmploymentHostType.Arena;
				return true;
		}

		return text.TryParseEnum(out hostType);
	}
}
