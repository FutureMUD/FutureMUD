using MudSharp.Framework;
using MudSharp.FutureProg.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MudSharp.FutureProg.Functions.Textual;

internal class TextUtilityFunction : BuiltInFunction
{
	private const int MaximumGeneratedTextLength = 65536;
	private readonly TextOperation _operation;
	private readonly ProgVariableTypes _returnType;

	private enum TextOperation
	{
		Length,
		Uppercase,
		Lowercase,
		Propercase,
		Titlecase,
		Trim,
		TrimStart,
		TrimEnd,
		Left,
		Right,
		Mid,
		Contains,
		ContainsCase,
		StartsWith,
		StartsWithCase,
		EndsWith,
		EndsWithCase,
		IndexOf,
		IndexOfCase,
		LastIndexOf,
		LastIndexOfCase,
		Replace,
		ReplaceCase,
		Insert,
		Remove,
		Repeat,
		IsEmpty,
		IsBlank,
		WordCount,
		LineCount,
		FirstWord,
		LastWord,
		Before,
		After,
		Between,
		PadLeft,
		PadRight,
		Truncate,
		TruncateEllipsis,
		NormalizeWhitespace,
		Count,
		CountCase,
		Reverse,
		CapitalizeFirst
	}

	private TextUtilityFunction(IList<IFunction> parameters, TextOperation operation, ProgVariableTypes returnType) : base(parameters)
	{
		_operation = operation;
		_returnType = returnType;
	}

	public override ProgVariableTypes ReturnType
	{
		get => _returnType;
		protected set { }
	}

	public override StatementResult Execute(IVariableSpace variables)
	{
		if (base.Execute(variables) == StatementResult.Error)
		{
			return StatementResult.Error;
		}

		var source = GetText(0);
		switch (_operation)
		{
			case TextOperation.Length:
				Result = new NumberVariable(source.Length);
				return StatementResult.Normal;
			case TextOperation.Uppercase:
				Result = new TextVariable(source.ToUpperInvariant());
				return StatementResult.Normal;
			case TextOperation.Lowercase:
				Result = new TextVariable(source.ToLowerInvariant());
				return StatementResult.Normal;
			case TextOperation.Propercase:
				Result = new TextVariable(source.ProperSentences());
				return StatementResult.Normal;
			case TextOperation.Titlecase:
				Result = new TextVariable(source.TitleCase());
				return StatementResult.Normal;
			case TextOperation.Trim:
				Result = new TextVariable(source.Trim());
				return StatementResult.Normal;
			case TextOperation.TrimStart:
				Result = new TextVariable(source.TrimStart());
				return StatementResult.Normal;
			case TextOperation.TrimEnd:
				Result = new TextVariable(source.TrimEnd());
				return StatementResult.Normal;
			case TextOperation.Left:
				Result = new TextVariable(Left(source, GetInteger(1)));
				return StatementResult.Normal;
			case TextOperation.Right:
				Result = new TextVariable(Right(source, GetInteger(1)));
				return StatementResult.Normal;
			case TextOperation.Mid:
				Result = new TextVariable(Mid(source, GetInteger(1), GetInteger(2)));
				return StatementResult.Normal;
			case TextOperation.Contains:
				Result = new BooleanVariable(source.Contains(GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.ContainsCase:
				Result = new BooleanVariable(source.Contains(GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.StartsWith:
				Result = new BooleanVariable(source.StartsWith(GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.StartsWithCase:
				Result = new BooleanVariable(source.StartsWith(GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.EndsWith:
				Result = new BooleanVariable(source.EndsWith(GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.EndsWithCase:
				Result = new BooleanVariable(source.EndsWith(GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.IndexOf:
				Result = new NumberVariable(source.IndexOf(GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.IndexOfCase:
				Result = new NumberVariable(source.IndexOf(GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.LastIndexOf:
				Result = new NumberVariable(source.LastIndexOf(GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.LastIndexOfCase:
				Result = new NumberVariable(source.LastIndexOf(GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.Replace:
				Result = new TextVariable(Replace(source, GetText(1), GetText(2), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.ReplaceCase:
				Result = new TextVariable(Replace(source, GetText(1), GetText(2), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.Insert:
				Result = new TextVariable(Insert(source, GetInteger(1), GetText(2)));
				return StatementResult.Normal;
			case TextOperation.Remove:
				Result = new TextVariable(Remove(source, GetInteger(1), GetInteger(2)));
				return StatementResult.Normal;
			case TextOperation.Repeat:
				return ExecuteRepeat(source, Math.Max(0, GetInteger(1)));
			case TextOperation.IsEmpty:
				Result = new BooleanVariable(string.IsNullOrEmpty(source));
				return StatementResult.Normal;
			case TextOperation.IsBlank:
				Result = new BooleanVariable(string.IsNullOrWhiteSpace(source));
				return StatementResult.Normal;
			case TextOperation.WordCount:
				Result = new NumberVariable(GetWords(source).Length);
				return StatementResult.Normal;
			case TextOperation.LineCount:
				Result = new NumberVariable(GetLineCount(source));
				return StatementResult.Normal;
			case TextOperation.FirstWord:
				Result = new TextVariable(GetWords(source).FirstOrDefault() ?? string.Empty);
				return StatementResult.Normal;
			case TextOperation.LastWord:
				Result = new TextVariable(GetWords(source).LastOrDefault() ?? string.Empty);
				return StatementResult.Normal;
			case TextOperation.Before:
				Result = new TextVariable(Before(source, GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.After:
				Result = new TextVariable(After(source, GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.Between:
				Result = new TextVariable(Between(source, GetText(1), GetText(2)));
				return StatementResult.Normal;
			case TextOperation.PadLeft:
				return ExecutePad(source, Math.Max(0, GetInteger(1)), true);
			case TextOperation.PadRight:
				return ExecutePad(source, Math.Max(0, GetInteger(1)), false);
			case TextOperation.Truncate:
				Result = new TextVariable(Left(source, GetInteger(1)));
				return StatementResult.Normal;
			case TextOperation.TruncateEllipsis:
				Result = new TextVariable(TruncateEllipsis(source, GetInteger(1)));
				return StatementResult.Normal;
			case TextOperation.NormalizeWhitespace:
				Result = new TextVariable(NormalizeWhitespace(source));
				return StatementResult.Normal;
			case TextOperation.Count:
				Result = new NumberVariable(Count(source, GetText(1), StringComparison.InvariantCultureIgnoreCase));
				return StatementResult.Normal;
			case TextOperation.CountCase:
				Result = new NumberVariable(Count(source, GetText(1), StringComparison.InvariantCulture));
				return StatementResult.Normal;
			case TextOperation.Reverse:
				Result = new TextVariable(new string(source.Reverse().ToArray()));
				return StatementResult.Normal;
			case TextOperation.CapitalizeFirst:
				Result = new TextVariable(CapitalizeFirst(source));
				return StatementResult.Normal;
			default:
				throw new NotSupportedException($"Unknown text utility operation {_operation}.");
		}
	}

	private string GetText(int index)
	{
		return ParameterFunctions[index].Result?.GetObject?.ToString() ?? string.Empty;
	}

	private int GetInteger(int index)
	{
		if (ParameterFunctions[index].Result?.GetObject is not decimal value)
		{
			return 0;
		}

		if (value > int.MaxValue)
		{
			return int.MaxValue;
		}

		if (value < int.MinValue)
		{
			return int.MinValue;
		}

		return (int)value;
	}

	private StatementResult ExecuteRepeat(string source, int count)
	{
		if (count == 0 || source.Length == 0)
		{
			Result = new TextVariable(string.Empty);
			return StatementResult.Normal;
		}

		if ((long)source.Length * count > MaximumGeneratedTextLength)
		{
			ErrorMessage = $"repeattext cannot create text longer than {MaximumGeneratedTextLength:N0} characters.";
			return StatementResult.Error;
		}

		Result = new TextVariable(string.Concat(Enumerable.Repeat(source, count)));
		return StatementResult.Normal;
	}

	private StatementResult ExecutePad(string source, int width, bool padLeft)
	{
		if (width > MaximumGeneratedTextLength)
		{
			ErrorMessage = $"pad text functions cannot create text longer than {MaximumGeneratedTextLength:N0} characters.";
			return StatementResult.Error;
		}

		Result = new TextVariable(padLeft ? source.PadLeft(width) : source.PadRight(width));
		return StatementResult.Normal;
	}

	private static string Left(string text, int count)
	{
		if (count <= 0)
		{
			return string.Empty;
		}

		return text.Length <= count ? text : text[..count];
	}

	private static string Right(string text, int count)
	{
		if (count <= 0)
		{
			return string.Empty;
		}

		return text.Length <= count ? text : text[^count..];
	}

	private static string Mid(string text, int index, int count)
	{
		index = Math.Max(0, index);
		count = Math.Max(0, count);
		if (index >= text.Length || count == 0)
		{
			return string.Empty;
		}

		return text.Substring(index, Math.Min(count, text.Length - index));
	}

	private static string Replace(string text, string search, string replacement, StringComparison comparison)
	{
		return string.IsNullOrEmpty(search) ? text : text.Replace(search, replacement, comparison);
	}

	private static string Insert(string text, int index, string insertion)
	{
		index = Math.Clamp(index, 0, text.Length);
		return text.Insert(index, insertion);
	}

	private static string Remove(string text, int index, int count)
	{
		index = Math.Max(0, index);
		count = Math.Max(0, count);
		if (index >= text.Length || count == 0)
		{
			return text;
		}

		return text.Remove(index, Math.Min(count, text.Length - index));
	}

	private static string[] GetWords(string text)
	{
		return text.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
	}

	private static int GetLineCount(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}

		return text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').Length;
	}

	private static string Before(string text, string marker, StringComparison comparison)
	{
		if (string.IsNullOrEmpty(marker))
		{
			return string.Empty;
		}

		var index = text.IndexOf(marker, comparison);
		return index < 0 ? string.Empty : text[..index];
	}

	private static string After(string text, string marker, StringComparison comparison)
	{
		if (string.IsNullOrEmpty(marker))
		{
			return string.Empty;
		}

		var index = text.IndexOf(marker, comparison);
		return index < 0 ? string.Empty : text[(index + marker.Length)..];
	}

	private static string Between(string text, string startMarker, string endMarker)
	{
		if (string.IsNullOrEmpty(startMarker) || string.IsNullOrEmpty(endMarker))
		{
			return string.Empty;
		}

		var start = text.IndexOf(startMarker, StringComparison.InvariantCultureIgnoreCase);
		if (start < 0)
		{
			return string.Empty;
		}

		start += startMarker.Length;
		var end = text.IndexOf(endMarker, start, StringComparison.InvariantCultureIgnoreCase);
		return end < 0 ? string.Empty : text[start..end];
	}

	private static string TruncateEllipsis(string text, int count)
	{
		if (count <= 0)
		{
			return string.Empty;
		}

		if (text.Length <= count)
		{
			return text;
		}

		return count <= 3 ? text[..count] : $"{text[..(count - 3)]}...";
	}

	private static string NormalizeWhitespace(string text)
	{
		List<char> output = new();
		var inWhitespace = false;
		foreach (var character in text)
		{
			if (char.IsWhiteSpace(character))
			{
				if (!inWhitespace && output.Count > 0)
				{
					output.Add(' ');
				}

				inWhitespace = true;
				continue;
			}

			output.Add(character);
			inWhitespace = false;
		}

		if (output.Count > 0 && output[^1] == ' ')
		{
			output.RemoveAt(output.Count - 1);
		}

		return new string(output.ToArray());
	}

	private static int Count(string text, string search, StringComparison comparison)
	{
		if (string.IsNullOrEmpty(search))
		{
			return 0;
		}

		var count = 0;
		var index = 0;
		while ((index = text.IndexOf(search, index, comparison)) >= 0)
		{
			count++;
			index += search.Length;
		}

		return count;
	}

	private static string CapitalizeFirst(string text)
	{
		return string.IsNullOrEmpty(text) ? string.Empty : $"{char.ToUpperInvariant(text[0])}{text[1..]}";
	}

	private static void RegisterTextFunction(string name, TextOperation operation, ProgVariableTypes returnType,
		IEnumerable<ProgVariableTypes> parameters, IEnumerable<string> parameterNames,
		IEnumerable<string> parameterHelp, string functionHelp)
	{
		FutureProg.RegisterBuiltInFunctionCompiler(new FunctionCompilerInformation(
			name,
			parameters,
			(pars, gameworld) => new TextUtilityFunction(pars, operation, returnType),
			parameterNames,
			parameterHelp,
			functionHelp,
			"Text",
			returnType
		));
	}

	private static void RegisterUnaryTextFunction(string name, TextOperation operation, ProgVariableTypes returnType,
		string functionHelp)
	{
		RegisterTextFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.Text },
			new[] { "text" },
			new[] { "The text value to inspect or transform" },
			functionHelp
		);
	}

	private static void RegisterTextAndNeedleFunction(string name, TextOperation operation, ProgVariableTypes returnType,
		string functionHelp)
	{
		RegisterTextFunction(
			name,
			operation,
			returnType,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text },
			new[] { "text", "search" },
			new[] { "The text value to inspect", "The text fragment to search for" },
			functionHelp
		);
	}

	public static void RegisterFunctionCompiler()
	{
		RegisterUnaryTextFunction("textlength", TextOperation.Length, ProgVariableTypes.Number,
			"Returns the number of characters in the supplied text.");
		RegisterUnaryTextFunction("uppercase", TextOperation.Uppercase, ProgVariableTypes.Text,
			"Returns the supplied text converted to upper case.");
		RegisterUnaryTextFunction("lowercase", TextOperation.Lowercase, ProgVariableTypes.Text,
			"Returns the supplied text converted to lower case.");
		RegisterUnaryTextFunction("propercase", TextOperation.Propercase, ProgVariableTypes.Text,
			"Returns the supplied text with sentence-style capitalisation.");
		RegisterUnaryTextFunction("titlecase", TextOperation.Titlecase, ProgVariableTypes.Text,
			"Returns the supplied text with title-style capitalisation.");
		RegisterUnaryTextFunction("trim", TextOperation.Trim, ProgVariableTypes.Text,
			"Returns the supplied text with leading and trailing whitespace removed.");
		RegisterUnaryTextFunction("trimstart", TextOperation.TrimStart, ProgVariableTypes.Text,
			"Returns the supplied text with leading whitespace removed.");
		RegisterUnaryTextFunction("trimend", TextOperation.TrimEnd, ProgVariableTypes.Text,
			"Returns the supplied text with trailing whitespace removed.");
		RegisterUnaryTextFunction("isemptytext", TextOperation.IsEmpty, ProgVariableTypes.Boolean,
			"Returns true if the supplied text is empty.");
		RegisterUnaryTextFunction("isblanktext", TextOperation.IsBlank, ProgVariableTypes.Boolean,
			"Returns true if the supplied text is empty or contains only whitespace.");
		RegisterUnaryTextFunction("wordcount", TextOperation.WordCount, ProgVariableTypes.Number,
			"Returns the number of whitespace-separated words in the supplied text.");
		RegisterUnaryTextFunction("linecount", TextOperation.LineCount, ProgVariableTypes.Number,
			"Returns the number of lines in the supplied text. Empty text returns zero.");
		RegisterUnaryTextFunction("firstword", TextOperation.FirstWord, ProgVariableTypes.Text,
			"Returns the first whitespace-separated word in the supplied text, or blank if there is none.");
		RegisterUnaryTextFunction("lastword", TextOperation.LastWord, ProgVariableTypes.Text,
			"Returns the last whitespace-separated word in the supplied text, or blank if there is none.");
		RegisterUnaryTextFunction("normalizewhitespace", TextOperation.NormalizeWhitespace, ProgVariableTypes.Text,
			"Returns the supplied text with all whitespace runs collapsed to single spaces and edge whitespace removed.");
		RegisterUnaryTextFunction("reversetext", TextOperation.Reverse, ProgVariableTypes.Text,
			"Returns the supplied text with its characters in reverse order.");
		RegisterUnaryTextFunction("capitalizefirst", TextOperation.CapitalizeFirst, ProgVariableTypes.Text,
			"Returns the supplied text with its first character converted to upper case.");

		RegisterTextFunction(
			"lefttext",
			TextOperation.Left,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "count" },
			new[] { "The text value to slice", "The maximum number of characters to return from the left side" },
			"Returns up to count characters from the left side of the supplied text."
		);
		RegisterTextFunction(
			"righttext",
			TextOperation.Right,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "count" },
			new[] { "The text value to slice", "The maximum number of characters to return from the right side" },
			"Returns up to count characters from the right side of the supplied text."
		);
		RegisterTextFunction(
			"midtext",
			TextOperation.Mid,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Number },
			new[] { "text", "index", "count" },
			new[] { "The text value to slice", "The zero-based starting index", "The maximum number of characters to return" },
			"Returns up to count characters from the supplied text starting at the zero-based index."
		);
		RegisterTextFunction(
			"inserttext",
			TextOperation.Insert,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Text },
			new[] { "text", "index", "insertion" },
			new[] { "The text value to edit", "The zero-based insertion index", "The text to insert" },
			"Returns the supplied text with another text value inserted at the zero-based index."
		);
		RegisterTextFunction(
			"removetext",
			TextOperation.Remove,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number, ProgVariableTypes.Number },
			new[] { "text", "index", "count" },
			new[] { "The text value to edit", "The zero-based index at which removal begins", "The maximum number of characters to remove" },
			"Returns the supplied text with up to count characters removed from the zero-based index."
		);
		RegisterTextFunction(
			"repeattext",
			TextOperation.Repeat,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "count" },
			new[] { "The text value to repeat", "The number of times to repeat the text" },
			"Returns the supplied text repeated count times."
		);
		RegisterTextFunction(
			"padleft",
			TextOperation.PadLeft,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "width" },
			new[] { "The text value to pad", "The minimum width of the returned text" },
			"Returns the supplied text padded on the left with spaces until it reaches the specified width."
		);
		RegisterTextFunction(
			"padright",
			TextOperation.PadRight,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "width" },
			new[] { "The text value to pad", "The minimum width of the returned text" },
			"Returns the supplied text padded on the right with spaces until it reaches the specified width."
		);
		RegisterTextFunction(
			"truncatetext",
			TextOperation.Truncate,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "count" },
			new[] { "The text value to truncate", "The maximum number of characters to keep" },
			"Returns the supplied text truncated to at most count characters."
		);
		RegisterTextFunction(
			"truncateellipsis",
			TextOperation.TruncateEllipsis,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Number },
			new[] { "text", "count" },
			new[] { "The text value to truncate", "The maximum number of characters to return, including the ellipsis" },
			"Returns the supplied text truncated to count characters, using an ellipsis when there is room."
		);

		RegisterTextAndNeedleFunction("textcontains", TextOperation.Contains, ProgVariableTypes.Boolean,
			"Returns true if the supplied text contains the search fragment, ignoring case.");
		RegisterTextAndNeedleFunction("textcontainscase", TextOperation.ContainsCase, ProgVariableTypes.Boolean,
			"Returns true if the supplied text contains the search fragment with case-sensitive matching.");
		RegisterTextAndNeedleFunction("startswith", TextOperation.StartsWith, ProgVariableTypes.Boolean,
			"Returns true if the supplied text starts with the search fragment, ignoring case.");
		RegisterTextAndNeedleFunction("startswithcase", TextOperation.StartsWithCase, ProgVariableTypes.Boolean,
			"Returns true if the supplied text starts with the search fragment with case-sensitive matching.");
		RegisterTextAndNeedleFunction("endswith", TextOperation.EndsWith, ProgVariableTypes.Boolean,
			"Returns true if the supplied text ends with the search fragment, ignoring case.");
		RegisterTextAndNeedleFunction("endswithcase", TextOperation.EndsWithCase, ProgVariableTypes.Boolean,
			"Returns true if the supplied text ends with the search fragment with case-sensitive matching.");
		RegisterTextAndNeedleFunction("textindexof", TextOperation.IndexOf, ProgVariableTypes.Number,
			"Returns the zero-based index of the first search fragment in the supplied text, ignoring case, or -1 if absent.");
		RegisterTextAndNeedleFunction("textindexofcase", TextOperation.IndexOfCase, ProgVariableTypes.Number,
			"Returns the zero-based index of the first search fragment in the supplied text with case-sensitive matching, or -1 if absent.");
		RegisterTextAndNeedleFunction("textlastindexof", TextOperation.LastIndexOf, ProgVariableTypes.Number,
			"Returns the zero-based index of the last search fragment in the supplied text, ignoring case, or -1 if absent.");
		RegisterTextAndNeedleFunction("textlastindexofcase", TextOperation.LastIndexOfCase, ProgVariableTypes.Number,
			"Returns the zero-based index of the last search fragment in the supplied text with case-sensitive matching, or -1 if absent.");
		RegisterTextAndNeedleFunction("textbefore", TextOperation.Before, ProgVariableTypes.Text,
			"Returns the text before the first search fragment, ignoring case, or blank if the fragment is absent.");
		RegisterTextAndNeedleFunction("textafter", TextOperation.After, ProgVariableTypes.Text,
			"Returns the text after the first search fragment, ignoring case, or blank if the fragment is absent.");
		RegisterTextAndNeedleFunction("counttext", TextOperation.Count, ProgVariableTypes.Number,
			"Returns the number of non-overlapping occurrences of the search fragment in the supplied text, ignoring case.");
		RegisterTextAndNeedleFunction("counttextcase", TextOperation.CountCase, ProgVariableTypes.Number,
			"Returns the number of non-overlapping occurrences of the search fragment in the supplied text with case-sensitive matching.");

		RegisterTextFunction(
			"replacetext",
			TextOperation.Replace,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text },
			new[] { "text", "search", "replacement" },
			new[] { "The text value to edit", "The text fragment to replace", "The replacement text" },
			"Returns the supplied text with all occurrences of search replaced, ignoring case."
		);
		RegisterTextFunction(
			"replacetextcase",
			TextOperation.ReplaceCase,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text },
			new[] { "text", "search", "replacement" },
			new[] { "The text value to edit", "The text fragment to replace", "The replacement text" },
			"Returns the supplied text with all case-sensitive occurrences of search replaced."
		);
		RegisterTextFunction(
			"textbetween",
			TextOperation.Between,
			ProgVariableTypes.Text,
			new[] { ProgVariableTypes.Text, ProgVariableTypes.Text, ProgVariableTypes.Text },
			new[] { "text", "start", "end" },
			new[] { "The text value to inspect", "The starting marker", "The ending marker" },
			"Returns the text between the first start marker and following end marker, ignoring marker case, or blank if either marker is absent."
		);
	}
}
