using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Arenas;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Hotels;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shops;
using MudSharp.Economy.Stables;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class UnifiedEmploymentDispatchTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	[TestMethod]
	public void HostShells_MinimumHostTypes_ImplementEmploymentHost()
	{
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IShop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Shop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(PermanentShop)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IAuctionHouse)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(AuctionHouse)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(ICombatArena)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(CombatArena)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IBank)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Bank)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IStable)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Stable)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(IHotel)));
		Assert.IsTrue(typeof(IEmploymentHost).IsAssignableFrom(typeof(Hotel)));
	}

	[TestMethod]
	public void EmploymentHostContract_DoesNotDependOnExistingPcJobSystem()
	{
		var forbidden = new[] { typeof(IJobListing), typeof(IActiveJob) };
		var exposedTypes = typeof(IEmploymentHost)
		                   .GetProperties()
		                   .Select(x => x.PropertyType)
		                   .Concat(typeof(IEmploymentHost)
		                           .GetMethods()
		                           .SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType)))
		                   .ToList();

		foreach (var type in exposedTypes)
		foreach (var forbiddenType in forbidden)
		{
			Assert.IsFalse(ContainsType(type, forbiddenType), $"{type} unexpectedly exposes {forbiddenType}.");
		}
	}

	[TestMethod]
	public void HireFire_UsesSimpleEmploymentStatusAndRecordsRegisterEntries()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("market shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var employee = Character(2, "Employee").Object;

		Assert.IsFalse(Enum.GetNames<EmploymentStatus>().Contains("Probation"));

		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), manager);

		Assert.AreEqual(EmploymentStatus.Active, contract.Status);
		host.Fire(contract, EmploymentTerminationReason.Fired, manager);
		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.AreEqual(EmploymentTerminationReason.Fired, contract.EndReason);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractHired));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractEnded));
	}

	[TestMethod]
	public void ManagerAuthority_ChecksDelegatedAuthorityForOpeningsAndGoals()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("bank", currency.Object);
		var clerk = Character(1, "Clerk").Object;
		var manager = Character(2, "Manager").Object;

		host.Hire(clerk, Offer(currency.Object, EmploymentRole.Employee), null);
		Assert.ThrowsException<InvalidOperationException>(() =>
			host.Employment.CreateJobOpening(Opening(currency.Object), clerk));

		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.CreateManagerGoals), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
		Assert.AreEqual(JobOpeningStatus.Open, opening.Status);

		var goal = new ManagerGoalDefinition(
			ManagerGoalType.MaintainMinimumStock,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post restock note", new EmploymentActionPlan([
				new BoardPostActionStep("Stock", "Please restock.")
			])),
			1,
			TimeSpan.FromMinutes(10));
		Assert.ThrowsException<InvalidOperationException>(() => host.ManagerGoalBoard.CreateGoal(goal, manager));
	}

	[TestMethod]
	public void JobOpeningCandidateMatching_FiltersRequirementsReservationWageAndPaymentMethods()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var applicant = Character(2, "Applicant").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateJobOpenings), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);

		var underpaid = Profile(applicant, 20.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities), skills: new Dictionary<string, double> { ["haggling"] = 50.0 });
		var rejected = host.Employment.Apply(opening, underpaid);

		Assert.AreEqual(JobApplicationStatus.Rejected, rejected.Status);
		Assert.IsTrue(rejected.DecisionReason?.Contains("reservation wage") == true);

		var qualified = Profile(applicant, 5.0M, PaymentMethodKind.Cash,
			Caps(EmploymentAICapability.CanPurchaseCommodities), skills: new Dictionary<string, double> { ["haggling"] = 50.0 });
		var acceptedForReview = host.Employment.Apply(opening, qualified);

		Assert.AreEqual(JobApplicationStatus.Pending, acceptedForReview.Status);
		var selection = EmploymentPaymentSelector.SelectPaymentMethod(opening, qualified);
		Assert.IsTrue(selection.Success);
		Assert.AreEqual(PaymentMethodKind.Cash, selection.PaymentMethod?.MethodKind);
	}

	[TestMethod]
	public void PaymentSelection_PrefersBankAccountFallsBackToCashAndCanFail()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var opening = new JobOpening(1, host, EmploymentRole.Employee, JobRequirementSet.None, Pay(currency.Object),
			WorkSchedule.AnyTime, EmploymentDuration.Indefinite, JobOpeningStatus.Open, 1, true,
			new PaymentMethod(PaymentMethodKind.EmployeeBankAccount), EmploymentAuthoritySet.Empty);
		var applicant = Character(1, "Applicant").Object;
		var bankAccount = new Mock<IBankAccount>();

		var bankProfile = Profile(applicant, 1.0M, PaymentMethodKind.EmployeeBankAccount, Caps());
		var bank = EmploymentPaymentSelector.SelectPaymentMethod(opening, bankProfile, bankAccount.Object);
		Assert.IsTrue(bank.Success);
		Assert.AreSame(bankAccount.Object, bank.PaymentMethod?.BankAccount);

		var cashProfile = Profile(applicant, 1.0M, PaymentMethodKind.Cash, Caps());
		var cash = EmploymentPaymentSelector.SelectPaymentMethod(opening, cashProfile);
		Assert.IsTrue(cash.Success);
		Assert.AreEqual(PaymentMethodKind.Cash, cash.PaymentMethod?.MethodKind);

		var failureProfile = Profile(applicant, 1.0M, PaymentMethodKind.PaymentItem, Caps());
		var failure = EmploymentPaymentSelector.SelectPaymentMethod(opening, failureProfile);
		Assert.IsFalse(failure.Success);
	}

	[TestMethod]
	public void HostBoard_BoardPostActionStepCreatesPostAndRegisterEntry()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.PostToHostBoard), null);

		var step = new BoardPostActionStep("Deposit complete", "The deposit has been made.");
		var task = host.TaskBoard.CreateActiveTask("post notice", new EmploymentActionPlan([step]), manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanPostToBoard));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		Assert.AreEqual(1, host.Board.Posts.Count());
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
	}

	[TestMethod]
	public void ScheduledRule_ComposesConditionsAndSpawnsActiveTaskOncePerWindow()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules | EmploymentAuthority.AssignTasks | EmploymentAuthority.PostToHostBoard), null);
		var context = new EmploymentTaskContext(host);
		context.SetStockLevel("butter", 2);
		context.SetManualOrder("restock-butter", true);

		var plan = new EmploymentActionPlan([new BoardPostActionStep("Butter", "Butter task spawned.")]);
		host.TaskBoard.CreateScheduledRule("buy butter", "butter-low",
			[
				new StockThresholdCondition("butter", 5, true),
				new ManualOrderCondition("restock-butter")
			],
			plan,
			TimeSpan.FromHours(1),
			manager);

		var first = host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow);
		var second = host.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow.AddMinutes(1));

		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(0, second.Count);
		Assert.AreEqual(EmploymentActionStepStatus.Pending, first.Single().StepStates.Single());
	}

	[TestMethod]
	public void Dispatcher_BlocksImpossibleTasksWithUsefulReason()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("auction house", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ApprovePurchases), null);
		var purchase = new PurchaseActionStep("10kg butter", new MoneyAmount(currency.Object, 10.0M));
		var task = host.TaskBoard.CreateActiveTask("purchase butter", new EmploymentActionPlan([purchase]), manager);
		var context = new EmploymentTaskContext(host);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanPurchaseCommodities));

		var assigned = dispatcher.TryAssignTask(task, [profile], context, out var reason);

		Assert.IsFalse(assigned);
		Assert.AreEqual(EmploymentTaskStatus.Blocked, task.Status);
		Assert.IsTrue(reason.Contains("eligible"));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskBlocked));
	}

	[TestMethod]
	public void FinancialActionSteps_RecordLedgerRegisterAndExistingFinancialRecordReuse()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("bank", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash |
			EmploymentAuthority.UseStoreAccount |
			EmploymentAuthority.ManageCraftRules |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var steps = new IEmploymentActionStep[]
		{
			new PurchaseActionStep("purchase supplies", new MoneyAmount(currency.Object, 5.0M), "shop-transaction-1"),
			new BankDepositActionStep(new MoneyAmount(currency.Object, 10.0M), "bank-deposit-1"),
			new BankWithdrawalActionStep(new MoneyAmount(currency.Object, 3.0M), "bank-withdrawal-1"),
			new StoreAccountPaymentActionStep("linen supplier", new MoneyAmount(currency.Object, 2.0M), "store-account-1"),
			new CraftTriggerActionStep("craft linen bundles", "craft-cost-1")
		};
		var context = new EmploymentTaskContext(host);
		foreach (var step in steps.Where(x => x.RequiresPaymentAuthorisation))
		{
			context.AuthorisePaymentFor(step);
		}

		var task = host.TaskBoard.CreateActiveTask("financial work", new EmploymentActionPlan(steps), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash,
			Caps(
				EmploymentAICapability.CanPurchaseCommodities,
				EmploymentAICapability.CanUseBankAccount,
				EmploymentAICapability.CanHandleCash,
				EmploymentAICapability.CanCraft,
				EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));

		while (task.Status is not EmploymentTaskStatus.Completed and not EmploymentTaskStatus.Failed)
		{
			dispatcher.AdvanceTask(task, context);
		}

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankDeposit));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.BankWithdrawal));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.StoreAccountPayment));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.ExistingFinancialRecordReuse));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.PaymentAuthorisationUsed));
	}

	[TestMethod]
	public void ManagerGoals_CreateTasksOnlyWhenManagerHasRequiredAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		var limited = Character(2, "Limited").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals | EmploymentAuthority.PostToHostBoard), null);
		host.Hire(limited, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateManagerGoals), null);
		var definition = new ManagerGoalDefinition(
			ManagerGoalType.MaintainHotelOperations,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("post room notice", new EmploymentActionPlan([
				new BoardPostActionStep("Rooms", "Room work required.")
			])),
			1,
			TimeSpan.Zero);

		Assert.ThrowsException<InvalidOperationException>(() => host.ManagerGoalBoard.CreateGoal(definition, limited));
		host.ManagerGoalBoard.CreateGoal(definition, manager);
		var tasks = host.ManagerGoalBoard.EvaluateGoals(new EmploymentTaskContext(host), DateTimeOffset.UtcNow);

		Assert.AreEqual(1, tasks.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ManagerGoalEvaluated));
	}

	[TestMethod]
	public void HotelEntity_LinksToPropertyOwnershipLocationAndExistingHotelXmlState()
	{
		var currency = Currency();
		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Currency).Returns(currency.Object);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(123);
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(42);
		property.SetupGet(x => x.Name).Returns("Blue House");
		property.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		property.SetupGet(x => x.PropertyLocations).Returns([cell.Object]);
		property.SetupGet(x => x.HotelRooms).Returns([]);
		property.SetupGet(x => x.IsApprovedHotel).Returns(true);
		property.SetupGet(x => x.HotelCashBalance).Returns(12.5M);
		property.SetupGet(x => x.HotelAvailableFunds).Returns(20.0M);

		var hotel = new Hotel(property.Object);

		Assert.AreSame(property.Object, hotel.Property);
		Assert.AreEqual(EmploymentHostType.Hotel, hotel.EmploymentHostType);
		Assert.IsTrue(hotel.CanAccessHotelLocation(cell.Object));
		Assert.AreEqual(12.5M, hotel.CashBalance);
		Assert.AreEqual(20.0M, hotel.AvailableFunds);
		Assert.IsNotNull(typeof(IProperty).GetProperty(nameof(IProperty.Hotel)));
	}

	[TestMethod]
	public void EmploymentPersistence_FirstAccessCreatesBoardAndEmptyHostState()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var gameworld = Gameworld(currency.Object, new Dictionary<long, ICharacter>());
			IEmploymentHost host = new PersistedEmploymentHost(10, "Copper Kettle", gameworld.Object, currency.Object);

			var employment = host.Employment;

			Assert.AreEqual(1, context.Boards.Count());
			Assert.AreEqual(1, context.EmploymentHostStates.Count());
			var state = context.EmploymentHostStates.Single();
			Assert.AreEqual(host.Id, state.HostId);
			Assert.AreEqual(EmploymentHostType.Shop.ToString(), state.HostType);
			Assert.AreEqual(state.BoardId, employment.Board.Id);
			Assert.IsFalse(context.EmploymentContracts.Any());
			Assert.IsFalse(context.EmploymentJobOpenings.Any());
			Assert.IsFalse(context.EmploymentActiveTasks.Any());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_RoundTripsCoreEmploymentTasksGoalsAndAuditRows()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(100, "Manager").Object;
			var employee = Character(101, "Employee").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager,
				[employee.Id] = employee
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost host = new PersistedEmploymentHost(20, "Blue Bank", gameworld.Object, currency.Object);
			var managerOffer = Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.HireEmployees |
				EmploymentAuthority.CreateJobOpenings |
				EmploymentAuthority.CreateScheduledRules |
				EmploymentAuthority.AssignTasks |
				EmploymentAuthority.CreateManagerGoals |
				EmploymentAuthority.PostToHostBoard |
				EmploymentAuthority.DepositBusinessCash);
			host.Hire(manager, managerOffer, null);
			host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), manager);
			var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
			host.Employment.Apply(opening, Profile(employee, 1.0M, PaymentMethodKind.Cash,
				Caps(EmploymentAICapability.CanPurchaseCommodities),
				new Dictionary<string, double> { ["haggling"] = 40.0 }));

			var scheduledPlan = new EmploymentActionPlan([
				new BoardPostActionStep("Restock", "Restock request spawned.")
			]);
			host.TaskBoard.CreateScheduledRule("restock butter", "restock-butter",
				[
					new StockThresholdCondition("butter", 5, true),
					new ManualOrderCondition("restock-butter")
				],
				scheduledPlan,
				TimeSpan.FromHours(1),
				manager);
			var activeTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("deposit float",
				new EmploymentActionPlan([
					new BankDepositActionStep(new MoneyAmount(currency.Object, 12.0M), "bank-ledger-1")
				]),
				manager);
			activeTask.Assign(manager);
			activeTask.MarkStep(0, EmploymentActionStepStatus.Completed);
			host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
				ManagerGoalType.MaintainMinimumStock,
				EmploymentAuthority.PostToHostBoard,
				new ManagerGoalConfiguration("post stock notice", scheduledPlan,
					[new AccountBalanceCondition("operating", 5.0M, true)]),
				1,
				TimeSpan.FromMinutes(15)), manager);
			host.BusinessLedger.Record(EmploymentLedgerEntryType.Purchase, manager,
				new MoneyAmount(currency.Object, 3.0M), "seed purchase audit");

			var reloadedHost = new PersistedEmploymentHost(20, "Blue Bank", gameworld.Object, currency.Object);
			var reloaded = reloadedHost.Employment;

			Assert.AreEqual(2, reloaded.EmploymentContracts.Count);
			Assert.AreEqual(1, reloaded.JobOpenings.Count);
			Assert.AreEqual(1, reloaded.Applications.Count);
			Assert.AreEqual(1, reloaded.TaskBoard.ScheduledRules.Count);
			Assert.AreEqual(1, reloaded.TaskBoard.ActiveTasks.Count);
			Assert.AreEqual(EmploymentTaskStatus.Completed, reloaded.TaskBoard.ActiveTasks.Single().Status);
			Assert.AreEqual(EmploymentActionStepStatus.Completed, reloaded.TaskBoard.ActiveTasks.Single().StepStates.Single());
			Assert.AreEqual(1, reloaded.ManagerGoalBoard.Goals.Count);
			Assert.AreEqual(1, reloaded.ManagerGoalBoard.Goals.Single().Priority);
			Assert.IsTrue(reloaded.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Purchase));
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractHired));
			Assert.AreEqual(context.EmploymentHostStates.Single().BoardId, reloaded.Board.Id);
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentPersistence_AllowsHostLocalRuntimeIdsAcrossDifferentHosts()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var employee1 = Character(201, "Employee One").Object;
			var employee2 = Character(202, "Employee Two").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[employee1.Id] = employee1,
				[employee2.Id] = employee2
			};
			var gameworld = Gameworld(currency.Object, characters);
			IEmploymentHost first = new PersistedEmploymentHost(301, "First Shop", gameworld.Object, currency.Object);
			IEmploymentHost second = new PersistedEmploymentHost(302, "Second Shop", gameworld.Object, currency.Object);

			first.Hire(employee1, Offer(currency.Object, EmploymentRole.Employee), null);
			second.Hire(employee2, Offer(currency.Object, EmploymentRole.Employee), null);
			first.Employment.CreateJobOpening(Opening(currency.Object), null);
			second.Employment.CreateJobOpening(Opening(currency.Object), null);

			Assert.AreEqual(2, context.EmploymentContracts.Count());
			Assert.AreEqual(2, context.EmploymentJobOpenings.Count());
			CollectionAssert.AreEquivalent(new[] { 1L, 1L },
				context.EmploymentContracts.Select(x => x.RuntimeId).ToArray());
			Assert.AreEqual(2, context.EmploymentContracts.Select(x => x.Id).Distinct().Count());
			Assert.AreEqual(2, context.EmploymentJobOpenings.Select(x => x.Id).Distinct().Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void HotelPersistence_LazilyCreatesRootPreservesXmlShadowAndDelegatesToProperty()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var zone = new Mock<IEconomicZone>();
			zone.SetupGet(x => x.Currency).Returns(currency.Object);
			var cell = new Mock<ICell>();
			cell.SetupGet(x => x.Id).Returns(987);
			var property = new Mock<IProperty>();
			property.SetupGet(x => x.Id).Returns(500);
			property.SetupGet(x => x.Name).Returns("Harbour House");
			property.SetupGet(x => x.EconomicZone).Returns(zone.Object);
			property.SetupGet(x => x.PropertyLocations).Returns([cell.Object]);
			property.SetupGet(x => x.HotelRooms).Returns([]);
			property.SetupGet(x => x.HotelLicenseStatus).Returns(HotelLicenseStatus.Approved);
			property.SetupGet(x => x.HotelLostPropertyRetention).Returns(MudTimeSpan.FromDays(14));
			property.SetupGet(x => x.HotelOutstandingTaxes).Returns(4.5M);
			property.SetupGet(x => x.HotelCashBalance).Returns(22.0M);
			property.SetupGet(x => x.HotelAvailableFunds).Returns(33.0M);

			var hotel = HotelPersistenceStore.LoadOrCreate(property.Object);
			HotelPersistenceStore.ShadowWrite(property.Object, "<Hotel status=\"Approved\" taxes=\"4.5\" />");
			context.SaveChanges();

			var row = context.Hotels.Single();
			Assert.AreEqual(property.Object.Id, row.PropertyId);
			Assert.AreEqual(hotel.Id, row.Id);
			Assert.AreEqual("<Hotel status=\"Approved\" taxes=\"4.5\" />", row.HotelDefinition);
			Assert.AreSame(property.Object, hotel.Property);
			Assert.AreEqual(22.0M, hotel.CashBalance);
			Assert.AreEqual(33.0M, hotel.AvailableFunds);
			Assert.IsTrue(hotel.CanAccessHotelLocation(cell.Object));
			Assert.AreEqual("<Legacy />", HotelPersistenceStore.DefinitionForProperty(600, "<Legacy />"));
			Assert.AreEqual(row.HotelDefinition, HotelPersistenceStore.DefinitionForProperty(property.Object.Id, "<Legacy />"));
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	private static bool ContainsType(Type type, Type target)
	{
		if (type == target)
		{
			return true;
		}

		if (type.IsGenericType && type.GetGenericArguments().Any(x => ContainsType(x, target)))
		{
			return true;
		}

		return type.HasElementType && type.GetElementType() is { } elementType && ContainsType(elementType, target);
	}

	private static Mock<ICurrency> Currency()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1);
		currency.SetupGet(x => x.Name).Returns("test dollars");
		return currency;
	}

	private static Mock<ICharacter> Character(long id, string name)
	{
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		         .Returns(name);
		return character;
	}

	private static CompensationTerms Pay(ICurrency currency, decimal amount = 10.0M)
	{
		return new CompensationTerms(
			new MoneyAmount(currency, amount),
			null,
			PayCadence.Hourly,
			new MoneyAmount(currency, amount),
			PaymentSource.HostCash);
	}

	private static EmploymentOffer Offer(ICurrency currency, EmploymentRole role,
		EmploymentAuthority authority = EmploymentAuthority.None)
	{
		return new EmploymentOffer(
			role,
			Pay(currency),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			authority);
	}

	private static JobOpeningDefinition Opening(ICurrency currency)
	{
		return new JobOpeningDefinition(
			EmploymentRole.Employee,
			new JobRequirementSet(
				[new SkillRequirement("haggling", 25.0)],
				[],
				[new AICapabilityRequirement(EmploymentAICapability.CanPurchaseCommodities)],
				[]),
			Pay(currency),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			1,
			true,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty);
	}

	private static IReadOnlySet<EmploymentAICapability> Caps(params EmploymentAICapability[] capabilities)
	{
		return new HashSet<EmploymentAICapability>(capabilities);
	}

	private static EmploymentCandidateProfile Profile(ICharacter candidate, decimal reservationWage,
		PaymentMethodKind paymentMethod, IReadOnlySet<EmploymentAICapability> capabilities,
		IReadOnlyDictionary<string, double>? skills = null)
	{
		return new EmploymentCandidateProfile(
			candidate,
			reservationWage,
			skills ?? new Dictionary<string, double>(),
			new HashSet<string>(),
			capabilities,
			new HashSet<string>(),
			[paymentMethod]);
	}

	private static FuturemudDatabaseContext BuildContext()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		              .UseInMemoryDatabase(Guid.NewGuid().ToString())
		              .Options;
		return new FuturemudDatabaseContext(options);
	}

	private static FMDBState CaptureFMDBState()
	{
		return new FMDBState(
			(FuturemudDatabaseContext?)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
			                                  .GetValue(null),
			typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
			(uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
			                  .GetValue(null)!);
	}

	private static void PrimeFMDB(FuturemudDatabaseContext context)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
	}

	private static void RestoreFMDBState(FMDBState state)
	{
		typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
		typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
		typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
		            .SetValue(null, state.InstanceCount);
	}

	private static Mock<IFuturemud> Gameworld(ICurrency currency, IReadOnlyDictionary<long, ICharacter> characters)
	{
		var boards = new All<IBoard>();
		var currencies = new All<ICurrency>();
		currencies.Add(currency);
		var bankAccounts = new All<IBankAccount>();
		var cells = new All<ICell>();
		var calendars = new All<ICalendar>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Boards).Returns(boards);
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.SetupGet(x => x.BankAccounts).Returns(bankAccounts);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.Setup(x => x.Add(It.IsAny<IBoard>())).Callback<IBoard>(board => boards.Add(board));
		gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns((long id, bool _) => characters.TryGetValue(id, out var character) ? character : null!);
		return gameworld;
	}

	private sealed class TestEmploymentHost : FrameworkItem, IEmploymentHost
	{
		public TestEmploymentHost(string name, ICurrency currency)
		{
			_id = 1;
			_name = name;
			Currency = currency;
			Employment = new EmploymentHostState(this);
		}

		public override string FrameworkItemType => "TestEmploymentHost";
		public ICurrency Currency { get; }
		public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Other;
		public IMarket? Market => null;
		public IEmploymentHostState Employment { get; }
	}

	private sealed class PersistedEmploymentHost : FrameworkItem, IEmploymentHost, IHaveFuturemud
	{
		private IEmploymentHostState? _employment;

		public PersistedEmploymentHost(long id, string name, IFuturemud gameworld, ICurrency currency)
		{
			_id = id;
			_name = name;
			Gameworld = gameworld;
			Currency = currency;
		}

		public override string FrameworkItemType => "PersistedEmploymentHost";
		public IFuturemud Gameworld { get; }
		public ICurrency Currency { get; }
		public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Shop;
		public IMarket? Market => null;
		public IEmploymentHostState Employment => _employment ??= EmploymentPersistenceStore.LoadOrCreate(this);
	}
}
