using MudSharp.Body.Traits.Subtypes;

namespace MudSharp.Body.Traits {
    public interface ISkill : ITrait {
        ISkillDefinition SkillDefinition { get; }
    }
}