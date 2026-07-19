#nullable enable


namespace MudSharp.Magic;

public static class MagicBuilderValidation
{
	public const int MaximumForcedPathMovementSteps = 100;
	public const double MaximumSpellTickSeconds = 86_400.0;
	public const double MaximumNeedDeltaHours = 1_000.0;
	public const double MaximumNeedDeltaAlcoholLitres = 25.0;
	public const double MaximumResistanceIntervalSeconds = 86_400.0;

	public static bool TryParseFiniteDouble(string text, out double value)
	{
		return double.TryParse(text, out value) && double.IsFinite(value);
	}

	public static bool TryParseFiniteDoubleInRange(string text, double minimum, double maximum, out double value)
	{
		return TryParseFiniteDouble(text, out value) && value >= minimum && value <= maximum;
	}

	public static double ClampFinite(double value, double minimum, double maximum, double defaultValue)
	{
		if (!double.IsFinite(value))
		{
			return defaultValue;
		}

		return Math.Clamp(value, minimum, maximum);
	}

	public static double ParseFiniteOrDefault(string? text, double defaultValue)
	{
		return text is not null && TryParseFiniteDouble(text, out var value) ? value : defaultValue;
	}
}
