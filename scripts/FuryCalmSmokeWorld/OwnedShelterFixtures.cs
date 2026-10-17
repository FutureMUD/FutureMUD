#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using RoomOutdoorsType = MudSharp.Construction.RoomOutdoorsType;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Models;

internal static class OwnedShelterFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var seed = db.Rooms.Include(x => x.RoomOverlays).Single(x => x.Id == 1);
		var overlaySeed = seed.RoomOverlays.Single(x => x.Id == seed.CurrentOverlayId);
		var terrain = overlaySeed.TerrainId;
		var permit = db.FutureProgs.SingleOrDefault(x => x.FunctionName == "QA_N17_MountPermit");
		if (permit is null)
		{
			permit = Copy(db.FutureProgs.Find(1L)!); permit.Id = 0; permit.FunctionName = "QA_N17_MountPermit";
			permit.FunctionText = "return true"; permit.StaticType = 0;
			permit.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "rider", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			permit.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "mount", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
			db.FutureProgs.Add(permit); db.SaveChanges();
		}
		var mountAi = db.ArtificialIntelligences.SingleOrDefault(x => x.Name == "QA N17 Mount");
		if (mountAi is null)
		{
			mountAi = new() { Name = "QA N17 Mount", Type = "Mount", Definition = new XElement("Definition",
				new XElement("PermitRiderProg", permit.Id), new XElement("PermitControlProg", permit.Id), new XElement("WhyCannotPermitRiderProg", 0),
				new XElement("MountNonConsensualMountDifficulty", 11), new XElement("MountControlDifficulty", 0), new XElement("MountResistBuckDifficulty", 0),
				new XElement("MaximumNumberOfRiders", 1), new XElement("RawMountEmote", "$1 mount|mounts $0."),
				new XElement("RawDismountEmote", "$1 dismount|dismounts $0."), new XElement("RawControlDeniedEmote", "$0 refuse|refuses $1."),
				new XElement("RawBuckEmote", "$0 try|tries to buck $1!")).ToString() };
			db.ArtificialIntelligences.Add(mountAi); db.SaveChanges();
		}
		Room Room(string key, string title, string description, bool indoors)
		{
			var existing = db.Rooms.SingleOrDefault(x => x.UniqueName == key);
			if (existing is not null) return existing;
			var row = new Room { ZoneId = seed.ZoneId, EffectData = "<Effects />", UniqueName = key };
			var overlay = Copy(overlaySeed); overlay.Id = 0; overlay.RoomId = 0; overlay.Room = row;
			overlay.RoomName = title; overlay.RoomDescription = description;
			overlay.OutdoorsType = (int)(indoors ? RoomOutdoorsType.Indoors : RoomOutdoorsType.Outdoors); overlay.SafeQuit = true;
			db.Rooms.Add(row); db.RoomOverlays.Add(overlay); db.SaveChanges();
			row.CurrentOverlayId = overlay.Id; db.SaveChanges(); return row;
		}
		var fallback = Room("QA N17 Recovery", "QA Shelter Recovery", "A stable dry place used only by the owned shelter qualification.", true);
		var anchor = Room("QA N17 Anchor", "QA Shelter Clearing", "An authored casting site in the owned qualification world.", false);
		var spring = Room("QA N17 Spring Template", "QA Spring Haven", "A sheltered hollow with a finite watering basin.", true);
		var burrow = Room("QA N17 Burrow Template", "QA Burrow Refuge", "An underground refuge reached through a burrow opening.", true);
		var sand = Room("QA N17 Sand Template", "QA Sand Shelter", "A protected sand enclosure, bounded by a solid roof and walls.", true);
		var invalid = Room("QA N17 Incompatible", "QA Incompatible Ground", "A separate source terrain which the three qualification spells do not admit.", false);
		var invalidTerrain = db.Terrains.SingleOrDefault(x => x.Name == "QA N17 inadmissible terrain");
		if (invalidTerrain is null)
		{
			invalidTerrain = Copy(db.Terrains.Find(terrain)!); invalidTerrain.Id = 0; invalidTerrain.Name = "QA N17 inadmissible terrain";
			db.Terrains.Add(invalidTerrain); db.SaveChanges();
		}
		db.RoomOverlays.Find(invalid.CurrentOverlayId)!.TerrainId = invalidTerrain.Id;
		var pool = db.GameItemProtos.SingleOrDefault(x => x.UniqueName == "QA N17 finite basin");
		var cup = db.GameItemProtos.SingleOrDefault(x => x.UniqueName == "QA N17 draw cup");
		if (pool is null || cup is null)
		{
			var itemSeed = db.GameItemProtos.Include(x => x.EditableItem).OrderBy(x => x.Id).First();
			var componentSeed = db.GameItemComponentProtos.Include(x => x.EditableItem).First(x => x.Type == "Holdable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
			var component = Copy(componentSeed); component.Id = db.GameItemComponentProtos.Max(x => x.Id) + 1; component.RevisionNumber = 0;
			component.Type = "Liquid Container"; component.Name = "QA N17 finite liquid container";
			component.Definition = "<Definition LiquidCapacity='10000000' Closable='false' Transparent='true' WeightLimit='10000000' OnceOnly='false' AdjustQuantityProg='0' DefaultLiquid='0' CanBeEmptiedWhenInRoom='true' />";
			component.EditableItemId = 0; component.EditableItem = Copy(itemSeed.EditableItem); component.EditableItem.Id = 0;
			component.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
			db.GameItemComponentProtos.Add(component); db.SaveChanges();
			GameItemProto Item(string key, string shortDescription, bool holdable)
			{
				var row = Copy(itemSeed); row.Id = db.GameItemProtos.Max(x => x.Id) + 1; row.RevisionNumber = 0;
				row.Name = key; row.UniqueName = key; row.ShortDescription = shortDescription; row.Keywords = holdable ? "qadrawcup cup" : "qawaterbasin basin pool water";
				row.FullDescription = "An explicitly authored native N17 qualification fixture.";
				row.ReadOnly = false; row.MorphTimeSeconds = 0; row.MorphGameItemProtoId = null;
				row.EditableItemId = 0; row.EditableItem = Copy(itemSeed.EditableItem); row.EditableItem.Id = 0;
				row.EditableItem.RevisionStatus = (int)RevisionStatus.Current;
				row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber, GameItemProto = row });
				if (holdable) row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = componentSeed.Id, GameItemComponentRevision = componentSeed.RevisionNumber, GameItemProto = row });
				db.GameItemProtos.Add(row); db.SaveChanges(); return row;
			}
			pool ??= Item("QA N17 finite basin", "a qawaterbasin", false);
			cup ??= Item("QA N17 draw cup", "a qadrawcup", true);
		}
		db.SaveChanges(); transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Anchor = anchor.Id, Fallback = fallback.Id,
			Spring = spring.Id, Burrow = burrow.Id, Sand = sand.Id, Terrain = terrain, Incompatible = invalid.Id,
			PoolPrototype = pool.Id, CupPrototype = cup.Id, MountAi = mountAi.Id, Water = db.Liquids.Single(x => x.Name == "Water").Id },
			new JsonSerializerOptions { WriteIndented = true }));
	}

	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var property in typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public)
			.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
			property.SetValue(row, property.GetValue(source));
		return row;
	}
}
