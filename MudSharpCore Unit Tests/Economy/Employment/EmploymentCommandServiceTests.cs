using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Commands.Helpers;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Banking;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;
using MudSharp.Economy.Shops;
using MudSharp.Economy.Stables;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate.Date;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class EmploymentCommandServiceTests
{
	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	[TestMethod]
	public void EmploymentHostResolver_ResolvesHostsByTypeIdAndName()
	{
		var gameworld = Gameworld();
		var shop = HostMock<IShop>(1, "north market");
		var auction = HostMock<IAuctionHouse>(2, "estate hall");
		var arena = HostMock<ICombatArena>(3, "red sands");
		var bank = HostMock<IBank>(4, "coin vault");
		var stable = HostMock<IStable>(5, "east stable");
		var hotel = HostMock<IHotel>(6, "harbour rooms");
		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(7);
		property.SetupGet(x => x.Name).Returns("harbour house");
		property.SetupGet(x => x.Hotel).Returns(hotel.Object);

		var shops = new All<IShop>();
		shops.Add(shop.Object);
		var auctions = new All<IAuctionHouse>();
		auctions.Add(auction.Object);
		var arenas = new All<ICombatArena>();
		arenas.Add(arena.Object);
		var banks = new All<IBank>();
		banks.Add(bank.Object);
		var stables = new All<IStable>();
		stables.Add(stable.Object);
		var properties = new All<IProperty>();
		properties.Add(property.Object);
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		gameworld.SetupGet(x => x.AuctionHouses).Returns(auctions);
		gameworld.SetupGet(x => x.CombatArenas).Returns(arenas);
		gameworld.SetupGet(x => x.Banks).Returns(banks);
		gameworld.SetupGet(x => x.Stables).Returns(stables);
		gameworld.SetupGet(x => x.Properties).Returns(properties);

		var resolver = new EmploymentHostResolver();

		Assert.AreSame(shop.Object, resolver.Resolve(gameworld.Object, "shop", "1", out _));
		Assert.AreSame(auction.Object, resolver.Resolve(gameworld.Object, "auction", "estate hall", out _));
		Assert.AreSame(arena.Object, resolver.Resolve(gameworld.Object, "arena", "red sands", out _));
		Assert.AreSame(bank.Object, resolver.Resolve(gameworld.Object, "bank", "coin vault", out _));
		Assert.AreSame(stable.Object, resolver.Resolve(gameworld.Object, "stable", "east stable", out _));
		Assert.AreSame(hotel.Object, resolver.Resolve(gameworld.Object, "hotel", "harbour house", out _));
	}

	[TestMethod]
	public void EmploymentCommandService_OutsiderCanOnlyViewOpenings()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var outsider = Character(10, "Outsider").Object;
		host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var service = new EmploymentCommandService();

		Assert.IsFalse(service.CanViewOperational(outsider, host));
		Assert.IsTrue(service.CanViewOpenings(outsider, host));
		StringAssert.Contains(service.RenderOpenings(outsider, host), "Employee");
	}

	[TestMethod]
	public void EmploymentCommandService_ActiveEmployeeCanViewOperationalState()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(11, "Employee").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
		var service = new EmploymentCommandService();

		Assert.IsTrue(service.CanViewOperational(employee, host));
		StringAssert.Contains(service.RenderStatus(employee, host), "Contracts");
		StringAssert.Contains(service.RenderContracts(employee, host), "Employee");
	}

	[TestMethod]
	public void EmploymentCommandService_BoardPostDeniedWithoutAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(12, "Employee").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Manager), null);
		var service = new EmploymentCommandService();

		var result = service.TryPostBoardPost(employee, host, "Stock", "Please restock.", out var message);

		Assert.IsFalse(result);
		StringAssert.Contains(message, "not authorised");
		Assert.AreEqual(0, host.Board.Posts.Count());
		Assert.IsFalse(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
	}

	[TestMethod]
	public void EmploymentCommandService_BoardPostAllowedWithAuthorityRecordsRegister()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(13, "Employee").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		var result = service.TryPostBoardPost(employee, host, "Stock", "Please restock.", out var message);

		Assert.IsTrue(result);
		StringAssert.Contains(message, "Stock");
		Assert.AreEqual(1, host.Board.Posts.Count());
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
	}

	[TestMethod]
	public void EmploymentCommandService_AdminCanPostWithoutContract()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var admin = Character(14, "Admin", administrator: true).Object;
		var service = new EmploymentCommandService();

		var result = service.TryPostBoardPost(admin, host, "Stock", "Please restock.", out var message);

		Assert.IsTrue(result, message);
		Assert.IsTrue(host.HasAuthority(admin, EmploymentAuthority.PostToHostBoard));
		Assert.AreEqual(1, host.Board.Posts.Count());
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
	}

	[TestMethod]
	public void EmploymentCommandService_DoesNotExposeLegacyJobSystem()
	{
		var forbidden = new[] { typeof(IJobListing), typeof(IActiveJob) };
		var exposedTypes = typeof(EmploymentCommandService)
		                   .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		                   .Where(x => !x.IsSpecialName)
		                   .SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType))
		                   .Concat(typeof(EmploymentHostResolver)
		                           .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		                           .Where(x => !x.IsSpecialName)
		                           .SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType)))
		                   .ToList();

		foreach (var type in exposedTypes)
		foreach (var forbiddenType in forbidden)
		{
			Assert.IsFalse(ContainsType(type, forbiddenType), $"{type} unexpectedly exposes {forbiddenType}.");
		}
	}

	[TestMethod]
	public void EmploymentCommandService_PersistedHostBoardPostUsesRealPersistedBoard()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var employee = Character(20, "Employee").Object;
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			         .Returns((long id, bool _) => id == employee.Id ? employee : null!);
			IEmploymentHost host = new PersistedEmploymentHost(30, "persistent shop", gameworld.Object, currency.Object);
			host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.PostToHostBoard), null);
			var service = new EmploymentCommandService();

			var result = service.TryPostBoardPost(employee, host, "Notice", "Persistent post.", out _);

			Assert.IsTrue(result);
			Assert.AreEqual("Board", host.Board.FrameworkItemType);
			Assert.AreEqual(context.EmploymentHostStates.Single().BoardId, host.Board.Id);
			Assert.AreEqual(1, host.Board.Posts.Count());
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentCommandService_CreateOpeningRequiresAuthorityAndRecordsRegister()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(30, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateJobOpenings), null);
		var service = new EmploymentCommandService();

		var result = service.TryCreateOpening(manager, host, EmploymentRole.Courier, 12.5M, 2, out var opening,
			out var message);

		Assert.IsTrue(result, message);
		Assert.IsNotNull(opening);
		Assert.AreEqual(EmploymentRole.Courier, opening.Role);
		Assert.AreEqual(2, opening.MaxPositions);
		Assert.AreEqual(1, host.JobOpenings.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.JobOpeningCreated));
	}

	[TestMethod]
	public void EmploymentCommandService_CreateOpeningCommandUsesCurrencyParser()
	{
		var currency = Currency();
		decimal parsedAmount = 12.5M;
		currency.Setup(x => x.TryGetBaseCurrency("12.50", out parsedAmount)).Returns(true);
		currency.Setup(x => x.Describe(12.5M, CurrencyDescriptionPatternType.ShortDecimal)).Returns("twelve coins");
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(32, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateJobOpenings), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("openings create courier 12.50 2"));

		var opening = host.JobOpenings.Single();
		Assert.AreEqual(EmploymentRole.Courier, opening.Role);
		Assert.AreEqual(12.5M, opening.Compensation.FixedRate!.Amount);
		Assert.AreEqual(2, opening.MaxPositions);
		Mock.Get(manager.OutputHandler).Verify(x => x.Send(
			It.Is<string>(text => text.Contains("twelve coins")),
			It.IsAny<bool>(),
			It.IsAny<bool>()), Times.Once);
	}

	[TestMethod]
	public void EmploymentCommandService_RecognisesSubsystemEmploymentShortcuts()
	{
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("contracts"));
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("board"));
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("openings"));
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("employmentledger"));
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("empledger"));
		Assert.IsFalse(EmploymentCommandService.IsEmploymentShortcut("ledger"));
		Assert.IsFalse(EmploymentCommandService.IsEmploymentShortcut("sell"));
		Assert.IsFalse(EmploymentCommandService.IsEmploymentShortcut("lodge"));
	}

	[TestMethod]
	public void EmploymentCommandService_CreateOpeningDeniedWithoutAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(31, "Employee").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Manager), null);
		var service = new EmploymentCommandService();

		var result = service.TryCreateOpening(employee, host, EmploymentRole.Courier, 12.5M, 1, out var opening,
			out var message);

		Assert.IsFalse(result);
		Assert.IsNull(opening);
		StringAssert.Contains(message, "delegated");
		Assert.AreEqual(0, host.JobOpenings.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_AcceptApplicationCreatesContractAndRegisterRows()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(40, "Manager").Object;
		var applicant = Character(41, "Applicant").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.HireEmployees), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var application = host.Employment.Apply(opening, Profile(applicant, PaymentMethodKind.Cash));
		var service = new EmploymentCommandService();

		var result = service.TryAcceptApplication(manager, host, 1, out var contract, out var message);

		Assert.IsTrue(result, message);
		Assert.IsNotNull(contract);
		Assert.AreEqual(applicant.Id, contract.Employee.Id);
		Assert.AreEqual(JobApplicationStatus.Accepted, application.Status);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ApplicationAccepted));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractHired));
	}

	[TestMethod]
	public void EmploymentCommandService_RejectApplicationRequiresHireAuthorityButAdminsPass()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(42, "Employee").Object;
		var admin = Character(43, "Admin", administrator: true).Object;
		var applicant = Character(44, "Applicant").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Manager), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var application = host.Employment.Apply(opening, Profile(applicant, PaymentMethodKind.Cash));
		var service = new EmploymentCommandService();

		var denied = service.TryRejectApplication(employee, host, 1, "Not now.", out var deniedMessage);
		var allowed = service.TryRejectApplication(admin, host, 1, "Not now.", out var allowedMessage);

		Assert.IsFalse(denied);
		StringAssert.Contains(deniedMessage, "delegated");
		Assert.IsTrue(allowed, allowedMessage);
		Assert.AreEqual(JobApplicationStatus.Rejected, application.Status);
		Assert.AreEqual("Not now.", application.DecisionReason);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ApplicationRejected));
	}

	[TestMethod]
	public void EmploymentCommandService_AcceptApplicationRespectsOpeningCapacity()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(45, "Manager").Object;
		var first = Character(46, "First").Object;
		var second = Character(47, "Second").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.HireEmployees), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		host.Employment.Apply(opening, Profile(first, PaymentMethodKind.Cash));
		host.Employment.Apply(opening, Profile(second, PaymentMethodKind.Cash));
		var service = new EmploymentCommandService();

		Assert.IsTrue(service.TryAcceptApplication(manager, host, 1, out _, out var firstMessage), firstMessage);
		var secondAccepted = service.TryAcceptApplication(manager, host, 2, out _, out var secondMessage);

		Assert.IsFalse(secondAccepted);
		StringAssert.Contains(secondMessage, "capacity");
		Assert.AreEqual(1, host.Employment.Applications.Count(x => x.Status == JobApplicationStatus.Accepted));
	}

	[TestMethod]
	public void EmploymentCommandService_AcceptedApplicationDecisionSurvivesReload()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(50, "Manager").Object;
			var applicant = Character(51, "Applicant").Object;
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			var currencies = new All<ICurrency>();
			currencies.Add(currency.Object);
			gameworld.SetupGet(x => x.Currencies).Returns(currencies);
			gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			         .Returns((long id, bool _) => id == manager.Id ? manager : id == applicant.Id ? applicant : null!);
			IEmploymentHost host = new PersistedEmploymentHost(52, "persistent shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.HireEmployees | EmploymentAuthority.CreateJobOpenings), null);
			var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
			host.Employment.Apply(opening, Profile(applicant, PaymentMethodKind.Cash));
			var service = new EmploymentCommandService();
			Assert.IsTrue(service.TryAcceptApplication(manager, host, 1, out _, out var acceptMessage), acceptMessage);

			IEmploymentHost reloaded = new PersistedEmploymentHost(52, "persistent shop", gameworld.Object, currency.Object);
			var application = reloaded.Employment.Applications.Single();

			Assert.AreEqual(JobApplicationStatus.Accepted, application.Status);
			Assert.AreEqual(2, reloaded.EmploymentContracts.Count);
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ApplicationAccepted));
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftCommandsManageTransientDraft()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(60, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Restock shelves", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("gettag 2 apple from here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("deliver to here containertag display"), out message), message);
		StringAssert.Contains(authoring.RenderDraft(manager, host), "Restock shelves");
		StringAssert.Contains(authoring.RenderDraft(manager, host), "CanDeliverItems");
		Assert.IsFalse(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated));

		Assert.IsTrue(authoring.TryRemoveStep(manager, host, 1, out message), message);
		StringAssert.Contains(authoring.RenderDraft(manager, host), "deliver all carried task items");
		StringAssert.DoesNotMatch(authoring.RenderDraft(manager, host), new System.Text.RegularExpressions.Regex("tagged apple"));
		Assert.IsTrue(authoring.TryRenameDraft(manager, host, "Deliver shelves", out message), message);
		StringAssert.Contains(authoring.RenderDraft(manager, host), "Deliver shelves");
		Assert.IsTrue(authoring.TryDiscardDraft(manager, host, out message), message);
		StringAssert.Contains(authoring.RenderDraft(manager, host), "do not have");
		Assert.AreEqual(0, host.TaskBoard.ActiveTasks.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_TaskActionsShowsActionSyntax()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(66, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);
		var service = new EmploymentCommandService();
		var messages = new List<string>();
		Mock.Get(manager.OutputHandler)
		    .Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
		    .Callback<string, bool, bool>((text, _, _) => messages.Add(text))
		    .Returns(true);

		service.ExecuteForHost(manager, host, new StringStack("tasks actions"));
		var output = messages.Single();

		StringAssert.Contains(output, "Employment Task Actions");
		StringAssert.Contains(output, "tasks step getid");
		StringAssert.Contains(output, "tasks step deliver");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftFinalisePersistsOrderedActionSteps()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var cell = Cell(100, "stockroom");
			var manager = Character(61, "Manager", gameworld: null, location: cell.Object).Object;
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			var cells = new All<ICell>();
			cells.Add(cell.Object);
			gameworld.SetupGet(x => x.Cells).Returns(cells);
			var currencies = new All<ICurrency>();
			currencies.Add(currency.Object);
			gameworld.SetupGet(x => x.Currencies).Returns(currencies);
			gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			         .Returns((long id, bool _) => id == manager.Id ? manager : null!);
			Mock.Get(manager).SetupGet(x => x.Gameworld).Returns(gameworld.Object);
			IEmploymentHost host = new PersistedEmploymentHost(62, "persistent shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
			var service = new EmploymentCommandService();

			service.ExecuteForHost(manager, host, new StringStack("tasks draft new Restock apples"));
			service.ExecuteForHost(manager, host, new StringStack("tasks step gettag 2 apple from here"));
			service.ExecuteForHost(manager, host, new StringStack("tasks step deliver to here containertag display"));
			service.ExecuteForHost(manager, host, new StringStack("tasks draft finalise"));

			IEmploymentHost reloaded = new PersistedEmploymentHost(62, "persistent shop", gameworld.Object, currency.Object);
			var task = reloaded.TaskBoard.ActiveTasks.Single();

			Assert.AreEqual("Restock apples", task.Name);
			Assert.AreEqual(2, task.ActionPlan.Steps.Count);
			Assert.IsInstanceOfType(task.ActionPlan.Steps[0], typeof(GetItemsByTagActionStep));
			Assert.IsInstanceOfType(task.ActionPlan.Steps[1], typeof(DeliverItemsActionStep));
			Assert.AreEqual("display", ((DeliverItemsActionStep)task.ActionPlan.Steps[1]).ContainerTag);
			Assert.AreEqual(2, task.StepStates.Count);
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x =>
				x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated));
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftFinaliseRechecksAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		IEmploymentHost otherHost = new TestEmploymentHost(2, "other shop", currency.Object);
		var manager = Character(63, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);
		otherHost.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Restock shelves", out var message), message);
		Assert.IsFalse(authoring.TryFinaliseDraft(manager, host, out _, out message));
		StringAssert.Contains(message, "no steps");
		Assert.IsFalse(authoring.TryFinaliseDraft(manager, otherHost, out _, out message));
		StringAssert.Contains(message, "do not have");
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("deliver to here"), out message), message);
		Assert.IsFalse(authoring.TryFinaliseDraft(manager, host, out _, out message));
		StringAssert.Contains(message, "not authorised");
		Assert.AreEqual(0, host.TaskBoard.ActiveTasks.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftRequiresAssignTasks()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(64, "Employee").Object;
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
		var authoring = new EmploymentTaskAuthoringService();

		var started = authoring.TryStartDraft(employee, host, "Restock shelves", out var message);

		Assert.IsFalse(started);
		StringAssert.Contains(message, "AssignTasks");
	}

	[TestMethod]
	public void EmploymentCommandService_TasksListStillShowsExistingActiveTasks()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(65, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		host.TaskBoard.CreateActiveTask(
			"Deliver apples",
			new EmploymentActionPlan(new[] { new DeliverItemsActionStep(manager.Location) }),
			manager);
		var service = new EmploymentCommandService();

		var rendered = service.RenderTasks(manager, host);

		StringAssert.Contains(rendered, "Deliver apples");
		StringAssert.Contains(rendered, "Active Tasks");
	}

	private static Mock<T> HostMock<T>(long id, string name)
		where T : class, IEmploymentHost
	{
		var host = new Mock<T>();
		host.SetupGet(x => x.Id).Returns(id);
		host.SetupGet(x => x.Name).Returns(name);
		return host;
	}

	private static Mock<IFuturemud> Gameworld()
	{
		return new Mock<IFuturemud>();
	}

	private static void SetupBoardPersistence(Mock<IFuturemud> gameworld)
	{
		var boards = new All<IBoard>();
		var calendars = new All<ICalendar>();
		var saveManager = new Mock<ISaveManager>();
		gameworld.SetupGet(x => x.Boards).Returns(boards);
		gameworld.SetupGet(x => x.Calendars).Returns(calendars);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.Setup(x => x.Add(It.IsAny<IBoard>())).Callback<IBoard>(board => boards.Add(board));
	}

	private static Mock<ICurrency> Currency()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1);
		currency.SetupGet(x => x.Name).Returns("test dollars");
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns((decimal amount, CurrencyDescriptionPatternType _) => $"{amount:N2} test dollars");
		return currency;
	}

	private static Mock<ICell> Cell(long id, string name)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.Location).Returns(cell.Object);
		cell.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns(name);
		return cell;
	}

	private static Mock<ICharacter> Character(long id, string name, bool administrator = false,
		IFuturemud? gameworld = null, ICell? location = null)
	{
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		var body = new Mock<IBody>();
		body.Setup(x => x.LookText(It.IsAny<IPerceivable>(), It.IsAny<bool>())).Returns(name);
		var currentLocation = location ?? Cell(id * 10, $"{name} location").Object;
		var character = new Mock<ICharacter>();
		var output = new Mock<IOutputHandler>();
		var effects = new List<IEffect>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.Gameworld).Returns(gameworld!);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(currentLocation);
		character.SetupGet(x => x.OutputHandler).Returns(output.Object);
		character.SetupGet(x => x.Effects).Returns(effects);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
		         .Callback<IEffect>(effect => effects.Add(effect));
		character.Setup(x => x.EffectsOfType<EmploymentTaskDraftEffect>(
					It.IsAny<Predicate<EmploymentTaskDraftEffect>?>()))
		         .Returns((Predicate<EmploymentTaskDraftEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentTaskDraftEffect>()
				         : effects.OfType<EmploymentTaskDraftEffect>().Where(x => predicate(x)));
		character.Setup(x => x.RemoveAllEffects<EmploymentTaskDraftEffect>(
					It.IsAny<Predicate<EmploymentTaskDraftEffect>?>(),
					It.IsAny<bool>()))
		         .Returns((Predicate<EmploymentTaskDraftEffect>? predicate, bool _) =>
		         {
			         var removed = predicate is null
				         ? effects.OfType<EmploymentTaskDraftEffect>().ToList()
				         : effects.OfType<EmploymentTaskDraftEffect>().Where(x => predicate(x)).ToList();
			         foreach (var effect in removed)
			         {
				         effects.Remove(effect);
			         }

			         return removed.Any();
		         });
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(administrator);
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
			JobRequirementSet.None,
			Pay(currency),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			1,
			true,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty);
	}

	private static EmploymentCandidateProfile Profile(ICharacter candidate, PaymentMethodKind paymentMethod)
	{
		return new EmploymentCandidateProfile(
			candidate,
			0.0M,
			new Dictionary<string, double>(),
			new HashSet<string>(),
			new HashSet<EmploymentAICapability>(),
			new HashSet<string>(),
			[paymentMethod]);
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

	private sealed class TestEmploymentHost : FrameworkItem, IEmploymentHost
	{
		public TestEmploymentHost(long id, string name, ICurrency currency)
		{
			_id = id;
			_name = name;
			Currency = currency;
			Employment = new EmploymentHostState(this);
		}

		public override string FrameworkItemType => "TestEmploymentHost";
		public ICurrency Currency { get; }
		public EmploymentHostType EmploymentHostType => MudSharp.Economy.Employment.EmploymentHostType.Shop;
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
