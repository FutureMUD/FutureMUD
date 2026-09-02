#nullable enable

using MudSharp.GameItems;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudSharp.Documentation.Export;

internal sealed record IndustrialisedPrerequisiteAuditResult(
	int RuntimeComponentCount,
	int ModernComponentCount,
	int FuturisticComponentCount,
	int GeneralComponentCount,
	int SeededPrototypeCount,
	int MissingSameTypeStockCount,
	IReadOnlyList<string> ChangedFiles,
	IReadOnlyList<string> Errors);

internal static class IndustrialisedPrerequisiteAuditExporter
{
	private sealed record ComponentTypeExportRow(
		[property: JsonPropertyName("Component Type Name")] string Name,
		[property: JsonPropertyName("Component Type Description")] string Description,
		[property: JsonPropertyName("Technology")] string Technology,
		[property: JsonPropertyName("Builder Primary Type")] string? PrimaryBuilderType,
		[property: JsonPropertyName("Builder Aliases")] IReadOnlyList<string> BuilderAliases,
		[property: JsonPropertyName("Prototype Class")] string PrototypeClass,
		[property: JsonPropertyName("Component Capabilities")] IReadOnlyList<string> ComponentCapabilities,
		[property: JsonPropertyName("Exclusive Types")] IReadOnlyList<string> ExclusiveCapabilities,
		[property: JsonPropertyName("Required Sibling Types")] IReadOnlyList<string> RequiredSiblingCapabilities,
		[property: JsonPropertyName("Runtime Component Class")] string RuntimeComponentClass,
		[property: JsonPropertyName("Has Prototype XML Load")] bool HasPrototypeXmlLoad,
		[property: JsonPropertyName("Has Prototype XML Save")] bool HasPrototypeXmlSave,
		[property: JsonPropertyName("Has Create Path")] bool HasCreateNew,
		[property: JsonPropertyName("Has Component Load Path")] bool HasComponentLoad,
		[property: JsonPropertyName("Has Revision Copy Path")] bool HasRevisionCopy,
		[property: JsonPropertyName("Has Builder Command Path")] bool HasBuilderCommands,
		[property: JsonPropertyName("Has Runtime Copy Path")] bool HasRuntimeCopy,
		[property: JsonPropertyName("Has Context-Dependent Requirements")] bool HasContextDependentRequirements,
		[property: JsonPropertyName("Has Builder Loader")] bool HasBuilderLoader,
		[property: JsonPropertyName("Has Database Loader")] bool HasDatabaseLoader,
		[property: JsonPropertyName("Has Help")] bool HasHelp);

	private sealed record CuratedDisposition(string Disposition, string Owner, string Relevance, string Notes);
	private sealed record ResourceRequirement(string Kind, string Name, string Owner, string Disposition, string Consumer);

	private static readonly HashSet<string> SourceSeededExportStale = new(StringComparer.OrdinalIgnoreCase)
	{
	};

	private static readonly HashSet<string> SystemOrContextOwned = new(StringComparer.OrdinalIgnoreCase)
	{
		"ActiveCraft", "Bodypart", "Commodity", "Corpse", "CurrencyPile", "Dwelling", "Pile", "ProgLight",
		"ProgLock", "ProgPowerSupply", "Puddle", "StableTicket"
	};

	private static readonly HashSet<string> HonestAlternateAvailable = new(StringComparer.OrdinalIgnoreCase)
	{
		"Board", "CashRegister", "Changer", "Food", "Fuse", "Key", "Lock", "LocksmithingTools", "PowerPack",
		"Selectable", "Thrown", "Wieldable"
	};

	private static readonly HashSet<string> DependencyBound = new(StringComparer.OrdinalIgnoreCase)
	{
		"BiometricScanner", "BreathingFilter", "FaxMachine", "Photocopier", "Salvageable", "SignalDetonator",
		"VehicleAccessPoint", "VehicleCargoSpace", "VehicleExterior"
	};

	private static readonly HashSet<string> CombatOwned = new(StringComparer.OrdinalIgnoreCase)
	{
		"AmmoClip", "BoltAction", "ClockDetonator", "CountdownDetonator", "Flare", "FlareAmmunition",
		"PinPullDetonator", "RadioDetonator", "RadioDetonatorTransmitter", "SignalDetonator"
	};

	private static readonly IReadOnlyList<ResourceRequirement> ResourceRequirements =
	[
		new("Material", "ABS plastic", "CoreDataSeeder", "reuse", "appliance and equipment housings"),
		new("Material", "aluminium", "CoreDataSeeder", "reuse", "light equipment housings and heat exchangers"),
		new("Material", "cardboard", "CoreDataSeeder", "reuse", "packaging and office consumables"),
		new("Material", "concrete", "CoreDataSeeder", "reuse", "industrial and civic fixtures"),
		new("Material", "copper", "CoreDataSeeder", "reuse", "electrical conductors and windings"),
		new("Material", "epoxy resin", "CoreDataSeeder", "reuse", "electrical and composite assembly"),
		new("Material", "fiberglass", "CoreDataSeeder", "reuse", "equipment shells and insulation"),
		new("Material", "mild steel", "CoreDataSeeder", "reuse", "general machinery"),
		new("Material", "natural rubber", "CoreDataSeeder", "reuse", "seals hoses and insulation"),
		new("Material", "silicon", "CoreDataSeeder", "reuse", "electronic component substrate"),
		new("Material", "silicone rubber", "CoreDataSeeder", "reuse", "electrical seals and flexible insulation"),
		new("Material", "stainless steel", "CoreDataSeeder", "reuse", "food medical and appliance equipment"),
		new("Material", "tool steel", "CoreDataSeeder", "reuse", "powered tool working surfaces"),
		new("Liquid", "detergent", "CoreDataSeeder", "reuse", "washing machinery"),
		new("Liquid", "diesel", "CoreDataSeeder", "reuse", "combustion engines and generators"),
		new("Liquid", "gasoline", "CoreDataSeeder", "reuse", "combustion engines and portable machinery"),
		new("Liquid", "hydraulic fluid", "CoreDataSeeder", "reuse", "industrial machinery"),
		new("Liquid", "kerosene", "CoreDataSeeder", "reuse", "heating lighting and engines"),
		new("Liquid", "machine oil", "CoreDataSeeder", "reuse", "machinery maintenance"),
		new("Gas", "Acetylene", "CoreDataSeeder", "reuse", "welding and cutting equipment"),
		new("Gas", "Argon", "CoreDataSeeder", "reuse", "shielding gas and industrial processes"),
		new("Gas", "Butane", "CoreDataSeeder", "reuse", "portable fuel stock"),
		new("Gas", "Carbon Monoxide", "CoreDataSeeder", "reuse", "hazard modelling"),
		new("Gas", "Natural Gas", "CoreDataSeeder", "reuse", "domestic and industrial fuel"),
		new("Gas", "Nitrogen", "CoreDataSeeder", "reuse", "compressed gas and inerting"),
		new("Gas", "Oxygen", "CoreDataSeeder", "reuse", "medical and industrial gas stock"),
		new("Gas", "Propane", "CoreDataSeeder", "reuse", "portable heating and machinery"),
		new("Gas", "Refrigerant R-134a", "CoreDataSeeder", "reuse", "refrigeration equipment"),
		new("Tag", "Era / Industrial Era", "UsefulSeeder", "reuse", "Industrial-era catalogue admission"),
		new("Tag", "Era / Modern Era", "UsefulSeeder", "reuse", "Modern-era catalogue admission"),
		new("Tag", "Era / Nuclear Era", "UsefulSeeder", "reuse", "Nuclear-era catalogue admission"),
		new("Tag", "Era / Information Age Era", "UsefulSeeder", "reuse", "Information-age catalogue admission"),
		new("Tag", "Functions / Tools", "UsefulSeeder", "reuse", "tool classification"),
		new("Tag", "Market / Communications", "UsefulSeeder", "reuse", "communications equipment"),
		new("Tag", "Market / Household Goods", "UsefulSeeder", "reuse", "domestic appliances"),
		new("Tag", "Market / Professional Tools", "UsefulSeeder", "reuse", "trade and office equipment"),
		new("Tag", "Market / Repair Supplies", "UsefulSeeder", "reuse", "maintenance stock"),
		new("Tag", "Market / Transportation", "UsefulSeeder", "reuse", "vehicle and transport support"),
		new("Tag", "Market / Warehousing", "UsefulSeeder", "reuse", "storage and logistics equipment")
	];

	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		WriteIndented = true
	};

	public static IndustrialisedPrerequisiteAuditResult Run(string repositoryRoot, bool checkOnly)
	{
		var root = Path.GetFullPath(repositoryRoot);
		var dataDirectory = Path.Combine(root, "Design Documents", "Data");
		var seedingDirectory = Path.Combine(root, "Design Documents", "Seeding");
		var componentTypesPath = Path.Combine(dataDirectory, "Item_Component_Types.json");
		var seededComponentsPath = Path.Combine(dataDirectory, "Seeded_Item_Components.json");
		var componentAuditPath = Path.Combine(seedingDirectory, "Industrialised_Component_Prerequisite_Audit.tsv");
		var resourceAuditPath = Path.Combine(seedingDirectory, "Industrialised_Resource_Prerequisite_Audit.tsv");

		var errors = new List<string>();
		var changedFiles = new List<string>();
		if (!File.Exists(seededComponentsPath))
		{
			return new IndustrialisedPrerequisiteAuditResult(0, 0, 0, 0, 0, 0, [],
				[$"Missing seeded component export: {seededComponentsPath}"]);
		}

		var manager = new GameItemComponentManager();
		var registrations = manager.RegistrationAuditEntries;
		ValidateRegistrations(registrations, errors);
		var seededCounts = ReadSeededComponentCounts(seededComponentsPath, out var seededPrototypeCount);
		var curated = ReadCuratedDispositions(componentAuditPath);
		var componentJson = BuildComponentTypesJson(registrations);
		var componentTsv = BuildComponentAuditTsv(registrations, seededCounts, curated);
		if (componentTsv.Contains("\treusable-stock-required\t", StringComparison.Ordinal))
		{
			errors.Add("Stage 1 is not closed: one or more component registrations still require reusable stock.");
		}
		var resourceTsv = BuildResourceAuditTsv(dataDirectory, errors);

		ReconcileFile(componentTypesPath, componentJson, checkOnly, changedFiles, errors, root);
		ReconcileFile(componentAuditPath, componentTsv, checkOnly, changedFiles, errors, root);
		ReconcileFile(resourceAuditPath, resourceTsv, checkOnly, changedFiles, errors, root);

		var modernCount = registrations.Count(x => x.Technology.HasFlag(GameItemComponentTypeTechnology.Modern));
		var futuristicCount = registrations.Count(x => x.Technology.HasFlag(GameItemComponentTypeTechnology.Futuristic));
		var generalCount = registrations.Count - modernCount - futuristicCount;
		var missingCount = registrations.Count(x => !seededCounts.ContainsKey(NormaliseType(x.CanonicalDatabaseType)));

		return new IndustrialisedPrerequisiteAuditResult(
			registrations.Count,
			modernCount,
			futuristicCount,
			generalCount,
			seededPrototypeCount,
			missingCount,
			changedFiles,
			errors);
	}

	private static void ValidateRegistrations(
		IReadOnlyList<GameItemComponentRegistrationAuditEntry> registrations,
		ICollection<string> errors)
	{
		foreach (var duplicate in registrations
		         .GroupBy(x => x.CanonicalDatabaseType, StringComparer.OrdinalIgnoreCase)
		         .Where(x => x.Count() > 1))
		{
			errors.Add($"Duplicate canonical database component type: {duplicate.Key}");
		}

		foreach (var registration in registrations)
		{
			if (!registration.HasDatabaseLoader)
			{
				errors.Add($"{registration.CanonicalDatabaseType} has no database loader.");
			}

			if (!registration.HasHelp)
			{
				errors.Add($"{registration.CanonicalDatabaseType} has no component help registration.");
			}

			if (string.IsNullOrWhiteSpace(registration.RuntimeComponentClass))
			{
				errors.Add($"{registration.CanonicalDatabaseType} has no matching runtime component class.");
			}

			if (!registration.HasPrototypeXmlLoad || !registration.HasPrototypeXmlSave)
			{
				errors.Add($"{registration.CanonicalDatabaseType} does not expose both prototype XML load and save paths.");
			}

			if (!registration.HasCreateNew || !registration.HasComponentLoad || !registration.HasRevisionCopy)
			{
				errors.Add($"{registration.CanonicalDatabaseType} does not expose all create, load and revision-copy paths.");
			}

			if (!registration.HasBuilderCommands)
			{
				errors.Add($"{registration.CanonicalDatabaseType} has no builder-command path.");
			}

			if (!registration.HasRuntimeCopy)
			{
				errors.Add($"{registration.CanonicalDatabaseType} has no runtime component copy path.");
			}
		}
	}

	private static Dictionary<string, int> ReadSeededComponentCounts(string path, out int total)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(path));
		var rows = document.RootElement.EnumerateArray().ToList();
		total = rows.Count;
		return rows
		       .Select(x => x.GetProperty("Component Type").GetString() ?? string.Empty)
		       .GroupBy(NormaliseType, StringComparer.Ordinal)
		       .ToDictionary(x => x.Key, x => x.Count(), StringComparer.Ordinal);
	}

	private static string BuildComponentTypesJson(
		IReadOnlyList<GameItemComponentRegistrationAuditEntry> registrations)
	{
		var rows = registrations.Select(x => new ComponentTypeExportRow(
			x.CanonicalDatabaseType,
			x.Description,
			DescribeTechnology(x.Technology),
			x.PrimaryBuilderType,
			x.BuilderAliases,
			x.PrototypeClass,
			x.ComponentCapabilities,
			x.ExclusiveCapabilities,
			x.RequiredSiblingCapabilities,
			x.RuntimeComponentClass,
			x.HasPrototypeXmlLoad,
			x.HasPrototypeXmlSave,
			x.HasCreateNew,
			x.HasComponentLoad,
			x.HasRevisionCopy,
			x.HasBuilderCommands,
			x.HasRuntimeCopy,
			x.HasContextDependentRequirements,
			x.HasBuilderLoader,
			x.HasDatabaseLoader,
			x.HasHelp)).ToList();
		return JsonSerializer.Serialize(rows, JsonOptions) + Environment.NewLine;
	}

	private static string BuildComponentAuditTsv(
		IReadOnlyList<GameItemComponentRegistrationAuditEntry> registrations,
		IReadOnlyDictionary<string, int> seededCounts,
		IReadOnlyDictionary<string, CuratedDisposition> curated)
	{
		var lines = new List<string>
		{
			"Canonical Type\tTechnology\tBuilder Primary\tBuilder Aliases\tDatabase Loader\tPrototype Class\tExclusive Capabilities\tRequired Sibling Capabilities\tSeeded Prototype Count\tSeeder Owner\tIndustrialised Relevance\tDisposition\tNotes\tRuntime Component Class\tPrototype XML Load\tPrototype XML Save\tCreate Path\tComponent Load Path\tRevision Copy Path\tBuilder Command Path\tRuntime Copy Path"
		};
		foreach (var registration in registrations)
		{
			var normalized = NormaliseType(registration.CanonicalDatabaseType);
			seededCounts.TryGetValue(normalized, out var seededCount);
			var defaults = DefaultDisposition(registration, seededCount);
			var preserved = curated.GetValueOrDefault(registration.CanonicalDatabaseType, defaults);
			var disposition = preserved.Disposition.StartsWith("manual:", StringComparison.OrdinalIgnoreCase)
				? preserved
				: defaults;
			lines.Add(string.Join('\t',
				Tsv(registration.CanonicalDatabaseType),
				Tsv(DescribeTechnology(registration.Technology)),
				Tsv(registration.PrimaryBuilderType ?? string.Empty),
				Tsv(string.Join("; ", registration.BuilderAliases)),
				"yes",
				Tsv(registration.PrototypeClass),
				Tsv(string.Join("; ", registration.ExclusiveCapabilities)),
				Tsv(string.Join("; ", registration.RequiredSiblingCapabilities) +
				    (registration.HasContextDependentRequirements ? "[context-dependent]" : string.Empty)),
				seededCount.ToString(),
				Tsv(disposition.Owner),
				Tsv(disposition.Relevance),
				Tsv(disposition.Disposition),
				Tsv(disposition.Notes),
				Tsv(registration.RuntimeComponentClass),
				YesNo(registration.HasPrototypeXmlLoad),
				YesNo(registration.HasPrototypeXmlSave),
				YesNo(registration.HasCreateNew),
				YesNo(registration.HasComponentLoad),
				YesNo(registration.HasRevisionCopy),
				YesNo(registration.HasBuilderCommands),
				YesNo(registration.HasRuntimeCopy)));
		}

		return string.Join(Environment.NewLine, lines) + Environment.NewLine;
	}

	private static string BuildResourceAuditTsv(string dataDirectory, ICollection<string> errors)
	{
		var materialNames = ReadExportNames(Path.Combine(dataDirectory, "Seeded_Materials.json"), "Material Name");
		var liquidNames = ReadExportNames(Path.Combine(dataDirectory, "Seeded_Liquids.json"), "Liquid Name");
		var gasNames = ReadExportNames(Path.Combine(dataDirectory, "Seeded_Gases.json"), "Gas Name");
		var tagNames = File.ReadLines(Path.Combine(dataDirectory, "SeededTagHierarchy.csv"))
		                   .Skip(1)
		                   .Select(x => x.Split('\t'))
		                   .Where(x => x.Length >= 3)
		                   .Select(x => x[2])
		                   .ToHashSet(StringComparer.OrdinalIgnoreCase);
		var catalogues = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
		{
			["Material"] = materialNames,
			["Liquid"] = liquidNames,
			["Gas"] = gasNames,
			["Tag"] = tagNames
		};
		var lines = new List<string>
		{
			"Kind\tCanonical Name or Path\tExport Match\tOwner\tDisposition\tIdentified Consumer"
		};
		foreach (var requirement in ResourceRequirements.OrderBy(x => x.Kind).ThenBy(x => x.Name))
		{
			var exists = catalogues[requirement.Kind].Contains(requirement.Name);
			if (!exists)
			{
				errors.Add($"Missing {requirement.Kind.ToLowerInvariant()} prerequisite: {requirement.Name}");
			}
			lines.Add(string.Join('\t', Tsv(requirement.Kind), Tsv(requirement.Name), exists ? "yes" : "no",
				Tsv(requirement.Owner), Tsv(requirement.Disposition), Tsv(requirement.Consumer)));
		}

		return string.Join(Environment.NewLine, lines) + Environment.NewLine;
	}

	private static HashSet<string> ReadExportNames(string path, string propertyName)
	{
		using var document = JsonDocument.Parse(File.ReadAllText(path));
		return document.RootElement.EnumerateArray()
		               .Select(x => x.GetProperty(propertyName).GetString() ?? string.Empty)
		               .ToHashSet(StringComparer.OrdinalIgnoreCase);
	}

	private static IReadOnlyDictionary<string, CuratedDisposition> ReadCuratedDispositions(string path)
	{
		if (!File.Exists(path))
		{
			return new Dictionary<string, CuratedDisposition>(StringComparer.OrdinalIgnoreCase);
		}

		var rows = File.ReadLines(path).Skip(1)
		               .Select(x => x.Split('\t'))
		               .Where(x => x.Length >= 13)
		               .ToDictionary(
			               x => x[0],
			               x => new CuratedDisposition(x[11], x[9], x[10], x[12]),
			               StringComparer.OrdinalIgnoreCase);
		return rows;
	}

	private static CuratedDisposition DefaultDisposition(
		GameItemComponentRegistrationAuditEntry registration,
		int seededCount)
	{
		if (seededCount > 0)
		{
			return new CuratedDisposition("exported-and-current", DefaultOwner(registration.CanonicalDatabaseType),
				DefaultRelevance(registration), "Reusable stock is present in the maintained component export.");
		}

		if (IsFutureSpecialist(registration))
		{
			return new CuratedDisposition("future-deferred", DefaultOwner(registration.CanonicalDatabaseType),
				"Futuristic", "Inventoried for completeness; no speculative later-era stock in this tranche.");
		}

		if (ContainsType(SourceSeededExportStale, registration.CanonicalDatabaseType))
		{
			return new CuratedDisposition("source-seeded-export-stale", DefaultOwner(registration.CanonicalDatabaseType),
				DefaultRelevance(registration), "Live seeder source creates reusable stock that is absent from the export.");
		}

		if (ContainsType(SystemOrContextOwned, registration.CanonicalDatabaseType))
		{
			return new CuratedDisposition("system-or-context-owned", "Runtime", DefaultRelevance(registration),
				"Do not expose as ordinary reusable catalogue stock.");
		}

		if (ContainsType(HonestAlternateAvailable, registration.CanonicalDatabaseType))
		{
			return new CuratedDisposition("honest-alternate-satisfied", DefaultOwner(registration.CanonicalDatabaseType),
				DefaultRelevance(registration), "Existing reusable profiles cover the near-term semantic requirement.");
		}

		if (ContainsType(DependencyBound, registration.CanonicalDatabaseType))
		{
			return new CuratedDisposition("dependency-bound", DefaultOwner(registration.CanonicalDatabaseType),
				DefaultRelevance(registration), DependencyReason(registration.CanonicalDatabaseType));
		}

		return new CuratedDisposition("reusable-stock-required", DefaultOwner(registration.CanonicalDatabaseType),
			DefaultRelevance(registration), "Runtime support exists but no same-type reusable stock is exported.");
	}

	private static string DependencyReason(string canonicalType)
	{
		return NormaliseType(canonicalType) switch
		{
			"biometricscanner" => "Requires a selected world anatomy shape; UsefulSeeder must not guess one when run independently.",
			"salvageable" => "Requires concrete material or item outputs owned by the finished item; a generic empty profile would not be useful.",
			"signaldetonator" => "Requires a concrete signal-source component and endpoint; seed it with the finished explosive or automation graph.",
			_ => "Requires finished-item or world-specific references; do not seed placeholder IDs."
		};
	}

	private static string DefaultOwner(string canonicalType)
	{
		if (canonicalType.StartsWith("Vehicle ", StringComparison.OrdinalIgnoreCase))
		{
			return "VehicleSeeder";
		}

		return ContainsType(CombatOwned, canonicalType) ? "CombatSeeder" : "UsefulSeeder";
	}

	private static bool ContainsType(IEnumerable<string> values, string canonicalType)
	{
		var normalized = NormaliseType(canonicalType);
		return values.Any(x => NormaliseType(x) == normalized);
	}

	private static string DefaultRelevance(GameItemComponentRegistrationAuditEntry registration)
	{
		if (IsFutureSpecialist(registration))
		{
			return "Futuristic";
		}

		return registration.Technology.HasFlag(GameItemComponentTypeTechnology.Modern)
			? "Industrialised-direct"
			: "General-shared";
	}

	private static bool IsFutureSpecialist(GameItemComponentRegistrationAuditEntry registration)
	{
		return registration.Technology.HasFlag(GameItemComponentTypeTechnology.Futuristic) ||
		       registration.CanonicalDatabaseType.Contains("Implant", StringComparison.OrdinalIgnoreCase) ||
		       registration.CanonicalDatabaseType.Equals("Laser", StringComparison.OrdinalIgnoreCase) ||
		       registration.CanonicalDatabaseType.Equals("NeuralInterface", StringComparison.OrdinalIgnoreCase) ||
		       registration.CanonicalDatabaseType.Equals("PowerPack", StringComparison.OrdinalIgnoreCase);
	}

	private static string DescribeTechnology(GameItemComponentTypeTechnology technology)
	{
		if (technology.HasFlag(GameItemComponentTypeTechnology.Futuristic))
		{
			return "Futuristic";
		}

		return technology.HasFlag(GameItemComponentTypeTechnology.Modern) ? "Modern" : "General";
	}

	private static string YesNo(bool value) => value ? "yes" : "no";

	private static void ReconcileFile(
		string path,
		string expected,
		bool checkOnly,
		ICollection<string> changedFiles,
		ICollection<string> errors,
		string root)
	{
		var current = File.Exists(path) ? File.ReadAllText(path) : null;
		if (current is not null && NormaliseText(current) == NormaliseText(expected))
		{
			return;
		}

		var relative = Path.GetRelativePath(root, path);
		if (checkOnly)
		{
			errors.Add($"Audit artifact is stale: {relative}");
			return;
		}

		Directory.CreateDirectory(Path.GetDirectoryName(path)!);
		File.WriteAllText(path, expected, new UTF8Encoding(true));
		changedFiles.Add(relative);
	}

	private static string NormaliseText(string text)
	{
		return text.TrimStart('\uFEFF').Replace("\r\n", "\n").TrimEnd() + "\n";
	}

	private static string NormaliseType(string text)
	{
		return new string(text.Where(char.IsLetterOrDigit).Select(char.ToLowerInvariant).ToArray());
	}

	private static string Tsv(string text)
	{
		return text.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
	}
}
