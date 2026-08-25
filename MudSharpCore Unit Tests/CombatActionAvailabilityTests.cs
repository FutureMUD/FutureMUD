#nullable enable

using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Climate;
using MudSharp.Combat;
using MudSharp.Combat.Moves;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Health.Strategies;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatActionAvailabilityTests
{
	[TestMethod]
	public void CanBehemothCharge_RequiresUnMountedLargerAttackerWithChargeAttack()
	{
		var position = PositionStanding.Instance;
		var bodypart = Mock.Of<IBodypart>();
		var weaponAttack = new Mock<IWeaponAttack>();
		weaponAttack.SetupGet(x => x.MoveType).Returns(BuiltInCombatMoveType.BehemothChargeAttack);
		weaponAttack.SetupGet(x => x.RequiredPositionStates).Returns([position]);
		var attack = new Mock<INaturalAttack>();
		attack.SetupGet(x => x.Attack).Returns(weaponAttack.Object);
		attack.SetupGet(x => x.Bodypart).Returns(bodypart);
		var race = new Mock<IRace>();
		race.SetupGet(x => x.NaturalWeaponAttacks).Returns([attack.Object]);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Bodyparts).Returns([bodypart]);
		body.Setup(x => x.CanUseBodypart(bodypart)).Returns(CanUseBodypartResult.CanUse);
		body.Setup(x => x.HeldItemsFor(bodypart)).Returns([]);
		body.Setup(x => x.WieldedItemsFor(bodypart)).Returns([]);
		var assailant = new Mock<ICharacter>();
		assailant.SetupGet(x => x.Race).Returns(race.Object);
		assailant.SetupGet(x => x.Body).Returns(body.Object);
		assailant.SetupGet(x => x.PositionState).Returns(position);
		assailant.Setup(x => x.CurrentContextualSize(SizeContext.GrappleAttack)).Returns(SizeCategory.VeryLarge);
		var target = new Mock<ICharacter>();
		target.Setup(x => x.CurrentContextualSize(SizeContext.GrappleDefense)).Returns(SizeCategory.Normal);
		assailant.Setup(x => x.ColocatedWith(target.Object)).Returns(true);

		Assert.IsTrue(ChargeToMeleeMove.CanBehemothCharge(assailant.Object, target.Object));

		target.Setup(x => x.CurrentContextualSize(SizeContext.GrappleDefense)).Returns(SizeCategory.VeryLarge);
		Assert.IsFalse(ChargeToMeleeMove.CanBehemothCharge(assailant.Object, target.Object));

		var mount = new Mock<ICharacter>();
		mount.Setup(x => x.IsPrimaryRider(assailant.Object)).Returns(true);
		assailant.SetupGet(x => x.RidingMount).Returns(mount.Object);
		target.Setup(x => x.CurrentContextualSize(SizeContext.GrappleDefense)).Returns(SizeCategory.Normal);
		Assert.IsFalse(ChargeToMeleeMove.CanBehemothCharge(assailant.Object, target.Object));

		assailant.SetupGet(x => x.RidingMount).Returns((ICharacter?)null);
		assailant.SetupGet(x => x.MeleeRange).Returns(true);
		Assert.IsFalse(ChargeToMeleeMove.CanBehemothCharge(assailant.Object, target.Object));
	}

	[TestMethod]
	public void BehemothImpact_KnockdownThreshold_IsTwoSizeCategoriesOrMajorSuccess()
	{
		Assert.IsFalse(MountedImpactNaturalAttackMove.ShouldKnockDown(1, 2, false, false));
		Assert.IsTrue(MountedImpactNaturalAttackMove.ShouldKnockDown(2, 1, false, false));
		Assert.IsTrue(MountedImpactNaturalAttackMove.ShouldKnockDown(1, 3, false, false));
		Assert.IsTrue(MountedImpactNaturalAttackMove.ShouldKnockDown(1, 1, true, false));
		Assert.IsTrue(MountedImpactNaturalAttackMove.ShouldKnockDown(1, 1, false, true));
	}

	[TestMethod]
	public void ApplyPainTolerance_UsesRaceMultiplierExactlyOnce()
	{
		var race = new Mock<IRace>();
		race.SetupGet(x => x.PainToleranceMultiplier).Returns(1.75);

		Assert.AreEqual(175.0, ComplexLivingHealthStrategy.ApplyPainTolerance(100.0, race.Object), 0.0001);
	}

	[TestMethod]
	public void GetMountedWeaponAttack_MountedWithoutChargeWeapon_ReturnsNull()
	{
		var body = new Mock<IBody>();
		body.SetupGet(x => x.WieldedItems).Returns([]);
		var assailant = new Mock<ICharacter>();
		var mount = new Mock<ICharacter>();
		mount.Setup(x => x.IsPrimaryRider(assailant.Object)).Returns(true);
		assailant.SetupGet(x => x.RidingMount).Returns(mount.Object);
		assailant.SetupGet(x => x.Body).Returns(body.Object);
		var move = new ChargeToMeleeMove { Assailant = assailant.Object };
		var method = typeof(ChargeToMeleeMove).GetMethod("GetMountedWeaponAttack",
			BindingFlags.Instance | BindingFlags.NonPublic)!;

		var result = method.Invoke(move, [Mock.Of<ICharacter>()]);

		Assert.IsNull(result);
	}

	[TestMethod]
	public void CanFire_MatchlockWithoutCurrentWeather_ReturnsTrue()
	{
		var prototype = (MusketGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(
			typeof(MusketGameItemComponentProto));
		typeof(MusketGameItemComponentProto)
			.GetProperty(nameof(MusketGameItemComponentProto.IgnitionFamily))!
			.SetValue(prototype, MusketIgnitionFamily.Matchlock);
		var parent = new Mock<IGameItem>();
		var musket = (MusketGameItemComponent)prototype.CreateNew(parent.Object, temporary: true);
		musket.LoadStage = 4;
		musket.IsReadied = true;
		typeof(MusketGameItemComponent)
			.GetProperty(nameof(MusketGameItemComponent.MatchLit))!
			.SetValue(musket, true);
		var location = new Mock<ICell>();
		location.SetupGet(x => x.Atmosphere).Returns(Mock.Of<IGas>());
		location.Setup(x => x.CurrentWeather(It.IsAny<ICharacter>())).Returns((IWeatherEvent)null!);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Location).Returns(location.Object);

		Assert.AreEqual(MusketIgnitionFamily.Matchlock, prototype.IgnitionFamily);
		Assert.IsTrue(musket.ReadyToFire);
		Assert.IsTrue(musket.MatchLit);
		Assert.IsTrue(musket.CanFire(actor.Object, Mock.Of<IPerceivable>()));
	}
}
