using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace MudClientBlazor.Services;

public class AnsiMxpParser
{
	private int _sendButtonId;
	private readonly Dictionary<int, string> _sendCommands = new();

	public bool TryGetSendCommand(int id, [NotNullWhen(true)] out string? command)
	{
		return _sendCommands.TryGetValue(id, out command);
	}

	public string Parse(string input)
	{
		if (string.IsNullOrEmpty(input))
			return string.Empty;

		input = MxpModeAnsiRegex.Replace(input, string.Empty);

		var spansStack = new Stack<string>();
		var ansiState = new AnsiRenderState();
		var result = new StringBuilder();
		int lastIndex = 0;

		foreach (Match match in AnsiRegex.Matches(input))
		{
			AppendParsedText(result, input.Substring(lastIndex, match.Index - lastIndex));
			ApplyAnsiCodes(match.Groups["codes"].Value, result, spansStack, ansiState);
			lastIndex = match.Index + match.Length;
		}

		AppendParsedText(result, input.Substring(lastIndex));

		while (spansStack.Count > 0)
		{
			result.Append(spansStack.Pop());
		}

		return result.ToString();
	}

	private static void ApplyAnsiCodes(string codesStr, StringBuilder result, Stack<string> spansStack, AnsiRenderState state)
	{
		var codes = string.IsNullOrEmpty(codesStr)
			? new List<string> { "0" }
			: codesStr.Split(';').Select(code => string.IsNullOrWhiteSpace(code) ? "0" : code.Trim()).ToList();

		int i = 0;
		while (i < codes.Count)
		{
			string code = codes[i];
			if (!int.TryParse(code, out var numericCode))
			{
				i++;
				continue;
			}

			if (numericCode == 0)
			{
				while (spansStack.Count > 0)
				{
					result.Append(spansStack.Pop());
				}

				state.Bold = false;
				i++;
			}
			else if (numericCode == 1)
			{
				if (!state.Bold)
				{
					result.Append("<span style=\"font-weight:bold\">");
					spansStack.Push("</span>");
					state.Bold = true;
				}

				i++;
			}
			else if (numericCode is 5 or 6)
			{
				result.Append("<span class=\"ansi-blink\">");
				spansStack.Push("</span>");
				i++;
			}
			else if ((state.Bold || EnablesBoldBeforeNextReset(codes, i + 1)) &&
			         AnsiBoldForegroundCodes.TryGetValue(numericCode, out var boldStyle))
			{
				result.Append($"<span style=\"{boldStyle}\">");
				spansStack.Push("</span>");
				i++;
			}
			else if (AnsiCodes.TryGetValue(numericCode, out var style))
			{
				result.Append($"<span style=\"{style}\">");
				spansStack.Push("</span>");
				i++;
			}
			else if (numericCode == 38 || numericCode == 48)
			{
				if (i + 1 < codes.Count)
				{
					string colorMode = codes[i + 1];
					if (colorMode == "2" &&
					    i + 4 < codes.Count &&
					    TryParseRgbComponent(codes[i + 2], out var r) &&
					    TryParseRgbComponent(codes[i + 3], out var g) &&
					    TryParseRgbComponent(codes[i + 4], out var b))
					{
						string cssColor = $"rgb({r},{g},{b})";
						string styleAttr = numericCode == 38 ? $"color:{cssColor}" : $"background-color:{cssColor}";

						result.Append($"<span style=\"{styleAttr}\">");
						spansStack.Push("</span>");

						i += 5;
					}
					else if (colorMode == "5" &&
					         i + 2 < codes.Count &&
					         int.TryParse(codes[i + 2], out var colorIndex) &&
					         TryGetAnsi256Color(colorIndex, out var paletteColor))
					{
						string styleAttr = numericCode == 38 ? $"color:{paletteColor}" : $"background-color:{paletteColor}";

						result.Append($"<span style=\"{styleAttr}\">");
						spansStack.Push("</span>");

						i += 3;
					}
					else
					{
						i = colorMode switch
						{
							"2" => Math.Min(i + 5, codes.Count),
							"5" => Math.Min(i + 3, codes.Count),
							_ => i + 2
						};
					}
				}
				else
				{
					i++;
				}
			}
			else
			{
				i++;
			}
		}
	}

	private static bool EnablesBoldBeforeNextReset(IReadOnlyList<string> codes, int startIndex)
	{
		for (var index = startIndex; index < codes.Count; index++)
		{
			if (!int.TryParse(codes[index], out var numericCode))
			{
				continue;
			}

			if (numericCode == 0)
			{
				return false;
			}

			if (numericCode == 1)
			{
				return true;
			}
		}

		return false;
	}

	private sealed class AnsiRenderState
	{
		public bool Bold { get; set; }
	}

	private static bool TryParseRgbComponent(string code, out int value)
	{
		return int.TryParse(code, out value) && value >= 0 && value <= 255;
	}

	private static bool TryGetAnsi256Color(int colorIndex, [NotNullWhen(true)] out string? color)
	{
		color = null;

		if (colorIndex is < 0 or > 255)
		{
			return false;
		}

		if (colorIndex < Ansi256SystemColors.Length)
		{
			color = Ansi256SystemColors[colorIndex];
			return true;
		}

		if (colorIndex <= 231)
		{
			var cubeIndex = colorIndex - 16;
			var red = Ansi256ColorCubeValues[cubeIndex / 36];
			var green = Ansi256ColorCubeValues[cubeIndex / 6 % 6];
			var blue = Ansi256ColorCubeValues[cubeIndex % 6];

			color = $"rgb({red},{green},{blue})";
			return true;
		}

		var gray = 8 + (colorIndex - 232) * 10;
		color = $"rgb({gray},{gray},{gray})";
		return true;
	}

	private void AppendParsedText(StringBuilder output, string input)
	{
		if (string.IsNullOrEmpty(input))
			return;

		int lastIndex = 0;
		int openAnchorCount = 0;
		int openColorSpanCount = 0;
		int openFontSpanCount = 0;

		foreach (Match match in MxpTokenRegex.Matches(input))
		{
			AppendEscapedText(output, input.Substring(lastIndex, match.Index - lastIndex), openAnchorCount == 0);

			if (match.Groups["protocol"].Success)
			{
				// MXP protocol negotiation/declaration tags affect client capability state, not visible output.
			}
			else if (match.Groups["sendAttrs"].Success)
			{
				string attrs = match.Groups["sendAttrs"].Value;
				string? command = GetAttributeValue(attrs, "HREF");
				string text = match.Groups["sendText"].Value;

				if (string.IsNullOrWhiteSpace(command))
				{
					output.Append(EscapeHtml(match.Value));
					lastIndex = match.Index + match.Length;
					continue;
				}

				int id = _sendButtonId++;
				_sendCommands[id] = command;

				var hint = GetAttributeValue(attrs, "HINT");
				var titleAttribute = string.IsNullOrWhiteSpace(hint) ? string.Empty : $" title=\"{EscapeHtml(hint)}\"";

				output.Append($"<a class=\"mxp-send-link\" href=\"javascript:void(0)\" onclick=\"javascript:sendCommand({id}); return false;\"{titleAttribute}>");
				AppendEscapedText(output, text, allowAutoLinks: false);
				output.Append("</a>");
			}
			else if (match.Groups["hr"].Success)
			{
				output.Append("<hr />");
			}
			else
			{
				string tag = match.Groups["tag"].Value.ToUpperInvariant();
				bool isClosingTag = match.Groups["closing"].Success && match.Groups["closing"].Value == "/";
				string attrs = match.Groups["attrs"].Value;

				switch (tag)
				{
					case "B":
						output.Append(isClosingTag ? "</strong>" : "<strong>");
						break;
					case "I":
						output.Append(isClosingTag ? "</em>" : "<em>");
						break;
					case "U":
						output.Append(isClosingTag ? "</u>" : "<u>");
						break;
					case "S":
						output.Append(isClosingTag ? "</s>" : "<s>");
						break;
					case "C":
					case "COLOR":
						if (isClosingTag)
						{
							if (openColorSpanCount > 0)
							{
								openColorSpanCount--;
								output.Append("</span>");
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						else
						{
							var span = tag == "C"
								? CreateColorSpan(GetBareAttributeValue(attrs) ?? attrs.Trim(), null)
								: CreateColorSpan(
									GetAttributeValue(attrs, "FORE") ?? GetAttributeValue(attrs, "COLOR"),
									GetAttributeValue(attrs, "BACK"));

							if (span != null)
							{
								openColorSpanCount++;
								output.Append(span);
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						break;
					case "F":
					case "FONT":
						if (isClosingTag)
						{
							if (openFontSpanCount > 0)
							{
								openFontSpanCount--;
								output.Append("</span>");
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						else
						{
							var span = tag == "F"
								? CreateFontSpan(null, GetBareAttributeValue(attrs) ?? attrs.Trim(), null, null)
								: CreateFontSpan(
									GetAttributeValue(attrs, "FACE"),
									GetAttributeValue(attrs, "SIZE"),
									GetAttributeValue(attrs, "COLOR") ?? GetAttributeValue(attrs, "FORE"),
									GetAttributeValue(attrs, "BACK"));

							if (span != null)
							{
								openFontSpanCount++;
								output.Append(span);
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						break;
					case "IMAGE":
						if (!isClosingTag && TryCreateImageMarkup(attrs, out var imageMarkup))
						{
							output.Append(imageMarkup);
						}
						else
						{
							output.Append(EscapeHtml(match.Value));
						}
						break;
					case "A":
						if (isClosingTag)
						{
							if (openAnchorCount > 0)
							{
								openAnchorCount--;
								output.Append("</a>");
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						else
						{
							string? href = GetAttributeValue(attrs, "HREF");
							if (IsSafeHref(href))
							{
								openAnchorCount++;
								output.Append($"<a href=\"{EscapeHtml(href!)}\" target=\"_blank\" rel=\"noopener noreferrer\">");
							}
							else
							{
								output.Append(EscapeHtml(match.Value));
							}
						}
						break;
				}
			}

			lastIndex = match.Index + match.Length;
		}

		AppendEscapedText(output, input.Substring(lastIndex), openAnchorCount == 0);
	}

	private static void AppendEscapedText(StringBuilder output, string text, bool allowAutoLinks)
	{
		if (string.IsNullOrEmpty(text))
			return;

		text = DecodeMxpEntities(text);

		if (!allowAutoLinks)
		{
			output.Append(EscapeHtml(text));
			return;
		}

		int lastIndex = 0;
		foreach (Match match in UrlRegex.Matches(text))
		{
			output.Append(EscapeHtml(text.Substring(lastIndex, match.Index - lastIndex)));

			string url = match.Value;
			output.Append($"<a href=\"{EscapeHtml(url)}\" target=\"_blank\" rel=\"noopener noreferrer\">{EscapeHtml(url)}</a>");

			lastIndex = match.Index + match.Length;
		}

		output.Append(EscapeHtml(text.Substring(lastIndex)));
	}

	private static string EscapeHtml(string text)
	{
		return WebUtility.HtmlEncode(text);
	}

	private static string DecodeMxpEntities(string text)
	{
		return WebUtility.HtmlDecode(text);
	}

	private static string? GetAttributeValue(string attrs, string name)
	{
		string pattern = $@"\b{Regex.Escape(name)}\s*=\s*(?:""(?<value>[^""]*)""|'(?<value>[^']*)'|(?<value>[^\s>]+))";
		var match = Regex.Match(attrs, pattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		return match.Success ? DecodeMxpEntities(match.Groups["value"].Value) : null;
	}

	private static string? GetBareAttributeValue(string attrs)
	{
		var match = BareAttributeRegex.Match(attrs.Trim().TrimEnd('/').Trim());
		if (!match.Success || match.Groups["value"].Value.Contains('='))
		{
			return null;
		}

		return DecodeMxpEntities(match.Groups["value"].Value.Trim('\'', '"'));
	}

	private static bool IsSafeHref(string? href)
	{
		return Uri.TryCreate(href, UriKind.Absolute, out var uri) &&
		       (uri.Scheme == Uri.UriSchemeHttp ||
		        uri.Scheme == Uri.UriSchemeHttps ||
		        uri.Scheme == Uri.UriSchemeMailto);
	}

	private static bool IsSafeCssValue(string value)
	{
		return !string.IsNullOrWhiteSpace(value) &&
		       value.Length <= 64 &&
		       SafeCssValueRegex.IsMatch(value);
	}

	private static string? CreateColorSpan(string? foreground, string? background)
	{
		var styles = new List<string>();
		var isBlink = AppendColorStyle(styles, "color", foreground) |
		              AppendColorStyle(styles, "background-color", background);

		return CreateSpan(isBlink, styles);
	}

	private static string? CreateFontSpan(string? face, string? size, string? foreground, string? background)
	{
		var styles = new List<string>();
		var isBlink = false;

		if (!string.IsNullOrWhiteSpace(face) && IsSafeFontFamily(face))
		{
			styles.Add($"font-family:{face.Trim()}");
		}

		if (!string.IsNullOrWhiteSpace(size) && IsSafeCssValue(size))
		{
			styles.Add($"font-size:{size.Trim()}");
		}

		isBlink |= AppendColorStyle(styles, "color", foreground);
		isBlink |= AppendColorStyle(styles, "background-color", background);

		return CreateSpan(isBlink, styles);
	}

	private static bool AppendColorStyle(List<string> styles, string property, string? color)
	{
		if (string.IsNullOrWhiteSpace(color))
		{
			return false;
		}

		color = color.Trim();
		if (color.Equals("blink", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}

		if (TryGetSafeColorValue(color, out var safeColor))
		{
			styles.Add($"{property}:{safeColor}");
		}

		return false;
	}

	private static string? CreateSpan(bool isBlink, List<string> styles)
	{
		if (!isBlink && styles.Count == 0)
		{
			return null;
		}

		var classAttribute = isBlink ? " class=\"ansi-blink\"" : string.Empty;
		var styleAttribute = styles.Count == 0 ? string.Empty : $" style=\"{EscapeHtml(string.Join(";", styles))}\"";
		return $"<span{classAttribute}{styleAttribute}>";
	}

	private static bool TryGetSafeColorValue(string value, [NotNullWhen(true)] out string? color)
	{
		color = null;

		if (MxpColorValues.TryGetValue(value, out var mappedColor))
		{
			color = mappedColor;
			return true;
		}

		if (IsSafeCssValue(value))
		{
			color = value;
			return true;
		}

		return false;
	}

	private static bool TryCreateImageMarkup(string attrs, out string markup)
	{
		markup = string.Empty;

		var fileName = GetAttributeValue(attrs, "FNAME") ??
		               GetAttributeValue(attrs, "FILE") ??
		               GetAttributeValue(attrs, "NAME") ??
		               GetBareAttributeValue(attrs);

		var src = GetAttributeValue(attrs, "SRC");
		if (!IsSafeImageUri(src))
		{
			var url = GetAttributeValue(attrs, "URL");
			if (!string.IsNullOrWhiteSpace(fileName) &&
			    IsSafeImageFileName(fileName) &&
			    Uri.TryCreate(url, UriKind.Absolute, out var baseUri))
			{
				var combinedUri = new Uri(baseUri, fileName);
				src = IsSafeImageUri(combinedUri.ToString()) ? combinedUri.ToString() : null;
			}
			else if (IsSafeImageUri(url))
			{
				src = url;
			}
		}

		if (string.IsNullOrWhiteSpace(src) || !IsSafeImageUri(src))
		{
			return false;
		}

		var safeSrc = src;
		var alt = !string.IsNullOrWhiteSpace(fileName)
			? fileName
			: new Uri(safeSrc).Segments.LastOrDefault()?.Trim('/') ?? "MXP image";

		markup = $"<img class=\"mxp-image\" src=\"{EscapeHtml(safeSrc)}\" alt=\"{EscapeHtml(alt)}\" loading=\"lazy\" />";
		return true;
	}

	private static bool IsSafeImageUri(string? uriText)
	{
		return Uri.TryCreate(uriText, UriKind.Absolute, out var uri) &&
		       (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
	}

	private static bool IsSafeImageFileName(string fileName)
	{
		return !string.IsNullOrWhiteSpace(fileName) &&
		       fileName.Length <= 256 &&
		       !fileName.Contains("..", StringComparison.Ordinal) &&
		       !fileName.Contains('/') &&
		       !fileName.Contains('\\') &&
		       fileName.All(character => !char.IsControl(character));
	}

	private static bool IsSafeFontFamily(string value)
	{
		return !string.IsNullOrWhiteSpace(value) &&
		       value.Length <= 128 &&
		       SafeFontFamilyRegex.IsMatch(value);
	}

	private static readonly Regex MxpModeAnsiRegex = new(@"\x1B\[\d+z", RegexOptions.CultureInvariant);
	private static readonly Regex AnsiRegex = new(@"\x1B\[(?<codes>[0-9;]*)m", RegexOptions.CultureInvariant);
	private static readonly Regex UrlRegex = new(@"https?://[^\s<]+", RegexOptions.CultureInvariant);
	private static readonly Regex SafeCssValueRegex = new(@"^[#a-zA-Z0-9 .,%+-]+$", RegexOptions.CultureInvariant);
	private static readonly Regex SafeFontFamilyRegex = new(@"^[a-zA-Z0-9 ,._'+-]+$", RegexOptions.CultureInvariant);
	private static readonly Regex BareAttributeRegex = new(@"^(?<value>""[^""]+""|'[^']+'|[^\s/>]+)", RegexOptions.CultureInvariant);
	private static readonly Regex MxpTokenRegex = new(
		@"(?<protocol></?(?:SUPPORTS?|VERSION)\b[^>]*>|<!(?:ELEMENT|ENTITY|ATTLIST)\b[^>]*>)|<SEND\b(?<sendAttrs>[^>]*)>(?<sendText>.*?)</SEND>|(?<hr><HR\s*/?>)|<(?<closing>/)?(?<tag>B|I|U|S|C|F|A|COLOR|FONT|IMAGE)\b(?<attrs>[^>]*)>",
		RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);

	private static readonly int[] Ansi256ColorCubeValues = [0, 95, 135, 175, 215, 255];

	private static readonly string[] Ansi256SystemColors =
	[
		"rgb(0,0,0)",
		"rgb(128,0,0)",
		"rgb(0,128,0)",
		"rgb(128,128,0)",
		"rgb(0,0,128)",
		"rgb(128,0,128)",
		"rgb(0,128,128)",
		"rgb(192,192,192)",
		"rgb(128,128,128)",
		"rgb(255,0,0)",
		"rgb(0,255,0)",
		"rgb(255,255,0)",
		"rgb(0,0,255)",
		"rgb(255,0,255)",
		"rgb(0,255,255)",
		"rgb(255,255,255)"
	];

	private static readonly Dictionary<string, string> MxpColorValues = new(StringComparer.OrdinalIgnoreCase)
	{
		["black"] = "black",
		["red"] = "red",
		["green"] = "green",
		["yellow"] = "yellow",
		["blue"] = "blue",
		["magenta"] = "magenta",
		["cyan"] = "cyan",
		["white"] = "white",
		["grey"] = "gray",
		["gray"] = "gray",
		["orange"] = "orange",
		["pink"] = "pink",
		["boldblack"] = "gray",
		["boldred"] = "red",
		["boldgreen"] = "lime",
		["boldyellow"] = "yellow",
		["boldblue"] = "blue",
		["boldmagenta"] = "fuchsia",
		["boldcyan"] = "aqua",
		["boldwhite"] = "white"
	};

	private static readonly Dictionary<int, string> AnsiBoldForegroundCodes = new()
	{
		[30] = "color:rgb(128, 128, 128);font-weight:bold",
		[31] = "color:rgb(255, 0, 0);font-weight:bold",
		[32] = "color:rgb(0, 255, 0);font-weight:bold",
		[33] = "color:rgb(255, 255, 0);font-weight:bold",
		[34] = "color:rgb(0, 0, 255);font-weight:bold",
		[35] = "color:rgb(255, 0, 255);font-weight:bold",
		[36] = "color:rgb(0, 255, 255);font-weight:bold",
		[37] = "color:rgb(255, 255, 255);font-weight:bold"
	};

	private static readonly Dictionary<int, string> AnsiCodes = new()
	{
		// Text attributes
		[0] = "reset",
		[3] = "font-style:italic",
		[4] = "text-decoration:underline",

		// Foreground colors (Standard)
		[30] = "color:rgb(0, 0, 0)",          // Black
		[31] = "color:rgb(128, 0, 0)",        // Red
		[32] = "color:rgb(0, 128, 0)",        // Green
		[33] = "color:rgb(128, 128, 0)",      // Yellow
		[34] = "color:rgb(0, 0, 128)",        // Blue
		[35] = "color:rgb(128, 0, 128)",      // Magenta
		[36] = "color:rgb(0, 128, 128)",      // Cyan
		[37] = "color:rgb(192, 192, 192)",    // White (Light Gray)

		// Foreground colors (Bright)
		[90] = "color:rgb(128, 128, 128);font-weight:bold",    // Bright Black (Gray)
		[91] = "color:rgb(255, 0, 0);font-weight:bold",        // Bright Red
		[92] = "color:rgb(0, 255, 0);font-weight:bold",        // Bright Green
		[93] = "color:rgb(255, 255, 0);font-weight:bold",      // Bright Yellow
		[94] = "color:rgb(0, 0, 255);font-weight:bold",        // Bright Blue
		[95] = "color:rgb(255, 0, 255);font-weight:bold",      // Bright Magenta
		[96] = "color:rgb(0, 255, 255);font-weight:bold",      // Bright Cyan
		[97] = "color:rgb(255, 255, 255);font-weight:bold",    // Bright White

		// Background colors (Standard)
		[40] = "background-color:rgb(0, 0, 0)",          // Black
		[41] = "background-color:rgb(128, 0, 0)",        // Red
		[42] = "background-color:rgb(0, 128, 0)",        // Green
		[43] = "background-color:rgb(128, 128, 0)",      // Yellow
		[44] = "background-color:rgb(0, 0, 128)",        // Blue
		[45] = "background-color:rgb(128, 0, 128)",      // Magenta
		[46] = "background-color:rgb(0, 128, 128)",      // Cyan
		[47] = "background-color:rgb(192, 192, 192)",    // White (Light Gray)

		// Background colors (Bright)
		[100] = "background-color:rgb(128, 128, 128)",   // Bright Black (Gray)
		[101] = "background-color:rgb(255, 0, 0)",       // Bright Red
		[102] = "background-color:rgb(0, 255, 0)",       // Bright Green
		[103] = "background-color:rgb(255, 255, 0)",     // Bright Yellow
		[104] = "background-color:rgb(0, 0, 255)",       // Bright Blue
		[105] = "background-color:rgb(255, 0, 255)",     // Bright Magenta
		[106] = "background-color:rgb(0, 255, 255)",     // Bright Cyan
		[107] = "background-color:rgb(255, 255, 255)"    // Bright White
	};
}
