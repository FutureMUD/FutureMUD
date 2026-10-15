#nullable enable

using System;
using System.Collections.Generic;
using CultureInfo = System.Globalization.CultureInfo;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Body;
using MudSharp.Database;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

public static partial class ArmageddonStockSupportInstaller
{
	public static string TravelRankFiveKey(string sphere) => sphere is "alteration" or "teleportation"
		? "arm.material." + sphere + ".rank5" : throw new ArgumentOutOfRangeException(nameof(sphere));

	private static void InstallTravelSupports(FuturemudDatabaseContext db, ArmageddonCatalogueOwnership owned,
		ProtectionBindings bindings, Dictionary<string, long> ids, List<string> messages)
	{
		foreach (var (key, name) in new[] { (TravelRestrictedTagKey, "Travel Restricted"), (TravelPopulatedTagKey, "Populated Travel Destination") })
			ids.Add(key, owned.Put(key, new Tag { Name = "Armageddon " + name }, []).Id);
		foreach (var sphere in new[] { "alteration", "teleportation" })
		{
			var parentKey = "arm.material." + sphere + ".rank4.tag";
			var parent = Require<Tag>(db, parentKey, _ => true);
			var key = TravelRankFiveKey(sphere);
			var title = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(sphere);
			var tag = owned.Put(key + ".tag", new Tag { Name = title + " rank 5", ParentId = parent.Id }, [parentKey]);
			var item = new GameItemProto
			{
				Id = owned.Id<GameItemProto>(key), Name = "Armageddon High " + title + " component",
				Keywords = "armageddon high " + sphere + " component", ShortDescription = "a high " + sphere + " component",
				FullDescription = "This rank-five " + sphere + " component supports the highest source travel grade.",
				MaterialId = bindings.Clay, Size = (int)SizeCategory.VerySmall, Weight = 0.01 * bindings.Kilograms,
				BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval()
			};
			foreach (var component in new[] { bindings.Holdable, bindings.Stackable })
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
			item.GameItemProtosTags.Add(new() { TagId = tag.Id });
			item = owned.Put(key, item, ["arm.binding.material.clay", "arm.catalogue.component.holdable", "arm.catalogue.component.stackable", key + ".tag"]);
			var gateKey = key + ".recipe.gate";
			var gate = owned.Put(gateKey, Prog("ArmageddonCraft_material_" + sphere + "_rank5", ProgVariableTypes.Boolean,
				$"return GetTrait(@ch, ToTrait({bindings.Crafting.Id.ToString(CultureInfo.InvariantCulture)})) >= 80", ("ch", ProgVariableTypes.Character)), [ArmageddonStockProgression.CraftTraitKey]);
			var craft = new Craft
			{
				Id = owned.Id<Craft>(key + ".recipe"), Name = "Prepare High " + title + " component", Category = "Armageddon supplies",
				Blurb = "Prepare a high " + sphere + " component.", ActionDescription = "preparing a high " + sphere + " component",
				ActiveCraftItemSdesc = "materials being prepared for magical work", Interruptable = true,
				InputQualityWeighting = 1, CheckQualityWeighting = 1, FailThreshold = (int)Outcome.MinorFail,
				CheckTraitId = bindings.Crafting.Id, CheckDifficulty = (int)Difficulty.Normal, FailPhase = 1,
				QualityFormula = ((int)ItemQuality.Standard).ToString(CultureInfo.InvariantCulture),
				AppearInCraftsListProgId = gate.Id, CanUseProgId = gate.Id, IsPracticalCheck = true, EditableItem = owned.Approval()
			};
			craft.CraftPhases.Add(new() { PhaseNumber = 1, PhaseLengthInSeconds = 360, Echo = "$0 finish|finishes preparing the magical supplies.",
				FailEcho = "$0 spoil|spoils the prepared materials.", ExertionLevel = (int)ExertionLevel.Low });
			craft.CraftInputs.Add(new() { InputType = "SimpleItem", Definition = new XElement("Definition", new XElement("TargetItemId", bindings.Blank.Id),
				new XElement("Quantity", 6)).ToString(), InputQualityWeight = 1, OriginalAdditionTime = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc) });
			craft.CraftProducts.Add(new() { ProductType = "SimpleProduct", Definition = new XElement("Definition", new XElement("ProductProducedId", item.Id),
				new XElement("Quantity", 1), new XElement("Skin", 0)).ToString(), OriginalAdditionTime = new DateTime(2026, 10, 15, 0, 0, 0, DateTimeKind.Utc) });
			craft = owned.Put(key + ".recipe", craft, ["arm.item.component_blank", key, gateKey]);
			ids.Add(key, item.Id); ids.Add(key + ".tag", tag.Id); ids.Add(key + ".recipe", craft.Id); ids.Add(gateKey, gate.Id);
		}
		messages.Add("Source travel rank-five Alteration/Teleportation supplies, real recipes and explicit restricted/populated room tags reconciled; no rooms were tagged or live items created.");
	}
}
