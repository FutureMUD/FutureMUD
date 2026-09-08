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
public class CultureToolkitGroupSeederTests
{
	[DataTestMethod]
	[DataRow("antiquity")]
	[DataRow("darkages")]
	[DataRow("medieval")]
	[DataRow("renaissance")]
	[DataRow("earlymodern")]
	public void GroupConsumersUseInstalledCompiledIdentitiesAndPreserveBuilderOverrides(string era)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose(era);
		var cultures = pack.Cultures.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Culture { Name = CultureToolkitCatalogue.Text(x, "label") });
		var ethnicities = pack.Ethnicities.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Ethnicity { Name = CultureToolkitCatalogue.Text(x, "label") });
		var languages = pack.Languages.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new TraitDefinition { Name = CultureToolkitCatalogue.Text(x.GetProperty("labels"), era), Type = 0, OwnerScope = 1 });
		context.Cultures.AddRange(cultures.Values);
		context.Ethnicities.AddRange(ethnicities.Values);
		context.TraitDefinitions.AddRange(languages.Values);
		context.SaveChanges();
		var conflicts = new List<string>();
		var first = CultureToolkitGroupSeeder.Upsert(context, catalogue, pack, cultures, ethnicities, languages, conflicts);
		Assert.AreEqual(pack.Groups.Count, first.Count);
		Assert.AreEqual(0, conflicts.Count);
		foreach (var result in first)
		{
			Assert.IsTrue(result.GroupId > 0);
			Assert.IsTrue(result.EligibilityProgId > 0);
			Assert.AreEqual(result.CandidateTraitIds.Count, result.CandidateTraitIds.Distinct().Count());
		}
		var edited = context.ChargenSkillSelectionGroups.First();
		edited.Description = "Builder curriculum";
		edited.MaximumPicks = 0;
		var prog = context.FutureProgs.Find(edited.EligibilityProgId)!;
		prog.FunctionText = "return false";
		context.SaveChanges();
		var second = CultureToolkitGroupSeeder.Upsert(context, catalogue, pack, cultures, ethnicities, languages, conflicts);
		CollectionAssert.AreEqual(first.Select(x => x.GroupId).ToArray(), second.Select(x => x.GroupId).ToArray());
		Assert.AreEqual("Builder curriculum", edited.Description);
		Assert.AreEqual(0, edited.MaximumPicks);
		Assert.AreEqual("return false", prog.FunctionText);
		Assert.IsTrue(conflicts.Any(x => x.Contains("body", StringComparison.Ordinal)));
		Assert.AreEqual(first.Count, context.ChargenSkillSelectionGroups.Count());
	}
}
