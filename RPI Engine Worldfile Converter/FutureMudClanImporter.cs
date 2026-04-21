#nullable enable

using Microsoft.EntityFrameworkCore;
using MudSharp.Community;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.TimeAndDate.Intervals;

using ClanModel = MudSharp.Models.Clan;
using RankModel = MudSharp.Models.Rank;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudClanValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudClanImportResult(
	int InsertedCount,
	int SkippedExistingCount,
	IReadOnlyList<FutureMudClanValidationIssue> Issues);

public static class FutureMudClanValidation
{
	public static IReadOnlyList<FutureMudClanValidationIssue> Validate(
		FutureMudClanBaselineCatalog baseline,
		IEnumerable<ConvertedClanDefinition> definitions)
	{
		List<FutureMudClanValidationIssue> issues = [];
		HashSet<string> canonicalAliases = [];
		HashSet<string> allAliases = [];

		foreach (var definition in definitions)
		{
			if (!canonicalAliases.Add(definition.CanonicalAlias))
			{
				issues.Add(new FutureMudClanValidationIssue(
					definition.SourceKey,
					"error",
					$"Duplicate canonical alias '{definition.CanonicalAlias}'."));
			}

			if (string.IsNullOrWhiteSpace(definition.FullName))
			{
				issues.Add(new FutureMudClanValidationIssue(definition.SourceKey, "error", "Clan full name is required."));
			}

			if (definition.Ranks.Count == 0)
			{
				issues.Add(new FutureMudClanValidationIssue(
					definition.SourceKey,
					"error",
					"No importable ranks were produced for this clan."));
			}

			foreach (var alias in definition.LegacyAliases.Append(definition.CanonicalAlias))
			{
				if (!allAliases.Add(alias))
				{
					issues.Add(new FutureMudClanValidationIssue(
						definition.SourceKey,
						"warning",
						$"Alias '{alias}' appears in more than one converted clan alias set."));
				}
			}
		}

		if (baseline.CalendarId <= 0)
		{
			issues.Add(new FutureMudClanValidationIssue("baseline", "error", "A seeded calendar is required."));
		}

		if (string.IsNullOrWhiteSpace(baseline.PayIntervalReferenceDate))
		{
			issues.Add(new FutureMudClanValidationIssue("baseline", "error", "A seeded calendar reference date is required."));
		}

		if (string.IsNullOrWhiteSpace(baseline.PayIntervalReferenceTime))
		{
			issues.Add(new FutureMudClanValidationIssue("baseline", "error", "A primary timezone is required."));
		}

		return issues;
	}
}

public sealed class FutureMudClanBaselineCatalog
{
	public required long CalendarId { get; init; }
	public required string PayIntervalReferenceDate { get; init; }
	public required string PayIntervalReferenceTime { get; init; }

	public static FutureMudClanBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var calendar = context.Calendars.FirstOrDefault()
		               ?? throw new InvalidOperationException("The target database does not contain any calendars.");
		var timezone = context.Timezones
			.Include(x => x.Clock)
			.FirstOrDefault(x => x.Clock.PrimaryTimezoneId == x.Id) ??
		               throw new InvalidOperationException("The target database does not contain a primary timezone.");

		return new FutureMudClanBaselineCatalog
		{
			CalendarId = calendar.Id,
			PayIntervalReferenceDate = calendar.Date,
			PayIntervalReferenceTime = $"{timezone.Name} 0:0:0",
		};
	}
}

public sealed class FutureMudClanImporter
{
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudClanBaselineCatalog _baseline;

	public FutureMudClanImporter(FuturemudDatabaseContext context, FutureMudClanBaselineCatalog baseline)
	{
		_context = context;
		_baseline = baseline;
	}

	public IReadOnlyList<FutureMudClanValidationIssue> Validate(IEnumerable<ConvertedClanDefinition> definitions)
	{
		return FutureMudClanValidation.Validate(_baseline, definitions);
	}

	public FutureMudClanImportResult Apply(IEnumerable<ConvertedClanDefinition> definitions, bool execute)
	{
		var ordered = definitions
			.OrderBy(x => x.FullName, StringComparer.OrdinalIgnoreCase)
			.ThenBy(x => x.CanonicalAlias, StringComparer.OrdinalIgnoreCase)
			.ToList();

		var issues = Validate(ordered).ToList();
		var fatalIssues = issues.Where(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase)).ToList();
		if (fatalIssues.Count > 0)
		{
			return new FutureMudClanImportResult(0, 0, issues);
		}

		var existingAliases = _context.Clans
			.Select(x => x.Alias)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		var skippedExistingCount = 0;
		var insertedCount = 0;

		foreach (var definition in ordered)
		{
			if (existingAliases.Contains(definition.CanonicalAlias))
			{
				skippedExistingCount++;
				continue;
			}

			var conflictingLegacyAlias = definition.LegacyAliases.FirstOrDefault(existingAliases.Contains);
			if (!string.IsNullOrWhiteSpace(conflictingLegacyAlias))
			{
				issues.Add(new FutureMudClanValidationIssue(
					definition.SourceKey,
					"warning",
					$"Skipped import because legacy alias '{conflictingLegacyAlias}' already exists in the target database."));
				skippedExistingCount++;
				continue;
			}

			if (!execute)
			{
				continue;
			}

			var clan = new ClanModel
			{
				Name = definition.FullName,
				Alias = definition.CanonicalAlias,
				FullName = definition.FullName,
				Description = BuildDescription(definition),
				PayIntervalType = (int)IntervalType.Monthly,
				PayIntervalModifier = 1,
				PayIntervalOther = 0,
				CalendarId = _baseline.CalendarId,
				PayIntervalReferenceDate = _baseline.PayIntervalReferenceDate,
				PayIntervalReferenceTime = _baseline.PayIntervalReferenceTime,
				IsTemplate = false,
				ShowClanMembersInWho = false,
				ShowFamousMembersInNotables = false,
				Sphere = null,
			};

			foreach (var rank in definition.Ranks.OrderBy(x => x.Order))
			{
				var rankModel = new RankModel
				{
					Clan = clan,
					Name = rank.Name,
					Privileges = rank.Privileges,
					RankPath = rank.RankPath,
					RankNumber = rank.Order,
					FameType = 0,
				};

				rankModel.RanksAbbreviations.Add(new RanksAbbreviations
				{
					Rank = rankModel,
					Order = 0,
					Abbreviation = rank.Name,
				});
				rankModel.RanksTitles.Add(new RanksTitle
				{
					Rank = rankModel,
					Order = 0,
					Title = rank.Name,
				});
				clan.Ranks.Add(rankModel);
			}

			_context.Clans.Add(clan);
			_context.SaveChanges();
			existingAliases.Add(definition.CanonicalAlias);
			insertedCount++;
		}

		return new FutureMudClanImportResult(insertedCount, skippedExistingCount, issues);
	}

	private static string BuildDescription(ConvertedClanDefinition definition)
	{
		var legacyAliases = definition.LegacyAliases.Count > 0
			? $" Legacy aliases: {string.Join(", ", definition.LegacyAliases)}."
			: string.Empty;
		var groupId = definition.GroupId is not null
			? $" Source group id: {definition.GroupId.Value}."
			: string.Empty;

		return $"Imported by the RPI Engine Worldfile Converter from clan.cpp and region references. Canonical alias: {definition.CanonicalAlias}.{legacyAliases}{groupId}";
	}
}
