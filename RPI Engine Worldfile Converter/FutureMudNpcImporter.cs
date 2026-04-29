#nullable enable

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Form.Shape;
using MudSharp.Models;
using SystemCultureInfo = System.Globalization.CultureInfo;

namespace RPI_Engine_Worldfile_Converter;

public sealed record FutureMudNpcValidationIssue(string SourceKey, string Severity, string Message);

public sealed record FutureMudNpcRaceReference(
	long Id,
	string Name,
	long? ParentRaceId,
	long NaturalArmourQuality,
	long DefaultHealthStrategyId,
	long? DefaultCombatSettingId,
	int AdultAge,
	int ElderAge,
	int DefaultHandedness,
	IReadOnlyDictionary<Gender, long> HeightWeightModelIds,
	IReadOnlyList<long> EthnicityIds);

public sealed record FutureMudNpcEthnicityReference(
	long Id,
	string Name,
	long ParentRaceId,
	IReadOnlyDictionary<Gender, long> NameCultureIds);

public sealed record FutureMudNpcCultureReference(
	long Id,
	string Name,
	long PrimaryCalendarId,
	IReadOnlyDictionary<Gender, long> NameCultureIds);

public sealed record FutureMudNpcLanguageReference(
	long Id,
	string Name,
	long LinkedTraitId,
	long? DefaultAccentId,
	long? FallbackAccentId);

public sealed record NpcApplyAuditEntry(
	string SourceKey,
	int Vnum,
	string Action,
	long? TemplateId,
	NpcTemplateKind TemplateKind,
	NpcConversionStatus Status,
	string? RaceName,
	string? EthnicityName,
	string? CultureName);

public sealed record NpcApplyAuditReport(
	DateTime GeneratedUtc,
	bool Execute,
	IReadOnlyList<NpcApplyAuditEntry> Npcs);

public sealed record FutureMudNpcImportResult(
	int InsertedCount,
	int SkippedExistingCount,
	int SkippedDeferredCount,
	int SkippedInvalidCount,
	IReadOnlyList<FutureMudNpcValidationIssue> Issues,
	NpcApplyAuditReport Audit);

public sealed class FutureMudNpcBaselineCatalog
{
	private static readonly Regex NonAlphaNumericRegex = new("[^A-Za-z0-9]+", RegexOptions.Compiled);

	public required long BuilderAccountId { get; init; }
	public required Dictionary<string, FutureMudNpcRaceReference> Races { get; init; }
	public required Dictionary<string, FutureMudNpcEthnicityReference> Ethnicities { get; init; }
	public required Dictionary<string, FutureMudNpcCultureReference> Cultures { get; init; }
	public required Dictionary<string, long> TraitIds { get; init; }
	public required Dictionary<long, FutureMudNpcLanguageReference> LanguagesByTraitId { get; init; }
	public required Dictionary<string, long> ArtificialIntelligenceIds { get; init; }
	public required Dictionary<long, Dictionary<Gender, long>> NameProfilesByCultureId { get; init; }
	public required Dictionary<long, string> CalendarDates { get; init; }

	public static FutureMudNpcBaselineCatalog Load(FuturemudDatabaseContext context)
	{
		var builderAccountId = context.Accounts
			.Select(x => x.Id)
			.OrderBy(x => x)
			.FirstOrDefault();
		if (builderAccountId == 0)
		{
			throw new InvalidOperationException("The target database does not contain any accounts to attribute imported NPC templates to.");
		}

		var races = context.Races
			.Select(x => new
			{
				x.Id,
				x.Name,
				x.ParentRaceId,
				x.NaturalArmourQuality,
				x.DefaultHealthStrategyId,
				x.DefaultCombatSettingId,
				x.AdultAge,
				x.ElderAge,
				x.DefaultHandedness,
				x.DefaultHeightWeightModelMaleId,
				x.DefaultHeightWeightModelFemaleId,
				x.DefaultHeightWeightModelNeuterId,
				x.DefaultHeightWeightModelNonBinaryId
			})
			.ToList();
		var ethnicities = context.Ethnicities
			.Select(x => new
			{
				x.Id,
				x.Name,
				x.ParentRaceId
			})
			.ToList();
		var ethnicityIdsByRaceId = ethnicities
			.Where(x => x.ParentRaceId.HasValue)
			.GroupBy(x => x.ParentRaceId!.Value)
			.ToDictionary(
				x => x.Key,
				x => x.Select(y => y.Id).ToList());
		var ethnicityNameCultures = context.EthnicitiesNameCultures
			.Select(x => new
			{
				x.EthnicityId,
				x.NameCultureId,
				x.Gender
			})
			.ToList();
		var cultures = context.Cultures
			.Select(x => new
			{
				x.Id,
				x.Name,
				x.PrimaryCalendarId
			})
			.ToList();
		var cultureNameCultures = context.CulturesNameCultures
			.Select(x => new
			{
				x.CultureId,
				x.NameCultureId,
				x.Gender
			})
			.ToList();
		var nameProfiles = context.RandomNameProfiles
			.Select(x => new
			{
				x.Id,
				x.Gender,
				x.NameCultureId
			})
			.ToList();
		var languages = context.Languages
			.Select(x => new
			{
				x.Id,
				x.Name,
				x.LinkedTraitId,
				x.DefaultLearnerAccentId
			})
			.ToList();
		var fallbackAccentsByLanguageId = context.Accents
			.Select(x => new
			{
				x.Id,
				x.LanguageId
			})
			.ToList()
			.GroupBy(x => x.LanguageId)
			.ToDictionary(
				x => x.Key,
				x => x.OrderBy(y => y.Id).Select(y => (long?)y.Id).FirstOrDefault());

		return new FutureMudNpcBaselineCatalog
		{
			BuilderAccountId = builderAccountId,
			Races = races.ToDictionary(
				x => x.Name,
				x => new FutureMudNpcRaceReference(
					x.Id,
					x.Name,
					x.ParentRaceId,
					x.NaturalArmourQuality,
					x.DefaultHealthStrategyId,
					x.DefaultCombatSettingId,
					x.AdultAge,
					x.ElderAge,
					x.DefaultHandedness,
					new Dictionary<Gender, long>
					{
						[Gender.Male] = x.DefaultHeightWeightModelMaleId ?? 0,
						[Gender.Female] = x.DefaultHeightWeightModelFemaleId ?? 0,
						[Gender.Neuter] = x.DefaultHeightWeightModelNeuterId ?? 0,
						[Gender.NonBinary] = x.DefaultHeightWeightModelNonBinaryId ?? 0,
					}
					.Where(y => y.Value > 0)
					.ToDictionary(y => y.Key, y => y.Value),
					ethnicityIdsByRaceId.GetValueOrDefault(x.Id) ?? []),
				StringComparer.OrdinalIgnoreCase),
			Ethnicities = ethnicities.ToDictionary(
				x => x.Name,
				x => new FutureMudNpcEthnicityReference(
					x.Id,
					x.Name,
					x.ParentRaceId ?? 0L,
					ethnicityNameCultures
						.Where(y => y.EthnicityId == x.Id)
						.GroupBy(y => (Gender)y.Gender)
						.ToDictionary(y => y.Key, y => y.First().NameCultureId)),
				StringComparer.OrdinalIgnoreCase),
			Cultures = cultures.ToDictionary(
				x => x.Name,
				x => new FutureMudNpcCultureReference(
					x.Id,
					x.Name,
					x.PrimaryCalendarId,
					cultureNameCultures
						.Where(y => y.CultureId == x.Id)
						.GroupBy(y => (Gender)y.Gender)
						.ToDictionary(y => y.Key, y => y.First().NameCultureId)),
				StringComparer.OrdinalIgnoreCase),
			TraitIds = context.TraitDefinitions
				.Select(x => new
				{
					x.Id,
					x.Name
				})
				.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			LanguagesByTraitId = languages
				.Where(x => x.LinkedTraitId > 0)
				.ToDictionary(
					x => x.LinkedTraitId,
					x => new FutureMudNpcLanguageReference(
						x.Id,
						x.Name,
						x.LinkedTraitId,
						x.DefaultLearnerAccentId,
						fallbackAccentsByLanguageId.GetValueOrDefault(x.Id)),
					EqualityComparer<long>.Default),
			ArtificialIntelligenceIds = context.ArtificialIntelligences
				.Select(x => new
				{
					x.Id,
					x.Name
				})
				.ToDictionary(x => x.Name, x => x.Id, StringComparer.OrdinalIgnoreCase),
			NameProfilesByCultureId = nameProfiles
				.GroupBy(x => x.NameCultureId)
				.ToDictionary(
					x => x.Key,
					x => x
						.GroupBy(y => (Gender)y.Gender)
						.ToDictionary(y => y.Key, y => y.OrderBy(z => z.Id).First().Id)),
			CalendarDates = context.Calendars
				.Select(x => new
				{
					x.Id,
					x.Date
				})
				.ToDictionary(x => x.Id, x => x.Date, EqualityComparer<long>.Default),
		};
	}

	public bool TryResolveRace(string? name, out FutureMudNpcRaceReference race)
	{
		return TryResolveByName(Races, name, out race);
	}

	public bool TryResolveEthnicity(string? name, out FutureMudNpcEthnicityReference ethnicity)
	{
		return TryResolveByName(Ethnicities, name, out ethnicity);
	}

	public bool TryResolveCulture(string? name, out FutureMudNpcCultureReference culture)
	{
		return TryResolveByName(Cultures, name, out culture);
	}

	public bool TryResolveTrait(string? name, out long traitId)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			traitId = 0;
			return false;
		}

		if (TraitIds.TryGetValue(name, out traitId))
		{
			return true;
		}

		var normalised = NormaliseLookupKey(name);
		var match = TraitIds.FirstOrDefault(x => NormaliseLookupKey(x.Key) == normalised);
		if (!string.IsNullOrWhiteSpace(match.Key))
		{
			traitId = match.Value;
			return true;
		}

		traitId = 0;
		return false;
	}

	public bool TryResolveArtificialIntelligence(string name, out long aiId)
	{
		if (ArtificialIntelligenceIds.TryGetValue(name, out aiId))
		{
			return true;
		}

		var normalised = NormaliseLookupKey(name);
		var match = ArtificialIntelligenceIds.FirstOrDefault(x => NormaliseLookupKey(x.Key) == normalised);
		if (!string.IsNullOrWhiteSpace(match.Key))
		{
			aiId = match.Value;
			return true;
		}

		aiId = 0;
		return false;
	}

	public bool TryResolveEthnicityForDefinition(
		ConvertedNpcDefinition definition,
		FutureMudNpcRaceReference race,
		out FutureMudNpcEthnicityReference ethnicity)
	{
		if (TryResolveEthnicity(definition.EthnicityName, out ethnicity) && ethnicity.ParentRaceId == race.Id)
		{
			return true;
		}

		if (TryResolveEthnicity(definition.CultureName, out ethnicity) && ethnicity.ParentRaceId == race.Id)
		{
			return true;
		}

		if (TryResolveEthnicity(definition.RaceName, out ethnicity) && ethnicity.ParentRaceId == race.Id)
		{
			return true;
		}

		if (race.EthnicityIds.Count == 1)
		{
			var targetId = race.EthnicityIds[0];
			var match = Ethnicities.Values.FirstOrDefault(x => x.Id == targetId);
			if (match is not null)
			{
				ethnicity = match;
				return true;
			}
		}

		ethnicity = default!;
		return false;
	}

	public bool TryResolveNameCultureId(
		FutureMudNpcCultureReference culture,
		FutureMudNpcEthnicityReference? ethnicity,
		Gender gender,
		out long nameCultureId)
	{
		if (TryResolveNameCultureId(ethnicity?.NameCultureIds, gender, out nameCultureId))
		{
			return true;
		}

		if (TryResolveNameCultureId(culture.NameCultureIds, gender, out nameCultureId))
		{
			return true;
		}

		nameCultureId = 0;
		return false;
	}

	public bool TryResolveNameProfileId(long nameCultureId, Gender gender, out long profileId)
	{
		if (NameProfilesByCultureId.TryGetValue(nameCultureId, out var profiles))
		{
			if (profiles.TryGetValue(gender, out profileId))
			{
				return true;
			}

			if (profiles.TryGetValue(Gender.Neuter, out profileId))
			{
				return true;
			}

			if (profiles.TryGetValue(Gender.NonBinary, out profileId))
			{
				return true;
			}

			if (profiles.TryGetValue(Gender.Male, out profileId))
			{
				return true;
			}

			if (profiles.TryGetValue(Gender.Female, out profileId))
			{
				return true;
			}
		}

		profileId = 0;
		return false;
	}

	public bool TryResolveHeightWeightModelId(FutureMudNpcRaceReference race, Gender gender, out long modelId)
	{
		if (race.HeightWeightModelIds.TryGetValue(gender, out modelId))
		{
			return true;
		}

		if (gender is Gender.NonBinary && race.HeightWeightModelIds.TryGetValue(Gender.Neuter, out modelId))
		{
			return true;
		}

		if (gender is Gender.Neuter && race.HeightWeightModelIds.TryGetValue(Gender.NonBinary, out modelId))
		{
			return true;
		}

		if (gender != Gender.Male && race.HeightWeightModelIds.TryGetValue(Gender.Male, out modelId))
		{
			return true;
		}

		if (gender != Gender.Female && race.HeightWeightModelIds.TryGetValue(Gender.Female, out modelId))
		{
			return true;
		}

		modelId = 0;
		return false;
	}

	public bool TryResolveAccentId(long traitId, out long accentId)
	{
		if (!LanguagesByTraitId.TryGetValue(traitId, out var language))
		{
			accentId = 0;
			return false;
		}

		accentId = language.DefaultAccentId ?? language.FallbackAccentId ?? 0;
		return accentId > 0;
	}

	public bool TryGetCurrentDate(long calendarId, out string date)
	{
		return CalendarDates.TryGetValue(calendarId, out date!);
	}

	private static bool TryResolveByName<T>(
		IReadOnlyDictionary<string, T> source,
		string? name,
		out T value)
		where T : class
	{
		if (!string.IsNullOrWhiteSpace(name) && source.TryGetValue(name, out value!))
		{
			return true;
		}

		if (!string.IsNullOrWhiteSpace(name))
		{
			var normalised = NormaliseLookupKey(name);
			var match = source.FirstOrDefault(x => NormaliseLookupKey(x.Key) == normalised);
			if (!string.IsNullOrWhiteSpace(match.Key))
			{
				value = match.Value;
				return true;
			}
		}

		value = default!;
		return false;
	}

	private static bool TryResolveNameCultureId(
		IReadOnlyDictionary<Gender, long>? values,
		Gender gender,
		out long nameCultureId)
	{
		nameCultureId = 0;
		if (values is null || values.Count == 0)
		{
			return false;
		}

		if (values.TryGetValue(gender, out nameCultureId))
		{
			return true;
		}

		if (gender != Gender.Male && values.TryGetValue(Gender.Male, out nameCultureId))
		{
			return true;
		}

		if (gender != Gender.Female && values.TryGetValue(Gender.Female, out nameCultureId))
		{
			return true;
		}

		if (values.TryGetValue(Gender.Neuter, out nameCultureId))
		{
			return true;
		}

		if (values.TryGetValue(Gender.NonBinary, out nameCultureId))
		{
			return true;
		}

		nameCultureId = values.Values.FirstOrDefault();
		return nameCultureId > 0;
	}

	public static string NormaliseLookupKey(string value)
	{
		var decomposed = value.Normalize(NormalizationForm.FormD);
		var builder = new StringBuilder(decomposed.Length);
		foreach (var character in decomposed)
		{
			var category = CharUnicodeInfo.GetUnicodeCategory(character);
			if (category != UnicodeCategory.NonSpacingMark)
			{
				builder.Append(character);
			}
		}

		return NonAlphaNumericRegex.Replace(builder.ToString().ToLowerInvariant(), "");
	}
}

public static class FutureMudNpcValidation
{
	public static IReadOnlyList<FutureMudNpcValidationIssue> Validate(
		FutureMudNpcBaselineCatalog catalog,
		IEnumerable<ConvertedNpcDefinition> definitions)
	{
		List<FutureMudNpcValidationIssue> issues = [];
		HashSet<string> templateNames = [];

		foreach (var definition in definitions)
		{
			if (!templateNames.Add(definition.TemplateName))
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					$"Duplicate template name '{definition.TemplateName}' was produced."));
			}

			if (definition.Status == NpcConversionStatus.Deferred)
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"warning",
					"NPC is deferred in this pass and will be skipped by apply-npcs."));
				continue;
			}

			if (definition.Status == NpcConversionStatus.Unresolved)
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					"NPC does not have a high-confidence race, ethnicity, and culture mapping."));
				continue;
			}

			if (!catalog.TryResolveRace(definition.RaceName, out var race))
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					$"Missing race '{definition.RaceName ?? "(null)"}' in the FutureMUD baseline."));
				continue;
			}

			if (!catalog.TryResolveCulture(definition.CultureName, out var culture))
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					$"Missing culture '{definition.CultureName ?? "(null)"}' in the FutureMUD baseline."));
				continue;
			}

			if (!catalog.TryResolveEthnicityForDefinition(definition, race, out var ethnicity))
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					$"Could not resolve an ethnicity for race '{race.Name}' from '{definition.EthnicityName ?? "(null)"}'."));
			}

			foreach (var attribute in definition.Attributes)
			{
				if (!catalog.TryResolveTrait(attribute.TraitName, out _))
				{
					issues.Add(new FutureMudNpcValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing attribute trait '{attribute.TraitName}'."));
				}
			}

			foreach (var trait in definition.Traits)
			{
				if (!catalog.TryResolveTrait(trait.TraitName, out var traitId))
				{
					issues.Add(new FutureMudNpcValidationIssue(
						definition.SourceKey,
						"warning",
						$"Missing optional skill or language trait '{trait.TraitName}'. It will be preserved as provenance but not imported as a live trait."));
					continue;
				}

				if (trait.IsLanguage && !catalog.TryResolveAccentId(traitId, out _))
				{
					issues.Add(new FutureMudNpcValidationIssue(
						definition.SourceKey,
						"warning",
						$"No accent is available for language trait '{trait.TraitName}'. The language trait will import without an accent."));
				}
			}

			foreach (var aiName in definition.ArtificialIntelligenceNames)
			{
				if (!catalog.TryResolveArtificialIntelligence(aiName, out _))
				{
					issues.Add(new FutureMudNpcValidationIssue(
						definition.SourceKey,
						"error",
						$"Missing Artificial Intelligence '{aiName}'."));
				}
			}

			if (definition.TemplateKind == NpcTemplateKind.Variable)
			{
				foreach (var chance in definition.GenderChances)
				{
					if (!catalog.TryResolveNameCultureId(culture, ethnicity, chance.Gender, out var nameCultureId))
					{
						issues.Add(new FutureMudNpcValidationIssue(
							definition.SourceKey,
							"error",
							$"No name culture mapping exists for gender '{chance.Gender}' on ethnicity '{ethnicity.Name}' / culture '{culture.Name}'."));
						continue;
					}

					if (!catalog.TryResolveNameProfileId(nameCultureId, chance.Gender, out _))
					{
						issues.Add(new FutureMudNpcValidationIssue(
							definition.SourceKey,
							"error",
							$"No random name profile exists for gender '{chance.Gender}' and name culture id {nameCultureId}."));
					}

					if (!catalog.TryResolveHeightWeightModelId(race, chance.Gender, out _))
					{
						issues.Add(new FutureMudNpcValidationIssue(
							definition.SourceKey,
							"error",
							$"No height/weight model exists for race '{race.Name}' and gender '{chance.Gender}'."));
					}
				}
			}
			else if (!catalog.TryResolveNameCultureId(culture, ethnicity, definition.SimpleGender, out _))
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"error",
					$"No name culture mapping exists for simple NPC gender '{definition.SimpleGender}'."));
			}

			if (definition.LegacyArmour > 0 && definition.LegacyArmour - race.NaturalArmourQuality >= 3)
			{
				issues.Add(new FutureMudNpcValidationIssue(
					definition.SourceKey,
					"warning",
					$"Natural armour gap: legacy armour {definition.LegacyArmour} is materially higher than race '{race.Name}' quality {race.NaturalArmourQuality}."));
			}
		}

		return issues;
	}
}

public sealed class FutureMudNpcImporter
{
	private readonly FuturemudDatabaseContext _context;
	private readonly FutureMudNpcBaselineCatalog _catalog;
	private readonly DateTime _now = DateTime.UtcNow;

	public FutureMudNpcImporter(FuturemudDatabaseContext context, FutureMudNpcBaselineCatalog catalog)
	{
		_context = context;
		_catalog = catalog;
	}

	public IReadOnlyList<FutureMudNpcValidationIssue> Validate(IEnumerable<ConvertedNpcDefinition> definitions)
	{
		return FutureMudNpcValidation.Validate(_catalog, definitions);
	}

	public FutureMudNpcImportResult Apply(IEnumerable<ConvertedNpcDefinition> definitions, bool execute)
	{
		var ordered = definitions
			.OrderBy(x => x.Zone)
			.ThenBy(x => x.Vnum)
			.ToList();

		var issues = Validate(ordered).ToList();
		var invalidSourceKeys = issues
			.Where(x => x.Severity.Equals("error", StringComparison.OrdinalIgnoreCase))
			.Select(x => x.SourceKey)
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
		List<NpcApplyAuditEntry> auditEntries = [];

		var existingMarkers = LoadExistingMarkers();
		var nextTemplateId = _context.NpcTemplates.Select(x => (long?)x.Id).Max() ?? 0L;
		var skippedExistingCount = 0;
		var skippedDeferredCount = 0;
		var skippedInvalidCount = 0;
		var insertedCount = 0;

		foreach (var definition in ordered)
		{
			var marker = CreateProvenanceMarker(definition);
			if (existingMarkers.Contains(marker))
			{
				skippedExistingCount++;
				auditEntries.Add(new NpcApplyAuditEntry(
					definition.SourceKey,
					definition.Vnum,
					"skipped-existing",
					null,
					definition.TemplateKind,
					definition.Status,
					definition.RaceName,
					definition.EthnicityName,
					definition.CultureName));
				continue;
			}

			if (definition.Status == NpcConversionStatus.Deferred)
			{
				skippedDeferredCount++;
				auditEntries.Add(new NpcApplyAuditEntry(
					definition.SourceKey,
					definition.Vnum,
					"skipped-deferred",
					null,
					definition.TemplateKind,
					definition.Status,
					definition.RaceName,
					definition.EthnicityName,
					definition.CultureName));
				continue;
			}

			if (invalidSourceKeys.Contains(definition.SourceKey) ||
			    definition.Status != NpcConversionStatus.Ready)
			{
				skippedInvalidCount++;
				auditEntries.Add(new NpcApplyAuditEntry(
					definition.SourceKey,
					definition.Vnum,
					"skipped-invalid",
					null,
					definition.TemplateKind,
					definition.Status,
					definition.RaceName,
					definition.EthnicityName,
					definition.CultureName));
				continue;
			}

			if (!execute)
			{
				auditEntries.Add(new NpcApplyAuditEntry(
					definition.SourceKey,
					definition.Vnum,
					"would-create",
					null,
					definition.TemplateKind,
					definition.Status,
					definition.RaceName,
					definition.EthnicityName,
					definition.CultureName));
				continue;
			}

			var npcTemplateId = ++nextTemplateId;
			var template = BuildNpcTemplate(definition, npcTemplateId);
			_context.NpcTemplates.Add(template);
			insertedCount++;
			existingMarkers.Add(marker);
			auditEntries.Add(new NpcApplyAuditEntry(
				definition.SourceKey,
				definition.Vnum,
				"created",
				npcTemplateId,
				definition.TemplateKind,
				definition.Status,
				definition.RaceName,
				definition.EthnicityName,
				definition.CultureName));

			if (insertedCount % 50 == 0)
			{
				_context.SaveChanges();
			}
		}

		if (execute)
		{
			_context.SaveChanges();
		}

		return new FutureMudNpcImportResult(
			insertedCount,
			skippedExistingCount,
			skippedDeferredCount,
			skippedInvalidCount,
			issues,
			new NpcApplyAuditReport(DateTime.UtcNow, execute, auditEntries));
	}

	private NpcTemplate BuildNpcTemplate(ConvertedNpcDefinition definition, long npcTemplateId)
	{
		_catalog.TryResolveRace(definition.RaceName, out var race);
		_catalog.TryResolveCulture(definition.CultureName, out var culture);
		_catalog.TryResolveEthnicityForDefinition(definition, race, out var ethnicity);

		var dbTemplate = new NpcTemplate
		{
			Id = npcTemplateId,
			RevisionNumber = 0,
			Name = definition.TemplateName,
			Type = definition.TemplateKind.ToString(),
			Definition = definition.TemplateKind == NpcTemplateKind.Variable
				? BuildVariableDefinition(definition, race, ethnicity, culture)
				: BuildSimpleDefinition(definition, race, ethnicity, culture),
			EditableItem = new EditableItem
			{
				RevisionNumber = 0,
				RevisionStatus = 4,
				BuilderAccountId = _catalog.BuilderAccountId,
				BuilderDate = _now,
				BuilderComment = BuildBuilderComment(definition),
				ReviewerAccountId = _catalog.BuilderAccountId,
				ReviewerDate = _now,
				ReviewerComment = "Imported by the RPI Engine Worldfile Converter."
			}
		};

		foreach (var aiName in definition.ArtificialIntelligenceNames.Distinct(StringComparer.OrdinalIgnoreCase))
		{
			if (!_catalog.TryResolveArtificialIntelligence(aiName, out var aiId))
			{
				continue;
			}

			dbTemplate.NpctemplatesArtificalIntelligences.Add(new NpcTemplatesArtificalIntelligences
			{
				Npctemplate = dbTemplate,
				NpcTemplateId = dbTemplate.Id,
				NpcTemplateRevisionNumber = dbTemplate.RevisionNumber,
				AiId = aiId
			});
		}

		return dbTemplate;
	}

	private string BuildSimpleDefinition(
		ConvertedNpcDefinition definition,
		FutureMudNpcRaceReference race,
		FutureMudNpcEthnicityReference ethnicity,
		FutureMudNpcCultureReference culture)
	{
		_catalog.TryResolveNameCultureId(culture, ethnicity, definition.SimpleGender, out var nameCultureId);
		var birthday = ResolveBirthday(definition, race, culture);
		var attributeNodes = definition.Attributes
			.Select(attribute => (_catalog.TryResolveTrait(attribute.TraitName, out var traitId), traitId, attribute))
			.Where(x => x.Item1)
			.Select(x => new XElement(
				"Attribute",
				new XAttribute("Id", x.traitId),
				new XAttribute("Value", x.attribute.Value.ToString(SystemCultureInfo.InvariantCulture))))
			.ToList();
		var skillNodes = definition.Traits
			.Select(trait => (_catalog.TryResolveTrait(trait.TraitName, out var traitId), traitId, trait))
			.Where(x => x.Item1)
			.Select(x => new XElement(
				"Skill",
				new XAttribute("Value", x.trait.Value.ToString(SystemCultureInfo.InvariantCulture)),
				x.traitId))
			.ToList();
		var accentIds = ResolveAccentIds(definition);

		return new XElement(
			"Definition",
			new XElement("OnLoadProg", 0),
			new XElement("HealthStrategy", race.DefaultHealthStrategyId),
			new XElement("DefaultCombatSetting", race.DefaultCombatSettingId ?? 0L),
			new XElement("SelectedSdesc", new XCData(definition.ShortDescription ?? string.Empty)),
			new XElement("SelectedFullDesc", new XCData(definition.FullDescription ?? definition.LongDescription ?? string.Empty)),
			new XElement("SelectedRace", race.Id),
			new XElement("SelectedCulture", culture.Id),
			new XElement("SelectedEthnicity", ethnicity.Id),
			new XElement("SelectedName", BuildNameXml(nameCultureId, definition.TechnicalName)),
			new XElement("SelectedBirthday", birthday),
			new XElement("SelectedGender", (int)definition.SimpleGender),
			new XElement("SelectedHeight", definition.HeightMetres.ToString(SystemCultureInfo.InvariantCulture)),
			new XElement("SelectedWeight", definition.WeightKilograms.ToString(SystemCultureInfo.InvariantCulture)),
			new XElement("SelectedEntityDescriptionPatterns"),
			new XElement("SelectedAttributes", attributeNodes),
			new XElement("SelectedSkills", skillNodes),
			new XElement("SelectedAccents", accentIds.Select(x => new XElement("Accent", x))),
			new XElement("SelectedCharacteristics"),
			new XElement("SelectedRoles",
				new XElement("Handedness", race.DefaultHandedness)),
			new XElement("Knowledges"),
			new XElement("MissingBodyparts"),
			new XElement("Merits")).ToString();
	}

	private string BuildVariableDefinition(
		ConvertedNpcDefinition definition,
		FutureMudNpcRaceReference race,
		FutureMudNpcEthnicityReference ethnicity,
		FutureMudNpcCultureReference culture)
	{
		var nameProfileEntries = definition.GenderChances
			.Select(chance =>
			{
				_catalog.TryResolveNameCultureId(culture, ethnicity, chance.Gender, out var nameCultureId);
				_catalog.TryResolveNameProfileId(nameCultureId, chance.Gender, out var profileId);
				return new { chance.Gender, NameCultureId = nameCultureId, ProfileId = profileId };
			})
			.ToList();
		var heightWeightEntries = definition.GenderChances
			.Select(chance =>
			{
				_catalog.TryResolveHeightWeightModelId(race, chance.Gender, out var modelId);
				return new { chance.Gender, ModelId = modelId };
			})
			.ToList();
		var accentIds = ResolveAccentIds(definition);
		var skillNodes = definition.Traits
			.Select(trait => (_catalog.TryResolveTrait(trait.TraitName, out var traitId), traitId, trait))
			.Where(x => x.Item1)
			.Select(x => new XElement(
				"Skill",
				new XAttribute("Chance", 100.0.ToString(SystemCultureInfo.InvariantCulture)),
				new XAttribute("Mean", x.trait.Value.ToString(SystemCultureInfo.InvariantCulture)),
				new XAttribute("Stddev", Math.Max(5.0, x.trait.Value * 0.1).ToString(SystemCultureInfo.InvariantCulture)),
				new XAttribute("Trait", x.traitId)))
			.ToList();

		var minimumAge = Math.Max(16, race.AdultAge);
		var maximumAge = Math.Max(minimumAge + 12, race.ElderAge > 0 ? race.ElderAge : minimumAge + 30);
		var attributeTotal = (int)Math.Round(definition.Attributes.Sum(x => x.Value), MidpointRounding.AwayFromZero);

		return new XElement(
			"Definition",
			new XElement("OnLoadProg", 0),
			new XElement("HealthStrategy", race.DefaultHealthStrategyId),
			new XElement("DefaultCombatSetting", race.DefaultCombatSettingId ?? 0L),
			new XElement("GenderChances",
				from chance in definition.GenderChances
				select new XElement(
					"GenderChance",
					new XAttribute("Gender", chance.Gender),
					new XAttribute("Chance", chance.Chance))),
			new XElement("NameProfiles",
				from entry in nameProfileEntries
				where entry.NameCultureId > 0 && entry.ProfileId > 0
				select new XElement(
					"NameProfile",
					new XAttribute("Gender", entry.Gender),
					new XAttribute("Culture", entry.NameCultureId),
					new XAttribute("Profile", entry.ProfileId))),
			new XElement("Race", race.Id),
			new XElement("Culture", culture.Id),
			new XElement("Ethnicity", ethnicity.Id),
			new XElement("MinimumAge", minimumAge),
			new XElement("MaximumAge", maximumAge),
			new XElement("AttributeTotal", attributeTotal),
			new XElement("SDescPattern", 0),
			new XElement("FDescPattern", 0),
			new XElement("HeightWeightModels",
				from entry in heightWeightEntries
				where entry.ModelId > 0
				select new XElement(
					"HeightWeightModel",
					new XAttribute("Model", entry.ModelId),
					new XAttribute("Gender", entry.Gender))),
			new XElement("PriorityAttributes"),
			new XElement("Skills", skillNodes),
			new XElement("Roles"),
			new XElement("Knowledges"),
			new XElement("Merits",
				new XAttribute("nummerits", 0),
				new XAttribute("numflaws", 0),
				new XAttribute("numquirks", 0)),
			new XElement("Accents", accentIds.Select(x => new XElement("Accent", x)))).ToString();
	}

	private IReadOnlyList<long> ResolveAccentIds(ConvertedNpcDefinition definition)
	{
		return definition.Traits
			.Where(x => x.IsLanguage)
			.Select(x => _catalog.TryResolveTrait(x.TraitName, out var traitId) && _catalog.TryResolveAccentId(traitId, out var accentId)
				? (long?)accentId
				: null)
			.Where(x => x.HasValue)
			.Select(x => x!.Value)
			.Distinct()
			.OrderBy(x => x)
			.ToList();
	}

	private string ResolveBirthday(
		ConvertedNpcDefinition definition,
		FutureMudNpcRaceReference race,
		FutureMudNpcCultureReference culture)
	{
		if (!string.IsNullOrWhiteSpace(definition.BirthdayDate))
		{
			return definition.BirthdayDate;
		}

		if (!_catalog.TryGetCurrentDate(culture.PrimaryCalendarId, out var currentDate))
		{
			return "1/1/1";
		}

		return OffsetCalendarYear(currentDate, race.AdultAge > 0 ? race.AdultAge : 25);
	}

	private static XElement BuildNameXml(long nameCultureId, string technicalName)
	{
		return new XElement(
			"Name",
			new XAttribute("culture", nameCultureId > 0 ? nameCultureId : 0),
			new XElement(
				"Element",
				new XAttribute("usage", "BirthName"),
				new XCData(technicalName)));
	}

	private static string OffsetCalendarYear(string date, int yearOffset)
	{
		if (yearOffset <= 0)
		{
			return date;
		}

		var parts = date.Split('/');
		if (parts.Length == 3 &&
		    int.TryParse(parts[2], NumberStyles.Integer, SystemCultureInfo.InvariantCulture, out var year))
		{
			parts[2] = Math.Max(1, year - yearOffset).ToString(SystemCultureInfo.InvariantCulture);
			return string.Join("/", parts);
		}

		return date;
	}

	private HashSet<string> LoadExistingMarkers()
	{
		return _context.NpcTemplates
			.Include(x => x.EditableItem)
			.Where(x => x.EditableItem != null &&
			            x.EditableItem.BuilderComment != null &&
			            x.EditableItem.BuilderComment.StartsWith("RPINPCIMPORT|"))
			.AsEnumerable()
			.Select(x => x.EditableItem!.BuilderComment.Split('\n', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? string.Empty)
			.Where(x => x.StartsWith("RPINPCIMPORT|", StringComparison.Ordinal))
			.ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static string CreateProvenanceMarker(ConvertedNpcDefinition definition)
	{
		return $"RPINPCIMPORT|{Path.GetFileName(definition.SourceFile)}|{definition.Vnum.ToString(SystemCultureInfo.InvariantCulture)}|{definition.TemplateKind}|{definition.LegacyRaceId.ToString(SystemCultureInfo.InvariantCulture)}|{definition.ZoneGroupKey}";
	}

	private static string BuildBuilderComment(ConvertedNpcDefinition definition)
	{
		return string.Join(
			'\n',
			[
				CreateProvenanceMarker(definition),
				$"Status={definition.Status}",
				$"Classification={definition.Classification}",
				$"Race={definition.RaceName ?? "unresolved"}",
				$"Ethnicity={definition.EthnicityName ?? "unresolved"}",
				$"Culture={definition.CultureName ?? "unresolved"}",
				$"AIs={string.Join(",", definition.ArtificialIntelligenceNames)}",
				$"Warnings={string.Join(" | ", definition.Warnings.Select(x => x.Message))}"
			]);
	}
}
