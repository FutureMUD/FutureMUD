using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Construction.Autobuilder.Areas;
using MudSharp.Framework;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AutobuilderFeatureMaskTests
{
	[TestMethod]
	public void Parse_ResolvesTagIdsInBottomLeftMaskOrder()
	{
		var road = Tag(41, "Road");
		var bridge = Tag(42, "Bridge");

		var result = AutobuilderFeatureMask.Parse("41|42,,42,41", 4, [road, bridge]);

		CollectionAssert.AreEqual(new long[] { 41, 42 }, result[0].Select(tag => tag.Id).ToArray());
		Assert.AreEqual(0, result[1].Length);
		CollectionAssert.AreEqual(new long[] { 42 }, result[2].Select(tag => tag.Id).ToArray());
		CollectionAssert.AreEqual(new long[] { 41 }, result[3].Select(tag => tag.Id).ToArray());
	}

	[DataTestMethod]
	[DataRow("road", "not a positive tag ID")]
	[DataRow("0", "not a positive tag ID")]
	[DataRow("99", "unknown tag ID 99")]
	public void TryParse_RejectsNamesNonPositiveIdsAndUnknownIds(string mask, string expectedError)
	{
		var parsed = AutobuilderFeatureMask.TryParse(mask, 1, [Tag(41, "Road")], out _, out var error);

		Assert.IsFalse(parsed);
		StringAssert.Contains(error, expectedError);
	}

	[TestMethod]
	public void TryParse_RejectsIncorrectCellCount()
	{
		var parsed = AutobuilderFeatureMask.TryParse("41,", 1, [Tag(41, "Road")], out _, out var error);

		Assert.IsFalse(parsed);
		Assert.AreEqual("The feature mask must exactly match the size of the grid.", error);
	}

	private static ITag Tag(long id, string name)
	{
		var tag = new Mock<ITag>();
		tag.SetupGet(x => x.Id).Returns(id);
		tag.SetupGet(x => x.Name).Returns(name);
		return tag.Object;
	}
}
