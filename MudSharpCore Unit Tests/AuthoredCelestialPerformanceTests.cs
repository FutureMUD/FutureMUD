#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MudSharp.Celestial;
using MudSharp.Celestial.Authored;
using MudSharp.Construction;
using MudSharp.TimeAndDate;

namespace MudSharp_Unit_Tests;

[TestClass]
public class AuthoredCelestialPerformanceTests
{
	public TestContext TestContext { get; set; } = null!;
	[TestMethod]
	public void NumericalAllocationsAndScaling_RecordMeasurementsAndAssertSingleEvaluation()
	{
		var rows = new List<object>();
		var small = AuthoredCelestialPresets.Create("RailSun", 1, 60, 24);
		var sparse = AuthoredCelestialPresets.Create("MorningStar", 1, 60, 24);
		var scale = 10000000L;
		sparse.Path.Period *= scale;
		sparse.Path.Keys = sparse.Path.Keys.Select(x => x with { Minute = x.Minute * scale }).ToList();
		var dense = AuthoredCelestialPresets.Create("ScriptedSun", 1, 60, 24);
		dense.Path.Mode = AuthoredPathMode.Dense; dense.Path.Period = 40000;
		dense.Path.Keys = Enumerable.Range(0, 40000).Select(i => new PositionKey(i, i % 360, 60 * Math.Sin(i * 2 * Math.PI / 40000))).ToList();
		foreach (var (name, definition) in new[] { ("small rail", small), ("long sparse", sparse), ("dense 40000", dense) })
		{
			var watch = Stopwatch.StartNew();
			var compiled = new CompiledAuthoredCelestial(definition, 60, 60, 24);
			var compileMilliseconds = watch.Elapsed.TotalMilliseconds;
			var cursor = new AuthoredCelestialCursor();
			var sum = 0.0;
			for (var i = 0; i < 10000; i++) sum += compiled.Evaluate(i * 60L, ref cursor).SourceLux;
			var allocated = GC.GetAllocatedBytesForCurrentThread();
			watch.Restart();
			for (var i = 0; i < 100000; i++) sum += compiled.Evaluate(i * 60L, ref cursor).SourceLux;
			var nanoseconds = watch.Elapsed.TotalNanoseconds / 100000;
			var bytes = GC.GetAllocatedBytesForCurrentThread() - allocated;
			Assert.AreEqual(0L, bytes, "Core numerical evaluation must not allocate.");
			rows.Add(new { operation = "evaluate", name, nanoseconds, allocatedBytes = bytes, calls = 100000, compileMilliseconds, compiled.StoredEntries, checksum = sum });
			foreach (var rank in new[] { 1L, 1000L, 1000000L })
			{
				var request = new CelestialEventRequest(AstronomicalEventType.Sunrise, rank);
				var reference = new MudInstant(MudInstant.CurrentEpoch, 0, 1, 1);
				for (var i = 0; i < 10000; i++) compiled.FindNext(reference, request);
				allocated = GC.GetAllocatedBytesForCurrentThread(); watch.Restart();
				for (var i = 0; i < 100000; i++) compiled.FindNext(reference, request);
				nanoseconds = watch.Elapsed.TotalNanoseconds / 100000; bytes = GC.GetAllocatedBytesForCurrentThread() - allocated;
				Assert.AreEqual(0L, bytes, "Direct recurring selection must not allocate.");
				rows.Add(new { operation = "nth event", name, rank, nanoseconds, allocatedBytes = bytes, calls = 100000, status = compiled.FindNext(reference, request).Status.ToString() });
			}
		}
		foreach (var count in new[] { 1, 100, 1000 })
		{
			var ctx = CelestialTestFactory.CreateEarthSystem(); ctx.SetDateTime("1/jan/2010", 8, 0, 0);
			using var c = AuthoredCelestial.Create(50, small, ctx.Clock, ctx.Gameworld);
			var locations = Enumerable.Range(0, count).Select(_ => new Mock<ILocation>().Object).ToArray();
			var wrappers = locations.Select(x => c.ReturnNewCelestialInformation(x, null!, ctx.ZeroGeography)).ToArray();
			c.MinuteUpdateEvent += sender =>
			{
				for (var i = 0; i < count; i++) { wrappers[i] = c.ReturnNewCelestialInformation(locations[i], wrappers[i], ctx.ZeroGeography); _ = c.CurrentIllumination(ctx.ZeroGeography); }
			};
			ctx.Clock.CurrentTime.AddSeconds(60);
			var start = c.IntrinsicEvaluationCount;
			var watch = Stopwatch.StartNew(); var allocated = GC.GetAllocatedBytesForCurrentThread();
			for (var i = 0; i < 60; i++) ctx.Clock.CurrentTime.AddSeconds(60);
			var bytes = GC.GetAllocatedBytesForCurrentThread() - allocated; var elapsed = watch.Elapsed.TotalMilliseconds;
			Assert.AreEqual(60L, c.IntrinsicEvaluationCount - start);
			rows.Add(new { operation = "live fanout with clock and mutable wrappers", zones = count, milliseconds = elapsed, allocatedBytes = bytes, updates = 60, intrinsicEvaluations = c.IntrinsicEvaluationCount - start });
			allocated = GC.GetAllocatedBytesForCurrentThread();
			for (var i = 0; i < 100000; i++) { _ = c.LiveState; _ = c.CurrentIllumination(ctx.ZeroGeography); }
			Assert.AreEqual(0L, GC.GetAllocatedBytesForCurrentThread() - allocated, "Warm live numerical reads must not allocate.");
		}
		var path = Path.Combine(AppContext.BaseDirectory, "authored-celestial-performance.json");
		File.WriteAllText(path, JsonSerializer.Serialize(rows, new JsonSerializerOptions { WriteIndented = true }));
		TestContext.AddResultFile(path);
		TestContext.WriteLine(path);
	}
}
