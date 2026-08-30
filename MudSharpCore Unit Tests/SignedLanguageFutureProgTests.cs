#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Communication.Language;
using MudSharp.FutureProg;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SignedLanguageFutureProgTests
{
	[TestMethod]
	public void SignedVariety_ReturnsFirstClassDotProperties()
	{
		var language = new Mock<ISignedLanguage>();
		var variety = new SignedLanguageVariety(new MudSharp.Models.SignedLanguageVariety
		{
			Id = 42,
			Name = "Sydney",
			Description = "A Sydney variety.",
			Suffix = "with Sydney signs",
			VagueSuffix = "with an unfamiliar regional style",
			RecognitionDifficulty = (int)Difficulty.Normal
		}, language.Object);

		Assert.AreEqual(ProgVariableTypes.SignedVariety, variety.Type);
		Assert.AreSame(language.Object, variety.GetProperty("language"));
		Assert.AreEqual("with Sydney signs", variety.GetProperty("suffix").GetObject);
		Assert.AreEqual((decimal)(int)Difficulty.Normal, variety.GetProperty("difficulty").GetObject);
	}
}
