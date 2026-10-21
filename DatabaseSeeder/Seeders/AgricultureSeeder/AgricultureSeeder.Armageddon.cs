#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using MudSharp.Body.Traits;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public sealed partial class AgricultureSeeder
{
	internal const string FarmingModule = "setting-optional-agriculture";
	private const string FarmingKey = "arm.optional.agriculture.farming";

	internal static bool HasFarmingFoundation(FuturemudDatabaseContext db) =>
		ResolveFarmingTrait(db) is not null || ArmageddonSkillPackage.IsInstalled(db);

	private static TraitDefinition? ResolveFarmingTrait(FuturemudDatabaseContext db)
	{
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == FarmingKey);
		if (record is null) return db.TraitDefinitions.FirstOrDefault(x => x.Name == FarmingTraitName);
		bool ValidClaim(SeederManagedRecord claim, string type) => claim.Module == FarmingModule && claim.EntityType == type &&
			!claim.Retired && claim.LogicalId is not null && claim.ManifestVersion == ArmageddonSkillPackage.Version && claim.SeedBaseline is not null &&
			claim.AppliedFingerprint == Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(claim.SeedBaseline))).ToLowerInvariant() &&
			!db.SeederManagedRecords.Any(x => x.Id != claim.Id && x.EntityType == type && x.LogicalId == claim.LogicalId);
		var cap = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == FarmingKey + ".cap");
		if (!ValidClaim(record, nameof(TraitDefinition)) || cap is null || !ValidClaim(cap, nameof(TraitExpression)) ||
			!db.TraitExpressions.Any(x => x.Id == cap.LogicalId))
			throw new InvalidOperationException("Invalid or competing optional Farming skill ownership.");
		var trait = db.TraitDefinitions.SingleOrDefault(x => x.Id == record.LogicalId);
		if (trait is null || trait.Type != 0 || trait.OwnerScope != (int)TraitOwnerScope.Character || trait.DerivedType != 0 ||
			!db.TraitExpressions.Any(x => x.Id == trait.ExpressionId) || !db.TraitDecorators.Any(x => x.Id == trait.DecoratorId) ||
			!db.Improvers.Any(x => x.Id == trait.ImproverId))
			throw new InvalidOperationException("Missing or invalid optional Farming character skill.");
		return trait;
	}

	internal static TraitDefinition EnsureFarmingTrait(FuturemudDatabaseContext db)
	{
		var existing = ResolveFarmingTrait(db);
		if (existing is not null) return existing;
		if (!ArmageddonSkillPackage.IsInstalled(db)) throw new InvalidOperationException("Agriculture requires an existing Farming skill.");
		// Authored optional-package support, separate from the frozen source skill inventory.
		// Installation does not make the skill learnable or add any character/role grant.
		var donor = ArmageddonSkillPackage.Resolve(db)["arm.scaffold.auxiliary"];
		var unavailable = db.FutureProgs.Single(x => x.FunctionName == "AlwaysFalse").Id;
		var messages = new List<string>();
		var cap = ArmageddonSettingOwnership.Apply(db, FarmingModule, FarmingKey + ".cap",
			new TraitExpression { Name = FarmingKey + ".cap", Expression = "100" }, messages);
		return ArmageddonSettingOwnership.Apply(db, FarmingModule, FarmingKey, new TraitDefinition
		{
			Name = FarmingTraitName, Alias = FarmingTraitName, Type = 0, OwnerScope = (int)TraitOwnerScope.Character, DerivedType = 0,
			TraitGroup = "Professional", Hidden = true, ExpressionId = cap.Id, DecoratorId = donor.DecoratorId, ImproverId = donor.ImproverId,
			AvailabilityProgId = unavailable, LearnableProgId = unavailable, TeachableProgId = unavailable,
			TeachDifficulty = 7, LearnDifficulty = 7, BranchMultiplier = 1.0, ChargenBlurb = ""
		}, messages);
	}
}
