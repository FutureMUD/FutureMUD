#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Body.Position.PositionStates;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.Planes;
using MudSharp.Vehicles;
using DB = MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class TerrainImmersionTests
{
	[TestMethod]
	public void VeryShallowWater_LoadsAndRoundTripsWithoutUnderwaterLayer()
	{
		var f = new Fixture();
		var terrain = f.Load("veryshallowwater 42 Small opaque norun");
		Assert.AreSame(f.Liquid.Object, terrain.WaterFluid);
		Assert.AreEqual(new TerrainImmersionProfile(TerrainMedium.Liquid, SizeCategory.Small, true, true), terrain.Immersion);
		CollectionAssert.AreEquivalent(new[] { RoomLayer.GroundLevel, RoomLayer.InAir, RoomLayer.HighInAir }, terrain.TerrainLayers.ToArray());
		Assert.AreEqual(terrain.Immersion, f.Load(terrain.TerrainBehaviourString).Immersion);
	}

	[TestMethod]
	public void Silt_HasNoLiquidAndRetainsIndependentOptions()
	{
		var f = new Fixture();
		var deep = f.Load("silt Large norun");
		var shallows = f.Load("silt Small clear");
		Assert.IsNull(deep.WaterFluid);
		Assert.IsTrue(deep.Immersion!.ObscuresItems);
		Assert.IsTrue(deep.Immersion.RestrictRunning);
		Assert.IsFalse(shallows.Immersion!.ObscuresItems);
		Assert.IsFalse(shallows.Immersion.RestrictRunning);
		Assert.AreEqual(shallows.Immersion, f.Load(shallows.TerrainBehaviourString).Immersion);
	}

	[TestMethod]
	[DataRow("veryshallowwater 999 Small")]
	[DataRow("veryshallowwater 42 invalidsize")]
	[DataRow("silt 99")]
	[DataRow("silt Small nonsense")]
	public void MalformedPersistedModels_RefuseInsteadOfSilentlyBecomingDryGround(string model)
	{
		Assert.ThrowsExactly<FormatException>(() => new Fixture().Load(model));
	}

	[TestMethod]
	public void BuilderModelChange_ValidatesBeforeMutationAndClearsOldLiquid()
	{
		var f = new Fixture();
		var actor = new Mock<ICharacter> { DefaultValue = DefaultValue.Mock };
		var terrain = f.Load("veryshallowwater 42 Small");
		Assert.IsFalse(terrain.BuildingCommand(actor.Object, new StringStack("model silt invalidsize")));
		Assert.AreSame(f.Liquid.Object, terrain.WaterFluid);
		Assert.IsTrue(terrain.BuildingCommand(actor.Object, new StringStack("model silt Small")));
		Assert.IsNull(terrain.WaterFluid);
		Assert.IsTrue(terrain.BuildingCommand(actor.Object, new StringStack("model outdoors")));
		Assert.IsNull(terrain.Immersion);
		Assert.IsNull(terrain.WaterFluid);
	}

	[TestMethod]
	public void LegacyShallowWater_StillHasSwimmingAndUnderwaterStrata()
	{
		var terrain = new Fixture().Load("shallowwater 42");
		Assert.IsNull(terrain.Immersion);
		CollectionAssert.Contains(terrain.TerrainLayers.ToArray(), RoomLayer.Underwater);
	}

	[TestMethod]
	public void StandingAndProne_UseSameContactForWettingAndAirway()
	{
		var f = new Fixture();
		Assert.AreEqual(ImmersionExtent.Partial, TerrainImmersionHelper.ContactFor(f.Body.Object).Extent);
		CollectionAssert.AreEquivalent(new[] { f.Foot.Object, f.Torso.Object }, TerrainImmersionHelper.ImmersedParts(f.Body.Object));
		CollectionAssert.AreEquivalent(new[] { f.Foot.Object, f.Torso.Object }, EnvironmentalExposureService.ImmersedParts(f.Body.Object));
		Assert.AreSame(f.Air.Object, TerrainImmersionHelper.RespiratoryFluid(f.Body.Object, f.Mouth.Object));
		f.Body.Object.PositionState = PositionProne.Instance;
		Assert.AreEqual(ImmersionExtent.Full, TerrainImmersionHelper.ContactFor(f.Body.Object).Extent);
		Assert.AreSame(f.Liquid.Object, TerrainImmersionHelper.RespiratoryFluid(f.Body.Object, f.Mouth.Object));
	}

	[TestMethod]
	public void WadingDependsOnSize_WithoutNewBodySpeedOrAnatomyData()
	{
		var f = new Fixture();
		Assert.IsTrue(TerrainImmersionHelper.IsWading(f.Actor.Object));
		Assert.IsFalse(TerrainImmersionHelper.RequiresSwimming(f.Actor.Object));
		f.Body.SetupGet(x => x.SizeStanding).Returns(SizeCategory.VeryLarge);
		Assert.IsFalse(TerrainImmersionHelper.IsWading(f.Actor.Object));
		f.Body.SetupGet(x => x.SizeStanding).Returns(SizeCategory.Small);
		Assert.IsTrue(TerrainImmersionHelper.RequiresSwimming(f.Actor.Object));
	}

	[TestMethod]
	public void SmallSurfaceSwimmer_KeepsAirwayAboveWaterUntilFlotationIsLost()
	{
		var f = new Fixture();
		f.Body.SetupGet(x => x.SizeStanding).Returns(SizeCategory.Small);
		f.Body.Object.PositionState = PositionSwimming.Instance;
		Assert.AreEqual(ImmersionExtent.Partial, TerrainImmersionHelper.ContactFor(f.Body.Object).Extent);
		Assert.IsFalse(TerrainImmersionHelper.IsPartImmersed(f.Body.Object, f.Mouth.Object));
		f.Actor.SetupGet(x => x.State).Returns(CharacterState.Unconscious);
		Assert.AreEqual(ImmersionExtent.Full, TerrainImmersionHelper.ContactFor(f.Body.Object).Extent);
		Assert.IsTrue(TerrainImmersionHelper.IsPartImmersed(f.Body.Object, f.Mouth.Object));
	}

	[TestMethod]
	public void Appendage_IsNotTreatedAsHigherThanTheHead()
	{
		var f = new Fixture();
		var hand = Part(Orientation.Appendage, BodypartTypeEnum.Grabbing);
		var limb = new Mock<ILimb>();
		limb.SetupGet(x => x.LimbType).Returns(LimbType.Arm);
		limb.SetupGet(x => x.Parts).Returns([hand.Object]);
		f.Prototype.SetupGet(x => x.Limbs).Returns([limb.Object]);
		f.Body.SetupGet(x => x.Bodyparts).Returns([hand.Object]);
		CollectionAssert.Contains(TerrainImmersionHelper.ImmersedParts(f.Body.Object), hand.Object);
	}

	[TestMethod]
	public void OpaqueCoverage_HidesOnlyFullyCoveredLooseBottomItems()
	{
		var f = new Fixture("veryshallowwater 42 Small opaque");
		var item = f.Item(SizeCategory.Small);
		Assert.IsTrue(TerrainImmersionHelper.IsObscuredByTerrain(item.Object));
		item.Setup(x => x.Buoyancy(It.IsAny<double>())).Returns(1);
		Assert.IsFalse(TerrainImmersionHelper.IsObscuredByTerrain(item.Object));
		item.Setup(x => x.Buoyancy(It.IsAny<double>())).Returns(-1);
		item.SetupGet(x => x.InInventoryOf).Returns(f.Body.Object);
		Assert.IsFalse(TerrainImmersionHelper.IsObscuredByTerrain(item.Object));
	}

	[TestMethod]
	public void ClearWater_LeavesCoveredItemsVisibleAndSiltDoesNotSupplyLiquid()
	{
		var f = new Fixture();
		Assert.IsFalse(TerrainImmersionHelper.IsObscuredByTerrain(f.Item(SizeCategory.Small).Object));
		f.Terrain = f.Load("silt Small");
		Assert.IsTrue(TerrainImmersionHelper.IsObscuredByTerrain(f.Item(SizeCategory.Small).Object));
		Assert.IsNull(TerrainImmersionHelper.LiquidFor(f.Body.Object));
		Assert.AreEqual(0, EnvironmentalExposureService.ImmersedParts(f.Body.Object).Length);
		Assert.AreSame(f.Air.Object, TerrainImmersionHelper.RespiratoryFluid(f.Body.Object, f.Mouth.Object));
	}

	[TestMethod]
	public void FloatingCorpse_UsesSurfaceCoverageRatherThanFullProneBottomCoverage()
	{
		var f = new Fixture();
		var remains = f.Item(SizeCategory.Small);
		remains.Setup(x => x.Buoyancy(It.IsAny<double>())).Returns(1);
		var parts = TerrainImmersionHelper.ImmersedParts(f.Body.Object, remains.Object);
		CollectionAssert.Contains(parts, f.Foot.Object);
		CollectionAssert.Contains(parts, f.Torso.Object);
		CollectionAssert.DoesNotContain(parts, f.Mouth.Object);
	}

	[TestMethod]
	public void FlyingLooseItem_HasNoContactOrOpaqueCoverage()
	{
		var f = new Fixture("silt Small");
		var item = f.Item(SizeCategory.Small);
		item.SetupGet(x => x.PositionState).Returns(PositionFlying.Instance);
		Assert.AreEqual(ImmersionExtent.None, TerrainImmersionHelper.ItemContactFor(item.Object).Extent);
		Assert.IsFalse(TerrainImmersionHelper.IsObscuredByTerrain(item.Object));
	}

	[TestMethod]
	public void RestrictedPace_UsesNativeDurationRatherThanGaitAliases()
	{
		var f = new Fixture("silt Small norun");
		var fast = new Mock<MudSharp.Movement.IMoveSpeed>();
		var slow = new Mock<MudSharp.Movement.IMoveSpeed>();
		fast.SetupGet(x => x.Position).Returns(PositionStanding.Instance);
		slow.SetupGet(x => x.Position).Returns(PositionStanding.Instance);
		fast.SetupGet(x => x.Multiplier).Returns(0.5);
		slow.SetupGet(x => x.Multiplier).Returns(2.0);
		f.Actor.SetupGet(x => x.Speeds).Returns([fast.Object, slow.Object]);
		Assert.AreSame(slow.Object, MudSharp.Character.Character.ImmersionMovementSpeed(f.Actor.Object,
			fast.Object, PositionStanding.Instance));
		f.Terrain = f.Load("silt Small");
		Assert.AreSame(fast.Object, MudSharp.Character.Character.ImmersionMovementSpeed(f.Actor.Object,
			fast.Object, PositionStanding.Instance));
	}

	[TestMethod]
	public void GroundExit_RequiresSwimmingForSmallActorButNotStandingWader()
	{
		var f = new Fixture();
		var origin = new Mock<IRoom>();
		origin.Setup(x => x.Terrain(It.IsAny<IPerceiver?>())).Returns(f.Load("outdoors"));
		var parent = new Mock<IExit>();
		parent.SetupGet(x => x.BlockedLayers).Returns([]);
		var exit = new RoomExit(parent.Object, origin.Object, f.Room.Object, CardinalDirection.North, CardinalDirection.South);
		f.Actor.SetupGet(x => x.Location).Returns(origin.Object);
		Assert.AreNotEqual(RoomMovementTransition.SwimOnly, exit.MovementTransition(f.Actor.Object).TransitionType);
		f.Body.SetupGet(x => x.SizeStanding).Returns(SizeCategory.Small);
		Assert.AreEqual(RoomMovementTransition.SwimOnly, exit.MovementTransition(f.Actor.Object).TransitionType);
	}

	[TestMethod]
	public void VehicleSurfaceCompatibility_UsesMediumRatherThanSwimmingClassification()
	{
		var f = new Fixture();
		Assert.IsTrue(f.Room.Object.IsSurfaceWater(RoomLayer.GroundLevel));
		Assert.IsFalse(f.Room.Object.IsSwimmingLayer(RoomLayer.GroundLevel));
		var vehicle = new Mock<IVehicle>();
		var profile = new Mock<IVehicleMovementProfilePrototype>();
		vehicle.SetupGet(x => x.Location).Returns(f.Room.Object);
		vehicle.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		vehicle.SetupGet(x => x.MovementProfile).Returns(profile.Object);
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.SurfaceWater);
		Assert.IsTrue(vehicle.Object.CanTraverseEnvironment(f.Room.Object, RoomLayer.GroundLevel, out _));
		f.Terrain = f.Load("silt Small");
		Assert.IsFalse(vehicle.Object.CanTraverseEnvironment(f.Room.Object, RoomLayer.GroundLevel, out _));
		profile.SetupGet(x => x.MovementEnvironment).Returns(VehicleMovementEnvironment.Silt);
		Assert.IsTrue(vehicle.Object.CanTraverseEnvironment(f.Room.Object, RoomLayer.GroundLevel, out _));
	}

	[TestMethod]
	[DataRow("silt Small norun", "outdoors", "wading", "stroll")]
	[DataRow("veryshallowwater 42 Small opaque norun", "outdoors", "wading", "stroll")]
	[DataRow("outdoors", "silt Small norun", "strolling", "wade")]
	[DataRow("outdoors", "outdoors", "running", "run")]
	public void MovementNarration_UsesWadingAndEffectivePaceAcrossMovementPhases(string originModel,
		string destinationModel, string participle, string arrivalVerb)
	{
		var f = new Fixture(originModel);
		var fast = new Mock<MudSharp.Movement.IMoveSpeed>();
		var slow = new Mock<MudSharp.Movement.IMoveSpeed>();
		fast.SetupGet(x => x.Position).Returns(PositionStanding.Instance);
		slow.SetupGet(x => x.Position).Returns(PositionStanding.Instance);
		fast.SetupGet(x => x.Multiplier).Returns(0.5);
		slow.SetupGet(x => x.Multiplier).Returns(2.0);
		fast.SetupGet(x => x.PresentParticiple).Returns("running");
		slow.SetupGet(x => x.PresentParticiple).Returns("strolling");
		fast.SetupGet(x => x.FirstPersonVerb).Returns("run");
		slow.SetupGet(x => x.FirstPersonVerb).Returns("stroll");
		fast.SetupGet(x => x.ThirdPersonVerb).Returns("runs");
		slow.SetupGet(x => x.ThirdPersonVerb).Returns("strolls");
		f.Actor.SetupGet(x => x.CurrentSpeed).Returns(fast.Object);
		f.Actor.SetupGet(x => x.Speeds).Returns([fast.Object, slow.Object]);
		f.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		f.Actor.Setup(x => x.IsSelf(It.IsAny<IPerceivable>())).Returns<IPerceivable>(x => ReferenceEquals(x, f.Actor.Object));
		f.Actor.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns<IPerceiver, bool, DescriptionType, bool, PerceiveIgnoreFlags>(
			(voyeur, _, _, _, _) => ReferenceEquals(voyeur, f.Actor.Object) ? "you" : "the walker");
		var destination = new Mock<IRoom>();
		destination.Setup(x => x.Terrain(It.IsAny<IPerceiver>())).Returns(f.Load(destinationModel));
		var exit = new Mock<IRoomExit>();
		exit.SetupGet(x => x.Origin).Returns(f.Room.Object);
		exit.SetupGet(x => x.Destination).Returns(destination.Object);
		exit.SetupGet(x => x.OutboundMovementSuffix).Returns("towards the north");
		exit.SetupGet(x => x.InboundMovementSuffix).Returns("from the south");
		var movement = new MudSharp.Movement.Movement(f.Actor.Object, null!, [], [], [f.Actor.Object], [], [], [], exit.Object);

		StringAssert.Contains(movement.DescribeBeginMove(f.Actor.Object), $"You begin {participle} towards the north.");
		StringAssert.Contains(movement.Describe(f.Actor.Object).StripANSIColour(), $"You are {participle} towards the north.");
		var observer = new Mock<IPerceiver>();
		observer.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(true);
		StringAssert.Contains(movement.DescribeBeginMove(observer.Object), $"The walker begins {participle} towards the north.");

		// Arrival descriptions run after ExecuteMove has changed the actor's room.
		f.Actor.SetupGet(x => x.Location).Returns(destination.Object);
		f.Body.SetupGet(x => x.Location).Returns(destination.Object);
		typeof(MudSharp.Movement.Movement).GetProperty(nameof(movement.Phase))!
			.SetValue(movement, MudSharp.Movement.MovementPhase.NewRoom);
		StringAssert.Contains(movement.DescribeEnterMove(f.Actor.Object), $"You {arrivalVerb} from the south.");
		Assert.AreSame(fast.Object, f.Actor.Object.CurrentSpeed, "Narrative preview must not mutate the selected gait.");
	}

	private static Mock<IExternalBodypart> Part(Orientation orientation, BodypartTypeEnum type)
	{
		var part = new Mock<IExternalBodypart>();
		part.SetupGet(x => x.Orientation).Returns(orientation);
		part.SetupGet(x => x.BodypartType).Returns(type);
		return part;
	}

	private sealed class Fixture
	{
		public Mock<IFuturemud> World { get; } = new();
		public Mock<ILiquid> Liquid { get; } = new();
		public Mock<IGas> Air { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IBodyPrototype> Prototype { get; } = new();
		public Mock<IExternalBodypart> Foot { get; } = Part(Orientation.Lowest, BodypartTypeEnum.Standing);
		public Mock<IExternalBodypart> Torso { get; } = Part(Orientation.Centre, BodypartTypeEnum.Wear);
		public Mock<IExternalBodypart> Mouth { get; } = Part(Orientation.Highest, BodypartTypeEnum.Mouth);
		public ITerrain Terrain { get; set; }

		public Fixture(string model = "veryshallowwater 42 Small")
		{
			Liquid.SetupGet(x => x.Id).Returns(42);
			Liquid.SetupGet(x => x.Name).Returns("water");
			Liquid.SetupGet(x => x.Density).Returns(1);
			World.SetupGet(x => x.Liquids).Returns(new All<ILiquid> { Liquid.Object });
			World.SetupGet(x => x.Vehicles).Returns(new All<IVehicle>());
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			Terrain = Load(model);
			Room.Setup(x => x.Terrain(It.IsAny<IPerceiver?>())).Returns(() => Terrain);
			Room.SetupGet(x => x.Atmosphere).Returns(Air.Object);
			Room.SetupGet(x => x.Location).Returns(Room.Object);
			Room.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns([]);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			Actor.SetupGet(x => x.State).Returns(CharacterState.Awake);
			Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Body.SetupGet(x => x.Gameworld).Returns(World.Object);
			Body.SetupGet(x => x.Location).Returns(Room.Object);
			Body.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			Body.SetupGet(x => x.Prototype).Returns(Prototype.Object);
			Body.SetupGet(x => x.SizeStanding).Returns(SizeCategory.Normal);
			Body.SetupGet(x => x.SizeSitting).Returns(SizeCategory.Small);
			Body.SetupGet(x => x.SizeProne).Returns(SizeCategory.Small);
			Body.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Mouth.SetupGet(x => x.UpstreamConnection).Returns(Torso.Object);
			Body.SetupGet(x => x.Bodyparts).Returns([Foot.Object, Torso.Object, Mouth.Object]);
			Body.SetupProperty(x => x.PositionState, PositionStanding.Instance);
			Actor.SetupGet(x => x.PositionState).Returns(() => Body.Object.PositionState);
			Prototype.SetupGet(x => x.Limbs).Returns([]);
		}

		public Terrain Load(string model) => new(new DB.Terrain
		{
			Id = 1, Name = "Immersion test", TerrainBehaviourMode = model, TagInformation = string.Empty
		}, World.Object);

		public Mock<IGameItem> Item(SizeCategory size)
		{
			var item = new Mock<IGameItem>();
			item.SetupGet(x => x.Gameworld).Returns(World.Object);
			item.SetupGet(x => x.Location).Returns(Room.Object);
			item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			item.SetupGet(x => x.Size).Returns(size);
			item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			item.Setup(x => x.Buoyancy(It.IsAny<double>())).Returns(-1);
			return item;
		}
	}
}
