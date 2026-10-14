#nullable enable

using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Form.Material;
using MudSharp.Health;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonItemConditionDefinitionTests
{
	[TestMethod]
	public void ItemCondition_FourFiniteSelectedGradeDefinitions_KeepDistinctMaterialAndRepairPolicies()
	{
		var contributions = ArmageddonItemConditionSpellContent.Create().ToDictionary(x => x.Content.Key);
		Assert.AreEqual(4, contributions.Count);
		foreach (var (key, contribution) in contributions)
		{
			var content = contribution.Content;
			var definition = content.BuildDefinition(10, 11, 0);
			Assert.AreEqual("item", (string?)definition.Element("Trigger")!.Attribute("type"), key);
			var effect = definition.Element("Effects")!.Elements().Single();
			Assert.AreEqual("boundeditemcondition", (string?)effect.Attribute("type"), key);
			Assert.AreEqual(5.0, (double)effect.Element("BudgetPerGrade")!, key);
			Assert.AreEqual("0", content.DurationFormula, key);
		}
		var stone = contributions[ArmageddonItemConditionSpellContent.ShatterStoneKey];
		Assert.AreEqual("arm.capability.earth", stone.NativeCapabilityKey);
		Assert.AreEqual(6.0, stone.Content.MinimumEnergy);
		var shatter = stone.Content.BuildDefinition(10, 11, 0).Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("damage", (string?)shatter.Element("Mode"));
		Assert.AreEqual((int)MaterialBehaviourType.Stone, (int)shatter.Element("MaterialBehaviour")!);
		Assert.AreEqual((int)DamageType.Crushing, (int)shatter.Element("DamageType")!);
		var repair = contributions[ArmageddonItemConditionSpellContent.RestoreObjectKey];
		Assert.AreEqual("arm.capability.earth", repair.NativeCapabilityKey);
		Assert.AreEqual(7.0, repair.Content.MinimumEnergy);
		var restore = repair.Content.BuildDefinition(10, 11, 0).Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("repair", (string?)restore.Element("Mode"));
		Assert.AreEqual(-1, (int)restore.Element("MaterialBehaviour")!);
		var erode = contributions[ArmageddonItemConditionSpellContent.ErodeObjectKey];
		Assert.AreEqual("arm.capability.void", erode.NativeCapabilityKey);
		Assert.AreEqual(7.0, erode.Content.MinimumEnergy);
		var erosion = erode.Content.BuildDefinition(10, 11, 0).Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("damage", (string?)erosion.Element("Mode"));
		Assert.AreEqual("true", (string?)erosion.Element("OrganicOnly"));
		Assert.AreEqual(-1, (int)erosion.Element("MaterialBehaviour")!);
		Assert.AreEqual((int)DamageType.Chemical, (int)erosion.Element("DamageType")!);
		var wither = contributions[ArmageddonItemConditionSpellContent.WitherGrowthKey];
		Assert.AreEqual("arm.capability.water", wither.NativeCapabilityKey);
		Assert.AreEqual(8.0, wither.Content.MinimumEnergy);
		var plant = wither.Content.BuildDefinition(10, 11, 0).Element("Effects")!.Element("Effect")!;
		Assert.AreEqual("damage", (string?)plant.Element("Mode"));
		Assert.AreEqual((int)MaterialBehaviourType.Plant, (int)plant.Element("MaterialBehaviour")!);
		Assert.AreEqual("true", (string?)plant.Element("OrganicOnly"));
		Assert.AreEqual((int)DamageType.Chemical, (int)plant.Element("DamageType")!);
	}
}
