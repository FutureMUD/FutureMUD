using System.Collections.Generic;
using MudSharp.Body.Traits;
using MudSharp.Character;

#nullable enable
namespace MudSharp.Magic;

/// <summary>Numerical inputs are separate from the real actor and live target state.</summary>
public interface ISpellNumericalContext
{
	int SpellLevel { get; }
	int CastingLevel { get; }
	int CasterLevel { get; }
	bool IsStored { get; }
	double Evaluate(string location, ITraitExpression expression, IHaveTraits actor, ITraitDefinition? variable,
		TraitBonusContext context, IEnumerable<(string Name, object Value)> values);
}
