#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using System.Xml.Linq;
using GameItem = MudSharp.Models.GameItem;
using GameItemComponent = MudSharp.Models.GameItemComponent;
using GameItemComponentProto = MudSharp.Models.GameItemComponentProto;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemLegacyCustodyTests
{
	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FindAmbiguousChildren_ConflictingBagAndClip_HonoursExplicitCustodyInEitherOrder(bool reverse)
	{
		var types = new Dictionary<(long, int), string> { [(40, 0)] = "Container", [(40, 1)] = "AmmoClip" };
		var rows = new[]
		{
			Parent(10, 20, 0, "<Definition><Contained>1</Contained><Contained>2</Contained><Contained>3</Contained><Contained>4</Contained></Definition>"),
			Parent(11, 21, 1, "<Definition><Contents><Item>1</Item><Item>2</Item><Item>3</Item></Contents></Definition>"),
			new GameItem { Id = 1 },
			new GameItem { Id = 2, ContainerId = 10 },
			new GameItem { Id = 3, RoomsGameItems = { new RoomsGameItems { GameItemId = 3, RoomId = 100 } } },
			new GameItem { Id = 4 }
		};
		var log = new List<string>();

		var blocked = LegacyItemCustody.FindAmbiguousChildren(reverse ? rows.Reverse() : rows, types, log.Add);

		CollectionAssert.AreEqual(new long[] { 1 }, blocked.ToArray());
		StringAssert.Contains(log.Single(), "item #10, component #20 | item #11, component #21");
	}

	[TestMethod]
	public void FindAmbiguousChildren_SelfAndInvalidXml_DoNotPreventAnUnrelatedClaim()
	{
		var types = new Dictionary<(long, int), string> { [(40, 0)] = "Container" };
		var log = new List<string>();
		var blocked = LegacyItemCustody.FindAmbiguousChildren(
			[Parent(10, 20, 0, "<Definition><Contained>10</Contained></Definition>"),
			 Parent(11, 21, 0, "<unclosed"),
			 Parent(12, 22, 0, "<Definition><Contained>10</Contained></Definition>")],
			types, log.Add);

		Assert.IsEmpty(blocked);
		StringAssert.Contains(log.Single(), "component #21 on item #11");
	}

	[TestMethod]
	public void FindAmbiguousChildren_SeveredRemainsAndContainerConflict_IsHeld()
	{
		var types = new Dictionary<(long, int), string>
		{
			[(40, 0)] = "Bodypart",
			[(41, 0)] = "Container"
		};
		var log = new List<string>();
		var rows = new[]
		{
			Parent(10, 20, 0, "<Definition><Contents><Item>30</Item></Contents><Implants><Implant>31</Implant></Implants></Definition>"),
			Parent(11, 21, 0, "<Definition><Contained>30</Contained></Definition>", 41),
			new GameItem { Id = 30 },
			new GameItem { Id = 31 }
		};

		var blocked = LegacyItemCustody.FindAmbiguousChildren(rows, types, log.Add);

		CollectionAssert.AreEqual(new long[] { 30 }, blocked.Order().ToArray());
		StringAssert.Contains(log.Single(), "item #10, component #20 | item #11, component #21");
	}

	[TestMethod]
	[DataRow("Gun", "<Magazine>30</Magazine>")]
	[DataRow("BoltAction", "<ChamberedRound>30</ChamberedRound>")]
	[DataRow("InternalMagazineGun", "<RoundsInMagazine><Round>30</Round></RoundsInMagazine>")]
	[DataRow("InternalMagazineGun", "<ChamberedCasing>30</ChamberedCasing>")]
	[DataRow("Gun", "<Attachments><Attachment slot='scope'>30</Attachment></Attachments>")]
	[DataRow("Sheath", "<Contents><Content>30</Content></Contents>")]
	[DataRow("Belt", "<Connected>30</Connected>")]
	[DataRow("Table", "<Chair>30</Chair>")]
	[DataRow("LockingContainer", "<Locks><Lock>30</Lock></Locks>")]
	[DataRow("Automation Housing", "<Contained>30</Contained>")]
	[DataRow("Refrigerator", "<Contained>30</Contained>")]
	[DataRow("ImplantRefrigerator", "<Contained>30</Contained>")]
	[DataRow("ImplantContainer", "<Contained>30</Contained>")]
	[DataRow("Dryer", "<Contained>30</Contained>")]
	[DataRow("FoldedPocket", "<Contained>30</Contained>")]
	[DataRow("BatteryCharger", "<Contained>30</Contained>")]
	[DataRow("Fuel Generator", "<Locks><Lock>30</Lock></Locks>")]
	[DataRow("CashRegister", "<Contained legacySlot='drawer'>30</Contained>")]
	[DataRow("LockingCashRegister", "<Locks><Lock legacySlot='mount'>30</Lock></Locks>")]
	[DataRow("WashingMachine", "<Locks><Lock legacySlot='mount'>30</Lock></Locks>")]
	[DataRow("FaxMachine", "<Contained>30</Contained>")]
	[DataRow("Photocopier", "<Locks><Lock>30</Lock></Locks>")]
	[DataRow("ShopStall", "<Contained>30</Contained>")]
	public void FindAmbiguousChildren_SpecialisedOwnerConflictsWithAnotherOwner_HoldsChild(string type, string contents)
	{
		var types = new Dictionary<(long, int), string> { [(40, 0)] = type, [(41, 0)] = "Container" };
		var rows = new[]
		{
			Parent(10, 20, 0, $"<Definition>{contents}</Definition>"),
			Parent(11, 21, 0, "<Definition><Contained>30</Contained></Definition>", 41),
			new GameItem { Id = 30 }
		};

		foreach (var orderedRows in new[] { rows, rows.Reverse().ToArray() })
		{
			var blocked = LegacyItemCustody.FindAmbiguousChildren(orderedRows, types, _ => { });
			CollectionAssert.AreEqual(new long[] { 30 }, blocked.Order().ToArray(), $"Failed for {type} discovery order.");
		}
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LegacyCycle_IsHeldAndSerializedInEitherDiscoveryOrder(bool reverse)
	{
		using var world = new Futuremud(null);
		var definitions = new Dictionary<(long, int), string> { [(40, 0)] = "Container" };
		var rows = new[]
		{
			Parent(10, 20, 0, "<Definition><Contained legacySlot='fixture'>11</Contained></Definition>"),
			Parent(11, 21, 0, "<Definition><Contained>10</Contained></Definition>")
		};
		world.IndexLegacyItemCustodyClaims(reverse ? rows.Reverse() : rows, definitions);
		var proto = (ContainerGameItemComponentProto)new GameItemComponentManager().GetProto(new GameItemComponentProto
		{
			Id = 40,
			Type = "Container",
			EditableItem = new EditableItem(),
			Definition = "<Definition />"
		}, world)!;
		var parent = Mock.Of<IGameItem>(x => x.Id == 10 && x.Gameworld == world);
		var container = new PersistedContainer(rows[0].GameItemComponents.Single(), proto, parent);

		Assert.IsTrue(world.HasAmbiguousLegacyItemCustody(10));
		Assert.IsTrue(world.HasAmbiguousLegacyItemCustody(11));
		Assert.IsEmpty(container.Contents);
		Assert.AreEqual("11", container.Definition.Element("Contained")?.Value);
		Assert.AreEqual("fixture", container.Definition.Element("Contained")?.Attribute("legacySlot")?.Value);
		container.Delete();
	}

	[TestMethod]
	public void CashRegisterAmbiguousChildClaim_IsPreservedAcrossSaveReloadAndDelete()
	{
		var definitions = new Dictionary<(long, int), string>
		{
			[(40, 0)] = "CashRegister",
			[(41, 0)] = "Container"
		};
		var rows = new[]
		{
			Parent(10, 20, 0, "<Definition Open='false'><Contained legacySlot='drawer'>30</Contained></Definition>"),
			Parent(11, 21, 0, "<Definition><Contained>30</Contained></Definition>", 41),
			new GameItem { Id = 30 }
		};

		var savedDefinition = SaveAndDeleteCashRegister(rows, definitions);
		var savedParent = Parent(10, 20, 0, savedDefinition);
		var restartedRows = new[] { savedParent, rows[1], rows[2] };
		var afterRestartDefinition = SaveAndDeleteCashRegister(restartedRows, definitions);

		var claim = XElement.Parse(afterRestartDefinition).Element("Contained");
		Assert.IsNotNull(claim);
		Assert.AreEqual("30", claim.Value);
		Assert.AreEqual("drawer", claim.Attribute("legacySlot")?.Value);
	}

	[TestMethod]
	public void CashRegisterTake_WithoutWorldLocation_RemovesItemWithoutThrowing()
	{
		using var world = new Futuremud(null);
		var proto = (CashRegisterGameItemComponentProto)new GameItemComponentManager().GetProto(
			new GameItemComponentProto
			{
				Id = 40,
				Type = "CashRegister",
				EditableItem = new EditableItem(),
				Definition = "<Definition />"
			}, world)!;
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(10);
		parent.SetupGet(x => x.Gameworld).Returns(world);
		parent.SetupGet(x => x.TrueLocations).Returns(Array.Empty<IRoom>());
		var child = new Mock<IGameItem>();
		child.SetupProperty(x => x.ContainedIn);
		var register = new CashRegisterGameItemComponent(proto, parent.Object, temporary: true);
		register.Put(null, child.Object);

		Assert.AreSame(child.Object, register.Contents.Single());
		Assert.IsTrue(register.Take(child.Object));
		Assert.IsEmpty(register.Contents);
	}

	private static string SaveAndDeleteCashRegister(IEnumerable<GameItem> rows,
		IReadOnlyDictionary<(long Id, int Revision), string> definitions)
	{
		using var world = new Futuremud(null);
		world.IndexLegacyItemCustodyClaims(rows, definitions);
		var proto = (CashRegisterGameItemComponentProto)new GameItemComponentManager().GetProto(
			new GameItemComponentProto
			{
				Id = 40,
				Type = "CashRegister",
				EditableItem = new EditableItem(),
				Definition = "<Definition />"
			}, world)!;
		var row = rows.Single(x => x.Id == 10).GameItemComponents.Single();
		var parent = Mock.Of<IGameItem>(x => x.Id == 10 && x.Gameworld == world);
		var register = new PersistedCashRegister(row, proto, parent);
		Assert.IsEmpty(register.Contents);
		var definition = register.Definition;
		register.Delete();
		Assert.IsTrue(XNode.DeepEquals(definition, register.Definition));
		return register.Definition.ToString(SaveOptions.DisableFormatting);
	}

	private sealed class PersistedContainer(GameItemComponent row, ContainerGameItemComponentProto prototype,
		IGameItem parent) : ContainerGameItemComponent(row, prototype, parent)
	{
		public XElement Definition => XElement.Parse(SaveToXml());
	}

	private sealed class PersistedCashRegister(GameItemComponent row, CashRegisterGameItemComponentProto prototype,
		IGameItem parent) : CashRegisterGameItemComponent(row, prototype, parent)
	{
		public XElement Definition => XElement.Parse(SaveToXml());
	}

	private static GameItem Parent(long itemId, long componentId, int revision, string definition, long prototypeId = 40) => new()
	{
		Id = itemId,
		GameItemComponents =
		{
			new GameItemComponent
			{
				Id = componentId,
				GameItemComponentProtoId = prototypeId,
				GameItemComponentProtoRevision = revision,
				Definition = definition
			}
		}
	};
}
