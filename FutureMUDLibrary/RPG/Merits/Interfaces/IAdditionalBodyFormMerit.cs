#nullable enable
using MudSharp.Character;
using MudSharp.RPG.Merits;

namespace MudSharp.RPG.Merits.Interfaces;

public interface IAdditionalBodyFormMerit : ICharacterMerit
{
	ICharacterFormSpecification FormSpecification { get; }
}
