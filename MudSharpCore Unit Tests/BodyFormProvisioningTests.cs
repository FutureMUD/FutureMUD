#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Accounts;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.CharacterCreation;
using MudSharp.Form.Characteristics;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;
using MudSharp.Magic.SpellEffects;
using MudSharp.NPC.Templates;
using MudSharp.Planes;
using MudSharp.RPG.Merits;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CharacterClass = MudSharp.Character.Character;

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
	public void SelectProvisionedFormDimensions_DifferentRace_UsesTargetRaceModel()
	{
		var sourceRace = new Mock<IRace>();
		var targetRace = new Mock<IRace>();
		var heightWeightModel = new Mock<IHeightWeightModel>();
		sourceRace.Setup(x => x.SameRace(targetRace.Object)).Returns(false);
		heightWeightModel.Setup(x => x.GetRandomHeightWeight()).Returns((55.0, 12000.0));
		targetRace.Setup(x => x.DefaultHeightWeightModel(Gender.Male)).Returns(heightWeightModel.Object);
		var template = new SimpleCharacterTemplate
		{
			SelectedRace = sourceRace.Object,
			SelectedHeight = 180.0,
			SelectedWeight = 80000.0
		};

		var (height, weight) = CharacterClass.SelectProvisionedFormDimensions(template, targetRace.Object, Gender.Male);

		Assert.AreEqual(55.0, height);
		Assert.AreEqual(12000.0, weight);
	}

	[TestMethod]
	public void SelectProvisionedFormDimensions_SameRace_PreservesSaneSourceDimensions()
	{
		var race = new Mock<IRace>();
		race.Setup(x => x.SameRace(race.Object)).Returns(true);
		var template = new SimpleCharacterTemplate
		{
			SelectedRace = race.Object,
			SelectedHeight = 181.0,
			SelectedWeight = 79000.0
		};

		var (height, weight) = CharacterClass.SelectProvisionedFormDimensions(template, race.Object, Gender.Female);

		Assert.AreEqual(181.0, height);
		Assert.AreEqual(79000.0, weight);
	}

	[TestMethod]
	public void SelectProvisionedFormCharacteristics_MissingTargetRaceCharacteristic_UsesEthnicityProfile()
	{
		var definition = new Mock<ICharacteristicDefinition>();
		definition.SetupGet(x => x.Pattern).Returns(new Regex("^furcolour$", RegexOptions.IgnoreCase));
		definition.SetupGet(x => x.Type).Returns(CharacteristicType.Standard);
		var generatedValue = new Mock<ICharacteristicValue>();
		generatedValue.SetupGet(x => x.GetValue).Returns("black");
		generatedValue.SetupGet(x => x.GetBasicValue).Returns("black");
		generatedValue.SetupGet(x => x.GetFancyValue).Returns("glossy black");
		var profile = new Mock<ICharacteristicProfile>();
		ICharacterTemplate? generatedAgainstTemplate = null;
		profile.Setup(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>()))
		       .Callback<ICharacterTemplate>(template => generatedAgainstTemplate = template)
		       .Returns(generatedValue.Object);
		var race = new Mock<IRace>();
		race.Setup(x => x.Characteristics(Gender.Female)).Returns(new[] { definition.Object });
		var ethnicity = new Mock<IEthnicity>();
		ethnicity.SetupGet(x => x.CharacteristicChoices)
		         .Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicProfile>
		         {
			         { definition.Object, profile.Object }
		         });
		var template = new SimpleCharacterTemplate
		{
			SelectedCharacteristics = [],
			SelectedHeight = 180.0,
			SelectedWeight = 80000.0
		};

		var selected = CharacterClass.SelectProvisionedFormCharacteristics(template, race.Object, ethnicity.Object,
			Gender.Female, 55.0, 12000.0);

		Assert.AreEqual(1, selected.Count);
		Assert.AreSame(definition.Object, selected[0].Item1);
		Assert.AreSame(generatedValue.Object, selected[0].Item2);
		Assert.IsNotNull(generatedAgainstTemplate);
		Assert.AreSame(race.Object, generatedAgainstTemplate!.SelectedRace);
		Assert.AreSame(ethnicity.Object, generatedAgainstTemplate.SelectedEthnicity);
		Assert.AreEqual(Gender.Female, generatedAgainstTemplate.SelectedGender);
		Assert.AreEqual(55.0, generatedAgainstTemplate.SelectedHeight);
		Assert.AreEqual(12000.0, generatedAgainstTemplate.SelectedWeight);

		var pattern = new Mock<IEntityDescriptionPattern>();
		pattern.SetupGet(x => x.Pattern).Returns("a $furcolourbasic mongrel");
		var resolvedTemplate = template with
		{
			SelectedRace = race.Object,
			SelectedEthnicity = ethnicity.Object,
			SelectedGender = Gender.Female,
			SelectedCharacteristics = selected
		};
		Assert.IsTrue(CharacterClass.DescriptionPatternIsUsableForTemplate(pattern.Object, resolvedTemplate));
	}

	[TestMethod]
	public void SelectProvisionedFormCharacteristics_CompatibleExistingValue_PreservesExistingValue()
	{
		var definition = new Mock<ICharacteristicDefinition>();
		var existingValue = new Mock<ICharacteristicValue>();
		definition.Setup(x => x.IsValue(existingValue.Object)).Returns(true);
		var profile = new Mock<ICharacteristicProfile>();
		var race = new Mock<IRace>();
		race.Setup(x => x.Characteristics(Gender.Male)).Returns(new[] { definition.Object });
		var ethnicity = new Mock<IEthnicity>();
		ethnicity.SetupGet(x => x.CharacteristicChoices)
		         .Returns(new Dictionary<ICharacteristicDefinition, ICharacteristicProfile>
		         {
			         { definition.Object, profile.Object }
		         });
		var template = new SimpleCharacterTemplate
		{
			SelectedCharacteristics = [(definition.Object, existingValue.Object)]
		};

		var selected = CharacterClass.SelectProvisionedFormCharacteristics(template, race.Object, ethnicity.Object,
			Gender.Male, 181.0, 79000.0);

		Assert.AreEqual(1, selected.Count);
		Assert.AreSame(existingValue.Object, selected[0].Item2);
		profile.Verify(x => x.GetRandomCharacteristic(It.IsAny<ICharacterTemplate>()), Times.Never);
		profile.Verify(x => x.GetRandomCharacteristic(), Times.Never);
	}

	[TestMethod]
	public void DescriptionPatternIsUsableForTemplate_UnresolvedCharacteristic_ReturnsFalse()
	{
		var pattern = new Mock<IEntityDescriptionPattern>();
		pattern.SetupGet(x => x.Pattern).Returns("a $furcolourbasic mongrel");
		var template = new SimpleCharacterTemplate
		{
			SelectedCharacteristics = new List<(ICharacteristicDefinition, ICharacteristicValue)>(),
			SelectedGender = Gender.Male
		};

		Assert.IsFalse(CharacterClass.DescriptionPatternIsUsableForTemplate(pattern.Object, template));
	}

	[TestMethod]
	public void DescriptionPatternIsUsableForTemplate_KnownCharacteristic_ReturnsTrue()
	{
		var definition = new Mock<ICharacteristicDefinition>();
		definition.SetupGet(x => x.Pattern).Returns(new Regex("^furcolour$", RegexOptions.IgnoreCase));
		definition.SetupGet(x => x.Type).Returns(CharacteristicType.Standard);
		var value = new Mock<ICharacteristicValue>();
		value.SetupGet(x => x.GetValue).Returns("brown");
		value.SetupGet(x => x.GetBasicValue).Returns("brown");
		value.SetupGet(x => x.GetFancyValue).Returns("brown");
		var pattern = new Mock<IEntityDescriptionPattern>();
		pattern.SetupGet(x => x.Pattern).Returns("a $furcolourbasic mongrel");
		var template = new SimpleCharacterTemplate
		{
			SelectedCharacteristics = new List<(ICharacteristicDefinition, ICharacteristicValue)>
			{
				(definition.Object, value.Object)
			},
			SelectedGender = Gender.Male
		};

		Assert.IsTrue(CharacterClass.DescriptionPatternIsUsableForTemplate(pattern.Object, template));
	}

	[TestMethod]
	public void BodyCommand_RegistersDeleteFormWithConfirmation()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Commands", "Modules", "CharacterInformation.cs"));

		StringAssert.Contains(source, "body delform <character> <form> confirm");
		StringAssert.Contains(source, "case \"delform\":");
		StringAssert.Contains(source, "BodyDeleteForm(actor, ss)");
		StringAssert.Contains(source, "This cannot be undone");
		StringAssert.Contains(source, "TryDeleteForm(form, out var whyNot)");
	}

	[TestMethod]
	public void TryDeleteForm_GuardsActiveAndPersistedBodyReferencesBeforeCleanup()
	{
		var source = File.ReadAllText(GetSourcePath("MudSharpCore", "Character", "CharacterForms.cs"));

		StringAssert.Contains(source, "You cannot delete the current body form.");
		StringAssert.Contains(source, "x.IsEmbodied");
		StringAssert.Contains(source, "EffectsOfType<IBodyBackupEffect>()");
		StringAssert.Contains(source, "HasPhysicalReferenceToRetiredBody(body.Id, null)");
		StringAssert.Contains(source, "PersistedInstanceReferencesBody(body, out whyNot)");
		StringAssert.Contains(source, "TryCleanupRetiredBody(body)");
		StringAssert.Contains(source, "_forms.AddRange(removedForms)");
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
		Assert.IsTrue(info.BuilderHelp.Contains("anchorroomecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("projectionroomecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("collapseecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("sdescoverride"));
		Assert.IsTrue(info.MatchingTriggers.Any());
		Assert.IsFalse(info.Instant);
		Assert.IsFalse(info.RequiresTarget);
	}

	[TestMethod]
	public void SpellEffectFactory_AstralProjectionBuilderDefaultsAndXmlRoundTripsEchoesAndSDesc()
	{
		var context = CreateCopyCloneSpellContext();

		var (built, error) =
			SpellEffectFactory.LoadEffectFromBuilderInput("astralprojection", new StringStack(string.Empty),
				context.Spell.Object);

		Assert.AreEqual(string.Empty, error);
		var defaultXml = built.SaveToXml();
		Assert.AreEqual("astralprojection", defaultXml.Attribute("type")?.Value);
		Assert.AreEqual("astral", defaultXml.Element("FormKey")?.Value);
		Assert.AreEqual(string.Empty, defaultXml.Element("AnchorEcho")?.Value);
		Assert.AreEqual(AstralProjectionSpellEffect.DefaultAnchorRoomEcho,
			defaultXml.Element("AnchorRoomEcho")?.Value);
		Assert.AreEqual(AstralProjectionSpellEffect.DefaultProjectionRoomEcho,
			defaultXml.Element("ProjectionRoomEcho")?.Value);
		Assert.AreEqual(string.Empty, defaultXml.Element("ProjectionSDescOverride")?.Value);

		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "astralprojection"),
			new XElement("FormKey", "warlock-spectre"),
			new XElement("Race", context.Race.Object.Id),
			new XElement("Ethnicity", 0L),
			new XElement("Gender", (int)Gender.Indeterminate),
			new XElement("Alias", "banshee"),
			new XElement("SortOrder", 4),
			new XElement("Plane", context.AstralPlane.Object.Id),
			new XElement("AnchorPolicy", AstralProjectionAnchorPolicy.Sleep),
			new XElement("ProjectionEcho", new XCData("You tear free.")),
			new XElement("AnchorEcho", new XCData("You fall still.")),
			new XElement("AnchorRoomEcho", new XCData("@ go|goes slack.")),
			new XElement("ProjectionRoomEcho", new XCData("@ coalesce|coalesces.")),
			new XElement("CollapseEcho", new XCData("You wake.")),
			new XElement("BacklashEcho", new XCData("A headache remains.")),
			new XElement("ProjectionSDescOverride", new XCData("a spectre of $desc"))), context.Spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("warlock-spectre", saved.Element("FormKey")?.Value);
		Assert.AreEqual(((int)Gender.Indeterminate).ToString(), saved.Element("Gender")?.Value);
		Assert.AreEqual("banshee", saved.Element("Alias")?.Value);
		Assert.AreEqual("4", saved.Element("SortOrder")?.Value);
		Assert.AreEqual(AstralProjectionAnchorPolicy.Sleep.ToString(), saved.Element("AnchorPolicy")?.Value);
		Assert.AreEqual("You tear free.", saved.Element("ProjectionEcho")?.Value);
		Assert.AreEqual("You fall still.", saved.Element("AnchorEcho")?.Value);
		Assert.AreEqual("@ go|goes slack.", saved.Element("AnchorRoomEcho")?.Value);
		Assert.AreEqual("@ coalesce|coalesces.", saved.Element("ProjectionRoomEcho")?.Value);
		Assert.AreEqual("You wake.", saved.Element("CollapseEcho")?.Value);
		Assert.AreEqual("A headache remains.", saved.Element("BacklashEcho")?.Value);
		Assert.AreEqual("a spectre of $desc", saved.Element("ProjectionSDescOverride")?.Value);
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
	public void SpellEffectFactory_RegistersPossessBodyEffect()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("possessbody"));
		var info = SpellEffectFactory.BuilderInfoForType("possessbody");
		Assert.IsTrue(info.BuilderHelp.Contains("formkey"));
		Assert.IsTrue(info.BuilderHelp.Contains("possessionecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("targetecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("roomecho"));
		Assert.IsTrue(info.BuilderHelp.Contains("collapseecho"));
		Assert.IsTrue(info.MatchingTriggers.Any());
		Assert.IsFalse(info.Instant);
		Assert.IsTrue(info.RequiresTarget);
	}

	[TestMethod]
	public void SpellEffectFactory_PossessBodyBuilderDefaultsAndXmlRoundTripsConfiguredValues()
	{
		var context = CreateCopyCloneSpellContext();

		var (built, error) =
			SpellEffectFactory.LoadEffectFromBuilderInput("possessbody", new StringStack(string.Empty),
				context.Spell.Object);

		Assert.AreEqual(string.Empty, error);
		var defaultXml = built.SaveToXml();
		Assert.AreEqual("possessbody", defaultXml.Attribute("type")?.Value);
		Assert.AreEqual("possessed-body", defaultXml.Element("FormKey")?.Value);
		Assert.AreEqual(PossessBodySpellEffect.DefaultPossessionEcho, defaultXml.Element("PossessionEcho")?.Value);
		Assert.AreEqual(PossessBodySpellEffect.DefaultRoomEcho, defaultXml.Element("RoomEcho")?.Value);
		Assert.AreEqual(PossessBodySpellEffect.DefaultCollapseEcho, defaultXml.Element("CollapseEcho")?.Value);

		var effect = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "possessbody"),
			new XElement("FormKey", "shadow-possession"),
			new XElement("SortOrder", 12),
			new XElement("PossessionEcho", new XCData("You enter the stolen shape.")),
			new XElement("TargetEcho", new XCData("Something pulls free of you.")),
			new XElement("RoomEcho", new XCData("@ stiffen|stiffens with another will.")),
			new XElement("CollapseEcho", new XCData("You recoil to yourself.")),
			new XElement("BacklashEcho", new XCData("A bitter echo follows."))), context.Spell.Object);

		var saved = effect.SaveToXml();

		Assert.AreEqual("shadow-possession", saved.Element("FormKey")?.Value);
		Assert.AreEqual("12", saved.Element("SortOrder")?.Value);
		Assert.AreEqual("You enter the stolen shape.", saved.Element("PossessionEcho")?.Value);
		Assert.AreEqual("Something pulls free of you.", saved.Element("TargetEcho")?.Value);
		Assert.AreEqual("@ stiffen|stiffens with another will.", saved.Element("RoomEcho")?.Value);
		Assert.AreEqual("You recoil to yourself.", saved.Element("CollapseEcho")?.Value);
		Assert.AreEqual("A bitter echo follows.", saved.Element("BacklashEcho")?.Value);
	}

	[TestMethod]
	public void SpellEffectFactory_RegistersDirectPossessionEffects()
	{
		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("seizebody"));
		var seizeInfo = SpellEffectFactory.BuilderInfoForType("seizebody");
		Assert.IsTrue(seizeInfo.BuilderHelp.Contains("possessionecho"));
		Assert.IsTrue(seizeInfo.BuilderHelp.Contains("victimecho"));
		Assert.IsTrue(seizeInfo.BuilderHelp.Contains("targetecho"));
		Assert.IsTrue(seizeInfo.BuilderHelp.Contains("allowadmins"));
		Assert.IsTrue(seizeInfo.MatchingTriggers.Any());
		Assert.IsFalse(seizeInfo.Instant);
		Assert.IsTrue(seizeInfo.RequiresTarget);

		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("possesscorpse"));
		var corpseInfo = SpellEffectFactory.BuilderInfoForType("possesscorpse");
		Assert.IsTrue(corpseInfo.BuilderHelp.Contains("possessionecho"));
		Assert.IsTrue(corpseInfo.BuilderHelp.Contains("targetecho"));
		Assert.IsTrue(corpseInfo.BuilderHelp.Contains("restoreecho"));
		Assert.IsTrue(corpseInfo.BuilderHelp.Contains("allowfinal"));
		Assert.IsTrue(corpseInfo.BuilderHelp.Contains("allowskeletal"));
		Assert.IsTrue(corpseInfo.MatchingTriggers.Any());
		Assert.IsFalse(corpseInfo.Instant);
		Assert.IsTrue(corpseInfo.RequiresTarget);

		Assert.IsTrue(SpellEffectFactory.MagicEffectTypes.Contains("animatecorpse"));
		var animateInfo = SpellEffectFactory.BuilderInfoForType("animatecorpse");
		Assert.IsTrue(animateInfo.BuilderHelp.Contains("ai add"));
		Assert.IsTrue(animateInfo.BuilderHelp.Contains("targetecho"));
		Assert.IsTrue(animateInfo.BuilderHelp.Contains("restoreecho"));
		Assert.IsTrue(animateInfo.BuilderHelp.Contains("allowfinal"));
		Assert.IsTrue(animateInfo.BuilderHelp.Contains("allowskeletal"));
		Assert.IsTrue(animateInfo.MatchingTriggers.Any());
		Assert.IsFalse(animateInfo.Instant);
		Assert.IsTrue(animateInfo.RequiresTarget);
	}

	[TestMethod]
	public void DirectPossessionSecurity_HasProtectedStaffAuthority_UsesPermissionLevelWithoutAdminSight()
	{
		var player = new Mock<ICharacter>();
		player.Setup(x => x.PermissionLevel).Returns(PermissionLevel.Player);

		var guide = new Mock<ICharacter>();
		guide.Setup(x => x.PermissionLevel).Returns(PermissionLevel.Guide);

		var juniorAdmin = new Mock<ICharacter>();
		juniorAdmin.Setup(x => x.PermissionLevel).Returns(PermissionLevel.JuniorAdmin);
		juniorAdmin.Setup(x => x.IsAdministrator(It.IsAny<PermissionLevel>())).Returns(false);

		Assert.IsFalse(SeizeBodySpellEffect.HasProtectedStaffAuthority(player.Object));
		Assert.IsFalse(SeizeBodySpellEffect.HasProtectedStaffAuthority(guide.Object));
		Assert.IsTrue(SeizeBodySpellEffect.HasProtectedStaffAuthority(juniorAdmin.Object));
	}

	[TestMethod]
	public void SpellEffectFactory_DirectPossessionBuilderDefaultsAndXmlRoundTrip()
	{
		var context = CreateCopyCloneSpellContext();

		var (seize, seizeError) =
			SpellEffectFactory.LoadEffectFromBuilderInput("seizebody", new StringStack(string.Empty),
				context.Spell.Object);
		var (corpse, corpseError) =
			SpellEffectFactory.LoadEffectFromBuilderInput("possesscorpse", new StringStack(string.Empty),
				context.Spell.Object);
		var (animate, animateError) =
			SpellEffectFactory.LoadEffectFromBuilderInput("animatecorpse", new StringStack(string.Empty),
				context.Spell.Object);

		Assert.AreEqual(string.Empty, seizeError);
		Assert.AreEqual(string.Empty, corpseError);
		Assert.AreEqual(string.Empty, animateError);
		var seizeXml = seize.SaveToXml();
		Assert.AreEqual("seizebody", seizeXml.Attribute("type")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), seizeXml.Element("AllowPCs")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), seizeXml.Element("AllowNPCs")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), seizeXml.Element("AllowAdmins")?.Value.ToLowerInvariant());
		Assert.AreEqual(SeizeBodySpellEffect.DefaultVictimEcho, seizeXml.Element("VictimEcho")?.Value);
		Assert.AreEqual(SeizeBodySpellEffect.DefaultVictimEndEcho, seizeXml.Element("VictimEndEcho")?.Value);

		var corpseXml = corpse.SaveToXml();
		Assert.AreEqual("possesscorpse", corpseXml.Attribute("type")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), corpseXml.Element("AllowFinal")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), corpseXml.Element("AllowSkeletal")?.Value.ToLowerInvariant());
		Assert.AreEqual(PossessCorpseSpellEffect.DefaultRestoreEcho, corpseXml.Element("RestoreEcho")?.Value);

		var animateXml = animate.SaveToXml();
		Assert.AreEqual("animatecorpse", animateXml.Attribute("type")?.Value);
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(), animateXml.Element("AllowFinal")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(), animateXml.Element("AllowSkeletal")?.Value.ToLowerInvariant());
		Assert.AreEqual(AnimateCorpseSpellEffect.DefaultRestoreEcho, animateXml.Element("RestoreEcho")?.Value);
		Assert.AreEqual(0, animateXml.Element("ArtificialIntelligences")?.Elements("AI").Count());

		var loadedSeize = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "seizebody"),
			new XElement("AllowPCs", false),
			new XElement("AllowNPCs", true),
			new XElement("AllowAdmins", true),
			new XElement("PossessionEcho", new XCData("You seize the limbs.")),
			new XElement("VictimEcho", new XCData("You are locked behind your eyes.")),
			new XElement("VictimEndEcho", new XCData("You can move again.")),
			new XElement("TargetEcho", new XCData("Your nerves burn.")),
			new XElement("RoomEcho", new XCData("@ lurch|lurches under alien control.")),
			new XElement("CollapseEcho", new XCData("You return.")),
			new XElement("BacklashEcho", new XCData("The stolen pulse fades."))), context.Spell.Object);

		var loadedSeizeXml = loadedSeize.SaveToXml();
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(),
			loadedSeizeXml.Element("AllowPCs")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(),
			loadedSeizeXml.Element("AllowAdmins")?.Value.ToLowerInvariant());
		Assert.AreEqual("You seize the limbs.", loadedSeizeXml.Element("PossessionEcho")?.Value);
		Assert.AreEqual("You are locked behind your eyes.", loadedSeizeXml.Element("VictimEcho")?.Value);
		Assert.AreEqual("You can move again.", loadedSeizeXml.Element("VictimEndEcho")?.Value);
		Assert.AreEqual("Your nerves burn.", loadedSeizeXml.Element("TargetEcho")?.Value);
		Assert.AreEqual("@ lurch|lurches under alien control.", loadedSeizeXml.Element("RoomEcho")?.Value);
		Assert.AreEqual("The stolen pulse fades.", loadedSeizeXml.Element("BacklashEcho")?.Value);

		var loadedCorpse = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "possesscorpse"),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", false),
			new XElement("AllowAdmins", true),
			new XElement("AllowFinal", false),
			new XElement("AllowSkeletal", true),
			new XElement("PossessionEcho", new XCData("The dead body obeys.")),
			new XElement("TargetEcho", new XCData("The corpse twitches.")),
			new XElement("RoomEcho", new XCData("@ rise|rises with dead hands.")),
			new XElement("CollapseEcho", new XCData("The dead weight drops.")),
			new XElement("BacklashEcho", new XCData("Cold dirt follows you.")),
			new XElement("RestoreEcho", new XCData("@ fold|folds into $1."))), context.Spell.Object);

		var loadedCorpseXml = loadedCorpse.SaveToXml();
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(),
			loadedCorpseXml.Element("AllowNPCs")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(),
			loadedCorpseXml.Element("AllowFinal")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(),
			loadedCorpseXml.Element("AllowSkeletal")?.Value.ToLowerInvariant());
		Assert.AreEqual("The dead body obeys.", loadedCorpseXml.Element("PossessionEcho")?.Value);
		Assert.AreEqual("The corpse twitches.", loadedCorpseXml.Element("TargetEcho")?.Value);
		Assert.AreEqual("@ rise|rises with dead hands.", loadedCorpseXml.Element("RoomEcho")?.Value);
		Assert.AreEqual("The dead weight drops.", loadedCorpseXml.Element("CollapseEcho")?.Value);
		Assert.AreEqual("Cold dirt follows you.", loadedCorpseXml.Element("BacklashEcho")?.Value);
		Assert.AreEqual("@ fold|folds into $1.", loadedCorpseXml.Element("RestoreEcho")?.Value);

		var loadedAnimate = SpellEffectFactory.LoadEffect(new XElement("Effect",
			new XAttribute("type", "animatecorpse"),
			new XElement("AllowPCs", true),
			new XElement("AllowNPCs", false),
			new XElement("AllowAdmins", true),
			new XElement("AllowFinal", false),
			new XElement("AllowSkeletal", true),
			new XElement("ArtificialIntelligences",
				new XElement("AI", new XAttribute("id", 11)),
				new XElement("AI", new XAttribute("id", 12))),
			new XElement("TargetEcho", new XCData("The corpse twitches.")),
			new XElement("RoomEcho", new XCData("@ rise|rises hungry.")),
			new XElement("CollapseEcho", new XCData("@ fall|falls inert.")),
			new XElement("RestoreEcho", new XCData("@ fold|folds into $1."))), context.Spell.Object);

		var loadedAnimateXml = loadedAnimate.SaveToXml();
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(),
			loadedAnimateXml.Element("AllowNPCs")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.FalseString.ToLowerInvariant(),
			loadedAnimateXml.Element("AllowFinal")?.Value.ToLowerInvariant());
		Assert.AreEqual(bool.TrueString.ToLowerInvariant(),
			loadedAnimateXml.Element("AllowSkeletal")?.Value.ToLowerInvariant());
		CollectionAssert.AreEqual(new[] { "11", "12" },
			loadedAnimateXml.Element("ArtificialIntelligences")?.Elements("AI")
			                .Select(x => x.Attribute("id")?.Value)
			                .ToArray());
		Assert.AreEqual("The corpse twitches.", loadedAnimateXml.Element("TargetEcho")?.Value);
		Assert.AreEqual("@ rise|rises hungry.", loadedAnimateXml.Element("RoomEcho")?.Value);
		Assert.AreEqual("@ fall|falls inert.", loadedAnimateXml.Element("CollapseEcho")?.Value);
		Assert.AreEqual("@ fold|folds into $1.", loadedAnimateXml.Element("RestoreEcho")?.Value);
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

	private static string GetSourcePath(params string[] parts)
	{
		var root = AppContext.BaseDirectory;
		for (var i = 0; i < 8 && !File.Exists(Path.Combine(root, "MudSharp.sln")); i++)
		{
			root = Path.GetFullPath(Path.Combine(root, ".."));
		}

		return Path.Combine(new[] { root }.Concat(parts).ToArray());
	}

	private sealed record CopyCloneSpellContext(
		Mock<IMagicSpell> Spell,
		Mock<IRace> Race,
		Mock<IPlane> AstralPlane);
}
