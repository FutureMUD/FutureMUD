#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Implementations;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.Movement;
using System.Linq;
using MoveSpeed = MudSharp.Movement.MoveSpeed;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyPrototypeMovementSpeedTests
{
	[ClassInitialize]
	public static void Initialise(TestContext _)
	{
		PositionState.SetupPositions();
	}

	[TestMethod]
	public void Constructor_LocalSpeedsReplaceInheritedSpeedsForTheSamePosition()
	{
		var (prototype, parent, inheritedBodypart) = LoadDerivedPrototype();

		CollectionAssert.AreEqual(
			new[] { "shamble" },
			prototype.Speeds
				.Where(x => x.Position == PositionStanding.Instance)
				.Select(x => x.Name)
				.ToArray());
		Assert.AreEqual("shamble", prototype.Speeds
			.Where(x => x.Position == PositionStanding.Instance)
			.OrderBy(x => x.Multiplier)
			.First().Name);
		CollectionAssert.AreEquivalent(
			new[] { "crawl", "float" },
			prototype.Speeds
				.Where(x => x.Position != PositionStanding.Instance)
				.Select(x => x.Name)
				.ToArray());
		Assert.IsTrue(prototype.AllBodyparts.Contains(inheritedBodypart));
		CollectionAssert.AreEquivalent(
			new[] { "sprint", "walk", "crawl", "float" },
			parent.Speeds.Select(x => x.Name).ToArray());
	}

	[TestMethod]
	public void Constructor_ReloadPreservesPositionSpecificSpeedReplacement()
	{
		var first = LoadDerivedPrototype().Prototype;
		var reloaded = LoadDerivedPrototype().Prototype;

		CollectionAssert.AreEqual(
			first.Speeds.Select(x => x.Id).OrderBy(x => x).ToArray(),
			reloaded.Speeds.Select(x => x.Id).OrderBy(x => x).ToArray());
		CollectionAssert.AreEqual(
			new long[] { 10 },
			reloaded.Speeds
				.Where(x => x.Position == PositionStanding.Instance)
				.Select(x => x.Id)
				.ToArray());
	}

	private static (BodyPrototype Prototype, IBodyPrototype Parent, IBodypart InheritedBodypart) LoadDerivedPrototype()
	{
		var parentSpeeds = new All<IMoveSpeed>();
		parentSpeeds.Add(Speed(1, "sprint", PositionStanding.Instance.Id, 0.33));
		parentSpeeds.Add(Speed(2, "walk", PositionStanding.Instance.Id, 1.0));
		parentSpeeds.Add(Speed(3, "crawl", PositionProne.Instance.Id, 5.0));
		parentSpeeds.Add(Speed(4, "float", PositionFloatingInZeroGravity.Instance.Id, 1.0));

		var inheritedBodypart = new Mock<IBodypart>().Object;
		var parent = new Mock<IBodyPrototype>();
		parent.SetupGet(x => x.Id).Returns(1);
		parent.SetupGet(x => x.Name).Returns("Parent");
		parent.SetupGet(x => x.FrameworkItemType).Returns("BodyPrototype");
		parent.SetupGet(x => x.Speeds).Returns(parentSpeeds);
		parent.SetupGet(x => x.AllBodyparts).Returns(new[] { inheritedBodypart });

		var bodies = new All<IBodyPrototype>();
		bodies.Add(parent.Object);
		var progs = new All<IFutureProg>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(bodies);
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs);

		var model = new BodyProto
		{
			Id = 2,
			Name = "Derived",
			CountsAsId = 1,
			WearSizeParameter = WearRules(),
			PlanarData = string.Empty
		};
		model.BodyProtosPositions.Add(new BodyProtosPositions { Position = (int)PositionStanding.Instance.Id });
		model.MoveSpeeds.Add(ModelSpeed(10, "shamble", PositionStanding.Instance.Id, 3.0));

		return (new BodyPrototype(model, gameworld.Object), parent.Object, inheritedBodypart);
	}

	private static IMoveSpeed Speed(long id, string alias, long positionId, double multiplier)
	{
		return new MoveSpeed(ModelSpeed(id, alias, positionId, multiplier));
	}

	private static MudSharp.Models.MoveSpeed ModelSpeed(long id, string alias, long positionId, double multiplier)
	{
		return new MudSharp.Models.MoveSpeed
		{
			Id = id,
			Alias = alias,
			PositionId = positionId,
			Multiplier = multiplier,
			StaminaMultiplier = 1.0,
			FirstPersonVerb = alias,
			ThirdPersonVerb = $"{alias}s",
			PresentParticiple = $"{alias}ing"
		};
	}

	private static WearableSizeParameterRule WearRules()
	{
		const string ratios = "<Ratios><Ratio Item=\"2\" Min=\"0\" Max=\"1000\" /></Ratios>";
		return new WearableSizeParameterRule
		{
			IgnoreTrait = true,
			MinHeightFactor = 0.5,
			MaxHeightFactor = 2.0,
			MinWeightFactor = 0.5,
			MaxWeightFactor = 2.0,
			WeightVolumeRatios = ratios,
			TraitVolumeRatios = ratios,
			HeightLinearRatios = ratios
		};
	}
}
