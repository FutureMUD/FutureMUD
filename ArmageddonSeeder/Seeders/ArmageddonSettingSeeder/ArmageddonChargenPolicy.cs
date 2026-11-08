#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Player-facing setting policy, independent of local account/approval policy.</summary>
internal static class ArmageddonChargenPolicy
{
	internal static bool ConfigureWorldfileStart(FuturemudDatabaseContext db)
	{
		var room = db.Rooms.SingleOrDefault(x => x.UniqueName == "Vnum50615");
		if (room is null) return false;
		var role = db.ChargenRoles.SingleOrDefault(x => x.Type == (int)ChargenRoleType.StartingLocation && x.Name == "Default Starting Location");
		if (role is null) return false;
		var changed = false;
		foreach (var storyboard in db.ChargenScreenStoryboards.Where(x => x.ChargenStage == (int)ChargenStage.SelectStartingLocation && x.ChargenType == "StartingLocationPicker").ToArray())
		{
			var xml = XElement.Parse(storyboard.StageDefinition);
			foreach (var location in xml.Element("Locations")?.Elements("Location") ?? Enumerable.Empty<XElement>())
			{
				// Replace only the untouched installer placeholder, retaining other starts and commencement progs.
				if (location.Element("Name")?.Value != "Guest Lounge" || location.Element("Location")?.Value != "1" ||
				    location.Element("Blurb")?.Value.Trim() != "As you have not yet set up any other starting areas, the default starting area will be the guest lounge." ||
				    location.Element("Role")?.Value != role.Id.ToString(System.Globalization.CultureInfo.InvariantCulture) ||
				    location.Element("OnCommenceProg")?.Value != "0") continue;
				location.SetElementValue("Name", "Allanak - the Gaj");
				location.SetElementValue("Blurb", "The Gladiator and the Gaj tavern, in the commoners quarter of Allanak.");
				location.SetElementValue("Location", room.Id);
				changed = true;
			}
			storyboard.StageDefinition = xml.ToString();
		}
		db.SaveChanges();
		return changed;
	}

	internal static void Install(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var unavailable = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var raceIds = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.EntityType == nameof(Race) && x.StableKey.StartsWith("arm.race.")).Select(x => x.LogicalId).ToArray();
		var cultureIds = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.EntityType == nameof(Culture) && x.StableKey.StartsWith("arm.culture.")).Select(x => x.LogicalId).ToArray();
		foreach (var race in db.Races.Where(x => !raceIds.Contains(x.Id)).ToArray())
			race.AvailabilityProgId = Field(db, nameof(Race), race.Id, "AvailabilityProgId", race.AvailabilityProgId, (long?)unavailable, messages);
		foreach (var culture in db.Cultures.Where(x => !cultureIds.Contains(x.Id)).ToArray())
			culture.AvailabilityProgId = Field(db, nameof(Culture), culture.Id, "AvailabilityProgId", culture.AvailabilityProgId, (long?)unavailable, messages);
		foreach (var knowledge in db.Knowledges.Where(x => x.Name == "Robot Diagnostics" || x.Name == "Robot Maintenance" || x.Name == "Robot Surgery").ToArray())
			knowledge.CanAcquireProgId = Field(db, nameof(Knowledge), knowledge.Id, "CanAcquireProgId", knowledge.CanAcquireProgId, (long?)unavailable, messages);
		foreach (var merit in db.Merits.Where(x => x.Type == "Planar State").ToArray())
		{
			var xml = XElement.Parse(merit.Definition);
			var value = Field(db, nameof(Merit), merit.Id, "ChargenAvailableProg", xml.Element("ChargenAvailableProg")?.Value,
				unavailable.ToString(System.Globalization.CultureInfo.InvariantCulture), messages);
			xml.SetElementValue("ChargenAvailableProg", value);
			merit.Definition = xml.ToString();
		}

		if (!db.ChargenRoles.Any(x => x.Type == (int)ChargenRoleType.Profession))
		foreach (var storyboard in db.ChargenScreenStoryboards.Where(x => x.ChargenStage == (int)ChargenStage.SelectRole && x.ChargenType == "RolePicker").ToArray())
		{
			var xml = XElement.Parse(storyboard.StageDefinition);
			var profession = xml.Element("RoleTypes")?.Elements("RoleType").SingleOrDefault(x => (string?)x.Attribute("Type") == "Profession");
			if (profession is null) continue;
			profession.SetAttributeValue("CanSelectNone", Field(db, nameof(ChargenScreenStoryboard), storyboard.Id,
				"ProfessionOptional", (string?)profession.Attribute("CanSelectNone"), "true", messages));
			storyboard.StageDefinition = xml.ToString();
		}

		db.SaveChanges();
	}

	private static T Field<T>(FuturemudDatabaseContext db, string type, long id, string field, T current, T desired,
		ICollection<string> messages)
	{
		var merged = ArmageddonSettingLinks.Reconcile(db, $"arm.chargen.policy.{type}.{id}", type + ".ChargenPolicy", id,
			new Dictionary<string, string> { [field] = JsonSerializer.Serialize(current) },
			new Dictionary<string, string> { [field] = JsonSerializer.Serialize(desired) }, messages);
		return JsonSerializer.Deserialize<T>(merged[field])!;
	}
}
