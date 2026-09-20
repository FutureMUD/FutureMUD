#nullable enable

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.CharacterCreation.Screens;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.CharacterCreation;

[TestClass]
public class RerollableCharacteristicPickerScreenTests
{
	[TestMethod]
	public void Screen_InitialRollAndContinue_AcceptsRolledValue()
	{
		var brown = Value("Chestnut", "brown");
		var fixture = CreateFixture(true, "Choose your look", brown);

		StringAssert.Contains(fixture.Screen.Display(), "Choose your look");
		Assert.IsFalse(fixture.Screen.Display().Contains("Chestnut"));
		StringAssert.Contains(fixture.Screen.HandleCommand("continue"), "Chestnut");
		Assert.AreEqual(string.Empty, fixture.Screen.HandleCommand("continue"));
		Assert.AreEqual(ChargenScreenState.Complete, fixture.Screen.State);
		Assert.AreSame(brown, fixture.Chargen.Object.SelectedCharacteristics[0].Item2);
	}

	[TestMethod]
	public void Screen_FullLockAndManualChoice_RerollKeepsChosenValueUntilUnlock()
	{
		var first = Value("Chestnut", "brown");
		var second = Value("Black", "black");
		var fixture = CreateFixture(false, "", first, second);
		fixture.Profile.SetupSequence(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>()))
			.Returns(first).Returns(first).Returns(first);

		StringAssert.Contains(fixture.Screen.HandleCommand("lock hair"), "[locked]");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Chestnut");
		fixture.Screen.HandleCommand("hair");
		StringAssert.Contains(fixture.Screen.HandleCommand("Black"), "[locked]");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Black");
		fixture.Screen.HandleCommand("unlock hair");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Chestnut");
	}

	[TestMethod]
	public void Screen_BasicLockAndReplacement_RerollsWithinCurrentBasicValue()
	{
		var chestnut = Value("Chestnut", "brown");
		var auburn = Value("Auburn", "brown");
		var black = Value("Black", "black");
		var fixture = CreateFixture(false, "", chestnut, auburn, black);
		fixture.Profile.SetupSequence(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>()))
			.Returns(black);

		StringAssert.Contains(fixture.Screen.HandleCommand("basiclock hair"), "[basic lock: brown]");
		for (var i = 0; i < 10; i++)
		{
			var output = fixture.Screen.HandleCommand("reroll");
			Assert.IsTrue(output.Contains("Chestnut") || output.Contains("Auburn"));
			Assert.IsFalse(output.Contains("Black"));
		}

		fixture.Screen.HandleCommand("hair");
		StringAssert.Contains(fixture.Screen.HandleCommand("Auburn"), "[basic lock: brown]");
		StringAssert.Contains(fixture.Screen.HandleCommand("lock hair"), "[locked]");
		StringAssert.Contains(fixture.Screen.HandleCommand("basiclock hair"), "[basic lock: brown]");
		fixture.Screen.HandleCommand("unlock hair");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Black");
	}

	[TestMethod]
	public void Screen_NoEligibleValues_BlocksCompletionAndExplainsSelection()
	{
		var fixture = CreateFixture(false, "", []);

		StringAssert.Contains(fixture.Screen.Display(), "Not Selected");
		StringAssert.Contains(fixture.Screen.HandleCommand("continue"), "must select valid values");
		Assert.AreNotEqual(ChargenScreenState.Complete, fixture.Screen.State);
	}

	[TestMethod]
	public void Screen_InlineBlurb_DisplaysAboveRoll()
	{
		var fixture = CreateFixture(false, "Choose your look", Value("Chestnut", "brown"));
		var display = fixture.Screen.Display();

		Assert.IsTrue(display.IndexOf("Choose your look", StringComparison.Ordinal) <
		              display.IndexOf("Chestnut", StringComparison.Ordinal));
	}

	[TestMethod]
	public void Screen_ManualChoiceUnderBasicLock_ReanchorsBasicValue()
	{
		var chestnut = Value("Chestnut", "brown");
		var auburn = Value("Auburn", "brown");
		var black = Value("Black", "black");
		var fixture = CreateFixture(false, "", chestnut, auburn, black);

		fixture.Screen.HandleCommand("basiclock hair");
		fixture.Screen.HandleCommand("hair");
		StringAssert.Contains(fixture.Screen.HandleCommand("Black"), "[basic lock: black]");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Black");
	}

	[TestMethod]
	public void Screen_NumberedManualChoice_UsesDisplayedOrder()
	{
		var chestnut = Value("Chestnut", "brown");
		var auburn = Value("Auburn", "brown");
		var fixture = CreateFixture(false, "", chestnut, auburn);

		var options = fixture.Screen.HandleCommand("hair");
		StringAssert.Contains(options, "1:");
		StringAssert.Contains(options, "Auburn");
		StringAssert.Contains(fixture.Screen.HandleCommand("1"), "Auburn");
	}

	[TestMethod]
	public void Screen_BasicLockWithoutVariant_RejectsLock()
	{
		var fixture = CreateFixture(false, "", Value("Black", "black"));

		StringAssert.Contains(fixture.Screen.HandleCommand("basiclock hair"), "no alternative eligible values");
		Assert.IsFalse(fixture.Screen.Display().Contains("[basic lock:"));
	}

	[TestMethod]
	public void Screen_LegacyRandomCommand_DoesNotBypassLock()
	{
		var first = Value("Chestnut", "brown");
		var second = Value("Black", "black");
		var fixture = CreateFixture(false, "", first, second);
		fixture.Profile.Setup(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>())).Returns(second);
		fixture.Screen.HandleCommand("lock hair");

		StringAssert.Contains(fixture.Screen.HandleCommand("random"), "not a valid characteristic");
		StringAssert.Contains(fixture.Screen.HandleCommand("reroll"), "Chestnut");
	}

	[TestMethod]
	public void Storyboard_MissingIntroSetting_DefaultsToSeparateBlurb()
	{
		var dbitem = new MudSharp.Models.ChargenScreenStoryboard
		{
			StageDefinition = new XElement("Definition", new XElement("Blurb", "Introduction")).ToString()
		};
		var storyboard = (RerollableCharacteristicsPickerScreenStoryboard)Activator.CreateInstance(
			typeof(RerollableCharacteristicsPickerScreenStoryboard),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[new Mock<IFuturemud>().Object, dbitem],
			null)!;

		Assert.IsTrue(storyboard.SeparateBlurb);
		Assert.AreEqual("Introduction", storyboard.Blurb);
	}

	[TestMethod]
	public void Storyboard_DisabledIntroSetting_LoadsAsInlineBlurb()
	{
		var dbitem = new MudSharp.Models.ChargenScreenStoryboard
		{
			StageDefinition = new XElement("Definition",
				new XElement("Blurb", "Introduction"),
				new XElement("SeparateBlurb", false)).ToString()
		};
		var storyboard = (RerollableCharacteristicsPickerScreenStoryboard)Activator.CreateInstance(
			typeof(RerollableCharacteristicsPickerScreenStoryboard),
			BindingFlags.Instance | BindingFlags.NonPublic,
			null,
			[new Mock<IFuturemud>().Object, dbitem],
			null)!;

		Assert.IsFalse(storyboard.SeparateBlurb);
		Assert.AreEqual(typeof(RerollableCharacteristicsPickerScreenStoryboard),
			typeof(RerollableCharacteristicsPickerScreenStoryboard).GetMethod("RegisterFactory")!.DeclaringType);
	}

	private static (RerollableCharacteristicsPickerScreenStoryboard.RerollableCharacteristicPickerScreen Screen,
		Mock<IChargen> Chargen, Mock<ICharacteristicProfile> Profile) CreateFixture(
		bool separateBlurb, string blurb, params ICharacteristicValue[] values)
	{
		var definition = new Mock<ICharacteristicDefinition>();
		definition.SetupGet(x => x.Name).Returns("Hair");
		definition.SetupGet(x => x.Pattern).Returns(new Regex("^hair$", RegexOptions.IgnoreCase));
		definition.SetupGet(x => x.ChargenDisplayType).Returns(CharacterGenerationDisplayType.DisplayAll);
		var race = new Mock<IRace>();
		race.Setup(x => x.Characteristics(It.IsAny<Gender>()))
			.Returns([definition.Object]);
		race.SetupGet(x => x.ChargenAdvices).Returns([]);
		var profile = new Mock<ICharacteristicProfile>();
		profile.SetupGet(x => x.Values).Returns(values);
		profile.Setup(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>()))
			.Returns(values.Length == 0 ? null! : values[0]);
		var ethnicity = new Mock<IEthnicity>();
		ethnicity.SetupGet(x => x.CharacteristicChoices)
			.Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicProfile>
			{
				[definition.Object] = profile.Object
			});
		ethnicity.SetupGet(x => x.ChargenAdvices).Returns([]);
		var account = new Mock<IAccount>();
		account.SetupGet(x => x.LineFormatLength).Returns(80);
		account.SetupGet(x => x.InnerLineFormatLength).Returns(76);
		var chargen = new Mock<IChargen>();
		chargen.SetupAllProperties();
		chargen.SetupGet(x => x.SelectedRace).Returns(race.Object);
		chargen.SetupGet(x => x.SelectedEthnicity).Returns(ethnicity.Object);
		chargen.SetupGet(x => x.Account).Returns(account.Object);
		chargen.SetupGet(x => x.SelectedRoles).Returns([]);
		var storyboard = (RerollableCharacteristicsPickerScreenStoryboard)Activator.CreateInstance(
			typeof(RerollableCharacteristicsPickerScreenStoryboard), true)!;
		typeof(SimpleCharacteristicsPickerScreenStoryboard).GetProperty("Blurb")!
			.SetValue(storyboard, blurb);
		typeof(RerollableCharacteristicsPickerScreenStoryboard).GetProperty("SeparateBlurb")!
			.SetValue(storyboard, separateBlurb);
		return ((RerollableCharacteristicsPickerScreenStoryboard.RerollableCharacteristicPickerScreen)
			storyboard.GetScreen(chargen.Object), chargen, profile);
	}

	private static ICharacteristicValue Value(string name, string basic)
	{
		var value = new Mock<ICharacteristicValue>();
		value.SetupGet(x => x.Name).Returns(name);
		value.SetupGet(x => x.GetValue).Returns(name);
		value.SetupGet(x => x.GetBasicValue).Returns(basic);
		return value.Object;
	}
}
