using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MudSharp.Arenas;
using MudSharp.Character;
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

		var host = hostType.CollapseString().ToLowerInvariant() switch
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
	private readonly IEmploymentHostResolver _resolver;
	private readonly EmploymentTaskAuthoringService _taskAuthoring;

	public EmploymentCommandService(IEmploymentHostResolver? resolver = null,
		EmploymentTaskAuthoringService? taskAuthoring = null)
	{
		_resolver = resolver ?? new EmploymentHostResolver();
		_taskAuthoring = taskAuthoring ?? new EmploymentTaskAuthoringService();
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
			case "tasks":
			case "task":
				HandleTasks(actor, host, input);
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
			"tasks" or "task" or
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

	public bool CanViewOpenings(ICharacter actor, IEmploymentHost host)
	{
		return CanViewOperational(actor, host) || host.JobOpenings.Any(x => x.Status == JobOpeningStatus.Open);
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
		return TryTerminateContractsForEmployee(actor, host,
			host.ActiveEmploymentContracts().Where(x => x.Employee.Id == target.Id).ToList(), out message);
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
		var existing = host.ActiveEmploymentContracts()
		                   .FirstOrDefault(x => x.Employee.Id == target.Id && x.Role == role);
		if (existing is not null)
		{
			return TryTerminateContract(actor, host, existing.Id, out message);
		}

		return TryHireDirectContract(actor, host, target, role, out _, out message);
	}

	public bool TryResignFromHost(ICharacter actor, IEmploymentHost host, out string message)
	{
		var contracts = host.ActiveEmploymentContracts()
		                    .Where(x => x.Employee.Id == actor.Id)
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

	public string RenderTasks(ICharacter actor, IEmploymentHost host)
	{
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
			foreach (var rule in host.TaskBoard.ScheduledRules.OrderBy(x => x.Name))
			{
				sb.AppendLine($"\t{rule.Name.ColourName()} - {rule.Conditions.Count.ToString("N0", actor)} condition{(rule.Conditions.Count == 1 ? string.Empty : "s")} - {rule.ActionPlan.Steps.Count.ToString("N0", actor)} step{(rule.ActionPlan.Steps.Count == 1 ? string.Empty : "s")}");
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
			foreach (var task in host.TaskBoard.ActiveTasks.OrderBy(x => x.Name))
			{
				sb.AppendLine($"\t{task.Name.ColourName()} - {task.Status.DescribeEnum().ColourValue()} - assigned to {task.AssignedEmployee?.HowSeen(actor, colour: false).ColourName() ?? "nobody".ColourError()} - next step {DescribeNextStep(task, actor)}");
			}
		}

		return sb.ToString();
	}

	public string RenderTaskDiagnostics(ICharacter actor, IEmploymentHost host)
	{
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
			sb.AppendLine($"\t{entry.RecordedAt.ToString("g", actor)} - {entry.EntryType.DescribeEnum().ColourName()} - {entry.Actor?.HowSeen(actor, colour: false).ColourName() ?? "System".ColourName()} - {entry.Description}");
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
			sb.AppendLine($"\t{entry.RecordedAt.ToString("g", actor)} - {entry.EntryType.DescribeEnum().ColourName()} - {DescribeMoney(entry.Amount, actor)} - {entry.Actor?.HowSeen(actor, colour: false).ColourName() ?? "System".ColourName()} - {entry.Description}");
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

	private void HandleTasks(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var taskCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (taskCommand)
		{
			case "":
			case "list":
			case "show":
				SendOperationalView(actor, host, RenderTasks);
				return;
			case "draft":
				HandleTaskDraft(actor, host, input);
				return;
			case "actions":
			case "action":
				actor.OutputHandler.Send(_taskAuthoring.RenderAvailableActions(actor));
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
			case "show":
				SendOperationalView(actor, host, RenderTasks);
				return;
		}

		actor.OutputHandler.Send(EmploymentHelp.SubstituteANSIColour());
	}

	private void HandleGoals(ICharacter actor, IEmploymentHost host, StringStack input)
	{
		var goalCommand = input.PopSpeech().CollapseString().ToLowerInvariant();
		switch (goalCommand)
		{
			case "":
			case "list":
			case "show":
				SendOperationalView(actor, host, RenderGoals);
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
		return host.EmploymentContracts.FirstOrDefault(x =>
			x.Employee.Id == actor.Id &&
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
			_ => host.EmploymentContracts.Select(x => x.Compensation.FixedRate?.Currency ?? x.Compensation.MinimumEffectivePay?.Currency)
			         .FirstOrDefault(x => x is not null)
		};
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

	private static IEmploymentActionStep? NextStep(IEmploymentActiveTask task)
	{
		var index = task.StepStates.ToList()
		                .FindIndex(x => x is EmploymentActionStepStatus.Pending or EmploymentActionStepStatus.Blocked);
		return index < 0 || index >= task.ActionPlan.Steps.Count ? null : task.ActionPlan.Steps[index];
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

		return $"{(index + 1).ToString("N0", actor)} ({task.ActionPlan.Steps[index].StepType.DescribeEnum()})";
	}

	public const string EmploymentHelp = @"You can use the following options with the employment command:

	#3employment <host type> <host> status#0 - shows employment status
	#3employment <host type> <host> contracts#0 - lists employment contracts
	#3employment <host type> <host> contracts fire <##>#0 - terminates an active employment contract
	#3employment <host type> <host> contracts delegate <##> show#0 - shows delegated authority for a contract
	#3employment <host type> <host> contracts delegate <##> grant|revoke <authority...>#0 - grants or revokes delegated authority
	#3employment <host type> <host> contracts delegate <##> set <none|all|authority...>#0 - replaces delegated authority
	#3employment <host type> <host> openings#0 - lists visible job openings
	#3employment <host type> <host> openings create <role> <hourly rate> [positions]#0 - creates an NPC-facing opening
	#3employment <host type> <host> applications#0 - lists applications
	#3employment <host type> <host> applications accept <##>#0 - accepts a pending application into an active contract
	#3employment <host type> <host> applications reject <##> <reason>#0 - rejects a pending application
	#3employment <host type> <host> tasks#0 - lists scheduled rules and active tasks
	#3employment <host type> <host> tasks diagnose#0 - explains why active employees can or cannot auto-claim tasks
	#3employment <host type> <host> tasks draft new <name>#0 - starts a transient active task draft
	#3employment <host type> <host> tasks draft show#0 - reviews your current draft
	#3employment <host type> <host> tasks draft rename <name>#0 - renames your current draft
	#3employment <host type> <host> tasks draft remove <##>#0 - removes a draft step
	#3employment <host type> <host> tasks draft discard#0 - discards your current draft
	#3employment <host type> <host> tasks actions#0 - shows task step actions and syntax
	#3employment <host type> <host> tasks step getid <quantity> <item ids...> from <here|cell ids...>#0 - adds an item-id retrieval step
	#3employment <host type> <host> tasks step gettag <quantity> <tag> from <here|cell ids...>#0 - adds a tagged-item retrieval step
	#3employment <host type> <host> tasks step commodity <weight> <material> [tag <tag>] from <here|cell ids...> [char <name>=<value> ...]#0 - adds a commodity retrieval step
	#3employment <host type> <host> tasks step deliver to <here|cell id> [container <item id>|containertag <tag>]#0 - adds a delivery step
	#3employment <host type> <host> tasks draft finalise#0 - creates the active task through the employment task board
	#3employment <host type> <host> goals#0 - lists manager goals
	#3employment <host type> <host> register#0 - shows recent employment register entries
	#3employment <host type> <host> employmentledger|empledger#0 - shows recent employment ledger entries
	#3employment <host type> <host> board#0 - lists employment board posts
	#3employment <host type> <host> board read <##>#0 - reads an employment board post
	#3employment <host type> <host> board write <title>#0 - writes an employment board post

Host types are #3shop#0, #3auction#0, #3arena#0, #3bank#0, #3stable#0, and #3hotel#0. Hotel hosts are resolved by property id or name.";
}
