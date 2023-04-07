using System;
using MudSharp.Body.Traits.Improvement;
using MudSharp.Logging;
using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Subtypes;

public class Skill : Trait, ISkill
{
	protected SkillDefinition _definition;

	public Skill(SkillDefinition definition, double value, IHaveTraits owner)
	{
		_definition = definition;
		_value = value;
		_owner = owner;
	}

	public Skill(SkillDefinition definition, MudSharp.Models.Trait trait, IHaveTraits owner)
		: base(trait, owner)
	{
		_definition = definition;
	}

	public override void Initialise(IHaveTraits owner)
	{
		base.Initialise(owner);
	}

	public ISkillDefinition SkillDefinition => _definition;

	protected IImprovementModel Improver => _definition.Improver;
	public override ITraitDefinition Definition => _definition;

	public override bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype)
	{
		Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillUse, user, Definition, result, difficulty, usetype);
		var improvement = Improver.GetImprovement(user, this, difficulty, result, usetype);
		var oldValue = _value;
		Value += improvement;
		return oldValue != _value;
	}

	public override double MaxValue => _definition.Cap.Evaluate(_owner);

	public override double Value
	{
		get => Math.Min(MaxValue, _value);
		set => base.Value = value;
	}
}