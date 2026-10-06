#nullable enable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudSharp.Construction.ImportExport;

public sealed class SpatialAreaPackageReadResult
{
	public int SourceVersion { get; init; } = SpatialAreaPackage.CurrentVersion;
	public string? SourceIntegritySha256 { get; init; }
	public SpatialAreaPackage? Package { get; init; }
	public IReadOnlyList<SpatialAreaTransferDiagnostic> Diagnostics { get; init; } = [];
	public bool Success => Package is not null &&
	                       Diagnostics.All(x => x.Severity != SpatialAreaTransferDiagnosticSeverity.Error);
}

public static partial class SpatialAreaPackageSerializer
{
	public const long MaximumPackageBytes = 16L * 1024L * 1024L;
	public const int MaximumCells = 20_000;
	public const int MaximumExits = 50_000;

	private static readonly JsonSerializerOptions Options = new()
	{
		AllowTrailingCommas = false,
		DefaultIgnoreCondition = JsonIgnoreCondition.Never,
		PropertyNameCaseInsensitive = false,
		ReadCommentHandling = JsonCommentHandling.Disallow,
		UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
		WriteIndented = true
	};

	public static string Serialize(SpatialAreaPackage package)
	{
		if (package.Version != SpatialAreaPackage.CurrentVersion)
			throw new ArgumentException("Only direct-cell version 4 packages can be written.", nameof(package));
		package.IntegritySha256 = string.Empty;
		var canonicalJson = SerializeForVersion(package);
		package.IntegritySha256 = ComputeHash(canonicalJson);
		return SerializeForVersion(package);
	}

	public static SpatialAreaPackageReadResult Deserialize(string json)
	{
		var diagnostics = new List<SpatialAreaTransferDiagnostic>();
		if (Encoding.UTF8.GetByteCount(json) > MaximumPackageBytes)
		{
			diagnostics.Add(Error("package-too-large",
				$"The package exceeds the {MaximumPackageBytes:N0}-byte safety limit."));
			return new SpatialAreaPackageReadResult { Diagnostics = diagnostics };
		}

		try
		{
			using var document = JsonDocument.Parse(json);
			if (document.RootElement.ValueKind != JsonValueKind.Object ||
			    !document.RootElement.TryGetProperty("Version", out var versionElement) ||
			    versionElement.ValueKind != JsonValueKind.Number ||
			    !versionElement.TryGetInt32(out var version))
				return new SpatialAreaPackageReadResult { Diagnostics = [Error("unsupported-version", "A numeric package version is required.")] };
			if (version is >= 1 and <= 3) return ReadLegacy(json);
			if (version != SpatialAreaPackage.CurrentVersion)
				return new SpatialAreaPackageReadResult { Diagnostics = [Error("unsupported-version", $"Package version {version} is not supported.")] };
		}
		catch (JsonException ex)
		{
			return new SpatialAreaPackageReadResult { Diagnostics = [Error("invalid-json", ex.Message)] };
		}

		SpatialAreaPackage? package;
		try
		{
			package = JsonSerializer.Deserialize<SpatialAreaPackage>(json, Options);
		}
		catch (JsonException ex)
		{
			diagnostics.Add(Error("invalid-json", $"The package is not valid versioned JSON: {ex.Message}"));
			return new SpatialAreaPackageReadResult { Diagnostics = diagnostics };
		}

		if (package is null)
		{
			diagnostics.Add(Error("empty-package", "The package did not contain an object."));
			return new SpatialAreaPackageReadResult { Diagnostics = diagnostics };
		}

		var suppliedHash = package.IntegritySha256;
		package.IntegritySha256 = string.Empty;
		var expectedHash = ComputeHash(SerializeForVersion(package));
		package.IntegritySha256 = suppliedHash;
		if (string.IsNullOrWhiteSpace(suppliedHash) ||
		    !CryptographicOperations.FixedTimeEquals(
			    Encoding.ASCII.GetBytes(suppliedHash.ToLowerInvariant()),
			    Encoding.ASCII.GetBytes(expectedHash)))
		{
			diagnostics.Add(Error("integrity-failed",
				"The package SHA-256 integrity value does not match its contents."));
		}

		diagnostics.AddRange(Validate(package));
		return new SpatialAreaPackageReadResult
		{
			Package = diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error)
				? null
				: package,
			Diagnostics = diagnostics
		};
	}

	public static IReadOnlyList<SpatialAreaTransferDiagnostic> Validate(SpatialAreaPackage package)
	{
		var diagnostics = new List<SpatialAreaTransferDiagnostic>();
		if (!string.Equals(package.Format, SpatialAreaPackage.CurrentFormat, StringComparison.Ordinal))
		{
			diagnostics.Add(Error("unsupported-format",
				$"Expected format '{SpatialAreaPackage.CurrentFormat}', but found '{package.Format}'."));
		}

		if (package.Version != SpatialAreaPackage.CurrentVersion)
		{
			diagnostics.Add(Error("unsupported-version",
				$"Package version {package.Version:N0} is not supported; normalised packages must use version {SpatialAreaPackage.CurrentVersion:N0}."));
		}

		if (package.Cells is null || package.Exits is null ||
		    package.Source is null || package.Omissions is null)
		{
			diagnostics.Add(Error("missing-sections", "The package is missing one or more required sections."));
			return diagnostics;
		}

		if (package.Version >= 2 &&
		    (package.Zones is null || package.SourceZones is null ||
		     package.Zones.Count != package.SourceZones.Count))
		{
			diagnostics.Add(Error("invalid-zone-sources",
				"Packages must contain one source entry for every zone definition."));
			return diagnostics;
		}

		if (package.Version >= 2 &&
		    (package.Zones.Cast<object?>().Any(x => x is null) ||
		     package.SourceZones.Cast<object?>().Any(x => x is null)))
		{
			diagnostics.Add(Error("null-zone-entry",
				"Zone and zone-source collections may not contain null entries."));
			return diagnostics;
		}

		if (package.Version >= 3 && package.Areas is null)
		{
			diagnostics.Add(Error("missing-areas", "Packages must contain their area collection."));
			return diagnostics;
		}

		if (package.Version >= 3 && package.Areas.Cast<object?>().Any(x => x is null))
		{
			diagnostics.Add(Error("null-area-entry", "Area collections may not contain null entries."));
			return diagnostics;
		}

		if (package.Cells.Count is < 1 or > MaximumCells)
		{
			diagnostics.Add(Error("invalid-cell-count",
				$"The package must contain between 1 and {MaximumCells:N0} cells."));
		}

		if (package.Exits.Count > MaximumExits)
		{
			diagnostics.Add(Error("invalid-exit-count",
				$"The package may contain at most {MaximumExits:N0} exits."));
		}

		if (package.Cells.Cast<object?>().Any(x => x is null) ||
		    package.Exits.Cast<object?>().Any(x => x is null) ||
		    package.Omissions.Cast<object?>().Any(x => x is null))
		{
			diagnostics.Add(Error("null-collection-entry",
				"Cell, exit, and omission collections may not contain null entries."));
			return diagnostics;
		}

		foreach (var omission in package.Omissions.Where(x =>
			         string.IsNullOrWhiteSpace(x.Code) || string.IsNullOrWhiteSpace(x.Message)))
		{
			diagnostics.Add(Error("invalid-omission",
				"Every package omission must contain both a code and a message."));
		}

		var zones = GetZoneDefinitions(package);
		if (zones.Count == 0)
		{
			diagnostics.Add(Error("missing-zones", "The package does not contain a zone definition."));
			return diagnostics;
		}

		ValidateUniqueKeys(zones.Select((zone, index) => ZoneKey(package, zone, index)), "zone", diagnostics);
		ValidateUniqueKeys(package.Cells.Select(x => x.Key), "cell", diagnostics);
		ValidateUniqueKeys(package.Exits.Select(x => x.Key), "exit", diagnostics);

		if (diagnostics.Any(x => x.Severity == SpatialAreaTransferDiagnosticSeverity.Error)) return diagnostics;

		var cellKeys = package.Cells.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var exitKeys = package.Exits.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var zoneKeys = zones
			.Select((zone, index) => ZoneKey(package, zone, index))
			.ToHashSet(StringComparer.Ordinal);
		foreach (var cell in package.Cells)
		{
			if (!zoneKeys.Contains(cell.ZoneKey))
			{
				diagnostics.Add(Error("orphan-cell",
					$"Cell '{cell.Key}' references missing zone '{cell.ZoneKey}'."));
			}

			if (cell.Overlay is null)
			{
				diagnostics.Add(Error("missing-overlay", $"Cell '{cell.Key}' has no overlay."));
				continue;
			}

			if (string.IsNullOrWhiteSpace(cell.Overlay.CellName) ||
			    string.IsNullOrWhiteSpace(cell.Overlay.CellDescription))
			{
				diagnostics.Add(Error("invalid-overlay-text",
					$"Cell '{cell.Key}' must have both a name and description."));
			}

			if (!double.IsFinite(cell.Overlay.AmbientLightFactor) ||
			    !double.IsFinite(cell.Overlay.AddedLight))
			{
				diagnostics.Add(Error("invalid-overlay-light",
					$"Cell '{cell.Key}' contains a non-finite light value."));
			}

			if (cell.Overlay.Terrain is null || string.IsNullOrWhiteSpace(cell.Overlay.Terrain.Name))
			{
				diagnostics.Add(Error("missing-terrain",
					$"Cell '{cell.Key}' does not identify a terrain dependency."));
			}

			if (cell.Tags is null || cell.RangedCovers is null || cell.MagicResources is null ||
			    cell.Overlay.ExitKeys is null)
			{
				diagnostics.Add(Error("missing-cell-collections",
					$"Cell '{cell.Key}' is missing one or more required collection fields."));
				continue;
			}

			if (cell.Tags.Cast<object?>().Any(x => x is null) ||
			    cell.RangedCovers.Cast<object?>().Any(x => x is null) ||
			    cell.MagicResources.Cast<object?>().Any(x => x is null) ||
			    cell.MagicResources.Any(x => x.Resource is null))
			{
				diagnostics.Add(Error("null-cell-dependency",
					$"Cell '{cell.Key}' contains a null dependency entry."));
				continue;
			}

			if (cell.Tags.Any(x => string.IsNullOrWhiteSpace(x.Name)) ||
			    cell.RangedCovers.Any(x => string.IsNullOrWhiteSpace(x.Name)) ||
			    cell.MagicResources.Any(x => string.IsNullOrWhiteSpace(x.Resource.Name)) ||
			    (cell.ForagableProfile is not null &&
			     string.IsNullOrWhiteSpace(cell.ForagableProfile.Name)) ||
			    (cell.Overlay.HearingProfile is not null &&
			     string.IsNullOrWhiteSpace(cell.Overlay.HearingProfile.Name)) ||
			    (cell.Overlay.Atmosphere is not null &&
			     (string.IsNullOrWhiteSpace(cell.Overlay.Atmosphere.Name) ||
			      string.IsNullOrWhiteSpace(cell.Overlay.Atmosphere.Kind))))
			{
				diagnostics.Add(Error("invalid-cell-dependency",
					$"Cell '{cell.Key}' contains a dependency without a name or kind."));
			}

			if (cell.Overlay.ExitKeys.Count != cell.Overlay.ExitKeys.Distinct(StringComparer.Ordinal).Count())
			{
				diagnostics.Add(Error("duplicate-overlay-exit",
					$"Cell '{cell.Key}' lists the same exit more than once."));
			}

			foreach (var exitKey in cell.Overlay.ExitKeys ?? [])
			{
				if (!exitKeys.Contains(exitKey))
				{
					diagnostics.Add(Error("orphan-overlay-exit",
						$"Cell '{cell.Key}' references missing exit '{exitKey}'."));
				}
			}

			ValidateRouteCell(cell, exitKeys, diagnostics);
		}

		foreach (var exit in package.Exits)
		{
			if (exit.Side1 is null || exit.Side2 is null || exit.BlockedLayers is null)
			{
				diagnostics.Add(Error("missing-exit-fields",
					$"Exit '{exit.Key}' is missing one or more required fields."));
				continue;
			}

			if (!cellKeys.Contains(exit.Cell1Key) || !cellKeys.Contains(exit.Cell2Key))
			{
				diagnostics.Add(Error("orphan-exit",
					$"Exit '{exit.Key}' references a cell outside the package."));
			}

			if (string.Equals(exit.Cell1Key, exit.Cell2Key, StringComparison.Ordinal))
			{
				diagnostics.Add(Error("self-exit", $"Exit '{exit.Key}' connects a cell to itself."));
			}

			if (exit.TimeMultiplier <= 0.0 || !double.IsFinite(exit.TimeMultiplier))
			{
				diagnostics.Add(Error("invalid-time-multiplier",
					$"Exit '{exit.Key}' has an invalid travel-time multiplier."));
			}

			if (exit.FallCellKey is not null && !cellKeys.Contains(exit.FallCellKey))
			{
				diagnostics.Add(Error("orphan-fall-cell",
					$"Exit '{exit.Key}' references missing fall cell '{exit.FallCellKey}'."));
			}

			if (exit.FallCellKey is not null &&
			    !string.Equals(exit.FallCellKey, exit.Cell1Key, StringComparison.Ordinal) &&
			    !string.Equals(exit.FallCellKey, exit.Cell2Key, StringComparison.Ordinal))
			{
				diagnostics.Add(Error("invalid-fall-cell",
					$"Exit '{exit.Key}' fall cell must be one of its two endpoints."));
			}

			var side1NonCardinal = !string.IsNullOrWhiteSpace(exit.Side1.Verb);
			var side2NonCardinal = !string.IsNullOrWhiteSpace(exit.Side2.Verb);
			if (side1NonCardinal != side2NonCardinal)
			{
				diagnostics.Add(Error("asymmetric-exit-kind",
					$"Exit '{exit.Key}' must be cardinal on both sides or non-cardinal on both sides."));
			}

			if (side1NonCardinal &&
			    (string.IsNullOrWhiteSpace(exit.Side1.PrimaryKeyword) ||
			     string.IsNullOrWhiteSpace(exit.Side2.PrimaryKeyword) ||
			     string.IsNullOrWhiteSpace(exit.Side1.OutboundTarget) ||
			     string.IsNullOrWhiteSpace(exit.Side2.OutboundTarget)))
			{
				diagnostics.Add(Error("incomplete-non-cardinal-exit",
					$"Non-cardinal exit '{exit.Key}' is missing verbs, keywords, or targets."));
			}
		}

		var exitsByKey = package.Exits
			.Where(x => !string.IsNullOrWhiteSpace(x.Key))
			.GroupBy(x => x.Key, StringComparer.Ordinal)
			.ToDictionary(x => x.Key, x => x.First(), StringComparer.Ordinal);
		var exitReferences = new Dictionary<string, int>(StringComparer.Ordinal);
		foreach (var cell in package.Cells.Where(x => x.Overlay?.ExitKeys is not null))
		{
			foreach (var exitKey in cell.Overlay.ExitKeys)
			{
				if (!exitsByKey.TryGetValue(exitKey, out var exit))
				{
					continue;
				}

				if (!string.Equals(cell.Key, exit.Cell1Key, StringComparison.Ordinal) &&
				    !string.Equals(cell.Key, exit.Cell2Key, StringComparison.Ordinal))
				{
					diagnostics.Add(Error("invalid-overlay-exit-endpoint",
						$"Cell '{cell.Key}' lists exit '{exitKey}' but is not one of its endpoints."));
				}

				exitReferences[exitKey] = exitReferences.GetValueOrDefault(exitKey) + 1;
			}
		}

		foreach (var exit in package.Exits.Where(x => !exitReferences.ContainsKey(x.Key)))
		{
			diagnostics.Add(Error("unreferenced-exit",
				$"Exit '{exit.Key}' is not active in any packaged cell overlay."));
		}

		ValidateZones(package, zones, cellKeys, diagnostics);
		ValidateAreas(package, cellKeys, diagnostics);

		return diagnostics;
	}

	public static IReadOnlyList<SpatialZoneDefinition> GetZoneDefinitions(SpatialAreaPackage package) => package.Zones ?? [];

	public static string ZoneKey(SpatialAreaPackage package, SpatialZoneDefinition zone, int index) => zone.Key ?? string.Empty;

	private static void ValidateZones(
		SpatialAreaPackage package,
		IReadOnlyList<SpatialZoneDefinition> zones,
		IReadOnlySet<string> cellKeys,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		foreach (var (zone, index) in zones.Select((value, index) => (value, index)))
		{
			var zoneKey = ZoneKey(package, zone, index);
			if (!cellKeys.Contains(zone.DefaultCellKey))
			{
				diagnostics.Add(Error("invalid-default-cell",
					$"Zone '{zoneKey}' default cell '{zone.DefaultCellKey}' is not in the package."));
			}
			else
			{
				var defaultCell = package.Cells.First(x => x.Key == zone.DefaultCellKey);
				if (!string.Equals(defaultCell.ZoneKey, zoneKey, StringComparison.Ordinal))
				{
					diagnostics.Add(Error("wrong-zone-default-cell",
						$"Zone '{zoneKey}' default cell belongs to another packaged zone."));
				}
			}

			if (string.IsNullOrWhiteSpace(zone.Name))
			{
				diagnostics.Add(Error("missing-zone-name", $"Package zone '{zoneKey}' must have a name."));
			}

			if (zone.TimeZones is null)
			{
				diagnostics.Add(Error("missing-timezones",
					$"Package zone '{zoneKey}' is missing its timezone collection."));
			}
			else
			{
				if (zone.TimeZones.Cast<object?>().Any(x => x is null))
				{
					diagnostics.Add(Error("null-timezone-reference",
						$"Package zone '{zoneKey}' timezone collection may not contain null entries."));
					continue;
				}

				var duplicateClock = zone.TimeZones
					.GroupBy(x => x.ClockAlias, StringComparer.InvariantCultureIgnoreCase)
					.FirstOrDefault(x => x.Count() > 1);
				if (duplicateClock is not null)
				{
					diagnostics.Add(Error("duplicate-clock-timezone",
						$"Package zone '{zoneKey}' contains more than one timezone for clock '{duplicateClock.Key}'."));
				}

				if (zone.TimeZones.Any(x =>
					    string.IsNullOrWhiteSpace(x.ClockAlias) ||
					    string.IsNullOrWhiteSpace(x.TimeZoneAlias)))
				{
					diagnostics.Add(Error("invalid-timezone-reference",
						$"Every timezone in package zone '{zoneKey}' must identify both a clock and timezone alias."));
				}
			}

			if ((zone.ForagableProfile is not null &&
			     string.IsNullOrWhiteSpace(zone.ForagableProfile.Name)) ||
			    (zone.WeatherController is not null &&
			     string.IsNullOrWhiteSpace(zone.WeatherController.Name)))
			{
				diagnostics.Add(Error("invalid-zone-dependency",
					$"Package zone '{zoneKey}' dependency references must have non-empty names."));
			}
		}

		var duplicateName = zones
			.GroupBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicateName is not null)
		{
			diagnostics.Add(Error("duplicate-zone-name",
				$"The package contains duplicate zone name '{duplicateName.Key}'."));
		}
	}

	private static void ValidateAreas(
		SpatialAreaPackage package,
		IReadOnlySet<string> cellKeys,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		if (package.Version < 3)
		{
			return;
		}

		ValidateUniqueKeys(package.Areas.Select(x => x.Key), "area", diagnostics);
		foreach (var area in package.Areas)
		{
			if (string.IsNullOrWhiteSpace(area.Name))
			{
				diagnostics.Add(Error("missing-area-name", $"Package area '{area.Key}' must have a name."));
			}

			if (area.CellKeys is null || area.CellKeys.Count == 0)
			{
				diagnostics.Add(Error("invalid-area-cells",
					$"Package area '{area.Key}' must contain at least one cell."));
				continue;
			}

			if (area.CellKeys.Any(x => !cellKeys.Contains(x)))
			{
				diagnostics.Add(Error("orphan-area-cell",
					$"Package area '{area.Key}' references a cell outside the package."));
			}

			if (area.CellKeys.Count != area.CellKeys.Distinct(StringComparer.Ordinal).Count())
			{
				diagnostics.Add(Error("duplicate-area-cell",
					$"Package area '{area.Key}' lists the same cell more than once."));
			}

			if (area.WeatherController is not null && string.IsNullOrWhiteSpace(area.WeatherController.Name))
			{
				diagnostics.Add(Error("invalid-area-weather-controller",
					$"Package area '{area.Key}' has a weather controller without a name."));
			}
		}
	}

	private static void ValidateRouteCell(
		SpatialCellDefinition cell,
		IReadOnlySet<string> exitKeys,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		var route = cell.RouteCell;
		if (route is null)
		{
			return;
		}

		if (!double.IsFinite(route.LengthMetres) || route.LengthMetres <= 0.0 ||
		    !double.IsFinite(route.DefaultPositionMetres) ||
		    route.DefaultPositionMetres < 0.0 ||
		    route.DefaultPositionMetres > route.LengthMetres ||
		    !double.IsFinite(route.MetresPerRoomEquivalent) ||
		    route.MetresPerRoomEquivalent <= 0.0 ||
		    route.TopologyVersion < 1 ||
		    string.IsNullOrWhiteSpace(route.PositiveDirectionName) ||
		    string.IsNullOrWhiteSpace(route.NegativeDirectionName))
		{
			diagnostics.Add(Error("invalid-route-cell",
				$"Cell '{cell.Key}' contains invalid route-cell geometry."));
			return;
		}

		if (route.Landmarks is null || route.ExitAnchors is null)
		{
			diagnostics.Add(Error("missing-route-collections",
				$"Cell '{cell.Key}' route-cell collections may not be null."));
			return;
		}

		foreach (var landmark in route.Landmarks)
		{
			if (landmark is null ||
			    string.IsNullOrWhiteSpace(landmark.Name) ||
			    !double.IsFinite(landmark.PositionMetres) ||
			    landmark.PositionMetres < 0.0 ||
			    landmark.PositionMetres > route.LengthMetres)
			{
				diagnostics.Add(Error("invalid-route-landmark",
					$"Cell '{cell.Key}' contains an invalid route landmark."));
			}
		}

		foreach (var anchor in route.ExitAnchors)
		{
			if (anchor is null ||
			    !exitKeys.Contains(anchor.ExitKey) ||
			    !cell.Overlay.ExitKeys.Contains(anchor.ExitKey) ||
			    !double.IsFinite(anchor.MinimumPositionMetres) ||
			    !double.IsFinite(anchor.MaximumPositionMetres) ||
			    !double.IsFinite(anchor.ArrivalPositionMetres) ||
			    anchor.MinimumPositionMetres < 0.0 ||
			    anchor.MaximumPositionMetres < anchor.MinimumPositionMetres ||
			    anchor.MaximumPositionMetres > route.LengthMetres ||
			    anchor.ArrivalPositionMetres < anchor.MinimumPositionMetres ||
			    anchor.ArrivalPositionMetres > anchor.MaximumPositionMetres)
			{
				diagnostics.Add(Error("invalid-route-anchor",
					$"Cell '{cell.Key}' contains an invalid route exit anchor."));
			}
		}

		var duplicateAnchor = route.ExitAnchors
			.Where(x => x is not null)
			.GroupBy(x => x.ExitKey, StringComparer.Ordinal)
			.FirstOrDefault(x => x.Count() > 1);
		if (duplicateAnchor is not null)
		{
			diagnostics.Add(Error("duplicate-route-anchor",
				$"Cell '{cell.Key}' contains more than one anchor for exit '{duplicateAnchor.Key}'."));
		}
	}

	private static void ValidateUniqueKeys(
		IEnumerable<string> keys,
		string kind,
		ICollection<SpatialAreaTransferDiagnostic> diagnostics)
	{
		var seen = new HashSet<string>(StringComparer.Ordinal);
		foreach (var key in keys)
		{
			if (string.IsNullOrWhiteSpace(key))
			{
				diagnostics.Add(Error($"empty-{kind}-key", $"A {kind} has an empty package key."));
				continue;
			}

			if (!seen.Add(key))
			{
				diagnostics.Add(Error($"duplicate-{kind}-key",
					$"The package contains duplicate {kind} key '{key}'."));
			}
		}
	}

	private static string ComputeHash(string text)
	{
		return Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(text)));
	}

	private static string SerializeForVersion(SpatialAreaPackage package) => JsonSerializer.Serialize(package, Options);

	private static SpatialAreaTransferDiagnostic Error(string code, string message)
	{
		return new SpatialAreaTransferDiagnostic(SpatialAreaTransferDiagnosticSeverity.Error, code, message);
	}

	private static SpatialAreaTransferDiagnostic Warning(string code, string message)
	{
		return new SpatialAreaTransferDiagnostic(SpatialAreaTransferDiagnosticSeverity.Warning, code, message);
	}
}
