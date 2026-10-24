using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp_Unit_Tests;

public partial class BackstabAttackTests
{
	private int _originalBlindFightingBonusScale;

	[TestInitialize]
	public void CaptureBlindFightingBonusScale() => _originalBlindFightingBonusScale = (int)typeof(StandardCheck)
		.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;

	[TestCleanup]
	public void RestoreBlindFightingBonusScale() => typeof(StandardCheck)
		.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, _originalBlindFightingBonusScale);
	private static Mock<ICheck> ConfigureBlindFighting(Fixture f)
	{
		f.World.Setup(x => x.GetStaticConfiguration("BlindFightingEnabled")).Returns("true");
		typeof(StandardCheck).GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, 10);
		var blind = new Mock<ICheck>();
		blind.SetupGet(x => x.TargetNumberExpression).Returns(new TraitExpression("40", f.World.Object));
		f.World.Setup(x => x.GetCheck(CheckType.BlindFightingCheck)).Returns(blind.Object);
		var room = Mock.Of<IRoom>();
		f.Actor.SetupGet(x => x.Location).Returns(room);
		f.Target.SetupGet(x => x.Location).Returns(room);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(1);
		f.Target.SetupGet(x => x.VisionPercentage).Returns(1);
		f.Target.Setup(x => x.ColocatedWith(f.Actor.Object)).Returns(true);
		f.Actor.Setup(x => x.IlluminationSightDifficulty(room)).Returns(Difficulty.Easy);
		f.Target.Setup(x => x.IlluminationSightDifficulty(room)).Returns(Difficulty.Easy);
		return blind;
	}

	[TestMethod]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void PhysicalMeleeResolution_AssessesEachParticipantsCurrentSight(bool natural, bool attackerUnseen)
	{
		var f = new Fixture(natural);
		var ordinary = f.ConfigureResolution(Outcome.MajorFail);
		var blind = ConfigureBlindFighting(f);
		f.Attack.MoveType = natural ? BuiltInCombatMoveType.NaturalWeaponAttack : BuiltInCombatMoveType.UseWeaponAttack;
		f.Actor.Setup(x => x.CanSee(f.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(!attackerUnseen);
		f.Target.Setup(x => x.CanSee(f.Actor.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(attackerUnseen);
		var move = natural ? CombatMoveFactory.CreateNaturalWeaponAttack(f.Actor.Object, f.Natural, f.Target.Object) :
			CombatMoveFactory.CreateWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);

		move.ResolveMove(new DodgeMove { Assailant = f.Target.Object });

		var invocations = ordinary.Invocations.Where(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)).ToArray();
		Assert.AreEqual(2, invocations.Length, "Only the original attack and defence combat checks are executed.");
		var attackerContext = CheckEvaluationContext.Find(((string, object)[])invocations.Single(x => ReferenceEquals(x.Arguments[0], f.Actor.Object)).Arguments[6]);
		var defenderContext = CheckEvaluationContext.Find(((string, object)[])invocations.Single(x => ReferenceEquals(x.Arguments[0], f.Target.Object)).Arguments[6]);
		var impaired = attackerUnseen ? attackerContext : defenderContext;
		Assert.IsNotNull(impaired);
		Assert.AreEqual(40.0, impaired!.MaximumBaseTargetNumber);
		Assert.AreEqual(Difficulty.ExtremelyHard, impaired.MinimumDifficulty);
		Assert.IsNull(attackerUnseen ? defenderContext : attackerContext);
		blind.Verify(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
			It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Never);
	}

	[TestMethod]
	public void Backstab_RevealedAttackerRetainsOpeningWithoutPenalisingCurrentDefenderSight()
	{
		var f = new Fixture();
		var ordinary = f.ConfigureResolution(Outcome.MajorFail);
		ConfigureBlindFighting(f);
		f.Actor.Setup(x => x.GetFacingFor(f.Target.Object, false)).Returns(Facing.Front);
		f.Capture();
		Assert.IsTrue(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
		var move = new BackstabAttackMove(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);

		move.ResolveMove(new DodgeMove { Assailant = f.Target.Object });

		var calls = ordinary.Invocations.Where(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)).ToArray();
		Assert.AreEqual(2, calls.Length);
		Assert.IsTrue(calls.All(x => CheckEvaluationContext.Find(((string, object)[])x.Arguments[6]) is null));
		Assert.IsFalse(UnseenCombatOpening.HasOpening(f.Actor.Object, f.Target.Object));
	}

	[TestMethod]
	public void Backstab_BlindAttackerStillFailsExistingEligibilityGateBeforeChecking()
	{
		var f = new Fixture();
		var ordinary = f.ConfigureResolution(Outcome.MajorPass);
		ConfigureBlindFighting(f);
		f.Actor.Setup(x => x.CanSee(f.Target.Object, It.IsAny<PerceiveIgnoreFlags>())).Returns(false);
		var move = new BackstabAttackMove(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);

		Assert.AreSame(CombatMoveResult.Irrelevant, move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object }));
		Assert.IsFalse(ordinary.Invocations.Any(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)));
	}

	[TestMethod]
	[DataRow("ward")]
	[DataRow("grapple")]
	public void InitialContactAndWardContests_UseTheSameSightConstraints(string contest)
	{
		var f = new Fixture(true);
		var ordinary = f.ConfigureResolution(Outcome.MajorFail);
		ConfigureBlindFighting(f);
		f.Actor.SetupGet(x => x.VisionPercentage).Returns(0);
		f.Target.SetupGet(x => x.VisionPercentage).Returns(0);
		if (contest == "ward")
		{
			new WardProbe(f).Resolve(new WardDefenseMove { Assailant = f.Target.Object, WardWeapon = f.Weapon.Object });
		}
		else
		{
			new InitiateGrappleMove(f.Actor.Object, f.Natural, f.Target.Object)
				.ResolveMove(new DodgeMove { Assailant = f.Target.Object });
		}

		var calls = ordinary.Invocations.Where(x => x.Method.Name == nameof(ICheck.CheckAgainstAllDifficulties)).ToArray();
		Assert.AreEqual(2, calls.Length);
		Assert.IsTrue(calls.All(x => CheckEvaluationContext.Find(((string, object)[])x.Arguments[6]) is
			{ MinimumDifficulty: Difficulty.ExtremelyHard, MaximumBaseTargetNumber: 40.0 }));
	}

	private sealed class WardProbe(Fixture fixture)
		: MeleeWeaponAttack(fixture.Actor.Object, fixture.Weapon.Object, fixture.Attack, fixture.Target.Object)
	{
		public void Resolve(WardDefenseMove ward) => ResolveWard(ward);
	}

	[TestMethod]
	public void MagicalDefenseSelection_WardPreviewUsesBlindConstraintAndPreservesSightedFallback()
	{
		var f = new Fixture();
		f.ConfigureResolution(Outcome.MajorFail);
		ConfigureBlindFighting(f);
		var wardCheck = new Mock<ICheck>();
		CheckEvaluationContext? captured = null;
		wardCheck.Setup(x => x.TargetNumber(f.Target.Object, It.IsAny<Difficulty>(), null, f.Actor.Object,
			0.0, It.IsAny<(string, object)[]>())).Callback((IPerceivableHaveTraits _, Difficulty _, ITraitDefinition _,
			IPerceivable _, double _, (string, object)[] values) => captured = CheckEvaluationContext.Find(values)).Returns(7.0);
		f.World.Setup(x => x.GetCheck(CheckType.Ward)).Returns(wardCheck.Object);
		var attack = new MeleeWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object);
		var ward = new WardDefenseMove { Assailant = f.Target.Object, WardWeapon = f.Weapon.Object };
		var preview = typeof(MagicDefenseMove).GetMethod("MundaneChance", BindingFlags.Static | BindingFlags.NonPublic)!;
		f.Target.SetupGet(x => x.VisionPercentage).Returns(0);

		Assert.AreEqual(7.0, preview.Invoke(null, [f.Target.Object, attack, ward]));
		Assert.IsNotNull(captured);
		Assert.AreEqual(Difficulty.ExtremelyHard, captured!.MinimumDifficulty);
		Assert.AreEqual(40.0, captured.MaximumBaseTargetNumber);
		f.Target.SetupGet(x => x.VisionPercentage).Returns(1);
		Assert.AreEqual(100.0, preview.Invoke(null, [f.Target.Object, attack, ward]));
		Assert.AreEqual(1, wardCheck.Invocations.Count(x => x.Method.Name == nameof(ICheck.TargetNumber)));
	}
}
