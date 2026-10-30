#nullable enable
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory.Size;
using MudSharp.GameItems.Prototypes;

namespace MudSharp_Unit_Tests;

[TestClass]
public class WearableSizingRuntimeTests
{
	[TestMethod]
	public void FirstWearCapture_DoesNotBindDuringChecksOrRestoration_CommitAndCopyPreserveSnapshot()
	{
		var (world, parent) = World(true);
		var proto = new Proto("<Definition><Sizing enabled=\"true\" firstwear=\"true\"/></Definition>", world);
		var item = new Component(proto, parent);
		var body = Body(1.8, 80);
		Assert.IsTrue(item.CanWear(body));
		Assert.AreEqual(WearableFitKind.Unfitted, item.Fit.Kind);
		item.UpdateWear(body, null!); // reconstruction uses this path, without a new fitting
		Assert.AreEqual(WearableFitKind.Unfitted, item.Fit.Kind);
		var captured = item.CaptureFitOnWear(body);
		Assert.AreEqual(WearableFitKind.Unfitted, item.Fit.Kind);
		item.SetFit(captured);
		var copy = (WearableGameItemComponent)item.Copy(parent, true);
		var restored = new Component(item.Xml(), proto, parent);
		Assert.AreEqual(captured, copy.Fit);
		Assert.AreEqual(captured, restored.Fit);
		Assert.IsFalse(restored.Fits(Body(2.6, 150)));
	}

	[TestMethod]
	public void GlobalOptOut_DoesNotEnforceOrCaptureFit_AndLegacyXmlRemainsUnfitted()
	{
		var (world, parent) = World(false);
		var proto = new Proto("<Definition><Sizing enabled=\"true\" firstwear=\"true\"/></Definition>", world);
		var item = new Component(proto, parent);
		Assert.AreEqual(WearableFitKind.Unfitted, item.CaptureFitOnWear(Body(1.8, 80)).Kind);
		item.SetFit(WearableFit.ForMeasurements(1.0, 30));
		Assert.IsTrue(item.Fits(Body(2.5, 200)));
		Assert.AreEqual(WearableFitKind.Unfitted, new Component("<Definition/>", proto, parent).Fit.Kind);
	}

	[TestMethod]
	public void FixedMeasurementFit_IsCheckedWithoutChangingItsSavedSize()
	{
		var (world, parent) = World(true);
		var xml = new XElement("Definition", new XElement("Sizing", new XAttribute("enabled", true),
			new XAttribute("firstwear", false), WearableFit.ForMeasurements(1.8, 80).SaveToXml()));
		var proto = new Proto(xml.ToString(), world);
		var item = new Component(proto, parent);
		Assert.AreEqual(item.Fit, new Component("<Definition/>", proto, parent).Fit);
		Assert.AreEqual(WearableFitKind.Unfitted,
			new Component(new XElement("Definition", WearableFit.Unfitted.SaveToXml()).ToString(), proto, parent).Fit.Kind);
		Assert.IsTrue(item.Fits(Body(1.81, 85)));
		var wrongBody = Body(2.5, 150);
		Assert.IsFalse(item.CanWear(wrongBody));
		using (item.BypassSizingFor(wrongBody))
		{
			Assert.IsTrue(item.CanWear(wrongBody));
			Assert.IsFalse(item.CanWear(Body(2.5, 150)));
			Assert.AreEqual(item.Fit, item.CaptureFitOnWear(wrongBody));
		}
		Assert.IsFalse(item.CanWear(wrongBody));
		Assert.AreEqual(WearableFit.ForMeasurements(1.8, 80), item.Fit);
		item.SetFit(WearableFit.Unfitted); // Explicit load/prog/admin overrides also work on fixed source patterns.
		Assert.AreEqual(WearableFit.ForBody(wrongBody), item.CaptureFitOnWear(wrongBody));
		Assert.AreEqual(WearableFitKind.Unfitted, item.Fit.Kind);
	}

	private static (IFuturemud World, IGameItem Item) World(bool enabled)
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticBool("EnableWearableSizing")).Returns(enabled);
		world.Setup(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		world.Setup(x => x.FutureProgs).Returns(new Mock<IUneditableAll<IFutureProg>>().Object);
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.Gameworld).Returns(world.Object);
		return (world.Object, parent.Object);
	}
	private static IBody Body(double height, double weight)
	{
		var body = new Mock<IBody>();
		var actor = new Mock<ICharacter>();
		body.Setup(x => x.Height).Returns(height);
		body.Setup(x => x.Weight).Returns(weight);
		body.Setup(x => x.Actor).Returns(actor.Object);
		actor.Setup(x => x.Body).Returns(body.Object);
		return body.Object;
	}
	private sealed class Proto(string xml, IFuturemud world) : WearableGameItemComponentProto(new MudSharp.Models.GameItemComponentProto
	{
		Id = 8, Name = "test", Description = "test", Type = "Wearable", Definition = xml,
		EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = 4 }
	}, world);
	private sealed class Component : WearableGameItemComponent
	{
		public Component(WearableGameItemComponentProto proto, IGameItem parent) : base(proto, parent, null!, true) { }
		public Component(string xml, WearableGameItemComponentProto proto, IGameItem parent) : base(new MudSharp.Models.GameItemComponent { Definition = xml }, proto, parent) { }
		public string Xml() => SaveToXml();
	}
}
