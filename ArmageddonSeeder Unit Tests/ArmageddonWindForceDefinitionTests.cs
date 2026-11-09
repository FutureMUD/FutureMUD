#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWindForceDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_RejectsMissingResource()
	{
		foreach (var contribution in ArmageddonWindForceSpellContent.Create())
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(0, 2, 0));
	}
}
