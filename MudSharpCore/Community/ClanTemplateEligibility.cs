#nullable enable

using MudSharp.Community.Templates;

namespace MudSharp.Community;

/// <summary>Evaluates data-authored nomination, voting and appointment preflight rules. Does not intercept ordinary commands.</summary>
public static partial class ClanTemplateEligibility
{
	public static IReadOnlyList<string> ValidateBindings(IFuturemud world, IClan clan, ClanTemplateProfile profile,
		bool checkConfiguration = true)
	{
		var errors = new List<string>();
		var definition = profile.Definition;
		foreach (var rank in definition.Ranks)
			if (!profile.Ranks.TryGetValue(rank.Key, out var id) || clan.Ranks.All(x => x.Id != id)) errors.Add($"Missing mapped rank {rank.Key}.");
		foreach (var office in definition.Appointments)
			if (!profile.Appointments.TryGetValue(office.Key, out var id) || clan.Appointments.All(x => x.Id != id)) errors.Add($"Missing mapped office {office.Key}.");
		var affiliationId = definition.IsAffiliationRoot ? clan.Id :
			definition.AffiliationRole is { } affiliationRole ? profile.Bindings.GetValueOrDefault(affiliationRole)?.ClanId : null;
		foreach (var requirement in definition.LiveBindings)
		{
			if (!profile.Bindings.TryGetValue(requirement.Role, out var binding))
			{
				if (requirement.Required) errors.Add($"Bind the {requirement.Role} role.");
				continue;
			}
			var target = world.Clans.Get(binding.ClanId);
			if (target is null || target.IsTemplate || target.Id == clan.Id)
			{
				errors.Add($"The {requirement.Role} binding must select another live clan.");
				continue;
			}
			var targetProfile = ClanTemplateProfile.Read(world, target.Id);
			if (requirement.AllowedTemplateKeys.Count > 0 &&
			    (targetProfile?.Activated != true || !requirement.AllowedTemplateKeys.Contains(targetProfile.TemplateKey)))
				errors.Add($"The {requirement.Role} clan has an incompatible or unconfigured template profile.");
			if (definition.AffiliationRole is { } role && requirement.Role != role)
			{
				var targetAffiliation = targetProfile is null ? binding.AffiliationId : targetProfile.Definition.IsAffiliationRoot
					? target.Id : targetProfile.Definition.AffiliationRole is { } targetRole
						? targetProfile.Bindings.GetValueOrDefault(targetRole)?.ClanId : null;
				if (targetAffiliation != affiliationId)
					errors.Add($"The {requirement.Role} authority must declare the same {role} affiliation.");
			}
			if (binding.AppointmentId is { } selected && target.Appointments.All(x => x.Id != selected) ||
			    requirement.RequiresSelectedOffice && binding.AppointmentId is null)
				errors.Add($"Select an existing office for {requirement.Role}.");
		}
		if (errors.Count > 0) return errors;
		IAppointment Local(string key) => clan.Appointments.Single(x => x.Id == profile.Appointments[key]);
		foreach (var source in definition.Appointments)
		{
			var office = Local(source.Key);
			var authority = ResolveAuthority(world, clan, profile, source.ElectionConfirmationAppointmentRef);
			if (source.ElectionConfirmationAppointmentRef is not null && authority is null)
				errors.Add($"Missing confirmation authority for {source.Key}.");
			if (authority == office) errors.Add($"{source.Key} cannot confirm itself.");
			if (checkConfiguration && office.ElectionConfirmationAppointment != authority)
				errors.Add($"Confirmation binding changed for {source.Key}; resolve ballots and reconfigure.");
			if (checkConfiguration && source.IsAppointedByElection && clan.ExternalControls.Any(x => x.VassalClan == clan && x.ControlledAppointment == office))
				errors.Add($"Elected {source.Key} must not have external reservations.");
			var visiting = new HashSet<long>();
			var visited = new HashSet<long>();
			bool Cycle(IAppointment current)
			{
				if (visiting.Contains(current.Id)) return true;
				if (!visited.Add(current.Id)) return false;
				visiting.Add(current.Id);
				var next = new List<IAppointment>();
				if (current.ParentPosition is { } parent) next.Add(parent);
				if (current.Clan == clan && definition.Appointments.FirstOrDefault(x => profile.Appointments[x.Key] == current.Id) is { } planned)
				{
					if (ResolveAuthority(world, clan, profile, planned.ElectionConfirmationAppointmentRef) is { } confirmer) next.Add(confirmer);
					foreach (var control in definition.ExternalControls.Where(x => x.ControlledAppointmentKey == planned.Key))
						if (ResolveAuthority(world, clan, profile, new ClanTemplateOfficeReference { Clan = "$" + control.LiegeClanRole, AppointmentKey = control.ControllingAppointmentKey }) is { } controller) next.Add(controller);
				}
				else if (current.ElectionConfirmationAppointment is { } confirmer) next.Add(confirmer);
				foreach (var control in current.Clan.ExternalControls.Where(x => x.ControlledAppointment == current && x.ControllingAppointment is not null)) next.Add(control.ControllingAppointment);
				if (next.Distinct().Any(Cycle)) return true;
				visiting.Remove(current.Id);
				return false;
			}
			if (Cycle(office)) errors.Add($"Cyclic authority dependency for {source.Key}.");
		}
		foreach (var recipe in definition.ExternalControls)
		{
			var authority = ResolveAuthority(world, clan, profile, new ClanTemplateOfficeReference { Clan = "$" + recipe.LiegeClanRole, AppointmentKey = recipe.ControllingAppointmentKey });
			if (authority is null) errors.Add($"Missing controlling authority for {recipe.ControlledAppointmentKey}.");
			else if (checkConfiguration && !clan.ExternalControls.Any(x => x.VassalClan == clan &&
			         x.ControlledAppointment == Local(recipe.ControlledAppointmentKey) && x.ControllingAppointment == authority &&
			         x.LiegeClan == authority.Clan && x.NumberOfAppointments == recipe.NumberOfAppointments))
				errors.Add($"External control changed for {recipe.ControlledAppointmentKey}; review the provisioning record.");
		}
		return errors.Distinct().ToList();
	}

	public static IAppointment? ResolveAuthority(IFuturemud world, IClan clan, ClanTemplateProfile profile, ClanTemplateOfficeReference? reference)
	{
		if (reference is null) return null;
		if (reference.Clan == "$self") return clan.Appointments.FirstOrDefault(x => x.Id == profile.Appointments.GetValueOrDefault(reference.AppointmentKey));
		if (!profile.Bindings.TryGetValue(reference.Clan.TrimStart('$'), out var binding)) return null;
		var target = world.Clans.Get(binding.ClanId);
		if (target is null || target.IsTemplate) return null;
		var id = reference.AppointmentKey == "selected_office" ? binding.AppointmentId :
			ClanTemplateProfile.Read(world, target.Id)?.Appointments.GetValueOrDefault(reference.AppointmentKey);
		if (binding.AppointmentId is { } pinned && pinned != id) return null;
		return target.Appointments.FirstOrDefault(x => x.Id == id);
	}

	/// <summary>Retains resolved office IDs in the export; fixed role keys must agree with any explicit choice.</summary>
	public static void RetainAuthorityIds(IFuturemud world, IClan clan, ClanTemplateProfile profile)
	{
		var definition = profile.Definition;
		var references = definition.Appointments.Select(x => x.ElectionConfirmationAppointmentRef)
			.Where(x => x is not null && x.Clan != "$self").Cast<ClanTemplateOfficeReference>()
			.Concat(definition.ExternalControls.Select(x => new ClanTemplateOfficeReference
			{ Clan = "$" + x.LiegeClanRole, AppointmentKey = x.ControllingAppointmentKey }));
		foreach (var reference in references)
		{
			var authority = ResolveAuthority(world, clan, profile, reference)
				?? throw new InvalidOperationException($"Unresolved authority {reference.Clan}.{reference.AppointmentKey}.");
			profile.Bindings[reference.Clan.TrimStart('$')].AppointmentId = authority.Id;
		}
	}

	/// <summary>Empty text means eligible. Every check uses live object identities and current rank ordinals.</summary>
	public static string Error(IFuturemud world, ICharacter? character, long clanId, long appointmentId, string purpose)
	{
		var clan = world.Clans.Get(clanId);
		var profile = ClanTemplateProfile.Read(world, clanId);
		if (purpose.StartsWith("address:", StringComparison.Ordinal))
			return AddressError(world, character, clan, profile, purpose["address:".Length..]);
		if (clan is null || clan.IsTemplate || profile?.Activated != true) return "This profiled clan has not been activated.";
		var bindingErrors = ValidateBindings(world, clan, profile);
		if (bindingErrors.Count > 0) return bindingErrors[0];
		var source = profile.Definition.Appointments.FirstOrDefault(x => profile.Appointments.GetValueOrDefault(x.Key) == appointmentId);
		if (source is null) return "This office is not in the configured template profile.";
		if (character is null || !character.IsPrimaryInstance || character.State.IsDead()) return "A living primary character is required.";
		var member = clan.Memberships.FirstOrDefault(x => x.MemberId == CharacterInstanceIdentityComparer.IdentityId(character) && !x.IsArchivedMembership);
		if (member is null) return "Active membership of this clan is required.";
		bool Meets(ClanTemplateCondition condition)
		{
			var target = condition.Role == "$self" ? clan : profile.Bindings.TryGetValue(condition.Role, out var binding)
				? world.Clans.Get(binding.ClanId) : null;
			if (target is null || target.IsTemplate) return false;
			var mapping = target == clan ? profile : ClanTemplateProfile.Read(world, target.Id);
			var membership = target.Memberships.FirstOrDefault(x => x.MemberId == member.MemberId && !x.IsArchivedMembership);
			if (membership is null) return false;
			if (condition.Kind == "membership") return true;
			if (mapping is null || condition.Key is null) return false;
			if (condition.Kind == "office") return membership.Appointments.Any(x => x.Id == mapping.Appointments.GetValueOrDefault(condition.Key));
			var rank = target.Ranks.FirstOrDefault(x => x.Id == mapping.Ranks.GetValueOrDefault(condition.Key));
			return rank is not null && (condition.Kind switch
			{
				"minimumrank" => membership.Rank.RankNumber >= rank.RankNumber,
				"exactrank" => membership.Rank.Id == rank.Id,
				_ => false
			});
		}
		IEnumerable<ClanTemplateCondition> conditions = profile.Definition.BaseConditions;
		switch (purpose)
		{
			case "votes": conditions = conditions.Concat(source.VoterConditions); break;
			case "recall": conditions = conditions.Concat(source.RecallConditions); break;
			case "nominate":
			case "preflight":
				if (!profile.Ranks.TryGetValue(source.MinimumRankKey, out var rankId) ||
				    clan.Ranks.FirstOrDefault(x => x.Id == rankId) is not { } minimum || member.Rank.RankNumber < minimum.RankNumber)
					return "The required local standing must already be recorded.";
				conditions = conditions.Concat(source.QualificationConditions);
				if (purpose == "nominate") conditions = conditions.Concat(source.CandidateConditions);
				break;
			default: return "Unknown template eligibility operation.";
		}
		foreach (var condition in conditions)
			if (!Meets(condition)) return condition.Error;
		return string.Empty;
	}
}
