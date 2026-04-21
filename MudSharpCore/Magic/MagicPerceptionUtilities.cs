#nullable enable
using MudSharp.Effects;
using MudSharp.Effects.Interfaces;
using MudSharp.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MudSharp.Magic;

public static class MagicPerceptionUtilities
{
	public static bool CanSenseMagic(IPerceiver? voyeur)
	{
		if (voyeur is null)
		{
			return false;
		}

		return (voyeur.GetPerception(voyeur.NaturalPerceptionTypes) & PerceptionTypes.SenseMagical) !=
		       PerceptionTypes.None;
	}

	public static string DescribeMagicAuras(IPerceiver? voyeur, IEnumerable<IEffect> effects, bool colour = true,
		string heading = "You sense magical auras:")
	{
		if (!CanSenseMagic(voyeur))
		{
			return string.Empty;
		}

		var auraDescriptions = effects
			.Where(x => x.Applies())
			.Where(x => x is IMagicEffect || x is IMagicSpellEffect)
			.Select(x => x.Describe(voyeur!))
			.Where(x => !string.IsNullOrWhiteSpace(x))
			.Distinct()
			.ToList();
		if (!auraDescriptions.Any())
		{
			return string.Empty;
		}

		var sb = new StringBuilder();
		sb.AppendLine(colour ? heading.Colour(Telnet.BoldMagenta) : heading);
		foreach (var aura in auraDescriptions)
		{
			sb.AppendLine($"\t{aura}");
		}

		return sb.ToString().TrimEnd();
	}
}
