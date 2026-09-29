#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MonsterAIStockTemplateTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

	[TestMethod]
	public void Recommendations_CoverBothCataloguesExactly_AndOnlyReferenceStockProfiles()
	{
		var recommendations = MonsterAIStockTemplates.Recommendations;
		Assert.AreEqual(89, recommendations.Count);
		Assert.AreEqual(89, recommendations.Select(x => (x.Pack, x.Race)).Distinct().Count());
		CollectionAssert.AreEquivalent(MythicalAnimalSeeder.TemplatesForTesting.Keys.ToArray(), recommendations.Where(x => x.Pack == "Mythical").Select(x => x.Race).ToArray());
		CollectionAssert.AreEquivalent(SupernaturalSeeder.TemplatesForTesting.Keys.ToArray(), recommendations.Where(x => x.Pack == "Supernatural").Select(x => x.Race).ToArray());
		Assert.IsTrue(recommendations.SelectMany(x => x.Profiles).All(MonsterAIStockTemplates.Names.Contains));
		CollectionAssert.AreEquivalent(MonsterAIStockTemplates.Names.ToArray(), recommendations.SelectMany(x => x.Profiles).Distinct().ToArray());
		foreach (var recommendation in recommendations)
		{
			Assert.IsTrue(recommendation.RequiredBindings.Count > 0, recommendation.Race);
			Assert.IsTrue(recommendation.NativeAttacks.Count > 0, recommendation.Race);
			Assert.IsTrue(recommendation.Limitations.Any(x => x.Contains("Wildlife group")), recommendation.Race);
		}
		Assert.AreEqual("Retain Animal/wildlife", recommendations.Single(x => x.Race == "Unicorn").Disposition);
		Assert.AreEqual("Builder-authored sapient role", recommendations.Single(x => x.Race == "Minotaur").Disposition);
		Assert.IsFalse(recommendations.Single(x => x.Race == "Zombie").CanUseWeapons);
		Assert.IsTrue(recommendations.Single(x => x.Race == "Skeleton").CanUseWeapons);
		Assert.IsTrue(recommendations.Single(x => x.Race == "Phoenix").Limitations.Any(x => x.Contains("resurrection")));
	}

	[DataTestMethod]
	[DataRow("Mythical", "Supernatural")]
	[DataRow("Supernatural", "Mythical")]
	public void Seed_PackOrderAndReruns_PreserveIdsClonesAndLegacyRows(string first, string second)
	{
		using var context = Context();
		var legacy = new ArtificialIntelligence { Name = "AnimalMythicGuardian", Type = "Animal", Definition = "<legacy/>" };
		var wildlife = new ArtificialIntelligence { Name = "Wildlife - Mythic Venom Web Hunter", Type = "Animal", Definition = "<wildlife/>" };
		var clone = new ArtificialIntelligence { Name = "My Dragon Controller", Type = "Monster", Definition = "<custom/>" };
		context.ArtificialIntelligences.AddRange(legacy, wildlife, clone); context.SaveChanges();
		Assert.IsTrue(MonsterAIStockTemplates.HasMissing(context, first));
		MonsterAIStockTemplates.Seed(context, first); MonsterAIStockTemplates.Seed(context, second);
		Assert.IsFalse(MonsterAIStockTemplates.HasMissing(context, first)); Assert.IsFalse(MonsterAIStockTemplates.HasMissing(context, second));
		var ids = context.ArtificialIntelligences.Where(x => x.Type == "Monster").ToDictionary(x => x.Name, x => x.Id);
		var stock = context.ArtificialIntelligences.Single(x => x.Name == MonsterAIStockTemplates.NightStalker); stock.Definition = "<old/>"; context.SaveChanges();
		MonsterAIStockTemplates.Seed(context, second); MonsterAIStockTemplates.Seed(context, first);
		Assert.AreEqual(MonsterAIStockTemplates.Names.Count + 3, context.ArtificialIntelligences.Count());
		Assert.IsTrue(context.ArtificialIntelligences.Where(x => x.Type == "Monster").All(x => ids[x.Name] == x.Id));
		Assert.AreEqual("<legacy/>", legacy.Definition); Assert.AreEqual("<wildlife/>", wildlife.Definition); Assert.AreEqual("<custom/>", clone.Definition);
		Assert.AreEqual("Monster", stock.Type); Assert.IsNotNull(XElement.Parse(stock.Definition).Element("Monster"));
	}

	[TestMethod]
	public void Seed_IncompatibleNameCollision_IsReportedWithoutConvertingIt()
	{
		using var context = Context();
		var custom = new ArtificialIntelligence { Name = MonsterAIStockTemplates.LairGuardian, Type = "Animal", Definition = "<custom/>" };
		context.ArtificialIntelligences.Add(custom); context.SaveChanges();
		Assert.ThrowsException<InvalidOperationException>(() => MonsterAIStockTemplates.Seed(context, "Mythical"));
		Assert.AreEqual("Animal", custom.Type); Assert.AreEqual("<custom/>", custom.Definition);
	}

	[TestMethod]
	public void StockDefinitions_UseNoFoodMotiveAndHavePhysicalTacticBindings()
	{
		foreach (var name in MonsterAIStockTemplates.Names)
		{
			var xml = MonsterAIStockTemplates.Definition(name);
			Assert.AreEqual("1", xml.Element("Monster")!.Attribute("version")!.Value);
			Assert.AreEqual("Off", xml.Element("Monster")!.Element("Feeding")!.Value);
			Assert.IsFalse(xml.Descendants("UseActiveNeeds").Any());
			Assert.IsFalse(xml.Descendants("Motive").Any(x => x.Value == "Hunger"));
			if (xml.Element("Hunting")!.Element("Followup")!.Value == "Extract")
				Assert.IsFalse(string.IsNullOrEmpty(xml.Element("Hunting")!.Element("PreferredLayer")!.Value), name);
		}
		var conditional = MonsterAIStockTemplates.Definition(MonsterAIStockTemplates.ConditionalHunter);
		Assert.IsNull(conditional.Descendants("Moon").FirstOrDefault(), "No lunar condition is imposed by stock werewolf recommendations.");
		Assert.AreEqual("Condition", conditional.Element("Monster")!.Elements("Motive").First().Value);
	}

	[TestMethod]
	public void RecommendationManifest_IsDeterministicAndMatchesCurrentSources()
	{
		var expected = MonsterAIStockTemplates.RecommendationManifestJson();
		var path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DatabaseSeeder", "Assets", "Manifests", "Monster_AI_Recommendations.json"));
		var actual = File.ReadAllText(path).Replace("\r\n", "\n");
		Assert.AreEqual(expected.Replace("\r\n", "\n"), actual);
		using var document = JsonDocument.Parse(actual);
		Assert.AreEqual(89, document.RootElement.GetProperty("Recommendations").GetArrayLength());
	}
}
