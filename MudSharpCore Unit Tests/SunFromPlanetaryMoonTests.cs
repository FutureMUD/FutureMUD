#nullable enable

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Construction;

namespace MudSharp_Unit_Tests;

[TestClass]
public class SunFromPlanetaryMoonTests
{
	[DataTestMethod]
	[DynamicData(nameof(CurrentPositionRegressionCases), DynamicDataSourceType.Method)]
	public void CurrentPosition_RegressionMatchesExpectedValues(
		string date,
		int hour,
		int minute,
		int second,
		double expectedDayNumber,
		double expectedRightAscension,
		double expectedDeclination,
		double expectedElevation,
		double expectedAzimuth,
		double expectedIllumination,
		CelestialMoveDirection expectedDirection,
		TimeOfDay expectedTimeOfDay)
	{
		var context = CelestialTestFactory.CreateEarthSystem();

		context.SetDateTime(date, hour, minute, second);

		var dayNumber = context.SunFromMoon.CurrentDayNumber;
		var (rightAscension, declination) =
			CelestialTestAssertions.SunFromMoonEquatorialCoordinates(context.SunFromMoon, dayNumber);
		var position = context.SunFromMoon.CurrentPosition(context.ZeroGeography);

		Assert.AreEqual(expectedDayNumber, dayNumber, CelestialTestAssertions.DayNumberTolerance);
		Assert.AreEqual(expectedRightAscension, rightAscension, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedDeclination, declination, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedElevation, position.LastAscensionAngle, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedAzimuth, position.LastAzimuthAngle, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedIllumination, context.SunFromMoon.CurrentIllumination(context.ZeroGeography), CelestialTestAssertions.LuxTolerance);
		Assert.AreEqual(expectedDirection, position.Direction);
		Assert.AreEqual(expectedTimeOfDay, context.SunFromMoon.CurrentTimeOfDay(context.ZeroGeography));
	}

	[TestMethod]
	public void CurrentPosition_ConjunctionCaseUsesRelativePositionAndRemainsStable()
	{
		var context = CelestialTestFactory.CreateEarthSystem();

		context.SetDateTime("5/feb/2000", 0, 0, 0);

		var dayNumber = context.SunFromMoon.CurrentDayNumber;
		var (rightAscension, declination) =
			CelestialTestAssertions.SunFromMoonEquatorialCoordinates(context.SunFromMoon, dayNumber);
		var rootRightAscension = context.Sun.RightAscension(dayNumber);
		var rootDeclination = context.Sun.Declension(dayNumber);

		Assert.AreEqual(2451580.5, dayNumber, CelestialTestAssertions.DayNumberTolerance);
		Assert.IsFalse(double.IsNaN(rightAscension));
		Assert.IsFalse(double.IsNaN(declination));
		Assert.AreEqual(5.566981301403694, rightAscension, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(-0.2780275545561409, declination, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(
			0.00010881460650172357,
			CelestialTestAssertions.WrappedAbsoluteDifference(rightAscension, rootRightAscension),
			CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(
			0.0006893531352700427,
			Math.Abs(declination - rootDeclination),
			CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(TimeOfDay.Night, context.SunFromMoon.CurrentTimeOfDay(context.DefaultGeography));
	}

	[TestMethod]
	public void CurrentPosition_NonStandardClockUsesActualClockLengthForDirectionSampling()
	{
		var context = CelestialTestFactory.CreateEarthSystem();
		var originalHoursPerDay = context.Clock.HoursPerDay;
		var originalMinutesPerHour = context.Clock.MinutesPerHour;

		try
		{
			CelestialTestAssertions.SetClockDimensions(context.Clock, 2, 10);
			context.Calendar.SetDate("21/jan/2000");

			for (var minute = 1; minute < 20; minute++)
			{
				context.Clock.CurrentTime.SetTime(minute / 10, minute % 10, 0);
				var current = context.SunFromMoon.CurrentPosition(context.DefaultGeography);

				context.Clock.CurrentTime.SetTime((minute - 1) / 10, (minute - 1) % 10, 0);
				var previous = context.SunFromMoon.CurrentPosition(context.DefaultGeography);

				var expectedDirection = current.LastAscensionAngle >= previous.LastAscensionAngle
					? CelestialMoveDirection.Ascending
					: CelestialMoveDirection.Descending;

				Assert.AreEqual(expectedDirection, current.Direction, $"Minute {minute} should use the actual clock length.");
			}
		}
		finally
		{
			CelestialTestAssertions.SetClockDimensions(context.Clock, originalHoursPerDay, originalMinutesPerHour);
			context.Clock.CurrentTime.SetTime(12, 0, 0);
		}
	}

	public static IEnumerable<object[]> CurrentPositionRegressionCases()
	{
		yield return
		[
			"21/jan/2000",
			6,
			0,
			0,
			2451565.75,
			5.302103424232795,
			-0.34514028288035736,
			-0.05237409437221521,
			-1.225162330935599,
			303.3273703333999,
			CelestialMoveDirection.Ascending,
			TimeOfDay.Dawn
		];

		yield return
		[
			"21/jan/2000",
			12,
			0,
			0,
			2451566.0,
			5.3068526003809176,
			-0.3441783767584873,
			1.2222543022806611,
			-0.1551916532451807,
			102277.36705119064,
			CelestialMoveDirection.Ascending,
			TimeOfDay.Morning
		];

		yield return
		[
			"21/jan/2000",
			19,
			0,
			0,
			2451566.2916666665,
			5.312386283637409,
			-0.34305065004429053,
			-0.19306448767813267,
			1.2209758525451642,
			7.09439956746637E-13,
			CelestialMoveDirection.Descending,
			TimeOfDay.Dusk
		];
	}
}
