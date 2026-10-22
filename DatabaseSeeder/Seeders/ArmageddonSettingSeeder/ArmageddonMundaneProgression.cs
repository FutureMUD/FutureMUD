#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

public sealed record ArmageddonRoleSkill(string Key, double Opening, double Cap, string? Parent, double Threshold, double Bonus);
public sealed record ArmageddonMundaneRole(string Key, string Name, bool Primary, IReadOnlyList<ArmageddonRoleSkill> Skills);

/// <summary>The frozen source tables plus approved writing overlay. No inferred numeric policy at install time.</summary>
public static class ArmageddonMundaneProgression
{
	public const string Module = "setting-mundane-progression";
	public const string ProgressionKey = "arm.merit.mundane_role_progression";
	private static readonly string[] RoleFields = ["Name", "Type", "PosterId", "ChargenBlurb", "AvailabilityProgId", "Expired", "MaximumNumberAlive", "MaximumNumberTotal", "MinimumAuthorityToApprove", "MinimumAuthorityToView"];
	private static readonly Lazy<IReadOnlyList<ArmageddonMundaneRole>> Manifest = new(ReadManifest);
	public static IReadOnlyList<ArmageddonMundaneRole> Roles => Manifest.Value;
	private static string Slug(string value) => value.ToLowerInvariant().Replace('-', '_').Replace(' ', '_');
	private static double Number(string value) => double.Parse(value, CultureInfo.InvariantCulture);
	private static Dictionary<string, string>[] Table(string file)
	{
		using var stream = typeof(ArmageddonMundaneProgression).Assembly.GetManifestResourceStream("DatabaseSeeder.Armageddon." + file)
			?? throw new InvalidOperationException($"Missing frozen mundane table {file}.");
		using var reader = new StreamReader(stream);
		var headings = reader.ReadLine()!.Split('\t');
		var result = new List<Dictionary<string, string>>();
		while (reader.ReadLine() is { } line)
		{
			if (string.IsNullOrWhiteSpace(line)) continue;
			var fields = line.Split('\t');
			if (fields.Length != headings.Length) throw new InvalidOperationException($"Invalid table row in {file}.");
			result.Add(headings.Select((x, index) => (x, fields[index])).ToDictionary(x => x.x, x => x.Item2));
		}
		return result.ToArray();
	}

	private static IReadOnlyList<ArmageddonMundaneRole> ReadManifest()
	{
		var main = Table("guild_memberships.tsv");
		var secondary = Table("subguild_progression_proposals.tsv");
		var writing = Table("optional_writing_grants_20261016.tsv");
		if (main.Length != 226 || secondary.Length != 93 || writing.Length != 7)
			throw new InvalidOperationException("Frozen Armageddon membership count drift.");
		var roles = new List<ArmageddonMundaneRole>();
		foreach (var group in main.GroupBy(x => x["guild"]))
		{
			roles.Add(new("arm.class." + Slug(group.Key), group.Key, true, group.Where(x => x["canonical_key"] != "arm.skill.analyze")
				.Select(x => new ArmageddonRoleSkill(ArmageddonSkillPackage.CanonicalKey(x["canonical_key"]), Number(x["opening"]),
					Number(x["futuremud_effective_cap"]), string.IsNullOrEmpty(x["parent_canonical_key"]) ? null : ArmageddonSkillPackage.CanonicalKey(x["parent_canonical_key"]),
					string.IsNullOrEmpty(x["inferred_parent_threshold"]) ? 0 : Number(x["inferred_parent_threshold"]), 0)).ToList()));
		}
		foreach (var group in secondary.GroupBy(x => x["subguild"]))
		{
			roles.Add(new("arm.subclass." + Slug(group.Key), group.Key, false, group.Where(x => x["association_strength"] != "merit")
				.Select(x => new ArmageddonRoleSkill(ArmageddonSkillPackage.CanonicalKey(x["canonical_key"]), Number(x["standalone_opening"]), Number(x["authored_cap"]),
					null, 0, string.IsNullOrEmpty(x["main_overlap_start_bonus"]) ? 0 : Number(x["main_overlap_start_bonus"]))).ToList()));
		}
		foreach (var row in writing)
		{
			var role = roles.Single(x => x.Name == row["role_name"] && x.Primary == (row["role_type"] == "Class"));
			((List<ArmageddonRoleSkill>)role.Skills).Add(new("arm.skill." + row["trait"].ToLowerInvariant(), Number(row["opening"]), Number(row["cap"]), null, 0,
				string.IsNullOrEmpty(row["main_overlap_start_bonus"]) ? 0 : Number(row["main_overlap_start_bonus"])));
		}
		if (roles.Count(x => x.Primary) != 7 || roles.Count(x => !x.Primary) != 24 ||
			roles.Any(x => x.Skills.GroupBy(y => y.Key).Any(y => y.Count() != 1)) ||
			roles.SelectMany(x => x.Skills).Any(x => !ArmageddonSkillPackage.Skills.Any(y => y.Key == x.Key && !y.Scaffold)))
			throw new InvalidOperationException("Mundane role identities or canonical memberships drifted.");
		return roles.Select(x => x with { Skills = Array.AsReadOnly(x.Skills.ToArray()) }).ToArray();
	}

	private static Dictionary<string, string> Fields(FuturemudDatabaseContext db, object entity, IEnumerable<string> fields) =>
		fields.ToDictionary(x => x, x => JsonSerializer.Serialize(db.Entry(entity).Property(x).CurrentValue, db.Entry(entity).Property(x).Metadata.ClrType));

	private static T Apply<T>(FuturemudDatabaseContext db, string key, T desired, ICollection<string> messages, params string[] fields) where T : class
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null)
		{
			db.Add(desired); db.SaveChanges();
			ArmageddonSettingOwnership.RecordNew(db, Module, key, desired, fields);
			return desired;
		}
		if (record.Module != Module || record.EntityType != typeof(T).Name || record.Retired || record.LogicalId is not { } id ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == id))
			throw new InvalidOperationException($"Invalid or competing mundane ownership: {key}.");
		var actual = db.Find<T>(id) ?? throw new InvalidOperationException($"Missing owned mundane entity: {key}.");
		if (actual is ChargenRole role)
		{
			var current = Fields(db, actual, fields);
			var next = Fields(db, desired, fields);
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			if (baseline.ContainsKey("RoleMerits"))
			{
				current["RoleMerits"] = JsonSerializer.Serialize(db.ChargenRolesMerits.Where(x => x.ChargenRoleId == role.Id).Select(x => x.MeritId).Order().ToArray());
				next["RoleMerits"] = baseline["RoleMerits"];
			}
			var merged = SeederManagedRecordReconciler.Reconcile(record, current, next, false, messages);
			foreach (var field in fields) db.Entry(actual).Property(field).CurrentValue = JsonSerializer.Deserialize(merged[field], db.Entry(actual).Property(field).Metadata.ClrType);
			record.AppliedAt = DateTime.UtcNow;
		}
		else ArmageddonSettingOwnership.Reconcile(db, record, actual, Fields(db, desired, fields), messages);
		return actual;
	}

	public static void Install(FuturemudDatabaseContext db, ICollection<string> messages)
	{
		var skills = ArmageddonSkillPackage.Resolve(db);
		var yes = db.FutureProgs.Single(x => x.FunctionName == "AlwaysTrue").Id;
		var no = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var poster = db.Accounts.OrderBy(x => x.Id).Select(x => x.Id).First();
		// Complete collision and reference preflight before the first role is created.
		foreach (var role in Roles)
		{
			if (!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == role.Key) && db.ChargenRoles.Any(x => x.Name == role.Name))
				throw new InvalidOperationException($"Unowned mundane role name collision: {role.Name}.");
			foreach (var skill in role.Skills)
				if (!skills.ContainsKey(skill.Key) || skill.Parent is { } parent && !role.Skills.Any(x => x.Key == parent))
					throw new InvalidOperationException($"Unresolved mundane role edge: {role.Name}/{skill.Key}.");
		}
		if (!db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == ProgressionKey) && db.Merits.Any(x => x.Name == "Armageddon Mundane Role Progression"))
			throw new InvalidOperationException("Unowned mundane progression merit name collision.");
		using var transaction = db.Database.IsRelational() && db.Database.CurrentTransaction is null ? db.Database.BeginTransaction() : null;
		var merits = ArmageddonMundaneMerits.Install(db, messages);
		var actualRoles = Roles.ToDictionary(x => x.Key, x => Apply(db, x.Key, new ChargenRole
		{
			Name = x.Name, Type = x.Primary ? 0 : 1, PosterId = poster, AvailabilityProgId = yes,
			ChargenBlurb = $"{x.Name}: {(x.Primary ? "main guild with a prerequisite skill tree" : "subclass with approved supplemental proficiencies")}. " +
				"Skill grants and caps compose with your other selected role. Psychic traits do not grant active psychic powers; recipe and script access remain separate.",
			Expired = false, MaximumNumberAlive = 0, MaximumNumberTotal = 0, MinimumAuthorityToApprove = 0, MinimumAuthorityToView = 0
		}, messages, RoleFields));
		var rules = Roles.SelectMany(role => role.Skills.Select(skill => new RoleSkillRule(actualRoles[role.Key].Id, skills[skill.Key].Id,
			role.Primary, skill.Opening, skill.Cap, skill.Parent is { } parent ? skills[parent].Id : null, skill.Threshold, skill.Bonus))).ToArray();
		_ = new RoleSkillProgression(rules); // Validate all trees, including cycles, before the merit is persisted.
		var definition = new XElement("Definition", new XElement("ChargenAvailableProg", no), new XElement("ApplicabilityProg", yes),
			new XElement("DescriptionText", "@ follow|follows role-specific skill progression"), new XElement("ChargenBlurb", "Role skill progression"),
			new XElement("Rules", rules.Select(x => new XElement("Rule", new XAttribute("role", x.RoleId), new XAttribute("trait", x.TraitId),
				new XAttribute("primary", x.Primary), new XAttribute("opening", x.Opening), new XAttribute("cap", x.Cap),
				x.ParentTraitId is { } parent ? new XAttribute("parent", parent) : null, new XAttribute("threshold", x.Threshold), new XAttribute("bonus", x.OverlapBonus)))));
		var progression = Apply(db, ProgressionKey, new Merit { Name = "Armageddon Mundane Role Progression", Type = "Role Skill Progression", MeritType = 0, MeritScope = 2, Definition = definition.ToString() },
			messages, "Name", "Type", "MeritType", "MeritScope", "Definition");
		foreach (var role in Roles)
		{
			var desired = new List<long> { progression.Id };
			if (role.Name == "Bard" && !role.Primary) desired.Add(merits[ArmageddonMundaneMerits.BardKey]);
			if (role.Name == "Mercenary" && !role.Primary) desired.Add(merits[ArmageddonMundaneMerits.MercenaryKey]);
			ReconcileRoleMerits(db, role.Key, actualRoles[role.Key], desired, messages);
		}
		ReconcileSkillCaps(db, skills, messages);
		BindAnalyze(db, actualRoles.Values.Where(x => x.Type == 0).Select(x => x.Id).ToArray(), messages);
		db.SaveChanges(); transaction?.Commit();
		messages.Add("Mundane progression installed: seven main guilds, 24 subclasses, source trees, approved writing grants and exact Bard/Mercenary merits. No existing character was enrolled.");
	}

	internal static void ReconcileSkillCaps(FuturemudDatabaseContext db, IReadOnlyDictionary<string, TraitDefinition> skills, ICollection<string> messages)
	{
		var roleIds = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module &&
			x.EntityType == nameof(ChargenRole) && !x.Retired && x.LogicalId != null).ToDictionary(x => x.StableKey, x => x.LogicalId!.Value);
		var caps = Roles.Where(x => roleIds.ContainsKey(x.Key)).SelectMany(role => role.Skills.Select(skill =>
			(Role: roleIds[role.Key], Trait: skills[skill.Key].Id, role.Primary, skill.Cap))).ToList();
		var templar = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonTemplarInstaller.Module && x.StableKey == ArmageddonTemplarInstaller.ProgressionKey &&
			x.EntityType == nameof(Merit) && !x.Retired);
		if (templar is not null)
		{
			// The ownership baseline is the authored source graph, even when a builder edits the merit.
			var fields = JsonSerializer.Deserialize<Dictionary<string, string>>(templar.SeedBaseline!)!;
			var definition = XElement.Parse(JsonSerializer.Deserialize<string>(fields["Definition"])!);
			caps.AddRange(definition.Element("Rules")!.Elements("Rule").Select(x =>
				((long)x.Attribute("role")!, (long)x.Attribute("trait")!, (bool)x.Attribute("primary")!, (double)x.Attribute("cap")!)));
		}
		var scaffolds = ArmageddonSkillPackage.Skills.Where(x => x.Scaffold).Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var languages = db.Languages.Select(x => x.LinkedTraitId).AsEnumerable()
			.Concat(ArmageddonSkillPackage.Skills.Where(x => x.Language).Select(x => skills[x.Key].Id)).ToHashSet();
		var canonical = ArmageddonSkillPackage.Skills.Where(x => !x.Scaffold).Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		foreach (var (key, trait) in skills.Where(x => ArmageddonSkillPackage.CanonicalKey(x.Key) == x.Key && !scaffolds.Contains(x.Key) &&
			(canonical.Contains(x.Key) || caps.Any(c => c.Trait == x.Value.Id))))
		{
			var members = caps.Where(x => x.Trait == trait.Id).ToArray();
			string Sum(bool primary) => string.Join(" + ", members.Where(x => x.Primary == primary).Select(x =>
				$"{x.Cap.ToString(CultureInfo.InvariantCulture)} * {{r{x.Role} role={x.Role}}}"));
			var primaryMembers = members.Where(x => x.Primary).ToArray();
			var main = Sum(true); var sub = Sum(false);
			var expression = languages.Contains(trait.Id) ? "200" : primaryMembers.Length == 0 ? (sub.Length == 0 ? "0" : sub) :
				$"if({{main role={string.Join(',', primaryMembers.Select(x => x.Role))}}}, {main}, {(sub.Length == 0 ? "0" : sub)})";
			var record = db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key + ".cap");
			var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
			var next = new Dictionary<string, string> { ["Expression"] = JsonSerializer.Serialize(expression) };
			if (baseline.TryGetValue("Name", out var name)) next["Name"] = name;
			ArmageddonSettingOwnership.Reconcile(db, record, trait.Expression, next, messages);
		}
	}

	private static void ReconcileRoleMerits(FuturemudDatabaseContext db, string key, ChargenRole role, IEnumerable<long> desired, ICollection<string> messages)
	{
		var record = db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		var current = Fields(db, role, RoleFields);
		var links = db.ChargenRolesMerits.Where(x => x.ChargenRoleId == role.Id).ToArray();
		current["RoleMerits"] = JsonSerializer.Serialize(links.Select(x => x.MeritId).Order().ToArray());
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		var next = new Dictionary<string, string>(baseline) { ["RoleMerits"] = JsonSerializer.Serialize(desired.Order().ToArray()) };
		var first = !baseline.ContainsKey("RoleMerits") && links.Length == 0;
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, next, first, messages);
		var ids = JsonSerializer.Deserialize<long[]>(merged["RoleMerits"])!;
		db.ChargenRolesMerits.RemoveRange(links.Where(x => !ids.Contains(x.MeritId)));
		foreach (var id in ids.Except(links.Select(x => x.MeritId))) db.ChargenRolesMerits.Add(new ChargenRolesMerit { ChargenRoleId = role.Id, MeritId = id });
	}

	internal static void BindAnalyze(FuturemudDatabaseContext db, long[] roleIds, ICollection<string> messages)
	{
		roleIds = roleIds.Concat(db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonTemplarInstaller.Module && x.EntityType == nameof(ChargenRole) && !x.Retired && x.LogicalId != null)
			.Select(x => x.LogicalId!.Value)).Distinct().ToArray();
		const string key = "arm.capability.analyze";
		var check = db.Checks.Include(x => x.TraitExpression).Single(x => x.Type == (int)CheckType.AnalyzeEligibilityCheck);
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is null)
		{
			if (check.TraitExpression.Expression != "0" || db.Checks.Count(x => x.TraitExpressionId == check.TraitExpressionId) != 1 || db.TraitDefinitions.Any(x => x.ExpressionId == check.TraitExpressionId))
				throw new InvalidOperationException("Analyze check is customised or shares its expression. Resolve its capability binding explicitly.");
			record = ArmageddonSettingOwnership.RecordNew(db, Module, key, check.TraitExpression, "Name", "Expression");
		}
		if (record.LogicalId != check.TraitExpressionId) throw new InvalidOperationException("Analyze capability check binding changed; preserve and resolve it explicitly.");
		ArmageddonSettingOwnership.Reconcile(db, record, check.TraitExpression, new Dictionary<string, string>
		{
			["Name"] = JsonSerializer.Serialize("Armageddon Analyze Eligibility"),
			["Expression"] = JsonSerializer.Serialize($"100 * {{analyst role={string.Join(',', roleIds.Order())}}}")
		}, messages);
	}
}
