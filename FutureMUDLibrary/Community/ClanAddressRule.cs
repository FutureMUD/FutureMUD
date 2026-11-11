#nullable enable

using System;

namespace MudSharp.Community;

[Flags]
public enum ClanAddressRelation
{
	Lower = 1,
	Peer = 2,
	Higher = 4,
	Outsider = 8,
	Any = Lower | Peer | Higher | Outsider
}

/// <summary>An ordered direct-address rule. Null text deliberately suppresses fallback.</summary>
public sealed class ClanAddressRule
{
	public Guid Id { get; set; } = Guid.NewGuid();
	public string? Text { get; set; }
	public ClanAddressRelation Relations { get; set; } = ClanAddressRelation.Any;
	public bool IsPublic { get; set; }
	public long? FutureProgId { get; set; }
	/// <summary>Optional stable profile key for explicitly regenerated, clan-bound predicates.</summary>
	public string? TemplateKey { get; set; }

	public ClanAddressRule Copy() => new()
	{
		Text = Text, Relations = Relations, IsPublic = IsPublic, FutureProgId = FutureProgId,
		TemplateKey = TemplateKey
	};
}
