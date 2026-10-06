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
public class CellUniqueNameTests
{
	[TestMethod]
	public void Rename_TrimCaseCollisionsClearAndNoAliases()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 2);
		var a = (Cell)world.Cells.First();
		var b = (Cell)world.Cells.Last();
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
		Assert.IsNull(world.Cells.FindByUniqueName("ÉGLISE:North Gate"));
		Assert.IsTrue(a.TrySetUniqueName(" \t ", out _));
		Assert.IsTrue(b.TrySetUniqueName(null, out _));
		Assert.AreEqual(string.Empty, a.GetProperty("uniquename").GetObject);
	}

	[TestMethod]
	public void PersistedValidation_ReportsAllCellIdentitiesAndNeverRepairs()
	{
		var rows = new[] { new Db.Cell { Id = 41, UniqueName = "Gate" }, new Db.Cell { Id = 99, UniqueName = "gate" } };
		var exception = Assert.ThrowsException<InvalidOperationException>(() => Cell.ValidatePersistedUniqueNames(rows));
		StringAssert.Contains(exception.Message, "41");
		StringAssert.Contains(exception.Message, "99");
		Assert.AreEqual("gate", rows[1].UniqueName);
		foreach (var value in new[] { "42", " Gate ", new string('x', 256) })
			Assert.ThrowsException<InvalidOperationException>(() => Cell.ValidatePersistedUniqueNames([new Db.Cell { Id = 50, UniqueName = value }]));
		Cell.ValidatePersistedUniqueNames([new Db.Cell { Id = 1 }, new Db.Cell { Id = 2, UniqueName = "" }, new Db.Cell { Id = 3, UniqueName = "  " }]);
	}

	[TestMethod]
	public void HydrationAndSimulation_KeepTheirOwnIdentity()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var source = (Cell)world.Cells.First();
		var model = world.Models[source.Id];
		model.UniqueName = "Persisted:Gate";
		source.SetupCell(model);
		Assert.AreEqual("Persisted:Gate", source.UniqueName);
		Assert.AreEqual("Persisted:Gate", source.GetProperty("uniquename").GetObject);
		var simulation = new Cell(source, -101);
		Assert.IsNull(simulation.UniqueName);
		Assert.IsFalse(simulation.TrySetUniqueName("simulation", out _));
	}

	[TestMethod]
	public void Builder_IdentityEditingRequiresNoOverlayAndSupportsClearTokens()
	{
		using var world = new EnvironmentalMagicTestWorld();
		var cell = (Cell)world.Cells.First();
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.SetupGet(x => x.Location).Returns(cell);
		var command = typeof(RoomBuilderModule).GetMethod("CellSet", BindingFlags.NonPublic | BindingFlags.Static)!;
		command.Invoke(null, [actor.Object, new StringStack("uniquename  North Gate ")]);
		Assert.AreEqual("North Gate", cell.UniqueName);
		foreach (var token in new[] { "none", "clear", "delete", "remove" })
		{
			Assert.IsTrue(cell.TrySetUniqueName("gate", out _));
			command.Invoke(null, [actor.Object, new StringStack("unique " + token)]);
			Assert.IsNull(cell.UniqueName, token);
		}
		command.Invoke(null, [actor.Object, new StringStack("uniquename")]);
		Assert.IsNull(cell.UniqueName);
	}

	[TestMethod]
	public void RoomLookupAndProgs_KeyBeforeDisplayAndStrictKeyWithoutSpecialGrammar()
	{
		using var world = new EnvironmentalMagicTestWorld(count: 2);
		var cell = (Cell)world.Cells.First();
		var other = (Cell)world.Cells.Last();
		Assert.IsTrue(cell.TrySetUniqueName(other.Name, out _));
		Assert.AreSame(cell, RoomBuilderModule.LookupCell(world.World.Object, other.Name));
		Assert.AreSame(other, RoomBuilderModule.LookupCell(world.World.Object, other.Id.ToString()));
		Assert.AreSame(cell, Execute(new LocationByUniqueNameFunction([Text(other.Name.ToUpperInvariant())], world.World.Object)));
		Assert.IsNull(Execute(new LocationByUniqueNameFunction([Text(cell.Id.ToString())], world.World.Object)));
		Assert.IsTrue(cell.TrySetUniqueName("here", out _));
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(other);
		actor.SetupGet(x => x.Gameworld).Returns(world.World.Object);
		Assert.AreSame(other, RoomBuilderModule.LookupCell(actor.Object, "here"));
		Assert.AreSame(cell, Execute(new LocationByUniqueNameFunction([Text("here")], world.World.Object)));
		Assert.IsNull(Execute(new LocationByUniqueNameFunction([Text("missing")], world.World.Object)));
		Assert.AreSame(other, Execute(new ToLocationFunction([Text(other.Id.ToString())], world.World.Object, false)));
		Assert.IsTrue(cell.TrySetUniqueName("@1", out _));
		Assert.AreSame(cell, Execute(new LocationByUniqueNameFunction([Text("@1")], world.World.Object)));
		var previous = RoomBuilderModule.BuiltCells.ToArray();
		try
		{
			RoomBuilderModule.BuiltCells.Clear(); RoomBuilderModule.BuiltCells.Add(other);
			Assert.AreSame(other, RoomBuilderModule.LookupCell(world.World.Object, "@1"));
			Assert.AreSame(other, RoomBuilderModule.LookupCell(actor.Object, "@1"));
		}
		finally { RoomBuilderModule.BuiltCells.Clear(); RoomBuilderModule.BuiltCells.AddRange(previous); }
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
		var cells = world.Cells.Cast<Cell>().ToArray();
		Assert.IsTrue(cells[1].TrySetUniqueName("gate", out _));
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		actor.SetupGet(x => x.Gameworld).Returns(world.World.Object);
		actor.SetupGet(x => x.Location).Returns(cells[0]);
		var target = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		target.SetupGet(x => x.Id).Returns(777);
		target.SetupGet(x => x.Location).Returns(cells[2]);
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
		void Check(string input, ICell expected)
		{
			actor.Invocations.Clear();
			command.Invoke(null, [actor.Object, "goto " + input]);
			actor.Verify(x => x.TransferTo(It.Is<SpatialLocation>(location => ReferenceEquals(location.Cell, expected))), Times.Once);
		}
		Check("gate", cells[2]);
		Check("#gate", cells[1]);
		Check(cells[1].Id.ToString(), cells[1]);
		Check("#" + cells[1].Id, cells[1]);
		Check(cells[1].Name, cells[1]);
		Assert.IsTrue(cells[1].TrySetUniqueName(null, out _));
		Check("#" + cells[1].Name, cells[1]);
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
