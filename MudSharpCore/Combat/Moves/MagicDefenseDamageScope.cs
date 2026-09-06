#nullable enable

using System.Threading;
using MudSharp.Health;

namespace MudSharp.Combat.Moves;

/// <summary>Limits barrier absorption to damage delivered by the selected attack, including ammunition callbacks.</summary>
internal sealed class MagicDefenseDamageScope : IDisposable
{
	private static readonly AsyncLocal<MagicDefenseDamageScope?> Current = new();
	private readonly MagicDefenseDamageScope? _previous;
	private MagicDefenseMove? _defense;
	private readonly ICharacter _attacker;
	private bool _absorbed;
	private bool _passed;
	public MagicDefenseDamageScope(ICombatMove attack, ICombatMove? defense)
	{
		_previous = Current.Value;
		_defense = defense as MagicDefenseMove;
		_attacker = attack.Assailant;
		Current.Value = this;
	}
	internal static void Bind(MagicDefenseMove defense) { if (Current.Value is { } scope) scope._defense = defense; }
	public static IDamage? Filter(ICharacter target, IDamage? damage)
	{
		if (damage is null) return null;
		var scope = Current.Value;
		if (scope?._defense is not { } defense || target != defense.Assailant || damage.ActorOrigin != scope._attacker) return damage;
		var result = defense.Absorb(damage);
		if (result is null) scope._absorbed = true; else scope._passed = true;
		return result;
	}
	public CombatMoveResult Finish(CombatMoveResult result)
	{
		if (_absorbed && !_passed) return new CombatMoveResult { MoveWasSuccessful = false,
			AttackerOutcome = result.AttackerOutcome, DefenderOutcome = result.DefenderOutcome,
			RecoveryDifficulty = result.RecoveryDifficulty, WoundsCaused = result.WoundsCaused, SelfWoundsCaused = result.SelfWoundsCaused };
		return result;
	}
	public void Dispose() => Current.Value = _previous;
}
