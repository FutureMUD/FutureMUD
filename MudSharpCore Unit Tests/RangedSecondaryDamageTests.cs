#nullable enable

using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using NativeExpression = MudSharp.Body.Traits.TraitExpression;

namespace MudSharp_Unit_Tests;

[TestClass]
public class RangedSecondaryDamageTests
{
	[TestMethod]
	public void RangedType_LoadEditAndSave_PreservesOptionalChannelFormulas()
	{
		var world = new Mock<IFuturemud>();
		var skill = new Mock<ITraitDefinition>(); skill.SetupGet(x => x.Id).Returns(11);
		var traits = new All<ITraitDefinition>(); traits.Add(skill.Object);
		world.SetupGet(x => x.Traits).Returns(traits);
		world.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
		var model = new MudSharp.Models.RangedWeaponTypes { Id = 101, Name = "test", FireTraitId = 11, OperateTraitId = 11,
			RangedWeaponType = (int)RangedWeaponType.Bow, AccuracyBonusExpression = "0", DamageBonusExpression = "10" };
		var native = new RangedWeaponTypeDefinition(model, world.Object);
		Assert.AreEqual("0", native.PainBonusExpression.OriginalFormulaText);
		Assert.AreEqual("0", native.StunBonusExpression.OriginalFormulaText);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		Assert.IsTrue(native.BuildingCommand(actor.Object, new StringStack("pain damage / 2")));
		Assert.IsTrue(native.BuildingCommand(actor.Object, new StringStack("stun damage + variable")));
		Assert.IsFalse(native.BuildingCommand(actor.Object, new StringStack("stun 1 + (")));
		typeof(RangedWeaponTypeDefinition).GetMethod("SaveToDBItem", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(native, [model]);
		var reloaded = new RangedWeaponTypeDefinition(model, world.Object);
		Assert.AreEqual("damage / 2", reloaded.PainBonusExpression.OriginalFormulaText);
		Assert.AreEqual("damage + variable", reloaded.StunBonusExpression.OriginalFormulaText);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void NativeProjectilePaths_AddConfiguredChannelsAndPreservePayloads(bool configured)
	{
		var world = new Mock<IFuturemud>();
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.Gameworld).Returns(world.Object);
		var target = new Mock<ICharacter>();
		var room = Mock.Of<IRoom>(); actor.SetupGet(x => x.Location).Returns(room); target.SetupGet(x => x.Location).Returns(room);
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Gameworld).Returns(world.Object);
		var payload = new Mock<IMagicProjectilePayloadEffect>();
		payload.Setup(x => x.AppliesToProjectileAttack(actor.Object, target.Object, item.Object)).Returns(true);
		payload.SetupGet(x => x.ProjectileDamageBonus).Returns(3);
		payload.SetupGet(x => x.ProjectilePainBonus).Returns(4);
		payload.SetupGet(x => x.ProjectileStunBonus).Returns(5);
		item.Setup(x => x.EffectsOfType<IMagicProjectilePayloadEffect>(It.IsAny<Predicate<IMagicProjectilePayloadEffect>>())).Returns([payload.Object]);
		NativeExpression Expression(string text) => new(text, world.Object);
		var ranged = new Mock<IRangedWeaponType>();
		ranged.SetupGet(x => x.DamageBonusExpression).Returns(Expression("10"));
		if (configured)
		{
			ranged.SetupGet(x => x.PainBonusExpression).Returns(Expression("damage/4"));
			ranged.SetupGet(x => x.StunBonusExpression).Returns(Expression("damage*2"));
		}
		var profile = new Mock<IDamageProfile>();
		profile.SetupGet(x => x.DamageExpression).Returns(Expression("dice(1,1)+19"));
		profile.SetupGet(x => x.PainExpression).Returns(Expression("damage/2"));
		profile.SetupGet(x => x.StunExpression).Returns(Expression("damage"));
		profile.SetupGet(x => x.DamageType).Returns(DamageType.Piercing);
		var ammunitionType = new Mock<IAmmunitionType>(); ammunitionType.SetupGet(x => x.DamageProfile).Returns(profile.Object);
		var ammoProto = TestObjectFactory.CreateUninitialized<AmmunitionGameItemComponentProto>(); ammoProto.AmmoType = ammunitionType.Object;
		var ammunition = new AmmunitionGameItemComponent(ammoProto, item.Object, temporary: true);
		var outcome = new OpposedOutcome(OpposedOutcomeDirection.Proponent, OpposedOutcomeDegree.None);
		var ammoDamage = (Damage)typeof(AmmunitionGameItemComponent).GetMethod("BuildDamage", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(ammunition, [actor.Object, target.Object, null, item.Object, ranged.Object, outcome, 1d])!;
		Assert.AreEqual(33d, ammoDamage.DamageAmount);
		Assert.AreEqual(configured ? 16.5 : 14, ammoDamage.PainAmount);
		Assert.AreEqual(configured ? 45d : 25, ammoDamage.StunAmount);

		var attack = new Mock<IWeaponAttack>(); attack.SetupGet(x => x.Profile).Returns(profile.Object); attack.SetupGet(x => x.Weighting).Returns(1);
		var melee = new Mock<IWeaponType>(); melee.SetupGet(x => x.Attacks).Returns([attack.Object]);
		var thrownProto = TestObjectFactory.CreateUninitialized<ThrownWeaponGameItemComponentProto>();
		thrownProto.RangedWeaponType = ranged.Object; thrownProto.MeleeWeaponType = melee.Object;
		var thrown = new ThrownWeaponGameItemComponent(thrownProto, item.Object, temporary: true);
		var throwDamage = (Damage)typeof(ThrownWeaponGameItemComponent).GetMethod("BuildThrownDamage", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(thrown, [actor.Object, target.Object, null, outcome, new[] { payload.Object }])!;
		Assert.AreEqual(13d, throwDamage.DamageAmount, "Thrown damage must not add the melee profile's damage again.");
		Assert.AreEqual(configured ? 6.5 : 4, throwDamage.PainAmount);
		Assert.AreEqual(configured ? 25d : 5, throwDamage.StunAmount);

		var packProto = TestObjectFactory.CreateUninitialized<PowerPackGameItemComponentProto>();
		var pack = new PowerPackGameItemComponent(packProto, item.Object, temporary: true);
		var beamDamage = (Damage)typeof(PowerPackGameItemComponent).GetMethod("BuildDamage", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(pack, [actor.Object, target.Object, null, ranged.Object, outcome, .5, .25])!;
		Assert.AreEqual(13d, beamDamage.DamageAmount);
		Assert.AreEqual(configured ? 13d : 10.5, beamDamage.PainAmount);
		Assert.AreEqual(configured ? 28.25 : 8.25, beamDamage.StunAmount);
	}
}
