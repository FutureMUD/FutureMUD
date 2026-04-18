#nullable enable

using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DatabaseSeeder.Seeders;

public partial class CoreDataSeeder
{
    private static readonly (string Name, string Parent)[] StockTerrainTagDefinitions =
    [
        ("Terrain", ""),
        ("Wild", "Terrain"),
        ("Human Influenced", "Terrain"),
        ("Urban", "Human Influenced"),
        ("Rural", "Human Influenced"),
        ("Public", "Urban"),
        ("Private", "Urban"),
        ("Commercial", "Urban"),
        ("Residential", "Urban"),
        ("Administrative", "Urban"),
        ("Industrial", "Urban"),
        ("Natural", "Urban"),
        ("Diggable Soil", "Terrain"),
        ("Foragable Clay", "Terrain"),
        ("Foragable Sand", "Terrain"),
        ("Terrestrial", "Wild"),
        ("Riparian", "Wild"),
        ("Littoral", "Wild"),
        ("Aquatic", "Wild"),
        ("Extraterrestrial", "Wild"),
        ("Lunar", "Extraterrestrial"),
        ("Space", "Extraterrestrial"),
        ("Vacuum", "Terrain"),
        ("Arid", "Terrain"),
        ("Glacial", "Terrain"),
        ("Volcanic", "Terrain"),
        ("Wetland", "Terrain")
    ];

    internal static IReadOnlyCollection<string> StockTerrainTagNamesForTesting =>
        StockTerrainTagDefinitions.Select(x => x.Name).ToArray();

    internal static void SeedTerrainFoundationsForTesting(FuturemudDatabaseContext context)
    {
        SeedTerrainFoundationsCore(context);
    }

    private void SeedTerrainFoundations(FuturemudDatabaseContext context)
    {
        SeedTerrainFoundationsCore(context);
    }

    private static void SeedTerrainFoundationsCore(FuturemudDatabaseContext context)
    {
        DictionaryWithDefault<string, Tag> tags = context.Tags.ToDictionaryWithDefault(x => x.Name, x => x, StringComparer.OrdinalIgnoreCase);
        foreach ((string? name, string? parent) in StockTerrainTagDefinitions)
        {
            AddTerrainTag(context, tags, name, parent);
        }

        context.SaveChanges();
        SeedStockTerrainCatalogue(context, tags);
    }

    private static void AddTerrainTag(FuturemudDatabaseContext context, DictionaryWithDefault<string, Tag> tags,
        string name, string parent)
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

    internal static void SeedStockTerrainCatalogue(FuturemudDatabaseContext context, DictionaryWithDefault<string, Tag> tagLookup,
        ICollection<string>? errors = null)
    {
        if (context.Terrains.Count() > 1)
        {
            errors?.Add("Terrains were already installed, so did not add any new data.");
            return;
        }

        context.Terrains.Find(1L)!.DefaultTerrain = false;

        void AddTerrain(string name, string behaviour, double movementRate, double staminaCost,
            Difficulty hideDifficulty, Difficulty spotDifficulty, string? atmosphere, CellOutdoorsType outdoorsType,
            Color editorColour, string editorText = null, bool isdefault = false, IEnumerable<string>? tags = null)
        {
            context.Terrains.Add(new Terrain
            {
                Name = name,
                TerrainBehaviourMode = behaviour,
                MovementRate = movementRate,
                StaminaCost = staminaCost,
                HideDifficulty = (int)hideDifficulty,
                SpotDifficulty = (int)spotDifficulty,
                AtmosphereId = context.Gases.FirstOrDefault(x => x.Name == atmosphere)?.Id,
                AtmosphereType = "Gas",
                InfectionMultiplier = 1.0,
                InfectionType = (int)InfectionType.Simple,
                InfectionVirulence = (int)Difficulty.Normal,
                ForagableProfileId = 0,
                DefaultTerrain = isdefault,
                TerrainANSIColour = "7",
                TerrainEditorColour = $"#{editorColour.R:X2}{editorColour.G:X2}{editorColour.B:X2}",
                TerrainEditorText = editorText,
                DefaultCellOutdoorsType = (int)outdoorsType,
                TagInformation = tags is not null ?
                    tags.SelectNotNull(x => tagLookup[x]?.Id.ToString("F0")).ListToCommaSeparatedValues() :
                    ""
            });
            context.SaveChanges();
        }

        Liquid poolwater = context.Liquids.First(x => x.Name == "pool water");
        Liquid springwater = context.Liquids.First(x => x.Name == "spring water");
        Liquid saltwater = context.Liquids.First(x => x.Name == "salt water");
        Liquid brackishwater = context.Liquids.First(x => x.Name == "brackish water");
        Liquid riverwater = context.Liquids.First(x => x.Name == "river water");
        Liquid lakewater = context.Liquids.First(x => x.Name == "lake water");
        Liquid swampwater = context.Liquids.First(x => x.Name == "swamp water");

        #region Urban

        AddTerrain("Residence", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.CornflowerBlue, "Re", true,
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Bedroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.MediumPurple, "Br",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Kitchen", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.Orange, "Ki",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Bathroom", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SkyBlue, "To",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Living Room", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SeaGreen, "LR",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Hallway", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.CadetBlue, "Hw",
                tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Hall", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Teal, "Ha", tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Barracks", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.OliveDrab, "Bk", tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Gymnasium", "indoors", 0.5, 3.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Goldenrod, "Gy", tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Shopfront", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SandyBrown, "Sf",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Workshop", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.SaddleBrown, "Ws",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Office", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.LightSteelBlue, "Of",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Factory", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.Silver, "Fa",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Warehouse", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.DarkGray, "Wh",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Indoor Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.Plum, "Im",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Underground Market", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsWithWindows, Color.DarkOrchid, "Um",
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsWithWindows, Color.DimGray, "Ga",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Underground Garage", "indoors", 0.5, 3.0, Difficulty.ExtremelyEasy, Difficulty.Easy,
                "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.DarkSlateGray, "Ug",
                tags: ["Urban", "Industrial", "Private"]);
        AddTerrain("Barn", "indoors", 0.5, 3.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Indoors, Color.Brown, "Bn", tags: ["Rural"]);
        AddTerrain("Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.LightSlateGray, "Ce",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Dank Cell", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.Gray, "Dc",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Dungeon", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.Indigo, "Du",
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Grotto", "cave", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsNoLight, Color.DarkSlateBlue, "Gr", tags: ["Rural"]);
        AddTerrain("Cellar", "indoors", 0.5, 3.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
                 CellOutdoorsType.IndoorsNoLight, Color.BurlyWood, "Cl",
                 tags: ["Urban", "Residential", "Private"]);
        AddTerrain("Baths", "indoors", 0.5, 3.0, Difficulty.ExtremelyHard, Difficulty.ExtremelyEasy,
                 "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.LightBlue, "Bt",
                 tags: ["Urban", "Aquatic", "Commercial", "Public"]);
        AddTerrain("Indoor Pool", $"shallowwater {poolwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
                 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.DeepSkyBlue, "IP",
                 tags: ["Urban", "Aquatic", "Private"]);
        AddTerrain("Indoor Spring", $"shallowwater {springwater.Id}", 0.5, 5.0, Difficulty.ExtremelyHard,
                 Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Indoors, Color.MediumAquamarine, "IS", tags: ["Rural", "Aquatic"]);

        AddTerrain("Rooftop", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.DarkSlateGray, tags: ["Urban", "Private"]);
        AddTerrain("Ghetto Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban", "Public"]);
        AddTerrain("Slum Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.Gray, tags: ["Urban", "Public"]);
        AddTerrain("Poor Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Urban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGray, tags: ["Urban", "Public"]);
        AddTerrain("Suburban Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightSlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Wealthy Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Gainsboro, tags: ["Urban", "Public"]);
        AddTerrain("Village Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGray, tags: ["Rural"]);
        AddTerrain("Rural Street", "outdoors", 0.75, 7.0, Difficulty.Easy, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.WhiteSmoke, tags: ["Rural"]);

        AddTerrain("Marketplace", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.VeryEasy, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Courtyard", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Private"]);
        AddTerrain("Park", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Public", "Diggable Soil"]);
        AddTerrain("Garden", "trees", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
        AddTerrain("Lawn", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Urban", "Natural", "Private", "Diggable Soil"]);
        AddTerrain("Showground", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Automatic,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen,
                tags: ["Urban", "Commercial", "Public", "Diggable Soil"]);
        AddTerrain("Forum", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Public Square", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Administrative", "Public"]);
        AddTerrain("Outdoor Mall", "outdoors", 1.0, 7.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Commercial", "Public"]);
        AddTerrain("Alleyway", "outdoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Public"]);
        AddTerrain("Garbage Dump", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
        AddTerrain("Midden Heap", "outdoors", 1.5, 10.0, Difficulty.VeryEasy, Difficulty.VeryEasy,
                "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SlateGray,
                tags: ["Urban", "Industrial", "Private", "Diggable Soil"]);
        AddTerrain("Gatehouse", "indoors", 1.0, 7.0, Difficulty.Easy, Difficulty.Trivial, "Breathable Atmosphere",
                CellOutdoorsType.IndoorsClimateExposed, Color.SlateGray,
                tags: ["Urban", "Administrative", "Private"]);
        AddTerrain("Battlement", "outdoors", 1.0, 7.0, Difficulty.VeryHard, Difficulty.Trivial, "Breathable Atmosphere",
                CellOutdoorsType.Outdoors, Color.SlateGray, tags: ["Urban", "Administrative", "Private"]);

        #endregion

        #region Roads

        AddTerrain("Animal Trail", "outdoors", 1.75, 10.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Trail", "outdoors", 1.6, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Dirt Road", "outdoors", 1.5, 10.0, Difficulty.Hard, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Compacted Dirt Road", "outdoors", 1.4, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Gravel Road", "outdoors", 1.3, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Cobblestone Road", "outdoors", 1.2, 10.0, Difficulty.ExtremelyHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Rural"]);
        AddTerrain("Asphalt Road", "outdoors", 1.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Urban"]);

        #endregion

        #region Terrestrial

        AddTerrain("Grasslands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Savannah", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Shrublands", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Steppe", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Shortgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tallgrass Prairie", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Heath", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Pasture", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Meadow", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Field", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Tundra", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Flood Plain", "outdoors", 2.0, 15.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightGreen, tags: ["Terrestrial", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Chaparral", "outdoors", 2.5, 18.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.OliveDrab, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Badlands", "outdoors", 3.5, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SandyBrown, tags: ["Terrestrial", "Arid"]);
        AddTerrain("Salt Flat", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Linen, tags: ["Terrestrial", "Arid", "Foragable Sand"]);

        AddTerrain("Hills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Foothills", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mound", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Drumlin", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Butte", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Kuppe", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mesa", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Canyon", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Knoll", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Moor", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tell", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Dunes", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.OrangeRed, tags: ["Terrestrial", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Plateau", "outdoors", 3.0, 15.0, Difficulty.Easy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Peru, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Escarpment", "cliff", 4.5, 22.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.IndianRed, tags: ["Terrestrial"]);
        AddTerrain("Scree Slope", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkKhaki, tags: ["Terrestrial"]);
        AddTerrain("Talus Field", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGoldenrod, tags: ["Terrestrial"]);

        AddTerrain("Mountainside", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mountain Pass", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Mountain Ridge", "outdoors", 4.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
        AddTerrain("Cliff Face", "cliff", 5.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);
        AddTerrain("Cliff Edge", "outdoors", 5.0, 20.0, Difficulty.ExtremelyEasy, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Red, tags: ["Terrestrial"]);

        AddTerrain("Valley", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Vale", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Dell", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Glen", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Strath", "trees", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Combe", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Ravine", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Gorge", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Gully", "outdoors", 3.0, 10.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Beige, tags: ["Terrestrial", "Diggable Soil"]);

        AddTerrain("Boreal Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Broadleaf Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Temperate Coniferous Forest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Temperate Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Tropical Rainforest", "talltrees", 3.5, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Bramble", "talltrees", 3.0, 20.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Terrestrial", "Diggable Soil"]);
        AddTerrain("Plantation Forest", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Orchard", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Grove", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);
        AddTerrain("Woodland", "talltrees", 3.0, 10.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkGreen, tags: ["Rural", "Diggable Soil"]);

        AddTerrain("Bog", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Fen", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.MediumPurple, tags: ["Terrestrial", "Wetland", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Marsh", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkMagenta, tags: ["Terrestrial", "Wetland", "Riparian", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Salt Marsh", $"shallowwater {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Littoral", "Wetland", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Mangrove Swamp", $"shallowwatertrees {brackishwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Littoral", "Wetland", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Wetland", $"shallowwater {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy, Difficulty.ExtremelyEasy,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Swamp Forest", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Tropical Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);
        AddTerrain("Temperate Freshwater Swamp", $"shallowwatertrees {swampwater.Id}", 4.0, 30.0, Difficulty.VeryEasy,
            Difficulty.ExtremelyEasy, "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Purple, tags: ["Terrestrial", "Wetland", "Diggable Soil", "Foragable Clay"]);

        AddTerrain("Sandy Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Rocky Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Coastal Desert", "outdoors", 4.0, 20.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Terrestrial", "Arid", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Oasis", "trees", 2.0, 12.0, Difficulty.Easy, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.MediumSeaGreen, tags: ["Terrestrial", "Arid", "Diggable Soil"]);
        AddTerrain("Volcanic Plain", "outdoors", 3.5, 20.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Firebrick, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Lava Field", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkRed, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Caldera", "outdoors", 3.5, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.IndianRed, tags: ["Terrestrial", "Volcanic"]);
        AddTerrain("Crater", "outdoors", 3.5, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.BurlyWood, tags: ["Terrestrial"]);
        AddTerrain("Glacier", "outdoors", 4.0, 22.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.LightCyan, tags: ["Terrestrial", "Glacial"]);
        AddTerrain("Ice Field", "outdoors", 3.0, 18.0, Difficulty.Hard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.AliceBlue, tags: ["Terrestrial", "Glacial"]);
        AddTerrain("Snowfield", "outdoors", 3.0, 18.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.WhiteSmoke, tags: ["Terrestrial", "Glacial"]);

        AddTerrain("Cave Entrance", "indoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.IndoorsClimateExposed, Color.LightGreen, tags: ["Terrestrial"]);
        AddTerrain("Cave", "cave", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial"]);
        AddTerrain("Cavern", "cave", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.IndoorsNoLight, Color.DarkSeaGreen, tags: ["Terrestrial"]);
        AddTerrain("Cave Pool", $"shallowwatercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
            Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial", "Aquatic"]);
        AddTerrain("Underground Water", $"deepwatercave {springwater.Id}", 3.0, 10.0, Difficulty.Normal,
            Difficulty.Automatic, "Breathable Atmosphere", CellOutdoorsType.IndoorsNoLight, Color.LightGreen, tags: ["Terrestrial", "Aquatic"]);

        #endregion

        #region Water

        AddTerrain("Sandy Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Sand"]);
        AddTerrain("Rocky Beach", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
        AddTerrain("Beachrock", "outdoors", 4.0, 20.0, Difficulty.Insane, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral"]);
        AddTerrain("Riverbank", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic, "Breathable Atmosphere",
            CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Riparian", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Lake Shore", "outdoors", 3.0, 20.0, Difficulty.Normal, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.Yellow, tags: ["Littoral", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);

        AddTerrain("Ocean Shallows", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Littoral", "Foragable Sand"]);
        AddTerrain("Ocean Surf", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Littoral", "Foragable Sand"]);
        AddTerrain("Ocean", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Mudflat", "outdoors", 4.0, 30.0, Difficulty.VeryHard, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.SaddleBrown, tags: ["Littoral", "Wetland", "Diggable Soil", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Bay", $"water {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Lagoon", $"water {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Cove", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Tide Pool", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Shoal", $"shallowwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Coral Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Reef", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Sound", $"deepwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);
        AddTerrain("Estuary", $"shallowwater {brackishwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Shallow River", $"shallowwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("River", $"water {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep River", $"deepwater {riverwater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Shallow Lake", $"shallowwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Lake", $"water {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep Lake", $"deepwater {lakewater.Id}", 3.0, 10.0, Difficulty.Insane, Difficulty.Automatic,
            "Breathable Atmosphere", CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Clay", "Foragable Sand"]);
        AddTerrain("Deep Ocean", $"verydeepunderwater {saltwater.Id}", 3.0, 10.0, Difficulty.Insane,
            Difficulty.Automatic, null, CellOutdoorsType.Outdoors, Color.DarkBlue, tags: ["Aquatic", "Foragable Sand"]);

        #endregion

        #region Extraterrestrial

        AddTerrain("Moon Surface", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.LightSlateGray, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Lunar Mare", "outdoors", 2.5, 18.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.DimGray, tags: ["Extraterrestrial", "Lunar", "Vacuum", "Volcanic"]);
        AddTerrain("Lunar Highlands", "outdoors", 3.0, 20.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Gainsboro, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Lunar Crater", "outdoors", 3.5, 22.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Gray, tags: ["Extraterrestrial", "Lunar", "Vacuum"]);
        AddTerrain("Asteroid Surface", "outdoors", 4.0, 25.0, Difficulty.Hard, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.DarkSlateGray, tags: ["Extraterrestrial", "Vacuum"]);
        AddTerrain("Orbital Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"]);
        AddTerrain("Interplanetary Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"]);
        AddTerrain("Interstellar Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"]);
        AddTerrain("Intergalactic Space", "outdoors", 1.0, 5.0, Difficulty.Insane, Difficulty.Automatic, null,
            CellOutdoorsType.Outdoors, Color.Black, tags: ["Extraterrestrial", "Space", "Vacuum"]);

        #endregion

        #region Autobuilders

        context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
        {
            Name = "Blank",
            TemplateType = "simple",
            Definition = @"<Template>
	<RoomName>An Unnamed Room</RoomName>
	<CellDescription>This room has not been given a description.</CellDescription>
	<ShowCommandByline>Create a blank, undescribed room</ShowCommandByline>
</Template>"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Rectangle",
            Definition = "<Definition/>",
            TemplateType = "rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Rectangle Diagonals",
            Definition = "<Definition/>",
            TemplateType = "rectangle diagonals"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Terrain Rectangle",
            Definition = "<Definition connect_diagonals=\"false\"/>",
            TemplateType = "terrain rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Terrain Rectangle Diagonals",
            Definition = "<Definition connect_diagonals=\"true\"/>",
            TemplateType = "terrain rectangle"
        });

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Feature Rectangle",
            Definition = "<Definition connect_diagonals=\"false\"/>",
            TemplateType = "terrain feature rectangle"
        });
        context.SaveChanges();

        context.AutobuilderAreaTemplates.Add(new AutobuilderAreaTemplate
        {
            Name = "Feature Rectangle Diagonals",
            Definition = "<Definition connect_diagonals=\"true\"/>",
            TemplateType = "terrain feature rectangle"
        });
        context.SaveChanges();

        //context.AutobuilderRoomTemplates.Add(new AutobuilderRoomTemplate
        //{
        //    Name = "Variable",
        //    TemplateType = "room random description",
        //    Definition = @""
        //});
        //context.SaveChanges();

        #endregion
    }
}
