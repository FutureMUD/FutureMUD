using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Merits.Interfaces;
public interface ICheckBonusMerit : ICharacterMerit
{
    double CheckBonus(ICharacter ch, IPerceivable target, CheckType type);
}
