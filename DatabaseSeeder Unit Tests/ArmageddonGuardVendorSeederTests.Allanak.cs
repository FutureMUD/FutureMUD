#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Commands;
using MudSharp.Commands.Trees;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.PerceptionEngine;
using MudSharp.Planes;
using MudSharp.RPG.Law;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

public partial class ArmageddonGuardVendorSeederTests
{
	[TestMethod]
	public void AllanakJustice_UnifiesCityAndMineButExcludesPlayerHousesAndKeepsJailerAtItsPost()
	{
		using var db = AllanakFixture(out var writer);
		InstallAllanak(writer, db);
		var authority = db.LegalAuthorities.Single();
		Assert.AreEqual("Allanak Militia", authority.Name);
		CollectionAssert.AreEquivalent(new long[] { 1050, 1066 }, authority.LegalAuthoritiesZones.Select(x => x.ZoneId).ToArray());
		var area = db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileLawAreaallanak").FunctionText;
		StringAssert.Contains(area, "150821"); StringAssert.Contains(area, "150535"); StringAssert.Contains(area, "166001");
		Assert.IsFalse(area.Contains("113920"), "Source reset file ownership must not annex player-house rooms.");
		Assert.AreEqual(150535L, authority.PrisonReleaseLocationId);
		var spice = db.Laws.Single(x => x.Name == "Spice Smuggling");
		Assert.AreEqual(18000.0, spice.ActivePeriod);
		Assert.IsFalse(spice.CanBeAppliedAutomatically, "Ordinary border crossing must not bypass the checkpoint's probabilistic search.");
		Assert.AreEqual(EnforcementStrategy.ArrestAndDetain.ToString(), spice.EnforcementStrategy);
		StringAssert.Contains(spice.PunishmentStrategy, "<DefaultFineAmount>10</DefaultFineAmount>");
		Assert.IsFalse(db.LegalClasses.Single().CanBeDetainedUntilFinesPaid);
		foreach (var id in new long[] { 50607, 150607 })
		{
			var jailer = db.NpcTemplates.Single(x => x.Id == id);
			Assert.AreEqual(0, jailer.NpctemplatesArtificalIntelligences.Count, "Receiving prisoners must not enrol the jailer into a roaming patrol.");
			var hooks = XElement.Parse(jailer.Definition).Element("TemplateLoadAdditions")!.Element("Hooks")!.Elements("Hook").ToArray();
			Assert.AreEqual(1, hooks.Length);
			var hook = db.Hooks.Single(x => x.Id == (long)hooks[0].Attribute("id")!);
			Assert.AreEqual("release", XElement.Parse(hook.Definition).Element("TargetCommand")!.Value);
		}
		var role = db.EnforcementAuthorities.Single(x => x.Name == "Jailer");
		Assert.IsFalse(role.CanAccuse || role.CanConvict || role.CanForgive);
		Assert.AreEqual(1, role.EnforcementAuthoritiesArrestableLegalClasses.Count);
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).ToArray());
		foreach (var prog in db.FutureProgs.ToArray())
		{
			try { compiler.Compile(prog.Id); }
			catch (InvalidOperationException exception)
			{
				throw new InvalidOperationException(exception.Message + "\n" + string.Join("\n",
					prog.FunctionText.Split('\n').Select((line, index) => $"{index + 1}: {line}")), exception);
			}
		}
	}

	[TestMethod]
	public void SpiceSearch_UsesEffectiveVariantPowerAndClosedUnlockedContainersButNotLockedContents()
	{
		using var db = AllanakFixture(out var writer); InstallAllanak(writer, db);
		var (_, progs, _) = Runtime(db);
		var guard = Actor();
		guard.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var baseSpice = SpiceItem(50264, 50264); var altered = SpiceItem(250264, 250264); var smoking = SpiceItem(150264, 150264);
		var power = progs.GetByName("ArmageddonWorldfileSpicePower");
		Assert.AreEqual(13m, power.Execute(baseSpice.Object));
		Assert.AreEqual(17m, power.Execute(altered.Object));
		Assert.AreEqual(0m, power.Execute(smoking.Object));
		var bag = SpiceItem(999, 999);
		bag.Setup(x => x.GetProperty("iscontainer")).Returns(new BooleanVariable(true));
		bag.Setup(x => x.GetProperty("contents")).Returns(new CollectionVariable(new[] { altered.Object }, ProgVariableTypes.Item));
		var search = progs.GetByName("ArmageddonWorldfileSpiceSearch");
		// A supplied parent chance of 150 makes the child chance exactly 100,
		// allowing deterministic verification of recursion without overriding RNG.
		Assert.AreEqual(17m, search.Execute(guard.Object, new[] { bag.Object }, 150));
		bag.Setup(x => x.GetProperty("locked")).Returns(new BooleanVariable(true));
		Assert.AreEqual(0m, search.Execute(guard.Object, new[] { bag.Object }, 150));
		guard.Setup(x => x.CanSee(altered.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		Assert.AreEqual(0m, search.Execute(guard.Object, new[] { altered.Object }, 100));
	}

	[TestMethod]
	[DataRow(4, false, false)]
	[DataRow(13, true, false)]
	[DataRow(13, true, true)]
	public void SpiceCheckpoint_AllowsFirstDetectionThenRefusesWantedTravellerAndLeavesUnattachedCommandsAlone(int power, bool detected, bool mounted)
	{
		using var db = AllanakFixture(out var writer); InstallAllanak(writer, db);
		// Isolate the input interaction from random searches; the real recursive search
		// and item bindings are executed separately above.
		db.FutureProgs.Single(x => x.FunctionName == "ArmageddonWorldfileSpiceSearch").FunctionText = $"return @items.Count(item, true) * {power}";
		db.SaveChanges();
		var (world, progs, _) = Runtime(db);
		var actor = Actor(); var guard = Actor();
		var room = Item<IRoom>(150821, ProgVariableTypes.Location);
		room.Setup(x => x.GetProperty("zone")).Returns(Item<IZone>(1050, ProgVariableTypes.Zone).Object);
		room.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		guard.Setup(x => x.GetProperty("location")).Returns(room.Object);
		actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var body = new Mock<IBody>();
		body.As<IHavePlanarPresence>().SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.Manifested(Mock.Of<IPlane>(x => x.Id == 1)));
		body.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(null)).Returns([]);
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.Merits).Returns([]);
		actor.Setup(x => x.EffectsOfType<IPlanarOverlayEffect>(null)).Returns([]);
		var inventoryIndex = 40L;
		foreach (var name in new[] { "wornitems", "wieldeditems", "helditems" })
			actor.Setup(x => x.GetProperty(name)).Returns(new CollectionVariable(
				mounted ? Array.Empty<IGameItem>() : new[] { SpiceItem(inventoryIndex++, 50264).Object }, ProgVariableTypes.Item));
		if (mounted)
		{
			var mount = Actor();
			mount.Setup(x => x.GetProperty("wornitems")).Returns(new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item));
			mount.Setup(x => x.GetProperty("wieldeditems")).Returns(new CollectionVariable(new List<IGameItem>(), ProgVariableTypes.Item));
			mount.Setup(x => x.GetProperty("helditems")).Returns(new CollectionVariable(new[] { SpiceItem(55, 50264).Object }, ProgVariableTypes.Item));
			actor.Setup(x => x.GetProperty("mount")).Returns(mount.Object);
		}
		guard.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var authority = Item<ILegalAuthority>(db.LegalAuthorities.Single().Id, ProgVariableTypes.LegalAuthority);
		var law = Item<ILaw>(db.Laws.Single(x => x.Name == "Spice Smuggling").Id, ProgVariableTypes.Law);
		law.SetupGet(x => x.Authority).Returns(authority.Object);
		law.SetupGet(x => x.EnforcementStrategy).Returns(EnforcementStrategy.ArrestAndDetain);
		authority.SetupGet(x => x.Laws).Returns(new[] { law.Object });
		var authorities = new All<ILegalAuthority>(); authorities.Add(authority.Object); world.SetupGet(x => x.LegalAuthorities).Returns(authorities);
		var known = new List<ICrime>(); authority.Setup(x => x.KnownCrimesForIndividual(actor.Object)).Returns(known);
		var crime = Item<ICrime>(1, ProgVariableTypes.Crime); crime.SetupGet(x => x.Law).Returns(law.Object);
		crime.SetupGet(x => x.CriminalIdentityIsKnown).Returns(true);
		authority.Setup(x => x.ReportWitnessedCrime(law.Object, actor.Object, guard.Object, null, null, It.IsAny<string>()))
			.Callback(() => known.Add(crime.Object)).Returns(crime.Object);
		var input = progs.GetByName("ArmageddonWorldfileSpiceCheckpointeast");
		bool Attempt(string command) => input.ExecuteBool(actor.Object, guard.Object, command, new List<string>());
		// commandname resolves through the actor's native command tree.
		UseNativeMovementMetadata(actor);
		Assert.IsFalse(Attempt("east"));
		if (!detected)
		{
			authority.Verify(x => x.ReportWitnessedCrime(It.IsAny<ILaw>(), It.IsAny<ICharacter>(), It.IsAny<ICharacter>(),
				It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<string>()), Times.Never);
			return;
		}
		authority.Verify(x => x.ReportWitnessedCrime(law.Object, actor.Object, guard.Object, null, null, It.IsAny<string>()), Times.Once);
		Assert.IsTrue(Attempt("east"));
		Assert.IsTrue(Attempt("e"));
		Assert.IsTrue(progs.GetByName("ArmageddonWorldfileSpiceCheckpointnorth").ExecuteBool(actor.Object, guard.Object, "n", new List<string>()));
		Assert.IsTrue(progs.GetByName("ArmageddonWorldfileSpiceCheckpointsouth").ExecuteBool(actor.Object, guard.Object, "s", new List<string>()));
		Assert.IsFalse(Attempt("west")); Assert.IsFalse(Attempt("open"));
		authority.Verify(x => x.ReportWitnessedCrime(law.Object, actor.Object, guard.Object, null, null, It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void JailerHandover_RequiresCorrectVisibleAddresseeAndActualControlThenUsesTheConfiguredAuthority()
	{
		using var db = AllanakFixture(out var writer); InstallAllanak(writer, db);
		var (world, progs, _) = Runtime(db);
		var holder = Actor(); var prisoner = Actor(); var jailer = Actor();
		holder.SetupGet(x => x.Id).Returns(10); prisoner.SetupGet(x => x.Id).Returns(20); jailer.SetupGet(x => x.Id).Returns(30);
		var room = Item<IRoom>(150535, ProgVariableTypes.Location); var zone = Item<IZone>(1050, ProgVariableTypes.Zone);
		room.SetupGet(x => x.Zone).Returns(zone.Object); room.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		foreach (var actor in new[] { holder, prisoner, jailer })
		{
			actor.SetupGet(x => x.Gameworld).Returns(world.Object); actor.SetupGet(x => x.Location).Returns(room.Object);
			actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			actor.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		}
		holder.Setup(x => x.TargetActor("prisoner", PerceiveIgnoreFlags.None)).Returns(prisoner.Object);
		holder.Setup(x => x.TargetActor("jailer", PerceiveIgnoreFlags.None)).Returns(jailer.Object);
		var body = new Mock<IBody>(); prisoner.SetupGet(x => x.Body).Returns(body.Object);
		var grapple = new Mock<IGrappling>(); var subdued = new Mock<IBeingGrappled>();
		grapple.SetupGet(x => x.CharacterOwner).Returns(holder.Object); subdued.SetupGet(x => x.Grappling).Returns(grapple.Object);
		subdued.Setup(x => x.Applies()).Returns(true); subdued.SetupGet(x => x.UnderControl).Returns(true);
		body.Setup(x => x.EffectsOfType<IBeingGrappled>(null)).Returns(new[] { subdued.Object });
		var held = new List<IGrappling>(); holder.Setup(x => x.EffectsOfType<IGrappling>(null)).Returns(held);
		prisoner.Setup(x => x.CombinedEffectsOfType<MudSharp.Effects.Concrete.Dragging.DragTarget>()).Returns([]);
		holder.Setup(x => x.EffectsOfType<MudSharp.Effects.Concrete.Dragging>(null)).Returns([]);
		holder.Setup(x => x.RemoveAllEffects<IGrappling>(It.IsAny<Predicate<IGrappling>>(), true))
			.Callback(() => { held.Clear(); subdued.SetupGet(x => x.UnderControl).Returns(false); }).Returns(true);
		var authority = Item<ILegalAuthority>(db.LegalAuthorities.Single().Id, ProgVariableTypes.LegalAuthority);
		var authorities = new All<ILegalAuthority>(); authorities.Add(authority.Object); world.SetupGet(x => x.LegalAuthorities).Returns(authorities);
		authority.SetupGet(x => x.Gameworld).Returns(world.Object); authority.SetupGet(x => x.EnforcementZones).Returns(new[] { zone.Object });
		authority.SetupGet(x => x.PrisonerBelongingsStorageLocation).Returns(room.Object); authority.SetupGet(x => x.RoomLocations).Returns(new[] { room.Object });
		var legalClass = Mock.Of<ILegalClass>(); var role = Mock.Of<IEnforcementAuthority>(x => x.ArrestableClasses == new[] { legalClass });
		authority.Setup(x => x.GetLegalClass(prisoner.Object)).Returns(legalClass);
		authority.Setup(x => x.GetEnforcementAuthority(jailer.Object)).Returns(role);
		var known = new List<ICrime>(); authority.Setup(x => x.KnownCrimesForIndividual(prisoner.Object)).Returns(known);
		var hook = progs.GetByName("ArmageddonWorldfileAllanakJailerHandover");
		bool Release(params string[] args) => hook.ExecuteBool(holder.Object, jailer.Object, "release", args.ToList());
		Assert.IsFalse(Release("prisoner", "other")); Assert.IsFalse(Release("prisoner", "jailer"));
		held.Add(grapple.Object);
		Assert.IsTrue(Release("prisoner", "jailer"));
		jailer.Verify(x => x.ExecuteCommand("say That person is not a criminal."), Times.Once);
		var law = Mock.Of<ILaw>(x => x.EnforcementStrategy == EnforcementStrategy.ArrestAndDetain);
		var crime = Item<ICrime>(1, ProgVariableTypes.Crime);
		crime.SetupGet(x => x.Law).Returns(law);
		crime.SetupGet(x => x.CriminalIdentityIsKnown).Returns(true);
		known.Add(crime.Object);
		Assert.IsTrue(Release("prisoner", "jailer"));
		authority.Verify(x => x.IncarcerateCriminal(prisoner.Object), Times.Once);
	}

	[TestMethod]
	public void NorthwardHorn_UsesNativeMoveRegistrationAndLeavesOtherDirectionsAlone()
	{
		using var db = AllanakFixture(out var writer);
		var source = Field<ArmageddonWorldfilePackage>(writer, "_source");
		var index = Array.FindIndex(source.Npcs, x => x.Vnum == 50028);
		source.Npcs[index] = source.Npcs[index] with { Programs = ["C north gypsy_hornblower"] };
		var clan = new Db.Clan { Id = 1024, Name = "Gypsies" }; db.Clans.Add(clan);
		Field<Dictionary<int, Db.Clan>>(writer, "_clans").Add(24, clan);
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallHornblowers", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges(); var (_, progs, _) = Runtime(db);
		var actor = Actor(); var guard = Actor(); UseNativeMovementMetadata(actor);
		var room = Item<IRoom>(150821, ProgVariableTypes.Location); var zone = Item<IZone>(1050, ProgVariableTypes.Zone);
		var output = new Mock<IOutputHandler>(); actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		zone.SetupGet(x => x.Characters).Returns(new[] { actor.Object });
		room.Setup(x => x.GetProperty("zone")).Returns(zone.Object); actor.Setup(x => x.GetProperty("location")).Returns(room.Object);
		guard.Setup(x => x.CanSee(actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		var horn = progs.GetByName("ArmageddonWorldfileNorthwardHorn");
		foreach (var command in new[] { "north", "n", "east", "west" })
			Assert.IsFalse(horn.ExecuteBool(actor.Object, guard.Object, command, new List<string>()));
		Assert.AreEqual(2, output.Invocations.Count(x => x.Method.Name == "Send"));
	}

	private static void UseNativeMovementMetadata(Mock<ICharacter> actor)
	{
		var commands = new CharacterCommandManager();
		var movement = typeof(MudSharp.FutureProg.FutureProg).Assembly.GetType("MudSharp.Commands.Modules.MovementModule")!
			.GetMethod("Move", BindingFlags.Static | BindingFlags.NonPublic)!;
		var metadata = movement.GetCustomAttributes().Single(x => x.GetType().Name == "PlayerCommand");
		var moveName = (string)metadata.GetType().GetProperty("Name")!.GetValue(metadata)!;
		var moveWords = (IEnumerable<string>)metadata.GetType().GetProperty("CommandWords")!.GetValue(metadata)!;
		Assert.AreEqual("Move", moveName);
		commands.Add(moveWords, new Command<ICharacter>((_, _) => { }, states: CharacterState.Awake, name: moveName));
		commands.Add(new[] { "open" }, new Command<ICharacter>((_, _) => { }, states: CharacterState.Awake, name: "Open"));
		actor.SetupGet(x => x.CommandTree).Returns(Mock.Of<ICharacterCommandTree>(x => x.Commands == commands));
		actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
	}

	private static Mock<IGameItem> SpiceItem(long id, long proto)
	{
		var item = Item<IGameItem>(id, ProgVariableTypes.Item);
		item.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(proto));
		item.Setup(x => x.GetProperty("iscontainer")).Returns(new BooleanVariable(false));
		item.Setup(x => x.GetProperty("locked")).Returns(new BooleanVariable(false));
		return item;
	}

	private static void InstallAllanak(ArmageddonWorldfileWriter writer, FuturemudDatabaseContext db)
	{
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallJustice", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
	}

	private static FuturemudDatabaseContext AllanakFixture(out ArmageddonWorldfileWriter writer)
	{
		var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var archive = ArmageddonWorldfileTestData.Load().ForInstallation();
		var mine = archive.Npcs.First(x => x.Programs.Contains("C pulse allanak_soldier"));
		var npcVnums = new[] { 50007, 50027, 50028, 50607, mine.Vnum }.Distinct().ToArray();
		var positions = new[] { (50007, 50803, 50), (50027, 13920, 50), (50028, 50821, 50), (50607, 50535, 50), (mine.Vnum, 66001, 66) };
		var source = new ArmageddonWorldfilePackage { Npcs = archive.Npcs.Where(x => npcVnums.Contains(x.Vnum)).ToArray(),
			Rooms = archive.Rooms.Where(x => positions.Any(p => p.Item2 == x.Vnum)).ToArray(),
			Resets = positions.Select((p, i) => new ArmWorldReset(new("fixture.cmd", i + 1), p.Item3, "M", [0, p.Item1, 1, p.Item2], [], [])).ToArray() };
		writer = new ArmageddonWorldfileWriter(db, source);
		foreach (var id in new[] { 12, 17, 22, 23, 36 })
		{
			var clan = new Db.Clan { Id = 1000 + id, Name = "Clan " + id }; db.Clans.Add(clan);
			Field<Dictionary<int, Db.Clan>>(writer, "_clans").Add(id, clan);
		}
		db.Clocks.Add(new Db.Clock { Id = 20, Definition = "<Clock><InGameSecondsPerRealSecond>6</InGameSecondsPerRealSecond></Clock>" });
		db.Calendars.Single().FeedClockId = 20;
		var currency = new Db.Currency { Id = 20, Name = "Obsidian" }; db.Currencies.Add(currency);
		typeof(ArmageddonWorldfileWriter).GetField("_currency", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, currency);
		var package = new Db.RoomOverlayPackage { Id = 20, Name = "Imported", RevisionNumber = 0 }; db.RoomOverlayPackages.Add(package);
		typeof(ArmageddonWorldfileWriter).GetField("_overlays", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(writer, package);
		db.Terrains.Add(new Db.Terrain { Id = 20, Name = "Urban Street" });
		foreach (var number in new[] { 13, 50, 66 })
		{
			var zone = new Db.Zone { Id = 1000 + number, Name = "Source zone " + number }; db.Zones.Add(zone);
			Field<Dictionary<int, Db.Zone>>(writer, "_zones").Add(number, zone);
		}
		foreach (var sourceRoom in source.Rooms)
		{
			var room = new Db.Room { Id = 100000 + sourceRoom.Vnum, ZoneId = 1000 + sourceRoom.Zone, UniqueName = "Vnum" + sourceRoom.Vnum, EffectData = "<Effects/>" };
			room.RoomOverlays.Add(new Db.RoomOverlay { Id = room.Id, RoomId = room.Id, RoomName = sourceRoom.Name });
			db.Rooms.Add(room); Field<Dictionary<int, Db.Room>>(writer, "_rooms").Add(sourceRoom.Vnum, room);
		}
		foreach (var npc in source.Npcs)
		{
			var cash = new Db.FutureProg { Id = 200000 + npc.Vnum, FunctionName = "ArmageddonWorldfileNpcCash" + npc.Vnum,
				FunctionText = "return true", ReturnTypeDefinition = ProgVariableTypes.Boolean.ToStorageString() };
			cash.FutureProgsParameters.Add(new Db.FutureProgsParameter { ParameterIndex = 0, ParameterName = "npc", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(cash); Field<List<Db.FutureProg>>(writer, "_progs").Add(cash);
			var definition = $"<Definition><OnLoadProg>{cash.Id}</OnLoadProg><TemplateLoadAdditions><Hooks/></TemplateLoadAdditions></Definition>";
			var template = new Db.NpcTemplate { Id = npc.Vnum, Definition = definition }; var variant = new Db.NpcTemplate { Id = npc.Vnum + 100000, Definition = definition };
			db.NpcTemplates.AddRange(template, variant); Field<Dictionary<int, Db.NpcTemplate>>(writer, "_npcs").Add(npc.Vnum, template);
			Field<Dictionary<int, long[]>>(writer, "_npcFamilyIds").Add(npc.Vnum, [template.Id, variant.Id]);
		}
		var spice = archive.Items.Single(x => x.Vnum == 50264);
		var bindings = Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems");
		foreach (var entry in new[] { (50264L, 13, 0), (150264L, 50, 1), (250264L, 17, 0) })
			bindings.Add(entry.Item1, (spice, [entry.Item2, 0, entry.Item3, 0, 0, 0]));
		db.SaveChanges(); return db;
	}
}
