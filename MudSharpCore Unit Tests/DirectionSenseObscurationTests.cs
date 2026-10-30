#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Body;
using MudSharp.Character;
using MudSharp.Climate;
using MudSharp.Construction;
using MudSharp.Effects;
using MudSharp.Effects.Concrete;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.Movement;
using MudSharp.PerceptionEngine;
using MudSharp.RPG.Checks;

namespace MudSharp_Unit_Tests;

[TestClass]
public class DirectionSenseObscurationTests
{
	[TestMethod]
	public void Immwalk_BypassesObscurationEvenWhenBlind()
	{
		var f = new Fixture();
		f.Body.SetupGet(x => x.VisionPercentage).Returns(0.0);
		f.Actor.Setup(x => x.EffectsOfType<MudSharp.Effects.Interfaces.IImmwalkEffect>(It.IsAny<Predicate<MudSharp.Effects.Interfaces.IImmwalkEffect>>()))
			.Returns([Mock.Of<MudSharp.Effects.Interfaces.IImmwalkEffect>()]);
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		f.World.Verify(x => x.GetCheck(It.IsAny<CheckType>()), Times.Never);
	}
	[TestMethod]
	[DataRow(Difficulty.Hard, false)]
	[DataRow(Difficulty.VeryHard, true)]
	[DataRow(Difficulty.Insane, true)]
	public void BuiltInTrigger_EffectiveSightSeverity_RespectsConfiguredBoundary(Difficulty difficulty, bool expected)
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Origin.Object)).Returns(difficulty);
		Assert.AreEqual(expected, DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
	}

	[TestMethod]
	[DataRow(0.7, false)]
	[DataRow(0.31, false)]
	[DataRow(0.3, true)]
	[DataRow(0.29, true)]
	public void BuiltInTrigger_DustWithoutChangedPrecipitation_UsesMultiplierBoundary(double multiplier, bool expected)
	{
		var f = new Fixture();
		f.Dust(multiplier);
		Assert.AreEqual(expected, DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		Assert.AreEqual(PrecipitationLevel.Dry, f.Weather.Object.Precipitation);
	}

	[TestMethod]
	public void BuiltInTrigger_PhysicalLossOfSight_TriggersWithAdequateLight()
	{
		var f = new Fixture();
		f.Body.Setup(x => x.CanSee(f.Origin.Object, PerceiveIgnoreFlags.None)).Returns(false);
		Assert.IsTrue(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		f.Body.Setup(x => x.CanSee(f.Origin.Object, PerceiveIgnoreFlags.None)).Returns(true);
		f.Body.SetupGet(x => x.VisionPercentage).Returns(0.0);
		Assert.IsTrue(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
	}

	[TestMethod]
	[DataRow(RoomOutdoorsType.Outdoors, true)]
	[DataRow(RoomOutdoorsType.Indoors, false)]
	[DataRow(RoomOutdoorsType.IndoorsWithWindows, false)]
	public void BuiltInTrigger_WeatherSightSeverity_RespectsRoomExposure(RoomOutdoorsType exposure, bool expected)
	{
		var f = new Fixture();
		f.Weather.SetupGet(x => x.Precipitation).Returns(PrecipitationLevel.TorrentialRain);
		f.Origin.Setup(x => x.OutdoorsType(f.Actor.Object)).Returns(exposure);
		Assert.AreEqual(expected, DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
	}

	[TestMethod]
	public void BuiltInTrigger_ConfiguredThresholdsAndEffectiveDarksight_AreUsed()
	{
		var f = new Fixture();
		f.World.Setup(x => x.GetStaticConfiguration("DirectionSenseMinimumSightDifficulty")).Returns("Insane");
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Origin.Object)).Returns(Difficulty.VeryHard);
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Origin.Object)).Returns(Difficulty.Easy);
		f.Dust(0.7);
		f.World.Setup(x => x.GetStaticDouble("DirectionSenseMaximumObscuringWeatherLightMultiplier")).Returns(0.7);
		Assert.IsTrue(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		f.Origin.Setup(x => x.OutdoorsType(f.Actor.Object)).Returns(RoomOutdoorsType.Indoors);
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
	}

	[TestMethod]
	public void IsActive_ValidFalseProgOverridesBuiltInAndMissingOverrideFallsBack()
	{
		var f = new Fixture();
		f.Actor.Setup(x => x.IlluminationSightDifficulty(f.Origin.Object)).Returns(Difficulty.Impossible);
		f.World.Setup(x => x.GetStaticLong("DirectionSenseObscurationProg")).Returns(7);
		Assert.IsTrue(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		var prog = new Mock<IFutureProg>();
		prog.SetupGet(x => x.Id).Returns(7L);
		prog.SetupGet(x => x.ReturnType).Returns(ProgVariableTypes.Boolean);
		prog.Setup(x => x.MatchesParameters(It.IsAny<IEnumerable<ProgVariableTypes>>())).Returns(true);
		prog.Setup(x => x.ExecuteBool(It.IsAny<object[]>())).Returns(false);
		f.World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg> { prog.Object });
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		f.World.Setup(x => x.GetStaticBool("DirectionSenseEnabled")).Returns(false);
		Assert.IsFalse(DirectionSenseMovement.IsActive(f.Actor.Object, f.Origin.Object));
		Assert.AreEqual("false", DefaultStaticSettings.DefaultStaticConfigurations["DirectionSenseEnabled"]);
	}

	[TestMethod]
	public void NoProgress_DefaultTenSecondDelay_AppliesToMoveAndDriveOnly()
	{
		var f = new Fixture();
		IEffect? delayed = null;
		var duration = TimeSpan.Zero;
		f.Actor.Setup(x => x.AddEffect(It.IsAny<IEffect>(), It.IsAny<TimeSpan>()))
			.Callback<IEffect, TimeSpan>((effect, time) => { delayed = effect; duration = time; });
		DirectionSenseMovement.NoProgress(f.Actor.Object);
		Assert.AreEqual(TimeSpan.FromSeconds(10), duration);
		Assert.IsInstanceOfType(delayed, typeof(CommandDelay));
		var delay = (CommandDelay)delayed!;
		Assert.IsTrue(delay.IsDelayed("Move"));
		Assert.IsTrue(delay.IsDelayed("Drive"));
		Assert.IsFalse(delay.IsDelayed("look"));
	}

	private sealed class Fixture
	{
		public Mock<ICharacter> Actor { get; } = new();
		public Mock<IFuturemud> World { get; } = new();
		public Mock<IBody> Body { get; } = new();
		public Mock<IRoom> Origin { get; } = new();
		public Mock<IWeatherEvent> Weather { get; } = new();
		public Fixture()
		{
			Actor.SetupGet(x => x.Gameworld).Returns(World.Object);
			Actor.SetupGet(x => x.Body).Returns(Body.Object);
			Actor.SetupGet(x => x.OutputHandler).Returns(Mock.Of<IOutputHandler>());
			Actor.SetupGet(x => x.QueuedMoveCommands).Returns(new Queue<string>());
			Actor.Setup(x => x.IlluminationSightDifficulty(Origin.Object)).Returns(Difficulty.Easy);
			Body.SetupGet(x => x.VisionPercentage).Returns(1.0);
			Body.Setup(x => x.CanSee(Origin.Object, PerceiveIgnoreFlags.None)).Returns(true);
			World.Setup(x => x.GetStaticBool("DirectionSenseEnabled")).Returns(true);
			World.SetupGet(x => x.FutureProgs).Returns(new All<IFutureProg>());
			World.Setup(x => x.GetStaticConfiguration(It.IsAny<string>())).Returns<string>(name =>
				DefaultStaticSettings.DefaultStaticConfigurations[name]);
			World.Setup(x => x.GetStaticInt(It.IsAny<string>())).Returns<string>(name =>
				int.Parse(DefaultStaticSettings.DefaultStaticConfigurations[name], CultureInfo.InvariantCulture));
			World.Setup(x => x.GetStaticDouble(It.IsAny<string>())).Returns<string>(name =>
				double.Parse(DefaultStaticSettings.DefaultStaticConfigurations[name], CultureInfo.InvariantCulture));
			Origin.Setup(x => x.OutdoorsType(Actor.Object)).Returns(RoomOutdoorsType.Outdoors);
			Origin.Setup(x => x.CurrentWeather(Actor.Object)).Returns(Weather.Object);
			Weather.SetupGet(x => x.Precipitation).Returns(PrecipitationLevel.Dry);
			Weather.SetupGet(x => x.LightLevelMultiplier).Returns(1.0);
			Weather.SetupGet(x => x.Hazards).Returns(new WeatherHazardSettings());
		}
		public void Dust(double multiplier)
		{
			Weather.SetupGet(x => x.Hazards).Returns(new WeatherHazardSettings { AtmosphereGasId = 42 });
			Weather.SetupGet(x => x.ObscuresViewOfSky).Returns(true);
			Weather.SetupGet(x => x.LightLevelMultiplier).Returns(multiplier);
		}
	}
}
