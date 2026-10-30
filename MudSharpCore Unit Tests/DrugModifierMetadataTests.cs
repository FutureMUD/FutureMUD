#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugModifierMetadataTests
{
	[TestMethod]
	public void DrugModifierAndDependenceModifiers_RoundTripAndPreserveLegacyDefaults()
	{
		var step = new DrugModifierStep(2.5, 11, -3.0, -5.0, 23, -7.0, 4.0, -6.0);
		var modifier = new DrugModifierAdditionalInfo { Steps = [step] };
		var dependence = new DrugDependenceAdditionalInfo
		{
			ExposureGainPerGram = 2.0, ExposureDecayPerDay = 0.1, ToleranceThreshold = 10.0,
			MinimumToleranceMultiplier = 1.0, WithdrawalThreshold = 5.0, WithdrawalDecayPerDay = 0.25,
			AffectedDrugTypes = [], WithdrawalCheckPenalty = -1.0, WithdrawalHungerMultiplier = 1.1,
			WithdrawalThirstMultiplier = 1.1, WithdrawalStaminaRegenMultiplier = 0.8,
			WithdrawalStaminaCostMultiplier = 1.1, WithdrawalNauseaIntensity = 0.5,
			WithdrawalRageIntensity = 0.0, SleepPreventionThreshold = 0.5,
			RelapseExposureMultiplier = 2.0, WithdrawalMaximumHitPointsMultiplier = 0.8,
			WithdrawalMagicResourceCapacityMultiplier = 0.7, ModifierSteps = [step]
		};
		var model = new MudSharp.Models.Drug
		{
			Id = 1, Name = "Modifiers", DrugVectors = 2, IntensityPerGram = 1.0, RelativeMetabolisationRate = 1.0
		};
		model.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Modifier, RelativeIntensity = 1.0, AdditionalEffects = modifier.DatabaseString });
		model.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Dependence, RelativeIntensity = 1.0, AdditionalEffects = dependence.DatabaseString });
		model.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Overdose, RelativeIntensity = 1.0, AdditionalEffects = "11 0.002|Armageddon Spice" });

		var drug = new MudSharp.Health.Drug(model, new Mock<IFuturemud>().Object);
		var loadedStep = drug.AdditionalInfoFor<DrugModifierAdditionalInfo>(DrugType.Modifier).Steps.Single();
		Assert.AreEqual(step, loadedStep);
		var loadedDependence = drug.AdditionalInfoFor<DrugDependenceAdditionalInfo>(DrugType.Dependence);
		Assert.AreEqual(2.0, loadedDependence.RelapseExposureMultiplier);
		Assert.AreEqual(0.8, loadedDependence.WithdrawalMaximumHitPointsMultiplier);
		Assert.AreEqual(0.7, loadedDependence.WithdrawalMagicResourceCapacityMultiplier);
		Assert.AreEqual(step, loadedDependence.ModifierSteps.Single());
		var overdose = drug.AdditionalInfoFor<OverdoseAdditionalInfo>(DrugType.Overdose);
		Assert.AreEqual(11L, overdose.TraitId);
		Assert.AreEqual(0.002, overdose.GramsPerTraitPoint, 0.0000001);
		Assert.AreEqual("Armageddon Spice", overdose.Group);
		Assert.AreEqual("11 0.002|Armageddon Spice", overdose.DatabaseString);
	}

	[TestMethod]
	public void LegacyOverdosePayload_UsesPerDrugGroupDefault()
	{
		var model = new MudSharp.Models.Drug { Id = 1, Name = "Legacy", DrugVectors = 2, IntensityPerGram = 1.0, RelativeMetabolisationRate = 1.0 };
		model.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Overdose, RelativeIntensity = 1.0, AdditionalEffects = "11 0.002" });
		var drug = new MudSharp.Health.Drug(model, new Mock<IFuturemud>().Object);
		Assert.AreEqual(string.Empty, drug.AdditionalInfoFor<OverdoseAdditionalInfo>(DrugType.Overdose).Group);
	}

	[TestMethod]
	public void LegacyDependencePayload_UsesNeutralExtensionDefaults()
	{
		var model = new MudSharp.Models.Drug { Id = 1, Name = "Legacy", DrugVectors = 2, IntensityPerGram = 1.0, RelativeMetabolisationRate = 1.0 };
		model.DrugsIntensities.Add(new DrugIntensity { DrugType = (int)DrugType.Dependence, RelativeIntensity = 1.0,
			AdditionalEffects = "1 0.1 10 0.25 5 0.25 -1 1.1 1.1 0.8 1.1 0.5 0 0.5|" });
		var drug = new MudSharp.Health.Drug(model, new Mock<IFuturemud>().Object);
		var info = drug.AdditionalInfoFor<DrugDependenceAdditionalInfo>(DrugType.Dependence);
		Assert.AreEqual(1.0, info.RelapseExposureMultiplier);
		Assert.AreEqual(1.0, info.WithdrawalMaximumHitPointsMultiplier);
		Assert.AreEqual(1.0, info.WithdrawalMagicResourceCapacityMultiplier);
		Assert.AreEqual(0, info.ModifierSteps.Count);
	}
}
