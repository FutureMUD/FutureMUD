using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Health;
using MudSharp.Models;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugMetadataTests
{
    private sealed record FMDBState(FuturemudDatabaseContext Context, object Connection, uint InstanceCount);

    private static FuturemudDatabaseContext BuildContext()
    {
        DbContextOptions<FuturemudDatabaseContext> options = new DbContextOptionsBuilder<FuturemudDatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        return new FuturemudDatabaseContext(options);
    }

    private static FMDBState CaptureFMDBState()
    {
        return new FMDBState(
            (FuturemudDatabaseContext)typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!
                .GetValue(null),
            typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.GetValue(null),
            (uint)typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
                .GetValue(null)!);
    }

    private static void RestoreFMDBState(FMDBState state)
    {
        typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Context);
        typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, state.Connection);
        typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!
            .SetValue(null, state.InstanceCount);
    }

    private static void PrimeFMDB(FuturemudDatabaseContext context)
    {
        typeof(FMDB).GetProperty("Context", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, context);
        typeof(FMDB).GetProperty("Connection", BindingFlags.Public | BindingFlags.Static)!.SetValue(null, null);
        typeof(FMDB).GetProperty("InstanceCount", BindingFlags.NonPublic | BindingFlags.Static)!.SetValue(null, 1u);
    }

    [TestMethod]
    public void DrugAdditionalInfo_LoadsEmptyAndLegacyFixedPointIdLists()
    {
        MudSharp.Models.Drug model = new()
        {
            Id = 1,
            Name = "test",
            DrugVectors = (int)DrugVector.Ingested,
            IntensityPerGram = 1.0,
            RelativeMetabolisationRate = 1.0
        };
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.NeutraliseSpecificDrug,
            RelativeIntensity = 1.0,
            AdditionalEffects = ""
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.MagicAbility,
            RelativeIntensity = 1.0,
            AdditionalEffects = "1.00 2"
        });

        MudSharp.Health.Drug drug = new(model, new Mock<IFuturemud>().Object);

        Assert.AreEqual(0, drug.AdditionalInfoFor<NeutraliseSpecificDrugAdditionalInfo>(DrugType.NeutraliseSpecificDrug).NeutralisedIds.Count);
        CollectionAssert.AreEqual(new[] { 1L, 2L },
            drug.AdditionalInfoFor<MagicAbilityAdditionalInfo>(DrugType.MagicAbility).MagicCapabilityIds.ToArray());
    }

    [TestMethod]
    public void DrugAdditionalInfo_SerializesIntegerIdLists()
    {
        MagicAbilityAdditionalInfo info = new() { MagicCapabilityIds = [1, 2] };

        Assert.AreEqual("1 2", info.DatabaseString);
    }

    [TestMethod]
    public void DrugAdditionalInfo_LoadsExpansionPayloads()
    {
        MudSharp.Models.Drug model = new()
        {
            Id = 1,
            Name = "expansion",
            DrugVectors = (int)DrugVector.Ingested,
            IntensityPerGram = 1.0,
            RelativeMetabolisationRate = 1.0
        };
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Coagulation,
            RelativeIntensity = 1.0,
            AdditionalEffects = new CoagulationAdditionalInfo
            {
                ExternalBleedingMultiplier = 0.5,
                WoundReopenMultiplier = 0.75,
                InternalBleedingMultiplier = 1.25
            }.DatabaseString
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Respiration,
            RelativeIntensity = 1.0,
            AdditionalEffects = new RespirationAdditionalInfo
            {
                BreathingDriveMultiplier = 1.5,
                HypoxiaDamageMultiplier = 0.8,
                AirwayToleranceMultiplier = 1.2
            }.DatabaseString
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.NeedRate,
            RelativeIntensity = 1.0,
            AdditionalEffects = new NeedRateAdditionalInfo
            {
                HungerMultiplier = 0.7,
                ThirstMultiplier = 1.3,
                DrunkennessMultiplier = 0.9,
                AppliesToPassive = true,
                AppliesToActive = false
            }.DatabaseString
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Arousal,
            RelativeIntensity = 1.0,
            AdditionalEffects = new ArousalAdditionalInfo
            {
                Mode = DrugArousalMode.SleepPreventing | DrugArousalMode.PassOutResistance,
                CheckBonusPerIntensity = 0.1,
                SleepIntensityThreshold = 0.6,
                KnockoutIntensityThreshold = 1.2,
                PainPassOutThresholdMultiplier = 1.1,
                StunUnconsciousThresholdMultiplier = 1.2,
                AnesthesiaUnconsciousThresholdMultiplier = 1.3,
                StaminaRegenMultiplier = 1.4,
                StaminaCostMultiplier = 0.8
            }.DatabaseString
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Dependence,
            RelativeIntensity = 1.0,
            AdditionalEffects = new DrugDependenceAdditionalInfo
            {
                ExposureGainPerGram = 0.9,
                ExposureDecayPerDay = 0.1,
                ToleranceThreshold = 5.0,
                MinimumToleranceMultiplier = 0.35,
                WithdrawalThreshold = 2.5,
                WithdrawalDecayPerDay = 0.2,
                AffectedDrugTypes = [DrugType.Analgesic, DrugType.Arousal],
                WithdrawalCheckPenalty = -0.1,
                WithdrawalHungerMultiplier = 1.2,
                WithdrawalThirstMultiplier = 1.3,
                WithdrawalStaminaRegenMultiplier = 0.7,
                WithdrawalStaminaCostMultiplier = 1.4,
                WithdrawalNauseaIntensity = 0.5,
                WithdrawalRageIntensity = 0.2,
                SleepPreventionThreshold = 0.25
            }.DatabaseString
        });

        MudSharp.Health.Drug drug = new(model, new Mock<IFuturemud>().Object);

        Assert.AreEqual(0.5, drug.AdditionalInfoFor<CoagulationAdditionalInfo>(DrugType.Coagulation).ExternalBleedingMultiplier);
        Assert.AreEqual(1.5, drug.AdditionalInfoFor<RespirationAdditionalInfo>(DrugType.Respiration).BreathingDriveMultiplier);
        Assert.IsFalse(drug.AdditionalInfoFor<NeedRateAdditionalInfo>(DrugType.NeedRate).AppliesToActive);
        Assert.IsTrue(drug.AdditionalInfoFor<ArousalAdditionalInfo>(DrugType.Arousal).Mode.HasFlag(DrugArousalMode.PassOutResistance));
        CollectionAssert.AreEqual(new[] { DrugType.Analgesic, DrugType.Arousal },
            drug.AdditionalInfoFor<DrugDependenceAdditionalInfo>(DrugType.Dependence).AffectedDrugTypes.ToArray());
    }

    [TestMethod]
    public void DrugGetProperty_IntensitiesKeepsLegacyItensitiesAlias()
    {
        MudSharp.Models.Drug model = new()
        {
            Id = 1,
            Name = "alias",
            DrugVectors = (int)DrugVector.Ingested,
            IntensityPerGram = 1.0,
            RelativeMetabolisationRate = 1.0
        };
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Analgesic,
            RelativeIntensity = 0.25,
            AdditionalEffects = string.Empty
        });
        model.DrugsIntensities.Add(new DrugIntensity
        {
            DrugType = (int)DrugType.Arousal,
            RelativeIntensity = 0.75,
            AdditionalEffects = string.Empty
        });

        MudSharp.Health.Drug drug = new(model, new Mock<IFuturemud>().Object);

        var intensities = ((IEnumerable)drug.GetProperty("intensities")).Cast<IProgVariable>().Select(x => x.GetObject).ToArray();
        var legacyAlias = ((IEnumerable)drug.GetProperty("itensities")).Cast<IProgVariable>().Select(x => x.GetObject).ToArray();

        CollectionAssert.AreEqual(intensities, legacyAlias);
        CollectionAssert.AreEqual(new object[] { 0.25m, 0.75m }, intensities);
    }

    [TestMethod]
    public void Clone_ScalarOnlyEffects_PreserveNullAdditionalInfo()
    {
        FMDBState state = CaptureFMDBState();
        using FuturemudDatabaseContext context = BuildContext();
        try
        {
            PrimeFMDB(context);
            MudSharp.Models.Drug model = new()
            {
                Id = 1,
                Name = "simple analgesic",
                DrugVectors = (int)DrugVector.Ingested,
                IntensityPerGram = 1.0,
                RelativeMetabolisationRate = 1.0
            };
            model.DrugsIntensities.Add(new DrugIntensity
            {
                DrugType = (int)DrugType.Analgesic,
                RelativeIntensity = 0.5
            });
            MudSharp.Health.Drug drug = new(model, new Mock<IFuturemud>().Object);

            MudSharp.Health.Drug clone = (MudSharp.Health.Drug)drug.Clone("simple analgesic copy");

            Assert.AreEqual("simple analgesic copy", clone.Name);
            Assert.AreEqual(0.5, clone.IntensityForType(DrugType.Analgesic));
            Assert.IsNull(clone.DrugTypeMulipliers[DrugType.Analgesic].ExtraInfo);

            MudSharp.Models.Drug persisted = context.Drugs
                .Include(x => x.DrugsIntensities)
                .Single();
            Assert.AreEqual("simple analgesic copy", persisted.Name);
            Assert.IsNull(persisted.DrugsIntensities.Single().AdditionalEffects);
        }
        finally
        {
            RestoreFMDBState(state);
        }
    }
}
