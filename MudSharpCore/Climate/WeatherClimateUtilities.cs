using MudSharp.Framework;
using System;

#nullable enable
#nullable disable warnings

namespace MudSharp.Climate;

internal static class WeatherClimateUtilities
{
    public static double ApplySeasonPhaseShift(double celestialDay, double yearLengthDays, bool oppositeHemisphere)
    {
        if (!oppositeHemisphere || yearLengthDays <= 0.0)
        {
            return celestialDay;
        }

        double shifted = celestialDay + yearLengthDays / 2.0;
        while (shifted >= yearLengthDays)
        {
            shifted -= yearLengthDays;
        }

        while (shifted < 0.0)
        {
            shifted += yearLengthDays;
        }

        return shifted;
    }

    public static double AdvanceTemperatureFluctuation(
        double currentOffset,
        double standardDeviation,
        TimeSpan period,
        int tickMinutes,
        Func<double>? nextRandom = null)
    {
        if (standardDeviation <= 0.0 || period <= TimeSpan.Zero || tickMinutes <= 0)
        {
            return 0.0;
        }

        nextRandom ??= Constants.Random.NextDouble;

        double effectivePeriodMinutes = Math.Max(period.TotalMinutes, tickMinutes);
        double baseStep = standardDeviation * 0.45 * Math.Sqrt(tickMinutes / effectivePeriodMinutes);
        double step = baseStep * (0.75 + nextRandom() * 0.5);
        double normalizedOffset = currentOffset / standardDeviation;

        double direction;
        if (Math.Abs(currentOffset) < 0.0001)
        {
            direction = nextRandom() < 0.5 ? -1.0 : 1.0;
        }
        else
        {
            double towardMeanChance = ApproximateNormalCdf(Math.Abs(normalizedOffset) / 2.5);
            double towardMeanDirection = currentOffset > 0.0 ? -1.0 : 1.0;
            direction = nextRandom() < towardMeanChance
                ? towardMeanDirection
                : -towardMeanDirection;
        }

        return Math.Clamp(currentOffset + step * direction, -standardDeviation * 3.5, standardDeviation * 3.5);
    }

    private static double ApproximateNormalCdf(double value)
    {
        return 0.5 * (1.0 + ApproximateErf(value / Math.Sqrt(2.0)));
    }

    private static double ApproximateErf(double value)
    {
        double sign = value < 0.0 ? -1.0 : 1.0;
        value = Math.Abs(value);
        double t = 1.0 / (1.0 + 0.3275911 * value);
        double polynomial = (((((1.061405429 * t) - 1.453152027) * t) + 1.421413741) * t - 0.284496736) * t + 0.254829592;
        double y = 1.0 - polynomial * t * Math.Exp(-(value * value));
        return sign * y;
    }
}
