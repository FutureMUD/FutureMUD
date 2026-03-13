using DatabaseSeeder.Seeders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CombatSeederMessageStyleHelperTests
{
	[TestMethod]
	public void FormatAttackMessage_SentenceStyle_AddsSingleTerminalFullStop()
	{
		const string raw = "@ plunge|plunges $2 deep into $1's throat";

		Assert.AreEqual(
			$"{raw}.",
			CombatSeederMessageStyleHelper.FormatAttackMessage(raw, SeedCombatMessageStyle.Sentences));
		Assert.AreEqual(
			$"{raw}.",
			CombatSeederMessageStyleHelper.FormatAttackMessage($"{raw}.", SeedCombatMessageStyle.Sentences));
	}

	[TestMethod]
	public void BuildDefenseFailure_CompactStyle_UsesInlineJoiner()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Compact,
			"#1 %1|attempt|attempts to dodge out of the way",
			"hit on &1's {1}");

		Assert.AreEqual(
			", and #1 %1|attempt|attempts to dodge out of the way but %1|get|gets hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void BuildDefenseFailure_SparseStyle_UsesSeparateHitLine()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Sparse,
			"#1 %1|attempt|attempts to parry with $3",
			"hit on &1's {1}");

		Assert.AreEqual(
			".\n#1 %1|attempt|attempts to parry with $3\n#1 %1|get|gets hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void BuildDefenseFailure_BeHitCompactStyle_UsesAndIsHitJoiner()
	{
		var message = CombatSeederMessageStyleHelper.BuildDefenseFailure(
			SeedCombatMessageStyle.Compact,
			"#1 %1|offer|offers no defense",
			"hit on &1's {1}",
			SeedCombatHitVerb.BeHit);

		Assert.AreEqual(
			", and #1 %1|offer|offers no defense and %1|are|is hit on &1's {1}",
			message);
	}

	[TestMethod]
	public void RepresentativeComposition_NaturalAndRangedMessages_AreGrammatical()
	{
		var natural = CombatSeederMessageStyleHelper.FormatAttackMessage(
			"@ rake|rakes &0's {0} across $1 with @hand precision",
			SeedCombatMessageStyle.Sentences);
		var renderedNatural = string.Format(natural, "claws").Replace("@hand", "left");
		Assert.AreEqual("@ rake|rakes &0's claws across $1 with left precision.", renderedNatural);

		var ranged = string.Format(
			CombatSeederMessageStyleHelper.FormatStandaloneMessage("@ stand|stands and {0} $2 at $1"),
			"fire|fires");
		Assert.AreEqual("@ stand|stands and fire|fires $2 at $1.", ranged);
	}
}
