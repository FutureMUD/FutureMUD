#nullable enable

using ExpressionEngine;

namespace MudSharp.GameItems;

internal static class ToolDurabilityExpressionHelper
{
	public static bool TryValidate(IExpression expression, out string error)
	{
		foreach (var quality in Enum.GetValues<ItemQuality>())
		{
			double seconds;
			try
			{
				seconds = expression.EvaluateDoubleWith(("quality", (int)quality));
			}
			catch (Exception ex)
			{
				error = $"The expression fails for {quality.Describe().ColourValue()} quality: {ex.Message.ColourError()}";
				return false;
			}

			if (!IsValidDurationSeconds(seconds))
			{
				error =
					$"The expression must produce a finite, non-negative duration no greater than {TimeSpan.MaxValue.TotalSeconds.ToString("N0").ColourValue()} seconds for every item quality. It produced {seconds.ToString("G17").ColourError()} for {quality.Describe().ColourValue()} quality.";
				return false;
			}
		}

		error = string.Empty;
		return true;
	}

	public static TimeSpan EvaluateDuration(IExpression expression, ItemQuality quality)
	{
		try
		{
			var seconds = expression.EvaluateDoubleWith(("quality", (int)quality));
			return IsValidDurationSeconds(seconds)
				? TimeSpan.FromSeconds(seconds)
				: TimeSpan.Zero;
		}
		catch
		{
			return TimeSpan.Zero;
		}
	}

	private static bool IsValidDurationSeconds(double seconds)
	{
		return double.IsFinite(seconds) &&
		       seconds >= 0.0 &&
		       seconds <= TimeSpan.MaxValue.TotalSeconds;
	}
}
