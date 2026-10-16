#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonNativeReserveContentTests
{
	[TestMethod]
	public void Contributions_LockedMinimumsExactModesAndTypedReserve_ExcludeCasterCreditOrAlternateVariants()
	{
		var all = ArmageddonNativeReserveSpellContent.Create(new(1234));
		CollectionAssert.AreEquivalent(new[] { ArmageddonNativeReserveSpellContent.SecondBreathKey, ArmageddonNativeReserveSpellContent.StealBreathKey,
			ArmageddonNativeReserveSpellContent.EmptyAuraKey }, all.Select(x => x.Content.Key).ToArray());
		foreach (var entry in all)
		{
			var expectedMinimum = entry.Content.Key == ArmageddonNativeReserveSpellContent.SecondBreathKey ? 20.0 : 7.0;
			Assert.AreEqual(expectedMinimum, entry.Content.MinimumEnergy);
			Assert.AreEqual(entry.Content.Key == ArmageddonNativeReserveSpellContent.EmptyAuraKey ? "arm.capability.void" : "arm.capability.lightning", entry.NativeCapabilityKey);
			var xml = entry.Content.BuildDefinition(10, 11, 0);
			Assert.AreEqual(expectedMinimum.ToString(System.Globalization.CultureInfo.InvariantCulture) + "*grade", entry.Content.CostRow(99).Expression);
			Assert.AreEqual("character", xml.Element("Trigger")!.Attribute("type")!.Value);
			Assert.AreEqual(0, xml.Element("CasterEffects")!.Elements().Count());
			Assert.IsNull(xml.Element("Area")); Assert.IsNull(xml.Element("Quiet")); Assert.AreEqual("0", entry.Content.DurationFormula);
			var effect = xml.Element("Effects")!.Elements().Single();
			if (entry.Content.Key == ArmageddonNativeReserveSpellContent.EmptyAuraKey)
			{
				Assert.AreEqual("1234", effect.Element("Resource")!.Value);
				Assert.AreEqual("-5*grade", effect.Element("Formula")!.Value); Assert.AreEqual(true, (bool)effect.Element("ExactResourceDepletion")!);
			}
			else
			{
				Assert.AreEqual(true, (bool)effect.Element("DirectStaminaAdjustment")!);
				Assert.AreEqual(entry.Content.Key == ArmageddonNativeReserveSpellContent.SecondBreathKey ? "5*grade" : "-5*grade", effect.Element("Formula")!.Value);
			}
		}
	}
	[DataTestMethod]
	[DataRow(1, 5.0)]
	[DataRow(7, 35.0)]
	public void ExternalGrade_Boundaries_ProduceFiniteApprovedBudgets(int grade, double expected) =>
		Assert.AreEqual(expected, ArmageddonNativeReserveSpellContent.AmountForGrade(grade));
	[DataTestMethod]
	[DataRow(0)]
	[DataRow(8)]
	[DataRow(int.MaxValue)]
	public void ExternalGrade_InvalidIndices_RejectInsteadOfInventingPower(int grade) =>
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonNativeReserveSpellContent.AmountForGrade(grade));
	[TestMethod]
	public void NativeBindings_MissingOrUnresolvedIds_RejectBeforeProducingDefinitions()
	{
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonNativeReserveSpellContent.Create(new(0)));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonNativeReserveSpellContent.EmptyAura(-1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonNativeReserveSpellContent.SecondBreath().Content.BuildDefinition(0, 11, 0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonNativeReserveSpellContent.StealBreath().Content.BuildDefinition(10, 0, 0));
	}
}
