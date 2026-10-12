using MudSharp.Character;
using MudSharp.GameItems;

namespace MudSharp.Magic;

/// <summary>Typed, creator-authorized assignment surface for bounded native summon missions.</summary>
public interface ISpellNpcMissionController
{
	bool AssignSeeking(ICharacter commander, ICharacter exactTarget);
	bool AssignCourier(ICharacter commander, IGameItem exactParcel, ICharacter exactRecipient);
	bool AssignHaunting(ICharacter commander, ICharacter exactTarget);
	bool ClearMission(ICharacter commander);
}
