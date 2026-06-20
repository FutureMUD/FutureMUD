using System;
using System.Collections.Generic;
using System.Globalization;
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
using MudSharp.Climate;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
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
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.TimeAndDate;
using MudSharp.TimeAndDate.Date;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class EmploymentCommandServiceTests
{
	private delegate bool TryGetBaseCurrencyCallback(string pattern, out decimal amount);

	private sealed record FMDBState(FuturemudDatabaseContext? Context, object? Connection, uint InstanceCount);

	[TestMethod]
	public void EmploymentHelpSurfacesUseGroupedCatalogueDiscovery()
	{
		var compactPlayerHelpTexts = new[]
		{
			("stable player", HelpConstant(typeof(EconomyModule), "StablePlayerHelp")),
			("shop player", HelpConstant(typeof(EconomyModule), "ShopPlayerHelp")),
			("bank player", HelpConstant(typeof(EconomyModule), "BankPlayerHelpText")),
			("auction player", HelpConstant(typeof(EconomyModule), "AuctionPlayerHelp")),
			("arena player", HelpConstant(typeof(ArenaModule), "ArenaHelp")),
			("roomrent player", HelpConstant(typeof(PropertyModule), "RoomRentHelp"))
		};

		foreach (var (name, help) in compactPlayerHelpTexts)
		{
			Assert.IsTrue(help.Contains("tasks actions", StringComparison.OrdinalIgnoreCase),
				$"{name} help should advertise task action catalogue discovery.");
			Assert.IsTrue(help.Contains("tasks conditions", StringComparison.OrdinalIgnoreCase),
				$"{name} help should advertise scheduled-rule condition catalogue discovery.");
			Assert.IsFalse(help.Contains("contracts delegate", StringComparison.OrdinalIgnoreCase),
				$"{name} help should not dump manager-only employment commands.");
			Assert.IsFalse(help.Contains("tasks rule create", StringComparison.OrdinalIgnoreCase),
				$"{name} help should point to manager help instead of dumping scheduled-rule authoring.");
			Assert.IsFalse(help.Contains("tasks step getid", StringComparison.OrdinalIgnoreCase),
				$"{name} help should point to the action catalogue instead of inlining the action list.");
		}

		var managerHelpTexts = new[]
		{
			("employment", EmploymentCommandService.EmploymentHelp),
			("stable", HelpConstant(typeof(EconomyModule), "StableHelp")),
			("shop", HelpConstant(typeof(EconomyModule), "ShopHelpPlayers")),
			("shop admin", HelpConstant(typeof(EconomyModule), "ShopHelpAdmins")),
			("bank", HelpConstant(typeof(EconomyModule), "BankHelpText")),
			("bank admin", HelpConstant(typeof(EconomyModule), "BankAdminHelpText")),
			("auction", HelpConstant(typeof(EconomyModule), "AuctionHelp")),
			("auction admin", HelpConstant(typeof(EconomyModule), "AuctionHelpAdmins")),
			("arena manager", HelpConstant(typeof(ArenaModule), "ArenaManagerHelp")),
			("roomrent", HelpConstant(typeof(PropertyModule), "RoomRentManagerHelp"))
		};

		foreach (var (name, help) in managerHelpTexts)
		{
			Assert.IsTrue(help.Contains("tasks actions", StringComparison.OrdinalIgnoreCase),
				$"{name} help should advertise task action catalogue discovery.");
			Assert.IsTrue(help.Contains("tasks conditions", StringComparison.OrdinalIgnoreCase),
				$"{name} help should advertise scheduled-rule condition catalogue discovery.");
			Assert.IsTrue(help.Contains("tasks rule show", StringComparison.OrdinalIgnoreCase),
				$"{name} help should advertise scheduled-rule detail viewing.");
			Assert.IsFalse(help.Contains("tasks step getid", StringComparison.OrdinalIgnoreCase),
				$"{name} help should point to the action catalogue instead of inlining the action list.");
			Assert.IsFalse(help.Contains("tasks step gettag", StringComparison.OrdinalIgnoreCase),
				$"{name} help should point to the action catalogue instead of inlining the action list.");
		}
	}

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

		service.ExecuteForHost(outsider, host, new StringStack("delegations"));

		Mock.Get(outsider.OutputHandler).Verify(x => x.Send(
			It.Is<string>(text => text.Contains("not an employee")),
			It.IsAny<bool>(),
			It.IsAny<bool>()), Times.Once);
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
	public void EmploymentCommandService_ManagerAliasHelpRequiresManagerOrMeaningfulDelegation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(15, "Manager").Object;
		var deliveryWorker = Character(16, "Delivery").Object;
		var taskLead = Character(17, "Task Lead").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager), null);
		host.Hire(deliveryWorker, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.ManageDeliveryRoutes), null);
		host.Hire(taskLead, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.AssignTasks), null);

		Assert.IsTrue(EmploymentCommandService.CanViewManagerAliasHelp(manager, host));
		Assert.IsFalse(EmploymentCommandService.CanViewManagerAliasHelp(deliveryWorker, host));
		Assert.IsTrue(EmploymentCommandService.CanViewManagerAliasHelp(taskLead, host));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerAliasHelpAllowsAdminsAndLegacySubsystemManagers()
	{
		var admin = Character(18, "Admin", administrator: true).Object;
		var legacyManager = Character(19, "Legacy Manager").Object;
		var outsider = Character(21, "Outsider").Object;

		Assert.IsTrue(EmploymentCommandService.CanViewManagerAliasHelp(admin, null));
		Assert.IsTrue(EmploymentCommandService.CanViewManagerAliasHelp(legacyManager, null, subsystemManagerAccess: true));
		Assert.IsFalse(EmploymentCommandService.CanViewManagerAliasHelp(outsider, null));
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
	public void EmploymentCommandService_DeprecatedMarketPricePayloadsAreQuarantinedOnLoad()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			var board = new MudSharp.Models.Board
			{
				Id = 900,
				Name = "legacy employment board",
				ShowOnLogin = false
			};
			var hostState = new MudSharp.Models.EmploymentHostState
			{
				Id = 901,
				HostType = EmploymentHostType.Shop.ToString(),
				HostId = 902,
				BoardId = board.Id,
				Board = board,
				CreatedAt = DateTime.UtcNow,
				LastUpdatedAt = DateTime.UtcNow
			};
			var plan = new MudSharp.Models.EmploymentActionPlanRecord
			{
				Id = 903,
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				Name = "legacy market pricing plan"
			};
			var legacyPayload = "{\"Kind\":\"MarketCategory\",\"MerchandiseSelector\":\"\",\"CurrencyId\":null,\"ExactPrice\":null,\"MarketSelector\":\"regional market\",\"CategorySelector\":\"food\",\"SupplyImpact\":0.25,\"DemandImpact\":-0.10,\"FlatPriceImpact\":0.05,\"InfluenceName\":\"legacy manager influence\",\"DurationTicks\":864000000000,\"UntilText\":\"one day\"}";
			plan.Steps.Add(new MudSharp.Models.EmploymentActionStepRecord
			{
				Id = 904,
				SortOrder = 0,
				StepType = (int)EmploymentActionStepType.PriceChange,
				RequiredAuthority = (long)EmploymentAuthority.AdjustPrices,
				RequiredCapabilities = EmploymentAICapability.CanManagePrices.ToString(),
				RequiresPaymentAuthorisation = false,
				IsFinancialStep = false,
				Description = "legacy market price action",
				BoardText = legacyPayload
			});
			var ruleId = Guid.NewGuid();
			var taskId = Guid.NewGuid();
			var goalId = Guid.NewGuid();
			context.Boards.Add(board);
			context.EmploymentHostStates.Add(hostState);
			context.EmploymentActionPlans.Add(plan);
			context.EmploymentScheduledTaskRules.Add(new MudSharp.Models.EmploymentScheduledTaskRuleRecord
			{
				PublicId = ruleId.ToString("D"),
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				Name = "legacy price rule",
				IdempotencyKey = "legacy-price-rule",
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				Status = (int)EmploymentScheduledRuleStatus.Active,
				CooldownTicks = TimeSpan.FromHours(1.0).Ticks
			});
			context.EmploymentActiveTasks.Add(new MudSharp.Models.EmploymentActiveTaskRecord
			{
				PublicId = Guid.NewGuid().ToString("D"),
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				Name = "legacy price task",
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				Status = (int)EmploymentTaskStatus.Pending,
				CorrelationId = taskId.ToString("D"),
				IdempotencyKey = "legacy-price-task"
			});
			context.EmploymentManagerGoals.Add(new MudSharp.Models.EmploymentManagerGoalRecord
			{
				RuntimeId = 1,
				EmploymentHostState = hostState,
				EmploymentHostStateId = hostState.Id,
				GoalType = (int)ManagerGoalType.AdjustPricesForProfit,
				RequiredAuthority = (long)EmploymentAuthority.AdjustPrices,
				Status = (int)ManagerGoalStatus.Active,
				ConfigurationDescription = "legacy manager pricing",
				EmploymentActionPlan = plan,
				EmploymentActionPlanId = plan.Id,
				Priority = 10,
				EvaluationCadenceTicks = TimeSpan.FromHours(1.0).Ticks,
				CorrelationId = goalId.ToString("D")
			});
			context.SaveChanges();

			IEmploymentHost reloaded = new PersistedEmploymentHost(902, "persistent shop", gameworld.Object, currency.Object);
			var employment = reloaded.Employment;

			Assert.AreEqual(EmploymentScheduledRuleStatus.Paused, employment.TaskBoard.ScheduledRules.Single().Status);
			Assert.AreEqual(EmploymentTaskStatus.Blocked, employment.TaskBoard.ActiveTasks.Single().Status);
			Assert.AreEqual(ManagerGoalStatus.Blocked, employment.ManagerGoalBoard.Goals.Single().Status);
			Assert.AreEqual((int)EmploymentScheduledRuleStatus.Paused, context.EmploymentScheduledTaskRules.Single().Status);
			Assert.AreEqual((int)EmploymentTaskStatus.Blocked, context.EmploymentActiveTasks.Single().Status);
			Assert.AreEqual((int)ManagerGoalStatus.Blocked, context.EmploymentManagerGoals.Single().Status);
			Assert.AreEqual(3, employment.EmploymentRegister.Entries.Count(x =>
				x.Description.Contains("Deprecated employment market-price action", StringComparison.OrdinalIgnoreCase)));
			Assert.IsTrue(employment.EmploymentRegister.Entries.Any(x =>
				x.Description.Contains("regional market", StringComparison.OrdinalIgnoreCase) &&
				x.Description.Contains("food", StringComparison.OrdinalIgnoreCase) &&
				x.Description.Contains("legacy manager influence", StringComparison.OrdinalIgnoreCase)));
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
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.ManageDeliveryRoutes), null);
		var service = new EmploymentCommandService();

		var result = service.TryCreateOpening(manager, host, EmploymentRole.Courier, 12.5M, 2, out var opening,
			out var message);

		Assert.IsTrue(result, message);
		Assert.IsNotNull(opening);
		Assert.AreEqual(EmploymentRole.Courier, opening.Role);
		Assert.IsTrue(opening.Authority.Contains(EmploymentAuthority.ManageDeliveryRoutes));
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
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateJobOpenings | EmploymentAuthority.ManageDeliveryRoutes), null);
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
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("delegations"));
		Assert.IsTrue(EmploymentCommandService.IsEmploymentShortcut("authority"));
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
	public void EmploymentCommandService_CreateOpeningUsesSensibleRoleDefaultAuthorities()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var admin = Character(33, "Admin", administrator: true).Object;
		host.Hire(admin, Offer(currency.Object, EmploymentRole.Proprietor, EmploymentAuthoritySet.All.Authorities), null);
		var service = new EmploymentCommandService();

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.Employee, 10.0M, 1, out var employeeOpening,
			out var message), message);
		Assert.AreEqual(EmploymentAuthority.ManageDeliveryRoutes, employeeOpening!.Authority.Authorities);

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.Clerk, 10.0M, 1, out var clerkOpening,
			out message), message);
		Assert.AreEqual(EmploymentAuthority.ManageDeliveryRoutes, clerkOpening!.Authority.Authorities);

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.Crafter, 10.0M, 1, out var crafterOpening,
			out message), message);
		Assert.AreEqual(
			EmploymentAuthority.ManageCraftRules | EmploymentAuthority.ManageDeliveryRoutes,
			crafterOpening!.Authority.Authorities);

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.BankTeller, 10.0M, 1,
			out var tellerOpening, out message), message);
		Assert.AreEqual(
			EmploymentAuthority.DepositBusinessCash | EmploymentAuthority.WithdrawBusinessCash,
			tellerOpening!.Authority.Authorities);

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.Manager, 10.0M, 1, out var managerOpening,
			out message), message);
		Assert.IsTrue(managerOpening!.Authority.Contains(EmploymentAuthority.HireEmployees));
		Assert.IsTrue(managerOpening.Authority.Contains(EmploymentAuthority.AssignTasks));
		Assert.IsTrue(managerOpening.Authority.Contains(EmploymentAuthority.ManageDeliveryRoutes));
		Assert.IsTrue(managerOpening.Authority.Contains(EmploymentAuthority.ManageCraftRules));
		Assert.IsTrue(managerOpening.Authority.Contains(EmploymentAuthority.PostToHostBoard));
		Assert.IsFalse(managerOpening.Authority.Contains(EmploymentAuthority.ApprovePurchases));
		Assert.IsFalse(managerOpening.Authority.Contains(EmploymentAuthority.UseStoreAccount));
		Assert.IsFalse(managerOpening.Authority.Contains(EmploymentAuthority.WithdrawBusinessCash));
		Assert.IsFalse(managerOpening.Authority.Contains(EmploymentAuthority.DepositBusinessCash));
		Assert.IsFalse(managerOpening.Authority.Contains(EmploymentAuthority.PayTaxes));

		Assert.IsTrue(service.TryCreateOpening(admin, host, EmploymentRole.Proprietor, 10.0M, 1,
			out var proprietorOpening, out message), message);
		Assert.AreEqual(EmploymentAuthoritySet.All.Authorities, proprietorOpening!.Authority.Authorities);
	}

	[TestMethod]
	public void EmploymentCommandService_CreateOpeningCannotDelegateUnheldDefaultAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(34, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.CreateJobOpenings), null);
		var service = new EmploymentCommandService();

		var result = service.TryCreateOpening(manager, host, EmploymentRole.Employee, 10.0M, 1, out var opening,
			out var message);

		Assert.IsFalse(result);
		Assert.IsNull(opening);
		StringAssert.Contains(message, "delegate authority");
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
	public void EmploymentCommandService_ContractsFireTerminatesContractThroughEmploymentSystem()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(48, "Manager").Object;
		var employee = Character(49, "Employee").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.FireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack($"contracts fire #{contract.Id}"));

		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.AreEqual(EmploymentTerminationReason.Fired, contract.EndReason);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.ContractEnded));
	}

	[TestMethod]
	public void EmploymentCommandService_DirectHireAndFireUseActiveEmploymentContracts()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(52, "Manager").Object;
		var employee = Character(53, "Employee").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees |
			EmploymentAuthority.FireEmployees |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var service = new EmploymentCommandService();

		Assert.IsTrue(service.TryHireDirectContract(manager, host, employee, EmploymentRole.Employee, out var contract,
			out var message), message);
		Assert.IsNotNull(contract);
		Assert.IsTrue(host.HasActiveEmploymentContract(employee));
		StringAssert.Contains(host.ActiveEmploymentContractsTable(manager), "Employee");

		Assert.IsTrue(service.TryTerminateContractsForEmployee(manager, host, "Employee", out message), message);
		Assert.AreEqual(EmploymentStatus.Ended, contract.Status);
		Assert.IsFalse(host.HasActiveEmploymentContract(employee));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagersCannotCreateOrTerminateProprietorContracts()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(54, "Manager").Object;
		var owner = Character(55, "Owner").Object;
		var employee = Character(56, "Employee").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var proprietor = host.Hire(owner, Offer(currency.Object, EmploymentRole.Proprietor), null);
		var service = new EmploymentCommandService();

		Assert.IsFalse(service.TryHireDirectContract(manager, host, employee, EmploymentRole.Proprietor,
			out var contract, out var hireMessage));
		Assert.IsNull(contract);
		StringAssert.Contains(hireMessage, "proprietor");

		Assert.IsFalse(service.TryTerminateContract(manager, host, proprietor.Id, out var fireMessage));
		StringAssert.Contains(fireMessage, "proprietor");
		Assert.AreEqual(EmploymentStatus.Active, proprietor.Status);
	}

	[TestMethod]
	public void EmploymentCommandService_ContractDelegationsCanBeGrantedAndRevoked()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(58, "Manager").Object;
		var employee = Character(59, "Employee").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.ManageDeliveryRoutes), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack($"contracts delegate #{contract.Id} grant delivery"));

		Assert.IsTrue(contract.Authority.Contains(EmploymentAuthority.ManageDeliveryRoutes));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.AuthorityChanged));

		var registerCount = host.EmploymentRegister.Entries.Count(x => x.EntryType == EmploymentRegisterEntryType.AuthorityChanged);
		Assert.IsFalse(service.TryGrantContractAuthority(manager, host, contract.Id,
			new EmploymentAuthoritySet(EmploymentAuthority.ApprovePurchases), out var message));
		StringAssert.Contains(message, "cannot delegate");
		Assert.IsFalse(contract.Authority.Contains(EmploymentAuthority.ApprovePurchases));
		Assert.AreEqual(registerCount, host.EmploymentRegister.Entries.Count(x => x.EntryType == EmploymentRegisterEntryType.AuthorityChanged));

		service.ExecuteForHost(manager, host, new StringStack($"delegations #{contract.Id} revoke delivery"));

		Assert.IsFalse(contract.Authority.Contains(EmploymentAuthority.ManageDeliveryRoutes));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagersCannotAlterAuthorityTheyDoNotPossessButAdminsCan()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(70, "Manager").Object;
		var employee = Character(71, "Employee").Object;
		var admin = Character(72, "Admin", administrator: true).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.HireEmployees), null);
		var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.ApprovePurchases), null);
		var service = new EmploymentCommandService();

		Assert.IsFalse(service.TrySetContractAuthority(manager, host, contract.Id, EmploymentAuthoritySet.Empty,
			out var managerMessage));
		StringAssert.Contains(managerMessage, "cannot alter");
		Assert.IsTrue(contract.Authority.Contains(EmploymentAuthority.ApprovePurchases));

		Assert.IsTrue(service.TrySetContractAuthority(admin, host, contract.Id,
			new EmploymentAuthoritySet(EmploymentAuthority.AssignTasks | EmploymentAuthority.PostToHostBoard),
			out var adminMessage), adminMessage);
		Assert.IsTrue(contract.Authority.Contains(EmploymentAuthority.AssignTasks));
		Assert.IsTrue(contract.Authority.Contains(EmploymentAuthority.PostToHostBoard));
		Assert.IsFalse(contract.Authority.Contains(EmploymentAuthority.ApprovePurchases));
	}

	[TestMethod]
	public void EmploymentCommandService_AcceptApplicationRejectsClosedOrChangedOpening()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(145, "Manager").Object;
		var firstApplicant = Character(146, "First").Object;
		var secondApplicant = Character(147, "Second").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.ModifyJobOpenings), null);
		var closedOpening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var closedApplication = host.Employment.Apply(closedOpening, Profile(firstApplicant, PaymentMethodKind.Cash));
		host.Employment.CloseJobOpening(closedOpening, manager, "No longer hiring.");
		var changedOpening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var changedApplication = host.Employment.Apply(changedOpening, Profile(secondApplicant, PaymentMethodKind.Cash));
		host.Employment.ModifyJobOpening(changedOpening, Opening(currency.Object) with { MaxPositions = 2 }, manager, "Changed terms.");
		var service = new EmploymentCommandService();

		Assert.IsFalse(service.TryAcceptApplication(manager, host, closedApplication.Id, out _, out var closedMessage));
		StringAssert.Contains(closedMessage, "no longer accepting");
		Assert.IsFalse(service.TryAcceptApplication(manager, host, changedApplication.Id, out _, out var changedMessage));
		StringAssert.Contains(changedMessage, "terms have changed");
		Assert.AreEqual(JobApplicationStatus.Pending, closedApplication.Status);
		Assert.AreEqual(JobApplicationStatus.Pending, changedApplication.Status);
	}

	[TestMethod]
	public void EmploymentCommandService_PositionReopensWhenOriginatingContractEnds()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(148, "Manager").Object;
		var first = Character(149, "First").Object;
		var second = Character(150, "Second").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.FireEmployees), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var firstApplication = host.Employment.Apply(opening, Profile(first, PaymentMethodKind.Cash));
		var service = new EmploymentCommandService();

		Assert.IsTrue(service.TryAcceptApplication(manager, host, firstApplication.Id, out var contract, out var message), message);
		Assert.IsFalse(opening.AcceptsApplications);
		Assert.AreEqual(1, opening.OccupiedPositions);

		host.Fire(contract!, EmploymentTerminationReason.Fired, manager);

		Assert.AreEqual(0, opening.OccupiedPositions);
		Assert.IsTrue(opening.AcceptsApplications);
		var secondApplication = host.Employment.Apply(opening, Profile(second, PaymentMethodKind.Cash));
		Assert.AreEqual(JobApplicationStatus.Pending, secondApplication.Status);
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
	public void EmploymentCommandService_AcceptsApplicationByDisplayedIdNotVisibleRowNumber()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(54, "Manager").Object;
		var first = Character(55, "First").Object;
		var second = Character(56, "Second").Object;
		var third = Character(57, "Third").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.HireEmployees), null);
		var firstOpening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var secondOpening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var acceptedApplication = host.Employment.Apply(firstOpening, Profile(first, PaymentMethodKind.Cash));
		var rejectedApplication = host.Employment.Apply(firstOpening, Profile(second, PaymentMethodKind.Cash));
		var pendingApplication = host.Employment.Apply(secondOpening, Profile(third, PaymentMethodKind.Cash));
		var service = new EmploymentCommandService();
		Assert.IsTrue(service.TryAcceptApplication(manager, host, acceptedApplication.Id, out _, out var message), message);
		Assert.IsTrue(service.TryRejectApplication(manager, host, rejectedApplication.Id, "Rejected by a manager.", out message), message);
		StringAssert.Contains(service.RenderApplications(manager, host), $"#{pendingApplication.Id}");

		service.ExecuteForHost(manager, host, new StringStack($"applications accept #{pendingApplication.Id}"));

		Assert.AreEqual(JobApplicationStatus.Accepted, pendingApplication.Status);
		Assert.AreEqual(JobApplicationStatus.Accepted, acceptedApplication.Status);
		Assert.AreEqual(JobApplicationStatus.Rejected, rejectedApplication.Status);
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
	public void EmploymentCommandService_ModifiedJobOpeningSurvivesReload()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(58, "Manager").Object;
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			var currencies = new All<ICurrency>();
			currencies.Add(currency.Object);
			gameworld.SetupGet(x => x.Currencies).Returns(currencies);
			gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			         .Returns((long id, bool _) => id == manager.Id ? manager : null!);
			IEmploymentHost host = new PersistedEmploymentHost(53, "persistent shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.CreateJobOpenings |
				EmploymentAuthority.ModifyJobOpenings |
				EmploymentAuthority.ManageCraftRules |
				EmploymentAuthority.ManageDeliveryRoutes), null);
			var opening = host.Employment.CreateJobOpening(Opening(currency.Object), manager);
			var modified = Opening(currency.Object) with
			{
				Role = EmploymentRole.Crafter,
				MaxPositions = 3,
				NpcApplicationsOnly = false,
				Requirements = new JobRequirementSet(
					[new SkillRequirement("crafting", 25.0)],
					[new KnowledgeRequirement("kiln lore")],
					[new AICapabilityRequirement(EmploymentAICapability.CanCraft)],
					[new TagRequirement("workshop")]),
				Authority = new EmploymentAuthoritySet(
					EmploymentAuthority.ManageCraftRules |
					EmploymentAuthority.ManageDeliveryRoutes)
			};

			host.Employment.ModifyJobOpening(opening, modified, manager, "persisted change");

			IEmploymentHost reloaded = new PersistedEmploymentHost(53, "persistent shop", gameworld.Object, currency.Object);
			var reloadedOpening = reloaded.JobOpenings.Single();

			Assert.AreEqual(1, context.EmploymentJobOpenings.Count());
			Assert.AreEqual(4, context.EmploymentJobOpeningRequirements.Count());
			Assert.AreEqual(EmploymentRole.Crafter, reloadedOpening.Role);
			Assert.AreEqual(3, reloadedOpening.MaxPositions);
			Assert.IsFalse(reloadedOpening.NpcApplicationsOnly);
			Assert.AreEqual(1, reloadedOpening.Requirements.Skills.Count);
			Assert.AreEqual(EmploymentAuthority.ManageCraftRules | EmploymentAuthority.ManageDeliveryRoutes,
				reloadedOpening.Authority.Authorities);
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x =>
				x.EntryType == EmploymentRegisterEntryType.JobOpeningModified));
		}
		finally
		{
			RestoreFMDBState(fmdbState);
		}
	}

	[TestMethod]
	public void EmploymentCommandService_PayrollRunSettleAndListUsesOverdueDays()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		IEmploymentHost host = shop.Object;
		var manager = Character(60, "Manager").Object;
		var employee = Character(61, "Employee").Object;
		var service = new EmploymentCommandService();
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.HireEmployees | EmploymentAuthority.ManagePayroll), null);
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), manager);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(2).AddMinutes(1));
		VirtualCashLedger.Credit(shop.Object, currency.Object,
			host.Payroll.OutstandingLiabilities.Sum(x => x.Amount.Amount), null, shop.Object, "Seed",
			"Seed payroll balance");

		var list = service.RenderPayroll(manager, host);

		StringAssert.Contains(list, "Outstanding");
		StringAssert.Contains(list, "days overdue");
		Assert.IsTrue(service.TrySettlePayroll(manager, host, "all", "funded cash payroll", out var message), message);
		StringAssert.Contains(message, "ready for employee claim");
		Assert.IsTrue(host.Payroll.Payables.All(x => x.Status == EmploymentPayableStatus.ReadyToClaim));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Wage));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.WageSettled));
	}

	[TestMethod]
	public void EmploymentCommandService_PayrollClaimAllExplainsUnsettledAccruedPayables()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var employee = Character(62, "Employee").Object;
		var service = new EmploymentCommandService();
		var compensation = new CompensationTerms(
			new MoneyAmount(currency.Object, 10.0M),
			null,
			PayCadence.Daily,
			new MoneyAmount(currency.Object, 10.0M),
			PaymentSource.HostCash);
		var contract = host.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			compensation,
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(1).AddMinutes(1));

		Assert.IsFalse(service.TryClaimPayroll(employee, host, "all", out var message));

		StringAssert.Contains(message, "outstanding employment payable");
		StringAssert.Contains(message, "not been settled by the employer yet");
	}

	[TestMethod]
	public void ImpDebugPayrollAdvancesAllLoadedEmploymentHosts()
	{
		var currency = Currency();
		var employee = Character(62, "Employee").Object;
		var shop = EmploymentHostMock<IShop>(63, "debug market", EmploymentHostType.Shop,
			out var state);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			Pay(currency.Object),
			new WorkSchedule("Day shift", TimeSpan.FromHours(9.0), TimeSpan.FromHours(17.0)),
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		var shops = new All<IShop>();
		shops.Add(shop.Object);
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.Shops).Returns(shops);

		var output = ImplementorModule.DebugAdvanceEmploymentPayrolls(
			gameworld.Object,
			contract.StartedAt.AddDays(1).AddMinutes(1));

		Assert.AreEqual(1, state.Payroll.Payables.Count);
		Assert.AreEqual(1, state.Payroll.OutstandingLiabilities.Count);
		Assert.AreEqual(80.0M, state.Payroll.Payables.Single().Amount.Amount);
		Assert.IsTrue(state.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.WageAccrued));
		StringAssert.Contains(output, "Employment payroll debug run complete");
		StringAssert.Contains(output, "debug market");
	}

	[TestMethod]
	public void EmploymentCommandService_DelegatedAuthoritySurvivesReload()
	{
		var fmdbState = CaptureFMDBState();
		using var context = BuildContext();
		try
		{
			PrimeFMDB(context);
			var currency = Currency();
			var manager = Character(73, "Manager").Object;
			var employee = Character(74, "Employee").Object;
			var gameworld = Gameworld();
			SetupBoardPersistence(gameworld);
			var currencies = new All<ICurrency>();
			currencies.Add(currency.Object);
			gameworld.SetupGet(x => x.Currencies).Returns(currencies);
			gameworld.Setup(x => x.TryGetCharacter(It.IsAny<long>(), It.IsAny<bool>()))
			         .Returns((long id, bool _) => id == manager.Id ? manager : id == employee.Id ? employee : null!);
			IEmploymentHost host = new PersistedEmploymentHost(75, "persistent shop", gameworld.Object, currency.Object);
			host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
				EmploymentAuthority.HireEmployees | EmploymentAuthority.AssignTasks), null);
			var contract = host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
			var service = new EmploymentCommandService();

			Assert.IsTrue(service.TryGrantContractAuthority(manager, host, contract.Id,
				new EmploymentAuthoritySet(EmploymentAuthority.AssignTasks), out var message), message);

			IEmploymentHost reloaded = new PersistedEmploymentHost(75, "persistent shop", gameworld.Object, currency.Object);
			var reloadedContract = reloaded.EmploymentContracts.Single(x => x.Employee.Id == employee.Id);

			Assert.IsTrue(reloadedContract.Authority.Contains(EmploymentAuthority.AssignTasks));
			Assert.IsTrue(reloaded.EmploymentRegister.Entries.Any(x =>
				x.EntryType == EmploymentRegisterEntryType.AuthorityChanged));
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
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.Tags).Returns(Tags(Tag(1, "apple").Object, Tag(2, "display").Object));
		var manager = Character(60, "Manager", gameworld: gameworld.Object).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Restock shelves", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("gettag 2 &apple from here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("deliver to here containertag &display"), out message), message);
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
	public void EmploymentCommandService_TaskDraftGetIdUsesPrototypeIds()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var cell = Cell(690, "stockroom").Object;
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.ItemProtos).Returns(ItemProtos(ItemProto(500, "a pair of leather gloves").Object).Object);
		var manager = Character(69, "Manager", gameworld: gameworld.Object, location: cell).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Restock gloves", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("getid 2 500 from here"), out message), message);
		var rendered = authoring.RenderDraft(manager, host);

		StringAssert.Contains(rendered, "Restock gloves");
		StringAssert.Contains(rendered, "a pair of leather gloves");
		StringAssert.Contains(rendered, "prototype #");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskItemSelectorsUsePrototypeSpecificTagAndKeyword()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var cell = Cell(691, "stockroom").Object;
		var prototype = ItemProto(500, "a cargo crate").Object;
		var liveItem = Item(700, "a battered crate", prototype, [cell]).Object;
		var keywordItem = Item(701, "a labelled crate", prototype, [cell]).Object;
		var cargoTag = Tag(800, "cargo").Object;
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.ItemProtos).Returns(ItemProtos(prototype).Object);
		gameworld.SetupGet(x => x.Tags).Returns(Tags(cargoTag));
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns((long id, bool _) => id == liveItem.Id ? liveItem : id == keywordItem.Id ? keywordItem : null!);
		var manager = Character(70, "Manager", gameworld: gameworld.Object, location: cell).Object;
		Mock.Get(manager).Setup(x => x.TargetLocalOrHeldItem("labelled")).Returns(keywordItem);
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Selectors", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("getid 1 *700 from here"), out message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host, new StringStack("gettag 1 500 from here"), out message));
		StringAssert.Contains(message, "must use explicit tag syntax");
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("load all into 500 at here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("unload *700 at here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("load all into &800 at here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("deliver to here container &cargo"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("return container labelled to here"), out message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host, new StringStack("load all into &missing at here"), out message));
		var rendered = authoring.RenderDraft(manager, host);

		StringAssert.Contains(rendered, "a battered crate");
		StringAssert.Contains(rendered, "prototype #");
		StringAssert.Contains(rendered, "item tagged");
		StringAssert.Contains(message, "There is no tag matching");
		StringAssert.Contains(rendered, "a labelled crate");
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
		StringAssert.Contains(output, "tasks step purchase");
		StringAssert.Contains(output, "Executable");
		StringAssert.Contains(output, "Deferred");
	}

	[TestMethod]
	public void EmploymentActionCatalog_ContainsPlannedFamiliesWithExpectedStatuses()
	{
		var executable = new[]
		{
			"getid", "gettag", "commodity", "deliver", "stocktransfer", "auctionlist", "auctionsettle", "auctionclaim", "arenaevent", "bankadmin", "stableadmin", "hoteladmin", "move", "board", "command", "report", "supplier",
			"authorise", "reserve", "release", "select", "estimate", "route", "routebatch", "tripcheck", "load", "unload",
			"return", "vehicle", "bankdeposit", "bankwithdraw", "purchase", "storepay", "craft",
			"paytax", "cashreconcile", "float", "physicalfloat", "station", "inspect", "batch", "animal", "stocktake", "price", "sale", "jobopening", "rule", "admintask", "goal"
		};
		var deferred = new[]
		{
			"marktask"
		};

		foreach (var key in executable)
		{
			Assert.AreEqual(EmploymentActionCatalogStatus.Executable, EmploymentActionCatalog.Get(key)?.Status, key);
		}

		foreach (var key in deferred)
		{
			Assert.AreEqual(EmploymentActionCatalogStatus.Deferred, EmploymentActionCatalog.Get(key)?.Status, key);
		}

		Assert.IsTrue(EmploymentActionCatalog.ForCategory("planning").Any(x => x.Key == "supplier"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("logistics").Any(x => x.Key == "stocktransfer"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("logistics").Any(x => x.Key == "auctionclaim"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "auctionlist"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "auctionsettle"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "arenaevent"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "bankadmin"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "stableadmin"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "hoteladmin"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("purchasing").Any(x => x.Key == "storepay"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "cashreconcile"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "stocktake"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "price"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "sale"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "rule"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "admintask"));
		Assert.IsTrue(EmploymentActionCatalog.ForCategory("administration").Any(x => x.Key == "goal"));
	}

	[TestMethod]
	public void EmploymentActionCatalog_NonDeferredEntriesHaveImplementationMetadata()
	{
		foreach (var definition in EmploymentActionCatalog.All.Where(x => x.Status != EmploymentActionCatalogStatus.Deferred))
		{
			Assert.IsTrue(definition.StepType.HasValue, definition.Key);
			StringAssert.Contains(definition.Syntax, "tasks step");
			Assert.IsFalse(string.IsNullOrWhiteSpace(definition.Summary), definition.Key);
			Assert.AreNotEqual(EmploymentAuthority.None, definition.RequiredAuthority.Authorities, definition.Key);
		}
	}

	[TestMethod]
	public void EmploymentCommandService_TaskActionsCanFilterByCategoryAndAction()
	{
		var manager = Character(67, "Manager").Object;
		var authoring = new EmploymentTaskAuthoringService();

		var purchasing = authoring.RenderAvailableActions(manager, "purchasing");
		var board = authoring.RenderAvailableActions(manager, "board");

		StringAssert.Contains(purchasing, "bankdeposit");
		StringAssert.Contains(purchasing, "storepay");
		StringAssert.Contains(purchasing, "payroll");
		StringAssert.Contains(board, "tasks step board");
		StringAssert.Contains(board, "CanPostToBoard");
		StringAssert.Contains(board, "Executable");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesSupplierSelectionActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(81, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageStockRules), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Supplier Search", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("supplier 2 apple from any max 5"), out message), message);

		var rendered = authoring.RenderDraft(manager, host);
		StringAssert.Contains(rendered, "find supplier for buy");
		StringAssert.Contains(rendered, "ManageStockRules");
	}
	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesCatalogueSafeActions()
	{
		var currency = Currency();
		decimal five = 5.0M;
		currency.Setup(x => x.TryGetBaseCurrency("5", out five)).Returns(true);
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.Tags).Returns(Tags(Tag(801, "crate").Object, Tag(802, "depot").Object));
		gameworld.SetupGet(x => x.ItemProtos)
		         .Returns(ItemProtos(ItemProto(500, "a bundle of test stock").Object).Object);
		var manager = Character(68, "Manager", gameworld: gameworld.Object).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.PostToHostBoard |
			EmploymentAuthority.ApprovePurchases |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash |
			EmploymentAuthority.UseStoreAccount |
			EmploymentAuthority.ManagePayroll |
			EmploymentAuthority.ManageCraftRules |
			EmploymentAuthority.ManageStockRules), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Catalogue", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("move to here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("board Notice = Please check stock"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("command at here say hello"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("purchase 5 for apples"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("purchase 2 item 500 from any max 5"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("purchase 12 commodity Iron|Nails|grade=refined from any max 5"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("bankdeposit 5"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("bankwithdraw 5"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("storepay supplier amount 5"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("payroll settle all routine settlement"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("craft linen bundles"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("station here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("physicalfloat issue 5 from bank"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("load all into &crate at here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("unload &crate at here"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("return container &crate to here containertag &depot"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("report shelves checked"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("authorise 5 for purchase approved"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("reserve 5 for feed crates"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("release all"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("select supplier A"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("estimate three trips"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("route to here then here test path"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("routebatch total 100 each 50 to here then here daily route"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("tripcheck fuel topped feed packed maintenance inspected rest scheduled to here then here dawn route"), out message), message);

		var rendered = authoring.RenderDraft(manager, host);

		StringAssert.Contains(rendered, "Executable");
		StringAssert.Contains(rendered, "PostToHostBoard");
		StringAssert.Contains(rendered, "linked bank account");
		StringAssert.Contains(rendered, "settle payroll");
		StringAssert.Contains(rendered, "reserve");
		StringAssert.Contains(rendered, "route");
		StringAssert.Contains(rendered, "physical employment float");
		StringAssert.Contains(rendered, "validate craft station");
		StringAssert.Contains(rendered, "load all carried task items");
		StringAssert.Contains(rendered, "return container");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesExecutablePriceAndSaleActions()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(200);
		merchandise.SetupGet(x => x.Name).Returns("apple pie");
		merchandise.SetupGet(x => x.ListDescription).Returns("apple pie");
		merchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		shop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);

		var manager = Character(69, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.AdjustPrices), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, shop.Object, "Prices", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object, new StringStack("price merch 200 12.50"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object,
			new StringStack("sale create summer sale target all adjustment -10% expires never"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object,
			new StringStack("sale create volume special target merchandise 200 adjustment -5% type volume 3 applies sell cumulative false expires never"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object,
			new StringStack("sale modify summer sale target all adjustment -15% type sale expires never"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object,
			new StringStack("sale cancel summer sale"), out message), message);

		var rendered = authoring.RenderDraft(manager, shop.Object);
		StringAssert.Contains(rendered, "set merchandise");
		StringAssert.Contains(rendered, "shop deal");
		StringAssert.Contains(rendered, "volume");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesShopStockTransferActions()
	{
		var currency = Currency();
		var gameworld = Gameworld();
		var sourceStockroom = Cell(80, "source stockroom").Object;
		var sourceShopfront = Cell(81, "source shopfront").Object;
		var targetStockroom = Cell(82, "target stockroom").Object;
		var targetShopfront = Cell(83, "target shopfront").Object;
		var cells = new All<ICell>();
		cells.Add(sourceStockroom);
		cells.Add(sourceShopfront);
		cells.Add(targetStockroom);
		cells.Add(targetShopfront);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var manager = Character(80, "Manager", gameworld: gameworld.Object, location: sourceStockroom).Object;
		var sourceMerchandise = new Mock<IMerchandise>();
		sourceMerchandise.SetupGet(x => x.Id).Returns(801);
		sourceMerchandise.SetupGet(x => x.Name).Returns("source linen");
		var targetMerchandise = new Mock<IMerchandise>();
		targetMerchandise.SetupGet(x => x.Id).Returns(802);
		targetMerchandise.SetupGet(x => x.Name).Returns("target linen");
		var source = EmploymentHostMock<IPermanentShop>(10, "source shop", EmploymentHostType.Shop,
			out var sourceState);
		var target = EmploymentHostMock<IPermanentShop>(11, "target shop", EmploymentHostType.Shop,
			out var targetState);
		SetupPermanentShop(source, currency.Object, sourceStockroom, [sourceShopfront], [sourceMerchandise.Object]);
		SetupPermanentShop(target, currency.Object, targetStockroom, [targetShopfront], [targetMerchandise.Object]);
		source.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => sourceState.HasAuthority(actor, authority));
		target.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => targetState.HasAuthority(actor, authority));
		sourceState.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var shops = new All<IShop>();
		shops.Add(source.Object);
		shops.Add(target.Object);
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, source.Object, "Transfer linen", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, source.Object,
			new StringStack("stocktransfer to 11 merch 802 destination 82"), out message), message);
		var rendered = authoring.RenderDraft(manager, source.Object);

		StringAssert.Contains(rendered, "transfer carried stock to");
		StringAssert.Contains(rendered, "target shop");
		StringAssert.Contains(rendered, "target linen");
		StringAssert.Contains(rendered, "stocktransfer");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesAuctionHouseActions()
	{
		var currency = Currency();
		var gameworld = Gameworld();
		var auctionCell = Cell(90, "auction floor").Object;
		var cells = new All<ICell>();
		cells.Add(auctionCell);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var prototype = ItemProto(900, "a brass urn").Object;
		gameworld.SetupGet(x => x.ItemProtos).Returns(ItemProtos(prototype).Object);

		var zone = new Mock<IEconomicZone>();
		zone.SetupGet(x => x.Id).Returns(91);
		zone.SetupGet(x => x.Name).Returns("auction zone");
		zone.SetupGet(x => x.Currency).Returns(currency.Object);

		var auction = EmploymentHostMock<IAuctionHouse>(90, "central auction", EmploymentHostType.AuctionHouse,
			out var state);
		auction.SetupGet(x => x.FrameworkItemType).Returns("AuctionHouse");
		auction.SetupGet(x => x.EmploymentHostName).Returns("central auction");
		auction.SetupGet(x => x.EconomicZone).Returns(zone.Object);
		auction.SetupGet(x => x.AuctionHouseCell).Returns(auctionCell);
		auction.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		       .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));

		var activeAsset = Item(901, "a silver vase", prototype);
		activeAsset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		var unclaimedAsset = Item(902, "a jade bowl", prototype);
		unclaimedAsset.SetupGet(x => x.FrameworkItemType).Returns("GameItem");
		var activeLot = new AuctionItem
		{
			Asset = activeAsset.Object,
			Seller = auction.Object,
			PayoutTarget = auction.Object,
			MinimumPrice = 100.0M,
			BuyoutPrice = 150.0M,
			ListingDateTime = MudDateTime.Never,
			FinishingDateTime = MudDateTime.Never
		};
		var unclaimedLot = new UnclaimedAuctionItem
		{
			AuctionItem = new AuctionItem
			{
				Asset = unclaimedAsset.Object,
				Seller = auction.Object,
				PayoutTarget = auction.Object,
				MinimumPrice = 100.0M,
				ListingDateTime = MudDateTime.Never,
				FinishingDateTime = MudDateTime.Never
			}
		};
		auction.SetupGet(x => x.ActiveAuctionItems).Returns([activeLot]);
		auction.SetupGet(x => x.UnclaimedItems).Returns([unclaimedLot]);

		var auctions = new All<IAuctionHouse>();
		auctions.Add(auction.Object);
		gameworld.SetupGet(x => x.AuctionHouses).Returns(auctions);

		var manager = Character(90, "Manager", gameworld: gameworld.Object, location: auctionCell).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageStockRules |
			EmploymentAuthority.SettleHostAccounts |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, auction.Object, "Auction Work", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, auction.Object,
			new StringStack("auctionlist 900 reserve 100 buyout 150"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, auction.Object,
			new StringStack("auctionsettle lot 901"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, auction.Object,
			new StringStack("auctionclaim 902"), out message), message);

		var rendered = authoring.RenderDraft(manager, auction.Object);
		StringAssert.Contains(rendered, "list");
		StringAssert.Contains(rendered, "auction lot");
		StringAssert.Contains(rendered, "settle auction lot");
		StringAssert.Contains(rendered, "claim auction lot");
		StringAssert.Contains(rendered, "auctionlist");
		StringAssert.Contains(rendered, "auctionsettle");
		StringAssert.Contains(rendered, "auctionclaim");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesArenaEventAdministrationActions()
	{
		var currency = Currency();
		var gameworld = Gameworld();
		var arena = EmploymentHostMock<ICombatArena>(95, "central arena", EmploymentHostType.Arena,
			out var state);
		arena.SetupGet(x => x.FrameworkItemType).Returns("CombatArena");
		arena.SetupGet(x => x.EmploymentHostName).Returns("central arena");
		arena.SetupGet(x => x.Currency).Returns(currency.Object);
		arena.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));

		var eventType = new Mock<IArenaEventType>();
		eventType.SetupGet(x => x.Id).Returns(950);
		eventType.SetupGet(x => x.Name).Returns("Trial Bout");
		eventType.SetupGet(x => x.Arena).Returns(arena.Object);
		arena.SetupGet(x => x.EventTypes).Returns([eventType.Object]);
		arena.Setup(x => x.IsReadyToHost(eventType.Object)).Returns((true, string.Empty));

		var arenaEvent = new Mock<IArenaEvent>();
		arenaEvent.SetupGet(x => x.Id).Returns(951);
		arenaEvent.SetupGet(x => x.Name).Returns("Trial Bout #1");
		arenaEvent.SetupGet(x => x.Arena).Returns(arena.Object);
		arenaEvent.SetupGet(x => x.State).Returns(ArenaEventState.Scheduled);
		arena.SetupGet(x => x.ActiveEvents).Returns([arenaEvent.Object]);

		var manager = Character(95, "Manager", gameworld: gameworld.Object).Object;
		Mock.Get(manager.Account).SetupGet(x => x.TimeZone).Returns(TimeZoneInfo.Utc);
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, arena.Object, "Arena Work", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, arena.Object,
			new StringStack("arenaevent create 950 at 2099-01-01 10:00"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, arena.Object,
			new StringStack("arenaevent phase 951 preparing"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, arena.Object,
			new StringStack("arenaevent abort 951 rain delay"), out message), message);

		var rendered = authoring.RenderDraft(manager, arena.Object);
		StringAssert.Contains(rendered, "create arena event");
		StringAssert.Contains(rendered, "move arena event");
		StringAssert.Contains(rendered, "abort arena event");
		StringAssert.Contains(rendered, "arenaevent");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesBankAdministrationActions()
	{
		var currency = Currency();
		var gameworld = Gameworld();
		var branchA = Cell(701, "north branch").Object;
		var branchB = Cell(702, "south branch").Object;
		var cells = new All<ICell>();
		cells.Add(branchA);
		cells.Add(branchB);
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		var bank = EmploymentHostMock<IBank>(96, "central bank", EmploymentHostType.Bank,
			out var state);
		bank.SetupGet(x => x.FrameworkItemType).Returns("Bank");
		bank.SetupGet(x => x.EmploymentHostName).Returns("central bank");
		bank.SetupGet(x => x.Code).Returns("TST");
		bank.SetupGet(x => x.PrimaryCurrency).Returns(currency.Object);
		bank.SetupGet(x => x.CurrencyReserves).Returns(new DecimalCounter<ICurrency> { [currency.Object] = 100.0M });
		bank.SetupGet(x => x.BankAccounts).Returns([]);
		bank.SetupGet(x => x.BranchLocations).Returns([branchA, branchB]);
		bank.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));

		var manager = Character(96, "Manager", gameworld: gameworld.Object, location: branchA).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash |
			EmploymentAuthority.UseStoreAccount |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, bank.Object, "Bank Work", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin reserve audit"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin reserve deposit 12"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin reserve withdraw 3"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin account credit 123 7 for fee reversal"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin account status 123 suspended fraud review"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin account close 123 customer request"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin branch post here teller roster posted"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, bank.Object,
			new StringStack("bankadmin branch courier 701 to 702 reserve bag moved"), out message), message);

		var rendered = authoring.RenderDraft(manager, bank.Object);
		StringAssert.Contains(rendered, "audit native reserves");
		StringAssert.Contains(rendered, "deposit");
		StringAssert.Contains(rendered, "withdraw");
		StringAssert.Contains(rendered, "credit bank account");
		StringAssert.Contains(rendered, "set bank account");
		StringAssert.Contains(rendered, "close bank account");
		StringAssert.Contains(rendered, "bank branch post");
		StringAssert.Contains(rendered, "bank branch courier");
		StringAssert.Contains(rendered, "bankadmin");
	}
	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesStableAndHotelAdministrationActions()
	{
		var currency = Currency();
		var stableCell = Cell(801, "stable stall").Object;
		var roomCell = Cell(981, "blue room").Object;
		var stable = EmploymentHostMock<IStable>(97, "east stable", EmploymentHostType.Stable,
			out var stableState);
		stable.SetupGet(x => x.FrameworkItemType).Returns("Stable");
		stable.SetupGet(x => x.EmploymentHostName).Returns("east stable");
		stable.SetupGet(x => x.Currency).Returns(currency.Object);
		stable.SetupGet(x => x.Location).Returns(stableCell);
		stable.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => stableState.HasAuthority(actor, authority));
		var stay = new Mock<IStableStay>();
		stay.SetupGet(x => x.Id).Returns(100);
		stay.SetupGet(x => x.Stable).Returns(stable.Object);
		stay.SetupGet(x => x.IsActive).Returns(true);
		stable.SetupGet(x => x.ActiveStays).Returns([stay.Object]);
		stable.SetupGet(x => x.Stays).Returns([stay.Object]);
		var account = new Mock<IStableAccount>();
		account.SetupGet(x => x.Id).Returns(101);
		account.SetupGet(x => x.Name).Returns("account1");
		account.SetupGet(x => x.AccountName).Returns("account1");
		account.SetupGet(x => x.Stable).Returns(stable.Object);
		account.SetupGet(x => x.Currency).Returns(currency.Object);
		stable.SetupGet(x => x.StableAccounts).Returns([account.Object]);
		stable.Setup(x => x.AccountByName(It.IsAny<string>()))
		      .Returns<string>(text => text.EqualTo("account1") ? account.Object : null!);

		var stableManager = Character(97, "Stable Manager", location: stableCell).Object;
		stableState.Hire(stableManager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.UseStoreAccount), null);
		var stableAuthoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(stableAuthoring.TryStartDraft(stableManager, stable.Object, "Stable Work", out var message), message);
		Assert.IsTrue(stableAuthoring.TryAddStep(stableManager, stable.Object,
			new StringStack("stableadmin care inspect 100 morning check"), out message), message);
		Assert.IsTrue(stableAuthoring.TryAddStep(stableManager, stable.Object,
			new StringStack("stableadmin care feed 100 hay ration topped"), out message), message);
		Assert.IsTrue(stableAuthoring.TryAddStep(stableManager, stable.Object,
			new StringStack("stableadmin fees all daily fee sweep"), out message), message);
		Assert.IsTrue(stableAuthoring.TryAddStep(stableManager, stable.Object,
			new StringStack("stableadmin stay 100 ticket verified"), out message), message);
		Assert.IsTrue(stableAuthoring.TryAddStep(stableManager, stable.Object,
			new StringStack("stableadmin account account1 monthly reconciliation"), out message), message);

		var stableRendered = stableAuthoring.RenderDraft(stableManager, stable.Object);
		StringAssert.Contains(stableRendered, "stable inspection");
		StringAssert.Contains(stableRendered, "stable feeding");
		StringAssert.Contains(stableRendered, "assess fees");
		StringAssert.Contains(stableRendered, "stable account reconciliation");
		StringAssert.Contains(stableRendered, "stableadmin");

		var property = new Mock<IProperty>();
		property.SetupGet(x => x.Id).Returns(980);
		property.SetupGet(x => x.Name).Returns("harbour house");
		var hotel = EmploymentHostMock<IHotel>(98, "harbour house hotel", EmploymentHostType.Hotel,
			out var hotelState);
		hotel.SetupGet(x => x.FrameworkItemType).Returns("Hotel");
		hotel.SetupGet(x => x.EmploymentHostName).Returns("harbour house hotel");
		hotel.SetupGet(x => x.Currency).Returns(currency.Object);
		hotel.SetupGet(x => x.Property).Returns(property.Object);
		hotel.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		     .Returns((ICharacter actor, EmploymentAuthority authority) => hotelState.HasAuthority(actor, authority));
		property.SetupGet(x => x.Hotel).Returns(hotel.Object);
		var room = new Mock<IHotelRoom>();
		room.SetupGet(x => x.Name).Returns("blue room");
		room.SetupGet(x => x.Cell).Returns(roomCell);
		hotel.SetupGet(x => x.Rooms).Returns([room.Object]);
		var lost = new Mock<IHotelLostProperty>();
		lost.SetupGet(x => x.BundleId).Returns(990);
		lost.SetupGet(x => x.Description).Returns("forgotten valise");
		lost.SetupGet(x => x.Status).Returns(HotelLostPropertyStatus.Held);
		var balance = new Mock<IHotelPatronBalance>();
		balance.SetupGet(x => x.PatronId).Returns(901);
		balance.SetupGet(x => x.Balance).Returns(12.5M);
		property.SetupGet(x => x.HotelLostProperties).Returns([lost.Object]);
		property.SetupGet(x => x.HotelPatronBalances).Returns([balance.Object]);

		var hotelManager = Character(98, "Hotel Manager", location: roomCell).Object;
		hotelState.Hire(hotelManager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.UseStoreAccount), null);
		var hotelAuthoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(hotelAuthoring.TryStartDraft(hotelManager, hotel.Object, "Hotel Work", out message), message);
		Assert.IsTrue(hotelAuthoring.TryAddStep(hotelManager, hotel.Object,
			new StringStack("hoteladmin room inspect here linen checked"), out message), message);
		Assert.IsTrue(hotelAuthoring.TryAddStep(hotelManager, hotel.Object,
			new StringStack("hoteladmin room ready 981 guest ready"), out message), message);
		Assert.IsTrue(hotelAuthoring.TryAddStep(hotelManager, hotel.Object,
			new StringStack("hoteladmin lost check retention sweep"), out message), message);
		Assert.IsTrue(hotelAuthoring.TryAddStep(hotelManager, hotel.Object,
			new StringStack("hoteladmin lost audit 1 owner matched"), out message), message);
		Assert.IsTrue(hotelAuthoring.TryAddStep(hotelManager, hotel.Object,
			new StringStack("hoteladmin balance 901 balance reviewed"), out message), message);

		var hotelRendered = hotelAuthoring.RenderDraft(hotelManager, hotel.Object);
		StringAssert.Contains(hotelRendered, "hotel room inspection");
		StringAssert.Contains(hotelRendered, "hotel room readiness");
		StringAssert.Contains(hotelRendered, "lost-property expiry");
		StringAssert.Contains(hotelRendered, "lost-property audit");
		StringAssert.Contains(hotelRendered, "patron-balance audit");
		StringAssert.Contains(hotelRendered, "hoteladmin");
	}
	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesCashReconciliationActions()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var manager = Character(82, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.WithdrawBusinessCash), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, shop.Object, "Cash Check", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object,
			new StringStack("cashreconcile closing count"), out message), message);

		var rendered = authoring.RenderDraft(manager, shop.Object);
		StringAssert.Contains(rendered, "reconcile shop cash against expected float");
		StringAssert.Contains(rendered, "DepositBusinessCash");
		StringAssert.Contains(rendered, "WithdrawBusinessCash");
	}
	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesShopStocktakeActions()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(201);
		merchandise.SetupGet(x => x.Name).Returns("linen bundle");
		merchandise.SetupGet(x => x.ListDescription).Returns("linen bundle");
		merchandise.SetupGet(x => x.MerchandiseType).Returns(MerchandiseType.Item);
		merchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		shop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);

		var manager = Character(80, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageStockRules), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, shop.Object, "Stocktake", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object, new StringStack("stocktake all"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, shop.Object, new StringStack("stocktake merch linen bundle"), out message), message);

		var rendered = authoring.RenderDraft(manager, shop.Object);
		StringAssert.Contains(rendered, "stocktake all shop merchandise");
		StringAssert.Contains(rendered, "stocktake merchandise");
		StringAssert.Contains(rendered, "ManageStockRules");
	}
	[TestMethod]
	public void EmploymentCommandService_TaskDraftRejectsDeprecatedPriceMarketActions()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var manager = Character(79, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.AdjustPrices), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, shop.Object, "Prices", out var message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, shop.Object,
			new StringStack("price market host category food supply 10%"), out message));
		StringAssert.Contains(message, "deprecated");
	}

	[TestMethod]
	public void EmploymentCommandService_PriceMerchandiseActionMutatesMerchandiseAndRecordsAudit()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var manager = Character(70, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AdjustPrices), null);
		var basePrice = 10.0M;
		var merchandise = new Mock<IMerchandise>();
		var reason = string.Empty;
		merchandise.SetupGet(x => x.Id).Returns(200);
		merchandise.SetupGet(x => x.Name).Returns("apple pie");
		merchandise.SetupGet(x => x.ListDescription).Returns("apple pie");
		merchandise.SetupGet(x => x.BasePrice).Returns(() => basePrice);
		merchandise.SetupGet(x => x.Shop).Returns(shop.Object);
		merchandise.Setup(x => x.CanSetBasePrice(It.IsAny<decimal>(), out reason)).Returns(true);
		merchandise.Setup(x => x.SetBasePrice(It.IsAny<decimal>(), It.IsAny<ICharacter>()))
		           .Callback<decimal, ICharacter>((amount, _) => basePrice = amount);
		shop.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);
		var step = new PriceChangeActionStep("200", new MoneyAmount(currency.Object, 12.5M));
		var context = new EmploymentTaskContext(shop.Object);

		Assert.IsTrue(step.CanExecute(context, manager, out var executeReason), executeReason);
		var result = step.Execute(context, manager);

		Assert.IsTrue(result.Success, result.Message);
		Assert.AreEqual(12.5M, basePrice);
		merchandise.Verify(x => x.SetBasePrice(12.5M, manager), Times.Once);
		Assert.IsTrue(shop.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("base price", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void EmploymentCommandService_DeprecatedPriceMarketActionFailsClosed()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var manager = Character(71, "Manager").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AdjustPrices), null);
		var step = new DeprecatedMarketPriceChangeActionStep("Employment market-price actions are deprecated.");
		var context = new EmploymentTaskContext(shop.Object);

		Assert.IsFalse(step.CanExecute(context, manager, out var reason));
		StringAssert.Contains(reason, "deprecated");
		var result = step.Execute(context, manager);

		Assert.IsFalse(result.Success);
		Assert.IsFalse(result.Completed);
		Assert.IsTrue(shop.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("deprecated", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void EmploymentCommandService_ShopDealActionsCreateAndCancelNativeDeals()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var gameworld = Gameworld();
		var saveManager = new Mock<ISaveManager>();
		var deals = new List<IShopDeal>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		shop.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		shop.SetupGet(x => x.Deals).Returns(() => deals);
		shop.Setup(x => x.AddDeal(It.IsAny<IShopDeal>())).Callback<IShopDeal>(deals.Add);
		shop.Setup(x => x.RemoveDeal(It.IsAny<IShopDeal>())).Callback<IShopDeal>(deal => deals.Remove(deal));
		var manager = Character(77, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AdjustPrices), null);
		var context = new EmploymentTaskContext(shop.Object);
		var create = new ShopDealAdministrationActionStep(
			"summer sale",
			ShopDealType.Sale,
			ShopDealTargetType.AllMerchandise,
			null,
			-0.10M,
			0,
			ShopDealApplicability.Sell,
			null,
			true,
			MudDateTime.Never);

		Assert.IsTrue(create.CanExecute(context, manager, out var reason), reason);
		var createResult = create.Execute(context, manager);

		Assert.IsTrue(createResult.Success, createResult.Message);
		Assert.AreEqual(1, deals.Count);
		Assert.AreEqual("summer sale", deals.Single().Name);
		Assert.AreEqual(ShopDealType.Sale, deals.Single().DealType);
		Assert.AreEqual(ShopDealTargetType.AllMerchandise, deals.Single().TargetType);
		Assert.AreEqual(-0.10M, deals.Single().PriceAdjustmentPercentage);

		var modify = new ShopDealAdministrationActionStep(
			"summer sale",
			string.Empty,
			ShopDealType.Volume,
			ShopDealTargetType.AllMerchandise,
			null,
			-0.20M,
			3,
			ShopDealApplicability.Sell,
			null,
			true,
			MudDateTime.Never);
		Assert.IsTrue(modify.CanExecute(context, manager, out reason), reason);
		var modifyResult = modify.Execute(context, manager);

		Assert.IsTrue(modifyResult.Success, modifyResult.Message);
		Assert.AreEqual(1, deals.Count);
		Assert.AreEqual(ShopDealType.Volume, deals.Single().DealType);
		Assert.AreEqual(3, deals.Single().MinimumQuantity);
		Assert.AreEqual(-0.20M, deals.Single().PriceAdjustmentPercentage);

		var cancel = new ShopDealAdministrationActionStep("summer sale");
		Assert.IsTrue(cancel.CanExecute(context, manager, out reason), reason);
		var cancelResult = cancel.Execute(context, manager);

		Assert.IsTrue(cancelResult.Success, cancelResult.Message);
		Assert.AreEqual(0, deals.Count);
		Assert.IsTrue(shop.Object.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.AuditActionRecorded &&
			x.Description.Contains("shop deal", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void EmploymentCommandService_ScheduledShopDealActionsCreateTemporarySaleAndVolumeDeal()
	{
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		var gameworld = Gameworld();
		var saveManager = new Mock<ISaveManager>();
		var deals = new List<IShopDeal>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		shop.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		shop.SetupGet(x => x.Deals).Returns(() => deals);
		shop.Setup(x => x.AddDeal(It.IsAny<IShopDeal>())).Callback<IShopDeal>(deals.Add);
		var manager = Character(78, "Manager", gameworld: gameworld.Object).Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules | EmploymentAuthority.AdjustPrices), null);
		var plan = new EmploymentActionPlan([
			new ShopDealAdministrationActionStep(
				"scheduled sale",
				ShopDealType.Sale,
				ShopDealTargetType.AllMerchandise,
				null,
				-0.10M,
				0,
				ShopDealApplicability.Sell,
				null,
				true,
				MudDateTime.Never),
			new ShopDealAdministrationActionStep(
				"scheduled volume",
				ShopDealType.Volume,
				ShopDealTargetType.AllMerchandise,
				null,
				-0.20M,
				3,
				ShopDealApplicability.Sell,
				null,
				true,
				MudDateTime.Never)
		]);
		state.TaskBoard.CreateScheduledRule("scheduled deals", "scheduled-deals",
			[new ManualOrderCondition("sale-now")], plan, TimeSpan.Zero, manager);
		var context = new EmploymentTaskContext(shop.Object);
		context.SetManualOrder("sale-now", true);
		var spawned = state.TaskBoard.EvaluateScheduledRules(context, DateTimeOffset.UtcNow);
		var task = spawned.Single();
		var dispatcher = new EmploymentTaskDispatcher();
		((EmploymentActiveTask)task).Assign(manager);
		var first = dispatcher.AdvanceTask(task, context);
		var second = dispatcher.AdvanceTask(task, context);

		Assert.IsTrue(first.Success, first.Message);
		Assert.IsTrue(second.Success, second.Message);
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(2, deals.Count);
		Assert.IsTrue(deals.Any(x => x.Name.EqualTo("scheduled sale") && x.DealType == ShopDealType.Sale));
		Assert.IsTrue(deals.Any(x => x.Name.EqualTo("scheduled volume") &&
			x.DealType == ShopDealType.Volume && x.MinimumQuantity == 3));
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesJobOpeningAdministrationActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(72, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateJobOpenings |
			EmploymentAuthority.ModifyJobOpenings |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.ManageCraftRules), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Openings", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("jobopening create role courier positions 2 pay fixed 12.50 hourly authority default requires capability CanDeliverItems"),
			out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"jobopening close #{opening.Id} seasonal hiring complete"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"jobopening modify #{opening.Id} role crafter positions 2 pay fixed 15 hourly schedule between 09:00 17:00 duration fixed 1:00:00 authority craft,delivery requires skill crafting 25"),
			out message), message);

		var rendered = authoring.RenderDraft(manager, host);
		StringAssert.Contains(rendered, "create");
		StringAssert.Contains(rendered, "close job opening");
		StringAssert.Contains(rendered, "modify job opening");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesScheduledRuleAdministrationActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(73, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var rule = host.TaskBoard.CreateScheduledRule("restock watch", "restock-watch",
			[new ManualOrderCondition("restock-now")],
			new EmploymentActionPlan([new BoardPostActionStep("Restock", "Restock now.")]),
			TimeSpan.Zero,
			manager);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Rule Admin", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("rule pause #1 pause for stocktake"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("rule resume restock"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"rule evaluate {rule.Id:D} manual restock-now"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("rule cancel restock"), out message), message);
		Assert.IsTrue(authoring.TryFinaliseDraft(manager, host, out var task, out message), message);

		Assert.IsNotNull(task);
		Assert.AreEqual(4, task!.ActionPlan.Steps.Count);
		Assert.IsTrue(task.ActionPlan.Steps.All(x => x is ScheduledRuleAdministrationActionStep));
		Assert.IsTrue(task.ActionPlan.RequiredAuthority.Contains(EmploymentAuthority.ModifyScheduledRules));
		var rendered = new EmploymentTaskAuthoringService().RenderAvailableActions(manager, "rule");
		StringAssert.Contains(rendered, "pause|resume|cancel");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesActiveTaskAdministrationActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(74, "Manager").Object;
		var worker = Character(75, "Worker").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CancelTasks |
			EmploymentAuthority.HireEmployees), null);
		host.Hire(worker, Offer(currency.Object, EmploymentRole.Employee,
			EmploymentAuthority.AssignTasks), manager);
		var retryTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("blocked report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "blocked work")]), manager);
		retryTask.MarkStep(0, EmploymentActionStepStatus.Blocked,
			new EmploymentActionStepOperationalState(FailureDiagnostic: "blocked"));
		retryTask.Block("blocked");
		var requeueTask = (EmploymentActiveTask)host.TaskBoard.CreateActiveTask("assigned report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "assigned work")]), manager);
		requeueTask.Assign(worker);
		var assignTask = host.TaskBoard.CreateActiveTask("pending report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "pending work")]), manager);
		var cancelTask = host.TaskBoard.CreateActiveTask("cancel report",
			new EmploymentActionPlan([new CataloguedActionShellStep("report", "cancel work")]), manager);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Task Admin", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("admintask retry blocked retry after manager review"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("admintask requeue assigned release for another worker"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"admintask assign {assignTask.Id:D} to #{worker.Id} assign to worker"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"admintask cancel {cancelTask.Id:D} no longer needed"), out message), message);
		Assert.IsTrue(authoring.TryFinaliseDraft(manager, host, out var task, out message), message);

		Assert.IsNotNull(task);
		Assert.AreEqual(4, task!.ActionPlan.Steps.Count);
		Assert.IsTrue(task.ActionPlan.Steps.All(x => x is ActiveTaskAdministrationActionStep));
		Assert.IsTrue(task.ActionPlan.RequiredAuthority.Contains(EmploymentAuthority.AssignTasks));
		Assert.IsTrue(task.ActionPlan.RequiredAuthority.Contains(EmploymentAuthority.CancelTasks));
		var rendered = new EmploymentTaskAuthoringService().RenderAvailableActions(manager, "admintask");
		StringAssert.Contains(rendered, "retry|requeue|cancel");
		StringAssert.Contains(rendered, "assign");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDraftParsesManagerGoalAdministrationActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(76, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainMinimumPhysicalCashFloat,
			EmploymentAuthority.PostToHostBoard,
			new ManagerGoalConfiguration("Daily manager goal", new EmploymentActionPlan([
				new BoardPostActionStep("Goal", "Do the manager goal work.")
			])),
			1,
			TimeSpan.Zero), manager);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Goal Admin", out var message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("goal evaluate #1"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("goal reactivate cashfloatlow review complete"), out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack($"goal cancel {goal.Id} no longer needed"), out message), message);
		Assert.IsTrue(authoring.TryFinaliseDraft(manager, host, out var task, out message), message);

		Assert.IsNotNull(task);
		Assert.AreEqual(3, task!.ActionPlan.Steps.Count);
		Assert.IsTrue(task.ActionPlan.Steps.All(x => x is ManagerGoalAdministrationActionStep));
		Assert.IsTrue(task.ActionPlan.RequiredAuthority.Contains(EmploymentAuthority.ModifyManagerGoals));
		var rendered = new EmploymentTaskAuthoringService().RenderAvailableActions(manager, "goal");
		StringAssert.Contains(rendered, "evaluate|cancel|reactivate");
	}

	[TestMethod]
	public void EmploymentCommandService_JobOpeningMarketPayRequiresMinimumEffectivePay()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(78, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks |
			EmploymentAuthority.CreateJobOpenings |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Openings", out var message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host,
			new StringStack("jobopening create role courier pay market multiplier 1.2 hourly"),
			out message));
		StringAssert.Contains(message, "minimum effective pay");

		Assert.IsTrue(authoring.TryAddStep(manager, host,
			new StringStack("jobopening create role courier pay market multiplier 1.2 hourly min 12.50"),
			out message), message);
	}

	[TestMethod]
	public void EmploymentCommandService_JobOpeningActionsCreateCloseAndModify()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(73, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateJobOpenings |
			EmploymentAuthority.ModifyJobOpenings |
			EmploymentAuthority.ManageDeliveryRoutes |
			EmploymentAuthority.ManageCraftRules), null);
		var createStep = new JobOpeningAdministrationActionStep(new JobOpeningDefinition(
			EmploymentRole.Courier,
			JobRequirementSet.None,
			Pay(currency.Object, 12.5M),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			2,
			true,
			new PaymentMethod(PaymentMethodKind.Cash),
			new EmploymentAuthoritySet(EmploymentAuthority.ManageDeliveryRoutes)));
		var context = new EmploymentTaskContext(host);

		var createResult = createStep.Execute(context, manager);

		Assert.IsTrue(createResult.Success, createResult.Message);
		var opening = host.JobOpenings.Single();
		Assert.AreEqual(EmploymentRole.Courier, opening.Role);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.JobOpeningCreated));

		var modifyDefinition = new JobOpeningDefinition(
			EmploymentRole.Crafter,
			new JobRequirementSet(
				[new SkillRequirement("crafting", 25.0)],
				[new KnowledgeRequirement("kiln lore")],
				[new AICapabilityRequirement(EmploymentAICapability.CanCraft)],
				[new TagRequirement("workshop")]),
			Pay(currency.Object, 15.0M),
			new WorkSchedule("Between 09:00 and 17:00", TimeSpan.FromHours(9), TimeSpan.FromHours(17)),
			new EmploymentDuration(EmploymentDurationType.FixedTerm, TimeSpan.FromDays(30)),
			3,
			false,
			new PaymentMethod(PaymentMethodKind.Cash),
			new EmploymentAuthoritySet(EmploymentAuthority.ManageCraftRules | EmploymentAuthority.ManageDeliveryRoutes));
		var modifyStep = new JobOpeningAdministrationActionStep(
			JobOpeningAdministrationActionKind.Modify,
			opening.Id,
			modifyDefinition);

		var modifyResult = modifyStep.Execute(context, manager);

		Assert.IsTrue(modifyResult.Success, modifyResult.Message);
		Assert.AreEqual(EmploymentRole.Crafter, opening.Role);
		Assert.AreEqual(3, opening.MaxPositions);
		Assert.IsFalse(opening.NpcApplicationsOnly);
		Assert.AreEqual(EmploymentDurationType.FixedTerm, opening.Duration.DurationType);
		Assert.AreEqual(1, opening.Requirements.Skills.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.JobOpeningModified));

		var closeStep = new JobOpeningAdministrationActionStep(
			JobOpeningAdministrationActionKind.Close,
			opening.Id,
			reason: "filled");
		var closeResult = closeStep.Execute(context, manager);
		var repeatClose = closeStep.Execute(context, manager);

		Assert.IsTrue(closeResult.Success, closeResult.Message);
		Assert.AreEqual(JobOpeningStatus.Closed, opening.Status);
		Assert.IsFalse(repeatClose.Success);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x => x.EntryType == EmploymentRegisterEntryType.JobOpeningClosed));
	}

	[TestMethod]
	public void EmploymentCommandService_JobOpeningModifyRejectsCapacityAndAuthorityEscalation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(74, "Manager").Object;
		var first = Character(75, "First").Object;
		var second = Character(76, "Second").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.ModifyJobOpenings |
			EmploymentAuthority.HireEmployees |
			EmploymentAuthority.ManageDeliveryRoutes), null);
		var opening = host.Employment.CreateJobOpening(Opening(currency.Object) with { MaxPositions = 2 }, null);
		var firstApplication = host.Employment.Apply(opening, Profile(first, PaymentMethodKind.Cash));
		var secondApplication = host.Employment.Apply(opening, Profile(second, PaymentMethodKind.Cash));
		host.Employment.AcceptApplication(firstApplication, manager);
		host.Employment.AcceptApplication(secondApplication, manager);
		var context = new EmploymentTaskContext(host);
		var reduceStep = new JobOpeningAdministrationActionStep(
			JobOpeningAdministrationActionKind.Modify,
			opening.Id,
			Opening(currency.Object) with { MaxPositions = 1 });

		var reduceResult = reduceStep.Execute(context, manager);

		Assert.IsFalse(reduceResult.Success);
		StringAssert.Contains(reduceResult.Message, "accepted application");

		var escalateStep = new JobOpeningAdministrationActionStep(
			JobOpeningAdministrationActionKind.Modify,
			opening.Id,
			Opening(currency.Object) with
			{
				MaxPositions = 2,
				Authority = new EmploymentAuthoritySet(EmploymentAuthority.AdjustPrices)
			});
		var escalateResult = escalateStep.Execute(context, manager);

		Assert.IsFalse(escalateResult.Success);
		StringAssert.Contains(escalateResult.Message, "delegate authority");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskAuthoringRejectsDeferredCatalogueActions()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(69, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);
		var authoring = new EmploymentTaskAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Deferred", out var message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host, new StringStack("transfer till bank 5"), out message));

		StringAssert.Contains(message, "deferred");
		StringAssert.Contains(message, "transfer");
	}

	[TestMethod]
	public void EmploymentCommandService_OneShotCatalogueShellFinalisesWithExecutablePlanningSteps()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(72, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageStockRules), null);
		var authoring = new EmploymentTaskAuthoringService();

		var created = authoring.TryCreateOneShotTask(
			manager,
			host,
			new StringStack("Audit report shelves checked then reserve feed crates"),
			out var task,
			out var message);

		Assert.IsTrue(created, message);
		Assert.IsNotNull(task);
		Assert.AreEqual(2, task.ActionPlan.Steps.Count);
		Assert.IsTrue(task.ActionPlan.Steps.All(x => x is CataloguedActionShellStep));
		StringAssert.Contains(message, "Executable");
	}

	[TestMethod]
	public void EmploymentCommandService_OneShotTaskCreateFinalisesOrderedSteps()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.Tags).Returns(Tags(Tag(1, "apple").Object, Tag(2, "display").Object));
		var manager = Character(70, "Manager", gameworld: gameworld.Object).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		var authoring = new EmploymentTaskAuthoringService();

		var created = authoring.TryCreateOneShotTask(
			manager,
			host,
			new StringStack("Restock gettag 2 &apple from here then deliver to here containertag &display"),
			out var task,
			out var message);

		Assert.IsTrue(created, message);
		Assert.IsNotNull(task);
		Assert.AreEqual("Restock", task.Name);
		Assert.AreEqual(2, task.ActionPlan.Steps.Count);
		Assert.IsInstanceOfType(task.ActionPlan.Steps[0], typeof(GetItemsByTagActionStep));
		Assert.IsInstanceOfType(task.ActionPlan.Steps[1], typeof(DeliverItemsActionStep));
		Assert.AreEqual(task, host.TaskBoard.ActiveTasks.Single());
		StringAssert.Contains(message, "tagged");
		StringAssert.Contains(message, "apple");
		StringAssert.Contains(message, "display");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskCreated));
	}

	[TestMethod]
	public void EmploymentCommandService_TasksCancelCancelsActiveTaskAndRecordsRegister()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(71, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes | EmploymentAuthority.CancelTasks), null);
		var task = host.TaskBoard.CreateActiveTask(
			"Deliver apples",
			new EmploymentActionPlan(new[] { new DeliverItemsActionStep(manager.Location) }),
			manager);
		var service = new EmploymentCommandService();

		var cancelled = service.TryCancelTask(manager, host, "1", "Wrong stock.", out var message);

		Assert.IsTrue(cancelled, message);
		Assert.AreEqual(EmploymentTaskStatus.Cancelled, task.Status);
		StringAssert.Contains(task.BlockedReason, "Wrong stock");
		StringAssert.Contains(message, "Deliver apples");
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskCancelled));
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
			gameworld.SetupGet(x => x.Tags).Returns(Tags(Tag(1, "apple").Object, Tag(2, "display").Object));
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
			service.ExecuteForHost(manager, host, new StringStack("tasks step gettag 2 &apple from here"));
			service.ExecuteForHost(manager, host, new StringStack("tasks step deliver to here containertag &display"));
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

	[TestMethod]
	public void EmploymentCommandService_TaskDetailShowsSpecificActionSteps()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var stockroom = Cell(660, "stockroom").Object;
		var shopfront = Cell(661, "shopfront").Object;
		var gameworld = Gameworld();
		gameworld.SetupGet(x => x.ItemProtos).Returns(ItemProtos(ItemProto(500, "a pair of leather gloves").Object).Object);
		var manager = Character(65, "Manager", gameworld: gameworld.Object, location: stockroom).Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		host.TaskBoard.CreateActiveTask(
			"Restock gloves",
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new GetItemsByIdActionStep(2, [500], [stockroom]),
				new DeliverItemsActionStep(shopfront, null, "display")
			}),
			manager);
		var service = new EmploymentCommandService();

		var rendered = service.RenderTaskDetail(manager, host, "1");

		StringAssert.Contains(rendered, "Restock gloves");
		StringAssert.Contains(rendered, "go to");
		StringAssert.Contains(rendered, "stockroom");
		StringAssert.Contains(rendered, "collect");
		StringAssert.Contains(rendered, "a pair of leather gloves");
		StringAssert.Contains(rendered, "prototype #");
		StringAssert.Contains(rendered, "shopfront");
		StringAssert.Contains(rendered, "deliver all carried task items");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDetailShowsOperationalStepState()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(65, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.AssignTasks), null);
		var task = host.TaskBoard.CreateActiveTask(
			"Report shelves",
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new CataloguedActionShellStep("report", "shelves counted")
			}),
			manager);
		var dispatcher = new EmploymentTaskDispatcher();
		var context = new EmploymentTaskContext(host);
		Assert.IsTrue(dispatcher.TryAssignTask(task,
			[Profile(manager, PaymentMethodKind.Cash)],
			context,
			out var reason), reason);
		Assert.IsTrue(dispatcher.AdvanceTask(task, context).Success);
		var service = new EmploymentCommandService();

		var rendered = service.RenderTaskDetail(manager, host, "1");

		StringAssert.Contains(rendered, "Report shelves");
		StringAssert.Contains(rendered, "State:");
		StringAssert.Contains(rendered, "Report: shelves counted");
	}

	[TestMethod]
	public void EmploymentCommandService_TaskDiagnosticsExplainsWhyPendingTaskIsNotClaimed()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(67, "Manager").Object;
		var employee = Character(68, "Employee").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.AssignTasks | EmploymentAuthority.ManageDeliveryRoutes), null);
		host.Hire(employee, Offer(currency.Object, EmploymentRole.Employee), null);
		host.TaskBoard.CreateActiveTask(
			"Collect crates",
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new GetItemsByIdActionStep(1, [123], [employee.Location])
			}),
			manager);
		var service = new EmploymentCommandService();

		var rendered = service.RenderTaskDiagnostics(manager, host);

		StringAssert.Contains(rendered, "Collect crates");
		StringAssert.Contains(rendered, "ManageDeliveryRoutes");
		StringAssert.Contains(rendered, "CanDeliverItems");
		StringAssert.Contains(rendered, "lacks");
		StringAssert.Contains(rendered, "has no EmploymentWorkerAI");
	}

	[TestMethod]
	public void EmploymentConditionCatalog_ResolvesDefinitionsAliasesAndCategories()
	{
		var service = new EmploymentScheduledRuleAuthoringService();
		var actor = Character(80, "Manager").Object;

		Assert.AreSame(EmploymentConditionCatalog.Get("manual"), EmploymentConditionCatalog.Get("trigger"));
		Assert.AreSame(EmploymentConditionCatalog.Get("stock"), EmploymentConditionCatalog.Get("merchandise"));
		Assert.IsTrue(EmploymentConditionCatalog.ForCategory("finance").Any(x => x.Key == "account"));
		Assert.IsTrue(EmploymentConditionCatalog.ForCategory("inventory").Any(x => x.Key == "item"));
		Assert.IsTrue(EmploymentConditionCatalog.ForCategory("inventory").Any(x => x.Key == "commodity"));
		Assert.IsTrue(EmploymentConditionCatalog.ForCategory("environment").Any(x => x.Key == "weather"));

		var rendered = service.RenderAvailableConditions(actor, "all");

		StringAssert.Contains(rendered, "manual");
		StringAssert.Contains(rendered, "time");
		StringAssert.Contains(rendered, "item");
		StringAssert.Contains(rendered, "commodity");
		StringAssert.Contains(rendered, "stock");
		StringAssert.Contains(rendered, "account");
		StringAssert.Contains(rendered, "shopaccount");
		StringAssert.Contains(rendered, "float");
		StringAssert.Contains(rendered, "weather");
		StringAssert.Contains(rendered, "tasks rule condition");
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_ParsesConditionFamiliesAndOvernightWindows()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(81, "Manager").Object;
		var service = new EmploymentScheduledRuleAuthoringService();

		Assert.IsTrue(service.TryParseCondition(manager, host, new StringStack("time 23:00 to 02:00"),
			out var timeCondition, out var message), message);
		Assert.IsInstanceOfType(timeCondition, typeof(TimeWindowCondition));
		Assert.IsTrue(timeCondition.IsSatisfied(new EmploymentTaskContext(host),
			new DateTimeOffset(2026, 5, 30, 1, 0, 0, TimeSpan.Zero), out _));
		Assert.IsFalse(timeCondition.IsSatisfied(new EmploymentTaskContext(host),
			new DateTimeOffset(2026, 5, 30, 12, 0, 0, TimeSpan.Zero), out _));
		Assert.IsTrue(service.TryParseCondition(manager, host, new StringStack("between 09:00 and 17:00"),
			out var betweenCondition, out message), message);
		Assert.IsInstanceOfType(betweenCondition, typeof(TimeWindowCondition));

		Assert.IsTrue(service.TryParseCondition(manager, host, new StringStack("stock key butter below 5"),
			out var stockCondition, out message), message);
		var context = new EmploymentTaskContext(host);
		context.SetStockLevel("butter", 2);
		Assert.AreEqual(EmploymentAuthority.ManageStockRules, stockCondition.RequiredAuthority.Authorities);
		Assert.IsTrue(stockCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out _));

		Assert.IsTrue(service.TryParseCondition(manager, host, new StringStack("account key operating atleast 12.50"),
			out var accountCondition, out message), message);
		context.SetAccountBalance("operating", 15.0M);
		Assert.AreEqual(EmploymentAuthority.CreateScheduledRules, accountCondition.RequiredAuthority.Authorities);
		Assert.IsTrue(accountCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out _));
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_ParsesNewConditionFamilies()
	{
		var currency = Currency();
		var stockroom = Cell(7000, "stockroom").Object;
		var crateProto = ItemProto(360, "crate").Object;
		var nailsTag = Tag(1000, "Nails").Object;
		var gameworld = Gameworld();
		var cells = new All<ICell>();
		cells.Add(stockroom);
		var itemProtos = ItemProtos(crateProto);
		var shops = new All<IShop>();
		gameworld.SetupGet(x => x.Cells).Returns(cells);
		gameworld.SetupGet(x => x.ItemProtos).Returns(itemProtos.Object);
		gameworld.SetupGet(x => x.Shops).Returns(shops);
		gameworld.SetupGet(x => x.Tags).Returns(Tags(nailsTag));
		var manager = Character(85, "Manager", gameworld: gameworld.Object, location: stockroom).Object;
		var host = EmploymentHostMock<IShop>(1, "market shop", EmploymentHostType.Shop, out _);
		host.SetupGet(x => x.Currency).Returns(currency.Object);
		host.SetupGet(x => x.CurrentLocations).Returns([stockroom]);

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService().TryParseCondition(manager, host.Object,
			new StringStack("item 360 in here below 3"), out var itemCondition, out var message), message);
		Assert.IsInstanceOfType(itemCondition, typeof(ItemThresholdCondition));

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService().TryParseCondition(manager, host.Object,
			new StringStack("commodity Iron|Nails|grade=refined in here atleast 12.50"),
			out var commodityCondition, out message), message);
		Assert.IsInstanceOfType(commodityCondition, typeof(CommodityThresholdCondition));

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService().TryParseCondition(manager, host.Object,
			new StringStack("float all below 12.50"), out var floatCondition, out message), message);
		Assert.IsInstanceOfType(floatCondition, typeof(ShopFloatThresholdCondition));

		var otherShop = EmploymentHostMock<IShop>(2, "other shop", EmploymentHostType.Shop, out _);
		otherShop.SetupGet(x => x.Currency).Returns(currency.Object);
		var account = new Mock<ILineOfCreditAccount>();
		account.SetupGet(x => x.Id).Returns(20);
		account.SetupGet(x => x.Name).Returns("builder tab");
		account.SetupGet(x => x.AccountName).Returns("builder tab");
		account.SetupGet(x => x.Currency).Returns(currency.Object);
		otherShop.SetupGet(x => x.LineOfCreditAccounts).Returns([account.Object]);
		shops.Add(otherShop.Object);

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService().TryParseCondition(manager, host.Object,
			new StringStack("shopaccount other shop account builder tab owing more than 50"), out var owingCondition,
			out message), message);
		Assert.IsInstanceOfType(owingCondition, typeof(ShopAccountOwingCondition));

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService().TryParseCondition(manager, host.Object,
			new StringStack("weather wind gale force begins"), out var weatherCondition, out message), message);
		Assert.IsInstanceOfType(weatherCondition, typeof(WeatherLevelCondition));
	}

	[TestMethod]
	public void EmploymentScheduledRuleConditions_EvaluateInventoryFloatAccountsAndWeather()
	{
		var currency = Currency();
		var stockroom = Cell(7100, "stockroom");
		var controller = new Mock<IWeatherController>();
		var weather = new Mock<IWeatherEvent>();
		weather.SetupGet(x => x.Precipitation).Returns(PrecipitationLevel.Rain);
		weather.SetupGet(x => x.Wind).Returns(WindLevel.GaleWind);
		controller.SetupGet(x => x.CurrentWeatherEvent).Returns(weather.Object);
		controller.SetupGet(x => x.ConsecutiveUnchangedPeriods).Returns(0);
		stockroom.SetupGet(x => x.WeatherController).Returns(controller.Object);
		var sockProto = ItemProto(26, "socks").Object;
		var ironProto = ItemProto(27, "iron nails").Object;
		var crateProto = ItemProto(360, "crate").Object;
		var sock1 = Item(7101, "a pair of socks", sockProto, [stockroom.Object]);
		var sock2 = Item(7102, "another pair of socks", sockProto, [stockroom.Object]);
		var ironOne = Item(7106, "some iron nails", ironProto, [stockroom.Object]);
		var ironTwo = Item(7107, "more iron nails", ironProto, [stockroom.Object]);
		var crate = Item(7103, "a crate", crateProto, [stockroom.Object]);
		sock1.SetupGet(x => x.DeepItems).Returns([sock1.Object]);
		sock2.SetupGet(x => x.DeepItems).Returns([sock2.Object]);
		ironOne.SetupGet(x => x.DeepItems).Returns([ironOne.Object]);
		ironTwo.SetupGet(x => x.DeepItems).Returns([ironTwo.Object]);
		crate.SetupGet(x => x.DeepItems).Returns([crate.Object, sock1.Object, sock2.Object, ironOne.Object, ironTwo.Object]);
		var container = new Mock<IContainer>();
		container.SetupGet(x => x.Contents).Returns([sock1.Object, sock2.Object, ironOne.Object, ironTwo.Object]);
		crate.Setup(x => x.GetItemType<IContainer>()).Returns(container.Object);

		var pileProto = ItemProto(500, "currency pile").Object;
		var pileItem = Item(7104, "some coins", pileProto, [stockroom.Object]);
		var till = Item(7105, "a cash register", crateProto, [stockroom.Object]);
		var tillContainer = new Mock<IContainer>();
		tillContainer.SetupGet(x => x.Contents).Returns([pileItem.Object]);
		till.Setup(x => x.GetItemType<IContainer>()).Returns(tillContainer.Object);
		var pile = new Mock<ICurrencyPile>();
		pile.SetupGet(x => x.Currency).Returns(currency.Object);
		pile.SetupGet(x => x.TotalValue).Returns(8.0M);
		pileItem.Setup(x => x.IsItemType<ICurrencyPile>()).Returns(true);
		pileItem.Setup(x => x.GetItemType<ICurrencyPile>()).Returns(pile.Object);
		pileItem.Setup(x => x.GetItemType<IContainer>()).Returns((IContainer)null!);

		var account = new Mock<ILineOfCreditAccount>();
		account.SetupGet(x => x.Id).Returns(90);
		account.SetupGet(x => x.Name).Returns("builder tab");
		account.SetupGet(x => x.AccountName).Returns("builder tab");
		account.SetupGet(x => x.OutstandingBalance).Returns(75.0M);
		account.SetupGet(x => x.Currency).Returns(currency.Object);

		var host = EmploymentHostMock<IPermanentShop>(10, "market shop", EmploymentHostType.Shop, out _);
		host.SetupGet(x => x.Currency).Returns(currency.Object);
		host.SetupGet(x => x.CurrentLocations).Returns([stockroom.Object]);
		host.SetupGet(x => x.AllShopCells).Returns([stockroom.Object]);
		host.SetupGet(x => x.TillItems).Returns([till.Object]);
		host.SetupGet(x => x.LineOfCreditAccounts).Returns([account.Object]);
		var context = new EmploymentTaskContext(host.Object);
		context.SetAvailableItems(stockroom.Object, [crate.Object, till.Object]);
		context.SetCommodityWeight(ironOne.Object, "Iron", 6.0, "Nails",
			new Dictionary<string, string> { ["grade"] = "refined" });
		context.SetCommodityWeight(ironTwo.Object, "Iron", 7.0, "Nails",
			new Dictionary<string, string> { ["grade"] = "refined" });

		var itemCondition = new ItemThresholdCondition(
			ItemThresholdCondition.CreateKey(EmploymentItemSelector.ForPrototype(26), stockroom.Object.Id,
				EmploymentItemSelector.ForPrototype(360)),
			3,
			true);
		Assert.IsTrue(itemCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out var reason), reason);

		var commodityCondition = new CommodityThresholdCondition(
			CommodityThresholdCondition.CreateKey("Iron", "Nails",
				new Dictionary<string, string> { ["grade"] = "refined" },
				stockroom.Object.Id,
				EmploymentItemSelector.ForPrototype(360)),
			12.0M,
			false);
		Assert.IsTrue(commodityCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out reason), reason);

		var accountCondition = new ShopAccountOwingCondition(
			ShopAccountOwingCondition.CreateKey(host.Object, account.Object),
			50.0M,
			true);
		Assert.IsTrue(accountCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out reason), reason);

		var floatCondition = new ShopFloatThresholdCondition(ShopFloatThresholdCondition.CreateKey(null), 10.0M, true);
		Assert.IsTrue(floatCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out reason), reason);

		var weatherCondition = new WeatherLevelCondition(WeatherLevelCondition.CreatePrecipitationKey("rain"));
		Assert.IsTrue(weatherCondition.IsSatisfied(context, DateTimeOffset.UtcNow, out reason), reason);
	}

	[TestMethod]
	public void EmploymentCommandService_ScheduledRuleDraftFinaliseCreatesInspectableRule()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(82, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Night notice"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft cooldown 1h"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual night-stock"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule step board Night Stock = Check the shelves."));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft finalise"));

		var rule = host.TaskBoard.ScheduledRules.Single();
		Assert.AreEqual("Night notice", rule.Name);
		Assert.AreEqual(1, rule.Conditions.Count);
		Assert.AreEqual(1, rule.ActionPlan.Steps.Count);
		Assert.IsInstanceOfType(rule.Conditions.Single(), typeof(ManualOrderCondition));
		Assert.IsInstanceOfType(rule.ActionPlan.Steps.Single(), typeof(BoardPostActionStep));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ScheduledRuleCreated));

		var detail = service.RenderTaskDetail(manager, host, "1");
		StringAssert.Contains(detail, "manual trigger");
		StringAssert.Contains(detail, "Night Stock");
		StringAssert.Contains(detail, "Last Spawned");
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_RejectsOverflowCooldown()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(182, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules), null);
		var service = new EmploymentScheduledRuleAuthoringService();

		Assert.IsTrue(service.TryStartDraft(manager, host, "Overflow", out var message), message);
		Assert.IsFalse(service.TrySetDraftCooldown(manager, host, "9999999999999999999999999999d", out message));
		StringAssert.Contains(message, "positive duration");
		Assert.AreEqual(0, host.TaskBoard.ScheduledRules.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_ScheduledRuleOneShotEvaluateAndCancel()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(83, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host,
			new StringStack("tasks rule create Restock cooldown 1h when manual restock-now do board Restock = Count the shelves."));

		Assert.AreEqual(1, host.TaskBoard.ScheduledRules.Count);
		var diagnosticBefore = new EmploymentScheduledRuleAuthoringService()
			.RenderDiagnostics(manager, host, "1", null);
		StringAssert.Contains(diagnosticBefore, "would not spawn");

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService()
			.TryEvaluate(manager, host, "1", "restock-now", out var message), message);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		StringAssert.Contains(message, "spawn");

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService()
			.TryCancelRule(manager, host, "1", "No longer needed.", out message), message);
		Assert.AreEqual(0, host.TaskBoard.ScheduledRules.Count);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ScheduledRuleCancelled));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalTypesDescribeInitialArchitecture()
	{
		var actor = Character(280, "Manager").Object;
		var authoring = new EmploymentManagerGoalAuthoringService();

		var allTypes = authoring.RenderGoalTypes(actor, "all");

		StringAssert.Contains(allTypes, "cashfloatlow");
		StringAssert.Contains(allTypes, "cashfloathigh");
		StringAssert.Contains(allTypes, "taxes");
		StringAssert.Contains(allTypes, "accounts");
		StringAssert.Contains(allTypes, "craftstock");
		StringAssert.Contains(allTypes, "craftmaterials");
		StringAssert.Contains(allTypes, "payroll");
		StringAssert.Contains(allTypes, "bankbalancelow");
		StringAssert.Contains(allTypes, "bankbalancehigh");
		StringAssert.Contains(allTypes, "pricemargin");
		StringAssert.Contains(allTypes, "staffing");

		var highFloat = authoring.RenderGoalTypes(actor, "cashfloathigh");
		StringAssert.Contains(highFloat, "maximum");
		StringAssert.Contains(highFloat, "MaintainMaximumPhysicalCashFloat");

		var taxes = authoring.RenderGoalTypes(actor, "taxes");
		StringAssert.Contains(taxes, "paytax");
		StringAssert.Contains(taxes, "PayTaxes");

		var payroll = authoring.RenderGoalTypes(actor, "payroll");
		StringAssert.Contains(payroll, "payroll settle");
		StringAssert.Contains(payroll, "KeepEmploymentPayrollCurrent");
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalDraftFinaliseCreatesInspectableGoal()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(281, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PayTaxes |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("goals draft new taxes Tax payment watch"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft cadence 30m"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft budget 20"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft risk tasks 2"));
		service.ExecuteForHost(manager, host, new StringStack("goals condition tax owing above 10"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft expression #1"));
		service.ExecuteForHost(manager, host, new StringStack("goals step board Taxes = Pay taxes before deadline."));
		service.ExecuteForHost(manager, host, new StringStack("goals draft finalise"));

		var goal = host.ManagerGoalBoard.Goals.Single();
		Assert.AreEqual(ManagerGoalType.PayTaxes, goal.GoalType);
		Assert.AreEqual("Tax payment watch", goal.Configuration.Description);
		Assert.AreEqual(TimeSpan.FromMinutes(30), goal.EvaluationCadence);
		Assert.AreEqual(20.0M, goal.Policy.BudgetLimits.Single().Amount);
		Assert.AreEqual(2, goal.Policy.RiskLimits.MaximumActiveTasks);
		Assert.IsTrue(goal.RequiredAuthority.Contains(EmploymentAuthority.PayTaxes));
		Assert.IsTrue(goal.RequiredAuthority.Contains(EmploymentAuthority.PostToHostBoard));
		Assert.IsInstanceOfType(goal.Configuration.Conditions!.Single(), typeof(TaxOwingCondition));
		Assert.IsNotNull(goal.Configuration.ConditionExpression);
		Assert.IsInstanceOfType(goal.Configuration.ActionPlan!.Steps.Single(), typeof(BoardPostActionStep));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ManagerGoalCreated));

		var detail = new EmploymentManagerGoalAuthoringService().RenderGoalDetail(manager, host, "#1");
		StringAssert.Contains(detail, "Tax payment watch");
		StringAssert.Contains(detail, "supported host taxes owing");
		StringAssert.Contains(detail, "Condition Expression");
		StringAssert.Contains(detail, "#1");
		StringAssert.Contains(detail, "Budget Limits");
		StringAssert.Contains(detail, "Risk Limits");
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalDraftPolicyRejectsExceededBudgetAndStepLimit()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(287, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.DepositBusinessCash |
			EmploymentAuthority.PostToHostBoard), null);
		var authoring = new EmploymentManagerGoalAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "cashfloatlow", "Budgeted float", out var message), message);
		Assert.IsTrue(authoring.TrySetDraftAuthority(manager, host,
			new StringStack("depositbusinesscash posttohostboard"), out message), message);
		Assert.IsTrue(authoring.TrySetDraftBudget(manager, host, "10", out message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host, new StringStack("bankdeposit 12"), out message));
		StringAssert.Contains(message, "budget");

		Assert.IsTrue(authoring.TrySetDraftBudget(manager, host, "20", out message), message);
		Assert.IsTrue(authoring.TryAddStep(manager, host, new StringStack("bankdeposit 12"), out message), message);
		Assert.IsTrue(authoring.TrySetDraftRiskLimit(manager, host, new StringStack("steps 1"), out message), message);
		Assert.IsFalse(authoring.TryAddStep(manager, host, new StringStack("board Notice = Check stock."), out message));
		StringAssert.Contains(message, "risk limit");

		Assert.IsTrue(authoring.TryFinaliseDraft(manager, host, out var goal, out message), message);
		Assert.IsNotNull(goal);
		Assert.AreEqual(20.0M, goal!.Policy.BudgetLimits.Single().Amount);
		Assert.AreEqual(1, goal.Policy.RiskLimits.MaximumActionSteps);
		Assert.AreEqual(1, goal.Configuration.ActionPlan!.Steps.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalDraftCopySupportsEditByDraftAndCancel()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(282, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("goals draft new cashfloatlow Cash float watch"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft authority posttohostboard"));
		service.ExecuteForHost(manager, host, new StringStack("goals step board Cash Float = Count the till."));
		service.ExecuteForHost(manager, host, new StringStack("goals draft finalise"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft copy #1 Updated float watch"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft priority 3"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft finalise"));
		service.ExecuteForHost(manager, host, new StringStack("goals cancel #1 Replaced by updated goal."));

		Assert.AreEqual(2, host.ManagerGoalBoard.Goals.Count);
		var original = host.ManagerGoalBoard.Goals.Single(x => x.Id == 1);
		var replacement = host.ManagerGoalBoard.Goals.Single(x => x.Id == 2);
		Assert.AreEqual(ManagerGoalStatus.Cancelled, original.Status);
		Assert.AreEqual("Updated float watch", replacement.Configuration.Description);
		Assert.AreEqual(3, replacement.Priority);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ManagerGoalCancelled));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalEvaluateSpawnsTaskThroughGoalBoard()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(283, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("goals draft new cashfloatlow Daily float notice"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft authority posttohostboard"));
		service.ExecuteForHost(manager, host, new StringStack("goals step board Float = Check physical cash float."));
		service.ExecuteForHost(manager, host, new StringStack("goals draft finalise"));
		service.ExecuteForHost(manager, host, new StringStack("goals evaluate"));

		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		Assert.AreEqual("Daily float notice", host.TaskBoard.ActiveTasks.Single().Name);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ManagerGoalEvaluated));
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalEvaluateSkipsDuplicateActiveTask()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(286, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.PostToHostBoard), null);
		var goal = host.ManagerGoalBoard.CreateGoal(new ManagerGoalDefinition(
			ManagerGoalType.MaintainMinimumPhysicalCashFloat,
			EmploymentAuthoritySet.Empty,
			new ManagerGoalConfiguration(
				"Daily float notice",
				new EmploymentActionPlan([
					new BoardPostActionStep("Float", "Check physical cash float.")
				]),
				[]),
			1,
			TimeSpan.FromSeconds(1)), manager);
		var context = new EmploymentTaskContext(host);
		var now = DateTimeOffset.UtcNow;

		var first = host.ManagerGoalBoard.EvaluateGoals(context, now);
		var second = host.ManagerGoalBoard.EvaluateGoals(context, now.AddSeconds(2));

		Assert.AreEqual(1, first.Count);
		Assert.AreEqual(0, second.Count);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		Assert.AreEqual($"manager-goal:{goal.CorrelationId:D}", host.TaskBoard.ActiveTasks.Single().IdempotencyKey);
		StringAssert.Contains(goal.LastEvaluationResult!, "already exists");
	}

	[TestMethod]
	public void EmploymentCommandService_ManagerGoalPayrollCreatesExecutableSettlementTask()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		var currency = Currency();
		var (shop, state) = ShopHost(1, "market shop", currency.Object);
		IEmploymentHost host = shop.Object;
		var manager = Character(284, "Manager").Object;
		var employee = Character(285, "Employee").Object;
		state.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateManagerGoals |
			EmploymentAuthority.ModifyManagerGoals |
			EmploymentAuthority.ManagePayroll), null);
		var contract = state.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			new CompensationTerms(
				new MoneyAmount(currency.Object, 10.0M),
				null,
				PayCadence.Daily,
				new MoneyAmount(currency.Object, 10.0M),
				PaymentSource.HostCash),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.Payroll.EvaluatePayroll(contract.StartedAt.AddDays(2).AddMinutes(1));
		VirtualCashLedger.Credit(shop.Object, currency.Object,
			host.Payroll.OutstandingLiabilities.Sum(x => x.Amount.Amount), null, shop.Object, "Seed",
			"Seed payroll balance");
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("goals draft new payroll Payroll watch"));
		service.ExecuteForHost(manager, host, new StringStack("goals condition payroll outstanding above 0"));
		service.ExecuteForHost(manager, host, new StringStack("goals step payroll settle all routine wage settlement"));
		service.ExecuteForHost(manager, host, new StringStack("goals draft finalise"));
		service.ExecuteForHost(manager, host, new StringStack("goals evaluate"));

		var task = host.TaskBoard.ActiveTasks.Single();
		Assert.AreEqual("Payroll watch", task.Name);
		var step = task.ActionPlan.Steps.Single();
		Assert.IsInstanceOfType(step, typeof(PayrollSettlementActionStep));
		var payrollStep = (PayrollSettlementActionStep)step;
		var context = new EmploymentTaskContext(host);
		Assert.IsTrue(payrollStep.CanExecute(context, manager, out var reason), reason);
		var result = payrollStep.Execute(context, manager);
		Assert.IsTrue(result.Success, result.Message);
		Assert.IsTrue(host.Payroll.Payables.All(x => x.Status == EmploymentPayableStatus.ReadyToClaim));
		Assert.IsTrue(host.BusinessLedger.Entries.Any(x => x.EntryType == EmploymentLedgerEntryType.Wage));
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.WageSettled));
	}

	[TestMethod]
	public void EmploymentCommandService_AdminCanCreateAndCancelRulesAndTasksWithoutDelegation()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var admin = Character(84, "Admin", administrator: true).Object;
		var service = new EmploymentCommandService();

		service.ExecuteForHost(admin, host,
			new StringStack("tasks rule create AdminRestock cooldown 1h when stock key butter below 5 do board Restock = Count the shelves."));

		Assert.AreEqual(1, host.TaskBoard.ScheduledRules.Count);
		Assert.IsInstanceOfType(host.TaskBoard.ScheduledRules.Single().Conditions.Single(), typeof(StockThresholdCondition));

		Assert.IsTrue(new EmploymentScheduledRuleAuthoringService()
			.TryCancelRule(admin, host, "1", "Admin cleanup.", out var message), message);
		Assert.AreEqual(0, host.TaskBoard.ScheduledRules.Count);

		var task = host.TaskBoard.CreateActiveTask("Admin notice",
			new EmploymentActionPlan([new BoardPostActionStep("Notice", "Check the shelves.")]), admin);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
		Assert.IsTrue(host.TaskBoard.CancelActiveTask(task, admin, "Admin cleanup."));
		Assert.AreEqual(EmploymentTaskStatus.Cancelled, task.Status);
	}

	[TestMethod]
	public void EmploymentCommandService_OneShotRuleKeepsBetweenAndConditionTogether()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(85, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host,
			new StringStack("tasks rule create DayRestock cooldown 1h when between 09:00 and 17:00 and manual restock-now do board Restock = Count the shelves."));

		var rule = host.TaskBoard.ScheduledRules.Single();
		Assert.AreEqual(2, rule.Conditions.Count);
		Assert.IsInstanceOfType(rule.Conditions.ElementAt(0), typeof(TimeWindowCondition));
		Assert.IsInstanceOfType(rule.Conditions.ElementAt(1), typeof(ManualOrderCondition));
	}

	[TestMethod]
	public void EmploymentCommandService_ScheduledRulePauseResumeAndCopyDraft()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(86, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();
		var authoring = new EmploymentScheduledRuleAuthoringService();

		service.ExecuteForHost(manager, host,
			new StringStack("tasks rule create Restock cooldown 1h when manual restock-now do board Restock = Count the shelves."));
		var rule = host.TaskBoard.ScheduledRules.Single();

		service.ExecuteForHost(manager, host, new StringStack("tasks rule pause 1 testing pause"));
		Assert.AreEqual(EmploymentScheduledRuleStatus.Paused, rule.Status);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ScheduledRulePaused));
		Assert.IsTrue(authoring.TryEvaluate(manager, host, "1", "restock-now", out var message), message);
		StringAssert.Contains(message, "Rule is paused");
		Assert.AreEqual(0, host.TaskBoard.ActiveTasks.Count);
		StringAssert.Contains(service.RenderTaskDetail(manager, host, "1"), "Paused");

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft copy 1 Restock updated"));
		var draft = authoring.RenderDraft(manager, host);
		StringAssert.Contains(draft, "Restock updated");
		StringAssert.Contains(draft, "manual trigger");
		StringAssert.Contains(draft, "host staff communication board");

		service.ExecuteForHost(manager, host, new StringStack("tasks rule resume 1 testing resume"));
		Assert.AreEqual(EmploymentScheduledRuleStatus.Active, rule.Status);
		Assert.IsTrue(host.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ScheduledRuleResumed));
		Assert.IsTrue(authoring.TryEvaluate(manager, host, "1", "restock-now", out message), message);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_RejectsMissingConditionAuthority()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(87, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules), null);
		var authoring = new EmploymentScheduledRuleAuthoringService();

		Assert.IsTrue(authoring.TryStartDraft(manager, host, "Restock butter", out var message), message);
		Assert.IsFalse(authoring.TryAddCondition(manager, host, new StringStack("stock key butter below 5"), out message));
		StringAssert.Contains(message, "ManageStockRules");
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_DraftExpressionPredicateAndTemplateCommands_Work()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(88, "Manager").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();
		var authoring = new EmploymentScheduledRuleAuthoringService();

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Reusable Window"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual window-open"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft expression #1"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule predicate create window-open"));

		Assert.AreEqual(1, host.TaskBoard.ConditionPredicates.Count);
		StringAssert.Contains(authoring.RenderPredicates(manager, host, "1"), "window-open");

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Window Notice"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual override"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft expression @window-open or #1"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule step board Window = Open the shop."));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft key window-template-key"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule template save Window Template"));
		Assert.AreEqual(1, host.TaskBoard.ScheduledRuleTemplates.Count);
		Assert.AreEqual("window-template-key", host.TaskBoard.ScheduledRuleTemplates.Single().IdempotencyKeyPattern);
		StringAssert.Contains(authoring.RenderTemplates(manager, host, "1"), "Window Template");

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft discard"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule template draft 1 Window Live"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft finalise"));

		var rule = host.TaskBoard.ScheduledRules.Single();
		Assert.IsNotNull(rule.ConditionExpression);
		Assert.AreEqual("window-template-key", rule.IdempotencyKey);
		StringAssert.Contains(service.RenderTaskDetail(manager, host, "1"), "@window-open");
		Assert.IsTrue(authoring.TryEvaluate(manager, host, "1", "window-open", out var message), message);
		Assert.AreEqual(1, host.TaskBoard.ActiveTasks.Count);

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Bad Expression"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual bad"));
		Assert.IsFalse(authoring.TrySetDraftExpression(manager, host, "@missing", out message));
		StringAssert.Contains(message, "does not exist");

		Assert.IsTrue(authoring.TryCancelPredicate(manager, host, "1", "test cleanup", out message), message);
		Assert.AreEqual(0, host.TaskBoard.ConditionPredicates.Count);
		Assert.IsTrue(authoring.TryCancelTemplate(manager, host, "1", "test cleanup", out message), message);
		Assert.AreEqual(0, host.TaskBoard.ScheduledRuleTemplates.Count);
	}

	[TestMethod]
	public void EmploymentCommandService_ScheduledRulePredicateAndTemplateViewsRequireOperationalAccess()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(90, "Manager").Object;
		var outsider = Character(91, "Outsider").Object;
		host.Hire(manager, Offer(currency.Object, EmploymentRole.Manager,
			EmploymentAuthority.CreateScheduledRules |
			EmploymentAuthority.ModifyScheduledRules |
			EmploymentAuthority.PostToHostBoard), null);
		var service = new EmploymentCommandService();

		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Reusable Window"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual window-open"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft expression #1"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule predicate create window-open"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft new Window Notice"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule condition manual override"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft expression @window-open or #1"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule step board Window = Open the shop."));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule draft key window-template-key"));
		service.ExecuteForHost(manager, host, new StringStack("tasks rule template save Window Template"));

		var outsiderMessages = new List<string>();
		Mock.Get(outsider.OutputHandler)
		    .Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
		    .Callback<string, bool, bool>((text, _, _) => outsiderMessages.Add(text))
		    .Returns(true);

		service.ExecuteForHost(outsider, host, new StringStack("tasks rule predicates"));
		service.ExecuteForHost(outsider, host, new StringStack("tasks rule predicate show 1"));
		service.ExecuteForHost(outsider, host, new StringStack("tasks rule templates"));
		service.ExecuteForHost(outsider, host, new StringStack("tasks rule template show 1"));

		Assert.AreEqual(4, outsiderMessages.Count);
		Assert.IsTrue(outsiderMessages.All(x => x.Contains("not an employee", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(outsiderMessages.Any(x =>
			x.Contains("window-open", StringComparison.OrdinalIgnoreCase) ||
			x.Contains("window-template-key", StringComparison.OrdinalIgnoreCase) ||
			x.Contains("Window Template", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_MarketPriceCondition_ParsesAndEvaluates()
	{
		var currency = Currency();
		var manager = Character(89, "Manager").Object;
		var merchandise = new Mock<IMerchandise>();
		merchandise.SetupGet(x => x.Id).Returns(44);
		merchandise.SetupGet(x => x.Name).Returns("silk");
		merchandise.SetupGet(x => x.BasePrice).Returns(10.0M);
		merchandise.SetupGet(x => x.EffectivePrice).Returns(15.0M);
		var host = EmploymentHostMock<IShop>(1, "market shop", EmploymentHostType.Shop, out _);
		host.SetupGet(x => x.Currency).Returns(currency.Object);
		host.SetupGet(x => x.Merchandises).Returns([merchandise.Object]);

		var authoring = new EmploymentScheduledRuleAuthoringService();
		Assert.IsTrue(authoring.TryParseCondition(manager, host.Object,
			new StringStack("marketprice merch silk effective above 12.50"), out var condition, out var message),
			message);
		Assert.IsInstanceOfType(condition, typeof(MarketPriceCondition));
		Assert.IsTrue(condition.IsSatisfied(new EmploymentTaskContext(host.Object), DateTimeOffset.UtcNow,
			out var reason), reason);

		Assert.IsTrue(authoring.TryParseCondition(manager, host.Object,
			new StringStack("marketprice merch 44 base below 12.50"), out condition, out message), message);
		Assert.IsTrue(condition.IsSatisfied(new EmploymentTaskContext(host.Object), DateTimeOffset.UtcNow,
			out reason), reason);
	}

	[TestMethod]
	public void EmploymentScheduledRuleAuthoring_PayrollAndStaffingConditions_ParseAndEvaluate()
	{
		var currency = Currency();
		IEmploymentHost host = new TestEmploymentHost(1, "market shop", currency.Object);
		var manager = Character(90, "Manager").Object;
		var employee = Character(91, "Employee").Object;
		var contract = host.Hire(employee, new EmploymentOffer(
			EmploymentRole.Employee,
			new CompensationTerms(
				new MoneyAmount(currency.Object, 10.0M),
				null,
				PayCadence.Daily,
				new MoneyAmount(currency.Object, 10.0M),
				PaymentSource.HostCash),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty), null);
		host.Employment.CreateJobOpening(Opening(currency.Object), null);
		var authoring = new EmploymentScheduledRuleAuthoringService();
		var evaluationTime = contract.StartedAt.AddDays(1).AddMinutes(1);

		Assert.IsTrue(authoring.TryParseCondition(manager, host,
			new StringStack("payroll outstanding above 0"), out var condition, out var message), message);
		Assert.IsInstanceOfType(condition, typeof(PayrollLiabilityCondition));
		Assert.IsTrue(condition.IsSatisfied(new EmploymentTaskContext(host), evaluationTime, out var reason), reason);

		Assert.IsTrue(authoring.TryParseCondition(manager, host,
			new StringStack("staffing role employee combined below 3"), out condition, out message), message);
		Assert.IsInstanceOfType(condition, typeof(StaffingLevelCondition));
		Assert.IsTrue(condition.IsSatisfied(new EmploymentTaskContext(host), DateTimeOffset.UtcNow, out reason), reason);
	}

	private static Mock<T> HostMock<T>(long id, string name)
		where T : class, IEmploymentHost
	{
		var host = new Mock<T>();
		host.SetupGet(x => x.Id).Returns(id);
		host.SetupGet(x => x.Name).Returns(name);
		return host;
	}

	private static Mock<T> EmploymentHostMock<T>(long id, string name, EmploymentHostType hostType,
		out IEmploymentHostState state)
		where T : class, IEmploymentHost
	{
		var host = new Mock<T>();
		host.SetupGet(x => x.Id).Returns(id);
		host.SetupGet(x => x.Name).Returns(name);
		host.SetupGet(x => x.EmploymentHostType).Returns(hostType);
		host.SetupGet(x => x.Market).Returns((IMarket?)null);
		state = new EmploymentHostState(host.Object);
		host.SetupGet(x => x.Employment).Returns(state);
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
		currency.Setup(x => x.TryGetBaseCurrency(It.IsAny<string>(), out It.Ref<decimal>.IsAny))
		        .Returns(new TryGetBaseCurrencyCallback((string text, out decimal amount) =>
			        decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)));
		return currency;
	}

	private static (Mock<IShop> Shop, IEmploymentHostState State) ShopHost(long id, string name, ICurrency currency)
	{
		var shop = new Mock<IShop>();
		shop.SetupGet(x => x.Id).Returns(id);
		shop.SetupGet(x => x.Name).Returns(name);
		shop.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		shop.SetupGet(x => x.EmploymentHostName).Returns(name);
		shop.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Shop);
		shop.SetupGet(x => x.Market).Returns((IMarket?)null);
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.BankAccount).Returns((IBankAccount)null!);
		shop.SetupGet(x => x.CashBalance).Returns(() => VirtualCashLedger.Balance(shop.Object, currency));
		var state = new EmploymentHostState(shop.Object);
		shop.SetupGet(x => x.Employment).Returns(state);
		shop.SetupGet(x => x.BusinessLedger).Returns(state.BusinessLedger);
		shop.SetupGet(x => x.EmploymentRegister).Returns(state.EmploymentRegister);
		shop.SetupGet(x => x.TaskBoard).Returns(state.TaskBoard);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(state.ManagerGoalBoard);
		shop.SetupGet(x => x.Payroll).Returns(state.Payroll);
		shop.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		shop.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (shop, state);
	}

	private static void SetupPermanentShop(Mock<IPermanentShop> shop, ICurrency currency, ICell stockroom,
		IEnumerable<ICell> shopfronts, IEnumerable<IMerchandise> merchandises)
	{
		var shopfrontList = shopfronts.ToList();
		var merchandiseList = merchandises.ToList();
		shop.SetupGet(x => x.FrameworkItemType).Returns("PermanentShop");
		shop.SetupGet(x => x.EmploymentHostName).Returns(shop.Object.Name);
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.StockroomCell).Returns(stockroom);
		shop.SetupGet(x => x.ShopfrontCells).Returns(shopfrontList);
		shop.SetupGet(x => x.AllShopCells).Returns(() => shopfrontList.Concat(new[] { stockroom }).DistinctBy(x => x.Id).ToList());
		shop.SetupGet(x => x.DisplayContainers).Returns([]);
		shop.SetupGet(x => x.Merchandises).Returns(merchandiseList);
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

	private static Mock<IGameItemProto> ItemProto(long id, string shortDescription)
	{
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(id);
		proto.SetupGet(x => x.Name).Returns(shortDescription);
		proto.SetupGet(x => x.ShortDescription).Returns(shortDescription);
		return proto;
	}

	private static Mock<IGameItem> Item(long id, string name, IGameItemProto prototype, IEnumerable<ICell>? trueLocations = null)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Prototype).Returns(prototype);
		item.SetupGet(x => x.TrueLocations).Returns(trueLocations ?? []);
		item.Setup(x => x.HowSeen(
			        It.IsAny<IPerceiver>(),
			        It.IsAny<bool>(),
			        It.IsAny<DescriptionType>(),
			        It.IsAny<bool>(),
			        It.IsAny<PerceiveIgnoreFlags>()))
		    .Returns(name);
		return item;
	}

	private static Mock<IUneditableRevisableAll<IGameItemProto>> ItemProtos(params IGameItemProto[] prototypes)
	{
		var lookup = prototypes.ToDictionary(x => x.Id);
		var itemProtos = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		itemProtos.Setup(x => x.Get(It.IsAny<long>()))
		          .Returns((long id) => lookup.TryGetValue(id, out var prototype) ? prototype : null!);
		return itemProtos;
	}

	private static All<ITag> Tags(params ITag[] tags)
	{
		var all = new All<ITag>();
		foreach (var tag in tags)
		{
			all.Add(tag);
		}

		return all;
	}

	private static Mock<ITag> Tag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		tag.SetupGet(x => x.FullName).Returns(name);
		return tag;
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
		var account = new Mock<IAccount>();
		var effects = new List<IEffect>();
		account.SetupGet(x => x.Culture).Returns(CultureInfo.InvariantCulture);
		account.SetupGet(x => x.UseUnicode).Returns(false);
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		character.SetupGet(x => x.Gameworld).Returns(gameworld!);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(currentLocation);
		character.SetupGet(x => x.OutputHandler).Returns(output.Object);
		character.SetupGet(x => x.Account).Returns(account.Object);
		character.SetupGet(x => x.Effects).Returns(effects);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
		         .Callback<IEffect>(effect => effects.Add(effect));
		character.Setup(x => x.EffectsOfType<EmploymentTaskDraftEffect>(
					It.IsAny<Predicate<EmploymentTaskDraftEffect>?>()))
		         .Returns((Predicate<EmploymentTaskDraftEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentTaskDraftEffect>()
				         : effects.OfType<EmploymentTaskDraftEffect>().Where(x => predicate(x)));
		character.Setup(x => x.EffectsOfType<EmploymentScheduledRuleDraftEffect>(
					It.IsAny<Predicate<EmploymentScheduledRuleDraftEffect>?>()))
		         .Returns((Predicate<EmploymentScheduledRuleDraftEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentScheduledRuleDraftEffect>()
				         : effects.OfType<EmploymentScheduledRuleDraftEffect>().Where(x => predicate(x)));
		character.Setup(x => x.EffectsOfType<EmploymentManagerGoalDraftEffect>(
					It.IsAny<Predicate<EmploymentManagerGoalDraftEffect>?>()))
		         .Returns((Predicate<EmploymentManagerGoalDraftEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentManagerGoalDraftEffect>()
				         : effects.OfType<EmploymentManagerGoalDraftEffect>().Where(x => predicate(x)));
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
		character.Setup(x => x.RemoveAllEffects<EmploymentScheduledRuleDraftEffect>(
					It.IsAny<Predicate<EmploymentScheduledRuleDraftEffect>?>(),
					It.IsAny<bool>()))
		         .Returns((Predicate<EmploymentScheduledRuleDraftEffect>? predicate, bool _) =>
		         {
			         var removed = predicate is null
				         ? effects.OfType<EmploymentScheduledRuleDraftEffect>().ToList()
				         : effects.OfType<EmploymentScheduledRuleDraftEffect>().Where(x => predicate(x)).ToList();
			         foreach (var effect in removed)
			         {
				         effects.Remove(effect);
			         }

			         return removed.Any();
		         });
		character.Setup(x => x.RemoveAllEffects<EmploymentManagerGoalDraftEffect>(
					It.IsAny<Predicate<EmploymentManagerGoalDraftEffect>?>(),
					It.IsAny<bool>()))
		         .Returns((Predicate<EmploymentManagerGoalDraftEffect>? predicate, bool _) =>
		         {
			         var removed = predicate is null
				         ? effects.OfType<EmploymentManagerGoalDraftEffect>().ToList()
				         : effects.OfType<EmploymentManagerGoalDraftEffect>().Where(x => predicate(x)).ToList();
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

	private static string HelpConstant(Type type, string fieldName)
	{
		return (string)type.GetField(fieldName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!
		                   .GetRawConstantValue()!;
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
