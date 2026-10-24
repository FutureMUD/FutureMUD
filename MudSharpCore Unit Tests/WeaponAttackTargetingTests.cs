#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WeaponAttackTargetingTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MeleeDamage_RetainsExactWeaponOriginWithAndWithoutWeaponWear(bool weaponWear)
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool("WeaponsTakeDamageFromAttacks")).Returns(weaponWear);
		var penetration = new Mock<ICheck>();
		penetration.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), 0.0, TraitUseType.Practical,
			It.IsAny<(string Parameter, object value)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.MeleeWeaponPenetrateCheck, Outcome.Pass));
		world.Setup(x => x.GetCheck(CheckType.MeleeWeaponPenetrateCheck)).Returns(penetration.Object);
		var attacker = new Mock<ICharacter>(); attacker.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = Mock.Of<ICharacter>(); attacker.Setup(x => x.ColocatedWith(target)).Returns(true);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(31);
		item.Setup(x => x.EffectsOfType<IMagicWeaponEnhancementEffect>(It.IsAny<Predicate<IMagicWeaponEnhancementEffect>>()))
			.Returns(Array.Empty<IMagicWeaponEnhancementEffect>());
		var weapon = Mock.Of<IMeleeWeapon>(x => x.Parent == item.Object);
		ITraitExpression Constant(double value)
		{
			var expression = new Mock<ITraitExpression>();
			expression.Setup(x => x.EvaluateWith(attacker.Object, null, TraitBonusContext.ArmedDamageCalculation,
				It.IsAny<(string Name, object Value)[]>())).Returns(value);
			return expression.Object;
		}
		var damage = Constant(3); var pain = Constant(1); var stun = Constant(0);
		var profile = Mock.Of<IDamageProfile>(x => x.DamageType == DamageType.Burning &&
			x.DamageExpression == damage && x.PainExpression == pain && x.StunExpression == stun);
		var attack = Mock.Of<IWeaponAttack>(x => x.Profile == profile);
		var move = new MeleeWeaponAttack(attacker.Object, weapon, attack, target);
		var result = ((IDamage TargetDamage, IDamage SelfDamage))typeof(WeaponAttackMove)
			.GetMethod("GetDamagePlusSelfDamageForWeapon", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(move, [weapon, Outcome.Pass, OpposedOutcomeDegree.Minor, target, null!, Math.PI * 0.5])!;
		Assert.AreSame(item.Object, result.TargetDamage.ToolOrigin);
		Assert.AreSame(attacker.Object, result.TargetDamage.ActorOrigin);
		Assert.AreEqual(3.0, result.TargetDamage.DamageAmount, 0.000001);
		Assert.AreEqual(1.0, result.TargetDamage.PainAmount, 0.000001);
		Assert.AreEqual(0.0, result.TargetDamage.StunAmount, 0.000001);
		if (weaponWear) Assert.IsNotNull(result.SelfDamage);
		else Assert.IsNull(result.SelfDamage);
	}

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
