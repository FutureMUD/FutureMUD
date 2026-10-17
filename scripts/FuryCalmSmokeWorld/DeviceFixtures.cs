#nullable enable

using System.Reflection;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using ArmageddonReviewedUtilityContent = MudSharp.Magic.ArmageddonReviewedUtilityContent;

namespace FuryCalmSmokeWorld;

/// <summary>Stopped-world N22 definitions; accounts and bodies must already exist through native chargen.</summary>
internal static class DeviceFixtures
{
	internal static void Run(DbContextOptions<FuturemudDatabaseContext> options, string receipt)
	{
		using var db = new FuturemudDatabaseContext(options);
		using var transaction = db.Database.BeginTransaction();
		var token = Path.GetFileName(Path.GetDirectoryName(receipt))!.Split('-').Last();
		// Only documented manifest keys, component FKs and native character/body/instance ownership are followed.
		var mendId = db.SeederManagedRecords.Single(x => x.StableKey == ArmageddonReviewedUtilityContent.MendFleshKey &&
			x.EntityType == nameof(MagicSpell)).LogicalId!.Value;
		var mend = db.MagicSpells.Single(x => x.Id == mendId);
		var componentId = db.SeederManagedRecords.Single(x => x.StableKey == "arm.component.charged_wand").LogicalId!.Value;
		var component = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x => x.Id == componentId &&
			x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var stockItems = new Dictionary<string, long>();
		foreach (var kind in new[] { "wand", "staff" })
		{
			var id = db.SeederManagedRecords.Single(x => x.StableKey == "arm.item.charged_" + kind).LogicalId!.Value;
			stockItems.Add(kind, id);
		}
		var seed = db.GameItemProtos.Include(x => x.EditableItem).Include(x => x.GameItemProtosGameItemComponentProtos)
			.Single(x => x.Id == stockItems["wand"] && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var playerRows = new Dictionary<string, object>();
		foreach (var role in new[] { "caster", "reader" })
		{
			var account = db.Accounts.Include(x => x.AuthorityGroup).Single(x => x.Name == "n22" + role);
			var character = db.Characters.Single(x => x.AccountId == account.Id && !x.IsArchived);
			if (account.AuthorityGroup.AuthorityLevel != (int)PermissionLevel.Player || character.IsAdminAvatar ||
				db.Npcs.Any(x => x.CharacterId == character.Id) || character.BodyId is null)
				throw new InvalidOperationException("N22 requires genuine ordinary player accounts and native physical bodies.");
			var primary = db.CharacterInstances.Single(x => x.CharacterId == character.Id && x.IsPrimary);
			if (primary.BodyId != character.BodyId || !primary.IsEmbodied)
				throw new InvalidOperationException("N22 requires the player's actual primary physical instance.");
			var body = db.Bodies.Single(x => x.Id == character.BodyId);
			body.ShortDescription = "a qan22" + role + " man"; body.ShortDescriptionPatternId = null;
			playerRows.Add(role, new { Character = character.Id, Body = body.Id, Instance = primary.Id, Account = account.Id,
				AccountName = account.Name, Permission = account.AuthorityGroup.AuthorityLevel, Keyword = "qan22" + role });
		}
		var roomSeed = db.Rooms.Include(x => x.RoomOverlays).Single(x => x.Id == 1);
		var room = new MudSharp.Models.Room
		{
			ZoneId = roomSeed.ZoneId, UniqueName = "QA N22 Device Room " + token, EffectData = "<Effects />"
		};
		var overlay = Copy(roomSeed.RoomOverlays.Single(x => x.Id == roomSeed.CurrentOverlayId));
		overlay.Id = 0; overlay.Room = room; overlay.RoomId = 0;
		overlay.RoomName = "QA N22 Device Room"; overlay.RoomDescription = "An owned, lit charged-device qualification room.";
		overlay.AddedLight = 10000; overlay.OutdoorsType = (int)RoomOutdoorsType.Indoors; overlay.SafeQuit = true;
		db.Rooms.Add(room); db.RoomOverlays.Add(overlay); db.SaveChanges(); room.CurrentOverlayId = overlay.Id;
		var tag = new Tag { Name = "QA N22 " + token + " focus material" };
		db.Tags.Add(tag); db.SaveChanges();
		var materialWearable = db.GameItemComponentProtos.Include(x => x.EditableItem).Single(x =>
			x.Name == "Wear_Ring" && x.Type == "Wearable" && x.EditableItem.RevisionStatus == (int)RevisionStatus.Current);
		var focus = Copy(mend); focus.Id = 0; focus.Name = "QA_N22_Focus_" + token;
		focus.CastingDifficulty = (int)Difficulty.Automatic;
		var definition = XElement.Parse(mend.Definition);
		definition.Element("StockIdentity")?.Remove();
		definition.Element("Plan")!.ReplaceWith(new XElement("Plan", new XElement("Phase", new XElement("Action",
			new XAttribute("state", "consumed"), new XAttribute("tag", tag.Id), new XAttribute("secondtag", 0),
			new XAttribute("quantity", 1), new XAttribute("carriedonly", true)))));
		focus.Definition = definition.ToString(); db.MagicSpells.Add(focus); db.SaveChanges();
		var privateComponent = Copy(component); privateComponent.Id = db.GameItemComponentProtos.Max(x => x.Id) + 1;
		privateComponent.RevisionNumber = 0; privateComponent.Name = "QA N22 " + token + " device";
		privateComponent.EditableItemId = 0; privateComponent.EditableItem = Copy(component.EditableItem); privateComponent.EditableItem.Id = 0;
		var privateDefinition = XElement.Parse(component.Definition); privateDefinition.Element("Seconds")!.Value = "1";
		privateComponent.Definition = privateDefinition.ToString(); db.GameItemComponentProtos.Add(privateComponent); db.SaveChanges();
		GameItemProto Item(string role, bool device)
		{
			var item = Copy(seed); item.Id = db.GameItemProtos.Max(x => x.Id) + 1; item.RevisionNumber = 0;
			item.Name = "QA N22 " + token + " " + role; item.UniqueName = item.Name;
			item.Keywords = "qan22" + role; item.ShortDescription = "a qan22" + role + " fixture";
			item.FullDescription = "A disposable N22 definition, independent of installer ownership.";
			item.EditableItemId = 0; item.EditableItem = Copy(seed.EditableItem); item.EditableItem.Id = 0;
			foreach (var link in seed.GameItemProtosGameItemComponentProtos)
			{
				if (link.GameItemComponentProtoId == component.Id && !device) continue;
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto = item,
					GameItemComponentProtoId = link.GameItemComponentProtoId == component.Id ? privateComponent.Id : link.GameItemComponentProtoId,
					GameItemComponentRevision = link.GameItemComponentProtoId == component.Id ? 0 : link.GameItemComponentRevision });
			}
			if (!device)
			{
				item.GameItemProtosTags.Add(new() { TagId = tag.Id });
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemProto = item,
					GameItemComponentProtoId = materialWearable.Id, GameItemComponentRevision = materialWearable.RevisionNumber });
			}
			db.GameItemProtos.Add(item); db.SaveChanges(); return item;
		}
		var privateItem = Item("device", true); var material = Item("material", false);
		var merit = db.Merits.Single(x => x.Name == "Armageddon partial sorcerer caster" && x.Type == "Magic Capability");
		var capabilityId = (long)XElement.Parse(merit.Definition).Element("Capabilities")!.Elements("Capability").Single();
		var capability = db.MagicCapabilities.Single(x => x.Id == capabilityId);
		var casting = XElement.Parse(capability.Definition).Element("Casting")!;
		db.SaveChanges();
		transaction.Commit();
		File.WriteAllText(receipt, JsonSerializer.Serialize(new { Status = "PASS", Players = playerRows, Room = room.Id,
			Stock = stockItems, PrivateDevice = privateItem.Id, PrivateComponent = privateComponent.Id, Material = material.Id,
			MaterialWearable = materialWearable.Id,
			Mend = mend.Id, Trait = mend.CastingTraitDefinitionId, Focus = focus.Id, Merit = merit.Id, Capability = capabilityId,
			Reserve = (long)casting.Attribute("reserve")!, CapabilityDefinition = capability.Definition,
			MendAdmitted = casting.Elements("Admission").Any(x => (long)x.Attribute("spell")! == mend.Id),
			StockComponent = component.Id, StockDefinition = component.Definition, PrivateInitialDefinition = privateComponent.Definition },
			new JsonSerializerOptions { WriteIndented = true }));
	}

	private static T Copy<T>(T source) where T : class, new()
	{
		var row = new T();
		foreach (var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
			.Where(x => x.CanWrite && (x.PropertyType.IsValueType || x.PropertyType == typeof(string))))
			property.SetValue(row, property.GetValue(source));
		return row;
	}
}
