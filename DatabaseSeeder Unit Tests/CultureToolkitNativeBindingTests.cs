#nullable enable

using System.Collections.Generic;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitNativeBindingTests
{
	[TestMethod]
	public void AncientVenetiAndRenaissanceVenetiansBindDifferentSourceLanguages()
	{
		var catalogue = new CultureToolkitCatalogue();
		var languages = new Dictionary<string, Language> { ["legacy:Venetic"] = new() { Id = 10 }, ["venetian"] = new() { Id = 20 } };
		var ancient = CultureToolkitNativeBindings.Source(catalogue, "antiquity", "earthantiquity", new Ethnicity { Name = "Venetian" }, languages);
		var later = CultureToolkitNativeBindings.Source(catalogue, "renaissance", "earthrenaissanceeurope", new Ethnicity { Name = "Venetian" }, languages);
		Assert.IsTrue(ancient.IsResolved);
		Assert.IsTrue(later.IsResolved);
		Assert.AreEqual(10L, ancient.LanguageIds[0]);
		Assert.AreEqual(20L, later.LanguageIds[0]);
		Assert.IsNull(ancient.OverlayKey);
		Assert.AreNotEqual(ancient.SourceIdentity, later.SourceIdentity);
	}

	[TestMethod]
	public void UnresolvedCossackDoesNotInheritLanguageFromTurkishNamingStructure()
	{
		var source = new Ethnicity { Name = "Cossack", EthnicGroup = "Turkic" };
		source.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures { NameCulture = new NameCulture { Name = "Turkish" } });
		var binding = CultureToolkitNativeBindings.Source(new CultureToolkitCatalogue(), "renaissance", "earthrenaissanceeurope", source,
			new Dictionary<string, Language> { ["turkish.ottoman"] = new() { Id = 10 } });
		Assert.IsFalse(binding.IsResolved);
		Assert.AreEqual(0, binding.References.Count);
		StringAssert.Contains(binding.Unresolved[0], "source.earthrenaissanceeurope.ethnicity.Cossack");
	}
}
