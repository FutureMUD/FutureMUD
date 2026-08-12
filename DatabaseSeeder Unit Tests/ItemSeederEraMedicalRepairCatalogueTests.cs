#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemSeederEraMedicalRepairCatalogueTests
{
	[TestMethod]
	public void EraMedicalRepairCatalogues_HaveTheContractedCountsAndUniqueProducts()
	{
		Assert.AreEqual(134, EraMedicalRepairCatalogue.Renaissance.Count);
		Assert.AreEqual(44, EraMedicalRepairCatalogue.EarlyModern.Count);

		var items = EraMedicalRepairCatalogue.Renaissance
			.Concat(EraMedicalRepairCatalogue.EarlyModern)
			.ToArray();
		Assert.AreEqual(178, items.Length);
		Assert.AreEqual(items.Length, items.Select(x => x.StableReference).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.AreEqual(items.Length, items.Select(x => x.ShortDescription).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(items.All(x => x.FullDescription.Count(c => c == '.') == 3));
		Assert.IsTrue(items.All(x => x.Tags.Length == x.Tags.Distinct(StringComparer.OrdinalIgnoreCase).Count()));
		Assert.IsTrue(items.All(x => x.Components.Length == x.Components.Distinct(StringComparer.OrdinalIgnoreCase).Count()));
		Assert.IsTrue(items.All(x => HasCorrectArticle(x.ShortDescription)));
		Assert.IsFalse(items.Any(x => x.StableReference.Contains("_pass_", StringComparison.OrdinalIgnoreCase) ||
		                         x.StableReference.Contains("_expansion_", StringComparison.OrdinalIgnoreCase)));
		Assert.IsFalse(items.Any(x => x.StableReference.Contains("_college_", StringComparison.OrdinalIgnoreCase) ||
		                         x.StableReference.Contains("_infirmary_", StringComparison.OrdinalIgnoreCase) ||
		                         x.StableReference.Contains("_academy_", StringComparison.OrdinalIgnoreCase)),
			"Institutional ownership is not a stock-item variant.");
		Assert.IsTrue(items.All(x => x.Tags.All(tag => !x.Tags.Any(other => !string.Equals(tag, other, StringComparison.OrdinalIgnoreCase) && other.StartsWith(tag + " / ", StringComparison.OrdinalIgnoreCase)))),
			"Catalogue rows should carry only the most specific tag in each hierarchy.");
		var cuppingGlass = items.First(x => x.StableReference.Contains("cupping_glass", StringComparison.OrdinalIgnoreCase));
		Assert.AreEqual("glass", cuppingGlass.Material);
		Assert.IsFalse(cuppingGlass.FullDescription.Contains("wrought iron", StringComparison.OrdinalIgnoreCase));
		foreach (var drug in items.Where(x => x.Category == "Drugs delivery"))
		{
			var delivery = drug.Components.Single(x => x.StartsWith("Pill_", StringComparison.Ordinal) ||
				x.StartsWith("TopicalCream_", StringComparison.Ordinal) || x.StartsWith("Smokeable_", StringComparison.Ordinal) ||
				x.StartsWith("LContainer_Medicine_", StringComparison.Ordinal));
			var stem = delivery.Replace("LContainer_Medicine_", "", StringComparison.Ordinal)
				.Replace("Pill_", "", StringComparison.Ordinal).Replace("TopicalCream_", "", StringComparison.Ordinal)
				.Replace("Smokeable_", "", StringComparison.Ordinal).Replace("_100ml", "", StringComparison.Ordinal)
				.Replace('_', ' ');
			Assert.IsTrue(Normalise(drug.ShortDescription).Contains(Normalise(stem), StringComparison.Ordinal),
				$"Delivery component must match its named medicine: {drug.StableReference}");
		}

		Assert.AreEqual(20, EraMedicalRepairCatalogue.Renaissance.Count(x => x.Category == "Clinical surgery"));
		Assert.AreEqual(15, EraMedicalRepairCatalogue.EarlyModern.Count(x => x.Category == "Drugs delivery"));
		AssertRepresentativeComponent(items, "adjustable_crutch", "Crutch");
		AssertRepresentativeComponent(items, "aromatic_vinegar_cloth", "Antiseptic_Single");
		AssertRepresentativeComponent(items, "animal_bandage", "Bandage_Simple");
		AssertRepresentativeComponent(items, "drag_harness", "DragAid_Harness");
		AssertRepresentativeComponent(items, "suture_needle", "Suture_Single");
		AssertRepresentativeComponent(items, "barber_surgeon_case", "FieldMedkit");

		Assert.IsFalse(EraMedicalRepairCatalogue.Renaissance.Any(x =>
			x.StableReference.Contains("cinchona", StringComparison.OrdinalIgnoreCase) ||
			x.StableReference.Contains("ipecacuanha", StringComparison.OrdinalIgnoreCase) ||
			x.StableReference.Contains("variolation", StringComparison.OrdinalIgnoreCase) ||
			x.StableReference.Contains("inoculation", StringComparison.OrdinalIgnoreCase)));
		foreach (var historicalGapProp in new[]
		{
			"cinchona_bark_stock", "ipecacuanha_root_stock", "ergot_stock", "bezoar_stock", "mummy_powder",
			"powdered_pearl", "sympathetic_powder", "weapon_salve", "royal_touch_token", "astrological_diagnostic_glass"
		})
		{
			Assert.IsTrue(EraMedicalRepairCatalogue.Renaissance
				.Concat(EraMedicalRepairCatalogue.EarlyModern)
				.Any(x =>
				x.StableReference.Contains(historicalGapProp, StringComparison.OrdinalIgnoreCase)),
				$"Expected Early Modern gap prop {historicalGapProp}.");
		}
	}

	[TestMethod]
	public void EraMedicalRepairCatalogues_OnlyUseMaintainedDependencies()
	{
		var items = EraMedicalRepairCatalogue.Entries;
		var components = ReadJsonNames("Seeded_Item_Components.json", "Component Name");
		var materials = ReadJsonNames("Seeded_Materials.json", "Material Name");
		var tags = File.ReadAllLines(PathFromRoot("Design Documents", "Data", "SeededTagHierarchy.csv"))
			.Skip(1).Select(x => x.Split('\t').Last()).ToHashSet(StringComparer.OrdinalIgnoreCase);

		foreach (var item in items)
		{
			Assert.IsTrue(materials.Contains(item.Material), $"Missing material export for {item.StableReference}: {item.Material}");
			foreach (var component in item.Components)
				Assert.IsTrue(components.Contains(component), $"Missing component export for {item.StableReference}: {component}");
			foreach (var tag in item.Tags)
				Assert.IsTrue(tags.Contains(tag), $"Missing tag export for {item.StableReference}: {tag}");
		}

		Assert.AreEqual(18, EraMedicalRepairCatalogue.Renaissance.Count(x => x.Category == "Repair"));
		Assert.IsTrue(EraMedicalRepairCatalogue.Renaissance
			.Where(x => x.Category == "Repair")
			.All(x => x.Components.Count(component => component.StartsWith("Repair_", StringComparison.Ordinal)) == 1));
	}

	[TestMethod]
	public void MedicalRepairReferenceAndDispatch_AreKeptInSync()
	{
		var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
		var dispatcher = File.ReadAllText(Path.Combine(root, "DatabaseSeeder", "Seeders", "ItemSeeder.PreIndustrialBaseline.cs"));
		var reference = Path.Combine(root, "Design Documents", "Seeding", "FutureMUD_Renaissance_EarlyModern_Medical_Repair_Design_Reference.md");
		Assert.IsTrue(File.Exists(reference));
		StringAssert.Contains(dispatcher, "SeedRenaissanceMedicalAndRepair();");
		StringAssert.Contains(dispatcher, "SeedEarlyModernMedicalAndRepair();");
		var text = File.ReadAllText(reference);
		StringAssert.Contains(text, "**134**");
		StringAssert.Contains(text, "**44**");
		StringAssert.Contains(text, "engine-extension");
		StringAssert.Contains(text, "prop-only");
	}

	private static void AssertRepresentativeComponent(EraMedicalRepairCatalogueEntry[] items, string referenceFragment, string component)
	{
		Assert.IsTrue(items.Where(x => x.StableReference.Contains(referenceFragment, StringComparison.OrdinalIgnoreCase))
			.All(x => x.Components.Contains(component, StringComparer.OrdinalIgnoreCase)));
	}

	private static bool HasCorrectArticle(string text)
	{
		var noun = text.StartsWith("an ", StringComparison.OrdinalIgnoreCase) ? text[3..] :
			text.StartsWith("a ", StringComparison.OrdinalIgnoreCase) ? text[2..] : string.Empty;
		if (noun.Length == 0) return false;
		return "aeiou".Contains(char.ToLowerInvariant(noun[0]))
			? text.StartsWith("an ", StringComparison.OrdinalIgnoreCase)
			: text.StartsWith("a ", StringComparison.OrdinalIgnoreCase);
	}

	private static string Normalise(string text) => new(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());

	private static System.Collections.Generic.HashSet<string> ReadJsonNames(string file, string property)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(PathFromRoot("Design Documents", "Data", file)));
		return document.RootElement.EnumerateArray().Select(x => x.GetProperty(property).GetString()!)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string PathFromRoot(params string[] parts) => Path.GetFullPath(
		Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", Path.Combine(parts)));
}
