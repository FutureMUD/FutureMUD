using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Helpers;
using MudSharp.Commands.Modules;
using MudSharp.Framework.Revision;
using MudSharp.GameItems;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class PrototypeUniqueNameBulkRenameTests
{
	[TestMethod]
	public void CreatePlan_NumberedAndNamedGroups_ProducesExpectedNames()
	{
		var prototypes = new[]
		{
			Proto(1, "antiquity_sword"),
			Proto(2, "medieval_shield")
		};

		var plan = CreatePlan(
			prototypes,
			@"^(?<era>[^_]+)_(.+)$",
			@"${era}_historic_$1");

		Assert.IsTrue(plan.IsValid);
		CollectionAssert.AreEqual(
			new[] { "antiquity_historic_sword", "medieval_historic_shield" },
			plan.Entries.Select(x => x.NewName).ToArray());
	}

	[TestMethod]
	public void CreatePlan_DefaultCaseSensitiveAndInlineIgnoreCase_UsesDotNetOptions()
	{
		var prototypes = new[] { Proto(1, "Antiquity_Sword") };

		var caseSensitive = CreatePlan(prototypes, "^antiquity_", "historic_");
		var ignoreCase = CreatePlan(prototypes, "(?i)^antiquity_", "historic_");

		Assert.AreEqual(0, caseSensitive.Entries.Count);
		Assert.AreEqual("historic_Sword", ignoreCase.Entries.Single().NewName);
	}

	[TestMethod]
	public void CreatePlan_ActiveRevisionScope_IgnoresHistoricalAndBlankNames()
	{
		var prototypes = new[]
		{
			Proto(1, "current", RevisionStatus.Current),
			Proto(2, "pending", RevisionStatus.PendingRevision),
			Proto(3, "design", RevisionStatus.UnderDesign),
			Proto(4, "revised", RevisionStatus.Revised),
			Proto(5, "rejected", RevisionStatus.Rejected),
			Proto(6, "obsolete", RevisionStatus.Obsolete),
			Proto(7, null, RevisionStatus.Current)
		};

		var plan = CreatePlan(prototypes, "^(.*)$", "new_$1");

		Assert.IsTrue(plan.IsValid);
		CollectionAssert.AreEqual(
			new long[] { 1, 2, 3 },
			plan.Entries.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void CreatePlan_EmptyReplacement_ClearsMatchedNames()
	{
		var plan = CreatePlan(new[] { Proto(1, "clear_me") }, "^clear_me$", "");

		Assert.IsTrue(plan.IsValid);
		Assert.IsNull(plan.Entries.Single().NewName);
	}

	[TestMethod]
	public void CreatePlan_NoMatchAndNoOpReplacement_ProduceNoChanges()
	{
		var prototype = Proto(1, "unchanged");
		var noMatch = CreatePlan(new[] { prototype }, "^other$", "replacement");
		var noOp = CreatePlan(new[] { prototype }, "^(.*)$", "$1");
		var applied = new List<string?>();

		Assert.AreEqual(0, noMatch.Entries.Count);
		Assert.IsTrue(noOp.TryApply((_, name) => applied.Add(name), out var changedCount));
		Assert.AreEqual(0, changedCount);
		Assert.AreEqual(0, applied.Count);
	}

	[TestMethod]
	public void CreatePlan_InvalidRegexAndReplacement_ReturnValidationErrors()
	{
		var prototype = Proto(1, "old_name");

		var invalidRegex = CreatePlan(new[] { prototype }, "(", "new");
		var invalidReplacement = CreatePlan(new[] { prototype }, "^(?<name>.*)$", "${missing}");

		Assert.IsFalse(invalidRegex.IsValid);
		StringAssert.Contains(invalidRegex.Errors.Single(), "regular expression is invalid");
		Assert.IsFalse(invalidReplacement.IsValid);
		StringAssert.Contains(invalidReplacement.Errors.Single(), "unknown regex group");
	}

	[TestMethod]
	public void CreatePlan_RegexTimeout_ReturnsValidationError()
	{
		var prototype = Proto(1, new string('a', 100_000) + "!");

		var plan = CreatePlan(
			new[] { prototype },
			"^(a+)+$",
			"replacement",
			TimeSpan.FromMilliseconds(1));

		Assert.IsFalse(plan.IsValid);
		Assert.IsTrue(plan.Errors.Any(x => x.Contains("timeout", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void CreatePlan_NumericResult_ReturnsValidationErrorAndDoesNotApply()
	{
		var prototype = Proto(1, "item_123");
		var plan = CreatePlan(new[] { prototype }, "^item_(.*)$", "$1");
		var mutationCount = 0;

		var applied = plan.TryApply((_, _) => mutationCount++, out var changedCount);

		Assert.IsFalse(plan.IsValid);
		Assert.IsFalse(applied);
		Assert.AreEqual(0, changedCount);
		Assert.AreEqual(0, mutationCount);
		StringAssert.Contains(plan.Errors.Single(), "entirely numeric");
	}

	[TestMethod]
	public void CreatePlan_CollisionWithUntouchedPrototype_ReturnsValidationError()
	{
		var prototypes = new[]
		{
			Proto(1, "old_name"),
			Proto(2, "existing_name")
		};

		var plan = CreatePlan(prototypes, "^old_name$", "existing_name");

		Assert.IsFalse(plan.IsValid);
		StringAssert.Contains(plan.Errors.Single(), "distinct prototype IDs");
	}

	[TestMethod]
	public void CreatePlan_IntraBatchCaseInsensitiveCollision_ReturnsValidationError()
	{
		var prototypes = new[]
		{
			Proto(1, "old_one"),
			Proto(2, "old_two")
		};

		var plan = CreatePlan(prototypes, "^old_.*$", "Shared_Name");
		var lowerCasePlan = CreatePlan(
			new[] { Proto(1, "old_name"), Proto(2, "shared_name") },
			"^old_name$",
			"SHARED_NAME");

		Assert.IsFalse(plan.IsValid);
		Assert.IsFalse(lowerCasePlan.IsValid);
	}

	[TestMethod]
	public void CreatePlan_SwapsAndRenameChains_ValidateAgainstFinalState()
	{
		var swapPlan = CreatePlan(
			new[] { Proto(1, "left_right"), Proto(2, "right_left") },
			@"^(?<left>[^_]+)_(?<right>[^_]+)$",
			"${right}_${left}");
		var chainPlan = CreatePlan(
			new[] { Proto(1, "a"), Proto(2, "new_a") },
			"^(.*)$",
			"new_$1");

		Assert.IsTrue(swapPlan.IsValid);
		Assert.IsTrue(chainPlan.IsValid);
	}

	[TestMethod]
	public void CreatePlan_SamePrototypeIdAcrossRevisions_MayShareFinalName()
	{
		var prototypes = new[]
		{
			Proto(1, "old_current", RevisionStatus.Current, revision: 1),
			Proto(1, "old_design", RevisionStatus.UnderDesign, revision: 2)
		};

		var plan = CreatePlan(prototypes, "^old_.*$", "shared_name");

		Assert.IsTrue(plan.IsValid);
		Assert.AreEqual(2, plan.Entries.Count);
	}

	[TestMethod]
	public void TryApply_ValidPlan_AppliesOnlyChangedEntriesAfterPlanning()
	{
		var prototypes = new[]
		{
			Proto(1, "old_one"),
			Proto(2, "old_two")
		};
		var plan = CreatePlan(prototypes, "^old_(.*)$", "new_$1");
		var applied = new Dictionary<long, string?>();

		var result = plan.TryApply((prototype, name) => applied[prototype.Id] = name, out var changedCount);

		Assert.IsTrue(result);
		Assert.AreEqual(2, changedCount);
		Assert.AreEqual("new_one", applied[1]);
		Assert.AreEqual("new_two", applied[2]);
	}

	[TestMethod]
	public void BuilderHelp_ListsItemAndNpcBulkRenameCommands()
	{
		StringAssert.Contains(ItemBuilderModule.ItemHelp, "item rename <match regex> <replacement text>");
		StringAssert.Contains(NPCBuilderModule.NPCHelp, "npc rename <match regex> <replacement text>");
	}

	private static PrototypeUniqueNameRenamePlan<TestPrototype> CreatePlan(
		IEnumerable<TestPrototype> prototypes,
		string pattern,
		string replacement,
		TimeSpan? timeout = null)
	{
		return PrototypeUniqueNameBulkRenamePlanner.CreatePlan(
			prototypes,
			pattern,
			replacement,
			x => x.Id,
			x => x.Revision,
			x => x.Status,
			x => x.UniqueName,
			GameItemProtoLookupExtensions.NormaliseUniqueName,
			GameItemProtoLookupExtensions.IsValidUniqueName,
			timeout);
	}

	private static TestPrototype Proto(
		long id,
		string? uniqueName,
		RevisionStatus status = RevisionStatus.Current,
		int revision = 1)
	{
		return new TestPrototype(id, revision, status, uniqueName);
	}

	private sealed record TestPrototype(
		long Id,
		int Revision,
		RevisionStatus Status,
		string? UniqueName);
}
