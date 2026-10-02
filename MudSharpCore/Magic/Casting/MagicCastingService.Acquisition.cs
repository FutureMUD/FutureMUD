using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private readonly Dictionary<long, HashSet<(long Capability, long Spell)>> _spellEdges = [];
	private readonly Dictionary<long, HashSet<(long Capability, long Spell)>> _traitEdges = [];
	private readonly Dictionary<long, HashSet<long>> _skillCapPolicies = [];
	private readonly HashSet<long> _cappedSkills = [];
	private bool _indexed;
	private readonly System.Collections.Concurrent.ConcurrentDictionary<long, byte> _evaluatingProgress = new();
	public void DefinitionsChanged()
	{
		_indexed = false;
		foreach (var actor in _world.Characters.DistinctBy(x => Owner(x).Id))
			Reconcile(actor);
	}

	public MagicCastingGrant Grant(ICharacter authority, ICharacter target, long capabilityId, long spellId, string reason)
		=> authority.IsAdministrator() && !string.IsNullOrWhiteSpace(reason) ? GrantAuthorised(target, capabilityId, spellId, $"Staff {authority.Id}: {reason}") :
			new(false, false, "Only staff or an authored grant Prog may acquire a configured spell.");

	internal MagicCastingGrant GrantAuthorised(ICharacter target, long capabilityId, long spellId, string provenance)
	{
		var result = MutateGrant(target, () => GrantCore(target, capabilityId, spellId, provenance));
		if (result.Allowed) { Reconcile(target); NotifyProgress(target, spellId: spellId); }
		return result;
	}

	private MagicCastingGrant MutateGrant(ICharacter target, Func<MagicCastingGrant> action)
	{
		lock (Guard(target))
		{
			var owner = Owner(target).Id;
			if (!_mutating.TryAdd(owner, 0)) return new(false, false, "A casting mutation is active for this identity.");
			try { return action(); }
			catch (Exception ex) { return new(false, false, $"Grant persistence failed; inspect the durable state and retry the idempotent grant: {ex.GetBaseException().Message}"); }
			finally { _mutating.TryRemove(owner, out _); }
		}
	}

	private MagicCastingGrant GrantCore(ICharacter target, long capabilityId, long spellId, string provenance)
	{
		if (string.IsNullOrWhiteSpace(provenance) || provenance.Length > 2048) return new(false, false, "Supply a nonempty provenance of at most 2048 characters.");
		if (_world.MagicCapabilities.Get(capabilityId) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p ||
			!target.Capabilities.Any(x => x.Id == capabilityId) || p.Admissions.FirstOrDefault(x => x.SpellId == spellId) is not { } admission)
			return new(false, false, "The target needs a current enabled capability explicitly admitting that spell.");
		if (c.CastingConfigurationErrors().FirstOrDefault() is { } error) return new(false, false, error);
		var owner = Owner(target);
		var trait = _world.Traits.Get(admission.TraitId ?? p.DefaultTraitId)!;
		if (QuarantineReason(target, spellId, trait.Id, p.ReserveResourceId) is { } q) return new(false, false, q);
		var spell = (IControlledMagicSpell)_world.MagicSpells.Get(spellId)!;
		var existing = Acquisition(target, spellId);
		// Persist authorisation before opening the skill. A retry repairs a missing skill after a failed native save;
		// native skill presence can never serve as an implicit grant after a failed acquisition write.
		RecordSkillCap(target, c, admission, existing is null
			? new(owner.Id, spellId, 1, spell.GradeProfile!.Version, _clock(), provenance, DateTime.UnixEpoch, 0) : null);
		if (!owner.HasTrait(trait)) { owner.AddTrait(trait, admission.OpeningSkill ?? spell.GradeProfile!.OpeningSkill); Flush(target); }
		if (existing is not null) return new(false, true, "Already acquired; proficiency is retained.");
		return new(true, true, "Spell acquired at controlled grade 1.");
	}

	public MagicCastingGrant Enrol(ICharacter authority, ICharacter target, long capabilityId, string reason)
	{
		if (!authority.IsAdministrator() || string.IsNullOrWhiteSpace(reason)) return new(false, false, "Explicit staff authority and a reason are required for enrolment.");
		return MutateGrant(target, () =>
		{
			var owner = Owner(target);
			if (_world.MagicCapabilities.Get(capabilityId) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p ||
				!target.Capabilities.Any(x => x.Id == capabilityId)) return new(false, false, "The target needs the current enabled capability.");
			if (c.CastingConfigurationErrors().FirstOrDefault() is { } error) return new(false, false, error);
			var previous = _store.Enrolment(owner.Id, p.Identity);
			if ((previous?.StartingVersion ?? 0) >= p.StartingGrantVersion) return new(false, true, "Already enrolled; starting grants are not repeated.");
			var pending = previous ?? new CastingEnrolment(owner.Id, p.Identity, c.Id, _clock(), 0);
			_store.Write(enrolment: pending);
			foreach (var root in p.Admissions.Where(x => x.Starting))
			{
				var grant = GrantCore(target, c.Id, root.SpellId, $"Enrolment {p.Identity}, staff {authority.Id}: {reason}");
				if (!grant.Allowed) return grant;
			}
			_store.Write(enrolment: pending with { StartingVersion = p.StartingGrantVersion });
			Reconcile(target);
			EvaluateEdges(target, p.Admissions.Where(x => x.Prerequisites.Count > 0).Select(x => (c.Id, x.SpellId)));
			return new(true, true, "Enrolled; starting grants are durably recorded once.");
		});
	}

	private void EnsureIndex()
	{
		if (_indexed) return;
		_spellEdges.Clear(); _traitEdges.Clear(); _skillCapPolicies.Clear(); _cappedSkills.Clear();
		foreach (var c in _world.MagicCapabilities.OfType<IMagicCastingCapability>().Where(x => x.CastingPolicy is not null))
		{
			var p = c.CastingPolicy!;
			foreach (var admission in p.Admissions)
			{
				var traitId = admission.TraitId ?? p.DefaultTraitId;
				if (!_skillCapPolicies.TryGetValue(traitId, out var policies)) _skillCapPolicies[traitId] = policies = [];
				policies.Add(c.Id);
				if (admission.RawSkillCap.HasValue) _cappedSkills.Add(traitId);
			}
			foreach (var a in p.Admissions)
				foreach (var e in a.Prerequisites)
				{
					if (!_spellEdges.TryGetValue(e.SpellId, out var spells)) _spellEdges[e.SpellId] = spells = [];
					spells.Add((c.Id, a.SpellId));
					var prior = p.Admissions.FirstOrDefault(x => x.SpellId == e.SpellId);
					if (prior is null) continue;
					var traitId = prior.TraitId ?? p.DefaultTraitId;
					if (!_traitEdges.TryGetValue(traitId, out var traits)) _traitEdges[traitId] = traits = [];
					traits.Add((c.Id, a.SpellId));
				}
		}
		_indexed = true;
	}

	public void NotifyProgress(ICharacter character, long? traitId = null, long? spellId = null)
	{
		if (_mutating.ContainsKey(Owner(character).Id)) return;
		EnsureIndex();
		var affected = (spellId.HasValue ? _spellEdges.GetValueOrDefault(spellId.Value) ?? [] : [])
			.Concat(traitId.HasValue ? _traitEdges.GetValueOrDefault(traitId.Value) ?? [] : []).Distinct().ToArray();
		if (affected.Length > 0) lock (Guard(character)) EvaluateEdges(character, affected);
	}

	private void EvaluateEdges(ICharacter actor, IEnumerable<(long Capability, long Spell)> affected)
	{
		EnsureIndex();
		var owner = Owner(actor);
		if (!_evaluatingProgress.TryAdd(owner.Id, 0)) return;
		try
		{
		var bodies = ProgressBodies(actor).ToArray();
		var queue = new Queue<(long Capability, long Spell)>(affected);
		// Unmet prerequisites are provisional: a later grant may enqueue this admission again.
		// Only new acquisitions schedule downstream edges, so the affected cascade remains bounded.
		while (queue.TryDequeue(out var entry))
		{
			if (_world.MagicCapabilities.Get(entry.Capability) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p ||
				_store.Enrolment(owner.Id, p.Identity) is null) continue;
			// Temporary effect attachment alone must never auto-acquire. Explicit enrolment plus permanent merit is required.
			var physical = bodies.FirstOrDefault(body => body.Capabilities.Any(x => x.Id == c.Id) &&
				body.Merits.OfType<IMagicCapabilityMerit>().Any(x => x.Applies(body) && x.Capabilities.Any(k => k.Id == c.Id)));
			if (physical is null) continue;
			var a = p.Admissions.FirstOrDefault(x => x.SpellId == entry.Spell);
			if (a is null || a.Prerequisites.Count == 0 || Acquisition(actor, a.SpellId) is not null || c.CastingConfigurationErrors().Count > 0) continue;
			if (!a.Prerequisites.All(e =>
			{
				var prior = p.Admissions.Single(x => x.SpellId == e.SpellId);
				var state = Acquisition(actor, e.SpellId);
				return state is not null && state.ControlledGrade >= e.MinimumGrade &&
					owner.TraitRawValue(_world.Traits.Get(prior.TraitId ?? p.DefaultTraitId)) >= e.MinimumProficiency &&
					QuarantineReason(actor, e.SpellId, prior.TraitId ?? p.DefaultTraitId, p.ReserveResourceId) is null;
			})) continue;
			var result = GrantCore(physical, c.Id, a.SpellId, $"Capability {p.Identity}, admission {a.Key}: prerequisites satisfied.");
			if (result.Changed && _spellEdges.TryGetValue(a.SpellId, out var next)) foreach (var edge in next) queue.Enqueue(edge);
		}
		}
		finally { _evaluatingProgress.TryRemove(owner.Id, out _); }
	}

	private static IEnumerable<ICharacter> ProgressBodies(ICharacter actor)
	{
		var owner = Owner(actor);
		return owner.IsPlayerCharacter ? [owner.Identity?.FocusedInstance ?? actor] : owner.Identity?.Instances.Cast<ICharacter>() ?? [actor];
	}
}
