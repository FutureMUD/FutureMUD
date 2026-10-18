#nullable enable
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using MudSharp.FutureProg;
using MudSharp.Framework;
using Db = MudSharp.Models;

namespace FutureMUD.GatheringNativePersistenceHarness;

internal static partial class GNHProgram
{
	private static void PrepareS6GatheringFixture(MudSharp.Database.FuturemudDatabaseContext db, JsonObject input,
		Db.Character actor, IReadOnlyDictionary<string, long> claims)
	{
		var stock = db.MagicCapabilities.Single(x => x.Id == claims["arm.capability.sorcerer"]);
		var definition = XElement.Parse(stock.Definition);
		Require((bool?)definition.Element("Casting")!.Attribute("enabled") == false && definition.Element("Gathering")!.Elements("Method").Count() == 3,
			"Gathering native fixture requires the persisted complete closed Sorcerer policy.");
		Db.FutureProg CopyProg(long id, string name)
		{
			var original = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == id);
			var copy = new Db.FutureProg(); db.Entry(copy).CurrentValues.SetValues(original);
			copy.Id = 0; copy.FunctionName = name;
			foreach (var parameter in original.FutureProgsParameters) copy.FutureProgsParameters.Add(new() {
				ParameterIndex = parameter.ParameterIndex, ParameterName = parameter.ParameterName, ParameterTypeDefinition = parameter.ParameterTypeDefinition });
			db.FutureProgs.Add(copy); db.SaveChanges(); return copy;
		}
		var stockGate = db.FutureProgs.Single(x => x.Id == claims[ArmageddonStockSupportInstaller.GatheringAvailabilityKey]);
		Require(stockGate.FunctionText == "return false", "Stock gathering availability must remain false.");
		var yes = CopyProg(stockGate.Id, "S6QualificationGatheringAvailable"); yes.FunctionText = "return true"; db.SaveChanges();
		foreach (var method in definition.Element("Gathering")!.Elements("Method"))
		{
			var kind = (string)method.Attribute("alias")!;
			var permission = CopyProg((long)method.Attribute("permission")!, "S6QualificationGather" + kind + "Permission");
			Require(permission.FunctionText.Contains("@" + stockGate.FunctionName + "("), "Qualification must preserve source permission checks.");
			permission.FunctionText = permission.FunctionText.Replace("@" + stockGate.FunctionName + "(", "@" + yes.FunctionName + "(", StringComparison.Ordinal);
			method.SetAttributeValue("permission", permission.Id);
		}
		definition.Element("Casting")!.SetAttributeValue("enabled", true);
		definition.Element("Casting")!.SetAttributeValue("identity", Guid.NewGuid());
		var capability = new Db.MagicCapability { Name = "S6 source gathering qualification", CapabilityModel = stock.CapabilityModel,
			MagicSchoolId = stock.MagicSchoolId, PowerLevel = stock.PowerLevel, Definition = definition.ToString() };
		db.MagicCapabilities.Add(capability); db.SaveChanges();
		var merit = new Db.Merit { Name = "S6 source gathering qualification caster", Type = "Magic Capability", MeritScope = 2, MeritType = 0,
			Definition = new XElement("Definition", new XElement("ChargenAvailableProg", ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysFalseProg]),
				new XElement("ApplicabilityProg", ArmageddonInstallation.Read(db)!.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg]),
				new XElement("ChargenBlurb", "Isolated qualification"), new XElement("DescriptionText", "@ have|has gathering magic."),
				new XElement("Capabilities", new XElement("Capability", capability.Id))).ToString() };
		db.Merits.Add(merit); db.SaveChanges();
		db.PerceiverMerits.Add(new() { CharacterId = actor.Id, MeritId = merit.Id });
		db.PerceiverMerits.Add(new() { CharacterId = actor.Id, MeritId = claims["arm.merit.sorcerer"] });
		var profile = new Db.AgricultureFieldProfile { Name = "S6 disposable pasture", Description = "Isolated gathering qualification.", Definition = "<Profile uses='Pasture' />" };
		db.AgricultureFieldProfiles.Add(profile); db.SaveChanges();
		Require(!db.AgricultureFields.Any(x => x.RoomId == actor.Location), "Fixture room already has an agriculture owner; preserve it.");
		var field = new Db.AgricultureField { RoomId = actor.Location, ProfileId = profile.Id, CurrentUse = 2, Moisture = 50, Drainage = 50,
			Nutrients = 50, Salinity = 0, Topsoil = 50, Tilth = 50, Rockiness = 0, Weeds = 0, Pests = 0, Fence = 50,
			Pasture = 10, Condition = 50, Definition = "<Field />" };
		db.AgricultureFields.Add(field);
		var room = db.Rooms.Single(x => x.Id == actor.Location);
		room.EnvironmentalMagicBindingMode = 1; room.EnvironmentalMagicProfileId = claims[ArmageddonProgressionInstaller.LandProfileKey];
		db.RoomsTags.Add(new() { RoomId = room.Id, TagId = claims[ArmageddonStockGatheringInstaller.TerrainTagKey("fertile")] });
		var expressions = new[] { "healthcapacity(@actor, \"health\")", "healthcapacity(@actor, \"stun\")", "healthcapacity(@actor, \"stamina\")",
			"healthremaining(@actor, \"health\")", "healthremaining(@actor, \"stun\")", "healthremaining(@actor, \"stamina\")",
			"gettrait(@actor, totrait(" + claims[ArmageddonProgressionInstaller.GatheringTraitKey] + "))" };
		var probe = new Db.FutureProg { FunctionName = "S6GatherBodyProbe", ReturnTypeDefinition = ProgVariableTypes.Text.ToStorageString(),
			FunctionText = "return \"GATHERVALUES \" + " + string.Join(" + \"|\" + ", expressions.Select(x => "totext(" + x + ")")) + " + \" ENDGATHER\"",
			Category = "Testing", Subcategory = "S6", FunctionComment = "Pure native current-body observation", AcceptsAnyParameters = false, StaticType = 0 };
		probe.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "actor", ParameterTypeDefinition = ProgVariableTypes.Character.ToStorageString() });
		db.FutureProgs.Add(probe); db.SaveChanges();
		using (var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray(), db.VariableDefinitions.ToArray()))
			foreach (var id in definition.Element("Gathering")!.Elements("Method").Select(x => (long)x.Attribute("permission")!).Append(probe.Id)) compiler.Compile(id);
		var schema = input["sql_schema"]!.AsArray();
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.VariableValue>(db, "VariableValues", ["ReferenceTypeDefinition", "ReferenceId", "ReferenceProperty", "ValueTypeDefinition", "ValueDefinition"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.Wound>(db, "Wounds", ["Id", "BodyId", "OriginalDamage", "CurrentDamage", "CurrentPain", "CurrentStun", "WoundType"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.AgricultureField>(db, "AgricultureFields", ["Id", "RoomId", "Pasture", "Definition"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.RoomEnvironmentalState>(db, "RoomEnvironmentalStates", ["RoomId", "Revision", "ScarDamage", "RecentPressure"])));
		schema.Add(JsonSerializer.SerializeToNode(PreparedStockSqlTable<Db.MagicGatheringOperation>(db, "MagicGatheringOperations", ["ActorId", "BodyId", "DamageCost", "PainCost", "StunCost", "SourceDebit", "SourceDebited", "NotificationCompleted", "LandDetailJson", "EcologicalChildId", "EcologicalApplied"])));
		input["s6_gathering"] = JsonSerializer.SerializeToNode(new { actor = actor.Id, body = actor.BodyId, room = room.Id, capability = capability.Id,
			stock_capability = stock.Id, resource = claims["arm.resource.sorcerer"], trait = claims[ArmageddonProgressionInstaller.GatheringTraitKey],
			field = field.Id, gate = stockGate.Id, probe = probe.Id, invoked = db.MagicSchools.Single(x => x.Id == stock.MagicSchoolId).SchoolVerb,
			character_type = ProgVariableTypes.Character.ToStorageString(), location_type = ProgVariableTypes.Location.ToStorageString(),
			limits = "Separate enabled copy, source permission checks retained, stock gate false. Disposable real pasture/fertile room, native standard human health model; no initial mana/Gather skill injection. Regional life is adapted to local yield units. No real-time ash-expiry wait." });
	}
}
