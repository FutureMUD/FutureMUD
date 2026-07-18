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

	private static readonly IReadOnlyDictionary<string, char> AnsiColourCodes =
		new Dictionary<string, char>(StringComparer.Ordinal)
	{
		["\x1B[0m"] = '0',
		["\x1B[0;39m"] = '0',
		["\x1B[39m"] = '0',
		["\x1B[30m"] = '7',
		["\x1B[31m"] = '1',
		["\x1B[32m"] = '2',
		["\x1B[33m"] = '3',
		["\x1B[34m"] = '4',
		["\x1B[35m"] = '5',
		["\x1B[36m"] = '6',
		["\x1B[37m"] = 'F',
		["\x1B[1;30m"] = '7',
		["\x1B[1;31m"] = '9',
		["\x1B[1;32m"] = 'A',
		["\x1B[1;33m"] = 'B',
		["\x1B[1;34m"] = 'C',
		["\x1B[1;35m"] = 'D',
		["\x1B[1;36m"] = 'E',
		["\x1B[1;37m"] = 'F',
		["\x1B[90m"] = '7',
		["\x1B[91m"] = '9',
		["\x1B[92m"] = 'A',
		["\x1B[93m"] = 'B',
		["\x1B[94m"] = 'C',
		["\x1B[95m"] = 'D',
		["\x1B[96m"] = 'E',
		["\x1B[97m"] = 'F',
		["\x1B[38;5;94m"] = '8',
		["\x1B[38;5;202m"] = 'G',
		["\x1B[38;5;183m"] = 'I',
		["\x1B[38;5;171m"] = 'H',
		["\x1B[38;2;220;220;170m"] = 'J',
		["\x1B[38;2;184;215;163m"] = 'K',
		["\x1B[38;2;86;156;214m"] = 'L',
		["\x1B[38;2;156;220;254m"] = 'M',
		["\x1B[38;2;214;157;133m"] = 'N',
		["\x1B[38;2;238;130;238m"] = 'O'
	};

	public static string ToSafeHtml(string? source)
	{
		if (string.IsNullOrEmpty(source))
		{
			return string.Empty;
		}

		var withoutMxp = MxpTagRegex().Replace(source, string.Empty);
		withoutMxp = AnsiEscapeRegex().Replace(withoutMxp, match =>
			AnsiColourCodes.TryGetValue(match.Value, out var colourCode)
				? $"#{colourCode}"
				: string.Empty);
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
