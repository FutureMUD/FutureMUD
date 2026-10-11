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
internal sealed class ArmageddonCatalogueOwnership
{
	internal const string Module = "stock-catalogue";
	private const string Version = "stock-catalogue-1";
	private static readonly IReadOnlyDictionary<Type, string[]> OwnedFields = new Dictionary<Type, string[]>
	{
		[typeof(Tag)] = ["Name", "ParentId", "ShouldSeeProgId"],
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
	};
	private readonly FuturemudDatabaseContext _db;
	private readonly Dictionary<string, SeederManagedRecord> _records;
	private readonly List<string> _messages;
	private readonly long _builder;

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
		var nativeMaximum = _db.Set<T>().Select(x => (long?)EF.Property<long>(x, "Id")).Max() ?? 0;
		var type = typeof(T).Name;
		var claimedMaximum = _db.SeederManagedRecords.Where(x => x.EntityType == type).Select(x => x.LogicalId).Max() ?? 0;
		return checked(Math.Max(nativeMaximum, claimedMaximum) + 1);
	}

	internal EditableItem Approval() => new()
	{
		BuilderAccountId = _builder, ReviewerAccountId = _builder, BuilderDate = DateTime.UtcNow, ReviewerDate = DateTime.UtcNow,
		RevisionStatus = (int)RevisionStatus.Current, BuilderComment = "Armageddon stock catalogue.", ReviewerComment = "Typed native dependencies; full magic activation remains gated."
	};

	internal T Put<T>(string key, T desired, IReadOnlyList<string> dependencies) where T : class
	{
		if (!OwnedFields.ContainsKey(typeof(T))) throw new InvalidOperationException($"Undeclared catalogue entity type {typeof(T).Name}.");
		var next = Fields(desired);
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
				if (_db.Set<T>().Any(x => EF.Property<string>(x, nameField) == name))
					throw new InvalidOperationException($"{key}: unowned native name collision; no adoption.");
			}
			row = desired;
			_db.Add(row);
			_db.SaveChanges();
			var id = (long)_db.Entry(row).Property("Id").CurrentValue!;
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
		_db.SaveChanges();
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
		return fields;
	}

	private void ApplyChildren(object row, IReadOnlyDictionary<string, string> current, IReadOnlyDictionary<string, string> merged)
	{
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
