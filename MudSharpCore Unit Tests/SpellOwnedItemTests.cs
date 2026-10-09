#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Economy;
using MudSharp.Economy.Auctions;
using MudSharp.Economy.Shops;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.GameItems.Prototypes;
using MudSharp.Magic;
using MudSharp.Magic.Lifecycle;
using MudSharp.Magic.SpellEffects;
using MudSharp.PerceptionEngine.Lists;
using MudSharp.Work.Crafts.Inputs;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SpellOwnedItemTests
{
	[TestMethod]
	public void LifecycleLookup_OrdinaryItems_ReadClaimsOnceWithoutPerItemDatabaseAccess()
	{
		var reads = 0;
		var service = new SpellOwnedItemService(Mock.Of<IFuturemud>(), () =>
		{
			reads++;
			return new HashSet<long> { 10001 };
		});
		// No FMDB has been configured: reaching either per-item query would fail this test.
		for (var id = 1; id <= 10000; id++)
		{
			Assert.IsNull(service.FindOrigin(id));
			Assert.IsFalse(service.IsActivationPending(id));
		}
		Assert.AreEqual(1, reads);
	}

	[TestMethod]
	public void LifecycleLookup_InvalidItemIds_DoNotInitialiseClaims()
	{
		var service = new SpellOwnedItemService(Mock.Of<IFuturemud>(), () =>
			throw new AssertFailedException("Invalid IDs must not access storage."));
		Assert.IsNull(service.FindOrigin(0));
		Assert.IsNull(service.FindOrigin(-1));
		Assert.IsFalse(service.IsActivationPending(0));
		Assert.IsFalse(service.IsActivationPending(-1));
	}

	[TestMethod]
	public void GameItem_PrototypeConstructor_PreservesPublicThreeArgumentSignatureAndDefaults()
	{
		var signature = new[] { typeof(IGameItemProto), typeof(ICharacter), typeof(ItemQuality) };
		var constructor = typeof(GameItem).GetConstructor(signature);
		Assert.IsNotNull(constructor, "Existing binaries require the public three-argument constructor.");
		Assert.IsTrue(constructor.IsPublic);
		Assert.IsNull(constructor.GetParameters()[1].DefaultValue);
		Assert.AreEqual(ItemQuality.Standard, constructor.GetParameters()[2].DefaultValue);
		Assert.IsNull(typeof(GameItem).GetConstructor([.. signature, typeof(bool)]));
		var deferred = typeof(GameItem).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
			null, [.. signature, typeof(bool)], null);
		Assert.IsNotNull(deferred);
		Assert.IsTrue(deferred.IsAssembly);
	}

	private static Mock<IGameItem> Temporary()
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.SpellCreationOrigin).Returns(new SpellOwnedItemOrigin(Guid.NewGuid(), SpellLifecycleMode.TemporaryCleanup, DateTime.UtcNow.AddMinutes(1)));
		item.SetupGet(x => x.DeepItems).Returns(() => [item.Object]);
		return item;
	}

	[DataTestMethod]
	[DataRow("nested")]
	[DataRow("attached")]
	[DataRow("lodged")]
	public void ValuePolicy_ForeignHostOfTemporaryItem_BlocksConversionWithoutMutatingCustody(string topology)
	{
		var item = Temporary(); var host = new Mock<IGameItem>();
		host.SetupGet(x => x.DeepItems).Returns(topology == "nested" ? [host.Object, item.Object] : [host.Object]);
		host.SetupGet(x => x.AttachedAndConnectedItems).Returns(topology == "attached" ? [item.Object] : []);
		host.SetupGet(x => x.LodgedItems).Returns(topology == "lodged" ? [item.Object] : []);
		Assert.IsTrue(SpellOwnedItemValuePolicy.ContainsTemporaryValue(new PerceivableGroup([host.Object])));
		Assert.ThrowsException<InvalidOperationException>(() => SpellOwnedItemValuePolicy.RequireOrdinaryValue(host.Object, "conversion"));
		host.Verify(x => x.Delete(), Times.Never); item.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void ValuePolicy_OrdinaryAndPaidPermanentGoods_AreUnrestrictedAndCyclesTerminate()
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.SpellCreationOrigin).Returns(new SpellOwnedItemOrigin(Guid.NewGuid(), SpellLifecycleMode.Permanent, null));
		item.SetupGet(x => x.DeepItems).Returns([item.Object]);
		item.SetupGet(x => x.AttachedAndConnectedItems).Returns([item.Object]);
		Assert.IsFalse(SpellOwnedItemValuePolicy.ContainsTemporaryValue(item.Object));
		SpellOwnedItemValuePolicy.RequireOrdinaryValue(item.Object, "conversion");
		Assert.IsFalse(SpellOwnedItemValuePolicy.ContainsTemporaryValue(Mock.Of<IGameItem>()));
	}

	[TestMethod]
	public void Salvage_TemporaryLeaf_RefusesBothAdmissionAndDirectProductFactory()
	{
		var proto = (SalvageableGameItemComponentProto)RuntimeHelpers.GetUninitializedObject(typeof(SalvageableGameItemComponentProto));
		var component = new SalvageableGameItemComponent(proto, Temporary().Object, temporary: true);
		Assert.IsFalse(component.CanSalvage(out var reason));
		StringAssert.Contains(reason, "permanent value");
		Assert.ThrowsException<InvalidOperationException>(() => component.CreateProducts(Mock.Of<ICharacter>(), true));
		Assert.AreEqual("A temporary weapon.", component.Decorate(null!, "weapon", "A temporary weapon.", DescriptionType.Full, false, default));
	}

	[TestMethod]
	public void Shop_TemporaryLeaf_RefusesQuoteAndDirectSaleBeforeAnyPaymentOrCustody()
	{
		var shop = (PermanentShop)RuntimeHelpers.GetUninitializedObject(typeof(PermanentShop)); var item = Temporary();
		Assert.IsFalse(shop.CanSell(null!, null!, null!, item.Object).Truth);
		Assert.ThrowsException<InvalidOperationException>(() => shop.Sell(null!, null!, null!, item.Object));
		Assert.ThrowsException<InvalidOperationException>(() => shop.AddToStock(null!, item.Object, null!));
		Assert.IsFalse(shop.CanBuyExact(null!, null!, 1, null!, [item.Object]).Truth);
		item.Verify(x => x.SetOwner(It.IsAny<IFrameworkItem>()), Times.Never);
	}

	[TestMethod]
	public void Auction_TemporaryLeaf_RefusesDirectListingBeforeAddingLot()
	{
		var auction = (AuctionHouse)RuntimeHelpers.GetUninitializedObject(typeof(AuctionHouse));
		Assert.ThrowsException<InvalidOperationException>(() => auction.AddAuctionItem(new AuctionItem { Asset = Temporary().Object }));
	}

	[TestMethod]
	public void CraftReservation_TemporaryInputInLaterPosition_RefusesBeforeConsumingEarlierOrdinaryInput()
	{
		var ordinary = new Mock<IGameItem>(); var temporary = Temporary();
		Assert.ThrowsException<InvalidOperationException>(() => new BaseInput.SimpleItemInputData([ordinary.Object, temporary.Object], 2));
		ordinary.Verify(x => x.Quit(), Times.Never); temporary.Verify(x => x.Quit(), Times.Never);
	}

	[TestMethod]
	public void CreationComponentScout_TemporaryCandidate_IsSkippedBeforeOrdinaryMaterial()
	{
		var temporary = Temporary(); temporary.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var ordinary = new Mock<IGameItem>(); ordinary.Setup(x => x.IsA(It.IsAny<ITag>())).Returns(true);
		var body = new Mock<IBody>(); body.SetupGet(x => x.HeldItems).Returns([temporary.Object, ordinary.Object]);
		var actor = Mock.Of<ICharacter>(x => x.Body == body.Object);
		var world = Mock.Of<IFuturemud>(x => x.Tags == new All<ITag>());
		var action = new InventoryPlanActionConsume(world, 1, 0, 0, _ => true, _ => true);
		Assert.AreSame(ordinary.Object, action.ScoutTarget(actor));
		temporary.Verify(x => x.Delete(), Times.Never);
	}

	[TestMethod]
	public void NativeDestruction_RefusedRemoval_DoesNotMarkDestroyedOrCallDeathObservers()
	{
		var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		typeof(GameItem).GetProperty(nameof(GameItem.SpellCreationOrigin))!.SetValue(item, Temporary().Object.SpellCreationOrigin);
		var service = new Mock<ISpellOwnedItemService>();
		var world = Mock.Of<IFuturemud>(x => x.SpellOwnedItems == service.Object);
		typeof(GameItem).GetProperty(nameof(GameItem.Gameworld))!.SetValue(item, world);
		var callbacks = 0; item.OnDeath += _ => callbacks++;
		Assert.AreSame(item, item.Die());
		Assert.IsFalse(item.Deleted); Assert.IsFalse(item.Destroyed); Assert.AreEqual(0, callbacks);
	}

	[TestMethod]
	public void GameItem_TemporaryCopyMergeAndReplacementFactories_ConserveValueBeforeSideEffects()
	{
		var item = (GameItem)RuntimeHelpers.GetUninitializedObject(typeof(GameItem));
		typeof(GameItem).GetProperty(nameof(GameItem.SpellCreationOrigin))!.SetValue(item, Temporary().Object.SpellCreationOrigin);
		Assert.IsFalse(item.CanMerge(Mock.Of<IGameItem>()));
		Assert.ThrowsException<InvalidOperationException>(() => new GameItem(item));
		Assert.ThrowsException<InvalidOperationException>(() => item.Merge(Mock.Of<IGameItem>()));
		Assert.IsFalse(item.CheckPrototypeForUpdate());
		var proto = (GameItemProto)RuntimeHelpers.GetUninitializedObject(typeof(GameItemProto));
		Assert.IsNull(proto.LoadDestroyedItem(item)); Assert.IsNull(proto.LoadMorphedItem(item));
	}

	[DataTestMethod]
	[DataRow("99", "TemporaryCleanup", "7")]
	[DataRow("1", "DeathOnExpiry", "7")]
	[DataRow("1", "TemporaryCleanup", "8")]
	[DataRow("1", "TemporaryCleanup", "not-a-grade")]
	public void Effect_MalformedLifecycle_ClonesWithoutSilentlyRevertingToLegacy(string version, string mode, string grade)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var spell = Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object);
		var xml = Definition(); xml.Add(new XElement("Lifecycle", new XAttribute("version", version), new XAttribute("mode", mode),
			new XElement("Family", "flame-knife"), new XElement("PermanentOutput", new XAttribute("grade", grade), 91)));
		var effect = new TestEffect(xml, spell);
		Assert.IsNotNull(effect.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(xml, effect.SaveToXml()));
		Assert.IsNotNull(((CreateItemEffect)effect.Clone()).DefinitionError);
	}

	[TestMethod]
	public void Effect_LegacyDefinition_RetainsXmlAndAdmissionWithoutRequiringNativeService()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var effect = new TestEffect(Definition(), Mock.Of<IMagicSpell>(x => x.Gameworld == world.Object));
		Assert.IsNull(effect.DefinitionError);
		Assert.IsTrue(XNode.DeepEquals(Definition(), effect.SaveToXml()));
		Assert.IsTrue(effect.TryPrepareApplication(Mock.Of<ICharacter>(), Mock.Of<IPerceivable>(), default, default, TimeSpan.Zero, out var application, out var error));
		Assert.IsNotNull(application); Assert.IsNull(error);
	}

	[DataTestMethod]
	[DataRow(0)]
	[DataRow(1001)]
	public void Service_InvalidReconciliationBound_RefusesBeforeOpeningDatabase(int limit)
	{
		var service = new SpellOwnedItemService(Mock.Of<IFuturemud>());
		Assert.ThrowsException<ArgumentException>(() => service.ReconcileRetirements(DateTime.UtcNow, limit));
		Assert.ThrowsException<ArgumentException>(() => service.ReconcileRetirements(DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Local)));
	}

	private static XElement Definition() => new("Effect", new XAttribute("type", "createitem"), new XElement("ItemQuality", new XCData("base")),
		new XElement("ItemPrototypeId", 0), new XElement("ItemSkinId", 0), new XElement("Quantity", 1), new XElement("LoadString", new XCData("")));
	private sealed class TestEffect(XElement root, IMagicSpell spell) : CreateItemEffect(root, spell);
}
