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
}
