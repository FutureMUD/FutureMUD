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
		['7'] = "ansi-dim",
		['8'] = "ansi-orange",
		['9'] = "ansi-bright-red",
		['A'] = "ansi-bright-green",
		['B'] = "ansi-bright-yellow",
		['C'] = "ansi-bright-blue",
		['D'] = "ansi-bright-magenta",
		['E'] = "ansi-bright-cyan",
		['F'] = "ansi-bright-white",
		['G'] = "ansi-bright-orange",
		['H'] = "ansi-bright-pink",
		['I'] = "ansi-pink",
		['J'] = "ansi-function",
		['K'] = "ansi-type",
		['L'] = "ansi-keyword-blue",
		['M'] = "ansi-variable",
		['N'] = "ansi-text",
		['O'] = "ansi-keyword-pink"
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

				if (ColourClasses.TryGetValue(char.ToUpperInvariant(code), out var cssClass))
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
		return builder.ToString()
			.Replace("\r\n", "\n", StringComparison.Ordinal)
			.Replace('\r', '\n');
	}

	[GeneratedRegex(@"<\/?(?:send|a|hint|version|support|expire|recommend|user|password|relocate|frame|dest|image|filter|sound|music|gauge|stat|var|element|attlist|entity)(?:\s[^>]*)?>", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)]
	private static partial Regex MxpTagRegex();

	[GeneratedRegex(@"\x1B\[[0-?]*[ -\/]*[@-~]", RegexOptions.CultureInvariant)]
	private static partial Regex AnsiEscapeRegex();
}
