using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#nullable enable
namespace MudSharp_Unit_Tests;

[TestClass]
public class ArmageddonSourceTreeTests
{
	[TestMethod]
	public void SourceTree_ExactRosterRootsCapsAndParentThresholds_ReconcileEveryCandidate()
	{
		var root = Path.Combine(VancianExampleProgTests.RepositoryRoot(), "Design Documents", "Magic");
		using var tree = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Armageddon_Sorcerer_Source_Tree.json")));
		using var repertoire = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Armageddon_Repertoire.json")));
		using var progress = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "Armageddon_Completion_Progress.json")));
		var rows = tree.RootElement.GetProperty("rows").EnumerateArray().ToArray();
		var spells = rows.Where(x => x.GetProperty("kind").GetString() == "spell").ToArray();
		Assert.AreEqual(94, rows.Length); Assert.AreEqual(82, spells.Length);
		Assert.AreEqual(12, rows.Length - spells.Length);
		var keys = rows.ToDictionary(x => x.GetProperty("key").GetString()!);
		CollectionAssert.AreEquivalent(new[] { "arm.spell.sense_enchantment", "arm.spell.unravel_enchantment", "arm.spell.gust_hands", "arm.spell.unyielding_veil" },
			spells.Where(x => x.GetProperty("parent_key").ValueKind == JsonValueKind.Null).Select(x => x.GetProperty("key").GetString()).ToArray());
		foreach (var row in rows)
		{
			if (row.GetProperty("parent_key").GetString() is { } parent)
				Assert.AreEqual(keys[parent].GetProperty("branches_at").GetInt32(), row.GetProperty("parent_threshold").GetInt32());
			if (row.GetProperty("kind").GetString() != "spell") continue;
			Assert.AreEqual(row.GetProperty("parent_key").ValueKind == JsonValueKind.Null ? 60 : 30, row.GetProperty("opening").GetInt32());
			Assert.AreEqual(row.GetProperty("key").GetString() == "arm.spell.mend_flesh" ? 60 : 90, row.GetProperty("raw_cap").GetInt32());
		}
		Assert.AreEqual("arm.support.component_crafting", keys["arm.spell.read_enchantment"].GetProperty("parent_key").GetString());
		Assert.AreEqual("arm.spell.shadow_passage", keys["arm.support.component_crafting"].GetProperty("parent_key").GetString());
		var entries = repertoire.RootElement.GetProperty("entries").EnumerateArray().ToArray();
		Assert.AreEqual(154, entries.Length);
		var admitted = entries.Where(x => x.GetProperty("memberships").EnumerateArray().Any(m => m.GetProperty("tradition").GetString() == "Sorcerer")).ToArray();
		CollectionAssert.AreEquivalent(spells.Select(x => x.GetProperty("key").GetString()).ToArray(), admitted.Select(x => x.GetProperty("key").GetString()).ToArray());
		foreach (var entry in admitted)
		{
			var row = keys[entry.GetProperty("key").GetString()!];
			var membership = entry.GetProperty("memberships").EnumerateArray().Single(m => m.GetProperty("tradition").GetString() == "Sorcerer");
			Assert.AreEqual(row.GetProperty("opening").GetInt32(), membership.GetProperty("opening_skill").GetInt32());
			Assert.AreEqual(row.GetProperty("raw_cap").GetInt32(), membership.GetProperty("raw_skill_cap").GetInt32());
			Assert.AreEqual(row.GetProperty("parent_key").ValueKind == JsonValueKind.Null, membership.GetProperty("starting_grant").GetBoolean());
		}
		Assert.IsTrue(entries.All(e => !e.GetProperty("portable_payload").GetProperty("scroll").GetBoolean()));
		CollectionAssert.AreEquivalent(entries.Select(x => x.GetProperty("key").GetString()).ToArray(),
			progress.RootElement.GetProperty("candidates").EnumerateArray().Select(x => x.GetProperty("key").GetString()).ToArray());
		Assert.AreEqual(25, progress.RootElement.GetProperty("native_acceptance").GetArrayLength());
		CollectionAssert.AreEqual(Enumerable.Range(0, 7).ToArray(), progress.RootElement.GetProperty("phases")
			.EnumerateArray().Select(x => x.GetProperty("id").GetInt32()).ToArray());
		Assert.AreEqual(16, progress.RootElement.GetProperty("decisions").GetArrayLength());
	}
}
