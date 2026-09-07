#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseSeeder.Seeders.Utilities.Chargen;

namespace DatabaseSeeder_Unit_Tests;

[TestClass]
public class ChargenSkillSelectionGroupSeederTests
{
	[TestMethod]
	public void SG17_ThreeWayMerge_PreservesBuilderMembershipRemovalAdditionAndDescription()
	{
		var previous = new Dictionary<string, string> { ["description"] = "old", ["member:1"] = "0", ["member:2"] = "1" };
		var current = new Dictionary<string, string> { ["description"] = "builder", ["member:2"] = "1", ["member:9"] = "2" };
		var desired = new Dictionary<string, string> { ["description"] = "upstream", ["member:1"] = "1", ["member:2"] = "0", ["member:3"] = "2" };
		var conflicts = new List<string>();
		var result = ChargenSkillSelectionGroupSeeder.Merge(previous, current, desired, conflicts);
		Assert.AreEqual("builder", result["description"]); Assert.IsFalse(result.ContainsKey("member:1"));
		Assert.AreEqual("0", result["member:2"]); Assert.IsTrue(result.ContainsKey("member:9")); Assert.IsTrue(result.ContainsKey("member:3"));
		Assert.AreEqual(2, conflicts.Count);
		var repeated = ChargenSkillSelectionGroupSeeder.Merge(desired, result, desired, new List<string>());
		CollectionAssert.AreEquivalent(new List<string>(result.Keys), new List<string>(repeated.Keys));
	}
}
