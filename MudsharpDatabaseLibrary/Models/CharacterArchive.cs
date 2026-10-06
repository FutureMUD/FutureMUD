#nullable enable

using System;

namespace MudSharp.Models;

/// <summary>Immutable attribution for a compacted spell-created NPC; never a materializable actor.</summary>
public class CharacterArchive
{
	public long CharacterId { get; set; }
	public long OriginalBodyId { get; set; }
	public Guid LifecycleId { get; set; }
	public DateTime ArchivedUtc { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public string ShortDescription { get; set; } = string.Empty;
	public string FullDescription { get; set; } = string.Empty;
	public string WoundHistory { get; set; } = "[]";
	public string Provenance { get; set; } = string.Empty;
}
