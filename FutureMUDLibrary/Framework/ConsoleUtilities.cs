using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace MudSharp.Framework;

public static class ConsoleUtilities
{
	private static readonly Regex CodeRegex = new("^(?<before>.*?)\\#(?<code>[a-f0-9])(?<after>.*)$",
		RegexOptions.IgnoreCase | RegexOptions.Singleline);

	[StringFormatMethod("text")]
	public static void WriteLine(string text, params object[] args)
	{
		string.Format(text, args).WriteLineConsole();
	}

	public static void WriteLineConsole(this string text)
	{
		var match = CodeRegex.Match(text);
		do
		{
			if (!match.Success)
			{
				Console.WriteLine(text);
				return;
			}

			Console.Write(match.Groups["before"].Value);
			switch (match.Groups["code"].Value.ToLower())
			{
				case "0":
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case "1":
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case "2":
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					break;
				case "3":
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					break;
				case "4":
					Console.ForegroundColor = ConsoleColor.DarkBlue;
					break;
				case "5":
					Console.ForegroundColor = ConsoleColor.DarkMagenta;
					break;
				case "6":
					Console.ForegroundColor = ConsoleColor.DarkCyan;
					break;
				case "7":
					Console.ForegroundColor = ConsoleColor.Black;
					break;
				case "8":
					Console.ForegroundColor = ConsoleColor.Gray;
					break;
				case "9":
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case "a":
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case "b":
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case "c":
					Console.ForegroundColor = ConsoleColor.Blue;
					break;
				case "d":
					Console.ForegroundColor = ConsoleColor.Magenta;
					break;
				case "e":
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case "f":
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}

			text = match.Groups["after"].Value;
			match = CodeRegex.Match(text);
		} while (true);
	}
}