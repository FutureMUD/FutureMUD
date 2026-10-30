using MudSharp.Body.PartProtos;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Effects.Concrete;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.Planes;
using MudSharp.RPG.Merits.Interfaces;

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

		ThermalImbalance thermal = ThermalImbalance.DominantAmbientExposureEffect(
			Actor.CombinedEffectsOfType<ThermalImbalance>());
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
        Dose(drug, vector, grams, originator,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseOriginId,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseFamily,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseFormula);
    }

    private void Dose(IDrug drug, DrugVector vector, double grams, object originator, Guid? spellOriginId,
        string spellOriginFamily, string spellOriginFormula)
    {
        if (!DrugDosePolicy.CanAccept(this, drug)) return;
        grams = DrugDosePolicy.Normalise(drug, grams);
        if (grams <= 0.0) return;
        if (drug.DrugTypes.Contains(DrugType.Infection))
        {
            MudSharp.Health.Infections.ConfiguredInfection.TryExpose(this, drug, spellOriginFamily);
            if (drug.DrugTypes.All(x => x == DrugType.Infection)) return;
        }
        DrugsChanged = true;
        DrugDosage latent = _latentDrugDosages.FirstOrDefault(x =>
            x.Drug == drug &&
            x.OriginalVector == vector &&
            SameDoseOrigin(x, originator, spellOriginId));
        if (latent == null)
        {
            latent = new DrugDosage { Drug = drug, OriginalVector = vector, Originator = originator,
                SpellOriginId = spellOriginId, SpellOriginFamily = spellOriginFamily,
                SpellOriginFormula = spellOriginFormula };
            _latentDrugDosages.Add(latent);
        }

        latent.Grams += grams;
        ApplyAdministeredDrugConsequences(drug, grams);
        CheckDrugTick();
    }

    public void DoseImmediate(IDrug drug, DrugVector vector, double grams, object originator)
    {
        DoseImmediate(drug, vector, grams, originator,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseOriginId,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseFamily,
            (originator as IPersistentDrugDoseOrigin)?.DrugDoseFormula);
    }

    private static bool SameDoseOrigin(DrugDosage dose, object originator, Guid? spellOriginId) =>
        spellOriginId.HasValue ? dose.SpellOriginId == spellOriginId :
            dose.SpellOriginId is null && Equals(dose.Originator, originator);

    private void DoseImmediate(IDrug drug, DrugVector vector, double grams, object originator, Guid? spellOriginId,
        string spellOriginFamily, string spellOriginFormula)
    {
        if (!DrugDosePolicy.CanAccept(this, drug)) return;
        grams = DrugDosePolicy.Normalise(drug, grams);
        if (grams <= 0.0)
        {
            return;
        }

        if (drug.DrugTypes.Contains(DrugType.Infection))
        {
            MudSharp.Health.Infections.ConfiguredInfection.TryExpose(this, drug, spellOriginFamily);
            if (drug.DrugTypes.All(x => x == DrugType.Infection)) return;
        }
        DrugsChanged = true;
        DrugDosage active = _activeDrugDosages.FirstOrDefault(x =>
            x.Drug == drug &&
            SameDoseOrigin(x, originator, spellOriginId));
        if (active == null)
        {
            active = new DrugDosage { Drug = drug, OriginalVector = vector, Originator = originator,
                SpellOriginId = spellOriginId, SpellOriginFamily = spellOriginFamily,
                SpellOriginFormula = spellOriginFormula };
            _activeDrugDosages.Add(active);
        }

        active.Grams += grams;
        ApplyAdministeredDrugConsequences(drug, grams);
        ApplyDrugEffects();
        CheckDrugTick();
        CheckHealthStatus();
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

    private void ApplyAdministeredDrugConsequences(IDrug drug, double grams)
    {
        if (!double.IsFinite(grams) || grams <= 0.0 || !drug.DrugTypes.Contains(DrugType.Symptoms)) return;
        var info = drug.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms);
        if (info is null || !info.OnsetPerDose) return;
        var symptoms = EffectsOfType<DrugSymptomEffect>().FirstOrDefault(x => x.DrugId == drug.Id && x.SourceInfectionDrugId == 0);
        if (symptoms is null)
        {
            symptoms = new DrugSymptomEffect(this, drug.Id, info.InitialStage);
            AddEffect(symptoms);
        }
        var adjustedGrams = Actor.Merits.OfType<ISpecificDrugResistanceMerit>().Where(x => x.Applies(Actor))
            .Aggregate(grams, (value, merit) => value * merit.MultiplierForDrug(drug));
        var intensity = IntensityPerGramMass(DrugEffectIntensity(drug, DrugType.Symptoms, adjustedGrams,
            Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor))));
        symptoms.ApplyAdministeredDose(intensity);
    }

    internal static bool DependenceRecordRequiresHeartbeat(double exposure, double peakExposure,
        double withdrawalIntensity)
    {
        return exposure > 0.0 ||
               peakExposure > 0.0 ||
               withdrawalIntensity > 0.0;
    }

    internal static double DependenceActiveDoseGainForHeartbeat(double activeGrams, double exposureGainPerGram,
        TimeSpan elapsedSinceLastPersistedUpdate, double relapseMultiplier = 1.0)
    {
        // Active drug doses are not metabolised for offline time, so dependence gain follows the same one-heartbeat model.
        _ = elapsedSinceLastPersistedUpdate;
        return activeGrams <= 0.0 ? 0.0 : activeGrams * exposureGainPerGram * relapseMultiplier;
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
            PartInfections.OfType<IDrugSymptomInfection>().Any() || needsDependenceHeartbeat ||
            CombinedEffectsOfType<ICauseDrugEffect>().Any())
        {
            if (_drugTickOn)
            {
                return;
            }

            Gameworld.HeartbeatManager.TenSecondHeartbeat -= DrugTenSecondHeartbeat;
            Gameworld.HeartbeatManager.TenSecondHeartbeat += DrugTenSecondHeartbeat;
            _drugTickOn = true;
            foreach (var symptoms in EffectsOfType<DrugSymptomEffect>()) symptoms.ResetClock();
            foreach (var infection in PartInfections.OfType<MudSharp.Health.Infections.ConfiguredInfection>()) infection.ResetClock();
            DrugTenSecondHeartbeat();
            StartHealthTick();
            return;
        }

        needsDependenceHeartbeat = HasDependenceRecordsNeedingHeartbeat();
        if (!_activeDrugDosages.Any() && !_latentDrugDosages.Any() && NeedsModel.AlcoholLitres <= 0.0 &&
            !PartInfections.OfType<IDrugSymptomInfection>().Any() && !needsDependenceHeartbeat &&
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

        foreach (var infection in PartInfections.OfType<MudSharp.Health.Infections.ConfiguredInfection>().ToArray())
        {
            infection.Tick();
            if (!IsActiveCharacterBody || !ReferenceEquals(Actor.Body, this) || Actor.State.HasFlag(CharacterState.Dead)) return;
        }
        EvaluateInfections();
        ProcessLatentDrugs();
        UpdateDrugDependenceExposure();
        ApplyDrugEffects();
        if (Actor.State.HasFlag(CharacterState.Dead) || !IsActiveCharacterBody || !ReferenceEquals(Actor.Body, this))
        {
            EndDrugTick();
            return;
        }
        foreach (var symptoms in EffectsOfType<DrugSymptomEffect>().ToArray())
        {
            symptoms.Tick();
            if (Actor.State.HasFlag(CharacterState.Dead) || !IsActiveCharacterBody || !ReferenceEquals(Actor.Body, this))
            {
                EndDrugTick();
                return;
            }
        }
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
            double rate = MudSharp.Magic.SubstanceDose.AbsorptionFraction(drug.OriginalVector);

            DrugDosage activeDose = _activeDrugDosages.FirstOrDefault(x =>
                x.Drug == drug.Drug &&
                SameDoseOrigin(x, drug.Originator, drug.SpellOriginId));
            if (activeDose == null)
            {
                activeDose = AbsorbedDrugDose(drug);
                _activeDrugDosages.Add(activeDose);
            }

            double amount = Math.Min(drug.Grams, Math.Max(0.0001, rate * drug.Grams));
            activeDose.Grams += amount;
            drug.Grams -= amount;
            if (drug.Grams <= 0)
            {
                _latentDrugDosages.Remove(drug);
            }

            DrugsChanged = true;
        }
    }

    internal static DrugDosage AbsorbedDrugDose(DrugDosage latent) => new()
    {
        Drug = latent.Drug,
        OriginalVector = latent.SpellOriginId.HasValue ? latent.OriginalVector : DrugVector.Injected,
        Originator = latent.Originator,
        SpellOriginId = latent.SpellOriginId,
        SpellOriginFamily = latent.SpellOriginFamily,
        SpellOriginFormula = latent.SpellOriginFormula
    };

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
            LastUpdatedAtUtc = RuntimeClock.UtcNow
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
            RefreshDrugModifierEffects();
            return;
        }

        var now = RuntimeClock.UtcNow;
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
                var relapseMultiplier = record.Exposure + 0.000001 < record.PeakExposure
                    ? info.RelapseExposureMultiplier
                    : 1.0;
                record.Exposure += DependenceActiveDoseGainForHeartbeat(activeGrams, info.ExposureGainPerGram,
                    elapsed, relapseMultiplier);
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
        RefreshDrugModifierEffects();
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

    private static DrugModifierStep? BestModifierStep(IEnumerable<DrugModifierStep> steps, double intensity,
        Func<DrugModifierStep, bool> hasTarget) => steps.Where(x => x.Threshold <= intensity && hasTarget(x))
        .OrderByDescending(x => x.Threshold).FirstOrDefault();

    private void RefreshDrugModifierEffects()
    {
        var traitBonuses = new Dictionary<long, double>();
        var resourceBonuses = new Dictionary<long, double>();
        var dependenceRecords = Gameworld.GetStaticBool(DrugDependenceEnabledStaticConfiguration)
            ? _drugExposureRecords.ToList()
            : [];
        double hpBonus = 0.0;
        double hpMultiplier = 1.0;
        double offensiveBonus = 0.0;
        double defensiveBonus = 0.0;
        var modifierContributions = new List<(DrugModifierAdditionalInfo Info, double Intensity)>();
        var modifierNeutralisation = 0.0;
        var modifierTotalIntensity = 0.0;
        var specificMerits = Actor.Merits.OfType<ISpecificDrugResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
        var resistanceMerits = Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor)).ToList();

        void ApplySteps(IEnumerable<DrugModifierStep> steps, double intensity)
        {
            var list = steps.ToList();
            foreach (var traitId in list.Where(x => x.TraitId > 0 && x.TraitBonus != 0.0).Select(x => x.TraitId).Distinct())
            {
                var step = BestModifierStep(list, intensity, x => x.TraitId == traitId && x.TraitBonus != 0.0);
                if (step is not null) traitBonuses[traitId] = traitBonuses.GetValueOrDefault(traitId) + step.TraitBonus;
            }
            var hp = BestModifierStep(list, intensity, x => x.MaximumHitPointsBonus != 0.0);
            if (hp is not null) hpBonus += hp.MaximumHitPointsBonus;
            foreach (var resourceId in list.Where(x => x.MagicResourceId > 0 && x.MagicResourceCapacityBonus != 0.0).Select(x => x.MagicResourceId).Distinct())
            {
                var step = BestModifierStep(list, intensity, x => x.MagicResourceId == resourceId && x.MagicResourceCapacityBonus != 0.0);
                if (step is not null) resourceBonuses[resourceId] = resourceBonuses.GetValueOrDefault(resourceId) + step.MagicResourceCapacityBonus;
            }
            var offensive = BestModifierStep(list, intensity, x => x.OffensiveCheckBonus != 0.0);
            if (offensive is not null) offensiveBonus += offensive.OffensiveCheckBonus;
            var defensive = BestModifierStep(list, intensity, x => x.DefensiveCheckBonus != 0.0);
            if (defensive is not null) defensiveBonus += defensive.DefensiveCheckBonus;
        }

        foreach (var (drug, grams) in ActiveDrugIntensities().Where(x => x.Value > 0.0))
        {
            var adjustedGrams = specificMerits.Aggregate(1.0, (value, merit) => value * merit.MultiplierForDrug(drug)) * grams;
            if (drug.DrugTypes.Contains(DrugType.Modifier))
            {
                var intensity = DrugEffectIntensity(drug, DrugType.Modifier, adjustedGrams, resistanceMerits);
                modifierTotalIntensity += intensity;
                modifierContributions.Add((drug.AdditionalInfoFor<DrugModifierAdditionalInfo>(DrugType.Modifier), intensity));
            }
            if (drug.DrugTypes.Contains(DrugType.NeutraliseDrugEffect) &&
                drug.AdditionalInfoFor<NeutraliseDrugAdditionalInfo>(DrugType.NeutraliseDrugEffect).NeutralisedTypes.Contains(DrugType.Modifier))
            {
                modifierNeutralisation += drug.IntensityForType(DrugType.NeutraliseDrugEffect) * adjustedGrams;
            }
        }

        var modifierIntensityScale = NetDrugModifierIntensityScale(modifierTotalIntensity, modifierNeutralisation);
        foreach (var (info, intensity) in modifierContributions)
        {
            ApplySteps(info.Steps, IntensityPerGramMass(intensity * modifierIntensityScale));
        }

        foreach (var record in dependenceRecords)
        {
            var info = DependenceInfoFor(record.Drug);
            if (info is null) continue;
            ApplySteps(info.ModifierSteps, record.Exposure);
            hpMultiplier = WeightedMultiplier(hpMultiplier, info.WithdrawalMaximumHitPointsMultiplier, record.WithdrawalIntensity);
        }

        var bodyTraitBonuses = traitBonuses.Where(x => Gameworld.Traits.Get(x.Key)?.OwnerScope == TraitOwnerScope.Body).ToDictionary(x => x.Key, x => x.Value);
        var characterTraitBonuses = traitBonuses.Where(x => Gameworld.Traits.Get(x.Key)?.OwnerScope == TraitOwnerScope.Character).ToDictionary(x => x.Key, x => x.Value);
        var modifier = EffectsOfType<DrugTraitModifierEffect>().FirstOrDefault();
        if (bodyTraitBonuses.Any() || hpBonus != 0.0 || hpMultiplier != 1.0)
        {
            modifier ??= new DrugTraitModifierEffect(this);
            var previousBonuses = modifier.TraitBonuses.ToDictionary(x => x.Key, x => x.Value);
            var previousHpBonus = modifier.MaximumHitPointsBonus;
            var previousHpMultiplier = modifier.MaximumHitPointsMultiplier;
            modifier.TraitBonuses.Clear();
            foreach (var entry in bodyTraitBonuses) modifier.TraitBonuses[entry.Key] = entry.Value;
            modifier.MaximumHitPointsBonus = hpBonus;
            modifier.MaximumHitPointsMultiplier = hpMultiplier;
            if (!EffectsOfType<DrugTraitModifierEffect>().Any()) EffectHandler.AddEffect(modifier);
            else if (!SameBonuses(previousBonuses, bodyTraitBonuses) || previousHpBonus != hpBonus || previousHpMultiplier != hpMultiplier)
                EffectHandler.NotifyEffectUpdated(modifier);
        }
        else if (modifier is not null) EffectHandler.RemoveEffect(modifier, true);

        foreach (var stale in Actor.EffectsOfType<DrugTraitModifierEffect>().Where(x => x.SourceBodyId != Id).ToList()) Actor.RemoveEffect(stale, true);
        var actorModifier = Actor.EffectsOfType<DrugTraitModifierEffect>().FirstOrDefault(x => x.SourceBodyId == Id);
        if (characterTraitBonuses.Any())
        {
            actorModifier ??= new DrugTraitModifierEffect(Actor);
            actorModifier.SourceBodyId = Id;
            var previousBonuses = actorModifier.TraitBonuses.ToDictionary(x => x.Key, x => x.Value);
            actorModifier.TraitBonuses.Clear();
            foreach (var entry in characterTraitBonuses) actorModifier.TraitBonuses[entry.Key] = entry.Value;
            actorModifier.MaximumHitPointsBonus = 0.0;
            actorModifier.MaximumHitPointsMultiplier = 1.0;
            if (!Actor.EffectsOfType<DrugTraitModifierEffect>().Any(x => x.SourceBodyId == Id)) Actor.AddEffect(actorModifier);
            else if (!SameBonuses(previousBonuses, characterTraitBonuses))
            {
                Gameworld.MagicCasting?.NotifyCapacityChange(Actor);
                Gameworld.MagicCasting?.NotifyPracticeInputsChanged(Actor);
            }
        }
        else if (actorModifier is not null) Actor.RemoveEffect(actorModifier, true);

        foreach (var resourceId in resourceBonuses.Keys.Union(Gameworld.MagicResources
                     .Where(x => x.ResourceType.HasFlag(MagicResourceType.PlayerResource)).Select(x => x.Id)).Where(x => x > 0).Distinct())
        {
            var effect = EffectsOfType<DrugResourceCapacityModifierEffect>().FirstOrDefault(x => x.MagicResourceId == resourceId);
            var penaltyMultiplier = dependenceRecords.Select(x => (Record: x, Info: DependenceInfoFor(x.Drug)))
                .Where(x => x.Info is not null)
                .Aggregate(1.0, (current, x) => WeightedMultiplier(current, x.Info!.WithdrawalMagicResourceCapacityMultiplier, x.Record.WithdrawalIntensity));
            var bonus = resourceBonuses.GetValueOrDefault(resourceId);
            if (bonus != 0.0 || penaltyMultiplier != 1.0)
            {
                effect ??= new DrugResourceCapacityModifierEffect(this, resourceId);
                var wasPresent = EffectsOfType<DrugResourceCapacityModifierEffect>().Any(x => x.MagicResourceId == resourceId);
                var oldBonus = effect.MagicResourceCapacityBonus;
                var oldMultiplier = effect.MagicResourceCapacityMultiplier;
                effect.MagicResourceCapacityBonus = bonus;
                effect.MagicResourceCapacityMultiplier = penaltyMultiplier;
                if (!wasPresent) EffectHandler.AddEffect(effect);
                else if (oldBonus != bonus || oldMultiplier != penaltyMultiplier) EffectHandler.NotifyEffectUpdated(effect);
            }
            else if (effect is not null) EffectHandler.RemoveEffect(effect, true);
        }

        SetCombatModifier(true, offensiveBonus);
        SetCombatModifier(false, defensiveBonus);
    }

    private static bool SameBonuses(Dictionary<long, double> left, Dictionary<long, double> right) =>
        left.Count == right.Count && left.All(x => right.TryGetValue(x.Key, out var value) && value == x.Value);

    private void SetCombatModifier(bool offensive, double bonus)
    {
        var effect = EffectsOfType<DrugCombatCheckModifierEffect>().FirstOrDefault(x => x.Offensive == offensive);
        if (bonus == 0.0)
        {
            if (effect is not null) EffectHandler.RemoveEffect(effect, true);
            return;
        }
        effect ??= new DrugCombatCheckModifierEffect(this, offensive);
        effect.CheckBonus = bonus;
        if (!EffectsOfType<DrugCombatCheckModifierEffect>().Any(x => x.Offensive == offensive)) EffectHandler.AddEffect(effect);
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

    internal static bool IsDrugOverdoseThresholdReached(double activeGrams, double effectiveTraitValue,
        double gramsPerTraitPoint)
    {
        return double.IsFinite(activeGrams) && activeGrams > 0.0 && double.IsFinite(effectiveTraitValue) &&
               double.IsFinite(gramsPerTraitPoint) && gramsPerTraitPoint > 0.0 &&
               activeGrams >= Math.Max(0.0, effectiveTraitValue) * gramsPerTraitPoint;
    }

    internal static double NetDrugModifierIntensityScale(double totalIntensity, double neutralisationIntensity) =>
        totalIntensity <= 0.0 ? 0.0 : Math.Max(0.0, (totalIntensity - neutralisationIntensity) / totalIntensity);

    internal static double ActiveGramsForOverdoseGroup(IEnumerable<(long DrugId, string Group)> configurations,
        IEnumerable<(long DrugId, double Grams)> activeDoses, string group)
    {
        var matchingDrugIds = configurations.Where(x => string.Equals(x.Group, group, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.DrugId).ToHashSet();
        return activeDoses.Where(x => matchingDrugIds.Contains(x.DrugId)).Sum(x => x.Grams);
    }

    private void CheckDrugOverdose()
    {
        if (!IsActiveCharacterBody || !ReferenceEquals(Actor.Body, this) || Actor.State.HasFlag(CharacterState.Dead)) return;
        var overdoseDrugs = ActiveDrugDosages.Select(x => x.Drug).Distinct()
            .Where(x => x.DrugTypes.Contains(DrugType.Overdose)).ToList();
        var overdoseConfigurations = overdoseDrugs.Select(drug => (Drug: drug,
            Config: drug.AdditionalInfoFor<OverdoseAdditionalInfo>(DrugType.Overdose))).ToList();
        foreach (var (drug, overdose) in overdoseConfigurations)
        {
            if (Actor.State.HasFlag(CharacterState.Dead)) break;
            var trait = Gameworld.Traits.Get(overdose.TraitId);
            if (trait is null || overdose.GramsPerTraitPoint <= 0.0) continue;
            var activeDoseGrams = string.IsNullOrWhiteSpace(overdose.Group)
                ? ActiveDrugDosages.Where(x => x.Drug.Id == drug.Id).Sum(x => x.Grams)
                : ActiveGramsForOverdoseGroup(
                    overdoseConfigurations.Select(x => (x.Drug.Id, x.Config.Group)),
                    ActiveDrugDosages.Select(x => (x.Drug.Id, x.Grams)), overdose.Group);
            if (IsDrugOverdoseThresholdReached(activeDoseGrams, Actor.TraitValue(trait), overdose.GramsPerTraitPoint))
            {
                Actor.Die();
                break;
            }
        }
    }

	private DoubleCounter<IDrug> ActiveDrugIntensities(bool includeInfections = true)
	{
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

        foreach (var infection in includeInfections ? PartInfections.OfType<IDrugSymptomInfection>() : Enumerable.Empty<IDrugSymptomInfection>())
        {
            if (infection.SymptomDrug is not { } profile || profile.IntensityPerGram <= 0 || profile.DrugTypes.Contains(DrugType.Infection)) continue;
            var grams = InfectionEquivalentGrams(infection);
            if (double.IsFinite(grams) && grams > 0) drugIntensities[profile] += grams;
        }
		return drugIntensities;
	}

	private double DrugEffectIntensity(IDrug drug, DrugType type, double adjustedGrams,
		IEnumerable<IDrugEffectResistanceMerit> resistanceMerits) =>
		drug.IntensityForType(type) * adjustedGrams * ToleranceMultiplierForDrugType(drug, type) *
		resistanceMerits.Aggregate(1.0, (value, merit) => value * merit.ModifierForDrugType(type));

	/// <summary>Refreshes only nausea symptoms; never advances metabolism or applies heartbeat consequences.</summary>
	public void RefreshNausea()
	{
		if (_loading) return;
		if (!IsActiveCharacterBody)
		{
			EffectHandler.RemoveAllEffects(x => x is Nausea, true);
			return;
		}
		var resistanceMerits = Actor.Merits.OfType<IDrugEffectResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
		var specificMerits = Actor.Merits.OfType<ISpecificDrugResistanceMerit>().Where(x => x.Applies(Actor)).ToList();
		var intensity = CombinedEffectsOfType<ICauseDrugEffect>()
			.Where(x => x.AffectedDrugTypes.Contains(DrugType.Nausea))
			.Sum(x => x.AddedIntensity(Actor, DrugType.Nausea)) *
			resistanceMerits.Aggregate(1.0, (value, merit) => value * merit.ModifierForDrugType(DrugType.Nausea));
		var neutralisation = 0.0;
		foreach (var (drug, grams) in ActiveDrugIntensities())
		{
			if (grams <= 0.0) continue;
			var adjustedGrams = grams * specificMerits.Aggregate(1.0, (value, merit) => value * merit.MultiplierForDrug(drug));
			if (drug.DrugTypes.Contains(DrugType.Nausea))
				intensity += DrugEffectIntensity(drug, DrugType.Nausea, adjustedGrams, resistanceMerits);
			if (drug.DrugTypes.Contains(DrugType.NeutraliseDrugEffect) &&
				drug.AdditionalInfoFor<NeutraliseDrugAdditionalInfo>(DrugType.NeutraliseDrugEffect).NeutralisedTypes.Contains(DrugType.Nausea))
				neutralisation += drug.IntensityForType(DrugType.NeutraliseDrugEffect) * adjustedGrams;
		}

		var bac = CurrentBloodVolumeLitres > 0.0 ? 10.0 * NeedsModel.AlcoholLitres / CurrentBloodVolumeLitres : 0.0;
		var net = intensity - neutralisation;
		if (net <= 0.0 && bac <= 0.0)
		{
			EffectHandler.RemoveAllEffects(x => x is Nausea, true);
			return;
		}
		var nausea = EffectsOfType<Nausea>().FirstOrDefault();
		if (nausea is null)
		{
			nausea = new Nausea(this, 0.0, 0.0);
			EffectHandler.AddEffect(nausea, TimeSpan.FromSeconds(35));
		}
		nausea.IntensityPerGramMass = Weight > 0.0 ? IntensityPerGramMass(net) : 0.0;
		nausea.BloodAlcoholContent = bac;
	}

    private double InfectionEquivalentGrams(IDrugSymptomInfection infection) =>
        infection.SymptomIntensity * Weight * Gameworld.UnitManager.BaseWeightToKilograms * 0.001 /
        (infection.SymptomDrug?.IntensityPerGram ?? 1.0);

    public void RefreshDrugEffects()
    {
        if (_loading || !ReferenceEquals(Actor.Body, this)) return;
        ApplyDrugEffects();
    }

    private void ApplyDrugEffects()
    {
        DoubleCounter<DrugType> neutralisingEffects = new();
        DoubleCounter<DrugType> effectIntensities = new();

        double healingRateIntensity = 0.0;
        double healingDifficultyIntensity = 0.0;

        var drugIntensities = ActiveDrugIntensities();

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

                if (drugEffect is DrugType.Dependence or DrugType.Infection)
                {
                    continue;
                }

                var adjustedEffectIntensity = DrugEffectIntensity(drug, drugEffect, adjustedgrams, resistanceMerits);
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

        var symptomDrugIds = new HashSet<long>();
        var physicalSymptomDrugIds = _activeDrugDosages.Concat(_latentDrugDosages)
            .Where(x => x.Grams > 0.0).Select(x => x.Drug.Id).ToHashSet();
        foreach (var (drug, grams) in ActiveDrugIntensities(includeInfections: false))
        {
            if (!drug.DrugTypes.Contains(DrugType.Symptoms) || grams <= 0.0 || !physicalSymptomDrugIds.Contains(drug.Id)) continue;
            var adjustedGrams = specificMerits.Aggregate(grams, (value, merit) => value * merit.MultiplierForDrug(drug));
            var intensity = IntensityPerGramMass(DrugEffectIntensity(drug, DrugType.Symptoms, adjustedGrams, resistanceMerits)) *
                            NetIntensityScale(DrugType.Symptoms);
            if (!double.IsFinite(intensity) || intensity <= 0.0) continue;
            symptomDrugIds.Add(drug.Id);
            var symptoms = EffectsOfType<DrugSymptomEffect>().FirstOrDefault(x => x.DrugId == drug.Id && x.SourceInfectionDrugId == 0);
            if (symptoms is null)
            {
                symptoms = new DrugSymptomEffect(this, drug.Id,
                    drug.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms).InitialStage);
                AddEffect(symptoms);
            }
            if (symptoms.Intensity <= 0.0) symptoms.ResetClock();
            symptoms.Intensity = intensity;
            symptoms.ApplyOnset();
        }
        foreach (var symptoms in EffectsOfType<DrugSymptomEffect>().Where(x => x.SourceInfectionDrugId == 0 && !symptomDrugIds.Contains(x.DrugId)))
            symptoms.Intensity = 0.0;
        RemoveAllEffects<DrugSymptomEffect>(x => x.SourceInfectionDrugId == 0 && !physicalSymptomDrugIds.Contains(x.DrugId));
        var infections = PartInfections.OfType<IDrugSymptomInfection>().ToArray();
        foreach (var infection in infections)
        {
            if (infection.SymptomDrug is not { } profile || !profile.DrugTypes.Contains(DrugType.Symptoms)) continue;
            var adjustedGrams = specificMerits.Aggregate(InfectionEquivalentGrams(infection), (value, merit) => value * merit.MultiplierForDrug(profile));
            var intensity = IntensityPerGramMass(DrugEffectIntensity(profile, DrugType.Symptoms, adjustedGrams, resistanceMerits)) * NetIntensityScale(DrugType.Symptoms);
            var symptoms = EffectsOfType<DrugSymptomEffect>().FirstOrDefault(x => x.SourceInfectionDrugId == infection.DefinitionDrugId);
            if (symptoms is not null && symptoms.DrugId != profile.Id)
            {
                RemoveEffect(symptoms, true);
                symptoms = null;
            }
            if (symptoms is null)
            {
                symptoms = new DrugSymptomEffect(this, profile.Id, profile.AdditionalInfoFor<DrugSymptomsAdditionalInfo>(DrugType.Symptoms).InitialStage, infection.DefinitionDrugId);
                AddEffect(symptoms);
            }
            symptoms.Intensity = double.IsFinite(intensity) ? Math.Max(0, intensity) : 0;
            symptoms.ApplyOnset();
        }
        RemoveAllEffects<DrugSymptomEffect>(x => x.SourceInfectionDrugId != 0 && !infections.Any(i => i.DefinitionDrugId == x.SourceInfectionDrugId));

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
                    // Reconciled by RefreshNausea below, also used by timed physical sources.
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

        RefreshNausea();

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

        RefreshDrugModifierEffects();
        CheckDrugOverdose();
		if (Actor.State.HasFlag(CharacterState.Dead) || !IsActiveCharacterBody || !ReferenceEquals(Actor.Body, this))
		{
			EndDrugTick();
			return;
		}

        // Kick off the health tick if necessary
        if (effectIntensities.Any() || NeedsModel.AlcoholLitres > 0.0)
        {
            StartHealthTick();
        }
    }

    private void MetaboliseActiveDrugs()
    {
        var naturallyRemovedDrugIds = new HashSet<long>();
        double rate = LiverAlcoholRemovalKilogramsPerHour * 10000.0 / 3600.0;
        foreach (DrugDosage drug in ActiveDrugDosages.ToList())
        {
            double removed = rate * drug.Drug.RelativeMetabolisationRate;
            drug.Grams -= removed;
            if (drug.Grams <= 0)
            {
                _activeDrugDosages.Remove(drug);
                naturallyRemovedDrugIds.Add(drug.Drug.Id);
            }

            DrugsChanged = true;
        }
        var remainingDrugIds = _activeDrugDosages.Concat(_latentDrugDosages)
            .Where(x => x.Grams > 0.0).Select(x => x.Drug.Id).ToHashSet();
        var expiredSymptoms = EffectsOfType<DrugSymptomEffect>().Where(x =>
            x.SourceInfectionDrugId == 0 && naturallyRemovedDrugIds.Contains(x.DrugId) && !remainingDrugIds.Contains(x.DrugId)).ToArray();
        // Detach the whole batch before callbacks: expiry of A can administer a new dose of B.
        foreach (var symptoms in expiredSymptoms) RemoveEffect(symptoms);
        foreach (var symptoms in expiredSymptoms) symptoms.ApplyNaturalExpiry();
    }

    private void SaveDrugs(MudSharp.Models.Body body)
    {
        ReconcileDrugDoseRows(body.BodiesDrugDoses, _activeDrugDosages, _latentDrugDosages);

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
                dbitem = new BodyDrugExposure { DrugId = item.Drug.Id };
                body.BodiesDrugExposures.Add(dbitem);
            }

            dbitem.Exposure = item.Exposure;
            dbitem.PeakExposure = item.PeakExposure;
            dbitem.WithdrawalIntensity = item.WithdrawalIntensity;
            dbitem.LastUpdatedAtUtc = item.LastUpdatedAtUtc;
        }

        DrugsChanged = false;
    }

    internal static DrugDosage RestoreDrugDoseRow(BodyDrugDose row, IDrug drug) => new()
    {
        Drug = drug,
        Grams = row.Grams,
        OriginalVector = (DrugVector)row.OriginalVector,
        SpellOriginId = row.SpellOriginId,
        SpellOriginFamily = row.SpellOriginFamily,
        SpellOriginFormula = row.SpellOriginFormula
    };

    internal static void ReconcileDrugDoseRows(ICollection<BodyDrugDose> rows,
        IEnumerable<DrugDosage> active, IEnumerable<DrugDosage> latent)
    {
        var desired = active.Select(x => (Dose: x, Active: true))
            .Concat(latent.Select(x => (Dose: x, Active: false))).ToArray();
        List<BodyDrugDose> drugsToDelete =
            rows.Where(x => !desired.Any(y => y.Dose.Drug.Id == x.DrugId && y.Active == x.Active &&
                y.Dose.SpellOriginId == x.SpellOriginId)).ToList();
        foreach (BodyDrugDose drug in drugsToDelete)
        {
            rows.Remove(drug);
        }

        foreach (var (item, isActive) in desired)
        {
            BodyDrugDose dbitem = rows.FirstOrDefault(x => x.Active == isActive && x.DrugId == item.Drug.Id &&
                x.SpellOriginId == item.SpellOriginId);
            if (dbitem == null)
            {
                dbitem = new Models.BodyDrugDose
                {
                    DrugId = item.Drug.Id,
                    Active = isActive,
                    OriginalVector = (int)item.OriginalVector,
                    SpellOriginId = item.SpellOriginId,
                    SpellOriginFamily = item.SpellOriginFamily,
                    SpellOriginFormula = item.SpellOriginFormula
                };
                rows.Add(dbitem);
            }

            dbitem.Grams = item.Grams;
            dbitem.OriginalVector = (int)item.OriginalVector;
            dbitem.SpellOriginFamily = item.SpellOriginFamily;
            dbitem.SpellOriginFormula = item.SpellOriginFormula;
        }
    }

    #endregion
}
