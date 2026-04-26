using MudSharp.Body.PartProtos;
using MudSharp.Character;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.Planes;
using MudSharp.RPG.Merits.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.Body.Implementations;

public partial class Body
{
    #region Drugs

    private void ApplyThermalImbalanceProgress(double progress)
    {
        if (progress == 0.0 ||
            !Gameworld.GetStaticBool(ThermalImbalanceConsequenceModel.EnabledStaticConfiguration))
        {
            return;
        }

        ThermalImbalance thermal = Actor.CombinedEffectsOfType<ThermalImbalance>()
            .OrderByDescending(x =>
                ThermalImbalanceConsequenceModel.EffectSeverityScore(x.TemperatureStatus, x.ImbalanceProgress))
            .FirstOrDefault();
        if (thermal == null)
        {
            thermal = new ThermalImbalance(Actor);
            Actor.AddEffect(thermal);
        }

        thermal.ImbalanceProgress += progress;
    }

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
        Dose(drug, vector, grams, null);
    }

    public void Dose(IDrug drug, DrugVector vector, double grams, object originator)
    {
        DrugsChanged = true;
        DrugDosage latent = _latentDrugDosages.FirstOrDefault(x =>
            x.Drug == drug &&
            x.OriginalVector == vector &&
            Equals(x.Originator, originator));
        if (latent == null)
        {
            latent = new DrugDosage { Drug = drug, OriginalVector = vector, Originator = originator };
            _latentDrugDosages.Add(latent);
        }

        latent.Grams += grams;
        CheckDrugTick();
    }

    public void RemoveDrugDosages(Predicate<DrugDosage> predicate)
    {
        int removed = _activeDrugDosages.RemoveAll(predicate);
        removed += _latentDrugDosages.RemoveAll(predicate);
        if (removed <= 0)
        {
            return;
        }

        DrugsChanged = true;
        ApplyDrugEffects();
        CheckDrugTick();
    }

    private bool _drugTickOn;

    public void CheckDrugTick()
    {
        if (!IsActiveCharacterBody)
        {
            EndDrugTick();
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
        if (!_drugTickOn)
        {
            return;
        }

        Gameworld.HeartbeatManager.TenSecondHeartbeat -= DrugTenSecondHeartbeat;
        _drugTickOn = false;
    }

    private void DrugTenSecondHeartbeat()
    {
        if (!IsActiveCharacterBody)
        {
            EndDrugTick();
            return;
        }

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
        foreach (DrugDosage drug in LatentDrugDosages.ToList())
        {
            double rate = 0.5;
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

            DrugDosage activeDose = _activeDrugDosages.FirstOrDefault(x =>
                x.Drug == drug.Drug &&
                Equals(x.Originator, drug.Originator));
            if (activeDose == null)
            {
                activeDose = new DrugDosage
                {
                    Drug = drug.Drug,
                    OriginalVector = DrugVector.Injected,
                    Originator = drug.Originator
                };
                _activeDrugDosages.Add(activeDose);
            }

            double amount = Math.Max(0.0001, rate * drug.Grams);
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
        DoubleCounter<DrugType> neutralisingEffects = new();
        DoubleCounter<DrugType> effectIntensities = new();

        // Apply nauseu from drunkenness
        double bac = 10.0 * NeedsModel.AlcoholLitres / CurrentBloodVolumeLitres;
        if (bac > 0.0)
        {
            effectIntensities[DrugType.Nausea] = 0.0;
        }

        double healingRateIntensity = 0.0;
        double healingDifficultyIntensity = 0.0;

        // Apply drug-neutralising drugs
        DoubleCounter<IDrug> drugIntensities = new();
        foreach (DrugDosage drug in ActiveDrugDosages)
        {
            drugIntensities[drug.Drug] += drug.Grams;
            if (drug.Drug.DrugTypes.Contains(DrugType.NeutraliseSpecificDrug))
            {
                IEnumerable<IDrug> drugs = drug.Drug
                                .AdditionalInfoFor<NeutraliseSpecificDrugAdditionalInfo>(DrugType.NeutraliseSpecificDrug)
                                .NeutralisedIds
                                .SelectNotNull(x => Gameworld.Drugs.Get(x));
                foreach (IDrug sdrug in drugs)
                {
                    drugIntensities[sdrug] -= drug.Drug.IntensityForType(DrugType.NeutraliseSpecificDrug) * drug.Grams;
                }
            }
        }

        // Sum impacts from effects
        foreach (ICauseDrugEffect effect in CombinedEffectsOfType<ICauseDrugEffect>())
        {
            foreach (DrugType drugType in effect.AffectedDrugTypes)
            {
                effectIntensities[drugType] += effect.AddedIntensity(Actor, drugType);
            }
        }

        // Sum impacts from drugs
        DoubleCounter<IMagicCapability> magicCapabilities = new();
        DoubleCounter<BodypartTypeEnum> damagedOrgans = new();
        DoubleCounter<BodypartTypeEnum> organFunctionMods = new();
        PlanarStateAdditionalInfo planarStateInfo = null;
        double planarStateIntensity = 0.0;
        List<ISpecificDrugResistanceMerit> specificMerits = Actor.Merits.OfType<ISpecificDrugResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
        foreach ((IDrug drug, double grams) in drugIntensities)
        {
            if (grams <= 0.0)
            {
                continue;
            }

            // Apply effects from merits that target specific drugs
            double adjustedgrams = specificMerits.Aggregate(1.0, (sum, x) => sum * x.MultiplierForDrug(drug)) * grams;

            foreach (DrugType drugEffect in drug.DrugTypes)
            {
                if (drugEffect == DrugType.NeutraliseDrugEffect)
                {
                    foreach (DrugType neutralDrug in drug.AdditionalInfoFor<NeutraliseDrugAdditionalInfo>(DrugType.NeutraliseDrugEffect).NeutralisedTypes)
                    {
                        neutralisingEffects[neutralDrug] += drug.IntensityForType(drugEffect) * adjustedgrams;
                    }

                    continue;
                }

                if (drugEffect == DrugType.MagicAbility)
                {
                    List<IMagicCapability> capabilities = drug.AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility)
                                           .MagicCapabilityIds
                                                       .Select(x => Gameworld.MagicCapabilities.Get(x))
                                                       .ToList();
                    foreach (IMagicCapability capability in capabilities)
                    {
                        magicCapabilities[capability] += drug.IntensityForType(DrugType.MagicAbility) * adjustedgrams;
                    }
                }

                if (drugEffect == DrugType.HealingRate)
                {
                    HealingRateAdditionalInfo split = drug.AdditionalInfoFor<HealingRateAdditionalInfo>(DrugType.HealingRate);
                    healingRateIntensity += split.HealingRateIntensity * (drug.IntensityForType(drugEffect) * adjustedgrams);
                    healingDifficultyIntensity += split.HealingDifficultyIntensity * (drug.IntensityForType(drugEffect) * adjustedgrams);
                }

                if (drugEffect == DrugType.BodypartDamage)
                {
                    foreach (BodypartTypeEnum organ in drug.AdditionalInfoFor<BodypartDamageAdditionalInfo>(DrugType.BodypartDamage).BodypartTypes)
                    {
                        damagedOrgans[organ] += drug.IntensityForType(drugEffect) * adjustedgrams;
                    }
                }

                if (drugEffect == DrugType.OrganFunction)
                {
                    foreach (BodypartTypeEnum organ in drug.AdditionalInfoFor<OrganFunctionAdditionalInfo>(DrugType.OrganFunction).OrganTypes)
                    {
                        organFunctionMods[organ] += drug.IntensityForType(drugEffect) * adjustedgrams;
                    }
                }

                if (drugEffect == DrugType.PlanarState)
                {
                    var intensity = drug.IntensityForType(drugEffect) * adjustedgrams;
                    if (intensity > planarStateIntensity)
                    {
                        planarStateIntensity = intensity;
                        planarStateInfo = drug.AdditionalInfoFor<PlanarStateAdditionalInfo>(DrugType.PlanarState);
                    }
                }

                effectIntensities[drugEffect] += drug.IntensityForType(drugEffect) * adjustedgrams;
            }
        }

        // Apply resistance merits
        List<IDrugEffectResistanceMerit> resistanceMerits = Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
        foreach (KeyValuePair<DrugType, double> effect in effectIntensities.ToArray())
        {
            effectIntensities[effect.Key] = effect.Value * resistanceMerits.Aggregate(1.0, (sum, x) => sum * x.ModifierForDrugType(effect.Key));
        }

        // Apply individual effects
        IEnumerable<KeyValuePair<IMagicCapability, double>> applicableCapabilities = magicCapabilities.Where(x => x.Value > 1.0);
        foreach (KeyValuePair<DrugType, double> effect in effectIntensities)
        {
            switch (effect.Key)
            {
                case DrugType.Analgesic:
                    Analgesic analgesicEffect = EffectsOfType<Analgesic>().FirstOrDefault();
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
                    Anesthesia anasthesiaEffect = EffectsOfType<Anesthesia>().FirstOrDefault();
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
                    Antibiotic antibioticEffect = EffectsOfType<Antibiotic>().FirstOrDefault();
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
                case DrugType.Antifungal:
                    Antifungal antifungalEffect = EffectsOfType<Antifungal>().FirstOrDefault();
                    if (antifungalEffect == null)
                    {
                        antifungalEffect = new Antifungal(this, 0);
                        EffectHandler.AddEffect(antifungalEffect);
                    }

                    antifungalEffect.IntensityPerGramMass =
                        (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                        (Weight * Gameworld.UnitManager.BaseWeightToKilograms *
                         0.001);
                    break;
                case DrugType.Immunosuppressive:
                    Immunosupressant immuneEffect = EffectsOfType<Immunosupressant>().FirstOrDefault();
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
                    MurderousRage rageEffect = EffectsOfType<MurderousRage>().FirstOrDefault();
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
                    PacifismDrug peaceEffect = EffectsOfType<PacifismDrug>().FirstOrDefault();
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
                    Nausea nauseaEffect = EffectsOfType<Nausea>().FirstOrDefault();
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
                case DrugType.Adrenaline:
                    {
                        double intensity = (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                                        (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
                        AdrenalineRush adrenalineEffect = EffectsOfType<AdrenalineRush>().FirstOrDefault();
                        if (adrenalineEffect == null)
                        {
                            adrenalineEffect = new AdrenalineRush(this, 0);
                            EffectHandler.AddEffect(adrenalineEffect);
                        }

                        adrenalineEffect.IntensityPerGramMass = intensity;

                        double supportFloor = Math.Min(Gameworld.GetStaticDouble("AdrenalineHeartSupportMaximumFloor"),
                            intensity * Gameworld.GetStaticDouble("AdrenalineHeartSupportFloorPerIntensity"));
                        foreach (HeartProto heart in Organs.OfType<HeartProto>())
                        {
                            AdrenalineHeartSupportEffect heartSupport =
                            EffectsOfType<AdrenalineHeartSupportEffect>().FirstOrDefault(x => x.Organ == heart);
                            if (heartSupport == null)
                            {
                                heartSupport = new AdrenalineHeartSupportEffect(this, heart, supportFloor, ExertionLevel.Normal);
                                EffectHandler.AddEffect(heartSupport);
                            }

                            heartSupport.Floor = supportFloor;
                            heartSupport.ExertionCap = ExertionLevel.Normal;
                        }

                        double thermalGain = intensity * Gameworld.GetStaticDouble("AdrenalineThermalGainPerIntensity");
                        ApplyThermalImbalanceProgress(thermalGain);

                        double cardiacDamage = intensity * Gameworld.GetStaticDouble("AdrenalineCardiacDamagePerIntensity");
                        if (cardiacDamage > 0.0)
                        {
                            List<HeartProto> hearts = Organs.OfType<HeartProto>().ToList();
                            if (hearts.Any())
                            {
                                List<IWound> wounds = new();
                                foreach (HeartProto heart in hearts)
                                {
                                    wounds.AddRange(PassiveSufferDamage(new Damage
                                    {
                                        DamageType = DamageType.Cellular,
                                        Bodypart = heart,
                                        ActorOrigin = Actor,
                                        DamageAmount = cardiacDamage / hearts.Count,
                                        PainAmount = 0.0
                                    }));
                                }

                                if (wounds.Any())
                                {
                                    wounds.ProcessPassiveWounds();
                                }
                            }
                        }

                        break;
                    }
                case DrugType.Paralysis:
                    DrugInducedParalysis paralysisEffect = EffectsOfType<DrugInducedParalysis>().FirstOrDefault();
                    if (paralysisEffect == null)
                    {
                        paralysisEffect = new DrugInducedParalysis(this, 0);
                        EffectHandler.AddEffect(paralysisEffect);
                    }

                    paralysisEffect.IntensityPerGramMass =
                        (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                        (Weight * Gameworld.UnitManager.BaseWeightToKilograms *
                         0.001);
                    break;
                case DrugType.HealingRate:
                    HealingRateDrug healingEffect = EffectsOfType<HealingRateDrug>().FirstOrDefault();
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

                    DrugInducedMagicCapability magicEffect = EffectsOfType<DrugInducedMagicCapability>().FirstOrDefault();
                    if (magicEffect == null)
                    {
                        magicEffect = new DrugInducedMagicCapability(this);
                        EffectHandler.AddEffect(magicEffect);
                    }

                    magicEffect.NewCapabilities(applicableCapabilities.Select(x => x.Key).ToArray());
                    break;
                case DrugType.BodypartDamage:
                    {
                        List<IWound> wounds = new();
                        foreach (KeyValuePair<BodypartTypeEnum, double> organ in damagedOrgans)
                        {
                            double damage = (organ.Value - neutralisingEffects.ValueOrDefault(effect.Key)) *
                                         Gameworld.GetStaticDouble("OrganDamagePerDrugIntensity");
                            double floor = Gameworld.GetStaticDouble(
                                "OrganDamageFloor"); // TODO - merits and flaws that alter this
                            if (damage <= floor)
                            {
                                break;
                            }

                            List<IBodypart> parts = new();
                            parts.AddRange(Organs.Where(x => x.BodypartType == organ.Key));
                            parts.AddRange(Bodyparts.Where(x => x.BodypartType == organ.Key));
                            parts.AddRange(Bones.Where(x => x.BodypartType == organ.Key));

                            foreach (IBodypart part in parts)
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
                case DrugType.OrganFunction:
                    {
                        OrganFunctionDrugEffect organEffect = EffectsOfType<OrganFunctionDrugEffect>().FirstOrDefault();
                        if (organEffect == null)
                        {
                            organEffect = new OrganFunctionDrugEffect(this);
                            EffectHandler.AddEffect(organEffect);
                        }
                        Dictionary<BodypartTypeEnum, double> map = organFunctionMods.ToDictionary(x => x.Key,
                                x => (x.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                                     (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001));
                        organEffect.SetBonuses(map);
                        break;
                    }
                case DrugType.VisionImpairment:
                    {
                        VisionImpairmentDrugEffect visionEffect = EffectsOfType<VisionImpairmentDrugEffect>().FirstOrDefault();
                        if (visionEffect == null)
                        {
                            visionEffect = new VisionImpairmentDrugEffect(this);
                            EffectHandler.AddEffect(visionEffect);
                        }
                        double intensity = (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                                        (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
                        visionEffect.VisionMultiplier = Math.Max(0.0, 1.0 - intensity);
                        break;
                    }
                case DrugType.ThermalImbalance:
                    {
                        ApplyThermalImbalanceProgress((effect.Value - neutralisingEffects.ValueOrDefault(effect.Key)) /
                                                      (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001));
                        break;
                    }
                case DrugType.PlanarState:
                    {
                        if (planarStateInfo is null || effect.Value - neutralisingEffects.ValueOrDefault(effect.Key) <= 0.0)
                        {
                            break;
                        }

                        var plane = Gameworld.Planes.Get(planarStateInfo.PlaneId) ?? Gameworld.DefaultPlane;
                        if (plane is null)
                        {
                            break;
                        }

                        var definition = planarStateInfo.State.EqualToAny("corporeal", "manifest", "manifested")
                            ? PlanarPresenceDefinition.Manifested(plane)
                            : PlanarPresenceDefinition.NonCorporeal(plane, planarStateInfo.VisibleToDefaultPlane);
                        var planarEffect = EffectsOfType<DrugInducedPlanarStateEffect>().FirstOrDefault();
                        if (planarEffect is null)
                        {
                            planarEffect = new DrugInducedPlanarStateEffect(this, definition);
                            EffectHandler.AddEffect(planarEffect);
                        }
                        else
                        {
                            planarEffect.UpdateDefinition(definition);
                        }

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

        if (effectIntensities.ValueOrDefault(DrugType.Antifungal) -
            neutralisingEffects.ValueOrDefault(DrugType.Antifungal) <=
            0.0)
        {
            EffectHandler.RemoveAllEffects(x => x.IsEffectType<Antifungal>(), true);
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

        if (effectIntensities.ValueOrDefault(DrugType.Adrenaline) -
            neutralisingEffects.ValueOrDefault(DrugType.Adrenaline) <= 0.0)
        {
            EffectHandler.RemoveAllEffects(x => x.IsEffectType<AdrenalineRush>(), true);
            EffectHandler.RemoveAllEffects(x => x.IsEffectType<AdrenalineHeartSupportEffect>(), true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.Paralysis) -
            neutralisingEffects.ValueOrDefault(DrugType.Paralysis) <= 0.0)
        {
            EffectHandler.RemoveAllEffects(x => x.IsEffectType<DrugInducedParalysis>(), true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.HealingRate) -
            neutralisingEffects.ValueOrDefault(DrugType.HealingRate) <=
            0.0)
        {
            EffectHandler.RemoveAllEffects(x => x.IsEffectType<HealingRateDrug>(), true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.OrganFunction) -
            neutralisingEffects.ValueOrDefault(DrugType.OrganFunction) <=
            0.0)
        {
            EffectHandler.RemoveAllEffects<OrganFunctionDrugEffect>(fireRemovalAction: true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.VisionImpairment) -
            neutralisingEffects.ValueOrDefault(DrugType.VisionImpairment) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<VisionImpairmentDrugEffect>(fireRemovalAction: true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.PlanarState) -
            neutralisingEffects.ValueOrDefault(DrugType.PlanarState) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<DrugInducedPlanarStateEffect>(fireRemovalAction: true);
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
        double rate = LiverAlcoholRemovalKilogramsPerHour * 10000.0 / 3600.0;
        foreach (DrugDosage drug in ActiveDrugDosages.ToList())
        {
            double removed = rate * drug.Drug.RelativeMetabolisationRate;
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
        List<BodyDrugDose> drugsToDelete =
            body.BodiesDrugDoses.Where(
                x =>
                    !_activeDrugDosages.Any(y => y.Drug.Id == x.DrugId && x.Active) &&
                    !_latentDrugDosages.Any(y => y.Drug.Id == x.DrugId && !x.Active)).ToList();
        foreach (BodyDrugDose drug in drugsToDelete)
        {
            body.BodiesDrugDoses.Remove(drug);
        }

        foreach (DrugDosage item in ActiveDrugDosages)
        {
            BodyDrugDose dbitem = body.BodiesDrugDoses.FirstOrDefault(x => x.Active && x.DrugId == item.Drug.Id);
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

        foreach (DrugDosage item in LatentDrugDosages)
        {
            BodyDrugDose dbitem = body.BodiesDrugDoses.FirstOrDefault(x => !x.Active && x.DrugId == item.Drug.Id);
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
