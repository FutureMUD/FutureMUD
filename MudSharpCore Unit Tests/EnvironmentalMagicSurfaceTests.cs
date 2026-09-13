#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.Magic;
using MudSharp.Magic.Environment;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EnvironmentalMagicSurfaceTests
{
	[TestMethod]
	public void FutureProgQueries_UsePureResourceAndScalarInspectionContracts()
	{
		var fixture = new Fixture();
		var location = Constant(fixture.Cell.Object, ProgVariableTypes.Location);
		var name = Constant(new TextVariable("Test Essence"));
		var id = Constant(new NumberVariable(1));
		var variables = new VariableSpace();
		var cases = new (string Name, IFunction[] Arguments, object Expected)[]
		{
			("environmentcap", [location, name], 12M),
			("environmentcap", [location, id], 12M),
			("environmentrate", [location, id], 0.5M),
			("environmentscardamage", [location], 4M),
			("environmentpressure", [location], 3M),
			("environmentlastdefile", [location], Fixture.DefileTime.UtcDateTime),
			("environmenthasdefile", [location], true)
		};

		foreach (var item in cases)
		{
			var function = Compile(item.Name, fixture.World.Object, item.Arguments);
			Assert.AreEqual(StatementResult.Normal, function.Execute(variables), function.ErrorMessage);
			Assert.AreEqual(item.Expected, function.Result.GetObject, item.Name);
		}

		fixture.Service.Verify(x => x.TryInspectResource(fixture.Cell.Object, fixture.Resource.Object,
			out It.Ref<EnvironmentalResourceSnapshot>.IsAny), Times.Exactly(3));
		fixture.Service.Verify(x => x.InspectState(fixture.Cell.Object), Times.Exactly(4));
		fixture.Service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void FutureProgLastDefile_AbsentEventReturnsDefaultDateAndFalsePresence()
	{
		var fixture = new Fixture();
		fixture.Service.Setup(x => x.InspectState(fixture.Cell.Object))
			.Returns(new EnvironmentalMagicStateSnapshot(EnvironmentalMagicState.Empty, 0));
		var location = Constant(fixture.Cell.Object, ProgVariableTypes.Location);
		var date = Compile("environmentlastdefile", fixture.World.Object, location);
		var present = Compile("environmenthasdefile", fixture.World.Object, location);

		Assert.AreEqual(StatementResult.Normal, date.Execute(new VariableSpace()));
		Assert.AreEqual(default(DateTime), date.Result.GetObject);
		Assert.AreEqual(StatementResult.Normal, present.Execute(new VariableSpace()));
		Assert.AreEqual(false, present.Result.GetObject);
	}

	[DataTestMethod]
	[DataRow("environmentcap")]
	[DataRow("environmentrate")]
	public void FutureProgResourceQueries_InvalidCalculationReportsError(string name)
	{
		var fixture = new Fixture();
		var invalid = new EnvironmentalResourceSnapshot(1, "Test Essence", 10, false, 0, 0, "Missing forage key.");
		fixture.Service.Setup(x => x.TryInspectResource(fixture.Cell.Object, fixture.Resource.Object, out invalid)).Returns(true);
		var function = Compile(name, fixture.World.Object,
			Constant(fixture.Cell.Object, ProgVariableTypes.Location), Constant(new NumberVariable(1)));

		Assert.AreEqual(StatementResult.Error, function.Execute(new VariableSpace()));
		Assert.AreEqual("Missing forage key.", function.ErrorMessage);
		fixture.Service.Verify(x => x.TryInspectResource(fixture.Cell.Object, fixture.Resource.Object,
			out It.Ref<EnvironmentalResourceSnapshot>.IsAny), Times.Once);
		fixture.Service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void FutureProgInvalidator_OnlyRequestsPolicyRecheck()
	{
		var fixture = new Fixture();
		var function = Compile("invalidateenvironment", fixture.World.Object,
			Constant(fixture.Cell.Object, ProgVariableTypes.Location));

		Assert.AreEqual(StatementResult.Normal, function.Execute(new VariableSpace()));
		Assert.AreEqual(true, function.Result.GetObject);
		fixture.Service.Verify(x => x.MarkDirty(fixture.Cell.Object, EnvironmentalMagicDirtyReason.Policy), Times.Once);
		fixture.Service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void MagicEnvironment_ActualAdminDispatcherRoutesBindingsInspectionAndRechecks()
	{
		var fixture = new Fixture();
		fixture.ExecuteAdmin("magic environment cell here Test Profile");
		fixture.ExecuteAdmin("magic environment cell 10 inherit");
		fixture.ExecuteAdmin("magic environment cell here disabled");
		fixture.ExecuteAdmin("magic environment show here");
		fixture.ExecuteAdmin("magic environment recheck 10");
		fixture.ExecuteAdmin("magic environment diagnostics");

		fixture.Service.Verify(x => x.SetBinding(fixture.Cell.Object, EnvironmentalMagicBindingMode.Explicit, 7L), Times.Once);
		fixture.Service.Verify(x => x.SetBinding(fixture.Cell.Object, EnvironmentalMagicBindingMode.Inherit, null), Times.Once);
		fixture.Service.Verify(x => x.SetBinding(fixture.Cell.Object, EnvironmentalMagicBindingMode.Disabled, null), Times.Once);
		fixture.Service.Verify(x => x.Inspect(fixture.Cell.Object), Times.Once);
		fixture.Service.Verify(x => x.MarkDirty(fixture.Cell.Object, EnvironmentalMagicDirtyReason.Policy), Times.Once);
		fixture.Service.Verify(x => x.DescribeDiagnostics(), Times.Once);
		Assert.IsTrue(fixture.Output.Any(x => x.Contains("Test Essence") && x.Contains("Recent Pressure")));
		Assert.IsTrue(fixture.Output.Any(x => x.Contains("coordinator diagnostics")));
		fixture.Service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void MagicEnvironment_PendingOperationPreventsBindingWithAnActionableError()
	{
		var fixture = new Fixture();
		fixture.Service.Setup(x => x.SetBinding(fixture.Cell.Object, EnvironmentalMagicBindingMode.Disabled, null))
			.Throws(new InvalidOperationException("Environmental operation 81c55c90-4858-4d40-ab53-bb7e3586f840 must be confirmed before changing its binding."));

		fixture.ExecuteAdmin("magic environment cell here disabled");

		Assert.IsTrue(fixture.Output.Any(x => x.Contains("must be confirmed before changing its binding")));
		Assert.IsFalse(fixture.Output.Any(x => x.Contains("production is disabled")));
	}

	[TestMethod]
	public void MagicEnvironment_PlayerCommandTreesCannotApplyStaffOperations()
	{
		var fixture = new Fixture(admin: false);
		const string command = "magic environment damage here 4 3 81c55c90-4858-4d40-ab53-bb7e3586f840 test";
		Assert.IsFalse(PlayerCommandTree.Instance.Commands.Execute(fixture.Actor.Object, command,
			CharacterState.Awake, PermissionLevel.Player));
		Assert.IsFalse(AdminCommandTree.StandardAdminCommandTree.Commands.Execute(fixture.Actor.Object, command,
			CharacterState.Awake, PermissionLevel.Player));
		MagicModule.MagicEnvironment(fixture.Actor.Object,
			new StringStack("damage here 4 3 81c55c90-4858-4d40-ab53-bb7e3586f840 test"));

		fixture.Service.VerifyNoOtherCalls();
		Assert.IsTrue(fixture.Output.Any(x => x.Contains("Only administrators")));
	}

	[TestMethod]
	public void MagicEnvironment_DamageAndRepairForwardStableIdentityAndAuditReason()
	{
		var fixture = new Fixture();
		var requests = new List<EnvironmentalMagicOperationRequest>();
		fixture.Service.Setup(x => x.ApplyOperation(fixture.Cell.Object, It.IsAny<EnvironmentalMagicOperationRequest>()))
			.Returns<ICell, EnvironmentalMagicOperationRequest>((_, request) =>
			{
				requests.Add(request);
				return new EnvironmentalMagicOperationResult(request.OperationId, true, requests.Count == 2,
					request.Damage, request.Pressure, Math.Min(2, request.Repair), null);
			});
		const string damage = "magic environment damage here 4 3 81c55c90-4858-4d40-ab53-bb7e3586f840 staff smoke test";
		fixture.ExecuteAdmin(damage);
		fixture.ExecuteAdmin(damage);
		fixture.ExecuteAdmin("magic environment repair here 5 97e2f4a1-c492-4208-a583-92d25ce06221 repair smoke test");

		Assert.AreEqual(3, requests.Count);
		Assert.AreEqual(requests[0], requests[1]);
		Assert.AreEqual(4.0, requests[0].Damage);
		Assert.AreEqual(3.0, requests[0].Pressure);
		Assert.AreEqual(100L, requests[0].ActorId);
		Assert.AreEqual("staff smoke test", requests[0].Attribution);
		Assert.AreEqual(5.0, requests[2].Repair);
		Assert.AreEqual(0.0, requests[2].Damage);
		Assert.IsTrue(fixture.Output.Any(x => x.Contains("recorded result")));
	}

	[DataTestMethod]
	[DataRow("damage here NaN 2 81c55c90-4858-4d40-ab53-bb7e3586f840 test")]
	[DataRow("damage here 2 Infinity 81c55c90-4858-4d40-ab53-bb7e3586f840 test")]
	[DataRow("damage here -1 2 81c55c90-4858-4d40-ab53-bb7e3586f840 test")]
	[DataRow("repair here 1 invalid-guid test")]
	[DataRow("repair here 1 81c55c90-4858-4d40-ab53-bb7e3586f840")]
	public void MagicEnvironment_InvalidStaffRequestDoesNotInvokeOperation(string arguments)
	{
		var fixture = new Fixture();
		fixture.ExecuteAdmin($"magic environment {arguments}");
		fixture.Service.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void TerrainEnvironment_BuilderAndStaffPathsPreserveInvalidBindingDiagnostics()
	{
		var fixture = new Fixture();
		var terrain = new Terrain(new MudSharp.Models.Terrain
		{
			Id = 20, Name = "Test Terrain", TerrainBehaviourMode = "outdoors", TagInformation = string.Empty,
			TerrainEditorColour = "#FF000000", TerrainANSIColour = "0", EnvironmentalMagicProfileId = 987L
		}, fixture.World.Object);
		var terrains = new All<ITerrain>();
		terrains.Add(terrain);
		fixture.World.SetupGet(x => x.Terrains).Returns(terrains);

		Assert.IsTrue(terrain.Show(fixture.Actor.Object).Contains("Missing environmental regenerator #987"));
		Assert.IsFalse(terrain.BuildingCommand(fixture.Actor.Object, new StringStack("environment unknown")));
		Assert.AreEqual(987L, terrain.EnvironmentalMagicProfileId);
		Assert.IsTrue(terrain.BuildingCommand(fixture.Actor.Object, new StringStack("environment Test Profile")));
		Assert.AreEqual(7L, terrain.EnvironmentalMagicProfileId);
		fixture.ExecuteAdmin("magic environment terrain \"Test Terrain\" none");
		Assert.IsNull(terrain.EnvironmentalMagicProfileId);
		fixture.ExecuteAdmin("magic environment terrain 20 Test Profile");
		Assert.AreEqual(7L, terrain.EnvironmentalMagicProfileId);
		fixture.Service.Verify(x => x.TerrainDefaultChanged(terrain), Times.Exactly(3));
	}

	private static IFunction Compile(string name, IFuturemud world, params IFunction[] arguments)
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var types = arguments.Select(x => x.ReturnType).ToArray();
		return FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName.EqualTo(name) &&
			x.Parameters.SequenceEqual(types, FutureProgVariableComparer.Instance)).CompilerFunction(arguments.ToList(), world);
	}

	private static IFunction Constant(IProgVariable value)
	{
		var function = new Mock<IFunction>();
		function.SetupGet(x => x.ReturnType).Returns(value.Type);
		function.SetupGet(x => x.Result).Returns(value);
		function.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		return function.Object;
	}

	private static IFunction Constant(object value, ProgVariableTypes type)
	{
		var variable = new Mock<IProgVariable>();
		variable.SetupGet(x => x.Type).Returns(type);
		variable.SetupGet(x => x.GetObject).Returns(value);
		return Constant(variable.Object);
	}

	private sealed class Fixture
	{
		public static readonly DateTimeOffset DefileTime = new(2026, 9, 13, 0, 0, 0, TimeSpan.Zero);
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IEnvironmentalMagicService> Service { get; } = new();
		public Mock<ICell> Cell { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IMagicResource> Resource { get; } = new();
		public List<string> Output { get; } = new();

		public Fixture(bool admin = true)
		{
			World.SetupGet(x => x.EnvironmentalMagic).Returns(Service.Object);
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Resource.SetupGet(x => x.Id).Returns(1L);
			Resource.SetupGet(x => x.Name).Returns("Test Essence");
			var resources = new All<IMagicResource>();
			resources.Add(Resource.Object);
			World.SetupGet(x => x.MagicResources).Returns(resources);
			Cell.SetupGet(x => x.Id).Returns(10L);
			Cell.SetupGet(x => x.Name).Returns("Test Cell");
			Cell.SetupGet(x => x.Gameworld).Returns(World.Object);
			var cells = new All<ICell>();
			cells.Add(Cell.Object);
			World.SetupGet(x => x.Cells).Returns(cells);
			var profile = new Mock<IEnvironmentalMagicProfile>();
			profile.SetupGet(x => x.Id).Returns(7L);
			profile.SetupGet(x => x.Name).Returns("Test Profile");
			var generators = new All<IMagicResourceRegenerator>();
			generators.Add(profile.Object);
			World.SetupGet(x => x.MagicResourceRegenerators).Returns(generators);
			Actor.SetupGet(x => x.Id).Returns(100L);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Cell.Object);
			Actor.SetupGet(x => x.Account).Returns(Mock.Of<IAccount>());
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Actor.SetupGet(x => x.LineFormatLength).Returns(120);
			Actor.SetupGet(x => x.InnerLineFormatLength).Returns(110);
			Actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(admin);
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Output.Add(text));
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
			var state = new EnvironmentalMagicState { ScarDamage = 4, LastDefileUtc = DefileTime };
			var resourceOutput = new EnvironmentalResourceSnapshot(1, "Test Essence", 10, true, 12, 0.5, null);
			Service.Setup(x => x.Inspect(Cell.Object)).Returns(new EnvironmentalMagicSnapshot(10,
				EnvironmentalMagicBindingMode.Explicit, 7, "Test Profile", state, 3,
				new Dictionary<string, double> { ["scardamage"] = 4 }, [resourceOutput], []));
			Service.Setup(x => x.InspectState(Cell.Object)).Returns(new EnvironmentalMagicStateSnapshot(state, 3));
			Service.Setup(x => x.TryInspectResource(Cell.Object, Resource.Object, out resourceOutput)).Returns(true);
			Service.Setup(x => x.DescribeDiagnostics()).Returns("Test coordinator diagnostics");
		}

		public void ExecuteAdmin(string command)
		{
			Assert.IsTrue(AdminCommandTree.StandardAdminCommandTree.Commands.Execute(Actor.Object, command,
				CharacterState.Awake, PermissionLevel.Admin, Actor.Object.OutputHandler), string.Join("\n", Output));
		}
	}
}
