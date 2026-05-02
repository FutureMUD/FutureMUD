#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RPI_Engine_Worldfile_Converter;

namespace RPI_Engine_Worldfile_Converter_Tests;

[TestClass]
public class RpiCraftConversionTests
{
	[TestMethod]
	public void CraftParser_ReadsFixtureCorpus_AndCapturesTypedDirectives()
	{
		var parser = new RpiCraftParser();
		var corpus = parser.ParseFile(GetCraftFixturePath());

		Assert.AreEqual(4, corpus.Crafts.Count);
		Assert.AreEqual(0, corpus.Failures.Count);

		var woodworking = corpus.Crafts.Single(x => x.CraftNumber == 1);
		Assert.AreEqual("woodworking", woodworking.CraftName);
		Assert.AreEqual("spear", woodworking.SubcraftName);
		Assert.AreEqual("carve", woodworking.Command);
		Assert.AreEqual(1, woodworking.Constraints.OpeningSkillIds.Count);
		Assert.AreEqual(2, woodworking.Constraints.FollowersRequired);
		Assert.AreEqual(1, woodworking.Constraints.ClanRequirements.Count);
		Assert.AreEqual("trusted", woodworking.Constraints.ClanRequirements[0].RankName);
		Assert.AreEqual("Rangers", woodworking.Constraints.ClanRequirements[0].ClanAlias);
		Assert.AreEqual(1, woodworking.Phases.Count);
		Assert.IsNotNull(woodworking.Phases[0].Cost);
		Assert.AreEqual(12, woodworking.Phases[0].Cost!.Moves);
		Assert.AreEqual(3, woodworking.Phases[0].Cost!.Hits);
		Assert.AreEqual(4, woodworking.Phases[0].ItemLists.Count);
		Assert.AreEqual(1, woodworking.Phases[0].FailItemLists.Count);

		var smithing = corpus.Crafts.Single(x => x.CraftNumber == 2);
		Assert.AreEqual(1, smithing.VariantLink.StartKey);
		Assert.AreEqual(2, smithing.VariantLink.EndKey);

		var hunting = corpus.Crafts.Single(x => x.CraftNumber == 3);
		CollectionAssert.AreEquivalent(new[] { 1005, 1004 }, hunting.FailObjectVnums.ToArray());
		CollectionAssert.AreEquivalent(new[] { 9001 }, hunting.FailMobVnums.ToArray());
		Assert.AreEqual(9002, hunting.Phases[0].LoadMobVnum);
		CollectionAssert.Contains(hunting.Phases[0].MobileFlags.ToList(), "aggressive");

		var mystery = corpus.Crafts.Single(x => x.CraftNumber == 4);
		Assert.AreEqual("mysteryskill", mystery.Phases[0].SkillCheck!.SourceName);
		Assert.AreEqual(2, mystery.Phases[0].ItemLists.Count);
	}

	[TestMethod]
	public void CraftTransformer_NormalisesFixtureCorpus_AndFlagsDeferredCases()
	{
		var parser = new RpiCraftParser();
		var corpus = parser.ParseFile(GetCraftFixturePath());
		var transformer = new FutureMudCraftTransformer(BuildConvertedItems());
		var conversion = transformer.Convert(corpus.Crafts);
		var converted = conversion.Crafts.ToDictionary(x => x.CraftNumber);

		Assert.AreEqual(1, conversion.GeneratedTags.Count);

		var woodworking = converted[1];
		Assert.AreEqual(CraftConversionStatus.Ready, woodworking.Status);
		Assert.AreEqual("Woodcraft", woodworking.PrimaryCheck!.TraitName);
		Assert.AreEqual("Dexterity", woodworking.AdditionalChecks.Single().TraitName);
		Assert.IsTrue(woodworking.Warnings.Any(x => x.Code == "multi-check-reduced"));
		Assert.IsTrue(woodworking.Constraints.HiddenFromCraftList);
		Assert.AreEqual("Broadleaf Forest", woodworking.Constraints.TerrainNames.Single());
		Assert.AreEqual(2, woodworking.Constraints.FollowersRequired);
		Assert.AreEqual(3, woodworking.FutureProgPlans.Count);
		Assert.IsTrue(woodworking.FutureProgPlans.Any(x => x.PreviewText?.Contains("followers: 2", StringComparison.OrdinalIgnoreCase) == true));
		Assert.AreEqual("UnusedInput", woodworking.Products.Single(x => x.IsFailProduct).ProductType);
		Assert.AreEqual(1.0, woodworking.Products.Single(x => x.IsFailProduct).PercentageRecovered!.Value, 0.0001);

		var smithing = converted[2];
		Assert.AreEqual(CraftConversionStatus.Ready, smithing.Status);
		var smithingInput = smithing.Inputs.Single();
		Assert.AreEqual("Tag", smithingInput.InputType);
		Assert.AreEqual("Knife", smithingInput.TagName);
		Assert.IsFalse(smithingInput.UsesGeneratedTag);
		Assert.AreEqual(0, smithing.GeneratedTags.Count);
		var smithingProduct = smithing.Products.Single(x => x.ProductType == "Prog");
		Assert.AreEqual(1, smithingProduct.SourceInputSlot);
		Assert.AreEqual(2, smithingProduct.SourceSlot);
		CollectionAssert.AreEquivalent(new[] { 2000, 2001 }, smithingProduct.ProgCases.Select(x => x.InputSourceVnum).ToArray());
		CollectionAssert.AreEquivalent(new[] { 3000, 3001 }, smithingProduct.ProgCases.Select(x => x.OutputSourceVnum).ToArray());

		var hunting = converted[3];
		Assert.AreEqual(CraftConversionStatus.Deferred, hunting.Status);
		Assert.IsTrue(hunting.Warnings.Any(x => x.Code == "deferred-phase-mob-output"));
		Assert.IsTrue(hunting.Warnings.Any(x => x.Code == "deferred-failmob-output"));
		Assert.IsTrue(hunting.Products.Any(x => x.LegacyGiveOutput));

		var mystery = converted[4];
		Assert.AreEqual(CraftConversionStatus.Ready, mystery.Status);
		Assert.IsNull(mystery.PrimaryCheck);
		Assert.IsTrue(mystery.Warnings.Any(x => x.Code == "unresolved-craft-trait"));
		var explicitTagInput = mystery.Inputs.Single();
		Assert.AreEqual("Tag", explicitTagInput.InputType);
		Assert.IsTrue(explicitTagInput.UsesGeneratedTag);
		StringAssert.StartsWith(explicitTagInput.TagName!, "RPI Craft Set 0004 Input 01");
		Assert.AreEqual(explicitTagInput.TagName, conversion.GeneratedTags.Single().TagName);
	}

	[TestMethod]
	public void CraftValidation_UsesFixtureBaselineWithoutUnexpectedErrors()
	{
		var parser = new RpiCraftParser();
		var corpus = parser.ParseFile(GetCraftFixturePath());
		var transformer = new FutureMudCraftTransformer(BuildConvertedItems());
		var conversion = transformer.Convert(corpus.Crafts);

		var baseline = new FutureMudCraftBaselineCatalog
		{
			BuilderAccountId = 1,
			TraitIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Metalcraft"] = 1,
				["Woodcraft"] = 2,
				["Dexterity"] = 3,
			},
			TagIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Knife"] = 1,
			},
			RaceIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Hobbit"] = 1,
			},
			TerrainIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["Broadleaf Forest"] = 1,
			},
			WeatherIdsByNormalisedName = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
			{
				["clear"] = 1,
			},
			ClansByAlias = new Dictionary<string, FutureMudCraftClanReference>(StringComparer.OrdinalIgnoreCase)
			{
				["Rangers"] = new FutureMudCraftClanReference(
					1,
					"Rangers",
					new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
					{
						["trusted"] = 1,
					}),
				["Bakers"] = new FutureMudCraftClanReference(
					2,
					"Bakers",
					new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase)
					{
						["Apprentice"] = 10,
						["Journeyman"] = 11,
						["Master"] = 12,
					}),
			},
			ImportedItemsBySourceVnum = BuildImportedItemCatalog(),
			FutureProgIds = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase),
		};

		var guildMemberFallbackCraft = conversion.Crafts.Single(x => x.CraftNumber == 1) with
		{
			SourceKey = "fixture/guild-member#999",
			Constraints = conversion.Crafts.Single(x => x.CraftNumber == 1).Constraints with
			{
				ClanRequirements =
				[
					new ConvertedCraftClanRequirement("Bakers", "Bakers", "member", Array.Empty<string>())
				]
			}
		};

		var issues = FutureMudCraftValidation.Validate(
			baseline,
			conversion.Crafts.Append(guildMemberFallbackCraft));

		Assert.IsFalse(issues.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(issues.Any(x => x.SourceKey == conversion.Crafts.Single(y => y.CraftNumber == 3).SourceKey &&
		                              x.Severity.Equals("warning", StringComparison.OrdinalIgnoreCase)));
	}

	private static string GetCraftFixturePath()
	{
		var candidates = new[]
		{
			Path.Combine(AppContext.BaseDirectory, "Fixtures", "Crafts", "crafts.txt"),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Fixtures", "Crafts", "crafts.txt")),
			Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "RPI Engine Worldfile Converter Tests", "Fixtures", "Crafts", "crafts.txt")),
		};

		return candidates.First(File.Exists);
	}

	private static IReadOnlyList<ConvertedItemDefinition> BuildConvertedItems()
	{
		return
		[
			BuildConvertedItem(1002, "steel sword", "steel sword", "a steel sword", tags: []),
			BuildConvertedItem(1004, "waterskin", "leather waterskin", "a leather waterskin", tags: []),
			BuildConvertedItem(1005, "bread", "bread loaf", "a loaf of bread", tags: []),
			BuildConvertedItem(1006, "bronze key", "bronze key", "a bronze key", tags: []),
			BuildConvertedItem(1008, "repair kit", "armour repair kit", "an armour repair kit", tags: []),
			BuildConvertedItem(2000, "kitchen knife", "kitchen knife", "a kitchen knife", tags: ["Knife"]),
			BuildConvertedItem(2001, "hunting knife", "hunting knife", "a hunting knife", tags: ["Knife"]),
			BuildConvertedItem(3000, "carving knife", "carving knife", "a carving knife", tags: []),
			BuildConvertedItem(3001, "paring knife", "paring knife", "a paring knife", tags: []),
		];
	}

	private static ConvertedItemDefinition BuildConvertedItem(int vnum, string baseName, string keywords, string shortDescription, IReadOnlyList<string> tags)
	{
		return new ConvertedItemDefinition
		{
			Vnum = vnum,
			SourceFile = "fixture",
			Zone = 1,
			SourceKey = $"fixture/{vnum}",
			SourceItemType = RPIItemType.Other,
			Status = ConversionStatus.FunctionalImport,
			BaseName = baseName,
			Keywords = keywords,
			ShortDescription = shortDescription,
			LongDescription = $"{shortDescription} is here.",
			FullDescription = $"{shortDescription} full description.",
			MaterialName = "steel",
			BaseItemQuality = 5,
			Size = 3,
			WeightGrams = 100,
			CostInBaseCurrency = 10m,
			RawOvals = new[] { 0, 0, 0, 0, 0, 0 },
			TagNames = tags,
		};
	}

	private static Dictionary<int, FutureMudCraftImportedItemReference> BuildImportedItemCatalog()
	{
		var items = BuildConvertedItems();
		return items.ToDictionary(
			x => x.Vnum,
			x => new FutureMudCraftImportedItemReference(x.Vnum, 0, x.ShortDescription, []));
	}
}
