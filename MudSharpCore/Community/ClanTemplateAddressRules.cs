#nullable enable

using MudSharp.Character;
using MudSharp.Community.Templates;

namespace MudSharp.Community;

public static partial class ClanTemplateProvisioning
{
	public static string AddressKey(string kind, string ownerKey, string ruleKey) => $"{kind}:{ownerKey}:{ruleKey}";

	internal static bool IsGeneratedAddressProg(IFutureProg? prog) => prog is not null &&
		System.Text.RegularExpressions.Regex.IsMatch(prog.FunctionName, @"^ClanTemplate_[0-9]+_Address_[0-9a-fA-F]{32}$");

	/// <summary>Rejects a copied clan-bound predicate without a recipe that can regenerate it for this owner.</summary>
	public static string? AddressCloneError(IFuturemud world, IClan source, ClanTemplateDefinition? definition)
	{
		var profile = ClanTemplateProfile.Read(world, source.Id);
		var supported = new HashSet<Guid>();
		if (definition is not null && profile is not null)
		{
			foreach (var owner in definition.Ranks)
			{
				var rank = source.Ranks.SingleOrDefault(x => x.Id == profile.Ranks.GetValueOrDefault(owner.Key));
				if (rank is null) continue;
				foreach (var rule in rank.FormsOfAddress.Where(x => owner.FormsOfAddress.Any(y => y.Conditions.Count > 0 && x.TemplateKey == AddressKey("rank", owner.Key, y.Key)))) supported.Add(rule.Id);
			}
			foreach (var owner in definition.Appointments)
			{
				var office = source.Appointments.SingleOrDefault(x => x.Id == profile.Appointments.GetValueOrDefault(owner.Key));
				if (office is null) continue;
				foreach (var rule in office.FormsOfAddress.Where(x => owner.FormsOfAddress.Any(y => y.Conditions.Count > 0 && x.TemplateKey == AddressKey("office", owner.Key, y.Key)))) supported.Add(rule.Id);
			}
		}
		return source.Ranks.SelectMany(x => x.FormsOfAddress).Concat(source.Appointments.SelectMany(x => x.FormsOfAddress))
			.Any(x => x.FutureProgId is { } id && IsGeneratedAddressProg(world.FutureProgs.Get(id)) && !supported.Contains(x.Id))
			? "A clan-bound address predicate has no matching owner/profile recipe. Restore its recipe binding or select a shared predicate before copying; no clone was created." : null;
	}

	private static void GenerateAddressProgs(IFuturemud world, IClan clan, ClanTemplateProfile profile)
	{
		foreach (var source in profile.Definition.Ranks)
		{
			var rank = clan.Ranks.Single(x => x.Id == profile.Ranks[source.Key]);
			Generate(rank.FormsOfAddress, source.FormsOfAddress, "rank", source.Key);
			rank.Changed = true;
			rank.Save();
		}
		foreach (var source in profile.Definition.Appointments)
		{
			var office = clan.Appointments.Single(x => x.Id == profile.Appointments[source.Key]);
			Generate(office.FormsOfAddress, source.FormsOfAddress, "office", source.Key);
			office.Changed = true;
			office.Save();
		}

		void Generate(List<ClanAddressRule> rules, List<ClanTemplateAddressRule> definitions, string kind, string owner)
		{
			foreach (var definition in definitions)
			{
				var key = AddressKey(kind, owner, definition.Key);
				var rule = rules.SingleOrDefault(x => x.TemplateKey == key);
				// Missing/edited rows are builder choices, not a reason to restore stock rows.
				if (rule is null) continue;
				if (definition.Conditions.Count > 0)
				{
					var name = $"ClanTemplate_{clan.Id}_Address_{rule.Id:N}";
					var prog = SaveProg(world, name, $"Clan-bound address predicate; {key}.", ProgVariableTypes.Boolean,
						$"return ClanTemplateEligibilityError(@ch, {clan.Id}, 0, \"address:{rule.Id}\") == \"\"");
					rule.FutureProgId = prog.Id;
					profile.GeneratedProgs[key] = prog.Id;
				}
				else if (definition.ProgName is { } progName)
				{
					var prog = world.FutureProgs.GetByName(progName)
						?? throw new InvalidOperationException($"Missing shared address predicate {progName}.");
					if (prog.ReturnType != ProgVariableTypes.Boolean || prog.AcceptsAnyParameters ||
					    !(prog.MatchesParameters([ProgVariableTypes.Character]) || prog.MatchesParameters([ProgVariableTypes.Character, ProgVariableTypes.Character])))
						throw new InvalidOperationException($"Invalid shared address predicate {progName}.");
					rule.FutureProgId = prog.Id;
				}
			}
		}
	}

	/// <summary>Applies deliberate keyed recipe changes while retaining unrelated builder rows.</summary>
	private static void UpdateAddressRules(List<ClanAddressRule> live, List<ClanTemplateAddressRule> before,
		List<ClanTemplateAddressRule> after, string kind, string owner)
	{
		foreach (var removed in before.Where(x => after.All(y => y.Key != x.Key)))
			live.RemoveAll(x => x.TemplateKey == AddressKey(kind, owner, removed.Key));
		foreach (var definition in after)
		{
			var previous = before.SingleOrDefault(x => x.Key == definition.Key);
			if (previous is not null && System.Text.Json.JsonSerializer.Serialize(previous) == System.Text.Json.JsonSerializer.Serialize(definition)) continue;
			var key = AddressKey(kind, owner, definition.Key);
			var index = live.FindIndex(x => x.TemplateKey == key);
			var rule = new ClanAddressRule { Text = definition.Text, Relations = definition.Relations, IsPublic = definition.IsPublic, TemplateKey = key };
			if (index >= 0) { rule.Id = live[index].Id; live[index] = rule; }
			else if (previous is null) live.Insert(0, rule);
		}
		// Profile order owns only its own slots; unrelated builder rules retain their positions.
		var owned = live.Where(x => after.Any(y => x.TemplateKey == AddressKey(kind, owner, y.Key)))
			.OrderBy(x => after.FindIndex(y => x.TemplateKey == AddressKey(kind, owner, y.Key))).ToList();
		var next = 0;
		for (var i = 0; i < live.Count; i++)
			if (after.Any(y => live[i].TemplateKey == AddressKey(kind, owner, y.Key))) live[i] = owned[next++];
	}
}

public static partial class ClanTemplateEligibility
{
	private static string AddressError(IFuturemud world, ICharacter? subject, IClan? clan, ClanTemplateProfile? profile, string idText)
	{
		if (subject is null || clan is null || clan.IsTemplate || profile?.Activated != true || !Guid.TryParse(idText, out var id))
			return "The address profile is unavailable.";
		var membership = subject.ClanMemberships.FirstOrDefault(x => x.Clan == clan && !x.IsArchivedMembership);
		if (membership is null) return "Active membership is required.";
		var key = membership.Rank.FormsOfAddress.Concat(membership.Appointments.SelectMany(x => x.FormsOfAddress))
			.FirstOrDefault(x => x.Id == id)?.TemplateKey;
		if (key is null) return "This address source is not held by the subject.";
		var definition = profile.Definition.Ranks.SelectMany(x => x.FormsOfAddress.Select(y => (Key: ClanTemplateProvisioning.AddressKey("rank", x.Key, y.Key), Rule: y)))
			.Concat(profile.Definition.Appointments.SelectMany(x => x.FormsOfAddress.Select(y => (Key: ClanTemplateProvisioning.AddressKey("office", x.Key, y.Key), Rule: y))))
			.FirstOrDefault(x => x.Key == key).Rule;
		if (definition is null) return "The address recipe is missing.";
		foreach (var condition in definition.Conditions)
		{
			var target = condition.Role == "$self" ? clan : profile.Bindings.TryGetValue(condition.Role, out var binding) ? world.Clans.Get(binding.ClanId) : null;
			if (target is null || target.IsTemplate) return condition.Error;
			var mapping = target == clan ? profile : ClanTemplateProfile.Read(world, target.Id);
			var member = target.Memberships.FirstOrDefault(x => x.MemberId == CharacterInstanceIdentityComparer.IdentityId(subject) && !x.IsArchivedMembership);
			if (member is null) return condition.Error;
			if (condition.Kind == "membership") continue;
			if (mapping is null || condition.Key is null) return condition.Error;
			if (condition.Kind == "office")
			{
				if (member.Appointments.All(x => x.Id != mapping.Appointments.GetValueOrDefault(condition.Key))) return condition.Error;
				continue;
			}
			var rank = target.Ranks.FirstOrDefault(x => x.Id == mapping.Ranks.GetValueOrDefault(condition.Key));
			if (rank is null || !(condition.Kind switch
			    {
				    "minimumrank" => member.Rank.RankNumber >= rank.RankNumber,
				    "exactrank" => member.Rank.Id == rank.Id,
				    _ => false
			    })) return condition.Error;
		}
		return string.Empty;
	}
}
