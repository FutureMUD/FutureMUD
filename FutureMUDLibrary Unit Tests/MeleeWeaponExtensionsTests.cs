#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MeleeWeaponExtensionsTests
{
	[TestMethod]
	public void HandednessForWeapon_WithAnotherMeleeWeapon_ReturnsDualWield()
	{
		var (weapon, character, body, item) = CreateWeapon();
		var offHandItem = new Mock<IGameItem>();
		offHandItem.Setup(x => x.IsItemType<IShield>()).Returns(false);
		offHandItem.Setup(x => x.IsItemType<IMeleeWeapon>()).Returns(true);
		body.SetupGet(x => x.WieldedItems).Returns([item.Object, offHandItem.Object]);

		Assert.AreEqual(AttackHandednessOptions.DualWieldOnly, weapon.Object.HandednessForWeapon(character.Object));
	}

	[TestMethod]
	public void HandednessForWeapon_WithShield_ReturnsOneHanded()
	{
		var (weapon, character, body, item) = CreateWeapon();
		var shieldItem = new Mock<IGameItem>();
		shieldItem.Setup(x => x.IsItemType<IShield>()).Returns(true);
		body.SetupGet(x => x.WieldedItems).Returns([item.Object, shieldItem.Object]);

		Assert.AreEqual(AttackHandednessOptions.OneHandedOnly, weapon.Object.HandednessForWeapon(character.Object));
	}

	[TestMethod]
	public void HandednessForWeapon_WithTwoHandedGrip_ReturnsTwoHanded()
	{
		var (weapon, character, body, _) = CreateWeapon(2);

		Assert.AreEqual(AttackHandednessOptions.TwoHandedOnly, weapon.Object.HandednessForWeapon(character.Object));
	}

	private static (Mock<IMeleeWeapon> Weapon, Mock<ICharacter> Character, Mock<IBody> Body, Mock<IGameItem> Item)
		CreateWeapon(int wieldedHands = 1)
	{
		var item = new Mock<IGameItem>();
		var weapon = new Mock<IMeleeWeapon>();
		weapon.SetupGet(x => x.Parent).Returns(item.Object);
		var body = new Mock<IBody>();
		body.Setup(x => x.WieldedHandCount(item.Object)).Returns(wieldedHands);
		body.SetupGet(x => x.WieldedItems).Returns([item.Object]);
		var character = new Mock<ICharacter>();
		character.SetupGet(x => x.Body).Returns(body.Object);
		return (weapon, character, body, item);
	}
}
