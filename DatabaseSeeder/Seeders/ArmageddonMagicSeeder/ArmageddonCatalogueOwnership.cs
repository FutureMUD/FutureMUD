#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Framework.Revision;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

/// <summary>Typed native rows and explicit child collections, with three-way stock reconciliation.</summary>
internal sealed partial class ArmageddonCatalogueOwnership
{
	internal const string Module = "stock-catalogue";
	internal const string Version = "stock-catalogue-1";
	private static readonly IReadOnlyDictionary<Type, string[]> OwnedFields = MedicalOwnedFields(CreatureOwnedFields(AnatomyOwnedFields(new Dictionary<Type, string[]>
	{
		[typeof(Tag)] = ["Name", "ParentId", "ShouldSeeProgId"],
		[typeof(RegionalGatheringProfile)] = ["Name", "Definition"],
		[typeof(MagicGenerator)] = ["Name", "Type", "Definition"],
		[typeof(ForagableProfile)] = ["Name"],
		[typeof(Terrain)] = ["Name", "MovementRate", "DefaultTerrain", "TerrainBehaviourMode", "HideDifficulty", "SpotDifficulty", "StaminaCost", "ForagableProfileId", "InfectionMultiplier", "InfectionType", "InfectionVirulence", "AtmosphereId", "AtmosphereType", "TerrainEditorColour", "TerrainANSIColour", "WeatherControllerId", "DefaultRoomOutdoorsType", "TerrainEditorText", "CanHaveTracks", "TrackIntensityMultiplierVisual", "TrackIntensityMultiplierOlfactory", "TagInformation", "GravityModel", "DefaultAgricultureFieldProfileId", "EnvironmentalMagicProfileId"],
		[typeof(ArmourType)] = ["Name", "MinimumPenetrationDegree", "BaseDifficultyDegrees", "StackedDifficultyDegrees", "Definition"],
		[typeof(Plane)] = ["Name", "Alias", "Description", "RoomDescriptionAddendum", "RoomNameFormat", "RemoteObservationTag", "DisplayOrder", "IsDefault"],
		[typeof(Liquid)] = ["Name", "Description", "LongDescription", "TasteText", "VagueTasteText", "SmellText", "VagueSmellText", "TasteIntensity", "SmellIntensity", "AlcoholLitresPerLitre", "WaterLitresPerLitre", "FoodSatiatedHoursPerLitre", "DrinkSatiatedHoursPerLitre", "Viscosity", "Density", "Organic", "ThermalConductivity", "ElectricalConductivity", "SpecificHeatCapacity", "IgnitionPoint", "FreezingPoint", "BoilingPoint", "DraughtProgId", "SolventId", "CountAsId", "CountAsQuality", "DisplayColour", "DampDescription", "WetDescription", "DrenchedDescription", "DampShortDescription", "WetShortDescription", "DrenchedShortDescription", "SolventVolumeRatio", "DriedResidueId", "DrugId", "DrugGramsPerUnitVolume", "InjectionConsequence", "ResidueVolumePercentage", "RelativeEnthalpy", "GasFormId", "LeaveResidueInRooms", "SurfaceReactionInfo", "StaleAfterSeconds", "SpoilAfterSeconds", "StaleLiquidId", "SpoiledLiquidId"],
		[typeof(TraitExpression)] = ["Name", "Expression"],
		[typeof(WeaponType)] = ["Name", "Classification", "AttackTraitId", "ParryTraitId", "ParryBonus", "Reach", "StaminaPerParry"],
		[typeof(WeaponAttack)] = ["Name", "WeaponTypeId", "Verb", "FutureProgId", "BaseAttackerDifficulty", "BaseBlockDifficulty", "BaseDodgeDifficulty", "BaseParryDifficulty", "BaseAngleOfIncidence", "RecoveryDifficultySuccess", "RecoveryDifficultyFailure", "MoveType", "Intentions", "ExertionLevel", "DamageType", "DamageExpressionId", "StunExpressionId", "PainExpressionId", "Weighting", "MaximumTargets", "BodypartShapeId", "StaminaCost", "BaseDelay", "Orientation", "Alignment", "AdditionalInfo", "HandednessOptions", "RequiredPositionStateIds", "OnUseProgId"],
		[typeof(CombatMessage)] = ["Type", "Outcome", "Message", "ProgId", "AuxiliaryProgId", "Priority", "Verb", "Chance", "FailureMessage"],
		[typeof(GameItemComponentProto)] = ["Name", "Description", "Type", "Definition"],
		[typeof(GameItemProto)] = ["Name", "Keywords", "ShortDescription", "FullDescription", "MaterialId", "Size", "Weight", "BaseItemQuality", "MorphEmote", "LongDescription"],
		[typeof(Craft)] = ["Name", "Blurb", "ActionDescription", "Category", "Interruptable", "ToolQualityWeighting", "InputQualityWeighting", "CheckQualityWeighting", "FreeSkillChecks", "FailThreshold", "CheckTraitId", "CheckDifficulty", "FailPhase", "QualityFormula", "AppearInCraftsListProgId", "CanUseProgId", "WhyCannotUseProgId", "ActiveCraftItemSdesc", "IsPracticalCheck"],
		[typeof(FutureProg)] = ["FunctionName", "FunctionText", "ReturnType", "AcceptsAnyParameters", "Category", "Subcategory", "FunctionComment", "Public", "StaticType"],
		[typeof(AutobuilderRoomTemplate)] = ["Name", "TemplateType", "Definition"],
		[typeof(Foragable)] = ["Name", "ForagableTypes", "ForageDifficulty", "RelativeChance", "MinimumOutcome", "MaximumOutcome", "QuantityDiceExpression", "ItemProtoId", "CommodityMaterialId", "CommodityTagId", "CommodityWeightExpression", "OnForageProgId", "CanForageProgId"]
	})));
	private readonly FuturemudDatabaseContext _db;
	private readonly Dictionary<string, SeederManagedRecord> _records;
	private readonly List<string> _messages;
	private readonly long _builder;
	private readonly Dictionary<(Type Type, string Key), long> _allocated = new();
	private readonly Dictionary<Type, long> _maximumIds = new();
	private readonly Dictionary<string, object> _staged = new(StringComparer.Ordinal);
	private readonly Dictionary<string, Dictionary<string, string>> _stagedFields = new(StringComparer.Ordinal);
	private readonly List<Action> _flush = new();
	private bool _batching;

	internal ArmageddonCatalogueOwnership(FuturemudDatabaseContext db, long builder, List<string> messages)
	{
		_db = db;
		_builder = builder;
		_messages = messages;
		_records = db.SeederManagedRecords.Where(x => x.Seeder == ArmageddonMagicInstaller.Package && x.Module == Module)
			.ToDictionary(x => x.StableKey, StringComparer.Ordinal);
		foreach (var record in _records.Values)
		{
			var type = OwnedFields.Keys.SingleOrDefault(x => x.Name == record.EntityType);
			var row = type is null || record.LogicalId is null ? null : Find(type, record);
			if (type is null || record.Retired || record.LogicalId is null || row is null ||
				db.SeederManagedRecords.Any(x => x.Id != record.Id && x.EntityType == record.EntityType && x.LogicalId == record.LogicalId))
				throw new InvalidOperationException($"{record.StableKey}: incompatible, missing, retired or competing catalogue ownership; no takeover.");
			if (db.Entry(row).Metadata.FindProperty("EditableItemId") is not null)
			{
				var editableId = (long)db.Entry(row).Property("EditableItemId").CurrentValue!;
				if (!db.EditableItems.Any(x => x.Id == editableId && x.RevisionStatus == (int)RevisionStatus.Current))
					throw new InvalidOperationException($"{record.StableKey}: retained native revision is not approved/current; resolve explicitly.");
			}
		}
	}

	private object? Find(Type type, SeederManagedRecord record)
	{
		var revisioned = _db.Model.FindEntityType(type)!.FindPrimaryKey()!.Properties.Count == 2;
		if (revisioned != (record.RevisionNumber is not null)) return null;
		return _db.Find(type, revisioned ? [record.LogicalId!.Value, record.RevisionNumber!.Value] : [record.LogicalId!.Value]);
	}

	internal long Id<T>(string key) where T : class
	{
		if (_records.TryGetValue(key, out var record))
		{
			if (record.EntityType != typeof(T).Name) throw new InvalidOperationException($"{key}: native identity type changed.");
			return record.LogicalId!.Value;
		}
		if (_allocated.TryGetValue((typeof(T), key), out var allocated)) return allocated;
		if (!_maximumIds.TryGetValue(typeof(T), out var maximum))
		{
			var nativeMaximum = _db.Set<T>().Select(x => (long?)EF.Property<long>(x, "Id")).Max() ?? 0;
			var type = typeof(T).Name;
			var claimedMaximum = _db.SeederManagedRecords.Where(x => x.EntityType == type).Select(x => x.LogicalId).Max() ?? 0;
			maximum = Math.Max(nativeMaximum, claimedMaximum);
		}
		_maximumIds[typeof(T)] = checked(maximum + 1);
		return _allocated[(typeof(T), key)] = _maximumIds[typeof(T)];
	}

	/// <summary>Stage an explicitly typed graph before saving its mutually dependent native rows.</summary>
	internal T Stage<T>(string key, T desired, IReadOnlyList<string> dependencies) where T : class
	{
		if (!OwnedFields.ContainsKey(typeof(T)) || !_staged.TryAdd(key, desired))
			throw new InvalidOperationException($"{key}: undeclared or duplicate staged catalogue row.");
		_flush.Add(() => Put(key, desired, dependencies));
		return desired;
	}

	internal void Flush()
	{
		// Only this call's newly constructed rows may be registered after graph insertion.
		// Existing unowned rows can never enter this set through name or ID matching.
		foreach (var entry in _staged) _stagedFields.Add(entry.Key, Fields(entry.Value));
		var fresh = _staged.Where(x => !_records.ContainsKey(x.Key)).ToArray();
		if (fresh.Select(x => x.Value).OfType<MoveSpeed>().GroupBy(x => x.BodyProtoId)
			.Any(body => body.GroupBy(x => x.Alias, StringComparer.OrdinalIgnoreCase).Any(alias => alias.Count() > 1)))
			throw new InvalidOperationException("A staged body has duplicate native movement aliases; resolve the source topology before installation.");
		foreach (var entry in fresh)
		{
			var name = _db.Entry(entry.Value).Metadata.FindProperty("Name");
			if (name is not null && StagedNameCollision(entry.Key, entry.Value))
				throw new InvalidOperationException($"{entry.Key}: unowned native name collision; no adoption.");
			if (entry.Value is MoveSpeed speed && _db.MoveSpeeds.Any(x => x.BodyProtoId == speed.BodyProtoId && x.Alias == speed.Alias))
				throw new InvalidOperationException($"{entry.Key}: native movement alias collision; retained IDs cannot be replaced by a newly bound source speed.");
		}
		foreach (var entry in fresh) _db.Add(entry.Value);
		// Body -> default smashing part -> body is a nullable native FK cycle.
		// Insert with that edge absent, then complete it inside the same transaction.
		var smashing = fresh.Select(x => x.Value).OfType<BodyProto>()
			.Where(x => x.DefaultSmashingBodypartId is not null).ToDictionary(x => x, x => x.DefaultSmashingBodypartId);
		if (smashing.Count != 0)
		{
			foreach (var body in smashing.Keys) body.DefaultSmashingBodypartId = null;
			_db.SaveChanges();
			foreach (var entry in smashing) entry.Key.DefaultSmashingBodypartId = entry.Value;
		}
		_batching = true;
		try
		{
			foreach (var action in _flush) action();
			_db.SaveChanges();
			// Several native columns have SQL defaults and a CLR zero sentinel. A genuine
			// authored zero may therefore be replaced during INSERT. Restore only these
			// freshly constructed rows' explicit whitelist values with ordinary UPDATEs,
			// inside the same transaction, before accepting their desired stock baselines.
			foreach (var entry in fresh)
				foreach (var field in OwnedFields[entry.Value.GetType()])
				{
					var property = _db.Entry(entry.Value).Property(field);
					property.CurrentValue = JsonSerializer.Deserialize(_stagedFields[entry.Key][field], property.Metadata.ClrType);
				}
			_db.SaveChanges();
			_flush.Clear();
			_staged.Clear();
			_stagedFields.Clear();
		}
		finally { _batching = false; }
	}

	internal T Copy<T>(T source) where T : class, new()
	{
		var result = new T();
		foreach (var field in OwnedFields[typeof(T)])
			_db.Entry(result).Property(field).CurrentValue = _db.Entry(source).Property(field).CurrentValue;
		return result;
	}

	internal EditableItem Approval() => new()
	{
		BuilderAccountId = _builder, ReviewerAccountId = _builder, BuilderDate = DateTime.UtcNow, ReviewerDate = DateTime.UtcNow,
		RevisionStatus = (int)RevisionStatus.Current, BuilderComment = "Armageddon stock catalogue.", ReviewerComment = "Typed native dependencies; full magic activation remains gated."
	};

	internal T Put<T>(string key, T desired, IReadOnlyList<string> dependencies) where T : class
	{
		if (!OwnedFields.ContainsKey(typeof(T))) throw new InvalidOperationException($"Undeclared catalogue entity type {typeof(T).Name}.");
		var next = _batching && _stagedFields.TryGetValue(key, out var stagedFields)
			? new Dictionary<string, string>(stagedFields, StringComparer.Ordinal) : Fields(desired);
		var created = !_records.TryGetValue(key, out var record);
		T row;
		if (created)
		{
			if (_db.SeederManagedRecords.Any(x => x.Seeder == ArmageddonMagicInstaller.Package && x.StableKey == key))
				throw new InvalidOperationException($"{key}: an existing module owns this stable identity.");
			var nameField = typeof(T) == typeof(FutureProg) ? "FunctionName" : "Name";
			if (_db.Entry(desired).Metadata.FindProperty(nameField) is not null)
			{
				var name = (string)_db.Entry(desired).Property(nameField).CurrentValue!;
				if (NameCollision(key, desired, nameField, name))
					throw new InvalidOperationException($"{key}: unowned native name collision; no adoption.");
			}
			row = desired;
			if (!_staged.TryGetValue(key, out var stagedRow) || !ReferenceEquals(stagedRow, row)) _db.Add(row);
			if (!_batching) _db.SaveChanges();
			var id = (long)_db.Entry(row).Property("Id").CurrentValue!;
			if (_maximumIds.TryGetValue(typeof(T), out var maximum) && id > maximum) _maximumIds[typeof(T)] = id;
			if (_db.SeederManagedRecords.Any(x => x.EntityType == typeof(T).Name && x.LogicalId == id))
				throw new InvalidOperationException($"{key}: retained logical identity collision.");
			record = new SeederManagedRecord { Seeder = ArmageddonMagicInstaller.Package, Module = Module, StableKey = key,
				EntityType = typeof(T).Name, LogicalId = id, RevisionNumber = _db.Entry(row).Metadata.FindProperty("RevisionNumber") is null ? null : (int)_db.Entry(row).Property("RevisionNumber").CurrentValue! };
			_db.SeederManagedRecords.Add(record);
			_records.Add(key, record);
		}
		else
		{
			if (record!.EntityType != typeof(T).Name) throw new InvalidOperationException($"{key}: incompatible native type.");
			row = (T)Find(typeof(T), record)!;
			LoadChildren(row);
		}
		var current = Fields(row);
		var baseline = record!.SeedBaseline is null ? null : JsonSerializer.Deserialize<Dictionary<string, string>>(record.SeedBaseline);
		if (row is Craft && baseline is not null)
		{
			// Native DATETIME is a wall-clock ordering key. Its provider Kind is not an edit.
			if (baseline.TryGetValue("Inputs", out var inputs)) baseline["Inputs"] = JsonSerializer.Serialize(
				JsonSerializer.Deserialize<Input[]>(inputs)!.Select(x => x with { Added = NativeDateTime(x.Added) }).ToArray());
			if (baseline.TryGetValue("Products", out var products)) baseline["Products"] = JsonSerializer.Serialize(
				JsonSerializer.Deserialize<Product[]>(products)!.Select(x => x with { Added = NativeDateTime(x.Added) }).ToArray());
			record.SeedBaseline = JsonSerializer.Serialize(baseline);
		}
		next["$dependencies"] = JsonSerializer.Serialize(dependencies);
		if (baseline?.TryGetValue("$dependencies", out var oldDependencies) == true) current["$dependencies"] = oldDependencies;
		var merged = SeederManagedRecordReconciler.Reconcile(record, current, next, created, _messages);
		foreach (var property in _db.Entry(row).Properties.Where(x => OwnedFields[typeof(T)].Contains(x.Metadata.Name)))
			property.CurrentValue = JsonSerializer.Deserialize(merged[property.Metadata.Name], property.Metadata.ClrType);
		if (!created) ApplyChildren(row, current, merged);
		record.ManifestVersion = Version;
		if (created || current.Count != merged.Count || current.Any(x => !merged.TryGetValue(x.Key, out var value) || value != x.Value)) record.AppliedAt = DateTime.UtcNow;
		if (!_batching) _db.SaveChanges();
		return row;
	}

	private void LoadChildren(object row)
	{
		if (row is GameItemProto item)
		{
			_db.Entry(item).Collection(x => x.GameItemProtosGameItemComponentProtos).Load();
			_db.Entry(item).Collection(x => x.GameItemProtosTags).Load();
		}
		if (row is Craft craft)
		{
			_db.Entry(craft).Collection(x => x.CraftInputs).Load();
			_db.Entry(craft).Collection(x => x.CraftProducts).Load();
			_db.Entry(craft).Collection(x => x.CraftPhases).Load();
			_db.Entry(craft).Collection(x => x.CraftTools).Load();
			if (craft.CraftTools.Count != 0) _messages.Add($"{craft.Name}: builder-added tools retained.");
		}
		if (row is FutureProg prog) _db.Entry(prog).Collection(x => x.FutureProgsParameters).Load();
		if (row is CombatMessage message) _db.Entry(message).Collection(x => x.CombatMessagesWeaponAttacks).Load();
		LoadAnatomyChildren(row);
		LoadCreatureChildren(row);
		LoadMedicalChildren(row);
		LoadLandChildren(row);
	}

	private Dictionary<string, string> Fields(object row)
	{
		var fields = _db.Entry(row).Properties.Where(x => OwnedFields[row.GetType()].Contains(x.Metadata.Name))
			.ToDictionary(x => x.Metadata.Name, x => JsonSerializer.Serialize(x.CurrentValue, x.Metadata.ClrType), StringComparer.Ordinal);
		if (row is GameItemProto item)
		{
			fields["Components"] = JsonSerializer.Serialize(item.GameItemProtosGameItemComponentProtos.OrderBy(x => x.GameItemComponentProtoId)
				.Select(x => new ComponentLink(x.GameItemComponentProtoId, x.GameItemComponentRevision)).ToArray());
			fields["Tags"] = JsonSerializer.Serialize(item.GameItemProtosTags.Select(x => x.TagId).Order().ToArray());
		}
		if (row is FutureProg prog) fields["Parameters"] = JsonSerializer.Serialize(prog.FutureProgsParameters.OrderBy(x => x.ParameterIndex)
			.Select(x => new Parameter(x.ParameterIndex, x.ParameterName, x.ParameterTypeDefinition)).ToArray());
		if (row is CombatMessage message) fields["WeaponAttacks"] = JsonSerializer.Serialize(message.CombatMessagesWeaponAttacks.Select(x => x.WeaponAttackId).Order().ToArray());
		if (row is Craft craft)
		{
			fields["Inputs"] = JsonSerializer.Serialize(craft.CraftInputs.OrderBy(x => x.OriginalAdditionTime)
				.Select(x => new Input(x.InputType, x.InputQualityWeight, x.Definition, NativeDateTime(x.OriginalAdditionTime))).ToArray());
			fields["Products"] = JsonSerializer.Serialize(craft.CraftProducts.OrderBy(x => x.IsFailProduct).ThenBy(x => x.OriginalAdditionTime)
				.Select(x => new Product(x.ProductType, x.Definition, x.IsFailProduct, x.MaterialDefiningInputIndex, NativeDateTime(x.OriginalAdditionTime))).ToArray());
			fields["Phases"] = JsonSerializer.Serialize(craft.CraftPhases.OrderBy(x => x.PhaseNumber)
				.Select(x => new Phase(x.PhaseNumber, x.PhaseLengthInSeconds, x.Echo, x.FailEcho, x.ExertionLevel, x.StaminaUsage)).ToArray());
		}
		AnatomyFields(row, fields);
		CreatureFields(row, fields);
		MedicalFields(row, fields);
		LandFields(row, fields);
		return fields;
	}

	private void ApplyChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
		ApplyAnatomyChildren(row, current, merged);
		ApplyCreatureChildren(row, current, merged);
		ApplyMedicalChildren(row, current, merged);
		ApplyLandChildren(row, current, merged);
		// Reconcile each native collection as an aggregate; builder edits are retained by the shared reconciler.
		if (row is CombatMessage message && current["WeaponAttacks"] != merged["WeaponAttacks"])
		{
			_db.RemoveRange(message.CombatMessagesWeaponAttacks); message.CombatMessagesWeaponAttacks.Clear();
			foreach (var id in JsonSerializer.Deserialize<long[]>(merged["WeaponAttacks"])!) message.CombatMessagesWeaponAttacks.Add(new() { WeaponAttackId = id });
		}
		if (row is GameItemProto item)
		{
			if (current["Components"] != merged["Components"])
			{
				_db.RemoveRange(item.GameItemProtosGameItemComponentProtos); item.GameItemProtosGameItemComponentProtos.Clear();
				foreach (var link in JsonSerializer.Deserialize<ComponentLink[]>(merged["Components"])!)
					item.GameItemProtosGameItemComponentProtos.Add(new() { GameItemComponentProtoId = link.Id, GameItemComponentRevision = link.Revision });
			}
			if (current["Tags"] != merged["Tags"])
			{
				_db.RemoveRange(item.GameItemProtosTags); item.GameItemProtosTags.Clear();
				foreach (var id in JsonSerializer.Deserialize<long[]>(merged["Tags"])!) item.GameItemProtosTags.Add(new() { TagId = id });
			}
		}
		if (row is FutureProg prog && current["Parameters"] != merged["Parameters"])
		{
			_db.RemoveRange(prog.FutureProgsParameters); prog.FutureProgsParameters.Clear();
			foreach (var parameter in JsonSerializer.Deserialize<Parameter[]>(merged["Parameters"])!) prog.FutureProgsParameters.Add(new()
			{ ParameterIndex = parameter.Index, ParameterName = parameter.Name, ParameterTypeDefinition = parameter.Type });
		}
		if (row is Craft craft)
		{
			if (current["Inputs"] != merged["Inputs"])
			{
				var inputs = JsonSerializer.Deserialize<Input[]>(merged["Inputs"])!;
				if (inputs.Length != craft.CraftInputs.Count || inputs.Any(x => !craft.CraftInputs.Any(y =>
					NativeDateTime(y.OriginalAdditionTime) == x.Added && y.InputType == x.Type)))
					throw new InvalidOperationException($"{craft.Name}: input layout/type change requires a native craft revision; retained active-craft input IDs cannot be replaced.");
				foreach (var input in inputs)
				{
					var rowInput = craft.CraftInputs.Single(x => NativeDateTime(x.OriginalAdditionTime) == input.Added);
					rowInput.InputQualityWeight = input.Quality; rowInput.Definition = input.Definition;
				}
			}
			if (current["Products"] != merged["Products"])
			{
				var products = JsonSerializer.Deserialize<Product[]>(merged["Products"])!;
				if (products.Length != craft.CraftProducts.Count || products.Any(x => !craft.CraftProducts.Any(y =>
					NativeDateTime(y.OriginalAdditionTime) == x.Added && y.ProductType == x.Type && y.IsFailProduct == x.Fail)))
					throw new InvalidOperationException($"{craft.Name}: product layout/type change requires a native craft revision; retained active-craft product IDs cannot be replaced.");
				foreach (var product in products)
				{
					var rowProduct = craft.CraftProducts.Single(x => NativeDateTime(x.OriginalAdditionTime) == product.Added && x.IsFailProduct == product.Fail);
					rowProduct.Definition = product.Definition; rowProduct.MaterialDefiningInputIndex = product.Material;
				}
			}
			if (current["Phases"] != merged["Phases"])
			{
				var phases = JsonSerializer.Deserialize<Phase[]>(merged["Phases"])!;
				if (phases.Length != craft.CraftPhases.Count || phases.Any(x => !craft.CraftPhases.Any(y => y.PhaseNumber == x.Number)))
					throw new InvalidOperationException($"{craft.Name}: phase layout change requires a native craft revision; retained active-craft phases cannot be replaced.");
				foreach (var phase in phases)
				{
					var rowPhase = craft.CraftPhases.Single(x => x.PhaseNumber == phase.Number);
					rowPhase.PhaseLengthInSeconds = phase.Seconds; rowPhase.Echo = phase.Echo; rowPhase.FailEcho = phase.FailEcho;
					rowPhase.ExertionLevel = phase.Exertion; rowPhase.StaminaUsage = phase.Stamina;
				}
			}
		}
	}

	private static DateTime NativeDateTime(DateTime value) => DateTime.SpecifyKind(value, DateTimeKind.Unspecified);

	private sealed record ComponentLink(long Id, int Revision);
	private sealed record Parameter(int Index, string Name, string Type);
	private sealed record Input(string Type, double Quality, string Definition, DateTime Added);
	private sealed record Product(string Type, string Definition, bool Fail, int? Material, DateTime Added);
	private sealed record Phase(int Number, double Seconds, string Echo, string FailEcho, int Exertion, double Stamina);
}
