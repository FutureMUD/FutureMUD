#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Position;
using MudSharp.Character;
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
	public void TelephoneNumber_FollowsAssignedEndpointRatherThanHandset()
	{
		var gameworld = CreateGameworld();
		var grid = new TelecommunicationsGrid(gameworld.Object, CreateCell().Object, "555", 4);
		var phone = new TelephoneDouble(gameworld.Object, 10);
		var lineA = CreateLineOwner(gameworld.Object, 11, phone);
		var lineB = CreateLineOwner(gameworld.Object, 12, phone);

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

	private static Mock<IFuturemud> CreateGameworld()
	{
		var gameworld = new Mock<IFuturemud>();
		var saveManager = new Mock<ISaveManager>();
		var bodyPrototypes = new Mock<IUneditableAll<IBodyPrototype>>();
		var heartbeatManager = new Mock<IHeartbeatManager>();
		var items = new Mock<IUneditableAll<IGameItem>>();
		var unitManager = new Mock<IUnitManager>();
		unitManager.SetupGet(x => x.BaseFluidToLitres).Returns(1.0);
		unitManager.SetupGet(x => x.BaseWeightToKilograms).Returns(1.0);
		bodyPrototypes.SetupGet(x => x.Count).Returns(0);
		bodyPrototypes.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<IBodyPrototype>().GetEnumerator());
		bodyPrototypes.Setup(x => x.Get(It.IsAny<long>())).Returns((IBodyPrototype?)null);
		items.SetupGet(x => x.Count).Returns(0);
		items.Setup(x => x.GetEnumerator()).Returns(Enumerable.Empty<IGameItem>().GetEnumerator());
		gameworld.SetupGet(x => x.BodyPrototypes).Returns(bodyPrototypes.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.Items).Returns(items.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		return gameworld;
	}

	private static Mock<ICell> CreateCell(long id = 1)
	{
		var cell = new Mock<ICell>();
		cell.SetupGet(x => x.Id).Returns(id);
		return cell;
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

	private static Mock<ITelephoneNumberOwner> CreateLineOwner(IFuturemud gameworld, long id, params ITelephone[] phones)
	{
		var parent = new Mock<IGameItem>();
		parent.SetupGet(x => x.Id).Returns(id);
		parent.SetupGet(x => x.Gameworld).Returns(gameworld);
		var owner = new Mock<ITelephoneNumberOwner>();
		var phoneNumber = default(string);
		owner.SetupGet(x => x.Parent).Returns(parent.Object);
		owner.SetupProperty(x => x.PreferredNumber);
		owner.SetupProperty(x => x.AllowSharedNumber);
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
		language.SetupGet(x => x.Model).Returns(model.Object);
		language.SetupGet(x => x.Name).Returns("Common");
		var accent = new Mock<IAccent>();
		return new SpokenLanguageInfo(language.Object, accent.Object, AudioVolume.Decent, "Hello there",
			Outcome.Pass, origin, origin);
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

	private sealed class TelephoneDouble : ITelephone, ITelephoneNumberOwner
	{
		private readonly IGameItem _parent;
		private readonly IGameItemComponentProto _prototype;
		private string? _phoneNumber;
		private string? _preferredNumber;
		private bool _allowSharedNumber;
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
		public bool IsPowered { get; set; }
		public bool IsOffHook => _isOffHook;
		public bool CanReceiveCalls =>
			SwitchedOn && IsPowered && TelecommunicationsGrid != null && !string.IsNullOrWhiteSpace(PhoneNumber) &&
			!_isOffHook && _currentCall == null;
		public bool IsRinging => _isRinging;
		public bool IsConnected => _currentCall?.Participants.Contains(this) == true && _currentCall.IsConnected;
		public bool IsEngaged => _currentCall != null || _isOffHook;
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
			if (!CanDial(actor, number, out error))
			{
				return false;
			}

			_isOffHook = true;
			return TelecommunicationsGrid!.TryStartCall(this, number, out error);
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
}
