using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Climate;
using MudSharp.GameItems.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moq;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;

namespace MudSharp_Unit_Tests;

[TestClass]
public class MaterialTests
{
    private int SecondsToEvaporate(double initial)
    {
        for (int i = 0; i < 864000; i++)
        {
            initial -= PuddleGameItemComponent.EvaporationRatePerSecond(initial, 1.0, 25.0, PrecipitationLevel.Dry,
                WindLevel.Breeze);
            if (initial <= 0.0)
            {
                return i;
            }
        }

        return 864000;
    }

    [TestMethod]
    public void TestEvaporation()
    {
        double evaporation0 =
            PuddleGameItemComponent.EvaporationRatePerSecond(1000.0, 1.0, 25.0, PrecipitationLevel.Dry,
                WindLevel.Breeze);
        double evaporation1 =
            PuddleGameItemComponent.EvaporationRatePerSecond(10.0, 1.0, 25.0, PrecipitationLevel.Dry,
                WindLevel.Breeze);
        double evaporation2 =
            PuddleGameItemComponent.EvaporationRatePerSecond(1.0, 1.0, 25.0, PrecipitationLevel.Dry,
                WindLevel.Breeze);
        double evaporation3 =
            PuddleGameItemComponent.EvaporationRatePerSecond(0.011, 1.0, 25.0, PrecipitationLevel.Dry,
                WindLevel.Breeze);

        double seconds0 = 1000.0 / evaporation0;
        double seconds1 = 10.0 / evaporation1;
        double seconds2 = 1.0 / evaporation2;
        double seconds3 = 0.011 / evaporation3;

        Dictionary<double, int> secondsDictionary = new();
        secondsDictionary[0.011] = SecondsToEvaporate(0.011);
        secondsDictionary[0.1] = SecondsToEvaporate(0.1);
        secondsDictionary[0.4] = SecondsToEvaporate(0.4);
        secondsDictionary[1.0] = SecondsToEvaporate(1.0);
        secondsDictionary[3.5] = SecondsToEvaporate(3.5);
        secondsDictionary[15] = SecondsToEvaporate(15);
        secondsDictionary[150] = SecondsToEvaporate(150);
        secondsDictionary[1500] = SecondsToEvaporate(1500);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public void GasCountsAs_CyclicChainForOutsideTarget_ReturnsFalse()
    {
        var gameworld = new Mock<IFuturemud>();
        var gases = new All<IGas>();
        gameworld.SetupGet(x => x.Gases).Returns(gases);
        gameworld.SetupGet(x => x.Tags).Returns(new All<ITag>());
        gameworld.SetupGet(x => x.Drugs).Returns(new All<IDrug>());
        var gasA = new Gas(GasModel(1, "gas a", 2), gameworld.Object);
        var gasB = new Gas(GasModel(2, "gas b", 1), gameworld.Object);
        var gasC = new Gas(GasModel(3, "gas c", null), gameworld.Object);
        gases.Add(gasA);
        gases.Add(gasB);
        gases.Add(gasC);

        Assert.IsFalse(gasA.CountsAs(gasC));
        Assert.AreEqual(ItemQuality.Terrible, gasA.CountAsQuality(gasC));
        Assert.AreEqual(0.0, gasA.CountsAsMultiplier(gasC), 0.0001);
    }

    [TestMethod]
    public void LiquidCountsAs_CyclicChainForOutsideTarget_ReturnsFalse()
    {
        var gameworld = new Mock<IFuturemud>();
        var liquids = new All<ILiquid>();
        gameworld.SetupGet(x => x.Liquids).Returns(liquids);
        gameworld.SetupGet(x => x.Gases).Returns(new All<IGas>());
        gameworld.SetupGet(x => x.Tags).Returns(new All<ITag>());
        gameworld.SetupGet(x => x.Drugs).Returns(new All<IDrug>());
        var liquidA = new Liquid(LiquidModel(1, "liquid a", 2), gameworld.Object);
        var liquidB = new Liquid(LiquidModel(2, "liquid b", 1), gameworld.Object);
        var liquidC = new Liquid(LiquidModel(3, "liquid c", null), gameworld.Object);
        liquids.Add(liquidA);
        liquids.Add(liquidB);
        liquids.Add(liquidC);

        Assert.IsFalse(liquidA.CountsAs(liquidC));
        Assert.IsFalse(liquidA.LiquidCountsAs(liquidC));
        Assert.AreEqual(ItemQuality.Terrible, liquidA.CountAsQuality(liquidC));
        Assert.AreEqual(0.0, liquidA.CountsAsMultiplier(liquidC), 0.0001);
    }

    private static MudSharp.Models.Gas GasModel(long id, string name, long? countsAsId)
    {
        return new MudSharp.Models.Gas
        {
            Id = id,
            Name = name,
            Description = name,
            Density = 1.0,
            Organic = false,
            ThermalConductivity = 0.14,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420.0,
            SmellText = string.Empty,
            VagueSmellText = string.Empty,
            SmellIntensity = 100.0,
            Viscosity = 1.0,
            CountAsId = countsAsId,
            CountsAsQuality = (int)ItemQuality.Standard,
            DisplayColour = Telnet.BoldCyan.Name,
            BoilingPoint = double.MinValue,
            DrugGramsPerUnitVolume = 0.0,
            OxidationFactor = 1.0
        };
    }

    private static MudSharp.Models.Liquid LiquidModel(long id, string name, long? countsAsId)
    {
        return new MudSharp.Models.Liquid
        {
            Id = id,
            Name = name,
            Description = name,
            LongDescription = name,
            Density = 1000.0,
            Organic = false,
            ThermalConductivity = 0.14,
            ElectricalConductivity = 0.0001,
            SpecificHeatCapacity = 420.0,
            TasteText = string.Empty,
            VagueTasteText = string.Empty,
            TasteIntensity = 100.0,
            SmellText = string.Empty,
            VagueSmellText = string.Empty,
            SmellIntensity = 100.0,
            AlcoholLitresPerLitre = 0.0,
            WaterLitresPerLitre = 1.0,
            FoodSatiatedHoursPerLitre = 0.0,
            DrinkSatiatedHoursPerLitre = 0.0,
            BoilingPoint = 100.0,
            FreezingPoint = 0.0,
            Viscosity = 1.0,
            IgnitionPoint = 0.0,
            CountAsId = countsAsId,
            CountAsQuality = (int)ItemQuality.Standard,
            DisplayColour = Telnet.BoldCyan.Name,
            DampDescription = "It is damp",
            WetDescription = "It is wet",
            DrenchedDescription = "It is drenched",
            DampShortDescription = "(damp)",
            WetShortDescription = "(wet)",
            DrenchedShortDescription = "(drenched)",
            SolventVolumeRatio = 1.0,
            DrugGramsPerUnitVolume = 0.0,
            InjectionConsequence = (int)LiquidInjectionConsequence.Harmful,
            ResidueVolumePercentage = 0.05,
            RelativeEnthalpy = 1.0,
            SurfaceReactionInfo = "<Reactions />"
        };
    }
}
