using System;

#nullable enable
namespace MudSharp.Models;

public class CharacterMagicCapabilityState
{
	public long CharacterId { get; set; }
	public long MagicCapabilityId { get; set; }
	public long StateVersion { get; set; }
	public string Definition { get; set; } = "";
	public virtual Character Character { get; set; } = null!;
	public virtual MagicCapability MagicCapability { get; set; } = null!;
}

/// <summary>Durable consumption and repair evidence; never an alternative slot ledger.</summary>
public class VancianMagicOperation
{
	public Guid Id { get; set; }
	public long CharacterId { get; set; }
	public long MagicCapabilityId { get; set; }
	public long? SourceItemId { get; set; }
	public long? DestinationItemId { get; set; }
	public string Kind { get; set; } = "";
	public string Status { get; set; } = "";
	public long ExpectedStateVersion { get; set; }
	public string Definition { get; set; } = "";
	public DateTime CreatedUtc { get; set; }
	public DateTime UpdatedUtc { get; set; }
	public string Diagnostic { get; set; } = "";
}
