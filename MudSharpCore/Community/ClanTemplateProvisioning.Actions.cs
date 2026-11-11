#nullable enable

using System.Text.Json;

namespace MudSharp.Community;

public static partial class ClanTemplateProvisioning
{
	/// <summary>Generic admin-only operations for data-authored profiles, exposed to content progs.</summary>
	public static string Execute(ICharacter? actor, IClan? clan, string action, string arguments)
	{
		if (actor?.IsAdministrator() != true) return "Only administrators may provision template copies.";
		var world = actor.Gameworld;
		var profile = clan is null ? null : ClanTemplateProfile.Read(world, clan.Id);
		if (clan is null || profile is null || profile.StockTemplate)
			return "Select a profiled copy created with clan create staged.";
		var command = new StringStack(arguments);
		try
		{
			switch (action.ToLowerInvariant())
			{
				case "export": return JsonSerializer.Serialize(profile, new JsonSerializerOptions { WriteIndented = true });
				case "status":
					var errors = ClanTemplateEligibility.ValidateBindings(world, clan, profile, profile.Activated);
					return $"{clan.FullName}: {profile.TemplateKey}, {(profile.Activated ? "activated" : "staged")}.\n" +
						(errors.Count == 0 ? "Bindings are ready." : string.Join("\n", errors)) +
						"\nAvailable variants: " + string.Join(", ", profile.Definition.AvailableVariants.Select(x => x.Key));
				case "bind":
					if (!clan.IsTemplate || profile.Activated || clan.Appointments.Any(x => x.Elections.Any()))
						return "Initial bindings require an unactivated staged copy without ballot history.";
					var role = command.PopSpeech();
					var requirement = profile.Definition.LiveBindings.SingleOrDefault(x => x.Role == role);
					if (requirement is null) return "That role is not declared by this template profile.";
					if (!long.TryParse(command.PopSpeech(), out var clanId) || world.Clans.Get(clanId) is not { IsTemplate: false } target || target == clan)
						return "Select another live clan by ID.";
					long? officeId = null;
					long? affiliationId = null;
					if (!command.IsFinished)
					{
						if (!long.TryParse(command.PopSpeech(), out var selected) || target.Appointments.All(x => x.Id != selected))
							return "Select an existing office in that live clan by ID.";
						officeId = selected;
					}
					if (!command.IsFinished)
					{
						if (!long.TryParse(command.PopSpeech(), out var selected)) return "Supply the custom authority's affiliation clan ID.";
						affiliationId = selected;
					}
					if (!command.IsFinished) return "Too many binding arguments.";
					if (requirement.RequiresSelectedOffice && officeId is null) return "This role requires an explicit office ID.";
					profile.Bindings[role] = new ClanTemplateBinding { ClanId = target.Id, AppointmentId = officeId, AffiliationId = affiliationId };
					SaveProfile(world, profile);
					return "Binding recorded. Review status before activation.";
				case "variant":
					ApplyVariant(world, clan, profile, command.PopSpeech());
					return "Variant applied to the staged copy; progs regenerated.";
				case "activate":
					var failures = Activate(world, clan, profile);
					return failures.Count == 0 ? "Template copy activated. Native elections now run." : string.Join("\n", failures);
				case "preflight":
					if (!profile.Appointments.TryGetValue(command.PopSpeech(), out var appointmentId)) return "Select an appointment key from the exported profile.";
					if (!long.TryParse(command.PopSpeech(), out var characterId)) return "Supply the intended character ID.";
					var error = ClanTemplateEligibility.Error(world, world.TryGetCharacter(characterId, true), clan.Id, appointmentId, "preflight");
					return error.Length == 0 ? "Qualification preflight passed. Use the authorised appointment route." : error;
				default: return "Unknown template operation. Use status, bind, variant, activate, preflight or export.";
			}
		}
		catch (InvalidOperationException ex) { return ex.Message; }
	}
}
