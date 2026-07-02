using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class OutfitTemplateTests
{
	[TestMethod]
	public void CloneCopiesItemsAndMetadata()
	{
		var gameworld = new Mock<IFuturemud>();
		var item = TemplateItem("coat", Prototype(1, wearable: true), OutfitTemplateItemPlacement.Worn, WearProfile(10), wearOrder: 3);
		var source = new TemplateOutfit(gameworld.Object, "Guard", "Gate guard issue.", OutfitExclusivity.ExcludeItemsBelow, new[] { item });

		var clone = new TemplateOutfit(source, "Guard Copy", persist: false);

		Assert.AreEqual("Guard Copy", clone.Name);
		Assert.AreEqual(source.Description, clone.Description);
		Assert.AreEqual(source.Exclusivity, clone.Exclusivity);
		Assert.AreEqual(1, clone.Items.Count());
		Assert.AreNotSame(source.Items.Single(), clone.Items.Single());
		Assert.AreEqual("coat", clone.Items.Single().TemplateKey);
		Assert.AreEqual(3, clone.Items.Single().WearOrder);
	}

	[TestMethod]
	public void ValidationRejectsStaleManualLoadBlockedAndNonWearableWornEntries()
	{
		var gameworld = new Mock<IFuturemud>();
		var stale = TemplateItem("stale", Prototype(1, status: RevisionStatus.Obsolete), OutfitTemplateItemPlacement.Inventory);
		var blocked = TemplateItem("blocked", Prototype(2, preventManualLoad: true), OutfitTemplateItemPlacement.Inventory);
		var nonWearable = TemplateItem("hat", Prototype(3), OutfitTemplateItemPlacement.Worn, WearProfile(20));
		var template = new TemplateOutfit(gameworld.Object, "Invalid", "Invalid.", OutfitExclusivity.NonExclusive, new[] { stale, blocked, nonWearable });

		var warnings = template.ValidationWarnings.ToList();

		Assert.IsTrue(warnings.Any(x => x.Contains("non-current")));
		Assert.IsTrue(warnings.Any(x => x.Contains("prevents manual loading")));
		Assert.IsTrue(warnings.Any(x => x.Contains("not wearable")));
	}

	[TestMethod]
	public void MaterialiseReturnsOutfitAttachedToTargetAndAutoSuffixesDuplicateName()
	{
		var created = Item(100, Prototype(1), "a travel cloak");
		var proto = Prototype(1, created.Object);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Travel Kit", "Road gear.", OutfitExclusivity.ExcludeAllItems,
			new[] { TemplateItem("cloak", proto, OutfitTemplateItemPlacement.Inventory) });
		var existingOutfit = new Mock<IOutfit>();
		existingOutfit.Setup(x => x.Name).Returns("Travel Kit");
		var target = Target(new[] { existingOutfit.Object }, out var location, out var body, out var added);
		body.Setup(x => x.CanGet(created.Object, 0, It.IsAny<ItemCanGetIgnore>())).Returns(true);

		var outfit = template.Materialise(target.Object);

		Assert.AreEqual("Travel Kit (2)", outfit.Name);
		Assert.AreEqual(OutfitExclusivity.ExcludeAllItems, outfit.Exclusivity);
		Assert.AreSame(outfit, added.Single());
		location.Verify(x => x.Insert(created.Object, false), Times.Once);
		body.Verify(x => x.Get(created.Object, 0, null, true, It.IsAny<ItemCanGetIgnore>()), Times.Once);
	}

	[TestMethod]
	public void OutfitItemsExposeActualGameItemsAndPrototypeId()
	{
		var target = Target(System.Array.Empty<IOutfit>(), out _, out _, out _);
		var proto = Prototype(55);
		var item = Item(101, proto, "a jacket").Object;
		var outfit = new Outfit(target.Object, "Test");

		var entry = outfit.AddItem(item, null, null);

		Assert.AreSame(item, entry.Item);
		Assert.AreEqual(55M, entry.GetProperty("item").GetProperty("proto").GetObject);
	}

	[TestMethod]
	public void ContainerPlacementFallsBackToRoomWhenRuntimeContainerRejectsItem()
	{
		var item = Item(100, Prototype(1), "a badge");
		var containerRuntime = new Mock<IContainer>();
		containerRuntime.Setup(x => x.CanPut(item.Object)).Returns(false);
		var container = Item(200, Prototype(2, container: true), "a pouch", containerRuntime.Object);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Pouch Kit", "Pouch gear.", OutfitExclusivity.NonExclusive,
			new[]
			{
				TemplateItem("pouch", container.Object.Prototype, OutfitTemplateItemPlacement.Room, wearOrder: 0),
				TemplateItem("badge", item.Object.Prototype, OutfitTemplateItemPlacement.Container, containerKey: "pouch", wearOrder: 1)
			});
		var target = Target(System.Array.Empty<IOutfit>(), out var location, out _, out _);

		template.Materialise(target.Object);

		location.Verify(x => x.Insert(item.Object, false), Times.Once);
		containerRuntime.Verify(x => x.Put(It.IsAny<ICharacter>(), item.Object, true), Times.Never);
	}

	[TestMethod]
	public void SuccessfulContainerPlacementRemovesRoomAnchor()
	{
		var item = Item(100, Prototype(1), "a badge");
		var containerRuntime = new Mock<IContainer>();
		containerRuntime.Setup(x => x.CanPut(item.Object)).Returns(true);
		var container = Item(200, Prototype(2, container: true), "a pouch", containerRuntime.Object);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Pouch Kit", "Pouch gear.", OutfitExclusivity.NonExclusive,
			new[]
			{
				TemplateItem("pouch", container.Object.Prototype, OutfitTemplateItemPlacement.Room, wearOrder: 0),
				TemplateItem("badge", item.Object.Prototype, OutfitTemplateItemPlacement.Container, containerKey: "pouch", wearOrder: 1)
			});
		Target(System.Array.Empty<IOutfit>(), out var location, out _, out var added);

		template.Materialise(BuilderTarget(location.Object, added).Object);

		location.Verify(x => x.Insert(item.Object, false), Times.Once);
		item.Verify(x => x.Get(null), Times.Once);
		containerRuntime.Verify(x => x.Put(It.IsAny<ICharacter>(), item.Object, false), Times.Once);
	}

	[TestMethod]
	public void BuilderItemAddAcceptsFinalFreeTextLoadArguments()
	{
		var proto = Prototype(1);
		var gameworld = GameworldForBuilder(new[] { proto });
		var template = new TemplateOutfit(gameworld.Object, "Template", "Desc.", OutfitExclusivity.NonExclusive, System.Array.Empty<IOutfitTemplateItem>());
		var actor = BuilderActor();

		var result = template.BuildingCommand(actor.Object, new StringStack("item add badge 1 room args colour=red size=large"));

		Assert.IsTrue(result);
		Assert.AreEqual("colour=red size=large", template.Items.Single().LoadArguments);
	}

	[TestMethod]
	public void BuilderItemAddAllowsWornPlacementWithoutExplicitProfile()
	{
		var proto = Prototype(1, wearable: true);
		var gameworld = GameworldForBuilder(new[] { proto });
		var template = new TemplateOutfit(gameworld.Object, "Template", "Desc.", OutfitExclusivity.NonExclusive, System.Array.Empty<IOutfitTemplateItem>());
		var actor = BuilderActor();

		var result = template.BuildingCommand(actor.Object, new StringStack("item add cloak 1 worn"));

		Assert.IsTrue(result);
		Assert.AreEqual(OutfitTemplateItemPlacement.Worn, template.Items.Single().Placement);
		Assert.IsNull(template.Items.Single().DesiredProfile);
		Assert.IsFalse(template.ValidationWarnings.Any());
	}

	[TestMethod]
	public void MaterialiseWornPlacementWithoutProfileUsesCreatedItemDefaultProfile()
	{
		var defaultProfile = WearProfile(10);
		var wearable = new Mock<IWearable>();
		wearable.SetupGet(x => x.DefaultProfile).Returns(defaultProfile);
		var created = Item(100, Prototype(1), "a travel cloak", wearable: wearable.Object);
		var proto = Prototype(1, created.Object, wearable: true);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Travel Kit", "Road gear.", OutfitExclusivity.NonExclusive,
			new[] { TemplateItem("cloak", proto, OutfitTemplateItemPlacement.Worn) });
		var target = Target(System.Array.Empty<IOutfit>(), out _, out var body, out _);
		body.Setup(x => x.CanWear(created.Object, defaultProfile)).Returns(true);

		template.Materialise(target.Object);

		body.Verify(x => x.Wear(created.Object, defaultProfile, null, true), Times.Once);
	}

	[TestMethod]
	public void MaterialiseAppliesWholeTemplateLoadArgumentsAfterItemArguments()
	{
		var created = Item(100, Prototype(1), "a cloak");
		var proto = created.Object.Prototype;
		string capturedLoadArguments = null;
		Mock.Get(proto)
		    .Setup(x => x.CreateNew(It.IsAny<ICharacter>(), null, 1, It.IsAny<string>()))
		    .Callback<ICharacter, IGameItemSkin, int, string>((_, _, _, args) => capturedLoadArguments = args)
		    .Returns(new[] { created.Object });
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Travel Kit", "Road gear.", OutfitExclusivity.NonExclusive,
			new[] { TemplateItem("cloak", proto, OutfitTemplateItemPlacement.Room, loadArguments: "shade=dark") });
		var target = Target(System.Array.Empty<IOutfit>(), out _, out _, out _);

		template.Materialise(target.Object, loadArguments: "colour=red");

		Assert.AreEqual("shade=dark colour=red", capturedLoadArguments);
	}

	[TestMethod]
	public void MaterialiseAttachedPlacementConnectsItemToAvailableBelt()
	{
		var beltable = new Mock<IBeltable>();
		var item = Item(100, Prototype(1), "a knife", beltable: beltable.Object);
		beltable.SetupGet(x => x.Parent).Returns(item.Object);
		var proto = Prototype(1, item.Object, beltable: true);
		var beltRuntime = new Mock<IBelt>();
		beltRuntime.Setup(x => x.CanAttachBeltable(beltable.Object)).Returns(IBeltCanAttachBeltableResult.Success);
		var belt = Item(200, Prototype(2, belt: true), "a belt", belt: beltRuntime.Object);
		beltRuntime.SetupGet(x => x.Parent).Returns(belt.Object);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Knife Kit", "Belt knife.", OutfitExclusivity.NonExclusive,
			new[] { TemplateItem("knife", proto, OutfitTemplateItemPlacement.AttachedToBelt) });
		var target = Target(System.Array.Empty<IOutfit>(), out _, out var body, out _);
		body.Setup(x => x.ExternalItems).Returns(new[] { belt.Object });

		template.Materialise(target.Object);

		item.Verify(x => x.Get(null), Times.Once);
		beltRuntime.Verify(x => x.AddConnectedItem(beltable.Object), Times.Once);
		body.Verify(x => x.RecalculateItemHelpers(), Times.Once);
	}

	[TestMethod]
	public void MaterialiseSheathedPlacementUsesAvailableSheath()
	{
		var wieldable = new Mock<IWieldable>();
		var item = Item(100, Prototype(1), "a sword", wieldable: wieldable.Object);
		var proto = Prototype(1, item.Object, wieldable: true);
		var sheathRuntime = new Mock<ISheath>();
		var sheath = Item(200, Prototype(2, sheath: true), "a scabbard", sheath: sheathRuntime.Object);
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Sword Kit", "Sword and scabbard.", OutfitExclusivity.NonExclusive,
			new[] { TemplateItem("sword", proto, OutfitTemplateItemPlacement.Sheathed) });
		var target = Target(System.Array.Empty<IOutfit>(), out _, out var body, out _);
		body.Setup(x => x.ExternalItems).Returns(new[] { sheath.Object });
		body.Setup(x => x.CanSheathe(item.Object, sheath.Object)).Returns(true);

		template.Materialise(target.Object);

		item.Verify(x => x.Get(null), Times.Once);
		body.Verify(x => x.Sheathe(item.Object, sheath.Object, null, OutputFlags.Normal, true), Times.Once);
	}

	[TestMethod]
	public void RejectedPrototypeEditRestoresPreviousPrototype()
	{
		var profile = WearProfile(10);
		var wearableProto = Prototype(1, wearable: true);
		var nonWearableProto = Prototype(2);
		var gameworld = GameworldForBuilder(new[] { wearableProto, nonWearableProto }, new[] { profile });
		var item = TemplateItem("cloak", wearableProto, OutfitTemplateItemPlacement.Worn, profile);
		var template = new TemplateOutfit(gameworld.Object, "Template", "Desc.", OutfitExclusivity.NonExclusive, new[] { item });
		var actor = BuilderActor();

		var result = template.BuildingCommand(actor.Object, new StringStack("item proto cloak 2"));

		Assert.IsFalse(result);
		Assert.AreSame(wearableProto, item.GameItemProto);
	}

	[TestMethod]
	public void ExplicitOverrideNameIsUsedAsSuffixBaseWhenDuplicate()
	{
		var created = Item(100, Prototype(1), "a cloak");
		var template = new TemplateOutfit(new Mock<IFuturemud>().Object, "Template", "Desc.", OutfitExclusivity.NonExclusive,
			new[] { TemplateItem("cloak", created.Object.Prototype, OutfitTemplateItemPlacement.Room) });
		var existingOutfit = new Mock<IOutfit>();
		existingOutfit.Setup(x => x.Name).Returns("Ceremony");
		var target = Target(new[] { existingOutfit.Object }, out _, out _, out _);

		var outfit = template.Materialise(target.Object, "Ceremony");

		Assert.AreEqual("Ceremony (2)", outfit.Name);
	}

	[TestMethod]
	public void FutureProgDocumentationIncludesLoadOutfitTemplateOverloads()
	{
		FutureProgTestBootstrap.EnsureInitialised();

		var overloads = FutureProg.GetFunctionCompilerInformations()
		                          .Where(x => x.FunctionName.EqualTo("loadoutfittemplate"))
		                          .ToList();

		Assert.AreEqual(6, overloads.Count);
		Assert.IsTrue(overloads.All(x => x.ReturnType == ProgVariableTypes.Outfit));
		Assert.IsTrue(overloads.All(x => x.Category.EqualTo("Outfits")));
		Assert.IsTrue(overloads.All(x => x.FunctionHelp.Contains("outfit template")));
		Assert.IsTrue(overloads.Any(x => x.Parameters.SequenceEqual(new[] { ProgVariableTypes.Number, ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text })));
		Assert.IsTrue(overloads.Any(x => x.Parameters.SequenceEqual(new[] { ProgVariableTypes.Text, ProgVariableTypes.Character, ProgVariableTypes.Text, ProgVariableTypes.Text })));
	}

	private static IOutfitTemplateItem TemplateItem(
		string key,
		IGameItemProto proto,
		OutfitTemplateItemPlacement placement,
		IWearProfile profile = null,
		string containerKey = null,
		int wearOrder = 0,
		string loadArguments = "")
	{
		return new TemplateOutfitItem
		{
			TemplateKey = key,
			GameItemProto = proto,
			Placement = placement,
			DesiredProfile = profile,
			ContainerKey = containerKey,
			WearOrder = wearOrder,
			LoadArguments = loadArguments
		};
	}

	private static IGameItemProto Prototype(
		long id,
		IGameItem createdItem = null,
		bool wearable = false,
		bool container = false,
		bool beltable = false,
		bool belt = false,
		bool sheath = false,
		bool wieldable = false,
		bool preventManualLoad = false,
		RevisionStatus status = RevisionStatus.Current)
	{
		var proto = new Mock<IGameItemProto>();
		proto.Setup(x => x.Id).Returns(id);
		proto.Setup(x => x.Name).Returns($"proto{id}");
		proto.Setup(x => x.ShortDescription).Returns($"proto {id}");
		proto.Setup(x => x.Status).Returns(status);
		proto.Setup(x => x.PreventManualLoad).Returns(preventManualLoad);
		proto.Setup(x => x.Components).Returns(System.Array.Empty<IGameItemComponentProto>());
		proto.Setup(x => x.IsItemType<IWearablePrototype>()).Returns(wearable);
		proto.Setup(x => x.IsItemType<IContainerPrototype>()).Returns(container);
		proto.Setup(x => x.IsItemType<IBeltablePrototype>()).Returns(beltable);
		proto.Setup(x => x.IsItemType<IBeltPrototype>()).Returns(belt);
		proto.Setup(x => x.IsItemType<ISheathPrototype>()).Returns(sheath);
		proto.Setup(x => x.IsItemType<IWieldablePrototype>()).Returns(wieldable);
		if (createdItem is not null)
		{
			proto.Setup(x => x.CreateNew(It.IsAny<ICharacter>(), null, 1, It.IsAny<string>()))
			     .Returns(new[] { createdItem });
		}

		return proto.Object;
	}

	private static IWearProfile WearProfile(long id)
	{
		var profile = new Mock<IWearProfile>();
		profile.Setup(x => x.Id).Returns(id);
		profile.Setup(x => x.Name).Returns($"profile{id}");
		return profile.Object;
	}

	private static Mock<IGameItem> Item(
		long id,
		IGameItemProto proto,
		string description,
		IContainer container = null,
		IWearable wearable = null,
		IBeltable beltable = null,
		IBelt belt = null,
		ISheath sheath = null,
		IWieldable wieldable = null)
	{
		var item = new Mock<IGameItem>();
		item.Setup(x => x.Id).Returns(id);
		item.Setup(x => x.Name).Returns(description);
		item.Setup(x => x.Prototype).Returns(proto);
		item.Setup(x => x.GetProperty("proto")).Returns(new NumberVariable(proto.Id));
		item.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(), It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>()))
		    .Returns(description);
		item.SetupProperty(x => x.RoomLayer);
		item.Setup(x => x.GetItemType<IContainer>()).Returns(container);
		item.Setup(x => x.IsItemType<IContainer>()).Returns(container is not null);
		item.Setup(x => x.GetItemType<IWearable>()).Returns(wearable);
		item.Setup(x => x.IsItemType<IWearable>()).Returns(wearable is not null);
		item.Setup(x => x.GetItemType<IBeltable>()).Returns(beltable);
		item.Setup(x => x.IsItemType<IBeltable>()).Returns(beltable is not null);
		item.Setup(x => x.GetItemType<IBelt>()).Returns(belt);
		item.Setup(x => x.IsItemType<IBelt>()).Returns(belt is not null);
		item.Setup(x => x.GetItemType<ISheath>()).Returns(sheath);
		item.Setup(x => x.IsItemType<ISheath>()).Returns(sheath is not null);
		item.Setup(x => x.GetItemType<IWieldable>()).Returns(wieldable);
		item.Setup(x => x.IsItemType<IWieldable>()).Returns(wieldable is not null);
		item.Setup(x => x.Get(null)).Returns(item.Object);
		if (proto is Mock<IGameItemProto>)
		{
			// Unreachable with Moq object instances, kept for readability.
		}
		Mock.Get(proto).Setup(x => x.CreateNew(It.IsAny<ICharacter>(), null, 1, It.IsAny<string>()))
		     .Returns(new[] { item.Object });
		return item;
	}

	private static Mock<ICharacter> Target(IEnumerable<IOutfit> outfits, out Mock<ICell> location, out Mock<IBody> body, out List<IOutfit> added)
	{
		location = new Mock<ICell>();
		body = new Mock<IBody>();
		var addedList = new List<IOutfit>();
		added = addedList;
		var currentOutfits = outfits.ToList();
		var target = new Mock<ICharacter>();
		target.Setup(x => x.Location).Returns(location.Object);
		target.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		target.Setup(x => x.Body).Returns(body.Object);
		target.Setup(x => x.Outfits).Returns(() => currentOutfits.Concat(addedList));
		target.Setup(x => x.AddOutfit(It.IsAny<IOutfit>())).Callback<IOutfit>(x => addedList.Add(x));
		return target;
	}

	private static Mock<ICharacter> BuilderTarget(ICell location, List<IOutfit> added)
	{
		var target = new Mock<ICharacter>();
		target.Setup(x => x.Location).Returns(location);
		target.SetupProperty(x => x.RoomLayer, RoomLayer.GroundLevel);
		target.Setup(x => x.Outfits).Returns(() => added);
		target.Setup(x => x.AddOutfit(It.IsAny<IOutfit>())).Callback<IOutfit>(x => added.Add(x));
		return target;
	}

	private static Mock<IFuturemud> GameworldForBuilder(IEnumerable<IGameItemProto> protos, IEnumerable<IWearProfile> wearProfiles = null)
	{
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.ItemProtos).Returns(ItemProtoCollection(protos.ToArray()));
		gameworld.Setup(x => x.WearProfiles).Returns(WearProfileCollection((wearProfiles ?? System.Array.Empty<IWearProfile>()).ToArray()));
		return gameworld;
	}

	private static IUneditableRevisableAll<IGameItemProto> ItemProtoCollection(params IGameItemProto[] protos)
	{
		var all = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		all.As<IEnumerable<IGameItemProto>>()
		   .Setup(x => x.GetEnumerator())
		   .Returns(() => ((IEnumerable<IGameItemProto>)protos).GetEnumerator());
		return all.Object;
	}

	private static IUneditableAll<IWearProfile> WearProfileCollection(params IWearProfile[] profiles)
	{
		var all = new Mock<IUneditableAll<IWearProfile>>();
		all.As<IEnumerable<IWearProfile>>()
		   .Setup(x => x.GetEnumerator())
		   .Returns(() => ((IEnumerable<IWearProfile>)profiles).GetEnumerator());
		return all.Object;
	}

	private static Mock<ICharacter> BuilderActor()
	{
		var output = new Mock<IOutputHandler>();
		output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>())).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.OutputHandler).Returns(output.Object);
		return actor;
	}
}
