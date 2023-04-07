using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces {
    public interface ICheckBonusEffect : IEffectSubtype {
        bool AppliesToCheck(CheckType type);
        double CheckBonus { get; }
    }
}