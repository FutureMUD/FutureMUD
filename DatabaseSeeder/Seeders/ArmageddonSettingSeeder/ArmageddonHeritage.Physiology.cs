#nullable enable

extern alias EngineCompiler;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using NativePositionState = EngineCompiler::MudSharp.Body.Position.PositionState;
using NativeStanding = EngineCompiler::MudSharp.Body.Position.PositionStates.PositionStanding;
using PositionHeightComparison = MudSharp.Body.Position.PositionHeightComparison;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonHeritage
{
	internal const string NormalEnvironmentKey = "arm.merit.environment.normal";
	internal const string DesertEnvironmentKey = "arm.merit.environment.desert_elf";
	internal const string GiantMetabolismKey = "arm.merit.metabolism.half_giant";
	internal const string DesertStaminaKey = "arm.merit.stamina.desert_elf";
	internal const string ReserveStaminaKey = "arm.merit.stamina.reserve";
	private sealed record NeedsTerrainBindings(long[] Severe, long[] Moderate, long[] Shelter);
	private static readonly (string Key, string Name)[] PhysiologyIdentities =
	[
		(NormalEnvironmentKey, "Armageddon Environmental Thirst"),
		(DesertEnvironmentKey, "Armageddon Desert Elf Hydration"),
		(GiantMetabolismKey, "Armageddon Giant Metabolism"),
		(DesertStaminaKey, "Armageddon Desert Elf Stamina"),
		(ReserveStaminaKey, "Armageddon Stamina Reserve")
	];

	private static void PreflightPhysiology(FuturemudDatabaseContext db)
	{
		Preflight<Merit>(db, PhysiologyIdentities);
		Preflight<FutureProg>(db,
			[("arm.needs.environment.normal", "ArmageddonEnvironmentalThirst"),
			 ("arm.needs.environment.desert_elf", "ArmageddonDesertElfEnvironmentalThirst")]);
	}

	private static NeedsTerrainBindings NeedsTerrains(FuturemudDatabaseContext db)
	{
		var prior = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == NormalEnvironmentKey)?.SeedBaseline;
		if (prior is not null)
		{
			var fields = JsonSerializer.Deserialize<Dictionary<string, string>>(prior)!;
			var definition = XElement.Parse(JsonSerializer.Deserialize<string>(fields["Definition"])!);
			if (definition.Element("SourceTerrainBindings") is { } saved)
			{
				var bindings = JsonSerializer.Deserialize<NeedsTerrainBindings>(saved.Value)!;
				var available = db.Terrains.Select(x => x.Id).ToHashSet();
				if (bindings.Severe.Concat(bindings.Moderate).Concat(bindings.Shelter).Any(x => !available.Contains(x)))
					throw new InvalidOperationException("An originally bound environmental-needs terrain is missing; preserve and resolve the terrain foundation before rerunning.");
				return bindings;
			}
		}
		long[] Resolve(params string[] names) => db.Terrains.AsEnumerable()
			.Where(x => names.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).Select(x => x.Id).Order().ToArray();
		return new(Resolve("Sandy Desert", "Rocky Desert", "Coastal Desert", "Salt Flat", "Thorn Scrub"),
			Resolve("Field", "Grasslands", "Steppe", "Shortgrass Prairie", "Tallgrass Prairie", "Pasture", "Meadow", "Montane Grassland", "Alpine Meadow", "Terraced Field",
				"Hills", "Foothills", "Mountainside", "Mountain Pass", "Mountain Ridge", "Boreal Forest", "Broadleaf Forest", "Temperate Coniferous Forest", "Temperate Rainforest",
				"Tropical Rainforest", "Tropical Dry Forest", "Monsoon Forest", "Cloud Forest", "Bamboo Forest", "Plantation Forest", "Woodland", "Swamp Forest", "Ruins", "Ocean Shallows", "Shallow River", "Shallow Lake"),
			Resolve("Sewer", "Residence", "Bedroom", "Bathroom", "Living Room", "Hallway", "Hall", "Shopfront", "Workshop", "Office"));
	}

	private static string EnvironmentalThirstBody(NeedsTerrainBindings terrains, bool desert)
	{
		NativePositionState.SetupPositions();
		var lower = NativePositionState.States.Where(x => x.CompareTo(NativeStanding.Instance) == PositionHeightComparison.Lower).Select(x => x.Id).ToArray();
		string IsTerrain(IEnumerable<long> ids) => string.Join(" or ", ids.Select(x => $"@ch.Location.Terrain == ToTerrain({x})"));
		var body = new StringBuilder("if (isnull(@ch.Location))\n  return 0\nend if\nvar drain as number\ndrain = 1\nvar daylight as text\ndaylight = TimeOfDay(@ch.Location)\nif (@daylight == \"Morning\" or @daylight == \"Afternoon\")\n  drain = @drain + 1\nend if\n");
		foreach (var (ids, amount) in new[] { (terrains.Severe, desert ? 2 : 4), (terrains.Moderate, desert ? 1 : 2), (terrains.Shelter, desert ? -2 : -1) })
			if (ids.Length > 0) body.AppendLine($"if ({IsTerrain(ids)})\n  drain = @drain + ({amount})\nend if");
		body.AppendLine($"if (@ch.Location.Outdoors == 0 or @ch.Location.Outdoors == 1 or @ch.Location.Outdoors == 3)\n  drain = @drain - {(desert ? 2 : 1)}\nend if");
		if (lower.Length > 0)
			body.AppendLine($"if ({string.Join(" or ", lower.Select(x => $"PositionId(@ch) == {x}"))})\n  drain = @drain - 1\nend if");
		body.AppendLine($"if (@drain < 0)\n  drain = 0\nend if\nif (@drain > {(desert ? 5 : 10)})\n  drain = {(desert ? 5 : 10)}\nend if\nreturn @drain - 1");
		return body.ToString();
	}

	private static IReadOnlyDictionary<string, long> InstallPhysiologyMerits(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var terrains = NeedsTerrains(db);
		XElement Root(string description) => new("Definition", new XElement("ChargenAvailableProg", no), new XElement("ApplicabilityProg", yes),
			new XElement("DescriptionText", new XCData(description)), new XElement("ChargenBlurb", "Intrinsic racial physiology; not a selectable quirk."));
		var result = new Dictionary<string, long>();
		foreach (var desert in new[] { false, true })
		{
			var key = desert ? DesertEnvironmentKey : NormalEnvironmentKey;
			var prog = Prog(db, desert ? "arm.needs.environment.desert_elf" : "arm.needs.environment.normal",
				desert ? "ArmageddonDesertElfEnvironmentalThirst" : "ArmageddonEnvironmentalThirst",
				EnvironmentalThirstBody(terrains, desert), ProgVariableTypes.Number, messages, (ProgVariableTypes.Character, "ch"));
			var definition = Root(desert ? "@ have|has a desert-adapted response to environmental thirst" : "@ experience|experiences environmental thirst");
			definition.Add(new XElement("HungerProg", 0), new XElement("ThirstProg", prog.Id), new XElement("WaterProg", 0), new XElement("MaximumThirstRate", desert ? 5 : 10),
				new XElement("SourceTerrainBindings", Json(terrains)),
				new XElement("AdaptationNote", "Source condition rates translated to native passive thirst counters. Native Morning/Afternoon receive +1; there is no native Highsun band. Litres are not inferred from condition points."));
			result[key] = Put(key, definition, "Environmental Needs");
		}
		var metabolism = Root("@ require|requires three times the ordinary food and drink");
		metabolism.Add(new XAttribute("hunger", 3), new XAttribute("thirst", 3), new XAttribute("alcohol", 1));
		result[GiantMetabolismKey] = Put(GiantMetabolismKey, metabolism, "Needs Rate Change");
		foreach (var (key, bonus) in new[] { (DesertStaminaKey, 60), (ReserveStaminaKey, 20) })
		{
			var definition = Root("@ have|has additional stamina reserves");
			definition.Add(new XElement("Bonus", bonus), new XElement("Multiplier", 1));
			result[key] = Put(key, definition, "Stamina Capacity");
		}
		return result;

		long Put(string key, XElement definition, string type)
		{
			var row = Apply(db, key, new Merit { Name = PhysiologyIdentities.Single(x => x.Key == key).Name, Type = type, MeritScope = 1, MeritType = 0, Definition = definition.ToString() }, messages);
			if (row.MeritScope != 1) throw new InvalidOperationException($"{row.Name}: intrinsic racial merits require Body scope. The builder's custom scope is preserved; resolve it before installation.");
			return row.Id;
		}
	}

	private static IEnumerable<long> IntrinsicMeritsFor(string profile, IReadOnlyDictionary<string, long> merits)
	{
		yield return merits[profile == "desert_elf" ? DesertEnvironmentKey : NormalEnvironmentKey];
		if (profile == "desert_elf") yield return merits[DesertStaminaKey];
		if (profile is "half_giant" or "mul") yield return merits[ReserveStaminaKey];
		if (profile == "half_giant") yield return merits[GiantMetabolismKey];
	}
}
