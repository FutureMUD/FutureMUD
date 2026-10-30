#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using Moq;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonTravelRuneSeederTests
{
	[TestMethod]
	public void PlacedRunes_InstallSavedDetectorAnchorsAndOnloadsWithoutInventingOwnersOrDestinations()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		var package = ArmageddonWorldfileTestData.Load().ForInstallation();
		var runes = package.Items.Where(x => x.ItemType == 29).ToArray();
		var writer = new ArmageddonWorldfileWriter(db, new ArmageddonWorldfilePackage { Items = runes, Resets = package.Resets });
		var waystep = new Db.MagicSpell { Id = 980001, Name = "Waystep", MagicSchoolId = 1,
			Definition = "<Definition><Effects><Effect type=\"waystep\" version=\"1\"/></Effects></Definition>" };
		db.MagicSpells.Add(waystep);
		db.SeederManagedRecords.Add(new Db.SeederManagedRecord { Seeder = "Fixture", StableKey = "arm.spell.waystep",
			EntityType = nameof(Db.MagicSpell), LogicalId = waystep.Id });
		var effective = Field<Dictionary<long, (ArmWorldItem Source, int[] Values)>>(writer, "_effectiveItems");
		var prototypes = new Dictionary<int, Db.GameItemProto>();
		foreach (var rune in runes)
		{
			var proto = new Db.GameItemProto { Id = 990000 + rune.Vnum, RevisionNumber = 0, Name = rune.Name };
			db.GameItemProtos.Add(proto);
			prototypes.Add(rune.Vnum, proto);
			effective.Add(proto.Id, (rune, rune.Values));
		}
		var variant = new Db.GameItemProto { Id = 1100000, RevisionNumber = 0, Name = "Variant rune" };
		db.GameItemProtos.Add(variant);
		effective.Add(variant.Id, (runes.Single(x => x.Vnum == 17032), [123, 456, 789, 0, 0, 0]));
		var stock = new Db.GameItem { Id = 1100001, GameItemProtoId = variant.Id, GameItemProtoRevision = 0,
			EffectData = "<Effects><Effect><Type>AdminInvis</Type></Effect></Effects>" };
		db.GameItems.Add(stock);
		db.SaveChanges();
		typeof(ArmageddonWorldfileWriter).GetMethod("InstallPlacedTravelRunes", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(writer, null);
		db.SaveChanges();
		var expected = new[] { 17032, 65900, 65902, 65903, 65904, 65905, 65906, 65907 };
		Assert.AreEqual("false", XElement.Parse(waystep.Definition).Element("Effects")!.Element("Effect")!.Element("RuneUsesPoolRestrictions")?.Value);
		foreach (var vnum in expected) Assert.AreEqual(1, prototypes[vnum].GameItemProtosOnLoadProgs.Count, $"Vnum{vnum}");
		Assert.AreEqual(0, prototypes[65901].GameItemProtosOnLoadProgs.Count, "Unplaced definition remains unactivated.");
		Assert.AreEqual(1, variant.GameItemProtosOnLoadProgs.Count);
		var saved = XElement.Parse(stock.EffectData);
		Assert.AreEqual(2, saved.Elements("Effect").Count());
		var anchorXml = saved.Elements("Effect").Single(x => (string?)x.Element("Type") == "PublicTravelAnchor");
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<MudSharp.FutureProg.IFutureProg>());
		var item = Mock.Of<IGameItem>(x => x.Id == stock.Id && x.Gameworld == world.Object);
		PublicTravelAnchor.InitialiseEffectType();
		var effect = (PublicTravelAnchor)Effect.LoadEffect(anchorXml, item);
		Assert.AreEqual(PerceptionTypes.SenseMagical, effect.RequiredPerception);
		Assert.AreEqual(stock.Id, effect.ItemId);
		Assert.IsNull(anchorXml.Descendants("OwnerId").SingleOrDefault());
		Assert.IsNull(anchorXml.Descendants("RoomId").SingleOrDefault());
		var progs = db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray();
		using var compiler = new OfflineProgCompilation(progs);
		foreach (var prog in progs.Where(x => x.FunctionName.Contains("PublicTravelRune", StringComparison.Ordinal)))
			compiler.Compile(prog.Id);
	}

	private static T Field<T>(object target, string name) =>
		(T)target.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(target)!;
}
