#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNeedHarmDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_RejectsMissingExpression()
	{
		foreach (var contribution in ArmageddonNeedHarmSpellContent.Create())
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(1, 0, 0));
	}
}
