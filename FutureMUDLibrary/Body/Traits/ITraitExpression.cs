using ExpressionEngine;
using MudSharp.Character;
using MudSharp.Framework;
using MudSharp.Framework.Save;
using System.Collections.Generic;

namespace MudSharp.Body.Traits
{
    public interface ITraitExpression : ISaveable, IFrameworkItem
    {
        Expression Formula { get; }
        Dictionary<string, TraitExpressionParameter> Parameters { get; }
		/// <summary>Exact references parsed by the native expression grammar, without loading traits or running it.</summary>
		IReadOnlyDictionary<string, long> TraitReferenceIds { get; }
        double Evaluate(IHaveTraits owner, ITraitDefinition variable = null, TraitBonusContext context = TraitBonusContext.None);

        double EvaluateWith(IHaveTraits owner, ITraitDefinition variable = null,
            TraitBonusContext context = TraitBonusContext.None, params (string Name, object Value)[] values);
        double EvaluateMax(IHaveTraits owner);
        string ShowBuilder(ICharacter actor);
        bool BuildingCommand(ICharacter actor, StringStack command);
        bool HasErrors();
        string Error { get; }
        string OriginalFormulaText { get; }
        IEnumerable<string> NonTraitParameters { get; }

    }
}
