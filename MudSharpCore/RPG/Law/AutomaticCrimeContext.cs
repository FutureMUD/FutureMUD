#nullable enable
using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.RPG.Law;

internal sealed class AutomaticCrimeContext
{
	private readonly IReadOnlyDictionary<string, string> _values;

	private AutomaticCrimeContext(IReadOnlyDictionary<string, string> values)
	{
		_values = values;
	}

	public string? Kind => Value("automatic");

	public static string DescribeForCrimeInfo(string additionalInformation, ICharacter enforcer,
		IPerceivable? thirdParty, bool includeRawContext)
	{
		var sb = new StringBuilder();
		sb.AppendLine("Automatic Evidence:");
		if (!TryParse(additionalInformation, out var context))
		{
			sb.AppendLine($"\tSource: {"Automatic engine report".ColourName()}");
			sb.AppendLine("\tBasis: This crime was recorded automatically by the engine.");
			AppendRawContext(sb, additionalInformation, includeRawContext);
			return sb.ToString().TrimEnd();
		}

		foreach (var line in context.EvidenceLines(enforcer, thirdParty))
		{
			sb.AppendLine($"\t{line}");
		}

		AppendRawContext(sb, additionalInformation, includeRawContext);
		return sb.ToString().TrimEnd();
	}

	private IEnumerable<string> EvidenceLines(ICharacter enforcer, IPerceivable? thirdParty)
	{
		switch (Kind?.ToLowerInvariant())
		{
			case "death":
				return DeathEvidenceLines(enforcer, thirdParty);
			case "wound":
				return WoundEvidenceLines(enforcer, thirdParty);
			case "property-entry":
				return PropertyEntryEvidenceLines();
			case "arena-bet":
				return ArenaBetEvidenceLines();
			case "contraband-boundary":
				return ContrabandBoundaryEvidenceLines(enforcer, thirdParty);
			default:
				return UnknownEvidenceLines();
		}
	}

	private IEnumerable<string> DeathEvidenceLines(ICharacter enforcer, IPerceivable? thirdParty)
	{
		var severity = SafeValue("maxseverity") ?? "an unknown-severity";
		var damageType = SafeValue("damagetype") ?? "unknown";
		var bodypart = SafeValue("bodypart");
		var age = SafeValue("woundage");
		var friendly = SafeValue("friendly")?.EqualTo("true") == true ? "friendly" : "non-friendly";
		var attackerPresent = SafeValue("attackerpresent")?.EqualTo("true") == true;
		var injury = $"{severity} {damageType} wound";
		if (!string.IsNullOrWhiteSpace(bodypart))
		{
			injury = $"{injury} to {bodypart}";
		}

		yield return $"Source: {"Automatic death investigation".ColourName()}";
		yield return $"Basis: A recent {friendly} wound was attributed to the accused.";
		yield return $"Injury: {injury.ColourValue()}";
		if (!string.IsNullOrWhiteSpace(age))
		{
			yield return $"Timing: The wound was recorded {age.ColourValue()} before death.";
		}

		yield return $"Scene: {(attackerPresent ? "The accused was present at the death scene." : "The accused was not seen at the death scene.").ColourValue()}";
		yield return $"Implement: {DescribeThirdParty(enforcer, thirdParty)}";
		yield return "Note: This is mechanical evidence for the court to weigh, not an automatic guilty verdict.";
	}

	private IEnumerable<string> WoundEvidenceLines(ICharacter enforcer, IPerceivable? thirdParty)
	{
		var severity = SafeValue("severity") ?? "an unknown-severity";
		var damageType = SafeValue("damagetype") ?? "unknown";
		var bodypart = SafeValue("bodypart");
		var injury = $"{severity} {damageType} wound";
		if (!string.IsNullOrWhiteSpace(bodypart))
		{
			injury = $"{injury} to {bodypart}";
		}

		yield return $"Source: {"Automatic injury report".ColourName()}";
		yield return "Basis: A qualifying wound was attributed to the accused.";
		yield return $"Injury: {injury.ColourValue()}";
		yield return $"Implement: {DescribeThirdParty(enforcer, thirdParty)}";
	}

	private IEnumerable<string> PropertyEntryEvidenceLines()
	{
		yield return $"Source: {"Automatic property entry report".ColourName()}";
		yield return "Basis: Entry into a protected property was recorded without matching authorisation.";
		if (SafeValue("propertyname") is { } propertyName)
		{
			yield return $"Property: {propertyName.ColourValue()}";
		}
	}

	private IEnumerable<string> ArenaBetEvidenceLines()
	{
		yield return $"Source: {"Automatic wagering report".ColourName()}";
		yield return "Basis: An arena wager was placed.";
		if (SafeValue("stake") is { } stake)
		{
			yield return $"Stake: {stake.ColourValue()}";
		}

		if (SafeValue("model") is { } model)
		{
			yield return $"Bet Type: {model.ColourValue()}";
		}
	}

	private IEnumerable<string> ContrabandBoundaryEvidenceLines(ICharacter enforcer, IPerceivable? thirdParty)
	{
		yield return $"Source: {"Automatic border contraband report".ColourName()}";
		yield return "Basis: Entry into a protected jurisdiction was recorded while carrying a potential contraband item.";
		yield return $"Item: {DescribeThirdParty(enforcer, thirdParty, SafeValue("itemname"))}";
	}

	private IEnumerable<string> UnknownEvidenceLines()
	{
		yield return $"Source: {"Automatic engine report".ColourName()}";
		yield return "Basis: This crime was recorded automatically by the engine.";
		if (SafeValue("automatic") is { } signal)
		{
			yield return $"Signal: {signal.ColourValue()}";
		}
	}

	private static bool TryParse(string text, out AutomaticCrimeContext context)
	{
		var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		foreach (var segment in text.Split(';'))
		{
			var trimmed = segment.Trim();
			if (trimmed.Length == 0)
			{
				continue;
			}

			var index = trimmed.IndexOf('=');
			if (index <= 0)
			{
				continue;
			}

			var key = trimmed[..index].Trim();
			var value = trimmed[(index + 1)..].Trim();
			if (key.Length == 0)
			{
				continue;
			}

			values[key] = value;
		}

		context = new AutomaticCrimeContext(values);
		return values.ContainsKey("automatic");
	}

	private string? Value(string key)
	{
		return _values.TryGetValue(key, out var value) ? value : null;
	}

	private string? SafeValue(string key)
	{
		var value = Value(key);
		if (string.IsNullOrWhiteSpace(value) || value.StartsWith('#'))
		{
			return null;
		}

		return value;
	}

	private static string DescribeThirdParty(ICharacter enforcer, IPerceivable? thirdParty, string? fallback = null)
	{
		return thirdParty?.HowSeen(enforcer, flags: PerceiveIgnoreFlags.TrueDescription) ??
		       fallback?.ColourValue() ??
		       "No identified implement".ColourError();
	}

	private static void AppendRawContext(StringBuilder sb, string additionalInformation, bool includeRawContext)
	{
		if (includeRawContext)
		{
			sb.AppendLine($"\tRaw Context: {additionalInformation.ColourCommand()}");
		}
	}
}
