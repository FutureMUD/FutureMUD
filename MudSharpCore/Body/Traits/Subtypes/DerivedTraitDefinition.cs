using System.Collections.Generic;
using MudSharp.Body.Traits.Decorators;
using MudSharp.Framework;

namespace MudSharp.Body.Traits.Subtypes;

public abstract class DerivedTraitDefinition : TraitDefinition
{
	protected DerivedTraitDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game)
		: base(trait, game)
	{
	}

	public override string MaxValueString => Expression.Formula.OriginalExpression;

	public ITraitExpression Expression { get; protected set; }

	public override void Initialise(MudSharp.Models.TraitDefinition definition)
	{
		Expression = Gameworld.TraitExpressions.Get(definition.ExpressionId ?? 0);
	}
}