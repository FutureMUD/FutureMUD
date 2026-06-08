#nullable enable

namespace MudSharp.Arenas;

internal static class ArenaRatingLimits
{
	internal const decimal MinimumRating = 0.0m;
	internal const decimal MaximumRating = 10000.0m;
	internal const decimal MaximumEloKFactor = 200.0m;

	internal static decimal ClampRating(decimal rating)
	{
		if (rating < MinimumRating)
		{
			return MinimumRating;
		}

		return rating > MaximumRating ? MaximumRating : rating;
	}

	internal static decimal ClampKFactor(decimal kFactor)
	{
		if (kFactor <= 0.0m)
		{
			return 32.0m;
		}

		return kFactor > MaximumEloKFactor ? MaximumEloKFactor : kFactor;
	}

	internal static decimal AddAndClampRating(decimal rating, decimal delta)
	{
		decimal current = ClampRating(rating);
		if (delta >= 0.0m && delta > MaximumRating - current)
		{
			return MaximumRating;
		}

		if (delta < 0.0m && delta < MinimumRating - current)
		{
			return MinimumRating;
		}

		return ClampRating(current + delta);
	}
}
