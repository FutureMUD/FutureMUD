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
public class CultureToolkitSourceNameTests
{
	[TestMethod]
	public void IdenticalSourceAliasesShareRowsAndDivergentProfilesPreserveTheirOwnElementsOnRerun()
	{
		using var first = Context();
		using var second = Context();
		foreach (var context in new[] { first, second })
		{
			var culture = new NameCulture { Name = "Source structure", Definition = "<Definition />" };
			var profile = new RandomNameProfile { Name = "Retained local", Gender = 2, NameCulture = culture };
			profile.RandomNameProfilesElements.Add(new RandomNameProfilesElements { NameUsage = 0, Name = "Source Token", Weighting = 17 });
			profile.RandomNameProfilesDiceExpressions.Add(new RandomNameProfilesDiceExpressions { NameUsage = 0, DiceExpression = "1" });
			context.Add(profile);
			context.SaveChanges();
		}
		var stages = new Dictionary<string, FuturemudDatabaseContext> { ["first"] = first, ["second"] = second };
		var shared = CultureToolkitSourceNames.Describe(stages);
		Assert.AreEqual(1, shared.Cultures.Count);
		Assert.AreEqual(1, shared.Profiles.Count);
		Assert.AreEqual(2, shared.Profiles[0].Sources.Count);
		var oldElement = second.RandomNameProfilesElements.Single();
		second.Remove(oldElement);
		second.SaveChanges();
		second.Add(new RandomNameProfilesElements { RandomNameProfileId = oldElement.RandomNameProfileId, NameUsage = 0, Name = "Other Source Token", Weighting = 17 });
		second.SaveChanges();
		var plan = CultureToolkitSourceNames.Describe(stages);
		Assert.AreEqual(2, plan.Profiles.Count);
		using var installed = Context();
		var conflicts = new List<string>();
		var resolution = CultureToolkitSourceNames.Upsert(installed, "medieval", plan,
			new Dictionary<string, NameCulture>(), new Dictionary<string, RandomNameProfile>(), new Dictionary<string, FutureProg>(), true, conflicts);
		Assert.AreEqual(1, installed.NameCultures.Count());
		Assert.AreEqual(2, installed.RandomNameProfiles.Count());
		var profileToEdit = resolution.Profiles.Single(x => x.Key.Contains(".first.")).Value;
		installed.Remove(profileToEdit.RandomNameProfilesElements.Single());
		profileToEdit.RandomNameProfilesElements.Add(new RandomNameProfilesElements { NameUsage = 0, Name = "Builder addition", Weighting = 3 });
		profileToEdit.RandomNameProfilesDiceExpressions.Single().DiceExpression = "2";
		installed.SaveChanges();
		CultureToolkitSourceNames.Upsert(installed, "medieval", plan, new Dictionary<string, NameCulture>(),
			new Dictionary<string, RandomNameProfile>(), new Dictionary<string, FutureProg>(), true, conflicts);
		Assert.AreEqual("Builder addition", profileToEdit.RandomNameProfilesElements.Single().Name);
		Assert.AreEqual("2", profileToEdit.RandomNameProfilesDiceExpressions.Single().DiceExpression);
		Assert.AreEqual("Other Source Token", resolution.Profiles.Single(x => x.Key.Contains(".second.")).Value.RandomNameProfilesElements.Single().Name);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}

	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
}
