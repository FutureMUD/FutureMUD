#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Work.Loot;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Character;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp_Unit_Tests;

[TestClass]
public class LootTableDefinitionTests
{
	[TestMethod]
	public void CanonicalXml_RoundTrip_PreservesExactOrderedDefinitionAndHash()
	{
		var definition = RepresentativeDefinition();

		var xml = definition.ToCanonicalXml();
		var loaded = LootTableDefinition.Load(xml);

		Assert.AreEqual(xml, loaded.ToCanonicalXml());
		Assert.AreEqual(definition.ComputeHash(), loaded.ComputeHash());
		Assert.AreEqual("container", loaded.Variants.Single().Groups[0].Key);
		Assert.AreEqual("contents", loaded.Variants.Single().Groups[1].Key);
	}

	[TestMethod]
	public void CanonicalXml_CharacteristicEnumerationOrder_DoesNotChangeHash()
	{
		var first = RepresentativeDefinition();
		var item = first.Variants.Single().Groups[0].Choices.Single();
		item.Characteristics.Add(new LootCharacteristicValue { DefinitionId = 20, ValueId = 200 });
		item.Characteristics.Add(new LootCharacteristicValue { DefinitionId = 10, ValueId = 100 });
		var second = first.Clone();
		second.Variants.Single().Groups[0].Choices.Single().Characteristics.Reverse();

		Assert.AreEqual(first.ComputeHash(), second.ComputeHash());
	}

	[TestMethod]
	public void Planner_SameIdentityVariantAndSeed_ReproducesPlanAndDigest()
	{
		var source = Source(1, 0, RepresentativeDefinition());
		var planner = new LootTablePlanner((_, _) => null);

		var first = planner.CreatePlan(source, "default", 123456789);
		var second = planner.CreatePlan(source, "default", 123456789);

		Assert.IsTrue(first.Success, first.ErrorMessage);
		Assert.IsTrue(second.Success, second.ErrorMessage);
		Assert.AreEqual(first.Plan!.Digest, second.Plan!.Digest);
		CollectionAssert.AreEqual(first.Plan.Leaves.Select(x => x.Path).ToList(),
			second.Plan.Leaves.Select(x => x.Path).ToList());
		CollectionAssert.AreEqual(first.Plan.Leaves.Select(x => x.Quantity).ToList(),
			second.Plan.Leaves.Select(x => x.Quantity).ToList());
	}

	[TestMethod]
	public void Planner_NestedExactRevision_UsesParentDestinationAndStableDigest()
	{
		var childDefinition = new LootTableDefinition();
		var childVariant = new LootVariantDefinition { Key = "child" };
		var childGroup = new LootRollGroupDefinition { Key = "leaf", DestinationKey = "target" };
		childGroup.Choices.Add(new LootChoiceDefinition
		{
			Key = "commodity",
			Kind = LootChoiceKind.Commodity,
			CommodityMaterialId = 50,
			MassMinimum = 0.25,
			MassMaximum = 0.25
		});
		childVariant.Groups.Add(childGroup);
		childDefinition.Variants.Add(childVariant);
		var child = Source(2, 3, childDefinition);

		var rootDefinition = new LootTableDefinition();
		var rootVariant = new LootVariantDefinition { Key = "default" };
		var rootGroup = new LootRollGroupDefinition { Key = "nested", DestinationKey = "target" };
		rootGroup.Choices.Add(new LootChoiceDefinition
		{
			Key = "child-table",
			Kind = LootChoiceKind.LootTable,
			NestedTableId = 2,
			NestedTableRevision = 3,
			NestedVariant = "child"
		});
		rootVariant.Groups.Add(rootGroup);
		rootDefinition.Variants.Add(rootVariant);
		var root = Source(1, 0, rootDefinition);

		var result = new LootTablePlanner((id, revision) => id == 2 && revision == 3 ? child : null)
			.CreatePlan(root, "default", 44);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		Assert.AreEqual(1, result.Plan!.Leaves.Count);
		Assert.AreEqual("target", result.Plan.Leaves[0].DestinationKey);
		StringAssert.Contains(result.Plan.Leaves[0].Path, "table:2r3|variant:child");
	}

	[TestMethod]
	public void Planner_DirectCycle_ReturnsStableError()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "cycle" };
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "self",
			Kind = LootChoiceKind.LootTable,
			NestedTableId = 1,
			NestedTableRevision = 0,
			NestedVariant = "default"
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		var source = Source(1, 0, definition);

		var result = new LootTablePlanner((id, revision) => id == 1 && revision == 0 ? source : null)
			.CreatePlan(source, "default", 1);

		Assert.IsFalse(result.Success);
		Assert.AreEqual("CYCLE", result.ErrorCode);
	}

	[TestMethod]
	public void DrawBounded_Boundaries_AlwaysFallWithinUnbiasedRange()
	{
		foreach (var bound in new ulong[] { 1, 2, 3, 7, 10, 65537, ulong.MaxValue })
		{
			for (var seed = 0; seed < 100; seed++)
			{
				Assert.IsTrue(LootTablePlanner.DrawBounded(seed, "boundary", bound) < bound);
			}
		}
	}

	[TestMethod]
	public void Planner_InclusiveFixedRanges_ReturnAuthoredEndpoints()
	{
		var source = Source(1, 0, RepresentativeDefinition());
		var result = new LootTablePlanner((_, _) => null).CreatePlan(source, "default", 9);

		Assert.IsTrue(result.Success, result.ErrorMessage);
		var container = result.Plan!.Leaves[0];
		Assert.AreEqual(1, container.Quantity);
		Assert.AreEqual(8, container.Quality);
		var contents = result.Plan.Leaves[1];
		Assert.IsTrue(contents.Quantity >= 2);
		Assert.IsTrue(contents.Quantity <= 4);
		Assert.IsTrue(contents.Quality >= 3);
		Assert.IsTrue(contents.Quality <= 7);
	}

	[TestMethod]
	public void AtomicBatch_PreflightFailure_RollsBackEveryCreatedObjectAndNeverCommits()
	{
		var rolledBack = new List<int>();
		var commits = 0;
		Assert.ThrowsException<InvalidOperationException>(() => LootAtomicBatch.Execute(
			new[] { 1, 2, 3 },
			x => new[] { x * 10, x * 10 + 1 },
			_ => throw new InvalidOperationException("preflight"),
			_ => commits++,
			items => rolledBack.AddRange(items)));
		Assert.AreEqual(0, commits);
		CollectionAssert.AreEqual(new[] { 10, 11, 20, 21, 30, 31 }, rolledBack);
	}

	[TestMethod]
	public void AtomicBatch_CreateFailure_RollsBackOnlyObjectsAlreadyCreated()
	{
		var rolledBack = new List<int>();
		IEnumerable<int> Create(int value)
		{
			yield return value;
			if (value == 2) throw new InvalidOperationException("creation");
		}

		Assert.ThrowsException<InvalidOperationException>(() => LootAtomicBatch.Execute(
			new[] { 1, 2, 3 }, Create, _ => { }, _ => { }, items => rolledBack.AddRange(items)));
		CollectionAssert.AreEqual(new[] { 1, 2 }, rolledBack);
	}

	[TestMethod]
	public void Clone_CanonicalSaveLoadCopy_PreservesHashAndIndependentOrdering()
	{
		var original = RepresentativeDefinition();
		var clone = original.Clone();
		Assert.AreEqual(original.ComputeHash(), clone.ComputeHash());
		clone.Variants[0].Groups.Reverse();
		Assert.AreNotEqual(original.ComputeHash(), clone.ComputeHash());
		Assert.AreEqual("container", original.Variants[0].Groups[0].Key);
	}

	[TestMethod]
	public void Planner_IndirectCycle_ReturnsStableCycleError()
	{
		var a = NestedDefinition(2, 0);
		var b = NestedDefinition(1, 0);
		var sources = new Dictionary<(long, int), LootTablePlanSource>
		{
			[(1, 0)] = Source(1, 0, a),
			[(2, 0)] = Source(2, 0, b)
		};
		var result = new LootTablePlanner((id, revision) => sources.GetValueOrDefault((id, revision)))
			.CreatePlan(sources[(1, 0)], "default", 12);
		Assert.IsFalse(result.Success);
		Assert.AreEqual("CYCLE", result.ErrorCode);
	}

	[TestMethod]
	public void FutureProgRegistration_AdvertisesExactlySixTypedLoadLootTableContracts()
	{
		var before = FutureProg.GetFunctionCompilerInformations().Where(x => x.FunctionName == "loadloottable").ToList();
		var type = typeof(FutureProg).Assembly.GetType("MudSharp.FutureProg.Functions.GameItem.LoadLootTableFunction");
		Assert.IsNotNull(type);
		type.GetMethod("RegisterFunctionCompiler", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)!.Invoke(null, null);
		var added = FutureProg.GetFunctionCompilerInformations().Where(x => x.FunctionName == "loadloottable").Skip(before.Count).ToList();
		try
		{
			Assert.AreEqual(6, added.Count);
			foreach (var target in new[] { ProgVariableTypes.Location, ProgVariableTypes.Item, ProgVariableTypes.Character })
			{
				Assert.AreEqual(1, added.Count(x => x.Parameters.SequenceEqual(new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, target, ProgVariableTypes.Text })));
				Assert.AreEqual(1, added.Count(x => x.Parameters.SequenceEqual(new[] { ProgVariableTypes.Number, ProgVariableTypes.Number, target, ProgVariableTypes.Text, ProgVariableTypes.Number })));
			}
			Assert.IsTrue(added.All(x => x.ReturnType == ProgVariableTypes.Text));
		}
		finally
		{
			var field = typeof(FutureProg).GetField("BuiltInFunctionCompilers", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			var registrations = (System.Collections.IList)field!.GetValue(null)!;
			foreach (var registration in added) registrations.Remove(registration);
		}
	}

	[TestMethod]
	public void ReviewProposal_ResolvesLootTableRegistryThroughRealLifecycleOwner()
	{
		var registry = new Mock<IUneditableRevisableAll<ILootTable>>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.LootTables).Returns(registry.Object);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Gameworld).Returns(gameworld.Object);
		var proposal = new EditableItemReviewProposal<ILootTable>(actor.Object, []);
		var method = typeof(EditableItemReviewProposal<ILootTable>).GetMethod("GetAppropriateAll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		Assert.IsNotNull(method);
		Assert.AreSame(registry.Object, method.Invoke(proposal, null));
	}

	private static LootTableDefinition RepresentativeDefinition()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var container = new LootRollGroupDefinition { Key = "container", DestinationKey = "target" };
		container.Choices.Add(new LootChoiceDefinition
		{
			Key = "box",
			Kind = LootChoiceKind.Item,
			ItemPrototypeId = 100,
			ItemPrototypeRevision = 2,
			QuantityMinimum = 1,
			QuantityMaximum = 1,
			QualityMinimum = 8,
			QualityMaximum = 8,
			ResultKey = "box"
		});
		var contents = new LootRollGroupDefinition
		{
			Key = "contents",
			DestinationKey = "box",
			RepeatMinimum = 1,
			RepeatMaximum = 1
		};
		contents.Choices.Add(new LootChoiceDefinition
		{
			Key = "parts",
			Kind = LootChoiceKind.Item,
			ItemPrototypeId = 200,
			ItemPrototypeRevision = 4,
			QuantityMinimum = 2,
			QuantityMaximum = 4,
			QualityMinimum = 3,
			QualityMaximum = 7
		});
		variant.Groups.Add(container);
		variant.Groups.Add(contents);
		definition.Variants.Add(variant);
		return definition;
	}

	private static LootTableDefinition NestedDefinition(long targetId, int targetRevision)
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "nested" };
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "next",
			Kind = LootChoiceKind.LootTable,
			NestedTableId = targetId,
			NestedTableRevision = targetRevision,
			NestedVariant = "default"
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		return definition;
	}

	private static LootTablePlanSource Source(long id, int revision, LootTableDefinition definition) =>
		new(id, revision, definition.ComputeHash(), definition);
}
