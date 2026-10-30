#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.RPG.Checks;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Models;
using MudSharp.FutureProg;

namespace DatabaseSeeder.Seeders;

/// <summary>Separate additive class/progression inventory; frozen guild/subguild tables stay unchanged.</summary>
public static partial class ArmageddonTemplarInstaller
{
	public const string Module = "setting-templar-classes";
	public const string ProgressionKey = "arm.merit.templar_role_progression";
	private static readonly string[] RoleFields = ["Name", "Type", "PosterId", "ChargenBlurb", "AvailabilityProgId", "Expired", "MaximumNumberAlive", "MaximumNumberTotal", "MinimumAuthorityToApprove", "MinimumAuthorityToView"];
	private sealed record Manifest(int Format, string Version, Role[] Roles, SouthernMagic SouthernMagic, string[] SourceAdaptations);
	private sealed record Role(string Key, string Name, string Type, string? Gender, string? ProgressionFrom, Skill[] Skills);
	private sealed record Skill(string Trait, double Opening, double Cap, double GoOn, string? Parent = null, bool Inferred = false, bool InferredTrait = false);
	private sealed record SouthernMagic(int FixedGrade, string[] PermanentSpellKeys, int SelectionLimit, Dictionary<string, string[]> Choices,
		bool ActivationDefault, string TerritoryProgKey, string AuthorityProgKey, string[] GemInterceptSpellKeys);
	private sealed record ProgParameter(long ParameterType, string ParameterName);

	public static IReadOnlyDictionary<string, double> BuildRoleRuleThresholds(IEnumerable<(string Trait, string? Parent, double GoOn)> skills,
		bool sourceThresholdsAlreadyResolved = false)
	{
		var values = skills.ToDictionary(x => x.Trait, x => x, StringComparer.Ordinal);
		return values.Values.ToDictionary(x => x.Trait,
		x => sourceThresholdsAlreadyResolved ? x.GoOn : x.Parent is { } parent ? values[parent].GoOn : 0.0, StringComparer.Ordinal);
	}

	private static Manifest ReadManifest()
	{
		using var stream = typeof(ArmageddonTemplarInstaller).Assembly.GetManifestResourceStream("DatabaseSeeder.Armageddon.templar_classes.json")
			?? throw new InvalidOperationException("Missing Armageddon Templar class manifest.");
		var result = JsonSerializer.Deserialize<Manifest>(stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower })
			?? throw new InvalidOperationException("Invalid Armageddon Templar class manifest.");
		if (result.Format != 1 || result.Roles.Length != 3 || result.Roles.Select(x => x.Key).Distinct(StringComparer.Ordinal).Count() != 3 ||
			result.Roles.Single(x => x.Key == "arm.class.jihaen_templar").Gender != "Male" ||
			result.Roles.Single(x => x.Key == "arm.class.lirathan_templar").Gender != "Female" ||
			result.SouthernMagic.SelectionLimit != 3 || result.SouthernMagic.FixedGrade != 7 ||
			result.SouthernMagic.GemInterceptSpellKeys is not { Length: > 0 } || result.SouthernMagic.GemInterceptSpellKeys.Distinct().Count() != result.SouthernMagic.GemInterceptSpellKeys.Length ||
			result.Roles.Any(x => x.Skills.Select(y => y.Trait).Distinct(StringComparer.Ordinal).Count() != x.Skills.Length))
			throw new InvalidOperationException("Templar class manifest has invalid identities, gender defaults, or spell policy.");
		foreach (var role in result.Roles.Where(x => x.ProgressionFrom is null))
		{
			var keys = role.Skills.Select(x => x.Trait).ToHashSet(StringComparer.Ordinal);
			if (role.Skills.Any(x => x.Parent is not null && !keys.Contains(x.Parent))) throw new InvalidOperationException($"{role.Name} has an unknown parent skill.");
		}
		return result;
	}

	public static void Install(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var manifest = ReadManifest();
		var poster = db.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse");
		var skills = ArmageddonSkillPackage.Resolve(db).ToDictionary(x => x.Key, x => x.Value, StringComparer.Ordinal);
		ArmageddonLanguages.AddInstalledSkills(db, skills);
		using var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
		foreach (var skill in manifest.Roles.SelectMany(x => x.Skills).Where(x => x.InferredTrait))
		{
			var key = skill.Trait;
			if (skills.ContainsKey(key)) continue;
			var name = key switch { "arm.skill.magicksense" => "Magick Sense", "arm.skill.tatlum" => "Tatlum", _ => key.Split('.').Last().Split('_').Select(x => char.ToUpperInvariant(x[0]) + x[1..]).Aggregate((a, b) => a + " " + b) };
			var owned = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module && x.StableKey == key);
			if (owned is not null)
			{
				if (owned.EntityType != nameof(TraitDefinition) || owned.LogicalId is not { } id || owned.Retired) throw new InvalidOperationException($"Invalid Templar skill ownership for {key}.");
				skills.Add(key, db.TraitDefinitions.Include(x => x.Expression).Single(x => x.Id == id));
				continue;
			}
			if (db.TraitDefinitions.Any(x => x.Name == name)) throw new InvalidOperationException($"Unowned Templar skill name collision: {name}.");
			var template = skills["arm.skill.contact"];
			var trait = new TraitDefinition { Name = name, Type = 0, OwnerScope = (int)TraitOwnerScope.Character, DerivedType = 0,
				TraitGroup = template.TraitGroup, Hidden = false, Expression = new TraitExpression { Name = key + ".cap", Expression = "0" },
				DecoratorId = template.DecoratorId, ImproverId = template.ImproverId, AvailabilityProg = no,
				LearnableProg = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue"), TeachableProg = no,
				TeachDifficulty = 7, LearnDifficulty = 7, ChargenBlurb = "", BranchMultiplier = 1.0 };
			db.TraitDefinitions.Add(trait); db.SaveChanges();
			ArmageddonSettingOwnership.RecordNew(db, Module, key, trait, nameof(TraitDefinition.Name), nameof(TraitDefinition.TraitGroup), nameof(TraitDefinition.Type),
				nameof(TraitDefinition.OwnerScope), nameof(TraitDefinition.DerivedType), nameof(TraitDefinition.ExpressionId), nameof(TraitDefinition.DecoratorId),
				nameof(TraitDefinition.ImproverId), nameof(TraitDefinition.Hidden), nameof(TraitDefinition.AvailabilityProgId), nameof(TraitDefinition.LearnableProgId),
				nameof(TraitDefinition.TeachableProgId), nameof(TraitDefinition.TeachDifficulty), nameof(TraitDefinition.LearnDifficulty),
				nameof(TraitDefinition.BranchMultiplier), nameof(TraitDefinition.ChargenBlurb));
			ArmageddonSettingOwnership.RecordNew(db, Module, key + ".cap", trait.Expression, nameof(TraitExpression.Name), nameof(TraitExpression.Expression));
			skills.Add(key, trait);
		}

		// Heritage owns the complete language catalogue. Reuse its Tatlum records (or
		// legacy Templar identities reconciled by Heritage) without reverting their accent policies.
		if (db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.language.tatlum"))
		{
			var language = Owned<Language>(db, "arm.language.tatlum");
			if (language.LinkedTraitId != skills["arm.skill.tatlum"].Id ||
				new[] { "standard", "learner" }.Any(x => Owned<Accent>(db, "arm.accent.tatlum." + x).LanguageId != language.Id))
				throw new InvalidOperationException("Tatlum language/accent bindings are incompatible; preserve and resolve them before Templar installation.");
		}
		else
		{
			var tatlum = (Language)Put(db, "arm.language.tatlum", new Language { Name = "Tatlum", LinkedTraitId = skills["arm.skill.tatlum"].Id,
				DifficultyModel = db.Languages.OrderBy(x => x.Id).First().DifficultyModel, UnknownLanguageDescription = ArmageddonLanguages.Profiles.Single(x => x.Slug == "tatlum").UnknownSpeech, LanguageObfuscationFactor = 0.1 },
				[nameof(Language.Name), nameof(Language.LinkedTraitId), nameof(Language.DifficultyModel), nameof(Language.UnknownLanguageDescription), nameof(Language.LanguageObfuscationFactor)], messages);
			foreach (var learner in new[] { false, true })
				Put(db, "arm.accent.tatlum." + (learner ? "learner" : "standard"), new Accent { LanguageId = tatlum.Id, Name = learner ? "Learner" : "Standard",
					Role = learner ? 2 : 0, Difficulty = learner ? 6 : 5, Group = "Templar language scaffold", Suffix = learner ? "with a learner's accent" : "in the standard accent",
					VagueSuffix = learner ? "with an unfamiliar accent" : "in the standard accent", Description = learner ? "The pronunciation of someone learning Tatlum as an additional language." : "The usual pronunciation of Tatlum.",
					ChargenAvailabilityProgId = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id },
					[nameof(Accent.LanguageId), nameof(Accent.Name), nameof(Accent.Role), nameof(Accent.Difficulty), nameof(Accent.Group), nameof(Accent.Suffix), nameof(Accent.VagueSuffix), nameof(Accent.Description), nameof(Accent.ChargenAvailabilityProgId)], messages);
		}
		var resolvedRoles = new Dictionary<string, ChargenRole>(StringComparer.Ordinal);
		foreach (var sourceRole in manifest.Roles)
		{
			var desired = new ChargenRole { Name = sourceRole.Name, Type = 0, PosterId = poster, AvailabilityProgId = AvailabilityProg(db, sourceRole, messages).Id,
				ChargenBlurb = ArmageddonPresentation.RoleDescription(sourceRole.Name,
				(sourceRole.ProgressionFrom is not null
					? ArmageddonMundaneProgression.Roles.Single(x => x.Name == "Warrior").Skills.Where(x => x.Parent is null && x.Opening > 0).Select(x => skills[x.Key].Name)
					: sourceRole.Skills.Where(x => x.Parent is null && x.Opening > 0).Select(x => skills[x.Trait].Name))),
				Expired = false, MaximumNumberAlive = 0, MaximumNumberTotal = 0, MinimumAuthorityToApprove = 0, MinimumAuthorityToView = 0 };
			var actual = (ChargenRole)Put(db, sourceRole.Key, desired, RoleFields, messages);
			resolvedRoles.Add(sourceRole.Key, actual);
		}

		var roleRules = new List<RoleSkillRule>();
		foreach (var role in manifest.Roles)
		{
			var copied = role.ProgressionFrom is not null;
			var all = role.ProgressionFrom is { } source
				? ArmageddonMundaneProgression.Roles.Single(x => x.Name == (source == "arm.class.warrior" ? "Warrior" : source)).Skills.Select(x => new Skill(x.Key, x.Opening, x.Cap, x.Threshold, x.Parent)).ToArray()
				: role.Skills;
			var thresholds = BuildRoleRuleThresholds(all.Select(x => (x.Trait, x.Parent, x.GoOn)), copied);
			foreach (var skill in all.Where(x => x.Trait != "arm.skill.analyze"))
			{
				if (!skills.TryGetValue(skill.Trait, out var trait))
				{
					trait = db.TraitDefinitions.Include(x => x.Expression).AsEnumerable().SingleOrDefault(x => x.Name.Equals(skill.Trait.Split('.').Last().Replace('_', ' '), StringComparison.OrdinalIgnoreCase))
						?? throw new InvalidOperationException($"Missing canonical Templar skill {skill.Trait}; install it before the Templar module.");
					skills.Add(skill.Trait, trait);
				}
				var parent = skill.Parent is null ? null : skills.TryGetValue(skill.Parent, out var parentTrait) ? parentTrait.Id :
					db.TraitDefinitions.AsEnumerable().SingleOrDefault(x => x.Name.Equals(skill.Parent.Split('.').Last().Replace('_', ' '), StringComparison.OrdinalIgnoreCase))?.Id;
				if (skill.Parent is not null && parent is null) throw new InvalidOperationException($"Missing parent Templar skill {skill.Parent}.");
				roleRules.Add(new(resolvedRoles[role.Key].Id, trait.Id, true, skill.Opening, skill.Cap, parent, thresholds[skill.Trait], 0));
			}
		}
		_ = new RoleSkillProgression(roleRules);
		var definition = new XElement("Definition", new XElement("ChargenAvailableProg", no.Id), new XElement("ApplicabilityProg", db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id),
			new XElement("DescriptionText", "@ follow|follows the builder-configured Jihaen, Lirathan, and Southern Templar skill progression"),
			new XElement("ChargenBlurb", "Templar role skill progression"), new XElement("Rules", roleRules.Select(x => new XElement("Rule",
				new XAttribute("role", x.RoleId), new XAttribute("trait", x.TraitId), new XAttribute("primary", true), new XAttribute("opening", x.Opening),
				new XAttribute("cap", x.Cap), x.ParentTraitId is { } parent ? new XAttribute("parent", parent) : null,
				new XAttribute("threshold", x.Threshold), new XAttribute("bonus", x.OverlapBonus)))));
		var progression = (Merit)Put(db, ProgressionKey, new Merit { Name = "Templar Role Progression", Type = "Role Skill Progression", MeritType = 0,
			MeritScope = 2, Definition = definition.ToString() }, ["Name", "Type", "MeritType", "MeritScope", "Definition"], messages);
		db.SaveChanges(); // The cap compiler reads the newly recorded source ownership baseline.
		ArmageddonMundaneProgression.ReconcileSkillCaps(db, skills, messages);
		InstallPoke(db, skills, messages);
		InstallLirathanPsionics(db, manifest.Roles.Single(x => x.Key == "arm.class.lirathan_templar"), resolvedRoles, skills, messages);
		InstallSouthernMagicAndItems(db, manifest, resolvedRoles, skills, messages);
		db.SaveChanges();
		ArmageddonMundaneProgression.ReconcilePsionicRoleGrants(db, messages);
		ArmageddonProgressionInstaller.ReconcilePsionicRoleGrants(db, messages);
		ArmageddonMundaneProgression.ReconcileSkillCaps(db, skills, messages);
		foreach (var (key, role) in resolvedRoles)
		{
			var desired = new List<long> { progression.Id };
			if (key == "arm.class.lirathan_templar") desired.Add(RequireOwned<Merit>(db, "arm.merit.lirathan_templar_psionics"));
			else desired.Add(RequireOwned<Merit>(db, NormalPsionicsMeritKey));
			if (key == "arm.class.southern_templar")
			{
				desired.Add(RequireOwned<Merit>(db, SouthernCastingMeritKey));
				desired.Add(RequireOwned<Merit>(db, "arm.merit.southern_templar_magic_progression"));
			}
			var links = db.ChargenRolesMerits.Where(x => x.ChargenRoleId == role.Id).ToArray();
			var merged = ArmageddonSettingLinks.Reconcile(db, key + ".links", nameof(ChargenRole), role.Id,
				new Dictionary<string, string> { ["Merits"] = JsonSerializer.Serialize(links.Select(x => x.MeritId).Order().ToArray()) },
				new Dictionary<string, string> { ["Merits"] = JsonSerializer.Serialize(desired.Order().ToArray()) }, messages);
			var ids = JsonSerializer.Deserialize<long[]>(merged["Merits"])!;
			db.ChargenRolesMerits.RemoveRange(links.Where(x => !ids.Contains(x.MeritId)));
			foreach (var id in ids.Except(links.Select(x => x.MeritId))) db.ChargenRolesMerits.Add(new ChargenRolesMerit { ChargenRoleId = role.Id, MeritId = id });
		}
		var mainRoleKeys = ArmageddonMundaneProgression.Roles.Where(x => x.Primary).Select(x => x.Key).ToArray();
		ArmageddonMundaneProgression.BindAnalyze(db, db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonMundaneProgression.Module && x.EntityType == nameof(ChargenRole) && !x.Retired && x.LogicalId != null && mainRoleKeys.Contains(x.StableKey))
			.Select(x => x.LogicalId!.Value).ToArray(), messages);
		db.SaveChanges(); transaction?.Commit();
		messages.Add($"Templar progression installed: {manifest.Roles.Length} classes and {roleRules.Count} class skill bindings. Role availability remains disabled until each enablement prog is activated; no players were enrolled.");
	}

	private static void InstallPoke(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var moveTrait = skills["arm.skill.pressure_points"];
		long Attribute(params string[] names)
		{
			foreach (var name in names)
				if (db.TraitDefinitions.SingleOrDefault(x => x.Name == name && (x.Type == 1 || x.Type == 3)) is { } trait) return trait.Id;
			throw new InvalidOperationException("Jihaen poke requires an explicit native agility attribute binding.");
		}
		var agility = Attribute("Agility", "Dexterity", "Coordination", "Speed");
		var dodge = skills["arm.scaffold.dodge"];
		var guard = skills["arm.skill.guarding"];
		var enabled = Owned<FutureProg>(db, "arm.class.jihaen_templar.enabled");
		var gateName = "ArmageddonJihaenPokeSkillGate";
		var gate = Prog(db, "arm.templar.jihaen.poke.gate", gateName,
			$"return @{enabled.FunctionName}() and Contains(@ch.Roles, ToRole({RequireOwned<ChargenRole>(db, "arm.class.jihaen_templar")})) and GetTrait(@ch, ToTrait({moveTrait.Id.ToString(CultureInfo.InvariantCulture)})) >= 1",
			[(ProgVariableTypes.Character, "ch")], messages);
		var effect = new XElement("Effect", new XAttribute("type", "pressurepoint"),
			new XAttribute("defensetrait", dodge.Id), new XAttribute("defensedifficulty", (int)Difficulty.Normal),
			new XAttribute("minimumdegree", (int)OpposedOutcomeDegree.Marginal), new XAttribute("flatamount", 0),
			new XAttribute("perdegreeamount", 0), new XAttribute("maximumamount", 0), new XAttribute("agilitytrait", agility),
			new XAttribute("agilitypenalty", -3), new XAttribute("neckseconds", 20), new XAttribute("spineseconds", 5), new XAttribute("throatseconds", 20),
			new XAttribute("stomachseconds", 45), new XAttribute("nauseaintensity", 4),
			new XAttribute("leftarmseconds", 5), new XAttribute("rightarmseconds", 5),
			new XAttribute("leftlegseconds", 5), new XAttribute("rightlegseconds", 5));
		var profile = new XElement("PhysicalImpact", new XAttribute("countertrait", dodge.Id), new XAttribute("guardtrait", guard.Id),
			new XAttribute("agilitytrait", agility), new XAttribute("minimumstamina", 5), new XAttribute("sizebonus", 0),
			new XAttribute("counterdegree", (int)OpposedOutcomeDegree.Marginal), new XAttribute("uprighttarget", false),
			new XAttribute("seatonmiss", false), new XAttribute("seatonhit", false), new XAttribute("seatoncounter", false),
			new XAttribute("shieldblock", false), new XAttribute("allowuntrained", false), new XAttribute("usableleg", false),
			new XAttribute("initiatecombat", true), new XAttribute("successdelay", 3), new XElement("Damage", 0), new XElement("Stun", 0),
			new XElement("CounterDamage", 0), new XElement("CounterStun", 0));
		var action = (CombatAction)Put(db, "arm.templar.jihaen.pressure_point.action", new CombatAction
		{
			Name = "Jihaen Pressure Point", MoveType = (int)BuiltInCombatMoveType.AuxiliaryMove, TraitDefinitionId = moveTrait.Id,
			UsabilityProgId = gate.Id, MaximumTargets = 1, Intentions = (long)CombatMoveIntentions.Attack,
			MoveDifficulty = (int)Difficulty.Normal, RecoveryDifficultySuccess = (int)Difficulty.Easy,
			RecoveryDifficultyFailure = (int)Difficulty.Hard, ExertionLevel = (int)ExertionLevel.Normal,
			StaminaCost = 5, BaseDelay = 3, Weighting = 0, RequiredPositionStateIds = "1 18",
			AdditionalInfo = new XElement("Root", profile, effect).ToString()
		}, [nameof(CombatAction.Name), nameof(CombatAction.MoveType), nameof(CombatAction.TraitDefinitionId), nameof(CombatAction.UsabilityProgId),
			nameof(CombatAction.MaximumTargets), nameof(CombatAction.Intentions), nameof(CombatAction.MoveDifficulty), nameof(CombatAction.RecoveryDifficultySuccess),
			nameof(CombatAction.RecoveryDifficultyFailure), nameof(CombatAction.ExertionLevel), nameof(CombatAction.StaminaCost), nameof(CombatAction.BaseDelay),
			nameof(CombatAction.Weighting), nameof(CombatAction.RequiredPositionStateIds), nameof(CombatAction.AdditionalInfo)], messages);
		var race = db.Races.SingleOrDefault(x => x.Name == "Organic Humanoid") ?? throw new InvalidOperationException("Jihaen poke requires the Organic Humanoid race.");
		if (db.ManualCombatCommands.Any(x => x.PrimaryVerb == "poke" && x.CombatActionId != action.Id))
			throw new InvalidOperationException("Unowned manual command 'poke' collides with the Jihaen auxiliary action.");
		var command = (ManualCombatCommand)Put(db, "arm.templar.jihaen.pressure_point.command", new ManualCombatCommand
		{
			Name = "Jihaen Pressure Point", PrimaryVerb = "poke", AdditionalVerbs = "", ActionKind = (int)ManualCombatActionKind.AuxiliaryAction,
			CombatActionId = action.Id, WeaponAttackId = null, PlayerUsable = true, NpcUsable = false, UsabilityProgId = gate.Id,
			CooldownSeconds = 15, CooldownMessage = "You must wait before attempting another pressure point strike.", DefaultAiWeightMultiplier = 0
		}, [nameof(ManualCombatCommand.Name), nameof(ManualCombatCommand.PrimaryVerb), nameof(ManualCombatCommand.AdditionalVerbs), nameof(ManualCombatCommand.ActionKind),
			nameof(ManualCombatCommand.CombatActionId), nameof(ManualCombatCommand.WeaponAttackId), nameof(ManualCombatCommand.PlayerUsable), nameof(ManualCombatCommand.NpcUsable),
			nameof(ManualCombatCommand.UsabilityProgId), nameof(ManualCombatCommand.CooldownSeconds), nameof(ManualCombatCommand.CooldownMessage), nameof(ManualCombatCommand.DefaultAiWeightMultiplier)], messages);
		var message = (CombatMessage)Put(db, "arm.templar.jihaen.pressure_point.message", new CombatMessage
		{
			Type = (int)BuiltInCombatMoveType.AuxiliaryMove, Priority = 10, Chance = 1,
			Message = "@ strike|strikes a precise pressure point on $1.", FailureMessage = "@ fail|fails to land a pressure point strike on $1."
		}, [nameof(CombatMessage.Type), nameof(CombatMessage.Priority), nameof(CombatMessage.Chance), nameof(CombatMessage.Message), nameof(CombatMessage.FailureMessage)], messages);
		var raceLink = db.RacesCombatActions.SingleOrDefault(x => x.RaceId == race.Id && x.CombatActionId == action.Id);
		var messageLink = db.CombatMessagesCombatActions.SingleOrDefault(x => x.CombatActionId == action.Id && x.CombatMessageId == message.Id);
		var links = ArmageddonSettingLinks.Reconcile(db, "arm.templar.jihaen.pressure_point.action.links", nameof(CombatAction), action.Id,
			new Dictionary<string, string> { ["RaceAttached"] = JsonSerializer.Serialize(raceLink is not null), ["MessageAttached"] = JsonSerializer.Serialize(messageLink is not null) },
			new Dictionary<string, string> { ["RaceAttached"] = "true", ["MessageAttached"] = "true" }, messages);
		if (JsonSerializer.Deserialize<bool>(links["RaceAttached"]) && raceLink is null)
			db.RacesCombatActions.Add(new RacesCombatActions { RaceId = race.Id, CombatActionId = action.Id });
		if (JsonSerializer.Deserialize<bool>(links["MessageAttached"]) && messageLink is null)
			db.CombatMessagesCombatActions.Add(new CombatMessagesCombatActions { CombatActionId = action.Id, CombatMessageId = message.Id });
		_ = command;
	}

	private static FutureProg AvailabilityProg(FuturemudDatabaseContext db, Role role, ICollection<string> messages)
	{
		var gender = role.Gender;
		var enableName = role.Name.Replace(" ", "") + "Enabled";
		var enable = Prog(db, role.Key + ".enabled", enableName, "return false", [], messages);
		var body = gender is null ? $"return @{enable.FunctionName}()" : $"return @{enable.FunctionName}() and @ch.Gender == ToGender(\"{gender}\")";
		return Prog(db, role.Key + ".availability", role.Name.Replace(" ", "") + "TemplarAvailability", body,
			[(ProgVariableTypes.Chargen, "ch")], messages);
	}

	private static FutureProg Prog(FuturemudDatabaseContext db, string key, string name, string body,
		IReadOnlyList<(ProgVariableTypes Type, string Name)> parameters, ICollection<string> messages, ProgVariableTypes? resultType = null)
	{
		var returnType = resultType ?? ProgVariableTypes.Boolean;
		var desired = new FutureProg { FunctionName = name, FunctionText = body, ReturnType = (long)returnType,
			ReturnTypeDefinition = returnType.ToStorageString(), Category = "Armageddon", Subcategory = "Templar Classes",
			FunctionComment = "Builder-editable Armageddon Templar class eligibility binding.", Public = true, AcceptsAnyParameters = false, StaticType = 0 };
		var actual = (FutureProg)Put(db, key, desired, ["FunctionName", "FunctionText", "ReturnTypeDefinition", "Category", "Subcategory", "FunctionComment", "Public", "AcceptsAnyParameters", "StaticType"], messages);
		EnsureParameters(db, key, actual, parameters, messages);
		return actual;
	}

	private static T Put<T>(FuturemudDatabaseContext db, string key, T desired, string[] fields, ICollection<string> messages) where T : class
	{
		var type = desired.GetType(); var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		ArmageddonSettingOwnership.RequireNameAvailable(db, desired, record);
		if (record is null)
		{
			var nameField = db.Model.FindEntityType(type)!.FindProperty("Name")?.Name ?? db.Model.FindEntityType(type)!.FindProperty("FunctionName")?.Name;
			if (nameField is not null)
			{
				var name = (string?)db.Entry(desired).Property(nameField).CurrentValue;
				if (db.Set<T>().AsEnumerable().Any(x => string.Equals((string?)db.Entry(x).Property(nameField).CurrentValue, name, StringComparison.OrdinalIgnoreCase) &&
					(desired is not Accent accent || x is Accent other && other.LanguageId == accent.LanguageId)))
					throw new InvalidOperationException($"Unowned Templar {type.Name} name collision: {name}.");
			}
			db.Add(desired); db.SaveChanges(); ArmageddonSettingOwnership.RecordNew(db, Module, key, desired, fields); return desired;
		}
		if (record.Module != Module || record.EntityType != type.Name || record.Retired || record.LogicalId is not { } id ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == type.Name && x.LogicalId == id)) throw new InvalidOperationException($"Invalid Templar ownership: {key}.");
		var actual = db.Find(type, id) ?? throw new InvalidOperationException($"Missing owned Templar record: {key}.");
		ArmageddonSettingOwnership.Reconcile(db, record, actual, fields.ToDictionary(x => x,
			x => JsonSerializer.Serialize(db.Entry(desired).Property(x).CurrentValue, db.Entry(desired).Property(x).Metadata.ClrType)), messages);
		return (T)actual;
	}
}
