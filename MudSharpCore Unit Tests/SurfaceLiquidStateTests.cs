#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SurfaceLiquidStateTests
{
	[TestMethod]
	public void AddLiquidVolume_PositiveVolume_IncreasesExistingVolume()
	{
		var liquid = CreateLiquid("water").Object;
		var mixture = new LiquidMixture(liquid, 10.0, null);

		mixture.AddLiquidVolume(5.0);

		Assert.AreEqual(15.0, mixture.TotalVolume, 0.0001);
	}

	[TestMethod]
	public void CleanWithLiquid_UsesAmountAndSolventRatio()
	{
		var gameworld = new Mock<IFuturemud>();
		var solvent = CreateLiquid("water");
		var contaminant = CreateLiquid("oil");
		contaminant.Setup(x => x.Solvent).Returns(solvent.Object);
		contaminant.Setup(x => x.SolventVolumeRatio).Returns(2.0);

		var state = new SurfaceLiquidState(gameworld.Object);
		state.AddLiquid(new LiquidMixture(contaminant.Object, 10.0, gameworld.Object));

		state.CleanWithLiquid(new LiquidMixture(solvent.Object, 6.0, gameworld.Object), 6.0);

		Assert.AreEqual(7.0, state.LiquidVolume, 0.0001);
	}

	[TestMethod]
	public void Dry_CreatesResidueFromLiquidResidueRules()
	{
		var gameworld = new Mock<IFuturemud>();
		var residue = CreateSolid("salt").Object;
		var liquid = CreateLiquid("salt water");
		liquid.Setup(x => x.DriedResidue).Returns(residue);
		liquid.Setup(x => x.ResidueVolumePercentage).Returns(0.25);

		var state = new SurfaceLiquidState(gameworld.Object);
		state.AddLiquid(new LiquidMixture(liquid.Object, 8.0, gameworld.Object));

		state.Dry(4.0);

		Assert.AreEqual(4.0, state.LiquidVolume, 0.0001);
		Assert.AreEqual(1.0, state.ResidueWeight, 0.0001);
	}

	[TestMethod]
	public void Dry_RoomSurface_RespectsLeaveResiduesInRooms()
	{
		var gameworld = new Mock<IFuturemud>();
		var residue = CreateSolid("salt").Object;
		var liquid = CreateLiquid("salt water");
		liquid.Setup(x => x.DriedResidue).Returns(residue);
		liquid.Setup(x => x.ResidueVolumePercentage).Returns(0.25);
		liquid.Setup(x => x.LeaveResiduesInRooms).Returns(false);

		var state = new SurfaceLiquidState(gameworld.Object);
		state.AddLiquid(new LiquidMixture(liquid.Object, 8.0, gameworld.Object));

		state.Dry(4.0, roomSurface: true);

		Assert.AreEqual(0.0, state.ResidueWeight, 0.0001);
	}

	[TestMethod]
	public void Dry_EmptyState_DoesNotMarkChanged()
	{
		var changed = 0;
		var state = new SurfaceLiquidState(new Mock<IFuturemud>().Object, () => changed++);

		state.Dry(10.0);

		Assert.AreEqual(0, changed);
		Assert.IsTrue(state.IsEmpty);
	}

	[TestMethod]
	public void ResolveDrying_DriesElapsedIntervalsAndCreatesResidue()
	{
		var gameworld = new Mock<IFuturemud>();
		var changed = 0;
		var residue = CreateSolid("salt").Object;
		var liquid = CreateLiquid("salt water");
		liquid.Setup(x => x.DriedResidue).Returns(residue);
		liquid.Setup(x => x.ResidueVolumePercentage).Returns(0.25);

		var state = new SurfaceLiquidState(gameworld.Object, () => changed++);
		state.AddLiquid(new LiquidMixture(liquid.Object, 8.0, gameworld.Object));
		changed = 0;
		state.LastResolvedUtc = DateTime.UtcNow - TimeSpan.FromSeconds(600);

		var result = state.ResolveDrying(TimeSpan.FromSeconds(300), 0.0, 0.25);

		Assert.IsTrue(result);
		Assert.AreEqual(4.5, state.LiquidVolume, 0.0001);
		Assert.AreEqual(0.875, state.ResidueWeight, 0.0001);
		Assert.AreEqual(1, changed);
	}

	[TestMethod]
	public void SaturationLevel_UsesCoatingAndAbsorbCapacity()
	{
		var gameworld = new Mock<IFuturemud>();
		var liquid = CreateLiquid("water").Object;
		var state = new SurfaceLiquidState(gameworld.Object);
		state.AddLiquid(new LiquidMixture(liquid, 6.0, gameworld.Object));

		Assert.AreEqual(ItemSaturationLevel.Wet, state.SaturationLevel(5.0, 10.0));
		state.AddLiquid(new LiquidMixture(liquid, 10.0, gameworld.Object));
		Assert.AreEqual(ItemSaturationLevel.Saturated, state.SaturationLevel(5.0, 10.0));
	}

	private static Mock<ILiquid> CreateLiquid(string name)
	{
		var liquid = new Mock<ILiquid>();
		liquid.Setup(x => x.Id).Returns((long)name.GetHashCode());
		liquid.Setup(x => x.Name).Returns(name);
		liquid.Setup(x => x.Density).Returns(1.0);
		liquid.Setup(x => x.RelativeEnthalpy).Returns(1.0);
		liquid.Setup(x => x.LiquidCountsAs(It.IsAny<ILiquid>())).Returns<ILiquid>(other => ReferenceEquals(other, liquid.Object));
		liquid.Setup(x => x.SolventVolumeRatio).Returns(1.0);
		return liquid;
	}

	private static Mock<ISolid> CreateSolid(string name)
	{
		var solid = new Mock<ISolid>();
		solid.Setup(x => x.Id).Returns((long)name.GetHashCode());
		solid.Setup(x => x.Name).Returns(name);
		solid.Setup(x => x.MaterialDescription).Returns(name);
		solid.Setup(x => x.SolventRatio).Returns(1.0);
		return solid;
	}
}
