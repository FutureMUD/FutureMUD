#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitIntelligibilityRuntimeTests
{
	[TestMethod]
	public void RuntimeLanguagesLoadDirectedSeededRowsWithoutReverseOrTransitiveInference()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var definitions = new[] { "a", "b", "c" }.ToDictionary(x => x, x => new MudSharp.Models.Language { Name = x, LinkedTraitId = x[0], DifficultyModel = 1 });
		context.AddRange(definitions.Values);
		context.SaveChanges();
		using var edges = JsonDocument.Parse("""[{"listener_language":"a","target_language":"b","difficulty_value":8},{"listener_language":"b","target_language":"c","difficulty_value":7}]""");
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose("medieval") with { DirectedEdges = edges.RootElement.EnumerateArray().Select(x => x.Clone()).ToArray() };
		CultureToolkitIntelligibility.Reconcile(context, catalogue, pack, definitions, [], [], definitions.Values.Select(x => x.Id).ToHashSet());
		var traits = new All<ITraitDefinition>();
		foreach (var definition in definitions.Values)
		{
			var trait = new Mock<ITraitDefinition>();
			trait.SetupGet(x => x.Id).Returns(definition.LinkedTraitId);
			traits.Add(trait.Object);
		}
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.LanguageDifficultyModels).Returns(new All<ILanguageDifficultyModel>());
		var runtime = context.Languages.Include(x => x.MutualIntelligabilitiesListenerLanguage).AsEnumerable()
			.ToDictionary(x => x.Name, x => new Language(x, world.Object));
		Assert.AreEqual(Difficulty.ExtremelyHard, runtime["a"].MutualIntelligability(runtime["b"]));
		Assert.AreEqual(Difficulty.Impossible, runtime["b"].MutualIntelligability(runtime["a"]));
		Assert.AreEqual(Difficulty.VeryHard, runtime["b"].MutualIntelligability(runtime["c"]));
		Assert.AreEqual(Difficulty.Impossible, runtime["a"].MutualIntelligability(runtime["c"]));
		Assert.AreEqual(definitions["a"].LinkedTraitId, runtime["a"].LinkedTrait.Id);
	}
}
