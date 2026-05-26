using MudSharp.Character;

#nullable enable

namespace MudSharp.Economy.Employment;

internal interface IEmploymentPersistenceStore
{
	long StateId { get; }
	void SaveContract(EmploymentContract contract);
	void SaveContractEnded(EmploymentContract contract);
	void SaveJobOpening(JobOpening opening);
	void SaveApplication(EmploymentApplication application);
	void SaveRegisterEntry(EmploymentRegisterEntry entry);
	void SaveLedgerEntry(EmploymentLedgerEntry entry);
	void SaveScheduledRule(EmploymentScheduledTaskRule rule);
	void SaveScheduledRuleState(EmploymentScheduledTaskRule rule);
	void SaveActiveTask(EmploymentActiveTask task);
	void SaveActiveTaskState(EmploymentActiveTask task);
	void SaveManagerGoal(ManagerGoal goal);
	void SaveManagerGoalState(ManagerGoal goal);
}
