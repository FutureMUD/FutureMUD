#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.RPG.Checks;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponAttackTargetingTests
{
	[TestMethod]
	public void MeleeWeaponAttack_AuthoredTargetShape_SelectsMatchingBodypart()
	{
		Mock<IBodypartShape> targetShape = new();
		Mock<IBodypartShape> otherShape = new();

		Mock<IBodypart> matchingPart = new();
		matchingPart.SetupGet(x => x.Shape).Returns(targetShape.Object);
		matchingPart.SetupGet(x => x.RelativeHitChance).Returns(1.0);

		Mock<IBodypart> otherPart = new();
		otherPart.SetupGet(x => x.Shape).Returns(otherShape.Object);
		otherPart.SetupGet(x => x.RelativeHitChance).Returns(1.0);

		Mock<IBody> targetBody = new();
		targetBody.SetupGet(x => x.Bodyparts).Returns(new[] { otherPart.Object, matchingPart.Object });

		Mock<ICharacter> target = new();
		target.SetupGet(x => x.Body).Returns(targetBody.Object);

		Mock<ICharacter> attacker = new();
		attacker.Setup(x => x.ColocatedWith(target.Object)).Returns(true);

		Mock<IWeaponAttack> attack = new();
		attack.SetupGet(x => x.BodypartShape).Returns(targetShape.Object);
		Mock<IMeleeWeapon> weapon = new();

		MeleeWeaponAttack move = new(attacker.Object, weapon.Object, attack.Object, target.Object);
		Mock<ICombatMove> defense = new();
		defense.SetupGet(x => x.Assailant).Returns(target.Object);

		typeof(WeaponAttackMove)
			.GetMethod("DetermineTargetBodypart", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(move, new object[] { defense.Object, Outcome.Pass });

		Assert.AreSame(matchingPart.Object, move.TargetBodypart);
	}
}
