#nullable enable

using MudSharp.Body;
using MudSharp.Body.Traits;
using MudSharp.Combat;
using MudSharp.Construction;
using MudSharp.Character;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Effects.Concrete.SpellEffects;
using MudSharp.FutureProg;
using MudSharp.Magic.Emotions;
using MudSharp.Magic.SpellEffects;
using MudSharp.RPG.Checks;

namespace MudSharp.Magic;

public partial class MagicSpell
{
	private SourceEmotionalEffect? EmotionalTemplate => _spellEffects.OfType<SourceEmotionalEffect>().SingleOrDefault();
	private sealed record EmotionalMember(MagicSpellParent Parent, Guid Identity, SpellEmotionalEffect Child,
		EmotionalRetainedState State, DateTime Expiry, bool Applicable);
	private sealed record EmotionalRecipient(ICharacter Character, IBody Body, ICharacter Focus, long Instance, CharacterState State, IRoom Room, RoomLayer Layer,
		ICombat? Combat, IPerceiver? CombatTarget, EmotionalMember[] Members, EmotionalCounter Counter,
		ICombatCessationAdmission? Cessation, EmotionalRetainedState? Incoming);
	private sealed record EmotionalSelection(long SpellId, int Version, int Grade, SpellPower Power,
		ICharacter Caster, IBody Body, IRoom Room, RoomLayer Layer, ITerrain Terrain, ITraitDefinition Trait,
		IFutureProg Eligibility, string ProgText, string CompileError, ProgVariableTypes[] Parameters, XElement Configuration, EmotionalRecipient[] Recipients)
		: IMagicSpellEffectPreparedSelectionToken;
	private sealed record EmotionalResolution(EmotionalSelection Selection, SourceEmotionalEffect Template,
		EmotionalRecipient Recipient, EmotionalMember[] Remaining, bool Changed);

	internal IMagicSpellEffectPreparedSelectionToken CaptureEmotionalSelection(SourceEmotionalEffect effect,
		ICharacter caster, IPerceivable recipient, IMagicSpellEffectPreparedSelectionToken? previous)
	{
		if (LifetimeConfigurationError is { } shapeError) throw new InvalidOperationException(shapeError);
		if (effect.Profile is not { } profile) throw new InvalidOperationException(effect.ProfileError ?? "Configure the source profile.");
		if (recipient is not ICharacter character || InvocationGrade is not { } grade || grade is < 1 or > 7 || GradeProfile is null)
			throw new InvalidOperationException("Source emotions require a character recipient and an explicit source grade.");
		profile.ValidateBindings(Gameworld);
		var power = GradeProfile.Grades.Single(x => x.Grade == grade).Power;
		if (previous is not null)
		{
			if (previous is not EmotionalSelection selected || selected.SpellId != Id || selected.Version != GradeProfile.Version ||
				selected.Grade != grade || selected.Power != power || !ReferenceEquals(selected.Caster, caster) ||
				!selected.Recipients.Any(x => ReferenceEquals(x.Character, character)))
				throw new InvalidOperationException("The emotional selection binding changed after preparation.");
			ConfirmEmotionalPolicy(selected, effect);
			foreach (var bound in selected.Recipients.Where(x => x.Counter.Continue)) ResolveEmotionalLifetime(selected, bound, profile);
			return selected;
		}
		if (Gameworld.EffectScheduler is not IEffectExpiryObserver || caster.Body is null || caster.Location is null)
			throw new InvalidOperationException("Source emotions require native bodies, terrain and observable expiry deadlines.");
		var terrain = caster.Location.Terrain(caster);
		if (terrain is null || terrain.Id <= 0 || !ReferenceEquals(Gameworld.Terrains.Get(terrain.Id), terrain))
			throw new InvalidOperationException("The caster has no current native terrain mapping.");
		var bounds = new[] { character, caster }.Distinct(ReferenceEqualityComparer.Instance).Cast<ICharacter>()
			.Select(target => CaptureEmotionalRecipient(target, profile, grade)).ToArray();
		var trait = Gameworld.Traits.Get(profile.TraitId) ?? throw new InvalidOperationException("The mapped emotional trait disappeared.");
		var eligibility = Gameworld.FutureProgs.Get(profile.EligibilityProgId) ?? throw new InvalidOperationException("The emotional eligibility prog disappeared.");
		var token = new EmotionalSelection(Id, GradeProfile.Version, grade, power, caster, caster.Body, caster.Location,
			caster.RoomLayer, terrain, trait, eligibility, eligibility.FunctionText, eligibility.CompileError,
			eligibility.Parameters.ToArray(), effect.SaveToXml(), bounds);
		// Run all callbacks before drawing. Rechecking raw ownership afterwards prevents a callback from replacing a cohort.
		ConfirmEmotionalPolicy(token, effect);
		foreach (var bound in bounds.Where(x => x.Counter.Continue))
		{
			var endurance = profile.Kind == EmotionalSpellKind.Fury
				? EmotionalSpellPolicy.RollEndurance(bound.Counter.RemainingIncomingGrade, profile.TerrainRule(terrain.Id), RandomUtilities.Random) : 0;
			var index = Array.IndexOf(bounds, bound);
			bounds[index] = bound with { Incoming = new(bound.Counter.RemainingIncomingGrade, power, profile.Intensity, endurance) };
			// Validate retained arithmetic, including conflicting equal-grade Calm mappings, before any debit.
			ResolveEmotionalLifetime(token, bounds[index], profile);
		}
		ConfirmEmotionalStructure(token, effect);
		return token;
	}

	private EmotionalRecipient CaptureEmotionalRecipient(ICharacter target, EmotionalStockProfile profile, int grade)
	{
		if (target.Body is null || target.Location is null) throw new InvalidOperationException("An emotional recipient has no native body/location.");
		var members = CaptureEmotionalMembers(target);
		if (members.Any(x => !x.Applicable))
			throw new InvalidOperationException("Conditional retained source emotions require explicit resolution before casting.");
		var same = members.SingleOrDefault(x => x.Applicable && x.Child.Kind == profile.Kind);
		if (same is not null && (same.Child.Group != profile.Group || same.Child.UnitSeconds != profile.UnitSeconds || same.Child.CapUnits != profile.CapUnits ||
			profile.Kind == EmotionalSpellKind.Fury && same.Child is SpellSourceFuryEffect fury &&
				(!ReferenceEquals(fury.EnduranceTrait, Gameworld.Traits.Get(profile.TraitId)) || fury.UnitsPerSourcePoint != profile.UnitsPerSourcePoint)))
			throw new InvalidOperationException("The retained source emotion has a different group or native lifetime/attribute mapping.");
		var counter = EmotionalSpellPolicy.Counter(grade, members.SingleOrDefault(x => x.Applicable && x.Child.Kind != profile.Kind)?.State.SourceGrade);
		var combat = target.Combat;
		var opponent = target.CombatTarget;
		ICombatCessationAdmission? cessation = null;
		if (profile.Kind == EmotionalSpellKind.Calm && counter.Continue && combat is not null)
		{
			cessation = (combat as ICombatSelectiveCessation)?.PrepareCessation(target, opponent, combat);
			if (cessation?.IsCurrent != true)
				throw new InvalidOperationException("Calm requires supported selective combat cessation before payment.");
		}
		return new(target, target.Body, target.Body.Actor, target.InstanceId, target.State, target.Location, target.RoomLayer, combat, opponent, members, counter, cessation, null);
	}

	private EmotionalMember[] CaptureEmotionalMembers(ICharacter target)
	{
		if (Gameworld.EffectScheduler is not IEffectExpiryObserver observer)
			throw new InvalidOperationException("Source emotions require observable native expiry deadlines.");
		var children = target.Effects.OfType<SpellEmotionalEffect>().ToArray();
		var parents = target.Effects.OfType<MagicSpellParent>().Where(x => x.SpellEffects.OfType<SpellEmotionalEffect>().Any()).ToArray();
		if (children.Length != parents.Length || children.GroupBy(x => x.Kind).Any(x => x.Count() > 1) || parents.Select(x => x.Identity).Distinct().Count() != parents.Length)
			throw new InvalidOperationException("The source emotional cohort has duplicate, orphaned or mixed ownership.");
		return children.Select(child =>
		{
			if (child.DefinitionError is not null || child.ParentEffect is not MagicSpellParent parent || !parents.Contains(parent) ||
				!ReferenceEquals(child.Owner, target) || !ReferenceEquals(parent.Owner, target) || parent.Identity == Guid.Empty ||
				parent.LifetimeState is not null || parent.LifetimePolicyError is not null || parent.Power != child.State.Power ||
				parent.SpellEffects.Count() != 1 || !parent.SpellEffects.Contains(child) ||
				!Gameworld.EffectScheduler.IsScheduled(parent) || observer.ScheduledExpiry(parent) is not { } expiry ||
				expiry <= RuntimeClock.UtcNow || (expiry - RuntimeClock.UtcNow).TotalSeconds > (double)child.UnitSeconds * child.CapUnits)
				throw new InvalidOperationException("The retained source emotion has invalid state, child ownership or expiry.");
			return new EmotionalMember(parent, parent.Identity, child, child.State, expiry, child.Applies());
		}).ToArray();
	}

	private void ConfirmEmotionalPolicy(EmotionalSelection token, SourceEmotionalEffect effect)
	{
		ConfirmEmotionalStructure(token, effect);
		if (!ReferenceEquals(token.Room.Terrain(token.Caster), token.Terrain))
			throw new InvalidOperationException("The emotional caster terrain changed after preparation.");
		foreach (var bound in token.Recipients)
		{
			if (!token.Eligibility.ExecuteWithStatus(out var permitted, bound.Character, token.Caster) || permitted is not true)
				throw new InvalidOperationException("The source emotional eligibility prog refused or failed.");
			foreach (var member in bound.Members)
				if (member.Child.Applies() != member.Applicable)
					throw new InvalidOperationException("Source emotional applicability changed after preparation.");
		}
		ConfirmEmotionalStructure(token, effect);
	}

	// This audit is also used inside Pay, after the casting service's final policy/capacity fence.
	// It must never execute a prog, applicability, perception or resource-capacity callback.
	private void ConfirmEmotionalStructure(EmotionalSelection token, SourceEmotionalEffect effect)
	{
		ConfirmEmotionalBindings(token, effect);
		foreach (var bound in token.Recipients) ConfirmEmotionalRecipient(bound, bound.Members);
	}

	private void ConfirmEmotionalBindings(EmotionalSelection token, SourceEmotionalEffect effect)
	{
		var profile = effect.Profile!;
		if (!XNode.DeepEquals(token.Configuration, effect.SaveToXml()) ||
			!ReferenceEquals(token.Body, token.Caster.Body) || !ReferenceEquals(token.Room, token.Caster.Location) || token.Layer != token.Caster.RoomLayer ||
			!ReferenceEquals(token.Terrain, Gameworld.Terrains.Get(token.Terrain.Id)) ||
			!ReferenceEquals(token.Trait, Gameworld.Traits.Get(profile.TraitId)) ||
			!ReferenceEquals(token.Eligibility, Gameworld.FutureProgs.Get(profile.EligibilityProgId)) || token.ProgText != token.Eligibility.FunctionText ||
			token.Eligibility.ReturnType != ProgVariableTypes.Boolean || token.CompileError != token.Eligibility.CompileError ||
			!token.Parameters.SequenceEqual(token.Eligibility.Parameters) || profile.Kind == EmotionalSpellKind.Fury && token.Trait.TraitType != TraitType.Attribute)
			throw new InvalidOperationException("The source emotional body, terrain, trait or profile binding changed.");
	}

	private void ConfirmEmotionalRecipient(EmotionalRecipient bound, EmotionalMember[] members, MagicSpellParent? added = null, bool afterCessation = false)
	{
		var target = bound.Character;
		if (!ReferenceEquals(bound.Body, target.Body) || !ReferenceEquals(bound.Focus, target.Body.Actor) || bound.Instance != target.InstanceId ||
			bound.State != target.State || !ReferenceEquals(bound.Room, target.Location) || bound.Layer != target.RoomLayer ||
			!afterCessation && (!ReferenceEquals(bound.Combat, target.Combat) || !ReferenceEquals(bound.CombatTarget, target.CombatTarget) ||
				bound.Cessation is not null && !bound.Cessation.IsCurrent))
			throw new InvalidOperationException("The admitted emotional recipient or combat pair changed.");
		var observer = (IEffectExpiryObserver)Gameworld.EffectScheduler;
		var live = target.Effects.OfType<SpellEmotionalEffect>().Where(x => !ReferenceEquals(x.ParentEffect, added)).ToArray();
		var parents = target.Effects.OfType<MagicSpellParent>().Where(x => !ReferenceEquals(x, added) && x.SpellEffects.OfType<SpellEmotionalEffect>().Any()).ToArray();
		if (live.Length != members.Length || parents.Length != members.Length || members.Any(x =>
			!live.Contains(x.Child) || !parents.Contains(x.Parent) || !ReferenceEquals(x.Child.ParentEffect, x.Parent) ||
			x.Parent.Identity != x.Identity || !ReferenceEquals(x.Parent.Owner, target) || !ReferenceEquals(x.Child.Owner, target) ||
			x.Child.DefinitionError is not null || x.Child.State != x.State || x.Parent.Power != x.State.Power ||
			x.Parent.SpellEffects.Count() != 1 || !x.Parent.SpellEffects.Contains(x.Child) ||
			!Gameworld.EffectScheduler.IsScheduled(x.Parent) || observer.ScheduledExpiry(x.Parent) != x.Expiry || x.Expiry <= RuntimeClock.UtcNow))
			throw new InvalidOperationException("The admitted source emotional cohort changed; replacement state is preserved.");
	}

	private void PrepareEmotionalInvocation(ICharacter caster, IPerceivable? target, SpellPower power)
	{
		if (EmotionalTemplate is not { } effect) return;
		if (target is not ICharacter || InvocationGrade is null) throw new InvalidOperationException(SourceEmotionalEffect.RuntimeIntegrationError);
		var token = (EmotionalSelection)effect.CapturePreparedSelection(caster, target);
		if (token.Power != power) throw new InvalidOperationException("Emotional invocation power differs from its selected source grade.");
	}

	private void ConfirmPendingEmotionalAdmissions()
	{
		if (EmotionalTemplate is { } effect)
		{
			if (effect.Selection is not EmotionalSelection token) throw new InvalidOperationException(SourceEmotionalEffect.RuntimeIntegrationError);
			ConfirmEmotionalStructure(token, effect);
			foreach (var bound in token.Recipients.Where(x => x.Counter.Continue)) ResolveEmotionalLifetime(token, bound, effect.Profile!);
		}
	}

	private EmotionalResolution BeginEmotionalResolution(ICharacter caster, ICharacter recipient)
	{
		var template = EmotionalTemplate!;
		var token = (EmotionalSelection)template.Selection!;
		ConfirmEmotionalPolicy(token, template);
		var bound = token.Recipients.Single(x => ReferenceEquals(x.Character, recipient));
		var members = bound.Members.ToList();
		var opposing = members.SingleOrDefault(x => x.Applicable && x.Child.Kind != template.Kind);
		if (opposing is null) return new(token, template, bound, members.ToArray(), false);
		if (bound.Counter.RemoveOpposing)
		{
			recipient.RemoveEffect(opposing.Parent, true);
			members.Remove(opposing);
			if (recipient.Effects.Contains(opposing.Parent) || recipient.Effects.Contains(opposing.Child) ||
				opposing.Parent.SpellEffects.Any() || Gameworld.EffectScheduler.IsScheduled(opposing.Parent))
				throw new InvalidOperationException("Source emotion counter removal was not confirmed.");
		}
		else
		{
			opposing.Child.ReduceSourceGrade(token.Grade);
			members[members.IndexOf(opposing)] = opposing with { State = opposing.Child.State };
		}
		ConfirmEmotionalRecipient(bound, members.ToArray());
		return new(token, template, bound, members.ToArray(), true);
	}

	private EmotionalLifetime ResolveEmotionalLifetime(EmotionalSelection token, EmotionalRecipient bound, EmotionalStockProfile profile)
	{
		var old = bound.Members.SingleOrDefault(x => x.Applicable && x.Child.Kind == profile.Kind);
		var lifetime = EmotionalSpellPolicy.ResolveLifetime(profile.Kind, bound.Counter.RemainingIncomingGrade, profile.TerrainRule(token.Terrain.Id),
			profile.UnitSeconds, profile.CapUnits, bound.Incoming!, old is null ? null : old.Expiry - RuntimeClock.UtcNow, old?.State);
		if (lifetime.Duration > DateTime.MaxValue - RuntimeClock.UtcNow)
			throw new InvalidOperationException("The selected emotional lifetime cannot be represented by the native clock.");
		return lifetime;
	}

	private MagicEffectOperation CompleteEmotionalResolution(EmotionalResolution phase, bool saved, OpposedOutcomeDegree outcome)
	{
		var bound = phase.Recipient; var target = bound.Character; var profile = phase.Template.Profile!;
		var members = phase.Remaining.ToList(); var changed = phase.Changed; var ceased = false;
		ConfirmEmotionalBindings(phase.Selection, phase.Template);
		ConfirmEmotionalRecipient(bound, members.ToArray());
		if (!bound.Counter.Continue) return new(changed ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		// A resisted Calm still performs the captured selective cessation. Never recapture combat after payment.
		if (profile.Kind == EmotionalSpellKind.Calm && bound.Cessation is not null)
		{
			var changes = ((ICombatSelectiveCessation)bound.Combat!).CeaseCombatFor(bound.Cessation);
			if (!changes.HasFlag(CombatCessationChanges.SubjectRemoved))
				throw new InvalidOperationException("The admitted Calm cessation was not confirmed.");
			ceased = true; changed = true;
			// The cessation hook may deliberately join replacement combat in a callback. Preserve it and stop here.
			if (target.Combat is not null || target.CombatTarget is not null)
				throw new InvalidOperationException("Calm cessation callbacks replaced combat; no emotional child was applied to the new fight.");
			ConfirmEmotionalRecipient(bound, members.ToArray(), afterCessation: true);
		}
		if (saved) return new(changed ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.Rejected, null);
		var lifetime = ResolveEmotionalLifetime(phase.Selection, bound, profile);
		var old = members.SingleOrDefault(x => x.Applicable && x.Child.Kind == profile.Kind);
		if (old is not null && profile.Kind == EmotionalSpellKind.Calm && old.State == lifetime.State &&
			old.Child is SpellSourceCalmEffect calm && calm.BreakOnAdmittedAttack == profile.BreakOnAdmittedAttack &&
			old.Expiry == RuntimeClock.UtcNow + lifetime.Duration)
			return new(changed ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		if (old is not null && profile.Kind == EmotionalSpellKind.Fury)
		{
			// Fury retains the exact existing child and bonus. A remove/re-add would transiently clamp stamina.
			Gameworld.EffectScheduler.Reschedule(old.Parent, lifetime.Duration);
			var expiry = ((IEffectExpiryObserver)Gameworld.EffectScheduler).ScheduledExpiry(old.Parent);
			if (expiry is null) throw new InvalidOperationException("The Fury recast deadline was not confirmed.");
			members[members.IndexOf(old)] = old with { Expiry = expiry.Value };
			ConfirmEmotionalRecipient(bound, members.ToArray(), afterCessation: ceased);
			target.EffectsChanged = true;
			return new(changed || expiry != old.Expiry ? MagicEffectOperationStatus.Applied : MagicEffectOperationStatus.NoChange, null);
		}
		var head = new MagicSpellParent(target, this, phase.Selection.Caster, lifetime.State.Power, outcome) { ResolvedDuration = lifetime.Duration };
		SpellEmotionalEffect child = profile.Kind == EmotionalSpellKind.Fury
			? new SpellSourceFuryEffect(target, head, profile.Group, profile.UnitSeconds, profile.CapUnits, lifetime.State, phase.Selection.Trait, profile.UnitsPerSourcePoint)
			: new SpellSourceCalmEffect(target, head, profile.Group, profile.UnitSeconds, profile.CapUnits, lifetime.State, profile.BreakOnAdmittedAttack);
		// Attach the scheduled owner first, so even a child callback failure leaves a bounded retained wrapper.
		DateTime? initialExpiry;
		try
		{
			target.AddEffect(head, lifetime.Duration);
			initialExpiry = ((IEffectExpiryObserver)Gameworld.EffectScheduler).ScheduledExpiry(head);
			var initialRemaining = Gameworld.EffectScheduler.RemainingDuration(head);
			if (initialExpiry is null || initialRemaining <= TimeSpan.Zero || initialRemaining > lifetime.Duration ||
				lifetime.Duration - initialRemaining > TimeSpan.FromSeconds(1))
				throw new InvalidOperationException("The source emotional schedule did not retain the resolved duration.");
			ConfirmEmotionalBindings(phase.Selection, phase.Template);
			ConfirmEmotionalRecipient(bound, members.ToArray(), head, ceased);
			target.AddEffect(child);
			if (target.Effects.Contains(child) && !head.SpellEffects.Contains(child)) head.AddSpellEffect(child);
			target.EffectsChanged = true;
		}
		catch
		{
			// A wrapper must never serialize a child that did not attach: loading it would replay a failed cast.
			if (target.Effects.Contains(child))
			{
				if (!head.SpellEffects.Contains(child)) head.AddSpellEffect(child);
				target.EffectsChanged = true;
			}
			else
			{
				head.RemoveSpellEffect(child);
				if (target.Effects.Contains(head)) target.RemoveEffect(head);
			}
			throw;
		}
		void ConfirmNew()
		{
			if (!target.Effects.Contains(head) || !target.Effects.Contains(child) || !ReferenceEquals(child.ParentEffect, head) ||
				head.SpellEffects.Count() != 1 || !head.SpellEffects.Contains(child) || !Gameworld.EffectScheduler.IsScheduled(head) ||
				((IEffectExpiryObserver)Gameworld.EffectScheduler).ScheduledExpiry(head) is not { } expiry ||
				expiry != initialExpiry)
				throw new InvalidOperationException("The source emotional parent, child or scheduled deadline was not confirmed.");
		}
		ConfirmNew(); ConfirmEmotionalRecipient(bound, members.ToArray(), head, ceased);
		if (old is not null)
		{
			target.RemoveEffect(old.Parent, true); members.Remove(old);
			if (target.Effects.Contains(old.Parent) || target.Effects.Contains(old.Child) || old.Parent.SpellEffects.Any() || Gameworld.EffectScheduler.IsScheduled(old.Parent))
				throw new InvalidOperationException("Previous Calm child cleanup was not confirmed.");
		}
		ConfirmNew(); ConfirmEmotionalRecipient(bound, members.ToArray(), head, ceased);
		return new(MagicEffectOperationStatus.Applied, child);
	}
}
