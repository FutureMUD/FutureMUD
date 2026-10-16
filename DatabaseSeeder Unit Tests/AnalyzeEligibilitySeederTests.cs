#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AnalyzeEligibilitySeederTests
{
	[TestMethod]
	public void SeedChecks_AnalyseIsDisabledStaticWithoutTraitOrProgression()
	{
		using var context = new FuturemudDatabaseContext(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);
		var method = typeof(SkillSeeder).GetMethod("SeedChecks", BindingFlags.Instance | BindingFlags.NonPublic)!;
		method.Invoke(new SkillSeeder(), new object[] { context, new Dictionary<string, string> { ["branching"] = "yes" } });
		var check = context.Checks.Include(x => x.TraitExpression).Include(x => x.CheckTemplate)
			.ThenInclude(x => x.CheckTemplateDifficulties).Single(x => x.Type == (int)CheckType.AnalyzeEligibilityCheck);
		Assert.AreEqual("0", check.TraitExpression.Expression);
		Assert.AreEqual(0, check.TraitExpression.TraitExpressionParameters.Count);
		Assert.AreEqual("Static Check", check.CheckTemplate.Name);
		Assert.AreEqual("Static", check.CheckTemplate.CheckMethod);
		Assert.IsFalse(check.CheckTemplate.ImproveTraits);
		Assert.IsFalse(check.CheckTemplate.CanBranchIfTraitMissing);
		Assert.AreEqual(Enum.GetValues<Difficulty>().Length, check.CheckTemplate.CheckTemplateDifficulties.Count);
		Assert.IsTrue(check.CheckTemplate.CheckTemplateDifficulties.All(x => x.Modifier == 0));
		Assert.AreEqual(0, context.TraitDefinitions.Count());
		Assert.AreEqual((int)Difficulty.Normal, check.MaximumDifficultyForImprovement);
	}
}
