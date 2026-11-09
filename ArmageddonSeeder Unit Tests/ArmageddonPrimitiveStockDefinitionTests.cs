#nullable enable
using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonPrimitiveStockDefinitionTests
{
	private static ArmageddonPrimitiveSpellBindings Bindings => new(101, 102, 103, 104, Difficulty.Normal);

	[TestMethod]
	public void FalsePresence_PropagatesViewerPolicyAndRejectsMissingIdentity()
	{
		var definition = ArmageddonPrimitiveSpellContent.FalsePresence(103).Content.BuildDefinition(201, 202, 0);
		Assert.AreEqual(103L, (long)definition.Element("Effects")!.Elements().Single().Element("FilterProg")!);
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.FalsePresence(0));
	}

	[TestMethod]
	public void Create_ExactDependenciesAndResistanceAreExplicitAndDefinitionsAreIndependent()
	{
		var contents = ArmageddonPrimitiveSpellContent.Create(Bindings).ToDictionary(x => x.Content.Key, x => x.Content);
		foreach (var key in new[] { "arm.spell.earthen_strength", "arm.spell.sapping_weight" })
		{
			var definition = contents[key].BuildDefinition(201, 202, 203);
			Assert.AreEqual(101L, (long)definition.Element("Effects")!.Elements().Single().Attribute("trait")!);
			definition.Element("Effects")!.RemoveNodes();
			Assert.AreEqual(1, contents[key].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Count());
		}
		Assert.AreEqual(103L, (long)contents["arm.spell.veil_from_sight"].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Single().Element("FilterProg")!);
		Assert.AreEqual(104L, (long)contents["arm.spell.empty_aura"].BuildDefinition(201, 202, 203).Element("Effects")!.Elements().Single().Element("Resource")!);
		Assert.AreEqual(102L, contents["arm.spell.still_limbs"].SpellRow(401, 402, 403).ResistingTraitDefinitionId);
		Assert.AreEqual((int)Difficulty.Normal, contents["arm.spell.still_limbs"].SpellRow(401, 402, 403).ResistingDifficulty);
	}

	[TestMethod]
	[DataRow(0L)]
	[DataRow(-1L)]
	public void Create_UnresolvedNativeBindingsRefuseWithoutFallback(long invalid)
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { StrengthTraitId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { ResistanceTraitId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { InvisibilityApplicabilityProgId = invalid }));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => ArmageddonPrimitiveSpellContent.Create(Bindings with { EmptyAuraResourceId = invalid }));
		foreach (var contribution in ArmageddonPrimitiveSpellContent.Create(Bindings))
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(invalid, 202, 203));
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, invalid, 203));
			if (invalid < 0)
				Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => contribution.Content.BuildDefinition(201, 202, invalid));
		}
	}

	[TestMethod]
	public void Create_NoEligibilitySourceUsesNativeZeroFilterWithoutSupportProg()
	{
		foreach (var contribution in ArmageddonPrimitiveSpellContent.Create(Bindings))
		{
			Assert.IsNull(contribution.Content.EligibilitySource);
			Assert.IsNull(contribution.Content.EligibilityRow(300));
			var definition = contribution.Content.BuildDefinition(201, 202, 0);
			Assert.AreEqual(0L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!, contribution.Content.Key);
		}
	}
}
