#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Work.Loot;
using MudSharp.FutureProg;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.Framework.Save;
using MudSharp.Framework.Units;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;
using MudSharp.PerceptionEngine;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
	public void CanonicalXml_ItemInitialState_RoundTripsAndLegacyPayloadDefaultsOpenUnlocked()
	{
		var definition = RepresentativeDefinition();
		var choice = definition.Variants.Single().Groups[0].Choices.Single();
		choice.StartsClosed = true;
		choice.StartsLocked = true;

		var xml = definition.ToCanonicalXml();
		var loaded = LootTableDefinition.Load(xml);

		StringAssert.Contains(xml, "closed=\"true\"");
		StringAssert.Contains(xml, "locked=\"true\"");
		Assert.IsTrue(loaded.Variants.Single().Groups[0].Choices.Single().StartsClosed);
		Assert.IsTrue(loaded.Variants.Single().Groups[0].Choices.Single().StartsLocked);

		var legacy = LootTableDefinition.Load(xml.Replace(" closed=\"true\"", string.Empty)
			.Replace(" locked=\"true\"", string.Empty));
		Assert.IsFalse(legacy.Variants.Single().Groups[0].Choices.Single().StartsClosed);
		Assert.IsFalse(legacy.Variants.Single().Groups[0].Choices.Single().StartsLocked);
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
	public void Planner_ItemQuantity_ContributesToExpansionLimit()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "large-result" };
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "many-items",
			Kind = LootChoiceKind.Item,
			ItemPrototypeId = 1,
			QuantityMinimum = LootTablePlanner.MaximumPlannedItems + 1,
			QuantityMaximum = LootTablePlanner.MaximumPlannedItems + 1
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);

		var result = new LootTablePlanner((_, _) => null).CreatePlan(Source(1, 0, definition), "default", 0);

		Assert.IsFalse(result.Success);
		Assert.AreEqual("EXPANSION_LIMIT", result.ErrorCode);
	}

	[TestMethod]
	public void Validator_ExaminesUnselectedNestedChoicePathsForCycles()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "selection" };
		group.Choices.Add(new LootChoiceDefinition { Key = "empty", Kind = LootChoiceKind.Nothing });
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "recursive", Kind = LootChoiceKind.LootTable, NestedTableId = 1,
			NestedTableRevision = 0, NestedVariant = "default"
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		var table = LootTableMock(1, 0, definition);
		var tables = new Mock<IUneditableRevisableAll<ILootTable>>();
		tables.Setup(x => x.Get(1, 0)).Returns(table.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.LootTables).Returns(tables.Object);

		var errors = LootTableValidator.Validate(table.Object, gameworld.Object);

		Assert.IsTrue(errors.Any(x => x.Contains("cycle", StringComparison.OrdinalIgnoreCase)));
	}

	[TestMethod]
	public void Validator_RejectsWorstCaseQuantityAboveExpansionLimit()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "selection" };
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "many-items", Kind = LootChoiceKind.Item, ItemPrototypeId = 101, ItemPrototypeRevision = 0,
			QuantityMinimum = LootTablePlanner.MaximumPlannedItems + 1,
			QuantityMaximum = LootTablePlanner.MaximumPlannedItems + 1
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		var table = LootTableMock(1, 0, definition);
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		prototype.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		prototypes.Setup(x => x.Get(101, 0)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		gameworld.SetupGet(x => x.LootTables).Returns(new Mock<IUneditableRevisableAll<ILootTable>>().Object);

		var errors = LootTableValidator.Validate(table.Object, gameworld.Object);

		Assert.IsTrue(errors.Any(x => x.Contains("maximum expansion", StringComparison.OrdinalIgnoreCase)));
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
	public void Planner_ItemInitialState_IsPartOfPlanAndDigest()
	{
		var definition = RepresentativeDefinition();
		var open = new LootTablePlanner((_, _) => null).CreatePlan(Source(1, 0, definition), "default", 9);
		definition.Variants.Single().Groups[0].Choices.Single().StartsClosed = true;
		var closed = new LootTablePlanner((_, _) => null).CreatePlan(Source(1, 0, definition), "default", 9);

		Assert.IsTrue(open.Success, open.ErrorMessage);
		Assert.IsTrue(closed.Success, closed.ErrorMessage);
		Assert.IsFalse(open.Plan!.Leaves[0].StartsClosed);
		Assert.IsTrue(closed.Plan!.Leaves[0].StartsClosed);
		Assert.AreNotEqual(open.Plan.Digest, closed.Plan.Digest);
	}

	[TestMethod]
	public void Materialiser_InitialClosedLockedState_IsAppliedAfterCreationWithoutEcho()
	{
		var isOpen = true;
		var isLocked = false;
		var openable = new Mock<IOpenable>();
		openable.SetupGet(x => x.IsOpen).Returns(() => isOpen);
		openable.Setup(x => x.Close()).Callback(() => isOpen = false);
		var lockComponent = new Mock<ILock>();
		lockComponent.SetupGet(x => x.IsLocked).Returns(() => isLocked);
		lockComponent.Setup(x => x.SetLocked(true, false)).Callback(() => isLocked = true).Returns(true);
		var item = new Mock<IGameItem>();
		item.Setup(x => x.GetItemType<IOpenable>()).Returns(openable.Object);
		item.Setup(x => x.GetItemType<ILock>()).Returns(lockComponent.Object);

		LootTableMaterialiser.ApplyInitialState(item.Object, true, true);

		Assert.IsFalse(isOpen);
		Assert.IsTrue(isLocked);
		openable.Verify(x => x.Close(), Times.Once);
		lockComponent.Verify(x => x.SetLocked(true, false), Times.Once);
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
	public void AtomicBatch_RollbackFailure_PreservesOriginalOperationFailure()
	{
		var error = Assert.ThrowsException<InvalidOperationException>(() => LootAtomicBatch.Execute(
			new[] { 1 }, _ => new[] { 1 }, _ => throw new InvalidOperationException("creation"), _ => { },
			_ => throw new InvalidOperationException("rollback")));

		Assert.AreEqual("creation", error.Message);
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

	[TestMethod]
	public void BuildingCommand_ChoiceAdd_TargetsNamedNonFirstGroupWithoutConsumingArguments()
	{
		var definition = RepresentativeDefinition();
		definition.Variants.Single().Groups[1].Choices.Clear();
		var row = new MudSharp.Models.LootTable
		{
			Id = 1,
			RevisionNumber = 0,
			Name = "Builder Test",
			AlgorithmVersion = LootTableDefinition.CurrentAlgorithmVersion,
			Definition = definition.ToCanonicalXml(),
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.UnderDesign,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		gameworld.Setup(x => x.ItemProtos).Returns(new Mock<IUneditableRevisableAll<IGameItemProto>>().Object);
		var table = new MudSharp.Work.Loot.LootTable(row, gameworld.Object);
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.OutputHandler).Returns(output.Object);

		var result = table.BuildingCommand(actor.Object,
			new StringStack("choice add default contents salvage 1 nothing"));

		Assert.IsTrue(result);
		Assert.AreEqual(1, table.Definition.Variants.Single().Groups[0].Choices.Count);
		var added = table.Definition.Variants.Single().Groups[1].Choices.Single();
		Assert.AreEqual("salvage", added.Key);
		Assert.AreEqual(LootChoiceKind.Nothing, added.Kind);
	}

	[TestMethod]
	public void Show_RendersHumanReadableVariantChoiceTable()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "selection", DestinationKey = "target" };
		group.Choices.Add(new LootChoiceDefinition { Key = "empty", Kind = LootChoiceKind.Nothing, Weight = 3 });
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		var row = new MudSharp.Models.LootTable
		{
			Id = 7,
			RevisionNumber = 2,
			Name = "Readable Test",
			AlgorithmVersion = LootTableDefinition.CurrentAlgorithmVersion,
			Definition = definition.ToCanonicalXml(),
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 2,
				RevisionStatus = (int)RevisionStatus.Current,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
		var gameworld = new Mock<IFuturemud>();
		var account = new Mock<MudSharp.Accounts.IAccount>();
		account.Setup(x => x.LineFormatLength).Returns(160);
		account.Setup(x => x.UseUnicode).Returns(true);
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.Account).Returns(account.Object);
		var table = new MudSharp.Work.Loot.LootTable(row, gameworld.Object);

		var shown = table.Show(actor.Object).StripANSIColour();

		StringAssert.Contains(shown, "Loot Table #7r2: Readable Test");
		StringAssert.Contains(shown, "Variant: default");
		foreach (var heading in new[] { "Group", "Repeat", "Destination", "Choice", "Weight / Chance", "Result" })
			StringAssert.Contains(shown, heading);
		StringAssert.Contains(shown, "1. selection");
		StringAssert.Contains(shown, "Outer target");
		StringAssert.Contains(shown, $"{3:N0} ({1.0:P2})");
		StringAssert.Contains(shown, "Nothing");
	}

	[TestMethod]
	public void BuildingCommand_CommodityMass_AcceptsExplicitHumanUnits()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		variant.Groups.Add(new LootRollGroupDefinition { Key = "contents" });
		definition.Variants.Add(variant);
		var row = new MudSharp.Models.LootTable
		{
			Id = 8,
			RevisionNumber = 0,
			Name = "Unit Test",
			AlgorithmVersion = LootTableDefinition.CurrentAlgorithmVersion,
			Definition = definition.ToCanonicalXml(),
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.UnderDesign,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
		var minimum = 125.0;
		var maximum = 250.0;
		var units = new Mock<IUnitManager>();
		var actor = new Mock<ICharacter>();
		units.Setup(x => x.TryGetBaseUnits("125g", UnitType.Mass, actor.Object, out minimum)).Returns(true);
		units.Setup(x => x.TryGetBaseUnits("250g", UnitType.Mass, actor.Object, out maximum)).Returns(true);
		var saveManager = new Mock<ISaveManager>();
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.UnitManager).Returns(units.Object);
		gameworld.Setup(x => x.SaveManager).Returns(saveManager.Object);
		var output = new Mock<IOutputHandler>();
		actor.Setup(x => x.OutputHandler).Returns(output.Object);
		var table = new MudSharp.Work.Loot.LootTable(row, gameworld.Object);

		var result = table.BuildingCommand(actor.Object,
			new StringStack("choice add default contents steel 1 commodity 24 mass 125g 250g"));

		Assert.IsTrue(result);
		var choice = table.Definition.Variants.Single().Groups.Single().Choices.Single();
		Assert.AreEqual(125.0, choice.MassMinimum);
		Assert.AreEqual(250.0, choice.MassMaximum);
	}

	[TestMethod]
	public void BuildingCommand_ItemLocked_AlsoAuthorsClosedState()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		variant.Groups.Add(new LootRollGroupDefinition { Key = "vessel" });
		definition.Variants.Add(variant);
		var row = new MudSharp.Models.LootTable
		{
			Id = 9,
			RevisionNumber = 0,
			Name = "State Test",
			AlgorithmVersion = LootTableDefinition.CurrentAlgorithmVersion,
			Definition = definition.ToCanonicalXml(),
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.UnderDesign,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
		var gameworld = new Mock<IFuturemud>();
		gameworld.Setup(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		gameworld.Setup(x => x.ItemProtos).Returns(new Mock<IUneditableRevisableAll<IGameItemProto>>().Object);
		var output = new Mock<IOutputHandler>();
		var actor = new Mock<ICharacter>();
		actor.Setup(x => x.OutputHandler).Returns(output.Object);
		var table = new MudSharp.Work.Loot.LootTable(row, gameworld.Object);

		var result = table.BuildingCommand(actor.Object,
			new StringStack("choice add default vessel cage 1 item 1010 revision 1 locked as vessel"));

		Assert.IsTrue(result);
		var choice = table.Definition.Variants.Single().Groups.Single().Choices.Single();
		Assert.IsTrue(choice.StartsClosed);
		Assert.IsTrue(choice.StartsLocked);
		Assert.AreEqual("vessel", choice.ResultKey);
	}

	[TestMethod]
	public void BuildingCommand_ChoiceVariables_InvalidInputPreservesExistingAssignments()
	{
		var definition = RepresentativeDefinition();
		var row = new MudSharp.Models.LootTable
		{
			Id = 10,
			RevisionNumber = 0,
			Name = "Variables Test",
			AlgorithmVersion = LootTableDefinition.CurrentAlgorithmVersion,
			Definition = definition.ToCanonicalXml(),
			EditableItem = new MudSharp.Models.EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = (int)RevisionStatus.UnderDesign,
				BuilderAccountId = 1,
				BuilderDate = DateTime.UtcNow
			}
		};
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.SaveManager).Returns(new Mock<ISaveManager>().Object);
		gameworld.SetupGet(x => x.ItemProtos).Returns(new Mock<IUneditableRevisableAll<IGameItemProto>>().Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var table = new MudSharp.Work.Loot.LootTable(row, gameworld.Object);
		var choice = table.Definition.Variants.Single().Groups.Single(x => x.Key == "container").Choices.Single();
		choice.Characteristics.Add(new LootCharacteristicValue { DefinitionId = 10, ValueId = 100 });

		var result = table.BuildingCommand(actor.Object,
			new StringStack("choice variables default container box 20=not-a-number"));

		Assert.IsFalse(result);
		Assert.AreEqual(1, choice.Characteristics.Count);
		Assert.AreEqual(10L, choice.Characteristics.Single().DefinitionId);
		Assert.AreEqual(100L, choice.Characteristics.Single().ValueId);
	}

	[TestMethod]
	public void BuilderLookup_NamedRevision_SelectsRequestedRevision()
	{
		var current = LootTableMock(41, 0, new LootTableDefinition());
		var requested = LootTableMock(41, 3, new LootTableDefinition());
		var tables = new Mock<IUneditableRevisableAll<ILootTable>>();
		tables.Setup(x => x.GetByName("parcel", true)).Returns(current.Object);
		tables.Setup(x => x.Get(41, 3)).Returns(requested.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.LootTables).Returns(tables.Object);
		var actor = new Mock<ICharacter>();
		actor.SetupGet(x => x.Gameworld).Returns(gameworld.Object);
		actor.SetupGet(x => x.OutputHandler).Returns(new Mock<IOutputHandler>().Object);
		var method = typeof(Futuremud).Assembly
			.GetType("MudSharp.Commands.Modules.ItemBuilderModule")!
			.GetMethod("TryGetLootTable", BindingFlags.NonPublic | BindingFlags.Static)!;
		object?[] arguments = [actor.Object, new StringStack("parcel 3"), null];

		var success = (bool)method.Invoke(null, arguments)!;

		Assert.IsTrue(success);
		Assert.AreSame(requested.Object, arguments[2]);
	}

	[TestMethod]
	public void Materialiser_NonStackableQuantity_CreatesEveryAuthoredItem()
	{
		var item = new Mock<IGameItem>();
		var prototype = new Mock<IGameItemProto>();
		prototype.Setup(x => x.IsItemType<IStackablePrototype>()).Returns(false);
		prototype.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		prototype.Setup(x => x.CreateNew<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(
				It.IsAny<ICharacter>(), It.IsAny<IGameItemSkin>(), 1,
				It.IsAny<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(), false))
			.Returns([item.Object]);
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		prototypes.Setup(x => x.Get(501, 2)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		var materialiser = new LootTableMaterialiser(gameworld.Object);
		var leaf = new LootPlannedLeaf(LootChoiceKind.Item, "test", "target", null, 501, 2, 12, 5,
			false, false, [], 0, null, 0.0);
		var method = typeof(LootTableMaterialiser).GetMethod("CreateLeaf",
			BindingFlags.NonPublic | BindingFlags.Instance)!;

		var created = ((IEnumerable<IGameItem>)method.Invoke(materialiser, [leaf])!).ToList();

		Assert.AreEqual(12, created.Count);
		prototype.Verify(x => x.CreateNew<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(
			It.IsAny<ICharacter>(), It.IsAny<IGameItemSkin>(), 1,
			It.IsAny<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(), false), Times.Exactly(12));
	}

	[TestMethod]
	public void Materialiser_PostCommitOnLoadFailure_LeavesCommittedPackageAndReportsWarning()
	{
		var definition = new LootTableDefinition();
		var variant = new LootVariantDefinition { Key = "default" };
		var group = new LootRollGroupDefinition { Key = "root" };
		group.Choices.Add(new LootChoiceDefinition
		{
			Key = "item",
			Kind = LootChoiceKind.Item,
			ItemPrototypeId = 501,
			ItemPrototypeRevision = 2,
			QuantityMinimum = 1,
			QuantityMaximum = 1,
			QualityMinimum = 5,
			QualityMaximum = 5
		});
		variant.Groups.Add(group);
		definition.Variants.Add(variant);
		var table = LootTableMock(17, 0, definition);
		table.SetupGet(x => x.AlgorithmVersion).Returns(LootTableDefinition.CurrentAlgorithmVersion);
		var cell = new Mock<ICell>();
		var item = new Mock<IGameItem>();
		item.SetupGet(x => x.Id).Returns(75L);
		item.SetupGet(x => x.Deleted).Returns(false);
		item.SetupGet(x => x.Location).Returns(cell.Object);
		var committed = false;
		var prototype = new Mock<IGameItemProto>();
		prototype.SetupGet(x => x.Components).Returns(Array.Empty<IGameItemComponentProto>());
		prototype.Setup(x => x.IsItemType<IStackablePrototype>()).Returns(false);
		prototype.Setup(x => x.CreateNew<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(
				It.IsAny<ICharacter>(), It.IsAny<IGameItemSkin>(), 1,
				It.IsAny<List<(ICharacteristicDefinition Definition, ICharacteristicValue Value)>>(), false))
			.Returns([item.Object]);
		prototype.Setup(x => x.ExecuteOnLoadProgs(item.Object, null))
			.Callback(() => Assert.IsTrue(committed, "OnLoad must execute only after the package commits."))
			.Throws<InvalidOperationException>();
		item.SetupGet(x => x.Prototype).Returns(prototype.Object);
		var prototypes = new Mock<IUneditableRevisableAll<IGameItemProto>>();
		prototypes.Setup(x => x.Get(501, 2)).Returns(prototype.Object);
		var gameworld = new Mock<IFuturemud>();
		gameworld.SetupGet(x => x.ItemProtos).Returns(prototypes.Object);
		gameworld.Setup(x => x.Add(item.Object)).Callback(() => committed = true);
		var materialiser = new LootTableMaterialiser(gameworld.Object);

		var result = materialiser.Materialise(table.Object, "default", 12, cell.Object);

		Assert.IsTrue(result.Success, result.Receipt);
		StringAssert.Contains(result.Receipt, "postcommitwarnings=1");
		prototype.Verify(x => x.ExecuteOnLoadProgs(item.Object, null), Times.Once);
		gameworld.Verify(x => x.Add(item.Object), Times.Once);
		item.Verify(x => x.Delete(), Times.Never);
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

	private static Mock<ILootTable> LootTableMock(long id, int revision, LootTableDefinition definition)
	{
		var table = new Mock<ILootTable>();
		table.SetupGet(x => x.Id).Returns(id);
		table.SetupGet(x => x.RevisionNumber).Returns(revision);
		table.SetupGet(x => x.Definition).Returns(definition);
		table.SetupGet(x => x.DefinitionHash).Returns(definition.ComputeHash());
		table.SetupGet(x => x.Status).Returns(RevisionStatus.Current);
		return table;
	}
}
