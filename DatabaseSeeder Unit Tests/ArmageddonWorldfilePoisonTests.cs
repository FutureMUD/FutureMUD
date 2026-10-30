using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using ExpressionEngine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.RPG.Checks;
using Db = MudSharp.Models;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWorldfilePoisonTests
{
	[TestMethod]
	public void Skellebain_OutcomesPreserveSourceEventOddsAndDeliveryConfiguration()
	{
		foreach (var index in new[] { 3, 9, 10 })
		{
			var model = ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(index, 1, 2, 3, 42);
			var info = DrugSymptomsAdditionalInfo.Parse(model.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Symptoms).AdditionalEffects);
			Assert.AreEqual(51.0, info.Stages[1].Outcomes.Sum(x => x.Weight));
			Assert.AreEqual(39.0, info.Stages[2].Outcomes.Sum(x => x.Weight));
			Assert.AreEqual(600.0, info.Stages[2].Outcomes.Single(x => x.Action == DrugSymptomAction.Babble).DurationSeconds);
			Assert.IsTrue(info.Stages[2].Outcomes.Single(x => x.Action == DrugSymptomAction.DropReadiedItems).ReleaseGrapple);
			Assert.AreEqual(42L, info.Stages[2].Outcomes.Single(x => x.Action == DrugSymptomAction.Residue).ResidueMaterialId);
		}
	}

	[TestMethod]
	public void Darts_UseNativeThrowingWithSourceDamageAndPiercingInsteadOfInterpretingPoisonAsAttackType()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		ArmageddonInstallation.Prepare(ArmageddonInstallationSettingsTests.Factory(db));
		const long baseline = 200_000;
		db.WeaponTypes.Add(new Db.WeaponType { Id = baseline, Name = "Knife" });
		db.WeaponAttacks.Add(new Db.WeaponAttack { Id = baseline, WeaponTypeId = baseline, Name = "Native stab" });
		db.RangedWeaponTypes.Add(new Db.RangedWeaponTypes { Id = baseline, Name = "Throwing Knife",
			RangedWeaponType = (int)MudSharp.Combat.RangedWeaponType.Thrown, DefaultRangeInRooms = 2, FireTraitId = 17 });
		db.SaveChanges();
		var source = ArmageddonWorldfileTestData.Load().ForInstallation();
		var writer = new ArmageddonWorldfileWriter(db, source);
		var darts = source.Items.Where(x => x.ItemType == 24).ToArray();
		Assert.IsTrue(darts.Length > 0);
		foreach (var dart in darts)
		{
			var components = ((Db.GameItemComponentProto?, Db.GameItemComponentProto?))typeof(ArmageddonWorldfileWriter)
				.GetMethod("CreateMeleeComponent", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(writer, [dart, dart.Values, "dart"])!;
			Assert.AreEqual("ThrownWeapon", components.Item1!.Type);
			var definition = XElement.Parse(components.Item1.Definition);
			var meleeId = (long)definition.Element("MeleeWeaponType")!;
			Assert.AreEqual((int)MudSharp.Health.DamageType.Piercing, db.WeaponAttacks.Local.Single(x => x.WeaponTypeId == meleeId).DamageType);
			var ranged = db.RangedWeaponTypes.Local.Single(x => x.Id == (long)definition.Element("RangedWeaponType")!);
			Assert.AreEqual(17L, ranged.FireTraitId);
			Assert.AreEqual(2, ranged.DefaultRangeInRooms);
			var damage = new Expression(ranged.DamageBonusExpression).EvaluateDouble();
			Assert.IsTrue(double.IsFinite(damage) && damage >= 0);
		}
	}

	[TestMethod]
	public void PoisonProfiles_AllTenIndicesLoadAndTheirEpisodeFormulasEvaluate()
	{
		for (var index = 1; index <= 10; index++)
		{
			var model = ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(index, 1, 2, 3);
			var drug = new MudSharp.Health.Drug(model, Mock.Of<IFuturemud>());
			if (index == 6)
			{
				CollectionAssert.AreEquivalent(new[] { DrugType.Paralysis, DrugType.Symptoms }, drug.DrugTypes.ToArray());
				Assert.IsTrue(drug.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms).Stages.Single().CeaseCombatOnParalysis);
			}
			var info = drug.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
			foreach (var stage in info.Stages)
			foreach (var text in new[] { stage.Chance, stage.Damage, stage.Pain, stage.Stun, stage.Stamina, stage.TransitionChance }.Concat(stage.NextStageWeights.Values))
			{
				var expression = new Expression(text);
				Assert.IsFalse(expression.HasErrors(), text);
				Assert.IsTrue(double.IsFinite(expression.EvaluateDoubleWith(("intensity", 1.0), ("resistance", 10.0),
					("fullness", 0.0), ("stamina", 100.0), ("sleeping", 0.0), ("unconscious", 0.0), ("stage", 0))), text);
			}
		}
	}

	[TestMethod]
	public void PoisonResistance_InstallsAttributeBindingAndPreservesBuilderPolicy()
	{
		using var db = ArmageddonInstallationSettingsTests.Fixture();
		var endurance = new Db.TraitDefinition { Id = 42, Type = 1, Name = "Endurance" };
		db.TraitDefinitions.Add(endurance); db.SaveChanges();
		var check = PoisonResistanceSeederHelper.Ensure(db, endurance);
		db.SaveChanges();
		Assert.AreEqual((int)CheckType.PoisonResistanceCheck, check.Type);
		Assert.AreEqual("endurance * 5", check.TraitExpression.Expression);
		Assert.AreEqual(42L, check.TraitExpression.TraitExpressionParameters.Single().TraitDefinitionId);
		Assert.IsFalse(check.CheckTemplate.ImproveTraits);
		check.TraitExpression.Expression = "27";
		db.SaveChanges();
		Assert.AreSame(check, PoisonResistanceSeederHelper.Ensure(db, endurance));
		Assert.AreEqual("27", check.TraitExpression.Expression);
	}

	[TestMethod]
	public void PoisonCalibration_PreservesDurationDefaultsHalvingAndReferenceMetabolism()
	{
		Assert.AreEqual(0.001, ArmageddonWorldfileWriter.LegacyPoisonDoseGrams(1, 1), 1e-12);
		Assert.AreEqual(0.05, ArmageddonWorldfileWriter.LegacyPoisonDoseGrams(4, 0), 1e-12);
		Assert.AreEqual(0.05, ArmageddonWorldfileWriter.LegacyPoisonDoseGrams(8, 4), 1e-12);
		Assert.AreEqual(0.002, ArmageddonWorldfileWriter.LegacyPoisonDoseGrams(6, 4), 1e-12);
		Assert.AreEqual(0.001 / 60, ArmageddonWorldfileWriter.LegacyPoisonDoseGrams(6, 1), 1e-12);
		var profile = ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(4, 1, 2, 3);
		var removedPerHeartbeat = 0.01 * 10000.0 / 3600.0 * profile.RelativeMetabolisationRate;
		Assert.AreEqual(600.0, 0.001 / removedPerHeartbeat * 10.0, 1e-8);
		var symptoms = DrugSymptomsAdditionalInfo.Parse(profile.DrugsIntensities.Single().AdditionalEffects);
		Assert.IsTrue(symptoms.Stages.Single().KillSleeping);
		Assert.AreEqual(3.0, symptoms.TickIntervalSeconds);
		var plague = ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(8, 1, 2, 3);
		Assert.AreEqual(-1.0, DrugSymptomsAdditionalInfo.Parse(plague.DrugsIntensities.Single(x => x.DrugType == (int)DrugType.Symptoms).AdditionalEffects).ExpiryTraitChanges.Single().Amount);
	}

	[TestMethod]
	public void SkellebainVariants_PreserveTheirInitialStagesAndMethelinocIsDistinctFromSpice()
	{
		foreach (var (index, expectedStage) in new[] { (3, 0), (9, 1), (10, 2) })
		{
			var model = ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(index, 1, 2, 3);
			var config = DrugSymptomsAdditionalInfo.Parse(model.DrugsIntensities.Single().AdditionalEffects);
			Assert.AreEqual(expectedStage, config.InitialStage);
			Assert.AreEqual(3, config.Stages.Count);
		}
		Assert.AreEqual("Methelinoc Toxin", ArmageddonWorldfileWriter.CreateLegacyPoisonDrug(4, 1, 2, 3).Name);
	}
}
