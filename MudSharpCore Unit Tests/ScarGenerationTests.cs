using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
using MudSharp.Character.Heritage;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using MudSharp.Health;
using MudSharp.Health.Wounds;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using MudSharp.RPG.Merits;
using MudSharp.RPG.Merits.CharacterMerits;
using MudSharp.RPG.Merits.Interfaces;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ScarGenerationTests
{
	[TestMethod]
	public void ScarGenerationSupport_UsesConfiguredDamageAndSurgeryMatrixValues()
	{
		var gameworld = CreateGameworldWithMatrix(new XElement("ScarGenerationChanceMatrix",
			new XElement("Damage",
				new XAttribute("Type", DamageType.Slashing),
				new XElement("Chance", new XAttribute("Severity", WoundSeverity.Severe), "0.42")),
			new XElement("Surgery",
				new XElement("Chance", new XAttribute("Severity", WoundSeverity.Moderate), "0.27"))));

		var damageChance = ScarGenerationSupport.GetBaseChance(gameworld.Object, new ScarWoundContext(
			false,
			DamageType.Slashing,
			WoundSeverity.Severe,
			null,
			0,
			Outcome.None,
			false,
			false,
			false,
			false,
			false));
		var surgeryChance = ScarGenerationSupport.GetBaseChance(gameworld.Object, new ScarWoundContext(
			true,
			DamageType.Slashing,
			WoundSeverity.Moderate,
			SurgicalProcedureType.InvasiveProcedureFinalisation,
			0,
			Outcome.None,
			false,
			false,
			false,
			false,
			false));

		Assert.AreEqual(0.42, damageChance, 0.0001);
		Assert.AreEqual(0.27, surgeryChance, 0.0001);
	}

	[TestMethod]
	public void ScarGeneration_ApplyMeritModifiers_SumsFlatThenMultiplies()
	{
		var owner = new Mock<IHaveMerits>();
		var wound = new Mock<IWound>();
		var meritOne = new Mock<IScarChanceMerit>();
		meritOne.Setup(x => x.Applies(owner.Object)).Returns(true);
		meritOne.Setup(x => x.AppliesTo(wound.Object)).Returns(true);
		meritOne.SetupGet(x => x.FlatModifier).Returns(0.05);
		meritOne.SetupGet(x => x.Multiplier).Returns(1.1);

		var meritTwo = new Mock<IScarChanceMerit>();
		meritTwo.Setup(x => x.Applies(owner.Object)).Returns(true);
		meritTwo.Setup(x => x.AppliesTo(wound.Object)).Returns(true);
		meritTwo.SetupGet(x => x.FlatModifier).Returns(-0.02);
		meritTwo.SetupGet(x => x.Multiplier).Returns(1.5);

		owner.SetupGet(x => x.Merits).Returns([meritOne.Object, meritTwo.Object]);

		var modified = ScarGeneration.ApplyMeritModifiers(owner.Object, wound.Object, 0.20);

		Assert.AreEqual(0.3795, modified, 0.0001, "Scar merits should sum flats before multiplying multipliers.");
	}

	[TestMethod]
	public void ScarGeneration_GetOccurrenceChance_ClampsAfterMerits()
	{
		var gameworld = CreateGameworldWithMatrix(new XElement("ScarGenerationChanceMatrix",
			new XElement("Damage",
				new XAttribute("Type", DamageType.Slashing),
				new XElement("Chance", new XAttribute("Severity", WoundSeverity.Moderate), "0.45")),
			new XElement("Surgery")));
		gameworld.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns<string>(key => key switch
		{
			"ScarGenerationChanceClampMaximum" => 0.50,
			_ => 0.0
		});

		var merit = new Mock<IScarChanceMerit>();
		var wound = new Mock<IWound>();
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Slashing);
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Moderate);
		merit.Setup(x => x.Applies(It.IsAny<IHaveMerits>())).Returns(true);
		merit.Setup(x => x.AppliesTo(wound.Object)).Returns(true);
		merit.SetupGet(x => x.FlatModifier).Returns(0.10);
		merit.SetupGet(x => x.Multiplier).Returns(1.2);

		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		owner.SetupGet(x => x.Merits).Returns([merit.Object]);

		var chance = ScarGeneration.GetOccurrenceChance(owner.Object, wound.Object);

		Assert.AreEqual(0.50, chance, 0.0001, "Final scar chance should clamp after merit modifiers are applied.");
	}

	[TestMethod]
	public void GenerateScar_UsesOrientationMappingAndPersistsGeneratedState()
	{
		var gameworld = CreateGameworldWithMatrix(new XElement("ScarGenerationChanceMatrix",
			new XElement("Damage",
				new XAttribute("Type", DamageType.Slashing),
				new XElement("Chance", new XAttribute("Severity", WoundSeverity.Severe), "0.42")),
			new XElement("Surgery")));
		gameworld.Setup(x => x.GetStaticConfiguration("ScarOrientationByBodypartShape")).Returns(
			new XElement("ScarOrientationProfiles",
				new XAttribute("Default", "linear"),
				new XElement("Shape", new XAttribute("Name", "forearm"), new XAttribute("Profile", "linear")))
			.ToString());

		var race = new Mock<IRace>();
		race.Setup(x => x.ModifiedSize(It.IsAny<IBodypart>())).Returns(SizeCategory.Normal);

		var shape = new Mock<IBodypartShape>();
		shape.SetupGet(x => x.Name).Returns("forearm");

		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.Shape).Returns(shape.Object);
		bodypart.SetupGet(x => x.Name).Returns("lforearm");
		bodypart.Setup(x => x.FullDescription()).Returns("left forearm");

		var scarTime = new MudDateTime("2025-12-31 00:00:00", gameworld.Object);

		var scar = ScarGeneration.GenerateScar(gameworld.Object, race.Object, bodypart.Object, new ScarWoundContext(
			false,
			DamageType.Slashing,
			WoundSeverity.Severe,
			null,
			0,
			Outcome.None,
			false,
			false,
			false,
			false,
			false), scarTime, 0);

		var xml = scar.SaveToXml();
		Assert.IsTrue(scar.ShortDescription.Contains("slash scar", System.StringComparison.InvariantCultureIgnoreCase));
		Assert.IsTrue(scar.FullDescription.Contains("left forearm", System.StringComparison.InvariantCultureIgnoreCase));
		Assert.AreEqual("Slashing", xml.Element("DamageType")?.Value);
		Assert.AreEqual("Severe", xml.Element("Severity")?.Value);
		Assert.AreEqual("False", xml.Element("IsSurgical")?.Value);
	}

	[TestMethod]
	public void GenerateScarOptions_ReturnDistinctChoicesForChargen()
	{
		var gameworld = CreateGameworldWithMatrix(new XElement("ScarGenerationChanceMatrix",
			new XElement("Damage",
				new XAttribute("Type", DamageType.Burning),
				new XElement("Chance", new XAttribute("Severity", WoundSeverity.Grievous), "0.42")),
			new XElement("Surgery")));
		gameworld.Setup(x => x.GetStaticConfiguration("ScarOrientationByBodypartShape")).Returns(
			new XElement("ScarOrientationProfiles",
				new XAttribute("Default", "broad"),
				new XElement("Shape", new XAttribute("Name", "abdomen"), new XAttribute("Profile", "broad")))
			.ToString());

		var race = new Mock<IRace>();
		race.Setup(x => x.ModifiedSize(It.IsAny<IBodypart>())).Returns(SizeCategory.Large);

		var shape = new Mock<IBodypartShape>();
		shape.SetupGet(x => x.Name).Returns("abdomen");

		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.Shape).Returns(shape.Object);
		bodypart.SetupGet(x => x.Name).Returns("abdomen");
		bodypart.Setup(x => x.FullDescription()).Returns("abdomen");

		var options = ScarGeneration.GenerateScarOptions(gameworld.Object, race.Object, bodypart.Object,
			new ScarWoundContext(false, DamageType.Burning, WoundSeverity.Grievous, null, 0, Outcome.None, false, false,
				false, false, false),
			new MudDateTime("2025-12-31 00:00:00", gameworld.Object),
			4);

		Assert.AreEqual(4, options.Count);
		Assert.AreEqual(4, options.Select(x => x.FullDescription).Distinct().Count());
	}

	[TestMethod]
	public void ScarChanceMerit_UsesWoundFlagsDamageTypesAndSurgeryFlag()
	{
		ScarChanceMerit.RegisterMeritInitialiser();
		var merit = LoadScarChanceMerit(new XElement("Merit",
			new XAttribute("wounds", true),
			new XAttribute("surgery", false),
			new XAttribute("flat", 0.05),
			new XAttribute("multiplier", 1.2),
			new XElement("ChargenAvailableProg", 0),
			new XElement("ApplicabilityProg", 0),
			new XElement("ChargenBlurb", string.Empty),
			new XElement("DescriptionText", string.Empty),
			new XElement("DamageTypes",
				new XElement("DamageType", (int)DamageType.Slashing))));

		var surgeryMerit = LoadScarChanceMerit(new XElement("Merit",
			new XAttribute("wounds", false),
			new XAttribute("surgery", true),
			new XAttribute("flat", 0.0),
			new XAttribute("multiplier", 1.0),
			new XElement("ChargenAvailableProg", 0),
			new XElement("ApplicabilityProg", 0),
			new XElement("ChargenBlurb", string.Empty),
			new XElement("DescriptionText", string.Empty),
			new XElement("DamageTypes")));

		var wound = new Mock<IWound>();
		wound.SetupGet(x => x.DamageType).Returns(DamageType.Slashing);
		wound.SetupGet(x => x.Severity).Returns(WoundSeverity.Moderate);

		var otherDamageWound = new Mock<IWound>();
		otherDamageWound.SetupGet(x => x.DamageType).Returns(DamageType.Burning);
		otherDamageWound.SetupGet(x => x.Severity).Returns(WoundSeverity.Moderate);

		var surgeryWound = CreateHealingWound();
		surgeryWound.MarkScarFromSurgery(SurgicalProcedureType.InvasiveProcedureFinalisation, 1);

		Assert.IsTrue(merit.AppliesTo(wound.Object), "The merit should apply to opted-in wound damage types.");
		Assert.IsFalse(merit.AppliesTo(otherDamageWound.Object), "The merit should not apply to wound damage types that were not opted in.");
		Assert.IsFalse(merit.AppliesTo(surgeryWound), "A wound-only merit should not apply to surgery scars.");
		Assert.IsTrue(surgeryMerit.AppliesTo(surgeryWound), "A surgery-enabled merit should apply to surgery scars.");
		Assert.IsFalse(surgeryMerit.AppliesTo(wound.Object), "A surgery-only merit should not apply to ordinary wounds.");
	}

	private static Mock<IFuturemud> CreateGameworldWithMatrix(XElement matrix)
	{
		var calendar = new Mock<ICalendar>();
		calendar.SetupGet(x => x.CurrentDateTime).Returns(new MudDateTime("2026-01-02 00:00:00", Mock.Of<IFuturemud>()));

		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticConfiguration("ScarGenerationChanceMatrix")).Returns(matrix.ToString());
		gameworld.Setup(x => x.GetStaticConfiguration("ScarOrientationByBodypartShape")).Returns(
			new XElement("ScarOrientationProfiles", new XAttribute("Default", "linear")).ToString());
		gameworld.Setup(x => x.GetStaticString("ScarSDescFresh")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticString("ScarSDescRecent")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticString("ScarSDescOld")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticString("ScarFDescFresh")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticString("ScarFDescRecent")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticString("ScarFDescOld")).Returns("{0}");
		gameworld.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns<string>(key => key switch
		{
			"ScarDaysForOld" => 999.0,
			"ScarDaysForRecent" => 999.0,
			_ => 0.0
		});
		return gameworld;
	}

	private static ScarChanceMerit LoadScarChanceMerit(XElement definition)
	{
		var alwaysTrueProg = new Mock<MudSharp.FutureProg.IFutureProg>();
		alwaysTrueProg.SetupGet(x => x.Id).Returns(0);
		var futureProgs = new Mock<IUneditableAll<MudSharp.FutureProg.IFutureProg>>();
		futureProgs.Setup(x => x.Get(It.IsAny<long>())).Returns(alwaysTrueProg.Object);

		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.AlwaysTrueProg).Returns(alwaysTrueProg.Object);
		gameworld.SetupGet(x => x.FutureProgs).Returns(futureProgs.Object);
		gameworld.SetupGet(x => x.ChargenResources).Returns(Mock.Of<IUneditableAll<MudSharp.CharacterCreation.Resources.IChargenResource>>());

		var merit = new Merit
		{
			Id = 1,
			Name = "Scar Chance Test",
			Type = "Scar Chance",
			MeritType = (int)MeritType.Merit,
			MeritScope = (int)MeritScope.Character,
			Definition = definition.ToString()
		};

		return (ScarChanceMerit)MeritFactory.LoadMerit(merit, gameworld.Object);
	}

	private static HealingSimpleWound CreateHealingWound()
	{
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(saveManager.Object);
		gameworld.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns(0.0);

		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.DamageModifier).Returns(1.0);

		var body = new Mock<IBody>();
		body.Setup(x => x.HitpointsForBodypart(bodypart.Object)).Returns(100.0);

		var owner = new Mock<ICharacter>();
		owner.SetupGet(x => x.Body).Returns(body.Object);

		return new HealingSimpleWound(gameworld.Object, owner.Object, 10.0, DamageType.Slashing, bodypart.Object, null, null, null);
	}
}
