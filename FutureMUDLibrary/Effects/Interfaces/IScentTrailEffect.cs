using MudSharp.Character;
using MudSharp.Construction;
using MudSharp.RPG.Checks;

#nullable enable

namespace MudSharp.Effects.Interfaces;

public interface IScentTrailEffect : IDescriptionAdditionEffect
{
	long SourceItemId { get; }
	string SourceDescription { get; }
	RoomLayer RoomLayer { get; }
	int Distance { get; }
	Difficulty ScentDifficulty(ICharacter actor);
	string DescribeForTracksCommand(ICharacter actor);
}
