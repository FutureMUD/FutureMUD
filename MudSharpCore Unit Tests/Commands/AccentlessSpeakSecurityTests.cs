#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character;
using MudSharp.Commands.Modules;
using MudSharp.Communication.Language;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests.Commands;

[TestClass]
public class AccentlessSpeakSecurityTests
{
	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<ILanguage> Language { get; } = new();
		public List<string> Messages { get; } = new();
		public Fixture()
		{
			Language.SetupGet(x => x.Name).Returns("test language");
			Language.SetupGet(x => x.Accents).Returns(Array.Empty<IAccent>());
			Actor.SetupGet(x => x.Languages).Returns(new[] { Language.Object });
			Actor.SetupGet(x => x.Accents).Returns(Array.Empty<IAccent>());
			Actor.SetupProperty(x => x.CurrentLanguage, Language.Object);
			Actor.SetupProperty(x => x.CurrentAccent, null!);
			var output = new Mock<IOutputHandler>();
			output.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<bool>()))
				.Callback<string, bool, bool>((text, _, _) => Messages.Add(text)).Returns(true);
			Actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		}
		public void Speak(string input) => typeof(CommunicationsModule)
			.GetMethod("Speak", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [Actor.Object, input]);
	}

	[TestMethod]
	public void Speak_StatusForAccentlessLanguage_DoesNotThrowOrInventAnAccent()
	{
		var f = new Fixture();
		f.Speak("speak");
		StringAssert.Contains(f.Messages[0], "You are currently speaking");
		StringAssert.Contains(f.Messages[0], "Test language");
		Assert.IsNull(f.Actor.Object.CurrentAccent);
		f.Actor.Verify(x => x.LearnAccent(It.IsAny<IAccent>(), It.IsAny<Difficulty>()), Times.Never);
	}

	[TestMethod]
	public void Speak_SelectAccentlessLanguage_ClearsPreviousLanguagesAccent()
	{
		var f = new Fixture();
		f.Actor.Object.CurrentLanguage = Mock.Of<ILanguage>();
		f.Actor.Object.CurrentAccent = Mock.Of<IAccent>();
		f.Speak("speak test");
		Assert.AreSame(f.Language.Object, f.Actor.Object.CurrentLanguage);
		Assert.IsNull(f.Actor.Object.CurrentAccent);
		StringAssert.Contains(f.Messages[0], "You will now speak in");
	}

	[TestMethod]
	public void Speak_ExplicitUnknownAccent_StillRejectsWithoutChangingSelection()
	{
		var f = new Fixture();
		var previous = Mock.Of<ILanguage>();
		var accent = Mock.Of<IAccent>();
		f.Actor.Object.CurrentLanguage = previous;
		f.Actor.Object.CurrentAccent = accent;
		f.Speak("speak test nonexistent");
		StringAssert.Contains(f.Messages[0], "You do not know that accent");
		Assert.AreSame(previous, f.Actor.Object.CurrentLanguage);
		Assert.AreSame(accent, f.Actor.Object.CurrentAccent);
	}

	[TestMethod]
	public void Speak_StatusWithConfiguredAcquisitionAccent_StillLearnsAndDisplaysIt()
	{
		var f = new Fixture();
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Language).Returns(f.Language.Object);
		accent.SetupGet(x => x.AccentSuffix).Returns("with a familiar accent");
		f.Language.SetupGet(x => x.Accents).Returns(new[] { accent.Object });
		f.Actor.Setup(x => x.AcquisitionAccent(f.Language.Object)).Returns(accent.Object);
		f.Speak("speak");
		Assert.AreSame(accent.Object, f.Actor.Object.CurrentAccent);
		StringAssert.Contains(f.Messages[0], "with a familiar accent.");
		f.Actor.Verify(x => x.LearnAccent(accent.Object, Difficulty.Normal), Times.Once);
	}
}
