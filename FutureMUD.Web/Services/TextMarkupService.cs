#nullable enable

using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace FutureMUD.Web.Services;

public static partial class TextMarkupService
{
	private static readonly IReadOnlyDictionary<char, string> ColourClasses = new Dictionary<char, string>
	{
		['1'] = "ansi-red",
		['2'] = "ansi-green",
		['3'] = "ansi-yellow",
		['4'] = "ansi-blue",
		['5'] = "ansi-magenta",
		['6'] = "ansi-cyan",
		['7'] = "ansi-white",
		['A'] = "ansi-bright-green",
		['B'] = "ansi-bright-cyan",
		['C'] = "ansi-bright-red",
		['D'] = "ansi-bright-yellow",
		['E'] = "ansi-bright-white"
	};

	public static string ToSafeHtml(string? source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return string.Empty;
		}

		var withoutMxp = MxpTagRegex().Replace(source, string.Empty);
		withoutMxp = AnsiEscapeRegex().Replace(withoutMxp, string.Empty);
		var builder = new StringBuilder();
		var spanOpen = false;
		for (var index = 0; index < withoutMxp.Length; index++)
		{
			var character = withoutMxp[index];
			if (character == '#' && index + 1 < withoutMxp.Length)
			{
				var code = withoutMxp[++index];
				if (code == '#')
				{
					builder.Append('#');
					continue;
				}

				if (code == '0')
				{
					if (spanOpen)
					{
						builder.Append("</span>");
						spanOpen = false;
					}
					continue;
				}

				if (ColourClasses.TryGetValue(code, out var cssClass))
				{
					if (spanOpen)
					{
						builder.Append("</span>");
					}
					builder.Append("<span class=\"").Append(cssClass).Append("\">");
					spanOpen = true;
					continue;
				}

				builder.Append(WebUtility.HtmlEncode($"#{code}"));
				continue;
			}

			builder.Append(WebUtility.HtmlEncode(character.ToString()));
		}

		if (spanOpen)
		{
			builder.Append("</span>");
		}
		return builder.ToString().Replace("\r\n", "\n", StringComparison.Ordinal).Replace("\n", "<br>\n", StringComparison.Ordinal);
	}

	[GeneratedRegex(@"<\/?(?:send|a|hint|version|support|expire|recommend|user|password|relocate|frame|dest|image|filter|sound|music|gauge|stat|var|element|attlist|entity)(?:\s[^>]*)?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
	private static partial Regex MxpTagRegex();

	[GeneratedRegex(@"\x1B\[[0-?]*[ -\/]*[@-~]", RegexOptions.CultureInvariant)]
	private static partial Regex AnsiEscapeRegex();
}
