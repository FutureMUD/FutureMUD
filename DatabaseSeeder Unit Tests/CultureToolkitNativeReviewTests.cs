#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using DatabaseSeeder;
using DatabaseSeeder.Seeders;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitNativeReviewTests
{
	internal static Dictionary<string, Language> Available(string era)
	{
		var catalogue = new CultureToolkitCatalogue();
		var result = catalogue.Compose(era).Languages.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Language { Name = CultureToolkitCatalogue.Text(x.GetProperty("labels"), era) });
		var eras = new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" };
		var aliases = new Dictionary<string, HashSet<string>>();
		foreach (var module in CultureToolkitLanguageBindings.SourceModuleFirstEra)
		{
			var path = Path.Combine(SeederSourceTestHelper.RepositoryRoot,
				"Design Documents", "Seeding", "CultureSeederOriginalCorpus", module.Key + ".json");
			using var document = JsonDocument.Parse(File.ReadAllText(path));
			foreach (var row in document.RootElement.GetProperty("Tables").GetProperty("Language").EnumerateArray())
			{
				var name = row.GetProperty("Name").GetString()!;
				var key = CultureToolkitLanguageBindings.Key(module.Key, name);
				if (!result.ContainsKey(key) && !(key.StartsWith("source.") && Array.IndexOf(eras, era) >= Array.IndexOf(eras, module.Value))) continue;
				result.TryAdd(key, new Language { Name = name });
				if (!aliases.ContainsKey(name)) aliases[name] = [];
				aliases[name].Add(key);
			}
		}
		if (era == "renaissance")
		foreach (var name in CultureSeeder.RenaissanceWorldLanguageNamesForTesting)
		{
			var key = CultureToolkitLanguageBindings.Key("earthrenaissanceworldexpansion", name);
			result.TryAdd(key, new Language { Name = name });
		}
		long id = 1;
		foreach (var language in result.Values) language.Id = id++;
		foreach (var alias in aliases.Where(x => x.Value.Count == 1)) result["legacy:" + alias.Key] = result[alias.Value.Single()];
		return result;
	}

	[TestMethod]
	public void ForeignAccentsReuseExistingAssociationsAndPreserveBuilderEditsOnRerun()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var languages = Available("antiquity");
		context.AddRange(languages.Values.Distinct());
		var latin = languages["latin"];
		var greek = languages["greek.koine"];
		var existing = new Accent { Name = "Existing Greek", Role = 1, Language = latin, AssociatedLanguages = [greek] };
		context.Add(existing);
		context.SaveChanges();
		context.ChangeTracker.Clear();
		languages = languages.ToDictionary(x => x.Key, x => context.Languages.Find(x.Value.Id)!);
		latin = languages["latin"];
		greek = languages["greek.koine"];
		var conflicts = new List<string>();
		var catalogue = new CultureToolkitCatalogue();
		CultureToolkitForeignAccents.Upsert(context, catalogue, "antiquity", languages, conflicts);
		Assert.AreEqual(0, conflicts.Count, string.Join("\n", conflicts));
		Assert.AreEqual(1, latin.Accents.Count(x => x.AssociatedLanguages.Contains(greek)));
		var generated = greek.Accents.Single(x => x.AssociatedLanguages.Contains(latin));
		var count = context.Accents.Count();
		generated.Role = 0;
		generated.Description = "Builder description";
		generated.ChargenAvailabilityProgId = 123;
		generated.AssociatedLanguages.Clear();
		context.SaveChanges();
		CultureToolkitForeignAccents.Upsert(context, catalogue, "antiquity", languages, conflicts);
		Assert.AreEqual(count, context.Accents.Count());
		Assert.AreEqual(0, generated.Role);
		Assert.AreEqual("Builder description", generated.Description);
		Assert.AreEqual(123L, generated.ChargenAvailabilityProgId);
		Assert.AreEqual(0, generated.AssociatedLanguages.Count);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DeferredMissingLanguagesClearOnlyStockNativeDefaults(bool builderEdited)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var source = new Ethnicity { Name = "Chola-Era Telugu" };
		var desired = new Ethnicity { Name = source.Name, NativeLanguageId = 10 };
		var conflicts = new List<string>();
		var resolved = new CultureNativeBinding("fixture", null, "old-template", ["georgian"], [10], []);
		CultureEthnicityDefinition[] definitions = [new("fixture", source, desired, new Dictionary<short, long>(), resolved)];
		var ethnicity = CultureToolkitEthnicities.Upsert(context, "medieval", definitions, new Dictionary<string, Ethnicity>(), conflicts).Ethnicities["fixture"];
		if (builderEdited) ethnicity.NativeLanguageId = 20;
		context.SaveChanges();
		var deferred = CultureToolkitNativeBindings.Source(new CultureToolkitCatalogue(), "medieval", "earthdarkagesandmedieval", source,
			new Dictionary<string, Language> { ["georgian"] = new() { Id = 10 } });
		desired.NativeLanguageId = null;
		definitions = [new("fixture", source, desired, new Dictionary<short, long>(), deferred)];
		CultureToolkitEthnicities.Upsert(context, "medieval", definitions, new Dictionary<string, Ethnicity>(), conflicts);
		Assert.AreEqual(builderEdited ? 20L : (long?)null, ethnicity.NativeLanguageId);
	}

	[TestMethod]
	public void LegacyAccentMetadataLoadsAndPreservesPersistedBuilderAssociations()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var english = new Language { Name = "English" };
		var japanese = new Language { Name = "Japanese" };
		var korean = new Language { Name = "Korean" };
		var accent = new Accent { Language = english, Name = "Japanese", Group = "Japanese" };
		context.AddRange(accent, japanese, korean);
		context.SaveChanges();
		var conflicts = new List<string>();
		CultureStockAccentRoles.ApplyLegacy(context, accent, true, conflicts);
		Assert.AreEqual(1, accent.Role);
		Assert.AreEqual(japanese.Id, accent.AssociatedLanguages.Single().Id);
		accent.Role = 0;
		accent.AssociatedLanguages.Clear();
		accent.AssociatedLanguages.Add(korean);
		context.SaveChanges();
		var accentId = accent.Id;
		var koreanId = korean.Id;
		context.ChangeTracker.Clear();
		accent = context.Accents.Include(x => x.Language).Single(x => x.Id == accentId);
		CultureStockAccentRoles.ApplyLegacy(context, accent, false, conflicts);
		Assert.AreEqual(0, accent.Role);
		Assert.AreEqual(koreanId, accent.AssociatedLanguages.Single().Id);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
