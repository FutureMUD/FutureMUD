#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using RuntimeCharacter = MudSharp.Character.Character;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MovementStaminaMeritTests
{
	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly Mock<ICharacter> Actor = new();
		internal readonly List<IMerit> Merits = new();

		internal Fixture()
		{
			var progs = new All<IFutureProg>();
			foreach (var applies in new[] { false, true })
			{
				var prog = new Mock<IFutureProg>();
				prog.SetupGet(x => x.Id).Returns(applies ? 1 : 2);
				prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(applies);
				progs.Add(prog.Object);
			}
			World.SetupGet(x => x.FutureProgs).Returns(progs);
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Actor.SetupGet(x => x.Merits).Returns(Merits);
			MovementStaminaMerit.RegisterMeritInitialiser();
			SwimmingStaminaMerit.RegisterMeritInitialiser();
			FlyingStaminaMerit.RegisterMeritInitialiser();
		}

		internal IMerit Merit(long id, string type, double? multiplier, bool applies = true,
			MovementType modes = MovementType.All)
		{
			var definition = new XElement("Merit", new XAttribute("movetypes", (int)modes), new XElement("ApplicabilityProg", applies ? 1 : 2),
				new XElement("ChargenAvailableProg", 2), new XElement("DescriptionText", "@ move|moves efficiently"),
				new XElement("ChargenBlurb", "Movement physiology"));
			if (multiplier.HasValue) definition.SetAttributeValue("multiplier", multiplier.Value);
			return MeritFactory.LoadMerit(new MudSharp.Models.Merit
			{
				Id = id, Name = "Movement test " + id, Type = type, MeritScope = (int)MeritScope.Body,
				Definition = definition.ToString()
			}, World.Object);
		}
	}

	[TestMethod]
	public void Multiplier_UsesActualPostureAndRespectsApplicabilityAndFloatingMode()
	{
		var fixture = new Fixture();
		fixture.Merits.Add(fixture.Merit(10, "Movement Stamina Multiplier", 0.5, modes: MovementType.Upright));
		fixture.Merits.Add(fixture.Merit(11, "Movement Stamina Multiplier", 0.1, applies: false));
		Assert.AreEqual(0.5, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionStanding.Instance));
		Assert.AreEqual(1.0, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionProne.Instance));
		fixture.Merits.Add(fixture.Merit(12, "Movement Stamina Multiplier", 0.25, modes: MovementType.Floating));
		Assert.AreEqual(0.25, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionFloatingInZeroGravity.Instance));
		var misleadingSpeed = Mock.Of<IMoveSpeed>(x => x.Position == PositionStanding.Instance);
		Assert.AreEqual(1.0, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionProne.Instance, misleadingSpeed),
			"The charged posture takes precedence over a speed belonging to a different posture.");
	}

	[TestMethod]
	public void Multiplier_StacksDistinctApplicableGenericAndSpecificMeritsExactlyOnce()
	{
		var fixture = new Fixture();
		var general = fixture.Merit(10, "Movement Stamina Multiplier", 0.5);
		var swimming = fixture.Merit(11, "Swimming Stamina Multiplier", 0.8);
		var flying = fixture.Merit(12, "Flying Stamina Multiplier", 0.25);
		fixture.Merits.AddRange(new[] { general, general, swimming, swimming, flying,
			fixture.Merit(13, "Swimming Stamina Multiplier", 0.1, applies: false),
			fixture.Merit(14, "Flying Stamina Multiplier", 0.1, applies: false) });
		Assert.AreEqual(0.4, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionSwimming.Instance), 0.000001);
		Assert.AreEqual(0.125, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionFlying.Instance), 0.000001);
		Assert.AreEqual(0.5, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionClimbing.Instance), 0.000001);
	}

	[TestMethod]
	public void GenericMerit_LoadDefaultsAndInvalidValuesAreNeutralButDeliberateZeroSurvives()
	{
		var fixture = new Fixture();
		foreach (var multiplier in new double?[] { null, -1, double.NaN, double.PositiveInfinity })
		{
			fixture.Merits.Clear();
			fixture.Merits.Add(fixture.Merit(10, "Movement Stamina Multiplier", multiplier));
			Assert.AreEqual(1.0, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionStanding.Instance));
		}
		fixture.Merits.Clear();
		fixture.Merits.Add(fixture.Merit(10, "Movement Stamina Multiplier", 0));
		Assert.AreEqual(0.0, RuntimeCharacter.MovementStaminaMeritMultiplier(fixture.Actor.Object, PositionStanding.Instance));
	}

	[TestMethod]
	public void NativeGroundCost_AppliesConfiguredMeritAndLeavesOrdinaryCalculationUnchanged()
	{
		var fixture = new Fixture();
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Merits).Returns(fixture.Merits);
		body.SetupGet(x => x.EncumbrancePercentage).Returns(0.4);
		body.SetupGet(x => x.CurrentSpeeds).Returns(new Dictionary<IPositionState, IMoveSpeed>());
		fixture.World.Setup(x => x.GetStaticDouble("StaminaMultiplierPerEncumbrancePercentage")).Returns(0.5);
		var terrain = Mock.Of<ITerrain>(x => x.StaminaCost == 2.0);
		var room = new Mock<IRoom>();
		room.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(terrain);
		var mover = NativeMover.Create(fixture.World.Object, body.Object, room.Object);
		var speed = Mock.Of<IMoveSpeed>(x => x.Position == PositionStanding.Instance && x.StaminaMultiplier == 3.0);
		Assert.AreEqual(10.8, mover.GroundCost(speed), 0.000001);
		fixture.Merits.Add(fixture.Merit(10, "Movement Stamina Multiplier", 0.5, modes: MovementType.Upright));
		Assert.AreEqual(5.4, mover.GroundCost(speed), 0.000001);
	}

	private sealed class NativeMover : RuntimeCharacter
	{
		private NativeMover() : base(null!, null!, true) { }

		internal static NativeMover Create(IFuturemud world, IBody body, IRoom room)
		{
			var mover = (NativeMover)RuntimeHelpers.GetUninitializedObject(typeof(NativeMover));
			typeof(LateKeywordedInitialisingItem).GetField("<Gameworld>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(mover, world);
			typeof(RuntimeCharacter).GetField("_merits", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(mover, new List<IMerit>());
			mover.Body = body;
			mover.Location = room;
			return mover;
		}

		internal double GroundCost(IMoveSpeed speed) => StaminaForMovement(PositionStanding.Instance, speed, 1.5, false);
	}
}
