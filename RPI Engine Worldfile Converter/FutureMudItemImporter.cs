#nullable enable

using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;
using DbFutureProg = MudSharp.Models.FutureProg;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudComponentReference(long Id, int RevisionNumber, string Name, string Type);

public sealed record FutureMudClanReference(long ClanId, string Alias, Dictionary<string, long> RankIds);

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
			var generatedComponentNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			if (definition.BoardDefinition is not null)
			{
				generatedComponentNames.Add(definition.BoardDefinition.ComponentName);
			}

			if (definition.DiceDefinition is not null)
			{
				generatedComponentNames.Add(definition.DiceDefinition.ComponentName);
			}

			if (!catalog.MaterialIds.ContainsKey(definition.MaterialName))
			{
				issues.Add(new FutureMudValidationIssue(definition.SourceKey, "error", $"Missing material '{definition.MaterialName}'."));
			}

			foreach (var component in definition.ComponentNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!generatedComponentNames.Contains(component) && !catalog.Components.ContainsKey(component))
				{
					issues.Add(new FutureMudValidationIssue(definition.SourceKey, "error", $"Missing component '{component}'."));
				}
			}

			if (definition.BoardDefinition is { ClanRestrictions.Count: 0 } && !catalog.FutureProgIds.ContainsKey("AlwaysTrue"))
			{
				issues.Add(new FutureMudValidationIssue(definition.SourceKey, "error", "Missing FutureProg 'AlwaysTrue' required for generated board components."));
			}

			if (definition.BoardDefinition is { ClanRestrictions.Count: > 0 } boardDefinition)
			{
				foreach (var restriction in boardDefinition.ClanRestrictions)
				{
					if (!catalog.ClansByAlias.TryGetValue(restriction.ClanAlias, out var clan))
					{
						issues.Add(new FutureMudValidationIssue(
							definition.SourceKey,
							"error",
							$"Missing imported clan alias '{restriction.ClanAlias}' required by board '{boardDefinition.BoardName}'. Run apply-clans before apply-items so board access can be bound to stable clan ids."));
						continue;
					}

					if (!IsMembershipOnlyBoardRank(restriction.RankName) && !clan.RankIds.ContainsKey(restriction.RankName))
					{
						issues.Add(new FutureMudValidationIssue(
							definition.SourceKey,
							"error",
							$"Missing imported clan rank '{restriction.RankName}' in clan alias '{restriction.ClanAlias}' required by board '{boardDefinition.BoardName}'. Run apply-clans before apply-items so board access can be bound to stable clan rank ids."));
					}
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

	private static bool IsMembershipOnlyBoardRank(string rankName)
	{
		return string.IsNullOrWhiteSpace(rankName) ||
		       rankName.Equals("member", StringComparison.OrdinalIgnoreCase) ||
		       rankName.Equals("membership", StringComparison.OrdinalIgnoreCase) ||
		       rankName.Equals("all", StringComparison.OrdinalIgnoreCase);
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
	public required Dictionary<string, FutureMudClanReference> ClansByAlias { get; init; }
	public Dictionary<string, long> FutureProgIds { get; init; } = new(StringComparer.OrdinalIgnoreCase);

	public static FutureMudBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var components = context.GameItemComponentProtos
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem == null || x.EditableItem.RevisionStatus == 4)
			.ToList();
		var uniqueComponents = components
			.Where(x => !string.IsNullOrWhiteSpace(x.Name))
			.GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.Select(x => x
				.OrderBy(y => y.Id)
				.ThenBy(y => y.RevisionNumber)
				.First())
			.ToList();

		return new FutureMudBaselineCatalog
		{
			Components = uniqueComponents.ToDictionary(
				x => x.Name,
				x => new FutureMudComponentReference(x.Id, x.RevisionNumber, x.Name, x.Type),
				StringComparer.OrdinalIgnoreCase),
			ComponentsByType = uniqueComponents
				.GroupBy(x => x.Type, StringComparer.OrdinalIgnoreCase)
				.ToDictionary(
					x => x.Key,
					x => x.Select(y => y.Name).OrderBy(y => y, StringComparer.OrdinalIgnoreCase).ToList(),
					StringComparer.OrdinalIgnoreCase),
			MaterialIds = ToUniqueIdDictionary(context.Materials, x => x.Name, x => x.Id),
			TagIds = ToUniqueIdDictionary(context.Tags, x => x.Name, x => x.Id),
			LiquidIds = ToUniqueIdDictionary(context.Liquids, x => x.Name, x => x.Id),
			TraitIds = ToUniqueIdDictionary(context.TraitDefinitions, x => x.Name, x => x.Id),
			ClansByAlias = LoadClansByAlias(context),
			FutureProgIds = ToUniqueIdDictionary(context.FutureProgs, x => x.FunctionName, x => x.Id)
		};
	}

	private static Dictionary<string, FutureMudClanReference> LoadClansByAlias(FuturemudDatabaseContext context)
	{
		var result = new Dictionary<string, FutureMudClanReference>(StringComparer.OrdinalIgnoreCase);
		var clans = context.Clans
			.Include(x => x.Ranks)
				.ThenInclude(x => x.RanksTitles)
			.Include(x => x.Ranks)
				.ThenInclude(x => x.RanksAbbreviations)
			.ToList()
			.Where(x => !string.IsNullOrWhiteSpace(x.Alias))
			.GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase)
			.Select(x => x.OrderBy(y => y.Id).First());

		foreach (var clan in clans)
		{
			var reference = new FutureMudClanReference(
				clan.Id,
				clan.Alias,
				clan.Ranks
					.SelectMany(rank =>
						rank.RanksTitles.Select(title => (name: title.Title, rank.Id))
							.Concat(rank.RanksAbbreviations.Select(abbreviation => (name: abbreviation.Abbreviation, rank.Id)))
							.Append((name: rank.Name, Id: rank.Id)))
					.Where(y => !string.IsNullOrWhiteSpace(y.name))
					.GroupBy(y => y.name, StringComparer.OrdinalIgnoreCase)
					.ToDictionary(y => y.Key, y => y.First().Id, StringComparer.OrdinalIgnoreCase));

			TryRegisterClanReference(result, clan.Alias, reference);
			TryRegisterClanReference(result, RpiClanAliasResolver.CollapseAlias(clan.Alias), reference);
			TryRegisterClanReference(result, RpiClanAliasResolver.ResolveCanonicalRule(clan.Alias).CanonicalAlias, reference);
		}

		return result;
	}

	private static void TryRegisterClanReference(
		IDictionary<string, FutureMudClanReference> clans,
		string alias,
		FutureMudClanReference reference)
	{
		if (string.IsNullOrWhiteSpace(alias) || clans.ContainsKey(alias))
		{
			return;
		}

		clans[alias] = reference;
	}

	private static Dictionary<string, long> ToUniqueIdDictionary<T>(
		IEnumerable<T> values,
		Func<T, string?> nameSelector,
		Func<T, long> idSelector)
	{
		return values
			.Where(x => !string.IsNullOrWhiteSpace(nameSelector(x)))
			.GroupBy(x => nameSelector(x)!, StringComparer.OrdinalIgnoreCase)
			.Select(x => x.OrderBy(idSelector).First())
			.ToDictionary(
				x => nameSelector(x)!,
				idSelector,
				StringComparer.OrdinalIgnoreCase);
	}

	public bool HasComponent(string name)
	{
		return Components.ContainsKey(name);
	}

	public void RegisterComponent(FutureMudComponentReference component)
	{
		Components[component.Name] = component;
		if (!ComponentsByType.TryGetValue(component.Type, out var componentsOfType))
		{
			componentsOfType = [];
			ComponentsByType[component.Type] = componentsOfType;
		}

		if (!componentsOfType.Contains(component.Name, StringComparer.OrdinalIgnoreCase))
		{
			componentsOfType.Add(component.Name);
			componentsOfType.Sort(StringComparer.OrdinalIgnoreCase);
		}
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
	private static readonly Regex NonProgNameRegex = new("[^a-z0-9_]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudBaselineCatalog _catalog;
	private readonly long _builderAccountId;
	private long _nextItemId;
	private long _nextComponentId;
	private long _nextBoardId;
	private readonly DateTime _now;

	private sealed record ResolvedBoardClanRestriction(long ClanId, long? RankId);

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
		_nextComponentId = context.GameItemComponentProtos.Any() ? context.GameItemComponentProtos.Max(x => x.Id) + 1 : 1;
		_nextBoardId = context.Boards.Any() ? context.Boards.Max(x => x.Id) + 1 : 1;
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

			EnsureGeneratedDependencies(definition);

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
				CustomColour = definition.CustomColour ?? string.Empty,
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

	private void EnsureGeneratedDependencies(ConvertedItemDefinition definition)
	{
		if (definition.BoardDefinition is not null)
		{
			EnsureBoardComponent(definition.BoardDefinition);
		}

		if (definition.DiceDefinition is not null)
		{
			EnsureDiceComponent(definition.DiceDefinition);
		}
	}

	private void EnsureDiceComponent(FutureMudDiceDefinition diceDefinition)
	{
		if (_catalog.HasComponent(diceDefinition.ComponentName))
		{
			return;
		}

		var existingComponent = _context.GameItemComponentProtos
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.FirstOrDefault(x =>
				x.Name.Equals(diceDefinition.ComponentName, StringComparison.OrdinalIgnoreCase) &&
				x.Type.Equals("Dice", StringComparison.OrdinalIgnoreCase) &&
				x.EditableItem.RevisionStatus == 4);
		if (existingComponent is not null)
		{
			_catalog.RegisterComponent(new FutureMudComponentReference(
				existingComponent.Id,
				existingComponent.RevisionNumber,
				existingComponent.Name,
				existingComponent.Type));
			return;
		}

		var component = new GameItemComponentProto
		{
			Id = _nextComponentId++,
			RevisionNumber = 0,
			Type = "Dice",
			Name = diceDefinition.ComponentName,
			Description = $"Generated dice component for imported RPI tossable with {diceDefinition.Faces.Count.ToString()} face(s).",
			Definition = BuildDiceComponentDefinition(diceDefinition),
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _builderAccountId,
				BuilderDate = _now,
				BuilderComment = $"RPIIMPORT|generated-dice-component|{diceDefinition.ComponentName}",
				ReviewerAccountId = _builderAccountId,
				ReviewerDate = _now,
				ReviewerComment = "Generated by the RPI Engine Worldfile Converter."
			}
		};
		_context.GameItemComponentProtos.Add(component);
		_catalog.RegisterComponent(new FutureMudComponentReference(component.Id, component.RevisionNumber, component.Name, component.Type));
	}

	private void EnsureBoardComponent(FutureMudBoardDefinition boardDefinition)
	{
		if (_catalog.HasComponent(boardDefinition.ComponentName))
		{
			return;
		}

		var existingComponent = _context.GameItemComponentProtos
			.Include(x => x.EditableItem)
			.AsEnumerable()
			.FirstOrDefault(x =>
				x.Name.Equals(boardDefinition.ComponentName, StringComparison.OrdinalIgnoreCase) &&
				x.Type.Equals("Board", StringComparison.OrdinalIgnoreCase) &&
				x.EditableItem.RevisionStatus == 4);
		if (existingComponent is not null)
		{
			_catalog.RegisterComponent(new FutureMudComponentReference(
				existingComponent.Id,
				existingComponent.RevisionNumber,
				existingComponent.Name,
				existingComponent.Type));
			return;
		}

		var boardAccessProgId = EnsureBoardAccessFutureProg(boardDefinition);

		var board = _context.Boards.Local
			.FirstOrDefault(x => x.Name.Equals(boardDefinition.BoardName, StringComparison.OrdinalIgnoreCase))
			?? _context.Boards
				.AsEnumerable()
				.FirstOrDefault(x => x.Name.Equals(boardDefinition.BoardName, StringComparison.OrdinalIgnoreCase));
		if (board is null)
		{
			board = new Board
			{
				Id = _nextBoardId++,
				Name = boardDefinition.BoardName,
				ShowOnLogin = false
			};
			_context.Boards.Add(board);
		}

		var component = new GameItemComponentProto
		{
			Id = _nextComponentId++,
			RevisionNumber = 0,
			Type = "Board",
			Name = boardDefinition.ComponentName,
			Description = $"Generated board component for imported RPI board '{boardDefinition.LegacyBoardKey}'.",
			Definition = BuildBoardComponentDefinition(board.Id, boardAccessProgId),
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _builderAccountId,
				BuilderDate = _now,
				BuilderComment = $"RPIIMPORT|generated-board-component|{boardDefinition.LegacyBoardKey}",
				ReviewerAccountId = _builderAccountId,
				ReviewerDate = _now,
				ReviewerComment = "Generated by the RPI Engine Worldfile Converter."
			}
		};
		_context.GameItemComponentProtos.Add(component);
		_catalog.RegisterComponent(new FutureMudComponentReference(component.Id, component.RevisionNumber, component.Name, component.Type));
	}

	private long EnsureBoardAccessFutureProg(FutureMudBoardDefinition boardDefinition)
	{
		if (boardDefinition.ClanRestrictions.Count == 0)
		{
			if (_catalog.FutureProgIds.TryGetValue("AlwaysTrue", out var alwaysTrueId))
			{
				return alwaysTrueId;
			}

			throw new InvalidOperationException("Cannot create generated RPI board components because the target database is missing the AlwaysTrue FutureProg.");
		}

		var resolvedRestrictions = ResolveBoardClanRestrictions(boardDefinition);
		var functionName = BuildBoardAccessProgName(boardDefinition);
		if (_catalog.FutureProgIds.TryGetValue(functionName, out var existingId))
		{
			return existingId;
		}

		var existingProg = _context.FutureProgs
			.AsEnumerable()
			.FirstOrDefault(x => x.FunctionName.Equals(functionName, StringComparison.OrdinalIgnoreCase));
		if (existingProg is not null)
		{
			_catalog.FutureProgIds[existingProg.FunctionName] = existingProg.Id;
			return existingProg.Id;
		}

		var prog = new DbFutureProg
		{
			FunctionName = functionName,
			FunctionComment =
				$"Checks legacy RPI clan access restrictions for imported board '{boardDefinition.LegacyBoardKey}'.",
			FunctionText = BuildBoardAccessProgText(resolvedRestrictions),
			ReturnType = (long)ProgVariableTypes.Boolean,
			Category = "Items",
			Subcategory = "RPI Import Boards",
			Public = false,
			AcceptsAnyParameters = false,
			StaticType = (int)FutureProgStaticType.NotStatic
		};
		prog.FutureProgsParameters.Add(new FutureProgsParameter
		{
			FutureProg = prog,
			ParameterIndex = 0,
			ParameterType = (long)ProgVariableTypes.Character,
			ParameterName = "ch"
		});

		_context.FutureProgs.Add(prog);
		_context.SaveChanges();
		_catalog.FutureProgIds[functionName] = prog.Id;
		return prog.Id;
	}

	private IReadOnlyList<ResolvedBoardClanRestriction> ResolveBoardClanRestrictions(FutureMudBoardDefinition boardDefinition)
	{
		return boardDefinition.ClanRestrictions
			.Select(restriction =>
			{
				if (!_catalog.ClansByAlias.TryGetValue(restriction.ClanAlias, out var clan))
				{
					throw new InvalidOperationException(
						$"Cannot create generated board access FutureProg for '{boardDefinition.BoardName}' because clan alias '{restriction.ClanAlias}' is missing. Run apply-clans before apply-items.");
				}

				if (IsAnyMemberRank(restriction.RankName))
				{
					return new ResolvedBoardClanRestriction(clan.ClanId, null);
				}

				if (!clan.RankIds.TryGetValue(restriction.RankName, out var rankId))
				{
					throw new InvalidOperationException(
						$"Cannot create generated board access FutureProg for '{boardDefinition.BoardName}' because rank '{restriction.RankName}' is missing from clan alias '{restriction.ClanAlias}'. Run apply-clans before apply-items.");
				}

				return new ResolvedBoardClanRestriction(clan.ClanId, rankId);
			})
			.ToList();
	}

	private static string BuildBoardAccessProgName(FutureMudBoardDefinition boardDefinition)
	{
		var suffix = NonProgNameRegex
			.Replace(boardDefinition.LegacyBoardKey.ToLowerInvariant(), "_")
			.Trim('_');
		if (string.IsNullOrWhiteSpace(suffix))
		{
			suffix = "board";
		}

		if (suffix.Length > 70)
		{
			suffix = suffix[..70].Trim('_');
		}

		return $"RPIBoardAccess_{suffix}";
	}

	private static string BuildBoardAccessProgText(IReadOnlyList<ResolvedBoardClanRestriction> restrictions)
	{
		List<string> lines =
		[
			"var clan as clan",
			"var rank as rank"
		];

		foreach (var restriction in restrictions)
		{
			lines.Add($"clan = ToClan({restriction.ClanId})");
			lines.Add("if (not(isnull(@clan)))");
			if (restriction.RankId is null)
			{
				lines.Add("\tif (IsClanMember(@ch, @clan))");
				lines.Add("\t\treturn true");
				lines.Add("\tend if");
			}
			else
			{
				lines.Add($"\trank = ToRank({restriction.RankId.Value})");
				lines.Add("\tif (not(isnull(@rank)))");
				lines.Add("\t\tif (IsClanMember(@ch, @clan, @rank))");
				lines.Add("\t\t\treturn true");
				lines.Add("\t\tend if");
				lines.Add("\tend if");
			}
			lines.Add("end if");
		}

		lines.Add("return false");
		return string.Join('\n', lines);
	}

	private static bool IsAnyMemberRank(string rankName)
	{
		return string.IsNullOrWhiteSpace(rankName) ||
		       rankName.Equals("member", StringComparison.OrdinalIgnoreCase) ||
		       rankName.Equals("membership", StringComparison.OrdinalIgnoreCase) ||
		       rankName.Equals("all", StringComparison.OrdinalIgnoreCase);
	}

	private static string BuildBoardComponentDefinition(long boardId, long boardAccessProgId)
	{
		return new XElement("Definition",
			new XElement("Board", boardId),
			new XElement("CanViewBoard", boardAccessProgId),
			new XElement("CanPostToBoard", boardAccessProgId),
			new XElement("CantViewBoardEcho", new XCData("You are not permitted to view the posts associated with this board.")),
			new XElement("CantPostToBoardEcho", new XCData("You are not permitted to make posts to this board.")),
			new XElement("ShowAuthorName", false),
			new XElement("ShowAuthorDescription", false),
			new XElement("ShowAuthorShortDescription", false),
			new XElement("StoredAuthorName", new XCData(string.Empty)),
			new XElement("StoredAuthorShortDescription", new XCData(string.Empty)),
			new XElement("StoredAuthorFullDescription", new XCData(string.Empty))
		).ToString();
	}

	private static string BuildDiceComponentDefinition(FutureMudDiceDefinition diceDefinition)
	{
		return new XElement("Definition",
			new XElement("Faces",
				diceDefinition.Faces.Select(face => new XElement("Face", new XCData(face)))),
			new XElement("Weights",
				diceDefinition.Faces.Select((_, index) =>
					new XElement("Weight",
						new XElement("Face", index),
						new XElement("Probability", 1))))
		).ToString();
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
			$"Board={definition.BoardDefinition?.BoardName ?? "none"}",
			$"BoardAccessRestrictions={definition.BoardDefinition?.ClanRestrictions.Count ?? 0}",
			$"Dice={definition.DiceDefinition?.ComponentName ?? "none"}",
			$"CustomColour={definition.CustomColour ?? "none"}",
			$"Liquid={definition.LiquidReference?.LiquidName ?? definition.LiquidReference?.RawLiquidValue.ToString() ?? "none"}",
			$"Warnings={string.Join(" | ", definition.Warnings.Select(x => x.Message))}"
		]);
	}
}
