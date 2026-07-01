using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Community;
using MudSharp.Community.Boards;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.NPC;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp.Commands.Helpers;

internal interface IEmploymentHostResolver
{
	IEmploymentHost? Resolve(IFuturemud gameworld, string hostType, string identifier, out string error);
}

internal sealed class EmploymentHostResolver : IEmploymentHostResolver
{
	public IEmploymentHost? Resolve(IFuturemud gameworld, string hostType, string identifier, out string error)
	{
		if (string.IsNullOrWhiteSpace(hostType))
		{
			error = "Which kind of employment host do you want to inspect?";
			return null;
		}

		if (string.IsNullOrWhiteSpace(identifier))
		{
			error = "Which employment host do you want to inspect?";
			return null;
		}

		var hostTypeKey = hostType.CollapseString().ToLowerInvariant();
		if (hostTypeKey is "clan" or "organisation" or "organization")
		{
			var clan = gameworld.Clans.GetByIdOrName(identifier);
			if (clan?.IsTemplate == true)
			{
				error = $"Clan templates cannot be used as employment hosts: {clan.FullName.ColourName()}.";
				return null;
			}

			if (clan is not null)
			{
				error = string.Empty;
				return clan;
			}
		}

		var host = hostTypeKey switch
		{
			"shop" or "store" => (IEmploymentHost?)gameworld.Shops.GetByIdOrName(identifier),
			"auction" or "auctionhouse" => (IEmploymentHost?)gameworld.AuctionHouses.GetByIdOrName(identifier),
			"arena" or "combatarena" => (IEmploymentHost?)gameworld.CombatArenas.GetByIdOrName(identifier),
			"bank" => (IEmploymentHost?)gameworld.Banks.GetByIdOrName(identifier),
			"stable" => (IEmploymentHost?)gameworld.Stables.GetByIdOrName(identifier),
			"hotel" => gameworld.Properties.GetByIdOrName(identifier)?.Hotel,
			_ => null
		};

		if (host is not null)
		{
			error = string.Empty;
			return host;
		}

		error = $"There is no {hostType.ColourCommand()} employment host matching {identifier.ColourCommand()}.";
		return null;
	}
}

internal sealed class EmploymentCommandService
{
	private const EmploymentAuthority ManagerAliasHelpAuthorities =
		EmploymentAuthority.ViewEmployees |
		EmploymentAuthority.HireEmployees |
		EmploymentAuthority.FireEmployees |
		EmploymentAuthority.CreateJobOpenings |
		EmploymentAuthority.ModifyJobOpenings |
		EmploymentAuthority.SetPayWithinBand |
		EmploymentAuthority.AssignTasks |
		EmploymentAuthority.CancelTasks |
		EmploymentAuthority.CreateScheduledRules |
		EmploymentAuthority.ModifyScheduledRules |
		EmploymentAuthority.CreateManagerGoals |
		EmploymentAuthority.ModifyManagerGoals |
		EmploymentAuthority.ApprovePurchases |
		EmploymentAuthority.UseStoreAccount |
		EmploymentAuthority.WithdrawBusinessCash |
		EmploymentAuthority.DepositBusinessCash |
		EmploymentAuthority.ManageStockRules |
		EmploymentAuthority.ManageCraftRules |
		EmploymentAuthority.AdjustPrices |
		EmploymentAuthority.PayTaxes |
		EmploymentAuthority.ModerateHostBoard |
		EmploymentAuthority.ManagePayroll;

	private readonly IEmploymentHostResolver _resolver;
	private readonly EmploymentTaskAuthoringService _taskAuthoring;
	private readonly EmploymentScheduledRuleAuthoringService _scheduledRuleAuthoring;
	private readonly EmploymentManagerGoalAuthoringService _managerGoalAuthoring;

	public EmploymentCommandService(IEmploymentHostResolver? resolver = null,
		EmploymentTaskAuthoringService? taskAuthoring = null,
		EmploymentScheduledRuleAuthoringService? scheduledRuleAuthoring = null,
		EmploymentManagerGoalAuthoringService? managerGoalAuthoring = null)
	{
		_resolver = resolver ?? new EmploymentHostResolver();
		_taskAuthoring = taskAuthoring ?? new EmploymentTaskAuthoringService();
		_scheduledRuleAuthoring = scheduledRuleAuthoring ?? new EmploymentScheduledRuleAuthoringService(_taskAuthoring);
		_managerGoalAuthoring = managerGoalAuthoring ?? new EmploymentManagerGoalAuthoringService(_taskAuthoring, _scheduledRuleAuthoring);
	}

	public IEmploymentHost? ResolveHost(IFuturemud gameworld, string hostType, string identifier, out string error)
	{
		return _resolver.Resolve(gameworld, hostType, identifier, out error);
	}

	public void Execute(ICharacter actor, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
			return;
		}

		var hostType = input.PopSpeech();
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which employment host do you want to inspect?");
			return;
		}

		var hostIdentifier = input.PopSpeech();
		var subcommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(subcommand))
		{
			subcommand = "status";
		}

		var host = ResolveHost(actor.Gameworld, hostType, hostIdentifier, out var error);
		if (host is null)
		{
			actor.OutputHandler.Send(error);
			return;
		}

		ExecuteForHost(actor, host, subcommand, input);
	}

	public void ExecuteForHost(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var subcommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		if (string.IsNullOrWhiteSpace(subcommand))
		{
			subcommand = "status";
		}

		ExecuteForHost(actor, host, subcommand, input);
	}

	public void ExecuteForHost(ICharacter actor, IEmploymentHost host, string subcommand, StringStack input)
	{
		switch (subcommand)
		{
			case "status":
				SendOperationalView(actor, host, RenderStatus);
				return;
			case "contracts":
			case "contract":
			case "employees":
			case "staff":
				HandleContracts(actor, host, input);
				return;
			case "delegations":
			case "delegation":
			case "delegate":
			case "authority":
			case "authorities":
				HandleContractAuthority(actor, host, input);
				return;
			case "openings":
			case "opening":
			case "jobs":
				HandleOpenings(actor, host, input);
				return;
			case "applications":
			case "application":
			case "apps":
				HandleApplications(actor, host, input);
				return;
			case "payroll":
			case "payables":
			case "wages":
				HandlePayroll(actor, host, input);
				return;
			case "tasks":
			case "task":
				HandleTasks(actor, host, input);
				return;
			case "conditions":
			case "condition":
			case "conditioncatalog":
				actor.OutputHandler.Send(_scheduledRuleAuthoring.RenderAvailableConditions(actor, input.SafeRemainingArgument));
				return;
			case "rules":
			case "rule":
			case "scheduled":
			case "schedule":
				HandleRules(actor, host, input);
				return;
			case "goals":
			case "goal":
				HandleGoals(actor, host, input);
				return;
			case "register":
			case "audit":
				SendOperationalView(actor, host, RenderRegister);
				return;
			case "employmentledger":
			case "empledger":
				SendOperationalView(actor, host, RenderLedger);
				return;
			case "board":
				HandleBoard(actor, host, input);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	public static bool IsEmploymentShortcut(string subcommand)
	{
		return subcommand.CollapseString().ToLowerInvariant() switch
		{
			"status" or
			"contracts" or "contract" or "employees" or "staff" or
			"delegations" or "delegation" or "delegate" or "authority" or "authorities" or
			"openings" or "opening" or "jobs" or
			"applications" or "application" or "apps" or
			"payroll" or "payables" or "wages" or
			"tasks" or "task" or
			"conditions" or "condition" or "conditioncatalog" or
			"rules" or "rule" or "scheduled" or "schedule" or
			"goals" or "goal" or
			"register" or "audit" or
			"employmentledger" or "empledger" or
			"board" => true,
			_ => false
		};
	}

	public bool TryExecuteShortcut(ICharacter actor, IEmploymentHost? host, string hostDescription,
		string subcommand, StringStack input)
	{
		if (!IsEmploymentShortcut(subcommand))
		{
			return false;
		}

		if (host is null)
		{
			actor.OutputHandler.Send($"You are not currently at any {hostDescription}.");
			return true;
		}

		ExecuteForHost(actor, host, subcommand.CollapseString().ToLowerInvariant(), input);
		return true;
	}

	public bool CanViewOperational(ICharacter actor, IEmploymentHost host)
	{
		return actor.IsAdministrator() || ActiveContractFor(actor, host) is not null;
	}

	public static bool CanViewManagerAliasHelp(ICharacter actor, IEmploymentHost? host, bool subsystemManagerAccess = false)
	{
		if (actor.IsAdministrator() || subsystemManagerAccess)
		{
			return true;
		}

		if (host is null)
		{
			return false;
		}

		var contract = ActiveContractFor(actor, host);
		if (contract?.Role is EmploymentRole.Manager or EmploymentRole.Proprietor)
		{
			return true;
		}

		return Enum.GetValues<EmploymentAuthority>()
		           .Where(x => x != EmploymentAuthority.None && ManagerAliasHelpAuthorities.HasFlag(x))
		           .Any(x => host.HasAuthority(actor, x));
	}

	public bool CanViewOpenings(ICharacter actor, IEmploymentHost host)
	{
		return CanViewOperational(actor, host) || host.JobOpenings.Any(x => x.Status == JobOpeningStatus.Open);
	}

	public bool CanViewPayroll(ICharacter actor, IEmploymentHost host)
	{
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return CanViewOperational(actor, host) || host.Payroll.Payables.Any(x => x.EmployeeId == actorIdentityId);
	}

	public bool CanViewAllPayroll(ICharacter actor, IEmploymentHost host)
	{
		return actor.IsAdministrator() ||
		       host.HasAuthority(actor, EmploymentAuthority.ViewEmployees) ||
		       host.HasAuthority(actor, EmploymentAuthority.ManagePayroll);
	}

	public bool CanPostToBoard(ICharacter actor, IEmploymentHost host)
	{
		return host.HasAuthority(actor, EmploymentAuthority.PostToHostBoard);
	}

	public bool TryCreateOpening(ICharacter actor, IEmploymentHost host, EmploymentRole role, decimal hourlyRate,
		int maxPositions, out IJobOpening? opening, out string message)
	{
		opening = null;
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.CreateJobOpenings, out message))
		{
			return false;
		}

		if (hourlyRate <= 0.0M)
		{
			message = "Job openings must have a positive hourly rate.";
			return false;
		}

		if (maxPositions <= 0)
		{
			message = "Job openings must have at least one position.";
			return false;
		}

		var currency = ResolveHostCurrency(host);
		if (currency is null)
		{
			message = $"Could not determine the currency for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		var pay = new CompensationTerms(
			new MoneyAmount(currency, hourlyRate),
			null,
			PayCadence.Hourly,
			new MoneyAmount(currency, hourlyRate),
			PaymentSource.HostCash);
		try
		{
			opening = host.Employment.CreateJobOpening(new JobOpeningDefinition(
				role,
				JobRequirementSet.None,
				pay,
				WorkSchedule.AnyTime,
				EmploymentDuration.Indefinite,
				maxPositions,
				true,
				new PaymentMethod(PaymentMethodKind.Cash),
				DefaultOpeningAuthority(role)), actor);
			message = $"You create a {role.DescribeEnum().ColourName()} employment opening for {host.EmploymentHostName.ColourName()} at {DescribeMoney(pay.FixedRate, actor)} hourly.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryPostBoardPost(ICharacter actor, IEmploymentHost host, string title, string text, out string message)
	{
		if (!CanPostToBoard(actor, host))
		{
			message = $"You are not authorised to post official notices for {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		title = title.Trim().TitleCase();
		if (string.IsNullOrWhiteSpace(title))
		{
			message = "What title do you want to give to your employment board post?";
			return false;
		}

		if (title.Length > 200)
		{
			message = "Employment board post titles must be under 200 characters in length.";
			return false;
		}

		if (text.Length > 5000)
		{
			message = "Employment board posts must be under 5000 characters in length.";
			return false;
		}

		host.Board.MakeNewPost(actor, title, text);
		host.EmploymentRegister.Record(EmploymentRegisterEntryType.BoardPostCreated, actor,
			$"Posted to host board: {title}.");
		message = $"You successfully make your employment board post {title.ColourName()} for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryAcceptApplication(ICharacter actor, IEmploymentHost host, long applicationId,
		out IEmploymentContract? contract, out string message)
	{
		contract = null;
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.HireEmployees, out message))
		{
			return false;
		}

		var application = ApplicationById(host, applicationId);
		if (application is null)
		{
			message = "There is no such employment application.";
			return false;
		}

		try
		{
			contract = host.Employment.AcceptApplication(application, actor);
			message = $"You accept {application.Candidate.HowSeen(actor, colour: false).ColourName()}'s application and create an active {contract.Role.DescribeEnum().ColourName()} contract.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryRejectApplication(ICharacter actor, IEmploymentHost host, long applicationId, string reason,
		out string message)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.HireEmployees, out message))
		{
			return false;
		}

		var application = ApplicationById(host, applicationId);
		if (application is null)
		{
			message = "There is no such employment application.";
			return false;
		}

		try
		{
			host.Employment.RejectApplication(application, actor, reason);
			message = $"You reject {application.Candidate.HowSeen(actor, colour: false).ColourName()}'s application.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryTerminateContract(ICharacter actor, IEmploymentHost host, long contractId, out string message)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.FireEmployees, out message))
		{
			return false;
		}

		var contract = host.EmploymentContracts.FirstOrDefault(x => x.Id == contractId);
		if (contract is null)
		{
			message = "There is no such employment contract.";
			return false;
		}

		if (contract.Status == EmploymentStatus.Ended)
		{
			message = "That employment contract has already ended.";
			return false;
		}

		try
		{
			host.Fire(contract, EmploymentTerminationReason.Fired, actor);
			message = $"You terminate {contract.Employee.HowSeen(actor, colour: false).ColourName()}'s {contract.Role.DescribeEnum().ColourName()} contract for {host.EmploymentHostName.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryRunPayroll(ICharacter actor, IEmploymentHost host, out IReadOnlyCollection<IEmploymentPayable> created,
		out string message)
	{
		created = Array.Empty<IEmploymentPayable>();
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.ManagePayroll, out message))
		{
			return false;
		}

		created = host.Payroll.EvaluatePayroll();
		message = created.Any()
			? $"Payroll evaluation accrued {created.Count.ToString("N0", actor).ColourValue()} payable{(created.Count == 1 ? string.Empty : "s")} for {host.EmploymentHostName.ColourName()}."
			: $"Payroll evaluation found no new payable wages for {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TrySettlePayroll(ICharacter actor, IEmploymentHost host, string selector, string reason,
		out string message)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.ManagePayroll, out message))
		{
			return false;
		}

		host.Payroll.EvaluatePayroll();
		var outstanding = host.Payroll.OutstandingLiabilities
		                      .OrderBy(x => x.DueAt)
		                      .ThenBy(x => x.Id)
		                      .ToList();
		var targets = SelectPayables(outstanding, selector);
		if (!targets.Any())
		{
			message = $"There is no outstanding employment payable matching {selector.ColourCommand()}.";
			return false;
		}

		return host.Payroll.TrySettlePayables(targets, actor, true, reason, out message);
	}

	public bool TryClaimPayroll(ICharacter actor, IEmploymentHost host, string selector, out string message)
	{
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		host.Payroll.EvaluatePayroll();
		var claimable = host.Payroll.ClaimablePayablesFor(actor)
		                    .OrderBy(x => x.DueAt)
		                    .ThenBy(x => x.Id)
		                    .ToList();
		var targets = SelectPayables(claimable, selector);
		if (!targets.Any())
		{
			var outstanding = host.Payroll.Payables
			                      .Where(x => x.EmployeeId == actorIdentityId)
			                      .Count(x => x.Status == EmploymentPayableStatus.Accrued);
			if (outstanding > 0)
			{
				message = $"You have {outstanding.ToString("N0", actor).ColourValue()} outstanding employment payable{(outstanding == 1 ? string.Empty : "s")}, but {(outstanding == 1 ? "it has" : "they have")} not been settled by the employer yet.";
				return false;
			}

			message = $"There is no claimable employment payable matching {selector.ColourCommand()}.";
			return false;
		}

		var messages = new List<string>();
		foreach (var target in targets)
		{
			if (!host.Payroll.TryClaimPayable(target, actor, out var claimMessage))
			{
				message = claimMessage;
				return false;
			}

			messages.Add(claimMessage);
		}

		message = messages.Count == 1
			? messages[0]
			: $"You claim {messages.Count.ToString("N0", actor).ColourValue()} employment payables from {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public bool TryCancelTask(ICharacter actor, IEmploymentHost host, string selector, string reason, out string message)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.CancelTasks, out message))
		{
			return false;
		}

		var task = ActiveTaskBySelector(host, selector);
		if (task is null)
		{
			message = $"There is no active employment task matching {selector.ColourCommand()}.";
			return false;
		}

		if (task.Status is EmploymentTaskStatus.Completed or EmploymentTaskStatus.Cancelled or EmploymentTaskStatus.Failed)
		{
			message = $"The task {task.Name.ColourName()} is already {task.Status.DescribeEnum().ColourValue()} and cannot be cancelled.";
			return false;
		}

		try
		{
			if (!host.TaskBoard.CancelActiveTask(task, actor, reason))
			{
				message = $"The task {task.Name.ColourName()} could not be cancelled.";
				return false;
			}
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}

		message = $"You cancel employment task {task.Name.ColourName()}.";
		return true;
	}

	public bool TrySetContractAuthority(ICharacter actor, IEmploymentHost host, long contractId,
		EmploymentAuthoritySet authority, out string message)
	{
		var contract = ContractById(host, contractId);
		if (contract is null)
		{
			message = "There is no such employment contract.";
			return false;
		}

		try
		{
			host.SetContractAuthority(contract, authority, actor);
			message = $"You set {contract.Employee.HowSeen(actor, colour: false).ColourName()}'s delegated authority for {host.EmploymentHostName.ColourName()} to {DescribeAuthoritySet(authority)}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryGrantContractAuthority(ICharacter actor, IEmploymentHost host, long contractId,
		EmploymentAuthoritySet authority, out string message)
	{
		var contract = ContractById(host, contractId);
		if (contract is null)
		{
			message = "There is no such employment contract.";
			return false;
		}

		return TrySetContractAuthority(actor, host, contractId,
			new EmploymentAuthoritySet(contract.Authority.Authorities | authority.Authorities), out message);
	}

	public bool TryRevokeContractAuthority(ICharacter actor, IEmploymentHost host, long contractId,
		EmploymentAuthoritySet authority, out string message)
	{
		var contract = ContractById(host, contractId);
		if (contract is null)
		{
			message = "There is no such employment contract.";
			return false;
		}

		return TrySetContractAuthority(actor, host, contractId,
			new EmploymentAuthoritySet(contract.Authority.Authorities & ~authority.Authorities), out message);
	}

	public bool TryHireDirectContract(ICharacter actor, IEmploymentHost host, ICharacter target, EmploymentRole role,
		out IEmploymentContract? contract, out string message)
	{
		contract = null;
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.HireEmployees, out message))
		{
			return false;
		}

		if (role == EmploymentRole.Proprietor && !host.HasProprietorEmploymentAccess(actor))
		{
			message = "Only a proprietor or administrator can create proprietor employment contracts.";
			return false;
		}

		var offer = new EmploymentOffer(
			role,
			UnpaidCompensation(),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			DefaultDirectHireAuthority(role));
		try
		{
			contract = host.Hire(target, offer, actor);
			message = $"You create an active {role.DescribeEnum().ColourName()} employment contract for {target.HowSeen(actor, colour: false).ColourName()} at {host.EmploymentHostName.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	public bool TryTerminateContractsForEmployee(ICharacter actor, IEmploymentHost host, ICharacter target,
		out string message)
	{
		var targetIdentityId = CharacterInstanceIdentityComparer.IdentityId(target);
		return TryTerminateContractsForEmployee(actor, host,
			host.ActiveEmploymentContracts().Where(x => x.Employee.Id == targetIdentityId).ToList(), out message);
	}

	public bool TryTerminateContractsForEmployee(ICharacter actor, IEmploymentHost host, string employeeName,
		out string message)
	{
		employeeName = employeeName.Trim();
		if (string.IsNullOrWhiteSpace(employeeName))
		{
			message = "Which employee do you want to terminate?";
			return false;
		}

		var matches = host.ActiveEmploymentContracts()
		                  .GroupBy(x => x.Employee.Id)
		                  .Select(x => new
		                  {
			                  Employee = x.First().Employee,
			                  Contracts = x.ToList()
		                  })
		                  .Where(x =>
			                  x.Employee.Name.StartsWith(employeeName, StringComparison.InvariantCultureIgnoreCase) ||
			                  x.Employee.HowSeen(actor, colour: false)
			                   .StartsWith(employeeName, StringComparison.InvariantCultureIgnoreCase))
		                  .ToList();
		if (matches.Count > 1)
		{
			message = $"More than one employee matches {employeeName.ColourCommand()}. Please be more specific.";
			return false;
		}

		return TryTerminateContractsForEmployee(actor, host, matches.SingleOrDefault()?.Contracts ?? [], out message);
	}

	public bool TryToggleRoleContract(ICharacter actor, IEmploymentHost host, ICharacter target, EmploymentRole role,
		out string message)
	{
		var targetIdentityId = CharacterInstanceIdentityComparer.IdentityId(target);
		var existing = host.ActiveEmploymentContracts()
		                   .FirstOrDefault(x => x.Employee.Id == targetIdentityId && x.Role == role);
		if (existing is not null)
		{
			return TryTerminateContract(actor, host, existing.Id, out message);
		}

		return TryHireDirectContract(actor, host, target, role, out _, out message);
	}

	public bool TryResignFromHost(ICharacter actor, IEmploymentHost host, out string message)
	{
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		var contracts = host.ActiveEmploymentContracts()
		                    .Where(x => x.Employee.Id == actorIdentityId)
		                    .ToList();
		if (!contracts.Any())
		{
			message = $"You do not have an active employment contract with {host.EmploymentHostName.ColourName()}.";
			return false;
		}

		foreach (var contract in contracts)
		{
			host.Employment.Fire(contract, EmploymentTerminationReason.Resigned, null);
		}

		message = $"You resign from your active employment contracts with {host.EmploymentHostName.ColourName()}.";
		return true;
	}

	public string RenderStatus(ICharacter actor, IEmploymentHost host)
	{
		var contracts = host.EmploymentContracts;
		var openings = host.JobOpenings;
		var tasks = host.TaskBoard.ActiveTasks;
		var goals = host.ManagerGoalBoard.Goals;
		var sb = new StringBuilder();
		sb.AppendLine($"Employment status for {host.EmploymentHostName.ColourName()}");
		sb.AppendLine($"Host Type: {host.EmploymentHostType.DescribeEnum().ColourName()}");
		sb.AppendLine($"Contracts: {contracts.Count(x => x.Status == EmploymentStatus.Active).ToString("N0", actor).ColourValue()} active, {contracts.Count(x => x.Status == EmploymentStatus.Ended).ToString("N0", actor).ColourValue()} ended");
		sb.AppendLine($"Job Openings: {openings.Count(x => x.Status == JobOpeningStatus.Open).ToString("N0", actor).ColourValue()} open, {openings.Count.ToString("N0", actor).ColourValue()} total");
		sb.AppendLine($"Applications: {host.Employment.Applications.Count.ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Payroll Liabilities: {host.Payroll.OutstandingLiabilities.Count.ToString("N0", actor).ColourValue()} outstanding, {host.Payroll.MaximumOverdueDays().ToString("N0", actor).ColourValue()} days max overdue");
		sb.AppendLine($"Active Tasks: {tasks.Count(x => x.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Cancelled and not EmploymentTaskStatus.Failed).ToString("N0", actor).ColourValue()}");
		sb.AppendLine($"Manager Goals: {goals.Count(x => x.Status == ManagerGoalStatus.Active).ToString("N0", actor).ColourValue()} active");
		sb.AppendLine($"Board Posts: {host.Board.Posts.Count().ToString("N0", actor).ColourValue()}");
		return sb.ToString();
	}

	public string RenderContracts(ICharacter actor, IEmploymentHost host)
	{
		if (!host.EmploymentContracts.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment contracts.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employment contracts for {host.EmploymentHostName.ColourName()}:");
		foreach (var contract in host.EmploymentContracts.OrderBy(x => x.Employee.Name).ThenBy(x => x.Role))
		{
			sb.AppendLine($"\t#{contract.Id.ToString("N0", actor)} - {contract.Employee.HowSeen(actor, colour: false).ColourName()} - {contract.Role.DescribeEnum().ColourName()} - {contract.Status.DescribeEnum().ColourValue()} - {contract.Authority.Authorities.DescribeEnum()}");
		}

		return sb.ToString();
	}

	public string RenderOpenings(ICharacter actor, IEmploymentHost host)
	{
		if (!CanViewOpenings(actor, host))
		{
			return $"{host.EmploymentHostName.ColourName()} has no open job openings visible to you.";
		}

		var openings = CanViewOperational(actor, host)
			? host.JobOpenings.OrderBy(x => x.Role).ThenBy(x => x.Id).ToList()
			: host.JobOpenings.Where(x => x.Status == JobOpeningStatus.Open).OrderBy(x => x.Role).ThenBy(x => x.Id).ToList();
		if (!openings.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no job openings.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Job openings for {host.EmploymentHostName.ColourName()}:");
		foreach (var opening in openings)
		{
			sb.AppendLine($"\t#{opening.Id.ToString("N0", actor)} - {opening.Role.DescribeEnum().ColourName()} - {opening.Status.DescribeEnum().ColourValue()} - {DescribeCompensation(opening.Compensation, actor)} - {opening.MaxPositions.ToString("N0", actor)} position{(opening.MaxPositions == 1 ? string.Empty : "s")} - authority {DescribeAuthoritySet(opening.Authority)}");
		}

		return sb.ToString();
	}

	public string RenderApplications(ICharacter actor, IEmploymentHost host)
	{
		if (!host.Employment.Applications.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment applications.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employment applications for {host.EmploymentHostName.ColourName()}:");
		foreach (var application in host.Employment.Applications.OrderByDescending(x => x.AppliedAt))
		{
			sb.AppendLine($"\t#{application.Id.ToString("N0", actor)} - {application.Candidate.HowSeen(actor, colour: false).ColourName()} for {application.Opening.Role.DescribeEnum().ColourName()} - {application.Status.DescribeEnum().ColourValue()}{(string.IsNullOrWhiteSpace(application.DecisionReason) ? string.Empty : $" - {application.DecisionReason}")}");
		}

		return sb.ToString();
	}

	public string RenderPayroll(ICharacter actor, IEmploymentHost host)
	{
		if (!CanViewPayroll(actor, host))
		{
			return $"You have no visible employment payables for {host.EmploymentHostName.ColourName()}.";
		}

		var canViewAll = CanViewAllPayroll(actor, host);
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		var payables = (canViewAll
				? host.Payroll.Payables
				: host.Payroll.Payables.Where(x => x.EmployeeId == actorIdentityId))
			.OrderByDescending(x => x.DueAt)
			.ThenByDescending(x => x.Id)
			.Take(25)
			.ToList();

		var allVisible = canViewAll
			? host.Payroll.Payables
			: host.Payroll.Payables.Where(x => x.EmployeeId == actorIdentityId).ToList();
		var outstanding = allVisible.Count(x => x.Status == EmploymentPayableStatus.Accrued);
		var claimable = allVisible.Count(x => x.Status == EmploymentPayableStatus.ReadyToClaim);

		var sb = new StringBuilder();
		sb.AppendLine($"Employment payroll for {host.EmploymentHostName.ColourName()}:");
		sb.AppendLine($"Outstanding: {outstanding.ToString("N0", actor).ColourValue()} | Claimable: {claimable.ToString("N0", actor).ColourValue()} | Max Overdue: {host.Payroll.MaximumOverdueDays().ToString("N0", actor).ColourValue()} days");
		if (!payables.Any())
		{
			sb.AppendLine("\tNone");
			return sb.ToString();
		}

		foreach (var payable in payables)
		{
			sb.AppendLine($"\t#{payable.Id.ToString("N0", actor)} - {payable.EmployeeName.ColourName()} - {payable.Role.DescribeEnum().ColourName()} - {DescribeMoney(payable.Amount, actor)} - {payable.Status.DescribeEnum().ColourValue()} - due {EmploymentClock.DescribeInstant(host, payable.DueAt, actor).ColourValue()} - {payable.DaysOverdue(EmploymentClock.CurrentInstant(host)).ToString("N0", actor).ColourValue()} days overdue");
		}

		return sb.ToString();
	}

	public string RenderTasks(ICharacter actor, IEmploymentHost host)
	{
		host.TaskBoard.AuditActiveTaskAssignments();
		var sb = new StringBuilder();
		sb.AppendLine($"Employment task board for {host.EmploymentHostName.ColourName()}:");
		sb.AppendLine();
		sb.AppendLine("Scheduled Rules:");
		if (!host.TaskBoard.ScheduledRules.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			var index = 1;
			foreach (var rule in host.TaskBoard.ScheduledRules.OrderBy(x => x.Name))
			{
				sb.AppendLine($"\t#{index++.ToString("N0", actor)} - {rule.Name.ColourName()} - {rule.Status.DescribeEnum().ColourValue()} - {rule.Conditions.Count.ToString("N0", actor)} condition{(rule.Conditions.Count == 1 ? string.Empty : "s")} - {rule.ActionPlan.Steps.Count.ToString("N0", actor)} step{(rule.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}");
			}
		}

		sb.AppendLine();
		sb.AppendLine("Active Tasks:");
		if (!host.TaskBoard.ActiveTasks.Any())
		{
			sb.AppendLine("\tNone");
		}
		else
		{
			var index = 1;
			foreach (var task in host.TaskBoard.ActiveTasks.OrderBy(x => x.Name))
			{
				sb.AppendLine($"\t#{index++.ToString("N0", actor)} - {task.Name.ColourName()} - {task.Status.DescribeEnum().ColourValue()} - assigned to {task.AssignedEmployee?.HowSeen(actor, colour: false).ColourName() ?? "nobody".ColourError()} - next step {DescribeNextStep(task, actor)}");
			}
		}

		return sb.ToString();
	}

	public string RenderTaskDetail(ICharacter actor, IEmploymentHost host, string selector)
	{
		host.TaskBoard.AuditActiveTaskAssignments();
		if (string.IsNullOrWhiteSpace(selector))
		{
			return "Which employment task do you want to view?";
		}

		var activeTask = ActiveTaskBySelector(host, selector);
		if (activeTask is not null)
		{
			return RenderActiveTaskDetail(actor, activeTask);
		}

		var scheduledRule = ScheduledRuleBySelector(host, selector);
		if (scheduledRule is not null)
		{
			return RenderScheduledRuleDetail(actor, scheduledRule);
		}

		return $"There is no active employment task or scheduled rule matching {selector.ColourCommand()}.";
	}

	private static string RenderActiveTaskDetail(ICharacter actor, IEmploymentActiveTask task)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Employment Task - {task.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Status: {task.Status.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Assigned Employee: {task.AssignedEmployee?.HowSeen(actor, colour: false).ColourName() ?? "nobody".ColourError()}");
		if (!string.IsNullOrWhiteSpace(task.BlockedReason))
		{
			sb.AppendLine($"Blocked Reason: {task.BlockedReason.ColourError()}");
		}

		sb.AppendLine($"Correlation: {task.CorrelationId.ToString("D").ColourValue()}");
		sb.AppendLine($"Required Authority: {task.ActionPlan.RequiredAuthority.Authorities.DescribeEnum().ColourName()}");
		sb.AppendLine($"Required AI Capabilities: {DescribeCapabilities(task.ActionPlan.RequiredCapabilities)}");
		sb.AppendLine();
		sb.AppendLine("Steps:");
		AppendActionPlanSteps(sb, actor, task.ActionPlan, task.StepStates, task.StepOperationalStates);
		return sb.ToString();
	}

	private static string RenderScheduledRuleDetail(ICharacter actor, IEmploymentScheduledTaskRule rule)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Scheduled Employment Rule - {rule.Name.ColourName()}".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine($"Status: {rule.Status.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Cooldown: {rule.Cooldown.Describe(actor).ColourValue()}");
		sb.AppendLine($"Last Spawned: {(rule.LastSpawnedAt.HasValue ? EmploymentClock.DescribeInstant(rule.Employer, rule.LastSpawnedAt.Value, actor).ColourValue() : "never".ColourError())}");
		sb.AppendLine($"Idempotency Key: {rule.IdempotencyKey.ColourValue()}");
		var conditionAuthority = rule.Conditions.Aggregate(EmploymentAuthority.None,
			(current, condition) => current | condition.RequiredAuthority.Authorities);
		sb.AppendLine($"Condition Authority: {(conditionAuthority == EmploymentAuthority.None ? "none".ColourValue() : conditionAuthority.DescribeEnum().ColourName())}");
		sb.AppendLine($"Condition Expression: {EmploymentScheduledRuleAuthoringService.DescribeConditionExpression(rule.ConditionExpression, rule.Conditions.ToList()).ColourCommand()}");
		sb.AppendLine($"Action Authority: {rule.ActionPlan.RequiredAuthority.Authorities.DescribeEnum().ColourName()}");
		sb.AppendLine($"Required AI Capabilities: {DescribeCapabilities(rule.ActionPlan.RequiredCapabilities)}");
		sb.AppendLine();
		EmploymentScheduledRuleAuthoringService.AppendConditionList(sb, actor, rule.Employer, rule.Conditions.ToList());
		sb.AppendLine();
		sb.AppendLine("Steps:");
		AppendActionPlanSteps(sb, actor, rule.ActionPlan, null, null);
		return sb.ToString();
	}

	private static void AppendActionPlanSteps(StringBuilder sb, ICharacter actor, EmploymentActionPlan plan,
		IReadOnlyList<EmploymentActionStepStatus>? stepStates,
		IReadOnlyList<EmploymentActionStepOperationalState>? operationalStates)
	{
		if (!plan.Steps.Any())
		{
			sb.AppendLine("\tNone");
			return;
		}

		for (var i = 0; i < plan.Steps.Count; i++)
		{
			var step = plan.Steps[i];
			var status = stepStates is not null && i < stepStates.Count
				? stepStates[i].DescribeEnum().ColourValue()
				: "planned".ColourValue();
			sb.AppendLine($"\t#{(i + 1).ToString("N0", actor)} - {status} - {EmploymentTaskAuthoringService.DescribeStep(step, actor)}");
			sb.AppendLine($"\t\tType: {step.StepType.DescribeEnum().ColourName()} | Authority: {step.RequiredAuthority.Authorities.DescribeEnum().ColourName()} | AI: {DescribeCapabilities(step.RequiredCapabilities)} | Catalogue: {EmploymentTaskAuthoringService.DescribeStepCatalogueStatus(step)}");
			var warning = EmploymentTaskAuthoringService.DescribeStepBoundaryWarning(step);
			if (!string.IsNullOrWhiteSpace(warning))
			{
				sb.AppendLine($"\t\t{warning}");
			}

			if (operationalStates is not null && i < operationalStates.Count && !operationalStates[i].IsEmpty)
			{
				sb.AppendLine($"\t\tState: {DescribeOperationalState(operationalStates[i], actor)}");
			}
		}
	}

	private static string DescribeOperationalState(EmploymentActionStepOperationalState state, IPerceiver voyeur)
	{
		var parts = new List<string>();
		if (!string.IsNullOrWhiteSpace(state.OperationalPayload))
		{
			parts.Add($"Detail: {state.OperationalPayload.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.TransactionReference))
		{
			parts.Add($"Transaction: {state.TransactionReference.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.SelectedResources))
		{
			parts.Add($"Selection: {state.SelectedResources.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.ReservationReference))
		{
			parts.Add($"Reservation: {state.ReservationReference.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.RouteResult))
		{
			parts.Add($"Route: {state.RouteResult.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.CraftJobReference))
		{
			parts.Add($"Craft: {EmploymentCraftService.DescribeCraftReference(state.CraftJobReference, voyeur).ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.LoadedAssets))
		{
			parts.Add($"Loaded: {state.LoadedAssets.ColourValue()}");
		}

		if (!string.IsNullOrWhiteSpace(state.FailureDiagnostic))
		{
			parts.Add($"Failure: {state.FailureDiagnostic.ColourError()}");
		}

		return parts.Any() ? string.Join("; ", parts) : "none".ColourValue();
	}

	public string RenderTaskDiagnostics(ICharacter actor, IEmploymentHost host)
	{
		host.TaskBoard.AuditActiveTaskAssignments();
		var activeTasks = host.TaskBoard.ActiveTasks
		                      .Where(x => x.Status is EmploymentTaskStatus.Pending or EmploymentTaskStatus.Assigned or
			                      EmploymentTaskStatus.InProgress or EmploymentTaskStatus.Blocked)
		                      .OrderBy(x => x.Name)
		                      .ToList();
		if (!activeTasks.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no active employment tasks to diagnose.";
		}

		var activeContracts = host.ActiveEmploymentContracts()
		                          .OrderBy(x => x.Employee.Name)
		                          .ThenBy(x => x.Role)
		                          .ToList();
		var sb = new StringBuilder();
		sb.AppendLine($"Employment task diagnostics for {host.EmploymentHostName.ColourName()}:");
		foreach (var task in activeTasks)
		{
			sb.AppendLine();
			sb.AppendLine($"{task.Name.ColourName()} - {task.Status.DescribeEnum().ColourValue()} - next step {DescribeNextStep(task, actor)}");
			sb.AppendLine($"\tRequired Authority: {task.ActionPlan.RequiredAuthority.Authorities.DescribeEnum().ColourName()}");
			sb.AppendLine($"\tRequired AI Capabilities: {(task.ActionPlan.RequiredCapabilities.Any() ? task.ActionPlan.RequiredCapabilities.Select(x => x.DescribeEnum().ColourName()).ListToString() : "none".ColourValue())}");
			if (task.AssignedEmployee is not null)
			{
				sb.AppendLine($"\tAssigned Employee: {task.AssignedEmployee.HowSeen(actor, colour: false).ColourName()}");
			}

			if (!activeContracts.Any())
			{
				sb.AppendLine("\tNo active employment contracts exist for this host.".ColourError());
				continue;
			}

			foreach (var contract in activeContracts)
			{
				sb.AppendLine($"\t{contract.Employee.HowSeen(actor, colour: false).ColourName()} ({contract.Role.DescribeEnum().ColourName()}): {DescribeTaskEligibility(host, task, contract, actor)}");
			}
		}

		return sb.ToString();
	}

	public string RenderGoals(ICharacter actor, IEmploymentHost host)
	{
		if (!host.ManagerGoalBoard.Goals.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no manager goals.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Manager goals for {host.EmploymentHostName.ColourName()}:");
		foreach (var goal in host.ManagerGoalBoard.Goals.OrderBy(x => x.Priority).ThenBy(x => x.GoalType))
		{
			sb.AppendLine($"\t#{goal.Id.ToString("N0", actor)} - {goal.GoalType.DescribeEnum().ColourName()} - {goal.Status.DescribeEnum().ColourValue()} - priority {goal.Priority.ToString("N0", actor).ColourValue()} - {goal.Configuration.Description}");
		}

		return sb.ToString();
	}

	public string RenderRegister(ICharacter actor, IEmploymentHost host)
	{
		if (!host.EmploymentRegister.Entries.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment register entries.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employment register for {host.EmploymentHostName.ColourName()}:");
		foreach (var entry in host.EmploymentRegister.Entries.OrderByDescending(x => x.RecordedAt).Take(25))
		{
			sb.AppendLine($"\t{EmploymentClock.DescribeInstant(host, entry.RecordedAt, actor).ColourValue()} - {entry.EntryType.DescribeEnum().ColourName()} - {entry.Actor?.HowSeen(actor, colour: false).ColourName() ?? "System".ColourName()} - {entry.Description}");
		}

		return sb.ToString();
	}

	public string RenderLedger(ICharacter actor, IEmploymentHost host)
	{
		if (!host.BusinessLedger.Entries.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment ledger entries.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employment ledger for {host.EmploymentHostName.ColourName()}:");
		foreach (var entry in host.BusinessLedger.Entries.OrderByDescending(x => x.RecordedAt).Take(25))
		{
			sb.AppendLine($"\t{EmploymentClock.DescribeInstant(host, entry.RecordedAt, actor).ColourValue()} - {entry.EntryType.DescribeEnum().ColourName()} - {DescribeMoney(entry.Amount, actor)} - {entry.Actor?.HowSeen(actor, colour: false).ColourName() ?? "System".ColourName()} - {entry.Description}");
		}

		return sb.ToString();
	}

	public string RenderBoard(ICharacter actor, IEmploymentHost host)
	{
		var posts = OrderedBoardPosts(host).ToList();
		if (!posts.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment board posts.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Employment board posts for {host.EmploymentHostName.ColourName()}:");
		for (var i = 0; i < posts.Count; i++)
		{
			var post = posts[i];
			sb.AppendLine($"\t{(i + 1).ToString("N0", actor)} - {post.Title.ColourName()} by {post.AuthorName.ColourName()} at {post.PostTime.ToString("g", actor)}");
		}

		return sb.ToString();
	}

	public string RenderBoardPost(ICharacter actor, IEmploymentHost host, int postNumber)
	{
		var posts = OrderedBoardPosts(host).ToList();
		if (postNumber < 1 || postNumber > posts.Count)
		{
			return "There is no such employment board post.";
		}

		var post = posts[postNumber - 1];
		var sb = new StringBuilder();
		sb.AppendLine($"{post.Title.ColourName()}");
		sb.AppendLine($"By {post.AuthorName.ColourName()} at {post.PostTime.ToString("g", actor)}");
		sb.AppendLine();
		sb.AppendLine(post.Text);
		return sb.ToString();
	}

	private void HandleContracts(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var contractCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (contractCommand)
		{
			case "":
			case "list":
			case "show":
				SendOperationalView(actor, host, RenderContracts);
				return;
			case "fire":
			case "terminate":
			case "end":
			case "dismiss":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which employment contract do you want to terminate?");
					return;
				}

				var contractText = input.PopSpeech();
				if (contractText.StartsWith("#", StringComparison.Ordinal))
				{
					contractText = contractText[1..];
				}

				if (!long.TryParse(contractText, out var contractId))
				{
					actor.OutputHandler.Send($"You must specify a contract number, e.g. {"contracts fire #1".ColourCommand()}.");
					return;
				}

				TryTerminateContract(actor, host, contractId, out var message);
				actor.OutputHandler.Send(message);
				return;
			case "authority":
			case "authorities":
			case "delegation":
			case "delegations":
			case "delegate":
				HandleContractAuthority(actor, host, input);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleContractAuthority(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		if (!CanViewOperational(actor, host))
		{
			actor.OutputHandler.Send($"You are not an employee of {host.EmploymentHostName.ColourName()}.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send(RenderContractAuthorities(actor, host));
			return;
		}

		var first = input.PopSpeech();
		if (first.EqualTo("list") || first.EqualTo("show"))
		{
			actor.OutputHandler.Send(RenderContractAuthorities(actor, host));
			return;
		}

		if (first.EqualTo("help") || first.EqualTo("?"))
		{
			actor.OutputHandler.Send(RenderAuthorityHelp(actor));
			return;
		}

		if (!TryParseCommandNumber(first, out var contractId))
		{
			actor.OutputHandler.Send($"Which employment contract do you want to change? Use syntax like {"contracts delegate #1 grant ManageDeliveryRoutes".ColourCommand()}.");
			return;
		}

		var contract = ContractById(host, contractId);
		if (contract is null)
		{
			actor.OutputHandler.Send("There is no such employment contract.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send(RenderContractAuthority(actor, contract));
			return;
		}

		var action = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (action)
		{
			case "show":
			case "view":
				actor.OutputHandler.Send(RenderContractAuthority(actor, contract));
				return;
			case "grant":
			case "add":
				if (!TryParseAuthoritySet(input, out var grantAuthority, out var grantError))
				{
					actor.OutputHandler.Send(grantError);
					return;
				}

				TryGrantContractAuthority(actor, host, contractId, grantAuthority, out var grantMessage);
				actor.OutputHandler.Send(grantMessage);
				return;
			case "revoke":
			case "remove":
				if (!TryParseAuthoritySet(input, out var revokeAuthority, out var revokeError))
				{
					actor.OutputHandler.Send(revokeError);
					return;
				}

				TryRevokeContractAuthority(actor, host, contractId, revokeAuthority, out var revokeMessage);
				actor.OutputHandler.Send(revokeMessage);
				return;
			case "set":
				if (!TryParseAuthoritySet(input, out var authority, out var setError, allowNone: true))
				{
					actor.OutputHandler.Send(setError);
					return;
				}

				TrySetContractAuthority(actor, host, contractId, authority, out var setMessage);
				actor.OutputHandler.Send(setMessage);
				return;
		}

		actor.OutputHandler.Send(RenderAuthorityHelp(actor));
	}

	private void HandleOpenings(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var openingCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (openingCommand)
		{
			case "":
			case "list":
			case "show":
				actor.OutputHandler.Send(RenderOpenings(actor, host));
				return;
			case "create":
			case "new":
				CreateOpening(actor, host, input);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void CreateOpening(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which employment role should this opening be for?");
			return;
		}

		if (!input.PopSpeech().TryParseEnum(out EmploymentRole role))
		{
			actor.OutputHandler.Send($"That is not a valid employment role. The options are {Enum.GetValues<EmploymentRole>().Select(x => x.DescribeEnum().ColourName()).ListToString()}.");
			return;
		}

		var currency = ResolveHostCurrency(host);
		if (currency is null)
		{
			actor.OutputHandler.Send($"Could not determine the currency for {host.EmploymentHostName.ColourName()}.");
			return;
		}

		if (!TryParseOpeningCompensation(input, currency, out var hourlyRate, out var maxPositions, out var error))
		{
			actor.OutputHandler.Send(error);
			return;
		}

		TryCreateOpening(actor, host, role, hourlyRate, maxPositions, out _, out var message);
		actor.OutputHandler.Send(message);
	}

	private void HandleApplications(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var applicationCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (applicationCommand)
		{
			case "":
			case "list":
			case "show":
				SendOperationalView(actor, host, RenderApplications);
				return;
			case "accept":
			case "approve":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var acceptNumber))
				{
					actor.OutputHandler.Send("Which employment application do you want to accept?");
					return;
				}

				TryAcceptApplication(actor, host, acceptNumber, out _, out var acceptMessage);
				actor.OutputHandler.Send(acceptMessage);
				return;
			case "reject":
			case "deny":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var rejectNumber))
				{
					actor.OutputHandler.Send("Which employment application do you want to reject?");
					return;
				}

				var reason = input.IsFinished ? "Rejected by a manager." : input.SafeRemainingArgument;
				TryRejectApplication(actor, host, rejectNumber, reason, out var rejectMessage);
				actor.OutputHandler.Send(rejectMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandlePayroll(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var payrollCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (payrollCommand)
		{
			case "":
			case "list":
			case "show":
				actor.OutputHandler.Send(RenderPayroll(actor, host));
				return;
			case "run":
			case "accrue":
			case "evaluate":
				TryRunPayroll(actor, host, out _, out var runMessage);
				actor.OutputHandler.Send(runMessage);
				return;
			case "settle":
			case "resolve":
			case "fund":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which employment payable do you want to settle? Use all or a payable number.");
					return;
				}

				var settleSelector = input.PopSpeech();
				var reason = input.IsFinished ? "Settled by employer." : input.SafeRemainingArgument;
				TrySettlePayroll(actor, host, settleSelector, reason, out var settleMessage);
				actor.OutputHandler.Send(settleMessage);
				return;
			case "claim":
			case "collect":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which employment payable do you want to claim? Use all or a payable number.");
					return;
				}

				TryClaimPayroll(actor, host, input.PopSpeech(), out var claimMessage);
				actor.OutputHandler.Send(claimMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleTasks(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var taskCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (taskCommand)
		{
			case "":
			case "list":
				SendOperationalView(actor, host, RenderTasks);
				return;
			case "show":
				if (input.IsFinished)
				{
					SendOperationalView(actor, host, RenderTasks);
					return;
				}

				SendOperationalView(actor, host,
					(viewer, employmentHost) => RenderTaskDetail(viewer, employmentHost, input.SafeRemainingArgument));
				return;
			case "view":
			case "detail":
			case "info":
				SendOperationalView(actor, host,
					(viewer, employmentHost) => RenderTaskDetail(viewer, employmentHost, input.SafeRemainingArgument));
				return;
			case "draft":
				HandleTaskDraft(actor, host, input);
				return;
			case "create":
			case "new":
				_taskAuthoring.TryCreateOneShotTask(actor, host, input, out _, out var createMessage);
				actor.OutputHandler.Send(createMessage);
				return;
			case "cancel":
			case "abort":
				HandleTaskCancel(actor, host, input);
				return;
			case "actions":
			case "action":
				actor.OutputHandler.Send(_taskAuthoring.RenderAvailableActions(actor, input.SafeRemainingArgument));
				return;
			case "conditions":
			case "condition":
			case "conditioncatalog":
				actor.OutputHandler.Send(_scheduledRuleAuthoring.RenderAvailableConditions(actor, input.SafeRemainingArgument));
				return;
			case "rule":
			case "rules":
			case "scheduled":
			case "schedule":
				HandleRules(actor, host, input);
				return;
			case "diagnose":
			case "diagnostic":
			case "why":
				SendOperationalView(actor, host, RenderTaskDiagnostics);
				return;
			case "step":
				_taskAuthoring.TryAddStep(actor, host, input, out var stepMessage);
				actor.OutputHandler.Send(stepMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleTaskCancel(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		if (input.IsFinished)
		{
			actor.OutputHandler.Send("Which employment task do you want to cancel?");
			return;
		}

		var selector = input.PopSpeech();
		var reason = input.IsFinished ? "Cancelled by a manager." : input.SafeRemainingArgument;
		TryCancelTask(actor, host, selector, reason, out var message);
		actor.OutputHandler.Send(message);
	}

	private void HandleTaskDraft(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var draftCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (draftCommand)
		{
			case "":
			case "show":
			case "view":
				actor.OutputHandler.Send(_taskAuthoring.RenderDraft(actor, host));
				return;
			case "new":
			case "create":
				_taskAuthoring.TryStartDraft(actor, host, input.SafeRemainingArgument, out var newMessage);
				actor.OutputHandler.Send(newMessage);
				return;
			case "rename":
			case "name":
				_taskAuthoring.TryRenameDraft(actor, host, input.SafeRemainingArgument, out var renameMessage);
				actor.OutputHandler.Send(renameMessage);
				return;
			case "remove":
			case "delete":
				if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var stepNumber))
				{
					actor.OutputHandler.Send("Which step do you want to remove from your employment task draft?");
					return;
				}

				_taskAuthoring.TryRemoveStep(actor, host, stepNumber, out var removeMessage);
				actor.OutputHandler.Send(removeMessage);
				return;
			case "discard":
			case "cancel":
				_taskAuthoring.TryDiscardDraft(actor, host, out var discardMessage);
				actor.OutputHandler.Send(discardMessage);
				return;
			case "finalise":
			case "finalize":
			case "finish":
				_taskAuthoring.TryFinaliseDraft(actor, host, out _, out var finaliseMessage);
				actor.OutputHandler.Send(finaliseMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleRules(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var ruleCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (ruleCommand)
		{
			case "":
			case "list":
				SendOperationalView(actor, host, RenderTasks);
				return;
			case "show":
			case "view":
			case "detail":
			case "info":
				if (input.IsFinished)
				{
					SendOperationalView(actor, host, RenderTasks);
					return;
				}

				SendOperationalView(actor, host,
					(viewer, employmentHost) => RenderTaskDetail(viewer, employmentHost, input.SafeRemainingArgument));
				return;
			case "draft":
				HandleRuleDraft(actor, host, input);
				return;
			case "predicate":
			case "predicates":
				HandleRulePredicates(actor, host, input);
				return;
			case "template":
			case "templates":
				HandleRuleTemplates(actor, host, input);
				return;
			case "condition":
				_scheduledRuleAuthoring.TryAddCondition(actor, host, input, out var conditionMessage);
				actor.OutputHandler.Send(conditionMessage);
				return;
			case "step":
				_scheduledRuleAuthoring.TryAddStep(actor, host, input, out var stepMessage);
				actor.OutputHandler.Send(stepMessage);
				return;
			case "create":
			case "new":
				_scheduledRuleAuthoring.TryCreateOneShotRule(actor, host, input, out _, out var createMessage);
				actor.OutputHandler.Send(createMessage);
				return;
			case "conditions":
			case "conditioncatalog":
			case "catalogue":
			case "catalog":
				actor.OutputHandler.Send(_scheduledRuleAuthoring.RenderAvailableConditions(actor, input.SafeRemainingArgument));
				return;
			case "diagnose":
			case "diagnostic":
			case "why":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to diagnose?");
					return;
				}

				var diagnoseSelector = input.PopSpeech();
				var diagnoseManualKey = PopOptionalManualKey(input);
				SendOperationalView(actor, host,
					(viewer, employmentHost) => _scheduledRuleAuthoring.RenderDiagnostics(viewer, employmentHost,
						diagnoseSelector, diagnoseManualKey));
				return;
			case "evaluate":
			case "eval":
			case "run":
			case "trigger":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to evaluate? Use a rule number/name or all.");
					return;
				}

				var evaluateSelector = input.PopSpeech();
				var evaluateManualKey = PopOptionalManualKey(input);
				_scheduledRuleAuthoring.TryEvaluate(actor, host, evaluateSelector, evaluateManualKey, out var evaluateMessage);
				actor.OutputHandler.Send(evaluateMessage);
				return;
			case "pause":
			case "disable":
			case "suspend":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to pause?");
					return;
				}

				var pauseSelector = input.PopSpeech();
				var pauseReason = input.IsFinished ? "Paused by a manager." : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryPauseRule(actor, host, pauseSelector, pauseReason, out var pauseMessage);
				actor.OutputHandler.Send(pauseMessage);
				return;
			case "resume":
			case "enable":
			case "unpause":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to resume?");
					return;
				}

				var resumeSelector = input.PopSpeech();
				var resumeReason = input.IsFinished ? "Resumed by a manager." : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryResumeRule(actor, host, resumeSelector, resumeReason, out var resumeMessage);
				actor.OutputHandler.Send(resumeMessage);
				return;
			case "cancel":
			case "delete":
			case "remove":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to cancel?");
					return;
				}

				var cancelSelector = input.PopSpeech();
				var reason = input.IsFinished ? "Cancelled by a manager." : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryCancelRule(actor, host, cancelSelector, reason, out var cancelMessage);
				actor.OutputHandler.Send(cancelMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleRuleDraft(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var draftCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (draftCommand)
		{
			case "":
			case "show":
			case "view":
				actor.OutputHandler.Send(_scheduledRuleAuthoring.RenderDraft(actor, host));
				return;
			case "new":
			case "create":
				_scheduledRuleAuthoring.TryStartDraft(actor, host, input.SafeRemainingArgument, out var newMessage);
				actor.OutputHandler.Send(newMessage);
				return;
			case "copy":
			case "clone":
			case "from":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled employment rule do you want to copy into a draft?");
					return;
				}

				var copySelector = input.PopSpeech();
				var copyName = input.IsFinished ? null : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryCopyRuleToDraft(actor, host, copySelector, copyName, out var copyMessage);
				actor.OutputHandler.Send(copyMessage);
				return;
			case "key":
			case "idempotency":
				_scheduledRuleAuthoring.TrySetDraftKey(actor, host, input.SafeRemainingArgument, out var keyMessage);
				actor.OutputHandler.Send(keyMessage);
				return;
			case "cooldown":
			case "window":
				_scheduledRuleAuthoring.TrySetDraftCooldown(actor, host, input.SafeRemainingArgument, out var cooldownMessage);
				actor.OutputHandler.Send(cooldownMessage);
				return;
			case "expression":
			case "expr":
				_scheduledRuleAuthoring.TrySetDraftExpression(actor, host, input.SafeRemainingArgument, out var expressionMessage);
				actor.OutputHandler.Send(expressionMessage);
				return;
			case "condition":
				_scheduledRuleAuthoring.TryAddCondition(actor, host, input, out var conditionMessage);
				actor.OutputHandler.Send(conditionMessage);
				return;
			case "step":
			case "action":
				_scheduledRuleAuthoring.TryAddStep(actor, host, input, out var stepMessage);
				actor.OutputHandler.Send(stepMessage);
				return;
			case "removecondition":
			case "removecond":
			case "deletecondition":
			case "deletecond":
				if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var conditionNumber))
				{
					actor.OutputHandler.Send("Which condition do you want to remove from your scheduled rule draft?");
					return;
				}

				_scheduledRuleAuthoring.TryRemoveCondition(actor, host, conditionNumber, out var removeConditionMessage);
				actor.OutputHandler.Send(removeConditionMessage);
				return;
			case "removestep":
			case "removeaction":
			case "deleteaction":
			case "deletestep":
			case "remove":
				if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var stepNumber))
				{
					actor.OutputHandler.Send("Which action step do you want to remove from your scheduled rule draft?");
					return;
				}

				_scheduledRuleAuthoring.TryRemoveStep(actor, host, stepNumber, out var removeStepMessage);
				actor.OutputHandler.Send(removeStepMessage);
				return;
			case "discard":
			case "cancel":
				_scheduledRuleAuthoring.TryDiscardDraft(actor, host, out var discardMessage);
				actor.OutputHandler.Send(discardMessage);
				return;
			case "finalise":
			case "finalize":
			case "finish":
				_scheduledRuleAuthoring.TryFinaliseDraft(actor, host, out _, out var finaliseMessage);
				actor.OutputHandler.Send(finaliseMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleRulePredicates(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var command = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (command)
		{
			case "":
			case "list":
				SendOperationalView(actor, host,
					(viewer, employmentHost) => _scheduledRuleAuthoring.RenderPredicates(viewer, employmentHost));
				return;
			case "show":
			case "view":
				if (input.IsFinished)
				{
					SendOperationalView(actor, host,
						(viewer, employmentHost) => _scheduledRuleAuthoring.RenderPredicates(viewer, employmentHost));
					return;
				}

				SendOperationalView(actor, host,
					(viewer, employmentHost) => _scheduledRuleAuthoring.RenderPredicates(viewer, employmentHost,
						input.SafeRemainingArgument));
				return;
			case "create":
			case "save":
				_scheduledRuleAuthoring.TryCreatePredicateFromDraft(actor, host, input.SafeRemainingArgument,
					out var createMessage);
				actor.OutputHandler.Send(createMessage);
				return;
			case "copy":
			case "draft":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled condition predicate do you want to copy into a draft?");
					return;
				}

				var copySelector = input.PopSpeech();
				var copyName = input.IsFinished ? null : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryCopyPredicateToDraft(actor, host, copySelector, copyName, out var copyMessage);
				actor.OutputHandler.Send(copyMessage);
				return;
			case "cancel":
			case "delete":
			case "remove":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled condition predicate do you want to cancel?");
					return;
				}

				var cancelSelector = input.PopSpeech();
				var reason = input.IsFinished ? "Cancelled by a manager." : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryCancelPredicate(actor, host, cancelSelector, reason, out var cancelMessage);
				actor.OutputHandler.Send(cancelMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleRuleTemplates(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var command = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (command)
		{
			case "":
			case "list":
				SendOperationalView(actor, host,
					(viewer, employmentHost) => _scheduledRuleAuthoring.RenderTemplates(viewer, employmentHost));
				return;
			case "show":
			case "view":
				if (input.IsFinished)
				{
					SendOperationalView(actor, host,
						(viewer, employmentHost) => _scheduledRuleAuthoring.RenderTemplates(viewer, employmentHost));
					return;
				}

				SendOperationalView(actor, host,
					(viewer, employmentHost) => _scheduledRuleAuthoring.RenderTemplates(viewer, employmentHost,
						input.SafeRemainingArgument));
				return;
			case "save":
			case "create":
				_scheduledRuleAuthoring.TrySaveTemplateFromDraft(actor, host, input.SafeRemainingArgument,
					out var saveMessage);
				actor.OutputHandler.Send(saveMessage);
				return;
			case "draft":
			case "copy":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled rule template do you want to draft from?");
					return;
				}

				var templateSelector = input.PopSpeech();
				var draftName = input.IsFinished ? null : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryDraftFromTemplate(actor, host, templateSelector, draftName,
					out var draftMessage);
				actor.OutputHandler.Send(draftMessage);
				return;
			case "cancel":
			case "delete":
			case "remove":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which scheduled rule template do you want to cancel?");
					return;
				}

				var cancelSelector = input.PopSpeech();
				var reason = input.IsFinished ? "Cancelled by a manager." : input.SafeRemainingArgument;
				_scheduledRuleAuthoring.TryCancelTemplate(actor, host, cancelSelector, reason, out var cancelMessage);
				actor.OutputHandler.Send(cancelMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private static string? PopOptionalManualKey(StringStack input)
	{
		if (input.IsFinished)
		{
			return null;
		}

		if (!input.PopSpeech().EqualTo("manual"))
		{
			return null;
		}

		return input.SafeRemainingArgument.Trim();
	}

	private void HandleGoals(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var goalCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (goalCommand)
		{
			case "":
			case "list":
				SendOperationalView(actor, host, RenderGoals);
				return;
			case "show":
			case "view":
				if (input.IsFinished)
				{
					SendOperationalView(actor, host, RenderGoals);
					return;
				}

				var showSelector = input.SafeRemainingArgument;
				SendOperationalView(actor, host,
					(character, employmentHost) =>
						_managerGoalAuthoring.RenderGoalDetail(character, employmentHost, showSelector));
				return;
			case "types":
			case "type":
			case "catalogue":
			case "catalog":
			case "help":
				actor.OutputHandler.Send(_managerGoalAuthoring.RenderGoalTypes(actor, input.SafeRemainingArgument));
				return;
			case "new":
			case "create":
				HandleGoalDraft(actor, host, new StringStack($"new {input.SafeRemainingArgument}"));
				return;
			case "edit":
			case "copy":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which manager goal do you want to copy into a draft?");
					return;
				}

				var editSelector = input.PopSpeech();
				var editDescription = input.IsFinished ? null : input.SafeRemainingArgument;
				_managerGoalAuthoring.TryCopyGoalToDraft(actor, host, editSelector, editDescription,
					out var editMessage);
				actor.OutputHandler.Send(editMessage);
				return;
			case "draft":
				HandleGoalDraft(actor, host, input);
				return;
			case "condition":
			case "when":
				_managerGoalAuthoring.TryAddCondition(actor, host, input, out var conditionMessage);
				actor.OutputHandler.Send(conditionMessage);
				return;
			case "step":
			case "action":
			case "do":
				_managerGoalAuthoring.TryAddStep(actor, host, input, out var stepMessage);
				actor.OutputHandler.Send(stepMessage);
				return;
			case "cancel":
			case "delete":
			case "remove":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which manager goal do you want to cancel?");
					return;
				}

				var cancelSelector = input.PopSpeech();
				var cancelReason = input.IsFinished ? "Cancelled by a manager." : input.SafeRemainingArgument;
				_managerGoalAuthoring.TryCancelGoal(actor, host, cancelSelector, cancelReason, out var cancelMessage);
				actor.OutputHandler.Send(cancelMessage);
				return;
			case "evaluate":
			case "eval":
			case "run":
				_managerGoalAuthoring.TryEvaluateGoals(actor, host, out var evaluateMessage);
				actor.OutputHandler.Send(evaluateMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleGoalDraft(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var draftCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (draftCommand)
		{
			case "":
			case "show":
			case "list":
				actor.OutputHandler.Send(_managerGoalAuthoring.RenderDraft(actor, host));
				return;
			case "new":
			case "create":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send($"Manager goal drafts use the syntax: {"goals draft new <type> <description>".ColourCommand()}.");
					return;
				}

				var typeSelector = input.PopSpeech();
				_managerGoalAuthoring.TryStartDraft(actor, host, typeSelector, input.SafeRemainingArgument,
					out var startMessage);
				actor.OutputHandler.Send(startMessage);
				return;
			case "copy":
			case "edit":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which manager goal do you want to copy into a draft?");
					return;
				}

				var copySelector = input.PopSpeech();
				var copyDescription = input.IsFinished ? null : input.SafeRemainingArgument;
				_managerGoalAuthoring.TryCopyGoalToDraft(actor, host, copySelector, copyDescription,
					out var copyMessage);
				actor.OutputHandler.Send(copyMessage);
				return;
			case "type":
				if (input.IsFinished)
				{
					actor.OutputHandler.Send("Which manager goal type should this draft use?");
					return;
				}

				_managerGoalAuthoring.TrySetDraftType(actor, host, input.PopSpeech(), out var typeMessage);
				actor.OutputHandler.Send(typeMessage);
				return;
			case "name":
			case "rename":
			case "description":
			case "desc":
				_managerGoalAuthoring.TrySetDraftDescription(actor, host, input.SafeRemainingArgument,
					out var descriptionMessage);
				actor.OutputHandler.Send(descriptionMessage);
				return;
			case "priority":
				_managerGoalAuthoring.TrySetDraftPriority(actor, host, input.SafeRemainingArgument,
					out var priorityMessage);
				actor.OutputHandler.Send(priorityMessage);
				return;
			case "cadence":
			case "cooldown":
			case "interval":
				_managerGoalAuthoring.TrySetDraftCadence(actor, host, input.SafeRemainingArgument,
					out var cadenceMessage);
				actor.OutputHandler.Send(cadenceMessage);
				return;
			case "budget":
			case "budgets":
				_managerGoalAuthoring.TrySetDraftBudget(actor, host, input.SafeRemainingArgument,
					out var budgetMessage);
				actor.OutputHandler.Send(budgetMessage);
				return;
			case "risk":
			case "limit":
			case "limits":
				_managerGoalAuthoring.TrySetDraftRiskLimit(actor, host, input, out var riskMessage);
				actor.OutputHandler.Send(riskMessage);
				return;
			case "expression":
			case "expr":
				_managerGoalAuthoring.TrySetDraftExpression(actor, host, input.SafeRemainingArgument,
					out var expressionMessage);
				actor.OutputHandler.Send(expressionMessage);
				return;
			case "authority":
			case "authorities":
				_managerGoalAuthoring.TrySetDraftAuthority(actor, host, input, out var authorityMessage);
				actor.OutputHandler.Send(authorityMessage);
				return;
			case "condition":
			case "when":
				_managerGoalAuthoring.TryAddCondition(actor, host, input, out var conditionMessage);
				actor.OutputHandler.Send(conditionMessage);
				return;
			case "step":
			case "action":
			case "do":
				_managerGoalAuthoring.TryAddStep(actor, host, input, out var stepMessage);
				actor.OutputHandler.Send(stepMessage);
				return;
			case "removecondition":
			case "removecond":
			case "rmcondition":
			case "rmcond":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var conditionNumber))
				{
					actor.OutputHandler.Send("Which condition number do you want to remove from the manager goal draft?");
					return;
				}

				_managerGoalAuthoring.TryRemoveCondition(actor, host, (int)conditionNumber, out var removeConditionMessage);
				actor.OutputHandler.Send(removeConditionMessage);
				return;
			case "removestep":
			case "rmstep":
			case "removeaction":
			case "rmaction":
				if (input.IsFinished || !TryParseCommandNumber(input.PopSpeech(), out var stepNumber))
				{
					actor.OutputHandler.Send("Which action step number do you want to remove from the manager goal draft?");
					return;
				}

				_managerGoalAuthoring.TryRemoveStep(actor, host, (int)stepNumber, out var removeStepMessage);
				actor.OutputHandler.Send(removeStepMessage);
				return;
			case "discard":
			case "cancel":
			case "clear":
				_managerGoalAuthoring.TryDiscardDraft(actor, host, out var discardMessage);
				actor.OutputHandler.Send(discardMessage);
				return;
			case "finalise":
			case "finalize":
			case "finish":
			case "done":
				_managerGoalAuthoring.TryFinaliseDraft(actor, host, out _, out var finaliseMessage);
				actor.OutputHandler.Send(finaliseMessage);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleBoard(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		if (!CanViewOperational(actor, host))
		{
			actor.OutputHandler.Send($"You are not an employee of {host.EmploymentHostName.ColourName()}.");
			return;
		}

		var boardCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (boardCommand)
		{
			case "":
			case "posts":
			case "list":
				actor.OutputHandler.Send(RenderBoard(actor, host));
				return;
			case "read":
			case "view":
				if (input.IsFinished || !int.TryParse(input.PopSpeech(), out var postNumber))
				{
					actor.OutputHandler.Send("Which employment board post do you want to read?");
					return;
				}

				actor.OutputHandler.Send(RenderBoardPost(actor, host, postNumber));
				return;
			case "write":
			case "post":
				BeginBoardPost(actor, host, input);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void BeginBoardPost(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		if (!CanPostToBoard(actor, host))
		{
			actor.OutputHandler.Send($"You are not authorised to post official notices for {host.EmploymentHostName.ColourName()}.");
			return;
		}

		if (input.IsFinished)
		{
			actor.OutputHandler.Send("What title do you want to give to your employment board post?");
			return;
		}

		var title = input.SafeRemainingArgument.Trim();
		if (title.Length > 200)
		{
			actor.OutputHandler.Send("Employment board post titles must be under 200 characters in length.");
			return;
		}

		actor.OutputHandler.Send("Enter the employment board post in the editor below.");
		actor.EditorMode(PostEmploymentBoardPost, CancelEmploymentBoardPost, 1.0,
			suppliedArguments: new object[] { actor, host, title });
	}

	private void SendOperationalView(ICharacter actor, IEmploymentHost host,
		Func<ICharacter, IEmploymentHost, string> renderer)
	{
		if (!CanViewOperational(actor, host))
		{
			actor.OutputHandler.Send($"You are not an employee of {host.EmploymentHostName.ColourName()}.");
			return;
		}

		actor.OutputHandler.Send(renderer(actor, host));
	}

	private static void CancelEmploymentBoardPost(IOutputHandler handler, object[] args)
	{
		handler.Send("You decide not to make an employment board post.");
	}

	private static void PostEmploymentBoardPost(string text, IOutputHandler handler, object[] args)
	{
		var actor = (ICharacter)args[0];
		var host = (IEmploymentHost)args[1];
		var title = (string)args[2];
		var service = new EmploymentCommandService();
		service.TryPostBoardPost(actor, host, title, text, out var message);
		handler.Send(message);
	}

	private static IEmploymentContract? ActiveContractFor(ICharacter actor, IEmploymentHost host)
	{
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return host.EmploymentContracts.FirstOrDefault(x =>
			x.Employee.Id == actorIdentityId &&
			x.Status == EmploymentStatus.Active);
	}

	private static bool TryRequireAuthority(ICharacter actor, IEmploymentHost host, EmploymentAuthority authority,
		out string message)
	{
		if (actor.IsAdministrator())
		{
			message = string.Empty;
			return true;
		}

		if (host.HasAuthority(actor, authority))
		{
			message = string.Empty;
			return true;
		}

		message = $"You do not have the delegated {authority.DescribeEnum().ColourName()} authority for {host.EmploymentHostName.ColourName()}.";
		return false;
	}

	private static ICurrency? ResolveHostCurrency(IEmploymentHost host)
	{
		return host switch
		{
			IShop shop => shop.Currency,
			IAuctionHouse auctionHouse => auctionHouse.EconomicZone.Currency,
			ICombatArena arena => arena.Currency,
			IBank bank => bank.PrimaryCurrency,
			IStable stable => stable.Currency,
			IHotel hotel => hotel.Currency,
			IClan clan => clan.ClanBankAccount?.Currency ?? ResolveContractCurrency(host),
			_ => ResolveContractCurrency(host)
		};
	}

	private static ICurrency? ResolveContractCurrency(IEmploymentHost host)
	{
		return host.EmploymentContracts
		           .Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
		           .FirstOrDefault(x => x is not null);
	}

	private static EmploymentAuthoritySet DefaultOpeningAuthority(EmploymentRole role)
	{
		return DefaultRoleAuthority(role);
	}

	private static EmploymentAuthoritySet DefaultDirectHireAuthority(EmploymentRole role)
	{
		return DefaultRoleAuthority(role);
	}

	private static EmploymentAuthoritySet DefaultRoleAuthority(EmploymentRole role)
	{
		return role switch
		{
			EmploymentRole.Proprietor => EmploymentAuthoritySet.All,
			EmploymentRole.Manager => DefaultManagerAuthority(),
			EmploymentRole.Employee or
			EmploymentRole.Clerk or
			EmploymentRole.Courier or
			EmploymentRole.StableHand or
			EmploymentRole.HotelWorker => new EmploymentAuthoritySet(EmploymentAuthority.ManageDeliveryRoutes),
			EmploymentRole.Crafter => new EmploymentAuthoritySet(
				EmploymentAuthority.ManageCraftRules |
				EmploymentAuthority.ManageDeliveryRoutes),
			EmploymentRole.BankTeller => new EmploymentAuthoritySet(
				EmploymentAuthority.DepositBusinessCash |
				EmploymentAuthority.WithdrawBusinessCash),
			_ => EmploymentAuthoritySet.Empty
		};
	}

	private static EmploymentAuthoritySet DefaultManagerAuthority()
	{
		return new EmploymentAuthoritySet(
			EmploymentAuthority.ViewEmployees |
			EmploymentAuthority.HireEmployees |
			EmploymentAuthority.FireEmployees |
			EmploymentAuthority.CreateJobOpenings |
			EmploymentAuthority.ModifyJobOpenings |
			EmploymentAuthority.SetPayWithinBand |
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CancelTasks |
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.ManageCraftRules |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.AdjustPrices |
			EmploymentAuthority.ManagePayroll |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ModerateHostBoard);
	}

	private static CompensationTerms UnpaidCompensation()
	{
		return new CompensationTerms(
			null,
			null,
			PayCadence.Unpaid,
			null,
			PaymentSource.HostCash);
	}

	private bool TryTerminateContractsForEmployee(ICharacter actor, IEmploymentHost host,
		IReadOnlyCollection<IEmploymentContract> contracts, out string message)
	{
		if (!TryRequireAuthority(actor, host, EmploymentAuthority.FireEmployees, out message))
		{
			return false;
		}

		if (!contracts.Any())
		{
			message = "There is no active employment contract for that employee.";
			return false;
		}

		try
		{
			foreach (var contract in contracts)
			{
				host.Fire(contract, EmploymentTerminationReason.Fired, actor);
			}

			var employee = contracts.First().Employee;
			message = $"You terminate {employee.HowSeen(actor, colour: false).ColourName()}'s active employment contracts for {host.EmploymentHostName.ColourName()}.";
			return true;
		}
		catch (InvalidOperationException ex)
		{
			message = ex.Message;
			return false;
		}
	}

	private static string DescribeTaskEligibility(IEmploymentHost host, IEmploymentActiveTask task,
		IEmploymentContract contract, ICharacter voyeur)
	{
		var reasons = new List<string>();
		if (task.AssignedEmployee is not null && task.AssignedEmployee.Id != contract.Employee.Id)
		{
			reasons.Add($"already assigned to {task.AssignedEmployee.HowSeen(voyeur, colour: false)}");
		}

		if (!contract.Authority.ContainsAll(task.ActionPlan.RequiredAuthority))
		{
			reasons.Add($"lacks {MissingAuthority(contract.Authority, task.ActionPlan.RequiredAuthority).DescribeEnum()} authority");
		}

		var workerAis = (contract.Employee as INPC)?.AIs.OfType<EmploymentWorkerAI>().ToList() ?? [];
		if (!workerAis.Any())
		{
			reasons.Add("has no EmploymentWorkerAI");
		}

		foreach (var ai in workerAis)
		{
			var aiReasons = new List<string>();
			if (!ai.TaskingEnabled)
			{
				aiReasons.Add("tasking disabled");
			}

			if (ai.HostTypeFilter is not null && ai.HostTypeFilter.Value != host.EmploymentHostType)
			{
				aiReasons.Add($"host filter is {ai.HostTypeFilter.Value.DescribeEnum()}");
			}

			var missingCapabilities = task.ActionPlan.RequiredCapabilities
			                          .Where(x => !ai.Capabilities.Contains(x))
			                          .ToList();
			if (missingCapabilities.Any())
			{
				aiReasons.Add($"missing {missingCapabilities.Select(x => x.DescribeEnum()).ListToString()} capability");
			}

			if (!aiReasons.Any() && NextStep(task) is { } step)
			{
				var context = new EmploymentTaskContext(host, usePhysicalItemMovement: true);
				if (!step.CanExecute(context, contract.Employee, out var reason))
				{
					aiReasons.Add(reason);
				}
			}

			reasons.Add(aiReasons.Any()
				? $"{ai.Name} AI: {aiReasons.ListToString()}"
				: $"{ai.Name} AI eligible on its next minute tick");
		}

		return reasons.Any()
			? reasons.ListToString().Colour(Telnet.Yellow)
			: "eligible".Colour(Telnet.Green);
	}

	private static EmploymentAuthority MissingAuthority(EmploymentAuthoritySet actual, EmploymentAuthoritySet required)
	{
		return required.Authorities & ~actual.Authorities;
	}

	private static string DescribeCapabilities(IReadOnlySet<EmploymentAICapability> capabilities)
	{
		return capabilities.Any()
			? capabilities.Select(x => x.DescribeEnum().ColourName()).ListToString()
			: "none".ColourValue();
	}

	private static IEmploymentActionStep? NextStep(IEmploymentActiveTask task)
	{
		var index = task.StepStates.ToList()
		                .FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.Blocked);
		return index < 0 || index >= task.ActionPlan.Steps.Count ? null : task.ActionPlan.Steps[index];
	}

	private static IEmploymentActiveTask? ActiveTaskBySelector(IEmploymentHost host, string selector)
	{
		var tasks = host.TaskBoard.ActiveTasks
		                .OrderBy(x => x.Name)
		                .ToList();
		if (!tasks.Any())
		{
			return null;
		}

		if (TryParseCommandNumber(selector, out var number))
		{
			return number > 0 && number <= tasks.Count ? tasks[(int)number - 1] : null;
		}

		if (Guid.TryParse(selector, out var id))
		{
			return tasks.FirstOrDefault(x => x.Id == id);
		}

		return tasks.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       tasks.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IEmploymentScheduledTaskRule? ScheduledRuleBySelector(IEmploymentHost host, string selector)
	{
		var rules = host.TaskBoard.ScheduledRules
		                .OrderBy(x => x.Name)
		                .ToList();
		if (!rules.Any())
		{
			return null;
		}

		if (TryParseCommandNumber(selector, out var number))
		{
			return number > 0 && number <= rules.Count ? rules[(int)number - 1] : null;
		}

		if (Guid.TryParse(selector, out var id))
		{
			return rules.FirstOrDefault(x => x.Id == id);
		}

		return rules.FirstOrDefault(x => x.Name.EqualTo(selector)) ??
		       rules.FirstOrDefault(x => x.Name.StartsWith(selector, StringComparison.InvariantCultureIgnoreCase));
	}

	private static IEnumerable<IBoardPost> OrderedBoardPosts(IEmploymentHost host)
	{
		return host.Board.Posts.OrderBy(x => x.PostTime).ThenBy(x => x.Id);
	}

	private static IEmploymentApplication? ApplicationById(IEmploymentHost host, long applicationId)
	{
		return applicationId <= 0
			? null
			: host.Employment.Applications
			      .FirstOrDefault(x => x.Id == applicationId);
	}

	private static List<IEmploymentPayable> SelectPayables(IReadOnlyList<IEmploymentPayable> payables, string selector)
	{
		selector = selector.Trim();
		if (selector.EqualTo("all"))
		{
			return payables.ToList();
		}

		if (!TryParseCommandNumber(selector, out var id))
		{
			return [];
		}

		return payables.Where(x => x.Id == id).ToList();
	}

	private static IEmploymentContract? ContractById(IEmploymentHost host, long contractId)
	{
		return contractId <= 0
			? null
			: host.EmploymentContracts
			      .FirstOrDefault(x => x.Id == contractId);
	}

	private static bool TryParseCommandNumber(string text, out long number)
	{
		text = text.Trim();
		if (text.StartsWith("#", StringComparison.Ordinal))
		{
			text = text[1..];
		}

		return long.TryParse(text, out number);
	}

	private static bool TryParseAuthoritySet(StringStack input, out EmploymentAuthoritySet authority, out string error,
		bool allowNone = false)
	{
		authority = EmploymentAuthoritySet.Empty;
		error = string.Empty;
		if (input.IsFinished)
		{
			error = "Which delegated authority do you want to use?";
			return false;
		}

		var authorities = EmploymentAuthority.None;
		while (!input.IsFinished)
		{
			var token = input.PopSpeech();
			foreach (var part in token.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			{
				if (part.EqualTo("all"))
				{
					authority = EmploymentAuthoritySet.All;
					return true;
				}

				if (part.EqualTo("none") || part.EqualTo("clear"))
				{
					if (!allowNone)
					{
						error = "Use a specific authority name, or use the set syntax if you want to clear authority.";
						return false;
					}

					authorities = EmploymentAuthority.None;
					continue;
				}

				if (!TryParseAuthority(part, out var parsed))
				{
					error = $"Unknown delegated authority {part.ColourCommand()}. Try {"contracts delegate #1 help".ColourCommand()} for the list.";
					return false;
				}

				authorities |= parsed;
			}
		}

		if (authorities == EmploymentAuthority.None && !allowNone)
		{
			error = "Which delegated authority do you want to use?";
			return false;
		}

		authority = new EmploymentAuthoritySet(authorities);
		return true;
	}

	private static bool TryParseAuthority(string text, out EmploymentAuthority authority)
	{
		if (text.TryParseEnum(out authority) && authority != EmploymentAuthority.None)
		{
			return true;
		}

		authority = text.CollapseString().ToLowerInvariant() switch
		{
			"view" or "viewemployees" or "employees" => EmploymentAuthority.ViewEmployees,
			"hire" or "hiring" => EmploymentAuthority.HireEmployees,
			"fire" or "firing" => EmploymentAuthority.FireEmployees,
			"openings" or "createopenings" or "createjobopenings" => EmploymentAuthority.CreateJobOpenings,
			"modifyopenings" or "modifyjobopenings" => EmploymentAuthority.ModifyJobOpenings,
			"pay" or "setpay" or "setpaywithinband" => EmploymentAuthority.SetPayWithinBand,
			"tasks" or "task" or "assign" or "assigntasks" => EmploymentAuthority.AssignTasks,
			"canceltasks" or "cancel" => EmploymentAuthority.CancelTasks,
			"rules" or "schedulerules" or "createscheduledrules" => EmploymentAuthority.CreateScheduledRules,
			"modifyrules" or "modifyscheduledrules" => EmploymentAuthority.ModifyScheduledRules,
			"goals" or "creategoals" or "createmanagergoals" => EmploymentAuthority.CreateManagerGoals,
			"modifygoals" or "modifymanagergoals" => EmploymentAuthority.ModifyManagerGoals,
			"purchases" or "purchase" or "approvepurchases" => EmploymentAuthority.ApprovePurchases,
			"storeaccount" or "usestoreaccount" => EmploymentAuthority.UseStoreAccount,
			"cashwithdraw" or "withdraw" or "withdrawbusinesscash" => EmploymentAuthority.WithdrawBusinessCash,
			"cashdeposit" or "deposit" or "depositbusinesscash" => EmploymentAuthority.DepositBusinessCash,
			"stock" or "stockrules" or "managestockrules" => EmploymentAuthority.ManageStockRules,
			"craft" or "crafting" or "craftrules" or "managecraftrules" => EmploymentAuthority.ManageCraftRules,
			"delivery" or "deliver" or "routes" or "managedeliveryroutes" => EmploymentAuthority.ManageDeliveryRoutes,
			"prices" or "pricing" or "adjustprices" => EmploymentAuthority.AdjustPrices,
			"tax" or "taxes" or "paytaxes" => EmploymentAuthority.PayTaxes,
			"board" or "postboard" or "posttohostboard" => EmploymentAuthority.PostToHostBoard,
			"moderateboard" or "moderatehostboard" => EmploymentAuthority.ModerateHostBoard,
			"payroll" or "wages" or "managepayroll" => EmploymentAuthority.ManagePayroll,
			_ => EmploymentAuthority.None
		};

		return authority != EmploymentAuthority.None;
	}

	private static bool TryParseOpeningCompensation(StringStack input, ICurrency currency, out decimal hourlyRate,
		out int maxPositions, out string error)
	{
		hourlyRate = 0.0M;
		maxPositions = 1;
		error = string.Empty;

		if (input.IsFinished)
		{
			error = "What positive hourly rate should this opening advertise?";
			return false;
		}

		var text = input.SafeRemainingArgument;
		if (currency.TryGetBaseCurrency(text, out hourlyRate))
		{
			return true;
		}

		var parts = new StringStack(text).PopSpeechAll().ToList();
		if (parts.Count < 2 || !int.TryParse(parts[^1], out maxPositions) || maxPositions <= 0)
		{
			error = $"What positive hourly rate in {currency.Name.ColourName()} should this opening advertise?";
			return false;
		}

		var amountText = string.Join(" ", parts.Take(parts.Count - 1));
		if (currency.TryGetBaseCurrency(amountText, out hourlyRate))
		{
			return true;
		}

		error = $"What positive hourly rate in {currency.Name.ColourName()} should this opening advertise?";
		return false;
	}

	private static string DescribeCompensation(CompensationTerms compensation, ICharacter actor)
	{
		return compensation.Cadence == PayCadence.Unpaid
			? "unpaid".ColourError()
			: $"{DescribeMoney(compensation.FixedRate ?? compensation.MinimumEffectivePay, actor)} {compensation.Cadence.DescribeEnum()}";
	}

	private static string DescribeMoney(MoneyAmount? amount, ICharacter actor)
	{
		if (amount is null)
		{
			return "no amount".ColourError();
		}

		return amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal).ColourValue();
	}

	private static string DescribeAuthoritySet(EmploymentAuthoritySet authority)
	{
		var authorities = EmploymentAuthorityValues()
		                  .Where(authority.Contains)
		                  .Select(x => x.DescribeEnum().ColourName())
		                  .ToList();
		return authorities.Any()
			? authorities.ListToString()
			: "none".ColourValue();
	}

	private static IEnumerable<EmploymentAuthority> EmploymentAuthorityValues()
	{
		return Enum.GetValues<EmploymentAuthority>()
		           .Where(x => x != EmploymentAuthority.None);
	}

	private static string RenderContractAuthority(ICharacter actor, IEmploymentContract contract)
	{
		var sb = new StringBuilder();
		sb.AppendLine($"Delegated authority for contract #{contract.Id.ToString("N0", actor).ColourValue()}:");
		sb.AppendLine($"Employee: {contract.Employee.HowSeen(actor, colour: false).ColourName()}");
		sb.AppendLine($"Role: {contract.Role.DescribeEnum().ColourName()}");
		sb.AppendLine($"Status: {contract.Status.DescribeEnum().ColourValue()}");
		sb.AppendLine($"Authority: {DescribeAuthoritySet(contract.Authority)}");
		return sb.ToString();
	}

	private static string RenderContractAuthorities(ICharacter actor, IEmploymentHost host)
	{
		if (!host.EmploymentContracts.Any())
		{
			return $"{host.EmploymentHostName.ColourName()} has no employment contracts.";
		}

		var sb = new StringBuilder();
		sb.AppendLine($"Delegated authorities for {host.EmploymentHostName.ColourName()}:");
		foreach (var contract in host.EmploymentContracts.OrderBy(x => x.Employee.Name).ThenBy(x => x.Role))
		{
			sb.AppendLine($"\t#{contract.Id.ToString("N0", actor)} - {contract.Employee.HowSeen(actor, colour: false).ColourName()} - {contract.Role.DescribeEnum().ColourName()} - {contract.Status.DescribeEnum().ColourValue()} - {DescribeAuthoritySet(contract.Authority)}");
		}

		return sb.ToString();
	}

	private static string RenderAuthorityHelp(ICharacter actor)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Employment Delegation Commands".GetLineWithTitle(actor, Telnet.Cyan, Telnet.BoldWhite));
		sb.AppendLine();
		sb.AppendLine($"{"contracts delegate <#> show".ColourCommand()} - shows delegated authority for a contract");
		sb.AppendLine($"{"contracts delegate <#> grant <authority...>".ColourCommand()} - grants delegated authority");
		sb.AppendLine($"{"contracts delegate <#> revoke <authority...>".ColourCommand()} - revokes delegated authority");
		sb.AppendLine($"{"contracts delegate <#> set <none|all|authority...>".ColourCommand()} - replaces delegated authority");
		sb.AppendLine();
		sb.AppendLine("Authorities:");
		foreach (var authority in EmploymentAuthorityValues())
		{
			sb.AppendLine($"\t{authority.DescribeEnum().ColourName()}");
		}

		sb.AppendLine();
		sb.AppendLine("Managers need HireEmployees authority and may only change authorities they already possess. Admins bypass those restrictions.");
		return sb.ToString();
	}

	private static string DescribeNextStep(IEmploymentActiveTask task, ICharacter actor)
	{
		var index = task.StepStates.ToList().FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.Blocked);
		if (index < 0)
		{
			return "none".ColourError();
		}

		return $"{(index + 1).ToString("N0", actor)} - {EmploymentTaskAuthoringService.DescribeStep(task.ActionPlan.Steps[index], actor)}";
	}

	public const string EmploymentHelp = @"You can use the following options with the employment command:

	#3employment <host type> <host> <subcommand>#0 - targets a specific employment host

Employment records:

	#3employment <host type> <host> status#0 - shows employment status
	#3employment <host type> <host> contracts#0 - lists employment contracts
	#3employment <host type> <host> contracts fire <##>#0 - terminates an active employment contract
	#3employment <host type> <host> contracts delegate <##> show|grant|revoke|set ...#0 - views or changes delegated authority
	#3employment <host type> <host> openings#0 - lists visible job openings
	#3employment <host type> <host> openings create <role> <hourly rate> [positions]#0 - creates an NPC-facing opening
	#3employment <host type> <host> applications#0 - lists applications
	#3employment <host type> <host> applications accept|reject <##> [reason]#0 - accepts or rejects a pending application
	#3employment <host type> <host> payroll#0 - lists wage payables and overdue days
	#3employment <host type> <host> payroll run|settle|claim ...#0 - accrues, settles, or claims employment wage payables

Active tasks:

	#3employment <host type> <host> tasks#0 - lists scheduled rules and active tasks
	#3employment <host type> <host> tasks show <##|name>#0 - shows an active task with its step details
	#3employment <host type> <host> tasks diagnose#0 - explains why active employees can or cannot auto-claim tasks
	#3employment <host type> <host> tasks cancel <##|name> [reason]#0 - cancels a pending, assigned, in-progress, or blocked active task
	#3employment <host type> <host> tasks create <name> <action> [then <action> ...]#0 - creates and finalises a task in one command
	#3employment <host type> <host> tasks draft new|show|rename|remove|discard|finalise ...#0 - drafts and finalises active tasks
	#3employment <host type> <host> tasks step <action syntax>#0 - adds an action to your current active-task draft
	#3employment <host type> <host> tasks actions [all|category|action]#0 - shows task action catalogue entries, status, and syntax

Scheduled rules:

	#3employment <host type> <host> tasks rule show <##|name>#0 - shows a scheduled rule with conditions and planned steps
	#3employment <host type> <host> tasks rule create <name> cooldown <timespan> when <condition> [and <condition> ...] do <action> [then <action> ...]#0 - creates a scheduled rule in one command
	#3employment <host type> <host> tasks rule draft new|copy|show|key|cooldown|expression|removecondition|removestep|discard|finalise ...#0 - drafts and finalises scheduled rules
	#3employment <host type> <host> tasks rule condition <condition>#0 - adds a condition to the current scheduled-rule draft
	#3employment <host type> <host> tasks rule step <action syntax>#0 - adds an action step to the current scheduled-rule draft
	#3employment <host type> <host> tasks rule predicate list|show|create|copy|cancel ...#0 - manages reusable named condition predicates
	#3employment <host type> <host> tasks rule template list|show|save|draft|cancel ...#0 - manages reusable scheduled-rule templates
	#3employment <host type> <host> tasks rule diagnose <##|name> [manual <key>]#0 - evaluates rule blockers without spawning
	#3employment <host type> <host> tasks rule evaluate <##|name|all> [manual <key>]#0 - manually evaluates scheduled rules for testing
	#3employment <host type> <host> tasks rule pause|resume <##|name> [reason]#0 - pauses or resumes a scheduled rule
	#3employment <host type> <host> tasks rule cancel <##|name> [reason]#0 - cancels and removes a scheduled rule
	#3employment <host type> <host> tasks conditions [all|category|condition]#0 - shows scheduled-rule condition syntax and authority

Communication and audit:

	#3employment <host type> <host> goals#0 - lists and manages manager goals
	#3employment <host type> <host> goals types [all|category|type]#0 - shows manager goal type help
	#3employment <host type> <host> goals show <##|type|description>#0 - shows a manager goal
	#3employment <host type> <host> goals draft new|copy|show|type|description|priority|cadence|budget|risk|expression|authority|removecondition|removestep|discard|finalise ...#0 - drafts and edits manager goals
	#3employment <host type> <host> goals condition <condition>#0 - adds a condition to the current manager-goal draft
	#3employment <host type> <host> goals step <action syntax>#0 - adds an action step to the current manager-goal draft
	#3employment <host type> <host> goals cancel <##|type|description> [reason]#0 - cancels a manager goal
	#3employment <host type> <host> goals evaluate#0 - manually evaluates manager goals for testing
	#3employment <host type> <host> register#0 - shows recent employment register entries
	#3employment <host type> <host> employmentledger|empledger#0 - shows recent employment ledger entries
	#3employment <host type> <host> board [read <##>|write <title>]#0 - uses the staff communication board

Host types are #3shop#0, #3auction#0, #3arena#0, #3bank#0, #3stable#0, #3hotel#0, and #3clan#0. Hotel hosts are resolved by property id or name; clan hosts are resolved by clan id, name, full name, or alias.
Staff boards are only for employee communication; active tasks, scheduled tasks, and manager goals are routed through the employment task board. Use #3tasks actions#0 and #3tasks conditions#0 for the full action and condition catalogues. Item selectors use bare prototype ids, #3*item ids#0 for specific live items, #3&tag#0 for verified tags, and bare text for a visible keyword target. Scheduled-rule drafts can combine conditions with #3and#0, #3or#0, #3not#0, parentheses, numbered condition references such as #3#1#0, and named predicates such as #3@restock-window#0.";
}
