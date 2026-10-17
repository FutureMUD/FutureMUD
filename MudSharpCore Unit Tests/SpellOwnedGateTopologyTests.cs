#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedGateTopologyTests
{
	[TestMethod]
	public void GateBirth_ClaimsExactlyTwoRoomsTwoOverlaysAndThreePairedExits()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var creation = new SpellOwnedCreation(context, 42);
		var south = new Room { Temporary = true, ZoneId = 1, EffectData = "<Effects />" };
		var north = new Room { Temporary = true, ZoneId = 2, EffectData = "<Effects />" };
		var southOverlay = new RoomOverlay();
		var northOverlay = new RoomOverlay();
		creation.StageGateTopology(south, southOverlay, north, northOverlay, 100, 101, 200, 201);
		creation.ValidateBeforeSave();
		context.SaveChanges();
		creation.FinishGateTopology();
		var claims = creation.SavedClaims();
		Assert.AreEqual(2, claims.Count(x => x.Kind == SpellOwnedEntityKind.Room));
		Assert.AreEqual(2, claims.Count(x => x.Kind == SpellOwnedEntityKind.RoomOverlay));
		Assert.AreEqual(3, claims.Count(x => x.Kind == SpellOwnedEntityKind.Exit));
		Assert.AreEqual(1, claims.Count(x => x.Role == SpellOwnedEntityRole.GateSourcePortal));
		Assert.AreEqual(1, claims.Count(x => x.Role == SpellOwnedEntityRole.GateCorridor));
		Assert.AreEqual(1, claims.Count(x => x.Role == SpellOwnedEntityRole.GateDestinationPortal));
		Assert.AreEqual(southOverlay.Id, south.CurrentOverlayId);
		Assert.AreEqual(northOverlay.Id, north.CurrentOverlayId);
		Assert.AreEqual(6, context.RoomOverlaysExits.Count());
		Assert.IsTrue(context.Exits.Any(x => x.RoomId1 == south.Id && x.RoomId2 == north.Id &&
			x.Direction1 == (int)MudSharp.Construction.CardinalDirection.North &&
			x.Direction2 == (int)MudSharp.Construction.CardinalDirection.South));
		Assert.IsTrue(context.Exits.Any(x => x.RoomId1 == 100 && x.RoomId2 == south.Id));
		Assert.IsTrue(context.Exits.Any(x => x.RoomId1 == north.Id && x.RoomId2 == 200));
	}

	[TestMethod]
	public void GateBirth_RejectsOrdinaryShelterTopologyInSameTransaction()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var creation = new SpellOwnedCreation(context, 42);
		creation.StageGateTopology(new Room { Temporary = true }, new RoomOverlay(),
			new Room { Temporary = true }, new RoomOverlay(), 100, 101, 200, 201);
		Assert.ThrowsException<InvalidOperationException>(() =>
			creation.StageShelterTopology(new Room { Temporary = true }, new RoomOverlay(), 100, 101, "shelter"));
	}

	private static string SavedMark(Guid identity, long itemId, int uses) =>
		new XElement("Effects", new XElement("Effect", new XElement("Type", "MagicSpellParent"),
			new XElement("Effect", new XElement("Identity", identity), new XElement("Spell", 81),
				new XElement("Children", new XElement("Effect", new XElement("Type", "SpellMagicTag"),
					new XElement("Effect", new XElement("Tag", ArmageddonSimpleStockContent.ArcaneMarkTag),
						new XElement("ItemId", itemId), new XElement("SavedRoomId", 200),
						new XElement("RemainingUses", uses))))))).ToString(SaveOptions.DisableFormatting);

	[TestMethod]
	public void GateBirth_DebitsExactPersistedDirectMarkOnceAndRejectsStaleReplay()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var identity = Guid.NewGuid();
		var item = new GameItem { Id = 51, EffectData = SavedMark(identity, 51, 3) };
		context.GameItems.Add(item);
		context.BodiesGameItems.Add(new BodiesGameItems { BodyId = 91, GameItemId = 51 });
		context.SaveChanges();
		var creation = new SpellOwnedCreation(context, 42);
		creation.StageGateTopology(new Room { Temporary = true }, new RoomOverlay(),
			new Room { Temporary = true }, new RoomOverlay(), 100, 101, 200, 201);
		creation.DebitMarkedRoomUse(51, 91, identity, 81, 200, 3);
		creation.ValidateBeforeSave();
		Assert.AreEqual(2, (int)XElement.Parse(item.EffectData).Descendants("RemainingUses").Single());
		Assert.ThrowsException<InvalidOperationException>(() =>
			creation.DebitMarkedRoomUse(51, 91, identity, 81, 200, 3));
		Assert.AreEqual(2, (int)XElement.Parse(item.EffectData).Descendants("RemainingUses").Single());
	}

	[TestMethod]
	public void GateBirth_RefusesMarkAfterDirectCustodyChanges()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var identity = Guid.NewGuid();
		var item = new GameItem { Id = 51, EffectData = SavedMark(identity, 51, 1) };
		context.GameItems.Add(item); context.SaveChanges();
		var creation = new SpellOwnedCreation(context, 42);
		creation.StageGateTopology(new Room { Temporary = true }, new RoomOverlay(),
			new Room { Temporary = true }, new RoomOverlay(), 100, 101, 200, 201);
		Assert.ThrowsException<InvalidOperationException>(() =>
			creation.DebitMarkedRoomUse(51, 91, identity, 81, 200, 1));
		Assert.AreEqual(1, (int)XElement.Parse(item.EffectData).Descendants("RemainingUses").Single());
	}

	[TestMethod]
	public void GateBirth_FinalUseRemovesTheExactPersistedMarkParent()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var identity = Guid.NewGuid();
		var item = new GameItem { Id = 51, EffectData = SavedMark(identity, 51, 1) };
		context.GameItems.Add(item);
		context.BodiesGameItems.Add(new BodiesGameItems { BodyId = 91, GameItemId = 51 });
		context.SaveChanges();
		var creation = new SpellOwnedCreation(context, 42);
		creation.StageGateTopology(new Room { Temporary = true }, new RoomOverlay(),
			new Room { Temporary = true }, new RoomOverlay(), 100, 101, 200, 201);
		creation.DebitMarkedRoomUse(51, 91, identity, 81, 200, 1);
		creation.ValidateBeforeSave();
		Assert.AreEqual(0, XElement.Parse(item.EffectData).Elements("Effect").Count());
		Assert.ThrowsException<InvalidOperationException>(() =>
			creation.DebitMarkedRoomUse(51, 91, identity, 81, 200, 1));
	}
}
