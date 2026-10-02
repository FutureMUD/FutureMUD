using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Magic;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ControlledSpellEfficiencyTests
{
	[DataTestMethod]
	[DataRow(1, 1, 7.0, 50.0)]
	[DataRow(2, 1, 7.0, 25.0)]
	[DataRow(3, 1, 7.0, 16.0)]
	[DataRow(7, 1, 7.0, 7.0)]
	[DataRow(7, 1, 20.0, 20.0)]
	[DataRow(1, 2, 7.0, 75.0)]
	[DataRow(1, 3, 7.0, 84.0)]
	[DataRow(1, 7, 7.0, 93.0)]
	[DataRow(7, 7, 50.0, 50.0)]
	public void Cost_SourceBranches_UseExternalGradesAndIntegerDivision(int mastery, int grade, double minimum, double expected)
	{
		Assert.AreEqual(expected, new ControlledSpellEfficiency(minimum, 1).Cost(mastery, grade));
		Assert.AreEqual(expected * 2, new ControlledSpellEfficiency(minimum, 2).Cost(mastery, grade));
	}

	[TestMethod]
	public void Cost_InvalidBoundsAndOverflow_Refuses()
	{
		foreach (var profile in new[] { new ControlledSpellEfficiency(-1, 1), new ControlledSpellEfficiency(51, 1),
			new ControlledSpellEfficiency(double.NaN, 1), new ControlledSpellEfficiency(7, 0), new ControlledSpellEfficiency(7, double.PositiveInfinity) })
			Assert.ThrowsException<InvalidOperationException>(() => profile.Cost(1, 1));
		Assert.ThrowsException<InvalidOperationException>(() => new ControlledSpellEfficiency(7, double.MaxValue).Cost(1, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ControlledSpellEfficiency(7, 1).Cost(0, 1));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => new ControlledSpellEfficiency(7, 1).Cost(1, 8));
	}

	[TestMethod]
	public void PowerWords_AllSevenCoordinates_RoundTripWithoutEnumIntegerCasts()
	{
		var words = new[] { "wek", "yuqa", "kral", "een", "pav", "sul", "mon" };
		var powers = new[] { SpellPower.ExtremelyWeak, SpellPower.VeryWeak, SpellPower.Weak, SpellPower.Standard,
			SpellPower.Strong, SpellPower.VeryStrong, SpellPower.ExtremelyStrong };
		for (var i = 0; i < words.Length; i++)
		{
			Assert.IsTrue(ArmageddonPowerVocabulary.TryParse(words[i].ToUpperInvariant(), out var grade));
			Assert.AreEqual(i + 1, grade);
			Assert.AreEqual(i, ArmageddonPowerVocabulary.SourceIndex(grade));
			Assert.AreEqual(grade, ArmageddonPowerVocabulary.Grade(i));
			Assert.AreEqual(words[i], ArmageddonPowerVocabulary.Word(grade));
			Assert.AreEqual(powers[i], ArmageddonPowerVocabulary.NativePower(grade));
		}
		Assert.IsFalse(ArmageddonPowerVocabulary.TryParse("first", out _));
		Assert.IsFalse(ArmageddonPowerVocabulary.TryParse(null, out _));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonPowerVocabulary.SourceIndex(0));
		Assert.ThrowsException<ArgumentOutOfRangeException>(() => ArmageddonPowerVocabulary.Grade(7));
	}
}
