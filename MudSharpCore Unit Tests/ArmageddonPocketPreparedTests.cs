#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPocketPreparedTests
{
	[TestMethod]
	[DataRow("quantity")][DataRow("deleted")][DataRow("registry")][DataRow("held")]
	[DataRow("custody")][DataRow("prototype-revision")][DataRow("fallback-overlay")][DataRow("body")]
	public void PreparedPocket_PhysicalSourceAndTypedBindingsDrift_RefuseBeforePayment(string change)
	{
		var f = new PocketFrameFixture();
		f.Cast.Checkpoint = stage => { if (stage == "BeforePayment") f.Change(change); };
		var result = f.Cast.Service.Cast(f.Intent);
		Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(100.0, f.Cast.Balances[f.Cast.Resources[1]]);
		Assert.AreEqual(0, f.Cast.Rolls); Assert.AreEqual(0, f.Cast.Store.Operations.Count);
		f.Service.Verify(x => x.CreatePrepared(It.IsAny<ICharacter>(), It.IsAny<IGameItem>(), It.IsAny<SpellPocketConfiguration>(),
			It.IsAny<SpellLifecycleOrigin>(), It.IsAny<Func<bool>>()), Times.Never);
	}

	[TestMethod]
	[DataRow("confirm")][DataRow("reuse")]
	public void PreparedPocket_AdmissionCallbackChangesBorrowedSource_PostCallbackFenceRefuses(string phase)
	{
		var f = new PocketFrameFixture();
		var effect = f.PreparedEffect();
		var token = effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Source.Object)!;
		f.Service.Setup(x => x.AdmissionError(f.Cast.Actor.Object, f.Source.Object, f.Configuration, 1))
			.Callback(() => f.Change("quantity")).Returns((string?)null);
		var admitted = phase == "confirm"
			? effect.TryConfirmPreparedSelection(f.Cast.Actor.Object, f.Source.Object, out _)
			: effect.TryReusePreparedSelection(token, f.Cast.Actor.Object, f.Source.Object, out _);
		Assert.IsFalse(admitted); Assert.IsFalse(((IMagicSpellEffectPreparedSelectionRawToken)token).IsCurrent);
		Assert.AreEqual(100.0, f.Cast.Balances[f.Cast.Resources[1]]); Assert.AreEqual(0, f.Cast.Store.Operations.Count);
	}

	[TestMethod]
	public void PreparedPocket_KnownDriftRefusesNativeCreationBeforeOpeningDatabase()
	{
		var f = new PocketFrameFixture();
		var now = DateTime.UtcNow;
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), f.Spell.Id, 1, 100, SpellPocketAnchor.Family,
			SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(300), new SpellPocketAnchor(9, 1, f.Configuration).Save());
		var native = new SpellOwnedPocketService(f.Cast.World.Object);
		var error = Assert.ThrowsExactly<InvalidOperationException>(() => native.CreatePrepared(
			f.Cast.Actor.Object, f.Source.Object, f.Configuration, origin, () => false));
		StringAssert.Contains(error.Message, "changed during native creation");
		f.Cast.World.Verify(x => x.Add(It.IsAny<IGameItem>()), Times.Never);
	}

	[TestMethod]
	[DataRow("temporary")][DataRow("water")][DataRow("no-ground")][DataRow("shelter")][DataRow("draft")]
	public void PocketFallback_UnsafeNativeDestinationRefuses(string change)
	{
		var room = SafeFallback();
		if (change == "temporary") room.SetupGet(x => x.Temporary).Returns(true);
		if (change == "water") room.Setup(x => x.IsSwimmingLayer(RoomLayer.GroundLevel)).Returns(true);
		if (change == "no-ground") room.Setup(x => x.Terrain(null!)).Returns(Mock.Of<ITerrain>(x => x.TerrainLayers == new[] { RoomLayer.Underwater }));
		if (change == "draft") Mock.Get(room.Object.CurrentOverlay.Package).SetupGet(x => x.Status).Returns(RevisionStatus.UnderDesign);
		Assert.IsFalse(SpellOwnedPocketService.TryFallbackLocation(room.Object, change == "shelter", out _));
	}

	[TestMethod]
	public void PocketFallback_UsesExplicitRouteDefaultInsteadOfMissingCoordinate()
	{
		var room = SafeFallback();
		var route = Mock.Of<IRouteRoomDefinition>(x => x.LengthMetres == 100 && x.DefaultPositionMetres == 37 && x.TopologyVersion == 2);
		room.SetupGet(x => x.RouteDefinition).Returns(route);
		Assert.IsTrue(SpellOwnedPocketService.TryFallbackLocation(room.Object, false, out var destination));
		Assert.AreSame(room.Object, destination.Room); Assert.AreEqual(37.0, destination.RoutePositionMetres);
	}

	[TestMethod]
	public void CanonicalPocket_ApprovedFloorAndBoundaryCapacityDeadline_RoundTrip()
	{
		var configuration = new SpellPocketConfiguration(7, 2, SizeCategory.VeryLarge, 300, SpellPocketAccess.Bearer, 73);
		var content = ArmageddonPocketContent.Create(configuration);
		Assert.AreEqual(7.0, content.MinimumEnergy); Assert.AreEqual("300*grade", content.DurationFormula);
		var definition = content.BuildDefinition(11, 12, 0);
		Assert.AreEqual(7.0, (double)definition.Element("ControlledPower")!.Element("Efficiency")!.Attribute("minimum")!);
		var native = new CreatePocketEffect(definition.Element("Effects")!.Elements().Single(), Mock.Of<IMagicSpell>());
		Assert.IsNull(native.DefinitionError); Assert.AreEqual(configuration, native.Configuration());
		foreach (var grade in new[] { 1, 7 })
		{
			var anchor = SpellPocketAnchor.Load(new SpellPocketAnchor(9, grade, configuration).Save());
			Assert.AreEqual(2.0 * grade, anchor.Capacity); Assert.AreEqual(TimeSpan.FromSeconds(300 * grade), anchor.Configuration.DurationForGrade(grade));
		}
	}

	private static Mock<IRoom> SafeFallback()
	{
		var room = new Mock<IRoom>(); room.As<ICustodyRollbackLocation>();
		var terrain = Mock.Of<ITerrain>(x => x.TerrainLayers == new[] { RoomLayer.GroundLevel });
		var package = Mock.Of<IRoomOverlayPackage>(x => x.Status == RevisionStatus.Current);
		room.Setup(x => x.Terrain(null!)).Returns(terrain);
		room.SetupGet(x => x.CurrentOverlay).Returns(Mock.Of<IRoomOverlay>(x => x.Package == package));
		return room;
	}

	private sealed class PocketFrameFixture
	{
		public MagicCastingFixture Cast { get; } = new();
		public Mock<IGameItem> Source { get; } = new();
		public Mock<IGameItemProto> Prototype { get; } = new();
		public Mock<ISpellOwnedPocketService> Service { get; } = new();
		public Mock<IRoom> Fallback { get; } = SafeFallback();
		public List<IGameItem> Items { get; } = [];
		public List<IGameItem> Held { get; } = [];
		public SpellPocketConfiguration Configuration { get; } = new(7, 2, SizeCategory.VeryLarge, 300, SpellPocketAccess.Bearer, 4);
		public MagicSpell Spell { get; }
		public CreatePocketEffect Effect => Spell.SpellEffects.OfType<CreatePocketEffect>().Single();
		public MagicCastingIntent Intent { get; }
		public PocketFrameFixture()
		{
			Source.SetupGet(x => x.Id).Returns(9); Source.SetupGet(x => x.Quantity).Returns(1);
			Source.SetupGet(x => x.Components).Returns([]); Source.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
			Source.SetupGet(x => x.InInventoryOf).Returns(Cast.Body.Object);
			Source.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
			Source.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>(x => x.Id == 9));
			Items.Add(Source.Object); Held.Add(Source.Object);
			Cast.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => Items));
			Cast.Body.SetupGet(x => x.HeldItems).Returns(Held);
			Prototype.SetupGet(x => x.Id).Returns(7); Prototype.SetupGet(x => x.Components).Returns([]);
			var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>(); prototypes.Setup(x => x.Get(7)).Returns(Prototype.Object);
			Cast.World.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
			Fallback.SetupGet(x => x.Id).Returns(4);
			Cast.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { Fallback.Object }));
			Cast.World.SetupGet(x => x.SpellOwnedPockets).Returns(Service.Object);
			Service.Setup(x => x.AdmissionError(Cast.Actor.Object, Source.Object, Configuration, 1)).Returns((string?)null);
			var content = ArmageddonPocketContent.Create(Configuration);
			Spell = Cast.NewSpell(2, "Folded Pocket", content.BuildDefinition(11, 1, 0).Element("Effects")!.Elements().Single().ToString());
			Assert.IsTrue(Spell.BuildingCommand(Cast.Actor.Object, new StringStack("trigger new item")));
			Assert.IsTrue(Spell.BuildingCommand(Cast.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Cast.Earth.BuildingCommand(Cast.Actor.Object, new StringStack("casting entry add 2")));
			Cast.Store.Write(acquired: new(100, 2, 7, 1, Cast.Now, "fixture", DateTime.UnixEpoch, 0));
			Cast.Actor.Setup(x => x.TargetItem("focus")).Returns(Source.Object);
			Intent = new(Cast.Actor.Object, Cast.Earth.Id, 2, 1, false, "focus", OriginId: Guid.NewGuid());
		}
		public CreatePocketEffect PreparedEffect()
		{
			var copy = (MagicSpell)typeof(MagicSpell).GetMethod("CastingCopy", BindingFlags.NonPublic | BindingFlags.Instance)!
				.Invoke(Spell, [Cast.Actor.Object, Cast.Traits[0], 1, SpellPower.ExtremelyWeak, Difficulty.Automatic, 1])!;
			return copy.SpellEffects.OfType<CreatePocketEffect>().Single();
		}
		public void Change(string change)
		{
			switch (change)
			{
				case "quantity": Source.SetupGet(x => x.Quantity).Returns(2); break;
				case "deleted": Source.SetupGet(x => x.Deleted).Returns(true); break;
				case "registry": Items.Clear(); break;
				case "held": Held.Clear(); break;
				case "custody": Source.SetupGet(x => x.InInventoryOf).Returns(Mock.Of<IBody>()); break;
				case "prototype-revision": Prototype.SetupGet(x => x.RevisionNumber).Returns(1); break;
				case "fallback-overlay": Fallback.SetupGet(x => x.CurrentOverlay).Returns(Mock.Of<IRoomOverlay>()); break;
				case "body": Cast.Actor.SetupGet(x => x.Body).Returns(Mock.Of<IBody>()); break;
			}
		}
	}
}
