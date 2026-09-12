using System.Threading;
using MudSharp.RPG.Checks;

#nullable enable
namespace MudSharp.Magic.Vancian;

public sealed record SpellTargetResolution(IPerceivable? Target, SpellAdditionalParameter[] Parameters);

/// <summary>Scoped adapter for existing target parsers. Their sole CastSpell call is intercepted before any costs or effects.</summary>
internal sealed class SpellTargetCapture : IDisposable
{
	private static readonly AsyncLocal<SpellTargetCapture?> Current = new();
	private readonly SpellTargetCapture? _previous;
	private readonly ICharacter _actor;
	private readonly IMagicSpell _spell;
	private readonly SpellPower _power;
	public SpellTargetResolution? Resolution { get; private set; }
	private SpellTargetCapture(ICharacter actor, IMagicSpell spell, SpellPower power)
	{ _actor = actor; _spell = spell; _power = power; _previous = Current.Value; Current.Value = this; }
	public static SpellTargetResolution? Resolve(ICharacter actor, IMagicSpell spell, SpellPower power, StringStack targets)
	{
		if (spell.Trigger is not ICastMagicTrigger trigger) return null;
		using var capture = new SpellTargetCapture(actor, spell, power);
		trigger.DoTriggerCast(actor, new StringStack($"{power} {targets.RemainingArgument}"));
		return capture.Resolution;
	}
	public static bool Intercept(ICharacter actor, IMagicSpell spell, IPerceivable? target, SpellPower power, SpellAdditionalParameter[] parameters)
	{
		var capture = Current.Value;
		if (capture is null) return false;
		if (!ReferenceEquals(actor, capture._actor) || !ReferenceEquals(spell, capture._spell) || power != capture._power || capture.Resolution is not null)
			throw new InvalidOperationException("A target parser attempted a secondary or differently priced casting.");
		capture.Resolution = new(target, parameters.ToArray());
		return true;
	}
	public void Dispose() => Current.Value = _previous;
}

internal enum SpellInvocationSource { VancianDirect, ScrollActivation }

/// <summary>Engine-issued commitment callback; there is no player-provided prepaid switch.</summary>
internal sealed class SpellInvocationContext(SpellInvocationSource source, Outcome outcome, Func<Action, bool> commit)
{
	public SpellInvocationSource Source { get; } = source;
	public Outcome Outcome { get; } = outcome;
	public Func<Action, bool> Commit { get; } = commit;
	public MagicInvocationStatus Status { get; set; } = MagicInvocationStatus.Refused;
}
