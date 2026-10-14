using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Movement;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp_Unit_Tests;

public partial class ArmageddonDirectDamageTests
{
	[TestMethod]
	public void WindHammer_PaidNativeHitPrecedesExactlyOneSelectedExitStep()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		var woundCountAtMovement = 0;
		f.Victim.Setup(x => x.Teleport(exit.Destination.Object, RoomLayer.GroundLevel,
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
			.Callback(() => woundCountAtMovement = f.Body.Wounds.Count());
		var result = f.F.Service.Cast(WindIntent(f, 2));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(100 - new ControlledSpellEfficiency(33, 1).Cost(7, 2),
			f.F.Balances[f.F.Resources[1]], 0.000001);
		Assert.AreEqual(1, woundCountAtMovement);
		Assert.AreEqual(10.0, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		f.Victim.Verify(x => x.Teleport(exit.Destination.Object, RoomLayer.GroundLevel,
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
	}

	[TestMethod]
	public void WindHammer_ZeroHarmNeverPushesEvenWhenExitRemainsLegal()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		((WindHammerEffect)f.Spell.SpellEffects.Single()).Damage.DamageExpression = new TraitExpression("0", f.F.World.Object);
		var result = f.F.Service.Cast(WindIntent(f, 1));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(67.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any());
		Assert.AreEqual(0, f.F.Samples);
		f.Victim.Verify(x => x.Teleport(exit.Destination.Object, RoomLayer.GroundLevel,
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_PostHitMovementRefusalKeepsDamageButNeverRelocates()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		f.Victim.Setup(x => x.CanMove(exit.Exit.Object,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement)).Returns(() => f.Body.Wounds.Any()
			? new CanMoveResponse { Result = false, ErrorMessage = "blocked after hit" }
			: CanMoveResponse.True);
		var result = f.F.Service.Cast(WindIntent(f, 1));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(5.0, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		f.Victim.Verify(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_PostHitExitRemovalKeepsDamageButNeverRelocates()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		var attached = true;
		Mock.Get(f.F.Actor.Object.Location).Setup(x => x.ExitsFor(f.Victim.Object, false))
			.Returns(() => attached ? [exit.Exit.Object] : []);
		f.Victim.Setup(x => x.ProcessPassiveWound(It.IsAny<IWound>())).Callback<IWound>(w =>
		{
			f.Body.ProcessPassiveWound(w);
			attached = false;
		});
		var result = f.F.Service.Cast(WindIntent(f, 1));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(5.0, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		f.Victim.Verify(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_DestinationRetargetDuringMovementAdmissionNeverRelocates()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		var redirected = new Mock<IRoom>();
		f.Victim.Setup(x => x.CanCross(exit.Exit.Object)).Returns(() =>
		{
			if (f.Body.Wounds.Any()) exit.Exit.SetupGet(x => x.Destination).Returns(redirected.Object);
			return (true, null!);
		});
		var result = f.F.Service.Cast(WindIntent(f, 1));
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(5.0, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		f.Victim.Verify(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_PostHitAuthorityRevocationNeverRelocates()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		AdmitExit(f);
		var authorized = true;
		f.Victim.Setup(x => x.ProcessPassiveWound(It.IsAny<IWound>())).Callback<IWound>(w =>
		{
			f.Body.ProcessPassiveWound(w);
			authorized = false;
		});
		using var scope = CommandExecutionScope.EnterDispatch(
			CommandExecutionAuthority.Prepare(f.F.Actor.Object, f.F.Actor.Object, "Wind Hammer", () => authorized),
			f.F.Actor.Object);
		f.F.Service.Cast(WindIntent(f, 1));
		Assert.IsFalse(authorized);
		Assert.AreEqual(5.0, f.Body.Wounds.Single().CurrentDamage, 0.000001);
		f.Victim.Verify(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_ExitDriftBeforePaymentRefusesWithoutDamageOrMovement()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var exit = AdmitExit(f);
		f.F.Checkpoint = stage =>
		{
			if (stage == "BeforePayment") exit.Exit.SetupGet(x => x.Destination).Returns(new Mock<IRoom>().Object);
		};
		var result = f.F.Service.Cast(WindIntent(f, 1));
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.F.Balances[f.F.Resources[1]]);
		Assert.IsFalse(f.Body.Wounds.Any());
		f.Victim.Verify(x => x.Teleport(It.IsAny<IRoom>(), It.IsAny<RoomLayer>(),
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
	}

	[TestMethod]
	public void WindHammer_ColdDefinitionBindsSelectedGradeAndDisablesStoredCarriers()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var effect = (WindHammerEffect)f.Spell.SpellEffects.Single();
		Assert.AreEqual(DamageType.Shockwave, effect.Damage.DamageType);
		Assert.IsTrue(effect.Damage.IdentitySafeDamage);
		Assert.AreEqual("5*grade", effect.Damage.DamageExpression.OriginalFormulaText);
		Assert.AreEqual(0, f.Spell.GradeConfigurationErrors().Count,
			string.Join("\n", f.Spell.GradeConfigurationErrors()));
		var copy = f.Spell.CastingCopy(f.F.Actor.Object, f.F.Traits[0], 2,
			SpellPower.ExtremelyWeak, Difficulty.Normal);
		Assert.AreEqual("CastingExpression",
			((WindHammerEffect)copy.SpellEffects.Single()).Damage.DamageExpression.GetType().Name);
		var cold = new MagicSpell(f.Spell.SnapshotModel(), f.F.World.Object);
		Assert.IsTrue(((WindHammerEffect)cold.SpellEffects.Single()).Damage.IdentitySafeDamage);
		Assert.IsTrue(MudSharp.Magic.Vancian.ScrollSpellCompatibility.Errors(f.Spell, false)
			.Any(x => x.Contains("linked native hit", StringComparison.Ordinal)));
	}

	[TestMethod]
	public void WindHammer_BuilderCanEditDamageButCannotDisableFrozenHitPolicy()
	{
		var f = new Fixture(ArmageddonWindHammerSpellContent.Key);
		var effect = (WindHammerEffect)f.Spell.SpellEffects.Single();
		Assert.IsFalse(effect.BuildingCommand(f.F.Actor.Object, new StringStack("identitysafe")));
		Assert.IsTrue(effect.Damage.IdentitySafeDamage);
		Assert.IsTrue(effect.BuildingCommand(f.F.Actor.Object, new StringStack("type crushing")));
		Assert.AreEqual(DamageType.Crushing, effect.Damage.DamageType);
		Assert.IsTrue(effect.Damage.IdentitySafeDamage);
	}

	private static MagicCastingIntent WindIntent(Fixture f, int grade) =>
		new(f.F.Actor.Object, f.F.Earth.Id, 1, grade, false, "target north", OriginId: Guid.NewGuid());

	private static (Mock<IRoomExit> Exit, Mock<IRoom> Destination) AdmitExit(Fixture f)
	{
		var origin = f.F.Actor.Object.Location;
		var destination = new Mock<IRoom>();
		var nativeExit = new Mock<IExit>();
		nativeExit.SetupGet(x => x.Id).Returns(1);
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.Exit).Returns(nativeExit.Object);
		exit.SetupGet(x => x.Origin).Returns(origin);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.Setup(x => x.WhichLayersExitAppears()).Returns([RoomLayer.GroundLevel]);
		exit.Setup(x => x.MovementTransition(f.Victim.Object))
			.Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		Mock.Get(origin).Setup(x => x.GetExitKeyword("north", f.F.Actor.Object)).Returns(exit.Object);
		Mock.Get(origin).Setup(x => x.ExitsFor(f.Victim.Object, false)).Returns([exit.Object]);
		f.F.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		f.Victim.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		f.Victim.Setup(x => x.CanMove(exit.Object,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement)).Returns(CanMoveResponse.True);
		f.Victim.Setup(x => x.CanCross(exit.Object)).Returns((true, null!));
		return (exit, destination);
	}
}
