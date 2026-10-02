#nullable enable

using System;

namespace MudSharp.Models;

/// <summary>Exact ordinary ownership retained while a retired body still has physical dependencies.</summary>
public partial class CharacterBodyRetirement
{
	public long BodyId { get; set; }
	public long CharacterId { get; set; }
	public DateTime RetiredUtc { get; set; }
	public virtual Body Body { get; set; } = null!;
	public virtual Character Character { get; set; } = null!;
}
