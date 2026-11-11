#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MudSharp.Community.Templates;

/// <summary>Content-independent definition validation and explicit variant application.</summary>
public static class ClanTemplateDefinitions
{
	public static T Copy<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value))!;
	public static long Privileges(IEnumerable<string> names) => names.Aggregate(0L, (flags, name) =>
		flags | (Enum.TryParse<ClanPrivilegeType>(name, out var value) ? (long)value :
			throw new InvalidOperationException($"Unknown clan privilege {name}.")));

	public static ClanTemplateDefinition WithVariant(ClanTemplateDefinition current, string key)
	{
		var variant = current.AvailableVariants.SingleOrDefault(x => x.Key == key)
			?? throw new InvalidOperationException($"Variant {key} does not apply to this template.");
		var result = Copy(current);
		foreach (var rank in variant.AddedRanks) result.Ranks.Add(Copy(rank));
		static T Patch<T>(T value, JsonObject patch)
		{
			var node = JsonSerializer.SerializeToNode(value)!.AsObject();
			foreach (var field in patch)
			{
				if (field.Key == "key" || !node.ContainsKey(field.Key))
					throw new InvalidOperationException($"Unsupported variant field {field.Key}.");
				node[field.Key] = field.Value?.DeepClone();
			}
			return node.Deserialize<T>()!;
		}
		foreach (var (rankKey, patch) in variant.RankOverrides)
		{
			var index = result.Ranks.FindIndex(x => x.Key == rankKey);
			if (index < 0) throw new InvalidOperationException($"Variant refers to missing rank {rankKey}.");
			result.Ranks[index] = Patch(result.Ranks[index], patch);
		}
		foreach (var (officeKey, patch) in variant.AppointmentOverrides)
		{
			var index = result.Appointments.FindIndex(x => x.Key == officeKey);
			if (index < 0) throw new InvalidOperationException($"Variant refers to missing office {officeKey}.");
			result.Appointments[index] = Patch(result.Appointments[index], patch);
		}
		result.ExternalControls.RemoveAll(x => variant.RemovedExternalControls.Contains(x.ControlledAppointmentKey));
		Validate(result);
		return result;
	}

	public static void Validate(ClanTemplateDefinition template)
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
		if (template.Ranks.Any(x => x.FormsOfAddress.Any(y => $"rank:{x.Key}:{y.Key}".Length > 100)) ||
		    template.Appointments.Any(x => x.FormsOfAddress.Any(y => $"office:{x.Key}:{y.Key}".Length > 100)))
			throw new InvalidOperationException("An address profile key exceeds 100 characters.");
		var ranks = template.Ranks.Select(x => x.Key).ToHashSet();
		var offices = template.Appointments.ToDictionary(x => x.Key);
		var conditions = template.BaseConditions.Concat(template.Appointments.SelectMany(x =>
			x.CandidateConditions.Concat(x.QualificationConditions).Concat(x.VoterConditions).Concat(x.RecallConditions)))
			.Concat(template.Ranks.SelectMany(x => x.FormsOfAddress).Concat(template.Appointments.SelectMany(x => x.FormsOfAddress)).SelectMany(x => x.Conditions));
		foreach (var condition in conditions)
		{
			if (condition.Kind is not ("membership" or "minimumrank" or "exactrank" or "office") ||
			    condition.Role != "$self" && template.LiveBindings.All(x => x.Role != condition.Role) ||
			    condition.Kind != "membership" && string.IsNullOrWhiteSpace(condition.Key) ||
			    condition.Role == "$self" && condition.Kind is "minimumrank" or "exactrank" && !ranks.Contains(condition.Key!) ||
			    condition.Role == "$self" && condition.Kind == "office" && !offices.ContainsKey(condition.Key!))
				throw new InvalidOperationException($"Invalid standing condition in {template.Key}.");
		}
		foreach (var rules in template.Ranks.Select(x => x.FormsOfAddress).Concat(template.Appointments.Select(x => x.FormsOfAddress)))
		{
			Unique(rules.Select(x => x.Key), "address key");
			if (rules.Any(x => x.Key.Length > 60 || x.Text is not null && (string.IsNullOrWhiteSpace(x.Text) || x.Text.Length > 200) ||
			                  x.Relations == 0 || (x.Relations & ~ClanAddressRelation.Any) != 0 ||
			                  x.Conditions.Count > 0 && x.ProgName is not null || x.ProgName is not null && string.IsNullOrWhiteSpace(x.ProgName)))
				throw new InvalidOperationException("Invalid address text, relation or predicate recipe.");
			var defaults = rules.Where(x => x.Relations == ClanAddressRelation.Any && x.Conditions.Count == 0 && x.ProgName is null).ToList();
			if (defaults.Count > 1 || defaults.Count == 1 && defaults[0] != rules.Last())
				throw new InvalidOperationException("An address list permits one unconditional default, last.");
		}
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
			    office.IsAppointedByElection && (office.NominationPeriodMinutes is not > 0 || office.VotingPeriodMinutes is not > 0))
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
