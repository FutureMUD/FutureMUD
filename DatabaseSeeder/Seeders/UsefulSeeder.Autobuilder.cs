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
		WildernessGroupedTerrainRandomDescriptionTemplateName
	];

	private static readonly string[] StockAutobuilderAreaTemplateNames =
	[
		WildernessGroupedTerrainRandomFeaturesAreaTemplateName
	];

	internal static IReadOnlyCollection<string> StockAutobuilderRoomTemplateNamesForTesting =>
		StockAutobuilderRoomTemplateNames;

	internal static IReadOnlyCollection<string> StockAutobuilderAreaTemplateNamesForTesting =>
		StockAutobuilderAreaTemplateNames;

	internal static IReadOnlyCollection<string> StockAutobuilderTagNamesForTesting =>
		WildernessGroupedTerrainAutobuilderTagDefinitions.Select(x => x.Name).ToArray();

	internal static ShouldSeedResult ClassifyAutobuilderPackagePresence(FuturemudDatabaseContext context)
	{
		List<bool> packageChecks =
		[
			.. StockAutobuilderRoomTemplateNames
				.Select(name => context.AutobuilderRoomTemplates.Any(x => x.Name == name)),
			.. StockAutobuilderAreaTemplateNames
				.Select(name => context.AutobuilderAreaTemplates.Any(x => x.Name == name)),
			.. WildernessGroupedTerrainAutobuilderTagDefinitions
				.Select(definition => context.Tags.Any(x => x.Name == definition.Name))
		];

		return SeederRepeatabilityHelper.ClassifyByPresence(packageChecks);
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
				"Could not seed the wilderness autobuilder starter package because no terrains are installed yet. Run the terrain foundations seeding first.");
			return;
		}

		EnsureWildernessTerrainAutobuilderTags(context);

		foreach (AutobuilderRoomTemplateSeedDefinition roomTemplate in
		         BuildWildernessGroupedTerrainAutobuilderRoomTemplates(terrains))
		{
			EnsureAutobuilderRoomTemplate(context, roomTemplate);
		}

		foreach (AutobuilderAreaTemplateSeedDefinition areaTemplate in
		         BuildWildernessGroupedTerrainAutobuilderAreaTemplates(terrains))
		{
			EnsureAutobuilderAreaTemplate(context, areaTemplate);
		}

		context.SaveChanges();
	}

	private static void EnsureWildernessTerrainAutobuilderTags(FuturemudDatabaseContext context)
	{
		DictionaryWithDefault<string, Tag> tags =
			context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);

		foreach ((string name, string parent) in WildernessGroupedTerrainAutobuilderTagDefinitions)
		{
			EnsureAutobuilderTag(context, tags, name, parent);
		}
	}

	private static void EnsureAutobuilderTag(FuturemudDatabaseContext context,
		DictionaryWithDefault<string, Tag> tags, string name, string parent)
	{
		if (tags.Any(x => x.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)))
		{
			return;
		}

		Tag tag = new()
		{
			Name = name,
			Parent = string.IsNullOrEmpty(parent) ? null : tags[parent]
		};
		tags[name] = tag;
		context.Tags.Add(tag);
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
