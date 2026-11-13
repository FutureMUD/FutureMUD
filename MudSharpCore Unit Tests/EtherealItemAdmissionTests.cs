#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Name;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.SpellEffects;
using MudSharp.Planes;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EtherealItemAdmissionTests
{
	private sealed class TargetCharacter : MudSharp.Character.Character
	{
		private TargetCharacter() : base(null!, null!, true) { }
		public Func<IPerceivable, bool> Visible { get; private set; } = null!;
		public static TargetCharacter Create(IRoom room, IBody body, Func<IPerceivable, bool> visible)
		{
			var actor = (TargetCharacter)RuntimeHelpers.GetUninitializedObject(typeof(TargetCharacter));
			actor.Location = room;
			actor.Body = body;
			actor.Visible = visible;
			return actor;
		}
		public override bool CanSee(IPerceivable thing, PerceiveIgnoreFlags flags = PerceiveIgnoreFlags.None) => Visible(thing);
		public override bool HasDubFor(IKeyworded target, string keyword) => false;
	}

	private sealed class Fixture
	{
		public MagicCastingFixture Cast { get; } = new();
		public Mock<IGameItem> Item { get; } = new();
		public Mock<IGameItem> Bag { get; } = new();
		public Mock<IRoom> Room { get; } = new();
		public Mock<ICharacter> Owner { get; } = new();
		public Mock<IBody> OwnerBody { get; } = new();
		public Mock<IOpenable> Openable { get; } = new();
		public List<IEffect> Effects { get; } = [];
		public bool Visible = true;
		public bool Open = true;
		public bool Accessible = true;
		public bool Colocated = true;
		public IGameItem? Container;
		public IBody? Holder;
		public TargetCharacter Lookup { get; }
		public MagicSpell Spell { get; }
		public string Target { get; }
		public int SightChecks;

		public Fixture(string placement = "floor")
		{
			Owner.As<IHavePersonalName>().SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>());
			Target = placement switch { "bag" => "bag@focus", "foreign" => "owner@focus", _ => "focus" };
			Cast.Actor.SetupGet(x => x.Location).Returns(Room.Object);
			Cast.Actor.Setup(x => x.CanSee(It.IsAny<IPerceivable>(), It.IsAny<PerceiveIgnoreFlags>()))
				.Returns<IPerceivable, PerceiveIgnoreFlags>((target, _) => { ++SightChecks; return !ReferenceEquals(target, Item.Object) || Visible; });
			Cast.Actor.Setup(x => x.ColocatedWith(Owner.Object)).Returns(() => Colocated);
			Room.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), Cast.Actor.Object)).Returns(() => Accessible);
			Cast.Body.SetupGet(x => x.ExternalItems).Returns([]);
			Item.SetupGet(x => x.Id).Returns(902);
			Item.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
			Item.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Item.SetupGet(x => x.TrueLocations).Returns([Room.Object]);
			Item.SetupGet(x => x.Location).Returns(() => Container is null && Holder is null ? Room.Object : null!);
			Item.SetupGet(x => x.ContainedIn).Returns(() => Container);
			Item.SetupGet(x => x.InInventoryOf).Returns(() => Holder);
			Item.SetupGet(x => x.Weight).Returns(2);
			Item.SetupGet(x => x.Effects).Returns(Effects);
			Item.Setup(x => x.EffectsOfType<EtherealItemAffinityEffect>(null)).Returns(() => Effects.OfType<EtherealItemAffinityEffect>());
			Item.Setup(x => x.AddEffect(It.IsAny<IEffect>())).Callback<IEffect>(Effects.Add);
			Item.Setup(x => x.RemoveEffect(It.IsAny<IEffect>(), It.IsAny<bool>())).Callback<IEffect, bool>((effect, _) => Effects.Remove(effect));
			Item.Setup(x => x.HasKeyword("focus", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
			Bag.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
			Bag.SetupGet(x => x.Location).Returns(Room.Object);
			Bag.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Bag.Setup(x => x.HasKeyword("bag", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
			var contents = Mock.Of<IContainer>(x => x.Contents == new[] { Item.Object });
			Bag.Setup(x => x.GetItemType<IContainer>()).Returns(contents);
			Bag.Setup(x => x.GetItemType<IOpenable>()).Returns(Openable.Object);
			Bag.Setup(x => x.GetItemType<ILiquidContainer>()).Returns((ILiquidContainer)null!);
			Bag.Setup(x => x.GetItemType<IDoor>()).Returns((IDoor)null!);
			Openable.SetupGet(x => x.IsOpen).Returns(() => Open);
			Owner.SetupGet(x => x.Body).Returns(OwnerBody.Object);
			Owner.SetupGet(x => x.Location).Returns(Room.Object);
			Owner.SetupGet(x => x.Gameworld).Returns(Cast.World.Object);
			Owner.SetupGet(x => x.PersonalName).Returns(Mock.Of<IPersonalName>());
			Owner.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			Owner.Setup(x => x.HasKeyword("owner", It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
			OwnerBody.SetupGet(x => x.Actor).Returns(Owner.Object);
			OwnerBody.SetupGet(x => x.BasePlanarPresence).Returns(PlanarPresenceDefinition.DefaultMaterial(1));
			OwnerBody.SetupGet(x => x.ExternalItemsForOtherActors).Returns([Item.Object]);
			Container = placement == "bag" ? Bag.Object : null;
			Holder = placement == "foreign" ? OwnerBody.Object : null;
			Room.SetupGet(x => x.Perceivables).Returns(placement switch
			{
				"bag" => [Bag.Object],
				"foreign" => [Owner.Object],
				_ => new IPerceivable[] { Item.Object }
			});
			Lookup = TargetCharacter.Create(Room.Object, Cast.Body.Object, target => Cast.Actor.Object.CanSee(target));
			// Keep native keyword/container parsing while using the configured casting fixture's actor and services.
			Cast.Actor.Setup(x => x.TargetItem(It.IsAny<string>())).Returns<string>(Lookup.TargetItem);
			Cast.Actor.Setup(x => x.TargetActorOrCorpse(It.IsAny<string>(), It.IsAny<PerceiveIgnoreFlags>()))
				.Returns<string, PerceiveIgnoreFlags>(Lookup.TargetActorOrCorpse);
			Cast.World.SetupGet(x => x.Items).Returns(MagicCastingFixture.Collection<IGameItem>(() => [Item.Object, Bag.Object]));
			Spell = Cast.NewSpell(2, "Item Attunement Admission", "<Effect type='etherealitemattunement'><PlaneId>9</PlaneId><AutoAdvance>true</AutoAdvance><MassPerGrade>3</MassPerGrade><RiskPercentPerGrade>0</RiskPercentPerGrade></Effect>");
			typeof(MagicSpell).GetProperty(nameof(MagicSpell.Trigger))!.SetValue(Spell,
				SpellTriggerFactory.LoadTrigger(XElement.Parse("<Trigger type='character'><MinimumPower>0</MinimumPower><MaximumPower>10</MaximumPower><CanTargetSelf>true</CanTargetSelf><TargetFilterProg>0</TargetFilterProg><CanTargetItems>true</CanTargetItems></Trigger>"), Spell));
			Assert.IsTrue(Spell.BuildingCommand(Cast.Actor.Object, new StringStack("grades fixture")));
			Assert.IsTrue(Cast.Earth.BuildingCommand(Cast.Actor.Object, new StringStack("casting entry add 2")));
			Cast.Store.Write(acquired: new(100, 2, 7, 1, Cast.Now, "fixture", DateTime.UnixEpoch, 0));
		}
		public MagicCastingResult Invoke() => Cast.Service.Cast(new(Cast.Actor.Object, Cast.Earth.Id, 2, 1, false, Target));
		public EtherealItemAttunementEffect PreparedEffect() => Spell.CastingCopy(Cast.Actor.Object, Cast.Traits[0], 1,
			SpellPower.ExtremelyWeak, Difficulty.Easy, 1).SpellEffects.OfType<EtherealItemAttunementEffect>().Single();
		public void AssertUnpaid(MagicCastingResult result)
		{
			Assert.AreEqual(MagicCastingStatus.Refused, result.Status, result.Message);
			Assert.IsNull(result.OperationId);
			Assert.AreEqual(100.0, Cast.Balances[Cast.Resources[1]]);
			Assert.AreEqual(0, Cast.Rolls);
			Assert.AreEqual(0, Cast.Store.Operations.Count);
			Assert.AreEqual(0, Effects.Count);
		}
	}

	[TestMethod]
	[DataRow("floor")]
	[DataRow("bag")]
	[DataRow("foreign")]
	public void NativeMixedCast_VisibleReachableItem_AttunesWithoutRequiringInventoryConsent(string placement)
	{
		var f = new Fixture(placement);
		Assert.AreSame(f.Item.Object, f.Lookup.TargetItem(f.Target));
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(95.0, f.Cast.Balances[f.Cast.Resources[1]]);
		Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsReadyForTransition);
		f.Owner.Verify(x => x.WillingToPermitInventoryManipulation(It.IsAny<ICharacter>()), Times.Never);
	}

	[TestMethod]
	[DataRow("hidden-child")]
	[DataRow("blocked-room")]
	[DataRow("distant-owner")]
	public void NativeMixedCast_SelectableButInaccessibleItem_RefusesBeforePayment(string scenario)
	{
		var f = new Fixture(scenario == "hidden-child" ? "bag" : scenario == "distant-owner" ? "foreign" : "floor");
		if (scenario == "hidden-child") f.Visible = false;
		if (scenario == "blocked-room") f.Accessible = false;
		if (scenario == "distant-owner") f.Colocated = false;
		Assert.AreSame(f.Item.Object, f.Lookup.TargetItem(f.Target), "Keyword selection alone does not establish admission.");
		f.AssertUnpaid(f.Invoke());
	}

	[TestMethod]
	[DataRow("hidden")]
	[DataRow("closed")]
	[DataRow("layer")]
	public void NativeMixedCast_NativeLookupCannotResolveItem_RefusesBeforePayment(string scenario)
	{
		var f = new Fixture(scenario == "closed" ? "bag" : "floor");
		if (scenario == "hidden") f.Visible = false;
		if (scenario == "closed") f.Open = false;
		if (scenario == "layer") f.Item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.InTrees);
		Assert.IsNull(f.Lookup.TargetItem(f.Target));
		f.AssertUnpaid(f.Invoke());
	}

	[TestMethod]
	[DataRow("hidden-child")]
	[DataRow("closed")]
	[DataRow("blocked-room")]
	[DataRow("distant-owner")]
	public void NativeMixedCast_AccessChangesBeforePayment_RefusesWithoutMutation(string scenario)
	{
		var f = new Fixture(scenario is "hidden-child" or "closed" ? "bag" : scenario == "distant-owner" ? "foreign" : "floor");
		var changed = false;
		f.Cast.Checkpoint = stage =>
		{
			if (stage != "BeforePayment") return;
			changed = true;
			if (scenario == "hidden-child") f.Visible = false;
			if (scenario == "closed") f.Open = false;
			if (scenario == "blocked-room") f.Accessible = false;
			if (scenario == "distant-owner") f.Colocated = false;
		};
		f.AssertUnpaid(f.Invoke());
		Assert.IsTrue(changed);
	}

	[TestMethod]
	[DataRow("capture")]
	[DataRow("confirm")]
	[DataRow("reuse")]
	public void PreparedSelection_ReachCallbackClosesContainer_RejectsChangedAncestry(string phase)
	{
		var f = new Fixture("bag");
		var effect = f.PreparedEffect();
		var token = phase == "capture" ? null : effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		f.Room.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), f.Cast.Actor.Object))
			.Callback(() => f.Open = false).Returns(true);
		if (phase == "capture")
			Assert.ThrowsExactly<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		else
			Assert.IsFalse(phase == "confirm"
				? effect.TryConfirmPreparedSelection(f.Cast.Actor.Object, f.Item.Object, out _)
				: f.PreparedEffect().TryReusePreparedSelection(token!, f.Cast.Actor.Object, f.Item.Object, out _));
		Assert.IsFalse(f.Open, "The native reach callback must actually run.");
		Assert.AreEqual(0, f.Effects.Count);
	}

	[TestMethod]
	public void NativeMixedCast_GlowPrecedesAttunement_AppliesBothEffectsForOnePayment()
	{
		var f = new Fixture();
		f.Cast.World.Setup(x => x.LightModel.GetIlluminationDescription(It.IsAny<double>())).Returns("dim");
		f.Item.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, _) => f.Effects.Add(effect));
		var glow = SpellEffectFactory.LoadEffect(XElement.Parse("<Effect type='glow'><GlowLuxPerPower>10</GlowLuxPerPower><SDescAddendum>(glowing)</SDescAddendum><DescAddendum>A white glow.</DescAddendum><GlowAddendumColour>bold white</GlowAddendumColour></Effect>"), f.Spell);
		((List<IMagicSpellEffectTemplate>)f.Spell.SpellEffects).Insert(0, glow);
		var result = f.Invoke();
		Assert.AreEqual(MagicCastingStatus.Succeeded, result.Status, result.Message);
		Assert.AreEqual(95.0, f.Cast.Balances[f.Cast.Resources[1]]);
		Assert.AreEqual(1, f.Effects.OfType<SpellGlowEffect>().Count());
		Assert.IsTrue(f.Effects.OfType<EtherealItemAffinityEffect>().Single().IsReadyForTransition);
	}

	[TestMethod]
	[DataRow("capture")]
	[DataRow("confirm")]
	[DataRow("reuse")]
	public void PreparedSelection_ReachCallbackHidesItem_RejectsBeforePayment(string phase)
	{
		var f = new Fixture();
		var effect = f.PreparedEffect();
		var token = phase == "capture" ? null : effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		f.Room.Setup(x => x.CanGetAccess(It.IsAny<IGameItem>(), f.Cast.Actor.Object))
			.Callback(() => f.Visible = false).Returns(true);
		if (phase == "capture")
			Assert.ThrowsExactly<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
		else
			Assert.IsFalse(phase == "confirm"
				? effect.TryConfirmPreparedSelection(f.Cast.Actor.Object, f.Item.Object, out _)
				: f.PreparedEffect().TryReusePreparedSelection(token!, f.Cast.Actor.Object, f.Item.Object, out _));
		Assert.IsFalse(f.Visible);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PreparedSelection_CyclicContainment_RejectsBeforeRecursiveLocationReads(bool afterCapture)
	{
		var f = new Fixture("bag");
		var effect = f.PreparedEffect();
		var token = afterCapture ? (IMagicSpellEffectPreparedSelectionRawToken)effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object) : null;
		f.Bag.SetupGet(x => x.ContainedIn).Returns(f.Item.Object);
		f.Item.SetupGet(x => x.Location).Throws(new AssertFailedException("Cyclic containment must be rejected before reading recursive location properties."));
		if (afterCapture) Assert.IsFalse(token!.IsCurrent);
		else Assert.ThrowsExactly<InvalidOperationException>(() => effect.CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object));
	}

	[TestMethod]
	public void PreparedSelection_RawFence_DetectsContainerClosureWithoutSightOrReachCallbacks()
	{
		var f = new Fixture("bag");
		var token = (IMagicSpellEffectPreparedSelectionRawToken)f.PreparedEffect().CapturePreparedSelection(f.Cast.Actor.Object, f.Item.Object);
		var sightChecks = f.SightChecks;
		f.Room.Invocations.Clear();
		Assert.IsTrue(token.IsCurrent);
		f.Open = false;
		Assert.IsFalse(token.IsCurrent);
		Assert.AreEqual(sightChecks, f.SightChecks);
		f.Room.Verify(x => x.CanGetAccess(It.IsAny<IGameItem>(), It.IsAny<ICharacter>()), Times.Never);
	}
}
