#nullable enable

using MudSharp.Body;
using MudSharp.GameItems;
using MudSharp.Health;
using MudSharp.NPC.AI;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic.SpellEffects;

/// <summary>One native injury operation and, at most once, its bounded healing settlement.</summary>
internal sealed class VitalTransferReceipt : IMagicVitalTransferDamage, IDisposable
{
	private readonly ICharacter _source;
	private readonly IBody _sourceBody;
	private readonly ICharacter _victim;
	private readonly IBody _victimBody;
	private readonly IGameItem? _tool;
	private readonly Func<bool> _continuation;
	private bool _closed;
	private bool _invalid;
	private double _delivered;

	internal VitalTransferReceipt(ICharacter source, ICharacter victim, IGameItem? tool = null,
		Func<bool>? continuation = null)
	{
		_source = source;
		_sourceBody = source.Body;
		_victim = victim;
		_victimBody = victim.Body;
		_tool = tool;
		_continuation = continuation ?? (() => true);
		_source.CurrentBodyChanged += BodyChanged;
		_victim.CurrentBodyChanged += BodyChanged;
		_source.OnQuit += Invalidate;
		_source.OnDeleted += Invalidate;
		_victim.OnQuit += Invalidate;
		_victim.OnDeleted += Invalidate;
	}

	private void BodyChanged(ICharacter actor, IBody oldBody, IBody newBody) => _invalid = true;
	private void Invalidate(IPerceivable owner) => _invalid = true;
	public void Dispose()
	{
		_closed = true;
		_source.CurrentBodyChanged -= BodyChanged;
		_victim.CurrentBodyChanged -= BodyChanged;
		_source.OnQuit -= Invalidate;
		_source.OnDeleted -= Invalidate;
		_victim.OnQuit -= Invalidate;
		_victim.OnDeleted -= Invalidate;
	}

	internal double DeliveredDamage => _delivered;
	internal bool IsCurrent => !_invalid && ReferenceEquals(_source.Body, _sourceBody) &&
		ReferenceEquals(_sourceBody.Actor, _source) && ReferenceEquals(_victim.Body, _victimBody) &&
		ReferenceEquals(_victimBody.Actor, _victim) && !ReferenceEquals(_sourceBody, _victimBody);

	private bool Continue()
	{
		// The command policy is callback-capable. Check the raw attachment again afterwards,
		// ending on the captured body references without another policy invocation.
		if (!IsCurrent || !_continuation() || !IsCurrent || !CommandExecutionScope.TryContinue() ||
			!_continuation() || !IsCurrent)
		{
			_invalid = true;
			return false;
		}
		return true;
	}

	public IMagicVitalTransferMeasurement? BeginMeasurement(IBody body, IDamage damage)
	{
		if (_closed || !ReferenceEquals(body, _victimBody) ||
			!ReferenceEquals(damage.VitalTransfer, this) || !ReferenceEquals(damage.ActorOrigin, _source) ||
			!ReferenceEquals(damage.ToolOrigin, _tool)) return null;
		// A matched live packet is about to mutate the captured native body. Returning no
		// measurement here would allow stale harm after the body's own authority callback.
		if (!Continue()) throw new InvalidOperationException("The active vitality transfer lost its exact body or weapon before native injury.");
		var before = new Dictionary<IWound, double>(ReferenceEqualityComparer.Instance);
		foreach (var wound in body.Wounds) before.TryAdd(wound, wound.CurrentDamage);
		return new Measurement(this, before);
	}

	private sealed class Measurement(VitalTransferReceipt receipt, Dictionary<IWound, double> before)
		: IMagicVitalTransferMeasurement
	{
		private bool _completed;
		public void Complete(IEnumerable<IWound> wounds)
		{
			if (_completed) return;
			_completed = true;
			if (receipt._closed || !receipt.IsCurrent) return;
			var delivered = new List<(IWound Wound, double Amount)>();
			foreach (var wound in wounds.Distinct<IWound>(ReferenceEqualityComparer.Instance))
			{
				if (wound.IsFriendlyWound || !receipt._victimBody.Wounds.Any(x => ReferenceEquals(x, wound)) ||
					!(ReferenceEquals(wound.Parent, receipt._victim) || ReferenceEquals(wound.Parent, receipt._victimBody))) continue;
				var previous = before.GetValueOrDefault(wound);
				var current = wound.CurrentDamage;
				if (!double.IsFinite(previous) || previous < 0 || !double.IsFinite(current) || current <= previous) continue;
				var amount = current - previous;
				delivered.Add((wound, amount));
			}
			// Freeze native deltas before a command policy can aggravate these same wounds.
			// Afterwards policy may invalidate the receipt, but cannot enlarge its credit.
			if (!receipt.Continue()) return;
			foreach (var (wound, amount) in delivered)
			{
				if (wound.IsFriendlyWound || !receipt._victimBody.Wounds.Any(x => ReferenceEquals(x, wound)) ||
					!(ReferenceEquals(wound.Parent, receipt._victim) || ReferenceEquals(wound.Parent, receipt._victimBody))) continue;
				if (double.IsFinite(receipt._delivered + amount)) receipt._delivered += amount;
			}
		}
	}

	/// <summary>Consume before any eligibility or health callbacks; stale bodies never redirect healing.</summary>
	internal double Settle(double fraction, double maximum)
	{
		if (_closed) return 0;
		_closed = true;
		if (!double.IsFinite(fraction) || fraction <= 0 || fraction > 1 ||
			!double.IsFinite(maximum) || maximum <= 0 || !Continue()) return 0;
		var budget = Math.Min(_delivered * fraction, maximum);
		if (!double.IsFinite(budget) || budget <= 0) return 0;
		return Heal(_source, _sourceBody, budget, Continue);
	}

	private static double Heal(ICharacter recipient, IBody body, double budget, Func<bool> continuation)
	{
		bool Current() => ReferenceEquals(recipient.Body, body) && ReferenceEquals(body.Actor, recipient) &&
			!recipient.State.HasFlag(CharacterState.Dead) && continuation();
		var healed = 0.0;
		foreach (var wound in body.Wounds.Distinct<IWound>(ReferenceEqualityComparer.Instance)
			.OrderByDescending(x => x.CurrentDamage).ToArray())
		{
			if (budget <= 0 || !Current()) break;
			if (!(ReferenceEquals(wound.Parent, recipient) || ReferenceEquals(wound.Parent, body))) continue;
			var difficulty = wound.CanBeTreated(TreatmentType.Mend);
			if (!Current()) break;
			if (difficulty == Difficulty.Impossible || !body.Wounds.Any(x => ReferenceEquals(x, wound)) ||
				!(ReferenceEquals(wound.Parent, recipient) || ReferenceEquals(wound.Parent, body))) continue;
			var before = wound.CurrentDamage;
			if (!double.IsFinite(before) || before <= 0) continue;
			var amount = Math.Min(budget, before);
			// Reserve the full attempted debit before invoking a native wound setter.
			budget -= amount;
			wound.CurrentDamage = before - amount;
			var actual = before - wound.CurrentDamage;
			if (double.IsFinite(actual)) healed += Math.Clamp(actual, 0, amount);
		}
		if (healed > 0 && Current()) body.EvaluateWounds();
		return healed;
	}
}
