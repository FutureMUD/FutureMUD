using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community.Boards;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
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

	public EmploymentHostState(IEmploymentHost host, IBoard? board = null)
	{
		Host = host;
		EmploymentRegister = new EmploymentRegister(host);
		BusinessLedger = new EmploymentLedger(host);
		Board = board ?? new EmploymentHostBoard(host);
		TaskBoard = new EmploymentTaskBoard(host);
		ManagerGoalBoard = new ManagerGoalBoard(host);
	}

	internal EmploymentHostState(IEmploymentHost host, IBoard board, IEmploymentPersistenceStore persistence,
		IEnumerable<IEmploymentContract> contracts, IEnumerable<IJobOpening> jobOpenings,
		IEnumerable<IEmploymentApplication> applications, IEnumerable<IEmploymentLedgerEntry> ledgerEntries,
		IEnumerable<IEmploymentRegisterEntry> registerEntries, IEnumerable<IEmploymentScheduledTaskRule> scheduledRules,
		IEnumerable<IEmploymentActiveTask> activeTasks, IEnumerable<IManagerGoal> managerGoals)
	{
		Host = host;
		_persistence = persistence;
		_contracts.AddRange(contracts);
		_jobOpenings.AddRange(jobOpenings);
		_applications.AddRange(applications);
		_nextContractId = _contracts.Select(x => x.Id).DefaultIfEmpty().Max();
		_nextJobOpeningId = _jobOpenings.Select(x => x.Id).DefaultIfEmpty().Max();
		_nextApplicationId = _applications.Select(x => x.Id).DefaultIfEmpty().Max();
		EmploymentRegister = new EmploymentRegister(host, persistence, registerEntries);
		BusinessLedger = new EmploymentLedger(host, persistence, ledgerEntries);
		Board = board;
		TaskBoard = new EmploymentTaskBoard(host, persistence, scheduledRules, activeTasks);
		ManagerGoalBoard = new ManagerGoalBoard(host, persistence, managerGoals);
	}

	public IEmploymentHost Host { get; }
	public IEmploymentLedger BusinessLedger { get; }
	public IEmploymentRegister EmploymentRegister { get; }
	public IBoard Board { get; }
	public IEmploymentTaskBoard TaskBoard { get; }
	public IManagerGoalBoard ManagerGoalBoard { get; }
	public IReadOnlyCollection<IEmploymentContract> EmploymentContracts => _contracts;
	public IReadOnlyCollection<IJobOpening> JobOpenings => _jobOpenings;
	public IReadOnlyCollection<IEmploymentApplication> Applications => _applications;

	public bool CanEmploy(ICharacter candidate, EmploymentRole role, out string reason)
	{
		if (candidate is null)
		{
			reason = "There is no candidate to employ.";
			return false;
		}

		if (_contracts.Any(x =>
			    x.Employee.Id == candidate.Id &&
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

		if (authorisedBy is not null && !HasAuthority(authorisedBy, EmploymentAuthority.HireEmployees))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to hire for {Host.EmploymentHostName}.");
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
			DateTimeOffset.UtcNow,
			null,
			null);
		_contracts.Add(contract);
		_persistence?.SaveContract(contract);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ContractHired,
			authorisedBy,
			$"Hired {candidate.HowSeen(candidate, colour: false)} as {offer.Role}.");
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

		if (concrete.Status == EmploymentStatus.Ended)
		{
			return;
		}

		concrete.End(reason, DateTimeOffset.UtcNow);
		_persistence?.SaveContractEnded(concrete);
		EmploymentRegister.Record(
			EmploymentRegisterEntryType.ContractEnded,
			authorisedBy,
			$"Ended {concrete.Employee.HowSeen(concrete.Employee, colour: false)}'s {concrete.Role} contract: {reason}.");
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

		return _contracts.Any(x =>
			x.Employee.Id == actor.Id &&
			x.Status == EmploymentStatus.Active &&
			x.Authority.Contains(authority));
	}

	public IJobOpening CreateJobOpening(JobOpeningDefinition definition, ICharacter? authorisedBy)
	{
		if (authorisedBy is not null && !HasAuthority(authorisedBy, EmploymentAuthority.CreateJobOpenings))
		{
			throw new InvalidOperationException($"{authorisedBy.HowSeen(authorisedBy, colour: false)} is not authorised to create job openings for {Host.EmploymentHostName}.");
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
		return opening;
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
			DateTimeOffset.UtcNow,
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
}

public sealed record JobOpening(
	long Id,
	IEmploymentHost Employer,
	EmploymentRole Role,
	JobRequirementSet Requirements,
	CompensationTerms Compensation,
	WorkSchedule Schedule,
	EmploymentDuration Duration,
	JobOpeningStatus Status,
	int MaxPositions,
	bool NpcApplicationsOnly,
	PaymentMethod PaymentMethod,
	EmploymentAuthoritySet Authority) : IJobOpening
{
	public bool AcceptsApplications =>
		Status == JobOpeningStatus.Open &&
		Employer.EmploymentContracts.Count(x => x.Role == Role && x.Status == EmploymentStatus.Active) < MaxPositions;
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
			DateTimeOffset.UtcNow);
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
			DateTimeOffset.UtcNow);
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
