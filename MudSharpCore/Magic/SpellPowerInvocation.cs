#nullable enable

using System.Threading;

namespace MudSharp.Magic;

/// <summary>Bridges void legacy trigger APIs without changing their parsing or spell resolution.</summary>
public sealed class SpellPowerInvocation : IDisposable
{
	private static readonly AsyncLocal<SpellPowerInvocation?> Ambient = new();
	private readonly SpellPowerInvocation? _previous;
	public ICharacter Actor { get; }
	public IMagicSpell Spell { get; }
	public IMagicPower Power { get; }
	public MagicInvocationResult Result { get; private set; } = new(MagicInvocationStatus.Refused);

	public SpellPowerInvocation(ICharacter actor, IMagicSpell spell, IMagicPower power)
	{
		Actor = actor;
		Spell = spell;
		Power = power;
		_previous = Ambient.Value;
		Ambient.Value = this;
	}

	public static SpellPowerInvocation? For(ICharacter actor, IMagicSpell spell)
	{
		var invocation = Ambient.Value;
		return invocation is not null && ReferenceEquals(actor, invocation.Actor) && ReferenceEquals(spell, invocation.Spell)
			? invocation : null;
	}

	public void Complete(MagicInvocationStatus status) => Result = new MagicInvocationResult(status);
	public void Dispose() => Ambient.Value = _previous;
}
