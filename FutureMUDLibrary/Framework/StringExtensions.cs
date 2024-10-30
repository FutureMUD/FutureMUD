using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MudSharp.Accounts;

namespace MudSharp.Framework {
	public static partial class StringExtensions {
		private static readonly Regex SpecialCharacterRegex = new(@"(\\""|\\n|\\t|\\\\)",
			RegexOptions.IgnoreCase);

		private static readonly Regex ProperCaseRegex = new("(?<=\x1B\\[[^m]+m|^)([a-z])");

		private static readonly Regex TitleCaseRegex = new("(?<=\x1B\\[[^m]+m|[ ]|^)([a-z])");

		private static readonly Regex NormaliseSpacingRegex1St = new(@"[ \t]{2,}", RegexOptions.Compiled);
		private static readonly Regex NormaliseSpacingRegex2Nd = new(@"[ \t]+([.,;:!?])", RegexOptions.Compiled);

		private static readonly Regex NormaliseSpacingRegex3Rd = new(@"(?<![a-zA-Z])([.,;:!?])([a-zA-Z])(?![.,;:!?])",
			RegexOptions.Compiled);

		/// <summary>
		///     Surrounds the input with "[" and "]"
		/// </summary>
		public static string SquareBrackets(this string input) {
			return "[" + input + "]";
		}

		/// <summary>
		///     Pads the input with specified "*" and " ", and then surrounds with "[" and "]"
		/// </summary>
		public static string StarRectangle(this string input, int stars, int length) {
			return input.PadRight(stars, '*').PadRight(length - stars, ' ').SquareBrackets();
		}

		/// <summary>
		///     Surrounds the input with "[" and "] "
		/// </summary>
		public static string SquareBracketsSpace(this string input) {
			return "[" + input + "] ";
		}

		/// <summary>
		///     Surrounds the input with "{" and "}"
		/// </summary>
		public static string CurlyBrackets(this string input) {
			return "{" + input + "}";
		}

		/// <summary>
		///     Surrounds the input with "{" and "} "
		/// </summary>
		public static string CurlyBracketsSpace(this string input) {
			return "{" + input + "} ";
		}

		/// <summary>
		///     Fluent method to surround the input with "(" and ")" if truth is true
		/// </summary>
		public static string Parentheses(this string input, bool truth = true) {
			return truth ? "(" + input + ")" : input;
		}

		/// <summary>
		/// Fluent method to surround the input with ( and ) if truth is true and not already surrounded by parentheses
		/// </summary>
		/// <param name="input">The input string</param>
		/// <param name="truth">The fluent truth variable</param>
		/// <returns>bloody -> (bloody)
		/// (bloody) -> (bloody)</returns>
		public static string ParenthesesIfNot(this string input, bool truth = true)
		{
			if (!truth)
			{
				return input;
			}

			if (input.Length < 2)
			{
				return input;
			}

			if (input[0] == '(' && input[^1] == ')')
			{
				return input;
			}

			return $"({input})";
		}

		/// <summary>
		///     Surrounds the input with "(" and ") "
		/// </summary>
		public static string ParenthesesSpace(this string input) {
			return "(" + input + ") ";
		}

		public static string ParenthesesSpacePrior(this string input)
		{
			return $" ({input})";
		}

		public static string FluentAppend(this string input, string appendText, bool truth)
		{
			return truth ? $"{input}{appendText}" : input;
		}

		/// <summary>
		///     Fluent method to surround the input with double quotes "input"
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string DoubleQuotes(this string input, bool truth = true) {
			if (!truth) {
				return input;
			}
			return "\"" + input + "\"";
		}

		public static string RemoveFirstCharacter(this string input) {
			return input.Length > 1 ? input.Substring(1).TrimStart() : "";
		}

		public static string RemoveLastCharacter(this string input)
		{
			return input.Length > 1 ? input.Substring(0, input.Length - 1) : "";
		}

		public static string RemoveFirstWord(this string input) {
			var firstSpace = input.IndexOf(' ');
			return firstSpace != -1 ? input.Remove(0, firstSpace + 1).TrimStart() : "";
		}

		public static string ParseSpecialCharacters(this string input) {
			if (string.IsNullOrEmpty(input)) {
				return string.Empty;
			}

			return SpecialCharacterRegex.Replace(input, m => {
				switch (m.Groups[1].Value.ToLowerInvariant()) {
					case "\\\"":
						return "\"";
					case "\\n":
						return "\n";
					case "\\t":
						return "\t";
					case "\\\\":
						return "\\";
					default:
						throw new NotSupportedException(
							"Invalid Special Character in StringExtensions.ParseSpecialCharacters.");
				}
			});
		}

		private static Regex FullstopRegex = new Regex($@".*[\n\.\,\?\!\""\'\)]({MXP.BeginMXP}[^{MXP.EndMXP}]+{MXP.EndMXP})*(\x1b\[[^m]+m)*$", RegexOptions.ExplicitCapture);

		/// <summary>
		///     Adds a fullstop to the end of the input if there is not already a sentence ending character at the end
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string Fullstop(this string input) {
			if (FullstopRegex.IsMatch(input))
			{
				return input;
			}

			return input + ".";
		}

		/// <summary>
		///     Capitalises the first character of the string
		/// </summary>
		public static string Proper(this string input) {
			return string.IsNullOrEmpty(input) ? "" : ProperCaseRegex.Replace(input, m => char.ToUpper(m.Groups[1].Value[0]).ToString(), 1);
		}

		public static string ReplaceFirst(this string text, string search, string replace) {
			if (string.IsNullOrEmpty(text)) {
				return string.Empty;
			}

			var pos = text.IndexOf(search, StringComparison.Ordinal);
			if (pos < 0) {
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
		}

		public static string ReplaceFirst(this string text, char search, char replace) {
			if (string.IsNullOrEmpty(text)) {
				return string.Empty;
			}

			var pos = text.IndexOf(search);
			if (pos < 0) {
				return text;
			}
			return text.Substring(0, pos) + replace + text.Substring(pos + 1);
		}

		public static string TitleCase(this string input) {
			return TitleCaseRegex.Replace(input, m => char.ToUpper(m.Groups[1].Value[0]).ToString());
		}

		/// <summary>
		///     Fluent extension method to handle Proper Case transformations of strings based on a boolean variable
		/// </summary>
		/// <param name="input"></param>
		/// <param name="proper"></param>
		/// <returns></returns>
		public static string FluentProper(this string input, bool proper) {
			return proper ? input.Proper() : input;
		}

		public static bool IsInteger(this string input) {
			return long.TryParse(input, out _);
		}

		/// <summary>
		///     Search for the specified target word in a list of words, optionally abbreviated.
		/// </summary>
		/// <param name="input">The collection on which this comparison is being done</param>
		/// <param name="targetWord">The word that is being searched for</param>
		/// <param name="abbreviated">Whether or not abbreviations are acceptable</param>
		/// <returns>True if the target word is in the collection, with the specified options</returns>
		public static bool HasWord(this List<string> input, string targetWord, bool abbreviated = true) {
			return abbreviated
				? input.Any(x => x.StartsWith(targetWord, StringComparison.InvariantCultureIgnoreCase))
				: input.Any(x => x.Equals(targetWord, StringComparison.InvariantCultureIgnoreCase));
		}

		/// <summary>
		///     Search for all of the specified target words in a list of words, optionally abbreviated.
		/// </summary>
		/// <param name="input">The collection on which this comparison is being done</param>
		/// <param name="targetWords">The words that are being searched for</param>
		/// <param name="abbreviated">Whether or not abbreviations are acceptable</param>
		/// <returns>True if the target words are all in the collection, with the specified options</returns>
		public static bool HasWord(this List<string> input, IEnumerable<string> targetWords, bool abbreviated = true) {
			return abbreviated
				? targetWords.All(x => input.Any(y => y.StartsWith(x, StringComparison.InvariantCultureIgnoreCase)))
				: targetWords.All(x => input.Any(y => y.Equals(x, StringComparison.InvariantCultureIgnoreCase)));
		}

		public static string IfEmpty(this string text, string emptytext) {
			return text.Length > 0 ? text : emptytext;
		}

		public static string SpaceIfNotEmpty(this string text) {
			return text.ConcatIfNotEmpty(" ");
		}

		public static string LeadingSpaceIfNotEmpty(this string text) {
			return text.LeadingConcatIfNotEmpty(" ");
		}

		public static string ConcatIfNotEmpty(this string text, string concat) {
			return string.IsNullOrEmpty(text) ? text : text + concat;
		}

		public static string LeadingConcatIfNotEmpty(this string text, string concat) {
			return string.IsNullOrEmpty(text) ? text : concat + text;
		}

		public static bool EqualTo(this string text, string compareText, bool caseInsensitive = true) {
			return string.Equals(text, compareText,
				caseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
		}

		public static bool EqualToAny(this string text, params string[] compareTexts) {
			return compareTexts.Any(x => string.Equals(text, x, StringComparison.InvariantCultureIgnoreCase));
		}

		public static string Strip(this string source, Func<char, bool> charsToStrip) {
			return new string(source.Where(x => !charsToStrip(x)).ToArray());
		}

		public static string Append(this string source, string suffix) {
			return source + suffix;
		}

		public static string Prepend(this string source, string prefix) {
			return prefix + source;
		}

		public static string NormaliseSpacing(this string text, bool ignorePunctuation = false) {
			return ignorePunctuation
				? NormaliseSpacingRegex1St.Replace(text, m => " ")
				: NormaliseSpacingRegex3Rd.Replace(
					NormaliseSpacingRegex2Nd.Replace(NormaliseSpacingRegex1St.Replace(text, m => " "),
						m => m.Groups[1].Value), m => m.Groups[1].Value + " " + m.Groups[2].Value);
		}

		public static string IfNullOrWhiteSpace(this string text, string fallback) {
			return string.IsNullOrWhiteSpace(text) ? fallback : text;
		}

		/// <summary>
		/// Strips out all MXP and ANSI Instructions from a string
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string RawText(this string text) {
			var sb = new StringBuilder();
			bool colour = false, mxp = false;
			foreach (var t in text)
			{
				colour = colour || (t == '\x1B');
				mxp = mxp || (t == MXP.BeginMXPChar);
				if (!mxp && !colour) {
					sb.Append(t);
				}
				colour = (!colour || (t != 'm')) && colour;
				mxp = (!mxp || (t != MXP.EndMXPChar)) && mxp;
			}

			return sb.ToString();
		}

		public static string RawTextForDiscord(this string text)
		{
			var sb = new StringBuilder();
			bool colour = false, mxp = false;
			foreach (var t in text)
			{
				colour = colour || (t == '\x1B');
				mxp = mxp || (t == MXP.BeginMXPChar);
				if (!mxp && !colour)
				{
					sb.Append(t);
				}

				if (colour && t == 'm')
				{
					sb.Append("**");
				}
				colour = (!colour || (t != 'm')) && colour;
				mxp = (!mxp || (t != MXP.EndMXPChar)) && mxp;
			}

			return sb.ToString();
		}

		public static string RawTextSubstring(this string input, int startIndex)
		{
			return input.RawTextSubstring(startIndex, input.Length - startIndex);
		}

		public static string RawTextSubstring(this string input, int startIndex, int characters)
		{
			var count = 0;
			bool colour = false, mxp = false;
			for (var i = 0; i < input.Length; i++)
			{
				colour = colour || (input[i] == '\x1B');
				mxp = mxp || (input[i] == MXP.BeginMXPChar);
				count += (i > startIndex) && (colour || mxp) ? 0 : 1;
				if (count == characters)
				{
					return input.Substring(startIndex, i - startIndex);
				}
				colour = (!colour || (input[i] != 'm')) && colour;
				mxp = (!mxp || (input[i] != MXP.EndMXPChar)) && mxp;
			}

			return input.Substring(startIndex);
		}

		public static int RawTextLength(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return 0;
			}
			var count = 0;
			bool colour = false, mxp = false;
			foreach (var ch in input)
			{
				colour = colour || (ch == '\x1B');
				mxp = mxp || (ch == MXP.BeginMXPChar);
				count += colour || mxp ? 0 : 1;
				colour = (!colour || (ch != 'm')) && colour;
				mxp = (!mxp || (ch != MXP.EndMXPChar)) && mxp;
			}

			return count;
		}

		public static string RawTextPadLeft(this string input, int pad, char padChar = ' ')
		{
			var len = input.RawTextLength();
			return (len < pad ? new string(padChar, pad - len) : "") + input;
		}

		public static string RawTextPadRight(this string input, int pad, char padChar = ' ')
		{
			var len = input.RawTextLength();
			return input + (len < pad ? new string(padChar, pad - len) : "");
		}

		public static Regex StringSanitisingRegex = new(@"[{}]", RegexOptions.Compiled);

		/// <summary>
		/// Takes input that might come from a player, and sanitises it to ensure that it cannot contain bad characters for string format functions
		/// </summary>
		/// <param name="input">The player input</param>
		/// <returns>A sanitised input that does not contain any {n} references</returns>
		public static string Sanitise(this string input) {
			return StringSanitisingRegex.Replace(input, m => {
				switch (m.Value) {
					case "{":
						return "{{";
					case "}":
						return "}}";
				}

				return m.Value;
			});
		}

		public static Regex StringSanitisingFilteredRegex = new(@"\{(?<index>\d+)[^\}]*\}");
		public static string SanitiseExceptNumbered(this string input, int maximumIndex)
		{
			return StringSanitisingFilteredRegex.Replace(input, m => {
				if (int.Parse(m.Groups["index"].Value) <= maximumIndex)
				{
					return m.Value;
				}

				return $"{{{m.Value}}}";
			});
		}

		public static string SplitCamelCase(this string str)
		{
			return Regex.Replace(
				Regex.Replace(
					str,
					@"(\P{Ll})(\P{Ll}\p{Ll})",
					"$1 $2"
				),
				@"(\p{Ll})(\P{Ll})",
				"$1 $2"
			);
		}

		public static string SplitCamelCase(this string str, bool truth)
		{
			if (!truth)
			{
				return str;
			}

			return Regex.Replace(
				Regex.Replace(
					str,
					@"(\P{Ll})(\P{Ll}\p{Ll})",
					"$1 $2"
				),
				@"(\p{Ll})(\P{Ll})",
				"$1 $2"
			);
		}


		private static Regex IncrementRegex = new("(?<pre>.+)(?<number>[0-9]+)$");

		/// <summary>
		/// This function takes a string, and checks to see if it ends in a number. If it ends in a number, it will increment that number by 1. Otherwise, it will add the number 1 to the end of the string.
		/// The use case for this function would be, for example, duplicate names of things where you might have "Item" become "Item1" or "Item55" become "Item56" etc.
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns></returns>
		public static string IncrementNumberOrAddNumber(this string str)
		{
			var match = IncrementRegex.Match(str);
			if (!match.Success)
			{
				return $"{str}1";
			}

			return $"{match.Groups["pre"].Value}{int.Parse(match.Groups["number"].Value) + 1:N0}";
		}

		/// <summary>
		/// This item takes a collection of strings and a proposed unique name. If the name is not found within the collection, it will return the proposed name.
		/// If the name is found, including in forms like {name}1, it will return the name with the next number in sequence appended.
		/// E.g. name = item, collection = banjo, totem, handle - return value will be "item"
		/// E.g. name = item, collection = item, totem, handle - return value will be "item1"
		/// E.g. name = item, collection = item, item1, item3, handle, totem - return value will be "item4"
		/// </summary>
		/// <typeparam name="T">IEnumerable of strings</typeparam>
		/// <param name="names">A collection of strings in which the name must be unique</param>
		/// <param name="name">The proposed name</param>
		/// <returns>The unique string</returns>
		public static string NameOrAppendNumberToName<T>(this T names, string name) where T : IEnumerable<string> {
			var regex = new Regex($"{name}(?<number>[0-9]+)?$", RegexOptions.IgnoreCase);
			var matches = names
				.Select(x => regex.Match(x))
				.Where(x => x.Success)
				.ToList();
			if (!matches.Any()) {
				return name;
			}

			return $"{name}{matches.Select(x => int.Parse(x.Groups["number"].Length == 0 ? "0" : x.Groups["number"].Value)).Max() + 1}";
		}

		public static string ToColouredString(this bool boolean)
		{
			return boolean ? "True".Colour(Telnet.Green) : "False".Colour(Telnet.Red);
		}

		/// <summary>
		/// Returns a number in a specified "bonus-style" format, that is to say +x.xx and -x.xx with optional colour and decimal place lengths
		/// </summary>
		/// <param name="number">The number to format</param>
		/// <param name="formatProvider">The format provider to provide the culture info for the number format</param>
		/// <param name="decimalPlaces">The number of decimal places</param>
		/// <param name="colour">Whether to colour green/red for bonus/penalty</param>
		/// <returns></returns>
		public static string ToBonusString(this double number, IFormatProvider formatProvider = null, uint decimalPlaces = 2, bool colour = true)
		{
			formatProvider ??= NumberFormatInfo.InvariantInfo;

			var decimalBit = (decimalPlaces > 0 ? $".{new string('0', (int)decimalPlaces)}" : "");
			var format = $"+#,0{decimalBit};-#,0{decimalBit};+0{decimalBit}";
			var value = number.ToString(format, formatProvider);
			if (colour)
			{
				return number >= 0.00 ?
					value.Colour(Telnet.Green) :
					value.Colour(Telnet.Red);
			}

			return value;
		}

		public static string ToBonusPercentageString(this double number, IFormatProvider formatProvider = null, uint decimalPlaces = 2, bool colour = true)
		{
			formatProvider ??= NumberFormatInfo.InvariantInfo;

			var decimalBit = (decimalPlaces > 0 ? $".{new string('0', (int)decimalPlaces)}" : "");
			var format = $"+#,0{decimalBit}%;-#,0{decimalBit}%;+0{decimalBit}%";
			var value = number.ToString(format, formatProvider);
			if (colour)
			{
				return number >= 0.00 ?
					value.Colour(Telnet.Green) :
					value.Colour(Telnet.Red);
			}

			return value;
		}

		/// <summary>
		/// Returns a number in a specified "bonus-style" format, that is to say +x.xx and -x.xx with optional colour and decimal place lengths
		/// </summary>
		/// <param name="number">The number to format</param>
		/// <param name="formatProvider">The format provider to provide the culture info for the number format</param>
		/// <param name="decimalPlaces">The number of decimal places</param>
		/// <param name="colour">Whether to colour green/red for bonus/penalty</param>
		/// <returns></returns>
		public static string ToBonusString(this int number, IFormatProvider formatProvider = null, bool colour = true)
		{
			return ((double)number).ToBonusString(formatProvider, 0U, colour);
		}

		public static string Wrap(this string text, int width, string indent = "")
		{
			if (width < 1 || string.IsNullOrEmpty(text))
			{
				return text;
			}

			var endsWithNewline = text.Last() == '\n';

			var sb = new StringBuilder();
			var strings = text.Replace("\t", "  ").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			if (strings.Any() && string.IsNullOrWhiteSpace(strings.Last()))
			{
				strings = strings.Take(strings.Length - 1).ToArray();
			}

			foreach (var line in strings)
			{
				if (sb.Length > 0)
				{
					sb.Append("\n");
				}

				var remains = line;

				if ((remains.Length > 0) && (remains[0] == Telnet.NoWordWrapChar))
				{
					sb.Append(remains.Substring(1));
					continue;
				}

				if (remains.RawTextLength() <= width - indent.Length)
				{
					sb.Append(indent + line);
					continue;
				}

				while (remains.RawTextLength() > width)
				{
					var splitIndex = LastWhiteSpaceIndex(remains, width - indent.Length);
					sb.AppendLine(indent + remains.Substring(0, splitIndex + 1).TrimEnd());
					remains = remains.Substring(splitIndex).TrimStart();
				}

				if (remains.Length > 0)
				{
					sb.Append(indent + remains.TrimStart());
				}
			}

			if (endsWithNewline)
			{
				sb.Append("\n");
			}

			return sb.ToString();
		}

		private static int LastWhiteSpaceIndex(string text, int width)
		{
			int charsFound = 0, lastWhitespace = 0;
			var noAnsiLength = 0;
			bool inAnsi = false, inMXP = false;
			for (var i = 0; i < text.Length - 1; i++)
			{
				inAnsi |= text[i] == '\x1B';
				inMXP |= text[i] == MXP.BeginMXPChar;
				if (!inAnsi && !inMXP && (text[i] != '\n'))
				{
					charsFound++;
				}
				inAnsi = (!inAnsi || (text[i] != 'm')) && inAnsi;
				inMXP = (!inMXP || (text[i] != MXP.EndMXPChar)) && inMXP;

				if (char.IsWhiteSpace(text[i]) && !inAnsi && !inMXP)
				{
					lastWhitespace = i;
				}

				if (charsFound == width)
				{
					noAnsiLength = i;
					break;
				}
			}

			// If no whitespace found, break at maximum length
			if (lastWhitespace == 0)
			{
				return noAnsiLength;
			}

			// Find start of whitespace
			inMXP = false;
			while ((lastWhitespace > 0) && char.IsWhiteSpace(text[lastWhitespace - 1]) && !inMXP)
			{
				var current = text[lastWhitespace - 1];
				if (current == MXP.EndMXPChar)
				{
					inMXP = true;
				}
				else if (current == MXP.BeginMXPChar)
				{
					inMXP = false;
				}
				lastWhitespace--;
			}

			// Return index of text before whitespace
			return lastWhitespace;
		}

		/// <summary>
		///     Marks this text as non-wrapping when sent to the player. Word wrap will not be applied. Must be applied on a
		///     per-line basis.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string NoWrap(this string text)
		{
			return Telnet.NoWordWrap + text;
		}

		public static string NormaliseOutputSentences(this string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return "";
			}

			var tokens = TokenizeInput(input);
			var output = new StringBuilder();
			bool sentenceEnded = true;

			foreach (var token in tokens)
			{
				if (IsAnsiCode(token) || IsMxpCode(token))
				{
					// Preserve ANSI and MXP codes as is
					output.Append(token);
				}
				else
				{
					// Process normal text
					var processedText = ProcessText(token, ref sentenceEnded);
					output.Append(processedText);
				}
			}

			return output.ToString();
		}

		private static List<string> TokenizeInput(string input)
		{
			var tokens = new List<string>();
			int i = 0;
			while (i < input.Length)
			{
				if (input[i] == '\x1B')
				{
					// ANSI code
					int start = i++;
					if (i < input.Length && input[i] == '[')
					{
						i++;
						while (i < input.Length && input[i] != 'm')
						{
							i++;
						}
						if (i < input.Length)
						{
							i++; // Include 'm'
						}
					}
					tokens.Add(input.Substring(start, i - start));
				}
				else if (input[i] == MXP.BeginMXPChar)
				{
					// MXP code
					int start = i++;
					while (i < input.Length && input[i] != MXP.EndMXPChar)
					{
						i++;
					}
					if (i < input.Length)
					{
						i++; // Include EndMXPChar
					}
					tokens.Add(input.Substring(start, i - start));
				}
				else
				{
					// Normal text
					int start = i;
					while (i < input.Length && input[i] != '\x1B' && input[i] != MXP.BeginMXPChar)
					{
						i++;
					}
					tokens.Add(input.Substring(start, i - start));
				}
			}
			return tokens;
		}

		private static bool IsAnsiCode(string token)
		{
			return token.StartsWith("\x1B[") && token.EndsWith("m");
		}

		private static bool IsMxpCode(string token)
		{
			return token.Length > 2 && token[0] == MXP.BeginMXPChar && token[^1] == MXP.EndMXPChar;
		}

		private static string ProcessText(string text, ref bool sentenceEnded)
		{
			var output = new StringBuilder();
			int i = 0;

			while (i < text.Length)
			{
				var punctuationSequence = new StringBuilder();

				// Accumulate punctuation characters
				while (i < text.Length && Constants.PunctuationCharacters.Contains(text[i]))
				{
					punctuationSequence.Append(text[i]);
					i++;
				}

				if (punctuationSequence.Length > 0)
				{
					string punctSeq = punctuationSequence.ToString();

					if (punctSeq == "...")
					{
						// Preserve ellipsis
						output.Append("...");
					}
					else
					{
						// Reduce any other repeated or mixed punctuation to a single character (first in sequence)
						char firstPunct = punctSeq[0];
						output.Append(firstPunct);

						// Check if it ends a sentence
						if (Constants.SentenceEndingCharacters.Contains(firstPunct))
						{
							sentenceEnded = true;
						}
					}
				}

				// Accumulate letters and whitespace
				while (i < text.Length && !Constants.PunctuationCharacters.Contains(text[i]) && text[i] != '\x1B' && text[i] != MXP.BeginMXPChar)
				{
					char ch = text[i];

					if (sentenceEnded && char.IsLetter(ch))
					{
						output.Append(char.ToUpper(ch));
						sentenceEnded = false;
					}
					else
					{
						output.Append(ch);
					}

					i++;

					// Update sentenceEnded flag
					if (char.IsWhiteSpace(ch))
					{
						// Do nothing
					}
					else if (Constants.SentenceEndingCharacters.Contains(ch))
					{
						sentenceEnded = true;
					}
					else
					{
						sentenceEnded = false;
					}
				}
			}

			return output.ToString();
		}

		public static string ProperSentences(this string input)
		{
			if (input.Length == 0)
			{
				return "";
			}

			var chararray = new List<char>(input.Length);
			bool colour = false, mxp = false;
			char? lastChar = null, secondLastChar = null, thirdLastChar = null;
			foreach (var ch in input)
			{
				colour = colour || (ch == '\x1B');
				mxp = mxp || (ch == MXP.BeginMXPChar);

				if (!colour && !mxp)
				{
					if (!lastChar.HasValue)
					{
						if (char.IsLetter(ch))
						{
							chararray.Add(char.ToUpper(ch));
							lastChar = ch;
							continue;
						}
					}

					if (secondLastChar.HasValue)
					{
						if (((Constants.SentenceEndingCharacters.Contains(secondLastChar.Value) && (lastChar == ' '))
							 || (lastChar == '\n') ||
							 lastChar == ' ' && secondLastChar == '"' && thirdLastChar.HasValue && Constants.SentenceEndingCharacters.Contains(thirdLastChar.Value)
							)
							&& char.IsLetter(ch))
						{
							chararray.Add(char.ToUpper(ch));
							secondLastChar = lastChar;
							lastChar = ch;
							continue;
						}
					}

					thirdLastChar = secondLastChar;
					secondLastChar = lastChar;
					lastChar = ch;
				}

				chararray.Add(ch);


				colour = (!colour || (ch != 'm')) && colour;
				mxp = (!mxp || (ch != MXP.EndMXPChar)) && mxp;
			}

			return new string(chararray.ToArray());
		}

		/// <summary>
		/// Returns text like the following:
		/// ═══════╣ Title ╠════════════════════════════════════════
		/// </summary>
		/// <param name="text">The title</param>
		/// <param name="person">The person who is viewing the output</param>
		/// <param name="rulerColour">The colour of the ruler</param>
		/// <param name="titleColour">The colour of the text title</param>
		/// <returns>The line</returns>
		public static string GetLineWithTitle(this string text, IHaveAccount person, ANSIColour rulerColour, ANSIColour titleColour)
		{
			return text.GetLineWithTitle(person.Account.LineFormatLength, person.Account.UseUnicode, rulerColour,
				titleColour);
		}

		public static string GetLineWithTitleInner(this string text, IHaveAccount person, ANSIColour rulerColour, ANSIColour titleColour)
		{
			return text.GetLineWithTitle(person.Account.InnerLineFormatLength, person.Account.UseUnicode, rulerColour,
				titleColour);
		}

		/// <summary>
		/// Returns text like the following:
		/// ═══════╣ Title ╠════════════════════════════════════════
		/// </summary>
		/// <param name="text">The title</param>
		/// <param name="linelength">The total line length</param>
		/// <param name="useUnicode">Whether to use unicode characters or not</param>
		/// <param name="rulerColour">The colour of the ruler</param>
		/// <param name="titleColour">The colour of the text title</param>
		/// <returns>The line</returns>
		public static string GetLineWithTitle(this string text, int linelength, bool useUnicode, ANSIColour rulerColour, ANSIColour titleColour)
		{
			var sb = new StringBuilder();
			if (rulerColour != null) {
				sb.Append(rulerColour.Colour);
			}
			sb.Append(useUnicode ? '═' : '=', 7);
			sb.Append(useUnicode ? "╣" : "[");
			if (rulerColour != null)
			{
				sb.Append(rulerColour.Reset());
			}

			if (titleColour != null) {
				sb.Append(titleColour.Colour);
			}

			sb.Append(' ');
			sb.Append(text);
			sb.Append(' ');

			if (titleColour != null) {
				sb.Append(titleColour.Reset());
			}

			if (rulerColour != null)
			{
				sb.Append(rulerColour.Colour);
			}
			sb.Append(useUnicode ? "╠" : "]");
			sb.Append(useUnicode ? '═' : '=', Math.Max(linelength - 11 - text.RawTextLength(), 1));
			
			if (rulerColour != null)
			{
				sb.Append(rulerColour.Reset());
			}

			return sb.ToString();
		}

		public static Regex CollapseStringRegex => new(@"[ _-]", RegexOptions.IgnoreCase);

		/// <summary>
		/// This function takes some text and "collapses" it for parsing purposes, removing spaces, dashes and underscores
		/// </summary>
		/// <param name="text">The text to collapse, e.g. "long_description"</param>
		/// <returns>A collapsed string, e.g. "longdescription"</returns>
		public static string CollapseString(this string text)
		{
			return CollapseStringRegex.Replace(text, m => string.Empty);
		}

		public static IEnumerable<string> SplitStringsForDiscord(this string message)
		{
			var list = new List<string>();
			while (message.Length > 1950)
			{
				var lastNewline = message.Substring(0, 1950).LastIndexOf('\n');
				if (lastNewline == -1)
				{
					list.Add(message.Substring(0, 1950));
					message = message.Substring(1950);
					continue;
				}

				list.Add(message.Substring(0, lastNewline));
				message = message.Substring(lastNewline + 1);
			}

			list.Add(message);
			return list;
		}

		public static string RemoveBlankLines(this string text, bool leaveLonelyNewlines = true)
		{
			var sb = new StringBuilder();
			var distanceFromNewline = 0;
			var sbLine = new StringBuilder();
			var foundNonSpaceCharacters = false;
			bool inAnsi = false;
			foreach (var c in text)
			{
				if (c == '\n')
				{
					if (distanceFromNewline == 0 && leaveLonelyNewlines)
					{
						sb.Append('\n');
						continue;
					}

					if (foundNonSpaceCharacters)
					{
						sb.Append(sbLine);
						sb.Append('\n');
						sbLine.Clear();
						foundNonSpaceCharacters = false;
						distanceFromNewline = 0;
						inAnsi = false;
						continue;
					}

					sbLine.Clear();
					distanceFromNewline = 0;
					inAnsi = false;
					continue;
				}

				if (c == '\x1B')
				{
					inAnsi = true;
				}

				if (c == '\r')
				{
					continue;
				}
				
				distanceFromNewline += 1;
				sbLine.Append(c);
				if (!foundNonSpaceCharacters && !inAnsi && !char.IsWhiteSpace(c) && c != Telnet.NoWordWrapChar)
				{
					foundNonSpaceCharacters = true;
				}

				if (inAnsi && c == 'm')
				{
					inAnsi = false;
				}
			}

			if (sbLine.Length > 0)
			{
				sb.Append(sbLine);
			}
			return sb.ToString();
		}

		public static Regex RegexCleanupRegex => new(@"\((?<option1>[a-z0-9]+)\|(?<option2>[a-z0-9]+)+\|*(?<option3>[a-z0-9]*)+\)", RegexOptions.IgnoreCase);

		public static string TransformRegexIntoPattern(this Regex regex)
		{
			var text = regex
				   .ToString()
				   .Replace("^", "")
				   .Replace("$", "")
				   .Replace("?", "")
				;

			return RegexCleanupRegex.Replace(text, m => m.Groups["option1"].Value);
		}

		class Latin1EncoderFallback : EncoderFallback
		{
			public override int MaxCharCount { get { return 11; } }
			private EncoderFallbackBuffer _buffer = new Latin1EncoderFallbackBuffer();
			public override EncoderFallbackBuffer CreateFallbackBuffer()
			{
				return _buffer;
			}
		}

		class Latin1EncoderFallbackBuffer : EncoderFallbackBuffer
		{
			public Latin1EncoderFallbackBuffer()
			{
				_encoded.Clear();
				_nextIndex = 0;
			}

			private List<char> _encoded = new(11);
			private int _nextIndex = 0;

			public override int Remaining => _encoded.Count - _nextIndex;

			public override bool Fallback(char unknownChar, int index)
			{
				var normalizedString = unknownChar.ToString().Normalize(NormalizationForm.FormD);
				_encoded.Clear();

				foreach (var c in normalizedString)
				{
					switch (c)
					{
						case '\u2554':
						case '\u255a':
						case '\u2566':
						case '\u2569':
						case '\u256c':
						case '\u2557':
						case '\u2563':
						case '\u2560':
						case '\u255d':
							_encoded.Add('+');
							continue;
						case '\u2550':
							_encoded.Add('-');
							continue;
						case '\u2551':
							_encoded.Add('|');
							continue;

					}

					if (c < 256)
					{
						_encoded.Add(c);
						continue;
					}
					
					// Check if the Unicode category of the character is not NonSpacingMark
					// This filters out diacritic marks
					if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
					{
						_encoded.Add('?');
					}
				}

				_nextIndex = 0;
				return true;
			}

			public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
			{
				return false;
			}

			public override char GetNextChar()
			{
				char next;
				if (_nextIndex < _encoded.Count)
				{
					next = _encoded[_nextIndex];
					_nextIndex += 1;
				}
				else
				{
					next = default(char);
				}

				return next;
			}

			public override bool MovePrevious()
			{
				bool result;

				if (_nextIndex > 0)
				{
					_nextIndex -= 1;
					result = true;
				}
				else
				{
					result = false;
				}

				return result;
			}

			public override void Reset()
			{
				_encoded.Clear();
				_nextIndex = 0;
			}
		}

		public static Encoding Latin1Encoder { get; } = Encoding.GetEncoding("iso-8859-1", new Latin1EncoderFallback(), DecoderFallback.ExceptionFallback);

		public static string ConvertToLatin1(this string input)
		{
			return Latin1Encoder.GetString(Latin1Encoder.GetBytes(input));
		}

		public static string ConvertToAscii(this string input)
		{
			var normalizedString = input.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				// Check if the Unicode category of the character is not NonSpacingMark
				// This filters out diacritic marks
				if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			// Convert to ascii - any non-ascii character will be converted to '?'
			return Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(stringBuilder.ToString()));
		}
	}
}