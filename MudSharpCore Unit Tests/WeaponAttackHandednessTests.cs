#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using System.Runtime.CompilerServices;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponAttackHandednessTests
{
    [TestMethod]
    public void UsableAttack_OneHandedWeaponWithSeparateShield_ExposesNormalAndSwordAndBoardAttacks()
    {
        IGameItem weapon = new Mock<IGameItem>().Object;
        Mock<IGameItem> shield = new();
        shield.Setup(x => x.IsItemType<IShield>()).Returns(true);

        Mock<IBody> body = new();
        body.Setup(x => x.WieldedItems).Returns([weapon, shield.Object]);

        Mock<ICharacterCombatSettings> combatSettings = new();
        combatSettings.Setup(x => x.RequiredIntentions).Returns(CombatMoveIntentions.None);
        combatSettings.Setup(x => x.ForbiddenIntentions).Returns(CombatMoveIntentions.None);

        Mock<ICharacter> attacker = new();
        attacker.Setup(x => x.Body).Returns(body.Object);
        attacker.Setup(x => x.CombatSettings).Returns(combatSettings.Object);

        IPerceiver target = new Mock<IPerceiver>().Object;
        WeaponAttack ordinaryOneHanded = CreateWeaponAttack(AttackHandednessOptions.OneHandedOnly);
        WeaponAttack swordAndBoard = CreateWeaponAttack(AttackHandednessOptions.SwordAndBoardOnly);

        Assert.IsTrue(ordinaryOneHanded.UsableAttack(attacker.Object, weapon, target,
            AttackHandednessOptions.OneHandedOnly, true, BuiltInCombatMoveType.UseWeaponAttack));
        Assert.IsTrue(swordAndBoard.UsableAttack(attacker.Object, weapon, target,
            AttackHandednessOptions.OneHandedOnly, true, BuiltInCombatMoveType.UseWeaponAttack));
    }

    [TestMethod]
    public void UsableAttack_OneHandedWeaponWithoutSeparateShield_DoesNotExposeSwordAndBoardAttacks()
    {
        IGameItem weapon = new Mock<IGameItem>().Object;

        Mock<IBody> body = new();
        body.Setup(x => x.WieldedItems).Returns([weapon]);

        Mock<ICharacterCombatSettings> combatSettings = new();
        combatSettings.Setup(x => x.RequiredIntentions).Returns(CombatMoveIntentions.None);
        combatSettings.Setup(x => x.ForbiddenIntentions).Returns(CombatMoveIntentions.None);

        Mock<ICharacter> attacker = new();
        attacker.Setup(x => x.Body).Returns(body.Object);
        attacker.Setup(x => x.CombatSettings).Returns(combatSettings.Object);

        IPerceiver target = new Mock<IPerceiver>().Object;
        WeaponAttack swordAndBoard = CreateWeaponAttack(AttackHandednessOptions.SwordAndBoardOnly);

        Assert.IsFalse(swordAndBoard.UsableAttack(attacker.Object, weapon, target,
            AttackHandednessOptions.OneHandedOnly, true, BuiltInCombatMoveType.UseWeaponAttack));
    }

    private static WeaponAttack CreateWeaponAttack(AttackHandednessOptions handedness)
    {
        WeaponAttack attack = (WeaponAttack)RuntimeHelpers.GetUninitializedObject(typeof(WeaponAttack));
        attack.MoveType = BuiltInCombatMoveType.UseWeaponAttack;
        attack.Intentions = CombatMoveIntentions.Attack;
        attack.HandednessOptions = handedness;
        return attack;
    }
}
