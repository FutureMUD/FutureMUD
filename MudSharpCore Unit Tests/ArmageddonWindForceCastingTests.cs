#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.Movement;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWindForceCastingTests
{
	[TestMethod]
	public void GustHands_CastingCopyBindsStepsToSelectedGradeWithoutChangingDefinition()
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonWindForceSpellContent.GustHandsKey);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
		for (var grade = 1; grade <= 7; grade++)
		{
			var power = (SpellPower)((int)SpellPower.ExtremelyWeak + 7 - grade);
			var copy = spell.CastingCopy(fixture.Actor.Object, fixture.Traits[0], grade, power, Difficulty.Normal);
			Assert.AreEqual(grade, ((ForcedPathMovementEffect)copy.SpellEffects.Single()).Steps);
		}
		Assert.AreEqual(1, ((ForcedPathMovementEffect)spell.SpellEffects.Single()).Steps);
	}

	[TestMethod]
	[DataRow("0")]
	[DataRow("1.5")]
	[DataRow("101")]
	public void GustHands_InvalidStepExpressionRefusesOnDetachedCopy(string formula)
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonWindForceSpellContent.GustHandsKey);
		var row = spell.SnapshotModel();
		var definition = XElement.Parse(row.Definition);
		definition.Element("ControlledPower")!.Element("ScalarBindings")!.Elements().Single().Value = formula;
		row.Definition = definition.ToString();
		var edited = new MagicSpell(row, fixture.World.Object);
		Assert.ThrowsExactly<InvalidOperationException>(() => edited.CastingCopy(
			fixture.Actor.Object, fixture.Traits[0], 2, SpellPower.ExtremelyWeak, Difficulty.Normal));
	}

	[TestMethod]
	public void DrivingGust_CastingCopyRetainsOneStepAtEveryGrade()
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonWindForceSpellContent.DrivingGustKey);
		Assert.AreEqual(0, spell.GradeConfigurationErrors().Count, string.Join("\n", spell.GradeConfigurationErrors()));
		foreach (var grade in new[] { 1, 4, 7 })
		{
			var copy = spell.CastingCopy(fixture.Actor.Object, fixture.Traits[0], grade, SpellPower.ExtremelyStrong, Difficulty.Normal);
			Assert.AreEqual(1, ((ForcedPathMovementEffect)copy.SpellEffects.Single()).Steps);
		}
	}

	[TestMethod]
	public void WindForce_MissingExitIsRejectedWithoutReportingAnAppliedStep()
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonWindForceSpellContent.GustHandsKey);
		var effect = (IMagicSpellEffectOperation)spell.SpellEffects.Single();
		var report = effect.Apply(fixture.Actor.Object, fixture.Actor.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, []);
		Assert.AreEqual(MagicEffectOperationStatus.Rejected, report.Status);
		Assert.IsNull(report.Effect);
	}

	[TestMethod]
	public void WindForce_NativeTeleportRefusalDoesNotReportMovementOrRunFallPhysics()
	{
		var fixture = new MagicCastingFixture();
		var spell = Load(fixture, ArmageddonWindForceSpellContent.DrivingGustKey);
		var effect = (IMagicSpellEffectOperation)spell.SpellEffects.Single();
		var origin = fixture.Actor.Object.Location;
		var destination = new Mock<IRoom>();
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.Origin).Returns(origin);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.Unknown);
		exit.Setup(x => x.WhichLayersExitAppears()).Returns([RoomLayer.GroundLevel]);
		exit.Setup(x => x.MovementTransition(fixture.Actor.Object))
			.Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		fixture.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		fixture.Actor.Setup(x => x.CanMove(exit.Object,
			CanMoveFlags.IgnoreWhetherExitCanBeCrossed | CanMoveFlags.IgnoreCancellableActionBlockers |
			CanMoveFlags.IgnoreSafeMovement)).Returns(CanMoveResponse.True);
		fixture.Actor.Setup(x => x.CanCross(exit.Object)).Returns((true, null!));

		var report = effect.Apply(fixture.Actor.Object, fixture.Actor.Object, OpposedOutcomeDegree.None,
			SpellPower.ExtremelyWeak, null!, [new SpellAdditionalParameter { ParameterName = "exit", Item = exit.Object }]);

		Assert.AreEqual(MagicEffectOperationStatus.NoChange, report.Status);
		fixture.Actor.Verify(x => x.Teleport(destination.Object, RoomLayer.GroundLevel,
			It.IsAny<bool>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
		fixture.Actor.Verify(x => x.FallToGround(), Times.Never);
	}

	private static MagicSpell Load(MagicCastingFixture fixture, string key)
	{
		var content = ArmageddonWindForceSpellContent.Create().Single(x => x.Content.Key == key).Content;
		fixture.Expressions.Clear();
		var cost = content.CostRow(1); cost.Id = 1;
		var duration = content.DurationRow(1); duration.Id = 2;
		fixture.Expressions.Add(new TraitExpression(cost, fixture.World.Object));
		fixture.Expressions.Add(new TraitExpression(duration, fixture.World.Object));
		var row = content.SpellRow(1, 1, 1); row.Id = 1; row.EffectDurationExpressionId = 2;
		row.Definition = content.BuildDefinition(10, 1, 0).ToString();
		return new MagicSpell(row, fixture.World.Object);
	}
}
