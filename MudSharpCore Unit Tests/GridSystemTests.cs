#nullable enable
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Construction.Grids;
using MudSharp.Events;
using MudSharp.Events.Hooks;
using MudSharp.Form.Audio;
using MudSharp.Form.Material;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class GridSystemTests
{
	[TestMethod]
	public void FaxMachine_FullQueueRejectsRoutedAndDirectFaxesThenPrintingFreesSpace()
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("FaxMaximumPendingJobs")).Returns("2");
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateRoom().Object, "555", 4);
		var sender = new FaxMachineDouble(gameworld.Object, 1) { TelecommunicationsGrid = grid };
		var receiver = CreateNativeFax(gameworld.Object, 2);
		((ICanConnectToTelecommunicationsGrid)receiver).TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)sender);
		receiver.OnPowerCutIn();
		var writing = new Mock<IWriting>();
		writing.SetupGet(x => x.Id).Returns(42);
		writing.SetupGet(x => x.DocumentLength).Returns(10);

		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [writing.Object], out var error), error);
		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [writing.Object], out error), error);
		Assert.IsFalse(receiver.CanReceiveFaxes);
		Assert.IsFalse(grid.TrySendFax(sender, receiver.PhoneNumber!, [writing.Object], out error));
		for (var i = 0; i < 100; i++) receiver.ReceiveFax("5550001", [writing.Object]);
		Assert.AreEqual(2, SaveFaxDefinition(receiver).Descendants("Fax").Count());

		var paper = CreateBasicItem(gameworld.Object, 3);
		paper.Setup(x => x.Equals(It.IsAny<IGameItem>())).Returns<IGameItem>(other => ReferenceEquals(other, paper.Object));
		var writeable = new Mock<IWriteable>();
		writeable.Setup(x => x.CanAddWriting(writing.Object)).Returns(true);
		writeable.Setup(x => x.AddWriting(writing.Object)).Returns(true);
		paper.Setup(x => x.GetItemType<IWriteable>()).Returns(writeable.Object);
		receiver.Put(null, paper.Object);
		writeable.Verify(x => x.AddWriting(writing.Object), Times.Once);
		Assert.AreEqual(1, SaveFaxDefinition(receiver).Descendants("Fax").Count());
		Assert.IsTrue(receiver.CanReceiveFaxes);
		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [writing.Object], out error), error);
		Assert.AreEqual(2, SaveFaxDefinition(receiver).Descendants("Fax").Count());
	}

	[TestMethod]
	public void FaxMachine_OversizedDocumentIsRejectedByRouterAndReceiver()
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("FaxMaximumDocumentReadables")).Returns("2");
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateRoom().Object, "555", 4);
		var sender = new FaxMachineDouble(gameworld.Object, 1) { TelecommunicationsGrid = grid };
		var receiver = CreateNativeFax(gameworld.Object, 2);
		((ICanConnectToTelecommunicationsGrid)receiver).TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)sender);
		receiver.OnPowerCutIn();
		ICanBeRead[] document = [CreateReadableDocument(10), CreateReadableDocument(20), CreateReadableDocument(30)];
		Assert.IsFalse(grid.TrySendFax(sender, receiver.PhoneNumber!, document, out var error));
		StringAssert.Contains(error, "too large");
		receiver.ReceiveFax(sender.PhoneNumber!, document);
		Assert.AreEqual(0, SaveFaxDefinition(receiver).Descendants("Fax").Count());
		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, document.Take(2).ToArray(), out error), error);
		Assert.AreEqual(2, SaveFaxDefinition(receiver).Descendants("Fax").Single().Elements().Count());
	}

	[TestMethod]
	public void FaxMachine_LoadPreservesLegacyJobsAboveLoweredQueueLimit()
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("FaxMaximumPendingJobs")).Returns("1");
		var drawing = new Mock<IDrawing>();
		drawing.SetupGet(x => x.Id).Returns(42);
		gameworld.SetupGet(x => x.Drawings).Returns(CreateCollection([drawing.Object]).Object);
		var receiver = CreateNativeFax(gameworld.Object, 2,
			"<Definition><PendingFaxes><Fax sender='5550001'><Drawing>42</Drawing></Fax><Fax sender='5550001'><Drawing>42</Drawing></Fax></PendingFaxes></Definition>");
		((ICanConnectToTelecommunicationsGrid)receiver).TelecommunicationsGrid = Mock.Of<ITelecommunicationsGrid>();
		receiver.AssignPhoneNumber("5550002");
		receiver.OnPowerCutIn();
		Assert.IsFalse(receiver.CanReceiveFaxes);
		receiver.ReceiveFax("5550001", [drawing.Object]);
		Assert.AreEqual(2, SaveFaxDefinition(receiver).Descendants("Fax").Count());
	}

	private static FaxMachineGameItemComponent CreateNativeFax(IFuturemud gameworld, long id, string? definition = null)
	{
		var proto = new FaxMachineGameItemComponentProto(new MudSharp.Models.GameItemComponentProto
		{
			Id = 50, Name = "Fax", Description = "Test", RevisionNumber = 1,
			Definition = "<Definition><Wattage>2</Wattage><RingEmote><![CDATA[@ ring|rings.]]></RingEmote><TransmitPremote><![CDATA[@ speak|speaks.]]></TransmitPremote><PaperWeightCapacity>1.5</PaperWeightCapacity></Definition>",
			EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = (int)RevisionStatus.Current, RevisionNumber = 1 }
		}, gameworld);
		var parent = CreateBasicItem(gameworld, id).Object;
		return definition == null
			? new FaxMachineGameItemComponent(proto, parent, true)
			: new FaxMachineGameItemComponent(new MudSharp.Models.GameItemComponent { Definition = definition }, proto, parent);
	}

	private static XElement SaveFaxDefinition(FaxMachineGameItemComponent fax) => XElement.Parse((string)typeof(FaxMachineGameItemComponent)
		.GetMethod("SaveToXml", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(fax, null)!);

	[DataTestMethod]
	[DataRow("full")]
	[DataRow("protected")]
	[DataRow("removed")]
	public void AnsweringMachine_GreetingStopsBeforeUnstorableSegmentsAndReleasesBuffer(string restriction)
	{
		var gameworld = CreateGameworld();
		var room = CreateRoom().Object;
		var parent = CreateBasicItem(gameworld.Object, 230, room);
		var tape = CreateAudioMedium(gameworld.Object, 231, 0.03);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2), parent.Object, true);
		var actor = CreateCharacter(gameworld.Object, 232, room);
		var language = CreateLanguage(301);
		var accent = CreateAccent(302, language.Object);
		machine.Put(actor.Object, tape.Item.Object);
		Assert.IsTrue(machine.Select(actor.Object, "greeting record", Mock.Of<IEmote>()));
		machine.HandleEvent(EventType.CharacterSpeaksWitness, actor.Object, actor.Object, AudioVolume.Decent,
			language.Object, accent.Object, "Hello");
		Assert.IsTrue(machine.IsRecordingGreeting);
		if (restriction == "protected") tape.Component.WriteProtected = true;
		if (restriction == "removed") Assert.IsTrue(machine.Take(tape.Item.Object));
		for (var i = 0; i < 100; i++)
		{
			machine.HandleEvent(EventType.CharacterSpeaksWitness, actor.Object, actor.Object, AudioVolume.Decent,
				language.Object, accent.Object, "Goodbye");
		}
		Assert.IsFalse(machine.IsRecordingGreeting);
		var working = (List<RecordedAudioSegment>)typeof(AnsweringMachineGameItemComponent)
			.GetField("_workingGreetingSegments", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(machine)!;
		Assert.AreEqual(0, working.Count);
		if (restriction == "full")
		{
			Assert.IsNotNull(machine.GreetingRecording);
			Assert.AreEqual(1, machine.GreetingRecording.Recording.Segments.Count);
			Assert.AreEqual("Hello", machine.GreetingRecording.Recording.Segments[0].RawText);
			Assert.IsTrue(machine.GreetingRecording.Recording.TotalDuration <= tape.Component.Capacity);
		}
		else
		{
			Assert.IsNull(machine.GreetingRecording);
		}
	}

	[DataTestMethod]
	[DataRow("segments", 10, 2)]
	[DataRow("characters", 10, 2)]
	[DataRow("characters", 21, 0)]
	public void HostedVoicemail_EndsAndSavesOnlyBoundedRecording(string limit, int textLength, int expectedSegments)
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("HostedVoicemailMaximumSegmentsPerMessage"))
			.Returns(limit == "segments" ? "2" : "64");
		gameworld.Setup(x => x.GetStaticConfiguration("HostedVoicemailMaximumCharactersPerMessage"))
			.Returns(limit == "characters" ? "20" : "8192");
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4, true);
		grid.SetMaximumRings(1);
		var caller = new TelephoneDouble(gameworld.Object, 1) { TelecommunicationsGrid = grid };
		var receiver = new TelephoneDouble(gameworld.Object, 2) { TelecommunicationsGrid = grid, HostedVoicemailEnabled = true };
		Mock.Get(caller.Parent).SetupGet(x => x.Name).Returns("Caller");
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);
		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
		for (var i = 0; i < 4; i++)
		{
			caller.Transmit(CreateSpokenLanguage(caller.Parent, new string('a', textLength)));
		}
		Assert.IsFalse(caller.IsEngaged);
		Assert.IsTrue(caller.ProgressMessages.Any(x => x.Contains("message limit")));
		var saved = SaveTelecomDefinition(grid);
		Assert.AreEqual(expectedSegments, saved.Descendants("Segment").Count());
		Assert.AreEqual(expectedSegments == 0 ? 0 : 1, saved.Descendants("StoredRecording").Count());
	}

	[TestMethod]
	public void HostedVoicemail_MailboxAndGridQuotasRefuseNewMessagesAndDeletionFreesCapacity()
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("HostedVoicemailMaximumMessagesPerMailbox")).Returns("2");
		gameworld.Setup(x => x.GetStaticConfiguration("HostedVoicemailMaximumMessagesPerGrid")).Returns("3");
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4, true);
		grid.SetMaximumRings(1);
		var caller = new TelephoneDouble(gameworld.Object, 1) { TelecommunicationsGrid = grid };
		var receivers = Enumerable.Range(2, 3).Select(id => new TelephoneDouble(gameworld.Object, id)
		{
			TelecommunicationsGrid = grid, HostedVoicemailEnabled = true
		}).ToArray();
		Mock.Get(caller.Parent).SetupGet(x => x.Name).Returns("Caller");
		foreach (var phone in receivers.Prepend(caller)) grid.JoinGrid((ITelephoneNumberOwner)phone);
		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
		void LeaveMessage(TelephoneDouble receiver, bool shouldRecord)
		{
			Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
			heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
			heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
			Assert.AreEqual(shouldRecord, caller.IsConnected);
			if (shouldRecord)
			{
				caller.Transmit(CreateSpokenLanguage(caller.Parent));
				Assert.IsTrue(caller.HangUp(null!, out error), error);
			}
			Assert.IsFalse(caller.IsEngaged);
		}
		LeaveMessage(receivers[0], true);
		LeaveMessage(receivers[0], true);
		LeaveMessage(receivers[0], false);
		LeaveMessage(receivers[1], true);
		LeaveMessage(receivers[2], false);
		Assert.AreEqual(3, SaveTelecomDefinition(grid).Descendants("StoredRecording").Count());
		Assert.IsTrue(receivers[0].Dial(null!, grid.HostedVoicemailAccessNumber, out var error), error);
		Assert.IsTrue(receivers[0].Dial(null!, "7", out error), error);
		Assert.IsTrue(receivers[0].Dial(null!, "#", out error), error);
		LeaveMessage(receivers[2], true);
		Assert.AreEqual(2, SaveTelecomDefinition(grid).Descendants("StoredRecording").Count());
	}

	[TestMethod]
	public void HostedVoicemail_LoadPreservesLegacyMessagesButRefusesAdditionalOverQuotaStorage()
	{
		var gameworld = CreateGameworld();
		gameworld.Setup(x => x.GetStaticConfiguration("HostedVoicemailMaximumMessagesPerMailbox")).Returns("1");
		var speech = RecordedAudioSegment.FromSpokenLanguage(CreateSpokenLanguage(Mock.Of<IGameItem>(x => x.Name == "Caller")), TimeSpan.Zero);
		var recording = new StoredAudioRecording("legacy", new RecordedAudio([speech]), DateTime.UtcNow);
		var definition = new XElement("Definition", new XElement("HostedVoicemailMailboxes",
			new XElement("Mailbox", new XAttribute("number", "5551200"), recording.SaveToXml(), recording.SaveToXml())));
		var grid = new TelecommunicationsGrid(new MudSharp.Models.Grid
		{
			Id = 1, GridType = "Telecommunications", Definition = definition.ToString()
		}, gameworld.Object);
		var admitted = (bool)typeof(TelecommunicationsGrid).GetMethod("StoreHostedVoicemailMessage", BindingFlags.NonPublic | BindingFlags.Instance)!
			.Invoke(grid, ["5551200", recording])!;
		Assert.IsFalse(admitted);
		Assert.AreEqual(2, SaveTelecomDefinition(grid).Descendants("StoredRecording").Count());
	}

	private static XElement SaveTelecomDefinition(TelecommunicationsGrid grid) =>
		(XElement)typeof(TelecommunicationsGrid).GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(grid, [])!;

	[TestMethod]
	public void TelecomFeeders_RejectMutualAndLongerCyclesWithoutLosingExistingPower()
	{
		var gameworld = CreateGameworld();
		var (aItem, a) = CreateTelecomFeeder(gameworld.Object);
		var (bItem, b) = CreateTelecomFeeder(gameworld.Object);
		var (cItem, c) = CreateTelecomFeeder(gameworld.Object);
		var connector = new ConnectorType(Gender.Neuter, "power", true);
		var rootItem = new Mock<IGameItem>();
		var root = new Mock<IProducePower>();
		root.SetupGet(x => x.Parent).Returns(rootItem.Object);
		root.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(true);
		root.SetupGet(x => x.MaximumPowerInWatts).Returns(100);
		root.SetupGet(x => x.ProducingPower).Returns(true);
		root.SetupGet(x => x.FuelLevel).Returns(9);
		root.Setup(x => x.CanBeginDrawDown(10)).Returns(true);
		root.Setup(x => x.CanDrawdownSpike(10)).Returns(true);
		root.Setup(x => x.DrawdownSpike(10)).Returns(true);
		rootItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([root.Object]);
		var rootConnection = new Mock<IConnectable>();
		rootConnection.SetupGet(x => x.Parent).Returns(rootItem.Object);
		aItem.Raise(x => x.OnConnected += null, rootConnection.Object, connector);
		bItem.Raise(x => x.OnConnected += null, rootConnection.Object, connector);
		aItem.Raise(x => x.OnConnected += null, b, connector);
		bItem.Raise(x => x.OnConnected += null, a, connector);
		Assert.AreEqual(100, a.MaximumPowerInWatts);
		Assert.AreEqual(100, b.MaximumPowerInWatts);
		root.Verify(x => x.EndDrawdown(a), Times.Once);
		root.Verify(x => x.EndDrawdown(b), Times.Never);
		cItem.Raise(x => x.OnConnected += null, a, connector);
		bItem.Raise(x => x.OnConnected += null, c, connector);
		bItem.Raise(x => x.OnConnected += null, b, connector);
		Assert.AreEqual(100, c.PowerConsumptionInWatts);
		Assert.AreEqual(9, c.FuelLevel);
		Assert.IsTrue(c.ProducingPower);
		Assert.IsTrue(c.CanBeginDrawDown(10));
		Assert.IsTrue(c.CanDrawdownSpike(10));
		Assert.IsTrue(c.DrawdownSpike(10));
		aItem.Raise(x => x.OnDisconnected += null, b, connector);
		Assert.AreEqual(0, a.MaximumPowerInWatts);
		Assert.AreEqual(0, c.MaximumPowerInWatts);
		Assert.AreEqual(100, b.MaximumPowerInWatts);
	}

	[TestMethod]
	public void TelecomFeeder_AcyclicSourceReplacementSupportsNonPrimaryPowerAndReleasesOldSource()
	{
		var gameworld = CreateGameworld();
		var (item, feeder) = CreateTelecomFeeder(gameworld.Object);
		var connector = new ConnectorType(Gender.Neuter, "power", true);
		var first = new Mock<IProducePower>();
		first.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(true);
		first.SetupGet(x => x.MaximumPowerInWatts).Returns(100);
		var second = new Mock<IProducePower>();
		second.SetupGet(x => x.MaximumPowerInWatts).Returns(50);
		foreach (var source in new[] { first, second })
		{
			var sourceItem = new Mock<IGameItem>();
			sourceItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([source.Object]);
			var connection = new Mock<IConnectable>();
			connection.SetupGet(x => x.Parent).Returns(sourceItem.Object);
			item.Raise(x => x.OnConnected += null, connection.Object, connector);
			if (source == second)
			{
				item.Raise(x => x.OnConnected += null, connection.Object, connector);
			}
		}
		Assert.AreEqual(50, feeder.MaximumPowerInWatts);
		first.Verify(x => x.EndDrawdown(feeder), Times.Once);
		second.Verify(x => x.BeginDrawdown(feeder), Times.Once);
	}

	private static (Mock<IGameItem> Item, TelecommunicationsGridFeederGameItemComponent Feeder) CreateTelecomFeeder(IFuturemud gameworld)
	{
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 124, Name = "Feeder", Description = "Test", RevisionNumber = 1,
			Definition = "<Definition><Connectors><Connection gender='0' type='power' powered='true'/></Connectors></Definition>",
			EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = (int)RevisionStatus.Current, RevisionNumber = 1 }
		};
		var proto = (TelecommunicationsGridFeederGameItemComponentProto)typeof(TelecommunicationsGridFeederGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([model, gameworld]);
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		var feeder = new TelecommunicationsGridFeederGameItemComponent(proto, item.Object, true);
		item.Setup(x => x.GetItemTypes<IProducePower>()).Returns([feeder]);
		return (item, feeder);
	}

	[TestMethod]
	public void CellularCoverage_UsesRegisteredTowersAndLivePowerSwitchLocation()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, null, "555", 4);
		var otherGrid = new TelecommunicationsGrid(gameworld.Object, null, "556", 4);
		var zone = new Mock<IZone>().Object;
		var otherZone = new Mock<IZone>().Object;
		var room = CreateRoom();
		room.SetupGet(x => x.Zone).Returns(zone);
		var otherRoom = CreateRoom();
		otherRoom.SetupGet(x => x.Zone).Returns(otherZone);
		IRoom towerRoom = room.Object;
		var towerItem = new Mock<IGameItem>();
		towerItem.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		towerItem.SetupGet(x => x.Id).Returns(12);
		towerItem.SetupGet(x => x.TrueLocations).Returns(() => new[] { towerRoom });
		var items = Mock.Get(gameworld.Object.Items);
		var towerLoaded = true;
		items.Setup(x => x.Get(12L)).Returns(() => towerLoaded ? towerItem.Object : null);
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 123, Name = "Tower", Description = "Test", RevisionNumber = 1,
			Definition = "<Definition><Wattage>250</Wattage></Definition>",
			EditableItem = new MudSharp.Models.EditableItem { RevisionStatus = (int)RevisionStatus.Current, RevisionNumber = 1 }
		};
		var proto = (CellPhoneTowerGameItemComponentProto)typeof(CellPhoneTowerGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([model, gameworld.Object]);
		var tower = new CellPhoneTowerGameItemComponent(proto, towerItem.Object, true);
		var phoneItem = new Mock<IGameItem>();
		phoneItem.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		phoneItem.SetupGet(x => x.TrueLocations).Returns([room.Object]);
		var phone = new CellularPhoneGameItemComponent(CreateCellularPhoneProto(gameworld.Object, 2), phoneItem.Object, true)
		{
			TelecommunicationsGrid = grid
		};
		gameworld.Invocations.Clear();
		Assert.IsFalse(phone.HasCoverage);
		tower.TelecommunicationsGrid = grid;
		tower.OnPowerCutIn();
		Assert.IsTrue(phone.HasCoverage);
		tower.SwitchedOn = false;
		Assert.IsFalse(phone.HasCoverage);
		tower.SwitchedOn = true;
		tower.OnPowerCutOut();
		Assert.IsFalse(phone.HasCoverage);
		tower.OnPowerCutIn();
		towerRoom = otherRoom.Object;
		Assert.IsFalse(phone.HasCoverage);
		towerRoom = room.Object;
		Assert.IsTrue(phone.HasCoverage);
		tower.TelecommunicationsGrid = otherGrid;
		tower.OnPowerCutIn();
		grid.LinkGrid(otherGrid);
		Assert.IsFalse(phone.HasCoverage);
		tower.TelecommunicationsGrid = grid;
		tower.OnPowerCutIn();
		Assert.IsTrue(phone.HasCoverage);
		towerLoaded = false;
		Assert.IsFalse(phone.HasCoverage);
		towerLoaded = true;
		Assert.IsTrue(phone.HasCoverage);
		tower.TelecommunicationsGrid = null;
		Assert.IsFalse(phone.HasCoverage);
		items.Verify(x => x.GetEnumerator(), Times.Never);
	}

	[TestMethod]
	public void CellularCoverage_RestoresTowerRegistryFromSavedConsumers()
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(12);
		var tower = new Mock<ICellPhoneTower>();
		tower.SetupGet(x => x.Parent).Returns(item.Object);
		item.Setup(x => x.GetItemType<IConsumePower>()).Returns(tower.Object);
		var gameworld = CreateGameworld([item.Object]);
		var grid = new TelecommunicationsGrid(new MudSharp.Models.Grid
		{
			Id = 1, GridType = "Telecommunications",
			Definition = "<Definition><Consumer>12</Consumer></Definition>"
		}, gameworld.Object);
		tower.SetupGet(x => x.TelecommunicationsGrid).Returns(grid);
		var zone = new Mock<IZone>().Object;
		tower.Setup(x => x.ProvidesCoverage(zone)).Returns(true);
		Assert.IsFalse(grid.HasCellularCoverage(zone));
		grid.LoadTimeInitialise();
		gameworld.Invocations.Clear();
		var items = Mock.Get(gameworld.Object.Items);
		items.Invocations.Clear();
		Assert.IsTrue(grid.HasCellularCoverage(zone));
		grid.LeaveGrid((IConsumePower)tower.Object);
		Assert.IsFalse(grid.HasCellularCoverage(zone));
		items.Verify(x => x.GetEnumerator(), Times.Never);
	}

    [TestMethod]
    public void GridPowerSupplyProto_RoundTripsWattage()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        GridPowerSupplyGameItemComponentProto proto = CreateGridPowerSupplyProto(gameworld.Object, 37.5);

        Assert.AreEqual(37.5, proto.Wattage, 0.0001);

        MethodInfo? saveMethod = typeof(GridPowerSupplyGameItemComponentProto).GetMethod("SaveToXml",
            BindingFlags.Instance | BindingFlags.NonPublic);
        string xml = (string)saveMethod!.Invoke(proto, [])!;
        StringAssert.Contains(xml, "<Wattage>37.5</Wattage>");
    }

    [TestMethod]
    public void GridPowerSupplyComponent_UsesConfiguredWattageAndDeduplicatesConsumers()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        GridPowerSupplyGameItemComponentProto proto = CreateGridPowerSupplyProto(gameworld.Object, 50.0);
        GridPowerSupplyGameItemComponent component = new(proto, parent.Object, true);

        Mock<IElectricalGrid> grid = new();
        grid.Setup(x => x.DrawdownSpike(10.0)).Returns(true);
        component.ElectricalGrid = grid.Object;
        component.OnPowerCutIn();

        Mock<IConsumePower> consumerOne = new();
        consumerOne.SetupGet(x => x.PowerConsumptionInWatts).Returns(20.0);
        Mock<IConsumePower> consumerTwo = new();
        consumerTwo.SetupGet(x => x.PowerConsumptionInWatts).Returns(15.0);

        component.BeginDrawdown(consumerOne.Object);
        component.BeginDrawdown(consumerOne.Object);
        component.BeginDrawdown(consumerTwo.Object);
        component.BeginDrawdown(consumerTwo.Object);

        Assert.AreEqual(50.0, component.MaximumPowerInWatts, 0.0001);
        Assert.AreEqual(35.0, component.PowerConsumptionInWatts, 0.0001);
        Assert.IsTrue(component.CanBeginDrawDown(15.0));
        Assert.IsFalse(component.CanBeginDrawDown(16.0));
        Assert.IsTrue(component.CanDrawdownSpike(10.0));
        Assert.IsFalse(component.CanDrawdownSpike(16.0));
        Assert.IsTrue(component.DrawdownSpike(10.0));

        consumerOne.Verify(x => x.OnPowerCutIn(), Times.Once);
        consumerTwo.Verify(x => x.OnPowerCutIn(), Times.Once);
        grid.Verify(x => x.DrawdownSpike(10.0), Times.Once);
    }

    [TestMethod]
    public void GridPowerSupplyComponent_CountsPendingConsumersAgainstWattageCap()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        GridPowerSupplyGameItemComponentProto proto = CreateGridPowerSupplyProto(gameworld.Object, 50.0);
        GridPowerSupplyGameItemComponent component = new(proto, parent.Object, true);

        Mock<IConsumePower> consumerOne = new();
        consumerOne.SetupGet(x => x.PowerConsumptionInWatts).Returns(20.0);
        Mock<IConsumePower> consumerTwo = new();
        consumerTwo.SetupGet(x => x.PowerConsumptionInWatts).Returns(15.0);

        component.BeginDrawdown(consumerOne.Object);
        component.BeginDrawdown(consumerTwo.Object);

        Assert.IsTrue(component.CanBeginDrawDown(15.0));
        Assert.IsFalse(component.CanBeginDrawDown(16.0));
        Assert.AreEqual(0.0, component.PowerConsumptionInWatts, 0.0001);
    }

    [TestMethod]
    public void LiquidGrid_CurrentMixtureWeightsSuppliersByVolume()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        LiquidGrid grid = new(gameworld.Object, CreateRoom().Object);
        ILiquid water = CreateLiquid(1, "water").Object;
        ILiquid toxin = CreateLiquid(2, "toxin", Telnet.Red).Object;
        SupplierDouble supplierOne = CreateSupplier(CreateMixture(gameworld.Object, (water, 3.0)));
        SupplierDouble supplierTwo = CreateSupplier(CreateMixture(gameworld.Object, (toxin, 1.0)));

        grid.JoinGrid(supplierOne.Object);
        grid.JoinGrid(supplierTwo.Object);

        LiquidMixture mixture = grid.CurrentLiquidMixture;
        Assert.AreEqual(4.0, mixture.TotalVolume, 0.0001);
        Assert.AreEqual(3.0, GetLiquidAmount(mixture, water), 0.0001);
        Assert.AreEqual(1.0, GetLiquidAmount(mixture, toxin), 0.0001);
    }

    [TestMethod]
    public void LiquidGrid_RemoveLiquidAmount_DepletesSuppliersProRata()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        LiquidGrid grid = new(gameworld.Object, CreateRoom().Object);
        ILiquid water = CreateLiquid(1, "water").Object;
        ILiquid toxin = CreateLiquid(2, "toxin", Telnet.Red).Object;
        SupplierDouble supplierOne = CreateSupplier(CreateMixture(gameworld.Object, (water, 3.0)));
        SupplierDouble supplierTwo = CreateSupplier(CreateMixture(gameworld.Object, (toxin, 1.0)));

        grid.JoinGrid(supplierOne.Object);
        grid.JoinGrid(supplierTwo.Object);
        LiquidMixture? removed = grid.RemoveLiquidAmount(2.0, null, "test");

        Assert.IsNotNull(removed);
        Assert.AreEqual(2.0, removed.TotalVolume, 0.0001);
        Assert.AreEqual(1.5, GetLiquidAmount(removed, water), 0.0001);
        Assert.AreEqual(0.5, GetLiquidAmount(removed, toxin), 0.0001);
        Assert.AreEqual(1.5, supplierOne.State.Mixture.TotalVolume, 0.0001);
        Assert.AreEqual(0.5, supplierTwo.State.Mixture.TotalVolume, 0.0001);
    }

    [TestMethod]
    public void LiquidGrid_CurrentMixtureReflectsSupplierRefill()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        LiquidGrid grid = new(gameworld.Object, CreateRoom().Object);
        ILiquid water = CreateLiquid(1, "water").Object;
        SupplierDouble supplier = CreateSupplier(CreateMixture(gameworld.Object, (water, 2.0)));
        grid.JoinGrid(supplier.Object);

        Assert.AreEqual(2.0, grid.TotalLiquidVolume, 0.0001);

        supplier.State.Mixture = CreateMixture(gameworld.Object, (water, 5.0));

        Assert.AreEqual(5.0, grid.TotalLiquidVolume, 0.0001);
        Assert.AreEqual(5.0, grid.CurrentLiquidMixture.TotalVolume, 0.0001);
    }

    [TestMethod]
    public void LiquidGrid_CopyDoesNotAttachRoomSuppliers()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        SupplierDouble supplier = CreateSupplier(CreateMixture(gameworld.Object, (CreateLiquid(1, "water").Object, 3.0)));
        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Id).Returns(10L);
        item.Setup(x => x.GetItemType<ILiquidGridSupplier>()).Returns(supplier.Object);
        Mock<IRoom> room = CreateRoom();
        room.SetupGet(x => x.GameItems).Returns([item.Object]);

        Mock<ILiquidGrid> original = new();
        original.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        original.SetupGet(x => x.Locations).Returns([room.Object]);

        LiquidGrid clone = new(original.Object);

        Assert.AreEqual(0.0, clone.TotalLiquidVolume, 0.0001);
        Assert.IsTrue(clone.CurrentLiquidMixture.IsEmpty);
    }

    [TestMethod]
    public void TelecommunicationsGrid_AssignsNumbersAndHonoursPreferredNumber()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble phoneOne = new(gameworld.Object, 1);
        TelephoneDouble phoneTwo = new(gameworld.Object, 2) { PreferredNumber = "5559999" };

        phoneOne.TelecommunicationsGrid = grid;
        phoneTwo.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)phoneOne);
        grid.JoinGrid((ITelephoneNumberOwner)phoneTwo);

        Assert.AreEqual("5559999", phoneTwo.PhoneNumber);
        Assert.IsFalse(string.IsNullOrWhiteSpace(phoneOne.PhoneNumber));
        Assert.AreNotEqual(phoneOne.PhoneNumber, phoneTwo.PhoneNumber);
        Assert.AreEqual(phoneOne.PhoneNumber, grid.GetPhoneNumber(phoneOne));
        Assert.AreEqual(phoneTwo.PhoneNumber, grid.GetPhoneNumber(phoneTwo));
    }

    [TestMethod]
    public void TelecommunicationsGrid_StartCallAnswerAndHangUp_TransitionsState()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        Assert.IsTrue(receiver.IsRinging);
        Assert.IsTrue(caller.IsEngaged);

        Assert.IsTrue(receiver.Answer(null!, out error), error);
        Assert.IsTrue(caller.IsConnected);
        Assert.IsTrue(receiver.IsConnected);
        Assert.AreSame(receiver, caller.ConnectedPhone);
        Assert.AreSame(caller, receiver.ConnectedPhone);

        Assert.IsTrue(caller.HangUp(null!, out error), error);
        Assert.IsFalse(caller.IsEngaged);
        Assert.IsFalse(receiver.IsEngaged);
    }

    [TestMethod]
    public void TelecommunicationsGrid_RejectsBusyInvalidAndUnavailableTargets()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);
        TelephoneDouble busy = new(gameworld.Object, 3);
        TelephoneDouble unpowered = new(gameworld.Object, 4) { IsPowered = false };

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        busy.TelecommunicationsGrid = grid;
        unpowered.TelecommunicationsGrid = grid;

        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);
        grid.JoinGrid((ITelephoneNumberOwner)busy);
        grid.JoinGrid((ITelephoneNumberOwner)unpowered);

        Assert.IsFalse(grid.TryStartCall(caller, "9999999", out string? error));
        StringAssert.Contains(error, "not connected");

        Assert.IsTrue(grid.TryStartCall(caller, busy.PhoneNumber!, out error), error);
        Assert.IsFalse(grid.TryStartCall(receiver, busy.PhoneNumber!, out error));
        StringAssert.Contains(error, "busy");

        caller.HangUp(null!, out _);

        Assert.IsFalse(grid.TryStartCall(caller, unpowered.PhoneNumber!, out error));
        StringAssert.Contains(error, "cannot receive");
    }

    [TestMethod]
    public void TelecommunicationsGrid_OffHookLineIsBusy_AndSharedExtensionsCanJoinActiveCall()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble primary = new(gameworld.Object, 2)
        {
            PreferredNumber = "5554444",
            AllowSharedNumber = true
        };
        TelephoneDouble extension = new(gameworld.Object, 3)
        {
            PreferredNumber = "5554444",
            AllowSharedNumber = true
        };
        TelephoneDouble blocked = new(gameworld.Object, 4);

        foreach (TelephoneDouble? phone in new[] { caller, primary, extension, blocked })
        {
            phone.TelecommunicationsGrid = grid;
            grid.JoinGrid((ITelephoneNumberOwner)phone);
        }

        Assert.IsTrue(blocked.PickUp(null!, out string? error), error);
        Assert.IsFalse(grid.TryStartCall(caller, blocked.PhoneNumber!, out error));
        StringAssert.Contains(error, "busy");
        Assert.IsTrue(blocked.HangUp(null!, out error), error);

        Assert.AreEqual(primary.PhoneNumber, extension.PhoneNumber);
        Assert.IsTrue(grid.TryStartCall(caller, primary.PhoneNumber!, out error), error);
        Assert.IsTrue(primary.IsRinging);
        Assert.IsTrue(extension.IsRinging);

        Assert.IsTrue(primary.Answer(null!, out error), error);
        Assert.IsTrue(caller.IsConnected);
        Assert.IsTrue(primary.IsConnected);
        Assert.IsFalse(extension.IsRinging);
        Assert.IsFalse(extension.IsConnected);

        Assert.IsTrue(extension.PickUp(null!, out error), error);
        CollectionAssert.AreEquivalent(new[] { primary, extension }, caller.ConnectedPhones.ToArray());
    }

    [TestMethod]
    public void TelecommunicationsGrid_RingsOutAfterConfiguredMaximumRings()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        grid.SetMaximumRings(2);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        CollectionAssert.Contains(caller.ProgressMessages, "You hear the line ringing.");

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
        CollectionAssert.AreEqual(
            new[] { "You hear the line ringing.", "You hear the line ringing." },
            caller.ProgressMessages);

        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
        CollectionAssert.Contains(caller.ProgressMessages, "The line rings out.");
        Assert.IsFalse(caller.IsEngaged);
        Assert.IsFalse(receiver.IsEngaged);
    }

    [TestMethod]
    public void TelecommunicationsGrid_ReportsConnectionProgressToCaller()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        Assert.IsTrue(receiver.Answer(null!, out error), error);

        CollectionAssert.Contains(caller.ProgressMessages, "The call connects.");
    }

    [TestMethod]
    public void TelecommunicationsGrid_PhoneDiallingFaxLine_PlaysModemNoiseAndDisconnects()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        FaxMachineDouble fax = new(gameworld.Object, 2);

        caller.TelecommunicationsGrid = grid;
        fax.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)fax);

        Assert.IsTrue(grid.TryStartCall(caller, fax.PhoneNumber!, out string? error), error);
        CollectionAssert.Contains(caller.ProgressMessages,
            "The line answers with a burst of unintelligible modem-like noises before disconnecting.");
        Assert.IsFalse(caller.IsEngaged);
    }

    [TestMethod]
    public void TelecommunicationsGrid_FaxToFax_DeliversDocument()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        FaxMachineDouble sender = new(gameworld.Object, 1);
        FaxMachineDouble receiver = new(gameworld.Object, 2);
        ICanBeRead document = CreateReadableDocument(120);

        sender.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)sender);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out string? error), error);
        Assert.AreEqual(1, receiver.CompletedFaxCount);
        Assert.AreEqual(0, receiver.PendingFaxCount);
    }

    [TestMethod]
    public void TelecommunicationsGrid_FaxToVoiceLine_NotifiesRecipientAndFails()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        FaxMachineDouble sender = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);
        ICanBeRead document = CreateReadableDocument(120);

        sender.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)sender);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsFalse(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out string? error));
        StringAssert.Contains(error, "modem-like noises");
        CollectionAssert.Contains(receiver.ProgressMessages,
            "The line erupts with a burst of unintelligible modem-like noises before disconnecting.");
    }

    [TestMethod]
    public void TelecommunicationsGrid_FaxQueuesUntilPaperOrInkBecomeAvailable()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        FaxMachineDouble sender = new(gameworld.Object, 1);
        FaxMachineDouble receiver = new(gameworld.Object, 2)
        {
            AvailablePages = 0
        };
        ICanBeRead document = CreateReadableDocument(120);

        sender.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)sender);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out string? error), error);
        Assert.AreEqual(1, receiver.PendingFaxCount);
        Assert.AreEqual(0, receiver.CompletedFaxCount);

        receiver.AvailablePages = 1;
        receiver.ProcessPendingFaxes();

        Assert.AreEqual(0, receiver.PendingFaxCount);
        Assert.AreEqual(1, receiver.CompletedFaxCount);
    }

    [TestMethod]
    public void TelecommunicationsGrid_UnansweredCallRoutesToHostedVoicemailAndStoresMessage()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4, true, "9999");
        grid.SetMaximumRings(2);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2)
        {
            HostedVoicemailEnabled = true
        };

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

        Assert.IsTrue(caller.IsConnected);
        Assert.IsFalse(receiver.IsEngaged);
        CollectionAssert.Contains(caller.ProgressMessages, "The exchange voicemail service answers.");
        CollectionAssert.Contains(caller.ProgressMessages, "A sharp beep sounds over the line.");

        caller.Transmit(CreateSpokenLanguage(caller.Parent));
        Assert.IsTrue(caller.HangUp(null!, out error), error);

        FieldInfo? mailboxesField = typeof(TelecommunicationsGrid).GetField("_hostedVoicemailRecordings",
            BindingFlags.Instance | BindingFlags.NonPublic);
        Dictionary<string, List<StoredAudioRecording>> mailboxes =
            (Dictionary<string, List<StoredAudioRecording>>)mailboxesField!.GetValue(grid)!;
        Assert.IsTrue(mailboxes.ContainsKey(receiver.PhoneNumber!));
        Assert.AreEqual(1, mailboxes[receiver.PhoneNumber!].Count);
        Assert.AreEqual("Hello there", mailboxes[receiver.PhoneNumber!][0].Recording.Segments[0].RawText);
    }

    [TestMethod]
    public void TelecommunicationsGrid_LocalAnsweringMachineTakesPriorityOverHostedVoicemail()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4, true, "9999");
        grid.SetMaximumRings(2);
        Mock<IGameItem> machineParent = CreateBasicItem(gameworld.Object, 230L, CreateRoom().Object);
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(gameworld.Object, 231L);
        AnsweringMachineGameItemComponent machine = new(CreateAnsweringMachineProto(gameworld.Object, 2),
            machineParent.Object, true);
        TelephoneDouble caller = new(gameworld.Object, 232);

        machine.Put(null!, tape.Item.Object);
        ((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
        ((ITelephoneNumberOwner)machine).HostedVoicemailEnabled = true;
        machine.OnPowerCutIn();
        caller.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);

        Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out string? error), error);

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

        Assert.IsTrue(machine.IsConnected);
        Assert.IsTrue(caller.IsConnected);
        Assert.IsFalse(caller.ProgressMessages.Any(x => x.Contains("exchange voicemail service", StringComparison.InvariantCultureIgnoreCase)));
    }

    [TestMethod]
    public void TelecommunicationsGrid_HostedVoicemailAccessPlaysAndDeletesMessages()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4, true, "9999");
        grid.SetMaximumRings(2);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2)
        {
            HostedVoicemailEnabled = true
        };

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
        caller.Transmit(CreateSpokenLanguage(caller.Parent));
        Assert.IsTrue(caller.HangUp(null!, out error), error);

        Assert.IsTrue(receiver.Dial(null!, grid.HostedVoicemailAccessNumber, out error), error);
        CollectionAssert.Contains(receiver.ProgressMessages, "The exchange voicemail service answers.");
        Assert.IsTrue(receiver.ProgressMessages.Any(x => x.Contains("1 saved voicemail message")));

        Assert.IsTrue(receiver.Dial(null!, "1", out error), error);
        Assert.IsTrue(receiver.ProgressMessages.Any(x => x.Contains("Playing message #1")));
        Assert.IsTrue(receiver.ProgressMessages.Any(x => x.Contains("Hello there")));

        Assert.IsTrue(receiver.Dial(null!, "3", out error), error);
        Assert.IsTrue(receiver.ProgressMessages.Any(x => x.Contains("current voicemail message is deleted")));
        Assert.IsTrue(receiver.Dial(null!, "#", out error), error);

        Assert.IsTrue(receiver.Dial(null!, grid.HostedVoicemailAccessNumber, out error), error);
        Assert.IsTrue(receiver.ProgressMessages.Any(x => x.Contains("no saved voicemail messages")));
    }

    [TestMethod]
    public void RecordedAudio_RoundTripsSpeakerMetadataAndTiming()
    {
        StoredAudioRecording stored = new(
            "message-1",
            new RecordedAudio(
            [
                new RecordedAudioSegment(
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromMilliseconds(750),
                    10,
                    11,
                    "Hello there",
                    AudioVolume.Loud,
                    Outcome.MajorPass,
                    new RecordedAudioSpeakerSnapshot(42, "Alice", Gender.Female)),
                new RecordedAudioSegment(
                    TimeSpan.FromMilliseconds(500),
                    TimeSpan.FromMilliseconds(250),
                    10,
                    11,
                    "Goodbye",
                    AudioVolume.Decent,
                    Outcome.Pass,
                    new RecordedAudioSpeakerSnapshot(null, "System Voice", Gender.Neuter))
            ]),
            new DateTime(2026, 3, 31, 9, 15, 0, DateTimeKind.Utc));

        XElement xml = stored.SaveToXml();
        StoredAudioRecording loaded = StoredAudioRecording.LoadFromXml(xml);

        Assert.AreEqual("message-1", loaded.Name);
        Assert.AreEqual(stored.RecordedAtUtc, loaded.RecordedAtUtc);
        Assert.AreEqual(2, loaded.Recording.Segments.Count);
        Assert.AreEqual(TimeSpan.FromSeconds(2), loaded.Recording.Segments[0].DelayBeforeSegment);
        Assert.AreEqual(TimeSpan.FromMilliseconds(750), loaded.Recording.Segments[0].EstimatedSegmentDuration);
        Assert.AreEqual(10L, loaded.Recording.Segments[0].LanguageId);
        Assert.AreEqual(11L, loaded.Recording.Segments[0].AccentId);
        Assert.AreEqual("Hello there", loaded.Recording.Segments[0].RawText);
        Assert.AreEqual(AudioVolume.Loud, loaded.Recording.Segments[0].Volume);
        Assert.AreEqual(Outcome.MajorPass, loaded.Recording.Segments[0].Outcome);
        Assert.AreEqual(42L, loaded.Recording.Segments[0].Speaker.CharacterId);
        Assert.AreEqual("Alice", loaded.Recording.Segments[0].Speaker.Name);
        Assert.AreEqual(Gender.Female, loaded.Recording.Segments[0].Speaker.Gender);
        Assert.AreEqual(TimeSpan.FromMilliseconds(3500), loaded.Recording.TotalDuration);
    }

    [TestMethod]
    public void TelecommunicationsGrid_DialDuringConnectedCall_RelaysKeypadDigitsToRecipient()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);

        caller.TelecommunicationsGrid = grid;
        receiver.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);
        grid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        Assert.IsTrue(receiver.Answer(null!, out error), error);
        Assert.IsTrue(caller.Dial(null!, "1 2# *", out error), error);

        CollectionAssert.AreEqual(new[] { "12#*" }, receiver.ReceivedDigits.ToArray());
    }

    [TestMethod]
    public void TelephoneGameItemComponent_ReceiveDigits_FiresTelephoneDigitsReceivedEvent()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = CreateBasicItem(gameworld.Object, 88L, CreateRoom().Object);
        TelephoneGameItemComponentProto proto = CreateTelephoneProto(gameworld.Object, 0.0);
        TelephoneGameItemComponent component = new(proto, parent.Object, true);
        TelephoneDouble source = new(gameworld.Object, 89);

        component.ReceiveDigits(source, "12#");

        parent.Verify(
            x => x.HandleEvent(
                EventType.TelephoneDigitsReceived,
                It.Is<object[]>(args =>
                    args.Length == 2 &&
                    ReferenceEquals(args[0], source.Parent) &&
                    Equals(args[1], "12#"))),
            Times.Once);
    }

    [TestMethod]
    public void TelecommunicationsGrid_AnsweringMachinePlaysGreetingAndRecordsMessage()
    {
        Mock<ILanguage> language = CreateLanguage(101);
        Mock<IAccent> accent = CreateAccent(201, language.Object);
        Mock<IFuturemud> gameworld = CreateGameworld(languageList: [language.Object], accentList: [accent.Object]);
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        Mock<IGameItem> machineParent = CreateBasicItem(gameworld.Object, 200L, CreateRoom().Object);
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(gameworld.Object, 201L);
        AnsweringMachineGameItemComponent machine = new(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
        TelephoneDouble caller = new(gameworld.Object, 202);

        Assert.IsTrue(StoreRecordedAudio(gameworld.Object, tape.Component,
            new StoredAudioRecording(
                "__greeting__",
                new RecordedAudio(
                [
                    new RecordedAudioSegment(
                        TimeSpan.Zero,
                        TimeSpan.Zero,
                        language.Object.Id,
                        accent.Object.Id,
                        "Please leave a message after the beep.",
                        AudioVolume.Decent,
                        Outcome.Pass,
                        new RecordedAudioSpeakerSnapshot(77, "Recorded Voice", Gender.Neuter))
                ]),
                DateTime.UtcNow),
            out string? error), error);

        machine.Put(null!, tape.Item.Object);
        ((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
        machine.OnPowerCutIn();
        caller.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);

        Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out error), error);

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

        Assert.IsTrue(machine.IsConnected);
        Assert.IsTrue(caller.IsConnected);
        Assert.IsFalse(caller.ProgressMessages.Contains("The line rings out."));

        heartbeat.Raise(x => x.SecondHeartbeat += null);

        CollectionAssert.AreEqual(
            new[] { "Please leave a message after the beep." },
            caller.ReceivedTransmissionTexts.ToArray());
        Assert.IsTrue(machine.IsRecordingMessage);

        caller.Transmit(CreateSpokenLanguage(caller.Parent));
        Assert.IsTrue(caller.HangUp(null!, out error), error);

        Assert.AreEqual(1, machine.MessageRecordings.Count);
        Assert.AreEqual("Hello there", machine.MessageRecordings[0].Recording.Segments[0].RawText);
    }

    [TestMethod]
    public void TelecommunicationsGrid_AnsweringMachineDisconnectsWhenExtensionAnswers()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        Mock<IGameItem> machineParent = CreateBasicItem(gameworld.Object, 210L, CreateRoom().Object);
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(gameworld.Object, 211L);
        AnsweringMachineGameItemComponent machine = new(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
        TelephoneDouble caller = new(gameworld.Object, 212);
        TelephoneDouble extension = new(gameworld.Object, 213) { ExternalOwner = machine };

        machine.Put(null!, tape.Item.Object);
        ((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
        machine.OnPowerCutIn();
        caller.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);

        Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out string? error), error);

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

        Assert.IsTrue(machine.IsRecordingMessage);
        caller.Transmit(CreateSpokenLanguage(caller.Parent));

        Assert.IsTrue(extension.PickUp(null!, out error), error);
        Assert.IsFalse(machine.IsConnected);
        Assert.IsFalse(machine.IsRecordingMessage);
        Assert.IsTrue(extension.IsConnected);
        Assert.AreEqual(1, machine.MessageRecordings.Count);
    }

    [TestMethod]
    public void TelecommunicationsGrid_AnsweringMachineWithWriteProtectedMediumStillAnswers()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        Mock<IGameItem> machineParent = CreateBasicItem(gameworld.Object, 220L, CreateRoom().Object);
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(gameworld.Object, 221L);
        AnsweringMachineGameItemComponent machine = new(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
        TelephoneDouble caller = new(gameworld.Object, 222);

        tape.Component.WriteProtected = true;
        machine.Put(null!, tape.Item.Object);
        ((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
        machine.OnPowerCutIn();
        caller.TelecommunicationsGrid = grid;
        grid.JoinGrid((ITelephoneNumberOwner)caller);

        Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out string? error), error);

        Mock<IHeartbeatManager> heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
        heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

        Assert.IsTrue(machine.IsConnected);
        Assert.IsFalse(machine.IsRecordingMessage);

        caller.Transmit(CreateSpokenLanguage(caller.Parent));
        Assert.IsTrue(caller.HangUp(null!, out error), error);

        Assert.AreEqual(0, machine.MessageRecordings.Count);
    }

    [TestMethod]
    public void AnsweringMachine_SelectGreetingRecordStop_SavesGreetingToMedium()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        IRoom room = CreateRoom().Object;
        Mock<IGameItem> machineParent = CreateBasicItem(gameworld.Object, 230L, room);
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(gameworld.Object, 231L);
        AnsweringMachineGameItemComponent machine = new(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
        Mock<ICharacter> actor = CreateCharacter(gameworld.Object, 232L, room);
        Mock<ILanguage> language = CreateLanguage(301);
        Mock<IAccent> accent = CreateAccent(302, language.Object);

        machine.Put(actor.Object, tape.Item.Object);

        Assert.IsTrue(machine.Select(actor.Object, "greeting record", Mock.Of<IEmote>()));
        Assert.IsTrue(machine.IsRecordingGreeting);

        machine.HandleEvent(EventType.CharacterSpeaksWitness, actor.Object, actor.Object, AudioVolume.Decent,
            language.Object, accent.Object, "Please leave a message.");

        Assert.IsTrue(machine.Select(actor.Object, "greeting stop", Mock.Of<IEmote>()));
        Assert.IsFalse(machine.IsRecordingGreeting);
        Assert.IsNotNull(machine.GreetingRecording);
        Assert.AreEqual("Please leave a message.", machine.GreetingRecording!.Recording.Segments[0].RawText);
    }

    [TestMethod]
    public void AnsweringMachine_LoadRestoresInsertedMediumAndSettings()
    {
        Mock<IFuturemud> tapeGameworld = CreateGameworld();
        (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) tape = CreateAudioMedium(tapeGameworld.Object, 241L);
        Mock<IFuturemud> gameworld = CreateGameworld(itemList: [tape.Item.Object]);
        gameworld.Setup(x => x.TryGetItem(tape.Item.Object.Id, true)).Returns(tape.Item.Object);

        Mock<IGameItem> parent = CreateBasicItem(gameworld.Object, 240L, CreateRoom().Object);
        AnsweringMachineGameItemComponentProto proto = CreateAnsweringMachineProto(gameworld.Object, 4);
        AnsweringMachineGameItemComponent component = new(new MudSharp.Models.GameItemComponent
        {
            Id = 1,
            Definition =
                $"<Definition><Grid>0</Grid><SwitchedOn>false</SwitchedOn><PreferredNumber></PreferredNumber><AllowSharedNumber>false</AllowSharedNumber><AutoAnswerRings>5</AutoAnswerRings><RingVolumeOverride>-1</RingVolumeOverride><Medium>{tape.Item.Object.Id}</Medium><ConnectedItems /></Definition>"
        }, proto, parent.Object);

        component.FinaliseLoad();

        Assert.IsFalse(component.SwitchedOn);
        Assert.AreEqual(5, component.AutoAnswerRings);
        Assert.IsNotNull(component.Medium);
        Assert.AreEqual(tape.Item.Object, component.Contents.Single());
    }

	[TestMethod]
	public void MediaStorageMedium_EnforcesCapacityAndWriteProtection()
	{
		Mock<IFuturemud> gameworld = CreateGameworld();
		var medium = CreateAudioMedium(gameworld.Object, 242L, capacityMinutes: 0.01).Component;
		var service = gameworld.Object.MediaRecordingService;
		var startedAt = DateTime.UtcNow;
		var first = service.CreateRecording(new MediaRecordingCreateRequest(MediaCapabilities.Audio, "first", startedAt,
			medium.Id));
		Assert.IsTrue(service.FinaliseRecording(first.RecordingId, MediaRecordingStatus.Finalised,
			startedAt.AddMilliseconds(500), out var error), error);
		Assert.IsTrue(medium.StoreRecording(service.GetRecording(first.RecordingId)!, out error), error);

		var second = service.CreateRecording(new MediaRecordingCreateRequest(MediaCapabilities.Audio, "second", startedAt,
			medium.Id));
		Assert.IsTrue(service.FinaliseRecording(second.RecordingId, MediaRecordingStatus.Finalised,
			startedAt.AddMilliseconds(200), out error), error);
		Assert.IsFalse(medium.StoreRecording(service.GetRecording(second.RecordingId)!, out error));
		StringAssert.Contains(error, "capacity");

		medium.WriteProtected = true;
		Assert.IsFalse(medium.DeleteRecording("first", out error));
		StringAssert.Contains(error, "write-protected");
	}

    [TestMethod]
    public void TelecommunicationsGrid_RoutesLongDistanceCallsAcrossLinkedExchanges()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid localGrid = new(gameworld.Object, CreateRoom(1).Object, "555", 4);
        TelecommunicationsGrid remoteGrid = new(gameworld.Object, CreateRoom(2).Object, "777", 4);
        localGrid.LinkGrid(remoteGrid);

        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble receiver = new(gameworld.Object, 2);
        caller.TelecommunicationsGrid = localGrid;
        receiver.TelecommunicationsGrid = remoteGrid;
        localGrid.JoinGrid((ITelephoneNumberOwner)caller);
        remoteGrid.JoinGrid((ITelephoneNumberOwner)receiver);

        Assert.IsTrue(localGrid.TryStartCall(caller, receiver.PhoneNumber!, out string? error), error);
        Assert.IsTrue(receiver.IsRinging);
        Assert.IsFalse(caller.IsConnected);

        Assert.IsTrue(receiver.Answer(null!, out error), error);
        Assert.IsTrue(caller.IsConnected);
        Assert.IsTrue(receiver.IsConnected);
    }

    [TestMethod]
    public void TelecommunicationsGrid_DoesNotForwardWhenDialledPrefixMatchesLocalExchange()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid localGrid = new(gameworld.Object, CreateRoom(1).Object, "555", 4);
        TelecommunicationsGrid conflictingRemoteGrid = new(gameworld.Object, CreateRoom(2).Object, "555", 4);
        localGrid.LinkGrid(conflictingRemoteGrid);

        TelephoneDouble caller = new(gameworld.Object, 1);
        TelephoneDouble localReceiver = new(gameworld.Object, 2);
        TelephoneDouble remoteReceiver = new(gameworld.Object, 3);

        caller.TelecommunicationsGrid = localGrid;
        localReceiver.TelecommunicationsGrid = localGrid;
        remoteReceiver.TelecommunicationsGrid = conflictingRemoteGrid;
        localGrid.JoinGrid((ITelephoneNumberOwner)caller);
        localGrid.JoinGrid((ITelephoneNumberOwner)localReceiver);
        conflictingRemoteGrid.JoinGrid((ITelephoneNumberOwner)remoteReceiver);

        Assert.IsTrue(localGrid.TryStartCall(caller, localReceiver.PhoneNumber!, out string? error), error);
        Assert.IsTrue(localReceiver.IsRinging);
        Assert.IsFalse(remoteReceiver.IsRinging);
    }

    [TestMethod]
    public void TelecommunicationsGrid_RejectsAmbiguousLinkedExchangePrefixes()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid localGrid = new(gameworld.Object, CreateRoom(1).Object, "555", 4);
        TelecommunicationsGrid remoteOne = new(gameworld.Object, CreateRoom(2).Object, "777", 4);
        TelecommunicationsGrid remoteTwo = new(gameworld.Object, CreateRoom(3).Object, "777", 4);
        localGrid.LinkGrid(remoteOne);
        localGrid.LinkGrid(remoteTwo);

        TelephoneDouble caller = new(gameworld.Object, 1);
        caller.TelecommunicationsGrid = localGrid;
        localGrid.JoinGrid((ITelephoneNumberOwner)caller);

        Assert.IsFalse(localGrid.TryStartCall(caller, "7771234", out string? error));
        StringAssert.Contains(error, "more than one linked");
    }

    [TestMethod]
    public void TelephoneNumber_FollowsAssignedEndpointRatherThanHandset()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        TelecommunicationsGrid grid = new(gameworld.Object, CreateRoom().Object, "555", 4);
        TelephoneDouble phone = new(gameworld.Object, 10);
        Mock<ITelephoneNumberOwner> lineA = CreateLineOwner(gameworld.Object, 11, 101, phone);
        Mock<ITelephoneNumberOwner> lineB = CreateLineOwner(gameworld.Object, 12, 102, phone);

        lineA.Object.TelecommunicationsGrid = grid;
        lineB.Object.TelecommunicationsGrid = grid;
        grid.JoinGrid(lineA.Object);
        grid.JoinGrid(lineB.Object);

        phone.ExternalOwner = lineA.Object;
        string? lineANumber = phone.PhoneNumber;
        Assert.IsFalse(string.IsNullOrWhiteSpace(lineANumber));

        phone.ExternalOwner = lineB.Object;
        string? lineBNumber = phone.PhoneNumber;
        Assert.IsFalse(string.IsNullOrWhiteSpace(lineBNumber));
        Assert.AreNotEqual(lineANumber, lineBNumber);
    }

    [TestMethod]
    public void TelecommunicationsGrid_LoadTimeInitialiseRestoresAssignmentsByComponentId()
    {
        Mock<IFuturemud> runtimeGameworld = CreateGameworld();
        TelecommunicationsGrid runtimeGrid = new(runtimeGameworld.Object, CreateRoom().Object, "555", 4);
        Mock<ITelephoneNumberOwner> lineA = CreateLineOwner(runtimeGameworld.Object, 11, 101);
        Mock<ITelephoneNumberOwner> lineB = CreateLineOwner(runtimeGameworld.Object, 11, 102);
        runtimeGrid.JoinGrid(lineA.Object);
        runtimeGrid.JoinGrid(lineB.Object);
        string? lineANumber = runtimeGrid.GetPhoneNumber(lineA.Object);
        string? lineBNumber = runtimeGrid.GetPhoneNumber(lineB.Object);

        XElement saveDefinition = (XElement)typeof(TelecommunicationsGrid)
            .GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
            .Invoke(runtimeGrid, [])!;
        saveDefinition.Elements("Location").Remove();

        Mock<IGameItem> item = new();
        item.SetupGet(x => x.Id).Returns(11L);
        item.SetupGet(x => x.Name).Returns("Telephone Hub");
        item.SetupGet(x => x.Gameworld).Returns(runtimeGameworld.Object);
        item.SetupGet(x => x.Components).Returns([lineA.Object, lineB.Object]);
        item.Setup(x => x.GetItemType<ITelephoneNumberOwner>()).Returns(lineA.Object);

        Mock<IFuturemud> loadGameworld = CreateGameworld([item.Object]);
        TelecommunicationsGrid loadedGrid = new(new MudSharp.Models.Grid
        {
            Id = 1,
            GridType = "Telecommunications",
            Definition = saveDefinition.ToString()
        }, loadGameworld.Object);
        loadedGrid.LoadTimeInitialise();

        Assert.AreEqual(lineANumber, loadedGrid.GetPhoneNumber(lineA.Object));
        Assert.AreEqual(lineBNumber, loadedGrid.GetPhoneNumber(lineB.Object));
    }

    [TestMethod]
    public void GridLiquidSourceGameItemComponent_RejectsIncomingLiquidMerges()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        GridLiquidSourceGameItemComponentProto proto = CreateGridLiquidSourceProto(gameworld.Object);
        GridLiquidSourceGameItemComponent component = new(proto, parent.Object, true);
        LiquidMixture mixture = CreateMixture(gameworld.Object, (CreateLiquid(1, "water").Object, 1.0));

        Assert.AreEqual(0.0, component.LiquidCapacity, 0.0001);
        component.MergeLiquid(mixture, null!, "test");
        Assert.AreEqual(0.0, component.LiquidCapacity, 0.0001);
    }

    [TestMethod]
    public void GridLiquidSourceGameItemComponent_ReduceLiquidQuantity_RemovesFromGrid()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        GridLiquidSourceGameItemComponentProto proto = CreateGridLiquidSourceProto(gameworld.Object);
        GridLiquidSourceGameItemComponent component = new(proto, parent.Object, true);
        var grid = new Mock<ILiquidGrid>();
        component.LiquidGrid = grid.Object;

        component.ReduceLiquidQuantity(2.5, null!, "drink");

        grid.Verify(x => x.RemoveLiquidAmount(2.5, null, "drink"), Times.Once);
    }

    [TestMethod]
    public void TelephoneGameItemComponent_Transmit_ForwardsSpeechToConnectedPhone()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.Id).Returns(1L);
        TelephoneGameItemComponentProto proto = CreateTelephoneProto(gameworld.Object, 5.0);
        TelephoneGameItemComponent component = new(proto, parent.Object, true);
        Mock<ITelephoneCall> call = new();
        call.SetupGet(x => x.IsConnected).Returns(true);
        call.SetupGet(x => x.Participants).Returns([component]);

        component.ConnectCall(call.Object);
        component.OnPowerCutIn();
        component.Transmit(CreateSpokenLanguage(parent.Object));

        call.Verify(x => x.RelayTransmission(component, It.IsAny<SpokenLanguageInfo>()), Times.Once);
    }

    [TestMethod]
    public void TelephoneGameItemComponent_Switch_RingModesMapToNamedPlayerSettings()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.Id).Returns(2L);
        TelephoneGameItemComponentProto proto = CreateTelephoneProto(gameworld.Object, 5.0);
        TelephoneGameItemComponent component = new(proto, parent.Object, true);

        CollectionAssert.AreEquivalent(
            new[] { "on", "off", "quiet", "normal", "loud", "vmon", "vmoff" },
            component.SwitchSettings.ToArray());

        Assert.IsTrue(component.Switch(null!, "quiet"));
        Assert.AreEqual(AudioVolume.Quiet, component.RingVolume);
        Assert.IsTrue(component.Switch(null!, "normal"));
        Assert.AreEqual(AudioVolume.Decent, component.RingVolume);
        Assert.IsTrue(component.Switch(null!, "loud"));
        Assert.AreEqual(AudioVolume.Loud, component.RingVolume);
        Assert.IsTrue(component.Switch(null!, "vmon"));
        Assert.IsTrue(component.HostedVoicemailEnabled);
        Assert.IsTrue(component.Switch(null!, "vmoff"));
        Assert.IsFalse(component.HostedVoicemailEnabled);
    }

    [TestMethod]
    public void CellularPhoneGameItemComponent_Switch_SilentModeVibratesWearerWhenInWornContainer()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IOutputHandler> wearerOutput = new();
        Mock<ICharacter> wearer = new();
        wearer.SetupGet(x => x.OutputHandler).Returns(wearerOutput.Object);
        Mock<IBody> body = new();
        body.SetupGet(x => x.Actor).Returns(wearer.Object);
        body.SetupGet(x => x.OutputHandler).Returns(wearerOutput.Object);
        Mock<IGameItem> wornContainer = new();
        wornContainer.SetupGet(x => x.InInventoryOf).Returns(body.Object);
        body.SetupGet(x => x.WornItems).Returns([wornContainer.Object]);

        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.Id).Returns(3L);
        parent.SetupGet(x => x.ContainedIn).Returns(wornContainer.Object);
        parent.SetupGet(x => x.TrueLocations).Returns(Array.Empty<IRoom>());
        CellularPhoneGameItemComponentProto proto = CreateCellularPhoneProto(gameworld.Object, 2.0);
        CellularPhoneGameItemComponent component = new(proto, parent.Object, true);

        component.OnPowerCutIn();
        Assert.IsTrue(component.SwitchSettings.Contains("silent"));
        Assert.IsTrue(component.Switch(null!, "silent"));
        Assert.AreEqual(AudioVolume.Silent, component.RingVolume);

        component.ReceiveIncomingCall(Mock.Of<ITelephoneCall>());

        wearerOutput.Verify(
            x => x.Send(
                "You feel a muted vibration from one of your worn items.",
                It.IsAny<bool>(),
                It.IsAny<bool>()),
            Times.Once);
        parent.Verify(
            x => x.Handle(It.IsAny<IOutput>(), It.IsAny<OutputRange>()),
            Times.Never);
    }

    [TestMethod]
    public void ImplantTelephoneGameItemComponent_CanDial_RequiresCellCoverage()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.Id).Returns(20L);
        parent.SetupGet(x => x.TrueLocations).Returns([]);
        ImplantTelephoneGameItemComponentProto proto = CreateImplantTelephoneProto(gameworld.Object);
        ImplantTelephoneGameItemComponent component = new(proto, parent.Object, true);
        Mock<ITelecommunicationsGrid> grid = new();

        component.TelecommunicationsGrid = grid.Object;
        component.AssignPhoneNumber("5551200");
        component.OnPowerCutIn();
		var (actor, _) = InstallAudioNeuralLink(component);

		Assert.IsFalse(component.CanDial(actor.Object, "5551201", out string? error));
        StringAssert.Contains(error, "no signal");
    }

    [TestMethod]
    public void ImplantTelephoneGameItemComponent_Transmit_ForwardsSpeechToConnectedPhone()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
        parent.SetupGet(x => x.Id).Returns(21L);
        ImplantTelephoneGameItemComponentProto proto = CreateImplantTelephoneProto(gameworld.Object);
        ImplantTelephoneGameItemComponent component = new(proto, parent.Object, true);
		var (actor, _) = InstallAudioNeuralLink(component);
        Mock<ITelephoneCall> call = new();
        call.SetupGet(x => x.IsConnected).Returns(true);
        call.SetupGet(x => x.Participants).Returns([component]);

        component.ConnectCall(call.Object);
        component.OnPowerCutIn();
		component.Transmit(CreateSpokenLanguage(actor.Object));

        call.Verify(x => x.RelayTransmission(component, It.IsAny<SpokenLanguageInfo>()), Times.Once);
    }

	[DataTestMethod]
	[DataRow("unlinked")]
	[DataRow("unpowered")]
	[DataRow("noaudio")]
	[DataRow("otheractor")]
	public void ImplantPhone_RejectsControlWithoutWearerAudioLink(string restriction)
	{
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var room = CreateRoom();
		room.SetupGet(x => x.Zone).Returns(new Mock<IZone>().Object);
		parent.SetupGet(x => x.TrueLocations).Returns([room.Object]);
		var component = new ImplantTelephoneGameItemComponent(CreateImplantTelephoneProto(gameworld.Object), parent.Object, true);
		var (owner, link) = InstallAudioNeuralLink(component);
		var grid = new Mock<ITelecommunicationsGrid>();
		grid.Setup(x => x.HasCellularCoverage(It.IsAny<IZone>())).Returns(true);
		component.TelecommunicationsGrid = grid.Object;
		component.AssignPhoneNumber("5551200");
		component.OnPowerCutIn();
		Assert.IsTrue(component.CanDial(owner.Object, "5551201", out _));
		ICharacter actor = owner.Object;
		switch (restriction)
		{
			case "unlinked": link.Setup(x => x.IsLinkedTo(component)).Returns(false); break;
			case "unpowered": link.SetupGet(x => x.DNIConnected).Returns(false); break;
			case "noaudio": link.SetupGet(x => x.PermitsAudio).Returns(false); break;
			case "otheractor": actor = new Mock<ICharacter>().Object; break;
		}
		Assert.IsFalse(component.ManualTransmit);
		Assert.IsFalse(component.CanDial(actor, "5551201", out _));
		Assert.IsFalse(component.Dial(actor, "5551201", out _));
		Assert.IsFalse(component.CanPickUp(actor, out _));
		Assert.IsFalse(component.PickUp(actor, out _));
		Assert.IsFalse(component.CanAnswer(actor, out _));
		Assert.IsFalse(component.CanHangUp(actor, out _));
		Assert.IsFalse(component.CanSendDigits(actor, "1", out _));
		Assert.IsFalse(component.Switch(actor, "off"));
		var call = new Mock<ITelephoneCall>();
		call.SetupGet(x => x.IsConnected).Returns(true);
		call.SetupGet(x => x.Participants).Returns([component]);
		component.ConnectCall(call.Object);
		component.Transmit(CreateSpokenLanguage(actor));
		call.Verify(x => x.RelayTransmission(component, It.IsAny<SpokenLanguageInfo>()), Times.Never);
	}

	[TestMethod]
	public void ImplantPhone_ReceivesSpeechOnlyFromCurrentCallParticipants()
	{
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var component = new ImplantTelephoneGameItemComponent(CreateImplantTelephoneProto(gameworld.Object), parent.Object, true);
		var (actor, link) = InstallAudioNeuralLink(component);
		var output = Mock.Get(actor.Object.OutputHandler);
		var source = new Mock<ITelephone>();
		var outsider = new Mock<ITelephone>();
		var call = new Mock<ITelephoneCall>();
		call.SetupGet(x => x.IsConnected).Returns(true);
		call.SetupGet(x => x.Participants).Returns([component, source.Object]);
		source.SetupGet(x => x.CurrentCall).Returns(call.Object);
		outsider.SetupGet(x => x.CurrentCall).Returns(call.Object);
		component.ConnectCall(call.Object);
		component.OnPowerCutIn();
		var speech = CreateSpokenLanguage(actor.Object);
		component.ReceiveTransmission(0, speech, 0, outsider.Object);
		source.SetupGet(x => x.CurrentCall).Returns(new Mock<ITelephoneCall>().Object);
		component.ReceiveTransmission(0, speech, 0, source.Object);
		output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Never);
		source.SetupGet(x => x.CurrentCall).Returns(call.Object);
		component.ReceiveTransmission(0, speech, 0, source.Object);
		output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Once);
		link.SetupGet(x => x.DNIConnected).Returns(false);
		component.ReceiveTransmission(0, speech, 0, source.Object);
		call.SetupGet(x => x.IsConnected).Returns(false);
		component.ReceiveTransmission(0, speech, 0, source.Object);
		output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Once);
	}

    [TestMethod]
    public void TelecommunicationsGridCreatorComponent_RecreatesMissingGridOnLoad()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = CreateBasicItem(gameworld.Object, 50L, CreateRoom().Object);
        TelecommunicationsGridCreatorGameItemComponentProto proto = CreateTelecommunicationsGridCreatorProto(gameworld.Object, "555", 4);

        TelecommunicationsGridCreatorGameItemComponent component = new(new MudSharp.Models.GameItemComponent
        {
            Id = 1,
            Definition = "<Definition><Grid>0</Grid></Definition>"
        }, proto, parent.Object);

        Assert.IsNotNull(component.Grid);
        Mock.Get(gameworld.Object.SaveManager)
            .Verify(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()), Times.Once);
    }

    [TestMethod]
    public void ElectricGridCreatorComponent_RecreatesMissingGridOnLoad()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = CreateBasicItem(gameworld.Object, 51L, CreateRoom().Object);
        ElectricGridCreatorGameItemComponentProto proto = CreateElectricGridCreatorProto(gameworld.Object);

        ElectricGridCreatorGameItemComponent component = new(new MudSharp.Models.GameItemComponent
        {
            Id = 1,
            Definition = "<Definition><Grid>0</Grid></Definition>"
        }, proto, parent.Object);

        Assert.IsNotNull(component.Grid);
        Mock.Get(gameworld.Object.SaveManager)
            .Verify(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()), Times.Once);
    }

    [TestMethod]
    public void LiquidGridCreatorComponent_RecreatesMissingGridOnLoad()
    {
        Mock<IFuturemud> gameworld = CreateGameworld();
        Mock<IGameItem> parent = CreateBasicItem(gameworld.Object, 52L, CreateRoom().Object);
        LiquidGridCreatorGameItemComponentProto proto = CreateLiquidGridCreatorProto(gameworld.Object);

        LiquidGridCreatorGameItemComponent component = new(new MudSharp.Models.GameItemComponent
        {
            Id = 1,
            Definition = "<Definition><Grid>0</Grid></Definition>"
        }, proto, parent.Object);

        Assert.IsNotNull(component.Grid);
        Mock.Get(gameworld.Object.SaveManager)
            .Verify(x => x.DirectInitialise(It.IsAny<ILateInitialisingItem>()), Times.Once);
    }

    private static Mock<IFuturemud> CreateGameworld(IEnumerable<IGameItem>? itemList = null,
        IEnumerable<IGrid>? gridList = null, IEnumerable<ILanguage>? languageList = null,
        IEnumerable<IAccent>? accentList = null)
    {
        Mock<IFuturemud> gameworld = new();
        Mock<ISaveManager> saveManager = new();
        Mock<IUneditableAll<IBodyPrototype>> bodyPrototypes = new();
        Mock<IHeartbeatManager> heartbeatManager = new();
        TestMediaRecordingService mediaRecordingService = new();
        Mock<IUnitManager> unitManager = new();
        Mock<IUneditableAll<IGameItem>> items = CreateCollection(itemList ?? Enumerable.Empty<IGameItem>());
        Mock<IUneditableAll<IGrid>> grids = CreateCollection(gridList ?? Enumerable.Empty<IGrid>());
        Mock<IUneditableAll<ILanguage>> languages = CreateCollection(languageList ?? Enumerable.Empty<ILanguage>());
        Mock<IUneditableAll<IAccent>> accents = CreateCollection(accentList ?? Enumerable.Empty<IAccent>());
        unitManager.SetupGet(x => x.BaseFluidToLitres).Returns(1.0);
        unitManager.SetupGet(x => x.BaseWeightToKilograms).Returns(1.0);
        bodyPrototypes.SetupGet(x => x.Count).Returns(0);
        bodyPrototypes.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<IBodyPrototype>().GetEnumerator());
        bodyPrototypes.Setup(x => x.Get(It.IsAny<long>())).Returns((IBodyPrototype?)null);
        gameworld.SetupGet(x => x.BodyPrototypes).Returns(bodyPrototypes.Object);
        gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
        gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.MediaRecordingService).Returns(mediaRecordingService);
        gameworld.SetupGet(x => x.Items).Returns(items.Object);
        gameworld.SetupGet(x => x.Grids).Returns(grids.Object);
        gameworld.SetupGet(x => x.Languages).Returns(languages.Object);
        gameworld.SetupGet(x => x.Accents).Returns(accents.Object);
        gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
        gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
                 .Returns<long, bool>((id, _) => items.Object.Get(id)!);
        return gameworld;
    }

	private sealed class TestMediaRecordingService : IMediaRecordingService
	{
		private sealed class RecordingState
		{
			public required MediaRecordingDescriptor Descriptor { get; set; }
			public List<MediaPacket> Packets { get; } = [];
			public List<(long StartMilliseconds, long EndMilliseconds, string Scene)> Frames { get; } = [];
		}

		private long _nextRecordingId;
		private readonly Dictionary<long, RecordingState> _recordings = [];
		private readonly Dictionary<string, MediaRecordingReference> _references =
			new(StringComparer.InvariantCultureIgnoreCase);

		public MediaRecordingDescriptor CreateRecording(MediaRecordingCreateRequest request)
		{
			var descriptor = new MediaRecordingDescriptor(++_nextRecordingId, request.Capabilities,
				MediaRecordingStatus.Recording, request.StartedAtUtc, null, TimeSpan.Zero, 0L, request.Name);
			_recordings[descriptor.RecordingId] = new RecordingState { Descriptor = descriptor };
			return descriptor;
		}

		public bool AppendPacket(long recordingId, MediaPacket packet, out string error)
		{
			error = string.Empty;
			if (!_recordings.TryGetValue(recordingId, out var state) ||
			    state.Descriptor.Status != MediaRecordingStatus.Recording)
			{
				error = "That recording is no longer accepting packets.";
				return false;
			}

			state.Packets.Add(packet);
			return true;
		}

		public bool AppendSceneFrame(long recordingId, TimeSpan startOffset, TimeSpan endOffset, string canonicalScene,
			out string error)
		{
			error = string.Empty;
			if (!_recordings.TryGetValue(recordingId, out var state) ||
			    state.Descriptor.Status != MediaRecordingStatus.Recording)
			{
				error = "That recording is no longer accepting scene frames.";
				return false;
			}

			state.Frames.Add(((long)startOffset.TotalMilliseconds, (long)endOffset.TotalMilliseconds, canonicalScene));
			return true;
		}

		public bool FinaliseRecording(long recordingId, MediaRecordingStatus status, DateTime completedAtUtc, out string error)
		{
			error = string.Empty;
			if (!_recordings.TryGetValue(recordingId, out var state) ||
			    state.Descriptor.Status != MediaRecordingStatus.Recording)
			{
				error = "That recording has already been finalised.";
				return false;
			}

			var packetDuration = state.Packets
				.Select(x => x.TimestampUtc - state.Descriptor.CreatedAtUtc)
				.DefaultIfEmpty(TimeSpan.Zero)
				.Max();
			var frameDuration = state.Frames.Select(x => x.EndMilliseconds).DefaultIfEmpty(0L).Max();
			var duration = new[]
			{
				packetDuration,
				TimeSpan.FromMilliseconds(frameDuration),
				completedAtUtc - state.Descriptor.CreatedAtUtc
			}.Max();
			state.Descriptor = state.Descriptor with
			{
				Status = status,
				FinalisedAtUtc = completedAtUtc,
				Duration = duration > TimeSpan.Zero ? duration : TimeSpan.Zero,
				LogicalSizeInBytes = state.Packets.Sum(x => x.Payload.ToString()?.Length ?? 0) +
				                     state.Frames.Sum(x => x.Scene.Length)
			};
			return true;
		}

		public MediaRecordingDescriptor? GetRecording(long recordingId)
		{
			return _recordings.GetValueOrDefault(recordingId)?.Descriptor;
		}

		public IEnumerable<MediaPacket> ReadPackets(long recordingId)
		{
			return _recordings.GetValueOrDefault(recordingId)?.Packets
				.OrderBy(x => x.TimestampUtc)
				.ThenBy(x => x.Sequence)
				.ToList() ?? [];
		}

		public MediaScenePayload? GetSceneAt(long recordingId, TimeSpan offset)
		{
			var offsetMilliseconds = (long)offset.TotalMilliseconds;
			var frame = _recordings.GetValueOrDefault(recordingId)?.Frames
				.Where(x => x.StartMilliseconds <= offsetMilliseconds && x.EndMilliseconds >= offsetMilliseconds)
				.Select(x => x.Scene)
				.LastOrDefault();
			return frame is null ? null : new MediaScenePayload(frame, "test");
		}

		public IEnumerable<MediaRecordingDescriptor> GetRecordings(long ownerGameItemComponentId)
		{
			return GetReferences(ownerGameItemComponentId)
				.Select(x => GetRecording(x.RecordingId))
				.Where(x => x is not null)
				.Cast<MediaRecordingDescriptor>()
				.ToList();
		}

		public IEnumerable<MediaRecordingReference> GetReferences(long ownerGameItemComponentId)
		{
			return _references.Values
				.Where(x => x.OwnerGameItemComponentId == ownerGameItemComponentId)
				.OrderBy(x => x.Name)
				.ToList();
		}

		public bool CreateReference(MediaRecordingReference reference, out string error)
		{
			error = string.Empty;
			var key = GetReferenceKey(reference.OwnerGameItemComponentId, reference.Name);
			if (!_recordings.ContainsKey(reference.RecordingId))
			{
				error = "That media recording no longer exists.";
				return false;
			}

			if (!_references.TryAdd(key, reference))
			{
				error = "That medium already has a recording with that name.";
				return false;
			}

			return true;
		}

		public bool DeleteReference(long ownerGameItemComponentId, string name, out string error)
		{
			error = string.Empty;
			var key = GetReferenceKey(ownerGameItemComponentId, name);
			if (!_references.Remove(key, out var reference))
			{
				error = "There is no media recording with that name.";
				return false;
			}

			if (!_references.Values.Any(x => x.RecordingId == reference.RecordingId))
			{
				_recordings.Remove(reference.RecordingId);
			}

			return true;
		}

		public bool SetReferencePubliclyAccessible(long ownerGameItemComponentId, string name, bool publiclyAccessible,
			out string error)
		{
			var key = GetReferenceKey(ownerGameItemComponentId, name);
			if (!_references.TryGetValue(key, out var reference))
			{
				error = "There is no media recording with that name.";
				return false;
			}

			_references[key] = reference with { PubliclyAccessible = publiclyAccessible, LastModifiedAtUtc = DateTime.UtcNow };
			error = string.Empty;
			return true;
		}

		public MediaRecordingReference? GetReference(long ownerGameItemComponentId, string name)
		{
			return _references.GetValueOrDefault(GetReferenceKey(ownerGameItemComponentId, name));
		}

		private static string GetReferenceKey(long ownerGameItemComponentId, string name)
		{
			return $"{ownerGameItemComponentId}:{name}";
		}
	}

    private static Mock<IUneditableAll<T>> CreateCollection<T>(IEnumerable<T> items)
        where T : class, IFrameworkItem
    {
        List<T> source = items.ToList();
        Mock<IUneditableAll<T>> collection = new();
        collection.SetupGet(x => x.Count).Returns(() => source.Count);
        collection.Setup(x => x.GetEnumerator()).Returns(() => source.GetEnumerator());
        collection.Setup(x => x.Has(It.IsAny<T>())).Returns<T>(value => source.Contains(value));
        collection.Setup(x => x.Has(It.IsAny<long>())).Returns<long>(id => source.Any(x => x.Id == id));
        collection.Setup(x => x.Has(It.IsAny<string>())).Returns<string>(name => source.Any(x => x.Name == name));
        collection.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => source.FirstOrDefault(x => x.Id == id));
        collection.Setup(x => x.TryGet(It.IsAny<long>(), out It.Ref<T?>.IsAny))
                  .Returns((long id, out T? result) =>
                  {
                      result = source.FirstOrDefault(x => x.Id == id);
                      return result != null;
                  });
        collection.Setup(x => x.Get(It.IsAny<string>())).Returns<string>(name => source.Where(x => x.Name == name).ToList());
        collection.Setup(x => x.GetByName(It.IsAny<string>())).Returns<string>(name => source.FirstOrDefault(x => x.Name == name));
        collection.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
                  .Returns<string, bool>((value, _) =>
                  {
                      if (long.TryParse(value, out long id))
                      {
                          return source.FirstOrDefault(x => x.Id == id);
                      }

                      return source.FirstOrDefault(x => x.Name == value);
                  });
        collection.Setup(x => x.ForEach(It.IsAny<Action<T>>()))
                  .Callback<Action<T>>(action =>
                  {
                      foreach (T? item in source)
                      {
                          action(item);
                      }
                  });
        return collection;
    }

    private static Mock<IRoom> CreateRoom(long id = 1)
    {
        Mock<IRoom> room = new();
        room.SetupGet(x => x.Id).Returns(id);
        return room;
    }

    private static Mock<IGameItem> CreateBasicItem(IFuturemud gameworld, long id, params IRoom[] trueLocations)
    {
        Mock<IGameItem> item = new();
        Mock<IOutputHandler> outputHandler = new();
        outputHandler.SetupGet(x => x.Perceiver).Returns(() => item.Object);
        item.SetupGet(x => x.Id).Returns(id);
        item.SetupGet(x => x.Name).Returns($"Item {id}");
        item.SetupGet(x => x.Gameworld).Returns(gameworld);
        item.SetupGet(x => x.TrueLocations).Returns(trueLocations);
        item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
        item.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
        item.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
        item.Setup(x => x.HandleEvent(It.IsAny<EventType>(), It.IsAny<object[]>())).Returns(false);
        return item;
    }

    private static Mock<ILiquid> CreateLiquid(long id, string description, ANSIColour? colour = null,
        double density = 1.0)
    {
        Mock<ILiquid> liquid = new();
        liquid.SetupGet(x => x.Id).Returns(id);
        liquid.SetupGet(x => x.Description).Returns(description);
        liquid.SetupGet(x => x.MaterialDescription).Returns(description);
        liquid.SetupGet(x => x.DisplayColour).Returns(colour ?? Telnet.Blue);
        liquid.SetupGet(x => x.Density).Returns(density);
        liquid.SetupGet(x => x.RelativeEnthalpy).Returns(1.0);
        return liquid;
    }

    private static LiquidMixture CreateMixture(IFuturemud gameworld, params (ILiquid Liquid, double Amount)[] contents)
    {
        LiquidMixture mixture = LiquidMixture.CreateEmpty(gameworld);
        foreach ((ILiquid? liquid, double amount) in contents)
        {
            mixture.AddLiquid(new LiquidMixture(liquid, amount, gameworld));
        }

        return mixture;
    }

    private static double GetLiquidAmount(LiquidMixture mixture, ILiquid liquid)
    {
        return mixture.Instances.Where(x => x.Liquid == liquid).Sum(x => x.Amount);
    }

    private static SupplierDouble CreateSupplier(LiquidMixture mixture)
    {
        SupplierState state = new() { Mixture = mixture };
        Mock<ILiquidGridSupplier> supplier = new();
        supplier.SetupGet(x => x.SuppliedMixture).Returns(() => state.Mixture);
        supplier.SetupGet(x => x.AvailableLiquidVolume).Returns(() => state.Mixture.TotalVolume);
        supplier.Setup(x => x.RemoveLiquidAmount(It.IsAny<double>(), It.IsAny<ICharacter?>(), It.IsAny<string>()))
                .Returns((double amount, ICharacter? _, string _) =>
                {
                    LiquidMixture removed = state.Mixture.RemoveLiquidVolume(amount);
                    return removed;
                });
        return new SupplierDouble(supplier, state);
    }

    private static GridPowerSupplyGameItemComponentProto CreateGridPowerSupplyProto(IFuturemud gameworld, double wattage)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 1,
            Name = "Grid Power Supply",
            Description = "Test",
            RevisionNumber = 1,
            Definition = $"<Definition><Wattage>{wattage}</Wattage></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (GridPowerSupplyGameItemComponentProto)typeof(GridPowerSupplyGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static TelephoneGameItemComponentProto CreateTelephoneProto(IFuturemud gameworld, double wattage)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 2,
            Name = "Telephone",
            Description = "Test",
            RevisionNumber = 1,
            Definition =
                $"<Definition><Wattage>{wattage}</Wattage><RingEmote><![CDATA[@ ring|rings loudly.]]></RingEmote><TransmitPremote><![CDATA[@ speak|speaks into $1 and say|says]]></TransmitPremote></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (TelephoneGameItemComponentProto)typeof(TelephoneGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static CellularPhoneGameItemComponentProto CreateCellularPhoneProto(IFuturemud gameworld, double wattage)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 23,
            Name = "Cellular Phone",
            Description = "Test",
            RevisionNumber = 1,
            Definition =
                $"<Definition><Wattage>{wattage}</Wattage><RingEmote><![CDATA[@ chirp|chirps insistently.]]></RingEmote><TransmitPremote><![CDATA[@ speak|speaks into $1 and say|says]]></TransmitPremote></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (CellularPhoneGameItemComponentProto)typeof(CellularPhoneGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static MediaStorageMediumGameItemComponentProto CreateAudioMediumProto(IFuturemud gameworld,
        double capacityMinutes = 30.0)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 24,
            Name = "Audio Medium",
            Description = "Test",
            RevisionNumber = 1,
            Definition = $"<Definition><FormatKey>compact-cassette</FormatKey><Capabilities>Audio</Capabilities><CapacityMilliseconds>{(long)TimeSpan.FromMinutes(capacityMinutes).TotalMilliseconds}</CapacityMilliseconds></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (MediaStorageMediumGameItemComponentProto)typeof(MediaStorageMediumGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static AnsweringMachineGameItemComponentProto CreateAnsweringMachineProto(IFuturemud gameworld,
        int defaultAutoAnswerRings, double wattage = 6.0)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 25,
            Name = "Answering Machine",
            Description = "Test",
            RevisionNumber = 1,
            Definition =
                $"<Definition><Wattage>{wattage}</Wattage><RingEmote><![CDATA[@ ring|rings insistently.]]></RingEmote><TransmitPremote><![CDATA[@ speak|speaks into $1 and say|says]]></TransmitPremote><RingVolume>{(int)AudioVolume.Decent}</RingVolume><DefaultAutoAnswerRings>{defaultAutoAnswerRings}</DefaultAutoAnswerRings></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (AnsweringMachineGameItemComponentProto)typeof(AnsweringMachineGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static (Mock<IGameItem> Item, MediaStorageMediumGameItemComponent Component) CreateAudioMedium(IFuturemud gameworld, long id,
        double capacityMinutes = 30.0)
    {
        Mock<IGameItem> item = CreateBasicItem(gameworld, id);
        item.SetupProperty(x => x.ContainedIn);
        item.Setup(x => x.LoadTimeSetContainedIn(It.IsAny<IGameItem>()))
            .Callback<IGameItem>(parent => item.Object.ContainedIn = parent);
        item.Setup(x => x.FinaliseLoadTimeTasks());
        item.Setup(x => x.Get(It.IsAny<IBody>())).Returns(item.Object);

        MediaStorageMediumGameItemComponent component = new(CreateAudioMediumProto(gameworld, capacityMinutes), item.Object, true);
        item.Setup(x => x.GetItemType<IMediaStorageMedium>()).Returns(component);
        return (item, component);
    }

	private static bool StoreRecordedAudio(IFuturemud gameworld, IMediaStorageMedium medium,
		StoredAudioRecording recording, out string error)
	{
		var mediaRecording = gameworld.MediaRecordingService.CreateRecording(new MediaRecordingCreateRequest(
			MediaCapabilities.Audio, recording.Name, recording.RecordedAtUtc, medium.Id));
		var timestamp = recording.RecordedAtUtc;
		var source = new MediaEndpointAddress(medium.Parent.Id, medium.Id, "test-audio");
		long sequence = 0L;
		foreach (var segment in recording.Recording.Segments)
		{
			timestamp += segment.DelayBeforeSegment;
			var packet = new MediaPacket(Guid.NewGuid(), ++sequence, timestamp, MediaCapabilities.Audio,
				MediaEventKind.Audio, source, [source], new MediaLanguagePayload(false, segment.LanguageId,
					segment.AccentId, segment.RawText, (int)segment.Volume, (int)segment.Outcome,
					segment.Speaker.CharacterId, segment.Speaker.Name, (short)segment.Speaker.Gender, string.Empty,
					string.Empty, (long)segment.EstimatedSegmentDuration.TotalMilliseconds));
			if (!gameworld.MediaRecordingService.AppendPacket(mediaRecording.RecordingId, packet, out error))
			{
				return false;
			}
		}

		if (!gameworld.MediaRecordingService.FinaliseRecording(mediaRecording.RecordingId,
			MediaRecordingStatus.Finalised, recording.RecordedAtUtc + recording.Recording.TotalDuration, out error))
		{
			return false;
		}

		var descriptor = gameworld.MediaRecordingService.GetRecording(mediaRecording.RecordingId);
		if (descriptor is null)
		{
			error = "The test recording did not finalise.";
			return false;
		}

		return medium.StoreRecording(descriptor with { Name = recording.Name }, out error);
	}

    private static TelecommunicationsGridCreatorGameItemComponentProto CreateTelecommunicationsGridCreatorProto(
        IFuturemud gameworld, string prefix, int numberLength)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 30,
            Name = "Telecommunications Grid Creator",
            Description = "Test",
            RevisionNumber = 1,
            Definition = $"<Definition><Prefix>{prefix}</Prefix><NumberLength>{numberLength}</NumberLength></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (TelecommunicationsGridCreatorGameItemComponentProto)typeof(TelecommunicationsGridCreatorGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static ElectricGridCreatorGameItemComponentProto CreateElectricGridCreatorProto(IFuturemud gameworld)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 31,
            Name = "Electric Grid Creator",
            Description = "Test",
            RevisionNumber = 1,
            Definition = "<Definition />",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (ElectricGridCreatorGameItemComponentProto)typeof(ElectricGridCreatorGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static LiquidGridCreatorGameItemComponentProto CreateLiquidGridCreatorProto(IFuturemud gameworld)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 32,
            Name = "Liquid Grid Creator",
            Description = "Test",
            RevisionNumber = 1,
            Definition = "<Definition />",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (LiquidGridCreatorGameItemComponentProto)typeof(LiquidGridCreatorGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

    private static ImplantTelephoneGameItemComponentProto CreateImplantTelephoneProto(IFuturemud gameworld)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 22,
            Name = "Implant Telephone",
            Description = "Test",
            RevisionNumber = 1,
            Definition =
                "<Definition><External>false</External><ExternalDescription><![CDATA[]]></ExternalDescription><PowerConsumptionInWatts>2.0</PowerConsumptionInWatts><PowerConsumptionDiscountPerQuality>0.0</PowerConsumptionDiscountPerQuality><TargetBody>0</TargetBody><TargetBodypart>0</TargetBodypart><ImplantSpaceOccupied>1.0</ImplantSpaceOccupied><InstallDifficulty>0</InstallDifficulty><ImplantDamageFunctionGrace>0.0</ImplantDamageFunctionGrace><RingText><![CDATA[Incoming call]]></RingText></Definition>",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (ImplantTelephoneGameItemComponentProto)typeof(ImplantTelephoneGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

	private static (Mock<ICharacter> Actor, Mock<IImplantNeuralLink> Link) InstallAudioNeuralLink(
		ImplantTelephoneGameItemComponent component)
	{
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Actor).Returns(actor.Object);
		var link = new Mock<IImplantNeuralLink>();
		link.Setup(x => x.IsLinkedTo(component)).Returns(true);
		link.SetupGet(x => x.DNIConnected).Returns(true);
		link.SetupGet(x => x.PermitsAudio).Returns(true);
		body.SetupGet(x => x.Implants).Returns([component, link.Object]);
		component.InstallImplant(body.Object);
		return (actor, link);
	}

    private static Mock<ITelephoneNumberOwner> CreateLineOwner(IFuturemud gameworld, long parentId, long componentId,
        params ITelephone[] phones)
    {
        Mock<IGameItem> parent = new();
        parent.SetupGet(x => x.Id).Returns(parentId);
        parent.SetupGet(x => x.Name).Returns($"Item {parentId}");
        parent.SetupGet(x => x.Gameworld).Returns(gameworld);
        Mock<ITelephoneNumberOwner> owner = new();
        string? phoneNumber = default;
        owner.SetupGet(x => x.Id).Returns(componentId);
        owner.SetupGet(x => x.Name).Returns($"Line Owner {componentId}");
        owner.SetupGet(x => x.FrameworkItemType).Returns("TelephoneNumberOwner");
        owner.SetupGet(x => x.Parent).Returns(parent.Object);
        owner.SetupProperty(x => x.PreferredNumber);
        owner.SetupProperty(x => x.AllowSharedNumber);
        owner.SetupProperty(x => x.HostedVoicemailEnabled);
        owner.SetupProperty(x => x.TelecommunicationsGrid);
        owner.SetupGet(x => x.PhoneNumber).Returns(() => phoneNumber);
        owner.SetupGet(x => x.ConnectedTelephones).Returns(() => phones);
        owner.Setup(x => x.AssignPhoneNumber(It.IsAny<string?>()))
             .Callback<string?>(value => phoneNumber = value);
        return owner;
    }

    private static GridLiquidSourceGameItemComponentProto CreateGridLiquidSourceProto(IFuturemud gameworld)
    {
        MudSharp.Models.GameItemComponentProto model = new()
        {
            Id = 3,
            Name = "Grid Liquid Source",
            Description = "Test",
            RevisionNumber = 1,
            Definition = "<Definition />",
            EditableItem = new MudSharp.Models.EditableItem
            {
                RevisionStatus = (int)RevisionStatus.Current,
                RevisionNumber = 1
            }
        };

        return (GridLiquidSourceGameItemComponentProto)typeof(GridLiquidSourceGameItemComponentProto)
               .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                   [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
               .Invoke([model, gameworld]);
    }

	private static SpokenLanguageInfo CreateSpokenLanguage(IPerceivable origin, string text = "Hello there")
    {
        Mock<ILanguageDifficultyModel> model = new();
        model.Setup(x => x.RateDifficulty(It.IsAny<ExplodedString>())).Returns(Difficulty.Automatic);
        Mock<ILanguage> language = new();
        language.SetupGet(x => x.Id).Returns(1L);
        language.SetupGet(x => x.Model).Returns(model.Object);
        language.SetupGet(x => x.Name).Returns("Common");
        Mock<IAccent> accent = new();
        accent.SetupGet(x => x.Id).Returns(2L);
        accent.SetupGet(x => x.Language).Returns(language.Object);
		return new SpokenLanguageInfo(language.Object, accent.Object, AudioVolume.Decent, text,
            Outcome.Pass, origin, origin);
    }

    private static Mock<ILanguage> CreateLanguage(long id, string name = "Common")
    {
        Mock<ILanguageDifficultyModel> model = new();
        model.Setup(x => x.RateDifficulty(It.IsAny<ExplodedString>())).Returns(Difficulty.Automatic);
        Mock<ILanguage> language = new();
        language.SetupGet(x => x.Id).Returns(id);
        language.SetupGet(x => x.Name).Returns(name);
        language.SetupGet(x => x.Model).Returns(model.Object);
        return language;
    }

    private static Mock<IAccent> CreateAccent(long id, ILanguage language, string name = "Standard")
    {
        Mock<IAccent> accent = new();
        accent.SetupGet(x => x.Id).Returns(id);
        accent.SetupGet(x => x.Name).Returns(name);
        accent.SetupGet(x => x.Language).Returns(language);
        accent.SetupGet(x => x.AccentSuffix).Returns(name);
        return accent;
    }

    private static Mock<ICharacter> CreateCharacter(IFuturemud gameworld, long id, IRoom location)
    {
        Mock<ICharacter> character = new();
		PhysicalManipulationTestHelper.SetUpUsableHands(character);
        character.SetupGet(x => x.Id).Returns(id);
        character.SetupGet(x => x.Gameworld).Returns(gameworld);
        character.SetupGet(x => x.Location).Returns(location);
        character.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
            It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("Tester");
        character.Setup(x => x.ApparentGender(It.IsAny<IPerceiver>())).Returns(Gendering.Get(Gender.Male));
        return character;
    }

    private sealed class SupplierState
    {
        public required LiquidMixture Mixture { get; set; }
    }

    private sealed class SupplierDouble
    {
        public SupplierDouble(Mock<ILiquidGridSupplier> supplier, SupplierState state)
        {
            Mock = supplier;
            State = state;
        }

        public Mock<ILiquidGridSupplier> Mock { get; }
        public ILiquidGridSupplier Object => Mock.Object;
        public SupplierState State { get; }
    }

    private class TelephoneDouble : ITelephone, ITelephoneNumberOwner
    {
        private readonly IGameItem _parent;
        private readonly IGameItemComponentProto _prototype;
        private string? _phoneNumber;
        private string? _preferredNumber;
        private bool _allowSharedNumber;
        private bool _hostedVoicemailEnabled;
        private bool _isOffHook;
        private bool _isRinging;
        private ITelephoneCall? _currentCall;

        public TelephoneDouble(IFuturemud gameworld, long id)
        {
            Id = id;
            Mock<IGameItem> parent = new();
            parent.SetupGet(x => x.Id).Returns(id);
            parent.SetupGet(x => x.Gameworld).Returns(gameworld);
            _parent = parent.Object;
            _prototype = Mock.Of<IGameItemComponentProto>(x => x.Name == "TelephoneDouble");
            SwitchedOn = true;
            IsPowered = true;
        }

        public List<string> ProgressMessages { get; } = [];
        public List<string> ReceivedDigits { get; } = [];
        public List<string> ReceivedTransmissionTexts { get; } = [];
        public ITelephoneNumberOwner? ExternalOwner { get; set; }
        public ITelephoneNumberOwner? NumberOwner => ExternalOwner ?? this;
        public string? PhoneNumber => NumberOwner == this ? _phoneNumber : NumberOwner?.PhoneNumber;
        public string? PreferredNumber
        {
            get => NumberOwner == this ? _preferredNumber : NumberOwner?.PreferredNumber;
            set
            {
                if (NumberOwner != this)
                {
                    NumberOwner!.PreferredNumber = value;
                    return;
                }

                _preferredNumber = value;
            }
        }
        public bool AllowSharedNumber
        {
            get => NumberOwner == this ? _allowSharedNumber : NumberOwner?.AllowSharedNumber ?? false;
            set
            {
                if (NumberOwner != this)
                {
                    NumberOwner!.AllowSharedNumber = value;
                    return;
                }

                _allowSharedNumber = value;
            }
        }
        public bool HostedVoicemailEnabled
        {
            get => NumberOwner == this ? _hostedVoicemailEnabled : NumberOwner?.HostedVoicemailEnabled ?? false;
            set
            {
                if (NumberOwner != this)
                {
                    NumberOwner!.HostedVoicemailEnabled = value;
                    return;
                }

                _hostedVoicemailEnabled = value;
            }
        }
        public bool IsPowered { get; set; }
        public bool IsOffHook => _isOffHook;
        public bool CanReceiveCalls =>
            SwitchedOn && IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
            !_isOffHook && _currentCall == null;
        public bool IsRinging => _isRinging;
        public bool IsConnected => _currentCall?.Participants.Contains(this) == true && _currentCall.IsConnected;
        public bool IsEngaged => _currentCall != null || _isOffHook;
        public AudioVolume RingVolume => AudioVolume.Decent;
        public ITelephoneCall? CurrentCall => _currentCall;
        public IEnumerable<ITelephone> ConnectedPhones => _currentCall?.Participants.Where(x => x != this).ToList() ?? [];
        public ITelephone? ConnectedPhone => ConnectedPhones.FirstOrDefault();
        public ITelecommunicationsGrid? TelecommunicationsGrid
        {
            get => NumberOwner == this ? _telecommunicationsGrid : NumberOwner?.TelecommunicationsGrid;
            set
            {
                if (NumberOwner != this)
                {
                    NumberOwner!.TelecommunicationsGrid = value;
                    return;
                }

                _telecommunicationsGrid = value;
            }
        }
        private ITelecommunicationsGrid? _telecommunicationsGrid;
        IEnumerable<ITelephone> ITelephoneNumberOwner.ConnectedTelephones => [this];

        public void AssignPhoneNumber(string? number)
        {
            _phoneNumber = number;
        }

        public bool CanPickUp(ICharacter actor, out string error)
        {
            if (_isOffHook && _currentCall == null)
            {
                error = "That telephone is already off the hook.";
                return false;
            }

            if (TelecommunicationsGrid == null)
            {
                error = "That telephone is not connected to a telecommunications line.";
                return false;
            }

            if (!SwitchedOn || !IsPowered)
            {
                error = "That telephone is not ready to use right now.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool PickUp(ICharacter actor, out string error)
        {
            if (!CanPickUp(actor, out error))
            {
                return false;
            }

            if (TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber))
            {
                if (TelecommunicationsGrid.TryPickUp(this, out error))
                {
                    return true;
                }

                if (!error.EqualTo("There is no live call on that line right now."))
                {
                    return false;
                }
            }

            _isOffHook = true;
            error = string.Empty;
            return true;
        }

        public bool CanDial(ICharacter actor, string number, out string error)
        {
            if (_currentCall?.IsConnected == true)
            {
                return CanSendDigits(actor, number, out error);
            }

            if (TelecommunicationsGrid == null)
            {
                error = "That telephone is not connected to a telecommunications grid.";
                return false;
            }

            if (!CanReceiveCalls)
            {
                error = "That telephone is not ready to make calls right now.";
                return false;
            }

            if (IsEngaged)
            {
                error = "That telephone is already in use.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool Dial(ICharacter actor, string number, out string error)
        {
            if (_currentCall?.IsConnected == true)
            {
                return SendDigits(actor, number, out error);
            }

            if (!CanDial(actor, number, out error))
            {
                return false;
            }

            _isOffHook = true;
            return TelecommunicationsGrid!.TryStartCall(this, number, out error);
        }

        public bool CanSendDigits(ICharacter actor, string digits, out string error)
        {
            if (_currentCall?.IsConnected != true)
            {
                error = "That telephone is not connected to a live call.";
                return false;
            }

            string normalised = new((digits ?? string.Empty).Where(x => !char.IsWhiteSpace(x)).ToArray());
            if (string.IsNullOrEmpty(normalised) || normalised.Any(x => !char.IsDigit(x) && x is not '*' and not '#'))
            {
                error = "You may only send keypad digits from 0-9, * and #.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool SendDigits(ICharacter actor, string digits, out string error)
        {
            if (!CanSendDigits(actor, digits, out error))
            {
                return false;
            }

            string normalised = new((digits ?? string.Empty).Where(x => !char.IsWhiteSpace(x)).ToArray());
            _currentCall!.RelayDigits(this, normalised);
            error = string.Empty;
            return true;
        }

        public bool CanAnswer(ICharacter actor, out string error)
        {
            if (!IsRinging || _currentCall == null)
            {
                error = "That telephone is not ringing.";
                return false;
            }

            if (!SwitchedOn || !IsPowered)
            {
                error = "That telephone cannot answer calls right now.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool Answer(ICharacter actor, out string error)
        {
            if (!CanAnswer(actor, out error))
            {
                return false;
            }

            return TelecommunicationsGrid!.TryPickUp(this, out error);
        }

        public bool CanHangUp(ICharacter actor, out string error)
        {
            if (!IsEngaged)
            {
                error = "That telephone is not currently in use.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool HangUp(ICharacter actor, out string error)
        {
            if (!CanHangUp(actor, out error))
            {
                return false;
            }

            EndCall(_currentCall);
            return true;
        }

        public void BeginOutgoingCall(ITelephoneCall call, string number)
        {
            _currentCall = call;
            _isOffHook = true;
            _isRinging = false;
        }

        public void ReceiveIncomingCall(ITelephoneCall call)
        {
            _currentCall = call;
            _isOffHook = false;
            _isRinging = true;
        }

        public void ConnectCall(ITelephoneCall call)
        {
            _currentCall = call;
            _isOffHook = true;
            _isRinging = false;
        }

        public void NotifyCallProgress(string message)
        {
            ProgressMessages.Add(message);
        }

        public void ReceiveDigits(ITelephone source, string digits)
        {
            ReceivedDigits.Add(digits);
        }

        public void EndCall(ITelephoneCall? call, bool notifyGrid = true)
        {
            if (call != null && _currentCall != null && !ReferenceEquals(call, _currentCall))
            {
                return;
            }

            ITelephoneCall? existingCall = _currentCall;
            _currentCall = null;
            _isRinging = false;
            _isOffHook = false;
            if (notifyGrid && existingCall != null)
            {
                TelecommunicationsGrid?.EndCall(this, existingCall);
            }
        }

        public bool ManualTransmit => true;
        public string TransmitPremote => "@ speak|speaks into $1 and say|says";

        public void Transmit(SpokenLanguageInfo spokenLanguage)
        {
            _currentCall?.RelayTransmission(this, spokenLanguage);
        }

        public void ReceiveTransmission(double frequency, SpokenLanguageInfo spokenLanguage, long encryption,
            ITransmit origin)
        {
            ReceivedTransmissionTexts.Add(spokenLanguage.RawText);
        }

        public void ReceiveTransmission(double frequency, string dataTransmission, long encryption, ITransmit origin)
        {
        }

        public double PowerConsumptionInWatts => 0.0;

        public void OnPowerCutIn()
        {
            IsPowered = true;
        }

        public void OnPowerCutOut()
        {
            IsPowered = false;
            EndCall(_currentCall);
        }

        public bool SwitchedOn { get; set; }

        public IEnumerable<string> SwitchSettings => ["on", "off"];

        public bool CanSwitch(ICharacter actor, string setting)
        {
            return setting.Equals("on", StringComparison.InvariantCultureIgnoreCase) ? !SwitchedOn
                : setting.Equals("off", StringComparison.InvariantCultureIgnoreCase) && SwitchedOn;
        }

        public string WhyCannotSwitch(ICharacter actor, string setting)
        {
            return "That is already in that state.";
        }

        public bool Switch(ICharacter actor, string setting)
        {
            if (!CanSwitch(actor, setting))
            {
                return false;
            }

            SwitchedOn = setting.Equals("on", StringComparison.InvariantCultureIgnoreCase);
            if (!SwitchedOn)
            {
                EndCall(_currentCall);
            }

            return true;
        }

        public IFuturemud Gameworld => _parent.Gameworld;
        public bool Changed { get; set; }
        public void Save()
        {
        }

        public bool HooksChanged { get; set; }
        public bool HandleEvent(EventType type, params dynamic[] arguments)
        {
            return false;
        }

        public bool HandlesEvent(params EventType[] types)
        {
            return false;
        }

        public bool InstallHook(IHook hook)
        {
            return false;
        }

        public bool RemoveHook(IHook hook)
        {
            return false;
        }

        public IEnumerable<IHook> Hooks => Enumerable.Empty<IHook>();
        public string Name => "TelephoneDouble";
        public long Id { get; }
        public string FrameworkItemType => "TelephoneDouble";
        public IGameItem Parent => _parent;
        public IGameItemComponentProto Prototype => _prototype;
        public bool DesignedForOffhandUse => false;
        public int DecorationPriority => 0;
        public double ComponentWeight => 0.0;
        public double ComponentWeightMultiplier => 1.0;
        public double ComponentBuoyancy(double fluidDensity)
        {
            return 0.0;
        }

        public bool OverridesMaterial => false;
        public ISolid OverridenMaterial => null!;
        public bool WrapFullDescription => true;
        public bool AffectsLocationOnDestruction => false;
        public int ComponentDieOrder => 0;
        public void Delete()
        {
        }

        public void Quit()
        {
        }

        public void Login()
        {
        }

        public void PrimeComponentForInsertion(MudSharp.Models.GameItem parent)
        {
        }

        public bool HandleDieOrMorph(IGameItem newItem, IRoom location)
        {
            return false;
        }

        public bool SwapInPlace(IGameItem existingItem, IGameItem newItem)
        {
            return false;
        }

        public bool Take(IGameItem item)
        {
            return false;
        }

        public IGameItemComponent Copy(IGameItem newParent, bool temporary = false)
        {
            return this;
        }

        public bool DescriptionDecorator(DescriptionType type)
        {
            return false;
        }

        public string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
                    PerceiveIgnoreFlags flags)
        {
            return description;
        }

        public event EventHandler? DescriptionUpdate
        {
            add { }
            remove { }
        }
        public bool PreventsMerging(IGameItemComponent component)
        {
            return false;
        }

        public bool PreventsRepositioning()
        {
            return false;
        }

        public bool PreventsMovement()
        {
            return false;
        }

        public string WhyPreventsMovement(ICharacter mover)
        {
            return string.Empty;
        }

        public void ForceMove()
        {
        }

        public string WhyPreventsRepositioning()
        {
            return string.Empty;
        }

        public bool CheckPrototypeForUpdate()
        {
            return false;
        }

        public void FinaliseLoad()
        {
        }

        public void Taken()
        {
        }

        public bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier)
        {
            return true;
        }

        public IBody HaveABody => null!;
        public bool WarnBeforePurge => false;
        public bool ExposeToLiquid(LiquidMixture mixture)
        {
            return false;
        }
    }

    private sealed class FaxMachineDouble : TelephoneDouble, IFaxMachine
    {
        private readonly List<List<ICanBeRead>> _pendingFaxes = [];
        private readonly List<List<ICanBeRead>> _completedFaxes = [];

        public FaxMachineDouble(IFuturemud gameworld, long id) : base(gameworld, id)
        {
        }

        public bool SupportsVoiceCalls => false;
        public bool CanReceiveFaxes =>
            SwitchedOn &&
            IsPowered &&
            TelecommunicationsGrid != null &&
            !string.IsNullOrWhiteSpace(PhoneNumber) &&
            !IsEngaged &&
            !IsRinging;
        public int PendingFaxCount => _pendingFaxes.Count;
        public int CompletedFaxCount => _completedFaxes.Count;
        public int AvailablePages { get; set; } = int.MaxValue;
        public int AvailableInk { get; set; } = int.MaxValue;

        public bool CanSendFax(ICharacter actor, string number, IReadable document, out string error)
        {
            if (TelecommunicationsGrid == null)
            {
                error = "That fax machine is not connected to a telecommunications grid.";
                return false;
            }

            if (!SwitchedOn || !IsPowered || string.IsNullOrWhiteSpace(PhoneNumber))
            {
                error = "That fax machine is not ready to send faxes right now.";
                return false;
            }

            if (document == null || !document.Readables.Any())
            {
                error = "That document has nothing on it to fax.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(number))
            {
                error = "You must specify a number to fax.";
                return false;
            }

            error = string.Empty;
            return true;
        }

        public bool SendFax(ICharacter actor, string number, IReadable document, out string error)
        {
            if (!CanSendFax(actor, number, document, out error))
            {
                return false;
            }

            return TelecommunicationsGrid!.TrySendFax(this, number, document.Readables.ToList(), out error);
        }

        public void ReceiveFax(string senderNumber, IReadOnlyCollection<ICanBeRead> document)
        {
            _pendingFaxes.Add(document.ToList());
            ProcessPendingFaxes();
        }

        public void ProcessPendingFaxes()
        {
            foreach (List<ICanBeRead>? fax in _pendingFaxes.ToList())
            {
                int requiredInk = fax.Sum(x => x.DocumentLength);
                if (AvailablePages <= 0 || AvailableInk < requiredInk)
                {
                    break;
                }

                AvailablePages -= 1;
                AvailableInk -= requiredInk;
                _pendingFaxes.Remove(fax);
                _completedFaxes.Add(fax);
            }
        }
    }

    private static ICanBeRead CreateReadableDocument(int length)
    {
        Mock<ICanBeRead> readable = new();
        readable.SetupGet(x => x.DocumentLength).Returns(length);
        readable.SetupGet(x => x.Id).Returns(length);
        readable.SetupGet(x => x.Name).Returns($"Document {length}");
        readable.SetupGet(x => x.FrameworkItemType).Returns("Readable");
        readable.SetupGet(x => x.Author).Returns((ICharacter)null!);
        readable.SetupGet(x => x.ImplementType).Returns(WritingImplementType.Biro);
        return readable.Object;
    }
}
