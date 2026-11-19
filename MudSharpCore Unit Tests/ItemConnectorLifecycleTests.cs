#nullable enable

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using System.Linq;
using System.Xml.Linq;
using DbComponent = MudSharp.Models.GameItemComponent;
using DbPrototype = MudSharp.Models.GameItemComponentProto;
using MudSharp.Form.Shape;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Components;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ItemConnectorLifecycleTests
{
    [TestMethod]
    public void FindExactFreeConnector_SeveralCompatiblePorts_ReturnsPersistedPort()
    {
        var savedPort = new ConnectorType(Gender.Female, "power", powered: false);
        var availablePorts = new[]
        {
            new ConnectorType(Gender.Male, "power"),
            savedPort,
            new ConnectorType(Gender.Neuter, "data")
        };

        var result = ConnectableGameItemComponent.FindExactFreeConnector(availablePorts, savedPort);

        Assert.AreSame(savedPort, result);
    }

    [TestMethod]
    public void FindExactFreeConnector_OnlyCompatibleButDifferentPort_ReturnsNull()
    {
        var savedPort = new ConnectorType(Gender.Female, "power", powered: false);
        var availablePorts = new[]
        {
            new ConnectorType(Gender.Male, "power"),
            new ConnectorType(Gender.Female, "power")
        };

        var result = ConnectableGameItemComponent.FindExactFreeConnector(availablePorts, savedPort);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindExactFreeConnector_DuplicateIdenticalPorts_HoldsAmbiguousLegacyIdentity()
    {
        var savedPort = new ConnectorType(Gender.Neuter, "data");
        var firstPort = new ConnectorType(Gender.Neuter, "data");
        var secondPort = new ConnectorType(Gender.Neuter, "data");

        var result = ConnectableGameItemComponent.FindExactFreeConnector(new[] { firstPort, secondPort }, savedPort);

        Assert.IsNull(result);
    }

    [TestMethod]
    public void FindSavedFreeConnector_DuplicatePhysicalPorts_UsesPersistedPortIndex()
    {
        var first = new ConnectorType(Gender.Neuter, "data");
        var second = new ConnectorType(Gender.Neuter, "data");
        var connectable = new Mock<IConnectable>();
        connectable.SetupGet(x => x.Connections).Returns([first, second]);
        connectable.SetupGet(x => x.ConnectedItems).Returns([]);
        connectable.SetupGet(x => x.FreeConnections).Returns([first, second]);

        var result = GameItemComponent.FindSavedFreeConnector(connectable.Object,
            new ConnectorType(second.ToString(), portIndex: 1));

        Assert.AreSame(second, result);
        Assert.IsNull(GameItemComponent.FindSavedFreeConnector(connectable.Object, new ConnectorType(second.ToString())));
    }

    [TestMethod]
    public void EnsurePrototypePortIds_LegacyDefinition_SeedsDeterministicallyAndSeparatesDuplicates()
    {
        var definition = new XElement("Definition", new XElement("Connectors",
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data")),
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"))));
        var firstLoad = new[]
        {
            new ConnectorType(Gender.Neuter, "data"),
            new ConnectorType(Gender.Neuter, "data")
        };
        var secondLoad = new[]
        {
            new ConnectorType(Gender.Neuter, "data"),
            new ConnectorType(Gender.Neuter, "data")
        };

        ConnectorType.EnsurePrototypePortIds(123, firstLoad, definition);
        ConnectorType.EnsurePrototypePortIds(123, secondLoad, definition);

        Assert.AreEqual(firstLoad[0].PortId, secondLoad[0].PortId);
        Assert.AreEqual(firstLoad[1].PortId, secondLoad[1].PortId);
        Assert.AreNotEqual(firstLoad[0].PortId, firstLoad[1].PortId);
    }

    [TestMethod]
    public void EnsurePrototypePortIds_ExplicitIds_StayWithReorderedPhysicalPorts()
    {
        var first = new ConnectorType(Gender.Neuter, "data");
        var second = new ConnectorType(Gender.Neuter, "data");
        var definition = new XElement("Definition", new XElement("Connectors",
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("id", "port-a")),
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("id", "port-b"))));

        ConnectorType.EnsurePrototypePortIds(123, [first, second], definition);
        Assert.AreEqual("port-a", first.PortId);
        Assert.AreEqual("port-b", second.PortId);

        ConnectorType.EnsurePrototypePortIds(123, [second, first], new XElement("Definition", new XElement("Connectors",
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("id", "port-b")),
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("id", "port-a")))));

        Assert.AreEqual("port-a", first.PortId);
        Assert.AreEqual("port-b", second.PortId);
    }

    [TestMethod]
    public void ConnectorTypePairing_PersistsReciprocalPhysicalIdentity()
    {
        var first = new ConnectorType(Gender.Male, "power", portId: "first");
        var second = new ConnectorType(Gender.Female, "power", portId: "second");

        var (pairedFirst, pairedSecond) = ConnectorType.CreatePairedConnectionInstances(first, second);

        Assert.AreEqual("second", pairedFirst.PeerPortId);
        Assert.AreEqual("first", pairedSecond.PeerPortId);
        Assert.IsNull(first.PeerPortId);
        Assert.IsNull(second.PeerPortId);
        var loadedFirst = new ConnectorType(pairedFirst.ToString(), 0, 1, pairedFirst.PortId, pairedFirst.PeerPortId);
        var loadedSecond = new ConnectorType(pairedSecond.ToString(), 1, 0, pairedSecond.PortId, pairedSecond.PeerPortId);
        Assert.AreEqual("second", loadedFirst.PeerPortId);
        Assert.AreEqual("first", loadedSecond.PeerPortId);
    }

    [TestMethod]
    public void ConnectorTypePairing_RejectsIncompatiblePorts()
    {
        var power = new ConnectorType(Gender.Male, "power");
        var data = new ConnectorType(Gender.Female, "data");

        Assert.ThrowsExactly<ArgumentException>(() => ConnectorType.CreatePairedConnectionInstances(power, data));
        Assert.IsNull(power.PeerPortId);
        Assert.IsNull(data.PeerPortId);
    }

    [TestMethod]
    public void ConnectPair_UsesPerConnectionPortsAndDefersObserversUntilBothEndsExist()
    {
        var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
        world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        var prototypePort = new ConnectorType(Gender.Neuter, "data");
        var prototype = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(
            CreatePrototype("Connectable", prototypePort), world.Object)!;
        var itemPrototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
        itemPrototype.SetupGet(x => x.Gameworld).Returns(world.Object);
        itemPrototype.SetupGet(x => x.Name).Returns("connector item");
        itemPrototype.SetupGet(x => x.Components).Returns([prototype]);
        itemPrototype.SetupGet(x => x.Keywords).Returns([]);
        var firstParent = new GameItem(itemPrototype.Object);
        var secondParent = new GameItem(itemPrototype.Object);
        var first = firstParent.GetItemType<IConnectable>()!;
        var second = secondParent.GetItemType<IConnectable>()!;
        var observed = 0;
        firstParent.OnConnected += (_, _) =>
        {
            Assert.AreEqual(1, first.ConnectedItems.Count());
            Assert.AreEqual(1, second.ConnectedItems.Count());
            observed++;
            first.Connect(null, second);
        };
        secondParent.OnConnected += (_, _) =>
        {
            Assert.AreEqual(1, first.ConnectedItems.Count());
            Assert.AreEqual(1, second.ConnectedItems.Count());
            observed++;
        };

        first.Connect(null, second);

        Assert.AreEqual(2, observed);
        Assert.AreEqual(1, first.ConnectedItems.Count());
        Assert.AreEqual(1, second.ConnectedItems.Count());
        Assert.IsNull(prototype.Connections.Single().PeerPortId,
            "A connection must not mutate the prototype's shared connector descriptor.");
        Assert.AreEqual(first.ConnectedItems.Single().Item1.PeerPortId, second.ConnectedItems.Single().Item1.PortId);
        Assert.AreEqual(second.ConnectedItems.Single().Item1.PeerPortId, first.ConnectedItems.Single().Item1.PortId);
    }

    [TestMethod]
    public void ConnectPair_SuppressesSecondConnectedNotificationWhenFirstObserverDisconnects()
    {
        var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
        world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        var connector = new ConnectorType(Gender.Neuter, "data");
        var connectorProto = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(
            CreatePrototype("Connectable", connector), world.Object)!;
        var itemPrototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
        itemPrototype.SetupGet(x => x.Gameworld).Returns(world.Object);
        itemPrototype.SetupGet(x => x.Name).Returns("connector item");
        itemPrototype.SetupGet(x => x.Components).Returns([connectorProto]);
        itemPrototype.SetupGet(x => x.Keywords).Returns([]);
        var firstItem = new GameItem(itemPrototype.Object);
        var secondItem = new GameItem(itemPrototype.Object);
        var first = firstItem.GetItemType<IConnectable>()!;
        var second = secondItem.GetItemType<IConnectable>()!;
        var secondNotifications = 0;
        firstItem.OnConnected += (_, _) => first.RawDisconnect(second, true);
        secondItem.OnConnected += (_, _) => secondNotifications++;

        first.Connect(null, second);

        Assert.AreEqual(0, secondNotifications,
            "The peer notification is stale once the first item's event handler disconnects the pair.");
        Assert.IsFalse(first.ConnectedItems.Any());
        Assert.IsFalse(second.ConnectedItems.Any());
    }

    [TestMethod]
    public void ConnectPair_DoesNotTreatSamePortReconnectAsOriginalPairForQueuedNotification()
    {
        var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
        world.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
        var connector = new ConnectorType(Gender.Neuter, "data");
        var connectorProto = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(
            CreatePrototype("Connectable", connector), world.Object)!;
        var itemPrototype = new Mock<IGameItemProto> { DefaultValue = DefaultValue.Mock };
        itemPrototype.SetupGet(x => x.Gameworld).Returns(world.Object);
        itemPrototype.SetupGet(x => x.Name).Returns("connector item");
        itemPrototype.SetupGet(x => x.Components).Returns([connectorProto]);
        itemPrototype.SetupGet(x => x.Keywords).Returns([]);
        var firstItem = new GameItem(itemPrototype.Object);
        var secondItem = new GameItem(itemPrototype.Object);
        var first = firstItem.GetItemType<IConnectable>()!;
        var second = secondItem.GetItemType<IConnectable>()!;
        var reconnected = false;
        var secondNotifications = 0;
        firstItem.OnConnected += (_, _) =>
        {
            if (reconnected) return;
            reconnected = true;
            first.RawDisconnect(second, true);
            first.Connect(null, second);
        };
        secondItem.OnConnected += (_, _) => secondNotifications++;

        first.Connect(null, second);

        Assert.IsTrue(reconnected);
        Assert.AreEqual(1, secondNotifications,
            "Only the new connection's notification should be delivered; the queued original must be discarded.");
        Assert.AreEqual(1, first.ConnectedItems.Count());
        Assert.AreEqual(1, second.ConnectedItems.Count());
    }

    [TestMethod]
    public void GetConnectedPortPairs_UsesSavedPeerIdentityWhenParallelLinksWereAddedInDifferentOrders()
    {
        var firstEndpoint = new Mock<IConnectable>();
        var secondEndpoint = new Mock<IConnectable>();
        var localA = new ConnectorType(Gender.Male, "power", portId: "local-a");
        var localB = new ConnectorType(Gender.Male, "power", portId: "local-b");
        var peerA = new ConnectorType(Gender.Female, "power", portId: "peer-a");
        var peerB = new ConnectorType(Gender.Female, "power", portId: "peer-b");
        var (pairedLocalA, pairedPeerA) = ConnectorType.CreatePairedConnectionInstances(localA, peerA);
        var (pairedLocalB, pairedPeerB) = ConnectorType.CreatePairedConnectionInstances(localB, peerB);
        firstEndpoint.SetupGet(x => x.ConnectedItems).Returns([
            Tuple.Create(pairedLocalA, secondEndpoint.Object), Tuple.Create(pairedLocalB, secondEndpoint.Object)
        ]);
        secondEndpoint.SetupGet(x => x.ConnectedItems).Returns([
            Tuple.Create(pairedPeerB, firstEndpoint.Object), Tuple.Create(pairedPeerA, firstEndpoint.Object)
        ]);

        var result = GameItemComponent.GetConnectedPortPairs(firstEndpoint.Object, secondEndpoint.Object);

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("peer-a", result[0].Peer!.PortId);
        Assert.AreEqual("peer-b", result[1].Peer!.PortId);
    }

    [TestMethod]
    public void GenericParallelConnections_SaveAndRestoreExactPorts()
    {
        var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
        var items = new All<IGameItem>();
        world.SetupGet(x => x.Items).Returns(items);
        var connectorDefinitions = new[]
        {
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("powered", false), new XAttribute("id", "port-a")),
            new XElement("Connection", new XAttribute("gender", (short)Gender.Neuter), new XAttribute("type", "data"), new XAttribute("powered", false), new XAttribute("id", "port-b"))
        };
        var prototypeRow = CreatePrototype("Connectable", new ConnectorType(Gender.Neuter, "data"));
        prototypeRow.Definition = new XElement("Definition", new XElement("Connectors", connectorDefinitions)).ToString();
        var prototype = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(prototypeRow, world.Object)!;
        var firstParent = CreateParent(world.Object, 501);
        var secondParent = CreateParent(world.Object, 502);
        var first = new PersistedConnectable(CreateEmptyRow(501), prototype, firstParent.Object);
        var second = new PersistedConnectable(CreateEmptyRow(502), prototype, secondParent.Object);

        first.Connect(null, second);
        first.Connect(null, second);

        var savedFirst = first.Definition;
        var savedSecond = second.Definition;
        var firstEntries = savedFirst.Element("ConnectedItems")!.Elements("Item").ToArray();
        Assert.AreEqual(2, firstEntries.Length);
        Assert.AreEqual("port-a", (string?)firstEntries.Single(x => (string?)x.Attribute("portid") == "port-a").Attribute("peerportid"));
        Assert.AreEqual("port-b", (string?)firstEntries.Single(x => (string?)x.Attribute("portid") == "port-b").Attribute("peerportid"));

        var reorderedDefinitions = connectorDefinitions.Reverse().ToArray();
        prototypeRow.Definition = new XElement("Definition", new XElement("Connectors", reorderedDefinitions)).ToString();
        var reloadedPrototype = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(prototypeRow, world.Object)!;
        savedFirst.Element("ConnectedItems")!.ReplaceNodes(savedFirst.Element("ConnectedItems")!.Elements("Item").Reverse());
        savedSecond.Element("ConnectedItems")!.ReplaceNodes(savedSecond.Element("ConnectedItems")!.Elements("Item").Reverse());
        var nextItems = new All<IGameItem>();
        world.SetupGet(x => x.Items).Returns(nextItems);
        var nextFirstParent = CreateParent(world.Object, 501);
        var nextSecondParent = CreateParent(world.Object, 502);
        var reloadedFirst = new PersistedConnectable(CreateRow(502, savedFirst), reloadedPrototype, nextFirstParent.Object);
        var reloadedSecond = new PersistedConnectable(CreateRow(501, savedSecond), reloadedPrototype, nextSecondParent.Object);
        nextFirstParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([reloadedFirst]);
        nextSecondParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([reloadedSecond]);
        nextItems.Add(nextSecondParent.Object);
        nextItems.Add(nextFirstParent.Object);

        reloadedSecond.FinaliseLoad();
        reloadedFirst.FinaliseLoad();

        Assert.AreEqual(2, reloadedFirst.ConnectedItems.Count());
        Assert.AreEqual(2, reloadedSecond.ConnectedItems.Count());
        var restoredPairs = reloadedFirst.Definition.Element("ConnectedItems")!.Elements("Item")
            .ToDictionary(x => (string)x.Attribute("portid")!, x => (string)x.Attribute("peerportid")!);
        Assert.AreEqual("port-a", restoredPairs["port-a"]);
        Assert.AreEqual("port-b", restoredPairs["port-b"]);
    }

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void PersistedBatteryConnection_LatePeer_RestoresBothExactPorts(bool batteryRestoresFirst)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var items = new All<IGameItem>();
		world.SetupGet(x => x.Items).Returns(items);
		var batteryParent = CreateParent(world.Object, 1);
		var peerParent = CreateParent(world.Object, 2);
		var batteryPort = new ConnectorType(Gender.Female, "power", powered: false);
		var peerPort = new ConnectorType(Gender.Male, "power", powered: false);
		var batteryProto = (BatteryPoweredGameItemComponentProto)new GameItemComponentManager().GetProto(
			CreatePrototype("BatteryPowered", batteryPort,
				new XElement("BatteryQuantity", 4), new XElement("BatteryType", "AAA"),
				new XElement("BatteriesInSeries", true), new XElement("Transparent", false),
				new XElement("ContentsPreposition", "in")), world.Object)!;
		var peerProto = (ConnectableGameItemComponentProto)new GameItemComponentManager().GetProto(
			CreatePrototype("Connectable", peerPort), world.Object)!;
		var battery = new PersistedBattery(CreateRow(2, batteryPort), batteryProto, batteryParent.Object);
		var peer = (IConnectable)peerProto.LoadComponent(CreateRow(1, peerPort), peerParent.Object);
		batteryParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([battery]);
		peerParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([peer]);

		items.Add(batteryRestoresFirst ? batteryParent.Object : peerParent.Object);
		var first = batteryRestoresFirst ? (IGameItemComponent)battery : peer;
		first.FinaliseLoad();
		Assert.IsFalse(battery.ConnectedItems.Any());
		Assert.AreEqual(batteryPort.ToString(), battery.Definition.Element("ConnectedItems")!
			.Elements("Item").Single().Attribute("connectiontype")!.Value);

		items.Add(batteryRestoresFirst ? peerParent.Object : batteryParent.Object);
		first.Login();
		peer.FinaliseLoad();
		battery.FinaliseLoad();
		Assert.AreEqual(batteryPort, battery.ConnectedItems.Single().Item1);
		Assert.AreEqual(peerPort, peer.ConnectedItems.Single().Item1);
		Assert.AreEqual(batteryPort.ToString(), battery.Definition.Element("ConnectedItems")!
			.Elements("Item").Single().Attribute("connectiontype")!.Value);
		Assert.IsFalse(battery.Changed);
		Assert.IsFalse(peer.Changed);
	}

	[TestMethod]
	public void PersistedParallelConnections_ReorderedPrototypePortsAndSavedRows_RestoreCorrectPhysicalPairs()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var items = new All<IGameItem>();
		world.SetupGet(x => x.Items).Returns(items);
		var batteryParent = CreateParent(world.Object, 301);
		var peerParent = CreateParent(world.Object, 302);
		var manager = new GameItemComponentManager();
		var batteryProtoDefinition = new[]
		{
			new XElement("Connection", new XAttribute("gender", (short)Gender.Female), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "battery-left")),
			new XElement("Connection", new XAttribute("gender", (short)Gender.Female), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "battery-right")),
			new XElement("Connection", new XAttribute("gender", (short)Gender.Female), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "battery-unused"))
		};
		var peerProtoDefinition = new[]
		{
			new XElement("Connection", new XAttribute("gender", (short)Gender.Male), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "peer-left")),
			new XElement("Connection", new XAttribute("gender", (short)Gender.Male), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "peer-right")),
			new XElement("Connection", new XAttribute("gender", (short)Gender.Male), new XAttribute("type", "power"), new XAttribute("powered", false), new XAttribute("id", "peer-unused"))
		};
		var batteryProtoRow = CreatePrototype("BatteryPowered", new ConnectorType(Gender.Female, "power", false),
			new XElement("BatteryQuantity", 4), new XElement("BatteryType", "AAA"),
			new XElement("BatteriesInSeries", true), new XElement("Transparent", false),
			new XElement("ContentsPreposition", "in"));
		batteryProtoRow.Definition = new XElement("Definition", XElement.Parse(batteryProtoRow.Definition).Elements()
			.Where(x => x.Name != "Connectors"), new XElement("Connectors", batteryProtoDefinition)).ToString();
		var peerProtoRow = CreatePrototype("Connectable", new ConnectorType(Gender.Male, "power", false));
		peerProtoRow.Definition = new XElement("Definition", new XElement("Connectors", peerProtoDefinition)).ToString();
		var batteryProto = (BatteryPoweredGameItemComponentProto)manager.GetProto(batteryProtoRow, world.Object)!;
		var peerProto = (ConnectableGameItemComponentProto)manager.GetProto(peerProtoRow, world.Object)!;
		var battery = new PersistedBattery(CreateEmptyRow(401), batteryProto, batteryParent.Object);
		var peer = new PersistedConnectable(CreateEmptyRow(402), peerProto, peerParent.Object);
		batteryParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([battery]);
		peerParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([peer]);

		GameItemComponent.ConnectPair(battery, battery.Connections.First(x => x.PortId == "battery-left"),
			peer, peer.Connections.First(x => x.PortId == "peer-left"));
		GameItemComponent.ConnectPair(battery, battery.Connections.First(x => x.PortId == "battery-right"),
			peer, peer.Connections.First(x => x.PortId == "peer-right"));
		var savedBattery = battery.Definition;
		var savedPeer = peer.Definition;
		Assert.AreEqual("peer-left", savedBattery.Element("ConnectedItems")!.Elements("Item").First().Attribute("peerportid")!.Value);

		// Reauthor both connector lists in reverse order and reorder one side's persisted rows.
		batteryProtoRow.Definition = new XElement("Definition", XElement.Parse(batteryProtoRow.Definition).Elements()
			.Where(x => x.Name != "Connectors"), new XElement("Connectors", batteryProtoDefinition.Reverse())).ToString();
		peerProtoRow.Definition = new XElement("Definition", new XElement("Connectors", peerProtoDefinition.Reverse())).ToString();
		var reloadManager = new GameItemComponentManager();
		var reloadedBatteryProto = (BatteryPoweredGameItemComponentProto)reloadManager.GetProto(batteryProtoRow, world.Object)!;
		var reloadedPeerProto = (ConnectableGameItemComponentProto)reloadManager.GetProto(peerProtoRow, world.Object)!;
		savedPeer.Element("ConnectedItems")!.ReplaceNodes(savedPeer.Element("ConnectedItems")!.Elements("Item").Reverse());
		var nextItems = new All<IGameItem>();
		world.SetupGet(x => x.Items).Returns(nextItems);
		var nextBatteryParent = CreateParent(world.Object, 301);
		var nextPeerParent = CreateParent(world.Object, 302);
		var reloadedBattery = new PersistedBattery(CreateRow(402, savedBattery), reloadedBatteryProto, nextBatteryParent.Object);
		var reloadedPeer = new PersistedConnectable(CreateRow(401, savedPeer), reloadedPeerProto, nextPeerParent.Object);
		nextBatteryParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([reloadedBattery]);
		nextPeerParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([reloadedPeer]);
		nextItems.Add(nextBatteryParent.Object);
		nextItems.Add(nextPeerParent.Object);

		reloadedPeer.FinaliseLoad();
		reloadedBattery.FinaliseLoad();

		Assert.AreEqual(2, reloadedBattery.ConnectedItems.Count());
		Assert.AreEqual(2, reloadedPeer.ConnectedItems.Count());
		Assert.AreEqual("battery-unused", reloadedBattery.FreeConnections.Single().PortId);
		Assert.AreEqual("peer-unused", reloadedPeer.FreeConnections.Single().PortId);
		var reloadedPairs = reloadedBattery.Definition.Element("ConnectedItems")!.Elements("Item")
			.ToDictionary(x => (string)x.Attribute("portid")!, x => (string)x.Attribute("peerportid")!);
		Assert.AreEqual("peer-left", reloadedPairs["battery-left"]);
		Assert.AreEqual("peer-right", reloadedPairs["battery-right"]);
	}

	private static Mock<IGameItem> CreateParent(IFuturemud world, long id)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(world);
		parent.SetupGet(x => x.Id).Returns(id);
		parent.Setup(x => x.ColocatedWith(It.IsAny<IGameItem>())).Returns(true);
		return parent;
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void PersistedDripConnection_LatePeer_ResolvesReciprocalPortAndDisconnects(bool dripLoadsFirst)
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var items = new All<IGameItem>();
		world.SetupGet(x => x.Items).Returns(items);
		var left = CreateParent(world.Object, 10);
		var right = CreateParent(world.Object, 20);
		var savedLeft = new ConnectorType(Gender.Male, "medical");
		var savedRight = new ConnectorType(Gender.Female, "medical", powered: false);
		var definitions = new GameItemComponentManager();
		var dripDefinition = CreatePrototype("Drip", savedLeft);
		dripDefinition.Definition = "<Definition><Connectors><Connection gender=\"" +
			(short)savedLeft.Gender + "\" type=\"medical\" /></Connectors></Definition>";
		var dripProto = (DripGameItemComponentProto)definitions.GetProto(dripDefinition, world.Object)!;
		var rightProto = (ConnectableGameItemComponentProto)definitions.GetProto(
			CreatePrototype("Connectable", savedRight), world.Object)!;
		var row = CreateRow(20, savedLeft);
		var serialized = XElement.Parse(row.Definition);
		serialized.Add(new XElement("RatePerMinute", 1));
		row.Definition = serialized.ToString();
		var drip = new PersistedDrip(row, dripProto, left.Object);
		var peer = (IConnectable)rightProto.LoadComponent(CreateRow(10, savedRight), right.Object);
		left.Setup(x => x.GetItemTypes<IConnectable>()).Returns([drip]);
		right.Setup(x => x.GetItemTypes<IConnectable>()).Returns([peer]);
		items.Add(dripLoadsFirst ? left.Object : right.Object);
		var first = dripLoadsFirst ? (IConnectable)drip : peer;
		first.FinaliseLoad();
		Assert.AreEqual(savedLeft.ToString(), drip.Definition.Element("ConnectedItems")!
			.Elements("Item").Single().Attribute("connectiontype")!.Value);

		items.Add(dripLoadsFirst ? right.Object : left.Object);
		first.Login();

		Assert.AreEqual(savedLeft, drip.ConnectedItems.Single().Item1);
		Assert.AreEqual(savedRight, peer.ConnectedItems.Single().Item1);
		Assert.IsFalse(drip.Changed);
		Assert.IsFalse(peer.Changed);
		drip.Delete();
		Assert.IsFalse(peer.ConnectedItems.Any());
	}

	private static DbComponent CreateRow(long peerId, ConnectorType port) => new()
	{
		Id = peerId + 100,
		Definition = new XElement("Definition", new XElement("ConnectedItems",
			new XElement("Item", new XAttribute("id", peerId), new XAttribute("connectiontype", port),
				new XAttribute("independent", true)))).ToString()
	};

	private static DbComponent CreateEmptyRow(long id) => new()
	{
		Id = id,
		Definition = new XElement("Definition", new XElement("ConnectedItems")).ToString()
	};

	private static DbComponent CreateRow(long peerId, XElement definition) => new()
	{
		Id = peerId + 100,
		Definition = definition.ToString()
	};

	[TestMethod]
	public void PersistedFeeder_ReplaysExactPoweredPortAndResumesUpstreamDraw()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var items = new All<IGameItem>();
		world.SetupGet(x => x.Items).Returns(items);
		var feederParent = CreateParent(world.Object, 10);
		var sourceParent = CreateParent(world.Object, 20);
		var feederPort = new ConnectorType(Gender.Male, "power");
		var sourcePort = new ConnectorType(Gender.Female, "power");
		var manager = new GameItemComponentManager();
		var feederProto = (ElectricGridFeederGameItemComponentProto)manager.GetProto(
			CreatePrototype("ElectricGridFeeder", feederPort), world.Object)!;
		var sourceProto = (ConnectableGameItemComponentProto)manager.GetProto(
			CreatePrototype("Connectable", sourcePort), world.Object)!;
		var feeder = (ElectricGridFeederGameItemComponent)feederProto.LoadComponent(CreateRow(20, feederPort), feederParent.Object);
		var remote = (IConnectable)sourceProto.LoadComponent(CreateRow(10, sourcePort), sourceParent.Object);
		var supply = new Mock<IProducePower>();
		sourceParent.Setup(x => x.GetItemTypes<IProducePower>()).Returns([supply.Object]);
		sourceParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([remote]);
		feederParent.Setup(x => x.GetItemTypes<IConnectable>()).Returns([feeder]);
		supply.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(true);
		supply.SetupGet(x => x.MaximumPowerInWatts).Returns(100);
		items.Add(feederParent.Object);
		items.Add(sourceParent.Object);

		feeder.FinaliseLoad();
		Assert.AreEqual(feederPort, feeder.ConnectedItems.Single().Item1);
		Assert.AreEqual(sourcePort, remote.ConnectedItems.Single().Item1);
		supply.Verify(x => x.BeginDrawdown(feeder), Times.Never);

		feeder.Login();
		feeder.Login();
		Assert.AreEqual(100.0, feeder.MaximumPowerInWatts);
		supply.Verify(x => x.BeginDrawdown(feeder), Times.Once);
		feeder.Quit();
		Assert.AreEqual(0.0, feeder.MaximumPowerInWatts);
		supply.Verify(x => x.EndDrawdown(feeder), Times.Once);
		feeder.Login();
		Assert.AreEqual(100.0, feeder.MaximumPowerInWatts);
		supply.Verify(x => x.BeginDrawdown(feeder), Times.Exactly(2));
	}

	private static DbPrototype CreatePrototype(string type, ConnectorType port, params XElement[] settings) => new()
	{
		Type = type,
		Name = type,
		EditableItem = new MudSharp.Models.EditableItem(),
        Definition = new XElement("Definition", settings, new XElement("Connectors",
            new XElement("Connection", new XAttribute("gender", (short)port.Gender),
                new XAttribute("type", port.ConnectionType), new XAttribute("powered", port.Powered == false ? true : port.Powered)),
            port.Powered == false
                ? new XElement("Connection", new XAttribute("gender", (short)port.Gender),
                    new XAttribute("type", port.ConnectionType), new XAttribute("powered", false))
                : null)).ToString()
	};

	private sealed class PersistedBattery(DbComponent row, BatteryPoweredGameItemComponentProto prototype, IGameItem parent)
		: BatteryPoweredGameItemComponent(row, prototype, parent)
	{
		public XElement Definition => XElement.Parse(SaveToXml());
	}

	private sealed class PersistedDrip(DbComponent row, DripGameItemComponentProto prototype, IGameItem parent)
		: DripGameItemComponent(row, prototype, parent)
	{
		public XElement Definition => XElement.Parse(SaveToXml());
	}

	private sealed class PersistedConnectable(DbComponent row, ConnectableGameItemComponentProto prototype, IGameItem parent)
		: ConnectableGameItemComponent(row, prototype, parent)
	{
		public XElement Definition => XElement.Parse(SaveToXml());
	}
}
