using System;
using System.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class GuardingCheckSeederTests
{
	private static FuturemudDatabaseContext Context() => new(new DbContextOptionsBuilder<FuturemudDatabaseContext>()
		.UseInMemoryDatabase(Guid.NewGuid().ToString(), x => x.EnableNullChecks(false)).Options);

	[TestMethod]
	public void EnsureChecks_FreshAndRepeated_DefaultOffAndUntrainedFloorWithoutTraitGrants()
	{
		using var context = Context();
		GuardingCheckSeederHelper.EnsureChecks(context); GuardingCheckSeederHelper.EnsureChecks(context);
		Assert.AreEqual(2, context.Checks.Count()); Assert.AreEqual(0, context.TraitDefinitions.Count());
		Assert.AreEqual(0, context.Traits.Count()); Assert.AreEqual(0, context.CharacterTraits.Count());
		Assert.AreEqual("false", context.StaticConfigurations.Single().Definition);
		foreach (var check in context.Checks.Include(x => x.CheckTemplate).ThenInclude(x => x.CheckTemplateDifficulties).Include(x => x.TraitExpression))
		{
			Assert.AreEqual("15", check.TraitExpression.Expression);
			Assert.IsTrue(check.CheckTemplate.ImproveTraits); Assert.IsFalse(check.CheckTemplate.CanBranchIfTraitMissing);
			Assert.AreEqual((short)FailIfTraitMissingType.DoNotFail, check.CheckTemplate.FailIfTraitMissingMode);
			Assert.AreEqual(0.0, check.CheckTemplate.CheckTemplateDifficulties.Single(x => x.Difficulty == (int)Difficulty.Normal).Modifier);
		}
	}

	[TestMethod]
	public void BindExistingSkill_RenamedSkillById_PreservesValuesAndDisabledSetting()
	{
		using var context = Context();
		context.TraitDefinitions.Add(new TraitDefinition { Id = 42, Name = "Renamed Guarding", Type = 0, OwnerScope = 1 });
		context.Traits.Add(new Trait { BodyId = 5, TraitDefinitionId = 42, Value = 27, AdditionalValue = 3 }); context.SaveChanges();
		context.CharacterTraits.Add(new CharacterTrait { CharacterId = 7, TraitDefinitionId = 42, Value = 61, AdditionalValue = 4 }); context.SaveChanges();
		Assert.IsTrue(GuardingCheckSeederHelper.BindExistingSkill(context, 42, out var message), message);
		var ids = context.Checks.ToDictionary(x => x.Type, x => x.TraitExpressionId);
		Assert.IsTrue(GuardingCheckSeederHelper.BindExistingSkill(context, 42, out message), message);
		foreach (var check in context.Checks.Include(x => x.TraitExpression).ThenInclude(x => x.TraitExpressionParameters))
		{
			Assert.AreEqual(ids[check.Type], check.TraitExpressionId);
			Assert.AreEqual("max(15, guarding:42{nobranch})", check.TraitExpression.Expression);
			var parameter = check.TraitExpression.TraitExpressionParameters.Single();
			Assert.AreEqual(42L, parameter.TraitDefinitionId); Assert.IsTrue(parameter.CanImprove); Assert.IsFalse(parameter.CanBranch);
		}
		Assert.AreEqual(1, context.TraitDefinitions.Count()); Assert.AreEqual(27.0, context.Traits.Single().Value);
		Assert.AreEqual(3.0, context.Traits.Single().AdditionalValue);
		Assert.AreEqual(61.0, context.CharacterTraits.Single().Value); Assert.AreEqual(4.0, context.CharacterTraits.Single().AdditionalValue);
		Assert.AreEqual("false", context.StaticConfigurations.Single().Definition);
	}

	[TestMethod]
	public void EnsureAndBind_CustomFormulaOrTemplate_PreservedWithoutPartialRebinding()
	{
		using var context = Context(); GuardingCheckSeederHelper.EnsureChecks(context);
		context.TraitDefinitions.Add(new TraitDefinition { Id = 42, Type = 0, OwnerScope = 1 });
		var custom = context.Checks.Include(x => x.TraitExpression).First(); custom.TraitExpression.Expression = "23";
		context.StaticConfigurations.Single().Definition = "true"; context.SaveChanges();
		GuardingCheckSeederHelper.EnsureChecks(context);
		Assert.IsFalse(GuardingCheckSeederHelper.BindExistingSkill(context, 42, out _));
		Assert.AreEqual("23", custom.TraitExpression.Expression);
		Assert.AreEqual("15", context.Checks.Include(x => x.TraitExpression).Single(x => x.Type != custom.Type).TraitExpression.Expression);
		Assert.AreEqual("true", context.StaticConfigurations.Single().Definition);
	}

	[TestMethod]
	public void BindExistingSkill_SharedPlaceholderExpression_IsReplacedWithoutChangingOtherChecks()
	{
		using var context = Context(); GuardingCheckSeederHelper.EnsureChecks(context);
		context.TraitDefinitions.Add(new TraitDefinition { Id = 42, Type = 0, OwnerScope = 1 });
		var original = context.Checks.Include(x => x.TraitExpression).First();
		var other = new Check { Type = (int)CheckType.GenericSkillCheck, CheckTemplate = original.CheckTemplate, TraitExpression = original.TraitExpression };
		context.Checks.Add(other); context.SaveChanges();
		Assert.IsTrue(GuardingCheckSeederHelper.BindExistingSkill(context, 42, out var message), message);
		Assert.AreEqual("15", other.TraitExpression.Expression);
		Assert.AreNotEqual(other.TraitExpressionId, original.TraitExpressionId);
	}
}
