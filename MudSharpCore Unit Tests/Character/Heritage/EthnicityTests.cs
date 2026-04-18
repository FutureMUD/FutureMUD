#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp_Unit_Tests.Character.Heritage;

[TestClass]
public class EthnicityTests
{
    [TestMethod]
    public void GetCharacteristicProfileSelections_PrefersExactAllProfile()
    {
        Mock<ICharacteristicDefinition> characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
        Mock<ICharacteristicDefinition> parentCharacteristic = CreateCharacteristicDefinition(2L, "Appearance");
        Mock<IRace> race = CreateRace(characteristic.Object);
        Mock<ICharacteristicProfile> exactStandard = CreateCharacteristicProfile(
            10L,
            "Eye Colour Standard",
            "Standard",
            characteristic.Object,
            characteristic.Object);
        Mock<ICharacteristicProfile> inheritedAll = CreateCharacteristicProfile(
            11L,
            "Appearance All",
            "All",
            parentCharacteristic.Object,
            characteristic.Object);
        Mock<ICharacteristicProfile> exactAll = CreateCharacteristicProfile(
            12L,
            "Eye Colour All",
            "All",
            characteristic.Object,
            characteristic.Object);

        (ICharacteristicDefinition Characteristic, ICharacteristicProfile Profile) selection = Ethnicity.GetCharacteristicProfileSelections(
                race.Object,
                [exactStandard.Object, inheritedAll.Object, exactAll.Object])
            .Single();

        Assert.AreSame(exactAll.Object, selection.Profile);
    }

    [TestMethod]
    public void GetCharacteristicProfileSelections_UsesInheritedApplicableProfile()
    {
        Mock<ICharacteristicDefinition> characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
        Mock<ICharacteristicDefinition> parentCharacteristic = CreateCharacteristicDefinition(2L, "Appearance");
        Mock<IRace> race = CreateRace(characteristic.Object);
        Mock<ICharacteristicProfile> inheritedAll = CreateCharacteristicProfile(
            11L,
            "Appearance All",
            "All",
            parentCharacteristic.Object,
            characteristic.Object);

        (ICharacteristicDefinition Characteristic, ICharacteristicProfile Profile) selection = Ethnicity.GetCharacteristicProfileSelections(race.Object, [inheritedAll.Object]).Single();

        Assert.AreSame(inheritedAll.Object, selection.Profile);
    }

    [TestMethod]
    public void GetCharacteristicProfileSelections_ReturnsNullWhenNoApplicableProfileExists()
    {
        Mock<ICharacteristicDefinition> characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
        Mock<ICharacteristicDefinition> otherCharacteristic = CreateCharacteristicDefinition(2L, "Skin Tone");
        Mock<IRace> race = CreateRace(characteristic.Object);
        Mock<ICharacteristicProfile> unrelatedProfile = CreateCharacteristicProfile(
            12L,
            "Skin Tone All",
            "All",
            otherCharacteristic.Object);

        (ICharacteristicDefinition Characteristic, ICharacteristicProfile Profile) selection = Ethnicity.GetCharacteristicProfileSelections(race.Object, [unrelatedProfile.Object]).Single();

        Assert.IsNull(selection.Profile);
    }

    private static Mock<ICharacteristicDefinition> CreateCharacteristicDefinition(long id, string name)
    {
        Mock<ICharacteristicDefinition> mock = new();
        mock.SetupGet(x => x.Id).Returns(id);
        mock.SetupGet(x => x.Name).Returns(name);
        return mock;
    }

    private static Mock<IRace> CreateRace(params ICharacteristicDefinition[] characteristics)
    {
        Mock<IRace> mock = new();
        mock.SetupGet(x => x.Id).Returns(99L);
        mock.SetupGet(x => x.Name).Returns("Human");
        mock.Setup(x => x.Characteristics(Gender.Indeterminate)).Returns(characteristics);
        return mock;
    }

    private static Mock<ICharacteristicProfile> CreateCharacteristicProfile(
        long id,
        string name,
        string type,
        ICharacteristicDefinition targetDefinition,
        params ICharacteristicDefinition[] applicableDefinitions)
    {
        Mock<ICharacteristicProfile> mock = new();
        mock.SetupGet(x => x.Id).Returns(id);
        mock.SetupGet(x => x.Name).Returns(name);
        mock.SetupGet(x => x.Type).Returns(type);
        mock.SetupGet(x => x.TargetDefinition).Returns(targetDefinition);
        mock.Setup(x => x.IsProfileFor(It.IsAny<ICharacteristicDefinition>()))
            .Returns<ICharacteristicDefinition>(definition => applicableDefinitions.Contains(definition));
        return mock;
    }
}
