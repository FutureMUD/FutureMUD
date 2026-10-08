#nullable enable
using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Functions;
using MudSharp.FutureProg.Functions.BuiltIn;
using MudSharp.FutureProg.Variables;
using MudSharp.Testing.EnvironmentalMagic;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RoomUniqueNameTests
{
	[TestMethod]
	public void Rename_TrimCaseCollisionsClearAndNoAliases()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 2);
		var a = (Room)world.Rooms.First();
		var b = (Room)world.Rooms.Last();
		Assert.IsTrue(a.TrySetUniqueName("  Église:North Gate  ", out _));
		Assert.AreEqual("Église:North Gate", a.UniqueName);
		Assert.IsTrue(a.Changed);
		Assert.IsFalse(b.TrySetUniqueName("église:north gate", out var error));
		StringAssert.Contains(error, $"#{a.Id}");
		Assert.IsNull(b.UniqueName);
		Assert.IsTrue(a.TrySetUniqueName("ÉGLISE:North Gate", out _));
		Assert.IsFalse(a.TrySetUniqueName("123", out _));
		Assert.IsFalse(a.TrySetUniqueName(new string('x', 256), out _));
		Assert.AreEqual("ÉGLISE:North Gate", a.UniqueName);
		Assert.IsTrue(a.TrySetUniqueName("new gate", out _));
		Assert.IsNull(world.Rooms.FindByUniqueName("ÉGLISE:North Gate"));
		Assert.IsTrue(a.TrySetUniqueName(" \t ", out _));
		Assert.IsTrue(b.TrySetUniqueName(null, out _));
		Assert.AreEqual(string.Empty, a.GetProperty("uniquename").GetObject);
	}

	[TestMethod]
	public void PersistedValidation_ReportsAllRoomIdentitiesAndNeverRepairs()
	{
		var rows = new[] { new Db.Room { Id = 41, UniqueName = "Gate" }, new Db.Room { Id = 99, UniqueName = "gate" } };
		var exception = Assert.ThrowsException<InvalidOperationException>(() => Room.ValidatePersistedUniqueNames(rows));
		StringAssert.Contains(exception.Message, "41");
		StringAssert.Contains(exception.Message, "99");
		Assert.AreEqual("gate", rows[1].UniqueName);
		foreach (var value in new[] { "42", " Gate ", new string('x', 256) })
			Assert.ThrowsException<InvalidOperationException>(() => Room.ValidatePersistedUniqueNames([new Db.Room { Id = 50, UniqueName = value }]));
		Room.ValidatePersistedUniqueNames([new Db.Room { Id = 1 }, new Db.Room { Id = 2, UniqueName = "" }, new Db.Room { Id = 3, UniqueName = "  " }]);
	}

	[TestMethod]
	public void HydrationAndSimulation_KeepTheirOwnIdentity()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var source = (Room)world.Rooms.First();
		var model = world.Models[source.Id];
		model.UniqueName = "Persisted:Gate";
		source.SetupRoom(model);
		Assert.AreEqual("Persisted:Gate", source.UniqueName);
		Assert.AreEqual("Persisted:Gate", source.GetProperty("uniquename").GetObject);
		var simulation = new Room(source, -101);
		Assert.IsNull(simulation.UniqueName);
		Assert.IsFalse(simulation.TrySetUniqueName("simulation", out _));
	}

	[TestMethod]
	public void Builder_IdentityEditingRequiresNoOverlayAndSupportsClearTokens()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var room = (Room)world.Rooms.First();
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.SetupGet(x => x.Location).Returns(room);
		var command = typeof(RoomBuilderModule).GetMethod("RoomSet", BindingFlags.NonPublic | BindingFlags.Static)!;
		command.Invoke(null, [actor.Object, new StringStack("uniquename  North Gate ")]);
		Assert.AreEqual("North Gate", room.UniqueName);
		foreach (var token in new[] { "none", "clear", "delete", "remove" })
		{
			Assert.IsTrue(room.TrySetUniqueName("gate", out _));
			command.Invoke(null, [actor.Object, new StringStack("unique " + token)]);
			Assert.IsNull(room.UniqueName, token);
		}
		command.Invoke(null, [actor.Object, new StringStack("uniquename")]);
		Assert.IsNull(room.UniqueName);
	}

	[TestMethod]
	public void RoomLookupAndProgs_KeyBeforeDisplayAndStrictKeyWithoutSpecialGrammar()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 2);
		var room = (Room)world.Rooms.First();
		var other = (Room)world.Rooms.Last();
		Assert.IsTrue(room.TrySetUniqueName(other.Name, out _));
		Assert.AreSame(room, RoomBuilderModule.LookupRoom(world.World.Object, other.Name));
		Assert.AreSame(other, RoomBuilderModule.LookupRoom(world.World.Object, other.Id.ToString()));
		Assert.AreSame(room, Execute(new LocationByUniqueNameFunction([Text(other.Name.ToUpperInvariant())], world.World.Object)));
		Assert.IsNull(Execute(new LocationByUniqueNameFunction([Text(room.Id.ToString())], world.World.Object)));
		Assert.IsTrue(room.TrySetUniqueName("here", out _));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(other);
		actor.SetupGet(x => x.Gameworld).Returns(world.World.Object);
		Assert.AreSame(other, RoomBuilderModule.LookupRoom(actor.Object, "here"));
		Assert.AreSame(room, Execute(new LocationByUniqueNameFunction([Text("here")], world.World.Object)));
		Assert.IsNull(Execute(new LocationByUniqueNameFunction([Text("missing")], world.World.Object)));
		Assert.AreSame(other, Execute(new ToLocationFunction([Text(other.Id.ToString())], world.World.Object, false)));
		Assert.IsTrue(room.TrySetUniqueName("@1", out _));
		Assert.AreSame(room, Execute(new LocationByUniqueNameFunction([Text("@1")], world.World.Object)));
		var previous = RoomBuilderModule.BuiltRooms.ToArray();
		try
		{
			RoomBuilderModule.BuiltRooms.Clear(); RoomBuilderModule.BuiltRooms.Add(other);
			Assert.AreSame(other, RoomBuilderModule.LookupRoom(world.World.Object, "@1"));
			Assert.AreSame(other, RoomBuilderModule.LookupRoom(actor.Object, "@1"));
		}
		finally { RoomBuilderModule.BuiltRooms.Clear(); RoomBuilderModule.BuiltRooms.AddRange(previous); }
	}

	[TestMethod]
	public void ProgRegistration_CompilesKeyLookupAndDotProperty()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var metadata = FutureProg.GetFunctionCompilerInformations().Single(x => x.FunctionName == "locationbyuniquename");
		Assert.AreEqual(ProgVariableTypes.Location, metadata.ReturnType);
		CollectionAssert.AreEqual(new[] { ProgVariableTypes.Text }, metadata.Parameters.ToArray());
		var prog = new FutureProg(FutureProgTestBootstrap.Gameworld, "cell_key_test", ProgVariableTypes.Text,
			[Tuple.Create(ProgVariableTypes.Location, "room")], "return @room.uniquename");
		Assert.IsTrue(prog.Compile(), prog.CompileError);
		var lookup = new FutureProg(FutureProgTestBootstrap.Gameworld, "cell_lookup_test", ProgVariableTypes.Location,
			[], "return locationbyuniquename(\"key\")");
		Assert.IsTrue(lookup.Compile(), lookup.CompileError);
	}

	[TestMethod]
	public void Goto_CharacterPrecedenceHashOverrideNumericAndLegacyFallback()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 3);
		var rooms = world.Rooms.Cast<Room>().ToArray();
		Assert.IsTrue(rooms[1].TrySetUniqueName("gate", out _));
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		actor.SetupGet(x => x.Gameworld).Returns(world.World.Object);
		actor.SetupGet(x => x.Location).Returns(rooms[0]);
		var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		target.SetupGet(x => x.Id).Returns(777);
		target.SetupGet(x => x.Location).Returns(rooms[2]);
		target.SetupGet(x => x.IsPlayerCharacter).Returns(true);
		target.Setup(x => x.GetKeywordsFor(actor.Object)).Returns(["gate"]);
		target.Setup(x => x.HasKeyword("gate", actor.Object, It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var personalName = new Mock<IPersonalName>();
		personalName.Setup(x => x.GetName(It.IsAny<NameStyle>())).Returns("gate");
		target.SetupGet(x => x.PersonalName).Returns(personalName.Object);
		target.As<IHavePersonalName>().SetupGet(x => x.PersonalName).Returns(personalName.Object);
		var actors = new All<ICharacter>();
		actors.Add(target.Object);
		world.World.SetupGet(x => x.Actors).Returns(actors);
		var command = typeof(SharedModule).GetMethod("Goto", BindingFlags.NonPublic | BindingFlags.Static)!;
		void Check(string input, IRoom expected)
		{
			actor.Invocations.Clear();
			command.Invoke(null, [actor.Object, "goto " + input]);
			actor.Verify(x => x.TransferTo(It.Is<SpatialLocation>(location => ReferenceEquals(location.Room, expected))), Times.Once);
		}
		Check("gate", rooms[2]);
		Check("#gate", rooms[1]);
		Check(rooms[1].Id.ToString(), rooms[1]);
		Check("#" + rooms[1].Id, rooms[1]);
		Check(rooms[1].Name, rooms[1]);
		Assert.IsTrue(rooms[1].TrySetUniqueName(null, out _));
		Check("#" + rooms[1].Name, rooms[1]);
	}

	private static IFunction Text(string value)
	{
		var parameter = new Mock<IFunction>();
		parameter.Setup(x => x.Execute(It.IsAny<IVariableSpace>())).Returns(StatementResult.Normal);
		parameter.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Text);
		parameter.SetupGet(x => x.Result).Returns(new TextVariable(value));
		return parameter.Object;
	}

	private static IProgVariable? Execute(IFunction function)
	{
		Assert.AreEqual(StatementResult.Normal, function.Execute(Mock.Of<IVariableSpace>()));
		return function.Result;
	}
}
