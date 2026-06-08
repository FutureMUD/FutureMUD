using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Framework;
using MudSharp.Health;
using MudSharp.Models;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DrugMetadataTests
{
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
}
