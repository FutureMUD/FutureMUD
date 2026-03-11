#nullable enable

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Character.Heritage;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;

namespace MudSharp_Unit_Tests.Character.Heritage;

[TestClass]
public class EthnicityTests
{
	[TestMethod]
	public void GetCharacteristicProfileSelections_PrefersExactAllProfile()
	{
		var characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
		var parentCharacteristic = CreateCharacteristicDefinition(2L, "Appearance");
		var race = CreateRace(characteristic.Object);
		var exactStandard = CreateCharacteristicProfile(
			10L,
			"Eye Colour Standard",
			"Standard",
			characteristic.Object,
			characteristic.Object);
		var inheritedAll = CreateCharacteristicProfile(
			11L,
			"Appearance All",
			"All",
			parentCharacteristic.Object,
			characteristic.Object);
		var exactAll = CreateCharacteristicProfile(
			12L,
			"Eye Colour All",
			"All",
			characteristic.Object,
			characteristic.Object);

		var selection = Ethnicity.GetCharacteristicProfileSelections(
				race.Object,
				[exactStandard.Object, inheritedAll.Object, exactAll.Object])
			.Single();

		Assert.AreSame(exactAll.Object, selection.Profile);
	}

	[TestMethod]
	public void GetCharacteristicProfileSelections_UsesInheritedApplicableProfile()
	{
		var characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
		var parentCharacteristic = CreateCharacteristicDefinition(2L, "Appearance");
		var race = CreateRace(characteristic.Object);
		var inheritedAll = CreateCharacteristicProfile(
			11L,
			"Appearance All",
			"All",
			parentCharacteristic.Object,
			characteristic.Object);

		var selection = Ethnicity.GetCharacteristicProfileSelections(race.Object, [inheritedAll.Object]).Single();

		Assert.AreSame(inheritedAll.Object, selection.Profile);
	}

	[TestMethod]
	public void GetCharacteristicProfileSelections_ReturnsNullWhenNoApplicableProfileExists()
	{
		var characteristic = CreateCharacteristicDefinition(1L, "Eye Colour");
		var otherCharacteristic = CreateCharacteristicDefinition(2L, "Skin Tone");
		var race = CreateRace(characteristic.Object);
		var unrelatedProfile = CreateCharacteristicProfile(
			12L,
			"Skin Tone All",
			"All",
			otherCharacteristic.Object);

		var selection = Ethnicity.GetCharacteristicProfileSelections(race.Object, [unrelatedProfile.Object]).Single();

		Assert.IsNull(selection.Profile);
	}

	private static Mock<ICharacteristicDefinition> CreateCharacteristicDefinition(long id, string name)
	{
		var mock = new Mock<ICharacteristicDefinition>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		return mock;
	}

	private static Mock<IRace> CreateRace(params ICharacteristicDefinition[] characteristics)
	{
		var mock = new Mock<IRace>();
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
		var mock = new Mock<ICharacteristicProfile>();
		mock.SetupGet(x => x.Id).Returns(id);
		mock.SetupGet(x => x.Name).Returns(name);
		mock.SetupGet(x => x.Type).Returns(type);
		mock.SetupGet(x => x.TargetDefinition).Returns(targetDefinition);
		mock.Setup(x => x.IsProfileFor(It.IsAny<ICharacteristicDefinition>()))
			.Returns<ICharacteristicDefinition>(definition => applicableDefinitions.Contains(definition));
		return mock;
	}
}
