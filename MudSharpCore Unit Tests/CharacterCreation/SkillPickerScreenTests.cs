#nullable enable

using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body.Traits;
using MudSharp.Character;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.FutureProg;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
public class SkillPickerScreenTests
{
	[TestMethod]
	public void CanSelectSuggestedSkill_ValidLanguageSkill_ReturnsTrue()
	{
		var skill = Skill(TraitType.Skill, false, "language");

		var result = SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			skill, [], [skill], [], 1.0);

		Assert.IsTrue(result);
	}

	[TestMethod]
	public void CanSelectSuggestedSkill_InvalidSuggestion_ReturnsFalse()
	{
		var freeSkill = Skill();
		var hiddenSkill = Skill(hidden: true);
		var attribute = Skill(TraitType.Attribute);
		var unavailableSkill = Skill();
		var selectedSkill = Skill();
		var available = new[] { freeSkill, hiddenSkill, attribute, selectedSkill };

		Assert.IsFalse(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			freeSkill, [freeSkill], available, [freeSkill], 2.0));
		Assert.IsFalse(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			hiddenSkill, [], available, [], 2.0));
		Assert.IsFalse(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			attribute, [], available, [], 2.0));
		Assert.IsFalse(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			unavailableSkill, [], available, [], 2.0));
		Assert.IsFalse(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			selectedSkill, [], available, [selectedSkill], 2.0));
	}

	[TestMethod]
	public void CanSelectSuggestedSkill_ProgOrderAndPickLimit_SelectsFirstDistinctSkills()
	{
		var first = Skill();
		var second = Skill();
		var third = Skill();
		var selected = new List<ITraitDefinition>();
		var available = new[] { first, second, third };

		foreach (var suggestion in new[] { first, first, second, third })
		{
			if (SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
					suggestion, [], available, selected, 2.0))
			{
				selected.Add(suggestion);
			}
		}

		CollectionAssert.AreEqual(new[] { first, second }, selected);
		selected.Remove(first);
		Assert.IsTrue(SkillPickerScreenStoryboard.SkillPickerScreen.CanSelectSuggestedSkill(
			first, [], available, selected, 2.0));
	}

	[TestMethod]
	public void Storyboard_LegacyXml_LoadsAndSavesWithoutSuggestedProg()
	{
		foreach (var value in new string?[] { null, "0", "none" })
		{
			Assert.IsNull(StoryboardFixture(value).Storyboard.SuggestedSkillsProg);
		}

		var fixture = StoryboardFixture();
		Assert.IsNull(fixture.Storyboard.SuggestedSkillsProg);
		var saved = XElement.Parse(SaveDefinition(fixture.Storyboard));
		Assert.AreEqual("0", saved.Element("SuggestedSkillsProg")?.Value);
	}

	[TestMethod]
	public void Storyboard_SuggestedProg_RoundTripsThroughXml()
	{
		var fixture = StoryboardFixture("3");

		Assert.AreSame(fixture.SuggestedProg, fixture.Storyboard.SuggestedSkillsProg);
		var saved = XElement.Parse(SaveDefinition(fixture.Storyboard));
		Assert.AreEqual("3", saved.Element("SuggestedSkillsProg")?.Value);
	}

	[TestMethod]
	public void BuildingCommandSuggestedSkills_SetsAndClearsProg()
	{
		var fixture = StoryboardFixture();
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(output.Object);
		actor.Setup(x => x.IsAdministrator(It.IsAny<MudSharp.Accounts.PermissionLevel>())).Returns(true);

		Assert.IsTrue(fixture.Storyboard.BuildingCommand(actor.Object,
			new StringStack("suggestedskills SuggestedSkills")));
		Assert.AreSame(fixture.SuggestedProg, fixture.Storyboard.SuggestedSkillsProg);
		Assert.IsTrue(fixture.Storyboard.BuildingCommand(actor.Object,
			new StringStack("suggestedskills none")));
		Assert.IsNull(fixture.Storyboard.SuggestedSkillsProg);
	}

	private static ITraitDefinition Skill(TraitType type = TraitType.Skill, bool hidden = false, string group = "")
	{
		var skill = new Mock<ITraitDefinition>();
		skill.SetupGet(x => x.TraitType).Returns(type);
		skill.SetupGet(x => x.Hidden).Returns(hidden);
		skill.SetupGet(x => x.Group).Returns(group);
		return skill.Object;
	}

	private static (SkillPickerScreenStoryboard Storyboard, IFutureProg SuggestedProg) StoryboardFixture(
		string? suggestedProgValue = null)
	{
		var picksProg = new Mock<IFutureProg>();
		var freeSkillsProg = new Mock<IFutureProg>();
		var suggestedProg = new Mock<IFutureProg>();
		suggestedProg.SetupGet(x => x.Id).Returns(3);
		suggestedProg.SetupGet(x => x.FunctionName).Returns("SuggestedSkills");
		suggestedProg.SetupGet(x => x.ReturnType)
			.Returns(ProgVariableTypes.Collection | ProgVariableTypes.Trait);
		suggestedProg.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);

		var progs = new Mock<IUneditableAll<IFutureProg>>();
		progs.Setup(x => x.Get(1)).Returns(picksProg.Object);
		progs.Setup(x => x.Get(2)).Returns(freeSkillsProg.Object);
		progs.Setup(x => x.Get(3)).Returns(suggestedProg.Object);
		progs.Setup(x => x.GetByIdOrName("SuggestedSkills", It.IsAny<bool>())).Returns(suggestedProg.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.FutureProgs).Returns(progs.Object);
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);

		var suggestedElement = suggestedProgValue is null
			? ""
			: $"<SuggestedSkillsProg>{suggestedProgValue}</SuggestedSkillsProg>";
		var model = new MudSharp.Models.ChargenScreenStoryboard
		{
			Id = 1,
			StageDefinition =
				$"<Definition><Blurb><![CDATA[Test]]></Blurb><NumberOfSkillPicksProg>1</NumberOfSkillPicksProg><FreeSkillsProg>2</FreeSkillsProg>{suggestedElement}</Definition>"
		};

		return (new SkillPickerScreenStoryboard(gameworld.Object, model), suggestedProg.Object);
	}

	private static string SaveDefinition(SkillPickerScreenStoryboard storyboard)
	{
		return (string)typeof(SkillPickerScreenStoryboard)
			.GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(storyboard, null)!;
	}
}
