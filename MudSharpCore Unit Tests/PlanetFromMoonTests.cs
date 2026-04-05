#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using MudSharp.Celestial;
using MudSharp.Construction;
using MudSharp.Framework;
using System;
using System.Collections.Generic;

namespace MudSharp_Unit_Tests;

[TestClass]
public class PlanetFromMoonTests
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
        MoonPhase expectedPhase,
        bool expectedEclipse)
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();

        context.SetDateTime(date, hour, minute, second);

        double dayNumber = context.Planet.CurrentDayNumber;
        (double rightAscension, double declination) = context.Planet.EquatorialCoordinates(dayNumber);
        (double moonRightAscension, double moonDeclination) = context.Moon.EquatorialCoordinates(dayNumber);
        CelestialInformation position = context.Planet.CurrentPosition(context.DefaultGeography);
        double moonIllumination = context.Moon.CurrentIllumination(context.DefaultGeography);
        double planetIllumination = context.Planet.CurrentIllumination(context.DefaultGeography);

        Assert.AreEqual(expectedDayNumber, dayNumber, CelestialTestAssertions.DayNumberTolerance);
        Assert.AreEqual(expectedRightAscension, rightAscension, CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(expectedDeclination, declination, CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(expectedElevation, position.LastAscensionAngle, CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(expectedAzimuth, position.LastAzimuthAngle, CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(expectedIllumination, planetIllumination, CelestialTestAssertions.UnitTolerance);
        Assert.AreEqual(expectedDirection, position.Direction);
        Assert.AreEqual(expectedPhase, context.Planet.CurrentPhase());
        Assert.AreEqual(expectedEclipse, context.Planet.IsSunEclipsed(context.DefaultGeography));

        Assert.AreEqual(
            (moonRightAscension + Math.PI).Modulus(2 * Math.PI),
            rightAscension,
            CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(-moonDeclination, declination, CelestialTestAssertions.AngleTolerance);
        Assert.AreEqual(1.0, moonIllumination + planetIllumination, CelestialTestAssertions.UnitTolerance);
    }

    [TestMethod]
    public void IsSunEclipsed_UsesSimpleDiscOverlapThreshold()
    {
        CelestialTestContext context = CelestialTestFactory.CreateEarthSystem();
        const double separationCaseAngularRadius = 0.2150304839745495;
        const double belowThresholdAngularRadius = 0.2117030874745495;

        context.SetDateTime("24/feb/2000", 23, 0, 0);

        Assert.IsFalse(context.Planet.IsSunEclipsed(context.ZeroGeography));

        PlanetFromMoon overlapPlanet = new(context.Moon, context.Sun)
        {
            PeakIllumination = 1.0,
            AngularRadius = separationCaseAngularRadius,
            SunAngularRadius = context.Sun.ApparentAngularRadius
        };
        PlanetFromMoon nonOverlapPlanet = new(context.Moon, context.Sun)
        {
            PeakIllumination = 1.0,
            AngularRadius = belowThresholdAngularRadius,
            SunAngularRadius = context.Sun.ApparentAngularRadius
        };

        Assert.IsTrue(overlapPlanet.IsSunEclipsed(context.ZeroGeography));
        Assert.IsFalse(nonOverlapPlanet.IsSunEclipsed(context.ZeroGeography));
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
            context.Calendar.SetDate("21/jan/2000");

            for (int minute = 1; minute < 20; minute++)
            {
                context.Clock.CurrentTime.SetTime(minute / 10, minute % 10, 0);
                CelestialInformation current = context.Planet.CurrentPosition(context.DefaultGeography);

                context.Clock.CurrentTime.SetTime((minute - 1) / 10, (minute - 1) % 10, 0);
                CelestialInformation previous = context.Planet.CurrentPosition(context.DefaultGeography);

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
            "21/jan/2000",
            0,
            0,
            0,
            2451545.0,
            0.740418602125783,
            -0.08906039423900632,
            -0.4685033019042977,
            -1.6230257758198263,
            0.0,
            CelestialMoveDirection.Ascending,
            MoonPhase.New,
            false
        ];

        yield return
        [
            "28/jan/2000",
            0,
            0,
            0,
            2451552.0,
            2.1863807284448242,
            0.0003548307866201067,
            -1.2015701042547373,
            2.498763350811901,
            0.45933730102322884,
            CelestialMoveDirection.Descending,
            MoonPhase.FirstQuarter,
            false
        ];

        yield return
        [
            "5/feb/2000",
            0,
            0,
            0,
            2451560.0,
            4.0309155832736625,
            0.08637756117244345,
            0.36354906494262795,
            1.5497230104697006,
            0.9993766762362777,
            CelestialMoveDirection.Descending,
            MoonPhase.Full,
            false
        ];
    }
}
