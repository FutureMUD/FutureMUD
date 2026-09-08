#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitCapTests
{
	[TestMethod]
	public void SeededCapSurvivesActualSkillValueClampAndKeepsScalingAndCustomLowerCap()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var intelligence = new MudSharp.Models.TraitDefinition { Name = "Intelligence", Alias = "int", Type = 1 };
		var decorator = new MudSharp.Models.TraitDecorator();
		var improver = new MudSharp.Models.Improver();
		var difficulty = new MudSharp.Models.LanguageDifficultyModels();
		var alwaysTrue = new MudSharp.Models.FutureProg { FunctionName = "AlwaysTrue" };
		var alwaysFalse = new MudSharp.Models.FutureProg { FunctionName = "AlwaysFalse" };
		context.AddRange(intelligence, decorator, improver, difficulty, alwaysTrue, alwaysFalse);
		context.SaveChanges();
		var catalogue = new CultureToolkitCatalogue();
		var pack = catalogue.Compose("earlymodern");
		pack = pack with { Languages = pack.Languages.Where(x => CultureToolkitCatalogue.Text(x, "key") == "english.earlymodern").ToArray() };
		var prerequisites = new CultureLanguagePrerequisites(intelligence, decorator, improver, difficulty, alwaysTrue, alwaysFalse);
		var result = CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(),
			new Dictionary<string, MudSharp.Models.Language>(), prerequisites, []);
		var model = result.Traits["english.earlymodern"];
		var capModel = context.TraitExpressions.Find(model.ExpressionId)!;
		var world = new Mock<IFuturemud>();
		var traits = new All<ITraitDefinition>();
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.TraitDecorators).Returns(new All<ITraitValueDecorator>());
		world.SetupGet(x => x.ImprovementModels).Returns(new All<IImprovementModel>());
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		var runtimeIntelligence = new Mock<ITraitDefinition>();
		runtimeIntelligence.SetupGet(x => x.Id).Returns(intelligence.Id);
		traits.Add(runtimeIntelligence.Object);
		var owner = new Mock<IHaveTraits>();
		owner.Setup(x => x.TraitValue(runtimeIntelligence.Object, TraitBonusContext.None)).Returns(12);
		var definition = new SkillDefinition(model, world.Object) { Cap = new TraitExpression(capModel, world.Object) };
		Assert.AreEqual(200.0, new Skill(definition, 200, owner.Object).Value);
		Assert.AreEqual(200.0, new Skill(definition, 224, owner.Object).Value);
		owner.Setup(x => x.TraitValue(runtimeIntelligence.Object, TraitBonusContext.None)).Returns(30);
		Assert.AreEqual(224.0, new Skill(definition, 224, owner.Object).Value);
		Assert.AreEqual(250.0, new Skill(definition, 250, owner.Object).Value);
		capModel.Expression = "175";
		context.SaveChanges();
		var conflicts = new List<string>();
		CultureToolkitLanguageSeeder.Upsert(context, catalogue, pack, new Dictionary<string, FuturemudDatabaseContext>(),
			new Dictionary<string, MudSharp.Models.Language>(), prerequisites, conflicts);
		definition.Cap = new TraitExpression(capModel, world.Object);
		Assert.AreEqual(175.0, new Skill(definition, 200, owner.Object).Value);
		Assert.IsTrue(conflicts.Any(x => x.Contains("builder edit")));
	}
}
