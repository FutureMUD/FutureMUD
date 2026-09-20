#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Roles;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Editor;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.PerceptionEngine;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
public class DescriptionPickerScreenTests
{
	[TestMethod]
	public void Storyboard_CustomTypeRegistersAndLoadsExistingDefinition()
	{
		Activator.CreateInstance(typeof(ChargenStoryboard), true);
		Assert.IsTrue(ChargenStoryboard.ChargenStageTypeInfos.Any(x =>
			x.Name == "CustomDescriptionPicker" && x.Stage == ChargenStage.SelectDescription));

		var storyboard = CustomStoryboard();
		Assert.AreEqual("Short blurb", storyboard.SDescBlurb);
		Assert.AreEqual("Full blurb", storyboard.FullDescBlurb);
		Assert.IsFalse(storyboard.AllowCustomDescription);
		Assert.IsTrue(storyboard.AllowEntityDescriptionPatterns);
		Assert.AreEqual("false", SaveDefinition(storyboard).Element("AllowCustomDescription")?.Value);
	}

	[TestMethod]
	public void Storyboard_SwitchCopiesBlurbAndSavedChoicesInBothDirections()
	{
		var regular = RegularStoryboard();
		var custom = CustomStoryboard();
		custom.CopyDescriptionSettings(regular);
		Assert.AreEqual(SaveDefinition(regular).ToString(), SaveDefinition(custom).ToString());

		var restored = RegularStoryboard(false, true);
		restored.CopyDescriptionSettings(custom);
		Assert.AreEqual(SaveDefinition(regular).ToString(), SaveDefinition(restored).ToString());
	}

	[TestMethod]
	public void Storyboard_NoCharacteristicsOrDisabledPatterns_UsesCustomEntry()
	{
		var noCharacteristics = ChargenFixture([]);
		Assert.IsTrue(RegularStoryboard(false, true).UseCustomDescriptionsOnly(noCharacteristics.Object));

		var definition = new Mock<ICharacteristicDefinition>().Object;
		var withCharacteristics = ChargenFixture([definition]);
		Assert.IsFalse(RegularStoryboard(false, true).UseCustomDescriptionsOnly(withCharacteristics.Object));
		Assert.IsTrue(RegularStoryboard(true, false).UseCustomDescriptionsOnly(withCharacteristics.Object));
		Assert.IsTrue(CustomStoryboard().UseCustomDescriptionsOnly(withCharacteristics.Object));
	}

	[TestMethod]
	public void CustomEntry_BlankFullDescriptionDoesNotCompleteAndValidTextClearsPatterns()
	{
		var chargen = ChargenFixture([]);
		var screen = CustomStoryboard().GetScreen(chargen.Object);
		Assert.AreEqual("You must enter a short description", screen.HandleCommand("   "));
		Assert.IsTrue(screen.HandleCommand("A TEST PERSON").Contains("custom full description"));
		Assert.AreEqual(ChargenScreenState.Incomplete, screen.State);
		Assert.IsTrue(screen.HandleCommand("continue").Contains("editor"));
		chargen.Verify(x => x.SetEditor(It.IsAny<EditorController>()), Times.Once);

		var output = new Mock<IOutputHandler>();
		CancelFullDescription(screen, output.Object);
		Assert.AreEqual(ChargenScreenState.Incomplete, screen.State);
		PostFullDescription(screen, "   ", output.Object);
		Assert.AreEqual(ChargenScreenState.Incomplete, screen.State);
		PostFullDescription(screen, "A detailed appearance.", output.Object);
		Assert.AreEqual(ChargenScreenState.Complete, screen.State);
		Assert.AreEqual("a test person", chargen.Object.SelectedSdesc);
		Assert.AreEqual("A detailed appearance.", chargen.Object.SelectedFullDesc);
		Assert.AreEqual(0, chargen.Object.SelectedEntityDescriptionPatterns.Count);
	}

	[TestMethod]
	public void ExistingStoryboard_NoCharacteristics_UsesCustomEntryWhenCustomFlagIsOff()
	{
		var chargen = ChargenFixture([]);
		var screen = RegularStoryboard(false, true).GetScreen(chargen.Object);
		Assert.IsTrue(screen.HandleCommand("A PERSON").Contains("custom full description"));
		Assert.AreEqual(ChargenScreenState.Incomplete, screen.State);
	}

	[TestMethod]
	public void CharacteristicPickers_NoDefinitions_CompleteWithEmptySelections()
	{
		var chargen = ChargenFixture([]);
		var sequential = (CharacteristicPickerScreenStoryboard)Activator.CreateInstance(
			typeof(CharacteristicPickerScreenStoryboard), true)!;
		var simple = (SimpleCharacteristicsPickerScreenStoryboard)Activator.CreateInstance(
			typeof(SimpleCharacteristicsPickerScreenStoryboard), true)!;

		Assert.AreEqual(ChargenScreenState.Complete, sequential.GetScreen(chargen.Object).State);
		Assert.AreEqual(0, chargen.Object.SelectedCharacteristics.Count);
		Assert.AreEqual(ChargenScreenState.Complete, simple.GetScreen(chargen.Object).State);
		Assert.AreEqual(0, chargen.Object.SelectedCharacteristics.Count);
	}

	[TestMethod]
	public void SubmissionCharacteristics_EmptyOnlyAllowedForRaceWithoutDefinitions()
	{
		var noCharacteristics = ChargenFixture([]);
		Assert.IsTrue(Chargen.HasRequiredCharacteristics(noCharacteristics.Object));

		var definition = new Mock<ICharacteristicDefinition>().Object;
		var withCharacteristics = ChargenFixture([definition]);
		Assert.IsFalse(Chargen.HasRequiredCharacteristics(withCharacteristics.Object));
		withCharacteristics.Object.SelectedCharacteristics = [(definition, new Mock<ICharacteristicValue>().Object)];
		Assert.IsTrue(Chargen.HasRequiredCharacteristics(withCharacteristics.Object));
	}

	private static Mock<IChargen> ChargenFixture(IEnumerable<ICharacteristicDefinition> definitions)
	{
		var race = new Mock<IRace>();
		race.Setup(x => x.Characteristics(It.IsAny<Gender>())).Returns(definitions);
		race.SetupGet(x => x.ChargenAdvices).Returns([]);
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.InnerLineFormatLength).Returns(80);
		var chargen = new Mock<IChargen>();
		chargen.SetupAllProperties();
		chargen.Object.SelectedRace = race.Object;
		chargen.Object.SelectedGender = Gender.Male;
		chargen.Object.SelectedCharacteristics = [];
		chargen.Object.SelectedEntityDescriptionPatterns = [];
		chargen.Object.SelectedRoles = new List<IChargenRole>();
		chargen.Object.Account = account.Object;
		return chargen;
	}

	private static DescriptionPickerScreenStoryboard RegularStoryboard(bool custom = false, bool patterns = true)
	{
		return new DescriptionPickerScreenStoryboard(new Mock<IFuturemud>().Object, Model(custom, patterns));
	}

	private static CustomDescriptionPickerScreenStoryboard CustomStoryboard()
	{
		return (CustomDescriptionPickerScreenStoryboard)Activator.CreateInstance(
			typeof(CustomDescriptionPickerScreenStoryboard),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[new Mock<IFuturemud>().Object, Model(false, true)],
			null)!;
	}

	private static MudSharp.Models.ChargenScreenStoryboard Model(bool custom, bool patterns) => new()
	{
		Id = 1,
		StageDefinition = $"<Definition><SDescBlurb>Short blurb</SDescBlurb><FullDescBlurb>Full blurb</FullDescBlurb><AllowCustomDescription>{custom}</AllowCustomDescription><AllowEntityDescriptionPatterns>{patterns}</AllowEntityDescriptionPatterns></Definition>"
	};

	private static XElement SaveDefinition(DescriptionPickerScreenStoryboard storyboard) =>
		XElement.Parse((string)typeof(DescriptionPickerScreenStoryboard)
			.GetMethod("SaveDefinition", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(storyboard, null)!);

	private static void PostFullDescription(IChargenScreen screen, string text, IOutputHandler output)
	{
		screen.GetType().GetMethod("PostCustomDescription", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(screen, [text, output, Array.Empty<object>()]);
	}

	private static void CancelFullDescription(IChargenScreen screen, IOutputHandler output)
	{
		screen.GetType().GetMethod("CancelCustomDescription", BindingFlags.Instance | BindingFlags.NonPublic)!
			.Invoke(screen, [output, Array.Empty<object>()]);
	}
}
