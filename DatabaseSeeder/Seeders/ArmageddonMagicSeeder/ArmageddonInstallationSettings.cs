#nullable enable
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using MudSharp.Database;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public enum ArmageddonProgressionMode { Classic, Simple }
public enum ArmageddonNativeBinding
{
	SkillDecorator, SkillImprover, AlwaysTrueProg, AlwaysFalseProg, HumanRace, Culture, Language,
	Wood, Stone, Water, Kilogram, Metre, Litre, BuilderAccount
}

public sealed record ArmageddonInstallationSettings(
	[property: JsonRequired] int Format,
	[property: JsonRequired] ArmageddonProgressionMode Mode,
	[property: JsonRequired] bool ClassesEnabled,
	[property: JsonRequired] long CapacityAttribute,
	[property: JsonRequired] IReadOnlyDictionary<ArmageddonNativeBinding, long> NativeBindings);

/// <summary>One immutable direction for one MUD. No entity or player-state writes.</summary>
public static partial class ArmageddonInstallation
{
	public const string SettingsKey = "arm.package.installation";
	public const string Module = "installation-settings";
	public const string EntityType = "ArmageddonInstallationSettings";
	public const string Version = "stock-1";
	public const string ModeAnswerKey = "armageddon-progression-mode";
	public const string ClassesAnswerKey = "armageddon-classes-enabled-at-install";
	public const string BindingsAnswerKey = "armageddon-fixed-native-bindings";
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		Converters = { new JsonStringEnumConverter<ArmageddonProgressionMode>(allowIntegerValues: false),
			new JsonStringEnumConverter<ArmageddonNativeBinding>(allowIntegerValues: false) }
	};

	public static string Serialize(ArmageddonInstallationSettings settings) => JsonSerializer.Serialize(settings, JsonOptions);
	public static string SerializeOverrides(IReadOnlyDictionary<ArmageddonNativeBinding, long> overrides) => JsonSerializer.Serialize(overrides, JsonOptions);
	public static IReadOnlyDictionary<ArmageddonNativeBinding, long> ParseOverrides(string json)
	{
		using var document = StrictDocument(json);
		if (document.RootElement.ValueKind != JsonValueKind.Object || document.RootElement.EnumerateObject().Any(x => !Enum.GetNames<ArmageddonNativeBinding>().Contains(x.Name, StringComparer.Ordinal)))
			throw new FormatException("Native bindings must be an object with declared binding names and positive native IDs.");
		var result = JsonSerializer.Deserialize<Dictionary<ArmageddonNativeBinding, long>>(json, JsonOptions)!;
		if (result.Values.Any(x => x <= 0)) throw new FormatException("Native binding IDs must be positive.");
		return result;
	}

	private static JsonDocument StrictDocument(string json)
	{
		if (json.Length > 32768) throw new FormatException("Installation settings exceed 32768 characters.");
		var document = JsonDocument.Parse(json);
		try
		{
			void Visit(JsonElement element)
			{
				if (element.ValueKind == JsonValueKind.Object)
				{
					var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (var property in element.EnumerateObject())
					{
						if (!names.Add(property.Name)) throw new FormatException($"Duplicate settings property {property.Name}.");
						Visit(property.Value);
					}
				}
				else if (element.ValueKind == JsonValueKind.Array) foreach (var elementChild in element.EnumerateArray()) Visit(elementChild);
			}
			Visit(document.RootElement);
			return document;
		}
		catch { document.Dispose(); throw; }
	}

	private static string Fingerprint(string json) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(json))).ToLowerInvariant();

	public static ArmageddonInstallationSettings? Read(FuturemudDatabaseContext db)
	{
		var records = db.SeederManagedRecords.AsNoTracking().Where(x => x.StableKey == SettingsKey ||
			(x.Seeder == ArmageddonMagicInstaller.Package && (x.Module == Module || x.EntityType == EntityType))).ToArray();
		if (records.Length == 0)
		{
			if (db.SeederChoices.AsNoTracking().Any(x => x.Seeder == "__shared__" &&
				(x.Choice == ModeAnswerKey || x.Choice == ClassesAnswerKey || x.Choice == BindingsAnswerKey)))
				throw new InvalidOperationException("Armageddon settings record is missing but fixed-answer mirrors exist. Restore the owned settings; answers cannot reconstruct it.");
			return null;
		}
		if (records.Length != 1) throw new InvalidOperationException("Armageddon installation settings ownership is ambiguous.");
		var record = records[0];
		if (record.Seeder != ArmageddonMagicInstaller.Package || record.Module != Module || record.EntityType != EntityType || record.StableKey != SettingsKey || record.Retired || record.LogicalId is not null || record.RevisionNumber is not null || record.SeedBaseline is null || record.ManifestVersion != Version || Fingerprint(record.SeedBaseline) != record.AppliedFingerprint)
			throw new InvalidOperationException("Armageddon installation settings are corrupt, retired or have incompatible ownership. Preserve and restore them explicitly.");
		using var document = StrictDocument(record.SeedBaseline);
		var settings = JsonSerializer.Deserialize<ArmageddonInstallationSettings>(record.SeedBaseline, JsonOptions)
			?? throw new InvalidOperationException("Armageddon settings cannot be null.");
		if (settings.Format != 1 || !Enum.IsDefined(settings.Mode) || settings.NativeBindings is null || settings.CapacityAttribute <= 0 || settings.NativeBindings.Count != Enum.GetValues<ArmageddonNativeBinding>().Length || settings.NativeBindings.Any(x => !Enum.IsDefined(x.Key) || x.Value <= 0))
			throw new InvalidOperationException("Armageddon fixed settings have invalid fields.");
		foreach (var pair in Mirrors(settings))
		{
			var mirrors = db.SeederChoices.AsNoTracking().Where(x => x.Seeder == "__shared__" && x.Choice == pair.Key).ToArray();
			if (mirrors.Length != 1 || mirrors[0].Answer != pair.Value)
				throw new InvalidOperationException($"Armageddon fixed answer {pair.Key} disagrees with the owned settings record.");
		}
		return settings;
	}

	private static IReadOnlyDictionary<string, string> Mirrors(ArmageddonInstallationSettings settings) =>
		new Dictionary<string, string>
		{
			[ModeAnswerKey] = settings.Mode.ToString(), [ClassesAnswerKey] = settings.ClassesEnabled ? "yes" : "no",
			[BindingsAnswerKey] = Serialize(settings)
		};

	public static ArmageddonInstallationSettings Prepare(Func<FuturemudDatabaseContext> contextFactory,
		ArmageddonProgressionMode? requestedMode = null, long? capacityAttribute = null,
		IReadOnlyDictionary<ArmageddonNativeBinding, long>? nativeOverrides = null,
		Action<ArmageddonInstallCheckpoint>? checkpoint = null)
	{
		using var db = contextFactory();
		if (db.ChangeTracker.HasChanges() || db.Database.CurrentTransaction is not null)
			throw new InvalidOperationException("Installation settings require a fresh, clean context.");
		using var transaction = db.Database.IsRelational() ? db.Database.BeginTransaction(IsolationLevel.Serializable) : null;
		var prior = Read(db);
		var classes = ClassesEnabled(db);
		var settings = prior ?? new ArmageddonInstallationSettings(1, requestedMode ?? ArmageddonProgressionMode.Classic, classes,
			capacityAttribute ?? ResolveCapacityAttribute(db), ResolveNativeBindings(db, nativeOverrides));
		if (!Enum.IsDefined(settings.Mode) || settings.ClassesEnabled != classes ||
			(requestedMode is { } mode && mode != settings.Mode) || (capacityAttribute is { } attribute && attribute != settings.CapacityAttribute) ||
			(nativeOverrides is not null && nativeOverrides.Any(x => !settings.NativeBindings.TryGetValue(x.Key, out var id) || id != x.Value)))
			throw new InvalidOperationException("Armageddon mode, class direction and native bindings are fixed for this MUD. A contradictory request cannot change them.");
		var errors = ValidateFoundations(db, settings).Concat(ArmageddonStockManifest.Validate(ArmageddonStockManifest.Read())).Concat(ValidateLegacyOwnership(db)).ToArray();
		if (errors.Length > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
		checkpoint?.Invoke(ArmageddonInstallCheckpoint.PreflightComplete);
		if (prior is not null) return prior;
		var json = Serialize(settings);
		db.SeederManagedRecords.Add(new SeederManagedRecord
		{
			Seeder = ArmageddonMagicInstaller.Package, Module = Module, EntityType = EntityType, StableKey = SettingsKey,
			SeedBaseline = json, AppliedFingerprint = Fingerprint(json), ManifestVersion = Version, AppliedAt = DateTime.UtcNow
		});
		foreach (var pair in Mirrors(settings)) db.SeederChoices.Add(new SeederChoice
		{
			Seeder = "__shared__", Choice = pair.Key, Answer = pair.Value, DateTime = DateTime.UtcNow, Version = Version
		});
		checkpoint?.Invoke(ArmageddonInstallCheckpoint.ContentCreated);
		db.SaveChanges();
		checkpoint?.Invoke(ArmageddonInstallCheckpoint.BeforeCommit);
		transaction?.Commit();
		checkpoint?.Invoke(ArmageddonInstallCheckpoint.AfterCommit);
		return settings;
	}
}
