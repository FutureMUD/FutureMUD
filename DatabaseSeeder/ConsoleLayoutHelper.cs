using System;
using System.Collections.Generic;
using System.Linq;
using MudSharp.Framework;

namespace DatabaseSeeder;

internal static class ConsoleLayoutHelper
{
	private const int MinimumWidth = 40;
	private const int DefaultWidth = 100;

	public static int GetSafeConsoleWidth()
	{
		try
		{
			var width = Console.WindowWidth;
			return width >= MinimumWidth ? width : DefaultWidth;
		}
		catch
		{
			return DefaultWidth;
		}
	}

	public static IEnumerable<string> WrapToConsoleLines(string text, int? width = null, string indent = "")
	{
		var effectiveWidth = Math.Max(MinimumWidth, width ?? GetSafeConsoleWidth());
		var wrapped = text.Wrap(Math.Max(1, effectiveWidth - 1), indent);
		return wrapped.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
	}

	public static IEnumerable<string> FormatMenuEntry(int index, string name, string status, string tagline, int? width = null)
	{
		var prefix = $"{index}) [{name,-20}] [{status,-8}] ";
		var continuationIndent = new string(' ', prefix.Length);
		var effectiveWidth = Math.Max(MinimumWidth, width ?? GetSafeConsoleWidth());
		var availableTaglineWidth = Math.Max(10, effectiveWidth - prefix.Length);
		var wrappedTagline = tagline.Wrap(availableTaglineWidth);
		var lines = wrappedTagline.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).ToList();
		if (!lines.Any() || string.IsNullOrWhiteSpace(lines[0]))
		{
			yield return prefix.TrimEnd();
			yield break;
		}

		yield return prefix + lines[0];
		foreach (var line in lines.Skip(1))
		{
			yield return continuationIndent + line;
		}
	}

	public static void WriteWrapped(string text, int? width = null, string indent = "")
	{
		foreach (var line in WrapToConsoleLines(text, width, indent))
		{
			Console.WriteLine(line);
		}
	}
}
