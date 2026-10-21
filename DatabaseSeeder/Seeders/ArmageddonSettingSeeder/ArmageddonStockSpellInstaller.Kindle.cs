extern alias EngineCompiler;
#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Magic;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static ArmageddonCanonicalSpellContribution ResolveKindle(FuturemudDatabaseContext db)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix installation mappings before Kindle.");
		var strength = ArmageddonCatalogueLinks.Bind(db, StrengthBindingKey, nameof(TraitDefinition), () =>
		{
			var candidates = db.TraitDefinitions.Where(x => x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 &&
				x.TraitGroup == "Physical" && x.Alias == "str").Select(x => x.Id).ToArray();
			if (candidates.Length != 1) throw new StockDependencyException("Kindle requires one ordinary native Strength binding.");
			return candidates[0];
		});
		var agility = ResolveOrdinaryAgility(db);
		var wisdom = ArmageddonCatalogueLinks.Bind(db, "arm.binding.trait.kindle_wisdom", nameof(TraitDefinition), () => settings.CapacityAttribute);
		var endurance = ArmageddonCatalogueLinks.Bind(db, "arm.binding.trait.kindle_endurance", nameof(TraitDefinition), () =>
			ArmageddonEmotionalInstaller.ResolveAttribute(db, null));
		foreach (var id in new[] { strength, agility, wisdom, endurance })
			if (!db.TraitDefinitions.Any(x => x.Id == id && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0))
				throw new StockDependencyException("Kindle must retain existing ordinary body attributes; no derived trait or new stat is adopted.");
		if (wisdom != settings.CapacityAttribute)
			throw new StockDependencyException("Kindle's wisdom mapping must retain the installation's explicit native capacity attribute.");
		var reserves = new Dictionary<string, long>(StringComparer.Ordinal);
		foreach (var resourceKey in new ArmageddonStockProgression(settings.Mode).Capabilities.Select(x => x.ResourceKey).Distinct(StringComparer.Ordinal))
		{
			var tradition = resourceKey["arm.resource.".Length..];
			var id = ProgressionIdentity<MagicResource>(db, resourceKey);
			if (!db.MagicResources.Any(x => x.Id == id && (x.MagicResourceType & (int)MagicResourceType.PlayerResource) != 0))
				throw new StockDependencyException("Kindle requires an exact native character reserve: " + tradition);
			reserves.Add(tradition, id);
		}
		return ArmageddonKindleContent.Create(strength, agility, wisdom, endurance, reserves);
	}
}
