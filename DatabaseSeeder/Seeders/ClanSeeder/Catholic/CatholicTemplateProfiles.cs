#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MudSharp.Community.Templates;

namespace DatabaseSeeder.Seeders;

/// <summary>Setting policy is compiled into editable content data; the engine knows only generic conditions.</summary>
public static class CatholicTemplateProfiles
{
	public static ClanTemplateDefinition Create(string key)
	{
		var result = Map(CatholicTemplateCatalogue.Get(key));
		foreach (var variantKey in CatholicTemplateCatalogue.VariantKeys)
		{
			CatholicTemplateDefinition alternate;
			try { alternate = CatholicTemplateCatalogue.Get(key, [variantKey]); }
			catch (InvalidOperationException ex) when (ex.Message.Contains("does not apply", StringComparison.Ordinal)) { continue; }
			var changed = Map(alternate);
			var variant = new ClanTemplateVariant { Key = variantKey };
			foreach (var rank in changed.Ranks)
			{
				var original = result.Ranks.SingleOrDefault(x => x.Key == rank.Key);
				if (original is null) variant.AddedRanks.Add(rank);
				else if (Diff(original, rank) is { Count: > 0 } diff) variant.RankOverrides[rank.Key] = diff;
			}
			foreach (var office in changed.Appointments)
				if (Diff(result.Appointments.Single(x => x.Key == office.Key), office) is { Count: > 0 } diff)
					variant.AppointmentOverrides[office.Key] = diff;
			variant.RemovedExternalControls = result.ExternalControls.Where(x =>
				changed.ExternalControls.All(y => y.ControlledAppointmentKey != x.ControlledAppointmentKey))
				.Select(x => x.ControlledAppointmentKey).ToList();
			result.AvailableVariants.Add(variant);
		}
		ClanTemplateDefinitions.Validate(result);
		return result;
	}

	private static JsonObject Diff<T>(T before, T after)
	{
		var original = JsonSerializer.SerializeToNode(before)!.AsObject();
		var changed = JsonSerializer.SerializeToNode(after)!.AsObject();
		var result = new JsonObject();
		foreach (var field in changed)
			if (!JsonNode.DeepEquals(original[field.Key], field.Value)) result[field.Key] = field.Value?.DeepClone();
		return result;
	}

	private static ClanTemplateDefinition Map(CatholicTemplateDefinition source)
	{
		var result = JsonSerializer.SerializeToNode(source)!.Deserialize<ClanTemplateDefinition>()!;
		result.IsAffiliationRoot = source.Key == "catholic.church";
		result.AffiliationRole = result.IsAffiliationRoot ? null : "church";
		var churchRole = result.IsAffiliationRoot ? "$self" : "church";
		ClanTemplateCondition Condition(string kind, string? key, string role, string error) =>
			new() { Kind = kind, Key = key, Role = role, Error = error };
		result.BaseConditions.Add(Condition("membership", null, "$self", "Active membership of this clan is required."));
		if (!result.IsAffiliationRoot)
			result.BaseConditions.Add(Condition("membership", null, "church", "Active membership of the bound Church is required."));
		foreach (var binding in result.LiveBindings) binding.RequiresSelectedOffice = binding.Role == "canonical_superior";
		foreach (var office in result.Appointments)
		{
			var original = source.Appointments.Single(x => x.Key == office.Key);
			ClanTemplateCondition Local(string kind, string key, string error) => Condition(kind, key, "$self", error);
			office.CandidateConditions = original.CandidateProfile switch
			{
				"bishop_cardinal" => [Local("exactrank", "bishop", "Bishop standing in this Church is required."), Local("office", "cardinal", "Only Cardinals of this Church may stand.")],
				"bishop" => [Local("exactrank", "bishop", "Bishop standing in this clan is required.")],
				"priest_canon" => [Local("minimumrank", "priest", "Priest standing in this clan is required."), Local("office", "cathedral_canon", "Only Cathedral Canons of this clan may stand.")],
				null or "minimum_rank" => [],
				_ => throw new InvalidOperationException($"Unknown Catholic candidate profile {original.CandidateProfile}.")
			};
			office.VoterConditions = original.ElectorateProfile switch
			{
				"cardinals" => [Local("office", "cardinal", "Only Cardinals of this Church may vote.")],
				"canons" => [Local("office", "cathedral_canon", "Only Cathedral Canons of this clan may vote.")],
				"professed" => [Local("minimumrank", "professed", "Only professed members of this community may vote.")],
				null => [],
				_ => throw new InvalidOperationException($"Unknown Catholic electorate {original.ElectorateProfile}.")
			};
			office.RecallConditions = source.RankModel == "religious"
				? [Local("minimumrank", "professed", "Only professed members of this community may initiate or vote in a recall.")] : [];
			var error = "The additional Church standing or qualifying office is required.";
			office.QualificationConditions = original.QualificationRule switch
			{
				null => [],
				"church_member" => [Condition("membership", null, churchRole, error)],
				"church_deacon" => [Condition("minimumrank", "deacon", churchRole, error)],
				"church_priest" => [Condition("minimumrank", "priest", churchRole, error)],
				"church_bishop" => [Condition("exactrank", "bishop", churchRole, error)],
				"church_cardinal" => [Condition("office", "cardinal", churchRole, error)],
				"same_holder_as_local_ordinary" => [Local("office", "ordinary", error), Condition("exactrank", "bishop", churchRole, error)],
				_ => throw new InvalidOperationException($"Unknown Catholic qualification {original.QualificationRule}.")
			};
		}
		CatholicAddressContent.Apply(source, result);
		return result;
	}
}
