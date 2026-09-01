#nullable enable

namespace MudSharp.GameItems.Components;

public static class AccessCredentialUtilities
{
	public const int MaximumCodes = 64;
	public const int MaximumCodeLength = 64;

	public static bool TryNormaliseCode(string? code, out string value, out string error)
	{
		value = code?.Trim() ?? string.Empty;
		if (value.Length is < 1 or > MaximumCodeLength)
		{
			error = $"Access codes must be between 1 and {MaximumCodeLength:N0} characters long.";
			return false;
		}
		if (value.Any(char.IsControl))
		{
			error = "Access codes cannot contain control characters.";
			return false;
		}
		error = string.Empty;
		return true;
	}

	public static IEnumerable<string> NormaliseCodes(IEnumerable<string> codes)
	{
		return codes
			.Select(x => TryNormaliseCode(x, out var value, out _) ? value : null)
			.Where(x => x is not null)
			.Cast<string>()
			.Distinct(StringComparer.Ordinal)
			.Take(MaximumCodes);
	}
}
