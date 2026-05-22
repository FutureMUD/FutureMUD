#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.RPG.Merits;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class BodyFormProvisioningTests
{
	[TestMethod]
	public void CanSee_WhenNoVisibilityProg_ReturnsTrue()
	{
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		var form = new CharacterForm(body.Object, "Wolf");

		Assert.IsTrue(form.CanSee(character.Object));
	}

	[TestMethod]
	public void CanSee_WhenVisibilityProgReturnsFalse_ReturnsFalse()
	{
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		var prog = new Mock<IFutureProg>();
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false);
		var form = new CharacterForm(body.Object, "Wolf")
		{
			CanSeeFormProg = prog.Object
		};

		Assert.IsFalse(form.CanSee(character.Object));
	}

	[TestMethod]
	public void CanSwitchVoluntarily_WhenVoluntarySwitchingDisabled_ReturnsFalseWithDefaultMessage()
	{
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		var form = new CharacterForm(body.Object, "Wolf")
		{
			AllowVoluntarySwitch = false
		};

		var result = form.CanSwitchVoluntarily(character.Object, out var whyNot);

		Assert.IsFalse(result);
		Assert.AreEqual("You cannot voluntarily switch to that form.", whyNot);
	}

	[TestMethod]
	public void CanSwitchVoluntarily_WhenEligibilityProgFails_UsesWhyCantProgMessage()
	{
		var body = new Mock<IBody>();
		var character = new Mock<ICharacter>();
		var canProg = new Mock<IFutureProg>();
		canProg.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false);
		var whyProg = new Mock<IFutureProg>();
		whyProg.Setup(x => x.Execute(It.IsAny<object[]>())).Returns("Moonrise only.");
		var form = new CharacterForm(body.Object, "Wolf")
		{
			AllowVoluntarySwitch = true,
			CanVoluntarilySwitchProg = canProg.Object,
			WhyCannotVoluntarilySwitchProg = whyProg.Object
		};

		var result = form.CanSwitchVoluntarily(character.Object, out var whyNot);

		Assert.IsFalse(result);
		Assert.AreEqual("Moonrise only.", whyNot);
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersTransformFormEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("transformform"));
		var info = SpellEffectFactory.BuilderInfoForType("transformform");
		Assert.IsTrue(info.BuilderHelp.Contains("visibleprog"));
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("echo"));
		Assert.IsTrue(info.BuilderHelp.Contains("sdescpattern"));
		Assert.IsTrue(info.BuilderHelp.Contains("fdescpattern"));
		Assert.IsTrue(info.BuilderHelp.Contains("priorityband"));
		Assert.IsTrue(info.BuilderHelp.Contains("priorityoffset"));
		Assert.IsTrue(info.MatchingTriggers.Any());
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersBodyBackupEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("bodybackup"));
		var info = SpellEffectFactory.BuilderInfoForType("bodybackup");
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("remains"));
		Assert.IsTrue(info.BuilderHelp.Contains("oldecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("newecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("selfecho"));
		Assert.IsTrue(info.MatchingTriggers.Any());
	}

	[TestMethod]
	public void BodyBackupEffect_ParsesContextsAndNormalisesEchoes()
	{
		Assert.IsTrue(BodyBackupEffect.TryParseRemainsContext("spent clone", out var context));
		Assert.AreEqual(BodyRemainsContext.SpentClone, context);
		Assert.IsFalse(BodyBackupEffect.TryParseBackupRemainsContext("final death", out context));
		Assert.AreEqual(BodyRemainsContext.SleeveDeath, context);
		Assert.AreEqual(BodyRemainsContext.SleeveDeath,
			BodyBackupEffect.NormaliseBackupRemainsContext(BodyRemainsContext.FinalCharacterDeath));
		Assert.AreEqual(BodyBackupEffect.DefaultSelfEcho,
			BodyBackupEffect.NormaliseEcho("default", BodyBackupEffect.DefaultSelfEcho));
		Assert.AreEqual(string.Empty,
			BodyBackupEffect.NormaliseEcho("none", BodyBackupEffect.DefaultSelfEcho));
		Assert.AreEqual(BodyBackupEffect.DefaultNoRemainsOldLocationEcho,
			BodyBackupEffect.OldLocationEchoForRemains(BodyBackupEffect.DefaultOldLocationEcho, false));
		Assert.AreEqual("custom $1",
			BodyBackupEffect.OldLocationEchoForRemains("custom $1", false));
	}

	[TestMethod]
	public void MeritFactory_RegistersAdditionalBodyFormMerit()
	{
		MeritFactory.InitialiseMerits();

		Assert.IsTrue(MeritFactory.Types.Contains("Additional Body Form"));
		Assert.IsTrue(MeritFactory.TypeHelps.Any(x =>
			x.Type == "Additional Body Form" &&
			x.HelpText.Contains("visibleprog") &&
			x.HelpText.Contains("echo") &&
			x.HelpText.Contains("sdescpattern") &&
			x.HelpText.Contains("fdescpattern") &&
			x.HelpText.Contains("autotransform") &&
			x.HelpText.Contains("priorityband") &&
			x.HelpText.Contains("recheck")));
	}

	[TestMethod]
	public void FutureProg_RegistersPhase15BodyFormFunctions()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var infos = FutureProg.GetFunctionCompilerInformations().ToList();

		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "canseeform"));
		Assert.AreEqual(3, infos.Count(x => x.FunctionName == "ensureform"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformalias"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformsortorder"));
		Assert.AreEqual(4, infos.Count(x => x.FunctionName == "setformtraumamode"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformtransformationecho"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformtransformationecho"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformshortdescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "randomiseformshortdescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformshortdescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformfulldescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "randomiseformfulldescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformfulldescriptionpattern"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "setformallowvoluntary"));
		Assert.AreEqual(4, infos.Count(x => x.FunctionName == "setformvisibilityprog"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformvisibilityprog"));
		Assert.AreEqual(4, infos.Count(x => x.FunctionName == "setformcanswitchprog"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformcanswitchprog"));
		Assert.AreEqual(4, infos.Count(x => x.FunctionName == "setformwhycantprog"));
		Assert.AreEqual(2, infos.Count(x => x.FunctionName == "clearformwhycantprog"));
		Assert.AreEqual(8, infos.Count(x => x.FunctionName == "readybodybackup"));
		Assert.AreEqual(3, infos.Count(x => x.FunctionName == "clearbodybackup"));
		Assert.AreEqual(3, infos.Count(x => x.FunctionName == "hasbodybackup"));
	}
}
