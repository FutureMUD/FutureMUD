#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial.Authored;
using MudSharp.Models;
using MudSharp.Database;

namespace DatabaseSeederUnitTests;

[TestClass]
public class AuthoredCelestialSeederTests
{
	[TestMethod]
	[DataRow("Rails", "RailSun RailMoon")]
	[DataRow("Scripts", "ScriptedSun ScriptedMoon")]
	[DataRow("MorningStar", "MorningStar")]
	[DataRow("MorningGlow", "MorningGlow")]
	[DataRow("Rails Rails", "RailSun RailMoon")]
	[DataRow("none", "")]
	public void Selection_InstallsOnlySelectedExamplesAndSkipsCompletedFollowups(string selection, string expected)
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		new TimeSeeder().SeedData(context, new Dictionary<string, string>
		{
			["secondsmultiplier"] = "2", ["mode"] = "gregorian-uk", ["startyear"] = "2000"
		});
		var answers = new Dictionary<string, string> { ["packages"] = selection };
		var seeder = new CelestialSeeder();
		seeder.SeedData(context, answers);
		CollectionAssert.AreEquivalent(expected.Split(' ', StringSplitOptions.RemoveEmptyEntries),
			context.Celestials.AsEnumerable().Select(x => AuthoredCelestialFormat.Parse(x.Definition).SeederPreset).ToArray());
		// A second calendar must not make completed selections ask for details again.
		context.Calendars.Add(new Calendar { Definition = "<calendar />", Date = "1-1-1", FeedClockId = context.Clocks.First().Id });
		context.SaveChanges();
		CollectionAssert.AreEqual(new[] { "packages" }, ((DatabaseSeeder.IDatabaseSeeder)seeder).Questions
			.Where(x => x.Filter(context, answers)).Select(x => x.Id).ToArray());
		seeder.SeedData(context, answers);
		Assert.AreEqual(expected.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length, context.Celestials.Count());
	}

	[TestMethod]
	public void AuthoredExamples_CompileAndRerunPreservesEditsAndRestoresOnlyMissingMember()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning)).Options);
		new TimeSeeder().SeedData(context, new Dictionary<string, string>
		{
			["secondsmultiplier"] = "2", ["mode"] = "gregorian-uk", ["startyear"] = "2000"
		});
		var answers = new Dictionary<string, string> { ["packages"] = "Rails Scripts MorningStar MorningGlow", ["authoredcalendar"] = context.Calendars.First().Id.ToString() };
		var seeder = new CelestialSeeder();
		seeder.SeedData(context, answers);
		var original = context.Celestials.OrderBy(x => x.Id).ToArray();
		Assert.AreEqual(6, original.Length);
		foreach (var row in original)
		{
			var definition = AuthoredCelestialFormat.Parse(row.Definition);
			Assert.AreEqual(definition.Kind.ToString(), row.CelestialType);
			_ = new CompiledAuthoredCelestial(definition, 60, 60, 24, new(SourceBytes: 65535));
		}

		var edited = AuthoredCelestialFormat.Parse(original[0].Definition);
		edited.Name = "Builder's private star";
		edited.LightProfile[0] = new(-90, 123);
		original[0].Definition = AuthoredCelestialFormat.Serialize(edited);
		context.SaveChanges();
		var preserved = original[0].Definition;
		seeder.SeedData(context, answers);
		CollectionAssert.AreEqual(original.Select(x => x.Id).ToArray(), context.Celestials.OrderBy(x => x.Id).Select(x => x.Id).ToArray());
		Assert.AreEqual(preserved, context.Celestials.Find(original[0].Id)!.Definition);
		context.Celestials.Remove(original[5]);
		context.SaveChanges();
		seeder.SeedData(context, answers);
		Assert.AreEqual(6, context.Celestials.Count());
		Assert.AreEqual(preserved, context.Celestials.Find(original[0].Id)!.Definition);
		Assert.AreEqual(6, context.Celestials.AsEnumerable().Select(x => AuthoredCelestialFormat.Parse(x.Definition).SeederPreset).Distinct().Count());
	}
}
