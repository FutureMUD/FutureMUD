#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.RPG.Checks;
using MudSharp.Work.Crafts;
using MudSharp.Work.Crafts.Products;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests.Crafts;

[TestClass]
[DoNotParallelize]
public class CraftAnalysisTests
{
	private int _previousBonusStage;
	private static readonly FieldInfo BonusStageField = typeof(StandardCheck)
		.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!;

	[TestInitialize]
	public void InitialiseCheckConfiguration()
	{
		_previousBonusStage = (int)BonusStageField.GetValue(null)!;
		BonusStageField.SetValue(null, 10);
	}

	[TestCleanup]
	public void RestoreCheckConfiguration() => BonusStageField.SetValue(null, _previousBonusStage);

	[TestMethod]
	public void Eligibility_ClassSubclassPairs_AreBinaryAcrossWorldsAndBonuses()
	{
		var first = Role(42, ChargenRoleType.Class);
		var second = Role(902, ChargenRoleType.Class);
		var subclass = Role(903, ChargenRoleType.Subclass);
		var world1 = World(first.Object);
		var world2 = World(second.Object, subclass.Object);
		var formula1 = new TraitExpression("100 * {allowed class=42}", world1.Object);
		var formula2 = new TraitExpression("100 * ({allowed class=902} * {sub role=903})", world2.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Roles).Returns(new[] { second.Object, subclass.Object });
		Assert.AreEqual(0.0, formula1.Evaluate(actor.Object));
		Assert.AreEqual(100.0, formula2.Evaluate(actor.Object));
		Assert.AreEqual(0, formula2.Parameters.Count);
		Assert.IsFalse(CheckType.AnalyzeEligibilityCheck.IsNonStaticCheck());
		var check = Static(world2, formula2);
		foreach (var bonus in new[] { -10000.0, 0.0, 10000.0 })
		{
			foreach (var difficulty in Enum.GetValues<Difficulty>())
			{
				Assert.AreEqual(Outcome.MajorPass, check.Check(actor.Object, difficulty,
					(ITraitDefinition?)null, externalBonus: bonus).Outcome);
			}
		}
		actor.SetupGet(x => x.Roles).Returns(new[] { second.Object });
		Assert.AreEqual(Outcome.MajorFail, check.Check(actor.Object, Difficulty.Normal,
			(ITraitDefinition?)null, externalBonus: 10000.0).Outcome);
		actor.SetupGet(x => x.Roles).Returns(Array.Empty<IChargenRole>());
		Assert.AreEqual(0.0, formula2.Evaluate(actor.Object));
		Assert.AreEqual(100.0, new TraitExpression("100", world2.Object).Evaluate(actor.Object));
		Assert.IsFalse(check.ImproveTraits);
		Assert.IsFalse(check.CanTraitBranchIfMissing);
		actor.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
		actor.Verify(x => x.SetTraitValue(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void Eligibility_LoadedExpression_UsesOwningWorldAndRejectsCrossPairs()
	{
		var class1 = Role(17, ChargenRoleType.Class);
		var class2 = Role(18, ChargenRoleType.Class);
		var sub1 = Role(27, ChargenRoleType.Subclass);
		var sub2 = Role(28, ChargenRoleType.Subclass);
		var world = World(class1.Object, class2.Object, sub1.Object, sub2.Object);
		var expression = new TraitExpression(new MudSharp.Models.TraitExpression
		{
			Expression = "100 * ({c1 class=17} * {s1 role=27} + {c2 class=18} * {s2 role=28})"
		}, world.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Roles).Returns(new[] { class1.Object, sub2.Object });
		Assert.AreEqual(0.0, expression.Evaluate(actor.Object));
		actor.SetupGet(x => x.Roles).Returns(new[] { class2.Object, sub2.Object });
		Assert.AreEqual(100.0, expression.Evaluate(actor.Object));
	}

	[TestMethod]
	public void FindRecipes_RestrictionsDoNotAbortEligibleAlternativesOrLeakRecipes()
	{
		var actor = new Mock<ICharacter>();
		var item = new Mock<IGameItem>();
		var skill = new Mock<ITraitDefinition>().Object;
		actor.Setup(x => x.HasTrait(skill)).Returns(true);
		actor.Setup(x => x.TraitRawValue(skill)).Returns(50);
		var restricted = Recipe("Secret tribe recipe", skill);
		restricted.SetupGet(x => x.CanUseProg).Returns(Prog(false));
		var hidden = Recipe("Hidden recipe", skill);
		hidden.SetupGet(x => x.AppearInCraftsListProg).Returns(Prog(false));
		var old = Recipe("Obsolete recipe", skill);
		old.SetupGet(x => x.Status).Returns(RevisionStatus.Obsolete);
		var failOnly = Recipe("Failure only", skill);
		failOnly.SetupGet(x => x.Products).Returns(Array.Empty<ICraftProduct>());
		var good = Recipe("Alternative recipe", skill);
		var recipes = CraftAnalysis.FindRecipes(actor.Object, item.Object,
			new[] { restricted.Object, hidden.Object, old.Object, failOnly.Object, good.Object });
		CollectionAssert.AreEqual(new[] { good.Object }, recipes.ToArray());
		actor.Setup(x => x.HasTrait(skill)).Returns(false);
		Assert.AreEqual(0, CraftAnalysis.FindRecipes(actor.Object, item.Object, new[] { good.Object }).Count);
		actor.Setup(x => x.HasTrait(skill)).Returns(true);
		actor.Setup(x => x.TraitRawValue(skill)).Returns(0);
		Assert.AreEqual(0, CraftAnalysis.FindRecipes(actor.Object, item.Object, new[] { good.Object }).Count);
		good.Verify(x => x.CanDoCraft(It.IsAny<ICharacter>(), It.IsAny<MudSharp.GameItems.Interfaces.IActiveCraftGameItemComponent>(),
			It.IsAny<bool>(), It.IsAny<bool>()), Times.Never);
		item.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void DescribeRecipes_ReportsRequirementsAndGenericNoMatchWithoutProduction()
	{
		var actor = new Mock<ICharacter>();
		var item = new Mock<IGameItem>();
		var recipe = Recipe("Known recipe", null);
		var input = new Mock<ICraftInput>();
		var tool = new Mock<ICraftTool>();
		input.Setup(x => x.HowSeen(actor.Object)).Returns("two lengths of cord");
		tool.Setup(x => x.HowSeen(actor.Object)).Returns("a lit forge workstation");
		recipe.SetupGet(x => x.Inputs).Returns(new[] { input.Object });
		recipe.SetupGet(x => x.Tools).Returns(new[] { tool.Object });
		var text = CraftAnalysis.DescribeRecipes(actor.Object, item.Object, new[] { recipe.Object });
		StringAssert.Contains(text, "two lengths of cord");
		StringAssert.Contains(text, "a lit forge workstation");
		StringAssert.Contains(text, "do not establish how this particular item was made");
		var none = CraftAnalysis.DescribeRecipes(actor.Object, item.Object, Array.Empty<ICraft>());
		StringAssert.Contains(none, "cannot determine a recipe");
		StringAssert.Contains(none, "crafting understanding");
	}

	[TestMethod]
	public void ProductMatch_RequiresStaticPrototypeSkinMaterialAndExplicitDynamicSupport()
	{
		var material = new Mock<ISolid>().Object;
		var proto = new Mock<IGameItemProto>();
		proto.SetupGet(x => x.Id).Returns(1234);
		proto.SetupGet(x => x.Material).Returns(material);
		var protos = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		protos.Setup(x => x.Get(1234)).Returns(proto.Object);
		var skins = new Mock<IUneditableRevisableAll<IGameItemSkin>>();
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.ItemProtos).Returns(protos.Object);
		world.SetupGet(x => x.ItemSkins).Returns(skins.Object);
		var model = new MudSharp.Models.CraftProduct
		{
			Definition = "<Definition><ProductProducedId>1234</ProductProducedId><Quantity>1</Quantity><Skin>0</Skin></Definition>"
		};
		var simple = (SimpleProduct)Activator.CreateInstance(typeof(SimpleProduct), BindingFlags.Instance | BindingFlags.NonPublic,
			null, new object[] { model, new Mock<ICraft>().Object, world.Object }, null)!;
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Prototype).Returns(proto.Object);
		item.SetupGet(x => x.Material).Returns(material);
		Assert.IsTrue(simple.MatchesForAnalysis(item.Object));
		var skin = new Mock<IGameItemSkin>();
		skin.SetupGet(x => x.Id).Returns(8);
		item.SetupGet(x => x.Skin).Returns(skin.Object);
		Assert.IsFalse(simple.MatchesForAnalysis(item.Object));
		item.SetupGet(x => x.Skin).Returns((IGameItemSkin?)null);
		item.SetupGet(x => x.Material).Returns(new Mock<ISolid>().Object);
		Assert.IsFalse(simple.MatchesForAnalysis(item.Object));
		item.SetupGet(x => x.Material).Returns(material);
		simple.MaterialDefiningInputIndex = 0;
		Assert.IsFalse(simple.MatchesForAnalysis(item.Object));
		var variable = (SimpleVariableProduct)Activator.CreateInstance(typeof(SimpleVariableProduct),
			BindingFlags.Instance | BindingFlags.NonPublic, null,
			new object[] { model, new Mock<ICraft>().Object, world.Object }, null)!;
		Assert.IsTrue(variable.IsItem(item.Object));
		Assert.IsFalse(variable.MatchesForAnalysis(item.Object));
		proto.Verify(x => x.CreateNew(It.IsAny<ICharacter>()), Times.Never);
	}

	private static Mock<IChargenRole> Role(long id, ChargenRoleType type)
	{
		var role = new Mock<IChargenRole>();
		role.SetupGet(x => x.Id).Returns(id);
		role.SetupGet(x => x.RoleType).Returns(type);
		return role;
	}

	private static Mock<IFuturemud> World(params IChargenRole[] roles)
	{
		var world = new Mock<IFuturemud>();
		var all = new All<IChargenRole>();
		foreach (var role in roles) all.Add(role);
		world.SetupGet(x => x.Roles).Returns(all);
		return world;
	}

	private static StaticCheck Static(Mock<IFuturemud> world, TraitExpression expression)
	{
		expression.Id = 1;
		var expressions = new All<ITraitExpression>();
		expressions.Add(expression);
		world.SetupGet(x => x.TraitExpressions).Returns(expressions);
		var template = new MudSharp.Models.CheckTemplate { Name = "Static Check", CheckMethod = "Static", FailIfTraitMissingMode = 1 };
		foreach (var difficulty in Enum.GetValues<Difficulty>())
			template.CheckTemplateDifficulties.Add(new MudSharp.Models.CheckTemplateDifficulty { Difficulty = (int)difficulty, Modifier = 0 });
		return new StaticCheck(new MudSharp.Models.Check
		{
			Type = (int)CheckType.AnalyzeEligibilityCheck, CheckTemplate = template, TraitExpressionId = expression.Id
		}, world.Object);
	}

	private static IFutureProg Prog(bool result)
	{
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(result);
		return prog.Object;
	}

	private static Mock<ICraft> Recipe(string name, ITraitDefinition? skill)
	{
		var recipe = new Mock<ICraft>();
		recipe.SetupGet(x => x.Name).Returns(name);
		recipe.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		recipe.SetupGet(x => x.CraftIsValid).Returns(true);
		recipe.SetupGet(x => x.CheckTrait).Returns(skill!);
		recipe.SetupGet(x => x.AppearInCraftsListProg).Returns(Prog(true));
		recipe.SetupGet(x => x.CanUseProg).Returns(Prog(true));
		var product = new Mock<ICraftProduct>();
		product.Setup(x => x.MatchesForAnalysis(It.IsAny<IGameItem>())).Returns(true);
		recipe.SetupGet(x => x.Products).Returns(new[] { product.Object });
		return recipe;
	}
}
