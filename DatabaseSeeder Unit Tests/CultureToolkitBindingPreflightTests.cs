#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitBindingPreflightTests
{
	[TestMethod]
	public void ExactSourceCrosswalkRejectsDuplicatesAndReportsInvalidReferencesWithoutEnablingUnrelatedRows()
	{
		var catalogue = new CultureToolkitCatalogue();
		var row = CultureToolkitNativeBindings.ExactSource(catalogue, "antiquity", "earthantiquity", "Achaean")!.Value;
		var duplicate = Assert.ThrowsException<InvalidOperationException>(() =>
			CultureToolkitNativeBindings.ExactSource([row, row], "antiquity", "earthantiquity", "Achaean"));
		StringAssert.Contains(duplicate.Message, "earthantiquity:Achaean:antiquity");
		var invalid = CultureToolkitNativeBindings.Resolve("source.earthantiquity.ethnicity.Achaean", null, "fixture", ["greeek"], new Dictionary<string, Language>());
		Assert.IsFalse(invalid.IsResolved);
		StringAssert.Contains(invalid.Unresolved.Single(), "source.earthantiquity.ethnicity.Achaean: unresolved supplied native language greeek");
		var unrelated = CultureToolkitNativeBindings.Source(catalogue, "antiquity", "earthantiquity", new Ethnicity { Name = "Unrelated fixture" }, new Dictionary<string, Language>());
		Assert.IsFalse(unrelated.IsResolved);
		Assert.AreEqual(0, unrelated.LanguageIds.Count);
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SourceAccentContextDistinguishesAStoredRenameFromUnrelatedSameLabelLanguage(bool unrelated)
	{
		using var source = Context();
		using var installed = Context();
		Language Greek(string description) => new()
		{
			Name = "Greek", LinkedTrait = new TraitDefinition { Name = "Greek", Expression = new TraitExpression { Name = "Greek Skill Cap", Expression = "100" } },
			Accents = [new Accent { Name = "Local", Group = "greek", Suffix = "with a local accent", Description = description }]
		};
		source.Add(Greek("Retained historical source pronunciation."));
		var existing = Greek(unrelated ? "A different source's pronunciation." : "Retained historical source pronunciation.");
		if (!unrelated) existing.Name = "Builder Greek label";
		installed.Add(existing);
		source.SaveChanges(); installed.SaveChanges();
		var issues = new List<string>();
		var result = CultureToolkitBindingPreflight.Resolve(installed, new CultureToolkitCatalogue(), new([], []),
			new Dictionary<string, FuturemudDatabaseContext> { ["earthrenaissanceeurope"] = source }, [], issues);
		Assert.AreEqual(unrelated ? 0 : 1, result.Languages.Count);
		Assert.AreEqual(unrelated ? 1 : 0, issues.Count);
		Assert.AreEqual(0, installed.SeederManagedRecords.Count());
		Assert.IsFalse(installed.ChangeTracker.HasChanges());
		if (!unrelated) Assert.AreEqual(existing.Id, result.Languages["greek.medieval"].Id);
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
}
