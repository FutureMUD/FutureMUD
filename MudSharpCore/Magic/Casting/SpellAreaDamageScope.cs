using System.Threading;

#nullable enable
namespace MudSharp.Magic.Casting;

/// <summary>Attenuates only this invocation's native damage application, without mutating templates.</summary>
internal sealed class SpellAreaDamageScope : IDisposable
{
	private static readonly AsyncLocal<SpellAreaDamageScope?> Current = new();
	private readonly SpellAreaDamageScope? _prior;
	private readonly ICharacter _caster;
	private readonly IPerceivable _target;
	private readonly IMagicSpell _spell;
	private readonly double _multiplier;
	public SpellAreaDamageScope(ICharacter caster, IPerceivable target, IMagicSpell spell, double multiplier)
	{
		_prior = Current.Value;
		_caster = caster; _target = target; _spell = spell; _multiplier = multiplier;
		Current.Value = this;
	}
	public static double ScaleDamage(ICharacter caster, IPerceivable target, IMagicSpell spell, double amount)
	{
		if (Current.Value is not { } scope || !ReferenceEquals(scope._caster, caster) || !ReferenceEquals(scope._target, target) ||
			!ReferenceEquals(scope._spell, spell)) return amount;
		var scaled = amount * scope._multiplier;
		if (!double.IsFinite(scaled) || scaled < 0) throw new InvalidOperationException("Resolved area spell damage must be finite and non-negative.");
		return scaled;
	}
	public void Dispose() => Current.Value = _prior;
}
