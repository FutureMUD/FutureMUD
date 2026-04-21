#nullable enable

using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.GameItems.Inventory.Plans;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using DbFutureProg = MudSharp.Models.FutureProg;
using SystemCultureInfo = System.Globalization.CultureInfo;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudCraftValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudCraftImportedItemReference(
	long ProtoId,
	int RevisionNumber,
	string ShortDescription,
	HashSet<long> TagIds);

public sealed record FutureMudCraftClanReference(
	long ClanId,
	string Alias,
	Dictionary<string, long> RankIds);

public sealed record CraftApplyAuditTagEntry(
	string TagName,
	string Action,
	long? TagId,
	IReadOnlyList<int> SourceVnums);

public sealed record CraftApplyAuditProgEntry(
	string FunctionName,
	string Kind,
	string Action,
	long? ProgId);

public sealed record CraftApplyAuditCraftEntry(
	string SourceKey,
	int CraftNumber,
	string Action,
	long? CraftId,
	IReadOnlyList<string> GeneratedTags,
	IReadOnlyList<string> FutureProgNames);

public sealed record CraftApplyAuditReport(
	DateTime GeneratedUtc,
	bool Execute,
	IReadOnlyList<CraftApplyAuditTagEntry> Tags,
	IReadOnlyList<CraftApplyAuditProgEntry> Progs,
	IReadOnlyList<CraftApplyAuditCraftEntry> Crafts);

public sealed record FutureMudCraftImportResult(
	int InsertedCount,
	int SkippedExistingCount,
	int SkippedDeferredCount,
	int SkippedInvalidCount,
	int CreatedTagCount,
	int CreatedProgCount,
	IReadOnlyList<FutureMudCraftValidationIssue> Issues,
	CraftApplyAuditReport Audit);

public sealed class FutureMudCraftBaselineCatalog
{
	public required long BuilderAccountId { get; init; }
	public required Dictionary<string, long> TraitIds { get; init; }
	public required Dictionary<string, long> TagIds { get; init; }
	public required Dictionary<string, long> RaceIds { get; init; }
	public required Dictionary<string, long> TerrainIds { get; init; }
	public required Dictionary<string, long> WeatherIdsByNormalisedName { get; init; }
	public required Dictionary<string, FutureMudCraftClanReference> ClansByAlias { get; init; }
	public required Dictionary<int, FutureMudCraftImportedItemReference> ImportedItemsBySourceVnum { get; init; }
	public required Dictionary<string, long> FutureProgIds { get; init; }

	public static FutureMudCraftBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var builderAccountId = context.Accounts
			.Select(x => x.Id)
			.OrderBy(x => x)
			.FirstOrDefault();
		if (builderAccountId == 0)
		{
			throw new InvalidOperationException("The target database does not contain any accounts to attribute imported crafts to.");
		}

		var importedItems = context.GameItemProtos
			.Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosTags)
			.Where(x => x.EditableItem != null &&
			            x.EditableItem.BuilderComment != null &&
			            x.EditableItem.BuilderComment.StartsWith("RPIIMPORT|"))
			.AsEnumerable()
			.Select(x => (proto: x, marker: x.EditableItem!.BuilderComment.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty))
			.Where(x => x.marker.StartsWith("RPIIMPORT|", StringComparison.Ordinal))
			.Select(x =>
			{
				var parts = x.marker.Split('|', StringSplitOptions.RemoveEmptyEntries);
				if (parts.Length < 3 || !int.TryParse(parts[2], System.Globalization.NumberStyles.Integer, SystemCultureInfo.InvariantCulture, out var vnum))
				{
					return (vnum: (int?)null, reference: (FutureMudCraftImportedItemReference?)null);
				}

				return (vnum: (int?)vnum, reference: new FutureMudCraftImportedItemReference(
					x.proto.Id,
					x.proto.RevisionNumber,
					x.proto.ShortDescription,
					x.proto.GameItemProtosTags.Select(y => y.TagId).ToHashSet()));
			})
			.Where(x => x.vnum.HasValue && x.reference is not null)
			.ToDictionary(x => x.vnum!.Value, x => x.reference!, EqualityComparer<int>.Default);

		var clans = context.Clans
			.Include(x => x.Ranks)
				.ThenInclude(x => x.RanksTitles)
			.Include(x => x.Ranks)
				.ThenInclude(x => x.RanksAbbreviations)
			.ToList()
			.ToDictionary(
				x => x.Alias,
				x => new FutureMudCraftClanReference(
					x.Id,
					x.Alias,
					x.Ranks
						.SelectMany(rank =>
							rank.RanksTitles.Select(title => (name: title.Title, rank.Id))
								.Concat(rank.RanksAbbreviations.Select(abbreviation => (name: abbreviation.Abbreviation, rank.Id)))
								.Append((name: rank.Name, Id: rank.Id)))
						.GroupBy(y => y.name, StringComparer.OrdinalIgnoreCase)
						.ToDictionary(y => y.Key, y => y.First().Id, StringComparer.OrdinalIgnoreCase)),
				StringComparer.OrdinalIgnoreCase);

		return new FutureMudCraftBaselineCatalog
		{
			BuilderAccountId = builderAccountId,
			TraitIds = context.TraitDefinitions.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			TagIds = context.Tags.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			RaceIds = context.Races.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			TerrainIds = context.Terrains.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			WeatherIdsByNormalisedName = context.WeatherEvents
				.ToDictionary(x => NormaliseLookupKey(x.Name), x => x.Id, StringComparer.OrdinalIgnoreCase),
			ClansByAlias = clans,
			ImportedItemsBySourceVnum = importedItems,
			FutureProgIds = context.FutureProgs.ToDictionary(x => x.FunctionName, x => x.Id, StringComparer.OrdinalIgnoreCase),
		};
	}

	public bool TryGetWeatherId(string weatherName, out long id)
	{
		return WeatherIdsByNormalisedName.TryGetValue(NormaliseLookupKey(weatherName), out id);
	}

	internal static string NormaliseLookupKey(string value)
	{
		return new string(value
			.ToLowerInvariant()
			.Where(char.IsLetterOrDigit)
			.ToArray());
	}
}

public static class FutureMudCraftValidation
{
	public static IReadOnlyList<FutureMudCraftValidationIssue> Validate(
		FutureMudCraftBaselineCatalog baseline,
		IEnumerable<ConvertedCraftDefinition> definitions)
	{
		List<FutureMudCraftValidationIssue> issues = [];

		foreach (var definition in definitions)
		{
			if (definition.Phases.Count == 0)
			{
				issues.Add(new FutureMudCraftValidationIssue(definition.SourceKey, "error", "Craft has no importable phases."));
			}

			if (definition.Status == CraftConversionStatus.Deferred)
			{
				issues.Add(new FutureMudCraftValidationIssue(
					definition.SourceKey,
					"warning",
					"Craft is deferred in this pass and will be skipped by apply-crafts."));
				continue;
			}

			if (definition.PrimaryCheck is not null &&
			    !baseline.TraitIds.ContainsKey(definition.PrimaryCheck.TraitName))
			{
				issues.Add(new FutureMudCraftValidationIssue(
					definition.SourceKey,
					"error",
					$"Missing primary check trait '{definition.PrimaryCheck.TraitName}'."));
			}

			foreach (var openingTrait in definition.Constraints.OpeningTraitNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!baseline.TraitIds.ContainsKey(openingTrait))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing opening trait '{openingTrait}'."));
				}
			}

			foreach (var terrain in definition.Constraints.TerrainNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!baseline.TerrainIds.ContainsKey(terrain))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing terrain '{terrain}'."));
				}
			}

			foreach (var race in definition.Constraints.RaceNames.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!baseline.RaceIds.ContainsKey(race))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing race '{race}'."));
				}
			}

			foreach (var weather in definition.Constraints.WeatherStates.Distinct(StringComparer.OrdinalIgnoreCase))
			{
				if (!baseline.TryGetWeatherId(weather, out _))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing weather event '{weather}'."));
				}
			}

			foreach (var clanRequirement in definition.Constraints.ClanRequirements)
			{
				if (!baseline.ClansByAlias.TryGetValue(clanRequirement.CanonicalAlias, out var clan))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing clan '{clanRequirement.CanonicalAlias}'."));
					continue;
				}

				if (!CanSatisfyClanRequirement(clan, clanRequirement.RankName))
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing clan rank '{clanRequirement.RankName}' for clan '{clanRequirement.CanonicalAlias}'."));
				}
			}

			foreach (var input in definition.Inputs)
			{
				ValidateItemReference(baseline, definition.SourceKey, input.SourceVnum, input.SourceSlot, "input", issues);
				ValidateTagReference(baseline, definition.SourceKey, input.TagName, input.UsesGeneratedTag, input.SourceSlot, "input", issues);
			}

			foreach (var tool in definition.Tools)
			{
				ValidateItemReference(baseline, definition.SourceKey, tool.SourceVnum, tool.SourceSlot, "tool", issues);
				ValidateTagReference(baseline, definition.SourceKey, tool.TagName, tool.UsesGeneratedTag, tool.SourceSlot, "tool", issues);
			}

			foreach (var product in definition.Products)
			{
				if (product.ProductType == "SimpleProduct")
				{
					ValidateItemReference(baseline, definition.SourceKey, product.OutputVnum, product.SourceSlot, "product", issues);
				}

				if (product.ProductType == "UnusedInput" && product.SourceInputIndex is null)
				{
					issues.Add(new FutureMudCraftValidationIssue(
						definition.SourceKey,
						"error",
						$"Fail product slot {product.SourceSlot} could not resolve its source input."));
				}

				if (product.ProductType == "Prog")
				{
					if (product.SourceInputIndex is null)
					{
						issues.Add(new FutureMudCraftValidationIssue(
							definition.SourceKey,
							"error",
							$"Prog product slot {product.SourceSlot} could not resolve its keyed input."));
					}

					foreach (var itemCase in product.ProgCases)
					{
						ValidateItemReference(baseline, definition.SourceKey, itemCase.InputSourceVnum, product.SourceSlot, "prog-input", issues);
						ValidateItemReference(baseline, definition.SourceKey, itemCase.OutputSourceVnum, product.SourceSlot, "prog-output", issues);
					}
				}
			}
		}

		return issues;
	}

	private static bool CanSatisfyClanRequirement(FutureMudCraftClanReference clan, string? rankName)
	{
		if (string.IsNullOrWhiteSpace(rankName))
		{
			return true;
		}

		if (clan.RankIds.ContainsKey(rankName))
		{
			return true;
		}

		return rankName.Equals("member", StringComparison.OrdinalIgnoreCase) ||
		       rankName.Equals("membership", StringComparison.OrdinalIgnoreCase);
	}

	private static void ValidateItemReference(
		FutureMudCraftBaselineCatalog baseline,
		string sourceKey,
		int? sourceVnum,
		int sourceSlot,
		string kind,
		ICollection<FutureMudCraftValidationIssue> issues)
	{
		if (sourceVnum is null)
		{
			return;
		}

		if (!baseline.ImportedItemsBySourceVnum.ContainsKey(sourceVnum.Value))
		{
			issues.Add(new FutureMudCraftValidationIssue(
				sourceKey,
				"error",
				$"Craft {kind} slot {sourceSlot} depends on RPI item vnum {sourceVnum.Value}, but no imported FutureMUD item prototype was found."));
		}
	}

	private static void ValidateTagReference(
		FutureMudCraftBaselineCatalog baseline,
		string sourceKey,
		string? tagName,
		bool usesGeneratedTag,
		int sourceSlot,
		string kind,
		ICollection<FutureMudCraftValidationIssue> issues)
	{
		if (string.IsNullOrWhiteSpace(tagName) || usesGeneratedTag)
		{
			return;
		}

		if (!baseline.TagIds.ContainsKey(tagName))
		{
			issues.Add(new FutureMudCraftValidationIssue(
				sourceKey,
				"error",
				$"Craft {kind} slot {sourceSlot} depends on tag '{tagName}', which does not exist in the baseline."));
		}
	}
}

public sealed class FutureMudCraftImporter
{
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudCraftBaselineCatalog _baseline;
	private readonly DateTime _now = DateTime.UtcNow;
	private long _nextCraftId;

	public FutureMudCraftImporter(FuturemudDatabaseContext context, FutureMudCraftBaselineCatalog baseline)
	{
		_context = context;
		_baseline = baseline;
		_nextCraftId = context.Crafts.Any() ? context.Crafts.Max(x => x.Id) + 1 : 1;
	}

	public IReadOnlyList<FutureMudCraftValidationIssue> Validate(IEnumerable<ConvertedCraftDefinition> definitions)
	{
		return FutureMudCraftValidation.Validate(_baseline, definitions);
	}

	public FutureMudCraftImportResult Apply(IEnumerable<ConvertedCraftDefinition> definitions, bool execute)
	{
		var ordered = definitions
			.OrderBy(x => x.CraftNumber)
			.ToList();
		var issues = Validate(ordered).ToList();
		var globalErrors = issues.Where(x =>
			x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase) &&
			x.SourceKey.Equals("baseline", StringComparison.OrdinalIgnoreCase)).ToList();
		if (globalErrors.Count > 0)
		{
			return new FutureMudCraftImportResult(
				0,
				0,
				0,
				0,
				0,
				0,
				issues,
				new CraftApplyAuditReport(DateTime.UtcNow, execute, [], [], []));
		}

		var issuesBySourceKey = issues
			.GroupBy(x => x.SourceKey, StringComparer.OrdinalIgnoreCase)
			.ToDictionary(x => x.Key, x => x.ToList(), StringComparer.OrdinalIgnoreCase);
		var existingMarkers = LoadExistingMarkers();
		var auditedTags = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		var auditedProgs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		List<CraftApplyAuditTagEntry> tagAudit = [];
		List<CraftApplyAuditProgEntry> progAudit = [];
		List<CraftApplyAuditCraftEntry> craftAudit = [];
		var insertedCount = 0;
		var skippedExistingCount = 0;
		var skippedDeferredCount = 0;
		var skippedInvalidCount = 0;
		var createdTagCount = 0;
		var createdProgCount = 0;

		foreach (var definition in ordered)
		{
			var marker = CreateMarker(definition);
			var generatedTagNames = definition.GeneratedTags
				.Select(x => x.TagName)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList();
			var futureProgNames = definition.FutureProgPlans
				.Select(x => x.FunctionName)
				.OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
				.ToList();

			if (existingMarkers.Contains(marker))
			{
				skippedExistingCount++;
				craftAudit.Add(new CraftApplyAuditCraftEntry(definition.SourceKey, definition.CraftNumber, "skipped-existing", null, generatedTagNames, futureProgNames));
				continue;
			}

			if (definition.Status == CraftConversionStatus.Deferred)
			{
				skippedDeferredCount++;
				craftAudit.Add(new CraftApplyAuditCraftEntry(definition.SourceKey, definition.CraftNumber, "skipped-deferred", null, generatedTagNames, futureProgNames));
				continue;
			}

			if (issuesBySourceKey.TryGetValue(definition.SourceKey, out var sourceIssues) &&
			    sourceIssues.Any(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)))
			{
				skippedInvalidCount++;
				craftAudit.Add(new CraftApplyAuditCraftEntry(definition.SourceKey, definition.CraftNumber, "skipped-invalid", null, generatedTagNames, futureProgNames));
				continue;
			}

			foreach (var tag in definition.GeneratedTags)
			{
				EnsureGeneratedTag(tag, execute, auditedTags, tagAudit, ref createdTagCount);
			}

			var progIdsByName = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);
			foreach (var plan in definition.FutureProgPlans)
			{
				var progId = EnsureFutureProg(definition, plan, execute, auditedProgs, progAudit, ref createdProgCount);
				if (progId > 0)
				{
					progIdsByName[plan.FunctionName] = progId;
				}
			}

			if (!execute)
			{
				craftAudit.Add(new CraftApplyAuditCraftEntry(definition.SourceKey, definition.CraftNumber, "would-create", null, generatedTagNames, futureProgNames));
				continue;
			}

			var craft = CreateCraftModel(definition, progIdsByName);
			_context.Crafts.Add(craft);
			_context.SaveChanges();

			foreach (var phase in definition.Phases.OrderBy(x => x.PhaseNumber))
			{
				craft.CraftPhases.Add(new MudSharp.Models.CraftPhase
				{
					Craft = craft,
					PhaseNumber = phase.PhaseNumber,
					PhaseLengthInSeconds = phase.Seconds,
					Echo = phase.Echo,
					FailEcho = phase.FailEcho,
					ExertionLevel = MapExertionLevel(phase.ExertionLevelName),
					StaminaUsage = phase.StaminaUsage,
				});
			}

			var inputsByIndex = new Dictionary<int, CraftInput>();
			foreach (var input in definition.Inputs.OrderBy(x => x.InputIndex))
			{
				var dbInput = new CraftInput
				{
					Craft = craft,
					InputType = input.InputType,
					InputQualityWeight = 1.0,
					Definition = BuildInputDefinition(input),
					OriginalAdditionTime = _now,
				};
				craft.CraftInputs.Add(dbInput);
				inputsByIndex[input.InputIndex] = dbInput;
			}

			foreach (var tool in definition.Tools.OrderBy(x => x.SourceSlot))
			{
				craft.CraftTools.Add(new CraftTool
				{
					Craft = craft,
					ToolType = tool.ToolType,
					ToolQualityWeight = 1.0,
					DesiredState = MapDesiredState(tool.DesiredStateName),
					Definition = BuildToolDefinition(tool),
					UseToolDuration = tool.UseToolDuration,
					OriginalAdditionTime = _now,
				});
			}

			_context.SaveChanges();

			foreach (var product in definition.Products)
			{
				var dbProduct = new CraftProduct
				{
					Craft = craft,
					ProductType = product.ProductType,
					Definition = BuildProductDefinition(product, progIdsByName, inputsByIndex),
					OriginalAdditionTime = _now,
					IsFailProduct = product.IsFailProduct,
					MaterialDefiningInputIndex = null,
				};
				craft.CraftProducts.Add(dbProduct);
			}

			_context.SaveChanges();
			existingMarkers.Add(marker);
			insertedCount++;
			craftAudit.Add(new CraftApplyAuditCraftEntry(definition.SourceKey, definition.CraftNumber, "created", craft.Id, generatedTagNames, futureProgNames));
		}

		return new FutureMudCraftImportResult(
			insertedCount,
			skippedExistingCount,
			skippedDeferredCount,
			skippedInvalidCount,
			createdTagCount,
			createdProgCount,
			issues,
			new CraftApplyAuditReport(DateTime.UtcNow, execute, tagAudit, progAudit, craftAudit));
	}

	private Craft CreateCraftModel(ConvertedCraftDefinition definition, IReadOnlyDictionary<string, long> progIdsByName)
	{
		var appearProgId = progIdsByName.TryGetValue($"RPICraft_{definition.CraftNumber:D4}_Appear", out var appearProg)
			? appearProg
			: (long?)null;
		var canUseProgId = progIdsByName.TryGetValue($"RPICraft_{definition.CraftNumber:D4}_CanUse", out var canUseProg)
			? canUseProg
			: (long?)null;
		var whyProgId = progIdsByName.TryGetValue($"RPICraft_{definition.CraftNumber:D4}_Why", out var whyProg)
			? whyProg
			: (long?)null;

		return new Craft
		{
			Id = _nextCraftId++,
			RevisionNumber = 0,
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _baseline.BuilderAccountId,
				BuilderDate = _now,
				BuilderComment = BuildBuilderComment(definition),
				ReviewerAccountId = _baseline.BuilderAccountId,
				ReviewerDate = _now,
				ReviewerComment = "Imported by the RPI Engine Worldfile Converter.",
			},
			Name = definition.Name,
			Category = definition.Category,
			Blurb = definition.Blurb,
			ActionDescription = definition.ActionDescription,
			ActiveCraftItemSdesc = definition.ActiveCraftItemSdesc,
			AppearInCraftsListProgId = appearProgId,
			CanUseProgId = canUseProgId,
			WhyCannotUseProgId = whyProgId,
			OnUseProgStartId = null,
			OnUseProgCompleteId = null,
			OnUseProgCancelId = null,
			CheckTraitId = definition.PrimaryCheck is not null ? _baseline.TraitIds[definition.PrimaryCheck.TraitName] : null,
			CheckDifficulty = ParseDifficulty(definition.CheckDifficultyName),
			FailThreshold = ParseOutcome(definition.FailThresholdName),
			FreeSkillChecks = definition.FreeSkillChecks,
			FailPhase = definition.FailPhase,
			Interruptable = true,
			QualityFormula = "5 + (outcome/3) + (variable/20)",
			CheckQualityWeighting = 1.0,
			InputQualityWeighting = 1.0,
			ToolQualityWeighting = 1.0,
			IsPracticalCheck = true,
		};
	}

	private void EnsureGeneratedTag(
		GeneratedCraftTagDefinition definition,
		bool execute,
		ISet<string> auditedTags,
		ICollection<CraftApplyAuditTagEntry> audit,
		ref int createdTagCount)
	{
		if (_baseline.TagIds.TryGetValue(definition.TagName, out var existingTagId))
		{
			if (auditedTags.Add(definition.TagName))
			{
				audit.Add(new CraftApplyAuditTagEntry(definition.TagName, "existing", existingTagId, definition.SourceVnums));
			}

			if (execute)
			{
				EnsureTagLinks(existingTagId, definition.SourceVnums);
			}

			return;
		}

		if (!execute)
		{
			if (auditedTags.Add(definition.TagName))
			{
				audit.Add(new CraftApplyAuditTagEntry(definition.TagName, "would-create", null, definition.SourceVnums));
			}

			return;
		}

		var tag = new Tag
		{
			Name = definition.TagName,
			ParentId = null,
			ShouldSeeProgId = null,
		};
		_context.Tags.Add(tag);
		_context.SaveChanges();

		_baseline.TagIds[definition.TagName] = tag.Id;
		EnsureTagLinks(tag.Id, definition.SourceVnums);
		createdTagCount++;

		if (auditedTags.Add(definition.TagName))
		{
			audit.Add(new CraftApplyAuditTagEntry(definition.TagName, "created", tag.Id, definition.SourceVnums));
		}
	}

	private void EnsureTagLinks(long tagId, IEnumerable<int> sourceVnums)
	{
		var changed = false;
		foreach (var sourceVnum in sourceVnums.Distinct())
		{
			if (!_baseline.ImportedItemsBySourceVnum.TryGetValue(sourceVnum, out var item))
			{
				continue;
			}

			if (item.TagIds.Contains(tagId))
			{
				continue;
			}

			_context.GameItemProtosTags.Add(new GameItemProtosTags
			{
				GameItemProtoId = item.ProtoId,
				GameItemProtoRevisionNumber = item.RevisionNumber,
				TagId = tagId,
			});
			item.TagIds.Add(tagId);
			changed = true;
		}

		if (changed)
		{
			_context.SaveChanges();
		}
	}

	private long EnsureFutureProg(
		ConvertedCraftDefinition craft,
		CraftFutureProgPlan plan,
		bool execute,
		ISet<string> auditedProgs,
		ICollection<CraftApplyAuditProgEntry> audit,
		ref int createdProgCount)
	{
		if (_baseline.FutureProgIds.TryGetValue(plan.FunctionName, out var existingId))
		{
			if (auditedProgs.Add(plan.FunctionName))
			{
				audit.Add(new CraftApplyAuditProgEntry(plan.FunctionName, plan.Kind, "existing", existingId));
			}

			return existingId;
		}

		if (!execute)
		{
			if (auditedProgs.Add(plan.FunctionName))
			{
				audit.Add(new CraftApplyAuditProgEntry(plan.FunctionName, plan.Kind, "would-create", null));
			}

			return 0;
		}

		var prog = new DbFutureProg
		{
			FunctionName = plan.FunctionName,
			Category = "Crafting",
			Subcategory = "RPI Import",
			ReturnType = GetReturnType(plan.Kind),
			FunctionComment = $"{plan.Description} Imported from {craft.SourceKey}.",
			FunctionText = BuildFutureProgText(craft, plan),
			StaticType = (int)FutureProgStaticType.NotStatic,
			AcceptsAnyParameters = false,
			Public = false,
		};

		foreach (var parameter in GetParameters(plan.Kind))
		{
			prog.FutureProgsParameters.Add(new FutureProgsParameter
			{
				FutureProg = prog,
				ParameterIndex = parameter.Index,
				ParameterType = parameter.Type,
				ParameterName = parameter.Name,
			});
		}

		_context.FutureProgs.Add(prog);
		_context.SaveChanges();

		_baseline.FutureProgIds[plan.FunctionName] = prog.Id;
		createdProgCount++;
		if (auditedProgs.Add(plan.FunctionName))
		{
			audit.Add(new CraftApplyAuditProgEntry(plan.FunctionName, plan.Kind, "created", prog.Id));
		}

		return prog.Id;
	}

	private static long GetReturnType(string kind)
	{
		return kind switch
		{
			"WhyCannotUseProg" => (long)ProgVariableTypes.Text,
			"ProgProduct" => (long)ProgVariableTypes.Item,
			_ => (long)ProgVariableTypes.Boolean,
		};
	}

	private static IReadOnlyList<(int Index, long Type, string Name)> GetParameters(string kind)
	{
		return kind switch
		{
			"ProgProduct" =>
			[
				(0, (long)(ProgVariableTypes.CollectionDictionary | ProgVariableTypes.Item), "items"),
				(1, (long)(ProgVariableTypes.CollectionDictionary | ProgVariableTypes.LiquidMixture), "liquids"),
			],
			_ =>
			[
				(0, (long)ProgVariableTypes.Character, "ch"),
			]
		};
	}

	private string BuildFutureProgText(ConvertedCraftDefinition craft, CraftFutureProgPlan plan)
	{
		return plan.Kind switch
		{
			"AppearInCraftsListProg" => BuildAppearProgText(craft),
			"CanUseProg" => BuildCanUseProgText(craft),
			"WhyCannotUseProg" => BuildWhyCannotUseProgText(craft),
			"ProgProduct" => BuildProductProgText(craft, plan.FunctionName),
			_ => "return true",
		};
	}

	private string BuildAppearProgText(ConvertedCraftDefinition craft)
	{
		if (craft.Constraints.HiddenFromCraftList)
		{
			return "return false";
		}

		return BuildBooleanConstraintProg(craft, forAppearList: true);
	}

	private string BuildCanUseProgText(ConvertedCraftDefinition craft)
	{
		return BuildBooleanConstraintProg(craft, forAppearList: false);
	}

	private string BuildBooleanConstraintProg(ConvertedCraftDefinition craft, bool forAppearList)
	{
		var lines = BuildConstraintCheckLines(craft, forWhyProg: false);
		if (lines.Count == 0)
		{
			return "return true";
		}

		lines.Add("return true");
		return string.Join('\n', lines);
	}

	private string BuildWhyCannotUseProgText(ConvertedCraftDefinition craft)
	{
		var lines = BuildConstraintCheckLines(craft, forWhyProg: true);
		if (lines.Count == 0)
		{
			return "return \"\"";
		}

		lines.Add("return \"\"");
		return string.Join('\n', lines);
	}

	private List<string> BuildConstraintCheckLines(ConvertedCraftDefinition craft, bool forWhyProg)
	{
		List<string> lines = [];
		var terrainIds = craft.Constraints.TerrainNames
			.Where(_baseline.TerrainIds.ContainsKey)
			.Select(x => _baseline.TerrainIds[x])
			.Distinct()
			.ToList();
		var raceIds = craft.Constraints.RaceNames
			.Where(_baseline.RaceIds.ContainsKey)
			.Select(x => _baseline.RaceIds[x])
			.Distinct()
			.ToList();
		var weatherIds = craft.Constraints.WeatherStates
			.Select(x => _baseline.TryGetWeatherId(x, out var id) ? (long?)id : null)
			.Where(x => x.HasValue)
			.Select(x => x!.Value)
			.Distinct()
			.ToList();
		var openingTraitIds = craft.Constraints.OpeningTraitNames
			.Where(_baseline.TraitIds.ContainsKey)
			.Select(x => _baseline.TraitIds[x])
			.Distinct()
			.ToList();
		var seasonAliases = FutureMudCraftTransformer.ResolveSeasonAliases(craft.Constraints.Seasons);
		var clanConditions = craft.Constraints.ClanRequirements
			.Where(x => _baseline.ClansByAlias.ContainsKey(x.CanonicalAlias))
			.Select(x =>
			{
				var clan = _baseline.ClansByAlias[x.CanonicalAlias];
				if (TryResolveClanRequirement(clan, x.RankName, out var rankId, out var fallbackToAnyMembership))
				{
					if (fallbackToAnyMembership)
					{
						return $"IsClanMember(@ch, ToClan({clan.ClanId.ToString(SystemCultureInfo.InvariantCulture)}))";
					}

					return $"IsClanMember(@ch, ToClan({clan.ClanId.ToString(SystemCultureInfo.InvariantCulture)}), ToRank({rankId.ToString(SystemCultureInfo.InvariantCulture)}))";
				}

				return "false";
			})
			.Distinct(StringComparer.OrdinalIgnoreCase)
			.ToList();

		if (openingTraitIds.Count > 0)
		{
			var denyExpression = string.Join(" and ", openingTraitIds.Select(x => $"not(@ch.Skills.Any(s, @s.Id == {x.ToString(SystemCultureInfo.InvariantCulture)}))"));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"You do not know any of the required opening skills: {string.Join(", ", craft.Constraints.OpeningSourceNames)}."
					: null));
		}

		if (terrainIds.Count > 0 || weatherIds.Count > 0)
		{
			lines.AddRange(BuildConditionalBlock(
				"isnull(@ch.Location)",
				forWhyProg ? "You are not in a valid location to begin that craft." : null));
		}

		if (terrainIds.Count > 0)
		{
			var denyExpression = string.Join(" and ", terrainIds.Select(x => $"@ch.Location.Terrain != ToTerrain({x.ToString(SystemCultureInfo.InvariantCulture)})"));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"You must be in one of the required terrains: {string.Join(", ", craft.Constraints.TerrainNames)}."
					: null));
		}

		if (seasonAliases.Count > 0)
		{
			lines.AddRange(BuildConditionalBlock(
				"isnull(@ch.Culture)",
				forWhyProg ? "You do not have a culture calendar available for this craft." : null));
			var denyExpression = string.Join(" and ", seasonAliases.Select(x => $"now(@ch.Culture.PrimaryCalendar).month != \"{EscapeProgString(x)}\""));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"This craft can only be attempted during {string.Join(", ", craft.Constraints.Seasons)}."
					: null));
		}

		if (weatherIds.Count > 0)
		{
			lines.AddRange(BuildConditionalBlock(
				"isnull(@ch.Location.Weather)",
				forWhyProg ? "The current weather cannot be determined for this craft." : null));
			var denyExpression = string.Join(" and ", weatherIds.Select(x => $"@ch.Location.Weather.Id != {x.ToString(SystemCultureInfo.InvariantCulture)}"));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"This craft requires one of the following weather states: {string.Join(", ", craft.Constraints.WeatherStates)}."
					: null));
		}

		if (raceIds.Count > 0)
		{
			var denyExpression = string.Join(" and ", raceIds.Select(x => $"@ch.Race != ToRace({x.ToString(SystemCultureInfo.InvariantCulture)})"));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"This craft is limited to: {string.Join(", ", craft.Constraints.RaceNames)}."
					: null));
		}

		if (clanConditions.Count > 0)
		{
			var denyExpression = string.Join(" and ", clanConditions.Select(x => $"not({x})"));
			lines.AddRange(BuildConditionalBlock(
				denyExpression,
				forWhyProg
					? $"This craft requires clan membership in one of: {string.Join(", ", craft.Constraints.ClanRequirements.Select(x => x.CanonicalAlias))}."
					: null));
		}

		if (craft.Constraints.FollowersRequired > 0)
		{
			var minimumPartySize = craft.Constraints.FollowersRequired + 1;
			lines.AddRange(BuildConditionalBlock(
				$"isnull(@ch.Party) or @ch.groupmembers.Count < {minimumPartySize.ToString(SystemCultureInfo.InvariantCulture)}",
				forWhyProg
					? $"This craft requires at least {craft.Constraints.FollowersRequired.ToString(SystemCultureInfo.InvariantCulture)} follower(s) assisting you."
					: null));
		}

		return lines;
	}

	private static bool TryResolveClanRequirement(
		FutureMudCraftClanReference clan,
		string? rankName,
		out long rankId,
		out bool fallbackToAnyMembership)
	{
		rankId = 0;
		fallbackToAnyMembership = false;

		if (string.IsNullOrWhiteSpace(rankName))
		{
			fallbackToAnyMembership = true;
			return true;
		}

		if (clan.RankIds.TryGetValue(rankName, out rankId))
		{
			return true;
		}

		if (rankName.Equals("member", StringComparison.OrdinalIgnoreCase) ||
		    rankName.Equals("membership", StringComparison.OrdinalIgnoreCase))
		{
			fallbackToAnyMembership = true;
			return true;
		}

		return false;
	}

	private static IEnumerable<string> BuildConditionalBlock(string condition, string? whyText)
	{
		if (string.IsNullOrWhiteSpace(condition))
		{
			return Array.Empty<string>();
		}

		if (whyText is null)
		{
			return
			[
				$"if ({condition})",
				"\treturn false",
				"end if"
			];
		}

		return
		[
			$"if ({condition})",
			$"\treturn \"{EscapeProgString(whyText)}\"",
			"end if"
		];
	}

	private string BuildProductProgText(ConvertedCraftDefinition craft, string functionName)
	{
		var product = craft.Products.First(x => string.Equals(x.GeneratedProgName, functionName, StringComparison.OrdinalIgnoreCase));
		List<string> lines = [];
		foreach (var itemCase in product.ProgCases)
		{
			var inputProto = _baseline.ImportedItemsBySourceVnum[itemCase.InputSourceVnum].ProtoId;
			var outputProto = _baseline.ImportedItemsBySourceVnum[itemCase.OutputSourceVnum].ProtoId;
			var quantity = Math.Max(product.Quantity, 1);
			lines.Add($"if (@items[\"{product.SourceInputIndex!.Value.ToString(SystemCultureInfo.InvariantCulture)}\"].Any(x, @x.Prototype.Id == {inputProto.ToString(SystemCultureInfo.InvariantCulture)}))");
			lines.Add(quantity > 1
				? $"\treturn loaditem({outputProto.ToString(SystemCultureInfo.InvariantCulture)}, {quantity.ToString(SystemCultureInfo.InvariantCulture)})"
				: $"\treturn loaditem({outputProto.ToString(SystemCultureInfo.InvariantCulture)})");
			lines.Add("end if");
		}

		lines.Add("return null");
		return string.Join('\n', lines);
	}

	private string BuildInputDefinition(ConvertedCraftInputDefinition input)
	{
		return input.InputType switch
		{
			"SimpleItem" => new XElement("Definition",
				new XElement("TargetItemId", _baseline.ImportedItemsBySourceVnum[input.SourceVnum!.Value].ProtoId),
				new XElement("Quantity", input.Quantity)).ToString(SaveOptions.DisableFormatting),
			"Tag" => new XElement("Definition",
				new XElement("TargetTagId", _baseline.TagIds[input.TagName!]),
				new XElement("Quantity", input.Quantity)).ToString(SaveOptions.DisableFormatting),
			_ => throw new InvalidOperationException($"Unsupported craft input type '{input.InputType}'."),
		};
	}

	private string BuildToolDefinition(ConvertedCraftToolDefinition tool)
	{
		return tool.ToolType switch
		{
			"SimpleTool" => new XElement("Definition",
				new XElement("TargetItemId", _baseline.ImportedItemsBySourceVnum[tool.SourceVnum!.Value].ProtoId))
				.ToString(SaveOptions.DisableFormatting),
			"TagTool" => new XElement("Definition",
				new XElement("TargetItemTag", _baseline.TagIds[tool.TagName!]))
				.ToString(SaveOptions.DisableFormatting),
			_ => throw new InvalidOperationException($"Unsupported craft tool type '{tool.ToolType}'."),
		};
	}

	private string BuildProductDefinition(
		ConvertedCraftProductDefinition product,
		IReadOnlyDictionary<string, long> progIdsByName,
		IReadOnlyDictionary<int, CraftInput> inputsByIndex)
	{
		return product.ProductType switch
		{
			"SimpleProduct" => new XElement("Definition",
				new XElement("ProductProducedId", _baseline.ImportedItemsBySourceVnum[product.OutputVnum!.Value].ProtoId),
				new XElement("Quantity", product.Quantity),
				new XElement("Skin", 0))
				.ToString(SaveOptions.DisableFormatting),
			"UnusedInput" => new XElement("Definition",
				new XElement("WhichInputId", inputsByIndex[product.SourceInputIndex!.Value].Id),
				new XElement("PercentageRecovered", (product.PercentageRecovered ?? 1.0).ToString(SystemCultureInfo.InvariantCulture)))
				.ToString(SaveOptions.DisableFormatting),
			"Prog" => new XElement("Definition",
				new XElement("ItemProg", progIdsByName[product.GeneratedProgName!]))
				.ToString(SaveOptions.DisableFormatting),
			_ => throw new InvalidOperationException($"Unsupported craft product type '{product.ProductType}'."),
		};
	}

	private HashSet<string> LoadExistingMarkers()
	{
		return _context.Crafts
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem != null &&
			            x.EditableItem.BuilderComment != null &&
			            x.EditableItem.BuilderComment.StartsWith("RPICRAFTIMPORT|"))
			.AsEnumerable()
			.Select(x => x.EditableItem!.BuilderComment.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty)
			.Where(x => x.StartsWith("RPICRAFTIMPORT|", StringComparison.Ordinal))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string CreateMarker(ConvertedCraftDefinition definition)
	{
		return $"RPICRAFTIMPORT|{Path.GetFileName(definition.SourceFile)}|{definition.CraftNumber.ToString(SystemCultureInfo.InvariantCulture)}|{definition.CraftName}|{definition.SubcraftName}|{definition.Command}";
	}

	private static string BuildBuilderComment(ConvertedCraftDefinition definition)
	{
		return string.Join('\n',
		[
			CreateMarker(definition),
			$"Status={definition.Status}",
			$"Check={(definition.PrimaryCheck?.TraitName ?? "none")}|{definition.CheckDifficultyName}",
			$"GeneratedTags={string.Join(",", definition.GeneratedTags.Select(x => x.TagName))}",
			$"FutureProgs={string.Join(",", definition.FutureProgPlans.Select(x => x.FunctionName))}",
			$"Warnings={string.Join(" | ", definition.Warnings.Select(x => x.Message))}"
		]);
	}

	private static int ParseDifficulty(string value)
	{
		return Enum.TryParse<Difficulty>(value, ignoreCase: true, out var difficulty)
			? (int)difficulty
			: (int)Difficulty.Automatic;
	}

	private static int ParseOutcome(string value)
	{
		return Enum.TryParse<Outcome>(value, ignoreCase: true, out var outcome)
			? (int)outcome
			: (int)Outcome.MinorFail;
	}

	private static int MapExertionLevel(string name)
	{
		return Enum.TryParse<ExertionLevel>(name, ignoreCase: true, out var exertion)
			? (int)exertion
			: (int)ExertionLevel.Rest;
	}

	private static int MapDesiredState(string name)
	{
		return Enum.TryParse<DesiredItemState>(name, ignoreCase: true, out var state)
			? (int)state
			: (int)DesiredItemState.Held;
	}

	private static string EscapeProgString(string value)
	{
		return value.Replace("\\", "\\\\").Replace("\"", "\"\"");
	}
}
