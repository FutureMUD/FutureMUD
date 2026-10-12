#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;
using MudSharp.Health;

namespace DatabaseSeeder.Seeders;

internal sealed record ArmageddonCatalogueAlias(string NativeType, long NativeId);
internal sealed record ArmageddonCatalogueForageRoute(long ProfileId, int ProfileRevision, long ForagableId);

/// <summary>References and additive joins do not claim the shared native entity they point at.</summary>
internal static class ArmageddonCatalogueLinks
{
	internal const string Module = "stock-catalogue-links";
	internal const string AliasType = nameof(ArmageddonCatalogueAlias);
	internal const string RouteType = nameof(ArmageddonCatalogueForageRoute);

	internal static long Bind(FuturemudDatabaseContext db, string key, string nativeType, Func<long> resolve)
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is not null)
		{
			if (!Valid(db, record) || record.EntityType != AliasType) throw new InvalidOperationException($"{key}: invalid native alias.");
			var binding = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(record.SeedBaseline!)!;
			if (binding.NativeType != nativeType) throw new InvalidOperationException($"{key}: native binding type cannot change.");
			return binding.NativeId;
		}
		var alias = new ArmageddonCatalogueAlias(nativeType, resolve());
		if (!AliasExists(db, alias)) throw new InvalidOperationException($"{key}: native binding is missing or incompatible.");
		db.SeederManagedRecords.Add(new() { Seeder = ArmageddonMagicInstaller.Package, Module = Module, StableKey = key,
			EntityType = AliasType, SeedBaseline = JsonSerializer.Serialize(alias), ManifestVersion = "stock-catalogue-links-1", AppliedAt = DateTime.UtcNow });
		db.SaveChanges();
		return alias.NativeId;
	}

	private static bool AliasExists(FuturemudDatabaseContext db, ArmageddonCatalogueAlias alias) => alias.NativeId > 0 && (alias.NativeType switch
	{
		nameof(Material) => db.Materials.Any(x => x.Id == alias.NativeId && x.Density > 0),
		nameof(Liquid) => db.Liquids.Any(x => x.Id == alias.NativeId && x.Density > 0),
		nameof(Terrain) => db.Terrains.Any(x => x.Id == alias.NativeId),
		nameof(WeaponType) => db.WeaponTypes.Any(x => x.Id == alias.NativeId),
		nameof(BodyProto) => db.BodyProtos.Any(x => x.Id == alias.NativeId),
		nameof(PopulationBloodModel) => db.PopulationBloodModels.Any(x => x.Id == alias.NativeId),
		nameof(Accent) => db.Accents.Any(x => x.Id == alias.NativeId),
		nameof(BodypartShape) => db.BodypartShapes.Any(x => x.Id == alias.NativeId),
		nameof(InfectionType) => alias.NativeId == (long)InfectionType.SownSickness,
		_ => false
	});

	internal static bool Valid(FuturemudDatabaseContext db, SeederManagedRecord record)
	{
		if (record.Module != Module || record.Retired || record.LogicalId is not null || record.RevisionNumber is not null || record.SeedBaseline is null) return false;
		try
		{
			if (record.EntityType == AliasType) return JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(record.SeedBaseline) is { } alias && AliasExists(db, alias);
			if (record.EntityType != RouteType) return false;
			var fields = JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
			if (fields is null || !fields.TryGetValue("$route", out var routeJson) || routeJson is null) return false;
			var route = JsonSerializer.Deserialize<ArmageddonCatalogueForageRoute>(routeJson);
			return route is not null && route.ForagableId > 0 && db.Foragables.Any(x => x.Id == route.ForagableId) &&
				db.ForagableProfiles.Any(x => x.Id == route.ProfileId && x.RevisionNumber == route.ProfileRevision);
		}
		catch (Exception error) when (error is JsonException or KeyNotFoundException or InvalidOperationException) { return false; }
	}

	internal static void AttachForage(FuturemudDatabaseContext db, long forageId, List<string> messages)
	{
		// Eligibility is proved through native yield/profile/terrain foreign keys. No terrain name or definition scan.
		var profiles = (from profile in db.ForagableProfiles
			join editable in db.EditableItems on profile.EditableItemId equals editable.Id
			where editable.RevisionStatus == (int)RevisionStatus.Current &&
				db.ForagableProfilesMaximumYields.Any(x => x.ForagableProfileId == profile.Id && x.ForagableProfileRevisionNumber == profile.RevisionNumber && x.ForageType == "clay" && x.Yield > 0) &&
				db.Terrains.Any(x => x.ForagableProfileId == profile.Id)
			select profile).ToArray();
		if (profiles.Length == 0) throw new InvalidOperationException("Install a native terrain-linked clay forage profile before Armageddon component supplies.");
		foreach (var profile in profiles)
		{
			var route = new ArmageddonCatalogueForageRoute(profile.Id, profile.RevisionNumber, forageId);
			var key = $"arm.gather.component_blank.profile.{profile.Id}.r{profile.RevisionNumber}";
			var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
			var created = record is null;
			if (record is not null && (!Valid(db, record) || record.EntityType != RouteType)) throw new InvalidOperationException($"{key}: invalid owned forage link.");
			var existing = db.ForagableProfilesForagables.SingleOrDefault(x => x.ForagableProfileId == profile.Id && x.ForagableProfileRevisionNumber == profile.RevisionNumber && x.ForagableId == forageId);
			if (created && existing is not null) throw new InvalidOperationException($"{key}: unowned forage join collision; no adoption.");
			if (created)
			{
				record = new() { Seeder = ArmageddonMagicInstaller.Package, Module = Module, StableKey = key, EntityType = RouteType };
				db.SeederManagedRecords.Add(record);
			}
			var next = new Dictionary<string, string> { ["$route"] = JsonSerializer.Serialize(route), ["Attached"] = "true" };
			var current = new Dictionary<string, string> { ["$route"] = next["$route"], ["Attached"] = existing is null ? "false" : "true" };
			var merged = SeederManagedRecordReconciler.Reconcile(record!, current, next, created, messages);
			if (merged["Attached"] == "true" && existing is null) db.ForagableProfilesForagables.Add(new()
			{ ForagableProfileId = profile.Id, ForagableProfileRevisionNumber = profile.RevisionNumber, ForagableId = forageId });
			if (created || current["Attached"] != merged["Attached"]) record!.AppliedAt = DateTime.UtcNow;
			record!.ManifestVersion = "stock-catalogue-links-1";
		}
		db.SaveChanges();
	}
}
