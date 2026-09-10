using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Communication.Language;
using MudSharp.Framework;
using MudSharp.NPC.Templates;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class NativeLanguageCreationTests
{
	[TestMethod]
	public void LanguageAcquisitionSeesAccentsAddedAfterLanguageWasLoaded()
	{
		var accents = new List<IAccent>();
		var repository = new Mock<IUneditableAll<IAccent>>();
		repository.Setup(x => x.GetEnumerator()).Returns(() => accents.GetEnumerator());
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		world.SetupGet(x => x.Accents).Returns(repository.Object);
		var language = new MudSharp.Communication.Language.Language(new MudSharp.Models.Language { Id = 9, Name = "Example" }, world.Object);
		Assert.IsFalse(language.Accents.Any());
		var accent = new Mock<IAccent>();
		accent.SetupGet(x => x.Language).Returns(language);
		accent.SetupGet(x => x.Role).Returns(AccentRole.Fallback);
		accents.Add(accent.Object);
		Assert.AreSame(accent.Object, LanguageAcquisition.ResolveAccent(language, null));
	}

	[TestMethod]
	public void AccentStoryboardOldXmlDefaultsOffAndNewSettingRoundTrips()
	{
		var world = Mock.Of<IFuturemud>();
		var xml = XElement.Parse("<Definition><Blurb>Choose accents</Blurb><NumberOfPicks>1</NumberOfPicks></Definition>");
		var old = new AccentPickerScreenStoryboard(world, new MudSharp.Models.ChargenScreenStoryboard { Id = 1, StageDefinition = xml.ToString() });
		Assert.IsFalse(old.SelectNativeLanguage);
		xml.Add(new XElement("SelectNativeLanguage", true));
		var enabled = new AccentPickerScreenStoryboard(world, new MudSharp.Models.ChargenScreenStoryboard { Id = 1, StageDefinition = xml.ToString() });
		Assert.IsTrue(enabled.SelectNativeLanguage);
		var saved = (string)typeof(AccentPickerScreenStoryboard).GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(enabled, null)!;
		Assert.IsTrue(new AccentPickerScreenStoryboard(world, new MudSharp.Models.ChargenScreenStoryboard { Id = 1, StageDefinition = saved }).SelectNativeLanguage);
	}

	[TestMethod]
	public void SimpleTemplateAcceptsMissingNativeLanguageAndReadsOptionalOverride()
	{
		var world = new Mock<IFuturemud> { DefaultValue = DefaultValue.Mock };
		var native = Mock.Of<ILanguage>();
		world.Setup(x => x.Languages.Get(0)).Returns((ILanguage)null!);
		world.Setup(x => x.Languages.Get(99)).Returns(native);
		world.Setup(x => x.Calendars.Get(1).GetDate("test")).Returns((MudSharp.TimeAndDate.Date.MudDate)null!);
		var xml = XElement.Parse("""
			<Character>
			  <SelectedAccents/><SelectedAttributes/><SelectedBirthday>1_test</SelectedBirthday>
			  <SelectedCharacteristics/><SelectedCulture>0</SelectedCulture><SelectedDisfigurements/>
			  <SelectedEntityDescriptionPatterns/><SelectedEthnicity>0</SelectedEthnicity>
			  <SelectedFullDesc>Example</SelectedFullDesc><SelectedGender>0</SelectedGender><SelectedHeight>1</SelectedHeight>
			  <SelectedKnowledges/><SelectedMerits/><SelectedName><Name culture="0"/></SelectedName>
			  <SelectedProstheses/><SelectedRace>0</SelectedRace><SelectedRoles/><SelectedSdesc>example</SelectedSdesc>
			  <SelectedStartingLocation>0</SelectedStartingLocation><SelectedWeight>1</SelectedWeight>
			  <SkillValues/><Handedness>0</Handedness><MissingBodyparts/>
			</Character>
			""");
		Assert.IsNull(new SimpleCharacterTemplate(xml, world.Object).SelectedNativeLanguage);
		xml.Add(new XElement("SelectedNativeLanguage", 99));
		var template = new SimpleCharacterTemplate(xml, world.Object);
		Assert.AreSame(native, template.SelectedNativeLanguage);
		Assert.AreSame(native, (template with { }).SelectedNativeLanguage);
	}
}
