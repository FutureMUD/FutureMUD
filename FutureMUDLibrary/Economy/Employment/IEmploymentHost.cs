using System;
using System.Collections.Generic;
using MudSharp.Character;
using MudSharp.Community.Boards;
using MudSharp.Framework;

#nullable enable

namespace MudSharp.Economy.Employment;

public interface IEmploymentHost : IFrameworkItem
{
	string EmploymentHostName => Name;
	EmploymentHostType EmploymentHostType { get; }
	IMarket? Market { get; }
	IEmploymentHostState Employment { get; }

	IEmploymentLedger BusinessLedger => Employment.BusinessLedger;
	IEmploymentRegister EmploymentRegister => Employment.EmploymentRegister;
	IBoard Board => Employment.Board;
	IEmploymentTaskBoard TaskBoard => Employment.TaskBoard;
	IManagerGoalBoard ManagerGoalBoard => Employment.ManagerGoalBoard;
	IEmploymentPayroll Payroll => Employment.Payroll;
	IReadOnlyCollection<IEmploymentContract> EmploymentContracts => Employment.EmploymentContracts;
	IReadOnlyCollection<IJobOpening> JobOpenings => Employment.JobOpenings;

	bool CanEmploy(ICharacter candidate, EmploymentRole role, out string reason)
	{
		return Employment.CanEmploy(candidate, role, out reason);
	}

	IEmploymentContract Hire(ICharacter candidate, EmploymentOffer offer, ICharacter? authorisedBy)
	{
		return Employment.Hire(candidate, offer, authorisedBy);
	}

	void Fire(IEmploymentContract contract, EmploymentTerminationReason reason, ICharacter? authorisedBy)
	{
		Employment.Fire(contract, reason, authorisedBy);
	}

	bool HasAuthority(ICharacter actor, EmploymentAuthority authority)
	{
		return Employment.HasAuthority(actor, authority);
	}

	void SetContractAuthority(IEmploymentContract contract, EmploymentAuthoritySet authority, ICharacter authorisedBy)
	{
		Employment.SetContractAuthority(contract, authority, authorisedBy);
	}
}

public interface IEmploymentHostState
{
	IEmploymentHost Host { get; }
	IEmploymentLedger BusinessLedger { get; }
	IEmploymentRegister EmploymentRegister { get; }
	IBoard Board { get; }
	IEmploymentTaskBoard TaskBoard { get; }
	IManagerGoalBoard ManagerGoalBoard { get; }
	IEmploymentPayroll Payroll { get; }
	IReadOnlyCollection<IEmploymentContract> EmploymentContracts { get; }
	IReadOnlyCollection<IJobOpening> JobOpenings { get; }
	IReadOnlyCollection<IEmploymentApplication> Applications { get; }

	bool CanEmploy(ICharacter candidate, EmploymentRole role, out string reason);
	IEmploymentContract Hire(ICharacter candidate, EmploymentOffer offer, ICharacter? authorisedBy);
	void Fire(IEmploymentContract contract, EmploymentTerminationReason reason, ICharacter? authorisedBy);
	IReadOnlyCollection<IEmploymentContract> EvaluateContractLifecycle(DateTimeOffset now);
	bool HasAuthority(ICharacter actor, EmploymentAuthority authority);
	void SetContractAuthority(IEmploymentContract contract, EmploymentAuthoritySet authority, ICharacter authorisedBy);
	IJobOpening CreateJobOpening(JobOpeningDefinition definition, ICharacter? authorisedBy);
	void CloseJobOpening(IJobOpening opening, ICharacter? authorisedBy, string reason);
	void ModifyJobOpening(IJobOpening opening, JobOpeningDefinition definition, ICharacter? authorisedBy, string reason);
	IEmploymentApplication Apply(IJobOpening opening, EmploymentCandidateProfile candidate);
	IEmploymentContract AcceptApplication(IEmploymentApplication application, ICharacter authorisedBy);
	void RejectApplication(IEmploymentApplication application, ICharacter authorisedBy, string reason);
}
