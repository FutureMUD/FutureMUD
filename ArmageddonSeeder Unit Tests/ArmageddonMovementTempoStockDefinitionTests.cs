#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonMovementTempoStockDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_RejectsMissingTraitOrInvalidEligibilityBinding()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonMovementTempoSpellContent.Create(0));
		foreach (var contribution in ArmageddonMovementTempoSpellContent.Create(42))
		{
			var content = contribution.Content;
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(1, 2, -1));
			if (content.EligibilitySource is not null)
				Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(1, 2, 0));
		}
	}
}
