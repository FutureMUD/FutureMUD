#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Shops;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using MudSharp.TimeAndDate;
using MySqlConnector;
using RuntimeBankAccount = MudSharp.Economy.Banking.BankAccount;

namespace MudSharp_Unit_Tests.Economy.Employment;

public partial class UnifiedEmploymentDispatchTests
{
	private sealed record NativeEmploymentFixture(long SourceAccount, long TargetAccount, long CharacterId,
		long CurrencyId, long BankId, long AccountTypeId, Guid ContinuedTask, Guid InterruptedTask, Guid RevokedTask,
		Guid GrantId, DateTimeOffset CreatedAt, DateTimeOffset? DueAt, int ProducerProcessId);

	[TestMethod]
	[TestCategory("Integration")]
	public void EmploymentRestartNative_IndependentProcessesPreserveApprovalsAndNeverReplayTransfers()
	{
		var connection = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_CONNECTION");
		if (string.IsNullOrWhiteSpace(connection)) Assert.Inconclusive("Requires the owned EmploymentRestartSmoke runner.");
		var settings = new MySqlConnectionStringBuilder(connection);
		var expectedPort = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_PORT");
		var expectedData = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_DATADIR") ?? throw new InvalidOperationException("Missing owned data directory.");
		var expectedUuid = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_UUID");
		var artifacts = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_ARTIFACTS") ?? throw new InvalidOperationException("Missing owned artifact directory.");
		var repository = new DirectoryInfo(AppContext.BaseDirectory);
		while (repository is not null && !File.Exists(Path.Combine(repository.FullName, "MudSharp.sln"))) repository = repository.Parent;
		Assert.IsNotNull(repository, "Native tests must run from this repository's built test assembly.");
		var ownedRoot = Path.Combine(repository.FullName, ".artifacts", "authored-celestials") + Path.DirectorySeparatorChar;
		Assert.IsTrue(Path.GetFullPath(expectedData).StartsWith(ownedRoot, StringComparison.OrdinalIgnoreCase), "Refusing a data directory outside owned acceptance artifacts.");
		var runDirectory = Directory.GetParent(Path.GetFullPath(expectedData))!.Parent!.FullName;
		Assert.AreEqual(runDirectory, Directory.GetParent(Path.GetFullPath(artifacts))!.FullName, true);
		StringAssert.StartsWith(new DirectoryInfo(artifacts).Name, "employment-attempt-");
		Assert.IsTrue(settings.Server == "127.0.0.1" && settings.Port.ToString() == expectedPort &&
			settings.Database == "authored_celestial_smoke" && settings.UserID == "root" && string.IsNullOrEmpty(settings.Password), "Refusing an unowned endpoint.");
		using (var sql = new MySqlConnection(connection))
		{
			sql.Open();
			using var query = new MySqlCommand("SELECT @@server_uuid, @@datadir, @@port", sql);
			using var reader = query.ExecuteReader();
			Assert.IsTrue(reader.Read());
			Assert.AreEqual(expectedUuid, reader.GetString(0));
			Assert.AreEqual(Path.GetFullPath(expectedData).TrimEnd('\\', '/'), Path.GetFullPath(reader.GetString(1)).TrimEnd('\\', '/'), true);
			Assert.AreEqual(settings.Port, reader.GetUInt32(2));
		}
		var previousConnection = FMDB.ConnectionString;
		try
		{
			FMDB.ConnectionString = connection!;
			using var isolated = FMDB.BeginIsolatedScope();
			using var database = new FMDB();
			var context = FMDB.Context;
			context.Database.Migrate();
			Assert.IsFalse(context.Database.HasPendingModelChanges());
			var manifest = Path.Combine(artifacts, "employment-fixture.json");
			var phase = Environment.GetEnvironmentVariable("FUTUREMUD_EMPLOYMENT_NATIVE_PHASE");
			if (phase == "write") NativeEmploymentWrite(context, manifest);
			else if (phase == "read") NativeEmploymentRead(context, manifest);
			else Assert.Fail("Owned restart phase must be write or read.");
		}
		finally { FMDB.ConnectionString = previousConnection; }
	}

	private static void NativeEmploymentWrite(FuturemudDatabaseContext context, string manifest)
	{
		Assert.IsFalse(File.Exists(manifest), "Refusing to overwrite an earlier producer receipt.");
		var currencyId = context.Currencies.Select(x => x.Id).First();
		var characterId = context.Characters.Select(x => x.Id).First();
		var zoneId = context.EconomicZones.Select(x => x.Id).FirstOrDefault();
		if (zoneId == 0)
		{
			var zone = new MudSharp.Models.EconomicZone
			{
				Name = "Employment acceptance zone", CurrencyId = currencyId,
				ZoneForTimePurposesId = context.Zones.Select(x => x.Id).First(),
				ReferenceClockId = context.Clocks.Select(x => x.Id).First(),
				ReferenceCalendarId = context.Calendars.Select(x => x.Id).First(), ReferenceTime = "0:0:0",
				EstateDefaultDiscoverTime = "0", EstateClaimPeriodLength = "0"
			};
			context.EconomicZones.Add(zone); context.SaveChanges(); zoneId = zone.Id;
		}
		var bank = new MudSharp.Models.Bank { Name = "Employment acceptance bank", Code = "ER", EconomicZoneId = zoneId,
			PrimaryCurrencyId = currencyId, MaximumBankAccountsPerCustomer = 10 };
		context.Banks.Add(bank); context.SaveChanges();
		var type = new MudSharp.Models.BankAccountType { Name = "Acceptance current", CustomerDescription = "Owned native acceptance fixture", BankId = bank.Id };
		context.BankAccountTypes.Add(type); context.SaveChanges();
		MudSharp.Models.BankAccount Account(int number, decimal balance) => new()
		{
			Name = $"Employment acceptance {number}", AccountNumber = number, BankId = bank.Id, BankAccountTypeId = type.Id,
			CurrentBalance = balance, AccountCreationDate = "Never", AccountStatus = (int)BankAccountStatus.Active,
			AuthorisedBankPaymentItems = string.Empty
		};
		var source = Account(1, 100);
		var target = Account(2, 0);
		context.BankAccounts.AddRange(source, target); context.SaveChanges();
		var initial = new NativeEmploymentFixture(source.Id, target.Id, characterId, currencyId, bank.Id, type.Id,
			Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, default, null, Environment.ProcessId);
		var (host, manager, _) = NativeEmploymentHost(context, initial);
		var authority = EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.ManageStockRules | EmploymentAuthority.WithdrawBusinessCash;
		host.Employment.Hire(manager, Offer(((IShop)host).Currency, EmploymentRole.Manager, authority), null);
		var legacy = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("legacy native transfer", NativeTransferPlan((IShop)host, target.Id, 1), manager);
		var migrationIds = context.Database.GetMigrations().ToArray();
		var position = Array.FindIndex(migrationIds, x => x.EndsWith("_PersistEmploymentTaskApprovals", StringComparison.Ordinal));
		Assert.IsTrue(position > 0);
		context.GetService<IMigrator>().Migrate(migrationIds[position - 1]);
		context.ChangeTracker.Clear();
		context.Database.Migrate();
		context.ChangeTracker.Clear();
		(host, manager, _) = NativeEmploymentHost(context, initial);
		legacy = (EmploymentActiveTask)host.TaskBoard.ActiveTasks.Single(x => x.Id == legacy.Id);
		Assert.IsTrue(legacy.LegacyApproval);
		Assert.AreEqual(EmploymentAuthority.None, legacy.AuthorisationGrant.Authority.Authorities);
		legacy.Block("legacy work is awaiting explicit approval");
		context.ChangeTracker.Clear();
		(host, manager, _) = NativeEmploymentHost(context, initial);
		legacy = (EmploymentActiveTask)host.TaskBoard.ActiveTasks.Single(x => x.Id == legacy.Id);
		Assert.IsTrue(legacy.LegacyApproval, "An ordinary blocked/state save and reload must retain legacy reapproval eligibility.");
		var old = legacy.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(legacy, manager, old.Id, old.Revision, false, null, "explicit legacy approval", out var reason), reason);
		Assert.IsFalse(legacy.LegacyApproval);
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(legacy, manager, legacy.AuthorisationGrant.Id,
			legacy.AuthorisationGrant.Revision, true, null, "legacy approval revoked", out reason), reason);

		var continued = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("continued native transfer",
			NativeTransferPlan((IShop)host, target.Id, 5, 7), manager, priority: 17, dueAt: DateTimeOffset.UtcNow.AddHours(1));
		continued.Assign(manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var taskContext = new EmploymentTaskContext(host);
		for (var i = 0; i < 3; i++)
		{
			var result = dispatcher.AdvanceTask(continued, taskContext);
			Assert.IsTrue(result.Success && result.Completed, result.Message);
		}
		var interrupted = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("interrupted native transfer",
			NativeTransferPlan((IShop)host, target.Id, 3), manager);
		interrupted.Assign(manager);
		for (var i = 0; i < 2; i++) Assert.IsTrue(dispatcher.AdvanceTask(interrupted, taskContext).Success);
		var index = interrupted.NextStepIndex;
		interrupted.MarkStep(index, EmploymentActionStepStatus.InProgress);
		taskContext.HydrateTaskState(interrupted, index);
		var nativeResult = interrupted.ActionPlan.Steps[index].Execute(taskContext, manager);
		Assert.IsTrue(nativeResult.Success, nativeResult.Message);
		manager.Gameworld.SaveManager.Flush();
		// Deliberately omit the task result checkpoint: emulate losing the process after native commit.
		AssertNativeBalances(context, initial, 92, 8, 4);
		var saved = initial with { ContinuedTask = continued.Id, InterruptedTask = interrupted.Id, RevokedTask = legacy.Id,
			GrantId = continued.AuthorisationGrant.Id, CreatedAt = continued.CreatedAt, DueAt = continued.DueAt };
		File.WriteAllText(manifest, JsonSerializer.Serialize(saved));
	}

	private static void NativeEmploymentRead(FuturemudDatabaseContext context, string manifest)
	{
		var saved = JsonSerializer.Deserialize<NativeEmploymentFixture>(File.ReadAllText(manifest))!;
		Assert.AreNotEqual(saved.ProducerProcessId, Environment.ProcessId, "Reader must be an independent test host process.");
		var (host, manager, _) = NativeEmploymentHost(context, saved);
		var continued = (EmploymentActiveTask)host.TaskBoard.ActiveTasks.Single(x => x.Id == saved.ContinuedTask);
		var interrupted = (EmploymentActiveTask)host.TaskBoard.ActiveTasks.Single(x => x.Id == saved.InterruptedTask);
		var revoked = host.TaskBoard.ActiveTasks.Single(x => x.Id == saved.RevokedTask);
		Assert.AreEqual(saved.GrantId, continued.AuthorisationGrant.Id);
		Assert.AreEqual(saved.CreatedAt, continued.CreatedAt);
		Assert.AreEqual(saved.DueAt, continued.DueAt);
		Assert.AreEqual(17, continued.Priority);
		Assert.AreEqual(saved.CharacterId, continued.AuthorisationGrant.AuthorityPrincipal!.CharacterId);
		Assert.AreEqual(12M, continued.AuthorisationGrant.AmountLimits[saved.CurrencyId]);
		Assert.AreEqual(EmploymentActionStepStatus.Completed, continued.StepStates[2]);
		Assert.IsTrue(revoked.AuthorisationGrant.RevokedAt.HasValue);
		var staleRevoked = (EmploymentActiveTask)NativeEmploymentHost(context, saved).Host.TaskBoard.ActiveTasks
			.Single(x => x.Id == saved.RevokedTask);
		var revokedApproval = revoked.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(revoked, manager, revokedApproval.Id, revokedApproval.Revision,
			true, null, "renew durable revocation before testing a stale ordinary save", out var staleReason), staleReason);
		context.ChangeTracker.Clear();
		Assert.ThrowsExactly<DbUpdateConcurrencyException>(() => staleRevoked.Block("stale task must not restore approval"));
		context.ChangeTracker.Clear();
		var storedRevoked = NativeEmploymentHost(context, saved).Host.TaskBoard.ActiveTasks.Single(x => x.Id == saved.RevokedTask);
		Assert.AreEqual(revoked.AuthorisationGrant.Revision, storedRevoked.AuthorisationGrant.Revision);
		Assert.IsTrue(storedRevoked.AuthorisationGrant.RevokedAt.HasValue);
		var taskContext = new EmploymentTaskContext(host);
		taskContext.HydrateTaskState(revoked, 2);
		Assert.IsFalse(taskContext.PaymentAuthorised(revoked.ActionPlan.Steps[2]));
		Assert.IsTrue(interrupted.HasUncertainFinancialSteps);
		Assert.IsFalse(host.TaskBoard.RetryActiveTask(interrupted, manager, "ordinary retry cannot acknowledge money"));
		var approval = interrupted.AuthorisationGrant;
		Assert.IsTrue(host.TaskBoard.ChangeTaskApproval(interrupted, manager, approval.Id, approval.Revision, false, null, "replace while held", out var reason), reason);
		Assert.IsTrue(interrupted.HasUncertainFinancialSteps);
		AssertNativeBalances(context, saved, 92, 8, 4);
		var contract = (EmploymentContract)host.EmploymentContracts.Single();
		var originalAuthority = contract.Authority;
		contract.SetAuthority(EmploymentAuthoritySet.Empty);
		taskContext.HydrateTaskState(continued, 3);
		Assert.IsFalse(taskContext.PaymentAuthorised(continued.ActionPlan.Steps[3]));
		contract.SetAuthority(originalAuthority);
		Assert.IsTrue(taskContext.PaymentAuthorised(continued.ActionPlan.Steps[3]));
		var dispatcher = new EmploymentTaskDispatcher();
		var result = dispatcher.AdvanceTask(continued, taskContext);
		Assert.IsTrue(result.Success && result.Completed, result.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, continued.Status);
		Assert.IsTrue(dispatcher.AdvanceTask(continued, taskContext).Completed);
		AssertNativeBalances(context, saved, 85, 15, 6);
		approval = interrupted.AuthorisationGrant;
		var reference = string.Join(",", context.BankAccountTransactions.AsNoTracking()
			.Where(x => (x.BankAccountId == saved.SourceAccount || x.BankAccountId == saved.TargetAccount) && x.Amount == 3)
			.Select(x => x.Id).ToArray());
		Assert.IsFalse(string.IsNullOrWhiteSpace(reference));
		var staleInterrupted = (EmploymentActiveTask)NativeEmploymentHost(context, saved).Host.TaskBoard.ActiveTasks
			.Single(x => x.Id == saved.InterruptedTask);
		Assert.IsTrue(host.TaskBoard.ReconcileFinancialStep(interrupted, manager, approval.Id, approval.Revision, 3,
			true, reference, "native paired transactions verified", out reason), reason);
		Assert.AreEqual(EmploymentTaskStatus.Completed, interrupted.Status);
		Assert.IsTrue(dispatcher.AdvanceTask(interrupted, taskContext).Completed);
		AssertNativeBalances(context, saved, 85, 15, 6);
		context.ChangeTracker.Clear();
		Assert.ThrowsExactly<DbUpdateConcurrencyException>(() => staleInterrupted.Block("stale task must not undo reconciliation"));
		context.ChangeTracker.Clear();
		var reloaded = NativeEmploymentHost(context, saved).Host;
		Assert.IsTrue(reloaded.TaskBoard.ActiveTasks.Single(x => x.Id == saved.RevokedTask).AuthorisationGrant.RevokedAt.HasValue);
		Assert.AreEqual(EmploymentTaskStatus.Completed, reloaded.TaskBoard.ActiveTasks.Single(x => x.Id == saved.InterruptedTask).Status);
		Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.TaskFinancialStepReconciled));
	}

	private static EmploymentActionPlan NativeTransferPlan(IShop host, long targetId, params decimal[] amounts) => new(
		new IEmploymentActionStep[]
		{
			new CataloguedActionShellStep("authorise", "native approval", new MoneyAmount(host.Currency, amounts.Sum())),
			new CataloguedActionShellStep("reserve", "native reservation", new MoneyAmount(host.Currency, amounts.Sum()))
		}.Concat(amounts.Select(x => new BankAccountTransferActionStep(targetId.ToString(), new MoneyAmount(host.Currency, x)))));

	private static void AssertNativeBalances(FuturemudDatabaseContext context, NativeEmploymentFixture fixture,
		decimal source, decimal target, int transactions)
	{
		Assert.AreEqual(source, context.BankAccounts.AsNoTracking().Single(x => x.Id == fixture.SourceAccount).CurrentBalance);
		Assert.AreEqual(target, context.BankAccounts.AsNoTracking().Single(x => x.Id == fixture.TargetAccount).CurrentBalance);
		Assert.AreEqual(transactions, context.BankAccountTransactions.Count(x => x.BankAccountId == fixture.SourceAccount || x.BankAccountId == fixture.TargetAccount));
	}

	private static (IEmploymentHost Host, ICharacter Manager, SaveManager Saves) NativeEmploymentHost(
		FuturemudDatabaseContext context, NativeEmploymentFixture fixture)
	{
		var currency = Currency(fixture.CurrencyId).Object;
		var manager = Character(fixture.CharacterId, "Native approver").Object;
		var world = Gameworld(currency, new Dictionary<long, ICharacter> { [manager.Id] = manager });
		var saves = new SaveManager();
		world.SetupGet(x => x.SaveManager).Returns(saves);
		Mock.Get(manager).SetupGet(x => x.Gameworld).Returns(world.Object);
		var type = new Mock<IBankAccountType>(); type.SetupGet(x => x.Id).Returns(fixture.AccountTypeId);
		var types = new All<IBankAccountType>(); types.Add(type.Object);
		world.SetupGet(x => x.BankAccountTypes).Returns(types);
		var bank = new Mock<IBank>();
		bank.SetupGet(x => x.Id).Returns(fixture.BankId);
		bank.SetupGet(x => x.Code).Returns("ER");
		bank.SetupGet(x => x.PrimaryCurrency).Returns(currency);
		bank.SetupGet(x => x.Gameworld).Returns(world.Object);
		bank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency> { [currency] = 1000 });
		bank.Setup(x => x.EconomicZone.ZoneForTimePurposes.DateTime(It.IsAny<MudSharp.TimeAndDate.Date.ICalendar>())).Returns(MudDateTime.Never);
		bank.SetupGet(x => x.EconomicZone.FinancialPeriodReferenceCalendar).Returns((MudSharp.TimeAndDate.Date.ICalendar)null!);
		var source = new RuntimeBankAccount(context.BankAccounts.AsNoTracking().Single(x => x.Id == fixture.SourceAccount), bank.Object);
		var target = new RuntimeBankAccount(context.BankAccounts.AsNoTracking().Single(x => x.Id == fixture.TargetAccount), bank.Object);
		var accounts = new All<IBankAccount>(); accounts.Add(source); accounts.Add(target);
		world.SetupGet(x => x.BankAccounts).Returns(accounts);
		bank.SetupGet(x => x.BankAccounts).Returns(accounts);
		IEmploymentHostState? state = null;
		var shop = new Mock<IShop>();
		shop.SetupGet(x => x.Id).Returns(910000000L + fixture.BankId);
		shop.SetupGet(x => x.Name).Returns("Native employment shop");
		shop.SetupGet(x => x.EmploymentHostName).Returns("Native employment shop");
		shop.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		shop.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Shop);
		shop.As<IHaveFuturemud>().SetupGet(x => x.Gameworld).Returns(world.Object);
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.EconomicZone).Returns(bank.Object.EconomicZone);
		shop.SetupGet(x => x.BankAccount).Returns(source);
		shop.SetupGet(x => x.CashBalance).Returns(() => VirtualCashLedger.Balance(shop.Object, currency));
		shop.SetupGet(x => x.Employment).Returns(() => state ??= EmploymentPersistenceStore.LoadOrCreate(shop.Object));
		shop.SetupGet(x => x.TaskBoard).Returns(() => shop.Object.Employment.TaskBoard);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(() => shop.Object.Employment.ManagerGoalBoard);
		shop.SetupGet(x => x.EmploymentRegister).Returns(() => shop.Object.Employment.EmploymentRegister);
		shop.SetupGet(x => x.BusinessLedger).Returns(() => shop.Object.Employment.BusinessLedger);
		shop.SetupGet(x => x.EmploymentContracts).Returns(() => shop.Object.Employment.EmploymentContracts);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
			.Returns((ICharacter actor, EmploymentAuthority authority) => shop.Object.Employment.HasAuthority(actor, authority));
		return (shop.Object, manager, saves);
	}
}
