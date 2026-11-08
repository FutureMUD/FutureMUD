extern alias EngineCompiler;
#nullable enable

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Framework;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static readonly string[] PrimitiveKeys = new[] {"borrowed_tongues", "hush_hearing", "seal_voice", "heavy_slumber",
		"earthen_strength", "sapping_weight", "veil_from_sight", "reveal_the_hidden", "shroud_eyes", "clear_eyes",
		"lift_hex", "dread_presence", "night_eyes", "clinging_hex", "second_breath", "wakeful_mind", "still_limbs", "empty_aura" }
		.Select(x => "arm.spell." + x).ToArray();
	internal const string StrengthBindingKey = "arm.binding.trait.strength";
	internal const string ResistanceBindingKey = "arm.binding.trait.spell_resistance";

	private static ArmageddonCanonicalSpellContribution ResolvePrimitive(FuturemudDatabaseContext db, string key)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix the installation before S4 primitive content.");
		var strength = ArmageddonCatalogueLinks.Bind(db, StrengthBindingKey, nameof(TraitDefinition), () =>
		{
			var candidates = db.TraitDefinitions.Where(x => x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 &&
				x.TraitGroup == "Physical" && x.Alias == "str").Select(x => x.Id).ToArray();
			if (candidates.Length != 1) throw new StockDependencyException("Bind one ordinary native Physical Strength attribute before S4 modifiers; no trait is created or claimed.");
			return candidates[0];
		});
		if (!db.TraitDefinitions.Any(x => x.Id == strength && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && x.TraitGroup == "Physical"))
			throw new StockDependencyException("The retained Strength binding must remain an ordinary body Physical attribute.");
		var resistance = ArmageddonCatalogueLinks.Bind(db, ResistanceBindingKey, nameof(TraitDefinition), () => settings.CapacityAttribute);
		if (resistance != settings.CapacityAttribute)
			throw new StockDependencyException("The authored resistance default must retain the installation's explicit capacity attribute.");
		var applicability = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		using (var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray()))
		{
			var prog = compiler.Compile(applicability);
			if (!prog.ReturnType.CompatibleWith(ProgVariableTypes.Boolean) || !prog.MatchesParameters([ProgVariableTypes.Character]))
				throw new StockDependencyException("The fixed native always-true applicability prog must accept its Character viewer.");
		}
		// Source GET_MANA is one shared character resource for spell costs and reserve effects.
		var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == ArmageddonProgressionInstaller.ManaResourceKey);
		if (claim is not { Retired: false, LogicalId: > 0, RevisionNumber: null, SeedBaseline: not null } ||
			claim.Module != ArmageddonProgressionInstaller.Module || claim.ManifestVersion != ArmageddonProgressionInstaller.Version ||
			claim.EntityType != nameof(MagicResource) || !db.MagicResources.Any(x => x.Id == claim.LogicalId && x.MagicResourceType == 1) ||
			db.SeederManagedRecords.Any(x => x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId))
			throw new StockDependencyException("Empty Aura requires the exact owned character Mana resource.");
		if (key == ArmageddonNativeReserveSpellContent.SecondBreathKey) return ArmageddonNativeReserveSpellContent.SecondBreath();
		if (key == ArmageddonNativeReserveSpellContent.EmptyAuraKey) return ArmageddonNativeReserveSpellContent.EmptyAura(claim.LogicalId.Value);
		return ArmageddonPrimitiveSpellContent.Create(new(strength, resistance, applicability, claim.LogicalId.Value, Difficulty.Normal))
			.Single(x => x.Content.Key == key);
	}
}
