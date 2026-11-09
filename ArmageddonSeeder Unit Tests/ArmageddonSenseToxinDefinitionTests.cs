#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSenseToxinDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_RejectsMissingResourceOrExpression()
	{
		var content = ArmageddonSenseToxinSpellContent.Create().Content;
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(0, 11, 0));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(10, 0, 0));
	}
}
