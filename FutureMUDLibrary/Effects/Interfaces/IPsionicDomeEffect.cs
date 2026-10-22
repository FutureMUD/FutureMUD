#nullable enable

using MudSharp.Character;

namespace MudSharp.Effects;

/// <summary>Identifies a room anchored psionic ward and the character sustaining it.</summary>
public interface IPsionicDomeEffect : IEffect
{
	ICharacter OwnerCharacter { get; }
	long RoomId { get; }
	bool AppliesTo(ICharacter character);
}
