#nullable enable

using MudSharp.GameItems;

namespace MudSharp.Magic;

/// <summary>Engine-issued invocation input; it carries neither a room ID nor authority to move an arbitrary entity.</summary>
public sealed record SpellTravelDestinationRequest(IGameItem? Rune)
{
	public const string ParameterName = "spell-travel-destination";
}
