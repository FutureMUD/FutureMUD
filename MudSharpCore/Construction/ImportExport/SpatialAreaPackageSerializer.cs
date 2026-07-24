#nullable enable

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MudSharp.Construction.ImportExport;

public sealed class SpatialAreaPackageReadResult
{
	public SpatialAreaPackage? Package { get; init; }
	public IReadOnlyList<SpatialAreaTransferDiagnostic> Diagnostics { get; init; } = [];
	public bool Success => Package is not null &&
	                       Diagnostics.All(x => x.Severity != SpatialAreaTransferDiagnosticSeverity.Error);
}

public static class SpatialAreaPackageSerializer
{
	public const long MaximumPackageBytes = 16L * 1024L * 1024L;
	public const int MaximumRooms = 10_000;
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
		package.IntegritySha256 = string.Empty;
		var canonicalJson = JsonSerializer.Serialize(package, Options);
		package.IntegritySha256 = ComputeHash(canonicalJson);
		return JsonSerializer.Serialize(package, Options);
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
		var expectedHash = ComputeHash(JsonSerializer.Serialize(package, Options));
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
				$"Package version {package.Version:N0} is not supported; this server supports version {SpatialAreaPackage.CurrentVersion:N0}."));
		}

		if (package.Rooms is null || package.Cells is null || package.Exits is null || package.Zone is null ||
		    package.Source is null)
		{
			diagnostics.Add(Error("missing-sections", "The package is missing one or more required sections."));
			return diagnostics;
		}

		if (package.Rooms.Count is < 1 or > MaximumRooms)
		{
			diagnostics.Add(Error("invalid-room-count",
				$"The package must contain between 1 and {MaximumRooms:N0} rooms."));
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

		if (package.Rooms.Cast<object?>().Any(x => x is null) ||
		    package.Cells.Cast<object?>().Any(x => x is null) ||
		    package.Exits.Cast<object?>().Any(x => x is null))
		{
			diagnostics.Add(Error("null-collection-entry",
				"Room, cell, and exit collections may not contain null entries."));
			return diagnostics;
		}

		ValidateUniqueKeys(package.Rooms.Select(x => x.Key), "room", diagnostics);
		ValidateUniqueKeys(package.Cells.Select(x => x.Key), "cell", diagnostics);
		ValidateUniqueKeys(package.Exits.Select(x => x.Key), "exit", diagnostics);

		var roomKeys = package.Rooms.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var cellKeys = package.Cells.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		var exitKeys = package.Exits.Select(x => x.Key).ToHashSet(StringComparer.Ordinal);
		foreach (var room in package.Rooms.Where(room =>
			         package.Cells.All(cell => !string.Equals(cell.RoomKey, room.Key, StringComparison.Ordinal))))
		{
			diagnostics.Add(Error("empty-room",
				$"Room '{room.Key}' does not contain a packaged cell."));
		}

		foreach (var cell in package.Cells)
		{
			if (!roomKeys.Contains(cell.RoomKey))
			{
				diagnostics.Add(Error("orphan-cell",
					$"Cell '{cell.Key}' references missing room '{cell.RoomKey}'."));
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

		if (!cellKeys.Contains(package.Zone.DefaultCellKey))
		{
			diagnostics.Add(Error("invalid-default-cell",
				$"Zone default cell '{package.Zone.DefaultCellKey}' is not in the package."));
		}

		if (string.IsNullOrWhiteSpace(package.Zone.Name))
		{
			diagnostics.Add(Error("missing-zone-name", "The package zone must have a name."));
		}

		if (package.Zone.TimeZones is null)
		{
			diagnostics.Add(Error("missing-timezones", "The package zone is missing its timezone collection."));
		}
		else
		{
			if (package.Zone.TimeZones.Cast<object?>().Any(x => x is null))
			{
				diagnostics.Add(Error("null-timezone-reference",
					"The package timezone collection may not contain null entries."));
				return diagnostics;
			}

			var duplicateClock = package.Zone.TimeZones
				.GroupBy(x => x.ClockAlias, StringComparer.InvariantCultureIgnoreCase)
				.FirstOrDefault(x => x.Count() > 1);
			if (duplicateClock is not null)
			{
				diagnostics.Add(Error("duplicate-clock-timezone",
					$"The package contains more than one timezone for clock '{duplicateClock.Key}'."));
			}

			if (package.Zone.TimeZones.Any(x =>
				    string.IsNullOrWhiteSpace(x.ClockAlias) ||
				    string.IsNullOrWhiteSpace(x.TimeZoneAlias)))
			{
				diagnostics.Add(Error("invalid-timezone-reference",
					"Every packaged timezone must identify both a clock alias and timezone alias."));
			}
		}

		if ((package.Zone.ForagableProfile is not null &&
		     string.IsNullOrWhiteSpace(package.Zone.ForagableProfile.Name)) ||
		    (package.Zone.WeatherController is not null &&
		     string.IsNullOrWhiteSpace(package.Zone.WeatherController.Name)))
		{
			diagnostics.Add(Error("invalid-zone-dependency",
				"Zone dependency references must have non-empty names."));
		}

		return diagnostics;
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

	private static SpatialAreaTransferDiagnostic Error(string code, string message)
	{
		return new SpatialAreaTransferDiagnostic(SpatialAreaTransferDiagnosticSeverity.Error, code, message);
	}
}
