using System.Collections.Generic;
using System.Linq;
using MudSharp.Body.PartProtos;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.Health;
using System;
using MudSharp.Character;
using MudSharp.Effects.Interfaces;
using MudSharp.Magic;
using MudSharp.RPG.Merits.Interfaces;

namespace MudSharp.Body.Implementations;

public partial class Body
{
	#region Drugs

	private bool _drugsChanged;

	public bool DrugsChanged
	{
		get => _drugsChanged;
		set
		{
			if (!_drugsChanged && value)
			{
				Changed = true;
			}

			_drugsChanged = value;
		}
	}

	private readonly List<DrugDosage> _activeDrugDosages = new();
	public IEnumerable<DrugDosage> ActiveDrugDosages => _activeDrugDosages;

	private readonly List<DrugDosage> _latentDrugDosages = new();
	public IEnumerable<DrugDosage> LatentDrugDosages => _latentDrugDosages;

	public void Dose(IDrug drug, DrugVector vector, double grams)
	{
		DrugsChanged = true;
		var latent = _latentDrugDosages.FirstOrDefault(x => x.Drug == drug && x.OriginalVector == vector);
		if (latent == null)
		{
			latent = new DrugDosage { Drug = drug, OriginalVector = vector };
			_latentDrugDosages.Add(latent);
		}

		latent.Grams += grams;
		CheckDrugTick();
	}

	private bool _drugTickOn;

	public void CheckDrugTick()
	{
		if (Actor.State.HasFlag(CharacterState.Stasis) || Actor.State.HasFlag(CharacterState.Dead))
		{
			return;
		}

		if (_activeDrugDosages.Any() || _latentDrugDosages.Any() || NeedsModel.AlcoholLitres > 0.0 ||
		    CombinedEffectsOfType<ICauseDrugEffect>().Any())
		{
			if (_drugTickOn)
			{
				return;
			}

			Gameworld.HeartbeatManager.TenSecondHeartbeat -= DrugTenSecondHeartbeat;
			Gameworld.HeartbeatManager.TenSecondHeartbeat += DrugTenSecondHeartbeat;
			_drugTickOn = true;
			DrugTenSecondHeartbeat();
			StartHealthTick();
			return;
		}

		if (!_activeDrugDosages.Any() && !_latentDrugDosages.Any() && NeedsModel.AlcoholLitres <= 0.0 &&
		    !CombinedEffectsOfType<ICauseDrugEffect>().Any() && _drugTickOn)
		{
			EndDrugTick();
		}
	}

	private void EndDrugTick()
	{
		Gameworld.HeartbeatManager.TenSecondHeartbeat -= DrugTenSecondHeartbeat;
		_drugTickOn = false;
	}

	private void DrugTenSecondHeartbeat()
	{
		ProcessLatentDrugs();
		ApplyDrugEffects();
		MetaboliseActiveDrugs();
		CheckHealthStatus();
	}

	public void Sober()
	{
		_activeDrugDosages.Clear();
		_latentDrugDosages.Clear();
		NeedsModel.AlcoholLitres = 0.0;
		ApplyDrugEffects();
		DrugsChanged = true;
	}

	private void ProcessLatentDrugs()
	{
		foreach (var drug in LatentDrugDosages.ToList())
		{
			var rate = 0.5;
			switch (drug.OriginalVector)
			{
				case DrugVector.Ingested:
					rate = 0.02;
					break;
				case DrugVector.Inhaled:
					rate = 0.4;
					break;
				case DrugVector.Touched:
					rate = 0.05;
					break;
			}

			var activeDose = _activeDrugDosages.FirstOrDefault(x => x.Drug == drug.Drug);
			if (activeDose == null)
			{
				activeDose = new DrugDosage { Drug = drug.Drug, OriginalVector = DrugVector.Injected };
				_activeDrugDosages.Add(activeDose);
			}

			var amount = Math.Max(0.0001, rate * drug.Grams);
			activeDose.Grams += amount;
			drug.Grams -= amount;
			if (drug.Grams <= 0)
			{
				_latentDrugDosages.Remove(drug);
			}

			DrugsChanged = true;
		}
	}

	private void ApplyDrugEffects()
	{
		var neutralisingEffects = new DoubleCounter<DrugType>();
		var effectIntensities = new DoubleCounter<DrugType>();

		// Apply nauseu from drunkenness
		var bac = 10.0 * NeedsModel.AlcoholLitres / CurrentBloodVolumeLitres;
		if (bac > 0.0)
		{
			effectIntensities[DrugType.Nausea] = 0.0;
		}

		var healingRateIntensity = 0.0;
		var healingDifficultyIntensity = 0.0;

		// Apply drug-neutralising drugs
		var drugIntensities = new DoubleCounter<IDrug>();
		foreach (var drug in ActiveDrugDosages)
		{
			drugIntensities[drug.Drug] += drug.Grams;
			if (drug.Drug.DrugTypes.Contains(DrugType.NeutraliseSpecificDrug))
			{
				var drugs = drug.Drug
				                .ExtraInfoFor(DrugType.NeutraliseSpecificDrug)
				                .Split(' ')
				                .SelectNotNull(x => Gameworld.Drugs.Get(long.Parse(x)))
				                .AsEnumerable();
				foreach (var sdrug in drugs)
				{
					drugIntensities[sdrug] -= drug.Drug.IntensityForType(DrugType.NeutraliseSpecificDrug) * drug.Grams;
				}
			}
		}

		// Sum impacts from effects
		foreach (var effect in CombinedEffectsOfType<ICauseDrugEffect>())
		foreach (var drugType in effect.AffectedDrugTypes)
		{
			effectIntensities[drugType] += effect.AddedIntensity(Actor, drugType);
		}

		// Sum impacts from drugs
		var magicCapabilities = new DoubleCounter<IMagicCapability>();
		var damagedOrgans = new DoubleCounter<BodypartTypeEnum>();
		var specificMerits = Actor.Merits.OfType<ISpecificDrugResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
		foreach (var (drug, grams) in drugIntensities)
		{
			if (grams <= 0.0)
			{
				continue;
			}

			// Apply effects from merits that target specific drugs
			var adjustedgrams = specificMerits.Aggregate(1.0, (sum, x) => sum * x.MultiplierForDrug(drug)) * grams;

			foreach (var drugEffect in drug.DrugTypes)
			{
				if (drugEffect == DrugType.NeutraliseDrugEffect)
				{
					foreach (var neutralDrug in drug.ExtraInfoFor(DrugType.NeutraliseDrugEffect).Split(' ')
					                                .Select(x => (DrugType)int.Parse(x)))
					{
						neutralisingEffects[neutralDrug] += drug.IntensityForType(drugEffect) * adjustedgrams;
					}

					continue;
				}

				if (drugEffect == DrugType.MagicAbility)
				{
					var capabilityString = drug.ExtraInfoFor(DrugType.MagicAbility);
					var capabilities = capabilityString.Split(' ')
					                                   .Select(x => Gameworld.MagicCapabilities.Get(int.Parse(x)))
					                                   .ToList();
					foreach (var capability in capabilities)
					{
						magicCapabilities[capability] += drug.IntensityForType(DrugType.MagicAbility) * adjustedgrams;
					}
				}

				if (drugEffect == DrugType.HealingRate)
				{
					var split = drug.ExtraInfoFor(DrugType.HealingRate).Split(' ');
					healingRateIntensity += double.Parse(split[0]) * (drug.IntensityForType(drugEffect) * adjustedgrams);
					healingDifficultyIntensity += double.Parse(split[1]) * (drug.IntensityForType(drugEffect) * adjustedgrams);
				}

				if (drugEffect == DrugType.BodypartDamage)
				{
					foreach (var organ in drug.ExtraInfoFor(DrugType.BodypartDamage).Split(' ')
					                          .Select(x => (BodypartTypeEnum)int.Parse(x)))
					{
						damagedOrgans[organ] += drug.IntensityForType(drugEffect) * adjustedgrams;
					}
				}

				effectIntensities[drugEffect] += drug.IntensityForType(drugEffect) * adjustedgrams;
			}
		}

		// Apply resistance merits
		var resistanceMerits = Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
		foreach (var effect in effectIntensities.ToArray())
		{
			effectIntensities[effect.Key] = effect.Value * resistanceMerits.Aggregate(1.0, (sum, x) => sum * x.ModifierForDrugType(effect.Key));
		}

		// Apply individual effects
		var applicableCapabilities = magicCapabilities.Where(x => x.Value > 1.0);
		foreach (var effect in effectIntensities)
		{
			switch (effect.Key)
			{
				case DrugType.Analgesic:
					var analgesicEffect = EffectsOfType<Analgesic>().FirstOrDefault();
					if (analgesicEffect == null)
					{
						analgesicEffect = new Analgesic(this, 0);
						EffectHandler.AddEffect(analgesicEffect);
					}

					analgesicEffect.AnalgesicIntensityPerGramMass =
						(effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
						(Weight *
						 Gameworld.UnitManager.BaseWeightToKilograms *
						 0.001);
					break;
				case DrugType.Anesthesia:
					var anasthesiaEffect = EffectsOfType<Anesthesia>().FirstOrDefault();
					if (anasthesiaEffect == null)
					{
						anasthesiaEffect = new Anesthesia(this, 0);
						EffectHandler.AddEffect(anasthesiaEffect);
					}

					anasthesiaEffect.IntensityPerGramMass =
						(effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
						(Weight * Gameworld.UnitManager.BaseWeightToKilograms *
						 0.001);
					break;
				case DrugType.Antibiotic:
					var antibioticEffect = EffectsOfType<Antibiotic>().FirstOrDefault();
					if (antibioticEffect == null)
					{
						antibioticEffect = new Antibiotic(this, 0);
						EffectHandler.AddEffect(antibioticEffect);
					}

					antibioticEffect.IntensityPerGramMass =
						(effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
						(Weight * Gameworld.UnitManager.BaseWeightToKilograms *
						 0.001);
					break;
				case DrugType.Immunosuppressive:
					var immuneEffect = EffectsOfType<Immunosupressant>().FirstOrDefault();
					if (immuneEffect == null)
					{
						immuneEffect = new Immunosupressant(this, 0);
						EffectHandler.AddEffect(immuneEffect);
					}

					immuneEffect.IntensityPerGramMass =
						(effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
						(Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
					break;
				case DrugType.Rage:
					var rageEffect = EffectsOfType<MurderousRage>().FirstOrDefault();
					if (rageEffect == null)
					{
						rageEffect = new MurderousRage(this, 0);
						EffectHandler.AddEffect(rageEffect, TimeSpan.FromSeconds(7));
					}

					rageEffect.IntensityPerGramMass = (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
					                                  (Weight * Gameworld.UnitManager.BaseWeightToKilograms *
					                                   0.001);
					break;
				case DrugType.Pacifism:
					var peaceEffect = EffectsOfType<PacifismDrug>().FirstOrDefault();
					if (peaceEffect == null)
					{
						peaceEffect = new PacifismDrug(this, 0);
						EffectHandler.AddEffect(peaceEffect);
					}

					peaceEffect.IntensityPerGramMass = (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
					                                   (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
					break;
				case DrugType.StaminaRegen:
					GainStamina((effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
					            (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001));
					break;
				case DrugType.Nausea:
					var nauseaEffect = EffectsOfType<Nausea>().FirstOrDefault();
					if (nauseaEffect == null)
					{
						nauseaEffect = new Nausea(this, 0, 0);
						EffectHandler.AddEffect(nauseaEffect, TimeSpan.FromSeconds(35));
					}

					nauseaEffect.IntensityPerGramMass =
						(effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
						(Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
					nauseaEffect.BloodAlcoholContent = bac;
					break;
				case DrugType.HealingRate:
					var healingEffect = EffectsOfType<HealingRateDrug>().FirstOrDefault();
					if (healingEffect == null)
					{
						healingEffect = new HealingRateDrug(this);
						EffectHandler.AddEffect(healingEffect);
					}

					healingEffect.HealingDifficultyStages = (int)healingDifficultyIntensity;
					healingEffect.HealingRateMultiplier = 1.0 + healingRateIntensity;
					break;
				case DrugType.MagicAbility:
					if (!applicableCapabilities.Any())
					{
						break;
					}

					var magicEffect = EffectsOfType<DrugInducedMagicCapability>().FirstOrDefault();
					if (magicEffect == null)
					{
						magicEffect = new DrugInducedMagicCapability(this);
						EffectHandler.AddEffect(magicEffect);
					}

					magicEffect.NewCapabilities(applicableCapabilities.Select(x => x.Key).ToArray());
					break;
				case DrugType.BodypartDamage:
				{
					var wounds = new List<IWound>();
					foreach (var organ in damagedOrgans)
					{
						var damage = (organ.Value - neutralisingEffects.ValueOrDefault(effect.Key)) *
						             Gameworld.GetStaticDouble("OrganDamagePerDrugIntensity");
						var floor = Gameworld.GetStaticDouble(
							"OrganDamageFloor"); // TODO - merits and flaws that alter this
						if (damage <= floor)
						{
							break;
						}

						var parts = new List<IBodypart>();
						parts.AddRange(Organs.Where(x => x.BodypartType == organ.Key));
						parts.AddRange(Bodyparts.Where(x => x.BodypartType == organ.Key));
						parts.AddRange(Bones.Where(x => x.BodypartType == organ.Key));

						foreach (var part in parts)
						{
							wounds.AddRange(PassiveSufferDamage(new Damage
							{
								DamageType = DamageType.Cellular,
								Bodypart = part,
								ActorOrigin = Actor,
								DamageAmount = damage / parts.Count,
								PainAmount = damage / parts.Count
							}));
						}
					}

					wounds.ProcessPassiveWounds();
					break;
				}
			}
		}

		// Tidy up Effects that have worn off
		if (effectIntensities.ValueOrDefault(DrugType.Analgesic) -
		    neutralisingEffects.ValueOrDefault(DrugType.Analgesic) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<Analgesic>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Anesthesia) -
		    neutralisingEffects.ValueOrDefault(DrugType.Anesthesia) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<Anesthesia>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Antibiotic) -
		    neutralisingEffects.ValueOrDefault(DrugType.Antibiotic) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<Antibiotic>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Immunosuppressive) -
		    neutralisingEffects.ValueOrDefault(DrugType.Immunosuppressive) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<Immunosupressant>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Rage) - neutralisingEffects.ValueOrDefault(DrugType.Rage) <= 0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<MurderousRage>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Pacifism) -
		    neutralisingEffects.ValueOrDefault(DrugType.Pacifism) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<PacifismDrug>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.Nausea) - neutralisingEffects.ValueOrDefault(DrugType.Nausea) <=
		    0.0 && bac <= 0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<Nausea>(), true);
		}

		if (effectIntensities.ValueOrDefault(DrugType.HealingRate) -
		    neutralisingEffects.ValueOrDefault(DrugType.HealingRate) <=
		    0.0)
		{
			EffectHandler.RemoveAllEffects(x => x.IsEffectType<HealingRateDrug>(), true);
		}

		if (!applicableCapabilities.Any())
		{
			EffectHandler.RemoveAllEffects<DrugInducedMagicCapability>(fireRemovalAction: true);
		}

		// Kick off the health tick if necessary
		if (effectIntensities.Any() || bac > 0.0)
		{
			StartHealthTick();
		}
	}

	private void MetaboliseActiveDrugs()
	{
		var rate = LiverAlcoholRemovalKilogramsPerHour * 10000.0 / 3600.0;
		foreach (var drug in ActiveDrugDosages.ToList())
		{
			var removed = rate * drug.Drug.RelativeMetabolisationRate;
			drug.Grams -= removed;
			if (drug.Grams <= 0)
			{
				_activeDrugDosages.Remove(drug);
			}

			DrugsChanged = true;
		}
	}

	private void SaveDrugs(MudSharp.Models.Body body)
	{
		var drugsToDelete =
			body.BodiesDrugDoses.Where(
				x =>
					!_activeDrugDosages.Any(y => y.Drug.Id == x.DrugId && x.Active) &&
					!_latentDrugDosages.Any(y => y.Drug.Id == x.DrugId && !x.Active)).ToList();
		foreach (var drug in drugsToDelete)
		{
			body.BodiesDrugDoses.Remove(drug);
		}

		foreach (var item in ActiveDrugDosages)
		{
			var dbitem = body.BodiesDrugDoses.FirstOrDefault(x => x.Active && x.DrugId == item.Drug.Id);
			if (dbitem == null)
			{
				dbitem = new Models.BodyDrugDose
				{
					DrugId = item.Drug.Id,
					Active = true,
					OriginalVector = (int)item.OriginalVector
				};
				body.BodiesDrugDoses.Add(dbitem);
			}

			dbitem.Grams = item.Grams;
		}

		foreach (var item in LatentDrugDosages)
		{
			var dbitem = body.BodiesDrugDoses.FirstOrDefault(x => !x.Active && x.DrugId == item.Drug.Id);
			if (dbitem == null)
			{
				dbitem = new Models.BodyDrugDose
				{
					DrugId = item.Drug.Id,
					Active = false,
					OriginalVector = (int)item.OriginalVector
				};
				body.BodiesDrugDoses.Add(dbitem);
			}

			dbitem.Grams = item.Grams;
		}

		DrugsChanged = false;
	}

	#endregion
}