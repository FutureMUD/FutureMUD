using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Body.Disfigurements;
using MudSharp.CharacterCreation;
using MudSharp.Form.Characteristics;
using MudSharp.Form.Shape;
using MudSharp.Framework;
using MudSharp.GameItems.Interfaces;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class CharacteristicMarkupTests
{
	private static readonly Gendering Gender = Gendering.Get(MudSharp.Form.Shape.Gender.Male);

	private static Mock<IPerceivableHaveCharacteristics> Owner(ICharacteristicDefinition definition)
	{
		var owner = new Mock<IPerceivableHaveCharacteristics>();
		owner.Setup(x => x.ApparentGender(It.IsAny<IPerceiver>())).Returns(Gender);
		owner.Setup(x => x.GetCharacteristicDefinition(It.IsAny<string>()))
			.Returns(Tuple.Create(definition, CharacteristicDescriptionType.Normal));
		owner.Setup(x => x.DescribeCharacteristic(definition, It.IsAny<IPerceiver>(), CharacteristicDescriptionType.Normal))
			.Returns("red");
		return owner;
	}

	[TestMethod]
	[DataRow("$colour[][hidden]", "")]
	[DataRow("$!colour[][hidden]", "$colour[][hidden]")]
	[DataRow("$!colour[]", "$colour[]")]
	[DataRow("$colour[@ hair][hidden]", "red hair")]
	public void ParseCharacteristics_ExplicitEmptyAndDeferredBranches_MatchPreview(string input, string expected)
	{
		var definition = Mock.Of<ICharacteristicDefinition>(x => x.Pattern == new Regex("^colour$"));
		var value = Mock.Of<ICharacteristicValue>(x => x.GetValue == "red");
		var owner = Owner(definition);
		Assert.AreEqual(expected, owner.Object.ParseCharacteristics(input, null!));
		Assert.AreEqual(expected, IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(input,
			new[] { (definition, value) }, Gender, null!));
	}

	[TestMethod]
	[DataRow("&HE &HIM &HIS &HIMSELF", "he him his himself")]
	[DataRow("&RACE &CULTURE &AGE", "unknown unknown adult")]
	public void ParseCharacteristics_UppercaseHelpers_MatchPreview(string input, string expected)
	{
		var owner = Owner(Mock.Of<ICharacteristicDefinition>());
		Assert.AreEqual(expected, owner.Object.ParseCharacteristics(input, null!));
		Assert.AreEqual(expected, IHaveCharacteristicsExtensions.ParseCharacteristicsAbsolute(input,
			Array.Empty<(ICharacteristicDefinition, ICharacteristicValue)>(), Gender, null!));
	}

	[TestMethod]
	public void ParseCharacteristics_NestedDefaultCheck_PreservesIgnoreObscurers()
	{
		var definition = Mock.Of<ICharacteristicDefinition>();
		var owner = Owner(definition);
		owner.Setup(x => x.GetObscurer(definition, It.IsAny<IPerceiver>()))
			.Returns(Mock.Of<IObscureCharacteristics>());
		Assert.AreEqual("red", owner.Object.ParseCharacteristics("$?colour[$colour[@][hidden]][missing]", null!, true));
		owner.Verify(x => x.GetObscurer(It.IsAny<ICharacteristicDefinition>(), It.IsAny<IPerceiver>()), Times.Never);
	}

	[TestMethod]
	[DataRow(0, "none")]
	[DataRow(1, "one")]
	[DataRow(2, "ordinary")]
	[DataRow(3, "many")]
	[DataRow(4, "many")]
	[DataRow(5, "ordinary")]
	public void ParseCharacteristics_BodypartAlternates_UseExactCountsAndInclusiveRanges(int count, string expected)
	{
		var shape = Mock.Of<IBodypartShape>();
		var definition = Mock.Of<IBodypartSpecificCharacteristicDefinition>(x => x.OrdinaryCount == 2 && x.TargetShape == shape);
		var body = new Mock<IBody>();
		body.Setup(x => x.ApparentGender(It.IsAny<IPerceiver>())).Returns(Gender);
		body.Setup(x => x.GetCharacteristicDefinition("eyes"))
			.Returns(Tuple.Create((ICharacteristicDefinition)definition, CharacteristicDescriptionType.Normal));
		body.Setup(x => x.Bodyparts).Returns(Enumerable.Range(0, count).Select(_ => Mock.Of<IBodypart>(x => x.Shape == shape)).ToArray());
		Assert.AreEqual(expected, body.Object.ParseCharacteristics("%eyes[ordinary][0:none][1:one][3-4:many]", null!));
		Assert.AreEqual("ordinary", body.Object.ParseCharacteristics("%eyes[ordinary][999999999999999999999:bad]", null!));
	}

	[TestMethod]
	[DataRow("&tattoos", "TattooSDescHeavilyInked")]
	[DataRow("&withtattoos", "TattooSDescHeavilyInkedWith")]
	[DataRow("&scars", "ScarSDescAdditionHeavilyScarred")]
	[DataRow("&withscars", "ScarSDescAdditionHeavilyScarredWith")]
	public void ParseCharacteristics_ManyChargenMarks_UsesHeavySummary(string input, string key)
	{
		var world = new Mock<IFuturemud>();
		world.Setup(x => x.GetStaticString(key)).Returns("heavy");
		var template = new Mock<ICharacterTemplate>();
		template.Setup(x => x.SelectedGender).Returns(MudSharp.Form.Shape.Gender.Male);
		template.Setup(x => x.Gameworld).Returns(world.Object);
		template.Setup(x => x.SelectedScars).Returns(Enumerable.Range(0, 11)
			.Select(_ => Mock.Of<IScar>(x => x.Bodypart == Mock.Of<IBodypart>())).ToList());
		template.Setup(x => x.SelectedDisfigurements).Returns(Enumerable.Range(0, 11)
			.Select(_ => ((IDisfigurementTemplate)Mock.Of<ITattooTemplate>(), Mock.Of<IBodypart>())).ToList());
		Assert.AreEqual("heavy ", template.Object.ParseCharacteristics(input, null!));
	}

	[TestMethod]
	public void ParseCharacteristics_PluralArticle_RendersInnerValueOnce()
	{
		var definition = Mock.Of<ICharacteristicDefinition>();
		var owner = Owner(definition);
		Assert.AreEqual("a red", owner.Object.ParseCharacteristics("&?a_an[$colour]", null!));
		owner.Verify(x => x.DescribeCharacteristic(definition, It.IsAny<IPerceiver>(), CharacteristicDescriptionType.Normal), Times.Once);
	}
}
