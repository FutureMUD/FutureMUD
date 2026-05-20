#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public sealed record AgricultureCustomScoreDefinition(
	AgricultureScoreType ScoreType,
	bool Enabled,
	string Name,
	bool HigherIsGood)
{
	public static AgricultureCustomScoreDefinition Default(AgricultureScoreType score)
	{
		return new AgricultureCustomScoreDefinition(score, false, score.DescribeEnum(true), true);
	}

	public XElement SaveToXml()
	{
		return new XElement("Score",
			new XAttribute("type", ScoreType.ToString()),
			new XAttribute("enabled", Enabled),
			new XAttribute("name", Name),
			new XAttribute("higherIsGood", HigherIsGood));
	}
}

public readonly record struct AgricultureScoreRange
{
	public AgricultureScoreRange(AgricultureScoreType score, int minimum, int maximum)
	{
		Score = score;
		Minimum = Math.Clamp(minimum, 0, 100);
		Maximum = Math.Clamp(maximum, 0, 100);
		if (Maximum < Minimum)
		{
			(Minimum, Maximum) = (Maximum, Minimum);
		}
	}

	public AgricultureScoreType Score { get; }
	public int Minimum { get; }
	public int Maximum { get; }

	public bool Contains(int value)
	{
		return value >= Minimum && value <= Maximum;
	}
}

public static class AgricultureScoreTypeExtensions
{
	public const string CustomScoreConfigurationStaticConfiguration = "AgricultureCustomScoreTypes";

	private static readonly AgricultureScoreType[] BuiltInScoreTypes =
	[
		AgricultureScoreType.Moisture,
		AgricultureScoreType.Drainage,
		AgricultureScoreType.Nutrients,
		AgricultureScoreType.Salinity,
		AgricultureScoreType.Topsoil,
		AgricultureScoreType.Tilth,
		AgricultureScoreType.Rockiness,
		AgricultureScoreType.Weeds,
		AgricultureScoreType.Pests,
		AgricultureScoreType.Fence,
		AgricultureScoreType.Pasture,
		AgricultureScoreType.Condition
	];

	private static readonly AgricultureScoreType[] CustomScoreTypes =
	[
		AgricultureScoreType.Custom1,
		AgricultureScoreType.Custom2,
		AgricultureScoreType.Custom3,
		AgricultureScoreType.Custom4,
		AgricultureScoreType.Custom5,
		AgricultureScoreType.Custom6,
		AgricultureScoreType.Custom7,
		AgricultureScoreType.Custom8,
		AgricultureScoreType.Custom9,
		AgricultureScoreType.Custom10,
		AgricultureScoreType.Custom11,
		AgricultureScoreType.Custom12
	];

	public static string DefaultCustomScoreConfigurationText =>
		new XElement("AgricultureScoreTypes",
			CustomScoreTypes.Select(x => AgricultureCustomScoreDefinition.Default(x).SaveToXml())).ToString();

	public static IEnumerable<AgricultureScoreType> BuiltInScores => BuiltInScoreTypes;
	public static IEnumerable<AgricultureScoreType> CustomScores => CustomScoreTypes;

	public static bool IsCustomScore(this AgricultureScoreType score)
	{
		return CustomScoreTypes.Contains(score);
	}

	public static bool IsBuiltInScore(this AgricultureScoreType score)
	{
		return BuiltInScoreTypes.Contains(score);
	}

	public static bool IsEnabledScore(this AgricultureScoreType score, IFuturemud? gameworld)
	{
		return score.IsBuiltInScore() ||
		       score.IsCustomScore() &&
		       GetCustomScoreConfiguration(gameworld)[score].Enabled;
	}

	public static IEnumerable<AgricultureScoreType> ActiveScoreTypes(IFuturemud? gameworld)
	{
		return BuiltInScoreTypes.Concat(CustomScoreTypes.Where(x => GetCustomScoreConfiguration(gameworld)[x].Enabled));
	}

	public static IReadOnlyDictionary<AgricultureScoreType, AgricultureCustomScoreDefinition> GetCustomScoreConfiguration(
		IFuturemud? gameworld)
	{
		var definitions = CustomScoreTypes.ToDictionary(x => x, AgricultureCustomScoreDefinition.Default);
		var text = GetConfigurationText(gameworld);
		if (string.IsNullOrWhiteSpace(text))
		{
			return definitions;
		}

		try
		{
			var root = XElement.Parse(text);
			foreach (var element in root.Elements("Score"))
			{
				var typeText = (string?)element.Attribute("type") ?? string.Empty;
				if (!TryParseCustomSlot(typeText, out var score))
				{
					continue;
				}

				var name = (string?)element.Attribute("name");
				definitions[score] = new AgricultureCustomScoreDefinition(
					score,
					(bool?)element.Attribute("enabled") ?? false,
					string.IsNullOrWhiteSpace(name) ? score.DescribeEnum(true) : name,
					(bool?)element.Attribute("higherIsGood") ?? true);
			}
		}
		catch (Exception)
		{
			return definitions;
		}

		return definitions;
	}

	public static XElement SaveCustomScoreConfiguration(
		IReadOnlyDictionary<AgricultureScoreType, AgricultureCustomScoreDefinition> definitions)
	{
		return new XElement("AgricultureScoreTypes",
			CustomScoreTypes.Select(x =>
				(definitions.TryGetValue(x, out var definition) ? definition : AgricultureCustomScoreDefinition.Default(x)).SaveToXml()));
	}

	public static bool TryParseScoreType(string? text, IFuturemud? gameworld, out AgricultureScoreType score,
		bool includeDisabledCustomSlots = false)
	{
		score = default;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		if (TryParseEnumName(text, out score))
		{
			return score.IsBuiltInScore() ||
			       score.IsCustomScore() &&
			       (includeDisabledCustomSlots || GetCustomScoreConfiguration(gameworld)[score].Enabled);
		}

		foreach (var definition in GetCustomScoreConfiguration(gameworld).Values)
		{
			if ((definition.Enabled || includeDisabledCustomSlots) &&
			    text.Trim().Equals(definition.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				score = definition.ScoreType;
				return true;
			}
		}

		return false;
	}

	public static bool TryParseCustomSlot(string? text, out AgricultureScoreType score)
	{
		score = default;
		return TryParseEnumName(text, out score) && score.IsCustomScore();
	}

	public static string DescribeFor(this AgricultureScoreType score, IFuturemud? gameworld)
	{
		if (!score.IsCustomScore())
		{
			return score.DescribeEnum();
		}

		return GetCustomScoreConfiguration(gameworld)[score] is { Enabled: true } definition
			? definition.Name
			: score.DescribeEnum(true);
	}

	public static bool HigherIsGood(this AgricultureScoreType score, IFuturemud? gameworld)
	{
		if (score.IsCustomScore())
		{
			return GetCustomScoreConfiguration(gameworld)[score].HigherIsGood;
		}

		return score switch
		{
			AgricultureScoreType.Salinity or
			AgricultureScoreType.Rockiness or
			AgricultureScoreType.Weeds or
			AgricultureScoreType.Pests => false,
			_ => true
		};
	}

	private static string? GetConfigurationText(IFuturemud? gameworld)
	{
		try
		{
			return gameworld?.GetStaticConfiguration(CustomScoreConfigurationStaticConfiguration);
		}
		catch (Exception)
		{
			return DefaultCustomScoreConfigurationText;
		}
	}

	private static bool TryParseEnumName(string? text, out AgricultureScoreType score)
	{
		score = default;
		if (string.IsNullOrWhiteSpace(text))
		{
			return false;
		}

		var compact = text.Trim()
		                  .Replace(" ", string.Empty)
		                  .Replace("-", string.Empty)
		                  .Replace("_", string.Empty);
		return Enum.TryParse(compact, true, out score);
	}
}
