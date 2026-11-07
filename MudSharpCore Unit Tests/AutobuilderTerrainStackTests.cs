#nullable enable
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Construction.Autobuilder;
using MudSharp.Construction.Autobuilder.Areas;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Editor;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class AutobuilderTerrainStackTests
{
	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void Execute_VerticalExit_RegistersBothEndpointsAndPersistsSettings(bool climb, bool fall)
	{
		using var database = new InMemoryDatabaseTestScope();
		var fixture = new Fixture();
		var payload = new TerrainStackPayload(1, 1, [-1, 0], [new(0, 0, 0, 1, []), new(0, 0, -1, 1, [9])],
			[new(0, 0, -1, Climb: climb, Difficulty: 7, Fall: fall)]);
		var results = fixture.Area.ExecuteTemplate(fixture.Actor.Object, [fixture.RoomTemplate.Object, payload]).ToList();
		Assert.AreEqual(2, results.Count);
		var exits = fixture.Created.SelectMany(room => Mock.Get(room.GetOrCreateOverlay(fixture.Package.Object)).Invocations
			.Where(call => call.Method.Name == "AddExit").Select(call => (MudSharp.Construction.Boundary.IExit)call.Arguments[0])).ToList();
		Assert.AreEqual(2, exits.Count);
		Assert.AreSame(exits[0], exits[1]);
		Assert.AreEqual(climb, exits[0].IsClimbExit);
		Assert.AreEqual((Difficulty)7, exits[0].ClimbDifficulty);
		Assert.AreEqual(fall ? results[0] : null, exits[0].FallRoom);
		exits[0].Save();
		var saved = database.Context.Exits.Single();
		Assert.AreEqual(climb, saved.IsClimbExit);
		Assert.AreEqual(fall ? results[0].Id : (long?)null, saved.FallRoom);
		Assert.AreEqual((int)CardinalDirection.Up, saved.Direction1);
		Assert.AreEqual((int)CardinalDirection.Down, saved.Direction2);
		Assert.AreEqual(4, Mock.Get(fixture.World.Object.ExitManager).Invocations.Count(call => call.Method.Name == "UpdateRoomOverlayExits"));
	}

	[TestMethod]
	public void Execute_RealExitManager_SeesClimbFallOverridesBeforeSaveTick()
	{
		using var database = new InMemoryDatabaseTestScope();
		var fixture = new Fixture();
		var manager = new MudSharp.Construction.Boundary.ExitManager(fixture.World.Object);
		fixture.World.SetupGet(world => world.ExitManager).Returns(manager);
		var payload = new TerrainStackPayload(1, 1, [-1, 0], [new(0, 0, -1, 1, []), new(0, 0, 0, 1, [])],
			[new(0, 0, -1, Climb: true, Difficulty: 7, Fall: true)]);
		var rooms = fixture.Area.ExecuteTemplate(fixture.Actor.Object, [fixture.RoomTemplate.Object, payload]).ToList();
		var up = manager.GetAllExits(rooms[0]).Single();
		var down = manager.GetAllExits(rooms[1]).Single();
		Assert.IsTrue(up.IsClimbExit);
		Assert.AreEqual((Difficulty)7, up.ClimbDifficulty);
		Assert.IsTrue(down.IsFallExit);
		Assert.AreSame(up.Exit, down.Exit);
		Assert.AreEqual(rooms[0], up.Exit.FallRoom);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void Command_TokenOrEditor_CreatesRoomsAndRunsPostProcessing(bool editor)
	{
		using var output = new PriorityOutputScope();
		var fixture = new Fixture();
		var token = TerrainStackCodec.Encode(new TerrainStackPayload(1, 1, [0], [new(0, 0, 0, 1, [])], []));
		fixture.Invoke($"5 prog=9 7 {(editor ? "editor" : token)}");
		if (editor)
		{
			Assert.AreEqual(0, fixture.Created.Count);
			Assert.IsNotNull(fixture.Submit);
			var editorOutput = new Mock<IOutputHandler>();
			fixture.Submit(TerrainStackCodec.WrapForEditor(token), editorOutput.Object, []);
			editorOutput.Verify(handler => handler.Send(It.Is<string>(text => text.Contains("Generated 1 rooms.")), true, false), Times.Once);
		}
		Assert.AreEqual(1, fixture.Created.Count);
		fixture.Actor.Verify(actor => actor.TransferTo(fixture.Created[0], RoomLayer.GroundLevel), Times.Once);
		Assert.IsTrue(fixture.Prog.Invocations.Any(call => call.Method.Name == "Execute"));
		Assert.IsTrue(RoomBuilderModule.BuiltRooms.Contains(fixture.Created[0]));
		RoomBuilderModule.BuiltRooms.Remove(fixture.Created[0]);
	}

	private sealed class PriorityOutputScope : IDisposable
	{
		private readonly Futuremud _game = TestObjectFactory.CreateUninitialized<Futuremud>();
		private readonly List<Futuremud> _games = (List<Futuremud>)typeof(Futuremud)
			.GetField("_allgames", BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;
		public PriorityOutputScope()
		{
			typeof(Futuremud).GetField("_connections", BindingFlags.NonPublic | BindingFlags.Instance)!
				.SetValue(_game, new MudSharp.Network.ConnectionSnapshotRegistry());
			_games.Insert(0, _game);
		}
		public void Dispose() => _games.Remove(_game);
	}

	[TestMethod]
	public void Editor_CancelMalformedOrChangedOverlay_CreatesNothing()
	{
		var fixture = new Fixture();
		fixture.Invoke("5 7 editor");
		Assert.IsNotNull(fixture.Cancel);
		fixture.Cancel(fixture.Actor.Object.OutputHandler, []);
		Assert.AreEqual(0, fixture.Created.Count);
		fixture.Invoke("5 7 editor");
		var editorOutput = new Mock<IOutputHandler>();
		fixture.Submit!("FMS1:bad", editorOutput.Object, []);
		editorOutput.Verify(handler => handler.Send(It.Is<string>(text => text.Contains("Truncated terrain stack")), true, false), Times.Once);
		Assert.AreEqual(0, fixture.Created.Count);
		fixture.Invoke("5 7 editor");
		fixture.Package.SetupGet(package => package.Status).Returns(RevisionStatus.Current);
		fixture.Submit!(TerrainStackCodec.Encode(new TerrainStackPayload(1, 1, [0], [new(0, 0, 0, 1, [])], [])), fixture.Actor.Object.OutputHandler, []);
		Assert.AreEqual(0, fixture.Created.Count);
	}

	[TestMethod]
	public void Preflight_UnknownTerrainOrTag_CreatesNothing()
	{
		var fixture = new Fixture();
		foreach (var room in new[] { new TerrainStackRoom(0, 0, 0, 99, []), new(0, 0, 0, 1, [99]) })
		{
			var token = TerrainStackCodec.Encode(new TerrainStackPayload(1, 1, [0], [room], []));
			fixture.Invoke($"5 7 {token}");
			Assert.AreEqual(0, fixture.Created.Count);
		}
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<ICharacter> Actor { get; } = new() { DefaultValue = DefaultValue.Mock };
		public Mock<IRoomOverlayPackage> Package { get; } = new();
		public Mock<IAutobuilderRoom> RoomTemplate { get; } = new();
		public Mock<IFutureProg> Prog { get; } = new();
		public AutobuilderAreaTerrainStack Area { get; }
		public List<IRoom> Created { get; } = [];
		public Action<string, IOutputHandler, object[]>? Submit { get; private set; }
		public Action<IOutputHandler, object[]>? Cancel { get; private set; }
		public Fixture()
		{
			var rooms = new All<IRoom>();
			World.SetupGet(world => world.Rooms).Returns(rooms);
			var terrain = Mock.Of<ITerrain>(value => value.Id == 1 && value.Name == "Grass");
			var terrains = new All<ITerrain>(); terrains.Add(terrain);
			World.SetupGet(world => world.Terrains).Returns(terrains);
			var tags = new All<ITag>(); tags.Add(Mock.Of<ITag>(value => value.Id == 9 && value.Name == "Grove"));
			World.SetupGet(world => world.Tags).Returns(tags);
			Package.SetupGet(package => package.Status).Returns(RevisionStatus.UnderDesign);
			Actor.SetupGet(actor => actor.Gameworld).Returns(World.Object);
			Actor.SetupGet(actor => actor.CurrentOverlayPackage).Returns(Package.Object);
			AutobuilderAreaTerrainStack.RegisterAutobuilderLoader();
			Area = (AutobuilderAreaTerrainStack)AutobuilderFactory.LoadArea(new MudSharp.Models.AutobuilderAreaTemplate
			{ Id = 5, Name = "Stack", TemplateType = "terrain stack", Definition = "<Template />" }, World.Object);
			var areas = new All<IAutobuilderArea>(); areas.Add(Area);
			World.SetupGet(world => world.AutobuilderAreas).Returns(areas);
			RoomTemplate.SetupGet(template => template.Id).Returns(7);
			RoomTemplate.SetupGet(template => template.Name).Returns("Room");
			var templates = new All<IAutobuilderRoom>(); templates.Add(RoomTemplate.Object);
			World.SetupGet(world => world.AutobuilderRooms).Returns(templates);
			RoomTemplate.Setup(template => template.CreateRoom(Actor.Object, terrain, false, It.IsAny<IReadOnlyCollection<ITag>>(), It.IsAny<string[]>()))
				.Returns(() =>
				{
					var room = new Mock<IRoom> { DefaultValue = DefaultValue.Mock };
					room.SetupGet(value => value.Id).Returns(Created.Count + 100L);
					var overlay = new Mock<IEditableRoomOverlay>();
					var exitIds = new List<long>();
					overlay.SetupGet(value => value.ExitIDs).Returns(exitIds);
					overlay.SetupGet(value => value.Package).Returns(Package.Object);
					overlay.Setup(value => value.AddExit(It.IsAny<MudSharp.Construction.Boundary.IExit>()))
						.Callback<MudSharp.Construction.Boundary.IExit>(exit =>
						{
							exitIds.Add(exit.Id);
							World.Object.ExitManager.UpdateRoomOverlayExits(room.Object, overlay.Object);
						});
					room.Setup(value => value.GetOrCreateOverlay(Package.Object)).Returns(overlay.Object);
					room.SetupGet(value => value.CurrentOverlay).Returns(overlay.Object);
					room.SetupGet(value => value.Overlays).Returns([overlay.Object]);
					Created.Add(room.Object); rooms.Add(room.Object); return room.Object;
				});
			Prog.SetupGet(prog => prog.Id).Returns(9);
			Prog.SetupGet(prog => prog.FunctionName).Returns("PostStack");
			Prog.Setup(prog => prog.MatchesParameters(It.IsAny<ProgVariableTypes[]>())).Returns(true);
			var progs = new All<IFutureProg>(); progs.Add(Prog.Object);
			World.SetupGet(world => world.FutureProgs).Returns(progs);
			Actor.Setup(actor => actor.EditorMode(It.IsAny<Action<string, IOutputHandler, object[]>>(), It.IsAny<Action<IOutputHandler, object[]>>(),
				It.IsAny<double>(), It.IsAny<string>(), It.IsAny<EditorOptions>(), It.IsAny<object[]>()))
				.Callback<Action<string, IOutputHandler, object[]>, Action<IOutputHandler, object[]>, double, string, EditorOptions, object[]>(
					(post, cancel, _, _, _, _) => { Submit = post; Cancel = cancel; });
		}
		public void Invoke(string arguments) => typeof(RoomBuilderModule).GetMethod("RoomNewTemplate", BindingFlags.NonPublic | BindingFlags.Static)!
			.Invoke(null, [Actor.Object, new StringStack(arguments)]);
	}
	[TestMethod]
	public void Connections_StackWithHolesAndMissingLevel_ConnectsEachNeighbourOnce()
	{
		var rooms = new[]
		{
			new TerrainStackRoom(0, 0, -1, 1, []), new(1, 0, -1, 1, []), new(0, 1, -1, 1, []),
			new(0, 0, 0, 1, []), new(1, 0, 0, 1, []), new(0, 0, 2, 1, [])
		};
		var payload = new TerrainStackPayload(2, 2, [-1, 0, 2], rooms, [new(1, 0, -1, Suppressed: true)]);
		var connections = AutobuilderAreaTerrainStack.GetConnections(payload).ToList();
		Assert.AreEqual(4, connections.Count);
		Assert.AreEqual(1, connections.Count(link => link.Direction == CardinalDirection.Up));
		Assert.AreEqual(2, connections.Count(link => link.Direction == CardinalDirection.East));
		Assert.AreEqual(1, connections.Count(link => link.Direction == CardinalDirection.North));
		Assert.AreEqual(connections.Count, connections.Distinct().Count());
		Assert.IsFalse(connections.Any(link => link.Destination.Z == 2));
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(true, true)]
	public void Connections_VerticalSettings_KeepClimbAndFallIndependent(bool climb, bool fall)
	{
		var settings = new TerrainStackLink(0, 0, -1, Climb: climb, Difficulty: 7, Fall: fall);
		var payload = new TerrainStackPayload(1, 1, [-1, 0], [new(0, 0, -1, 1, []), new(0, 0, 0, 1, [])], [settings]);
		var connection = AutobuilderAreaTerrainStack.GetConnections(payload).Single();
		Assert.AreEqual((0, 0, -1), connection.Origin);
		Assert.AreEqual((0, 0, 0), connection.Destination);
		Assert.AreEqual(CardinalDirection.Up, connection.Direction);
		Assert.AreEqual(CardinalDirection.Down, connection.Direction.Opposite());
		Assert.AreEqual(settings, connection.Settings);
	}

	[TestMethod]
	public void CanImport_ChangedOrApprovedOverlay_RejectsBeforeCreation()
	{
		var actor = new Mock<ICharacter>();
		var package = new Mock<IRoomOverlayPackage>();
		package.SetupGet(value => value.Status).Returns(RevisionStatus.UnderDesign);
		actor.SetupGet(value => value.CurrentOverlayPackage).Returns(package.Object);
		Assert.IsTrue(RoomBuilderModule.CanImportTerrainStack(actor.Object, package.Object));
		Assert.IsFalse(RoomBuilderModule.CanImportTerrainStack(actor.Object, null));
		Assert.IsFalse(RoomBuilderModule.CanImportTerrainStack(actor.Object, new Mock<IRoomOverlayPackage>().Object));
		package.SetupGet(value => value.Status).Returns(RevisionStatus.Current);
		Assert.IsFalse(RoomBuilderModule.CanImportTerrainStack(actor.Object, package.Object));
	}
}
