using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public abstract class DerivedTrait : Trait
{
	protected DerivedTraitDefinition _definition;

	protected DerivedTrait(DerivedTraitDefinition def)
	{
		_definition = def;
	}

	protected DerivedTrait(DerivedTraitDefinition definition, MudSharp.Models.Trait trait, IHaveTraits owner)
		: base(trait, owner)
	{
		_definition = definition;
	}

	protected DerivedTrait(DerivedTraitDefinition definition, IHaveTraits owner)
	{
		_definition = definition;
		_owner = owner;
	}

	public override ITraitDefinition Definition => _definition;

	public override double MaxValue => _definition.Expression.EvaluateMax(_owner);

	public override bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype)
	{
		return false;
	}

	public override void Initialise(IHaveTraits owner)
	{
		base.Initialise(owner);
		if (owner == null)
		{
			return;
		}

		_value = _definition.Expression.Evaluate(owner);
		foreach (var trait in _definition.Expression.Parameters.Values)
		{
			var trait2 = owner.GetTrait(trait.Trait);
			if (trait2 != null)
			{
				trait2.TraitValueChanged += LinkedTraitUpdated;
			}
		}
	}

	protected void LinkedTraitUpdated(object sender, TraitChangedEventArgs args)
	{
		Value = _definition.Expression.Evaluate(_owner);
	}
}