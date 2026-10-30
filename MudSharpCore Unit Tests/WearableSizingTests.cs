#nullable enable
using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory.Size;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WearableSizingTests
{
	[TestMethod]
	public void TryParse_SupportsLegacyScalarAndExplicitUnfittedOrUniversal()
	{
		var world = World();

		Assert.IsTrue(WearableSizing.TryParse("12.5", null, world, out var scalar, out _));
		Assert.AreEqual(WearableFitKind.Scalar, scalar.Kind);
		Assert.AreEqual(12.5, scalar.Scalar);
		Assert.IsTrue(WearableSizing.TryParse("unfitted", null, world, out var unfitted, out _));
		Assert.AreEqual(WearableFitKind.Unfitted, unfitted.Kind);
		Assert.IsTrue(WearableSizing.TryParse("universal", null, world, out var universal, out _));
		Assert.AreEqual(WearableFitKind.Universal, universal.Kind);
		Assert.IsFalse(WearableSizing.TryParse("-1", null, world, out _, out _));
	}

	[TestMethod]
	public void TryParse_ConvertsConfiguredLengthAndMassUnitsToNativeMeasurements()
	{
		var units = new Mock<IUnitManager>();
		var success = true;
		units.Setup(x => x.GetBaseUnits("175 cm", UnitType.Length, out success))
			.Returns((string _, UnitType _, out bool parsed) => { parsed = true; return 1.75; });
		units.Setup(x => x.GetBaseUnits("70 kg", UnitType.Mass, out success))
			.Returns((string _, UnitType _, out bool parsed) => { parsed = true; return 70.0; });
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.UnitManager).Returns(units.Object);

		Assert.IsTrue(WearableSizing.TryParse("height 175 cm weight 70 kg", null, world.Object, out var fit, out _));
		Assert.AreEqual(WearableFitKind.Measurements, fit.Kind);
		Assert.AreEqual(1.75, fit.Height);
		Assert.AreEqual(70.0, fit.Weight);
	}

	[TestMethod]
	public void LoadArguments_NormaliseTargetToPortableSnapshotAndLastOptionWins()
	{
		var body = Body(1.72, 66.0);
		var target = new Mock<ITarget>();
		target.Setup(x => x.TargetBody("Mira")).Returns(body);
		var world = World();
		var normalised = WearableSizing.NormaliseLoadArguments("colour=blue wearsize=\"for Mira\"", target.Object, world);

		Assert.AreEqual("colour=blue wearsize=measurements:1.72,66", normalised);
		Assert.AreEqual("colour=blue ", WearableSizing.RemoveLoadArguments(normalised));
		Assert.ThrowsExactly<ArgumentException>(() => WearableSizing.NormaliseLoadArguments("wearsize=\"for missing\"", target.Object, world));

		var item = new Mock<IGameItem>();
		var wearable = new Mock<IWearable>();
		item.Setup(x => x.Gameworld).Returns(world);
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wearable.Object);
		var args = "wearsize=1 wearsize=universal";
		WearableSizing.ApplyLoadArguments(item.Object, args, target.Object);
		wearable.Verify(x => x.SetFit(It.Is<WearableFit>(f => f.Kind == WearableFitKind.Universal)), Times.Once);
	}

	[TestMethod]
	public void LoadArguments_QuotedCharacteristicTextIsOpaque_AndNonWearablePreflightRejectsSizing()
	{
		var world = World();
		const string args = "pattern=\"red wearsize=10\" colour=blue";
		Assert.AreEqual(args, WearableSizing.NormaliseLoadArguments(args, null, world));
		Assert.AreEqual(args, WearableSizing.RemoveLoadArguments(args));
		Assert.IsFalse(WearableSizing.HasLoadArguments(args));
		Assert.ThrowsExactly<ArgumentException>(() => WearableSizing.NormaliseLoadArguments("wearsize=", null, world));
		var proto = new Mock<IGameItemProto>();
		Assert.ThrowsExactly<ArgumentException>(() => WearableSizing.NormaliseLoadArguments("wearsize=10", null, world, proto.Object));
	}

	private static IBody Body(double height, double weight)
	{
		var body = new Mock<IBody>();
		body.Setup(x => x.Height).Returns(height);
		body.Setup(x => x.Weight).Returns(weight);
		return body.Object;
	}

	private static IFuturemud World() => new Mock<IFuturemud>().Object;
}
