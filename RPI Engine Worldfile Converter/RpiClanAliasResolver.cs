#nullable enable

using MudSharp.Framework;

namespace RPI_Engine_Worldfile_Converter;

public sealed record RpiCanonicalClanRule(
	string CanonicalAlias,
	string? FullNameOverride,
	IReadOnlyList<string> LegacyAliases,
	bool ImportFromUnrankedReferences = false);

public static class RpiClanAliasResolver
{
	private static readonly IReadOnlyList<RpiCanonicalClanRule> CanonicalRuleList =
	[
		new("mordor_char", "Minas Morgul", ["mm_denizens"]),
		new("malred", "Malred Family", ["housemalred"]),
		new("rogues", "Rogues' Fellowship", ["rouges"]),
		new("hawk_dove_2", "Hawk and Dove", ["hawk_and_dove"]),
		new("fahad_jafari", "Fahad Jafari", ["fahad-jafari"]),
		new("seekers", "Seekers", Array.Empty<string>()),
		new("shadow-cult", "Shadow Cult", Array.Empty<string>()),
		new("tirithguard", "Minas Tirith Guard", Array.Empty<string>()),
		new("eradan_battalion", "Eradan Battalion", ["eradan_battalions"]),
		new("ithilien_battalion", "Ithilien Battalion", ["ithilien_battalions"]),
		new("gothakra", "Gothakra Warband", ["gothraka"]),
		new("jewelers", "Jewelers", ["jewellers"]),
		new("mt_theatre", "Minas Tirith Theatre", ["m_t_theatre", "mt_theater"]),
		new("wardogs", "Wardogs", ["wardog"]),

		new("com", "Cult of Morgoth", Array.Empty<string>()),
		new("com_priests", "Cult of Morgoth Priests", ["com-priests"], true),
		new("tecouncil", "Tur Edendor Council", Array.Empty<string>(), true),
		new("metalsmiths_fellowship", "Metalsmiths Fellowship", ["metalsmithsfellow"], true),
		new("mm_slaves", "Minas Morgul Slaves", Array.Empty<string>(), true),
		new("mt_ratcatchers", "Minas Tirith Ratcatchers", Array.Empty<string>(), true),
		new("osgi_ratcatchers", "Osgiliath Rat Catchers", Array.Empty<string>(), true),

		new("witchkings_horde", "Witchking's Horde",
			["witchking_horde", "witchkinghorde", "witchkings_horde", "withchking_horde", "witchking_horse"]),
		new("pel_pelennor", "Pel Pelennor", ["pel_pelenor"]),
		new("osgi_citizens", "Osgiliath Citizens", ["osgi_citizen", "osgi_citizensmember", "osgi_citzens"]),

		new("abominations", "Abominations", ["abomination"]),
		new("carnivores", "Carnivores", ["carnivore"]),
		new("cobra_enforcers", "Cobra Enforcers", ["cobra_enforcer"]),
		new("fahad_slummers", "Fahad Slummers", ["fahad_slummer"]),
		new("harlequins", "Harlequins", ["harlequin"]),
		new("mineworkers", "Mineworkers", ["mineworker"]),
		new("mordor_slavers", "Mordor Slavers", ["mordor_slaver"]),
		new("moria_hordes", "Moria Hordes", ["moria_horde"]),
		new("outpost_guilds", "Outpost Guilds", ["outpost_guild"]),
		new("scorpions", "Scorpions", ["scorpion"]),
		new("slavers", "Slavers", ["slaver"]),
		new("watchers", "Watchers", ["watcher"]),
	];

	private static readonly IReadOnlyDictionary<string, RpiCanonicalClanRule> RulesByAlias = BuildRulesByAlias();

	private static readonly IReadOnlyDictionary<string, RpiCanonicalClanRule> RulesByCollapsedAlias =
		BuildRulesByCollapsedAlias();

	public static string CollapseAlias(string alias)
	{
		return alias.Trim().ToLowerInvariant().CollapseString();
	}

	public static RpiCanonicalClanRule ResolveCanonicalRule(string alias)
	{
		if (string.IsNullOrWhiteSpace(alias))
		{
			return new RpiCanonicalClanRule(string.Empty, null, Array.Empty<string>());
		}

		var trimmed = alias.Trim();
		if (RulesByAlias.TryGetValue(trimmed, out var rule))
		{
			return rule;
		}

		var collapsed = CollapseAlias(trimmed);
		if (RulesByCollapsedAlias.TryGetValue(collapsed, out rule))
		{
			return rule;
		}

		return new RpiCanonicalClanRule(collapsed, null, Array.Empty<string>());
	}

	public static bool AreAliasesEquivalent(string lhs, string rhs)
	{
		return ResolveCanonicalRule(lhs).CanonicalAlias.Equals(
			ResolveCanonicalRule(rhs).CanonicalAlias,
			StringComparison.OrdinalIgnoreCase);
	}

	private static IReadOnlyDictionary<string, RpiCanonicalClanRule> BuildRulesByAlias()
	{
		var result = new Dictionary<string, RpiCanonicalClanRule>(StringComparer.OrdinalIgnoreCase);
		foreach (var rule in CanonicalRuleList)
		{
			RegisterRuleAlias(result, rule.CanonicalAlias, rule);
			foreach (var alias in rule.LegacyAliases)
			{
				RegisterRuleAlias(result, alias, rule);
			}
		}

		return result;
	}

	private static IReadOnlyDictionary<string, RpiCanonicalClanRule> BuildRulesByCollapsedAlias()
	{
		var result = new Dictionary<string, RpiCanonicalClanRule>(StringComparer.OrdinalIgnoreCase);
		foreach (var rule in CanonicalRuleList)
		{
			RegisterRuleAlias(result, CollapseAlias(rule.CanonicalAlias), rule);
			foreach (var alias in rule.LegacyAliases)
			{
				RegisterRuleAlias(result, CollapseAlias(alias), rule);
			}
		}

		return result;
	}

	private static void RegisterRuleAlias(
		IDictionary<string, RpiCanonicalClanRule> rules,
		string alias,
		RpiCanonicalClanRule rule)
	{
		if (string.IsNullOrWhiteSpace(alias) || rules.ContainsKey(alias))
		{
			return;
		}

		rules[alias] = rule;
	}
}
