#nullable enable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.TimeAndDate;
using System;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AstronomicalSearchSecurityTests
{
	[DataTestMethod]
	[DataRow(-1)]
	[DataRow(0)]
	[DataRow(33)]
	[DataRow(int.MaxValue)]
	public void TryFindNext_InvalidOccurrence_RejectsBeforeSampling(int occurrence)
	{
		var ephemeris = new Mock<ICelestialEphemeris>(MockBehavior.Strict);
		Assert.IsFalse(AstronomicalEventService.Instance.TryFindNext(AstronomicalEventType.Sunrise,
			new MudInstant(MudInstant.CurrentEpoch, 0), occurrence, ephemeris.Object,
			new GeographicCoordinate(0, 0, 0, 0), out var instant, out var error));
		Assert.IsTrue(instant.IsNever);
		StringAssert.Contains(error, "between 1 and 32");
		ephemeris.VerifyNoOtherCalls();
	}

	[DataTestMethod]
	[DataRow(33)]
	[DataRow(int.MaxValue)]
	public void FindNextForCelestial_ExcessivePhysicalOccurrence_ReturnsInvalidRequestBeforeSampling(int occurrence)
	{
		var celestial = new Mock<ICelestialObject>(MockBehavior.Strict);
		var ephemeris = celestial.As<ICelestialEphemeris>();
		var result = AstronomicalEventService.Instance.FindNextForCelestial(
			new MudInstant(MudInstant.CurrentEpoch, 0), new(AstronomicalEventType.Sunrise, occurrence),
			celestial.Object, new GeographicCoordinate(0, 0, 0, 0));
		Assert.AreEqual(CelestialEventStatus.InvalidRequest, result.Status);
		Assert.IsTrue(result.Instant.IsNever);
		ephemeris.VerifyNoOtherCalls();
		celestial.VerifyNoOtherCalls();
	}

	[TestMethod]
	public void TryFindNext_SlowRepeatedSunrisesOrNestedCrescents_SharesSamplingBudget()
	{
		foreach (var eventType in new[] { AstronomicalEventType.Sunrise, AstronomicalEventType.VisibleCrescent })
		{
			var samples = 0;
			var sun = new Mock<ISolarEphemeris>();
			var moon = new Mock<ILunarEphemeris>();
			sun.Setup(x => x.ApparentAltitudeAt(It.IsAny<MudInstant>(), It.IsAny<GeographicCoordinate>()))
				.Returns((MudInstant instant, GeographicCoordinate _) =>
				{
					samples++;
					return Math.Cos(2 * Math.PI * instant.Ticks / (800 * 86400.0));
				});
			sun.Setup(x => x.RightAscensionAt(It.IsAny<MudInstant>())).Returns(() => { samples++; return 0; });
			sun.Setup(x => x.DeclinationAt(It.IsAny<MudInstant>())).Returns(() => { samples++; return 0; });
			moon.Setup(x => x.ApparentAltitudeAt(It.IsAny<MudInstant>(), It.IsAny<GeographicCoordinate>()))
				.Returns(() => { samples++; return 0; });
			moon.Setup(x => x.RightAscensionAt(It.IsAny<MudInstant>())).Returns(() => { samples++; return 1; });
			moon.Setup(x => x.DeclinationAt(It.IsAny<MudInstant>())).Returns(() => { samples++; return 0; });

			Assert.IsFalse(AstronomicalEventService.Instance.TryFindNext(eventType,
				new MudInstant(MudInstant.CurrentEpoch, 0), 32, sun.Object,
				new GeographicCoordinate(0, 0, 0, 0), out var result, out var error, secondary: moon.Object));
			Assert.IsTrue(result.IsNever);
			StringAssert.Contains(error, "sampling budget");
			Assert.AreEqual(AstronomicalEventService.MaximumPhysicalEventSamples, samples, eventType.ToString());
		}
	}

	[TestMethod]
	public void TryFindNext_MaximumPhysicalOccurrence_ResolvesSunrise()
	{
		var context = CelestialTestFactory.CreateEarthSystem();
		context.SetDateTime("1/jan/2000", 12, 0, 0);
		var reference = MudInstant.FromLegacyState(context.Calendar, context.Clock);
		Assert.IsTrue(AstronomicalEventService.Instance.TryFindNext(AstronomicalEventType.Sunrise,
			reference, AstronomicalEventService.MaximumPhysicalEventOccurrence, context.Sun,
			context.ZeroGeography, out var instant, out var error), error);
		Assert.IsTrue(instant > reference);
		Assert.IsTrue(instant.Ticks - reference.Ticks > 30 * 86400L);
	}
}
