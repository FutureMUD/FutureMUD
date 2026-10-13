extern alias EngineCompiler;
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.FutureProg;
using MudSharp.Magic;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using OfflineProgCompilation = EngineCompiler::MudSharp.Framework.OfflineProgCompilation;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSpellInstaller
{
	private static long CatalogueIdentity<T>(FuturemudDatabaseContext db, string key) where T : class
	{
		var claim = CatalogueClaim<T>(db, key, false);
		if (db.Find<T>(claim.LogicalId!.Value) is null)
			throw new StockDependencyException(key + ": restore the exact native catalogue identity.");
		return claim.LogicalId.Value;
	}

	private static long CataloguePrototype(FuturemudDatabaseContext db, string key, string? requiredComponent = null)
	{
		var claim = CatalogueClaim<GameItemProto>(db, key, true);
		var proto = db.GameItemProtos.AsNoTracking().Include(x => x.EditableItem)
			.Include(x => x.GameItemProtosGameItemComponentProtos).ThenInclude(x => x.GameItemComponent).ThenInclude(x => x.EditableItem)
			.SingleOrDefault(x => x.Id == claim.LogicalId && x.RevisionNumber == claim.RevisionNumber);
		if (proto?.EditableItem?.RevisionStatus != (int)RevisionStatus.Current ||
			proto.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.EditableItem?.RevisionStatus != (int)RevisionStatus.Current) ||
			requiredComponent is not null && !proto.GameItemProtosGameItemComponentProtos.Any(x => x.GameItemComponent.Type == requiredComponent))
			throw new StockDependencyException(key + ": restore the approved native prototype and declared component revisions.");
		return proto.Id;
	}

	private static long CatalogueProg(FuturemudDatabaseContext db, string key, ProgVariableTypes result, params ProgVariableTypes[] parameters)
	{
		var id = CatalogueIdentity<FutureProg>(db, key);
		RequireProg(db, id, result, parameters);
		return id;
	}

	private static void RequireProg(FuturemudDatabaseContext db, long id, ProgVariableTypes result, params ProgVariableTypes[] parameters)
	{
		using var compiler = new OfflineProgCompilation(db.FutureProgs.Include(x => x.FutureProgsParameters).AsNoTracking().ToArray());
		var prog = compiler.Compile(id);
		if (!prog.ReturnType.CompatibleWith(result) || !prog.MatchesParameters(parameters))
			throw new StockDependencyException($"Prog #{id}: restore its declared {result.DescribeEnum()} signature before stock invocation.");
	}

	private static long ProjectionPlane(FuturemudDatabaseContext db) => ArmageddonCatalogueLinks.Bind(db,
		"arm.binding.plane.s4_projection", nameof(Plane), () =>
		{
			var candidates = db.Planes.AsNoTracking().Where(x => x.IsDefault).Select(x => x.Id).ToArray();
			if (candidates.Length != 1) throw new StockDependencyException("Select one explicit native default plane for the authored occupied-plane projection policy.");
			return candidates[0];
		});

	private static ArmageddonCanonicalSpellContribution ResolveExistingFactory(FuturemudDatabaseContext db, string key)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix package settings before stock factories.");
		var character = ProgVariableTypes.Character;
		Dictionary<int, long> Weapons(string family) => Enumerable.Range(1, 7)
			.ToDictionary(x => x, x => CataloguePrototype(db, $"arm.item.{family}.g{x}", "MeleeWeapon"));
		var always = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		long EmotionEligibility() { RequireProg(db, always, ProgVariableTypes.Boolean, character, character); return always; }
		ArmageddonUtilitySpellContent content;
		string school;
		switch (key)
		{
			case "arm.spell.sand_effigy":
				school = "earth";
				content = ArmageddonExistingFactorySpellContent.SandEffigy(ProjectionPlane(db), CataloguePrototype(db, "arm.item.sand_effigy_anchor", "Holdable"));
				break;
			case "arm.spell.walking_shadow":
				school = "shadow";
				content = ArmageddonExistingFactorySpellContent.WalkingShadow(CatalogueIdentity<Plane>(db, "arm.support.plane.shadow"));
				break;
			case "arm.spell.raise_servitor":
				school = "void";
				var commandable = CatalogueIdentity<ArtificialIntelligence>(db, "arm.ai.servitor");
				var combatEnd = CatalogueIdentity<ArtificialIntelligence>(db, "arm.ai.servitor.combat_end");
				if (!db.ArtificialIntelligences.Any(x => x.Id == commandable && x.Type == "Commandable") ||
					!db.ArtificialIntelligences.Any(x => x.Id == combatEnd && x.Type == "CombatEnd"))
					throw new StockDependencyException("Servitor control requires its exact native Commandable and CombatEnd bundle.");
				content = ArmageddonExistingFactorySpellContent.RaiseServitor(new(
					CatalogueProg(db, "arm.support.prog.corpse_eligibility", ProgVariableTypes.Boolean, ProgVariableTypes.Item, character), commandable, combatEnd));
				break;
			case "arm.spell.roused_fury":
				school = "earth";
				var strength = ArmageddonCatalogueLinks.Bind(db, StrengthBindingKey, nameof(TraitDefinition), () =>
					throw new StockDependencyException("Resolve the ordinary Strength binding before native Fury."));
				content = ArmageddonExistingFactorySpellContent.RousedFury(new(strength, EmotionEligibility()));
				break;
			case "arm.spell.still_anger":
				school = "water";
				// Normal is an explicit editable native save default, not a historical probability claim.
				content = ArmageddonExistingFactorySpellContent.StillAnger(new(
					CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey), EmotionEligibility(),
					Enumerable.Range(1, 7).ToDictionary(x => x, _ => Difficulty.Normal)));
				break;
			case "arm.spell.flame_knife":
				school = "fire";
				content = ArmageddonExistingFactorySpellContent.FlameKnife(new(
					Enumerable.Range(1, 6).Select(x => CataloguePrototype(db, $"arm.item.flame_knife.g{x}", "MeleeWeapon")).ToArray(),
					Enumerable.Range(1, 8).Select(x => CataloguePrototype(db, $"arm.item.flame_knife.permanent_choice_{x}", "MeleeWeapon")).ToArray(),
					CatalogueIdentity<Tag>(db, "arm.material.conjuration.rank6"),
					CatalogueProg(db, "arm.support.prog.weapon_eligibility", ProgVariableTypes.Boolean, character),
					CatalogueProg(db, "arm.support.prog.flame_multiplier", ProgVariableTypes.Number, character)));
				break;
			case "arm.spell.sand_knife":
				school = "earth"; content = ArmageddonExistingFactorySpellContent.SandKnife(Weapons("sand_knife")); break;
			case "arm.spell.storm_spear":
				school = "lightning"; content = ArmageddonExistingFactorySpellContent.StormSpear(Weapons("storm_spear")); break;
			default: throw new StockDependencyException(key + ": no exact native stock factory is registered.");
		}
		return new("arm.capability." + school, content);
	}

	private static ArmageddonCanonicalSpellContribution ResolveVital(FuturemudDatabaseContext db, string key)
	{
		if (key == "arm.spell.leeching_edge") return new("arm.capability.void", ArmageddonVitalTransferSpellContent.LeechingEdge());
		var eligibility = CatalogueProg(db, "arm.support.prog.target_eligibility", ProgVariableTypes.Boolean,
			ProgVariableTypes.Character, ProgVariableTypes.Character);
		// Copy the exact owned, compiled explicit merit policy into the reconciled target/caster child.
		var source = db.FutureProgs.AsNoTracking().Single(x => x.Id == eligibility).FunctionText;
		var resistance = CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey);
		return key switch
		{
			"arm.spell.vital_siphon" => new("arm.capability.water", ArmageddonVitalTransferSpellContent.VitalSiphon(resistance, source)),
			"arm.spell.devouring_touch" => new("arm.capability.void", ArmageddonVitalTransferSpellContent.DevouringTouch(resistance, source)),
			_ => throw new StockDependencyException(key + ": no exact vitality factory is registered.")
		};
	}
	private static readonly string[] SimpleKeys = ["arm.spell.fireworks", "arm.spell.distant_mirage", "arm.spell.shadowplay",
		"arm.spell.sky_lantern", "arm.spell.phantasm", "arm.spell.veil_of_elements", "arm.spell.buoyant_lift",
		"arm.spell.windborne_flight", "arm.spell.gentle_descent", "arm.spell.sand_barrier", "arm.spell.wind_barrier",
		"arm.spell.clouded_will", "arm.spell.arcane_mark", "arm.spell.unmaking_ward", "arm.spell.bar_the_elements"];

	private static long ProgressionIdentity<T>(FuturemudDatabaseContext db, string key) where T : class
	{
		var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
		if (claim is not { Retired: false, LogicalId: > 0, RevisionNumber: null, SeedBaseline: not null } ||
			claim.Module != ArmageddonProgressionInstaller.Module || claim.ManifestVersion != ArmageddonProgressionInstaller.Version ||
			claim.EntityType != typeof(T).Name || db.Find<T>(claim.LogicalId.Value) is null ||
			db.SeederManagedRecords.Any(x => x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId))
			throw new StockDependencyException(key + ": restore the exact unique typed progression support identity.");
		return claim.LogicalId.Value;
	}

	private static ArmageddonCanonicalSpellContribution ResolveSimple(FuturemudDatabaseContext db, string key) =>
		ArmageddonSimpleStockContent.Create(new(ProgressionIdentity<MagicResource>(db, "arm.resource.void"),
			ProgressionIdentity<MagicSchool>(db, "arm.school.fire"))).Single(x => x.Content.Key == key);

	private static ArmageddonCanonicalSpellContribution ResolveGradedWeapon(FuturemudDatabaseContext db, string key)
	{
		if (key == "arm.spell.gather_puddle")
			return new("arm.capability.water", ArmageddonGradedWeaponSpellContent.GatherPuddle(
				ArmageddonCatalogueLinks.Bind(db, "arm.liquid.water", nameof(Liquid), () =>
					throw new StockDependencyException("Retain the explicit native clean-water catalogue alias before puddle creation."))));
		if (key == "arm.spell.champion_blade")
			return new("arm.capability.shadow", ArmageddonGradedWeaponSpellContent.ChampionBlade(Enumerable.Range(1, 7)
				.ToDictionary(x => x, x => CataloguePrototype(db, $"arm.item.champion_blade.g{x}", "MeleeWeapon"))));
		var temporaryFamily = key == "arm.spell.shadow_blade" ? "shadow_blade" : "spark_lash";
		var temporary = Enumerable.Range(1, 6).Select(x => CataloguePrototype(db, $"arm.item.{temporaryFamily}.g{x}", "MeleeWeapon")).ToArray();
		var tag = CatalogueIdentity<Tag>(db, "arm.material.conjuration.rank6");
		var eligibility = CatalogueProg(db, ArmageddonStockSupportInstaller.WeaponEligibilityKey, ProgVariableTypes.Boolean, ProgVariableTypes.Character);
		if (key == "arm.spell.shadow_blade") return new("arm.capability.shadow", ArmageddonGradedWeaponSpellContent.ShadowBlade(new(temporary,
			Enumerable.Range(1, 8).Select(x => CataloguePrototype(db, ArmageddonStockSupportInstaller.ShadowStaffKey(x), "MeleeWeapon")).ToArray(), tag, eligibility,
			CatalogueProg(db, ArmageddonStockSupportInstaller.ShadowMultiplierKey, ProgVariableTypes.Number, ProgVariableTypes.Character))));
		if (key == "arm.spell.spark_lash") return new("arm.capability.lightning", ArmageddonGradedWeaponSpellContent.SparkLash(new(temporary,
			Enumerable.Range(1, 8).Select(x => CataloguePrototype(db, ArmageddonStockSupportInstaller.SparkStaffKey(x), "MeleeWeapon")).ToArray(), tag, eligibility)));
		throw new StockDependencyException(key + ": no exact graded native factory is registered.");
	}

	private static readonly string[] UtilityKeys = ["arm.spell.sense_enchantment", "arm.spell.mend_flesh", "arm.spell.draw_water",
		"arm.spell.hovering_light", "arm.spell.draw_wine", "arm.spell.sustain_meal", "arm.spell.pierce_concealment", "arm.spell.apex_bane"];

	private static ArmageddonCanonicalSpellContribution ResolveUtility(FuturemudDatabaseContext db, string key)
	{
		string Source(string progKey, ProgVariableTypes target)
		{
			var id = CatalogueProg(db, progKey, ProgVariableTypes.Boolean, target, ProgVariableTypes.Character);
			return db.FutureProgs.AsNoTracking().Single(x => x.Id == id).FunctionText;
		}
		var bindings = new ArmageddonCanonicalUtilityBindings(
			ArmageddonCatalogueLinks.Bind(db, "arm.liquid.water", nameof(Liquid), () =>
				throw new StockDependencyException("Retain the typed S3 native clean-water alias before utilities.")),
			CatalogueIdentity<Liquid>(db, "arm.liquid.wine"), CatalogueIdentity<Plane>(db, ArmageddonStockSupportInstaller.WaterPlaneKey),
			CataloguePrototype(db, "arm.item.hovering_light", "Wearable"),
			new[] { "arm.item.food.bread", "arm.item.food.fruit", "arm.item.food.stew" }.Select(x => CataloguePrototype(db, x, "Food")).ToArray(),
			CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey),
			Source(ArmageddonStockSupportInstaller.PerceptionEligibilityKey, ProgVariableTypes.Character),
			Source(ArmageddonStockSupportInstaller.TargetEligibilityKey, ProgVariableTypes.Character),
			Source(ArmageddonStockSupportInstaller.LiquidEligibilityKey, ProgVariableTypes.Item),
			Source(ArmageddonStockSupportInstaller.MagicalEligibilityKey, ProgVariableTypes.Character));
		return ArmageddonCanonicalUtilityContent.Contributions(bindings).Single(x => x.Content.Key == key);
	}

}
