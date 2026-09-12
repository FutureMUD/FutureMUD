#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Communication.Language;
using MudSharp.Communication.Language.DifficultyModels;
using MudSharp.Form.Audio;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;
using MudSharp.PerceptionEngine.Outputs;
using MudSharp.PerceptionEngine.Parsers;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests.PerceptionEngine;

[TestClass]
public class EmoteFormatTests
{
	[DataTestMethod]
	[DataRow("{")]
	[DataRow("}")]
	[DataRow("{0}")]
	[DataRow("{99}")]
	[DataRow("{the gate}")]
	public void ParseFor_MalformedInternalFormat_ReturnsAnErrorInsteadOfThrowing(string text)
	{
		var source = Mock.Of<IPerceiver>();
		var original = new Emote(text, source);
		IEmote[] emotes =
		[
			original,
			new NoFormatEmote(text, source),
			new NoLanguageEmote(text, source),
			Emote.LoadEmote(original.SaveToXml(), Mock.Of<IFuturemud>(), source)
		];

		foreach (var emote in emotes)
		{
			StringAssert.StartsWith(new EmoteOutput(emote).ParseFor(source), "Invalid emote");
		}
	}

	[DataTestMethod]
	[DataRow("{")]
	[DataRow("}")]
	[DataRow("{0}")]
	[DataRow("{99}")]
	[DataRow("{{the gate}}")]
	public void PlayerEmote_ExplicitPerceivables_PreservesLiteralBracesAndSourceTokens(string text)
	{
		var source = new Mock<IPerceiver>();
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		var emote = new PlayerEmote($"@ gesture|gestures toward {text}.", source.Object, source.Object);

		Assert.IsTrue(emote.Valid, emote.ErrorMessage);
		Assert.AreEqual($"you gesture toward {text}.", emote.ParseFor(source.Object).RawText());
	}

	[DataTestMethod]
	[DataRow("{")]
	[DataRow("}")]
	[DataRow("{0}")]
	[DataRow("{99}")]
	[DataRow("{{the gate}}")]
	public void LanguageOutput_SpeechAndParentheticalEmote_PreserveLiteralBraces(string text)
	{
		var source = new Mock<ILanguagePerceiver>();
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		source.SetupGet(x => x.InnerLineFormatLength).Returns(200);
		source.SetupGet(x => x.Accents).Returns([]);
		var arguments = new StringStack($"(pointing toward {text}) Remember {text}.");
		var emote = new PlayerEmote(arguments.PopParentheses(), source.Object);
		var language = new Mock<ILanguage>();
		language.SetupGet(x => x.Name).Returns("Common");
		language.SetupGet(x => x.Model).Returns(Mock.Of<ILanguageDifficultyModel>());
		var speech = new SpokenLanguageInfo(language.Object, Mock.Of<IAccent>(), AudioVolume.Decent,
			arguments.SafeRemainingArgument, Outcome.MajorPass, source.Object, null);
		var output = new LanguageOutput(new Emote("@ say|says", source.Object), speech, emote);

		Assert.IsTrue(output.AllValid);
		var rendered = output.ParseFor(source.Object).RawText();
		StringAssert.Contains(rendered, $"pointing toward {text}");
		StringAssert.Contains(rendered, $"Remember {text}.");
	}

	[DataTestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void ParseFor_MalformedNestedEmote_ReturnsAnErrorInsteadOfThrowing(bool targetPresent)
	{
		var source = Mock.Of<IPerceiver>();
		var emote = new Emote("$?0|unclosed {|unmatched }|$", source,
			new IPerceivable[] { targetPresent ? source : null! });

		StringAssert.StartsWith(emote.ParseFor(source), "Invalid emote");
	}

	[TestMethod]
	public void ParseFor_EscapedBracesAndTargetTokens_RenderForEachViewer()
	{
		var source = new Mock<IPerceiver>();
		var observer = new Mock<IPerceiver>();
		source.Setup(x => x.IsSelf(source.Object)).Returns(true);
		var emote = new Emote("$0|your|their {0} {{message}}".Sanitise(), source.Object, source.Object);

		Assert.AreEqual("your {0} {{message}}", emote.ParseFor(source.Object));
		Assert.AreEqual("their {0} {{message}}", emote.ParseFor(observer.Object));
	}
}
