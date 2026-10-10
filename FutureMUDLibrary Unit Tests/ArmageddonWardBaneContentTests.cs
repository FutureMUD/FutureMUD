#nullable enable

using System;
using System.Linq;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonWardBaneContentTests
{
	[TestMethod]
	public void RefugeJournal_V2PreservesSelectors_V1KeepsItsOriginalWireForm()
	{
		var legacy = new SpellShelterAnchor(SpellShelterKind.SandShelter, 1, 2, null, 8, 3);
		Assert.AreEqual("1", (string?)XElement.Parse(legacy.Save()).Attribute("version"));
		Assert.AreEqual(legacy.Save(), SpellShelterAnchor.Load(legacy.Save()).Save());
		var refuge = legacy with { Kind = SpellShelterKind.SeveringRefuge,
			Ward = new([long.MaxValue], ["fire", "void"], MagicInterdictionCoverage.Both) };
		Assert.AreEqual("2", (string?)XElement.Parse(refuge.Save()).Attribute("version"));
		Assert.AreEqual(refuge.Save(), SpellShelterAnchor.Load(refuge.Save()).Save());
		var downgrade = XElement.Parse(refuge.Save()); downgrade.SetAttributeValue("version", 1);
		Assert.ThrowsException<FormatException>(() => SpellShelterAnchor.Load(downgrade.ToString()));
		Assert.ThrowsException<ArgumentException>(() => (refuge with { Ward = null }).Save());
		Assert.ThrowsException<ArgumentException>(() => (legacy with { Ward = refuge.Ward }).Save());
	}

	[DataTestMethod]
	[DataRow("<Ward version='1'><Coverage>Both</Coverage><IncludesSubschools>true</IncludesSubschools><Room>99</Room></Ward>")]
	[DataRow("<Ward version='1'><Coverage>Both</Coverage><Coverage>Both</Coverage><IncludesSubschools>true</IncludesSubschools><School>1</School></Ward>")]
	[DataRow("<Ward version='2' />")]
	public void WardRejectsUnknownAndAmbiguousSchemas(string xml) =>
		Assert.ThrowsException<FormatException>(() => SpellShelterWardConfiguration.Load(XElement.Parse(xml)));

	[TestMethod]
	public void WardRejectsEmptyDuplicateOrInvalidSelectors()
	{
		foreach (var ward in new[] { new SpellShelterWardConfiguration([], [], MagicInterdictionCoverage.Both),
			new([0], [], MagicInterdictionCoverage.Both), new([1, 1], [], MagicInterdictionCoverage.Both),
			new([], ["fire", "FIRE"], MagicInterdictionCoverage.Both), new([], [" "], MagicInterdictionCoverage.Both),
			new([1], [], (MagicInterdictionCoverage)99) })
			Assert.ThrowsException<ArgumentException>(() => ward.Save());
	}

	[TestMethod]
	public void BaneContribution_BindsEligibilityNativeResistanceAndCappedDamage()
	{
		var content = ArmageddonBaneContent.Create(long.MaxValue, Difficulty.Hard, 8, 40, DamageType.Arcane);
		var row = content.SpellRow(1, 2, 3);
		Assert.AreEqual(long.MaxValue, row.ResistingTraitDefinitionId); Assert.AreEqual((int)Difficulty.Hard, row.ResistingDifficulty);
		Assert.IsFalse(row.ScrollInscriptionAllowed);
		Assert.AreEqual("12*grade", content.CostRow(1).Expression);
		var definition = content.BuildDefinition(4, 5, 6);
		Assert.AreEqual(6L, (long)definition.Element("Trigger")!.Element("TargetFilterProg")!);
		Assert.AreEqual("min(40,8*grade)", definition.Descendants("DamageExpression").Single().Value);
		Assert.AreEqual(ArmageddonBaneContent.Key, definition.Element("StockIdentity")!.Value);
	}

	[DataTestMethod]
	[DataRow(0.0, 1.0)][DataRow(-1.0, 1.0)][DataRow(double.NaN, 1.0)]
	[DataRow(double.PositiveInfinity, 1.0)][DataRow(1.0, 0.0)][DataRow(1.0, double.PositiveInfinity)]
	public void BaneRejectsUnboundedOrNonPositiveDamage(double amount, double maximum) =>
		Assert.ThrowsException<ArgumentException>(() => ArmageddonBaneContent.Create(1, Difficulty.Normal, amount, maximum, DamageType.Arcane));
}
