#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Construction;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class CelestialTests
{
    [TestMethod]
    public void CurrentDayNumber_CustomOffsetShiftsComputedDayNumber()
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();
        NewSun offsetSun = context.CreateSunWithDayOffset(0.75);

        context.SetDateTime("1/jan/2000", 12, 0, 0);

        Assert.AreEqual(
            context.Sun.CurrentDayNumber + 0.25,
            offsetSun.CurrentDayNumber,
            CelestialTestAssertions.DayNumberTolerance);
    }

    [DataTestMethod]
    [DynamicData(nameof(CurrentPositionRegressionCases), DynamicDataSourceType.Method)]
    public void CurrentPosition_RegressionMatchesExpectedValues(
        string date,
        int hour,
        int minute,
        int second,
        double latitude,
        double longitude,
        double expectedDayNumber,
        double expectedRightAscension,
        double expectedDeclination,
        double expectedElevation,
        double expectedAzimuth,
        double expectedIllumination,
        CelestialMoveDirection expectedDirection,
        TimeOfDay expectedTimeOfDay)
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();
        GeographicCoordinate geography = new(latitude, longitude, 0.0, 0.0);

        context.SetDateTime(date, hour, minute, second);

        double dayNumber = context.Sun.CurrentDayNumber;
        CelestialInformation position = context.Sun.CurrentPosition(geography);

        Assert.AreEqual(expectedDayNumber, dayNumber, CelestialTestAssertions.DayNumberTolerance);
        Assert.AreEqual(
            expectedRightAscension,
            context.Sun.RightAscension(dayNumber),
            CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(
            expectedDeclination,
            context.Sun.Declension(dayNumber),
            CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(
            expectedElevation,
            position.LastAscensionAngle,
            CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(
            expectedAzimuth,
            position.LastAzimuthAngle,
            CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(
            expectedIllumination,
            context.Sun.CurrentIllumination(geography),
            CelestialTestAssertions.LuxTolerance);
        Assert.AreEqual(expectedDirection, position.Direction);
        Assert.AreEqual(expectedTimeOfDay, context.Sun.CurrentTimeOfDay(geography));
    }

    [TestMethod]
    public void CurrentTimeOfDay_TwilightThresholdsMatchCorrectedClassification()
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();

        context.SetDateTime("20/mar/2000", 18, 30, 0);
        Assert.AreEqual(TimeOfDay.Dusk, context.Sun.CurrentTimeOfDay(context.ZeroGeography));

        context.SetDateTime("20/mar/2000", 19, 0, 0);
        Assert.AreEqual(TimeOfDay.Night, context.Sun.CurrentTimeOfDay(context.ZeroGeography));
    }

    [TestMethod]
    public void CurrentPosition_NonStandardClockUsesActualClockLengthForDirectionSampling()
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();
        int originalHoursPerDay = context.Clock.HoursPerDay;
        int originalMinutesPerHour = context.Clock.MinutesPerHour;

        try
        {
            CelestialTestAssertions.SetClockDimensions(context.Clock, 2, 10);
            context.Calendar.SetDate("20/mar/2000");

            for (int minute = 1; minute < 20; minute++)
            {
                context.Clock.CurrentTime.SetTime(minute / 10, minute % 10, 0);
                CelestialInformation current = context.Sun.CurrentPosition(context.DefaultGeography);

                context.Clock.CurrentTime.SetTime((minute - 1) / 10, (minute - 1) % 10, 0);
                CelestialInformation previous = context.Sun.CurrentPosition(context.DefaultGeography);

                CelestialMoveDirection expectedDirection = current.LastAscensionAngle >= previous.LastAscensionAngle
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
            "1/jan/2000",
            12,
            0,
            0,
            0.0,
            0.0,
            2451546.0,
            -1.3544400675617951,
            -0.4005532294375676,
            1.1696691699436135,
            -0.05203232556097422,
            99969.03957501706,
            CelestialMoveDirection.Ascending,
            TimeOfDay.Morning
        ];

        yield return
        [
            "20/mar/2000",
            6,
            0,
            0,
            0.0,
            0.0,
            2451623.75,
            -0.0009115616334453687,
            -0.00039521047268252296,
            -0.03807368296376589,
            -1.5704008296994763,
            635.5987177154168,
            CelestialMoveDirection.Ascending,
            TimeOfDay.Dawn
        ];

        yield return
        [
            "20/mar/2000",
            18,
            0,
            0,
            0.0,
            0.0,
            2451624.25,
            0.007041342111377198,
            0.0030527623500603094,
            0.03742506880220377,
            1.573851228304077,
            1645.8246208887333,
            CelestialMoveDirection.Descending,
            TimeOfDay.Afternoon
        ];
    }
}
