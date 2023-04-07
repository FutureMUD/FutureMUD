using ExpressionEngine;

namespace MudSharp.Body.Traits.Subtypes {
    public interface ITheoreticalSkillDefinition : ISkillDefinition {
        Expression ValueExpression { get; }
    }
}
