#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Helpers;
using MudSharp.Economy.Employment;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC;
using MudSharp.NPC.AI;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	private static (IEmploymentHost Host, ICharacter Manager, EmploymentActiveTask Task, EmploymentTaskContext Context)
		ApprovalFixture()
	{
		var currency = Currency().Object;
		IEmploymentHost host = new TestEmploymentHost("Approval policy", currency);
		var manager = Character(9001, "Approver").Object;
		host.Hire(manager, Offer(currency, EmploymentRole.Manager, EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases | EmploymentAuthority.DepositBusinessCash | EmploymentAuthority.CreateScheduledRules), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("bounded payments",
			new EmploymentActionPlan([
				new CataloguedActionShellStep("authorise", "approval", new MoneyAmount(currency, 10)),
				new BankDepositActionStep(new MoneyAmount(currency, 5)),
				new BankDepositActionStep(new MoneyAmount(currency, 6))
			]), manager, priority: 9, dueAt: DateTimeOffset.UtcNow.AddHours(2));
		var context = new EmploymentTaskContext(host);
		context.HydrateTaskState(task, 1);
		return (host, manager, task, context);
	}

	[TestMethod]
	public void ApprovalDocument_RoundTrip_PreservesIdentityScopesLimitsAndScheduling()
	{
		var (host, manager, task, _) = ApprovalFixture();
		var document = new EmploymentApprovalJson.Document(1, task.Id, host.EmploymentHostType.ToString(), host.Id,
			"original ordered plan", task.Provenance with { RequiresExplicitRetry = true });
		var loaded = EmploymentApprovalJson.Read<EmploymentApprovalJson.Document>(EmploymentApprovalJson.Write(document));
		Assert.IsTrue(EmploymentApprovalJson.Valid(loaded, task.Id, host, task.CorrelationId, "original ordered plan"));
		Assert.AreEqual(task.AuthorisationGrant.Id, loaded!.Provenance.AuthorisationGrant.Id);
		Assert.AreEqual(manager.Id, loaded.Provenance.AuthorisationGrant.AuthorityPrincipal!.CharacterId);
		Assert.AreEqual(10M, loaded.Provenance.AuthorisationGrant.AmountLimits[1]);
		Assert.IsTrue(loaded.Provenance.AuthorisationGrant.PaymentSourceScopes.SetEquals(task.AuthorisationGrant.PaymentSourceScopes));
		Assert.IsTrue(loaded.Provenance.AuthorisationGrant.CounterpartyScopes.SetEquals(task.AuthorisationGrant.CounterpartyScopes));
		Assert.AreEqual(9, loaded.Provenance.Priority);
		Assert.AreEqual(task.CreatedAt, loaded.Provenance.CreatedAt);
		Assert.AreEqual(task.DueAt, loaded.Provenance.DueAt);
		Assert.IsTrue(loaded.Provenance.RequiresExplicitRetry);
		Assert.IsFalse(EmploymentApprovalJson.Valid(loaded, Guid.NewGuid(), host, task.CorrelationId, "original ordered plan"));
		Assert.IsFalse(EmploymentApprovalJson.Valid(loaded, task.Id, host, task.CorrelationId, "different plan"));
	}

	[TestMethod]
	[DataRow("Version")]
	[DataRow("AmountLimits")]
	[DataRow("PaymentSourceScopes")]
	[DataRow("CounterpartyScopes")]
	public void ApprovalDocument_MissingSecurityField_FailsClosed(string field)
	{
		var (host, _, task, _) = ApprovalFixture();
		var json = JsonNode.Parse(EmploymentApprovalJson.Write(new EmploymentApprovalJson.Document(1, task.Id,
			host.EmploymentHostType.ToString(), host.Id, "bound", task.Provenance)))!;
		if (field == "Version") json.AsObject().Remove(field);
		else json["Provenance"]!["AuthorisationGrant"]!.AsObject().Remove(field);
		var loaded = EmploymentApprovalJson.Read<EmploymentApprovalJson.Document>(json.ToJsonString());
		Assert.IsFalse(EmploymentApprovalJson.Valid(loaded, task.Id, host, task.CorrelationId, "bound"));
	}

	[TestMethod]
	public void ApprovalExecution_RevocationAndIssuerAuthorityLoss_AreRecheckedInReusedContext()
	{
		var (host, manager, task, context) = ApprovalFixture();
		var step = task.ActionPlan.Steps[1];
		Assert.IsTrue(context.PaymentAuthorised(step));
		var contract = (EmploymentContract)host.EmploymentContracts.Single();
		var authority = contract.Authority;
		contract.SetAuthority(EmploymentAuthoritySet.Empty);
		Assert.IsFalse(context.PaymentAuthorised(step));
		contract.SetAuthority(authority);
		Assert.IsTrue(context.PaymentAuthorised(step));

		var grant = task.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(task, manager, grant.Id, grant.Revision, true, null, "withdraw approval", out var reason), reason);
		context.AuthorisePaymentFor(step); // An unrelated/stale explicit context approval cannot bypass a revoked task grant.
		Assert.IsFalse(context.PaymentAuthorised(step));
		Assert.IsFalse(host.TaskBoard.ChangeTaskApproval(task, manager, grant.Id, grant.Revision, false, null, "stale decision", out _));
		var revoked = task.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(task, manager, revoked.Id, revoked.Revision, false, null, "fresh decision", out reason), reason);
		Assert.AreEqual(10M, task.AuthorisationGrant.AmountLimits[1]);
		Assert.AreNotEqual(grant.Id, task.AuthorisationGrant.Id);
		Assert.IsTrue(context.PaymentAuthorised(step));
	}

	[TestMethod]
	public void ApprovalDocument_UnknownIssuerKind_FailsClosed()
	{
		var (host, _, task, _) = ApprovalFixture();
		var provenance = task.Provenance with
		{
			AuthorisationGrant = task.AuthorisationGrant with
			{
				AuthorityPrincipal = task.AuthorisationGrant.AuthorityPrincipal! with { Kind = (EmploymentPrincipalKind)999 }
			}
		};
		Assert.IsFalse(EmploymentApprovalJson.Valid(new EmploymentApprovalJson.Document(1, task.Id,
			host.EmploymentHostType.ToString(), host.Id, "bound", provenance), task.Id, host, task.CorrelationId, "bound"));
	}

	[TestMethod]
	public void ApprovalExecution_ExpiredGrantAndSharedRulePlan_DoNotBorrowAnotherTasksApproval()
	{
		var (host, manager, task, context) = ApprovalFixture();
		var expired = task.Provenance with
		{
			AuthorisationGrant = task.AuthorisationGrant with { IssuedAt = DateTimeOffset.UtcNow.AddDays(-2), ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1) }
		};
		var second = new EmploymentActiveTask(host, "same steps different approval", task.ActionPlan, task.CorrelationId, expired);
		Assert.IsTrue(context.PaymentAuthorised(task.ActionPlan.Steps[1]));
		context.HydrateTaskState(second, 1);
		Assert.IsFalse(context.PaymentAuthorised(second.ActionPlan.Steps[1]));
		var stranger = Character(9002, "Unauthorised").Object;
		Assert.IsFalse(host.TaskBoard.ChangeTaskApproval(task, stranger, task.AuthorisationGrant.Id,
			task.AuthorisationGrant.Revision, false, null, "not allowed", out _));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ApprovalRecovery_ReconciliationPreservesSpendingAndNeverExecutesNativePayment(bool executed)
	{
		var (host, manager, task, context) = ApprovalFixture();
		task.MarkStep(0, EmploymentActionStepStatus.Completed);
		task.MarkStep(1, EmploymentActionStepStatus.InProgress);
		task.HoldInterruptedFinancialSteps();
		task.ReleaseAssignment("worker disappeared");
		Assert.IsFalse(task.CanResume(out _));
		Assert.IsFalse(host.TaskBoard.RequeueActiveTask(task, manager, "ordinary retry"));
		var before = host.BusinessLedger.Entries.Count;
		var grant = task.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(task, manager, grant.Id, grant.Revision, false, null, "replacement", out var reason), reason);
		Assert.IsTrue(task.HasUncertainFinancialSteps, "Replacing approval must not clear an unresolved native attempt.");
		grant = task.AuthorisationGrant;
		new EmploymentCommandService().ExecuteForHost(manager, host, new StringStack(
			$"tasks approval reconcile {task.Id:D} {grant.Id:D} {grant.Revision} 2 {(executed ? "executed" : "not-executed")} verified-ledger manager inspected balance and transaction"));
		Assert.AreEqual(before, host.BusinessLedger.Entries.Count, "Reconciliation itself must not execute financial operations.");
		Assert.IsFalse(task.HasUncertainFinancialSteps);
		Assert.AreEqual(executed ? EmploymentActionStepStatus.Completed : EmploymentActionStepStatus.Pending, task.StepStates[1]);
		Assert.IsTrue(task.Provenance.RequiresExplicitRetry);
		host.TaskBoard.AuditActiveTaskAssignments();
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status, "Assignment auditing must not acknowledge retry after reconciliation.");
		Assert.IsTrue(host.TaskBoard.RetryActiveTask(task, manager, "explicitly resume reconciled work"));
		context.HydrateTaskState(task, executed ? 2 : 1);
		Assert.AreEqual(!executed, context.PaymentAuthorised(task.ActionPlan.Steps[executed ? 2 : 1]),
			"A completed payment remains charged against the original limit after replacement.");
	}

	[TestMethod]
	public void ApprovalRecovery_CancelledUncertainTask_ContinuesToBlockAutomationIdempotencyKey()
	{
		var (host, manager, original, _) = ApprovalFixture();
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("automation attempt", original.ActionPlan,
			manager, idempotencyKey: "one-payment");
		task.MarkStep(1, EmploymentActionStepStatus.InProgress);
		task.HoldInterruptedFinancialSteps();
		task.Cancel("cancel requested during unknown payment outcome");
		Assert.IsTrue(host.TaskBoard.HasBlockingActiveTask("one-payment"));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ApprovalExecution_FinancialExecuteOrFlushException_HoldsSameProcessAgainstReplay(bool throwInExecute)
	{
		var (host, manager, _, _) = ApprovalFixture();
		var calls = 0;
		var world = new Mock<IFuturemud>();
		var saves = new Mock<ISaveManager>();
		world.SetupGet(x => x.SaveManager).Returns(saves.Object);
		Mock.Get(manager).SetupGet(x => x.Gameworld).Returns(world.Object);
		if (!throwInExecute) saves.Setup(x => x.Flush()).Throws(new InvalidOperationException("lost commit acknowledgement"));
		var step = new ApprovalProbeStep(() =>
		{
			calls++;
			if (throwInExecute) throw new InvalidOperationException("native operation committed before failure");
		});
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("native failure", new EmploymentActionPlan([step]), manager);
		task.Assign(manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		Assert.IsFalse(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(task.HasUncertainFinancialSteps);
		Assert.IsFalse(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsFalse(host.TaskBoard.RequeueActiveTask(task, manager, "not acknowledged"));
		Assert.AreEqual(1, calls);
	}

	[TestMethod]
	public void ApprovalRecovery_FailedNonfinancialTask_RemainsRetryable()
	{
		var (host, manager, _, _) = ApprovalFixture();
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("ordinary failure",
			new EmploymentActionPlan([new ApprovalProbeStep(() => { }, false)]), manager);
		task.MarkStep(0, EmploymentActionStepStatus.Failed);
		Assert.IsTrue(host.TaskBoard.RetryActiveTask(task, manager, "corrected ordinary failure"));
		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.AreEqual(EmploymentActionStepStatus.Pending, task.StepStates[0]);
	}

	[TestMethod]
	public void ApprovalRecovery_UnknownStoredStepStatus_QuarantinesAndPreservesCompletedEvidence()
	{
		var (host, manager, original, _) = ApprovalFixture();
		var task = new EmploymentActiveTask(original.Id, host, original.Name, original.ActionPlan,
			EmploymentTaskStatus.Pending, manager, null,
			[EmploymentActionStepStatus.Completed, (EmploymentActionStepStatus)999, EmploymentActionStepStatus.Pending],
			Enumerable.Repeat(EmploymentActionStepOperationalState.Empty, 3), original.CorrelationId, string.Empty, null!, original.Provenance);
		Assert.IsNotNull(task.Provenance.RecoveryIssue);
		Assert.AreEqual(EmploymentActionStepStatus.Completed, task.StepStates[0]);
		Assert.AreEqual(999, (int)task.StepStates[1]);
		Assert.IsFalse(new EmploymentTaskDispatcher().AdvanceTask(task, new EmploymentTaskContext(host)).Success);
	}

	[TestMethod]
	[DataRow("revoked", "revoked")]
	[DataRow("issuer", "no longer has the required host authority")]
	[DataRow("legacy", "auditable payment authorisation")]
	[DataRow("held", "reconciliation")]
	[DataRow("retry", "explicit retry or requeue")]
	public void ApprovalDiagnostics_TaskBoundFinancialPreview_ReportsActualRecoveryBlocker(string state, string expected)
	{
		var currency = Currency().Object;
		IEmploymentHost host = new TestEmploymentHost("Diagnostic host", currency);
		var manager = Character(9101, "Manager").Object;
		var authority = EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases;
		host.Hire(manager, Offer(currency, EmploymentRole.Manager, authority), null);
		var world = new Mock<IFuturemud>();
		var ai = (EmploymentWorkerAI)Activator.CreateInstance(typeof(EmploymentWorkerAI),
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			new object[] { world.Object, "Preview worker", false }, CultureInfo.InvariantCulture)!;
		var employee = Character(9102, "Worker");
		employee.As<INPC>().SetupGet(x => x.AIs).Returns([ai]);
		host.Hire(employee.Object, Offer(currency, EmploymentRole.Employee, EmploymentAuthority.ApprovePurchases), null);
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("financial preview",
			new EmploymentActionPlan([new CataloguedActionShellStep("authorise", "bounded payment", new MoneyAmount(currency, 3))]), manager);
		var service = new EmploymentCommandService();
		StringAssert.Contains(service.RenderTaskDiagnostics(manager, host), "Preview worker AI eligible");

		switch (state)
		{
			case "revoked":
				Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(task, manager, task.AuthorisationGrant.Id,
					task.AuthorisationGrant.Revision, true, null, "withdraw approval", out _));
				break;
			case "issuer":
				((EmploymentContract)host.EmploymentContracts.Single(x => x.Employee.Id == manager.Id)).SetAuthority(EmploymentAuthoritySet.Empty);
				break;
			case "legacy":
				task.CommitApprovalTransition(task.Provenance with
					{ AuthorisationGrant = task.AuthorisationGrant with { Authority = EmploymentAuthoritySet.Empty } },
					new EmploymentRegisterEntry(task.CorrelationId, EmploymentRegisterEntryType.TaskApprovalChanged,
						host, manager, "Compatibility fixture has no approval authority.", DateTimeOffset.UtcNow));
				break;
			case "held":
				task.MarkStep(0, EmploymentActionStepStatus.InProgress);
				task.HoldInterruptedFinancialSteps();
				break;
			case "retry":
				task.MarkStep(0, EmploymentActionStepStatus.InProgress);
				task.HoldInterruptedFinancialSteps();
				Assert.IsTrue(host.TaskBoard.ReconcileFinancialStep(task, manager, task.AuthorisationGrant.Id,
					task.AuthorisationGrant.Revision, 1, false, "native ledger unchanged", "verified no payment", out _));
				break;
		}

		var rendered = service.RenderTaskDiagnostics(manager, host);
		StringAssert.Contains(rendered, expected);
		StringAssert.Contains(rendered, "next step 1", "An interrupted financial step must retain its visible step number.");
		Assert.IsFalse(rendered.Contains("Preview worker AI eligible"), "The preview must use the same bound approval and recovery policy as dispatch.");
	}

	[TestMethod]
	[DataRow("retry")]
	[DataRow("requeue")]
	public void ApprovalRecovery_TaskCommand_RequiresAuthorityReasonAndResolvedFinancialOutcome(string command)
	{
		var (host, manager, task, _) = ApprovalFixture();
		task.MarkStep(0, EmploymentActionStepStatus.Completed);
		task.MarkStep(1, EmploymentActionStepStatus.InProgress);
		task.HoldInterruptedFinancialSteps();
		task.Block("interrupted payment");
		var service = new EmploymentCommandService();
		service.ExecuteForHost(manager, host, new StringStack($"tasks {command} {task.Id:D} ordinary recovery"));
		Assert.IsTrue(task.HasUncertainFinancialSteps);
		Assert.AreEqual(EmploymentActionStepStatus.InProgress, task.StepStates[1]);
		var grant = task.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ReconcileFinancialStep(task, manager, grant.Id, grant.Revision, 2,
			false, "native ledger unchanged", "verified no payment", out var reason), reason);
		Assert.IsTrue(task.Provenance.RequiresExplicitRetry);
		host.TaskBoard.AuditActiveTaskAssignments();
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		service.ExecuteForHost(Character(9103, "Outsider").Object, host, new StringStack($"tasks {command} {task.Id:D} unauthorised recovery"));
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		service.ExecuteForHost(manager, host, new StringStack($"tasks {command} {task.Id:D}"));
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status, "An explicit audit reason is required.");
		grant = task.AuthorisationGrant;
		var ledgerCount = host.BusinessLedger.Entries.Count;
		service.ExecuteForHost(manager, host, new StringStack($"tasks {command} {task.Id:D} inspected native ledger"));
		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsFalse(task.Provenance.RequiresExplicitRetry);
		Assert.AreEqual(EmploymentActionStepStatus.Completed, task.StepStates[0]);
		Assert.AreEqual(EmploymentActionStepStatus.Pending, task.StepStates[1]);
		Assert.AreEqual(grant, task.AuthorisationGrant, "Retrying work cannot replace or expand its approval.");
		Assert.AreEqual(ledgerCount, host.BusinessLedger.Entries.Count);
	}

	[TestMethod]
	public void ApprovalRecovery_ManualAssignment_CannotBypassExplicitRetryForNonfinancialFollowup()
	{
		var (host, manager, _, _) = ApprovalFixture();
		var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("payment and followup",
			new EmploymentActionPlan([new ApprovalProbeStep(() => { }), new ApprovalProbeStep(() => { }, false)]), manager);
		task.MarkStep(0, EmploymentActionStepStatus.InProgress);
		task.HoldInterruptedFinancialSteps();
		Assert.IsTrue(host.TaskBoard.ReconcileFinancialStep(task, manager, task.AuthorisationGrant.Id,
			task.AuthorisationGrant.Revision, 1, true, "verified native payment", "resume followup deliberately", out var reason), reason);
		var context = new EmploymentTaskContext(host);
		Assert.IsFalse(host.TaskBoard.AssignActiveTask(task, manager, context, manager, "assignment is not retry"));
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.IsTrue(task.Provenance.RequiresExplicitRetry);
		Assert.IsTrue(host.TaskBoard.RetryActiveTask(task, manager, "explicitly resume followup"));
		Assert.IsTrue(host.TaskBoard.AssignActiveTask(task, manager, context, manager, "assign resumed work"));
	}

	private sealed class ApprovalProbeStep(Action action, bool financial = true) : EmploymentActionStepBase(
		EmploymentActionStepType.Command, EmploymentAuthority.ApprovePurchases, [], false, financial)
	{
		public override EmploymentActionStepResult Execute(IEmploymentTaskContext context, ICharacter actor)
		{
			action();
			return EmploymentActionStepResult.CompletedResult("Probe completed.");
		}
	}
}
