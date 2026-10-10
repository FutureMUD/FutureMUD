#nullable enable

using System.Reflection;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using Room = MudSharp.Models.Room;

internal static class OwnedProjectionFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var seed = db.Rooms.Include(x => x.RoomOverlays).Single(x => x.Id == 1);
		var overlaySeed = seed.RoomOverlays.Single(x => x.Id == seed.CurrentOverlayId);
		Room Room(int i)
		{
			var key = $"QA N18 Room {i}";
			if (db.Rooms.SingleOrDefault(x => x.UniqueName == key) is { } existing) return existing;
			var row = new Room { ZoneId = seed.ZoneId, EffectData = "<Effects />", UniqueName = key };
			var overlay = Copy(overlaySeed); overlay.Id = 0; overlay.RoomId = 0; overlay.Room = row;
			overlay.RoomName = key; overlay.RoomDescription = "A stable room in the disposable projection qualification chain.";
			overlay.SafeQuit = true; overlay.OutdoorsType = (int)RoomOutdoorsType.Indoors;
			db.Rooms.Add(row); db.RoomOverlays.Add(overlay); db.SaveChanges(); row.CurrentOverlayId = overlay.Id; db.SaveChanges(); return row;
		}
		var rooms = Enumerable.Range(0, 3).Select(Room).ToArray();
		for (var i = 0; i < 2; i++)
		{
			var from = rooms[i]; var to = rooms[i + 1];
			if (db.Exits.Any(x => x.RoomId1 == from.Id && x.RoomId2 == to.Id)) continue;
			var exit = new Exit { RoomId1 = from.Id, RoomId2 = to.Id, Direction1 = (int)CardinalDirection.North, Direction2 = (int)CardinalDirection.South,
				TimeMultiplier = 1, MaximumSizeToEnter = 100, MaximumSizeToEnterUpright = 100,
				Keywords1 = "north", Keywords2 = "south", InboundDescription1 = "", InboundDescription2 = "", OutboundDescription1 = "", OutboundDescription2 = "",
				InboundTarget1 = "", InboundTarget2 = "", OutboundTarget1 = "", OutboundTarget2 = "", Verb1 = "", Verb2 = "", PrimaryKeyword1 = "north", PrimaryKeyword2 = "south", BlockedLayers = "" };
			db.Exits.Add(exit); db.RoomOverlaysExits.Add(new() { Exit = exit, RoomOverlayId = from.CurrentOverlayId!.Value });
			db.RoomOverlaysExits.Add(new() { Exit = exit, RoomOverlayId = to.CurrentOverlayId!.Value }); db.SaveChanges();
		}
		var plane = db.Planes.SingleOrDefault(x => x.Alias == "qan18shadow");
		if (plane is null)
		{
			plane = new() { Name = "QA N18 Shadow", Alias = "qan18shadow", Description = "A qualification shadow plane.", RoomDescriptionAddendum = "Shadows surround you.",
				RoomNameFormat = "{0}", RemoteObservationTag = "shadow", DisplayOrder = 90, IsDefault = false };
			db.Planes.Add(plane); db.SaveChanges();
		}
		var proto = db.GameItemProtos.SingleOrDefault(x => x.UniqueName == "QA N18 Effigy");
		if (proto is null)
		{
			var itemSeed = db.GameItemProtos.Include(x => x.EditableItem).OrderBy(x => x.Id).First();
			var holdable = db.GameItemComponentProtos.Include(x => x.EditableItem).First(x => x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
			proto = Copy(itemSeed); proto.Id = db.GameItemProtos.Max(x => x.Id) + 1; proto.RevisionNumber = 0; proto.Name = "QA N18 Effigy"; proto.UniqueName = proto.Name;
			proto.ShortDescription = "a qasandeffigy figurine"; proto.FullDescription = "A small sand figurine bound to an owned projection."; proto.Keywords = "qasandeffigy figurine";
			proto.ReadOnly = false; proto.MorphTimeSeconds = 0; proto.MorphGameItemProtoId = null;
			proto.EditableItemId = 0; proto.EditableItem = Copy(itemSeed.EditableItem); proto.EditableItem.Id = 0; proto.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
			proto.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdable.Id, GameItemComponentRevision = holdable.RevisionNumber, GameItemProto = proto });
			db.GameItemProtos.Add(proto); db.SaveChanges();
		}
		var foreign = db.GameItemProtos.SingleOrDefault(x => x.UniqueName == "QA N18 Foreign Item");
		if (foreign is null)
		{
			foreign = Copy(proto); foreign.Id = db.GameItemProtos.Max(x => x.Id) + 1;
			foreign.Name = "QA N18 Foreign Item"; foreign.UniqueName = foreign.Name;
			foreign.ShortDescription = "a qan18foreign figurine"; foreign.Keywords = "qan18foreign figurine";
			foreign.EditableItemId = 0; foreign.EditableItem = Copy(db.EditableItems.Find(proto.EditableItemId)!); foreign.EditableItem.Id = 0;
			var component = db.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemProtoId == proto.Id && x.GameItemProtoRevision == proto.RevisionNumber);
			foreign.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.GameItemComponentProtoId,
				GameItemComponentRevision = component.GameItemComponentRevision, GameItemProto = foreign });
			db.GameItemProtos.Add(foreign); db.SaveChanges();
		}
		transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Rooms = rooms.Select(x => x.Id), ShadowPlane = plane.Id,
			MaterialPlane = db.Planes.Single(x => x.IsDefault).Id, EffigyPrototype = proto.Id, ForeignPrototype = foreign.Id }, new JsonSerializerOptions { WriteIndented = true }));
	}

	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var p in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
			p.SetValue(row, p.GetValue(source));
		return row;
	}
}
