#nullable enable

using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

public partial class UsefulSeeder
{
	private static readonly string[] StockAutobuilderRoomTemplateNames =
	[
		"Seeded Terrain Baseline",
		"Seeded Terrain Random Description"
	];

	internal static IReadOnlyCollection<string> StockAutobuilderRoomTemplateNamesForTesting =>
		StockAutobuilderRoomTemplateNames;

	internal static ShouldSeedResult ClassifyAutobuilderPackagePresence(FuturemudDatabaseContext context)
	{
		return SeederRepeatabilityHelper.ClassifyByPresence(
			StockAutobuilderRoomTemplateNames.Select(name => context.AutobuilderRoomTemplates.Any(x => x.Name == name)));
	}

	internal void SeedTerrainAutobuilderForTesting(FuturemudDatabaseContext context)
	{
		SeedTerrainAutobuilderCore(context, new List<string>());
	}

	private void SeedTerrainAutobuilderCore(FuturemudDatabaseContext context, ICollection<string> errors)
	{
		List<Terrain> terrains = context.Terrains
			.OrderBy(x => x.Id)
			.AsEnumerable()
			.Where(x => !string.Equals(x.Name, "Void", StringComparison.OrdinalIgnoreCase))
			.ToList();
		if (!terrains.Any())
		{
			errors.Add(
				"Could not seed the terrain autobuilder room templates because no terrains are installed yet. Run the terrain foundations seeding first.");
			return;
		}

		Dictionary<long, HashSet<string>> terrainTags = BuildTerrainTagLookup(context, terrains);
		foreach (AutobuilderRoomTemplateSeedDefinition roomTemplate in BuildStockAutobuilderRoomTemplates(terrains, terrainTags))
		{
			EnsureAutobuilderRoomTemplate(context, roomTemplate);
		}

		context.SaveChanges();
	}

	private static IReadOnlyList<AutobuilderRoomTemplateSeedDefinition> BuildStockAutobuilderRoomTemplates(
		IReadOnlyCollection<Terrain> terrains,
		IReadOnlyDictionary<long, HashSet<string>> terrainTags)
	{
		Terrain defaultTerrain = terrains.FirstOrDefault(x => x.DefaultTerrain) ?? terrains.OrderBy(x => x.Id).First();
		List<XElement> roomInfos = terrains
			.Select(x => CreateRoomInfoElement(x, x.Name, BuildSeededTerrainBaseDescription(x, terrainTags[x.Id])))
			.ToList();
		XElement defaultInfo = roomInfos.First(x => long.Parse(x.Element("DefaultTerrain")!.Value) == defaultTerrain.Id);
		List<XElement> overrides = roomInfos
			.Where(x => long.Parse(x.Element("DefaultTerrain")!.Value) != defaultTerrain.Id)
			.ToList();

		return
		[
			new AutobuilderRoomTemplateSeedDefinition(
				"Seeded Terrain Baseline",
				"room by terrain",
				CreateRoomByTerrainTemplateDefinition(
					"Terrain-aware baseline descriptions for the stock seeded terrain catalogue.",
					defaultInfo,
					overrides,
					applyTagsAsFrameworkTags: true
				)
			),
			new AutobuilderRoomTemplateSeedDefinition(
				"Seeded Terrain Random Description",
				"room random description",
				CreateRandomDescriptionRoomTemplateDefinition(
					"Terrain-aware random descriptions layered on top of the stock seeded terrain catalogue.",
					defaultInfo,
					overrides,
					BuildStockRandomDescriptionElements(terrains, terrainTags),
					defaultRandomElementExpression: "2+1d2",
					applyTagsAsFrameworkTags: true
				)
			)
		];
	}

	private static IEnumerable<XElement> BuildStockRandomDescriptionElements(
		IReadOnlyCollection<Terrain> terrains,
		IReadOnlyDictionary<long, HashSet<string>> terrainTags)
	{
		List<XElement> elements = new();
		List<Terrain> outdoorsTerrains = terrains.Where(IsOutdoorsTerrain).ToList();
		List<Terrain> indoorsTerrains = terrains.Except(outdoorsTerrains).ToList();
		List<Terrain> urbanTerrains = GetTerrainsByTag(terrains, terrainTags, "Urban");
		List<Terrain> ruralTerrains = GetTerrainsByTag(terrains, terrainTags, "Rural");
		List<Terrain> publicTerrains = GetTerrainsByTag(terrains, terrainTags, "Public");
		List<Terrain> privateTerrains = GetTerrainsByTag(terrains, terrainTags, "Private");
		List<Terrain> commercialTerrains = GetTerrainsByTag(terrains, terrainTags, "Commercial");
		List<Terrain> residentialTerrains = GetTerrainsByTag(terrains, terrainTags, "Residential");
		List<Terrain> administrativeTerrains = GetTerrainsByTag(terrains, terrainTags, "Administrative");
		List<Terrain> industrialTerrains = GetTerrainsByTag(terrains, terrainTags, "Industrial");
		List<Terrain> terrestrialTerrains = GetTerrainsByTag(terrains, terrainTags, "Terrestrial");
		List<Terrain> aquaticTerrains = GetTerrainsByTag(terrains, terrainTags, "Aquatic");
		List<Terrain> littoralTerrains = GetTerrainsByTag(terrains, terrainTags, "Littoral");
		List<Terrain> riparianTerrains = GetTerrainsByTag(terrains, terrainTags, "Riparian");
		List<Terrain> wetlandTerrains = GetTerrainsByTag(terrains, terrainTags, "Wetland");
		List<Terrain> aridTerrains = GetTerrainsByTag(terrains, terrainTags, "Arid");
		List<Terrain> glacialTerrains = GetTerrainsByTag(terrains, terrainTags, "Glacial");
		List<Terrain> volcanicTerrains = GetTerrainsByTag(terrains, terrainTags, "Volcanic");
		List<Terrain> lunarTerrains = GetTerrainsByTag(terrains, terrainTags, "Lunar");
		List<Terrain> spaceTerrains = GetTerrainsByTag(terrains, terrainTags, "Space");
		List<Terrain> vacuumTerrains = GetTerrainsByTag(terrains, terrainTags, "Vacuum");
		List<Terrain> diggableTerrains = GetTerrainsByTag(terrains, terrainTags, "Diggable Soil");
		List<Terrain> sandTerrains = GetTerrainsByTag(terrains, terrainTags, "Foragable Sand");
		List<Terrain> clayTerrains = GetTerrainsByTag(terrains, terrainTags, "Foragable Clay");
		List<Terrain> woodedTerrains = terrains.Where(x =>
			x.TerrainBehaviourMode.Contains("trees", StringComparison.OrdinalIgnoreCase)).ToList();
		List<Terrain> caveTerrains = terrains.Where(x =>
			x.TerrainBehaviourMode.Contains("cave", StringComparison.OrdinalIgnoreCase)).ToList();
		List<Terrain> roadTerrains = terrains.Where(x =>
				x.Name.Contains("Road", StringComparison.OrdinalIgnoreCase) ||
				x.Name.Contains("Street", StringComparison.OrdinalIgnoreCase) ||
				x.Name.Contains("Trail", StringComparison.OrdinalIgnoreCase) ||
				x.Name.Contains("Alley", StringComparison.OrdinalIgnoreCase))
			.ToList();

		AddElementIfAny(elements,
			"Open exposure leaves the location at the mercy of the wider environment.",
			outdoorsTerrains);
		AddElementIfAny(elements,
			"Weather, light, and distant sound from the surrounding area reach this spot with little resistance.",
			outdoorsTerrains);
		AddElementIfAny(elements,
			"Nearby walls and overhead structure keep the space feeling enclosed.",
			indoorsTerrains);
		AddElementIfAny(elements,
			"The surrounding construction contains sound and movement, giving the area a more sheltered feel.",
			indoorsTerrains);
		AddElementIfAny(elements,
			"Worked surfaces and maintained edges give the place an unmistakably urban character.",
			urbanTerrains);
		AddElementIfAny(elements,
			"Scuffs, repairs, and habitual traffic show in the way the space has been worn down over time.",
			urbanTerrains);
		AddElementIfAny(elements,
			"Domestic details hint at private lives carried on nearby.",
			residentialTerrains);
		AddElementIfAny(elements,
			"The layout suggests exchange, visitors, and repeated daily traffic.",
			commercialTerrains);
		AddElementIfAny(elements,
			"Orderly lines and formal fittings lend the location an official air.",
			administrativeTerrains);
		AddElementIfAny(elements,
			"Heavy-duty materials and stubborn residue hint at hard use and regular labour.",
			industrialTerrains);
		AddElementIfAny(elements,
			"Practical construction and rougher boundaries give the area a distinctly rural feel.",
			ruralTerrains);
		AddElementIfAny(elements,
			"The place feels meant to be shared or passed through rather than held privately.",
			publicTerrains);
		AddElementIfAny(elements,
			"The arrangement of the space suggests ownership, oversight, or restricted use.",
			privateTerrains);
		AddElementIfAny(elements,
			"Natural growth and uneven ground keep the place feeling more grown than built.",
			terrestrialTerrains);
		AddElementIfAny(elements,
			"Overhead growth filters the light through leaves, limbs, or tangled branches.",
			woodedTerrains);
		AddElementIfAny(elements,
			"Vegetation crowds in enough to break sightlines and soften the edges of the terrain.",
			woodedTerrains);
		AddElementIfAny(elements,
			"Moisture, motion, and the persistent presence of water dominate the immediate surroundings.",
			aquaticTerrains);
		AddElementIfAny(elements,
			"Debris, spray, or shifting shoreline traces mark the meeting of land and water.",
			littoralTerrains);
		AddElementIfAny(elements,
			"Nearby flowing water leaves the ground damp and the air lively.",
			riparianTerrains);
		AddElementIfAny(elements,
			"Soft footing and trapped water make the terrain feel uncertain underfoot.",
			wetlandTerrains);
		AddElementIfAny(elements,
			"Dry grit and bleached surfaces suggest long heat and very little moisture.",
			aridTerrains);
		AddElementIfAny(elements,
			"Cold brightness and hard frozen surfaces give the area a stark clarity.",
			glacialTerrains);
		AddElementIfAny(elements,
			"Dark stone, ash, or scorched textures lend the ground a volcanic harshness.",
			volcanicTerrains);
		AddElementIfAny(elements,
			"Stone presses close enough here to swallow light and soften distant sounds.",
			caveTerrains);
		AddElementIfAny(elements,
			"The landscape feels exposed and alien, stripped back to stone, dust, and stark sky.",
			lunarTerrains);
		AddElementIfAny(elements,
			"The surrounding void makes even nearby features feel isolated and remote.",
			spaceTerrains);
		AddElementIfAny(elements,
			"The lack of atmosphere leaves the area exposed in a way few environments ever are.",
			vacuumTerrains);
		AddElementIfAny(elements,
			"The ground looks loose enough to be worked, disturbed, or marked by recent activity.",
			diggableTerrains);
		AddElementIfAny(elements,
			"Loose sand shifts with the smallest disturbance and refuses to keep a clean edge for long.",
			sandTerrains);
		AddElementIfAny(elements,
			"Heavier clay and damp earth lend the ground a dense, stubborn texture.",
			clayTerrains);
		AddElementIfAny(elements,
			"The terrain carries the clear suggestion of travel and repeated passage.",
			roadTerrains);
		AddElementIfAny(elements,
			"Repeated movement has worn a clearer route through the surrounding terrain.",
			roadTerrains,
			weight: 75.0);

		return elements;
	}

	private static Dictionary<long, HashSet<string>> BuildTerrainTagLookup(FuturemudDatabaseContext context,
		IEnumerable<Terrain> terrains)
	{
		Dictionary<long, string> tagsById = context.Tags.ToDictionary(x => x.Id, x => x.Name);
		Dictionary<long, HashSet<string>> result = new();
		foreach (Terrain terrain in terrains)
		{
			HashSet<string> tags = new(StringComparer.OrdinalIgnoreCase);
			if (!string.IsNullOrWhiteSpace(terrain.TagInformation))
			{
				foreach (string value in terrain.TagInformation.Split(',',
					         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
				{
					if (long.TryParse(value, out long tagId) && tagsById.TryGetValue(tagId, out string? tagName))
					{
						tags.Add(tagName);
					}
				}
			}

			result[terrain.Id] = tags;
		}

		return result;
	}

	private static List<Terrain> GetTerrainsByTag(IEnumerable<Terrain> terrains,
		IReadOnlyDictionary<long, HashSet<string>> terrainTags, string tag)
	{
		return terrains.Where(x => terrainTags[x.Id].Contains(tag)).ToList();
	}

	private static bool IsOutdoorsTerrain(Terrain terrain)
	{
		return (CellOutdoorsType)terrain.DefaultCellOutdoorsType is CellOutdoorsType.Outdoors or
			CellOutdoorsType.IndoorsClimateExposed;
	}

	private static string BuildSeededTerrainBaseDescription(Terrain terrain, IReadOnlyCollection<string> tags)
	{
		List<string> fragments = new();
		if (tags.Contains("Urban"))
		{
			fragments.Add("worked, human-made surroundings");
		}

		if (tags.Contains("Rural"))
		{
			fragments.Add("practical, lightly developed ground");
		}

		if (tags.Contains("Aquatic"))
		{
			fragments.Add("the immediate presence of water");
		}

		if (tags.Contains("Wetland"))
		{
			fragments.Add("soft, uncertain footing");
		}

		if (tags.Contains("Arid"))
		{
			fragments.Add("dry heat and gritty surfaces");
		}

		if (tags.Contains("Glacial"))
		{
			fragments.Add("cold, bright frozen terrain");
		}

		if (tags.Contains("Volcanic"))
		{
			fragments.Add("dark stone and scorched texture");
		}

		if (tags.Contains("Lunar") || tags.Contains("Space") || tags.Contains("Vacuum"))
		{
			fragments.Add("an exposed, alien environment");
		}

		string supportingText = fragments.Any()
			? $"The location immediately suggests {fragments.ListToString()}."
			: "The terrain establishes the place before any finer details are noticed.";

		return
			$"{terrain.Name} sets the broad character of this location. {supportingText}";
	}

	private static void AddElementIfAny(ICollection<XElement> elements, string text, IEnumerable<Terrain> terrains,
		string roomNameText = "{0}", double weight = 100.0, IEnumerable<string>? tags = null)
	{
		List<Terrain> terrainList = terrains.Distinct().ToList();
		if (!terrainList.Any())
		{
			return;
		}

		elements.Add(CreateRandomDescriptionElement(text, roomNameText, weight, terrainList, tags));
	}

	private static void EnsureAutobuilderRoomTemplate(FuturemudDatabaseContext context,
		AutobuilderRoomTemplateSeedDefinition definition)
	{
		AutobuilderRoomTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
			context.AutobuilderRoomTemplates,
			definition.Name,
			x => x.Name,
			() =>
			{
				AutobuilderRoomTemplate created = new();
				context.AutobuilderRoomTemplates.Add(created);
				return created;
			});

		template.Name = definition.Name;
		template.TemplateType = definition.TemplateType;
		template.Definition = definition.Definition.ToString();
	}

	private static void EnsureAutobuilderAreaTemplate(FuturemudDatabaseContext context,
		AutobuilderAreaTemplateSeedDefinition definition)
	{
		AutobuilderAreaTemplate template = SeederRepeatabilityHelper.EnsureNamedEntity(
			context.AutobuilderAreaTemplates,
			definition.Name,
			x => x.Name,
			() =>
			{
				AutobuilderAreaTemplate created = new();
				context.AutobuilderAreaTemplates.Add(created);
				return created;
			});

		template.Name = definition.Name;
		template.TemplateType = definition.TemplateType;
		template.Definition = definition.Definition.ToString();
	}

	private static XElement CreateRoomInfoElement(Terrain terrain, string roomName, string roomDescription,
		CellOutdoorsType? outdoorsType = null, double? ambientLightFactor = null, long? foragableProfileId = null)
	{
		CellOutdoorsType outdoorType = outdoorsType ?? (CellOutdoorsType)terrain.DefaultCellOutdoorsType;
		return new XElement("Terrain",
			new XElement("DefaultTerrain", terrain.Id),
			new XElement("RoomName", new XCData(roomName)),
			new XElement("RoomDescription", new XCData(roomDescription)),
			new XElement("OutdoorsType", (int)outdoorType),
			new XElement("CellLightMultiplier", ambientLightFactor ?? GetDefaultAmbientLightFactor(outdoorType)),
			new XElement("ForagableProfile", foragableProfileId ?? terrain.ForagableProfileId)
		);
	}

	private static XElement CreateRoomByTerrainTemplateDefinition(string byline, XElement defaultInfo,
		IEnumerable<XElement> terrainOverrides, bool applyTagsAsFrameworkTags = true)
	{
		return new XElement("Template",
			new XElement("ApplyAutobuilderTagsAsFrameworkTags", applyTagsAsFrameworkTags),
			new XElement("ShowCommandByline", new XCData(byline)),
			new XElement("Default", defaultInfo),
			new XElement("Terrains", terrainOverrides)
		);
	}

	private static XElement CreateRandomDescriptionRoomTemplateDefinition(string byline, XElement defaultInfo,
		IEnumerable<XElement> terrainOverrides, IEnumerable<XElement> descriptionElements,
		string defaultRandomElementExpression = "2+1d2", bool applyTagsAsFrameworkTags = true,
		string addToAllRoomDescriptions = "")
	{
		List<XElement> overrideElements = terrainOverrides
			.Select(x =>
			{
				XElement clone = new(x);
				if (clone.Element("NumberOfRandomElements") == null)
				{
					clone.Add(new XElement("NumberOfRandomElements", new XCData(defaultRandomElementExpression)));
				}

				return clone;
			})
			.ToList();

		return new XElement("Template",
			new XElement("ApplyAutobuilderTagsAsFrameworkTags", applyTagsAsFrameworkTags),
			new XElement("ShowCommandByline", new XCData(byline)),
			new XElement("AddToAllRoomDescriptions", new XCData(addToAllRoomDescriptions)),
			new XElement("Default",
				defaultInfo,
				new XElement("NumberOfRandomElements", new XCData(defaultRandomElementExpression))
			),
			new XElement("Descriptions", descriptionElements),
			new XElement("Terrains", overrideElements)
		);
	}

	private static XElement CreateTerrainRectangleAreaDefinition(string byline, bool connectDiagonals = false)
	{
		return new XElement("Template",
			new XAttribute("connect_diagonals", connectDiagonals),
			new XElement("ShowCommandByLine", new XCData(byline))
		);
	}

	private static XElement CreateTerrainFeatureRectangleAreaDefinition(string byline, bool connectDiagonals = false)
	{
		return CreateTerrainRectangleAreaDefinition(byline, connectDiagonals);
	}

	private static XElement CreateRandomFeaturesAreaDefinition(string byline, IEnumerable<XElement> groups,
		bool connectDiagonals = false)
	{
		XElement template = CreateTerrainRectangleAreaDefinition(byline, connectDiagonals);
		template.Add(new XElement("Groups", groups));
		return template;
	}

	private static XElement CreateRandomDescriptionElement(string text, string roomNameText = "{0}",
		double weight = 100.0, IEnumerable<Terrain>? terrains = null, IEnumerable<string>? tags = null,
		bool mandatoryIfValid = false, int mandatoryPosition = 100000)
	{
		return new XElement("Description",
			new XAttribute("mandatory", mandatoryIfValid),
			new XAttribute("fixedposition", mandatoryPosition),
			new XElement("Text", new XCData(text)),
			new XElement("RoomNameText", new XCData(roomNameText)),
			new XElement("Weight", weight),
			new XElement("Tags", tags?.ListToCommaSeparatedValues() ?? string.Empty),
			new XElement("Terrains",
				from terrain in terrains ?? Enumerable.Empty<Terrain>()
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	private static XElement CreateRoadRandomDescriptionElement(string text, string triggerTag,
		string roomNameText = "{0}", double weight = 100.0, IEnumerable<Terrain>? terrains = null,
		IEnumerable<string>? additionalTags = null, bool mandatoryIfValid = false, int mandatoryPosition = 100000)
	{
		return new XElement("Description",
			new XAttribute("type", "road"),
			new XAttribute("mandatory", mandatoryIfValid),
			new XAttribute("fixedposition", mandatoryPosition),
			new XElement("Text", new XCData(text)),
			new XElement("RoomNameText", new XCData(roomNameText)),
			new XElement("Weight", weight),
			new XElement("Tags",
				(new[] { triggerTag }).Concat(additionalTags ?? Enumerable.Empty<string>())
				.ListToCommaSeparatedValues()),
			new XElement("Terrains",
				from terrain in terrains ?? Enumerable.Empty<Terrain>()
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	private static XElement CreateRandomDescriptionGroupElement(IEnumerable<XElement> subElements, double weight = 100.0,
		bool mandatoryIfValid = false, int mandatoryPosition = 100000)
	{
		return new XElement("Description",
			new XAttribute("type", "group"),
			new XAttribute("mandatory", mandatoryIfValid),
			new XAttribute("fixedposition", mandatoryPosition),
			new XElement("Weight", weight),
			subElements
		);
	}

	private static XElement CreateSimpleFeatureGroupElement(double minimumFeatureDensity, double maximumFeatureDensity,
		int maximumFeaturesPerRoom, IEnumerable<XElement> features)
	{
		return new XElement("Group",
			new XAttribute("type", "simple"),
			new XElement("MinimumFeatureDensity", minimumFeatureDensity),
			new XElement("MaximumFeatureDensity", maximumFeatureDensity),
			new XElement("MaximumFeaturesPerRoom", maximumFeaturesPerRoom),
			new XElement("Features", features)
		);
	}

	private static XElement CreateUniformFeatureGroupElement(int numberOfFeaturesPerRoom, IEnumerable<XElement> features)
	{
		return new XElement("Group",
			new XAttribute("type", "uniform"),
			new XElement("NumberOfFeaturesPerRoom", numberOfFeaturesPerRoom),
			new XElement("Features", features)
		);
	}

	private static XElement CreateRoadFeatureGroupElement(string baseFeature, string straightRoadFeature,
		string crossRoadsFeature, string teeIntersectionFeature, string isolatedRoadFeature,
		string bendInTheRoadFeature, string endOfTheRoadFeature, IEnumerable<Terrain> terrains)
	{
		return new XElement("Group",
			new XAttribute("type", "road"),
			new XElement("BaseFeature", new XCData(baseFeature)),
			new XElement("StraightRoadFeature", new XCData(straightRoadFeature)),
			new XElement("CrossRoadsFeature", new XCData(crossRoadsFeature)),
			new XElement("TeeIntersectionFeature", new XCData(teeIntersectionFeature)),
			new XElement("IsolatedRoadFeature", new XCData(isolatedRoadFeature)),
			new XElement("BendInTheRoadFeature", new XCData(bendInTheRoadFeature)),
			new XElement("EndOfTheRoadFeature", new XCData(endOfTheRoadFeature)),
			new XElement("Terrains",
				from terrain in terrains
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	private static XElement CreateFeatureElement(string name, double weighting = 100.0, int minimumCount = 0,
		int maximumCount = 1, IEnumerable<Terrain>? terrains = null)
	{
		return new XElement("Feature",
			new XAttribute("type", "none"),
			new XElement("Name", new XCData(name)),
			new XElement("Weighting", weighting),
			new XElement("MinimumCount", minimumCount),
			new XElement("MaximumCount", maximumCount),
			new XElement("Terrains",
				from terrain in terrains ?? Enumerable.Empty<Terrain>()
				select new XElement("Terrain", terrain.Id)
			)
		);
	}

	private static XElement CreateAdjacentFeatureElement(string name, string adjacentFeatureTag, bool ignoreExits = false,
		double weighting = 100.0, int minimumCount = 0, int maximumCount = 1, IEnumerable<Terrain>? terrains = null,
		IReadOnlyDictionary<CardinalDirection, string>? directionAdjacencyTags = null)
	{
		return new XElement("Feature",
			new XAttribute("type", "adjacent"),
			new XElement("Name", new XCData(name)),
			new XElement("Weighting", weighting),
			new XElement("MinimumCount", minimumCount),
			new XElement("MaximumCount", maximumCount),
			new XElement("Terrains",
				from terrain in terrains ?? Enumerable.Empty<Terrain>()
				select new XElement("Terrain", terrain.Id)
			),
			new XElement("IgnoreExits", ignoreExits),
			new XElement("AdjacentFeatureTag", new XCData(adjacentFeatureTag)),
			new XElement("Adjacents",
				from tag in directionAdjacencyTags ?? new Dictionary<CardinalDirection, string>()
				select new XElement("Adjacent",
					new XAttribute("direction", (int)tag.Key),
					new XCData(tag.Value))
			)
		);
	}

	private static double GetDefaultAmbientLightFactor(CellOutdoorsType outdoorsType)
	{
		return outdoorsType switch
		{
			CellOutdoorsType.Indoors => 0.25,
			CellOutdoorsType.IndoorsWithWindows => 0.35,
			CellOutdoorsType.Outdoors => 1.0,
			CellOutdoorsType.IndoorsNoLight => 0.0,
			CellOutdoorsType.IndoorsClimateExposed => 0.9,
			_ => 1.0
		};
	}

	private sealed record AutobuilderRoomTemplateSeedDefinition(string Name, string TemplateType, XElement Definition);

	private sealed record AutobuilderAreaTemplateSeedDefinition(string Name, string TemplateType, XElement Definition);
}
