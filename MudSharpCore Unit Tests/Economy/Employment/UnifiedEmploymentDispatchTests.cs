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
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class UnifiedEmploymentDispatchTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	private sealed record PermanentShopFixture(Mock<IPermanentShop> Shop, EmploymentHostState State);

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
	public void GetItemsByIdActionStep_CollectsMatchingItemsFromSourceLocations()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var sourceOne = Cell(10, "stock room").Object;
		var sourceTwo = Cell(11, "warehouse").Object;
		var requestedOne = Item(100, "first crate").Object;
		var ignored = Item(101, "wrong crate").Object;
		var requestedTwo = Item(102, "second crate").Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(sourceOne, [requestedOne, ignored]);
		context.SetAvailableItems(sourceTwo, [requestedTwo]);
		var step = new GetItemsByIdActionStep(2, [requestedOne.Id, requestedTwo.Id], [sourceOne, sourceTwo]);
		var task = host.TaskBoard.CreateActiveTask("collect crates", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { requestedOne.Id, requestedTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.DoesNotContain(context.AvailableItems(sourceOne).Select(x => x.Id).ToArray(), requestedOne.Id);
		CollectionAssert.DoesNotContain(context.AvailableItems(sourceTwo).Select(x => x.Id).ToArray(), requestedTwo.Id);
		var completionEntries = host.EmploymentRegister.Entries.Where(x =>
			x.EntryType == EmploymentRegisterEntryType.ActionStepCompleted &&
			x.Description.Contains("Collected", StringComparison.InvariantCultureIgnoreCase)).ToList();
		Assert.AreEqual(1, completionEntries.Count);
		Assert.AreEqual(task.CorrelationId, completionEntries.Single().CorrelationId);
	}

	[TestMethod]
	public void GetItemsByTagActionStep_CollectsTaggedItemsOnly()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("stable", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(20, "feed store").Object;
		var taggedOne = Item(200, "oat sack").Object;
		var taggedTwo = Item(201, "second oat sack").Object;
		var ignored = Item(202, "linen sack").Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [taggedOne, ignored, taggedTwo]);
		context.SetItemTags(taggedOne, "feed");
		context.SetItemTags(taggedTwo, "feed");
		context.SetItemTags(ignored, "linen");
		var step = new GetItemsByTagActionStep(2, "feed", [source]);
		var task = host.TaskBoard.CreateActiveTask("collect feed", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { taggedOne.Id, taggedTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.Contains(context.AvailableItems(source).Select(x => x.Id).ToArray(), ignored.Id);
	}

	[TestMethod]
	public void GetCommodityActionStep_CollectsRequiredMaterialWeight()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("auction house", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(30, "materials store").Object;
		var ironOne = Item(300, "iron bundle").Object;
		var ironTwo = Item(301, "second iron bundle").Object;
		var copper = Item(302, "copper bundle").Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [ironOne, copper, ironTwo]);
		context.SetCommodityWeight(ironOne, "iron", 2.5, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		context.SetCommodityWeight(ironTwo, "iron", 4.0, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		context.SetCommodityWeight(copper, "copper", 8.0, "ingot", new Dictionary<string, string> { ["grade"] = "refined" });
		var step = new GetCommodityActionStep(6.0, "iron", "ingot",
			new Dictionary<string, string> { ["grade"] = "refined" }, [source]);
		var task = host.TaskBoard.CreateActiveTask("collect iron", new EmploymentActionPlan([step]), manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		var result = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(result.Success);
		CollectionAssert.AreEquivalent(new[] { ironOne.Id, ironTwo.Id }, context.CarriedTaskItems(manager).Select(x => x.Id).ToArray());
		CollectionAssert.Contains(context.AvailableItems(source).Select(x => x.Id).ToArray(), copper.Id);
	}

	[TestMethod]
	public void DeliverItemsActionStep_DeliversCarriedTaskItemsToContainerTag()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("hotel", currency.Object);
		var manager = Character(1, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var source = Cell(40, "linen store").Object;
		var destination = Cell(41, "laundry").Object;
		var linenOne = Item(400, "linen bundle").Object;
		var linenTwo = Item(401, "second linen bundle").Object;
		var hamperContents = new List<IGameItem>();
		var hamper = ContainerItem(402, "clean hamper", [destination], [], hamperContents).Object;
		var context = new EmploymentTaskContext(host);
		context.SetAvailableItems(source, [linenOne, linenTwo]);
		context.SetAvailableItems(destination, [hamper]);
		context.SetItemTags(linenOne, "linen");
		context.SetItemTags(linenTwo, "linen");
		context.SetItemTags(hamper, "clean-linen-container");
		var plan = new EmploymentActionPlan([
			new GetItemsByTagActionStep(2, "linen", [source]),
			new DeliverItemsActionStep(destination, containerTag: "clean-linen-container")
		]);
		var task = host.TaskBoard.CreateActiveTask("move linen", plan, manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));

		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out _));
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(0, context.CarriedTaskItems(manager).Count);
		CollectionAssert.AreEquivalent(new[] { linenOne.Id, linenTwo.Id }, context.ContainedItems(hamper).Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void TaskBoard_CreateActiveTaskRequiresActionPlanAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost("shop", currency.Object);
		var manager = Character(1, "Manager").Object;
		var destination = Cell(50, "shopfront").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);

		Assert.ThrowsException<InvalidOperationException>(() =>
			host.TaskBoard.CreateActiveTask("move stock",
				new EmploymentActionPlan([new DeliverItemsActionStep(destination)]),
				manager));
	}

	[TestMethod]
	public void ShopEmploymentTaskService_CreatesAndExecutesStockroomRestockMovement()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroomItems = new List<IGameItem>();
		var shopfrontItems = new List<IGameItem>();
		var stockroom = PhysicalCell(60, "stockroom", stockroomItems).Object;
		var shopfront = PhysicalCell(61, "shopfront", shopfrontItems).Object;
		var movedOne = Item(600, "linen bundle", trueLocations: [stockroom]).Object;
		var movedTwo = Item(601, "second linen bundle", trueLocations: [stockroom]).Object;
		stockroomItems.AddRange([movedOne, movedTwo]);
		var containerContents = new List<IGameItem>();
		var displayTag = Tag(700, "display-basket").Object;
		var container = ContainerItem(602, "display basket", [shopfront], [displayTag], containerContents).Object;
		shopfrontItems.Add(container);
		var merchandise = Merchandise(800, "linen");
		var shop = PermanentShop(900, "linen shop", currency.Object, manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes,
			stockroom, [shopfront], [container], [merchandise.Object], [movedOne, movedTwo]);
		var service = new ShopEmploymentTaskService();

		var created = service.TryCreateStockroomRestockTask(manager, shop.Shop.Object, merchandise.Object, 2,
			out var task, out var message, containerTag: "display-basket");

		Assert.IsTrue(created, message);
		Assert.IsNotNull(task);
		Assert.AreEqual(2, task.ActionPlan.Steps.Count);
		Assert.AreEqual(EmploymentActionStepType.GetItemsById, task.ActionPlan.Steps[0].StepType);
		Assert.AreEqual(EmploymentActionStepType.DeliverItems, task.ActionPlan.Steps[1].StepType);
		Assert.IsFalse(task.ActionPlan.Steps.Any(x => x.StepType == EmploymentActionStepType.BoardPost));

		var dispatcher = new EmploymentTaskDispatcher();
		var context = service.CreatePhysicalContext(shop.Shop.Object);
		var profile = Profile(manager, 1.0M, PaymentMethodKind.Cash, Caps(EmploymentAICapability.CanDeliverItems));
		Assert.IsTrue(dispatcher.TryAssignTask(task, [profile], context, out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);

		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(0, stockroomItems.Count);
		CollectionAssert.AreEquivalent(new[] { movedOne.Id, movedTwo.Id }, containerContents.Select(x => x.Id).ToArray());
		Assert.AreEqual(0, shop.Shop.Object.Board.Posts.Count());
		Assert.IsTrue(shop.State.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated));
		Assert.IsTrue(shop.State.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCompleted));
	}

	[TestMethod]
	public void ShopEmploymentTaskService_RequiresDelegatedDeliveryAuthority()
	{
		var currency = Currency();
		var manager = Character(1, "Manager").Object;
		var stockroomItems = new List<IGameItem>();
		var shopfrontItems = new List<IGameItem>();
		var stockroom = PhysicalCell(70, "stockroom", stockroomItems).Object;
		var shopfront = PhysicalCell(71, "shopfront", shopfrontItems).Object;
		var item = Item(700, "linen bundle", trueLocations: [stockroom]).Object;
		stockroomItems.Add(item);
		var merchandise = Merchandise(801, "linen");
		var shop = PermanentShop(901, "linen shop", currency.Object, manager, EmploymentAuthority.AssignTasks,
			stockroom, [shopfront], [], [merchandise.Object], [item]);
		var service = new ShopEmploymentTaskService();

		var created = service.TryCreateStockroomRestockTask(manager, shop.Shop.Object, merchandise.Object, 1,
			out var task, out var message);

		Assert.IsFalse(created);
		Assert.IsNull(task);
		StringAssert.Contains(message, "delegated authority");
		Assert.AreEqual(0, shop.Shop.Object.TaskBoard.ActiveTasks.Count);
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
	public void EmploymentPersistence_RoundTripsInventoryActionSteps()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(250, "Manager").Object;
			var source = Cell(800, "stock room").Object;
			var destination = Cell(801, "sales floor").Object;
			var container = Item(802, "display basket").Object;
			var characters = new Dictionary<long, ICharacter>
			{
				[manager.Id] = manager
			};
			var gameworld = Gameworld(currency.Object, characters, [source, destination],
				new Dictionary<long, IGameItem> { [container.Id] = container });
			IEmploymentHost host = new PersistedEmploymentHost(251, "Stock Shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
			host.TaskBoard.CreateActiveTask("inventory movement",
				new EmploymentActionPlan([
					new GetItemsByIdActionStep(1, [9001], [source]),
					new GetItemsByTagActionStep(2, "linen", [source]),
					new GetCommodityActionStep(5.0, "iron", "ingot",
						new Dictionary<string, string> { ["grade"] = "refined" }, [source]),
					new DeliverItemsActionStep(destination, container, "display-container")
				]),
				manager);

			var reloadedHost = new PersistedEmploymentHost(251, "Stock Shop", gameworld.Object, currency.Object);
			var steps = reloadedHost.Employment.TaskBoard.ActiveTasks.Single().ActionPlan.Steps;

			Assert.AreEqual(4, steps.Count);
			Assert.AreEqual(EmploymentActionStepType.GetItemsById, steps[0].StepType);
			Assert.AreEqual(EmploymentActionStepType.GetItemsByTag, steps[1].StepType);
			Assert.AreEqual(EmploymentActionStepType.GetCommodity, steps[2].StepType);
			Assert.AreEqual(EmploymentActionStepType.DeliverItems, steps[3].StepType);
			Assert.AreEqual(9001, ((GetItemsByIdActionStep)steps[0]).ItemIds.Single());
			Assert.AreEqual("linen", ((GetItemsByTagActionStep)steps[1]).TagName);
			Assert.AreEqual("iron", ((GetCommodityActionStep)steps[2]).MaterialName);
			Assert.AreEqual("refined", ((GetCommodityActionStep)steps[2]).Characteristics["grade"]);
			Assert.AreSame(destination, ((DeliverItemsActionStep)steps[3]).Destination);
			Assert.AreSame(container, ((DeliverItemsActionStep)steps[3]).Container);
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

	private static Mock<ICell> Cell(long id, string name)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		return cell;
	}

	private static Mock<ICell> PhysicalCell(long id, string name, List<IGameItem> items)
	{
		var cell = Cell(id, name);
		cell.SetupGet(x => x.GameItems).Returns(() => items);
		cell.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
		    .Callback<IGameItem, bool>((item, _) => items.Add(item));
		cell.Setup(x => x.Extract(It.IsAny<IGameItem>()))
		    .Callback<IGameItem>(item => items.RemoveAll(x => x.Id == item.Id));
		cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns(name);
		return cell;
	}

	private static Mock<IGameItem> Item(long id, string name, IEnumerable<ICell>? trueLocations = null,
		IEnumerable<ITag>? tags = null)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Tags).Returns(tags ?? []);
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		item.SetupGet(x => x.TrueLocations).Returns(trueLocations ?? []);
		item.Setup(x => x.GetItemType<IContainer>()).Returns((IContainer)null!);
		item.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		    .Returns(name);
		return item;
	}

	private static Mock<IGameItem> ContainerItem(long id, string name, IEnumerable<ICell> trueLocations,
		IEnumerable<ITag> tags, List<IGameItem> contents)
	{
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Contents).Returns(() => contents);
		container.Setup(x => x.CanPut(It.IsAny<IGameItem>())).Returns(true);
		container.Setup(x => x.Put(It.IsAny<ICharacter?>(), It.IsAny<IGameItem>(), It.IsAny<bool>()))
		         .Callback<ICharacter?, IGameItem, bool>((_, item, _) => contents.Add(item));
		var item = Item(id, name, trueLocations, tags);
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		item.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);
		return item;
	}

	private static Mock<ITag> Tag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		tag.SetupGet(x => x.FullName).Returns(name);
		return tag;
	}

	private static Mock<IMerchandise> Merchandise(long id, string name)
	{
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(id);
		merchandise.SetupGet(x => x.Name).Returns(name);
		return merchandise;
	}

	private static PermanentShopFixture PermanentShop(long id, string name, ICurrency currency, ICharacter manager,
		EmploymentAuthority managerAuthority, ICell stockroom, IEnumerable<ICell> shopfronts,
		IEnumerable<IGameItem> displayContainers, IEnumerable<IMerchandise> merchandises,
		IEnumerable<IGameItem> stockedItems)
	{
		var shop = new Mock<IPermanentShop>();
		shop.SetupGet(x => x.Id).Returns(id);
		shop.SetupGet(x => x.Name).Returns(name);
		shop.SetupGet(x => x.EmploymentHostName).Returns(name);
		shop.SetupGet(x => x.FrameworkItemType).Returns("PermanentShop");
		shop.SetupGet(x => x.EmploymentHostType).Returns(MudSharp.Economy.Employment.EmploymentHostType.Shop);
		shop.SetupGet(x => x.Market).Returns((IMarket?)null);
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.StockroomCell).Returns(stockroom);
		shop.SetupGet(x => x.ShopfrontCells).Returns(shopfronts.ToList());
		shop.SetupGet(x => x.DisplayContainers).Returns(displayContainers.ToList());
		shop.SetupGet(x => x.Merchandises).Returns(merchandises.ToList());
		var employment = new EmploymentHostState(shop.Object);
		shop.SetupGet(x => x.Employment).Returns(employment);
		shop.SetupGet(x => x.TaskBoard).Returns(employment.TaskBoard);
		shop.SetupGet(x => x.EmploymentRegister).Returns(employment.EmploymentRegister);
		shop.SetupGet(x => x.BusinessLedger).Returns(employment.BusinessLedger);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(employment.ManagerGoalBoard);
		shop.SetupGet(x => x.Board).Returns(employment.Board);
		shop.SetupGet(x => x.EmploymentContracts).Returns(employment.EmploymentContracts);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => employment.HasAuthority(actor, authority));
		shop.Setup(x => x.StockedItems(It.IsAny<IMerchandise>()))
		    .Returns((IMerchandise merchandise) => stockedItems.Where(x => merchandises.Any(y => y.Id == merchandise.Id)).ToList());
		employment.Hire(manager, Offer(currency, EmploymentRole.Manager, managerAuthority), null);
		return new PermanentShopFixture(shop, employment);
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

	private static Mock<IFuturemud> Gameworld(ICurrency currency, IReadOnlyDictionary<long, ICharacter> characters,
		IEnumerable<ICell>? cellsToAdd = null, IReadOnlyDictionary<long, IGameItem>? items = null)
	{
		var boards = new All<IBoard>();
		var currencies = new All<ICurrency>();
		currencies.Add(currency);
		var bankAccounts = new All<IBankAccount>();
		var cells = new All<ICell>();
		foreach (var cell in cellsToAdd ?? [])
		{
			cells.Add(cell);
		}

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
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns((long id, bool _) => items is not null && items.TryGetValue(id, out var item) ? item : null!);
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
