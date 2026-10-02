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

    public override bool TraitUsed(IHaveTraits user, Outcome result, Difficulty difficulty, TraitUseType usetype, IEnumerable<Tuple<string, double>> bonuses)
    {
        Gameworld.LogManager.CustomLogEntry(LogEntryType.SkillUse, user, Definition, result, difficulty, usetype, bonuses);
		var castingCap = _owner is ICharacter character ? Gameworld.MagicCasting?.RawSkillImprovementCap(character, Definition.Id) : null;
		if (castingCap.HasValue && _value >= castingCap.Value)
		{
			// Ordinary native branching is independent of numerical improvement. Reaching the
			// casting ceiling must not suppress a legitimate branch on the next skill use.
			if (Improver is BranchingImprover branching) branching.EvaluateBranches(user, this);
			return false;
		}
        double improvement = Improver.GetImprovement(user, this, difficulty, result, usetype);
        double oldValue = _value;
		if (castingCap.HasValue)
		{
			if (!double.IsFinite(improvement) || improvement <= 0) return false;
			Value = _value + Math.Min(improvement, castingCap.Value - _value);
		}
		else Value += improvement;
        return oldValue != _value;
    }

    public override double MaxValue => _definition.Cap.Evaluate(_owner);

    public override double Value
    {
        get => Math.Min(MaxValue, _value);
		set
		{
			var cap = _owner is ICharacter character ? Gameworld.MagicCasting?.RawSkillImprovementCap(character, Definition.Id) : null;
			// Native lessons and other positive skill writes share the ceiling. Route loss preserves history.
			base.Value = cap.HasValue && value > _value ? Math.Max(_value, Math.Min(value, cap.Value)) : value;
		}
    }
}
