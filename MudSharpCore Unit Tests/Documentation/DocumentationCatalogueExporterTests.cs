#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Documentation.Export;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace MudSharp_Unit_Tests.Documentation;

[TestClass]
public sealed class DocumentationCatalogueExporterTests
{
	[TestMethod]
	public void CatalogueContainsAllCodeBackedMetadataFamiliesAndGroupsOverloads()
	{
		var generatedAt = new DateTimeOffset(2026, 7, 15, 0, 0, 0, TimeSpan.Zero);
		var catalogue = DocumentationCatalogueExporter.CreateCatalogue(null, new string('a', 40), generatedAt);

		Assert.IsTrue(catalogue.Commands.Count > 100);
		Assert.IsTrue(catalogue.ProgFunctions.Count > 100);
		Assert.IsTrue(catalogue.ProgTypes.Count > 50);
		Assert.IsTrue(catalogue.CollectionExtensions.Count > 5);
		Assert.IsTrue(catalogue.ItemComponents.Count > 100);
		Assert.AreEqual(catalogue.ProgFunctions.Count, catalogue.ProgFunctions.Select(function => function.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
		Assert.IsTrue(catalogue.Commands.Any(command => command.Audience == "admin"));
		Assert.IsTrue(catalogue.Commands.Any(command => command.ConditionalHelp.Count > 0));
		Assert.IsTrue(catalogue.ProgFunctions
			.SelectMany(function => function.Overloads)
			.All(overload => !string.IsNullOrWhiteSpace(overload.GeneralHelp)));
		Assert.IsTrue(catalogue.ProgFunctions
			.SelectMany(function => function.Overloads)
			.SelectMany(overload => overload.Parameters)
			.All(parameter => !string.IsNullOrWhiteSpace(parameter.Help)));
		Assert.IsTrue(catalogue.ProgFunctions
			.SelectMany(function => function.Overloads)
			.Any(overload => overload.Parameters.Count > 0 && overload.Help != overload.GeneralHelp));
	}

	[TestMethod]
	public void CatalogueGenerationIsDeterministicForFixedInputs()
	{
		var generatedAt = new DateTimeOffset(2026, 7, 15, 0, 0, 0, TimeSpan.Zero);
		var first = DocumentationCatalogueExporter.CreateCatalogue(null, new string('b', 40), generatedAt);
		var second = DocumentationCatalogueExporter.CreateCatalogue(null, new string('b', 40), generatedAt);

		Assert.AreEqual(JsonSerializer.Serialize(first), JsonSerializer.Serialize(second));
	}

	[TestMethod]
	public async Task FileExportRequiresACompleteCommitSha()
	{
		var path = Path.Combine(Path.GetTempPath(), $"futuremud-documentation-{Guid.NewGuid():N}.json");
		await Assert.ThrowsExceptionAsync<ArgumentException>(() =>
			DocumentationCatalogueExporter.ExportAsync(path, "development"));
		Assert.IsFalse(File.Exists(path));
	}

	[TestMethod]
	public void PrerequisiteAudit_AttributesWearableStockToItsActualAnatomySeeders()
	{
		var repository = new DirectoryInfo(AppContext.BaseDirectory);
		while (repository is not null && !Directory.Exists(Path.Combine(repository.FullName, "Design Documents", "Data"))) repository = repository.Parent;
		Assert.IsNotNull(repository);
		var temporary = Directory.CreateTempSubdirectory("FutureMUD-wearable-owner-audit-");
		try
		{
			var data = Directory.CreateDirectory(Path.Combine(temporary.FullName, "Design Documents", "Data"));
			var seeding = Directory.CreateDirectory(Path.Combine(temporary.FullName, "Design Documents", "Seeding"));
			foreach (var file in new[] { "Seeded_Item_Components.json", "Seeded_Materials.json", "Seeded_Liquids.json", "Seeded_Gases.json", "SeededTagHierarchy.csv" })
				File.Copy(Path.Combine(repository.FullName, "Design Documents", "Data", file), Path.Combine(data.FullName, file));
			var result = IndustrialisedPrerequisiteAuditExporter.Run(temporary.FullName, false);
			Assert.AreEqual(0, result.Errors.Count, string.Join(Environment.NewLine, result.Errors));
			var row = File.ReadLines(Path.Combine(seeding.FullName, "Industrialised_Component_Prerequisite_Audit.tsv"))
				.Single(x => x.StartsWith("Wearable\t", StringComparison.Ordinal)).Split('\t');
			Assert.AreEqual("HumanSeeder;AnimalSeeder", row[9]);
			using var types = JsonDocument.Parse(File.ReadAllText(Path.Combine(data.FullName, "Item_Component_Types.json")));
			var exported = types.RootElement.EnumerateArray().ToArray();
			Assert.AreEqual(244, exported.Length);
			Assert.IsTrue(exported.Single(x => x.GetProperty("Component Type Name").GetString() == "ActiveCraft")
				.GetProperty("Prevents Manual Load").GetBoolean());
			Assert.IsFalse(exported.Single(x => x.GetProperty("Component Type Name").GetString() == "Wearable")
				.GetProperty("Prevents Manual Load").GetBoolean());
			Assert.IsTrue(exported.All(x => x.TryGetProperty("Prevents Manual Load", out _)));
			Assert.AreEqual(0, IndustrialisedPrerequisiteAuditExporter.Run(temporary.FullName, true).Errors.Count);
		}
		finally { temporary.Delete(true); }
	}
}
