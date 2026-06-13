#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Effects.Concrete;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Planes;
using MudSharp.RPG.Merits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

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
	public void SpellEffectFactory_RegistersAstralProjectionEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("astralprojection"));
		var info = SpellEffectFactory.BuilderInfoForType("astralprojection");
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("plane"));
		Assert.IsTrue(info.BuilderHelp.Contains("anchorpolicy"));
		Assert.IsTrue(info.BuilderHelp.Contains("projectionecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("anchorecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("collapseecho"));
		Assert.IsTrue(info.MatchingTriggers.Any());
		Assert.IsFalse(info.Instant);
		Assert.IsFalse(info.RequiresTarget);
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersMagicalCopyCreateCopyEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("createcopy"));
		var info = SpellEffectFactory.BuilderInfoForType("createcopy");
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("focusable"));
		Assert.IsTrue(info.BuilderHelp.Contains("persistent"));
		Assert.IsTrue(info.BuilderHelp.Contains("intangible"));
		Assert.IsTrue(info.BuilderHelp.Contains("plane"));
		Assert.IsTrue(info.BuilderHelp.Contains("collapseecho"));
		Assert.IsTrue(info.MatchingTriggers.Any());
		Assert.IsFalse(info.Instant);
		Assert.IsFalse(info.RequiresTarget);
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersPhysicalCloneCreateCloneEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("createclone"));
		var info = SpellEffectFactory.BuilderInfoForType("createclone");
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("focusable"));
		Assert.IsTrue(info.BuilderHelp.Contains("persistent"));
		Assert.IsTrue(info.BuilderHelp.Contains("deathecho"));
		Assert.IsTrue(info.MatchingTriggers.Any());
		Assert.IsFalse(info.Instant);
		Assert.IsFalse(info.RequiresTarget);
	}

	[TestMethod]
	public void SpellEffectFactory_MagicalCopyAndPhysicalCloneBuilderDefaults()
	{
		var context = CreateCopyCloneSpellContext();

		var (copy, copyError) =
			SpellEffectFactory.LoadEffectFromBuilderInput("createcopy", new StringStack(string.Empty),
				context.Spell.Object);
		var (clone, cloneError) =
			SpellEffectFactory.LoadEffectFromBuilderInput("createclone", new StringStack(string.Empty),
				context.Spell.Object);

		Assert.AreEqual(string.Empty, copyError);
		Assert.AreEqual(string.Empty, cloneError);
		var copyXml = copy.SaveToXml();
		var cloneXml = clone.SaveToXml();
		Assert.AreEqual("createcopy", copyXml.Attribute("type")?.Value);
		Assert.AreEqual("copy", copyXml.Element("FormKey")?.Value);
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), copyXml.Element("PlayerFocusable")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), copyXml.Element("Intangible")?.Value.ToLowerInvariant());
		Assert.AreEqual(context.AstralPlane.Object.Id.ToString(), copyXml.Element("Plane")?.Value);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot.ToString(),
			copyXml.Element("PersistencePolicy")?.Value);
		Assert.AreEqual("createclone", cloneXml.Attribute("type")?.Value);
		Assert.AreEqual("clone", cloneXml.Element("FormKey")?.Value);
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), cloneXml.Element("PlayerFocusable")?.Value.ToLowerInvariant());
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnReboot.ToString(),
			cloneXml.Element("PersistencePolicy")?.Value);
	}

	[TestMethod]
	public void SpellEffectFactory_MagicalCopyXmlRoundTripsConfiguredValues()
	{
		var context = CreateCopyCloneSpellContext();

		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "createcopy"),
			new XElement("FormKey", "decoy"),
			new XElement("Race", context.Race.Object.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", (int)Gender.Female),
			new XElement("Alias", "moonlit duplicate"),
			new XElement("SortOrder", 7),
			new XElement("PlayerFocusable", true),
			new XElement("Intangible", false),
			new XElement("Plane", context.AstralPlane.Object.Id),
			new XElement("PersistencePolicy", CharacterInstancePersistencePolicy.DespawnOnLogout),
			new XElement("CollapseEcho", new XCData("The decoy snaps away.")),
			new XElement("BacklashEcho", new XCData("A silver ache follows."))), context.Spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("createcopy", saved.Attribute("type")?.Value);
		Assert.AreEqual("decoy", saved.Element("FormKey")?.Value);
		Assert.AreEqual(context.Race.Object.Id.ToString(), saved.Element("Race")?.Value);
		Assert.AreEqual(((int)Gender.Female).ToString(), saved.Element("Gender")?.Value);
		Assert.AreEqual("moonlit duplicate", saved.Element("Alias")?.Value);
		Assert.AreEqual("7", saved.Element("SortOrder")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), saved.Element("PlayerFocusable")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), saved.Element("Intangible")?.Value.ToLowerInvariant());
		Assert.AreEqual(context.AstralPlane.Object.Id.ToString(), saved.Element("Plane")?.Value);
		Assert.AreEqual(CharacterInstancePersistencePolicy.DespawnOnLogout.ToString(),
			saved.Element("PersistencePolicy")?.Value);
		Assert.AreEqual("The decoy snaps away.", saved.Element("CollapseEcho")?.Value);
		Assert.AreEqual("A silver ache follows.", saved.Element("BacklashEcho")?.Value);
	}

	[TestMethod]
	public void SpellEffectFactory_PhysicalCloneXmlRoundTripsConfiguredValues()
	{
		var context = CreateCopyCloneSpellContext();

		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "createclone"),
			new XElement("FormKey", "clone-vat"),
			new XElement("Race", context.Race.Object.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", (int)Gender.Male),
			new XElement("Alias", "fresh clone"),
			new XElement("SortOrder", 9),
			new XElement("PlayerFocusable", true),
			new XElement("PersistencePolicy", CharacterInstancePersistencePolicy.Persistent),
			new XElement("DeathEcho", new XCData("The clone body falls away.")),
			new XElement("BacklashEcho", new XCData("A borrowed heartbeat stops."))), context.Spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("createclone", saved.Attribute("type")?.Value);
		Assert.AreEqual("clone-vat", saved.Element("FormKey")?.Value);
		Assert.AreEqual(context.Race.Object.Id.ToString(), saved.Element("Race")?.Value);
		Assert.AreEqual(((int)Gender.Male).ToString(), saved.Element("Gender")?.Value);
		Assert.AreEqual("fresh clone", saved.Element("Alias")?.Value);
		Assert.AreEqual("9", saved.Element("SortOrder")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), saved.Element("PlayerFocusable")?.Value.ToLowerInvariant());
		Assert.AreEqual(CharacterInstancePersistencePolicy.Persistent.ToString(),
			saved.Element("PersistencePolicy")?.Value);
		Assert.AreEqual("The clone body falls away.", saved.Element("DeathEcho")?.Value);
		Assert.AreEqual("A borrowed heartbeat stops.", saved.Element("BacklashEcho")?.Value);
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

	private static CopyCloneSpellContext CreateCopyCloneSpellContext()
	{
		var race = CreateFrameworkItemMock<IRace>(1, "Human");
		var primePlane = CreateFrameworkItemMock<IPlane>(1, "Prime Material");
		var astralPlane = CreateFrameworkItemMock<IPlane>(2, "Astral Plane");
		var races = CreateCollection(race.Object);
		var ethnicities = CreateCollection<IEthnicity>();
		var planes = CreateCollection(primePlane.Object, astralPlane.Object);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.Races).Returns(races.Object);
		gameworld.SetupGet(x => x.Ethnicities).Returns(ethnicities.Object);
		gameworld.SetupGet(x => x.Planes).Returns(planes.Object);
		gameworld.SetupGet(x => x.DefaultPlane).Returns(primePlane.Object);

		var spell = new Mock<IMagicSpell>();
		spell.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		spell.SetupGet(x => x.Id).Returns(9001);
		spell.SetupGet(x => x.Name).Returns("Copy Clone Test Spell");
		spell.SetupProperty(x => x.Changed, false);

		return new CopyCloneSpellContext(spell, race, astralPlane);
	}

	private static Mock<IUneditableAll<T>> CreateCollection<T>(params T[] items) where T : class, IFrameworkItem
	{
		var list = items.ToList();
		var mock = new Mock<IUneditableAll<T>>();
		mock.SetupGet(x => x.Count).Returns(() => list.Count);
		mock.As<IEnumerable<T>>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		mock.As<IEnumerable>().Setup(x => x.GetEnumerator()).Returns(() => list.GetEnumerator());
		mock.Setup(x => x.Get(It.IsAny<long>())).Returns<long>(id => list.FirstOrDefault(x => x.Id == id));
		mock.Setup(x => x.GetByIdOrName(It.IsAny<string>(), It.IsAny<bool>()))
		    .Returns<string, bool>((value, _) => long.TryParse(value, out var id)
			    ? list.FirstOrDefault(x => x.Id == id)
			    : list.FirstOrDefault(x => x.Name.Equals(value, StringComparison.InvariantCultureIgnoreCase)));
		return mock;
	}

	private static Mock<T> CreateFrameworkItemMock<T>(long id, string name) where T : class, IFrameworkItem
	{
		var mock = new Mock<T>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.FrameworkItemType).Returns(typeof(T).Name);
		return mock;
	}

	private sealed record CopyCloneSpellContext(
		Mock<IMagicSpell> Spell,
		Mock<IRace> Race,
		Mock<IPlane> AstralPlane);
}
