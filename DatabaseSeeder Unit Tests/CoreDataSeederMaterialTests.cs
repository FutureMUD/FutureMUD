#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CoreDataSeederMaterialTests
{
    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static void SeedMaterials(FuturemudDatabaseContext context)
    {
        CoreDataSeeder seeder = new();
        typeof(CoreDataSeeder)
            .GetMethod("SeedMaterials", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(seeder, [context]);
    }

    private static void SeedMaterialsBase(FuturemudDatabaseContext context)
    {
        CoreDataSeeder seeder = new();
        typeof(CoreDataSeeder)
            .GetMethod("SeedMaterialsBase", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(seeder, [context]);
    }

    private static MudSharp.Models.Liquid BuildPlaceholderLiquid(string name)
    {
        return new MudSharp.Models.Liquid
        {
            Name = name,
            Description = name,
            LongDescription = name,
            TasteText = name,
            VagueTasteText = name,
            SmellText = name,
            VagueSmellText = name,
            DisplayColour = "blue",
            DampDescription = name,
            WetDescription = name,
            DrenchedDescription = name,
            DampShortDescription = name,
            WetShortDescription = name,
            DrenchedShortDescription = name,
            SurfaceReactionInfo = string.Empty
        };
    }

    private static Mock<IFuturemud> CreateGameworld(All<ISolid> materials, FuturemudDatabaseContext context)
    {
        All<ITag> tags = new();
        foreach (MudSharp.Models.Tag tag in context.Tags)
        {
            Mock<ITag> mockTag = new();
            mockTag.SetupGet(x => x.Id).Returns(tag.Id);
            mockTag.SetupGet(x => x.Name).Returns(tag.Name);
            mockTag.SetupGet(x => x.FrameworkItemType).Returns("Tag");
            mockTag.SetupGet(x => x.FullName).Returns(tag.Name);
            tags.Add(mockTag.Object);
        }

        Mock<IFuturemud> gameworld = new();
        gameworld.SetupGet(x => x.Materials).Returns(materials);
        gameworld.SetupGet(x => x.Tags).Returns(tags);
        gameworld.SetupGet(x => x.Liquids).Returns(new All<ILiquid>());
        gameworld.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
        gameworld.Setup(x => x.Add(It.IsAny<IMaterial>()));
        return gameworld;
    }

    [TestMethod]
    public void SeedMaterials_SeedsAliasesAndCommonMissingMaterials()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedMaterials(context);

        Assert.IsTrue(context.Materials.Any(x => x.Name == "ABS plastic"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "polycarbonate"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "tempered glass"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "tool steel"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "spring steel"));

        Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == "mild steel" && x.Alias == "steel"));
        Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == "high-density polyethylene" && x.Alias == "hdpe"));
    }

    [TestMethod]
    public void SeededMaterials_AliasLookupFindsExpectedDefaults()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedMaterials(context);

        All<ISolid> materials = new();
        Mock<IFuturemud> gameworld = CreateGameworld(materials, context);
        foreach (MudSharp.Models.Material dbMaterial in context.Materials
                     .Include(x => x.MaterialAliases)
                     .Include(x => x.MaterialsTags))
        {
            materials.Add(new Solid(dbMaterial, gameworld.Object));
        }

        Assert.AreEqual("mild steel", materials.GetByName("steel")?.Name);
        Assert.AreEqual("high-density polyethylene", materials.GetByName("hdpe")?.Name);
    }

    [TestMethod]
    public void SeedMaterials_CorrectsKnownIssuesAndExpandsCatalogue()
    {
        using FuturemudDatabaseContext baselineContext = BuildContext();
        SeedMaterialsBase(baselineContext);
        int baselineCount = baselineContext.Materials.Count();

        using FuturemudDatabaseContext context = BuildContext();
        SeedMaterials(context);

        MudSharp.Models.Material coachwood = context.Materials
            .Include(x => x.MaterialAliases)
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "coachwood");
        MudSharp.Models.Material aubergine = context.Materials
            .Include(x => x.MaterialAliases)
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "aubergine");
        MudSharp.Models.Material paper = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "paper");
        MudSharp.Models.Material soap = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "soap");
        MudSharp.Models.Material paraffinWax = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "paraffin wax");
        MudSharp.Models.Material honey = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "honey");
        MudSharp.Models.Material honeycomb = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "honeycomb");
        MudSharp.Models.Material beeswax = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "beeswax");
        MudSharp.Models.Material milk = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "milk");
        MudSharp.Models.Material egg = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "egg");
        MudSharp.Models.Material wool = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "wool");
        MudSharp.Models.Material feces = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "feces");
        MudSharp.Models.Material compost = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "compost");
        MudSharp.Models.Material salt = context.Materials.Single(x => x.Name == "salt");
        MudSharp.Models.Material guano = context.Materials.Single(x => x.Name == "guano");
        MudSharp.Models.Material cream = context.Materials.Single(x => x.Name == "cream");

        Assert.IsTrue(context.Materials.Count() >= Math.Ceiling(baselineCount * 1.25),
            $"Expected at least a 25% material increase over the baseline catalogue of {baselineCount} materials.");
        Assert.IsFalse(context.Tags.Any(x => x.Name == "Water Soluable"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Water Soluble"));
        Assert.IsTrue(coachwood.MaterialAliases.Any(x => x.Alias == "coach"));
        Assert.IsTrue(aubergine.MaterialAliases.Any(x => x.Alias == "eggplant"));
        Assert.IsTrue(aubergine.MaterialAliases.Any(x => x.Alias == "brinjal"));
        Assert.AreEqual((int)MaterialBehaviourType.Feces, guano.BehaviourType);
        Assert.IsFalse(salt.Organic);
        Assert.IsTrue(cream.Organic);
        Assert.IsTrue(paper.MaterialsTags.Any(x => x.Tag.Name == "Paper Product"));
        Assert.IsTrue(soap.MaterialsTags.Any(x => x.Tag.Name == "Manufactured Materials"));
        Assert.IsFalse(soap.MaterialsTags.Any(x => x.Tag.Name == "Natural Materials"));
        Assert.IsFalse(paraffinWax.MaterialsTags.Any(x => x.Tag.Name == "Natural Materials"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "bone"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "blood"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "rosewood"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Apiary Product"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Raw Honeycomb"));
        Assert.AreEqual((int)MaterialBehaviourType.Food, honeycomb.BehaviourType);
        Assert.IsTrue(honey.MaterialsTags.Any(x => x.Tag.Name == "Apiary Product"));
        Assert.IsTrue(honeycomb.MaterialsTags.Any(x => x.Tag.Name == "Apiary Product"));
        Assert.IsTrue(beeswax.MaterialsTags.Any(x => x.Tag.Name == "Apiary Product"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Pastoral Product"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Raw Milk"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Egg Product"));
        Assert.IsTrue(context.Tags.Any(x => x.Name == "Manure Product"));

        var butcheryTagParents = context.Tags
            .Include(x => x.Parent)
            .Where(x => new[]
            {
                "Butchery Output",
                "Raw Meat Cut",
                "Raw Hide",
                "Offal",
                "Trophy Part",
                "Venom Organ",
                "Crafting Animal Product"
            }.Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.Parent?.Name, StringComparer.OrdinalIgnoreCase);
        Assert.AreEqual("Animal Product", butcheryTagParents["Butchery Output"]);
        foreach (var tagName in butcheryTagParents.Keys.Where(x => x != "Butchery Output"))
        {
            Assert.AreEqual("Butchery Output", butcheryTagParents[tagName], $"{tagName} should be grouped under the butchery output bridge tag.");
        }
        Assert.IsTrue(milk.MaterialsTags.Any(x => x.Tag.Name == "Raw Milk"));
        Assert.IsTrue(egg.MaterialsTags.Any(x => x.Tag.Name == "Egg Product"));
        Assert.IsTrue(wool.MaterialsTags.Any(x => x.Tag.Name == "Raw Wool"));
        Assert.IsTrue(feces.MaterialsTags.Any(x => x.Tag.Name == "Manure Product"));
        Assert.IsTrue(compost.MaterialsTags.Any(x => x.Tag.Name == "Manure Product"));
        Assert.IsTrue(context.Materials.Any(x => x.Name == "saffron crocus"));
    }

	[TestMethod]
	public void SeedMaterials_SeedsTraditionalCraftMaterialsWithExpectedTags()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedMaterials(context);

		Dictionary<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectations = new(StringComparer.InvariantCultureIgnoreCase)
		{
			["rawhide"] = (MaterialBehaviourType.Skin, ["Animal Skin", "Animal Product"]),
			["sinew"] = (MaterialBehaviourType.Muscle, ["Animal Product"]),
			["rattan"] = (MaterialBehaviourType.Wood, ["Wood"]),
			["wicker"] = (MaterialBehaviourType.Wood, ["Manufactured Wood"]),
			["reed"] = (MaterialBehaviourType.Plant, ["Vegetation"]),
			["cane"] = (MaterialBehaviourType.Wood, ["Wood"]),
			["lacquer"] = (MaterialBehaviourType.Paste, ["Manufactured Materials"]),
			["horsehair"] = (MaterialBehaviourType.Hair, ["Hair", "Animal Product"]),
			["goat leather"] = (MaterialBehaviourType.Leather, ["Leather"]),
			["sheep leather"] = (MaterialBehaviourType.Leather, ["Leather"])
		};

		Dictionary<string, MudSharp.Models.Material> materials = context.Materials
			.Include(x => x.MaterialsTags)
			.ThenInclude(x => x.Tag)
			.AsEnumerable()
			.Where(x => expectations.ContainsKey(x.Name))
			.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

		foreach (KeyValuePair<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectation in expectations)
		{
			Assert.IsTrue(materials.TryGetValue(expectation.Key, out MudSharp.Models.Material? material),
				$"{expectation.Key} should be seeded.");
			Assert.IsNotNull(material);
			Assert.AreEqual((int)expectation.Value.Behaviour, material!.BehaviourType,
				$"{expectation.Key} should use the expected material behaviour.");

			foreach (string tag in expectation.Value.Tags)
			{
				Assert.IsTrue(material.MaterialsTags.Any(x => x.Tag.Name == tag),
					$"{expectation.Key} should be tagged as {tag}.");
			}
		}

		(string Material, string Alias)[] expectedAliases =
		[
			("rawhide", "raw hide"),
			("reed", "reeds"),
			("horsehair", "horse hair"),
			("goat leather", "goatskin"),
			("sheep leather", "sheepskin"),
			("lacquer", "urushi")
		];

		foreach ((string material, string alias) in expectedAliases)
		{
			Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == material && x.Alias == alias),
				$"{material} should have the {alias} alias.");
		}
	}

	[TestMethod]
	public void SeedMaterials_SeedsMedievalMedicalMaterialsAndReleasesAlumAlias()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedMaterials(context);

		Dictionary<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectations = new(StringComparer.InvariantCultureIgnoreCase)
		{
			["alum"] = (MaterialBehaviourType.Powder, ["Textile Mordant"]),
			["ephedra"] = (MaterialBehaviourType.Plant, ["Herb"]),
			["foxglove"] = (MaterialBehaviourType.Plant, ["Herb"]),
			["gut"] = (MaterialBehaviourType.Remains, ["Animal Product", "Crafting Animal Product"]),
			["henbane"] = (MaterialBehaviourType.Plant, ["Herb"]),
			["mandrake"] = (MaterialBehaviourType.Plant, ["Herb"]),
			["yarrow"] = (MaterialBehaviourType.Plant, ["Herb"])
		};

		Dictionary<string, MudSharp.Models.Material> materials = context.Materials
			.Include(x => x.MaterialAliases)
			.Include(x => x.MaterialsTags)
			.ThenInclude(x => x.Tag)
			.AsEnumerable()
			.Where(x => expectations.ContainsKey(x.Name) || x.Name == "alum mordant")
			.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

		foreach (KeyValuePair<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectation in expectations)
		{
			Assert.IsTrue(materials.TryGetValue(expectation.Key, out MudSharp.Models.Material? material),
				$"{expectation.Key} should be seeded.");
			Assert.IsNotNull(material);
			Assert.AreEqual((int)expectation.Value.Behaviour, material!.BehaviourType,
				$"{expectation.Key} should use the expected material behaviour.");

			foreach (string tag in expectation.Value.Tags)
			{
				Assert.IsTrue(material.MaterialsTags.Any(x => x.Tag.Name == tag),
					$"{expectation.Key} should be tagged as {tag}.");
			}
		}

		Assert.IsTrue(materials.ContainsKey("alum mordant"), "alum mordant should still be seeded.");
		Assert.IsFalse(materials["alum mordant"].MaterialAliases.Any(x => x.Alias == "alum"),
			"alum should be reserved for the exact alum material, not as an alum mordant alias.");
	}

	[TestMethod]
	public void SeededMaterialsCatalogue_IncludesMedievalMedicalMaterials()
	{
		var catalogueSource = ReadSource("Design Documents", "Data", "Seeded_Materials.json");
		using var catalogue = JsonDocument.Parse(catalogueSource);

		var entries = catalogue.RootElement
			.EnumerateArray()
			.ToDictionary(
				x => x.GetProperty("Material Name").GetString()!,
				x => x.GetProperty("Tags").EnumerateArray().Select(y => y.GetString()!).ToArray(),
				StringComparer.InvariantCultureIgnoreCase);

		Dictionary<string, string[]> expectations = new(StringComparer.InvariantCultureIgnoreCase)
		{
			["alum"] = ["Materials / Textile Mordant"],
			["ephedra"] = ["Materials / Natural Materials / Food / Herb"],
			["foxglove"] = ["Materials / Natural Materials / Food / Herb"],
			["gut"] =
			[
				"Materials / Animal Product",
				"Materials / Animal Product / Butchery Output / Crafting Animal Product"
			],
			["henbane"] = ["Materials / Natural Materials / Food / Herb"],
			["mandrake"] = ["Materials / Natural Materials / Food / Herb"],
			["yarrow"] = ["Materials / Natural Materials / Food / Herb"]
		};

		foreach (var expectation in expectations)
		{
			Assert.IsTrue(entries.TryGetValue(expectation.Key, out var tags),
				$"Seeded_Materials.json should include {expectation.Key}.");
			Assert.IsNotNull(tags);
			foreach (var tag in expectation.Value)
			{
				Assert.IsTrue(tags!.Contains(tag),
					$"Seeded_Materials.json should tag {expectation.Key} as {tag}.");
			}
		}

		Assert.IsFalse(File.Exists(SourcePath("Design Documents", "Data", "Seeded_Materials_Medieval_Medical_Additions.json")),
			"Medieval medical material additions should be integrated into Seeded_Materials.json, not kept as a supplement.");
	}

	[TestMethod]
	public void SeedMaterials_SeedsMedievalJewelleryMaterialsWithExpectedTags()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedMaterials(context);

		Dictionary<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectations =
			new(StringComparer.InvariantCultureIgnoreCase)
			{
				["coral"] = (MaterialBehaviourType.Shell, ["Shell"]),
				["rock crystal"] = (MaterialBehaviourType.Stone, ["Gemstone"]),
				["faience"] = (MaterialBehaviourType.Ceramic, ["Faience"]),
				["enamel"] = (MaterialBehaviourType.Ceramic, ["Enamel", "Glass"]),
				["niello"] = (MaterialBehaviourType.Metal, ["Inlay Material"]),
				["silver-gilt"] = (MaterialBehaviourType.Metal, ["Gilded Metal", "Precious Metal"]),
				["gilded bronze"] = (MaterialBehaviourType.Metal, ["Gilded Metal"]),
				["gilded copper"] = (MaterialBehaviourType.Metal, ["Gilded Metal"]),
				["mother-of-pearl"] = (MaterialBehaviourType.Shell, ["Shell", "Gemstone"]),
				["nacre"] = (MaterialBehaviourType.Shell, ["Shell", "Gemstone"]),
				["cowrie shell"] = (MaterialBehaviourType.Shell, ["Shell"]),
				["conch shell"] = (MaterialBehaviourType.Shell, ["Shell"]),
				["tortoiseshell"] = (MaterialBehaviourType.Shell, ["Tortoiseshell"]),
				["flower"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["fresh flower"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["wilted flower"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["dried flower"] = (MaterialBehaviourType.Plant, ["Dried Flower"]),
				["petal"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["dried petal"] = (MaterialBehaviourType.Plant, ["Dried Flower"]),
				["rose"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["violet"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["daisy"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["jasmine"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["lotus flower"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["marigold"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["lily"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["chrysanthemum"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["blossom"] = (MaterialBehaviourType.Plant, ["Flower"]),
				["ivy"] = (MaterialBehaviourType.Plant, ["Leaf"]),
				["laurel"] = (MaterialBehaviourType.Plant, ["Leaf"]),
				["rush"] = (MaterialBehaviourType.Plant, ["Rush"]),
				["straw"] = (MaterialBehaviourType.Plant, ["Vegetation"])
			};

		Dictionary<string, MudSharp.Models.Material> materials = context.Materials
			.Include(x => x.MaterialAliases)
			.Include(x => x.MaterialsTags)
			.ThenInclude(x => x.Tag)
			.AsEnumerable()
			.Where(x => expectations.ContainsKey(x.Name))
			.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

		foreach (KeyValuePair<string, (MaterialBehaviourType Behaviour, string[] Tags)> expectation in expectations)
		{
			Assert.IsTrue(materials.TryGetValue(expectation.Key, out MudSharp.Models.Material? material),
				$"{expectation.Key} should be seeded.");
			Assert.IsNotNull(material);
			Assert.AreEqual((int)expectation.Value.Behaviour, material!.BehaviourType,
				$"{expectation.Key} should use the expected material behaviour.");

			foreach (string tag in expectation.Value.Tags)
			{
				Assert.IsTrue(material.MaterialsTags.Any(x => x.Tag.Name == tag),
					$"{expectation.Key} should be tagged as {tag}.");
			}
		}

		(string Material, string Alias)[] expectedAliases =
		[
			("mother-of-pearl", "mother of pearl"),
			("rock crystal", "clear quartz"),
			("silver-gilt", "vermeil"),
			("gilded bronze", "gilt bronze"),
			("gilded copper", "gilt copper"),
			("cowrie shell", "cowrie"),
			("conch shell", "conch")
		];

		foreach ((string material, string alias) in expectedAliases)
		{
			Assert.IsTrue(context.MaterialAliases.Any(x => x.Material.Name == material && x.Alias == alias),
				$"{material} should have the {alias} alias.");
		}
	}

	[TestMethod]
	public void SeededMaterialsCatalogue_IncludesMedievalJewelleryMaterials()
	{
		var catalogueSource = ReadSource("Design Documents", "Data", "Seeded_Materials.json");
		using var catalogue = JsonDocument.Parse(catalogueSource);

		var entries = catalogue.RootElement
			.EnumerateArray()
			.ToDictionary(
				x => x.GetProperty("Material Name").GetString()!,
				x => x.GetProperty("Tags").EnumerateArray().Select(y => y.GetString()!).ToArray(),
				StringComparer.InvariantCultureIgnoreCase);

		Dictionary<string, string[]> expectations = new(StringComparer.InvariantCultureIgnoreCase)
		{
			["coral"] = ["Materials / Animal Product / Shell"],
			["rock crystal"] = ["Materials / Natural Materials / Stone / Economically Useful Stone / Gemstone"],
			["faience"] = ["Materials / Manufactured Materials / Ceramic / Faience"],
			["enamel"] = ["Materials / Manufactured Materials / Glass / Enamel"],
			["niello"] = ["Materials / Manufactured Materials / Manufactured Metal / Inlay Material"],
			["silver-gilt"] = ["Materials / Manufactured Materials / Manufactured Metal / Gilded Metal"],
			["gilded bronze"] = ["Materials / Manufactured Materials / Manufactured Metal / Gilded Metal"],
			["gilded copper"] = ["Materials / Manufactured Materials / Manufactured Metal / Gilded Metal"],
			["mother-of-pearl"] = ["Materials / Animal Product / Shell"],
			["nacre"] = ["Materials / Animal Product / Shell"],
			["cowrie shell"] = ["Materials / Animal Product / Shell"],
			["conch shell"] = ["Materials / Animal Product / Shell"],
			["tortoiseshell"] = ["Materials / Animal Product / Tortoiseshell"],
			["flower"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["fresh flower"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["wilted flower"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["dried flower"] = ["Materials / Natural Materials / Vegetation / Flower / Dried Flower"],
			["petal"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["dried petal"] = ["Materials / Natural Materials / Vegetation / Flower / Dried Flower"],
			["rose"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["violet"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["daisy"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["jasmine"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["lotus flower"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["marigold"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["lily"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["chrysanthemum"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["blossom"] = ["Materials / Natural Materials / Vegetation / Flower"],
			["ivy"] = ["Materials / Natural Materials / Vegetation / Leaf"],
			["laurel"] = ["Materials / Natural Materials / Vegetation / Leaf"],
			["rush"] = ["Materials / Natural Materials / Vegetation / Rush"],
			["straw"] = ["Materials / Natural Materials / Vegetation"]
		};

		foreach (var expectation in expectations)
		{
			Assert.IsTrue(entries.TryGetValue(expectation.Key, out var tags),
				$"Seeded_Materials.json should include {expectation.Key}.");
			Assert.IsNotNull(tags);
			foreach (var tag in expectation.Value)
			{
				Assert.IsTrue(tags!.Contains(tag),
					$"Seeded_Materials.json should tag {expectation.Key} as {tag}.");
			}
		}
	}

    [TestMethod]
    public void SeedMaterials_IncludesAllAgricultureCommodityOutputs()
    {
        using FuturemudDatabaseContext context = BuildContext();
        SeedMaterials(context);

        HashSet<string> materialNames = context.Materials
            .Select(x => x.Name)
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        string[] missingMaterials = AgricultureSeeder.StockCommodityOutputMaterialsForTesting
            .Where(x => !materialNames.Contains(x))
            .ToArray();

        Assert.AreEqual(0, missingMaterials.Length,
            $"Agriculture stock output materials should be seeded by CoreDataSeeder. Missing: {string.Join(", ", missingMaterials)}");

        HashSet<string> seedableMaterials = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Where(x => x.MaterialsTags.Any(y => y.Tag.Name == "Agriculture Seedable"))
            .Select(x => x.Name)
            .ToHashSet(StringComparer.InvariantCultureIgnoreCase);

        string[] missingSeedTags = AgricultureSeeder.StockSeedMaterialNamesForTesting
            .Where(x => !seedableMaterials.Contains(x))
            .ToArray();

        Assert.AreEqual(0, missingSeedTags.Length,
            $"Agriculture stock seed materials should be tagged as Agriculture Seedable. Missing: {string.Join(", ", missingSeedTags)}");

        MudSharp.Models.Material walnutWood = context.Materials.Single(x => x.Name == "walnut");
        MudSharp.Models.Material walnutNut = context.Materials
            .Include(x => x.MaterialsTags)
            .ThenInclude(x => x.Tag)
            .Single(x => x.Name == "walnut nut");
        Assert.AreEqual((int)MaterialBehaviourType.Wood, walnutWood.BehaviourType);
        Assert.AreEqual((int)MaterialBehaviourType.Food, walnutNut.BehaviourType);
        Assert.IsTrue(walnutNut.MaterialsTags.Any(x => x.Tag.Name == "Agriculture Seedable"));

        Dictionary<string, string[]> representativeNewMaterials = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ["tepary bean"] = ["Food Crop", "Agriculture Seedable"],
            ["vanilla"] = ["Spice", "Agriculture Seedable"],
            ["henequen fibre"] = ["Fiber Crop", "Agriculture Seedable"],
            ["pineapple"] = ["Fruit", "Agriculture Seedable"],
            ["gum arabic"] = ["Natural Materials"],
            ["mangrove wood"] = ["Hardwood"],
            ["charcoal"] = ["Manufactured Materials"],
			["tobacco leaf"] = ["Herb", "Agriculture Seedable", "Primary Production Commodity"],
			["logwood"] = ["Hardwood", "Textile Dye", "Agriculture Seedable", "Primary Production Commodity"],
			["cochineal"] = ["Animal Product", "Textile Dye", "Primary Production Commodity"],
			["type metal"] = ["Renaissance Age", "Primary Production Metal Stock", "Primary Production Commodity"],
			["printing ink"] = ["Writing Product", "Primary Production Commodity"],
			["taffeta"] = ["Natural Fiber Fabric", "Primary Production Commodity"],
			["molasses"] = ["Food", "Primary Production Commodity"],
			["cacao bean"] = ["Food Crop", "Primary Production Commodity"],
			["cotton fibre"] = ["Fiber Crop", "Primary Production Commodity"]
        };

        foreach (KeyValuePair<string, string[]> expectation in representativeNewMaterials)
        {
            MudSharp.Models.Material material = context.Materials
                .Include(x => x.MaterialsTags)
                .ThenInclude(x => x.Tag)
                .Single(x => x.Name == expectation.Key);
            foreach (string tag in expectation.Value)
            {
                Assert.IsTrue(material.MaterialsTags.Any(x => x.Tag.Name == tag),
                    $"{expectation.Key} should be tagged {tag}.");
            }
        }
    }

	[TestMethod]
	public void SeedMaterials_SeedsRenaissanceAndEarlyModernFoundationMaterials()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedMaterials(context);

		string[] expectedMaterials =
		[
			"taffeta", "ribbon", "calico", "chintz", "logwood", "cochineal", "tobacco leaf", "type metal",
			"printing ink", "molasses", "sugar loaf", "tobacco twist", "snuff", "roasted coffee", "cacao bean",
			"cacao nibs", "chocolate block", "tea brick", "cotton fibre", "glass blank", "indigo dye cake"
		];

		var materials = context.Materials
			.Include(x => x.MaterialAliases)
			.Include(x => x.MaterialsTags)
			.ThenInclude(x => x.Tag)
			.AsEnumerable()
			.Where(x => expectedMaterials.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))
			.ToDictionary(x => x.Name, StringComparer.InvariantCultureIgnoreCase);

		CollectionAssert.AreEquivalent(expectedMaterials, materials.Keys.ToArray());
		Assert.AreEqual((int)MaterialBehaviourType.Fabric, materials["taffeta"].BehaviourType);
		Assert.AreEqual((int)MaterialBehaviourType.Wood, materials["logwood"].BehaviourType);
		Assert.AreEqual((int)MaterialBehaviourType.Metal, materials["type metal"].BehaviourType);
		Assert.AreEqual((int)MaterialBehaviourType.Paste, materials["printing ink"].BehaviourType);
		Assert.AreEqual((int)MaterialBehaviourType.Paste, materials["molasses"].BehaviourType);

		Assert.IsTrue(materials["tobacco leaf"].MaterialsTags.Any(x => x.Tag.Name == "Agriculture Seedable"));
		Assert.IsTrue(materials["logwood"].MaterialsTags.Any(x => x.Tag.Name == "Agriculture Seedable"));
		Assert.IsTrue(materials["cochineal"].MaterialsTags.Any(x => x.Tag.Name == "Textile Dye"));
		Assert.IsTrue(materials["type metal"].MaterialsTags.Any(x => x.Tag.Name == "Primary Production Metal Stock"));
		Assert.IsTrue(materials["printing ink"].MaterialsTags.Any(x => x.Tag.Name == "Writing Product"));

		Assert.IsTrue(materials["tobacco leaf"].MaterialAliases.Any(x => x.Alias == "tobacco"));
		Assert.IsTrue(materials["printing ink"].MaterialAliases.Any(x => x.Alias == "oil-based printing ink"));
		Assert.IsTrue(materials["cacao bean"].MaterialAliases.Any(x => x.Alias == "cocoa bean"));
		Assert.IsTrue(materials["tea brick"].MaterialAliases.Any(x => x.Alias == "tea cake"));
	}

    [TestMethod]
    public void SeedMaterialsBase_DoesNotAssumeWaterHasLiquidIdOne()
    {
        using FuturemudDatabaseContext context = BuildContext();
        MudSharp.Models.Liquid retiredLiquid = BuildPlaceholderLiquid("retired liquid placeholder");
        context.Liquids.Add(retiredLiquid);
        context.SaveChanges();
        context.Liquids.Remove(retiredLiquid);
        context.SaveChanges();

        SeedMaterialsBase(context);

        MudSharp.Models.Liquid water = context.Liquids.Single(x => x.Name == "water");
        MudSharp.Models.Liquid soapyWater = context.Liquids.Single(x => x.Name == "soapy water");
        MudSharp.Models.Liquid detergent = context.Liquids.Single(x => x.Name == "detergent");

        Assert.AreNotEqual(1L, water.Id, "Test setup should exercise a non-1 water id.");
        Assert.AreEqual(water.Id, context.Liquids.Single(x => x.Name == "blood").SolventId);
        Assert.AreEqual(water.Id, context.Liquids.Single(x => x.Name == "sweat").SolventId);
        Assert.AreEqual(water.Id, context.Liquids.Single(x => x.Name == "vomit").SolventId);
        Assert.AreEqual(water.Id, context.Liquids.Single(x => x.Name == "lager").SolventId);
        Assert.AreEqual(water.Id, context.Materials.Single(x => x.Name == "dried blood").SolventId);
        Assert.AreEqual(water.Id, context.Materials.Single(x => x.Name == "dried Sweat").SolventId);
        Assert.AreEqual(water.Id, context.Materials.Single(x => x.Name == "dried vomit").SolventId);
        Assert.AreEqual(soapyWater.Id, context.Liquids.Single(x => x.Name == "olive oil").SolventId);
        Assert.AreEqual(detergent.Id, context.Liquids.Single(x => x.Name == "kerosene").SolventId);
    }

	private static string ReadSource(params string[] parts)
	{
		return File.ReadAllText(SourcePath(parts));
	}

	private static string SourcePath(params string[] parts)
	{
		return Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..",
			"..",
			"..",
			"..",
			Path.Combine(parts)));
	}
}
