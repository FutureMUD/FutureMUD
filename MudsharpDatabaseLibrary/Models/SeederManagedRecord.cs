using System;

namespace MudSharp.Models;

public class SeederManagedRecord
{
	public long Id { get; set; }
	public string Seeder { get; set; } = string.Empty;
	public string Module { get; set; } = string.Empty;
	public string EntityType { get; set; } = string.Empty;
	public string StableKey { get; set; } = string.Empty;
	public long? LogicalId { get; set; }
	public int? RevisionNumber { get; set; }
	public string AppliedFingerprint { get; set; } = string.Empty;
	public string ManifestVersion { get; set; } = string.Empty;
	public DateTime AppliedAt { get; set; }
	public bool Retired { get; set; }
}
