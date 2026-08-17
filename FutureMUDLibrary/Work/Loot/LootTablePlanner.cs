#nullable enable

using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MudSharp.Work.Loot;

public sealed record LootTablePlanSource(long Id, int Revision, string Hash, LootTableDefinition Definition);

public sealed record LootPlannedLeaf(
	LootChoiceKind Kind,
	string Path,
	string DestinationKey,
	string? ResultKey,
	long ItemPrototypeId,
	int ItemPrototypeRevision,
	int Quantity,
	int Quality,
	bool StartsClosed,
	bool StartsLocked,
	IReadOnlyList<LootCharacteristicValue> Characteristics,
	long CommodityMaterialId,
	long? CommodityTagId,
	double Mass);

public sealed class LootTablePlan
{
	public required LootTablePlanSource Root { get; init; }
	public required string Variant { get; init; }
	public required long Seed { get; init; }
	public required IReadOnlyList<LootPlannedLeaf> Leaves { get; init; }
	public required string Digest { get; init; }
}

public sealed class LootTablePlanResult
{
	public LootTablePlan? Plan { get; init; }
	public string? ErrorCode { get; init; }
	public string? ErrorMessage { get; init; }
	public bool Success => Plan is not null;
}

public sealed class LootTablePlanner
{
	public const int MaximumPlannedItems = 1000;
	public const int MaximumPlannedLeaves = MaximumPlannedItems;

	private readonly Func<long, int, LootTablePlanSource?> _resolver;

	public LootTablePlanner(Func<long, int, LootTablePlanSource?> resolver)
	{
		_resolver = resolver;
	}

	public LootTablePlanResult CreatePlan(LootTablePlanSource root, string variant, long seed)
	{
		if (seed < 0)
		{
			return Error("INVALID_SEED", "The seed must be non-negative.");
		}

		if (root.Definition.AlgorithmVersion != LootTableDefinition.CurrentAlgorithmVersion)
		{
			return Error("UNSUPPORTED_ALGORITHM", "The LootTable algorithm version is not supported.");
		}

		try
		{
			var leaves = new List<LootPlannedLeaf>();
			var ancestry = new HashSet<(long Id, int Revision, string Variant)>();
			var plannedItemCount = 0;
			var rootIdentity = $"a:{root.Definition.AlgorithmVersion}|root:{root.Id}r{root.Revision}|seed:{seed}";
			Expand(root, variant, seed, rootIdentity, "target", ancestry, leaves, ref plannedItemCount);

			return new LootTablePlanResult
			{
				Plan = new LootTablePlan
				{
					Root = root,
					Variant = variant,
					Seed = seed,
					Leaves = leaves,
					Digest = ComputePlanDigest(leaves)
				}
			};
		}
		catch (LootPlanningException ex)
		{
			return Error(ex.Code, ex.Message);
		}
		catch (OverflowException)
		{
			return Error("WEIGHT_OVERFLOW", "Choice weights exceed the supported range.");
		}
	}

	private void Expand(
		LootTablePlanSource source,
		string variantKey,
		long seed,
		string path,
		string targetDestination,
		HashSet<(long Id, int Revision, string Variant)> ancestry,
		List<LootPlannedLeaf> leaves,
		ref int plannedItemCount)
	{
		var variant = source.Definition.Variants.SingleOrDefault(x =>
			x.Key.Equals(variantKey, StringComparison.OrdinalIgnoreCase));
		if (variant is null)
		{
			throw new LootPlanningException("VARIANT_NOT_FOUND", $"Variant {variantKey} does not exist.");
		}

		var identity = (source.Id, source.Revision, variant.Key);
		if (!ancestry.Add(identity))
		{
			throw new LootPlanningException("CYCLE", "A nested LootTable cycle was encountered.");
		}

		try
		{
			var scope = $"{path}|table:{source.Id}r{source.Revision}|variant:{variant.Key}";
			foreach (var group in variant.Groups)
			{
				if (group.RepeatMinimum < 0 || group.RepeatMaximum < group.RepeatMinimum)
				{
					throw new LootPlanningException("INVALID_REPEAT", $"Group {group.Key} has an invalid repetition range.");
				}

				if (group.Choices.Count == 0 || group.Choices.Any(x => x.Weight <= 0))
				{
					throw new LootPlanningException("INVALID_CHOICES", $"Group {group.Key} has invalid choices.");
				}

				var groupPath = $"{scope}|group:{group.Key}";
				var repetitions = DrawIntInclusive(seed, groupPath + "|field:repeat", group.RepeatMinimum,
					group.RepeatMaximum);
				var destination = group.DestinationKey.Equals("target", StringComparison.OrdinalIgnoreCase)
					? targetDestination
					: $"{scope}|key:{group.DestinationKey}";

				for (var repetition = 0; repetition < repetitions; repetition++)
				{
					var repetitionPath = $"{groupPath}|repetition:{repetition}";
					var choice = SelectChoice(seed, repetitionPath, group.Choices);
					var choicePath = $"{repetitionPath}|choice:{choice.Key}";
					switch (choice.Kind)
					{
					case LootChoiceKind.Nothing:
						break;
					case LootChoiceKind.Item:
						ValidateItemQuantityRange(choice.QuantityMinimum, choice.QuantityMaximum);
						ValidateIntegerRange(choice.QualityMinimum, choice.QualityMaximum, "INVALID_QUALITY");
						var quantity = DrawIntInclusive(seed, choicePath + "|field:quantity", choice.QuantityMinimum,
							choice.QuantityMaximum);
						AddLeaf(leaves, new LootPlannedLeaf(
							LootChoiceKind.Item,
								choicePath,
								destination,
								string.IsNullOrEmpty(choice.ResultKey) ? null : $"{scope}|key:{choice.ResultKey}",
								choice.ItemPrototypeId,
								choice.ItemPrototypeRevision,
							quantity,
								DrawIntInclusive(seed, choicePath + "|field:quality", choice.QualityMinimum,
									choice.QualityMaximum),
								choice.StartsClosed,
								choice.StartsLocked,
								choice.Characteristics.Select(x => new LootCharacteristicValue
								{
									DefinitionId = x.DefinitionId,
									ValueId = x.ValueId
								}).ToList(),
							0,
							null,
							0.0), quantity, ref plannedItemCount);
						break;
						case LootChoiceKind.Commodity:
							if (!double.IsFinite(choice.MassMinimum) || !double.IsFinite(choice.MassMaximum) ||
							    choice.MassMinimum <= 0.0 || choice.MassMaximum < choice.MassMinimum)
							{
								throw new LootPlanningException("INVALID_MASS", "A commodity mass range is invalid.");
							}

						AddLeaf(leaves, new LootPlannedLeaf(
								LootChoiceKind.Commodity,
								choicePath,
								destination,
								null,
								0,
								0,
								1,
								0,
								false,
								false,
								[],
								choice.CommodityMaterialId,
							choice.CommodityTagId,
							DrawDoubleInclusive(seed, choicePath + "|field:mass", choice.MassMinimum,
								choice.MassMaximum)), 1, ref plannedItemCount);
							break;
						case LootChoiceKind.LootTable:
							var nested = _resolver(choice.NestedTableId, choice.NestedTableRevision);
							if (nested is null)
							{
								throw new LootPlanningException("TABLE_NOT_FOUND",
									$"Nested LootTable {choice.NestedTableId}r{choice.NestedTableRevision} does not exist.");
							}

						Expand(nested, choice.NestedVariant, seed, choicePath, destination, ancestry, leaves,
							ref plannedItemCount);
						break;
				}
				}
			}
		}
		finally
		{
			ancestry.Remove(identity);
		}
	}

	private static void AddLeaf(List<LootPlannedLeaf> leaves, LootPlannedLeaf leaf, int itemCount,
		ref int plannedItemCount)
	{
		try
		{
			plannedItemCount = checked(plannedItemCount + itemCount);
		}
		catch (OverflowException)
		{
			throw new LootPlanningException("EXPANSION_LIMIT",
				$"The realised plan exceeds {MaximumPlannedItems} generated items.");
		}

		if (plannedItemCount > MaximumPlannedItems)
		{
			throw new LootPlanningException("EXPANSION_LIMIT",
				$"The realised plan exceeds {MaximumPlannedItems} generated items.");
		}

		leaves.Add(leaf);
	}

	private static LootChoiceDefinition SelectChoice(long seed, string path, IReadOnlyList<LootChoiceDefinition> choices)
	{
		var total = choices.Aggregate(0L, (current, choice) => checked(current + choice.Weight));
		var choiceKeys = string.Join(',', choices.Select(x => x.Key));
		var draw = (long)DrawBounded(seed, $"{path}|choices:{choiceKeys}|field:choice", (ulong)total);
		long boundary = 0;
		foreach (var choice in choices)
		{
			boundary = checked(boundary + choice.Weight);
			if (draw < boundary)
			{
				return choice;
			}
		}

		throw new InvalidOperationException("Weighted selection did not resolve a choice.");
	}

	public static ulong DrawBounded(long seed, string semanticPath, ulong upperExclusive)
	{
		if (seed < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(seed));
		}

		if (upperExclusive == 0)
		{
			throw new ArgumentOutOfRangeException(nameof(upperExclusive));
		}

		var threshold = unchecked(0UL - upperExclusive) % upperExclusive;
		for (var attempt = 0; ; attempt++)
		{
			var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(
				$"loot-v1|seed:{seed}|path:{semanticPath}|attempt:{attempt}"));
			var value = BinaryPrimitives.ReadUInt64BigEndian(bytes);
			if (value >= threshold)
			{
				return value % upperExclusive;
			}
		}
	}

	private static int DrawIntInclusive(long seed, string path, int minimum, int maximum)
	{
		ValidateIntegerRange(minimum, maximum, "INVALID_RANGE");
		var width = checked((ulong)((long)maximum - minimum) + 1UL);
		return checked(minimum + (int)DrawBounded(seed, path, width));
	}

	private static double DrawDoubleInclusive(long seed, string path, double minimum, double maximum)
	{
		if (minimum == maximum)
		{
			return minimum;
		}

		var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"loot-v1|seed:{seed}|path:{path}"));
		var value = BinaryPrimitives.ReadUInt64BigEndian(bytes);
		var fraction = value / (double)ulong.MaxValue;
		return minimum + (maximum - minimum) * fraction;
	}

	private static void ValidateIntegerRange(int minimum, int maximum, string code)
	{
		if (minimum < 0 || maximum < minimum)
		{
			throw new LootPlanningException(code, "An integer range is invalid.");
		}
	}

	private static void ValidateItemQuantityRange(int minimum, int maximum)
	{
		if (minimum < 1 || maximum < minimum)
		{
			throw new LootPlanningException("INVALID_QUANTITY", "An item quantity range is invalid.");
		}
	}

	private static string ComputePlanDigest(IEnumerable<LootPlannedLeaf> leaves)
	{
		var canonical = string.Join('\n', leaves.Select(leaf => string.Join('|',
			leaf.Kind,
			leaf.Path,
			leaf.DestinationKey,
			leaf.ResultKey ?? string.Empty,
			leaf.ItemPrototypeId.ToString(CultureInfo.InvariantCulture),
			leaf.ItemPrototypeRevision.ToString(CultureInfo.InvariantCulture),
			leaf.Quantity.ToString(CultureInfo.InvariantCulture),
			leaf.Quality.ToString(CultureInfo.InvariantCulture),
			leaf.StartsClosed.ToString(CultureInfo.InvariantCulture),
			leaf.StartsLocked.ToString(CultureInfo.InvariantCulture),
			string.Join(',', leaf.Characteristics.OrderBy(x => x.DefinitionId).ThenBy(x => x.ValueId)
				.Select(x => $"{x.DefinitionId}:{x.ValueId}")),
			leaf.CommodityMaterialId.ToString(CultureInfo.InvariantCulture),
			leaf.CommodityTagId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
			leaf.Mass.ToString("R", CultureInfo.InvariantCulture))));
		return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
	}

	private static LootTablePlanResult Error(string code, string message) => new()
	{
		ErrorCode = code,
		ErrorMessage = message
	};

	private sealed class LootPlanningException(string code, string message) : Exception(message)
	{
		public string Code { get; } = code;
	}
}
