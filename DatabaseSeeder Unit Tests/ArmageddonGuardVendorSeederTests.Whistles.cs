#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Database;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void HuntingWhistle_PreservesRaceRecognitionAndSixDirectionRange(bool halfling)
	{
		using var db = WhistleFixture(out var writer);
		InstallWhistle(writer, db);
		var (world, progs, _) = Runtime(db);
		var actor = Actor();
		var race = Item<MudSharp.Character.Heritage.IRace>(halfling ? 900 : 901, ProgVariableTypes.Race);
		actor.Setup(x => x.GetProperty("race")).Returns(race.Object);
		var whistle = Item<IGameItem>(39701, ProgVariableTypes.Item);
		actor.Setup(x => x.GetProperty("helditems")).Returns(new CollectionVariable(new[] { whistle.Object }, ProgVariableTypes.Item));
		actor.Setup(x => x.GetProperty("wieldeditems")).Returns(new CollectionVariable(Array.Empty<IGameItem>(), ProgVariableTypes.Item));
		var room = Item<IRoom>(1, ProgVariableTypes.Location);
		actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		room.SetupGet(x => x.Characters).Returns(new[] { actor.Object });
		room.SetupGet(x => x.Gameworld).Returns(world.Object);
		var known = Actor(); var stranger = Actor(); var asleep = Actor(); var deaf = Actor();
		asleep.Setup(x => x.GetProperty("isawake")).Returns(new BooleanVariable(false));
		var listeners = new[] { known, stranger, asleep, deaf };
		var check = new Mock<ICheck>();
		world.Setup(x => x.GetCheck(CheckType.GenericListenCheck)).Returns(check.Object);
		check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable?>(),
			It.IsAny<IUseTrait?>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericListenCheck, Outcome.Pass));
		var destination = Item<IRoom>(2, ProgVariableTypes.Location);
		destination.Setup(x => x.GetProperty("characters")).Returns(new CollectionVariable(listeners.Select(x => x.Object).ToList(), ProgVariableTypes.Character));
		foreach (var listener in listeners)
		{
			listener.Setup(x => x.GetProperty("race")).Returns(Item<MudSharp.Character.Heritage.IRace>(listener == stranger ? 901 : 900, ProgVariableTypes.Race).Object);
			listener.SetupGet(x => x.Location).Returns(destination.Object);
			listener.SetupGet(x => x.Gameworld).Returns(world.Object);
			listener.Setup(x => x.CanHear(actor.Object)).Returns(listener != deaf);
		}
		var passage = new Mock<IRoomExit>();
		passage.SetupGet(x => x.GetObject).Returns(passage.Object);
		passage.SetupGet(x => x.Type).Returns(ProgVariableTypes.Exit);
		passage.Setup(x => x.GetProperty("destination")).Returns(destination.Object);
		// One-way exits do not need an opposite exit to establish the source direction.
		passage.Setup(x => x.GetProperty("direction")).Returns(new TextVariable("North"));
		room.Setup(x => x.GetProperty("exits")).Returns(new CollectionVariable(new[] { passage.Object }, ProgVariableTypes.Exit));
		var prog = progs.GetByName("ArmageddonWorldfileHuntingWhistle");
		var hook = new HookOnInput(db.Hooks.Single(), world.Object);
		Assert.IsTrue(hook.Function(EventType.CommandInput, [actor.Object, whistle.Object, "whistle", new StringStack("")]));
		string[] Echoes(Mock<ICharacter> listener) => Mock.Get(listener.Object.OutputHandler).Invocations
			.SelectMany(x => x.Arguments.OfType<EmoteOutput>()).Select(x => x.DefaultEmote.RawText).ToArray();
		if (halfling)
		{
			CollectionAssert.AreEqual(new[] { "You hear the familiar sound of a hunting whistle from the south." }, Echoes(known));
			CollectionAssert.AreEqual(new[] { "You hear the long, slow whistle of an unfamiliar bird from nearby." }, Echoes(stranger));
		}
		else
		{
			Assert.AreEqual(0, Echoes(known).Length);
			Assert.AreEqual(0, Echoes(stranger).Length);
		}
		Assert.AreEqual(0, Echoes(asleep).Length);
		Assert.AreEqual(0, Echoes(deaf).Length);
		Assert.IsTrue(Echoes(actor).Single().Contains(halfling ? "whistling noise" : "warbling noise", StringComparison.Ordinal));
		foreach (var listener in listeners) Mock.Get(listener.Object.OutputHandler).Invocations.Clear();
		passage.Setup(x => x.GetProperty("direction")).Returns(new TextVariable("Northeast"));
		Assert.IsTrue(prog.ExecuteBool(actor.Object, whistle.Object, "whistle", new List<string>()));
		Assert.IsTrue(listeners.All(x => Echoes(x).Length == 0));
		actor.Setup(x => x.GetProperty("helditems")).Returns(new CollectionVariable(Array.Empty<IGameItem>(), ProgVariableTypes.Item));
		Assert.IsFalse(hook.Function(EventType.CommandInput, [actor.Object, whistle.Object, "whistle", new StringStack("")]));
		Assert.IsFalse(hook.Function(EventType.CommandInput, [actor.Object, whistle.Object, "look", new StringStack("")]));
	}

	[TestMethod]
	public void HuntingWhistle_InstallsOnlyActiveAttachmentsAndIncludesResetVariants()
	{
		using var db = WhistleFixture(out var writer);
		InstallWhistle(writer, db);
		Assert.AreEqual(1, db.Hooks.Count());
		foreach (var proto in db.GameItemProtos)
		{
			var onload = db.FutureProgs.Single(x => x.Id == proto.GameItemProtosOnLoadProgs.Single().FutureProgId);
			StringAssert.Contains(onload.FunctionText, $"addhook(@item, {db.Hooks.Single().Id})");
		}
		var definition = XElement.Parse(db.Hooks.Single().Definition);
		Assert.AreEqual("whistle", definition.Element("TargetCommand")!.Value);
		Assert.AreEqual("true", definition.Element("UseProgResultForHandling")!.Value);
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void HuntingWhistle_UnplacedOrUnattachedDefinitionStaysInactive(bool removeAttachment)
	{
		using var db = WhistleFixture(out var writer, removeAttachment);
		if (removeAttachment)
		{
			var source = Field<ArmageddonWorldfilePackage>(writer, "_source");
			Array.Fill(source.Items, source.Items[0] with { Programs = [] });
		}
		InstallWhistle(writer, db);
		Assert.AreEqual(0, db.Hooks.Count());
	}

	private static void InstallWhistle(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallHuntingWhistles", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static FuturemudDatabaseContext WhistleFixture(out ArmageddonWorldfileWriter writer, bool resetPlaced = true)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var source = new ArmageddonWorldfilePackage { Items = archive.Items.Where(x => x.Vnum == 39701).ToArray(),
			Resets = resetPlaced ? archive.Resets.Where(x => x.Arguments.Length > 1 && x.Arguments[1] == 39701 && x.ReferenceIssues.Length == 0).ToArray() : [] };
		Assert.IsTrue(!resetPlaced || source.Resets.Length > 0);
		db.Races.Add(new Db.Race { Id = 900, Name = "Halfling" });
		writer = new ArmageddonWorldfileWriter(db, source);
		var proto = new Db.GameItemProto { Id = 39701, Name = "bone whistle" };
		var variant = new Db.GameItemProto { Id = 139701, Name = "bone whistle reset variant" };
		db.GameItemProtos.AddRange(proto, variant);
		Field<Dictionary<int, Db.GameItemProto>>(writer, "_items").Add(39701, proto);
		Field<Dictionary<string, Db.GameItemProto>>(writer, "_resetItemVariants").Add("39701:fixture", variant);
		db.SaveChanges();
		return db;
	}
}
