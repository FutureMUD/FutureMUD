extern alias EngineCompiler;
#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
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
	private static ArmageddonCanonicalSpellContribution ResolveWeightedTravel(FuturemudDatabaseContext db, string key) =>
		ArmageddonWeightedTravelSpellContent.Create(CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.TravelRestrictedTagKey),
			CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.TravelDeathTagKey),
			CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.TravelUrbanPoolTagKey),
			CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.TravelWildPoolTagKey),
			CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.TravelElementalPoolTagKey),
			new[] { ArmageddonStockSupportInstaller.EarthPlaneKey, ArmageddonStockSupportInstaller.WaterPlaneKey,
				ArmageddonStockSupportInstaller.FirePlaneKey, ArmageddonStockSupportInstaller.WindPlaneKey,
				ArmageddonStockSupportInstaller.LightningPlaneKey, ArmageddonStockSupportInstaller.ShadowPlaneKey,
				ArmageddonStockSupportInstaller.VoidPlaneKey }.Select(plane => CatalogueIdentity<Plane>(db, plane)).ToArray(),
			CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey)).Single(x => x.Content.Key == key);

	private static long CanonicalSpellIdentity(FuturemudDatabaseContext db, string key)
	{
		var claims = db.SeederManagedRecords.AsNoTracking().Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key).ToArray();
		if (claims.Length != 1 || claims[0] is not { Retired: false, LogicalId: > 0, RevisionNumber: null, SeedBaseline: not null } claim ||
			claim.Module != Module || claim.ManifestVersion != Version || claim.EntityType != nameof(MagicSpell) ||
			db.SeederManagedRecords.Any(x => x.Id != claim.Id && x.EntityType == claim.EntityType && x.LogicalId == claim.LogicalId) ||
			db.MagicSpells.AsNoTracking().SingleOrDefault(x => x.Id == claim.LogicalId) is not { } spell || StockIdentity(spell.Definition) != key)
			throw new StockDependencyException(key + ": restore the exact uniquely owned canonical S4 spell before its dependent contribution.");
		return claim.LogicalId.Value;
	}

	private static (int Rank, long Tag)[] ConjurationRankOptions(FuturemudDatabaseContext db) => new[] { 0, 1, 2, 3, 4, 6 }.Select(rank =>
	{
		var key = "arm.material.conjuration.rank" + rank;
		CataloguePrototype(db, key, "Stackable");
		return (rank, CatalogueIdentity<Tag>(db, key + ".tag"));
	}).ToArray();

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
					CatalogueProg(db, ArmageddonStockSupportInstaller.CorpseEligibilityKey, ProgVariableTypes.Boolean, character, ProgVariableTypes.Item), commandable, combatEnd));
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
					CatalogueIdentity<Tag>(db, "arm.material.conjuration.rank6.tag"),
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

	private static ArmageddonCanonicalSpellContribution ResolveBoundConsumable(FuturemudDatabaseContext db, string key) =>
		key switch
		{
			ArmageddonBoundConsumableSpellContent.EmberSeedKey => new("arm.capability.fire",
				ArmageddonBoundConsumableSpellContent.EmberSeed(CataloguePrototype(db, "arm.item.ember_seed", "ArmageddonEmberSeed"))),
			ArmageddonBoundConsumableSpellContent.RestorativeMudKey => new("arm.capability.water",
				ArmageddonBoundConsumableSpellContent.RestorativeMud(RestorativeMudPrototype(db))),
			_ => throw new StockDependencyException(key + ": no exact native bound consumable factory is registered.")
		};

	private static long RestorativeMudPrototype(FuturemudDatabaseContext db)
	{
		const string key = "arm.item.restorative_mud";
		var id = CataloguePrototype(db, key, "TopicalCream");
		var component = db.GameItemProtos.AsNoTracking()
			.Include(x => x.GameItemProtosGameItemComponentProtos).ThenInclude(x => x.GameItemComponent)
			.Single(x => x.Id == id && x.RevisionNumber == CatalogueClaim<GameItemProto>(db, key, true).RevisionNumber)
			.GameItemProtosGameItemComponentProtos.Single(x => x.GameItemComponent.Type == "TopicalCream").GameItemComponent;
		XElement definition;
		try { definition = XElement.Parse(component.Definition); }
		catch (System.Xml.XmlException) { throw new StockDependencyException(key + ": restore the approved native one-gram healing dose profile."); }
		var dose = definition.Element("NativeHealingDose");
		var valid = dose is not null && (string?)dose.Attribute("key") == key &&
			double.TryParse((string?)dose.Attribute("grams"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var grams) && grams == 1.0 &&
			double.TryParse((string?)dose.Attribute("budgetPerGrade"), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out var budget) && budget > 0 && budget <= 10.0 &&
			(string?)definition.Element("OnApplyProg") == "0" && definition.Element("Drugs") is { HasElements: false };
		if (!valid) throw new StockDependencyException(key + ": restore the approved one-gram, at-most-10-per-grade unscripted healing dose profile.");
		return id;
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
		var tag = CatalogueIdentity<Tag>(db, "arm.material.creation.rank6.tag");
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

	private static ArmageddonCanonicalSpellContribution ResolveDirectDamage(FuturemudDatabaseContext db, string key) =>
		ArmageddonDirectDamageSpellContent.Create(new(CharacterResistance(db, "water", ArmageddonStockSpellContent.ReadAttunementKey),
			Difficulty.Normal, MudSharp.Health.DamageType.Arcane)).Single(x => x.Content.Key == key);

	private static ArmageddonCanonicalSpellContribution ResolveConsumingFlame(FuturemudDatabaseContext db)
	{
		CataloguePrototype(db, "arm.material.necromancy.rank0");
		return ArmageddonDirectDamageSpellContent.ConsumingFlame(
			CatalogueIdentity<Tag>(db, "arm.material.necromancy.rank0.tag"));
	}

	private static ArmageddonCanonicalSpellContribution ResolveFalsePresence(FuturemudDatabaseContext db)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix the installation before False Presence.");
		var applicability = settings.NativeBindings[ArmageddonNativeBinding.AlwaysTrueProg];
		RequireProg(db, applicability, ProgVariableTypes.Boolean, ProgVariableTypes.Character);
		return ArmageddonPrimitiveSpellContent.FalsePresence(applicability);
	}

	private static ArmageddonCanonicalSpellContribution ResolveWaterBreathing(FuturemudDatabaseContext db) =>
		new("arm.capability.water", ArmageddonReviewedWaterSeeContent.WaterBreathing(
			[CatalogueAlias(db, "arm.liquid.water", nameof(Liquid))]));

	private static ArmageddonCanonicalSpellContribution ResolveSeeTheUnbodied(FuturemudDatabaseContext db)
	{
		return new("arm.capability.shadow", ArmageddonReviewedWaterSeeContent.SeeTheUnbodiedOwned(
			CatalogueIdentity<Tag>(db, ArmageddonStockSupportInstaller.SiltTagKey),
			CatalogueIdentity<Plane>(db, ArmageddonStockSupportInstaller.ShadowPlaneKey), RankedComponentTags(db, "divination")));
	}

	private static long[] RankedComponentTags(FuturemudDatabaseContext db, string sphere, int count = 5) =>
		Enumerable.Range(0, count).Select(rank =>
		{
			var key = "arm.material." + sphere + ".rank" + rank;
			CataloguePrototype(db, key, "Stackable");
			var tag = CatalogueIdentity<Tag>(db, key + ".tag");
			if (rank > 0 && db.Tags.AsNoTracking().Single(x => x.Id == tag).ParentId !=
				CatalogueIdentity<Tag>(db, "arm.material." + sphere + ".rank" + (rank - 1) + ".tag"))
				throw new StockDependencyException("Owned " + sphere + " rank tags must retain their ascending ancestry.");
			return tag;
		}).ToArray();

	private static readonly (string Key, string School)[] ArmourKeys =
	[
		("flame_mantle", "fire"), ("unyielding_veil", "water"), ("mist_shield", "water"),
		("earthen_ward", "earth"), ("stone_skin", "earth"), ("wind_mantle", "wind"),
		("wind_shield", "wind"), ("shadow_mantle", "shadow"), ("charged_aegis", "lightning"),
		("sheltering_veil", "water")
	];

	private static ArmageddonCanonicalSpellContribution ResolveArmour(FuturemudDatabaseContext db, string key, string school)
	{
		var settings = ArmageddonInstallation.Read(db) ?? throw new StockDependencyException("Fix package settings before finite armour.");
		var material = settings.NativeBindings[ArmageddonNativeBinding.Stone];
		if (!db.Materials.Any(x => x.Id == material && x.Density > 0))
			throw new StockDependencyException("Retain the exact explicit native solid Stone material before finite armour.");
		var agility = key == "arm.spell.stone_skin" ? ResolveOrdinaryAgility(db) : 0;
		var profile = CatalogueIdentity<ArmourType>(db, key == "arm.spell.charged_aegis"
			? "arm.support.armour.electrical" : "arm.support.armour.external");
		if (agility > 0 && !db.TraitDefinitions.Any(x => x.Id == agility && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && x.TraitGroup == "Physical"))
			throw new StockDependencyException("Retain the exact ordinary physical mobility attribute binding; no replacement or derived stat is adopted.");
		var content = key == "arm.spell.sheltering_veil"
			? ArmageddonArmourSpellContent.ShelteringVeil(profile, material,
				CatalogueIdentity<Plane>(db, ArmageddonStockSupportInstaller.WaterPlaneKey), RankedComponentTags(db, "clerical"))
			: ArmageddonArmourSpellContent.Create(key, profile, material, agility);
		if (agility > 0) content = content with { Description = content.Description +
			" The retained native mobility attribute is " + db.TraitDefinitions.Single(x => x.Id == agility).Name + "." };
		return new("arm.capability." + school, content);
	}

	private static ArmageddonCanonicalSpellContribution ResolveMovementTempo(FuturemudDatabaseContext db, string key)
	{
		var agility = ResolveOrdinaryAgility(db);
		if (!db.TraitDefinitions.Any(x => x.Id == agility && x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 && x.TraitGroup == "Physical"))
			throw new StockDependencyException("Retain the exact ordinary physical mobility attribute binding before movement spells.");
		return ArmageddonMovementTempoSpellContent.Create(agility).Single(x => x.Content.Key == key);
	}

	private static long ResolveOrdinaryAgility(FuturemudDatabaseContext db) => ArmageddonCatalogueLinks.Bind(db,
			"arm.binding.trait.agility", nameof(TraitDefinition), () =>
			{
				// The supported labmud preset represents physical flexibility with ordinary Dexterity.
				// Prefer explicit Agility, then retain the sole ordinary Dexterity mapping; never create a stat.
				var ordinary = db.TraitDefinitions.Where(x => x.Type == 1 && x.DerivedType == 0 && x.OwnerScope == 0 &&
					x.TraitGroup == "Physical");
				var candidates = ordinary.Where(x => x.Alias == "agi").Select(x => x.Id).ToArray();
				if (candidates.Length == 0) candidates = ordinary.Where(x => x.Alias == "dex").Select(x => x.Id).ToArray();
				if (candidates.Length != 1) throw new StockDependencyException("Bind one ordinary native Physical mobility attribute (Agility, or the supported Dexterity equivalent); ambiguous identities are not adopted.");
				return candidates[0];
			});

	private static readonly string[] RetainedUtilityKeys =
		["arm.spell.luminous_trail", "arm.spell.read_enchantment", "arm.spell.echo_servant", "arm.spell.voice_of_remains"];

	private static ArmageddonCanonicalSpellContribution ResolveRetainedUtility(FuturemudDatabaseContext db, string key) =>
		ArmageddonRetainedUtilityContent.Create(new(
			CatalogueProg(db, ArmageddonStockSupportInstaller.IdentifyTextKey, ProgVariableTypes.Text, ProgVariableTypes.Character, ProgVariableTypes.Character),
			CatalogueProg(db, ArmageddonStockSupportInstaller.ReciteLinkKey, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Character),
			CatalogueProg(db, ArmageddonStockSupportInstaller.DeadSpeakLinkKey, ProgVariableTypes.Character, ProgVariableTypes.Character, ProgVariableTypes.Item),
			CorpseEligibilitySource: "return @target.iscorpse")).Single(x => x.Content.Key == key);

}
