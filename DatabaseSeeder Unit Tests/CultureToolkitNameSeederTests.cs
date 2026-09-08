#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitNameSeederTests
{
	[TestMethod]
	public void LocalProfilesPreserveBuilderMembersAndExistingFemaleExclusionAcrossThreePasses()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var catalogue = new CultureToolkitCatalogue();
		var ethnicities = catalogue.Compose("renaissance").Ethnicities.ToDictionary(x => CultureToolkitCatalogue.Text(x, "key"),
			x => new Ethnicity { Name = CultureToolkitCatalogue.Text(x, "label") });
		context.Ethnicities.AddRange(ethnicities.Values);
		var suggestions = new FutureProg { FunctionName = "AlwaysTrue" };
		context.FutureProgs.Add(suggestions);
		var oldFemale = new NameCulture { Name = "Retained Prussian Female", Definition = "<NameCulture />" };
		context.NameCultures.Add(oldFemale);
		context.SaveChanges();
		var prussian = ethnicities["ethnicity.old-prussian"];
		prussian.EthnicitiesNameCultures.Add(new EthnicitiesNameCultures
		{
			Ethnicity = prussian, Gender = (short)Gender.Female, NameCulture = oldFemale
		});
		context.SaveChanges();
		var conflicts = new List<string>();
		var first = CultureToolkitNameSeeder.Upsert(context, catalogue, "renaissance", ethnicities, suggestions, conflicts);
		Assert.AreEqual(16, first.Count);
		Assert.AreEqual(0, conflicts.Count);
		Assert.AreEqual(oldFemale.Id, prussian.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.Female).NameCultureId);
		var finnish = context.NameCultures.Single(x => x.Name == "Finnish Documentary Names");
		var male = finnish.RandomNameProfiles.Single(x => x.Gender == (int)Gender.Male);
		var edited = male.RandomNameProfilesElements.First();
		edited.Weighting = 7;
		male.RandomNameProfilesElements.Add(new RandomNameProfilesElements { NameUsage = 0, Name = "BuilderName", Weighting = 3 });
		male.Name = "Builder profile";
		finnish.Definition = "<NameCulture><BuilderOverride /></NameCulture>";
		context.SaveChanges();
		for (var pass = 0; pass < 2; pass++)
		{
			var next = CultureToolkitNameSeeder.Upsert(context, catalogue, "renaissance", ethnicities, suggestions, conflicts);
			CollectionAssert.AreEqual(first.Select(x => x.NameCultureId).ToArray(), next.Select(x => x.NameCultureId).ToArray());
			Assert.AreEqual(7, edited.Weighting);
			Assert.AreEqual("Builder profile", male.Name);
			Assert.IsTrue(male.RandomNameProfilesElements.Any(x => x.Name == "BuilderName" && x.Weighting == 3));
			Assert.AreEqual("<NameCulture><BuilderOverride /></NameCulture>", finnish.Definition);
		}
		Assert.IsTrue(conflicts.Any(x => x.Contains("definition")));
		Assert.IsTrue(conflicts.Any(x => x.Contains("element:")));
		Assert.AreEqual(17, context.NameCultures.Count());
		var finnishEthnicity = ethnicities["ethnicity.finnish"];
		Assert.AreEqual(finnish.Id, finnishEthnicity.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.Male).NameCultureId);
		Assert.AreNotEqual(finnish.Id, finnishEthnicity.EthnicitiesNameCultures.Single(x => x.Gender == (short)Gender.NonBinary).NameCultureId);
	}
}
