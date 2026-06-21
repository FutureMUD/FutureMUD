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
using System.Linq;
using System.Reflection;

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
            ["charcoal"] = ["Manufactured Materials"]
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
}
