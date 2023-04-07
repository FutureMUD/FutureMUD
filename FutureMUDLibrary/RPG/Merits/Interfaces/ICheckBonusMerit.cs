using MudSharp.Character;
using MudSharp.RPG.Checks;

namespace MudSharp.RPG.Merits.Interfaces {
    public interface ICheckBonusMerit : ICharacterMerit {
        double CheckBonus(ICharacter ch, CheckType type);
    }
}