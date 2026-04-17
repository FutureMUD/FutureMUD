#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Computers;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Scheduling;
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
				"automationhousing"
			},
			primaryTypes);
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"PushButton", "ToggleSwitch", "MotionSensor", "TimerSensor", "Microcontroller", "SignalLight",
				"ElectronicLock", "ElectronicDoor", "AlarmSiren", "Automation Mount Host", "Signal Cable Segment",
				"Automation Housing"
			},
			helpTypes);
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
		hostItem.Setup(x => x.GetItemType<IProducePower>()).Returns(hostPower.Object);

		var host = new AutomationMountHostGameItemComponent(
			CreateAutomationMountHostProto(gameworld.Object, [("controller", "Microcontroller")]),
			hostItem.Object,
			true);
		hostComponents = [host];

		var moduleLocation = CreateCell(41L);
		var moduleItem = CreateBasicItem(gameworld.Object, 401L, "Mounted Controller", moduleLocation.Object);
		var localPower = new Mock<IProducePower>();
		moduleItem.Setup(x => x.GetItemType<IProducePower>()).Returns(localPower.Object);

		var controller = new MicrocontrollerGameItemComponent(CreateMicrocontrollerProto(gameworld.Object),
			moduleItem.Object,
			true);

		Assert.IsTrue(host.InstallModule(null!, controller, "controller", out var error), error);
		controller.SwitchedOn = true;

		hostPower.Verify(x => x.BeginDrawdown(controller), Times.Once);
		localPower.Verify(x => x.BeginDrawdown(controller), Times.Never);
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
		var itemList = (items ?? Enumerable.Empty<IGameItem>()).ToList();
		gameworld.SetupGet(x => x.HeartbeatManager).Returns(heartbeatManager.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.Setup(x => x.TryGetItem(It.IsAny<long>(), It.IsAny<bool>()))
			.Returns((long id, bool _) => itemList.FirstOrDefault(x => x.Id == id));
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns((MudSharp.FutureProg.IFutureProg?)null);
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
		return cell;
	}

	private static Mock<IGameItem> CreateBasicItem(IFuturemud gameworld, long id, string name, params ICell[] trueLocations)
	{
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(id);
		item.SetupGet(x => x.Name).Returns(name);
		item.SetupGet(x => x.Gameworld).Returns(gameworld);
		item.SetupGet(x => x.TrueLocations).Returns(trueLocations);
		item.SetupGet(x => x.Location).Returns(() => trueLocations.FirstOrDefault());
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
