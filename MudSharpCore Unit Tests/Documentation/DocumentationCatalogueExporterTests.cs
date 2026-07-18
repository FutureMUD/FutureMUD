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
}
