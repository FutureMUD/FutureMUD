using MudSharp.Body;
using MudSharp.Effects.Concrete;
using MudSharp.RPG.Checks;
using MudSharp.NPC.AI;

namespace MudSharp.Combat.Moves;

public abstract class CombatMoveBase : ICombatMove
{
    private ICharacter _assailant;
	private bool _executionRejected;
	internal void RejectUnexecutedCommand() => _executionRejected = true;
	protected bool CanContinueCommand() => CommandExecutionScope.TryContinue();
	protected CombatMoveResult RefusedContinuationResult() => CommandExecutionScope.HasCommitted
		? new CombatMoveResult() : CombatMoveResult.Irrelevant;

	protected bool ApplyOwnedMutation(ICharacter executor, Action mutation)
	{
		using var owned = CommandExecutionScope.EnterOwnedOperation(executor);
		if (!CommandExecutionScope.TryContinue()) return false;
		CommandExecutionScope.MarkCommitted();
		mutation();
		return true;
	}

	// Internal defender selection can run authored progs after CombatBase's outer gate.
	protected bool CanContinueAfterInternalResponse(ICombat combat)
	{
		if (_executionRejected) return false;
		bool Participating() => ReferenceEquals(Assailant.Combat, combat) &&
			combat.Combatants.Any(x => ReferenceEquals(x, Assailant));
		if (Participating() && CommandExecutionAuthority.MayExecute(this, Assailant) && Participating()) return true;
		_executionRejected = true;
		return false;
	}

	protected IEnumerable<ICombatMove> InternalResponses(ICombat combat)
	{
		// A response can remove combatants; never retain a live collection enumerator across it.
		foreach (var target in combat.Combatants.Where(x => x.CombatTarget == Assailant).ToArray())
		{
			var response = target.ResponseToMove(this, Assailant);
			if (!CanContinueAfterInternalResponse(combat)) yield break;
			if (response is not null) yield return response;
		}
	}

    protected HashSet<ICharacter> _characterTargets = new();

    protected HashSet<IPerceiver> _targets = new();

    public ICharacter Assailant
    {
        get => _assailant;
        init
        {
            _assailant = value;
            if (value.CombatTarget is ICharacter item && !_characterTargets.Contains(item))
            {
                _characterTargets.Add(item);
            }
        }
    }

    public virtual ExertionLevel AssociatedExertion { get; } = ExertionLevel.Rest;

    public virtual CheckType Check { get; } = CheckType.CombatMoveCheck;

    public virtual Difficulty CheckDifficulty { get; } = Difficulty.Normal;

    public abstract string Description { get; }

    public virtual Difficulty RecoveryDifficultyFailure { get; } = Difficulty.Hard;

    public virtual Difficulty RecoveryDifficultySuccess { get; } = Difficulty.Normal;

    public virtual double StaminaCost { get; } = 0;

    public abstract CombatMoveResult ResolveMove(ICombatMove defenderMove);

    public IEnumerable<ICharacter> CharacterTargets => _characterTargets;

    public IEnumerable<IPerceiver> Targets => _targets;

#nullable enable
    protected ICharacter? PrimaryCharacterTarget => PrimaryTarget as ICharacter ?? CharacterTargets.FirstOrDefault();
#nullable restore

    public virtual IPerceiver PrimaryTarget
    {
        get => _targets.FirstOrDefault() ??
               Assailant.CombatTarget ??
               Assailant.Combat.Combatants.FirstOrDefault(x => x.CombatTarget == Assailant)
               ;
        set
        {
            _targets.Add(value);
            if (value is ICharacter ch)
            {
                _characterTargets.Add(ch);
            }
        }
    }

    public IFuturemud Gameworld => Assailant.Gameworld;

    public virtual double BaseDelay { get; } = 0.2;

    public override string ToString()
    {
        return Description;
    }

    protected int GetPositionPenalty(Facing facing)
    {
        switch (facing)
        {
            case Facing.Front:
                return 0;
            case Facing.LeftFlank:
            case Facing.RightFlank:
                return 2;
            case Facing.Rear:
                return 4;
        }

        return 0;
    }

    protected void WorsenCombatPosition(ICharacter assailant, ICharacter defender)
    {
        CombatPositioningUtilities.WorsenCombatPosition(assailant, defender);
    }

    protected void ImproveCombatPosition(ICharacter assailant, ICharacter defender)
    {
        CombatPositioningUtilities.ImproveCombatPosition(assailant, defender);
    }

    public virtual bool UsesStaminaWithResult(CombatMoveResult result)
    {
        return !_executionRejected;
    }
}
