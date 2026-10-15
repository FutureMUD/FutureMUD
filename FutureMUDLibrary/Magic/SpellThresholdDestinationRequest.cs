#nullable enable

using System;
using System.Runtime.CompilerServices;
using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

public sealed record SpellThresholdDestinationRequest(IGameItem? Mark, ICharacter? Traveller)
{
	public const string ParameterName = "threshold-destination";
	public bool Equals(SpellThresholdDestinationRequest? other) => other is not null && ReferenceEquals(Mark, other.Mark) && ReferenceEquals(Traveller, other.Traveller);
	public override int GetHashCode() => HashCode.Combine(Mark is null ? 0 : RuntimeHelpers.GetHashCode(Mark), Traveller is null ? 0 : RuntimeHelpers.GetHashCode(Traveller));
}
