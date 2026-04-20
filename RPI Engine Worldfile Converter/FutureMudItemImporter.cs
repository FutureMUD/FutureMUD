#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudComponentReference(long Id, int RevisionNumber, string Name, string Type);

public sealed record FutureMudValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudImportResult(
	int InsertedCount,
	int SkippedExistingCount,
	IReadOnlyList<FutureMudValidationIssue> Issues);

public static class FutureMudItemValidation
{
	public static IReadOnlyList<FutureMudValidationIssue> Validate(
		FutureMudBaselineCatalog catalog,
		IEnumerable<ConvertedItemDefinition> definitions)
	{
		List<FutureMudValidationIssue> issues = [];

		foreach (var definition in definitions)
		{
			if (!catalog.MaterialIds.ContainsKey(definition.MaterialName))
			{
				issues.Add(new FutureMudValidationIssue(definition.SourceKey, "error", $"Missing material '{definition.MaterialName}'."));
			}

			foreach (var component in definition.ComponentNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!catalog.Components.ContainsKey(component))
				{
					issues.Add(new FutureMudValidationIssue(definition.SourceKey, "error", $"Missing component '{component}'."));
				}
			}

			foreach (var tag in definition.TagNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!catalog.TagIds.ContainsKey(tag))
				{
					issues.Add(new FutureMudValidationIssue(definition.SourceKey, "warning", $"Missing optional tag '{tag}'."));
				}
			}

			foreach (var trait in definition.TraitReferences)
			{
				if (!catalog.TraitIds.ContainsKey(trait.TraitName))
				{
					issues.Add(new FutureMudValidationIssue(
						definition.SourceKey,
						"warning",
						$"Missing trait '{trait.TraitName}' for source skill {trait.SourceSkill}."));
				}
			}

			if (definition.LiquidReference?.LiquidName is not null &&
			    !catalog.LiquidIds.ContainsKey(definition.LiquidReference.LiquidName))
			{
				issues.Add(new FutureMudValidationIssue(
					definition.SourceKey,
					"warning",
					$"Missing liquid '{definition.LiquidReference.LiquidName}' for raw liquid value {definition.LiquidReference.RawLiquidValue}."));
			}
		}

		return issues;
	}
}

public sealed class FutureMudBaselineCatalog
{
	public required Dictionary<string, FutureMudComponentReference> Components { get; init; }
	public required Dictionary<string, List<string>> ComponentsByType { get; init; }
	public required Dictionary<string, long> MaterialIds { get; init; }
	public required Dictionary<string, long> TagIds { get; init; }
	public required Dictionary<string, long> LiquidIds { get; init; }
	public required Dictionary<string, long> TraitIds { get; init; }

	public static FutureMudBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var components = context.GameItemComponentProtos
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem == null || x.EditableItem.RevisionStatus == 4)
			.ToList();

		return new FutureMudBaselineCatalog
		{
			Components = components.ToDictionary(
				x => x.Name,
				x => new FutureMudComponentReference(x.Id, x.RevisionNumber, x.Name, x.Type),
				StringComparer.OrdinalIgnoreCase),
			ComponentsByType = components
				.GroupBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					x => x.Key,
					x => x.Select(y => y.Name).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList(),
					StringComparer.OrdinalIgnoreCase),
			MaterialIds = context.Materials.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			TagIds = context.Tags.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			LiquidIds = context.Liquids.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			TraitIds = context.TraitDefinitions.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase)
		};
	}

	public bool HasComponent(string name)
	{
		return Components.ContainsKey(name);
	}

	public string? ChooseComponent(IEnumerable<string> candidates)
	{
		foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (Components.ContainsKey(candidate))
			{
				return candidate;
			}
		}

		return null;
	}

	public FutureMudComponentReference? GetComponent(string name)
	{
		return Components.TryGetValue(name, out var component) ? component : null;
	}

	public string? FindFirstComponentByType(params string[] candidateTypes)
	{
		foreach (var candidateType in candidateTypes)
		{
			if (ComponentsByType.TryGetValue(candidateType, out var matches) && matches.Count > 0)
			{
				return matches[0];
			}
		}

		foreach (var candidateType in candidateTypes)
		{
			var fuzzyMatch = ComponentsByType
				.Where(x =>
					x.Key.Contains(candidateType, StringComparison.OrdinalIgnoreCase) ||
					candidateType.Contains(x.Key, StringComparison.OrdinalIgnoreCase))
				.SelectMany(x => x.Value)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.FirstOrDefault();

			if (!string.IsNullOrWhiteSpace(fuzzyMatch))
			{
				return fuzzyMatch;
			}
		}

		return null;
	}

	public string? ChooseMaterial(IEnumerable<string> candidates)
	{
		foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (MaterialIds.ContainsKey(candidate))
			{
				return candidate;
			}
		}

		return null;
	}

	public string? ChooseTag(IEnumerable<string> candidates)
	{
		foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (TagIds.ContainsKey(candidate))
			{
				return candidate;
			}
		}

		return null;
	}

	public string? ChooseLiquid(IEnumerable<string> candidates)
	{
		foreach (var candidate in candidates.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (LiquidIds.ContainsKey(candidate))
			{
				return candidate;
			}
		}

		return null;
	}
}

public sealed class FutureMudItemImporter
{
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudBaselineCatalog _catalog;
	private readonly long _builderAccountId;
	private long _nextItemId;
	private readonly DateTime _now;

	public FutureMudItemImporter(FuturemudDatabaseContext context, FutureMudBaselineCatalog catalog)
	{
		_context = context;
		_catalog = catalog;
		_builderAccountId = context.Accounts.Select(x => x.Id).FirstOrDefault();
		if (_builderAccountId == 0)
		{
			throw new InvalidOperationException("The target database does not contain any accounts to attribute imported items to.");
		}

		_nextItemId = context.GameItemProtos.Any() ? context.GameItemProtos.Max(x => x.Id) + 1 : 1;
		_now = DateTime.UtcNow;
	}

	public IReadOnlyList<FutureMudValidationIssue> Validate(IEnumerable<ConvertedItemDefinition> definitions)
	{
		return FutureMudItemValidation.Validate(_catalog, definitions);
	}

	public FutureMudImportResult Apply(IEnumerable<ConvertedItemDefinition> definitions, bool execute)
	{
		var ordered = definitions
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.ToList();

		var issues = Validate(ordered).ToList();
		var fatalIssues = issues.Where(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)).ToList();
		if (fatalIssues.Count > 0)
		{
			return new FutureMudImportResult(0, 0, issues);
		}

		var existingMarkers = LoadExistingMarkers();
		var skippedExistingCount = ordered.Count(x => existingMarkers.Contains(CreateProvenanceMarker(x)));
		if (!execute)
		{
			return new FutureMudImportResult(0, skippedExistingCount, issues);
		}

		var insertedCount = 0;

		foreach (var definition in ordered)
		{
			var marker = CreateProvenanceMarker(definition);
			if (existingMarkers.Contains(marker))
			{
				continue;
			}

			var proto = new GameItemProto
			{
				Id = _nextItemId++,
				RevisionNumber = 0,
				Name = definition.BaseName.ToLowerInvariant(),
				Keywords = definition.Keywords,
				MaterialId = _catalog.MaterialIds[definition.MaterialName],
				EditableItem = new EditableItem
				{
					RevisionNumber = 0,
					RevisionStatus = 4,
					BuilderAccountId = _builderAccountId,
					BuilderDate = _now,
					BuilderComment = BuildBuilderComment(definition),
					ReviewerAccountId = _builderAccountId,
					ReviewerDate = _now,
					ReviewerComment = "Imported by the RPI Engine Worldfile Converter."
				},
				Size = definition.Size,
				Weight = definition.WeightGrams,
				ReadOnly = false,
				LongDescription = string.IsNullOrWhiteSpace(definition.LongDescription) ? null : definition.LongDescription,
				BaseItemQuality = definition.BaseItemQuality,
				ShortDescription = definition.ShortDescription,
				FullDescription = definition.FullDescription,
				PermitPlayerSkins = definition.PermitPlayerSkins,
				CostInBaseCurrency = definition.CostInBaseCurrency,
				IsHiddenFromPlayers = false,
				HighPriority = false
			};

			foreach (var componentName in definition.ComponentNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				var component = _catalog.GetComponent(componentName)!;
				proto.GameItemProtosGameItemComponentProtos.Add(new GameItemProtosGameItemComponentProtos
				{
					GameItemProto = proto,
					GameItemComponentProtoId = component.Id,
					GameItemProtoRevision = proto.RevisionNumber,
					GameItemComponentRevision = component.RevisionNumber
				});
			}

			foreach (var tagName in definition.TagNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!_catalog.TagIds.TryGetValue(tagName, out var tagId))
				{
					continue;
				}

				proto.GameItemProtosTags.Add(new GameItemProtosTags
				{
					GameItemProto = proto,
					TagId = tagId,
					GameItemProtoRevisionNumber = proto.RevisionNumber
				});
			}

			_context.GameItemProtos.Add(proto);
			insertedCount++;
			existingMarkers.Add(marker);

			if (insertedCount % 100 == 0)
			{
				_context.SaveChanges();
			}
		}

		_context.SaveChanges();
		return new FutureMudImportResult(insertedCount, skippedExistingCount, issues);
	}

	private HashSet<string> LoadExistingMarkers()
	{
		return _context.GameItemProtos
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem != null && x.EditableItem.BuilderComment != null && x.EditableItem.BuilderComment.StartsWith("RPIIMPORT|"))
			.AsEnumerable()
			.Select(x => x.EditableItem!.BuilderComment.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty)
			.Where(x => x.StartsWith("RPIIMPORT|", StringComparison.Ordinal))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string CreateProvenanceMarker(ConvertedItemDefinition definition)
	{
		return $"RPIIMPORT|{Path.GetFileName(definition.SourceFile)}|{definition.Vnum}|{definition.SourceItemType}|{string.Join(",", definition.RawOvals)}";
	}

	private static string BuildBuilderComment(ConvertedItemDefinition definition)
	{
		return string.Join('\n',
		[
			CreateProvenanceMarker(definition),
			$"Status={definition.Status}",
			$"Material={definition.MaterialName}",
			$"Components={string.Join(",", definition.ComponentNames)}",
			$"Tags={string.Join(",", definition.TagNames)}",
			$"Liquid={definition.LiquidReference?.LiquidName ?? definition.LiquidReference?.RawLiquidValue.ToString() ?? "none"}",
			$"Warnings={string.Join(" | ", definition.Warnings.Select(x => x.Message))}"
		]);
	}
}
