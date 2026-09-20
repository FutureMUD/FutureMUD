using System;

#nullable enable
namespace MudSharp.Models;

/// <summary>
/// Durable evidence for one cross-holder gathering commitment. It is not a spendable-resource ledger and
/// deliberately never supplies automatic replay or refund authority.
/// </summary>
public class MagicGatheringOperation
{
	public Guid Id { get; set; }
	public long OwnerId { get; set; }
	public long ActorId { get; set; }
	public long BodyId { get; set; }
	public long MagicCapabilityId { get; set; }
	public Guid MethodKey { get; set; }
	public int MethodVersion { get; set; }
	public long? CellId { get; set; }
	public long? SourceProfileId { get; set; }
	public long? SourceProfileRevision { get; set; }
	public long? SourceResourceId { get; set; }
	public long DestinationResourceId { get; set; }
	public string Kind { get; set; } = "";
	public double RequestedAmount { get; set; }
	public double SourceDebit { get; set; }
	public double StaminaCost { get; set; }
	public double DamageCost { get; set; }
	public double PainCost { get; set; }
	public double StunCost { get; set; }
	public bool SourceDebited { get; set; }
	public bool BodilyCostApplied { get; set; }
	public bool DestinationCredited { get; set; }
	public bool AccountingPersisted { get; set; }
	public bool NotificationCompleted { get; set; }
	public string Status { get; set; } = "";
	public DateTime CreatedUtc { get; set; }
	public DateTime UpdatedUtc { get; set; }
	public string Diagnostic { get; set; } = "";
	public string? LandDetailJson { get; set; }
	public Guid? EcologicalChildId { get; set; }
	public bool EcologicalApplied { get; set; }
}
