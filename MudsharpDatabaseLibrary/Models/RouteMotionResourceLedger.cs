#nullable enable

using System;

namespace MudSharp.Models;

public partial class RouteMotionResourceLedger
{
	public long Id { get; set; }
	public long ActiveRouteMotionId { get; set; }
	public long CheckpointSequence { get; set; }
	public string IdempotencyKey { get; set; } = null!;
	public int ResourceOwnerType { get; set; }
	public long ResourceOwnerId { get; set; }
	public int ResourceType { get; set; }
	public long? ResourceReferenceId { get; set; }
	public string ResourceKey { get; set; } = null!;
	public decimal ReservedAmount { get; set; }
	public decimal ConsumedAmount { get; set; }
	public int Status { get; set; }
	public DateTime CreatedDateTime { get; set; }
	public DateTime? CommittedDateTime { get; set; }

	public virtual ActiveRouteMotion ActiveRouteMotion { get; set; } = null!;
}
