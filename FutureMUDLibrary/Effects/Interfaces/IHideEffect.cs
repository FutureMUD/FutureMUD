namespace MudSharp.Effects.Interfaces {
    public interface IHideEffect : IRemoveOnCombatStart {
        double EffectiveHideSkill { get; }
    }
}