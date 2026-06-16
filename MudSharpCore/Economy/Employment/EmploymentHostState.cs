using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community.Boards;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

#nullable enable

namespace MudSharp.Economy.Employment;

public sealed class EmploymentHostState : IEmploymentHostState
{
	private readonly List<IEmploymentContract> _contracts = new();
	private readonly List<IJobOpening> _jobOpenings = new();
	private readonly List<IEmploymentApplication> _applications = new();
	private readonly IEmploymentPersistenceStore? _persistence;
	private long _nextContractId;
	private long _nextJobOpeningId;
	private long _nextApplicationId;
	private long _nextPayableId;

	public EmploymentHostState(IEmploymentHost host, IBoard? board = null)
	{
		Host = host;
		EmploymentRegister = new EmploymentRegister(host);
		BusinessLedger = new EmploymentLedger(host);
		Board = board ?? new EmploymentHostBoard(host);
		TaskBoard = new EmploymentTaskBoard(host);
		ManagerGoalBoard = new ManagerGoalBoard(host);
		Payroll = new EmploymentPayroll(this);
	}

	internal EmploymentHostState(IEmploymentHost host, IBoard board, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentContract> contracts, IEnumerable<IJobOpening> jobOpenings,
		IEnumerable<IEmploymentApplication> applications, IEnumerable<IEmploymentLedgerEntry> ledgerEntries,
		IEnumerable<IEmploymentPayable> payables,
		IEnumerable<IEmploymentRegisterEntry> registerEntries, IEnumerable<IEmploymentScheduledTaskRule> scheduledRules,
		IEnumerable<IEmploymentActiveTask> activeTasks, IEnumerable<IManagerGoal> managerGoals,
		IEnumerable<IEmploymentConditionPredicate>? conditionPredicates = null,
		IEnumerable<IEmploymentScheduledRuleTemplate>? scheduledRuleTemplates = null)
	{
		Host = host;
		_persistence = persistence;
		_contracts.AddRange(contracts);
		_jobOpenings.AddRange(jobOpenings);
		_applications.AddRange(applications);
		_nextContractId = _contracts.Select(x => x.Id).DefaultIfEmpty().Max();
		_nextJobOpeningId = _jobOpenings.Select(x => x.Id).DefaultIfEmpty().Max();
		_nextApplicationId = _applications.Select(x => x.Id).DefaultIfEmpty().Max();
		_nextPayableId = payables.Select(x => x.Id).DefaultIfEmpty().Max();
		EmploymentRegister = new EmploymentRegister(host, persistence, registerEntries);
		BusinessLedger = new EmploymentLedger(host, persistence, ledgerEntries);
		Board = board;
		TaskBoard = new EmploymentTaskBoard(host, persistence, scheduledRules, activeTasks, conditionPredicates,
			scheduledRuleTemplates);
		ManagerGoalBoard = new ManagerGoalBoard(host, persistence, managerGoals);
		Payroll = new EmploymentPayroll(this, persistence, payables);
	}

	public IEmploymentHost Host { get; }
	public IEmploymentLedger BusinessLedger { get; }
	public IEmploymentRegister EmploymentRegister { get; }
	public IBoard Board { get; }
	public IEmploymentTaskBoard TaskBoard { get; }
	public IManagerGoalBoard ManagerGoalBoard { get; }
	public IEmploymentPayroll Payroll { get; }
	public IReadOnlyCollection<IEmploymentContract> EmploymentContracts => _contracts;
	public IReadOnlyCollection<IJobOpening> JobOpenings => _jobOpenings;
	public IReadOnlyCollection<IEmploymentApplication> Applications => _applications;

	internal long NextPayableId()
	{
		return Interlocked.Increment(ref _nextPayableId);
	}

	public bool CanEmploy(ICharacter candidate, EmploymentRole role, out string reason)
	{
		if (candidate is null)
		{
			reason = "There is no candidate to employ.";
			return false;
		}

		var candidateIdentityId = CharacterInstanceIdentityComparer.IdentityId(candidate);
		if (_contracts.Any(x =>
			    x.Employee.Id == candidateIdentityId &&
			    x.Role == role &&
			    x.Status is EmploymentStatus.Active or EmploymentStatus.Suspended))
		{
			reason = $"{candidate.HowSeen(candidate, colour: false)} already has an active {role} contract with {Host.EmploymentHostName}.";
			return false;
		}

		reason = string.Empty;
		return true;
	}

	public IEmploymentContract Hire(ICharacter candidate, EmploymentOffer offer, ICharacter? authorisedBy)
	{
		if (!CanEmploy(candidate, offer.Role, out var reason))
		{
			throw new InvalidOperationException(reason);
		}

		if (authorisedBy is not null && !authorisedBy.IsAdministrator())
		{
			var authorisingAuthority = AuthorityFor(authorisedBy);
			if (!authorisingAuthority.Contains(EmploymentAuthority.HireEmployees))
			{
				throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to hire for {Host.EmploymentHostName}.");
			}

			if (!authorisingAuthority.ContainsAll(offer.Authority))
			{
				throw new InvalidOperationException("Managers cannot hire employees with authority that they do not personally possess.");
			}
		}

		if (offer.Compensation.IsPaid && offer.Compensation.NominalAmount <= 0.0M)
		{
			throw new InvalidOperationException("Paid employment contracts must have a positive effective rate.");
		}

		var contract = new EmploymentContract(
			Interlocked.Increment(ref _nextContractId),
			Host,
			candidate,
			offer.Role,
			EmploymentStatus.Active,
			offer.Authority,
			offer.Compensation,
			offer.Schedule,
			offer.Duration,
			offer.PaymentMethod,
			EmploymentClock.CurrentInstant(Host),
			null,
			null);
		_contracts.Add(contract);
		_persistence?.SaveContract(contract);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ContractHired,
			authorisedBy,
			$"Hired {candidate.HowSeen(candidate, colour: false)} as {offer.Role}.");
		Host.DebugEmployment(
			$"Hired {candidate.Name} as {offer.Role.DescribeEnum()} with {offer.Authority.Authorities.DescribeEnum()} authority.",
			candidate.Gameworld);
		return contract;
	}

	public void Fire(IEmploymentContract contract, EmploymentTerminationReason reason, ICharacter? authorisedBy)
	{
		if (contract is not EmploymentContract concrete || !ReferenceEquals(concrete.Employer, Host))
		{
			throw new InvalidOperationException("That employment contract does not belong to this host.");
		}

		if (authorisedBy is not null && !HasAuthority(authorisedBy, EmploymentAuthority.FireEmployees))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to fire employees for {Host.EmploymentHostName}.");
		}

		if (authorisedBy is not null && !authorisedBy.IsAdministrator())
		{
			if (concrete.Role == EmploymentRole.Proprietor)
			{
				throw new InvalidOperationException("Only an administrator can terminate a proprietor employment contract.");
			}

			if (concrete.Role == EmploymentRole.Manager &&
			    !Host.HasActiveEmploymentRole(authorisedBy, EmploymentRole.Proprietor))
			{
				throw new InvalidOperationException("Only a proprietor or administrator can terminate manager employment contracts.");
			}
		}

		if (concrete.Status == EmploymentStatus.Ended)
		{
			return;
		}

		concrete.End(reason, EmploymentClock.CurrentInstant(Host));
		_persistence?.SaveContractEnded(concrete);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ContractEnded,
			authorisedBy,
			$"Ended {concrete.Employee.HowSeen(concrete.Employee, colour: false)}'s {concrete.Role} contract: {reason}.");
		TaskBoard.AuditActiveTaskAssignments();
		Host.DebugEmployment(
			$"Ended {concrete.Employee.Name}'s {concrete.Role.DescribeEnum()} contract: {reason.DescribeEnum()}.",
			concrete.Employee.Gameworld);
	}

	public bool HasAuthority(ICharacter actor, EmploymentAuthority authority)
	{
		if (actor is null)
		{
			return false;
		}

		if (actor.IsAdministrator())
		{
			return true;
		}

		return AuthorityFor(actor).Contains(authority);
	}

	public void SetContractAuthority(IEmploymentContract contract, EmploymentAuthoritySet authority,
		ICharacter authorisedBy)
	{
		if (contract is not EmploymentContract concrete || !ReferenceEquals(concrete.Employer, Host))
		{
			throw new InvalidOperationException("That employment contract does not belong to this host.");
		}

		if (concrete.Status != EmploymentStatus.Active)
		{
			throw new InvalidOperationException("Only active employment contracts can have delegated authority changed.");
		}

		if (authorisedBy is null)
		{
			throw new InvalidOperationException("Changing delegated authority requires an authorised actor.");
		}

		if (!authorisedBy.IsAdministrator())
		{
			var authorisingAuthority = AuthorityFor(authorisedBy);
			if (!authorisingAuthority.Contains(EmploymentAuthority.HireEmployees))
			{
				throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to change employment delegations for {Host.EmploymentHostName}.");
			}

			if (!authorisingAuthority.ContainsAll(concrete.Authority))
			{
				throw new InvalidOperationException("Managers cannot alter authority that they do not personally possess.");
			}

			if (!authorisingAuthority.ContainsAll(authority))
			{
				throw new InvalidOperationException("Managers cannot delegate authority that they do not personally possess.");
			}
		}

		var oldAuthority = concrete.Authority;
		if (oldAuthority.Authorities == authority.Authorities)
		{
			return;
		}

		concrete.SetAuthority(authority);
		_persistence?.SaveContractAuthority(concrete);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.AuthorityChanged,
			authorisedBy,
			$"Changed {concrete.Employee.HowSeen(authorisedBy, colour: false)}'s {concrete.Role} authority from {oldAuthority.Authorities.DescribeEnum()} to {authority.Authorities.DescribeEnum()}.");
		Host.DebugEmployment(
			$"{authorisedBy.Name} changed contract #{concrete.Id:N0} authority from {oldAuthority.Authorities.DescribeEnum()} to {authority.Authorities.DescribeEnum()}.",
			authorisedBy.Gameworld);
	}

	public IJobOpening CreateJobOpening(JobOpeningDefinition definition, ICharacter? authorisedBy)
	{
		if (authorisedBy is not null && !authorisedBy.IsAdministrator())
		{
			var authorisingAuthority = AuthorityFor(authorisedBy);
			if (!authorisingAuthority.Contains(EmploymentAuthority.CreateJobOpenings))
			{
				throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create job openings for {Host.EmploymentHostName}.");
			}

			if (!authorisingAuthority.ContainsAll(definition.Authority))
			{
				throw new InvalidOperationException("Managers cannot create job openings that delegate authority they do not personally possess.");
			}
		}

		if (definition.MaxPositions <= 0)
		{
			throw new InvalidOperationException("Job openings must have at least one available position.");
		}

		if (definition.Compensation.IsPaid && definition.Compensation.NominalAmount <= 0.0M)
		{
			throw new InvalidOperationException("Paid job openings must advertise a positive effective rate.");
		}

		var opening = new JobOpening(
			Interlocked.Increment(ref _nextJobOpeningId),
			Host,
			definition.Role,
			definition.Requirements,
			definition.Compensation,
			definition.Schedule,
			definition.Duration,
			JobOpeningStatus.Open,
			definition.MaxPositions,
			definition.NpcApplicationsOnly,
			definition.PaymentMethod,
			definition.Authority);
		_jobOpenings.Add(opening);
		_persistence?.SaveJobOpening(opening);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.JobOpeningCreated,
			authorisedBy,
			$"Created a {definition.Role} job opening for {Host.EmploymentHostName}.");
		Host.DebugEmployment(
			$"Created {definition.Role.DescribeEnum()} opening #{opening.Id:N0} for {definition.MaxPositions:N0} position(s).",
			authorisedBy?.Gameworld);
		return opening;
	}

	public void CloseJobOpening(IJobOpening opening, ICharacter? authorisedBy, string reason)
	{
		if (opening is not JobOpening concrete || !ReferenceEquals(concrete.Employer, Host))
		{
			throw new InvalidOperationException("That job opening does not belong to this host.");
		}

		if (authorisedBy is not null && !HasAuthority(authorisedBy, EmploymentAuthority.ModifyJobOpenings))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to close job openings for {Host.EmploymentHostName}.");
		}

		if (concrete.Status == JobOpeningStatus.Closed)
		{
			throw new InvalidOperationException("That job opening is already closed.");
		}

		concrete.Close();
		_persistence?.SaveJobOpening(concrete);
		var closeReason = string.IsNullOrWhiteSpace(reason) ? "Closed by a manager." : reason.Trim();
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.JobOpeningClosed,
			authorisedBy,
			$"Closed {concrete.Role} job opening #{concrete.Id:N0}: {closeReason}");
		Host.DebugEmployment(
			$"Closed {concrete.Role.DescribeEnum()} opening #{concrete.Id:N0}: {closeReason}.",
			authorisedBy?.Gameworld);
	}

	public void ModifyJobOpening(IJobOpening opening, JobOpeningDefinition definition, ICharacter? authorisedBy,
		string reason)
	{
		if (opening is not JobOpening concrete || !ReferenceEquals(concrete.Employer, Host))
		{
			throw new InvalidOperationException("That job opening does not belong to this host.");
		}

		if (authorisedBy is not null && !authorisedBy.IsAdministrator())
		{
			var authorisingAuthority = AuthorityFor(authorisedBy);
			if (!authorisingAuthority.Contains(EmploymentAuthority.ModifyJobOpenings))
			{
				throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to modify job openings for {Host.EmploymentHostName}.");
			}

			if (!authorisingAuthority.ContainsAll(definition.Authority))
			{
				throw new InvalidOperationException("Managers cannot modify job openings to delegate authority they do not personally possess.");
			}
		}

		if (concrete.Status == JobOpeningStatus.Closed)
		{
			throw new InvalidOperationException("Closed job openings cannot be modified.");
		}

		if (definition.MaxPositions <= 0)
		{
			throw new InvalidOperationException("Job openings must have at least one available position.");
		}

		if (definition.Compensation.IsPaid && definition.Compensation.NominalAmount <= 0.0M)
		{
			throw new InvalidOperationException("Paid job openings must advertise a positive effective rate.");
		}

		var acceptedApplications = _applications.Count(x =>
			x.Opening.Id == concrete.Id &&
			x.Status == JobApplicationStatus.Accepted);
		if (definition.MaxPositions < acceptedApplications)
		{
			throw new InvalidOperationException("A job opening cannot be reduced below its accepted application count.");
		}

		concrete.Modify(definition);
		_persistence?.SaveJobOpening(concrete);
		var modifyReason = string.IsNullOrWhiteSpace(reason) ? "Modified by a manager." : reason.Trim();
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.JobOpeningModified,
			authorisedBy,
			$"Modified {concrete.Role} job opening #{concrete.Id:N0}: {modifyReason}");
		Host.DebugEmployment(
			$"Modified {concrete.Role.DescribeEnum()} opening #{concrete.Id:N0}: {modifyReason}.",
			authorisedBy?.Gameworld);
	}

	public IEmploymentApplication Apply(IJobOpening opening, EmploymentCandidateProfile candidate)
	{
		if (opening is not JobOpening concrete || !ReferenceEquals(concrete.Employer, Host))
		{
			throw new InvalidOperationException("That job opening does not belong to this host.");
		}

		var status = JobApplicationStatus.Pending;
		string? decisionReason = null;
		if (!concrete.AcceptsApplications)
		{
			status = JobApplicationStatus.Rejected;
			decisionReason = "The opening is not accepting applications.";
		}
		else if (!EmploymentCandidateMatcher.IsMatch(concrete, candidate, out var reason))
		{
			status = JobApplicationStatus.Rejected;
			decisionReason = reason;
		}

		var application = new EmploymentApplication(
			Interlocked.Increment(ref _nextApplicationId),
			concrete,
			candidate.Candidate,
			EmploymentClock.CurrentInstant(Host),
			status,
			decisionReason);
		_applications.Add(application);
		_persistence?.SaveApplication(application);
		EmploymentRegister.Record(
			status == JobApplicationStatus.Rejected
				? EmploymentRegisterEntryType.ApplicationRejected
				: EmploymentRegisterEntryType.ApplicationSubmitted,
			candidate.Candidate,
			status == JobApplicationStatus.Rejected
				? $"Rejected application for {concrete.Role}: {decisionReason}"
				: $"Submitted application for {concrete.Role}.");
		Host.DebugEmployment(
			status == JobApplicationStatus.Rejected
				? $"{candidate.Candidate.Name} application #{application.Id:N0} for {concrete.Role.DescribeEnum()} was rejected automatically: {decisionReason}"
				: $"{candidate.Candidate.Name} submitted pending application #{application.Id:N0} for {concrete.Role.DescribeEnum()}.",
			candidate.Candidate.Gameworld);
		if (status == JobApplicationStatus.Pending)
		{
			Host.EchoToPresentEmploymentObservers(observer =>
				$"{candidate.Candidate.HowSeen(observer, colour: true)} has applied for the {concrete.Role.DescribeEnum().ColourName()} opening at {Host.EmploymentHostName.ColourName()}.");
		}

		return application;
	}

	public IEmploymentContract AcceptApplication(IEmploymentApplication application, ICharacter authorisedBy)
	{
		if (application is not EmploymentApplication concrete || !_applications.Contains(concrete))
		{
			throw new InvalidOperationException("That employment application does not belong to this host.");
		}

		if (!HasAuthority(authorisedBy, EmploymentAuthority.HireEmployees))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to accept applications for {Host.EmploymentHostName}.");
		}

		if (concrete.Status != JobApplicationStatus.Pending)
		{
			throw new InvalidOperationException("Only pending employment applications can be accepted.");
		}

		var opening = concrete.Opening;
		if (_applications.Count(x =>
			    x.Opening.Id == opening.Id &&
			    x.Status == JobApplicationStatus.Accepted) >= opening.MaxPositions)
		{
			throw new InvalidOperationException("That employment opening has already reached its accepted application capacity.");
		}

		var contract = Hire(concrete.Candidate, new EmploymentOffer(
			opening.Role,
			opening.Compensation,
			opening.Schedule,
			opening.Duration,
			opening.PaymentMethod,
			opening.Authority), authorisedBy);
		concrete.Decide(JobApplicationStatus.Accepted, "Accepted by a manager.");
		_persistence?.SaveApplication(concrete);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ApplicationAccepted,
			authorisedBy,
			$"Accepted {concrete.Candidate.HowSeen(authorisedBy, colour: false)}'s application for {opening.Role}.");
		Host.DebugEmployment(
			$"{authorisedBy.Name} accepted application #{concrete.Id:N0} from {concrete.Candidate.Name} for {opening.Role.DescribeEnum()}.",
			authorisedBy.Gameworld);
		return contract;
	}

	public void RejectApplication(IEmploymentApplication application, ICharacter authorisedBy, string reason)
	{
		if (application is not EmploymentApplication concrete || !_applications.Contains(concrete))
		{
			throw new InvalidOperationException("That employment application does not belong to this host.");
		}

		if (!HasAuthority(authorisedBy, EmploymentAuthority.HireEmployees))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to reject applications for {Host.EmploymentHostName}.");
		}

		if (concrete.Status != JobApplicationStatus.Pending)
		{
			throw new InvalidOperationException("Only pending employment applications can be rejected.");
		}

		reason = string.IsNullOrWhiteSpace(reason) ? "Rejected by a manager." : reason.Trim();
		concrete.Decide(JobApplicationStatus.Rejected, reason);
		_persistence?.SaveApplication(concrete);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ApplicationRejected,
			authorisedBy,
			$"Rejected {concrete.Candidate.HowSeen(authorisedBy, colour: false)}'s application for {concrete.Opening.Role}: {reason}");
		Host.DebugEmployment(
			$"{authorisedBy.Name} rejected application #{concrete.Id:N0} from {concrete.Candidate.Name} for {concrete.Opening.Role.DescribeEnum()}: {reason}",
			authorisedBy.Gameworld);
	}

	private EmploymentAuthoritySet AuthorityFor(ICharacter actor)
	{
		var actorIdentityId = CharacterInstanceIdentityComparer.IdentityId(actor);
		return new EmploymentAuthoritySet(_contracts
		                                  .Where(x => x.Employee.Id == actorIdentityId)
		                                  .Where(x => x.Status == EmploymentStatus.Active)
		                                  .Aggregate(EmploymentAuthority.None,
			                                  (current, contract) => current | contract.Authority.Authorities));
	}
}

public sealed class EmploymentContract : IEmploymentContract
{
	public EmploymentContract(long id, IEmploymentHost employer, ICharacter employee, EmploymentRole role,
		EmploymentStatus status, EmploymentAuthoritySet authority, CompensationTerms compensation,
		WorkSchedule schedule, EmploymentDuration duration, PaymentMethod paymentMethod, DateTimeOffset startedAt,
		DateTimeOffset? endsAt, EmploymentTerminationReason? endReason)
	{
		Id = id;
		Employer = employer;
		Employee = employee;
		Role = role;
		Status = status;
		Authority = authority;
		Compensation = compensation;
		Schedule = schedule;
		Duration = duration;
		PaymentMethod = paymentMethod;
		StartedAt = startedAt;
		EndsAt = endsAt;
		EndReason = endReason;
	}

	public long Id { get; }
	public IEmploymentHost Employer { get; }
	public ICharacter Employee { get; }
	public EmploymentRole Role { get; }
	public EmploymentStatus Status { get; private set; }
	public EmploymentAuthoritySet Authority { get; private set; }
	public CompensationTerms Compensation { get; }
	public WorkSchedule Schedule { get; }
	public EmploymentDuration Duration { get; }
	public PaymentMethod PaymentMethod { get; }
	public DateTimeOffset StartedAt { get; }
	public DateTimeOffset? EndsAt { get; private set; }
	public EmploymentTerminationReason? EndReason { get; private set; }

	public void End(EmploymentTerminationReason reason, DateTimeOffset endedAt)
	{
		Status = EmploymentStatus.Ended;
		EndsAt = endedAt;
		EndReason = reason;
	}

	public void SetAuthority(EmploymentAuthoritySet authority)
	{
		Authority = authority;
	}
}

public sealed class JobOpening : IJobOpening
{
	public JobOpening(long id, IEmploymentHost employer, EmploymentRole role, JobRequirementSet requirements,
		CompensationTerms compensation, WorkSchedule schedule, EmploymentDuration duration, JobOpeningStatus status,
		int maxPositions, bool npcApplicationsOnly, PaymentMethod paymentMethod, EmploymentAuthoritySet authority)
	{
		Id = id;
		Employer = employer;
		Role = role;
		Requirements = requirements;
		Compensation = compensation;
		Schedule = schedule;
		Duration = duration;
		Status = status;
		MaxPositions = maxPositions;
		NpcApplicationsOnly = npcApplicationsOnly;
		PaymentMethod = paymentMethod;
		Authority = authority;
	}

	public long Id { get; }
	public IEmploymentHost Employer { get; }
	public EmploymentRole Role { get; private set; }
	public JobRequirementSet Requirements { get; private set; }
	public CompensationTerms Compensation { get; private set; }
	public WorkSchedule Schedule { get; private set; }
	public EmploymentDuration Duration { get; private set; }
	public JobOpeningStatus Status { get; private set; }
	public int MaxPositions { get; private set; }
	public bool NpcApplicationsOnly { get; private set; }
	public PaymentMethod PaymentMethod { get; private set; }
	public EmploymentAuthoritySet Authority { get; private set; }

	public bool AcceptsApplications =>
		Status == JobOpeningStatus.Open &&
		Employer.Employment.Applications.Count(x =>
			x.Opening.Id == Id &&
			x.Status == JobApplicationStatus.Accepted) < MaxPositions;

	public void Close()
	{
		Status = JobOpeningStatus.Closed;
	}

	public void Modify(JobOpeningDefinition definition)
	{
		Role = definition.Role;
		Requirements = definition.Requirements;
		Compensation = definition.Compensation;
		Schedule = definition.Schedule;
		Duration = definition.Duration;
		MaxPositions = definition.MaxPositions;
		NpcApplicationsOnly = definition.NpcApplicationsOnly;
		PaymentMethod = definition.PaymentMethod;
		Authority = definition.Authority;
	}
}

public sealed class EmploymentApplication : IEmploymentApplication
{
	public EmploymentApplication(long id, IJobOpening opening, ICharacter candidate, DateTimeOffset appliedAt,
		JobApplicationStatus status, string? decisionReason)
	{
		Id = id;
		Opening = opening;
		Candidate = candidate;
		AppliedAt = appliedAt;
		Status = status;
		DecisionReason = decisionReason;
	}

	public long Id { get; }
	public IJobOpening Opening { get; }
	public ICharacter Candidate { get; }
	public DateTimeOffset AppliedAt { get; }
	public JobApplicationStatus Status { get; private set; }
	public string? DecisionReason { get; private set; }

	public void Decide(JobApplicationStatus status, string? reason)
	{
		Status = status;
		DecisionReason = reason;
	}
}

public sealed class EmploymentPayable : IEmploymentPayable
{
	public EmploymentPayable(long id, Guid correlationId, IEmploymentHost employer, long? contractId, long employeeId,
		string employeeName, EmploymentRole role, MoneyAmount amount, PayCadence cadence, PaymentMethod paymentMethod,
		DateTimeOffset payPeriodStart, DateTimeOffset payPeriodEnd, DateTimeOffset dueAt, DateTimeOffset accruedAt,
		EmploymentPayableStatus status, DateTimeOffset? settledAt, DateTimeOffset? claimedAt, string? settlementNote)
	{
		Id = id;
		CorrelationId = correlationId;
		Employer = employer;
		ContractId = contractId;
		EmployeeId = employeeId;
		EmployeeName = string.IsNullOrWhiteSpace(employeeName) ? $"employee #{employeeId:N0}" : employeeName;
		Role = role;
		Amount = amount;
		Cadence = cadence;
		PaymentMethod = paymentMethod;
		PayPeriodStart = payPeriodStart;
		PayPeriodEnd = payPeriodEnd;
		DueAt = dueAt;
		AccruedAt = accruedAt;
		Status = status;
		SettledAt = settledAt;
		ClaimedAt = claimedAt;
		SettlementNote = settlementNote;
	}

	public long Id { get; }
	public Guid CorrelationId { get; }
	public IEmploymentHost Employer { get; }
	public long? ContractId { get; }
	public long EmployeeId { get; }
	public string EmployeeName { get; }
	public EmploymentRole Role { get; }
	public MoneyAmount Amount { get; }
	public PayCadence Cadence { get; }
	public PaymentMethod PaymentMethod { get; }
	public DateTimeOffset PayPeriodStart { get; }
	public DateTimeOffset PayPeriodEnd { get; }
	public DateTimeOffset DueAt { get; }
	public DateTimeOffset AccruedAt { get; }
	public DateTimeOffset? SettledAt { get; private set; }
	public DateTimeOffset? ClaimedAt { get; private set; }
	public EmploymentPayableStatus Status { get; private set; }
	public string? SettlementNote { get; private set; }

	public int DaysOverdue(DateTimeOffset now)
	{
		if (Status != EmploymentPayableStatus.Accrued || now <= DueAt)
		{
			return 0;
		}

		return Math.Max(0, (int)Math.Floor((now - DueAt).TotalDays));
	}

	public void MarkReadyToClaim(DateTimeOffset settledAt, string reason)
	{
		Status = EmploymentPayableStatus.ReadyToClaim;
		SettledAt = settledAt;
		SettlementNote = reason;
	}

	public void MarkSettled(DateTimeOffset settledAt, string reason)
	{
		Status = EmploymentPayableStatus.Settled;
		SettledAt = settledAt;
		SettlementNote = reason;
	}

	public void MarkClaimed(DateTimeOffset claimedAt)
	{
		Status = EmploymentPayableStatus.Claimed;
		ClaimedAt = claimedAt;
	}
}

public sealed class EmploymentPayroll : IEmploymentPayroll
{
	private const int MaxPeriodsPerEvaluation = 1000;
	private readonly EmploymentHostState _state;
	private readonly List<IEmploymentPayable> _payables = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentPayroll(EmploymentHostState state)
	{
		_state = state;
	}

	internal EmploymentPayroll(EmploymentHostState state, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentPayable> payables)
	{
		_state = state;
		_persistence = persistence;
		_payables.AddRange(payables);
	}

	public IReadOnlyCollection<IEmploymentPayable> Payables => _payables;

	public IReadOnlyCollection<IEmploymentPayable> OutstandingLiabilities =>
		_payables
			.Where(x => x.Status == EmploymentPayableStatus.Accrued)
			.ToList();

	public IReadOnlyCollection<IEmploymentPayable> ClaimablePayablesFor(ICharacter employee)
	{
		var employeeIdentityId = CharacterInstanceIdentityComparer.IdentityId(employee);
		return _payables
		       .Where(x => x.EmployeeId == employeeIdentityId)
		       .Where(x => x.Status == EmploymentPayableStatus.ReadyToClaim)
		       .OrderBy(x => x.DueAt)
		       .ThenBy(x => x.Id)
		       .ToList();
	}

	public IReadOnlyCollection<IEmploymentPayable> EvaluatePayroll()
	{
		return EvaluatePayroll(EmploymentClock.CurrentInstant(_state.Host));
	}

	public IReadOnlyCollection<IEmploymentPayable> EvaluatePayroll(DateTimeOffset now)
	{
		var created = new List<IEmploymentPayable>();
		foreach (var contract in _state.EmploymentContracts
		                               .Where(x => x.Status is EmploymentStatus.Active or EmploymentStatus.Ended)
		                               .OrderBy(x => x.Id))
		{
			var periodLength = PeriodLength(contract.Compensation.Cadence);
			var amount = contract.Compensation.FixedRate ?? contract.Compensation.MinimumEffectivePay;
			if (periodLength is null || amount is null || !amount.IsPositive)
			{
				continue;
			}

			var evaluationEnd = contract.Status == EmploymentStatus.Ended && contract.EndsAt.HasValue
				? contract.EndsAt.Value < now ? contract.EndsAt.Value : now
				: now;
			if (evaluationEnd <= contract.StartedAt)
			{
				continue;
			}

			var periodStart = _payables
			                  .Where(x => x.ContractId == contract.Id)
			                  .Select(x => x.PayPeriodEnd)
			                  .DefaultIfEmpty(contract.StartedAt)
			                  .Max();
			if (periodStart < contract.StartedAt)
			{
				periodStart = contract.StartedAt;
			}

			var createdForContract = 0;
			while (createdForContract++ < MaxPeriodsPerEvaluation)
			{
				var periodEnd = periodStart.Add(periodLength.Value);
				var isFinalPartialPeriod =
					contract.Status == EmploymentStatus.Ended &&
					contract.EndsAt.HasValue &&
					periodEnd > contract.EndsAt.Value &&
					contract.EndsAt.Value > periodStart;
				if (periodEnd > evaluationEnd)
				{
					if (!isFinalPartialPeriod)
					{
						break;
					}

					periodEnd = evaluationEnd;
				}

				if (periodEnd <= periodStart)
				{
					break;
				}

				if (_payables.Any(x =>
					    x.ContractId == contract.Id &&
					    x.PayPeriodStart == periodStart &&
					    x.PayPeriodEnd == periodEnd))
				{
					periodStart = periodEnd;
					continue;
				}

				var payable = new EmploymentPayable(
					_state.NextPayableId(),
					Guid.NewGuid(),
					_state.Host,
					contract.Id,
					contract.Employee.Id,
					contract.Employee.Name,
					contract.Role,
					AmountForPeriod(amount, contract.Compensation.Cadence, periodStart, periodEnd, periodLength.Value),
					contract.Compensation.Cadence,
					contract.PaymentMethod,
					periodStart,
					periodEnd,
					periodEnd,
					now,
					EmploymentPayableStatus.Accrued,
					null,
					null,
					null);
				_payables.Add(payable);
				created.Add(payable);
				_persistence?.SavePayable(payable);
				_state.EmploymentRegister.Record(
					EmploymentRegisterEntryType.WageAccrued,
					null,
					$"Accrued {DescribeMoney(payable.Amount)} payable to {payable.EmployeeName} for {payable.Role} wages.",
					payable.CorrelationId);
				periodStart = periodEnd;
			}
		}

		return created;
	}

	public int MaximumOverdueDays()
	{
		return MaximumOverdueDays(EmploymentClock.CurrentInstant(_state.Host));
	}

	public int MaximumOverdueDays(DateTimeOffset now)
	{
		return _payables
		       .Select(x => x.DaysOverdue(now))
		       .DefaultIfEmpty()
		       .Max();
	}

	public bool TrySettlePayables(IEnumerable<IEmploymentPayable> payables, ICharacter? actor, bool makeClaimable,
		string reason, out string message)
	{
		reason = string.IsNullOrWhiteSpace(reason) ? "Settled by employer." : reason.Trim();
		var targets = payables
		              .OfType<EmploymentPayable>()
		              .Where(x => ReferenceEquals(x.Employer, _state.Host))
		              .Where(x => x.Status is EmploymentPayableStatus.Accrued or EmploymentPayableStatus.ReadyToClaim)
		              .OrderBy(x => x.DueAt)
		              .ThenBy(x => x.Id)
		              .ToList();
		if (!targets.Any())
		{
			message = "There are no outstanding employment payables to settle.";
			return false;
		}

		var accruedAmounts = targets
		                     .Where(x => x.Status == EmploymentPayableStatus.Accrued)
		                     .GroupBy(x => x.Amount.Currency.Id)
		                     .Select(x => new MoneyAmount(x.First().Amount.Currency, x.Sum(y => y.Amount.Amount)))
		                     .ToList();
		foreach (var amount in accruedAmounts)
		{
			if (!EmploymentFinanceService.CanSettlePayroll(_state.Host, amount, out message))
			{
				return false;
			}
		}

		foreach (var amount in accruedAmounts)
		{
			if (!EmploymentFinanceService.TrySettlePayroll(_state.Host, actor, amount, Guid.NewGuid(), out message))
			{
				return false;
			}
		}

		var newlyFunded = 0;
		var settled = 0;
		var now = EmploymentClock.CurrentInstant(_state.Host);
		foreach (var payable in targets)
		{
			var wasAccrued = payable.Status == EmploymentPayableStatus.Accrued;
			if (makeClaimable && IsActiveCashEmployee(payable.EmployeeId) &&
			    payable.PaymentMethod.MethodKind == PaymentMethodKind.Cash)
			{
				if (payable.Status == EmploymentPayableStatus.Accrued)
				{
					payable.MarkReadyToClaim(now, reason);
					newlyFunded++;
				}
			}
			else
			{
				payable.MarkSettled(now, reason);
				settled++;
			}

			_persistence?.SavePayableState(payable);
			if (wasAccrued)
			{
				_state.BusinessLedger.Record(
					EmploymentLedgerEntryType.Wage,
					actor,
					payable.Amount,
					$"Settled wages payable to {payable.EmployeeName}: {reason}",
					payable.CorrelationId);
				_state.EmploymentRegister.Record(
					EmploymentRegisterEntryType.WageSettled,
					actor,
					$"Settled {DescribeMoney(payable.Amount)} payable to {payable.EmployeeName}.",
					payable.CorrelationId);
			}
		}

		message = $"Settled {targets.Count:N0} employment payable{(targets.Count == 1 ? string.Empty : "s")}.";
		if (newlyFunded > 0)
		{
			message += $" {newlyFunded:N0} cash payable{(newlyFunded == 1 ? " is" : "s are")} ready for employee claim.";
		}

		if (settled > 0)
		{
			message += $" {settled:N0} payable{(settled == 1 ? " was" : "s were")} closed without a cash claim.";
		}

		return true;
	}

	public bool TryClaimPayable(IEmploymentPayable payable, ICharacter actor, out string message)
	{
		if (payable is not EmploymentPayable concrete || !ReferenceEquals(concrete.Employer, _state.Host))
		{
			message = "That employment payable does not belong to this host.";
			return false;
		}

		if (concrete.EmployeeId != CharacterInstanceIdentityComparer.IdentityId(actor))
		{
			message = "That employment payable is not owed to you.";
			return false;
		}

		if (concrete.Status != EmploymentPayableStatus.ReadyToClaim)
		{
			message = "That employment payable is not ready to be claimed.";
			return false;
		}

		if (concrete.PaymentMethod.MethodKind != PaymentMethodKind.Cash)
		{
			concrete.MarkClaimed(EmploymentClock.CurrentInstant(_state.Host));
			_persistence?.SavePayableState(concrete);
			_state.EmploymentRegister.Record(
				EmploymentRegisterEntryType.WageClaimed,
				actor,
				$"Claimed non-cash wage settlement for {concrete.EmployeeName}.",
				concrete.CorrelationId);
			message = "You mark that employment payable as claimed.";
			return true;
		}

		if (!TryGiveCash(actor, concrete.Amount, out var cashMessage))
		{
			message = cashMessage;
			return false;
		}

		concrete.MarkClaimed(EmploymentClock.CurrentInstant(_state.Host));
		_persistence?.SavePayableState(concrete);
		_state.EmploymentRegister.Record(
			EmploymentRegisterEntryType.WageClaimed,
			actor,
			$"Claimed {DescribeMoney(concrete.Amount)} payable to {concrete.EmployeeName}.",
			concrete.CorrelationId);
		message = $"You claim {DescribeMoney(concrete.Amount)} in wages from {concrete.Employer.EmploymentHostName.ColourName()}.";
		if (!string.IsNullOrWhiteSpace(cashMessage))
		{
			message += $" {cashMessage}";
		}

		return true;
	}

	private bool IsActiveCashEmployee(long employeeId)
	{
		return _state.EmploymentContracts.Any(x =>
			x.Employee.Id == employeeId &&
			x.Status == EmploymentStatus.Active);
	}

	private static TimeSpan? PeriodLength(PayCadence cadence)
	{
		return cadence switch
		{
			PayCadence.Hourly => TimeSpan.FromDays(1),
			PayCadence.Daily => TimeSpan.FromDays(1),
			PayCadence.Weekly => TimeSpan.FromDays(7),
			PayCadence.Salary => TimeSpan.FromDays(30),
			_ => null
		};
	}

	private static MoneyAmount AmountForPeriod(MoneyAmount rate, PayCadence cadence, DateTimeOffset periodStart,
		DateTimeOffset periodEnd, TimeSpan nominalPeriodLength)
	{
		var elapsed = periodEnd - periodStart;
		var amount = cadence switch
		{
			PayCadence.Hourly => rate.Amount * (decimal)elapsed.TotalHours,
			PayCadence.Daily or PayCadence.Weekly or PayCadence.Salary
				when elapsed < nominalPeriodLength && nominalPeriodLength > TimeSpan.Zero =>
				rate.Amount * (decimal)(elapsed.TotalSeconds / nominalPeriodLength.TotalSeconds),
			_ => rate.Amount
		};

		return new MoneyAmount(rate.Currency, decimal.Round(amount, 2, MidpointRounding.AwayFromZero));
	}

	private static bool TryGiveCash(ICharacter actor, MoneyAmount amount, out string message)
	{
		var coins = amount.Currency.FindCoinsForAmount(amount.Amount, out _);
		if (!coins.Any())
		{
			message = "The currency system could not produce a cash pile for that wage amount.";
			return false;
		}

		var pile = CurrencyGameItemComponentProto.CreateNewCurrencyPile(amount.Currency, coins);
		actor.Gameworld.Add(pile);
		if (!actor.Body.CanGet(pile, 0))
		{
			pile.RoomLayer = actor.RoomLayer;
			actor.Location.Insert(pile, true);
			message = "You could not hold the cash, so it has been placed at your feet.";
			return true;
		}

		actor.Body.Get(pile);
		var container = actor.Inventory
		                     .Where(x => !ReferenceEquals(x, pile))
		                     .Where(x => x.GetItemType<IContainer>() is not null)
		                     .FirstOrDefault(x => actor.Body.CanPut(pile, x, null, 0, false));
		if (container is not null)
		{
			actor.Body.Put(pile, container, null);
			message = $"The cash is put away in {container.HowSeen(actor, colour: false).ColourName()}.";
			return true;
		}

		message = string.Empty;
		return true;
	}

	private static string DescribeMoney(MoneyAmount amount)
	{
		return amount.Currency.Describe(amount.Amount, CurrencyDescriptionPatternType.ShortDecimal);
	}
}

public sealed record EmploymentLedgerEntry(
	Guid CorrelationId,
	EmploymentLedgerEntryType EntryType,
	IEmploymentHost Employer,
	ICharacter? Actor,
	MoneyAmount? Amount,
	string Description,
	DateTimeOffset RecordedAt) : IEmploymentLedgerEntry;

public sealed class EmploymentLedger : IEmploymentLedger
{
	private readonly IEmploymentHost _host;
	private readonly List<IEmploymentLedgerEntry> _entries = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentLedger(IEmploymentHost host)
	{
		_host = host;
	}

	internal EmploymentLedger(IEmploymentHost host, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentLedgerEntry> entries)
	{
		_host = host;
		_persistence = persistence;
		_entries.AddRange(entries);
	}

	public IReadOnlyCollection<IEmploymentLedgerEntry> Entries => _entries;

	public IEmploymentLedgerEntry Record(EmploymentLedgerEntryType entryType, ICharacter? actor, MoneyAmount? amount,
		string description, Guid? correlationId = null)
	{
		var entry = new EmploymentLedgerEntry(
			correlationId ?? Guid.NewGuid(),
			entryType,
			_host,
			actor,
			amount,
			description,
			EmploymentClock.CurrentInstant(_host));
		_entries.Add(entry);
		_persistence?.SaveLedgerEntry(entry);
		return entry;
	}
}

public sealed record EmploymentRegisterEntry(
	Guid CorrelationId,
	EmploymentRegisterEntryType EntryType,
	IEmploymentHost Employer,
	ICharacter? Actor,
	string Description,
	DateTimeOffset RecordedAt) : IEmploymentRegisterEntry;

public sealed class EmploymentRegister : IEmploymentRegister
{
	private readonly IEmploymentHost _host;
	private readonly List<IEmploymentRegisterEntry> _entries = new();
	private readonly IEmploymentPersistenceStore? _persistence;

	public EmploymentRegister(IEmploymentHost host)
	{
		_host = host;
	}

	internal EmploymentRegister(IEmploymentHost host, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentRegisterEntry> entries)
	{
		_host = host;
		_persistence = persistence;
		_entries.AddRange(entries);
	}

	public IReadOnlyCollection<IEmploymentRegisterEntry> Entries => _entries;

	public IEmploymentRegisterEntry Record(EmploymentRegisterEntryType entryType, ICharacter? actor,
		string description, Guid? correlationId = null)
	{
		var entry = new EmploymentRegisterEntry(
			correlationId ?? Guid.NewGuid(),
			entryType,
			_host,
			actor,
			description,
			EmploymentClock.CurrentInstant(_host));
		_entries.Add(entry);
		_persistence?.SaveRegisterEntry(entry);
		return entry;
	}
}

public sealed class EmploymentHostBoard : FrameworkItem, IBoard
{
	private readonly List<IBoardPost> _posts = new();

	public EmploymentHostBoard(IEmploymentHost host)
	{
		_id = -host.Id;
		_name = $"{host.EmploymentHostName} Staff Board";
	}

	public override string FrameworkItemType => "EmploymentHostBoard";
	public bool DisplayOnLogin => false;
	public ICalendar Calendar => null!;
	public IEnumerable<IBoardPost> Posts => _posts;

	public void MakeNewPost(IAccount author, string title, string text)
	{
		_posts.Add(new EmploymentHostBoardPost(this, author?.Id, author?.Name ?? "System", false, title, text));
	}

	public void MakeNewPost(ICharacter author, string title, string text)
	{
		_posts.Add(new EmploymentHostBoardPost(
			this,
			author?.Id,
			author?.CurrentName.GetName(NameStyle.FullName) ?? "System",
			true,
			title,
			text));
	}

	public void MakeNewPost(string authorName, string title, string text)
	{
		_posts.Add(new EmploymentHostBoardPost(this, null, authorName, false, title, text));
	}

	public void DeletePost(IBoardPost post)
	{
		_posts.Remove(post);
	}
}

public sealed class EmploymentHostBoardPost : FrameworkItem, IBoardPost
{
	private static long _nextId;

	public EmploymentHostBoardPost(IBoard board, long? authorId, string authorName, bool authorIsCharacter,
		string title, string text)
	{
		_id = Interlocked.Increment(ref _nextId);
		_name = title;
		Board = board;
		AuthorId = authorId;
		AuthorName = string.IsNullOrWhiteSpace(authorName) ? "System" : authorName.Trim();
		AuthorIsCharacter = authorIsCharacter;
		Title = title;
		Text = text;
		PostTime = DateTime.UtcNow;
	}

	public override string FrameworkItemType => "EmploymentHostBoardPost";
	public IBoard Board { get; }
	public string Title { get; }
	public string Text { get; }
	public long? AuthorId { get; }
	public string AuthorName { get; }
	public DateTime PostTime { get; }
	public bool AuthorIsCharacter { get; }
	public MudDateTime InGameDateTime => null!;
	public string AuthorShortDescription => AuthorName;
	public string AuthorFullDescription => AuthorName;

	public void Delete()
	{
		Board.DeletePost(this);
	}
}
