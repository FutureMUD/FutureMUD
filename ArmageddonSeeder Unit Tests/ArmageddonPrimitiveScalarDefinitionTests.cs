#nullable enable
using System;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveScalarDefinitionTests
{

	[TestMethod]
	public void BuildDefinition_RejectsInvalidIdentitiesAndRoomCharacterFilters()
	{
		foreach (var contribution in ArmageddonPrimitiveSpellContent.CreateScalarContributions())
		{
			var content = contribution.Content;
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(0, 202, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(201, 0, 0));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => content.BuildDefinition(201, 202, -1));
			var definition = content.BuildDefinition(201, 202, 0);
			if ((string?)definition.Element("Trigger")!.Attribute("type") == "room")
				Assert.ThrowsExactly<ArgumentException>(() => content.BuildDefinition(201, 202, 203));
		}
	}
}
