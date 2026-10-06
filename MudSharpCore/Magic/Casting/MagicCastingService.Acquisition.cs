using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private readonly record struct ProgressionNode(long Capability, MagicCastingPrerequisiteKind Kind, long Id);
	private static IEnumerable<ProgressionNode> ProgressionNodes(IMagicCastingCapability c) => c.CastingPolicy!.Admissions
		.Select(x => new ProgressionNode(c.Id, MagicCastingPrerequisiteKind.Spell, x.SpellId))
		.Concat(c.CastingPolicy.Supports.Select(x => new ProgressionNode(c.Id, MagicCastingPrerequisiteKind.SupportTrait, x.TraitId)));
	private readonly Dictionary<long, HashSet<ProgressionNode>> _spellEdges = [];
	private readonly Dictionary<long, HashSet<ProgressionNode>> _traitEdges = [];
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
		return EnrolCore(target, capabilityId, $"staff {authority.Id}: {reason}", false);
	}

	internal MagicCastingGrant EnrolAuthorised(ICharacter target, long capabilityId, string provenance) =>
		EnrolCore(target, capabilityId, provenance, true);

	private MagicCastingGrant EnrolCore(ICharacter target, long capabilityId, string provenance, bool authored)
	{
		if (string.IsNullOrWhiteSpace(provenance) || provenance.Length > 1024) return new(false, false, "Enrolment needs provenance of at most 1024 characters.");
		return MutateGrant(target, () =>
		{
			var owner = Owner(target);
			if (_world.MagicCapabilities.Get(capabilityId) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p ||
				!target.Capabilities.Any(x => x.Id == capabilityId)) return new(false, false, "The target needs the current enabled capability.");
			if (authored && !target.Merits.OfType<IMagicCapabilityMerit>().Any(x => x.Applies(target) && x.Capabilities.Any(k => k.Id == capabilityId)))
				return new(false, false, "Authored enrolment requires an applicable permanent capability merit.");
			if (c.CastingConfigurationErrors().FirstOrDefault() is { } error) return new(false, false, error);
			var previous = _store.Enrolment(owner.Id, p.Identity);
			if ((previous?.StartingVersion ?? 0) >= p.StartingGrantVersion)
			{
				Reconcile(target);
				EvaluateEdges(target, ProgressionNodes(c));
				return new(false, true, "Already enrolled; starting grants are not repeated; authorised support openings are reconciled.");
			}
			var pending = previous ?? new CastingEnrolment(owner.Id, p.Identity, c.Id, _clock(), 0);
			CastingOperation? receipt = authored ? new(Guid.NewGuid(), owner.Id, target.InstanceId, target.Body.Id, c.Id, 0, 0,
				p.ReserveResourceId, MagicCastingStateStore.EnrolmentRecorded, new XElement("Enrolment", new XAttribute("version", 1),
					new XAttribute("identity", p.Identity), new XElement("Provenance", provenance)).ToString(SaveOptions.DisableFormatting), _clock(), _clock()) : null;
			_store.Write(operation: receipt, enrolment: pending);
			foreach (var root in p.Admissions.Where(x => x.Starting))
			{
				var grant = GrantCore(target, c.Id, root.SpellId, $"Enrolment {p.Identity}, {provenance}");
				if (!grant.Allowed) return grant;
			}
			_store.Write(enrolment: pending with { StartingVersion = p.StartingGrantVersion });
			Reconcile(target);
			EvaluateEdges(target, ProgressionNodes(c));
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
			foreach (var binding in p.Admissions.Select(x => (Trait: x.TraitId ?? p.DefaultTraitId, Cap: x.RawSkillCap))
				.Concat(p.Supports.Select(x => (Trait: x.TraitId, Cap: x.RawSkillCap))))
			{
				if (!_skillCapPolicies.TryGetValue(binding.Trait, out var policies)) _skillCapPolicies[binding.Trait] = policies = [];
				policies.Add(c.Id);
				if (binding.Cap.HasValue) _cappedSkills.Add(binding.Trait);
			}
			foreach (var node in ProgressionNodes(c))
				foreach (var e in NodeEdges(p, node))
				{
					if (e.Kind == MagicCastingPrerequisiteKind.Spell)
					{
						if (!_spellEdges.TryGetValue(e.SpellId, out var spells)) _spellEdges[e.SpellId] = spells = [];
						spells.Add(node);
					}
					var traitId = e.Kind == MagicCastingPrerequisiteKind.SupportTrait ? e.TraitId :
						p.Admissions.FirstOrDefault(x => x.SpellId == e.SpellId) is { } prior ? prior.TraitId ?? p.DefaultTraitId : (long?)null;
					if (!traitId.HasValue) continue;
					if (!_traitEdges.TryGetValue(traitId.Value, out var traits)) _traitEdges[traitId.Value] = traits = [];
					traits.Add(node);
				}
		}
		_indexed = true;
	}

	private static IReadOnlyList<MagicCastingPrerequisite> NodeEdges(MagicCastingPolicy p, ProgressionNode n) =>
		n.Kind == MagicCastingPrerequisiteKind.Spell ? p.Admissions.First(x => x.SpellId == n.Id).Prerequisites :
			p.Supports.First(x => x.TraitId == n.Id).Prerequisites;

	public void NotifyProgress(ICharacter character, long? traitId = null, long? spellId = null)
	{
		if (_mutating.ContainsKey(Owner(character).Id)) return;
		EnsureIndex();
		var affected = (spellId.HasValue ? _spellEdges.GetValueOrDefault(spellId.Value) ?? [] : [])
			.Concat(traitId.HasValue ? _traitEdges.GetValueOrDefault(traitId.Value) ?? [] : []).Distinct().ToArray();
		if (affected.Length > 0) lock (Guard(character)) EvaluateEdges(character, affected);
	}

	private bool PrerequisitesMet(ICharacter actor, MagicCastingPolicy p, IEnumerable<MagicCastingPrerequisite> edges) => edges.All(e =>
	{
		long traitId;
		if (e.Kind == MagicCastingPrerequisiteKind.Spell)
		{
			var prior = p.Admissions.Single(x => x.SpellId == e.SpellId);
			var state = Acquisition(actor, e.SpellId);
			if (state is null || state.ControlledGrade < e.MinimumGrade) return false;
			traitId = prior.TraitId ?? p.DefaultTraitId;
		}
		else
		{
			var support = p.Supports.Single(x => x.TraitId == e.TraitId);
			var grant = _store.SupportGrant(Owner(actor).Id, p.Identity, support.Key);
			if (grant is null || grant.TraitId != support.TraitId) return false;
			traitId = support.TraitId;
		}
		var trait = _world.Traits.Get(traitId);
		return Owner(actor).HasTrait(trait) && Owner(actor).TraitRawValue(trait) >= e.MinimumProficiency &&
			QuarantineReason(actor, e.Kind == MagicCastingPrerequisiteKind.Spell ? e.SpellId : null, traitId, p.ReserveResourceId) is null;
	});

	private void EvaluateEdges(ICharacter actor, IEnumerable<ProgressionNode> affected)
	{
		EnsureIndex();
		var owner = Owner(actor);
		if (!_evaluatingProgress.TryAdd(owner.Id, 0)) return;
		try
		{
			var bodies = ProgressBodies(actor).ToArray();
			var queue = new Queue<ProgressionNode>(affected);
			while (queue.TryDequeue(out var entry))
			{
				if (_world.MagicCapabilities.Get(entry.Capability) is not IMagicCastingCapability c || c.CastingPolicy is not { Enabled: true } p ||
					_store.Enrolment(owner.Id, p.Identity) is null || c.CastingConfigurationErrors().Count > 0) continue;
				var physical = bodies.FirstOrDefault(body => body.Capabilities.Any(x => x.Id == c.Id) &&
					body.Merits.OfType<IMagicCapabilityMerit>().Any(x => x.Applies(body) && x.Capabilities.Any(k => k.Id == c.Id)));
				if (physical is null || !ProgressionNodes(c).Contains(entry)) continue;
				var edges = NodeEdges(p, entry);
				MagicCastingGrant result;
				if (entry.Kind == MagicCastingPrerequisiteKind.Spell)
				{
					var admission = p.Admissions.Single(x => x.SpellId == entry.Id);
					if (Acquisition(actor, entry.Id) is not null || edges.Count == 0 || !PrerequisitesMet(actor, p, edges)) continue;
					result = GrantCore(physical, c.Id, entry.Id, $"Capability {p.Identity}, admission {admission.Key}: prerequisites satisfied.");
				}
				else
				{
					var support = p.Supports.Single(x => x.TraitId == entry.Id);
					var authorised = _store.SupportGrant(owner.Id, p.Identity, support.Key);
					if (authorised is not null && owner.HasTrait(_world.Traits.Get(support.TraitId))) continue;
					if (authorised is null && (!support.Starting && edges.Count == 0 || !PrerequisitesMet(actor, p, edges))) continue;
					result = GrantSupportCore(physical, c, support, $"Capability {p.Identity}, support {support.Key}: prerequisites satisfied.");
				}
				if (!result.Changed) continue;
				var next = entry.Kind == MagicCastingPrerequisiteKind.Spell ? _spellEdges.GetValueOrDefault(entry.Id) : _traitEdges.GetValueOrDefault(entry.Id);
				if (next is not null) foreach (var n in next) queue.Enqueue(n);
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
