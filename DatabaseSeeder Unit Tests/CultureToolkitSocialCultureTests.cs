#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using FutureProg = MudSharp.Models.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitSocialCultureTests
{
	[TestMethod]
	public void All110SocialIdentitiesInstallAcrossEraFixturesAndRerunsKeepStartingWrappersAndBuilderEdits()
	{
		var catalogue = new CultureToolkitCatalogue();
		var seen = new HashSet<string>();
		foreach (var era in new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" })
		{
			using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
			var pack = catalogue.Compose(era);
			var languages = CultureToolkitNativeReviewTests.Available(era);
			context.AddRange(languages.Values.Distinct());
			var structures = pack.Cultures.Select(x => CultureToolkitCatalogue.Text(x, "naming_fallback")).Distinct()
				.ToDictionary(x => x, x => new NameCulture { Name = x, Definition = "<Definition />" });
			var calendar = new Calendar();
			var availability = new FutureProg { FunctionName = "Available" };
			var start = new FutureProg { FunctionName = "OriginalStart", ReturnType = (long)ProgVariableTypes.Number };
			var parameters = new[] { ProgVariableTypes.Toon, ProgVariableTypes.Trait, ProgVariableTypes.Number };
			for (var i = 0; i < parameters.Length; i++) start.FutureProgsParameters.Add(new FutureProgsParameter { ParameterIndex = i, ParameterType = (long)parameters[i] });
			context.AddRange(structures.Values);
			context.AddRange(calendar, availability, start);
			context.SaveChanges();
			var conflicts = new List<string>();
			var cultures = CultureToolkitSocialCultures.Upsert(context, pack, structures, calendar, start, availability, conflicts, catalogue, languages);
			foreach (var row in pack.Cultures)
			{
				var expected = CultureToolkitSocialCultures.NativeReferences(catalogue, row, era).Select(x => (long?)languages[x].Id).FirstOrDefault();
				Assert.AreEqual(expected, cultures[CultureToolkitCatalogue.Text(row, "key")].NativeLanguageId);
			}
			Assert.AreEqual(pack.Cultures.Count, cultures.Count);
			seen.UnionWith(cultures.Keys);
			var edited = cultures.Values.First();
			Assert.AreEqual(start.Id, edited.SkillStartingValueProgId);
			edited.SkillStartingValueProgId = 999;
			edited.Description = "Builder background";
			edited.NativeLanguageId = 999;
			var gender = edited.CulturesNameCultures.First().Gender;
			context.Remove(edited.CulturesNameCultures.First());
			context.SaveChanges();
			CultureToolkitSocialCultures.Upsert(context, pack, structures, calendar, start, availability, conflicts, catalogue, languages);
			Assert.AreEqual(pack.Cultures.Count, context.Cultures.Count());
			Assert.AreEqual(999, edited.SkillStartingValueProgId);
			Assert.AreEqual("Builder background", edited.Description);
			Assert.AreEqual(999L, edited.NativeLanguageId);
			Assert.IsFalse(edited.CulturesNameCultures.Any(x => x.Gender == gender));
			Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
		}
		Assert.AreEqual(110, seen.Count);
	}
}
