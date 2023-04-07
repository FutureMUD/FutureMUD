using MudSharp.Framework;
using ExpressionEngine;

namespace MudSharp.Body.Traits.Subtypes;

public class TheoreticalSkillDefinition : SkillDefinition, ITheoreticalSkillDefinition
{
	public TheoreticalSkillDefinition(MudSharp.Models.TraitDefinition trait, IFuturemud game) : base(trait, game)
	{
		ValueExpression = new Expression(trait.ValueExpression);
	}

	#region Overrides of TraitDefinition

	public override TraitType TraitType => TraitType.Skill;

	public override ITrait LoadTrait(MudSharp.Models.Trait trait, IHaveTraits owner)
	{
		return new TheoreticalSkill(this, trait, owner);
	}

	#endregion

	#region Implementation of ITheoreticalSkillDefinition

	public Expression ValueExpression { get; set; }

	#endregion
}