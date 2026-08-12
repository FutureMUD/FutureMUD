using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Construction;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CellSurfaceLiquidDescriptionTests
{
	[TestMethod]
	public void DescribeSurfaceLiquidRoomLine_LowercaseNounPhrase_SentenceCasesLine()
	{
		Assert.AreEqual(
			"A small puddle of a mixture of blood and water is here.",
			Cell.DescribeSurfaceLiquidRoomLine("small puddle", "a mixture of blood and water"));
	}
}
