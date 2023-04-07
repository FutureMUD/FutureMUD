using MudSharp.RPG.Checks;

namespace MudSharp.Body.Traits.Improvement;

public class NonImproving : ImprovementModel
{
	public NonImproving()
	{
	}

	public NonImproving(long id)
	{
		_id = id;
	}

	public override double GetImprovement(IHaveTraits person, ITrait trait, Difficulty difficulty, Outcome outcome,
		TraitUseType usetype)
	{
		trait.Gameworld.LogManager.CustomLogEntry(Logging.LogEntryType.SkillImprovement,
			"-- NoGain [Non-Improving Trait]");
		return 0.0;
	}

	/// <inheritdoc />
	public override bool CanImprove(IHaveTraits person, ITrait trait, Difficulty difficulty, TraitUseType useType,
		bool ignoreTemporaryBlockers)
	{
		return false;
	}

	#region Overrides of FrameworkItem

	public override string Name => "Non Improving";

	#endregion
}