#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.Communication;
using MudSharp.Communication.Language;
using MudSharp.Construction;
using MudSharp.Construction.Boundary;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Handlers;
using MudSharp.RPG.Checks;
using MudSharp.RPG.AIStorytellers;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PassiveSpeechRelayTests
{
	[TestMethod]
	public void Relay_RequiresPassiveCapabilityAudibilityAndSuccessfulNativeCheckBeforeOutputOrEvent()
	{
		var f = new Fixture();
		f.Listener.Setup(x => x.CombinedEffectsOfType<IPassiveListeningEffect>()).Returns([]);
		Assert.IsFalse(f.Relay()); f.Output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Never);
		f.Enable(); f.Listener.Setup(x => x.CanHear(f.Source.Object)).Returns(false);
		Assert.IsFalse(f.Relay()); f.Check.Verify(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()), Times.Never);
		f.Listener.Setup(x => x.CanHear(f.Source.Object)).Returns(true);
		f.SetOutcome(Outcome.Fail); Assert.IsFalse(f.Relay());
		f.Output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Never);
		f.Listener.Verify(x => x.HandleEvent(EventType.CharacterSpeaksNearbyWitness, It.IsAny<object[]>()), Times.Never);
		f.SetOutcome(Outcome.Pass); Assert.IsTrue(f.Relay());
		f.Output.Verify(x => x.Send(It.Is<LanguageOutput>(output => output.LanguageInfo.RawText == f.Speech.RawText), true, false), Times.Once);
		f.Listener.Verify(x => x.HandleEvent(EventType.CharacterSpeaksNearbyWitness,
			It.Is<object[]>(args => args.Last()!.Equals(f.Speech.RawText))), Times.Once);
	}

	[TestMethod]
	public void Relay_NpcWithoutATerminalStillReceivesTheNativeNearbySpeechEvent()
	{
		var f = new Fixture();
		f.Listener.SetupGet(x => x.OutputHandler).Returns(new NonPlayerOutputHandler());
		Assert.IsTrue(f.Relay());
		f.Listener.Verify(x => x.HandleEvent(EventType.CharacterSpeaksNearbyWitness, It.IsAny<object[]>()), Times.Once);
	}

	[TestMethod]
	public void OrdinarySay_LocalSpeechWitnessesRunBeforeNearbyCallbacksCanMoveTheSpeaker()
	{
		var f = new Fixture(); var local = new Mock<ICharacter>(); var unrelated = new Mock<ICharacter>();
		var elsewhere = new Mock<IRoom>(); var current = f.Origin.Object;
		var observed = new System.Collections.Generic.List<string>();
		f.Source.SetupGet(x => x.Location).Returns(() => current); f.Body.SetupGet(x => x.Location).Returns(() => current);
		f.Source.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.Body.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.Body.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		f.Body.SetupGet(x => x.CurrentLanguage).Returns(f.Speech.Language); f.Body.SetupGet(x => x.CurrentAccent).Returns(f.Speech.Accent);
		f.Origin.SetupGet(x => x.EventHandlers).Returns([f.Source.Object, local.Object]);
		f.Origin.SetupGet(x => x.Gameworld).Returns(f.World.Object);
		f.Origin.Setup(x => x.EffectsOfType<IRecentSpeechContextEffect>(It.IsAny<Predicate<IRecentSpeechContextEffect>>()))
			.Returns([Mock.Of<IRecentSpeechContextEffect>()]);
		elsewhere.SetupGet(x => x.EventHandlers).Returns([unrelated.Object]);
		f.Origin.Setup(x => x.ExitsFor(f.Source.Object, false)).Returns([f.Exit.Object]);
		f.Destination.SetupGet(x => x.Characters).Returns([f.Listener.Object]);
		f.World.SetupGet(x => x.AIStorytellers).Returns(new All<IAIStoryteller>());
		f.World.Setup(x => x.GetCheck(CheckType.SpokenLanguageSpeakCheck)).Returns(f.Check.Object);
		f.Check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<ITraitDefinition>(),
			It.IsAny<IPerceivable>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.SpokenLanguageSpeakCheck, Outcome.Pass));
		local.Setup(x => x.HandleEvent(EventType.CharacterSpeaksWitness, It.IsAny<object[]>())).Callback(() => observed.Add("local"));
		f.Listener.Setup(x => x.HandleEvent(EventType.CharacterSpeaksNearbyWitness, It.IsAny<object[]>())).Callback(() =>
		{
			observed.Add("nearby"); current = elsewhere.Object;
		});
		var strategy = new Mock<HumanoidCommunicationStrategy> { CallBase = true };
		strategy.Setup(x => x.CanVocalise(f.Body.Object, AudioVolume.Decent)).Returns(true);
		strategy.Object.Say(f.Body.Object, null!, "A secret.");
		CollectionAssert.AreEqual(new[] { "local", "nearby" }, observed);
		unrelated.Verify(x => x.HandleEvent(EventType.CharacterSpeaksWitness, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Relay_ClosedDoorAddsDifficultyAndCheckCallbackDepartureCancelsBothDeliveryAndEvent()
	{
		var f = new Fixture(); f.Door.SetupGet(x => x.IsOpen).Returns(false);
		Assert.IsTrue(f.Relay());
		f.Check.Verify(x => x.Check(f.Listener.Object, Difficulty.Normal.StageUp(1), f.Source.Object, null,
			0.0, TraitUseType.Practical, It.IsAny<(string, object)[]>()), Times.Once);
		f.Output.Invocations.Clear(); f.Listener.Invocations.Clear();
		f.Check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(), It.IsAny<IPerceivable>(),
			It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Callback(() => f.Listener.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericListenCheck, Outcome.Pass));
		Assert.IsFalse(f.Relay());
		f.Output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Never);
		f.Listener.Verify(x => x.HandleEvent(EventType.CharacterSpeaksNearbyWitness, It.IsAny<object[]>()), Times.Never);
	}

	[TestMethod]
	public void Relay_DoesNotDuplicateMultipleExitsOrRelayToSelfLoopAndPreservesCapturedOrigin()
	{
		var f = new Fixture();
		var loop = new Mock<IRoomExit>(); loop.SetupGet(x => x.Origin).Returns(f.Origin.Object); loop.SetupGet(x => x.Destination).Returns(f.Origin.Object);
		f.Origin.Setup(x => x.ExitsFor(f.Source.Object, false)).Returns([loop.Object, f.Exit.Object, f.Exit.Object]);
		f.Destination.SetupGet(x => x.Characters).Returns([f.Listener.Object]);
		PassiveSpeechRelay.Relay(f.Body.Object, f.Speech, f.Origin.Object);
		f.Output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Once);
		f.Output.Invocations.Clear(); f.Body.SetupGet(x => x.Location).Returns(Mock.Of<IRoom>());
		PassiveSpeechRelay.Relay(f.Body.Object, f.Speech, f.Origin.Object);
		f.Output.Verify(x => x.Send(It.IsAny<IOutput>(), true, false), Times.Never);
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Source { get; } = new();
		public Mock<ICharacter> Listener { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IRoom> Origin { get; } = new();
		public Mock<IRoom> Destination { get; } = new();
		public Mock<IRoomExit> Exit { get; } = new();
		public Mock<IDoor> Door { get; } = new();
		public Mock<ICheck> Check { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public SpokenLanguageInfo Speech { get; }
		public Fixture()
		{
			var world = World; world.Setup(x => x.GetCheck(CheckType.GenericListenCheck)).Returns(Check.Object);
			Source.SetupGet(x => x.Location).Returns(Origin.Object);
			Body.SetupGet(x => x.Actor).Returns(Source.Object); Body.SetupGet(x => x.Location).Returns(Origin.Object);
			Listener.SetupGet(x => x.Location).Returns(Destination.Object); Listener.SetupGet(x => x.Gameworld).Returns(world.Object);
			Listener.SetupGet(x => x.State).Returns(CharacterState.Awake); Listener.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Listener.Setup(x => x.CanHear(Source.Object)).Returns(true);
			Destination.Setup(x => x.LocalAudioDifficulty(Listener.Object, AudioVolume.Quiet, Proximity.VeryDistant)).Returns(Difficulty.Normal);
			Exit.SetupGet(x => x.Origin).Returns(Origin.Object); Exit.SetupGet(x => x.Destination).Returns(Destination.Object);
			Exit.SetupGet(x => x.InboundDirectionSuffix).Returns("from the north");
			var connection = new Mock<IExit>(); connection.SetupGet(x => x.Door).Returns(Door.Object); Exit.SetupGet(x => x.Exit).Returns(connection.Object);
			Door.SetupGet(x => x.IsOpen).Returns(true);
			Output.Setup(x => x.Send(It.IsAny<IOutput>(), true, false)).Returns(true);
			var language = new Mock<ILanguage> { DefaultValue = DefaultValue.Mock };
			Speech = new SpokenLanguageInfo(language.Object, Mock.Of<IAccent>(), AudioVolume.Decent, "A secret.", Outcome.Pass, Source.Object, null!);
			Enable(); SetOutcome(Outcome.Pass);
		}
		public void Enable() => Listener.Setup(x => x.CombinedEffectsOfType<IPassiveListeningEffect>()).Returns([new ListeningAwarenessStatus(Listener.Object, 10)]);
		public void SetOutcome(Outcome outcome) => Check.Setup(x => x.Check(It.IsAny<IPerceivableHaveTraits>(), It.IsAny<Difficulty>(),
			It.IsAny<IPerceivable>(), It.IsAny<IUseTrait>(), It.IsAny<double>(), It.IsAny<TraitUseType>(), It.IsAny<(string, object)[]>()))
			.Returns(CheckOutcome.SimpleOutcome(CheckType.GenericListenCheck, outcome));
		public bool Relay() => PassiveSpeechRelay.TryRelay(Body.Object, Listener.Object, Exit.Object, Speech);
	}
}
