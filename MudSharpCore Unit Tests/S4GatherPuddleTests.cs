#nullable enable

using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;

namespace MudSharp_Unit_Tests;

[TestClass]
public class S4GatherPuddleTests
{
	private sealed class LiquidEffect(XElement root, IMagicSpell spell) : CreateLiquidEffect(root, spell);
	private sealed class Fixture
	{
		internal readonly MagicCastingFixture F = new();
		internal readonly Mock<IRoom> Room = new();
		internal readonly Mock<IUnitManager> Units = new();
		internal readonly Mock<ILiquid> Water = new();
		internal readonly SurfaceLiquidState Surface;
		internal readonly LiquidEffect Effect;
		internal double Capacity = 100000;
		internal double Conversion = 0.001;
		internal double Transferred;
		internal Fixture(int grade = 1)
		{
			Room.SetupGet(x => x.Id).Returns(11); Room.SetupGet(x => x.Gameworld).Returns(F.World.Object);
			F.Actor.SetupGet(x => x.Location).Returns(Room.Object); F.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
			F.Actor.SetupGet(x => x.RoutePositionMetres).Returns((double?)null);
			F.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection<IRoom>(() => [Room.Object]));
			Water.SetupGet(x => x.Id).Returns(34); Water.SetupGet(x => x.Name).Returns("clean water");
			Water.SetupGet(x => x.Density).Returns(1); Water.SetupGet(x => x.CountsAsLiquid).Returns(Water.Object);
			var liquids = new All<ILiquid>(); liquids.Add(Water.Object); F.World.SetupGet(x => x.Liquids).Returns(liquids);
			Units.SetupGet(x => x.BaseFluidToLitres).Returns(() => Conversion); F.World.SetupGet(x => x.UnitManager).Returns(Units.Object);
			F.World.Setup(x => x.GetStaticDouble("EnormousPoolLiquidQuantity")).Returns(() => Capacity);
			F.World.Setup(x => x.GetStaticBool("PuddlesEnabled")).Returns(true);
			Surface = new(F.World.Object);
			Room.SetupGet(x => x.SurfaceLiquidStates).Returns(() => [(RoomLayer.GroundLevel, Surface)]);
			Room.Setup(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), RoomLayer.GroundLevel, F.Actor.Object))
				.Callback<LiquidMixture, RoomLayer, IPerceivable?>((mixture, _, _) =>
				{ Transferred = mixture.TotalVolume; SurfaceLiquidTransferService.TransferToSurface(Surface, mixture, Capacity); });
			typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(F.Spell, grade);
			Effect = new(ArmageddonGradedWeaponSpellContent.GatherPuddle(34).BuildDefinition(41, 42, 0).Descendants("Effect").Single(), F.Spell);
		}
	}
	[DataTestMethod]
	[DataRow(1, 0.25)] [DataRow(7, 1.75)]
	public void NativeRoomPuddle_GradeLitresAndTopUp_PreservesNativeSurfaceCustody(int grade, double litres)
	{
		var f = new Fixture(grade); f.Surface.AddLiquid(new LiquidMixture(f.Water.Object, 100, f.F.World.Object));
		Assert.IsTrue(f.Effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, SpellPower.ExtremelyWeak, TimeSpan.Zero, out var application, out var error), error);
		Assert.AreEqual(100, f.Surface.LiquidVolume, 1e-9, "Admission cannot manifest water.");
		var operation = ((IMagicSpellEffectApplicationOperation)application!).Apply(null!);
		Assert.AreEqual(MagicEffectOperationStatus.Applied, operation.Status);
		Assert.AreEqual(litres, f.Transferred * f.Conversion, 1e-9);
		Assert.AreEqual(100 + litres / f.Conversion, f.Surface.LiquidVolume, 1e-9);
		Assert.IsTrue(f.Surface.ContaminatingLiquid.Instances.All(x => ReferenceEquals(x.Liquid, f.Water.Object)));
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(null!));
		f.Room.Verify(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), RoomLayer.GroundLevel, f.F.Actor.Object), Times.Once);
	}
	[TestMethod]
	public void NativeRoomPuddle_CapacityClampAndContainerTarget_NeverRefillsInventory()
	{
		var f = new Fixture(7) { Capacity = 1000 }; f.Surface.AddLiquid(new LiquidMixture(f.Water.Object, 900, f.F.World.Object));
		var container = new Mock<ILiquidContainer>(); var item = new Mock<IGameItem>(); item.Setup(x => x.GetItemType<ILiquidContainer>()).Returns(container.Object);
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.F.Actor.Object, item.Object, default, default, TimeSpan.Zero, out _, out _));
		Assert.IsTrue(f.Effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out var application, out var error), error);
		application!.Create(null!); Assert.AreEqual(1000, f.Surface.LiquidVolume, 1e-9); Assert.AreEqual(1000, f.Transferred, 1e-9);
		item.Verify(x => x.GetItemType<ILiquidContainer>(), Times.Never);
		container.Verify(x => x.MergeLiquid(It.IsAny<LiquidMixture>(), It.IsAny<MudSharp.Character.ICharacter>(), It.IsAny<string>()), Times.Never);
	}
	[DataTestMethod]
	[DataRow("room")] [DataRow("layer")] [DataRow("point")] [DataRow("conversion")] [DataRow("registry")]
	public void CapturedPuddleFrame_DriftBeforePayment_RefusesOriginalSelection(string drift)
	{
		var f = new Fixture(); var token = f.Effect.CapturePreparedSelection(f.F.Actor.Object, f.Room.Object)!;
		switch (drift)
		{
			case "room": f.F.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>()); break;
			case "layer": f.F.Actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InAir); break;
			case "point": f.F.Actor.SetupGet(x => x.RoutePositionMetres).Returns(1); break;
			case "conversion": f.Conversion = 0.01; break;
			case "registry": f.F.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection<IRoom>(() => [Mock.Of<IRoom>(x => x.Id == 11)])); break;
		}
		Assert.IsFalse(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent);
		Assert.ThrowsException<InvalidOperationException>(() => f.Effect.CapturePreparedSelection(f.F.Actor.Object, f.Room.Object));
		Assert.IsFalse(f.Effect.TryReusePreparedSelection(token, f.F.Actor.Object, f.Room.Object, out _));
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out _, out _));
		Assert.AreEqual(0, f.Surface.LiquidVolume); f.Room.Verify(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), It.IsAny<RoomLayer>(), It.IsAny<IPerceivable>()), Times.Never);
	}
	[TestMethod]
	public void AdmissionCallback_MovesBody_RefusesOriginalFrameBeforeWaterTransfer()
	{
		var f = new Fixture(); f.F.World.Setup(x => x.GetStaticDouble("EnormousPoolLiquidQuantity"))
			.Callback(() => f.F.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>())).Returns(1000);
		Assert.IsFalse(f.Effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out _, out _));
		Assert.AreEqual(0, f.Surface.LiquidVolume);
	}
	[DataTestMethod]
	[DataRow("room")] [DataRow("capacity")]
	public void NativeTransferCallback_ChangesPaidFrame_RetainsPaidWaterAndRefusesReplay(string mutation)
	{
		var f = new Fixture(); Assert.IsTrue(f.Effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out var application, out _));
		f.Room.Setup(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), RoomLayer.GroundLevel, f.F.Actor.Object))
			.Callback<LiquidMixture, RoomLayer, IPerceivable?>((mixture, _, _) =>
			{
				SurfaceLiquidTransferService.TransferToSurface(f.Surface, mixture, f.Capacity);
				if (mutation == "room") f.F.Actor.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
				else f.Capacity *= 2;
			});
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(null!)); Assert.AreEqual(250, f.Surface.LiquidVolume, 1e-9);
		Assert.ThrowsException<InvalidOperationException>(() => application!.Create(null!));
		f.Room.Verify(x => x.AddLiquidToSurface(It.IsAny<LiquidMixture>(), RoomLayer.GroundLevel, f.F.Actor.Object), Times.Once);
	}
	[DataTestMethod]
	[DataRow("namespace")] [DataRow("duplicate")] [DataRow("version")] [DataRow("litres")] [DataRow("container")]
	public void PuddlePolicy_MalformedColdXml_StaysRefusedAndPreserved(string mutation)
	{
		var f = new Fixture(); var xml = f.Effect.SaveToXml(); var policy = xml.Element("GatherPuddle")!;
		switch (mutation)
		{
			case "namespace": policy.Name = XName.Get("GatherPuddle", "urn:foreign"); break;
			case "duplicate": xml.Add(new XElement(policy)); break;
			case "version": policy.SetAttributeValue("version", 2); break;
			case "litres": policy.Element("Litres")!.Value = "250*power"; break;
			case "container": xml.Add(new XElement("ContainerFill", new XAttribute("version", 1), new XElement("Litres", "1"))); break;
		}
		var effect = new LiquidEffect(xml, f.F.Spell); Assert.IsNotNull(effect.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml())); Assert.IsNotNull(((CreateLiquidEffect)effect.Clone()).DefinitionError);
		Assert.IsFalse(effect.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out _, out _));
	}
	[TestMethod]
	public void ExplicitPuddlePolicy_ColdReloadAndTriggerCompatibility_RemainsRoomOnly()
	{
		var f = new Fixture(); var cold = (CreateLiquidEffect)f.Effect.Clone();
		Assert.IsTrue(cold.GatherPuddleOnly); Assert.IsFalse(cold.ContainerOnly); Assert.IsNull(cold.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(f.Effect.SaveToXml(), cold.SaveToXml()));
		Assert.IsTrue(cold.IsCompatibleWithTrigger(Mock.Of<IMagicTrigger>(x => x.TargetTypes == "room")));
		Assert.IsFalse(cold.IsCompatibleWithTrigger(Mock.Of<IMagicTrigger>(x => x.TargetTypes == "item")));
		Assert.IsFalse(cold.IsCompatibleWithTrigger(Mock.Of<IMagicTrigger>(x => x.TargetTypes == "character")));
	}
	[DataTestMethod]
	[DataRow("configuration")] [DataRow("grade")] [DataRow("liquid")]
	public void AdoptedPuddleSelection_ReceivingCopyMutation_RefusesWithoutChangingOriginalToken(string mutation)
	{
		var f = new Fixture(); var token = f.Effect.CapturePreparedSelection(f.F.Actor.Object, f.Room.Object)!;
		var model = (MudSharp.Models.MagicSpell)typeof(MagicSpell).GetMethod("SnapshotModel", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(f.F.Spell, [])!;
		var copiedSpell = new MagicSpell(model, f.F.World.Object);
		typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(copiedSpell, 1);
		var receiving = new LiquidEffect(f.Effect.SaveToXml(), copiedSpell);
		Assert.IsTrue(receiving.TryReusePreparedSelection(token, f.F.Actor.Object, f.Room.Object, out var error), error);
		switch (mutation)
		{
			case "configuration": Assert.IsTrue(receiving.BuildingCommand(f.F.Actor.Object, new StringStack("amount 1"))); break;
			case "grade": typeof(MagicSpell).GetProperty("InvocationGrade", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(copiedSpell, 7); break;
			case "liquid":
				((All<ILiquid>)f.F.World.Object.Liquids).Add(Mock.Of<ILiquid>(x => x.Id == 35 && x.Name == "other water"));
				Assert.IsTrue(receiving.BuildingCommand(f.F.Actor.Object, new StringStack("liquid 35"))); break;
		}
		Assert.IsTrue(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent, "Only the receiving copy changed.");
		Assert.IsFalse(receiving.TryConfirmPreparedSelection(f.F.Actor.Object, f.Room.Object, out _));
		Assert.IsFalse(receiving.TryPrepareApplication(f.F.Actor.Object, f.Room.Object, default, default, TimeSpan.Zero, out _, out _));
		Assert.IsFalse(receiving.TryReusePreparedSelection(token, f.F.Actor.Object, f.Room.Object, out _));
		Assert.AreEqual(0, f.Surface.LiquidVolume);
	}
}
