#nullable enable

using MudSharp.Models;
using System;

namespace DatabaseSeeder.Seeders;

internal static class SatiationLimitSeederHelper
{
	private const double FullySatedThresholdFraction = 0.75;
	private const double Tolerance = 0.0001;

	internal static double MaximumFoodHoursForCadence(double hoursUntilStarvingFromFullyFed)
	{
		return hoursUntilStarvingFromFullyFed / FullySatedThresholdFraction;
	}

	internal static double MaximumDrinkHoursForCadence(double hoursUntilParchedFromFullySated)
	{
		return hoursUntilParchedFromFullySated / FullySatedThresholdFraction;
	}

	internal static (double MaximumFoodSatiatedHours, double MaximumDrinkSatiatedHours) MaximumLimitsForCadence(
		double hoursUntilStarvingFromFullyFed,
		double hoursUntilParchedFromFullySated)
	{
		return (
			MaximumFoodHoursForCadence(hoursUntilStarvingFromFullyFed),
			MaximumDrinkHoursForCadence(hoursUntilParchedFromFullySated)
		);
	}

	internal static bool MatchesLimits(Race race, double maximumFoodSatiatedHours, double maximumDrinkSatiatedHours)
	{
		return Math.Abs(race.MaximumFoodSatiatedHours - maximumFoodSatiatedHours) < Tolerance &&
		       Math.Abs(race.MaximumDrinkSatiatedHours - maximumDrinkSatiatedHours) < Tolerance;
	}

	internal static bool ApplyLimits(Race race, double maximumFoodSatiatedHours, double maximumDrinkSatiatedHours)
	{
		if (MatchesLimits(race, maximumFoodSatiatedHours, maximumDrinkSatiatedHours))
		{
			return false;
		}

		race.MaximumFoodSatiatedHours = maximumFoodSatiatedHours;
		race.MaximumDrinkSatiatedHours = maximumDrinkSatiatedHours;
		return true;
	}
}
