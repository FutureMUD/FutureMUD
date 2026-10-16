#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Native specialised stock; invocation bindings and calibrated exposure remain S4/S5 gates.</summary>
public static class ArmageddonCatalogueSpecials
{
	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before specialised catalogue content.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings).ToList();
		var holdableRecord = db.SeederManagedRecords.SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.Module == ArmageddonCatalogueOwnership.Module && x.StableKey == "arm.catalogue.component.holdable");
		if (holdableRecord is null || holdableRecord.EntityType != nameof(GameItemComponentProto) || holdableRecord.Retired ||
			holdableRecord.LogicalId is null || holdableRecord.RevisionNumber is null)
			errors.Add("Install the owned stock holdable before specialised items.");
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var messages = new List<string>(); var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false; var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var catalogue = new ArmageddonStockCatalogue();
			var holdable = db.GameItemComponentProtos.Single(x => x.Id == holdableRecord!.LogicalId && x.RevisionNumber == holdableRecord.RevisionNumber);
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			var litres = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Litre]).BaseMultiplier;
			var bodyId = db.Races.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.HumanRace]).BaseBodyId;
			var headShapeId = ArmageddonCatalogueLinks.Bind(db, "arm.binding.light.head_shape", nameof(BodypartShape), () =>
				db.BodypartShapes.Where(x => x.Name == "head").Select(x => x.Id).Single());
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var wearKey = "arm.item.hovering_light.wear_profile";
			var wear = owned.Put(wearKey, new WearProfile { Name = "Armageddon Hovering Light attachment", Type = "Shape", BodyPrototypeId = bodyId,
				Description = "A transparent light attachment hovering around one head.", WearStringInventory = "hovering around",
				WearAction1st = "attach", WearAction3rd = "attaches", WearAffix = "around", RequireContainerIsEmpty = false,
				WearlocProfiles = new XElement("Profiles", new XElement("Shape", new XAttribute("ShapeId", headShapeId), new XAttribute("Count", 1),
					new XAttribute("Transparent", true), new XAttribute("NoArmour", true), new XAttribute("PreventsRemoval", false),
					new XAttribute("Mandatory", true), new XAttribute("HidesSevered", false))).ToString() }, ["arm.binding.light.head_shape"]);
			var light = catalogue.ByKey["arm.item.hovering_light"];
			var wearable = Component(owned, light.Key + ".wearable", light.Name, "Wearable", new XElement("Definition",
				new XAttribute("DisplayInventoryWhenWorn", true), new XAttribute("Bulky", false), new XElement("WearableProg", 0),
				new XElement("WhyCannotWearProg", 0), new XElement("SeeThroughDamageRatio", 0.5), new XElement("LayerWeightConsumption", 0),
				new XElement("Waterproof", new XAttribute("ratio", 0.5), false),
				new XElement("Profiles", new XAttribute("Default", wear.Id), new XElement("Profile", wear.Id))), [wearKey]);
			var illumination = Component(owned, light.Key + ".light", light.Name, "Prog Light",
				new XElement("Definition", new XElement("IlluminationProvided", light.Configuration.GetProperty("lux_per_grade").GetDouble()),
					new XElement("MultiplyIlluminationBySpellGrade", true)), []);
			ids.Add(light.Key, Item(owned, light, settings, kilograms, [holdable, wearable, illumination],
				["arm.catalogue.component.holdable", light.Key + ".wearable", light.Key + ".light"]).Id);
			var ember = catalogue.ByKey["arm.item.ember_seed"];
			var emberComponent = Component(owned, ember.Key + ".component", ember.Name, "ArmageddonEmberSeed",
				new XElement("Definition", new XAttribute("damagePerGrade", 3)), []);
			ids.Add(ember.Key, Item(owned, ember, settings, kilograms, [holdable, emberComponent],
				["arm.catalogue.component.holdable", ember.Key + ".component"]).Id);
			var effigy = catalogue.ByKey["arm.item.sand_effigy_anchor"];
			ids.Add(effigy.Key, Item(owned, effigy, settings, kilograms, [holdable], ["arm.catalogue.component.holdable"]).Id);
			var pocket = catalogue.ByKey["arm.item.folded_pocket"];
			var pocketComponent = Component(owned, pocket.Key + ".component", pocket.Name, "FoldedPocket", new XElement("Definition",
				new XAttribute("Weight", 14 * kilograms), new XAttribute("MaxSize", (int)SizeCategory.VeryLarge),
				new XAttribute("Preposition", "in"), new XAttribute("Closable", false), new XAttribute("Transparent", false),
				new XAttribute("OnceOnly", false), new XElement("AllowedTags"), new XElement("BlockedTags")), []);
			ids.Add(pocket.Key, Item(owned, pocket, settings, kilograms, [holdable, pocketComponent],
				["arm.catalogue.component.holdable", pocket.Key + ".component"]).Id);
			var water = catalogue.ByKey["arm.item.haven_water"];
			var waterComponent = Component(owned, water.Key + ".component", water.Name, "Liquid Container", new XElement("Definition",
				new XAttribute("LiquidCapacity", 3.5 * litres), new XAttribute("Closable", false), new XAttribute("Transparent", true),
				new XAttribute("WeightLimit", 3.5 * kilograms), new XAttribute("OnceOnly", false), new XAttribute("AdjustQuantityProg", 0),
				new XAttribute("DefaultLiquid", 0), new XAttribute("CanBeEmptiedWhenInRoom", true)), []);
			ids.Add(water.Key, Item(owned, water, settings, kilograms, [waterComponent], [water.Key + ".component"]).Id);
			var mud = catalogue.ByKey["arm.item.restorative_mud"];
			var mudComponent = Component(owned, mud.Key + ".component", mud.Name, "TopicalCream", new XElement("Definition",
				new XElement("TotalGrams", kilograms / 1000.0), new XElement("OnApplyProg", 0), new XElement("Drugs"),
				new XElement("NativeHealingDose", new XAttribute("key", mud.Key), new XAttribute("grams", 1), new XAttribute("budgetPerGrade", 10))), []);
			ids.Add(mud.Key, Item(owned, mud, settings, kilograms, [holdable, mudComponent],
				["arm.catalogue.component.holdable", mud.Key + ".component"], 0.001).Id);
			var poison = new Drug { Name = "Armageddon Venom Touch poison", DrugVectors = (int)DrugVector.Touched,
				IntensityPerGram = 1000, RelativeMetabolisationRate = 1 };
			poison.DrugsIntensities.Add(new() { DrugType = (int)DrugType.BodypartDamage, RelativeIntensity = 1,
				AdditionalEffects = ((int)BodypartTypeEnum.Liver).ToString(System.Globalization.CultureInfo.InvariantCulture) });
			poison = owned.Put("arm.effect.poison", poison, []); ids.Add("arm.effect.poison", poison.Id);
			var infection = ArmageddonCatalogueLinks.Bind(db, "arm.effect.disease", nameof(InfectionType), () => (long)InfectionType.SownSickness);
			ids.Add("arm.effect.disease", infection);
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated); checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true; transaction?.Commit(); committed = true; checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 specialised stock committed: six native item prototypes, contact poison and the explicit nonspreading infection type. Real invocation bindings and S5 exposure calibration remain gated.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown; reconnect before rerunning. " :
				"Specialised stock failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown :
				ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static GameItemComponentProto Component(ArmageddonCatalogueOwnership owned, string key, string name, string type,
		XElement definition, IReadOnlyList<string> dependencies) => owned.Put(key, new GameItemComponentProto { Id = owned.Id<GameItemComponentProto>(key),
		RevisionNumber = 0, EditableItem = owned.Approval(), Name = "Armageddon " + name + " " + type,
		Description = "Native stock component; graded invocation binding is deferred to S4.", Type = type, Definition = definition.ToString() }, dependencies);

	private static GameItemProto Item(ArmageddonCatalogueOwnership owned, ArmageddonStockBlueprint blueprint,
		ArmageddonInstallationSettings settings, double kilograms, IReadOnlyList<GameItemComponentProto> components,
		IReadOnlyList<string> dependencies, double? weight = null)
	{
		var row = new GameItemProto { Id = owned.Id<GameItemProto>(blueprint.Key), RevisionNumber = 0, EditableItem = owned.Approval(),
			Name = "Armageddon " + blueprint.Name, Keywords = blueprint.Name.ToLowerInvariant(), ShortDescription = "a " + blueprint.Name.ToLowerInvariant(),
			FullDescription = $"An unmistakable magical {blueprint.Name.ToLowerInvariant()}.", LongDescription = "", MorphEmote = "",
			MaterialId = settings.NativeBindings[ArmageddonNativeBinding.Stone], Size = (int)SizeCategory.Small,
			Weight = kilograms * (weight ?? (blueprint.Configuration.TryGetProperty("weight_kilograms", out var value) ? value.GetDouble() : 0.2)),
			BaseItemQuality = (int)ItemQuality.Standard };
		foreach (var component in components) row.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
		return owned.Put(blueprint.Key, row, dependencies);
	}
}
