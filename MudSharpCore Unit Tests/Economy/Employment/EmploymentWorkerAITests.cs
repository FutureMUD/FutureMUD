using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Arenas;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Community.Boards;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Economy.Employment;
using MudSharp.Economy.Property;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine;

#nullable enable

namespace MudSharp_Unit_Tests.Economy.Employment;

[TestClass]
[DoNotParallelize]
public class EmploymentWorkerAITests
{
	private delegate bool TryGetBaseCurrencyCallback(string pattern, out decimal amount);

	[TestMethod]
	public void EmploymentWorkerAI_LoadsAndSavesXmlConfiguration()
	{
		var currency = Currency();
		var gameworld = Gameworld(currencies: [currency.Object]);
		var ai = LoadAI(gameworld.Object, """
		                                  <Definition>
		                                    <Currency>1</Currency>
		                                    <ReservationWage>12.5</ReservationWage>
		                                    <MaxPathRange>25</MaxPathRange>
		                                    <SearchEnabled>false</SearchEnabled>
		                                    <TaskingEnabled>true</TaskingEnabled>
		                                    <SearchCadenceMinutes>3</SearchCadenceMinutes>
		                                    <HostTypeFilter>Bank</HostTypeFilter>
		                                    <AcceptedPaymentMethods>
		                                      <Method>Cash</Method>
		                                      <Method>EmployeeBankAccount</Method>
		                                    </AcceptedPaymentMethods>
		                                    <Capabilities>
		                                      <Capability>CanDeliverItems</Capability>
		                                      <Capability>CanUseBankAccount</Capability>
		                                    </Capabilities>
		                                  </Definition>
		                                  """);

		Assert.AreEqual(12.5M, ai.ReservationWage);
		Assert.AreSame(currency.Object, ai.Currency);
		Assert.AreEqual(25u, ai.MaxPathRange);
		Assert.IsFalse(ai.SearchEnabled);
		Assert.IsTrue(ai.TaskingEnabled);
		Assert.AreEqual(TimeSpan.FromMinutes(3), ai.SearchCadence);
		Assert.AreEqual(EmploymentHostType.Bank, ai.HostTypeFilter);
		Assert.IsTrue(ai.AcceptedPaymentMethods.Contains(PaymentMethodKind.EmployeeBankAccount));
		Assert.IsTrue(ai.Capabilities.Contains(EmploymentAICapability.CanUseBankAccount));

		var xml = SaveXml(ai);

		StringAssert.Contains(xml, "<Currency>1</Currency>");
		StringAssert.Contains(xml, "EmployeeBankAccount");
		StringAssert.Contains(xml, "CanUseBankAccount");
		StringAssert.Contains(xml, "<HostTypeFilter>Bank</HostTypeFilter>");
	}

	[TestMethod]
	public void EmploymentWorkerAI_BuilderCommandsValidateEnumPaymentAndCapabilityInput()
	{
		var currency = Currency();
		var gameworld = Gameworld(currencies: [currency.Object]);
		var ai = LoadAI(gameworld.Object);
		var actor = Character(1, "Builder", gameworld.Object, Cell(10, "workshop").Object).Object;

		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("currency test dollars")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("wage 15.5")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("payment employeebankaccount")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("capability canusebankaccount")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("host stable")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("range 12")));
		Assert.IsTrue(ai.BuildingCommand(actor, new StringStack("cadence 2")));
		Assert.IsFalse(ai.BuildingCommand(actor, new StringStack("capability notarealcapability")));

		Assert.AreEqual(15.5M, ai.ReservationWage);
		Assert.AreSame(currency.Object, ai.Currency);
		Assert.IsTrue(ai.AcceptedPaymentMethods.Contains(PaymentMethodKind.EmployeeBankAccount));
		Assert.IsTrue(ai.Capabilities.Contains(EmploymentAICapability.CanUseBankAccount));
		Assert.AreEqual(EmploymentHostType.Stable, ai.HostTypeFilter);
		Assert.AreEqual(12u, ai.MaxPathRange);
		Assert.AreEqual(TimeSpan.FromMinutes(2), ai.SearchCadence);
	}

	[TestMethod]
	public void EmploymentWorkerAI_NewBuilderDefaultsCurrencyAndClosesDoors()
	{
		var currency = Currency();
		var gameworld = Gameworld(currencies: [currency.Object]);

		var ai = CreateAI(gameworld.Object);

		Assert.AreSame(currency.Object, ai.Currency);
		Assert.IsTrue(ai.CloseDoorsBehind);
	}

	[TestMethod]
	public void EmploymentWorkerAI_UnemployedNpcAppliesForBestMatchingOpening()
	{
		var currency = Currency();
		var workplace = Cell(20, "shopfront");
		var lowHost = Shop(1, "low shop", currency.Object, workplace.Object);
		var highHost = Shop(2, "high shop", currency.Object, workplace.Object);
		lowHost.State.CreateJobOpening(Opening(currency.Object, 5.0M), null);
		highHost.State.CreateJobOpening(Opening(currency.Object, 20.0M), null);
		var gameworld = Gameworld(shops: [lowHost.Shop.Object, highHost.Shop.Object], currencies: [currency.Object]);
		var worker = Character(2, "Worker", gameworld.Object, workplace.Object).Object;
		var ai = LoadAI(gameworld.Object, """
		                                  <Definition>
		                                    <ReservationWage>10</ReservationWage>
		                                    <AcceptedPaymentMethods><Method>Cash</Method></AcceptedPaymentMethods>
		                                    <Capabilities><Capability>CanDeliverItems</Capability></Capabilities>
		                                  </Definition>
		                                  """);

		var acted = ai.HandleMinuteTick(worker);

		Assert.IsTrue(acted);
		Assert.AreEqual(0, lowHost.State.Applications.Count);
		Assert.AreEqual(1, highHost.State.Applications.Count);
		Assert.AreEqual(JobApplicationStatus.Pending, highHost.State.Applications.Single().Status);
	}

	[TestMethod]
	public void EmploymentWorkerAI_RejectedApplicationToFullOpeningCanApplyToSecondOpeningSameRole()
	{
		var currency = Currency();
		var workplace = Cell(21, "stable yard");
		var host = Stable(21, "busy stable", currency.Object, workplace.Object);
		var gameworld = Gameworld(stables: [host.Stable.Object], currencies: [currency.Object]);
		var worker = Character(21, "Amos", gameworld.Object, workplace.Object).Object;
		var hiredWorker = Character(22, "Stryder", gameworld.Object, workplace.Object).Object;
		var manager = Character(23, "Manager", gameworld.Object, workplace.Object).Object;
		host.State.Hire(manager, Offer(currency.Object, EmploymentRole.Manager, EmploymentAuthority.HireEmployees), null);
		var firstOpening = host.State.CreateJobOpening(Opening(currency.Object, 10.0M), null);
		var secondOpening = host.State.CreateJobOpening(Opening(currency.Object, 10.0M), null);
		var rejectedApplication = host.State.Apply(firstOpening, Profile(worker));
		var acceptedApplication = host.State.Apply(firstOpening, Profile(hiredWorker));
		host.State.AcceptApplication(acceptedApplication, manager);
		host.State.RejectApplication(rejectedApplication, manager, "Rejected by a manager.");
		var ai = LoadAI(gameworld.Object);

		var acted = ai.HandleMinuteTick(worker);

		Assert.IsTrue(acted);
		Assert.IsFalse(firstOpening.AcceptsApplications);
		Assert.IsTrue(secondOpening.AcceptsApplications);
		Assert.IsTrue(host.State.Applications.Any(x =>
			x.Candidate.Id == worker.Id &&
			x.Opening.Id == secondOpening.Id &&
			x.Status == JobApplicationStatus.Pending));
	}

	[TestMethod]
	public void EmploymentWorkerAI_EmployedNpcTicksTowardStableWhenIdle()
	{
		var currency = Currency();
		var current = Cell(40, "road");
		var workplace = Cell(41, "stable");
		var stable = Stable(40, "task stable", currency.Object, workplace.Object);
		var gameworld = Gameworld(stables: [stable.Stable.Object], currencies: [currency.Object]);
		var worker = Character(40, "Worker", gameworld.Object, current.Object).Object;
		stable.State.Hire(worker, Offer(currency.Object, EmploymentRole.Employee), null);
		var ai = LoadAI(gameworld.Object);

		var acted = ai.HandleMinuteTick(worker);

		Assert.IsTrue(acted);
	}

	[TestMethod]
	public void EmploymentWorkerAI_ClaimsAndAdvancesRetrievalDeliveryTaskWithoutBoardPosts()
	{
		var currency = Currency();
		var workplace = Cell(30, "stockroom");
		var item = Item(300, "apple");
		var cellItems = new List<IGameItem> { item.Object };
		workplace.SetupGet(x => x.GameItems).Returns(() => cellItems);
		workplace.Setup(x => x.Extract(It.IsAny<IGameItem>()))
		         .Callback<IGameItem>(_ => cellItems.Clear());
		workplace.Setup(x => x.Insert(It.IsAny<IGameItem>(), It.IsAny<bool>()))
		         .Callback<IGameItem, bool>((target, _) => cellItems.Add(target));
		var host = Shop(3, "task shop", currency.Object, workplace.Object);
		var gameworld = Gameworld(shops: [host.Shop.Object], currencies: [currency.Object]);
		var worker = Character(3, "Worker", gameworld.Object, workplace.Object).Object;
		host.State.Hire(worker, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.State.TaskBoard.CreateActiveTask(
			"Move apples",
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new GetItemsByIdActionStep(1, [item.Object.Id], [workplace.Object]),
				new DeliverItemsActionStep(workplace.Object)
			}),
			null);
		var ai = LoadAI(gameworld.Object);

		Assert.IsTrue(ai.HandleMinuteTick(worker));
		Assert.AreEqual(EmploymentTaskStatus.InProgress, task.Status);
		Assert.AreEqual(EmploymentActionStepStatus.Completed, task.StepStates[0]);
		Assert.AreEqual(0, cellItems.Count);

		Assert.IsTrue(ai.HandleMinuteTick(worker));
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(EmploymentActionStepStatus.Completed, task.StepStates[1]);
		Assert.AreEqual(1, cellItems.Count);
		Assert.IsTrue(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskAssigned));
		Assert.IsTrue(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskCompleted));
		Assert.IsFalse(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.BoardPostCreated));
		Assert.AreEqual(0, host.State.Board.Posts.Count());
	}

	[TestMethod]
	public void EmploymentWorkerAI_EvaluatesScheduledRulesBeforeClaimingTasks()
	{
		var currency = Currency();
		var workplace = Cell(32, "stockroom");
		var item = Item(302, "crate");
		var cellItems = new List<IGameItem> { item.Object };
		workplace.SetupGet(x => x.GameItems).Returns(() => cellItems);
		workplace.Setup(x => x.Extract(It.IsAny<IGameItem>()))
		         .Callback<IGameItem>(_ => cellItems.Clear());
		var host = Shop(32, "task shop", currency.Object, workplace.Object);
		var gameworld = Gameworld(shops: [host.Shop.Object], currencies: [currency.Object]);
		var worker = Character(32, "Worker", gameworld.Object, workplace.Object).Object;
		host.State.Hire(worker, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.ManageDeliveryRoutes), null);
		host.State.TaskBoard.CreateScheduledRule(
			"Move crates",
			"move-crates",
			[],
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new GetItemsByIdActionStep(1, [item.Object.Id], [workplace.Object])
			}),
			TimeSpan.FromHours(1),
			null);
		var ai = LoadAI(gameworld.Object);

		Assert.IsTrue(ai.HandleMinuteTick(worker));

		var task = host.State.TaskBoard.ActiveTasks.Single();
		Assert.AreEqual(EmploymentTaskStatus.Completed, task.Status);
		Assert.AreEqual(worker.Id, task.AssignedEmployee?.Id);
		Assert.IsTrue(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ScheduledRuleEvaluated));
		Assert.IsTrue(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskAssigned));
	}

	[TestMethod]
	public void EmploymentWorkerAI_DoesNotBlockTasksWhenThisWorkerIsIneligible()
	{
		var currency = Currency();
		var workplace = Cell(31, "stockroom");
		var item = Item(301, "crate");
		var host = Shop(31, "task shop", currency.Object, workplace.Object);
		var gameworld = Gameworld(shops: [host.Shop.Object], currencies: [currency.Object]);
		var worker = Character(31, "Worker", gameworld.Object, workplace.Object).Object;
		host.State.Hire(worker, Offer(currency.Object, EmploymentRole.Employee, EmploymentAuthority.ManageDeliveryRoutes), null);
		var task = host.State.TaskBoard.CreateActiveTask(
			"Move crates",
			new EmploymentActionPlan(new IEmploymentActionStep[]
			{
				new GetItemsByIdActionStep(1, [item.Object.Id], [workplace.Object])
			}),
			null);
		var ai = LoadAI(gameworld.Object, """
		                                  <Definition>
		                                    <ReservationWage>0</ReservationWage>
		                                    <AcceptedPaymentMethods><Method>Cash</Method></AcceptedPaymentMethods>
		                                    <Capabilities><Capability>CanPostToBoard</Capability></Capabilities>
		                                  </Definition>
		                                  """);

		var acted = ai.HandleMinuteTick(worker);

		Assert.IsFalse(acted);
		Assert.AreEqual(EmploymentTaskStatus.Pending, task.Status);
		Assert.IsNull(task.AssignedEmployee);
		Assert.IsFalse(host.State.EmploymentRegister.Entries.Any(x =>
			x.EntryType == EmploymentRegisterEntryType.ActiveTaskBlocked));
	}

	[TestMethod]
	public void EmploymentWorkerAI_DoesNotExposeLegacyJobApis()
	{
		var forbidden = new[] { typeof(IJobListing), typeof(IActiveJob) };
		var exposedTypes = typeof(EmploymentWorkerAI)
		                   .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
		                   .Where(x => !x.IsSpecialName)
		                   .SelectMany(x => x.GetParameters().Select(y => y.ParameterType).Append(x.ReturnType))
		                   .ToList();

		foreach (var type in exposedTypes)
		foreach (var forbiddenType in forbidden)
		{
			Assert.IsFalse(ContainsType(type, forbiddenType), $"{type} unexpectedly exposes {forbiddenType}.");
		}
	}

	private static EmploymentWorkerAI LoadAI(IFuturemud gameworld, string? xml = null)
	{
		var model = new MudSharp.Models.ArtificialIntelligence
		{
			Id = 1,
			Name = "worker",
			Type = "EmploymentWorker",
			Definition = xml ?? """
			                    <Definition>
			                      <ReservationWage>0</ReservationWage>
			                      <AcceptedPaymentMethods><Method>Cash</Method></AcceptedPaymentMethods>
			                      <Capabilities><Capability>CanDeliverItems</Capability></Capabilities>
			                    </Definition>
			                    """
		};
		return (EmploymentWorkerAI)Activator.CreateInstance(
			typeof(EmploymentWorkerAI),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { model, gameworld },
			CultureInfo.InvariantCulture)!;
	}

	private static EmploymentWorkerAI CreateAI(IFuturemud gameworld)
	{
		return (EmploymentWorkerAI)Activator.CreateInstance(
			typeof(EmploymentWorkerAI),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { gameworld, "worker", false },
			CultureInfo.InvariantCulture)!;
	}

	private static string SaveXml(EmploymentWorkerAI ai)
	{
		return (string)typeof(EmploymentWorkerAI)
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(ai, Array.Empty<object>())!;
	}

	private static (Mock<IShop> Shop, EmploymentHostState State) Shop(long id, string name, ICurrency currency,
		ICell workplace)
	{
		var shop = new Mock<IShop>();
		var state = new EmploymentHostState(shop.Object);
		shop.SetupGet(x => x.Id).Returns(id);
		shop.SetupGet(x => x.Name).Returns(name);
		shop.SetupGet(x => x.FrameworkItemType).Returns("Shop");
		shop.SetupGet(x => x.Currency).Returns(currency);
		shop.SetupGet(x => x.CurrentLocations).Returns([workplace]);
		shop.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Shop);
		shop.SetupGet(x => x.Employment).Returns(state);
		shop.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		shop.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		shop.SetupGet(x => x.TaskBoard).Returns(() => state.TaskBoard);
		shop.SetupGet(x => x.EmploymentRegister).Returns(() => state.EmploymentRegister);
		shop.SetupGet(x => x.BusinessLedger).Returns(() => state.BusinessLedger);
		shop.SetupGet(x => x.Board).Returns(() => state.Board);
		shop.SetupGet(x => x.ManagerGoalBoard).Returns(() => state.ManagerGoalBoard);
		shop.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		    .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (shop, state);
	}

	private static (Mock<IStable> Stable, EmploymentHostState State) Stable(long id, string name, ICurrency currency,
		ICell workplace)
	{
		var stable = new Mock<IStable>();
		var state = new EmploymentHostState(stable.Object);
		stable.SetupGet(x => x.Id).Returns(id);
		stable.SetupGet(x => x.Name).Returns(name);
		stable.SetupGet(x => x.FrameworkItemType).Returns("Stable");
		stable.SetupGet(x => x.Currency).Returns(currency);
		stable.SetupGet(x => x.Location).Returns(workplace);
		stable.SetupGet(x => x.EmploymentHostType).Returns(EmploymentHostType.Stable);
		stable.SetupGet(x => x.Employment).Returns(state);
		stable.SetupGet(x => x.EmploymentContracts).Returns(() => state.EmploymentContracts);
		stable.SetupGet(x => x.JobOpenings).Returns(() => state.JobOpenings);
		stable.SetupGet(x => x.TaskBoard).Returns(() => state.TaskBoard);
		stable.SetupGet(x => x.EmploymentRegister).Returns(() => state.EmploymentRegister);
		stable.SetupGet(x => x.BusinessLedger).Returns(() => state.BusinessLedger);
		stable.SetupGet(x => x.Board).Returns(() => state.Board);
		stable.SetupGet(x => x.ManagerGoalBoard).Returns(() => state.ManagerGoalBoard);
		stable.Setup(x => x.HasAuthority(It.IsAny<ICharacter>(), It.IsAny<EmploymentAuthority>()))
		      .Returns((ICharacter actor, EmploymentAuthority authority) => state.HasAuthority(actor, authority));
		return (stable, state);
	}

	private static Mock<IFuturemud> Gameworld(IEnumerable<IShop>? shops = null, IEnumerable<IStable>? stables = null,
		IEnumerable<ICurrency>? currencies = null)
	{
		var gameworld = new Mock<IFuturemud>();
		var shopCollection = new All<IShop>();
		foreach (var shop in shops ?? [])
		{
			shopCollection.Add(shop);
		}

		var stableCollection = new All<IStable>();
		foreach (var stable in stables ?? [])
		{
			stableCollection.Add(stable);
		}

		var currencyCollection = new All<ICurrency>();
		foreach (var currency in (currencies ?? shopCollection.Select(x => x.Currency).Concat(stableCollection.Select(x => x.Currency)))
		         .Where(x => x is not null)
		         .DistinctBy(x => x.Id))
		{
			currencyCollection.Add(currency);
		}

		gameworld.SetupGet(x => x.Shops).Returns(shopCollection);
		gameworld.SetupGet(x => x.AuctionHouses).Returns(new All<IAuctionHouse>());
		gameworld.SetupGet(x => x.CombatArenas).Returns(new All<ICombatArena>());
		gameworld.SetupGet(x => x.Banks).Returns(new All<IBank>());
		gameworld.SetupGet(x => x.Stables).Returns(stableCollection);
		gameworld.SetupGet(x => x.Properties).Returns(new All<IProperty>());
		gameworld.SetupGet(x => x.Currencies).Returns(currencyCollection);
		gameworld.Setup(x => x.GetStaticLong("DefaultCurrencyID"))
		         .Returns(() => currencyCollection.FirstOrDefault()?.Id ?? 0L);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		return gameworld;
	}

	private static Mock<ICurrency> Currency()
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(1);
		currency.SetupGet(x => x.Name).Returns("test dollars");
		currency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(1.0M);
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns((decimal amount, CurrencyDescriptionPatternType _) => $"{amount:N2} test dollars");
		currency.Setup(x => x.TryGetBaseCurrency(It.IsAny<string>(), out It.Ref<decimal>.IsAny))
		        .Returns(new TryGetBaseCurrencyCallback((string text, out decimal amount) =>
			        decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)));
		return currency;
	}

	private static Mock<ICell> Cell(long id, string name)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.SetupGet(x => x.Name).Returns(name);
		cell.SetupGet(x => x.Location).Returns(cell.Object);
		cell.SetupGet(x => x.GameItems).Returns(Array.Empty<IGameItem>());
		cell.SetupGet(x => x.Characters).Returns(Array.Empty<ICharacter>());
		cell.Setup(x => x.GetFriendlyReference(It.IsAny<IPerceiver>())).Returns(name);
		return cell;
	}

	private static Mock<IGameItem> Item(long id, string name)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		item.SetupGet(x => x.Tags).Returns(Array.Empty<ITag>());
		return item;
	}

	private static Mock<ICharacter> Character(long id, string name, IFuturemud gameworld, ICell location)
	{
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns(name);
		var body = new Mock<IBody>();
		body.Setup(x => x.LookText(It.IsAny<IPerceivable>(), It.IsAny<bool>())).Returns(name);
		body.SetupGet(x => x.ExternalItems).Returns(Array.Empty<IGameItem>());
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.Culture).Returns(CultureInfo.InvariantCulture);
		var effects = new List<IEffect>();
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.CurrentName).Returns(personalName.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupGet(x => x.Location).Returns(location);
		character.SetupGet(x => x.OutputHandler).Returns(output.Object);
		character.SetupGet(x => x.Account).Returns(account.Object);
		character.SetupGet(x => x.State).Returns(CharacterState.Awake);
		character.Setup(x => x.CanMove(It.IsAny<CanMoveFlags>())).Returns(CanMoveResponse.True);
		character.SetupGet(x => x.Effects).Returns(effects);
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>()))
		         .Callback<IEffect>(effect => effects.Add(effect));
		character.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		         .Callback<IEffect, TimeSpan>((effect, _) => effects.Add(effect));
		character.Setup(x => x.AffectedBy<EmploymentWorkerSearchCooldownEffect>())
		         .Returns(() => effects.OfType<EmploymentWorkerSearchCooldownEffect>().Any());
		character.Setup(x => x.EffectsOfType<EmploymentWorkerTaskContextEffect>(
					It.IsAny<Predicate<EmploymentWorkerTaskContextEffect>?>()))
		         .Returns((Predicate<EmploymentWorkerTaskContextEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentWorkerTaskContextEffect>()
				         : effects.OfType<EmploymentWorkerTaskContextEffect>().Where(x => predicate(x)));
		character.Setup(x => x.EffectsOfType<EmploymentWorkerHostEvaluationEffect>(
					It.IsAny<Predicate<EmploymentWorkerHostEvaluationEffect>?>()))
		         .Returns((Predicate<EmploymentWorkerHostEvaluationEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentWorkerHostEvaluationEffect>()
				         : effects.OfType<EmploymentWorkerHostEvaluationEffect>().Where(x => predicate(x)));
		character.Setup(x => x.EffectsOfType<EmploymentWorkerRejectedOpeningEffect>(
					It.IsAny<Predicate<EmploymentWorkerRejectedOpeningEffect>?>()))
		         .Returns((Predicate<EmploymentWorkerRejectedOpeningEffect>? predicate) =>
			         predicate is null
				         ? effects.OfType<EmploymentWorkerRejectedOpeningEffect>()
				         : effects.OfType<EmploymentWorkerRejectedOpeningEffect>().Where(x => predicate(x)));
		character.Setup(x => x.RemoveAllEffects<EmploymentWorkerTaskContextEffect>(
					It.IsAny<Predicate<EmploymentWorkerTaskContextEffect>?>(),
					It.IsAny<bool>()))
		         .Returns((Predicate<EmploymentWorkerTaskContextEffect>? predicate, bool _) =>
		         {
			         var removed = predicate is null
				         ? effects.OfType<EmploymentWorkerTaskContextEffect>().ToList()
				         : effects.OfType<EmploymentWorkerTaskContextEffect>().Where(x => predicate(x)).ToList();
			         foreach (var effect in removed)
			         {
				         effects.Remove(effect);
			         }

			         return removed.Any();
		         });
		character.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);
		character.Setup(x => x.HowSeen(
				It.IsAny<IPerceiver>(),
				It.IsAny<bool>(),
				It.IsAny<DescriptionType>(),
				It.IsAny<bool>(),
				It.IsAny<PerceiveIgnoreFlags>()))
		         .Returns(name);
		return character;
	}

	private static JobOpeningDefinition Opening(ICurrency currency, decimal amount)
	{
		return new JobOpeningDefinition(
			EmploymentRole.Employee,
			JobRequirementSet.None,
			Pay(currency, amount),
			WorkSchedule.AnyTime,
			EmploymentDuration.Indefinite,
			1,
			true,
			new PaymentMethod(PaymentMethodKind.Cash),
			EmploymentAuthoritySet.Empty);
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

	private static EmploymentCandidateProfile Profile(ICharacter candidate)
	{
		return new EmploymentCandidateProfile(
			candidate,
			0.0M,
			new Dictionary<string, double>(),
			new HashSet<string>(),
			new HashSet<EmploymentAICapability> { EmploymentAICapability.CanDeliverItems },
			new HashSet<string>(),
			[PaymentMethodKind.Cash]);
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
}
