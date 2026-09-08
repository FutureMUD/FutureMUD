#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitIntelligibilityTests
{
	[TestMethod]
	public void FreshSourceEdgesAreCreatedAndMatrixOverridesBothVerifiedHistoricalDefaults()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var languages = new[] { "fixture.a", "fixture.b" }.ToDictionary(x => x, x => new Language { Name = x });
		context.Languages.AddRange(languages.Values);
		context.SaveChanges();
		var a = languages["fixture.a"].Id; var b = languages["fixture.b"].Id;
		var catalogue = new CultureToolkitCatalogue();
		using var edge = JsonDocument.Parse("""{"listener_language":"fixture.a","target_language":"fixture.b","difficulty_value":8}""");
		var pack = catalogue.Compose("medieval") with { DirectedEdges = [edge.RootElement.Clone()] };
		CultureSourceLanguageEdge[] source = [new("fixture.a", "fixture.b", 4, "module.one"), new("fixture.a", "fixture.b", 5, "module.two")];
		var conflicts = new List<string>();
		var result = CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, languages, source, conflicts, new HashSet<long> { a, b });
		Assert.AreEqual(8, context.MutualIntelligabilities.Find(a, b)!.IntelligabilityDifficulty);
		StringAssert.Contains(result.Single().SourceKey!, "module.one");
		StringAssert.Contains(result.Single().SourceKey!, "module.two");
		context.Remove(context.MutualIntelligabilities.Find(a, b)!);
		context.SaveChanges();
		CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, languages, source, conflicts);
		Assert.IsNull(context.MutualIntelligabilities.Find(a, b));
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder deletion")));
	}

	[TestMethod]
	public void OneWayEdgeKeepsListenerDirectionWithoutClosureAndReconcilesSourceFloorsAndOverrides()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var languages = new[] { "fixture.a", "fixture.b", "fixture.c", "fixture.d" }.ToDictionary(x => x, x => new Language { Name = x });
		context.Languages.AddRange(languages.Values);
		context.SaveChanges();
		var a = languages["fixture.a"].Id; var b = languages["fixture.b"].Id;
		var c = languages["fixture.c"].Id; var d = languages["fixture.d"].Id;
		context.MutualIntelligabilities.AddRange(
			new MutualIntelligability { ListenerLanguageId = b, TargetLanguageId = c, IntelligabilityDifficulty = 4 },
			new MutualIntelligability { ListenerLanguageId = c, TargetLanguageId = b, IntelligabilityDifficulty = 10 },
			new MutualIntelligability { ListenerLanguageId = c, TargetLanguageId = d, IntelligabilityDifficulty = 2 });
		context.SaveChanges();
		var catalogue = new CultureToolkitCatalogue();
		using var edge = JsonDocument.Parse("""{"listener_language":"fixture.a","target_language":"fixture.b","difficulty_value":8} """);
		var pack = catalogue.Compose("medieval") with { DirectedEdges = [edge.RootElement.Clone()] };
		CultureSourceLanguageEdge[] source =
		[
			new("fixture.b", "fixture.c", 4, "original.module:edge:1"),
			new("fixture.c", "fixture.b", 10, "original.module:edge:2"),
			new("fixture.c", "fixture.d", 5, "original.module:edge:3")
		];
		var conflicts = new List<string>();
		var report = CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, languages, source, conflicts);
		Assert.AreEqual(4, report.Count);
		Assert.AreEqual(8, context.MutualIntelligabilities.Find(a, b)!.IntelligabilityDifficulty);
		Assert.IsNull(context.MutualIntelligabilities.Find(b, a));
		Assert.IsNull(context.MutualIntelligabilities.Find(a, c));
		Assert.AreEqual(7, context.MutualIntelligabilities.Find(b, c)!.IntelligabilityDifficulty);
		Assert.AreEqual(10, context.MutualIntelligabilities.Find(c, b)!.IntelligabilityDifficulty);
		Assert.AreEqual(2, context.MutualIntelligabilities.Find(c, d)!.IntelligabilityDifficulty);
		Assert.IsTrue(report.Any(x => x.SourceKey == "original.module:edge:3" && x.Policy == "preserved-override"));
		context.MutualIntelligabilities.Find(a, b)!.IntelligabilityDifficulty = 9;
		context.SaveChanges();
		CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, languages, source, conflicts);
		Assert.AreEqual(9, context.MutualIntelligabilities.Find(a, b)!.IntelligabilityDifficulty);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
