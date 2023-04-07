using MudSharp.Character;

namespace MudSharp.Effects.Interfaces {
    public interface IItemHiddenEffect : IRemoveOnGet
    {
        bool KnewOriginalHidingPlace(ICharacter actor);
        double EffectiveHideSkill { get; }
    }
}