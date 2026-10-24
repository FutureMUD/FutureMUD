#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Body.Traits.Subtypes;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Framework;
using MudSharp.RPG.Checks;
using MudSharp.Vehicles;

namespace MudSharp_Unit_Tests;

[TestClass]
public class VehiclePilotTests
{
	[TestMethod]
	[DataRow(20, 20, 60, true, 1, 0)]
	[DataRow(20, 60, 20, false, 2, 1)]
	[DataRow(60, 60, 60, true, 1, 0)]
	public void Resolve_ProficiencyAndRecovery_UseSourceSixtyPointScale(double proficiency, int first, int second,
		bool progresses, int rolls, int uses)
	{
		var f = new Fixture(proficiency);
		var supplied = new Queue<int>([first, second]);
		var called = 0;
		var selected = VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [], _ => true, (minimum, maximum) =>
		{
			Assert.AreEqual(1, minimum); Assert.AreEqual(60, maximum); called++; return supplied.Dequeue();
		});
		Assert.AreEqual(progresses, selected is not null);
		Assert.AreEqual(rolls, called);
		f.Trait.Verify(x => x.TraitUsed(f.Actor.Object, Outcome.Fail, Difficulty.Normal, TraitUseType.Practical,
			It.IsAny<IEnumerable<Tuple<string, double>>>()), Times.Exactly(uses));
	}

	[TestMethod]
	public void Resolve_TwoFailures_OnlyAdmitsOneVisibleSafeUsableHeading()
	{
		var f = new Fixture(20);
		var choices = new Queue<int>([60, 60, 1]);
		var selected = VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [f.Wrong.Object], _ => true,
			(_, _) => choices.Dequeue());
		Assert.AreSame(f.Wrong.Object, selected);
		foreach (var visible in new[] { true, false })
		{
			f.Body.Setup(x => x.CanSee(f.Origin.Object, f.Wrong.Object)).Returns(visible);
			choices = new Queue<int>([60, 60, 1]);
			Assert.IsNull(VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [f.Wrong.Object], _ => false,
				(_, _) => choices.Dequeue()));
		}
	}

	[TestMethod]
	public void Resolve_HostedController_UsesExteriorVehicleForExitTransition()
	{
		var f = new Fixture(20);
		var exterior = Mock.Of<IPerceiver>();
		f.Wrong.Setup(x => x.MovementTransition(f.Actor.Object)).Returns((RoomMovementTransition.NoViableTransition, RoomLayer.GroundLevel));
		f.Wrong.Setup(x => x.MovementTransition(exterior)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
		var rolls = new Queue<int>([60, 60, 1]);
		Assert.AreSame(f.Wrong.Object, VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [f.Wrong.Object], _ => true,
			(_, _) => rolls.Dequeue(), exterior));
	}

	[TestMethod]
	public void Resolve_MissingSkill_RefusesWithoutRollLearningOrAutomaticGrant()
	{
		var f = new Fixture(20);
		f.Actor.Setup(x => x.HasTrait(f.Definition.Object)).Returns(false);
		Assert.IsNull(VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [], _ => true,
			(_, _) => throw new AssertFailedException("Missing skills must not roll.")));
		f.Actor.Verify(x => x.AddTrait(It.IsAny<ITraitDefinition>(), It.IsAny<double>()), Times.Never);
		f.Actor.Verify(x => x.GetTrait(It.IsAny<ITraitDefinition>()), Times.Never);
	}

	[TestMethod]
	public void Resolve_Disabled_PreservesExistingDrivingWithoutTraitLookup()
	{
		var f = new Fixture(20);
		f.World.Setup(x => x.GetStaticBool(VehiclePilot.EnabledSetting)).Returns(false);
		Assert.AreSame(f.Intended.Object, VehiclePilot.Resolve(f.Actor.Object, f.Intended.Object, [], _ => true));
		f.World.Verify(x => x.GetStaticLong(VehiclePilot.TraitSetting), Times.Never);
	}

	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly Mock<ICharacter> Actor = new();
		internal readonly Mock<IBody> Body = new();
		internal readonly Mock<ISkillDefinition> Definition = new();
		internal readonly Mock<ITrait> Trait = new();
		internal readonly Mock<IRoom> Origin = new();
		internal readonly Mock<IRoomExit> Intended = new();
		internal readonly Mock<IRoomExit> Wrong = new();
		internal Fixture(double proficiency)
		{
			World.Setup(x => x.GetStaticBool(VehiclePilot.EnabledSetting)).Returns(true);
			World.Setup(x => x.GetStaticLong(VehiclePilot.TraitSetting)).Returns(400);
			Definition.SetupGet(x => x.Id).Returns(400);
			World.SetupGet(x => x.Traits).Returns(new All<ITraitDefinition> { Definition.Object });
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.Setup(x => x.HasTrait(Definition.Object)).Returns(true);
			Actor.Setup(x => x.GetTrait(Definition.Object)).Returns(Trait.Object);
			Actor.Setup(x => x.TraitValue(Definition.Object, TraitBonusContext.None)).Returns(proficiency);
			Intended.SetupGet(x => x.Origin).Returns(Origin.Object);
			Intended.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.North);
			Wrong.SetupGet(x => x.Origin).Returns(Origin.Object);
			Wrong.SetupGet(x => x.Destination).Returns(Mock.Of<IRoom>());
			Wrong.SetupGet(x => x.OutboundDirection).Returns(CardinalDirection.NorthEast);
			Wrong.Setup(x => x.MovementTransition(Actor.Object)).Returns((RoomMovementTransition.GroundToGround, RoomLayer.GroundLevel));
			Body.Setup(x => x.CanSee(Origin.Object, Wrong.Object)).Returns(true);
		}
	}
}
