using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class BlindFightingTests
{
	private int _originalBonusScale;

	[TestInitialize]
	public void CaptureBonusScale() => _originalBonusScale = (int)typeof(StandardCheck)
		.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

	[TestCleanup]
	public void RestoreBonusScale() => typeof(StandardCheck)
		.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, _originalBonusScale);

	[DataTestMethod]
	[DataRow("single")]
	[DataRow("all")]
	[DataRow("multi")]
	public void Check_Disabled_PreservesNativeResolutionAndSkipsBlindFighting(string entry)
	{
		var f = new Fixture();
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns("false");
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.World.Setup(x => x.GetStaticDouble("BlindnessCheckBonus")).Returns(-10);
		var merit = new Mock<IBlindFightingMerit>();
		merit.SetupGet(x => x.DifficultyStages).Returns(2);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);

		Assert.AreSame(f.Ordinary, CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck));
		List<CheckOutcome> Resolve(ICheck check)
		{
			using var random = Constants.PushRandom(new Random(17));
			return entry switch
			{
				"all" => check.CheckAgainstAllDifficulties(f.Actor.Object, Difficulty.Normal,
					f.Weapon.Object, f.Opponent.Object).Values.ToList(),
				"multi" => ResolveMulti(check),
				_ => [check.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object)]
			};
		}
		List<CheckOutcome> ResolveMulti(ICheck check)
		{
			var result = check.MultiDifficultyCheck(f.Actor.Object, Difficulty.Normal, Difficulty.Hard,
				f.Opponent.Object, f.Weapon.Object);
			return [result.Item1, result.Item2];
		}

		var expected = Resolve(f.Ordinary);
		// A wrapper obtained before disabling must also bypass every feature adjustment.
		var actual = Resolve(f.Combat);

		CollectionAssert.AreEqual(expected.Select(x => x.TargetNumber).ToArray(), actual.Select(x => x.TargetNumber).ToArray());
		CollectionAssert.AreEqual(expected.Select(x => x.FinalDifficulty).ToArray(), actual.Select(x => x.FinalDifficulty).ToArray());
		CollectionAssert.AreEqual(expected.Select(x => x.Outcome).ToArray(), actual.Select(x => x.Outcome).ToArray());
		if (entry == "single") Assert.IsTrue(actual[0].ActiveBonuses.Any(x => x.Item1 == "vision"));
		Assert.AreEqual(0, f.Blind.CheckCount);
		Assert.AreEqual(0, f.Blind.RollCount);
		f.World.Verify(x => x.GetCheck(CheckType.BlindFightingCheck), Times.Never);
		f.Actor.Verify(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>()), Times.Never);
		f.Actor.Verify(x => x.IlluminationSightDifficulty(It.IsAny<IRoom>()), Times.Never);
		merit.Verify(x => x.Applies(It.IsAny<IHaveMerits>(), It.IsAny<IPerceivable>()), Times.Never);
		f.BlindSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(null)]
	[DataRow("false")]
	[DataRow("invalid")]
	public void TargetNumber_MissingInvalidOrDisabledEnableSetting_BypassesConstraints(string? setting)
	{
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations["BlindFightingEnabled"]);
		var f = new Fixture();
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns(setting!);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);

		Assert.AreSame(f.Ordinary, CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck));
		Assert.AreEqual(70.0, f.Combat.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object));
		Assert.AreEqual(100.0, ((CombatCheck)f.Combat).ImpairedTargetNumberOrDefault(
			f.Actor.Object, Difficulty.Normal, f.Opponent.Object, 100.0));
		f.World.Verify(x => x.GetCheck(CheckType.BlindFightingCheck), Times.Never);
		Assert.AreEqual(0, f.Ordinary.RollCount);
		Assert.AreEqual(0, f.Blind.CheckCount);
	}

	[TestMethod]
	public void Check_EnableSettingChangedAtRuntime_ResumesConstraintsAndLearning()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns("false");
		var disabled = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);
		Assert.AreEqual(70.0, disabled.TargetNumber);
		Assert.AreEqual(0, f.Blind.CheckCount);

		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns("true");
		var enabled = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);
		Assert.AreEqual(10.0, enabled.TargetNumber);
		Assert.AreEqual(Difficulty.ExtremelyHard, enabled.FinalDifficulty);
		Assert.AreEqual(1, f.Blind.RollCount);
		f.BlindSkill.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), Difficulty.ExtremelyHard,
			TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
	}

	[TestMethod]
	public void Check_UnseenOpponent_CapsBaseAndAppliesHarderDifficultyModifierOnce()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.CanSee(f.Opponent.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.World.Setup(x => x.GetStaticDouble("BlindnessCheckBonus")).Returns(-100);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(10.0, result.TargetNumber, "min(70,40) plus the ExtremelyHard modifier (-30), once");
		Assert.AreEqual(Difficulty.ExtremelyHard, result.FinalDifficulty);
		Assert.AreEqual(Difficulty.Normal, result.OriginalDifficulty);
		Assert.AreEqual(1, f.Ordinary.RollCount);
		Assert.IsFalse(result.ActiveBonuses.Any(x => x.Item1 == "vision"), "The replaced legacy vision adjustment must not also apply.");
		f.VerifyLearning(Difficulty.Normal, Difficulty.ExtremelyHard);
		Assert.IsNull(f.Blind.LastVariable, "The auxiliary check must not receive the weapon trait.");
		Assert.AreEqual(1, f.Blind.RollCount);
	}

	[TestMethod]
	public void Check_NormalDifficultyHarderThanBlindFighting_PreservesNormalBurdenAndBonus()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Room.Object)).Returns(Difficulty.Hard);
		f.Actor.SetupGet(x => x.CombatBurdenOffense).Returns(2);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.VeryHard, f.Weapon.Object, f.Opponent.Object,
			externalBonus: 10);

		Assert.AreEqual(Difficulty.ExtremelyHard, result.FinalDifficulty, "VeryHard - one bonus stage + two burden stages.");
		Assert.AreEqual(2, result.Burden);
		Assert.AreEqual(10.0, result.TargetNumber);
		f.VerifyLearning(Difficulty.Hard, Difficulty.Easy);
	}

	[TestMethod]
	public void CheckAgainstAllDifficulties_BlindFighting_UsesSharedRollsAndOneLearningOpportunity()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);

		var results = f.Combat.CheckAgainstAllDifficulties(f.Actor.Object, Difficulty.Normal,
			f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(Enum.GetValues<Difficulty>().Length, results.Count);
		foreach (var result in results.Values)
		{
			Assert.AreSame(results[Difficulty.Normal].Rolls, result.Rolls);
			Assert.IsTrue(result.FinalDifficulty >= Difficulty.ExtremelyHard);
		}
		Assert.AreEqual(10.0, results[Difficulty.Normal].TargetNumber);
		Assert.AreEqual(-10.0, results[Difficulty.Impossible].TargetNumber);
		f.VerifyLearning(Difficulty.Normal, Difficulty.ExtremelyHard);
		Assert.AreEqual(1, f.Blind.RollCount);
	}

	[TestMethod]
	public void MultiDifficultyCheck_BlindFighting_SharesRollsAndRetainsOrdinaryLearningSemantics()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);

		var results = f.Combat.MultiDifficultyCheck(f.Actor.Object, Difficulty.Normal, Difficulty.Impossible,
			f.Opponent.Object, f.Weapon.Object);

		CollectionAssert.AreEqual(results.Item1.Rolls.ToArray(), results.Item2.Rolls.ToArray());
		Assert.AreEqual(10.0, results.Item1.TargetNumber);
		Assert.AreEqual(-10.0, results.Item2.TargetNumber);
		Assert.AreEqual(1, f.Blind.RollCount);
		f.BlindSkill.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), Difficulty.ExtremelyHard,
			TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
		f.WeaponSkill.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Exactly(2));
	}

	[DataTestMethod]
	[DataRow(70.0, 40.0)]
	[DataRow(120.0, 70.0)]
	public void Check_ConstantHundred_SkipsAuxiliaryRollButRetainsDifficultyAndHundredCap(double ordinary, double expected)
	{
		var f = new Fixture(constantBlind: true);
		f.Actor.Setup(x => x.TraitValue(f.Weapon.Object, It.IsAny<TraitBonusContext>())).Returns(ordinary);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(expected, result.TargetNumber);
		Assert.AreEqual(Difficulty.ExtremelyHard, result.FinalDifficulty);
		Assert.AreEqual(0, f.Blind.RollCount, "A constant expression has no skill to improve or branch.");
		Assert.AreEqual(0, f.Blind.CheckCount);
		f.BlindSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
		f.Actor.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
	}

	[TestMethod]
	public void Check_ConstantHundredWithStaleSkillBinding_DoesNotRollLearnOrBranchBlindFighting()
	{
		var f = new Fixture(constantBlind: true);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		// The normal expression editor can retain binding rows after replacing a skill term with 100.
		f.Blind.TargetNumberExpression.Parameters.Add("oldblindskill", new TraitExpressionParameter
		{
			Trait = f.BlindDefinition.Object, CanImprove = true, CanBranch = true
		});

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(40.0, result.TargetNumber);
		Assert.AreEqual(0, f.Blind.CheckCount);
		Assert.AreEqual(0, f.Blind.RollCount);
		f.BlindSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
		f.Actor.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
		f.World.Verify(x => x.GetCheck(CheckType.TraitBranchCheck), Times.Never);
	}

	[TestMethod]
	public void Check_SkillBasedBlindFormulaWithStaleWeaponBinding_TrainsOnlyActiveFormulaTraits()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Blind.TargetNumberExpression.Parameters.Add("oldweapon", new TraitExpressionParameter
		{
			Trait = f.Weapon.Object, CanImprove = true, CanBranch = true
		});

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(10.0, result.TargetNumber);
		Assert.AreEqual(1, f.Blind.CheckCount);
		f.VerifyLearning(Difficulty.Normal, Difficulty.ExtremelyHard);
		f.WeaponSkill.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
	}

	[TestMethod]
	public void Check_UntrainedConfiguredSkill_UsesZeroWithoutPassingWeaponAsFallback()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Actor.Setup(x => x.TraitValue(f.BlindDefinition.Object, It.IsAny<TraitBonusContext>())).Returns(0);
		f.Actor.Setup(x => x.GetTrait(f.BlindDefinition.Object)).Returns((ITrait)null!);
		f.Actor.Setup(x => x.HasTrait(f.BlindDefinition.Object)).Returns(false);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(-30.0, result.TargetNumber);
		Assert.IsNull(f.Blind.LastVariable);
		Assert.AreEqual(1, f.Blind.CheckCount);
	}

	[TestMethod]
	public void Check_AdequateEffectiveSight_PreservesOrdinaryCheckAndSkipsBlindLearning()
	{
		var f = new Fixture();
		// This is the effective sight API after racial vision, darksight merits and effects.
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Room.Object)).Returns(Difficulty.Easy);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Hard, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(60.0, result.TargetNumber);
		Assert.AreEqual(Difficulty.Hard, result.FinalDifficulty);
		Assert.AreEqual(0, f.Blind.CheckCount);
		f.Actor.Verify(x => x.IlluminationSightDifficulty(f.Room.Object), Times.Once);
		f.BlindSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow(Difficulty.Normal, null)]
	[DataRow(Difficulty.Hard, Difficulty.Easy)]
	[DataRow(Difficulty.VeryHard, Difficulty.Normal)]
	[DataRow(Difficulty.ExtremelyHard, Difficulty.Hard)]
	[DataRow(Difficulty.Insane, Difficulty.VeryHard)]
	[DataRow(Difficulty.Impossible, Difficulty.ExtremelyHard)]
	public void SightDifficulty_EffectiveLowLight_UsesApprovedScale(Difficulty effectiveSight, Difficulty? expected)
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Room.Object)).Returns(effectiveSight);

		Assert.AreEqual(expected, CombatCheck.SightDifficulty(f.Actor.Object, f.Opponent.Object));
	}

	[DataTestMethod]
	[DataRow(false, 1.0)]
	[DataRow(true, 0.0)]
	public void SightDifficulty_UnseenOrBlind_UsesConfigurableUnseenDifficulty(bool canSee, double vision)
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.CanSee(f.Opponent.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(canSee);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(vision);
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingUnseenDifficulty")).Returns("9");

		Assert.AreEqual(Difficulty.Insane, CombatCheck.SightDifficulty(f.Actor.Object, f.Opponent.Object));
	}

	[TestMethod]
	public void SightDifficulty_ConfigurableThresholdAndMapping_UsesValidSettingsAndRejectsInvalidSettings()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Room.Object)).Returns(Difficulty.Hard);
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingMinimumSightDifficulty")).Returns("7");
		Assert.IsNull(CombatCheck.SightDifficulty(f.Actor.Object, f.Opponent.Object));
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingMinimumSightDifficulty")).Returns("-3");
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingSightHardDifficulty")).Returns("6");
		Assert.AreEqual(Difficulty.Hard, CombatCheck.SightDifficulty(f.Actor.Object, f.Opponent.Object));
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingSightHardDifficulty")).Returns("not-a-difficulty");
		Assert.AreEqual(Difficulty.Easy, CombatCheck.SightDifficulty(f.Actor.Object, f.Opponent.Object));
	}

	[TestMethod]
	public void MeritDifficulty_ApplicableDistinctSignedMerits_AddsThenClamps()
	{
		var f = new Fixture();
		var easier = Merit(f, 1, -3);
		var harder = Merit(f, 2, 1);
		var inapplicable = Merit(f, 3, 10, false);
		f.Actor.SetupGet(x => x.Merits).Returns([easier, easier, harder, inapplicable]);
		Assert.AreEqual(Difficulty.Hard, CombatCheck.MeritDifficulty(f.Actor.Object, f.Opponent.Object, Difficulty.ExtremelyHard));
		f.Actor.SetupGet(x => x.Merits).Returns([Merit(f, 4, int.MaxValue), Merit(f, 5, int.MaxValue)]);
		Assert.AreEqual(Difficulty.Impossible, CombatCheck.MeritDifficulty(f.Actor.Object, f.Opponent.Object, Difficulty.Normal));
		f.Actor.SetupGet(x => x.Merits).Returns([Merit(f, 6, int.MinValue), Merit(f, 7, int.MinValue)]);
		Assert.AreEqual(Difficulty.Automatic, CombatCheck.MeritDifficulty(f.Actor.Object, f.Opponent.Object, Difficulty.Normal));
	}

	[DataTestMethod]
	[DataRow(-10, Difficulty.Hard, 30.0)]
	[DataRow(2, Difficulty.Impossible, -10.0)]
	public void Check_MeritStages_AppliesBeforeComparisonWithOrdinaryDifficulty(int stages, Difficulty expectedDifficulty, double expectedTarget)
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Actor.SetupGet(x => x.Merits).Returns([Merit(f, 1, stages)]);

		var result = f.Combat.Check(f.Actor.Object, Difficulty.Hard, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(expectedDifficulty, result.FinalDifficulty);
		Assert.AreEqual(expectedTarget, result.TargetNumber);
	}

	[DataTestMethod]
	[DataRow(-3)]
	[DataRow(2)]
	public void BlindFightingMerit_SignedXmlConfiguration_LoadsStages(int stages)
	{
		var f = new Fixture();
		var merit = new BlindFightingMerit(new MudSharp.Models.Merit
		{
			Id = 42, Name = "sight training", Type = "Blind Fighting",
			Definition = $"<Definition><DifficultyStages>{stages}</DifficultyStages></Definition>"
		}, f.World.Object);

		Assert.AreEqual(stages, merit.DifficultyStages);
	}

	[TestMethod]
	public void TargetNumber_Preview_DoesNotSpotRollOrAdvanceEitherSkill()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);

		var tn = f.Combat.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(10.0, tn);
		Assert.AreEqual(0, f.Ordinary.RollCount);
		Assert.AreEqual(0, f.Blind.CheckCount);
		f.Actor.Verify(x => x.CanSee(f.Opponent.Object, PerceiveIgnoreFlags.IgnoreSpotting), Times.Once);
		f.Actor.Verify(x => x.CanSee(f.Opponent.Object, PerceiveIgnoreFlags.None), Times.Never);
		f.WeaponSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
		f.BlindSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
	}

	[TestMethod]
	public void TargetNumber_NonColocatedOrNonCharacterOpponent_PreservesOrdinaryResult()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Actor.Setup(x => x.ColocatedWith(f.Opponent.Object)).Returns(false);

		Assert.AreEqual(70.0, f.Combat.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object));
		Assert.AreEqual(70.0, f.Combat.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Room.Object));
		f.Actor.Verify(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>()), Times.Never);
		f.World.Verify(x => x.GetCheck(CheckType.BlindFightingCheck), Times.Never);
	}

	[TestMethod]
	public void TargetNumber_NonCharacterActor_PreservesOrdinaryResult()
	{
		var f = new Fixture();
		var actor = new Mock<IPerceivableHaveTraits>();
		actor.SetupGet(x => x.Merits).Returns([]);
		actor.Setup(x => x.EffectsOfType<ICheckBonusEffect>(It.IsAny<Predicate<ICheckBonusEffect>>())).Returns([]);
		actor.Setup(x => x.TraitValue(f.Weapon.Object, It.IsAny<TraitBonusContext>())).Returns(70);

		Assert.AreEqual(70.0, f.Combat.TargetNumber(actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object));
		f.World.Verify(x => x.GetCheck(CheckType.BlindFightingCheck), Times.Never);
	}

	[DataTestMethod]
	[DataRow("standard", 10.0)]
	[DataRow("static", 10.0)]
	[DataRow("bonus absent", 10.0)]
	[DataRow("ogl", -70.0)]
	public void TargetNumber_ConfiguredCheckBackend_ConstrainsFormulaAndDifficulty(string backend, double expectedTarget)
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		var model = f.Model(f.Ordinary.TargetNumberExpression, CheckType.MeleeWeaponCheck);
		ICheck check = backend switch
		{
			"static" => new StaticCheck(model, f.World.Object),
			"bonus absent" => new BonusAbsentCheck(model, f.World.Object),
			"ogl" => new OGLCheck(model, f.World.Object),
			_ => new StandardCheck(model, f.World.Object)
		};
		f.World.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(check);

		var targetNumber = CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck)
			.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(expectedTarget, targetNumber);
		Assert.AreEqual(0, f.Blind.CheckCount);
	}

	[TestMethod]
	public void Check_LimbRequirementRejected_DoesNotAdvanceOrRollBlindFighting()
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Limbs).Returns([]);
		f.Actor.SetupGet(x => x.Body).Returns(body.Object);
		var model = f.Model(f.Ordinary.TargetNumberExpression, CheckType.MeleeWeaponCheck);
		model.CheckTemplate.Definition = "<Definition><Limbs><Limb>0</Limb><Minimum>1</Minimum></Limbs></Definition>";
		f.World.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(new LimbCheck(model, f.World.Object));

		var result = CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck)
			.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		Assert.AreEqual(Outcome.MajorFail, result.Outcome);
		Assert.AreEqual(0, f.Blind.CheckCount);
		f.WeaponSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);
	}

	[DataTestMethod]
	[DataRow("preview", true, Difficulty.ExtremelyHard)]
	[DataRow("single", true, Difficulty.ExtremelyHard)]
	[DataRow("multi", true, Difficulty.ExtremelyHard)]
	[DataRow("preview", false, Difficulty.Hard)]
	[DataRow("single", false, Difficulty.Hard)]
	[DataRow("multi", false, Difficulty.Hard)]
	public void OGLCheck_SharedBonuses_OnlyStagesBlindExclusiveBonuses(string entry, bool shared, Difficulty expectedFloor)
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Actor.Setup(x => x.GetCurrentBonusLevel()).Returns(10);
		var effect = new Mock<ICheckBonusEffect>();
		effect.SetupGet(x => x.CheckBonus).Returns(10);
		effect.Setup(x => x.AppliesToCheck(It.IsAny<CheckType>()))
			.Returns((CheckType type) => type == CheckType.BlindFightingCheck || shared && type == CheckType.MeleeWeaponCheck);
		f.Actor.Setup(x => x.EffectsOfType<ICheckBonusEffect>(It.IsAny<Predicate<ICheckBonusEffect>>())).Returns([effect.Object]);
		var merit = new Mock<ICheckBonusMerit>();
		merit.Setup(x => x.Applies(f.Actor.Object, f.Opponent.Object)).Returns(true);
		merit.Setup(x => x.CheckBonus(f.Actor.Object, f.Opponent.Object, It.IsAny<CheckType>()))
			.Returns((ICharacter _, IPerceivable _, CheckType type) =>
				type == CheckType.BlindFightingCheck || shared && type == CheckType.MeleeWeaponCheck ? 10 : 0);
		f.Actor.SetupGet(x => x.Merits).Returns([merit.Object]);
		var model = f.Model(f.Ordinary.TargetNumberExpression, CheckType.MeleeWeaponCheck);
		// OGL templates use increasing DCs, whereas the percentile fixture uses decreasing bonuses.
		foreach (var difficulty in model.CheckTemplate.CheckTemplateDifficulties) difficulty.Modifier = difficulty.Difficulty * 10;
		var ogl = new ProbeOGLCheck(model, f.World.Object);
		f.World.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(ogl);
		var combat = CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck);

		if (entry == "preview")
			Assert.AreEqual(10.0, combat.TargetNumber(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object));
		else if (entry == "single")
		{
			combat.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);
			Assert.AreEqual(10.0, ogl.LastRollThreshold);
		}
		else
			combat.MultiDifficultyCheck(f.Actor.Object, Difficulty.Normal, Difficulty.Hard, f.Opponent.Object, f.Weapon.Object);

		Assert.AreEqual(expectedFloor, ogl.LastMinimumDifficulty,
			"Innate and shared effect/merit bonuses already lower the ordinary OGL DC. Only blind-exclusive bonuses may also stage its floor.");
	}

	[DataTestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void TimeboundCheck_BlindFightingContext_BypassesExistingCacheAndDoesNotPopulateCache(bool useTool)
	{
		var f = new Fixture();
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		var cached = new Mock<ICheckResultEffect>();
		cached.SetupGet(x => x.Outcome).Returns(Outcome.MajorPass);
		cached.Setup(x => x.SameCheck(It.IsAny<CheckType>(), It.IsAny<Difficulty>(), It.IsAny<IFrameworkItem>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IFrameworkItem>())).Returns(true);
		f.Actor.Setup(x => x.EffectsOfType<ICheckResultEffect>(It.IsAny<Predicate<ICheckResultEffect>>())).Returns([cached.Object]);
		var model = f.Model(f.Ordinary.TargetNumberExpression, CheckType.MeleeWeaponCheck);
		model.CheckTemplate.Definition = "<Definition minimum_time='60' maximum_time='60'/>";
		var timebound = new CountingTimeboundCheck(model, f.World.Object);
		f.World.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(timebound);
		var tool = new Mock<IUseTrait>();
		tool.SetupGet(x => x.Trait).Returns(f.Weapon.Object);
		CheckOutcome Resolve(ICheck check) => useTool
			? check.Check(f.Actor.Object, Difficulty.Normal, f.Opponent.Object, tool.Object)
			: check.Check(f.Actor.Object, Difficulty.Normal, f.Weapon.Object, f.Opponent.Object);

		// The ordinary cache remains effective and does not award a second learning opportunity.
		Assert.AreEqual(Outcome.MajorPass, Resolve(timebound).Outcome);
		Assert.AreEqual(0, timebound.RollCount);
		f.WeaponSkill.Verify(x => x.TraitUsed(It.IsAny<IHaveTraits>(), It.IsAny<Outcome>(), It.IsAny<Difficulty>(),
			It.IsAny<TraitUseType>(), It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Never);

		// Current sight constraints require a fresh evaluation, without reading or writing that cache.
		var constrained = Resolve(CombatCheck.For(f.World.Object, CheckType.MeleeWeaponCheck));
		Assert.AreEqual(Outcome.Fail, constrained.Outcome);
		Assert.AreEqual(10.0, constrained.TargetNumber);
		Assert.AreEqual(1, timebound.RollCount);
		Assert.AreEqual(1, f.Blind.CheckCount);
		cached.Verify(x => x.SameCheck(It.IsAny<CheckType>(), It.IsAny<Difficulty>(), It.IsAny<IFrameworkItem>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IFrameworkItem>()), Times.Once);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()), Times.Never);

		// An ordinary cache miss still rolls, learns and stores the ordinary result.
		f.Actor.Setup(x => x.EffectsOfType<ICheckResultEffect>(It.IsAny<Predicate<ICheckResultEffect>>())).Returns([]);
		Assert.AreEqual(Outcome.Fail, Resolve(timebound).Outcome);
		Assert.AreEqual(2, timebound.RollCount);
		f.Actor.Verify(x => x.AddEffect(It.IsAny<CheckResult>(),
			It.Is<TimeSpan>(duration => duration.TotalSeconds >= 60 && duration.TotalSeconds <= 61)), Times.Once);
		f.WeaponSkill.Verify(x => x.TraitUsed(f.Actor.Object, It.IsAny<Outcome>(), Difficulty.Normal,
			TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Exactly(2));
	}

	[TestMethod]
	public void ForDefense_OnlyEligiblePhysicalMelee_WrapsAccuracyAndExcludesRangedOrEstablishedContact()
	{
		var f = new Fixture();
		f.World.Setup(x => x.GetCheck(CheckType.DodgeCheck)).Returns(f.Ordinary);
		ICombatMove[] physical =
		[
			TestObjectFactory.CreateUninitialized<MeleeWeaponAttack>(),
			TestObjectFactory.CreateUninitialized<NaturalAttackMove>(),
			TestObjectFactory.CreateUninitialized<InitiateGrappleMove>()
		];
		ICombatMove[] excluded =
		[
			TestObjectFactory.CreateUninitialized<RangedNaturalAttackMove>(),
			TestObjectFactory.CreateUninitialized<RangedWeaponAttackMove>(),
			TestObjectFactory.CreateUninitialized<RangedTaggedMelee>(),
			TestObjectFactory.CreateUninitialized<RangedWeaponTaggedMelee>(),
			TestObjectFactory.CreateUninitialized<ExtendGrappleMove>(),
			TestObjectFactory.CreateUninitialized<BreakClinchMove>()
		];

		foreach (var move in physical)
		{
			Assert.IsTrue(CombatCheck.CoversPhysicalDefense(move), move.GetType().Name);
			Assert.IsInstanceOfType(CombatCheck.ForDefense(f.World.Object, CheckType.DodgeCheck, move), typeof(CombatCheck));
		}
		foreach (var move in excluded)
		{
			Assert.IsFalse(CombatCheck.CoversPhysicalDefense(move), move.GetType().Name);
			Assert.AreSame(f.Ordinary, CombatCheck.ForDefense(f.World.Object, CheckType.DodgeCheck, move), move.GetType().Name);
		}
	}

	private static IBlindFightingMerit Merit(Fixture f, long id, int stages, bool applies = true)
	{
		var merit = new Mock<IBlindFightingMerit>();
		merit.SetupGet(x => x.Id).Returns(id);
		merit.SetupGet(x => x.DifficultyStages).Returns(stages);
		merit.Setup(x => x.Applies(f.Actor.Object, f.Opponent.Object)).Returns(applies);
		return merit.Object;
	}

	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly Mock<ICharacter> Actor = new();
		internal readonly Mock<ICharacter> Opponent = new();
		internal readonly Mock<IRoom> Room = new();
		internal readonly Mock<ITraitDefinition> Weapon = new();
		internal readonly Mock<ITraitDefinition> BlindDefinition = new();
		internal readonly Mock<ITrait> WeaponSkill = new();
		internal readonly Mock<ITrait> BlindSkill = new();
		internal readonly CountingCheck Ordinary;
		internal readonly CountingCheck Blind;
		internal readonly ICheck Combat;
		private readonly All<ITraitExpression> _expressions = new();

		internal Fixture(bool constantBlind = false)
		{
			// Avoid relying on a running Futuremud singleton to initialise the shared bonus scale.
			typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!
				.SetValue(null, 10);
			World.SetupGet(x => x.TraitExpressions).Returns(_expressions);
			World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns("true");
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Opponent.SetupGet(x => x.Location).Returns(Room.Object);
			Actor.SetupGet(x => x.Merits).Returns([]);
			Actor.SetupGet(x => x.VisionPercentage).Returns(1);
			Actor.Setup(x => x.EffectsOfType<ICheckBonusEffect>(It.IsAny<Predicate<ICheckBonusEffect>>())).Returns([]);
			Actor.Setup(x => x.ColocatedWith(Opponent.Object)).Returns(true);
			Actor.Setup(x => x.CanSee(Opponent.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
			Actor.Setup(x => x.IlluminationSightDifficulty(Room.Object)).Returns(Difficulty.Normal);
			Weapon.SetupGet(x => x.Id).Returns(1);
			BlindDefinition.SetupGet(x => x.Id).Returns(2);
			WeaponSkill.SetupGet(x => x.Definition).Returns(Weapon.Object);
			BlindSkill.SetupGet(x => x.Definition).Returns(BlindDefinition.Object);
			Actor.Setup(x => x.GetTrait(Weapon.Object)).Returns(WeaponSkill.Object);
			Actor.Setup(x => x.GetTrait(BlindDefinition.Object)).Returns(BlindSkill.Object);
			Actor.Setup(x => x.HasTrait(It.IsAny<ITraitDefinition>())).Returns(true);
			Actor.Setup(x => x.TraitValue(Weapon.Object, It.IsAny<TraitBonusContext>())).Returns(70);
			Actor.Setup(x => x.TraitValue(BlindDefinition.Object, It.IsAny<TraitBonusContext>())).Returns(40);
			var ordinaryExpression = Expression(100, "variable");
			var blindExpression = Expression(101, constantBlind ? "100" : "blind");
			if (!constantBlind) blindExpression.Parameters.Add("blind", new TraitExpressionParameter
			{
				Trait = BlindDefinition.Object, CanImprove = true, CanBranch = false
			});
			Ordinary = new CountingCheck(Model(ordinaryExpression, CheckType.MeleeWeaponCheck), World.Object);
			Blind = new CountingCheck(Model(blindExpression, CheckType.BlindFightingCheck), World.Object);
			World.Setup(x => x.GetCheck(CheckType.MeleeWeaponCheck)).Returns(Ordinary);
			World.Setup(x => x.GetCheck(CheckType.BlindFightingCheck)).Returns(Blind);
			Combat = CombatCheck.For(World.Object, CheckType.MeleeWeaponCheck);
		}

		private TraitExpression Expression(long id, string formula)
		{
			var expression = new TraitExpression(new MudSharp.Models.TraitExpression
			{
				Id = id, Name = "test expression", Expression = formula
			}, World.Object);
			_expressions.Add(expression);
			return expression;
		}

		internal MudSharp.Models.Check Model(ITraitExpression expression, CheckType type)
		{
			var template = new MudSharp.Models.CheckTemplate
			{
				Name = "blind fighting test", ImproveTraits = true, CanBranchIfTraitMissing = false
			};
			foreach (var difficulty in Enum.GetValues<Difficulty>())
				template.CheckTemplateDifficulties.Add(new MudSharp.Models.CheckTemplateDifficulty
				{
					Difficulty = (int)difficulty, Modifier = ((int)Difficulty.Normal - (int)difficulty) * 10
				});
			return new MudSharp.Models.Check
			{
				Type = (int)type, TraitExpressionId = expression.Id, CheckTemplate = template,
				MaximumDifficultyForImprovement = (int)Difficulty.Impossible
			};
		}

		internal void VerifyLearning(Difficulty normalDifficulty, Difficulty blindDifficulty)
		{
			WeaponSkill.Verify(x => x.TraitUsed(Actor.Object, It.IsAny<Outcome>(), normalDifficulty,
				TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
			BlindSkill.Verify(x => x.TraitUsed(Actor.Object, It.IsAny<Outcome>(), blindDifficulty,
				TraitUseType.Practical, It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Once);
		}
	}

	private sealed class CountingCheck(MudSharp.Models.Check model, IFuturemud world) : StandardCheck(model, world)
	{
		internal int RollCount { get; private set; }
		internal int CheckCount { get; private set; }
		internal ITraitDefinition? LastVariable { get; private set; }

		protected override Outcome RollAgainst(double target, out List<double> rolls)
		{
			RollCount++;
			rolls = [50, 50, 50];
			return target >= 50 ? Outcome.Pass : Outcome.Fail;
		}

		public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
			IPerceivable target = null!, double externalBonus = 0, TraitUseType traitUseType = TraitUseType.Practical,
			params (string Parameter, object value)[] customParameters)
		{
			CheckCount++;
			LastVariable = trait;
			return base.Check(checkee, difficulty, trait, target, externalBonus, traitUseType, customParameters);
		}
	}

	private sealed class ProbeOGLCheck(MudSharp.Models.Check model, IFuturemud world) : OGLCheck(model, world)
	{
		internal Difficulty? LastMinimumDifficulty { get; private set; }
		internal double LastRollThreshold { get; private set; }

		protected override Outcome RollAgainst(double target)
		{
			LastRollThreshold = target;
			return Outcome.Fail;
		}

		public override double TargetNumber(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
			IPerceivable target = null!, double externalBonus = 0, params (string Parameter, object value)[] customParameters)
		{
			LastMinimumDifficulty = CheckEvaluationContext.Find(customParameters)?.MinimumDifficulty;
			return base.TargetNumber(checkee, difficulty, trait, target, externalBonus, customParameters);
		}

		public override CheckOutcome Check(IPerceivableHaveTraits checkee, Difficulty difficulty, ITraitDefinition trait,
			IPerceivable target = null!, double externalBonus = 0, TraitUseType traitUseType = TraitUseType.Practical,
			params (string Parameter, object value)[] customParameters)
		{
			LastMinimumDifficulty = CheckEvaluationContext.Find(customParameters)?.MinimumDifficulty;
			return base.Check(checkee, difficulty, trait, target, externalBonus, traitUseType, customParameters);
		}

		public override Tuple<CheckOutcome, CheckOutcome> MultiDifficultyCheck(IPerceivableHaveTraits checkee,
			Difficulty difficulty1, Difficulty difficulty2, IPerceivable target = null!, ITraitDefinition trait = null!,
			double externalBonus = 0, TraitUseType traitUseType = TraitUseType.Practical,
			params (string Parameter, object value)[] customParameters)
		{
			LastMinimumDifficulty = CheckEvaluationContext.Find(customParameters)?.MinimumDifficulty;
			return base.MultiDifficultyCheck(checkee, difficulty1, difficulty2, target, trait, externalBonus, traitUseType, customParameters);
		}
	}

	private sealed class CountingTimeboundCheck(MudSharp.Models.Check model, IFuturemud world) : TimeboundCheck(model, world)
	{
		internal int RollCount { get; private set; }

		protected override Outcome RollAgainst(double target, out List<double> rolls)
		{
			RollCount++;
			rolls = [50, 50, 50];
			return Outcome.Fail;
		}
	}

	// These subclasses also meet the physical melee allowlist, proving each ranged
	// interface itself excludes a move. The eligibility test never runs constructors.
	private sealed class RangedTaggedMelee() : MeleeWeaponAttack(null!, null!, null!, null!), IRangedAttackMove
	{
	}

	private sealed class RangedWeaponTaggedMelee() : MeleeWeaponAttack(null!, null!, null!, null!), IRangedWeaponAttackMove
	{
		IRangedWeapon IRangedWeaponAttackMove.Weapon => null!;
	}
}
