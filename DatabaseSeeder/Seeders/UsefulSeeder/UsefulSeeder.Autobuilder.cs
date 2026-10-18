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
				.Select(name => context.AutobuilderAreaTemplates.Any(x => x.Name == name))
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

        var resolvedTags = CoreTagCatalogue.ResolveFamily(context, "useful_autobuilder");
        var tagBindings = new XElement("FrameworkTagBindings",
            CoreTagCatalogue.Declarations.Where(x => x.Family == "useful_autobuilder")
                .DistinctBy(x => x.Path).Select(x => new XElement("Binding",
                    new XAttribute("selector", x.Name), new XAttribute("id", resolvedTags[x.Path].Id))));

		foreach (AutobuilderRoomTemplateSeedDefinition roomTemplate in
		         BuildWildernessGroupedTerrainAutobuilderRoomTemplates(terrains))
		{
            roomTemplate.Definition.Add(new XElement(tagBindings));
			EnsureAutobuilderRoomTemplate(context, roomTemplate, errors);
		}

		foreach (AutobuilderAreaTemplateSeedDefinition areaTemplate in
		         BuildWildernessGroupedTerrainAutobuilderAreaTemplates(terrains))
		{
			EnsureAutobuilderAreaTemplate(context, areaTemplate, errors);
		}

		context.SaveChanges();
	}

	private static string BuildSeededTerrainBaseDescription(Terrain terrain, IReadOnlyCollection<string> tags)
	{
		List<string> fragments = new();
        if (terrain.Name.Equals("Sewer", StringComparison.OrdinalIgnoreCase)) fragments.Add("sludge underfoot in the unlit passage");
        if (terrain.Name.Equals("Ruins", StringComparison.OrdinalIgnoreCase)) fragments.Add("dusty ground and scattered stone amid the ruined site");
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

    private static SeederManagedRecord? TemplateRecord(FuturemudDatabaseContext context, string entityType, string name)
    {
        var key = "useful.autobuilder." + entityType + "." + name;
        var records = context.SeederManagedRecords.Where(x => x.Seeder == nameof(UsefulSeeder) && x.StableKey == key).ToList();
        if (records.Count > 1 || records.Any(x => x.Module != "Autobuilder" || x.EntityType != entityType ||
                x.Retired || x.LogicalId is null || x.RevisionNumber is not null))
            throw new InvalidOperationException($"Invalid autobuilder ownership for {key}; review provenance before rerunning.");
        var record = records.SingleOrDefault();
        if (record is not null && context.SeederManagedRecords.Any(x => x.Seeder == nameof(UsefulSeeder) &&
                x.EntityType == entityType && x.LogicalId == record.LogicalId && x.StableKey != key))
            throw new InvalidOperationException($"Shared autobuilder identity for {key}; review provenance before rerunning.");
        return record;
    }

    private static bool MayReconcileTemplate(SeederManagedRecord? record, string name,
        long? id, string currentName, string current, ICollection<string> errors)
    {
        if (record is not null && id is null)
            throw new InvalidOperationException($"Missing owned autobuilder template {name} (#{record.LogicalId}); explicit reconciliation is required.");
        if (id is not null && (record is null || currentName != name || record.SeedBaseline != current))
        {
            errors.Add($"Preserved unowned, renamed or builder-edited autobuilder template {name} (#{id}). Legacy named selectors may be ambiguous; review before adopting explicit Core bindings.");
            return false;
        }
        return true;
    }

    private static void RecordTemplate(FuturemudDatabaseContext context, string entityType, string name, long id, string definition)
    {
        var key = "useful.autobuilder." + entityType + "." + name;
        var record = context.SeederManagedRecords.SingleOrDefault(x => x.Seeder == nameof(UsefulSeeder) && x.Module == "Autobuilder" && x.StableKey == key);
        if (record is null)
        {
            record = new SeederManagedRecord { Seeder = nameof(UsefulSeeder), Module = "Autobuilder", EntityType = entityType, StableKey = key, LogicalId = id };
            context.SeederManagedRecords.Add(record);
        }
        record.SeedBaseline = definition;
        record.ManifestVersion = "2026-10-18";
        record.AppliedAt = DateTime.UtcNow;
        context.SaveChanges();
    }

    private static void EnsureAutobuilderRoomTemplate(FuturemudDatabaseContext context,
        AutobuilderRoomTemplateSeedDefinition definition, ICollection<string> errors)
    {
        var record = TemplateRecord(context, nameof(AutobuilderRoomTemplate), definition.Name);
        var template = record is null
            ? context.AutobuilderRoomTemplates.SingleOrDefault(x => x.Name == definition.Name)
            : context.AutobuilderRoomTemplates.SingleOrDefault(x => x.Id == record.LogicalId);
        if (!MayReconcileTemplate(record, definition.Name, template?.Id, template?.Name ?? "",
            template?.Definition ?? "", errors)) return;
        if (template is null)
        {
            template = new AutobuilderRoomTemplate { Name = definition.Name };
            context.AutobuilderRoomTemplates.Add(template);
        }
        template.TemplateType = definition.TemplateType;
        template.Definition = definition.Definition.ToString();
        context.SaveChanges();
        RecordTemplate(context, nameof(AutobuilderRoomTemplate), template.Name, template.Id, template.Definition);
    }

    private static void EnsureAutobuilderAreaTemplate(FuturemudDatabaseContext context,
        AutobuilderAreaTemplateSeedDefinition definition, ICollection<string> errors)
    {
        var record = TemplateRecord(context, nameof(AutobuilderAreaTemplate), definition.Name);
        var template = record is null
            ? context.AutobuilderAreaTemplates.SingleOrDefault(x => x.Name == definition.Name)
            : context.AutobuilderAreaTemplates.SingleOrDefault(x => x.Id == record.LogicalId);
        if (!MayReconcileTemplate(record, definition.Name, template?.Id, template?.Name ?? "",
            template?.Definition ?? "", errors)) return;
        if (template is null)
        {
            template = new AutobuilderAreaTemplate { Name = definition.Name };
            context.AutobuilderAreaTemplates.Add(template);
        }
        template.TemplateType = definition.TemplateType;
        template.Definition = definition.Definition.ToString();
        context.SaveChanges();
        RecordTemplate(context, nameof(AutobuilderAreaTemplate), template.Name, template.Id, template.Definition);
    }

	private static XElement CreateRoomInfoElement(Terrain terrain, string roomName, string roomDescription,
		RoomOutdoorsType? outdoorsType = null, double? ambientLightFactor = null, long? foragableProfileId = null)
	{
		RoomOutdoorsType outdoorType = outdoorsType ?? (RoomOutdoorsType)terrain.DefaultRoomOutdoorsType;
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

	private static double GetDefaultAmbientLightFactor(RoomOutdoorsType outdoorsType)
	{
		return outdoorsType switch
		{
			RoomOutdoorsType.Indoors => 0.25,
			RoomOutdoorsType.IndoorsWithWindows => 0.35,
			RoomOutdoorsType.Outdoors => 1.0,
			RoomOutdoorsType.IndoorsNoLight => 0.0,
			RoomOutdoorsType.IndoorsClimateExposed => 0.9,
			_ => 1.0
		};
	}

	private sealed record AutobuilderRoomTemplateSeedDefinition(string Name, string TemplateType, XElement Definition);

	private sealed record AutobuilderAreaTemplateSeedDefinition(string Name, string TemplateType, XElement Definition);
}
