extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Accounts;
using MudSharp.CharacterCreation;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.Framework.Units;
using MudSharp.FutureProg;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonInstallation
{
	public static bool ClassesEnabled(FuturemudDatabaseContext db)
	{
		var shared = SeederAnswerMemory.GetLatestSeederAnswer(db, "__shared__", "use-classes");
		var own = SeederAnswerMemory.GetLatestSeederAnswer(db, new ChargenSeeder().Name, "class");
		bool Answer(string value) => value.Trim().ToLowerInvariant() switch
		{
			"yes" or "y" => true, "no" or "n" => false,
			_ => throw new InvalidOperationException("Earlier Chargen class answer must be yes/no.")
		};
		if (shared is null && own is null) throw new InvalidOperationException("Earlier Chargen class selection is missing. Run ChargenSeeder first.");
		var enabled = Answer(shared ?? own!);
		if (shared is not null && own is not null && enabled != Answer(own)) throw new InvalidOperationException("Shared and Chargen class answers disagree.");
		var storyboards = db.ChargenScreenStoryboards.AsNoTracking().Where(x => x.ChargenStage == (int)ChargenStage.SelectRole).ToArray();
		if (storyboards.Length != 1 || storyboards[0].ChargenType != "RolePicker") throw new InvalidOperationException("Exactly one native RolePicker storyboard is required to verify the earlier class direction.");
		var roles = XElement.Parse(storyboards[0].StageDefinition).Element("RoleTypes")?.Elements("RoleType").Where(x => (string?)x.Attribute("Type") == "Class").ToArray();
		if (roles is null || roles.Length != 1 || !bool.TryParse((string?)roles[0].Attribute("CanSelectNone"), out var optional) || enabled == optional)
			throw new InvalidOperationException("Chargen Class RoleType disagrees with the earlier class selection or is malformed. Resolve Chargen explicitly before Armageddon installation.");
		return enabled;
	}

	public static long ResolveCapacityAttribute(FuturemudDatabaseContext db)
	{
		var candidates = db.TraitDefinitions.AsNoTracking().Where(x => x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && (x.TraitGroup == "Mental" || x.TraitGroup == "Spiritual")).ToArray();
		var normal = candidates.Where(x => x.Name.Equals("Wisdom", StringComparison.OrdinalIgnoreCase) || x.Name.Equals("Aura", StringComparison.OrdinalIgnoreCase)).ToArray();
		var selected = normal.Length > 0 ? normal : candidates;
		return selected.Length == 1 ? selected[0].Id : 0;
	}

	public static IReadOnlyDictionary<ArmageddonNativeBinding, long> ResolveNativeBindings(FuturemudDatabaseContext db,
		IReadOnlyDictionary<ArmageddonNativeBinding, long>? overrides = null)
	{
		var result = new Dictionary<ArmageddonNativeBinding, long>();
		void Select(ArmageddonNativeBinding key, IQueryable<long> query)
		{
			if (overrides is not null && overrides.TryGetValue(key, out var explicitId)) { result[key] = explicitId; return; }
			var matches = query.Take(2).ToArray();
			result[key] = matches.Length == 1 ? matches[0] : 0;
		}
		Select(ArmageddonNativeBinding.SkillDecorator, db.TraitDecorators.Where(x => x.Name == "General Skill").Select(x => x.Id));
		Select(ArmageddonNativeBinding.SkillImprover, db.Improvers.Where(x => x.Name == "Skill Improver").Select(x => x.Id));
		Select(ArmageddonNativeBinding.AlwaysTrueProg, db.FutureProgs.Where(x => x.FunctionName == "AlwaysTrue").Select(x => x.Id));
		Select(ArmageddonNativeBinding.AlwaysFalseProg, db.FutureProgs.Where(x => x.FunctionName == "AlwaysFalse").Select(x => x.Id));
		Select(ArmageddonNativeBinding.HumanRace, db.Races.Where(x => x.Name == "Human").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Culture, db.Cultures.Where(x => x.Name == "Admin").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Language, db.Languages.Where(x => x.Name == "Admin Speech").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Wood, db.Materials.Where(x => x.Name == "wood").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Stone, db.Materials.Where(x => x.Name == "stone").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Water, db.Liquids.Where(x => x.Name == "water").Select(x => x.Id));
		Select(ArmageddonNativeBinding.Kilogram, db.UnitsOfMeasure.Where(x => x.Name == "kilogram" && x.System == "Metric" && x.Type == (int)UnitType.Mass).Select(x => x.Id));
		Select(ArmageddonNativeBinding.Metre, db.UnitsOfMeasure.Where(x => x.Name == "metre" && x.System == "Metric" && x.Type == (int)UnitType.Length).Select(x => x.Id));
		Select(ArmageddonNativeBinding.Litre, db.UnitsOfMeasure.Where(x => x.Name == "litre" && x.System == "Metric" && x.Type == (int)UnitType.FluidVolume).Select(x => x.Id));
		Select(ArmageddonNativeBinding.BuilderAccount, db.Accounts.Where(x => x.AuthorityGroup.AuthorityLevel == (int)PermissionLevel.Founder).Select(x => x.Id));
		return result;
	}

	public static IReadOnlyList<string> ValidateFoundations(FuturemudDatabaseContext db, ArmageddonInstallationSettings settings, bool checkCapacity = true)
	{
		var errors = new List<string>();
		var bindings = settings.NativeBindings;
		long Id(ArmageddonNativeBinding key) => bindings.TryGetValue(key, out var value) ? value : 0;
		foreach (var key in Enum.GetValues<ArmageddonNativeBinding>()) if (Id(key) <= 0) errors.Add($"Native {key} is missing/ambiguous. Supply an explicit compatible binding or install its foundation.");
		var decoratorId = Id(ArmageddonNativeBinding.SkillDecorator);
		var improverId = Id(ArmageddonNativeBinding.SkillImprover);
		var raceId = Id(ArmageddonNativeBinding.HumanRace);
		var cultureId = Id(ArmageddonNativeBinding.Culture);
		var languageId = Id(ArmageddonNativeBinding.Language);
		var waterId = Id(ArmageddonNativeBinding.Water);
		var authorId = Id(ArmageddonNativeBinding.BuilderAccount);
		if (checkCapacity && !db.TraitDefinitions.Any(x => x.Id == settings.CapacityAttribute && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && (x.TraitGroup == "Mental" || x.TraitGroup == "Spiritual"))) errors.Add("Capacity requires one ordinary body Mental/Spiritual attribute.");
		var decorator = db.TraitDecorators.AsNoTracking().SingleOrDefault(x => x.Id == decoratorId);
		if (decorator is null || !new[] { "SimpleNumeric", "Range", "CurrentMax", "Percentage" }.Contains(decorator.Type, StringComparer.OrdinalIgnoreCase)) errors.Add("Select a compatible native skill decorator.");
		var improver = db.Improvers.AsNoTracking().SingleOrDefault(x => x.Id == improverId);
		if (improver is null || !new[] { "classic", "branching" }.Contains(improver.Type, StringComparer.OrdinalIgnoreCase)) errors.Add("Select a native classic/branching skill improver.");
		var progs = db.FutureProgs.AsNoTracking().Include(x => x.FutureProgsParameters).ToArray();
		var culture = db.Cultures.AsNoTracking().SingleOrDefault(x => x.Id == cultureId);
		try
		{
			using var compiler = new OfflineProgCompilation(progs);
			foreach (var (key, text) in new[] { (ArmageddonNativeBinding.AlwaysTrueProg, "return true"), (ArmageddonNativeBinding.AlwaysFalseProg, "return false") })
			{
				var row = progs.SingleOrDefault(x => x.Id == Id(key));
				if (row is null || !row.FunctionText.Trim().Equals(text, StringComparison.OrdinalIgnoreCase)) { errors.Add($"Native {key} must be the ordinary constant Boolean prog."); continue; }
				var compiled = compiler.Compile(row.Id);
				if (compiled.ReturnType != ProgVariableTypes.Boolean || !compiled.MatchesParameters([])) errors.Add($"Native {key} must compile as Boolean with no arguments.");
			}
			if (culture is not null && progs.Any(x => x.Id == culture.SkillStartingValueProgId))
			{
				var starting = compiler.Compile(culture.SkillStartingValueProgId);
				if (!starting.ReturnType.CompatibleWith(ProgVariableTypes.Number) || !starting.MatchesParameters([ProgVariableTypes.Chargen, ProgVariableTypes.Trait, ProgVariableTypes.Number]))
					errors.Add("Native culture skill-starting prog must compile as Number with the native Chargen/Trait/Number signature.");
			}
		}
		catch (Exception error) when (error is InvalidOperationException or ArgumentException) { errors.Add("Native prog preflight: " + error.Message); }
		var race = db.Races.AsNoTracking().SingleOrDefault(x => x.Id == raceId);
		if (race is null || !db.BodyProtos.Any(x => x.Id == race.BaseBodyId) || !db.HealthStrategies.Any(x => x.Id == race.DefaultHealthStrategyId) || !db.BodypartProtos.Any(x => x.BodyId == race.BaseBodyId)) errors.Add("Native race/body/anatomy/health foundation is incomplete.");
		if (culture is null || !db.Calendars.Any(x => x.Id == culture.PrimaryCalendarId) || !db.FutureProgs.Any(x => x.Id == culture.SkillStartingValueProgId) || !db.CulturesNameCultures.Any(x => x.CultureId == culture.Id && db.NameCultures.Any(n => n.Id == x.NameCultureId))) errors.Add("Native culture/calendar/skill-prog/name-culture foundation is incomplete.");
		var language = db.Languages.AsNoTracking().SingleOrDefault(x => x.Id == languageId);
		if (language is null || !db.TraitDefinitions.Any(x => x.Id == language.LinkedTraitId && x.Type == 0) || !db.LanguageDifficultyModels.Any(x => x.Id == language.DifficultyModel)) errors.Add("Native language/skill/difficulty foundation is incomplete.");
		foreach (var (key, behaviour) in new[] { (ArmageddonNativeBinding.Wood, MaterialBehaviourType.Wood), (ArmageddonNativeBinding.Stone, MaterialBehaviourType.Stone) })
		{
			var materialId = Id(key);
			var material = db.Materials.AsNoTracking().SingleOrDefault(x => x.Id == materialId);
			if (material is null || material.BehaviourType != (int)behaviour || !double.IsFinite(material.Density) || material.Density <= 0) errors.Add($"Native {key} material has incompatible behaviour/density.");
		}
		var liquid = db.Liquids.AsNoTracking().SingleOrDefault(x => x.Id == waterId);
		if (liquid is null || !double.IsFinite(liquid.Density) || liquid.Density <= 0) errors.Add("Native water liquid is missing or has invalid density.");
		foreach (var (key, type) in new[] { (ArmageddonNativeBinding.Kilogram, UnitType.Mass), (ArmageddonNativeBinding.Metre, UnitType.Length), (ArmageddonNativeBinding.Litre, UnitType.FluidVolume) })
		{
			var unitId = Id(key);
			var unit = db.UnitsOfMeasure.AsNoTracking().SingleOrDefault(x => x.Id == unitId);
			if (unit is null || unit.Type != (int)type || !double.IsFinite(unit.BaseMultiplier) || unit.BaseMultiplier <= 0 || unit.PreMultiplierBaseOffset != 0 || unit.PostMultiplierBaseOffset != 0) errors.Add($"Native {key} must be a compatible finite positive zero-offset unit.");
		}
		if (!db.Accounts.Any(x => x.Id == authorId && x.AuthorityGroup.AuthorityLevel == (int)PermissionLevel.Founder)) errors.Add("Native content author must be an existing Founder account.");
		foreach (var type in new[] { CheckType.CastSpellCheck, CheckType.TraitBranchCheck, CheckType.CraftOutcomeCheck, CheckType.CraftQualityCheck })
		{
			var check = db.Checks.AsNoTracking().SingleOrDefault(x => x.Type == (int)type);
			if (check is null || !db.CheckTemplates.Any(x => x.Id == check.CheckTemplateId) || !db.TraitExpressions.Any(x => x.Id == check.TraitExpressionId)) errors.Add($"Native {type} check/template/expression foundation is incomplete.");
		}
		return errors;
	}

	private static IReadOnlyList<string> ValidateLegacyOwnership(FuturemudDatabaseContext db)
	{
		var errors = new List<string>();
		var modules = new[] { ArmageddonMagicInstaller.Module, ArmageddonTraditionInstaller.Module, ArmageddonPierceInstaller.Module, ArmageddonProvisionInstaller.Module, ArmageddonWaterSeeInstaller.Module, ArmageddonEmotionalInstaller.Module, ArmageddonProgressionInstaller.Module, ArmageddonCatalogueOwnership.Module, ArmageddonStockSpellInstaller.Module };
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey != SettingsKey).ToArray();
		foreach (var record in records)
		{
			if (record.Module == ArmageddonGatheringRegisters.Module)
			{
				if (!ArmageddonGatheringRegisters.Valid(db, record)) errors.Add($"Existing Armageddon {record.StableKey} has invalid typed gathering register ownership.");
				continue;
			}
			if (record.Module == ArmageddonCatalogueLinks.Module)
			{
				if (!ArmageddonCatalogueLinks.Valid(db, record)) errors.Add($"Existing Armageddon {record.StableKey} has invalid typed catalogue reference ownership.");
				continue;
			}
			var exists = record.EntityType switch
			{
				nameof(MagicSpell) => db.MagicSpells.Any(x => x.Id == record.LogicalId),
				nameof(MagicCapability) => db.MagicCapabilities.Any(x => x.Id == record.LogicalId),
				nameof(Merit) => db.Merits.Any(x => x.Id == record.LogicalId),
				nameof(MagicSchool) => db.MagicSchools.Any(x => x.Id == record.LogicalId),
				nameof(MagicResource) => db.MagicResources.Any(x => x.Id == record.LogicalId),
				nameof(MagicGenerator) => db.MagicGenerators.Any(x => x.Id == record.LogicalId),
				nameof(RegionalGatheringProfile) => record.RevisionNumber is null && db.RegionalGatheringProfiles.Any(x => x.Id == record.LogicalId),
				nameof(Terrain) => record.RevisionNumber is null && db.Terrains.Any(x => x.Id == record.LogicalId),
				nameof(ForagableProfile) => record.RevisionNumber is not null && db.ForagableProfiles.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				nameof(ChargenRole) => db.ChargenRoles.Any(x => x.Id == record.LogicalId),
				nameof(TraitExpression) => db.TraitExpressions.Any(x => x.Id == record.LogicalId),
				nameof(TraitDefinition) => db.TraitDefinitions.Any(x => x.Id == record.LogicalId),
				nameof(Improver) => db.Improvers.Any(x => x.Id == record.LogicalId),
				nameof(FutureProg) => db.FutureProgs.Any(x => x.Id == record.LogicalId),
				nameof(Tag) => db.Tags.Any(x => x.Id == record.LogicalId),
				nameof(Plane) => db.Planes.Any(x => x.Id == record.LogicalId),
				nameof(Liquid) => db.Liquids.Any(x => x.Id == record.LogicalId),
				nameof(WeaponType) => db.WeaponTypes.Any(x => x.Id == record.LogicalId),
				nameof(ArmourType) => record.RevisionNumber is null && db.ArmourTypes.Any(x => x.Id == record.LogicalId),
				nameof(WeaponAttack) => db.WeaponAttacks.Any(x => x.Id == record.LogicalId),
				nameof(CombatMessage) => db.CombatMessages.Any(x => x.Id == record.LogicalId),
				nameof(BodyProto) => db.BodyProtos.Any(x => x.Id == record.LogicalId),
				nameof(Race) => db.Races.Any(x => x.Id == record.LogicalId),
				nameof(Culture) => db.Cultures.Any(x => x.Id == record.LogicalId),
				nameof(Ethnicity) => db.Ethnicities.Any(x => x.Id == record.LogicalId),
				nameof(NpcTemplate) => record.RevisionNumber is not null && db.NpcTemplates.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				nameof(ArtificialIntelligence) => db.ArtificialIntelligences.Any(x => x.Id == record.LogicalId),
				nameof(WearProfile) => db.WearProfiles.Any(x => x.Id == record.LogicalId),
				nameof(Drug) => db.Drugs.Any(x => x.Id == record.LogicalId),
				nameof(BodypartShape) => db.BodypartShapes.Any(x => x.Id == record.LogicalId),
				nameof(BodypartProto) => db.BodypartProtos.Any(x => x.Id == record.LogicalId),
				nameof(Limb) => db.Limbs.Any(x => x.Id == record.LogicalId),
				nameof(MoveSpeed) => db.MoveSpeeds.Any(x => x.Id == record.LogicalId),
				nameof(BodypartProtoAlignmentHits) => db.BodypartProtoAlignmentHits.Any(x => x.Id == record.LogicalId),
				nameof(BodypartProtoOrientationHits) => db.BodypartProtoOrientationHits.Any(x => x.Id == record.LogicalId),
				nameof(AutobuilderRoomTemplate) => db.AutobuilderRoomTemplates.Any(x => x.Id == record.LogicalId),
				nameof(Craft) => record.RevisionNumber is not null && db.Crafts.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				nameof(Foragable) => record.RevisionNumber is not null && db.Foragables.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				nameof(GameItemProto) => record.RevisionNumber is not null && db.GameItemProtos.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				nameof(GameItemComponentProto) => record.RevisionNumber is not null && db.GameItemComponentProtos.Any(x => x.Id == record.LogicalId && x.RevisionNumber == record.RevisionNumber),
				_ => false
			};
			if (!modules.Contains(record.Module) || record.Retired || record.LogicalId is null || !exists || db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				errors.Add($"Existing Armageddon {record.StableKey} has unknown/incomplete/retired/competing typed ownership; preserve and resolve it before the full installation.");
		}
		return errors;
	}
}
