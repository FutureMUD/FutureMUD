#nullable enable

using System.Collections.Generic;
using DatabaseSeeder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Models;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SeederManagedRecordReconcilerTests
{
	[TestMethod]
	public void UnbaselinedExistingValuesAndRemovedMembersAreNeverOverwritten()
	{
		var record = new SeederManagedRecord { EntityType = "NameProfile", StableKey = "source:local-profile" };
		var actual = new Dictionary<string, string> { ["name"] = "Builder name", ["element:local"] = "7" };
		var desired = new Dictionary<string, string> { ["name"] = "Stock name", ["element:stock"] = "1" };
		var conflicts = new List<string>();
		var first = SeederManagedRecordReconciler.Reconcile(record, actual, desired, false, conflicts);
		Assert.AreEqual("Builder name", first["name"]);
		Assert.AreEqual("7", first["element:local"]);
		Assert.IsFalse(first.ContainsKey("element:stock"));
		Assert.AreEqual(2, conflicts.Count);
		desired["name"] = "Revised stock name";
		desired["element:stock"] = "2";
		var second = SeederManagedRecordReconciler.Reconcile(record, first, desired, false, conflicts);
		Assert.AreEqual("Builder name", second["name"]);
		Assert.IsFalse(second.ContainsKey("element:stock"));
	}

	[TestMethod]
	public void ThreeWayMergeUpdatesStockAndRetainsIndependentEditsAcrossThreeRuns()
	{
		var record = new SeederManagedRecord { EntityType = "Language", StableKey = "english.old" };
		var desired = new Dictionary<string, string> { ["name"] = "Old English", ["description"] = "Original", ["obsolete"] = "stock" };
		var conflicts = new List<string>();
		var first = SeederManagedRecordReconciler.Reconcile(record, new Dictionary<string, string>(), desired, true, conflicts);
		var actual = new Dictionary<string, string>(first) { ["description"] = "Builder prose", ["extra"] = "local" };
		desired["name"] = "Saxon";
		desired["description"] = "Revised stock";
		desired.Remove("obsolete");
		desired["new"] = "stock addition";
		var second = SeederManagedRecordReconciler.Reconcile(record, actual, desired, false, conflicts);
		Assert.AreEqual("Saxon", second["name"]);
		Assert.AreEqual("Builder prose", second["description"]);
		Assert.AreEqual("local", second["extra"]);
		Assert.AreEqual("stock addition", second["new"]);
		Assert.IsFalse(second.ContainsKey("obsolete"));
		var third = SeederManagedRecordReconciler.Reconcile(record, second, desired, false, conflicts);
		Assert.AreEqual("Builder prose", third["description"]);
		Assert.AreEqual(64, record.AppliedFingerprint.Length);
		Assert.AreEqual(2, conflicts.Count);
	}
}
