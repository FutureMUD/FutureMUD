#nullable enable

using MudSharp.Effects;
using MudSharp.Body.Traits;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public partial class MagicSpell
{
	private bool HasCasterLifetimePolicy => _casterSpellEffects.OfType<IMagicSpellEffectLifetimePolicy>().Any(x => x.LifetimePolicy is not null);

	internal string? LifetimeConfigurationError
	{
		get
		{
			var all = _spellEffects.Concat(_casterSpellEffects).ToArray();
			if (all.OfType<SourceWaterBreathingEffect>().FirstOrDefault() is { } water)
			{
				if (water.ConfigurationError is { } waterError) return waterError;
				if (all.Length != 1 || _casterSpellEffects.Any() || !AppliedEffectsAreExclusive ||
					Trigger is not (SpellTriggers.CastingTriggerCharacter or SpellTriggers.CastingTriggerSelf) ||
					GradeProfile?.Area is not null || OpposedTrait is not null)
					return "Source water breathing requires one exclusive target effect, a character/self trigger and no opposed save.";
				if (EffectDurationExpression?.OriginalFormulaText.Trim() != "0")
					return "Source water breathing requires duration expression 0; its immutable source selection supplies the duration.";
				return DetectInvisibleEffect.PolicyError(water.LifetimePolicy!);
			}
			var policies = all.OfType<IMagicSpellEffectLifetimePolicy>().ToArray();
			if (policies.Select(x => x.LifetimePolicyError).FirstOrDefault(x => x is not null) is { } error) return error;
			var active = policies.Where(x => x.LifetimePolicy is not null).ToArray();
			if (active.Length == 0) return null;
			if (active.Length != 1 || all.Length != 1 || all[0] is not DetectInvisibleEffect || !AppliedEffectsAreExclusive)
				return "Accumulated detection requires one detection effect, no other target/caster effects, and exclusive replacement.";
			if (EffectDurationExpression is null) return "Accumulated detection requires a duration expression.";
			if (EffectDurationExpression.NonTraitParameters.Any(x => x.EqualTo("degrees") || x.EqualTo("success")))
				return "Accumulated detection duration cannot depend on an outcome unavailable before payment.";
			return DetectInvisibleEffect.PolicyError(active[0].LifetimePolicy!);
		}
	}

	private sealed record LifetimeMember(MagicSpellParent Parent, Guid Identity, DateTime Expiry,
		MagicSpellLifetimeState State, SpellPower Power, IMagicSpellEffect[] Children);
	private sealed record LifetimeAdmission(IPerceivable Recipient, MagicSpellLifetimePolicy Policy,
		TimeSpan Increment, int Grade, SpellPower Power, LifetimeMember[] Members, SourceWaterBreathingEffect? Water = null);
	private sealed record LifetimeResolution(LifetimeAdmission Admission, TimeSpan Duration, int Grade, SpellPower Power);
	private Dictionary<IPerceivable, LifetimeAdmission>? _pendingLifetimeAdmissions;

	internal void ConfirmPendingLifetimeAdmissions()
	{
		if (_pendingLifetimeAdmissions is null) return;
		foreach (var admission in _pendingLifetimeAdmissions.Values)
		{
			if (admission.Water is { } water) ConfirmWaterConfiguration(water);
			ConfirmLifetimeMembers(admission, admission.Members, new HashSet<MagicSpellParent>(ReferenceEqualityComparer.Instance));
		}
	}

	internal void ValidateLifetimeInvocation(ICharacter caster, IPerceivable recipient)
	{
		if (LifetimeConfigurationError is { } error) throw new InvalidOperationException(error);
		var templates = _spellEffects.Any() ? _spellEffects : _casterSpellEffects;
		var power = InvocationGrade is { } grade && GradeProfile is { } profile
			? profile.Grades.Single(x => x.Grade == grade).Power : SpellPower.Standard;
		CaptureLifetimeAdmission(caster, recipient, templates, power);
	}

	private LifetimeAdmission? CaptureLifetimeAdmission(ICharacter caster, IPerceivable recipient,
		IEnumerable<IMagicSpellEffectTemplate> templates, SpellPower power)
	{
		var policy = templates.OfType<IMagicSpellEffectLifetimePolicy>().Select(x => x.LifetimePolicy).SingleOrDefault(x => x is not null);
		if (policy is null) return null;
		if (LifetimeConfigurationError is { } error) throw new InvalidOperationException(error);
		if (recipient is not ICharacter || InvocationGrade is not { } grade || grade is < 1 or > 7 || !Enum.IsDefined(power))
			throw new InvalidOperationException("Accumulated detection requires a character recipient and an explicit selected source grade.");
		if (templates.OfType<SourceWaterBreathingEffect>().SingleOrDefault() is { } water)
		{
			if ((double)policy.UnitSeconds * policy.MaximumUnits > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds)
				throw new InvalidOperationException("Water breathing lifetime cannot be represented by the native clock.");
			return CaptureWaterLifetimeAdmission(water, caster, recipient, grade, power);
		}
		var seconds = EffectDurationExpression.EvaluateWith(caster, CastingTrait, TraitBonusContext.SpellDuration,
			("grade", grade), ("power", (int)power));
		if (!double.IsFinite(seconds) || seconds <= 0 || seconds > TimeSpan.MaxValue.TotalSeconds ||
			seconds % policy.UnitSeconds != 0 || seconds / policy.UnitSeconds > int.MaxValue)
			throw new InvalidOperationException("Accumulated detection increment must be finite, positive, representable whole source units.");
		var increment = TimeSpan.FromSeconds(seconds);
		if (increment <= TimeSpan.Zero || (double)policy.UnitSeconds * policy.MaximumUnits > (DateTime.MaxValue - RuntimeClock.UtcNow).TotalSeconds)
			throw new InvalidOperationException("Accumulated detection lifetime cannot be represented by the native clock.");
		return new(recipient, policy, increment, grade, power,
			CaptureLifetimeMembers(recipient, policy, new HashSet<MagicSpellParent>(ReferenceEqualityComparer.Instance)));
	}

	private LifetimeMember[] CaptureLifetimeMembers(IPerceivable recipient, MagicSpellLifetimePolicy policy,
		ISet<MagicSpellParent> exclude, SourceWaterBreathingEffect? water = null)
	{
		if (Gameworld.EffectScheduler is not IEffectExpiryObserver observer)
			throw new InvalidOperationException("Accumulated detection requires observable native schedule deadlines.");
		var members = new List<LifetimeMember>();
		foreach (var parent in recipient.Effects.OfType<MagicSpellParent>().Where(x => !exclude.Contains(x)))
		{
			if (parent.LifetimePolicyError is not null && string.IsNullOrEmpty(parent.LifetimeGroup))
				throw new InvalidOperationException("A retained parent has unreadable lifetime grouping metadata.");
			if (parent.LifetimeGroup != policy.Group) continue;
			if (parent.LifetimePolicyError is not null || parent.LifetimeState is not { } state || state.Policy != policy ||
				state.Grade is < 1 or > 7 || !Enum.IsDefined(parent.Power) || parent.Identity == Guid.Empty ||
				!ReferenceEquals(parent.Owner, recipient))
				throw new InvalidOperationException("Conflicting or invalid retained detection lifetime metadata.");
			if (!Gameworld.EffectScheduler.IsScheduled(parent) || observer.ScheduledExpiry(parent) is not { } expiry)
				throw new InvalidOperationException("A grouped detection parent has no proven native expiry; permanent effects cannot be replaced.");
			var children = parent.SpellEffects.ToArray();
			if (children.Length != 1 || (water is null ? children[0] is not SpellDetectInvisibleEffect :
				children[0] is not SpellScopedWaterBreathingEffect scoped || !scoped.MatchesScope(water.Scope!)) ||
				!ReferenceEquals(children[0].ParentEffect, parent) || !recipient.Effects.Contains(children[0]))
				throw new InvalidOperationException("A grouped detection parent has uncertain or mixed child ownership.");
			members.Add(new(parent, parent.Identity, expiry, state, parent.Power, children));
		}
		if (members.Select(x => x.Identity).Distinct().Count() != members.Count)
			throw new InvalidOperationException("Grouped detection parents have duplicate identities.");
		if (members.GroupBy(x => x.State.Grade).Any(x => x.Select(y => y.Power).Distinct().Count() > 1))
			throw new InvalidOperationException("Grouped detection parents disagree on the power retained for one source grade.");
		return members.ToArray();
	}

	private void ConfirmLifetimeMembers(LifetimeAdmission admission, IEnumerable<LifetimeMember> expected,
		ISet<MagicSpellParent> exclude)
	{
		if (admission.Water is { } water) ConfirmWaterConfiguration(water);
		var before = expected.ToArray(); var live = CaptureLifetimeMembers(admission.Recipient, admission.Policy, exclude, admission.Water);
		if (before.Length != live.Length || before.Any(x => !live.Any(y => ReferenceEquals(x.Parent, y.Parent) &&
			x.Identity == y.Identity && x.Expiry == y.Expiry && x.State == y.State && x.Power == y.Power &&
			x.Children.SequenceEqual(y.Children, ReferenceEqualityComparer.Instance))))
			throw new InvalidOperationException("The detection lifetime cohort changed after admission; no guessed replacement is permitted.");
	}

	private LifetimeResolution? ResolveLifetime(LifetimeAdmission? admission, ISet<MagicSpellParent> exclude)
	{
		if (admission is null) return null;
		ConfirmLifetimeMembers(admission, admission.Members, exclude);
		var units = admission.Increment.TotalSeconds / admission.Policy.UnitSeconds;
		var grade = admission.Grade; var power = admission.Power;
		foreach (var member in admission.Members)
		{
			var remaining = Gameworld.EffectScheduler.RemainingDuration(member.Parent);
			if (remaining <= TimeSpan.Zero) continue;
			// The historical expiry is initiation + unit*duration - 1 second;
			// floor(delta/unit)+1 becomes ceiling(remaining/unit) after normalising that inclusive endpoint.
			units = Math.Min(admission.Policy.MaximumUnits, units + Math.Ceiling(remaining.TotalSeconds / admission.Policy.UnitSeconds));
			if (member.State.Grade > grade) { grade = member.State.Grade; power = member.Power; }
		}
		return new(admission, TimeSpan.FromSeconds(Math.Min(units, admission.Policy.MaximumUnits) * admission.Policy.UnitSeconds), grade, power);
	}

	private bool FinaliseLifetimeParent(MagicSpellParent head, LifetimeResolution resolution,
		ISet<MagicSpellParent> parentsAppliedThisCast, bool applicationComplete)
	{
		var admission = resolution.Admission; var recipient = admission.Recipient;
		if (!head.SpellEffects.Any()) return false;
		recipient.AddEffect(head, resolution.Duration);
		parentsAppliedThisCast.Add(head);
		var observer = (IEffectExpiryObserver)Gameworld.EffectScheduler;
		var expiry = observer.ScheduledExpiry(head);
		void ConfirmNewParent()
		{
			if (!recipient.Effects.Contains(head) || expiry is null || !Gameworld.EffectScheduler.IsScheduled(head) ||
				observer.ScheduledExpiry(head) != expiry || head.SpellEffects.Count() != 1 ||
				head.SpellEffects.Any(x => !recipient.Effects.Contains(x) || !ReferenceEquals(x.ParentEffect, head)))
				throw new InvalidOperationException("Replacement detection parent/child attachment or schedule is uncertain.");
		}
		ConfirmNewParent();
		if (!applicationComplete) return false; // Bound a retained partial child; leave the admitted cohort for reconciliation.
		ConfirmLifetimeMembers(admission, admission.Members, parentsAppliedThisCast);
		var remaining = admission.Members.ToList();
		foreach (var member in admission.Members)
		{
			ConfirmLifetimeMembers(admission, remaining, parentsAppliedThisCast); ConfirmNewParent();
			recipient.RemoveEffect(member.Parent, true);
			remaining.Remove(member);
			if (recipient.Effects.Contains(member.Parent) || member.Parent.SpellEffects.Any() ||
				Gameworld.EffectScheduler.IsScheduled(member.Parent) || member.Children.Any(recipient.Effects.Contains))
				throw new InvalidOperationException("Previous detection parent cleanup is uncertain.");
		}
		ConfirmLifetimeMembers(admission, remaining, parentsAppliedThisCast); ConfirmNewParent();
		// Use observed deadlines and source strength, never OriginalDuration or a merely allocated child.
		return admission.Members.Length != 1 || admission.Members[0].Expiry != expiry ||
			admission.Members[0].State.Grade != head.LifetimeState!.Grade || admission.Members[0].Power != head.Power;
	}
}
