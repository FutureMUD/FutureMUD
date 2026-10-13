#nullable enable
using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Text.Json;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.Form.Material;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSupportInstaller
{
	public const string CreationRankSixKey = "arm.material.creation.rank6";
	public const string CreationRankSixTagKey = CreationRankSixKey + ".tag";
	public const string CreationRankSixRecipeKey = CreationRankSixKey + ".recipe";
	public const string CreationRankSixGateKey = CreationRankSixRecipeKey + ".gate";
	public const string ExternalArmourKey = "arm.support.armour.external";
	public const string ElectricalArmourKey = "arm.support.armour.electrical";

	private static readonly DamageType[] ExternalDamageTypes =
	[
		DamageType.Slashing, DamageType.Chopping, DamageType.Crushing, DamageType.Piercing,
		DamageType.Ballistic, DamageType.Burning, DamageType.Freezing, DamageType.Chemical,
		DamageType.Shockwave, DamageType.Bite, DamageType.Claw, DamageType.Electrical,
		DamageType.Sonic, DamageType.Shearing, DamageType.BallisticArmourPiercing, DamageType.Wrenching,
		DamageType.Shrapnel, DamageType.Falling, DamageType.Arcane, DamageType.ArmourPiercing
	];

	private sealed record ProtectionBindings(GameItemComponentProto Holdable, GameItemComponentProto Stackable,
		GameItemProto Blank, TraitDefinition Crafting, long Clay, double Kilograms);

	private static ProtectionBindings ProtectionPreflight(FuturemudDatabaseContext db, ArmageddonInstallationSettings settings)
	{
		var holdable = Require<GameItemComponentProto>(db, "arm.catalogue.component.holdable", x => x.Type == "Holdable");
		var stackable = Require<GameItemComponentProto>(db, "arm.catalogue.component.stackable", x => x.Type == "Stackable");
		var blank = Require<GameItemProto>(db, "arm.item.component_blank", _ => true);
		var crafting = Require<TraitDefinition>(db, ArmageddonStockProgression.CraftTraitKey,
			x => x.Type == 0 && x.OwnerScope == 1, ArmageddonProgressionInstaller.Module);
		var alias = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == "arm.binding.material.clay");
		if (alias is null || !ArmageddonCatalogueLinks.Valid(db, alias) || alias.EntityType != ArmageddonCatalogueLinks.AliasType)
			throw new InvalidOperationException("Creation rank six requires the retained typed native clay alias.");
		var clay = JsonSerializer.Deserialize<ArmageddonCatalogueAlias>(alias.SeedBaseline!)!;
		if (clay.NativeType != nameof(Material) || !db.Materials.Any(x => x.Id == clay.NativeId && x.Density > 0 && x.BehaviourType == (int)MaterialBehaviourType.Soil))
			throw new InvalidOperationException("Creation rank six requires the exact compatible clay material.");
		var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
		if (!double.IsFinite(kilograms) || kilograms <= 0)
			throw new InvalidOperationException("Creation rank six requires a positive finite native weight conversion.");
		foreach (var (key, type, name) in new[]
		{
			(CreationRankSixKey, typeof(GameItemProto), "Armageddon High Creation component"),
			(CreationRankSixTagKey, typeof(Tag), "Creation rank 6"),
			(CreationRankSixRecipeKey, typeof(Craft), "Prepare High Creation component"),
			(CreationRankSixGateKey, typeof(FutureProg), "ArmageddonCraft_material_creation_rank6"),
			(ExternalArmourKey, typeof(ArmourType), "Armageddon external protection"),
			(ElectricalArmourKey, typeof(ArmourType), "Armageddon electrical protection")
		})
		{
			var claim = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
			if (claim is not null)
			{
				if (claim.Module != Module || claim.ManifestVersion != Version || claim.Retired || claim.EntityType != type.Name || claim.SeedBaseline is null)
					throw new InvalidOperationException($"{key}: retain complete typed supplemental ownership.");
				continue;
			}
			var collision = type == typeof(GameItemProto) ? db.GameItemProtos.Any(x => x.Name == name) :
				type == typeof(Tag) ? db.Tags.Any(x => x.Name == name) :
				type == typeof(Craft) ? db.Crafts.Any(x => x.Name == name) :
				type == typeof(FutureProg) ? db.FutureProgs.Any(x => x.FunctionName == name) : db.ArmourTypes.Any(x => x.Name == name);
			if (collision) throw new InvalidOperationException($"{key}: unowned native name collision; no adoption.");
		}
		return new(holdable, stackable, blank, crafting, clay.NativeId, kilograms);
	}

	private static void InstallProtection(ArmageddonCatalogueOwnership owned, ProtectionBindings bindings, Dictionary<string, long> ids, List<string> messages)
	{
		var tag = owned.Put(CreationRankSixTagKey, new Tag { Name = "Creation rank 6" }, []);
		var item = new GameItemProto
		{
			Id = owned.Id<GameItemProto>(CreationRankSixKey), Name = "Armageddon High Creation component",
			Keywords = "armageddon high creation component", ShortDescription = "a high creation component",
			FullDescription = "This high creation component is an authored rank-six supply for permanent magical weapon creation.",
			MaterialId = bindings.Clay, Size = (int)SizeCategory.VerySmall, Weight = 0.01 * bindings.Kilograms,
			BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval()
		};
		foreach (var component in new[] { bindings.Holdable, bindings.Stackable })
			item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		item.GameItemProtosTags.Add(new() { TagId = tag.Id });
		item = owned.Put(CreationRankSixKey, item, ["arm.binding.material.clay", "arm.catalogue.component.holdable", "arm.catalogue.component.stackable", CreationRankSixTagKey]);
		var gate = owned.Put(CreationRankSixGateKey, Prog("ArmageddonCraft_material_creation_rank6", ProgVariableTypes.Boolean,
			$"return GetTrait(@ch, ToTrait({bindings.Crafting.Id.ToString(CultureInfo.InvariantCulture)})) >= 90", ("ch", ProgVariableTypes.Character)), [ArmageddonStockProgression.CraftTraitKey]);
		var craft = new Craft
		{
			Id = owned.Id<Craft>(CreationRankSixRecipeKey), Name = "Prepare High Creation component", Category = "Armageddon supplies",
			Blurb = "Prepare a high creation component.", ActionDescription = "preparing a high creation component",
			ActiveCraftItemSdesc = "materials being prepared for magical work", Interruptable = true,
			InputQualityWeighting = 1, CheckQualityWeighting = 1, FailThreshold = (int)Outcome.MinorFail,
			CheckTraitId = bindings.Crafting.Id, CheckDifficulty = (int)Difficulty.Normal, FailPhase = 1,
			QualityFormula = ((int)ItemQuality.Standard).ToString(CultureInfo.InvariantCulture),
			AppearInCraftsListProgId = gate.Id, CanUseProgId = gate.Id, IsPracticalCheck = true, EditableItem = owned.Approval()
		};
		craft.CraftPhases.Add(new() { PhaseNumber = 1, PhaseLengthInSeconds = 420,
			Echo = "$0 finish|finishes preparing the magical supplies.", FailEcho = "$0 spoil|spoils the prepared materials.", ExertionLevel = (int)ExertionLevel.Low });
		craft.CraftInputs.Add(new() { InputType = "SimpleItem", Definition = new XElement("Definition", new XElement("TargetItemId", bindings.Blank.Id), new XElement("Quantity", 7)).ToString(), InputQualityWeight = 1, OriginalAdditionTime = new DateTime(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc) });
		craft.CraftProducts.Add(new() { ProductType = "SimpleProduct", Definition = new XElement("Definition", new XElement("ProductProducedId", item.Id), new XElement("Quantity", 1), new XElement("Skin", 0)).ToString(), OriginalAdditionTime = new DateTime(2026, 10, 11, 0, 0, 0, DateTimeKind.Utc) });
		craft = owned.Put(CreationRankSixRecipeKey, craft, ["arm.item.component_blank", CreationRankSixKey, CreationRankSixGateKey]);
		ids.Add(CreationRankSixKey, item.Id); ids.Add(CreationRankSixTagKey, tag.Id);
		ids.Add(CreationRankSixRecipeKey, craft.Id); ids.Add(CreationRankSixGateKey, gate.Id);
		foreach (var (key, electrical) in new[] { (ExternalArmourKey, false), (ElectricalArmourKey, true) })
			ids.Add(key, owned.Put(key, ArmourDefinition(electrical), []).Id);
		messages.Add("Supplemental Creation rank-six supply and recipe plus external/electrical armour profiles reconciled. Recipe and protection values are editable authored defaults; they do not certify historical crafting or protection probabilities. No live items were created.");
	}

	private static ArmourType ArmourDefinition(bool electrical)
	{
		var allowed = electrical ? new[] { DamageType.Electrical } : ExternalDamageTypes;
		XElement Expressions(string name, string channel, bool absorb) => new(name,
			Enum.GetValues<DamageType>().Select(type => new XElement("Expression", new XAttribute("damagetype", (int)type),
				new XCData(absorb && allowed.Contains(type) ? "0" : channel))));
		return new ArmourType
		{
			Name = electrical ? "Armageddon electrical protection" : "Armageddon external protection",
			MinimumPenetrationDegree = (int)OpposedOutcomeDegree.Total,
			Definition = new XElement("Definition", Expressions("DissipateExpressions", "damage", false),
				Expressions("DissipateExpressionsPain", "pain", false), Expressions("DissipateExpressionsStun", "stun", false),
				Expressions("AbsorbExpressions", "damage", true), Expressions("AbsorbExpressionsPain", "pain", true),
				Expressions("AbsorbExpressionsStun", "stun", true), new XElement("DamageTransformations")).ToString()
		};
	}
}
