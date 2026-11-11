#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MudSharp.Community;
using MudSharp.Community.Templates;

namespace DatabaseSeeder.Seeders;

/// <summary>Shared authoring data; keys and bindings are not native database columns.</summary>
public static class CatholicTemplateCatalogue
{
	private static readonly JsonObject Catalogue = Load();
	public static IReadOnlyList<string> Keys => Catalogue["templates"]!.AsArray()
		.Select(x => x!["key"]!.GetValue<string>()).ToList();
	public static IReadOnlyList<string> VariantKeys => Catalogue["variants"]!.AsArray()
		.Select(x => x!["key"]!.GetValue<string>()).ToList();

	private static JsonObject Load()
	{
		using var stream = typeof(CatholicTemplateCatalogue).Assembly.GetManifestResourceStream("CatholicTemplates")!;
		return JsonNode.Parse(stream)!.AsObject();
	}

	/// <summary>Returns an independent definition, optionally applying deliberate provisioning variants.</summary>
	public static CatholicTemplateDefinition Get(string key, IEnumerable<string>? variants = null)
	{
		var node = Catalogue["templates"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == key)!.DeepClone();
		foreach (var variantKey in variants ?? [])
		{
			var variant = Catalogue["variants"]!.AsArray().SingleOrDefault(x => x!["key"]!.GetValue<string>() == variantKey)
				?? throw new InvalidOperationException($"Unknown Catholic variant {variantKey}.");
			ApplyVariant(node, variant, key);
		}
		var result = node.Deserialize<CatholicTemplateDefinition>()!;
		Validate(result);
		return result;
	}

	private static void ApplyVariant(JsonNode template, JsonNode variant, string key)
	{
		var variantKey = variant["key"]!.GetValue<string>();
		var religious = template["rank_model"]!.GetValue<string>() == "religious";
		bool Listed(string property) => variant[property]!.AsArray().Any(x => x!.GetValue<string>() == key);
		if (variant["template_key"] is not null && variant["template_key"]!.GetValue<string>() != key ||
		    variant["template_keys"] is not null && !Listed("template_keys") ||
		    variant["applies_to"] is JsonArray && !Listed("applies_to") ||
		    variantKey == "historical_minor_orders" && religious ||
		    variantKey.StartsWith("religious_", StringComparison.Ordinal) && !religious ||
		    variantKey == "nonclerical_religious_superior" && !religious ||
		    variantKey == "voluntary_immediate_episcopal_release" && religious)
			throw new InvalidOperationException($"Variant {variantKey} does not apply to {key}.");

		if (variantKey == "historical_minor_orders")
		{
			foreach (var rank in variant["add_ranks"]!.AsArray()) template["ranks"]!.AsArray().Add(rank!.DeepClone());
			if (key == "catholic.church")
				template["appointments"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == "cardinal")!["MinimumRankKey"] = "minor_cleric";
			return;
		}
		foreach (var office in template["appointments"]!.AsArray())
		{
			var officeKey = office!["key"]!.GetValue<string>();
			if (variantKey == "female_monastic_house")
			{
				foreach (var mapping in variant["office_title_map"]!.AsObject())
				{
					if (office["Name"]!.GetValue<string>() != mapping.Key) continue;
					office["Name"] = mapping.Value!.DeepClone();
					office["Title"] = mapping.Value.DeepClone();
					office["Abbreviation"] = mapping.Value.DeepClone();
				}
				if (officeKey == "superior") office["QualificationRule"] = "church_member";
				continue;
			}
			if (variant["appointment_key"] is not null && variant["appointment_key"]!.GetValue<string>() != officeKey) continue;
			if (variantKey == "nonclerical_religious_superior" && officeKey != "superior") continue;
			if (variantKey == "religious_confirmed_intervention" &&
			    (officeKey != "superior" || office["ElectionConfirmationAppointmentRef"] is null ||
			     !office["IsAppointedByElection"]!.GetValue<bool>())) continue;
			if (variantKey == "religious_recall_game_variant" && !office["IsAppointedByElection"]!.GetValue<bool>()) continue;
			if (variant["set"] is JsonObject overrides)
				foreach (var field in overrides) office[field.Key] = field.Value?.DeepClone();
		}
		if (variantKey == "female_monastic_house")
			template["ranks"]!.AsArray().Single(x => x!["key"]!.GetValue<string>() == "professed")!["Title"] = "Nun";
		if (variant["remove_external_controls_for"] is JsonArray removed)
		{
			var controls = template["external_controls"]!.AsArray();
			foreach (var control in controls.ToList())
				if (removed.Any(x => x!.GetValue<string>() == control!["controlled_appointment_key"]!.GetValue<string>())) controls.Remove(control);
		}
	}

	public static string Marker(string templateKey, string appointmentKey) =>
		$"CatholicTemplate_v1_{templateKey.Replace('.', '_')}_{appointmentKey}";

	public static long Privileges(IEnumerable<string> names) => names.Aggregate(0L, (flags, name) =>
		flags | (name == nameof(ClanPrivilegeType.All) || !Enum.TryParse<ClanPrivilegeType>(name, out var value)
			? throw new InvalidOperationException($"Invalid Catholic privilege {name}.") : (long)value));

	public static void Validate(CatholicTemplateDefinition template)
	{
		static void Unique(IEnumerable<string> values, string what)
		{
			var list = values.ToList();
			if (list.Any(string.IsNullOrWhiteSpace) || list.Distinct(StringComparer.OrdinalIgnoreCase).Count() != list.Count)
				throw new InvalidOperationException($"Empty or duplicate {what}.");
		}
		Unique(template.Ranks.Select(x => x.Key), "rank key");
		Unique(template.Ranks.Select(x => x.Name), "rank name");
		Unique(template.Appointments.Select(x => x.Key), "appointment key");
		Unique(template.Appointments.Select(x => x.Name), "appointment name");
		var ranks = template.Ranks.Select(x => x.Key).ToHashSet();
		var offices = template.Appointments.ToDictionary(x => x.Key);
		foreach (var rank in template.Ranks) _ = Privileges(rank.PrivilegeNames);
		foreach (var office in template.Appointments)
		{
			_ = Privileges(office.PrivilegeNames);
			if (!ranks.Contains(office.MinimumRankKey) ||
			    office.MinimumRankToAppointKey is { } appoint && !ranks.Contains(appoint) ||
			    office.MaximumSimultaneousHolders < 1 ||
			    office.ElectionConfirmationAppointmentRef is { Clan: "$self" } reference && !offices.ContainsKey(reference.AppointmentKey) ||
			    office.ElectionConfirmationAppointmentRef is { Clan: not "$self" } external &&
			        template.LiveBindings.All(x => "$" + x.Role != external.Clan) ||
			    office.IsAppointedByElection && (office.CanNominateProgRef is null || office.WhyCantNominateProgRef is null ||
			        office.NumberOfVotesProgRef is null || office.NominationPeriodMinutes is not > 0 || office.VotingPeriodMinutes is not > 0))
				throw new InvalidOperationException($"Invalid rank, capacity or governance reference in {template.Key}.{office.Key}.");
			var visited = new HashSet<string> { office.Key };
			var parent = office.ParentAppointmentKey;
			while (parent is not null)
			{
				if (!visited.Add(parent) || !offices.TryGetValue(parent, out var ancestor))
					throw new InvalidOperationException($"Missing or cyclic parent in {template.Key}.{office.Key}.");
				parent = ancestor.ParentAppointmentKey;
			}
		}
		foreach (var control in template.ExternalControls)
			if (!offices.TryGetValue(control.ControlledAppointmentKey, out var office) || office.IsAppointedByElection ||
			    control.NumberOfAppointments < 1 || control.NumberOfAppointments > office.MaximumSimultaneousHolders ||
			    template.LiveBindings.All(x => x.Role != control.LiegeClanRole))
				throw new InvalidOperationException($"Invalid external-control recipe in {template.Key}.");
	}
}
