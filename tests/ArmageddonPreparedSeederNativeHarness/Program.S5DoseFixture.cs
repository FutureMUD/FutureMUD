#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.Magic;
using MudSharp.Health;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareS5DoseFixture(TestDatabase database, string path)
	{
		using var db = NewIndependentContext(database.ConnectionString);
		var settings = ArmageddonInstallation.Read(db)!;
		var ids = db.SeederManagedRecords.AsNoTracking()
			.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && !x.Retired && x.LogicalId > 0)
			.ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var spell = db.MagicSpells.Single(x => x.Id == ids[ArmageddonBoundConsumableSpellContent.RestorativeMudKey]);
		var prototype = ids["arm.item.restorative_mud"];
		var component = db.GameItemComponentProtos.Single(x => x.Id == ids["arm.item.restorative_mud.component"]);
		var definition = XElement.Parse(spell.Definition);
		var approved = ArmageddonBoundConsumableSpellContent.RestorativeMud(prototype).BuildDefinition(1, 1, 0);
		Require(XNode.DeepEquals(definition.Element("Effects"), approved.Element("Effects")), "Restored topical production effects differ from current approved content.");
		var dose = XElement.Parse(component.Definition);
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		Require(component.Type == "TopicalCream" && dose.Element("NativeHealingDose") is { } profile &&
			(string?)profile.Attribute("key") == "arm.item.restorative_mud" && (double?)profile.Attribute("grams") == 1 &&
			(double?)profile.Attribute("budgetPerGrade") == 10 && (double)dose.Element("TotalGrams")! == kilograms / 1000.0 &&
			!dose.Element("Drugs")!.Elements().Any() && (long?)dose.Element("OnApplyProg") == 0,
			"Restored native topical profile differs from the locked one-gram, ten-per-grade dose.");
		var reserve = ids["arm.resource.water"];
		var route = PreparedStockCampaignRoute(db, settings, ArmageddonBoundConsumableSpellContent.RestorativeMudKey,
			spell, reserve, ids[ArmageddonProgressionInstaller.SourceResourceKey], 1);
		var input = JsonNode.Parse(File.ReadAllText(path))!.AsObject();
		var actor = db.Characters.Single(x => x.Id == input["character"]!.GetValue<long>());
		var bodyId = input["body"]!.GetValue<long>();
		Require(!db.Wounds.Any(x => x.BodyId == bodyId), "The sealed calibration actor must start without wounds.");
		var ancestry = new List<long>();
		long? current = db.Bodies.Single(x => x.Id == bodyId).BodyPrototypeId;
		while (current is { } bodyPrototype)
		{
			Require(!ancestry.Contains(bodyPrototype), "Cyclic fixture body ancestry.");
			ancestry.Add(bodyPrototype);
			current = db.BodyProtos.Single(x => x.Id == bodyPrototype).CountsAsId;
		}
		Db.Wound CalibrationWound(string part, double damage)
		{
			var bodypart = db.BodypartProtos.Single(x => ancestry.Contains(x.BodyId) && x.Name == part && x.IsOrgan == 0);
			var wound = new Db.Wound { BodyId = bodyId, BodypartProtoId = bodypart.Id, WoundType = "SimpleOrganic",
				DamageType = (int)DamageType.Necrotic, OriginalDamage = damage, CurrentDamage = damage, RealTimeOfWound = DateTime.UtcNow,
				ExtraInformation = new XElement("Definition", new XElement("DamageDescription", "necrosis"),
					new XElement("BleedStatus", (int)BleedStatus.NeverBled)).ToString() };
			db.Wounds.Add(wound);
			return wound;
		}
		// Preparation is deliberately independent of native armour/organ attack RNG.
		// The production MUD loader materialises these ordinary native wound rows.
		var calibrationWound = CalibrationWound("abdomen", 30);
		var foreignWound = CalibrationWound("lhand", 1);
		var privateCapability = db.MagicCapabilities.Single(x => x.Id == route.Capability);
		var privatePolicy = XElement.Parse(privateCapability.Definition);
		privatePolicy.Element("Gathering")!.Element("Method")!.SetAttributeValue("maximumHealthSeverity", WoundSeverity.Horrifying);
		privateCapability.Definition = privatePolicy.ToString();
		var room = db.Rooms.Include(x => x.CurrentOverlay).Single(x => x.Id == actor.Location);
		room.CurrentOverlay.AddedLight = 1000;
		db.SaveChanges();
		var school = db.MagicSchools.Single(x => x.Id == spell.MagicSchoolId);
		var anchor = db.GameItemProtos.Single(x => x.Id == ids["arm.item.component_blank"]);
		// Ordinary selectors use visible descriptions. This exact owned blank is an inert item anchor.
		Require(anchor.ShortDescription.Contains("blank", StringComparison.OrdinalIgnoreCase), "The known item anchor description changed.");
		var source = Path.Combine(CatalogueRepository, "tests", "ArmageddonPreparedSeederNativeHarness", "Program.S5DoseFixture.cs");
		input["s5_dose"] = JsonSerializer.SerializeToNode(new
		{
			spell = spell.Id, prototype, component = component.Id, capability = route.Capability, merit = route.Merit,
			resource = reserve, invoked = school.SchoolVerb.ToLowerInvariant(), anchor = anchor.Id,
			kilograms, calibration_wound = calibrationWound.Id, foreign_wound = foreignWound.Id,
			wound_setup = "Preboot native SimpleOrganic Necrotic rows: abdomen30, foreign left hand1; native armour/attack and natural healing are outside calibration.",
			mode = settings.Mode.ToString(), source = new { path = source, sha256 = BaseHash(source) },
			limits = "Real stock grade-one paid production and one-gram topical calibration only. Separate builder route opens100 and uses finite paid Self gathering with a private Horrifying health ceiling for the prepared calibration injury. Natural stock acquisition, full progression and affordability are not qualified."
		});
		File.WriteAllText(path, input.ToJsonString());
		Console.WriteLine("ARMS5-dose-input=prepared exact canonical production and untouched owned one-gram topical profile; only separate qualification route enabled");
	}
}
