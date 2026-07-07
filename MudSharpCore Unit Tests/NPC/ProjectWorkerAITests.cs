using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Economy;
using MudSharp.Economy.Currency;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.NPC.AI;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.Work.Projects;
using MudSharp.Work.Projects.ConcreteTypes;
using ActiveProjectModel = MudSharp.Models.ActiveProject;

#nullable enable

namespace MudSharp_Unit_Tests.NPC;

[TestClass]
[DoNotParallelize]
public class ProjectWorkerAITests
{
	private delegate bool TryGetBaseCurrencyCallback(string pattern, out decimal amount);

	[TestInitialize]
	public void Setup()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		ProjectPaymentService.ClearInMemoryForTests();
	}

	[TestCleanup]
	public void Cleanup()
	{
		VirtualCashLedger.ClearInMemoryForTests();
		ProjectPaymentService.ClearInMemoryForTests();
	}

	[TestMethod]
	public void ProjectWorkerAI_LoadsAndSavesXmlConfiguration()
	{
		var currency = Currency(1, "test dollars");
		var gameworld = Gameworld(currency.Object);
		var ai = LoadAI(gameworld.Object, """
		                                  <Definition>
		                                    <Currency>1</Currency>
		                                    <MinimumHourlyPay>12.5</MinimumHourlyPay>
		                                    <MaxPathRange>25</MaxPathRange>
		                                    <SearchEnabled>false</SearchEnabled>
		                                    <ClaimPayments>false</ClaimPayments>
		                                    <DepositToBank>true</DepositToBank>
		                                    <SearchCadenceMinutes>3</SearchCadenceMinutes>
		                                    <BankAccountType>9</BankAccountType>
		                                  </Definition>
		                                  """);

		Assert.AreEqual(12.5M, ai.MinimumHourlyPay);
		Assert.AreSame(currency.Object, ai.Currency);
		Assert.AreEqual(25u, ai.MaxPathRange);
		Assert.IsFalse(ai.SearchEnabled);
		Assert.IsFalse(ai.ClaimPayments);
		Assert.IsTrue(ai.DepositToBank);
		Assert.AreEqual(TimeSpan.FromMinutes(3), ai.SearchCadence);
		Assert.AreEqual(9L, ai.BankAccountTypeId);

		var xml = SaveXml(ai);

		StringAssert.Contains(xml, "<Currency>1</Currency>");
		StringAssert.Contains(xml, "<MinimumHourlyPay>12.5</MinimumHourlyPay>");
		StringAssert.Contains(xml, "<DepositToBank>true</DepositToBank>");
		StringAssert.Contains(xml, "<BankAccountType>9</BankAccountType>");
	}

	[TestMethod]
	public void ProjectPaymentService_CreatesInMemoryLabourPayables()
	{
		var currency = Currency(3, "crowns");
		var owner = Character(10, "Owner");
		var worker = Character(20, "Worker");
		var labour = Labour(50, "Masonry");
		var projectDefinition = new Mock<IProject>();
		projectDefinition.SetupGet(x => x.Id).Returns(100);
		projectDefinition.SetupGet(x => x.RevisionNumber).Returns(2);
		var project = new Mock<IActiveProject>();
		project.SetupGet(x => x.Id).Returns(200);
		project.SetupGet(x => x.Name).Returns("Build a Wall");
		project.SetupGet(x => x.CharacterOwner).Returns(owner.Object);
		project.SetupGet(x => x.ProjectDefinition).Returns(projectDefinition.Object);

		ProjectPaymentService.CreateLabourPayable(project.Object, worker.Object, labour.Object, currency.Object, 7.5M);

		var payables = ProjectPaymentService.OutstandingPayablesFor(worker.Object).ToList();
		Assert.AreEqual(1, payables.Count);
		Assert.AreEqual(200, payables.Single().ActiveProjectId);
		Assert.AreEqual(100, payables.Single().ProjectDefinitionId);
		Assert.AreEqual(2, payables.Single().ProjectRevisionNumber);
		Assert.AreEqual("Build a Wall", payables.Single().ProjectName);
		Assert.AreEqual(10, payables.Single().ProjectOwnerCharacterId);
		Assert.AreEqual(20, payables.Single().CharacterId);
		Assert.AreEqual(3, payables.Single().CurrencyId);
		Assert.AreEqual(7.5M, payables.Single().Amount);
		Assert.AreEqual((int)ProjectPayableType.Labour, payables.Single().PayableType);
		Assert.AreEqual(50, payables.Single().ProjectLabourRequirementId);
		Assert.AreEqual("Masonry", payables.Single().RequirementName);
	}

	[TestMethod]
	public void ActiveProjectTick_UnfundedPaidLabourStopsWithoutProgressOrPayable()
	{
		var currency = Currency(1, "crowns");
		var gameworld = Gameworld(currency.Object);
		gameworld.Setup(x => x.GetStaticDouble("ProjectProgressMultiplier")).Returns(0.25);
		var labour = Labour(5, "Masonry");
		labour.Setup(x => x.HourlyProgress(It.IsAny<ICharacter>(), It.IsAny<bool>())).Returns(4.0);
		labour.Setup(x => x.ProgressMultiplierForOtherLabourPerPercentageComplete(It.IsAny<IProjectLabourRequirement>(), It.IsAny<IActiveProject>())).Returns(1.0);
		labour.SetupGet(x => x.LabourImpacts).Returns(Array.Empty<ILabourImpact>());
		var phase = new Mock<IProjectPhase>();
		phase.SetupGet(x => x.LabourRequirements).Returns([labour.Object]);
		phase.SetupGet(x => x.MaterialRequirements).Returns(Array.Empty<IProjectMaterialRequirement>());
		phase.SetupGet(x => x.CompletionActions).Returns(Array.Empty<IProjectAction>());
		var definition = new Mock<IProject>();
		definition.SetupGet(x => x.Id).Returns(100);
		definition.SetupGet(x => x.RevisionNumber).Returns(1);
		definition.SetupGet(x => x.Name).Returns("Paid Wall");
		definition.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		definition.SetupGet(x => x.Phases).Returns([phase.Object]);
		var worker = Character(20, "Worker");
		var project = new TestActiveProject(definition.Object, worker.Object, 200);
		project.SetPaymentCurrency(currency.Object);
		project.SetLabourPaymentRate(labour.Object, 100.0M);
		project.Join(worker.Object, labour.Object);

		project.DoProjectsTick();

		Assert.AreEqual(0, project.FulfilLabourCalls);
		Assert.IsNull(worker.Object.CurrentProject.Project);
		Assert.IsFalse(ProjectPaymentService.OutstandingPayablesFor(worker.Object).Any());
	}

	[TestMethod]
	public void ActiveProjectMaterialPreflight_RejectsPaidContributionWhenUnfunded()
	{
		var currency = Currency(1, "crowns");
		var gameworld = Gameworld(currency.Object);
		var phase = new Mock<IProjectPhase>();
		phase.SetupGet(x => x.LabourRequirements).Returns(Array.Empty<IProjectLabourRequirement>());
		phase.SetupGet(x => x.MaterialRequirements).Returns(Array.Empty<IProjectMaterialRequirement>());
		phase.SetupGet(x => x.CompletionActions).Returns(Array.Empty<IProjectAction>());
		var definition = new Mock<IProject>();
		definition.SetupGet(x => x.Id).Returns(100);
		definition.SetupGet(x => x.RevisionNumber).Returns(1);
		definition.SetupGet(x => x.Name).Returns("Paid Wall");
		definition.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		definition.SetupGet(x => x.Phases).Returns([phase.Object]);
		var owner = Character(10, "Owner");
		var material = new Mock<IProjectMaterialRequirement>();
		material.SetupGet(x => x.Id).Returns(30);
		material.SetupGet(x => x.Name).Returns("Stone");
		var project = new TestActiveProject(definition.Object, owner.Object, 201);
		project.SetPaymentCurrency(currency.Object);
		project.SetMaterialPaymentRate(material.Object, 5.0M);

		var canPay = project.CanPayMaterialContribution(material.Object, 2.0, out var error);

		Assert.IsFalse(canPay);
		StringAssert.Contains(error, "only has");
	}

	[TestMethod]
	public void ActiveLocalProject_DatabaseInsertPersistsCreatorAndCell()
	{
		var currency = Currency(1, "crowns");
		var gameworld = Gameworld(currency.Object);
		var phase = new Mock<IProjectPhase>();
		phase.SetupGet(x => x.LabourRequirements).Returns(Array.Empty<IProjectLabourRequirement>());
		phase.SetupGet(x => x.MaterialRequirements).Returns(Array.Empty<IProjectMaterialRequirement>());
		phase.SetupGet(x => x.CompletionActions).Returns(Array.Empty<IProjectAction>());
		var definition = new Mock<IProject>();
		definition.SetupGet(x => x.Id).Returns(100);
		definition.SetupGet(x => x.RevisionNumber).Returns(1);
		definition.SetupGet(x => x.Name).Returns("Paid Wall");
		definition.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		definition.SetupGet(x => x.Phases).Returns([phase.Object]);
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(300);
		var owner = Character(10, "Owner", gameworld.Object);
		owner.SetupGet(x => x.Location).Returns(cell.Object);
		var active = new TestActiveLocalProject(definition.Object, owner.Object);
		var model = new ActiveProjectModel();

		active.PopulateDatabaseModel(model);

		Assert.AreEqual(10, model.CharacterId);
		Assert.AreEqual(300, model.CellId);
	}

	[TestMethod]
	public void ActiveProject_EnsurePaymentCurrencyMarksProjectChanged()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.Changed = false;

		InvokeEnsurePaymentCurrency(project);

		Assert.IsTrue(project.Changed);
		Assert.AreEqual(1L, PaymentCurrencyId(project));
	}

	[TestMethod]
	public void ProjectPayLabour_LoweringPayWithActiveWorkersRequiresAccept()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var worker = Character(20, "Worker", gameworld.Object);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.SetPaymentCurrency(currency.Object);
		project.SetLabourPaymentRate(labour.Object, 20.0M);
		project.Join(worker.Object, labour.Object);
		Accept? capturedAccept = null;
		owner.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		     .Callback<IEffect, TimeSpan>((effect, _) => capturedAccept = effect as Accept);

		InvokeProjectPayLabour(owner.Object, project, "Masonry 10");

		Assert.IsNotNull(capturedAccept);
		Assert.AreEqual(20.0M, project.LabourPaymentRateFor(labour.Object));
		Assert.AreSame(project, worker.Object.CurrentProject.Project);
	}

	[TestMethod]
	public void ProjectPayLabour_AcceptingLowerPayRemovesWorkersAndAppliesRate()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var worker = Character(20, "Worker", gameworld.Object);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.SetPaymentCurrency(currency.Object);
		project.SetLabourPaymentRate(labour.Object, 20.0M);
		project.Join(worker.Object, labour.Object);
		Accept? capturedAccept = null;
		owner.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
		     .Callback<IEffect, TimeSpan>((effect, _) => capturedAccept = effect as Accept);

		InvokeProjectPayLabour(owner.Object, project, "Masonry 5");
		capturedAccept!.Proposal.Accept();

		Assert.AreEqual(5.0M, project.LabourPaymentRateFor(labour.Object));
		Assert.IsNull(worker.Object.CurrentProject.Project);
		Assert.IsFalse(project.ActiveLabour.Any());
		Assert.AreEqual(0, project.LeaveCalls);
	}

	[TestMethod]
	public void ProjectPayLabour_IncreasingPayWithActiveWorkersAppliesImmediately()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var worker = Character(20, "Worker", gameworld.Object);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.SetPaymentCurrency(currency.Object);
		project.SetLabourPaymentRate(labour.Object, 20.0M);
		project.Join(worker.Object, labour.Object);

		InvokeProjectPayLabour(owner.Object, project, "Masonry 25");

		Assert.AreEqual(25.0M, project.LabourPaymentRateFor(labour.Object));
		Assert.AreSame(project, worker.Object.CurrentProject.Project);
		owner.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);
	}

	[TestMethod]
	public void ActiveProjectTryJoinLabour_PlayerDisplacesNpcWhenSlotFull()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var player = Character(20, "Player", gameworld.Object);
		var npc = Character(30, "Worker NPC", gameworld.Object, isPlayerCharacter: false);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.Join(npc.Object, labour.Object);

		var joined = project.TryJoinLabour(player.Object, labour.Object, true, out var displacedWorker);

		Assert.IsTrue(joined);
		Assert.AreSame(npc.Object, displacedWorker);
		Assert.AreSame(project, player.Object.CurrentProject.Project);
		Assert.AreSame(labour.Object, player.Object.CurrentProject.Labour);
		Assert.IsNull(npc.Object.CurrentProject.Project);
		Assert.AreEqual(1, project.ActiveLabour.Count());
		Assert.AreSame(player.Object, project.ActiveLabour.Single().Character);
	}

	[TestMethod]
	public void ActiveProjectTryJoinLabour_NpcCannotDisplaceNpcWhenSlotFull()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var firstNpc = Character(30, "Worker NPC", gameworld.Object, isPlayerCharacter: false);
		var secondNpc = Character(31, "Other NPC", gameworld.Object, isPlayerCharacter: false);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.Join(firstNpc.Object, labour.Object);

		var joined = project.TryJoinLabour(secondNpc.Object, labour.Object, true, out var displacedWorker);

		Assert.IsFalse(joined);
		Assert.IsNull(displacedWorker);
		Assert.AreSame(project, firstNpc.Object.CurrentProject.Project);
		Assert.IsNull(secondNpc.Object.CurrentProject.Project);
		Assert.AreEqual(1, project.ActiveLabour.Count());
		Assert.AreSame(firstNpc.Object, project.ActiveLabour.Single().Character);
	}

	[TestMethod]
	public void ProjectLabourQueueEntry_PlayerCanBeReadyWhenNpcSlotIsDisplaceable()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var player = Character(20, "Player", gameworld.Object);
		var npc = Character(30, "Worker NPC", gameworld.Object, isPlayerCharacter: false);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.Join(npc.Object, labour.Object);
		var entry = new ProjectLabourQueueEntry(player.Object, project, labour.Object, 1);

		Assert.AreEqual(ProjectLabourQueueStatus.Ready, entry.StatusFor(player.Object));
	}

	[TestMethod]
	public void ProjectLabourQueueEntry_NpcStillWaitsWhenNpcSlotIsFull()
	{
		var currency = Currency(1, "crowns");
		var activeProjects = new All<IActiveProject>();
		var gameworld = Gameworld(currency.Object, activeProjects);
		var owner = Character(10, "Owner", gameworld.Object);
		var firstNpc = Character(30, "Worker NPC", gameworld.Object, isPlayerCharacter: false);
		var secondNpc = Character(31, "Other NPC", gameworld.Object, isPlayerCharacter: false);
		var labour = Labour(5, "Masonry");
		var project = ProjectWithLabour(gameworld.Object, owner.Object, labour.Object, activeProjects);
		project.Join(firstNpc.Object, labour.Object);
		var entry = new ProjectLabourQueueEntry(secondNpc.Object, project, labour.Object, 1);

		Assert.AreEqual(ProjectLabourQueueStatus.WaitingForSlot, entry.StatusFor(secondNpc.Object));
	}

	private static void InvokeProjectPayLabour(ICharacter actor, IActiveProject project, string command)
	{
		var moduleType = typeof(ProjectWorkerAI).Assembly.GetType("MudSharp.Commands.Modules.CraftModule");
		Assert.IsNotNull(moduleType);
		var method = moduleType.GetMethod("ProjectPayLabour", BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		method.Invoke(null, new object[] { actor, project, new StringStack(command) });
	}

	private static void InvokeEnsurePaymentCurrency(ActiveProject project)
	{
		typeof(ActiveProject)
			.GetMethod("EnsurePaymentCurrency", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(project, Array.Empty<object>());
	}

	private static long? PaymentCurrencyId(ActiveProject project)
	{
		return (long?)typeof(ActiveProject)
		              .GetField("_paymentCurrencyId", BindingFlags.Instance | BindingFlags.NonPublic)!
		              .GetValue(project);
	}

	private static TestActiveProject ProjectWithLabour(IFuturemud gameworld, ICharacter owner,
		IProjectLabourRequirement labour, All<IActiveProject> activeProjects)
	{
		var phase = new Mock<IProjectPhase>();
		phase.SetupGet(x => x.LabourRequirements).Returns([labour]);
		phase.SetupGet(x => x.MaterialRequirements).Returns(Array.Empty<IProjectMaterialRequirement>());
		phase.SetupGet(x => x.CompletionActions).Returns(Array.Empty<IProjectAction>());
		var definition = new Mock<IProject>();
		definition.SetupGet(x => x.Id).Returns(100);
		definition.SetupGet(x => x.RevisionNumber).Returns(1);
		definition.SetupGet(x => x.Name).Returns("Paid Wall");
		definition.SetupGet(x => x.Gameworld).Returns(gameworld);
		definition.SetupGet(x => x.Phases).Returns([phase.Object]);
		var project = new TestActiveProject(definition.Object, owner, 200);
		activeProjects.Add(project);
		return project;
	}

	private static ProjectWorkerAI LoadAI(IFuturemud gameworld, string xml)
	{
		var model = new MudSharp.Models.ArtificialIntelligence
		{
			Id = 1,
			Name = "project worker",
			Type = "ProjectWorker",
			Definition = xml
		};
		return (ProjectWorkerAI)Activator.CreateInstance(
			typeof(ProjectWorkerAI),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			new object[] { model, gameworld },
			CultureInfo.InvariantCulture)!;
	}

	private static string SaveXml(ProjectWorkerAI ai)
	{
		return (string)typeof(ProjectWorkerAI)
		                .GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!
		                .Invoke(ai, Array.Empty<object>())!;
	}

	private static Mock<IFuturemud> Gameworld(ICurrency currency, All<IActiveProject>? activeProjects = null)
	{
		var currencies = new All<ICurrency>();
		currencies.Add(currency);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Currencies).Returns(currencies);
		gameworld.SetupGet(x => x.ActiveProjects).Returns(activeProjects ?? new All<IActiveProject>());
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		gameworld.Setup(x => x.GetStaticLong("DefaultCurrencyID")).Returns(currency.Id);
		return gameworld;
	}

	private static Mock<ICurrency> Currency(long id, string name)
	{
		var currency = new Mock<ICurrency>();
		currency.SetupGet(x => x.Id).Returns(id);
		currency.SetupGet(x => x.Name).Returns(name);
		currency.SetupGet(x => x.BaseCurrencyToGlobalBaseCurrencyConversion).Returns(1.0M);
		currency.Setup(x => x.Describe(It.IsAny<decimal>(), It.IsAny<CurrencyDescriptionPatternType>()))
		        .Returns((decimal amount, CurrencyDescriptionPatternType _) => $"{amount:N2} {name}");
		currency.Setup(x => x.TryGetBaseCurrency(It.IsAny<string>(), out It.Ref<decimal>.IsAny))
		        .Returns(new TryGetBaseCurrencyCallback((string text, out decimal amount) =>
			        decimal.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out amount)));
		return currency;
	}

	private static Mock<ICharacter> Character(long id, string name, IFuturemud? gameworld = null,
		bool isPlayerCharacter = true)
	{
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Id).Returns(id);
		character.SetupGet(x => x.Name).Returns(name);
		character.SetupGet(x => x.IsPlayerCharacter).Returns(isPlayerCharacter);
		character.SetupGet(x => x.OutputHandler).Returns(new NonPlayerOutputHandler());
		if (gameworld is not null)
		{
			character.SetupGet(x => x.Gameworld).Returns(gameworld);
		}

		character.SetupProperty(x => x.CurrentProject);
		return character;
	}

	private static Mock<IProjectLabourRequirement> Labour(long id, string name)
	{
		var labour = new Mock<IProjectLabourRequirement>();
		labour.SetupGet(x => x.Id).Returns(id);
		labour.SetupGet(x => x.Name).Returns(name);
		labour.SetupGet(x => x.MaximumSimultaneousWorkers).Returns(1);
		labour.Setup(x => x.CharacterIsQualified(It.IsAny<ICharacter>())).Returns(true);
		return labour;
	}

	private sealed class TestActiveProject : ActiveProject
	{
		public TestActiveProject(IProject project, ICharacter owner, long id) : base(project)
		{
			_characterOwner = owner;
			_characterOwnerId = owner.Id;
			SetIDFromDatabase(new ActiveProjectModel { Id = id });
			IdInitialised = true;
		}

		public int FulfilLabourCalls { get; private set; }
		public int LeaveCalls { get; private set; }

		public override void Cancel(ICharacter actor)
		{
		}

		public override bool FulfilLabour(IProjectLabourRequirement labour, double progress)
		{
			FulfilLabourCalls++;
			_labourProgress[labour] += progress;
			return false;
		}

		public override void FulfilMaterial(IProjectMaterialRequirement material, double progress)
		{
			_materialProgress[material] += progress;
		}

		public override void Join(ICharacter actor, IProjectLabourRequirement labour)
		{
			_activeLabour.Add((actor, labour));
			actor.CurrentProject = (this, labour);
		}

		public override void Leave(ICharacter actor)
		{
			LeaveCalls++;
			_activeLabour.RemoveAll(x => x.Character == actor);
			actor.CurrentProject = (null, null);
		}

		public override string ProjectsCommandOutput(ICharacter actor)
		{
			return Name;
		}

		protected override void DatabaseInsert(ActiveProjectModel project)
		{
		}
	}

	private sealed class TestActiveLocalProject : ActiveLocalProject
	{
		public TestActiveLocalProject(IProject project, ICharacter owner) : base(project, owner)
		{
		}

		public void PopulateDatabaseModel(ActiveProjectModel project)
		{
			base.DatabaseInsert(project);
		}
	}
}
