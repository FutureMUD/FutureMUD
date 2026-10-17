#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DirectionSenseSeederTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	private static Dictionary<string, long> Owned(long id = 42) => new() { [DirectionSenseSeederHelper.TraitKey] = id };
	private static void AddSkill(FuturemudDatabaseContext context) => context.TraitDefinitions.Add(new TraitDefinition
	{
		Id = 42, Name = "Renamable orientation", Type = 0, OwnerScope = 1,
		Expression = new TraitExpression { Name = "Builder cap", Expression = "73" }
	});

	[TestMethod]
	public void BindOwnedTrait_TwoChecksSameSkill_InitialImprovesRecoveryCannotImproveOrBranch()
	{
		using var context = Context();
		AddSkill(context);
		context.SaveChanges();
		DirectionSenseSeederHelper.EnsureUnboundChecks(context);
		Assert.IsTrue(DirectionSenseSeederHelper.BindOwnedTrait(context, Owned()).Success);

		var checks = context.Checks.OrderBy(x => x.Type).ToArray();
		Assert.AreEqual(2, checks.Length);
		Assert.AreEqual((int)CheckType.DirectionSenseCheck, checks[0].Type);
		Assert.AreEqual((int)CheckType.DirectionSenseRecoveryCheck, checks[1].Type);
		foreach (var check in checks)
		{
			var recovery = check.Type == (int)CheckType.DirectionSenseRecoveryCheck;
			Assert.AreEqual(!recovery, check.CheckTemplate.ImproveTraits);
			Assert.IsFalse(check.CheckTemplate.CanBranchIfTraitMissing);
			Assert.AreEqual((short)FailIfTraitMissingType.FailIfAnyMissing, check.CheckTemplate.FailIfTraitMissingMode);
			var parameter = check.TraitExpression.TraitExpressionParameters.Single();
			Assert.AreEqual(42L, parameter.TraitDefinitionId);
			Assert.AreEqual(!recovery, parameter.CanImprove);
			Assert.IsFalse(parameter.CanBranch);
			StringAssert.Contains(check.TraitExpression.Expression, "nobranch");
		}
		Assert.AreEqual(0, context.Traits.Count());
		Assert.AreEqual(0, context.StaticConfigurations.Count());
	}

	[TestMethod]
	public void BindOwnedTrait_RerunKeepsIdsCustomDifficultyCapAndPlayerValues()
	{
		using var context = Context();
		AddSkill(context);
		context.SaveChanges();
		Assert.IsTrue(DirectionSenseSeederHelper.BindOwnedTrait(context, Owned()).Success);
		var checks = context.Checks.OrderBy(x => x.Type).ToArray();
		var ids = checks.Select(x => (x.Type, x.TraitExpressionId, x.CheckTemplateId)).ToArray();
		checks[0].CheckTemplate.CheckTemplateDifficulties.First().Modifier = 87;
		context.TraitDefinitions.Single().Name = "Renamed";
		context.Traits.Add(new Trait { BodyId = 100, TraitDefinitionId = 42, Value = 31 });
		context.SaveChanges();
		var counts = (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count());

		DirectionSenseSeederHelper.EnsureUnboundChecks(context);
		var result = DirectionSenseSeederHelper.BindOwnedTrait(context, Owned());

		Assert.IsTrue(result.Success, result.Message);
		Assert.IsFalse(result.Changed);
		CollectionAssert.AreEqual(ids, context.Checks.OrderBy(x => x.Type)
			.Select(x => new { x.Type, x.TraitExpressionId, x.CheckTemplateId }).AsEnumerable()
			.Select(x => (x.Type, x.TraitExpressionId, x.CheckTemplateId)).ToArray());
		Assert.AreEqual(counts, (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count()));
		Assert.AreEqual(31.0, context.Traits.Single().Value);
		Assert.AreEqual("73", context.TraitDefinitions.Single().Expression.Expression);
		Assert.AreEqual(87.0, checks[0].CheckTemplate.CheckTemplateDifficulties.First().Modifier);
	}

	[TestMethod]
	public void BindOwnedTrait_MissingOrConflictingManifestRefusesBeforeChangingEitherCheck()
	{
		using var context = Context();
		Assert.IsFalse(DirectionSenseSeederHelper.BindOwnedTrait(context, new Dictionary<string, long>()).Success);
		Assert.AreEqual(0, context.Checks.Count());
		AddSkill(context);
		context.SaveChanges();
		DirectionSenseSeederHelper.EnsureUnboundChecks(context);
		context.Checks.Single(x => x.Type == (int)CheckType.DirectionSenseRecoveryCheck).TraitExpression.Expression = "55";
		context.SaveChanges();
		var counts = (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count());

		Assert.IsFalse(DirectionSenseSeederHelper.BindOwnedTrait(context, Owned()).Success);

		Assert.AreEqual("0", context.Checks.Single(x => x.Type == (int)CheckType.DirectionSenseCheck).TraitExpression.Expression);
		Assert.AreEqual("55", context.Checks.Single(x => x.Type == (int)CheckType.DirectionSenseRecoveryCheck).TraitExpression.Expression);
		Assert.AreEqual(counts, (context.Checks.Count(), context.TraitExpressions.Count(), context.CheckTemplates.Count()));
	}
	[TestMethod]
	public void BindOwnedTrait_SharedPlaceholderExpression_DoesNotChangeUnrelatedCheck()
	{
		using var context = Context();
		AddSkill(context);
		context.SaveChanges();
		DirectionSenseSeederHelper.EnsureUnboundChecks(context);
		var placeholder = context.Checks.Single(x => x.Type == (int)CheckType.DirectionSenseCheck);
		var sharedExpression = placeholder.TraitExpression;
		var sharedId = sharedExpression.Id;
		context.Checks.Add(new Check
		{
			Type = (int)CheckType.GenericSkillCheck,
			CheckTemplate = placeholder.CheckTemplate,
			TraitExpression = sharedExpression
		});
		context.SaveChanges();

		Assert.IsTrue(DirectionSenseSeederHelper.BindOwnedTrait(context, Owned()).Success);

		var unrelated = context.Checks.Single(x => x.Type == (int)CheckType.GenericSkillCheck);
		Assert.AreEqual(sharedId, unrelated.TraitExpressionId);
		Assert.AreEqual("0", unrelated.TraitExpression.Expression);
		Assert.AreEqual(0, unrelated.TraitExpression.TraitExpressionParameters.Count);
		Assert.AreNotEqual(sharedId, placeholder.TraitExpressionId);
	}

}
