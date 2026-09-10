#nullable enable

using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace DatabaseSeeder.Seeders;

/// <summary>
/// Wilderness-heavy grouped terrain autobuilder content for the UsefulSeeder stock package.
/// It expands per-tag prose variation, adds time/weather/season-reactive sound and smell elements,
/// and leans hard toward outdoor and road content rather than urban detail.
/// </summary>
public partial class UsefulSeeder
{
    private const string WildernessGroupedTerrainRandomDescriptionTemplateName = "Seeded Terrain Wilderness Grouped Description";
    private const string WildernessGroupedTerrainRandomFeaturesAreaTemplateName = "Seeded Terrain Wilderness Grouped Features";
    private const string WildernessGroupedTerrainRandomFeaturesAreaTemplateType = "room by terrain random features";
    private const string WildernessGroupedTerrainPrimaryPhysicalLayerTag = "Physical Primary";
    private const string WildernessGroupedTerrainSecondaryPhysicalLayerTag = "Physical Secondary";

    internal static IReadOnlyCollection<(string Name, string Parent)> WildernessGroupedTerrainAutobuilderTagDefinitions =>
        WildernessGroupedTerrainAutobuilderTagDefinitionsData;

    private static readonly (string Name, string Parent)[] WildernessGroupedTerrainAutobuilderTagDefinitionsData =
    [
        (@"Feature", @"Terrain"),
        (@"Descriptive Element", @"Feature"),
        (@"Cave Feature", @"Descriptive Element"),
        (@"Desert Feature", @"Descriptive Element"),
        (@"Extraterrestrial Feature", @"Descriptive Element"),
        (@"Forest Feature", @"Descriptive Element"),
        (@"Glacial Feature", @"Descriptive Element"),
        (@"Open Land Feature", @"Descriptive Element"),
        (@"Road Feature", @"Descriptive Element"),
        (@"Rock Feature", @"Descriptive Element"),
        (@"Urban Feature", @"Descriptive Element"),
        (@"Volcanic Feature", @"Descriptive Element"),
        (@"Water Feature", @"Descriptive Element"),
        (@"Wetland Feature", @"Descriptive Element"),
        (@"Worn Furnishings", @"Urban Feature"),
        (@"Orderly Domestic Arrangement", @"Urban Feature"),
        (@"Soft Household Clutter", @"Urban Feature"),
        (@"Recent Cleaning", @"Urban Feature"),
        (@"Personal Touches", @"Urban Feature"),
        (@"Window Light", @"Urban Feature"),
        (@"Muted Textiles", @"Urban Feature"),
        (@"Lingering Cooking Smell", @"Urban Feature"),
        (@"Damp Corners", @"Urban Feature"),
        (@"Long Corridor Sightline", @"Urban Feature"),
        (@"Echoing Floorboards", @"Urban Feature"),
        (@"Wall Niches", @"Urban Feature"),
        (@"Drafty Passage", @"Urban Feature"),
        (@"Foot Traffic Wear", @"Urban Feature"),
        (@"Communal Benches", @"Urban Feature"),
        (@"Training Scuffs", @"Urban Feature"),
        (@"Stacked Goods", @"Urban Feature"),
        (@"Tool Marks", @"Urban Feature"),
        (@"Paperwork Tidy", @"Urban Feature"),
        (@"Loading Space", @"Urban Feature"),
        (@"Machine Residue", @"Urban Feature"),
        (@"Shuttered Front", @"Urban Feature"),
        (@"Stall Layout", @"Urban Feature"),
        (@"Storage Racks", @"Urban Feature"),
        (@"Iron Fittings", @"Urban Feature"),
        (@"Chained Fixtures", @"Urban Feature"),
        (@"Stale Air", @"Urban Feature"),
        (@"Dripping Masonry", @"Urban Feature"),
        (@"Close Ceiling", @"Urban Feature"),
        (@"Steam Haze", @"Urban Feature"),
        (@"Mineral Warmth", @"Urban Feature"),
        (@"Still Water", @"Urban Feature"),
        (@"Exposed Roofline", @"Urban Feature"),
        (@"Defensive Height", @"Urban Feature"),
        (@"Gate Traffic", @"Urban Feature"),
        (@"Narrow Frontages", @"Urban Feature"),
        (@"Laundry Lines", @"Urban Feature"),
        (@"Broken Paving", @"Urban Feature"),
        (@"Refuse Scatter", @"Urban Feature"),
        (@"Encroaching Weeds", @"Urban Feature"),
        (@"Drainage Channel", @"Urban Feature"),
        (@"Steady Traffic", @"Urban Feature"),
        (@"Mixed Frontages", @"Urban Feature"),
        (@"Street Trees", @"Urban Feature"),
        (@"Clean Stonework", @"Urban Feature"),
        (@"Quiet Verge", @"Urban Feature"),
        (@"Formal Planting", @"Urban Feature"),
        (@"Broad Pavement", @"Urban Feature"),
        (@"Open Garden Edge", @"Urban Feature"),
        (@"Public Monuments", @"Urban Feature"),
        (@"Trim Lawn", @"Urban Feature"),
        (@"Flower Beds", @"Urban Feature"),
        (@"Market Stalls", @"Urban Feature"),
        (@"Hard-Worn Ground", @"Urban Feature"),
        (@"Seating Cluster", @"Urban Feature"),
        (@"Fence Lines", @"Urban Feature"),
        (@"Rotting Heap", @"Urban Feature"),
        (@"Carrion Interest", @"Urban Feature"),
        (@"Smoke Stain", @"Urban Feature"),
        (@"Trampled Verge", @"Road Feature"),
        (@"Narrow Tread", @"Road Feature"),
        (@"Root-Broken Path", @"Road Feature"),
        (@"Beast Tracks", @"Road Feature"),
        (@"Dusty Ruts", @"Road Feature"),
        (@"Compacted Wheel Marks", @"Road Feature"),
        (@"Loose Gravel Scatter", @"Road Feature"),
        (@"Drainage Ditch", @"Road Feature"),
        (@"Stone Kerb", @"Road Feature"),
        (@"Even Camber", @"Road Feature"),
        (@"Weed Through Cracks", @"Road Feature"),
        (@"Roadside Marker", @"Road Feature"),
        (@"Tall Grass", @"Open Land Feature"),
        (@"Short Grass", @"Open Land Feature"),
        (@"Wildflowers", @"Open Land Feature"),
        (@"Seedhead Sweep", @"Open Land Feature"),
        (@"Scattered Stones", @"Open Land Feature"),
        (@"Animal Run", @"Open Land Feature"),
        (@"Wind-Pressed Grass", @"Open Land Feature"),
        (@"Dry Patches", @"Open Land Feature"),
        (@"Shallow Swale", @"Open Land Feature"),
        (@"Scattered Trees", @"Open Land Feature"),
        (@"Hard-Baked Soil", @"Open Land Feature"),
        (@"Thorn Scrub", @"Open Land Feature"),
        (@"Dry Seed Pods", @"Open Land Feature"),
        (@"Shade Tree", @"Open Land Feature"),
        (@"Low Shrub", @"Open Land Feature"),
        (@"Resinous Brush", @"Open Land Feature"),
        (@"Tangle of Thorns", @"Open Land Feature"),
        (@"Bare Earth Patches", @"Open Land Feature"),
        (@"Insect Hum", @"Open Land Feature"),
        (@"Lichen Mat", @"Open Land Feature"),
        (@"Frost-Hardened Ground", @"Open Land Feature"),
        (@"Sparse Sedge", @"Open Land Feature"),
        (@"Water-Laid Silt", @"Open Land Feature"),
        (@"Reed Fringe", @"Open Land Feature"),
        (@"Debris Snag", @"Open Land Feature"),
        (@"Salt Crust", @"Open Land Feature"),
        (@"Eroded Gullies", @"Open Land Feature"),
        (@"Cracked Earth", @"Open Land Feature"),
        (@"Chalky Dust", @"Open Land Feature"),
        (@"Rolling Rise", @"Rock Feature"),
        (@"Sheltered Hollow", @"Rock Feature"),
        (@"Boulder Scatter", @"Rock Feature"),
        (@"Exposed Bedrock", @"Rock Feature"),
        (@"Layered Rock", @"Rock Feature"),
        (@"Wind-Carved Stone", @"Rock Feature"),
        (@"Broken Scree", @"Rock Feature"),
        (@"Loose Talus", @"Rock Feature"),
        (@"Sharp Drop", @"Rock Feature"),
        (@"Narrow Defile", @"Rock Feature"),
        (@"High Rim", @"Rock Feature"),
        (@"Cliff Shadow", @"Rock Feature"),
        (@"Steep Grade", @"Rock Feature"),
        (@"Stone Overhang", @"Rock Feature"),
        (@"River Cut", @"Rock Feature"),
        (@"Echoing Walls", @"Rock Feature"),
        (@"Dense Canopy", @"Forest Feature"),
        (@"Open Canopy", @"Forest Feature"),
        (@"Mixed Leaf Litter", @"Forest Feature"),
        (@"Clear Understory", @"Forest Feature"),
        (@"Thick Underbrush", @"Forest Feature"),
        (@"Mossed Trunkfall", @"Forest Feature"),
        (@"Broadleaf Shade", @"Forest Feature"),
        (@"Conifer Needles", @"Forest Feature"),
        (@"Resin Scent", @"Forest Feature"),
        (@"Boggy Roots", @"Forest Feature"),
        (@"Rain-Heavy Leaves", @"Forest Feature"),
        (@"Hanging Vines", @"Forest Feature"),
        (@"Giant Trunks", @"Forest Feature"),
        (@"Ferny Floor", @"Forest Feature"),
        (@"Ordered Rows", @"Forest Feature"),
        (@"Fruiting Trees", @"Forest Feature"),
        (@"Managed Copse", @"Forest Feature"),
        (@"Sunlit Glade", @"Forest Feature"),
        (@"Standing Water", @"Wetland Feature"),
        (@"Reed Bed", @"Wetland Feature"),
        (@"Rush Clumps", @"Wetland Feature"),
        (@"Mud Churn", @"Wetland Feature"),
        (@"Sphagnum Mat", @"Wetland Feature"),
        (@"Peaty Ground", @"Wetland Feature"),
        (@"Brackish Slick", @"Wetland Feature"),
        (@"Tidal Mud", @"Wetland Feature"),
        (@"Mangrove Roots", @"Wetland Feature"),
        (@"Mosquito Swarm", @"Wetland Feature"),
        (@"Waterlogged Timber", @"Wetland Feature"),
        (@"Dune Face", @"Desert Feature"),
        (@"Wind Rippled Sand", @"Desert Feature"),
        (@"Drift Ridge", @"Desert Feature"),
        (@"Bleached Stone", @"Desert Feature"),
        (@"Heat-Shattered Rock", @"Desert Feature"),
        (@"Desert Pavement", @"Desert Feature"),
        (@"Salted Breeze", @"Desert Feature"),
        (@"Palm Shade", @"Desert Feature"),
        (@"Spring Water", @"Desert Feature"),
        (@"Green Fringe", @"Desert Feature"),
        (@"Ash Dust", @"Volcanic Feature"),
        (@"Cooling Basalt", @"Volcanic Feature"),
        (@"Broken Obsidian", @"Volcanic Feature"),
        (@"Sulphur Reek", @"Volcanic Feature"),
        (@"Blackened Crack", @"Volcanic Feature"),
        (@"Heat Haze", @"Volcanic Feature"),
        (@"Cindered Hollow", @"Volcanic Feature"),
        (@"Fumarole Stain", @"Volcanic Feature"),
        (@"Wind-Hardened Snow", @"Glacial Feature"),
        (@"Fresh Drift", @"Glacial Feature"),
        (@"Blue Ice", @"Glacial Feature"),
        (@"Crevasse Hint", @"Glacial Feature"),
        (@"Old Meltwater", @"Glacial Feature"),
        (@"Sastrugi Ridges", @"Glacial Feature"),
        (@"Frost Crystals", @"Glacial Feature"),
        (@"Snow Blind Glare", @"Glacial Feature"),
        (@"Dripping Water", @"Cave Feature"),
        (@"Still Pool", @"Cave Feature"),
        (@"Mineral Stain", @"Cave Feature"),
        (@"Jagged Stalactites", @"Cave Feature"),
        (@"Echoing Chamber", @"Cave Feature"),
        (@"Powdery Dust", @"Cave Feature"),
        (@"Slick Stone", @"Cave Feature"),
        (@"Narrow Throat", @"Cave Feature"),
        (@"Underground Current", @"Cave Feature"),
        (@"Darkness Pocket", @"Cave Feature"),
        (@"Foam Line", @"Water Feature"),
        (@"Drift Line", @"Water Feature"),
        (@"Pebble Wash", @"Water Feature"),
        (@"Sand Ripple", @"Water Feature"),
        (@"Mud Slick", @"Water Feature"),
        (@"Gentle Lapping", @"Water Feature"),
        (@"Spray Marks", @"Water Feature"),
        (@"Kelp Wrack", @"Water Feature"),
        (@"Tide Pool Basin", @"Water Feature"),
        (@"Fast Current", @"Water Feature"),
        (@"Braided Flow", @"Water Feature"),
        (@"Deep Channel", @"Water Feature"),
        (@"Reed Margin", @"Water Feature"),
        (@"Waterweed", @"Water Feature"),
        (@"Open Fetch", @"Water Feature"),
        (@"Swell Lift", @"Water Feature"),
        (@"Coral Heads", @"Water Feature"),
        (@"Glassy Surface", @"Water Feature"),
        (@"Sounding Depths", @"Water Feature"),
        (@"Dusty Regolith", @"Extraterrestrial Feature"),
        (@"Glassy Impact Fragments", @"Extraterrestrial Feature"),
        (@"Crater Lip", @"Extraterrestrial Feature"),
        (@"Basalt Sheet", @"Extraterrestrial Feature"),
        (@"Bright Highlands", @"Extraterrestrial Feature"),
        (@"Jagged Rubble", @"Extraterrestrial Feature"),
        (@"Hard Vacuum Silence", @"Extraterrestrial Feature"),
        (@"Orbital Debris Glint", @"Extraterrestrial Feature"),
        (@"Planetary Arc", @"Extraterrestrial Feature"),
        (@"Sparse Starlight", @"Extraterrestrial Feature"),
        (@"Dense Starfield", @"Extraterrestrial Feature"),
        (@"Distant Nebula", @"Extraterrestrial Feature"),
        (@"Whitecaps", @"Water Feature"),
        (@"Long Swell", @"Water Feature"),
        (@"Pelagic Stillness", @"Water Feature"),
        (@"Horizon Blur", @"Water Feature"),
        (@"Sunlit Hull Glint", @"Extraterrestrial Feature"),
        (@"Station Shadow", @"Extraterrestrial Feature"),
        (@"Remote Beacon", @"Extraterrestrial Feature"),
        (@"Void Blackness", @"Extraterrestrial Feature"),
        (@"Galactic Haze", @"Extraterrestrial Feature"),
        (@"Slow Tumble", @"Extraterrestrial Feature"),
        (@"Avalanche Debris", @"Rock Feature"),
        (@"Corniced Edge", @"Rock Feature"),
        (@"Distant Galaxy Smear", @"Extraterrestrial Feature"),
        (@"Resource", @"Terrain"),
        (@"Aquatic Resource", @"Resource"),
        (@"Botanical Resource", @"Resource"),
        (@"Hydrological Resource", @"Resource"),
        (@"Mineral Resource", @"Resource"),
        (@"Diggable Soil Deposit", @"Mineral Resource"),
        (@"Clay Deposit", @"Mineral Resource"),
        (@"Sand Deposit", @"Mineral Resource"),
        (@"Peat Deposit", @"Botanical Resource"),
        (@"Reed Harvest", @"Botanical Resource"),
        (@"Timber Stand", @"Botanical Resource"),
        (@"Fruit Grove", @"Botanical Resource"),
        (@"Herb Patch", @"Botanical Resource"),
        (@"Salt Deposit", @"Mineral Resource"),
        (@"Stone Deposit", @"Mineral Resource"),
        (@"Ore Vein", @"Mineral Resource"),
        (@"Obsidian Deposit", @"Mineral Resource"),
        (@"Sulphur Deposit", @"Mineral Resource"),
        (@"Freshwater Spring", @"Hydrological Resource"),
        (@"Fish Shoal", @"Aquatic Resource"),
        (@"Coral Growth", @"Aquatic Resource"),
        (@"Ice Block", @"Mineral Resource"),
        (@"Road Topology", @"Road Feature"),
        (@"Animal Trail", @"Road Topology"),
        (@"Animal Trail Straight", @"Road Topology"),
        (@"Animal Trail Cross", @"Road Topology"),
        (@"Animal Trail Tee", @"Road Topology"),
        (@"Animal Trail Isolated", @"Road Topology"),
        (@"Animal Trail Bend", @"Road Topology"),
        (@"Animal Trail End", @"Road Topology"),
        (@"Trail", @"Road Topology"),
        (@"Trail Straight", @"Road Topology"),
        (@"Trail Cross", @"Road Topology"),
        (@"Trail Tee", @"Road Topology"),
        (@"Trail Isolated", @"Road Topology"),
        (@"Trail Bend", @"Road Topology"),
        (@"Trail End", @"Road Topology"),
        (@"Dirt Road", @"Road Topology"),
        (@"Dirt Road Straight", @"Road Topology"),
        (@"Dirt Road Cross", @"Road Topology"),
        (@"Dirt Road Tee", @"Road Topology"),
        (@"Dirt Road Isolated", @"Road Topology"),
        (@"Dirt Road Bend", @"Road Topology"),
        (@"Dirt Road End", @"Road Topology"),
        (@"Compacted Dirt Road", @"Road Topology"),
        (@"Compacted Dirt Road Straight", @"Road Topology"),
        (@"Compacted Dirt Road Cross", @"Road Topology"),
        (@"Compacted Dirt Road Tee", @"Road Topology"),
        (@"Compacted Dirt Road Isolated", @"Road Topology"),
        (@"Compacted Dirt Road Bend", @"Road Topology"),
        (@"Compacted Dirt Road End", @"Road Topology"),
        (@"Gravel Road", @"Road Topology"),
        (@"Gravel Road Straight", @"Road Topology"),
        (@"Gravel Road Cross", @"Road Topology"),
        (@"Gravel Road Tee", @"Road Topology"),
        (@"Gravel Road Isolated", @"Road Topology"),
        (@"Gravel Road Bend", @"Road Topology"),
        (@"Gravel Road End", @"Road Topology"),
        (@"Cobblestone Road", @"Road Topology"),
        (@"Cobblestone Road Straight", @"Road Topology"),
        (@"Cobblestone Road Cross", @"Road Topology"),
        (@"Cobblestone Road Tee", @"Road Topology"),
        (@"Cobblestone Road Isolated", @"Road Topology"),
        (@"Cobblestone Road Bend", @"Road Topology"),
        (@"Cobblestone Road End", @"Road Topology"),
        (@"Asphalt Road", @"Road Topology"),
        (@"Asphalt Road Straight", @"Road Topology"),
        (@"Asphalt Road Cross", @"Road Topology"),
        (@"Asphalt Road Tee", @"Road Topology"),
        (@"Asphalt Road Isolated", @"Road Topology"),
        (@"Asphalt Road Bend", @"Road Topology"),
        (@"Asphalt Road End", @"Road Topology"),
        (@"Sensory Element", @"Feature"),
        (@"Soundscape", @"Sensory Element"),
        (@"Smellscape", @"Sensory Element"),
        (@"Open Land Sound", @"Soundscape"),
        (@"Forest Sound", @"Soundscape"),
        (@"Wetland Sound", @"Soundscape"),
        (@"Water Sound", @"Soundscape"),
        (@"Desert Sound", @"Soundscape"),
        (@"Volcanic Sound", @"Soundscape"),
        (@"Glacial Sound", @"Soundscape"),
        (@"Cave Sound", @"Soundscape"),
        (@"Extraterrestrial Sound", @"Soundscape"),
        (@"Open Land Smell", @"Smellscape"),
        (@"Forest Smell", @"Smellscape"),
        (@"Wetland Smell", @"Smellscape"),
        (@"Water Smell", @"Smellscape"),
        (@"Desert Smell", @"Smellscape"),
        (@"Volcanic Smell", @"Smellscape"),
        (@"Glacial Smell", @"Smellscape"),
        (@"Cave Smell", @"Smellscape"),
        (@"Wind Through Grass", @"Open Land Sound"),
        (@"Field Birds", @"Open Land Sound"),
        (@"Grasshopper Chirr", @"Open Land Sound"),
        (@"Animal Rustle", @"Forest Sound"),
        (@"Bird Chorus", @"Forest Sound"),
        (@"Canopy Insects", @"Forest Sound"),
        (@"Needle Wind", @"Forest Sound"),
        (@"Frog Chorus", @"Wetland Sound"),
        (@"Mosquito Whine", @"Wetland Sound"),
        (@"Reed Rustle", @"Wetland Sound"),
        (@"Waterfowl Calls", @"Water Sound"),
        (@"Surf Wash", @"Water Sound"),
        (@"Shorebird Calls", @"Water Sound"),
        (@"Water Murmur", @"Water Sound"),
        (@"River Rush", @"Water Sound"),
        (@"Lake Lapping", @"Water Sound"),
        (@"Desert Wind", @"Desert Sound"),
        (@"Heat Silence", @"Desert Sound"),
        (@"Vent Hiss", @"Volcanic Sound"),
        (@"Ice Creak", @"Glacial Sound"),
        (@"Wind Keening", @"Glacial Sound"),
        (@"Cave Drip Echo", @"Cave Sound"),
        (@"Hollow Quiet", @"Cave Sound"),
        (@"Bat Flutter", @"Cave Sound"),
        (@"Vacuum Silence", @"Extraterrestrial Sound"),
        (@"Crushed Grass Scent", @"Open Land Smell"),
        (@"Wildflower Sweetness", @"Open Land Smell"),
        (@"Dry Earth Smell", @"Open Land Smell"),
        (@"Animal Musk", @"Open Land Smell"),
        (@"Leaf Mold Scent", @"Forest Smell"),
        (@"Humid Rot Smell", @"Forest Smell"),
        (@"Wet Earth Scent", @"Wetland Smell"),
        (@"Peat Reek", @"Wetland Smell"),
        (@"Brackish Rot Smell", @"Wetland Smell"),
        (@"Salt Spray", @"Water Smell"),
        (@"Kelp Tang", @"Water Smell"),
        (@"River Silt Smell", @"Water Smell"),
        (@"Hot Dust Smell", @"Desert Smell"),
        (@"Sulphur Tang", @"Volcanic Smell"),
        (@"Clean Cold Scent", @"Glacial Smell"),
        (@"Mineral Damp", @"Cave Smell"),
        (@"Needle Resin Smell", @"Forest Smell")
    ];

    private sealed record WildernessGroupedTerrainDomainSpec(
        string Intro,
        string[] PrimaryFeatures,
        string[] SecondaryFeatures,
        string[] SoundFeatures,
        string[] SmellFeatures,
        string[] ResourceFeatures);

    private sealed record WildernessGroupedTerrainFeatureSpec(
        string Name,
        string ParentTag,
        string[] Variants,
        double GroupWeight = 100.0,
        bool IsResource = false);

    private static IReadOnlyList<AutobuilderRoomTemplateSeedDefinition> BuildWildernessGroupedTerrainAutobuilderRoomTemplates(
        IReadOnlyCollection<Terrain> terrains)
    {
        IReadOnlyDictionary<string, string> terrainDomains = BuildWildernessGroupedTerrainTerrainDomainLookup();
        IReadOnlyDictionary<string, WildernessGroupedTerrainDomainSpec> domains = BuildWildernessGroupedTerrainDomainSpecs();

        Terrain defaultTerrain = terrains.FirstOrDefault(x => x.DefaultTerrain) ?? terrains.OrderBy(x => x.Id).First();
        List<XElement> roomInfos = terrains
            .Select(terrain =>
            {
                string description = terrainDomains.TryGetValue(terrain.Name, out string? domainKey) &&
                                     domains.TryGetValue(domainKey, out WildernessGroupedTerrainDomainSpec? spec)
                    ? spec.Intro
                    : BuildSeededTerrainBaseDescription(terrain, Array.Empty<string>());

                return CreateRoomInfoElement(terrain, terrain.Name, description);
            })
            .ToList();

        XElement defaultInfo = roomInfos.First(x => long.Parse(x.Element("DefaultTerrain")!.Value) == defaultTerrain.Id);
        List<XElement> overrides = roomInfos
            .Where(x => long.Parse(x.Element("DefaultTerrain")!.Value) != defaultTerrain.Id)
            .ToList();

        return
        [
            new AutobuilderRoomTemplateSeedDefinition(
                WildernessGroupedTerrainRandomDescriptionTemplateName,
                "room random description",
                CreateRandomDescriptionRoomTemplateDefinition(
                    "Wilderness-focused grouped terrain descriptions with two physical feature layers, optional sensory layers, and road-topology-aware prose.",
                    defaultInfo,
                    overrides,
                    BuildWildernessGroupedTerrainDescriptionElements(terrains),
                    defaultRandomElementExpression: "2+1d2",
                    applyTagsAsFrameworkTags: true
                )
            )
        ];
    }

    private static IReadOnlyList<AutobuilderAreaTemplateSeedDefinition> BuildWildernessGroupedTerrainAutobuilderAreaTemplates(
        IReadOnlyCollection<Terrain> terrains)
    {
        List<Terrain> allTerrains = terrains.ToList();
        IReadOnlyDictionary<string, Terrain> terrainByName = terrains
            .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        IReadOnlyDictionary<string, WildernessGroupedTerrainDomainSpec> domains = BuildWildernessGroupedTerrainDomainSpecs();
        IReadOnlyDictionary<string, string[]> domainTerrains = BuildWildernessGroupedTerrainDomainTerrainNames();

        List<XElement> primaryFeatures = new();
        List<XElement> secondaryFeatures = new();
        List<XElement> soundFeatures = new();
        List<XElement> smellFeatures = new();
        List<XElement> resourceFeatures = new();

        foreach ((string domainKey, WildernessGroupedTerrainDomainSpec spec) in domains)
        {
            if (!domainTerrains.TryGetValue(domainKey, out string[]? terrainNames))
            {
                continue;
            }

            List<Terrain> domainTerrainList = GetTerrainsByNames(terrainByName, terrainNames);
            if (!domainTerrainList.Any())
            {
                continue;
            }

            foreach (string feature in spec.PrimaryFeatures)
            {
                primaryFeatures.Add(CreateFeatureElement(feature, terrains: domainTerrainList));
            }

            foreach (string feature in spec.SecondaryFeatures)
            {
                secondaryFeatures.Add(CreateFeatureElement(feature, terrains: domainTerrainList));
            }

            foreach (string feature in spec.SoundFeatures)
            {
                soundFeatures.Add(CreateFeatureElement(feature, terrains: domainTerrainList));
            }

            foreach (string feature in spec.SmellFeatures)
            {
                smellFeatures.Add(CreateFeatureElement(feature, terrains: domainTerrainList));
            }

            foreach (string feature in spec.ResourceFeatures)
            {
                resourceFeatures.Add(CreateFeatureElement(feature, terrains: domainTerrainList));
            }
        }

        List<XElement> groups = new();
        groups.Add(CreateUniformFeatureGroupElement(1,
            new[] { CreateFeatureElement(WildernessGroupedTerrainPrimaryPhysicalLayerTag, terrains: allTerrains) }));
        groups.Add(CreateUniformFeatureGroupElement(1,
            new[] { CreateFeatureElement(WildernessGroupedTerrainSecondaryPhysicalLayerTag, terrains: allTerrains) }));
        if (primaryFeatures.Any()) groups.Add(CreateUniformFeatureGroupElement(1, primaryFeatures));
        if (secondaryFeatures.Any()) groups.Add(CreateUniformFeatureGroupElement(1, secondaryFeatures));
        if (soundFeatures.Any()) groups.Add(CreateSimpleFeatureGroupElement(0.35, 0.65, 1, soundFeatures));
        if (smellFeatures.Any()) groups.Add(CreateSimpleFeatureGroupElement(0.25, 0.55, 1, smellFeatures));
        if (resourceFeatures.Any()) groups.Add(CreateSimpleFeatureGroupElement(0.08, 0.18, 1, resourceFeatures));

        AddWildernessGroupedTerrainRoadGroups(groups, terrainByName);

        return
        [
            new AutobuilderAreaTemplateSeedDefinition(
                WildernessGroupedTerrainRandomFeaturesAreaTemplateName,
                WildernessGroupedTerrainRandomFeaturesAreaTemplateType,
                CreateRandomFeaturesAreaDefinition(
                    "Assigns a primary and secondary descriptive element to every room, plus optional sound, smell, resource, and road-topology tags.",
                    groups
                )
            )
        ];
    }

    private static IEnumerable<XElement> BuildWildernessGroupedTerrainDescriptionElements(IReadOnlyCollection<Terrain> terrains)
    {
        IReadOnlyDictionary<string, Terrain> terrainByName = terrains
            .ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
        IReadOnlyDictionary<string, WildernessGroupedTerrainDomainSpec> domains = BuildWildernessGroupedTerrainDomainSpecs();
        IReadOnlyDictionary<string, string[]> domainTerrains = BuildWildernessGroupedTerrainDomainTerrainNames();
        IReadOnlyDictionary<string, WildernessGroupedTerrainFeatureSpec> features = BuildWildernessGroupedTerrainFeatureSpecs();

        Dictionary<string, HashSet<string>> primaryTerrainsByFeature = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> secondaryTerrainsByFeature = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> soundTerrainsByFeature = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, HashSet<string>> smellTerrainsByFeature = new(StringComparer.OrdinalIgnoreCase);

        foreach ((string domainKey, WildernessGroupedTerrainDomainSpec spec) in domains)
        {
            if (!domainTerrains.TryGetValue(domainKey, out string[]? terrainNames))
            {
                continue;
            }

            AddTerrainNamesToFeatureLookup(primaryTerrainsByFeature, spec.PrimaryFeatures, terrainNames, terrainByName);
            AddTerrainNamesToFeatureLookup(secondaryTerrainsByFeature, spec.SecondaryFeatures, terrainNames, terrainByName);
            AddTerrainNamesToFeatureLookup(soundTerrainsByFeature, spec.SoundFeatures, terrainNames, terrainByName);
            AddTerrainNamesToFeatureLookup(smellTerrainsByFeature, spec.SmellFeatures, terrainNames, terrainByName);
        }

        foreach ((string featureName, HashSet<string> terrainNames) in primaryTerrainsByFeature.OrderBy(x => x.Key))
        {
            if (!features.TryGetValue(featureName, out WildernessGroupedTerrainFeatureSpec? feature) || feature.IsResource)
            {
                continue;
            }

            XElement? group = CreateWildernessGroupedTerrainDescriptionGroup(
                terrainByName,
                terrainNames,
                feature,
                new[] { WildernessGroupedTerrainPrimaryPhysicalLayerTag, featureName },
                mandatoryIfValid: true,
                mandatoryPosition: 1);
            if (group is not null)
            {
                yield return group;
            }
        }

        foreach ((string featureName, HashSet<string> terrainNames) in secondaryTerrainsByFeature.OrderBy(x => x.Key))
        {
            if (!features.TryGetValue(featureName, out WildernessGroupedTerrainFeatureSpec? feature) || feature.IsResource)
            {
                continue;
            }

            XElement? group = CreateWildernessGroupedTerrainDescriptionGroup(
                terrainByName,
                terrainNames,
                feature,
                new[] { WildernessGroupedTerrainSecondaryPhysicalLayerTag, featureName },
                mandatoryIfValid: true,
                mandatoryPosition: 2);
            if (group is not null)
            {
                yield return group;
            }
        }

        foreach ((string featureName, HashSet<string> terrainNames) in soundTerrainsByFeature.OrderBy(x => x.Key))
        {
            if (!features.TryGetValue(featureName, out WildernessGroupedTerrainFeatureSpec? feature) || feature.IsResource)
            {
                continue;
            }

            XElement? group = CreateWildernessGroupedTerrainDescriptionGroup(
                terrainByName,
                terrainNames,
                feature,
                new[] { featureName });
            if (group is not null)
            {
                yield return group;
            }
        }

        foreach ((string featureName, HashSet<string> terrainNames) in smellTerrainsByFeature.OrderBy(x => x.Key))
        {
            if (!features.TryGetValue(featureName, out WildernessGroupedTerrainFeatureSpec? feature) || feature.IsResource)
            {
                continue;
            }

            XElement? group = CreateWildernessGroupedTerrainDescriptionGroup(
                terrainByName,
                terrainNames,
                feature,
                new[] { featureName });
            if (group is not null)
            {
                yield return group;
            }
        }

        foreach (XElement roadGroup in BuildWildernessGroupedTerrainRoadDescriptionGroups(terrainByName))
        {
            yield return roadGroup;
        }
    }

    private static void AddTerrainNamesToFeatureLookup(
        IDictionary<string, HashSet<string>> terrainsByFeature,
        IEnumerable<string> featureNames,
        IEnumerable<string> terrainNames,
        IReadOnlyDictionary<string, Terrain> terrainByName)
    {
        foreach (string featureName in featureNames)
        {
            if (!terrainsByFeature.TryGetValue(featureName, out HashSet<string>? terrainSet))
            {
                terrainSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                terrainsByFeature[featureName] = terrainSet;
            }

            foreach (string terrainName in terrainNames)
            {
                if (terrainByName.ContainsKey(terrainName))
                {
                    terrainSet.Add(terrainName);
                }
            }
        }
    }

    private static XElement? CreateWildernessGroupedTerrainDescriptionGroup(
        IReadOnlyDictionary<string, Terrain> terrainByName,
        IEnumerable<string> terrainNames,
        WildernessGroupedTerrainFeatureSpec feature,
        IEnumerable<string> tags,
        bool mandatoryIfValid = false,
        int mandatoryPosition = 100000)
    {
        List<Terrain> featureTerrains = terrainNames
            .Where(x => terrainByName.ContainsKey(x))
            .Select(x => terrainByName[x])
            .ToList();
        if (!featureTerrains.Any())
        {
            return null;
        }

        return CreateRandomDescriptionGroupElement(
            feature.Variants.Select(variant =>
                CreateRandomDescriptionElement(
                    variant,
                    terrains: featureTerrains,
                    tags: tags)),
            weight: feature.GroupWeight,
            mandatoryIfValid: mandatoryIfValid,
            mandatoryPosition: mandatoryPosition);
    }

    private static void AddWildernessGroupedTerrainRoadGroups(
        ICollection<XElement> groups,
        IReadOnlyDictionary<string, Terrain> terrainByName)
    {
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Animal Trail" }, @"Animal Trail", @"Animal Trail Straight", @"Animal Trail Cross", @"Animal Trail Tee", @"Animal Trail Isolated", @"Animal Trail Bend", @"Animal Trail End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Trail" }, @"Trail", @"Trail Straight", @"Trail Cross", @"Trail Tee", @"Trail Isolated", @"Trail Bend", @"Trail End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Dirt Road" }, @"Dirt Road", @"Dirt Road Straight", @"Dirt Road Cross", @"Dirt Road Tee", @"Dirt Road Isolated", @"Dirt Road Bend", @"Dirt Road End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Compacted Dirt Road" }, @"Compacted Dirt Road", @"Compacted Dirt Road Straight", @"Compacted Dirt Road Cross", @"Compacted Dirt Road Tee", @"Compacted Dirt Road Isolated", @"Compacted Dirt Road Bend", @"Compacted Dirt Road End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Gravel Road" }, @"Gravel Road", @"Gravel Road Straight", @"Gravel Road Cross", @"Gravel Road Tee", @"Gravel Road Isolated", @"Gravel Road Bend", @"Gravel Road End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Cobblestone Road" }, @"Cobblestone Road", @"Cobblestone Road Straight", @"Cobblestone Road Cross", @"Cobblestone Road Tee", @"Cobblestone Road Isolated", @"Cobblestone Road Bend", @"Cobblestone Road End");
        AddWildernessGroupedTerrainRoadGroupIfAny(groups, terrainByName, new[] { @"Asphalt Road" }, @"Asphalt Road", @"Asphalt Road Straight", @"Asphalt Road Cross", @"Asphalt Road Tee", @"Asphalt Road Isolated", @"Asphalt Road Bend", @"Asphalt Road End");
    }

    private static void AddWildernessGroupedTerrainRoadGroupIfAny(
        ICollection<XElement> groups,
        IReadOnlyDictionary<string, Terrain> terrainByName,
        IEnumerable<string> terrainNames,
        string baseFeature,
        string straightRoadFeature,
        string crossRoadsFeature,
        string teeIntersectionFeature,
        string isolatedRoadFeature,
        string bendInTheRoadFeature,
        string endOfTheRoadFeature)
    {
        List<Terrain> terrains = GetTerrainsByNames(terrainByName, terrainNames);
        if (!terrains.Any())
        {
            return;
        }

        groups.Add(CreateRoadFeatureGroupElement(
            baseFeature,
            straightRoadFeature,
            crossRoadsFeature,
            teeIntersectionFeature,
            isolatedRoadFeature,
            bendInTheRoadFeature,
            endOfTheRoadFeature,
            terrains));
    }

    private static IEnumerable<XElement> BuildWildernessGroupedTerrainRoadDescriptionGroups(
        IReadOnlyDictionary<string, Terrain> terrainByName)
    {
        List<Terrain> terrains;
        terrains = GetTerrainsByNames(terrainByName, new[] { @"Animal Trail" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail runs through here from $thedirections, little more than a narrow run beaten through the surrounding growth.",
                        @"Animal Trail Straight",
                        roomNameText: @"{0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail keeps a clear line from $thedirections, its line clearer to the feet than to the eye.",
                        @"Animal Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail runs through here from $thedirections, and roots have worked up through the line of it and made the footing uneven.",
                        @"Animal Trail Straight",
                        roomNameText: @"{0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Root-Broken Path" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail keeps a clear line from $thedirections, while fresh sign suggests beasts still claim it as readily as any traveller.",
                        @"Animal Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Beast Tracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail bends here, curving away toward $thedirections, the tread curving away with the habits of beasts rather than the geometry of builders.",
                        @"Animal Trail Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail bends here toward $thedirections, and fresh sign suggests beasts still claim it as readily as any traveller.",
                        @"Animal Trail Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Beast Tracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail comes to a tee here, diverging to $thedirections, the branching made by repeated use rather than deliberate engineering.",
                        @"Animal Trail Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail comes to a tee here, diverging to $thedirections, and its margins are brushed flat where bodies drift off the centre of the tread.",
                        @"Animal Trail Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Trampled Verge" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail meets a crossroads here, diverging to $thedirections, the crossing defined by overlapping traffic rather than any worked surface.",
                        @"Animal Trail Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail meets a crossroads here, diverging to $thedirections, and some deliberate marker makes the trail more legible than the surrounding ground would.",
                        @"Animal Trail Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail comes to an end here, as though the line only matters so long as animals keep choosing it.",
                        @"Animal Trail End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail comes to an end here, but roots have worked up through the line of it and made the footing uneven.",
                        @"Animal Trail End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Root-Broken Path" }),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail begins and ends within sight here, as though it is a habit of passage rather than part of a wider route.",
                        @"Animal Trail Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An animal trail is isolated here, and some deliberate marker makes the trail more legible than the surrounding ground would.",
                        @"Animal Trail Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Trail" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"A trail runs through here from $thedirections, a narrow track worn by repeated passage.",
                        @"Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail keeps a clear line from $thedirections, the path plain enough to follow without needing much imagination.",
                        @"Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail runs through here from $thedirections, and roots have lifted parts of the tread into rough ridges.",
                        @"Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Root-Broken Path" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail keeps a clear line from $thedirections, while animal sign crosses and overlaps the marks of travellers.",
                        @"Trail Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Beast Tracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail bends here, curving away toward $thedirections, the route curving with the land instead of resisting it.",
                        @"Trail Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail bends here toward $thedirections, and animal sign crosses and overlaps the marks of travellers.",
                        @"Trail Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Beast Tracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail comes to a tee here, diverging to $thedirections, the split clean enough to send travellers toward $thedirections.",
                        @"Trail Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail comes to a tee here, diverging to $thedirections, and the edges are beaten down where traffic drifts away from the centre.",
                        @"Trail Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Trampled Verge" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail meets a crossroads here, diverging to $thedirections, the intersection making the traffic of more than one direction obvious.",
                        @"Trail Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail meets a crossroads here, diverging to $thedirections, and a post, cairn, or similar marker helps keep the way intelligible.",
                        @"Trail Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail comes to an end here, the route remaining visible only toward $thedirections.",
                        @"Trail End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail comes to an end here, but roots have lifted parts of the tread into rough ridges.",
                        @"Trail End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Root-Broken Path" }),
                    CreateRoadRandomDescriptionElement(
                        @"A trail begins and ends within sight here, as though a usable line survived here without connecting cleanly to anywhere else.",
                        @"Trail Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A trail is isolated here, and a post, cairn, or similar marker helps keep the way intelligible.",
                        @"Trail Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Dirt Road" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road runs through here from $thedirections, its bare surface marked more by use than by maintenance.",
                        @"Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road keeps a clear line from $thedirections, a simple worked line through the surrounding ground.",
                        @"Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road runs through here from $thedirections, and shallow ruts hold dust longer than the surrounding earth.",
                        @"Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Dusty Ruts" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road keeps a clear line from $thedirections, while a ditch along the edge is there to carry runoff away in wet weather.",
                        @"Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road bends here, curving away toward $thedirections, its curve set by old practical needs rather than elegance.",
                        @"Dirt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road bends here toward $thedirections, and a ditch along the edge is there to carry runoff away in wet weather.",
                        @"Dirt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road comes to a tee here, diverging to $thedirections, the meeting of routes obvious in the worn spread of earth.",
                        @"Dirt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road comes to a tee here, diverging to $thedirections, and the verge has been worn down by feet, hooves, and wheels straying wide.",
                        @"Dirt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Trampled Verge" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road meets a crossroads here, diverging to $thedirections, the crossing broadened by repeated traffic from every side.",
                        @"Dirt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road meets a crossroads here, diverging to $thedirections, and a marker at the roadside lends the route a more deliberate character.",
                        @"Dirt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road comes to an end here, the road carrying on only toward $thedirections.",
                        @"Dirt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road comes to an end here, but shallow ruts hold dust longer than the surrounding earth.",
                        @"Dirt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Dusty Ruts" }),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road begins and ends within sight here, as if a short serviceable stretch outlived the route it once belonged to.",
                        @"Dirt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A dirt road is isolated here, and a marker at the roadside lends the route a more deliberate character.",
                        @"Dirt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Compacted Dirt Road" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road runs through here from $thedirections, its packed surface firmer and more intentional than a merely worn track.",
                        @"Compacted Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road keeps a clear line from $thedirections, the road holding a clear engineered line through the area.",
                        @"Compacted Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road runs through here from $thedirections, and wheel traffic has pressed the centre into an even firmer line.",
                        @"Compacted Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Compacted Wheel Marks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road keeps a clear line from $thedirections, while a drainage cut keeps water from softening the road too much.",
                        @"Compacted Dirt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road bends here, curving away toward $thedirections, the curve staying tidy despite the softness of the material.",
                        @"Compacted Dirt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road bends here toward $thedirections, and a drainage cut keeps water from softening the road too much.",
                        @"Compacted Dirt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road comes to a tee here, diverging to $thedirections, the junction reading as planned work rather than incidental drift.",
                        @"Compacted Dirt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road comes to a tee here, diverging to $thedirections, and the shoulders show regular overflow from the centre line of travel.",
                        @"Compacted Dirt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Trampled Verge" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road meets a crossroads here, diverging to $thedirections, the crossing broad and solid under repeated passage.",
                        @"Compacted Dirt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road meets a crossroads here, diverging to $thedirections, and a roadside marker confirms the route as something once worth maintaining.",
                        @"Compacted Dirt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road comes to an end here, the compacted line continuing only toward $thedirections.",
                        @"Compacted Dirt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road comes to an end here, but wheel traffic has pressed the centre into an even firmer line.",
                        @"Compacted Dirt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Compacted Wheel Marks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road begins and ends within sight here, as though a maintained segment survived after the rest was lost.",
                        @"Compacted Dirt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A compacted dirt road is isolated here, and a roadside marker confirms the route as something once worth maintaining.",
                        @"Compacted Dirt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Gravel Road" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road runs through here from $thedirections, its surface a shifting bed of stone loud enough to announce traffic.",
                        @"Gravel Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road keeps a clear line from $thedirections, the laid gravel giving the route a firmer line than bare earth would manage.",
                        @"Gravel Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road runs through here from $thedirections, and looser stone has worked out across the surface and edges.",
                        @"Gravel Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Loose Gravel Scatter" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road keeps a clear line from $thedirections, while a drainage ditch helps keep the gravel from washing away too quickly.",
                        @"Gravel Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road bends here, curving away toward $thedirections, loose stone gathers more thickly through the curve.",
                        @"Gravel Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road bends here toward $thedirections, and a drainage ditch helps keep the gravel from washing away too quickly.",
                        @"Gravel Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road comes to a tee here, diverging to $thedirections, the meeting of routes obvious where gravel fans more widely under use.",
                        @"Gravel Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road comes to a tee here, diverging to $thedirections, and a roadside marker keeps the route legible even at a distance.",
                        @"Gravel Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road meets a crossroads here, diverging to $thedirections, the intersection spread broad with displaced stone.",
                        @"Gravel Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road meets a crossroads here, diverging to $thedirections, and the occasional tree gives a little shelter beside the otherwise open road.",
                        @"Gravel Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Shade Tree" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road comes to an end here, the gravelled way continuing only toward $thedirections.",
                        @"Gravel Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road comes to an end here, but looser stone has worked out across the surface and edges.",
                        @"Gravel Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Loose Gravel Scatter" }),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road begins and ends within sight here, as though a practical strip of metalling survived without much context.",
                        @"Gravel Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A gravel road is isolated here, and the occasional tree gives a little shelter beside the otherwise open road.",
                        @"Gravel Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Shade Tree" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Cobblestone Road" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road runs through here from $thedirections, its set stones still giving it shape even where age shows.",
                        @"Cobblestone Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road keeps a clear line from $thedirections, the worked stone keeping a hard clear line through the area.",
                        @"Cobblestone Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road runs through here from $thedirections, and a kerb or raised edge lends the road a more finished outline.",
                        @"Cobblestone Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Stone Kerb" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road keeps a clear line from $thedirections, while weeds have started prising open the neglected seams between stones.",
                        @"Cobblestone Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Weed Through Cracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road bends here, curving away toward $thedirections, the stonework carrying the bend with deliberate structure.",
                        @"Cobblestone Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road bends here toward $thedirections, and weeds have started prising open the neglected seams between stones.",
                        @"Cobblestone Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Weed Through Cracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road comes to a tee here, diverging to $thedirections, the stone-laid meeting making the split in routes immediately clear.",
                        @"Cobblestone Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road comes to a tee here, diverging to $thedirections, and some of the paving has cracked, shifted, or gone missing outright.",
                        @"Cobblestone Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Broken Paving" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road meets a crossroads here, diverging to $thedirections, the crossroads broad enough for the paving pattern to change underfoot.",
                        @"Cobblestone Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road meets a crossroads here, diverging to $thedirections, and a roadside marker reinforces the impression of deliberate civic work.",
                        @"Cobblestone Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road comes to an end here, the paved line continuing only toward $thedirections.",
                        @"Cobblestone Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road comes to an end here, but a kerb or raised edge lends the road a more finished outline.",
                        @"Cobblestone Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Stone Kerb" }),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road begins and ends within sight here, as though one stubborn piece of old infrastructure refused to disappear.",
                        @"Cobblestone Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"A cobblestone road is isolated here, and a roadside marker reinforces the impression of deliberate civic work.",
                        @"Cobblestone Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

        terrains = GetTerrainsByNames(terrainByName, new[] { @"Asphalt Road" });
        if (terrains.Any())
        {
            yield return CreateRandomDescriptionGroupElement(
                new XElement[]
                {
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road runs through here from $thedirections, its dark surface cutting a comparatively clean line through the landscape.",
                        @"Asphalt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road keeps a clear line from $thedirections, the hard surface making the route plain even from a distance.",
                        @"Asphalt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road runs through here from $thedirections, and weeds have found the neglected seams and begun to prise them apart.",
                        @"Asphalt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Weed Through Cracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road keeps a clear line from $thedirections, while cracks and breaks interrupt the hard surface in several places.",
                        @"Asphalt Road Straight",
                        roomNameText: @"$dashdirections {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Broken Paving" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road bends here, curving away toward $thedirections, the curve carried on a surface clearly meant for reliable travel.",
                        @"Asphalt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road bends here toward $thedirections, and cracks and breaks interrupt the hard surface in several places.",
                        @"Asphalt Road Bend",
                        roomNameText: @"{0} bend",
                        terrains: terrains,
                        additionalTags: new[] { @"Broken Paving" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road comes to a tee here, diverging to $thedirections, the junction feels engineered first and worn second.",
                        @"Asphalt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road comes to a tee here, diverging to $thedirections, and a drainage cut runs beside the road to spare the surface standing water.",
                        @"Asphalt Road Tee",
                        roomNameText: @"{0} tee",
                        terrains: terrains,
                        additionalTags: new[] { @"Drainage Ditch" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road meets a crossroads here, diverging to $thedirections, the crossroads opens the route in a way that feels planned rather than accidental.",
                        @"Asphalt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road meets a crossroads here, diverging to $thedirections, and a roadside marker gives the route a maintained and legible look.",
                        @"Asphalt Road Cross",
                        roomNameText: @"{0} crossroads",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road comes to an end here, the blacktop continuing only toward $thedirections.",
                        @"Asphalt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road comes to an end here, but weeds have found the neglected seams and begun to prise them apart.",
                        @"Asphalt Road End",
                        roomNameText: @"{0} dead-end",
                        terrains: terrains,
                        additionalTags: new[] { @"Weed Through Cracks" }),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road begins and ends within sight here, as though a remnant of more ambitious connection still lingers here.",
                        @"Asphalt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: Array.Empty<string>()),
                    CreateRoadRandomDescriptionElement(
                        @"An asphalt road is isolated here, and a roadside marker gives the route a maintained and legible look.",
                        @"Asphalt Road Isolated",
                        roomNameText: @"Isolated {0}",
                        terrains: terrains,
                        additionalTags: new[] { @"Roadside Marker" })
                },
                weight: 165.0);
        }

    }

    private static List<Terrain> GetTerrainsByNames(
        IReadOnlyDictionary<string, Terrain> terrainByName,
        IEnumerable<string> terrainNames)
    {
        return terrainNames
            .Where(terrainByName.ContainsKey)
            .Select(x => terrainByName[x])
            .ToList();
    }

    private static IReadOnlyDictionary<string, WildernessGroupedTerrainDomainSpec> BuildWildernessGroupedTerrainDomainSpecs()
    {
        return new Dictionary<string, WildernessGroupedTerrainDomainSpec>(StringComparer.OrdinalIgnoreCase)
        {
            [@"UrbanDomestic"] = new WildernessGroupedTerrainDomainSpec(
                @"The space feels shaped by repeated ordinary use rather than by display. environment{night=Shadow settles softly into the corners and under the furnishings.}{morning=Early light picks out the places most often used first.}{winter=The enclosed air feels a little closer and stiller.}{rain=Any bad weather reaches here only as a muted patter beyond walls or roof.}{Everything about it suggests private routines repeated over a long time.}",
                new[] { @"Worn Furnishings", @"Orderly Domestic Arrangement", @"Soft Household Clutter", @"Personal Touches", @"Muted Textiles" },
                new[] { @"Recent Cleaning", @"Window Light", @"Lingering Cooking Smell", @"Damp Corners" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanCirculation"] = new WildernessGroupedTerrainDomainSpec(
                @"This space is more for passing through than lingering in. environment{night=At night the passage feels longer and emptier than it does by day.}{morning=Morning light and movement lend the route a brisk practicality.}{rain=Sound from the weather carries in and along the surfaces.}{winter=Cooler air tends to gather in the more open stretches.}{Wear and proportion make the intended flow of traffic immediately obvious.}",
                new[] { @"Long Corridor Sightline", @"Echoing Floorboards", @"Wall Niches", @"Drafty Passage", @"Foot Traffic Wear" },
                new[] { @"Window Light", @"Damp Corners", @"Close Ceiling", @"Recent Cleaning" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanCommunal"] = new WildernessGroupedTerrainDomainSpec(
                @"The place feels built for shared use, with signs of repeated occupation by many different people. environment{night=In quieter hours the larger shapes and scuffed surfaces stand out more clearly.}{morning=Fresh activity makes the space feel ready to fill again.}{rain=Weather outside becomes a distant accompaniment rather than the main event.}{winter=The enclosed air holds the trace of many bodies and long use.}{Its scale and arrangement favour function over intimacy.}",
                new[] { @"Communal Benches", @"Training Scuffs", @"Foot Traffic Wear", @"Echoing Floorboards", @"Wall Niches" },
                new[] { @"Drafty Passage", @"Recent Cleaning", @"Window Light", @"Muted Textiles" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanCommercial"] = new WildernessGroupedTerrainDomainSpec(
                @"This is a space arranged to catch the eye, receive visitors, or move goods and coin. environment{morning=Morning light makes the public-facing parts of the place seem almost expectant.}{night=After dark, the bones of the layout become more obvious than the bustle it was made for.}{rain=Bad weather outside only sharpens the sense of shelter and exchange here.}{autumn=Tracked dirt and the marks of trade show more clearly in the cooler season.}{The whole arrangement suggests regular traffic and practiced routine.}",
                new[] { @"Shuttered Front", @"Stall Layout", @"Storage Racks", @"Steady Traffic", @"Mixed Frontages" },
                new[] { @"Iron Fittings", @"Window Light", @"Foot Traffic Wear", @"Market Stalls" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanWork"] = new WildernessGroupedTerrainDomainSpec(
                @"The space is organised around labour, storage, or the management of practical tasks. environment{morning=Early light gives everything the feel of work about to begin in earnest.}{night=At night the tools and harder edges of the place feel more pronounced.}{rain=Weather beyond the walls becomes a dull accompaniment to the place's harder purpose.}{winter=Cold seems to cling more readily to the sterner materials here.}{Even at rest, it carries a sense of use rather than ornament.}",
                new[] { @"Stacked Goods", @"Tool Marks", @"Paperwork Tidy", @"Loading Space", @"Machine Residue" },
                new[] { @"Iron Fittings", @"Foot Traffic Wear", @"Smoke Stain", @"Stale Air" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanConfinement"] = new WildernessGroupedTerrainDomainSpec(
                @"Confinement and control shape this place more than comfort ever did. environment{night=Darkness thickens quickly here and seems reluctant to leave the edges alone.}{morning=Whatever light arrives does little to soften the harsher details.}{rain=Moisture and distant echoes make the enclosure feel even meaner.}{winter=The air has a close, unfriendly chill to it.}{The structure presses in more than it opens out.}",
                new[] { @"Iron Fittings", @"Chained Fixtures", @"Stale Air", @"Dripping Masonry", @"Close Ceiling" },
                new[] { @"Damp Corners", @"Foot Traffic Wear", @"Echoing Floorboards", @"Smoke Stain" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanWater"] = new WildernessGroupedTerrainDomainSpec(
                @"Warmth, dampness, or still enclosed water gives this place a softened, echoing atmosphere. environment{morning=Early light catches on moisture and faint steam where it can.}{night=In the dimmer hours the wet shine of surfaces becomes more apparent.}{rain=External rain only adds to the sense of enclosed damp.}{winter=Heat and humidity stand out more sharply against the colder season.}{The space feels set apart from the drier world beyond it.}",
                new[] { @"Steam Haze", @"Mineral Warmth", @"Still Water", @"Damp Corners", @"Dripping Masonry" },
                new[] { @"Window Light", @"Close Ceiling", @"Recent Cleaning", @"Foot Traffic Wear" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanRoofDefense"] = new WildernessGroupedTerrainDomainSpec(
                @"Exposure and elevation make this place feel alert even when it stands empty. environment{dawn=First light sharpens edges and makes every line of sight matter.}{night=At night the drop and the open sky feel especially pronounced.}{rain=Rain leaves stone and timber darkened, slick, and plainly weathered.}{winter=Cold wind finds the exposed parts of the structure without difficulty.}{Height and hard construction dominate the impression it leaves.}",
                new[] { @"Exposed Roofline", @"Defensive Height", @"Gate Traffic", @"Clean Stonework", @"Drafty Passage" },
                new[] { @"Iron Fittings", @"Smoke Stain", @"Wall Niches", @"Steady Traffic" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanStreetPoor"] = new WildernessGroupedTerrainDomainSpec(
                @"Hard use and limited means show plainly in the way this street or lane has worn over time. environment{morning=Daylight makes every patch, crack, and improvised detail easier to pick out.}{night=After dark the poorer edges of the street feel closer and rougher.}{rain=Rain gathers quickly in the imperfect drainage and worn surfaces.}{winter=The exposed street seems to offer little comfort against the season.}{The whole place feels adapted rather than carefully finished.}",
                new[] { @"Narrow Frontages", @"Laundry Lines", @"Broken Paving", @"Refuse Scatter", @"Smoke Stain" },
                new[] { @"Encroaching Weeds", @"Drainage Channel", @"Steady Traffic", @"Mixed Frontages" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanStreetCommon"] = new WildernessGroupedTerrainDomainSpec(
                @"Regular use has worn this street into something practical, familiar, and unpretentious. environment{morning=Morning light catches the details of habitual traffic and maintenance.}{night=In the dark, the street simplifies into lines of route and frontage.}{rain=Rain darkens the travelled surfaces and finds every shallow dip.}{autumn=Fallen leaves and tracked dirt make the ordinary wear more noticeable.}{It feels made for steady passage rather than spectacle.}",
                new[] { @"Drainage Channel", @"Steady Traffic", @"Mixed Frontages", @"Street Trees", @"Broken Paving" },
                new[] { @"Laundry Lines", @"Narrow Frontages", @"Clean Stonework", @"Foot Traffic Wear" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanStreetWealthyRural"] = new WildernessGroupedTerrainDomainSpec(
                @"This street carries a more deliberate sense of care and boundary than most. environment{morning=Morning light makes the tidier edges and kept surfaces stand out.}{night=After dark the quiet order of the place becomes even more noticeable.}{rain=Rain beads or runs cleanly from better-kept surfaces.}{spring=Fresh growth around the edges lends the whole street an easier grace.}{Attention and oversight are evident in the way the place holds together.}",
                new[] { @"Clean Stonework", @"Quiet Verge", @"Formal Planting", @"Broad Pavement", @"Street Trees" },
                new[] { @"Open Garden Edge", @"Fence Lines", @"Trim Lawn", @"Flower Beds" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanCivicOpen"] = new WildernessGroupedTerrainDomainSpec(
                @"The space is meant to be seen and shared, with room for gathering, display, or public pause. environment{morning=Morning light gives the open space a fresh, almost ceremonial clarity.}{night=At night the emptier reaches feel broader and more formal.}{rain=Rain leaves the open ground and paving darkened but still legible in layout.}{spring=Fresh growth and cleaner light soften the harder civic edges.}{Its openness feels intentional rather than accidental.}",
                new[] { @"Open Garden Edge", @"Public Monuments", @"Trim Lawn", @"Flower Beds", @"Broad Pavement" },
                new[] { @"Seating Cluster", @"Market Stalls", @"Clean Stonework", @"Formal Planting" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"UrbanRefuse"] = new WildernessGroupedTerrainDomainSpec(
                @"Neglect, discard, or the aftermath of hard use dominates this place at first glance. environment{day=In fuller light the layered waste and staining are impossible to ignore.}{night=At night the smell and suggestion of the place lead the eye.}{rain=Rain turns the worst of it slick, dark, and newly active.}{autumn=Rot and old debris seem more obvious in the colder, damper season.}{The ground looks burdened by what has been thrown aside.}",
                new[] { @"Rotting Heap", @"Carrion Interest", @"Smoke Stain", @"Refuse Scatter", @"Hard-Worn Ground" },
                new[] { @"Encroaching Weeds", @"Drainage Channel", @"Broken Paving", @"Mud Churn" },
                Array.Empty<string>(),
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"RoadTrail"] = new WildernessGroupedTerrainDomainSpec(
                @"The way through here is defined more by repeated passage than by any formal construction. environment{dawn=Low light picks out the line of travel with particular clarity.}{night=After dark the route feels narrower and more dependent on instinct or memory.}{rain=Rain softens the edges of the path and deepens every mark.}{autumn=Fallen leaves and dry litter shift to reveal the most-used line.}{Passage has shaped this place without ever fully mastering it.}",
                new[] { @"Narrow Tread", @"Root-Broken Path", @"Beast Tracks", @"Trampled Verge", @"Roadside Marker" },
                new[] { @"Scattered Stones", @"Dry Patches", @"Low Shrub", @"Bare Earth Patches" },
                new[] { @"Field Birds", @"Wind Through Grass", @"Animal Rustle" },
                new[] { @"Crushed Grass Scent", @"Dry Earth Smell", @"Animal Musk" },
                new[] { @"Herb Patch" }),
            [@"RoadUnpaved"] = new WildernessGroupedTerrainDomainSpec(
                @"Travel has imposed a clearer course on the ground here, but not a refined one. environment{dawn=Early light catches every rut, print, and worn edge.}{night=At night the road is more a band of texture than a clean line.}{rain=Rain works quickly into the surface, darkening low points and wheel-worn stretches.}{dry=Under dry conditions the whole route turns pale, loose, and dusty.}{The road feels serviceable rather than graceful.}",
                new[] { @"Dusty Ruts", @"Compacted Wheel Marks", @"Drainage Ditch", @"Trampled Verge", @"Roadside Marker" },
                new[] { @"Scattered Stones", @"Hard-Baked Soil", @"Bare Earth Patches", @"Shade Tree" },
                new[] { @"Field Birds", @"Wind Through Grass" },
                new[] { @"Dry Earth Smell", @"Crushed Grass Scent" },
                new[] { @"Clay Deposit", @"Sand Deposit" }),
            [@"RoadPaved"] = new WildernessGroupedTerrainDomainSpec(
                @"The route here shows deliberate work in its line, grading, and surface. environment{dawn=Early light throws the set edges and surface texture into relief.}{night=In darkness the road reads as a firmer, more trustworthy band than the ground beside it.}{rain=Rain slicks the harder surface and sends water searching for its drainage.}{winter=Cold only sharpens the stony or tarred character of the route.}{It is plainly meant to carry traffic with more certainty than the land around it.}",
                new[] { @"Loose Gravel Scatter", @"Stone Kerb", @"Even Camber", @"Weed Through Cracks", @"Roadside Marker" },
                new[] { @"Drainage Ditch", @"Mixed Frontages", @"Steady Traffic", @"Broken Paving" },
                new[] { @"Field Birds", @"Wind Through Grass" },
                new[] { @"Dry Earth Smell" },
                new[] { @"Sand Deposit" }),
            [@"OpenGrass"] = new WildernessGroupedTerrainDomainSpec(
                @"Broad sky and open ground leave this place exposed to weather, light, and distance. environment{dawn=Early light turns dew and seedheads into pale flashes across the ground.}{night=At night the land flattens into bands of shadow and faint movement.}{spring=Fresh growth and scattered blooms brighten the open space.}{autumn=Dry stalks and tired colour give the land an older look.}{The terrain is simple in form but never empty of detail.}",
                new[] { @"Tall Grass", @"Short Grass", @"Wildflowers", @"Seedhead Sweep", @"Wind-Pressed Grass" },
                new[] { @"Scattered Stones", @"Animal Run", @"Dry Patches", @"Shallow Swale" },
                new[] { @"Wind Through Grass", @"Field Birds", @"Grasshopper Chirr", @"Animal Rustle" },
                new[] { @"Crushed Grass Scent", @"Wildflower Sweetness", @"Dry Earth Smell", @"Animal Musk" },
                new[] { @"Diggable Soil Deposit", @"Herb Patch" }),
            [@"Savannah"] = new WildernessGroupedTerrainDomainSpec(
                @"Open country stretches here beneath a wide sky, broken only here and there by tougher growth. environment{dawn=Low light catches the taller grasses and isolated trees first.}{night=In the dark, the scattered silhouettes stand out against the openness.}{dry=Everything seems dustier, paler, and more brittle in the dry conditions.}{rain=Rain sharpens the scent of earth and living growth across the plain.}{The balance between open ground and scattered cover defines the place.}",
                new[] { @"Tall Grass", @"Scattered Trees", @"Shade Tree", @"Dry Seed Pods", @"Insect Hum" },
                new[] { @"Animal Run", @"Bare Earth Patches", @"Hard-Baked Soil", @"Wind-Pressed Grass" },
                new[] { @"Wind Through Grass", @"Field Birds", @"Grasshopper Chirr", @"Animal Rustle" },
                new[] { @"Dry Earth Smell", @"Animal Musk", @"Crushed Grass Scent" },
                new[] { @"Diggable Soil Deposit", @"Herb Patch" }),
            [@"Shrubland"] = new WildernessGroupedTerrainDomainSpec(
                @"Low growth and rougher, half-open ground give the land a snagged and irregular feel. environment{day=Clearer light picks out the knots of scrub and the bare earth between them.}{night=At night the darker thickets gather into heavier shapes.}{rain=Rain darkens the brush and leaves trapped moisture clinging beneath it.}{autumn=Dry seed and browning growth make the tougher plants look even harder.}{The terrain resists clean lines and easy movement.}",
                new[] { @"Low Shrub", @"Thorn Scrub", @"Resinous Brush", @"Tangle of Thorns", @"Bare Earth Patches" },
                new[] { @"Dry Seed Pods", @"Scattered Stones", @"Insect Hum", @"Hard-Baked Soil" },
                new[] { @"Field Birds", @"Grasshopper Chirr", @"Animal Rustle" },
                new[] { @"Dry Earth Smell", @"Wildflower Sweetness", @"Animal Musk" },
                new[] { @"Diggable Soil Deposit", @"Herb Patch" }),
            [@"Tundra"] = new WildernessGroupedTerrainDomainSpec(
                @"The land here is spare, wind-touched, and only lightly claimed by growth. environment{day=Under fuller light, every patch of low growth and stone seems starkly separate.}{night=At night the open cold and low horizon feel even more exposed.}{winter=The season deepens the hard stillness already present in the ground.}{spring=The first signs of thaw and new growth remain small against the wider barrenness.}{Nothing about the place feels sheltered or abundant.}",
                new[] { @"Lichen Mat", @"Frost-Hardened Ground", @"Sparse Sedge", @"Wind-Pressed Grass", @"Scattered Stones" },
                new[] { @"Bare Earth Patches", @"Shallow Swale", @"Animal Run", @"Frost Crystals" },
                new[] { @"Wind Through Grass", @"Wind Keening" },
                new[] { @"Clean Cold Scent", @"Dry Earth Smell" },
                new[] { @"Diggable Soil Deposit" }),
            [@"Floodplain"] = new WildernessGroupedTerrainDomainSpec(
                @"Water has shaped this ground as much as grass or soil ever could. environment{day=Light makes the silted levels and old deposits easier to read.}{night=At night the darker swales and damp ground merge into broader patches of shadow.}{rain=Fresh rain quickly renews the impression that water rules the place.}{spring=New growth rises quickest where the ground still holds last water.}{The land feels fertile, low, and vulnerable to change.}",
                new[] { @"Water-Laid Silt", @"Reed Fringe", @"Debris Snag", @"Shallow Swale", @"Tall Grass" },
                new[] { @"Mud Churn", @"Insect Hum", @"Bare Earth Patches", @"Short Grass" },
                new[] { @"Field Birds", @"Reed Rustle", @"Waterfowl Calls", @"Grasshopper Chirr" },
                new[] { @"Wet Earth Scent", @"Crushed Grass Scent", @"Animal Musk" },
                new[] { @"Clay Deposit", @"Reed Harvest", @"Diggable Soil Deposit" }),
            [@"BadlandsSalt"] = new WildernessGroupedTerrainDomainSpec(
                @"Erosion and mineral residue have stripped the ground here down to harsher essentials. environment{day=Strong light throws every cut, crust, and pale scar into view.}{night=In darkness the broken ground feels more treacherous than distant.}{rain=Rain briefly darkens the harder earth and wakes old channels to life.}{dry=In the dry, the land looks bleached and stubbornly lifeless.}{The terrain feels exhausted rather than empty.}",
                new[] { @"Salt Crust", @"Eroded Gullies", @"Cracked Earth", @"Chalky Dust", @"Exposed Bedrock" },
                new[] { @"Bleached Stone", @"Wind-Carved Stone", @"Hard-Baked Soil", @"Scattered Stones" },
                new[] { @"Desert Wind", @"Heat Silence" },
                new[] { @"Hot Dust Smell" },
                new[] { @"Salt Deposit", @"Clay Deposit", @"Stone Deposit" }),
            [@"RollingUpland"] = new WildernessGroupedTerrainDomainSpec(
                @"Low rises and hollows give this country more shape than shelter. environment{day=Light moves cleanly across the upland folds and exposed patches.}{night=At night the higher ground stands out only as darker swells against the horizon.}{spring=Fresh growth softens the harsher stone and open patches.}{autumn=The browsed and weathered ground looks more worn as colour fades from it.}{The land offers outlook more readily than concealment.}",
                new[] { @"Rolling Rise", @"Sheltered Hollow", @"Boulder Scatter", @"Exposed Bedrock", @"Animal Run" },
                new[] { @"Tall Grass", @"Short Grass", @"Wind-Pressed Grass", @"Scattered Stones" },
                new[] { @"Field Birds", @"Wind Through Grass", @"Animal Rustle" },
                new[] { @"Dry Earth Smell", @"Crushed Grass Scent", @"Wildflower Sweetness" },
                new[] { @"Diggable Soil Deposit", @"Stone Deposit" }),
            [@"Tableland"] = new WildernessGroupedTerrainDomainSpec(
                @"Higher ground and broader rock shapes make the terrain feel deliberate and enduring. environment{day=In clear light the layered forms and hard edges are easy to read.}{night=At night the upland masses feel heavier than the sky above them.}{rain=Rain darkens the rock and briefly sharpens the planes and seams.}{dry=Dry weather leaves the surfaces pale, hard, and severe.}{Stone and exposure dominate the place more than growth does.}",
                new[] { @"High Rim", @"Layered Rock", @"Wind-Carved Stone", @"Exposed Bedrock", @"Boulder Scatter" },
                new[] { @"Sheltered Hollow", @"Chalky Dust", @"Bleached Stone", @"Hard-Baked Soil" },
                new[] { @"Field Birds", @"Wind Through Grass", @"Animal Rustle" },
                new[] { @"Dry Earth Smell", @"Crushed Grass Scent" },
                new[] { @"Stone Deposit", @"Ore Vein" }),
            [@"Cutland"] = new WildernessGroupedTerrainDomainSpec(
                @"The land has been cut, channelled, or folded into deeper shapes here. environment{day=Light reaches unevenly, leaving the deeper forms plain to read.}{night=At night the confined shapes feel closer and the open sky more distant.}{rain=Rain lends the cut ground a freshly worked look and deepens every shade.}{autumn=Loose debris and thinning growth make the contours easier to follow.}{The terrain guides movement as much by restriction as by invitation.}",
                new[] { @"River Cut", @"Echoing Walls", @"Narrow Defile", @"Layered Rock", @"Sheltered Hollow" },
                new[] { @"Boulder Scatter", @"Debris Snag", @"Cliff Shadow", @"Exposed Bedrock" },
                new[] { @"Field Birds", @"Wind Keening" },
                new[] { @"Dry Earth Smell" },
                new[] { @"Stone Deposit", @"Ore Vein" }),
            [@"Dunescape"] = new WildernessGroupedTerrainDomainSpec(
                @"Wind has made the ground here more fluid in shape than solid in line. environment{day=Strong light brings out every ripple and knife-edge of sand.}{night=At night the dunes become larger, quieter masses against the dark.}{dry=Dry conditions leave the sand loose enough to shift with the lightest touch.}{rain=Even brief rain lays the outer grains down and darkens the hollows.}{The land seems always on the verge of remaking itself.}",
                new[] { @"Dune Face", @"Wind Rippled Sand", @"Drift Ridge", @"Salted Breeze", @"Bare Earth Patches" },
                new[] { @"Heat Haze", @"Dry Seed Pods", @"Wind-Pressed Grass", @"Scattered Stones" },
                new[] { @"Desert Wind", @"Heat Silence" },
                new[] { @"Hot Dust Smell" },
                new[] { @"Sand Deposit", @"Salt Deposit" }),
            [@"MountainCliff"] = new WildernessGroupedTerrainDomainSpec(
                @"Steep grade, falling stone, and abrupt changes in level give the terrain an exposed severity. environment{day=Clear light exaggerates the drop, the rubble, and the hard angles of the slope.}{night=At night the unseen depth beyond the edge becomes the defining feature.}{winter=Cold sharpens every hard surface and makes the heights feel even less forgiving.}{rain=Rain turns the stone darker and the footing more doubtful.}{The place feels won from the mountain rather than settled upon it.}",
                new[] { @"Broken Scree", @"Loose Talus", @"Sharp Drop", @"Cliff Shadow", @"Steep Grade" },
                new[] { @"Stone Overhang", @"Avalanche Debris", @"Corniced Edge", @"Narrow Defile" },
                new[] { @"Wind Keening", @"Field Birds" },
                new[] { @"Clean Cold Scent", @"Dry Earth Smell" },
                new[] { @"Stone Deposit", @"Ore Vein" }),
            [@"ForestBroadleaf"] = new WildernessGroupedTerrainDomainSpec(
                @"Leafy growth and softer ground close the place in without fully swallowing the light. environment{spring=Fresh leaf and wet earth make the whole forest feel newly alive.}{autumn=Turning leaves lay warm colour across the branches and ground alike.}{winter=Bare branches and a thinner canopy leave the woods feeling more open and austere.}{rain=Rain works through the leaves in a long, patient drip.}{The forest feels layered rather than empty.}",
                new[] { @"Dense Canopy", @"Open Canopy", @"Mixed Leaf Litter", @"Broadleaf Shade", @"Thick Underbrush" },
                new[] { @"Clear Understory", @"Mossed Trunkfall", @"Sunlit Glade", @"Rain-Heavy Leaves" },
                new[] { @"Bird Chorus", @"Canopy Insects", @"Animal Rustle" },
                new[] { @"Leaf Mold Scent", @"Wet Earth Scent", @"Wildflower Sweetness" },
                new[] { @"Timber Stand", @"Herb Patch" }),
            [@"ForestConifer"] = new WildernessGroupedTerrainDomainSpec(
                @"Needled growth and resinous shade give these woods a darker, more enduring character. environment{spring=Fresh growth softens the harsher greens without changing the deeper shade much.}{autumn=The forest changes less in colour than in the smell of needles and damp earth.}{winter=Snow and cold only deepen the austere calm of the conifers.}{rain=Rain gathers on needles and falls in thin, delayed drops.}{The trees lend the place a disciplined hush.}",
                new[] { @"Dense Canopy", @"Open Canopy", @"Conifer Needles", @"Resin Scent", @"Mossed Trunkfall" },
                new[] { @"Clear Understory", @"Thick Underbrush", @"Sunlit Glade", @"Boggy Roots" },
                new[] { @"Needle Wind", @"Bird Chorus", @"Animal Rustle" },
                new[] { @"Needle Resin Smell", @"Clean Cold Scent", @"Leaf Mold Scent" },
                new[] { @"Timber Stand" }),
            [@"Rainforest"] = new WildernessGroupedTerrainDomainSpec(
                @"Dense vegetation and persistent moisture make the place feel crowded with life at every level. environment{day=Fuller light only reveals how much growth is layered into the space.}{night=At night the unseen life of the forest seems closer than the visible shapes.}{rain=Rain vanishes into the larger wetness of the place rather than changing it outright.}{dry=Even in drier weather, the air holds more moisture than most lands ever do.}{The forest presses in vertically as much as horizontally.}",
                new[] { @"Dense Canopy", @"Rain-Heavy Leaves", @"Hanging Vines", @"Giant Trunks", @"Ferny Floor" },
                new[] { @"Boggy Roots", @"Thick Underbrush", @"Clear Understory", @"Mossed Trunkfall" },
                new[] { @"Canopy Insects", @"Bird Chorus", @"Animal Rustle" },
                new[] { @"Humid Rot Smell", @"Wet Earth Scent", @"Leaf Mold Scent" },
                new[] { @"Timber Stand", @"Herb Patch", @"Fruit Grove" }),
            [@"ManagedWoodland"] = new WildernessGroupedTerrainDomainSpec(
                @"The trees and undergrowth here show signs of being used, tended, or at least long observed by people. environment{spring=New growth and blossom make the managed character of the place look almost deliberate to excess.}{autumn=Fruit, leaf-fall, and worked paths give the woodland a busier feel.}{winter=With less leaf cover, the spacing and order of the trees are easier to see.}{rain=Rain deepens the scent of bark, leaf mould, and worked ground.}{Nature here feels guided rather than untouched.}",
                new[] { @"Ordered Rows", @"Fruiting Trees", @"Managed Copse", @"Sunlit Glade", @"Open Canopy" },
                new[] { @"Mixed Leaf Litter", @"Clear Understory", @"Mossed Trunkfall", @"Broadleaf Shade" },
                new[] { @"Bird Chorus", @"Wind Through Grass", @"Animal Rustle" },
                new[] { @"Leaf Mold Scent", @"Wildflower Sweetness", @"Crushed Grass Scent" },
                new[] { @"Timber Stand", @"Fruit Grove", @"Herb Patch" }),
            [@"WetlandFresh"] = new WildernessGroupedTerrainDomainSpec(
                @"Water lingers here in the soil as much as in the open patches between the plants. environment{day=Light glints from hidden wet places and pooled water alike.}{night=At night the wetter ground reads as darker, softer patches under the reeds and brush.}{rain=Fresh rain quickly becomes indistinguishable from the wetness already held here.}{spring=New growth thickens the wet ground with fresh life.}{The land feels uncertain underfoot even when it looks still.}",
                new[] { @"Standing Water", @"Reed Bed", @"Rush Clumps", @"Mud Churn", @"Waterlogged Timber" },
                new[] { @"Sphagnum Mat", @"Peaty Ground", @"Mosquito Swarm", @"Boggy Roots" },
                new[] { @"Frog Chorus", @"Mosquito Whine", @"Reed Rustle", @"Waterfowl Calls" },
                new[] { @"Wet Earth Scent", @"Peat Reek", @"Humid Rot Smell" },
                new[] { @"Peat Deposit", @"Reed Harvest", @"Clay Deposit" }),
            [@"WetlandSaline"] = new WildernessGroupedTerrainDomainSpec(
                @"Salt water or brackish seep has given the wet ground here a harsher edge than inland marsh usually carries. environment{day=Light catches on slick mud, standing water, and rooted growth alike.}{night=At night the dark water and darker mud blend into one another in treacherous patches.}{rain=Rain freshens the surface but does little to change the deep wetness below it.}{autumn=Dead or drying growth leaves the rooted structure of the wetland more exposed.}{The place feels both fertile and corrosive.}",
                new[] { @"Brackish Slick", @"Tidal Mud", @"Mangrove Roots", @"Reed Bed", @"Waterlogged Timber" },
                new[] { @"Mosquito Swarm", @"Mud Churn", @"Drift Line", @"Debris Snag" },
                new[] { @"Waterfowl Calls", @"Reed Rustle", @"Mosquito Whine", @"Surf Wash" },
                new[] { @"Brackish Rot Smell", @"Salt Spray", @"Wet Earth Scent" },
                new[] { @"Salt Deposit", @"Reed Harvest" }),
            [@"DesertSand"] = new WildernessGroupedTerrainDomainSpec(
                @"Loose sand and open exposure give the land here a stripped, wind-shaped simplicity. environment{day=Harsh light makes every ridge, drift, and bare stretch stand out.}{night=After dark the sand loses detail but not its breadth.}{dry=In dry conditions the surface sits ready to lift and shift with the least disturbance.}{rain=Brief rain firms the outer skin of the ground without changing its nature for long.}{The country feels spare, bright, and difficult to hold.}",
                new[] { @"Dune Face", @"Wind Rippled Sand", @"Drift Ridge", @"Bare Earth Patches", @"Salted Breeze" },
                new[] { @"Heat Haze", @"Scattered Stones", @"Hard-Baked Soil", @"Dry Seed Pods" },
                new[] { @"Desert Wind", @"Heat Silence" },
                new[] { @"Hot Dust Smell" },
                new[] { @"Sand Deposit", @"Salt Deposit" }),
            [@"DesertRock"] = new WildernessGroupedTerrainDomainSpec(
                @"Stone and hard-packed ground dominate here, with little to soften heat or emptiness. environment{day=Strong light sharpens every fracture, plate, and bleached surface.}{night=At night the rock keeps its mass even after the detail drains away.}{dry=Dry weather leaves the ground looking almost flayed down to its harder bones.}{rain=Rain briefly darkens the stone and pulls a rougher scent from dust and heat.}{The terrain feels durable and unforgiving in equal measure.}",
                new[] { @"Bleached Stone", @"Heat-Shattered Rock", @"Desert Pavement", @"Hard-Baked Soil", @"Chalky Dust" },
                new[] { @"Salted Breeze", @"Exposed Bedrock", @"Cracked Earth", @"Wind-Carved Stone" },
                new[] { @"Desert Wind", @"Heat Silence" },
                new[] { @"Hot Dust Smell" },
                new[] { @"Stone Deposit", @"Ore Vein", @"Salt Deposit" }),
            [@"Oasis"] = new WildernessGroupedTerrainDomainSpec(
                @"Shelter, water, and living growth gather here in sharp contrast to the drier land around it. environment{day=The sight of shade and water feels more striking in clearer light.}{night=At night the darker mass of growth marks the water before anything else does.}{dry=Dry conditions beyond the oasis only strengthen the sense of relief here.}{rain=Rain turns the place lush rather than merely wet.}{The meeting of scarcity and abundance defines the place.}",
                new[] { @"Palm Shade", @"Spring Water", @"Green Fringe", @"Reed Fringe", @"Insect Hum" },
                new[] { @"Bare Earth Patches", @"Tall Grass", @"Mud Churn", @"Water-Laid Silt" },
                new[] { @"Water Murmur", @"Field Birds", @"Frog Chorus", @"Grasshopper Chirr" },
                new[] { @"Wet Earth Scent", @"Wildflower Sweetness", @"Crushed Grass Scent" },
                new[] { @"Freshwater Spring", @"Fruit Grove", @"Reed Harvest" }),
            [@"Volcanic"] = new WildernessGroupedTerrainDomainSpec(
                @"Fire-shaped stone gives the ground here a harsher history than most landscapes wear openly. environment{day=Light catches sharp edges, black glass, and ash-dulled surfaces in equal measure.}{night=At night the darker rock seems to drink what little light there is.}{rain=Rain makes the scorched ground smell raw and mineral.}{winter=Cold weather throws the old violence of the stone into sharper relief.}{The terrain feels wounded, cooling, or both.}",
                new[] { @"Ash Dust", @"Cooling Basalt", @"Broken Obsidian", @"Sulphur Reek", @"Blackened Crack" },
                new[] { @"Heat Haze", @"Cindered Hollow", @"Fumarole Stain", @"Exposed Bedrock" },
                new[] { @"Vent Hiss", @"Desert Wind" },
                new[] { @"Sulphur Tang", @"Hot Dust Smell" },
                new[] { @"Obsidian Deposit", @"Sulphur Deposit", @"Ore Vein" }),
            [@"Glacial"] = new WildernessGroupedTerrainDomainSpec(
                @"Ice and snow hold the land here in harder, cleaner lines than ordinary ground permits. environment{day=Clear light turns every ridge, crust, and blue shadow painfully distinct.}{night=At night the pale ground keeps its presence long after detail is lost.}{winter=The season deepens the frozen authority the place already carries.}{spring=Meltwater and softened edges hint at change without removing the ice's hold.}{Cold and brightness govern the whole impression.}",
                new[] { @"Wind-Hardened Snow", @"Fresh Drift", @"Blue Ice", @"Crevasse Hint", @"Frost Crystals" },
                new[] { @"Old Meltwater", @"Sastrugi Ridges", @"Snow Blind Glare", @"Scattered Stones" },
                new[] { @"Ice Creak", @"Wind Keening" },
                new[] { @"Clean Cold Scent" },
                new[] { @"Ice Block", @"Freshwater Spring" }),
            [@"CaveDry"] = new WildernessGroupedTerrainDomainSpec(
                @"Stone has closed over this place enough to change light, sound, and distance alike. environment{day=What little outside light reaches in only serves to measure the deeper dark.}{night=At night the cave feels scarcely touched by the world beyond it.}{rain=Moisture awakens small echoes and dark patches across the stone.}{winter=Season matters less here, though cold still finds its way into the nearer reaches.}{The cave keeps its own scale and its own silence.}",
                new[] { @"Mineral Stain", @"Jagged Stalactites", @"Echoing Chamber", @"Powdery Dust", @"Slick Stone" },
                new[] { @"Narrow Throat", @"Darkness Pocket", @"Stone Overhang", @"Dripping Water" },
                new[] { @"Hollow Quiet", @"Bat Flutter" },
                new[] { @"Mineral Damp" },
                new[] { @"Stone Deposit", @"Ore Vein", @"Obsidian Deposit" }),
            [@"CaveWater"] = new WildernessGroupedTerrainDomainSpec(
                @"Water is an active presence here, shaping the cave as much as the stone itself. environment{day=Such light as reaches this place glints from wet surfaces and still water.}{night=At night the sound of water becomes more prominent than the shapes around it.}{rain=Rain beyond the cave seems to answer back through drip, run, and echo here.}{spring=Seasonal thaw or runoff lends the hidden water more life and movement.}{The space feels subterranean, wet, and alive with quiet motion.}",
                new[] { @"Dripping Water", @"Still Pool", @"Underground Current", @"Mineral Stain", @"Slick Stone" },
                new[] { @"Echoing Chamber", @"Jagged Stalactites", @"Narrow Throat", @"Darkness Pocket" },
                new[] { @"Cave Drip Echo", @"Hollow Quiet", @"Bat Flutter" },
                new[] { @"Mineral Damp", @"Wet Earth Scent" },
                new[] { @"Freshwater Spring", @"Clay Deposit", @"Stone Deposit" }),
            [@"Shoreline"] = new WildernessGroupedTerrainDomainSpec(
                @"Land and water meet here in a restless boundary that never keeps a single clean line for long. environment{day=Clear light picks out every wet mark, deposit, and change in the edge.}{night=At night the meeting of land and water becomes more a matter of sound and sheen than shape.}{rain=Rain blurs the smaller traces but deepens the whole sense of saturation.}{autumn=Wind and drift make the margin look newly rearranged.}{This place is defined by transition more than permanence.}",
                new[] { @"Foam Line", @"Drift Line", @"Pebble Wash", @"Sand Ripple", @"Mud Slick" },
                new[] { @"Reed Margin", @"Spray Marks", @"Gentle Lapping", @"Kelp Wrack" },
                new[] { @"Surf Wash", @"Shorebird Calls", @"Waterfowl Calls" },
                new[] { @"Salt Spray", @"Kelp Tang" },
                new[] { @"Sand Deposit", @"Clay Deposit", @"Reed Harvest" }),
            [@"CoastalWater"] = new WildernessGroupedTerrainDomainSpec(
                @"Salt water dominates the immediate surroundings, but the shore still leaves its mark on the scene. environment{day=In fuller light, colour changes in the water and the traces of tide stand out clearly.}{night=At night the movement of the water speaks louder than its detail.}{rain=Rain roughens the surface and flattens the more delicate colour differences.}{spring=Warmer conditions make littoral life and salt smell more obvious.}{The place feels open, tidal, and in constant low motion.}",
                new[] { @"Gentle Lapping", @"Spray Marks", @"Tide Pool Basin", @"Kelp Wrack", @"Coral Heads" },
                new[] { @"Foam Line", @"Drift Line", @"Open Fetch", @"Glassy Surface" },
                new[] { @"Surf Wash", @"Waterfowl Calls" },
                new[] { @"Salt Spray", @"Kelp Tang" },
                new[] { @"Fish Shoal", @"Coral Growth", @"Sand Deposit" }),
            [@"RiverWater"] = new WildernessGroupedTerrainDomainSpec(
                @"Flow is the first fact of this place, whether the channel is wide, narrow, shallow, or deep. environment{day=Light shows the grain of the current and the shape of the channel more clearly.}{night=At night the river is read more by sound and pull than by sight.}{rain=Rain thickens the sense of movement and wakes every side channel or eddy.}{spring=Seasonal rise lends the water extra force and impatience.}{Nothing here feels as settled as standing ground.}",
                new[] { @"Fast Current", @"Braided Flow", @"Deep Channel", @"Foam Line", @"Reed Margin" },
                new[] { @"Waterweed", @"Mud Slick", @"Pebble Wash", @"Drift Line" },
                new[] { @"River Rush", @"Water Murmur", @"Waterfowl Calls" },
                new[] { @"River Silt Smell", @"Wet Earth Scent" },
                new[] { @"Fish Shoal", @"Clay Deposit", @"Reed Harvest" }),
            [@"LakeWater"] = new WildernessGroupedTerrainDomainSpec(
                @"The water here feels held rather than driven, even when wind or weather stirs it. environment{day=Clear light reveals broader sheets of reflection, weed, or shallow margin.}{night=At night the lake becomes a dark, quieter breadth edged by softer sounds.}{rain=Rain dimples the surface into countless brief disturbances.}{autumn=Fading growth and cooler light make the margin feel barer and more open.}{Scale and stillness matter here as much as motion.}",
                new[] { @"Gentle Lapping", @"Glassy Surface", @"Reed Margin", @"Waterweed", @"Sounding Depths" },
                new[] { @"Mud Slick", @"Pebble Wash", @"Foam Line", @"Drift Line" },
                new[] { @"Lake Lapping", @"Waterfowl Calls", @"Water Murmur" },
                new[] { @"Wet Earth Scent", @"River Silt Smell" },
                new[] { @"Fish Shoal", @"Clay Deposit", @"Reed Harvest" }),
            [@"OpenOcean"] = new WildernessGroupedTerrainDomainSpec(
                @"Open water stretches far enough here that scale becomes difficult to judge by anything but sky and swell. environment{day=In stronger light the distance and moving surface feel almost boundless.}{night=At night the sea is more depth and motion than visible detail.}{rain=Rain roughens the surface and pulls the horizon in.}{heavyrain=Heavy weather makes the larger water feel forceful long before anything breaks nearby.}{The place feels exposed in every direction.}",
                new[] { @"Long Swell", @"Whitecaps", @"Open Fetch", @"Swell Lift", @"Sounding Depths" },
                new[] { @"Pelagic Stillness", @"Horizon Blur", @"Spray Marks", @"Glassy Surface" },
                new[] { @"Surf Wash" },
                new[] { @"Salt Spray" },
                new[] { @"Fish Shoal" }),
            [@"Lunar"] = new WildernessGroupedTerrainDomainSpec(
                @"Exposure, dust, and hard-baked stone make the landscape feel stripped to essentials. environment{day=In full light the ground looks brutally clear, each edge and crater thrown into hard relief.}{night=Without stronger light, the surface becomes a harsher geometry of pale and dark.}{dawn=Low light exaggerates every rise and hollow across the terrain.}{The place feels ancient, airless, and utterly unsoftened.}",
                new[] { @"Dusty Regolith", @"Glassy Impact Fragments", @"Crater Lip", @"Basalt Sheet", @"Bright Highlands" },
                new[] { @"Jagged Rubble", @"Hard Vacuum Silence", @"Sparse Starlight", @"Planetary Arc" },
                new[] { @"Vacuum Silence" },
                Array.Empty<string>(),
                new[] { @"Ore Vein", @"Stone Deposit" }),
            [@"Asteroid"] = new WildernessGroupedTerrainDomainSpec(
                @"The ground here feels accidental rather than weathered, as though shaped by impact and fracture rather than by climate. environment{day=Light turns sharp rubble and broken surfaces into a chaos of hard angles.}{night=In weaker light the asteroid's roughness is felt more by outline than by detail.}{dawn=Low-angle light picks out pits, shards, and abrupt changes in surface.}{The whole place feels provisional, exposed, and unstable in scale.}",
                new[] { @"Jagged Rubble", @"Glassy Impact Fragments", @"Crater Lip", @"Slow Tumble", @"Hard Vacuum Silence" },
                new[] { @"Dusty Regolith", @"Sparse Starlight", @"Void Blackness", @"Planetary Arc" },
                new[] { @"Vacuum Silence" },
                Array.Empty<string>(),
                new[] { @"Ore Vein", @"Stone Deposit" }),
            [@"NearSpace"] = new WildernessGroupedTerrainDomainSpec(
                @"No air, no weather, and no natural ground soften the space here into something familiar. environment{day=Hard light leaves sharp contrasts and stark-edged shadow wherever it falls.}{night=Without direct light, only distant stars and lit objects give the void any structure at all.}{dawn=Changing light slowly reveals hull, debris, or orbital structure by degrees.}{Everything nearby seems isolated from everything else.}",
                new[] { @"Hard Vacuum Silence", @"Orbital Debris Glint", @"Planetary Arc", @"Sunlit Hull Glint", @"Station Shadow" },
                new[] { @"Remote Beacon", @"Sparse Starlight", @"Dense Starfield", @"Slow Tumble" },
                new[] { @"Vacuum Silence" },
                Array.Empty<string>(),
                Array.Empty<string>()),
            [@"DeepSpace"] = new WildernessGroupedTerrainDomainSpec(
                @"The surrounding void is vast enough to make even large objects seem lonely and slight. environment{day=Direct light is only meaningful where it strikes a surface; beyond that, the dark remains absolute.}{night=Without nearby illumination, distance is measured almost entirely in points of starlight.}{dawn=Shifting light changes little except the few surfaces close enough to catch it.}{The place feels remote beyond ordinary intuition.}",
                new[] { @"Void Blackness", @"Dense Starfield", @"Sparse Starlight", @"Distant Nebula", @"Galactic Haze" },
                new[] { @"Hard Vacuum Silence", @"Remote Beacon", @"Slow Tumble", @"Distant Galaxy Smear" },
                new[] { @"Vacuum Silence" },
                Array.Empty<string>(),
                Array.Empty<string>())
        };
    }

    private static IReadOnlyDictionary<string, WildernessGroupedTerrainFeatureSpec> BuildWildernessGroupedTerrainFeatureSpecs()
    {
        return new Dictionary<string, WildernessGroupedTerrainFeatureSpec>(StringComparer.OrdinalIgnoreCase)
        {
            [@"Animal Musk"] = new WildernessGroupedTerrainFeatureSpec(
                @"Animal Musk",
                @"Open Land Smell",
                new[] { @"environment{day=There is the occasional musky taint of animals that have passed this way recently enough.}{rain=Rain thins the smell, but does not erase it entirely.}{night=At night the musk can seem stronger in the cooling air.}{A faint animal musk lingers here and there.}", @"environment{day=The place smells, in part, of nearby or recently passing animals.}{spring=The greener season does little to hide the warm musk of beasts.}{There is a lived-in, animal scent to the ground.}", @"environment{morning=Cool morning air can make the smell of animals easier to separate from grass and earth.}{night=The same scent seems heavier after dark.}{Animal presence leaves a musky thread in the air.}", @"environment{day=The musk is not overwhelming, but it is unmistakably organic and warm.}{rain=Wet weather muddies the scent without entirely suppressing it.}{The air carries a light trace of beast.}" },
                75.0,
                false),
            [@"Animal Run"] = new WildernessGroupedTerrainFeatureSpec(
                @"Animal Run",
                @"Open Land Feature",
                new[] { @"A narrow and repeated animal line cuts through the growth where smaller bodies pass most often.", @"At a glance, a narrow and repeated animal line cuts through the growth where smaller bodies pass most often.", @"One of the clearest local details is that a narrow and repeated animal line cuts through the growth where smaller bodies pass most often.", @"The eye is drawn to the fact that a narrow and repeated animal line cuts through the growth where smaller bodies pass most often.", @"A narrow and repeated animal line cuts through the growth where smaller bodies pass most often, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Animal Rustle"] = new WildernessGroupedTerrainFeatureSpec(
                @"Animal Rustle",
                @"Forest Sound",
                new[] { @"environment{day=Something small moves now and then through the nearby cover, setting leaves and stems rustling.}{night=At night the unseen life in the brush feels closer, betrayed only by sudden rustles.}{rain=Rain masks the smaller noises until something larger displaces the cover.}{There are occasional rustles from unseen animals in the nearby growth.}", @"environment{dawn=Early movement in the cover produces quick, furtive sounds before the day settles in.}{dusk=The undergrowth stirs more often as the light wanes.}{night=The brush gives away hidden movement in sudden, nervous fits.}{Unseen animals make their presence known in passing rustles.}", @"environment{spring=New growth hides the animals well, but not well enough to silence every stir.}{autumn=Dryer litter makes every small movement easier to hear.}{The surrounding cover never stays wholly still for long.}", @"environment{day=The odd snap of twig or brush of leaves hints at foraging life just out of sight.}{night=After dark, the sound of movement seems to come from the edges more often than the middle.}{Hidden animals keep disturbing the quieter margins of the place.}" },
                80.0,
                false),
            [@"Ash Dust"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ash Dust",
                @"Volcanic Feature",
                new[] { @"Fine ash or cinder dust lies over the ground in a dark, easily disturbed layer.", @"At a glance, fine ash or cinder dust lies over the ground in a dark, easily disturbed layer.", @"One of the clearest local details is that fine ash or cinder dust lies over the ground in a dark, easily disturbed layer.", @"The eye is drawn to the fact that fine ash or cinder dust lies over the ground in a dark, easily disturbed layer.", @"Fine ash or cinder dust lies over the ground in a dark, easily disturbed layer, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Avalanche Debris"] = new WildernessGroupedTerrainFeatureSpec(
                @"Avalanche Debris",
                @"Rock Feature",
                new[] { @"Old spill-lines of stone show where heavier material has come down from above.", @"At a glance, old spill-lines of stone show where heavier material has come down from above.", @"One of the clearest local details is that old spill-lines of stone show where heavier material has come down from above.", @"The eye is drawn to the fact that old spill-lines of stone show where heavier material has come down from above.", @"Old spill-lines of stone show where heavier material has come down from above, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Bare Earth Patches"] = new WildernessGroupedTerrainFeatureSpec(
                @"Bare Earth Patches",
                @"Open Land Feature",
                new[] { @"The living cover gives way in places to exposed earth scuffed bare by weather or use.", @"At a glance, the living cover gives way in places to exposed earth scuffed bare by weather or use.", @"One of the clearest local details is that the living cover gives way in places to exposed earth scuffed bare by weather or use.", @"The eye is drawn to the fact that the living cover gives way in places to exposed earth scuffed bare by weather or use.", @"The living cover gives way in places to exposed earth scuffed bare by weather or use, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Basalt Sheet"] = new WildernessGroupedTerrainFeatureSpec(
                @"Basalt Sheet",
                @"Extraterrestrial Feature",
                new[] { @"Dark smoother stone lies spread here in a broad and ancient sheet.", @"At a glance, dark smoother stone lies spread here in a broad and ancient sheet.", @"One of the clearest local details is that dark smoother stone lies spread here in a broad and ancient sheet.", @"The eye is drawn to the fact that dark smoother stone lies spread here in a broad and ancient sheet.", @"Dark smoother stone lies spread here in a broad and ancient sheet, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Bat Flutter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Bat Flutter",
                @"Cave Sound",
                new[] { @"environment{night=Small wings disturb the cave air now and then with abrupt leathery flutters.}{day=The cave's higher darkness occasionally releases the sound of restless wings.}{rain=Wet weather can stir hidden roosts into brief complaint.}{There is the occasional flutter of small wings from the darker reaches.}", @"environment{day=Something roosting above shifts just often enough to be heard.}{night=At night the hidden fliers are less willing to remain wholly silent.}{The upper dark is not entirely unoccupied.}", @"environment{night=Now and then, unseen bats or cave birds worry the air with quick motion.}{day=Even by day, the ceiling-hollows sometimes betray life overhead.}{The cave's ceiling keeps its own small secrets.}", @"environment{day=The sound is brief, soft, and enough to remind one that the cave is inhabited.}{night=After dark the same flutter can seem much closer than it really is.}{Life in the cave sometimes announces itself by wingbeat alone.}" },
                80.0,
                false),
            [@"Beast Tracks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Beast Tracks",
                @"Road Feature",
                new[] { @"Animal prints and sign have mixed freely with the marks of travellers here.", @"At a glance, animal prints and sign have mixed freely with the marks of travellers here.", @"One of the clearest local details is that animal prints and sign have mixed freely with the marks of travellers here.", @"The eye is drawn to the fact that animal prints and sign have mixed freely with the marks of travellers here.", @"Animal prints and sign have mixed freely with the marks of travellers here, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Bird Chorus"] = new WildernessGroupedTerrainFeatureSpec(
                @"Bird Chorus",
                @"Forest Sound",
                new[] { @"environment{dawn=The forest wakes with a layered bird chorus that seems to come from every level of the trees at once.}{day=Birdsong threads through the trees in overlapping calls.}{rain=Rain suppresses the smaller singers until only scattered notes remain.}{night=The daytime chorus dies back to the occasional restless call.}{Birdsong moves through the trees in overlapping threads.}", @"environment{spring=In spring the birds sound fuller, sharper, and more intent on being heard.}{autumn=The birds still call, but less constantly than in the greener months.}{day=The canopy carries a lively exchange of birdsong.}{The trees trade bird calls from branch to branch.}", @"environment{morning=Morning turns the upper branches into a busy mesh of birdsong.}{afternoon=By afternoon the chorus becomes more intermittent, but never vanishes entirely.}{night=Only an occasional note breaks the darker hush.}{The forest air is rarely long without some bird call in it.}", @"environment{day=Different birds answer one another from different heights in the cover.}{rain=Wet leaves and rain-muffled boughs make the birds sound more distant than they are.}{Bird calls keep finding paths through the trees.}" },
                80.0,
                false),
            [@"Blackened Crack"] = new WildernessGroupedTerrainFeatureSpec(
                @"Blackened Crack",
                @"Volcanic Feature",
                new[] { @"Cracks through the ground or stone are dark enough to stand out even at a glance.", @"At a glance, cracks through the ground or stone are dark enough to stand out even at a glance.", @"One of the clearest local details is that cracks through the ground or stone are dark enough to stand out even at a glance.", @"The eye is drawn to the fact that cracks through the ground or stone are dark enough to stand out even at a glance.", @"Cracks through the ground or stone are dark enough to stand out even at a glance, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Bleached Stone"] = new WildernessGroupedTerrainFeatureSpec(
                @"Bleached Stone",
                @"Desert Feature",
                new[] { @"Exposed stone looks sun-bleached and long stripped of softer colour.", @"At a glance, exposed stone looks sun-bleached and long stripped of softer colour.", @"One of the clearest local details is that exposed stone looks sun-bleached and long stripped of softer colour.", @"The eye is drawn to the fact that exposed stone looks sun-bleached and long stripped of softer colour.", @"Exposed stone looks sun-bleached and long stripped of softer colour, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Blue Ice"] = new WildernessGroupedTerrainFeatureSpec(
                @"Blue Ice",
                @"Glacial Feature",
                new[] { @"Dense ice shows through with a colder, bluer clarity than ordinary snow can manage.", @"At a glance, dense ice shows through with a colder, bluer clarity than ordinary snow can manage.", @"One of the clearest local details is that dense ice shows through with a colder, bluer clarity than ordinary snow can manage.", @"The eye is drawn to the fact that dense ice shows through with a colder, bluer clarity than ordinary snow can manage.", @"Dense ice shows through with a colder, bluer clarity than ordinary snow can manage, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Boggy Roots"] = new WildernessGroupedTerrainFeatureSpec(
                @"Boggy Roots",
                @"Forest Feature",
                new[] { @"Roots and damp ground together make the footing here less certain than it first appears.", @"At a glance, roots and damp ground together make the footing here less certain than it first appears.", @"One of the clearest local details is that roots and damp ground together make the footing here less certain than it first appears.", @"The eye is drawn to the fact that roots and damp ground together make the footing here less certain than it first appears.", @"Roots and damp ground together make the footing here less certain than it first appears, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Boulder Scatter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Boulder Scatter",
                @"Rock Feature",
                new[] { @"Boulders or larger stones lie scattered across the ground in irregular placements.", @"At a glance, boulders or larger stones lie scattered across the ground in irregular placements.", @"One of the clearest local details is that boulders or larger stones lie scattered across the ground in irregular placements.", @"The eye is drawn to the fact that boulders or larger stones lie scattered across the ground in irregular placements.", @"Boulders or larger stones lie scattered across the ground in irregular placements, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Brackish Rot Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Brackish Rot Smell",
                @"Wetland Smell",
                new[] { @"environment{day=The air carries the half-salt, half-rotten smell of brackish wet ground.}{rain=Rain freshens the surface a little, but not enough to hide the rot.}{night=At night the sour brackish smell seems to linger lower over the mud.}{There is a brackish, rotting smell here.}", @"environment{summer=Warmth coaxes a more sour, tidal stink from the mud.}{winter=Cooler air keeps the smell leaner, though no more pleasant.}{The place smells of salt-tainted decay.}", @"environment{day=Salt and rot share the air here without either quite mastering the other.}{rain=Wet weather muddies the line between tidal salt and old organic stink.}{The wet margin smells both marine and decayed.}", @"environment{morning=Morning damp makes the sour salt-smell especially evident.}{afternoon=Heat turns the brackish reek stronger and more insistent.}{The marsh smells of tidal mud and old water.}" },
                75.0,
                false),
            [@"Brackish Slick"] = new WildernessGroupedTerrainFeatureSpec(
                @"Brackish Slick",
                @"Wetland Feature",
                new[] { @"A faint mineral slick tells of salt mixed imperfectly with trapped fresh water.", @"At a glance, a faint mineral slick tells of salt mixed imperfectly with trapped fresh water.", @"One of the clearest local details is that a faint mineral slick tells of salt mixed imperfectly with trapped fresh water.", @"The eye is drawn to the fact that a faint mineral slick tells of salt mixed imperfectly with trapped fresh water.", @"A faint mineral slick tells of salt mixed imperfectly with trapped fresh water, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Braided Flow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Braided Flow",
                @"Water Feature",
                new[] { @"The water divides and rejoins in smaller moving lines rather than keeping one clear channel.", @"At a glance, the water divides and rejoins in smaller moving lines rather than keeping one clear channel.", @"One of the clearest local details is that the water divides and rejoins in smaller moving lines rather than keeping one clear channel.", @"The eye is drawn to the fact that the water divides and rejoins in smaller moving lines rather than keeping one clear channel.", @"The water divides and rejoins in smaller moving lines rather than keeping one clear channel, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Bright Highlands"] = new WildernessGroupedTerrainFeatureSpec(
                @"Bright Highlands",
                @"Extraterrestrial Feature",
                new[] { @"Paler rougher ground stands apart from darker nearby surfaces.", @"At a glance, paler rougher ground stands apart from darker nearby surfaces.", @"One of the clearest local details is that paler rougher ground stands apart from darker nearby surfaces.", @"The eye is drawn to the fact that paler rougher ground stands apart from darker nearby surfaces.", @"Paler rougher ground stands apart from darker nearby surfaces, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Broad Pavement"] = new WildernessGroupedTerrainFeatureSpec(
                @"Broad Pavement",
                @"Urban Feature",
                new[] { @"The walking surface feels generous enough to suggest planning rather than mere expedience.", @"At a glance, the walking surface feels generous enough to suggest planning rather than mere expedience.", @"One of the clearest local details is that the walking surface feels generous enough to suggest planning rather than mere expedience.", @"The eye is drawn to the fact that the walking surface feels generous enough to suggest planning rather than mere expedience.", @"The walking surface feels generous enough to suggest planning rather than mere expedience, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Broadleaf Shade"] = new WildernessGroupedTerrainFeatureSpec(
                @"Broadleaf Shade",
                @"Forest Feature",
                new[] { @"Wide leaves shape the light into larger, shifting patches rather than thin lines.", @"At a glance, wide leaves shape the light into larger, shifting patches rather than thin lines.", @"One of the clearest local details is that wide leaves shape the light into larger, shifting patches rather than thin lines.", @"The eye is drawn to the fact that wide leaves shape the light into larger, shifting patches rather than thin lines.", @"Wide leaves shape the light into larger, shifting patches rather than thin lines, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Broken Obsidian"] = new WildernessGroupedTerrainFeatureSpec(
                @"Broken Obsidian",
                @"Volcanic Feature",
                new[] { @"Glassy fragments catch the light sharply among duller surrounding rock.", @"At a glance, glassy fragments catch the light sharply among duller surrounding rock.", @"One of the clearest local details is that glassy fragments catch the light sharply among duller surrounding rock.", @"The eye is drawn to the fact that glassy fragments catch the light sharply among duller surrounding rock.", @"Glassy fragments catch the light sharply among duller surrounding rock, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Broken Paving"] = new WildernessGroupedTerrainFeatureSpec(
                @"Broken Paving",
                @"Urban Feature",
                new[] { @"The surface underfoot has failed in places, leaving irregular footing and visible neglect.", @"At a glance, the surface underfoot has failed in places, leaving irregular footing and visible neglect.", @"One of the clearest local details is that the surface underfoot has failed in places, leaving irregular footing and visible neglect.", @"The eye is drawn to the fact that the surface underfoot has failed in places, leaving irregular footing and visible neglect.", @"The surface underfoot has failed in places, leaving irregular footing and visible neglect, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Broken Scree"] = new WildernessGroupedTerrainFeatureSpec(
                @"Broken Scree",
                @"Rock Feature",
                new[] { @"Loose fractured stone covers the slope in unstable, shifting fragments.", @"At a glance, loose fractured stone covers the slope in unstable, shifting fragments.", @"One of the clearest local details is that loose fractured stone covers the slope in unstable, shifting fragments.", @"The eye is drawn to the fact that loose fractured stone covers the slope in unstable, shifting fragments.", @"Loose fractured stone covers the slope in unstable, shifting fragments, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Canopy Insects"] = new WildernessGroupedTerrainFeatureSpec(
                @"Canopy Insects",
                @"Forest Sound",
                new[] { @"environment{humid=The humid air carries a steady insect drone from leaves, bark, and hidden hollows.}{night=The insect noise thickens after dark into a denser, less individual hum.}{rain=Rain flattens the smaller buzzing noises for a time.}{There is a constant insect undertone to the place.}", @"environment{day=Insects buzz and rasp from the warmer reaches of bark and leaf.}{dusk=As dusk deepens, the insect chorus grows steadier and more enveloping.}{night=After nightfall the droning seems to settle over everything.}{The canopy hides a busy, persistent insect life.}", @"environment{summer=Warm weather teases a near-constant buzzing from the cover.}{spring=Fresh warmth wakes the smaller insects into noisy activity.}{rain=Heavy rain drives the insect noise downward and duller.}{Insects lend the air a thin, ceaseless vibration.}", @"environment{day=The sound is not loud, but it is everywhere once noticed.}{night=In darkness the insect drone can seem larger than the trees themselves.}{The smaller lives of the forest are mostly betrayed by their noise.}" },
                80.0,
                false),
            [@"Carrion Interest"] = new WildernessGroupedTerrainFeatureSpec(
                @"Carrion Interest",
                @"Urban Feature",
                new[] { @"Birds, insects, or smaller scavengers have clearly found reason to return here.", @"At a glance, birds, insects, or smaller scavengers have clearly found reason to return here.", @"One of the clearest local details is that birds, insects, or smaller scavengers have clearly found reason to return here.", @"The eye is drawn to the fact that birds, insects, or smaller scavengers have clearly found reason to return here.", @"Birds, insects, or smaller scavengers have clearly found reason to return here, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Cave Drip Echo"] = new WildernessGroupedTerrainFeatureSpec(
                @"Cave Drip Echo",
                @"Cave Sound",
                new[] { @"environment{day=Water falls somewhere in the cave with a patient, echoing drip.}{night=The cave's drip is no less regular in darkness, only more dominant.}{rain=Rain beyond the stone fattens the dripping into a busier pattern.}{A patient drip echoes somewhere nearby.}", @"environment{day=The cave measures time in small falling drops and their echoes.}{rain=Wet weather outside multiplies the number of answering drips.}{The place is accompanied by the quiet punctuation of dripping water.}", @"environment{night=In darkness the drip seems to define more of the chamber than the eye can.}{spring=Runoff lends the cave a livelier pattern of drops.}{Water somewhere above or beyond keeps up a measured dripping.}", @"environment{day=Each falling drop sounds larger than it ought to in the cave air.}{rain=Recent rain can be heard long after it has finished beyond the entrance.}{The cave rarely finds complete silence while water is at work.}" },
                80.0,
                false),
            [@"Chained Fixtures"] = new WildernessGroupedTerrainFeatureSpec(
                @"Chained Fixtures",
                @"Urban Feature",
                new[] { @"Restraint points or fixed fittings make the room feel controlled even when empty.", @"At a glance, restraint points or fixed fittings make the room feel controlled even when empty.", @"One of the clearest local details is that restraint points or fixed fittings make the room feel controlled even when empty.", @"The eye is drawn to the fact that restraint points or fixed fittings make the room feel controlled even when empty.", @"Restraint points or fixed fittings make the room feel controlled even when empty, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Chalky Dust"] = new WildernessGroupedTerrainFeatureSpec(
                @"Chalky Dust",
                @"Open Land Feature",
                new[] { @"Fine pale dust sits over the ground and clings readily to disturbance.", @"At a glance, fine pale dust sits over the ground and clings readily to disturbance.", @"One of the clearest local details is that fine pale dust sits over the ground and clings readily to disturbance.", @"The eye is drawn to the fact that fine pale dust sits over the ground and clings readily to disturbance.", @"Fine pale dust sits over the ground and clings readily to disturbance, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Cindered Hollow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Cindered Hollow",
                @"Volcanic Feature",
                new[] { @"One slight hollow or low pocket has gathered darker ash than the surrounding ground.", @"At a glance, one slight hollow or low pocket has gathered darker ash than the surrounding ground.", @"One of the clearest local details is that one slight hollow or low pocket has gathered darker ash than the surrounding ground.", @"The eye is drawn to the fact that one slight hollow or low pocket has gathered darker ash than the surrounding ground.", @"One slight hollow or low pocket has gathered darker ash than the surrounding ground, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Clay Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Clay Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Clean Cold Scent"] = new WildernessGroupedTerrainFeatureSpec(
                @"Clean Cold Scent",
                @"Glacial Smell",
                new[] { @"environment{day=The air smells clean, cold, and spare in the way only snow and ice can manage.}{spring=Melt softens that clean cold with a little mineral wetness.}{night=At night the cold smell feels almost empty of everything but frost.}{The air carries a clean cold scent of snow and ice.}", @"environment{winter=The frozen season gives the air a severe, pared-down cleanness.}{spring=Thaw complicates the purity with wet stone and runoff.}{The place smells mostly of cold itself.}", @"environment{day=Snow and ice leave the air strangely clean and mineral-light.}{night=The same scent grows thinner and more austere after dark.}{There is almost nothing in the air but cold.}", @"environment{morning=Morning cold is especially sharp and scent-poor.}{afternoon=Even under fuller light the air stays clean and frozen.}{The smell here is largely the absence of all warmer things.}" },
                75.0,
                false),
            [@"Clean Stonework"] = new WildernessGroupedTerrainFeatureSpec(
                @"Clean Stonework",
                @"Urban Feature",
                new[] { @"Well-kept stone and fitted edges suggest money, labour, or civic attention.", @"At a glance, well-kept stone and fitted edges suggest money, labour, or civic attention.", @"One of the clearest local details is that well-kept stone and fitted edges suggest money, labour, or civic attention.", @"The eye is drawn to the fact that well-kept stone and fitted edges suggest money, labour, or civic attention.", @"Well-kept stone and fitted edges suggest money, labour, or civic attention, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Clear Understory"] = new WildernessGroupedTerrainFeatureSpec(
                @"Clear Understory",
                @"Forest Feature",
                new[] { @"The space beneath the trees is more open than the trunks alone first suggest.", @"At a glance, the space beneath the trees is more open than the trunks alone first suggest.", @"One of the clearest local details is that the space beneath the trees is more open than the trunks alone first suggest.", @"The eye is drawn to the fact that the space beneath the trees is more open than the trunks alone first suggest.", @"The space beneath the trees is more open than the trunks alone first suggest, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Cliff Shadow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Cliff Shadow",
                @"Rock Feature",
                new[] { @"The shadow of larger stone lies over the area longer and more heavily than the open ground around it.", @"At a glance, the shadow of larger stone lies over the area longer and more heavily than the open ground around it.", @"One of the clearest local details is that the shadow of larger stone lies over the area longer and more heavily than the open ground around it.", @"The eye is drawn to the fact that the shadow of larger stone lies over the area longer and more heavily than the open ground around it.", @"The shadow of larger stone lies over the area longer and more heavily than the open ground around it, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Close Ceiling"] = new WildernessGroupedTerrainFeatureSpec(
                @"Close Ceiling",
                @"Urban Feature",
                new[] { @"The space presses down a little, more concerned with containment than ease.", @"At a glance, the space presses down a little, more concerned with containment than ease.", @"One of the clearest local details is that the space presses down a little, more concerned with containment than ease.", @"The eye is drawn to the fact that the space presses down a little, more concerned with containment than ease.", @"The space presses down a little, more concerned with containment than ease, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Communal Benches"] = new WildernessGroupedTerrainFeatureSpec(
                @"Communal Benches",
                @"Urban Feature",
                new[] { @"Shared seating or resting places suggest use by many people rather than one household alone.", @"At a glance, shared seating or resting places suggest use by many people rather than one household alone.", @"One of the clearest local details is that shared seating or resting places suggest use by many people rather than one household alone.", @"The eye is drawn to the fact that shared seating or resting places suggest use by many people rather than one household alone.", @"Shared seating or resting places suggest use by many people rather than one household alone, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Compacted Wheel Marks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Compacted Wheel Marks",
                @"Road Feature",
                new[] { @"Wheel or cart traffic has pressed the route into a firmer and more reliable line.", @"At a glance, wheel or cart traffic has pressed the route into a firmer and more reliable line.", @"One of the clearest local details is that wheel or cart traffic has pressed the route into a firmer and more reliable line.", @"The eye is drawn to the fact that wheel or cart traffic has pressed the route into a firmer and more reliable line.", @"Wheel or cart traffic has pressed the route into a firmer and more reliable line, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Conifer Needles"] = new WildernessGroupedTerrainFeatureSpec(
                @"Conifer Needles",
                @"Forest Feature",
                new[] { @"Needles soften the ground underfoot and lend the place a cleaner, resin-touched scent.", @"At a glance, needles soften the ground underfoot and lend the place a cleaner, resin-touched scent.", @"One of the clearest local details is that needles soften the ground underfoot and lend the place a cleaner, resin-touched scent.", @"The eye is drawn to the fact that needles soften the ground underfoot and lend the place a cleaner, resin-touched scent.", @"Needles soften the ground underfoot and lend the place a cleaner, resin-touched scent, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Cooling Basalt"] = new WildernessGroupedTerrainFeatureSpec(
                @"Cooling Basalt",
                @"Volcanic Feature",
                new[] { @"Dark volcanic stone dominates here, rough-textured and still suggestive of old heat.", @"At a glance, dark volcanic stone dominates here, rough-textured and still suggestive of old heat.", @"One of the clearest local details is that dark volcanic stone dominates here, rough-textured and still suggestive of old heat.", @"The eye is drawn to the fact that dark volcanic stone dominates here, rough-textured and still suggestive of old heat.", @"Dark volcanic stone dominates here, rough-textured and still suggestive of old heat, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Coral Growth"] = new WildernessGroupedTerrainFeatureSpec(
                @"Coral Growth",
                @"Aquatic Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Coral Heads"] = new WildernessGroupedTerrainFeatureSpec(
                @"Coral Heads",
                @"Water Feature",
                new[] { @"Hard living growth lifts from the shallows in pale and irregular forms.", @"At a glance, hard living growth lifts from the shallows in pale and irregular forms.", @"One of the clearest local details is that hard living growth lifts from the shallows in pale and irregular forms.", @"The eye is drawn to the fact that hard living growth lifts from the shallows in pale and irregular forms.", @"Hard living growth lifts from the shallows in pale and irregular forms, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Corniced Edge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Corniced Edge",
                @"Rock Feature",
                new[] { @"Wind and exposure have left the edge sharp enough to look undercut from a distance.", @"At a glance, wind and exposure have left the edge sharp enough to look undercut from a distance.", @"One of the clearest local details is that wind and exposure have left the edge sharp enough to look undercut from a distance.", @"The eye is drawn to the fact that wind and exposure have left the edge sharp enough to look undercut from a distance.", @"Wind and exposure have left the edge sharp enough to look undercut from a distance, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Cracked Earth"] = new WildernessGroupedTerrainFeatureSpec(
                @"Cracked Earth",
                @"Open Land Feature",
                new[] { @"Drying ground has broken into hard plates and seams that shift little underfoot.", @"At a glance, drying ground has broken into hard plates and seams that shift little underfoot.", @"One of the clearest local details is that drying ground has broken into hard plates and seams that shift little underfoot.", @"The eye is drawn to the fact that drying ground has broken into hard plates and seams that shift little underfoot.", @"Drying ground has broken into hard plates and seams that shift little underfoot, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Crater Lip"] = new WildernessGroupedTerrainFeatureSpec(
                @"Crater Lip",
                @"Extraterrestrial Feature",
                new[] { @"A raised rim interrupts the broader surface and hints at older violence in the landscape.", @"At a glance, a raised rim interrupts the broader surface and hints at older violence in the landscape.", @"One of the clearest local details is that a raised rim interrupts the broader surface and hints at older violence in the landscape.", @"The eye is drawn to the fact that a raised rim interrupts the broader surface and hints at older violence in the landscape.", @"A raised rim interrupts the broader surface and hints at older violence in the landscape, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Crevasse Hint"] = new WildernessGroupedTerrainFeatureSpec(
                @"Crevasse Hint",
                @"Glacial Feature",
                new[] { @"A subtle break or dark line suggests deeper ice-structure beneath the visible surface.", @"At a glance, a subtle break or dark line suggests deeper ice-structure beneath the visible surface.", @"One of the clearest local details is that a subtle break or dark line suggests deeper ice-structure beneath the visible surface.", @"The eye is drawn to the fact that a subtle break or dark line suggests deeper ice-structure beneath the visible surface.", @"A subtle break or dark line suggests deeper ice-structure beneath the visible surface, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Crushed Grass Scent"] = new WildernessGroupedTerrainFeatureSpec(
                @"Crushed Grass Scent",
                @"Open Land Smell",
                new[] { @"environment{day=The air carries the green smell of grass bruised underfoot or by passing animals.}{rain=Rain deepens the scent of crushed grass into something wetter and richer.}{night=At night the grassy smell sits lower and cooler in the air.}{There is a fresh, bruised-grass scent to the place.}", @"environment{spring=Fresh growth makes the grassy smell sharper and greener.}{autumn=Drying stalks leave the scent fainter and rougher.}{The smell of bent or broken grass hangs lightly in the air.}", @"environment{day=Warmth draws a sweet green scent from the grass.}{rain=Wet conditions make the smell cling more obviously.}{The ground smells of recently bruised grass.}", @"environment{morning=Morning damp makes the grassy scent especially noticeable.}{afternoon=Sun-warm stems release a drier green smell.}{The place smells of living grass disturbed in passing.}" },
                75.0,
                false),
            [@"Damp Corners"] = new WildernessGroupedTerrainFeatureSpec(
                @"Damp Corners",
                @"Urban Feature",
                new[] { @"Moisture has found the least-aired corners, leaving them a little cooler and less pleasant than the rest.", @"At a glance, moisture has found the least-aired corners, leaving them a little cooler and less pleasant than the rest.", @"One of the clearest local details is that moisture has found the least-aired corners, leaving them a little cooler and less pleasant than the rest.", @"The eye is drawn to the fact that moisture has found the least-aired corners, leaving them a little cooler and less pleasant than the rest.", @"Moisture has found the least-aired corners, leaving them a little cooler and less pleasant than the rest, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Darkness Pocket"] = new WildernessGroupedTerrainFeatureSpec(
                @"Darkness Pocket",
                @"Cave Feature",
                new[] { @"The cave light fails badly in one quarter, leaving a pocket of deeper shadow.", @"At a glance, the cave light fails badly in one quarter, leaving a pocket of deeper shadow.", @"One of the clearest local details is that the cave light fails badly in one quarter, leaving a pocket of deeper shadow.", @"The eye is drawn to the fact that the cave light fails badly in one quarter, leaving a pocket of deeper shadow.", @"The cave light fails badly in one quarter, leaving a pocket of deeper shadow, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Debris Snag"] = new WildernessGroupedTerrainFeatureSpec(
                @"Debris Snag",
                @"Open Land Feature",
                new[] { @"Twigs, stalks, and flood-borne debris have caught against minor rises and vegetation.", @"At a glance, twigs, stalks, and flood-borne debris have caught against minor rises and vegetation.", @"One of the clearest local details is that twigs, stalks, and flood-borne debris have caught against minor rises and vegetation.", @"The eye is drawn to the fact that twigs, stalks, and flood-borne debris have caught against minor rises and vegetation.", @"Twigs, stalks, and flood-borne debris have caught against minor rises and vegetation, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Deep Channel"] = new WildernessGroupedTerrainFeatureSpec(
                @"Deep Channel",
                @"Water Feature",
                new[] { @"The darker line of the water suggests a channel deeper than the margins around it.", @"At a glance, the darker line of the water suggests a channel deeper than the margins around it.", @"One of the clearest local details is that the darker line of the water suggests a channel deeper than the margins around it.", @"The eye is drawn to the fact that the darker line of the water suggests a channel deeper than the margins around it.", @"The darker line of the water suggests a channel deeper than the margins around it, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Defensive Height"] = new WildernessGroupedTerrainFeatureSpec(
                @"Defensive Height",
                @"Urban Feature",
                new[] { @"Height and line of sight make the place feel suited to watching as much as standing.", @"At a glance, height and line of sight make the place feel suited to watching as much as standing.", @"One of the clearest local details is that height and line of sight make the place feel suited to watching as much as standing.", @"The eye is drawn to the fact that height and line of sight make the place feel suited to watching as much as standing.", @"Height and line of sight make the place feel suited to watching as much as standing, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Dense Canopy"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dense Canopy",
                @"Forest Feature",
                new[] { @"The upper growth interlocks strongly enough to shape both light and weather below it.", @"At a glance, the upper growth interlocks strongly enough to shape both light and weather below it.", @"One of the clearest local details is that the upper growth interlocks strongly enough to shape both light and weather below it.", @"The eye is drawn to the fact that the upper growth interlocks strongly enough to shape both light and weather below it.", @"The upper growth interlocks strongly enough to shape both light and weather below it, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Dense Starfield"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dense Starfield",
                @"Extraterrestrial Feature",
                new[] { @"Stars crowd the view so thickly that the darkness seems textured rather than empty.", @"At a glance, stars crowd the view so thickly that the darkness seems textured rather than empty.", @"One of the clearest local details is that stars crowd the view so thickly that the darkness seems textured rather than empty.", @"The eye is drawn to the fact that stars crowd the view so thickly that the darkness seems textured rather than empty.", @"Stars crowd the view so thickly that the darkness seems textured rather than empty, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Desert Pavement"] = new WildernessGroupedTerrainFeatureSpec(
                @"Desert Pavement",
                @"Desert Feature",
                new[] { @"The surface is armoured with tightly packed stone where finer material has long since gone.", @"At a glance, the surface is armoured with tightly packed stone where finer material has long since gone.", @"One of the clearest local details is that the surface is armoured with tightly packed stone where finer material has long since gone.", @"The eye is drawn to the fact that the surface is armoured with tightly packed stone where finer material has long since gone.", @"The surface is armoured with tightly packed stone where finer material has long since gone, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Desert Wind"] = new WildernessGroupedTerrainFeatureSpec(
                @"Desert Wind",
                @"Desert Sound",
                new[] { @"environment{day=Wind moves over the dry ground with a thin, abrasive hiss.}{night=After dark the desert wind sounds colder and more spacious.}{rain=Rain is rare enough to muffle the dry hiss whenever it comes.}{The wind has a dry, sand-bearing voice here.}", @"environment{afternoon=The hotter hours sharpen the sound of wind into a finer rasp.}{night=At night the wind sounds larger than the landscape it crosses.}{Wind skates over the arid ground with a dry whisper.}", @"environment{dry=In dry conditions every gust sounds as though it is carrying dust with it.}{rain=Stronger weather turns the hiss of the wind harsher and more insistent.}{The country is exposed enough for the wind to make itself heard.}", @"environment{day=The sound is mostly a hiss, a rasp, and the occasional stony tick.}{night=The darkness leaves the wind to define the open space for itself.}{The desert rarely sounds sheltered.}" },
                80.0,
                false),
            [@"Diggable Soil Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Diggable Soil Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Distant Galaxy Smear"] = new WildernessGroupedTerrainFeatureSpec(
                @"Distant Galaxy Smear",
                @"Extraterrestrial Feature",
                new[] { @"A faint smear of distant galactic light marks part of the otherwise clean dark.", @"At a glance, a faint smear of distant galactic light marks part of the otherwise clean dark.", @"One of the clearest local details is that a faint smear of distant galactic light marks part of the otherwise clean dark.", @"The eye is drawn to the fact that a faint smear of distant galactic light marks part of the otherwise clean dark.", @"A faint smear of distant galactic light marks part of the otherwise clean dark, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Distant Nebula"] = new WildernessGroupedTerrainFeatureSpec(
                @"Distant Nebula",
                @"Extraterrestrial Feature",
                new[] { @"A faint distant cloud of colour softens one patch of the wider black.", @"At a glance, a faint distant cloud of colour softens one patch of the wider black.", @"One of the clearest local details is that a faint distant cloud of colour softens one patch of the wider black.", @"The eye is drawn to the fact that a faint distant cloud of colour softens one patch of the wider black.", @"A faint distant cloud of colour softens one patch of the wider black, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Drafty Passage"] = new WildernessGroupedTerrainFeatureSpec(
                @"Drafty Passage",
                @"Urban Feature",
                new[] { @"Air has a way of finding this space, slipping through it more freely than an enclosed room ought to allow.", @"At a glance, air has a way of finding this space, slipping through it more freely than an enclosed room ought to allow.", @"One of the clearest local details is that air has a way of finding this space, slipping through it more freely than an enclosed room ought to allow.", @"The eye is drawn to the fact that air has a way of finding this space, slipping through it more freely than an enclosed room ought to allow.", @"Air has a way of finding this space, slipping through it more freely than an enclosed room ought to allow, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Drainage Channel"] = new WildernessGroupedTerrainFeatureSpec(
                @"Drainage Channel",
                @"Urban Feature",
                new[] { @"Water has a clear route to follow here, either by design or long habit.", @"At a glance, water has a clear route to follow here, either by design or long habit.", @"One of the clearest local details is that water has a clear route to follow here, either by design or long habit.", @"The eye is drawn to the fact that water has a clear route to follow here, either by design or long habit.", @"Water has a clear route to follow here, either by design or long habit, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Drainage Ditch"] = new WildernessGroupedTerrainFeatureSpec(
                @"Drainage Ditch",
                @"Road Feature",
                new[] { @"A ditch or cut edge carries runoff away from the route when the weather turns.", @"At a glance, a ditch or cut edge carries runoff away from the route when the weather turns.", @"One of the clearest local details is that a ditch or cut edge carries runoff away from the route when the weather turns.", @"The eye is drawn to the fact that a ditch or cut edge carries runoff away from the route when the weather turns.", @"A ditch or cut edge carries runoff away from the route when the weather turns, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Drift Line"] = new WildernessGroupedTerrainFeatureSpec(
                @"Drift Line",
                @"Water Feature",
                new[] { @"Debris or lighter material has been sorted into a line by tide or current.", @"At a glance, debris or lighter material has been sorted into a line by tide or current.", @"One of the clearest local details is that debris or lighter material has been sorted into a line by tide or current.", @"The eye is drawn to the fact that debris or lighter material has been sorted into a line by tide or current.", @"Debris or lighter material has been sorted into a line by tide or current, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Drift Ridge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Drift Ridge",
                @"Desert Feature",
                new[] { @"Wind has gathered loose material into a low drifted ridge instead of leaving it evenly spread.", @"At a glance, wind has gathered loose material into a low drifted ridge instead of leaving it evenly spread.", @"One of the clearest local details is that wind has gathered loose material into a low drifted ridge instead of leaving it evenly spread.", @"The eye is drawn to the fact that wind has gathered loose material into a low drifted ridge instead of leaving it evenly spread.", @"Wind has gathered loose material into a low drifted ridge instead of leaving it evenly spread, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Dripping Masonry"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dripping Masonry",
                @"Urban Feature",
                new[] { @"Damp stone or mortar leaves the room marked by slow seepage and old moisture.", @"At a glance, damp stone or mortar leaves the room marked by slow seepage and old moisture.", @"One of the clearest local details is that damp stone or mortar leaves the room marked by slow seepage and old moisture.", @"The eye is drawn to the fact that damp stone or mortar leaves the room marked by slow seepage and old moisture.", @"Damp stone or mortar leaves the room marked by slow seepage and old moisture, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Dripping Water"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dripping Water",
                @"Cave Feature",
                new[] { @"Somewhere close by, water falls with the patient rhythm caves always seem to prefer.", @"At a glance, somewhere close by, water falls with the patient rhythm caves always seem to prefer.", @"One of the clearest local details is that somewhere close by, water falls with the patient rhythm caves always seem to prefer.", @"The eye is drawn to the fact that somewhere close by, water falls with the patient rhythm caves always seem to prefer.", @"Somewhere close by, water falls with the patient rhythm caves always seem to prefer, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Dry Earth Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dry Earth Smell",
                @"Open Land Smell",
                new[] { @"environment{dry=The ground smells dry, powdery, and sun-worked.}{rain=Rain briefly wakens a richer earth-smell from the dirt.}{night=After dark the smell of dry ground cools, but remains.}{There is a plain, dry-earth smell here.}", @"environment{day=Warm soil lends the air a dusty mineral note.}{rain=Wet earth overtakes that drier smell for a while.}{The exposed ground keeps a clear smell of earth in the air.}", @"environment{summer=Heat brings out the baked smell of open soil.}{spring=Moisture leaves the earth-smell fuller and less dusty.}{The place smells of soil with little cover to hide it.}", @"environment{day=Sun on open ground draws out a dry dirt-scent.}{night=The smell persists after the heat has gone.}{Bare patches of soil announce themselves by smell as much as sight.}" },
                75.0,
                false),
            [@"Dry Patches"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dry Patches",
                @"Open Land Feature",
                new[] { @"Dryer, paler patches interrupt the healthier growth and show where the ground gives less back.", @"At a glance, dryer, paler patches interrupt the healthier growth and show where the ground gives less back.", @"One of the clearest local details is that dryer, paler patches interrupt the healthier growth and show where the ground gives less back.", @"The eye is drawn to the fact that dryer, paler patches interrupt the healthier growth and show where the ground gives less back.", @"Dryer, paler patches interrupt the healthier growth and show where the ground gives less back, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Dry Seed Pods"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dry Seed Pods",
                @"Open Land Feature",
                new[] { @"Dry pods and brittle plant matter give the surrounding growth a sharper, more rattling character.", @"At a glance, dry pods and brittle plant matter give the surrounding growth a sharper, more rattling character.", @"One of the clearest local details is that dry pods and brittle plant matter give the surrounding growth a sharper, more rattling character.", @"The eye is drawn to the fact that dry pods and brittle plant matter give the surrounding growth a sharper, more rattling character.", @"Dry pods and brittle plant matter give the surrounding growth a sharper, more rattling character, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Dune Face"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dune Face",
                @"Desert Feature",
                new[] { @"A rising face of sand gives the immediate ground both slope and grain.", @"At a glance, a rising face of sand gives the immediate ground both slope and grain.", @"One of the clearest local details is that a rising face of sand gives the immediate ground both slope and grain.", @"The eye is drawn to the fact that a rising face of sand gives the immediate ground both slope and grain.", @"A rising face of sand gives the immediate ground both slope and grain, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Dusty Regolith"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dusty Regolith",
                @"Extraterrestrial Feature",
                new[] { @"Fine dust lies over the surface in a dry skin that looks ready to lift at the least disturbance.", @"At a glance, fine dust lies over the surface in a dry skin that looks ready to lift at the least disturbance.", @"One of the clearest local details is that fine dust lies over the surface in a dry skin that looks ready to lift at the least disturbance.", @"The eye is drawn to the fact that fine dust lies over the surface in a dry skin that looks ready to lift at the least disturbance.", @"Fine dust lies over the surface in a dry skin that looks ready to lift at the least disturbance, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Dusty Ruts"] = new WildernessGroupedTerrainFeatureSpec(
                @"Dusty Ruts",
                @"Road Feature",
                new[] { @"Repeated passage has cut shallow ruts that hold dust longer than the surrounding ground.", @"At a glance, repeated passage has cut shallow ruts that hold dust longer than the surrounding ground.", @"One of the clearest local details is that repeated passage has cut shallow ruts that hold dust longer than the surrounding ground.", @"The eye is drawn to the fact that repeated passage has cut shallow ruts that hold dust longer than the surrounding ground.", @"Repeated passage has cut shallow ruts that hold dust longer than the surrounding ground, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Echoing Chamber"] = new WildernessGroupedTerrainFeatureSpec(
                @"Echoing Chamber",
                @"Cave Feature",
                new[] { @"The space throws sound back upon itself, making it feel larger and emptier at once.", @"At a glance, the space throws sound back upon itself, making it feel larger and emptier at once.", @"One of the clearest local details is that the space throws sound back upon itself, making it feel larger and emptier at once.", @"The eye is drawn to the fact that the space throws sound back upon itself, making it feel larger and emptier at once.", @"The space throws sound back upon itself, making it feel larger and emptier at once, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Echoing Floorboards"] = new WildernessGroupedTerrainFeatureSpec(
                @"Echoing Floorboards",
                @"Urban Feature",
                new[] { @"Sound carries strangely here, picked up and thrown back by the room's hard surfaces.", @"At a glance, sound carries strangely here, picked up and thrown back by the room's hard surfaces.", @"One of the clearest local details is that sound carries strangely here, picked up and thrown back by the room's hard surfaces.", @"The eye is drawn to the fact that sound carries strangely here, picked up and thrown back by the room's hard surfaces.", @"Sound carries strangely here, picked up and thrown back by the room's hard surfaces, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Echoing Walls"] = new WildernessGroupedTerrainFeatureSpec(
                @"Echoing Walls",
                @"Rock Feature",
                new[] { @"Nearby stone throws sound back in a way that makes the space feel tighter than its size alone.", @"At a glance, nearby stone throws sound back in a way that makes the space feel tighter than its size alone.", @"One of the clearest local details is that nearby stone throws sound back in a way that makes the space feel tighter than its size alone.", @"The eye is drawn to the fact that nearby stone throws sound back in a way that makes the space feel tighter than its size alone.", @"Nearby stone throws sound back in a way that makes the space feel tighter than its size alone, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Encroaching Weeds"] = new WildernessGroupedTerrainFeatureSpec(
                @"Encroaching Weeds",
                @"Urban Feature",
                new[] { @"Tough growth pushes in from neglected edges and cracks, reclaiming space a little at a time.", @"At a glance, tough growth pushes in from neglected edges and cracks, reclaiming space a little at a time.", @"One of the clearest local details is that tough growth pushes in from neglected edges and cracks, reclaiming space a little at a time.", @"The eye is drawn to the fact that tough growth pushes in from neglected edges and cracks, reclaiming space a little at a time.", @"Tough growth pushes in from neglected edges and cracks, reclaiming space a little at a time, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Eroded Gullies"] = new WildernessGroupedTerrainFeatureSpec(
                @"Eroded Gullies",
                @"Open Land Feature",
                new[] { @"Water has bitten small channels into the ground and left their lines plainly visible.", @"At a glance, water has bitten small channels into the ground and left their lines plainly visible.", @"One of the clearest local details is that water has bitten small channels into the ground and left their lines plainly visible.", @"The eye is drawn to the fact that water has bitten small channels into the ground and left their lines plainly visible.", @"Water has bitten small channels into the ground and left their lines plainly visible, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Even Camber"] = new WildernessGroupedTerrainFeatureSpec(
                @"Even Camber",
                @"Road Feature",
                new[] { @"The route has been shaped to shed water and keep a smoother line under travel.", @"At a glance, the route has been shaped to shed water and keep a smoother line under travel.", @"One of the clearest local details is that the route has been shaped to shed water and keep a smoother line under travel.", @"The eye is drawn to the fact that the route has been shaped to shed water and keep a smoother line under travel.", @"The route has been shaped to shed water and keep a smoother line under travel, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Exposed Bedrock"] = new WildernessGroupedTerrainFeatureSpec(
                @"Exposed Bedrock",
                @"Rock Feature",
                new[] { @"The skin of soil has worn thin enough to leave solid stone showing through.", @"At a glance, the skin of soil has worn thin enough to leave solid stone showing through.", @"One of the clearest local details is that the skin of soil has worn thin enough to leave solid stone showing through.", @"The eye is drawn to the fact that the skin of soil has worn thin enough to leave solid stone showing through.", @"The skin of soil has worn thin enough to leave solid stone showing through, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Exposed Roofline"] = new WildernessGroupedTerrainFeatureSpec(
                @"Exposed Roofline",
                @"Urban Feature",
                new[] { @"From here the upper structure of the settlement is easier to read than its street-level detail.", @"At a glance, from here the upper structure of the settlement is easier to read than its street-level detail.", @"One of the clearest local details is that from here the upper structure of the settlement is easier to read than its street-level detail.", @"The eye is drawn to the fact that from here the upper structure of the settlement is easier to read than its street-level detail.", @"From here the upper structure of the settlement is easier to read than its street-level detail, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Fast Current"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fast Current",
                @"Water Feature",
                new[] { @"The water moves with enough speed to announce that crossing or wading is no simple matter.", @"At a glance, the water moves with enough speed to announce that crossing or wading is no simple matter.", @"One of the clearest local details is that the water moves with enough speed to announce that crossing or wading is no simple matter.", @"The eye is drawn to the fact that the water moves with enough speed to announce that crossing or wading is no simple matter.", @"The water moves with enough speed to announce that crossing or wading is no simple matter, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Fence Lines"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fence Lines",
                @"Urban Feature",
                new[] { @"Boundaries are marked more by low division and habit than by heavy barriers.", @"At a glance, boundaries are marked more by low division and habit than by heavy barriers.", @"One of the clearest local details is that boundaries are marked more by low division and habit than by heavy barriers.", @"The eye is drawn to the fact that boundaries are marked more by low division and habit than by heavy barriers.", @"Boundaries are marked more by low division and habit than by heavy barriers, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Ferny Floor"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ferny Floor",
                @"Forest Feature",
                new[] { @"Ferns and similar low growth give the forest floor a softer, layered look.", @"At a glance, ferns and similar low growth give the forest floor a softer, layered look.", @"One of the clearest local details is that ferns and similar low growth give the forest floor a softer, layered look.", @"The eye is drawn to the fact that ferns and similar low growth give the forest floor a softer, layered look.", @"Ferns and similar low growth give the forest floor a softer, layered look, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Field Birds"] = new WildernessGroupedTerrainFeatureSpec(
                @"Field Birds",
                @"Open Land Sound",
                new[] { @"environment{dawn=Small field birds are at their boldest at dawn, casting sharp calls out across the open ground.}{day=Birdsong rises and falls in short, bright bursts from the grass and low branches.}{rain=Rain reduces the birds to only the occasional stubborn note.}{night=The daytime birds have gone mostly quiet, leaving only the odd sleepy flutter or warning call.}{Field birds trade short calls across the open ground.}", @"environment{spring=The birds sound busier and more territorial in the fresh season.}{autumn=The calls are thinner and more scattered now, as though the birds have better things to do elsewhere.}{day=Quick birdsong carries clearly in the open air.}{Scattered bird calls travel a long way in the exposed space.}", @"environment{morning=Morning fills the place with brief, overlapping birdsong.}{afternoon=By afternoon the calls are fewer, but they still travel cleanly over the open land.}{night=Birdsong gives way to long patches of quiet.}{The open country gives every bird call room to carry.}", @"environment{day=The silence never quite settles while little birds keep piping from the grass or hedging.}{rain=Wet weather quiets the smaller singers almost at once.}{Field birds keep the air from ever feeling completely still.}" },
                80.0,
                false),
            [@"Fish Shoal"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fish Shoal",
                @"Aquatic Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Flower Beds"] = new WildernessGroupedTerrainFeatureSpec(
                @"Flower Beds",
                @"Urban Feature",
                new[] { @"Deliberate planting adds colour and care to ground that might otherwise seem merely functional.", @"At a glance, deliberate planting adds colour and care to ground that might otherwise seem merely functional.", @"One of the clearest local details is that deliberate planting adds colour and care to ground that might otherwise seem merely functional.", @"The eye is drawn to the fact that deliberate planting adds colour and care to ground that might otherwise seem merely functional.", @"Deliberate planting adds colour and care to ground that might otherwise seem merely functional, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Foam Line"] = new WildernessGroupedTerrainFeatureSpec(
                @"Foam Line",
                @"Water Feature",
                new[] { @"A line of foam marks where recent water movement has repeatedly broken and gathered.", @"At a glance, a line of foam marks where recent water movement has repeatedly broken and gathered.", @"One of the clearest local details is that a line of foam marks where recent water movement has repeatedly broken and gathered.", @"The eye is drawn to the fact that a line of foam marks where recent water movement has repeatedly broken and gathered.", @"A line of foam marks where recent water movement has repeatedly broken and gathered, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Foot Traffic Wear"] = new WildernessGroupedTerrainFeatureSpec(
                @"Foot Traffic Wear",
                @"Urban Feature",
                new[] { @"The busiest route through the area is written plainly into the floor by repeated passage.", @"At a glance, the busiest route through the area is written plainly into the floor by repeated passage.", @"One of the clearest local details is that the busiest route through the area is written plainly into the floor by repeated passage.", @"The eye is drawn to the fact that the busiest route through the area is written plainly into the floor by repeated passage.", @"The busiest route through the area is written plainly into the floor by repeated passage, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Formal Planting"] = new WildernessGroupedTerrainFeatureSpec(
                @"Formal Planting",
                @"Urban Feature",
                new[] { @"Planting has been shaped and kept for appearance as much as growth.", @"At a glance, planting has been shaped and kept for appearance as much as growth.", @"One of the clearest local details is that planting has been shaped and kept for appearance as much as growth.", @"The eye is drawn to the fact that planting has been shaped and kept for appearance as much as growth.", @"Planting has been shaped and kept for appearance as much as growth, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Fresh Drift"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fresh Drift",
                @"Glacial Feature",
                new[] { @"A fresher drift lies softer and less settled than the surrounding snow cover.", @"At a glance, a fresher drift lies softer and less settled than the surrounding snow cover.", @"One of the clearest local details is that a fresher drift lies softer and less settled than the surrounding snow cover.", @"The eye is drawn to the fact that a fresher drift lies softer and less settled than the surrounding snow cover.", @"A fresher drift lies softer and less settled than the surrounding snow cover, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Freshwater Spring"] = new WildernessGroupedTerrainFeatureSpec(
                @"Freshwater Spring",
                @"Hydrological Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Frog Chorus"] = new WildernessGroupedTerrainFeatureSpec(
                @"Frog Chorus",
                @"Wetland Sound",
                new[] { @"environment{dusk=Frogs begin calling in earnest as the light goes down, filling the wet ground with pulses and croaks.}{night=After dark the frogs keep up a loud, uneven chorus from every wetter hollow.}{day=The frogs are quieter by day, but the occasional croak still carries.}{rain=Rain wakes more voices from the mud and reeds at once.}{Frogs call from the wetter ground in uneven chorus.}", @"environment{spring=The fresh season brings a busier and more competitive frog chorus.}{summer=Warmth thickens the frog-noise until it becomes part of the wetland's fabric.}{night=The darker hours belong increasingly to the frogs.}{The marsh is never wholly free of frog-song.}", @"environment{dawn=The last of the night's croaking lingers well into first light.}{night=By night the sound of frogs spreads far wider than the patches of open water suggest.}{Frog calls roll out from the reeds and mud.}", @"environment{rain=Rain seems to give the frogs permission to answer one another more loudly.}{day=Only scattered croaks puncture the insect noise by day.}{night=At night the frogs take over much of the soundscape.}{Wet ground nearby shelters a noisy population of frogs.}" },
                80.0,
                false),
            [@"Frost Crystals"] = new WildernessGroupedTerrainFeatureSpec(
                @"Frost Crystals",
                @"Glacial Feature",
                new[] { @"Fine crystals catch what light there is and give the surface an almost granular brightness.", @"At a glance, fine crystals catch what light there is and give the surface an almost granular brightness.", @"One of the clearest local details is that fine crystals catch what light there is and give the surface an almost granular brightness.", @"The eye is drawn to the fact that fine crystals catch what light there is and give the surface an almost granular brightness.", @"Fine crystals catch what light there is and give the surface an almost granular brightness, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Frost-Hardened Ground"] = new WildernessGroupedTerrainFeatureSpec(
                @"Frost-Hardened Ground",
                @"Open Land Feature",
                new[] { @"Cold has tightened the upper ground into something firmer and more brittle than usual.", @"At a glance, cold has tightened the upper ground into something firmer and more brittle than usual.", @"One of the clearest local details is that cold has tightened the upper ground into something firmer and more brittle than usual.", @"The eye is drawn to the fact that cold has tightened the upper ground into something firmer and more brittle than usual.", @"Cold has tightened the upper ground into something firmer and more brittle than usual, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Fruit Grove"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fruit Grove",
                @"Botanical Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Fruiting Trees"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fruiting Trees",
                @"Forest Feature",
                new[] { @"At least some of the nearby growth is valued as much for produce as for shade or timber.", @"At a glance, at least some of the nearby growth is valued as much for produce as for shade or timber.", @"One of the clearest local details is that at least some of the nearby growth is valued as much for produce as for shade or timber.", @"The eye is drawn to the fact that at least some of the nearby growth is valued as much for produce as for shade or timber.", @"At least some of the nearby growth is valued as much for produce as for shade or timber, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Fumarole Stain"] = new WildernessGroupedTerrainFeatureSpec(
                @"Fumarole Stain",
                @"Volcanic Feature",
                new[] { @"Mineral staining marks where hot gas or venting has once had its say.", @"At a glance, mineral staining marks where hot gas or venting has once had its say.", @"One of the clearest local details is that mineral staining marks where hot gas or venting has once had its say.", @"The eye is drawn to the fact that mineral staining marks where hot gas or venting has once had its say.", @"Mineral staining marks where hot gas or venting has once had its say, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Galactic Haze"] = new WildernessGroupedTerrainFeatureSpec(
                @"Galactic Haze",
                @"Extraterrestrial Feature",
                new[] { @"A faint galactic haze smears part of the dark with distant unresolved light.", @"At a glance, a faint galactic haze smears part of the dark with distant unresolved light.", @"One of the clearest local details is that a faint galactic haze smears part of the dark with distant unresolved light.", @"The eye is drawn to the fact that a faint galactic haze smears part of the dark with distant unresolved light.", @"A faint galactic haze smears part of the dark with distant unresolved light, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Gate Traffic"] = new WildernessGroupedTerrainFeatureSpec(
                @"Gate Traffic",
                @"Urban Feature",
                new[] { @"The space carries the sense of controlled entry, passage, and scrutiny.", @"At a glance, the space carries the sense of controlled entry, passage, and scrutiny.", @"One of the clearest local details is that the space carries the sense of controlled entry, passage, and scrutiny.", @"The eye is drawn to the fact that the space carries the sense of controlled entry, passage, and scrutiny.", @"The space carries the sense of controlled entry, passage, and scrutiny, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Gentle Lapping"] = new WildernessGroupedTerrainFeatureSpec(
                @"Gentle Lapping",
                @"Water Feature",
                new[] { @"The nearby water moves with an easy repeated edge rather than any stronger force.", @"At a glance, the nearby water moves with an easy repeated edge rather than any stronger force.", @"One of the clearest local details is that the nearby water moves with an easy repeated edge rather than any stronger force.", @"The eye is drawn to the fact that the nearby water moves with an easy repeated edge rather than any stronger force.", @"The nearby water moves with an easy repeated edge rather than any stronger force, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Giant Trunks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Giant Trunks",
                @"Forest Feature",
                new[] { @"Some of the trunks here carry enough size to dominate the eye even among other growth.", @"At a glance, some of the trunks here carry enough size to dominate the eye even among other growth.", @"One of the clearest local details is that some of the trunks here carry enough size to dominate the eye even among other growth.", @"The eye is drawn to the fact that some of the trunks here carry enough size to dominate the eye even among other growth.", @"Some of the trunks here carry enough size to dominate the eye even among other growth, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Glassy Impact Fragments"] = new WildernessGroupedTerrainFeatureSpec(
                @"Glassy Impact Fragments",
                @"Extraterrestrial Feature",
                new[] { @"Scattered impact-made glass or vitrified fragments catch the light with unnatural sharpness.", @"At a glance, scattered impact-made glass or vitrified fragments catch the light with unnatural sharpness.", @"One of the clearest local details is that scattered impact-made glass or vitrified fragments catch the light with unnatural sharpness.", @"The eye is drawn to the fact that scattered impact-made glass or vitrified fragments catch the light with unnatural sharpness.", @"Scattered impact-made glass or vitrified fragments catch the light with unnatural sharpness, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Glassy Surface"] = new WildernessGroupedTerrainFeatureSpec(
                @"Glassy Surface",
                @"Water Feature",
                new[] { @"For the moment, the water has settled smooth enough to reflect rather than break.", @"At a glance, for the moment, the water has settled smooth enough to reflect rather than break.", @"One of the clearest local details is that for the moment, the water has settled smooth enough to reflect rather than break.", @"The eye is drawn to the fact that for the moment, the water has settled smooth enough to reflect rather than break.", @"For the moment, the water has settled smooth enough to reflect rather than break, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Grasshopper Chirr"] = new WildernessGroupedTerrainFeatureSpec(
                @"Grasshopper Chirr",
                @"Open Land Sound",
                new[] { @"environment{day=Grasshoppers keep up a dry, mechanical chirr from the cover.}{night=The insect noise sharpens after dark into a more continuous rasp.}{rain=Rain knocks the smaller insect noises down to almost nothing.}{Insects make a thin, dry music in the grass.}", @"environment{afternoon=Heat draws a constant insect trill from the grass.}{dusk=As dusk settles, the chirring grows steadier and less distinct.}{night=The hidden insects seem louder when the light is gone.}{A fine thread of insect noise hangs over the grass.}", @"environment{summer=Warm conditions wake the grass-dwelling insects into near-constant sound.}{spring=The first stronger warmth coaxes a tentative insect chorus from the cover.}{rain=The insect noise gutters out beneath the rain.}{The grass hides a busy, rasping insect life.}", @"environment{day=The sound comes in short chirrs and rubbing pulses from below knee height.}{night=After dark the chirring becomes one of the place's more constant sounds.}{The insects here are much easier to hear than to see.}" },
                80.0,
                false),
            [@"Green Fringe"] = new WildernessGroupedTerrainFeatureSpec(
                @"Green Fringe",
                @"Desert Feature",
                new[] { @"A ring of healthier growth marks where moisture lingers longer than the surrounding land would suggest.", @"At a glance, a ring of healthier growth marks where moisture lingers longer than the surrounding land would suggest.", @"One of the clearest local details is that a ring of healthier growth marks where moisture lingers longer than the surrounding land would suggest.", @"The eye is drawn to the fact that a ring of healthier growth marks where moisture lingers longer than the surrounding land would suggest.", @"A ring of healthier growth marks where moisture lingers longer than the surrounding land would suggest, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Hanging Vines"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hanging Vines",
                @"Forest Feature",
                new[] { @"Vines or trailing growth break clean lines and make the surrounding vegetation feel more entangled.", @"At a glance, vines or trailing growth break clean lines and make the surrounding vegetation feel more entangled.", @"One of the clearest local details is that vines or trailing growth break clean lines and make the surrounding vegetation feel more entangled.", @"The eye is drawn to the fact that vines or trailing growth break clean lines and make the surrounding vegetation feel more entangled.", @"Vines or trailing growth break clean lines and make the surrounding vegetation feel more entangled, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Hard Vacuum Silence"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hard Vacuum Silence",
                @"Extraterrestrial Feature",
                new[] { @"The lack of atmosphere is felt here as a stark visual stillness rather than any comforting calm.", @"At a glance, the lack of atmosphere is felt here as a stark visual stillness rather than any comforting calm.", @"One of the clearest local details is that the lack of atmosphere is felt here as a stark visual stillness rather than any comforting calm.", @"The eye is drawn to the fact that the lack of atmosphere is felt here as a stark visual stillness rather than any comforting calm.", @"The lack of atmosphere is felt here as a stark visual stillness rather than any comforting calm, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Hard-Baked Soil"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hard-Baked Soil",
                @"Open Land Feature",
                new[] { @"Where vegetation thins, the exposed soil shows a firmer and more sun-worked surface.", @"At a glance, where vegetation thins, the exposed soil shows a firmer and more sun-worked surface.", @"One of the clearest local details is that where vegetation thins, the exposed soil shows a firmer and more sun-worked surface.", @"The eye is drawn to the fact that where vegetation thins, the exposed soil shows a firmer and more sun-worked surface.", @"Where vegetation thins, the exposed soil shows a firmer and more sun-worked surface, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Hard-Worn Ground"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hard-Worn Ground",
                @"Urban Feature",
                new[] { @"The ground has been beaten firm by repeated gathering, movement, or event use.", @"At a glance, the ground has been beaten firm by repeated gathering, movement, or event use.", @"One of the clearest local details is that the ground has been beaten firm by repeated gathering, movement, or event use.", @"The eye is drawn to the fact that the ground has been beaten firm by repeated gathering, movement, or event use.", @"The ground has been beaten firm by repeated gathering, movement, or event use, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Heat Haze"] = new WildernessGroupedTerrainFeatureSpec(
                @"Heat Haze",
                @"Volcanic Feature",
                new[] { @"Subtle wavering in the air hints at stored or rising heat nearby.", @"At a glance, subtle wavering in the air hints at stored or rising heat nearby.", @"One of the clearest local details is that subtle wavering in the air hints at stored or rising heat nearby.", @"The eye is drawn to the fact that subtle wavering in the air hints at stored or rising heat nearby.", @"Subtle wavering in the air hints at stored or rising heat nearby, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Heat Silence"] = new WildernessGroupedTerrainFeatureSpec(
                @"Heat Silence",
                @"Desert Sound",
                new[] { @"environment{day=In the harsher heat, silence seems to spread wider between the smaller sounds.}{afternoon=The hottest part of the day leaves the landscape feeling almost sound-struck.}{night=Night finally releases the country from that baked stillness.}{There is a kind of heat-made silence here that swallows lesser noise.}", @"environment{day=The land grows so still in the heat that every small sound feels isolated.}{rain=Rain breaks the heat-silence at once.}{The arid air often keeps the place unnaturally quiet.}", @"environment{afternoon=The afternoon heat presses much of the usual life into silence.}{night=Once the sun is gone, the silence loosens its grip.}{The silence here feels imposed rather than accidental.}", @"environment{day=At times the loudest thing here is the absence of anything louder.}{rain=Wind and weather can break that stillness, but not often.}{Heat leaves the country quieter than comfort would prefer.}" },
                80.0,
                false),
            [@"Heat-Shattered Rock"] = new WildernessGroupedTerrainFeatureSpec(
                @"Heat-Shattered Rock",
                @"Desert Feature",
                new[] { @"Repeated heat has split some of the rock into sharp angular fragments.", @"At a glance, repeated heat has split some of the rock into sharp angular fragments.", @"One of the clearest local details is that repeated heat has split some of the rock into sharp angular fragments.", @"The eye is drawn to the fact that repeated heat has split some of the rock into sharp angular fragments.", @"Repeated heat has split some of the rock into sharp angular fragments, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Herb Patch"] = new WildernessGroupedTerrainFeatureSpec(
                @"Herb Patch",
                @"Botanical Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"High Rim"] = new WildernessGroupedTerrainFeatureSpec(
                @"High Rim",
                @"Rock Feature",
                new[] { @"A raised edge or rim dominates part of the view and suggests stronger relief nearby.", @"At a glance, a raised edge or rim dominates part of the view and suggests stronger relief nearby.", @"One of the clearest local details is that a raised edge or rim dominates part of the view and suggests stronger relief nearby.", @"The eye is drawn to the fact that a raised edge or rim dominates part of the view and suggests stronger relief nearby.", @"A raised edge or rim dominates part of the view and suggests stronger relief nearby, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Hollow Quiet"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hollow Quiet",
                @"Cave Sound",
                new[] { @"environment{day=The cave holds a broad, hollow quiet that makes any small noise seem deliberate.}{night=At night the surrounding quiet feels even more complete and structural.}{rain=Rain outside turns the silence into something intermittently broken rather than absent.}{There is a hollow quiet here particular to enclosed stone.}", @"environment{day=Even in stillness, the quiet here feels shaped by stone and distance.}{night=Darkness deepens that hollow silence rather than changing it.}{The cave keeps a more resonant quiet than the surface world.}", @"environment{night=The silence waits in the dark like another physical feature of the cave.}{day=In light, the same quiet only seems to define the chamber more clearly.}{Stone and enclosure make this place quieter than comfort prefers.}", @"environment{day=It is not simple silence but a silence with room in it.}{rain=Outside weather only occasionally intrudes on that larger hush.}{The cave's quiet has depth to it.}" },
                80.0,
                false),
            [@"Horizon Blur"] = new WildernessGroupedTerrainFeatureSpec(
                @"Horizon Blur",
                @"Water Feature",
                new[] { @"The far line between water and sky softens until it seems almost imagined.", @"At a glance, the far line between water and sky softens until it seems almost imagined.", @"One of the clearest local details is that the far line between water and sky softens until it seems almost imagined.", @"The eye is drawn to the fact that the far line between water and sky softens until it seems almost imagined.", @"The far line between water and sky softens until it seems almost imagined, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Hot Dust Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Hot Dust Smell",
                @"Desert Smell",
                new[] { @"environment{day=The air smells of hot dust and sun-worked stone.}{rain=Rain briefly replaces the dust-smell with darker earth.}{night=After dark the dust smell cools, but does not vanish.}{There is a hot dusty smell to the place.}", @"environment{afternoon=The harsher heat bakes a powdery smell out of the ground.}{night=Night keeps the dust but strips away some of the heat from it.}{The land smells dry enough to taste.}", @"environment{dry=Dry weather leaves the air thin with dust and mineral grit.}{rain=Wind lifts the dust-smell into something sharper and more abrasive.}{The air carries a clear scent of heat and powder.}", @"environment{day=Sun on bare ground draws out the smell of dust, grit, and old heat.}{night=The smell lingers after the light has gone.}{The country smells baked.}" },
                75.0,
                false),
            [@"Humid Rot Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Humid Rot Smell",
                @"Forest Smell",
                new[] { @"environment{humid=The air is thick with the warm smell of wet growth and quick decay.}{rain=Rain deepens the smell of green rot almost at once.}{night=At night that damp organic smell seems to settle around everything.}{There is a warm, humid smell of growth and rot here.}", @"environment{day=Moist heat makes every living and dying thing smell stronger.}{spring=Fresh growth only layers itself atop the older scent of rot.}{The air smells rich, damp, and half-rotten.}", @"environment{humid=Humidity makes the forest smell almost edible in its richness.}{dry=Even drier spells never quite clear the scent of damp decay.}{This is the smell of a place where growth outruns cleanliness.}", @"environment{day=The nose catches wet leaf, bark, loam, and the beginning of rot all together.}{rain=Rain gathers those smells into something close and insistent.}{The forest smells busy with decomposition.}" },
                75.0,
                false),
            [@"Ice Block"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ice Block",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Ice Creak"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ice Creak",
                @"Glacial Sound",
                new[] { @"environment{day=Ice and hard-packed snow answer stress and temperature with the occasional dry creak.}{night=At night the smaller creaks of ice seem to come from farther away.}{spring=Thaw lends the ice a busier language of creaks and small shifts.}{There are intermittent creaks from ice or hard snow nearby.}", @"environment{winter=The frozen ground speaks in small crackles and creaks under strain.}{day=Clear cold makes the ice sound brittle and articulate.}{The ice is not truly silent, only sparing in its speech.}", @"environment{morning=Morning cold can coax a few sharp sounds out of ice and crusted snow.}{afternoon=The warmer part of the day leaves the frozen surfaces complaining more softly.}{The frozen terrain gives off the occasional creak of adjustment.}", @"environment{night=In darkness, each small creak seems to belong to a larger frozen body beyond sight.}{spring=Melt and refreeze complicate the ice's old language.}{The place is accompanied by the quiet report of working ice.}" },
                80.0,
                false),
            [@"Insect Hum"] = new WildernessGroupedTerrainFeatureSpec(
                @"Insect Hum",
                @"Open Land Feature",
                new[] { @"Small life makes itself known here as a constant undertone rather than a visible presence.", @"At a glance, small life makes itself known here as a constant undertone rather than a visible presence.", @"One of the clearest local details is that small life makes itself known here as a constant undertone rather than a visible presence.", @"The eye is drawn to the fact that small life makes itself known here as a constant undertone rather than a visible presence.", @"Small life makes itself known here as a constant undertone rather than a visible presence, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Iron Fittings"] = new WildernessGroupedTerrainFeatureSpec(
                @"Iron Fittings",
                @"Urban Feature",
                new[] { @"Heavy fastenings and sturdy fixtures give the place a harder, less forgiving character.", @"At a glance, heavy fastenings and sturdy fixtures give the place a harder, less forgiving character.", @"One of the clearest local details is that heavy fastenings and sturdy fixtures give the place a harder, less forgiving character.", @"The eye is drawn to the fact that heavy fastenings and sturdy fixtures give the place a harder, less forgiving character.", @"Heavy fastenings and sturdy fixtures give the place a harder, less forgiving character, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Jagged Rubble"] = new WildernessGroupedTerrainFeatureSpec(
                @"Jagged Rubble",
                @"Extraterrestrial Feature",
                new[] { @"Broken rock lies in sharp irregular fragments with little to soften it.", @"At a glance, broken rock lies in sharp irregular fragments with little to soften it.", @"One of the clearest local details is that broken rock lies in sharp irregular fragments with little to soften it.", @"The eye is drawn to the fact that broken rock lies in sharp irregular fragments with little to soften it.", @"Broken rock lies in sharp irregular fragments with little to soften it, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Jagged Stalactites"] = new WildernessGroupedTerrainFeatureSpec(
                @"Jagged Stalactites",
                @"Cave Feature",
                new[] { @"The ceiling carries hanging stone sharp enough to keep the eye lifting warily.", @"At a glance, the ceiling carries hanging stone sharp enough to keep the eye lifting warily.", @"One of the clearest local details is that the ceiling carries hanging stone sharp enough to keep the eye lifting warily.", @"The eye is drawn to the fact that the ceiling carries hanging stone sharp enough to keep the eye lifting warily.", @"The ceiling carries hanging stone sharp enough to keep the eye lifting warily, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Kelp Tang"] = new WildernessGroupedTerrainFeatureSpec(
                @"Kelp Tang",
                @"Water Smell",
                new[] { @"environment{day=There is a marine tang here of kelp, weed, and things long wet.}{rain=Rain freshens the weed-smell but does not remove it.}{night=At night the kelp-tang sits closer to the edge of every breath.}{A kelpy marine smell lingers around the margin.}", @"environment{summer=Warmth makes the weed-scent stronger and a little ranker.}{winter=Cooler air leaves it cleaner, though still unmistakably marine.}{The shore smells of stranded weed and salt-wet growth.}", @"environment{day=The air carries the vegetable tang of seaweed drying, soaking, and drying again.}{torrential=Rougher weather renews the stronger weed-smell from the tideline.}{There is a seaweed note to the place that salt alone cannot explain.}", @"environment{morning=Morning damp keeps the kelp-smell soft but present.}{afternoon=Sun-warmed wrack makes the marine tang heavier.}{Kelp and wrack give the air a more complicated marine smell.}" },
                75.0,
                false),
            [@"Kelp Wrack"] = new WildernessGroupedTerrainFeatureSpec(
                @"Kelp Wrack",
                @"Water Feature",
                new[] { @"Cast-off weed and marine growth have been left behind above the current waterline.", @"At a glance, cast-off weed and marine growth have been left behind above the current waterline.", @"One of the clearest local details is that cast-off weed and marine growth have been left behind above the current waterline.", @"The eye is drawn to the fact that cast-off weed and marine growth have been left behind above the current waterline.", @"Cast-off weed and marine growth have been left behind above the current waterline, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Lake Lapping"] = new WildernessGroupedTerrainFeatureSpec(
                @"Lake Lapping",
                @"Water Sound",
                new[] { @"environment{day=Small waves or stirred water lap softly at the margin.}{night=At night the lake is mostly known by the quiet lapping at its edge.}{rain=Rain blurs the gentler lapping into a busier patter and slap.}{The lake answers the shore in small lapping sounds.}", @"environment{morning=Morning air lets each soft lap carry a little farther than expected.}{afternoon=The water continues its light, repetitive conversation with the shore.}{night=The quieter lapping grows more distinct in darkness.}{Held water gives the edge a softer voice than a river would.}", @"environment{autumn=Cooler weather makes the small sounds of the water seem barer and cleaner.}{spring=Fresh growth around the edge does little to quiet the patient lapping.}{The water meets the margin with a repeated, modest slap.}", @"environment{day=Nothing in the lake's sound suggests hurry.}{night=The soft edge-noise keeps the dark from going wholly silent.}{The water here sounds patient rather than forceful.}" },
                80.0,
                false),
            [@"Laundry Lines"] = new WildernessGroupedTerrainFeatureSpec(
                @"Laundry Lines",
                @"Urban Feature",
                new[] { @"Suspended washing or domestic line-work adds a lived-in disorder overhead.", @"At a glance, suspended washing or domestic line-work adds a lived-in disorder overhead.", @"One of the clearest local details is that suspended washing or domestic line-work adds a lived-in disorder overhead.", @"The eye is drawn to the fact that suspended washing or domestic line-work adds a lived-in disorder overhead.", @"Suspended washing or domestic line-work adds a lived-in disorder overhead, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Layered Rock"] = new WildernessGroupedTerrainFeatureSpec(
                @"Layered Rock",
                @"Rock Feature",
                new[] { @"The exposed stone shows distinct layers, making the land look built up rather than simply broken.", @"At a glance, the exposed stone shows distinct layers, making the land look built up rather than simply broken.", @"One of the clearest local details is that the exposed stone shows distinct layers, making the land look built up rather than simply broken.", @"The eye is drawn to the fact that the exposed stone shows distinct layers, making the land look built up rather than simply broken.", @"The exposed stone shows distinct layers, making the land look built up rather than simply broken, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Leaf Mold Scent"] = new WildernessGroupedTerrainFeatureSpec(
                @"Leaf Mold Scent",
                @"Forest Smell",
                new[] { @"environment{day=The forest smells of leaf mold, damp loam, and slow decay.}{rain=Rain turns that dark organic smell richer and more immediate.}{night=At night the scent of rot and loam seems to settle lower and heavier.}{There is a strong smell of leaf mold and forest loam here.}", @"environment{autumn=Fresh leaf-fall thickens the scent of vegetal decay underfoot.}{spring=New growth sits atop the older smell of the forest floor without replacing it.}{The ground gives off the dark smell of leaves returning to soil.}", @"environment{humid=Humidity makes the mold-scent cling to the air more obviously.}{dry=Dry weather leaves it lighter, though never absent.}{The air is marked by the scent of old leaves and damp earth.}", @"environment{day=Every disturbed patch of litter threatens to release more of that rich, rotting smell.}{rain=Moisture wakes the forest floor into scent at once.}{The woodland floor speaks plainly to the nose.}" },
                75.0,
                false),
            [@"Lichen Mat"] = new WildernessGroupedTerrainFeatureSpec(
                @"Lichen Mat",
                @"Open Land Feature",
                new[] { @"Crusts of lichen spread across the more stable surfaces, adding muted colour and fine texture.", @"At a glance, crusts of lichen spread across the more stable surfaces, adding muted colour and fine texture.", @"One of the clearest local details is that crusts of lichen spread across the more stable surfaces, adding muted colour and fine texture.", @"The eye is drawn to the fact that crusts of lichen spread across the more stable surfaces, adding muted colour and fine texture.", @"Crusts of lichen spread across the more stable surfaces, adding muted colour and fine texture, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Lingering Cooking Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Lingering Cooking Smell",
                @"Urban Feature",
                new[] { @"A faint domestic smell lingers here, the kind that settles into a place after repeated meals.", @"At a glance, a faint domestic smell lingers here, the kind that settles into a place after repeated meals.", @"One of the clearest local details is that a faint domestic smell lingers here, the kind that settles into a place after repeated meals.", @"The eye is drawn to the fact that a faint domestic smell lingers here, the kind that settles into a place after repeated meals.", @"A faint domestic smell lingers here, the kind that settles into a place after repeated meals, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Loading Space"] = new WildernessGroupedTerrainFeatureSpec(
                @"Loading Space",
                @"Urban Feature",
                new[] { @"The room has enough clear working space to make movement of goods easier than comfort would require.", @"At a glance, the room has enough clear working space to make movement of goods easier than comfort would require.", @"One of the clearest local details is that the room has enough clear working space to make movement of goods easier than comfort would require.", @"The eye is drawn to the fact that the room has enough clear working space to make movement of goods easier than comfort would require.", @"The room has enough clear working space to make movement of goods easier than comfort would require, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Long Corridor Sightline"] = new WildernessGroupedTerrainFeatureSpec(
                @"Long Corridor Sightline",
                @"Urban Feature",
                new[] { @"The length of the space draws the eye onward, making distance itself part of the room's character.", @"At a glance, the length of the space draws the eye onward, making distance itself part of the room's character.", @"One of the clearest local details is that the length of the space draws the eye onward, making distance itself part of the room's character.", @"The eye is drawn to the fact that the length of the space draws the eye onward, making distance itself part of the room's character.", @"The length of the space draws the eye onward, making distance itself part of the room's character, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Long Swell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Long Swell",
                @"Water Feature",
                new[] { @"Long, patient swells lift and lower the water with a rhythm too broad to ignore.", @"At a glance, long, patient swells lift and lower the water with a rhythm too broad to ignore.", @"One of the clearest local details is that long, patient swells lift and lower the water with a rhythm too broad to ignore.", @"The eye is drawn to the fact that long, patient swells lift and lower the water with a rhythm too broad to ignore.", @"Long, patient swells lift and lower the water with a rhythm too broad to ignore, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Loose Gravel Scatter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Loose Gravel Scatter",
                @"Road Feature",
                new[] { @"Loose stone shifts and crunches underfoot, refusing to settle into a seamless surface.", @"At a glance, loose stone shifts and crunches underfoot, refusing to settle into a seamless surface.", @"One of the clearest local details is that loose stone shifts and crunches underfoot, refusing to settle into a seamless surface.", @"The eye is drawn to the fact that loose stone shifts and crunches underfoot, refusing to settle into a seamless surface.", @"Loose stone shifts and crunches underfoot, refusing to settle into a seamless surface, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Loose Talus"] = new WildernessGroupedTerrainFeatureSpec(
                @"Loose Talus",
                @"Rock Feature",
                new[] { @"Larger fallen stone lies jumbled here, rougher and less uniform than finer scree.", @"At a glance, larger fallen stone lies jumbled here, rougher and less uniform than finer scree.", @"One of the clearest local details is that larger fallen stone lies jumbled here, rougher and less uniform than finer scree.", @"The eye is drawn to the fact that larger fallen stone lies jumbled here, rougher and less uniform than finer scree.", @"Larger fallen stone lies jumbled here, rougher and less uniform than finer scree, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Low Shrub"] = new WildernessGroupedTerrainFeatureSpec(
                @"Low Shrub",
                @"Open Land Feature",
                new[] { @"Shrub growth sits low to the ground, breaking lines of sight without becoming true cover.", @"At a glance, shrub growth sits low to the ground, breaking lines of sight without becoming true cover.", @"One of the clearest local details is that shrub growth sits low to the ground, breaking lines of sight without becoming true cover.", @"The eye is drawn to the fact that shrub growth sits low to the ground, breaking lines of sight without becoming true cover.", @"Shrub growth sits low to the ground, breaking lines of sight without becoming true cover, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Machine Residue"] = new WildernessGroupedTerrainFeatureSpec(
                @"Machine Residue",
                @"Urban Feature",
                new[] { @"Grease, soot, or ingrained dust hints at harder and less delicate labour.", @"At a glance, grease, soot, or ingrained dust hints at harder and less delicate labour.", @"One of the clearest local details is that grease, soot, or ingrained dust hints at harder and less delicate labour.", @"The eye is drawn to the fact that grease, soot, or ingrained dust hints at harder and less delicate labour.", @"Grease, soot, or ingrained dust hints at harder and less delicate labour, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Managed Copse"] = new WildernessGroupedTerrainFeatureSpec(
                @"Managed Copse",
                @"Forest Feature",
                new[] { @"The wood shows signs of having been managed, thinned, or otherwise guided by deliberate hands.", @"At a glance, the wood shows signs of having been managed, thinned, or otherwise guided by deliberate hands.", @"One of the clearest local details is that the wood shows signs of having been managed, thinned, or otherwise guided by deliberate hands.", @"The eye is drawn to the fact that the wood shows signs of having been managed, thinned, or otherwise guided by deliberate hands.", @"The wood shows signs of having been managed, thinned, or otherwise guided by deliberate hands, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Mangrove Roots"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mangrove Roots",
                @"Wetland Feature",
                new[] { @"Rootwork rises visibly from the wet ground and turns open footing into a tangled obstacle.", @"At a glance, rootwork rises visibly from the wet ground and turns open footing into a tangled obstacle.", @"One of the clearest local details is that rootwork rises visibly from the wet ground and turns open footing into a tangled obstacle.", @"The eye is drawn to the fact that rootwork rises visibly from the wet ground and turns open footing into a tangled obstacle.", @"Rootwork rises visibly from the wet ground and turns open footing into a tangled obstacle, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Market Stalls"] = new WildernessGroupedTerrainFeatureSpec(
                @"Market Stalls",
                @"Urban Feature",
                new[] { @"Temporary or semi-permanent stalls suggest that exchange regularly spills out into the open.", @"At a glance, temporary or semi-permanent stalls suggest that exchange regularly spills out into the open.", @"One of the clearest local details is that temporary or semi-permanent stalls suggest that exchange regularly spills out into the open.", @"The eye is drawn to the fact that temporary or semi-permanent stalls suggest that exchange regularly spills out into the open.", @"Temporary or semi-permanent stalls suggest that exchange regularly spills out into the open, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Mineral Damp"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mineral Damp",
                @"Cave Smell",
                new[] { @"environment{day=The cave air smells of mineral damp, cold stone, and long-kept moisture.}{rain=Rain beyond the cave strengthens the damp mineral smell at once.}{night=In darkness the smell of wet stone seems even more complete.}{There is a strong smell of damp mineral stone here.}", @"environment{day=Moist stone and enclosed air lend the place a clear subterranean smell.}{rain=Fresh runoff or seepage sharpens every wet mineral note.}{The cave smells of stone that has held water for a very long time.}", @"environment{night=Cool air keeps the smell of damp stone crisp and unmistakable.}{day=Warmer cave air leaves it broader and more humid.}{The nose confirms the place as underground before the eye finishes the job.}", @"environment{day=It smells of rock, water, and old enclosure.}{rain=Weather outside leaves its trace here mostly by smell.}{The cave air is wet with minerals.}" },
                75.0,
                false),
            [@"Mineral Stain"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mineral Stain",
                @"Cave Feature",
                new[] { @"Mineral deposits have marked the nearby stone in paler runs and patches.", @"At a glance, mineral deposits have marked the nearby stone in paler runs and patches.", @"One of the clearest local details is that mineral deposits have marked the nearby stone in paler runs and patches.", @"The eye is drawn to the fact that mineral deposits have marked the nearby stone in paler runs and patches.", @"Mineral deposits have marked the nearby stone in paler runs and patches, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Mineral Warmth"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mineral Warmth",
                @"Urban Feature",
                new[] { @"Stone and water together lend the room a mineral warmth unlike ordinary indoor air.", @"At a glance, stone and water together lend the room a mineral warmth unlike ordinary indoor air.", @"One of the clearest local details is that stone and water together lend the room a mineral warmth unlike ordinary indoor air.", @"The eye is drawn to the fact that stone and water together lend the room a mineral warmth unlike ordinary indoor air.", @"Stone and water together lend the room a mineral warmth unlike ordinary indoor air, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Mixed Frontages"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mixed Frontages",
                @"Urban Feature",
                new[] { @"Homes, shops, and workshops seem to press together without any single plan dominating.", @"At a glance, homes, shops, and workshops seem to press together without any single plan dominating.", @"One of the clearest local details is that homes, shops, and workshops seem to press together without any single plan dominating.", @"The eye is drawn to the fact that homes, shops, and workshops seem to press together without any single plan dominating.", @"Homes, shops, and workshops seem to press together without any single plan dominating, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Mixed Leaf Litter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mixed Leaf Litter",
                @"Forest Feature",
                new[] { @"The ground is thick with fallen organic litter that softens footfall and hides smaller detail.", @"At a glance, the ground is thick with fallen organic litter that softens footfall and hides smaller detail.", @"One of the clearest local details is that the ground is thick with fallen organic litter that softens footfall and hides smaller detail.", @"The eye is drawn to the fact that the ground is thick with fallen organic litter that softens footfall and hides smaller detail.", @"The ground is thick with fallen organic litter that softens footfall and hides smaller detail, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Mosquito Swarm"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mosquito Swarm",
                @"Wetland Feature",
                new[] { @"Small biting insects find the wet stillness here much to their liking.", @"At a glance, small biting insects find the wet stillness here much to their liking.", @"One of the clearest local details is that small biting insects find the wet stillness here much to their liking.", @"The eye is drawn to the fact that small biting insects find the wet stillness here much to their liking.", @"Small biting insects find the wet stillness here much to their liking, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Mosquito Whine"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mosquito Whine",
                @"Wetland Sound",
                new[] { @"environment{humid=The humid air is alive with the thin, mean whine of biting insects.}{night=After dark the high insect whine feels closer to the ear.}{rain=Steady rain knocks the smaller swarms back for a while.}{Biting insects add a sharp, high whine to the wet air.}", @"environment{day=Mosquitoes and their kin announce themselves in a needle-thin drone.}{dusk=Toward dusk the whining swarms seem to thicken.}{night=The finer insect note grows more oppressive once the light fades.}{The wet stillness here suits biting insects all too well.}", @"environment{summer=Warm weather wakes the mosquitoes into furious activity.}{spring=The first stronger warmth brings the insect swarms back in force.}{rain=Rain scatters the lighter swarms, though never for long.}{A constant insect whine hangs over the place.}", @"environment{day=The sound is small, but too persistent to ignore for long.}{night=In darkness the whining insects seem to orbit the ear itself.}{The marsh's smaller predators mostly announce themselves by sound.}" },
                80.0,
                false),
            [@"Mossed Trunkfall"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mossed Trunkfall",
                @"Forest Feature",
                new[] { @"Fallen timber has remained long enough to take on moss, softness, and a settled place in the landscape.", @"At a glance, fallen timber has remained long enough to take on moss, softness, and a settled place in the landscape.", @"One of the clearest local details is that fallen timber has remained long enough to take on moss, softness, and a settled place in the landscape.", @"The eye is drawn to the fact that fallen timber has remained long enough to take on moss, softness, and a settled place in the landscape.", @"Fallen timber has remained long enough to take on moss, softness, and a settled place in the landscape, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Mud Churn"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mud Churn",
                @"Wetland Feature",
                new[] { @"The mud has been churned by movement, water, or both into a less readable surface.", @"At a glance, the mud has been churned by movement, water, or both into a less readable surface.", @"One of the clearest local details is that the mud has been churned by movement, water, or both into a less readable surface.", @"The eye is drawn to the fact that the mud has been churned by movement, water, or both into a less readable surface.", @"The mud has been churned by movement, water, or both into a less readable surface, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Mud Slick"] = new WildernessGroupedTerrainFeatureSpec(
                @"Mud Slick",
                @"Water Feature",
                new[] { @"Wet mud gives the ground a darker sheen and a less trustworthy surface.", @"At a glance, wet mud gives the ground a darker sheen and a less trustworthy surface.", @"One of the clearest local details is that wet mud gives the ground a darker sheen and a less trustworthy surface.", @"The eye is drawn to the fact that wet mud gives the ground a darker sheen and a less trustworthy surface.", @"Wet mud gives the ground a darker sheen and a less trustworthy surface, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Muted Textiles"] = new WildernessGroupedTerrainFeatureSpec(
                @"Muted Textiles",
                @"Urban Feature",
                new[] { @"Fabric, bedding, or hangings soften the room's harder lines and catch sound before it can carry.", @"At a glance, fabric, bedding, or hangings soften the room's harder lines and catch sound before it can carry.", @"One of the clearest local details is that fabric, bedding, or hangings soften the room's harder lines and catch sound before it can carry.", @"The eye is drawn to the fact that fabric, bedding, or hangings soften the room's harder lines and catch sound before it can carry.", @"Fabric, bedding, or hangings soften the room's harder lines and catch sound before it can carry, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Narrow Defile"] = new WildernessGroupedTerrainFeatureSpec(
                @"Narrow Defile",
                @"Rock Feature",
                new[] { @"The ground constricts into a tighter passage where movement feels channelled rather than free.", @"At a glance, the ground constricts into a tighter passage where movement feels channelled rather than free.", @"One of the clearest local details is that the ground constricts into a tighter passage where movement feels channelled rather than free.", @"The eye is drawn to the fact that the ground constricts into a tighter passage where movement feels channelled rather than free.", @"The ground constricts into a tighter passage where movement feels channelled rather than free, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Narrow Frontages"] = new WildernessGroupedTerrainFeatureSpec(
                @"Narrow Frontages",
                @"Urban Feature",
                new[] { @"Buildings crowd close enough to turn the street into a channel rather than an open way.", @"At a glance, buildings crowd close enough to turn the street into a channel rather than an open way.", @"One of the clearest local details is that buildings crowd close enough to turn the street into a channel rather than an open way.", @"The eye is drawn to the fact that buildings crowd close enough to turn the street into a channel rather than an open way.", @"Buildings crowd close enough to turn the street into a channel rather than an open way, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Narrow Throat"] = new WildernessGroupedTerrainFeatureSpec(
                @"Narrow Throat",
                @"Cave Feature",
                new[] { @"The space pinches down enough in one direction to suggest tighter passages beyond.", @"At a glance, the space pinches down enough in one direction to suggest tighter passages beyond.", @"One of the clearest local details is that the space pinches down enough in one direction to suggest tighter passages beyond.", @"The eye is drawn to the fact that the space pinches down enough in one direction to suggest tighter passages beyond.", @"The space pinches down enough in one direction to suggest tighter passages beyond, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Narrow Tread"] = new WildernessGroupedTerrainFeatureSpec(
                @"Narrow Tread",
                @"Road Feature",
                new[] { @"The usable line through this route is narrower than the space around it would suggest.", @"At a glance, the usable line through this route is narrower than the space around it would suggest.", @"One of the clearest local details is that the usable line through this route is narrower than the space around it would suggest.", @"The eye is drawn to the fact that the usable line through this route is narrower than the space around it would suggest.", @"The usable line through this route is narrower than the space around it would suggest, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Needle Resin Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"Needle Resin Smell",
                @"Forest Smell",
                new[] { @"environment{day=The air carries a clean resinous note from sap, bark, and warmed needles.}{rain=Rain dampens the sharper resin without erasing it.}{night=After dark the resin smell remains, but cooler and less vivid.}{A resinous scent hangs in the air.}", @"environment{summer=Warmth draws a clearer piney sharpness from the trees.}{winter=In colder air the resin smell is fainter, but still distinct.}{The nearby trees lend the place a sharper, cleaner smell than broadleaf woods often do.}", @"environment{day=Sun-warmed bark and sap release a faint, pleasant resin-scent.}{rain=Wet trunks smell greener and less sharply resinous.}{The wood carries something of pitch and clean sap in its scent.}", @"environment{morning=Morning cool gives the resin smell a crisp clarity.}{afternoon=By afternoon the scent grows slightly fuller and warmer.}{The trees season the air with resin.}" },
                75.0,
                false),
            [@"Needle Wind"] = new WildernessGroupedTerrainFeatureSpec(
                @"Needle Wind",
                @"Forest Sound",
                new[] { @"environment{day=Wind passing through needles makes a drier, finer music than broad leaves ever could.}{night=At night the whisper in the conifers sounds thinner and colder.}{rain=Rain turns the sound of wind in the needles heavier and less papery.}{The conifers answer wind with a dry, needled whisper.}", @"environment{morning=Morning wind moves through the needles in soft, high rustling passes.}{afternoon=Stronger gusts set the needled branches hissing together overhead.}{night=The sound becomes almost ghostly once the trees are mostly silhouettes.}{Wind gives the conifers a voice of fine, rasping motion.}", @"environment{winter=Cold wind through the needles sounds sharper and more austere.}{spring=New growth softens the sound only a little.}{The high branches keep up a restrained, resinous whisper whenever the air moves.}", @"environment{day=The trees do not so much rustle as whisper in thin dry sheets.}{rain=Wet needles clatter more softly, but they still speak when the wind freshens.}{The sound of the wind is filtered into something narrower by the conifers.}" },
                80.0,
                false),
            [@"Obsidian Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Obsidian Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Old Meltwater"] = new WildernessGroupedTerrainFeatureSpec(
                @"Old Meltwater",
                @"Glacial Feature",
                new[] { @"Marks of former melt or refreeze complicate what would otherwise be a simpler frozen surface.", @"At a glance, marks of former melt or refreeze complicate what would otherwise be a simpler frozen surface.", @"One of the clearest local details is that marks of former melt or refreeze complicate what would otherwise be a simpler frozen surface.", @"The eye is drawn to the fact that marks of former melt or refreeze complicate what would otherwise be a simpler frozen surface.", @"Marks of former melt or refreeze complicate what would otherwise be a simpler frozen surface, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Open Canopy"] = new WildernessGroupedTerrainFeatureSpec(
                @"Open Canopy",
                @"Forest Feature",
                new[] { @"The trees part often enough overhead to leave broad gaps for sky and light.", @"At a glance, the trees part often enough overhead to leave broad gaps for sky and light.", @"One of the clearest local details is that the trees part often enough overhead to leave broad gaps for sky and light.", @"The eye is drawn to the fact that the trees part often enough overhead to leave broad gaps for sky and light.", @"The trees part often enough overhead to leave broad gaps for sky and light, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Open Fetch"] = new WildernessGroupedTerrainFeatureSpec(
                @"Open Fetch",
                @"Water Feature",
                new[] { @"Wind has enough uninterrupted reach here to work visibly upon the water.", @"At a glance, wind has enough uninterrupted reach here to work visibly upon the water.", @"One of the clearest local details is that wind has enough uninterrupted reach here to work visibly upon the water.", @"The eye is drawn to the fact that wind has enough uninterrupted reach here to work visibly upon the water.", @"Wind has enough uninterrupted reach here to work visibly upon the water, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Open Garden Edge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Open Garden Edge",
                @"Urban Feature",
                new[] { @"Cultivated ground nearby softens the transition between street and private holding.", @"At a glance, cultivated ground nearby softens the transition between street and private holding.", @"One of the clearest local details is that cultivated ground nearby softens the transition between street and private holding.", @"The eye is drawn to the fact that cultivated ground nearby softens the transition between street and private holding.", @"Cultivated ground nearby softens the transition between street and private holding, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Orbital Debris Glint"] = new WildernessGroupedTerrainFeatureSpec(
                @"Orbital Debris Glint",
                @"Extraterrestrial Feature",
                new[] { @"Tiny drifting specks catch the light against the void and hint at wider traffic or ruin.", @"At a glance, tiny drifting specks catch the light against the void and hint at wider traffic or ruin.", @"One of the clearest local details is that tiny drifting specks catch the light against the void and hint at wider traffic or ruin.", @"The eye is drawn to the fact that tiny drifting specks catch the light against the void and hint at wider traffic or ruin.", @"Tiny drifting specks catch the light against the void and hint at wider traffic or ruin, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Ordered Rows"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ordered Rows",
                @"Forest Feature",
                new[] { @"The planting or growth pattern feels more regular than a truly wild wood would allow.", @"At a glance, the planting or growth pattern feels more regular than a truly wild wood would allow.", @"One of the clearest local details is that the planting or growth pattern feels more regular than a truly wild wood would allow.", @"The eye is drawn to the fact that the planting or growth pattern feels more regular than a truly wild wood would allow.", @"The planting or growth pattern feels more regular than a truly wild wood would allow, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Orderly Domestic Arrangement"] = new WildernessGroupedTerrainFeatureSpec(
                @"Orderly Domestic Arrangement",
                @"Urban Feature",
                new[] { @"Daily necessities have been set in a practical order, suggesting habits repeated often enough to become invisible.", @"At a glance, daily necessities have been set in a practical order, suggesting habits repeated often enough to become invisible.", @"One of the clearest local details is that daily necessities have been set in a practical order, suggesting habits repeated often enough to become invisible.", @"The eye is drawn to the fact that daily necessities have been set in a practical order, suggesting habits repeated often enough to become invisible.", @"Daily necessities have been set in a practical order, suggesting habits repeated often enough to become invisible, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Ore Vein"] = new WildernessGroupedTerrainFeatureSpec(
                @"Ore Vein",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Palm Shade"] = new WildernessGroupedTerrainFeatureSpec(
                @"Palm Shade",
                @"Desert Feature",
                new[] { @"Sparse but welcome shade pools beneath hardy growth that can reach hidden water.", @"At a glance, sparse but welcome shade pools beneath hardy growth that can reach hidden water.", @"One of the clearest local details is that sparse but welcome shade pools beneath hardy growth that can reach hidden water.", @"The eye is drawn to the fact that sparse but welcome shade pools beneath hardy growth that can reach hidden water.", @"Sparse but welcome shade pools beneath hardy growth that can reach hidden water, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Paperwork Tidy"] = new WildernessGroupedTerrainFeatureSpec(
                @"Paperwork Tidy",
                @"Urban Feature",
                new[] { @"Work surfaces have the ordered look of a place where record and routine matter.", @"At a glance, work surfaces have the ordered look of a place where record and routine matter.", @"One of the clearest local details is that work surfaces have the ordered look of a place where record and routine matter.", @"The eye is drawn to the fact that work surfaces have the ordered look of a place where record and routine matter.", @"Work surfaces have the ordered look of a place where record and routine matter, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Peat Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Peat Deposit",
                @"Botanical Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Peat Reek"] = new WildernessGroupedTerrainFeatureSpec(
                @"Peat Reek",
                @"Wetland Smell",
                new[] { @"environment{day=There is a peaty reek here, dark, fibrous, and old with trapped water.}{rain=Rain makes the peat-smell fuller and sourer.}{night=At night the peaty scent seems to thicken in the cooler air.}{A dark peaty smell lingers over the ground.}", @"environment{summer=Warm wet weather wakes the peat into a deeper, more stagnant smell.}{spring=Fresh water sharpens the old bog-scent instead of refreshing it.}{The place smells of old waterlogged plant matter.}", @"environment{day=The ground gives off the smell of soaked roots and ancient decay.}{rain=Moisture only confirms the bog's older scent.}{The smell here is more bog than soil.}", @"environment{morning=Cool air makes the peat-scent feel cleaner but no less unmistakable.}{afternoon=Heat fattens the smell into something more oppressive.}{The wet ground smells old, sour, and fibrous.}" },
                75.0,
                false),
            [@"Peaty Ground"] = new WildernessGroupedTerrainFeatureSpec(
                @"Peaty Ground",
                @"Wetland Feature",
                new[] { @"The ground feels dark, fibrous, and old with trapped water and decaying growth.", @"At a glance, the ground feels dark, fibrous, and old with trapped water and decaying growth.", @"One of the clearest local details is that the ground feels dark, fibrous, and old with trapped water and decaying growth.", @"The eye is drawn to the fact that the ground feels dark, fibrous, and old with trapped water and decaying growth.", @"The ground feels dark, fibrous, and old with trapped water and decaying growth, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Pebble Wash"] = new WildernessGroupedTerrainFeatureSpec(
                @"Pebble Wash",
                @"Water Feature",
                new[] { @"Rounded stones lie where water has worked them smooth and sorted them by weight.", @"At a glance, rounded stones lie where water has worked them smooth and sorted them by weight.", @"One of the clearest local details is that rounded stones lie where water has worked them smooth and sorted them by weight.", @"The eye is drawn to the fact that rounded stones lie where water has worked them smooth and sorted them by weight.", @"Rounded stones lie where water has worked them smooth and sorted them by weight, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Pelagic Stillness"] = new WildernessGroupedTerrainFeatureSpec(
                @"Pelagic Stillness",
                @"Water Feature",
                new[] { @"When the water stills, it feels vast enough to swallow scale and distance alike.", @"At a glance, when the water stills, it feels vast enough to swallow scale and distance alike.", @"One of the clearest local details is that when the water stills, it feels vast enough to swallow scale and distance alike.", @"The eye is drawn to the fact that when the water stills, it feels vast enough to swallow scale and distance alike.", @"When the water stills, it feels vast enough to swallow scale and distance alike, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Personal Touches"] = new WildernessGroupedTerrainFeatureSpec(
                @"Personal Touches",
                @"Urban Feature",
                new[] { @"Small personal touches interrupt plain utility and make the space feel claimed.", @"At a glance, small personal touches interrupt plain utility and make the space feel claimed.", @"One of the clearest local details is that small personal touches interrupt plain utility and make the space feel claimed.", @"The eye is drawn to the fact that small personal touches interrupt plain utility and make the space feel claimed.", @"Small personal touches interrupt plain utility and make the space feel claimed, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Planetary Arc"] = new WildernessGroupedTerrainFeatureSpec(
                @"Planetary Arc",
                @"Extraterrestrial Feature",
                new[] { @"A vast curve of planet or moon dominates part of the surrounding sky.", @"At a glance, a vast curve of planet or moon dominates part of the surrounding sky.", @"One of the clearest local details is that a vast curve of planet or moon dominates part of the surrounding sky.", @"The eye is drawn to the fact that a vast curve of planet or moon dominates part of the surrounding sky.", @"A vast curve of planet or moon dominates part of the surrounding sky, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Powdery Dust"] = new WildernessGroupedTerrainFeatureSpec(
                @"Powdery Dust",
                @"Cave Feature",
                new[] { @"Dry mineral dust softens the ground where no recent water has reached.", @"At a glance, dry mineral dust softens the ground where no recent water has reached.", @"One of the clearest local details is that dry mineral dust softens the ground where no recent water has reached.", @"The eye is drawn to the fact that dry mineral dust softens the ground where no recent water has reached.", @"Dry mineral dust softens the ground where no recent water has reached, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Public Monuments"] = new WildernessGroupedTerrainFeatureSpec(
                @"Public Monuments",
                @"Urban Feature",
                new[] { @"Built features meant to be seen rather than used lend the area a civic confidence.", @"At a glance, built features meant to be seen rather than used lend the area a civic confidence.", @"One of the clearest local details is that built features meant to be seen rather than used lend the area a civic confidence.", @"The eye is drawn to the fact that built features meant to be seen rather than used lend the area a civic confidence.", @"Built features meant to be seen rather than used lend the area a civic confidence, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Quiet Verge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Quiet Verge",
                @"Urban Feature",
                new[] { @"The road-edge here feels a little broader and less pressed by constant use.", @"At a glance, the road-edge here feels a little broader and less pressed by constant use.", @"One of the clearest local details is that the road-edge here feels a little broader and less pressed by constant use.", @"The eye is drawn to the fact that the road-edge here feels a little broader and less pressed by constant use.", @"The road-edge here feels a little broader and less pressed by constant use, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Rain-Heavy Leaves"] = new WildernessGroupedTerrainFeatureSpec(
                @"Rain-Heavy Leaves",
                @"Forest Feature",
                new[] { @"Moisture gathers and releases slowly from the vegetation above instead of passing straight through.", @"At a glance, moisture gathers and releases slowly from the vegetation above instead of passing straight through.", @"One of the clearest local details is that moisture gathers and releases slowly from the vegetation above instead of passing straight through.", @"The eye is drawn to the fact that moisture gathers and releases slowly from the vegetation above instead of passing straight through.", @"Moisture gathers and releases slowly from the vegetation above instead of passing straight through, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Recent Cleaning"] = new WildernessGroupedTerrainFeatureSpec(
                @"Recent Cleaning",
                @"Urban Feature",
                new[] { @"Recently cleaned surfaces leave the place feeling a touch barer and sharper than it otherwise would.", @"At a glance, recently cleaned surfaces leave the place feeling a touch barer and sharper than it otherwise would.", @"One of the clearest local details is that recently cleaned surfaces leave the place feeling a touch barer and sharper than it otherwise would.", @"The eye is drawn to the fact that recently cleaned surfaces leave the place feeling a touch barer and sharper than it otherwise would.", @"Recently cleaned surfaces leave the place feeling a touch barer and sharper than it otherwise would, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Reed Bed"] = new WildernessGroupedTerrainFeatureSpec(
                @"Reed Bed",
                @"Wetland Feature",
                new[] { @"Dense reeds gather in a stand thick enough to move as one when wind or water reaches them.", @"At a glance, dense reeds gather in a stand thick enough to move as one when wind or water reaches them.", @"One of the clearest local details is that dense reeds gather in a stand thick enough to move as one when wind or water reaches them.", @"The eye is drawn to the fact that dense reeds gather in a stand thick enough to move as one when wind or water reaches them.", @"Dense reeds gather in a stand thick enough to move as one when wind or water reaches them, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Reed Fringe"] = new WildernessGroupedTerrainFeatureSpec(
                @"Reed Fringe",
                @"Open Land Feature",
                new[] { @"Reeds or similar water-loving growth gather where damp ground tips toward standing moisture.", @"At a glance, reeds or similar water-loving growth gather where damp ground tips toward standing moisture.", @"One of the clearest local details is that reeds or similar water-loving growth gather where damp ground tips toward standing moisture.", @"The eye is drawn to the fact that reeds or similar water-loving growth gather where damp ground tips toward standing moisture.", @"Reeds or similar water-loving growth gather where damp ground tips toward standing moisture, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Reed Harvest"] = new WildernessGroupedTerrainFeatureSpec(
                @"Reed Harvest",
                @"Botanical Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Reed Margin"] = new WildernessGroupedTerrainFeatureSpec(
                @"Reed Margin",
                @"Water Feature",
                new[] { @"Water-loving growth marks the softer boundary between open water and firm ground.", @"At a glance, water-loving growth marks the softer boundary between open water and firm ground.", @"One of the clearest local details is that water-loving growth marks the softer boundary between open water and firm ground.", @"The eye is drawn to the fact that water-loving growth marks the softer boundary between open water and firm ground.", @"Water-loving growth marks the softer boundary between open water and firm ground, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Reed Rustle"] = new WildernessGroupedTerrainFeatureSpec(
                @"Reed Rustle",
                @"Wetland Sound",
                new[] { @"environment{day=Every passing breeze sets the reeds rasping softly against one another.}{night=At night the movement of reeds is easier to hear than to see.}{rain=Rain turns the reed-rustle into a heavier, wetter whisper.}{The reeds keep up a dry, brushing sound whenever the air moves.}", @"environment{dawn=Morning air stirs the reeds into a gentle, papery rustle.}{afternoon=Stronger gusts pass visibly and audibly through the stands of reed.}{night=The reeds talk quietly to the darkness.}{The marsh edge is seldom silent while the reeds are moving.}", @"environment{autumn=Drier reed-stems rasp more sharply in the wind.}{spring=New growth softens the sound into something greener and gentler.}{Wind turns the reeds into a low whispering wall.}", @"environment{day=Reed against reed makes a sound like dry cloth being worried by careful hands.}{rain=Wet stems brush together more softly, but never quite fall silent.}{The taller wetland plants have their own restless voice.}" },
                80.0,
                false),
            [@"Refuse Scatter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Refuse Scatter",
                @"Urban Feature",
                new[] { @"Discarded scraps gather where no one has yet bothered to clear them away.", @"At a glance, discarded scraps gather where no one has yet bothered to clear them away.", @"One of the clearest local details is that discarded scraps gather where no one has yet bothered to clear them away.", @"The eye is drawn to the fact that discarded scraps gather where no one has yet bothered to clear them away.", @"Discarded scraps gather where no one has yet bothered to clear them away, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Remote Beacon"] = new WildernessGroupedTerrainFeatureSpec(
                @"Remote Beacon",
                @"Extraterrestrial Feature",
                new[] { @"A remote beacon or running light blinks in measured intervals against the dark.", @"At a glance, a remote beacon or running light blinks in measured intervals against the dark.", @"One of the clearest local details is that a remote beacon or running light blinks in measured intervals against the dark.", @"The eye is drawn to the fact that a remote beacon or running light blinks in measured intervals against the dark.", @"A remote beacon or running light blinks in measured intervals against the dark, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Resin Scent"] = new WildernessGroupedTerrainFeatureSpec(
                @"Resin Scent",
                @"Forest Feature",
                new[] { @"Warmth and bark together leave a resinous smell hanging faintly in the air.", @"At a glance, warmth and bark together leave a resinous smell hanging faintly in the air.", @"One of the clearest local details is that warmth and bark together leave a resinous smell hanging faintly in the air.", @"The eye is drawn to the fact that warmth and bark together leave a resinous smell hanging faintly in the air.", @"Warmth and bark together leave a resinous smell hanging faintly in the air, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Resinous Brush"] = new WildernessGroupedTerrainFeatureSpec(
                @"Resinous Brush",
                @"Open Land Feature",
                new[] { @"The brush here gives off a dry, resinous scent when stirred or warmed.", @"At a glance, the brush here gives off a dry, resinous scent when stirred or warmed.", @"One of the clearest local details is that the brush here gives off a dry, resinous scent when stirred or warmed.", @"The eye is drawn to the fact that the brush here gives off a dry, resinous scent when stirred or warmed.", @"The brush here gives off a dry, resinous scent when stirred or warmed, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"River Cut"] = new WildernessGroupedTerrainFeatureSpec(
                @"River Cut",
                @"Rock Feature",
                new[] { @"Running water has clearly helped carve the land into its present shape.", @"At a glance, running water has clearly helped carve the land into its present shape.", @"One of the clearest local details is that running water has clearly helped carve the land into its present shape.", @"The eye is drawn to the fact that running water has clearly helped carve the land into its present shape.", @"Running water has clearly helped carve the land into its present shape, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"River Rush"] = new WildernessGroupedTerrainFeatureSpec(
                @"River Rush",
                @"Water Sound",
                new[] { @"environment{day=The river pushes by with an urgent, rushing voice.}{night=At night the force of the river is more audible than visible.}{rain=Rain swells the sound until even smaller currents feel impatient.}{The river moves with a constant rushing sound.}", @"environment{morning=Morning air makes every quickening run and broken line of current seem louder.}{afternoon=The stronger reaches keep up a steady rushing complaint.}{night=The river dominates the dark by sound alone.}{The water does not pass quietly here.}", @"environment{spring=Seasonal force gives the river a fuller, harder rush.}{autumn=Cooler flow sharpens the rush into a cleaner sound.}{The channel speaks in fast water and hurried eddies.}", @"environment{day=Even where the surface looks manageable, the river sounds more forceful than still water ever could.}{night=The rush keeps going in the dark, indifferent to what can or cannot be seen.}{The character of the place is shaped by swift water.}" },
                80.0,
                false),
            [@"River Silt Smell"] = new WildernessGroupedTerrainFeatureSpec(
                @"River Silt Smell",
                @"Water Smell",
                new[] { @"environment{day=The water and banks smell of silt, damp clay, and fresh disturbance.}{rain=Rain thickens the smell of worked-up silt and bank-mud.}{night=At night the river-smell becomes more mud and moisture than anything else.}{There is a clear river smell of silt and wet bank here.}", @"environment{spring=Higher water gives the river a stronger smell of moved earth.}{autumn=Lower, clearer flow leaves the mud-smell leaner but still present.}{The place smells of river mud and stirred sediment.}", @"environment{day=Fresh water alone would smell clean; here the banks add clay and silt to it.}{rain=Rain renews every muddier note in the river air.}{The air carries the smell of a working river, not an ornamental one.}", @"environment{morning=Cool air often makes the smell of silt easier to distinguish.}{afternoon=Warmth draws more mud-scent off the banks.}{The river announces itself by smell as much as sound.}" },
                75.0,
                false),
            [@"Roadside Marker"] = new WildernessGroupedTerrainFeatureSpec(
                @"Roadside Marker",
                @"Road Feature",
                new[] { @"A post, stone, or similar marker gives the way a more maintained and legible character.", @"At a glance, a post, stone, or similar marker gives the way a more maintained and legible character.", @"One of the clearest local details is that a post, stone, or similar marker gives the way a more maintained and legible character.", @"The eye is drawn to the fact that a post, stone, or similar marker gives the way a more maintained and legible character.", @"A post, stone, or similar marker gives the way a more maintained and legible character, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Rolling Rise"] = new WildernessGroupedTerrainFeatureSpec(
                @"Rolling Rise",
                @"Rock Feature",
                new[] { @"The land lifts and falls in broad shapes rather than abrupt breaks.", @"At a glance, the land lifts and falls in broad shapes rather than abrupt breaks.", @"One of the clearest local details is that the land lifts and falls in broad shapes rather than abrupt breaks.", @"The eye is drawn to the fact that the land lifts and falls in broad shapes rather than abrupt breaks.", @"The land lifts and falls in broad shapes rather than abrupt breaks, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Root-Broken Path"] = new WildernessGroupedTerrainFeatureSpec(
                @"Root-Broken Path",
                @"Road Feature",
                new[] { @"Roots have worked up through the way, disturbing what should have been a simpler passage.", @"At a glance, roots have worked up through the way, disturbing what should have been a simpler passage.", @"One of the clearest local details is that roots have worked up through the way, disturbing what should have been a simpler passage.", @"The eye is drawn to the fact that roots have worked up through the way, disturbing what should have been a simpler passage.", @"Roots have worked up through the way, disturbing what should have been a simpler passage, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Rotting Heap"] = new WildernessGroupedTerrainFeatureSpec(
                @"Rotting Heap",
                @"Urban Feature",
                new[] { @"A ripe, active heap of organic waste dominates the immediate area.", @"At a glance, a ripe, active heap of organic waste dominates the immediate area.", @"One of the clearest local details is that a ripe, active heap of organic waste dominates the immediate area.", @"The eye is drawn to the fact that a ripe, active heap of organic waste dominates the immediate area.", @"A ripe, active heap of organic waste dominates the immediate area, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Rush Clumps"] = new WildernessGroupedTerrainFeatureSpec(
                @"Rush Clumps",
                @"Wetland Feature",
                new[] { @"Rushes rise in tougher, sparser clumps that mark the wetter patches of ground.", @"At a glance, rushes rise in tougher, sparser clumps that mark the wetter patches of ground.", @"One of the clearest local details is that rushes rise in tougher, sparser clumps that mark the wetter patches of ground.", @"The eye is drawn to the fact that rushes rise in tougher, sparser clumps that mark the wetter patches of ground.", @"Rushes rise in tougher, sparser clumps that mark the wetter patches of ground, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Salt Crust"] = new WildernessGroupedTerrainFeatureSpec(
                @"Salt Crust",
                @"Open Land Feature",
                new[] { @"A pale crust lies over parts of the surface where moisture has gone and left its minerals behind.", @"At a glance, a pale crust lies over parts of the surface where moisture has gone and left its minerals behind.", @"One of the clearest local details is that a pale crust lies over parts of the surface where moisture has gone and left its minerals behind.", @"The eye is drawn to the fact that a pale crust lies over parts of the surface where moisture has gone and left its minerals behind.", @"A pale crust lies over parts of the surface where moisture has gone and left its minerals behind, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Salt Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Salt Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Salt Spray"] = new WildernessGroupedTerrainFeatureSpec(
                @"Salt Spray",
                @"Water Smell",
                new[] { @"environment{day=Salt spray leaves a clean briny smell in the air.}{rain=Rain blunts the sharper salt, but the sea still declares itself.}{night=At night the briny smell lingers even when the water is hard to read.}{A clear salt smell rides the air here.}", @"environment{day=Moving air carries the sharper smell of salt and open water.}{day=The sea lends the place its clean, brined scent.}{There is no mistaking the smell of salt water here.}", @"environment{day=Brine and spray keep the air tasting of the sea.}{torrential=Heavier weather throws more salt into the air at once.}{The water perfumes the place in salt.}", @"environment{morning=Morning air often makes the salt smell feel especially clean.}{night=The same briny scent remains after detail has gone from the water.}{Salt hangs easily in the air here.}" },
                75.0,
                false),
            [@"Salted Breeze"] = new WildernessGroupedTerrainFeatureSpec(
                @"Salted Breeze",
                @"Desert Feature",
                new[] { @"The air carries a faint salt note, suggesting a coast or drying basin not far off.", @"At a glance, the air carries a faint salt note, suggesting a coast or drying basin not far off.", @"One of the clearest local details is that the air carries a faint salt note, suggesting a coast or drying basin not far off.", @"The eye is drawn to the fact that the air carries a faint salt note, suggesting a coast or drying basin not far off.", @"The air carries a faint salt note, suggesting a coast or drying basin not far off, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Sand Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sand Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Sand Ripple"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sand Ripple",
                @"Water Feature",
                new[] { @"Shallow ridges in the sand show the last direction in which water or wind strongly moved.", @"At a glance, shallow ridges in the sand show the last direction in which water or wind strongly moved.", @"One of the clearest local details is that shallow ridges in the sand show the last direction in which water or wind strongly moved.", @"The eye is drawn to the fact that shallow ridges in the sand show the last direction in which water or wind strongly moved.", @"Shallow ridges in the sand show the last direction in which water or wind strongly moved, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Sastrugi Ridges"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sastrugi Ridges",
                @"Glacial Feature",
                new[] { @"The wind has shaped the snow into harder ridges and troughs aligned with its passing.", @"At a glance, the wind has shaped the snow into harder ridges and troughs aligned with its passing.", @"One of the clearest local details is that the wind has shaped the snow into harder ridges and troughs aligned with its passing.", @"The eye is drawn to the fact that the wind has shaped the snow into harder ridges and troughs aligned with its passing.", @"The wind has shaped the snow into harder ridges and troughs aligned with its passing, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Scattered Stones"] = new WildernessGroupedTerrainFeatureSpec(
                @"Scattered Stones",
                @"Open Land Feature",
                new[] { @"Loose stones lie through the ground here, too many to ignore and too few to dominate.", @"At a glance, loose stones lie through the ground here, too many to ignore and too few to dominate.", @"One of the clearest local details is that loose stones lie through the ground here, too many to ignore and too few to dominate.", @"The eye is drawn to the fact that loose stones lie through the ground here, too many to ignore and too few to dominate.", @"Loose stones lie through the ground here, too many to ignore and too few to dominate, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Scattered Trees"] = new WildernessGroupedTerrainFeatureSpec(
                @"Scattered Trees",
                @"Open Land Feature",
                new[] { @"A few trees stand apart from one another, offering points of shade without closing the land in.", @"At a glance, a few trees stand apart from one another, offering points of shade without closing the land in.", @"One of the clearest local details is that a few trees stand apart from one another, offering points of shade without closing the land in.", @"The eye is drawn to the fact that a few trees stand apart from one another, offering points of shade without closing the land in.", @"A few trees stand apart from one another, offering points of shade without closing the land in, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Seating Cluster"] = new WildernessGroupedTerrainFeatureSpec(
                @"Seating Cluster",
                @"Urban Feature",
                new[] { @"Benches, low walls, or similar resting points encourage lingering rather than simple passage.", @"At a glance, benches, low walls, or similar resting points encourage lingering rather than simple passage.", @"One of the clearest local details is that benches, low walls, or similar resting points encourage lingering rather than simple passage.", @"The eye is drawn to the fact that benches, low walls, or similar resting points encourage lingering rather than simple passage.", @"Benches, low walls, or similar resting points encourage lingering rather than simple passage, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Seedhead Sweep"] = new WildernessGroupedTerrainFeatureSpec(
                @"Seedhead Sweep",
                @"Open Land Feature",
                new[] { @"Dry seedheads move together whenever the wind catches them, giving the ground a restless surface.", @"At a glance, dry seedheads move together whenever the wind catches them, giving the ground a restless surface.", @"One of the clearest local details is that dry seedheads move together whenever the wind catches them, giving the ground a restless surface.", @"The eye is drawn to the fact that dry seedheads move together whenever the wind catches them, giving the ground a restless surface.", @"Dry seedheads move together whenever the wind catches them, giving the ground a restless surface, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Shade Tree"] = new WildernessGroupedTerrainFeatureSpec(
                @"Shade Tree",
                @"Open Land Feature",
                new[] { @"One tree or a small cluster gives the surrounding openness a rare and useful patch of shade.", @"At a glance, one tree or a small cluster gives the surrounding openness a rare and useful patch of shade.", @"One of the clearest local details is that one tree or a small cluster gives the surrounding openness a rare and useful patch of shade.", @"The eye is drawn to the fact that one tree or a small cluster gives the surrounding openness a rare and useful patch of shade.", @"One tree or a small cluster gives the surrounding openness a rare and useful patch of shade, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Shallow Swale"] = new WildernessGroupedTerrainFeatureSpec(
                @"Shallow Swale",
                @"Open Land Feature",
                new[] { @"A slight low run in the ground catches the eye once the vegetation is read carefully enough.", @"At a glance, a slight low run in the ground catches the eye once the vegetation is read carefully enough.", @"One of the clearest local details is that a slight low run in the ground catches the eye once the vegetation is read carefully enough.", @"The eye is drawn to the fact that a slight low run in the ground catches the eye once the vegetation is read carefully enough.", @"A slight low run in the ground catches the eye once the vegetation is read carefully enough, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Sharp Drop"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sharp Drop",
                @"Rock Feature",
                new[] { @"The land gives way abruptly enough to command immediate caution.", @"At a glance, the land gives way abruptly enough to command immediate caution.", @"One of the clearest local details is that the land gives way abruptly enough to command immediate caution.", @"The eye is drawn to the fact that the land gives way abruptly enough to command immediate caution.", @"The land gives way abruptly enough to command immediate caution, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Sheltered Hollow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sheltered Hollow",
                @"Rock Feature",
                new[] { @"A slight hollow offers a little shelter from wind and sight without becoming truly hidden.", @"At a glance, a slight hollow offers a little shelter from wind and sight without becoming truly hidden.", @"One of the clearest local details is that a slight hollow offers a little shelter from wind and sight without becoming truly hidden.", @"The eye is drawn to the fact that a slight hollow offers a little shelter from wind and sight without becoming truly hidden.", @"A slight hollow offers a little shelter from wind and sight without becoming truly hidden, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Shorebird Calls"] = new WildernessGroupedTerrainFeatureSpec(
                @"Shorebird Calls",
                @"Water Sound",
                new[] { @"environment{day=Sharp shorebird cries carry over the water and exposed margin.}{dawn=The birds are louder and busier in the first light.}{rain=Wet weather leaves only the occasional annoyed call.}{night=The shorebirds fall mostly quiet once darkness settles fully.}{Shorebirds cry out now and then along the edge.}", @"environment{spring=The birds sound quick, territorial, and unembarrassed in the growing season.}{autumn=Passing flocks make the shore feel briefly busier and more transient.}{day=Quick, bright calls run along the water's edge.}{The littoral margin is punctuated by restless bird-calls.}", @"environment{morning=Morning air lets the smaller cries travel farther than expected.}{afternoon=By afternoon the calls come in shorter, more scattered bursts.}{The sound of shoreline birds comes and goes with the margin's own activity.}", @"environment{day=Even small birds sound distinct against the more open water-noise.}{night=Only the odd disturbed cry breaks the darker hush.}{Birds keep some claim on the place, despite the water.}" },
                80.0,
                false),
            [@"Short Grass"] = new WildernessGroupedTerrainFeatureSpec(
                @"Short Grass",
                @"Open Land Feature",
                new[] { @"The growth stays cropped close to the soil, leaving the land's shape plainly visible.", @"At a glance, the growth stays cropped close to the soil, leaving the land's shape plainly visible.", @"One of the clearest local details is that the growth stays cropped close to the soil, leaving the land's shape plainly visible.", @"The eye is drawn to the fact that the growth stays cropped close to the soil, leaving the land's shape plainly visible.", @"The growth stays cropped close to the soil, leaving the land's shape plainly visible, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Shuttered Front"] = new WildernessGroupedTerrainFeatureSpec(
                @"Shuttered Front",
                @"Urban Feature",
                new[] { @"The frontage feels built to open and close against passing trade rather than remain private.", @"At a glance, the frontage feels built to open and close against passing trade rather than remain private.", @"One of the clearest local details is that the frontage feels built to open and close against passing trade rather than remain private.", @"The eye is drawn to the fact that the frontage feels built to open and close against passing trade rather than remain private.", @"The frontage feels built to open and close against passing trade rather than remain private, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Slick Stone"] = new WildernessGroupedTerrainFeatureSpec(
                @"Slick Stone",
                @"Cave Feature",
                new[] { @"Moisture has left the nearby stone smoother and more treacherous than dry rock would be.", @"At a glance, moisture has left the nearby stone smoother and more treacherous than dry rock would be.", @"One of the clearest local details is that moisture has left the nearby stone smoother and more treacherous than dry rock would be.", @"The eye is drawn to the fact that moisture has left the nearby stone smoother and more treacherous than dry rock would be.", @"Moisture has left the nearby stone smoother and more treacherous than dry rock would be, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Slow Tumble"] = new WildernessGroupedTerrainFeatureSpec(
                @"Slow Tumble",
                @"Extraterrestrial Feature",
                new[] { @"Nearby fragments seem to drift in a slow tumble that never quite resolves into stillness.", @"At a glance, nearby fragments seem to drift in a slow tumble that never quite resolves into stillness.", @"One of the clearest local details is that nearby fragments seem to drift in a slow tumble that never quite resolves into stillness.", @"The eye is drawn to the fact that nearby fragments seem to drift in a slow tumble that never quite resolves into stillness.", @"Nearby fragments seem to drift in a slow tumble that never quite resolves into stillness, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Smoke Stain"] = new WildernessGroupedTerrainFeatureSpec(
                @"Smoke Stain",
                @"Urban Feature",
                new[] { @"Old smoke and greasy discolouration mark nearby surfaces.", @"At a glance, old smoke and greasy discolouration mark nearby surfaces.", @"One of the clearest local details is that old smoke and greasy discolouration mark nearby surfaces.", @"The eye is drawn to the fact that old smoke and greasy discolouration mark nearby surfaces.", @"Old smoke and greasy discolouration mark nearby surfaces, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Snow Blind Glare"] = new WildernessGroupedTerrainFeatureSpec(
                @"Snow Blind Glare",
                @"Glacial Feature",
                new[] { @"The reflected brightness off snow and ice is harsh enough to flatten lesser detail.", @"At a glance, the reflected brightness off snow and ice is harsh enough to flatten lesser detail.", @"One of the clearest local details is that the reflected brightness off snow and ice is harsh enough to flatten lesser detail.", @"The eye is drawn to the fact that the reflected brightness off snow and ice is harsh enough to flatten lesser detail.", @"The reflected brightness off snow and ice is harsh enough to flatten lesser detail, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Soft Household Clutter"] = new WildernessGroupedTerrainFeatureSpec(
                @"Soft Household Clutter",
                @"Urban Feature",
                new[] { @"A little harmless clutter remains, making the room feel occupied rather than neglected.", @"At a glance, a little harmless clutter remains, making the room feel occupied rather than neglected.", @"One of the clearest local details is that a little harmless clutter remains, making the room feel occupied rather than neglected.", @"The eye is drawn to the fact that a little harmless clutter remains, making the room feel occupied rather than neglected.", @"A little harmless clutter remains, making the room feel occupied rather than neglected, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Sounding Depths"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sounding Depths",
                @"Water Feature",
                new[] { @"The water beyond the nearer edge darkens quickly into greater depth.", @"At a glance, the water beyond the nearer edge darkens quickly into greater depth.", @"One of the clearest local details is that the water beyond the nearer edge darkens quickly into greater depth.", @"The eye is drawn to the fact that the water beyond the nearer edge darkens quickly into greater depth.", @"The water beyond the nearer edge darkens quickly into greater depth, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Sparse Sedge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sparse Sedge",
                @"Open Land Feature",
                new[] { @"Thin sedge and cold-tough growth cling to the ground without softening it much.", @"At a glance, thin sedge and cold-tough growth cling to the ground without softening it much.", @"One of the clearest local details is that thin sedge and cold-tough growth cling to the ground without softening it much.", @"The eye is drawn to the fact that thin sedge and cold-tough growth cling to the ground without softening it much.", @"Thin sedge and cold-tough growth cling to the ground without softening it much, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Sparse Starlight"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sparse Starlight",
                @"Extraterrestrial Feature",
                new[] { @"The visible stars feel widely spaced, leaving the darkness between them pronounced.", @"At a glance, the visible stars feel widely spaced, leaving the darkness between them pronounced.", @"One of the clearest local details is that the visible stars feel widely spaced, leaving the darkness between them pronounced.", @"The eye is drawn to the fact that the visible stars feel widely spaced, leaving the darkness between them pronounced.", @"The visible stars feel widely spaced, leaving the darkness between them pronounced, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Sphagnum Mat"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sphagnum Mat",
                @"Wetland Feature",
                new[] { @"Soft mossy growth spreads over the wet ground like a living sponge.", @"At a glance, soft mossy growth spreads over the wet ground like a living sponge.", @"One of the clearest local details is that soft mossy growth spreads over the wet ground like a living sponge.", @"The eye is drawn to the fact that soft mossy growth spreads over the wet ground like a living sponge.", @"Soft mossy growth spreads over the wet ground like a living sponge, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Spray Marks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Spray Marks",
                @"Water Feature",
                new[] { @"Salt, moisture, or damp marking shows how often broken water reaches this point.", @"At a glance, salt, moisture, or damp marking shows how often broken water reaches this point.", @"One of the clearest local details is that salt, moisture, or damp marking shows how often broken water reaches this point.", @"The eye is drawn to the fact that salt, moisture, or damp marking shows how often broken water reaches this point.", @"Salt, moisture, or damp marking shows how often broken water reaches this point, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Spring Water"] = new WildernessGroupedTerrainFeatureSpec(
                @"Spring Water",
                @"Desert Feature",
                new[] { @"Reliable water gives this place a significance out of proportion to its size.", @"At a glance, reliable water gives this place a significance out of proportion to its size.", @"One of the clearest local details is that reliable water gives this place a significance out of proportion to its size.", @"The eye is drawn to the fact that reliable water gives this place a significance out of proportion to its size.", @"Reliable water gives this place a significance out of proportion to its size, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Stacked Goods"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stacked Goods",
                @"Urban Feature",
                new[] { @"Goods or supplies have been gathered in a way that puts storage ahead of appearance.", @"At a glance, goods or supplies have been gathered in a way that puts storage ahead of appearance.", @"One of the clearest local details is that goods or supplies have been gathered in a way that puts storage ahead of appearance.", @"The eye is drawn to the fact that goods or supplies have been gathered in a way that puts storage ahead of appearance.", @"Goods or supplies have been gathered in a way that puts storage ahead of appearance, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Stale Air"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stale Air",
                @"Urban Feature",
                new[] { @"The air sits heavy and unmoving, as though it has lingered here longer than is comfortable.", @"At a glance, the air sits heavy and unmoving, as though it has lingered here longer than is comfortable.", @"One of the clearest local details is that the air sits heavy and unmoving, as though it has lingered here longer than is comfortable.", @"The eye is drawn to the fact that the air sits heavy and unmoving, as though it has lingered here longer than is comfortable.", @"The air sits heavy and unmoving, as though it has lingered here longer than is comfortable, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Stall Layout"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stall Layout",
                @"Urban Feature",
                new[] { @"The arrangement of space suggests bargaining, display, and repeated short exchanges.", @"At a glance, the arrangement of space suggests bargaining, display, and repeated short exchanges.", @"One of the clearest local details is that the arrangement of space suggests bargaining, display, and repeated short exchanges.", @"The eye is drawn to the fact that the arrangement of space suggests bargaining, display, and repeated short exchanges.", @"The arrangement of space suggests bargaining, display, and repeated short exchanges, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Standing Water"] = new WildernessGroupedTerrainFeatureSpec(
                @"Standing Water",
                @"Wetland Feature",
                new[] { @"Water stands close enough to the surface here to claim part of the ground outright.", @"At a glance, water stands close enough to the surface here to claim part of the ground outright.", @"One of the clearest local details is that water stands close enough to the surface here to claim part of the ground outright.", @"The eye is drawn to the fact that water stands close enough to the surface here to claim part of the ground outright.", @"Water stands close enough to the surface here to claim part of the ground outright, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Station Shadow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Station Shadow",
                @"Extraterrestrial Feature",
                new[] { @"A stark-edged shadow cuts across part of the scene where structure blocks the light.", @"At a glance, a stark-edged shadow cuts across part of the scene where structure blocks the light.", @"One of the clearest local details is that a stark-edged shadow cuts across part of the scene where structure blocks the light.", @"The eye is drawn to the fact that a stark-edged shadow cuts across part of the scene where structure blocks the light.", @"A stark-edged shadow cuts across part of the scene where structure blocks the light, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Steady Traffic"] = new WildernessGroupedTerrainFeatureSpec(
                @"Steady Traffic",
                @"Urban Feature",
                new[] { @"Regular movement has given the place a worked smoothness that newer ground never quite has.", @"At a glance, regular movement has given the place a worked smoothness that newer ground never quite has.", @"One of the clearest local details is that regular movement has given the place a worked smoothness that newer ground never quite has.", @"The eye is drawn to the fact that regular movement has given the place a worked smoothness that newer ground never quite has.", @"Regular movement has given the place a worked smoothness that newer ground never quite has, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Steam Haze"] = new WildernessGroupedTerrainFeatureSpec(
                @"Steam Haze",
                @"Urban Feature",
                new[] { @"Humidity softens the air and blurs sharper edges into something warmer and less distinct.", @"At a glance, humidity softens the air and blurs sharper edges into something warmer and less distinct.", @"One of the clearest local details is that humidity softens the air and blurs sharper edges into something warmer and less distinct.", @"The eye is drawn to the fact that humidity softens the air and blurs sharper edges into something warmer and less distinct.", @"Humidity softens the air and blurs sharper edges into something warmer and less distinct, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Steep Grade"] = new WildernessGroupedTerrainFeatureSpec(
                @"Steep Grade",
                @"Rock Feature",
                new[] { @"The angle of the ground makes every movement here feel a little more deliberate.", @"At a glance, the angle of the ground makes every movement here feel a little more deliberate.", @"One of the clearest local details is that the angle of the ground makes every movement here feel a little more deliberate.", @"The eye is drawn to the fact that the angle of the ground makes every movement here feel a little more deliberate.", @"The angle of the ground makes every movement here feel a little more deliberate, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Still Pool"] = new WildernessGroupedTerrainFeatureSpec(
                @"Still Pool",
                @"Cave Feature",
                new[] { @"Water has collected into a still pool that holds light and darkness with equal ease.", @"At a glance, water has collected into a still pool that holds light and darkness with equal ease.", @"One of the clearest local details is that water has collected into a still pool that holds light and darkness with equal ease.", @"The eye is drawn to the fact that water has collected into a still pool that holds light and darkness with equal ease.", @"Water has collected into a still pool that holds light and darkness with equal ease, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Still Water"] = new WildernessGroupedTerrainFeatureSpec(
                @"Still Water",
                @"Urban Feature",
                new[] { @"Water held close at hand gives the space a quieter, more reflective quality.", @"At a glance, water held close at hand gives the space a quieter, more reflective quality.", @"One of the clearest local details is that water held close at hand gives the space a quieter, more reflective quality.", @"The eye is drawn to the fact that water held close at hand gives the space a quieter, more reflective quality.", @"Water held close at hand gives the space a quieter, more reflective quality, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Stone Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stone Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Stone Kerb"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stone Kerb",
                @"Road Feature",
                new[] { @"Worked edging gives the road a cleaner and more deliberate boundary.", @"At a glance, worked edging gives the road a cleaner and more deliberate boundary.", @"One of the clearest local details is that worked edging gives the road a cleaner and more deliberate boundary.", @"The eye is drawn to the fact that worked edging gives the road a cleaner and more deliberate boundary.", @"Worked edging gives the road a cleaner and more deliberate boundary, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Stone Overhang"] = new WildernessGroupedTerrainFeatureSpec(
                @"Stone Overhang",
                @"Rock Feature",
                new[] { @"Part of the rock-face projects enough to suggest a shallow shelter or visual weight above.", @"At a glance, part of the rock-face projects enough to suggest a shallow shelter or visual weight above.", @"One of the clearest local details is that part of the rock-face projects enough to suggest a shallow shelter or visual weight above.", @"The eye is drawn to the fact that part of the rock-face projects enough to suggest a shallow shelter or visual weight above.", @"Part of the rock-face projects enough to suggest a shallow shelter or visual weight above, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Storage Racks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Storage Racks",
                @"Urban Feature",
                new[] { @"Shelving and racks turn the walls into working storage rather than decoration.", @"At a glance, shelving and racks turn the walls into working storage rather than decoration.", @"One of the clearest local details is that shelving and racks turn the walls into working storage rather than decoration.", @"The eye is drawn to the fact that shelving and racks turn the walls into working storage rather than decoration.", @"Shelving and racks turn the walls into working storage rather than decoration, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Street Trees"] = new WildernessGroupedTerrainFeatureSpec(
                @"Street Trees",
                @"Urban Feature",
                new[] { @"Planted trees break the harder lines of the street and lend it a more deliberate care.", @"At a glance, planted trees break the harder lines of the street and lend it a more deliberate care.", @"One of the clearest local details is that planted trees break the harder lines of the street and lend it a more deliberate care.", @"The eye is drawn to the fact that planted trees break the harder lines of the street and lend it a more deliberate care.", @"Planted trees break the harder lines of the street and lend it a more deliberate care, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Sulphur Deposit"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sulphur Deposit",
                @"Mineral Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Sulphur Reek"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sulphur Reek",
                @"Volcanic Feature",
                new[] { @"A mineral, sulphurous tang lingers in the air and leaves the place feeling less hospitable.", @"At a glance, a mineral, sulphurous tang lingers in the air and leaves the place feeling less hospitable.", @"One of the clearest local details is that a mineral, sulphurous tang lingers in the air and leaves the place feeling less hospitable.", @"The eye is drawn to the fact that a mineral, sulphurous tang lingers in the air and leaves the place feeling less hospitable.", @"A mineral, sulphurous tang lingers in the air and leaves the place feeling less hospitable, which gives the terrain a harsher, more worked-over edge." },
                100.0,
                false),
            [@"Sulphur Tang"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sulphur Tang",
                @"Volcanic Smell",
                new[] { @"environment{day=A faint sulphurous tang marks the air here.}{rain=Rain can sharpen the sulphur briefly as it wakes hot ground and old vents.}{night=In cooler darkness the sulphur note seems thinner, but harder to mistake.}{There is a clear sulphur tang in the air.}", @"environment{dry=Dry conditions leave the sulphur smell lean but persistent.}{humid=Moister air makes it fuller and meaner.}{The place smells touched by venting minerals.}", @"environment{day=The nose catches sulphur before the eye always knows why.}{rain=Moisture stirs the mineral stink into greater life.}{The volcanic ground lends the air a sour mineral smell.}", @"environment{morning=Cooler air can make the sulphur seem sharper.}{afternoon=Heat fattens the smell into something harsher.}{The air tastes faintly of sulphur.}" },
                75.0,
                false),
            [@"Sunlit Glade"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sunlit Glade",
                @"Forest Feature",
                new[] { @"An opening in the cover lets in enough light to change both mood and growth nearby.", @"At a glance, an opening in the cover lets in enough light to change both mood and growth nearby.", @"One of the clearest local details is that an opening in the cover lets in enough light to change both mood and growth nearby.", @"The eye is drawn to the fact that an opening in the cover lets in enough light to change both mood and growth nearby.", @"An opening in the cover lets in enough light to change both mood and growth nearby, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Sunlit Hull Glint"] = new WildernessGroupedTerrainFeatureSpec(
                @"Sunlit Hull Glint",
                @"Extraterrestrial Feature",
                new[] { @"Hard points of reflected light flash from distant metal or stone.", @"At a glance, hard points of reflected light flash from distant metal or stone.", @"One of the clearest local details is that hard points of reflected light flash from distant metal or stone.", @"The eye is drawn to the fact that hard points of reflected light flash from distant metal or stone.", @"Hard points of reflected light flash from distant metal or stone, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Surf Wash"] = new WildernessGroupedTerrainFeatureSpec(
                @"Surf Wash",
                @"Water Sound",
                new[] { @"environment{day=The surf washes in and out with a patient, breaking rhythm.}{night=At night the surf seems larger because it is heard more than seen.}{rain=Rain roughens the surface, but the repeated wash of surf still dominates.}{The water keeps breaking and drawing back in steady surf-sounds.}", @"environment{dawn=First light finds the surf already at its old work of breaking and retreating.}{afternoon=The brighter hours make the repeated wash seem almost methodical.}{night=The surf becomes a larger presence in the dark.}{Wave after wave works the margin with a continuous wash.}", @"environment{torrential=Heavier weather fattens the sound into something more forceful and insistent.}{rain=Even in rain, the sea keeps speaking in broken white noise.}{The shoreline is under constant negotiation from the surf.}", @"environment{day=The surf never quite allows silence to settle.}{night=Darkness strips away detail until mostly the wash remains.}{The sea makes itself known by the sound of breaking water.}" },
                80.0,
                false),
            [@"Swell Lift"] = new WildernessGroupedTerrainFeatureSpec(
                @"Swell Lift",
                @"Water Feature",
                new[] { @"The water rises and falls in broader heaves rather than in smaller broken chop.", @"At a glance, the water rises and falls in broader heaves rather than in smaller broken chop.", @"One of the clearest local details is that the water rises and falls in broader heaves rather than in smaller broken chop.", @"The eye is drawn to the fact that the water rises and falls in broader heaves rather than in smaller broken chop.", @"The water rises and falls in broader heaves rather than in smaller broken chop, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Tall Grass"] = new WildernessGroupedTerrainFeatureSpec(
                @"Tall Grass",
                @"Open Land Feature",
                new[] { @"Grass stands high enough here to brush against movement and blur the lesser folds of the ground.", @"At a glance, grass stands high enough here to brush against movement and blur the lesser folds of the ground.", @"One of the clearest local details is that grass stands high enough here to brush against movement and blur the lesser folds of the ground.", @"The eye is drawn to the fact that grass stands high enough here to brush against movement and blur the lesser folds of the ground.", @"Grass stands high enough here to brush against movement and blur the lesser folds of the ground, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Tangle of Thorns"] = new WildernessGroupedTerrainFeatureSpec(
                @"Tangle of Thorns",
                @"Open Land Feature",
                new[] { @"Interlocked thorn-growth creates a harsher boundary than its height alone would suggest.", @"At a glance, interlocked thorn-growth creates a harsher boundary than its height alone would suggest.", @"One of the clearest local details is that interlocked thorn-growth creates a harsher boundary than its height alone would suggest.", @"The eye is drawn to the fact that interlocked thorn-growth creates a harsher boundary than its height alone would suggest.", @"Interlocked thorn-growth creates a harsher boundary than its height alone would suggest, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Thick Underbrush"] = new WildernessGroupedTerrainFeatureSpec(
                @"Thick Underbrush",
                @"Forest Feature",
                new[] { @"Brush and sapling growth crowd the lower reaches and complicate easy movement.", @"At a glance, brush and sapling growth crowd the lower reaches and complicate easy movement.", @"One of the clearest local details is that brush and sapling growth crowd the lower reaches and complicate easy movement.", @"The eye is drawn to the fact that brush and sapling growth crowd the lower reaches and complicate easy movement.", @"Brush and sapling growth crowd the lower reaches and complicate easy movement, which changes both the light and the feeling of cover." },
                100.0,
                false),
            [@"Thorn Scrub"] = new WildernessGroupedTerrainFeatureSpec(
                @"Thorn Scrub",
                @"Open Land Feature",
                new[] { @"Low thorn-bearing growth makes some approaches less inviting than they first appear.", @"At a glance, low thorn-bearing growth makes some approaches less inviting than they first appear.", @"One of the clearest local details is that low thorn-bearing growth makes some approaches less inviting than they first appear.", @"The eye is drawn to the fact that low thorn-bearing growth makes some approaches less inviting than they first appear.", @"Low thorn-bearing growth makes some approaches less inviting than they first appear, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Tidal Mud"] = new WildernessGroupedTerrainFeatureSpec(
                @"Tidal Mud",
                @"Wetland Feature",
                new[] { @"The mud shows the smoothing and reworking that comes from regular tidal influence.", @"At a glance, the mud shows the smoothing and reworking that comes from regular tidal influence.", @"One of the clearest local details is that the mud shows the smoothing and reworking that comes from regular tidal influence.", @"The eye is drawn to the fact that the mud shows the smoothing and reworking that comes from regular tidal influence.", @"The mud shows the smoothing and reworking that comes from regular tidal influence, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Tide Pool Basin"] = new WildernessGroupedTerrainFeatureSpec(
                @"Tide Pool Basin",
                @"Water Feature",
                new[] { @"A small basin has trapped water and life apart from the larger body beside it.", @"At a glance, a small basin has trapped water and life apart from the larger body beside it.", @"One of the clearest local details is that a small basin has trapped water and life apart from the larger body beside it.", @"The eye is drawn to the fact that a small basin has trapped water and life apart from the larger body beside it.", @"A small basin has trapped water and life apart from the larger body beside it, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Timber Stand"] = new WildernessGroupedTerrainFeatureSpec(
                @"Timber Stand",
                @"Botanical Resource",
                Array.Empty<string>(),
                0.0,
                true),
            [@"Tool Marks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Tool Marks",
                @"Urban Feature",
                new[] { @"Bench edges, posts, or nearby surfaces bear the accumulated marks of practical work.", @"At a glance, bench edges, posts, or nearby surfaces bear the accumulated marks of practical work.", @"One of the clearest local details is that bench edges, posts, or nearby surfaces bear the accumulated marks of practical work.", @"The eye is drawn to the fact that bench edges, posts, or nearby surfaces bear the accumulated marks of practical work.", @"Bench edges, posts, or nearby surfaces bear the accumulated marks of practical work, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Training Scuffs"] = new WildernessGroupedTerrainFeatureSpec(
                @"Training Scuffs",
                @"Urban Feature",
                new[] { @"Scuffs, scrapes, and hard landings suggest repeated physical exertion in the space.", @"At a glance, scuffs, scrapes, and hard landings suggest repeated physical exertion in the space.", @"One of the clearest local details is that scuffs, scrapes, and hard landings suggest repeated physical exertion in the space.", @"The eye is drawn to the fact that scuffs, scrapes, and hard landings suggest repeated physical exertion in the space.", @"Scuffs, scrapes, and hard landings suggest repeated physical exertion in the space, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Trampled Verge"] = new WildernessGroupedTerrainFeatureSpec(
                @"Trampled Verge",
                @"Road Feature",
                new[] { @"The edge of the way has been worn down by feet, hooves, or wheels drifting off the main tread.", @"At a glance, the edge of the way has been worn down by feet, hooves, or wheels drifting off the main tread.", @"One of the clearest local details is that the edge of the way has been worn down by feet, hooves, or wheels drifting off the main tread.", @"The eye is drawn to the fact that the edge of the way has been worn down by feet, hooves, or wheels drifting off the main tread.", @"The edge of the way has been worn down by feet, hooves, or wheels drifting off the main tread, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Trim Lawn"] = new WildernessGroupedTerrainFeatureSpec(
                @"Trim Lawn",
                @"Urban Feature",
                new[] { @"Grass is kept in deliberate order rather than allowed to grow as it pleases.", @"At a glance, grass is kept in deliberate order rather than allowed to grow as it pleases.", @"One of the clearest local details is that grass is kept in deliberate order rather than allowed to grow as it pleases.", @"The eye is drawn to the fact that grass is kept in deliberate order rather than allowed to grow as it pleases.", @"Grass is kept in deliberate order rather than allowed to grow as it pleases, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Underground Current"] = new WildernessGroupedTerrainFeatureSpec(
                @"Underground Current",
                @"Cave Feature",
                new[] { @"Moving water can be sensed here even when it is not fully visible.", @"At a glance, moving water can be sensed here even when it is not fully visible.", @"One of the clearest local details is that moving water can be sensed here even when it is not fully visible.", @"The eye is drawn to the fact that moving water can be sensed here even when it is not fully visible.", @"Moving water can be sensed here even when it is not fully visible, which deepens the sense of enclosure underground." },
                100.0,
                false),
            [@"Vacuum Silence"] = new WildernessGroupedTerrainFeatureSpec(
                @"Vacuum Silence",
                @"Extraterrestrial Sound",
                new[] { @"There is no natural sound here at all; vacuum denies the landscape even that small mercy.", @"Silence is absolute here, the kind imposed by emptiness rather than peace.", @"Whatever can be seen here cannot be heard through the indifferent absence of air.", @"The place is defined partly by a silence so complete it feels structural." },
                80.0,
                false),
            [@"Vent Hiss"] = new WildernessGroupedTerrainFeatureSpec(
                @"Vent Hiss",
                @"Volcanic Sound",
                new[] { @"environment{day=Somewhere nearby, hot ground or venting gas gives off a faint, persistent hiss.}{night=In the dimmer hours, the hiss of hot vents seems sharper and more intimate.}{rain=Rain striking heated surfaces wakes brief spatters and harsher whispers.}{A faint hiss from hot ground or venting gas haunts the place.}", @"environment{day=The volcanic ground is not wholly still, betrayed by the occasional hiss of escaping heat.}{night=Darkness makes the sound feel closer than the source may actually be.}{The terrain leaks its heat in quiet, needling sounds.}", @"environment{rain=Moisture meeting warm stone briefly makes the vent-noise harsher.}{dry=Dry conditions leave the hiss thin but persistent.}{The place is accompanied by the subtle voice of stored heat.}", @"environment{day=The sound is easy to miss once, but difficult to stop hearing after that.}{night=When the light fails, the hiss becomes one of the landscape's defining details.}{Even old fire remembers how to speak here.}" },
                80.0,
                false),
            [@"Void Blackness"] = new WildernessGroupedTerrainFeatureSpec(
                @"Void Blackness",
                @"Extraterrestrial Feature",
                new[] { @"The black around everything feels absolute, broken only by scattered points of light.", @"At a glance, the black around everything feels absolute, broken only by scattered points of light.", @"One of the clearest local details is that the black around everything feels absolute, broken only by scattered points of light.", @"The eye is drawn to the fact that the black around everything feels absolute, broken only by scattered points of light.", @"The black around everything feels absolute, broken only by scattered points of light, which makes the scene feel more alien and unsoftened." },
                100.0,
                false),
            [@"Wall Niches"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wall Niches",
                @"Urban Feature",
                new[] { @"Shallow recesses and wall-set storage break up what would otherwise be plain interior faces.", @"At a glance, shallow recesses and wall-set storage break up what would otherwise be plain interior faces.", @"One of the clearest local details is that shallow recesses and wall-set storage break up what would otherwise be plain interior faces.", @"The eye is drawn to the fact that shallow recesses and wall-set storage break up what would otherwise be plain interior faces.", @"Shallow recesses and wall-set storage break up what would otherwise be plain interior faces, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Water Murmur"] = new WildernessGroupedTerrainFeatureSpec(
                @"Water Murmur",
                @"Water Sound",
                new[] { @"environment{day=The nearby water speaks in a low, continuous murmur against bank, stone, or reed.}{night=At night the water's murmur becomes more noticeable than its shape.}{rain=Fresh rain thickens the murmur into a busier, less settled sound.}{There is a constant, low murmur from nearby water.}", @"environment{morning=Morning quiet makes the smaller sounds of moving water easy to pick out.}{afternoon=The water keeps up its patient murmuring under the brighter day.}{night=The stream or margin is heard before it is clearly seen.}{The place is accompanied by the talk of water.}", @"environment{spring=Seasonal fullness gives the water a fuller, more confident voice.}{autumn=Lower, cooler water sounds clearer and more precise.}{Flowing or shifting water gives the place a steady undertone.}", @"environment{day=The sound is not loud, but it is constant enough to shape the mood.}{night=Darkness leaves the ear to do more of the work, and the water obliges.}{Water nearby refuses to be entirely ignored.}" },
                80.0,
                false),
            [@"Water-Laid Silt"] = new WildernessGroupedTerrainFeatureSpec(
                @"Water-Laid Silt",
                @"Open Land Feature",
                new[] { @"Fine silt has been left behind by retreating water and now dries in a smoother skin than ordinary soil.", @"At a glance, fine silt has been left behind by retreating water and now dries in a smoother skin than ordinary soil.", @"One of the clearest local details is that fine silt has been left behind by retreating water and now dries in a smoother skin than ordinary soil.", @"The eye is drawn to the fact that fine silt has been left behind by retreating water and now dries in a smoother skin than ordinary soil.", @"Fine silt has been left behind by retreating water and now dries in a smoother skin than ordinary soil, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Waterfowl Calls"] = new WildernessGroupedTerrainFeatureSpec(
                @"Waterfowl Calls",
                @"Water Sound",
                new[] { @"environment{dawn=Waterfowl grow loud in the low light, answering one another across the margin.}{day=The odd honk, quack, or sharp cry carries over the water.}{night=At night the birds go quieter, save for sudden startled calls.}{rain=Rain turns the birds quieter and more sullen sounding.}{Waterfowl call now and then from somewhere along the water.}", @"environment{spring=The calls sound more frequent and quarrelsome in the breeding season.}{autumn=Migrating birds give the water's edge a busier, less settled voice.}{day=Bird calls carry cleanly over the open water.}{The edge of the water is punctuated by the calls of birds.}", @"environment{morning=Morning air lets every bird cry travel farther than expected.}{dusk=As dusk gathers, the birds exchange lower and less frequent notes.}{The open water gives bird calls room to spread.}", @"environment{day=The birds break the quieter water-sounds with abrupt, carrying calls.}{night=Most of the time the birds fall silent, then one suddenly voices the dark.}{There are enough waterbirds nearby to keep the place from complete quiet.}" },
                80.0,
                false),
            [@"Waterlogged Timber"] = new WildernessGroupedTerrainFeatureSpec(
                @"Waterlogged Timber",
                @"Wetland Feature",
                new[] { @"Wood left in the wet has swollen, darkened, and settled into slow decay.", @"At a glance, wood left in the wet has swollen, darkened, and settled into slow decay.", @"One of the clearest local details is that wood left in the wet has swollen, darkened, and settled into slow decay.", @"The eye is drawn to the fact that wood left in the wet has swollen, darkened, and settled into slow decay.", @"Wood left in the wet has swollen, darkened, and settled into slow decay, which reinforces the sense that water is never far away." },
                100.0,
                false),
            [@"Waterweed"] = new WildernessGroupedTerrainFeatureSpec(
                @"Waterweed",
                @"Water Feature",
                new[] { @"Submerged growth can be seen where the water clears enough to show below its surface.", @"At a glance, submerged growth can be seen where the water clears enough to show below its surface.", @"One of the clearest local details is that submerged growth can be seen where the water clears enough to show below its surface.", @"The eye is drawn to the fact that submerged growth can be seen where the water clears enough to show below its surface.", @"Submerged growth can be seen where the water clears enough to show below its surface, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Weed Through Cracks"] = new WildernessGroupedTerrainFeatureSpec(
                @"Weed Through Cracks",
                @"Road Feature",
                new[] { @"Tough little plants have found purchase in neglected seams and joins.", @"At a glance, tough little plants have found purchase in neglected seams and joins.", @"One of the clearest local details is that tough little plants have found purchase in neglected seams and joins.", @"The eye is drawn to the fact that tough little plants have found purchase in neglected seams and joins.", @"Tough little plants have found purchase in neglected seams and joins, which changes the way the route reads as much as the way it travels." },
                110.0,
                false),
            [@"Wet Earth Scent"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wet Earth Scent",
                @"Wetland Smell",
                new[] { @"environment{day=The ground smells dark and wet, as though water sits just beneath the surface.}{rain=Rain makes the wet-earth smell almost immediate and tactile.}{night=At night the smell of wet soil feels closer and heavier.}{The place smells strongly of wet earth.}", @"environment{spring=Fresh water and new growth sharpen the scent of wet soil.}{summer=Heat turns the damp ground smell fuller and heavier.}{The saturated ground lends the air a deep earthen smell.}", @"environment{day=Every softer patch of ground seems ready to give off more damp soil-scent.}{rain=Water wakens the mud and earth by smell before anything else.}{The nose confirms what the footing already suggests: wet ground rules here.}", @"environment{morning=Morning damp makes the earth-smell especially noticeable.}{afternoon=Warmth pulls a stronger mud-and-soil scent upward.}{Wet ground keeps the air rich with earth.}" },
                75.0,
                false),
            [@"Whitecaps"] = new WildernessGroupedTerrainFeatureSpec(
                @"Whitecaps",
                @"Water Feature",
                new[] { @"Whitecaps break and vanish across the wider water in irregular lines.", @"At a glance, whitecaps break and vanish across the wider water in irregular lines.", @"One of the clearest local details is that whitecaps break and vanish across the wider water in irregular lines.", @"The eye is drawn to the fact that whitecaps break and vanish across the wider water in irregular lines.", @"Whitecaps break and vanish across the wider water in irregular lines, which lets motion, wetness, or reflected light dominate the eye." },
                100.0,
                false),
            [@"Wildflower Sweetness"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wildflower Sweetness",
                @"Open Land Smell",
                new[] { @"environment{spring=Wildflowers lend the air a light, honeyed sweetness.}{summer=Warmth coaxes a sweeter, fuller floral scent from the groundcover.}{rain=Rain mutes the flowers a little, but does not silence them entirely.}{There is a faint sweetness of wildflowers in the air.}", @"environment{day=Now and then a sweeter thread of scent suggests flowering plants nearby.}{night=At night the floral smell lingers more softly, but still finds the air.}{A mild floral sweetness drifts through the place.}", @"environment{spring=The season makes the flowering growth impossible to miss by scent alone.}{autumn=Only the faintest remnant of flower-sweetness remains.}{The air is touched by the scent of bloom.}", @"environment{day=Warm air carries the delicate sweetness of scattered blossoms.}{rain=Moisture leaves the floral scent lower and closer to the ground.}{Wild bloom sweetens the harder outdoor smells.}" },
                75.0,
                false),
            [@"Wildflowers"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wildflowers",
                @"Open Land Feature",
                new[] { @"Small bursts of colour interrupt the dominant greens and browns of the surrounding growth.", @"At a glance, small bursts of colour interrupt the dominant greens and browns of the surrounding growth.", @"One of the clearest local details is that small bursts of colour interrupt the dominant greens and browns of the surrounding growth.", @"The eye is drawn to the fact that small bursts of colour interrupt the dominant greens and browns of the surrounding growth.", @"Small bursts of colour interrupt the dominant greens and browns of the surrounding growth, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Wind Keening"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind Keening",
                @"Glacial Sound",
                new[] { @"environment{day=Open cold ground gives the wind a thin, keening note.}{night=At night the wind sounds lonelier and less bounded.}{blizzard=The wind's voice rises into a hard, near-continuous keen.}{The wind takes on a high, cold keening here.}", @"environment{afternoon=The stronger daylight wind threads itself into a narrow cry across the ice and snow.}{night=In darkness the same sound seems larger and emptier.}{The exposed frozen country lets the wind sing too clearly.}", @"environment{winter=The keening of the wind suits the season too well.}{spring=Softening edges blunt the sound only a little.}{Cold exposure gives every gust a sharpened voice.}", @"environment{day=The wind does not merely blow here; it complains.}{night=The dark leaves mostly the wind and its cold note.}{The open frost-hardened land teaches the air a bleak music.}" },
                80.0,
                false),
            [@"Wind Rippled Sand"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind Rippled Sand",
                @"Desert Feature",
                new[] { @"The sand has been combed into fine ripples by recent wind.", @"At a glance, the sand has been combed into fine ripples by recent wind.", @"One of the clearest local details is that the sand has been combed into fine ripples by recent wind.", @"The eye is drawn to the fact that the sand has been combed into fine ripples by recent wind.", @"The sand has been combed into fine ripples by recent wind, which makes the country feel barer and more exposed." },
                100.0,
                false),
            [@"Wind Through Grass"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind Through Grass",
                @"Open Land Sound",
                new[] { @"environment{dawn=The first breeze of dawn runs through the grass with a soft, silken hiss.}{afternoon=Warm wind moves through the grass in long whispering passes.}{night=Night wind combs the grass into a lower, more secretive rustle.}{rain=Rain weighs the grass down, but the wind still presses a heavy rush through it.}{Wind moves through the grass with a steady, brushing whisper.}", @"environment{day=Every stronger gust leaves the grass talking to itself in dry, rippling sheets.}{dusk=As the light fails, the sound of wind in the grass seems to spread farther than the eye can follow.}{night=The grass answers the darkness with a restless, shifting murmur.}{The grass makes a papery sound whenever the wind catches it.}", @"environment{spring=Fresh growth softens the sound into a greener, wetter rustle.}{autumn=Dry stalks rattle more sharply whenever the wind freshens.}{rain=The rain blunts the lighter rustle into a heavier swish.}{Wind gives the grass a low, ceaseless voice of its own.}", @"environment{morning=The grass stirs in light, irregular whispers as the day begins.}{afternoon=Broader gusts set whole patches of grass rushing at once.}{night=The unseen movement of grass is easier to hear than to see.}{The grass keeps up a quiet susurrus around the place.}" },
                80.0,
                false),
            [@"Wind-Carved Stone"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind-Carved Stone",
                @"Rock Feature",
                new[] { @"Stone surfaces have been worn into subtler shapes by long exposure to wind and grit.", @"At a glance, stone surfaces have been worn into subtler shapes by long exposure to wind and grit.", @"One of the clearest local details is that stone surfaces have been worn into subtler shapes by long exposure to wind and grit.", @"The eye is drawn to the fact that stone surfaces have been worn into subtler shapes by long exposure to wind and grit.", @"Stone surfaces have been worn into subtler shapes by long exposure to wind and grit, which lends the landscape a harder and more structural outline." },
                100.0,
                false),
            [@"Wind-Hardened Snow"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind-Hardened Snow",
                @"Glacial Feature",
                new[] { @"The snow has been packed and shaped by wind into a harder surface than fresh fall would make.", @"At a glance, the snow has been packed and shaped by wind into a harder surface than fresh fall would make.", @"One of the clearest local details is that the snow has been packed and shaped by wind into a harder surface than fresh fall would make.", @"The eye is drawn to the fact that the snow has been packed and shaped by wind into a harder surface than fresh fall would make.", @"The snow has been packed and shaped by wind into a harder surface than fresh fall would make, which strengthens the authority of cold across the ground." },
                100.0,
                false),
            [@"Wind-Pressed Grass"] = new WildernessGroupedTerrainFeatureSpec(
                @"Wind-Pressed Grass",
                @"Open Land Feature",
                new[] { @"The prevailing wind has laid the grass in one direction long enough to become visible at a glance.", @"At a glance, the prevailing wind has laid the grass in one direction long enough to become visible at a glance.", @"One of the clearest local details is that the prevailing wind has laid the grass in one direction long enough to become visible at a glance.", @"The eye is drawn to the fact that the prevailing wind has laid the grass in one direction long enough to become visible at a glance.", @"The prevailing wind has laid the grass in one direction long enough to become visible at a glance, which keeps the ground from feeling empty even when it is open." },
                100.0,
                false),
            [@"Window Light"] = new WildernessGroupedTerrainFeatureSpec(
                @"Window Light",
                @"Urban Feature",
                new[] { @"Whatever light enters here does so through a clear and deliberate opening rather than by accident.", @"At a glance, whatever light enters here does so through a clear and deliberate opening rather than by accident.", @"One of the clearest local details is that whatever light enters here does so through a clear and deliberate opening rather than by accident.", @"The eye is drawn to the fact that whatever light enters here does so through a clear and deliberate opening rather than by accident.", @"Whatever light enters here does so through a clear and deliberate opening rather than by accident, which makes the place feel shaped by use and maintenance." },
                100.0,
                false),
            [@"Worn Furnishings"] = new WildernessGroupedTerrainFeatureSpec(
                @"Worn Furnishings",
                @"Urban Feature",
                new[] { @"Furniture and fittings show the softened edges and polished spots left by regular use.", @"At a glance, furniture and fittings show the softened edges and polished spots left by regular use.", @"One of the clearest local details is that furniture and fittings show the softened edges and polished spots left by regular use.", @"The eye is drawn to the fact that furniture and fittings show the softened edges and polished spots left by regular use.", @"Furniture and fittings show the softened edges and polished spots left by regular use, which makes the place feel shaped by use and maintenance." },
                100.0,
                false)
        };
    }

    private static IReadOnlyDictionary<string, string> BuildWildernessGroupedTerrainTerrainDomainLookup()
    {
        return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [@"Residence"] = @"UrbanDomestic",
            [@"Bedroom"] = @"UrbanDomestic",
            [@"Kitchen"] = @"UrbanDomestic",
            [@"Bathroom"] = @"UrbanDomestic",
            [@"Living Room"] = @"UrbanDomestic",
            [@"Hallway"] = @"UrbanCirculation",
            [@"Hall"] = @"UrbanCirculation",
            [@"Barracks"] = @"UrbanCommunal",
            [@"Gymnasium"] = @"UrbanCommunal",
            [@"Shopfront"] = @"UrbanCommercial",
            [@"Indoor Market"] = @"UrbanCommercial",
            [@"Underground Market"] = @"UrbanCommercial",
            [@"Marketplace"] = @"UrbanCommercial",
            [@"Outdoor Mall"] = @"UrbanCommercial",
            [@"Workshop"] = @"UrbanWork",
            [@"Office"] = @"UrbanWork",
            [@"Factory"] = @"UrbanWork",
            [@"Warehouse"] = @"UrbanWork",
            [@"Garage"] = @"UrbanWork",
            [@"Underground Garage"] = @"UrbanWork",
            [@"Barn"] = @"UrbanWork",
            [@"Cell"] = @"UrbanConfinement",
            [@"Dank Cell"] = @"UrbanConfinement",
            [@"Dungeon"] = @"UrbanConfinement",
            [@"Grotto"] = @"UrbanConfinement",
            [@"Cellar"] = @"UrbanConfinement",
            [@"Baths"] = @"UrbanWater",
            [@"Indoor Pool"] = @"UrbanWater",
            [@"Indoor Spring"] = @"UrbanWater",
            [@"Rooftop"] = @"UrbanRoofDefense",
            [@"Gatehouse"] = @"UrbanRoofDefense",
            [@"Battlement"] = @"UrbanRoofDefense",
            [@"Ghetto Street"] = @"UrbanStreetPoor",
            [@"Slum Street"] = @"UrbanStreetPoor",
            [@"Poor Street"] = @"UrbanStreetPoor",
            [@"Alleyway"] = @"UrbanStreetPoor",
            [@"Urban Street"] = @"UrbanStreetCommon",
            [@"Suburban Street"] = @"UrbanStreetCommon",
            [@"Wealthy Street"] = @"UrbanStreetWealthyRural",
            [@"Village Street"] = @"UrbanStreetWealthyRural",
            [@"Rural Street"] = @"UrbanStreetWealthyRural",
            [@"Courtyard"] = @"UrbanCivicOpen",
            [@"Park"] = @"UrbanCivicOpen",
            [@"Garden"] = @"UrbanCivicOpen",
            [@"Lawn"] = @"UrbanCivicOpen",
            [@"Showground"] = @"UrbanCivicOpen",
            [@"Forum"] = @"UrbanCivicOpen",
            [@"Public Square"] = @"UrbanCivicOpen",
            [@"Garbage Dump"] = @"UrbanRefuse",
            [@"Midden Heap"] = @"UrbanRefuse",
            [@"Animal Trail"] = @"RoadTrail",
            [@"Trail"] = @"RoadTrail",
            [@"Dirt Road"] = @"RoadUnpaved",
            [@"Compacted Dirt Road"] = @"RoadUnpaved",
            [@"Gravel Road"] = @"RoadUnpaved",
            [@"Cobblestone Road"] = @"RoadPaved",
            [@"Asphalt Road"] = @"RoadPaved",
            [@"Grasslands"] = @"OpenGrass",
            [@"Steppe"] = @"OpenGrass",
            [@"Shortgrass Prairie"] = @"OpenGrass",
            [@"Tallgrass Prairie"] = @"OpenGrass",
            [@"Heath"] = @"OpenGrass",
            [@"Pasture"] = @"OpenGrass",
            [@"Meadow"] = @"OpenGrass",
            [@"Field"] = @"OpenGrass",
            [@"Savannah"] = @"Savannah",
            [@"Shrublands"] = @"Shrubland",
            [@"Chaparral"] = @"Shrubland",
            [@"Bramble"] = @"Shrubland",
            [@"Tundra"] = @"Tundra",
            [@"Flood Plain"] = @"Floodplain",
            [@"Badlands"] = @"BadlandsSalt",
            [@"Salt Flat"] = @"BadlandsSalt",
            [@"Hills"] = @"RollingUpland",
            [@"Foothills"] = @"RollingUpland",
            [@"Mound"] = @"RollingUpland",
            [@"Drumlin"] = @"RollingUpland",
            [@"Knoll"] = @"RollingUpland",
            [@"Moor"] = @"RollingUpland",
            [@"Tell"] = @"RollingUpland",
            [@"Butte"] = @"Tableland",
            [@"Kuppe"] = @"Tableland",
            [@"Mesa"] = @"Tableland",
            [@"Plateau"] = @"Tableland",
            [@"Escarpment"] = @"Tableland",
            [@"Canyon"] = @"Cutland",
            [@"Valley"] = @"Cutland",
            [@"Vale"] = @"Cutland",
            [@"Dell"] = @"Cutland",
            [@"Glen"] = @"Cutland",
            [@"Strath"] = @"Cutland",
            [@"Combe"] = @"Cutland",
            [@"Ravine"] = @"Cutland",
            [@"Gorge"] = @"Cutland",
            [@"Gully"] = @"Cutland",
            [@"Dunes"] = @"Dunescape",
            [@"Scree Slope"] = @"MountainCliff",
            [@"Talus Field"] = @"MountainCliff",
            [@"Mountainside"] = @"MountainCliff",
            [@"Mountain Pass"] = @"MountainCliff",
            [@"Mountain Ridge"] = @"MountainCliff",
            [@"Cliff Face"] = @"MountainCliff",
            [@"Cliff Edge"] = @"MountainCliff",
            [@"Broadleaf Forest"] = @"ForestBroadleaf",
            [@"Boreal Forest"] = @"ForestConifer",
            [@"Temperate Coniferous Forest"] = @"ForestConifer",
            [@"Temperate Rainforest"] = @"Rainforest",
            [@"Tropical Rainforest"] = @"Rainforest",
            [@"Plantation Forest"] = @"ManagedWoodland",
            [@"Orchard"] = @"ManagedWoodland",
            [@"Grove"] = @"ManagedWoodland",
            [@"Woodland"] = @"ManagedWoodland",
            [@"Bog"] = @"WetlandFresh",
            [@"Fen"] = @"WetlandFresh",
            [@"Marsh"] = @"WetlandFresh",
            [@"Wetland"] = @"WetlandFresh",
            [@"Swamp Forest"] = @"WetlandFresh",
            [@"Tropical Freshwater Swamp"] = @"WetlandFresh",
            [@"Temperate Freshwater Swamp"] = @"WetlandFresh",
            [@"Salt Marsh"] = @"WetlandSaline",
            [@"Mangrove Swamp"] = @"WetlandSaline",
            [@"Sandy Desert"] = @"DesertSand",
            [@"Rocky Desert"] = @"DesertRock",
            [@"Coastal Desert"] = @"DesertRock",
            [@"Oasis"] = @"Oasis",
            [@"Volcanic Plain"] = @"Volcanic",
            [@"Lava Field"] = @"Volcanic",
            [@"Caldera"] = @"Volcanic",
            [@"Crater"] = @"Volcanic",
            [@"Glacier"] = @"Glacial",
            [@"Ice Field"] = @"Glacial",
            [@"Snowfield"] = @"Glacial",
            [@"Cave Entrance"] = @"CaveDry",
            [@"Cave"] = @"CaveDry",
            [@"Cavern"] = @"CaveDry",
            [@"Cave Pool"] = @"CaveWater",
            [@"Underground Water"] = @"CaveWater",
            [@"Sandy Beach"] = @"Shoreline",
            [@"Rocky Beach"] = @"Shoreline",
            [@"Beachrock"] = @"Shoreline",
            [@"Riverbank"] = @"Shoreline",
            [@"Lake Shore"] = @"Shoreline",
            [@"Mudflat"] = @"Shoreline",
            [@"Ocean Shallows"] = @"CoastalWater",
            [@"Ocean Surf"] = @"CoastalWater",
            [@"Bay"] = @"CoastalWater",
            [@"Lagoon"] = @"CoastalWater",
            [@"Cove"] = @"CoastalWater",
            [@"Tide Pool"] = @"CoastalWater",
            [@"Shoal"] = @"CoastalWater",
            [@"Coral Reef"] = @"CoastalWater",
            [@"Reef"] = @"CoastalWater",
            [@"Sound"] = @"CoastalWater",
            [@"Estuary"] = @"CoastalWater",
            [@"Shallow River"] = @"RiverWater",
            [@"River"] = @"RiverWater",
            [@"Deep River"] = @"RiverWater",
            [@"Shallow Lake"] = @"LakeWater",
            [@"Lake"] = @"LakeWater",
            [@"Deep Lake"] = @"LakeWater",
            [@"Ocean"] = @"OpenOcean",
            [@"Deep Ocean"] = @"OpenOcean",
            [@"Moon Surface"] = @"Lunar",
            [@"Lunar Mare"] = @"Lunar",
            [@"Lunar Highlands"] = @"Lunar",
            [@"Lunar Crater"] = @"Lunar",
            [@"Asteroid Surface"] = @"Asteroid",
            [@"Orbital Space"] = @"NearSpace",
            [@"Interplanetary Space"] = @"NearSpace",
            [@"Interstellar Space"] = @"DeepSpace",
            [@"Intergalactic Space"] = @"DeepSpace"
        };
    }

    private static IReadOnlyDictionary<string, string[]> BuildWildernessGroupedTerrainDomainTerrainNames()
    {
        return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [@"UrbanDomestic"] = new[] { @"Residence", @"Bedroom", @"Kitchen", @"Bathroom", @"Living Room" },
            [@"UrbanCirculation"] = new[] { @"Hallway", @"Hall" },
            [@"UrbanCommunal"] = new[] { @"Barracks", @"Gymnasium" },
            [@"UrbanCommercial"] = new[] { @"Shopfront", @"Indoor Market", @"Underground Market", @"Marketplace", @"Outdoor Mall" },
            [@"UrbanWork"] = new[] { @"Workshop", @"Office", @"Factory", @"Warehouse", @"Garage", @"Underground Garage", @"Barn" },
            [@"UrbanConfinement"] = new[] { @"Cell", @"Dank Cell", @"Dungeon", @"Grotto", @"Cellar" },
            [@"UrbanWater"] = new[] { @"Baths", @"Indoor Pool", @"Indoor Spring" },
            [@"UrbanRoofDefense"] = new[] { @"Rooftop", @"Gatehouse", @"Battlement" },
            [@"UrbanStreetPoor"] = new[] { @"Ghetto Street", @"Slum Street", @"Poor Street", @"Alleyway" },
            [@"UrbanStreetCommon"] = new[] { @"Urban Street", @"Suburban Street" },
            [@"UrbanStreetWealthyRural"] = new[] { @"Wealthy Street", @"Village Street", @"Rural Street" },
            [@"UrbanCivicOpen"] = new[] { @"Courtyard", @"Park", @"Garden", @"Lawn", @"Showground", @"Forum", @"Public Square" },
            [@"UrbanRefuse"] = new[] { @"Garbage Dump", @"Midden Heap" },
            [@"RoadTrail"] = new[] { @"Animal Trail", @"Trail" },
            [@"RoadUnpaved"] = new[] { @"Dirt Road", @"Compacted Dirt Road", @"Gravel Road" },
            [@"RoadPaved"] = new[] { @"Cobblestone Road", @"Asphalt Road" },
            [@"OpenGrass"] = new[] { @"Grasslands", @"Steppe", @"Shortgrass Prairie", @"Tallgrass Prairie", @"Heath", @"Pasture", @"Meadow", @"Field" },
            [@"Savannah"] = new[] { @"Savannah" },
            [@"Shrubland"] = new[] { @"Shrublands", @"Chaparral", @"Bramble" },
            [@"Tundra"] = new[] { @"Tundra" },
            [@"Floodplain"] = new[] { @"Flood Plain" },
            [@"BadlandsSalt"] = new[] { @"Badlands", @"Salt Flat" },
            [@"RollingUpland"] = new[] { @"Hills", @"Foothills", @"Mound", @"Drumlin", @"Knoll", @"Moor", @"Tell" },
            [@"Tableland"] = new[] { @"Butte", @"Kuppe", @"Mesa", @"Plateau", @"Escarpment" },
            [@"Cutland"] = new[] { @"Canyon", @"Valley", @"Vale", @"Dell", @"Glen", @"Strath", @"Combe", @"Ravine", @"Gorge", @"Gully" },
            [@"Dunescape"] = new[] { @"Dunes" },
            [@"MountainCliff"] = new[] { @"Scree Slope", @"Talus Field", @"Mountainside", @"Mountain Pass", @"Mountain Ridge", @"Cliff Face", @"Cliff Edge" },
            [@"ForestBroadleaf"] = new[] { @"Broadleaf Forest" },
            [@"ForestConifer"] = new[] { @"Boreal Forest", @"Temperate Coniferous Forest" },
            [@"Rainforest"] = new[] { @"Temperate Rainforest", @"Tropical Rainforest" },
            [@"ManagedWoodland"] = new[] { @"Plantation Forest", @"Orchard", @"Grove", @"Woodland" },
            [@"WetlandFresh"] = new[] { @"Bog", @"Fen", @"Marsh", @"Wetland", @"Swamp Forest", @"Tropical Freshwater Swamp", @"Temperate Freshwater Swamp" },
            [@"WetlandSaline"] = new[] { @"Salt Marsh", @"Mangrove Swamp" },
            [@"DesertSand"] = new[] { @"Sandy Desert" },
            [@"DesertRock"] = new[] { @"Rocky Desert", @"Coastal Desert" },
            [@"Oasis"] = new[] { @"Oasis" },
            [@"Volcanic"] = new[] { @"Volcanic Plain", @"Lava Field", @"Caldera", @"Crater" },
            [@"Glacial"] = new[] { @"Glacier", @"Ice Field", @"Snowfield" },
            [@"CaveDry"] = new[] { @"Cave Entrance", @"Cave", @"Cavern" },
            [@"CaveWater"] = new[] { @"Cave Pool", @"Underground Water" },
            [@"Shoreline"] = new[] { @"Sandy Beach", @"Rocky Beach", @"Beachrock", @"Riverbank", @"Lake Shore", @"Mudflat" },
            [@"CoastalWater"] = new[] { @"Ocean Shallows", @"Ocean Surf", @"Bay", @"Lagoon", @"Cove", @"Tide Pool", @"Shoal", @"Coral Reef", @"Reef", @"Sound", @"Estuary" },
            [@"RiverWater"] = new[] { @"Shallow River", @"River", @"Deep River" },
            [@"LakeWater"] = new[] { @"Shallow Lake", @"Lake", @"Deep Lake" },
            [@"OpenOcean"] = new[] { @"Ocean", @"Deep Ocean" },
            [@"Lunar"] = new[] { @"Moon Surface", @"Lunar Mare", @"Lunar Highlands", @"Lunar Crater" },
            [@"Asteroid"] = new[] { @"Asteroid Surface" },
            [@"NearSpace"] = new[] { @"Orbital Space", @"Interplanetary Space" },
            [@"DeepSpace"] = new[] { @"Interstellar Space", @"Intergalactic Space" }
        };
    }
}
