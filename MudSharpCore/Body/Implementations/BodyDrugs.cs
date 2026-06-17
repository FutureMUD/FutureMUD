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

    private const string DrugDependenceEnabledStaticConfiguration = "DrugDependenceEnabled";
    private const string DrugCoagulationMinimumMultiplierStaticConfiguration = "DrugCoagulationMinimumMultiplier";
    private const string DrugCoagulationMaximumMultiplierStaticConfiguration = "DrugCoagulationMaximumMultiplier";
    private const string DrugRespirationMinimumMultiplierStaticConfiguration = "DrugRespirationMinimumMultiplier";
    private const string DrugRespirationMaximumMultiplierStaticConfiguration = "DrugRespirationMaximumMultiplier";

    private sealed class DrugExposureRecord
    {
        public required IDrug Drug { get; init; }
        public double Exposure { get; set; }
        public double PeakExposure { get; set; }
        public double WithdrawalIntensity { get; set; }
        public DateTime LastUpdatedAtUtc { get; set; }
    }

    private readonly List<DrugExposureRecord> _drugExposureRecords = new();

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

    internal static bool DependenceRecordRequiresHeartbeat(double exposure, double peakExposure,
        double withdrawalIntensity)
    {
        return exposure > 0.0 ||
               peakExposure > 0.0 ||
               withdrawalIntensity > 0.0;
    }

    internal static double DependenceActiveDoseGainForHeartbeat(double activeGrams, double exposureGainPerGram,
        TimeSpan elapsedSinceLastPersistedUpdate)
    {
        // Active drug doses are not metabolised for offline time, so dependence gain follows the same one-heartbeat model.
        _ = elapsedSinceLastPersistedUpdate;
        return activeGrams <= 0.0 ? 0.0 : activeGrams * exposureGainPerGram;
    }

    private bool HasDependenceRecordsNeedingHeartbeat()
    {
        return Gameworld.GetStaticBool(DrugDependenceEnabledStaticConfiguration) &&
               _drugExposureRecords.Any(x =>
                   DependenceRecordRequiresHeartbeat(x.Exposure, x.PeakExposure, x.WithdrawalIntensity));
    }

    public void CheckDrugTick()
    {
        if (!IsActiveCharacterBody)
        {
            EndDrugTick();
            return;
        }

        var needsDependenceHeartbeat = HasDependenceRecordsNeedingHeartbeat();
        if (_activeDrugDosages.Any() || _latentDrugDosages.Any() || NeedsModel.AlcoholLitres > 0.0 ||
            needsDependenceHeartbeat ||
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

        needsDependenceHeartbeat = HasDependenceRecordsNeedingHeartbeat();
        if (!_activeDrugDosages.Any() && !_latentDrugDosages.Any() && NeedsModel.AlcoholLitres <= 0.0 &&
            !needsDependenceHeartbeat &&
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
        UpdateDrugDependenceExposure();
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

    private DrugDependenceAdditionalInfo DependenceInfoFor(IDrug drug)
    {
        return drug.DrugTypes.Contains(DrugType.Dependence)
            ? drug.AdditionalInfoFor<DrugDependenceAdditionalInfo>(DrugType.Dependence)
            : null;
    }

    private DrugExposureRecord ExposureRecordFor(IDrug drug)
    {
        DrugExposureRecord record = _drugExposureRecords.FirstOrDefault(x => x.Drug.Id == drug.Id);
        if (record is not null)
        {
            return record;
        }

        record = new DrugExposureRecord
        {
            Drug = drug,
            LastUpdatedAtUtc = DateTime.UtcNow
        };
        _drugExposureRecords.Add(record);
        DrugsChanged = true;
        return record;
    }

    private void UpdateDrugDependenceExposure()
    {
        if (!Gameworld.GetStaticBool(DrugDependenceEnabledStaticConfiguration))
        {
            EffectHandler.RemoveAllEffects<DrugWithdrawalEffect>(fireRemovalAction: true);
            Actor.RemoveAllEffects(x => x.IsEffectType<DrugWithdrawalSleepPreventionEffect>(), true);
            return;
        }

        var now = DateTime.UtcNow;
        var activeDependentDrugs = ActiveDrugDosages
                                   .Select(x => x.Drug)
                                   .Where(x => DependenceInfoFor(x) is not null)
                                   .Distinct()
                                   .ToList();
        foreach (var drug in activeDependentDrugs)
        {
            ExposureRecordFor(drug);
        }

        foreach (var record in _drugExposureRecords.ToList())
        {
            var info = DependenceInfoFor(record.Drug);
            if (info is null)
            {
                _drugExposureRecords.Remove(record);
                DrugsChanged = true;
                continue;
            }

            var elapsed = record.LastUpdatedAtUtc == default
                ? TimeSpan.FromSeconds(10.0)
                : now - record.LastUpdatedAtUtc;
            if (elapsed < TimeSpan.Zero)
            {
                elapsed = TimeSpan.FromSeconds(10.0);
            }

            var elapsedDays = Math.Max(elapsed.TotalDays, 10.0 / 86400.0);
            record.LastUpdatedAtUtc = now;

            record.Exposure = Math.Max(0.0, record.Exposure - info.ExposureDecayPerDay * elapsedDays);
            record.PeakExposure = Math.Max(record.Exposure,
                Math.Max(0.0, record.PeakExposure - info.ExposureDecayPerDay * 0.25 * elapsedDays));

            var activeGrams = ActiveDrugDosages
                              .Where(x => x.Drug.Id == record.Drug.Id)
                              .Sum(x => x.Grams);
            if (activeGrams > 0.0)
            {
                record.Exposure += DependenceActiveDoseGainForHeartbeat(activeGrams, info.ExposureGainPerGram,
                    elapsed);
                record.PeakExposure = Math.Max(record.PeakExposure, record.Exposure);
            }

            if (record.Exposure >= info.WithdrawalThreshold)
            {
                record.WithdrawalIntensity = 0.0;
            }
            else if (record.PeakExposure >= info.WithdrawalThreshold && activeGrams <= 0.0)
            {
                var onsetIntensity = Math.Min(1.0,
                    (info.WithdrawalThreshold - record.Exposure) / Math.Max(info.WithdrawalThreshold, 0.0001));
                record.WithdrawalIntensity = Math.Max(record.WithdrawalIntensity, onsetIntensity);
                record.WithdrawalIntensity = Math.Max(0.0,
                    record.WithdrawalIntensity - info.WithdrawalDecayPerDay * elapsedDays);
            }
            else
            {
                record.WithdrawalIntensity = Math.Max(0.0,
                    record.WithdrawalIntensity - info.WithdrawalDecayPerDay * elapsedDays);
            }

            if (record.Exposure <= 0.0 &&
                record.PeakExposure <= 0.0 &&
                record.WithdrawalIntensity <= 0.0 &&
                activeGrams <= 0.0)
            {
                _drugExposureRecords.Remove(record);
            }

            DrugsChanged = true;
        }

        ApplyWithdrawalEffects();
    }

    private void ApplyWithdrawalEffects()
    {
        var activeWithdrawalRecords = _drugExposureRecords
                                      .Where(x => x.WithdrawalIntensity > 0.0)
                                      .Select(x => (Record: x, Info: DependenceInfoFor(x.Drug)))
                                      .Where(x => x.Info is not null)
                                      .ToList();
        if (!activeWithdrawalRecords.Any())
        {
            EffectHandler.RemoveAllEffects<DrugWithdrawalEffect>(fireRemovalAction: true);
            Actor.RemoveAllEffects(x => x.IsEffectType<DrugWithdrawalSleepPreventionEffect>(), true);
            return;
        }

        var withdrawal = EffectsOfType<DrugWithdrawalEffect>().FirstOrDefault();
        if (withdrawal is null)
        {
            withdrawal = new DrugWithdrawalEffect(this);
            EffectHandler.AddEffect(withdrawal);
        }

        withdrawal.DrugIntensities.Clear();
        withdrawal.WithdrawalIntensity = activeWithdrawalRecords.Sum(x => x.Record.WithdrawalIntensity);
        withdrawal.CheckBonus = activeWithdrawalRecords.Sum(x =>
            x.Info.WithdrawalCheckPenalty * x.Record.WithdrawalIntensity);
        withdrawal.HungerMultiplier = activeWithdrawalRecords.Aggregate(1.0,
            (current, item) => WeightedMultiplier(current, item.Info.WithdrawalHungerMultiplier,
                item.Record.WithdrawalIntensity));
        withdrawal.ThirstMultiplier = activeWithdrawalRecords.Aggregate(1.0,
            (current, item) => WeightedMultiplier(current, item.Info.WithdrawalThirstMultiplier,
                item.Record.WithdrawalIntensity));
        withdrawal.StaminaRegenMultiplier = activeWithdrawalRecords.Aggregate(1.0,
            (current, item) => WeightedMultiplier(current, item.Info.WithdrawalStaminaRegenMultiplier,
                item.Record.WithdrawalIntensity));
        withdrawal.StaminaCostMultiplier = activeWithdrawalRecords.Aggregate(1.0,
            (current, item) => WeightedMultiplier(current, item.Info.WithdrawalStaminaCostMultiplier,
                item.Record.WithdrawalIntensity));

        var nausea = activeWithdrawalRecords.Sum(x => x.Info.WithdrawalNauseaIntensity * x.Record.WithdrawalIntensity);
        if (nausea > 0.0)
        {
            withdrawal.DrugIntensities[DrugType.Nausea] = nausea;
        }

        var rage = activeWithdrawalRecords.Sum(x => x.Info.WithdrawalRageIntensity * x.Record.WithdrawalIntensity);
        if (rage > 0.0)
        {
            withdrawal.DrugIntensities[DrugType.Rage] = rage;
        }

        var preventSleep = activeWithdrawalRecords.Any(x =>
            x.Info.SleepPreventionThreshold > 0.0 &&
            x.Record.WithdrawalIntensity >= x.Info.SleepPreventionThreshold);
        var sleepBlock = Actor.EffectsOfType<DrugWithdrawalSleepPreventionEffect>().FirstOrDefault();
        if (preventSleep && sleepBlock is null)
        {
            Actor.AddEffect(new DrugWithdrawalSleepPreventionEffect(Actor));
        }
        else if (!preventSleep && sleepBlock is not null)
        {
            Actor.RemoveEffect(sleepBlock, true);
        }
    }

    private double ToleranceMultiplierForDrugType(IDrug drug, DrugType drugType)
    {
        if (!Gameworld.GetStaticBool(DrugDependenceEnabledStaticConfiguration))
        {
            return 1.0;
        }

        var info = DependenceInfoFor(drug);
        if (info is null ||
            info.ToleranceThreshold <= 0.0 ||
            info.MinimumToleranceMultiplier >= 1.0 ||
            info.AffectedDrugTypes.Any() && !info.AffectedDrugTypes.Contains(drugType))
        {
            return 1.0;
        }

        var record = _drugExposureRecords.FirstOrDefault(x => x.Drug.Id == drug.Id);
        if (record is null || record.Exposure <= info.ToleranceThreshold)
        {
            return 1.0;
        }

        var thresholdSteps = Math.Min(1.0,
            (record.Exposure - info.ToleranceThreshold) / Math.Max(info.ToleranceThreshold, 0.0001));
        return 1.0 - (1.0 - info.MinimumToleranceMultiplier) * thresholdSteps;
    }

    private double IntensityPerGramMass(double value)
    {
        return value / (Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001);
    }

    private static double WeightedMultiplier(double current, double configuredMultiplier, double intensity)
    {
        return current * Math.Max(0.0, 1.0 + (configuredMultiplier - 1.0) * intensity);
    }

    private static double ClampMultiplier(double value, double minimum, double maximum)
    {
        return Math.Max(minimum, Math.Min(maximum, value));
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

        List<IDrugEffectResistanceMerit> resistanceMerits = Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor)).ToList();

        // Sum impacts from effects
        foreach (ICauseDrugEffect effect in CombinedEffectsOfType<ICauseDrugEffect>())
        {
            foreach (DrugType drugType in effect.AffectedDrugTypes)
            {
                effectIntensities[drugType] += effect.AddedIntensity(Actor, drugType) *
                                               resistanceMerits.Aggregate(1.0,
                                                   (sum, x) => sum * x.ModifierForDrugType(drugType));
            }
        }

        // Sum impacts from drugs
        DoubleCounter<IMagicCapability> magicCapabilities = new();
        DoubleCounter<BodypartTypeEnum> damagedOrgans = new();
        DoubleCounter<BodypartTypeEnum> organFunctionMods = new();
        PlanarStateAdditionalInfo planarStateInfo = null;
        double planarStateIntensity = 0.0;
        double coagulationExternalMultiplier = 1.0;
        double coagulationReopenMultiplier = 1.0;
        double coagulationInternalMultiplier = 1.0;
        double respirationDriveMultiplier = 1.0;
        double respirationHypoxiaMultiplier = 1.0;
        double respirationAirwayMultiplier = 1.0;
        double needHungerMultiplier = 1.0;
        double needThirstMultiplier = 1.0;
        double needDrunkennessMultiplier = 1.0;
        bool needAppliesPassive = false;
        bool needAppliesActive = false;
        List<(CoagulationAdditionalInfo Info, double Intensity)> coagulationContributions = new();
        List<(RespirationAdditionalInfo Info, double Intensity)> respirationContributions = new();
        List<(NeedRateAdditionalInfo Info, double Intensity)> needRateContributions = new();
        List<(ArousalAdditionalInfo Info, double Intensity)> arousalContributions = new();
        DrugArousalMode arousalMode = DrugArousalMode.None;
        double arousalIntensity = 0.0;
        double arousalCheckBonus = 0.0;
        double arousalPainThresholdMultiplier = 1.0;
        double arousalStunThresholdMultiplier = 1.0;
        double arousalAnesthesiaThresholdMultiplier = 1.0;
        double arousalStaminaRegenMultiplier = 1.0;
        double arousalStaminaCostMultiplier = 1.0;
        double arousalSleepThreshold = double.MaxValue;
        double arousalKnockoutThreshold = double.MaxValue;
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

                if (drugEffect == DrugType.Dependence)
                {
                    continue;
                }

                var adjustedEffectIntensity =
                    drug.IntensityForType(drugEffect) *
                    adjustedgrams *
                    ToleranceMultiplierForDrugType(drug, drugEffect) *
                    resistanceMerits.Aggregate(1.0, (sum, x) => sum * x.ModifierForDrugType(drugEffect));
                var adjustedEffectIntensityPerGramMass = IntensityPerGramMass(adjustedEffectIntensity);

                if (drugEffect == DrugType.MagicAbility)
                {
                    List<IMagicCapability> capabilities = drug.AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility)
                                           .MagicCapabilityIds
                                                       .Select(x => Gameworld.MagicCapabilities.Get(x))
                                                       .ToList();
                    foreach (IMagicCapability capability in capabilities)
                    {
                        magicCapabilities[capability] += adjustedEffectIntensity;
                    }
                }

                if (drugEffect == DrugType.HealingRate)
                {
                    HealingRateAdditionalInfo split = drug.AdditionalInfoFor<HealingRateAdditionalInfo>(DrugType.HealingRate);
                    healingRateIntensity += split.HealingRateIntensity * adjustedEffectIntensity;
                    healingDifficultyIntensity += split.HealingDifficultyIntensity * adjustedEffectIntensity;
                }

                if (drugEffect == DrugType.BodypartDamage)
                {
                    foreach (BodypartTypeEnum organ in drug.AdditionalInfoFor<BodypartDamageAdditionalInfo>(DrugType.BodypartDamage).BodypartTypes)
                    {
                        damagedOrgans[organ] += adjustedEffectIntensity;
                    }
                }

                if (drugEffect == DrugType.OrganFunction)
                {
                    foreach (BodypartTypeEnum organ in drug.AdditionalInfoFor<OrganFunctionAdditionalInfo>(DrugType.OrganFunction).OrganTypes)
                    {
                        organFunctionMods[organ] += adjustedEffectIntensity;
                    }
                }

                if (drugEffect == DrugType.PlanarState)
                {
                    if (adjustedEffectIntensity > planarStateIntensity)
                    {
                        planarStateIntensity = adjustedEffectIntensity;
                        planarStateInfo = drug.AdditionalInfoFor<PlanarStateAdditionalInfo>(DrugType.PlanarState);
                    }
                }

                if (drugEffect == DrugType.Coagulation)
                {
                    var info = drug.AdditionalInfoFor<CoagulationAdditionalInfo>(DrugType.Coagulation);
                    coagulationContributions.Add((info, adjustedEffectIntensityPerGramMass));
                }

                if (drugEffect == DrugType.Respiration)
                {
                    var info = drug.AdditionalInfoFor<RespirationAdditionalInfo>(DrugType.Respiration);
                    respirationContributions.Add((info, adjustedEffectIntensityPerGramMass));
                }

                if (drugEffect == DrugType.NeedRate)
                {
                    var info = drug.AdditionalInfoFor<NeedRateAdditionalInfo>(DrugType.NeedRate);
                    needRateContributions.Add((info, adjustedEffectIntensityPerGramMass));
                }

                if (drugEffect == DrugType.Arousal)
                {
                    var info = drug.AdditionalInfoFor<ArousalAdditionalInfo>(DrugType.Arousal);
                    arousalContributions.Add((info, adjustedEffectIntensityPerGramMass));
                }

                effectIntensities[drugEffect] += adjustedEffectIntensity;
            }
        }

        double NetIntensityScale(DrugType type)
        {
            var total = effectIntensities.ValueOrDefault(type);
            return total <= 0.0
                ? 0.0
                : Math.Max(0.0, (total - neutralisingEffects.ValueOrDefault(type)) / total);
        }

        foreach (var (info, rawIntensity) in coagulationContributions)
        {
            var intensity = rawIntensity * NetIntensityScale(DrugType.Coagulation);
            coagulationExternalMultiplier = WeightedMultiplier(coagulationExternalMultiplier,
                info.ExternalBleedingMultiplier, intensity);
            coagulationReopenMultiplier = WeightedMultiplier(coagulationReopenMultiplier,
                info.WoundReopenMultiplier, intensity);
            coagulationInternalMultiplier = WeightedMultiplier(coagulationInternalMultiplier,
                info.InternalBleedingMultiplier, intensity);
        }

        foreach (var (info, rawIntensity) in respirationContributions)
        {
            var intensity = rawIntensity * NetIntensityScale(DrugType.Respiration);
            respirationDriveMultiplier = WeightedMultiplier(respirationDriveMultiplier,
                info.BreathingDriveMultiplier, intensity);
            respirationHypoxiaMultiplier = WeightedMultiplier(respirationHypoxiaMultiplier,
                info.HypoxiaDamageMultiplier, intensity);
            respirationAirwayMultiplier = WeightedMultiplier(respirationAirwayMultiplier,
                info.AirwayToleranceMultiplier, intensity);
        }

        foreach (var (info, rawIntensity) in needRateContributions)
        {
            var intensity = rawIntensity * NetIntensityScale(DrugType.NeedRate);
            if (intensity <= 0.0)
            {
                continue;
            }

            needHungerMultiplier = WeightedMultiplier(needHungerMultiplier, info.HungerMultiplier, intensity);
            needThirstMultiplier = WeightedMultiplier(needThirstMultiplier, info.ThirstMultiplier, intensity);
            needDrunkennessMultiplier = WeightedMultiplier(needDrunkennessMultiplier,
                info.DrunkennessMultiplier, intensity);
            needAppliesPassive |= info.AppliesToPassive;
            needAppliesActive |= info.AppliesToActive;
        }

        foreach (var (info, rawIntensity) in arousalContributions)
        {
            var intensity = rawIntensity * NetIntensityScale(DrugType.Arousal);
            if (intensity <= 0.0)
            {
                continue;
            }

            arousalMode |= info.Mode;
            arousalIntensity += intensity;
            arousalCheckBonus += info.CheckBonusPerIntensity * intensity;
            arousalPainThresholdMultiplier = WeightedMultiplier(arousalPainThresholdMultiplier,
                info.PainPassOutThresholdMultiplier, intensity);
            arousalStunThresholdMultiplier = WeightedMultiplier(arousalStunThresholdMultiplier,
                info.StunUnconsciousThresholdMultiplier, intensity);
            arousalAnesthesiaThresholdMultiplier = WeightedMultiplier(arousalAnesthesiaThresholdMultiplier,
                info.AnesthesiaUnconsciousThresholdMultiplier, intensity);
            arousalStaminaRegenMultiplier = WeightedMultiplier(arousalStaminaRegenMultiplier,
                info.StaminaRegenMultiplier, intensity);
            arousalStaminaCostMultiplier = WeightedMultiplier(arousalStaminaCostMultiplier,
                info.StaminaCostMultiplier, intensity);
            if (info.Mode.HasFlag(DrugArousalMode.SleepInducing))
            {
                arousalSleepThreshold = Math.Min(arousalSleepThreshold, info.SleepIntensityThreshold);
            }

            if (info.Mode.HasFlag(DrugArousalMode.Knockout))
            {
                arousalKnockoutThreshold = Math.Min(arousalKnockoutThreshold, info.KnockoutIntensityThreshold);
            }
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
                case DrugType.Coagulation:
                    {
                        if (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key) <= 0.0)
                        {
                            break;
                        }

                        var coagulationEffect = EffectsOfType<DrugCoagulationEffect>().FirstOrDefault();
                        if (coagulationEffect is null)
                        {
                            coagulationEffect = new DrugCoagulationEffect(this);
                            EffectHandler.AddEffect(coagulationEffect);
                        }

                        coagulationEffect.ExternalBleedingMultiplier = ClampMultiplier(coagulationExternalMultiplier,
                            Gameworld.GetStaticDouble(DrugCoagulationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugCoagulationMaximumMultiplierStaticConfiguration));
                        coagulationEffect.WoundReopenMultiplier = ClampMultiplier(coagulationReopenMultiplier,
                            Gameworld.GetStaticDouble(DrugCoagulationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugCoagulationMaximumMultiplierStaticConfiguration));
                        coagulationEffect.InternalBleedingMultiplier = ClampMultiplier(coagulationInternalMultiplier,
                            Gameworld.GetStaticDouble(DrugCoagulationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugCoagulationMaximumMultiplierStaticConfiguration));
                        break;
                    }
                case DrugType.Respiration:
                    {
                        if (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key) <= 0.0)
                        {
                            break;
                        }

                        var respirationEffect = EffectsOfType<DrugRespirationEffect>().FirstOrDefault();
                        if (respirationEffect is null)
                        {
                            respirationEffect = new DrugRespirationEffect(this);
                            EffectHandler.AddEffect(respirationEffect);
                        }

                        respirationEffect.BreathingDriveMultiplier = ClampMultiplier(respirationDriveMultiplier,
                            Gameworld.GetStaticDouble(DrugRespirationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugRespirationMaximumMultiplierStaticConfiguration));
                        respirationEffect.HypoxiaDamageMultiplier = ClampMultiplier(respirationHypoxiaMultiplier,
                            Gameworld.GetStaticDouble(DrugRespirationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugRespirationMaximumMultiplierStaticConfiguration));
                        respirationEffect.AirwayToleranceMultiplier = ClampMultiplier(respirationAirwayMultiplier,
                            Gameworld.GetStaticDouble(DrugRespirationMinimumMultiplierStaticConfiguration),
                            Gameworld.GetStaticDouble(DrugRespirationMaximumMultiplierStaticConfiguration));
                        break;
                    }
                case DrugType.NeedRate:
                    {
                        if (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key) <= 0.0)
                        {
                            break;
                        }

                        var needRateEffect = EffectsOfType<DrugNeedRateEffect>().FirstOrDefault();
                        if (needRateEffect is null)
                        {
                            needRateEffect = new DrugNeedRateEffect(this);
                            EffectHandler.AddEffect(needRateEffect);
                        }

                        needRateEffect.HungerMultiplier = needHungerMultiplier;
                        needRateEffect.ThirstMultiplier = needThirstMultiplier;
                        needRateEffect.DrunkennessMultiplier = needDrunkennessMultiplier;
                        needRateEffect.AppliesToPassive = needAppliesPassive;
                        needRateEffect.AppliesToActive = needAppliesActive;
                        break;
                    }
                case DrugType.Arousal:
                    {
                        if (effect.Value - neutralisingEffects.ValueOrDefault(effect.Key) <= 0.0)
                        {
                            break;
                        }

                        var hasThresholdEffect =
                            Math.Abs(arousalCheckBonus) > 0.0 ||
                            Math.Abs(arousalPainThresholdMultiplier - 1.0) > 0.0001 ||
                            Math.Abs(arousalStunThresholdMultiplier - 1.0) > 0.0001 ||
                            Math.Abs(arousalAnesthesiaThresholdMultiplier - 1.0) > 0.0001 ||
                            Math.Abs(arousalStaminaRegenMultiplier - 1.0) > 0.0001 ||
                            Math.Abs(arousalStaminaCostMultiplier - 1.0) > 0.0001;
                        var arousalEffect = EffectsOfType<DrugArousalEffect>().FirstOrDefault();
                        if (hasThresholdEffect)
                        {
                            if (arousalEffect is null)
                            {
                                arousalEffect = new DrugArousalEffect(this);
                                EffectHandler.AddEffect(arousalEffect);
                            }

                            arousalEffect.Intensity = arousalIntensity;
                            arousalEffect.CheckBonusPerIntensity = arousalIntensity > 0.0
                                ? arousalCheckBonus / arousalIntensity
                                : 0.0;
                            arousalEffect.PainPassOutThresholdMultiplier = arousalPainThresholdMultiplier;
                            arousalEffect.StunUnconsciousThresholdMultiplier = arousalStunThresholdMultiplier;
                            arousalEffect.AnesthesiaUnconsciousThresholdMultiplier =
                                arousalAnesthesiaThresholdMultiplier;
                            arousalEffect.StaminaRegenMultiplier = arousalStaminaRegenMultiplier;
                            arousalEffect.StaminaCostMultiplier = arousalStaminaCostMultiplier;
                        }
                        else if (arousalEffect is not null)
                        {
                            EffectHandler.RemoveEffect(arousalEffect, true);
                        }

                        var sleepEffect = Actor.EffectsOfType<DrugSleepEffect>().FirstOrDefault();
                        if (arousalMode.HasFlag(DrugArousalMode.SleepInducing) &&
                            arousalIntensity >= arousalSleepThreshold &&
                            sleepEffect is null)
                        {
                            Actor.AddEffect(new DrugSleepEffect(Actor));
                        }
                        else if ((!arousalMode.HasFlag(DrugArousalMode.SleepInducing) ||
                                  arousalIntensity < arousalSleepThreshold) &&
                                 sleepEffect is not null)
                        {
                            Actor.RemoveEffect(sleepEffect, true);
                        }

                        var sleepPreventionEffect = Actor.EffectsOfType<DrugSleepPreventionEffect>().FirstOrDefault();
                        if (arousalMode.HasFlag(DrugArousalMode.SleepPreventing) &&
                            sleepPreventionEffect is null)
                        {
                            Actor.AddEffect(new DrugSleepPreventionEffect(Actor));
                        }
                        else if (!arousalMode.HasFlag(DrugArousalMode.SleepPreventing) &&
                                 sleepPreventionEffect is not null)
                        {
                            Actor.RemoveEffect(sleepPreventionEffect, true);
                        }

                        var passOutEffect = EffectsOfType<DrugPassOutResistanceEffect>().FirstOrDefault();
                        if (arousalMode.HasFlag(DrugArousalMode.PassOutResistance) &&
                            passOutEffect is null)
                        {
                            EffectHandler.AddEffect(new DrugPassOutResistanceEffect(this));
                        }
                        else if (!arousalMode.HasFlag(DrugArousalMode.PassOutResistance) &&
                                 passOutEffect is not null)
                        {
                            EffectHandler.RemoveEffect(passOutEffect, true);
                        }

                        var knockoutEffect = EffectsOfType<DrugKnockoutEffect>().FirstOrDefault();
                        if (arousalMode.HasFlag(DrugArousalMode.Knockout) &&
                            arousalIntensity >= arousalKnockoutThreshold &&
                            knockoutEffect is null)
                        {
                            EffectHandler.AddEffect(new DrugKnockoutEffect(this));
                        }
                        else if ((!arousalMode.HasFlag(DrugArousalMode.Knockout) ||
                                  arousalIntensity < arousalKnockoutThreshold) &&
                                 knockoutEffect is not null)
                        {
                            EffectHandler.RemoveEffect(knockoutEffect, true);
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

        if (effectIntensities.ValueOrDefault(DrugType.Coagulation) -
            neutralisingEffects.ValueOrDefault(DrugType.Coagulation) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<DrugCoagulationEffect>(fireRemovalAction: true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.Respiration) -
            neutralisingEffects.ValueOrDefault(DrugType.Respiration) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<DrugRespirationEffect>(fireRemovalAction: true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.NeedRate) -
            neutralisingEffects.ValueOrDefault(DrugType.NeedRate) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<DrugNeedRateEffect>(fireRemovalAction: true);
        }

        if (effectIntensities.ValueOrDefault(DrugType.Arousal) -
            neutralisingEffects.ValueOrDefault(DrugType.Arousal) <= 0.0)
        {
            EffectHandler.RemoveAllEffects<DrugArousalEffect>(fireRemovalAction: true);
            EffectHandler.RemoveAllEffects<DrugPassOutResistanceEffect>(fireRemovalAction: true);
            EffectHandler.RemoveAllEffects<DrugKnockoutEffect>(fireRemovalAction: true);
            Actor.RemoveAllEffects(x => x.IsEffectType<DrugSleepEffect>(), true);
            Actor.RemoveAllEffects(x => x.IsEffectType<DrugSleepPreventionEffect>(), true);
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

        List<BodyDrugExposure> exposuresToDelete =
            body.BodiesDrugExposures
                .Where(x => !_drugExposureRecords.Any(y => y.Drug.Id == x.DrugId))
                .ToList();
        foreach (var exposure in exposuresToDelete)
        {
            body.BodiesDrugExposures.Remove(exposure);
        }

        foreach (var item in _drugExposureRecords)
        {
            BodyDrugExposure dbitem = body.BodiesDrugExposures.FirstOrDefault(x => x.DrugId == item.Drug.Id);
            if (dbitem is null)
            {
                dbitem = new BodyDrugExposure
                {
                    DrugId = item.Drug.Id
                };
                body.BodiesDrugExposures.Add(dbitem);
            }

            dbitem.Exposure = item.Exposure;
            dbitem.PeakExposure = item.PeakExposure;
            dbitem.WithdrawalIntensity = item.WithdrawalIntensity;
            dbitem.LastUpdatedAtUtc = item.LastUpdatedAtUtc;
        }

        DrugsChanged = false;
    }

    #endregion
}
