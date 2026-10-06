#nullable enable
namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	private void RecordSupportSkillCap(ICharacter actor, IMagicCastingCapability capability, MagicCastingSupportGrant support)
	{
		var owner = Owner(actor);
		var policy = capability.CastingPolicy!;
		if (support.RawSkillCap is not { } cap || _store.Enrolment(owner.Id, policy.Identity) is null ||
			CapHistory(owner.Id).Contains(support.TraitId) ||
			_store.SupportGrant(owner.Id, policy.Identity, support.Key) is not { } grant || grant.TraitId != support.TraitId) return;
		var now = _clock();
		// A definition may opt an existing uncapped support into a cap. Preserve the immutable
		// acquisition receipt and journal the adoption separately before reconciling its native skill.
		var definition = new XElement("SkillCap", new XAttribute("version", 1), new XAttribute("identity", policy.Identity),
			new XAttribute("support", support.Key), new XAttribute("grant", grant.OperationId), new XAttribute("rawCap", cap));
		_store.Write(operation: new(Guid.NewGuid(), owner.Id, actor.InstanceId, actor.Body.Id, capability.Id, 0,
			support.TraitId, policy.ReserveResourceId, MagicCastingStateStore.SkillCapRecorded,
			definition.ToString(SaveOptions.DisableFormatting), now, now));
		_capHistory[owner.Id] = CapHistory(owner.Id).Append(support.TraitId).ToHashSet();
	}

	private MagicCastingGrant GrantSupportCore(ICharacter actor, IMagicCastingCapability capability, MagicCastingSupportGrant support, string provenance)
	{
		var owner = Owner(actor); var policy = capability.CastingPolicy!;
		var prior = _store.SupportGrant(owner.Id, policy.Identity, support.Key);
		if (prior is not null && prior.TraitId != support.TraitId) return new(false, false, "Support binding changed; an explicit player migration is required.");
		var trait = _world.Traits.Get(support.TraitId)!;
		if (QuarantineReason(actor, traitId: trait.Id, reserveId: policy.ReserveResourceId) is { } q) return new(false, false, q);
		if (prior is null)
		{
			var now = _clock();
			var xml = new XElement("SupportGrant", new XAttribute("version", 1), new XAttribute("identity", policy.Identity),
				new XAttribute("key", support.Key), new XAttribute("trait", support.TraitId), new XAttribute("opening", support.OpeningSkill),
				support.RawSkillCap is { } cap ? new XAttribute("rawCap", cap) : null, new XElement("Provenance", provenance));
			var operationId = new Guid(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes($"CastingSupport/v1/{owner.Id}/{policy.Identity}/{support.Key}")).AsSpan(0, 16));
			_store.Write(operation: new(operationId, owner.Id, actor.InstanceId, actor.Body.Id, capability.Id, 0, trait.Id,
				policy.ReserveResourceId, support.RawSkillCap.HasValue ? MagicCastingStateStore.CappedSupportGranted : MagicCastingStateStore.SupportGranted,
				xml.ToString(SaveOptions.DisableFormatting), now, now));
			if (support.RawSkillCap.HasValue) _capHistory[owner.Id] = CapHistory(owner.Id).Append(trait.Id).ToHashSet();
		}
		var missing = !owner.HasTrait(trait);
		if (missing)
		{
			if (!owner.AddTrait(trait, prior?.OpeningSkill ?? support.OpeningSkill)) throw new InvalidOperationException("Native support skill opening failed; retry the durable grant.");
			Flush(actor);
		}
		return new(prior is null || missing, true, "Support skill authorised; existing proficiency retained.");
	}
}
