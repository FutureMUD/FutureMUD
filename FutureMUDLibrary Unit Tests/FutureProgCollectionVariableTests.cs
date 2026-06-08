using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class FutureProgCollectionVariableTests
{
	[TestMethod]
	public void TextCollection_NormalisesRawStringsToTextVariables()
	{
		CollectionVariable variable = new(new List<string> { "north", "east" }, ProgVariableTypes.Text);

		Assert.AreEqual("north", variable.GetProperty("first").GetObject);

		CollectionVariable reversed = (CollectionVariable)variable.GetProperty("reverse");

		Assert.AreEqual("east", reversed.GetProperty("first").GetObject);
	}
}
