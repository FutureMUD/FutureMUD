#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Character.Name;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.CharacterCreation.Roles;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests;

[TestClass]
public class NameCulturePolicyTests
{
	private sealed class Fixture
	{
		internal readonly Mock<IFuturemud> World = new();
		internal readonly Mock<IChargen> Chargen = new();
		internal readonly All<IFutureProg> Progs = new();
		internal readonly All<INameCulture> Cultures = new();
		internal Fixture()
		{
			World.SetupGet(x => x.FutureProgs).Returns(Progs);
			World.SetupGet(x => x.NameCultures).Returns(Cultures);
			World.SetupGet(x => x.RandomNameProfiles).Returns(new All<IRandomNameProfile>());
			World.SetupGet(x => x.SaveManager).Returns(Mock.Of<ISaveManager>());
			World.SetupGet(x => x.AlwaysFalseProg).Returns(Mock.Of<IFutureProg>());
			Chargen.SetupGet(x => x.Gameworld).Returns(World.Object);
			Chargen.SetupProperty(x => x.SelectedName);
			Chargen.SetupProperty(x => x.Account, Mock.Of<IAccount>(x => x.InnerLineFormatLength == 80));
			Chargen.SetupGet(x => x.SelectedGender).Returns(Gender.Male);
			Chargen.SetupProperty(x => x.SelectedRoles, new List<IChargenRole>());
		}
		internal NameCulture Culture(long id, bool optIn)
		{
			var definition = new XElement("NameCulture",
				new XElement("PreserveNameCase", optIn), new XElement("ExactRandomProfileGender", optIn), new XElement("AllowTypographicPunctuation", optIn),
				new XElement("Patterns", Enumerable.Range(0, 6).Select(x => new XElement("Pattern", new XAttribute("Style", x), new XAttribute("Text", "{0}"), new XAttribute("Params", "0")))),
				new XElement("Elements", new XElement("Element", new XAttribute("Usage", 0), new XAttribute("MinimumCount", 1), new XAttribute("MaximumCount", 1), new XAttribute("Name", "Personal name"), "Enter a personal name.")),
				new XElement("CuratedNamingDefinition", new XCData("{\"clan_requires_parent\":true,\"random_enabled\":false}")),
				new XElement("NameEntryRegex", new XCData(@"^(?<birthname>[\p{L} '’‐-]+)$")));
			var culture = new NameCulture(new MudSharp.Models.NameCulture { Id = id, Name = "Style " + id, Definition = definition.ToString() }, World.Object);
			Cultures.Add(culture); return culture;
		}
		internal void DefaultCulture(INameCulture culture)
		{
			var upbringing = new Mock<ICulture>();
			upbringing.SetupGet(x => x.ChargenAdvices).Returns(Array.Empty<IChargenAdvice>());
			upbringing.Setup(x => x.NameCultureForGender(It.IsAny<Gender>())).Returns(culture);
			Chargen.SetupGet(x => x.SelectedCulture).Returns(upbringing.Object);
		}
		internal NamePickerScreenStoryboard Storyboard(long stylesProg = 0, bool unicode = true) => new(World.Object,
			new MudSharp.Models.ChargenScreenStoryboard { Id = 1, StageDefinition = $"<Screen><AllowUnicodeNames>{unicode.ToString().ToLowerInvariant()}</AllowUnicodeNames><NameCultureOptionsProg>{stylesProg}</NameCultureOptionsProg></Screen>" });
	}

	[TestMethod]
	public void NameCulture_RoundTripRetainsFlagsExactSpellingAndCuratedConstraints()
	{
		var f = new Fixture(); var culture = f.Culture(1, true);
		var xml = culture.SaveToXml();
		Assert.AreEqual("true", xml.Element("ExactRandomProfileGender")!.Value);
		Assert.AreEqual("{\"clan_requires_parent\":true,\"random_enabled\":false}", xml.Element("CuratedNamingDefinition")!.Value);
		var reloaded = new NameCulture(new MudSharp.Models.NameCulture { Id = 2, Name = "Reloaded", Definition = xml.ToString() }, f.World.Object);
		foreach (var spelling in new[] { "Ka’Cha", "Ka’Ka’Kyl", "Klik‐Chaka’da", "Al’Raam’ke", "T’Chai" })
		{
			var name = reloaded.GetPersonalName(spelling, true);
			Assert.IsNotNull(name); Assert.AreEqual(spelling, name.GetName(NameStyle.FullName));
			Assert.AreEqual(spelling, new PersonalName(reloaded, name.SaveToXml()).GetName(NameStyle.FullName));
		}
	}

	[TestMethod]
	public void RandomProfiles_ExactGenderOptInDoesNotChangeLegacyWildcardPolicy()
	{
		var f = new Fixture();
		foreach (var optIn in new[] { false, true })
		{
			var culture = f.Culture(optIn ? 2 : 1, optIn);
			foreach (var stored in Enumerable.Range(0, 5).Select(x => (Gender)x))
			foreach (var requested in Enumerable.Range(0, 5).Select(x => (Gender)x))
			{
				var profile = new RandomNameProfile(new MudSharp.Models.RandomNameProfile { Id = 3, Name = "Test", Gender = (int)stored }, culture);
				Assert.AreEqual(stored == requested || !optIn && stored is Gender.NonBinary or Gender.Indeterminate, profile.IsCompatibleGender(requested));
			}
		}
	}

	[TestMethod]
	public void NamePicker_OptionalStyleChangesOnlyNameAndPersistsChosenCultureOnResume()
	{
		var f = new Fixture(); var upbringing = f.Culture(1, true); var racial = f.Culture(2, true); f.DefaultCulture(upbringing);
		var styles = new Mock<IFutureProg>(); styles.SetupGet(x => x.Id).Returns(10);
		styles.Setup(x => x.ExecuteCollection<INameCulture>(It.IsAny<object[]>())).Returns(new[] { upbringing, racial }); f.Progs.Add(styles.Object);
		var selectedCulture = f.Chargen.Object.SelectedCulture;
		var storyboard = f.Storyboard(10); var screen = storyboard.GetScreen(f.Chargen.Object);
		StringAssert.Contains(screen.HandleCommand("/style 2"), "Style 2");
		StringAssert.Contains(screen.HandleCommand("Klik‐Chaka’da"), "Klik‐Chaka’da");
		Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		Assert.AreSame(selectedCulture, f.Chargen.Object.SelectedCulture);
		Assert.AreSame(racial, f.Chargen.Object.SelectedName.Culture);
		Assert.AreEqual("Klik‐Chaka’da", new PersonalName(f.Chargen.Object.SelectedName.SaveToXml(), f.World.Object).GetName(NameStyle.FullName));
		StringAssert.Contains(storyboard.GetScreen(f.Chargen.Object).Display(), "Naming style: ");
		StringAssert.Contains(storyboard.GetScreen(f.Chargen.Object).Display(), "Style 2");
	}

	[TestMethod]
	public void NamePicker_TypographicPunctuationRequiresOptInAndStillRejectsInvalidSequences()
	{
		foreach (var optIn in new[] { false, true })
		{
			var f = new Fixture(); f.DefaultCulture(f.Culture(1, optIn));
			var screen = f.Storyboard().GetScreen(f.Chargen.Object);
			var result = screen.HandleCommand("Ka’Cha");
			Assert.AreEqual(optIn ? ChargenScreenState.Complete : ChargenScreenState.Incomplete, screen.State, result);
		}
		foreach (var invalid in new[] { "Ka’‐Cha", "Ka’ ’Cha", "Ka<Cha", "Ka1Cha", "’‐" })
		{
			var f = new Fixture(); f.DefaultCulture(f.Culture(1, true));
			var screen = f.Storyboard().GetScreen(f.Chargen.Object); screen.HandleCommand(invalid);
			Assert.AreEqual(ChargenScreenState.Incomplete, screen.State, invalid);
		}
	}

	[TestMethod]
	public void RandomProfileBuilder_WeightAndAddPreserveExactCaseAndPunctuation()
	{
		var f = new Fixture(); var culture = f.Culture(1, true);
		var model = new MudSharp.Models.RandomNameProfile { Id = 2, Name = "Test" };
		model.RandomNameProfilesElements.Add(new MudSharp.Models.RandomNameProfilesElements { NameUsage = 0, Name = "Ka’Cha", Weighting = 1 });
		var profile = new RandomNameProfile(model, culture);
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		Assert.IsTrue(profile.BuildingCommand(actor.Object, new StringStack("birthname weight Ka’Cha 9")));
		Assert.AreEqual(("Ka’Cha", 9), profile.RandomNames[NameUsage.BirthName].Single());
		Assert.IsTrue(profile.BuildingCommand(actor.Object, new StringStack("birthname add t’Chai 3")));
		Assert.IsTrue(profile.RandomNames[NameUsage.BirthName].Any(x => x.Value == "t’Chai" && x.Weight == 3));
		Assert.IsFalse(profile.BuildingCommand(actor.Object, new StringStack("birthname remove ka’cha")));
		Assert.IsTrue(profile.BuildingCommand(actor.Object, new StringStack("birthname remove Ka’Cha")));
	}
}
