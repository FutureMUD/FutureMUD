#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Health;
using MudSharp.Magic;
using Db = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CorpseBootTests
{
	[TestMethod]
	public void LoadWorldItems_SavedCorpse_ActivatesBodyWoundLodgedItemAfterCharacterLoading()
	{
		var options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options;
		using var database = new InMemoryDatabaseTestScope(options);
		using var session = new FMDB();
		var db = database.Context;
		var roomRow = new Db.Room { Id = 1, CurrentOverlayId = 1 };
		roomRow.RoomOverlays.Add(new Db.RoomOverlay { Id = 1 });
		db.Rooms.Add(roomRow);
		var corpseRow = new Db.GameItem { Id = 10, GameItemProtoId = 1 };
		corpseRow.RoomsGameItems.Add(new Db.RoomsGameItems { Room = roomRow });
		corpseRow.GameItemComponents.Add(new Db.GameItemComponent
		{
			GameItemComponentProtoId = 2,
			Definition = """
				<Definition><OriginalCharacter>5025</OriginalCharacter><OriginalBody>12</OriginalBody>
				<Model>1</Model><DecayPoints>0</DecayPoints><DecayState>0</DecayState>
				<TimeOfDeath>2026-01-01T00:00:00Z</TimeOfDeath></Definition>
				"""
		});
		db.GameItems.AddRange(corpseRow, new Db.GameItem { Id = 11, GameItemProtoId = 1 });
		db.Wounds.Add(new Db.Wound
		{
			Id = 13, BodyId = 12, LodgedItemId = 11, WoundType = "SimpleOrganic", ExtraInformation = "<Definition/>"
		});
		db.SaveChanges();
		db.ChangeTracker.Clear();

		using var world = new Futuremud(null);
		// Spell ownership has a separate database session. Neither fixture item has a spell claim.
		typeof(Futuremud).GetField("_spellOwnedItems", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(world, Mock.Of<ISpellOwnedItemService>());
		world.Add(Mock.Of<IGameItemProto>(x => x.Id == 1 && x.Name == "test item"));
		world.Add(world.GameItemComponentManager.GetProto(new Db.GameItemComponentProto
		{
			Id = 2, Type = "Corpse", Name = "corpse", EditableItem = new Db.EditableItem(), Definition = "<Definition/>"
		}, world)!);
		world.Add(Mock.Of<ICorpseModel>(x => x.Id == 1));
		var room = new Room(db.Rooms.Include(x => x.RoomOverlays).Single(), Mock.Of<IZone>(x => x.Gameworld == world));
		world.Add(room);
		world.SaveManager.MudBootingMode = true;
		world.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Disallowed);
		world.PrimeGameItems();
		((IFuturemudLoader)world).LoadWorldItems();
		var corpse = room.GameItems.Single();
		Assert.AreEqual(corpseRow.Id, corpse.Id);
		Assert.IsTrue(((GameItem)corpse).IsActive);
		Assert.IsNotNull(corpse.GetItemType<ICorpse>());
		Assert.IsFalse(world.Items.Any(x => x.Id == 11));

		world.SetCharacterMaterialisationBootPhase(CharacterMaterialisationBootPhase.Allowed);
		var owner = Mock.Of<ICharacter>(x => x.Id == 5025 && x.Name == "corpse owner");
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Id).Returns(12);
		body.SetupGet(x => x.Actor).Returns(owner);
		Mock.Get(owner).SetupGet(x => x.Bodies).Returns([body.Object]);
		var wounds = new Lazy<IWound[]>(() => db.Wounds.Where(x => x.BodyId == body.Object.Id)
			.AsEnumerable().Select(row => WoundFactory.LoadWound(row, owner, world, body.Object)).ToArray());
		body.SetupGet(x => x.Wounds).Returns(() => wounds.Value);
		world.Add(owner, true);
		world.FinalisePostCharacterLoadObjects();

		Assert.IsTrue(wounds.IsValueCreated, "Post-character finalisation must visit the body's wounds.");
		Assert.AreEqual(1, wounds.Value.Length);
		var lodged = (GameItem)world.Items.Single(x => x.Id == 11);
		Assert.IsTrue(lodged.IsActive);
		Assert.AreSame(body.Object, corpse.GetItemType<ICorpse>().OriginalBody);
		Assert.AreEqual(13L, corpse.Wounds.Single().Id);
		Assert.AreSame(lodged, corpse.LodgedItems.Single());
		Assert.AreSame(corpse, room.GameItems.Single());
	}
}
