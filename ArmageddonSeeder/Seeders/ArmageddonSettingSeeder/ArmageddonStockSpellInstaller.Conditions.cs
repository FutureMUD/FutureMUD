#nullable enable

using System.Linq;
using MudSharp.Database;
using MudSharp.Health;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static long CharacterResistance(FuturemudDatabaseContext db, string school, string classicStartingSpell)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix package settings before S4 resistance bindings.");
		var key = settings.Mode == ArmageddonProgressionMode.Simple ? $"arm.skill.{school}_casting" : classicStartingSpell + ".skill";
		var record = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (record is not { Retired: false, LogicalId: > 0, RevisionNumber: null, SeedBaseline: not null } ||
			record.ManifestVersion != ArmageddonProgressionInstaller.Version ||
			record.Module is not (ArmageddonProgressionInstaller.Module or ArmageddonTraditionInstaller.Module) ||
			record.EntityType != nameof(TraitDefinition) ||
			!db.TraitDefinitions.Any(x => x.Id == record.LogicalId && x.Type == 0 && x.OwnerScope == 1) ||
			db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
			throw new StockDependencyException(key + ": restore the exact character-owned resistance skill; no body attribute substitution.");
		return record.LogicalId.Value;
	}

	private static ArmageddonCanonicalSpellContribution ResolveCondition(FuturemudDatabaseContext db, string key)
	{
		var disease = ArmageddonCatalogueLinks.Bind(db, "arm.effect.disease", nameof(InfectionType), () =>
			throw new StockDependencyException("Install the explicit S3 native disease alias before S4 conditions."));
		return ArmageddonConditionSpellContent.Create(new(disease, CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey)))
			.Single(x => x.Content.Key == key);
	}
}
