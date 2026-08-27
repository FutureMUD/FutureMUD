using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Commands.Helpers;
using MudSharp.Framework.Revision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable

namespace MudSharp_Unit_Tests;

[TestClass]
public class EditableItemNameBulkRenameTests
{
	[TestMethod]
	public void CreatePlan_NumberedAndNamedGroups_ProducesExpectedNames()
	{
		var plan = CreatePlan(
			new[] { Item(1, "Old Sword"), Item(2, "Old Shield") },
			@"^Old (?<name>.+)$",
			@"New ${name}");

		Assert.IsTrue(plan.IsValid);
		CollectionAssert.AreEqual(
			new[] { "New Sword", "New Shield" },
			plan.Entries.Select(x => x.NewName).ToArray());
	}

	[TestMethod]
	public void CreatePlan_IsCaseSensitiveUnlessThePatternSuppliesAnInlineOption()
	{
		var caseSensitive = CreatePlan(new[] { Item(1, "Old Sword") }, "^old", "new");
		var caseInsensitive = CreatePlan(new[] { Item(1, "Old Sword") }, "(?i)^old", "new");

		Assert.IsTrue(caseSensitive.IsValid);
		Assert.AreEqual(0, caseSensitive.Entries.Count);
		Assert.IsTrue(caseInsensitive.IsValid);
		Assert.AreEqual("new Sword", caseInsensitive.Entries.Single().NewName);
	}

	[TestMethod]
	public void CreatePlan_TrimsAndRejectsBlankOrNumericFinalNames()
	{
		var trimmed = CreatePlan(new[] { Item(1, "old") }, "old", "  replacement  ");
		var blank = CreatePlan(new[] { Item(1, "old") }, "old", "   ");
		var numeric = CreatePlan(new[] { Item(1, "old") }, "old", "12345");

		Assert.IsTrue(trimmed.IsValid);
		Assert.AreEqual("replacement", trimmed.Entries.Single().NewName);
		Assert.IsFalse(blank.IsValid);
		Assert.IsFalse(numeric.IsValid);
		Assert.AreEqual(1, blank.Entries.Count);
		Assert.AreEqual("   ", blank.Entries.Single().NewName);
		Assert.AreEqual(1, numeric.Entries.Count);
		Assert.AreEqual("12345", numeric.Entries.Single().NewName);
		StringAssert.Contains(blank.Errors.Single(), "blank");
		StringAssert.Contains(numeric.Errors.Single(), "numeric");
	}

	[TestMethod]
	public void CreatePlan_RejectsInvalidExpressionsAndRegexTimeouts()
	{
		var invalidRegex = CreatePlan(new[] { Item(1, "name") }, "(", "replacement");
		var invalidReplacement = CreatePlan(new[] { Item(1, "name") }, "^(?<name>.*)$", "${missing}");
		var timeout = EditableItemNameBulkRenamePlanner.CreatePlan(
			new[] { Item(1, new string('a', 100_000) + "!") },
			"^(a+)+$",
			"replacement",
			x => x.Id,
			x => x.Revision,
			x => x.Status,
			x => x.Name,
			x => x.Scope,
			(_, name) => Normalise(name),
			TimeSpan.FromMilliseconds(1));

		Assert.IsFalse(invalidRegex.IsValid);
		Assert.IsFalse(invalidReplacement.IsValid);
		Assert.IsFalse(timeout.IsValid);
		StringAssert.Contains(invalidRegex.Errors.Single(), "regular expression is invalid");
		StringAssert.Contains(invalidReplacement.Errors.Single(), "unknown regex group");
		Assert.IsTrue(timeout.Errors.Any(x => x.Contains("timeout", StringComparison.InvariantCultureIgnoreCase)));
	}

	[TestMethod]
	public void CreatePlan_ValidatesTheFinalStateForUntouchedCollisionsSwapsAndChains()
	{
		var collision = CreatePlan(
			new[] { Item(1, "old"), Item(2, "existing") },
			"old",
			"existing");
		var swap = CreatePlan(
			new[] { Item(1, "left_right"), Item(2, "right_left") },
			@"^(?<left>[^_]+)_(?<right>[^_]+)$",
			"${right}_${left}");
		var chain = CreatePlan(
			new[] { Item(1, "a"), Item(2, "new_a") },
			"^(.*)$",
			"new_$1");

		Assert.IsFalse(collision.IsValid);
		Assert.IsTrue(swap.IsValid);
		Assert.IsTrue(chain.IsValid);
	}

	[TestMethod]
	public void CreatePlan_AllowsNoMatchAndNoOpDrafts()
	{
		var noMatch = CreatePlan(new[] { Item(1, "one") }, "missing", "replacement");
		var noOp = CreatePlan(new[] { Item(1, "one") }, "one", "one");

		Assert.IsTrue(noMatch.IsValid);
		Assert.AreEqual(0, noMatch.Entries.Count);
		Assert.IsTrue(noOp.IsValid);
		Assert.AreEqual(1, noOp.Entries.Count);
		Assert.IsFalse(noOp.Entries.Single().ChangesName);
	}

	[TestMethod]
	public void CreatePlan_UsesNaturalScopesForCollisionChecks()
	{
		var permitted = CreatePlan(
			new[] { Item(1, "one", scope: "school-a"), Item(2, "two", scope: "school-b") },
			"^(one|two)$",
			"shared");
		var rejected = CreatePlan(
			new[] { Item(1, "one", scope: "school-a"), Item(2, "two", scope: "school-a") },
			"^(one|two)$",
			"shared");

		Assert.IsTrue(permitted.IsValid);
		Assert.IsFalse(rejected.IsValid);
	}

	[TestMethod]
	public void CreatePlan_RevisableItemsIgnoreHistoryAndAllowSameIdAcrossActiveRevisions()
	{
		var plan = CreatePlan(
			new[]
			{
				Item(1, "current", revision: 1, status: RevisionStatus.Current),
				Item(1, "design", revision: 2, status: RevisionStatus.UnderDesign),
				Item(2, "historical", revision: 1, status: RevisionStatus.Revised)
			},
			"^(current|design)$",
			"shared");

		Assert.IsTrue(plan.IsValid);
		CollectionAssert.AreEqual(new long[] { 1, 1 }, plan.Entries.Select(x => x.Id).ToArray());
	}

	[TestMethod]
	public void TryApply_InvalidPlanDoesNotMutateAndValidPlanAppliesOnlyChanges()
	{
		var invalid = CreatePlan(
			new[] { Item(1, "one"), Item(2, "two") },
			"^(one|two)$",
			"shared");
		var valid = CreatePlan(new[] { Item(1, "one") }, "one", "two");
		var mutations = new List<string>();

		Assert.IsFalse(invalid.TryApply((_, _) => mutations.Add("invalid"), out var invalidCount));
		Assert.AreEqual(0, invalidCount);
		Assert.AreEqual(0, mutations.Count);

		Assert.IsTrue(valid.TryApply((_, name) => mutations.Add(name), out var validCount));
		Assert.AreEqual(1, validCount);
		CollectionAssert.AreEqual(new[] { "two" }, mutations);
	}

	[TestMethod]
	public void RegisteredEditableHelpers_ExposeTheSharedNameMetadata()
	{
		var helpers = GetRegisteredHelpers<EditableItemHelper>()
			.Select(x => (x.NameSetCommand, HasScope: x.NameScopeKeyFunc is not null,
				HasNormalisation: x.NameNormalisationFunc is not null,
				HasApply: x.SetNameFromValidatedBulkRenameAction is not null))
			.Concat(GetRegisteredHelpers<EditableRevisableItemHelper>()
				.Select(x => (x.NameSetCommand, HasScope: x.NameScopeKeyFunc is not null,
					HasNormalisation: x.NameNormalisationFunc is not null,
					HasApply: x.SetNameFromValidatedBulkRenameAction is not null)))
			.ToList();

		Assert.IsTrue(helpers.Count > 0);
		foreach (var helper in helpers)
		{
			Assert.IsFalse(string.IsNullOrWhiteSpace(helper.NameSetCommand));
			Assert.IsTrue(helper.HasScope);
			Assert.IsTrue(helper.HasNormalisation);
			Assert.IsTrue(helper.HasApply);
		}
	}

	private static IEnumerable<T> GetRegisteredHelpers<T>() where T : class
	{
		const BindingFlags Flags = BindingFlags.Public | BindingFlags.Static;
		return typeof(T)
			.GetProperties(Flags)
			.Where(x => x.PropertyType == typeof(T))
			.Select(x => x.GetValue(null))
			.Concat(typeof(T)
				.GetFields(Flags)
				.Where(x => x.FieldType == typeof(T))
				.Select(x => x.GetValue(null)))
			.OfType<T>();
	}

	private static EditableItemNameRenamePlan<TestItem> CreatePlan(
		IEnumerable<TestItem> items,
		string pattern,
		string replacement)
	{
		var activeItems = items
			.Where(x => x.Status is RevisionStatus.Current or RevisionStatus.PendingRevision or RevisionStatus.UnderDesign)
			.ToList();
		return EditableItemNameBulkRenamePlanner.CreatePlan(
			activeItems,
			pattern,
			replacement,
			x => x.Id,
			x => x.Revision,
			x => x.Status,
			x => x.Name,
			x => x.Scope,
			(_, name) => Normalise(name));
	}

	private static EditableItemNameValidationResult Normalise(string name)
	{
		var normalised = name.Trim();
		if (normalised.Length == 0)
		{
			return EditableItemNameValidationResult.Failure("Names cannot be blank.");
		}

		if (normalised.All(char.IsDigit))
		{
			return EditableItemNameValidationResult.Failure("Names cannot be entirely numeric.");
		}

		return EditableItemNameValidationResult.Success(normalised);
	}

	private static TestItem Item(long id, string name, string scope = "default", int revision = 1,
		RevisionStatus status = RevisionStatus.Current)
	{
		return new TestItem(id, revision, status, name, scope);
	}

	private sealed record TestItem(long Id, int Revision, RevisionStatus Status, string Name, string Scope);
}
