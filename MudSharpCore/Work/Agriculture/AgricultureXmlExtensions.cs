using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Work.Agriculture;

namespace MudSharp.Work.Agriculture;

internal static class AgricultureXmlExtensions
{
	public static XElement RootOrDefault(string definition, string rootName)
	{
		if (string.IsNullOrWhiteSpace(definition))
		{
			return new XElement(rootName);
		}

		return XElement.Parse(definition);
	}

	public static Dictionary<AgricultureScoreType, int> LoadScores(this XElement root)
	{
		return root.Elements("Score")
		           .Select(x => (
			           Success: AgricultureScoreTypeExtensions.TryParseScoreType((string)x.Attribute("type"), null,
				           out var type, true),
			           Type: type,
			           Value: (int?)x.Attribute("value") ?? 50))
		           .Where(x => x.Success)
		           .ToDictionary(x => x.Type, x => Math.Clamp(x.Value, 0, 100));
	}

	public static Dictionary<AgricultureScoreType, AgricultureScoreRange> LoadScoreRanges(this XElement root)
	{
		return root.Elements("Score")
		           .Select(x => (
			           Success: AgricultureScoreTypeExtensions.TryParseScoreType((string)x.Attribute("type"), null,
				           out var type, true),
			           Type: type,
			           Minimum: (int?)x.Attribute("min") ?? 0,
			           Maximum: (int?)x.Attribute("max") ?? 100))
		           .Where(x => x.Success)
		           .ToDictionary(x => x.Type, x => new AgricultureScoreRange(x.Type, x.Minimum, x.Maximum));
	}

	public static XElement SaveScores(string rootName, IReadOnlyDictionary<AgricultureScoreType, int> scores)
	{
		return new XElement(rootName,
			scores.Select(x => new XElement("Score",
				new XAttribute("type", x.Key.ToString()),
				new XAttribute("value", Math.Clamp(x.Value, 0, 100)))));
	}

	public static HashSet<AgricultureFieldUse> LoadUses(this XElement root, bool defaultAll = true)
	{
		var text = (string)root.Attribute("uses") ?? string.Empty;
		var result = new HashSet<AgricultureFieldUse>();
		foreach (var item in text.Split(new[] { ' ', ',', ';', '|' }, StringSplitOptions.RemoveEmptyEntries))
		{
			if (Enum.TryParse<AgricultureFieldUse>(item, true, out var use))
			{
				result.Add(use);
			}
		}

		if (!result.Any() && defaultAll)
		{
			result.Add(AgricultureFieldUse.Fallow);
			result.Add(AgricultureFieldUse.Crop);
			result.Add(AgricultureFieldUse.Pasture);
			result.Add(AgricultureFieldUse.Woodland);
			result.Add(AgricultureFieldUse.Orchard);
		}

		return result;
	}

	public static int ClampScore(this int value)
	{
		return Math.Clamp(value, 0, 100);
	}
}
