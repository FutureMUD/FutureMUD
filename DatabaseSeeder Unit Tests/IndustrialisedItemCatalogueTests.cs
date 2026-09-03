#nullable enable

using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class IndustrialisedItemCatalogueTests
{
	[TestMethod]
	public void Catalogue_LoadsExactAllocationAndGraphCoverage()
	{
		var document = ItemSeeder.IndustrialisedCatalogueForTesting;
		Assert.AreEqual(6450, document.Items.Count);
		Assert.AreEqual(5800, document.Items.Count(x => x.Layer == "shared-industrialised"));
		Assert.AreEqual(650, document.Items.Count(x => x.Layer == "industrial"));
		Assert.AreEqual(24, document.Items.Select(x => x.Domain).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(6450, document.Items.Select(x => x.StableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(document.Crafts.Select(x => x.ProductStableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count() >= 2258);
		Assert.IsTrue(document.Items.Count(x => x.LifecycleKind is not null) >= 1290);
		Assert.AreEqual(100, document.Outfits.Count);
	}

	[TestMethod]
	public void Catalogue_UsesSubstantiveProseSingleWordNounsAndAdmissionReadyPrices()
	{
		var document = ItemSeeder.IndustrialisedCatalogueForTesting;
		var readyEvidence = document.PriceEvidence
			.Where(x => x.NominalPrice > 0 && x.DailyWage > 0 && x.LabourDays > 0 && x.CostIndex > 0)
			.Select(x => x.EvidenceId)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(document.Items.All(x => !x.Noun.Contains(' ')));
		Assert.IsTrue(document.Items.All(x => x.FullDescription.Length >= 140));
		Assert.IsTrue(document.Items.All(x => x.PriceEvidence.Any(readyEvidence.Contains)));
		Assert.IsTrue(document.Items.All(x => !x.FullDescription.Contains("Sears", StringComparison.OrdinalIgnoreCase)));
		Assert.IsTrue(document.Items.All(x => string.IsNullOrWhiteSpace(x.SourceNote) == false));
		Assert.AreEqual(document.Items.Count, document.Items
			.Select(x => $"{x.Noun}\u001f{x.ShortDescription}\u001f{x.FullDescription}")
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.Count());
	}

	[TestMethod]
	public void Catalogue_MatchesExactDomainAllocationControl()
	{
		var expected = new Dictionary<string, (int Shared, int Industrial)>(StringComparer.OrdinalIgnoreCase)
		{
			["Clothing, footwear and uniforms"] = (600, 70), ["PPE and ballistic protection"] = (150, 35),
			["Household furniture and storage"] = (390, 25), ["Kitchen and appliances"] = (325, 25),
			["Food, drink and packaged consumables"] = (650, 50), ["Cleaning and personal care"] = (170, 10),
			["Office, school, mail and payment"] = (220, 35), ["Computing and networking"] = (180, 0),
			["Telecom, media and photography"] = (220, 30), ["Electrical power and lighting"] = (230, 30),
			["Automation and access control"] = (100, 5), ["Tools, repair and machinery"] = (390, 55),
			["Construction and utilities"] = (210, 30), ["Medical and mobility"] = (290, 25),
			["Weapons, police and military"] = (270, 35), ["Transport support and spares"] = (210, 40),
			["Agriculture, forestry and fishing"] = (150, 25), ["Logistics, retail and hospitality"] = (220, 35),
			["Science and education"] = (140, 25), ["Sports, recreation and music"] = (225, 10),
			["Emergency, rescue and civic"] = (125, 15), ["Raw materials, chemicals and waste"] = (140, 25),
			["Religious and institutional"] = (80, 5), ["Printed media and signage"] = (115, 10)
		};
		var document = ItemSeeder.IndustrialisedCatalogueForTesting;
		foreach (var (domain, counts) in expected)
		{
			Assert.AreEqual(counts.Shared, document.Items.Count(x => x.Domain == domain && x.Layer == "shared-industrialised"), domain);
			Assert.AreEqual(counts.Industrial, document.Items.Count(x => x.Domain == domain && x.Layer == "industrial"), domain);
		}
	}

	[TestMethod]
	public void TechnologyProfiles_DefineEveryDimensionAndCustomAnswersNormalize()
	{
		var document = ItemSeeder.IndustrialisedCatalogueForTesting;
		foreach (var profile in new[] { "neutral", "northamerican", "continentaleuropean", "britishirish", "australasian", "japanese", "chinese" })
		{
			Assert.AreEqual(5, document.TechnologyBindings.Count(x => x.Profile == profile), profile);
		}
		var normalized = new ItemSeeder().NormalizeAnswers(new Dictionary<string, string>
		{
			["eras"] = "revolution medieval industrial",
			["technologyprofile"] = " Custom ",
			["technologypower"] = "Zulu, alpha, zulu",
			["technologypaper"] = "Letter, A4"
		});
		Assert.AreEqual("medieval industrial", normalized["eras"]);
		Assert.AreEqual("custom", normalized["technologyprofile"]);
		Assert.AreEqual("alpha, Zulu", normalized["technologypower"]);
		Assert.AreEqual("A4, Letter", normalized["technologypaper"]);
	}

	[TestMethod]
	public void ComponentMetadata_CoversRuntimeRegistryAndEveryCatalogueBinding()
	{
		var metadata = IndustrialisedComponentMetadataCatalogue.Document;
		var document = ItemSeeder.IndustrialisedCatalogueForTesting;
		Assert.AreEqual(244, metadata.Types.Count);
		Assert.IsTrue(metadata.Prototypes.Count >= 4402);
		var seededNames = metadata.Prototypes.Values
			.Select(x => x.Name)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		Assert.IsTrue(document.Items
			.SelectMany(x => x.FixedComponents)
			.Concat(document.TechnologyBindings.Where(x => x.ComponentBacked).SelectMany(x => x.Values))
			.All(seededNames.Contains));
	}

	[TestMethod]
	public void GeneratedCatalogueAuditAndSyncScriptAreCheckedIn()
	{
		var root = DatabaseSeeder.ItemSeederManifestCatalogue.FindRepositoryRoot();
		Assert.IsTrue(File.Exists(Path.Combine(root, "scripts", "sync-industrialised-item-catalogue.ps1")));
		var audit = File.ReadAllLines(Path.Combine(root, "Design Documents", "Seeding", "Industrialised_Item_Catalogue_Audit.tsv"));
		Assert.AreEqual(6451, audit.Length);
		Assert.IsTrue(audit.Skip(1).All(x => x.EndsWith("\tvalid", StringComparison.Ordinal)));
	}
}
