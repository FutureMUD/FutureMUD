#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitProseTests
{
	[TestMethod]
	public void ReviewedSourceCorrectionsKeepNamingRegexesStylesAndAccentEncoding()
	{
		var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Design Documents", "Seeding", "CultureSeederOriginalCorpus"));
		foreach (var module in new[] { "earthantiquity", "earthdarkagesandmedieval", "earthrenaissanceeurope", "earthrenaissanceworldexpansion" })
		{
			using var source = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, module + ".json")));
			var tables = source.RootElement.GetProperty("Tables");
			var languages = tables.GetProperty("Language").EnumerateArray().ToDictionary(x => x.GetProperty("Id").GetInt64(), x => x.GetProperty("Name").GetString()!);
			foreach (var accent in tables.GetProperty("Accent").EnumerateArray())
			foreach (var field in new[] { "Description", "Suffix", "VagueSuffix" })
			{
				var identity = languages[accent.GetProperty("LanguageId").GetInt64()] + ":" + accent.GetProperty("Name").GetString();
				var display = CultureToolkitProse.Display(CultureToolkitProse.Rewrite(module, "Accent", identity, field, accent.GetProperty(field).GetString()!));
				Assert.IsFalse(display.Contains("[U+"), $"{module}:{identity}:{field}: {display}");
				Assert.IsTrue(display.All(x => x <= 255));
			}
			foreach (var culture in tables.GetProperty("NameCulture").EnumerateArray())
			{
				var original = culture.GetProperty("Definition").GetString()!;
				var updated = CultureToolkitProse.Rewrite(module, "NameCulture", culture.GetProperty("Name").GetString()!, "Definition", original);
				var before = XElement.Parse(original);
				var after = XElement.Parse(updated);
				Assert.IsFalse(after.Descendants("Element").Any(x => x.Value.Contains("seeded") || x.Value.Contains("stock") || x.Value.Contains("known from Egyptian") || x.Value.Contains("later generations") || x.Value.Contains("fantasynamegenerators")), updated);
				Assert.IsTrue(XNode.DeepEquals(before.Element("NameEntryRegex"), after.Element("NameEntryRegex")));
				Assert.IsTrue(XNode.DeepEquals(before.Element("Patterns"), after.Element("Patterns")));
				CollectionAssert.AreEqual(before.Descendants("Element").SelectMany(x => x.Attributes()).Select(x => x.ToString()).ToArray(),
					after.Descendants("Element").SelectMany(x => x.Attributes()).Select(x => x.ToString()).ToArray());
			}
		}
	}
}
