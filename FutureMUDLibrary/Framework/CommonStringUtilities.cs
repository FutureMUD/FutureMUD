using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace MudSharp.Framework
{
	public enum StringListToCSVOptions
	{
		None,
		SpaceAfterComma,
		RFC4180Compliant
	}

	public static partial class CommonStringUtilities
	{
		public static string WrapText(this string text, int width, string indent = "")
		{
			if (width < 1)
			{
				return text;
			}

			var sb = new StringBuilder();
			var strings = text.Replace("\t", "  ").Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
			if (strings.Any() && string.IsNullOrWhiteSpace(strings.Last()))
			{
				strings = strings.Take(strings.Length - 1).ToArray();
			}

			foreach (var line in strings)
			{
				var remains = line;
				if (remains.RawTextLength() <= width - indent.Length)
				{
					sb.AppendLine(indent + line);
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
					sb.AppendLine(indent + remains.TrimStart());
				}
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

		public static string CultureFormat(this IFormattable text, IFormatProvider culture)
		{
			return text.ToString(null, culture);
		}

		/// <summary>
		/// This function splits up specified text into "columns", in a fashion similar to what you might expect to see in a newspaper or word processor.
		/// That is to say, the text will go down to a certain point then continue back at the first line offset horizontally.
		/// 
		/// E.g.:
		/// 
		/// Item 1    Item 4
		/// Item 2    Item 5
		/// Item 3    Item 6
		/// </summary>
		/// <param name="text">The text to split into columns, generally without line-wrapping applied</param>
		/// <param name="columns">The number of columns</param>
		/// <param name="lineLength">The length of the total line</param>
		/// <param name="columnBufferSpaces">The number of spaces to use as a buffer between each column</param>
		/// <returns>The formatted text</returns>
		public static string SplitTextIntoColumns(this string text, uint columns = 2, uint lineLength = 120, uint columnBufferSpaces = 2)
		{
			var lines = text.Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList();
			var linesPerColumn = lines.Count / (int)columns;
			var remainder = lines.Count % (int)columns;
			var lineWidthPerColumn = (lineLength / columns) - (columnBufferSpaces / 2);
			var i = 0;
			foreach (var line in lines.ToList())
			{
				if (line.RawTextLength() > lineWidthPerColumn)
				{
					var newLines = line.WrapText((int)lineWidthPerColumn).Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList();
					lines.RemoveAt(i);
					for (var j = 0; j < newLines.Count; j++)
					{
						lines.Insert(i + j, newLines[j]);
					}
					i += newLines.Count;
				}
				else
				{
					i++;
				}
			}

			var sb = new StringBuilder();
			var firstColumnLines = linesPerColumn + (remainder > 0 ? 1 : 0);
			var columnDictionary = new Dictionary<int, List<string>>();
			var runningOffset = 0;
			for (i = 0; i < columns; i++)
			{
				if (i < remainder)
				{
					columnDictionary[i] = lines.Skip(runningOffset).Take(linesPerColumn + 1).ToList();
					runningOffset += linesPerColumn + 1;
				}
				else
				{
					columnDictionary[i] = lines.Skip(runningOffset).Take(linesPerColumn).ToList();
					runningOffset += linesPerColumn;
				}
			}
			
			for (i = 0; i < firstColumnLines; i++)
			{
				for (var j = 0; j < columns; j++)
				{
					if (i < columnDictionary[j].Count)
					{
						var thisLine = columnDictionary[j][i];
						sb.Append(thisLine.RawTextPadRight((int)lineWidthPerColumn));
					}

					if (j == (linesPerColumn - 1) && columnBufferSpaces > 0)
					{
						sb.Append(new string(' ', (int)columnBufferSpaces));
					}
				}

				sb.AppendLine();
			}

			return sb.ToString();
		}

		/// <summary>
		/// This function splits up specified text into "columns", in a fashion similar to what you might expect to see in a newspaper or word processor.
		/// That is to say, the text will go down to a certain point then continue back at the first line offset horizontally.
		/// 
		/// E.g.:
		/// 
		/// Item 1    Item 4
		/// Item 2    Item 5
		/// Item 3    Item 6
		/// </summary>
		/// <typeparam name="T">Any type implementing IEnumerable<<string>></typeparam>
		/// <param name="inputLines">An IEnumerable of strings, where each string represents a line</param>
		/// <param name="columns">The number of columns</param>
		/// <param name="lineLength">The length of the total line</param>
		/// <param name="columnBufferSpaces">The number of spaces to use as a buffer between each column</param>
		/// <returns>The formatted text</returns>
		public static string SplitTextIntoColumns<T> (this T inputLines, uint columns = 2, uint lineLength = 120, uint columnBufferSpaces = 2)
			where T : class, IEnumerable<string>
		{
			var lines = inputLines.ToList();
			var linesPerColumn = lines.Count / (int)columns;
			var remainder = lines.Count % (int)columns;
			var lineWidthPerColumn = (lineLength / columns) - (columnBufferSpaces / 2);
			var i = 0;
			foreach (var line in lines.ToList())
			{
				if (line.RawTextLength() > lineWidthPerColumn)
				{
					var newLines = line.WrapText((int)lineWidthPerColumn).Split(new string[] { "\r\n", "\n", "\r" }, StringSplitOptions.None).ToList();
					lines.RemoveAt(i);
					for (var j = 0; j < newLines.Count; j++)
					{
						lines.Insert(i + j, newLines[j]);
					}
					i += newLines.Count;
				}
				else
				{
					i++;
				}
			}

			var sb = new StringBuilder();
			var firstColumnLines = linesPerColumn + (remainder > 0 ? 1 : 0);
			var columnDictionary = new Dictionary<int, List<string>>();
			var runningOffset = 0;
			for (i = 0; i < columns; i++)
			{
				if (i < remainder)
				{
					columnDictionary[i] = lines.Skip(runningOffset).Take(linesPerColumn + 1).ToList();
					runningOffset += linesPerColumn + 1;
				}
				else
				{
					columnDictionary[i] = lines.Skip(runningOffset).Take(linesPerColumn).ToList();
					runningOffset += linesPerColumn;
				}
			}

			for (i = 0; i < firstColumnLines; i++)
			{
				for (var j = 0; j < columns; j++)
				{
					if (i < columnDictionary[j].Count)
					{
						var thisLine = columnDictionary[j][i];
						sb.Append(thisLine.RawTextPadRight((int)lineWidthPerColumn));
					}

					if (j == (linesPerColumn - 1) && columnBufferSpaces > 0)
					{
						sb.Append(new string(' ', (int)columnBufferSpaces));
					}
				}

				sb.AppendLine();
			}

			return sb.ToString();
		}

		public static string ArrangeStringsOntoLines(this IEnumerable<string> strings, uint numberPerLine = 4,
													 uint lineLength = 120)
		{
			var sb = new StringBuilder();
			var currentLine = new StringBuilder();
			if (numberPerLine == 0) {
				numberPerLine = 1;
			}

			var charactersAllowedPerString = (int)(lineLength / numberPerLine);
			var currentLineCount = 0;

			foreach (var entry in strings)
			{
				if (currentLineCount == numberPerLine)
				{
					sb.AppendLine(currentLine.ToString().TrimEnd());
					currentLine = new StringBuilder();
					currentLineCount = 0;
				}

				currentLine.Append(entry.RawTextPadRight(charactersAllowedPerString));
				currentLineCount++;
			}
			sb.AppendLine(currentLine.ToString().TrimEnd());
			return sb.ToString();
		}

		private static readonly Regex WindowsLineEndingRegex = new(@"(?<!\r)\n");
		public static string Indent = "   ";

		// TODO: Instead of taking a string as the article, allow it to handle properly indefinite articles, and numbered articles.
		/// <summary>
		///     Takes any list of strings, and then outputs it in to a grammatically correct string as a single sentence.
		/// </summary>
		/// <param name="item">The list of strings</param>
		/// <param name="article">The article to proceed each item in a string: a, the, some,"",</param>
		/// <param name="separator">How items in the string are to be separated: ,;-:</param>
		/// <param name="conjunction">How the article of the final item is proceeded: "the dog and the cat", "the dog or the cat"</param>
		/// <param name="twoItemJoiner">What to insert before the conjunction if the list has exactly two items</param>
		/// <param name="oxfordComma">
		///     Whether the separator appears before the last item: "mouse, dog and cat" vs "mouse, dog, and
		///     cat"
		/// </param>
		/// <returns>A wonderfully formatted string.</returns>
		public static string ListToString<T>(this T item, string article = "", string separator = ", ",
			string conjunction = "and ", string twoItemJoiner = " ", bool oxfordComma = true)
			where T : IEnumerable<string>
		{
			if (item == null)
			{
				return "";
			}

			var itemList = item.ToList();
			switch (itemList.Count)
			{
				case 0:
					return "";
				case 1:
					return article + itemList[0];
				case 2:
					return article + itemList[0] + twoItemJoiner + conjunction + article + itemList[1];
			}

			return
				string.Join(separator, itemList.GetRange(0, itemList.Count - 1).ConvertAll(x => x.Insert(0, article))) +
				(oxfordComma ? separator : "") + conjunction + article + itemList[^1];
		}

		/// <summary>
		/// Returns the items in the list on separate lines of a single string text response
		/// </summary>
		/// <typeparam name="T">Any IEnumerable of strings</typeparam>
		/// <param name="item">The list of strings</param>
		/// <param name="includedTabs">Whether to include a tab before each item in the list</param>
		/// <returns>A wonderfully formatted string</returns>
		public static string ListToLines<T>(this T item, bool includedTabs = false) where T: IEnumerable<string>
		{
			return item.ListToString(includedTabs ? "\t" : "", "\n", conjunction: "", twoItemJoiner: "\n");
		}

		/// <summary>
		/// Turns a list of strings into a single string representing a CSV
		/// </summary>
		/// <typeparam name="T">Any IEnumerable of strings</typeparam>
		/// <param name="items">The list of strings</param>
		/// <param name="separator">The separator to use between each item, commas by default</param>
		/// <param name="options">Additional options about how to format the CSV</param>
		/// <returns>A CSV</returns>
		public static string ListToCommaSeparatedValues<T>(this T items, string separator = ",", StringListToCSVOptions options = StringListToCSVOptions.None) where T : IEnumerable<string>
		{
			if (items == null)
			{
				return "";
			}

			var sb = new StringBuilder();
			foreach (var item in items)
			{
				if (sb.Length > 0)
				{
					sb.Append(separator);
					if (options == StringListToCSVOptions.SpaceAfterComma)
					{
						sb.Append(' ');
					}
				}
                               if (options == StringListToCSVOptions.RFC4180Compliant)
                               {
                                       if (item.Contains('"') || item.Contains(separator) || item.Contains('\n'))
                                       {
                                               sb.Append('"');
                                               sb.Append(item.Replace("\"", "\"\""));
                                               sb.Append('"');
                                       }
                                       else
                                       {
                                               sb.Append(item);
                                       }
                               }
                               else
                               {
                                       sb.Append(item);
                               }
			}
			return sb.ToString();
		}

		public static string ListToCompactString<T>(this T item, string separator = ", ", string conjunction = "and ", string twoItemJoiner = " ", bool oxfordComma = true, string compactFormat = "{0} (x{1})") where T : IEnumerable<string>
		{
			if (item == null)
			{
				return "";
			}

			var items =
				(from x in item
				 group x by x into g
				 let count = g.Count()
				 select count == 1 ? g.Key : string.Format(compactFormat, g.Key, g.Count())
				).ToList();

			switch (items.Count)
			{
				case 0:
					return "";
				case 1:
					return items[0];
				case 2:
					return $"{items[0]}{twoItemJoiner}{conjunction}{items[1]}";
			}

			return
				$"{string.Join(separator, items.GetRange(0, items.Count - 1))}{(oxfordComma ? separator : "")}{conjunction}{items[^1]}";
		}

		/// <summary>
		/// This is a shortcut for doing .Select(x = x.DescribeEnum().Colour()).ListToString()
		/// </summary>
		/// <typeparam name="T">Any enum type</typeparam>
		/// <param name="items">A collection of enums</param>
		/// <param name="colour">The colour to give the values - else Telnet.Green</param>
		/// <param name="explodeCamelCase">Whether to explode camel case in enum names</param>
		/// <returns>The string result</returns>
		public static string ListToColouredString<T>(this IEnumerable<T> items, ANSIColour colour = null, bool explodeCamelCase = false) where T : Enum
		{
			colour ??= Telnet.Green;
			return items.Select(x => x.DescribeEnum(explodeCamelCase, colour)).ListToString();
		}

		/// <summary>
		/// This is a shortcut for doing .Select(x = x.DescribeEnum().Colour()).ListToString(conjunction: "or ")
		/// </summary>
		/// <typeparam name="T">Any enum type</typeparam>
		/// <param name="items">A collection of enums</param>
		/// <param name="colour">The colour to give the values - else Telnet.Green</param>
		/// <param name="explodeCamelCase">Whether to explode camel case in enum names</param>
		/// <returns>The string result</returns>
		public static string ListToColouredStringOr<T>(this IEnumerable<T> items, ANSIColour colour = null, bool explodeCamelCase = false) where T : Enum
		{
			colour ??= Telnet.Green;
			return items.Select(x => x.DescribeEnum(explodeCamelCase, colour)).ListToString(conjunction: "or ");
		}

		/// <summary>
		/// This is a shortcut for doing .Select(x => x.Colour()).ListToString(conjunction: " or")
		/// </summary>
		/// <param name="items">A collection of strings</param>
		/// <param name="colour">The colour to give the values - else Telnet.Green</param>
		/// <returns>The string result</returns>
		public static string ListToColouredStringOr(this IEnumerable<string> items, ANSIColour colour = null)
		{
			colour ??= Telnet.Green;
                       return items.Select(x => x.Colour(colour)).ListToString(conjunction: "or ");
               }

		/// <summary>
		/// This is a shortcut for doing .Select(x => x.Colour()).ListToString()
		/// </summary>
		/// <param name="items">A collection of strings</param>
		/// <param name="colour">The colour to give the values - else Telnet.Green</param>
		/// <returns>The string result</returns>
		public static string ListToColouredString(this IEnumerable<string> items, ANSIColour colour = null)
		{
			colour ??= Telnet.Green;
			return items.Select(x => x.Colour(colour)).ListToString();
		}

		/// <summary>
		/// An alias for .ToString("N0", format).Colour(colour) - default colour green if not specified
		/// </summary>
		/// <typeparam name="T">Any numeric type</typeparam>
		/// <param name="number">The number to format</param>
		/// <param name="format">The IFormatProvider to format the string</param>
		/// <param name="colour">The colour - default Telnet.Green if not provided</param>
		/// <returns>The number as a string</returns>
		public static string ToStringN0Colour<T>(this T number, IFormatProvider format = null, ANSIColour colour = null) where T : INumber<T>
		{
			colour ??= Telnet.Green;
			return number.ToString("N0", format).Colour(colour);
		}

		/// <summary>
		/// An alias for .ToString("N0", format)
		/// </summary>
		/// <typeparam name="T">Any numeric type</typeparam>
		/// <param name="number">The number to format</param>
		/// <param name="format">The IFormatProvider to format the string</param>
		/// <returns>The number as a string</returns>
		public static string ToStringN0<T>(this T number, IFormatProvider format = null) where T : INumber<T>
		{
			return number.ToString("N0", format);
		}

		/// <summary>
		/// An alias for .ToString("N2", format).Colour(colour)
		/// </summary>
		/// <typeparam name="T">Any numeric type</typeparam>
		/// <param name="number">The number to format</param>
		/// <param name="format">The IFormatProvider to format the string</param>
		/// <param name="colour">The colour - default Telnet.Green if not provided</param>
		/// <returns>The number as a string</returns>
		public static string ToStringN2Colour<T>(this T number, IFormatProvider format = null, ANSIColour colour = null) where T : INumber<T>
		{
			colour ??= Telnet.Green;
			return number.ToString("N2", format).Colour(colour);
		}

		/// <summary>
		/// An alias for .ToString("P2", format).Colour(colour)
		/// </summary>
		/// <typeparam name="T">Any numeric type</typeparam>
		/// <param name="number">The number to format</param>
		/// <param name="format">The IFormatProvider to format the string</param>
		/// <param name="colour">The colour - default Telnet.Green if not provided</param>
		/// <returns>The number as a string</returns>
		public static string ToStringP2Colour<T>(this T number, IFormatProvider format = null, ANSIColour colour = null) where T : INumber<T>
		{
			colour ??= Telnet.Green;
			return number.ToString("P2", format).Colour(colour);
		}

		public static string WindowsLineEndings(this string input)
		{
			return WindowsLineEndingRegex.Replace(input, m => "\n");
		}

		public static bool Contains(this string source, string toCheck, StringComparison comp)
		{
			if (string.IsNullOrEmpty(toCheck) || string.IsNullOrEmpty(source))
			{
				return true;
			}

			return source.IndexOf(toCheck, comp) >= 0;
		}

		public static string GetWidthRuler(int width) {
			if (width <= 80) {
				return new StringBuilder("|").Append(new string('-', 78)).Append("|").ToString();
			}

			return
				new StringBuilder("|").Append(new string('-', 78))
									  .Append("|")
									  .Append(new string('-', width - 80 - 2))
									  .Append("|")
									  .ToString();
		}

		//// str - the source string
		//// index- the start location to replace at (0-based)
		//// length - the number of characters to be removed before inserting
		//// replace - the string that is replacing characters
		public static string ReplaceAt(this string str, int index, int length, string replace)
		{
			return str.Remove(index, Math.Min(length, str.Length - index))
				.Insert(index, replace);
		}

		public static StringBuilder AppendLineColumns(this StringBuilder sb, uint linelength, uint columns,
			params string[] strings)
		{
			return sb.Append(strings.ArrangeStringsOntoLines(columns, linelength));
		}
	}
}
