#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.Events;
using MudSharp.Form.Characteristics;
using MudSharp.GameItems;
using MudSharp.GameItems.Interfaces;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Work.Loot;

public sealed record LootMaterialisationResult(bool Success, string Receipt, LootTablePlan? Plan = null);

public sealed class LootTableMaterialiser
{
	private readonly IFuturemud _gameworld;

	public LootTableMaterialiser(IFuturemud gameworld)
	{
		_gameworld = gameworld;
	}

	public LootTablePlanResult Preview(ILootTable table, string variant, long seed) => Planner().CreatePlan(Source(table), variant, seed);

	public LootMaterialisationResult Materialise(ILootTable table, string variant, long seed, ICell target) =>
		Materialise(table, variant, seed, new Destination(target, null, null));

	public LootMaterialisationResult Materialise(ILootTable table, string variant, long seed, IGameItem target) =>
		Materialise(table, variant, seed, new Destination(null, target, null));

	public LootMaterialisationResult Materialise(ILootTable table, string variant, long seed, ICharacter target) =>
		Materialise(table, variant, seed, new Destination(null, null, target));

	private LootMaterialisationResult Materialise(ILootTable table, string variant, long seed, Destination destination)
	{
		var planned = Preview(table, variant, seed);
		if (!planned.Success) return Error(planned.ErrorCode!, planned.ErrorMessage!);
		var plan = planned.Plan!;
		var created = new List<IGameItem>();
		IReadOnlyList<(LootPlannedLeaf Leaf, IGameItem Item)> plannedItems = [];
		var keyed = new Dictionary<string, IGameItem>(StringComparer.OrdinalIgnoreCase);
		try
		{
			plannedItems = LootAtomicBatch.Execute<LootPlannedLeaf, (LootPlannedLeaf Leaf, IGameItem Item)>(
				plan.Leaves,
				leaf => CreateLeaf(leaf).Select(item => (leaf, item)),
				items =>
				{
					foreach (var pair in items.Where(x => x.Leaf.Kind == LootChoiceKind.Item))
					{
						ValidateInitialState(pair.Item, pair.Leaf.StartsClosed, pair.Leaf.StartsLocked);
					}
					foreach (var grouping in items.Where(x => x.Leaf.ResultKey is not null).GroupBy(x => x.Leaf.ResultKey!))
					{
						if (grouping.Count() != 1 || !keyed.TryAdd(grouping.Key, grouping.Single().Item)) throw new LootMaterialisationException("INVALID_RESULT_KEY", "A result key did not resolve to exactly one item.");
					}
					PreflightDestinations(items, keyed, destination);
				},
				items =>
				{
					foreach (var pair in items.Where(x => x.Leaf.DestinationKey != "target"))
					{
						var target = keyed[pair.Leaf.DestinationKey];
						target.GetItemType<IContainer>().Put(null, pair.Item, allowMerge: false);
						if (!ReferenceEquals(pair.Item.ContainedIn, target)) throw new LootMaterialisationException("PLACEMENT_FAILED", "A planned item was not inserted into its local container.");
					}
					foreach (var item in items.Where(x => x.Leaf.DestinationKey == "target").Select(x => x.Item)) PlaceRoot(destination, item);
					foreach (var pair in items.Where(x => x.Leaf.Kind == LootChoiceKind.Item))
					{
						ApplyInitialState(pair.Item, pair.Leaf.StartsClosed, pair.Leaf.StartsLocked);
					}
					foreach (var item in items.Select(x => x.Item))
					{
						_gameworld.Add(item);
					}
				},
				items => Cleanup(items.Select(x => x.Item)));
			created.AddRange(plannedItems.Select(x => x.Item));
			var postCommitWarnings = FinaliseCommittedItems(created);
			var roots = plannedItems.Where(x => x.Leaf.DestinationKey == "target").Select(x => x.Item).ToList();
			var rootIds = string.Join(',', roots.Select(x => x.Id));
			var warningReceipt = postCommitWarnings == 0 ? string.Empty : $" postcommitwarnings={postCommitWarnings}";
			return new LootMaterialisationResult(true,
				$"OK table={table.Id}r{table.RevisionNumber} hash={table.DefinitionHash} algorithm={table.AlgorithmVersion} variant={variant} seed={seed} roots={rootIds} created={created.Count} digest={plan.Digest}{warningReceipt}", plan);
		}
		catch (LootMaterialisationException ex)
		{
			return Error(ex.Code, ex.Message);
		}
		catch (Exception ex)
		{
			return Error("MATERIALISATION_FAILED", ex.Message);
		}
	}

	private IEnumerable<IGameItem> CreateLeaf(LootPlannedLeaf leaf)
	{
		if (leaf.Kind == LootChoiceKind.Commodity)
		{
			var material = _gameworld.Materials.Get(leaf.CommodityMaterialId) ?? throw new LootMaterialisationException("MATERIAL_NOT_FOUND", "A planned material no longer exists.");
			var tag = leaf.CommodityTagId is null ? null : _gameworld.Tags.Get(leaf.CommodityTagId.Value) ?? throw new LootMaterialisationException("TAG_NOT_FOUND", "A planned commodity tag no longer exists.");
			return [CommodityGameItemComponentProto.CreateNewCommodity(material, leaf.Mass, tag)];
		}

		var proto = _gameworld.ItemProtos.Get(leaf.ItemPrototypeId, leaf.ItemPrototypeRevision) ?? throw new LootMaterialisationException("PROTOTYPE_NOT_FOUND", "A planned exact item prototype no longer exists.");
		if (proto.Components.Any(x => x.PreventManualLoad)) throw new LootMaterialisationException("PROTOTYPE_NOT_LOADABLE", "A planned item prototype prevents loading.");
		var variables = leaf.Characteristics.Select(x =>
		{
			var definition = _gameworld.Characteristics.Get(x.DefinitionId) ?? throw new LootMaterialisationException("CHARACTERISTIC_NOT_FOUND", "A planned characteristic definition no longer exists.");
			var value = _gameworld.CharacteristicValues.Get(x.ValueId) ?? throw new LootMaterialisationException("CHARACTERISTIC_VALUE_NOT_FOUND", "A planned characteristic value no longer exists.");
			if (value.Definition.Id != definition.Id) throw new LootMaterialisationException("CHARACTERISTIC_MISMATCH", "A planned characteristic value does not belong to its definition.");
			return (definition, value);
		}).ToList();
		var items = new List<IGameItem>();
		var quantityPerCreation = proto.IsItemType<IStackablePrototype>() ? leaf.Quantity : 1;
		for (var remaining = leaf.Quantity; remaining > 0; remaining -= quantityPerCreation)
		{
			items.AddRange(proto.CreateNew(null!, null!, quantityPerCreation, variables, executeOnLoadProgs: false));
		}
		if (items.Count == 0)
		{
			throw new LootMaterialisationException("PROTOTYPE_CREATE_FAILED", "A planned item prototype did not create an item.");
		}
		foreach (var item in items) item.Quality = (ItemQuality)leaf.Quality;
		return items;
	}

	private static void ValidateInitialState(IGameItem item, bool startsClosed, bool startsLocked)
	{
		if (startsClosed && item.GetItemType<IOpenable>() is null)
		{
			throw new LootMaterialisationException("ITEM_NOT_OPENABLE", "A planned item cannot start closed because it is not openable.");
		}

		if (startsLocked && item.GetItemType<ILock>() is null)
		{
			throw new LootMaterialisationException("ITEM_NOT_LOCKABLE", "A planned item cannot start locked because it has no built-in lock.");
		}
	}

	public static void ApplyInitialState(IGameItem item, bool startsClosed, bool startsLocked)
	{
		if (startsClosed)
		{
			var openable = item.GetItemType<IOpenable>() ??
			               throw new LootMaterialisationException("ITEM_NOT_OPENABLE", "A planned item cannot start closed because it is not openable.");
			if (openable.IsOpen)
			{
				openable.Close();
			}
			if (openable.IsOpen)
			{
				throw new LootMaterialisationException("ITEM_STATE_FAILED", "A planned item could not be closed.");
			}
		}

		if (!startsLocked)
		{
			return;
		}

		var lockComponent = item.GetItemType<ILock>() ??
		                    throw new LootMaterialisationException("ITEM_NOT_LOCKABLE", "A planned item cannot start locked because it has no built-in lock.");
		if (!lockComponent.SetLocked(true, false) || !lockComponent.IsLocked)
		{
			throw new LootMaterialisationException("ITEM_STATE_FAILED", "A planned item could not be locked.");
		}
	}

	private static void PreflightDestinations(IEnumerable<(LootPlannedLeaf Leaf, IGameItem Item)> items, IReadOnlyDictionary<string, IGameItem> keyed, Destination root)
	{
		if (root.Item is not null && root.Item.GetItemType<IContainer>() is null) throw new LootMaterialisationException("TARGET_NOT_CONTAINER", "The target item is not a container.");
		foreach (var pair in items)
		{
			if (pair.Leaf.DestinationKey == "target")
			{
				if (root.Item is not null && !root.Item.GetItemType<IContainer>().CanPut(pair.Item)) throw new LootMaterialisationException("TARGET_REJECTED_ITEM", "The target container rejected a planned item.");
				if (root.Character is not null && !root.Character.Body.CanGet(pair.Item, 0)) throw new LootMaterialisationException("TARGET_REJECTED_ITEM", "The target character cannot receive a planned item.");
				continue;
			}
			if (!keyed.TryGetValue(pair.Leaf.DestinationKey, out var destination)) throw new LootMaterialisationException("DESTINATION_NOT_FOUND", "A planned local destination was not created.");
			var container = destination.GetItemType<IContainer>() ?? throw new LootMaterialisationException("DESTINATION_NOT_CONTAINER", "A planned local destination is not a container.");
			if (!container.CanPut(pair.Item)) throw new LootMaterialisationException("DESTINATION_REJECTED_ITEM", "A planned local container rejected an item.");
		}
	}

	private static void PlaceRoot(Destination target, IGameItem item)
	{
		if (target.Cell is not null)
		{
			target.Cell.Insert(item, newStack: true);
			if (!ReferenceEquals(item.Location, target.Cell)) throw new LootMaterialisationException("PLACEMENT_FAILED", "A planned item was not inserted into the target location.");
		}
		else if (target.Item is not null)
		{
			target.Item.GetItemType<IContainer>().Put(null, item, allowMerge: false);
			if (!ReferenceEquals(item.ContainedIn, target.Item)) throw new LootMaterialisationException("PLACEMENT_FAILED", "A planned item was not inserted into the target container.");
		}
		else if (target.Character is not null)
		{
			if (target.Character.Body.GetWithoutMerge(item, silent: true, triggerEvents: false) is null) throw new LootMaterialisationException("PLACEMENT_FAILED", "A planned item was not inserted into the target inventory.");
		}
		else throw new LootMaterialisationException("NULL_TARGET", "The materialisation target was null.");
	}

	private int FinaliseCommittedItems(IEnumerable<IGameItem> items)
	{
		var warnings = 0;
		foreach (var item in items)
		{
			warnings += TryFinalise(item, "OnLoad", () => item.Prototype.ExecuteOnLoadProgs(item, null));
		}

		foreach (var item in items)
		{
			warnings += TryFinalise(item, "ItemFinishedLoading", () => item.HandleEvent(EventType.ItemFinishedLoading, item));
		}

		foreach (var item in items)
		{
			warnings += TryFinalise(item, "Login", item.Login);
		}

		return warnings;
	}

	private int TryFinalise(IGameItem item, string operation, Action action)
	{
		try
		{
			action();
			return 0;
		}
		catch (Exception ex)
		{
			try
			{
				_gameworld.SystemMessage($"LootTable post-commit {operation} failed for item #{item.Id:N0}: {ex.Message}", true);
			}
			catch
			{
				// A diagnostics failure must not make a committed loot package look as though it rolled back.
			}

			return 1;
		}
	}

	private static void Cleanup(IEnumerable<IGameItem> items)
	{
		foreach (var item in items.Reverse().Where(x => !x.Deleted))
		{
			try
			{
				item.Delete();
			}
			catch
			{
				// Preserve the original materialisation failure while attempting every remaining cleanup.
			}
		}
	}

	private LootTablePlanner Planner() => new((id, revision) =>
	{
		var nested = _gameworld.LootTables.Get(id, revision);
		return nested is null ? null : Source(nested);
	});

	private static LootTablePlanSource Source(ILootTable table) => new(table.Id, table.RevisionNumber, table.DefinitionHash, table.Definition);
	private static LootMaterialisationResult Error(string code, string message) => new(false, $"ERROR code={code} message={message}");
	private sealed record Destination(ICell? Cell, IGameItem? Item, ICharacter? Character);
	private sealed class LootMaterialisationException(string code, string message) : Exception(message) { public string Code { get; } = code; }
}
