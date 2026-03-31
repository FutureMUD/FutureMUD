#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
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

namespace MudSharp_Unit_Tests;

[TestClass]
public class GridSystemTests
{
	[TestMethod]
	public void GridPowerSupplyProto_RoundTripsWattage()
	{
		var gameworld = CreateGameworld();
		var proto = CreateGridPowerSupplyProto(gameworld.Object, 37.5);

		Assert.AreEqual(37.5, proto.Wattage, 0.0001);

		var saveMethod = typeof(GridPowerSupplyGameItemComponentProto).GetMethod("SaveToXml",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var xml = (string)saveMethod!.Invoke(proto, [])!;
		StringAssert.Contains(xml, "<Wattage>37.5</Wattage>");
	}

	[TestMethod]
	public void GridPowerSupplyComponent_UsesConfiguredWattageAndDeduplicatesConsumers()
	{
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var proto = CreateGridPowerSupplyProto(gameworld.Object, 50.0);
		var component = new GridPowerSupplyGameItemComponent(proto, parent.Object, true);

		var grid = new Mock<IElectricalGrid>();
		grid.Setup(x => x.DrawdownSpike(10.0)).Returns(true);
		component.ElectricalGrid = grid.Object;
		component.OnPowerCutIn();

		var consumerOne = new Mock<IConsumePower>();
		consumerOne.SetupGet(x => x.PowerConsumptionInWatts).Returns(20.0);
		var consumerTwo = new Mock<IConsumePower>();
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
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var proto = CreateGridPowerSupplyProto(gameworld.Object, 50.0);
		var component = new GridPowerSupplyGameItemComponent(proto, parent.Object, true);

		var consumerOne = new Mock<IConsumePower>();
		consumerOne.SetupGet(x => x.PowerConsumptionInWatts).Returns(20.0);
		var consumerTwo = new Mock<IConsumePower>();
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
		var gameworld = CreateGameworld();
		var grid = new LiquidGrid(gameworld.Object, CreateCell().Object);
		var water = CreateLiquid(1, "water").Object;
		var toxin = CreateLiquid(2, "toxin", Telnet.Red).Object;
		var supplierOne = CreateSupplier(CreateMixture(gameworld.Object, (water, 3.0)));
		var supplierTwo = CreateSupplier(CreateMixture(gameworld.Object, (toxin, 1.0)));

		grid.JoinGrid(supplierOne.Object);
		grid.JoinGrid(supplierTwo.Object);

		var mixture = grid.CurrentLiquidMixture;
		Assert.AreEqual(4.0, mixture.TotalVolume, 0.0001);
		Assert.AreEqual(3.0, GetLiquidAmount(mixture, water), 0.0001);
		Assert.AreEqual(1.0, GetLiquidAmount(mixture, toxin), 0.0001);
	}

	[TestMethod]
	public void LiquidGrid_RemoveLiquidAmount_DepletesSuppliersProRata()
	{
		var gameworld = CreateGameworld();
		var grid = new LiquidGrid(gameworld.Object, CreateCell().Object);
		var water = CreateLiquid(1, "water").Object;
		var toxin = CreateLiquid(2, "toxin", Telnet.Red).Object;
		var supplierOne = CreateSupplier(CreateMixture(gameworld.Object, (water, 3.0)));
		var supplierTwo = CreateSupplier(CreateMixture(gameworld.Object, (toxin, 1.0)));

		grid.JoinGrid(supplierOne.Object);
		grid.JoinGrid(supplierTwo.Object);
		var removed = grid.RemoveLiquidAmount(2.0, null, "test");

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
		var gameworld = CreateGameworld();
		var grid = new LiquidGrid(gameworld.Object, CreateCell().Object);
		var water = CreateLiquid(1, "water").Object;
		var supplier = CreateSupplier(CreateMixture(gameworld.Object, (water, 2.0)));
		grid.JoinGrid(supplier.Object);

		Assert.AreEqual(2.0, grid.TotalLiquidVolume, 0.0001);

		supplier.State.Mixture = CreateMixture(gameworld.Object, (water, 5.0));

		Assert.AreEqual(5.0, grid.TotalLiquidVolume, 0.0001);
		Assert.AreEqual(5.0, grid.CurrentLiquidMixture.TotalVolume, 0.0001);
	}

	[TestMethod]
	public void LiquidGrid_CopyDoesNotAttachRoomSuppliers()
	{
		var gameworld = CreateGameworld();
		var supplier = CreateSupplier(CreateMixture(gameworld.Object, (CreateLiquid(1, "water").Object, 3.0)));
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(10L);
		item.Setup(x => x.GetItemType<ILiquidGridSupplier>()).Returns(supplier.Object);
		var cell = CreateCell();
		cell.SetupGet(x => x.GameItems).Returns([item.Object]);

		var original = new Mock<ILiquidGrid>();
		original.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		original.SetupGet(x => x.Locations).Returns([cell.Object]);

		var clone = new LiquidGrid(original.Object);

		Assert.AreEqual(0.0, clone.TotalLiquidVolume, 0.0001);
		Assert.IsTrue(clone.CurrentLiquidMixture.IsEmpty);
	}

	[TestMethod]
	public void TelecommunicationsGrid_AssignsNumbersAndHonoursPreferredNumber()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var phoneOne = new TelephoneDouble(gameworld.Object, 1);
		var phoneTwo = new TelephoneDouble(gameworld.Object, 2) { PreferredNumber = "5559999" };

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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);
		var busy = new TelephoneDouble(gameworld.Object, 3);
		var unpowered = new TelephoneDouble(gameworld.Object, 4) { IsPowered = false };

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		busy.TelecommunicationsGrid = grid;
		unpowered.TelecommunicationsGrid = grid;

		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);
		grid.JoinGrid((ITelephoneNumberOwner)busy);
		grid.JoinGrid((ITelephoneNumberOwner)unpowered);

		Assert.IsFalse(grid.TryStartCall(caller, "9999999", out var error));
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var primary = new TelephoneDouble(gameworld.Object, 2)
		{
			PreferredNumber = "5554444",
			AllowSharedNumber = true
		};
		var extension = new TelephoneDouble(gameworld.Object, 3)
		{
			PreferredNumber = "5554444",
			AllowSharedNumber = true
		};
		var blocked = new TelephoneDouble(gameworld.Object, 4);

		foreach (var phone in new[] { caller, primary, extension, blocked })
		{
			phone.TelecommunicationsGrid = grid;
			grid.JoinGrid((ITelephoneNumberOwner)phone);
		}

		Assert.IsTrue(blocked.PickUp(null!, out var error), error);
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		grid.SetMaximumRings(2);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		CollectionAssert.Contains(caller.ProgressMessages, "You hear the line ringing.");

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		Assert.IsTrue(receiver.Answer(null!, out error), error);

		CollectionAssert.Contains(caller.ProgressMessages, "The call connects.");
	}

	[TestMethod]
	public void TelecommunicationsGrid_PhoneDiallingFaxLine_PlaysModemNoiseAndDisconnects()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var fax = new FaxMachineDouble(gameworld.Object, 2);

		caller.TelecommunicationsGrid = grid;
		fax.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)fax);

		Assert.IsTrue(grid.TryStartCall(caller, fax.PhoneNumber!, out var error), error);
		CollectionAssert.Contains(caller.ProgressMessages,
			"The line answers with a burst of unintelligible modem-like noises before disconnecting.");
		Assert.IsFalse(caller.IsEngaged);
	}

	[TestMethod]
	public void TelecommunicationsGrid_FaxToFax_DeliversDocument()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var sender = new FaxMachineDouble(gameworld.Object, 1);
		var receiver = new FaxMachineDouble(gameworld.Object, 2);
		var document = CreateReadableDocument(120);

		sender.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)sender);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out var error), error);
		Assert.AreEqual(1, receiver.CompletedFaxCount);
		Assert.AreEqual(0, receiver.PendingFaxCount);
	}

	[TestMethod]
	public void TelecommunicationsGrid_FaxToVoiceLine_NotifiesRecipientAndFails()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var sender = new FaxMachineDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);
		var document = CreateReadableDocument(120);

		sender.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)sender);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsFalse(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out var error));
		StringAssert.Contains(error, "modem-like noises");
		CollectionAssert.Contains(receiver.ProgressMessages,
			"The line erupts with a burst of unintelligible modem-like noises before disconnecting.");
	}

	[TestMethod]
	public void TelecommunicationsGrid_FaxQueuesUntilPaperOrInkBecomeAvailable()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var sender = new FaxMachineDouble(gameworld.Object, 1);
		var receiver = new FaxMachineDouble(gameworld.Object, 2)
		{
			AvailablePages = 0
		};
		var document = CreateReadableDocument(120);

		sender.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)sender);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TrySendFax(sender, receiver.PhoneNumber!, [document], out var error), error);
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4, true, "9999");
		grid.SetMaximumRings(2);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2)
		{
			HostedVoicemailEnabled = true
		};

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

		Assert.IsTrue(caller.IsConnected);
		Assert.IsFalse(receiver.IsEngaged);
		CollectionAssert.Contains(caller.ProgressMessages, "The exchange voicemail service answers.");
		CollectionAssert.Contains(caller.ProgressMessages, "A sharp beep sounds over the line.");

		caller.Transmit(CreateSpokenLanguage(caller.Parent));
		Assert.IsTrue(caller.HangUp(null!, out error), error);

		var mailboxesField = typeof(TelecommunicationsGrid).GetField("_hostedVoicemailRecordings",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var mailboxes =
			(Dictionary<string, List<StoredAudioRecording>>)mailboxesField!.GetValue(grid)!;
		Assert.IsTrue(mailboxes.ContainsKey(receiver.PhoneNumber!));
		Assert.AreEqual(1, mailboxes[receiver.PhoneNumber!].Count);
		Assert.AreEqual("Hello there", mailboxes[receiver.PhoneNumber!][0].Recording.Segments[0].RawText);
	}

	[TestMethod]
	public void TelecommunicationsGrid_LocalAnsweringMachineTakesPriorityOverHostedVoicemail()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4, true, "9999");
		grid.SetMaximumRings(2);
		var machineParent = CreateBasicItem(gameworld.Object, 230L, CreateCell().Object);
		var tape = CreateTape(gameworld.Object, 231L);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2),
			machineParent.Object, true);
		var caller = new TelephoneDouble(gameworld.Object, 232);

		machine.Put(null!, tape.Item.Object);
		((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
		((ITelephoneNumberOwner)machine).HostedVoicemailEnabled = true;
		machine.OnPowerCutIn();
		caller.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);

		Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out var error), error);

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

		Assert.IsTrue(machine.IsConnected);
		Assert.IsTrue(caller.IsConnected);
		Assert.IsFalse(caller.ProgressMessages.Any(x => x.Contains("exchange voicemail service", StringComparison.InvariantCultureIgnoreCase)));
	}

	[TestMethod]
	public void TelecommunicationsGrid_HostedVoicemailAccessPlaysAndDeletesMessages()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4, true, "9999");
		grid.SetMaximumRings(2);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2)
		{
			HostedVoicemailEnabled = true
		};

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
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
		var stored = new StoredAudioRecording(
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

		var xml = stored.SaveToXml();
		var loaded = StoredAudioRecording.LoadFromXml(xml);

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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);

		caller.TelecommunicationsGrid = grid;
		receiver.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);
		grid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(grid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		Assert.IsTrue(receiver.Answer(null!, out error), error);
		Assert.IsTrue(caller.Dial(null!, "1 2# *", out error), error);

		CollectionAssert.AreEqual(new[] { "12#*" }, receiver.ReceivedDigits.ToArray());
	}

	[TestMethod]
	public void TelephoneGameItemComponent_ReceiveDigits_FiresTelephoneDigitsReceivedEvent()
	{
		var gameworld = CreateGameworld();
		var parent = CreateBasicItem(gameworld.Object, 88L, CreateCell().Object);
		var proto = CreateTelephoneProto(gameworld.Object, 0.0);
		var component = new TelephoneGameItemComponent(proto, parent.Object, true);
		var source = new TelephoneDouble(gameworld.Object, 89);

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
		var language = CreateLanguage(101);
		var accent = CreateAccent(201, language.Object);
		var gameworld = CreateGameworld(languageList: [language.Object], accentList: [accent.Object]);
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var machineParent = CreateBasicItem(gameworld.Object, 200L, CreateCell().Object);
		var tape = CreateTape(gameworld.Object, 201L);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
		var caller = new TelephoneDouble(gameworld.Object, 202);

		Assert.IsTrue(tape.Component.StoreRecording(
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
			out var error), error);

		machine.Put(null!, tape.Item.Object);
		((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
		machine.OnPowerCutIn();
		caller.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);

		Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out error), error);

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
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
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var machineParent = CreateBasicItem(gameworld.Object, 210L, CreateCell().Object);
		var tape = CreateTape(gameworld.Object, 211L);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
		var caller = new TelephoneDouble(gameworld.Object, 212);
		var extension = new TelephoneDouble(gameworld.Object, 213) { ExternalOwner = machine };

		machine.Put(null!, tape.Item.Object);
		((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
		machine.OnPowerCutIn();
		caller.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);

		Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out var error), error);

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
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
	public void TelecommunicationsGrid_AnsweringMachineWithWriteProtectedTapeStillAnswers()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var machineParent = CreateBasicItem(gameworld.Object, 220L, CreateCell().Object);
		var tape = CreateTape(gameworld.Object, 221L);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
		var caller = new TelephoneDouble(gameworld.Object, 222);

		tape.Component.WriteProtected = true;
		machine.Put(null!, tape.Item.Object);
		((ITelephoneNumberOwner)machine).TelecommunicationsGrid = grid;
		machine.OnPowerCutIn();
		caller.TelecommunicationsGrid = grid;
		grid.JoinGrid((ITelephoneNumberOwner)caller);

		Assert.IsTrue(grid.TryStartCall(caller, machine.PhoneNumber!, out var error), error);

		var heartbeat = Mock.Get(gameworld.Object.HeartbeatManager);
		heartbeat.Raise(x => x.FuzzyFiveSecondHeartbeat += null);

		Assert.IsTrue(machine.IsConnected);
		Assert.IsFalse(machine.IsRecordingMessage);

		caller.Transmit(CreateSpokenLanguage(caller.Parent));
		Assert.IsTrue(caller.HangUp(null!, out error), error);

		Assert.AreEqual(0, machine.MessageRecordings.Count);
	}

	[TestMethod]
	public void AnsweringMachine_SelectGreetingRecordStop_SavesGreetingToTape()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell().Object;
		var machineParent = CreateBasicItem(gameworld.Object, 230L, cell);
		var tape = CreateTape(gameworld.Object, 231L);
		var machine = new AnsweringMachineGameItemComponent(CreateAnsweringMachineProto(gameworld.Object, 2), machineParent.Object, true);
		var actor = CreateCharacter(gameworld.Object, 232L, cell);
		var language = CreateLanguage(301);
		var accent = CreateAccent(302, language.Object);

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
	public void AnsweringMachine_LoadRestoresInsertedTapeAndSettings()
	{
		var tapeGameworld = CreateGameworld();
		var tape = CreateTape(tapeGameworld.Object, 241L);
		var gameworld = CreateGameworld(itemList: [tape.Item.Object]);
		gameworld.Setup(x => x.TryGetItem(tape.Item.Object.Id, true)).Returns(tape.Item.Object);

		var parent = CreateBasicItem(gameworld.Object, 240L, CreateCell().Object);
		var proto = CreateAnsweringMachineProto(gameworld.Object, 4);
		var component = new AnsweringMachineGameItemComponent(new MudSharp.Models.GameItemComponent
		{
			Id = 1,
			Definition =
				$"<Definition><Grid>0</Grid><SwitchedOn>false</SwitchedOn><PreferredNumber></PreferredNumber><AllowSharedNumber>false</AllowSharedNumber><AutoAnswerRings>5</AutoAnswerRings><RingVolumeOverride>-1</RingVolumeOverride><Tape>{tape.Item.Object.Id}</Tape><ConnectedItems /></Definition>"
		}, proto, parent.Object);

		component.FinaliseLoad();

		Assert.IsFalse(component.SwitchedOn);
		Assert.AreEqual(5, component.AutoAnswerRings);
		Assert.IsNotNull(component.Tape);
		Assert.AreEqual(tape.Item.Object, component.Contents.Single());
	}

	[TestMethod]
	public void TelecommunicationsGrid_RoutesLongDistanceCallsAcrossLinkedExchanges()
	{
		var gameworld = CreateGameworld();
		var localGrid = new TelecommunicationsGrid(gameworld.Object, CreateCell(1).Object, "555", 4);
		var remoteGrid = new TelecommunicationsGrid(gameworld.Object, CreateCell(2).Object, "777", 4);
		localGrid.LinkGrid(remoteGrid);

		var caller = new TelephoneDouble(gameworld.Object, 1);
		var receiver = new TelephoneDouble(gameworld.Object, 2);
		caller.TelecommunicationsGrid = localGrid;
		receiver.TelecommunicationsGrid = remoteGrid;
		localGrid.JoinGrid((ITelephoneNumberOwner)caller);
		remoteGrid.JoinGrid((ITelephoneNumberOwner)receiver);

		Assert.IsTrue(localGrid.TryStartCall(caller, receiver.PhoneNumber!, out var error), error);
		Assert.IsTrue(receiver.IsRinging);
		Assert.IsFalse(caller.IsConnected);

		Assert.IsTrue(receiver.Answer(null!, out error), error);
		Assert.IsTrue(caller.IsConnected);
		Assert.IsTrue(receiver.IsConnected);
	}

	[TestMethod]
	public void TelecommunicationsGrid_DoesNotForwardWhenDialledPrefixMatchesLocalExchange()
	{
		var gameworld = CreateGameworld();
		var localGrid = new TelecommunicationsGrid(gameworld.Object, CreateCell(1).Object, "555", 4);
		var conflictingRemoteGrid = new TelecommunicationsGrid(gameworld.Object, CreateCell(2).Object, "555", 4);
		localGrid.LinkGrid(conflictingRemoteGrid);

		var caller = new TelephoneDouble(gameworld.Object, 1);
		var localReceiver = new TelephoneDouble(gameworld.Object, 2);
		var remoteReceiver = new TelephoneDouble(gameworld.Object, 3);

		caller.TelecommunicationsGrid = localGrid;
		localReceiver.TelecommunicationsGrid = localGrid;
		remoteReceiver.TelecommunicationsGrid = conflictingRemoteGrid;
		localGrid.JoinGrid((ITelephoneNumberOwner)caller);
		localGrid.JoinGrid((ITelephoneNumberOwner)localReceiver);
		conflictingRemoteGrid.JoinGrid((ITelephoneNumberOwner)remoteReceiver);

		Assert.IsTrue(localGrid.TryStartCall(caller, localReceiver.PhoneNumber!, out var error), error);
		Assert.IsTrue(localReceiver.IsRinging);
		Assert.IsFalse(remoteReceiver.IsRinging);
	}

	[TestMethod]
	public void TelecommunicationsGrid_RejectsAmbiguousLinkedExchangePrefixes()
	{
		var gameworld = CreateGameworld();
		var localGrid = new TelecommunicationsGrid(gameworld.Object, CreateCell(1).Object, "555", 4);
		var remoteOne = new TelecommunicationsGrid(gameworld.Object, CreateCell(2).Object, "777", 4);
		var remoteTwo = new TelecommunicationsGrid(gameworld.Object, CreateCell(3).Object, "777", 4);
		localGrid.LinkGrid(remoteOne);
		localGrid.LinkGrid(remoteTwo);

		var caller = new TelephoneDouble(gameworld.Object, 1);
		caller.TelecommunicationsGrid = localGrid;
		localGrid.JoinGrid((ITelephoneNumberOwner)caller);

		Assert.IsFalse(localGrid.TryStartCall(caller, "7771234", out var error));
		StringAssert.Contains(error, "more than one linked");
	}

	[TestMethod]
	public void TelephoneNumber_FollowsAssignedEndpointRatherThanHandset()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var phone = new TelephoneDouble(gameworld.Object, 10);
		var lineA = CreateLineOwner(gameworld.Object, 11, 101, phone);
		var lineB = CreateLineOwner(gameworld.Object, 12, 102, phone);

		lineA.Object.TelecommunicationsGrid = grid;
		lineB.Object.TelecommunicationsGrid = grid;
		grid.JoinGrid(lineA.Object);
		grid.JoinGrid(lineB.Object);

		phone.ExternalOwner = lineA.Object;
		var lineANumber = phone.PhoneNumber;
		Assert.IsFalse(string.IsNullOrWhiteSpace(lineANumber));

		phone.ExternalOwner = lineB.Object;
		var lineBNumber = phone.PhoneNumber;
		Assert.IsFalse(string.IsNullOrWhiteSpace(lineBNumber));
		Assert.AreNotEqual(lineANumber, lineBNumber);
	}

	[TestMethod]
	public void TelecommunicationsGrid_LoadTimeInitialiseRestoresAssignmentsByComponentId()
	{
		var runtimeGameworld = CreateGameworld();
		var runtimeGrid = new TelecommunicationsGrid(runtimeGameworld.Object, CreateCell().Object, "555", 4);
		var lineA = CreateLineOwner(runtimeGameworld.Object, 11, 101);
		var lineB = CreateLineOwner(runtimeGameworld.Object, 11, 102);
		runtimeGrid.JoinGrid(lineA.Object);
		runtimeGrid.JoinGrid(lineB.Object);
		var lineANumber = runtimeGrid.GetPhoneNumber(lineA.Object);
		var lineBNumber = runtimeGrid.GetPhoneNumber(lineB.Object);

		var saveDefinition = (XElement)typeof(TelecommunicationsGrid)
			.GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(runtimeGrid, [])!;
		saveDefinition.Elements("Location").Remove();

		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(11L);
		item.SetupGet(x => x.Name).Returns("Telephone Hub");
		item.SetupGet(x => x.Gameworld).Returns(runtimeGameworld.Object);
		item.SetupGet(x => x.Components).Returns([lineA.Object, lineB.Object]);
		item.Setup(x => x.GetItemType<ITelephoneNumberOwner>()).Returns(lineA.Object);

		var loadGameworld = CreateGameworld([item.Object]);
		var loadedGrid = new TelecommunicationsGrid(new MudSharp.Models.Grid
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
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		var proto = CreateGridLiquidSourceProto(gameworld.Object);
		var component = new GridLiquidSourceGameItemComponent(proto, parent.Object, true);
		var mixture = CreateMixture(gameworld.Object, (CreateLiquid(1, "water").Object, 1.0));

		Assert.AreEqual(0.0, component.LiquidCapacity, 0.0001);
		Assert.ThrowsException<InvalidOperationException>(() => component.MergeLiquid(mixture, null!, "test"));
	}

	[TestMethod]
	public void TelephoneGameItemComponent_Transmit_ForwardsSpeechToConnectedPhone()
	{
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Id).Returns(1L);
		var proto = CreateTelephoneProto(gameworld.Object, 5.0);
		var component = new TelephoneGameItemComponent(proto, parent.Object, true);
		var call = new Mock<ITelephoneCall>();
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
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Id).Returns(2L);
		var proto = CreateTelephoneProto(gameworld.Object, 5.0);
		var component = new TelephoneGameItemComponent(proto, parent.Object, true);

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
		var gameworld = CreateGameworld();
		var wearerOutput = new Mock<IOutputHandler>();
		var wearer = new Mock<ICharacter>();
		wearer.SetupGet(x => x.OutputHandler).Returns(wearerOutput.Object);
		var body = new Mock<IBody>();
		body.SetupGet(x => x.Actor).Returns(wearer.Object);
		body.SetupGet(x => x.OutputHandler).Returns(wearerOutput.Object);
		var wornContainer = new Mock<IGameItem>();
		wornContainer.SetupGet(x => x.InInventoryOf).Returns(body.Object);
		body.SetupGet(x => x.WornItems).Returns([wornContainer.Object]);

		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Id).Returns(3L);
		parent.SetupGet(x => x.ContainedIn).Returns(wornContainer.Object);
		parent.SetupGet(x => x.TrueLocations).Returns(Array.Empty<ICell>());
		var proto = CreateCellularPhoneProto(gameworld.Object, 2.0);
		var component = new CellularPhoneGameItemComponent(proto, parent.Object, true);

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
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Id).Returns(20L);
		parent.SetupGet(x => x.TrueLocations).Returns([]);
		var proto = CreateImplantTelephoneProto(gameworld.Object);
		var component = new ImplantTelephoneGameItemComponent(proto, parent.Object, true);
		var grid = new Mock<ITelecommunicationsGrid>();

		component.TelecommunicationsGrid = grid.Object;
		component.AssignPhoneNumber("5551200");
		component.OnPowerCutIn();

		Assert.IsFalse(component.CanDial(null!, "5551201", out var error));
		StringAssert.Contains(error, "no signal");
	}

	[TestMethod]
	public void ImplantTelephoneGameItemComponent_Transmit_ForwardsSpeechToConnectedPhone()
	{
		var gameworld = CreateGameworld();
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		parent.SetupGet(x => x.Id).Returns(21L);
		var proto = CreateImplantTelephoneProto(gameworld.Object);
		var component = new ImplantTelephoneGameItemComponent(proto, parent.Object, true);
		var call = new Mock<ITelephoneCall>();
		call.SetupGet(x => x.IsConnected).Returns(true);
		call.SetupGet(x => x.Participants).Returns([component]);

		component.ConnectCall(call.Object);
		component.OnPowerCutIn();
		component.Transmit(CreateSpokenLanguage(parent.Object));

		call.Verify(x => x.RelayTransmission(component, It.IsAny<SpokenLanguageInfo>()), Times.Once);
	}

	[TestMethod]
	public void TelecommunicationsGridCreatorComponent_RecreatesMissingGridOnLoad()
	{
		var gameworld = CreateGameworld();
		var parent = CreateBasicItem(gameworld.Object, 50L, CreateCell().Object);
		var proto = CreateTelecommunicationsGridCreatorProto(gameworld.Object, "555", 4);

		var component = new TelecommunicationsGridCreatorGameItemComponent(new MudSharp.Models.GameItemComponent
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
		var gameworld = CreateGameworld();
		var parent = CreateBasicItem(gameworld.Object, 51L, CreateCell().Object);
		var proto = CreateElectricGridCreatorProto(gameworld.Object);

		var component = new ElectricGridCreatorGameItemComponent(new MudSharp.Models.GameItemComponent
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
		var gameworld = CreateGameworld();
		var parent = CreateBasicItem(gameworld.Object, 52L, CreateCell().Object);
		var proto = CreateLiquidGridCreatorProto(gameworld.Object);

		var component = new LiquidGridCreatorGameItemComponent(new MudSharp.Models.GameItemComponent
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
		var gameworld = new Mock<IFuturemud>();
		var saveManager = new Mock<ISaveManager>();
		var bodyPrototypes = new Mock<IUneditableAll<IBodyPrototype>>();
		var heartbeatManager = new Mock<IHeartbeatManager>();
		var unitManager = new Mock<IUnitManager>();
		var items = CreateCollection(itemList ?? Enumerable.Empty<IGameItem>());
		var grids = CreateCollection(gridList ?? Enumerable.Empty<IGrid>());
		var languages = CreateCollection(languageList ?? Enumerable.Empty<ILanguage>());
		var accents = CreateCollection(accentList ?? Enumerable.Empty<IAccent>());
		unitManager.SetupGet(x => x.BaseFluidToLitres).Returns(1.0);
		unitManager.SetupGet(x => x.BaseWeightToKilograms).Returns(1.0);
		bodyPrototypes.SetupGet(x => x.Count).Returns(0);
		bodyPrototypes.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<IBodyPrototype>().GetEnumerator());
		bodyPrototypes.Setup(x => x.Get(It.IsAny<long>())).Returns((IBodyPrototype?)null);
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(bodyPrototypes.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.Items).Returns(items.Object);
		gameworld.SetupGet(x => x.Grids).Returns(grids.Object);
		gameworld.SetupGet(x => x.Languages).Returns(languages.Object);
		gameworld.SetupGet(x => x.Accents).Returns(accents.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
		         .Returns<long, bool>((id, _) => items.Object.Get(id));
		return gameworld;
	}

	private static Mock<IUneditableAll<T>> CreateCollection<T>(IEnumerable<T> items)
		where T : class, IFrameworkItem
	{
		var source = items.ToList();
		var collection = new Mock<IUneditableAll<T>>();
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
			          if (long.TryParse(value, out var id))
			          {
				          return source.FirstOrDefault(x => x.Id == id);
			          }

			          return source.FirstOrDefault(x => x.Name == value);
		          });
		collection.Setup(x => x.ForEach(It.IsAny<Action<T>>()))
		          .Callback<Action<T>>(action =>
		          {
			          foreach (var item in source)
			          {
				          action(item);
			          }
		          });
		return collection;
	}

	private static Mock<ICell> CreateCell(long id = 1)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		return cell;
	}

	private static Mock<IGameItem> CreateBasicItem(IFuturemud gameworld, long id, params ICell[] trueLocations)
	{
		var item = new Mock<IGameItem>();
		var outputHandler = new Mock<IOutputHandler>();
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
		var liquid = new Mock<ILiquid>();
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
		var mixture = LiquidMixture.CreateEmpty(gameworld);
		foreach (var (liquid, amount) in contents)
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
		var state = new SupplierState { Mixture = mixture };
		var supplier = new Mock<ILiquidGridSupplier>();
		supplier.SetupGet(x => x.SuppliedMixture).Returns(() => state.Mixture);
		supplier.SetupGet(x => x.AvailableLiquidVolume).Returns(() => state.Mixture.TotalVolume);
		supplier.Setup(x => x.RemoveLiquidAmount(It.IsAny<double>(), It.IsAny<ICharacter?>(), It.IsAny<string>()))
		        .Returns((double amount, ICharacter? _, string _) =>
		        {
			        var removed = state.Mixture.RemoveLiquidVolume(amount);
			        return removed;
		        });
		return new SupplierDouble(supplier, state);
	}

	private static GridPowerSupplyGameItemComponentProto CreateGridPowerSupplyProto(IFuturemud gameworld, double wattage)
	{
		var model = new MudSharp.Models.GameItemComponentProto
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
		var model = new MudSharp.Models.GameItemComponentProto
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
		var model = new MudSharp.Models.GameItemComponentProto
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

	private static TapeGameItemComponentProto CreateTapeProto(IFuturemud gameworld, double capacityMinutes = 30.0)
	{
		var model = new MudSharp.Models.GameItemComponentProto
		{
			Id = 24,
			Name = "Tape",
			Description = "Test",
			RevisionNumber = 1,
			Definition = $"<Definition><CapacityMs>{(long)TimeSpan.FromMinutes(capacityMinutes).TotalMilliseconds}</CapacityMs></Definition>",
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionStatus = (int)RevisionStatus.Current,
				RevisionNumber = 1
			}
		};

		return (TapeGameItemComponentProto)typeof(TapeGameItemComponentProto)
		       .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
			       [typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
		       .Invoke([model, gameworld]);
	}

	private static AnsweringMachineGameItemComponentProto CreateAnsweringMachineProto(IFuturemud gameworld,
		int defaultAutoAnswerRings, double wattage = 6.0)
	{
		var model = new MudSharp.Models.GameItemComponentProto
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

	private static (Mock<IGameItem> Item, TapeGameItemComponent Component) CreateTape(IFuturemud gameworld, long id,
		double capacityMinutes = 30.0)
	{
		var item = CreateBasicItem(gameworld, id);
		item.SetupProperty(x => x.ContainedIn);
		item.Setup(x => x.LoadTimeSetContainedIn(It.IsAny<IGameItem>()))
		    .Callback<IGameItem>(parent => item.Object.ContainedIn = parent);
		item.Setup(x => x.FinaliseLoadTimeTasks());
		item.Setup(x => x.Get(It.IsAny<IBody>())).Returns(item.Object);

		var component = new TapeGameItemComponent(CreateTapeProto(gameworld, capacityMinutes), item.Object, true);
		item.Setup(x => x.GetItemType<IAudioStorageTape>()).Returns(component);
		return (item, component);
	}

	private static TelecommunicationsGridCreatorGameItemComponentProto CreateTelecommunicationsGridCreatorProto(
		IFuturemud gameworld, string prefix, int numberLength)
	{
		var model = new MudSharp.Models.GameItemComponentProto
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
		var model = new MudSharp.Models.GameItemComponentProto
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
		var model = new MudSharp.Models.GameItemComponentProto
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
		var model = new MudSharp.Models.GameItemComponentProto
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

	private static Mock<ITelephoneNumberOwner> CreateLineOwner(IFuturemud gameworld, long parentId, long componentId,
		params ITelephone[] phones)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(parentId);
		parent.SetupGet(x => x.Name).Returns($"Item {parentId}");
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		var owner = new Mock<ITelephoneNumberOwner>();
		var phoneNumber = default(string);
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
		var model = new MudSharp.Models.GameItemComponentProto
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

	private static SpokenLanguageInfo CreateSpokenLanguage(IPerceivable origin)
	{
		var model = new Mock<ILanguageDifficultyModel>();
		model.Setup(x => x.RateDifficulty(It.IsAny<ExplodedString>())).Returns(Difficulty.Automatic);
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(1L);
		language.SetupGet(x => x.Model).Returns(model.Object);
		language.SetupGet(x => x.Name).Returns("Common");
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Id).Returns(2L);
		accent.SetupGet(x => x.Language).Returns(language.Object);
		return new SpokenLanguageInfo(language.Object, accent.Object, AudioVolume.Decent, "Hello there",
			Outcome.Pass, origin, origin);
	}

	private static Mock<ILanguage> CreateLanguage(long id, string name = "Common")
	{
		var model = new Mock<ILanguageDifficultyModel>();
		model.Setup(x => x.RateDifficulty(It.IsAny<ExplodedString>())).Returns(Difficulty.Automatic);
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Id).Returns(id);
		language.SetupGet(x => x.Name).Returns(name);
		language.SetupGet(x => x.Model).Returns(model.Object);
		return language;
	}

	private static Mock<IAccent> CreateAccent(long id, ILanguage language, string name = "Standard")
	{
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Id).Returns(id);
		accent.SetupGet(x => x.Name).Returns(name);
		accent.SetupGet(x => x.Language).Returns(language);
		accent.SetupGet(x => x.AccentSuffix).Returns(name);
		return accent;
	}

	private static Mock<ICharacter> CreateCharacter(IFuturemud gameworld, long id, ICell location)
	{
		var character = new Mock<ICharacter>();
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
			var parent = new Mock<IGameItem>();
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

			var normalised = new string((digits ?? string.Empty).Where(x => !char.IsWhiteSpace(x)).ToArray());
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

			var normalised = new string((digits ?? string.Empty).Where(x => !char.IsWhiteSpace(x)).ToArray());
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

			var existingCall = _currentCall;
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
		public bool HandleEvent(EventType type, params dynamic[] arguments) => false;
		public bool HandlesEvent(params EventType[] types) => false;
		public bool InstallHook(IHook hook) => false;
		public bool RemoveHook(IHook hook) => false;
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
		public double ComponentBuoyancy(double fluidDensity) => 0.0;
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

		public bool HandleDieOrMorph(IGameItem newItem, ICell location) => false;
		public bool SwapInPlace(IGameItem existingItem, IGameItem newItem) => false;
		public bool Take(IGameItem item) => false;
		public IGameItemComponent Copy(IGameItem newParent, bool temporary = false) => this;
		public bool DescriptionDecorator(DescriptionType type) => false;
		public string Decorate(IPerceiver voyeur, string name, string description, DescriptionType type, bool colour,
			PerceiveIgnoreFlags flags) => description;
		public event EventHandler? DescriptionUpdate;
		public bool PreventsMerging(IGameItemComponent component) => false;
		public bool PreventsRepositioning() => false;
		public bool PreventsMovement() => false;
		public string WhyPreventsMovement(ICharacter mover) => string.Empty;
		public void ForceMove()
		{
		}

		public string WhyPreventsRepositioning() => string.Empty;
		public bool CheckPrototypeForUpdate() => false;
		public void FinaliseLoad()
		{
		}

		public void Taken()
		{
		}

		public bool CanBePositionedAgainst(IPositionState state, PositionModifier modifier) => true;
		public IBody HaveABody => null!;
		public bool WarnBeforePurge => false;
		public bool ExposeToLiquid(LiquidMixture mixture) => false;
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
			foreach (var fax in _pendingFaxes.ToList())
			{
				var requiredInk = fax.Sum(x => x.DocumentLength);
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
		var readable = new Mock<ICanBeRead>();
		readable.SetupGet(x => x.DocumentLength).Returns(length);
		readable.SetupGet(x => x.Id).Returns(length);
		readable.SetupGet(x => x.Name).Returns($"Document {length}");
		readable.SetupGet(x => x.FrameworkItemType).Returns("Readable");
		readable.SetupGet(x => x.Author).Returns((ICharacter)null!);
		readable.SetupGet(x => x.ImplementType).Returns(WritingImplementType.Biro);
		return readable.Object;
	}
}
