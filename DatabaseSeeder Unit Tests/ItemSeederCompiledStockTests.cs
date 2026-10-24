#nullable enable

using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text.Json;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederCompiledStockTests
{
	public TestContext TestContext { get; set; } = null!;
	[TestMethod]
	public void IndustrialStock_AllFieldsAndEvidenceMatchFrozenSource()
	{
		var compiled = IndustrialisedItemCatalogue.Document;
		Assert.AreEqual(6450, compiled.Items.Count);
		Assert.AreEqual(17, compiled.PriceEvidence.Count);
		FrozenCatalogueContract.AssertGraph("industrialised");
		foreach (var item in CompiledStockCatalogue.Industrial)
		{
			CatalogueValidator.Validate(item);
			Assert.IsNotNull(item.Industrial);
		}
	}

	[TestMethod]
	public void MedicalStock_AllFieldsCategoriesAndNotesMatchFrozenSource()
	{
		Assert.AreEqual(178, EraMedicalRepairCatalogue.Entries.Count);
		Assert.AreEqual(134, EraMedicalRepairCatalogue.Renaissance.Count);
		Assert.AreEqual(44, EraMedicalRepairCatalogue.EarlyModern.Count);
		FrozenCatalogueContract.AssertGraph("renaissanceMedical");
		FrozenCatalogueContract.AssertGraph("earlyModernMedical");
		foreach (var item in CompiledStockCatalogue.Medical)
		{
			CatalogueValidator.Validate(item);
			Assert.IsFalse(string.IsNullOrWhiteSpace(item.MedicalCategory));
			Assert.IsFalse(item.AllowLegacyShortDescriptionMatch);
		}
	}

	[TestMethod]
	public void SourceGraphs_MatchAllIndependentOriginalFamilies()
	{
		FrozenCatalogueContract.AssertSourceGraphs();
	}

	[TestMethod]
	public void ContentAssemblies_StayWithinStringBudgetAndNeverReferenceInstaller()
	{
		var assemblies = typeof(ItemSeeder).Assembly.GetReferencedAssemblies()
			.Where(reference => reference.Name!.StartsWith("DatabaseSeeder.Catalogue.", StringComparison.Ordinal))
			.Select(Assembly.Load)
			.ToArray();
		Assert.IsTrue(assemblies.Length >= 47, "Every compiled content assembly must be included in the size check.");
		foreach (var assembly in assemblies)
		{
			using var stream = File.OpenRead(assembly.Location);
			using var pe = new PEReader(stream);
			var bytes = pe.GetMetadataReader().GetHeapSize(HeapIndex.UserString);
			TestContext.WriteLine($"{assembly.GetName().Name}: {bytes} user-string bytes.");
			Assert.IsTrue(bytes < 4 * 1024 * 1024, $"{assembly.GetName().Name}: {bytes:N0} user-string bytes; split the batch before adding content.");
			Assert.IsFalse(assembly.GetReferencedAssemblies().Any(reference => reference.Name == "DatabaseSeeder"));
		}
	}

	[TestMethod]
	public void SourceFingerprint_TracksProjectWiringAndIgnoresBuildOutputs()
	{
		var root = Path.Combine(Path.GetTempPath(), "stock-fingerprint-" + Guid.NewGuid().ToString("N"));
		var project = Path.Combine(root, "DatabaseSeeder", "Seeders", "ItemSeeder", "Catalogue", "Assemblies", "ContentBatch");
		Directory.CreateDirectory(project);
		try
		{
			File.WriteAllText(Path.Combine(project, "Stock.cs"), "stock");
			var first = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
			File.WriteAllText(Path.Combine(project, "Stock.csproj"), "project");
			var second = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
			Assert.AreNotEqual(first, second);
			foreach (var output in new[] { "bin", "obj" })
			{
				Directory.CreateDirectory(Path.Combine(project, output));
				File.WriteAllText(Path.Combine(project, output, "Generated.cs"), "generated");
				File.WriteAllText(Path.Combine(project, output, "Generated.csproj"), "generated");
				File.WriteAllText(Path.Combine(project, output, "Generated.targets"), "generated");
			}
			Assert.AreEqual(second, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
			File.WriteAllText(Path.Combine(project, "Stock.targets"), "publishing");
			Assert.AreNotEqual(second, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
			second = ItemSeederManifestCatalogue.ComputeSourceFingerprint(root);
			File.WriteAllText(Path.Combine(root, "DatabaseSeeder", "DatabaseSeeder.csproj"), "references");
			Assert.AreNotEqual(second, ItemSeederManifestCatalogue.ComputeSourceFingerprint(root));
		}
		finally { Directory.Delete(root, true); }
	}

	[TestMethod]
	[DataRow(true, false)]
	[DataRow(false, true)]
	[DataRow(false, false)]
	public void IndustrialTypedWriter_PreservesCommonFieldsSkinPermissionsAndCustomizations(bool authoredSkins, bool clothingSkins)
	{
		using var context = ItemSeederUnifiedCatalogueTests.Fixture();
		var garment = RepresentativeCatalogue.Braies();
		var definition = CompiledStockCatalogue.Industrial.First(item => item.MorphToStableReference is null && item.DestroyedItemStableReference is null) with
		{
			Noun = "tool", ShortDescription = "a catalogue test tool", Material = "linen", Tags = garment.Tags,
			Components = garment.Components, LongDescription = "An authored catalogue tool rests here.",
			BuilderNotes = "authored stock notes", HiddenFromPlayers = true, Skinnable = authoredSkins
		};
		var item = new ItemSeeder().InstallIndustrialCatalogueItemForTesting(context, definition, clothingSkins);
		context.SaveChanges();
		Assert.AreEqual(definition.LongDescription, item.LongDescription);
		Assert.IsTrue(item.IsHiddenFromPlayers);
		Assert.AreEqual(authoredSkins || clothingSkins, item.PermitPlayerSkins);
		Assert.IsNull(item.BuilderNotes, "Existing stock policy keeps source notes in the catalogue, not builder notes.");
		var id = item.Id;
		item.BuilderNotes = "authored stock notes\nBuilder-authored note.";
		context.SaveChanges();
		var reconciled = new ItemSeeder().InstallIndustrialCatalogueItemForTesting(context, definition, clothingSkins);
		Assert.AreEqual("Builder-authored note.", reconciled.BuilderNotes, "Forward stock notes to the existing cleanup policy without removing builder notes.");
		item.LongDescription = "The builder's custom tool.";
		item.Weight = 2345;
		context.SaveChanges();
		var retained = new ItemSeeder().InstallIndustrialCatalogueItemForTesting(context, definition, clothingSkins);
		Assert.AreEqual(id, retained.Id);
		Assert.AreEqual(2345.0, retained.Weight);
		Assert.AreEqual("The builder's custom tool.", retained.LongDescription);
		Assert.AreEqual("Builder-authored note.", retained.BuilderNotes);
	}

	[TestMethod]
	[DataRow("neutral")]
	[DataRow("northamerican")]
	[DataRow("continentaleuropean")]
	[DataRow("britishirish")]
	[DataRow("australasian")]
	[DataRow("japanese")]
	[DataRow("chinese")]
	[DataRow("custom")]
	public void IndustrialSelection_EveryTechnologyProfilePreservesFullDefinitions(string profile)
	{
		using var context = Program.CreateItemManifestCaptureContext();
		var seeder = new ItemSeeder();
		var capture = seeder.CaptureFullCatalogue(context, ItemSeederManifestCatalogue.FindRepositoryRoot(),
			"antiquity medieval renaissance earlymodern industrial", profile);
		FrozenCatalogueContract.AssertScenario("profile-" + profile, seeder, capture);
	}
}
