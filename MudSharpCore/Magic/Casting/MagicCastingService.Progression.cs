using MudSharp.RPG.Merits.Interfaces;

#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private readonly System.Collections.Concurrent.ConcurrentDictionary<long, IReadOnlySet<long>> _capHistory = new();
	private IReadOnlySet<long> CapHistory(long ownerId) => _capHistory.GetOrAdd(ownerId, _store.CappedTraits);

	private void RecordSkillCap(ICharacter actor, IMagicCastingCapability capability, MagicCastingAdmission admission, AcquiredSpell? acquired = null)
	{
		var owner = Owner(actor);
		var policy = capability.CastingPolicy!;
		var traitId = admission.TraitId ?? policy.DefaultTraitId;
		if (admission.RawSkillCap is not { } cap || _store.Enrolment(owner.Id, policy.Identity) is null ||
			CapHistory(owner.Id).Contains(traitId))
		{
			if (acquired is not null) _store.Write(acquired: acquired);
			return;
		}
		var now = _clock();
		// The terminal journal marker survives removal/rebinding of the authored admission.
		// It records cap opt-in, rather than imposing a global policy on untouched native skills.
		var definition = new XElement("SkillCap", new XAttribute("version", 1),
			new XAttribute("identity", policy.Identity), new XAttribute("admission", admission.Key), new XAttribute("rawCap", cap));
		_store.Write(operation: new(Guid.NewGuid(), owner.Id, actor.InstanceId, actor.Body.Id, capability.Id,
			admission.SpellId, traitId, policy.ReserveResourceId, MagicCastingStateStore.SkillCapRecorded,
			definition.ToString(SaveOptions.DisableFormatting), now, now), acquired: acquired);
		_capHistory[owner.Id] = CapHistory(owner.Id).Append(traitId).ToHashSet();
	}

	public double? RawSkillImprovementCap(ICharacter actor, long traitId)
	{
		EnsureIndex();
		// A lost entitlement inhibits future gains; it must not erase stored proficiency.
		var owner = Owner(actor);
		var cappedBefore = CapHistory(owner.Id).Contains(traitId);
		if (!_cappedSkills.Contains(traitId) && !cappedBefore) return null;
		var bodies = ProgressBodies(actor).ToArray();
		double highest = 0;
		foreach (var capabilityId in _skillCapPolicies.GetValueOrDefault(traitId) ?? [])
		{
			if (_world.MagicCapabilities.Get(capabilityId) is not IMagicCastingCapability { CastingPolicy: not null } capability) continue;
			var policy = capability.CastingPolicy!;
			if (!policy.Enabled || capability.CastingConfigurationErrors().Count > 0 ||
				_store.Enrolment(owner.Id, policy.Identity) is null || !bodies.Any(body =>
					body.Capabilities.Any(x => x.Id == capability.Id) && body.Merits.OfType<IMagicCapabilityMerit>()
						.Any(x => x.Applies(body) && x.Capabilities.Any(c => c.Id == capability.Id)))) continue;
			foreach (var admission in policy.Admissions.Where(x => (x.TraitId ?? policy.DefaultTraitId) == traitId))
			{
				if (Acquisition(actor, admission.SpellId) is null) continue;
				// A legitimate uncapped shared route retains its ordinary native ceiling.
				if (admission.RawSkillCap is null) return null;
				highest = Math.Max(highest, admission.RawSkillCap.Value);
			}
		}
		return highest > 0 || cappedBefore ? highest : null;
	}
}
