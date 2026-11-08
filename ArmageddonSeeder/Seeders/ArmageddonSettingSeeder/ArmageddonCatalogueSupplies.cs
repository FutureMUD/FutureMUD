#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using Microsoft.EntityFrameworkCore;
using MudSharp.Body;
using MudSharp.Construction;
using MudSharp.Database;
using MudSharp.Form.Material;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;
using CultureInfo = System.Globalization.CultureInfo;

namespace DatabaseSeeder.Seeders;

/// <summary>The native supply and shelter portion of S3. Full-preset activation is a separate gate.</summary>
public static class ArmageddonCatalogueSupplies
{
	private static readonly DateTime AdditionTime = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Fix Armageddon settings before catalogue content.");
		var errors = ArmageddonInstallation.ValidateFoundations(db, settings);
		var crafting = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package &&
			x.StableKey == ArmageddonStockProgression.CraftTraitKey && x.Module == ArmageddonProgressionInstaller.Module);
		if (crafting is null || crafting.EntityType != nameof(TraitDefinition) || crafting.Retired || crafting.LogicalId is null ||
			!db.TraitDefinitions.Any(x => x.Id == crafting.LogicalId && x.Type == 0 && x.OwnerScope == 1)) errors = errors.Append("Install the S2 native Component Crafting support before supply recipes.").ToArray();
		var decorators = db.StackDecorators.AsNoTracking().Where(x => x.Name == "Suffix" && x.Type == "Suffix").ToArray();
		if (decorators.Length != 1) errors = errors.Append("Exactly one native CoreDataSeeder Suffix stack decorator is required.").ToArray();
		if (errors.Count != 0) return new(ArmageddonInstallStatus.Blocked, errors, new Dictionary<string, long>());
		var catalogue = new ArmageddonStockCatalogue();
		var supplies = catalogue.Blueprints.Where(x => x.Kind is ArmageddonBlueprintKind.ComponentSupply or ArmageddonBlueprintKind.CraftSupply or ArmageddonBlueprintKind.StackableSupply).ToArray();
		if (supplies.Length != 64 || supplies.Any(x => x.Recipe is null)) throw new InvalidOperationException("The approved 61 ranked components and three base supplies require 64 recipes.");
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			var wood = ArmageddonCatalogueLinks.Bind(db, "arm.binding.material.wood", nameof(Material), () => settings.NativeBindings[ArmageddonNativeBinding.Wood]);
			var clay = ArmageddonCatalogueLinks.Bind(db, "arm.binding.material.clay", nameof(Material), () =>
			{
				var candidates = db.Materials.Where(x => x.Name == "clay" && x.BehaviourType == (int)MaterialBehaviourType.Soil && x.Density > 0).ToArray();
				if (candidates.Length != 1) throw new InvalidOperationException("Exactly one compatible CoreDataSeeder clay material is required; ambiguous world content is not adopted.");
				return candidates[0].Id;
			});
			var materials = new Dictionary<string, long>(StringComparer.Ordinal) { ["arm.binding.material.wood"] = wood, ["arm.binding.material.clay"] = clay };
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			if (db.Materials.Single(x => x.Id == wood).BehaviourType != (int)MaterialBehaviourType.Wood ||
				db.Materials.Single(x => x.Id == clay).BehaviourType != (int)MaterialBehaviourType.Soil) throw new InvalidOperationException("Fixed catalogue materials no longer have compatible native behaviours.");
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			var holdable = Component(owned, "arm.catalogue.component.holdable", "Armageddon catalogue holdable", "Holdable", new XElement("Definition"), []);
			var stackable = Component(owned, "arm.catalogue.component.stackable", "Armageddon catalogue stackable", "Stackable",
				new XElement("Definition", new XAttribute("Decorator", decorators[0].Id)), []);
			var items = new Dictionary<string, GameItemProto>(StringComparer.Ordinal);
			var rankedTags = new Dictionary<string, long>(StringComparer.Ordinal);
			foreach (var blueprint in supplies)
			{
				var material = blueprint.Configuration.TryGetProperty("native_material", out var materialKey) ? materials[materialKey.GetString()!] : clay;
				var item = new GameItemProto
				{
					Id = owned.Id<GameItemProto>(blueprint.Key),
					Name = blueprint.Kind == ArmageddonBlueprintKind.ComponentSupply ? "component" : blueprint.Key switch
					{
						"arm.item.focus_blank" or "arm.item.component_blank" => "blank",
						"arm.item.binding_seal" => "seal",
						_ => throw new InvalidOperationException($"Undeclared supply primary noun: {blueprint.Key}.")
					},
					UniqueName = "Armageddon " + blueprint.Name, Keywords = "armageddon " + blueprint.Name.ToLowerInvariant(),
					ShortDescription = "a " + blueprint.Name.ToLowerInvariant(), FullDescription = "This " + blueprint.Name.ToLowerInvariant() + " is a prepared material for magical work.",
					MaterialId = material, Size = (int)SizeCategory.VerySmall,
					Weight = blueprint.Configuration.GetProperty("weight_kilograms").GetDouble() * kilograms,
					BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval()
				};
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdable.Id, GameItemComponentRevision = holdable.RevisionNumber });
				if (blueprint.Kind is ArmageddonBlueprintKind.ComponentSupply or ArmageddonBlueprintKind.StackableSupply)
					item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = stackable.Id, GameItemComponentRevision = stackable.RevisionNumber });
				if (blueprint.Kind == ArmageddonBlueprintKind.ComponentSupply)
				{
					var sphere = blueprint.Configuration.GetProperty("sphere").GetString()!.ToLowerInvariant();
					var rank = blueprint.Configuration.GetProperty("rank").GetInt32();
					var name = blueprint.Configuration.TryGetProperty("native_tag", out var tagName) ? tagName.GetString()! :
						blueprint.Configuration.GetProperty("sphere").GetString() + " rank " + rank.ToString(CultureInfo.InvariantCulture);
					var rankedFamily = sphere is "clerical" or "divination" or "alteration" or "teleportation";
					var parentKey = sphere + ":" + (rank - 1).ToString(CultureInfo.InvariantCulture);
					if (rankedFamily && rank > 0 && !rankedTags.ContainsKey(parentKey))
						throw new InvalidOperationException("Owned ranked component families must be installed in ascending order.");
					var tag = owned.Put(blueprint.Key + ".tag", new Tag
					{
						Name = name, ParentId = rankedFamily && rank > 0 ? rankedTags[parentKey] : null
					}, rankedFamily && rank > 0 ? ["arm.material." + sphere + ".rank" + (rank - 1).ToString(CultureInfo.InvariantCulture) + ".tag"] : []);
					if (rankedFamily) rankedTags.Add(sphere + ":" + rank.ToString(CultureInfo.InvariantCulture), tag.Id);
					item.GameItemProtosTags.Add(new() { TagId = tag.Id });
				}
				items.Add(blueprint.Key, owned.Put(blueprint.Key, item, blueprint.Dependencies));
				ids.Add(blueprint.Key, item.Id);
			}
			foreach (var blueprint in supplies) Recipe(owned, blueprint, items, materials, crafting!.LogicalId!.Value, kilograms);
			var forageKey = "arm.gather.component_blank";
			var forage = owned.Put(forageKey, new Foragable
			{
				Id = owned.Id<Foragable>(forageKey), Name = "Armageddon component blank", ForagableTypes = "clay", ForageDifficulty = (int)Difficulty.Normal,
				RelativeChance = 100, MinimumOutcome = (int)Outcome.MinorPass, MaximumOutcome = (int)Outcome.MajorPass, QuantityDiceExpression = "1",
				ItemProtoId = items["arm.item.component_blank"].Id, EditableItem = owned.Approval()
			}, ["arm.item.component_blank"]);
			ArmageddonCatalogueLinks.AttachForage(db, forage.Id, messages);
			foreach (var blueprint in catalogue.Blueprints.Where(x => x.Kind == ArmageddonBlueprintKind.Shelter))
			{
				var underground = blueprint.Key == "arm.roomtemplate.burrow_refuge";
				var terrain = ArmageddonCatalogueLinks.Bind(db, underground ? "arm.binding.terrain.underground" : "arm.binding.terrain.sheltered", nameof(Terrain), () =>
				{
					var name = underground ? "Cave" : "Hallway";
					var candidates = db.Terrains.Where(x => x.Name == name).ToArray();
					if (candidates.Length != 1) throw new InvalidOperationException($"Exactly one CoreDataSeeder {name} terrain is required for shelter templates.");
					return candidates[0].Id;
				});
				var definition = new XElement("Template", new XElement("ApplyAutobuilderTagsAsFrameworkTags", false),
					new XElement("ShowCommandByline", new XCData("An Armageddon magical shelter template")), new XElement("Default", new XElement("Terrain",
						new XElement("DefaultTerrain", terrain), new XElement("RoomName", new XCData(blueprint.Name)),
						new XElement("RoomDescription", new XCData("The enclosed space offers shelter within its conjured walls.")),
						new XElement("OutdoorsType", (int)(underground ? RoomOutdoorsType.IndoorsNoLight : RoomOutdoorsType.Indoors)),
						new XElement("CellLightMultiplier", 1), new XElement("ForagableProfile", 0))), new XElement("Terrains"));
				var template = owned.Put(blueprint.Key, new AutobuilderRoomTemplate { Name = "Armageddon " + blueprint.Name, TemplateType = "room by terrain", Definition = definition.ToString() }, blueprint.Dependencies);
				ids.Add(blueprint.Key, template.Id);
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S3 supply/shelter batch committed: 61 ranked components, three base supplies, 64 native recipes, terrain-linked finite clay forage, and four native autobuilder room templates. No live items, creatures or rooms were created. Other S3 catalogue families and S4 spell activation remain pending.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. Rerun preserves owned identities. " : committing ? "Commit outcome unknown; reconnect before rerunning. " : "Catalogue batch failed before commit; relational transaction rolled back. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}

	private static GameItemComponentProto Component(ArmageddonCatalogueOwnership owned, string key, string name, string type, XElement definition, string[] dependencies) =>
		owned.Put(key, new GameItemComponentProto { Id = owned.Id<GameItemComponentProto>(key), Name = name, Type = type,
			Description = "Native component for the automatic Armageddon catalogue.", Definition = definition.ToString(), EditableItem = owned.Approval() }, dependencies);

	private static void Recipe(ArmageddonCatalogueOwnership owned, ArmageddonStockBlueprint blueprint,
		IReadOnlyDictionary<string, GameItemProto> items, IReadOnlyDictionary<string, long> materials, long crafting, double kilograms)
	{
		var recipe = blueprint.Recipe!;
		var key = blueprint.Key + ".recipe";
		var functionSuffix = blueprint.Key["arm.".Length..].Replace('.', '_');
		var gateKey = key + ".gate";
		var gate = new FutureProg { FunctionName = "ArmageddonCraft_" + functionSuffix,
			FunctionText = $"return GetTrait(@ch, ToTrait({crafting.ToString(CultureInfo.InvariantCulture)})) >= {recipe.MinimumComponentCrafting.ToString(CultureInfo.InvariantCulture)}",
			ReturnType = (long)ProgVariableTypes.Boolean, Category = "Armageddon", Subcategory = "Crafting", FunctionComment = "Native Component Crafting prerequisite.", Public = false };
		gate.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "ch", ParameterTypeDefinition = ((long)ProgVariableTypes.Character).ToString(CultureInfo.InvariantCulture) });
		gate = owned.Put(gateKey, gate, [ArmageddonStockProgression.CraftTraitKey]);
		var craft = new Craft { Id = owned.Id<Craft>(key), Name = "Prepare " + blueprint.Name, Category = "Armageddon supplies",
			Blurb = "Prepare " + blueprint.Name.ToLowerInvariant() + " from ordinary materials.", ActionDescription = "preparing " + blueprint.Name.ToLowerInvariant(),
			ActiveCraftItemSdesc = "materials being prepared for magical work", Interruptable = true, ToolQualityWeighting = 0,
			InputQualityWeighting = 1, CheckQualityWeighting = 1, FreeSkillChecks = 0, FailThreshold = (int)Outcome.MinorFail,
			CheckTraitId = crafting, CheckDifficulty = (int)Difficulty.Normal, FailPhase = 1, QualityFormula = ((int)ItemQuality.Standard).ToString(CultureInfo.InvariantCulture),
			AppearInCraftsListProgId = gate.Id, CanUseProgId = gate.Id, IsPracticalCheck = true, EditableItem = owned.Approval() };
		craft.CraftPhases.Add(new() { PhaseNumber = 1, PhaseLengthInSeconds = recipe.Seconds, Echo = "$0 finish|finishes preparing the magical supplies.",
			FailEcho = "$0 spoil|spoils the prepared materials.", ExertionLevel = (int)ExertionLevel.Low, StaminaUsage = 0 });
		var index = 0;
		foreach (var input in recipe.Inputs)
		{
			var definition = recipe.Kilograms
				? new XElement("Definition", new XElement("Material", materials[input.Key]),
					new XElement("Weight", input.Value * kilograms),
					new XElement("CommodityPileTag", 0), new XElement("Characteristics"))
				: new XElement("Definition", new XElement("TargetItemId", items[input.Key].Id), new XElement("Quantity", (int)input.Value));
			craft.CraftInputs.Add(new() { InputType = recipe.Kilograms ? "Commodity" : "SimpleItem", Definition = definition.ToString(), InputQualityWeight = 1,
				OriginalAdditionTime = AdditionTime.AddSeconds(index++) });
		}
		craft.CraftProducts.Add(new() { ProductType = "SimpleProduct", Definition = new XElement("Definition", new XElement("ProductProducedId", items[blueprint.Key].Id),
			new XElement("Quantity", 1), new XElement("Skin", 0)).ToString(), OriginalAdditionTime = AdditionTime });
		owned.Put(key, craft, blueprint.Dependencies.Append(blueprint.Key).Append(gateKey).ToArray());
	}
}
