#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

internal static class ArmageddonRepairBinding
{
	internal static void InstallFresh(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills)
	{
		const string name = "Armageddon Armour Repair Kit";
		if (db.GameItemComponentProtos.Any(x => x.Name == name)) throw new InvalidOperationException("Unowned armour repair component collision.");
		var armour = db.Tags.Single(x => x.Name == "Military Armour");
		var account = db.Accounts.First();
		var now = DateTime.UtcNow;
		// Authored native starter tuning; the source proficiency remains the only check trait.
		var component = new GameItemComponentProto
		{
			Id = db.GameItemComponentProtos.Select(x => (long?)x.Id).Max().GetValueOrDefault() + 1, RevisionNumber = 0,
			Name = name, Type = "RepairKit", Description = "Repairs tagged armour using canonical Armor Repair proficiency",
			Definition = new XElement("Definition", new XElement("MaximumSeverity", (int)WoundSeverity.Grievous),
				new XElement("RepairPoints", 500), new XElement("CheckTrait", skills["arm.skill.armor_repair"].Id),
				new XElement("CheckBonus", 0), new XElement("Echoes", new XElement("Echo", new XCData("@ work|works on repairing $1 with $2."))),
				new XElement("Materials"), new XElement("DamageTypes"), new XElement("Tags", new XElement("Tag", armour.Id))).ToString(),
			EditableItem = new EditableItem { RevisionNumber = 0, RevisionStatus = 4, BuilderAccountId = account.Id, BuilderDate = now,
				ReviewerAccountId = account.Id, ReviewerDate = now, BuilderComment = "Armageddon native repair adaptation", ReviewerComment = "Armageddon native repair adaptation" }
		};
		db.GameItemComponentProtos.Add(component); db.SaveChanges();
		var record = ArmageddonSettingOwnership.RecordNew(db, ArmageddonMundaneBindings.Module, "arm.repair.armour.component", component,
			nameof(GameItemComponentProto.Type), nameof(GameItemComponentProto.Name), nameof(GameItemComponentProto.Definition));
		record.RevisionNumber = 0; db.SaveChanges();
	}
}
