#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonProgressionInstaller
{
	private static readonly IReadOnlyDictionary<Type, string[]> OwnedFields = new Dictionary<Type, string[]>
	{
		[typeof(FutureProg)] = ["FunctionName", "FunctionText", "ReturnType", "AcceptsAnyParameters", "Category", "Subcategory", "FunctionComment", "Public", "StaticType"],
		[typeof(TraitExpression)] = ["Name", "Expression"],
		[typeof(Improver)] = ["Name", "Type", "Definition"],
		[typeof(TraitDefinition)] = ["Name", "Type", "OwnerScope", "DecoratorId", "TraitGroup", "DerivedType", "ExpressionId", "ImproverId", "Hidden",
			"ChargenBlurb", "BranchMultiplier", "Alias", "AvailabilityProgId", "TeachableProgId", "LearnableProgId", "TeachDifficulty", "LearnDifficulty", "ValueExpression", "ShowInScoreCommand"],
		[typeof(MagicResource)] = ["Name", "ShortName", "Type", "MagicResourceType", "BottomColour", "MidColour", "TopColour", "Definition"],
		[typeof(MagicSchool)] = ["Name", "SchoolVerb", "SchoolAdjective", "PowerListColour"],
		[typeof(MagicGenerator)] = ["Name", "Type", "Definition"],
		[typeof(MagicCapability)] = ["Name", "CapabilityModel", "PowerLevel", "Definition", "MagicSchoolId"],
		[typeof(Merit)] = ["Name", "Type", "MeritType", "MeritScope", "Definition"],
		[typeof(ChargenRole)] = ["Name", "Type", "PosterId", "ChargenBlurb", "AvailabilityProgId", "Expired", "MaximumNumberAlive", "MaximumNumberTotal", "MinimumAuthorityToApprove", "MinimumAuthorityToView"]
	};

	private static Dictionary<string, string> Fields(FuturemudDatabaseContext db, object row, Type type) => db.Entry(row).Properties
		.Where(x => OwnedFields[type].Contains(x.Metadata.Name))
		.ToDictionary(x => x.Metadata.Name, x => JsonSerializer.Serialize(x.CurrentValue, x.Metadata.ClrType), StringComparer.Ordinal);

	private static T Apply<T>(FuturemudDatabaseContext db, Spec spec, T desired,
		Dictionary<string, SeederManagedRecord> records, List<string> messages) where T : class
	{
		var created = !records.TryGetValue(spec.Key, out var record);
		// Capture explicit stock values before SaveChanges substitutes provider defaults (e.g. BranchMultiplier=1).
		var next = Fields(db, desired, spec.Type);
		T row;
		if (created)
		{
			row = desired;
			db.Add(row);
			db.SaveChanges();
			var id = (long)db.Entry(row).Property("Id").CurrentValue!;
			if (db.SeederManagedRecords.Any(x => x.EntityType == spec.Type.Name && x.LogicalId == id))
				throw new InvalidOperationException($"{spec.Key}: competing logical identity; no takeover.");
			record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = Module,
				StableKey = spec.Key, EntityType = spec.Type.Name, LogicalId = id };
			db.SeederManagedRecords.Add(record);
			records.Add(spec.Key, record);
		}
		else row = (T)db.Find(spec.Type, record!.LogicalId!.Value)!;
		var actual = Fields(db, row, spec.Type);
		// Retain the legacy module and identity. Metadata is versioned stock, never inferred player state.
		next["$stockProgression"] = JsonSerializer.Serialize(Version);
		var baseline = record!.SeedBaseline is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
		foreach (var metadata in baseline?.Where(x => x.Key.StartsWith("$", StringComparison.Ordinal)) ?? []) actual[metadata.Key] = metadata.Value;
		if (row is ChargenRole role && baseline?.ContainsKey("RoleMerits") == true)
		{
			actual["RoleMerits"] = JsonSerializer.Serialize(db.ChargenRolesMerits.Where(x => x.ChargenRoleId == role.Id).Select(x => x.MeritId).Order().ToArray());
			actual["RoleTraits"] = JsonSerializer.Serialize(db.ChargenRolesTraits.Where(x => x.ChargenRoleId == role.Id).OrderBy(x => x.TraitId)
				.AsEnumerable().Select(x => new RoleTrait(x.TraitId, x.Amount, x.GiveIfDoesntHave)).ToArray());
			next["RoleMerits"] = baseline["RoleMerits"];
			next["RoleTraits"] = baseline["RoleTraits"];
		}
		var merged = SeederManagedRecordReconciler.Reconcile(record, actual, next, created, messages);
		foreach (var property in db.Entry(row).Properties.Where(x => OwnedFields[spec.Type].Contains(x.Metadata.Name)))
			if (merged.TryGetValue(property.Metadata.Name, out var value)) property.CurrentValue = JsonSerializer.Deserialize(value, property.Metadata.ClrType);
		if (created || actual.Count != merged.Count || actual.Any(x => !merged.TryGetValue(x.Key, out var value) || value != x.Value)) record.AppliedAt = DateTime.UtcNow;
		record.ManifestVersion = Version;
		db.SaveChanges();
		return row;
	}

	private static void RoleLinks(FuturemudDatabaseContext db, ChargenRole role, SeederManagedRecord record, long meritId,
		IReadOnlyDictionary<long, double> startingTraits, List<string> messages)
	{
		var merits = db.ChargenRolesMerits.Where(x => x.ChargenRoleId == role.Id).ToArray();
		var traits = db.ChargenRolesTraits.Where(x => x.ChargenRoleId == role.Id).ToArray();
		var current = Fields(db, role, typeof(ChargenRole));
		current["$stockProgression"] = JsonSerializer.Serialize(Version);
		current["RoleMerits"] = JsonSerializer.Serialize(merits.Select(x => x.MeritId).Order().ToArray());
		current["RoleTraits"] = JsonSerializer.Serialize(traits.OrderBy(x => x.TraitId).Select(x => new RoleTrait(x.TraitId, x.Amount, x.GiveIfDoesntHave)).ToArray());
		var baseline = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline!)!;
		var desired = new Dictionary<string, string>(baseline)
		{
			["RoleMerits"] = JsonSerializer.Serialize(new[] { meritId }),
			["RoleTraits"] = JsonSerializer.Serialize(startingTraits.OrderBy(x => x.Key).Select(x => new RoleTrait(x.Key, x.Value, true)).ToArray())
		};
		var newLinks = !baseline.ContainsKey("RoleMerits") && !baseline.ContainsKey("RoleTraits") && merits.Length == 0 && traits.Length == 0;
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, desired, newLinks, messages);
		var nextMerits = JsonSerializer.Deserialize<long[]>(merged["RoleMerits"])!;
		var nextTraits = JsonSerializer.Deserialize<RoleTrait[]>(merged["RoleTraits"])!;
		db.ChargenRolesMerits.RemoveRange(merits.Where(x => !nextMerits.Contains(x.MeritId)));
		foreach (var id in nextMerits.Except(merits.Select(x => x.MeritId))) db.ChargenRolesMerits.Add(new ChargenRolesMerit { ChargenRoleId = role.Id, MeritId = id });
		db.ChargenRolesTraits.RemoveRange(traits.Where(x => nextTraits.All(y => y.Id != x.TraitId)));
		foreach (var value in nextTraits)
		{
			var join = traits.SingleOrDefault(x => x.TraitId == value.Id);
			if (join is null) { join = new ChargenRolesTrait { ChargenRoleId = role.Id, TraitId = value.Id }; db.ChargenRolesTraits.Add(join); }
			join.Amount = value.Amount;
			join.GiveIfDoesntHave = value.Give;
		}
		if (newLinks || current["RoleMerits"] != merged["RoleMerits"] || current["RoleTraits"] != merged["RoleTraits"]) record.AppliedAt = DateTime.UtcNow;
		db.SaveChanges();
	}

	private sealed record RoleTrait(long Id, double Amount, bool Give);

	private static bool Collision(FuturemudDatabaseContext db, Spec spec)
	{
		var name = spec.Name.ToLowerInvariant();
		var resourceShortName = spec.Type == typeof(MagicResource)
			? spec.Key == SourceResourceKey ? "armsource" : "arm" + spec.Key["arm.resource.".Length..] : "";
		var schoolVerb = spec.Type == typeof(MagicSchool) ? "arm" + spec.Key["arm.school.".Length..] : "";
		return spec.Type == typeof(FutureProg) ? db.FutureProgs.Any(x => x.FunctionName.ToLower() == name) :
			spec.Type == typeof(TraitExpression) ? db.TraitExpressions.Any(x => x.Name.ToLower() == name) :
			spec.Type == typeof(TraitDefinition) ? db.TraitDefinitions.Any(x => x.Name.ToLower() == name) :
			spec.Type == typeof(Improver) ? db.Improvers.Any(x => x.Name.ToLower() == name) :
			spec.Type == typeof(MagicResource) ? db.MagicResources.Any(x => x.Name.ToLower() == name || x.ShortName.ToLower() == resourceShortName) :
			spec.Type == typeof(MagicSchool) ? db.MagicSchools.Any(x => x.Name.ToLower() == name || x.SchoolVerb.ToLower() == schoolVerb) :
			spec.Type == typeof(MagicGenerator) ? db.MagicGenerators.Any(x => x.Name.ToLower() == name) :
			spec.Type == typeof(MagicCapability) ? db.MagicCapabilities.Any(x => x.Name.ToLower() == name) :
			spec.Type == typeof(Merit) ? db.Merits.Any(x => x.Name.ToLower() == name) : db.ChargenRoles.Any(x => x.Name.ToLower() == name);
	}
}
