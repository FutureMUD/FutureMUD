#nullable enable

using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitCatalogueTests
{
	[TestMethod]
	public void UnrelatedResourcesCannotHideMissingRequiredInputs()
	{
		foreach (var missing in CultureToolkitCatalogue.RequiredDocuments)
		{
			var names = CultureToolkitCatalogue.RequiredDocuments.Where(x => x != missing).Append("data.unrelated.json");
			StringAssert.Contains(Assert.ThrowsException<InvalidDataException>(() => CultureToolkitCatalogue.ValidateRequiredDocuments(names)).Message, missing);
		}
	}

	[TestMethod]
	public void EveryAuthoredPlayableCellHasTwentyDistinctFamiliesAndDisplays()
	{
		var catalogue = new CultureToolkitCatalogue();
		var requirements = catalogue.Document("data.name_playability_policy.json").GetProperty("requirements").EnumerateArray().ToArray();
		Assert.AreEqual(58, requirements.Length);
		foreach (var requirement in requirements)
		{
			var era = CultureToolkitCatalogue.Text(requirement, "pack");
			var key = CultureToolkitCatalogue.Text(requirement, "pool");
			var gender = CultureToolkitCatalogue.Text(requirement, "gender") == "male" ? (int)MudSharp.Form.Shape.Gender.Male : (int)MudSharp.Form.Shape.Gender.Female;
			var repertoire = CultureToolkitNameCatalogue.Build(catalogue, era).Single(x => x.StableKey == key);
			var profile = repertoire.Culture.RandomNameProfiles.Single(x => x.Gender == gender);
			var elements = profile.RandomNameProfilesElements.Where(x => x.NameUsage == 0).ToArray();
			Assert.IsTrue(elements.Length >= 20, $"{key}:{era}:{gender}");
			Assert.AreEqual(elements.Length, elements.Select(x => x.Name).Distinct(StringComparer.OrdinalIgnoreCase).Count());
			Assert.IsTrue(elements.All(x => x.Weighting > 0));
			Assert.IsFalse(elements.Any(x => x.Name == "Botezata"));
		}
	}

	[DataTestMethod]
	[DataRow("classical", "antiquity", true)]
	[DataRow("liturgical", "antiquity", false)]
	[DataRow("liturgical", "darkages", true)]
	[DataRow("neo-classical", "medieval", false)]
	[DataRow("neo-classical", "renaissance", true)]
	public void ExplicitLatinAccentRulesOverrideSourceModuleDates(string name, string era, bool allowed)
	{
		var policy = CultureToolkitAccentPolicy.Resolve(new CultureToolkitCatalogue(), "earthrenaissanceeurope", "Latin", name, "native", false);
		Assert.AreEqual(allowed, policy.AllowedPacks.Contains(era));
		Assert.AreEqual("native-tradition", policy.Role);
	}

	[TestMethod]
	public void EmbeddedDeliveryMatchesAllOriginalChecksums()
	{
		var root = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..",
			"Design Documents", "Seeding", "CultureSeederRedesignHandoff"));
		var lines = File.ReadAllLines(Path.Combine(root, "JSON_SHA256SUMS.txt"));
		Assert.AreEqual(28, lines.Length);
		foreach (var line in lines)
		{
			var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
			using var stream = typeof(CultureToolkitCatalogue).Assembly.GetManifestResourceStream(
				"CultureToolkit." + parts[1].Replace('/', '.'));
			Assert.IsNotNull(stream, parts[1]);
			Assert.AreEqual(parts[0], Convert.ToHexString(SHA256.HashData(stream!)).ToLowerInvariant(), parts[1]);
		}
	}

	[DataTestMethod]
	[DataRow("antiquity")]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	[DataRow("earlymodern")]
	public void AllEraPlansHaveDistinctLanguageStagesAndDirectedEndpoints(string era)
	{
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		Assert.IsTrue(pack.Cultures.Count > 0);
		Assert.IsTrue(pack.Languages.Count > 0);
		Assert.IsFalse(pack.Languages.Any(x => CultureToolkitCatalogue.Text(x, "key") == "south-slavic"));
		foreach (var group in pack.Groups)
		{
			var key = CultureToolkitCatalogue.Text(group.GetProperty("membership_recipe"), "source_key");
			var candidates = catalogue.Candidates(key, pack);
			Assert.AreEqual(candidates.CanonicalKeys.Count, candidates.CanonicalKeys.Distinct().Count());
			Assert.AreEqual(0, group.GetProperty("minimum_picks").GetInt32());
			Assert.AreEqual(1, group.GetProperty("maximum_picks").GetInt32());
			Assert.AreEqual("CountKnown", CultureToolkitCatalogue.Text(group, "existing_skill_policy"));
		}
	}

	[TestMethod]
	public void WelshNobilityAndOverlapUseExactAuthoredMandatoryFactors()
	{
		var catalogue = new CultureToolkitCatalogue();
		var welsh = catalogue.FixedGrantFactors("medieval", "ethnicity.welsh", "culture.english-nobility");
		Assert.AreEqual(4, welsh.Count);
		Assert.AreEqual(1.0, welsh["welsh"]);
		Assert.AreEqual(0.9, welsh["english.middle"]);
		Assert.AreEqual(0.75, welsh["french.anglonorman"]);
		Assert.AreEqual(0.25, welsh["latin"]);
		var english = catalogue.FixedGrantFactors("medieval", "ethnicity.english", "culture.english-nobility");
		Assert.AreEqual(3, english.Count);
		Assert.AreEqual(1.0, english["english.middle"]);
		var later = catalogue.FixedGrantFactors("earlymodern", "ethnicity.english", "culture.english-nobility");
		CollectionAssert.AreEquivalent(new[] { "english.earlymodern", "latin" }, later.Keys.ToArray());
	}

	[TestMethod]
	public void OptionalEducationQualifiesOrdinaryPurchasesWithoutGrantingThem()
	{
		var catalogue = new CultureToolkitCatalogue();
		var mandatory = catalogue.FixedGrantFactors("medieval", "ethnicity.welsh", "culture.western-european-merchant-class");
		var factors = catalogue.BackgroundFactors("medieval", "ethnicity.welsh", "culture.western-european-merchant-class");
		Assert.IsFalse(mandatory.ContainsKey("latin"));
		Assert.AreEqual(0.5, factors["latin"]);
		Assert.AreEqual(1.0, factors["welsh"]);
	}

	[TestMethod]
	public void RetainedCandidatesRemainUnresolvedAndStageAlternativesAreNotExtraGrants()
	{
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose("medieval");
		var candidates = catalogue.Candidates("zoroastrian-learning", pack);
		CollectionAssert.AreEqual(new[] { "legacy:Avestan" }, candidates.RetainedSourceReferences.ToArray());
		CollectionAssert.AreEqual(new[] { "english.middle" }, catalogue.ResolveSelector("english", "medieval", []).ToArray());
		Assert.ThrowsException<InvalidDataException>(() => catalogue.ResolveSelector("south-slavic", "medieval", []));
		Assert.ThrowsException<InvalidOperationException>(() => catalogue.FixedGrantFactors("medieval", "unknown", "culture.english-nobility"));
	}
}
