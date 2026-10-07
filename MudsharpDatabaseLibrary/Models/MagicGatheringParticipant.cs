#nullable enable

using System;

namespace MudSharp.Models;

/// <summary>Indexed physical participation in a durable gathering operation.</summary>
public class MagicGatheringParticipant
{
	public Guid OperationId { get; set; }
	public long RoomId { get; set; }
	public string SourceKey { get; set; } = "";
}
