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

		var planner = new LootTablePlanner((id, revision) =>
		{
			var nested = gameworld.LootTables.Get(id, revision);
			return nested is null ? null : new LootTablePlanSource(nested.Id, nested.RevisionNumber, nested.DefinitionHash, nested.Definition);
		});
		foreach (var variant in definition.Variants)
		{
			var plan = planner.CreatePlan(new LootTablePlanSource(table.Id, table.RevisionNumber, table.DefinitionHash, table.Definition), variant.Key, 0);
			if (!plan.Success && plan.ErrorCode is "CYCLE" or "EXPANSION_LIMIT") errors.Add($"Nesting validation failed for '{variant.Key}': {plan.ErrorCode} {plan.ErrorMessage}");
		}
		return errors.Distinct().ToList();
	}
}
