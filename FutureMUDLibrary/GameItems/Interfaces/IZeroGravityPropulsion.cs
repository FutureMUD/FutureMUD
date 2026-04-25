using MudSharp.Character;

namespace MudSharp.GameItems.Interfaces;

public interface IZeroGravityPropulsion : IGameItemComponent
{
	bool CanPropel(ICharacter character);
	string WhyCannotPropel(ICharacter character);
	bool ConsumePropellant(ICharacter character);
}
