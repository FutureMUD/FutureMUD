#nullable enable
using System.Globalization;

namespace MudSharp.Framework;

internal static class ExternalIntegrationAlertHelper
{
	private const int DefaultMaximumDiscordAlertLength = 3500;

	public static string SanitiseDiscordAdminAlert(string? text, int maximumLength = DefaultMaximumDiscordAlertLength)
	{
		if (maximumLength <= 0)
		{
			return string.Empty;
		}

		var cleaned = text
			.IfNullOrWhiteSpace("(no details supplied)")
			.StripANSIColour()
			.StripMXP()
			.Replace("\r\n", "\n")
			.Replace('\r', '\n')
			.Replace("```", "'''", StringComparison.Ordinal)
			.Replace("@everyone", "@\u200beveryone", StringComparison.OrdinalIgnoreCase)
			.Replace("@here", "@\u200bhere", StringComparison.OrdinalIgnoreCase)
			.Replace("<@", "<@\u200b", StringComparison.Ordinal);

		if (cleaned.Length <= maximumLength)
		{
			return cleaned;
		}

		const string suffix = "\n[Alert truncated.]";
		var retainedLength = Math.Max(0, maximumLength - suffix.Length);
		return $"{cleaned[..retainedLength]}{suffix}";
	}

	public static string BuildSafeGptErrorAlert(Exception exception, string context)
	{
		var safeContext = SanitiseDiscordAdminAlert(context.IfNullOrWhiteSpace("GPT request"), 200);
		var safeType = SanitiseDiscordAdminAlert(exception.GetType().Name, 120);
		return $"**GPT Error**\n\n{safeContext} failed with {safeType}. Full exception details were written to the server console.";
	}

	public static string SummariseSensitivePayload(string? payload)
	{
		var cleaned = SanitiseDiscordAdminAlert(payload, int.MaxValue);
		return string.Format(CultureInfo.InvariantCulture,
			"[Sensitive AI payload omitted from debug output; {0:N0} characters after sanitisation.]",
			cleaned.Length);
	}
}
