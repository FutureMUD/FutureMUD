#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Magic.SpellEffects;
using MudSharp.Magic.SpellTriggers;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PublicTravelAnchorTests
{
	private sealed class Fixture
	{
		public MagicCastingFixture Magic { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public PublicTravelAnchor Anchor { get; }
		public PerceptionTypes Sense { get; set; } = PerceptionTypes.SenseMagical;
		public Fixture()
		{
			Magic.Body.SetupGet(x => x.Actor).Returns(Magic.Actor.Object);
			Magic.Actor.Setup(x => x.GetPerception(It.IsAny<PerceptionTypes>())).Returns(() => Sense);
			Room.SetupGet(x => x.Id).Returns(301);
			Room.SetupGet(x => x.Gameworld).Returns(Magic.World.Object);
			Item.SetupGet(x => x.Id).Returns(201);
			Item.SetupGet(x => x.Gameworld).Returns(Magic.World.Object);
			Item.SetupGet(x => x.Effects).Returns(Effects);
			Item.SetupGet(x => x.Location).Returns(Room.Object);
			Item.SetupGet(x => x.TrueLocations).Returns([Room.Object]);
			Item.SetupGet(x => x.Keywords).Returns(["quetalz"]);
			Magic.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection(() => new[] { Item.Object }));
			Magic.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { Room.Object }));
			Anchor = new PublicTravelAnchor(Item.Object);
			Effects.Add(Anchor);
		}
	}

	[TestMethod]
	public void PublicRune_DetectionControlsVisibilityAndGlobalKeywordAdmissionWithoutOwnership()
	{
		var f = new Fixture();
		Assert.IsFalse(f.Item.Object.IsOwnedBy(f.Magic.Actor.Object));
		Assert.IsFalse(f.Item.Object.HasKeyword("quetalz", f.Magic.Actor.Object, true), "Perceived descriptions need not expose a remote rune's keywords.");
		Assert.IsTrue(f.Anchor.AllowsTravel(f.Magic.Actor.Object));
		Assert.IsFalse(f.Anchor.Applies(f.Magic.Actor.Object, PerceiveIgnoreFlags.None));
		Assert.AreSame(f.Item.Object, TravelRuneSelector.Resolve(f.Magic.Actor.Object, "quetalz"));
		Assert.IsNull(TravelRuneSelector.Resolve(f.Magic.Actor.Object, "#201"));
		Assert.IsNull(TravelRuneSelector.Resolve(f.Magic.Actor.Object, "201"));
		f.Sense = PerceptionTypes.VisualMagical;
		Assert.IsFalse(f.Anchor.AllowsTravel(f.Magic.Actor.Object));
		Assert.IsTrue(f.Anchor.Applies(f.Magic.Actor.Object, PerceiveIgnoreFlags.None));
		Assert.IsNull(TravelRuneSelector.Resolve(f.Magic.Actor.Object, "quetalz"));
	}

	[TestMethod]
	[DataRow("carried")]
	[DataRow("contained")]
	[DataRow("deleted")]
	[DataRow("temporary")]
	[DataRow("removed")]
	[DataRow("sense")]
	public void PublicRune_PreparedAnchorRejectsLostAdmissionOrPlacement(string change)
	{
		var f = new Fixture();
		Assert.IsTrue(SpellTravelAnchorResolver.TryResolve(f.Magic.Actor.Object, f.Item.Object,
			SpellTravelAnchorMode.AnchorRuneLiveRoom, out var anchor, out var error), error);
		switch (change)
		{
			case "carried": f.Item.SetupGet(x => x.InInventoryOf).Returns(f.Magic.Body.Object); break;
			case "contained": f.Item.SetupGet(x => x.ContainedIn).Returns(Mock.Of<IGameItem>()); break;
			case "deleted": f.Item.SetupGet(x => x.Deleted).Returns(true); break;
			case "temporary": f.Room.SetupGet(x => x.Temporary).Returns(true); break;
			case "removed": f.Effects.Clear(); break;
			case "sense": f.Sense = PerceptionTypes.None; break;
		}
		Assert.IsFalse(anchor!.IsCurrentFor(f.Magic.Actor.Object));
	}

	[TestMethod]
	public void PublicRune_PersistsDetectionPolicyAndNeverAuthorisesAnArcaneMark()
	{
		var f = new Fixture();
		PublicTravelAnchor.InitialiseEffectType();
		var saved = f.Anchor.SaveToXml(new Dictionary<IEffect, TimeSpan>());
		var loaded = (PublicTravelAnchor)Effect.LoadEffect(saved, f.Item.Object);
		f.Effects.Clear(); f.Effects.Add(loaded);
		Assert.AreEqual(PerceptionTypes.SenseMagical, loaded.RequiredPerception);
		Assert.IsTrue(loaded.IsActive);
		Assert.IsTrue(loaded.AllowsTravel(f.Magic.Actor.Object));
		Assert.IsFalse(SpellTravelAnchorResolver.TryResolve(f.Magic.Actor.Object, f.Item.Object,
			SpellTravelAnchorMode.ArcaneMarkedRoom, out _, out _));
	}
}
