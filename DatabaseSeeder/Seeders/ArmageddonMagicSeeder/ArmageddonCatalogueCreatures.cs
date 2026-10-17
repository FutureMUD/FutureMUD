extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using NativeCalendar = EngineCompiler::MudSharp.TimeAndDate.Date.Calendar;
using NativeTraitExpression = EngineCompiler::MudSharp.Body.Traits.TraitExpression;

namespace DatabaseSeeder.Seeders;

/// <summary>Current native Simple templates with exact owned creature and controller links.</summary>
public static class ArmageddonCatalogueCreatures
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before creature templates.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		var catalogue = new ArmageddonStockCatalogue();
		var creatures = catalogue.Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.Creature).ToArray();
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package)
			.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
		long Owned(string key, string type)
		{
			if (!records.TryGetValue(key, out var record) || record.Module != ArmageddonCatalogueOwnership.Module ||
				record.EntityType != type || record.Retired || record.LogicalId is not > 0 || record.RevisionNumber is not null)
				throw new InvalidOperationException($"{key}: install its exact owned {type} dependency before creature templates.");
			return record.LogicalId.Value;
		}
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, ids);
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var cultureId = Owned("arm.culture.constructs", nameof(Culture));
			var culture = db.Cultures.AsNoTracking().Include(x => x.CulturesNameCultures).Single(x => x.Id == cultureId);
			var calendarRow = db.Calendars.AsNoTracking().Single(x => x.Id == culture.PrimaryCalendarId);
			var calendar = new NativeCalendar(XElement.Parse(calendarRow.Definition));
			var nameCultureId = culture.CulturesNameCultures.Where(x => x.Gender == (short)MudSharp.Form.Shape.Gender.Neuter)
				.OrderBy(x => x.NameCultureId).Select(x => x.NameCultureId).First();
			if (!db.NameCultures.Any(x => x.Id == nameCultureId)) throw new InvalidOperationException("The owned culture's neuter name culture is missing.");
			var language = db.Languages.AsNoTracking().Single(x => x.Id == culture.NativeLanguageId);
			var accentId = ArmageddonCatalogueLinks.Bind(db, "arm.binding.creatures.accent", nameof(Accent), () =>
				db.Accents.AsNoTracking().Where(x => x.LanguageId == language.Id).OrderBy(x => x.Id).Select(x => x.Id).First());
			if (!db.Accents.Any(x => x.Id == accentId && x.LanguageId == language.Id))
				throw new InvalidOperationException("The retained creature accent no longer belongs to its native language.");
			var combatSkills = CheckSkills(db, CheckType.NaturalWeaponAttack, CheckType.DodgeCheck);
			var flightSkills = CheckSkills(db, CheckType.FlyCheck);
			var trackingSkills = CheckSkills(db, CheckType.SearchForTracksCheck, CheckType.SearchForTracksByScentScheck);
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			var metres = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Metre]).BaseMultiplier;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			foreach (var blueprint in creatures)
			{
				var config = blueprint.Configuration;
				var raceKey = config.GetProperty("race_bundle").GetString()!;
				var raceBlueprint = catalogue.Blueprints.Single(x => x.Key == raceKey);
				var raceId = Owned(raceKey, nameof(Race));
				var race = db.Races.AsNoTracking().Include(x => x.RacesAttributes).Single(x => x.Id == raceId);
				var ethnicityKey = raceBlueprint.Configuration.GetProperty("ethnicity_key").GetString()!;
				var ethnicityId = Owned(ethnicityKey, nameof(Ethnicity));
				if (!db.Ethnicities.Any(x => x.Id == ethnicityId && x.ParentRaceId == raceId))
					throw new InvalidOperationException($"{blueprint.Key}: owned ethnicity/race link is incompatible.");
				var aiKey = config.GetProperty("ai_bundle").GetString()!;
				var aiId = Owned(aiKey, nameof(ArtificialIntelligence));
				if (!db.ArtificialIntelligences.Any(x => x.Id == aiId)) throw new InvalidOperationException($"{aiKey}: owned native AI is missing.");
				var affinityKey = raceKey + ".affinity";
				var affinityId = Owned(affinityKey, nameof(Merit));
				if (!db.Merits.Any(x => x.Id == affinityId && x.Type == "Elemental Affinity"))
					throw new InvalidOperationException($"{affinityKey}: owned declared affinity merit is missing or incompatible.");
				var skills = combatSkills.ToDictionary(x => x, _ => config.GetProperty("combat_skill").GetDouble());
				if (raceBlueprint.Configuration.GetProperty("body_plan").GetString()!.StartsWith("winged", StringComparison.Ordinal))
					foreach (var id in flightSkills) skills[id] = Math.Max(skills.GetValueOrDefault(id), config.GetProperty("combat_skill").GetDouble());
				var tracking = config.GetProperty("tracking_skill").GetDouble();
				if (tracking > 0)
					foreach (var id in trackingSkills) skills[id] = Math.Max(skills.GetValueOrDefault(id), tracking);
				skills[language.LinkedTraitId] = 100;
				var birthday = RetainedBirthday(records.GetValueOrDefault(blueprint.Key));
				if (birthday is null)
				{
					var adultDate = calendar.GetDate(calendarRow.Date);
					adultDate.AdvanceYears(-Math.Max(18, race.AdultAge), true);
					birthday = adultDate.GetDateString();
				}
				else _ = calendar.GetDate(birthday); // Reject an incompatible stored date rather than silently replacing it.
				var template = new NpcTemplate
				{
					Id = owned.Id<NpcTemplate>(blueprint.Key), RevisionNumber = 0, Type = "Simple",
					Name = "Armageddon " + blueprint.Name, UniqueName = blueprint.Key, EditableItem = owned.Approval(),
					BuilderNotes = "Stock magical creature. Grade-specific native stats; spell invocation and lifetime binding are S4.",
					Definition = Definition(blueprint, raceBlueprint, race, ethnicityId, cultureId, nameCultureId,
						birthday, language.Id, accentId, affinityId, skills, metres, kilograms).ToString()
				};
				if (db.NpcTemplates.Any(x => x.Id != template.Id && x.UniqueName == template.UniqueName))
					throw new InvalidOperationException($"{blueprint.Key}: unowned native unique-name collision; no adoption.");
				template.NpctemplatesArtificalIntelligences.Add(new() { NpcTemplateId = template.Id,
					NpcTemplateRevisionNumber = template.RevisionNumber, AiId = aiId });
				if (raceKey == "arm.race.earth_mount")
				{
					var mountId = Owned(aiKey + ".riding", nameof(ArtificialIntelligence));
					if (!db.ArtificialIntelligences.Any(x => x.Id == mountId && x.Type == "Mount"))
						throw new InvalidOperationException("Earth Mount's owned native riding AI is missing or incompatible.");
					template.NpctemplatesArtificalIntelligences.Add(new() { NpcTemplateId = template.Id,
						NpcTemplateRevisionNumber = template.RevisionNumber, AiId = mountId });
				}
				var dependencies = new List<string> { raceKey, ethnicityKey, aiKey, affinityKey, "arm.culture.constructs" };
				if (raceKey == "arm.race.earth_mount") dependencies.Add(aiKey + ".riding");
				owned.Put(blueprint.Key, template, dependencies);
				ids.Add(blueprint.Key, template.Id);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 creature templates committed: 49 approved native Simple templates with exact owned anatomy, affinity and controller links.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerunning. " :
				"Creature templates failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown :
				ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static long[] CheckSkills(FuturemudDatabaseContext db, params CheckType[] types)
	{
		var ids = new HashSet<long>();
		foreach (var type in types)
		{
			var check = db.Checks.AsNoTracking().Single(x => x.Type == (int)type);
			var row = db.TraitExpressions.AsNoTracking().Include(x => x.TraitExpressionParameters).Single(x => x.Id == check.TraitExpressionId);
			// The native expression loader owns this grammar and resolves row/inline precedence.
			// Accessing its typed metadata never evaluates the expression or loads a character.
			var expression = new NativeTraitExpression(row, null!);
			foreach (var id in expression.TraitReferenceIds.Values)
			{
				var trait = db.TraitDefinitions.AsNoTracking().Single(x => x.Id == id);
				if (trait.Type == 0) ids.Add(id);
			}
		}
		return ids.Order().ToArray();
	}

	private static string? RetainedBirthday(SeederManagedRecord? record)
	{
		if (record?.SeedBaseline is null) return null;
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline)!;
		return XElement.Parse(JsonSerializer.Deserialize<string>(baseline["Definition"])!).Element("SelectedBirthday")!.Value;
	}

	private static XElement Definition(ArmageddonStockBlueprint blueprint, ArmageddonStockBlueprint raceBlueprint, Race race,
		long ethnicityId, long cultureId, long nameCultureId, string birthday, long languageId, long accentId,
		long affinityId, IReadOnlyDictionary<long, double> skills, double metres, double kilograms)
	{
		var family = blueprint.Name[..blueprint.Name.LastIndexOf(" grade ", StringComparison.Ordinal)];
		return new("Definition", new XElement("OnLoadProg", 0), new XElement("HealthStrategy", 0), new XElement("DefaultCombatSetting", 0),
			new XElement("TemplateLoadAdditions", new XElement("ClanMemberships"), new XElement("Outfits"), new XElement("Hooks"),
				new XElement("BankAccounts"), new XElement("BodyLoadout")),
			new XElement("SelectedEntityDescriptionPatterns"), new XElement("SelectedSdesc", "a conjured " + family.ToLowerInvariant()),
			new XElement("SelectedFullDesc", $"A tangible {family.ToLowerInvariant()} shaped by magic, with its own form and natural limbs."),
			new XElement("SelectedRace", race.Id), new XElement("SelectedCulture", cultureId), new XElement("SelectedEthnicity", ethnicityId),
			new XElement("SelectedName", new XElement("Name", new XAttribute("culture", nameCultureId),
				new XElement("Element", new XAttribute("usage", "BirthName"), family))),
			new XElement("SelectedBirthday", birthday), new XElement("SelectedGender", (short)MudSharp.Form.Shape.Gender.Neuter),
			new XElement("SelectedHeight", metres * raceBlueprint.Configuration.GetProperty("height_metres").GetDouble()),
			new XElement("SelectedWeight", kilograms * raceBlueprint.Configuration.GetProperty("weight_kilograms").GetDouble()),
			new XElement("SelectedAttributes", race.RacesAttributes.OrderBy(x => x.AttributeId).Select(x => new XElement("Attribute",
				new XAttribute("Id", x.AttributeId), new XAttribute("Value", blueprint.Configuration.GetProperty("attribute_value").GetDouble())))),
			new XElement("SelectedSkills", skills.OrderBy(x => x.Key).Select(x => new XElement("Skill", new XAttribute("Value", x.Value), x.Key))),
			new XElement("SelectedNativeLanguage", languageId), new XElement("SelectedAccents", new XElement("Accent", accentId)),
			new XElement("SelectedCharacteristics"), new XElement("SelectedRoles", new XElement("Handedness", race.DefaultHandedness)),
			new XElement("Knowledges"), new XElement("MissingBodyparts"), new XElement("Merits", new XElement("Merit", affinityId)));
	}
}
