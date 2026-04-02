#nullable enable

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlanetaryMoonTests
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
		MoonPhase expectedPhase)
	{
		var context = CelestialTestFactory.CreateEarthSystem();

		context.SetDateTime(date, hour, minute, second);

		var dayNumber = context.Moon.CurrentDayNumber;
		var (rightAscension, declination) = context.Moon.EquatorialCoordinates(dayNumber);
		var position = context.Moon.CurrentPosition(context.DefaultGeography);

		Assert.AreEqual(expectedDayNumber, dayNumber, CelestialTestAssertions.DayNumberTolerance);
		Assert.AreEqual(expectedRightAscension, rightAscension, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedDeclination, declination, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedElevation, position.LastAscensionAngle, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedAzimuth, position.LastAzimuthAngle, CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(expectedIllumination, context.Moon.CurrentIllumination(context.DefaultGeography), CelestialTestAssertions.UnitTolerance);
		Assert.AreEqual(expectedDirection, position.Direction);
		Assert.AreEqual(expectedPhase, context.Moon.CurrentPhase());
	}

	[TestMethod]
	public void StockFixture_UsesParentSiderealRotationModel()
	{
		var context = CelestialTestFactory.CreateEarthSystem();

		Assert.AreEqual(
			context.Sun.SiderealTimeAtEpoch,
			context.Moon.SiderealTimeAtEpoch,
			CelestialTestAssertions.AngleTolerance);
		Assert.AreEqual(
			context.Sun.SiderealTimePerDay,
			context.Moon.SiderealTimePerDay,
			CelestialTestAssertions.AngleTolerance);
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
				var current = context.Moon.CurrentPosition(context.DefaultGeography);

				context.Clock.CurrentTime.SetTime((minute - 1) / 10, (minute - 1) % 10, 0);
				var previous = context.Moon.CurrentPosition(context.DefaultGeography);

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
			0,
			0,
			0,
			2451545.0,
			3.882011255715576,
			0.08906039423900632,
			0.46850330190429756,
			1.518566877769967,
			1.0,
			CelestialMoveDirection.Descending,
			MoonPhase.Full
		];

		yield return
		[
			"28/jan/2000",
			0,
			0,
			0,
			2451552.0,
			5.327973382034617,
			-0.0003548307866201067,
			1.2015701042547373,
			-0.6428293027778917,
			0.5406626989767713,
			CelestialMoveDirection.Ascending,
			MoonPhase.LastQuarter
		];

		yield return
		[
			"5/feb/2000",
			0,
			0,
			0,
			2451560.0,
			0.8893229296838694,
			-0.08637756117244345,
			-0.36354906494262806,
			-1.5918696431200925,
			0.0006233237637223721,
			CelestialMoveDirection.Ascending,
			MoonPhase.New
		];
	}
}
