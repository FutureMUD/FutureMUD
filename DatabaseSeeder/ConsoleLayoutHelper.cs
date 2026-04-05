using MudSharp.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseSeeder;

internal static class ConsoleLayoutHelper
{
    private const int MinimumWidth = 40;
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
        int availableTaglineWidth = Math.Max(10, effectiveWidth - prefix.Length);
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
}
