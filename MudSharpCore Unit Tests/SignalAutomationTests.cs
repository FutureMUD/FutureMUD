#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Computers;
using MudSharp.Events;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Components;
using MudSharp.GameItems.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
				"electroniclock", "electronicdoor", "alarmsiren"
			},
			primaryTypes);
		CollectionAssert.IsSubsetOf(
			new[]
			{
				"PushButton", "ToggleSwitch", "MotionSensor", "TimerSensor", "Microcontroller", "SignalLight",
				"ElectronicLock", "ElectronicDoor", "AlarmSiren"
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

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 41L, "OriginalButton");

		Assert.AreSame(renamedSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_ResolvesCorrectSourceWhenNamesOverlap()
	{
		var firstSource = CreateSignalSourceMock(41L, "SignalSource");
		var secondSource = CreateSignalSourceMock(99L, "SignalSource");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { firstSource.Object, secondSource.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 99L, "SignalSource");

		Assert.AreSame(secondSource.Object, resolved);
	}

	[TestMethod]
	public void SignalComponentUtilities_FindSignalSource_FallsBackToLegacyNameWhenIdentifierMissing()
	{
		var source = CreateSignalSourceMock(41L, "LegacyButton");
		var parent = new Mock<IGameItem>();
		parent.Setup(x => x.GetItemTypes<ISignalSourceComponent>())
			.Returns(new[] { source.Object });

		var resolved = SignalComponentUtilities.FindSignalSource(parent.Object, 0L, "LegacyButton");

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

	private static Mock<ISignalSourceComponent> CreateSignalSourceMock(long identifier, string name)
	{
		var source = new Mock<ISignalSourceComponent>();
		source.SetupGet(x => x.LocalSignalSourceIdentifier).Returns(identifier);
		source.SetupGet(x => x.CurrentSignal).Returns(default(ComputerSignal));
		source.SetupGet(x => x.CurrentValue).Returns(0.0);
		source.SetupGet(x => x.Duration).Returns((TimeSpan?)null);
		source.SetupGet(x => x.PulseInterval).Returns((TimeSpan?)null);
		source.As<IFrameworkItem>().SetupGet(x => x.Name).Returns(name);
		source.As<ISignalSource>().SetupGet(x => x.Name).Returns(name);
		return source;
	}
}
