#nullable enable

using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.GameItems.Interfaces;

public interface ISealable : IGameItemComponent
{
	bool IsSealed { get; }
	bool SealBroken { get; }
	bool HasSealResidue { get; }
	SealImpression? CurrentSeal { get; }
	Difficulty InspectionDifficulty { get; }
	bool CanSeal(ICharacter actor, ISealStamp stamp, IGameItem? medium, out string error);
	void Seal(ICharacter actor, ISealStamp stamp, IGameItem? medium);
	bool BreakSeal(ICharacter? actor, string reason);
	string InspectSeal(ICharacter actor);
	bool SealMatches(ISealStamp stamp);
	bool SealMatches(ISealable sealable);
}
