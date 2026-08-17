#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;
using MudSharp.Framework.Revision;
using MudSharp.GameItems.Prototypes;

namespace MudSharp.Work.Loot;

public static class LootTableValidator
{
	public static IReadOnlyList<string> Validate(ILootTable table, IFuturemud gameworld)
	{
		var errors = new List<string>();
		var definition = table.Definition;
		if (definition.AlgorithmVersion != LootTableDefinition.CurrentAlgorithmVersion) errors.Add("The deterministic algorithm version is unsupported.");
		if (definition.Variants.Count == 0) errors.Add("At least one variant is required.");
		foreach (var duplicate in definition.Variants.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1)) errors.Add($"Variant key '{duplicate.Key}' is duplicated.");
		foreach (var variant in definition.Variants)
		{
			if (string.IsNullOrWhiteSpace(variant.Key)) errors.Add("Variant keys cannot be empty.");
			if (variant.Groups.Count == 0) errors.Add($"Variant '{variant.Key}' has no groups.");
			var availableKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "target" };
			foreach (var group in variant.Groups)
			{
				var path = $"{variant.Key}/{group.Key}";
					if (string.IsNullOrWhiteSpace(group.Key)) errors.Add($"{path}: group key is empty.");
				if (group.RepeatMinimum < 0 || group.RepeatMaximum < group.RepeatMinimum) errors.Add($"{path}: repetition range is invalid.");
				if (!availableKeys.Contains(group.DestinationKey)) errors.Add($"{path}: destination '{group.DestinationKey}' is not an earlier stable item key.");
				if (group.Choices.Count == 0) errors.Add($"{path}: group has no choices.");
				foreach (var duplicate in group.Choices.GroupBy(x => x.Key, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() > 1)) errors.Add($"{path}: choice key '{duplicate.Key}' is duplicated.");
				foreach (var choice in group.Choices)
				{
					var choicePath = $"{path}/{choice.Key}";
					if (string.IsNullOrWhiteSpace(choice.Key)) errors.Add($"{choicePath}: choice key is empty.");
					if (choice.Weight <= 0) errors.Add($"{choicePath}: weight must be positive.");
					switch (choice.Kind)
					{
						case LootChoiceKind.Item:
							var proto = gameworld.ItemProtos.Get(choice.ItemPrototypeId, choice.ItemPrototypeRevision);
							if (proto is null) errors.Add($"{choicePath}: exact item prototype does not exist.");
							else if (proto.Status != RevisionStatus.Current) errors.Add($"{choicePath}: exact item prototype is not approved/current.");
							if (choice.StartsClosed && proto?.Components.Any(x => x is IOpenablePrototype) != true) errors.Add($"{choicePath}: a closed item must use an openable prototype.");
							if (choice.StartsLocked && !choice.StartsClosed) errors.Add($"{choicePath}: a locked item must also start closed.");
							if (choice.StartsLocked && proto?.Components.Any(x => x is ILockPrototype) != true) errors.Add($"{choicePath}: a locked item must have a built-in lock.");
							if (choice.QuantityMinimum < 1 || choice.QuantityMaximum < choice.QuantityMinimum) errors.Add($"{choicePath}: quantity range is invalid.");
							if (choice.QualityMinimum < 0 || choice.QualityMaximum > 11 || choice.QualityMaximum < choice.QualityMinimum) errors.Add($"{choicePath}: quality range is invalid.");
							foreach (var value in choice.Characteristics)
							{
								var definitionValue = gameworld.Characteristics.Get(value.DefinitionId);
								var characteristicValue = gameworld.CharacteristicValues.Get(value.ValueId);
								if (definitionValue is null || characteristicValue is null || characteristicValue.Definition.Id != definitionValue.Id) errors.Add($"{choicePath}: characteristic #{value.DefinitionId}=#{value.ValueId} is invalid.");
							}
							foreach (var duplicate in choice.Characteristics.GroupBy(x => x.DefinitionId).Where(x => x.Count() > 1)) errors.Add($"{choicePath}: characteristic definition #{duplicate.Key} is assigned more than once.");
							break;
						case LootChoiceKind.Commodity:
							if (gameworld.Materials.Get(choice.CommodityMaterialId) is null) errors.Add($"{choicePath}: material does not exist.");
							if (choice.CommodityTagId is not null && gameworld.Tags.Get(choice.CommodityTagId.Value) is null) errors.Add($"{choicePath}: tag does not exist.");
							if (!double.IsFinite(choice.MassMinimum) || !double.IsFinite(choice.MassMaximum) || choice.MassMinimum <= 0.0 || choice.MassMaximum < choice.MassMinimum) errors.Add($"{choicePath}: mass range is invalid.");
							break;
						case LootChoiceKind.LootTable:
							var nested = gameworld.LootTables.Get(choice.NestedTableId, choice.NestedTableRevision);
							if (nested is null) errors.Add($"{choicePath}: exact nested table does not exist.");
							else if (nested.Status != RevisionStatus.Current && nested.Id != table.Id) errors.Add($"{choicePath}: exact nested table is not approved/current.");
							else if (!nested.Definition.Variants.Any(x => x.Key.EqualTo(choice.NestedVariant))) errors.Add($"{choicePath}: nested variant does not exist.");
							break;
						default:
							errors.Add($"{choicePath}: choice kind is invalid.");
							break;
					}
				}
				var stableKeys = group.Choices.Where(x => x.Kind == LootChoiceKind.Item && !string.IsNullOrWhiteSpace(x.ResultKey)).Select(x => x.ResultKey!).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
				foreach (var key in stableKeys)
				{
					var producers = group.Choices.Where(x => x.ResultKey.EqualTo(key)).ToList();
					if (group.RepeatMinimum == 1 && group.RepeatMaximum == 1 && producers.Count == group.Choices.Count && producers.All(x =>
					    x.QuantityMinimum == 1 && x.QuantityMaximum == 1 &&
					    gameworld.ItemProtos.Get(x.ItemPrototypeId, x.ItemPrototypeRevision)?.Components.Any(y => y is IContainerPrototype) == true)) availableKeys.Add(key);
					else errors.Add($"{path}: result key '{key}' is not produced exactly once on every selectable path.");
				}
			}
		}

		ValidateEveryNestingPath(table, gameworld, errors);
		return errors.Distinct().ToList();
	}

	private static void ValidateEveryNestingPath(ILootTable root, IFuturemud gameworld, ICollection<string> errors)
	{
		foreach (var variant in root.Definition.Variants)
		{
			var maximum = MaximumGeneratedItems(root, variant.Key, gameworld, new HashSet<NestingIdentity>(),
				errors, $"{root.Id}r{root.RevisionNumber}/{variant.Key}");
			if (maximum > LootTablePlanner.MaximumPlannedItems)
			{
				errors.Add($"Nesting validation failed for '{variant.Key}': the maximum expansion exceeds {LootTablePlanner.MaximumPlannedItems:N0} generated items.");
			}
		}
	}

	private static long MaximumGeneratedItems(ILootTable table, string variantKey, IFuturemud gameworld,
		ISet<NestingIdentity> ancestry, ICollection<string> errors, string path)
	{
		var variant = table.Definition.Variants.SingleOrDefault(x => x.Key.EqualTo(variantKey));
		if (variant is null)
		{
			return 0;
		}

		var identity = new NestingIdentity(table.Id, table.RevisionNumber, variant.Key);
		if (!ancestry.Add(identity))
		{
			errors.Add($"Nesting validation failed at '{path}': a nested LootTable cycle was encountered.");
			return 0;
		}

		try
		{
			long total = 0;
			foreach (var group in variant.Groups)
			{
				long maximumChoice = 0;
				foreach (var choice in group.Choices)
				{
					var choicePath = $"{path}/{group.Key}/{choice.Key}";
					var generatedItems = choice.Kind switch
					{
						LootChoiceKind.Item => Math.Max(0, choice.QuantityMaximum),
						LootChoiceKind.Commodity => 1,
						LootChoiceKind.LootTable => MaximumNestedGeneratedItems(choice, gameworld, ancestry, errors,
							choicePath),
						_ => 0
					};
					maximumChoice = Math.Max(maximumChoice, generatedItems);
				}

				total = AddCapped(total, MultiplyCapped(Math.Max(0, group.RepeatMaximum), maximumChoice));
				if (total > LootTablePlanner.MaximumPlannedItems)
				{
					return total;
				}
			}

			return total;
		}
		finally
		{
			ancestry.Remove(identity);
		}
	}

	private static long MaximumNestedGeneratedItems(LootChoiceDefinition choice, IFuturemud gameworld,
		ISet<NestingIdentity> ancestry, ICollection<string> errors, string path)
	{
		var nested = gameworld.LootTables.Get(choice.NestedTableId, choice.NestedTableRevision);
		return nested is null
			? 0
			: MaximumGeneratedItems(nested, choice.NestedVariant, gameworld, ancestry, errors, path);
	}

	private static long AddCapped(long left, long right)
	{
		if (left > LootTablePlanner.MaximumPlannedItems || right > LootTablePlanner.MaximumPlannedItems ||
		    left > LootTablePlanner.MaximumPlannedItems - right)
		{
			return LootTablePlanner.MaximumPlannedItems + 1L;
		}

		return left + right;
	}

	private static long MultiplyCapped(long left, long right)
	{
		if (left == 0 || right == 0)
		{
			return 0;
		}

		return left > LootTablePlanner.MaximumPlannedItems || right > LootTablePlanner.MaximumPlannedItems ||
		       left > LootTablePlanner.MaximumPlannedItems / right
			? LootTablePlanner.MaximumPlannedItems + 1L
			: left * right;
	}

	private sealed record NestingIdentity(long TableId, int Revision, string Variant);
}
