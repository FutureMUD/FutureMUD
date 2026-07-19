
#nullable enable

namespace MudSharp.Economy.Employment;

internal interface IEmploymentPersistenceStore
{
	long StateId { get; }
	void SaveContract(EmploymentContract contract);
	void SaveContractEnded(EmploymentContract contract);
	void SaveContractAuthority(EmploymentContract contract);
	void SaveJobOpening(JobOpening opening);
	void SaveApplication(EmploymentApplication application);
	void SavePayable(EmploymentPayable payable);
	void SavePayableState(EmploymentPayable payable);
	void SaveRegisterEntry(EmploymentRegisterEntry entry);
	void SaveLedgerEntry(EmploymentLedgerEntry entry);
	void SaveScheduledRule(EmploymentScheduledTaskRule rule);
	void SaveScheduledRuleState(EmploymentScheduledTaskRule rule);
	void DeleteScheduledRule(EmploymentScheduledTaskRule rule);
	void SaveConditionPredicate(EmploymentConditionPredicate predicate);
	void DeleteConditionPredicate(EmploymentConditionPredicate predicate);
	void SaveScheduledRuleTemplate(EmploymentScheduledRuleTemplate template);
	void DeleteScheduledRuleTemplate(EmploymentScheduledRuleTemplate template);
	void SaveActiveTask(EmploymentActiveTask task);
	void SaveActiveTaskState(EmploymentActiveTask task);
	void SaveManagerGoal(ManagerGoal goal);
	void SaveManagerGoalState(ManagerGoal goal);
}
