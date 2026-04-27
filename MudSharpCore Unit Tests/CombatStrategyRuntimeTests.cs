#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Combat.Strategies;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Scheduling;
using MudSharp.GameItems;
using MudSharp.Magic.Powers;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Reflection;
using TraitExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatStrategyRuntimeTests
{
	private static Mock<IFuturemud> CreateGameworld()
	{
		Mock<IFuturemud> gameworld = new();
		gameworld.Setup(x => x.GetStaticDouble("DodgeMoveStaminaCost")).Returns(10.0);
		gameworld.Setup(x => x.GetStaticDouble("DodgeRangeMoveStaminaCost")).Returns(1.0);

		Mock<ICheck> check = new();
		check.Setup(x => x.TargetNumber(
				It.IsAny<IPerceivableHaveTraits>(),
				It.IsAny<Difficulty>(),
				It.IsAny<ITraitDefinition>(),
				It.IsAny<IPerceivable>(),
				It.IsAny<double>(),
				It.IsAny<(string Parameter, object value)[]>()))
			.Returns(50.0);
		gameworld.Setup(x => x.GetCheck(It.IsAny<CheckType>())).Returns(check.Object);

		typeof(CombatBase)
			.GetProperty("GraceMoveStaminaCost", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, new TraitExpression("1", gameworld.Object));
		typeof(StandardCheck)
			.GetField("_bonusesPerDifficultyLevel", BindingFlags.Static | BindingFlags.NonPublic)!
			.SetValue(null, 10);

		return gameworld;
	}

	private static Mock<ICharacter> CreateDefendingCharacter(IFuturemud gameworld)
	{
		Mock<IBody> body = new();
		body.SetupGet(x => x.WieldedItems).Returns(Array.Empty<IGameItem>());

		Mock<IRace> race = new();
		race.SetupGet(x => x.CombatSettings).Returns(new RacialCombatSettings
		{
			CanAttack = true,
			CanDefend = true,
			CanUseWeapons = true
		});

		Mock<ICharacterCombatSettings> combatSettings = new();

		Mock<ICharacter> character = new();
		character.SetupGet(x => x.Gameworld).Returns(gameworld);
		character.SetupGet(x => x.Race).Returns(race.Object);
		character.SetupGet(x => x.Body).Returns(body.Object);
		character.SetupProperty(x => x.State, CharacterState.Awake);
		character.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		character.SetupProperty(x => x.PreferredDefenseType, DefenseType.Dodge);
		character.SetupProperty(x => x.RidingMount, null);
		character.SetupProperty(x => x.CombatSettings, combatSettings.Object);
		character.SetupGet(x => x.Encumbrance).Returns(EncumbranceLevel.Unencumbered);
		character.SetupGet(x => x.EncumbrancePercentage).Returns(0.0);
		character.SetupGet(x => x.Effects).Returns(Array.Empty<IEffect>());
		character.Setup(x => x.CombinedEffectsOfType<IEffect>()).Returns(Array.Empty<IEffect>());
		character.Setup(x => x.EffectsOfType<ClinchEffect>(It.IsAny<Predicate<ClinchEffect>>()))
			.Returns(Array.Empty<ClinchEffect>());
		character.Setup(x => x.EffectsOfType<IWardBeatenEffect>(It.IsAny<Predicate<IWardBeatenEffect>>()))
			.Returns(Array.Empty<IWardBeatenEffect>());
		character.Setup(x => x.CanSpendStamina(It.IsAny<double>())).Returns(true);
		return character;
	}

	[TestMethod]
	public void StandardMelee_ResponseToStartClinch_CanDodgeWhenStandingAndHasStamina()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<ICharacter> defender = CreateDefendingCharacter(gameworld.Object);
		Mock<ICharacter> attacker = CreateDefendingCharacter(gameworld.Object);
		StartClinchMove move = new(defender.Object) { Assailant = attacker.Object };

		ICombatMove response = StandardMeleeStrategy.Instance.ResponseToMove(move, defender.Object, attacker.Object);

		Assert.IsInstanceOfType(response, typeof(DodgeMove));
	}

	[TestMethod]
	public void StandardRange_ResponseToMagicPowerAttack_DelegatesToDodgeDefense()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		Mock<ICharacter> defender = CreateDefendingCharacter(gameworld.Object);
		Mock<ICharacter> attacker = CreateDefendingCharacter(gameworld.Object);

		Mock<IDamageProfile> profile = new();
		profile.SetupProperty(x => x.BaseBlockDifficulty, Difficulty.Impossible);
		profile.SetupProperty(x => x.BaseParryDifficulty, Difficulty.Impossible);
		profile.SetupProperty(x => x.BaseDodgeDifficulty, Difficulty.Normal);

		Mock<IWeaponAttack> weaponAttack = new();
		weaponAttack.SetupGet(x => x.Profile).Returns(profile.Object);

		Mock<IMagicAttackPower> power = new();
		power.SetupGet(x => x.WeaponAttack).Returns(weaponAttack.Object);
		power.SetupGet(x => x.ValidDefenseTypes).Returns(new[] { DefenseType.Dodge });

		MagicPowerAttackMove move = new(attacker.Object, defender.Object, power.Object);

		ICombatMove response = StandardRangeStrategy.Instance.ResponseToMove(move, defender.Object, attacker.Object);

		Assert.IsInstanceOfType(response, typeof(DodgeMove));
	}

	[TestMethod]
	public void RangePathFunction_SwimOnly_RejectsAbjectFailureAndAllowsCapableSwimmers()
	{
		Mock<ICharacter> character = new();
		Mock<IFuturemud> gameworld = new();
		Mock<ICheck> swimCheck = new();
		Mock<IExit> exitData = new();
		Mock<ICellExit> exit = new();

		character.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		gameworld.Setup(x => x.GetCheck(CheckType.SwimStayAfloatCheck)).Returns(swimCheck.Object);
		exit.SetupGet(x => x.Exit).Returns(exitData.Object);
		exit.Setup(x => x.MovementTransition(character.Object))
			.Returns((CellMovementTransition.SwimOnly, RoomLayer.GroundLevel));
		Func<ICellExit, bool> predicate = StandardRangeStrategy.Instance.GetPathFunction(character.Object);

		swimCheck.Setup(x => x.WouldBeAbjectFailure(character.Object, null)).Returns(true);
		Assert.IsFalse(predicate(exit.Object));

		swimCheck.Setup(x => x.WouldBeAbjectFailure(character.Object, null)).Returns(false);
		Assert.IsTrue(predicate(exit.Object));
	}

	[TestMethod]
	public void RangePathFunction_FlyOnly_RequiresFlightCapability()
	{
		Mock<ICharacter> character = new();
		Mock<IExit> exitData = new();
		Mock<ICellExit> exit = new();

		character.SetupProperty(x => x.PositionState, PositionStanding.Instance);
		exit.SetupGet(x => x.Exit).Returns(exitData.Object);
		exit.Setup(x => x.MovementTransition(character.Object))
			.Returns((CellMovementTransition.FlyOnly, RoomLayer.InAir));
		Func<ICellExit, bool> predicate = StandardRangeStrategy.Instance.GetPathFunction(character.Object);

		character.Setup(x => x.CanFly()).Returns((false, "cannot fly"));
		Assert.IsFalse(predicate(exit.Object));

		character.Setup(x => x.CanFly()).Returns((true, string.Empty));
		Assert.IsTrue(predicate(exit.Object));

		character.Object.PositionState = PositionFlying.Instance;
		character.Setup(x => x.CanFly()).Returns((false, "already airborne"));
		Assert.IsTrue(predicate(exit.Object));
	}

	[TestMethod]
	public void CombatStrategyModeExtensions_Swooper_IsValidMeleeAndRangedMode()
	{
		Assert.AreEqual("Swooper", CombatStrategyMode.Swooper.Describe());
		Assert.IsTrue(CombatStrategyMode.Swooper.DescribeWordy().Contains("swooping"));
		Assert.IsTrue(CombatStrategyMode.Swooper.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.Swooper.IsRangedStrategy());
		Assert.IsTrue(CombatStrategyMode.Swooper.IsRangedStartDesiringStrategy());
		Assert.IsFalse(CombatStrategyMode.Swooper.IsMeleeDesiredStrategy());
	}

	[TestMethod]
	public void CombatStrategyModeExtensions_NewPredatorModes_AreClassifiedForMeleeSelection()
	{
		Assert.AreEqual("Drowner", CombatStrategyMode.Drowner.Describe());
		Assert.AreEqual("Dropper", CombatStrategyMode.Dropper.Describe());
		Assert.AreEqual("Physical Avoider", CombatStrategyMode.PhysicalAvoider.Describe());

		Assert.IsTrue(CombatStrategyMode.Drowner.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.Dropper.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsMeleeStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsRangedStrategy());
		Assert.IsTrue(CombatStrategyMode.Drowner.IsMeleeDesiredStrategy());
		Assert.IsTrue(CombatStrategyMode.Dropper.IsMeleeDesiredStrategy());
		Assert.IsTrue(CombatStrategyMode.PhysicalAvoider.IsRangedStartDesiringStrategy());
	}

	[TestMethod]
	public void CheckTypeExtensions_ForcedPositioningChecks_AreCombatPhysicalChecks()
	{
		foreach (var check in new[]
		         {
			         CheckType.PushbackCheck,
			         CheckType.OpposePushbackCheck,
			         CheckType.ForcedMovementCheck,
			         CheckType.OpposeForcedMovementCheck
		         })
		{
			Assert.IsTrue(check.IsPhysicalActivityCheck(), $"{check} should be a physical activity check.");
			Assert.IsTrue(check.IsTargettedHostileCheck(), $"{check} should be targeted and hostile.");
			Assert.IsTrue(check.IsVisionInfluencedCheck(), $"{check} should be vision influenced.");
		}

		Assert.IsTrue(CheckType.PushbackCheck.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.ForcedMovementCheck.IsOffensiveCombatAction());
		Assert.IsTrue(CheckType.OpposePushbackCheck.IsDefensiveCombatAction());
		Assert.IsTrue(CheckType.OpposeForcedMovementCheck.IsDefensiveCombatAction());
	}

	[TestMethod]
	public void CombatForcedMovementUtilities_ApplyPushback_ClearsMeleeAndSchedulesScaledDelay()
	{
		Mock<IScheduler> scheduler = new();
		Mock<IFuturemud> gameworld = new();
		gameworld.SetupGet(x => x.Scheduler).Returns(scheduler.Object);
		gameworld.Setup(x => x.GetStaticDouble("PushbackCombatDelayBaseSeconds")).Returns(2.0);
		gameworld.Setup(x => x.GetStaticDouble("PushbackCombatDelayPerDegreeSeconds")).Returns(5.0);

		Mock<IBody> actorBody = new();
		Mock<IBody> targetBody = new();
		Mock<ICharacter> actor = new();
		Mock<ICharacter> target = new();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.Body).Returns(actorBody.Object);
		actor.SetupProperty(x => x.MeleeRange, true);
		actor.SetupProperty(x => x.CombatTarget, target.Object);
		target.SetupGet(x => x.Body).Returns(targetBody.Object);
		target.SetupProperty(x => x.MeleeRange, true);

		CombatForcedMovementUtilities.ApplyPushback(actor.Object, target.Object, 3);

		Assert.IsFalse(actor.Object.MeleeRange);
		Assert.IsFalse(target.Object.MeleeRange);
		scheduler.Verify(x => x.DelayScheduleType(
			target.Object,
			ScheduleType.Combat,
			It.Is<TimeSpan>(delay => Math.Abs(delay.TotalSeconds - 17.0 * CombatBase.CombatSpeedMultiplier) < 0.001)),
			Times.Once);
	}

	[TestMethod]
	public void ForcedMovementMove_Description_UsesValidVerbParticiple()
	{
		Mock<ICharacter> attacker = new();
		Mock<ICharacter> target = new();
		Mock<IForcedMovementAttack> attack = new();

		ForcedMovementMove shove = new(attacker.Object, target.Object, attack.Object, ForcedMovementVerbs.Shove,
			RoomLayer.GroundLevel);
		ForcedMovementMove pull = new(attacker.Object, target.Object, attack.Object, ForcedMovementVerbs.Pull,
			RoomLayer.GroundLevel);

		StringAssert.StartsWith(shove.Description, "Shoving");
		StringAssert.StartsWith(pull.Description, "Pulling");
	}
}
