#nullable enable

namespace MudSharp.Framework;

internal static class SpreadsheetSafeCsv
{
	public static string EncodeCell(string? value)
	{
		var text = value ?? string.Empty;
		if (text.Length > 0 && text[0].In('=', '+', '-', '@', '\t', '\r', '\n'))
		{
			text = $"'{text}";
		}

		return $"\"{text.Replace("\"", "\"\"")}\"";
	}
}
