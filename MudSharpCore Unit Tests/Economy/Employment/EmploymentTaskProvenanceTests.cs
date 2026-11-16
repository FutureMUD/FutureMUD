#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Models;
using MudSharp.Character;

namespace MudSharp_Unit_Tests.Economy.Employment;

/// <summary>Pure snapshot/authority boundaries; no model construction, database, processes or sleeps.</summary>
[TestClass]
public class EmploymentTaskProvenanceTests
{
	[DataTestMethod]
	[DataRow(EmploymentTaskSourceKind.Manual)]
	[DataRow(EmploymentTaskSourceKind.ScheduledRule)]
	[DataRow(EmploymentTaskSourceKind.ManagerGoal)]
	public void Snapshot_RoundTrip_PreservesApprovalAndOrdering(EmploymentTaskSourceKind kind)
	{
		var (task, record) = Snapshot(kind);
		task.MarkStep(0, EmploymentActionStepStatus.Completed);
		EmploymentTaskProvenancePersistence.Update(task, record);
		var restored = EmploymentTaskProvenancePersistence.Read(record, out var diagnostic);
		Assert.IsNotNull(restored, diagnostic);
		Assert.AreEqual(task.SourceKind, restored.SourceKind);
		Assert.AreEqual(task.SourceRuleId, restored.SourceRuleId);
		Assert.AreEqual(task.SourceGoalId, restored.SourceGoalId);
		Assert.AreEqual(task.Priority, restored.Priority);
		Assert.AreEqual(task.CreatedAt, restored.CreatedAt);
		Assert.AreEqual(task.DueAt, restored.DueAt);
		Assert.AreEqual(task.Provenance.StartedAt, restored.StartedAt);
		Assert.AreEqual(task.CreatedByPrincipal, restored.CreatedByPrincipal);
		Assert.AreEqual(task.AuthorisedByPrincipal, restored.AuthorisedByPrincipal);
		Assert.AreEqual(task.AuthorisationGrant.Id, restored.AuthorisationGrant.Id);
		Assert.AreEqual(task.AuthorisationGrant.IssuerPrincipal, restored.AuthorisationGrant.IssuerPrincipal);
		Assert.AreEqual(task.AuthorisationGrant.Authority.Authorities, restored.AuthorisationGrant.Authority.Authorities);
		Assert.AreEqual(task.AuthorisationGrant.CorrelationId, restored.AuthorisationGrant.CorrelationId);
		Assert.AreEqual(12M, restored.AuthorisationGrant.AmountLimits[1]);
		Assert.IsFalse(restored.AuthorisationGrant.AllowsUnboundedFinancialSteps);
		Assert.AreEqual(task.AuthorisationGrant.IssuedAt, restored.AuthorisationGrant.IssuedAt);
		Assert.IsTrue(restored.AuthorisationGrant.PaymentSourceScopes.SetEquals(task.AuthorisationGrant.PaymentSourceScopes));
		Assert.IsTrue(restored.AuthorisationGrant.CounterpartyScopes.SetEquals(task.AuthorisationGrant.CounterpartyScopes));
	}

	[DataTestMethod]
	[DataRow("version")]
	[DataRow("host")]
	[DataRow("task")]
	[DataRow("plan")]
	[DataRow("principal")]
	[DataRow("time")]
	[DataRow("limit")]
	[DataRow("incomplete")]
	public void Snapshot_InvalidBindingOrPayload_NeverRestoresAuthority(string damage)
	{
		var (_, record) = Snapshot();
		switch (damage)
		{
			case "version": record.ProvenanceVersion = 99; break;
			case "host": record.AuthorisationGrant!.EmploymentHostStateId++; break;
			case "task": record.AuthorisationGrant!.TaskPublicId = Guid.NewGuid().ToString(); break;
			case "plan": record.AuthorisationGrant!.EmploymentActionPlanId++; break;
			case "principal": record.CreatedByPrincipal = "{}"; break;
			case "time": record.CreatedAtTicks = -1; break;
			case "limit": foreach (var limit in record.AuthorisationGrant!.Limits) limit.Amount = -1; break;
			case "incomplete": record.AuthorisationGrant!.ExpectedPaymentScopeCount = 1; break;
		}
		Assert.IsNull(EmploymentTaskProvenancePersistence.Read(record, out var diagnostic));
		Assert.IsFalse(string.IsNullOrWhiteSpace(diagnostic));
	}

	[TestMethod]
	public void Snapshot_LegacyRow_RemainsExplicitlyUnrecorded()
	{
		Assert.IsNull(EmploymentTaskProvenancePersistence.Read(new EmploymentActiveTaskRecord(), out var diagnostic));
		StringAssert.Contains(diagnostic, "Legacy");
	}

	[TestMethod]
	public void Grant_CompletedSpendAndRevocation_CannotRestoreBudgetOrAuthority()
	{
		var (task, record) = Snapshot();
		Assert.IsTrue(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(task, task.ActionPlan.Steps[0], 0));
		task.MarkStep(0, EmploymentActionStepStatus.Completed);
		Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(task, task.ActionPlan.Steps[1], 1));
		task.RevokeAuthorisation(EmploymentPrincipal.HostSystem("test revoker"), "withdraw approval");
		EmploymentTaskProvenancePersistence.Update(task, record);
		var restored = EmploymentTaskProvenancePersistence.Read(record, out var diagnostic);
		Assert.IsNotNull(restored, diagnostic);
		Assert.IsNotNull(restored.AuthorisationGrant.RevokedAt);
		Assert.AreEqual("withdraw approval", restored.AuthorisationGrant.RevocationReason);
		Assert.AreEqual(task.AuthorisationGrant.RevokedBy, restored.AuthorisationGrant.RevokedBy);
		Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(task, task.ActionPlan.Steps[0], 0));
	}

	[TestMethod]
	public void Grant_ExpiredApproval_IsRefusedWithoutTimingAssumptions()
	{
		var (task, _) = Snapshot(expires: DateTimeOffset.MinValue);
		Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(task, task.ActionPlan.Steps[0], 0));
	}

	[TestMethod]
	public void Grant_ReusedContext_CannotKeepRevokedApprovalAlive()
	{
		var (task, _) = Snapshot();
		var context = new EmploymentTaskContext(task.Employer);
		var step = task.ActionPlan.Steps[0];
		context.AuthorisePaymentFor(step, recordRegister: false);
		context.HydrateTaskState(task, 0);
		Assert.IsTrue(context.PaymentAuthorised(step));
		task.RevokeAuthorisation(EmploymentPrincipal.HostSystem("revoker"), "approval withdrawn");
		Assert.IsFalse(context.PaymentAuthorised(step));
		context.AuthorisePaymentFor(step);
		Assert.IsFalse(context.PaymentAuthorised(step));
	}


	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FinancialFailure_QuarantinesNativeEffectAndCompletionCheckpoint(bool failCheckpoint)
	{
		var (original, record) = Snapshot();
		var step = new Mock<IEmploymentActionStep>();
		step.SetupGet(x => x.RequiresPaymentAuthorisation).Returns(true);
		step.SetupGet(x => x.RequiredAuthority).Returns(original.ActionPlan.RequiredAuthority);
		step.SetupGet(x => x.RequiredCapabilities).Returns(new HashSet<EmploymentAICapability>());
		var reason = string.Empty;
		step.Setup(x => x.CanExecute(It.IsAny<IEmploymentTaskContext>(), It.IsAny<ICharacter>(), out reason)).Returns(true);
		var effects = 0;
		step.Setup(x => x.Execute(It.IsAny<IEmploymentTaskContext>(), It.IsAny<ICharacter>())).Returns(() =>
		{
			effects++;
			if (!failCheckpoint) throw new InvalidOperationException("native outcome uncertain");
			return EmploymentActionStepResult.CompletedResult("native effect committed");
		});
		var plan = new EmploymentActionPlan([step.Object, step.Object]);
		var store = new FaultStore();
		var task = new EmploymentActiveTask(original.Employer, "fault", plan, original.CorrelationId, store,
			original.Provenance with { AuthorisationGrant = original.AuthorisationGrant with { AllowsUnboundedFinancialSteps = true } });
		store.Save = saved =>
		{
			if (failCheckpoint && saved.StepStates[0] == EmploymentActionStepStatus.Completed && !saved.RequiresFinancialReconciliation)
				throw new InvalidOperationException("completion checkpoint unavailable");
			EmploymentTaskProvenancePersistence.Update(saved, record);
		};
		task.Assign(new Mock<ICharacter>().Object);
		var context = new Mock<IEmploymentTaskContext>();
		context.SetupGet(x => x.Employer).Returns(task.Employer);
		var dispatcher = new EmploymentTaskDispatcher();
		Assert.Throws<InvalidOperationException>(() => dispatcher.AdvanceTask(task, context.Object));
		Assert.IsTrue(task.RequiresFinancialReconciliation);
		Assert.IsTrue(record.FinancialRecoveryRequired);
		task.RequeueForAdministration("retry", true, true);
		task.Assign(new Mock<ICharacter>().Object);
		Assert.IsFalse(dispatcher.AdvanceTask(task, context.Object).Success);
		Assert.AreEqual(1, effects);
		Assert.IsTrue(record.FinancialRecoveryRequired);
	}

	private static (EmploymentActiveTask Task, EmploymentActiveTaskRecord Record) Snapshot(
		EmploymentTaskSourceKind kind = EmploymentTaskSourceKind.Manual, DateTimeOffset? expires = null)
	{
		var host = new Mock<IEmploymentHost>();
		host.SetupGet(x => x.Id).Returns(12);
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1);
		var plan = new EmploymentActionPlan([
			new BankDepositActionStep(new MoneyAmount(currency.Object, 10M)),
			new BankDepositActionStep(new MoneyAmount(currency.Object, 7M))]);
		var correlation = Guid.NewGuid();
		var created = new DateTimeOffset(2026, 1, 2, 3, 4, 5, TimeSpan.Zero).AddTicks(7);
		var principal = new EmploymentPrincipal(EmploymentPrincipalKind.Character, CharacterId: 42, Label: "Issuer");
		var grant = new EmploymentAuthorisationGrant(Guid.NewGuid(), principal, plan.RequiredAuthority,
			new Dictionary<long, decimal> { [1] = 12M }, false, created, expires, correlation,
			new HashSet<string>(), new HashSet<string>());
		var provenance = new EmploymentTaskProvenance(kind,
			kind == EmploymentTaskSourceKind.ScheduledRule ? Guid.NewGuid() : null,
			kind == EmploymentTaskSourceKind.ManagerGoal ? 73 : null,
			principal, principal, grant, 17, created, created.AddHours(2));
		var task = new EmploymentActiveTask(host.Object, "task", plan, correlation, provenance);
		var record = new EmploymentActiveTaskRecord
		{
			PublicId = task.Id.ToString("D"), CorrelationId = correlation.ToString("D"),
			EmploymentHostStateId = 12, EmploymentActionPlanId = 34
		};
		EmploymentTaskProvenancePersistence.Create(task, record);
		return (task, record);
	}

	private sealed class FaultStore : IEmploymentPersistenceStore
	{
		public long StateId => 12;
		public Action<EmploymentActiveTask> Save { get; set; } = _ => { };
		public void SaveContract(EmploymentContract contract) { throw new NotSupportedException(); }
		public void SaveContractEnded(EmploymentContract contract) { throw new NotSupportedException(); }
		public void SaveContractAuthority(EmploymentContract contract) { throw new NotSupportedException(); }
		public void SaveJobOpening(JobOpening opening) { throw new NotSupportedException(); }
		public void SaveApplication(EmploymentApplication application) { throw new NotSupportedException(); }
		public void SavePayable(EmploymentPayable payable) { throw new NotSupportedException(); }
		public void SavePayableState(EmploymentPayable payable) { throw new NotSupportedException(); }
		public void SaveRegisterEntry(EmploymentRegisterEntry entry) { throw new NotSupportedException(); }
		public void SaveLedgerEntry(EmploymentLedgerEntry entry) { throw new NotSupportedException(); }
		public void SaveScheduledRule(EmploymentScheduledTaskRule rule) { throw new NotSupportedException(); }
		public void SaveScheduledRuleState(EmploymentScheduledTaskRule rule) { throw new NotSupportedException(); }
		public void DeleteScheduledRule(EmploymentScheduledTaskRule rule) { throw new NotSupportedException(); }
		public void SaveConditionPredicate(EmploymentConditionPredicate predicate) { throw new NotSupportedException(); }
		public void DeleteConditionPredicate(EmploymentConditionPredicate predicate) { throw new NotSupportedException(); }
		public void SaveScheduledRuleTemplate(EmploymentScheduledRuleTemplate template) { throw new NotSupportedException(); }
		public void DeleteScheduledRuleTemplate(EmploymentScheduledRuleTemplate template) { throw new NotSupportedException(); }
		public void SaveActiveTask(EmploymentActiveTask task) { throw new NotSupportedException(); }
		public void SaveActiveTaskState(EmploymentActiveTask task) { Save(task); }
		public void SaveManagerGoal(ManagerGoal goal) { throw new NotSupportedException(); }
		public void SaveManagerGoalState(ManagerGoal goal) { throw new NotSupportedException(); }
	}
}
