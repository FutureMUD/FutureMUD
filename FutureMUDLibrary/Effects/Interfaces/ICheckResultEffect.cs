using MudSharp.Body.Traits;
using MudSharp.Framework;
using MudSharp.RPG.Checks;

namespace MudSharp.Effects.Interfaces {
    public interface ICheckResultEffect : IEffectSubtype {
        CheckType Check { get; }
        Difficulty Difficulty { get; }
        long? TargetID { get; }
        string TargetType { get; }
        long? ToolID { get; }
        string ToolType { get; }
        Outcome Outcome { get; }
        ITraitDefinition Trait { get; }
        bool SameResult(ICheckResultEffect other);
        bool SameCheck(CheckType type, Difficulty difficulty, IFrameworkItem target, ITraitDefinition trait, IFrameworkItem tool);
    }
}