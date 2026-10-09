#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.CommunicationStrategies;
using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class EmoteSpeechSecurityTests
{
	[DataTestMethod]
	[DataRow(PermitLanguageOptions.LanguageIsMuffling)]
	[DataRow(PermitLanguageOptions.LanguageIsGasping)]
	[DataRow(PermitLanguageOptions.LanguageIsChoking)]
	[DataRow(PermitLanguageOptions.LanguageIsBabbling)]
	[DataRow(PermitLanguageOptions.LanguageIsBuzzing)]
	[DataRow(PermitLanguageOptions.LanguageIsClicking)]
	public void Emote_ReplacementSpeech_DoesNotEmitHiddenWords(PermitLanguageOptions option)
	{
		var f = new Fixture();
		new Strategy(option, true).Emote(f.Body.Object, "@ attempts, \"secret command\"");
		f.VerifySpeech(0);
	}

	[DataTestMethod]
	[DataRow("@ says, \"secret command\"")]
	[DataRow("@ says, \"secret command")]
	public void Emote_SpeechForbidden_RejectsBeforeAnySpeechEvents(string emote)
	{
		var f = new Fixture();
		new Strategy(PermitLanguageOptions.PermitLanguage, true).Emote(f.Body.Object, emote, false);
		f.VerifySpeech(0);
		f.Output.Verify(x => x.Send(It.IsAny<string>(), true, false), Times.Once);
	}

	[TestMethod]
	public void Emote_UnableToVocalise_DoesNotEmitSpeech()
	{
		var f = new Fixture();
		new Strategy(PermitLanguageOptions.PermitLanguage, false).Emote(f.Body.Object, "@ tries, \"secret command\"");
		f.VerifySpeech(0);
	}

	[TestMethod]
	public void Emote_Gagged_DoesNotEmitSpeech()
	{
		var f = new Fixture();
		var mouth = TestObjectFactory.CreateUninitialized<MouthProto>();
		f.Body.SetupGet(x => x.Bodyparts).Returns([mouth]);
		var gag = new Mock<IGameItem>();
		gag.Setup(x => x.IsItemType<IGag>()).Returns(true);
		f.Body.Setup(x => x.WornItemsFor(mouth)).Returns([gag.Object]);
		new Strategy(PermitLanguageOptions.PermitLanguage, true).Emote(f.Body.Object, "@ tries, \"secret command\"");
		f.VerifySpeech(0);
	}

	[TestMethod]
	public void Emote_PermittedSpeech_EmitsWordsToActorWitnessAndItem()
	{
		var f = new Fixture();
		new Strategy(PermitLanguageOptions.PermitLanguage, true).Emote(f.Body.Object, "@ says, \"hello there\"");
		f.VerifySpeech(1);
		f.Actor.Verify(x => x.HandleEvent(EventType.CharacterSpeaks,
			It.Is<object[]>(args => (string)args[4] == "Hello there.")), Times.Once);
	}

	private sealed class Strategy(PermitLanguageOptions option, bool canVocalise) : HumanoidCommunicationStrategy
	{
		public override PermitLanguageOptions VocalisationOption(IBody body, AudioVolume volume) => option;
		public override bool CanVocalise(IBody body, AudioVolume volume) => canVocalise;
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IOutputHandler> Output { get; } = new();
		private Mock<IHandleEvents> Witness { get; } = new();
		private Mock<IGameItem> Item { get; } = new();

		public Fixture()
		{
			var language = new Mock<ILanguage>();
			language.SetupGet(x => x.Model).Returns(Mock.Of<ILanguageDifficultyModel>());
			var accent = Mock.Of<IAccent>();
			var world = new Mock<IFuturemud>();
			var check = new Mock<ICheck>();
			check.SetReturnsDefault(CheckOutcome.SimpleOutcome(CheckType.SpokenLanguageSpeakCheck, Outcome.Pass));
			world.Setup(x => x.GetCheck(CheckType.SpokenLanguageSpeakCheck)).Returns(check.Object);
			Actor.SetupGet(x => x.Gameworld).Returns(world.Object);
			Actor.SetupGet(x => x.CurrentLanguage).Returns(language.Object);
			Actor.SetupGet(x => x.CurrentAccent).Returns(accent);
			Body.SetupGet(x => x.Actor).Returns(Actor.Object);
			Body.SetupGet(x => x.OutputHandler).Returns(Output.Object);
			Body.SetupGet(x => x.CurrentLanguage).Returns(language.Object);
			Body.SetupGet(x => x.CurrentAccent).Returns(accent);
			var room = new Mock<IRoom>();
			room.SetupGet(x => x.EventHandlers).Returns([Actor.Object, Witness.Object]);
			Body.SetupGet(x => x.Location).Returns(room.Object);
			Body.SetupGet(x => x.ExternalItems).Returns([Item.Object]);
		}

		public void VerifySpeech(int count)
		{
			Actor.Verify(x => x.HandleEvent(EventType.CharacterSpeaks, It.IsAny<object[]>()), Times.Exactly(count));
			Witness.Verify(x => x.HandleEvent(EventType.CharacterSpeaksWitness, It.IsAny<object[]>()), Times.Exactly(count));
			Item.Verify(x => x.HandleEvent(EventType.CharacterSpeaksWitness, It.IsAny<object[]>()), Times.Exactly(count));
		}
	}
}
