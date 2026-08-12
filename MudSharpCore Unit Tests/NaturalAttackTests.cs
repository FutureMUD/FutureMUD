#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NaturalAttackTests
{
	[TestMethod]
	public void UsableAttack_PresentUsableBodypart_ReturnsTrue()
	{
		var (attack, attacker, body, bodypart, target) = CreateFixture(true, CanUseBodypartResult.CanUse);

		Assert.IsTrue(attack.UsableAttack(attacker.Object, target.Object, true,
			BuiltInCombatMoveType.NaturalWeaponAttack));
		body.Verify(x => x.CanUseBodypart(bodypart.Object), Times.Once);
	}

	[TestMethod]
	public void UsableAttack_PresentDisabledBodypart_ReturnsFalse()
	{
		var (attack, attacker, body, bodypart, target) = CreateFixture(true, CanUseBodypartResult.CantUsePartDamage);

		Assert.IsFalse(attack.UsableAttack(attacker.Object, target.Object, true,
			BuiltInCombatMoveType.NaturalWeaponAttack));
		body.Verify(x => x.CanUseBodypart(bodypart.Object), Times.Once);
	}

	[TestMethod]
	public void UsableAttack_AbsentBodypart_ReturnsFalseWithoutUseCheck()
	{
		var (attack, attacker, body, _, target) = CreateFixture(false, CanUseBodypartResult.CanUse);

		Assert.IsFalse(attack.UsableAttack(attacker.Object, target.Object, true,
			BuiltInCombatMoveType.NaturalWeaponAttack));
		body.Verify(x => x.CanUseBodypart(It.IsAny<IBodypart>()), Times.Never);
	}

	private static (NaturalAttack Attack, Mock<ICharacter> Attacker, Mock<IBody> Body,
		Mock<IBodypart> Bodypart, Mock<IPerceiver> Target) CreateFixture(bool partPresent,
		CanUseBodypartResult useResult)
	{
		var bodypart = new Mock<IBodypart>();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Bodyparts)
			.Returns(partPresent ? [bodypart.Object] : []);
		body.Setup(x => x.CanUseBodypart(bodypart.Object)).Returns(useResult);
		body.Setup(x => x.HeldItemsFor(bodypart.Object)).Returns([]);
		body.Setup(x => x.WieldedItemsFor(bodypart.Object)).Returns([]);

		var settings = new Mock<ICharacterCombatSettings>();
		settings.SetupGet(x => x.RequiredIntentions).Returns(CombatMoveIntentions.None);
		settings.SetupGet(x => x.ForbiddenIntentions).Returns(CombatMoveIntentions.None);

		var attacker = new Mock<ICharacter>();
		attacker.SetupGet(x => x.Body).Returns(body.Object);
		attacker.SetupGet(x => x.CombatSettings).Returns(settings.Object);
		attacker.SetupGet(x => x.PositionState).Returns(PositionStanding.Instance);

		var weaponAttack = new Mock<IWeaponAttack>();
		weaponAttack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.NaturalWeaponAttack);
		weaponAttack.SetupGet(x => x.Intentions).Returns(CombatMoveIntentions.Attack);
		weaponAttack.SetupGet(x => x.RequiredPositionStates).Returns([PositionStanding.Instance]);
		weaponAttack.SetupGet(x => x.UsabilityProg).Returns((IFutureProg)null!);

		return (new NaturalAttack
		{
			Attack = weaponAttack.Object,
			Bodypart = bodypart.Object,
			Quality = ItemQuality.Standard
		}, attacker, body, bodypart, new Mock<IPerceiver>());
	}
}
