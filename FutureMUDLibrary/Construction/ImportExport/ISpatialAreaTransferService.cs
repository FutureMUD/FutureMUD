#nullable enable

using System.Collections.Generic;
using MudSharp.Character;

namespace MudSharp.Construction.ImportExport;

public enum SpatialAreaTransferDiagnosticSeverity
{
	Information,
	Warning,
	Error
}

public sealed record SpatialAreaTransferDiagnostic(
	SpatialAreaTransferDiagnosticSeverity Severity,
	string Code,
	string Message);

public sealed class SpatialAreaTransferResult
{
	public bool Success { get; init; }
	public string Summary { get; init; } = string.Empty;
	public string? PackagePath { get; init; }
	public long? ImportedZoneId { get; init; }
	public int RoomCount { get; init; }
	public int CellCount { get; init; }
	public int ExitCount { get; init; }
	public IReadOnlyList<SpatialAreaTransferDiagnostic> Diagnostics { get; init; } =
		[];
}

/// <summary>
/// Exports and imports versioned, portable spatial-area packages.
/// </summary>
public interface ISpatialAreaTransferService
{
	string PackageDirectory { get; }

	SpatialAreaTransferResult ExportZone(IZone zone, string packageFileName);

	SpatialAreaTransferResult ValidateImport(
		ICharacter actor,
		IShard targetShard,
		string packageFileName,
		string? zoneNameOverride = null);

	SpatialAreaTransferResult ImportZone(
		ICharacter actor,
		IShard targetShard,
		string packageFileName,
		string? zoneNameOverride = null);
}
