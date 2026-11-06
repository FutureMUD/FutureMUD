#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
[DoNotParallelize]
public class TargetTraitExpressionTests
{
	private int _oldBonusScale;
	[TestInitialize]
	public void Initialise()
	{
		var field = typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!;
		_oldBonusScale = (int)field.GetValue(null)!;
		field.SetValue(null, 10);
	}

	[TestCleanup]
	public void Cleanup() => typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!
		.SetValue(null, _oldBonusScale);

	[TestMethod]
	public void Evaluate_MixedBindings_ReadsCorrectSubjectsAndKeepsVariableAndFlags()
	{
		var f = new Fixture();
		var expression = f.Expression("perception:51{noimprove} - defense:!52 + variable + extra");
		Assert.AreEqual(55d, expression.EvaluateWithTarget(f.Actor.Object, f.Target.Object, f.Self.Object, values: [("extra", 5)]));
		Assert.AreEqual(85d, expression.EvaluateWith(f.Actor.Object, f.Self.Object, values: [("extra", 5)]));
		Assert.IsFalse(expression.Parameters["perception"].IsTarget);
		Assert.IsFalse(expression.Parameters["perception"].CanImprove);
		Assert.IsTrue(expression.Parameters["perception"].CanBranch);
		Assert.IsTrue(expression.Parameters["defense"].IsTarget);
		Assert.IsFalse(expression.Parameters["defense"].CanImprove);
		Assert.IsFalse(expression.Parameters["defense"].CanBranch);
		Assert.AreEqual(52L, expression.TraitReferenceIds["defense"]);
		CollectionAssert.AreEquivalent(new[] { "variable", "extra" }, expression.NonTraitParameters.ToArray());
		f.Actor.Verify(x => x.TraitValue(f.Defence.Object, It.IsAny<TraitBonusContext>()), Times.Never);
		Assert.AreEqual(0d, f.Expression("defense:!52").EvaluateMax(f.Actor.Object));
		Assert.AreEqual(0d, f.Expression("defense:!52").CaptureNumericalBindings(f.Actor.Object, null!, TraitBonusContext.None)["defense"]);
	}

	[TestMethod]
	public void Evaluate_RawFlagBypassesEffectiveModifiersAndSurvivesBuilderReload()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.TraitRawValue(f.Self.Object)).Returns(11);
		f.Target.Setup(x => x.TraitRawValue(f.Defence.Object)).Returns(3);
		var expression = f.Expression("own:51{raw} - target:!52{raw}");
		Assert.AreEqual(8d, expression.EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.AreEqual(11d, expression.Evaluate(f.Actor.Object));
		Assert.AreEqual(11d, expression.CaptureNumericalBindings(f.Actor.Object, null!, TraitBonusContext.None)["own"]);
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("trait target 52")));
		var loaded = f.Expression(expression.OriginalFormulaText);
		Assert.IsTrue(loaded.Parameters["own"].UseRawValue); Assert.IsTrue(loaded.Parameters["target"].UseRawValue);
		Assert.AreEqual(8d, loaded.EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula own - target + 1")));
		loaded = f.Expression(expression.OriginalFormulaText);
		Assert.IsTrue(loaded.Parameters["own"].UseRawValue);
		Assert.AreEqual(9d, loaded.EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		f.Actor.Verify(x => x.TraitValue(f.Self.Object, It.IsAny<TraitBonusContext>()), Times.Never);
		f.Target.Verify(x => x.TraitValue(f.Defence.Object, It.IsAny<TraitBonusContext>()), Times.Never);
	}

	[TestMethod]
	public void Load_InlineTargetScopeSurvivesStoredRowsAndFormulaEdits()
	{
		var f = new Fixture();
		var row = new MudSharp.Models.TraitExpression { Id = 4, Name = "saved", Expression = "100 - defense:!52" };
		row.TraitExpressionParameters.Add(new MudSharp.Models.TraitExpressionParameters
		{
			Parameter = "defense", TraitDefinitionId = 52, CanImprove = true, CanBranch = true
		});
		var expression = new TraitExpression(row, f.World.Object);
		Assert.AreEqual(70d, expression.EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula 100 - defense:52")));
		Assert.AreEqual(-899d, expression.EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.IsFalse(expression.Parameters["defense"].IsTarget);
		Assert.IsFalse(expression.BuildingCommand(f.Actor.Object, new StringStack("formula defense:!52 + (")));
		Assert.IsFalse(expression.Parameters["defense"].IsTarget, "Rejected edits must not change scope.");
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula 100 - defense:!52")));
		Assert.IsFalse(expression.BuildingCommand(f.Actor.Object, new StringStack("improve defense")));
		Assert.IsFalse(expression.BuildingCommand(f.Actor.Object, new StringStack("branch defense")));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("trait defense 51")));
		var reloaded = f.Expression(expression.OriginalFormulaText);
		Assert.IsTrue(reloaded.Parameters["defense"].IsTarget);
		Assert.AreEqual(51L, reloaded.Parameters["defense"].Trait.Id);
		Assert.IsTrue(expression.ShowBuilder(f.Actor.Object).Contains("[target]"));
	}

	[TestMethod]
	public void Builder_TargetBindingsRemainDurableWhenAddedRemovedOrReused()
	{
		var f = new Fixture();
		var expression = f.Expression("0");
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("parameter defense 52")));
		Assert.IsFalse(expression.BuildingCommand(f.Actor.Object, new StringStack("trait defense !52")));
		Assert.IsFalse(expression.Parameters["defense"].IsTarget);
		Assert.IsFalse(expression.BuildingCommand(f.Actor.Object, new StringStack("parameter defense !52")));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula defense")));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("parameter defense !52")));
		Assert.AreEqual(30d, f.Expression(expression.OriginalFormulaText).EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula defense + 1")));
		Assert.AreEqual(31d, f.Expression(expression.OriginalFormulaText).EvaluateWithTarget(f.Actor.Object, f.Target.Object));
		Assert.IsTrue(expression.BuildingCommand(f.Actor.Object, new StringStack("formula 0")));
		Assert.IsFalse(expression.Parameters.ContainsKey("defense"));
	}

	[TestMethod]
	[DataRow("standard")]
	[DataRow("static")]
	[DataRow("bonus absent")]
	[DataRow("ogl")]
	public void CheckPreview_BackendsSupplyTargetAndDefaultIneligibleTargetsToZero(string backend)
	{
		var f = new Fixture();
		var check = f.Check(f.Expression("100 - defense:!52"), backend);
		Assert.AreEqual(backend == "ogl" ? -70d : 70d,
			check.TargetNumber(f.Actor.Object, Difficulty.Normal, null!, f.Target.Object));
		Assert.AreEqual(backend == "ogl" ? -100d : 100d,
			check.TargetNumber(f.Actor.Object, Difficulty.Normal, null!, new Mock<IPerceivable>().Object));
		Assert.AreEqual(backend == "ogl" ? -100d : 100d,
			check.TargetNumber(f.Actor.Object, Difficulty.Normal, null!));
	}

	[TestMethod]
	[DataRow("single")]
	[DataRow("multi")]
	[DataRow("all")]
	public void StandardCheck_AllEntryPointsUseTargetWithoutLearningOrRequiringItsTrait(string entry)
	{
		var f = new Fixture();
		var check = f.Check(f.Expression("100 - defense:!52"), "standard");
		var results = entry switch
		{
			"multi" => Multi(),
			"all" => check.CheckAgainstAllDifficulties(f.Actor.Object, Difficulty.Normal, null!, f.Target.Object).Values.ToArray(),
			_ => new[] { check.Check(f.Actor.Object, Difficulty.Normal, (ITraitDefinition)null!, f.Target.Object) }
		};
		foreach (var result in results)
		{
			Assert.AreEqual(70d, result.TargetNumber);
			Assert.IsFalse(result.IsAbjectFailure);
			Assert.AreEqual(0, result.ImprovedTraits.Count());
			Assert.AreEqual(0, result.AcquiredTraits.Count());
		}
		Assert.IsFalse(((StandardCheck)check).WouldBeAbjectFailure(f.Actor.Object, null!));
		f.Actor.Verify(x => x.GetTrait(f.Defence.Object), Times.Never);
		f.Actor.Verify(x => x.HasTrait(f.Defence.Object), Times.Never);
		f.Target.Verify(x => x.GetTrait(It.IsAny<ITraitDefinition>()), Times.Never);

		CheckOutcome[] Multi()
		{
			var result = check.MultiDifficultyCheck(f.Actor.Object, Difficulty.Normal, Difficulty.Hard, f.Target.Object);
			return [result.Item1, result.Item2];
		}
	}

	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly Mock<ICharacter> Actor = new();
		internal readonly Mock<ICharacter> Target = new();
		internal readonly Mock<ITraitDefinition> Self = new();
		internal readonly Mock<ITraitDefinition> Defence = new();
		private readonly All<ITraitDefinition> _traits = new();
		private readonly All<ITraitExpression> _expressions = new();

		internal Fixture()
		{
			Self.SetupGet(x => x.Id).Returns(51);
			Self.SetupGet(x => x.Name).Returns("Perception");
			Defence.SetupGet(x => x.Id).Returns(52);
			Defence.SetupGet(x => x.Name).Returns("Defence");
			_traits.Add(Self.Object); _traits.Add(Defence.Object);
			World.SetupGet(x => x.Traits).Returns(_traits);
			World.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
			World.SetupGet(x => x.TraitExpressions).Returns(_expressions);
			World.SetupGet(x => x.MagicResources).Returns(new All<MudSharp.Magic.IMagicResource>());
			Actor.SetupGet(x => x.Merits).Returns([]);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.VisionPercentage).Returns(1);
			Actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
			Actor.Setup(x => x.TraitValue(Self.Object, It.IsAny<TraitBonusContext>())).Returns(40);
			Actor.Setup(x => x.TraitValue(Defence.Object, It.IsAny<TraitBonusContext>())).Returns(999);
			Target.Setup(x => x.TraitValue(Defence.Object, It.IsAny<TraitBonusContext>())).Returns(30);
		}

		internal TraitExpression Expression(string text) => new(text, World.Object) { Id = 100 };

		internal ICheck Check(TraitExpression expression, string backend)
		{
			_expressions.Add(expression);
			var template = new MudSharp.Models.CheckTemplate
			{
				Name = "Target tests", ImproveTraits = true, CanBranchIfTraitMissing = true,
				FailIfTraitMissingMode = (int)FailIfTraitMissingType.FailIfAnyMissing
			};
			foreach (var difficulty in Enum.GetValues<Difficulty>())
				template.CheckTemplateDifficulties.Add(new MudSharp.Models.CheckTemplateDifficulty { Difficulty = (int)difficulty, Modifier = 0 });
			var model = new MudSharp.Models.Check
			{
				Type = (int)CheckType.MeleeWeaponCheck, TraitExpressionId = expression.Id,
				CheckTemplate = template, MaximumDifficultyForImprovement = (int)Difficulty.Impossible
			};
			return backend switch
			{
				"static" => new StaticCheck(model, World.Object),
				"bonus absent" => new BonusAbsentCheck(model, World.Object),
				"ogl" => new OGLCheck(model, World.Object),
				_ => new StandardCheck(model, World.Object)
			};
		}
	}
}
