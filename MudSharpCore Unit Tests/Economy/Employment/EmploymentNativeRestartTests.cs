#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Economy.Employment;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	/// <summary>Separate test-host processes share only a verified disposable MySQL database.</summary>
	[TestMethod]
	[TestCategory("Integration")]
	public void EmploymentNative_ProcessRestart_PreservesApprovalAndRefusal()
	{
		var connection = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION");
		if (string.IsNullOrWhiteSpace(connection))
		{
			Assert.Inconclusive("Requires the owned employment restart scenario; never uses a default database.");
		}
		var settings = new MySqlConnectionStringBuilder(connection);
		var phase = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_PHASE");
		Assert.AreEqual("127.0.0.1", settings.Server);
		Assert.AreEqual(phase == "upgrade" ? "employment_upgrade" : "authored_celestial_smoke", settings.Database);
		Assert.AreEqual("root", settings.UserID);
		Assert.AreEqual(string.Empty, settings.Password);
		var expectedData = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_DATADIR");
		Assert.IsFalse(string.IsNullOrWhiteSpace(expectedData));
		using (var sql = new MySqlConnection(connection))
		{
			sql.Open();
			using var identity = new MySqlCommand("SELECT @@datadir", sql);
			Assert.AreEqual(Path.GetFullPath(expectedData!).TrimEnd('\\', '/'),
				Path.GetFullPath((string)identity.ExecuteScalar()!).TrimEnd('\\', '/'), true);
		}
		Assert.IsTrue(phase is "upgrade" or "write" or "read" or "revoked");
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseMySql(connection, ServerVersion.AutoDetect(connection)).Options;
		using var context = new FuturemudDatabaseContext(options);
		if (phase == "upgrade")
		{
			var migrations = context.Database.GetMigrations().ToList();
			Assert.IsTrue(migrations.Last().EndsWith("_PersistEmploymentTaskAuthorisation", StringComparison.Ordinal));
			context.Database.Migrate();
			VerifyLegacyUpgradeRow(context);
			context.GetService<IMigrator>().Migrate(migrations[^2]);
			using (var sql = new MySqlConnection(connection))
			{
				sql.Open();
				using var count = new MySqlCommand("SELECT COUNT(*) FROM EmploymentActiveTasks WHERE Id=991071 AND Name='F03 upgrade legacy'", sql);
				Assert.AreEqual(1L, Convert.ToInt64(count.ExecuteScalar()));
			}
			context.GetService<IMigrator>().Migrate(migrations[^1]);
			VerifyLegacyUpgradeRow(context);
			Assert.IsFalse(context.Database.HasPendingModelChanges());
			return;
		}
		var savedFMDB = CaptureFMDBState();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			currency.SetupGet(x => x.Id).Returns(context.Currencies.Select(x => x.Id).First());
			var manager = Character(991071, "Native Manager").Object;
			var world = Gameworld(currency.Object, new Dictionary<long, ICharacter> { [manager.Id] = manager });
			IEmploymentHost host = new PersistedEmploymentHost(991071, "F03 Native Store", world.Object, currency.Object);
			if (phase == "write")
			{
				Assert.IsFalse(context.EmploymentHostStates.Any(x => x.HostId == host.Id), "Writer requires a fresh owned fixture.");
				host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
					EmploymentAuthority.AssignTasks | EmploymentAuthority.CancelTasks |
					EmploymentAuthority.ApprovePurchases | EmploymentAuthority.DepositBusinessCash), null);
				foreach (var name in new[] { "resume", "budget", "legacy", "corrupt", "missing-scope", "ambiguous", "expired", "rule", "goal" })
				{
					var plan = new EmploymentActionPlan([
						new CataloguedActionShellStep("authorise", "native approval", new MoneyAmount(currency.Object, 12M)),
						new BankDepositActionStep(new MoneyAmount(currency.Object, name == "budget" ? 10M : 5M)),
						new BankDepositActionStep(new MoneyAmount(currency.Object, 7M))]);
					var correlation = Guid.NewGuid();
					var source = name == "rule" ? EmploymentTaskSourceKind.ScheduledRule :
						name == "goal" ? EmploymentTaskSourceKind.ManagerGoal : EmploymentTaskSourceKind.Manual;
					var principal = EmploymentPrincipal.ForCharacter(manager);
					var provenance = EmploymentTaskBoard.CreateTaskProvenance(host, plan, correlation, source,
						principal, principal, 19, EmploymentClock.CurrentInstant(host),
						EmploymentClock.CurrentInstant(host).AddHours(2),
						source == EmploymentTaskSourceKind.ScheduledRule ? Guid.Parse("455cda22-53ef-40ee-b6da-a7b0260b76ff") : null,
						source == EmploymentTaskSourceKind.ManagerGoal ? 713 : null);
					if (name == "expired")
					{
						provenance = provenance with { AuthorisationGrant = provenance.AuthorisationGrant with { ExpiresAt = DateTimeOffset.MinValue } };
					}
					var task = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask(name, plan, manager,
						correlationId: correlation, provenance: provenance);
					task.MarkStep(0, EmploymentActionStepStatus.Completed);
					task.MarkStep(1, name == "ambiguous" ? EmploymentActionStepStatus.InProgress : EmploymentActionStepStatus.Completed,
						EmploymentActionStepOperationalState.Empty with { TransactionReference = $"f03-recorded-{name}" });
				}
				var legacy = context.EmploymentActiveTasks.Single(x => x.Name == "legacy");
				context.EmploymentTaskGrants.Remove(legacy.AuthorisationGrant!);
				legacy.AuthorisationGrant = null;
				legacy.ProvenanceVersion = null;
				context.EmploymentActiveTasks.Single(x => x.Name == "corrupt").ProvenanceVersion = 99;
				var incomplete = context.EmploymentActiveTasks.Single(x => x.Name == "missing-scope");
				var missingScopes = context.EmploymentTaskGrantScopes.Where(x => x.EmploymentActiveTaskId == incomplete.Id).ToList();
				Assert.IsTrue(missingScopes.Count > 0, "Fixture needs a declared scope to test loss of child rows.");
				context.EmploymentTaskGrantScopes.RemoveRange(missingScopes);
				context.SaveChanges();
				return;
			}

			var tasks = host.TaskBoard.ActiveTasks.ToDictionary(x => x.Name);
			Assert.AreEqual(9, tasks.Count);
			foreach (var name in new[] { "resume", "budget", "ambiguous", "expired", "rule", "goal" })
			{
				var task = tasks[name];
				Assert.IsTrue(task.HasRecordedProvenance, name);
				Assert.AreEqual(19, task.Priority);
				Assert.AreEqual(manager.Id, task.CreatedByPrincipal.CharacterId);
				Assert.IsNotNull(task.DueAt);
				Assert.AreEqual($"f03-recorded-{name}", task.StepOperationalStates[1].TransactionReference);
				Assert.AreEqual(12M, task.AuthorisationGrant.AmountLimits[currency.Object.Id]);
			}
			Assert.AreEqual(EmploymentTaskSourceKind.ScheduledRule, tasks["rule"].SourceKind);
			Assert.AreEqual(Guid.Parse("455cda22-53ef-40ee-b6da-a7b0260b76ff"), tasks["rule"].SourceRuleId);
			Assert.AreEqual(EmploymentTaskSourceKind.ManagerGoal, tasks["goal"].SourceKind);
			Assert.AreEqual(713L, tasks["goal"].SourceGoalId);
			foreach (var name in new[] { "legacy", "corrupt", "missing-scope" })
			{
				Assert.IsFalse(tasks[name].HasRecordedProvenance);
				Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep((EmploymentActiveTask)tasks[name], tasks[name].ActionPlan.Steps[2], 2));
			}
			Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep((EmploymentActiveTask)tasks["budget"], tasks["budget"].ActionPlan.Steps[2], 2));
			Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep((EmploymentActiveTask)tasks["expired"], tasks["expired"].ActionPlan.Steps[2], 2));
			Assert.IsTrue(((EmploymentActiveTask)tasks["ambiguous"]).RequiresFinancialReconciliation);
			Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep((EmploymentActiveTask)tasks["ambiguous"], tasks["ambiguous"].ActionPlan.Steps[1], 1));
			var resume = (EmploymentActiveTask)tasks["resume"];
			Assert.AreEqual(2, resume.NextStepIndex);
			Assert.AreEqual(EmploymentActionStepStatus.Completed, resume.StepStates[1]);
			if (phase == "read")
			{
				// A reset/requeue must not erase the durable interrupted-operation quarantine.
				((EmploymentActiveTask)tasks["ambiguous"]).MarkStep(1, EmploymentActionStepStatus.Pending);
				Assert.IsTrue(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(resume, resume.ActionPlan.Steps[2], 2));
				Assert.IsTrue(host.TaskBoard.RevokeTaskAuthorisation(resume, manager, "native restart revocation"));
				return;
			}
			Assert.IsNotNull(resume.AuthorisationGrant.RevokedAt);
			Assert.AreEqual("native restart revocation", resume.AuthorisationGrant.RevocationReason);
			Assert.AreEqual(manager.Id, resume.AuthorisationGrant.RevokedBy!.CharacterId);
			Assert.IsFalse(EmploymentTaskDispatcher.TaskGrantAuthorisesStep(resume, resume.ActionPlan.Steps[2], 2));
		}
		finally { RestoreFMDBState(savedFMDB); }
	}

	private static void VerifyLegacyUpgradeRow(FuturemudDatabaseContext context)
	{
		var row = context.EmploymentActiveTasks.AsNoTracking().Single(x => x.Id == 991071);
		Assert.AreEqual("F03 upgrade legacy", row.Name);
		Assert.AreEqual("f03-legacy-proof", row.IdempotencyKey);
		Assert.IsNull(row.ProvenanceVersion);
		Assert.IsNull(EmploymentTaskProvenancePersistence.Read(row, out var diagnostic));
		StringAssert.Contains(diagnostic, "Legacy");
	}
}
