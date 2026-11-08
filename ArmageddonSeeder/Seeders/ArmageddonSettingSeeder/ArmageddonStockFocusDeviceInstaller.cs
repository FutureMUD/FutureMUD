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
using MudSharp.FutureProg;
using MudSharp.GameItems;
using MudSharp.GameItems.Prototypes;
using MudSharp.Models;
using MudSharp.RPG.Checks;

namespace DatabaseSeeder.Seeders;

/// <summary>Approved stock foci and Mend Flesh devices with installation-scoped, builder-editable permissions.</summary>
public static class ArmageddonStockFocusDeviceInstaller
{
	public const string UseGateKey = "arm.stock.focus-device.use-gate";
	public const string CraftGateKey = "arm.stock.focus-device.craft-gate";
	public static IReadOnlyList<string> Keys => new ArmageddonStockCatalogue().Blueprints
		.Where(x => x.DeferredToS4).Select(x => x.Key).ToArray();

	public static ArmageddonInstallResult Install(Func<FuturemudDatabaseContext> factory,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = factory();
		var messages = new List<string>();
		var ids = new Dictionary<string, long>(StringComparer.Ordinal);
		var committing = false;
		var committed = false;
		try
		{
			var settings = ArmageddonInstallation.Read(db) ?? throw new InvalidOperationException("Install fixed Armageddon settings first.");
			long Require<T>(string key) where T : class
			{
				var claim = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
				if (claim is null || claim.Retired || claim.EntityType != typeof(T).Name || claim.LogicalId is not > 0)
					throw new InvalidOperationException($"{key}: missing compatible owned dependency.");
				var identity = claim.LogicalId.Value;
				if (!db.Set<T>().Any(x => EF.Property<long>(x, "Id") == identity))
					throw new InvalidOperationException($"{key}: owned native row is missing.");
				return identity;
			}
			foreach (var (key, parameters) in new[] { (UseGateKey, 2), (CraftGateKey, 1) })
			{
				var claim = db.SeederManagedRecords.AsNoTracking().SingleOrDefault(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key);
				if (claim is null) continue;
				var id = Require<FutureProg>(key);
				var gate = db.FutureProgs.Include(x => x.FutureProgsParameters).Single(x => x.Id == id);
				var signature = gate.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => x.ParameterTypeDefinition).ToArray();
				var expected = parameters == 2 ? new[] { ((long)ProgVariableTypes.Character).ToString(), ((long)ProgVariableTypes.Item).ToString() } : new[] { ((long)ProgVariableTypes.Character).ToString() };
				if (gate.ReturnType != (long)ProgVariableTypes.Boolean || gate.AcceptsAnyParameters || !signature.SequenceEqual(expected))
					throw new InvalidOperationException($"{key}: stock availability requires its exact native signature; preserve and resolve the edited gate explicitly.");
			}
			var blueprints = new ArmageddonStockCatalogue().Blueprints.Where(x => x.DeferredToS4).ToArray();
			if (blueprints.Length != 12 || blueprints.Count(x => x.Kind == ArmageddonBlueprintKind.Focus) != 10)
				throw new InvalidOperationException("The locked S4 batch requires ten foci and two blank devices.");
			var progression = new ArmageddonStockProgression(settings.Mode);
			var spells = ArmageddonStockSpellInstaller.CanonicalKeys.ToDictionary(x => x, Require<MagicSpell>, StringComparer.Ordinal);
			foreach (var spell in spells)
			{
				var claim = db.SeederManagedRecords.Single(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == spell.Key);
				var definition = XElement.Parse(db.MagicSpells.Single(x => x.Id == spell.Value).Definition);
				if (claim.Module != ArmageddonStockSpellInstaller.Module || (string?)definition.Element("StockIdentity") != spell.Key || definition.Element("ControlledPower") is null)
					throw new InvalidOperationException($"{spell.Key}: require the real owned canonical controlled spell definition.");
			}
			var crafting = Require<TraitDefinition>(ArmageddonStockProgression.CraftTraitKey);
			var holdableId = Require<GameItemComponentProto>("arm.catalogue.component.holdable");
			var holdable = db.GameItemComponentProtos.Single(x => x.Id == holdableId);
			if (holdable.Type != "Holdable") throw new InvalidOperationException("The owned holdable component has changed native type.");
			var inputs = blueprints.SelectMany(x => x.Dependencies).Distinct().ToDictionary(x => x, Require<GameItemProto>);
			var material = db.GameItemProtos.Single(x => x.Id == inputs["arm.item.focus_blank"]).MaterialId;
			var kilograms = db.UnitsOfMeasure.Single(x => x.Id == settings.NativeBindings[ArmageddonNativeBinding.Kilogram]).BaseMultiplier;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
			using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
			var owned = new ArmageddonCatalogueOwnership(db, settings.NativeBindings[ArmageddonNativeBinding.BuilderAccount], messages);
			FutureProg Gate(string key, bool item)
			{
				var gate = new FutureProg { FunctionName = item ? "ArmageddonStockFocusDeviceUse" : "ArmageddonStockFocusDeviceCraft",
					FunctionText = settings.StockAvailabilityByDefault ? "return true" : "return false", ReturnType = (long)ProgVariableTypes.Boolean, Category = "Armageddon",
					Subcategory = "Stock availability", FunctionComment = "Approved stock permission; native capability, knowledge, payment and crafting checks still apply.", Public = false };
				gate.FutureProgsParameters.Add(new() { ParameterIndex = 0, ParameterName = "ch", ParameterTypeDefinition = ((long)ProgVariableTypes.Character).ToString() });
				if (item) gate.FutureProgsParameters.Add(new() { ParameterIndex = 1, ParameterName = "item", ParameterTypeDefinition = ((long)ProgVariableTypes.Item).ToString() });
				return owned.Put(key, gate, []);
			}
			var useGate = Gate(UseGateKey, true);
			var craftGate = Gate(CraftGateKey, false);
			foreach (var blueprint in blueprints)
			{
				var focus = blueprint.Kind == ArmageddonBlueprintKind.Focus;
				var capability = focus ? progression.Capabilities.Single(x => x.Key == blueprint.Key.Replace("arm.focus.", "arm.capability.")) : null;
				var allowed = focus ? capability!.SpellKeys : new[] { "arm.spell.mend_flesh" };
				var componentKey = blueprint.Key + ".component";
				var definition = MagicDeviceDefinition.Serialize(blueprint.Key.EndsWith("staff", StringComparison.Ordinal) ? MagicDeviceKind.Staff : MagicDeviceKind.Wand,
					focus ? MagicDeviceRole.Focus : MagicDeviceRole.Dual, MagicDeviceEligibility.Caster,
					focus ? 1 : blueprint.Configuration.GetProperty("capacity").GetInt32(), 60, 0, useGate.Id, 0, 0,
					Difficulty.Normal, Outcome.MinorPass, allowed.Select(x => spells[x]), new XElement("Plan", new XElement("Phase")));
				var component = owned.Put(componentKey, new GameItemComponentProto
				{
					Id = owned.Id<GameItemComponentProto>(componentKey), Name = "Armageddon stock " + blueprint.Name + " component",
					Type = "ChargedMagicDevice", Description = focus ? "Explicit retained personal casting focus; no charges or mastery granted." : "Empty caster-only homogeneous charge bank. Focus casting must be requested explicitly.",
					Definition = definition, EditableItem = owned.Approval()
				}, allowed.Append(UseGateKey).ToArray());
				var item = new GameItemProto
				{
					Id = owned.Id<GameItemProto>(blueprint.Key),
					Name = focus ? "focus" : blueprint.Key.EndsWith("staff", StringComparison.Ordinal) ? "staff" : "wand",
					UniqueName = "Armageddon stock " + blueprint.Name,
					Keywords = "armageddon " + blueprint.Name.ToLowerInvariant(), ShortDescription = "a " + blueprint.Name.ToLowerInvariant(),
					FullDescription = focus ? "This retained focus supports explicit personal casting. It supplies neither mastery nor a charged fallback." : "This plain wooden carrier is supplied empty. Every charge requires legitimate paid production.",
					MaterialId = material, Size = (int)(blueprint.Key.EndsWith("staff", StringComparison.Ordinal) ? SizeCategory.Normal : SizeCategory.VerySmall),
					Weight = blueprint.Configuration.GetProperty("weight_kilograms").GetDouble() * kilograms,
					BaseItemQuality = (int)ItemQuality.Standard, MorphEmote = "$0 changes.", EditableItem = owned.Approval()
				};
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = holdable.Id, GameItemComponentRevision = holdable.RevisionNumber });
				item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = component.Id, GameItemComponentRevision = component.RevisionNumber });
				item = owned.Put(blueprint.Key, item, blueprint.Dependencies.Append(componentKey).Append("arm.catalogue.component.holdable").ToArray());
				ids.Add(blueprint.Key, item.Id);
				var key = blueprint.Key + ".recipe";
				var craft = new Craft
				{
					Id = owned.Id<Craft>(key), Name = "Prepare Armageddon stock " + blueprint.Name, Category = "Armageddon foci and devices",
					Blurb = "Prepare " + blueprint.Name.ToLowerInvariant() + ".", ActionDescription = "preparing " + blueprint.Name.ToLowerInvariant(),
					ActiveCraftItemSdesc = "materials being prepared for magical work", Interruptable = true, InputQualityWeighting = 1, CheckQualityWeighting = 1,
					FailThreshold = (int)Outcome.MinorFail, CheckTraitId = crafting, CheckDifficulty = (int)Difficulty.Normal, FailPhase = 1,
					QualityFormula = ((int)ItemQuality.Standard).ToString(), AppearInCraftsListProgId = craftGate.Id, CanUseProgId = craftGate.Id,
					IsPracticalCheck = true, EditableItem = owned.Approval()
				};
				craft.CraftPhases.Add(new() { PhaseNumber = 1, PhaseLengthInSeconds = blueprint.Recipe!.Seconds,
					Echo = "$0 work|works on the focus or carrier.", FailEcho = "$0 spoil|spoils the materials.", ExertionLevel = (int)ExertionLevel.Low });
				// Native crafting does not schedule the final phase duration. A separate
				// zero-duration finish phase releases products after the timed work phase.
				craft.CraftPhases.Add(new() { PhaseNumber = 2, PhaseLengthInSeconds = 0,
					Echo = "$0 finish|finishes preparing the focus or carrier.", FailEcho = "$0 discard|discards the spoiled materials.", ExertionLevel = (int)ExertionLevel.Low });
				var addition = new DateTime(2000, 1, 1);
				foreach (var input in blueprint.Recipe.Inputs)
				{
					craft.CraftInputs.Add(new() { InputType = "SimpleItem", InputQualityWeight = 1, OriginalAdditionTime = addition,
						Definition = new XElement("Definition", new XElement("TargetItemId", inputs[input.Key]), new XElement("Quantity", checked((int)input.Value))).ToString() });
					addition = addition.AddSeconds(1);
				}
				craft.CraftProducts.Add(new() { ProductType = "SimpleProduct", OriginalAdditionTime = new DateTime(2000, 1, 1),
					Definition = new XElement("Definition", new XElement("ProductProducedId", item.Id), new XElement("Quantity", 1), new XElement("Skin", 0)).ToString() });
				owned.Put(key, craft, blueprint.Dependencies.Append(blueprint.Key).Append(CraftGateKey).Append(ArmageddonStockProgression.CraftTraitKey).ToArray());
			}
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
			db.SaveChanges();
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
			committing = true;
			transaction?.Commit();
			committed = true;
			checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
			messages.Add("S4 installed ten tradition foci, empty capacity-five wand, empty capacity-ten staff and twelve native recipes. Use/crafting defaults follow the fixed installation policy; builder edits are preserved. Only Mend Flesh is approved for stock charges.");
			return new(ArmageddonInstallStatus.Completed, messages.AsReadOnly(), ids);
		}
		catch (Exception error)
		{
			messages.Add((committed ? "Committed; confirmation failed. " : committing ? "Commit outcome unknown. " : "S4 focus/device batch failed before commit. ") + error.Message);
			if (!committed) ids.Clear();
			return new(committed ? ArmageddonInstallStatus.CommittedConfirmationFailed : committing ? ArmageddonInstallStatus.CommitOutcomeUnknown : ArmageddonInstallStatus.Failed, messages.AsReadOnly(), ids);
		}
	}
}
