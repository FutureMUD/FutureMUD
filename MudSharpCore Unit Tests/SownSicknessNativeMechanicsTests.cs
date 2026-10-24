#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Character;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Health.Infections;
using MudSharp.GameItems.Interfaces;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SownSicknessNativeMechanicsTests
{
	[TestMethod]
	[DataRow(double.NegativeInfinity, 1)]
	[DataRow(0.0, 1)]
	[DataRow(1.0, 1)]
	[DataRow(1.01, 2)]
	[DataRow(6.2, 7)]
	[DataRow(100.0, 7)]
	[DataRow(double.PositiveInfinity, 1)]
	[DataRow(double.NaN, 1)]
	public void GradeFromIntensity_IsFiniteAndBounded(double intensity, int expected)
	{
		Assert.AreEqual(expected, SownSicknessInfection.GradeFromIntensity(intensity));
	}

	[TestMethod]
	public void SownSickness_UsesItsOwnNativeInfectionTypeAndBoundedStaminaConsequences()
	{
		Assert.AreEqual(5, (int)InfectionType.SownSickness);
		Assert.AreEqual("sown sickness", InfectionType.SownSickness.Describe());
		Assert.AreEqual(1.1, SownSicknessSymptoms.StaminaExpenditureMultiplier(1), 0.0001);
		Assert.AreEqual(1.7, SownSicknessSymptoms.StaminaExpenditureMultiplier(99), 0.0001);
		Assert.AreEqual(0.95, SownSicknessSymptoms.StaminaRegenerationMultiplier(1), 0.0001);
		Assert.AreEqual(0.65, SownSicknessSymptoms.StaminaRegenerationMultiplier(99), 0.0001);
	}

	[TestMethod]
	public void RestorativeMudBinding_RequiresNativeKeyAndBudgetsOnlyConsumedDose()
	{
		var dose = new ArmageddonTopicalDoseBinding("arm.item.restorative_mud", Guid.NewGuid(), 123, 4, 42, 43, 1.0, 10.0, false);
		Assert.IsTrue(dose.IsValid);
		Assert.AreEqual(40.0, dose.HealingBudget);
		Assert.AreEqual(0.0, dose.HealingBudgetForConsumedDose(0.0));
		Assert.AreEqual(20.0, dose.HealingBudgetForConsumedDose(0.5));
		Assert.AreEqual(40.0, dose.HealingBudgetForConsumedDose(2.0));
		Assert.IsTrue(dose.IsExactFullDose(1.0, 1.0));
		Assert.IsFalse(dose.IsExactFullDose(0.5, 1.0));
		Assert.IsFalse(dose.IsExactFullDose(1.0, 0.0));
		Assert.IsFalse((dose with { Spent = true }).CanConsume(1.0, 1.0));
		Assert.AreEqual(0.0, new ArmageddonTopicalDoseBinding("wrong-key", Guid.NewGuid(), 123, 4, 42, 43, 1.0, 10.0, false).HealingBudget);
		Assert.AreEqual(0.0, new ArmageddonTopicalDoseBinding("arm.item.restorative_mud", Guid.NewGuid(), 123, 8, 42, 43, 1.0, 10.0, false).HealingBudget);
	}

	[TestMethod]
	public void EmberSeedBinding_RequiresLiveSpellLifecycleAndCommitsAtMostOnce()
	{
		var now = new DateTime(2026, 10, 12, 0, 0, 0, DateTimeKind.Utc);
		var binding = new ArmageddonEmberSeedBinding("arm.item.ember_seed", Guid.NewGuid(),
			123, SpellLifecycleMode.TemporaryCleanup, 7, 42, 43, now.AddSeconds(2100), 3.0);
		Assert.AreEqual(21.0, binding.DamagePerEligibleTarget);
		Assert.IsTrue(binding.CanActivate(false, now));
		Assert.IsFalse(binding.CanActivate(true, now));
		Assert.IsFalse(binding.CanActivate(false, now.AddSeconds(2100)));
		Assert.IsFalse((binding with { LifecycleId = Guid.Empty }).CanActivate(false, now));
		Assert.IsFalse((binding with { LifecycleMode = SpellLifecycleMode.Permanent }).CanActivate(false, now));
		Assert.IsFalse((binding with { Grade = 8 }).CanActivate(false, now));
	}

	[TestMethod]
	public void SpecialDoseComponentPhysicalReferences_ExposeCreatorIdentityAndExactInstance()
	{
		const string definition = "<Definition CreatorId=\"42\" CreatorInstanceId=\"43\" />";
		var references = PhysicalReferenceCodecs.Component("TopicalCream", definition, _ => null).ToArray();
		Assert.AreEqual(2, references.Length);
		Assert.AreEqual(42L, references[0].Id);
		Assert.AreEqual(43L, references[1].Id);
	}
}
