#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Models;
using Liquid = MudSharp.Models.Liquid;

namespace DatabaseSeeder.Seeders;

/// <summary>Native food and drink stock. Source nutrition is translated into satiation; no calorie model.</summary>
public static class ArmageddonCatalogueProvisions
{
	public static double SatiationHours(double sourceCalories)
	{
		if (!double.IsFinite(sourceCalories) || sourceCalories <= 0)
			throw new ArgumentOutOfRangeException(nameof(sourceCalories));
		// Approved source translation: 2,400 per day, expressed solely as native satiation hours.
		return sourceCalories * 24.0 / 2400.0;
	}

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before provisions.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		var holdable = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonCatalogueOwnership.Module && x.StableKey == "arm.catalogue.component.holdable");
		if (holdable is null || holdable.EntityType != nameof(GameItemComponentProto) || holdable.Retired ||
			holdable.LogicalId is null || holdable.RevisionNumber is null)
			errors.Add("Install the S3 catalogue holdable before provisions.");
		var decorators = db.StackDecorators.AsNoTracking().Where(x => x.Name == "Suffix" && x.Type == "Suffix").ToArray();
		if (decorators.Length != 1) errors.Add("Exactly one native Suffix decorator is required for partial food.");
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var catalogue = new ArmageddonStockCatalogue();
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var holdableComponent = db.GameItemComponentProtos.Single(x => x.Id == holdable!.LogicalId && x.RevisionNumber == holdable.RevisionNumber);
			var foodMaterial = ArmageddonCatalogueLinks.Bind(db, "arm.binding.material.food", nameof(Material), () =>
			{
				var matches = db.Materials.Where(x => x.Name == "food" && x.BehaviourType == (int)MaterialBehaviourType.Food && x.Density > 0).ToArray();
				if (matches.Length != 1) throw new InvalidOperationException("Exactly one compatible CoreDataSeeder food material is required.");
				return matches[0].Id;
			});
			if (db.Materials.Single(x => x.Id == foodMaterial).BehaviourType != (int)MaterialBehaviourType.Food)
				throw new InvalidOperationException("The retained food material no longer has native Food behaviour.");
			var waterId = ArmageddonCatalogueLinks.Bind(db, "arm.liquid.water", nameof(Liquid), () => settings.NativeBindings[ArmageddonNativeBinding.Water]);
			var water = db.Liquids.AsNoTracking().Single(x => x.Id == waterId);
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			var litres = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			foreach (var blueprint in catalogue.Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.Food))
			{
				var config = blueprint.Configuration;
				var hydration = config.GetProperty("water_litres").GetDouble();
				var component = Component(owned, blueprint.Key + ".component", blueprint.Name, "Food",
					new XElement("Definition", new XAttribute("Satiation", SatiationHours(config.GetProperty("calories").GetDouble())),
						new XAttribute("Water", hydration), new XAttribute("Thirst", hydration * water.DrinkSatiatedHoursPerLitre),
						new XAttribute("Alcohol", 0), new XAttribute("Bites", config.GetProperty("bites").GetDouble()),
						new XAttribute("Decorator", decorators[0].Id), new XElement("Taste", new XCData("It tastes like sustaining prepared food.")),
						new XElement("OnEatProg", 0)), ["arm.liquid.water"]);
				var item = Item(owned, blueprint, foodMaterial, kilograms, [holdableComponent, component], ["arm.binding.material.food", blueprint.Key + ".component"]);
				ids.Add(blueprint.Key, item.Id);
			}
			var bottle = catalogue.ByKey["arm.item.drink_container"];
			var capacity = bottle.Configuration.GetProperty("capacity_litres").GetDouble();
			var container = Component(owned, bottle.Key + ".component", bottle.Name, "Liquid Container",
				new XElement("Definition", new XAttribute("LiquidCapacity", capacity * litres), new XAttribute("Closable", true),
					new XAttribute("Transparent", false), new XAttribute("WeightLimit", capacity * kilograms),
					new XAttribute("OnceOnly", false), new XAttribute("AdjustQuantityProg", 0), new XAttribute("DefaultLiquid", 0),
					new XAttribute("CanBeEmptiedWhenInRoom", true)), []);
			var vessel = Item(owned, bottle, settings.NativeBindings[ArmageddonNativeBinding.Wood], kilograms,
				[holdableComponent, container], [bottle.Key + ".component"]);
			ids.Add(bottle.Key, vessel.Id);
			var wine = catalogue.ByKey["arm.liquid.wine"];
			var wineConfig = wine.Configuration;
			var hydrationFraction = wineConfig.GetProperty("water_fraction").GetDouble();
			var liquid = owned.Put(wine.Key, new Liquid
			{
				Name = "Armageddon " + wine.Name, Description = "a dark red wine", LongDescription = "A dark red wine.",
				TasteText = "It tastes of red wine.", VagueTasteText = "It tastes fruity.", SmellText = "It smells of red wine.", VagueSmellText = "It smells fruity.",
				TasteIntensity = 1, SmellIntensity = 1, AlcoholLitresPerLitre = wineConfig.GetProperty("ethanol_fraction").GetDouble(),
				WaterLitresPerLitre = hydrationFraction, FoodSatiatedHoursPerLitre = 0, DrinkSatiatedHoursPerLitre = hydrationFraction * water.DrinkSatiatedHoursPerLitre,
				Density = 1000.0 * wineConfig.GetProperty("native_density_kg_per_litre").GetDouble(), Viscosity = water.Viscosity, Organic = true,
				ThermalConductivity = water.ThermalConductivity, ElectricalConductivity = water.ElectricalConductivity,
				SpecificHeatCapacity = water.SpecificHeatCapacity, FreezingPoint = -5, BoilingPoint = 95,
				SolventId = waterId, SolventVolumeRatio = 1, DisplayColour = "red",
				DampDescription = "It is damp with wine.", WetDescription = "It is wet with wine.", DrenchedDescription = "It is drenched with wine.",
				DampShortDescription = "wine-damp", WetShortDescription = "wine-wet", DrenchedShortDescription = "wine-drenched",
				InjectionConsequence = water.InjectionConsequence, RelativeEnthalpy = water.RelativeEnthalpy
			}, ["arm.liquid.water"]);
			ids.Add(wine.Key, liquid.Id); ids.Add("arm.liquid.water", waterId);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true; transaction?.Commit(); committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 provisions committed: three native foods with five satiation hours each, an empty closable one-litre bottle, native red wine and a retained water binding. No live items or player nutrition changed. Foci/devices remain deferred to S4.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Rerun preserves owned identities. " : committing ? "Commit outcome unknown; reconnect before rerunning. " : "Provisions failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static GameItemComponentProto Component(ArmageddonCatalogueOwnership owned, string key, string name, string type, XElement definition, string[] dependencies) =>
		owned.Put(key, new GameItemComponentProto { Id = owned.Id<GameItemComponentProto>(key), Name = "Armageddon " + name + " component",
			Type = type, Description = "Native Armageddon provision component.", Definition = definition.ToString(), EditableItem = owned.Approval() }, dependencies);

	private static GameItemProto Item(ArmageddonCatalogueOwnership owned, ArmageddonStockBlueprint blueprint, long material, double kilograms,
		GameItemComponentProto[] components, string[] dependencies)
	{
		var item = new GameItemProto { Id = owned.Id<GameItemProto>(blueprint.Key), Name = "Armageddon " + blueprint.Name,
			Keywords = "armageddon " + blueprint.Name.ToLowerInvariant(), ShortDescription = "a serving of " + blueprint.Name.ToLowerInvariant(),
			FullDescription = "This is " + blueprint.Name.ToLowerInvariant() + ".", MaterialId = material, Size = (int)SizeCategory.Small,
			Weight = blueprint.Configuration.GetProperty("weight_kilograms").GetDouble() * kilograms,
			BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval() };
		if (blueprint.Kind == ArmageddonBlueprintKind.LiquidContainer) item.ShortDescription = "a wooden supply bottle";
		foreach (var component in components) item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		return owned.Put(blueprint.Key, item, dependencies.Prepend("arm.catalogue.component.holdable").ToArray());
	}
}
