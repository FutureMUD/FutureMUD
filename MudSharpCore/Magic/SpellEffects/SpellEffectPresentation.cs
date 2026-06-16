#nullable enable

using MudSharp.Character;
using MudSharp.Framework;
using System;
using System.Linq;
using System.Text;

namespace MudSharp.Magic.SpellEffects;

internal static class SpellEffectPresentation
{
	public static string Describe(ICharacter actor, string title, params (string Label, string Value)[] rows)
	{
		var sb = new StringBuilder();
		sb.AppendLine(title.ColourName());
		foreach (var row in rows.Where(x => !string.IsNullOrWhiteSpace(x.Label)))
		{
			var valueLines = row.Value
				.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
				.ToList();
			sb.AppendLine($"\t{row.Label}: {valueLines.FirstOrDefault()?.TrimEnd()}");
			foreach (var line in valueLines.Skip(1))
			{
				sb.AppendLine($"\t\t{line.TrimEnd()}");
			}
		}

		return sb.ToString().TrimEnd('\r', '\n');
	}
}
