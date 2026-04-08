using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.Character;
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
	public void ScarTemplateIndexSnapshot_ReturnsOnlyValidDamageCandidates()
	{
		var body = new Mock<IBody>();
		var shape = CreateShape(10, "Arm");
		var otherShape = CreateShape(11, "Leg");
		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.Shape).Returns(shape.Object);

		var wildcardTemplate = CreateScarTemplate("wildcard",
			bodypartShapes: [],
			canApplyToBodypart: true,
			canApplyFromDamage: (type, severity) => type == DamageType.Slashing && severity >= WoundSeverity.Moderate);
		var shapeTemplate = CreateScarTemplate("shape",
			bodypartShapes: [shape.Object],
			canApplyToBodypart: true,
			canApplyFromDamage: (type, severity) => type == DamageType.Slashing && severity >= WoundSeverity.Moderate);
		var wrongShapeTemplate = CreateScarTemplate("wrong-shape",
			bodypartShapes: [otherShape.Object],
			canApplyToBodypart: true,
			canApplyFromDamage: (type, severity) => true);
		var uniqueRejectedTemplate = CreateScarTemplate("unique-rejected",
			bodypartShapes: [shape.Object],
			canApplyToBodypart: false,
			canApplyFromDamage: (type, severity) => true);

		var snapshot = new ScarTemplateIndexSnapshot([
			wildcardTemplate.Object,
			shapeTemplate.Object,
			wrongShapeTemplate.Object,
			uniqueRejectedTemplate.Object
		]);

		var results = snapshot.GetCandidates(body.Object, bodypart.Object, new ScarWoundContext(
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
			false)).ToList();

		CollectionAssert.AreEquivalent(
			new[] { wildcardTemplate.Object, shapeTemplate.Object },
			results,
			"Damage candidate lookup should intersect shape and damage buckets and still honour runtime eligibility checks.");
	}

	[TestMethod]
	public void ScarTemplateIndexSnapshot_ReturnsOnlyValidSurgeryCandidates()
	{
		var body = new Mock<IBody>();
		var shape = CreateShape(10, "Arm");
		var bodypart = new Mock<IBodypart>();
		bodypart.SetupGet(x => x.Shape).Returns(shape.Object);

		var matchingTemplate = CreateScarTemplate("matching",
			bodypartShapes: [shape.Object],
			canApplyToBodypart: true,
			canApplyFromDamage: (type, severity) => false,
			canApplyFromSurgery: type => type == SurgicalProcedureType.InvasiveProcedureFinalisation);
		var wrongSurgeryTemplate = CreateScarTemplate("wrong-surgery",
			bodypartShapes: [shape.Object],
			canApplyToBodypart: true,
			canApplyFromDamage: (type, severity) => false,
			canApplyFromSurgery: type => type == SurgicalProcedureType.Amputation);

		var snapshot = new ScarTemplateIndexSnapshot([matchingTemplate.Object, wrongSurgeryTemplate.Object]);

		var results = snapshot.GetCandidates(body.Object, bodypart.Object, new ScarWoundContext(
			true,
			DamageType.Slashing,
			WoundSeverity.Severe,
			SurgicalProcedureType.InvasiveProcedureFinalisation,
			2,
			Outcome.Pass,
			false,
			false,
			false,
			false,
			false)).ToList();

		CollectionAssert.AreEqual(new[] { matchingTemplate.Object }, results);
	}

	[TestMethod]
	public void ScarTemplate_LoadHealingScarWeight_LoadsLegacyChanceElements()
	{
		var xml = XElement.Parse("""
			<Scar>
				<DamageHealingScarChance>0.35</DamageHealingScarChance>
				<SurgeryHealingScarChance>0.60</SurgeryHealingScarChance>
			</Scar>
			""");

		var damageWeight = ScarTemplate.LoadHealingScarWeight(xml, "DamageHealingScarWeight", "DamageHealingScarChance");
		var surgeryWeight = ScarTemplate.LoadHealingScarWeight(xml, "SurgeryHealingScarWeight", "SurgeryHealingScarChance");

		Assert.AreEqual(0.35, damageWeight, 0.0001);
		Assert.AreEqual(0.60, surgeryWeight, 0.0001);
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
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.GetStaticConfiguration("ScarGenerationChanceMatrix")).Returns(matrix.ToString());
		gameworld.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns(0.0);
		return gameworld;
	}

	private static Mock<IBodypartShape> CreateShape(long id, string name)
	{
		var shape = new Mock<IBodypartShape>();
		shape.SetupGet(x => x.Id).Returns(id);
		shape.SetupGet(x => x.Name).Returns(name);
		return shape;
	}

	private static Mock<IScarTemplate> CreateScarTemplate(
		string name,
		IEnumerable<IBodypartShape> bodypartShapes,
		bool canApplyToBodypart,
		System.Func<DamageType, WoundSeverity, bool> canApplyFromDamage,
		System.Func<SurgicalProcedureType, bool>? canApplyFromSurgery = null)
	{
		var template = new Mock<IScarTemplate>();
		template.SetupGet(x => x.Name).Returns(name);
		template.SetupGet(x => x.Id).Returns(name.GetHashCode());
		template.SetupGet(x => x.BodypartShapes).Returns(bodypartShapes.ToList());
		template.Setup(x => x.CanBeAppliedToBodypart(It.IsAny<IBody>(), It.IsAny<IBodypart>())).Returns(canApplyToBodypart);
		template.Setup(x => x.CanBeAppliedFromDamage(It.IsAny<DamageType>(), It.IsAny<WoundSeverity>()))
			.Returns<DamageType, WoundSeverity>((type, severity) => canApplyFromDamage(type, severity));
		template.Setup(x => x.CanBeAppliedFromSurgery(It.IsAny<SurgicalProcedureType>()))
			.Returns<SurgicalProcedureType>(type => canApplyFromSurgery?.Invoke(type) ?? false);
		return template;
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
