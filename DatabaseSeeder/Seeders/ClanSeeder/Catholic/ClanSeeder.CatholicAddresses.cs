#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using MudSharp.Community.Templates;
using MudSharp.Database;
using MudSharp.FutureProg;
using MudSharp.Models;

namespace DatabaseSeeder.Seeders;

public partial class ClanSeeder
{
	private const string ProfilePrefix = "ClanTemplateProfile_v1\n";

	private static bool NeedsCatholicAddresses(FuturemudDatabaseContext context)
	{
		var templateIds = context.Clans.Where(x => x.IsTemplate).Select(x => x.Id).ToHashSet();
		return context.FutureProgs.Where(x => x.Subcategory == "Template Profiles").AsEnumerable().Any(x =>
			ReadAddressUpgradeProfile(x) is { } data && templateIds.Contains(data["ClanId"]!.GetValue<long>()));
	}

	private static JsonObject? ReadAddressUpgradeProfile(FutureProg prog)
	{
		if (prog.FunctionComment?.StartsWith(ProfilePrefix, StringComparison.Ordinal) != true) return null;
		var data = JsonNode.Parse(prog.FunctionComment[ProfilePrefix.Length..])!.AsObject();
		return data["StockTemplate"]?.GetValue<bool>() == true && data["TemplateKey"]?.GetValue<string>().StartsWith("catholic.", StringComparison.Ordinal) == true &&
		       (data["AddressContentVersion"]?.GetValue<int>() ?? 0) < 1 ? data : null;
	}

	private void UpgradeCatholicAddresses(FuturemudDatabaseContext context, Clan clan)
	{
		var prog = context.FutureProgs.SingleOrDefault(x => x.FunctionName == $"ClanTemplate_{clan.Id}_Profile");
		var data = prog is null ? null : ReadAddressUpgradeProfile(prog);
		if (data is null || !clan.IsTemplate) return;
		var stock = CatholicTemplateProfiles.Create(data["TemplateKey"]!.GetValue<string>());
		foreach (var variant in data["Variants"]?.AsArray() ?? [])
			stock = ClanTemplateDefinitions.WithVariant(stock, variant!.GetValue<string>());
		var rankIds = data["Ranks"]!.AsObject().ToDictionary(x => x.Key, x => x.Value!.GetValue<long>());
		var officeIds = data["Appointments"]!.AsObject().ToDictionary(x => x.Key, x => x.Value!.GetValue<long>());
		var oldDefinition = data["Definition"]!.AsObject();
		var definition = oldDefinition.Deserialize<ClanTemplateDefinition>()!;
		// Existing authored context (including custom owners/bindings) remains authoritative.
		foreach (var rank in oldDefinition["ranks"]!.AsArray())
		{
			var key = rank!["key"]!.GetValue<string>();
			if (!rank.AsObject().ContainsKey("FormsOfAddress"))
				definition.Ranks.Single(x => x.Key == key).FormsOfAddress = stock.Ranks.SingleOrDefault(x => x.Key == key)?.FormsOfAddress ?? [];
		}
		foreach (var office in oldDefinition["appointments"]!.AsArray())
		{
			var key = office!["key"]!.GetValue<string>();
			var source = definition.Appointments.Single(x => x.Key == key);
			var fresh = stock.Appointments.SingleOrDefault(x => x.Key == key);
			if (!office.AsObject().ContainsKey("FormsOfAddress")) source.FormsOfAddress = fresh?.FormsOfAddress ?? [];
			if (!office.AsObject().ContainsKey("AddressPriority")) source.AddressPriority = fresh?.AddressPriority ?? 0;
		}
		ClanTemplateDefinitions.Validate(definition);
		InstallCatholicAddresses(context, clan, definition,
			rankIds.ToDictionary(x => x.Key, x => context.Ranks.Find(x.Value)!),
			officeIds.ToDictionary(x => x.Key, x => context.Appointments.Find(x.Value)!));
		// Enrich only the new fields. Preserve names, governance, variants and other profile edits.

		foreach (var rank in oldDefinition["ranks"]!.AsArray())
		{
			var source = definition.Ranks.Single(x => x.Key == rank!["key"]!.GetValue<string>());
			if (!rank!.AsObject().ContainsKey("FormsOfAddress"))
				rank["FormsOfAddress"] = JsonSerializer.SerializeToNode(source.FormsOfAddress);
		}
		foreach (var office in oldDefinition["appointments"]!.AsArray())
		{
			var source = definition.Appointments.Single(x => x.Key == office!["key"]!.GetValue<string>());
			if (!office!.AsObject().ContainsKey("FormsOfAddress"))
				office["FormsOfAddress"] = JsonSerializer.SerializeToNode(source.FormsOfAddress);
			if (!office.AsObject().ContainsKey("AddressPriority"))
				office["AddressPriority"] = context.Appointments.Find(officeIds[source.Key])!.AddressPriority;
		}
		// New variant recipes may change courtesy styles, e.g. a female monastic house.
		foreach (var existingVariant in oldDefinition["AvailableVariants"]!.AsArray())
		{
			var recipe = stock.AvailableVariants.SingleOrDefault(x => x.Key == existingVariant!["Key"]!.GetValue<string>());
			if (recipe is null) continue;
			foreach (var field in new[] { "RankOverrides", "AppointmentOverrides" })
			{
				var fresh = JsonSerializer.SerializeToNode(recipe)![field]!.AsObject();
				var old = existingVariant![field]!.AsObject();
				foreach (var patch in fresh)
				foreach (var addition in patch.Value!.AsObject().Where(x => x.Key is "FormsOfAddress" or "AddressPriority"))
				{
					if (old[patch.Key] is null) old[patch.Key] = new JsonObject();
					if (old[patch.Key]![addition.Key] is null) old[patch.Key]![addition.Key] = addition.Value?.DeepClone();
				}
			}
		}
		data["AddressContentVersion"] = 1;
		prog!.FunctionComment = ProfilePrefix + data.ToJsonString();
		context.SaveChanges();
	}

	private void InstallCatholicAddresses(FuturemudDatabaseContext context, Clan clan, ClanTemplateDefinition definition,
		Dictionary<string, Rank> ranks, Dictionary<string, Appointment> offices)
	{
		long? Predicate(ClanTemplateAddressRule source, string key, Guid id)
		{
			if (source.ProgName is { } shared)
			{
				var prog = context.FutureProgs.SingleOrDefault(x => x.FunctionName == shared);
				var parameters = prog?.FutureProgsParameters.OrderBy(x => x.ParameterIndex).Select(x => x.ParameterType).ToList();
				if (prog is null || prog.ReturnType != (long)ProgVariableTypes.Boolean || prog.AcceptsAnyParameters ||
				    parameters is null || !(parameters.SequenceEqual([(long)ProgVariableTypes.Character]) ||
				                           parameters.SequenceEqual([(long)ProgVariableTypes.Character, (long)ProgVariableTypes.Character])))
					throw new InvalidOperationException($"Invalid shared address predicate {shared}.");
				return prog.Id;
			}
			if (source.Conditions.Count == 0) return null;
			var name = $"ClanTemplate_{clan.Id}_Address_{id:N}";
			EnsureTemplateElectionPlaceholderProg(context, name, $"Clan-bound address predicate; {key}; inactive stock template.",
				ProgVariableTypes.Boolean, "return false");
			return context.FutureProgs.Single(x => x.FunctionName == name).Id;
		}
		foreach (var source in definition.Ranks)
		{
			var rank = ranks[source.Key];
			if (rank.FormsOfAddress.Count > 0) { source.FormsOfAddress = []; continue; }
			foreach (var (rule, order) in source.FormsOfAddress.Select((x, i) => (x, i)))
			{
				var id = Guid.NewGuid();
				var key = $"rank:{source.Key}:{rule.Key}";
				rank.FormsOfAddress.Add(new RankFormOfAddress { Id = id, Text = rule.Text, Relations = (int)rule.Relations,
					IsPublic = rule.IsPublic, TemplateKey = key, FutureProgId = Predicate(rule, key, id), Order = order });
			}
		}
		foreach (var source in definition.Appointments)
		{
			var office = offices[source.Key];
			if (office.FormsOfAddress.Count > 0) { source.FormsOfAddress = []; continue; }
			if (office.AddressPriority == 0) office.AddressPriority = source.AddressPriority;
			foreach (var (rule, order) in source.FormsOfAddress.Select((x, i) => (x, i)))
			{
				var id = Guid.NewGuid();
				var key = $"office:{source.Key}:{rule.Key}";
				office.FormsOfAddress.Add(new AppointmentFormOfAddress { Id = id, Text = rule.Text, Relations = (int)rule.Relations,
					IsPublic = rule.IsPublic, TemplateKey = key, FutureProgId = Predicate(rule, key, id), Order = order });
			}
		}
		context.SaveChanges();
	}
}
