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
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Casting;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPocketTests
{
	[TestMethod]
	public void PocketPlacement_PublishesExactCommittedInstanceBeforeHandCustody()
	{
		var world = new Mock<IFuturemud>();
		var carrier = Item(700);
		var body = new Mock<IBody>();
		var actor = Mock.Of<ICharacter>(x => x.Body == body.Object);
		var cache = new All<IGameItem>();
		world.Setup(x => x.Add(carrier.Object)).Callback(() => cache.Add(carrier.Object));
		body.Setup(x => x.CanGet(carrier.Object, 0, ItemCanGetIgnore.None)).Returns(() =>
		{
			Assert.AreSame(carrier.Object, cache.Get(carrier.Object.Id));
			return true;
		});
		new SpellOwnedPocketService(world.Object).PlaceCommittedCarrier(carrier.Object, actor);
		world.Verify(x => x.Add(carrier.Object), Times.Once);
		body.Verify(x => x.Get(carrier.Object, 0, null, true, ItemCanGetIgnore.None), Times.Once);
		Assert.AreSame(carrier.Object, cache.Get(carrier.Object.Id));
	}
	private static Mock<IGameItem> Item(long id, double weight = 1)
	{
		var item = new Mock<IGameItem>(); item.SetupGet(x => x.Id).Returns(id); item.SetupGet(x => x.Weight).Returns(weight);
		item.SetupGet(x => x.Size).Returns(SizeCategory.Small); item.SetupGet(x => x.Quantity).Returns(1);
		item.SetupGet(x => x.Components).Returns([]); item.SetupGet(x => x.AttachedAndConnectedItems).Returns([]); item.SetupGet(x => x.LodgedItems).Returns([]);
		item.SetupProperty(x => x.ContainedIn); return item;
	}
	private static (Mock<IGameItem> Carrier, Mock<ISpellPocket> Pocket, Mock<ISpellOwnedPocketService> Service) Pocket(double capacity, params IGameItem[] contents)
	{
		var carrier = Item(1); var pocket = new Mock<ISpellPocket>(); var world = new Mock<IFuturemud>();
		var service = new Mock<ISpellOwnedPocketService>(); world.SetupGet(x => x.SpellOwnedPockets).Returns(service.Object);
		service.Setup(x => x.IsActive(carrier.Object)).Returns(true); carrier.SetupGet(x => x.Gameworld).Returns(world.Object);
		carrier.Setup(x => x.GetItemType<ISpellPocket>()).Returns(pocket.Object);
		pocket.SetupGet(x => x.Parent).Returns(carrier.Object);
		pocket.SetupGet(x => x.Anchor).Returns(new SpellPocketAnchor(9, 1, new(7, capacity, SizeCategory.Normal, 30, SpellPocketAccess.Bearer, 4)));
		pocket.SetupGet(x => x.Contents).Returns(contents); carrier.SetupGet(x => x.Components).Returns([pocket.Object]);
		foreach (var item in contents) item.ContainedIn = carrier.Object;
		return (carrier, pocket, service);
	}
	[TestMethod]
	public void PocketCapacity_ExactBoundaryAdmits_OverflowAndExpiredPocketRefuse()
	{
		var stored = Item(2, 4); var f = Pocket(10, stored.Object);
		Assert.IsNull(SpellPocketContainment.AdmissionError(Item(3, 6).Object, f.Carrier.Object));
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(3, 6.01).Object, f.Carrier.Object));
		f.Service.Setup(x => x.IsActive(f.Carrier.Object)).Returns(false);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(3).Object, f.Carrier.Object));
	}
	[TestMethod]
	public void NestedBag_DirectInsertionStillUsesOuterPocketCapacityAndSize()
	{
		var bag = Item(2, 4); var container = new Mock<IContainer>(); container.SetupGet(x => x.Contents).Returns([]);
		bag.SetupGet(x => x.Components).Returns([container.Object]); var f = Pocket(10, bag.Object);
		Assert.IsNull(SpellPocketContainment.AdmissionError(Item(3, 6).Object, bag.Object));
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(3, 7).Object, bag.Object));
		var huge = Item(4); huge.SetupGet(x => x.Size).Returns(SizeCategory.Titanic);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(huge.Object, bag.Object));
	}
	[TestMethod]
	public void PocketAdmission_IncomingAliasOfStoredIdentityRefuses_ExistingInstanceCanMove()
	{
		var stored = Item(2);
		var f = Pocket(20, stored.Object);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(2).Object, f.Carrier.Object));
		Assert.IsNull(SpellPocketContainment.AdmissionError(stored.Object, f.Carrier.Object));
	}
	[TestMethod]
	public void PocketNesting_DirectAndHiddenInsideOrdinaryBagRefuse()
	{
		var f = Pocket(20); var inner = Item(3); inner.Setup(x => x.IsItemType<ISpellPocket>()).Returns(true);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(inner.Object, f.Carrier.Object));
		var bag = Item(4); var container = new Mock<IContainer>(); container.SetupGet(x => x.Contents).Returns([inner.Object]);
		bag.SetupGet(x => x.Components).Returns([container.Object]);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(bag.Object, f.Carrier.Object));
	}
	[TestMethod]
	public void PocketGraph_SelfCyclesAndDuplicatePersistentIdentitiesRefuse()
	{
		var f = Pocket(20); Assert.IsNotNull(SpellPocketContainment.AdmissionError(f.Carrier.Object, f.Carrier.Object));
		var bag = Item(3); var container = new Mock<IContainer>(); container.SetupGet(x => x.Contents).Returns([bag.Object]); bag.SetupGet(x => x.Components).Returns([container.Object]);
		Assert.IsFalse(SpellPocketContainment.TryGraph([bag.Object], out _, out _));
		Assert.IsFalse(SpellPocketContainment.TryGraph([Item(4).Object, Item(4).Object], out _, out _));
	}
	[TestMethod]
	public void NestedAccess_NullActorAndUnauthorizedActorCannotBypassOuterPocket()
	{
		var bag = Item(2); var f = Pocket(20, bag.Object); var actor = Mock.Of<ICharacter>();
		f.Pocket.Setup(x => x.CanAccess(actor)).Returns(false);
		Assert.IsFalse(SpellPocketContainment.CanAccess(actor, bag.Object)); Assert.IsFalse(SpellPocketContainment.CanAccess(null, bag.Object));
		f.Pocket.Setup(x => x.CanAccess(actor)).Returns(true); Assert.IsTrue(SpellPocketContainment.CanAccess(actor, bag.Object));
		Assert.IsTrue(SpellPocketContainment.CanAccess(null, Item(10).Object));
	}
	[TestMethod]
	public void PocketGraph_BoundedTraversalPreservesOrdinaryContainerChildren()
	{
		Assert.IsFalse(SpellPocketContainment.TryGraph(Enumerable.Range(1, 257).Select(i => Item(i).Object), out _, out _));
		var child = Item(3); var bag = Item(2); var container = new Mock<IContainer>(); container.SetupGet(x => x.Contents).Returns([child.Object]);
		bag.SetupGet(x => x.Components).Returns([container.Object]); child.Object.ContainedIn = bag.Object;
		Assert.IsTrue(SpellPocketContainment.TryGraph([bag.Object], out var graph, out _)); CollectionAssert.AreEquivalent(new[] { bag.Object, child.Object }, graph);
	}
	[TestMethod]
	public void PocketAdmission_InvalidWeightsMorphsAndAliasedCustodyRefuse()
	{
		var f = Pocket(20);
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(3, double.NaN).Object, f.Carrier.Object));
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(Item(3, -1).Object, f.Carrier.Object));
		var morph = Item(3); morph.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>(x => x.Morphs));
		Assert.IsNotNull(SpellPocketContainment.AdmissionError(morph.Object, f.Carrier.Object));
		var item = Item(4).Object; Assert.IsFalse(SpellPocketContainment.TryGraph([item, item], out _, out _));
	}
	[DataTestMethod]
	[DataRow("paid")][DataRow("refused")][DataRow("drift")]
	public void PocketCasting_AdmitsBeforePaymentAndCreatesOnceAfterPayment(string scenario)
	{
		var f = new MagicCastingFixture(); var source = Item(9); var service = new Mock<ISpellOwnedPocketService>();
		source.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		source.SetupGet(x => x.BasePlanarPresence).Returns(MudSharp.Planes.PlanarPresenceDefinition.DefaultMaterial(1));
		source.SetupGet(x => x.Prototype).Returns(Mock.Of<IGameItemProto>(x => x.Id == 9));
		var allowed = scenario != "refused";
		f.World.SetupGet(x => x.SpellOwnedPockets).Returns(service.Object);
		var config = new SpellPocketConfiguration(7, 10, SizeCategory.Normal, 30, SpellPocketAccess.Bearer, 4);
		service.Setup(x => x.AdmissionError(f.Actor.Object, source.Object, config, 1)).Returns(() => allowed ? null : "Unsuitable focus");
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		var prototype = Mock.Of<IGameItemProto>(x => x.Id == 7);
		prototypes.Setup(x => x.Get(7)).Returns(prototype);
		f.World.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		var fallback = Mock.Of<MudSharp.Construction.IRoom>(x => x.Id == 4);
		f.World.SetupGet(x => x.Rooms).Returns(MagicCastingFixture.Collection(() => new[] { fallback }));
		var content = ArmageddonPocketContent.Create(config);
		var spell = f.NewSpell(2, "Folded Pocket", content.BuildDefinition(11, 1, 0).Element("Effects")!.Elements().Single().ToString());
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("trigger new item")));
		Assert.IsTrue(spell.BuildingCommand(f.Actor.Object, new StringStack("grades fixture")));
		Assert.IsTrue(f.Earth.BuildingCommand(f.Actor.Object, new StringStack("casting entry add 2")));
		f.Store.Write(acquired: new(100, 2, 7, 1, f.Now, "fixture", DateTime.UnixEpoch, 0));
		f.Actor.Setup(x => x.TargetItem("focus")).Returns(source.Object);
		f.Checkpoint = stage => { if (scenario == "drift" && stage == "BeforePayment") allowed = false; };
		var intent = new MagicCastingIntent(f.Actor.Object, f.Earth.Id, 2, 1, false, "focus", OriginId: Guid.NewGuid());
		var result = f.Service.Cast(intent);
		Assert.AreEqual(scenario == "paid" ? MagicCastingStatus.Succeeded : MagicCastingStatus.Refused, result.Status, result.Message);
		Assert.AreEqual(scenario == "paid" ? 95.0 : 100.0, f.Balances[f.Resources[1]]);
		service.Verify(x => x.Create(f.Actor.Object, source.Object, config, It.Is<SpellLifecycleOrigin>(o => o.Grade == 1 && o.Provenance == new SpellPocketAnchor(9, 1, config).Save())),
			scenario == "paid" ? Times.Once() : Times.Never());
		if (scenario == "paid") { Assert.AreEqual(MagicCastingStatus.Refused, f.Service.Cast(intent).Status); Assert.AreEqual(95.0, f.Balances[f.Resources[1]]); }
	}
	[TestMethod]
	public void PocketComponent_CopyDoesNotDuplicateBindingOrAccess()
	{
		var world = new Mock<IFuturemud>(); var carrier = Item(1); carrier.SetupGet(x => x.Gameworld).Returns(world.Object);
		var proto = (FoldedPocketGameItemComponentProto)typeof(FoldedPocketGameItemComponentProto).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([new MudSharp.Models.GameItemComponentProto
			{ Id = 7, Name = "pocket", Description = "", Definition = "<Definition Weight=\"100\" MaxSize=\"6\" />", EditableItem = new() }, world.Object]);
		var component = new FoldedPocketGameItemComponent(proto, carrier.Object, temporary: true);
		var anchor = new SpellPocketAnchor(9, 2, new(8, 100, SizeCategory.Normal, 30, SpellPocketAccess.Bearer, 4));
		var now = DateTime.UtcNow;
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 2, 10, SpellPocketAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(60), anchor.Save());
		component.Bind(origin, anchor); Assert.AreEqual(origin.Id, component.LifecycleId);
		Assert.ThrowsException<InvalidOperationException>(() => component.Bind(origin, anchor));
		var copy = (FoldedPocketGameItemComponent)component.Copy(Item(2).Object, true);
		Assert.AreEqual(Guid.Empty, copy.LifecycleId); Assert.IsNull(copy.Anchor); Assert.IsFalse(copy.CanPut(Item(3).Object));
	}
	[DataTestMethod]
	[DataRow(SpellPocketAccess.Bearer, true)][DataRow(SpellPocketAccess.Creator, false)]
	public void RetiringPocket_WithdrawalRemainsAuthorized_DepositsAndForeignCreatorRefuse(SpellPocketAccess access, bool otherAllowed)
	{
		var world = new Mock<IFuturemud>(); var carrier = Item(1); carrier.SetupGet(x => x.Gameworld).Returns(world.Object);
		var service = new Mock<ISpellOwnedPocketService>(); world.SetupGet(x => x.SpellOwnedPockets).Returns(service.Object);
		service.Setup(x => x.CanWithdraw(carrier.Object)).Returns(true); service.Setup(x => x.IsActive(carrier.Object)).Returns(false);
		var proto = (FoldedPocketGameItemComponentProto)typeof(FoldedPocketGameItemComponentProto).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!.Invoke([new MudSharp.Models.GameItemComponentProto
			{ Id = 7, Name = "pocket", Description = "", Definition = "<Definition Weight=\"100\" MaxSize=\"6\" />", EditableItem = new() }, world.Object]);
		var component = new FoldedPocketGameItemComponent(proto, carrier.Object, temporary: true);
		var anchor = new SpellPocketAnchor(9, 1, new(8, 100, SizeCategory.Normal, 30, access, 4)); var now = DateTime.UtcNow;
		var origin = new SpellLifecycleOrigin(Guid.NewGuid(), 1, 1, 50, SpellPocketAnchor.Family, SpellLifecycleMode.TemporaryCleanup, now, now.AddSeconds(30), anchor.Save());
		component.Bind(origin, anchor); carrier.Setup(x => x.GetItemType<ISpellPocket>()).Returns(component);
		carrier.SetupGet(x => x.SpellCreationOrigin).Returns(new SpellOwnedItemOrigin(origin.Id, origin.Mode, origin.DeadlineUtc, CreatorId: 50));
		var creator = Mock.Of<ICharacter>(x => x.Id == 50 && x.Gameworld == world.Object);
		var other = Mock.Of<ICharacter>(x => x.Id == 51 && x.Gameworld == world.Object);
		Assert.IsTrue(component.CanAccess(creator)); Assert.AreEqual(otherAllowed, component.CanAccess(other));
		Assert.IsFalse(component.CanPut(Item(3).Object)); Assert.IsFalse(component.SwapInPlace(Item(3).Object, Item(4).Object));
		Assert.IsFalse(component.InstallLock(Mock.Of<ILock>()));
		service.Setup(x => x.CanWithdraw(carrier.Object)).Returns(false); Assert.IsFalse(component.CanAccess(creator));
	}
	[TestMethod]
	public void TetherItemReferences_UseExactCodecsAndDoNotMatchCharacterNumbersOrUnknownFields()
	{
		var payload = "<Effects><Effect><Type>ZeroGravityTether</Type><Effect><AnchorType>GameItem</AnchorType><AnchorId>17</AnchorId><PhysicalTetherId>23</PhysicalTetherId><Unrelated>99</Unrelated></Effect></Effect></Effects>";
		var refs = MudSharp.Character.PhysicalReferenceCodecs.Effects(payload).ToArray();
		Assert.IsTrue(refs.Any(x => x.Kind == PhysicalEntityKind.GameItem && x.Id == 17));
		Assert.IsTrue(refs.Any(x => x.Kind == PhysicalEntityKind.GameItem && x.Id == 23));
		Assert.IsFalse(refs.Any(x => x.Kind == PhysicalEntityKind.Character || x.Id == 99));
		var live = PhysicalEntityReference.FromItem(Item(17).Object, "anchor").Single();
		Assert.AreEqual(PhysicalEntityKind.GameItem, live.Kind); Assert.AreEqual(17L, live.Id);
	}
}
