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
			           Success: Enum.TryParse<AgricultureScoreType>((string)x.Attribute("type"), true, out var type),
			           Type: type,
			           Value: (int?)x.Attribute("value") ?? 50))
		           .Where(x => x.Success)
		           .ToDictionary(x => x.Type, x => Math.Clamp(x.Value, 0, 100));
	}

	public static XElement SaveScores(string rootName, IReadOnlyDictionary<AgricultureScoreType, int> scores)
	{
		return new XElement(rootName,
			scores.Select(x => new XElement("Score",
				new XAttribute("type", x.Key.ToString()),
				new XAttribute("value", Math.Clamp(x.Value, 0, 100)))));
	}

	public static HashSet<AgricultureFieldUse> LoadUses(this XElement root)
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

		if (!result.Any())
		{
			result.Add(AgricultureFieldUse.Fallow);
			result.Add(AgricultureFieldUse.Crop);
			result.Add(AgricultureFieldUse.Pasture);
			result.Add(AgricultureFieldUse.Woodland);
		}

		return result;
	}

	public static int ClampScore(this int value)
	{
		return Math.Clamp(value, 0, 100);
	}
}
