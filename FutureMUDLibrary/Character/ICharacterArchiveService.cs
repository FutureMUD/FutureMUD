#nullable enable

using System;

namespace MudSharp.Character;

/// <summary>Historical attribution, read without constructing a character, body, controller or AI.</summary>
public sealed record ArchivedCharacterIdentity(long CharacterId, long OriginalBodyId, Guid LifecycleId,
	DateTime ArchivedUtc, string DisplayName, string ShortDescription, string FullDescription)
{
	/// <summary>Resolved from retained name metadata, independently of nickname-inclusive display.</summary>
	public string? FullName { get; init; }
	public string? NameInfo { get; init; }
}

public interface ICharacterArchiveService
{
	ArchivedCharacterIdentity? Find(long characterId);
	bool TryArchiveNpc(Guid lifecycleId, long expectedVersion, ICharacter character, DateTime nowUtc,
		out string diagnostic);
}
