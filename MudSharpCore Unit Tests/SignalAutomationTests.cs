#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
using MudSharp.Framework.Units;
using MudSharp.Form.Shape;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Inventory;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.RPG.Checks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SignalAutomationTests
{
	[ClassInitialize]
	public static void ClassInitialise(TestContext _)
	{
		FutureProgTestBootstrap.EnsureInitialised();
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_CompilesExecutableLogic()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"TestMicrocontroller",
			new[] { "Signal1", "ToggleValue" },
			@"if (@signal1 == 0)
	return 0
end if
return @togglevalue");

		Assert.IsNotNull(prog, error);
		Assert.AreEqual(string.Empty, error);
		Assert.AreEqual(0.0m, prog.ExecuteDecimal(0.0m, 0.0m, 7.5m));
		Assert.AreEqual(7.5m, prog.ExecuteDecimal(0.0m, 1.0m, 7.5m));
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_RejectsDuplicateVariableNamesIgnoringCase()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"DuplicateInputs",
			new[] { "Signal1", "signal1" },
			"return 0");

		Assert.IsNull(prog);
		StringAssert.Contains(error, "duplicate");
	}

	[TestMethod]
	public void MicrocontrollerLogicCompiler_RejectsInvalidVariableNames()
	{
		var (prog, error) = MicrocontrollerLogicCompiler.Compile(
			FutureProgTestBootstrap.Gameworld,
			"InvalidInputs",
			new[] { "1badname" },
			"return 0");

		Assert.IsNull(prog);
		StringAssert.Contains(error, "not valid");
	}

	[TestMethod]
	public void GameItemComponentManager_RegistersSignalAutomationComponentTypes()
	{
		var manager = new GameItemComponentManager();
		var primaryTypes = manager.PrimaryTypes.ToList();
		var helpTypes = manager.TypeHelpInfo.Select(x => x.Name).ToList();

		CollectionAssert.IsSubsetOf(
			new[]
			{
				"pushbutton", "toggleswitch", "motionsensor", "timersensor", "microcontroller", "signallight",
				"electroniclock", "electronicdoor", "alarmsiren", "automationmounthost", "signalcable",
				"lightsensor", "rainsensor", "temperaturesensor", "keypad", "relayswitch",
				"filesignalgenerator",
				"automationhousing"
			},
			primaryTypes);
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"PushButton", "ToggleSwitch", "MotionSensor", "TimerSensor", "Microcontroller", "SignalLight",
				"ElectronicLock", "ElectronicDoor", "AlarmSiren", "Automation Mount Host", "Signal Cable Segment",
				"LightSensor", "RainSensor", "TemperatureSensor", "Keypad", "RelaySwitch",
				"FileSignalGenerator",
				"Automation Housing"
			},
			helpTypes);
	}

	[TestMethod]
	public void FileSignalGenerator_UpdatesSignalWhenBackingFileChanges()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell(9060L);
		var item = CreateBasicItem(gameworld.Object, 9061L, "Signal Controller", cell.Object);
		var proto = CreateFileSignalGeneratorProto(gameworld.Object);
		var component = new FileSignalGeneratorGameItemComponent(proto, item.Object, true);

		component.SwitchedOn = true;
		component.OnPowerCutIn();

		Assert.AreEqual(0.0, component.CurrentValue, 0.001);
		component.FileSystem!.WriteFile(component.SignalFileName, "7.5");
		Assert.AreEqual(7.5, component.CurrentValue, 0.001);
		Assert.IsTrue(component.FileValueValid);

		component.FileSystem.WriteFile(component.SignalFileName, "invalid");
		Assert.AreEqual(0.0, component.CurrentValue, 0.001);
		Assert.IsFalse(component.FileValueValid);
	}

	[TestMethod]
	public void ComputerFileTransferUtilities_EnumerateOwners_IncludesHostLocalFileSignalGenerator()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell(9070L);
		var hostItem = CreateBasicItem(gameworld.Object, 9071L, "Network Appliance", cell.Object);
		IGameItemComponent[] components = [];
		hostItem.SetupGet(x => x.Components).Returns(() => components);

		var host = new ComputerHostGameItemComponent(CreateComputerHostProto(gameworld.Object), hostItem.Object, true);
		var fileGenerator = new FileSignalGeneratorGameItemComponent(CreateFileSignalGeneratorProto(gameworld.Object),
			hostItem.Object,
			true);
		components = [host, fileGenerator];

		var owners = ComputerFileTransferUtilities.EnumerateOwners(host).ToList();

		Assert.IsTrue(owners.Contains(host));
		Assert.IsTrue(owners.Contains(fileGenerator));
		Assert.IsTrue(owners.Any(x => x.FileOwnerId == fileGenerator.Id));
	}

	[TestMethod]
	public void MotionSensorDetectionMode_MatchesExpectedWitnessEvents()
	{
		Assert.IsTrue(MotionSensorDetectionMode.AnyMovement.MatchesEventType(EventType.CharacterBeginMovementWitness));
		Assert.IsTrue(MotionSensorDetectionMode.AnyMovement.MatchesEventType(EventType.CharacterEnterCellWitness));
		Assert.IsTrue(MotionSensorDetectionMode.AnyMovement.MatchesEventType(EventType.CharacterStopMovementWitness));
		Assert.IsTrue(MotionSensorDetectionMode.AnyMovement.MatchesEventType(EventType.CharacterStopMovementClosedDoorWitness));
		Assert.IsFalse(MotionSensorDetectionMode.AnyMovement.MatchesEventType(EventType.CharacterEnterCellItems));
		Assert.IsTrue(MotionSensorDetectionMode.BeginMovement.MatchesEventType(EventType.CharacterBeginMovementWitness));
		Assert.IsFalse(MotionSensorDetectionMode.BeginMovement.MatchesEventType(EventType.CharacterEnterCellWitness));
		Assert.IsTrue(MotionSensorDetectionMode.EnterCell.MatchesEventType(EventType.CharacterEnterCellWitness));
		Assert.IsFalse(MotionSensorDetectionMode.EnterCell.MatchesEventType(EventType.CharacterBeginMovementWitness));
		Assert.IsTrue(MotionSensorDetectionMode.StopMovement.MatchesEventType(EventType.CharacterStopMovementWitness));
		Assert.IsTrue(MotionSensorDetectionMode.StopMovement.MatchesEventType(EventType.CharacterStopMovementClosedDoorWitness));
		Assert.IsFalse(MotionSensorDetectionMode.StopMovement.MatchesEventType(EventType.CharacterEnterCellWitness));
	}

	[TestMethod]
	public void TimerSensorCycleScheduler_ResolvesActiveAnchorsAcrossCycles()
	{
		var anchor = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var activeDuration = TimeSpan.FromSeconds(10);
		var inactiveDuration = TimeSpan.FromSeconds(20);

		var activeState = TimerSensorCycleScheduler.Resolve(anchor, true, activeDuration, inactiveDuration,
			anchor.AddSeconds(5));
		Assert.IsTrue(activeState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(10), activeState.NextTransition);

		var inactiveState = TimerSensorCycleScheduler.Resolve(anchor, true, activeDuration, inactiveDuration,
			anchor.AddSeconds(10));
		Assert.IsFalse(inactiveState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(30), inactiveState.NextTransition);

		var nextCycleState = TimerSensorCycleScheduler.Resolve(anchor, true, activeDuration, inactiveDuration,
			anchor.AddSeconds(35));
		Assert.IsTrue(nextCycleState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(40), nextCycleState.NextTransition);
	}

	[TestMethod]
	public void TimerSensorCycleScheduler_ResolvesInactiveAnchorsAcrossCycles()
	{
		var anchor = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		var activeDuration = TimeSpan.FromSeconds(15);
		var inactiveDuration = TimeSpan.FromSeconds(45);

		var inactiveState = TimerSensorCycleScheduler.Resolve(anchor, false, activeDuration, inactiveDuration,
			anchor.AddSeconds(30));
		Assert.IsFalse(inactiveState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(45), inactiveState.NextTransition);

		var activeState = TimerSensorCycleScheduler.Resolve(anchor, false, activeDuration, inactiveDuration,
			anchor.AddSeconds(50));
		Assert.IsTrue(activeState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(60), activeState.NextTransition);

		var wrappedState = TimerSensorCycleScheduler.Resolve(anchor, false, activeDuration, inactiveDuration,
			anchor.AddSeconds(120));
		Assert.IsFalse(wrappedState.IsActive);
		Assert.AreEqual(anchor.AddSeconds(165), wrappedState.NextTransition);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_UsesStableIdentifierAcrossRenames()
	{
		var renamedSource = CreateSignalSourceMock(41L, "RenamedButton");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { renamedSource.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 41L, "OriginalButton",
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);

		Assert.AreSame(renamedSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_CreateBinding_UsesRuntimeComponentIdAndNormalisedEndpointKey()
	{
		var source = CreateSignalSourceMock(41L, "SignalSource", "AuX", 208L);

		var binding = SignalComponentUtilities.CreateBinding(source.Object, "  AUX  ");

		Assert.AreEqual(208L, binding.SourceComponentId);
		Assert.AreEqual("SignalSource", binding.SourceComponentName);
		Assert.AreEqual("aux", binding.SourceEndpointKey);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_ResolvesRuntimeComponentIdBeforePrototypeIdentifier()
	{
		var runtimeSource = CreateSignalSourceMock(41L, "SignalSource", componentId: 208L);
		var prototypeSource = CreateSignalSourceMock(208L, "OtherSource", componentId: 41L);
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { runtimeSource.Object, prototypeSource.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 208L, "SignalSource",
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);

		Assert.AreSame(runtimeSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_ResolvesCorrectSourceWhenNamesOverlap()
	{
		var firstSource = CreateSignalSourceMock(41L, "SignalSource");
		var secondSource = CreateSignalSourceMock(99L, "SignalSource");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { firstSource.Object, secondSource.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 99L, "SignalSource",
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);

		Assert.AreSame(secondSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_UsesEndpointKeyAsPartOfStableIdentity()
	{
		var firstSource = CreateSignalSourceMock(41L, "SignalSource", "signal");
		var secondSource = CreateSignalSourceMock(41L, "SignalSource", "aux");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { firstSource.Object, secondSource.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 41L, "SignalSource", "aux");

		Assert.AreSame(secondSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_ReturnsNullWhenEndpointKeyDoesNotMatch()
	{
		var source = CreateSignalSourceMock(41L, "SignalSource", "aux");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { source.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 41L, "SignalSource",
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);

		Assert.IsNull(resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_FallsBackToLegacyNameWhenIdentifierMissing()
	{
		var source = CreateSignalSourceMock(41L, "LegacyButton");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { source.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 0L, "LegacyButton",
			SignalComponentUtilities.DefaultLocalSignalEndpointKey);

		Assert.AreSame(source.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_UsesMountedHostAnchorForRoomLocalSources()
	{
		var gameworld = CreateGameworld();
		var sharedCell = CreateCell(150L);
		var hostItem = CreateBasicItem(gameworld.Object, 151L, "Electronic Door", sharedCell.Object);
		var moduleItem = CreateBasicItem(gameworld.Object, 152L, "Airlock Controller Module");
		var sourceItem = CreateBasicItem(gameworld.Object, 153L, "Outside Motion Sensor", sharedCell.Object);
		var source = CreateSignalSourceMock(154L, "Door Outside Motion Sensor", parent: sourceItem.Object,
			componentId: 155L,
			signal: new ComputerSignal(1.0, TimeSpan.FromSeconds(10), null));
		var host = new Mock<IAutomationMountHost>();
		var mountable = new Mock<IAutomationMountable>();

		host.SetupGet(x => x.Parent).Returns(hostItem.Object);
		mountable.SetupGet(x => x.MountHost).Returns(host.Object);
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(mountable.Object);
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);
		gameworld.Setup(x => x.TryGetItem(sourceItem.Object.Id, true)).Returns(sourceItem.Object);

		var resolved = SignalComponentUtilities.FindSignalSource(
			moduleItem.Object,
			new LocalSignalBinding(
				sourceItem.Object.Id,
				sourceItem.Object.Name,
				155L,
				"Door Outside Motion Sensor",
				SignalComponentUtilities.DefaultLocalSignalEndpointKey));

		Assert.AreSame(source.Object, resolved);
	}

	[TestMethod]
	public void ElectronicDoorControlEvaluator_Evaluate_RequestsOpenWhenClosedDoorCanOpen()
	{
		var outcome = ElectronicDoorControlEvaluator.Evaluate(true, false, true, false);

		Assert.AreEqual(ElectronicDoorControlAction.Open, outcome.Action);
		Assert.IsFalse(outcome.RequiresRetry);
	}

	[TestMethod]
	public void ElectronicDoorControlEvaluator_Evaluate_RequestsRetryWhenOpeningIsBlocked()
	{
		var outcome = ElectronicDoorControlEvaluator.Evaluate(true, false, false, false);

		Assert.AreEqual(ElectronicDoorControlAction.None, outcome.Action);
		Assert.IsTrue(outcome.RequiresRetry);
	}

	[TestMethod]
	public void ElectronicDoorControlEvaluator_Evaluate_RequestsCloseWhenOpenDoorCanClose()
	{
		var outcome = ElectronicDoorControlEvaluator.Evaluate(false, true, false, true);

		Assert.AreEqual(ElectronicDoorControlAction.Close, outcome.Action);
		Assert.IsFalse(outcome.RequiresRetry);
	}

	[TestMethod]
	public void AutomationMountHost_InstallAndRemoveModule_TracksMountedSeparateItem()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(10L);
		var hostItem = CreateBasicItem(gameworld.Object, 100L, "Automation Cabinet", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostItem.Setup(x => x.GetItemType<IAutomationMountHost>()).Returns(host);
		hostComponents = [host];

		var moduleLocation = CreateCell(11L);
		var moduleItem = CreateBasicItem(gameworld.Object, 200L, "Loose Controller", moduleLocation.Object);
		var module = new Mock<IAutomationMountable>();
		var connectable = module.As<IConnectable>();
		module.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		module.SetupGet(x => x.MountType).Returns("Microcontroller");
		module.SetupGet(x => x.IsMounted).Returns(false);
		connectable.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		connectable.Setup(x => x.RawConnect(It.IsAny<IConnectable>(), It.IsAny<ConnectorType>()));
		connectable.Setup(x => x.RawDisconnect(It.IsAny<IConnectable>(), It.IsAny<bool>()));

		Assert.IsTrue(host.InstallModule(null!, module.Object, "controller", out var error), error);
		Assert.AreEqual(string.Empty, error);
		Assert.IsTrue(host.Bays.Single().Occupied);
		Assert.AreSame(moduleItem.Object, host.Bays.Single().MountedItem);
		moduleLocation.Verify(x => x.Extract(moduleItem.Object), Times.Once);
		connectable.Verify(x => x.RawConnect(host, It.Is<ConnectorType>(y =>
			y.ConnectionType.Equals("Automation:Microcontroller", StringComparison.InvariantCultureIgnoreCase))), Times.Once);

		Assert.IsTrue(host.RemoveModule(null!, "controller", out var removedItem, out error), error);
		Assert.AreEqual(string.Empty, error);
		Assert.AreSame(moduleItem.Object, removedItem);
		Assert.IsFalse(host.Bays.Single().Occupied);
		hostLocation.Verify(x => x.Insert(moduleItem.Object), Times.Once);
		connectable.Verify(x => x.RawDisconnect(host, false), Times.Once);
	}

	[TestMethod]
	public void AutomationMountHost_CanAccessMounts_RequiresOpenSiblingPanel()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(12L);
		var hostItem = CreateBasicItem(gameworld.Object, 110L, "Sealed Cabinet", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")], 501L,
				"Maintenance Housing"),
			hostItem.Object,
			true);
		var housing = new AutomationHousingGameItemComponent(
			CreateAutomationHousingProto(gameworld.Object, 501L, "Maintenance Housing"),
			hostItem.Object,
			true);
		housing.Close();
		hostComponents = [host, housing];
		hostItem.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns("sealed cabinet");

		var actor = new Mock<ICharacter>();

		Assert.IsFalse(host.CanAccessMounts(actor.Object, out var error));
		StringAssert.Contains(error, "open");
	}

	[TestMethod]
	public void AutomationHousing_CanAccessHousing_RequiresOpenableParentOpen()
	{
		var gameworld = CreateGameworld();
		var housingItem = CreateBasicItem(gameworld.Object, 120L, "Cable Junction");
		var housing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object),
			housingItem.Object,
			true);
		housing.Close();
		var actor = new Mock<ICharacter>();

		Assert.IsFalse(housing.CanAccessHousing(actor.Object, out var error));
		StringAssert.Contains(error, "open");
	}

	[TestMethod]
	public void AutomationHousing_CanConcealItem_AcceptsCableModuleAndSignalItems()
	{
		var gameworld = CreateGameworld();
		var housingItem = CreateBasicItem(gameworld.Object, 121L, "Service Housing");
		var housing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object),
			housingItem.Object,
			true);

		var cableItem = CreateBasicItem(gameworld.Object, 122L, "Cable Segment");
		cableItem.Setup(x => x.GetItemType<ISignalCableSegment>()).Returns(Mock.Of<ISignalCableSegment>());
		Assert.IsTrue(housing.CanConcealItem(cableItem.Object, out var cableError), cableError);

		var moduleItem = CreateBasicItem(gameworld.Object, 123L, "Control Module");
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(Mock.Of<IAutomationMountable>());
		Assert.IsTrue(housing.CanConcealItem(moduleItem.Object, out var moduleError), moduleError);

		var signalSource = CreateSignalSourceMock(77L, "Indicator");
		var signalItem = CreateBasicItem(gameworld.Object, 124L, "Signal Widget");
		signalItem.SetupGet(x => x.Components).Returns(new IGameItemComponent[] { signalSource.Object });
		Assert.IsTrue(housing.CanConcealItem(signalItem.Object, out var signalError), signalError);

		var mundaneItem = CreateBasicItem(gameworld.Object, 125L, "Spanner");
		Assert.IsFalse(housing.CanConcealItem(mundaneItem.Object, out var mundaneError));
		StringAssert.Contains(mundaneError, "automation");
	}

	[TestMethod]
	public void AutomationHousing_ConcealedItems_FiltersContainerContentsToSupportedAutomationItems()
	{
		var gameworld = CreateGameworld();
		var cableItem = CreateBasicItem(gameworld.Object, 126L, "Cable Segment");
		cableItem.Setup(x => x.GetItemType<ISignalCableSegment>()).Returns(Mock.Of<ISignalCableSegment>());

		var signalSource = CreateSignalSourceMock(78L, "Indicator");
		var signalItem = CreateBasicItem(gameworld.Object, 127L, "Signal Widget");
		signalItem.SetupGet(x => x.Components).Returns(new IGameItemComponent[] { signalSource.Object });

		var mundaneItem = CreateBasicItem(gameworld.Object, 128L, "Loose Bolt");
		var concealedItems = new[] { cableItem.Object, signalItem.Object, mundaneItem.Object };

		var housingItem = CreateBasicItem(gameworld.Object, 129L, "Service Housing");
		var housing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object),
			housingItem.Object,
			true);
		AddHousingContents(housing, concealedItems);

		var concealed = housing.ConcealedItems.ToList();

		CollectionAssert.AreEquivalent(new[] { cableItem.Object, signalItem.Object }, concealed);
	}

	[TestMethod]
	public void Microcontroller_UsesMountedHostPowerWhenConfigured()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(40L);
		var hostItem = CreateBasicItem(gameworld.Object, 400L, "Automation Cabinet", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);
		var hostPower = new Mock<IProducePower>();
		hostPower.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		hostPower.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		hostPower.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		hostPower.SetupGet(x => x.ProducingPower).Returns(true);
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns(hostPower.Object);
		hostItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([hostPower.Object]);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleLocation = CreateCell(41L);
		var moduleItem = CreateBasicItem(gameworld.Object, 401L, "Mounted Controller", moduleLocation.Object);
		var localPower = new Mock<IProducePower>();
		moduleItem.Setup(x => x.GetItemType<IProducePower>()).Returns(localPower.Object);
		moduleItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([localPower.Object]);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);

		var method = typeof(PoweredMachineBaseGameItemComponent)
			.GetMethod("ResolvePowerSource", BindingFlags.Instance | BindingFlags.NonPublic);
		var resolved = (IProducePower?)method!.Invoke(controller, []);

		Assert.AreSame(hostPower.Object, resolved);
		Assert.AreNotSame(localPower.Object, resolved);
	}

	[TestMethod]
	public void Microcontroller_UsesAttachedHostPowerProducerWhenMounted()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(42L);
		var hostItem = CreateBasicItem(gameworld.Object, 402L, "Security Door", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns((IProducePower)null!);
		hostItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns(Array.Empty<IProducePower>());

		var attachedGeneratorItem = CreateBasicItem(gameworld.Object, 403L, "Unlimited Generator", hostLocation.Object);
		var attachedHostPower = new Mock<IProducePower>();
		attachedHostPower.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		attachedHostPower.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		attachedHostPower.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		attachedHostPower.SetupGet(x => x.ProducingPower).Returns(true);
		attachedHostPower.Setup(x => x.CanBeginDrawDown(It.IsAny<double>())).Returns(true);
		attachedGeneratorItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([attachedHostPower.Object]);
		attachedGeneratorItem.Setup(x => x.GetItemType<IProducePower>()).Returns(attachedHostPower.Object);
		hostItem.Setup(x => x.AttachedAndConnectedItems).Returns([attachedGeneratorItem.Object]);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleLocation = CreateCell(43L);
		var moduleItem = CreateBasicItem(gameworld.Object, 404L, "Mounted Controller", moduleLocation.Object);
		var localPower = new Mock<IProducePower>();
		moduleItem.Setup(x => x.GetItemType<IProducePower>()).Returns(localPower.Object);
		moduleItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([localPower.Object]);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);
		controller.SwitchedOn = true;
		attachedHostPower.Verify(x => x.BeginDrawdown(controller), Times.Never);

		controller.Login();

		attachedHostPower.Verify(x => x.BeginDrawdown(controller), Times.Once);
		localPower.Verify(x => x.BeginDrawdown(controller), Times.Never);
	}

	[TestMethod]
	public void PoweredMachineBase_DoesNotBeginDrawdownBeforeLogin()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 4041L, "Mounted Controller");
		var producer = new Mock<IProducePower>();
		producer.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		producer.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		producer.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		producer.SetupGet(x => x.ProducingPower).Returns(true);
		producer.Setup(x => x.CanBeginDrawDown(It.IsAny<double>())).Returns(true);
		item.Setup(x => x.GetItemType<IProducePower>()).Returns(producer.Object);
		item.Setup(x => x.GetItemTypes<IProducePower>()).Returns([producer.Object]);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			item.Object,
			true);
		controller.SwitchedOn = true;

		producer.Verify(x => x.BeginDrawdown(controller), Times.Never);

		controller.Login();

		producer.Verify(x => x.BeginDrawdown(controller), Times.Once);
	}

	[TestMethod]
	public void Microcontroller_RestoresMountHostFromPendingHostIdForPowerResolution()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(44L);
		var hostItem = CreateBasicItem(gameworld.Object, 405L, "Security Door", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);
		var hostPower = new Mock<IProducePower>();
		hostPower.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		hostPower.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		hostPower.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		hostPower.SetupGet(x => x.ProducingPower).Returns(true);
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns(hostPower.Object);
		hostItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([hostPower.Object]);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];
		hostItem.Setup(x => x.GetItemType<IAutomationMountHost>()).Returns(host);
		hostItem.Setup(x => x.GetItemTypes<IAutomationMountHost>()).Returns([host]);
		hostItem.Setup(x => x.GetItemType<IConnectable>()).Returns(host);
		hostItem.Setup(x => x.GetItemTypes<IConnectable>()).Returns([host]);
		gameworld.Setup(x => x.TryGetItem(hostItem.Object.Id, true)).Returns(hostItem.Object);

		var moduleLocation = CreateCell(45L);
		var moduleItem = CreateBasicItem(gameworld.Object, 406L, "Mounted Controller", moduleLocation.Object);
		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);

		typeof(MicrocontrollerGameItemComponent)
			.GetField("_mountedHost", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(controller, null);
		typeof(MicrocontrollerGameItemComponent)
			.GetField("_pendingMountedHostId", BindingFlags.Instance | BindingFlags.NonPublic)!
			.SetValue(controller, hostItem.Object.Id);

		Assert.IsTrue(controller.IsMounted);
		Assert.AreSame(host, controller.MountHost);

		var method = typeof(PoweredMachineBaseGameItemComponent)
			.GetMethod("ResolvePowerSource", BindingFlags.Instance | BindingFlags.NonPublic);
		var resolved = (IProducePower?)method!.Invoke(controller, []);
		Assert.AreSame(hostPower.Object, resolved);
	}

	[TestMethod]
	public void Microcontroller_Login_ReconnectsSourcesAndSeedsCurrentValues()
	{
		var gameworld = CreateGameworld();
		var sharedCell = CreateCell(46L);
		var hostItem = CreateBasicItem(gameworld.Object, 407L, "Security Door", sharedCell.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);
		var hostPower = new Mock<IProducePower>();
		hostPower.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		hostPower.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		hostPower.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		hostPower.SetupGet(x => x.ProducingPower).Returns(true);
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns(hostPower.Object);
		hostItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([hostPower.Object]);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleItem = CreateBasicItem(gameworld.Object, 408L, "Airlock Controller Module");
		var sourceItem = CreateBasicItem(gameworld.Object, 409L, "Outside Motion Sensor", sharedCell.Object);
		var currentSignal = new ComputerSignal(1.0, TimeSpan.FromSeconds(10), null);
		var source = CreateSignalSourceMock(410L, "Door Outside Motion Sensor", parent: sourceItem.Object,
			componentId: 411L, signal: currentSignal);
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);
		sourceItem.SetupGet(x => x.Components).Returns([source.Object]);
		sharedCell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([hostItem.Object, sourceItem.Object]);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(controller);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);
		Assert.IsTrue(controller.SetInputBinding("outside", source.Object,
			SignalComponentUtilities.DefaultLocalSignalEndpointKey, out error), error);
		Assert.IsTrue(controller.SetLogicText("return @outside", out error), error);

		typeof(MicrocontrollerGameItemComponent)
			.GetMethod("DisconnectSources", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(controller, []);

		controller.Login();

		Assert.AreEqual(1.0, controller.Inputs["outside"], 0.0001);
	}

	[TestMethod]
	public void Microcontroller_Login_RetriesPowerResolutionUntilHostPowerBecomesAccessible()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(47L);
		var hostItem = CreateBasicItem(gameworld.Object, 412L, "Security Door", hostLocation.Object);
		IGameItem[] attachedItems = [];
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);
		hostItem.SetupGet(x => x.AttachedAndConnectedItems).Returns(() => attachedItems);
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns((IProducePower)null!);
		hostItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns(Array.Empty<IProducePower>());

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleItem = CreateBasicItem(gameworld.Object, 413L, "Airlock Controller Module");
		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);
		controller.SwitchedOn = true;
		controller.Login();
		Assert.IsFalse(controller.IsPowered);

		var generatorItem = CreateBasicItem(gameworld.Object, 414L, "Unlimited Generator", hostLocation.Object);
		var hostPower = new Mock<IProducePower>();
		hostPower.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		hostPower.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		hostPower.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		hostPower.SetupGet(x => x.ProducingPower).Returns(true);
		hostPower.Setup(x => x.CanBeginDrawDown(It.IsAny<double>())).Returns(true);
		hostPower.Setup(x => x.BeginDrawdown(It.IsAny<IConsumePower>()))
			.Callback<IConsumePower>(x => x.OnPowerCutIn());
		generatorItem.Setup(x => x.GetItemType<IProducePower>()).Returns(hostPower.Object);
		generatorItem.Setup(x => x.GetItemTypes<IProducePower>()).Returns([hostPower.Object]);
		attachedItems = [generatorItem.Object];

		typeof(PoweredMachineBaseGameItemComponent)
			.GetMethod("RetryPowerConnectionHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(controller, []);

		Assert.IsTrue(controller.IsPowered);
		hostPower.Verify(x => x.BeginDrawdown(controller), Times.Once);
	}

	[TestMethod]
	public void PoweredMachineBase_RetriesSameProducerWhenPowerBecomesAvailableLater()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 415L, "Mounted Controller");
		var canBeginDraw = false;
		var producer = new Mock<IProducePower>();
		producer.SetupGet(x => x.PrimaryLoadTimePowerProducer).Returns(true);
		producer.SetupGet(x => x.PrimaryExternalConnectionPowerProducer).Returns(false);
		producer.SetupGet(x => x.MaximumPowerInWatts).Returns(1000.0);
		producer.SetupGet(x => x.ProducingPower).Returns(() => canBeginDraw);
		producer.Setup(x => x.CanBeginDrawDown(It.IsAny<double>())).Returns(() => canBeginDraw);
		producer.Setup(x => x.BeginDrawdown(It.IsAny<IConsumePower>()))
			.Callback<IConsumePower>(x =>
			{
				if (canBeginDraw)
				{
					x.OnPowerCutIn();
				}
			});
		item.Setup(x => x.GetItemType<IProducePower>()).Returns(producer.Object);
		item.Setup(x => x.GetItemTypes<IProducePower>()).Returns([producer.Object]);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			item.Object,
			true)
		{
			SwitchedOn = true
		};

		controller.Login();
		Assert.IsFalse(controller.IsPowered);

		canBeginDraw = true;
		typeof(PoweredMachineBaseGameItemComponent)
			.GetMethod("RetryPowerConnectionHeartbeat", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(controller, []);

		Assert.IsTrue(controller.IsPowered);
		producer.Verify(x => x.BeginDrawdown(controller), Times.AtLeastOnce);
	}

	[TestMethod]
	public void MotionSensor_DoesNotEmitSignalsUntilPowered()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 500L, "Motion Sensor");
		var power = new Mock<IProducePower>();
		item.Setup(x => x.GetItemType<IProducePower>()).Returns(power.Object);

		var sensor = new MotionSensorGameItemComponent(CreateMotionSensorProto(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};

		var mover = new Mock<ICharacter>();
		mover.SetupGet(x => x.Size).Returns(SizeCategory.Normal);

		sensor.HandleEvent(EventType.CharacterEnterCellWitness, mover.Object);
		Assert.AreEqual(0.0, sensor.CurrentValue, 0.0001);

		sensor.OnPowerCutIn();
		sensor.HandleEvent(EventType.CharacterEnterCellWitness, mover.Object);
		Assert.AreEqual(1.0, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void AutomationMountHost_Quit_QuitsMountedModules()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(49L);
		var hostItem = CreateBasicItem(gameworld.Object, 416L, "Automation Cabinet", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleLocation = CreateCell(50L);
		var moduleItem = CreateBasicItem(gameworld.Object, 417L, "Mounted Controller", moduleLocation.Object);
		var module = new Mock<IAutomationMountable>();
		var connectable = module.As<IConnectable>();
		module.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		module.SetupGet(x => x.MountType).Returns("Microcontroller");
		module.SetupGet(x => x.IsMounted).Returns(false);
		connectable.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		connectable.Setup(x => x.RawConnect(It.IsAny<IConnectable>(), It.IsAny<ConnectorType>()));

		Assert.IsTrue(host.InstallModule(null!, module.Object, "controller", out var error), error);

		host.Quit();

		moduleItem.Verify(x => x.Quit(), Times.Once);
	}

	[TestMethod]
	public void AutomationMountHost_Login_LogsInMountedModules()
	{
		var gameworld = CreateGameworld();
		var hostLocation = CreateCell(51L);
		var hostItem = CreateBasicItem(gameworld.Object, 418L, "Automation Cabinet", hostLocation.Object);
		IGameItemComponent[] hostComponents = [];
		hostItem.SetupGet(x => x.Components).Returns(() => hostComponents);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleLocation = CreateCell(52L);
		var moduleItem = CreateBasicItem(gameworld.Object, 419L, "Mounted Controller", moduleLocation.Object);
		var module = new Mock<IAutomationMountable>();
		var connectable = module.As<IConnectable>();
		module.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		module.SetupGet(x => x.MountType).Returns("Microcontroller");
		module.SetupGet(x => x.IsMounted).Returns(false);
		connectable.SetupGet(x => x.Parent).Returns(moduleItem.Object);
		connectable.Setup(x => x.RawConnect(It.IsAny<IConnectable>(), It.IsAny<ConnectorType>()));

		Assert.IsTrue(host.InstallModule(null!, module.Object, "controller", out var error), error);

		host.Login();

		moduleItem.Verify(x => x.Login(), Times.Once);
	}

	[TestMethod]
	public void MotionSensor_IgnoresImmwalkMovers()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 501L, "Motion Sensor");
		var sensor = new MotionSensorGameItemComponent(CreateMotionSensorProto(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};
		var mover = new Mock<ICharacter>();
		mover.SetupGet(x => x.Size).Returns(SizeCategory.Normal);
		mover.Setup(x => x.CombinedEffectsOfType<IImmwalkEffect>()).Returns([Mock.Of<IImmwalkEffect>()]);

		sensor.OnPowerCutIn();
		sensor.HandleEvent(EventType.CharacterEnterCellWitness, mover.Object);

		Assert.AreEqual(0.0, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void LightSensor_ReportsCurrentIlluminationWhenPowered()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell(502L);
		var illumination = 37.5;
		cell.Setup(x => x.CurrentIllumination(It.IsAny<IPerceiver>())).Returns(() => illumination);
		var item = CreateBasicItem(gameworld.Object, 5020L, "Light Sensor", cell.Object);
		var sensor = new LightSensorGameItemComponent(CreateLightSensorProto(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};

		sensor.OnPowerCutIn();
		Assert.AreEqual(37.5, sensor.CurrentValue, 0.0001);

		illumination = 12.25;
		typeof(LightSensorGameItemComponent)
			.GetMethod("HeartbeatTick", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(sensor, []);

		Assert.AreEqual(12.25, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void RainSensor_ReportsRainIntensityAndSheltersIndoorLocations()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell(503L);
		var outdoorsType = CellOutdoorsType.Outdoors;
		var precipitation = PrecipitationLevel.Rain;
		var weather = new Mock<IWeatherEvent>();
		weather.SetupGet(x => x.Precipitation).Returns(() => precipitation);
		cell.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(() => outdoorsType);
		cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns(() => weather.Object);
		var item = CreateBasicItem(gameworld.Object, 5030L, "Rain Sensor", cell.Object);
		var sensor = new RainSensorGameItemComponent(CreateRainSensorProto(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};

		sensor.OnPowerCutIn();
		Assert.AreEqual(2.0, sensor.CurrentValue, 0.0001);

		outdoorsType = CellOutdoorsType.Indoors;
		typeof(RainSensorGameItemComponent)
			.GetMethod("HeartbeatTick", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(sensor, []);

		Assert.AreEqual(0.0, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void TemperatureSensor_ReportsCurrentTemperatureInCelsiusWhenPowered()
	{
		var gameworld = CreateGameworld();
		var unitManager = new Mock<IUnitManager>();
		unitManager.SetupGet(x => x.BaseTemperatureToCelcius).Returns(2.0);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		var cell = CreateCell(504L);
		cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(10.75);
		var item = CreateBasicItem(gameworld.Object, 5040L, "Temperature Sensor", cell.Object);
		var sensor = new TemperatureSensorGameItemComponent(CreateTemperatureSensorProto(gameworld.Object), item.Object,
			true)
		{
			SwitchedOn = true
		};

		sensor.OnPowerCutIn();

		Assert.AreEqual(21.5, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void Keypad_Select_OnlyEmitsSignalForCorrectPoweredCode()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 5050L, "Access Keypad");
		var sensor = new KeypadGameItemComponent(CreateKeypadProto(gameworld.Object), item.Object, true)
		{
			SwitchedOn = true
		};
		var actor = new Mock<ICharacter>();

		sensor.OnPowerCutIn();
		Assert.IsFalse(sensor.Select(actor.Object, "9999", Mock.Of<IEmote>(), true));
		Assert.AreEqual(0.0, sensor.CurrentValue, 0.0001);

		Assert.IsTrue(sensor.Select(actor.Object, "1234", Mock.Of<IEmote>(), true));
		Assert.AreEqual(1.0, sensor.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void RelaySwitch_ReceiveSignal_PowersConnectedConsumersWhenClosed()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 5060L, "Relay Switch");
		var relay = new RelaySwitchGameItemComponent(CreateRelaySwitchProto(gameworld.Object), item.Object, true);
		var consumer = new Mock<IConsumePower>();
		consumer.SetupGet(x => x.PowerConsumptionInWatts).Returns(10.0);

		relay.BeginDrawdown(consumer.Object);
		Assert.IsFalse(relay.ProducingPower);

		relay.ReceiveSignal(new ComputerSignal(1.0, null, null), Mock.Of<ISignalSource>());
		Assert.IsTrue(relay.ProducingPower);
		consumer.Verify(x => x.OnPowerCutIn(), Times.Once);

		relay.ReceiveSignal(default, Mock.Of<ISignalSource>());
		Assert.IsFalse(relay.ProducingPower);
		consumer.Verify(x => x.OnPowerCutOut(), Times.Once);
	}

	[TestMethod]
	public void SignalCableSegment_ConfigureRoute_MirrorsAdjacentSourceAcrossSpecificExit()
	{
		var destinationCell = CreateCell(20L);
		var sourceExit = new Mock<ICellExit>();
		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.Id).Returns(601L);
		sourceExit.SetupGet(x => x.Exit).Returns(exit.Object);
		sourceExit.SetupGet(x => x.Destination).Returns(destinationCell.Object);
		sourceExit.SetupGet(x => x.OutboundDirectionDescription).Returns("east");
		var sourceCell = CreateCell(19L, [sourceExit.Object]);

		var sourceSignal = new ComputerSignal(7.5, TimeSpan.FromSeconds(15), null);
		var gameworld = CreateGameworld();
		var sourceItem = CreateBasicItem(gameworld.Object, 300L, "Motion Sensor", sourceCell.Object);
		var source = CreateSignalSourceMock(41L, "MotionSensor", parent: sourceItem.Object, signal: sourceSignal,
			componentId: 208L);
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);

		var cableItem = CreateBasicItem(gameworld.Object, 301L, "Signal Cable", destinationCell.Object);
		var cable = new SignalCableSegmentGameItemComponent(CreateSignalCableProto(gameworld.Object), cableItem.Object,
			true);

		gameworld.Setup(x => x.TryGetItem(sourceItem.Object.Id, true)).Returns(sourceItem.Object);

		Assert.IsTrue(cable.ConfigureRoute(source.Object, exit.Object.Id, out var error), error);
		Assert.AreEqual(string.Empty, error);
		Assert.IsTrue(cable.IsRouted);
		Assert.AreEqual(exit.Object.Id, cable.RoutedExitId);
		Assert.AreEqual("east", cable.RouteDescription);
		Assert.AreEqual(7.5, cable.CurrentValue, 0.0001);
		Assert.AreEqual(TimeSpan.FromSeconds(15), cable.Duration);
		Assert.AreEqual(sourceItem.Object.Id, cable.CurrentBinding.SourceItemId);
		Assert.AreEqual(208L, cable.CurrentBinding.SourceComponentId);
	}

	[TestMethod]
	public void SignalCableSegment_ConfigureRoute_RejectsNonAdjacentSource()
	{
		var destinationCell = CreateCell(30L);
		var sourceCell = CreateCell(29L);
		var sourceSignal = new ComputerSignal(2.0, null, null);
		var gameworld = CreateGameworld();
		var sourceItem = CreateBasicItem(gameworld.Object, 310L, "Remote Sensor", sourceCell.Object);
		var source = CreateSignalSourceMock(51L, "RemoteSensor", parent: sourceItem.Object, signal: sourceSignal,
			componentId: 309L);
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);

		var cableItem = CreateBasicItem(gameworld.Object, 311L, "Signal Cable", destinationCell.Object);
		var cable = new SignalCableSegmentGameItemComponent(CreateSignalCableProto(gameworld.Object), cableItem.Object,
			true);

		Assert.IsFalse(cable.ConfigureRoute(source.Object, 777L, out var error));
		StringAssert.Contains(error, "specified one-room exit hop");
		Assert.IsFalse(cable.IsRouted);
		Assert.AreEqual(0.0, cable.CurrentValue, 0.0001);
	}

	[TestMethod]
	public void ElectronicsModule_AdminFastPath_ExecutesImmediatelyAndFinalisesPlansWithExemptions()
	{
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(true);
		var firstPlan = new Mock<IInventoryPlan>();
		var secondPlan = new Mock<IInventoryPlan>();
		var exemptionItem = CreateBasicItem(CreateGameworld().Object, 900L, "Mounted Module").Object;
		var successInvoked = false;
		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("TryExecuteConfiguredActionImmediatelyForAdministrator",
				BindingFlags.Static | BindingFlags.NonPublic);

		var handled = (bool)method!.Invoke(null,
		[
			actor.Object,
			CheckType.ConfigureElectricalComponentCheck,
			new[] { firstPlan.Object, secondPlan.Object },
			(Func<CheckOutcome, bool>)(outcome =>
			{
				successInvoked = true;
				Assert.IsTrue(outcome.IsPass());
				return true;
			}),
			(Func<CheckOutcome, IList<IGameItem>>)(_ => [exemptionItem])
		])!;

		Assert.IsTrue(handled);
		Assert.IsTrue(successInvoked);
		firstPlan.Verify(x => x.FinalisePlanWithExemptions(
			It.Is<IList<IGameItem>>(items => items.Count == 1 && ReferenceEquals(items[0], exemptionItem))), Times.Once);
		secondPlan.Verify(x => x.FinalisePlanWithExemptions(
			It.Is<IList<IGameItem>>(items => items.Count == 1 && ReferenceEquals(items[0], exemptionItem))), Times.Once);
		firstPlan.Verify(x => x.FinalisePlan(), Times.Never);
		secondPlan.Verify(x => x.FinalisePlan(), Times.Never);
	}

	[TestMethod]
	public void ManipulationModule_ServicePanelResolver_ResolvesSingleHousingAlias()
	{
		var gameworld = CreateGameworld();
		var parentItem = CreateBasicItem(gameworld.Object, 901L, "Electronic Door");
		IGameItemComponent[] components = [];
		parentItem.SetupGet(x => x.Components).Returns(() => components);
		var housing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object),
			parentItem.Object,
			true);
		components = [housing];
		var actor = new Mock<ICharacter>();
		parentItem.Setup(x => x.HowSeen(actor.Object, true, It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("electronic door");

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ManipulationModule", true)!
			.GetMethod("TryResolveAutomationServicePanelOpenable", BindingFlags.Static | BindingFlags.NonPublic);
		var arguments = new object?[] { actor.Object, parentItem.Object, "panel", null, null };

		var resolved = (bool)method!.Invoke(null, arguments)!;

		Assert.IsTrue(resolved);
		Assert.AreSame(housing, arguments[3]);
		Assert.AreEqual(string.Empty, arguments[4]);
	}

	[TestMethod]
	public void ManipulationModule_ServicePanelResolver_RejectsAmbiguousAlias()
	{
		var gameworld = CreateGameworld();
		var parentItem = CreateBasicItem(gameworld.Object, 902L, "Electronic Door");
		IGameItemComponent[] components = [];
		parentItem.SetupGet(x => x.Components).Returns(() => components);
		var firstHousing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object, 403L,
				"First Housing"),
			parentItem.Object,
			true);
		var secondHousing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object, 404L,
				"Second Housing"),
			parentItem.Object,
			true);
		components = [firstHousing, secondHousing];
		var actor = new Mock<ICharacter>();
		parentItem.Setup(x => x.HowSeen(actor.Object, true, It.IsAny<DescriptionType>(), It.IsAny<bool>(),
			It.IsAny<PerceiveIgnoreFlags>())).Returns("electronic door");

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ManipulationModule", true)!
			.GetMethod("TryResolveAutomationServicePanelOpenable", BindingFlags.Static | BindingFlags.NonPublic);
		var arguments = new object?[] { actor.Object, parentItem.Object, "panel", null, null };

		var resolved = (bool)method!.Invoke(null, arguments)!;

		Assert.IsFalse(resolved);
		Assert.IsNull(arguments[3]);
		StringAssert.Contains(arguments[4]?.ToString() ?? string.Empty, "more than one");
	}

	[TestMethod]
	public void ItemComponentConfigurationAction_RemovalEffect_IsIdempotent()
	{
		var actor = new Mock<ICharacter>();
		var body = new Mock<IBody>();
		var outputHandler = new Mock<IOutputHandler>();
		actor.SetupGet(x => x.Body).Returns(body.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);

		var target = CreateBasicItem(CreateGameworld().Object, 903L, "Target Panel");
		var tool = CreateBasicItem(CreateGameworld().Object, 904L, "Tool");
		var plan = new Mock<IInventoryPlan>();
		var effect = new ItemComponentConfigurationAction(
			actor.Object,
			target.Object,
			tool.Object,
			plan.Object,
			"configuring $0",
			"@ begin|begins configuring $0 with $1.",
			"@ continue|continues configuring $0 with $1.",
			"@ stop|stops configuring $0.",
			"@ finish|finishes configuring $0 with $1.",
			"@ fail|fails to configure $0 with $1.",
			TimeSpan.FromSeconds(1),
			3,
			() => CheckOutcome.SimpleOutcome(CheckType.ConfigureElectricalComponentCheck, Outcome.Pass),
			_ => true);

		effect.RemovalEffect();
		effect.RemovalEffect();

		plan.Verify(x => x.FinalisePlan(), Times.Once);
	}

	[TestMethod]
	public void AutomationHousing_Decorate_FullDescription_DoesNotRepeatBaseDescription()
	{
		var gameworld = CreateGameworld();
		var housingItem = CreateBasicItem(gameworld.Object, 905L, "Service Housing");
		var housing = new AutomationHousingGameItemComponent(CreateAutomationHousingProto(gameworld.Object),
			housingItem.Object,
			true);
		housing.Open();

		var description = housing.Decorate(Mock.Of<IPerceiver>(), housing.Name, "Base description",
			DescriptionType.Full,
			true,
			PerceiveIgnoreFlags.None);

		Assert.AreEqual(1, CountOccurrences(description, "Base description"));
	}

	[TestMethod]
	public void ElectronicDoor_Decorate_DoesNotExposeControllerDiagnostics()
	{
		var gameworld = CreateGameworld();
		var item = CreateBasicItem(gameworld.Object, 906L, "Electronic Door");
		var door = new ElectronicDoorGameItemComponent(CreateElectronicDoorProto(gameworld.Object), item.Object, true);

		var description = door.Decorate(Mock.Of<IPerceiver>(), door.Name, "Base description", DescriptionType.Full,
			true,
			PerceiveIgnoreFlags.None);

		Assert.AreEqual("Base description", description.TrimEnd('\r', '\n'));
		Assert.IsFalse(description.Contains("electronic controller is listening", StringComparison.InvariantCulture));
		Assert.IsFalse(description.Contains("current control signal", StringComparison.InvariantCulture));
	}

	[TestMethod]
	public void ElectronicDoor_HeartbeatReconnect_ResolvesLateSourceAndOpensWhenSignalIsActive()
	{
		var gameworld = CreateGameworld();
		var sharedCell = CreateCell(9061L);
		var currentLocations = Array.Empty<ICell>();
		var doorItem = CreateBasicItem(gameworld.Object, 9062L, "Electronic Door");
		doorItem.SetupGet(x => x.TrueLocations).Returns(() => currentLocations);
		doorItem.SetupGet(x => x.Location).Returns(() => currentLocations.FirstOrDefault()!);
		doorItem.SetupGet(x => x.AttachedAndConnectedItems).Returns(Array.Empty<IGameItem>());

		var sourceItem = CreateBasicItem(gameworld.Object, 9063L, "Airlock Controller Module", sharedCell.Object);
		var source = CreateSignalSourceMock(1L, "DoorController",
			parent: sourceItem.Object,
			componentId: 1L,
			signal: new ComputerSignal(1.0, null, null));
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);
		sourceItem.SetupGet(x => x.Components).Returns([source.Object]);

		var door = new ElectronicDoorGameItemComponent(CreateElectronicDoorProto(gameworld.Object), doorItem.Object, true)
		{
			State = DoorState.Closed
		};

		door.FinaliseLoad();
		door.Login();
		Assert.IsFalse(door.IsOpen);

		currentLocations = [sharedCell.Object];
		sharedCell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([doorItem.Object, sourceItem.Object]);

		typeof(ElectronicDoorGameItemComponent)
			.GetMethod("HeartbeatTick", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(door, []);

		Assert.IsTrue(door.IsOpen);
	}

	[TestMethod]
	public void ElectronicsModule_ResolveNearbySignalSource_UsesMountedHostLocationAndPlayerItemKeywords()
	{
		var gameworld = CreateGameworld();
		var cell = CreateCell(907L);
		var hostItem = CreateBasicItem(gameworld.Object, 9070L, "Electronic Door", cell.Object);
		var moduleItem = CreateBasicItem(gameworld.Object, 9071L, "Airlock Controller Module");
		var sourceItem = CreateBasicItem(gameworld.Object, 9072L, "Outside Motion Sensor", cell.Object);
		var source = CreateSignalSourceMock(9073L, "Door Outside Motion Sensor", parent: sourceItem.Object,
			componentId: 9074L);
		var host = new Mock<IAutomationMountHost>();
		var mountable = new Mock<IAutomationMountable>();
		var actor = new Mock<ICharacter>();

		host.SetupGet(x => x.Parent).Returns(hostItem.Object);
		mountable.SetupGet(x => x.MountHost).Returns(host.Object);
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(mountable.Object);
		sourceItem.SetupGet(x => x.Components).Returns([source.Object]);
		actor.Setup(x => x.TargetItem("outside")).Returns(sourceItem.Object);
		cell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([hostItem.Object, sourceItem.Object]);

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("ResolveNearbySignalSource", BindingFlags.Static | BindingFlags.NonPublic);

		var resolved = method!.Invoke(null, [actor.Object, moduleItem.Object, "outside", "signal source component"]);

		Assert.AreSame(source.Object, resolved);
	}

	[TestMethod]
	public void ElectronicsModule_ResolveNearbySignalSource_FallsBackToActorLocationWhenMountedHostHasNoTrueLocation()
	{
		var gameworld = CreateGameworld();
		var actorCell = CreateCell(9075L);
		var hostItem = CreateBasicItem(gameworld.Object, 9076L, "Security Door");
		var moduleItem = CreateBasicItem(gameworld.Object, 9077L, "Airlock Controller Module");
		var sourceItem = CreateBasicItem(gameworld.Object, 9078L, "Outside Motion Sensor", actorCell.Object);
		var source = CreateSignalSourceMock(9079L, "Door Outside Motion Sensor", parent: sourceItem.Object,
			componentId: 9080L);
		var host = new Mock<IAutomationMountHost>();
		var mountable = new Mock<IAutomationMountable>();
		var actor = new Mock<ICharacter>();

		host.SetupGet(x => x.Parent).Returns(hostItem.Object);
		mountable.SetupGet(x => x.MountHost).Returns(host.Object);
		moduleItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(mountable.Object);
		sourceItem.SetupGet(x => x.Components).Returns([source.Object]);
		actor.SetupGet(x => x.Location).Returns(actorCell.Object);
		actor.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		actor.Setup(x => x.TargetItem("sensor")).Returns(sourceItem.Object);
		actorCell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([sourceItem.Object]);

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("ResolveNearbySignalSource", BindingFlags.Static | BindingFlags.NonPublic);

		var resolved = method!.Invoke(null, [actor.Object, moduleItem.Object, "sensor", "signal source component"]);

		Assert.AreSame(source.Object, resolved);
	}

	[TestMethod]
	public void ElectronicsModule_DescribeComponent_DoesNotIncludeRawIds()
	{
		var gameworld = CreateGameworld();
		var actor = new Mock<ICharacter>();
		var item = CreateBasicItem(gameworld.Object, 9080L, "Outside Motion Sensor");
		var source = CreateSignalSourceMock(9081L, "Door Outside Motion Sensor", parent: item.Object, componentId: 9082L);
		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("DescribeComponent", BindingFlags.Static | BindingFlags.NonPublic);

		var description = (string)method!.Invoke(null, [actor.Object, source.Object])!;

		Assert.AreEqual("Outside Motion Sensor@Door Outside Motion Sensor", description);
		Assert.IsFalse(description.Contains("[", StringComparison.InvariantCulture));
		Assert.IsFalse(description.Contains("9082", StringComparison.InvariantCulture));
	}

	[TestMethod]
	public void ElectronicsModule_ShowElectricalStatus_DisplaysControllerInputsAndResolvedSignalPaths()
	{
		var gameworld = CreateGameworld();
		var sharedCell = CreateCell(9083L);
		var doorItem = CreateBasicItem(gameworld.Object, 9084L, "Electronic Door", sharedCell.Object);
		var controllerItem = CreateBasicItem(gameworld.Object, 9085L, "Airlock Controller Module");
		var sourceItem = CreateBasicItem(gameworld.Object, 9086L, "Outside Motion Sensor", sharedCell.Object);
		var source = CreateSignalSourceMock(9087L, "Door Outside Motion Sensor", parent: sourceItem.Object,
			componentId: 9088L,
			signal: new ComputerSignal(1.0, TimeSpan.FromSeconds(10), null));
		var controller = new Mock<IRuntimeProgrammableMicrocontroller>();
		var host = new Mock<IAutomationMountHost>();
		var mountable = new Mock<IAutomationMountable>();
		var outputHandler = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		string? statusText = null;

		host.SetupGet(x => x.Parent).Returns(doorItem.Object);
		mountable.SetupGet(x => x.MountHost).Returns(host.Object);
		controllerItem.Setup(x => x.GetItemType<IAutomationMountable>()).Returns(mountable.Object);
		controllerItem.SetupGet(x => x.Components).Returns([controller.Object]);
		controllerItem.SetupGet(x => x.AttachedAndConnectedItems).Returns(Array.Empty<IGameItem>());
		sourceItem.SetupGet(x => x.Components).Returns([source.Object]);
		sourceItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([source.Object]);
		doorItem.SetupGet(x => x.AttachedAndConnectedItems).Returns([controllerItem.Object]);
		gameworld.Setup(x => x.TryGetItem(sourceItem.Object.Id, true)).Returns(sourceItem.Object);

		controller.SetupGet(x => x.Parent).Returns(controllerItem.Object);
		controller.SetupGet(x => x.Id).Returns(9089L);
		controller.As<IFrameworkItem>().SetupGet(x => x.Name).Returns("Door Controller");
		controller.SetupGet(x => x.EndpointKey).Returns(SignalComponentUtilities.DefaultLocalSignalEndpointKey);
		controller.SetupGet(x => x.CurrentSignal).Returns(new ComputerSignal(1.0, null, null));
		controller.SetupGet(x => x.CurrentValue).Returns(1.0);
		controller.SetupGet(x => x.Duration).Returns((TimeSpan?)null);
		controller.SetupGet(x => x.PulseInterval).Returns((TimeSpan?)null);
		controller.SetupGet(x => x.LogicCompiles).Returns(true);
		controller.SetupGet(x => x.CompileError).Returns(string.Empty);
		controller.SetupGet(x => x.InputBindings).Returns([
			new MicrocontrollerRuntimeInputBinding(
				"outside",
				new LocalSignalBinding(
					sourceItem.Object.Id,
					sourceItem.Object.Name,
					9088L,
					"Door Outside Motion Sensor",
					SignalComponentUtilities.DefaultLocalSignalEndpointKey),
				1.0)
		]);
		controller.As<IOnOff>().SetupGet(x => x.SwitchedOn).Returns(true);

		actor.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		outputHandler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => statusText = text);

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("ShowElectricalStatus", BindingFlags.Static | BindingFlags.NonPublic);

		method!.Invoke(null, [actor.Object, doorItem.Object]);

		Assert.IsNotNull(statusText);
		var stripped = statusText!.StripANSIColour();
		StringAssert.Contains(stripped, "Programmable Controllers:");
		StringAssert.Contains(stripped, "outside <- Outside Motion Sensor@Door Outside Motion Sensor:signal = 1.00");
		StringAssert.Contains(stripped, "resolved to Outside Motion Sensor@Door Outside Motion Sensor");
	}

	[TestMethod]
	public void ElectronicsModule_ShowElectricalStatus_DisplaysNearbyCableRoutesForInspectedSensor()
	{
		var gameworld = CreateGameworld();
		var sharedCell = CreateCell(9090L);
		var outputHandler = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		string? statusText = null;
		actor.SetupGet(x => x.OutputHandler).Returns(outputHandler.Object);
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		outputHandler.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
			.Callback<string, bool, bool>((text, _, _) => statusText = text);

		var sensorItem = CreateBasicItem(gameworld.Object, 9091L, "Outside Motion Sensor", sharedCell.Object);
		var sensor = CreateSignalSourceMock(9092L, "Door Outside Motion Sensor", parent: sensorItem.Object,
			componentId: 9093L,
			signal: new ComputerSignal(1.0, TimeSpan.FromSeconds(10), null));
		sensorItem.SetupGet(x => x.Components).Returns([sensor.Object]);
		sensorItem.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns([sensor.Object]);

		var cableItem = CreateBasicItem(gameworld.Object, 9094L, "Signal Cable", sharedCell.Object);
		var cable = new SignalCableSegmentGameItemComponent(CreateSignalCableProto(gameworld.Object), cableItem.Object,
			true);
		cableItem.SetupGet(x => x.Components).Returns([cable]);
		sharedCell.Setup(x => x.LayerGameItems(RoomLayer.GroundLevel)).Returns([sensorItem.Object, cableItem.Object]);
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns((long id, bool _) => id == sensorItem.Object.Id ? sensorItem.Object : null!);

		var exit = new Mock<IExit>();
		exit.SetupGet(x => x.Id).Returns(9095L);
		var route = new Mock<ICellExit>();
		route.SetupGet(x => x.Exit).Returns(exit.Object);
		route.SetupGet(x => x.Destination).Returns(sharedCell.Object);
		route.SetupGet(x => x.OutboundDirectionDescription).Returns("south");
		sharedCell.Setup(x => x.ExitsFor(sensorItem.Object)).Returns([route.Object]);

		Assert.IsTrue(cable.ConfigureRoute(sensor.Object, exit.Object.Id, out var error), error);

		var method = typeof(ElectronicDoorGameItemComponent).Assembly
			.GetType("MudSharp.Commands.Modules.ElectronicsModule", true)!
			.GetMethod("ShowElectricalStatus", BindingFlags.Static | BindingFlags.NonPublic);

		method!.Invoke(null, [actor.Object, sensorItem.Object]);

		Assert.IsNotNull(statusText);
		var stripped = statusText!.StripANSIColour();
		StringAssert.Contains(stripped, "Nearby Cable Routes:");
		StringAssert.Contains(stripped, "Signal Cable");
		StringAssert.Contains(stripped, "route live");
		StringAssert.Contains(stripped, "south");
	}

	private static Mock<ISignalSourceComponent> CreateSignalSourceMock(long identifier, string name,
		string endpointKey = SignalComponentUtilities.DefaultLocalSignalEndpointKey, long? componentId = null,
		IGameItem? parent = null, ComputerSignal? signal = null)
	{
		var computerSignal = signal ?? default;
		var source = new Mock<ISignalSourceComponent>();
		source.SetupGet(x => x.LocalSignalSourceIdentifier).Returns(identifier);
		source.SetupGet(x => x.Id).Returns(componentId ?? identifier);
		source.SetupGet(x => x.CurrentSignal).Returns(computerSignal);
		source.SetupGet(x => x.CurrentValue).Returns(computerSignal.Value);
		source.SetupGet(x => x.Duration).Returns(computerSignal.Duration);
		source.SetupGet(x => x.PulseInterval).Returns(computerSignal.PulseInterval);
		source.SetupGet(x => x.Parent).Returns(parent ?? Mock.Of<IGameItem>());
		source.As<IFrameworkItem>().SetupGet(x => x.Name).Returns(name);
		source.As<ISignalSource>().SetupGet(x => x.Name).Returns(name);
		source.As<ISignalSource>().SetupGet(x => x.EndpointKey).Returns(endpointKey);
		return source;
	}

	private static Mock<IFuturemud> CreateGameworld(IEnumerable<IGameItem>? items = null)
	{
		var gameworld = new Mock<IFuturemud>();
		var heartbeatManager = new Mock<IHeartbeatManager>();
		var futureProgs = new Mock<IUneditableAll<MudSharp.FutureProg.IFutureProg>>();
		var unitManager = new Mock<IUnitManager>();
		var itemList = (items ?? Enumerable.Empty<IGameItem>()).ToList();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.UnitManager).Returns(unitManager.Object);
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns((long id, bool _) => itemList.FirstOrDefault(x => x.Id == id)!);
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((MudSharp.FutureProg.IFutureProg)null!);
		unitManager.SetupGet(x => x.BaseTemperatureToCelcius).Returns(1.0);
		return gameworld;
	}

	private static Mock<ICell> CreateCell(long id, IEnumerable<ICellExit>? exits = null)
	{
		var cell = new Mock<ICell>();
		var exitList = (exits ?? Enumerable.Empty<ICellExit>()).ToList();
		cell.SetupGet(x => x.Id).Returns(id);
		cell.Setup(x => x.ExitsFor(It.IsAny<IPerceiver>(), It.IsAny<bool>())).Returns(() => exitList);
		cell.Setup(x => x.Insert(It.IsAny<IGameItem>()));
		cell.Setup(x => x.Extract(It.IsAny<IGameItem>()));
		cell.Setup(x => x.CurrentIllumination(It.IsAny<IPerceiver>())).Returns(0.0);
		cell.Setup(x => x.CurrentTemperature(It.IsAny<IPerceiver>())).Returns(0.0);
		cell.Setup(x => x.OutdoorsType(It.IsAny<IPerceiver>())).Returns(CellOutdoorsType.Outdoors);
		cell.Setup(x => x.CurrentWeather(It.IsAny<IPerceiver>())).Returns((IWeatherEvent)null!);
		return cell;
	}

	private static Mock<IGameItem> CreateBasicItem(IFuturemud gameworld, long id, string name, params ICell[] trueLocations)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		item.SetupGet(x => x.TrueLocations).Returns(trueLocations);
		item.SetupGet(x => x.Location).Returns(() => trueLocations.FirstOrDefault()!);
		item.SetupGet(x => x.RoomLayer).Returns(RoomLayer.GroundLevel);
		item.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponent>());
		item.Setup(x => x.HowSeen(It.IsAny<IPerceiver>(), It.IsAny<bool>(), It.IsAny<DescriptionType>(),
			It.IsAny<bool>(), It.IsAny<PerceiveIgnoreFlags>())).Returns(name);
		item.Setup(x => x.GetItemTypes<IConnectable>()).Returns(Array.Empty<IConnectable>());
		item.Setup(x => x.GetItemTypes<ISignalSourceComponent>()).Returns(Array.Empty<ISignalSourceComponent>());
		item.Setup(x => x.AttachedAndConnectedItems).Returns(Array.Empty<IGameItem>());
		return item;
	}

	private static AutomationMountHostGameItemComponentProto CreateAutomationMountHostProto(IFuturemud gameworld,
		IEnumerable<(string Name, string MountType)> bays, long accessPanelPrototypeId = 0L,
		string accessPanelPrototypeName = "")
	{
		var definition = new XElement("Definition",
			new XElement("Bays",
				from bay in bays
				select new XElement("Bay",
					new XAttribute("name", bay.Name),
					new XAttribute("mounttype", bay.MountType))),
			new XElement("AccessPanelPrototypeId", accessPanelPrototypeId),
			new XElement("AccessPanelPrototypeName", new XCData(accessPanelPrototypeName))
		);

		return (AutomationMountHostGameItemComponentProto)typeof(AutomationMountHostGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 401L,
					Name = "Automation Mount Host",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static SignalCableSegmentGameItemComponentProto CreateSignalCableProto(IFuturemud gameworld)
	{
		return (SignalCableSegmentGameItemComponentProto)typeof(SignalCableSegmentGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 402L,
					Name = "Signal Cable Segment",
					Description = "Test",
					RevisionNumber = 1,
					Definition = new XElement("Definition").ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static AutomationHousingGameItemComponentProto CreateAutomationHousingProto(IFuturemud gameworld,
		long id = 403L, string name = "Automation Housing", bool allowCableSegments = true,
		bool allowMountableModules = true, bool allowSignalItems = true)
	{
		var definition = new XElement("Definition",
			new XAttribute("Weight", 1000.0),
			new XAttribute("MaxSize", (int)SizeCategory.Small),
			new XAttribute("Preposition", "in"),
			new XAttribute("Transparent", false),
			new XElement("ForceDifficulty", (int)Difficulty.Normal),
			new XElement("PickDifficulty", (int)Difficulty.Normal),
			new XElement("LockEmote", new XCData("@ lock|locks $1$?2| with $2||$")),
			new XElement("UnlockEmote", new XCData("@ unlock|unlocks $1$?2| with $2||$")),
			new XElement("LockEmoteNoActor", new XCData("@ lock|locks")),
			new XElement("UnlockEmoteNoActor", new XCData("@ unlock|unlocks")),
			new XElement("LockType", "Lever Lock"),
			new XElement("AllowCableSegments", allowCableSegments),
			new XElement("AllowMountableModules", allowMountableModules),
			new XElement("AllowSignalItems", allowSignalItems)
		);

		return (AutomationHousingGameItemComponentProto)typeof(AutomationHousingGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = id,
					Name = name,
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static MicrocontrollerGameItemComponentProto CreateMicrocontrollerProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 650.0),
			new XElement("WattageDiscount", 30.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0),
			new XElement("LogicText", new XCData("return 0"))
		);

		return (MicrocontrollerGameItemComponentProto)typeof(MicrocontrollerGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 404L,
					Name = "Microcontroller",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static MotionSensorGameItemComponentProto CreateMotionSensorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 50.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0),
			new XElement("SignalValue", 1.0),
			new XElement("SignalDurationSeconds", 10.0),
			new XElement("MinimumSize", SizeCategory.Normal),
			new XElement("DetectionMode", MotionSensorDetectionMode.AnyMovement)
		);

		return (MotionSensorGameItemComponentProto)typeof(MotionSensorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 405L,
					Name = "Motion Sensor",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static LightSensorGameItemComponentProto CreateLightSensorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 25.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0)
		);

		return (LightSensorGameItemComponentProto)typeof(LightSensorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4061L,
					Name = "Light Sensor",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static RainSensorGameItemComponentProto CreateRainSensorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 25.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0)
		);

		return (RainSensorGameItemComponentProto)typeof(RainSensorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4062L,
					Name = "Rain Sensor",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static TemperatureSensorGameItemComponentProto CreateTemperatureSensorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 25.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0)
		);

		return (TemperatureSensorGameItemComponentProto)typeof(TemperatureSensorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4063L,
					Name = "Temperature Sensor",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static KeypadGameItemComponentProto CreateKeypadProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 35.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0),
			new XElement("Code", new XCData("1234")),
			new XElement("SignalValue", 1.0),
			new XElement("SignalDurationSeconds", 1.0),
			new XElement("EntryEmote", new XCData("@ tap|taps digits into $1"))
		);

		return (KeypadGameItemComponentProto)typeof(KeypadGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4064L,
					Name = "Keypad",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static RelaySwitchGameItemComponentProto CreateRelaySwitchProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 100.0),
			new XElement("SourceComponentId", 1L),
			new XElement("SourceComponentName", new XCData("RelaySource")),
			new XElement("SourceEndpointKey", new XCData("signal")),
			new XElement("ActivationThreshold", 0.5),
			new XElement("ClosedWhenAboveThreshold", true)
		);

		return (RelaySwitchGameItemComponentProto)typeof(RelaySwitchGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4065L,
					Name = "Relay Switch",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static ComputerHostGameItemComponentProto CreateComputerHostProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 100.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", false),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0),
			new XElement("StorageCapacityInBytes", 8192L),
			new XElement("StoragePorts", 1),
			new XElement("TerminalPorts", 1),
			new XElement("NetworkPorts", 1)
		);

		return (ComputerHostGameItemComponentProto)typeof(ComputerHostGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4066L,
					Name = "Computer Host",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static FileSignalGeneratorGameItemComponentProto CreateFileSignalGeneratorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XElement("Wattage", 40.0),
			new XElement("WattageDiscount", 0.0),
			new XElement("Switchable", true),
			new XElement("UseMountHostPowerSource", true),
			new XElement("PowerOnEmote", new XCData("@ hum|hums briefly as it powers on")),
			new XElement("PowerOffEmote", new XCData("@ shudder|shudders as it powers down.")),
			new XElement("OnPoweredProg", 0),
			new XElement("OnUnpoweredProg", 0),
			new XElement("SignalFileName", new XCData("signal.txt")),
			new XElement("InitialFileContents", new XCData("0")),
			new XElement("FileCapacityInBytes", 4096L),
			new XElement("PubliclyAccessibleByDefault", false)
		);

		return (FileSignalGeneratorGameItemComponentProto)typeof(FileSignalGeneratorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 4067L,
					Name = "File Signal Generator",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static ElectronicDoorGameItemComponentProto CreateElectronicDoorProto(IFuturemud gameworld)
	{
		var definition = new XElement("Definition",
			new XAttribute("SeeThrough", false),
			new XAttribute("CanFireThrough", false),
			new XElement("InstalledExitDescription", "door"),
			new XElement("CanBeOpenedByPlayers", false),
			new XElement("Uninstall",
				new XAttribute("CanPlayersUninstall", false),
				new XAttribute("UninstallDifficultyHingeSide", (int)Difficulty.Impossible),
				new XAttribute("UninstallDifficultyNotHingeSide", (int)Difficulty.Impossible),
				new XAttribute("UninstallTrait", 0)),
			new XElement("Smash",
				new XAttribute("CanPlayersSmash", false),
				new XAttribute("SmashDifficulty", (int)Difficulty.Impossible)),
			new XElement("SourceComponentId", 1L),
			new XElement("SourceComponentName", new XCData("DoorController")),
			new XElement("SourceEndpointKey", new XCData("signal")),
			new XElement("ActivationThreshold", 0.5),
			new XElement("OpenWhenAboveThreshold", true),
			new XElement("OpenEmoteNoActor", new XCData("@ slide|slides open")),
			new XElement("CloseEmoteNoActor", new XCData("@ slide|slides closed"))
		);

		return (ElectronicDoorGameItemComponentProto)typeof(ElectronicDoorGameItemComponentProto)
			.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
				[typeof(MudSharp.Models.GameItemComponentProto), typeof(IFuturemud)], null)!
			.Invoke([
				new MudSharp.Models.GameItemComponentProto
				{
					Id = 406L,
					Name = "Electronic Door",
					Description = "Test",
					RevisionNumber = 1,
					Definition = definition.ToString(),
					EditableItem = new MudSharp.Models.EditableItem
					{
						RevisionStatus = (int)RevisionStatus.Current,
						RevisionNumber = 1
					}
				},
				gameworld
			]);
	}

	private static int CountOccurrences(string text, string value)
	{
		var count = 0;
		var index = 0;
		while ((index = text.IndexOf(value, index, StringComparison.InvariantCulture)) >= 0)
		{
			count++;
			index += value.Length;
		}

		return count;
	}

	private static void AddHousingContents(AutomationHousingGameItemComponent housing, IEnumerable<IGameItem> items)
	{
		var field = typeof(LockingContainerGameItemComponent).GetField("_contents",
			BindingFlags.Instance | BindingFlags.NonPublic);
		var contents = (List<IGameItem>)field!.GetValue(housing)!;
		contents.AddRange(items);
	}
}
