#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseSeeder.Seeders.CultureToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CultureToolkitNamingTests
{
	private static Mock<IFuturemud> World()
	{
		var world = new Mock<IFuturemud>();
		world.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
		world.SetupGet(x => x.RandomNameProfiles).Returns(new All<IRandomNameProfile>());
		return world;
	}

	[TestMethod]
	public void DeliveredFixturesUseActualParserAllStylesAndXmlRoundTrip()
	{
		var catalogue = new CultureToolkitCatalogue();
		var repertoires = new[] { "antiquity", "darkages", "medieval", "renaissance", "earlymodern" }
			.SelectMany(era => CultureToolkitNameCatalogue.Build(catalogue, era)).ToArray();
		foreach (var fixture in catalogue.Document("data.naming_pattern_tests.json").GetProperty("fixtures").EnumerateArray())
		{
			var pool = fixture.GetProperty("pool").GetString();
			var birth = fixture.GetProperty("birth").GetString()!;
			var byname = fixture.GetProperty("byname").GetString()!;
			var full = fixture.GetProperty("expected_full").GetString()!;
			var repertoire = repertoires.First(x => x.StableKey == pool && x.Culture.RandomNameProfiles
				.Any(p => p.RandomNameProfilesElements.Any(e => e.Name == birth)));
			repertoire.Culture.Id = 1;
			var culture = new NameCulture(repertoire.Culture, World().Object);
			var parsed = culture.GetPersonalName(full, true);
			Assert.IsNotNull(parsed, full);
			var structured = new PersonalName(culture, new Dictionary<NameUsage, List<string>>
			{
				[NameUsage.BirthName] = [birth], [NameUsage.Surname] = [byname]
			}, true);
			var restored = new PersonalName(culture, parsed.SaveToXml());
			foreach (var style in Enum.GetValues<NameStyle>())
			{
				var expected = style switch
				{
					NameStyle.GivenOnly or NameStyle.Affectionate => birth,
					NameStyle.SurnameOnly => byname.Length > 0 ? byname : birth,
					_ => full
				};
				Assert.AreEqual(expected, parsed.GetName(style), $"{pool}:{style}");
				Assert.AreEqual(expected, structured.GetName(style));
				Assert.AreEqual(expected, restored.GetName(style));
			}
			Assert.AreEqual(parsed, culture.GetPersonalName(parsed.GetName(NameStyle.FullName), true));
			Assert.IsTrue(culture.PreserveNameCase);
			Assert.IsTrue(new NameCulture(new MudSharp.Models.NameCulture
			{
				Definition = culture.SaveToXml().ToString(), Name = "Round trip"
			}, World().Object).PreserveNameCase);
		}
	}

	[TestMethod]
	public void NewProfilesGenerateWithinDeliveredPoolsWithoutInventedOptionalBynames()
	{
		var catalogue = new CultureToolkitCatalogue();
		foreach (var era in new[] { "darkages", "medieval", "renaissance", "earlymodern" })
		foreach (var repertoire in CultureToolkitNameCatalogue.Build(catalogue, era))
		{
			var culture = new NameCulture(repertoire.Culture, World().Object);
			foreach (var profile in culture.RandomNameProfiles)
			{
				Assert.IsTrue(profile.IsReady, $"{era}:{profile.Name}");
				var source = repertoire.Culture.RandomNameProfiles.Single(x => x.Name == profile.Name)
					.RandomNameProfilesElements.Where(x => x.NameUsage == (int)NameUsage.BirthName)
					.Select(x => x.Name).ToHashSet();
				for (var i = 0; i < 20; i++)
				{
					var name = profile.GetRandomPersonalName(true).GetName(NameStyle.FullName);
					Assert.IsTrue(source.Contains(name), $"{era}:{profile.Name}:{name}");
				}
			}
			if (repertoire.StableKey == "names.target.old-prussian")
			{
				Assert.IsFalse(culture.RandomNameProfiles.Any(x => x.Gender == Gender.Female));
				Assert.IsTrue(repertoire.Exclusions.Any(x => x.Contains(":female:")));
			}
		}
	}

	[DataTestMethod]
	[DataRow("Bar Sauma", "", "Bar Sauma")]
	[DataRow("e\u0301mile", "de la Roche", "émile de la Roche")]
	public void NamePickerUsesEthnicityFirstAndPreservesStructuredCaseWithUnicodeOff(string birth, string byname, string full)
	{
		var world = World();
		var culture = new NameCulture(new MudSharp.Models.NameCulture
		{
			Name = "Local", Definition = CultureToolkitNameCatalogue.Definition("Choose a name.", [birth])
		}, world.Object);
		var ethnicity = new Mock<MudSharp.Character.Heritage.IEthnicity>();
		ethnicity.Setup(x => x.NameCultureForGender(Gender.Male)).Returns(culture);
		ethnicity.SetupGet(x => x.ChargenAdvices).Returns([]);
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.InnerLineFormatLength).Returns(80);
		var chargen = new Mock<IChargen>();
		chargen.SetupGet(x => x.Account).Returns(account.Object);
		chargen.SetupGet(x => x.Gameworld).Returns(world.Object);
		chargen.SetupGet(x => x.SelectedGender).Returns(Gender.Male);
		chargen.SetupGet(x => x.SelectedEthnicity).Returns(ethnicity.Object);
		chargen.SetupGet(x => x.SelectedRoles).Returns([]);
		chargen.SetupProperty(x => x.SelectedName);
		// No selected culture: reaching completion proves the ethnicity path wins.
		var storyboard = new NamePickerScreenStoryboard(world.Object, new MudSharp.Models.ChargenScreenStoryboard
		{
			StageDefinition = "<Definition><AllowUnicodeNames>false</AllowUnicodeNames></Definition>"
		});
		var screen = storyboard.GetScreen(chargen.Object);
		StringAssert.Contains(screen.HandleCommand("Łukasz"), "may not contain Unicode");
		screen.HandleCommand(birth);
		screen.HandleCommand(byname);
		Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		Assert.AreEqual(full, chargen.Object.SelectedName.GetName(NameStyle.FullName));
	}

	[TestMethod]
	public void NamePickerDisplay_ProfileWithoutOptionalBynamePool_DoesNotRequestMissingElement()
	{
		var world = World();
		var culture = new NameCulture(new MudSharp.Models.NameCulture
		{
			Name = "Local", Definition = CultureToolkitNameCatalogue.Definition("Choose a name.", ["Jonas"])
		}, world.Object);
		var profile = new Mock<IRandomNameProfile>();
		profile.SetupGet(x => x.Culture).Returns(culture);
		profile.SetupGet(x => x.RandomNames).Returns(new Dictionary<NameUsage, List<(string Value, int Weight)>>
		{
			[NameUsage.BirthName] = [("Jonas", 100)]
		});
		profile.Setup(x => x.IsCompatibleGender(Gender.Male)).Returns(true);
		profile.Setup(x => x.UseForChargenNameSuggestions(It.IsAny<ICharacterTemplate>())).Returns(true);
		profile.Setup(x => x.GetRandomNameElement(NameUsage.BirthName)).Returns("Jonas");
		world.SetupGet(x => x.RandomNameProfiles).Returns(new All<IRandomNameProfile> { profile.Object });
		var ethnicity = new Mock<MudSharp.Character.Heritage.IEthnicity>();
		ethnicity.Setup(x => x.NameCultureForGender(Gender.Male)).Returns(culture);
		ethnicity.SetupGet(x => x.ChargenAdvices).Returns([]);
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.InnerLineFormatLength).Returns(80);
		var chargen = new Mock<IChargen>();
		chargen.SetupGet(x => x.Account).Returns(account.Object);
		chargen.SetupGet(x => x.Gameworld).Returns(world.Object);
		chargen.SetupGet(x => x.SelectedGender).Returns(Gender.Male);
		chargen.SetupGet(x => x.SelectedEthnicity).Returns(ethnicity.Object);
		chargen.SetupGet(x => x.SelectedRoles).Returns([]);
		var storyboard = new NamePickerScreenStoryboard(world.Object, new MudSharp.Models.ChargenScreenStoryboard
		{
			StageDefinition = "<Definition><AllowUnicodeNames>true</AllowUnicodeNames></Definition>"
		});

		var screen = storyboard.GetScreen(chargen.Object);
		var bynameDisplay = screen.HandleCommand("Jonas");

		StringAssert.Contains(bynameDisplay, "Byname Selection");
		profile.Verify(x => x.GetRandomNameElement(NameUsage.Surname), Times.Never);
	}
}
