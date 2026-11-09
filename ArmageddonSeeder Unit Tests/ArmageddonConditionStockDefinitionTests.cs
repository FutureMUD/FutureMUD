#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonConditionStockDefinitionTests
{

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	[DataRow(1L)]
	[DataRow(4L)]
	public void DiseasePair_OnlyExactResolvedNativeS3KindIsAccepted(long invalidKind) =>
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonConditionSpellContent.Create(new(invalidKind, 102)));

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	public void DiseasePair_UnresolvedTraitsResourcesExpressionsRefuseWithoutFallback(long invalid)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonConditionSpellContent.Create(new(5, invalid)));
		foreach (var contribution in ArmageddonConditionSpellContent.Create(new(5, 102)))
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(invalid, 202, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, invalid, 0));
			if (invalid < 0) Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, 202, invalid));
		}
	}
}
