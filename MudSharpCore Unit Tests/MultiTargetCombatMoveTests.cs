#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MultiTargetCombatMoveTests
{
	[TestMethod]
	public void PullMoveTypes_AppendAfterPersistedMoveCatalogue()
	{
		Assert.AreEqual((int)BuiltInCombatMoveType.AquaticVehicleAttack + 1,
			(int)BuiltInCombatMoveType.PullToMelee);
		Assert.AreEqual((int)BuiltInCombatMoveType.PullToMelee + 1,
			(int)BuiltInCombatMoveType.PullToMeleeUnarmed);
	}

	[TestMethod]
	public void MeleeWeaponAttack_ExplicitSecondaryTarget_IsThePrimaryTarget()
	{
		var assailant = new Mock<ICharacter>();
		var originalTarget = new Mock<ICharacter>();
		var secondaryTarget = new Mock<ICharacter>();
		assailant.SetupGet(x => x.CombatTarget).Returns(originalTarget.Object);
		assailant.Setup(x => x.ColocatedWith(secondaryTarget.Object)).Returns(true);

		var move = new MeleeWeaponAttack(
			assailant.Object,
			new Mock<IMeleeWeapon>().Object,
			new Mock<IWeaponAttack>().Object,
			secondaryTarget.Object);

		Assert.AreSame(secondaryTarget.Object, move.PrimaryTarget);
		CollectionAssert.Contains(move.CharacterTargets.ToList(), secondaryTarget.Object);
	}

	[TestMethod]
	public void NaturalAttackMove_ExplicitSecondaryTarget_IsThePrimaryTarget()
	{
		var assailant = new Mock<ICharacter>();
		var originalTarget = new Mock<ICharacter>();
		var secondaryTarget = new Mock<ICharacter>();
		var naturalAttack = new Mock<INaturalAttack>();
		naturalAttack.SetupGet(x => x.Attack).Returns(new Mock<IWeaponAttack>().Object);
		assailant.SetupGet(x => x.CombatTarget).Returns(originalTarget.Object);

		var move = new NaturalAttackMove(assailant.Object, naturalAttack.Object, secondaryTarget.Object);

		Assert.AreSame(secondaryTarget.Object, move.PrimaryTarget);
		CollectionAssert.Contains(move.CharacterTargets.ToList(), secondaryTarget.Object);
	}

	[TestMethod]
	public void WrapWeaponAttack_IntrinsicAreaAttack_DoesNotMultiplyPayloadResolution()
	{
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<IWeaponAttack> attack = new();
		Mock<ICombatMove> singleMove = new();
		attack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.BreathWeaponAttack);
		attack.SetupGet(x => x.MaximumTargets).Returns(5);

		var move = MultiTargetCombatMove.WrapWeaponAttack(assailant.Object, primary.Object, attack.Object, null,
			_ => singleMove.Object);

		Assert.AreSame(singleMove.Object, move);
	}

	[TestMethod]
	public void WrapWeaponAttack_ScreechUsesBoundedHostileTargets()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> secondary = new();
		Mock<IWeaponAttack> attack = new();

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
		assailant.Setup(x => x.CanSee(It.IsAny<ICharacter>())).Returns(true);
		assailant.Setup(x => x.ColocatedWith(secondary.Object)).Returns(true);
		primary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.Combat).Returns(combat.Object);
		attack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.ScreechAttack);
		attack.SetupGet(x => x.MaximumTargets).Returns(2);
		attack.Setup(x => x.UsableAttack(assailant.Object, null, secondary.Object,
			AttackHandednessOptions.Any, true, BuiltInCombatMoveType.ScreechAttack)).Returns(true);
		combat.SetupGet(x => x.Combatants)
		      .Returns(new IPerceiver[] { assailant.Object, primary.Object, secondary.Object });

		var move = MultiTargetCombatMove.WrapWeaponAttack(assailant.Object, primary.Object, attack.Object, null,
			target =>
			{
				Mock<IWeaponAttackMove> targetMove = new();
				targetMove.SetupGet(x => x.Assailant).Returns(assailant.Object);
				return targetMove.Object;
			});

		Assert.IsInstanceOfType(move, typeof(MultiTargetCombatMove));
		Assert.AreEqual(2, move.CharacterTargets.Count());
	}

	[TestMethod]
	public void ScreechDamageComponents_RejectNegativeAndNonFiniteValues()
	{
		Assert.AreEqual(0.0, ScreechAttackMove.NonNegativeFinite(-1.0));
		Assert.AreEqual(0.0, ScreechAttackMove.NonNegativeFinite(double.NaN));
		Assert.AreEqual(0.0, ScreechAttackMove.NonNegativeFinite(double.PositiveInfinity));
		Assert.AreEqual(12.5, ScreechAttackMove.NonNegativeFinite(12.5));
	}

	[TestMethod]
	public void WrapWeaponAttack_TargetFailsTargetSpecificUsability_DoesNotAddSecondaryTarget()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> secondary = new();
		Mock<IWeaponAttack> attack = new();
		Mock<IGameItem> weapon = new();
		Mock<ICombatMove> primaryMove = new();

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		assailant.Setup(x => x.ColocatedWith(secondary.Object)).Returns(true);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
		primary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		secondary.SetupGet(x => x.MeleeRange).Returns(true);
		attack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.UseWeaponAttack);
		attack.SetupGet(x => x.MaximumTargets).Returns(2);
		attack.Setup(x => x.UsableAttack(assailant.Object, weapon.Object, secondary.Object,
				AttackHandednessOptions.Any, true, BuiltInCombatMoveType.UseWeaponAttack))
		      .Returns(false);
		combat.SetupGet(x => x.Combatants)
		      .Returns(new IPerceiver[] { assailant.Object, primary.Object, secondary.Object });

		var move = MultiTargetCombatMove.WrapWeaponAttack(assailant.Object, primary.Object, attack.Object,
			weapon.Object, _ => primaryMove.Object);

		Assert.AreSame(primaryMove.Object, move);
		attack.Verify(x => x.UsableAttack(assailant.Object, weapon.Object, secondary.Object,
			AttackHandednessOptions.Any, true, BuiltInCombatMoveType.UseWeaponAttack), Times.Once);
	}

	[TestMethod]
	public void SelectTargets_MeleeAttack_OnlyAddsHostilesEngagedWithAssailantAtMeleeRange()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> engagedEnemy = new();
		Mock<ICharacter> spacedEnemy = new();
		Mock<ICharacter> otherEnemy = new();
		Mock<ICharacter> ally = new();

		foreach (var character in new[] { assailant, primary, engagedEnemy, spacedEnemy, otherEnemy, ally })
		{
			character.SetupGet(x => x.Combat).Returns(combat.Object);
		}

		engagedEnemy.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		engagedEnemy.SetupGet(x => x.MeleeRange).Returns(true);
		spacedEnemy.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		spacedEnemy.SetupGet(x => x.MeleeRange).Returns(false);
		otherEnemy.SetupGet(x => x.CombatTarget).Returns(primary.Object);
		otherEnemy.SetupGet(x => x.MeleeRange).Returns(true);
		ally.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		ally.SetupGet(x => x.MeleeRange).Returns(true);

		assailant.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>()))
		         .Returns((ICharacter character) => character == ally.Object);
		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[]
		{
			assailant.Object,
			primary.Object,
			engagedEnemy.Object,
			spacedEnemy.Object,
			otherEnemy.Object,
			ally.Object
		});

		var targets = MultiTargetCombatMove
			.SelectTargets(assailant.Object, primary.Object, 6, false, null)
			.ToList();

		CollectionAssert.AreEqual(new[] { primary.Object, engagedEnemy.Object }, targets);
	}

	[TestMethod]
	public void SelectTargets_RangedNaturalAttack_AddsVisibleHostilesDespiteMeleeSpacing()
	{
		Mock<ICombat> combat = new();
		Mock<ICell> location = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> spacedEnemy = new();
		Mock<ICharacter> hiddenEnemy = new();
		Mock<ICharacter> ally = new();
		Mock<IRangedNaturalAttack> attack = new();

		foreach (var character in new[] { assailant, primary, spacedEnemy, hiddenEnemy, ally })
		{
			character.SetupGet(x => x.Combat).Returns(combat.Object);
			character.SetupGet(x => x.Location).Returns(location.Object);
		}

		spacedEnemy.SetupGet(x => x.MeleeRange).Returns(false);
		attack.SetupGet(x => x.RangeInRooms).Returns(2);
		assailant.Setup(x => x.CanSee(spacedEnemy.Object)).Returns(true);
		assailant.Setup(x => x.CanSee(hiddenEnemy.Object)).Returns(false);
		assailant.Setup(x => x.CanSee(ally.Object)).Returns(true);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>()))
		         .Returns((ICharacter character) => character == ally.Object);
		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[]
		{
			assailant.Object,
			primary.Object,
			spacedEnemy.Object,
			hiddenEnemy.Object,
			ally.Object
		});

		var targets = MultiTargetCombatMove
			.SelectTargets(assailant.Object, primary.Object, 5, true, attack.Object)
			.ToList();

		CollectionAssert.AreEqual(new[] { primary.Object, spacedEnemy.Object }, targets);
	}

	[TestMethod]
	public void PullTargetIntoMelee_SharedCombatAndLocation_SetsBothEngagementDirections()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> target = new();

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		assailant.SetupGet(x => x.CombatTarget).Returns(target.Object);
		assailant.SetupProperty(x => x.MeleeRange, false);
		assailant.Setup(x => x.ColocatedWith(target.Object)).Returns(true);
		target.SetupGet(x => x.Combat).Returns(combat.Object);
		target.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		target.SetupProperty(x => x.MeleeRange, false);

		PullToMeleeMove.PullTargetIntoMelee(assailant.Object, target.Object);

		Assert.IsTrue(assailant.Object.MeleeRange);
		Assert.IsTrue(target.Object.MeleeRange);
	}

	[TestMethod]
	public void SelectTargets_PullToMelee_OnlyAddsColocatedOpponentsTargetingAssailant()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> pressingEnemy = new();
		Mock<ICharacter> unrelatedEnemy = new();
		Mock<IWeaponAttack> attack = new();

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
		assailant.Setup(x => x.ColocatedWith(It.IsAny<IPerceivable>())).Returns(true);
		assailant.Setup(x => x.CanSee(It.IsAny<ICharacter>())).Returns(true);
		primary.SetupGet(x => x.Combat).Returns(combat.Object);
		pressingEnemy.SetupGet(x => x.Combat).Returns(combat.Object);
		pressingEnemy.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		unrelatedEnemy.SetupGet(x => x.Combat).Returns(combat.Object);
		unrelatedEnemy.SetupGet(x => x.CombatTarget).Returns(primary.Object);
		attack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.PullToMelee);
		combat.SetupGet(x => x.Combatants).Returns(new IPerceiver[]
		{
			assailant.Object,
			primary.Object,
			pressingEnemy.Object,
			unrelatedEnemy.Object
		});

		var targets = MultiTargetCombatMove
			.SelectTargets(assailant.Object, primary.Object, 4, true, attack.Object)
			.ToList();

		CollectionAssert.AreEqual(new[] { primary.Object, pressingEnemy.Object }, targets);
	}

	[TestMethod]
	public void WrapAuxiliaryAction_MultipleEligibleTargets_PreservesTargetInventory()
	{
		Mock<ICombat> combat = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> secondary = new();
		Mock<IAuxiliaryCombatAction> action = new();

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		assailant.Setup(x => x.ColocatedWith(secondary.Object)).Returns(true);
		assailant.Setup(x => x.IsAlly(It.IsAny<ICharacter>())).Returns(false);
		primary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.CombatTarget).Returns(assailant.Object);
		secondary.SetupGet(x => x.MeleeRange).Returns(true);
		action.SetupGet(x => x.MaximumTargets).Returns(2);
		combat.SetupGet(x => x.Combatants)
		      .Returns(new IPerceiver[] { assailant.Object, primary.Object, secondary.Object });

		var move = MultiTargetCombatMove.WrapAuxiliaryAction(assailant.Object, primary.Object, action.Object,
			target => new AuxiliaryMove(assailant.Object, target, action.Object));

		Assert.IsInstanceOfType(move, typeof(MultiTargetCombatMove));
		CollectionAssert.AreEqual(new[] { primary.Object, secondary.Object }, move.CharacterTargets.ToList());
	}

	[TestMethod]
	public void ScoringResolutionsFor_MultiTargetMove_PreservesEachTargetsResponseAndOutcome()
	{
		Mock<ICombat> combat = new();
		Mock<IRace> race = new();
		Mock<ICharacter> assailant = new();
		Mock<ICharacter> primary = new();
		Mock<ICharacter> secondary = new();
		Mock<ICombatMove> primaryMove = new();
		Mock<ICombatMove> secondaryMove = new();
		Mock<ICombatMove> primaryResponse = new();
		Mock<ICombatMove> secondaryResponse = new();
		var primaryResult = new CombatMoveResult
		{
			MoveWasSuccessful = false,
			AttackerOutcome = Outcome.Fail,
			DefenderOutcome = Outcome.Pass
		};
		var secondaryResult = new CombatMoveResult
		{
			MoveWasSuccessful = true,
			AttackerOutcome = Outcome.Pass,
			DefenderOutcome = Outcome.Fail
		};

		assailant.SetupGet(x => x.Combat).Returns(combat.Object);
		primary.SetupGet(x => x.Combat).Returns(combat.Object);
		primary.SetupGet(x => x.Race).Returns(race.Object);
		primary.Setup(x => x.ResponseToMove(primaryMove.Object, assailant.Object)).Returns(primaryResponse.Object);
		secondary.SetupGet(x => x.Combat).Returns(combat.Object);
		secondary.SetupGet(x => x.Race).Returns(race.Object);
		secondary.Setup(x => x.ResponseToMove(secondaryMove.Object, assailant.Object)).Returns(secondaryResponse.Object);
		primaryMove.SetupGet(x => x.Assailant).Returns(assailant.Object);
		primaryMove.Setup(x => x.ResolveMove(primaryResponse.Object)).Returns(primaryResult);
		secondaryMove.SetupGet(x => x.Assailant).Returns(assailant.Object);
		secondaryMove.Setup(x => x.ResolveMove(secondaryResponse.Object)).Returns(secondaryResult);

		var move = new TestMultiTargetCombatMove(
			[primaryMove.Object, secondaryMove.Object],
			[primary.Object, secondary.Object]);
		var aggregate = move.ResolveMove(null!);
		var scoringResolutions = CombatBase.ScoringResolutionsFor(move, null!, aggregate).ToList();

		Assert.AreEqual(2, scoringResolutions.Count);
		Assert.AreSame(primaryMove.Object, scoringResolutions[0].Move);
		Assert.AreSame(primaryResponse.Object, scoringResolutions[0].Response);
		Assert.AreSame(primaryResult, scoringResolutions[0].Result);
		Assert.AreSame(secondaryMove.Object, scoringResolutions[1].Move);
		Assert.AreSame(secondaryResponse.Object, scoringResolutions[1].Response);
		Assert.AreSame(secondaryResult, scoringResolutions[1].Result);
	}

	private sealed class TestMultiTargetCombatMove : MultiTargetCombatMove
	{
		public TestMultiTargetCombatMove(IReadOnlyList<ICombatMove> moves, IReadOnlyList<ICharacter> targets)
			: base(moves, targets)
		{
		}
	}
}
