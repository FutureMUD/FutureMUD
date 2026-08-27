using MudSharp.Health;
using MudSharp.Health.Infections;

#nullable enable

namespace MudSharp.Body.Implementations;

public partial class Body
{
	internal void RestoreCombatSimulationEffects(XElement effects)
	{
		LoadEffects(effects);
		ScheduleCachedEffects();
	}

	internal void CopyCombatSimulationBiologyFrom(IBody source)
	{
		ExecuteWithSuppressedHealthFeedback(() => CopyCombatSimulationBiologyFromCore(source));
	}

	private void CopyCombatSimulationBiologyFromCore(IBody source)
	{
		_currentBloodVolumeLitres = Math.Clamp(source.CurrentBloodVolumeLitres, 0.0, TotalBloodVolumeLitres);
		Changed = true;
		CurrentStamina = source.CurrentStamina;
		CurrentExertion = source.CurrentExertion;
		LongtermExertion = source.LongtermExertion;
		HeldBreathTime = source.HeldBreathTime;

		foreach (var dosage in source.ActiveDrugDosages)
		{
			DoseImmediate(dosage.Drug, dosage.OriginalVector, dosage.Grams, dosage.Originator);
		}

		foreach (var dosage in source.LatentDrugDosages)
		{
			Dose(dosage.Drug, dosage.OriginalVector, dosage.Grams, dosage.Originator);
		}

		var woundMap = new Dictionary<IWound, IWound>(ReferenceEqualityComparer.Instance);
		foreach (var sourceWound in source.Wounds)
		{
			var targetPart = sourceWound.Bodypart is null
				? null
				: Bodyparts.FirstOrDefault(x => x.Id == sourceWound.Bodypart.Id);
			var wounds = SufferDamage(new Damage
			{
				DamageType = sourceWound.DamageType,
				DamageAmount = sourceWound.OriginalDamage,
				PainAmount = sourceWound.CurrentPain,
				ShockAmount = sourceWound.CurrentShock,
				StunAmount = sourceWound.CurrentStun,
				Bodypart = targetPart
			}).ToList();
			var wound = wounds.FirstOrDefault(x => x.DamageType == sourceWound.DamageType) ?? wounds.FirstOrDefault();
			if (wound is null)
			{
				continue;
			}

			wound.OriginalDamage = sourceWound.OriginalDamage;
			wound.CurrentDamage = sourceWound.CurrentDamage;
			wound.CurrentPain = sourceWound.CurrentPain;
			wound.CurrentShock = sourceWound.CurrentShock;
			wound.CurrentStun = sourceWound.CurrentStun;
			wound.BleedStatus = sourceWound.BleedStatus;
			woundMap[sourceWound] = wound;
		}

		foreach (var sourceInfection in source.PartInfections)
		{
			var targetWound = sourceInfection.Wound is not null && woundMap.TryGetValue(sourceInfection.Wound, out var wound)
				? wound
				: null;
			var targetPart = sourceInfection.Bodypart is null
				? null
				: Bodyparts.Concat(Organs).FirstOrDefault(x => x.Id == sourceInfection.Bodypart.Id);
			var infection = Infection.LoadNewInfection(
				sourceInfection.InfectionType,
				sourceInfection.VirulenceDifficulty,
				sourceInfection.Intensity,
				this,
				targetWound,
				targetPart,
				sourceInfection.Virulence);
			if (infection is Infection concreteInfection)
			{
				concreteInfection.Immunity = sourceInfection.Immunity;
				if (sourceInfection is Infection concreteSource)
				{
					concreteInfection.InfectionStage = concreteSource.InfectionStage;
				}
			}

			AddInfection(infection);
			if (targetWound is not null)
			{
				targetWound.Infection = infection;
			}
		}

		CalculateOrganFunctions(true);
		EvaluateWounds();
	}
}
