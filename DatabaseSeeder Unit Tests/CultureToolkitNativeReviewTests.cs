#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
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
			var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Design Documents", "Seeding", "CultureSeederOriginalCorpus", module.Key + ".json");
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
		long id = 1;
		foreach (var language in result.Values) language.Id = id++;
		foreach (var alias in aliases.Where(x => x.Value.Count == 1)) result["legacy:" + alias.Key] = result[alias.Value.Single()];
		return result;
	}

	[DataTestMethod]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	public void EveryExpandedEthnicityResolvesOrHasAnExplicitMissingLanguage(string era)
	{
		var catalogue = new CultureToolkitCatalogue();
		var module = era == "renaissance" ? "earthrenaissanceworldexpansion" : "earthdarkagesandmedieval";
		var coverage = era == "renaissance" ? CultureSeeder.RenaissanceWorldEthnicityNameCulturesForTesting : CultureSeeder.DarkAgesAndMedievalEthnicityNameCulturesForTesting;
		var languages = Available(era);
		var errors = new List<string>();
		foreach (var row in coverage)
		{
			var ethnicity = new Ethnicity { Name = row.Key };
			ethnicity.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { NameCulture = new NameCulture { Name = row.Value } });
			var binding = CultureToolkitNativeBindings.Source(catalogue, era, module, ethnicity, languages);
			if (!binding.IsResolved && binding.Rule != "reviewed-missing-language") errors.AddRange(binding.Unresolved);
			if (binding.Rule == "reviewed-missing-language") Assert.AreEqual(0, binding.LanguageIds.Count);
		}
		Assert.AreEqual(0, errors.Count, string.Join(Environment.NewLine, errors));
	}

	[DataTestMethod]
	[DataRow("antiquity")]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	public void ReviewedNativeDefaultsAndNeighbourAccentsUseInstalledEraLanguages(string era)
	{
		var catalogue = new CultureToolkitCatalogue();
		var languages = Available(era);
		foreach (var row in catalogue.Document("data.legacy_native_language_rules.json").GetProperty("exact_source_bindings").EnumerateArray()
			.Where(x => CultureToolkitCatalogue.Strings(x.GetProperty("packs")).Contains(era)))
		foreach (var reference in CultureToolkitCatalogue.Strings(row.GetProperty("languages"))) Assert.IsTrue(languages.ContainsKey(reference), reference);
		foreach (var culture in catalogue.Compose(era).Cultures)
		foreach (var reference in CultureToolkitSocialCultures.NativeReferences(catalogue, culture, era)) Assert.IsTrue(languages.ContainsKey(reference), reference);
		foreach (var ethnicity in catalogue.Compose(era).Ethnicities)
		{
			var binding = CultureToolkitNativeBindings.Canonical(catalogue, era, CultureToolkitCatalogue.Text(ethnicity, "key"), languages);
			Assert.IsTrue(binding.IsResolved, string.Join("\n", binding.Unresolved));
		}
		foreach (var row in catalogue.Document("data.historical_foreign_accents.json").EnumerateArray()
			.Where(x => CultureToolkitCatalogue.Strings(x.GetProperty("packs")).Contains(era)))
		{
			Assert.IsTrue(languages.ContainsKey(CultureToolkitCatalogue.Text(row, "target")), row.ToString());
			Assert.IsTrue(languages.ContainsKey(CultureToolkitCatalogue.Text(row, "source")), row.ToString());
		}
	}

	[DataTestMethod]
	[DataRow("antiquity", "earthantiquity", "Antiquity")]
	[DataRow("medieval", "earthrenaissanceeurope", "MedievalEurope")]
	[DataRow("renaissance", "earthrenaissanceeurope", "MedievalEurope")]
	public void EveryRetainedAncientAndEuropeanEthnicityHasAResolvedDefault(string era, string module, string suffix)
	{
		var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "DatabaseSeeder", "Seeders", "CultureSeeder", $"CultureSeeder.Heritage.{suffix}.cs");
		var names = Regex.Matches(File.ReadAllText(path), "AddEthnicity\\(_humanRace, \"([^\"]+)\"")
			.Select(x => x.Groups[1].Value).Distinct().ToArray();
		Assert.IsTrue(names.Length >= 50);
		var catalogue = new CultureToolkitCatalogue();
		var languages = Available(era);
		foreach (var name in names)
		{
			var binding = CultureToolkitNativeBindings.Source(catalogue, era, module, new Ethnicity { Name = name }, languages);
			Assert.IsTrue(binding.IsResolved, string.Join("\n", binding.Unresolved));
		}
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

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DeferredMissingLanguagesClearOnlyStockNativeDefaults(bool builderEdited)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var source = new Ethnicity { Name = "Renaissance Mingrelian" };
		var desired = new Ethnicity { Name = source.Name, NativeLanguageId = 10 };
		var conflicts = new List<string>();
		var resolved = new CultureNativeBinding("fixture", null, "old-template", ["georgian"], [10], []);
		CultureEthnicityDefinition[] definitions = [new("fixture", source, desired, new Dictionary<short, long>(), resolved)];
		var ethnicity = CultureToolkitEthnicities.Upsert(context, "renaissance", definitions, new Dictionary<string, Ethnicity>(), conflicts).Ethnicities["fixture"];
		if (builderEdited) ethnicity.NativeLanguageId = 20;
		context.SaveChanges();
		var deferred = CultureToolkitNativeBindings.Source(new CultureToolkitCatalogue(), "renaissance", "earthrenaissanceworldexpansion", source,
			new Dictionary<string, Language> { ["georgian"] = new() { Id = 10 } });
		desired.NativeLanguageId = null;
		definitions = [new("fixture", source, desired, new Dictionary<short, long>(), deferred)];
		CultureToolkitEthnicities.Upsert(context, "renaissance", definitions, new Dictionary<string, Ethnicity>(), conflicts);
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

	[DataTestMethod]
	[DataRow("Coptic", "Greek", "Greek", 1)]
	[DataRow("Latin", "Greek", "Greek", 1)]
	[DataRow("Koine Greek", "Doric", "Greek", 1)]
	[DataRow("Hausa", "West African", "west african", 1)]
	[DataRow("Literary Chinese", "Japanese Kanbun", "Literary Sinitic Reading", 1)]
	[DataRow("Classical Chinese", "Korean-reading", "Korean scholastic", 1)]
	[DataRow("Westron", "Dwarven", "Dwarven", 1)]
	[DataRow("Ukranian", "Balachka", "foreign", 0)]
	[DataRow("Latin", "Foreign", "Foreign", 2)]
	[DataRow("Latin", "Ecclesiastical", "learned", 0)]
	public void RolesDistinguishForeignSpeakersRegionalDialectsAndFallbacks(string language, string accent, string group, int role)
		=> Assert.AreEqual(role, (int)CultureStockAccentRoles.Role(language, accent, group));
}
