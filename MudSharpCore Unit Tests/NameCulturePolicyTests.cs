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
		internal NameCulture Culture(long id, bool optIn, NameCultureElement[]? components = null)
		{
			var definition = new XElement("NameCulture",
				new XElement("UseStructuredEntry", components is not null), new XElement("PreserveNameCase", optIn), new XElement("ExactRandomProfileGender", optIn), new XElement("AllowTypographicPunctuation", optIn),
				new XElement("Patterns", Enumerable.Range(0, 6).Select(x => new XElement("Pattern", new XAttribute("Style", x), new XAttribute("Text", components is null || x == 0 ? "{0}" : x == 5 ? "{4} {0} {1} {3} ?2[{2}]" : "{4} {0} {1} {3}"), new XAttribute("Params", "0,6,2,9,18")))),
				new XElement("Elements", new XElement("Element", new XAttribute("Usage", 0), new XAttribute("MinimumCount", 1), new XAttribute("MaximumCount", 1), new XAttribute("Name", "Personal name"), "Enter a personal name."), components?.Select(x => x.SaveToXml())),
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

	private static NameCultureElement Component(NameUsage usage, string name, int maximum = 1) => new()
	{ Usage = usage, Name = name, ChargenBlurb = "Approved " + name + " rules.", MinimumCount = 0, MaximumCount = maximum };

	private static NameCultureElement Tribe => Component(NameUsage.FamilyGroupName, "Tribe") with
	{
		AllowedValues = new[] { "Night Runner Tribe", "Silt Stalker Tribe" }, DefaultDisplay = false,
		ConfirmationPrompt = "Confirm background membership; no membership is granted.", DisplayChoicePrompt = "Display tribe as a surname?"
	};
	private static NameCultureElement Clan => Component(NameUsage.ClanName, "Clan") with
	{
		ParentUsage = NameUsage.FamilyGroupName, ParentValues = new Dictionary<string, string[]>
		{ ["Night Runner Tribe"] = new[] { "Dark Moons" }, ["Silt Stalker Tribe"] = new[] { "Fire Bow" } }
	};

	[TestMethod]
	public void StructuredEntry_OptionalComponentsPersistWithoutConflatingStatusOrAliases()
	{
		var f = new Fixture();
		var culture = f.Culture(1, true, new[] { Component(NameUsage.Surname, "Family"), Component(NameUsage.Nickname, "Alias", -1),
			Component(NameUsage.Title, "Title"), Component(NameUsage.Affiliation, "Affiliation") });
		var name = culture.GetPersonalName("/elements birthname Agis surname Al\u2019Raam\u2019ke nickname \"Silt Runner\" nickname Dust-Cutter title Captain affiliation \"Some House\"", true);
		Assert.IsNotNull(name);
		Assert.AreEqual("Agis Al\u2019Raam\u2019ke", name.GetName(NameStyle.FullName));
		var xml = name.SaveToXml();
		CollectionAssert.AreEqual(new[] { "Silt Runner", "Dust-Cutter" }, xml.Elements("Element").Where(x => x.Attribute("usage")!.Value == "Nickname").Select(x => x.Value).ToArray());
		Assert.AreEqual("Captain", xml.Elements("Element").Single(x => x.Attribute("usage")!.Value == "Title").Value);
		Assert.AreEqual(name.GetName(NameStyle.FullWithNickname), new PersonalName(culture, xml).GetName(NameStyle.FullWithNickname));
		StringAssert.Contains(name.GetName(NameStyle.FullWithNickname), "\"Silt Runner\" \"Dust-Cutter\"");
		Assert.AreEqual(-1, new NameCultureElement(culture.SaveToXml().Element("Elements")!.Elements().Single(x => x.Attribute("Usage")!.Value == "2")).MaximumCount);
	}

	[TestMethod]
	public void StructuredEntry_TribeRequiresDeclarationAndIndependentDisplayChoiceAndValidClan()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Tribe, Clan });
		const string root = "/elements birthname Agis familygroupname \"Night Runner Tribe\"";
		Assert.IsNull(culture.GetPersonalName(root, true));
		Assert.IsNull(culture.GetPersonalName(root + " confirm familygroupname clanname \"Fire Bow\"", true));
		Assert.IsNull(culture.GetPersonalName("/elements birthname Agis clanname \"Dark Moons\"", true));
		var hidden = culture.GetPersonalName(root + " confirm familygroupname clanname \"Dark Moons\"", true)!;
		Assert.AreEqual("Agis", hidden.GetName(NameStyle.FullName));
		var shown = culture.GetPersonalName(root + " confirm familygroupname display familygroupname clanname \"Dark Moons\"", true)!;
		Assert.AreEqual("Agis Night Runner Tribe", shown.GetName(NameStyle.FullName));
		Assert.AreNotEqual(hidden, shown);
		Assert.AreEqual(shown, new PersonalName(culture, shown.SaveToXml()));
		Assert.AreEqual("Agis Night Runner Tribe", new PersonalName(culture, shown.SaveToXml()).GetName(NameStyle.FullName));
	}

	[TestMethod]
	public void StructuredEntry_EarnedNameRequiresConfirmationAndPrefixesPersonalName()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Component(NameUsage.EarnedName, "Path") with
		{ AllowedValues = new[] { "Aandu" }, ConfirmationPrompt = "Confirm completed apprenticeship; grants no training." } });
		Assert.IsNull(culture.GetPersonalName("/elements birthname Ptek earnedname Aandu", true));
		var name = culture.GetPersonalName("/elements birthname Ptek earnedname Aandu confirm earnedname", true)!;
		Assert.AreEqual("Aandu Ptek", name.GetName(NameStyle.FullName));
		Assert.AreEqual("Ptek", name.GetName(NameStyle.GivenOnly));
		Assert.IsNull(culture.GetPersonalName("/elements birthname Ptek earnedname Invented confirm earnedname", true));
		var unconfirmed = name.SaveToXml(); unconfirmed.Elements("Element").Single(x => x.Attribute("usage")!.Value == "EarnedName").Attribute("confirmed")!.Remove();
		Assert.AreEqual("Ptek", new PersonalName(culture, unconfirmed).GetName(NameStyle.FullName));
	}

	[TestMethod]
	public void NamePicker_PendingTribeConfirmationAndDisplaySurviveNameXmlResume()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Component(NameUsage.Nickname, "Alias", -1), Tribe, Clan }); f.DefaultCulture(culture);
		var storyboard = f.Storyboard(); var screen = storyboard.GetScreen(f.Chargen.Object);
		screen.HandleCommand("Agis"); screen.HandleCommand("\"Silt Runner\" Dust-Cutter");
		StringAssert.Contains(screen.HandleCommand("Night Runner Tribe"), "Confirm background");
		f.Chargen.Object.SelectedName = new PersonalName(culture, f.Chargen.Object.SelectedName.SaveToXml());
		screen = storyboard.GetScreen(f.Chargen.Object);
		StringAssert.Contains(screen.Display(), "Confirm background");
		StringAssert.Contains(screen.HandleCommand("yes"), "Display tribe");
		f.Chargen.Object.SelectedName = new PersonalName(culture, f.Chargen.Object.SelectedName.SaveToXml());
		screen = storyboard.GetScreen(f.Chargen.Object); StringAssert.Contains(screen.Display(), "Display tribe");
		screen.HandleCommand("yes"); StringAssert.Contains(screen.HandleCommand("Fire Bow"), "selected parent");
		screen.HandleCommand("Dark Moons"); Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		Assert.AreEqual("Agis Night Runner Tribe", f.Chargen.Object.SelectedName.GetName(NameStyle.FullName));
		Assert.AreEqual(2, f.Chargen.Object.SelectedName.SaveToXml().Elements("Element").Count(x => x.Attribute("usage")!.Value == "Nickname"));
		Assert.IsNull(f.Chargen.Object.SelectedName.EntryProgress);
	}

	[TestMethod]
	public void NamePicker_DecliningConfirmationAllowsBlankAndRandomDefaultsRemainPersonalOnly()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Tribe, Clan }); f.DefaultCulture(culture);
		var screen = f.Storyboard().GetScreen(f.Chargen.Object); screen.HandleCommand("Agis"); screen.HandleCommand("Night Runner Tribe");
		screen.HandleCommand("no"); screen.HandleCommand(""); screen.HandleCommand("");
		Assert.AreEqual(ChargenScreenState.Complete, screen.State); Assert.AreEqual("Agis", f.Chargen.Object.SelectedName.Name);
		var model = new MudSharp.Models.RandomNameProfile { Id = 5, Name = "Personal only" };
		model.RandomNameProfilesElements.Add(new() { NameUsage = 0, Name = "Agis", Weighting = 1 });
		model.RandomNameProfilesDiceExpressions.Add(new() { NameUsage = 0, DiceExpression = "1" });
		model.RandomNameProfilesDiceExpressions.Add(new() { NameUsage = 9, DiceExpression = "0" });
		model.RandomNameProfilesDiceExpressions.Add(new() { NameUsage = 17, DiceExpression = "0" });
		var profile = new RandomNameProfile(model, culture); Assert.IsTrue(profile.IsReady);
		Assert.AreEqual("Agis", profile.GetRandomPersonalName(true).Name);
	}

	[TestMethod]
	public void StructuredEntry_RulesAndExistingEnumValuesRoundTripWithoutChangingLegacyParsing()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Tribe, Clan });
		var loaded = new NameCulture(new MudSharp.Models.NameCulture { Id = 2, Name = "Reload", Definition = culture.SaveToXml().ToString() }, f.World.Object);
		Assert.IsTrue(loaded.UseStructuredEntry);
		Assert.AreEqual("Jos\u00e9", loaded.GetPersonalName("/elements birthname Jose\u0301", true)!.GetName(NameStyle.FullName));
		var clan = loaded.NameCultureElements.Single(x => x.Usage == NameUsage.ClanName);
		Assert.AreEqual("Approved Clan rules.", clan.ChargenBlurb);
		CollectionAssert.AreEqual(new[] { "Dark Moons" }, clan.ParentValues["Night Runner Tribe"]);
		Assert.AreEqual(14, (int)NameUsage.OwnerName); Assert.AreEqual(0, (int)NameUsage.BirthName);
		var legacy = f.Culture(3, false);
		Assert.IsFalse(legacy.UseStructuredEntry); Assert.IsNull(legacy.GetPersonalName("/elements birthname Agis", true));
		Assert.AreEqual("Agis", legacy.GetPersonalName("Agis", true)!.Name);
	}

	[TestMethod]
	public void RandomProfile_ZeroRequiredDiceRemainStoredAndUnreadyRatherThanGeneratingInvalidNames()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Component(NameUsage.Nickname, "Alias", -1), Tribe, Clan });
		var model = new MudSharp.Models.RandomNameProfile { Id = 9, Name = "Suppressed" };
		model.RandomNameProfilesDiceExpressions.Add(new() { NameUsage = 0, DiceExpression = "0" });
		var profile = new RandomNameProfile(model, culture);
		Assert.IsFalse(profile.IsReady); Assert.AreEqual("0", profile.NameUsageDiceExpressions[NameUsage.BirthName]);
		model.RandomNameProfilesDiceExpressions.Single().DiceExpression = "1d2";
		Assert.IsFalse(new RandomNameProfile(model, culture).IsReady, "Unsupported variable counts remain stored but unready.");
		Assert.ThrowsException<InvalidOperationException>(() => profile.GetRandomPersonalName(true));
	}

	[TestMethod]
	public void NamePicker_RemovedSavedStyleRestartsRatherThanApplyingProgressToFallback()
	{
		var f = new Fixture(); var fallback = f.Culture(1, true, new[] { Component(NameUsage.FamilyGroupName, "Group") });
		var elf = f.Culture(2, true, new[] { Tribe, Clan }); f.DefaultCulture(elf);
		var screen = f.Storyboard().GetScreen(f.Chargen.Object); screen.HandleCommand("Agis"); screen.HandleCommand("Night Runner Tribe"); screen.HandleCommand("yes");
		Assert.IsNotNull(f.Chargen.Object.SelectedName.EntryProgress);
		f.DefaultCulture(fallback);
		screen = f.Storyboard().GetScreen(f.Chargen.Object);
		StringAssert.Contains(screen.Display(), "Personal name Selection"); Assert.IsNull(f.Chargen.Object.SelectedName);
		screen.HandleCommand("Newname"); screen.HandleCommand(""); Assert.AreEqual(ChargenScreenState.Complete, screen.State);
	}

	[TestMethod]
	public void NameCulture_BuilderCanRestoreUnlimitedAliasCardinality()
	{
		var f = new Fixture(); var culture = f.Culture(1, true, new[] { Component(NameUsage.Nickname, "Alias", -1) });
		var actor = new Mock<ICharacter>(); actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
		Assert.IsTrue(culture.BuildingCommand(actor.Object, new StringStack("element nickname max 2")));
		Assert.IsTrue(culture.BuildingCommand(actor.Object, new StringStack("element nickname max unlimited")));
		Assert.AreEqual(-1, culture.NameCultureElements.Single(x => x.Usage == NameUsage.Nickname).MaximumCount);
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
