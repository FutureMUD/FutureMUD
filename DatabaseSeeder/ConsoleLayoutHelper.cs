using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DatabaseSeeder;

internal static class ConsoleLayoutHelper
{
    private const int MinimumWidth = 10;
    private const int DefaultWidth = 100;

    public static int GetSafeConsoleWidth()
    {
        try
        {
            int width = Console.WindowWidth;
            return width >= MinimumWidth ? width : DefaultWidth;
        }
        catch
        {
            return DefaultWidth;
        }
    }

    public static IEnumerable<string> WrapToConsoleLines(string text, int? width = null, string indent = "")
    {
        int effectiveWidth = Math.Max(MinimumWidth, width ?? GetSafeConsoleWidth());
        string wrapped = text.Wrap(Math.Max(1, effectiveWidth - 1), indent);
        return wrapped.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
    }

    public static IEnumerable<string> FormatMenuEntry(int index, string name, string status, string tagline, int? width = null)
    {
        string prefix = $"{index}) [{name,-20}] [{status,-8}] ";
        string continuationIndent = new(' ', prefix.Length);
        int effectiveWidth = Math.Max(MinimumWidth, width ?? GetSafeConsoleWidth());
        if (prefix.Length + 10 >= effectiveWidth)
        {
            foreach (var line in WrapToConsoleLines($"{index}) {name} [{status}]\n{tagline}", effectiveWidth))
            {
                yield return line;
            }
            yield break;
        }

        int availableTaglineWidth = Math.Max(1, effectiveWidth - prefix.Length - 1);
        string wrappedTagline = tagline.Wrap(availableTaglineWidth);
        List<string> lines = wrappedTagline.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
        if (!lines.Any() || string.IsNullOrWhiteSpace(lines[0]))
        {
            yield return prefix.TrimEnd();
            yield break;
        }

        yield return prefix + lines[0];
        foreach (string? line in lines.Skip(1))
        {
            yield return continuationIndent + line;
        }
    }

    public static void WriteWrapped(string text, int? width = null, string indent = "")
    {
        foreach (string line in WrapToConsoleLines(text, width, indent))
        {
            Console.WriteLine(line);
        }
    }

	public static IEnumerable<string> FormatPrompt(string text, int? width = null)
	{
		var available = Math.Max(10, width ?? GetSafeConsoleWidth()) - 1;
		var lines = text.Replace("\r", "").Replace("\t", "  ").Trim().Split('\n');
		for (var i = 0; i < lines.Length; i++)
		{
			var options = lines.Skip(i).TakeWhile(x => Regex.IsMatch(x, @"^\s*#[Bb].+#[Ff]:")).ToList();
			if (options.Count >= 4 && available >= 119)
			{
				var columnWidth = (available - 3) / 2;
				var rows = (options.Count + 1) / 2;
				var columns = new List<string>();
				for (var row = 0; row < rows; row++)
				{
					var left = WrapMarkup(options[row].Trim(), columnWidth).ToList();
					var right = row + rows < options.Count
						? WrapMarkup(options[row + rows].Trim(), columnWidth).ToList()
						: new List<string>();
					for (var line = 0; line < Math.Max(left.Count, right.Count); line++)
					{
						var first = left.ElementAtOrDefault(line) ?? "";
						columns.Add(first + new string(' ', columnWidth - MarkupLength(first) + 3) +
						            (right.ElementAtOrDefault(line) ?? ""));
					}
				}
				var singleColumn = options.SelectMany(x => WrapMarkup(x, available)).ToList();
				foreach (var line in columns.Count < singleColumn.Count ? columns : singleColumn)
				{
					yield return line;
				}
				i += options.Count - 1;
				continue;
			}

			foreach (var line in WrapMarkup(lines[i], available))
			{
				yield return line;
			}
		}
	}

	private static int MarkupLength(string text) => Regex.Replace(text, @"#[a-fA-F0-9]", "").Length;

	private static IEnumerable<string> WrapMarkup(string text, int width)
	{
		while (MarkupLength(text) > width)
		{
			var visible = 0;
			var end = 0;
			var space = -1;
			while (end < text.Length && visible < width)
			{
				if (text[end] == '#' && end + 1 < text.Length && Uri.IsHexDigit(text[end + 1]))
				{
					end += 2;
					continue;
				}
				if (char.IsWhiteSpace(text[end])) space = end;
				end++;
				visible++;
			}
			if (space > 0) end = space;
			yield return text[..end].TrimEnd();
			text = text[end..].TrimStart();
		}
		yield return text;
	}

	public static void WritePrompt(string text)
	{
		var pageSize = int.MaxValue;
		if (!Console.IsInputRedirected && !Console.IsOutputRedirected)
		{
			try { pageSize = Math.Max(1, Console.WindowHeight - 5); }
			catch (System.IO.IOException) { }
		}
		var lines = FormatPrompt(text).ToList();
		for (var i = 0; i < lines.Count; i++)
		{
			if (i > 0 && i % pageSize == 0)
			{
				Console.Write("-- More: press any key --");
				Console.ReadKey(intercept: true);
				Console.WriteLine();
			}
			lines[i].WriteLineConsole();
		}
	}
}
