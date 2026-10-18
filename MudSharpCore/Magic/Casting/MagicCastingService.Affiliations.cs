#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Character;

namespace MudSharp.Magic.Casting;

public sealed partial class MagicCastingService
{
	public IReadOnlyList<MagicCastingAffiliation> LearnedAffiliations(ICharacter character)
	{
		var owner = Owner(character);
		var result = new List<MagicCastingAffiliation>();
		var capabilities = _world.MagicCapabilities.OfType<IMagicCastingCapability>().ToArray();
		var identities = capabilities.Select(x => x.CastingPolicy?.Identity).OfType<Guid>()
			.Where(x => x != Guid.Empty).Distinct().ToArray();
		var enrolments = identities.Length == 0 ? new Dictionary<Guid, CastingEnrolment>() :
			_store.EnrolmentsFor(owner.Id, identities).ToDictionary(x => x.CapabilityIdentity);
		foreach (var capability in capabilities)
		{
			if (capability.CastingPolicy is not { Identity: var identity } policy || identity == Guid.Empty)
			{
				continue;
			}

			enrolments.TryGetValue(identity, out var enrolment);
			// A completed native enrolment follows durable starting acquisitions. Canonical
			// spell knowledge alone cannot identify which of several routes taught it.
			if (enrolment is null || enrolment.CharacterId != owner.Id ||
				enrolment.CapabilityIdentity != identity || enrolment.CapabilityId != capability.Id ||
				enrolment.StartingVersion <= 0)
			{
				continue;
			}

			var active = policy.Enabled && capability.CastingConfigurationErrors().Count == 0 &&
				character.Capabilities.Any(x => x.Id == capability.Id);
			result.Add(new(capability.Id, capability.Name, active));
		}

		return result
			.OrderBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
			.ThenBy(x => x.CapabilityId)
			.ToArray();
	}
}
