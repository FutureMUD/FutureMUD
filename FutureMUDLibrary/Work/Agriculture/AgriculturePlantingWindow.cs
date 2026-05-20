#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using MudSharp.Celestial;
using MudSharp.Climate;
using MudSharp.Framework;

namespace MudSharp.Work.Agriculture;

public enum AgriculturePlantingWindowType
{
	Group,
	Season
}

public sealed record AgriculturePlantingWindow(AgriculturePlantingWindowType Type, string Value)
{
	public XElement SaveToXml()
	{
		return new XElement("Window",
			new XAttribute("type", Type.ToString().ToLowerInvariant()),
			new XAttribute("value", Value));
	}
}

public readonly record struct AgricultureSeasonGroupRange
{
	public AgricultureSeasonGroupRange(string group, double start, double end)
	{
		Group = group;
		Start = NormaliseFraction(start);
		End = NormaliseFraction(end);
	}

	public string Group { get; }
	public double Start { get; }
	public double End { get; }

	public bool Contains(double fraction)
	{
		fraction = NormaliseFraction(fraction);
		return Start <= End
			? fraction >= Start && fraction < End
			: fraction >= Start || fraction < End;
	}

	public XElement SaveToXml()
	{
		return new XElement("Range",
			new XAttribute("start", Start.ToString("0.###", CultureInfo.InvariantCulture)),
			new XAttribute("end", End.ToString("0.###", CultureInfo.InvariantCulture)));
	}

	private static double NormaliseFraction(double value)
	{
		if (double.IsNaN(value) || double.IsInfinity(value))
		{
			return 0.0;
		}

		return Math.Clamp(value.Modulus(1.0), 0.0, 1.0);
	}
}

public static class AgriculturePlantingWindowExtensions
{
	public const string SeasonGroupWindowsStaticConfiguration = "AgricultureSeasonGroupWindows";

	private static readonly AgricultureSeasonGroupRange[] DefaultSeasonGroupRanges =
	[
		new("Winter", 334.0 / 365.0, 61.0 / 365.0),
		new("Spring", 61.0 / 365.0, 153.0 / 365.0),
		new("Summer", 153.0 / 365.0, 244.0 / 365.0),
		new("Autumn", 244.0 / 365.0, 334.0 / 365.0)
	];

	public static string DefaultSeasonGroupWindowsConfigurationText =>
		SaveSeasonGroupWindows(DefaultSeasonGroupRanges).ToString();

	public static bool TryLoadPlantingWindow(this XElement element, out AgriculturePlantingWindow? window)
	{
		window = null;
		var typeText = (string?)element.Attribute("type") ?? string.Empty;
		var value = (string?)element.Attribute("value") ?? string.Empty;
		if (string.IsNullOrWhiteSpace(value) ||
		    !Enum.TryParse<AgriculturePlantingWindowType>(typeText, true, out var type))
		{
			return false;
		}

		window = new AgriculturePlantingWindow(type, value.Trim());
		return true;
	}

	public static XElement SaveSeasonGroupWindows(IEnumerable<AgricultureSeasonGroupRange> ranges)
	{
		return new XElement("AgricultureSeasonGroupWindows",
			ranges.GroupBy(x => x.Group, StringComparer.InvariantCultureIgnoreCase)
			      .Select(x => new XElement("Group",
				      new XAttribute("name", x.Key),
				      x.Select(y => y.SaveToXml()))));
	}

	public static IReadOnlyDictionary<string, IReadOnlyCollection<AgricultureSeasonGroupRange>> GetSeasonGroupWindows(
		IFuturemud? gameworld)
	{
		return LoadSeasonGroupWindows(GetConfigurationText(gameworld));
	}

	public static bool CanPlantIn(this IAgricultureCropDefinition crop, IAgricultureField field, out string reason)
	{
		reason = string.Empty;
		if (crop.PlantingWindows.Count == 0)
		{
			return true;
		}

		if (field?.Cell == null)
		{
			reason = $"The {crop.Name} crop has planting season restrictions, but there is no field location to check.";
			return false;
		}

		var currentSeason = field.Cell.CurrentSeason(null);
		if (currentSeason != null)
		{
			if (crop.PlantingWindows.Any(x => MatchesCurrentSeason(x, currentSeason)))
			{
				return true;
			}

			reason = PlantingWindowMismatchReason(crop);
			return false;
		}

		if (!TryCurrentYearFraction(field, out var fraction))
		{
			reason =
				$"The {crop.Name} crop has planting season restrictions, but this field has no current season or celestial year source.";
			return false;
		}

		var configuredRanges = GetSeasonGroupWindows(field.Gameworld);
		var groupWindows = crop.PlantingWindows
		                       .Where(x => x.Type == AgriculturePlantingWindowType.Group)
		                       .ToList();
		var resolvedAnyGroup = false;
		foreach (var window in groupWindows)
		{
			if (!configuredRanges.TryGetValue(window.Value, out var ranges))
			{
				continue;
			}

			resolvedAnyGroup = true;
			if (ranges.Any(x => x.Contains(fraction)))
			{
				return true;
			}
		}

		if (!resolvedAnyGroup)
		{
			reason =
				$"The {crop.Name} crop has planting season restrictions, but no configured season group window could be resolved.";
			return false;
		}

		reason = PlantingWindowMismatchReason(crop);
		return false;
	}

	public static string DescribePlantingWindows(this IEnumerable<AgriculturePlantingWindow> windows)
	{
		var items = windows.Select(x => x.Type == AgriculturePlantingWindowType.Group
			                   ? $"group {x.Value}"
			                   : $"season {x.Value}")
		                   .ToList();
		return items.Count == 0 ? "any season" : items.ListToString();
	}

	private static string PlantingWindowMismatchReason(IAgricultureCropDefinition crop)
	{
		return $"The {crop.Name} crop can only be planted during {crop.PlantingWindows.DescribePlantingWindows()}.";
	}

	private static IReadOnlyDictionary<string, IReadOnlyCollection<AgricultureSeasonGroupRange>> LoadSeasonGroupWindows(
		string? text)
	{
		var ranges = new List<AgricultureSeasonGroupRange>();
		try
		{
			var root = XElement.Parse(string.IsNullOrWhiteSpace(text)
				? DefaultSeasonGroupWindowsConfigurationText
				: text);
			foreach (var group in root.Elements("Group"))
			{
				var name = (string?)group.Attribute("name") ?? string.Empty;
				if (string.IsNullOrWhiteSpace(name))
				{
					continue;
				}

				foreach (var range in group.Elements("Range"))
				{
					ranges.Add(new AgricultureSeasonGroupRange(
						name.Trim(),
						ParseFraction((string?)range.Attribute("start"), 0.0),
						ParseFraction((string?)range.Attribute("end"), 0.0)));
				}
			}
		}
		catch (Exception)
		{
			ranges.AddRange(DefaultSeasonGroupRanges);
		}

		if (ranges.Count == 0)
		{
			ranges.AddRange(DefaultSeasonGroupRanges);
		}

		return ranges.GroupBy(x => x.Group, StringComparer.InvariantCultureIgnoreCase)
		             .ToDictionary(
			             x => x.Key,
			             x => (IReadOnlyCollection<AgricultureSeasonGroupRange>)x.ToList(),
			             StringComparer.InvariantCultureIgnoreCase);
	}

	private static string? GetConfigurationText(IFuturemud? gameworld)
	{
		try
		{
			return gameworld?.GetStaticConfiguration(SeasonGroupWindowsStaticConfiguration);
		}
		catch (Exception)
		{
			return DefaultSeasonGroupWindowsConfigurationText;
		}
	}

	private static bool MatchesCurrentSeason(AgriculturePlantingWindow window, ISeason season)
	{
		return Matches(window.Value, season.SeasonGroup) ||
		       Matches(window.Value, season.Name) ||
		       Matches(window.Value, season.DisplayName);
	}

	private static bool Matches(string expected, string? actual)
	{
		return !string.IsNullOrWhiteSpace(expected) &&
		       !string.IsNullOrWhiteSpace(actual) &&
		       expected.Trim().Equals(actual.Trim(), StringComparison.InvariantCultureIgnoreCase);
	}

	private static bool TryCurrentYearFraction(IAgricultureField field, out double fraction)
	{
		fraction = 0.0;
		var celestial = field.Cell.WeatherController?.Celestial ?? field.Gameworld.CelestialObjects.FirstOrDefault();
		if (celestial == null || celestial.CelestialDaysPerYear <= 0.0)
		{
			return false;
		}

		fraction = celestial.CurrentCelestialDay.Modulus(celestial.CelestialDaysPerYear) /
		           celestial.CelestialDaysPerYear;
		return true;
	}

	private static double ParseFraction(string? text, double fallback)
	{
		return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
			? result
			: fallback;
	}
}
