#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

public partial class BackstabAttackTests
{
	[TestMethod]
	public void DualWield_DefenceLearningCallbackInvalidatesActor_PreservesBothOutcomesWithoutDamage()
	{
		var f = new Fixture(); var check = f.ConfigureResolution(Outcome.Pass); var valid = true;
		f.Attack.RecoveryDifficultyFailure = Difficulty.Hard;
		check.Setup(x => x.CheckAgainstAllDifficulties(f.Target.Object, It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), f.Actor.Object, It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string, object)[]>())).Returns(() =>
		{
			valid = false;
			return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.DodgeCheck, Outcome.Fail));
		});
		var move = new MeleeWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object, 0, () => valid);
		var result = move.ResolveMove(new DodgeMove { Assailant = f.Target.Object });
		Assert.AreEqual(Outcome.Pass, result.AttackerOutcome); Assert.AreEqual(Outcome.Fail, result.DefenderOutcome);
		Assert.IsTrue(result.DefenderResponseWasUsed); Assert.AreEqual(Difficulty.Hard, result.RecoveryDifficulty);
		f.Target.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
	}

	[TestMethod]
	public void DualWield_RealStrike_UsesFamilySkillAtAdjustedDifficultyAndOrdinaryDamagePath()
	{
		var f = new Fixture(); var check = f.ConfigureResolution(Outcome.Pass);
		var trait = Mock.Of<ITraitDefinition>(); f.WeaponType.SetupGet(x => x.AttackTrait).Returns(trait);
		IDamage? damage = null;
		f.Target.Setup(x => x.PassiveSufferDamage(It.IsAny<IDamage>())).Callback<IDamage>(x => damage = x).Returns([]);
		var move = new MeleeWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object, 1, () => true);
		var result = move.ResolveMove(new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.IsTrue(move.AttackWasAttempted); Assert.IsNotNull(damage); Assert.IsTrue(result.MoveWasSuccessful);
		var attackRoll = check.Invocations.Single(x => x.Method.Name == "CheckAgainstAllDifficulties");
		Assert.AreEqual(Difficulty.Hard, attackRoll.Arguments[1]); Assert.AreSame(trait, attackRoll.Arguments[2]);
	}

	[TestMethod]
	public void DualWield_CheckCallbackInvalidatesPhysicalActor_CommittedAttemptKeepsCostAndFailureRecovery()
	{
		var f = new Fixture(); var check = f.ConfigureResolution(Outcome.Pass); var valid = true;
		f.Attack.RecoveryDifficultyFailure = Difficulty.Hard;
		check.Setup(x => x.CheckAgainstAllDifficulties(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<ITraitDefinition>(), It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(),
			It.IsAny<(string, object)[]>())).Returns(() =>
		{
			valid = false;
			return Enum.GetValues<Difficulty>().ToDictionary(x => x, _ => CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponCheck, Outcome.Fail));
		});
		var move = new MeleeWeaponAttack(f.Actor.Object, f.Weapon.Object, f.Attack, f.Target.Object, 0, () => valid);
		using var scope = CommandExecutionScope.EnterMove(move);
		var result = CommandExecutionScope.Resolve(move, new HelplessDefenseMove { Assailant = f.Target.Object });
		Assert.IsTrue(CommandExecutionScope.HasCommitted); Assert.IsTrue(move.UsesStaminaWithResult(result));
		Assert.AreEqual(Outcome.Fail, result.AttackerOutcome); Assert.AreEqual(Difficulty.Hard, result.RecoveryDifficulty);
		Assert.IsFalse(result.DefenderResponseWasUsed);
		f.Target.Verify(x => x.PassiveSufferDamage(It.IsAny<IDamage>()), Times.Never);
	}
}
