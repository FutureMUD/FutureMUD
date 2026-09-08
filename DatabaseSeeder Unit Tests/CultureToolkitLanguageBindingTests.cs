#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitLanguageBindingTests
{
	[TestMethod]
	public void EveryDeclaredBindingNamesAnActualHistoricalSourceAndApprovedCanonicalStage()
	{
		var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
			"Design Documents", "Seeding", "CultureSeederOriginalCorpus"));
		var catalogue = new CultureToolkitCatalogue();
		var keys = catalogue.Document("data.language_identity_and_labels.json").EnumerateArray()
			.Where(x => CultureToolkitCatalogue.Text(x, "entity_kind") == "language-specification")
			.Select(x => CultureToolkitCatalogue.Text(x, "key")).ToHashSet();
		foreach (var group in CultureToolkitLanguageBindings.All.GroupBy(x => x.SourcePack))
		{
			using var source = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, group.Key + ".json")));
			var names = source.RootElement.GetProperty("Tables").GetProperty("Language").EnumerateArray()
				.Select(x => x.GetProperty("Name").GetString()).ToHashSet();
			foreach (var binding in group)
			{
				Assert.IsTrue(names.Contains(binding.SourceName), $"{group.Key}:{binding.SourceName}");
				Assert.IsTrue(keys.Contains(binding.CanonicalKey) || binding.CanonicalKey.StartsWith("source."), binding.CanonicalKey);
			}
		}
		Assert.AreEqual(CultureToolkitLanguageBindings.All.Count,
			CultureToolkitLanguageBindings.All.Select(x => (x.SourcePack, x.SourceName)).Distinct().Count());
		Assert.AreNotEqual(CultureToolkitLanguageBindings.Key("earthantiquity", "Attic Greek"),
			CultureToolkitLanguageBindings.Key("earthrenaissanceeurope", "Greek"));
		Assert.AreNotEqual(CultureToolkitLanguageBindings.Key("earthmodern", "Greek"),
			CultureToolkitLanguageBindings.Key("earthrenaissanceeurope", "Greek"));
		Assert.AreNotEqual(CultureToolkitLanguageBindings.Key("earthantiquity", "Venetic"),
			CultureToolkitLanguageBindings.Key("earthrenaissanceeurope", "Venetian"));
	}
}
