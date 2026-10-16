#nullable enable

using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public enum SpellNpcMissionKind
{
	None,
	Seek,
	Courier,
	Haunt
}

/// <summary>Explicit ephemeral mission input; entity references must be fenced by physical identity.</summary>
public sealed record SpellNpcMissionInputs(SpellNpcMissionKind Kind, ICharacter ExactTarget, IGameItem? ExactParcel = null)
{
	public const string ParameterName = "SpellNpcMission";
}
