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

	[TestMethod]
	public void SeedMaterials_RerunUsesTheMaterialTagHierarchyWhenLeafNamesOverlap()
	{
		using FuturemudDatabaseContext context = BuildContext();
		SeedMaterials(context);

		MudSharp.Models.Tag legacyWaterSoluble = context.Tags.Single(x =>
			x.Name == "Water Soluble" &&
			x.Parent!.Name == "Liquids");
		legacyWaterSoluble.Name = "Water Soluable";
		context.SaveChanges();

		new CoreDataSeeder().SeedTagsForTesting(context);
		SeedMaterials(context);
		SeedMaterials(context);

		MudSharp.Models.Tag materials = context.Tags.Single(x => x.Name == "Materials" && x.ParentId == null);
		MudSharp.Models.Tag animalProduct = context.Tags.Single(x => x.Name == "Animal Product" && x.ParentId == materials.Id);
		MudSharp.Models.Tag materialApiaryProduct = context.Tags.Single(x =>
			x.Name == "Apiary Product" &&
			x.ParentId == animalProduct.Id);
		MudSharp.Models.Tag functions = context.Tags.Single(x => x.Name == "Functions" && x.ParentId == null);
		MudSharp.Models.Tag materialFunctions = context.Tags.Single(x => x.Name == "Material Functions" && x.ParentId == functions.Id);
		MudSharp.Models.Tag agriculture = context.Tags.Single(x => x.Name == "Agriculture" && x.ParentId == materialFunctions.Id);
		MudSharp.Models.Tag agricultureApiaryProduct = context.Tags.Single(x =>
			x.Name == "Apiary Product" &&
			x.ParentId == agriculture.Id);
		MudSharp.Models.Tag liquids = context.Tags.Single(x => x.Name == "Liquids" && x.ParentId == materials.Id);
		MudSharp.Models.Material honey = context.Materials.Single(x => x.Name == "honey");

		Assert.AreNotEqual(materialApiaryProduct.Id, agricultureApiaryProduct.Id);
		Assert.AreEqual(2, context.Tags.Count(x => x.Name == "Apiary Product"));
		Assert.IsTrue(context.MaterialsTags.Any(x =>
			x.MaterialId == honey.Id &&
			x.TagId == materialApiaryProduct.Id));
        Assert.AreEqual(legacyWaterSoluble.Id, CoreTagCatalogue.Require(context, "Materials / Liquids / Water Soluble").Id);
        Assert.AreEqual("Water Soluable", legacyWaterSoluble.Name, "Builder renames remain intact after stable identity binding.");
		Assert.IsFalse(context.MaterialsTags
			.GroupBy(x => new { x.MaterialId, x.TagId })
			.Any(x => x.Count() > 1));
	}

	[TestMethod]
	public void Pith_RerunRestoresAliasesWithoutDuplicatingOrOverwritingCustomProperties()
	{
		using var context = BuildContext();
		SeedMaterials(context);
		var pith = context.Materials.Include(x => x.MaterialAliases).Single(x => x.Name == "pith");
		var id = pith.Id;
		pith.Density = 53;
		pith.MaterialDescription = "Builder's pith stock";
		context.MaterialAliases.Remove(pith.MaterialAliases.Single(x => x.Alias == "shola pith"));
		context.SaveChanges();
		var counts = (context.Materials.Count(), context.Tags.Count(), context.MaterialsTags.Count());
		SeedMaterials(context);
		SeedMaterials(context);
		Assert.AreEqual(counts, (context.Materials.Count(), context.Tags.Count(), context.MaterialsTags.Count()));
		Assert.AreEqual(id, context.Materials.Single(x => x.Name == "pith").Id);
		Assert.AreEqual(53.0, pith.Density);
		Assert.AreEqual("Builder's pith stock", pith.MaterialDescription);
		Assert.AreEqual(3, context.MaterialAliases.Count(x => x.MaterialId == id));
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
