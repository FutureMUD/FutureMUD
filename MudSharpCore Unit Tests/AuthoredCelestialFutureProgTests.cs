#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Construction;
using MudSharp.Framework;
using MudSharp.FutureProg;
using MudSharp.FutureProg.Variables;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialFutureProgTests
{
	private sealed class Constant(IProgVariable value) : IFunction
	{
		public IProgVariable Result => value;
		public ProgVariableTypes ReturnType => value.Type;
		public string ErrorMessage => string.Empty;
		public StatementResult ExpectedResult => StatementResult.Normal;
		public StatementResult Execute(IVariableSpace variables) => StatementResult.Normal;
		public bool IsReturnOrContainsReturnOnAllBranches() => false;
	}

	[TestMethod]
	public void EveryRegisteredEventOverload_ExecutesAuthoredCapabilityThroughPublicCompiler()
	{
		FutureProgTestBootstrap.EnsureInitialised();
		var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 0, 0, 0);
		var sd = AuthoredCelestialPresets.Create("RailSun", 1, 60, 24); sd.SyntheticLongitude = true; sd.Milestones = [new("custom:test", 1440, 15)];
		using var sun = AuthoredCelestial.Create(51, sd, ctx.Clock, ctx.Gameworld);
		var md = AuthoredCelestialPresets.Create("RailMoon", 1, 60, 24); md.Milestones = [new("custom:crescent", 40320, 24000)];
		md.Phase!.CrescentSunId = sun.Id; md.Phase.CrescentMilestones.Add("custom:crescent");
		using var moon = AuthoredCelestial.Create(52, md, ctx.Clock, ctx.Gameworld);
		((All<ICelestialObject>)ctx.Gameworld.CelestialObjects).Add(sun); ((All<ICelestialObject>)ctx.Gameworld.CelestialObjects).Add(moon);
		var zone = new Mock<IZone>(); zone.SetupGet(x => x.GetObject).Returns(zone.Object);
		zone.SetupGet(x => x.Celestials).Returns(new ICelestialObject[] { sun, moon });
		zone.SetupGet(x => x.Geography).Returns(ctx.ZeroGeography);
		zone.Setup(x => x.TimeZone(ctx.Clock)).Returns(ctx.Clock.PrimaryTimezone);
		zone.Setup(x => x.GetInfo(sun)).Returns(() => sun.CurrentPosition(ctx.ZeroGeography));
		var cell = new Mock<ICell>(); cell.SetupGet(x => x.GetObject).Returns(cell.Object); cell.SetupGet(x => x.Zone).Returns(zone.Object);
		var functions = new Dictionary<string, AstronomicalEventType?>
		{
			["nextsunrise"] = AstronomicalEventType.Sunrise, ["nextsunset"] = AstronomicalEventType.Sunset,
			["nextnewmoon"] = AstronomicalEventType.NewMoon, ["nextfullmoon"] = AstronomicalEventType.FullMoon,
			["nextsolarlongitude"] = AstronomicalEventType.SolarLongitude, ["nextvisiblecrescent"] = AstronomicalEventType.VisibleCrescent,
			["nextcelestialevent"] = null
		};
		var count = 0;
		foreach (var info in FutureProg.GetFunctionCompilerInformations().Where(x => functions.ContainsKey(x.FunctionName)))
		{
			var types = info.Parameters.ToArray();
			var type = functions[info.FunctionName];
			var primary = type is AstronomicalEventType.NewMoon or AstronomicalEventType.FullMoon ? moon : sun;
			var parameters = new List<IFunction>();
			var baseCount = type is AstronomicalEventType.VisibleCrescent or AstronomicalEventType.SolarLongitude || type is null ? 4 : 3;
			var rank = types.Length > baseCount ? 1000 : 1;
			for (var i = 0; i < types.Length; i++)
			{
				IProgVariable value = i switch
				{
					0 => types[0] == ProgVariableTypes.Zone ? zone.Object : cell.Object,
					1 => types[1] == ProgVariableTypes.Number ? new NumberVariable(primary.Id) : primary,
					2 when type == AstronomicalEventType.VisibleCrescent => types[2] == ProgVariableTypes.Number ? new NumberVariable(moon.Id) : moon,
					_ when types[i] == ProgVariableTypes.Calendar => ctx.Calendar,
					_ when types[i] == ProgVariableTypes.Text => new TextVariable("custom:test"),
					3 when type == AstronomicalEventType.SolarLongitude => new NumberVariable(90),
					_ => new NumberVariable(rank)
				};
				parameters.Add(new Constant(value));
			}
			var function = info.CompilerFunction(parameters, ctx.Gameworld);
			Assert.AreEqual(StatementResult.Normal, function.Execute(null!), info.FunctionName);
			var actual = (MudDateTime)function.Result.GetObject;
			Assert.IsNotNull(actual.Date, info.FunctionName);
			var expected = (type == AstronomicalEventType.VisibleCrescent ? moon : primary).FindNext(ctx.Calendar.CurrentInstant,
				new(type, rank, Math.PI / 2, type is null ? "custom:test" : null, type == AstronomicalEventType.VisibleCrescent ? sun.Id : null));
			Assert.AreEqual(expected.Instant.Ticks, MudInstant.FromMudDateTime(actual).Ticks, info.FunctionName);
			if (rank > 1)
			{
				foreach (var invalid in new[] { -1m, 0m, 1.5m, (decimal)int.MaxValue + 1 })
				{
					parameters[^1] = new Constant(new NumberVariable(invalid));
					var bad = info.CompilerFunction(parameters, ctx.Gameworld); bad.Execute(null!);
					Assert.IsNull(((MudDateTime)bad.Result.GetObject).Date, $"{info.FunctionName} accepted {invalid}");
				}
			}
			count++;
		}
		Assert.IsTrue(count >= 56, $"Only executed {count} overloads.");
		foreach (var name in new[] { "moonphase", "celestialelevation" })
		{
			foreach (var info in FutureProg.GetFunctionCompilerInformations().Where(x => x.FunctionName == name))
			{
				var types = info.Parameters.ToArray();
				var args = new List<IFunction> { new Constant(types[0] == ProgVariableTypes.Zone ? zone.Object : cell.Object) };
				if (args.Count < types.Length) args.Add(new Constant(types[1] == ProgVariableTypes.Number ? new NumberVariable(sun.Id) : sun));
				var f = info.CompilerFunction(args, ctx.Gameworld); Assert.AreEqual(StatementResult.Normal, f.Execute(null!));
				if (name == "moonphase") Assert.AreEqual(MoonPhase.Full.Describe(), f.Result.GetObject);
				else Assert.AreEqual(sun.LiveState.Elevation, Convert.ToDouble(f.Result.GetObject), 1e-12);
			}
		}
	}
}
