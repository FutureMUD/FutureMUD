#nullable enable

namespace MudSharp.Framework;

internal static class SpreadsheetSafeCsv
{
	public static string EncodeRoom(string? value)
	{
		return EncodeCell(value);
	}

	public static string EncodeCell(string? value)
	{
		var text = value ?? string.Empty;
		var trimmed = text.AsSpan().TrimStart();
		if (text.Length > 0 &&
		    (text[0].In('=', '+', '-', '@', '\t', '\r', '\n') ||
		     (trimmed.Length > 0 && trimmed[0].In('=', '+', '-', '@'))))
		{
			text = $"'{text}";
		}

		return $"\"{text.Replace("\"", "\"\"")}\"";
	}
}
