#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonAreaHarmDefinitionTests
{

	[TestMethod]
	public void Create_RejectsMissingResistanceTrait()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonAreaHarmSpellContent.Create(0));
	}
}
